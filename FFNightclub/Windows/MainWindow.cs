using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using FFNightclub.Models;
using FFXIVClientStructs.Havok;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using static Lumina.Data.Parsing.Layer.LayerCommon;

namespace FFNightclub.Windows;

public class MainWindow : Window, IDisposable
{
    private FFNightclub Plugin;
    public List<Player> SessionPlayerList;
    public Player newPlayer;

    public MainWindow(FFNightclub plugin) : base(
        "Night Club Stalker", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.Plugin = plugin;
        SessionPlayerList = new List<Player>();
        newPlayer = new Player();
    }

    private void ExportPlayerList()
    {
        string csvHeaderRow = String.Join(",", typeof(Player).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(x => x.Name).ToArray<string>()) + Environment.NewLine;
        string csv = csvHeaderRow + String.Join(Environment.NewLine, SessionPlayerList.Select(x => x.ToString()).ToArray());
        PluginLog.Debug(csv);
    }

    private void ImportPlayerList()
    {
        string csvHeaderRow = String.Join(",", typeof(Player).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(x => x.Name).ToArray<string>()) + Environment.NewLine;
        string csv = csvHeaderRow + String.Join(Environment.NewLine, SessionPlayerList.Select(x => x.ToString()).ToArray());
        PluginLog.Debug(csv);
    }

    private void UpdatePlayerList()
    {
        var localPlayers = FFNightclub.Objects.Where(o => o.ObjectKind == Dalamud.Game.ClientState.Objects.Enums.ObjectKind.Player).ToList();
        if (localPlayers != null)
        {
            foreach (var player in localPlayers)
            {
                newPlayer.Name = player.Name.TextValue;
                newPlayer.ID = Int32.Parse(player.ObjectId.ToString());
                AddPlayer();
            }
        }
    }

    private void AddPlayer()
    {
        if (string.IsNullOrEmpty(newPlayer.Name))
            return;

        if (SessionPlayerList.FirstOrDefault(p => p.ID == newPlayer.ID) == null)
        {
            Player p = new Player()
            {
                Name = newPlayer.Name,
                ID = newPlayer.ID,
                FirstSeen = DateTime.Now.ToString(),
                LastSeen = DateTime.Now.ToString(),
            };
            p.Alias = p.GetAlias(NameMode.Both);
            SessionPlayerList.Add(p);
        }
        else
        {
            SessionPlayerList.FirstOrDefault(p => p.ID == newPlayer.ID).LastSeen = DateTime.Now.ToString();
        }
        SessionPlayerList = SessionPlayerList.OrderBy(p => p.Name).ToList();
        SessionPlayerList = SessionPlayerList.OrderByDescending(p => p.LastSeen).ToList();
    }

    private void RemovePlayer(int id)
    {
        var p = SessionPlayerList.Find(p => p.ID == id);
        if (p != null)
        {
            SessionPlayerList.Remove(p);
        }
    }

    public void Dispose()
    {
    }

    private void DrawPlayerTable()
    {
        ImGui.BeginTable("Player List", 5, ImGuiTableFlags.ScrollY | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Resizable);
        ImGui.TableSetupColumn("ID");
        ImGui.TableSetupColumn("Name");
        ImGui.TableSetupColumn("First Seen");
        ImGui.TableSetupColumn("Last Seen");
        ImGui.TableSetupColumn("Notes");
        ImGui.TableHeadersRow();
        ImGui.TableNextRow();
        foreach (var player in SessionPlayerList)
        {
            ImGui.TableNextColumn();
            ImGui.Text(player.ID.ToString());
            ImGui.TableNextColumn();
            ImGui.Text(player.Name);
            ImGui.TableNextColumn();
            ImGui.Text(player.FirstSeen);
            ImGui.TableNextColumn();
            ImGui.Text(player.LastSeen);
            ImGui.TableNextColumn();
            ImGui.Text("Notes Here");
            ImGui.TableNextRow();
        }

        ImGui.EndTable();
    }

    public override void Draw()
    {
        ImGui.Text($"The random config bool is {this.Plugin.Configuration.SomePropertyToBeSavedAndWithADefault}");

        if (ImGui.Button("Show Settings"))
        {
            this.Plugin.DrawConfigUI();
        }

        ImGui.Spacing();
        if (ImGui.Button("Scan for players"))
        {
            UpdatePlayerList();
        }
        if (ImGui.Button("Export Player List"))
        {
            ExportPlayerList();
        }
        if (ImGui.Button("Import Player List"))
        {
            ImportPlayerList();
        }
        ImGui.Text("Player List:");
        ImGui.SameLine();
        ImGui.Text(SessionPlayerList.Count.ToString());
        DrawPlayerTable();
    }
}
