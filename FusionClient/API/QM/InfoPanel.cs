using FusionClient.API;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace FC.Utils.API.QM
{
    [Obfuscation(Exclude = true, ApplyToMembers = true, StripAfterObfuscation = true)]
    public class InfoPanel
    {
        public GameObject InfoObject;
        public Text InfoText;
        public Image InfoBackground;

        public InfoPanel(Transform location, float PosX, float PosY, float SizeX, float SizeY, string panelText)
        {
            Initialize(location, PosX, PosY, SizeX, SizeY, panelText);
        }

        private void Initialize(Transform location, float PosX, float PosY, float SizeX, float SizeY, string panelText)
        {
            InfoObject = Object.Instantiate(APIStuff.GetInfoPanelTemplate(), location);
            InfoObject.name = $"InfoPanel-{APIStuff.RandomNumbers()}";
            InfoText = InfoObject.GetComponentInChildren<Text>();
            InfoBackground = InfoObject.GetComponentInChildren<Image>();
            SetSize(new Vector2(SizeX, SizeY));
            SetLocation(new Vector3(PosX, PosY, 1));
            SetText(panelText);
        }

        public void SetSize(Vector2 size)
        {
            InfoObject.GetComponent<RectTransform>().sizeDelta = size;
            InfoObject.transform.Find("Text").GetComponent<RectTransform>().sizeDelta = new Vector2(-25, 0);
        }

        public void SetLocation(Vector3 location)
        {
            InfoObject.GetComponent<RectTransform>().anchoredPosition = location;
        }

        public void SetText(string text)
        {
            InfoText.text = text;
        }

        public void ToggleBackground(bool state)
        {
            InfoObject.GetComponentInChildren<Image>().enabled = state;
        }

        public void SetActive(bool state)
        {
            InfoObject.SetActive(state);
        }
    }
}