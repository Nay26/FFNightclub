using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFNightClub.Windows;
using Dalamud.Game;
using System;
using FFNightClub.Config;
using Util;
using System.Linq;

namespace FFNightClub
{
    public sealed class FFNightClub : IDalamudPlugin
    {
        [PluginService] public static IClientState ClientState { get; private set; } = null!;
        [PluginService] public static IObjectTable Objects { get; private set; } = null!;

        [PluginService]
        internal static IGameInteropProvider GameInteropProvider { get; private set; } = null!;

        [PluginService] public static ISigScanner SigScanner { get; private set; } = null!;
        [PluginService] public static IChatGui ChatGui { get; private set; } = null!;
        public static Chat Chat;

        public string Name => "FFNightClub";
        private const string CommandName = "/test";
        private DalamudPluginInterface PluginInterface { get; init; }
        private ICommandManager CommandManager { get; init; }

        private static MainWindow MainWindow;
        public WindowSystem WindowSystem = new("FFNightClub");

        public FFNightClub(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] ICommandManager commandManager)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;

            WindowSystem = new WindowSystem(Name);
            MainWindow = new MainWindow(this) { IsOpen = false };
            MainWindow.Config = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Chat = new Chat();
            MainWindow.Config.Initialize(PluginInterface);
            WindowSystem.AddWindow(MainWindow);
            MainWindow.Initialize();


            this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "A useful message to display in /xlhelp"
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;

            ChatGui.ChatMessage += MainWindow.TruthOrDareGame.OnChatMessage;

        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();
            this.CommandManager.RemoveHandler(CommandName);
        }

        private void OnCommand(string command, string args)
        {
            // in response to the slash command, just display our main ui
            MainWindow.IsOpen = true;
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }
    }
}
