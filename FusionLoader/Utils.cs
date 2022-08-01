using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace FusionLoader
{
    internal static class Utils
    {
        internal static void InitializeFiles()
        {
            if (!Directory.Exists("Fusion Client"))
            {
                Directory.CreateDirectory("Fusion Client");
            }
        }

        internal static string InitializeKey()
        {
            if (!File.Exists(Loader.ModDir + "\\Auth.FC"))
            {
                File.Create(Loader.ModDir + "\\Auth.FC").Close();
            }
            if (new FileInfo(Loader.ModDir + "\\Auth.FC").Length <= 0)
            {
                Loader.logger.Msg(ConsoleColor.Cyan, "Please Enter your Fusion Auth Key:");
                var input = Console.ReadLine();
                File.WriteAllText(Loader.ModDir + "\\Auth.FC", input.Trim());
            }
            return File.ReadAllText(Loader.ModDir + "\\Auth.FC").Trim();
        }

        public static AssemblyResponse InitializeAPI()
        {
            var unityWebRequest = UnityWebRequest.Put("https://api.vrcmods.xyz/api/users/login", JsonConvert.SerializeObject(new
            {
                Key = Loader.AuthKey,
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
                        var AR200 = AssemblyResponse.OK(Convert.FromBase64String(unityWebRequest.downloadHandler.text), "Successfully logged in!");
                        unityWebRequest.Dispose();
                        return AR200;

                    case 204:
                        unityWebRequest.Dispose();
                        return AssemblyResponse.Error("Invalid Body Data!");

                    case 401:
                        switch (unityWebRequest.downloadHandler.text)
                        {
                            case "Invalid HWID":
                                unityWebRequest.Dispose();
                                return AssemblyResponse.HWID();

                            case string a when a.StartsWith("User is banned! Ban Reason: "):
                                var reason = a.Split(':')[1];
                                reason = reason.Remove(0, 1);
                                unityWebRequest.Dispose();
                                return AssemblyResponse.Banned("User is banned!", reason);

                            default:
                                var AR401Default = AssemblyResponse.Error(unityWebRequest.downloadHandler.text);
                                unityWebRequest.Dispose();
                                return AR401Default;
                        }

                    case 404:
                        unityWebRequest.Dispose();
                        return AssemblyResponse.Error("Invalid Key!");

                    default:
                        var ARDefault = AssemblyResponse.Error(unityWebRequest.downloadHandler.text);
                        unityWebRequest.Dispose();
                        return ARDefault;
                }


                /*var result = JsonConvert.DeserializeObject<JToken>(unityWebRequest.downloadHandler.text);
                var apiInfo = result["api_info"];
                var response = result["response"];

                if ((bool)apiInfo["Valid"])
                {
                    unityWebRequest.Dispose();
                    Loader.Hash = (string)response["hash"];
                    return AssemblyResponse.OK(Convert.FromBase64String((string)response["mod"]), (string)response["message"]);
                }
                else
                {
                    Console.WriteLine($"Response: {(string)response["message"]}");
                    switch ((string)response["message"])
                    {
                        case "This key is banned, please contact blaze":
                            unityWebRequest.Dispose();
                            return AssemblyResponse.Banned((string)response["message"], (string)response["ban_reason"]);

                        case "Loader is outdated":
                            return AssemblyResponse.Outdated();

                        case "HWID is invalid":
                            return AssemblyResponse.HWID();
                    }
                }
                unityWebRequest.Dispose();
                return AssemblyResponse.Error("uwu");*/
            }
            return AssemblyResponse.Error(unityWebRequest.downloadHandler.text);
        }

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

        public static IEnumerable<string> GetCommandLineArgs()
        {
            var text = Environment.CommandLine.ToLower();
            return text.Split(new[]
            {
                ' '
            }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static bool IsDevMode()
        {
            var commandLineArgs = GetCommandLineArgs();
            return commandLineArgs.Any(text => text.ToLower() == "--fusion.dev");
        }
    }
}
