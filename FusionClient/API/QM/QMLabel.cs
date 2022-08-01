using TMPro;
using UnityEngine;
using VRC.UI.Core.Styles;

namespace FusionClient.API.QM
{
    public class QMLabel
    {
        protected GameObject gameObject;
        protected TextMeshProUGUI text;

        public QMLabel(QMNestedButton location, float posX, float posY, string labelText, Color? textColor)
        {
            Initialize(location.GetMenuObject().transform, posX, posY, labelText, textColor);
        }

        public QMLabel(QMTabMenu location, float posX, float posY, string labelText, Color? textColor)
        {
            Initialize(location.GetMenuObject().transform, posX, posY, labelText, textColor);
        }

        public QMLabel(Transform location, float posX, float posY, string labelText, Color? textColor)
        {
            Initialize(location, posX, posY, labelText, textColor);
        }

        private void Initialize(Transform location, float posX, float posY, string labelText, Color? textColor)
        {
            gameObject = UnityEngine.Object.Instantiate(GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Header_QuickLinks/LeftItemContainer/Text_Title"), location, false);
            gameObject.name = $"{BlazesAPI.Identifier}-QMLabel-{APIStuff.RandomNumbers()}";
            text = gameObject.GetComponent<TextMeshProUGUI>();
            text.alignment = TextAlignmentOptions.Center;
            text.autoSizeTextContainer = true;
            text.enableWordWrapping = false;
            text.fontSize = 32;
            text.richText = true;
            Object.Destroy(gameObject.GetComponent<StyleElement>());
            SetPosition(new Vector2(posX, posY));
            SetText(labelText);
            if (textColor != null) SetTextColor((Color)textColor);
            BlazesAPI.allQMLabels.Add(this);
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public TextMeshProUGUI GetText()
        {
            return text;
        }

        public void SetText(string newText)
        {
            text.text = newText;
        }

        public void SetTextColor(Color newColor)
        {
            text.color = newColor;
        }

        public void SetPosition(Vector2 newPosition)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = newPosition;
        }

        public void SetAlignment(TextAlignmentOptions option)
        {
            text.alignment = option;
        }

        public void SetFontSize(float size)
        {
            text.fontSize = size;
        }

        public void DestroyMe()
        {
            try
            {
                UnityEngine.Object.Destroy(gameObject);
                BlazesAPI.allQMLabels.Remove(this);
            }
            catch { }
        }
    }
}
