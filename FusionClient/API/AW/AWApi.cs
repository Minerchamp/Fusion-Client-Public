using System.Collections.Generic;
using System;
using UnhollowerRuntimeLib;
using UnityEngine;
using FusionClient.Core;

namespace Blaze.API.AW
{
	public static class ActionWheelAPI
	{
		private static readonly List<ActionMenuButton> mainMenuButtons = new List<ActionMenuButton>();

		public static ActionMenu activeActionMenu;

		public enum ActionMenuBaseMenu
		{
			MainMenu = 1
		}

		public static bool IsOpen(this ActionMenuOpener actionMenuOpener)
		{
			return actionMenuOpener.field_Private_Boolean_0;
		}

		private static ActionMenuOpener GetActionMenuOpener()
		{
			if (ActionMenuDriver.field_Public_Static_ActionMenuDriver_0.field_Public_ActionMenuOpener_0.IsOpen() == false &&
				ActionMenuDriver.field_Public_Static_ActionMenuDriver_0.field_Public_ActionMenuOpener_1.IsOpen() == true)
			{
				return ActionMenuDriver.field_Public_Static_ActionMenuDriver_0.field_Public_ActionMenuOpener_1;
			}

			if (ActionMenuDriver.field_Public_Static_ActionMenuDriver_0.field_Public_ActionMenuOpener_0.IsOpen() == true &&
				ActionMenuDriver.field_Public_Static_ActionMenuDriver_0.field_Public_ActionMenuOpener_1.IsOpen() == false)
			{
				return ActionMenuDriver.field_Public_Static_ActionMenuDriver_0.field_Public_ActionMenuOpener_0;
			}

			return null;
		}

		public static void OpenMainPage(ActionMenu menu)
		{
			activeActionMenu = menu;
			if (!Config.Main.UseActionMenu) return;
			foreach (ActionMenuButton button in mainMenuButtons)
			{
				PedalOption pedalOption = activeActionMenu.Method_Private_PedalOption_0();

				pedalOption.prop_String_0 = button.buttonText;
				pedalOption.field_Public_Func_1_Boolean_0 = DelegateSupport.ConvertDelegate<Il2CppSystem.Func<bool>>(button.buttonAction);

				if (button.buttonIcon != null)
				{
					pedalOption.Method_Public_Virtual_Final_New_Void_Texture2D_0(button.buttonIcon);
				}

				button.currentPedalOption = pedalOption;
			}
		}

		public class ActionMenuPage
		{
			public List<ActionMenuButton> buttons = new List<ActionMenuButton>();
			public ActionMenuPage previousPage;
			public ActionMenuButton menuEntryButton;

			public ActionMenuPage(ActionMenuBaseMenu baseMenu, string buttonText, Texture2D buttonIcon = null)
			{
				if (baseMenu == ActionMenuBaseMenu.MainMenu)
				{
					menuEntryButton = new ActionMenuButton(ActionMenuBaseMenu.MainMenu, buttonText, delegate ()
					{
						OpenMenu();
					}, buttonIcon);
				}
			}

			public ActionMenuPage(ActionMenuPage basePage, string buttonText, Texture2D buttonIcon = null)
			{
				ActionMenuPage page = this;

				previousPage = basePage;
				menuEntryButton = new ActionMenuButton(previousPage, buttonText, delegate ()
				{
					page.OpenMenu();
				}, buttonIcon);
			}

			public void OpenMenu()
			{
				GetActionMenuOpener().field_Public_ActionMenu_0.Method_Public_Page_Action_Action_Texture2D_String_0(new Action(() =>
				{
					foreach (ActionMenuButton button in buttons)
					{
						PedalOption pedalOption = GetActionMenuOpener().field_Public_ActionMenu_0.Method_Private_PedalOption_0();

						pedalOption.prop_String_0 = button.buttonText;
						pedalOption.field_Public_Func_1_Boolean_0 = DelegateSupport.ConvertDelegate<Il2CppSystem.Func<bool>>(button.buttonAction);

						if (button.buttonIcon != null)
						{
							pedalOption.Method_Public_Virtual_Final_New_Void_Texture2D_0(button.buttonIcon);
						}

						button.currentPedalOption = pedalOption;
					}
				}));
			}
		}

		public class ActionMenuButton
		{
			public PedalOption currentPedalOption;

			public string buttonText;
			public Texture2D buttonIcon;
			public Func<bool> buttonAction;

			public ActionMenuButton(ActionMenuBaseMenu baseMenu, string text, Action action, Texture2D icon = null)
			{
				buttonText = text;
				buttonIcon = icon;
				buttonAction = delegate ()
				{
					action();

					return true;
				};

				if (baseMenu == ActionMenuBaseMenu.MainMenu)
				{
					mainMenuButtons.Add(this);
				}
			}

			public ActionMenuButton(ActionMenuPage basePage, string text, Action action, Texture2D icon = null)
			{
				buttonText = text;
				buttonIcon = icon;
				buttonAction = delegate ()
				{
					action();

					return true;
				};

				basePage.buttons.Add(this);
			}

			public void SetButtonText(string newText)
			{
				buttonText = newText;

				if (currentPedalOption != null)
				{
					currentPedalOption.prop_String_0 = newText;
				}
			}

			public void SetIcon(Texture2D newTexture)
			{
				buttonIcon = newTexture;

				if (currentPedalOption != null)
				{
					currentPedalOption.Method_Public_Virtual_Final_New_Void_Texture2D_0(newTexture);
				}
			}
		}
	}
}
