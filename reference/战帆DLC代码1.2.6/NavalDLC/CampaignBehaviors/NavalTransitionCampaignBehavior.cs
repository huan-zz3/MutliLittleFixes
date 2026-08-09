using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using NavalDLC.Missions;
using NavalDLC.Storyline;
using NavalDLC.Storyline.Quests;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.CampaignBehaviors
{
	// Token: 0x0200016F RID: 367
	public class NavalTransitionCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x060017FC RID: 6140 RVA: 0x000A3BC6 File Offset: 0x000A1DC6
		public override void RegisterEvents()
		{
			CampaignEvents.OnAfterSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnAfterSessionLaunched));
		}

		// Token: 0x060017FD RID: 6141 RVA: 0x000A3BDF File Offset: 0x000A1DDF
		private void OnAfterSessionLaunched(CampaignGameStarter campaignGameSystemStarter)
		{
			this.AddGameMenus(campaignGameSystemStarter);
		}

		// Token: 0x060017FE RID: 6142 RVA: 0x000A3BE8 File Offset: 0x000A1DE8
		private void AddGameMenus(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddGameMenuOption("town", "port", "{=JTZ3L8gS}Go to the port", new GameMenuOption.OnConditionDelegate(this.visit_port_condition), new GameMenuOption.OnConsequenceDelegate(this.visit_port_consequence), false, 1, false, null);
			campaignGameStarter.AddGameMenu("port_menu", "{=AZajdfxc}You are at the port.", new OnInitDelegate(this.port_game_menu_on_init), 3, 0, null);
			campaignGameStarter.AddGameMenuOption("port_menu", "leave_option", "{=VJ4CUSE9}Go to the town center", new GameMenuOption.OnConditionDelegate(this.visit_town_condition), new GameMenuOption.OnConsequenceDelegate(this.visit_town_consequence), false, -1, false, null);
			campaignGameStarter.AddGameMenuOption("port_menu", "leave_option_isleave", "{=VJ4CUSE9}Go to the town center", new GameMenuOption.OnConditionDelegate(this.visit_town_isleave_condition), new GameMenuOption.OnConsequenceDelegate(this.visit_town_consequence), true, -1, false, null);
			campaignGameStarter.AddGameMenuOption("port_menu", "call_fleet", "{=GsDF9PJb}Call fleet", new GameMenuOption.OnConditionDelegate(this.call_fleet_condition), new GameMenuOption.OnConsequenceDelegate(this.call_fleet_consequence), false, -1, false, null);
			campaignGameStarter.AddGameMenuOption("port_menu", "inspect_fleet", "{=KuOj4IWq}Browse shipyard", new GameMenuOption.OnConditionDelegate(this.inspect_fleet_condition), new GameMenuOption.OnConsequenceDelegate(this.inspect_fleet_consequence), false, -1, false, null);
			campaignGameStarter.AddGameMenuOption("port_menu", "manage_fleet", "{=rQp1JolW}Manage fleet", new GameMenuOption.OnConditionDelegate(this.manage_fleet_condition), new GameMenuOption.OnConsequenceDelegate(this.manage_fleet_consequence), false, -1, false, null);
			campaignGameStarter.AddGameMenuOption("port_menu", "repair_ships", "{=hqGD0o4E}Repair ships ({TOTAL_AMOUNT}{GOLD_ICON})", new GameMenuOption.OnConditionDelegate(this.repair_ships_condition), new GameMenuOption.OnConsequenceDelegate(this.repair_ships_consequence), false, -1, false, null);
			campaignGameStarter.AddGameMenuOption("port_menu", "trade", "{=GmcgoiGy}Trade", new GameMenuOption.OnConditionDelegate(this.trade_on_condition), new GameMenuOption.OnConsequenceDelegate(this.trade_on_consequence), false, -1, false, null);
			campaignGameStarter.AddGameMenuOption("port_menu", "enter_port", "{=PwV5gaLa}Take a walk around the port", new GameMenuOption.OnConditionDelegate(this.enter_port_condition), new GameMenuOption.OnConsequenceDelegate(this.enter_port_consequence), false, -1, false, null);
			campaignGameStarter.AddGameMenuOption("port_menu", "port_wait", "{=zEoHYEUS}Wait here for some time", new GameMenuOption.OnConditionDelegate(this.wait_here_on_condition), new GameMenuOption.OnConsequenceDelegate(this.wait_here_on_consequence), false, -1, false, null);
			campaignGameStarter.AddGameMenuOption("port_menu", "sail_option", "{=fbCbFqyj}Set sail", new GameMenuOption.OnConditionDelegate(this.set_sail_condition), new GameMenuOption.OnConsequenceDelegate(this.set_sail_consequence), true, -1, false, null);
			campaignGameStarter.AddWaitGameMenu("port_wait_menu", "{=VqVYMGIP}You are waiting at the port of {CURRENT_SETTLEMENT}. {FURTHER_EXPLANATION}", new OnInitDelegate(this.wait_menu_on_init), new OnConditionDelegate(this.wait_menu_on_condition), null, new OnTickDelegate(this.wait_menu_on_tick), 3, 3, 0f, 0, null);
			campaignGameStarter.AddGameMenuOption("port_wait_menu", "wait_leave", "{=UqDNAZqM}Stop waiting", new GameMenuOption.OnConditionDelegate(this.wait_menu_back_on_condition), delegate(MenuCallbackArgs args)
			{
				PlayerEncounter.Current.IsPlayerWaiting = false;
				GameMenu.SwitchToMenu("port_menu");
			}, true, -1, false, null);
		}

		// Token: 0x060017FF RID: 6143 RVA: 0x000A3EB4 File Offset: 0x000A20B4
		[GameMenuInitializationHandler("port_menu")]
		[GameMenuInitializationHandler("port_wait_menu")]
		public static void port_menu_on_init(MenuCallbackArgs args)
		{
			string text = Settlement.CurrentSettlement.Culture.StringId + "_port";
			args.MenuContext.SetBackgroundMeshName(text);
			args.MenuContext.SetAmbientSound("event:/map/ambient/node/settlements/2d/port");
		}

		// Token: 0x06001800 RID: 6144 RVA: 0x000A3EF7 File Offset: 0x000A20F7
		private void call_fleet_consequence(MenuCallbackArgs args)
		{
			MobileParty.MainParty.Anchor.CallFleet(Settlement.CurrentSettlement);
			Campaign.Current.GameMenuManager.RefreshMenuOptions(args.MenuContext);
		}

		// Token: 0x06001801 RID: 6145 RVA: 0x000A3F24 File Offset: 0x000A2124
		private bool call_fleet_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 45;
			if (!this.CanMainPartySail() || MobileParty.MainParty.Anchor.IsAtSettlement(Settlement.CurrentSettlement))
			{
				return false;
			}
			args.Tooltip = this.GetWaitingForFleetText();
			args.IsEnabled = !this.IsFleetMovingToCurrentSettlement();
			return true;
		}

		// Token: 0x06001802 RID: 6146 RVA: 0x000A3F78 File Offset: 0x000A2178
		private TextObject GetWaitingForFleetText()
		{
			if (!this.CanMainPartySail() || MobileParty.MainParty.Anchor == null)
			{
				return null;
			}
			if (MobileParty.MainParty.Anchor.IsAtSettlement(Settlement.CurrentSettlement))
			{
				return new TextObject("{=1DY0jYK1}Your fleet has arrived.", null);
			}
			TextObject textObject = (this.IsFleetMovingToCurrentSettlement() ? new TextObject("{=u6UWSZMW}Your fleet is on its way and is {ETA}.", null) : new TextObject("{=nywEqaW2}Your fleet is {ETA}.", null));
			textObject.SetTextVariable("ETA", this.GetETAText());
			return textObject;
		}

		// Token: 0x06001803 RID: 6147 RVA: 0x000A3FF0 File Offset: 0x000A21F0
		private TextObject GetETAText()
		{
			int num = (this.IsFleetMovingToCurrentSettlement() ? ((int)Math.Ceiling((MobileParty.MainParty.Anchor.ArrivalTime - CampaignTime.Now).ToHours + 1.0)) : ((int)Math.Ceiling(Campaign.Current.Models.PartyTransitionModel.GetFleetTravelTimeToSettlement(MobileParty.MainParty, Settlement.CurrentSettlement).ToHours)));
			if ((float)num < 6f)
			{
				TextObject textObject = new TextObject("{=QDWuxaQI}{HOURS} {?(HOURS > 1)}hours{?}hour{\\?} away", null);
				textObject.SetTextVariable("HOURS", num);
				return textObject;
			}
			if ((float)num < 16f)
			{
				return new TextObject("{=Q4lKFt80}half a day away", null);
			}
			if ((float)num < 28f)
			{
				return new TextObject("{=QFaGoMkg}a day away", null);
			}
			if ((float)num < 36f)
			{
				return new TextObject("{=CIggfIra}more than a day away", null);
			}
			TextObject textObject2 = new TextObject("{=AX96ftdN}{DAYS} days away", null);
			textObject2.SetTextVariable("DAYS", MathF.Round((float)num / 24f));
			return textObject2;
		}

		// Token: 0x06001804 RID: 6148 RVA: 0x000A40EC File Offset: 0x000A22EC
		private void port_game_menu_on_init(MenuCallbackArgs args)
		{
			if (MenuHelper.CheckAndOpenNextLocation(args))
			{
				return;
			}
			Campaign.Current.GameMenuManager.MenuLocations.Clear();
			Campaign.Current.GameMenuManager.MenuLocations.Add(Settlement.CurrentSettlement.LocationComplex.GetLocationWithId("port"));
		}

		// Token: 0x06001805 RID: 6149 RVA: 0x000A4140 File Offset: 0x000A2340
		private bool visit_port_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 41;
			bool flag = MobileParty.MainParty.CurrentSettlement != null && MobileParty.MainParty.CurrentSettlement.HasPort && MobileParty.MainParty.CurrentSettlement.IsTown;
			if (flag)
			{
				bool flag3;
				TextObject textObject;
				bool flag2 = Campaign.Current.Models.SettlementAccessModel.CanMainHeroAccessLocation(Settlement.CurrentSettlement, "port", ref flag3, ref textObject);
				List<Location> list = Settlement.CurrentSettlement.LocationComplex.FindAll((string x) => x == "port").ToList<Location>();
				MenuHelper.SetIssueAndQuestDataForLocations(args, list);
				args.IsEnabled = flag2;
				args.Tooltip = textObject;
			}
			return flag;
		}

		// Token: 0x06001806 RID: 6150 RVA: 0x000A41F3 File Offset: 0x000A23F3
		private void visit_port_consequence(MenuCallbackArgs args)
		{
			GameMenu.ActivateGameMenu("port_menu");
		}

		// Token: 0x06001807 RID: 6151 RVA: 0x000A41FF File Offset: 0x000A23FF
		private bool CanMainPartySail()
		{
			return MobileParty.MainParty.HasNavalNavigationCapability;
		}

		// Token: 0x06001808 RID: 6152 RVA: 0x000A420B File Offset: 0x000A240B
		private void SetSail()
		{
			MobileParty.MainParty.SetSailAtPosition(Settlement.CurrentSettlement.PortPosition);
			PlayerEncounter.Finish(true);
		}

		// Token: 0x06001809 RID: 6153 RVA: 0x000A4227 File Offset: 0x000A2427
		private bool IsFleetMovingToCurrentSettlement()
		{
			return MobileParty.MainParty.Anchor != null && MobileParty.MainParty.Anchor.IsMovingToPoint && MobileParty.MainParty.Anchor.IsTargetingSettlement(Settlement.CurrentSettlement);
		}

		// Token: 0x0600180A RID: 6154 RVA: 0x000A425C File Offset: 0x000A245C
		private float GetGoldCostToRepairShips()
		{
			int num = 0;
			foreach (Ship ship in MobileParty.MainParty.Ships)
			{
				if (ship.HitPoints < ship.MaxHitPoints)
				{
					num += (int)Campaign.Current.Models.ShipCostModel.GetShipRepairCost(ship, PartyBase.MainParty);
				}
			}
			return (float)num;
		}

		// Token: 0x0600180B RID: 6155 RVA: 0x000A42DC File Offset: 0x000A24DC
		private bool GetIsSetSailEnabled()
		{
			return this.CanMainPartySail() && MobileParty.MainParty.Anchor.IsAtSettlement(Settlement.CurrentSettlement);
		}

		// Token: 0x0600180C RID: 6156 RVA: 0x000A42FC File Offset: 0x000A24FC
		private bool repair_ships_condition(MenuCallbackArgs args)
		{
			if (MobileParty.MainParty.Ships.Count == 0)
			{
				return false;
			}
			args.optionLeaveType = 47;
			if (MobileParty.MainParty.Anchor.IsAtSettlement(Settlement.CurrentSettlement))
			{
				float goldCostToRepairShips = this.GetGoldCostToRepairShips();
				if (goldCostToRepairShips > 0f)
				{
					if (goldCostToRepairShips > (float)Hero.MainHero.Gold)
					{
						args.IsEnabled = false;
						args.Tooltip = new TextObject("{=d0kbtGYn}You don't have enough gold.", null);
					}
					MBTextManager.SetTextVariable("TOTAL_AMOUNT", goldCostToRepairShips, 2);
				}
				else if (goldCostToRepairShips == 0f)
				{
					args.IsEnabled = false;
					args.Tooltip = new TextObject("{=Zgv6NCze}None of your ships are damaged.", null);
					MBTextManager.SetTextVariable("TOTAL_AMOUNT", goldCostToRepairShips, 2);
				}
				else
				{
					Debug.FailedAssert("There is a problem in here with ship repair cost calculation", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\CampaignBehaviors\\NavalTransitionCampaignBehavior.cs", "repair_ships_condition", 256);
				}
			}
			else
			{
				args.IsEnabled = false;
				args.Tooltip = new TextObject("{=EtTUPPeM}None of your ships are docked at this port.", null);
				MBTextManager.SetTextVariable("TOTAL_AMOUNT", 0);
			}
			return true;
		}

		// Token: 0x0600180D RID: 6157 RVA: 0x000A43F0 File Offset: 0x000A25F0
		private void repair_ships_consequence(MenuCallbackArgs args)
		{
			foreach (Ship ship in MobileParty.MainParty.Ships)
			{
				if (ship.HitPoints < ship.MaxHitPoints)
				{
					RepairShipAction.Apply(ship, Settlement.CurrentSettlement);
				}
			}
			Campaign.Current.GameMenuManager.RefreshMenuOptions(args.MenuContext);
		}

		// Token: 0x0600180E RID: 6158 RVA: 0x000A4470 File Offset: 0x000A2670
		private bool set_sail_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 43;
			if (!this.CanMainPartySail())
			{
				args.Tooltip = new TextObject("{=HUUd7Ohd}You don't own any ships!", null);
				args.IsEnabled = false;
			}
			else if (!MobileParty.MainParty.Anchor.IsAtSettlement(Settlement.CurrentSettlement))
			{
				args.Tooltip = new TextObject("{=LmTqrE8x}Your fleet is not docked at this settlement.", null);
				args.IsEnabled = false;
			}
			return true;
		}

		// Token: 0x0600180F RID: 6159 RVA: 0x000A44D6 File Offset: 0x000A26D6
		private void set_sail_consequence(MenuCallbackArgs args)
		{
			this.SetSail();
		}

		// Token: 0x06001810 RID: 6160 RVA: 0x000A44E0 File Offset: 0x000A26E0
		private bool enter_port_condition(MenuCallbackArgs args)
		{
			bool flag2;
			TextObject textObject;
			bool flag = Campaign.Current.Models.SettlementAccessModel.CanMainHeroAccessLocation(Settlement.CurrentSettlement, "port", ref flag2, ref textObject);
			List<Location> list = Settlement.CurrentSettlement.LocationComplex.FindAll((string x) => x == "port").ToList<Location>();
			MenuHelper.SetIssueAndQuestDataForLocations(args, list);
			args.optionLeaveType = 1;
			return MenuHelper.SetOptionProperties(args, flag, flag2, textObject);
		}

		// Token: 0x06001811 RID: 6161 RVA: 0x000A455C File Offset: 0x000A275C
		private void enter_port_consequence(MenuCallbackArgs args)
		{
			LocationEncounter locationEncounter = PlayerEncounter.LocationEncounter;
			if (Settlement.CurrentSettlement == NavalStorylineData.HomeSettlement && Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(SpeakToGunnarAndSisterQuest)))
			{
				NavalMissions.OpenNavalFinalConversationMission();
				return;
			}
			PlayerEncounter.LocationEncounter.CreateAndOpenMissionController(LocationComplex.Current.GetLocationWithId("port"), null, null, null);
		}

		// Token: 0x06001812 RID: 6162 RVA: 0x000A45BC File Offset: 0x000A27BC
		private bool inspect_fleet_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 44;
			MBReadOnlyList<Ship> availableShips = Settlement.CurrentSettlement.Town.AvailableShips;
			bool flag = availableShips != null && availableShips.Count > 0;
			MBReadOnlyList<Ship> ships = MobileParty.MainParty.Ships;
			bool flag2 = ships != null && ships.Count > 0;
			if ((!MobileParty.MainParty.Anchor.IsMovingToPoint && MobileParty.MainParty.Anchor.IsAtSettlement(Settlement.CurrentSettlement)) || !flag2)
			{
				return false;
			}
			if (!flag)
			{
				args.IsEnabled = false;
				TextObject textObject = new TextObject("{=kc2wu8UH}{SETTLEMENT} does not have any available ships and your fleet is away.", null);
				textObject.SetTextVariable("SETTLEMENT", Settlement.CurrentSettlement.Name.ToString());
				args.Tooltip = textObject;
			}
			else
			{
				args.Tooltip = new TextObject("{=CuhXWzub}You can only view ships at the port while your fleet is away.", null);
			}
			return true;
		}

		// Token: 0x06001813 RID: 6163 RVA: 0x000A4685 File Offset: 0x000A2885
		private void inspect_fleet_consequence(MenuCallbackArgs args)
		{
			PortStateHelper.OpenAsRestricted(Settlement.CurrentSettlement.Town, new TextObject("{=wkm1Jxap}Your fleet is not in this settlement", null));
		}

		// Token: 0x06001814 RID: 6164 RVA: 0x000A46A4 File Offset: 0x000A28A4
		private bool manage_fleet_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 44;
			MBReadOnlyList<Ship> availableShips = Settlement.CurrentSettlement.Town.AvailableShips;
			bool flag = availableShips != null && availableShips.Count > 0;
			MBReadOnlyList<Ship> ships = MobileParty.MainParty.Ships;
			bool flag2 = ships != null && ships.Count > 0;
			if ((MobileParty.MainParty.Anchor.IsMovingToPoint || !MobileParty.MainParty.Anchor.IsAtSettlement(Settlement.CurrentSettlement)) && flag2)
			{
				return false;
			}
			if (!flag && !flag2)
			{
				args.IsEnabled = false;
				args.Tooltip = new TextObject("{=bBT9xyQQ}Both you and the town have no available ships", null);
			}
			return true;
		}

		// Token: 0x06001815 RID: 6165 RVA: 0x000A4742 File Offset: 0x000A2942
		private void manage_fleet_consequence(MenuCallbackArgs args)
		{
			PortStateHelper.OpenAsTrade(Settlement.CurrentSettlement.Town);
		}

		// Token: 0x06001816 RID: 6166 RVA: 0x000A4753 File Offset: 0x000A2953
		private void visit_town_consequence(MenuCallbackArgs args)
		{
			GameMenu.ActivateGameMenu("town");
		}

		// Token: 0x06001817 RID: 6167 RVA: 0x000A475F File Offset: 0x000A295F
		private bool visit_town_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 42;
			return this.GetIsSetSailEnabled();
		}

		// Token: 0x06001818 RID: 6168 RVA: 0x000A476F File Offset: 0x000A296F
		private bool visit_town_isleave_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 42;
			return !this.GetIsSetSailEnabled();
		}

		// Token: 0x06001819 RID: 6169 RVA: 0x000A4784 File Offset: 0x000A2984
		private bool wait_here_on_condition(MenuCallbackArgs args)
		{
			bool flag2;
			TextObject textObject;
			bool flag = Campaign.Current.Models.SettlementAccessModel.CanMainHeroDoSettlementAction(Settlement.CurrentSettlement, 6, ref flag2, ref textObject);
			args.optionLeaveType = 15;
			return MenuHelper.SetOptionProperties(args, flag, flag2, textObject);
		}

		// Token: 0x0600181A RID: 6170 RVA: 0x000A47C1 File Offset: 0x000A29C1
		private void wait_here_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("port_wait_menu");
		}

		// Token: 0x0600181B RID: 6171 RVA: 0x000A47D0 File Offset: 0x000A29D0
		private void wait_menu_on_init(MenuCallbackArgs args)
		{
			if (MenuHelper.CheckAndOpenNextLocation(args))
			{
				return;
			}
			Campaign.Current.GameMenuManager.MenuLocations.Clear();
			Campaign.Current.GameMenuManager.MenuLocations.Add(Settlement.CurrentSettlement.LocationComplex.GetLocationWithId("port"));
			if (PlayerEncounter.Current != null)
			{
				PlayerEncounter.Current.IsPlayerWaiting = true;
			}
			this._isWaitingForFleet = this.IsFleetMovingToCurrentSettlement();
			MBTextManager.SetTextVariable("FURTHER_EXPLANATION", this.GetWaitingForFleetText(), false);
		}

		// Token: 0x0600181C RID: 6172 RVA: 0x000A4851 File Offset: 0x000A2A51
		private bool wait_menu_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 15;
			MBTextManager.SetTextVariable("CURRENT_SETTLEMENT", Settlement.CurrentSettlement.EncyclopediaLinkWithName, false);
			return true;
		}

		// Token: 0x0600181D RID: 6173 RVA: 0x000A4871 File Offset: 0x000A2A71
		private bool wait_menu_back_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 16;
			return true;
		}

		// Token: 0x0600181E RID: 6174 RVA: 0x000A487C File Offset: 0x000A2A7C
		private void wait_menu_on_tick(MenuCallbackArgs args, CampaignTime dt)
		{
			MBTextManager.SetTextVariable("FURTHER_EXPLANATION", this.GetWaitingForFleetText(), false);
			this.SwitchToMenuIfThereIsAnInterrupt(args.MenuContext.GameMenu.StringId);
		}

		// Token: 0x0600181F RID: 6175 RVA: 0x000A48A8 File Offset: 0x000A2AA8
		private void SwitchToMenuIfThereIsAnInterrupt(string currentMenuId)
		{
			string text = Campaign.Current.Models.EncounterGameMenuModel.GetGenericStateMenu();
			if (text == "town_wait_menus")
			{
				text = "port_wait_menu";
			}
			if (!(text != currentMenuId))
			{
				if (this._isWaitingForFleet)
				{
					AnchorPoint anchor = MobileParty.MainParty.Anchor;
					if (anchor != null && anchor.IsAtSettlement(Settlement.CurrentSettlement))
					{
						PlayerEncounter.Current.IsPlayerWaiting = false;
						GameMenu.SwitchToMenu("port_menu");
					}
				}
				return;
			}
			if (!string.IsNullOrEmpty(text))
			{
				PlayerEncounter.Current.IsPlayerWaiting = false;
				GameMenu.SwitchToMenu(text);
				return;
			}
			PlayerEncounter.Current.IsPlayerWaiting = false;
			GameMenu.SwitchToMenu("port_menu");
		}

		// Token: 0x06001820 RID: 6176 RVA: 0x000A4950 File Offset: 0x000A2B50
		private bool trade_on_condition(MenuCallbackArgs args)
		{
			bool flag2;
			TextObject textObject;
			bool flag = Campaign.Current.Models.SettlementAccessModel.CanMainHeroDoSettlementAction(Settlement.CurrentSettlement, 5, ref flag2, ref textObject);
			args.optionLeaveType = 14;
			return MenuHelper.SetOptionProperties(args, flag, flag2, textObject);
		}

		// Token: 0x06001821 RID: 6177 RVA: 0x000A498D File Offset: 0x000A2B8D
		private void trade_on_consequence(MenuCallbackArgs args)
		{
			LocationEncounter locationEncounter = PlayerEncounter.LocationEncounter;
			InventoryScreenHelper.OpenScreenAsTrade(Settlement.CurrentSettlement.ItemRoster, Settlement.CurrentSettlement.Town, -1, null);
		}

		// Token: 0x06001822 RID: 6178 RVA: 0x000A49B0 File Offset: 0x000A2BB0
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x04000BF2 RID: 3058
		private bool _isWaitingForFleet;
	}
}
