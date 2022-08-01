using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FC;
using FC.Utils;
using MelonLoader;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Playables;
using VRC.Core;
using VRC.UI;
using System.Reflection;
using FusionClient.AviShit;
using FusionClient.API.SM;
using FusionClient.Utils.VRChat;
using FC.Components;
using FusionClient.Core;
using FusionClient.Modules.Other;
using FusionClient.API;

namespace FusionClient.Modules
{
    class AvatarFavorites : FusionModule
    {
        public static Dictionary<int, SMList> FavlistDictonary = new();
        private static GameObject avatarPage;
        internal static PageAvatar currPageAvatar;
        private static GameObject PublicAvatarList;
        public static List<GameObject> TestAvatarButtons = new();
        public static List<UiVRCList> AvatarpageLists = new();

        public override void UI()
        {
            avatarPage = GameObject.Find("UserInterface/MenuContent/Screens/Avatar");
            PublicAvatarList = GameObject.Find("/UserInterface/MenuContent/Screens/Avatar/Vertical Scroll View/Viewport/Content/Public Avatar List");
            currPageAvatar = avatarPage.GetComponent<PageAvatar>();

            try
            {
                LoadList();
                Logs.Log("Successfully created Avatar Favorite Lists!", ConsoleColor.Cyan);
            }
            catch (Exception e) { Logs.Log("Creating Avatar Favorite Lists!\n" + e, ConsoleColor.Red); }

            try
            {
                // Create New Avi Fav List
                new SMButton(SMButton.SMButtonType.EditStatus, avatarPage.transform, -120, 375, "New List", AddNewList, 1.6f, 1);

                //Select Author
                var selectAuthor = new SMButton(SMButton.SMButtonType.EditStatus, avatarPage.transform, -120, 305, "Select Author", delegate
                {
                    VRCUiManager.prop_VRCUiManager_0.SelectAPIUser(currPageAvatar.field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0.authorId);
                }, 1.6f, 1);
                selectAuthor.GetText().resizeTextForBestFit = true;

                //Copy Avatar ID
                var copyID = new SMButton(SMButton.SMButtonType.EditStatus, avatarPage.transform, -120, 235, "Copy Avi ID", delegate
                {
                    try
                    {
                        Clipboard.SetText(currPageAvatar.field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0.id);
                        PopupUtils.InformationAlert("Successfully copied avatar's id to your clipboard!");
                    }
                    catch
                    {
                        PopupUtils.InformationAlert("Failed to copy avatar's id to your clipboard!");
                    }

                }, 1.6f, 1);
                copyID.GetText().resizeTextForBestFit = true;

                var downloadvrca = new SMButton(SMButton.SMButtonType.EditStatus, avatarPage.transform, -120, 165, "Download VRCA", delegate
                {
                    ModulesFunctions.DownloadVRCA(currPageAvatar.field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0);
                }, 1.6f, 1);
                downloadvrca.GetText().resizeTextForBestFit = true;
            }
            catch (Exception e)
            {
                Logs.Log("Creating Avi Favorite Buttons\n" + e, ConsoleColor.Cyan);
            }

            var listener = avatarPage.AddComponent<EnableDisableListener>();
            listener.OnEnabled += () =>
            {
                MelonCoroutines.Start(RefreshMenu(1f));
            };
        }

        public static void LoadList()
        {
            for (int i = 0; i < AviConfig.Instance.list.FavoriteLists.Count; i++)
            {
                AviFavoritesObjects.ModAviFavorites.FavoriteList list = AviConfig.Instance.list.FavoriteLists[i];
                if (!FavlistDictonary.ContainsKey(list.ID))
                {
                    var newlist = new SMList(APIStuff.GetSocialMenuInstance().transform.Find("Avatar/Vertical Scroll View/Viewport/Content"), list.name, list.ID);
                    var listofbuttons = new List<SMButton>
                    {
                        new(SMButton.SMButtonType.ChangeAvatar, newlist.UiVRCList.expandButton.gameObject.transform, 600, 0, "Fav/UnFav", delegate {
                            if (!list.Avatars.Exists(avi => avi.ID == currPageAvatar.field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0.id))
                                FavoriteAvatar(currPageAvatar.field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0, list.ID);
                            else
                                UnfavoriteAvatar(currPageAvatar.field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0, list.ID);
                        }, 2, 1),
                        new(SMButton.SMButtonType.ChangeAvatar, newlist.UiVRCList.expandButton.gameObject.transform, 750, 0, "Destroy", delegate
                        {
                            PopupUtils.AlertV2($"Woah There! By Destroying this list you are deleting it forever! Are you sure you want to do delete [{list.name}]?", "Delete", delegate
                            {
                                RemoveList(list.ID);
                                PopupUtils.HideCurrentPopUp();
                            }, "Cancel", PopupUtils.HideCurrentPopUp);
                        }, 2, 1),
                        new(SMButton.SMButtonType.ChangeAvatar, newlist.UiVRCList.expandButton.gameObject.transform, 900, 0, "Rename", delegate
                        {
                            PopupUtils.InputPopup("Set Name", "ERP Avatars", delegate (string text)
                            {
                                list.name = text;
                                newlist.Text.text = text;
                                PopupUtils.HideCurrentPopUp();
                            });
                        }, 2, 1)
                    };
                    listofbuttons.ForEach(b => b.SetActive(false));
                    newlist.UiVRCList.expandButton.onClick.AddListener(new Action(() =>
                    {
                        listofbuttons.ForEach(b => b.SetActive(!b.button.activeSelf));
                    }));
                    newlist.UiVRCList.extendRows = list.Rows;
                    newlist.UiVRCList.expandedHeight += 300 * (list.Rows - 2);
                    newlist.GameObject.GetComponent<UnityEngine.Canvas>().enabled = true;
                    FavlistDictonary.Add(list.ID, newlist);
                }
            }
            MelonCoroutines.Start(RefreshMenu(1f));
        }

        internal static void AddNewList()
        {
            var newID = AviConfig.Instance.list.FavoriteLists.Count;
            AviConfig.Instance.list.FavoriteLists.Add(new AviFavoritesObjects.ModAviFavorites.FavoriteList
            {
                ID = newID,
                Avatars = new List<AviFavoritesObjects.ModAviFav>(),
                Desciption = "",
                name = "[FC] List #" + newID
            });
            LoadList();
        }

        internal static void RemoveList(int ListID)
        {
            var ConfigList = AviConfig.Instance.list.FavoriteLists.FirstOrDefault(list => list.ID == ListID);
            var AvatarList = FavlistDictonary[ListID];
            if (ConfigList != null && AvatarList != null)
            {
                AviConfig.Instance.list.FavoriteLists.Remove(ConfigList);
                UnityEngine.Object.Destroy(AvatarList.GameObject);
                FavlistDictonary.Remove(ListID);
                Config.AvatarFavorites.Save();
                MelonCoroutines.Start(RefreshMenu(1f));
            }
        }

        internal static void FavoriteAvatar(ApiAvatar avatar, int ListID)
        {
            var avatarobject = avatar.ToModFavAvi();
            if (GetConfigList(ListID) != null)
            {
                if (!GetConfigList(ListID).Avatars.Exists(avi => avi.ID == avatar.id))
                {
                    GetConfigList(ListID).Avatars.Insert(0, avatarobject);
                }
            }
            MelonCoroutines.Start(RefreshMenu(1f));
            Config.AvatarFavorites.Save();
        }

        internal static void UnfavoriteAvatar(ApiAvatar avatar, int ListID)
        {
            if (GetConfigList(ListID) != null)
            {
                GetConfigList(ListID).Avatars.Remove(GetConfigList(ListID).Avatars.FirstOrDefault(avi => avi.ID == avatar.id));
            }
            Config.AvatarFavorites.Save();
        }

        private static AviFavoritesObjects.ModAviFavorites.FavoriteList GetConfigList(int ID)
        {
            return AviConfig.Instance.list.FavoriteLists.FirstOrDefault(List => List.ID == ID);
        }

        private static IEnumerator RefreshMenu(float v)
        {
            for (int i = 0; i < AviConfig.Instance.list.FavoriteLists.Count; i++)
            {
                AviFavoritesObjects.ModAviFavorites.FavoriteList list = AviConfig.Instance.list.FavoriteLists[i];
                yield return new WaitForSeconds(v);
                var list2 = FavlistDictonary[list.ID];
                if (list2 != null)
                {
                    Il2CppSystem.Collections.Generic.List<ApiAvatar> AvatarList = new();
                    list.Avatars.ForEach(avi => AvatarList.Add(avi.ToApiAvatar()));
                    list2.RenderElement(AvatarList);
                    list2.Text.text = $"{list.name} [{AvatarList.Count}] {list.Desciption}";
                }
            }
        }
    }
}