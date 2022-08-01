using FC.Utils.Managers;
using FusionClient.API;
using FusionClient.API.SM;
using FusionClient.Core;
using FusionClient.Utils.Objects.Mod;
using FusionClient.Utils.VRChat;
using MelonLoader;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using VRC.Core;
using VRC.UI;

namespace FusionClient.Modules.Other
{
    public class AvatarSearch
    {
        public static List<ApiAvatar> avatars = new();
        public static List<List<ApiAvatar>> SortedLists = new();
        public static List<FusionAvatar> foundAvatars = new List<FusionAvatar>();
        private static SMList searchList;
        private static SMButton backPage;
        private static SMButton nextPage;
        private static SMText currentLabel;
        public static bool QuickCheck = false;
        public static int currentPage = 0;
        private static GameObject avatarPage;

        public static void InitializeUI()
        {
            avatarPage = GameObject.Find("UserInterface/MenuContent/Screens/Avatar");
            searchList = new SMList(APIStuff.GetSocialMenuInstance().transform.Find("Avatar/Vertical Scroll View/Viewport/Content"), "Fusion Search Results");
            searchList.GetText().supportRichText = true;
            searchList.GetUiVRCList().hideWhenEmpty = true;
            searchList.GetUiVRCList().clearUnseenListOnCollapse = false;
            searchList.GetUiVRCList().expandedHeight *= 1.8f;
            searchList.GetGameObject().GetComponent<UiAvatarList>().clearUnseenListOnCollapse = false;
            searchList.GetGameObject().GetComponent<UiAvatarList>().extendRows = 3;

            backPage = new SMButton(SMButton.SMButtonType.ChangeAvatar, searchList.GetUiVRCList().expandButton.gameObject.transform, 660, 0, "<-", delegate
            {
                if (currentPage == 0) return;
                currentPage--;
                currentLabel.SetText($"Page {currentPage + 1}/{SortedLists.Count}");
                Il2CppSystem.Collections.Generic.List<ApiAvatar> newList = new();
                foreach (var avi in SortedLists[currentPage]) { newList.Add(avi); }
                searchList.RenderElement(newList);
            }, 3, 1);
            currentLabel = new SMText(searchList.GetUiVRCList().expandButton.gameObject.transform, 800, 0, 150, 75, "Page 0/0");
            nextPage = new SMButton(SMButton.SMButtonType.ChangeAvatar, searchList.GetUiVRCList().expandButton.gameObject.transform, 950, 0, "->", delegate
            {
                if (currentPage + 1 == SortedLists.Count) return;
                currentPage++;
                currentLabel.SetText($"Page {currentPage + 1}/{SortedLists.Count}");
                Il2CppSystem.Collections.Generic.List<ApiAvatar> newList = new();
                foreach (var avi in SortedLists[currentPage]) { newList.Add(avi); }
                searchList.RenderElement(newList);
            }, 3, 1);
            var SearchByName = new SMButton(SMButton.SMButtonType.EditStatus, avatarPage.gameObject.transform, 1645, 290, "By Name", delegate
            {
                try
                {
                    PopupUtils.InputPopup("Search", "Sneefff", delegate (string s)
                    {
                        SearchByAvatarName(s);
                    });
                }
                catch { }
            }, 1.6f, 1);
            SearchByName.GetText().resizeTextForBestFit = true;
            var SearchByID = new SMButton(SMButton.SMButtonType.EditStatus, avatarPage.gameObject.transform, 1645, 220, "By ID", delegate
            {
                try
                {
                    PopupUtils.InputPopup("Search", "avtr_0000000000-0000-000-0000000000", delegate (string s)
                    {
                        SearchByAvatarID(s);
                    });
                }
                catch { }
            }, 1.6f, 1);
            SearchByID.GetText().resizeTextForBestFit = true;
            var SearchByAuthorI = new SMButton(SMButton.SMButtonType.EditStatus, avatarPage.gameObject.transform, 1645, 150, "By Author ID", delegate
            {
                try
                {
                    PopupUtils.InputPopup("Search", "Usr_0000000000-0000-000-0000000000", delegate (string s)
                    {
                        SearchByAuthorID(s);
                    });
                }
                catch { }
            }, 1.6f, 1);
            SearchByAuthorI.GetText().resizeTextForBestFit = true;
            var SearchByAuthornam = new SMButton(SMButton.SMButtonType.EditStatus, avatarPage.gameObject.transform, 1645, 80, "By Author Name", delegate
            {
                try
                {
                    PopupUtils.InputPopup("Search", "TurboVirgin", delegate (string s)
                    {
                        SearchByAuthorName(s);
                    });
                }
                catch { }
            }, 1.6f, 1);
            SearchByAuthornam.GetText().resizeTextForBestFit = true;
        }

        public static void SearchByAvatarID(string query)
        {
            ProcessAPICall("AvatarID", query).Start();
        }

        public static void SearchByAvatarName(string query)
        {
            ProcessAPICall("AvatarName", query).Start();
        }

        public static void SearchByAuthorID(string query)
        {
            ProcessAPICall("AuthorID", query).Start();
        }

        public static void SearchByAuthorName(string query)
        {
            ProcessAPICall("AuthorName", query).Start();
        }

        public static IEnumerator ProcessAPICall(string searchType, string query)
        {
            var request = UnityWebRequest.Put("https://api.vrcmods.xyz/api/avatars/search", JsonConvert.SerializeObject(new
            {
                SearchBy = searchType,
                Query = query,
            }));
            request.method = "POST";
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("FCAuth", File.ReadAllText("Fusion Client\\Auth.FC").Trim());
            request.SendWebRequest();
            while (!request.isDone) yield return null;

            if (string.IsNullOrWhiteSpace(request.error))
            {
                var results = JsonConvert.DeserializeObject<List<FusionAvatar>>(request.downloadHandler.text);
                avatars.Clear();
                foreach (var a in results)
                {
                    avatars.Add(FusionAvatar.ToApiAvatar(a));
                }
                searchList.GetText().supportRichText = true;
                //searchList.GetUiVRCList().expandedHeight += 300 * (3 - 2);
                searchList.GetUiVRCList().extendRows = 4;
                searchList.GetUiVRCList().startExpanded = false;
                SortedLists = avatars.ChunkBy(150);
                Il2CppSystem.Collections.Generic.List<ApiAvatar> firstList = new();
                foreach (var avi in SortedLists[0])
                {
                    firstList.Add(avi);
                }
                searchList.GetText().supportRichText = true;
                foreach (var avatar in avatars.ToArray().Where(a => a.releaseStatus.ToLower().Equals("private")))
                {
                    avatar.name = $"<color=red>[P]</color> {avatar.name}";
                }
                searchList.GetGameObject().SetActive(true);
                searchList.RenderElement(firstList);
                currentPage = 0;
                currentLabel.SetText($"Page {currentPage + 1}/{SortedLists.Count}");
                searchList.GetText().text = $"Fusion Search - Found: <color=yellow>{avatars.Count}</color>";
            }
            else
            {
                Console.WriteLine(request.error);
            }
            yield break;
        }
    }
}
