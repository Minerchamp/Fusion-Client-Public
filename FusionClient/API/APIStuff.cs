using FusionClient.API.Wings;
using FusionClient.Core;
using System;
using UnityEngine;
using VRC.UI.Elements;

namespace FusionClient.API
{
    public static class APIStuff
    {
        private static VRC.UI.Elements.QuickMenu QuickMenuInstance;
        private static MenuStateController MenuStateControllerInstance;
        private static VRCUiPopupManager PopupManagerInstance;
        private static GameObject SocialMenuInstance;
        private static GameObject SingleButtonReference;
        private static GameObject TabButtonReference;
        private static GameObject MenuPageReference;
        private static GameObject SliderReference;
        private static GameObject SliderLabelReference;
        private static GameObject PopupMenuReference;
        private static GameObject QMInfoReference;
        private static GameObject InfoPanelReference;
        private static Sprite OnIconReference;
        private static Sprite OffIconReference;
        private static System.Random rnd = new();
        public static BaseWing Left = new();
        public static BaseWing Right = new();

        public static Action Init_L = () =>
        {
            Init_L = () => { };
            UI.InitializeWing(Left);
        };

        public static Action Init_R = () =>
        {
            Init_R = () => { };
            UI.InitializeWing(Right);
        };

        public static GameObject GetInfoPanelTemplate()
        {
            if (InfoPanelReference == null)
            {
                InfoPanelReference = GameObject.Find("UserInterface").transform.Find("MenuContent/Popups/PerformanceSettingsPopup/Popup/Pages/Page_LimitAvatarPerformance/Tooltip_Details").gameObject;
            }
            return InfoPanelReference;
        }

        public static VRC.UI.Elements.QuickMenu GetQuickMenuInstance()
        {
            if (QuickMenuInstance == null)
                QuickMenuInstance = Resources.FindObjectsOfTypeAll<VRC.UI.Elements.QuickMenu>()[0];
            return QuickMenuInstance;
        }

        public static GameObject GetSocialMenuInstance()
        {
            if (SocialMenuInstance == null)
            {
                SocialMenuInstance = GameObject.Find("UserInterface/MenuContent/Screens");
            }
            return SocialMenuInstance;
        }

        public static MenuStateController GetMenuStateControllerInstance()
        {
            if (MenuStateControllerInstance == null)
            {
                MenuStateControllerInstance = GetQuickMenuInstance().GetComponent<MenuStateController>();
            }
            return MenuStateControllerInstance;
        }

        public static Transform GetScreensInstance()
        {
            return GetSocialMenuInstance().transform.Find("Screens");
        }

        public static VRCUiPopupManager GetPopupManagerInstance()
        {
            if (PopupManagerInstance == null)
            {
                PopupManagerInstance = Resources.FindObjectsOfTypeAll<VRCUiPopupManager>()[0];
            }
            return PopupManagerInstance;
        }

        // Templates

        public static GameObject SingleButtonTemplate()
        {
            if (SingleButtonReference == null)
            {
                var Buttons = GetQuickMenuInstance().GetComponentsInChildren<UnityEngine.UI.Button>(true);
                foreach (var button in Buttons)
                {
                    if (button.name == "Button_Screenshot")
                    {
                        SingleButtonReference = button.gameObject;
                    }
                };
            }
            return SingleButtonReference;
        }

        public static GameObject GetMenuPageTemplate()
        {
            if (MenuPageReference == null)
            {
                MenuPageReference = GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard").gameObject;
            }
            return MenuPageReference;
        }

        public static GameObject GetQMInfoTemplate()
        {
            if (QMInfoReference == null)
            {
                QMInfoReference = GameObject.Find("UserInterface").transform.Find("MenuContent/Popups/PerformanceSettingsPopup/Popup/Pages/Page_LimitAvatarPerformance/Tooltip_Details").gameObject;
            }
            return QMInfoReference;
        }

        public static GameObject GetSliderTemplate()
        {
            if (SliderReference == null)
            {
                SliderReference = GameObject.Find("UserInterface").transform.Find("MenuContent/Screens/Settings/AudioDevicePanel/VolumeSlider").gameObject;
            }
            return SliderReference;
        }

        public static GameObject GetSliderLabelTemplate()
        {
            if (SliderLabelReference == null)
            {
                SliderLabelReference = GameObject.Find("UserInterface").transform.Find("MenuContent/Screens/Settings/AudioDevicePanel/LevelText").gameObject;
            }
            return SliderLabelReference;
        }

        public static GameObject GetTabButtonTemplate()
        {
            if (TabButtonReference == null)
            {
                TabButtonReference = GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)/Container/Window/Page_Buttons_QM/HorizontalLayoutGroup/Page_Settings").gameObject;
            }
            return TabButtonReference;
        }

        public static GameObject GetPopupMenuReference()
        {
            if (PopupMenuReference == null)
            {
                PopupMenuReference = GameObject.Find("UserInterface").transform.Find("MenuContent/Screens/UserInfo/ModerateDialog").gameObject;
            }
            return PopupMenuReference;
        }

        // Icon Sprites

        public static Sprite GetOnIconSprite()
        {
            if (OnIconReference == null)
            {
                OnIconReference = GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Notifications/Panel_NoNotifications_Message/Icon").GetComponent<UnityEngine.UI.Image>().sprite;
            }
            return OnIconReference;
        }

        public static Sprite GetOffIconSprite()
        {
            if (OffIconReference == null)
            {
                OffIconReference = GameObject.Find("UserInterface").transform.Find("Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Settings/Panel_QM_ScrollRect/Viewport/VerticalLayoutGroup/Buttons_UI_Elements_Row_1/Button_ToggleQMInfo/Icon_Off").GetComponent<UnityEngine.UI.Image>().sprite;
            }
            return OffIconReference;
        }

        public enum SMLocations
        {
            Worlds,
            Avatars,
            Social,
            Settings,
            Safety,
            UserInfo
        }

        // Functions

        public static int RandomNumbers()
        {
            return rnd.Next(100000, 999999);
        }

        public static void DestroyChildren(this Transform transform)
        {
            transform.DestroyChildren(null);
        }

        public static void DestroyChildren(this Transform transform, Func<Transform, bool> exclude)
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                if (exclude == null || exclude(transform.GetChild(i)))
                {
                    UnityEngine.Object.DestroyImmediate(transform.GetChild(i).gameObject);
                }
            }
        }
    }
}
