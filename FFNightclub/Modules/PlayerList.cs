using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Interface;
using Dalamud.IoC;
using Dalamud.Utility;
using FFNightClub.Models;
using FFNightClub.Windows;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Utility;
using Dalamud.Logging;
using FFNightClub.Config;


namespace FFNightClub.Modules
{
    public class PlayerList
    {
        private readonly MainWindow MainWindow;
        public bool Enabled = false;
        public List<Player> Players;
        public Player newPlayer;
        public PlayerManager PlayerManager;
        public Player Dealer;

        public PlayerList(MainWindow mainWindow)
        {
            newPlayer = new Player();
            Players = new List<Player>()
            {
            };
            MainWindow = mainWindow;
            Initialize();
        }

        public void Initialize()
        {
            Dealer = new Player(0);
            Players = new List<Player>();
        }

        private void AddParty()
        {
            if (MainWindow.Config.ChatChannel == "/p" && MainWindow.Config.AutoParty)
            {
                if (MainWindow.TruthOrDareGame != null && Players != null)
                {
                    if (PlayerManager == null)
                    {
                        PlayerManager = new PlayerManager();
                    }
                    if (FFNightClub.ClientState.LocalPlayer != null)
                    {
                        PlayerManager.UpdateParty(ref Players, FFNightClub.ClientState.LocalPlayer.Name.TextValue, MainWindow.Config.AutoNameMode);
                    }
                }
            }
        }

        private void AddPlayer()
        {
            if (string.IsNullOrEmpty(newPlayer.Name))
                return;

            if (Players.FirstOrDefault(p => p.Name.ToLower().Equals(newPlayer.Name.ToLower())) == null)
            {
                Player p = new Player()
                {
                    Name = newPlayer.Name,
                };
                p.Alias = p.GetAlias(NameMode.Both);
                Players.Add(p);
            }
        }

        private void AddTarget()
        {
            var target = FFNightClub.ClientState.LocalPlayer?.TargetObject;
            PluginLog.Debug(target.ObjectId.ToString() + " ++++++++++ ");
            if (target.ObjectKind == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Player)
            {
                newPlayer.Name = target.Name.TextValue;
                AddPlayer();
            }
        }

        private void Clear()
        {
            newPlayer = new Player();
            Players.Clear();
        }

        public void DrawPlayerList()
        {
            if (string.IsNullOrEmpty(Dealer.Name) && FFNightClub.ClientState != null)
            {
                Dealer.Name = FFNightClub.ClientState.LocalPlayer.Name.TextValue;
                Dealer.Alias = Dealer.GetAlias(NameMode.Both);
            }

            if (ImGui.Button("Add Party"))
            {
                AddParty();
            }
            ImGui.SameLine();
            if (ImGui.Button("Add Target"))
            {
                AddTarget();
            }
            ImGui.SameLine();
            if (ImGui.Button("Clear Players"))
            {
                Clear();
            }
            DrawAddPlayer();

            ImGui.Text("Player List:");
            ImGui.SameLine();
            ImGui.Text(Players.Count.ToString());
            ImGui.Separator();
            DrawPlayers();
        }

        private void DrawAddPlayer()
        {
            ImGui.BeginTable("Add Players", 2, ImGuiTableFlags.Resizable);
            ImGui.TableSetupColumn("Name");
            ImGui.TableSetupColumn("Add");
            ImGui.TableHeadersRow();
            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            ImGui.InputText($"###name", ref newPlayer.Name, 255);
            ImGui.TableNextColumn();
            if (ImGui.Button("Add"))
            {
                AddPlayer();
                MainWindow.TruthOrDareGame.Initialize();
            }
            ImGui.TableNextRow();
            ImGui.EndTable();
            ImGui.Separator();
        }

        private void DrawPlayers()
        {
            ImGui.BeginTable("Player List", 3, ImGuiTableFlags.ScrollY | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Resizable);
            ImGui.TableSetupColumn("Name");
            ImGui.TableSetupColumn("Enable");
            ImGui.TableSetupColumn("Remove");
            ImGui.TableHeadersRow();
            ImGui.TableNextRow();
            foreach (var player in Players)
            {
                ImGui.TableNextColumn();
                ImGui.Text(player.Name);
                ImGui.TableNextColumn();
                ImGui.Checkbox($"Enable {player.Name}", ref player.enabled);
                ImGui.TableNextColumn();
                if (ImGui.Button($"Delete {player.Name}", new Vector2(160, 50)))
                {
                    RemovePlayer(player.Name);
                    MainWindow.TruthOrDareGame.Initialize();
                }
                ImGui.TableNextRow();
            }
            ImGui.EndTable();
        }

        private void RemovePlayer(string name)
        {
            var p = Players.Find(p => p.Name.ToLower().Equals(name.ToLower()));
            if (p != null)
            {
                Players.Remove(p);
            }
        }
    }
}
