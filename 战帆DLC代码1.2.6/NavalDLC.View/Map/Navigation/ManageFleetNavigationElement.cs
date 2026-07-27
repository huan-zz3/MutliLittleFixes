using System;
using Helpers;
using SandBox.View;
using SandBox.View.Map.Navigation;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ScreenSystem;

namespace NavalDLC.View.Map.Navigation
{
	// Token: 0x02000036 RID: 54
	public class ManageFleetNavigationElement : MapNavigationElementBase
	{
		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000198 RID: 408 RVA: 0x0000CCB8 File Offset: 0x0000AEB8
		public override string StringId
		{
			get
			{
				return "manage_fleet";
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000199 RID: 409 RVA: 0x0000CCBF File Offset: 0x0000AEBF
		public override bool IsActive
		{
			get
			{
				return base._game.GameStateManager.ActiveState is PortState;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600019A RID: 410 RVA: 0x0000CCDC File Offset: 0x0000AEDC
		public override bool IsLockingNavigation
		{
			get
			{
				PortState portState;
				return (portState = base._game.GameStateManager.ActiveState as PortState) != null && portState.PortScreenMode == 2;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x0600019B RID: 411 RVA: 0x0000CD0D File Offset: 0x0000AF0D
		public override bool HasAlert
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600019C RID: 412 RVA: 0x0000CD10 File Offset: 0x0000AF10
		public ManageFleetNavigationElement(NavalMapNavigationHandler handler)
			: base(handler)
		{
		}

		// Token: 0x0600019D RID: 413 RVA: 0x0000CD19 File Offset: 0x0000AF19
		protected override TextObject GetAlertTooltip()
		{
			return TextObject.GetEmpty();
		}

		// Token: 0x0600019E RID: 414 RVA: 0x0000CD20 File Offset: 0x0000AF20
		protected override TextObject GetTooltip()
		{
			if (!Input.IsGamepadActive && (base.Permission.IsAuthorized || this.IsActive))
			{
				string text = GameKeyTextExtensions.GetHotKeyGameText(Game.Current.GameTextManager, "GenericCampaignPanelsGameKeyCategory", 45).ToString();
				TextObject textObject = GameTexts.FindText("str_hotkey_with_hint", null);
				textObject.SetTextVariable("TEXT", GameTexts.FindText("str_fleet", null).ToString());
				textObject.SetTextVariable("HOTKEY", text);
				return textObject;
			}
			return GameTexts.FindText("str_fleet", null);
		}

		// Token: 0x0600019F RID: 415 RVA: 0x0000CDA8 File Offset: 0x0000AFA8
		protected override NavigationPermissionItem GetPermission()
		{
			if (!MapNavigationHelper.IsNavigationBarEnabled(this._handler))
			{
				return new NavigationPermissionItem(false, null);
			}
			if (this.IsActive)
			{
				return new NavigationPermissionItem(false, null);
			}
			if (PartyBase.MainParty.Ships.Count == 0)
			{
				return new NavigationPermissionItem(false, new TextObject("{=lb2hbQyx}You don't have any ships", null));
			}
			if (Mission.Current != null)
			{
				return new NavigationPermissionItem(false, GameTexts.FindText("str_cannot_open_fleet", null));
			}
			if (MobileParty.MainParty.MapEvent != null)
			{
				return new NavigationPermissionItem(false, GameTexts.FindText("str_cannot_open_fleet", null));
			}
			if (MobileParty.MainParty.IsInRaftState)
			{
				return new NavigationPermissionItem(false, new TextObject("{=Lo0E5dKh}You cannot manage your fleet while you are drifting to shore", null));
			}
			if (Hero.MainHero.IsPrisoner)
			{
				return new NavigationPermissionItem(false, new TextObject("{=a8UQow7P}You cannot manage your fleet while you are imprisoned", null));
			}
			Settlement currentSettlement = Settlement.CurrentSettlement;
			if (currentSettlement != null && currentSettlement.HasPort)
			{
				return new NavigationPermissionItem(false, new TextObject("{=Ug3Tmhr5}You can access your fleet from the port", null));
			}
			MobileParty mainParty = MobileParty.MainParty;
			if (mainParty != null && !mainParty.IsCurrentlyAtSea)
			{
				return new NavigationPermissionItem(false, new TextObject("{=lVes97xY}You cannot access your fleet when you are on land", null));
			}
			return new NavigationPermissionItem(true, null);
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x0000CEC4 File Offset: 0x0000B0C4
		public override void OpenView()
		{
			this.PrepareToOpenManageFleet(delegate
			{
				this.OpenManageFleetAction();
			});
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x0000CED8 File Offset: 0x0000B0D8
		public override void OpenView(params object[] parameters)
		{
			Debug.FailedAssert("Manage Fleet screen shouldn't be opened with parameters from navigation", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.View\\Map\\Navigation\\ManageFleetNavigationElement.cs", "OpenView", 106);
			this.OpenView();
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x0000CEF6 File Offset: 0x0000B0F6
		public override void GoToLink()
		{
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x0000CEF8 File Offset: 0x0000B0F8
		private void OpenManageFleetAction()
		{
			PortStateHelper.OpenAsManageFleet(new MBReadOnlyList<Ship>());
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x0000CF04 File Offset: 0x0000B104
		private void PrepareToOpenManageFleet(Action openManageFleetAction)
		{
			if (base.Permission.IsAuthorized)
			{
				IChangeableScreen changeableScreen;
				if ((changeableScreen = ScreenManager.TopScreen as IChangeableScreen) != null && changeableScreen.AnyUnsavedChanges())
				{
					InformationManager.ShowInquiry(changeableScreen.CanChangesBeApplied() ? MapNavigationHelper.GetUnsavedChangedInquiry(openManageFleetAction) : MapNavigationHelper.GetUnapplicableChangedInquiry(), false, false);
					return;
				}
				MapNavigationHelper.SwitchToANewScreen(openManageFleetAction);
			}
		}
	}
}
