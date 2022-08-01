using Blaze.API.AW;
using Fusion.Networking;
using Fusion.Networking.Serializable;
using ExitGames.Client.Photon;
using FC.Utils;
using FusionClient.API;
using FusionClient.Modules;
using FusionClient.Utils.Objects.Mod;
using FusionClient.Utils.VRChat;
using HarmonyLib;
using MelonLoader;
using Microsoft.Win32;
using Newtonsoft.Json;
using Photon.Realtime;
using RootMotion.FinalIK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Transmtn;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib.XrefScans;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using VRC;
using VRC.Core;
using VRC.Networking;
using VRC.SDKBase;
using VRCSDK2;
using static VRC.Core.API;
using static VRC.SDKBase.VRC_EventHandler;
using Analytics = MonoBehaviourPublicStInStDiIn2ObInSiVRUnique;
using FusionClient.Utils.Manager;
using FusionClient.Modules.AntiCrash;
using FusionClient.Utils.NativePatchUtils;

namespace FusionClient.Core
{
    public static class Patches
    {
        public static List<string> Userids = new List<string>();
        internal static string PipeLineDupeHud;
        internal static string PipeLineDupeDebug;
        internal static string PipeLineDupeMsg;
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void VoidDelegate(IntPtr thisPtr, IntPtr nativeMethodInfo);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr ObjectInstantiateDelegate(IntPtr assetPtr, Vector3 pos, Quaternion rot, byte allowCustomShaders, byte isUI, byte validate, IntPtr nativeMethodPointer);
        private static VRCAvatarManager cachedManager;
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
                for (int i = 0; i < Patches.Count; i++)
                {
                    Patch patch = Patches[i];
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

        private delegate IntPtr AssetBundleDownloadDelegate(IntPtr hiddenValueTypeReturn, IntPtr thisPtr, IntPtr apiAvatarPtr, IntPtr multicastDelegatePtr, bool param3, IntPtr nativeMethodInfo);
        private static AssetBundleDownloadDelegate ourABDownloadDelegate;
        private static string LastLoggedAssetBundleID;

        public static void Initialize()
        {
            try
            {
                #region Anti Crash Shit

                var matchingMethods = typeof(AssetManagement)
   .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).Where(it =>
       it.Name.StartsWith("Method_Public_Static_Object_Object_Vector3_Quaternion_Boolean_Boolean_Boolean_") && it.GetParameters().Length == 6).ToList();

                foreach (var matchingMethod in matchingMethods)
                {
                    ObjectInstantiateDelegate originalInstantiateDelegate = null;
                    ObjectInstantiateDelegate replacement = (assetPtr, pos, rot, allowCustomShaders, isUI, validate, nativeMethodPointer) =>
                        NativeObjectInstantiatePatch(assetPtr, pos, rot, allowCustomShaders, isUI, validate, nativeMethodPointer, originalInstantiateDelegate);
                    NativePatchUtils.NativePatch(matchingMethod, out originalInstantiateDelegate, replacement);
                }

                foreach (var nestedType in typeof(VRCAvatarManager).GetNestedTypes())
                {
                    var moveNext = nestedType.GetMethod("MoveNext");
                    if (moveNext == null) continue;
                    var avatarManagerField = nestedType.GetProperties().SingleOrDefault(it => it.PropertyType == typeof(VRCAvatarManager));
                    if (avatarManagerField == null) continue;

                    MelonDebug.Msg($"Patching UniTask type {nestedType.FullName}");
                    var fieldOffset = (int)IL2CPP.il2cpp_field_get_offset((IntPtr)UnhollowerUtils
                        .GetIl2CppFieldInfoPointerFieldForGeneratedFieldAccessor(avatarManagerField.GetMethod)
                        .GetValue(null));
                    unsafe
                    {
                        var originalMethodPointer = *(IntPtr*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(moveNext).GetValue(null);
                        originalMethodPointer = XrefScannerLowLevel.JumpTargets(originalMethodPointer).First();
                        VoidDelegate originalDelegate = null;

                        void TaskMoveNextPatch(IntPtr taskPtr, IntPtr nativeMethodInfo)
                        {
                            var avatarManager = *(IntPtr*)(taskPtr + fieldOffset - 16);
                            cachedManager = new VRCAvatarManager(avatarManager);
                            originalDelegate(taskPtr, nativeMethodInfo);
                            cachedManager = null;
                        }

                        var patchDelegate = new VoidDelegate(TaskMoveNextPatch);

                        NativePatchUtils.NativePatch(originalMethodPointer, out originalDelegate, patchDelegate);
                    }
                }

                #endregion
                new Patch(typeof(Analytics).GetMethod(nameof(Analytics.Update)), GetPatch(nameof(CancelMethod)), null);
                new Patch(typeof(Analytics).GetMethod(nameof(Analytics.Start)), GetPatch(nameof(CancelMethod)), null);
                new Patch(typeof(Analytics).GetMethod(nameof(Analytics.OnEnable)), GetPatch(nameof(CancelMethod)), null);
                new Patch(typeof(AmplitudeSDKWrapper.AmplitudeWrapper).GetMethod(nameof(AmplitudeSDKWrapper.AmplitudeWrapper.UpdateServer)), GetPatch(nameof(CancelMethod)), null);
                new Patch(typeof(AmplitudeSDKWrapper.AmplitudeWrapper).GetMethod(nameof(AmplitudeSDKWrapper.AmplitudeWrapper.UpdateServerDelayed)), GetPatch(nameof(CancelMethod)), null);
                new Patch(typeof(PhotonPeer).GetProperty("RoundTripTime").GetGetMethod(), GetPatch(nameof(FakePing)), null);
                new Patch(typeof(Time).GetProperty("smoothDeltaTime").GetGetMethod(), GetPatch(nameof(FakeFPS)), null);
                new Patch(typeof(VRC.SDKBase.VRC_EventHandler).GetMethod(nameof(VRC.SDKBase.VRC_EventHandler.InternalTriggerEvent)), GetPatch(nameof(InternalTriggerEvent)), null);
                new Patch(AccessTools.Method(typeof(VRC_EventDispatcherRFC), "Method_Public_Void_Player_VrcEvent_VrcBroadcastType_Int32_Single_0"), GetPatch(nameof(RPCEvent)));
                new Patch(AccessTools.Method(typeof(LoadBalancingClient), nameof(LoadBalancingClient.OnEvent)), GetPatch(nameof(OnEventLog)));
                new Patch(typeof(PortalTrigger).GetMethod(nameof(PortalTrigger.OnTriggerEnter), BindingFlags.Public | BindingFlags.Instance), GetPatch(nameof(PortalTriggerCall)));
                new Patch(typeof(VRCPlayer).GetMethod(nameof(VRCPlayer.Awake)), null, GetPatch(nameof(OnAvatarChanged)));
                if (BlazesXRefs.OnPlayerJoinedMethod != null)
                    new Patch(BlazesXRefs.OnPlayerJoinedMethod, GetPatch(nameof(PlayerJoin)));
                if (BlazesXRefs.OnPlayerLeftMethod != null)
                    new Patch(BlazesXRefs.OnPlayerLeftMethod, GetPatch(nameof(PlayerLeft)));
                new Patch(typeof(PortalInternal).GetMethod("Method_Private_Void_1"), GetPatch(nameof(DestroyPortalPrefix)), null);
                new Patch(typeof(LoadBalancingClient).GetMethod(nameof(LoadBalancingClient.Method_Public_Virtual_New_Boolean_Byte_Object_RaiseEventOptions_SendOptions_0)), GetPatch("InstanceLockPatches"));
                new Patch(typeof(PhotonPeer1PublicObSiInSiBoInObLiBo1Unique).GetMethod(nameof(PhotonPeer1PublicObSiInSiBoInObLiBo1Unique.Method_Public_Virtual_New_Boolean_Byte_Object_RaiseEventOptions_SendOptions_0)), GetPatch("InstanceLockPatches"));
                //new Patch(typeof(PhotonPeer1PublicObSiInSiBoInObLiBo1Unique).GetMethod(nameof(PhotonPeer1PublicObSiInSiBoInObLiBo1Unique.Method_Public_Virtual_New_Boolean_Byte_Object_RaiseEventOptions_SendOptions_1)), GetPatch("InstanceLockPatches"));
                new Patch(typeof(IKSolverHeuristic).GetMethods().Where(m => m.Name.Equals("IsValid") && m.GetParameters().Length == 1).First(), GetPatch("IsValid"));
                if (PlaceUiMethod != null)
                    new Patch(typeof(VRCUiManager).GetMethod(PlaceUiMethod.Name), GetPatch(nameof(PlaceUiPatch)));
                new Patch(AccessTools.Method(typeof(VRC.Core.API), "SendPutRequest"), GetPatch(nameof(SendPutRequest)));
                new Patch(typeof(VRC_SyncVideoPlayer).GetMethod(nameof(VRC_SyncVideoPlayer.AddURL)), GetPatch(nameof(OnVideoPlayerUrlQueuedPatch)));
                new Patch(typeof(VRC_SyncVideoPlayer).GetMethod(nameof(VRC_SyncVideoPlayer.Play)), GetPatch(nameof(OnVideoPlayerUrlPlayedPatch)));
                new Patch(typeof(VRC_SyncVideoPlayer).GetMethod(nameof(VRC_SyncVideoPlayer.PlayIndex)), GetPatch(nameof(OnVideoPlayerUrlPlayedIndexPatch)));
                new Patch(AccessTools.Method(typeof(UdonSync), "UdonSyncRunProgramAsRPC"), GetPatch(nameof(UdonEvents)));
                new Patch(typeof(AssetManagement).GetMethod(nameof(AssetManagement.Method_Public_Static_Object_Object_Boolean_Boolean_Boolean_0)), GetPatch(nameof(OnObjectInstantiated)));
                if (BlazesXRefs.ActionWheelMethod != null)
                    new Patch(BlazesXRefs.ActionWheelMethod, null, GetPatch(nameof(OpenMainPage)));
                SetupPipelinePatch();
                SetupAssetBundleDownloadManagerUnitask();
                Patch.DoPatches();
            }
            catch (Exception ex)
            {
                Logs.Log($"{ex}");
            }
        }

        private static void SetupAssetBundleDownloadManagerUnitask()
        {
            NativePatchManager.NativePatch(typeof(AssetBundleDownloadManager).
                GetMethod(nameof(AssetBundleDownloadManager.Method_Internal_UniTask_1_InterfacePublicAbstractIDisposableGaObGaUnique_ApiAvatar_MulticastDelegateNInternalSealedVoUnUnique_Boolean_0))!,
                out ourABDownloadDelegate, AssetBundleDownloadUnitaskPatch);
        }

        #region Patch Methods

        private static bool CancelMethod()
        {
            return false;
        }

        #region AntiInstinceLock

        public static void AntiLockInstance(int buildIndex, string sceneName)
        {
            if (buildIndex == -1)
            {
                IEnumerator VRCPlayerWait()
                {
                    while (VRCPlayer.field_Internal_Static_VRCPlayer_0 == null)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                    yield return new WaitForSecondsRealtime(10);
                    if (Config.Main.AntiLockInstance)
                    {
                        //MonoBehaviour2PublicSiInBoSiObLiOb1PrDoUnique.field_Internal_Static_MonoBehaviour2PublicSiInBoSiObLiOb1PrDoUnique_0.field_Internal_MonoBehaviour1NPublicObPrPrPrUnique_0.field_Private_Boolean_0 = true;
                        //VRC_EventLog.field_Internal_Static_VRC_EventLog_0.field_Internal_MonoBehaviour1NPublicObPrPrPrUnique_0.field_Private_Boolean_0 = true;
                        //VRC_EventLog.field_Internal_Static_VRC_EventLog_0.field_Private_MonoBehaviourPrivateBo1SiObNuObSiIn1UIUnique_0.field_Private_Boolean_0 = true;
                        //VRC_EventLog.field_Internal_Static_VRC_EventLog_0.field_Private_MonoBehaviourPrivateStBo1SiObNuObSiIn1Unique_0.field_Private_Boolean_0 = true;
                        //VRC_EventLog.field_Internal_Static_VRC_EventLog_0.field_Private_MonoBehaviourPrivateOb1SiNuSiIn1BoSiNuUnique_0.field_Private_Boolean_0 = true;
                        VRC_EventLog.field_Internal_Static_VRC_EventLog_0.field_Internal_EventReplicator_0.field_Private_Boolean_0 = true;
                    }
                }
                MelonCoroutines.Start(VRCPlayerWait());
            }
        }

        #endregion

        #region ObjectInstantiatePatch

        private static IntPtr NativeObjectInstantiatePatch(IntPtr assetPtr, Vector3 pos, Quaternion rot, byte allowCustomShaders, byte isUI, byte validate, IntPtr nativeMethodPointer, ObjectInstantiateDelegate originalInstantiateDelegate)
        {
            if (cachedManager == null || assetPtr == IntPtr.Zero)
                return originalInstantiateDelegate(assetPtr, pos, rot, allowCustomShaders, isUI, validate, nativeMethodPointer);
            var avatarManager = cachedManager;
            var vrcPlayer = avatarManager.field_Private_VRCPlayer_0;
            if (vrcPlayer == null) return originalInstantiateDelegate(assetPtr, pos, rot, allowCustomShaders, isUI, validate, nativeMethodPointer);
            //if (vrcPlayer == VRCPlayer.field_Internal_Static_VRCPlayer_0) // never apply to self
            //    return originalInstantiateDelegate(assetPtr, pos, rot, allowCustomShaders, isUI, validate, nativeMethodPointer);
            var go = new UnityEngine.Object(assetPtr).TryCast<GameObject>();
            if (go == null)
                return originalInstantiateDelegate(assetPtr, pos, rot, allowCustomShaders, isUI, validate, nativeMethodPointer);
            var wasActive = go.activeSelf;
            go.SetActive(false);
            var result = originalInstantiateDelegate(assetPtr, pos, rot, allowCustomShaders, isUI, validate, nativeMethodPointer);
            go.SetActive(wasActive);
            if (result == IntPtr.Zero) return result;
            var instantiated = new GameObject(result);
            try
            {
                AntiCrash.CleanAvatar(cachedManager, instantiated);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Exception when cleaning avatar: {ex}");
            }

            return result;
        }


        #endregion

        #region AssetBundleDownloader

        private static IntPtr AssetBundleDownloadUnitaskPatch(IntPtr hiddenValueTypeReturn, IntPtr thisPtr, IntPtr apiAvatarPtr, IntPtr multicastDelegatePtr, bool param3, IntPtr nativeMethodInfo)
        {
            if (apiAvatarPtr != IntPtr.Zero)
            {
                ApiAvatar avi = new(apiAvatarPtr);
                if (Config.IdBlackList.list.Contains(avi.id))
                {
                    apiAvatarPtr = IntPtr.Zero;
                    var user = PlayerUtils.GetPlayerByAviID(avi.id);
                    if (user != null || LastLoggedAssetBundleID == avi.id)
                    {
                        Logs.Log($"<color=red>Blocked </color><color=cyan>{user.field_Private_APIUser_0.displayName}'s </color><color=red>avatar. Blacklisted ({avi.id})</color>>");
                        Logs.Debug($"<color=cyan>{user.field_Private_APIUser_0.displayName}</color> <color=yello>-></color> <color=red>Blacklisted Avatar</color>");
                        Logs.Hud($"<color=cyan>{user.field_Private_APIUser_0.displayName}</color> -> <color=red>Blacklisted Avatar</color>");
                        LastLoggedAssetBundleID = avi.id;
                    }
                }
                //if (Config.Main.LogAssetBundles && apiAvatarPtr != IntPtr.Zero)
                //{
                Logs.Log($"Downloading Asset Bundle {avi.id} ({avi.name})", ConsoleColor.Cyan);
                //}
            }
            return ourABDownloadDelegate(hiddenValueTypeReturn, thisPtr, apiAvatarPtr, multicastDelegatePtr, param3, nativeMethodInfo);
        }

        #endregion

        #region AW Patch

        private static void OpenMainPage(ActionMenu __instance)
        {
            ActionWheelAPI.OpenMainPage(__instance);
        }

        #endregion

        #region PipelinePatch
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ProcessPipelineDelegate(IntPtr instancePtr, IntPtr senderPtr, IntPtr argsPtr);
        private static ProcessPipelineDelegate processPipelineDelegate = null;

        private static void SetupPipelinePatch()
        {
            MethodInfo webSocketMethod = typeof(WebsocketPipeline).GetMethod(nameof(WebsocketPipeline._ProcessPipe_b__21_0));
            unsafe
            {
                IntPtr originalMethodPointer = *(IntPtr*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(webSocketMethod).GetValue(null);
                MelonUtils.NativeHookAttach((IntPtr)(&originalMethodPointer), GetDetour(nameof(OnDataReceivedPatch)));
                processPipelineDelegate = Marshal.GetDelegateForFunctionPointer<ProcessPipelineDelegate>(originalMethodPointer);
            }
        }

        private static void OnDataReceivedPatch(IntPtr instancePtr, IntPtr senderPtr, IntPtr argsPtr)
        {
            processPipelineDelegate(instancePtr, senderPtr, argsPtr);
            if (argsPtr == null || argsPtr == IntPtr.Zero)
            {
                return;
            }
            string data;
            try
            {
                unsafe
                {
                    nint dataOffset = (nint)argsPtr + 16;
                    data = IL2CPP.Il2CppStringToManaged(*(IntPtr*)dataOffset);
                }
            }
            catch (Exception) { return; }
            try
            {
                var WebSocketRawData = JsonConvert.DeserializeObject<VRCWebSocketObject>(data);
                var WebSocketData = JsonConvert.DeserializeObject<VRCWebSocketContent>(WebSocketRawData?.content);
                var apiuser = WebSocketData?.user;
                if (WebSocketData.userId == APIUser.CurrentUser.id) return;

                switch (WebSocketRawData?.type)
                {
                    case "user-location": // Self Location Update (called on first world load in)
                        break;

                    case "friend-update": // friend updates personal info
                        break;

                    case "notification": // Whenever any form of a notification is sent or recieved (ignoring this since we manually patch Transmtn Put & Send)
                        break;

                    case "friend-active": // Whenever someone becomes Active
                        if (Config.Main.PlayerLog)
                        {
                            /*if (Config.Main.PipelineHudOnlyFavs)
                            {
                                if (!APIUser.CurrentUser._favoriteFriendIdsInGroup.First().Contains(WebSocketData.userId)) return;
                            }*/
                            if (PipeLineDupeHud != $"<color=cyan>[PIPELINE]</color> <color=cyan>{apiuser.displayName}</color> -> <color=yellow>Active</color>")
                            {
                                PipeLineDupeHud = $"<color=cyan>[PIPELINE]</color> <color=cyan>{apiuser.displayName}</color> -> <color=yellow>Active</color>";
                                Logs.Hud($"<color=cyan>FusionClient</color> | <color=cyan>{apiuser.displayName}</color> -> <color=yellow>Active</color>");
                            }
                            if (PipeLineDupeDebug != $"<color=cyan>PIPELINE |</color> <color=cyan>{apiuser.displayName}</color> is now <color=yellow>Active</color>")
                            {
                                PipeLineDupeDebug = $"<color=cyan>PIPELINE |</color> <color=cyan>{apiuser.displayName}</color> is now <color=yellow>Active</color>";
                                Logs.Debug($"<color=cyan>PIPELINE |</color> <color=cyan>{apiuser.displayName}</color> is now <color=yellow>Active</color>");
                            }
                            if (PipeLineDupeMsg != $"[PIPELINE] {apiuser.displayName} is now Active!")
                            {
                                PipeLineDupeMsg = $"[PIPELINE] {apiuser.displayName} is now Active!";
                                Logs.Log($"[PIPELINE] {apiuser.displayName} is now Active!", ConsoleColor.Cyan);
                            }
                        }
                        break;

                    case "friend-online": // Whenever someone comes online
                        if (Config.Main.PlayerLog)
                        {
                            /*if (Config.Main.PipelineHudOnlyFavs)
                            {
                                if (!APIUser.CurrentUser._favoriteFriendIdsInGroup.First().Contains(WebSocketData.userId)) return;
                            }*/
                            if (PipeLineDupeHud != $"<color=cyan>[PIPELINE]</color> <color=cyan>{apiuser.displayName}</color> -> <color=green>online</color>")
                            {
                                PipeLineDupeHud = $"<color=cyan>[PIPELINE]</color> <color=cyan>{apiuser.displayName}</color> -> <color=green>online</color>";
                                Logs.Hud($"<color=cyan>FusionClient</color> | <color=cyan>{apiuser.displayName}</color> -> <color=green>online</color>");
                            }
                            if (PipeLineDupeDebug != $"<color=cyan>PIPELINE |</color> <color=cyan>{apiuser.displayName}</color> has come <color=green>online</color>")
                            {
                                PipeLineDupeDebug = $"<color=cyan>PIPELINE |</color> <color=cyan>{apiuser.displayName}</color> has come <color=green>online</color>";
                                Logs.Debug($"<color=cyan>PIPELINE |</color> <color=cyan>{apiuser.displayName}</color> has come <color=green>online</color>");
                            }
                            if (PipeLineDupeMsg != $"[PIPELINE] {apiuser.displayName} has come online!")
                            {
                                PipeLineDupeMsg = $"[PIPELINE] {apiuser.displayName} has come online!";
                                Logs.Log($"[PIPELINE] {apiuser.displayName} has come online!", ConsoleColor.Cyan);
                            }
                        }
                        break;

                    case "friend-offline": // Whenever someone goes offline
                        if (Config.Main.PlayerLog)
                        {
                            /*if (Config.Main.PipelineHudOnlyFavs)
                            {
                                if (!APIUser.CurrentUser._favoriteFriendIdsInGroup.First().Contains(WebSocketData.userId)) return;
                            }*/
                            if (PipeLineDupeHud != $"<color=cyan>[PIPELINE]</color> <color=cyan>{apiuser.displayName}</color> -> <color=red>offline</color>")
                            {
                                PipeLineDupeHud = $"<color=cyan>[PIPELINE]</color> <color=cyan>{apiuser.displayName}</color> -> <color=red>offline</color>";
                                Logs.Hud($"<color=cyan>FusionClient</color> | <color=cyan>{apiuser.displayName}</color> -> <color=red>offline</color>");
                            }
                            if (PipeLineDupeDebug != $"<color=cyan>PIPELINE |</color> <color=cyan>{apiuser.displayName}</color> has gone <color=red>offline</color>")
                            {
                                PipeLineDupeDebug = $"<color=cyan>PIPELINE |</color> <color=cyan>{apiuser.displayName}</color> has gone <color=red>offline</color>";
                                Logs.Debug($"<color=cyan>PIPELINE |</color> <color=cyan>{apiuser.displayName}</color> has gone <color=red>offline</color>");
                            }
                            if (PipeLineDupeMsg != $"[PIPELINE] {apiuser.displayName} has gone offline!")
                            {
                                PipeLineDupeMsg = $"[PIPELINE] {apiuser.displayName} has gone offline!";
                                Logs.Log($"[PIPELINE] {apiuser.displayName} has gone offline!", ConsoleColor.Cyan);
                            }
                        }
                        break;

                    case "friend-location": // Whenever someone changes worlds
                        if (Config.Main.PlayerLog)
                        {
                            /*if (Config.Main.PipelineHudOnlyFavs)
                            {
                                if (!APIUser.CurrentUser._favoriteFriendIdsInGroup.First().Contains(WebSocketData.userId)) return;
                            }*/

                            if (PipeLineDupeHud != $"<color=cyan>[PIPELINE]</color> <color=cyan>{apiuser.displayName}</color> -> <color=yellow>{(WebSocketData.location == "private" ? "[PRIVATE]" : WebSocketData.world.name)}</color>")
                            {
                                PipeLineDupeHud = $"<color=cyan>[PIPELINE]</color> <color=cyan>{apiuser.displayName}</color> -> <color=yellow>{(WebSocketData.location == "private" ? "[PRIVATE]" : WebSocketData.world.name)}</color>";
                                Logs.Hud($"<color=cyan>FusionClient</color> | <color=cyan>{apiuser.displayName}</color> -> <color=yellow>{(WebSocketData.location == "private" ? "[PRIVATE]" : WebSocketData.world.name)}</color>");
                            }
                            if (PipeLineDupeDebug != $"<color=cyan>PIPELINE |</color> <color=cyan>{apiuser.displayName}</color> went to <color=yellow>{(WebSocketData.location == "private" ? "[PRIVATE]" : WebSocketData.world.name)}</color>")
                            {
                                PipeLineDupeHud = $"<color=cyan>PIPELINE |</color> <color=cyan>{apiuser.displayName}</color> went to <color=yellow>{(WebSocketData.location == "private" ? "[PRIVATE]" : WebSocketData.world.name)}</color>";
                                Logs.Debug($"<color=cyan>PIPELINE |</color> <color=cyan>{apiuser.displayName}</color> went to <color=yellow>{(WebSocketData.location == "private" ? "[PRIVATE]" : WebSocketData.world.name)}</color>");
                            }
                            if (PipeLineDupeMsg != $"[PIPELINE] {apiuser.displayName} went to {(WebSocketData.location == "private" ? "[PRIVATE]" : $"{WebSocketData.world.name} [{WebSocketData.location}]")}!")
                            {
                                PipeLineDupeMsg = $"[PIPELINE] {apiuser.displayName} went to {(WebSocketData.location == "private" ? "[PRIVATE]" : $"{WebSocketData.world.name} [{WebSocketData.location}]")}!";
                                Logs.Log($"[PIPELINE] {apiuser.displayName} went to {(WebSocketData.location == "private" ? "[PRIVATE]" : $"{WebSocketData.world.name} [{WebSocketData.location}]")}!", ConsoleColor.Cyan);
                            }
                        }
                        break;

                    case "friend-delete": // Whenever someone remove you as a friend
                        if (Config.Main.PlayerLog)
                        {
                            if (PipeLineDupeHud != $"<color=cyan>[PIPELINE]</color> <color=cyan>{apiuser.displayName}</color> -> <color=yellow>REMOVED</color>")
                            {
                                PipeLineDupeHud = $"<color=cyan>[PIPELINE]</color> <color=cyan>{apiuser.displayName}</color> -> <color=yellow>REMOVED</color>";
                                Logs.Hud($"<color=cyan>FusionClient</color> | <color=cyan>{apiuser.displayName}</color> -> <color=yellow>REMOVED</color>");
                            }
                            if (PipeLineDupeDebug != $"<color=cyan>PIPELINE |</color> <color=cyan>{apiuser.displayName}</color> has <color=yellow>removed you</color>")
                            {
                                PipeLineDupeDebug = $"<color=cyan>PIPELINE |</color> <color=cyan>{apiuser.displayName}</color> has <color=yellow>removed you</color>";
                                Logs.Debug($"<color=cyan>PIPELINE |</color> <color=cyan>{apiuser.displayName}</color> has <color=yellow>removed you</color>");
                            }
                            if (PipeLineDupeMsg != $"[PIPELINE] {apiuser.displayName} has removed you!")
                            {
                                PipeLineDupeMsg = $"[PIPELINE] {apiuser.displayName} has removed you!";
                                Logs.Log($"[PIPELINE] {apiuser.displayName} has removed you!", ConsoleColor.Cyan);
                            }
                        }
                        break;

                    case "friend-add": // Whenever someone adds you as a friend
                        if (Config.Main.PlayerLog)
                        {
                            /*if (Config.Main.PipelineHudOnlyFavs)
                            {
                                if (!APIUser.CurrentUser._favoriteFriendIdsInGroup.First().Contains(WebSocketData.userId)) return;
                            }*/
                            if (PipeLineDupeHud != $"<color=cyan>[PIPELINE]</color> <color=cyan>{apiuser.displayName}</color> -> <color=yellow>ADDED</color>")
                            {
                                PipeLineDupeHud = $"<color=cyan>[PIPELINE]</color> <color=cyan>{apiuser.displayName}</color> -> <color=yellow>ADDED</color>";
                                Logs.Hud($"<color=cyan>FusionClient</color> | <color=cyan>{apiuser.displayName}</color> -> <color=yellow>ADDED</color>");
                            }
                            if (PipeLineDupeDebug != $"<color=cyan>PIPELINE |</color> <color=cyan>{apiuser.displayName}</color> has <color=yellow>added you</color>")
                            {
                                PipeLineDupeDebug = $"<color=cyan>PIPELINE |</color> <color=cyan>{apiuser.displayName}</color> has <color=yellow>added you</color>";
                                Logs.Debug($"<color=cyan>PIPELINE |</color> <color=cyan>{apiuser.displayName}</color> has <color=yellow>added you</color>");
                            }
                            if (PipeLineDupeMsg != $"[PIPELINE] {apiuser.displayName} has added you!")
                            {
                                PipeLineDupeDebug = $"[PIPELINE] {apiuser.displayName} has added you!";
                                Logs.Log($"[PIPELINE] {apiuser.displayName} has added you!", ConsoleColor.Cyan);
                            }
                        }
                        break;

                    default:
                        Logs.Log($"[PIPELINE] Unrecognized Type: {WebSocketRawData.type}\n{JsonConvert.SerializeObject(WebSocketData, Formatting.Indented)} Report To VIPER", ConsoleColor.Cyan);
                        break;
                }
            }
            catch { }
        }

        #endregion

        #region AssetLoaded

        private static bool OnObjectInstantiated(ref UnityEngine.Object __0)
        {
            bool result = true;
            try
            {
                if (__0 == null) return true;
                var instantiatedGameObject = __0.TryCast<GameObject>();
                if (!instantiatedGameObject.name.ToLower().Contains("avatar"))
                {
                    result = !ModulesFunctions.AviCrashStart;
                }
                if (!instantiatedGameObject.name.ToLower().Contains("prefab"))
                {
                    result = !ModulesFunctions.AviCrashStart;
                }
                if (!instantiatedGameObject.name.ToLower().Contains("_customavatar"))
                {
                    result = !ModulesFunctions.AviCrashStart;
                }
                if (ModulesFunctions.HideSelfBool)
                {
                    result = false;
                }
                if (ModulesFunctions.AviCrashStart)
                {
                    result = false;
                }
                if (PlayerUtils.GetApiAvatar(PlayerUtils.GetCurrentUser()).id == Config.Main.AviClapID)
                {
                    result = false;
                }

            }
            catch { }
            return result;
        }

        #endregion

        #region Udon Events

        private static bool UdonEvents(string __0)
        {
            switch (__0)
            {
                case "SyncKill":
                    return !UI.GodMode;
            }

            return true;
        }

        #endregion

        #region VidPlayer

        internal static readonly List<string> suitableVideoUrls = new List<string>
        {
            "https://www.youtube.com/",
            "https://youtube.com/",
            "https://www.youtu.be/",
            "https://youtu.be",
            "https://www.twitch.tv/",
            "https://twitch.tv/",
            "https://www.dropbox.com/",
            "https://dropbox.com/",
            "https://cdn.discordapp.com/",
            "https://www.soundcloud.com/",
            "https://soundcloud.com/",
            "https://shintostudios.net/",
            "https://rootworld.xyz/",
            "http://vrcm.nl/",
            "https://pornhub.com",
            "https://vimeo.com/"
        };

        private static bool OnVideoPlayerUrlQueuedPatch(string __0)
        {
            if (Config.Main.VideoPlayerProtection)
            {
                for (int i = 0; i < suitableVideoUrls.Count; i++)
                {
                    if (__0.StartsWith(suitableVideoUrls[i]) == true)
                    {
                        return true;
                    }
                }

                Logs.Hud($"<color=cyan>FusionClient</color> | Video Player URL: {__0} isn't suitable for being queued");

                return false;
            }
            return true;
        }

        private static bool OnVideoPlayerUrlPlayedPatch(VRC_SyncVideoPlayer __instance)
        {
            if (Config.Main.VideoPlayerProtection)
            {
                if (__instance.Videos.Length > 0)
                {
                    string url = __instance.Videos.First().URL;

                    for (int i = 0; i < suitableVideoUrls.Count; i++)
                    {
                        if (url.StartsWith(suitableVideoUrls[i]) == true)
                        {
                            return true;
                        }
                    }

                    Logs.Hud($"<color=cyan>FusionClient</color> | Video Player URL: {url} isn't suitable for being played");

                    return false;
                }
            }

            return true;
        }

        private static bool OnVideoPlayerUrlPlayedIndexPatch(int __0, VRC_SyncVideoPlayer __instance)
        {
            if (Config.Main.VideoPlayerProtection)
            {
                if (__instance.Videos.Length >= __0)
                {
                    string url = __instance.Videos[__0].URL;

                    for (int i = 0; i < suitableVideoUrls.Count; i++)
                    {
                        if (url.StartsWith(suitableVideoUrls[i]) == true)
                        {
                            return true;
                        }
                    }

                    Logs.Hud($"<color=cyan>FusionClient</color> | Video Player URL: {url} isn't suitable for being played");

                    return false;
                }
            }

            return true;
        }

        #endregion

        #region PutRequest

        private static bool SendPutRequest(ref string __0, ref ApiContainer __1, ref Il2CppSystem.Collections.Generic.Dictionary<string, Il2CppSystem.Object> __2, ref CredentialsBundle __3)
        {
            try
            {
                if (__2 != null && (__0 == "visits" || __0 == "joins"))
                {

                    if (Config.Main.OfflineSpoof) return false;
                    if (Config.Main.WorldSpoof)
                    {
                        __2.Clear();
                        __2.Add("userId", APIUser.CurrentUser.id);
                        __2.Add("worldId", Config.Main.WorldID);
                    }
                }
                /* if (Settings.ApiLogs)
                 {
                     EvoConsole.Log($"Target: {__0}");
                     var Dictionary = FC.Utils.MiscUtils.FromIL2CPPToManaged<object>(__2);
                     EvoConsole.Log(JsonConvert.SerializeObject(Dictionary));
                     EvoConsole.Log($"Api container: {__1.Code} / {__1.Text} ");
                 }*/
            }
            catch { }
            return true;
        }

        #endregion

        #region ComfyVR

        private static bool PlaceUiPatch(VRCUiManager __instance, bool __0, bool __1)
        {
            try
            {
                if (Config.Main.ComfyVR)
                {
                    if (!Config.Main.ComfyVR || !PlayerUtils.SelfIsInVR()) return true;
                    float num = PlayerUtils.VRCTrackingManager != null ? PlayerUtils.VRCTrackingManager.transform.localScale.x : 1f;
                    if (num <= 0f)
                    {
                        num = 1f;
                    }
                    var playerTrackingDisplay = __instance.transform;
                    var unscaledUIRoot = __instance.transform.Find("UnscaledUI");
                    playerTrackingDisplay.position = FC.Utils.WorldUtils.GetWorldCameraPosition();
                    Vector3 rotation = GameObject.Find("Camera (eye)").transform.rotation.eulerAngles;
                    Vector3 euler = new Vector3(rotation.x - 30f, rotation.y, 0f);
                    //if (rotation.x > 0f && rotation.x < 300f) rotation.x = 0f;
                    if (FC.Utils.PlayerUtils.GetCurrentUser() == null)
                    {
                        euler.x = euler.z = 0f;
                    }
                    if (!__0)
                    {
                        playerTrackingDisplay.rotation = Quaternion.Euler(euler);
                    }
                    else
                    {
                        Quaternion quaternion = Quaternion.Euler(euler);
                        if (!(Quaternion.Angle(playerTrackingDisplay.rotation, quaternion) < 15f))
                        {
                            if (!(Quaternion.Angle(playerTrackingDisplay.rotation, quaternion) < 25f))
                            {
                                playerTrackingDisplay.rotation = Quaternion.RotateTowards(playerTrackingDisplay.rotation, quaternion, 5f);
                            }
                            else
                            {
                                playerTrackingDisplay.rotation = Quaternion.RotateTowards(playerTrackingDisplay.rotation, quaternion, 1f);
                            }
                        }
                    }
                    if (num >= 0f)
                    {
                        playerTrackingDisplay.localScale = num * Vector3.one;
                    }
                    else
                    {
                        playerTrackingDisplay.localScale = Vector3.one;
                    }
                    if (num > float.Epsilon)
                    {
                        unscaledUIRoot.localScale = 1f / num * Vector3.one;
                    }
                    else
                    {
                        unscaledUIRoot.localScale = Vector3.one;
                    }
                    return false;
                }
            }
            catch
            {
            }
            return true;
        }

        #endregion

        #region Anti IK Crash

        private static bool IsValid(ref IKSolverHeuristic __instance, ref bool __result, ref string message)
        {
            if (__instance.maxIterations > 9999 && Config.Main.AntiIKCrash)
                Logs.Log("Prevented IK crash", ConsoleColor.Cyan);
            Logs.Debug("Prevented IK crash");
            if (__instance.maxIterations > 60)
            {
                __result = false;
                return false;
            }

            return true;
        }

        #endregion

        #region InstanceLock

        private static bool previouslyUsedInvisibleJoin = false;
        private static bool InstanceLockPatches(byte __0, object __1, RaiseEventOptions __2)
        {
            try
            {
                if (__0 != 7)
                {
                    if (__0 == 202)
                    {
                        if (Config.Main.InvisibleJoin)
                        {
                            __2.field_Public_ReceiverGroup_0 = (ReceiverGroup)2;
                            previouslyUsedInvisibleJoin = true;
                            //do later
                            //Buttons2.InvisJoin.ClickMe();
                        }
                        //else if (previouslyUsedInvisibleJoin)
                        //{
                        //    __2.field_Public_ReceiverGroup_0 = 0;
                        //    Buttons2.InvisJoin.ClickMe();
                        //}
                    }
                }
                if (__0 == 4 || __0 == 5)
                    return !Config.Main.InstanceLock;
                if (__1 != null && __2 != null)
                    return __0 != 7 || !ModulesFunctions.Serialization;
            }
            catch { }
            return true;
        }

        #endregion

        #region Inf Portals

        private static bool DestroyPortalPrefix()
        {
            try
            {
                if (ModulesFunctions.WindowsSound)
                {
                    return true;
                }
                else
                {
                    return Config.Main.InfPortals;
                }
            }
            catch
            {
            }
            return true;
        }

        #endregion

        #region PlayerJoin/Leave

        private static void PlayerJoin(ref VRC.Player __0)
        {
            try
            {
                if (Config.Main.NamePlates)
                {
                    Nameplates.Enable(__0);
                    Nameplates.CreateNameplate(__0);
                }

                if (__0.IsFriend())
                {
                    if (Config.Main.HudPlayerJoinFriends)
                    {
                        Logs.Hud($"<color=cyan>FusionClient</color> | <color=#FFFF00>{__0.GetDisplayName()}</color> -> Joined");
                        Logs.Debug($"<color=#FFFF00>{__0.GetDisplayName()}</color> -> <color=green>Joined</color>");
                    }
                }
                else
                {
                    if (Config.Main.HudPlayerJoin)
                    {
                        if (__0.IsFriend())
                        {
                            Logs.Hud($"<color=cyan>FusionClient</color> | <color=#FFFF00>{__0.GetDisplayName()}</color> -> Joined");
                            Logs.Debug($"<color=#FFFF00>{__0.GetDisplayName()}</color> -> <color=green>Joined</color>");
                        }
                        else
                        {
                            Logs.Hud($"<color=cyan>FusionClient</color> | {__0.GetDisplayName()} -> Joined");
                            Logs.Debug($"<color={__0.GetAPIUser().GetRankColor()}>{__0.GetDisplayName()}</color> -> <color=green>Joined</color>");
                        }
                        Logs.Debug($"<color={__0.GetAPIUser().GetRankColor()}>{__0.GetDisplayName()}</color> -> Joined");

                        if (__0.GetAPIUser().hasSuperPowers || __0.GetAPIUser().hasModerationPowers)
                        {
                            if (Config.Main.HudModJoin)
                            {
                                Logs.Hud($"<color=cyan>FusionClient</color> | <color=red>(VRCHAT Staff)</color> {__0.GetDisplayName()} -> Joined");
                                Logs.Debug($"<color=red>(VRCHAT Staff)</color> <color={__0.GetAPIUser().GetRankColor()}>{__0.GetDisplayName()}</color> -> Joined");
                                if (Config.Main.PopUpModJoin)
                                {
                                    PopupUtils.AlertV2($"WARNING: A Vrchat Staff Member With The Name {__0.GetAPIUser().displayName} Has Joined Your Lobby Would Yo Like To Leave?", "Leave", delegate
                                    {
                                        ModulesFunctions.JoinRoom(PlayerUtils.GetCurrentUser().GetHomeWorld());
                                    }, "Stay", PopupUtils.HideCurrentPopUp);
                                }
                            }
                        }
                    }
                }
                if (Config.Main.AntiPhotonBots && __0.IsFlying() && __0.GetPing() == 0 && __0.GetFrames() == 0)
                {
                    Logs.Hud("Destroying Player: " + __0.field_Private_APIUser_0.displayName + " Due To Them Likely Being A Photon Bot.");
                    Logs.Debug("Destroying Player: " + __0.field_Private_APIUser_0.displayName + " Due To Them Likely Being A Photon Bot.");
                    if (__0._vrcplayer.prop_VRCAvatarManager_0 != null && __0._vrcplayer.prop_VRCAvatarManager_0.gameObject != null)
                    {
                        UnityEngine.Object.Destroy(__0._vrcplayer.prop_VRCAvatarManager_0.gameObject);
                    }
                    if (__0.field_Private_VRCPlayerApi_0 != null && __0.field_Private_VRCPlayerApi_0.gameObject != null)
                    {
                        UnityEngine.Object.Destroy(__0.field_Private_VRCPlayerApi_0.gameObject);
                    }
                    if (__0._vrcplayer != null && __0._vrcplayer.gameObject != null)
                    {
                        UnityEngine.Object.Destroy(__0._vrcplayer.gameObject);
                    }
                    if (__0.gameObject != null)
                    {
                        UnityEngine.Object.Destroy(__0.gameObject);
                    }
                }
            }
            catch { }
        }

        private static void PlayerLeft(ref VRC.Player __0)
        {
            try
            {
                var photonuserid = __0.GetUserID();
                if (FusionClient.Startup.Hooks.PhotonHook.PhotonHandlers.Photon_PlayerModerationHandler.BlockedYouPlayers.Contains(photonuserid)) FusionClient.Startup.Hooks.PhotonHook.PhotonHandlers.Photon_PlayerModerationHandler.BlockedYouPlayers.Remove(photonuserid);
                if (FusionClient.Startup.Hooks.PhotonHook.PhotonHandlers.Photon_PlayerModerationHandler.MutedYouPlayers.Contains(photonuserid)) FusionClient.Startup.Hooks.PhotonHook.PhotonHandlers.Photon_PlayerModerationHandler.MutedYouPlayers.Remove(photonuserid);
                Nameplates.Disable(__0);
                Nameplates.DeleteNameplate(__0);

                if (Config.Main.NamePlates)
                {
                    MelonCoroutines.Start(Nameplates.DelayedRefresh());
                }

                if (__0.IsFriend())
                {
                    if (Config.Main.HudPlayerLeaveFriends)
                    {
                        Logs.Hud($"<color=cyan>FusionClient</color> | <color=#FFFF00>{__0.GetDisplayName()}</color> -> Left");
                        Logs.Debug($"<color=#FFFF00>{__0.GetDisplayName()}</color> -> <color=red>Left</color>");
                    }
                }
                else
                {
                    if (Config.Main.HudPlayerLeave)
                    {
                        if (__0.IsFriend())
                        {
                            Logs.Hud($"<color=cyan>FusionClient</color> | <color=#FFFF00>{__0.GetDisplayName()}</color> -> Left");
                            Logs.Debug($"<color=#FFFF00>{__0.GetDisplayName()}</color> -> <color=red>Left</color>");
                        }
                        else
                        {
                            Logs.Hud($"<color=cyan>FusionClient</color> | {__0.GetDisplayName()} -> Left");
                            Logs.Debug($"<color={__0.GetAPIUser().GetRankColor()}>{__0.GetDisplayName()}</color> -> <color=red>Left</color>");
                        }
                    }
                }
            }
            catch { }
        }

        #endregion

        #region OnAvatarChanged

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

        private static readonly Dictionary<string, string> StopDuplicatingAviTextFML = new();

        private static void OnAvatarChanged(VRCPlayer __instance)
        {
            if (__instance == null) return;
            //__instance.Method_Public_add_Void_MulticastDelegateNPublicSealedVoUnique_0(new Action(() =>
            __instance.Method_Public_add_Void_OnAvatarIsReady_0(new Action(() =>
            {
                if (__instance._player != null && __instance._player.field_Private_APIUser_0 != null && __instance.field_Private_ApiAvatar_0 != null)
                {
                    if (ModulesFunctions.AviCrashStart)
                    {
                        __instance.gameObject.active = false;
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

                    var p = __instance._player.field_Private_APIUser_0;
                    var a = __instance.field_Private_ApiAvatar_0;

                    SeenAvatars.AddAvatar(a);

                    if (StopDuplicatingAviTextFML.TryGetValue(p.id, out var avatarID))
                    {
                        if (avatarID == a.id) return;
                        StopDuplicatingAviTextFML[p.id] = a.id;
                        if (__instance._player.IsFriend())
                        {
                            Logs.Debug($"<color=#FFFF00>{__instance._player.field_Private_APIUser_0.GetDisplayName()}</color> -> Avatar Switch");
                        }
                        else
                        {
                            Logs.Debug($"<color={p.GetRankColor()}>{p.displayName}</color> -> Avatar Switch");
                        }
                    }
                    else
                    {
                        StopDuplicatingAviTextFML.Add(p.id, a.id);
                        if (__instance._player.IsFriend())
                        {
                            Logs.Debug($"<color=#FFFF00>{__instance._player.field_Private_APIUser_0.GetDisplayName()}</color> -> Avatar Switch");
                        }
                        else
                        {
                            Logs.Debug($"<color={p.GetRankColor()}>{p.displayName}</color> -> Avatar Switch");
                        }
                        Functions.SendAviToDB(a).Start();
                        if (Config.Main.AvatarLogging)
                        {
                            if (File.Exists(Environment.CurrentDirectory + "\\Fusion Client\\Logs\\AvatarLog.html"))
                            {
                                HTMLAvatarLogger.LogAvatar(a);
                            }
                            Logs.Log($"[AVATAR] {a.name} - By: {a.authorName} Logged", ConsoleColor.Cyan);
                            Logs.Debug($"{a.name} - By: {a.authorName} -> Avatar Logged");
                        }
                    }
                }
            }));
        }

        #endregion

        #region PortalTrigger

        private static bool PortalTriggerCall(PortalTrigger __instance)
        {
            try
            {
                if (Vector3.Distance(PlayerUtils.GetCurrentUser().transform.position, __instance.transform.position) > 1) return false;
                if (Config.Main.AntiPortal) return false;
                if (Config.Main.PortalPrompt)
                {
                    var portalInternal = __instance.field_Private_PortalInternal_0;
                    PopupUtils.AlertV2($"[WARNING]\nYou're Trying To Join: {portalInternal.field_Private_ApiWorld_0.name}\n\n", "Yes", () =>
                    {
                        ModulesFunctions.JoinRoom(portalInternal.field_Private_ApiWorld_0.id + ":" + portalInternal.field_Private_String_4);
                        Logs.Log($"[Portals] Joining: {portalInternal.field_Private_ApiWorld_0.id}:{portalInternal.field_Private_String_4}", ConsoleColor.Yellow);
                        PopupUtils.HideCurrentPopUp();
                    }, "No", PopupUtils.HideCurrentPopUp);
                    return false;
                }
            }
            catch { }
            return true;
        }

        #endregion

        #region OnEvent

        internal static bool VoiceMimic;

        public static int spamFailsafe = 0;
        public static int spamFailsafe2 = 0;
        public static int spamFailsafe3 = 0;
        public static int spamFailsafe4 = 0;
        public static int spamFailsafe5 = 0;
        public static int spamFailsafe6 = 0;
        public static bool blocked = false;
        public static bool blocked2 = false;
        public static bool blocked3 = false;
        public static bool blocked4 = false;
        public static bool blocked5 = false;
        public static bool blocked6 = false;

        private static bool OnEventLog(ref EventData __0)
        {
            try
            {
                if (__0 == null)
                {
                    return false;
                }

                var Parameters = __0.Parameters;
                if (Config.Main.RPCLog)
                {
                    string PlayerName = FC.Utils.PlayerUtils.GetDisplayName(FC.Utils.PlayerUtils.GetCurrentUser());
                    if (__0.Code != 7 && __0.Code != 1 && __0.Code != 8)
                    {
                        var Code = __0.Code;
                        var Sender = "Null";
                        if (__0.Sender != null) Sender = PlayerUtils.GetCurrentUser().GetDisplayName();
                        object value = SerializationUtils.FromIL2CPPToManaged<object>(Parameters);
                        var Serialized = JsonConvert.SerializeObject(value, Formatting.Indented);

                        Logs.Log($"\n-----------[RPC]-----------\n\nCode: {Code}\nSender: {Sender}\nSerialized data:\n{Serialized}", ConsoleColor.Cyan);
                        if (Config.Main.RPCBlock)
                        {
                            return false;
                        }
                    }
                }
                if (Config.Main.PhotonLog)
                {
                    foreach (var data in __0.Parameters)
                    {
                        object Data = SerializationUtils.FromIL2CPPToManaged<object>(data.Value);
                        Logs.Log($"\n-----------[Photon]-----------\n\n[Event {__0.Code}] [Key:{__0.CustomDataKey}] [Sender: {__0.Sender}] Event \n{Newtonsoft.Json.JsonConvert.SerializeObject(Data, Newtonsoft.Json.Formatting.Indented)}");
                        //if (!File.Exists($"{Environment.CurrentDirectory}\\Fusion Client\\Logs\\PhotonLogs.txt"))
                        //{
                        //    File.Create($"{Environment.CurrentDirectory}\\Fusion Client\\Logs\\PhotonLogs.txt");
                        //}
                        //else
                        //{
                        //    File.AppendAllText($"{Environment.CurrentDirectory}\\Fusion Client\\Logs\\PhotonLogs.txt", $"\n-----------[Photon]-----------\n\n[Event {__0.Code}] [Sender: {__0.Sender}] Event \n{Newtonsoft.Json.JsonConvert.SerializeObject(Data, Newtonsoft.Json.Formatting.Indented)}" + Environment.NewLine);
                        //}
                    }
                }
                try
                {
                    if (Config.Main.AntiBrokenUspeak)
                    {
                        if (__0.Code == 1) //VoiceData
                        {
                            byte[] earrapeExploitSequence = new byte[31]
                            {
        0, 248, 125, 232, 192, 92, 160, 82, 254, 48,
        228, 30, 187, 149, 196, 177, 215, 140, 223, 127,
        209, 66, 60, 0, 226, 53, 180, 176, 97, 104, 4
                            };
                            byte[] earrapeExploitSequence2 = Convert.FromBase64String("Ho8KDOT58voAu4Y7APgAfejAXKBS/jDkHrv/xLHXjN9/0UILAOI1tLBhaAT47sOGLLm2RF5yzbWWOP9+95t7rGxiUBZxWaCG3dnd3Y6Cgv+pp2JCC+biqgIptIX//zMh+ebp4iRKqiIx");
                            byte[] earrapeExploitSequence3 = Convert.FromBase64String("AAAAAAAAAAC7hjsA+H3owFygUv4w5B67lcSx14zff9FCPADiNbSwYWgE+O7Dhiy5tkRecs21ljjofvebe6xsYlA4cVmght0=");
                            byte[] earrapeExploitSequence4 = Convert.FromBase64String("hwIAACmxfaE7CEgAeL1iJ2Bkj3KRCONbQGclHWF0qJ8JbM6WARDYeJ25oH2Gb+gA7mZ4aEbkznWVce9LjcNa5uMef1QpNRjwkbVYM3VVE3mFHbpLPAhDAHi//50nTm569Waiu/yxAamDw0GX8pimEBAILbZJthSkvj3qaSXeJcSOQKdcJe57KBn1tBx8CUJz1s09JOJzP+nHsmM9CDsAeL1w7SDWm09Uc/WynowuxSIh5GhyIxo4rvjOpPT9KDQIlGz2DBcPQVJByj+Hhh120NFh4Mn7OgcKsUo=");
                            byte[] earrapeExploitSequence5 = Convert.FromBase64String("AAAAAAAAAAC7hjsA+H3owFygUv4w5B67lcSx14zff9FCPADiNbSwYWgE+O7Dhiy5tkRecs21ljjofvebe6xsYlA4cVmght0=");
                            byte[] nulldata = new byte[4];
                            byte[] ServerTime = BitConverter.GetBytes(Networking.GetServerTimeInMilliseconds());
                            Buffer.BlockCopy(nulldata, 0, earrapeExploitSequence5, 0, 4);
                            Buffer.BlockCopy(ServerTime, 0, earrapeExploitSequence5, 4, 4);
                            byte[] voiceData = SerializationUtils.FromIL2CPPToManaged<byte[]>(__0.CustomData);
                            byte[] voiceDataFirstSequence = voiceData.Skip(11).Take(voiceData.Length - 40).ToArray();
                            if (voiceDataFirstSequence.SequenceEqual(earrapeExploitSequence) == true)
                            {
                                if (Logs.lastHud != $"<color=cyan>FusionClient</color> | Blocking Broken Uspeak From {PhotonUtils.TryGetPlayer(__0.sender).GetDisplayName()}")
                                {
                                    if (!Userids.Contains($"{PhotonUtils.TryGetPlayer(__0.sender).GetUserID()}"))
                                    {
                                        Userids.Add(PhotonUtils.TryGetPlayer(__0.sender).GetUserID());
                                        Logs.Hud($"<color=cyan>FusionClient</color> | Blocking Broken Uspeak From {PhotonUtils.TryGetPlayer(__0.sender).GetDisplayName()}");
                                    }
                                }
                                Logs.Debug($"Blocking Broken Uspeak");
                                return false; //We detected the earrape sequence
                            }
                            if (voiceDataFirstSequence.SequenceEqual(earrapeExploitSequence2) == true)
                            {
                                if (Logs.lastHud != $"<color=cyan>FusionClient</color> | Blocking Broken Uspeak From {PhotonUtils.TryGetPlayer(__0.sender).GetDisplayName()}")
                                {
                                    if (!Userids.Contains($"{PhotonUtils.TryGetPlayer(__0.sender).GetUserID()}"))
                                    {
                                        Userids.Add(PhotonUtils.TryGetPlayer(__0.sender).GetUserID());
                                        Logs.Hud($"<color=cyan>FusionClient</color> | Blocking Broken Uspeak From {PhotonUtils.TryGetPlayer(__0.sender).GetDisplayName()}");
                                    }
                                }
                                Logs.Debug($"Blocking Broken Uspeak");
                                return false; //We detected the earrape sequence
                            }
                            if (voiceDataFirstSequence.SequenceEqual(earrapeExploitSequence3) == true)
                            {
                                if (Logs.lastHud != $"<color=cyan>FusionClient</color> | Blocking Broken Uspeak From {PhotonUtils.TryGetPlayer(__0.sender).GetDisplayName()}")
                                {
                                    if (!Userids.Contains($"{PhotonUtils.TryGetPlayer(__0.sender).GetUserID()}"))
                                    {
                                        Userids.Add(PhotonUtils.TryGetPlayer(__0.sender).GetUserID());
                                        Logs.Hud($"<color=cyan>FusionClient</color> | Blocking Broken Uspeak From {PhotonUtils.TryGetPlayer(__0.sender).GetDisplayName()}");
                                    }
                                }
                                Logs.Debug($"Blocking Broken Uspeak");
                                return false; //We detected the earrape sequence
                            }
                            if (voiceDataFirstSequence.SequenceEqual(earrapeExploitSequence4) == true)
                            {
                                if (Logs.lastHud != $"<color=cyan>FusionClient</color> | Blocking Broken Uspeak From {PhotonUtils.TryGetPlayer(__0.sender).GetDisplayName()}")
                                {
                                    if (!Userids.Contains($"{PhotonUtils.TryGetPlayer(__0.sender).GetUserID()}"))
                                    {
                                        Userids.Add(PhotonUtils.TryGetPlayer(__0.sender).GetUserID());
                                        Logs.Hud($"<color=cyan>FusionClient</color> | Blocking Broken Uspeak From {PhotonUtils.TryGetPlayer(__0.sender).GetDisplayName()}");
                                    }
                                }
                                Logs.Debug($"Blocking Broken Uspeak");
                                return false; //We detected the earrape sequence
                            }
                            if (voiceDataFirstSequence.SequenceEqual(earrapeExploitSequence5) == true)
                            {
                                if (Logs.lastHud != $"<color=cyan>FusionClient</color> | Blocking Broken Uspeak From {PhotonUtils.TryGetPlayer(__0.sender).GetDisplayName()}")
                                {
                                    if (!Userids.Contains($"{PhotonUtils.TryGetPlayer(__0.sender).GetUserID()}"))
                                    {
                                        Userids.Add(PhotonUtils.TryGetPlayer(__0.sender).GetUserID());
                                        Logs.Hud($"<color=cyan>FusionClient</color> | Blocking Broken Uspeak From {PhotonUtils.TryGetPlayer(__0.sender).GetDisplayName()}");
                                    }
                                }
                                Logs.Debug($"Blocking Broken Uspeak");
                                return false; //We detected the earrape sequence
                            }
                            if (voiceData.Length <= 8)
                            {
                                if (Logs.lastHud != $"<color=cyan>FusionClient</color> | Blocking Potential Broken Uspeak From {PhotonUtils.TryGetPlayer(__0.sender).GetDisplayName()}")
                                {
                                    if (!Userids.Contains($"{PhotonUtils.TryGetPlayer(__0.sender).GetUserID()}"))
                                    {
                                        Userids.Add(PhotonUtils.TryGetPlayer(__0.sender).GetUserID());
                                        Logs.Hud($"<color=cyan>FusionClient</color> | Blocking Potential Broken Uspeak From {PhotonUtils.TryGetPlayer(__0.sender).GetDisplayName()}");
                                    }
                                }
                                Logs.Debug($"Blocking Potential Broken Uspeak");
                                return false; //Potential exploit
                            }
                            if (BitConverter.ToInt32(voiceData, 0) == 0)
                            {
                                if (Logs.lastHud != $"<color=cyan>FusionClient</color> | Blocking Potential Broken Uspeak From {PhotonUtils.TryGetPlayer(__0.sender).GetDisplayName()}")
                                {
                                    if (!Userids.Contains($"{PhotonUtils.TryGetPlayer(__0.sender).GetUserID()}"))
                                    {
                                        Userids.Add(PhotonUtils.TryGetPlayer(__0.sender).GetUserID());
                                        Logs.Hud($"<color=cyan>FusionClient</color> | Blocking Potential Broken Uspeak From {PhotonUtils.TryGetPlayer(__0.sender).GetDisplayName()}");
                                    }
                                }
                                Logs.Debug($"Blocking Potential Broken Uspeak");
                                return false; //Another potential exploit
                            }
                        }
                    }
                }
                catch { }
                if (Userids.Count > 1)
                {
                    Delay(10f, delegate
                    {
                        Userids.Clear();
                    });
                }
                if (ModulesFunctions.AviCrashStart)
                {
                    if (__0.Code == 42)
                    {
                        return false;
                    }
                }
                if (Config.Main.EventLimit)
                {
                    if (__0.Code == 9)
                    {
                        if (blocked)
                        {
                            spamFailsafe++;
                            return false;
                        }
                        if (spamFailsafe == 920)
                        {
                            if (Config.Main.EventLimit)
                            {
                                Logs.Hud("<color=cyan>FusionClient</color> | Blocking Event 9 for 15 seconds");
                                Logs.Log("Blocking Event 9 for 15 seconds after " + spamFailsafe.ToString() + " Requests" + " in 2 second", ConsoleColor.Cyan);
                                Logs.Debug($"Blocking Event 9 for 15 seconds after " + spamFailsafe.ToString() + " Requests" + " in 2 second");
                            }
                            blocked = true;
                            Delay(15f, delegate
                            {
                                if (Config.Main.EventLimit)
                                {
                                    Logs.Hud("<color=cyan>FusionClient</color> | Unblocked Event 9 [" + spamFailsafe.ToString() + " blocked Events]");
                                    Logs.Log("Unblocked Event 9 [" + spamFailsafe.ToString() + " blocked Events]", ConsoleColor.Cyan);
                                    Logs.Debug("Unblocked Event 9 [" + spamFailsafe.ToString() + " blocked Events]");
                                }
                                spamFailsafe = 0;
                                blocked = false;
                            });
                            return false;
                        }
                        spamFailsafe++;
                        Delay(2f, delegate
                        {
                            spamFailsafe = 0;
                        });
                    }
                    else
                    {
                        if (__0.Code == 6)
                        {
                            if (blocked2)
                            {
                                spamFailsafe2++;
                                return false;
                            }
                            if (spamFailsafe2 == 500)
                            {
                                if (Config.Main.EventLimit)
                                {
                                    Logs.Hud("<color=cyan>FusionClient</color> | Blocking Event 6 for 25 seconds after " + spamFailsafe2.ToString() + " in 2 second");
                                    Logs.Log("Blocking Event 6 for 25 seconds after " + spamFailsafe2.ToString() + " Requests" + " in 2 second", ConsoleColor.Cyan);
                                    Logs.Debug("Blocking Event 6 for 25 seconds after " + spamFailsafe2.ToString() + " Requests" + " in 2 second");
                                }
                                blocked2 = true;
                                Delay(25f, delegate
                                {
                                    if (Config.Main.EventLimit)
                                    {
                                        Logs.Hud("<color=cyan>FusionClient</color> | Unblocked Event 6 [" + spamFailsafe2.ToString() + " blocked Events]");
                                        Logs.Log("Unblocked Event 6 [" + spamFailsafe2.ToString() + " blocked Events]", ConsoleColor.Cyan);
                                        Logs.Debug("Unblocked Event 6 [" + spamFailsafe2.ToString() + " blocked Events]");
                                    }
                                    spamFailsafe2 = 0;
                                    blocked2 = false;
                                });
                                return false;
                            }
                            spamFailsafe2++;
                            Delay(2f, delegate
                            {
                                spamFailsafe2 = 0;
                            });
                        }
                        else
                        {
                            if (__0.Code == 8)
                            {
                                if (blocked3)
                                {
                                    spamFailsafe3++;
                                    return false;
                                }
                                if (spamFailsafe3 == 920)
                                {
                                    if (Config.Main.EventLimit)
                                    {
                                        Logs.Hud("<color=cyan>FusionClient</color> | Blocking Event 8 for 10 seconds after " + spamFailsafe3.ToString() + " in 1 second");
                                        Logs.Log("Blocking Event 8 for 10 seconds after " + spamFailsafe3.ToString() + " Requests" + " in 1 second", ConsoleColor.Cyan);
                                        Logs.Debug("Blocking Event 8 for 10 seconds after " + spamFailsafe3.ToString() + " Requests" + " in 1 second");
                                    }
                                    blocked3 = true;
                                    Delay(10f, delegate
                                    {
                                        if (Config.Main.EventLimit)
                                        {
                                            Logs.Hud("<color=cyan>FusionClient</color> | Unblocked Event 8 [" + spamFailsafe3.ToString() + " blocked Events]");
                                            Logs.Log("Unblocked Event 8 [" + spamFailsafe3.ToString() + " blocked Events]", ConsoleColor.Cyan);
                                            Logs.Debug("Unblocked Event 8 [" + spamFailsafe3.ToString() + " blocked Events]");
                                        }
                                        spamFailsafe3 = 0;
                                        blocked3 = false;
                                    });
                                    return false;
                                }
                                spamFailsafe3++;
                                Delay(1f, delegate
                                {
                                    spamFailsafe3 = 0;
                                });
                            }
                            else
                            {
                                if (__0.Code == 7)
                                {
                                    if (blocked4)
                                    {
                                        spamFailsafe4++;
                                        return false;
                                    }
                                    if (spamFailsafe4 == 550)
                                    {
                                        if (Config.Main.EventLimit)
                                        {
                                            Logs.Hud("<color=cyan>FusionClient</color> | Blocking Event 7 for 10 seconds after " + spamFailsafe4.ToString() + " in 1 second");
                                            Logs.Log("Blocking Event 7 for 10 seconds after " + spamFailsafe4.ToString() + " Requests" + " in 1 second", ConsoleColor.Cyan);
                                            Logs.Debug("Blocking Event 7 for 10 seconds after " + spamFailsafe4.ToString() + " Requests" + " in 1 second");
                                        }
                                        blocked4 = true;
                                        Delay(10f, delegate
                                        {
                                            if (Config.Main.EventLimit)
                                            {
                                                Logs.Hud("<color=cyan>FusionClient</color> | Unblocked Event 7 [" + spamFailsafe4.ToString() + " blocked Events]");
                                                Logs.Log("Unblocked Event 7 [" + spamFailsafe4.ToString() + " blocked Events]", ConsoleColor.Cyan);
                                                Logs.Debug("Unblocked Event 7 [" + spamFailsafe4.ToString() + " blocked Events]");
                                            }
                                            spamFailsafe4 = 0;
                                            blocked4 = false;
                                        });
                                        return false;
                                    }
                                    spamFailsafe4++;
                                    Delay(1f, delegate
                                    {
                                        spamFailsafe4 = 0;
                                    });
                                }
                                else
                                {
                                    if (__0.Code == 209)
                                    {
                                        if (blocked5)
                                        {
                                            spamFailsafe5++;
                                            return false;
                                        }
                                        if (spamFailsafe5 == 420)
                                        {
                                            if (Config.Main.EventLimit)
                                            {
                                                Logs.Hud("<color=cyan>FusionClient</color> | Blocking Event 209 for 10 seconds after " + spamFailsafe5.ToString() + " in 1 second");
                                                Logs.Log("Blocking Event 219 for 10 seconds after " + spamFailsafe5.ToString() + " Requests" + " in 1 second", ConsoleColor.Cyan);
                                                Logs.Debug("Blocking Event 209 for 10 seconds after " + spamFailsafe5.ToString() + " Requests" + " in 1 second");
                                            }
                                            blocked5 = true;
                                            Delay(10f, delegate
                                            {
                                                if (Config.Main.EventLimit)
                                                {
                                                    Logs.Hud("<color=cyan>FusionClient</color> | Unblocked Event 209 [" + spamFailsafe5.ToString() + " blocked Events]");
                                                    Logs.Log("Unblocked Event 209 [" + spamFailsafe5.ToString() + " blocked Events]", ConsoleColor.Cyan);
                                                    Logs.Debug("Unblocked Event 209 [" + spamFailsafe5.ToString() + " blocked Events]");
                                                }
                                                spamFailsafe5 = 0;
                                                blocked5 = false;
                                            });
                                            return false;
                                        }
                                        spamFailsafe5++;
                                        Delay(1f, delegate
                                        {
                                            spamFailsafe5 = 0;
                                        });
                                    }
                                    else
                                    {
                                        if (__0.Code == 210)
                                        {
                                            if (blocked6)
                                            {
                                                spamFailsafe6++;
                                                return false;
                                            }
                                            if (spamFailsafe6 == 420)
                                            {
                                                if (Config.Main.EventLimit)
                                                {
                                                    Logs.Hud("<color=cyan>FusionClient</color> | Blocking Event 210 for 10 seconds after " + spamFailsafe6.ToString() + " in 1 second");
                                                    Logs.Log("Blocking Event 210 for 10 seconds after " + spamFailsafe6.ToString() + " Requests" + " in 1 second", ConsoleColor.Cyan);
                                                    Logs.Debug("Blocking Event 210 for 10 seconds after " + spamFailsafe6.ToString() + " Requests" + " in 1 second");
                                                }
                                                blocked6 = true;
                                                Delay(10f, delegate
                                                {
                                                    if (Config.Main.EventLimit)
                                                    {
                                                        Logs.Hud("<color=cyan>FusionClient</color> | Unblocked Event 210 [" + spamFailsafe6.ToString() + " blocked Events]");
                                                        Logs.Log("Unblocked Event 210 [" + spamFailsafe6.ToString() + " blocked Events]", ConsoleColor.Cyan);
                                                        Logs.Debug("Unblocked Event 210 [" + spamFailsafe6.ToString() + " blocked Events]");
                                                    }
                                                    spamFailsafe6 = 0;
                                                    blocked6 = false;
                                                });
                                                return false;
                                            }
                                            spamFailsafe6++;
                                            Delay(1f, delegate
                                            {
                                                spamFailsafe6 = 0;
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                switch (__0.Code)
                {
                    case 1:
                        if (VoiceMimic)
                        {
                            if (PlayerManager.prop_PlayerManager_0.GetPlayerID(__0.Sender).GetUserID() == FusionClient.Core.UI.Target.GetUserID())
                            {
                                MiscUtils.OpRaiseEvent(1, __0.CustomData, new Photon.Realtime.RaiseEventOptions()
                                {
                                    field_Public_ReceiverGroup_0 = Photon.Realtime.ReceiverGroup.Others,
                                    field_Public_Byte_0 = 1,
                                    field_Public_Byte_1 = 1,
                                }, SendOptions.SendUnreliable);
                            }
                        }
                        break;
                }


                return true;
            }
            catch { }
            return true;
        }

        #endregion

        #region RPC Events

        internal static bool ItemPOS(Vector3 vector3_0)
        {
            return float.IsNaN(vector3_0.x) || float.IsNaN(vector3_0.y) || float.IsNaN(vector3_0.z);
        }

        private static bool RPCEvent(ref VRC.Player __0, ref VRC.SDKBase.VRC_EventHandler.VrcEvent __1, ref VRC.SDKBase.VRC_EventHandler.VrcBroadcastType __2, ref int __3, ref float __4)
        {
            if (__0 == null)
            {
                return false;
            }
            try
            {
                var Player = __0;
                string Sender = Player.GetDisplayName();


                if (Config.Main.RPCLog)
                {
                    if (__1.ParameterObject != null) Logs.Log($"\n-----------[RPC]-----------\n\nSender: {Sender}\nType: {__1.EventType}\nBroadcast: {__2}\nString: {__1.ParameterString}\nGameObject Name: {__1.ParameterObject.name}\nGameObject position: {__1.ParameterObject.transform.position.x}, {__1.ParameterObject.transform.position.y}, {__1.ParameterObject.transform.position.y}\nFloat: {__1.ParameterFloat}\nInt: {__1.ParameterInt}\nBool: {__1.ParameterBoolOp}", ConsoleColor.Cyan);
                    else
                    {
                        var AllNames = new List<string>();
                        foreach (var Object in __1.ParameterObjects)
                        {
                            AllNames.Add(Object.name);
                        }
                        Logs.Log($"\n-----------[RPC]-----------\n\nSender: {Sender}\nType: {__1.EventType}\nBroadcast: {__2}\nString: {__1.ParameterString}\nGameObjects: {string.Join(", ", AllNames)}\nFloat: {__1.ParameterFloat}\nInt: {__1.ParameterInt}\nBool: {__1.ParameterBoolOp}", ConsoleColor.Cyan);
                    }
                }
                if (Config.Main.RPCBlock)
                {
                    if (__0 != PlayerUtils.GetCurrentUser())
                    {
                        if (__1.ParameterString.Length > 40)
                        {
                            return false;
                        }
                    }
                }

                if (Config.Main.AntiDesync)
                {
                    if (__0 != PlayerUtils.GetCurrentUser())
                    {
                        if (__1.ParameterBytes.Length > 1000 || __1.ParameterString.Length > 60) // Anti Desync
                        {
                            if (!string.IsNullOrEmpty(__1.ParameterString))
                            {
                                __1.ParameterBytes = new byte[0];
                                __1.ParameterString = "";
                                __1.ParameterObject = null;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }

                if (Config.Main.AntiMasterDc && (__1.ParameterString.Length > 60 || __1.ParameterString.Contains("<color=") == true || __1.ParameterString.Contains("love") == true || __1.ParameterString.Contains("Stellar") == true || __1.ParameterString.Contains("NoSightz") == true))
                {
                    if (__0 != PlayerUtils.GetCurrentUser())
                    {
                        if (__0 != null)
                        {
                            Logs.Log($"Blocked MasterDC from {Sender} - {Player.prop_APIUser_0.id}", ConsoleColor.Cyan);
                        }
                    }
                    return false;
                }

                if (Config.Main.AntiRPCCrash)
                {
                    if (__0 != PlayerUtils.GetCurrentUser())
                    {
                        if (__4 > 100000f)
                        {
                            if (__0 != null)
                            {
                                if (!Userids.Contains($"{__0.prop_APIUser_0.id}"))
                                {
                                    Logs.Hud($"{__0.prop_APIUser_0.displayName} is trying to use a lag exploit");
                                    MelonLogger.Msg($"{__0.prop_APIUser_0.displayName} is trying to use a lag exploit");
                                    Logs.Debug($"{__0.prop_APIUser_0.displayName} is trying to use a lag exploit");
                                    Userids.Add(__0.prop_APIUser_0.id);
                                }
                            }
                            return false;
                        }
                    }
                }

                if (Config.Main.AntiWorldTriggers)
                {
                    if (__0 != PlayerUtils.GetCurrentUser())
                    {
                        try
                        {
                            if (__2 == VrcBroadcastType.Always || __2 == VrcBroadcastType.AlwaysUnbuffered || __2 == VrcBroadcastType.AlwaysBufferOne)
                            {
                                if (__1.ParameterString == "ConfigurePortal" || __1.ParameterString == "_SendOnSpawn" || __1.ParameterString == "_InstantiateObject")
                                {
                                    return true;
                                }
                                else
                                {
                                    if (__2 != VrcBroadcastType.Always || __2 != VrcBroadcastType.AlwaysBufferOne || __2 != VrcBroadcastType.AlwaysUnbuffered)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }

                if (Config.Main.AntiCamCrash && __1.ParameterObject != null && (!FC.Utils.PlayerUtils.LocalPlayer && ItemPOS(__1.ParameterObject.transform.position)))
                {
                    if (__0 != PlayerUtils.GetCurrentUser())
                    {
                        Logs.Log($"{__0.field_Private_APIUser_0.displayName} attempted to use VRCEvent on an object in infinity! | Event Name:  {__1.Name} | OBJ: {__1.ParameterObject.name}", ConsoleColor.Cyan);
                        __1.ParameterObject.SetActive(false);
                        return false;
                    }
                }

                if (Config.Main.AntiUdon && (__1.ParameterBytes != null && __1.ParameterBytes.Length > 0) && (Convert.ToString(Networking.DecodeParameters(__1.ParameterBytes)[0]).Contains("_interact") || __1.ParameterString == "UdonSyncRunProgramAsRPC") && !FC.Utils.PlayerUtils.LocalPlayer)
                {
                    if (__0 != PlayerUtils.GetCurrentUser())
                    {
                        Logs.Log("Prevented " + __0.field_Private_APIUser_0.displayName + " from using Udon Exploits!", ConsoleColor.Cyan);
                        return false;
                    }
                }

            }
            catch { }
            return true;
        }

        #endregion

        #region InternalTriggerEvent

        private static bool InternalTriggerEvent(VrcEvent __0, ref VrcBroadcastType __1)
        {
            if (Config.Main.WorldTriggers == true && (__1 != VrcBroadcastType.Always || __1 != VrcBroadcastType.AlwaysBufferOne || __1 != VrcBroadcastType.AlwaysUnbuffered))
            {
                __1 = VrcBroadcastType.Always;
            }
            return true;
        }

        #endregion

        #region FakeFPS
        private static bool FakeFPS(ref float __result)
        {
            bool flag = !Config.Main.FakeFramesToggle;
            bool result;
            if (flag)
            {
                result = true;
            }
            else
            {
                float num = (Config.Main.FakeFramesValue != -1) ? (1f / (float)Config.Main.FakeFramesValue) : 0.01f;
                bool spooferRealisticMode = Config.Main.FakeFramesReal;
                if (spooferRealisticMode)
                {
                    float num2 = Mathf.Sin(Time.realtimeSinceStartup / 10f);
                    bool flag2 = num2 < 0f;
                    if (flag2)
                    {
                        num2 = -num2;
                    }
                    float num3 = Mathf.Cos(Time.realtimeSinceStartup / 10f);
                    bool flag3 = num3 < 0f;
                    if (flag3)
                    {
                        num3 = -num3;
                    }
                    float num4 = num2 * num3;
                    num += num / 2f * num4;
                }
                __result = num;
                result = false;
            }
            return result;
        }

        #endregion

        #region FakePing

        private static bool FakePing(ref int __result)
        {
            bool flag = !Config.Main.FakePing;
            bool result;
            if (flag)
            {
                result = true;
            }
            else
            {
                int num = (Config.Main.FakePingValue != -1) ? Config.Main.FakePingValue : 1337;
                bool spooferRealisticMode = Config.Main.FakePingReal;
                if (spooferRealisticMode)
                {
                    float num2 = Mathf.Sin(Time.realtimeSinceStartup / 10f);
                    bool flag2 = num2 < 0f;
                    if (flag2)
                    {
                        num2 = -num2;
                    }
                    float num3 = Mathf.Cos(Time.realtimeSinceStartup / 10f);
                    bool flag3 = num3 < 0f;
                    if (flag3)
                    {
                        num3 = -num3;
                    }
                    float num4 = num2 * num3;
                    num = (int)((float)num + (float)(num / 2) * num4);
                }
                __result = num;
                result = false;
            }
            return result;
        }

        #endregion

        #endregion

        #region Utils For Log And Other Patches

        internal static MethodInfo PlaceUiMethod
        {
            get
            {
                if (_placeUi == null)
                {
                    try
                    {
                        var xrefs = XrefScanner.XrefScan(typeof(VRCUiManager).GetMethod(nameof(VRCUiManager.LateUpdate)));

                        foreach (var x in xrefs)
                        {
                            if (x.Type == XrefType.Method && x.TryResolve() != null &&
                                x.TryResolve().GetParameters().Length == 2 &&
                                x.TryResolve().GetParameters().All(a => a.ParameterType == typeof(bool)))
                            {
                                _placeUi = (MethodInfo)x.TryResolve();
                                break;
                            }
                        };
                    }
                    catch
                    {
                    }
                }
                return _placeUi;
            }
        }

        private static MethodInfo _placeUi;

        // Delay Functions
        internal static void Delay(float del, Action action)
        {
            MelonCoroutines.Start(DelayFunc(del, action));
        }
        private static IEnumerator DelayFunc(float del, Action action)
        {
            yield return new WaitForSeconds(del);
            action.Invoke();
            yield break;
        }

        public class ModerationEntry
        {
            public string UserID { get; set; }
            public string Displayname { get; set; }
            public bool BlockedU { get; set; }
            public bool MutedU { get; set; }
            public bool UBlocked { get; set; }
            public bool UMuted { get; set; }
            public int WarnU { get; set; }
            public int MicOffU { get; set; }
            public int KickedU { get; set; }
        }

        public enum ModerationType
        {
            BlockU,
            MuteU,
            UBlocked,
            UMute,
            WarnU,
            MicOffU,
            KickU,
            None
        }

        public static List<ModerationEntry> Moderations = new List<ModerationEntry>();

        public static ModerationEntry GetModeration(string SourceID, string Display = null, bool GenNew = true)
        {
            var Moderation = Moderations.Find(moderation => moderation.UserID == SourceID);
            if (Moderation == null && GenNew)
            {
                Moderations.Add(new ModerationEntry() { UserID = SourceID, BlockedU = false, Displayname = Display, MutedU = false, UBlocked = false, UMuted = false, WarnU = 0, MicOffU = 0, KickedU = 0 });
                return GetModeration(SourceID, Display, GenNew);
            }
            return Moderation;
        }

        public static ModerationType GetModerationType(string userid)
        {
            var Moderation = GetModeration(userid, null, false);
            if (Moderation != null)
            {
                if (Moderation.BlockedU)
                    return ModerationType.BlockU;
                if (Moderation.MutedU)
                    return ModerationType.MuteU;
            }
            return ModerationType.None;
        }

        public static void UpdateModeration(string userid, ModerationType type, bool Change, string Displayname = null)
        {
            var Moderation = GetModeration(userid, Displayname);
            if (Displayname != null)
                Moderation.Displayname = Displayname;
            switch (type)
            {
                case ModerationType.BlockU:
                    Moderation.BlockedU = Change;
                    break;

                case ModerationType.MuteU:
                    Moderation.MutedU = Change;
                    break;

                case ModerationType.UBlocked:
                    Moderation.UBlocked = Change;
                    break;

                case ModerationType.UMute:
                    Moderation.UMuted = Change;
                    break;

                case ModerationType.WarnU:
                    Moderation.WarnU += 1;
                    break;

                case ModerationType.MicOffU:
                    Moderation.MicOffU += 1;
                    break;

                case ModerationType.KickU:
                    Moderation.KickedU += 1;
                    break;
            }
        }

        public class ModerationLogEntry
        {
            public string UserID { get; set; }
            public string Displayname { get; set; }
            public string type { get; set; }
            public object Change { get; set; }
            public DateTime Time { get; set; }
        }

        public static T FromIL2CPPToManaged<T>(Il2CppSystem.Object obj)
        {
            //if (obj.GetIl2CppType().Attributes == Il2CppSystem.Reflection.TypeAttributes.Serializable)
            //    return obj.Cast<T>();
            return FromByteArray<T>(ToByteArray(obj));
        }

        public static T FromByteArray<T>(byte[] data)
        {
            if (data == null) return default(T);
            var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (var ms = new System.IO.MemoryStream(data))
            {
                object obj = bf.Deserialize(ms);
                return (T)obj;
            }
        }

        public static byte[] ToByteArray(Il2CppSystem.Object obj)
        {
            if (obj == null) return null;
            var bf = new Il2CppSystem.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var ms = new Il2CppSystem.IO.MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        #endregion
    }
}
