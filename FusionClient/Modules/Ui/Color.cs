using FC;
using FC.Utils;
using FC.Utils.API.QM;
using FusionClient.Core;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Color = UnityEngine.Color;
using FontStyle = UnityEngine.FontStyle;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;

namespace FusionClient.Modules
{
    internal class Color_Edit
    {
        private static List<Image> MainImageColor;
        private static List<Image> DimmerImageColor;
        private static List<Image> DarkerImageColor;
        private static List<Text> SecondaryColor;
        public static byte[] MenuPic;
        public static Bitmap MenuPicPNG;
        public static byte[] MenuPicV2;
        public static byte[] MenuDebug;
        public static InfoPanel DebugPanel;

        public static Sprite LoadSpriteFromBytes(byte[] bytes)
        {
            Texture2D tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            if (!ImageConversion.LoadImage(tex, bytes))
            {
                Logs.Log("Failed to load sprite", ConsoleColor.Red);
                return null;
            }
            tex.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            tex.wrapMode = TextureWrapMode.Clamp;

            var rect = new Rect(0.0f, 0.0f, tex.width, tex.height);
            var anchor = new Vector2(0.5f, 0.5f);
            var border = new Vector4();
            Sprite sprite = Sprite.CreateSprite_Injected(tex, ref rect, ref anchor, 100.0f, 0, 0, ref border, false);
            sprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            return sprite;
        }

        public static byte[] ConverterPNG(System.Drawing.Image x)
        {
            ImageConverter _imageConverter = new ImageConverter();
            byte[] xByte = (byte[])_imageConverter.ConvertTo(x, typeof(byte[]));
            return xByte;
        }

        internal static IEnumerator MenuColorEditSM()
        {
            yield return new WaitForSeconds(0.3f);
            Color color = MenuColor();
            Color SecondColor = TextColor();
            Color color2 = new Color(color.r, color.g, color.b, 0.4f);
            new Color(color.r / 0.75f, color.g / 0.75f, color.b / 0.75f);
            Color color3 = new Color(color.r / 0.75f, color.g / 0.75f, color.b / 0.75f, 0.9f);
            Color color4 = new Color(color.r / 2.5f, color.g / 2.5f, color.b / 2.5f);
            Color color5 = new Color(color.r / 2.8f, color.g / 2.8f, color.b / 2.8f, 0.25f);
            if (MainImageColor == null || MainImageColor.Count == 0)
            {
                MainImageColor = new List<Image>();
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Screens/Settings_Safety/_Description_SafetyLevel").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Custom/ON").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Screens/Settings_Safety/_Buttons_SafetyLevel/Button_None/ON").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Normal/ON").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Maxiumum/ON").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/InputKeypadPopup/Rectangle/Panel").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/InputKeypadPopup/InputField").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/StandardPopupV2/Popup/Panel").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/StandardPopup/InnerDashRing").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/StandardPopup/RingGlow").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/UpdateStatusPopup/Popup/Panel").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/InputPopup/InputField").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/UpdateStatusPopup/Popup/InputFieldStatus").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/AdvancedSettingsPopup/Popup/Panel").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/PerformanceSettingsPopup/Popup/Panel").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/AlertPopup/Lighter").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/RoomInstancePopup/Popup/Panel").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/ReportWorldPopup/Popup/Panel").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/ReportUserPopup/Popup/Panel").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/SearchOptionsPopup/Popup/Panel (1)").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Screens/UserInfo/User Panel/PanelHeaderBackground").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel/Panel_Header").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Screens/Settings/VoiceOptionsPanel/Panel_Header").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/Panel_Header Top").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/Panel_Header Side").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Screens/Settings/OtherOptionsPanel/Panel_Header (1)").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Screens/Settings/MousePanel/Panel_Header").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Screens/Settings/AudioDevicePanel/Panel_Header").GetComponent<Image>());
                MainImageColor.Add(GameObject.Find("UserInterface/MenuContent/Screens/Settings/UserVolumeOptions/Panel_Header (1)").GetComponent<Image>());
            }
            if (DimmerImageColor == null || DimmerImageColor.Count == 0)
            {
                DimmerImageColor = new List<Image>();
                DimmerImageColor.Add(GameObject.Find("UserInterface/MenuContent/Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Custom/ON/TopPanel_SafetyLevel").GetComponent<Image>());
                DimmerImageColor.Add(GameObject.Find("UserInterface/MenuContent/Screens/Settings_Safety/_Buttons_SafetyLevel/Button_None/ON/TopPanel_SafetyLevel").GetComponent<Image>());
                DimmerImageColor.Add(GameObject.Find("UserInterface/MenuContent/Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Normal/ON/TopPanel_SafetyLevel").GetComponent<Image>());
                DimmerImageColor.Add(GameObject.Find("UserInterface/MenuContent/Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Maxiumum/ON/TopPanel_SafetyLevel").GetComponent<Image>());
                DimmerImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/PerformanceSettingsPopup/Popup/ExitButton").GetComponent<Image>());
                foreach (Transform transform3 in from x in GameObject.Find("UserInterface/MenuContent").GetComponentsInChildren<Transform>(true)
                                                 where x.name.Contains("Fill")
                                                 select x)
                {
                    UnhollowerBaseLib.Il2CppArrayBase<Image> list4 = GameObject.Find("UserInterface/MenuContent").GetComponentsInChildren<Image>();
                    for (int i = 0; i < list4.Count; i++)
                    {
                        Image item3 = list4[i];
                        DimmerImageColor.Add(item3);
                    }
                }
            }
            if (DarkerImageColor == null || DarkerImageColor.Count == 0)
            {
                DarkerImageColor = new List<Image>();
                DarkerImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/InputKeypadPopup/Rectangle").GetComponent<Image>());
                DarkerImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/StandardPopupV2/Popup/BorderImage").GetComponent<Image>());
                DarkerImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/StandardPopup/Rectangle").GetComponent<Image>());
                DarkerImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/StandardPopup/MidRing").GetComponent<Image>());
                DarkerImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/UpdateStatusPopup/Popup/BorderImage").GetComponent<Image>());
                DarkerImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/AdvancedSettingsPopup/Popup/BorderImage").GetComponent<Image>());
                DarkerImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/PerformanceSettingsPopup/Popup/BorderImage").GetComponent<Image>());
                DarkerImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/RoomInstancePopup/Popup/BorderImage").GetComponent<Image>());
                DarkerImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/RoomInstancePopup/Popup/BorderImage (1)").GetComponent<Image>());
                DarkerImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/ReportWorldPopup/Popup/BorderImage").GetComponent<Image>());
                DarkerImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/ReportUserPopup/Popup/BorderImage").GetComponent<Image>());
                DarkerImageColor.Add(GameObject.Find("UserInterface/MenuContent/Popups/SearchOptionsPopup/Popup/BorderImage").GetComponent<Image>());
                foreach (Transform transform4 in from x in GameObject.Find("UserInterface/MenuContent").GetComponentsInChildren<Transform>(true)
                                                 where x.name.Contains("Background")
                                                 select x)
                {
                    UnhollowerBaseLib.Il2CppArrayBase<Image> list4 = GameObject.Find("UserInterface/MenuContent").GetComponentsInChildren<Image>();
                    for (int i = 0; i < list4.Count; i++)
                    {
                        Image item4 = list4[i];
                        DarkerImageColor.Add(item4);
                    }
                }
            }
            if (SecondaryColor == null || SecondaryColor.Count == 0)
            {
                SecondaryColor = new List<Text>();
                foreach (Text item5 in GameObject.Find("UserInterface/MenuContent/Popups/InputPopup/Keyboard/Keys").GetComponentsInChildren<Text>(true))
                {
                    SecondaryColor.Add(item5);
                }
                UnhollowerBaseLib.Il2CppArrayBase<Text> list4 = GameObject.Find("UserInterface/MenuContent/Popups/InputKeypadPopup/Keyboard/Keys").GetComponentsInChildren<Text>(true);
                for (int i = 0; i < list4.Count; i++)
                {
                    Text item6 = list4[i];
                    SecondaryColor.Add(item6);
                }
            }
            for (int i = 0; i < MainImageColor.Count; i++)
            {
                Image image = MainImageColor[i];
                image.color = color;
            }
            for (int i = 0; i < DimmerImageColor.Count; i++)
            {
                Image image2 = DimmerImageColor[i];
                image2.color = color;
            }
            for (int i = 0; i < DarkerImageColor.Count; i++)
            {
                Image image3 = DarkerImageColor[i];
                image3.color = color5;
            }
            for (int i = 0; i < SecondaryColor.Count; i++)
            {
                Text text = SecondaryColor[i];
                text.color = SecondColor;
            }
        }

        internal static IEnumerator MenuColorEdit()
        {
            yield return new WaitForSeconds(0.3f);
            if (Config.Main.CustomUi)
            {
                GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/BackgroundLayer01").GetComponent<Image>().sprite = LoadSpriteFromBytes(MenuPicV2);
            }
            if (Config.Main.CustomUiColor)
            {
                if (Config.Main.CustomUi)
                {
                    if (Config.Main.MenuPicture != "")
                    {
                        GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/BackgroundLayer01").GetComponent<Image>().sprite = LoadSpriteFromBytes(MenuPic);
                    }
                }
                Color color = MenuColor();
                Color SecondColor = TextColor();
                Color color2 = new Color(color.r, color.g, color.b, 0.4f);
                new Color(color.r / 0.75f, color.g / 0.75f, color.b / 0.75f);
                Color color3 = new Color(color.r / 0.75f, color.g / 0.75f, color.b / 0.75f, 0.9f);
                Color color4 = new Color(color.r / 2.5f, color.g / 2.5f, color.b / 2.5f);
                Color color5 = new Color(color.r / 2.8f, color.g / 2.8f, color.b / 2.8f, 0.25f);

                #region Menu Repos

                UnhollowerBaseLib.Il2CppArrayBase<Image> list = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport").GetComponentsInChildren<Image>(true);
                for (int i = 0; i < list.Count; i++)
                {
                    Image toggle = list[i];
                    toggle.color = color;
                }
                UnhollowerBaseLib.Il2CppArrayBase<Image> list1 = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/Page_Buttons_QM/HorizontalLayoutGroup").GetComponentsInChildren<Image>(true);
                for (int i = 0; i < list1.Count; i++)
                {
                    Image toggle = list1[i];
                    if (toggle.name == ("Background_Modal") || toggle.name == ("Menu_QM_AvatarDetails") || toggle.name == ("Background_Alert") || toggle.name == ("HidePhotos") || toggle.name == ("Checkbox_Background") || toggle.name == ("Background_QM_PagePanel") || toggle.name == ("Image") || toggle.name == ("Mic") || toggle.name == ("Audio") || toggle.name == ("Checkmark") || toggle.name == ("Menu_SelectedUser_Local") || toggle.name == ("BackgroundLayer01") || toggle.name == ("Menu_UserIconCamera") || toggle.name == ("HeaderBackground") || toggle.name == ("PanelBG") || toggle.name == ("Scrim") || toggle.name == ("Panel_Info") || toggle.name == ("Background_Button") || toggle.name == ("Viewport") || toggle.name == ("Flash") || toggle.name == ("Fill") || toggle.name == ("MenuPanel") || toggle.name == ("Checkmark"))
                    {
                        toggle.color = new Color(0, 0, 0, 0);
                    }
                    else
                    {
                        toggle.color = color;
                    }
                }
                UnhollowerBaseLib.Il2CppArrayBase<Image> list2 = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Notifications/Panel_Content/Friend Requests/Viewport/VerticalLayoutGroup").GetComponentsInChildren<Image>(true);
                for (int i = 0; i < list2.Count; i++)
                {
                    Image toggle = list2[i];
                    toggle.color = color;
                }
                if (Config.Main.CustomUi)
                {
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Carousel_Banners/Image_MASK").gameObject.SetActive(false);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Carousel_Banners").gameObject.SetActive(false);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Carousel_Banners").GetComponent<VRC.UI.Core.Styles.StyleElement>().enabled = false;
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Carousel_Banners").GetComponent<UnityEngine.RectTransform>().sizeDelta = new Vector2(1024, 380);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Carousel_Banners").gameObject.SetActive(true);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/Header_H1/LeftItemContainer/Text_Title").GetComponent<TMPro.TextMeshProUGUI>().m_text = "                  Fusion Client                  ";
                    UI.DebugPanel.SetActive(false);
                    try
                    {
                        DebugPanel = new InfoPanel(GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Carousel_Banners").transform, 0, 0, 950, 350, "")
                        {
                            InfoText =
                                {
                                    color = Color.white,
                                    supportRichText = true,
                                    fontSize = 30,
                                    fontStyle = FontStyle.Normal,
                                    alignment = TextAnchor.UpperLeft
                                },
                            InfoBackground =
                                {
                                    color = new Color(255, 255, 255, 0.85f),
                                }
                        };
                        DebugPanel.SetActive(Config.Main.CustomUi);
                        DebugPanel.InfoBackground.sprite = LoadSpriteFromBytes(MenuDebug);
                        DebugPanel.InfoBackground.m_Sprite = LoadSpriteFromBytes(MenuDebug);
                    }
                    catch (Exception e)
                    {
                        Logs.Log("[UI] Error Creating Debug! | Error Message: " + e.Message, ConsoleColor.Red);
                    }
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Header_QuickActions").gameObject.SetActive(false);
                }
                UnhollowerBaseLib.Il2CppArrayBase<Image> list3 = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent").GetComponentsInChildren<Image>(true);
                for (int i = 0; i < list3.Count; i++)
                {
                    Image toggle = list3[i];
                    if (toggle.name == ("Background_Modal") || toggle.name == ("Menu_QM_AvatarDetails") || toggle.name == ("Background_Alert") || toggle.name == ("HidePhotos") || toggle.name == ("Checkbox_Background") || toggle.name == ("Background_QM_PagePanel") || toggle.name == ("Image") || toggle.name == ("Mic") || toggle.name == ("Audio") || toggle.name == ("Checkmark") || toggle.name == ("Menu_SelectedUser_Local") || toggle.name == ("BackgroundLayer01") || toggle.name == ("Menu_UserIconCamera") || toggle.name == ("HeaderBackground") || toggle.name == ("PanelBG") || toggle.name == ("Scrim") || toggle.name == ("Panel_Info") || toggle.name == ("Background_Button") || toggle.name == ("Viewport") || toggle.name == ("Flash") || toggle.name == ("Fill") || toggle.name == ("MenuPanel") || toggle.name == ("Checkmark"))
                    {
                        toggle.color = new Color(0, 0, 0, 0);
                    }
                    else
                    {
                        toggle.color = color;
                    }
                }

                #endregion

                ColorBlock colors = new ColorBlock
                {
                    colorMultiplier = 1f,
                    disabledColor = Color.gray,
                    highlightedColor = SecondColor * 1.5f,
                    normalColor = color,
                    pressedColor = SecondColor / 1.5f,
                    fadeDuration = 0.5f,
                };
                color.a = 1;
                if (UnityEngine.Resources.FindObjectsOfTypeAll<HighlightsFXStandalone>().Count != 0)
                {
                    UnityEngine.Resources.FindObjectsOfTypeAll<HighlightsFXStandalone>().FirstOrDefault<HighlightsFXStandalone>().highlightColor = color;
                }
                if (QuickMenuUtils.GetVRCUiMInstance().field_Public_GameObject_0 != null)
                {
                    GameObject gameObject = GameObject.Find("UserInterface/MenuContent/");
                    try
                    {
                        Transform transform5 = gameObject.transform.Find("Popups/InputPopup");
                        color4.a = 0.8f;
                        transform5.Find("Rectangle").GetComponent<Image>().color = color4;
                        color4.a = 0.5f;
                        color.a = 0.8f;
                        transform5.Find("Rectangle/Panel").GetComponent<Image>().color = color;
                        color.a = 0.5f;
                        Transform transform6 = gameObject.transform.Find("Backdrop/Header/Tabs/ViewPort/Content/Search");
                        transform6.Find("SearchTitle").GetComponent<Text>().color = color;
                        transform6.Find("InputField").GetComponent<Image>().color = color;
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        ColorBlock colors2 = new ColorBlock
                        {
                            colorMultiplier = 1f,
                            disabledColor = Color.black,
                            highlightedColor = SecondColor * 1.5f,
                            normalColor = color,
                            pressedColor = SecondColor / 1.5f,
                            fadeDuration = 0.5f,
                        };
                        gameObject.GetComponentsInChildren<Transform>(true).FirstOrDefault((Transform x) => x.name == "Row:4 Column:0").GetComponent<Button>().colors = colors;
                        color.a = 0.5f;
                        color4.a = 1f;
                        colors2.normalColor = color4;
                        UnhollowerBaseLib.Il2CppArrayBase<Slider> list4 = gameObject.GetComponentsInChildren<Slider>(true);
                        for (int i = 0; i < list4.Count; i++)
                        {
                            Slider slider = list4[i];
                            slider.colors = colors2;
                        }
                        color4.a = 0.5f;
                        colors2.normalColor = color;
                        UnhollowerBaseLib.Il2CppArrayBase<Button> list5 = gameObject.GetComponentsInChildren<Button>(true);
                        for (int i = 0; i < list5.Count; i++)
                        {
                            Button button = list5[i];
                            button.colors = colors;
                        }
                        gameObject = GameObject.Find("QuickMenu");
                        //UnhollowerBaseLib.Il2CppArrayBase<Button> list6 = GameObject.Find("UserInterface/MenuContent").GetComponentsInChildren<Button>(true);
                        //for (int i = 0; i < list6.Count; i++)
                        //{
                        //    Button button2 = list6[i];
                        //    if (button2.gameObject.name != "rColorButton" && button2.gameObject.name != "gColorButton" && button2.gameObject.name != "bColorButton" && button2.gameObject.name != "ColorPickPreviewButton" && button2.transform.parent.parent.name != "EmojiMenu")
                        //    {
                        //        button2.colors = colors;
                        //    }
                        //}
                        UnhollowerBaseLib.Il2CppArrayBase<UiToggleButton> list7 = gameObject.GetComponentsInChildren<UiToggleButton>(true);
                        for (int i = 0; i < list7.Count; i++)
                        {
                            UiToggleButton uiToggleButton = list7[i];
                            foreach (Image image6 in uiToggleButton.GetComponentsInChildren<Image>(true))
                            {
                                image6.color = color * 1.1f;
                            }
                        }
                        UnhollowerBaseLib.Il2CppArrayBase<Slider> list8 = gameObject.GetComponentsInChildren<Slider>(true);
                        for (int i = 0; i < list8.Count; i++)
                        {
                            Slider slider2 = list8[i];
                            slider2.colors = colors2;
                            foreach (Image image7 in slider2.GetComponentsInChildren<Image>(true))
                            {
                                image7.color = color;
                            }
                        }
                        UnhollowerBaseLib.Il2CppArrayBase<Toggle> list9 = gameObject.GetComponentsInChildren<Toggle>(true);
                        for (int i = 0; i < list9.Count; i++)
                        {
                            Toggle toggle = list9[i];
                            toggle.colors = colors2;
                            foreach (Image image8 in toggle.GetComponentsInChildren<Image>(true))
                            {
                                image8.color = SecondColor;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        new Exception();
                    }

                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Here/ScrollRect/Viewport/VerticalLayoutGroup/QM_Foldout_WorldActions/Background_Button").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Here/ScrollRect/Viewport/VerticalLayoutGroup/QM_Foldout_UsersInWorld/Background_Button").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Settings/Panel_QM_ScrollRect/Viewport/VerticalLayoutGroup/QM_Foldout_AvInteractions/Background_Button").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Settings/Panel_QM_ScrollRect/Viewport/VerticalLayoutGroup/QM_Foldout_UI_Elements/Background_Button").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Settings/Panel_QM_ScrollRect/Viewport/VerticalLayoutGroup/QM_Foldout_Comfort/Background_Button").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Settings/Panel_QM_ScrollRect/Viewport/VerticalLayoutGroup/QM_Foldout_Debug/Background_Button").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_QM_Report_Issue/Panel/Panel_QM_ScrollRect/Viewport").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_QM_Report_Details/Panel/Panel_QM_ScrollRect/Viewport").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_UserIconCamera/ScrollRect/Viewport/VerticalLayoutGroup/Preview Window/Preview/Flash").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_UserIconCamera/ScrollRect/Viewport/VerticalLayoutGroup/Preview Window/Preview/Bounds").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_UserIconCamera/ScrollRect/Viewport/VerticalLayoutGroup/Preview Window/Preview/Bounds").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Here/ScrollRect/Viewport/VerticalLayoutGroup/QMCell_InstanceDetails/Panel/PanelBG").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Modal_ConfirmDialog/Scrim").GetComponent<Image>().color = new Color(0, 0, 0, 0.9549f);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Modal_ConfirmDialog/MenuPanel").GetComponent<Image>().color = new Color(0, 0, 0, 0.9549f);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_SelectedUser_Local/ScrollRect/Viewport/VerticalLayoutGroup/UserProfile_Compact/Sliders_UserVolume/PanelBG/InputLevel/Sliders/MicSensitivitySlider/Fill Area/Fill").GetComponent<Image>().color = new Color(0, 0, 0, 0.9549f);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_SelectedUser_Local/ScrollRect/Viewport/VerticalLayoutGroup/UserProfile_Compact/Sliders_UserVolume/PanelBG/InputLevel/Sliders/MicSensitivitySlider/Fill Area/Fill").GetComponent<Image>().color = new Color(0, 0, 0, 0.9549f);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Modal_HoveredUser/Background").GetComponent<Image>().color = new Color(0, 0, 0, 0.9549f);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Modal_HoveredUser/QMUserProfile_Compact/PanelBG").GetComponent<Image>().color = new Color(0, 0, 0, 0.9949f);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Modal_HoveredUser/Scrim").GetComponent<Image>().color = new Color(0, 0, 0, 0.9549f);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_SelectedUser_Local/ScrollRect/Viewport/VerticalLayoutGroup/UserProfile_Compact/PanelBG").GetComponent<Image>().color = new Color(0, 0, 0, 0.9549f);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_SelectedUser_Local/ScrollRect/Viewport/VerticalLayoutGroup/UserProfile_Compact/Sliders_UserVolume/PanelBG").GetComponent<Image>().color = new Color(0, 0, 0, 0.9549f);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_SelectedUser_Local/ScrollRect/Scrollbar").GetComponent<Image>().color = new Color(0, 0, 0, 0.9549f);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Here/QMHeader_H1/HeaderBackground").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_InviteResponse/Header_H1/HeaderBackground").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_InviteResponse").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Modal_AddMessage/Scrim").GetComponent<Image>().color = new Color(0, 0, 0, 0.9549f);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Modal_AddMessage/MenuPanel").GetComponent<Image>().color = new Color(0, 0, 0, 00.9549f);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Notifications/QMHeader_H1/HeaderBackground").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_SelectedUser_Local/ScrollRect/Viewport/VerticalLayoutGroup/QM_Foldout_UserActions/Background_Button").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_SelectedUser_Local/ScrollRect/Viewport/VerticalLayoutGroup/QM_Foldout_PerUseInteractions/Background_Button").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Settings/QMHeader_H1/HeaderBackground").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_AudioSettings/Content/Audio").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_AudioSettings/Content/Mic").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_AudioSettings/QMHeader_H1/HeaderBackground").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Camera/Header_Camera/HeaderBackground").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_SelectedUser_Local").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Camera/Panel_Info").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_UserIconCamera").GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/BackgroundLayer01").GetComponent<Image>().color = SecondColor * 0.988f;
                    GameObject.Find("UserInterface/MenuContent/Screens/Settings/OtherOptionsPanel/HeadLookToggle/Background/Checkmark").GetComponent<Image>().color = SecondColor;
                    GameObject.Find("UserInterface/MenuContent/Screens/Settings/OtherOptionsPanel/TooltipsToggle/Background/Checkmark").GetComponent<Image>().color = SecondColor;
                    GameObject.Find("UserInterface/MenuContent/Screens/Settings/OtherOptionsPanel/3PRotationToggle/Background/Checkmark").GetComponent<Image>().color = SecondColor;
                    GameObject.Find("UserInterface/MenuContent/Screens/Settings/OtherOptionsPanel/ViveAdvancedToggle/Background/Checkmark").GetComponent<Image>().color = SecondColor;
                    GameObject.Find("UserInterface/MenuContent/Screens/Settings/OtherOptionsPanel/SkipGoButtonInLoad/Background/Checkmark").GetComponent<Image>().color = SecondColor;
                    GameObject.Find("UserInterface/MenuContent/Screens/Settings/OtherOptionsPanel/DesktopReticle/Background/Checkmark").GetComponent<Image>().color = SecondColor;
                    GameObject.Find("UserInterface/MenuContent/Screens/Settings/OtherOptionsPanel/AllowAvatarCopyingToggle/Background/Checkmark").GetComponent<Image>().color = SecondColor;
                    GameObject.Find("UserInterface/MenuContent/Screens/Settings/OtherOptionsPanel/ShowCommunityLabsToggle/Background/Checkmark").GetComponent<Image>().color = SecondColor;
                    GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel/HoloportToggle/Background/Checkmark").GetComponent<Image>().color = SecondColor;
                    GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel/ComfortTurnToggle/Background/Checkmark").GetComponent<Image>().color = SecondColor;
                    GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel/PersonalSpaceToggle/Background/Checkmark").GetComponent<Image>().color = SecondColor;
                    GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel/AllowUntrustedURL/Background/Checkmark").GetComponent<Image>().color = SecondColor;
                    GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel/StreamerModeToggle/Background/Checkmark").GetComponent<Image>().color = SecondColor;
                    GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel/MuteUsersToggle/Background/Checkmark").GetComponent<Image>().color = SecondColor;
                    GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel/BlockAvatarsToggle/Background/Checkmark").GetComponent<Image>().color = SecondColor;
                    GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel/HeadSetGazeToggle/Background/Checkmark").GetComponent<Image>().color = SecondColor;
                    GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel/KeyboardToggle/Background/Checkmark").GetComponent<Image>().color = SecondColor;
                    GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel/GamepadToggle/Background/Checkmark").GetComponent<Image>().color = SecondColor;
                    GameObject.Find("UserInterface/MenuContent/Screens/Settings/MousePanel/InvertedMouse/Background/Checkmark").GetComponent<Image>().color = SecondColor;
                    GameObject.Find("UserInterface/MenuContent/Screens/Settings/VoiceOptionsPanel/HardwareConfigToggle (1)/Background/Checkmark").GetComponent<Image>().color = SecondColor;
                    GameObject.Find("UserInterface/MenuContent/Screens/Settings/VoiceOptionsPanel/HardwareConfigToggle (4)/Background/Checkmark").GetComponent<Image>().color = SecondColor;
                    GameObject.Find("UserInterface/MenuContent/Screens/Settings/VoiceOptionsPanel/HardwareConfigToggle (6)/Background/Checkmark").GetComponent<Image>().color = SecondColor;
                    GameObject.Find("UserInterface/MenuContent/Screens/Settings/VoiceOptionsPanel/HardwareConfigToggle (2)/Background/Checkmark").GetComponent<Image>().color = SecondColor;
                    GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/Header_H1/HeaderBackground").gameObject.SetActive(false);

                    VRCUiCursorManager.field_Private_Static_VRCUiCursorManager_0.field_Public_VRCUiCursor_0.field_Public_Color_0 = SecondColor;
                    VRCUiCursorManager.field_Private_Static_VRCUiCursorManager_0.field_Public_VRCUiCursor_1.field_Public_Color_0 = SecondColor;
                    VRCUiCursorManager.field_Private_Static_VRCUiCursorManager_0.field_Public_VRCUiCursor_2.field_Public_Color_0 = SecondColor;
                    VRCUiCursorManager.field_Private_Static_VRCUiCursorManager_0.field_Public_VRCUiCursor_3.field_Public_Color_0 = SecondColor;
                    VRCUiCursorManager.field_Private_Static_VRCUiCursorManager_0.field_Public_VRCUiCursor_4.field_Public_Color_0 = SecondColor;

                    //All the quickmenu text
                    UnhollowerBaseLib.Il2CppArrayBase<Text> list10 = QuickMenuUtils.GetVRCUiMInstance().field_Public_GameObject_0.GetComponentsInChildren<Text>(true);
                    for (int i = 0; i < list10.Count; i++)
                    {
                        Text text = list10[i];
                        text.fontStyle = FontStyle.Italic;
                    }

                    UnhollowerBaseLib.Il2CppArrayBase<TMPro.TextMeshProUGUI> list11 = UnityEngine.Object.FindObjectOfType<VRC.UI.Elements.QuickMenu>().gameObject.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true);
                    for (int i = 0; i < list11.Count; i++)
                    {
                        TMPro.TextMeshProUGUI text = list11[i];
                        text.fontStyle = (TMPro.FontStyles)FontStyle.Italic;
                        text.color = SecondColor;
                        text.m_Color = SecondColor;
                    }

                }
            }
        }

        public static Color HexToColor(string hexColor)
        {
            if (hexColor.IndexOf('#') != -1)
            {
                hexColor = hexColor.Replace("#", "");
            }
            float r = int.Parse(hexColor.Substring(0, 2), NumberStyles.AllowHexSpecifier) / 255f;
            float g = int.Parse(hexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier) / 255f;
            float b = int.Parse(hexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier) / 255f;
            return new Color(r, g, b);
        }

        public static string ColorToHex(Color baseColor, bool hash = false)
        {
            int num = Convert.ToInt32(baseColor.r * 255f);
            int num2 = Convert.ToInt32(baseColor.g * 255f);
            int num3 = Convert.ToInt32(baseColor.b * 255f);
            string text = num.ToString("X2") + num2.ToString("X2") + num3.ToString("X2");
            if (hash)
            {
                text = "#" + text;
            }
            return text;
        }

        public static Color MenuColor()
        {
            return HexToColor(Config.Main.UIColorHex);
        }

        public static Color TextColor()
        {
            return HexToColor(Config.Main.UITextColorHex);
        }
    }
}