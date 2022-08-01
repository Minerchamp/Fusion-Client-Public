using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using Newtonsoft.Json;
using UnityEngine;
using VRC.Core;
using VRC.UI;
using FC;
using FC.Utils;
using FC.Components;
using FusionClient.API.SM;
using FusionClient.API;
using FusionClient.Core;

namespace FusionClient.Modules
{
    internal class SeenAvatars : FusionModule
    {
        public override void UI()
        {
            try
            {
                SeenAvatars.SAvatars = new SMList(APIStuff.GetSocialMenuInstance().transform.Find("Avatar/Vertical Scroll View/Viewport/Content"), "᲼FC Seen Avatars");
                SeenAvatars.AvatarObjects = JsonConvert.DeserializeObject<System.Collections.Generic.List<AvatarObject>>(File.ReadAllText("Fusion Client\\Misc\\SeenAvatars.json"));
            }
            catch { }
        }

        public static IEnumerator RefreshMenu()
        {
            for (; ; )
            {
                try
                {
                    if (GameObject.Find("UserInterface/MenuContent/Screens/Avatar"))
                    {
                        Il2CppSystem.Collections.Generic.List<ApiAvatar> avilist = new Il2CppSystem.Collections.Generic.List<ApiAvatar>();
                        SeenAvatars.AvatarObjects.ForEach(delegate (AvatarObject avi)
                        {
                            avilist.Add(avi.ToApiAvatar());
                        });
                        SeenAvatars.SAvatars.RenderElement(avilist);
                    }
                }
                catch { }
                yield return new WaitForSecondsRealtime(10);
            }
        }

        public static IEnumerator ClearMenu()
        {
            try
            {
                File.Delete("Fusion Client\\Misc\\SeenAvatars.json");
                AvatarObjects.Clear();
                string contents = JsonConvert.SerializeObject(new System.Collections.Generic.List<AvatarObject>(), Formatting.Indented);
                File.WriteAllText("Fusion Client\\Misc\\SeenAvatars.json", contents);
                Il2CppSystem.Collections.Generic.List<ApiAvatar> avatarList = new Il2CppSystem.Collections.Generic.List<ApiAvatar>();
                SeenAvatars.SAvatars.RenderElement(avatarList);
                yield break;
            }
            catch { }
        }


        internal static void AddAvatar(ApiAvatar avatar)
        {
            try
            {
                if (!SeenAvatars.AvatarObjects.Exists((AvatarObject avi) => avi.id == avatar.id))
                {
                    SeenAvatars.AvatarObjects.Insert(0, new AvatarObject(avatar));
                }
                string contents = JsonConvert.SerializeObject(SeenAvatars.AvatarObjects, Formatting.Indented);
                File.WriteAllText("Fusion Client\\Misc\\SeenAvatars.json", contents);
            }
            catch { }
        }

        private static SMList SAvatars;

        public static System.Collections.Generic.List<AvatarObject> AvatarObjects = new System.Collections.Generic.List<AvatarObject>();

        private static GameObject avatarPage;

        private static PageAvatar currPageAvatar;

        private static GameObject PublicSAvatars;

        public static bool QuickCheck = false;
    }
}
