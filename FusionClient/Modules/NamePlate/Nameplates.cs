using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FusionClient;
using FC;
using FC.Utils;
using FC.Components;
using MelonLoader;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using Player = VRC.Player;
using System.Threading.Tasks;
using FusionClient.Core;
using FusionClient.Utils.Manager;
using UnityEngine.Networking;
using System.IO;
using Newtonsoft.Json;
using System;
using Object = UnityEngine.Object;

namespace FusionClient.Modules
{
    class Nameplates : FusionModule
    {
        public static List<FusionTag> tags = new();
        public static List<string> NovaTags = new();
        public static List<string> EvoTags = new();
        public static List<string> yoinkers = new();
        public static List<string> Boosters = new();

        internal static void CreateNameplate(Player player)
        {
            if (player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/FC Nameplate") != null) return;
            var plate = Object.Instantiate(player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Quick Stats"), player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents"), false);
            plate.name = "FC Nameplate";
            var comp = plate.gameObject.AddComponent<FCNameplate>();
            comp.vrcPlayer = player._vrcplayer;
            for (var i = plate.transform.childCount; i > 0; i--)
            {
                var child = plate.transform.GetChild(i - 1);
                if (child.name == "Trust Text")
                {
                    comp.text = child.GetComponent<TextMeshProUGUI>();
                    child.GetComponent<TextMeshProUGUI>().color = Color.white;
                }
                else Object.Destroy(child.gameObject);
            }
            plate.gameObject.SetActive(true);
            player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Quick Stats").GetComponent<RectTransform>().localPosition = new Vector3(0, 90, 0);
        }

        internal static void DeleteNameplate(Player player)
        {
            if (player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/FC Nameplate") == null) return;
            Object.Destroy(player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/FC Nameplate").gameObject);
            player.transform.Find("Player Nameplate/Canvas/Avatar Progress").localPosition = new Vector3(0, -15, 0);
        }

        internal static IEnumerator DelayedRefresh()
        {
            Refresh();
            yield return new WaitForSeconds(15);
        }

        public static void AddTag()
        {
        }

        public static void Enable(Player player)
        {
            var transform = player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents");
            var transform2 = transform.Find("Quick Stats");
            if (Config.Main.CustomNamePlates)
            {
                player._vrcplayer.field_Public_PlayerNameplate_0.field_Public_Graphic_0.color = ColorManager.HexToColor(player.GetAPIUser().GetRankColor());
                player._vrcplayer.field_Public_PlayerNameplate_0.field_Public_Graphic_2.color = ColorManager.HexToColor(player.GetAPIUser().GetRankColor());
                player._vrcplayer.field_Public_PlayerNameplate_0.field_Public_Graphic_4.color = ColorManager.HexToColor(player.GetAPIUser().GetRankColor());
                player._vrcplayer.field_Public_PlayerNameplate_0.field_Public_Graphic_6.color = ColorManager.HexToColor(player.GetAPIUser().GetRankColor());
                player._vrcplayer.field_Public_PlayerNameplate_0.field_Public_Graphic_7.color = ColorManager.HexToColor(player.GetAPIUser().GetRankColor());
                player._vrcplayer.field_Public_PlayerNameplate_0.field_Public_Graphic_8.color = ColorManager.HexToColor(player.GetAPIUser().GetRankColor());
            }
            if (Config.Main.CustomNamePlateTags)
            {
                var num = 0;
                for (; ; )
                {
                    var transform3 = transform.Find(string.Format("FCTag{0}", num));
                    if (transform3 == null)
                    {
                        break;
                    }
                    transform3.gameObject.active = false;
                    num++;
                }
                var num2 = 0;
                SetTag(ref num2, transform2, transform, ColorManager.HexToColor(player.GetAPIUser().GetRankColor()), player.prop_APIUser_0.GetRank());
                transform2.localPosition = new Vector3(0f, 90, 0f);
                // Yoinker Tags
                foreach (var _ in yoinkers.Where(t => t == player._vrcplayer.GetUserID()))
                {
                    SetTag(ref num2, transform2, transform, Color.red, "YOINKER");
                }
                // Nova Tags
                foreach (var _ in NovaTags.Where(t => t == player._vrcplayer.GetUserID()))
                {
                    SetTag(ref num2, transform2, transform, Color.red, "NovaCore User");
                }
                // Evolve Tags
                foreach (var _ in EvoTags.Where(t => t == player._vrcplayer.GetUserID()))
                {
                    SetTag(ref num2, transform2, transform, Color.red, "Evolve User");
                }
                // Custom Tags
                foreach (var t in tags.Where(t => t.UserID == player._vrcplayer.GetUserID()))
                {
                    foreach (var t2 in t.Tags)
                    {
                        SetTag(ref num2, transform2, transform, Color.white, t2);
                    }
                }
                // Blocked Tags
                foreach (var t in FusionClient.Startup.Hooks.PhotonHook.PhotonHandlers.Photon_PlayerModerationHandler.BlockedYouPlayers.Where(t => t == player._vrcplayer.GetUserID()))
                {
                    SetTag(ref num2, transform2, transform, Color.white, "<color=red>Blocked</color>");
                }
                if (player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/FCTag0"))
                {
                    player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/FCTag0").localPosition = new Vector3(0f, 60, 0f);
                    if (player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/FCTag1"))
                    {
                        player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/FCTag1").localPosition = new Vector3(0f, 120, 0f);
                        if (player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/FCTag2"))
                        {
                            player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/FCTag2").localPosition = new Vector3(0f, 150, 0f);
                            if (player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/FCTag3"))
                            {
                                player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/FCTag3").localPosition = new Vector3(0f, 180, 0f);
                                if (player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/FCTag4"))
                                {
                                    player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/FCTag4").localPosition = new Vector3(0f, 210, 0f);
                                    if (player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/FCTag5"))
                                    {
                                        player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/FCTag5").localPosition = new Vector3(0f, 230, 0f);
                                        if (player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/FCTag6"))
                                        {
                                            player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/FCTag6").localPosition = new Vector3(0f, 260, 0f);
                                            if (player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/FCTag6"))
                                            {
                                                player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/FCTag6").localPosition = new Vector3(0f, 290, 0f);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (Config.Main.CustomNamePlates)
            {
                transform2.GetComponent<ImageThreeSlice>().color = ColorManager.HexToColor(player.GetAPIUser().GetRankColor());
                transform2.GetComponent<ImageThreeSlice>().prop_Sprite_0 = AssetBundleManager.NamePlate;
                transform.Find("Main/Background").GetComponent<ImageThreeSlice>().prop_Sprite_0 = AssetBundleManager.NamePlate;
                transform.Find("Main/Background").GetComponent<ImageThreeSlice>().color = ColorManager.HexToColor(player.GetAPIUser().GetRankColor());
                transform.Find("Icon/Background").GetComponent<UnityEngine.UI.Image>().sprite = AssetBundleManager.NamePlateIco;
                transform.Find("Icon/Background").GetComponent<UnityEngine.UI.Image>().color = ColorManager.HexToColor(player.GetAPIUser().GetRankColor());
            }

        }

        public static void Disable(Player player)
        {
            var transform = player.transform.Find("Player Nameplate/Canvas/Nameplate/Contents");
            var transform2 = transform.Find("Main");
            var transform3 = transform.Find("Icon");
            var transform4 = transform.Find("Quick Stats");
            transform2.Find("Background").GetComponent<ImageThreeSlice>().color = Color.white;
            transform2.Find("Pulse").GetComponent<ImageThreeSlice>().color = Color.white;
            transform2.Find("Glow").GetComponent<ImageThreeSlice>().color = Color.white;
            transform3.Find("Background").GetComponent<Image>().color = Color.white;
            transform3.Find("Pulse").GetComponent<Image>().color = Color.white;
            transform3.Find("Glow").GetComponent<Image>().color = Color.white;
            transform4.GetComponent<ImageThreeSlice>().color = Color.white;
            var num = 0;
            for (; ; )
            {
                var transform5 = transform.Find($"FCTag{num}");
                if (transform5 == null) break;
                transform5.gameObject.active = false;
                num++;
            }
            transform4.localPosition = new Vector3(0f, 30f, 0f);
        }

        public static void Refresh()
        {
            if (!Config.Main.NamePlates) return;
            var field_Private_Static_PlayerManager_ = PlayerManager.field_Private_Static_PlayerManager_0;
            if (field_Private_Static_PlayerManager_.field_Private_List_1_Player_0 == null) return;
            foreach (var player in PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0)
            {
                if (player.field_Private_VRCPlayerApi_0 != null && player.field_Private_APIUser_0 != null)
                {
                    if (Config.Main.NamePlates)
                    {
                        Enable(player);
                    }
                    else
                    {
                        Disable(player);
                    }
                }
            }
        }

        private static Transform MakeTag(Transform stats, int index)
        {
            var transform = Object.Instantiate(stats, stats.parent, false);
            transform.name = $"FCTag{index}";
            transform.localPosition = new Vector3(0f, 30 * (index + 1), 0f);
            transform.gameObject.active = true;
            Transform result = null;
            for (var i = transform.childCount; i > 0; i--)
            {
                var child = transform.GetChild(i - 1);
                if (child.name == "Trust Text") result = child;
                else Object.Destroy(child.gameObject);
            }
            transform.GetComponent<ImageThreeSlice>()._sprite = AssetBundleManager.NamePlate;
            return result;
        }

        private static void SetTag(ref int stack, Transform stats, Transform contents, Color color, string content)
        {
            var transform = contents.Find($"FCTag{stack}");
            Transform transform2;
            if (transform == null)
            {
                transform2 = MakeTag(stats, stack);
            }
            else
            {
                transform.gameObject.SetActive(true);
                transform2 = transform.Find("Trust Text");
            }
            var component = transform2.GetComponent<TextMeshProUGUI>();
            component.color = color;
            component.text = content;
            stack++;
        }

        public static IEnumerator FetchTags()
        {
            tags.Clear();
            var unityWebRequest = UnityWebRequest.Get("https://api.vrcmods.xyz/api/tags/fetch");
            unityWebRequest.SetRequestHeader("FCAuth", File.ReadAllText("Fusion Client\\Auth.FC").Trim());
            unityWebRequest.method = "GET";
            unityWebRequest.SendWebRequest();
            while (!unityWebRequest.isDone) yield return null;
            if (string.IsNullOrEmpty(unityWebRequest.error))
            {
                tags = JsonConvert.DeserializeObject<List<FusionTag>>(unityWebRequest.downloadHandler.text);
            }
            else
            {
                Console.WriteLine(unityWebRequest.error);
            }
            yield break;
        }
    }

    public class FusionTag
    {
        [JsonProperty("UserID")]
        public string UserID { get; set; }
        [JsonProperty("Tags")]
        public List<string> Tags { get; set; }
    }
}