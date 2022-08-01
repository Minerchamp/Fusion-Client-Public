namespace FusionClient.Modules
{
    using HarmonyLib;
    using MelonLoader;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using UnhollowerRuntimeLib;
    using UnityEngine;
    using UnityEngine.Audio;
    using VRC;
    using FC;
    using FC.Components;
    using FC.Utils;
    using Fusion.Networking;
    using Fusion.Networking.Serializable;
    using global::FusionClient.Modules.BotStuff;
    using FCConsole;
    using System.Threading;
    using FusionClient.Core;
    using VRC.Core;
    using System.Linq;

    internal class BotModule : FusionModule
    {

        public static string accounts = "";
        internal static GameObject GameObject;

        public static float OrbitSpeed = 1f;
        public static float BotDistance = 1f;
        public static int SpinbotSpeed = 10;
        public static float alpha = 0f;
        public static bool Spinbot = false;
        public static bool DEBUGTEST = false;
        public static int axismode = 2;
        public static float a = 1f;
        public static float b = 1f;
        public static bool FollowMyGuy = false;
        public static bool IsBot;
        public static int BotID = 0;
        public static int BotCount = 0;
        public static object Cancel;
        public static Player SelectedTarget = null;
        public static Player OrbitTarget = null;
        public static Player ParrotTarget = null;
        internal static VRCPlayer Target;
        public static bool FollowSomeone;
        public static Player FollowTarget;
        public static bool DumpNextEvent = false;
        public static bool Captcha = false;
        public static bool VoiceMimic = false;

        public override void Start()
        {
            if (IsBot)
            {
                System.Console.Clear();
                System.Console.Title = "FusionClient Bots V1.0";
                FCConsole.Console.WriteFigletWithGradient(FigletFont.LoadFromAssembly("Larry3D.flf"), "fC Bot", System.Drawing.Color.Red, System.Drawing.Color.DarkRed);
                Logs.Log("Initializing as Bot...", ConsoleColor.Red);
                Logs.Log("==============================", ConsoleColor.DarkRed);
                //Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;
                //ClassInjector.RegisterTypeInIl2Cpp<Optimizer>();
                BotStuff.Patches.Initialize();
                Logs.Log("Initialized", ConsoleColor.Red);
                Logs.Log("==============================", ConsoleColor.DarkRed);
                Logs.Log("Connecting to Socket...", ConsoleColor.Red);
                Logs.Log("==============================", ConsoleColor.DarkRed);
                for (; !BotNetworkingManager.IsReady;){}
                Logs.Log("Connected.", ConsoleColor.Red);
                Logs.Log("==============================", ConsoleColor.DarkRed);
                if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=1")))
                {
                    if (File.Exists("Fusion Client\\Misc\\BotLogins\\Bot1.txt"))
                    {
                        var Login1 = File.ReadAllText("Fusion Client\\Misc\\BotLogins\\Bot1.txt").Split(':');
                        MelonCoroutines.Start(Login(Login1[0], Login1[1]));
                    }
                }
                else if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=2")))
                {
                    if (File.Exists("Fusion Client\\Misc\\BotLogins\\Bot2.txt"))
                    {
                        var Login2 = File.ReadAllText("Fusion Client\\Misc\\BotLogins\\Bot2.txt").Split(':');
                        MelonCoroutines.Start(Login(Login2[0], Login2[1]));
                    }
                }
                else if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=3")))
                {
                    Logs.Log("Starte3");
                    if (File.Exists("Fusion Client\\Misc\\BotLogins\\Bot3.txt"))
                    {
                        Logs.Log("Started3");
                        var Login3 = File.ReadAllText("Fusion Client\\Misc\\BotLogins\\Bot3.txt").Split(':');
                        MelonCoroutines.Start(Login(Login3[0], Login3[1]));
                    }
                }
                else if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=4")))
                {
                    if (File.Exists("Fusion Client\\Misc\\BotLogins\\Bot4.txt"))
                    {
                        var Login4 = File.ReadAllText("Fusion Client\\Misc\\BotLogins\\Bot4.txt").Split(':');
                        MelonCoroutines.Start(Login(Login4[0], Login4[1]));
                    }
                }
                else if (Environment.GetCommandLineArgs().Any(a => a.Contains("--profile=5")))
                {
                    if (File.Exists("Fusion Client\\Misc\\BotLogins\\Bot5.txt"))
                    {
                        var Login5 = File.ReadAllText("Fusion Client\\Misc\\BotLogins\\Bot5.txt").Split(':');
                        MelonCoroutines.Start(Login(Login5[0], Login5[1]));
                    }
                }
                Logs.Log("Starting Optimization Routine...", ConsoleColor.Red);
                Logs.Log("==============================", ConsoleColor.DarkRed);
                MelonCoroutines.Start(RamClearLoop());
                Logs.Log("Started", ConsoleColor.Red);
                Logs.Log("==============================", ConsoleColor.DarkRed);
            }
        }

        public static IEnumerator Login(string Username, string Password)
        {
            yield return new WaitForSecondsRealtime(2);
            for (; ; )
            {
                if (APIUser.IsLoggedIn)
                {
                    Logs.Log("Stopped Auto Login Do To User Being Logged In Already");
                    yield break;
                }
                else
                {
                    if (GameObject.Find("UserInterface/MenuContent/Screens/Authentication"))
                    {
                        // Auto Login Username
                        GameObject.Find("UserInterface/MenuContent/Screens/Authentication/StoreLoginPrompt/VRChatButtonLogin").GetComponent<UnityEngine.UI.Button>().Press();
                        yield return new WaitForSeconds(1f);
                        GameObject.Find("UserInterface/MenuContent/Popups/InputPopup/ButtonLeft").GetComponent<UnityEngine.UI.Button>().Press();
                        yield return new WaitForSeconds(1f);
                        GameObject.Find("UserInterface/MenuContent/Screens/Authentication/LoginUserPass/BoxLogin/InputFieldUser").GetComponent<UiInputField>().prop_String_0 = Username;
                        yield return new WaitForSeconds(2f);
                        GameObject.Find("UserInterface/MenuContent/Screens/Authentication/LoginUserPass/BoxLogin/InputFieldUser").GetComponent<UnityEngine.UI.Button>().Press();
                        yield return new WaitForSeconds(1f);
                        GameObject.Find("UserInterface/MenuContent/Popups/InputPopup/ButtonRight").GetComponent<UnityEngine.UI.Button>().Press();
                        yield return new WaitForSeconds(1f);
                        // Auto Login Password
                        GameObject.Find("UserInterface/MenuContent/Screens/Authentication/LoginUserPass/BoxLogin/InputFieldPassword").GetComponent<UiInputField>().prop_String_0 = Password;
                        yield return new WaitForSeconds(2f);
                        GameObject.Find("UserInterface/MenuContent/Screens/Authentication/LoginUserPass/BoxLogin/InputFieldPassword").GetComponent<UnityEngine.UI.Button>().Press();
                        yield return new WaitForSeconds(1f);
                        GameObject.Find("UserInterface/MenuContent/Popups/InputPopup/ButtonRight").GetComponent<UnityEngine.UI.Button>().Press();
                        yield return new WaitForSeconds(1f);
                        yield break;
                    }
                }
                yield return new WaitForSeconds(2f);
            }
        }

        public static void StartBot(int BotCount)
        {
            MelonCoroutines.Start(Delayed());
            IEnumerator Delayed()
            {
                try
                {
                    foreach (var file in Directory.EnumerateFiles("Mods"))
                    {
                        string destFile = Path.Combine("Fusion Client\\Misc\\Mods", Path.GetFileName(file));
                        File.Move(file, destFile);
                    }
                }
                catch { }
                yield return new WaitForSeconds(0.2f);
                foreach (var file in Directory.EnumerateFiles("Fusion Client\\Misc\\Mods"))
                {
                    string destFile = Path.Combine("Mods", Path.GetFileName(file));
                    if (file.Contains("Boo.Lang.Log.dll"))
                    {
                        File.Move(file, destFile);
                    }
                    if (file.Contains("FusionLoader.dll"))
                    {
                        File.Move(file, destFile);
                    }
                    if (file.Contains("AstralCore.dll"))
                    {
                        File.Move(file, destFile);
                    }
                    if (file.Contains("AstralCore (1).dll"))
                    {
                        File.Move(file, destFile);
                    }
                    if (file.Contains("AstralCore (2).dll"))
                    {
                        File.Move(file, destFile);
                    }
                    if (file.Contains("AbyssCore.dll"))
                    {
                        File.Move(file, destFile);
                    }
                    if (file.Contains("AbyssBypass.dll"))
                    {
                        File.Move(file, destFile);
                    }
                }
                yield return new WaitForSeconds(4);

                if (Config.Main.DebugMode)
                {
                    Process.Start(Directory.GetCurrentDirectory() + $"\\VRChat.exe", $"--FCBot --profile={BotCount} --fps=25 --no-vr -noUpm -disable-gpu-skinning -no-stereo-rendering -nolog %2");
                }
                else
                {
                    Process.Start(Directory.GetCurrentDirectory() + $"\\VRChat.exe", $"--FCBot --profile={BotCount} --fps=25 --no-vr -noUpm -nographics -disable-gpu-skinning -no-stereo-rendering -batchmode -nolog %2 ");
                }

                yield return new WaitForSeconds(25);

                try
                {
                    foreach (var file in Directory.EnumerateFiles("Fusion Client\\Misc\\Mods"))
                    {
                        string destFile = Path.Combine("Mods", Path.GetFileName(file));
                        if (!File.Exists(destFile))
                            File.Move(file, destFile);
                    }
                }
                catch { }
            }
        }

        public override void Update()
        {
            lock (MainThreadQueue)
            {
                foreach (var action in MainThreadQueue)
                {
                    action();
                }
                MainThreadQueue.Clear();
            }

            if (BotModule.IsBot)
            {
                if (BotModule.Spinbot && PlayerUtils.GetCurrentUser() != null)
                {
                    PlayerUtils.GetCurrentUser().gameObject.transform.Rotate(new Vector3(0f, SpinbotSpeed, 0f));
                }
                if (BotModule.OrbitTarget != null && PlayerUtils.GetCurrentUser() != null)
                {
                    if (BotModule.BotID == 1)
                    {
                        BotModule.alpha += Time.deltaTime * OrbitSpeed;
                        PlayerUtils.GetCurrentUser().transform.position = new Vector3(BotModule.OrbitTarget.transform.position.x + (BotModule.a * (float)Math.Cos(BotModule.alpha)), BotModule.OrbitTarget.transform.position.y, BotModule.OrbitTarget.transform.position.z + (BotModule.b * (float)Math.Sin(BotModule.alpha)));
                    }
                    else
                    {
                        BotModule.alpha += Time.deltaTime * OrbitSpeed;
                        PlayerUtils.GetCurrentUser().transform.position = new Vector3(BotModule.OrbitTarget.transform.position.x + (BotModule.a * -(float)Math.Cos(BotModule.alpha)), BotModule.OrbitTarget.transform.position.y, BotModule.OrbitTarget.transform.position.z + (BotModule.b * -(float)Math.Sin(BotModule.alpha)));
                    }
                }
                //if (BotModule.DEBUGTEST)
                //{
                //    var movementdata = File.ReadAllText("Fusion Client\\Logs\\Event7.txt").Split('\n');
                //    byte[] viewIDOut = BitConverter.GetBytes(int.Parse(PlayerUtils.GetCurrentUser().GetVRCPlayerApi().playerId + "00001"));
                //    Buffer.BlockCopy(viewIDOut, 0, movementdata, 0, 4);

                //    byte[] vecbytes = BufferRW.Vector3ToBytes(BotModule.GetOffset(PlayerUtils.GetCurrentUser().transform.position));
                //    Buffer.BlockCopy(vecbytes, 0, movementdata, 48, vecbytes.Length);
                //    MiscUtils.OpRaiseEvent(7, movementdata, BotModule.UnreliableEventOptions, BotModule.UnreliableOptions);
                //}
            }
        }

        public override void SceneLoaded(int buildIndex, string sceneName)
        {
            if (BotModule.IsBot)
            {
                if (!WorldUtils.IsInWorld())
                {
                    if (Config.Main.LogBotEvents)
                    {
                        if (BotStuff.Patches.IsLoading)
                        {
                            BotNetworkClient.Client.Send(new PacketData(PacketBotType.LEVEL_CHANGED));
                        }
                    }
                }
                if (GameObject.Find("UserInterface/MenuContent/Popups/InputCaptchaPopup"))
                {
                    if (Config.Main.LogBotEvents)
                    {
                        if (!Captcha)
                        {
                            BotNetworkClient.Client.Send(new PacketData(PacketBotType.LEVEL_CHANGED, "Captcha"));
                            Captcha = true;
                        }
                    }
                }
            }
        }

        public static Photon.Realtime.RaiseEventOptions UnreliableEventOptions = new Photon.Realtime.RaiseEventOptions
        {
            field_Public_ReceiverGroup_0 = Photon.Realtime.ReceiverGroup.Others,
            field_Public_EventCaching_0 = Photon.Realtime.EventCaching.DoNotCache
        };

        public static ExitGames.Client.Photon.SendOptions UnreliableOptions = new ExitGames.Client.Photon.SendOptions
        {
            DeliveryMode = ExitGames.Client.Photon.DeliveryMode.Unreliable
        };

        public static Vector3 GetOffset(Vector3 pos)
        {
            float offset = BotDistance;
            if (BotID == 1)
            {
                switch (axismode)
                {
                    case 0: pos.x += offset; break;
                    case 1: pos.x += offset; pos.z += offset; break;
                    case 2: pos.z += offset; break;
                    case 3: pos.x -= offset; pos.z -= offset; break;
                    case 4: pos.x -= offset; break;
                    case 5: pos.x += offset; break;
                    case 6: pos.z += offset; break;
                    case 7: pos.x -= offset; break;
                    case 8: pos.z -= offset; break;
                    case 9: pos.y += offset; break;
                    case 10: pos.x += 1; pos.z += offset; break;
                    case 11: pos.x += 1; pos.z -= offset; break;
                    case 12: pos.x += offset; pos.z += offset; break;
                    case 13: pos.x -= offset; pos.z += offset; break;
                }
            }
            else if(BotID == 2)
            {
                switch (axismode)
                {
                    case 0: pos.x -= offset; break;
                    case 1: pos.x -= offset; pos.z -= offset; break;
                    case 2: pos.z -= offset; break;
                    case 3: pos.x += offset; pos.z += offset; break;
                    case 4: pos.x += offset; break;
                    case 5: pos.x += offset * 2; break;
                    case 6: pos.z += offset * 2; break;
                    case 7: pos.x -= offset * 2; break;
                    case 8: pos.z -= offset * 2; break;
                    case 9: pos.y += offset * 2; break;
                    case 10: pos.z += offset; break;
                    case 11: pos.z -= offset; break;
                    case 12: pos.x += offset; pos.z -= offset; break;
                    case 13: pos.x -= offset; pos.z -= offset; break;
                }
            }
            if (OrbitTarget != null && PlayerUtils.GetCurrentUser() != null)
            {
                if (BotID == 1)
                {
                    pos = new Vector3(OrbitTarget.transform.position.x + (a * (float)Math.Cos(alpha)), OrbitTarget.transform.position.y, OrbitTarget.transform.position.z + (b * (float)Math.Sin(alpha)));
                }
                else
                {
                    alpha += Time.deltaTime * OrbitSpeed;
                    pos = new Vector3(OrbitTarget.transform.position.x + (a * -(float)Math.Cos(alpha)), OrbitTarget.transform.position.y, OrbitTarget.transform.position.z + (b * -(float)Math.Sin(alpha)));
                }

            }
            return pos;
        }

        public static void ReceiveAction(Action action)
        {
            HandleActionOnMainThread(action);
        }

        private static List<Action> MainThreadQueue = new List<Action>();

        private static void HandleActionOnMainThread(Action action)
        {
            MainThreadQueue.Add(action);
        }

        private static IEnumerator RamClearLoop()
        {
            for (; ; )
            {
                yield return new WaitForSeconds(5f);
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
            }
        }
    }
}
