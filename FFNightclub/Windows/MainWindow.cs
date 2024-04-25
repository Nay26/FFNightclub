using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using FFNightClub.Modules;
using FFNightClub.Config;
using FFNightClub.Models;
using FFNightClub.Modules;
using FFXIVClientStructs.Havok;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using static FFXIVClientStructs.FFXIV.Client.Game.Control.GazeController;
using static Lumina.Data.Parsing.Layer.LayerCommon;

namespace FFNightClub.Windows;

public class MainWindow : Window, IDisposable
{
    private FFNightClub Plugin;
    public static Configuration Config { get; set; }
    public TruthOrDareGame TruthOrDareGame;
    public GambaRPGGame GambaRPGGame;
    public PlayerList PlayerList;
    private MainTab currentMainTab = MainTab.PlayerList;


    public MainWindow(FFNightClub plugin) : base(
        "Night Club Stalker", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.Plugin = plugin;
        TruthOrDareGame = new TruthOrDareGame(this);
        GambaRPGGame = new GambaRPGGame(this);
        PlayerList = new PlayerList(this);
    }

    public void Initialize()
    {
        TruthOrDareGame = new TruthOrDareGame(this);
        GambaRPGGame = new GambaRPGGame(this);
        PlayerList = new PlayerList(this);
    }

    public void Dispose()
    {
    }

    public void Close_Window(object? sender, System.EventArgs e) => IsOpen = false;

    public override void Draw()
    {
        DrawMainTabs();
        switch (currentMainTab)
        {
            case MainTab.PlayerList:
                {
                    PlayerList.DrawPlayerList();
                    break;
                }
            case MainTab.TruthOrDare:
                {
                    TruthOrDareGame.DrawMatch();
                    break;
                }
            case MainTab.GambaRPG:
                {
                    GambaRPGGame.DrawMatch();
                    break;
                }
            case MainTab.About:
                {
                    DrawAbout();
                    break;
                }
            default:
                PlayerList.DrawPlayerList();
                break;
        }
    }

    private void DrawMainTabs()
    {
        if (ImGui.BeginTabBar("FFNightClubMainTabBar", ImGuiTabBarFlags.NoTooltip))
        {
            if (ImGui.BeginTabItem("Player List###FFNightClub_PlayerList_MainTab"))
            {
                currentMainTab = MainTab.PlayerList;
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Truth Or Dare###FFNightClub_Match_MainTab"))
            {
                currentMainTab = MainTab.TruthOrDare;
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Gamba Rpg###FFNightClub_RPG_MainTab"))
            {
                currentMainTab = MainTab.GambaRPG;
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("About###FFNightClub_About_MainTab"))
            {
                currentMainTab = MainTab.About;
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
            ImGui.Spacing();
        }
    }

    private void DrawAbout()
    {
        ImGui.TextColored(ImGuiColors.DalamudGrey, "About");
        ImGui.TextWrapped("Made by Adiana Umbra@Cerberus");
        ImGui.TextWrapped("Thanks to Amazing Primu Pyon@Omega who's code I have shamelessly modified");
        ImGui.Separator();
        ImGui.Spacing();
        ImGui.TextWrapped("+ Use Add Party to add all players in your current party to the player list(Have to be nearby).");
        ImGui.TextWrapped("+ Use Add Target to add current target to the player list");
        ImGui.TextWrapped("+ Or type a name and add it manually");
        ImGui.Separator();
        ImGui.Spacing();
        ImGui.TextWrapped("1) Press New Round to begin a new round");
        ImGui.TextWrapped("2) Press Roll Next Player to roll the dice for the next player. (Has to be one at a time due to chat spam limits)");
        ImGui.TextWrapped("3) Once all players have been rolled (no longer have a roll of -1) click declare results");
        ImGui.Columns(1);
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5);
        if (ImGui.Button("Close"))
        {
            IsOpen = false;
        }
    }
}
