using Dalamud.Game.Text;
using Dalamud.Interface.Utility;
using FFNightClub;
using FFNightClub.Extensions;
using FFNightClub.Models;
using FFNightClub.Windows;
using System.Numerics;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Lumina.Data.Parsing.Layer.LayerCommon;
using FFNightclub.Models;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using static FFXIVClientStructs.FFXIV.Client.UI.Misc.GroupPoseModule;
using System.Diagnostics;
using Dalamud.Logging;
using Dalamud.Plugin.Services;
using Dalamud.Logging.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;
using FFNightclub.Config;

namespace FFNightClub.Modules
{
    public class GambaRPGGame
    {
        private readonly MainWindow MainWindow;
        private List<string> QueuedMessages = new List<string>();
        private enum Event
        { ConfigureSettings, TakePayments, CharacterCreation, GameRunning, GameOver }
        private Event CurrentEvent = Event.ConfigureSettings;
        private MessageType LastRoll = MessageType.RPGOptionSelectRoll;
        //Players
        private List<Player> players;
        private List<RPGPlayer> RPGPlayers;

        //Config
        private int NumberOfRounds = 3;
        private int CurrentRound = 0;
        private int MaxRerolls = 3;
        private string MaxRerollsString = "3";
        private string NumberOfRoundsString = "3";
        //Character Creation
        private string[] statList = { "Fighty", "Sneaky", "Brainy", "Talky", "Lusty" };
        private Stats statSelected;

        //Game
        private enum GameEvent
        { WaitingForNextEvent, EventStarted, ProcessingEvent, RoundFinished }
        private GameEvent CurrentGameEvent = GameEvent.WaitingForNextEvent;
        private List<RPGEvent> RPGEvents = new List<RPGEvent>();
        private RPGEvent CurrentRPGEvent;
        private RPGEventOption CurrentRPGEventOption;
        private RPGPlayer CurrentPlayer;
        private static Random rng = new Random();
        private string debug = "not recieved";
        private int Roll;
        private List<RPGPlayer> RoundRPGPlayers;
        private float CurrentMoneyGain = 1;

        public GambaRPGGame(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            Initialize();
        }

        public void Initialize()
        {
            RPGPlayers = new List<RPGPlayer>();
            CurrentEvent = Event.ConfigureSettings;
            CurrentRound = 0;
            CurrentGameEvent = GameEvent.WaitingForNextEvent;
            CurrentPlayer = new RPGPlayer();
            MaxRerolls = 3;
            NumberOfRounds = 3;
        }

        private void GenerateRPGEvents()
        {
            RPGEvents = MainWindow.Config.GambaRPGConfig.Events;
        }

        public void DrawMatch()
        {
            if (ImGui.Button("Start New Game"))
            {
                GenerateRPGEvents();
                StartGame();
            }

            ImGui.Separator();

            if (CurrentEvent == Event.ConfigureSettings)
            {                          
                DrawConfigureGame();
            }

            if (CurrentEvent == Event.TakePayments)
            {
                DrawTakePayments();
            }
           
            if (CurrentEvent == Event.CharacterCreation)
            {
                DrawPlayerCharacterCreation();
            }

            if (CurrentEvent == Event.GameRunning)
            {
                DrawCommonGame();
              
                if (CurrentGameEvent == GameEvent.WaitingForNextEvent)
                {
                    DrawStartNextEvent();
                }
                if (CurrentGameEvent == GameEvent.EventStarted)
                {
                    DrawEventDetails();
                }
                if (CurrentGameEvent == GameEvent.ProcessingEvent)
                {
                    DrawStartNextEvent();
                    DrawEventOutcome();
                }
            }

            if (CurrentEvent == Event.GameOver)
            {
                DrawGameOver();
            }

            if (QueuedMessages.Count > 0)
            {
                FFNightClub.Chat.SendMessage(QueuedMessages[0]);
                QueuedMessages.RemoveAt(0);
            }
        }

        private void DrawGameOver()
        {
            ImGui.Text("Take Player Payments :");
            ImGui.BeginTable("GameOver", 4, ImGuiTableFlags.Reorderable | ImGuiTableFlags.Resizable);
            ImGui.TableSetupColumn("Name");
            ImGui.TableSetupColumn("Starting Money");
            ImGui.TableSetupColumn("Ending Money");
            ImGui.TableSetupColumn("Paid?");
            ImGui.TableHeadersRow();
            ImGui.TableNextRow();
            foreach (var player in RPGPlayers)
            {
                ImGui.TableNextColumn();
                ImGui.Text(player.Name);
                ImGui.TableNextColumn();
                ImGui.Text(string.Format("{0:0,0}", player.StartingMoney));
                ImGui.TableNextColumn();
                ImGui.Text(string.Format("{0:0,0}", player.Money));
                ImGui.TableNextColumn();
                ImGui.Checkbox($"{player.Name} Paid?", ref player.hasBeenPaid);
                ImGui.TableNextRow();
            }
            ImGui.EndTable();
        }

        private void DrawStartNextEvent()
        {
            if (ImGui.Button("Start Next Event"))
            {
                SendMessage("**  NEW EVENT HAPPENING!  <se.12> **");
                DecideNewEvent();             
            }
        }

        private void DecideNewEvent()
        {
            if (RoundRPGPlayers.Count < 1)
            {             
                FFNightClub.Log.Debug("Next Round");
                CurrentRound++;
                if (CurrentRound > NumberOfRounds)
                {
                    FFNightClub.Log.Debug("Ending Game");
                    EndGame();
                }
                else
                {
                    RoundRPGPlayers = new List<RPGPlayer>();
                    foreach (var player in RPGPlayers)
                    {
                        RoundRPGPlayers.Add(new RPGPlayer
                        {
                            Name = player.Name
                        });
                    }
                    SendMessage($"{FormatMessage(MainWindow.Config.GambaRPGConfig.Messages["NewRound"], CurrentPlayer)}");
                    FFNightClub.Log.Debug("Next Event");
                    StartNextEvent();
                }
            } else
            {
                SendMessage("**  NEW EVENT HAPPENING!  <se.12> **");
                FFNightClub.Log.Debug("Next Event");
                StartNextEvent();
            }               
            
        }

        private void StartNextEvent()
        {
            CurrentRPGEvent = RPGEvents[0]; //GENERATE EVENTS
            CurrentRPGEventOption = CurrentRPGEvent.Options.Where(o => o.Stat == (Stats)(Roll)).FirstOrDefault();
            if (CurrentRPGEventOption == null)
            {
                CurrentRPGEventOption = CurrentRPGEvent.Options[0];
            }
            var playerIndex = (Int32)rng.NextInt64(RoundRPGPlayers.Count);
            FFNightClub.Log.Debug($"{playerIndex}");
            var playerRef = RoundRPGPlayers[playerIndex];
            CurrentPlayer = RPGPlayers.Where(p => p.Name.Equals(playerRef.Name)).FirstOrDefault();
            FFNightClub.Log.Debug($"++++++++ {CurrentPlayer.Name}");
            RoundRPGPlayers.Remove(playerRef);
            SendMessage(FormatMessage($"**  #player#  {CurrentRPGEvent.IntroText} **", CurrentPlayer));
            SendOptionRoll();
            CurrentGameEvent = GameEvent.EventStarted;
        }

        private void RerollEvent()
        {        
            CurrentRPGEventOption = CurrentRPGEvent.Options.Where(o => o.Stat == (Stats)(Roll)).FirstOrDefault();
            if (CurrentRPGEventOption == null)
            {
                CurrentRPGEventOption = CurrentRPGEvent.Options[0];
            }          
            SendMessage(FormatMessage($"**  #player#  {CurrentRPGEvent.IntroText} **", CurrentPlayer));
            SendOptionRoll();
            CurrentGameEvent = GameEvent.EventStarted;
        }

        private void EndGame()
        {
            SendMessage($"{MainWindow.Config.GambaRPGConfig.Messages["GameOver"]}");
            CurrentGameEvent = GameEvent.WaitingForNextEvent;
            CurrentEvent = Event.GameOver;
        }

        private void DrawCommonGame()
        {
            ImGui.Separator();
            DrawPlayers();
            ImGui.Separator();
        }

        public void StartGame()
        {
            SendMessage($"{MainWindow.Config.GambaRPGConfig.Messages["ExplainRules"]}");
            Initialize();
            CurrentEvent = Event.ConfigureSettings;
            players = new List<Player>(MainWindow.PlayerList.Players.Where(p => p.enabled).ToList());
            RPGPlayers = new List<RPGPlayer>();
            foreach (Player p in players) {
                RPGPlayers.Add(new RPGPlayer(p.ID,p.Name,p.Alias));
            }
            RoundRPGPlayers = new List<RPGPlayer>();
            foreach (var player in RPGPlayers)
            {
                player.ReRolls = MaxRerolls;
                RoundRPGPlayers.Add(new RPGPlayer
                {
                    Name = player.Name
                });
            }
            CurrentPlayer = RPGPlayers[0];
        }

        private void DrawConfigureGame()
        {
            ImGui.Text("Configure Game : ");
            if (ImGui.Button("Go To Take Payments"))
            {
                int.TryParse(NumberOfRoundsString, out NumberOfRounds);
                int.TryParse(MaxRerollsString,out MaxRerolls);
                foreach (var player in RPGPlayers)
                {
                    player.ReRolls = MaxRerolls;
                }
                SendMessage($"{MainWindow.Config.GambaRPGConfig.Messages["TakePayments"]}");
                CurrentEvent = Event.TakePayments;
            }
            ImGui.BeginTable("Config", 2, ImGuiTableFlags.Reorderable | ImGuiTableFlags.Resizable);
            ImGui.TableSetupColumn("Rounds");
            ImGui.TableSetupColumn("Player Rerolls");
            ImGui.TableHeadersRow();
            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.InputText($"###NumberOfRounds", ref NumberOfRoundsString, 255);
            ImGui.TableNextColumn();
            ImGui.InputText($"###MaxRerolls", ref MaxRerollsString, 255); 
            ImGui.TableNextRow();
            
            ImGui.EndTable();
        }

        private void DrawTakePayments()
        {
            ImGui.Text("Take Player Payments :");
            ImGui.BeginTable("PRG Player Payment List", 3, ImGuiTableFlags.Reorderable | ImGuiTableFlags.Resizable);
            ImGui.TableSetupColumn("Name");
            ImGui.TableSetupColumn("Starting Money");
            ImGui.TableSetupColumn("Paid?");
            ImGui.TableHeadersRow();
            ImGui.TableNextRow();
            foreach (var player in RPGPlayers)
            {
                ImGui.TableNextColumn();
                ImGui.Text(player.Name);
                ImGui.TableNextColumn();
                ImGui.InputText($"###{player.Name}startingGil",ref player.StartingMoney, 255);
                ImGui.TableNextColumn();
                ImGui.Checkbox($"{player.Name} Paid?", ref player.hasPaid);
                ImGui.TableNextRow();
            }
            ImGui.EndTable();
            if (ImGui.Button("Go To Character Creation"))
            {
                bool allPaid = true;
                foreach (var player in RPGPlayers)
                {
                    if (player.hasPaid == false)
                    {
                        allPaid = false;
                    }
                }
                if (allPaid)
                {
                    foreach (var player in RPGPlayers)
                    {
                        player.Money = float.Parse(player.StartingMoney);
                    }
                    SendMessage($"{MainWindow.Config.GambaRPGConfig.Messages["CharacterCreation"]}");
                    CurrentEvent = Event.CharacterCreation;

                }
            }
        }

        private void DrawPlayerCharacterCreation()
        {
            ImGui.Separator();
            ImGui.Text("Set Player Stats :");
            if (ImGui.Button("Complete Creation"))
            {
                bool allSet = true;
                foreach (var player in RPGPlayers)
                {
                    player.statStartingList.Clear();
                    player.statStartingList.Add(player.Stat1);
                    player.statStartingList.Add(player.Stat2);
                    player.statStartingList.Add(player.Stat3);
                    player.statStartingList.Add(player.Stat4);
                    player.statStartingList.Add(player.Stat5);
                    bool isUnique = player.statStartingList.Distinct().Count() == player.statStartingList.Count();  
                    if (!isUnique)
                    {
                        allSet = false;
                    }
                }
                if (allSet)
                {
                    foreach (var player in RPGPlayers)
                    {
                        player.RPGStats = new RPGStats();
                        player.RPGStats.SetValue((Stats)player.Stat1,2);
                        player.RPGStats.SetValue((Stats)player.Stat2, 1);
                        player.RPGStats.SetValue((Stats)player.Stat3, 0);
                        player.RPGStats.SetValue((Stats)player.Stat4, -1);
                        player.RPGStats.SetValue((Stats)player.Stat5, -2);
                    }
                    SendMessage($"{MainWindow.Config.GambaRPGConfig.Messages["StartGame"]}");
                    CurrentEvent = Event.GameRunning;

                }
            }
            ImGui.BeginTable("PRG Player List", 6, ImGuiTableFlags.ScrollY | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Resizable);
            ImGui.TableSetupColumn("Name");
            ImGui.TableSetupColumn("+2");
            ImGui.TableSetupColumn("+1");
            ImGui.TableSetupColumn("+0");
            ImGui.TableSetupColumn("-1");
            ImGui.TableSetupColumn("-2");
            ImGui.TableHeadersRow();
            ImGui.TableNextRow();
            foreach (var player in RPGPlayers)
            {
                ImGui.TableNextColumn();
                ImGui.Text(player.Name);
                ImGui.TableNextColumn();
                ImGui.ListBox($"##{player.Name}FirstChoice",ref player.Stat1, statList,5,5);
                ImGui.TableNextColumn();
                ImGui.ListBox($"##{player.Name}SecondChoice", ref player.Stat2, statList, 5, 5);
                ImGui.TableNextColumn();
                ImGui.ListBox($"##{player.Name}ThirdChoice", ref player.Stat3, statList, 5, 5);
                ImGui.TableNextColumn();
                ImGui.ListBox($"##{player.Name}FourthChoice", ref player.Stat4, statList, 5, 5);
                ImGui.TableNextColumn();
                ImGui.ListBox($"##{player.Name}FithChoice", ref player.Stat5, statList, 5, 5);
                ImGui.TableNextRow();
            }
            ImGui.EndTable();
        }

        private void DrawPlayers()
        {
            ImGui.Text("Players");
            ImGui.BeginTable("PRG Player List", 9, ImGuiTableFlags.Reorderable | ImGuiTableFlags.Resizable);
            ImGui.TableSetupColumn("Name");
            ImGui.TableSetupColumn("Send info to chat?");
            ImGui.TableSetupColumn("Money");
            ImGui.TableSetupColumn("Rerolls");
            ImGui.TableSetupColumn("Fighty");
            ImGui.TableSetupColumn("Sneaky");
            ImGui.TableSetupColumn("Brainy");
            ImGui.TableSetupColumn("Talky");
            ImGui.TableSetupColumn("Lusty");
            ImGui.TableHeadersRow();
            ImGui.TableNextRow();
            foreach (var player in RPGPlayers)
            {
                ImGui.TableNextColumn();
                ImGui.Text(player.Name);
                ImGui.TableNextColumn();
                if (ImGui.Button($"Send Info for {player.Name}"))
                {               
                    SendPlayerRoll();
                    SendMessage($"**  {player.Name}  : GIL {player.Money}/{player.StartingMoney} : REROLLS {player.ReRolls}/{MaxRerolls} : STATS FIGHTY {player.RPGStats.Fighty.Value}, SNEAKY {player.RPGStats.Fighty.Value}, BRAINY {player.RPGStats.Fighty.Value}, TALKY {player.RPGStats.Fighty.Value} and LUSTY {player.RPGStats.Fighty.Value} **");
                }
                ImGui.TableNextColumn();
                ImGui.Text(string.Format("{0:0,0}", player.Money.ToString()) + " / " + string.Format("{0:0,0}", player.StartingMoney));
                ImGui.TableNextColumn();
                ImGui.Text(player.ReRolls.ToString());
                ImGui.TableNextColumn();
                ImGui.Text(player.RPGStats.Fighty.Value.ToString());
                ImGui.TableNextColumn();
                ImGui.Text(player.RPGStats.Sneaky.Value.ToString());
                ImGui.TableNextColumn();
                ImGui.Text(player.RPGStats.Brainy.Value.ToString());
                ImGui.TableNextColumn();
                ImGui.Text(player.RPGStats.Talky.Value.ToString());
                ImGui.TableNextColumn();
                ImGui.Text(player.RPGStats.Lusty.Value.ToString());
                ImGui.TableNextRow();
            }
            ImGui.EndTable();
        }

        private void DrawEventOutcome()
        {
            ImGui.Text($"Current Player(s) : {CurrentPlayer.Name}");
            ImGui.Text($"Event Name : {CurrentRPGEvent.Name}");
            ImGui.Text($"Description : {CurrentRPGEvent.IntroText}");    
        }

        private void DrawEventDetails()
        {         
            ImGui.Text($"Current Player(s) : {CurrentPlayer.Name}");
            ImGui.Text($"Event Name : {CurrentRPGEvent.Name}");
            ImGui.Text($"Description : {CurrentRPGEvent.IntroText}");
            ImGui.Text($"Options :");
            ImGui.BeginTable("Options", 4, ImGuiTableFlags.Reorderable | ImGuiTableFlags.Resizable);
            ImGui.TableSetupColumn("Select This Option?");
            ImGui.TableSetupColumn("Stat Used");
            ImGui.TableSetupColumn("Skill Check");
            ImGui.TableSetupColumn("Description");
            ImGui.TableHeadersRow();
            ImGui.TableNextRow();        
            ImGui.TableNextColumn();
            if (ImGui.Button($"Use {CurrentRPGEventOption.Stat}"))
            {
                statSelected = CurrentRPGEventOption.Stat;
                CurrentGameEvent = GameEvent.ProcessingEvent;           
                SendPlayerRoll();
            }
            ImGui.TableNextColumn();
            ImGui.Text(CurrentRPGEventOption.Stat.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(CurrentRPGEventOption.SkillCheck.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(CurrentRPGEventOption.Description.ToString());
            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            if (ImGui.Button($"Reroll {CurrentPlayer.ReRolls}/{MaxRerolls}"))
            {
                if (CurrentPlayer.ReRolls > 0)
                {
                    CurrentPlayer.ReRolls--;
                    statSelected = CurrentRPGEventOption.Stat;
                    CurrentGameEvent = GameEvent.ProcessingEvent;
                    SendMessage($"{FormatMessage(MainWindow.Config.GambaRPGConfig.Messages["Reroll"], CurrentPlayer)}");
                    RerollEvent();
                }
            }
            ImGui.TableNextColumn();
            ImGui.Text("New Stat");
            ImGui.TableNextColumn();
            ImGui.Text("?");
            ImGui.TableNextColumn();
            ImGui.Text("Reroll for a different event option");
            ImGui.TableNextRow();
            ImGui.EndTable();
        }

        private void SendMessageToQueue(string message, MessageType messageType)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                if (messageType == MessageType.Normal)
                {
                    QueuedMessages.Add($"{(MainWindow.Config.Debug ? "/echo" : "/p")} {message}");
                }
                else if (messageType == MessageType.RPGOptionSelectRoll)
                {
                    int.TryParse(message, out int num);
                    QueuedMessages.Add($"{MainWindow.Config.RollCommand} {(num == 0 ? MainWindow.Config.GambaRPGConfig.OptionRoll : num)}");
                }
                else if (messageType == MessageType.RPGPlayerRoll)
                {
                    int.TryParse(message, out int num);
                    QueuedMessages.Add($"{MainWindow.Config.RollCommand} {(num == 0 ? MainWindow.Config.GambaRPGConfig.PlayerRoll : num)}");
                }
            }
        }

        public void CheckPlayerRoll(string sender, string message)
        {
            FFNightClub.Log.Debug("Check Player Roll");
            try
            {
                CurrentPlayer.Roll = int.Parse(message.Replace("Random! (1-20) ", ""));                
                SendMessage($"{FormatMessage(MainWindow.Config.GambaRPGConfig.Messages["PlayerRoll"], CurrentPlayer)}");
                FFNightClub.Log.Debug($"Adjusted Roll : {CurrentPlayer.Roll + CurrentPlayer.RPGStats.GetValue(statSelected)}");
                
                if (CurrentPlayer.Roll+CurrentPlayer.RPGStats.GetValue(statSelected) >= CurrentRPGEventOption.SkillCheck) {
                    SendMessage($"{FormatMessage(MainWindow.Config.GambaRPGConfig.Messages["OptionSuccess"], CurrentPlayer)}");
                    CurrentPlayer.Money = CurrentPlayer.Money + CurrentMoneyGain;
                    SendMessage($"{FormatMessage(MainWindow.Config.GambaRPGConfig.Messages["MoneyGain"], CurrentPlayer)}");
                }
                else
                {
                    SendMessage($"{FormatMessage(MainWindow.Config.GambaRPGConfig.Messages["OptionFailure"], CurrentPlayer)}");
                    CurrentPlayer.Money = CurrentPlayer.Money - CurrentMoneyGain;
                    SendMessage($"{FormatMessage(MainWindow.Config.GambaRPGConfig.Messages["MoneyLoss"], CurrentPlayer)}");
                }
            }
            catch { }
        }

        public void CheckRollForOptions(string sender, string message)
        {
            FFNightClub.Log.Debug("Check options Roll");
            try
            {
                var str = message.Replace("Random! (1-5) ", "");
                Roll = int.Parse(str);
                Roll -= 1;            
                CurrentRPGEventOption = CurrentRPGEvent.Options.Where(o => o.Stat == (Stats)(Roll)).FirstOrDefault();
                if (CurrentRPGEventOption == null)
                {
                    CurrentRPGEventOption = CurrentRPGEvent.Options[0];
                }
                CurrentMoneyGain = float.Parse(CurrentPlayer.StartingMoney) * ((CurrentRPGEventOption.SkillCheck/20)/2);
                SendMessage($"{FormatMessage(MainWindow.Config.GambaRPGConfig.Messages["OptionsRolled"], CurrentPlayer)}");
                SendMessage($"{FormatMessage(MainWindow.Config.GambaRPGConfig.Messages["PlayerDecided"], CurrentPlayer)}");
                SendMessage($"{FormatMessage(MainWindow.Config.GambaRPGConfig.Messages["AskPlayerOption"], CurrentPlayer)}");
            }
            catch { }
        }

        public void OnChatMessage(XivChatType type, uint senderId, ref Dalamud.Game.Text.SeStringHandling.SeString sender, ref Dalamud.Game.Text.SeStringHandling.SeString message, ref bool isHandled)
        {
            if (isHandled) { return; }

            ReceivedMessage(sender.TextValue, message.TextValue);
        }

        private void ReceivedMessage(string sender, string message)
        {
            FFNightClub.Log.Debug("Recived Roll");
            if (LastRoll == MessageType.RPGOptionSelectRoll) {
                CheckRollForOptions(sender, message);
            } else
            {
                CheckPlayerRoll(sender, message);
            }

        }

        private string FormatMessage(string message, RPGPlayer player)
        {
            FFNightClub.Log.Debug($"Formatting message : {message} for {player.Name}");
            debug = "Here" + message + player;
            if (string.IsNullOrWhiteSpace(message)) { return ""; }

            return message.Replace("#dealer#", player.Alias)
                .Replace("#player#", player.Alias)
                .Replace("#availableOption#", ((Stats)Roll).ToString().ToUpper())
                .Replace("#optionDC#", CurrentRPGEventOption.SkillCheck.ToString())
                .Replace("#roll#", CurrentPlayer.Roll.ToString())
                .Replace("#playerSkill#", player.RPGStats.GetValue((Stats)Roll) >= 0 ? "+" + player.RPGStats.GetValue((Stats)Roll).ToString() : player.RPGStats.GetValue((Stats)Roll).ToString())
                .Replace("#adjustedRoll#", (CurrentPlayer.Roll + CurrentPlayer.RPGStats.GetValue((Stats)Roll)).ToString())
                .Replace("#description#", CurrentRPGEventOption.Description)
                .Replace("#optionFailure#", CurrentRPGEventOption.FailureDescription)
                .Replace("#optionSuccess#", CurrentRPGEventOption.SuccessDescription)
                .Replace("#maxRerolls#", MaxRerolls.ToString())
                .Replace("#playerMoney#", string.Format("{0:0,0}", CurrentPlayer.Money))
                .Replace("#moneyGain#", string.Format("{0:0,0}", CurrentMoneyGain))
                .Replace("#PlayerRerolls#", player.ReRolls.ToString())
                .Replace("#currentRound#", CurrentRound.ToString())
                .Replace("#startingRound#", NumberOfRounds.ToString());
        }

        private void SendMessage(string message)
        {
            debug = "here" + message;
            SendMessageToQueue(message, MessageType.Normal);
        }

        private void SendOptionRoll()
        {
            LastRoll = MessageType.RPGOptionSelectRoll;
            SendMessageToQueue(MainWindow.Config.GambaRPGConfig.OptionRoll.ToString(), MessageType.RPGOptionSelectRoll);
        }
        private void SendPlayerRoll()
        {
            LastRoll = MessageType.RPGPlayerRoll;
            SendMessageToQueue(MainWindow.Config.GambaRPGConfig.PlayerRoll.ToString(), MessageType.RPGPlayerRoll);
        }
    }
}
