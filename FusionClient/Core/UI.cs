using FusionClient.API.QM;
using FusionClient.API.Wings;
using FusionClient.Modules;
using FusionClient.Utils.Objects.Mod;
using FusionClient.Utils.VRChat;
using MelonLoader;
using System.Text;
using UnityEngine;
using FC.Utils;
using FC.Utils.API.QM;
using Fusion.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FusionClient;
using System.Diagnostics;
using System.Windows;
using RealisticEyeMovements;
using RootMotion.FinalIK;
using VRC.Core;
using VRC.SDKBase;
using System.IO;
using UnityEngine.UI;
using System.Collections;
using VRC.UI;
using VRC;
using TMPro;
using VRC.Udon;
using Fusion.Networking.Serializable;
using System.Threading;
using System.Globalization;
using Button = UnityEngine.UI.Button;
using System.Windows.Forms;
using FusionClient.Utils;
using FusionClient.API;
using FusionClient.Other;
using FusionClient.Utils.Manager;
using static Blaze.API.AW.ActionWheelAPI;
using VRC.Udon.Common.Interfaces;
using FusionClient.API.SM;
using Transmtn.DTO.Notifications;
using Transmtn.DTO;
using WTFBlaze.API.QM;
using Application = System.Windows.Forms.Application;

namespace FusionClient.Core
{
    internal static class UI
    {
        #region InitializeQuickMenu

        #region Udon Manipulator

        public static QMNestedButton UdonMenu;
        public static QMNestedButton ActionsMenu;
        public static QMNestedButton UdonSettings;
        public static QMScrollMenu Scroll;
        public static QMScrollMenu Scroll2;
        public static bool NetworkEvents;
        public static bool TargetedEvents;
        public static UdonBehaviour selectedScript;
        public static QMNestedButton UdonManipulator;

        #endregion

        #region Room History

        public static QMNestedButton roomHistory;
        public static QMNestedButton roomHistorySelected;
        public static QMScrollMenu roomHistoryScroll;
        public static QMInfo roomHistorySelectedPanel;
        private static RoomHistoryObject selectedRoom;

        #endregion

        #region Main Shit

        #region Bot Stuff

        public static BotServer BServer;

        #endregion

        #region Random Shit

        internal static bool GodMode = false;

        #endregion

        public static QMTabMenu Menu;
        public static QMNestedButton Self;
        public static QMNestedButton Movement;
        public static QMNestedButton World;
        public static QMNestedButton Protection;
        public static QMNestedButton Exploits;
        public static QMNestedButton Visual;
        public static QMNestedButton IKTweaks;
        public static QMNestedButton Settings;
        public static QMNestedButton Bots;
        internal static InfoPanel PlayerList;
        internal static InfoPanel DebugPanel;
        private static GameObject UserInfo;
        private static GameObject UserInfoGetId;
        internal static Transform SelectedUserScroll;
        internal static Transform CloneButton;
        internal static VRCPlayer Target;
        internal static int BotTarget = 1;
        internal static VRC.Player Target2;

        #endregion

        #region Movement

        public static QMToggleButton FlyToggle;

        #endregion

        #region Exploits

        public static QMToggleButton InvisJoin;
        public static QMToggleButton Serialization;
        public static QMToggleButton Swastika;
        public static QMToggleButton BrokenUspeak;
        public static QMToggleButton ItemLock;
        public static QMToggleButton FreeFall;
        public static QMToggleButton MirrorSpam;
        public static QMToggleButton ItemOrbitButton;

        #endregion

        #region QM

        internal static QMSingleButton ForceClone;

        #endregion

        #region Bot

        public static QMNestedButton BotOneMovment;

        #endregion

        #endregion

        #region InitializeActionMenu

        public static ActionMenuPage mainAW;
        // Movement Menu
        public static ActionMenuButton flightButton;
        // Mic Menu
        public static ActionMenuButton SerializationButton;
        public static ActionMenuButton SwastikaButton;
        public static ActionMenuButton BrokenUspeakButton;
        public static ActionMenuButton ItemLockButton;
        public static ActionMenuButton FreeFallButton;
        public static ActionMenuButton SpamMirrorsButton;

        #endregion

        internal static void InitializeHUD()
        {

        }

        internal static void InitializeSocialMenu()
        {
            UserInfo = GameObject.Find("UserInterface/MenuContent/Screens/UserInfo");
            UserInfoGetId = GameObject.Find("UserInterface/MenuContent/Screens/UserInfo/User Panel/UserIcon");

            new SMButton(SMButton.SMButtonType.EditStatus, UserInfo.transform, -115, 315, "Teleport To", delegate
            {
                foreach (var p in WorldUtils.GetPlayers())
                {
                    if (PlayerUtils.GetUser().id == p.GetAPIUser().id)
                    {
                        PlayerUtils.GetCurrentUser().transform.position = p.GetVRCPlayer().transform.position;
                    }
                }
            }, 1f, 1);

            new SMButton(SMButton.SMButtonType.EditStatus, UserInfo.transform, -115, 245, "Download VRCA", delegate
            {
                foreach (var p in WorldUtils.GetPlayers())
                {
                    if (PlayerUtils.GetUser().id == p.GetAPIUser().id)
                    {
                        var avatar = p.GetVRCPlayer().GetApiAvatar();
                        new Task(() => { ModulesFunctions.DownloadVRCA(avatar); }).Start();
                        new Task(() => { ModulesFunctions.DownloadAviImage(avatar); }).Start();
                    }
                }
            }, 1f, 1);

            new SMButton(SMButton.SMButtonType.EditStatus, UserInfo.transform, -115, 175, "Copy Avatar ID", delegate
            {
                foreach (var p in WorldUtils.GetPlayers())
                {
                    if (PlayerUtils.GetUser().id == p.GetAPIUser().id)
                    {
                        Clipboard.SetText(p.GetVRCPlayer().GetApiAvatar().id);
                    }
                    else
                    {
                        Clipboard.SetText(UserInfoGetId.GetComponent<MonoBehaviour1PublicVoAPVoOnAPVoAPVoAPVoUnique>().field_Private_APIUser_0.avatarId);
                    }
                }
            }, 1f, 1);

            new SMButton(SMButton.SMButtonType.EditStatus, UserInfo.transform, -115, 105, "Copy User ID", delegate
            {
                foreach (var p in WorldUtils.GetPlayers())
                {
                    if (PlayerUtils.GetUser().id == p.GetAPIUser().id)
                    {
                        Clipboard.SetText(p.GetVRCPlayer().GetUserID());
                    }
                    else
                    {
                        Clipboard.SetText(UserInfoGetId.GetComponent<MonoBehaviour1PublicVoAPVoOnAPVoAPVoAPVoUnique>().field_Private_APIUser_0.id);
                    }
                }
            }, 1f, 1);

            new SMButton(SMButton.SMButtonType.EditStatus, UserInfo.transform, -115, 35, "Copy User Info", delegate
            {
                foreach (var p in WorldUtils.GetPlayers())
                {
                    if (PlayerUtils.GetUser().id == p.GetAPIUser().id)
                    {
                        Clipboard.SetText($"╱▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔╲\nName: {p.GetDisplayName()} \nReg Name: {p.GetVRCPlayerApi().GetDisplayName()} \nUserPicURL: {p.GetVRCPlayer().GetAPIUser().profilePicImageUrl} \nUser ID: {p.GetVRCPlayer().GetUserID()} \nAvatar ID: {p.GetVRCPlayer().GetApiAvatar().id} \nAvatarImageURL: {p.GetVRCPlayer().GetAPIUser().currentAvatarImageUrl} \nAvatarAssetURL: {p.GetVRCPlayer().GetAPIUser().currentAvatarAssetUrl} \nIs Know: {p.GetVRCPlayer().GetAPIUser().hasKnownTrustLevel} \nIs Trusted: {p.GetVRCPlayer().GetAPIUser().hasTrustedTrustLevel}\nBio: {p.GetVRCPlayer().GetAPIUser().bio} \nStatus: {p.GetVRCPlayer().GetAPIUser().status} \nPlatform: {p.GetVRCPlayer().GetAPIUser().last_platform} \nIs Friend: {p.GetVRCPlayer().GetAPIUser().isFriend}\n╲▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂╱");
                    }
                    else
                    {
                        Clipboard.SetText($"╱▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔╲\nName: {UserInfoGetId.GetComponent<MonoBehaviour1PublicVoAPVoOnAPVoAPVoAPVoUnique>().field_Private_APIUser_0.GetDisplayName()} \nReg Name: {UserInfoGetId.GetComponent<MonoBehaviour1PublicVoAPVoOnAPVoAPVoAPVoUnique>().field_Private_APIUser_0.GetDisplayName()} \nUserPicURL: {UserInfoGetId.GetComponent<MonoBehaviour1PublicVoAPVoOnAPVoAPVoAPVoUnique>().field_Private_APIUser_0.profilePicImageUrl} \nUser ID: {UserInfoGetId.GetComponent<MonoBehaviour1PublicVoAPVoOnAPVoAPVoAPVoUnique>().field_Private_APIUser_0.id} \nAvatar ID: {UserInfoGetId.GetComponent<MonoBehaviour1PublicVoAPVoOnAPVoAPVoAPVoUnique>().field_Private_APIUser_0.avatarId} \nAvatarImageURL: {UserInfoGetId.GetComponent<MonoBehaviour1PublicVoAPVoOnAPVoAPVoAPVoUnique>().field_Private_APIUser_0.currentAvatarImageUrl} \nAvatarAssetURL: {UserInfoGetId.GetComponent<MonoBehaviour1PublicVoAPVoOnAPVoAPVoAPVoUnique>().field_Private_APIUser_0.currentAvatarAssetUrl} \nIs Know: {UserInfoGetId.GetComponent<MonoBehaviour1PublicVoAPVoOnAPVoAPVoAPVoUnique>().field_Private_APIUser_0.hasKnownTrustLevel} \nIs Trusted: {UserInfoGetId.GetComponent<MonoBehaviour1PublicVoAPVoOnAPVoAPVoAPVoUnique>().field_Private_APIUser_0.hasTrustedTrustLevel}\nBio: {UserInfoGetId.GetComponent<MonoBehaviour1PublicVoAPVoOnAPVoAPVoAPVoUnique>().field_Private_APIUser_0.bio} \nStatus: {UserInfoGetId.GetComponent<MonoBehaviour1PublicVoAPVoOnAPVoAPVoAPVoUnique>().field_Private_APIUser_0.status} \nPlatform: {UserInfoGetId.GetComponent<MonoBehaviour1PublicVoAPVoOnAPVoAPVoAPVoUnique>().field_Private_APIUser_0.last_platform} \nIs Friend: {UserInfoGetId.GetComponent<MonoBehaviour1PublicVoAPVoOnAPVoAPVoAPVoUnique>().field_Private_APIUser_0.isFriend}\n╲▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂╱");
                    }
                }
            }, 1f, 1);

            new SMButton(SMButton.SMButtonType.EditStatus, UserInfo.transform, -115, -35, "Invite Spam", delegate
            {
                MonoBehaviourPublicObApAcApStAcBoStBoObUnique.field_Private_Static_MonoBehaviourPublicObApAcApStAcBoStBoObUnique_0.prop_Api_0.PostOffice.Send(Invite.Create(UserInfoGetId.GetComponent<MonoBehaviour1PublicVoAPVoOnAPVoAPVoAPVoUnique>().field_Private_APIUser_0.id, "", new Location(ModulesFunctions.CopyWorldID()), WorldUtils.GetCurrentWorld().name));
                MonoBehaviourPublicObApAcApStAcBoStBoObUnique.field_Private_Static_MonoBehaviourPublicObApAcApStAcBoStBoObUnique_0.prop_Api_0.PostOffice.Send(Invite.Create(UserInfoGetId.GetComponent<MonoBehaviour1PublicVoAPVoOnAPVoAPVoAPVoUnique>().field_Private_APIUser_0.id, "", new Location(ModulesFunctions.CopyWorldID()), WorldUtils.GetCurrentWorld().name));
                MonoBehaviourPublicObApAcApStAcBoStBoObUnique.field_Private_Static_MonoBehaviourPublicObApAcApStAcBoStBoObUnique_0.prop_Api_0.PostOffice.Send(Invite.Create(UserInfoGetId.GetComponent<MonoBehaviour1PublicVoAPVoOnAPVoAPVoAPVoUnique>().field_Private_APIUser_0.id, "", new Location(ModulesFunctions.CopyWorldID()), WorldUtils.GetCurrentWorld().name));
                MonoBehaviourPublicObApAcApStAcBoStBoObUnique.field_Private_Static_MonoBehaviourPublicObApAcApStAcBoStBoObUnique_0.prop_Api_0.PostOffice.Send(Invite.Create(UserInfoGetId.GetComponent<MonoBehaviour1PublicVoAPVoOnAPVoAPVoAPVoUnique>().field_Private_APIUser_0.id, "", new Location(ModulesFunctions.CopyWorldID()), WorldUtils.GetCurrentWorld().name));
                MonoBehaviourPublicObApAcApStAcBoStBoObUnique.field_Private_Static_MonoBehaviourPublicObApAcApStAcBoStBoObUnique_0.prop_Api_0.PostOffice.Send(Invite.Create(UserInfoGetId.GetComponent<MonoBehaviour1PublicVoAPVoOnAPVoAPVoAPVoUnique>().field_Private_APIUser_0.id, "", new Location(ModulesFunctions.CopyWorldID()), WorldUtils.GetCurrentWorld().name));
                MonoBehaviourPublicObApAcApStAcBoStBoObUnique.field_Private_Static_MonoBehaviourPublicObApAcApStAcBoStBoObUnique_0.prop_Api_0.PostOffice.Send(Invite.Create(UserInfoGetId.GetComponent<MonoBehaviour1PublicVoAPVoOnAPVoAPVoAPVoUnique>().field_Private_APIUser_0.id, "", new Location(ModulesFunctions.CopyWorldID()), WorldUtils.GetCurrentWorld().name));
                MonoBehaviourPublicObApAcApStAcBoStBoObUnique.field_Private_Static_MonoBehaviourPublicObApAcApStAcBoStBoObUnique_0.prop_Api_0.PostOffice.Send(Invite.Create(UserInfoGetId.GetComponent<MonoBehaviour1PublicVoAPVoOnAPVoAPVoAPVoUnique>().field_Private_APIUser_0.id, "", new Location(ModulesFunctions.CopyWorldID()), WorldUtils.GetCurrentWorld().name));
            }, 1f, 1);

            new SMButton(SMButton.SMButtonType.EditStatus, UserInfo.transform, -115, -105, "Target Crash", delegate
            {
                foreach (var p in WorldUtils.GetPlayers())
                {
                    if (PlayerUtils.GetUser().id == p.GetAPIUser().id)
                    {
                        MelonCoroutines.Start(ModulesFunctions.TargetCrash(p));
                    }
                }
            }, 1f, 1);
        }

        internal static void InitializeQuickMenu()
        {
            #region Main Start Buttons

            #region TabMenu

            Menu = new QMTabMenu("FusionClient Menu", "FusionClient", AssetBundleManager.Logo);
            var openObject = Menu.GetMenuObject().transform.Find("Header_H1/RightItemContainer/Button_QM_Expand").gameObject;
            openObject.GetComponent<RectTransform>().sizeDelta = new Vector2(84, 84);
            openObject.SetActive(true);
            var iconObject = openObject.transform.Find("Icon").gameObject;
            iconObject.SetActive(true);
            var iconComp = iconObject.GetComponent<Image>();
            iconComp.sprite = AssetBundleManager.Discord;
            iconComp.overrideSprite = AssetBundleManager.Discord;
            var btnComp = openObject.GetComponent<UnityEngine.UI.Button>();
            btnComp.onClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            btnComp.onClick.AddListener(new System.Action(() => { PopupUtils.AskConfirmOpenURL("https://discord.gg/eWp8P4q5UU", "Discord"); }));

            #endregion

            #region ButtonsGroups

            Self = new QMNestedButton(Menu, 1, 0.5f, "Self", "Features that modify yourself", "FC - Self");
            Movement = new QMNestedButton(Menu, 2, 0.5f, "Movement", "Features that modify movement", "FC - Movement");
            World = new QMNestedButton(Menu, 3, 0.5f, "World", "Features that modify world", "FC - World");
            Protection = new QMNestedButton(Menu, 4, 0.5f, "Protection", "Features that Protect You From Bad Exploits", "FC - Protection");
            Exploits = new QMNestedButton(Menu, 1, 1.5f, "Exploits", "Features that Can Do Harm/Crash Players", "FC - Exploits");
            Visual = new QMNestedButton(Menu, 2, 1.5f, "Visual", "Features that will improve visuals", "FC - Visual");
            IKTweaks = new QMNestedButton(Menu, 3, 1.5f, "IKTweaks", "Features That Will Make You Look Like You Have VR", "FC - IKTweaks");
            Settings = new QMNestedButton(Menu, 4, 1.5f, "Settings", "Settings If You Dont Know What Settings Is Please Leave The Game <3", "FC - Settings");
            Bots = new QMNestedButton(Menu, 1, 2.5f, "Bots", "This is for bot functions start/stop so on", "FC - Bots");
            roomHistory = new QMNestedButton(Menu, 2, 2.5f, "Room\nHistory", "View and rejoin previous worlds you have been to", "FC - Instance History");

            #endregion

            #region Playerlist/Debug

            try
            {
                DebugPanel = new InfoPanel(GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/Wing_Right/Button").transform, 490, 0, 815, 1130, "")
                {
                    InfoText =
                    {
                        color = UnityEngine.Color.white,
                        supportRichText = true,
                        fontSize = 35,
                        fontStyle = FontStyle.Normal,
                        alignment = TextAnchor.UpperLeft
                    },
                    InfoBackground =
                    {
                        color = new Color(0, 0, 0, 0.85f),
                    }
                };
                DebugPanel.SetActive(Config.Main.DebugPanel);
            }
            catch (Exception e)
            {
                Logs.Log("[UI] Error Creating Debug! | Error Message: " + e.Message, ConsoleColor.Red);
            }

            try
            {
                PlayerList = new InfoPanel(GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/Wing_Left/Button").transform, -490, 0, 815, 1130, "")
                {
                    InfoText =
                    {
                        color = UnityEngine.Color.white,
                        supportRichText = true,
                        fontSize = 35,
                        fontStyle = FontStyle.Italic,
                        alignment = TextAnchor.UpperLeft
                    },
                    InfoBackground =
                    {
                        color = new Color(0, 0, 0, 0.85f),
                    }
                };
                PlayerList.InfoObject.AddComponent<PlayerList>();
                PlayerList.SetActive(Config.Main.PlayerList);
            }
            catch (Exception e)
            {
                Logs.Log("[UI] Error Creating Player List! | Error Message: " + e.Message, ConsoleColor.Red);
            }

            #endregion

            #endregion

            #region Self

            new QMToggleButton(Self, 1, 0, "Fake Ping", delegate
            {
                Config.Main.FakePing = true;
            }, delegate
            {
                Config.Main.FakePing = false;
            }, "Toggle: Fake Ping", Config.Main.FakePing);

            new QMToggleButton(Self, 2, 0, "Fake Frames", delegate
            {
                Config.Main.FakeFramesToggle = true;
            }, delegate
            {
                Config.Main.FakeFramesToggle = false;
            }, "Toggle: Fake Frames", Config.Main.FakeFramesToggle);

            new QMToggleButton(Self, 3, 0, "Offline Spoof", delegate
            {
                Config.Main.OfflineSpoof = true;
            }, delegate
            {
                Config.Main.OfflineSpoof = false;
            }, "Toggle: This Will Make You Look Like Your Offline <color=red>(WARNING This can get you ban)</color>", Config.Main.OfflineSpoof);

            new QMToggleButton(Self, 4, 0, "World Spoof", delegate
            {
                Config.Main.WorldSpoof = true;
            }, delegate
            {
                Config.Main.WorldSpoof = false;
            }, "Toggle: This Will Make You Look Like Your In A World Your Not In <color=red>(WARNING This can get you ban)</color>", Config.Main.WorldSpoof);

            new QMToggleButton(Self, 1, 1, "KinectMic", delegate
            {
                PlayerUtils.GetCurrentUser().SetBitrate(BitRate.BitRate_10K);
            }, delegate
            {
                PlayerUtils.GetCurrentUser().SetBitrate(BitRate.BitRate_24K);
            }, "Toggle: Makes Your Mic Sound Like A KinectMic");

            new QMToggleButton(Self, 2, 1, "EarRapeMic", delegate
            {
                USpeaker.field_Internal_Static_Single_1 = float.MaxValue;
            }, delegate
            {
                USpeaker.field_Internal_Static_Single_1 = 1;
            }, "Toggle: This Makes Your Mic Loud");

            new QMToggleButton(Self, 3, 1, "Clipping Distance", delegate
            {
                Camera.main.nearClipPlane = 0.001f;
                Config.Main.MinimalnClipping = true;
            }, delegate
            {
                Camera.main.nearClipPlane = Camera.main.nearClipPlane;
                Config.Main.MinimalnClipping = false;
            }, "Toggle: Makes It So You Dont Clip Into Avis With Your View Point", Config.Main.MinimalnClipping);

            new QMToggleButton(Self, 4, 1, "Comfy VR", delegate
            {
                Config.Main.ComfyVR = true;
            }, delegate
            {
                Config.Main.ComfyVR = false;
            }, "Toggle: Opens the VRCMenu in the Direction you are looking", Config.Main.ComfyVR);

            new QMToggleButton(Self, 1, 2, "HeadFlipper", delegate
            {
                ModulesFunctions.HeadFlipper(true);
            }, delegate
            {
                ModulesFunctions.HeadFlipper(false);
            }, "Toggle: This Makes It So You Can Flip Your Head In VR");

            new QMToggleButton(Self, 2, 2, "UnCap FPS", delegate
            {
                ModulesFunctions.FPSUnCap();
                Config.Main.FpsUncap = true;
            }, delegate
            {
                ModulesFunctions.FPSUnCap();
                Config.Main.FpsUncap = false;
            }, "Toggle: This Makes It So Can Get Up To 250 FPS", Config.Main.FpsUncap);

            new QMToggleButton(Self, 3, 2, "FlashLight", delegate
            {
                ModulesFunctions.FlashLightEnable();
            }, delegate
            {
                ModulesFunctions.FlashLightDisable();
            }, "Toggle: This Will Brighten Up The World Around You");

            new QMToggleButton(Self, 4, 2, "PortalPrompt", delegate
            {
                Config.Main.PortalPrompt = true;
            }, delegate
            {
                Config.Main.PortalPrompt = false;
            }, "Toggle: This Makes It So Get A PortalPrompt When Trying To Go Into A Portal", Config.Main.PortalPrompt);

            new QMSingleButton(Self, 1, 3, "Copy Userid", delegate
            {
                Clipboard.SetText(APIUser.CurrentUser.id);
                Logs.Log($"Copied your UserID", ConsoleColor.Cyan);
            }, "Copys your userID to your clipboard", true);

            new QMSingleButton(Self, 1, 3.5f, "Copy AvatarID", delegate
            {
                Clipboard.SetText(APIUser.CurrentUser.avatarId);
                Logs.Log($"Copied your AvatarID", ConsoleColor.Cyan);
            }, "Copys your userID to your AvatarID", true);

            new QMSingleButton(Self, 2, 3, "ChangeAvi\nBy ID", delegate
            {
                PopupUtils.InputPopup("Enter Avatar ID", "avtr_XXXXXXX", delegate (string s)
                {
                    ModulesFunctions.ChangeAvatar(s);
                });
            }, "Change into an avatar by avatar ID!", true);

            new QMSingleButton(Self, 2, 3.5f, "Invite\nAll Friends", delegate
            {
                PopupUtils.AlertV2("Are you sure that you want to invite all your friends ? \n WARNING: This Sends Lots Of API Calls This Can Get You Ban", "Yes", delegate ()
                {
                    foreach (var Id in APIUser.CurrentUser.friendIDs)
                    {
                        ModulesFunctions.SendInvite("FusionClient: Auto Invite All Friends", Id, WorldUtils.GetCurrentWorld().id, WorldUtils.GetCurrentInstance().id.Split(':')[1]);
                        Logs.Log($"Invited " + $"{Id}" + "To World " + WorldUtils.GetCurrentWorld().name, ConsoleColor.Cyan);
                    }
                    PopupUtils.HideCurrentPopUp();
                }, "No", delegate ()
                {
                    PopupUtils.HideCurrentPopUp();
                    Logs.Log($"Invite All Friends Canceled", ConsoleColor.Cyan);
                });
            }, "Invite all your friends in this world.", true);

            new QMSingleButton(Self, 3, 3f, "Dump\nFriendlist", delegate
            {
                MelonCoroutines.Start(Start());
                IEnumerator Start()
                {
                    if (!File.Exists($"Fusion Client/Misc/FriendList.txt"))
                        File.Create("Fusion Client/Misc/FriendList.txt");
                    while (!File.Exists("Fusion Client/Misc/FriendList.txt")) yield return null;
                    yield return new WaitForSeconds(1);

                    foreach (var Friend in APIUser.CurrentUser.friendIDs)
                    {
                        if (!File.ReadLines("Fusion Client/Misc/FriendList.txt").Contains(Friend))
                        {
                            File.AppendAllText("Fusion Client/Misc/FriendList.txt", $"{Friend}\n");
                            Logs.Log($"Added {Friend} to the list.", ConsoleColor.Cyan);
                        }
                        else Logs.Log($"Friend is already in the list: {Friend}", ConsoleColor.Cyan);
                    }
                }
            }, "Will log every of your friends id in case you get ban", true);

            new QMSingleButton(Self, 3, 3.5f, "Add\nFriendlist", delegate
            {
                ModulesFunctions.FriendEveryone(APIUser.CurrentUser.authToken);
            }, "Will add all of your friends in case you get ban", true);


            #region Page 2

            var Page2Self = new QMNestedButton(Self, 4, 3, "Page 2", "Page 2 Of Self", "FC - Self");

            new QMToggleButton(Page2Self, 1, 0, "Hide Avi", delegate
            {
                ModulesFunctions.HideSelfBool = true;
                ModulesFunctions.HideSelf().Start();
            }, delegate
            {
                ModulesFunctions.HideSelfBool = false;
                PlayerUtils.GetCurrentUser().prop_VRCAvatarManager_0.gameObject.SetActive(true);
                GameObject.Find("/UserInterface/MenuContent/Screens/Avatar/AvatarPreviewBase").SetActive(true);
            }, "Toggle: Locally Hides Your Avi");

            new QMSingleButton(Page2Self, 2.5f, 3.5f, "Download VRCA", delegate
            {
                var avatar = PlayerUtils.GetCurrentUser().GetApiAvatar();
                new Task(() => { ModulesFunctions.DownloadVRCA(avatar); }).Start();
                new Task(() => { ModulesFunctions.DownloadAviImage(avatar); }).Start();
            }, "Downloads Your VRCA Of The Avatar Your In", true);

            #endregion

            #endregion

            #region Movement

            FlyToggle = new QMToggleButton(Movement, 1, 0, "Fly/Noclip", delegate
            {
                Flight.FlyOn();
            }, delegate
            {
                Flight.FlyOff();
            }, "Toggle: Fly/Noclip");

            new QMToggleButton(Movement, 2, 0, "Speed Hack", delegate
            {
                Flight.SpeedHack = true;
            }, delegate
            {
                Flight.SpeedHack = false;
            }, "Toggle: Makes You Run Fast");

            new QMToggleButton(Movement, 3, 0, "Inf Jump", delegate
            {
                Config.Main.InfJump = true;
            }, delegate
            {
                Config.Main.InfJump = false;
            }, "Toggle: This Will Make You Jump With No Limit", Config.Main.InfJump);

            new QMToggleButton(Movement, 4, 0, "BunnyHop", delegate
            {
                Config.Main.BunnyHop = true;
            }, delegate
            {
                Config.Main.BunnyHop = false;
            }, "Toggle: This Will Help You BunnyHop When Jump Is Held Down", Config.Main.BunnyHop);

            new QMToggleButton(Movement, 1, 1, "MouseTP", delegate
            {
                Config.Main.MouseTP = true;
            }, delegate
            {
                Config.Main.MouseTP = false;
            }, "Toggle: If You Press `LeftControl + T` You Will Get Tped To Where Your Looking", Config.Main.MouseTP);

            new QMToggleButton(Movement, 2, 1, "Beyblade Mode", delegate
            {
                PlayerUtils.GetCurrentUser().transform.rotation = new Quaternion(90f, 0f, 0f, 0f);
                PlayerUtils.GetCurrentUser().transform.position += new Vector3(0f, 1f, 0f);
                Flight.FlyOn();
            }, delegate
            {
                PlayerUtils.GetCurrentUser().transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
                PlayerUtils.GetCurrentUser().transform.position += new Vector3(0f, 1f, 0f);
                Flight.FlyOff();
            }, "Toggle: Makes You Spin Like Your On Crack!");

            new QMSingleButton(Movement, 2.5f, 3.5f, "Force Jump", delegate
            {
                if (PlayerUtils.GetCurrentUser().GetVRCPlayerApi().GetJumpImpulse() > 0)
                {
                    Logs.Hud("Jumping is already enabled!");
                    return;
                }
                else
                {
                    PlayerUtils.GetCurrentUser().GetVRCPlayerApi().SetJumpImpulse(2.8f);
                    Logs.Hud("Jumping is now enabled!");
                }
            }, "Makes It So You Can Jump In Worlds Your Cound Not Jump In Before", true);

            #endregion

            #region World

            new QMToggleButton(World, 1, 0, "Post\nProcessing", delegate
            {
                Config.Main.PostProcessing = true;
                ModulesFunctions.PostProcessing(false);
            }, delegate
            {
                Config.Main.PostProcessing = false;
                ModulesFunctions.PostProcessing(true);
            }, "Toggle: PostProcessing On/Off", Config.Main.PostProcessing);

            new QMToggleButton(World, 2, 0, "World Pickups", delegate
            {
                Config.Main.NoPickups = true;
                ModulesFunctions.TogglePickups(true);
            }, delegate
            {
                Config.Main.NoPickups = false;
                ModulesFunctions.TogglePickups(false);
            }, "Toggle: World Pickups On/Off", Config.Main.NoPickups);

            new QMToggleButton(World, 3, 0, "Seats", delegate
            {
                Config.Main.AntiSeats = true;
                ModulesFunctions.ToggleSeats(true);
            }, delegate
            {
                Config.Main.AntiSeats = false;
                ModulesFunctions.ToggleSeats(false);
            }, "Toggle: This Turns Off Seats In The World On/Off", Config.Main.AntiSeats);

            new QMToggleButton(World, 4, 0, "No Pickup", delegate
            {
                ModulesFunctions.NoPickUps(true);
            }, delegate
            {
                ModulesFunctions.NoPickUps(false);
            }, "Toggle: Pickups On/Off");

            new QMToggleButton(World, 1, 1, "World Portals", delegate
            {
                Config.Main.WorldPortals = true;
                ModulesFunctions.AntiWorldPortals(true);
            }, delegate
            {
                Config.Main.WorldPortals = false;
                ModulesFunctions.AntiWorldPortals(false);
            }, "Toggle: World Portals On/Off", Config.Main.WorldPortals);

            new QMToggleButton(World, 2, 1, "Video player", delegate
            {
                Config.Main.VideoPlayer = true;
                ModulesFunctions.AntiVideoplayer(true);
            }, delegate
            {
                Config.Main.VideoPlayer = false;
                ModulesFunctions.AntiVideoplayer(false);
            }, "Toggle: Video player On/Off", Config.Main.VideoPlayer);

            new QMToggleButton(World, 3, 1, "Optimize Mirrors", delegate
            {
                Config.Main.OptMirrors = true;
                ModulesFunctions.OptimizeMirrors(true);
            }, delegate
            {
                Config.Main.OptMirrors = false;
                ModulesFunctions.OptimizeMirrors(false);
            }, "Toggle: Optimize Mirrors On/Off", Config.Main.OptMirrors);

            new QMToggleButton(World, 4, 1, "AutoPortal\nDelete", delegate
            {
                Config.Main.AutoPortalDelete = true;
            }, delegate
            {
                Config.Main.AutoPortalDelete = false;
            }, "Toggle: Auto Removes Portals", Config.Main.AutoPortalDelete);

            new QMToggleButton(World, 1, 2, "Freeze Portal Time", delegate
            {
                Config.Main.PortalTimeReset = true;
            }, delegate
            {
                Config.Main.PortalTimeReset = false;
            }, "Toggle: Freezes Your Portal Time", Config.Main.PortalTimeReset);

            new QMSingleButton(World, 2, 2.5f, "Respawn\nPickups", delegate
            {
                ModulesFunctions.RespawnPickups();
            }, "Respawns All Of The Pickups In The World", true);

            new QMSingleButton(World, 3, 2.5f, "Reload\nAll Avis", delegate
            {
                foreach (var Player in PlayerUtils.GetPlayers())
                {
                    Player.ReloadAvatar();
                }
            }, "Reloads All Avis In The World", true);

            new QMSingleButton(World, 1, 3, "Copy\nWorld Info", delegate
            {
                Clipboard.SetText($"╱▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔╲\nWorld Name: {WorldUtils.GetCurrentWorld().name} \nWorld ID: {WorldUtils.GetWorldID()} \nWorld Description: {WorldUtils.GetCurrentWorld().description} \nWorld PicURL: {WorldUtils.GetCurrentWorld().imageUrl} \nWorld Auther Name: {WorldUtils.GetCurrentWorld().authorName} \nWorld Auther ID: {WorldUtils.GetCurrentWorld().authorId} \nWorld AssetURL: {WorldUtils.GetCurrentWorld().assetUrl} \n╲▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂╱");
            }, "teleport to the selected user", true);

            new QMSingleButton(World, 2, 3.5f, "Drop Portal\nTo WorldID", delegate
            {
                ModulesFunctions.PortalToWorld();
            }, "Join a room by World ID", true);

            new QMSingleButton(World, 2, 3f, "Join\nWorld", delegate
            {
                PopupUtils.InputPopup("Enter World ID", "Enter", delegate (string s)
                {
                    new WaitForSeconds(15f);
                    ModulesFunctions.JoinRoom(s);
                });
            }, "Join a room by World ID", true);

            new QMSingleButton(World, 3, 3.5f, "Copy\nWorld ID", delegate
            {
                System.Windows.Forms.Clipboard.SetText(ModulesFunctions.CopyWorldID());
                Logs.Log("World: " + ModulesFunctions.CopyWorldID() + " Has Been Copied To Your ClipBoard", ConsoleColor.Cyan);
            }, "Copies the current instance id to your clipboard", true);

            new QMSingleButton(World, 3, 3, "ReJoin\nWorld", delegate
            {
                Networking.GoToRoom(ModulesFunctions.CopyWorldID());
            }, "Rejoins Current Instance", true);

            new QMSingleButton(World, 4, 3, "Delete\nPortals", delegate
            {
                ModulesFunctions.DeletePortals();
            }, "Globally delete all player dropped portals", true);

            new QMSingleButton(World, 4, 3.5f, "Download\nVRCW", delegate
            {
                var world = WorldUtils.GetCurrentWorld();
                if (!File.Exists(ModFiles.VRCWPath + $"\\FusionClient-{world.name}-{world.authorName}-{world.version}.vrcw"))
                {
                    ModulesFunctions.DownloadVRCW(world);
                    ModulesFunctions.DownloadWorldImage(world);
                }
                else
                {
                    Logs.Log("[VRCW] That world is already downloaded!", ConsoleColor.Yellow);
                }
            }, "Downloads The VRCW Of The World Your In", true);

            #region World Cheats

            var WorldCheats = new QMNestedButton(World, 1, 3.5f, "World Cheats", "Features That Modify Game Worlds", "FC - World Cheats", true);
            var STDMenu = new QMNestedButton(WorldCheats, 1, 0, "STD", "Super Tower Defence Cheats", "FC - STD");
            var AmongUsMenu = new QMNestedButton(WorldCheats, 2, 0, "AmongUs", "AmongUs Cheats", "FC - AmongUs");
            var ClubBMenu = new QMNestedButton(WorldCheats, 3, 0, "ClubB", "ClubB Cheats", "FC - ClubB");
            var FBTMenu = new QMNestedButton(WorldCheats, 4, 0, "FBT", "FBT Cheats", "FC - FBT");
            var Murder4Menu = new QMNestedButton(WorldCheats, 1, 1, "Murder4", "Murder4 Cheats", "FC - Murder4");
            var PrisonEscapeMenu = new QMNestedButton(WorldCheats, 2, 1, "Prison Escape", "Prison Escape Cheats", "FC - Prison Escape");

            #region STD

            new QMSingleButton(STDMenu, 1, 0, "Start Round", delegate
            {
                ModulesFunctions.StartRound();
            }, "This Starts The Game So You Dont Need To Run To The Button");

            new QMSingleButton(STDMenu, 2, 0, "Reset Game", delegate
            {
                ModulesFunctions.ResetGame();
            }, "This Will Reset Your Game To Make It A New Game");

            #endregion

            #region AmongUs

            new QMSingleButton(AmongUsMenu, 2, 1, "Complete\nAllTasks", delegate
            {
                ModulesFunctions.ForceCompleteAllTasks();
            }, "This Will Force All Task To Be Complete", true);

            new QMSingleButton(AmongUsMenu, 3, 1f, "Force Close Vote", delegate
            {
                ModulesFunctions.ForceCloseVote();
            }, "This Will Force Close The Votting Stage Of AmongUs", true);

            new QMSingleButton(AmongUsMenu, 1, 1.5f, "Force Body Is Found", delegate
            {
                ModulesFunctions.ForceBodywasfound();
            }, "This Will Force AmongUs Into Thinking A Body Was Found", true);

            new QMSingleButton(AmongUsMenu, 2, 1.5f, "Force Skip Void", delegate
            {
                ModulesFunctions.ForceSkipVote();
            }, "This Will Force The Vote To Be Skipped", true);

            new QMSingleButton(AmongUsMenu, 3, 1.5f, "Force Emergency Meeting", delegate
            {
                ModulesFunctions.Emergencymeeting();
            }, "This Will Force An Emergency Meeting", true);

            new QMSingleButton(AmongUsMenu, 4, 1.5f, "Force Start Game", delegate
            {
                ModulesFunctions.ForceStartGame();
            }, "This Will Force Start The Lobby", true);

            new QMSingleButton(AmongUsMenu, 1, 2, "Force Stop Game", delegate
            {
                ModulesFunctions.ForceAbortGame();
            }, "This Will Force Stop The Game", true);

            new QMSingleButton(AmongUsMenu, 2, 2f, "Force Imposter Win", delegate
            {
                ModulesFunctions.ImposterForceWin();
            }, "This Will Force Imposter's To Win The Game", true);

            new QMSingleButton(AmongUsMenu, 3, 2, "Force Crew Win", delegate
            {
                ModulesFunctions.CrewForceWin();
            }, "This Will Force Crew To Win The Game", true);

            new QMSingleButton(AmongUsMenu, 4, 2, "Kill All Players", delegate
            {
                ModulesFunctions.ForceKillAllPlayers();
            }, "This Will Force Kill All Players In The Lobby", true);

            new QMSingleButton(AmongUsMenu, 1, 2.5f, "Everyone Crewmate", delegate
            {
                var Nodes = GameObject.Find("Player Nodes");
                foreach (var Object in Nodes.GetComponentsInChildren<Transform>())
                {
                    if (Object.name != Nodes.name) MiscUtils.SendUdonRPC(Object.gameObject, $"SyncAssignB");
                }
            }, "Everyone Crewmate", true);

            new QMSingleButton(AmongUsMenu, 2, 2.5f, "Everyone Impostor", delegate
            {
                var Nodes = GameObject.Find("Player Nodes");
                foreach (var Object in Nodes.GetComponentsInChildren<Transform>())
                {
                    if (Object.name != Nodes.name) MiscUtils.SendUdonRPC(Object.gameObject, $"SyncAssignM");
                }
            }, "Everyone Impostor", true);

            new QMSingleButton(AmongUsMenu, 3, 2.5f, "Become Impostor", delegate
            {
                var Nodes = GameObject.Find("Player Nodes");
                foreach (var Object in Nodes.GetComponentsInChildren<Transform>())
                {
                    if (Object.name != Nodes.name) MiscUtils.SendUdonRPC(Object.gameObject, $"SyncAssignM", PlayerUtils.GetCurrentUser()._player, true);
                }
            }, "Become Crewmate <color=red>(Makes It Look Like All Players Are Impostor But There Not)</color>", true);

            new QMSingleButton(AmongUsMenu, 4, 2.5f, "Become Crewmate", delegate
            {
                var Nodes = GameObject.Find("Player Nodes");
                foreach (var Object in Nodes.GetComponentsInChildren<Transform>())
                {
                    if (Object.name != Nodes.name) MiscUtils.SendUdonRPC(Object.gameObject, $"SyncAssignB", PlayerUtils.GetCurrentUser()._player, true);
                }
            }, "Become Crewmate <color=red>(Makes It Look Like All Players Are Crewmate But There Not)</color>", true);

            #endregion

            #region Club B

            new QMSingleButton(ClubBMenu, 1, 1, "Tp To\nRoom 1", delegate
            {
                ModulesFunctions.TpToRoom1();
            }, "This Will Tp You To BedRoom 1", true);

            new QMSingleButton(ClubBMenu, 2, 1, "Tp To\nRoom 2", delegate
            {
                ModulesFunctions.TpToRoom2();
            }, "This Will Tp You To BedRoom 2", true);

            new QMSingleButton(ClubBMenu, 3, 1, "Tp To\nRoom 3", delegate
            {
                ModulesFunctions.TpToRoom3();
            }, "This Will Tp You To BedRoom 3", true);

            new QMSingleButton(ClubBMenu, 4, 1, "Tp To\nRoom 4", delegate
            {
                ModulesFunctions.TpToRoom4();
            }, "This Will Tp You To BedRoom 4", true);

            new QMSingleButton(ClubBMenu, 1, 1.5f, "Tp To\nRoom 5", delegate
            {
                ModulesFunctions.TpToRoom5();
            }, "This Will Tp You To BedRoom 5", true);

            new QMSingleButton(ClubBMenu, 2, 1.5f, "Tp To\nRoom 6", delegate
            {
                ModulesFunctions.TpToRoom6();
            }, "This Will Tp You To BedRoom 6", true);

            new QMSingleButton(ClubBMenu, 3, 1.5f, "Tp To\nRoom VIP", delegate
            {
                ModulesFunctions.TpToRoom7();
            }, "This Will Tp You To BedRoom 7", true);

            new QMSingleButton(ClubBMenu, 4, 1.5f, "Become Elite", delegate
            {
                ModulesFunctions.EliteBool = true;
                MelonCoroutines.Start(ModulesFunctions.BecomeElite());
            }, "This Will Make You VIP", true);

            #endregion

            #region FBT

            new QMSingleButton(FBTMenu, 2, 1.5f, "Unlock\nAll Doors", delegate
            {
                ModulesFunctions.OpenAllRooms();
            }, "Unlocks All Room Doors In FBT");

            new QMSingleButton(FBTMenu, 3, 1.5f, "Lock/Unlock\nAll Doors", delegate
            {
                ModulesFunctions.LockUnlock();
            }, "Locks And Unlocks All Doors In FBT Haven");

            #endregion

            #region Murder4

            new QMSingleButton(Murder4Menu, 1, 0, "Force Win\nBystanders", delegate
            {
                ModulesFunctions.ForceWinBystand();
            }, "Forces A Win For Bystanders", true);

            new QMSingleButton(Murder4Menu, 2, 0, "Force Win\nMurder", delegate
            {
                ModulesFunctions.ForceWinMurder();
            }, "Forces A Win For Murder", true);

            new QMSingleButton(Murder4Menu, 3, 0, "Stop Game", delegate
            {
                ModulesFunctions.AbortGame();
            }, "Stops Game Thats Running", true);

            new QMSingleButton(Murder4Menu, 4, 0, "Start Game", delegate
            {
                ModulesFunctions.StartGame();
            }, "Starts Game", true);

            new QMSingleButton(Murder4Menu, 1, 0.5f, "Blind All\nUsers", delegate
            {
                ModulesFunctions.BlackOutAll();
            }, "Blinds All Users Of The Game", true);

            new QMSingleButton(Murder4Menu, 1, 1, "Kill All\nPlayers", delegate
            {
                ModulesFunctions.KillAllPlayers();
            }, "This Will Kill All Players In The Game", true);

            new QMSingleButton(Murder4Menu, 1, 1.5f, "Get Shotgun", delegate
            {
                ModulesFunctions.GetShotgun();
            }, "This Will Get A Shotgun From The Map", true);

            new QMSingleButton(Murder4Menu, 1, 2, "Get Grenade", delegate
            {
                ModulesFunctions.GetFrag();
            }, "This Will Get A Grenade From The Map", true);

            new QMSingleButton(Murder4Menu, 1, 2.5f, "Get Luger", delegate
            {
                ModulesFunctions.GetLuger();
            }, "This Will Get A Luger From The Map", true);

            new QMSingleButton(Murder4Menu, 1, 3, "Get Knife", delegate
            {
                ModulesFunctions.GetKnife();
            }, "This Will Get A Knife From The Map", true);

            new QMSingleButton(Murder4Menu, 1, 3.5f, "Get Revolver", delegate
            {
                ModulesFunctions.GetRevolver();
            }, "This Will Get A Revolver From The Map", true);

            new QMSingleButton(Murder4Menu, 2, 3.5f, "Everyone Murder", delegate
            {
                var Nodes = GameObject.Find("Player Nodes");
                foreach (var Object in Nodes.GetComponentsInChildren<Transform>())
                {
                    if (Object.name != Nodes.name) MiscUtils.SendUdonRPC(Object.gameObject, "SyncAssignM");
                }
            }, "Make Everyone Murder", true);

            new QMSingleButton(Murder4Menu, 3, 3.5f, "Everyone Detective", delegate
            {
                var Nodes = GameObject.Find("Player Nodes");
                foreach (var Object in Nodes.GetComponentsInChildren<Transform>())
                {
                    if (Object.name != Nodes.name) MiscUtils.SendUdonRPC(Object.gameObject, "SyncAssignD");
                }
            }, "Make Everyone Detective", true);

            new QMSingleButton(Murder4Menu, 4, 0.5f, "Everyone Bystander", delegate
            {
                var Nodes = GameObject.Find("Player Nodes");
                foreach (var Object in Nodes.GetComponentsInChildren<Transform>())
                {
                    if (Object.name != Nodes.name) MiscUtils.SendUdonRPC(Object.gameObject, "SyncAssignB");
                }
            }, "Make Everyone Bystander", true);

            new QMSingleButton(Murder4Menu, 4, 1, "Become Murder", delegate
            {
                var Nodes = GameObject.Find("Player Nodes");
                foreach (var Object in Nodes.GetComponentsInChildren<Transform>())
                {
                    if (Object.name != Nodes.name) MiscUtils.SendUdonRPC(Object.gameObject, "SyncAssignM", PlayerUtils.GetCurrentUser()._player, true);
                }
            }, "Make Your Self Murder <color=red>(Makes It Look Like All Players Are Murder But There Not)</color>", true);

            new QMSingleButton(Murder4Menu, 4, 1.5f, "Become Detective", delegate
            {
                var Nodes = GameObject.Find("Player Nodes");
                foreach (var Object in Nodes.GetComponentsInChildren<Transform>())
                {
                    if (Object.name != Nodes.name) MiscUtils.SendUdonRPC(Object.gameObject, "SyncAssignD", PlayerUtils.GetCurrentUser()._player, true);
                }
            }, "Make Your Self Detective <color=red>(Makes It Look Like All Players Are Detective But There Not)</color>", true);

            new QMSingleButton(Murder4Menu, 4, 2, "Become Bystander", delegate
            {
                var Nodes = GameObject.Find("Player Nodes");
                foreach (var Object in Nodes.GetComponentsInChildren<Transform>())
                {
                    if (Object.name != Nodes.name) MiscUtils.SendUdonRPC(Object.gameObject, "SyncAssignB", PlayerUtils.GetCurrentUser()._player, true);
                }
            }, "Make Your Self Bystander <color=red>(Makes It Look Like All Players Are Bystander But There Not)</color>", true);

            new QMSingleButton(Murder4Menu, 4, 2.5f, "Check Roles", delegate
            {
                Logs.Hud("<color=cyan>FusionClient</color> | Starting Please Wait ");
                MelonCoroutines.Start(ModulesFunctions.CheckRoles());
            }, "This Will Check To See Who Is Who", true);

            new QMSingleButton(Murder4Menu, 4, 3, "Lock All Doors", delegate
            {
                List<Transform> Doors = new List<Transform>()
                {
                    GameObject.Find("Door").transform.Find("Door Anim/Hinge/Interact lock"),
                    GameObject.Find("Door (3)").transform.Find("Door Anim/Hinge/Interact lock"),
                    GameObject.Find("Door (4)").transform.Find("Door Anim/Hinge/Interact lock"),
                    GameObject.Find("Door (5)").transform.Find("Door Anim/Hinge/Interact lock"),
                    GameObject.Find("Door (6)").transform.Find("Door Anim/Hinge/Interact lock"),
                    GameObject.Find("Door (7)").transform.Find("Door Anim/Hinge/Interact lock"),
                    GameObject.Find("Door (15)").transform.Find("Door Anim/Hinge/Interact lock"),
                    GameObject.Find("Door (16)").transform.Find("Door Anim/Hinge/Interact lock"),
                    GameObject.Find("Door (8)").transform.Find("Door Anim/Hinge/Interact lock"),
                    GameObject.Find("Door (13)").transform.Find("Door Anim/Hinge/Interact lock"),
                    GameObject.Find("Door (17)").transform.Find("Door Anim/Hinge/Interact lock"),
                    GameObject.Find("Door (18)").transform.Find("Door Anim/Hinge/Interact lock"),
                    GameObject.Find("Door (19)").transform.Find("Door Anim/Hinge/Interact lock"),
                    GameObject.Find("Door (20)").transform.Find("Door Anim/Hinge/Interact lock"),
                    GameObject.Find("Door (21)").transform.Find("Door Anim/Hinge/Interact lock"),
                    GameObject.Find("Door (22)").transform.Find("Door Anim/Hinge/Interact lock"),
                    GameObject.Find("Door (23)").transform.Find("Door Anim/Hinge/Interact lock"),
                    GameObject.Find("Door (14)").transform.Find("Door Anim/Hinge/Interact lock"),
                };
                foreach (var Door in Doors)
                {
                    MiscUtils.TakeOwnershipIfNecessary(Door.gameObject);
                    Door.GetComponent<UdonBehaviour>().Interact();
                }
            }, "Lock All Doors", true);

            new QMSingleButton(Murder4Menu, 4, 3.5f, "UnLock All Doors", delegate
            {
                List<Transform> Doors = new List<Transform>()
                {
                    GameObject.Find("Door").transform.Find("Door Anim/Hinge/Interact shove"),
                    GameObject.Find("Door (3)").transform.Find("Door Anim/Hinge/Interact shove"),
                    GameObject.Find("Door (4)").transform.Find("Door Anim/Hinge/Interact shove"),
                    GameObject.Find("Door (5)").transform.Find("Door Anim/Hinge/Interact shove"),
                    GameObject.Find("Door (6)").transform.Find("Door Anim/Hinge/Interact shove"),
                    GameObject.Find("Door (7)").transform.Find("Door Anim/Hinge/Interact shove"),
                    GameObject.Find("Door (15)").transform.Find("Door Anim/Hinge/Interact shove"),
                    GameObject.Find("Door (16)").transform.Find("Door Anim/Hinge/Interact shove"),
                    GameObject.Find("Door (8)").transform.Find("Door Anim/Hinge/Interact shove"),
                    GameObject.Find("Door (13)").transform.Find("Door Anim/Hinge/Interact shove"),
                    GameObject.Find("Door (17)").transform.Find("Door Anim/Hinge/Interact shove"),
                    GameObject.Find("Door (18)").transform.Find("Door Anim/Hinge/Interact shove"),
                    GameObject.Find("Door (19)").transform.Find("Door Anim/Hinge/Interact shove"),
                    GameObject.Find("Door (20)").transform.Find("Door Anim/Hinge/Interact shove"),
                    GameObject.Find("Door (21)").transform.Find("Door Anim/Hinge/Interact shove"),
                    GameObject.Find("Door (22)").transform.Find("Door Anim/Hinge/Interact shove"),
                    GameObject.Find("Door (23)").transform.Find("Door Anim/Hinge/Interact shove"),
                    GameObject.Find("Door (14)").transform.Find("Door Anim/Hinge/Interact shove"),
                };
                foreach (var Door in Doors)
                {
                    MiscUtils.TakeOwnershipIfNecessary(Door.gameObject);
                    Door.GetComponent<UdonBehaviour>().Interact();
                    Door.GetComponent<UdonBehaviour>().Interact();
                    Door.GetComponent<UdonBehaviour>().Interact();
                    Door.GetComponent<UdonBehaviour>().Interact();
                }
            }, "UnLock All Doors", true);

            new QMToggleButton(Murder4Menu, 2, 1.5f, "Spam All Doors", delegate
            {
                ModulesFunctions.SpamDoorsBool = true;
                MelonCoroutines.Start(ModulesFunctions.SpamDoors());
            }, delegate
            {
                ModulesFunctions.SpamDoorsBool = false;
                MelonCoroutines.Start(ModulesFunctions.SpamDoors());
            }, "Toggle: Spam All Doors");

            new QMToggleButton(Murder4Menu, 3, 1.5f, "GodMode", delegate
            {
                GodMode = true;
            }, delegate
            {
                GodMode = false;
            }, "Toggle: Ear rape");

            #endregion

            #region Prison Escape

            new QMSingleButton(PrisonEscapeMenu, 2, 0, "Start Game", delegate
            {
                ModulesFunctions.StartGamePE();
            }, "Starts The Game", true);

            new QMSingleButton(PrisonEscapeMenu, 3, 0, "Grab All Money", delegate
            {
                ModulesFunctions.GrabAllMoneyPE();
            }, "This Grabs All Money In The Game", true);

            new QMSingleButton(PrisonEscapeMenu, 1, 0.5f, "Grab All Modded Money", delegate
            {
                ModulesFunctions.GrabAllModdedMoneyPE();
            }, "This Grabs All Modded Money In The Game", true);

            new QMSingleButton(PrisonEscapeMenu, 2, 0.5f, "Kill All Players", delegate
            {
                ModulesFunctions.KillPlayersPE();
            }, "This Will Kill All Players", true);

            new QMSingleButton(PrisonEscapeMenu, 3, 0.5f, "DrinkEnergyDrink", delegate
            {
                ModulesFunctions.DrinkEnergyDrinkPE();
            }, "This Will Drink A EnergyDrink", true);

            new QMSingleButton(PrisonEscapeMenu, 4, 0.5f, "Get KeyCards", delegate
            {
                ModulesFunctions.GetKeyCardsPE();
            }, "This Tp's The Keycards To Your Feet", true);

            new QMSingleButton(PrisonEscapeMenu, 1, 1, "Get ShotGuns", delegate
            {
                ModulesFunctions.GetShotgunPE();
            }, "This Tp's The ShotGun To Your Feet", true);

            new QMSingleButton(PrisonEscapeMenu, 2, 1, "Get Revolvers", delegate
            {
                ModulesFunctions.GetRevolverPE();
            }, "This Tp's The Revolver To Your Feet", true);

            new QMSingleButton(PrisonEscapeMenu, 3, 1, "Get Snipers", delegate
            {
                ModulesFunctions.GetSniperPE();
            }, "This Tp's The Sniper To Your Feet", true);

            new QMSingleButton(PrisonEscapeMenu, 4, 1, "Get SMGs", delegate
            {
                ModulesFunctions.GetSMGPE();
            }, "This Tp's The SMG To Your Feet", true);

            new QMSingleButton(PrisonEscapeMenu, 1, 1.5f, "Get AssaultRifles", delegate
            {
                ModulesFunctions.GetAssaultRiflePE();
            }, "This Tp's The AssaultRifle To Your Feet", true);

            new QMSingleButton(PrisonEscapeMenu, 2, 1.5f, "Get Knifes", delegate
            {
                ModulesFunctions.GetKnifePE();
            }, "This Tp's The Knife To Your Feet", true);

            new QMSingleButton(PrisonEscapeMenu, 3, 1.5f, "Get SmokeGrenades", delegate
            {
                ModulesFunctions.GetSmokeGrenadePE();
            }, "This Tp's The SmokeGrenade To Your Feet", true);

            new QMSingleButton(PrisonEscapeMenu, 4, 1.5f, "Get RPGs", delegate
            {
                ModulesFunctions.GetRPGPE();
            }, "This Tp's The RPG To Your Feet", true);

            new QMSingleButton(PrisonEscapeMenu, 1, 2, "Get Grenades", delegate
            {
                ModulesFunctions.GetGrenadePE();
            }, "This Tp's The Grenade To Your Feet", true);

            new QMSingleButton(PrisonEscapeMenu, 2, 2, "Get Machetes", delegate
            {
                ModulesFunctions.GetMachetePE();
            }, "This Tp's The Machete To Your Feet", true);

            new QMSingleButton(PrisonEscapeMenu, 3, 2, "Get P90s", delegate
            {
                ModulesFunctions.GetP90PE();
            }, "This Tp's The P90 To Your Feet", true);

            new QMSingleButton(PrisonEscapeMenu, 4, 2, "Get M4A1s", delegate
            {
                ModulesFunctions.GetM4A1PE();
            }, "This Tp's The M4A1 To Your Feet", true);

            #endregion

            #region Misc

            new QMToggleButton(WorldCheats, 2.5f, 2.5f, "Pretend\nWorld Owner", delegate
            {
                ModulesFunctions.FWO = true;
                ModulesFunctions.ForceWorldOwner().Start();
            }, delegate
            {
                ModulesFunctions.FWO = false;
            }, "Toggle: This Will Make You Act Like The World Owner To Posably Give You Acsess To VIP Rooms <color=red>(This Will Change Your Nmaeplate Name)</color>");

            #endregion

            #endregion

            #endregion

            #region Protection

            #region Page 1

            new QMToggleButton(Protection, 1, 0, "AntiDesync", delegate
            {
                Config.Main.AntiDesync = true;
            }, delegate
            {
                Config.Main.AntiDesync = false;
            }, "Toggle: Stops You From Getting Desynced", Config.Main.AntiDesync);

            new QMToggleButton(Protection, 2, 0, "AntiMasterDC", delegate
            {
                Config.Main.AntiMasterDc = true;
            }, delegate
            {
                Config.Main.AntiMasterDc = false;
            }, "Toggle: This Makes It So Your Cant Get DC'ed As Master", Config.Main.AntiMasterDc);

            new QMToggleButton(Protection, 3, 0, "AntiItemOrbit", delegate
            {
                Config.Main.AntiItemOrbit = true;
            }, delegate
            {
                Config.Main.AntiItemOrbit = false;
            }, "Toggle: Stops ItemOrbiting For Others And You (BETA) <color=red>(WARNING: Will Stop Players From Picking Up Items)</color>", Config.Main.AntiItemOrbit);

            new QMToggleButton(Protection, 4, 0, "AntiLockInstance", delegate
            {
                Config.Main.AntiLockInstance = true;
            }, delegate
            {
                Config.Main.AntiLockInstance = false;
            }, "Toggle: Stops Instance Locks From Master Of Lobby", Config.Main.AntiLockInstance);

            new QMToggleButton(Protection, 1, 1, "RPCBlock", delegate
            {
                Config.Main.RPCBlock = true;
            }, delegate
            {
                Config.Main.RPCBlock = false;
            }, "Toggle: Stops All RPC's Thats Not Needed", Config.Main.RPCBlock);

            new QMToggleButton(Protection, 2, 1, "AntiWorld\nTriggers", delegate
            {
                Config.Main.AntiWorldTriggers = true;
            }, delegate
            {
                Config.Main.AntiWorldTriggers = false;
            }, "Toggle: This Makes It So WorldTriggers Cant Be Used", Config.Main.AntiWorldTriggers);

            new QMToggleButton(Protection, 3, 1, "AntiUdon", delegate
            {
                Config.Main.AntiUdon = true;
            }, delegate
            {
                Config.Main.AntiUdon = false;
            }, "Toggle: This Makes It So Udon Exploits And Udon Cant Be Used", Config.Main.AntiUdon);

            new QMToggleButton(Protection, 4, 1, "AntiCamCrash", delegate
            {
                Config.Main.AntiCamCrash = true;
            }, delegate
            {
                Config.Main.AntiCamCrash = false;
            }, "Toggle: This Makes It So Camera Crash Cant Be Used", Config.Main.AntiCamCrash);

            new QMToggleButton(Protection, 1, 2, "AntiWarns", delegate
            {
                Config.Main.AntiWarns = true;
            }, delegate
            {
                Config.Main.AntiWarns = false;
            }, "Toggle: This Makes It So You Cant Get Warns From Owner Of The Instance", Config.Main.AntiWarns);

            new QMToggleButton(Protection, 2, 2, "AntiMicOff", delegate
            {
                Config.Main.AntiMicOff = true;
            }, delegate
            {
                Config.Main.AntiMicOff = false;
            }, "Toggle: This Makes It So You Cant Get Miced Off From Owner Of The Instance", Config.Main.AntiMicOff);

            new QMToggleButton(Protection, 3, 2, "AntiBlock", delegate
            {
                Config.Main.AntiBlock = true;
            }, delegate
            {
                Config.Main.AntiBlock = false;
            }, "Toggle: This Makes It So You See The People Who Block You", Config.Main.AntiBlock);

            new QMToggleButton(Protection, 4, 2, "EventLimiter", delegate
            {
                Config.Main.EventLimit = true;
            }, delegate
            {
                Config.Main.EventLimit = false;
            }, "Toggle: This Makes It So You Cant Get Killed By Event Exploits (W.I.P)", Config.Main.EventLimit);

            new QMToggleButton(Protection, 1, 3, "AntiPhoton\nBots", delegate
            {
                Config.Main.AntiPhotonBots = true;
            }, delegate
            {
                Config.Main.AntiPhotonBots = false;
            }, "Toggle: This Makes It So If A Photon Bot Is Detected The Photon Bots Get Destroyed\nStoping Avi Crashes From Photon Bots And Annoying Audio Shit", Config.Main.AntiPhotonBots);

            new QMToggleButton(Protection, 2, 3, "AntiIKCrash", delegate
            {
                Config.Main.AntiIKCrash = true;
            }, delegate
            {
                Config.Main.AntiIKCrash = false;
            }, "Toggle: This Makes It So You Cant Get Crashed By IKCrashers", Config.Main.AntiIKCrash);

            new QMToggleButton(Protection, 3, 3, "AntiPortal", delegate
            {
                Config.Main.AntiPortal = true;
            }, delegate
            {
                Config.Main.AntiPortal = false;
            }, "Toggle: Stops you from going into portals", Config.Main.AntiPortal);

            #endregion

            #region Page 2

            var Page2Protection = new QMNestedButton(Protection, 4, 3, "Page 2", "Page 2 Of Protection's", "FC - Protection");

            new QMToggleButton(Page2Protection, 1, 0, "Anti Broken\nUspeak", delegate
            {
                Config.Main.AntiBrokenUspeak = true;
            }, delegate
            {
                Config.Main.AntiBrokenUspeak = false;
            }, "Toggle: Stops You From Getting EarRaped By The BrokenUspeak Exploit", Config.Main.AntiBrokenUspeak);

            new QMToggleButton(Page2Protection, 2, 0, "VideoPlayer\nProtection", delegate
            {
                Config.Main.VideoPlayerProtection = true;
            }, delegate
            {
                Config.Main.VideoPlayerProtection = false;
            }, "Toggle: Stops You From Getting Ip Grabbed By VideoPlayers", Config.Main.VideoPlayerProtection);

            new QMToggleButton(Page2Protection, 3, 0, "Anti\nEmoji Crash", delegate
            {
                Config.Main.AntiRPCCrash = true;
            }, delegate
            {
                Config.Main.AntiRPCCrash = false;
            }, "Toggle: This Makes It So Your Cant Get DC'ed By Shitty Emojis", Config.Main.AntiRPCCrash);

            new QMSingleButton(Page2Protection, 3, 0, "Anti\nPlayer Orbit", delegate
            {
                MelonCoroutines.Start(ModulesFunctions.AntiPlayerOrbit(true));
            }, "Anti Player Orbit This Makes It So The Player Who Is Orbiting You Has To Restart There Game (When Done It Will Tp You Back To Your POS)");


            #endregion

            #endregion

            #region Exploits

            #region Page 1

            new QMToggleButton(Exploits, 1, 0, "World Triggers", delegate
            {
                Config.Main.WorldTriggers = true;
            }, delegate
            {
                Config.Main.WorldTriggers = false;
            }, "Toggle: World Triggers", Config.Main.WorldTriggers);

            new QMToggleButton(Exploits, 2, 0, "ColliderHider", delegate
            {
                ModulesFunctions.ColliderHider(true);
            }, delegate
            {
                ModulesFunctions.ColliderHider(false);
            }, "Toggle: ColliderHider Will Hide Your Colider On Your Body Making It So You Cant Be Clicked \n<color=red>MAKE SURE YOU KEEP FLY ON </color>");

            new QMToggleButton(Exploits, 3, 0, "Inf Portals", delegate
            {
                Config.Main.InfPortals = false;
            }, delegate
            {
                Config.Main.InfPortals = true;
            }, "Toggle: This Will Make It So You Can Spawn Inf Portals", !Config.Main.InfPortals);

            new QMToggleButton(Exploits, 4, 0, "ItemLagger", delegate
            {
                ModulesFunctions.ItemLaggerBool = true;
                MelonCoroutines.Start(ModulesFunctions.ItemLagger());
            }, delegate
            {
                ModulesFunctions.ItemLaggerBool = false;
                ModulesFunctions.RespawnPickups();
            }, "Toggle: This Will Lag Worlds With More Then 15 items In It <color=red>(WARNING: Will Lag You)</color>");

            Swastika = new QMToggleButton(Exploits, 1, 1, "Swastika", delegate
            {
                ModulesFunctions.SwastikaBool = true;
                ModulesFunctions.PositionToGoTo = PlayerUtils.GetCurrentUser().field_Private_VRCPlayerApi_0.GetBonePosition(HumanBodyBones.Head);
            }, delegate
            {
                ModulesFunctions.SwastikaBool = false;
                ModulesFunctions.PositionToGoTo = Vector3.negativeInfinity;
                ModulesFunctions.RespawnPickups();
            }, "Toggle: This Will Spawn A Swastika");

            Serialization = new QMToggleButton(Exploits, 2, 1, "Serialization", delegate
            {
                ModulesFunctions.SerializationM(true);
            }, delegate
            {
                ModulesFunctions.SerializationM(false);
            }, "Toggle: Freeze your body for everyone but yourself");


            new QMToggleButton(Exploits, 3, 1, "Blink", delegate
            {
                ModulesFunctions.Blink = true;
                MelonCoroutines.Start(ModulesFunctions.BlinkMeth());
            }, delegate
            {
                ModulesFunctions.Blink = false;
                ModulesFunctions.Serialization = false;
            }, "Toggle: Makes You Look Like Your Lagging");

            new QMToggleButton(Exploits, 4, 1, "Emoji Spam", delegate
            {
                ModulesFunctions.EmojiSpamBool = true;
                MelonCoroutines.Start(ModulesFunctions.EmojiSpam());
            }, delegate
            {
                ModulesFunctions.EmojiSpamBool = false;
                MelonCoroutines.Start(ModulesFunctions.EmojiSpam());
            }, "Toggle: This Will Spam Random Emojis");

            BrokenUspeak = new QMToggleButton(Exploits, 1, 2, "Broken Uspeak", delegate
            {
                ModulesFunctions.earrapeExploitRunning = true;
                MelonCoroutines.Start(ModulesFunctions.EarrapeExploit());
            }, delegate
            {
                ModulesFunctions.earrapeExploitRunning = false;
                MelonCoroutines.Start(ModulesFunctions.EarrapeExploit());
            }, "Toggle: This Will EarRape Others With Broken Uspeak Bytes <color=red>(WARNING: Can Get You Ban)</color>");

            ItemLock = new QMToggleButton(Exploits, 2, 2, "ItemLock", delegate
            {
                ModulesFunctions.ItemLockBool = true;
            }, delegate
            {
                ModulesFunctions.ItemLockBool = false;
            }, "Toggle: Stops People From Picking Up Items");

            new QMToggleButton(Exploits, 3, 2, "InstanceLock", delegate
            {
                Config.Main.InstanceLock = true;
            }, delegate
            {
                Config.Main.InstanceLock = false;
            }, "Toggle: This Will Lock The Instance <color=red>(Only Works If Your Master)</color>");

            new QMToggleButton(Exploits, 4, 2, "NukeUdon", delegate
            {
                ModulesFunctions.killudon = true;
                MelonCoroutines.Start(ModulesFunctions.KillUdon());
            }, delegate
            {
                ModulesFunctions.killudon = false;
            }, "Toggle: This Will Toggle All Udon Events In A World Making It Laggy And Unusable");

            new QMSingleButton(Exploits, 1, 3, "AviClap", delegate
            {
                ModulesFunctions.WorldClap(Config.Main.AviClapID, 15);
            }, "Toggle: Avi Clap", true);

            new QMSingleButton(Exploits, 1, 3.5f, "Quest Crash", delegate
            {
                ModulesFunctions.WorldClap(Config.Main.QuestCrashID, 15);
            }, "Toggle: Quest Crash", true);

            new QMSingleButton(Exploits, 2, 3, "Advertise Portal", delegate
            {
                ModulesFunctions.CoolThing();
            }, "Drops An Advertise Portal For FusionClient", true);

            new QMSingleButton(Exploits, 2, 3.5f, "Ip Logger Portal", delegate
            {
                Process.Start("https://iplogger.org/logger/F6rd3q6ezxcK");
                MiscUtils.DropPortal("wrld_09d08b37-57be-438e-b529-40d6870f58e3", $"8813", 13, PlayerUtils.GetCurrentUser().transform.position + PlayerUtils.GetCurrentUser().transform.forward * 2.1f, PlayerUtils.GetCurrentUser().transform.rotation);
            }, "Drops A Portal To A IP Logging World A Window Will Popup In Chrome And It Will Log There Ip There", true);

            new QMSingleButton(Exploits, 3, 3f, "VidPlayer EarRape", delegate
            {
                var url = Config.Main.VideoPlayerURL;
                ModulesFunctions.OverrideVideoPlayers(url);
            }, "EarRape's users with VidPlayer\n<color=#ff0000>(Works In Some SDK3 Worlds)</color>", true);

            new QMSingleButton(Exploits, 3, 3.5f, "PenCrash", delegate
            {
                ModulesFunctions.PenCrash();
            }, "If There Is (VRCSDK2) Pens From Vrc In The World It Will Use Them To Crash EveryOne", true);

            #endregion

            #region Page 2

            var Page2Exploits = new QMNestedButton(Exploits, 4, 3, "Page 2", "Page 2 Of Exploits's", "FC - Exploits");

            new QMToggleButton(Page2Exploits, 1, 0, "Spam\nWindows Sound", delegate
            {
                ModulesFunctions.WindowsSound = true;
            }, delegate
            {
                ModulesFunctions.WindowsSound = false;
            }, "Toggle: This Spams the windows error noise for people using clients like Teo and Notorious");

            InvisJoin = new QMToggleButton(Page2Exploits, 2, 0, "Invisible\nJoin", delegate
            {
                Config.Main.InvisibleJoin = true;
            }, delegate
            {
                Config.Main.InvisibleJoin = false;
            }, "Toggle: This Will Make You Join The Lobby Invisible To Players <color=red>(Master Can See You)</color>");

            FreeFall = new QMToggleButton(Page2Exploits, 3, 0, "Free Fall", delegate
            {
                ModulesFunctions.FreeFallBool = true;
                MelonCoroutines.Start(ModulesFunctions.FreeFallLoop());
            }, delegate
            {
                ModulesFunctions.FreeFallBool = false;
                ModulesFunctions.RespawnPickups();
            }, "Toggle: This Works In Some Worlds You Just Have To Test");

            MirrorSpam = new QMToggleButton(Page2Exploits, 4, 0, "Spam Mirrors", delegate
            {
                ModulesFunctions.SpamMirrorsBool = true;
                MelonCoroutines.Start(ModulesFunctions.SpamMirrors());
            }, delegate
            {
                ModulesFunctions.SpamMirrorsBool = false;
            }, "Toggle: This Will Spam Mirrors In The World (Only Works In Some World's Just Test It)");

            new QMToggleButton(Page2Exploits, 1, 1, "Free Cam", delegate
            {
                ModulesFunctions.FreeCam(true);
            }, delegate
            {
                ModulesFunctions.FreeCam(false);
            }, "Toggle: This Will Freeze You In Place And Let You Fly Around Spying On Users");

            new QMToggleButton(Page2Exploits, 2, 1, "CamAnnoy", delegate
            {
                ModulesFunctions.IsFlashing = true;
                MelonCoroutines.Start(ModulesFunctions.CameraAnn(PlayerUtils.GetCurrentUser()._player));
            }, delegate
            {
                ModulesFunctions.IsFlashing = false;
            }, "Toggle: This Will Spam An Annoying Ass Sound By The Camera");

            new QMToggleButton(Page2Exploits, 3, 1, "Force World Master", delegate
            {
                ModulesFunctions.FWM = true;
                ModulesFunctions.ForceWorldMaster().Start();
            }, delegate
            {
                ModulesFunctions.FWM = false;
            }, "Toggle: This Will Make You Act Like The World Master To Possibly Give You Access To Video Player's And Other World Master Stuff  <color=red>(This Will Change Your Nmaeplate Name)</color>");

            new QMSingleButton(Page2Exploits, 1.5f, 3, "Draw A Tornado", delegate
            {
                ModulesFunctions.PosLocal = PlayerUtils.GetCurrentUser().gameObject.transform.position;
                MelonCoroutines.Start(ModulesFunctions.Tornado());
            }, "If There Is Pens In The World It Will Draw A Tornado @ Youre POS", true);

            #endregion

            #endregion

            #region Visual

            new QMToggleButton(Visual, 1, 0, "PlayerCapsule\nESP", delegate
            {
                ModulesFunctions.PlayerCapsuleESP = true;
                ModulesFunctions.PlayerESP();
            }, delegate
            {
                ModulesFunctions.PlayerCapsuleESP = false;
                ModulesFunctions.PlayerESP();
            }, "Toggle: This Is To Make Players capsule's Have ESP");

            new QMToggleButton(Visual, 2, 0, "PlayersMesh\nESP", delegate
            {
                foreach (var Player in WorldUtils.GetPlayers())
                {
                    ModulesFunctions.PlayerMeshEsp(Player, true);
                }
                ModulesFunctions.PlayersMeshESP = true;
            }, delegate
            {
                foreach (var Player in WorldUtils.GetPlayers())
                {
                    ModulesFunctions.PlayerMeshEsp(Player, false);
                }
                ModulesFunctions.PlayersMeshESP = false;
            }, "Toggle: This Is To Make Players Mesh Have ESP");

            new QMToggleButton(Visual, 3, 0, "AviDistance\nHider", delegate
            {
                Config.Main.AvatarHider = true;
                AviDistanceHide.UnHideAvatars();
            }, delegate
            {
                Config.Main.AvatarHider = false;
                AviDistanceHide.UnHideAvatars();
            }, "This Will Increase FPS By Hiding Avatars By Distance", Config.Main.AvatarHider);

            #endregion

            #region IKTweaks

            new QMToggleButton(IKTweaks, 1, 0, "LeftHandUp", delegate
            {
                IKTweeks.LeftHandUp = true;
            }, delegate
            {
                IKTweeks.LeftHandUp = false;
            }, "Toggle: Makes You Go Put Your LeftHandUp");

            new QMToggleButton(IKTweaks, 2, 0, "RightHandUp", delegate
            {
                IKTweeks.RightHandUp = true;
            }, delegate
            {
                IKTweeks.RightHandUp = false;
            }, "Toggle: Makes You Go Put Your RightHandUp");

            new QMToggleButton(IKTweaks, 3, 0, "TwistHead", delegate
            {
                IKTweeks.TwistHead = true;
            }, delegate
            {
                IKTweeks.TwistHead = false;
            }, "Toggle: This Makes You Look Like Your Flying Away");

            new QMToggleButton(IKTweaks, 4, 0, "Slingy", delegate
            {
                IKTweeks.slingy = true;
            }, delegate
            {
                IKTweeks.slingy = false;
            }, "Toggle: Makes You Into A Slingy");

            new QMToggleButton(IKTweaks, 1, 1, "BreakBones", delegate
            {
                IKTweeks.BrakeBones = true;
            }, delegate
            {
                IKTweeks.BrakeBones = false;
            }, "Toggle: BreakBones");

            #endregion

            #region Bots

            new QMSingleButton(Bots, 1, 0.5f, "Start Bots", delegate
            {
                if (!BotServer.Started)
                {
                    BServer = new BotServer();
                }
                if (File.Exists(Directory.GetCurrentDirectory() + "\\Fusion Client\\profiles.txt"))
                {
                    var profiles = File.ReadAllText(Directory.GetCurrentDirectory() + "\\Fusion Client\\profiles.txt").Split(',');
                    //SocketConnection.StartServer();
                    profiles.ToList().ForEach(p =>
                    {
                        try
                        {
                            var BotCount = System.Convert.ToInt32(p);
                            BotModule.StartBot(BotCount);
                            Logs.Log($"Starting Bot: {BotCount}", ConsoleColor.Cyan);
                        }
                        catch
                        {

                        }
                    });
                }
                else
                {
                    PopupUtils.InputPopup("Confirm", "How Many Bots Do You Want", delegate (string s)
                    {
                        File.WriteAllText("Fusion Client\\profiles.txt", s);
                        var profiles = File.ReadAllText(Directory.GetCurrentDirectory() + "\\Fusion Client\\profiles.txt").Split(',');
                        //SocketConnection.StartServer();
                        profiles.ToList().ForEach(p =>
                        {
                            try
                            {
                                var BotCount = System.Convert.ToInt32(p);
                                BotModule.StartBot(BotCount);
                                Thread.Sleep(2000);
                                Logs.Log($"Starting Bot: {BotCount}", ConsoleColor.Cyan);
                            }
                            catch
                            {

                            }
                        });
                    });
                }
            }, "Toggle: Start Bots");

            new QMSingleButton(Bots, 2, 0.5f, "Stop Bots", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.EXIT));
            }, "Toggle: Stop Bots");

            new QMSingleButton(Bots, 3, 0.5f, "Join Me", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.JOIN_WORLD, RoomManager.field_Internal_Static_ApiWorldInstance_0.id));
            }, "Toggle: Join Me");

            new QMSingleButton(Bots, 4, 0.5f, "Join WorldID", delegate
            {
                PopupUtils.InputPopup("Confirm", "Wrld_0000_00000_0000", delegate (string s)
                {
                    BotServer.SendAll(new PacketData(PacketBotServerType.JOIN_WORLD, s));
                });
            }, "Toggle: Join Me");

            #region Bot Funk

            var BotFunctions = new QMNestedButton(Bots, 1.5f, 2.5f, "Bot Functions", "Bot Functions", "Bot Functions");
            var BotMovment = new QMNestedButton(Bots, 2.5f, 2.5f, "Bot Movment", "Bot Movment", "Bot Movment");
            var BotSettings = new QMNestedButton(Bots, 3.5f, 2.5f, "Bot Settings", "Bot Movment", "Bot Movment");

            #region BotFunctions

            new QMSingleButton(BotFunctions, 1, 0, "Change Avatar", delegate
            {
                PopupUtils.InputPopup("Confirm", "Avtr_00000_00000_0000", delegate (string a)
                {
                    BotServer.SendAll(new PacketData(PacketBotServerType.CHANGE_AVATAR, a));
                });
            }, "Change Bots Avis");

            new QMSingleButton(BotFunctions, 2, 0f, "Mimic Self", delegate
            {
                BotModule.FollowSomeone = true;
                BotModule.FollowTarget = PlayerUtils.GetCurrentUser().GetPlayer();
                BotServer.SendAll(new PacketData(PacketBotServerType.FOLLOW, PlayerUtils.GetCurrentUser().GetAPIUser().id));
            }, "This Will Make The Bots Follow You Around");

            new QMSingleButton(BotFunctions, 3, 0, "Stop\nMimic/Orbit", delegate
            {
                BotModule.FollowSomeone = false;
                BotModule.FollowTarget = null;
                BotServer.SendAll(new PacketData(PacketBotServerType.STOP_FOLLOW));
            }, "This Will Stop The Bots From Following You Or Anyone You Targeted");

            new QMSingleButton(BotFunctions, 4, 0f, "Orbit Self", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.ORBIT, PlayerUtils.GetCurrentUser().GetUserID()));
            }, "This Will Make The Bots Follow Orbit You");

            new QMSingleButton(BotFunctions, 2f, 1, "Mute /\nUnmute", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.UNMUTE));
            }, "This Will Mute/Unmute Your Bots");

            new QMSingleButton(BotFunctions, 3f, 1, "Realign", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.ALIGN_CHANGE));
            }, "This Will Realign The Bots In Mimic Mode");

            new QMToggleButton(BotFunctions, 1, 2, "Spin Bots", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.SPINBOT_TOGGLE, "true"));
            }, delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.SPINBOT_TOGGLE, string.Empty));
            }, "Toggle: ");

            new QMToggleButton(BotFunctions, 2, 2, "Event 6", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.E6, "true"));
            }, delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.E6, "false"));
            }, "Toggle: This Will Spam Players With Event 6 <color=red>(WARNING: Can Get You Ban)</color>");

            new QMToggleButton(BotFunctions, 3, 2, "Broken Uspeak", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.E1, "true"));
            }, delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.E1, "false"));
            }, "Toggle: This Will EarRape Others With Broken Uspeak Bytes <color=red>(WARNING: Can Get You Ban)</color>");

            new QMToggleButton(BotFunctions, 4, 2, "Master DC", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.RPC_DC_MASTER, "true"));
                Config.Main.AntiDesync = true;
                Config.Main.AntiMasterDc = true;
            }, delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.RPC_DC_MASTER, "false"));
                Config.Main.AntiDesync = false;
                Config.Main.AntiMasterDc = false;
            }, "Toggle: Disconnect the master of the room when a new player joins the world. <color=red>(WARNING: Can Get You Ban)</color>");

            new QMToggleButton(BotFunctions, 2.5f, 3, "Voice Mimic", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.E1_MIMIC, PlayerUtils.GetCurrentUser().GetAPIUser().id));
            }, delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.E1_MIMIC, $""));
            }, "Toggle: This Is To Mimic The Voice Of The User");

            #endregion

            #region Movement

            var BotAllMovment = new QMNestedButton(BotMovment, 1.5f, 1.5f, "All\nMovment", "All Movment", "All Movment");
            BotOneMovment = new QMNestedButton(BotMovment, 2.5f, 1.5f, "Individual\nMovment", "Individual Movment", "Individual Movment");
            var BotPlacement = new QMNestedButton(BotMovment, 3.5f, 1.5f, "Bot\nPlacement", "Bot Placement", "Bot Placement");

            #region all Movement

            new QMSingleButton(BotAllMovment, 1, 1, "Move Right", delegate
            {
                BotServer.SendClient(BotTarget, new PacketData(PacketBotServerType.MOVEMENT, Newtonsoft.Json.JsonConvert.SerializeObject(new Movement(Fusion.Networking.Movement.MovementInfo.Right, Fusion.Networking.Movement.MovementInfo2.Null))));
            }, "Change Bots POS");

            new QMSingleButton(BotAllMovment, 2, 1, "Move Left", delegate
            {
                BotServer.SendClient(BotTarget, new PacketData(PacketBotServerType.MOVEMENT, Newtonsoft.Json.JsonConvert.SerializeObject(new Movement(Fusion.Networking.Movement.MovementInfo.Left, Fusion.Networking.Movement.MovementInfo2.Null))));
            }, "Change Bots POS");

            new QMSingleButton(BotAllMovment, 3, 1, "Move Forword", delegate
            {
                BotServer.SendClient(BotTarget, new PacketData(PacketBotServerType.MOVEMENT, Newtonsoft.Json.JsonConvert.SerializeObject(new Movement(Fusion.Networking.Movement.MovementInfo.Forword, Fusion.Networking.Movement.MovementInfo2.Null))));
            }, "Change Bots POS");

            new QMSingleButton(BotAllMovment, 4, 1, "Move Backwards", delegate
            {
                BotServer.SendClient(BotTarget, new PacketData(PacketBotServerType.MOVEMENT, Newtonsoft.Json.JsonConvert.SerializeObject(new Movement(Fusion.Networking.Movement.MovementInfo.Backwards, Fusion.Networking.Movement.MovementInfo2.Null))));
            }, "Change Bots POS");

            new QMSingleButton(BotAllMovment, 1, 2, "Tp To You", delegate
            {
                BotServer.SendClient(BotTarget, new PacketData(PacketBotServerType.TP, $"{PlayerUtils.GetCurrentUser().GetUserID()}"));
            }, "Change Bots POS");

            new QMSingleButton(BotAllMovment, 2, 2, "Mute /\nUnmute", delegate
            {
                BotServer.SendClient(BotTarget, new PacketData(PacketBotServerType.UNMUTE));
            }, "Change Mute/Unmute");

            new QMSingleButton(BotAllMovment, 3, 2, "Join Me", delegate
            {
                BotServer.SendClient(BotTarget, new PacketData(PacketBotServerType.JOIN_WORLD, RoomManager.field_Internal_Static_ApiWorldInstance_0.id));
            }, "Change Mute/Unmute");

            new QMSingleButton(BotAllMovment, 4, 2, "Change Avatar", delegate
            {
                PopupUtils.InputPopup("Confirm", "Avtr_00000_00000_0000", delegate (string a)
                {
                    BotServer.SendClient(BotTarget, new PacketData(PacketBotServerType.CHANGE_AVATAR, a));
                });
            }, "Change Bot Avis");

            #endregion

            #region Individual Movement

            new QMSingleButton(BotOneMovment, 1, 1, "Move Left", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.MOVEMENT, Newtonsoft.Json.JsonConvert.SerializeObject(new Movement(Fusion.Networking.Movement.MovementInfo.Left, Fusion.Networking.Movement.MovementInfo2.Null))));
            }, "Change Bots POS");
            new QMSingleButton(BotOneMovment, 2, 1, "Move Right", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.MOVEMENT, Newtonsoft.Json.JsonConvert.SerializeObject(new Movement(Fusion.Networking.Movement.MovementInfo.Right, Fusion.Networking.Movement.MovementInfo2.Null))));
            }, "Change Bots POS");
            new QMSingleButton(BotOneMovment, 3, 1, "Move Forword", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.MOVEMENT, Newtonsoft.Json.JsonConvert.SerializeObject(new Movement(Fusion.Networking.Movement.MovementInfo.Forword, Fusion.Networking.Movement.MovementInfo2.Null))));
            }, "Change Bots POS");
            new QMSingleButton(BotOneMovment, 4, 1, "Move Backwards", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.MOVEMENT, Newtonsoft.Json.JsonConvert.SerializeObject(new Movement(Fusion.Networking.Movement.MovementInfo.Backwards, Fusion.Networking.Movement.MovementInfo2.Null))));
            }, "Change Bots POS");

            new QMSingleButton(BotOneMovment, 2.5f, 2, "Tp To You", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.TP, $"{PlayerUtils.GetCurrentUser().GetUserID()}"));
            }, "Change Bots POS", true);

            new QMSingleButton(BotOneMovment, 1, 2.5f, "Back Left And Right", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.MOVEMENT, Newtonsoft.Json.JsonConvert.SerializeObject(new Movement(Fusion.Networking.Movement.MovementInfo.Null, Fusion.Networking.Movement.MovementInfo2.RightAndLeftBack))));
            }, "Change Bots POS", true);

            new QMSingleButton(BotOneMovment, 2, 2.5f, "LeftAndRight", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.MOVEMENT, Newtonsoft.Json.JsonConvert.SerializeObject(new Movement(Fusion.Networking.Movement.MovementInfo.Null, Fusion.Networking.Movement.MovementInfo2.LeftAndRight))));
            }, "Change Bots POS", true);

            new QMSingleButton(BotOneMovment, 3, 2.5f, "Front Left And Right", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.MOVEMENT, Newtonsoft.Json.JsonConvert.SerializeObject(new Movement(Fusion.Networking.Movement.MovementInfo.Null, Fusion.Networking.Movement.MovementInfo2.FrontLeftAndRight))));
            }, "Change Bots POS", true);

            new QMSingleButton(BotOneMovment, 4, 3f, "All LeftBack", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.MOVEMENT, Newtonsoft.Json.JsonConvert.SerializeObject(new Movement(Fusion.Networking.Movement.MovementInfo.Null, Fusion.Networking.Movement.MovementInfo2.AllLeftBack))));
            }, "Change Bots POS", true);

            new QMSingleButton(BotOneMovment, 1, 3f, "All RightBack", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.MOVEMENT, Newtonsoft.Json.JsonConvert.SerializeObject(new Movement(Fusion.Networking.Movement.MovementInfo.Null, Fusion.Networking.Movement.MovementInfo2.AllRightBack))));
            }, "Change Bots POS", true);

            new QMSingleButton(BotOneMovment, 2, 3f, "Zigzag", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.MOVEMENT, Newtonsoft.Json.JsonConvert.SerializeObject(new Movement(Fusion.Networking.Movement.MovementInfo.Null, Fusion.Networking.Movement.MovementInfo2.Zigzag))));
            }, "Change Bots POS", true);

            new QMSingleButton(BotOneMovment, 3, 3f, "Line", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.MOVEMENT, Newtonsoft.Json.JsonConvert.SerializeObject(new Movement(Fusion.Networking.Movement.MovementInfo.Null, Fusion.Networking.Movement.MovementInfo2.Line))));
            }, "Change Bots POS", true);

            new QMSingleButton(BotOneMovment, 4, 2.5f, "FrontLine", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.MOVEMENT, Newtonsoft.Json.JsonConvert.SerializeObject(new Movement(Fusion.Networking.Movement.MovementInfo.Null, Fusion.Networking.Movement.MovementInfo2.FrontLine))));
            }, "Change Bots POS", true);

            new QMSingleButton(BotOneMovment, 1, 3.5f, "LineBack", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.MOVEMENT, Newtonsoft.Json.JsonConvert.SerializeObject(new Movement(Fusion.Networking.Movement.MovementInfo.Null, Fusion.Networking.Movement.MovementInfo2.LineBack))));
            }, "Change Bots POS", true);

            new QMSingleButton(BotOneMovment, 2, 3.5f, "LineRight", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.MOVEMENT, Newtonsoft.Json.JsonConvert.SerializeObject(new Movement(Fusion.Networking.Movement.MovementInfo.Null, Fusion.Networking.Movement.MovementInfo2.LineLeft))));
            }, "Change Bots POS", true);

            new QMSingleButton(BotOneMovment, 3, 3.5f, "LineLeft", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.MOVEMENT, Newtonsoft.Json.JsonConvert.SerializeObject(new Movement(Fusion.Networking.Movement.MovementInfo.Null, Fusion.Networking.Movement.MovementInfo2.LineRight))));
            }, "Change Bots POS", true);

            new QMSingleButton(BotOneMovment, 4, 3.5f, "HeadStack", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.MOVEMENT, Newtonsoft.Json.JsonConvert.SerializeObject(new Movement(Fusion.Networking.Movement.MovementInfo.Null, Fusion.Networking.Movement.MovementInfo2.HeadStack))));
            }, "Change Bots POS", true);

            #endregion

            #region Bot Placement

            new QMToggleButton(BotPlacement, 1, 0, "Midnight Rooftop\nTop", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.PLACEMENT, $"Midnight Top"));
            }, delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.PLACEMENT, $"Reset"));
            }, "Toggle: This Will Tp Bots To The Placement");

            new QMToggleButton(BotPlacement, 2, 0, "Midnight Rooftop\nBottom", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.PLACEMENT, $"Midnight Bottom"));
            }, delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.PLACEMENT, $"Reset"));
            }, "Toggle: This Will Tp Bots To The Placement");

            new QMToggleButton(BotPlacement, 3, 0, "BlackCat\nMain", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.PLACEMENT, $"BlackCat Main"));
            }, delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.PLACEMENT, $"Reset"));
            }, "Toggle: This Will Tp Bots To The Placement");

            new QMToggleButton(BotPlacement, 4, 0, "BlackCat\nTop", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.PLACEMENT, $"BlackCat Top"));
            }, delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.PLACEMENT, $"Reset"));
            }, "Toggle: This Will Tp Bots To The Placement");

            new QMToggleButton(BotPlacement, 1, 1, "Room Of Rain", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.PLACEMENT, $"Rain"));
            }, delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.PLACEMENT, $"Reset"));
            }, "Toggle: This Will Tp Bots To The Placement");

            new QMToggleButton(BotPlacement, 2, 1, "Serenity\nSleepOver", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.PLACEMENT, $"Rain"));
            }, delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.PLACEMENT, $"Reset"));
            }, "Toggle: This Will Tp Bots To The Placement");

            #endregion

            #endregion

            #region Bot Settings

            new QMToggleButton(BotSettings, 1, 0, "Debug Mode", delegate
            {
                Config.Main.DebugMode = true;
            }, delegate
            {
                Config.Main.DebugMode = false;
            }, "Toggle: This Is If You Want Vrchat's Main Window To Show AKA (Console Or No Console)", Config.Main.DebugMode);

            new QMToggleButton(BotSettings, 2, 0, "Bot Events", delegate
            {
                Config.Main.LogBotEvents = true;
            }, delegate
            {
                Config.Main.LogBotEvents = false;
            }, "Toggle: This Will Log Bot Events Like JOIN,LEAVE,JOINING WORLD And So On", Config.Main.LogBotEvents);

            new QMToggleButton(BotSettings, 3, 0, "Auto Join", delegate
            {
                Config.Main.AutoJoin = true;
            }, delegate
            {
                Config.Main.AutoJoin = false;
            }, "Toggle: This Will Make The Bots Auto Join You On Load Of A World", Config.Main.AutoJoin);

            new QMToggleButton(BotSettings, 4, 0, "Auto Start\nBot Server", delegate
            {
                Config.Main.AutoStartBotManagerServer = true;
            }, delegate
            {
                Config.Main.AutoStartBotManagerServer = false;
            }, "Toggle: This Will Make It So You Can Auto Connect Back To Your Bots Without Restarting Your Bots (This Is Good If You Crash And Dont Want To Restart Your Bots) I Would Keep This On", Config.Main.AutoStartBotManagerServer);

            new QMSingleButton(BotSettings, 1, 1, "Orbit Speed", delegate
            {
                PopupUtils.InputPopup("Confirm", "Defalt Speed Is 1", delegate (string s)
                {
                    float value = float.Parse(s, CultureInfo.InvariantCulture.NumberFormat);
                    BotServer.SendAll(new PacketData(PacketBotServerType.EDIT_VALUES, Newtonsoft.Json.JsonConvert.SerializeObject(new EditValues(value, 0, 0))));
                });
            }, "Toggle: This Is For Your Orbit Speed");

            new QMSingleButton(BotSettings, 2, 1, "Bot Distance", delegate
            {
                PopupUtils.InputPopup("Confirm", "Defalt Distance Is 1", delegate (string s)
                {
                    float value = float.Parse(s, CultureInfo.InvariantCulture.NumberFormat);
                    BotServer.SendAll(new PacketData(PacketBotServerType.EDIT_VALUES, Newtonsoft.Json.JsonConvert.SerializeObject(new EditValues(0, value, 0))));
                });
            }, "Toggle: This Is For Your Bot's Distance When They Orbit You And Mimic You Or Anyone");

            new QMSingleButton(BotSettings, 3, 1, "Spinbot Speed", delegate
            {
                PopupUtils.InputPopup("Confirm", "Defalt Speed Is 1", delegate (string s)
                {
                    int value = int.Parse(s, CultureInfo.InvariantCulture.NumberFormat);
                    BotServer.SendAll(new PacketData(PacketBotServerType.EDIT_VALUES, Newtonsoft.Json.JsonConvert.SerializeObject(new EditValues(0, 0, value))));
                });
            }, "Toggle: This Is For Your Bot's Spin Speed On Spin Bot");

            new QMSingleButton(BotSettings, 4, 1, "Bot Count", delegate
            {
                PopupUtils.InputPopup("Confirm", "How Many Bots Do You Want", delegate (string s)
                {
                    File.WriteAllText("Fusion Client\\profiles.txt", s);
                });
            }, "Toggle: This Is To Know How Many Bot You Want To Start");

            var BotLogin = new QMNestedButton(BotSettings, 1, 2, "Bot\nLogin's", "Bot Login's", "Bot Login's");

            new QMSingleButton(BotLogin, 1, 1, "Bot 1", delegate
            {
                PopupUtils.InputPopup("Confirm", "EMAIL:PASSWORD", delegate (string s)
                {
                    File.WriteAllText("Fusion Client\\Misc\\BotLogins\\Bot1.txt", s);
                });
            }, "Toggle: This Is To Know The Bots Login");

            new QMSingleButton(BotLogin, 2, 1, "Bot 2", delegate
            {
                PopupUtils.InputPopup("Confirm", "EMAIL:PASSWORD", delegate (string s)
                {
                    File.WriteAllText("Fusion Client\\Misc\\BotLogins\\Bot2.txt", s);
                });
            }, "Toggle: This Is To Know The Bots Login");

            new QMSingleButton(BotLogin, 3, 1, "Bot 3", delegate
            {
                PopupUtils.InputPopup("Confirm", "EMAIL:PASSWORD", delegate (string s)
                {
                    File.WriteAllText("Fusion Client\\Misc\\BotLogins\\Bot3.txt", s);
                });
            }, "Toggle: This Is To Know The Bots Login");

            new QMSingleButton(BotLogin, 4, 1, "Bot 4", delegate
            {
                PopupUtils.InputPopup("Confirm", "EMAIL:PASSWORD", delegate (string s)
                {
                    File.WriteAllText("Fusion Client\\Misc\\BotLogins\\Bot4.txt", s);
                });
            }, "Toggle: This Is To Know The Bots Login");

            new QMSingleButton(BotLogin, 2.5f, 2, "Bot 5", delegate
            {
                PopupUtils.InputPopup("Confirm", "EMAIL:PASSWORD", delegate (string s)
                {
                    File.WriteAllText("Fusion Client\\Misc\\BotLogins\\Bot5.txt", s);
                });
            }, "Toggle: This Is To Know The Bots Login");

            #endregion

            #endregion

            #endregion

            #region Settings

            var SettingsMovement = new QMNestedButton(Settings, 1, 1, "Movement", "Movement Settings", "FC - Movement");
            var SettingsSelf = new QMNestedButton(Settings, 2, 1, "Self", "Self Settings", "FC - Self");
            var SettingsLogging = new QMNestedButton(Settings, 3, 1, "Logging", "Logging Settings", "FC - Logging");
            var SettingsUI = new QMNestedButton(Settings, 4, 1, "UI", "UI Settings", "FC - UI");
            var SettingsModeration = new QMNestedButton(Settings, 1.5f, 2, "Moderation", "Moderation Settings", "FC - Moderation");
            var SettingsMisc = new QMNestedButton(Settings, 2.5f, 2, "Misc", "Misc Settings", "FC - Misc");
            var SettingsOrbit = new QMNestedButton(Settings, 3.5f, 2, "Orbit", "Orbit Settings", "FC - Orbit");

            #region Movement

            new QMSingleButton(SettingsMovement, 1, 0, "Fly Speed\nValue", delegate
            {
                PopupUtils.InputPopup("Enter Value", "Default Value Is 6", delegate (string s)
                {
                    short Speed = Convert.ToInt16(s);

                    if (Speed > 1000)
                    {
                        Logs.Log("[Movement] You cannot set your fly speed more than 1000!", ConsoleColor.Red);
                        Logs.Hud("[Movement] You cannot set your fly speed more than 1000!");
                        return;
                    }
                    if (Speed < -0)
                    {
                        Logs.Log("[Movement] You cannot set your fly speed less than 0!", ConsoleColor.Red);
                        Logs.Hud("[Movement] You cannot set your fly speed less than 0!");
                        return;
                    }
                    Config.Main.FlightSpeed = Speed;
                });
            }, "Set what you want your fly speed to be");

            new QMSingleButton(SettingsMovement, 2, 0, "Speed\nValue", delegate
            {
                PopupUtils.InputPopup("Enter Value", "Default Value Is 16", delegate (string s)
                {
                    short Speed = Convert.ToInt16(s);

                    if (Speed > 1000)
                    {
                        Logs.Log("[Movement] You cannot set your speed more than 1000!", ConsoleColor.Red);
                        Logs.Hud("[Movement] You cannot set your speed more than 1000!");
                        return;
                    }
                    if (Speed < -0)
                    {
                        Logs.Log("[Movement] You cannot set your speed less than 0!", ConsoleColor.Red);
                        Logs.Hud("[Movement] You cannot set your speed less than 0!");
                        return;
                    }
                    Config.Main.SpeedHack = Speed;
                });
            }, "Set what you want your speed to be");

            new QMToggleButton(SettingsMovement, 3, 0, "KeyBinds", delegate
            {
                Config.Main.KeyBinds = true;
            }, delegate
            {
                Config.Main.KeyBinds = false;
            }, "Toggle: Turn on or off keybinds", Config.Main.KeyBinds);

            #endregion

            #region Self

            new QMSingleButton(SettingsSelf, 1, 0, "FPS Value\nValue", delegate
            {
                PopupUtils.InputPopup("Enter FPS Value", "Default Value Is 69", delegate (string s)
                {
                    short FPS = Convert.ToInt16(s);

                    if (FPS > 1337)
                    {
                        Logs.Log("[Self] You cannot set your FPS Value more than 1337!", ConsoleColor.Red);
                        Logs.Hud("[Self] You cannot set your FPS Value more than 1337!");
                        return;
                    }
                    if (FPS < -1337)
                    {
                        Logs.Log("[Self] You cannot set your FPS Value less than -1337!", ConsoleColor.Red);
                        Logs.Hud("[Self] You cannot set your FPS Value less than -1337!");
                        return;
                    }
                    Config.Main.FakeFramesValue = FPS;
                });
            }, "Set what you want your FPS to be");

            new QMToggleButton(SettingsSelf, 2, 0, "Real Fake Frames", delegate
            {
                Config.Main.FakeFramesReal = true;
            }, delegate
            {
                Config.Main.FakeFramesReal = false;
            }, "Toggle: Fake Frames That Look Real", Config.Main.FakeFramesReal);

            new QMSingleButton(SettingsSelf, 3, 0, "Ping Value\nValue", delegate
            {
                PopupUtils.InputPopup("Enter Ping Value", "Default Value Is 69 ;)", delegate (string s)
                {
                    short Ping = Convert.ToInt16(s);

                    if (Ping > 32767)
                    {
                        Logs.Log("[Self] You cannot set your Ping Value more than -32767!", ConsoleColor.Red);
                        Logs.Hud("[Self] You cannot set your Ping Value more than -32767!");
                        return;
                    }
                    if (Ping < -32768)
                    {
                        Logs.Log("[Self] You cannot set your Ping Value less than -32768!", ConsoleColor.Red);
                        Logs.Hud("[Self] You cannot set your Ping Value less than -32768!");
                        return;
                    }
                    Config.Main.FakePingValue = Ping;
                });
            }, "Set what you want your Ping to be");

            new QMToggleButton(SettingsSelf, 4, 0, "Real Fake Ping", delegate
            {
                Config.Main.FakePingReal = true;
            }, delegate
            {
                Config.Main.FakePingReal = false;
            }, "Toggle: Fake Ping That Look Real", Config.Main.FakePingReal);

            new QMSingleButton(SettingsSelf, 2.5f, 1, "Fake WorldID", delegate
            {
                PopupUtils.InputPopup("Enter WorldID", "wrld_0000_0000_000_00", delegate (string s)
                {
                    Config.Main.WorldID = s;
                });
            }, "Set what to WorldSpoof Id Is");

            #endregion

            #region Logging

            new QMToggleButton(SettingsLogging, 1, 0, "Avatar\nLogging", delegate
            {
                Config.Main.AvatarLogging = true;
            }, delegate
            {
                Config.Main.AvatarLogging = false;
            }, "Toggle: This Will Turn Off Or On The AvatarLogging <color=red>(This Dose Lag Your Game For Like 0.5secs When Users Join)</color>", Config.Main.AvatarLogging);

            new QMToggleButton(SettingsLogging, 2, 0, "PhotonLog", delegate
            {
                Config.Main.PhotonLog = true;
                //Config.Main.LogEvents = true;
                //PhotonOnEventHook.LogCurrentHookActivity = true;
            }, delegate
            {
                Config.Main.PhotonLog = false;
                //Config.Main.LogEvents = false;
                //PhotonOnEventHook.LogCurrentHookActivity = false;
            }, "Toggle: This Logs All Photon Events <color=red>(Since This Shit Logs All Photon Events It Will DropYour FPS To 10)</color>", Config.Main.PhotonLog);

            new QMToggleButton(SettingsLogging, 3, 0, "RPCLog", delegate
            {
                Config.Main.RPCLog = true;
            }, delegate
            {
                Config.Main.RPCLog = false;
            }, "Toggle: This Logs All Events And RPCS", Config.Main.RPCLog);

            new QMSingleButton(SettingsLogging, 4, 0, "DumpUdonEvents", delegate
            {
                MelonCoroutines.Start(ModulesFunctions.DumpUdonEvents());
            }, "This Will Dump Udon Events For Modding Game Worlds");

            #endregion

            #region UI

            new QMToggleButton(SettingsUI, 1, 0, "PlayerList", delegate
            {
                PlayerList.SetActive(true);
                Config.Main.PlayerList = true;
            }, delegate
            {
                PlayerList.SetActive(false);
                Config.Main.PlayerList = false;
            }, "Toggle: This Will Turn Off Or On The PlayerList", Config.Main.PlayerList);

            new QMToggleButton(SettingsUI, 2, 0, "Debug Panel", delegate
            {
                DebugPanel.SetActive(true);
                Config.Main.DebugPanel = true;
            }, delegate
            {
                DebugPanel.SetActive(false);
                Config.Main.DebugPanel = false;
            }, "Toggle: This Will Turn Off Or On The Debug Pane", Config.Main.DebugPanel);

            new QMToggleButton(SettingsUI, 3, 0, "NamePlate Stats", delegate
            {
                Config.Main.NamePlates = true;
                foreach (var p in WorldUtils.GetPlayers())
                {
                    Nameplates.Enable(p);
                    Nameplates.CreateNameplate(p);
                }
            }, delegate
            {
                Config.Main.NamePlates = false;
                foreach (var p in WorldUtils.GetPlayers())
                {
                    Nameplates.Disable(p);
                    Nameplates.DeleteNameplate(p);
                }
            }, "Toggle: This Will Turn Off Or On The NamePlates", Config.Main.NamePlates);

            new QMToggleButton(SettingsUI, 4, 0, "Custom\nNamePlates", delegate
            {
                Config.Main.CustomNamePlates = true;
                foreach (var p in WorldUtils.GetPlayers())
                {
                    Nameplates.Enable(p);
                    Nameplates.CreateNameplate(p);
                }
            }, delegate
            {
                Config.Main.CustomNamePlates = false;
                foreach (var p in WorldUtils.GetPlayers())
                {
                    Nameplates.Disable(p);
                    Nameplates.DeleteNameplate(p);
                }
            }, "Toggle: This Will Turn Off Or On The Custom NamePlates", Config.Main.CustomNamePlates);

            new QMToggleButton(SettingsUI, 1, 1, "Custom\nNamePlate\nTags", delegate
            {
                Config.Main.CustomNamePlateTags = true;
                foreach (var p in WorldUtils.GetPlayers())
                {
                    Nameplates.Enable(p);
                    Nameplates.CreateNameplate(p);
                }
            }, delegate
            {
                Config.Main.CustomNamePlateTags = false;
                foreach (var p in WorldUtils.GetPlayers())
                {
                    Nameplates.Disable(p);
                    Nameplates.DeleteNameplate(p);
                }
            }, "Toggle: This Will Turn Off Or On The Custom NamePlates", Config.Main.CustomNamePlates);

            new QMToggleButton(SettingsUI, 2, 1, "Expire Panel", delegate
            {
                GetTime.Panel.SetActive(true);
                Config.Main.ExpirePanel = true;
            }, delegate
            {
                GetTime.Panel.SetActive(false);
                Config.Main.ExpirePanel = false;
            }, "Toggle: This Will Turn Off Or On The Expire Panel", Config.Main.ExpirePanel);

            new QMToggleButton(SettingsUI, 3, 1, "NoConsoleClear", delegate
            {
                Config.Main.NoConsoleClear = true;
            }, delegate
            {
                Config.Main.NoConsoleClear = false;
            }, "Toggle: Turn on or off ConsoleClear On StartUp", Config.Main.NoConsoleClear);

            new QMToggleButton(SettingsUI, 4, 1, "DiscordRPC", delegate
            {
                Config.Main.DiscordRPC = true;
                Modules.Discord.DiscordHelper.StartRPC();
            }, delegate
            {
                Config.Main.DiscordRPC = false;
                Modules.Discord.DiscordHelper.StopRPC();
            }, "Toggle: Stops The Discord RPC", Config.Main.DiscordRPC);

            new QMToggleButton(SettingsUI, 1, 2, "Custom Loading Screen", delegate
            {
                Config.Main.CustomLoading = true;
            }, delegate
            {
                Config.Main.CustomLoading = false;
            }, "Toggle: This Will Turn Off Or On The Custom Loading Screen", Config.Main.CustomLoading);

            new QMToggleButton(SettingsUI, 2, 2, "UseActionMenu", delegate
            {
                Config.Main.UseActionMenu = true;
            }, delegate
            {
                Config.Main.UseActionMenu = false;
            }, "Toggle: This Will Turn Off Or On The UseActionMenu", Config.Main.UseActionMenu);

            #region Ui Color

            var Color = new QMNestedButton(SettingsUI, 4, 1, "UI Color", "Settings If You Dont Know What Settings Is Please Leave The Game <3", "FC - UI Color");

            new QMToggleButton(Color, 1, 0, "Custom UI", delegate
            {
                Config.Main.CustomUi = true;
                MelonCoroutines.Start(Color_Edit.MenuColorEdit());
            }, delegate
            {
                Config.Main.CustomUi = false;
                MelonCoroutines.Start(Color_Edit.MenuColorEdit());
            }, "Toggle: This Will Edit The Main Menus Picture And UI Color <color=red>(NEED TO RESTART)</color>", Config.Main.CustomUi);

            new QMToggleButton(Color, 2, 0, "Ui ReColor", delegate
            {
                Config.Main.CustomUiColor = true;
                MelonCoroutines.Start(Color_Edit.MenuColorEdit());
            }, delegate
            {
                Config.Main.CustomUiColor = false;
            }, "Toggle: This Will Edit The Main Menus Picture And UI Color <color=red>(NEED TO RESTART)</color>", Config.Main.CustomUiColor);

            new QMSingleButton(Color, 3, 0, "Reset Ui\nColor", delegate
            {
                Config.Main.UIColorHex = "#0056FF";
                Config.Main.UITextColorHex = "#00359E";
                MelonCoroutines.Start(Color_Edit.MenuColorEdit());
            }, "Makes Your Ui Color Reset");

            new QMSingleButton(Color, 4, 0, "Custom Ui\nColor", delegate
            {
                PopupUtils.InputPopup("Enter Hex ID", "#FFFFFF", delegate (string s)
                {
                    Config.Main.UIColorHex = s;
                    MelonCoroutines.Start(Color_Edit.MenuColorEdit());
                });
                MelonCoroutines.Start(Color_Edit.MenuColorEdit());
            }, "");

            new QMSingleButton(Color, 1, 1, "Custom Text\nColor", delegate
            {
                PopupUtils.InputPopup("Enter Hex ID", "#FFFFFF", delegate (string s)
                {
                    Config.Main.UITextColorHex = s;
                    MelonCoroutines.Start(Color_Edit.MenuColorEdit());
                });
                MelonCoroutines.Start(Color_Edit.MenuColorEdit());
            }, "");

            new QMSingleButton(Color, 2, 1, "Custom Ui\nPic", delegate
            {
                PopupUtils.InputPopup("Enter URL", "Has To End In .PNG Or .JPG", delegate (string s)
                {
                    Config.Main.MenuPicture = s;
                    MelonCoroutines.Start(Color_Edit.MenuColorEdit());
                });
                MelonCoroutines.Start(Color_Edit.MenuColorEdit());
            }, "");

            #endregion

            #endregion

            #region Moderation

            new QMToggleButton(SettingsModeration, 1, 0, "HudPlayer\nJoin", delegate
            {
                Config.Main.HudPlayerJoin = true;
            }, delegate
            {
                Config.Main.HudPlayerJoin = false;
            }, "Toggle: This Will Turn Off Or On The HudPlayerJoin", Config.Main.HudPlayerJoin);

            new QMToggleButton(SettingsModeration, 2, 0, "HudPlayer\nJoinFriends", delegate
            {
                Config.Main.HudPlayerJoinFriends = true;
            }, delegate
            {
                Config.Main.HudPlayerJoinFriends = false;
            }, "Toggle: This Will Turn Off Or On The HudPlayerJoinFriends", Config.Main.HudPlayerJoinFriends);

            new QMToggleButton(SettingsModeration, 3, 0, "HudPlayer\nLeave", delegate
            {
                Config.Main.HudPlayerLeave = true;
            }, delegate
            {
                Config.Main.HudPlayerLeave = false;
            }, "Toggle: This Will Turn Off Or On The HudPlayerLeave", Config.Main.HudPlayerLeave);

            new QMToggleButton(SettingsModeration, 4, 0, "HudPlayer\nLeaveFriends", delegate
            {
                Config.Main.HudPlayerLeaveFriends = true;
            }, delegate
            {
                Config.Main.HudPlayerLeaveFriends = false;
            }, "Toggle: This Will Turn Off Or On The HudPlayerJoinFriends", Config.Main.HudPlayerLeaveFriends);

            new QMToggleButton(SettingsModeration, 1, 1, "HudModJoin", delegate
            {
                Config.Main.HudModJoin = true;
            }, delegate
            {
                Config.Main.HudModJoin = false;
            }, "Toggle: This Will Turn Off Or On The HudModJoin", Config.Main.HudModJoin);

            new QMToggleButton(SettingsModeration, 2, 1, "PopUpModJoin", delegate
            {
                Config.Main.PopUpModJoin = true;
            }, delegate
            {
                Config.Main.PopUpModJoin = false;
            }, "Toggle: This Will Turn Off Or On The PopUpModJoin", Config.Main.PopUpModJoin);

            new QMToggleButton(SettingsModeration, 3, 1, "HudWarns", delegate
            {
                Config.Main.HudWarns = true;
            }, delegate
            {
                Config.Main.HudWarns = false;
            }, "Toggle: This Will Turn Off Or On The HudWarns", Config.Main.HudWarns);

            new QMToggleButton(SettingsModeration, 3, 1, "HudMicOff", delegate
            {
                Config.Main.HudMicOff = true;
            }, delegate
            {
                Config.Main.HudMicOff = false;
            }, "Toggle: This Will Turn Off Or On The HudMicOff", Config.Main.HudMicOff);

            new QMToggleButton(SettingsModeration, 4, 1, "HudVoteKicks", delegate
            {
                Config.Main.HudVoteKicks = true;
            }, delegate
            {
                Config.Main.HudVoteKicks = false;
            }, "Toggle: This Will Turn Off Or On The HudVoteKicks", Config.Main.HudVoteKicks);

            new QMToggleButton(SettingsModeration, 1, 2, "HudBlocks", delegate
            {
                Config.Main.HudBlocks = true;
            }, delegate
            {
                Config.Main.HudBlocks = false;
            }, "Toggle: This Will Turn Off Or On The HudBlocks", Config.Main.HudBlocks);

            new QMToggleButton(SettingsModeration, 2, 2, "HudMutes", delegate
            {
                Config.Main.HudMutes = true;
            }, delegate
            {
                Config.Main.HudMutes = false;
            }, "Toggle: This Will Turn Off Or On The HudMutes", Config.Main.HudMutes);

            new QMToggleButton(SettingsModeration, 3, 2, "HudUnmutes", delegate
            {
                Config.Main.HudUnmutes = true;
            }, delegate
            {
                Config.Main.HudUnmutes = false;
            }, "Toggle: This Will Turn Off Or On The HudUnmutes", Config.Main.HudUnmutes);

            new QMToggleButton(SettingsModeration, 4, 2, "PlayerLog", delegate
            {
                Config.Main.PlayerLog = true;
            }, delegate
            {
                Config.Main.PlayerLog = false;
            }, "Toggle: This Will Turn Off Or On The PlayerLog", Config.Main.PlayerLog);

            #endregion

            #region Misc

            new QMToggleButton(SettingsMisc, 2, 0, "Auto Update\nCrasher", delegate
            {
                Config.Main.AutoUpdateCrasher = true;
            }, delegate
            {
                Config.Main.AutoUpdateCrasher = false;
            }, "Toggle: This Will Auto Update The Crasher ID In The Config", Config.Main.AutoUpdateCrasher);

            new QMSlider(SettingsMisc, -510, -740, "Avatar Hide Distance", 0.1f, 55, Config.Main.AviDistinceHider, delegate (float f)
            {
                Config.Main.AviDistinceHider = f;
            });

            #endregion

            #region Orbit

            new QMSlider(SettingsOrbit, -770, -360, "Item Orbit Speed", 0.1f, 55, Config.Main.ItemOrbitSpeed, delegate (float f)
            {
                Config.Main.ItemOrbitSpeed = f;
            });

            new QMSlider(SettingsOrbit, -770, -460, "Item Orbit Size", 0.1f, 55, Config.Main.ItemOrbitSize, delegate (float f)
            {
                Config.Main.ItemOrbitSize = f;
            });

            new QMSlider(SettingsOrbit, -270, -360, "Item Orbit Up/Down", 0.1f, 55, Config.Main.ItemOrbitUpDown, delegate (float f)
            {
                Config.Main.ItemOrbitUpDown = f;
            });

            #endregion

            #endregion

            #region QM

            var Selected = new QMNestedButton("Menu_SelectedUser_Local", 0, 0, "", "Target functions", "FC - Target");
            var AttachMenu = new QMNestedButton(Selected, 2, 2, "Attach Menu", "Target functions", "FC - Target");
            var BotMenu = new QMNestedButton(Selected, 3, 2, "Bot Menu", "Target functions", "FC - Target");
            var selfOpenBtn = UnityEngine.Object.Instantiate(
                APIStuff.GetQuickMenuInstance().transform.Find("Container/Window/QMParent/Menu_SelectedUser_Local/Header_H1/RightItemContainer/Button_QM_Expand").gameObject,
                APIStuff.GetQuickMenuInstance().transform.Find("Container/Window/QMParent/Menu_SelectedUser_Local/Header_H1/RightItemContainer"), false);
            selfOpenBtn.GetComponentInChildren<Image>().sprite = AssetBundleManager.Logo;
            selfOpenBtn.GetComponentInChildren<Image>().overrideSprite = AssetBundleManager.Logo;
            selfOpenBtn.GetComponentInChildren<Image>().gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
            selfOpenBtn.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            selfOpenBtn.GetComponent<Button>().onClick.AddListener(new Action(() =>
            {
                UI.Target = QuickMenuUtils.GetPlayerSelectedUser().GetVRCPlayer();
                UI.Target2 = QuickMenuUtils.GetPlayerSelectedUser();
                Selected.OpenMe();
            }));
            var otherOpenBtn = UnityEngine.Object.Instantiate(
                APIStuff.GetQuickMenuInstance().transform.Find("Container/Window/QMParent/Menu_SelectedUser_Remote/Header_H1/RightItemContainer/Button_QM_Expand").gameObject,
                APIStuff.GetQuickMenuInstance().transform.Find("Container/Window/QMParent/Menu_SelectedUser_Remote/Header_H1/RightItemContainer"), false);
            otherOpenBtn.GetComponentInChildren<Image>().sprite = AssetBundleManager.Logo;
            otherOpenBtn.GetComponentInChildren<Image>().overrideSprite = AssetBundleManager.Logo;
            otherOpenBtn.GetComponentInChildren<Image>().gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
            otherOpenBtn.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            otherOpenBtn.GetComponent<Button>().onClick.AddListener(new Action(() =>
            {
                UI.Target = QuickMenuUtils.GetPlayerSelectedUser().GetVRCPlayer();
                UI.Target2 = QuickMenuUtils.GetPlayerSelectedUser();
                Selected.OpenMe();
            }));

            var CloneButton = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_SelectedUser_Local/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_AvatarActions").transform;

            ForceClone = new QMSingleButton("Menu_Dashboard", 4, 0, "Clone", delegate
            {
                ModulesFunctions.ChangeAvatar(QuickMenuUtils.GetPlayerSelectedUser().GetVRCPlayer().GetApiAvatar().id);
            }, "Clones the selected user's Avi");

            Selected.GetMainButton().SetActive(false);
            ForceClone.GetGameObject().transform.SetParent(CloneButton);

            GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_SelectedUser_Local/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_AvatarActions/Button_CloneAvatar").DestroyMeLocal();

            #region Main

            ItemOrbitButton = new QMToggleButton(Selected, 1, 0, "Item Orbit", delegate
            {
                ItemOrbit.ItemOrbitToggle = true;
            }, delegate
            {
                ItemOrbit.ItemOrbitToggle = false;
            }, "Toggle: Orbit all pickup items in the world around the target");

            new QMToggleButton(Selected, 2, 0, "CamAnnoy", delegate
            {
                ModulesFunctions.IsFlashing = true;
                MelonCoroutines.Start(ModulesFunctions.CameraAnn(UI.Target2));
            }, delegate
            {
                ModulesFunctions.IsFlashing = false;
            }, "Toggle: This Will Spam An Annoying Ass Sound By The Camera");

            new QMToggleButton(Selected, 3, 0, "Swastika", delegate
            {
                ModulesFunctions.SwastikaBool = true;
                ModulesFunctions.SwastikaFolowBool = true;
                MelonCoroutines.Start(ModulesFunctions.SwastikaFolow());
            }, delegate
            {
                ModulesFunctions.SwastikaBool = false;
                ModulesFunctions.SwastikaFolowBool = false;
                MelonCoroutines.Start(ModulesFunctions.SwastikaFolow());
                ModulesFunctions.PositionToGoTo = Vector3.negativeInfinity;
                ModulesFunctions.RespawnPickups();
            }, "Toggle: This Will Spawn A Swastika On Them");

            new QMToggleButton(Selected, 4, 0, "Voice Mimic", delegate
            {
                Patches.VoiceMimic = true;
            }, delegate
            {
                Patches.VoiceMimic = false;
            }, "Toggle: This Will Mimic That Players Voice");

            new QMToggleButton(Selected, 1, 1, "PortalAnnoy", delegate
            {
                Config.Main.InfPortals = false;
                ModulesFunctions.PortalSpam = true;
                MelonCoroutines.Start(ModulesFunctions.PortalAnnoy(UI.Target2));
            }, delegate
            {
                ModulesFunctions.PortalSpam = false;
            }, "Toggle: This Will Spam INF FC Portals On A Player");

            new QMToggleButton(Selected, 2, 1, "Ip Grabber\nPortal", delegate
            {
                Process.Start("https://iplogger.org/logger/F6rd3q6ezxcK");
                Logs.Hud("Please Look At Your Browser");
                Config.Main.InfPortals = true;
                ModulesFunctions.IpGrabber = true;
                MelonCoroutines.Start(ModulesFunctions.IpGrabberPortal(UI.Target2));
            }, delegate
            {
                Config.Main.InfPortals = false;
                ModulesFunctions.IpGrabber = false;
            }, "Toggle: This Will Spam Portals On A Player When Entered Will Log There IP (BETA)");

            new QMSingleButton(Selected, 3, 1, "Silent Favorite", delegate
            {
                foreach (var list in Config.AvatarFavorites.list.FavoriteLists)
                {
                    if (!list.Avatars.Exists(avi => avi.ID == Target2.prop_ApiAvatar_0.id))
                        AvatarFavorites.FavoriteAvatar(Target2.prop_ApiAvatar_0, list.ID);
                    else
                        AvatarFavorites.UnfavoriteAvatar(Target2.prop_ApiAvatar_0, list.ID);
                }
            }, "This Will Silently Favorite The Persons Avatar Into Your List");

            new QMSingleButton(Selected, 1, 3, "Target Crash", delegate
            {
                MelonCoroutines.Start(ModulesFunctions.TargetCrash(Target2));
            }, "Crash User With The Avatar You Have In Your Config", true);

            new QMSingleButton(Selected, 1, 3.5f, "Teleport To", delegate
            {
                ModulesFunctions.TeleportToPlayer(Target2);
            }, "teleport to the selected user", true);

            new QMSingleButton(Selected, 2, 3, "Copy\nAvatar ID", delegate
            {
                Clipboard.SetText(Target2.GetApiAvatar().id);
            }, "teleport to the selected user", true);

            new QMSingleButton(Selected, 2, 3.5f, "Copy\nUser ID", delegate
            {
                Clipboard.SetText(Target2.GetUserID());
            }, "teleport to the selected user", true);

            new QMSingleButton(Selected, 3, 3, "Open\nVRC Profile", delegate
            {
                Process.Start("https://vrchat.com/home/user/" + Target2.GetUserID());
            }, "teleport to the selected user", true);

            new QMSingleButton(Selected, 3, 3.5f, "Download VRCA", delegate
            {
                var avatar = Target2.GetApiAvatar();
                new Task(() => { ModulesFunctions.DownloadVRCA(avatar); }).Start();
                new Task(() => { ModulesFunctions.DownloadAviImage(avatar); }).Start();
            }, "teleport to the selected user", true);

            new QMSingleButton(Selected, 4, 3, "Copy\nUser Info", delegate
            {
                Clipboard.SetText($"╱▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔▔╲\nName: {Target2.GetDisplayName()} \nReg Name: {Target2.GetVRCPlayerApi().GetDisplayName()} \nUserPicURL: {Target2.GetVRCPlayer().GetAPIUser().profilePicImageUrl} \nUser ID: {Target2.GetVRCPlayer().GetUserID()} \nAvatar ID: {Target2.GetVRCPlayer().GetApiAvatar().id} \nAvatarImageURL: {Target2.GetVRCPlayer().GetAPIUser().currentAvatarImageUrl} \nAvatarAssetURL: {Target2.GetVRCPlayer().GetAPIUser().currentAvatarAssetUrl} \nIs Know: {Target2.GetVRCPlayer().GetAPIUser().hasKnownTrustLevel} \nIs Trusted: {Target2.GetVRCPlayer().GetAPIUser().hasTrustedTrustLevel}\nBio: {Target2.GetVRCPlayer().GetAPIUser().bio} \nStatus: {Target2.GetVRCPlayer().GetAPIUser().status} \nPlatform: {Target2.GetVRCPlayer().GetAPIUser().last_platform} \nIs Friend: {Target2.GetVRCPlayer().GetAPIUser().isFriend}\n╲▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂▂╱");
            }, "teleport to the selected user", true);

            new QMSingleButton(Selected, 4, 3.5f, "Reload Avatar", delegate
            {
                VRCPlayer.Method_Public_Static_Void_APIUser_0(Target2.GetVRCPlayer().GetAPIUser());
            }, "Reload's the selected user's avatar", true);

            #endregion

            #region Attach Menu

            new QMSingleButton(AttachMenu, 2.5f, 0.5f, "head", delegate
            {
                var Player = Target2;
                ModulesFunctions.AttachToPlayer = true;
                MelonCoroutines.Start(ModulesFunctions.Attach(Player));
            }, "Attach to a player's head");

            new QMSingleButton(AttachMenu, 3, 1.5f, "Right Hand", delegate
            {
                var Player = Target2;
                ModulesFunctions.AttachToPlayerRightHand = true;
                MelonCoroutines.Start(ModulesFunctions.AttachRightHand(Player));
            }, "Attach to a player's Right Hand");

            new QMSingleButton(AttachMenu, 2, 1.5f, "Left Hand", delegate
            {
                var Player = Target2;
                ModulesFunctions.AttachToPlayerLeftHand = true;
                MelonCoroutines.Start(ModulesFunctions.AttachLeftHand(Player));
            }, "Attach to a player's Left Hand");

            new QMSingleButton(AttachMenu, 3, 2.5f, "Right Leg", delegate
            {
                var Player = Target2;
                ModulesFunctions.AttachToPlayerRightLeg = true;
                MelonCoroutines.Start(ModulesFunctions.AttachRightLeg(Player));
            }, "Attach to a player's Right Leg");

            new QMSingleButton(AttachMenu, 2, 2.5f, "Left Leg", delegate
            {
                var Player = Target2;
                ModulesFunctions.AttachToPlayerLeftLeg = true;
                MelonCoroutines.Start(ModulesFunctions.AttachLeftLeg(Player));
            }, "Attach to a player's Left Leg");

            #endregion

            #region Bot CMDS

            new QMSingleButton(BotMenu, 1, 0, "Mimic", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.FOLLOW, Target2.GetUserID()));
            }, "This Will Make The Bots Mmic The Selected Player");

            new QMSingleButton(BotMenu, 2, 0, "Orbit", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.ORBIT, Target2.GetUserID()));
            }, "This Will Make The Bots Orbit The Selected Player");

            new QMSingleButton(BotMenu, 3, 0, "Realign", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.ALIGN_CHANGE));
            }, "This Will Realign The Bots");

            new QMSingleButton(BotMenu, 4, 0, "Tp To Player", delegate
            {
                BotServer.SendClient(BotTarget, new PacketData(PacketBotServerType.TP, $"{Target2.GetUserID()}"));
            }, "Change Bots Avis");

            new QMToggleButton(BotMenu, 1, 1, "Voice Mimic", delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.E1_MIMIC, Target2.GetAPIUser().id));
            }, delegate
            {
                BotServer.SendAll(new PacketData(PacketBotServerType.E1_MIMIC, $""));
            }, "Toggle: This Is To Mimic The Voice Of The User");

            #endregion

            #endregion

            #region Room History

            roomHistoryScroll = new QMScrollMenu(roomHistory);
            roomHistorySelected = new QMNestedButton(roomHistory, 0, 0, "", "", "Selected Instance");
            roomHistorySelected.GetMainButton().SetActive(false);
            roomHistorySelectedPanel = new QMInfo(roomHistorySelected, 0, 0, 950, 250, "")
            {
                InfoText =
                {
                    color = UnityEngine.Color.white,
                    fontSize = 30,
                    alignment = TextAnchor.MiddleCenter,
                    supportRichText = true,
                },
            };
            roomHistorySelectedPanel.InfoBackground.enabled = false;

            new QMSingleButton(roomHistory, 4, 0, "<color=yellow>Clear\nHistory</color>", delegate
            {
                PopupUtils.AlertV2("Are you sure you want to clear your instance history? This cannot be reversed!", "Clear", delegate
                {
                    Config.RoomHistory.list.Clear();
                    Config.RoomHistory.Save();
                    PopupUtils.HideCurrentPopUp();
                    roomHistoryScroll.Refresh();
                }, "Cancel", PopupUtils.HideCurrentPopUp);
            }, "Clears your Instance History list");

            new QMSingleButton(roomHistorySelected, 1, 0, "Join\nInstance", delegate
            {
                ModulesFunctions.JoinRoom(selectedRoom.JoinID);
            }, "Click to join the instance for yourself");

            new QMSingleButton(roomHistorySelected, 2, 0, "Portal To\nInstance", delegate
            {
                MiscUtils.DropPortal(selectedRoom.JoinID);
            }, "Click to drop a portal for others to join the instance");

            new QMSingleButton(roomHistorySelected, 3, 0, "Copy ID", delegate
            {
                System.Windows.Forms.Clipboard.SetText(selectedRoom.JoinID);
            }, "Copy WorldId to this instance");

            new QMSingleButton(roomHistorySelected, 4, 0, "Remove\nInstance", delegate
            {
                PopupUtils.AlertV2("Are you sure you would like to remove this instance from your history?", "Remove", delegate
                {
                    Config.RoomHistory.list.Remove(selectedRoom);
                    Config.RoomHistory.Save();
                    PopupUtils.HideCurrentPopUp();
                    roomHistorySelected.CloseMe();
                    roomHistoryScroll.Refresh();
                }, "Cancel", PopupUtils.HideCurrentPopUp);
            }, "Click to remove this instance from your history");

            roomHistoryScroll.SetAction(delegate
            {
                var tmp = Config.RoomHistory.list;
                tmp.Reverse();
                foreach (var item in tmp)
                {
                    roomHistoryScroll.Add(new QMSingleButton(roomHistoryScroll.BaseMenu, 0, 0, item.Name, delegate
                    {
                        selectedRoom = item;
                        StringBuilder sb = new();
                        sb.AppendLine("<color=cyan><b>World Name:</b></color> " + item.Name);
                        sb.AppendLine("<color=cyan><b>Instance ID:</b></color> " + item.JoinID);
                        roomHistorySelectedPanel.SetText(sb.ToString());
                        roomHistorySelected.OpenMe();
                    }, "Click to view options about this instance"));
                }
            });

            #endregion

            #region Udon Menu

            UdonMenu = new QMNestedButton(Menu, 3, 2.5f, "Udon\nIndexer", "View udon scripts and manipulate them how you want", "Udon Indexer");
            ActionsMenu = new QMNestedButton(UdonMenu, 0, 0, "", "", "Udon Manipulator");
            ActionsMenu.GetMainButton().SetActive(false);
            Scroll = new QMScrollMenu(UdonMenu);
            Scroll2 = new QMScrollMenu(ActionsMenu);

            Scroll2.SetAction(delegate
            {
                foreach (var e in selectedScript._eventTable)
                {
                    Scroll2.Add(new QMSingleButton(Scroll2.BaseMenu, 0, 0, e.key.StartsWith("_") ? $"<color=red>{e.key}</color>" : $"<color=green>{e.key}</color>", delegate
                    {
                        selectedScript.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, e.Key);
                    }, "Click me to trigger this event."));
                }
            });

            Scroll.SetAction(delegate
            {
                if (WorldUtils.GetSDKType() == "SDK3")
                {
                    int scriptCount = 0;
                    foreach (var u in WorldUtils.GetUdonScripts())
                    {
                        scriptCount++;
                        Scroll.Add(new QMSingleButton(Scroll.BaseMenu, 0, 0, $"{u.name} [<color=#00FFFF>{scriptCount}</color>]", delegate
                        {
                            selectedScript = u;
                            ActionsMenu.GetMainButton().ClickMe();
                        }, "Click to view options for: " + u.name));
                    }
                }
                else
                {
                    Scroll.Add(new QMSingleButton(Scroll.BaseMenu, 0, 0, "SDK2\nWorld", delegate { }, "This world is made in SDK2! No Udon here."));
                }
            });

            #endregion
        }

        internal static void InitializeWing(BaseWing wing)
        {

        }

        internal static void InitializeActionMenu()
        {
            mainAW = new ActionMenuPage(ActionMenuBaseMenu.MainMenu, "Fusion Client", AssetBundleManager.LogoCircle.texture);

            flightButton = new ActionMenuButton(mainAW, "Flight: Off", delegate
            {
                UI.FlyToggle.SetToggleState(!Flight.Fly, true);
                if (Flight.Fly)
                {
                    flightButton.SetButtonText("Flight: On");
                }
                else
                {
                    flightButton.SetButtonText("Flight: Off");
                }
            }, AssetBundleManager.FlyAW.texture);

            #region Exploits

            var mic = new ActionMenuPage(mainAW, "Exploits", AssetBundleManager.ExploitsAW.texture);

            SerializationButton = new ActionMenuButton(mic, "Serialization: Off", delegate
            {
                UI.Serialization.SetToggleState(!ModulesFunctions.Serialization, true);
                if (ModulesFunctions.Serialization)
                {
                    SerializationButton.SetButtonText("Serialization: On");
                }
                else
                {
                    SerializationButton.SetButtonText("Serialization: Off");
                }
            }, AssetBundleManager.SerializationAW.texture);

            SwastikaButton = new ActionMenuButton(mic, "Swastika: Off", delegate
            {
                UI.Swastika.SetToggleState(!ModulesFunctions.SwastikaBool, true);
                if (ModulesFunctions.SwastikaBool)
                {
                    SwastikaButton.SetButtonText("Swastika: On");
                }
                else
                {
                    SwastikaButton.SetButtonText("Swastika: Off");
                }
            }, AssetBundleManager.SwastikaAW.texture);

            BrokenUspeakButton = new ActionMenuButton(mic, "Broken Uspeak: Off", delegate
            {
                UI.BrokenUspeak.SetToggleState(!ModulesFunctions.earrapeExploitRunning, true);
                if (ModulesFunctions.earrapeExploitRunning)
                {
                    BrokenUspeakButton.SetButtonText("Broken Uspeak: On");
                }
                else
                {
                    BrokenUspeakButton.SetButtonText("Broken Uspeak: Off");
                }
            }, AssetBundleManager.BrokenUspeakAW.texture);

            ItemLockButton = new ActionMenuButton(mic, "Item Lock: Off", delegate
            {
                UI.ItemLock.SetToggleState(!ModulesFunctions.ItemLockBool, true);
                if (ModulesFunctions.ItemLockBool)
                {
                    ItemLockButton.SetButtonText("Item Lock: On");
                }
                else
                {
                    ItemLockButton.SetButtonText("Item Lock: Off");
                }
            }, AssetBundleManager.ItemLockAW.texture);

            FreeFallButton = new ActionMenuButton(mic, "FreeFall: Off", delegate
            {
                UI.FreeFall.SetToggleState(!ModulesFunctions.FreeFallBool, true);
                if (ModulesFunctions.FreeFallBool)
                {
                    FreeFallButton.SetButtonText("FreeFall: On");
                }
                else
                {
                    FreeFallButton.SetButtonText("FreeFall: Off");
                }
            }, AssetBundleManager.FreeFallAW.texture);

            SpamMirrorsButton = new ActionMenuButton(mic, "Mirror Spam: Off", delegate
            {
                UI.MirrorSpam.SetToggleState(!ModulesFunctions.SpamMirrorsBool, true);
                if (ModulesFunctions.SpamMirrorsBool)
                {
                    SpamMirrorsButton.SetButtonText("Mirror Spam: On");
                }
                else
                {
                    SpamMirrorsButton.SetButtonText("Mirror Spam: Off");
                }
            }, AssetBundleManager.MirrorSpamAW.texture);

            #endregion
        }
    }
}
