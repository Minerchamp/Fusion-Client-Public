using System;
using System.Collections.Generic;
using FusionClient.API.QM;
using FusionClient.Core;
using FusionClient.Utils.VRChat;

namespace WTFBlaze.API.QM
{
    public class QMScrollMenu
    {
        public class ScrollObject
        {
            public QMButtonBase ButtonBase;
            public int Index;
        }

        public QMNestedButton BaseMenu;
        public QMSingleButton NextButton;
        public QMSingleButton BackButton;
        public QMSingleButton IndexButton;
        public List<ScrollObject> QMButtons = new List<ScrollObject>();

        // If bugged Posy = 5 & max is 9
        // if not bugged Posy = 0 & max is 4

        private int Posx = 0;
        private int Posy = 0;
        private int Index = 0;
        private Action<QMScrollMenu> OpenAction;
        public int currentMenuIndex = 0;
        public bool ShouldChangePos = true;
        public bool AllowOverStepping = false;
        public bool IgnoreEverything = false;

        public QMScrollMenu(QMNestedButton basemenu)
        {
            BaseMenu = basemenu;
            IndexButton = new QMSingleButton(BaseMenu, 4, 1.50f, "Page:\n" + (currentMenuIndex + 1).ToString() + " of " + (Index + 1).ToString(), delegate
            {
                PopupUtils.NumericPopup("Select Page", "Enter Page number to view", delegate (string s)
                {
                    ShowMenu(int.Parse(s));
                });
            }, "Click to input what page you want to jump to");
            BackButton = new QMSingleButton(BaseMenu, 4, 1f, "Down", delegate
            {
                ShowMenu(currentMenuIndex - 1);
            }, "Go Back", true);
            NextButton = new QMSingleButton(BaseMenu, 4, 2.40f, "Up", delegate
            {
                ShowMenu(currentMenuIndex + 1);
            }, "Go Next", true);
        }

        public void ShowMenu(int MenuIndex)
        {
            if (!AllowOverStepping && (MenuIndex < 0 || MenuIndex > Index))
                return;

            foreach (var item in QMButtons)
            {
                if (item.Index == MenuIndex)
                    item.ButtonBase?.SetActive(true);
                else
                    item.ButtonBase?.SetActive(false);
            }
            currentMenuIndex = MenuIndex;
            IndexButton.SetButtonText("Page:\n" + (currentMenuIndex + 1).ToString() + " of " + (Index + 1).ToString());
        }

        public void SetAction(Action<QMScrollMenu> Open, bool shouldClear = true)
        {
            try
            {
                OpenAction = Open;
                BaseMenu.GetMainButton().SetAction(new Action(() =>
                {
                    if (shouldClear) Clear();
                    OpenAction.Invoke(this);
                    BaseMenu.OpenMe();
                    ShowMenu(0);
                }));
            }
            catch (Exception ex) { Logs.Error("QMScrollMenu", ex); }
        }

        public void Refresh()
        {
            Clear();
            OpenAction?.Invoke(this);
            BaseMenu.OpenMe();
            ShowMenu(0);
        }

        public void Refresh2()
        {
            BaseMenu.OpenMe();
            ShowMenu(0);
        }

        public void DestroyMe()
        {
            foreach (var item in QMButtons)
            {
                UnityEngine.Object.Destroy(item.ButtonBase.GetGameObject());
            }
            QMButtons.Clear();
            if (BaseMenu.GetBackButton() != null)
                UnityEngine.Object.Destroy(BaseMenu.GetBackButton());
            if (IndexButton != null)
                IndexButton.DestroyMe();
            if (BackButton != null)
                BackButton.DestroyMe();
            if (NextButton != null)
                NextButton.DestroyMe();
        }

        public void Clear()
        {
            try
            {
                foreach (var item in QMButtons)
                {
                    UnityEngine.Object.Destroy(item.ButtonBase.GetGameObject());
                }
                QMButtons.Clear();
                Posx = 0;
                Posy = 5;
                Index = 0;
                currentMenuIndex = 0;
            }
            catch { }
        }

        public void Add(QMButtonBase Button)
        {
            if (Posx < 4)
            {
                Posx++;
            }
            if (Posx == 4)
            {
                Posx = 1;
                Posy++;
            }
            if (Posy == 9)
            {
                Posy = 5;
                Index++;
            }

            //Console.WriteLine($"[Scroll Button] X:{Posx} Y:{Posy} Index:{Index}");

            if (ShouldChangePos)
                Button.SetLocation(Posx, Posy);
            Button.SetActive(false);
            QMButtons.Add(new ScrollObject()
            {
                ButtonBase = Button,
                Index = Index
            });
        }

        public void Add(QMButtonBase Button, int Page, float POSX = 0, float POSY = 0)
        {
            if (ShouldChangePos)
                Button.SetLocation(Posx, Posy);
            Button.SetActive(false);
            QMButtons.Add(new ScrollObject()
            {
                ButtonBase = Button,
                Index = Page
            });
            if (!IgnoreEverything)
            {
                if (Page > Index)
                {
                    Index = Page;
                }
            }
        }
    }

}