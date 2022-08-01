using FC;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace FusionClient.Modules
{
    class Hud : FusionModule
    {
        private static Text log;
        private static List<string> lines = new List<string>();

        public override void UI()
        {
            GameObject gameObject = new GameObject("FusionClient's Hud Logs");
            log = gameObject.AddComponent<Text>();

            gameObject.transform.SetParent(GameObject.Find("UserInterface/UnscaledUI/HudContent_Old/Hud").transform, false);
            gameObject.transform.localPosition = new Vector3(85, -250);

            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(850, 40);

            log.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            log.horizontalOverflow = HorizontalWrapMode.Wrap;
            log.verticalOverflow = VerticalWrapMode.Overflow;
            log.alignment = TextAnchor.LowerRight;
            log.fontStyle = FontStyle.Bold;
            log.supportRichText = true;
            log.fontSize = 27;
        }

        public static void Display(string text, float duration) => MelonCoroutines.Start(LogAndRemove(text, duration));

        private static IEnumerator LogAndRemove(string text, float duration)
        {
            if (log is null) yield break;
            lines.Add(text);
            log.text = string.Join("\n", lines);
            yield return new WaitForSecondsRealtime(duration);
            lines.Remove(text);
            log.text = string.Join("\n", lines);
            ClearDisplay();
        }

        public static void ClearDisplay()
        {
            lines.Clear();
            log.text = "";
        }
    }
}
