using Fusion.Networking;
using FusionClient;
using FC.Components;
using FC.Utils;
using FCConsole;
using FusionClient.API;
using FusionClient.Core;
using FusionClient.Modules;
using FusionClient.Modules.Discord;
using FusionClient.Modules.Other;
using FusionClient.Utils;
using FusionClient.Utils.Manager;
using FusionClient.Utils.Objects.Mod;
using MelonLoader;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using FusionClient.Startup.Hooks.PhotonHook.Startup;

namespace FusionClient
{
    public static class Main
    {
        [Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
        private static bool IsValid { get; set; } = false;
        private static bool IsValidKey;
        private static bool IsReal;
        public static bool IsLoaded;
        public static bool HasHudMsgShown;
        internal static List<FusionModule> Modules = new();

        public static void OnApplicationStart(string loaderID = null)
        {
            MelonCoroutines.Start(CrackProtection.DllProtection());
            IsValid = Functions.IsValidLoader(loaderID);
            IsValidKey = Functions.IsValidKey();
            IsReal = Functions.IsRealClient();
            if (File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"system32\drivers\etc\hosts")).Contains("vrcmods.xyz"))
            {
                return;
            }
            //Functions.CheckIfUnix();
            if (MelonLoader.MelonHandler.Mods.Exists(x => x.Info.Name.ToLower() == "UnixWare"))
            {
                CrackProtection.GameKill();
                CrackProtection.GameKillLoop().Start();
                return;
            }

            if (!IsValid) return;
            if (!IsReal) return;
            if (!IsValidKey) return;

            System.Console.Title = $"FusionClient V2.2 | By Viper";

            BotModule.IsBot = Environment.GetCommandLineArgs().Any(a => a.Contains("FCBot"));

            #region Move Mods Back

            if (!BotModule.IsBot)
            {
                try
                {
                    if (Directory.GetFiles("Fusion Client\\Misc\\Mods").Any())
                    {
                        foreach (var file in Directory.EnumerateFiles("Fusion Client\\Misc\\Mods"))
                        {
                            string destFile = Path.Combine("Mods", Path.GetFileName(file));
                            if (!File.Exists(destFile))
                                File.Move(file, destFile);
                            Logs.Log("Moving Mods Back Into Mods Folder Last Time You Used The Bots They Never Moved The Mods Back: Restart To Use Your Old Mods");
                            Logs.Log("Moving Mods Back Into Mods Folder Last Time You Used The Bots They Never Moved The Mods Back: Restart To Use Your Old Mods");
                            Logs.Log("Moving Mods Back Into Mods Folder Last Time You Used The Bots They Never Moved The Mods Back: Restart To Use Your Old Mods");
                            Logs.Log("Moving Mods Back Into Mods Folder Last Time You Used The Bots They Never Moved The Mods Back: Restart To Use Your Old Mods");
                            Logs.Log("Moving Mods Back Into Mods Folder Last Time You Used The Bots They Never Moved The Mods Back: Restart To Use Your Old Mods");
                            Logs.Log("Moving Mods Back Into Mods Folder Last Time You Used The Bots They Never Moved The Mods Back: Restart To Use Your Old Mods");
                        }
                    }
                }
                catch { }
            }

            #endregion

            if (!BotModule.IsBot)
            {
                ModFiles.Initialize();
            }

            Config.Initialize();
            Modules.Add(new BotModule()); // Bots

            if (!Directory.Exists("Fusion Client\\Misc\\Mods"))
            {
                Directory.CreateDirectory("Fusion Client\\Misc\\Mods");
            }
            if (!Directory.Exists("Fusion Client\\Misc\\BotLogins"))
            {
                Directory.CreateDirectory("Fusion Client\\Misc\\BotLogins");
            }

            if (BotModule.IsBot)
            {
                System.Random rnd = new System.Random();
                int Time = rnd.Next(1500, 2500);
                Thread.Sleep(Time);
                Logs.Log("Connecting to Socket...", ConsoleColor.Red);
                for (; !BotNetworkingManager.IsReady;) { }
                Logs.Log("Connected to Socket", ConsoleColor.Red);
                Modules.Add(new HWIDSpoof());
            }

            if (!BotModule.IsBot)
            {

                if (Config.Main.CustomUi)
                {
                    HttpWebRequest.DefaultWebProxy = new WebProxy();
                    System.Net.WebClient wc6 = new System.Net.WebClient();
                    var Pic = wc6.DownloadData(Config.Main.MenuPicture);
                    File.WriteAllBytes("Fusion Client\\Misc\\MenuPic.png", Pic);
                    Color_Edit.MenuPicPNG = new System.Drawing.Bitmap("Fusion Client\\Misc\\MenuPic.png");
                    var PicBitmap = Color_Edit.ConverterPNG(Color_Edit.MenuPicPNG);
                    Color_Edit.MenuPic = PicBitmap;
                }
                AssetBundleManager.Initialize();

                WaitForHUD().Start();
                WaitForAM().Start();
                WaitForQM().Start();
                WaitForSM().Start();

                if (!BotServer.Started)
                {
                    if (Config.Main.AutoStartBotManagerServer)
                    {
                        UI.BServer = new BotServer();
                    }
                }

                if (!Config.Main.NoConsoleClear) System.Console.Clear();

                FCConsole.Console.WriteFigletWithGradient(FigletFont.LoadFromAssembly("Larry3D.flf"), "fusionClient", System.Drawing.Color.LightCyan, System.Drawing.Color.DarkCyan);
                PhotonOnEventHook.HookPhotonOnEvent();
                Patches.Initialize();

                Modules.Add(new Hud()); // Load HudMsg Method
                Modules.Add(new HWIDSpoof()); // HWID Spoofer
                Modules.Add(new DiscordHelper()); // Discord Presence
                Modules.Add(new Flight()); // Flight
                Modules.Add(new Loading()); // Loading Edit
                //Modules.Add(new GetTime()); // Gets Time Left On Key
                Modules.Add(new ItemOrbit()); // Item Orbt
                Modules.Add(new AvatarFavorites()); // Avi Favs
                Modules.Add(new SeenAvatars()); // SeenAvatars

                ClassInjector.RegisterTypeInIl2Cpp<EnableDisableListener>(); // Idk Something For Avis
                ClassInjector.RegisterTypeInIl2Cpp<PlayerList>(); // PlayerList
                //ClassInjector.RegisterTypeInIl2Cpp<GetTimeLeft>(); // Gets Time Left On Key
                ClassInjector.RegisterTypeInIl2Cpp<FCNameplate>(); // NamePlate

                MelonCoroutines.Start(ModulesFunctions.ItemLockLoop()); // LockLitems
                MelonCoroutines.Start(ModulesFunctions.AntiItemOrbitLoop()); // LockItems But Fast
                MelonCoroutines.Start(Optimizations.Loop()); // Loop For Optimizations
                MelonCoroutines.Start(Config.SaveConfigLoop()); // Save Loop For Config
                MelonCoroutines.Start(ModulesFunctions.Loop()); // Portal Loops
                MelonCoroutines.Start(ModulesFunctions.Check(true)); // forceclone check
                MelonCoroutines.Start(AviDistanceHide.AvatarScanner());

                if (Config.Main.CustomUi)
                {
                    HttpWebRequest.DefaultWebProxy = new WebProxy();
                    System.Net.WebClient wc7 = new System.Net.WebClient();
                    var Pic2 = wc7.DownloadData("https://i.imgur.com/JeGqWwK.png");
                    Color_Edit.MenuPicV2 = Pic2;
                    HttpWebRequest.DefaultWebProxy = new WebProxy();
                    System.Net.WebClient wc8 = new System.Net.WebClient();
                    var Pic3 = wc8.DownloadData("https://i.imgur.com/oMWgck3.png");
                    Color_Edit.MenuDebug = Pic3;
                }
                if (Config.Main.NamePlates)
                {
                    HttpWebRequest.DefaultWebProxy = new WebProxy();
                    System.Net.WebClient wc3 = new System.Net.WebClient();
                    var Nova = wc3.DownloadString("https://vrcmods.xyz/assets/NotADIRLOL1235235777/NovaTags.json");
                    Nameplates.NovaTags = JsonConvert.DeserializeObject<List<string>>(Nova);
                    HttpWebRequest.DefaultWebProxy = new WebProxy();
                    System.Net.WebClient wc4 = new System.Net.WebClient();
                    var Evo = wc4.DownloadString("https://vrcmods.xyz/assets/NotADIRLOL1235235777/EvoTags.json");
                    Nameplates.EvoTags = JsonConvert.DeserializeObject<List<string>>(Evo);
                    //System.Net.WebClient wc5 = new System.Net.WebClient();
                    //var Boost = wc5.DownloadString("https://vrcmods.xyz/assets/NotADIRLOL1235235777/BoostTags.json");
                    //Nameplates.Boosters = JsonConvert.DeserializeObject<List<string>>(Boost);
                }
                if (Config.Main.AutoUpdateCrasher)
                {
                    Config.Main.AviClapID = "";
                    Config.Main.QuestCrashID = "";
                    Config.Main.Save();
                    try
                    {
                        HttpWebRequest.DefaultWebProxy = new WebProxy();
                        System.Net.WebClient wc1 = new System.Net.WebClient();
                        var Quest = wc1.DownloadString("https://vrcmods.xyz/IDQ");
                        Config.Main.QuestCrashID = Quest;
                        HttpWebRequest.DefaultWebProxy = new WebProxy();
                        System.Net.WebClient wc2 = new System.Net.WebClient();
                        var Avi = wc2.DownloadString("https://vrcmods.xyz/ID");
                        Config.Main.AviClapID = Avi;
                    }
                    catch
                    {
                        HttpWebRequest.DefaultWebProxy = new WebProxy();
                        System.Net.WebClient wc1 = new System.Net.WebClient();
                        var Quest = wc1.DownloadString("https://fusionclient.shop/IDQ");
                        Config.Main.QuestCrashID = Quest;
                        HttpWebRequest.DefaultWebProxy = new WebProxy();
                        System.Net.WebClient wc2 = new System.Net.WebClient();
                        var Avi = wc2.DownloadString("https://fusionclient.shop/ID");
                        Config.Main.AviClapID = Avi;
                    }
                }
            }

            for (int i = 0; i < Modules.Count; i++) Modules[i].Start();
        }

        public static void OnApplicationQuit()
        {
            if (!IsValid) return;
            for (int i = 0; i < Modules.Count; i++) Modules[i].Stop();
        }

        public static void OnUpdate()
        {
            if (!IsValid) return;
            if (!BotModule.IsBot)
            {
                IKTweeks.OnUpdate();
                ModulesFunctions.OnUpdate();
            }
            MainThreadRunner.Update();
            for (int i = 0; i < Modules.Count; i++) Modules[i].Update();
        }

        public static void OnSceneWasInitialized(int index, string name)
        {
            if (!IsValid) return;
            Patches.AntiLockInstance(index, null);
            if (!BotModule.IsBot)
            {
                try
                {
                    ModulesFunctions.HighlightColor(Color.cyan);
                }
                catch { }
            }
        }

        public static void OnSceneWasLoaded(int index, string name)
        {
            if (!IsValid) return;
            if (!IsReal) return;
            if (!IsValidKey) return;
            if (!BotModule.IsBot)
            {
                Optimizations.RamClear();
            }
            if (index == -1)
            {
                WaitForWorld().Start();
                WaitForPlayer().Start();
            }
            for (int i = 0; i < Modules.Count; i++) Modules[i].SceneLoaded(index, name);
        }

        public static void OnLateUpdate()
        {
            if (!IsValid) return;
            if (!BotModule.IsBot)
            {
                if (ModulesFunctions.SwastikaBool)
                {
                    ModulesFunctions.Swastika();
                }
            }
        }

        public static void OnSceneWasUnloaded(int index, string name)
        {
            if (!IsValid) return;
        }

        private static IEnumerator WaitForWorld()
        {
            while (RoomManager.field_Internal_Static_ApiWorld_0 == null) yield return null;
            while (RoomManager.field_Internal_Static_ApiWorldInstance_0 == null) yield return null;

            yield return new WaitForSecondsRealtime(3);

            if (Config.Main.NamePlates)
            {
                Nameplates.FetchTags().Start();
            }

            MelonCoroutines.Start(SeenAvatars.ClearMenu());

            ItemOrbit.Recache();

            if (!Main.IsLoaded)
            {
                Main.IsLoaded = true;
                MelonCoroutines.Start(SeenAvatars.RefreshMenu());
            }

            if (BotServer.Started && Config.Main.AutoJoin)
            {
                BotServer.SendAll(new Fusion.Networking.Serializable.PacketData(PacketBotServerType.JOIN_WORLD, RoomManager.field_Internal_Static_ApiWorldInstance_0.id));
            }

            FusionClient.Startup.Hooks.PhotonHook.PhotonHandlers.Photon_PlayerModerationHandler.BlockedYouPlayers.Clear();
            FusionClient.Startup.Hooks.PhotonHook.PhotonHandlers.Photon_PlayerModerationHandler.MutedYouPlayers.Clear();
            ModulesFunctions.AllPickups = Resources.FindObjectsOfTypeAll<VRC.SDKBase.VRC_Pickup>().ToList();
            ModulesFunctions.AllUdonPickups = Resources.FindObjectsOfTypeAll<VRC.SDK3.Components.VRCPickup>().ToList();
            ModulesFunctions.AllSyncPickups = Resources.FindObjectsOfTypeAll<VRCSDK2.VRC_ObjectSync>().ToList();
            ModulesFunctions.AllTriggers = Resources.FindObjectsOfTypeAll<VRC.SDKBase.VRC_Trigger>().ToList();
            ModulesFunctions.UdonBehaviours = WorldUtils.Get_UdonBehaviours().ToList();

            Config.RoomHistory.list.Add(new RoomHistoryObject()
            {
                Name = WorldUtils.GetCurrentWorld().name,
                JoinID = ModulesFunctions.CopyWorldID(),
            });

            if (WorldUtils.GetCurrentWorld().id != "wrld_1b3b3259-0a1f-4311-984e-826abab6f481")
            {
                if (ModulesFunctions.EliteBool)
                {
                    ModulesFunctions.EliteBool = false;
                }
            }

            if (Config.Main.AntiSeats)
            {
                ModulesFunctions.ToggleSeats(false);
            }
            else
            {
                ModulesFunctions.ToggleSeats(true);
            }
            if (Config.Main.OptMirrors)
            {
                ModulesFunctions.OptimizeMirrors(true);
            }
            else
            {
                ModulesFunctions.OptimizeMirrors(false);
            }
            if (Config.Main.PostProcessing)
            {
                ModulesFunctions.PostProcessing(false);
            }
            else
            {
                ModulesFunctions.PostProcessing(true);
            }
            if (Config.Main.VideoPlayer)
            {
                ModulesFunctions.AntiVideoplayer(true);
            }
            else
            {
                ModulesFunctions.AntiVideoplayer(false);
            }
            if (ModulesFunctions.PlayerCapsuleESP)
            {
                ModulesFunctions.PlayerCapsuleESP = true;
                ModulesFunctions.PlayerESP();
            }
            else
            {
                ModulesFunctions.PlayerCapsuleESP = false;
                ModulesFunctions.PlayerESP();
            }
            if (Config.Main.WorldPortals)
            {
                ModulesFunctions.AntiWorldPortals(true);
            }
            else
            {
                ModulesFunctions.AntiWorldPortals(false);
            }
            if (ModulesFunctions.PlayersMeshESP)
            {
                foreach (var Player in WorldUtils.GetPlayers()) ModulesFunctions.PlayerMeshEsp(Player, true);
                ModulesFunctions.PlayersMeshESP = true;
            }
            else
            {
                foreach (var Player in WorldUtils.GetPlayers()) ModulesFunctions.PlayerMeshEsp(Player, false);
                ModulesFunctions.PlayersMeshESP = false;
            }
            if (Config.Main.FpsUncap)
            {
                Application.targetFrameRate = 250;
            }
            else
            {
                Application.targetFrameRate = 90;
            }


            foreach (var m in Modules) m.WorldLoaded(RoomManager.field_Internal_Static_ApiWorld_0, RoomManager.field_Internal_Static_ApiWorldInstance_0);
        }

        private static IEnumerator WaitForPlayer()
        {
            while (VRCPlayer.field_Internal_Static_VRCPlayer_0 == null) yield return null;
            while (VRCPlayer.field_Internal_Static_VRCPlayer_0._player == null) yield return null;
            while (VRCPlayer.field_Internal_Static_VRCPlayer_0._player.field_Private_APIUser_0 == null) yield return null;
            foreach (var m in Modules) m.LocalPlayerLoaded(VRCPlayer.field_Internal_Static_VRCPlayer_0._player.field_Private_APIUser_0);
        }

        private static IEnumerator WaitForHUD()
        {
            while (GameObject.Find("UserInterface/UnscaledUI/HudContent_Old/Hud") == null) yield return null;
            UI.InitializeHUD();
            for (int i = 0; i < Modules.Count; i++)
                Modules[i].UI();
        }

        private static IEnumerator WaitForSM()
        {
            while (VRCUiManager.prop_VRCUiManager_0 == null) yield return null;
            UI.InitializeSocialMenu();
            AvatarSearch.InitializeUI();
            if (Config.Main.CustomUiColor)
            {
                MelonCoroutines.Start(Color_Edit.MenuColorEditSM());
            }
            yield break;
        }

        private static IEnumerator WaitForQM()
        {
            while (UnityEngine.Object.FindObjectOfType<VRC.UI.Elements.QuickMenu>() == null) yield return null;

            APIStuff.Left.Setup(GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/Wing_Left").transform);
            APIStuff.Right.Setup(GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/Wing_Right").transform);
            APIStuff.Left.WingOpen.GetComponent<Button>().onClick.AddListener(new System.Action(() => APIStuff.Init_L()));
            APIStuff.Right.WingOpen.GetComponent<Button>().onClick.AddListener(new System.Action(() => APIStuff.Init_R()));

            UI.InitializeQuickMenu();
            if (Config.Main.CustomUiColor)
            {
                MelonCoroutines.Start(Color_Edit.MenuColorEdit());
            }
            yield break;
        }

        private static IEnumerator WaitForAM()
        {
            while (ActionMenuDriver.prop_ActionMenuDriver_0 == null) yield return null;
            UI.InitializeActionMenu();
            yield break;
        }
    }
}
