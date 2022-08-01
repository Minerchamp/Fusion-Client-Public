using FusionClient.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FusionClient.Utils
{
    internal static class ModFiles
    {
        public static string FusionPath = Directory.GetParent(Application.dataPath) + "\\Fusion Client";
        public static string MelonLoaderPath = Directory.GetParent(Application.dataPath) + "\\MelonLoader";
        public static string GameDataPath = Directory.GetParent(Application.dataPath) + "\\VRChat_Data";

        // Main Directories
        public static string ConfigsPath = FusionPath + "\\Configs";
        public static string DependenciesPath = FusionPath + "\\Dependencies";
        public static string MiscPath = FusionPath + "\\Misc";

        public static string DownloadsPath = FusionPath + "\\Downloads";
        public static string VRCWPath = FusionPath + "\\VRCW";
        public static string VRCAPath = FusionPath + "\\VRCA";
        public static string LogsPath = FusionPath + "\\Logs";

        // Bot Shit
        public static string ModsPath = MiscPath + "\\Mods";
        public static string FreedomPath = MiscPath + "\\Freedom";
        public static string BotLoginsPath = MiscPath + "\\BotLogins";

        // Files
        public static string ConfigFile = ConfigsPath + "\\Config.json";
        public static string RoomHistoryFile = MiscPath + "\\RoomHistory.json";
        public static string IDBlackListFile = MiscPath + "\\IDBlackList.json";
        public static string AviFavFile = MiscPath + "\\AviFavFile.json";
        public static string KeyFile = ModsPath + "\\Auth.FC";

        internal static void Initialize()
        {
            Directory.CreateDirectory(FusionPath);
            Directory.CreateDirectory(ConfigsPath);
            Directory.CreateDirectory(DependenciesPath);
            Directory.CreateDirectory(MiscPath);
            Directory.CreateDirectory(DownloadsPath);
            Directory.CreateDirectory(VRCWPath);
            Directory.CreateDirectory(VRCAPath);
            Directory.CreateDirectory(ModsPath);
            Directory.CreateDirectory(FreedomPath);
            Directory.CreateDirectory(BotLoginsPath);
            Directory.CreateDirectory(LogsPath);

            if (!File.Exists("Newtonsoft.Json.Bson.dll"))
            {
                try
                {
                    Logs.Log("Downloading Newtonsoft.Json.Bson Off Main Site", ConsoleColor.Cyan);
                    using (WebClient wc = new WebClient())
                    {
                        var bytes = wc.DownloadData("https://vrcmods.xyz/assets/1223131Load11246676231235/Newtonsoft.Json.Bson.dll");
                        File.WriteAllBytes("Newtonsoft.Json.Bson.dll", bytes);
                    }
                }
                catch
                {
                    Logs.Log("Error Downloading Newtonsoft.Json.Bson Off Main Site Trying BackUp", ConsoleColor.Cyan);
                    using (WebClient wc = new WebClient())
                    {
                        var bytes = wc.DownloadData("http://fusionclient.shop/assets/1223131Load11246676231235/Newtonsoft.Json.Bson.dll");
                        File.WriteAllBytes("Newtonsoft.Json.Bson.dll", bytes);
                    }
                }
            }
            if (!File.Exists(Directory.GetCurrentDirectory() + "\\Fusion Client\\Dependencies\\discord-rpc.dll"))
            {
                try
                {
                    Logs.Log("Downloading Discord-RPC Off Main Site", ConsoleColor.Cyan);
                    using (WebClient wc = new WebClient())
                    {
                        var bytes = wc.DownloadData("https://vrcmods.xyz/assets/1223131Load11246676231235/discord-rpc.dll");
                        File.WriteAllBytes("Fusion Client\\Dependencies\\discord-rpc.dll", bytes);
                    }
                }
                catch
                {
                    Logs.Log("Error Downloading Discord-RPC Off Main Site Trying BackUp", ConsoleColor.Cyan);
                    using (WebClient wc = new WebClient())
                    {
                        var bytes = wc.DownloadData("http://fusionclient.shop/assets/1223131Load11246676231235/discord-rpc.dll");
                        File.WriteAllBytes("Fusion Client\\Dependencies\\discord-rpc.dll", bytes);
                    }
                }
            }
        }
    }
}
