using MelonLoader;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using VRC.Core;

namespace FusionClient.Core
{
    internal static class Functions
    {
        internal static System.Random rnd = new();

        [ObfuscationAttribute(Exclude = false, Feature = "-rename")]
        internal static bool IsValidLoader(string loaderID)
        {
            if (loaderID != "9777c51b-8b38-4b5e-ba61-e87ac74f86f9")
            {
                Logs.Warning("You attempted to run Fusion Client on an unofficial loader. Fusion Client will not be initialized.");
                CrackProtection.GameKillLoop().Start();
                CrackProtection.GameKill();
                return false;
            }
            return true;
        }

        [ObfuscationAttribute(Exclude = false, Feature = "-rename")]
        private static string GetDeviceID()
        {
            const string location = @"SOFTWARE\Microsoft\Cryptography";
            const string name = "MachineGuid";
            using var localMachineX64View = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            using var rk = localMachineX64View.OpenSubKey(location);
            if (rk == null)
            {
                throw new KeyNotFoundException($"Key Not Found: {location}");
            }
            var machineGuid = rk.GetValue(name);
            if (machineGuid == null)
            {
                throw new IndexOutOfRangeException($"Index Not Found: {name}");
            }
            return machineGuid.ToString();
        }

        [ObfuscationAttribute(Exclude = false, Feature = "-rename")]
        internal static bool IsValidKey()
        {
            var unityWebRequest = UnityWebRequest.Put("https://api.vrcmods.xyz/api/users/login", JsonConvert.SerializeObject(new
            {
                Key = File.ReadAllText(Directory.GetCurrentDirectory() + "\\Fusion Client\\Auth.FC").Trim(),
                HWID = GetDeviceID()
            }));
            unityWebRequest.method = "POST";
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");
            unityWebRequest.SendWebRequest();
            while (!unityWebRequest.isDone) { }

            if (string.IsNullOrWhiteSpace(unityWebRequest.error))
            {
                switch (unityWebRequest.responseCode)
                {
                    case 200:
                        unityWebRequest.Dispose();
                        return true;

                    case 204:
                        unityWebRequest.Dispose();
                        return false;

                    case 401:
                        switch (unityWebRequest.downloadHandler.text)
                        {
                            case "Invalid HWID":
                                unityWebRequest.Dispose();
                                return false;

                            case string a when a.StartsWith("User is banned! Ban Reason: "):
                                var reason = a.Split(':')[1];
                                reason = reason.Remove(0, 1);
                                unityWebRequest.Dispose();
                                return false;

                            default:
                                unityWebRequest.Dispose();
                                return false;
                        }

                    case 404:
                        unityWebRequest.Dispose();
                        return false;

                    default:
                        unityWebRequest.Dispose();
                        return false;
                }
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Sorry Looks Like There Was A Error With Loading | [I V K]");
            Console.ForegroundColor = ConsoleColor.White;
            return false;
        }

        [ObfuscationAttribute(Exclude = false, Feature = "-rename")]
        public static void CheckIfUnix()
        {
            if (MelonLoader.MelonHandler.Mods.Exists(x => x.Info.Author.ToLower() == "unixian"))
            {
                CrackProtection.GameKill();
                CrackProtection.GameKillLoop().Start();
            }
            //if (MelonLoader.MelonHandler.Plugins.Exists(x => x.Info.Author.ToLower() == "unixian"))
            //{
            //    CrackProtection.GameKill();
            //    CrackProtection.GameKillLoop().Start();
            //}
            //if (MelonLoader.MelonHandler.Plugins.Exists(x => x.Info.Author.ToLower() == "unixian :)"))
            //{
            //    CrackProtection.GameKill();
            //    CrackProtection.GameKillLoop().Start();
            //}
            //if (MelonLoader.MelonHandler.Plugins.Exists(x => x.Info.Name.ToLower() == "cracktool"))
            //{
            //    CrackProtection.GameKill();
            //    CrackProtection.GameKillLoop().Start();
            //}
            if (MelonLoader.MelonHandler.Mods.Exists(x => x.Info.Name.ToLower() == "UnixWare"))
            {
                CrackProtection.GameKill();
                CrackProtection.GameKillLoop().Start();
            }
            if (System.Environment.MachineName == "Unixian")
            {
                CrackProtection.GameKill();
                CrackProtection.GameKillLoop().Start();
            }
        }

        [ObfuscationAttribute(Exclude = false, Feature = "-rename")]
        internal static bool IsRealClient()
        {
            if (Directory.Exists(Environment.CurrentDirectory + "\\Fusion Client") && File.Exists(Environment.CurrentDirectory + "\\Fusion Client\\Auth.FC") && File.Exists(Environment.CurrentDirectory + "\\Mods\\FusionLoader.dll"))
            {
                return true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Sorry Looks Like There Was A Error With Loading | [I R C]");
                Console.ForegroundColor = ConsoleColor.White;
                return false;
            }
        }

        public static void Start(this IEnumerator routine)
        {
            MelonCoroutines.Start(routine);
        }

        public static void Stop(this IEnumerator routine)
        {
            MelonCoroutines.Stop(routine);
        }

        public static void Delay(float time, System.Action action) => ProcessDelay(time, action).Start();

        private static IEnumerator ProcessDelay(float time, System.Action action)
        {
            yield return new WaitForSecondsRealtime(time);
            action.Invoke();
            yield break;
        }

        [ObfuscationAttribute(Exclude = false, Feature = "-rename")]
        public static IEnumerator SendAviToDB(ApiAvatar a)
        {
            var unityWebRequest = UnityWebRequest.Put("https://api.vrcmods.xyz/api/avatars/upload", JsonConvert.SerializeObject(new
            {
                AvatarID = a.id,
                AssetURL = a.assetUrl,
                AuthorID = a.authorId,
                AuthorName = a.authorName,
                AvatarName = a.name,
                Description = a.description,
                ImageURL = a.imageUrl,
                ReleaseStatus = a.releaseStatus,
                ThumbnailImageURL = a.thumbnailImageUrl,
                Version = a.version
            }));
            unityWebRequest.method = "POST";
            unityWebRequest.SetRequestHeader("FCAuth", File.ReadAllText("Fusion Client\\Auth.FC").Trim());
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");
            unityWebRequest.SendWebRequest();
            while (!unityWebRequest.isDone) yield return null;
            yield break;
        }

        [ObfuscationAttribute(Exclude = false, Feature = "-rename")]
        public static IEnumerator CrackAlert(string a)
        {
            var unityWebRequest = UnityWebRequest.Put("https://api.vrcmods.xyz/api/users/crackalert", JsonConvert.SerializeObject(new
            {
                Message = a
            }));
            unityWebRequest.method = "POST";
            unityWebRequest.SetRequestHeader("FCAuth", File.ReadAllText("Fusion Client\\Auth.FC").Trim());
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");
            unityWebRequest.SendWebRequest();
            while (!unityWebRequest.isDone) yield return null;
            yield break;
        }

        [ObfuscationAttribute(Exclude = false, Feature = "-rename")]
        public static IEnumerator CrackAlertv2(string a)
        {
            var unityWebRequest = UnityWebRequest.Put("https://api.vrcmods.xyz/api/users/crackalertv2", JsonConvert.SerializeObject(new
            {
                Message = a
            }));
            unityWebRequest.method = "POST";
            unityWebRequest.SetRequestHeader("FCAuth", File.ReadAllText("Fusion Client\\Auth.FC").Trim());
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");
            unityWebRequest.SendWebRequest();
            while (!unityWebRequest.isDone) yield return null;
            yield break;
        }

        [ObfuscationAttribute(Exclude = false, Feature = "-rename")]
        public static IEnumerator BanKey(string a)
        {
            var unityWebRequest = UnityWebRequest.Put("https://api.vrcmods.xyz/api/users/ban", JsonConvert.SerializeObject(new
            {
                Key = a,
                Reason = "Crack attempt"
            }));
            unityWebRequest.method = "POST";
            unityWebRequest.SetRequestHeader("FCAuth", File.ReadAllText("Fusion Client\\Auth.FC").Trim());
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");
            unityWebRequest.SendWebRequest();
            while (!unityWebRequest.isDone) yield return null;
            yield break;
        }
    }
}
