using FC.Utils;
using FusionClient.AviShit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRC;
using VRC.Core;
using VRC.SDKBase;
using VRC.UI;
using UnityEngine;

namespace FC.Utils
{
    internal static class AvatarManager
    {

        #region Api Avatar Quest
        internal static ApiAvatar GetApiAvatarQuest(this VRCPlayer instance)
        {
            return instance.prop_ApiAvatar_1;
        }

        internal static ApiAvatar GetApiAvatarQuest(this VRC.Player instance)
        {
            return instance._vrcplayer.prop_ApiAvatar_1;
        }
        #endregion

        #region Platforms
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

        #region Change To Avatar
        //internal static void ChangeToAvatar(string AvatarID)
        //{
        //    new ApiAvatar() { id = AvatarID }.Get(new Action<ApiContainer>(x =>
        //    {
        //        APIStuff.GetSocialMenuInstance().transform.Find("Avatar").GetComponent<PageAvatar>().field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0 = x.Model.Cast<ApiAvatar>();
        //        APIStuff.GetSocialMenuInstance().transform.Find("Avatar").GetComponent<PageAvatar>().ChangeToSelectedAvatar();
        //    }), new Action<ApiContainer>(x =>
        //    {
        //        Logs.Msg($"Failed to change to avatar: {AvatarID} | Error Message: {x.Error}", ConsoleColor.Red);
        //    }), null, false);
        //}

        //internal static void ChangeToAvatar(this ApiAvatar instance)
        //{
        //    new ApiAvatar() { id = instance.id }.Get(new Action<ApiContainer>(x =>
        //    {
        //        APIStuff.GetSocialMenuInstance().transform.Find("Avatar").GetComponent<PageAvatar>().field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0 = x.Model.Cast<ApiAvatar>();
        //        APIStuff.GetSocialMenuInstance().transform.Find("Avatar").GetComponent<PageAvatar>().ChangeToSelectedAvatar();
        //    }), new Action<ApiContainer>(x =>
        //    {
        //        Logs.Msg($"Failed to change to avatar: {instance.id} | Error Message: {x.Error}", ConsoleColor.Red);
        //    }), null, false);
        //}
        #endregion

        #region Reload Avatar
        internal static void ReloadAvatar(this VRCPlayer Instance)
        {
            VRCPlayer.Method_Public_Static_Void_APIUser_0(Instance.GetAPIUser());
        }
        #endregion

        #region Conversions

        public class VRCUnityPackage
        {
            public string id { get; set; }
            public DateTime created_at { get; set; }
            public string assetUrl { get; set; }
            public string impostorUrl { get; set; }
            public string unityVersion { get; set; }
            public int unitySortNumber { get; set; }
            public int assetVersion { get; set; }
            public string platform { get; set; }
        }
        #endregion
    }
}
