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

namespace FFNightClub.Modules
{
    public class GambaRPGGame
    {
        private readonly MainWindow MainWindow;
        private List<Player> players; //list of all players
        private List<RPGPlayer> RPGPlayers; //list of all players
        private string[] statList = { "Fighty","Sneaky","Brainy","Talky","Lusty"};
        private RPGEvent CurrentRPGEvent;
        private RPGEventOption CurrentRPGEventOption;
        private Stats statSelected;
        private RPGPlayer CurrentPlayer;
        private static Random rng = new Random();
        private bool printMatches;
        private string debug = "not recieved";
        private int LastRoll = 0;
        private int currentPlayerNumber;
        private List<string> QueuedMessages = new List<string>();
        private int Roll;
        private RPGEventOption chosenOption;

        private Event CurrentEvent = Event.ConfigureSettings;
        private GameEvent CurrentGameEvent = GameEvent.WaitingForNextEvent;
        private enum Event
        { ConfigureSettings, TakePayments, CharacterCreation, GameRunning }
        private enum GameEvent
        { WaitingForNextEvent, EventStarted, ProcessingEvent, EventFinished }

        public GambaRPGGame(MainWindow mainWindow)
        {
            MainWindow = mainWindow;
            RPGPlayers = new List<RPGPlayer>()
            {
            };
            Initialize();
        }

        public void Initialize()
        {
            RPGPlayers = new List<RPGPlayer>()
            {
            };
            CurrentEvent = Event.ConfigureSettings;
            CurrentGameEvent = GameEvent.WaitingForNextEvent;
        }       

        public void DrawMatch()
        {
            if (ImGui.Button("Start New Game"))
            {
                SendMessage($"{MainWindow.Config.GambaRPGConfig.Messages["ExplainRules"]}");
                StartGame();
            }
            ImGui.Separator();

            if (CurrentEvent == Event.ConfigureSettings)
            {             
                ImGui.Text("Configure Game : ");
                DrawConfigureGame();
            }

            if (CurrentEvent == Event.TakePayments)
            {
                ImGui.Text("Take Player Payments :");
                DrawTakePayments();
            }
           
            if (CurrentEvent == Event.CharacterCreation)
            {
                ImGui.Separator();
                ImGui.Text("Set Player Stats :");
                DrawPlayerCharacterCreation();
            }

            if (CurrentEvent == Event.GameRunning)
            {

                ImGui.Separator();
                ImGui.Text("Players");
                DrawPlayers();
                ImGui.Separator();
                ImGui.Text("Event:");
                if (CurrentGameEvent == GameEvent.WaitingForNextEvent)
                {
                    if (ImGui.Button("Start Next Event"))
                    {
                        CurrentPlayer = RPGPlayers[0];                       
                        CurrentRPGEvent = new RPGEvent();
                        SendMessage($"{CurrentRPGEvent.IntroText}");
                        SendOptionRoll();                     
                        CurrentRPGEventOption = CurrentRPGEvent.Options.Where(o => o.Stat == (Stats)(Roll)).FirstOrDefault();
                        if (CurrentRPGEventOption == null)
                        {
                            CurrentRPGEventOption = CurrentRPGEvent.Options[0];
                        }
                        CurrentGameEvent = GameEvent.EventStarted;
                    }
                }             
            }

            if (CurrentGameEvent == GameEvent.EventStarted)
            {
                DrawEventDetails();
            }

            if (CurrentGameEvent == GameEvent.ProcessingEvent)
            {
                if (ImGui.Button("Start Next Event"))
                {
                    SendMessage($"{MainWindow.Config.GambaRPGConfig.Messages["NewEvent"]}");
                    CurrentPlayer = RPGPlayers[0];
                    CurrentRPGEvent = new RPGEvent();

                    SendOptionRoll();
                   
                    CurrentGameEvent = GameEvent.EventStarted;
                }
                DrawEventOutcome();
            }

            if (QueuedMessages.Count > 0)
            {
                FFNightClub.Chat.SendMessage(QueuedMessages[0]);
                QueuedMessages.RemoveAt(0);
            }
        }

        public void StartGame()
        {
            Initialize();
            CurrentEvent = Event.ConfigureSettings;
            players = new List<Player>(MainWindow.PlayerList.Players.Where(p => p.enabled).ToList());
            RPGPlayers = new List<RPGPlayer>();
            foreach (Player p in players) {
                RPGPlayers.Add(new RPGPlayer(p.ID,p.Name,p.Alias));
            }
        }

        private void DrawConfigureGame()
        {
            if (ImGui.Button("Go To Take Payments"))
            {
                SendMessage($"{MainWindow.Config.GambaRPGConfig.Messages["TakePayments"]}");
                CurrentEvent = Event.TakePayments;
            }
        }

        private void DrawTakePayments()
        {
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
            ImGui.BeginTable("PRG Player List", 8, ImGuiTableFlags.Reorderable | ImGuiTableFlags.Resizable);
            ImGui.TableSetupColumn("Name");
            ImGui.TableSetupColumn("HP");
            ImGui.TableSetupColumn("Money");
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
                ImGui.Text(player.HP.ToString());
                ImGui.TableNextColumn();
                ImGui.Text(player.Money.ToString());
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
                
            }
            ImGui.TableNextColumn();
            ImGui.Text(CurrentRPGEventOption.Stat.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(CurrentRPGEventOption.SkillCheck.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(CurrentRPGEventOption.Description.ToString());
            ImGui.TableNextRow();
            ImGui.EndTable();
        }
        // UTILSSSS

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
            }
        }

        public void CheckPlayerRoll(string sender, string message)
        {
            try
            {
                LastRoll = int.Parse(MainWindow.Config.RollCommand == "/dice" ? message.Replace("Random! (1-999) ", "") : Regex.Replace(message, ".*You roll a ([^\\(]+)\\(.*", "$1", RegexOptions.Singleline).Trim());
                if (CurrentPlayer.Roll == -1)
                {
                    CurrentPlayer.Roll = LastRoll;
                    SendMessage($"{FormatMessage(MainWindow.Config.TruthOrDareConfig.Messages["OptionsRolled"], CurrentPlayer)}");
                }
            }
            catch { }
        }

        public void CheckRollForOptions(string sender, string message)
        {
            try
            {
                var str = message.Replace("Random! (1-5) ", "");
                Roll = int.Parse(str);
                Roll -= 1;
                FFNightClub.Log.Debug($"+++ {(Stats)(Roll)} +++ {Roll}");
                FFNightClub.Log.Debug($"+++ {CurrentRPGEvent.Options.Where(o => o.Stat == (Stats)(Roll)).FirstOrDefault().Description}");
                CurrentRPGEventOption = CurrentRPGEvent.Options.Where(o => o.Stat == (Stats)(Roll)).FirstOrDefault();
                if (CurrentRPGEventOption == null)
                {
                    CurrentRPGEventOption = CurrentRPGEvent.Options[0];
                }
                SendMessage($"{FormatMessage(MainWindow.Config.GambaRPGConfig.Messages["OptionsRolled"], CurrentPlayer)}");
                SendMessage($"{CurrentRPGEventOption.Description}");
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
            CheckRollForOptions(sender, message);
        }

        private string FormatMessage(string message, RPGPlayer player)
        {
            debug = "Here" + message + player;
            if (string.IsNullOrWhiteSpace(message)) { return ""; }

            return message.Replace("#dealer#", player.Alias)
                .Replace("#player#", player.Alias)
                .Replace("#availableOption#", ((Stats)Roll).ToString() )
                .Replace("#roll#", player.Roll.ToString());
        }

        private void SendMessage(string message)
        {
            debug = "here" + message;
            SendMessageToQueue(message, MessageType.Normal);
        }

        private void SendOptionRoll()
        {
            SendMessageToQueue(MainWindow.Config.GambaRPGConfig.OptionRoll.ToString(), MessageType.RPGOptionSelectRoll);
        }
    }
}
