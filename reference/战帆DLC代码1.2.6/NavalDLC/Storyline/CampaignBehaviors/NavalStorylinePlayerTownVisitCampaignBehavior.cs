using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.Storyline.CampaignBehaviors
{
	// Token: 0x02000074 RID: 116
	public class NavalStorylinePlayerTownVisitCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600082D RID: 2093 RVA: 0x0003A25F File Offset: 0x0003845F
		public override void RegisterEvents()
		{
			if (!NavalStorylineData.IsNavalStorylineCanceled())
			{
				CampaignEvents.OnAfterSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnAfterSessionLaunched));
			}
		}

		// Token: 0x0600082E RID: 2094 RVA: 0x0003A27F File Offset: 0x0003847F
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x0600082F RID: 2095 RVA: 0x0003A281 File Offset: 0x00038481
		private void OnAfterSessionLaunched(CampaignGameStarter campaignGameSystemStarter)
		{
			this.AddGameMenus(campaignGameSystemStarter);
		}

		// Token: 0x06000830 RID: 2096 RVA: 0x0003A28C File Offset: 0x0003848C
		private void AddGameMenus(CampaignGameStarter campaignGameSystemStarter)
		{
			campaignGameSystemStarter.AddGameMenu("naval_storyline_virtualport", "{=!}{VIRTUAL_PORT_TEXT}", new OnInitDelegate(this.virtual_port_menu_on_init), 2, 0, null);
			campaignGameSystemStarter.AddGameMenuOption("naval_storyline_virtualport", "repair_ships", "{=hqGD0o4E}Repair ships ({TOTAL_AMOUNT}{GOLD_ICON})", new GameMenuOption.OnConditionDelegate(this.virtual_port_menu_repair_ships_on_condition), new GameMenuOption.OnConsequenceDelegate(this.virtual_port_menu_repair_ships_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("naval_storyline_virtualport", "gather_reinforcements", "{=2NRLzk5K}Gather Reinforcements", new GameMenuOption.OnConditionDelegate(this.virtual_port_menu_gather_reinforcements_on_condition), new GameMenuOption.OnConsequenceDelegate(this.virtual_port_menu_gather_reinforcements_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("naval_storyline_virtualport", "trade", "{=GmcgoiGy}Trade", new GameMenuOption.OnConditionDelegate(this.virtual_port_menu_trade_on_condition), new GameMenuOption.OnConsequenceDelegate(this.virtual_port_menu_trade_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("naval_storyline_virtualport", "visit_port", "{=sq7Qoh4Z}Visit the port", new GameMenuOption.OnConditionDelegate(this.visit_port_menu_on_condition), new GameMenuOption.OnConsequenceDelegate(this.visit_port_menu_on_consequence), false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("naval_storyline_virtualport", "naval_storyline_exit", "{=0hA4wOqV}Exit Story Mode", new GameMenuOption.OnConditionDelegate(this.virtual_port_menu_naval_storyline_exit_on_condition), delegate(MenuCallbackArgs x)
			{
				GameMenu.SwitchToMenu("naval_storyline_exit");
			}, false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("naval_storyline_virtualport", "port_leave", "{=fbCbFqyj}Set sail", new GameMenuOption.OnConditionDelegate(this.virtual_port_menu_leave_on_condition), new GameMenuOption.OnConsequenceDelegate(this.virtual_port_menu_leave_on_consequence), true, -1, false, null);
			campaignGameSystemStarter.AddGameMenu("naval_storyline_exit", "{=dV92VE8i}When you leave story mode, you will be returned to Ostican. You can speak to Gunnar in port to try again later. Do you wish to continue?", null, 2, 0, null);
			campaignGameSystemStarter.AddGameMenuOption("naval_storyline_exit", "continue", "{=DM6luo3c}Continue", new GameMenuOption.OnConditionDelegate(this.naval_storyline_exit_continue_on_condition), delegate(MenuCallbackArgs x)
			{
				this.ExitStoryMode();
			}, false, -1, false, null);
			campaignGameSystemStarter.AddGameMenuOption("naval_storyline_exit", "cancel", "{=3CpNUnVl}Cancel", new GameMenuOption.OnConditionDelegate(this.naval_storyline_exit_cancel_on_condition), delegate(MenuCallbackArgs x)
			{
				GameMenu.SwitchToMenu("naval_storyline_virtualport");
			}, true, -1, false, null);
		}

		// Token: 0x06000831 RID: 2097 RVA: 0x0003A47C File Offset: 0x0003867C
		public void virtual_port_menu_on_init(MenuCallbackArgs args)
		{
			MBTextManager.SetTextVariable("VIRTUAL_PORT_TEXT", new TextObject("{=2p7Z6OAb}You are at the port", null), false);
			if (MenuHelper.CheckAndOpenNextLocation(args))
			{
				return;
			}
			string text = Settlement.CurrentSettlement.Culture.StringId + "_port";
			args.MenuContext.SetBackgroundMeshName(text);
			args.MenuContext.SetAmbientSound("event:/map/ambient/node/settlements/2d/port");
			this.UpdateMenuLocations();
			if (NavalStorylinePlayerTownVisitCampaignBehavior.IsPortInteractionDisabled())
			{
				MBTextManager.SetTextVariable("VIRTUAL_PORT_TEXT", new TextObject("{=fs3uB3y4}Gunnar says to return after the siege is over.", null), false);
			}
		}

		// Token: 0x06000832 RID: 2098 RVA: 0x0003A501 File Offset: 0x00038701
		private void UpdateMenuLocations()
		{
			Campaign.Current.GameMenuManager.MenuLocations.Clear();
			Campaign.Current.GameMenuManager.MenuLocations.Add(Settlement.CurrentSettlement.LocationComplex.GetLocationWithId("port"));
		}

		// Token: 0x06000833 RID: 2099 RVA: 0x0003A53F File Offset: 0x0003873F
		private static bool IsPortInteractionDisabled()
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			return currentSettlement == null || currentSettlement.IsUnderSiege;
		}

		// Token: 0x06000834 RID: 2100 RVA: 0x0003A554 File Offset: 0x00038754
		private bool visit_port_menu_on_condition(MenuCallbackArgs args)
		{
			if (NavalStorylinePlayerTownVisitCampaignBehavior.IsPortInteractionDisabled())
			{
				return false;
			}
			List<Location> list = Settlement.CurrentSettlement.LocationComplex.FindAll((string x) => x == "port").ToList<Location>();
			MenuHelper.SetIssueAndQuestDataForLocations(args, list);
			args.optionLeaveType = 1;
			return true;
		}

		// Token: 0x06000835 RID: 2101 RVA: 0x0003A5B0 File Offset: 0x000387B0
		private void visit_port_menu_on_consequence(MenuCallbackArgs args)
		{
			LocationEncounter locationEncounter = PlayerEncounter.LocationEncounter;
			Campaign.Current.GameMenuManager.NextLocation = LocationComplex.Current.GetLocationWithId("port");
			Campaign.Current.GameMenuManager.PreviousLocation = LocationComplex.Current.GetLocationWithId("center");
			PlayerEncounter.LocationEncounter.CreateAndOpenMissionController(Campaign.Current.GameMenuManager.NextLocation, null, null, null);
			Campaign.Current.GameMenuManager.NextLocation = null;
			Campaign.Current.GameMenuManager.PreviousLocation = null;
		}

		// Token: 0x06000836 RID: 2102 RVA: 0x0003A63C File Offset: 0x0003883C
		private bool virtual_port_menu_repair_ships_on_condition(MenuCallbackArgs args)
		{
			if (NavalStorylinePlayerTownVisitCampaignBehavior.IsPortInteractionDisabled())
			{
				return false;
			}
			float goldCostToRepairShips = this.GetGoldCostToRepairShips();
			if (goldCostToRepairShips > 0f)
			{
				if (goldCostToRepairShips > (float)Hero.MainHero.Gold)
				{
					args.IsEnabled = false;
					args.Tooltip = new TextObject("{=d0kbtGYn}You don't have enough gold.", null);
				}
				MBTextManager.SetTextVariable("TOTAL_AMOUNT", (int)goldCostToRepairShips);
				return true;
			}
			return false;
		}

		// Token: 0x06000837 RID: 2103 RVA: 0x0003A698 File Offset: 0x00038898
		private float GetGoldCostToRepairShips()
		{
			float num = 0f;
			foreach (Ship ship in MobileParty.MainParty.Ships)
			{
				if (ship.HitPoints < ship.MaxHitPoints)
				{
					num += Campaign.Current.Models.ShipCostModel.GetShipRepairCost(ship, PartyBase.MainParty);
				}
			}
			return num;
		}

		// Token: 0x06000838 RID: 2104 RVA: 0x0003A71C File Offset: 0x0003891C
		private void virtual_port_menu_repair_ships_on_consequence(MenuCallbackArgs args)
		{
			foreach (Ship ship in MobileParty.MainParty.Ships)
			{
				if (ship.HitPoints < ship.MaxHitPoints)
				{
					RepairShipAction.Apply(ship, Settlement.CurrentSettlement);
				}
			}
			args.MenuContext.Refresh();
		}

		// Token: 0x06000839 RID: 2105 RVA: 0x0003A790 File Offset: 0x00038990
		private bool virtual_port_menu_gather_reinforcements_on_condition(MenuCallbackArgs args)
		{
			if (NavalStorylinePlayerTownVisitCampaignBehavior.IsPortInteractionDisabled())
			{
				return false;
			}
			args.optionLeaveType = 37;
			NavalStorylinePartyData navalStorylinePartyData;
			if (MobileParty.MainParty.IsNavalStorylineQuestParty(out navalStorylinePartyData) && navalStorylinePartyData != null && navalStorylinePartyData.IsQuestParty)
			{
				int num = PartyBase.MainParty.Ships.Where<Ship>((Ship s) => s.IsUsedByQuest).Count<Ship>();
				int num2 = 0;
				foreach (ShipTemplateStack shipTemplateStack in navalStorylinePartyData.Template.ShipHulls)
				{
					num2 += shipTemplateStack.MaxValue;
				}
				if (MobileParty.MainParty.MemberRoster.TotalManCount >= MobileParty.MainParty.Party.PartySizeLimit && num2 <= num)
				{
					args.IsEnabled = false;
					args.Tooltip = new TextObject("{=Tbg46Xm3}Party does not need any more reinforcement.", null);
				}
				return true;
			}
			return false;
		}

		// Token: 0x0600083A RID: 2106 RVA: 0x0003A894 File Offset: 0x00038A94
		private void virtual_port_menu_gather_reinforcements_on_consequence(MenuCallbackArgs args)
		{
			NavalStorylinePartyData navalStorylinePartyData;
			if (MobileParty.MainParty.Party.IsNavalStorylineQuestParty(out navalStorylinePartyData) && navalStorylinePartyData != null && navalStorylinePartyData.IsQuestParty)
			{
				this.ReinforceMainParty(navalStorylinePartyData);
				args.MenuContext.Refresh();
			}
		}

		// Token: 0x0600083B RID: 2107 RVA: 0x0003A8D4 File Offset: 0x00038AD4
		private void ReinforceMainParty(NavalStorylinePartyData data)
		{
			int totalManCount = MobileParty.MainParty.MemberRoster.TotalManCount;
			int num = data.PartySize - totalManCount;
			int num2 = (int)NavalDLCHelpers.GetMaxPartySizeLimitFromTemplate(data.Template).ResultNumber;
			float num3 = (float)num / (float)num2;
			foreach (PartyTemplateStack partyTemplateStack in data.Template.Stacks)
			{
				CharacterObject character = partyTemplateStack.Character;
				int num4 = MathF.Floor((float)partyTemplateStack.MaxValue * num3);
				num -= num4;
				MobileParty.MainParty.MemberRoster.AddToCounts(character, num4, false, 0, 0, true, -1);
			}
			for (int i = 0; i < num; i++)
			{
				int num5 = MBRandom.RandomInt(data.Template.Stacks.Count);
				CharacterObject character2 = data.Template.Stacks[num5].Character;
				MobileParty.MainParty.MemberRoster.AddToCounts(character2, 1, false, 0, 0, true, -1);
			}
			List<Ship> list = PartyBase.MainParty.Ships.Where<Ship>((Ship s) => s.IsUsedByQuest).ToList<Ship>();
			using (List<ShipTemplateStack>.Enumerator enumerator2 = data.Template.ShipHulls.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					ShipTemplateStack stack = enumerator2.Current;
					int num6 = list.Where<Ship>((Ship s) => s.ShipHull.StringId == stack.ShipHull.StringId).Count<Ship>();
					if (num6 < stack.MaxValue)
					{
						for (int j = 0; j < stack.MaxValue - num6; j++)
						{
							Ship ship = new Ship(stack.ShipHull)
							{
								IsTradeable = false,
								IsUsedByQuest = true
							};
							ChangeShipOwnerAction.ApplyByMobilePartyCreation(PartyBase.MainParty, ship);
						}
					}
				}
			}
		}

		// Token: 0x0600083C RID: 2108 RVA: 0x0003AAE8 File Offset: 0x00038CE8
		private bool virtual_port_menu_trade_on_condition(MenuCallbackArgs args)
		{
			if (NavalStorylinePlayerTownVisitCampaignBehavior.IsPortInteractionDisabled())
			{
				return false;
			}
			bool flag2;
			TextObject textObject;
			bool flag = Campaign.Current.Models.SettlementAccessModel.CanMainHeroDoSettlementAction(Settlement.CurrentSettlement, 5, ref flag2, ref textObject);
			args.optionLeaveType = 14;
			return MenuHelper.SetOptionProperties(args, flag, flag2, textObject);
		}

		// Token: 0x0600083D RID: 2109 RVA: 0x0003AB2E File Offset: 0x00038D2E
		private void virtual_port_menu_trade_on_consequence(MenuCallbackArgs args)
		{
			LocationEncounter locationEncounter = PlayerEncounter.LocationEncounter;
			InventoryScreenHelper.OpenScreenAsTrade(Settlement.CurrentSettlement.ItemRoster, Settlement.CurrentSettlement.Town, -1, null);
		}

		// Token: 0x0600083E RID: 2110 RVA: 0x0003AB51 File Offset: 0x00038D51
		private bool naval_storyline_exit_continue_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 17;
			return true;
		}

		// Token: 0x0600083F RID: 2111 RVA: 0x0003AB5C File Offset: 0x00038D5C
		private bool naval_storyline_exit_cancel_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 16;
			return true;
		}

		// Token: 0x06000840 RID: 2112 RVA: 0x0003AB67 File Offset: 0x00038D67
		private bool virtual_port_menu_naval_storyline_exit_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 21;
			return true;
		}

		// Token: 0x06000841 RID: 2113 RVA: 0x0003AB72 File Offset: 0x00038D72
		private void ExitStoryMode()
		{
			NavalStorylineData.DeactivateNavalStoryline();
		}

		// Token: 0x06000842 RID: 2114 RVA: 0x0003AB79 File Offset: 0x00038D79
		private bool virtual_port_menu_leave_on_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 43;
			return true;
		}

		// Token: 0x06000843 RID: 2115 RVA: 0x0003AB84 File Offset: 0x00038D84
		private void virtual_port_menu_leave_on_consequence(MenuCallbackArgs args)
		{
			MobileParty.MainParty.SetSailAtPosition(Settlement.CurrentSettlement.PortPosition);
			PlayerEncounter.Finish(true);
		}
	}
}
