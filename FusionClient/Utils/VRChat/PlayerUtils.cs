using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.XR;
using VRC;
using VRC.Core;
using VRC.SDKBase;
using VRC.UI;
using VRC.UI.Elements.Menus;
using Object = Il2CppSystem.Object;
using PhotonHandler = Photon.Pun.PhotonHandler;

namespace FC.Utils
{
    internal static class PlayerUtils
    {
        #region CurrentUser

        internal static VRCPlayer GetCurrentUser()
        {
            if (VRCPlayer.field_Internal_Static_VRCPlayer_0 != null)
            {
                return VRCPlayer.field_Internal_Static_VRCPlayer_0;
            }
            else return null;
        }

        public static int GetActorNumber(this VRC.Player player)
        {
            return player.GetVRCPlayerApi().playerId;
        }

        public static VRC.Player LocalPlayer
        {
            get
            {
                return VRC.Player.prop_Player_0;
            }
        }

        //public static VRCTrackingManager VRCTrackingManager
        //{
        //    get
        //    {
        //        return VRCTrackingManager.field_Private_Static_VRCTrackingManager_0;
        //    }
        //}

        public static VRCUiManager VRCTrackingManager
        {
            get
            {
                return VRCUiManager.field_Private_Static_VRCUiManager_0;
            }
        }

        internal static bool SelfIsInVR()
        {
            return XRDevice.isPresent;
        }

        #endregion

        #region Player

        public static VRC.Player GetPlayerID(this VRC.PlayerManager Instance, int playerID)
        {
            var Players = Instance.GetPlayers();
            foreach (VRC.Player player in Players.ToArray())
                if (player.GetVRCPlayerApi().playerId == playerID)
                    return player;
            return null;
        }

        internal static IEnumerable<VRC.Player> GetPlayers()
        {
            return VRC.PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0.ToArray();
        }

        internal static System.Collections.Generic.List<VRC.Player> GetAllPlayersToList()
        {
            if (!(PlayerManager.field_Private_Static_PlayerManager_0 == null))
            {
                return PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0.ToArray().ToList();
            }
            return null;
        }

        internal static VRC.Player GetPlayer(this VRCPlayer instance)
        {
            return instance._player;
        }

        public static VRC.Player GetPlayerLog(this PlayerManager Instance, int playerID)
        {
            List<VRC.Player> list = Instance.GetPlayers();
            foreach (VRC.Player player in list.ToArray())
            {
                if (player.GetVRCPlayerApi().playerId == playerID)
                {
                    return player;
                }
            }
            return null;
        }

        public static PlayerManager PlayerManager
        {
            get
            {
                return PlayerManager.field_Private_Static_PlayerManager_0;
            }
        }

        #endregion

        #region API User

        internal static APIUser GetAPIUser(this VRCPlayer instance)
        {
            return instance._player.prop_APIUser_0;
        }

        internal static APIUser GetAPIUser(this VRC.Player instance)
        {
            return instance.prop_APIUser_0;
        }

        #endregion

        #region Api Avatar
        // Get PC Api Avatar

        internal static ApiAvatar GetApiAvatar(this VRCPlayer instance)
        {
            return instance.prop_ApiAvatar_0;
        }

        internal static ApiAvatar GetApiAvatar(this VRC.Player instance)
        {
            return instance.prop_ApiAvatar_0;
        }

        // Get Quest Api Avatar (fallback avatar)
        internal static ApiAvatar GetApiAvatarQuest(this VRCPlayer instance)
        {
            return instance.prop_ApiAvatar_1;
        }

        internal static ApiAvatar GetApiAvatarQuest(this VRC.Player instance)
        {
            return instance._vrcplayer.prop_ApiAvatar_1;
        }

        // Api Avatar Details
        internal static bool IsAllPlatform(this ApiAvatar instance)
        {
            if (instance.supportedPlatforms == ApiModel.SupportedPlatforms.All) return true;
            else return false;
        }

        internal static bool IsPCPlatform(this ApiAvatar instance)
        {
            if (instance.supportedPlatforms == ApiModel.SupportedPlatforms.StandaloneWindows) return true;
            else return false;
        }

        internal static bool IsQuestPlatform(this ApiAvatar instance)
        {
            if (instance.supportedPlatforms == ApiModel.SupportedPlatforms.Android) return true;
            else return false;
        }

        #endregion

        #region VRCPlayer

        internal static VRCPlayer GetVRCPlayer(this VRC.Player instance)
        {
            return instance._vrcplayer;
        }

        #endregion

        #region USpeaker

        internal static USpeaker GetUSpeaker(this VRC.Player instance)
        {
            return instance.prop_USpeaker_0;
        }

        internal static USpeaker GetUSpeaker(this VRCPlayer instance)
        {
            return instance.prop_USpeaker_0;
        }

        internal static bool Is_Talking(this USpeaker speaker)
        {
            return speaker.prop_Boolean_0;
        }

        #endregion

        #region VRCPlayerApi
        internal static VRCPlayerApi GetVRCPlayerApi(this VRCPlayer instance)
        {
            return instance.prop_VRCPlayerApi_0;
        }

        internal static VRCPlayerApi GetVRCPlayerApi(this VRC.Player instance)
        {
            return instance.prop_VRCPlayerApi_0;
        }
        #endregion

        #region PlayerNet
        internal static PlayerNet GetPlayerNet(this VRC.Player instance)
        {
            return instance._playerNet;
        }

        internal static PlayerNet GetPlayerNet(this VRCPlayer instance)
        {
            return instance._playerNet;
        }
        #endregion

        #region Photon
        internal static Photon.Realtime.LoadBalancingClient LoadBalancingPeer
        {
            get
            {
                return PhotonHandler.prop_LoadBalancingClient_0;
            }
        }

        internal static PhotonHandler PhotonHandler
        {
            get
            {
                //return PhotonHandler.field_Internal_Static_MonoBehaviour1PrivateObInPrInBoInInInInUnique_0;
                return PhotonHandler.field_Internal_Static_PhotonHandler_0;
            }
        }
        #endregion

        #region Avatar
        internal static void ReloadAvatar(this VRCPlayer Instance)
        {
            VRCPlayer.Method_Public_Static_Void_APIUser_0(Instance.GetAPIUser());
        }

        internal static void ReloadAvatar(this VRC.Player Instance)
        {
            ReloadAvatar(Instance.GetVRCPlayer());
        }

        public static GameObject GetAvatar(this VRC.Player Instance)
        {
            return Instance.GetVRCPlayer().GetAvatarManager().GetAvatar();
        }

        public static GameObject GetAvatar(this VRCPlayer Instance)
        {
            return Instance.GetAvatarManager().GetAvatar();
        }

        public static VRCAvatarManager GetAvatarManager(this VRCPlayer Instance)
        {
            return Instance.prop_VRCAvatarManager_0;
        }

        public static GameObject GetAvatar(this VRCAvatarManager Instance)
        {
            if (Instance.prop_GameObject_0 != null)
                return Instance.prop_GameObject_0;
            //if (Instance.field_Private_GameObject_1 != null) //field_Private_GameObject_1
            //    return Instance.field_Private_GameObject_1;
            return null;
        }

        #endregion

        #region User Info
        // QM
        internal static VRCPlayer GetSelectedUser()
        {
            var a = UnityEngine.Object.FindObjectOfType<SelectedUserMenuQM>().field_Private_IUser_0;
            return GetPlayerByUserID(a.prop_String_0)._vrcplayer;
        }
        internal static VRC.Player GetPlayerByUserID(string userID)
        {
            return WorldUtils.GetPlayers().FirstOrDefault(p => p.prop_APIUser_0.id == userID);
        }

        // UserInfo

        internal static APIUser GetUser()
        {
            return GameObject.Find("UserInterface/MenuContent/Screens/UserInfo").GetComponent<PageUserInfo>().field_Private_APIUser_0;
        }
        internal static ApiWorld GetUserWorld()
        {
            return GameObject.Find("UserInterface/MenuContent/Screens/UserInfo").GetComponent<PageUserInfo>().field_Private_ApiWorld_0;
        }

        internal static ApiWorld GetSelectedWorld()
        {
            return GameObject.Find("UserInterface/MenuContent/Screens/WorldInfo").GetComponent<PageWorldInfo>().field_Private_ApiWorld_0;
        }
        internal static ApiWorldInstance GetSelectedInstance()
        {
            return GameObject.Find("UserInterface/MenuContent/Screens/WorldInfo").GetComponent<PageWorldInfo>().field_Public_ApiWorldInstance_0;
        }

        // Is In VR
        internal static bool IsInVR(this VRCPlayer instance)
        {
            return instance.prop_VRCPlayerApi_0.IsUserInVR();
        }

        internal static bool IsInVR(this VRCPlayerApi instance)
        {
            return instance.IsUserInVR();
        }

        internal static bool IsInVR(this VRC.Player instance)
        {
            return instance.prop_VRCPlayerApi_0.IsUserInVR();
        }

        // Is Instance Master
        internal static bool IsInstanceMaster(this VRCPlayer instance)
        {
            return instance.prop_VRCPlayerApi_0.isMaster;
        }

        internal static bool IsInstanceMaster(this VRCPlayerApi instance)
        {
            return instance.isMaster;
        }

        internal static bool IsInstanceMaster(this VRC.Player instance)
        {
            return instance.prop_VRCPlayerApi_0.isMaster;
        }

        // Get Users Home World
        internal static void GetHomeWorld(this VRC.Player instance)
        {
            instance.prop_Player_1.field_Private_Room_0.ToString();
        }

        internal static string GetHomeWorld(this VRCPlayer instance)
        {
            return instance._player.field_Private_Player_0.field_Private_Room_0.prop_String_0;
        }

        // Is Instance Creator
        internal static bool IsInstanceCreator(this VRCPlayer instance)
        {
            return instance.prop_VRCPlayerApi_0.isInstanceOwner;
        }

        internal static bool IsInstanceCreator(this VRCPlayerApi instance)
        {
            return instance.isInstanceOwner;
        }

        internal static bool IsInstanceCreator(this VRC.Player instance)
        {
            return instance.prop_VRCPlayerApi_0.isInstanceOwner;
        }

        // Is World Creator
        internal static bool IsWorldCreator(this VRC.Player instance)
        {
            if (WorldUtils.GetCurrentWorld().authorId == instance.prop_APIUser_0.id) return true;
            else return false;
        }

        internal static bool IsWorldCreator(this VRCPlayer instance)
        {
            if (WorldUtils.GetCurrentWorld().authorId == instance._player.prop_APIUser_0.id) return true;
            else return false;
        }

        internal static bool IsWorldCreator(this APIUser instance)
        {
            if (WorldUtils.GetCurrentWorld().authorId == instance.id) return true;
            else return false;
        }

        // Is Friend
        internal static bool IsFriend(this VRCPlayer instance)
        {
            if (APIUser.IsFriendsWith(instance._player.field_Private_APIUser_0.id)) return true;
            else return false;
        }

        internal static bool IsFriend(this VRC.Player instance)
        {
            if (APIUser.IsFriendsWith(instance.field_Private_APIUser_0.id)) return true;
            else return false;
        }

        internal static bool IsFriend(this APIUser instance)
        {
            if (APIUser.IsFriendsWith(instance.id)) return true;
            else return false;
        }

        // Is Staff Member
        internal static bool IsStaffMember(this APIUser instance)
        {
            if (instance.tags.Contains("admin_moderator")
                || instance.hasModerationPowers
                || instance.tags.Contains("admin_")
                || instance.hasSuperPowers
                || instance.id == "usr_81ac81f6-cdd1-4eaa-961f-22a80dc772c9")
                return true;
            else return false;
        }

        internal static bool IsStaffMember(this VRCPlayer instance)
        {
            if (instance.GetAPIUser().tags.Contains("admin_moderator")
                || instance.GetAPIUser().hasModerationPowers
                || instance.GetAPIUser().tags.Contains("admin_")
                || instance.GetAPIUser().hasSuperPowers
                || instance.GetAPIUser().id == "usr_81ac81f6-cdd1-4eaa-961f-22a80dc772c9")
                return true;
            else return false;
        }

        internal static bool IsStaffMember(this VRC.Player instance)
        {
            if (instance.GetAPIUser().tags.Contains("admin_moderator")
                || instance.GetAPIUser().hasModerationPowers
                || instance.GetAPIUser().tags.Contains("admin_")
                || instance.GetAPIUser().hasSuperPowers
                || instance.GetAPIUser().id == "usr_81ac81f6-cdd1-4eaa-961f-22a80dc772c9")
                return true;
            else return false;
        }

        // Display Name
        internal static string GetDisplayName(this VRCPlayerApi instance)
        {
            return instance.displayName;
        }

        internal static string GetDisplayName(this VRCPlayer instance)
        {
            return instance._player.prop_APIUser_0.displayName;
        }

        internal static string GetDisplayName(this VRC.Player instance)
        {
            return instance.prop_APIUser_0.displayName;
        }

        internal static string GetDisplayName(this APIUser instance)
        {
            return instance.displayName;
        }

        // User ID
        internal static string GetUserID(this VRCPlayer instance)
        {
            return instance._player.prop_APIUser_0.id;
        }

        internal static string GetUserID(this VRC.Player instance)
        {
            return instance.prop_APIUser_0.id;
        }

        internal static string GetUserID(this APIUser instance)
        {
            return instance.id;
        }

        // Frames
        internal static int GetFrames(this VRC.Player instance)
        {
            return instance.GetPlayerNet().prop_Byte_0 != 0 ? (int)(1000f / instance.GetPlayerNet().prop_Byte_0) : 0;
        }

        internal static int GetFrames(this VRCPlayer instance)
        {
            return instance.GetPlayerNet().prop_Byte_0 != 0 ? (int)(1000f / instance.GetPlayerNet().prop_Byte_0) : 0;
        }

        internal static string GetPlatform(this VRCPlayer instance)
        {
            if (instance.GetVRCPlayerApi().IsUserInVR())
            {
                if (instance.GetPlatform() != "VR")
                {
                    return "<color=#1aff00>Q</color>";
                }
                else
                {
                    return "<color=grey>VR</color>";
                }
            }
            else
            {
                return "<color=grey>PC</color>";
            }
        }

        internal static string GetFramesColored(this VRC.Player instance)
        {
            var frames = GetFrames(instance);
            if (frames >= 80)
            {
                return $"<color=green>{frames}</color>";
            }
            else
            {
                if (frames <= 25)
                {
                    return $"<color=red>{frames}</color>";
                }
                else
                {
                    return $"<color=orange>{frames}</color>";
                }
            }
        }

        internal static string GetFramesColored(this VRCPlayer instance)
        {
            var frames = GetFrames(instance);
            if (frames >= 80)
            {
                return $"<color=green>{frames}</color>";
            }
            else
            {
                if (frames <= 25)
                {
                    return $"<color=red>{frames}</color>";
                }
                else
                {
                    return $"<color=orange>{frames}</color>";
                }
            }
        }

        // Ping
        internal static short GetPing(this VRCPlayer instance)
        {
            return instance.GetPlayerNet().field_Private_Int16_0;
        }

        internal static short GetPing(this VRC.Player instance)
        {
            return instance.GetPlayerNet().field_Private_Int16_0;
        }

        internal static string GetPingColored(this VRCPlayer instance)
        {
            var ping = GetPing(instance);
            if (ping >= 80)
            {
                return $"<color=red>{ping}</color>";
            }
            else
            {
                if (ping <= 35)
                {
                    return $"<color=green>{ping}</color>";
                }
                else
                {
                    return $"<color=orange>{ping}</color>";
                }
            }
        }

        internal static string GetPingColored(this VRC.Player instance)
        {
            var ping = GetPing(instance);
            if (ping >= 80)
            {
                return $"<color=red>{ping}</color>";
            }
            else
            {
                if (ping <= 35)
                {
                    return $"<color=green>{ping}</color>";
                }
                else
                {
                    return $"<color=orange>{ping}</color>";
                }
            }
        }

        internal static List<string> UserIds = new List<string>();

        internal static void IsHacker(this VRC.Player Instance)
        {
            if (!Instance.field_Private_APIUser_0.IsOnMobile)
            {
                if (Instance.GetFrames() > 90)
                {
                    if (!PlayerUtils.UserIds.Contains(Instance.GetUserID()))
                    {
                        UserIds.Add(Instance.GetUserID());
                    }
                }
                if (Instance.GetFrames() < 0)
                {
                    if (!PlayerUtils.UserIds.Contains(Instance.GetUserID()))
                    {
                        UserIds.Add(Instance.GetUserID());
                    }
                }
                if (Instance.GetPing() > 1200)
                {
                    if (!PlayerUtils.UserIds.Contains(Instance.GetUserID()))
                    {
                        UserIds.Add(Instance.GetUserID());
                    }
                }
                if (Instance.GetPing() < -1)
                {
                    if (!PlayerUtils.UserIds.Contains(Instance.GetUserID()))
                    {
                        UserIds.Add(Instance.GetUserID());
                    }
                }
            }
        }

        internal static void IsHacker(this VRCPlayer Instance)
        {
            if (!Instance.GetAPIUser().IsOnMobile)
            {
                if (Instance.GetFrames() > 90)
                {
                    if (!PlayerUtils.UserIds.Contains(Instance.GetUserID()))
                    {
                        UserIds.Add(Instance.GetUserID());
                    }
                }
                if (Instance.GetFrames() < 0)
                {
                    if (!PlayerUtils.UserIds.Contains(Instance.GetUserID()))
                    {
                        UserIds.Add(Instance.GetUserID());
                    }
                }
                if (Instance.GetPing() > 1200)
                {
                    if (!PlayerUtils.UserIds.Contains(Instance.GetUserID()))
                    {
                        UserIds.Add(Instance.GetUserID());
                    }
                }
                if (Instance.GetPing() < -1)
                {
                    if (!PlayerUtils.UserIds.Contains(Instance.GetUserID()))
                    {
                        UserIds.Add(Instance.GetUserID());
                    }
                }
            }
        }

        internal static bool IsFlying(this VRC.Player Instance)
        {
            if (Instance.GetPlatform().Equals("Quest"))
            {
                return !Instance.GetVRCPlayerApi().IsPlayerGrounded();
            }
            if (Instance.GetVRCPlayerApi().GetVelocity().y < 0 && !Instance.GetVRCPlayerApi().IsPlayerGrounded())
            {
                return Instance.GetVRCPlayerApi().IsPlayerGrounded();
            }
            else
            {
                return !Instance.GetVRCPlayerApi().IsPlayerGrounded();
            }
            //return (Instance.GetVRCPlayerApi().GetVelocity().y < 0
            //    && !Instance.GetVRCPlayerApi().IsPlayerGrounded());
            //return !Instance.GetVRCPlayerApi().IsPlayerGrounded();
        }

        internal static bool IsFlying(this VRCPlayer Instance)
        {
            if (Instance.GetPlatform().Equals("Quest"))
            {
                return !Instance.GetVRCPlayerApi().IsPlayerGrounded();
            }
            if (Instance.GetVRCPlayerApi().GetVelocity().y > 1 && !Instance.GetVRCPlayerApi().IsPlayerGrounded())
            {
                return Instance.GetVRCPlayerApi().IsPlayerGrounded();
            }
            else
            {
                return !Instance.GetVRCPlayerApi().IsPlayerGrounded();
            }
            //return (Instance.GetVRCPlayerApi().GetVelocity().y < 0
            //    && !Instance.GetVRCPlayerApi().IsPlayerGrounded());
            //return !Instance.GetVRCPlayerApi().IsPlayerGrounded();
        }

        internal static bool GetIsBlocked(this VRCPlayer Instance)
        {
            return Instance.GetUserID() != APIUser.CurrentUser.id && ModerationHelper.GetModerationType(Instance.GetUserID()) == ModerationHelper.ModerationType.BlockU;
        }

        #endregion

        #region Ranks

        public static string GetRankColor(this APIUser Instance)
        {
            string playerRank = Instance.GetRank().ToLower();
            switch (playerRank.ToLower())
            {
                case "moderation user":
                    return "#5e0000";

                case "admin user":
                    return "#5e0000";

                case "legend":
                    return "#ff5e5e";

                case "veteran":
                    return "#fff821";

                case "trusted":
                    return "#a621ff";

                case "known":
                    return "#ffa200";

                case "user":
                    return "#00e62a";

                case "new user":
                    return "#00aeff";

                case "visitor":
                    return "#bababa";
            }
            return "#303030";
        }

        public static string GetRankColor(string rank)
        {
            switch (rank.ToLower().ToLower())
            {
                case "moderation user":
                    return "#5e0000";

                case "admin user":
                    return "#5e0000";

                case "legend":
                    return "#ff5e5e";

                case "veteran":
                    return "#fff821";

                case "trusted":
                    return "#a621ff";

                case "known":
                    return "#ffa200";

                case "user":
                    return "#00e62a";

                case "new user":
                    return "#00aeff";

                case "visitor":
                    return "#bababa";
            }
            return "#303030";
        }

        public static string GetRank(this APIUser Instance)
        {
            if (Instance.hasModerationPowers || Instance.tags.Contains("admin_moderator"))
                return "Moderation User";
            if (Instance.hasSuperPowers || Instance.tags.Contains("admin_"))
                return "Admin User";
            if (Instance.tags.Contains("system_legend") && Instance.tags.Contains("system_trust_legend") && Instance.tags.Contains("system_trust_trusted"))
                return "Legend";
            if (Instance.tags.Contains("system_trust_legend") && Instance.tags.Contains("system_trust_trusted"))
                return "Veteran";
            if (Instance.hasVeteranTrustLevel)
                return "Trusted";
            if (Instance.hasTrustedTrustLevel)
                return "Known";
            if (Instance.hasKnownTrustLevel)
                return "User";
            if (Instance.hasBasicTrustLevel || Instance.isNewUser)
                return "New User";
            if (Instance.hasNegativeTrustLevel || Instance.tags.Contains("system_probable_troll"))
                return "NegativeTrust";
            return Instance.hasVeryNegativeTrustLevel ? "VeryNegativeTrust" : "Visitor";
        }
        #endregion

        #region Module Based Methods
        internal static string GetTags(VRC.Player instance)
        {
            string results = string.Empty;

            if (instance.IsStaffMember())
                return "[<color=red>MOD</color>]";
            else
            {
                if (instance.IsWorldCreator())
                    results += $"[<color=#{ColorUtility.ToHtmlStringRGB(new Color32(255, 0, 220, 255))}>WC</color>]";

                if (instance.IsInstanceCreator() && !instance.IsWorldCreator())
                    results += $"[<color=#{ColorUtility.ToHtmlStringRGB(new Color32(76, 255, 0, 255))}>IC</color>]";

                if (instance.IsInstanceMaster() && !instance.IsWorldCreator() && !instance.IsInstanceCreator())
                    results += $"[<color=#{ColorUtility.ToHtmlStringRGB(new Color32(255, 106, 0, 255))}>M</color>]";

                return results;
            }
        }

        internal static string GetPlatformNP(this VRC.Player instance)
        {
            if (instance.GetVRCPlayerApi().IsUserInVR())
            {
                return instance.GetAPIUser().last_platform == "android" ? "<color=#1aff00>Q" : "<color=grey>VR";
            }
            return "<color=grey>PC";
        }

        public static string GetiFft(this VRC.Player player)
        {
            if (player.field_Private_APIUser_0.last_platform.ToLower() != "android")
            {
                if (player.field_Private_VRCPlayerApi_0.IsUserInVR())
                {
                    var ikType = player._vrcplayer.field_Private_VRCAvatarManager_0.field_Internal_IkController_0.field_Private_IkType_0;
                    if (ikType == IkController.IkType.SixPoint)
                    {
                        return "<color=blue>FBT</color>";
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        internal static string GetPlatform(this VRC.Player instance)
        {
            if (instance.prop_VRCPlayerApi_0.IsUserInVR())
            {
                if (instance.prop_APIUser_0.last_platform.ToLower() != "standalonewindows")
                {
                    return "Quest";
                }
                else
                {
                    return "VR";
                }
            }
            else
            {
                return "Desktop";
            }
        }

        internal static string GetPlatformColored(this VRC.Player instance)
        {
            if (instance.prop_VRCPlayerApi_0.IsUserInVR())
            {
                if (instance.GetPlatform() != "VR")
                {
                    return "<color=#1aff00>[Q]</color>";
                }
                else
                {
                    return "<color=grey>[VR]</color>";
                }
            }
            else
            {
                return "<color=grey>[PC]</color>";
            }
        }

        internal static string GetPlatformNamePlates(this VRC.Player instance)
        {
            if (instance.prop_VRCPlayerApi_0.IsUserInVR())
            {
                if (instance.GetPlatform() != "VR")
                {
                    return "Q";
                }
                else
                {
                    return "VR";
                }
            }
            else
            {
                return "PC";
            }
        }

        internal static string GetState(this VRC.Player instance)
        {
            if (instance.prop_APIUser_0.statusIsSetToAskMe)
            {
                return "Ask Me";
            }
            else if (instance.prop_APIUser_0.statusIsSetToDoNotDisturb)
            {
                return "Do Not Disturb";
            }
            else if (instance.prop_APIUser_0.statusIsSetToJoinMe)
            {
                return "Join Me";
            }
            else
            {
                return "Online";
            }
        }

        internal static string GetState(this APIUser instance)
        {
            if (instance.statusIsSetToAskMe)
            {
                return "Ask Me";
            }
            else if (instance.statusIsSetToDoNotDisturb)
            {
                return "Do Not Disturb";
            }
            else if (instance.statusIsSetToJoinMe)
            {
                return "Join Me";
            }
            else
            {
                return "Online";
            }
        }

        internal static string GetState(this VRCPlayer instance)
        {
            if (instance._player.prop_APIUser_0.statusIsSetToAskMe)
            {
                return "Ask Me";
            }
            else if (instance._player.prop_APIUser_0.statusIsSetToDoNotDisturb)
            {
                return "Do Not Disturb";
            }
            else if (instance._player.prop_APIUser_0.statusIsSetToJoinMe)
            {
                return "Join Me";
            }
            else
            {
                return "Online";
            }
        }
        #endregion

        #region Avi Utils

        public static VRC.Player GetPlayerByAviID(string id)
        {
            foreach (var plr in WorldUtils.GetPlayers())
            {
                if (plr.prop_ApiAvatar_0 != null)
                {
                    if (plr.prop_ApiAvatar_0.id == id)
                    {
                        return plr;
                    }
                }
            }
            return null;
        }

        #endregion
    }
}