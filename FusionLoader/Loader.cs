using MelonLoader;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using UnityEngine;
using static Astrum.AstralBypass;

[assembly: MelonInfo(typeof(FusionLoader.Loader), $"FüsîonCliënt", "0.69.420", "Viper", "https://vrcmods.xyz")]
[assembly: MelonGame("VRChat", "VRChat")]
[assembly: MelonColor(ConsoleColor.Cyan)]

namespace FusionLoader
{
    public class Loader : MelonMod
    {
        public static MelonLogger.Instance logger = new("FusionLoader", ConsoleColor.Magenta);
        public static string ModDir = Directory.GetParent(Application.dataPath) + "\\Fusion Client";
        public static string MLDir = Directory.GetParent(Application.dataPath) + "\\MelonLoader";
        internal static Assembly _assembly { get; private set; }
        internal static Type ModType { get; private set; }
        internal static string AuthKey { get; set; }
        internal static bool CanLoad { get; set; } = true;

        public override void OnApplicationStart()
        {
            IntegrityChecks.Bypass();
            Utils.InitializeFiles();
            AuthKey = Utils.InitializeKey();
            var api = Utils.InitializeAPI();
            switch (api.Response)
            {
                case Response.Banned:
                    logger.Error("You are banned from using Fusion Client! | Reason: " + api.BanReason);
                    CanLoad = false;
                    return;

                case Response.HWID:
                    logger.Error("Your Current PC's HWID does not match the one attached to your key. Please contact support to get your key reset to be used on a new device!");
                    CanLoad = false;
                    return;

                case Response.OK:
                    if (Utils.IsDevMode())
                    {
                        if (!File.Exists("Fusion Client\\Resources\\FusionClient.dll"))
                        {
                            logger.Error("Failed to find Developer File! Cancelling...");
                            return;
                        }
                        var rawAssembly = File.ReadAllBytes("Fusion Client\\Resources\\FusionClient.dll");
                        _assembly = Assembly.Load(rawAssembly, null);
                        IntegrityChecks.Repair();
                    }
                    else
                    {
                        MelonLogger.Msg("Loadded");
                        _assembly = Assembly.Load(api.Assembly, null);
                        IntegrityChecks.Repair();
                    }
                    break;

                default:
                    logger.Error("There was a problem loading Fusion Client! | Message: " + api.Message);
                    CanLoad = false;
                    return;
            }

            if (_assembly != null)
            {
                ModType = _assembly.GetType("FusionClient.Main");
                VRC.OnApplicationStart();
            }
        }

        public override void OnApplicationQuit()
        {
            if (!CanLoad) return;
            VRC.OnApplicationQuit();
        }

        public override void OnUpdate()
        {
            if (!CanLoad) return;
            VRC.OnUpdate();
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (!CanLoad) return;
            VRC.OnSceneWasInitialized(buildIndex, sceneName);
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (!CanLoad) return;
            VRC.OnSceneWasLoaded(buildIndex, sceneName);
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            if (!CanLoad) return;
            VRC.OnSceneWasUnloaded(buildIndex, sceneName);
        }

        public override void OnLateUpdate()
        {
            if (!CanLoad) return;
            VRC.OnLateUpdate();
        }
    }
}
