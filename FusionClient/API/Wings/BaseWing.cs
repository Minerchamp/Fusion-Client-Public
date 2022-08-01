using System.Collections.Generic;
using UnityEngine;

namespace FusionClient.API.Wings
{
    public class BaseWing
    {
        public readonly List<WingPage> openedPages = new();
        public void Setup(Transform wing)
        {
            Wing = wing;
            WingOpen = wing.Find("Button");
            WingPages = wing.Find("Container/InnerContainer");
            WingMenu = WingPages.Find("WingMenu");
            WingButtons = WingPages.Find("WingMenu/ScrollRect/Viewport/VerticalLayoutGroup");
            WingSlider = WingPages.Find("Emotes/Wing_Menu_RadialPuppet/Container/Slider");

            ProfilePage = WingPages.Find("Profile");
            ProfileButton = WingButtons.Find("Button_Profile");
        }

        public Transform Wing; //        UserInterface/Canvas_QuickMenu(Clone)/Container/Window/Wing_Left
        public Transform WingOpen; //    UserInterface/Canvas_QuickMenu(Clone)/Container/Window/Wing_Left/Button
        public Transform WingPages; //   UserInterface/Canvas_QuickMenu(Clone)/Container/Window/Wing_Left/Container/InnerContainer
        public Transform WingMenu; //    UserInterface/Canvas_QuickMenu(Clone)/Container/Window/Wing_Left/Container/InnerContainer/WingMenu
        public Transform WingButtons; // UserInterface/Canvas_QuickMenu(Clone)/Container/Window/Wing_Left/Container/InnerContainer/WingMenu/ScrollRect/Viewport/VerticalLayoutGroup
        public Transform WingSlider; // UserInterface/Canvas_QuickMenu(Clone)/Container/Window/Wing_Left/Container/InnerContainer/Emotes/Wing_Menu_RadialPuppet/Container/Slider

        public Transform ProfilePage;
        public Transform ProfileButton;
    }
}
