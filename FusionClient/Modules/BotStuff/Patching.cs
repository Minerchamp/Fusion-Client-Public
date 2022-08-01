using HarmonyLib;
using MelonLoader;
using VRC;
using UnityEngine;
using System.Collections;
using ExitGames.Client.Photon;
using System;
using System.Reflection;
using FC.Utils;
using static VRC.SDKBase.VRC_EventHandler;
using VRC.Core;
using VRC.SDKBase;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Photon.Realtime;
using UnhollowerBaseLib;
using Object = UnityEngine.Object;
using Harmony;
using VRCSDK2;
using AccessTools = HarmonyLib.AccessTools;
using HarmonyMethod = HarmonyLib.HarmonyMethod;
using HarmonyPatchType = HarmonyLib.HarmonyPatchType;
using UnityEngine.UI;
using RootMotion.FinalIK;
using FusionClient.Modules;
using System.IO;
using System.Runtime.InteropServices;
using UnhollowerRuntimeLib.XrefScans;
using UnityEngine.Diagnostics;
using VRC.SDK.Internal.MeetingBunker;
using VRC.SDK.Internal.MeetingBunker.VRC_Presentation_Utils;
using FC;
using System.Net;
using static VRC.Core.API;
using System.Threading.Tasks;
using VRC.Networking;
using Fusion.Networking;
using Fusion.Networking.Serializable;
using FusionClient.Core;

namespace FusionClient.Modules.BotStuff
{
    public static class Patches
    {
        internal static bool IsLoading;
        public static List<string> Userids = new List<string>();
        internal static string PipeLineDupeHud;
        internal static string PipeLineDupeDebug;
        internal static string PipeLineDupeMsg;
        public class Patch
        {
            private static List<Patch> Patches = new List<Patch>();
            private static int Failed = 0;
            public MethodInfo TargetMethod { get; set; }
            public HarmonyMethod PrefixMethod { get; set; }
            public HarmonyMethod PostfixMethod { get; set; }
            public HarmonyLib.Harmony Instance { get; set; }

            public Patch(MethodInfo targetMethod, HarmonyMethod Before = null, HarmonyMethod After = null)
            {
                if (targetMethod == null || Before == null && After == null)
                {
                    //Logs.Error("[Patches] TargetMethod is NULL or Pre And PostFix are Null");
                    Failed++;
                    return;
                }
                Instance = new HarmonyLib.Harmony($"Patch:{targetMethod.DeclaringType.FullName}.{targetMethod.Name}");
                TargetMethod = targetMethod;
                PrefixMethod = Before;
                PostfixMethod = After;
                Patches.Add(this);
            }

            public static void DoPatches()
            {
                foreach (var patch in Patches)
                {
                    try
                    {
                        patch.Instance.Patch(patch.TargetMethod, patch.PrefixMethod, patch.PostfixMethod);
                        //Logs.Dev($"[Patches] Patched! {patch.TargetMethod.DeclaringType.FullName}.{patch.TargetMethod.Name} | with {patch.PrefixMethod?.method.Name}{patch.PostfixMethod?.method.Name}");
                        Logs.Log($"[Patches] Successfully patched {patch.PrefixMethod?.method.Name}{patch.PostfixMethod?.method.Name}!", ConsoleColor.Green);
                    }
                    catch
                    {
                        Failed++;
                        //Logs.Dev($"[Patches] Failed At {patch.TargetMethod?.Name} | {patch.PrefixMethod?.method.Name} | {patch.PostfixMethod?.method.Name}", ConsoleColor.Red);
                        Logs.Log($"[Patches] Failed to patch {patch.PrefixMethod?.method.Name}{patch.PostfixMethod?.method.Name}!", ConsoleColor.Red);
                    }
                }
                Logs.Log($"[Patches] Done! Patched {Patches.Count} Methods! and {Failed} Failed Patches!", ConsoleColor.Yellow);
            }
        }

        public static HarmonyMethod GetPatch(string name)
        {
            return new HarmonyMethod(typeof(Patches).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic));
        }

        private static IntPtr GetDetour(string name)
        {
            return typeof(Patches).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static)!.MethodHandle.GetFunctionPointer();
        }

        public static void Initialize()
        {

            new Patch(typeof(Photon.Realtime.LoadBalancingClient).GetMethod(nameof(Photon.Realtime.LoadBalancingClient.OnEvent)), GetPatch(nameof(OnPhotonReceiveEventPatch)));
            Patch.DoPatches();
            HookFadeTo();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void FadeToDelegate(IntPtr thisPtr, IntPtr fadeTypePtr, float duration, IntPtr action);
        private static FadeToDelegate _fadeToDelegate;

        private static void HookFadeTo() // added as a function due to being unable to run unsafe code in IEnumerators
        {
            unsafe
            {
                var originalMethod = *(IntPtr*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(VRCUiManager).GetMethod(nameof(VRCUiManager.Method_Public_Void_String_Single_Action_0))).GetValue(null);
                MelonUtils.NativeHookAttach((IntPtr)(&originalMethod), typeof(Patches).GetMethod(nameof(WorldRevealed), BindingFlags.Static | BindingFlags.NonPublic).MethodHandle.GetFunctionPointer());
                _fadeToDelegate = Marshal.GetDelegateForFunctionPointer<FadeToDelegate>(originalMethod);
                if (_fadeToDelegate == null)
                {
                    MelonLogger.Msg($"[Patches] Could not hook OnFadeTo", ConsoleColor.Red);
                }
            }
        }

        private static void WorldRevealed(IntPtr thisPtr, IntPtr fadeTypePtr, float duration, IntPtr action)
        {
            try
            {
                if (thisPtr != IntPtr.Zero && fadeTypePtr != IntPtr.Zero)
                {
                    string fadeType = IL2CPP.Il2CppStringToManaged(fadeTypePtr);
                    //ModConsole.Log("FadeType Called : " + fadeType + " With duration : " + duration, ConsoleColor.Yellow);
                    if (fadeType.Equals("BlackFade") && duration.Equals(0f) && RoomManager.field_Internal_Static_ApiWorldInstance_0 != null)
                    {
                        try
                        {
                            BotStuff.Patches.IsLoading = true;
                            Logs.Log($"World [{WorldUtils.GetCurrentWorld().name}] Loaded", ConsoleColor.Red);
                            if (Config.Main.LogBotEvents)
                            {
                                BotNetworkClient.Client.Send(new PacketData(PacketBotType.ROOM_JOINED, $"{WorldUtils.GetCurrentWorld().name}"));
                            }
                            for (int i = 0; i < Main.Modules.Count; i++)
                            {
                                FusionModule m = Main.Modules[i];
                                m.WorldLoaded(WorldUtils.GetCurrentWorld(), WorldUtils.GetCurrentInstance());
                            }
                        }
                        catch { }
                    }
                }
            }
            catch { }
            finally
            {
                _fadeToDelegate(thisPtr, fadeTypePtr, duration, action);
            }
        }

        #region Patch Methods

        #region OnPhotonReceiveEventPatch

        private static bool OnPhotonReceiveEventPatch(ref EventData __0)
        {
            if (__0 == null)
            {
                return false;
            }
            if (BotModule.IsBot && __0.Code != 7 && __0.Code != 5 && __0.Code != 8 && __0.Code != 202 && __0.Code != 254 && __0.Code != 255 && __0.Code != 33) return false;
            var plr2 = PlayerManager.prop_PlayerManager_0.GetPlayerID(__0.Sender);
            if (__0.Code == 7 && !BotModule.IsBot && BotModule.FollowSomeone)
            {
                if (plr2 == BotModule.SelectedTarget)
                {
                    byte[] movementdata = new Il2CppStructArray<byte>(__0.Parameters[245].Pointer);
                    if (movementdata == null || movementdata.Length < 60)
                    {
                        return true;
                    }
                    short ping = 69;
                    byte[] pingg = BitConverter.GetBytes(ping);
                    Buffer.BlockCopy(pingg, 0, movementdata, 68, pingg.Length);
                    BotServer.SendAll(new PacketData(PacketBotServerType.MOVE_LIKE, Convert.ToBase64String(movementdata)));
                    return true;
                }
                return true;
            }
            else
            if (__0.Code == 7 && BotModule.IsBot && BotModule.FollowMyGuy)
            {
                if (__0.Sender == BotModule.SelectedTarget.GetVRCPlayerApi().playerId)
                {
                    byte[] movementdata = new Il2CppStructArray<byte>(__0.Parameters[245].Pointer);
                    if (movementdata == null || movementdata.Length < 60)
                    {
                        return true;
                    }
                    byte[] viewIDOut = BitConverter.GetBytes(int.Parse(PlayerUtils.GetCurrentUser().prop_VRCPlayerApi_0.playerId + "00001"));
                    Buffer.BlockCopy(viewIDOut, 0, movementdata, 0, 4);
                    byte[] vecbytes = BufferRW.Vector3ToBytes(BotModule.GetOffset(BufferRW.ReadVector3(movementdata, 48)));
                    Buffer.BlockCopy(vecbytes, 0, movementdata, 48, vecbytes.Length);
                    MiscUtils.OpRaiseEvent(7, movementdata, new RaiseEventOptions()
                    {
                        field_Public_ReceiverGroup_0 = ReceiverGroup.Others,
                        field_Public_EventCaching_0 = EventCaching.DoNotCache,
                    }, SendOptions.SendUnreliable);
                    return true;
                }
                return true;
            }
            if (__0.Code == 1 && BotModule.VoiceMimic)
            {
                if (PlayerManager.prop_PlayerManager_0.GetPlayerID(__0.Sender).GetUserID() == BotModule.SelectedTarget.GetAPIUser().id)
                {
                    Logs.Log("Test", ConsoleColor.Cyan);
                    MiscUtils.OpRaiseEvent(1, __0.CustomData, new Photon.Realtime.RaiseEventOptions()
                    {
                        field_Public_ReceiverGroup_0 = Photon.Realtime.ReceiverGroup.Others,
                        field_Public_Byte_0 = 1,
                        field_Public_Byte_1 = 1,
                    }, SendOptions.SendUnreliable);
                }
            }

            return true;
        }


        #endregion

        #endregion
    }
}