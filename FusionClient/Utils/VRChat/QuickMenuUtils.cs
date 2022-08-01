using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using VRC;
using VRC.Core;
using VRC.UI;
using VRC.UI.Elements.Menus;
using Object = Il2CppSystem.Object;

namespace FC.Utils
{
    internal static class QuickMenuUtils
    {
        private static VRCUiManager vrcuimInstance;
        public static VRCUiManager GetVRCUiMInstance()
        {
            if (vrcuimInstance == null)
            {
                vrcuimInstance = VRCUiManager.prop_VRCUiManager_0;
            }
            return vrcuimInstance;
        }

        internal static QuickMenu GetQuickMenu()
        {
            return QuickMenu.prop_QuickMenu_0;
        }

        internal static void SelectPlayer(Player instance)
        {
            QuickMenu.prop_QuickMenu_0.Method_Public_Void_Player_0(instance);
        }

        internal static VRCPlayer GetVRCPlayerSelectedUser()
        {
            var a = UnityEngine.Object.FindObjectOfType<SelectedUserMenuQM>().field_Private_IUser_0;
            return GetPlayerByUserID(a.prop_String_0)._vrcplayer;
        }

        internal static Player GetPlayerSelectedUser()
        {
            var a = UnityEngine.Object.FindObjectOfType<SelectedUserMenuQM>().field_Private_IUser_0;
            return GetPlayerByUserID(a.prop_String_0).field_Private_Player_0.GetPlayer();
        }

        internal static Player GetPlayerByUserID(string userID)
        {
            return WorldUtils.GetPlayers().FirstOrDefault(p => p.prop_APIUser_0.id == userID);
        }

        internal static void OpenQM()
        {
            QuickMenu.prop_QuickMenu_0.Method_Private_Void_Boolean_0(true);
        }

        internal static void CloseQM()
        {
            QuickMenu.prop_QuickMenu_0.Method_Private_Void_Boolean_0(false);
        }

        internal static void ResizeCollider(Vector3 size)
        {
            QuickMenu.prop_QuickMenu_0.GetComponent<BoxCollider>().size = size;
        }

        internal static void ResetCollider()
        {
            QuickMenu.prop_QuickMenu_0.GetComponent<BoxCollider>().size = new Vector3(2517.34f, 1671.213f, 1f);
        }

        internal async static void LoadSprite(Image Instance, string url)
        {
            await GetRemoteTexture(Instance, url);
        }

        private static async Task<Texture2D> GetRemoteTexture(Image Instance, string url)
        {
            var www = UnityWebRequestTexture.GetTexture(url);
            var asyncOp = www.SendWebRequest();
            while (asyncOp.isDone == false)
                await Task.Delay(1000 / 30);//30 hertz

            if (www.isNetworkError || www.isHttpError)
            {
                return null;
            }
            else
            {
                var Sprite = new Sprite();
                Sprite = Sprite.CreateSprite(DownloadHandlerTexture.GetContent(www), new Rect(0, 0, DownloadHandlerTexture.GetContent(www).width, DownloadHandlerTexture.GetContent(www).height), Vector2.zero, 100 * 1000, 1000, SpriteMeshType.FullRect, Vector4.zero, false);
                Instance.sprite = Sprite;
                Instance.color = Color.white;
                return DownloadHandlerTexture.GetContent(www);
            }
        }

        internal async static void LoadSprite(Sprite Instance, string url)
        {
            await GetRemoteTexture(Instance, url);
        }

        private static async Task<Texture2D> GetRemoteTexture(Sprite Instance, string url)
        {
            var www = UnityWebRequestTexture.GetTexture(url);
            var asyncOp = www.SendWebRequest();
            while (asyncOp.isDone == false)
                await Task.Delay(1000 / 30);//30 hertz

            if (www.isNetworkError || www.isHttpError)
            {
                return null;
            }
            else
            {
                Instance = Sprite.CreateSprite(DownloadHandlerTexture.GetContent(www), new Rect(0, 0, DownloadHandlerTexture.GetContent(www).width, DownloadHandlerTexture.GetContent(www).height), Vector2.zero, 100 * 1000, 1000, SpriteMeshType.FullRect, Vector4.zero, false);
                return DownloadHandlerTexture.GetContent(www);
            }
        }
    }
}
