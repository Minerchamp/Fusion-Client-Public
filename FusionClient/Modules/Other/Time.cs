using System.Reflection;
using TMPro;
using UnityEngine;
using VRC.UI.Elements;
using FC;
using FC.Utils;
using FusionClient.Core;

namespace FusionClient.Modules
{
    class GetTime : FusionModule
    {
        [Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]

        public static GameObject Panel;
        public static TextMeshProUGUI Text;

        public override void UI()
        {
            var obj = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMNotificationsArea/DebugInfoPanel");
            Panel = Object.Instantiate(obj, obj.transform.parent, false);
            Panel.name = "FCTimePanel";
            Panel.GetComponent<RectTransform>().anchoredPosition = new Vector2(614, 0);
            Text = Panel.transform.Find("Panel/Text_FPS").GetComponent<TextMeshProUGUI>();
            Text.color = Color.white;
            Panel.transform.Find("Panel/Text_FPS").GetComponent<RectTransform>().sizeDelta = new Vector2(330, 0);
            Panel.transform.Find("Panel/Text_FPS").GetComponent<RectTransform>().anchoredPosition = new Vector2(-165, 0);
            Panel.transform.Find("Panel/Text_FPS").GetComponent<RectTransform>().anchoredPosition = new Vector2(-165, 0);
            //Panel.transform.localPosition = new Vector3(-512, 0, 0);
            Object.Destroy(Panel.transform.Find("Panel/Text_Ping"));
            Object.Destroy(Panel.GetComponent<DebugInfoPanel>());
            Panel.AddComponent<GetTimeLeft>();
            Panel.transform.SetSiblingIndex(1);
            Panel.SetActive(Config.Main.ExpirePanel);
        }
    }
}