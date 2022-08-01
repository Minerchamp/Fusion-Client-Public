using FusionClient.Modules;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FusionClient.Core
{
    internal static class Logs
    {
        private static readonly MelonLogger.Instance logger = new("FusionClient", ConsoleColor.DarkCyan);

        public static void Log(string log) => logger.Msg(log);
        public static void Log(string log, ConsoleColor color) => logger.Msg(color, log);
        public static void Warning(string log) => logger.Warning(log);
        public static void Error(string log) => logger.Error(log);
        public static void Error(string moduleName, Exception ex)
        {
            logger.Msg(ConsoleColor.Red, "==================[ERROR]==================");
            logger.Msg(ConsoleColor.DarkRed, $"Module: {moduleName}");
            logger.Msg(ConsoleColor.DarkRed, $"Message: {ex.Message}");
            logger.Msg(ConsoleColor.Red, "-------------------------------------------");
            logger.Msg(ConsoleColor.DarkRed, $"StackTrace: {ex.StackTrace}");
            logger.Msg(ConsoleColor.Red, "================[END ERROR]================");
        }
        // Debug Panel Logs
        public static List<string> DebugLogs = new List<string>();
        public static List<string> DebugLogs2 = new List<string>();
        private static string lastMsg = string.Empty;
        private static int duplicateCount = 1;

        internal static void Debug(string message) => HandleDebug(message);

        private static void HandleDebug(string message)
        {
            if (UI.DebugPanel == null) return;
            if (Color_Edit.DebugPanel == null) return;
            if (message == lastMsg)
            {
                DebugLogs.RemoveAt(DebugLogs.Count - 1);
                DebugLogs2.RemoveAt(DebugLogs2.Count - 1);
                duplicateCount++;
                DebugLogs.Add($"<color=#00eeff>{DateTime.Now:hh:mm tt}</color> | {message} <color=red><i>x{duplicateCount}</i></color>");
                DebugLogs2.Add($"<color=#00eeff>{DateTime.Now:hh:mm tt}</color> | {message} <color=red><i>x{duplicateCount}</i></color>");
            }
            else
            {
                lastMsg = message;
                duplicateCount = 1;
                DebugLogs.Add($"<color=#00eeff>{DateTime.Now:hh:mm tt}</color> | {message}");
                DebugLogs2.Add($"<color=#00eeff>{DateTime.Now:hh:mm tt}</color> | {message}");
                if (DebugLogs2.Count == 9)
                {
                    DebugLogs2.RemoveAt(0);
                    DebugLogs2.Clear();
                }
                if (DebugLogs.Count == 30)
                {
                    DebugLogs.RemoveAt(0);
                }
            }
            if (Config.Main.CustomUi)
            {
                Color_Edit.DebugPanel.SetText(string.Join("\n", DebugLogs.Take(22)));
            }
            else
            {
                UI.DebugPanel.SetText(string.Join("\n", DebugLogs.Take(22)));
            }
        }

        // Hud Logs
        internal static string lastHud = string.Empty;

        public static void Hud(string message)
        {
            if (message == lastHud) return;
            lastHud = message;
            FusionClient.Modules.Hud.Display(message, 7f);
        }
    }
}
