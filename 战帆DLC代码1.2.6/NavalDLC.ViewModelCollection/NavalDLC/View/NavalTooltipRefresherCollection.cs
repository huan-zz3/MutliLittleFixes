using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.View
{
	// Token: 0x02000004 RID: 4
	public static class NavalTooltipRefresherCollection
	{
		// Token: 0x06000003 RID: 3 RVA: 0x00002058 File Offset: 0x00000258
		public static void RefreshShipTooltip(PropertyBasedTooltipVM propertyBasedTooltipVM, object[] args)
		{
			if (args == null || args.Length == 0)
			{
				Debug.FailedAssert("Invalid ship hull arguments for tooltip", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.ViewModelCollection\\NavalTooltipRefresherCollection.cs", "RefreshShipTooltip", 28);
				return;
			}
			Ship ship;
			if ((ship = args[0] as Ship) == null)
			{
				Debug.FailedAssert("Invalid ship hull arguments for tooltip", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.ViewModelCollection\\NavalTooltipRefresherCollection.cs", "RefreshShipTooltip", 34);
				return;
			}
			propertyBasedTooltipVM.Mode = 1;
			propertyBasedTooltipVM.AddProperty(ship.Name.ToString(), string.Empty, 0, 4096);
			propertyBasedTooltipVM.AddProperty(new TextObject("{=sqdzHOPe}Class", null).ToString(), GameTexts.FindText("str_ship_type", ship.ShipHull.Type.ToString().ToLowerInvariant()).ToString(), 0, 0);
			propertyBasedTooltipVM.AddProperty(new TextObject("{=UbZL2BJQ}Hitpoints", null).ToString(), ship.MaxHitPoints.ToString(), 0, 0);
			int num = ship.TotalCrewCapacity - ship.MainDeckCrewCapacity;
			string text;
			if (num > 0)
			{
				text = new TextObject("{=r2fvxfwZ}{TOTAL} ({MAIN_DECK}+{RESERVE})", null).SetTextVariable("TOTAL", ship.TotalCrewCapacity.ToString()).SetTextVariable("MAIN_DECK", ship.MainDeckCrewCapacity.ToString()).SetTextVariable("RESERVE", num.ToString())
					.ToString();
			}
			else
			{
				text = ship.TotalCrewCapacity.ToString();
			}
			propertyBasedTooltipVM.AddProperty(new TextObject("{=oqVVGxgb}Crew Capacity", null).ToString(), text, 0, 0);
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000021C8 File Offset: 0x000003C8
		public static void RefreshShipHullTooltip(PropertyBasedTooltipVM propertyBasedTooltipVM, object[] args)
		{
			if (args == null || args.Length == 0)
			{
				Debug.FailedAssert("Invalid ship hull arguments for tooltip", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.ViewModelCollection\\NavalTooltipRefresherCollection.cs", "RefreshShipHullTooltip", 67);
				return;
			}
			ShipHull shipHull;
			if ((shipHull = args[0] as ShipHull) == null)
			{
				Debug.FailedAssert("Invalid ship hull arguments for tooltip", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.ViewModelCollection\\NavalTooltipRefresherCollection.cs", "RefreshShipHullTooltip", 73);
				return;
			}
			propertyBasedTooltipVM.Mode = 1;
			propertyBasedTooltipVM.AddProperty(shipHull.Name.ToString(), string.Empty, 0, 4096);
			propertyBasedTooltipVM.AddProperty(new TextObject("{=sqdzHOPe}Class", null).ToString(), GameTexts.FindText("str_ship_type", shipHull.Type.ToString().ToLowerInvariant()).ToString(), 0, 0);
			propertyBasedTooltipVM.AddProperty(new TextObject("{=UbZL2BJQ}Hitpoints", null).ToString(), shipHull.MaxHitPoints.ToString(), 0, 0);
			int num = shipHull.TotalCrewCapacity - shipHull.MainDeckCrewCapacity;
			string text;
			if (num > 0)
			{
				text = new TextObject("{=r2fvxfwZ}{TOTAL} ({MAIN_DECK}+{RESERVE})", null).SetTextVariable("TOTAL", shipHull.TotalCrewCapacity.ToString()).SetTextVariable("MAIN_DECK", shipHull.MainDeckCrewCapacity.ToString()).SetTextVariable("RESERVE", num.ToString())
					.ToString();
			}
			else
			{
				text = shipHull.TotalCrewCapacity.ToString();
			}
			propertyBasedTooltipVM.AddProperty(new TextObject("{=oqVVGxgb}Crew Capacity", null).ToString(), text, 0, 0);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002334 File Offset: 0x00000534
		public static void RefreshShipPieceTooltip(PropertyBasedTooltipVM propertyBasedTooltipVM, object[] args)
		{
			if (args == null || args.Length == 0)
			{
				Debug.FailedAssert("Invalid ship piece arguments for tooltip", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.ViewModelCollection\\NavalTooltipRefresherCollection.cs", "RefreshShipPieceTooltip", 105);
				return;
			}
			ShipUpgradePiece shipUpgradePiece = args[0] as ShipUpgradePiece;
			if (shipUpgradePiece == null)
			{
				Debug.FailedAssert("Invalid ship piece arguments for tooltip", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.ViewModelCollection\\NavalTooltipRefresherCollection.cs", "RefreshShipPieceTooltip", 112);
				return;
			}
			bool flag = false;
			object obj;
			if (args.Length > 1 && (obj = args[1]) is bool)
			{
				bool flag2 = (bool)obj;
				flag = flag2;
			}
			propertyBasedTooltipVM.Mode = 1;
			propertyBasedTooltipVM.AddProperty(shipUpgradePiece.GetName().ToString(), "", 0, 4096);
			if (flag)
			{
				if (shipUpgradePiece.RequiredCulture1 != null && shipUpgradePiece.RequiredCulture2 != null)
				{
					TextObject commaSeparatedText = CampaignUIHelper.GetCommaSeparatedText(null, new TextObject[]
					{
						shipUpgradePiece.RequiredCulture1.Name,
						shipUpgradePiece.RequiredCulture2.Name
					});
					propertyBasedTooltipVM.AddProperty(new TextObject("{=n0R6yfth}Required Cultures", null).ToString(), commaSeparatedText.ToString(), 0, 0);
				}
				else if (shipUpgradePiece.RequiredCulture1 != null || shipUpgradePiece.RequiredCulture2 != null)
				{
					BasicCultureObject basicCultureObject = shipUpgradePiece.RequiredCulture1 ?? shipUpgradePiece.RequiredCulture2;
					propertyBasedTooltipVM.AddProperty(new TextObject("{=11c9lb6E}Required Culture", null).ToString(), basicCultureObject.Name.ToString(), 0, 0);
				}
				propertyBasedTooltipVM.AddProperty(new TextObject("{=gGWVrUPh}Required Port Level", null).ToString(), shipUpgradePiece.RequiredPortLevel.ToString(), 0, 0);
				return;
			}
			TextObject textObject = GameTexts.FindText("str_plus_with_number", null);
			if (shipUpgradePiece.SeaWorthinessBonus != 0)
			{
				textObject.SetTextVariable("NUMBER", shipUpgradePiece.SeaWorthinessBonus);
				propertyBasedTooltipVM.AddProperty(new TextObject("{=cN03zpII}Seaworthiness", null).ToString(), textObject.ToString(), 0, 0);
			}
			if (shipUpgradePiece.AdditionalAmmoBonus != 0)
			{
				textObject.SetTextVariable("NUMBER", shipUpgradePiece.AdditionalAmmoBonus);
				propertyBasedTooltipVM.AddProperty(new TextObject("{=pJz8SBGB}Additional Ammo Bonus", null).ToString(), textObject.ToString(), 0, 0);
			}
			if (shipUpgradePiece.ArcherQuiverBonus != 0)
			{
				textObject.SetTextVariable("NUMBER", shipUpgradePiece.ArcherQuiverBonus);
				propertyBasedTooltipVM.AddProperty(new TextObject("{=EqJiCbQL}Quivers", null).ToString(), textObject.ToString(), 0, 0);
			}
			if (shipUpgradePiece.ThrowingWeaponStackBonus != 0)
			{
				textObject.SetTextVariable("NUMBER", shipUpgradePiece.ThrowingWeaponStackBonus);
				propertyBasedTooltipVM.AddProperty(new TextObject("{=bbAzBnhC}Throwing Weapon Stacks", null).ToString(), textObject.ToString(), 0, 0);
			}
			TextObject textObject2 = GameTexts.FindText("str_NUMBER_percent", null);
			if (shipUpgradePiece.CrewCapacityBonusMultiplier != 0f)
			{
				textObject2.SetTextVariable("NUMBER", (shipUpgradePiece.CrewCapacityBonusMultiplier * 100f).ToString("#"));
				propertyBasedTooltipVM.AddProperty(new TextObject("{=oqVVGxgb}Crew Capacity", null).ToString(), textObject2.ToString(), 0, 0);
			}
			if (shipUpgradePiece.ShipWeightBonusMultiplier != 0f)
			{
				textObject2.SetTextVariable("NUMBER", (shipUpgradePiece.ShipWeightBonusMultiplier * 100f).ToString("#"));
				propertyBasedTooltipVM.AddProperty(new TextObject("{=4Dd2xgPm}Weight", null).ToString(), textObject2.ToString(), 0, 0);
			}
			if (shipUpgradePiece.DecreaseForwardDragMultiplier != 0f)
			{
				textObject2.SetTextVariable("NUMBER", (shipUpgradePiece.DecreaseForwardDragMultiplier * 100f).ToString("#"));
				propertyBasedTooltipVM.AddProperty(new TextObject("{=AOpCa0ZB}Top Speed", null).ToString(), textObject2.ToString(), 0, 0);
			}
			if (shipUpgradePiece.CampaignSpeedBonusMultiplier != 0f)
			{
				textObject2.SetTextVariable("NUMBER", (shipUpgradePiece.CampaignSpeedBonusMultiplier * 100f).ToString("#"));
				propertyBasedTooltipVM.AddProperty(new TextObject("{=DbERaPfF}Travel Speed", null).ToString(), textObject2.ToString(), 0, 0);
			}
			if (shipUpgradePiece.MaxHitPointsBonusMultiplier != 0f)
			{
				textObject2.SetTextVariable("NUMBER", (shipUpgradePiece.MaxHitPointsBonusMultiplier * 100f).ToString("#"));
				propertyBasedTooltipVM.AddProperty(new TextObject("{=lfEJZZfG}Ship Hitpoints", null).ToString(), textObject2.ToString(), 0, 0);
			}
			if (shipUpgradePiece.MaxSailHitPointsBonusMultiplier != 0f)
			{
				textObject2.SetTextVariable("NUMBER", (shipUpgradePiece.MaxSailHitPointsBonusMultiplier * 100f).ToString("#"));
				propertyBasedTooltipVM.AddProperty(new TextObject("{=EAnQtOuG}Sail Hitpoints", null).ToString(), textObject2.ToString(), 0, 0);
			}
			if (shipUpgradePiece.CrewShieldHitPointsBonusMultiplier != 0f)
			{
				textObject2.SetTextVariable("NUMBER", (shipUpgradePiece.CrewShieldHitPointsBonusMultiplier * 100f).ToString("#"));
				propertyBasedTooltipVM.AddProperty(new TextObject("{=4ZbgDw60}Crew Shield Hitpoints", null).ToString(), textObject2.ToString(), 0, 0);
			}
			if (shipUpgradePiece.InventoryCapacityBonusMultiplier != 0f)
			{
				textObject2.SetTextVariable("NUMBER", (shipUpgradePiece.InventoryCapacityBonusMultiplier * 100f).ToString("#"));
				propertyBasedTooltipVM.AddProperty(new TextObject("{=IE1KbkaH}Cargo Capacity", null).ToString(), textObject2.ToString(), 0, 0);
			}
			if (shipUpgradePiece.MaxOarPowerBonusMultiplier != 0f)
			{
				textObject2.SetTextVariable("NUMBER", (shipUpgradePiece.MaxOarPowerBonusMultiplier * 100f).ToString("#"));
				propertyBasedTooltipVM.AddProperty(new TextObject("{=VLugPMkM}Oar Speed", null).ToString(), textObject2.ToString(), 0, 0);
			}
			if (shipUpgradePiece.MaxOarForceBonusMultiplier != 0f)
			{
				textObject2.SetTextVariable("NUMBER", (shipUpgradePiece.MaxOarForceBonusMultiplier * 100f).ToString("#"));
				propertyBasedTooltipVM.AddProperty(new TextObject("{=gOM8Eibs}Oar Power", null).ToString(), textObject2.ToString(), 0, 0);
			}
			if (shipUpgradePiece.SailForceBonusMultiplier != 0f)
			{
				textObject2.SetTextVariable("NUMBER", (shipUpgradePiece.SailForceBonusMultiplier * 100f).ToString("#"));
				propertyBasedTooltipVM.AddProperty(new TextObject("{=ruAdMru6}Sail Power", null).ToString(), textObject2.ToString(), 0, 0);
			}
			if (shipUpgradePiece.CrewMeleeDamageBonusMultiplier != 0f)
			{
				textObject2.SetTextVariable("NUMBER", (shipUpgradePiece.CrewMeleeDamageBonusMultiplier * 100f).ToString("#"));
				propertyBasedTooltipVM.AddProperty(new TextObject("{=vGqCgA6v}Crew Melee Damage", null).ToString(), textObject2.ToString(), 0, 0);
			}
			if (shipUpgradePiece.SailRotationSpeedBonusMultiplier != 0f)
			{
				textObject2.SetTextVariable("NUMBER", (shipUpgradePiece.SailRotationSpeedBonusMultiplier * 100f).ToString("#"));
				propertyBasedTooltipVM.AddProperty(new TextObject("{=idjVMLKe}Sail Rotation Speed", null).ToString(), textObject2.ToString(), 0, 0);
			}
			if (shipUpgradePiece.RudderSurfaceAreaBonusMultiplier != 0f)
			{
				textObject2.SetTextVariable("NUMBER", (shipUpgradePiece.RudderSurfaceAreaBonusMultiplier * 100f).ToString("#"));
				propertyBasedTooltipVM.AddProperty(new TextObject("{=b6dbh1uN}Rudder Effectiveness", null).ToString(), textObject2.ToString(), 0, 0);
			}
			if (shipUpgradePiece.MaxRudderForceBonusMultiplier != 0f)
			{
				textObject2.SetTextVariable("NUMBER", (shipUpgradePiece.MaxRudderForceBonusMultiplier * 100f).ToString("#"));
				propertyBasedTooltipVM.AddProperty(new TextObject("{=djdlcniG}Rudder Power", null).ToString(), textObject2.ToString(), 0, 0);
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002A70 File Offset: 0x00000C70
		public static void RefreshFigureheadTooltip(PropertyBasedTooltipVM propertyBasedTooltipVM, object[] args)
		{
			Figurehead figurehead;
			if (args == null || args.Length == 0 || (figurehead = args[0] as Figurehead) == null)
			{
				Debug.FailedAssert("Invalid arguments for figurehead tooltip", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.ViewModelCollection\\NavalTooltipRefresherCollection.cs", "RefreshFigureheadTooltip", 288);
				return;
			}
			propertyBasedTooltipVM.Mode = 1;
			propertyBasedTooltipVM.AddProperty(figurehead.Name.ToString(), "", 0, 4096);
			if (figurehead.Culture != null)
			{
				propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_culture", null).ToString(), figurehead.Culture.Name.ToString(), 0, 0);
			}
			StringHelpers.SetEffectIncrementTypeTextVariable("EFFECT_AMOUNT", figurehead.Description, figurehead.EffectAmount, figurehead.EffectIncrementType);
			propertyBasedTooltipVM.AddProperty(new TextObject("{=opVqBNLh}Effect", null).ToString(), figurehead.Description.ToString(), 0, 0);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002B3C File Offset: 0x00000D3C
		public static void RefreshAnchorPointTooltip(PropertyBasedTooltipVM propertyBasedTooltipVM, object[] args)
		{
			AnchorPoint anchorPoint;
			if (args == null || args.Length == 0 || (anchorPoint = args[0] as AnchorPoint) == null)
			{
				Debug.FailedAssert("Invalid anchor arguments for tooltip", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.ViewModelCollection\\NavalTooltipRefresherCollection.cs", "RefreshAnchorPointTooltip", 312);
				return;
			}
			if (!anchorPoint.IsValid)
			{
				Debug.FailedAssert("Anchor tooltip should not be visible when its not at a valid position", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.ViewModelCollection\\NavalTooltipRefresherCollection.cs", "RefreshAnchorPointTooltip", 318);
				return;
			}
			propertyBasedTooltipVM.Mode = 1;
			propertyBasedTooltipVM.AddProperty(anchorPoint.Name.ToString(), "", 0, 4096);
			if (!anchorPoint.IsMovingToPoint)
			{
				MBReadOnlyList<Settlement> all = Settlement.All;
				Settlement settlement = null;
				for (int i = 0; i < all.Count; i++)
				{
					if (all[i].HasPort && anchorPoint.IsAtSettlement(all[i]))
					{
						settlement = all[i];
						break;
					}
				}
				if (settlement != null)
				{
					TextObject textObject = new TextObject("{=a6vEx1tM}Anchored at {SETTLEMENT}", null).SetTextVariable("SETTLEMENT", settlement.Name.ToString());
					propertyBasedTooltipVM.AddProperty("", textObject.ToString(), 0, 1);
				}
			}
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002C3C File Offset: 0x00000E3C
		public static void RefreshSettlementTooltip(PropertyBasedTooltipVM propertyBasedTooltipVM, object[] args)
		{
			Settlement settlement = args[0] as Settlement;
			PartyBase settlementAsParty = settlement.Party;
			if (settlementAsParty == null)
			{
				return;
			}
			if (FactionManager.IsAtWarAgainstFaction(settlementAsParty.MapFaction, PartyBase.MainParty.MapFaction))
			{
				propertyBasedTooltipVM.Mode = 3;
			}
			else if (settlementAsParty.MapFaction == PartyBase.MainParty.MapFaction || DiplomacyHelper.IsSameFactionAndNotEliminated(settlementAsParty.MapFaction, PartyBase.MainParty.MapFaction))
			{
				propertyBasedTooltipVM.Mode = 2;
			}
			else
			{
				propertyBasedTooltipVM.Mode = 1;
			}
			if (Game.Current.IsDevelopmentMode)
			{
				string text = settlement.Name.ToString();
				int num = 1;
				string text2 = "";
				if (settlement.IsHideout)
				{
					text2 = settlement.LocationComplex.GetScene("hideout_center", num);
					propertyBasedTooltipVM.AddProperty("", string.Concat(new string[] { text, "( id: ", settlementAsParty.Id, ")\n(Scene: ", text2, ")" }), 1, 0);
				}
				else
				{
					if (settlement.IsFortification)
					{
						num = settlement.Town.GetWallLevel();
						text2 = settlement.LocationComplex.GetScene("center", num);
					}
					else if (settlement.IsVillage)
					{
						text2 = settlement.LocationComplex.GetScene("village_center", num);
					}
					propertyBasedTooltipVM.AddProperty("", text + " (" + text2 + ")", 0, 4096);
				}
				if (settlement.IsFortification)
				{
					propertyBasedTooltipVM.AddProperty("", "", 0, 512);
					string text3 = "[DEBUG WALL DATA]\n";
					text3 = string.Concat(new object[]
					{
						text3,
						"Current wall level: ",
						settlement.Town.GetWallLevel(),
						"\n"
					});
					text3 = string.Concat(new object[] { text3, "Current wall hp: ", settlement.SettlementTotalWallHitPoints, "\n" });
					text3 = string.Concat(new object[] { text3, "Max wall hp: ", settlement.MaxWallHitPoints, "\n" });
					propertyBasedTooltipVM.AddProperty("", text3, 0, 4096);
				}
			}
			else
			{
				propertyBasedTooltipVM.AddProperty("", settlement.Name.ToString(), 0, 4096);
			}
			TextObject textObject;
			bool flag = !CampaignUIHelper.IsSettlementInformationHidden(settlement, ref textObject);
			propertyBasedTooltipVM.AddProperty(string.Empty, string.Empty, -1, 0);
			propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_owner", null).ToString(), " ", 0, 0);
			propertyBasedTooltipVM.AddProperty("", "", 0, 512);
			TextObject textObject2 = new TextObject("{=!}{PARTY_OWNERS_FACTION}", null);
			TextObject textObject3 = ((settlement.OwnerClan == null) ? new TextObject("{=3PzgpFGq}Neutral", null) : settlement.OwnerClan.Name);
			textObject2.SetTextVariable("PARTY_OWNERS_FACTION", textObject3);
			propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_clan", null).ToString(), textObject2.ToString(), 0, 0);
			if (settlementAsParty.MapFaction != null)
			{
				TextObject textObject4 = new TextObject("{=!}{MAP_FACTION}", null);
				TextObject textObject5 = textObject4;
				string text4 = "MAP_FACTION";
				IFaction mapFaction = settlementAsParty.MapFaction;
				textObject5.SetTextVariable(text4, ((mapFaction != null) ? mapFaction.Name : null) ?? new TextObject("{=!}ERROR", null));
				propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_faction", null).ToString(), textObject4.ToString(), 0, 0);
			}
			if (settlement.Culture != null && !TextObject.IsNullOrEmpty(settlement.Culture.Name))
			{
				TextObject textObject6 = new TextObject("{=!}{CULTURE}", null);
				textObject6.SetTextVariable("CULTURE", settlement.Culture.Name);
				propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_culture", null).ToString(), textObject6.ToString(), 0, 0);
			}
			if (flag)
			{
				if (settlementAsParty.IsSettlement && (settlementAsParty.Settlement.IsVillage || settlementAsParty.Settlement.IsTown || settlementAsParty.Settlement.IsCastle))
				{
					propertyBasedTooltipVM.AddProperty(string.Empty, string.Empty, -1, 0);
					propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_information", null).ToString(), " ", 0, 0);
					propertyBasedTooltipVM.AddProperty("", "", 0, 512);
				}
				if (settlement.IsFortification)
				{
					int wallLevel = settlementAsParty.Settlement.Town.GetWallLevel();
					propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_map_tooltip_wall_level", null).ToString(), wallLevel.ToString(), 0, 0);
				}
				Town town = settlement.Town;
				Building building = ((town != null) ? town.GetShipyard() : null);
				if (building != null)
				{
					propertyBasedTooltipVM.AddProperty(new TextObject("{=NfhYN9yt}Shipyard Level", null).ToString(), building.CurrentLevel.ToString(), 0, 0);
				}
				if (settlement.IsFortification)
				{
					Func<string> func = delegate
					{
						int num5 = (int)settlementAsParty.Settlement.Town.FoodChange;
						int num6 = (int)settlementAsParty.Settlement.Town.FoodStocks;
						TextObject textObject9 = new TextObject("{=Jyfkahka}{VALUE} ({?POSITIVE}+{?}{\\?}{DELTA_VALUE})", null);
						textObject9.SetTextVariable("VALUE", num6);
						textObject9.SetTextVariable("POSITIVE", (num5 > 0) ? 1 : 0);
						textObject9.SetTextVariable("DELTA_VALUE", num5);
						return textObject9.ToString();
					};
					propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_map_tooltip_food_stocks", null).ToString(), func, 0, 0);
				}
				if (settlement.IsVillage || settlement.IsFortification)
				{
					Func<string> func2 = delegate
					{
						float num7 = (settlementAsParty.Settlement.IsFortification ? settlementAsParty.Settlement.Town.ProsperityChange : settlementAsParty.Settlement.Village.HearthChange);
						int num8 = (int)(settlementAsParty.Settlement.IsFortification ? settlementAsParty.Settlement.Town.Prosperity : settlementAsParty.Settlement.Village.Hearth);
						TextObject textObject10 = new TextObject("{=Jyfkahka}{VALUE} ({?POSITIVE}+{?}{\\?}{DELTA_VALUE})", null);
						textObject10.SetTextVariable("VALUE", num8);
						textObject10.SetTextVariable("POSITIVE", (num7 > 0f) ? 1 : 0);
						textObject10.SetTextVariable("DELTA_VALUE", num7, 2);
						return textObject10.ToString();
					};
					propertyBasedTooltipVM.AddProperty(settlementAsParty.Settlement.IsFortification ? GameTexts.FindText("str_map_tooltip_prosperity", null).ToString() : GameTexts.FindText("str_map_tooltip_hearths", null).ToString(), func2, 0, 0);
				}
				if (settlement.IsFortification)
				{
					Func<string> func3 = delegate
					{
						TextObject textObject11 = new TextObject("{=Jyfkahka}{VALUE} ({?POSITIVE}+{?}{\\?}{DELTA_VALUE})", null);
						textObject11.SetTextVariable("VALUE", settlement.Town.Loyalty, 2);
						textObject11.SetTextVariable("POSITIVE", (settlement.Town.LoyaltyChange > 0f) ? 1 : 0);
						textObject11.SetTextVariable("DELTA_VALUE", settlement.Town.LoyaltyChange, 2);
						return textObject11.ToString();
					};
					propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_loyalty", null).ToString(), func3, 0, 0);
					Func<string> func4 = delegate
					{
						TextObject textObject12 = new TextObject("{=Jyfkahka}{VALUE} ({?POSITIVE}+{?}{\\?}{DELTA_VALUE})", null);
						textObject12.SetTextVariable("VALUE", settlement.Town.Security, 2);
						textObject12.SetTextVariable("POSITIVE", (settlement.Town.SecurityChange > 0f) ? 1 : 0);
						textObject12.SetTextVariable("DELTA_VALUE", settlement.Town.SecurityChange, 2);
						return textObject12.ToString();
					};
					propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_security", null).ToString(), func4, 0, 0);
				}
			}
			if (settlement.IsVillage)
			{
				string text5 = GameTexts.FindText("str_bound_settlement", null).ToString();
				string text6 = settlementAsParty.Settlement.Village.Bound.Name.ToString();
				propertyBasedTooltipVM.AddProperty(text5, text6, 0, 0);
				if (settlementAsParty.Settlement.Village.TradeBound != null)
				{
					string text7 = GameTexts.FindText("str_trade_bound_settlement", null).ToString();
					string text8 = settlementAsParty.Settlement.Village.TradeBound.Name.ToString();
					propertyBasedTooltipVM.AddProperty(text7, text8, 0, 0);
				}
				ItemObject primaryProduction = settlementAsParty.Settlement.Village.VillageType.PrimaryProduction;
				propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_primary_production", null).ToString(), primaryProduction.Name.ToString(), 0, 0);
			}
			if (settlement.BoundVillages.Count > 0)
			{
				string text9 = GameTexts.FindText("str_bound_village", null).ToString();
				IEnumerable<TextObject> enumerable = settlementAsParty.Settlement.BoundVillages.Select<Village, TextObject>((Village x) => x.Name);
				propertyBasedTooltipVM.AddProperty(text9, CampaignUIHelper.GetCommaNewlineSeparatedText(TextObject.GetEmpty(), enumerable).ToString(), 0, 0);
				if (propertyBasedTooltipVM.IsExtended && settlement.IsTown && settlement.Town.TradeBoundVillages.Count > 0)
				{
					string text10 = GameTexts.FindText("str_trade_bound_village", null).ToString();
					IEnumerable<TextObject> enumerable2 = settlement.Town.TradeBoundVillages.Select<Village, TextObject>((Village x) => x.Name);
					propertyBasedTooltipVM.AddProperty(text10, CampaignUIHelper.GetCommaNewlineSeparatedText(TextObject.GetEmpty(), enumerable2).ToString(), 0, 0);
				}
			}
			if (Game.Current.IsDevelopmentMode && settlement.IsTown)
			{
				propertyBasedTooltipVM.AddProperty(string.Empty, string.Empty, -1, 0);
				propertyBasedTooltipVM.AddProperty("[DEV] " + GameTexts.FindText("str_shops", null).ToString(), " ", 0, 0);
				propertyBasedTooltipVM.AddProperty("", "", 0, 512);
				int num2 = 1;
				foreach (Workshop workshop in settlementAsParty.Settlement.Town.Workshops)
				{
					if (workshop.WorkshopType != null)
					{
						propertyBasedTooltipVM.AddProperty("[DEV] Shop " + num2.ToString(), workshop.WorkshopType.Name.ToString(), 0, 0);
						num2++;
					}
				}
			}
			TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
			TroopRoster troopRoster2 = TroopRoster.CreateDummyTroopRoster();
			TroopRoster.CreateDummyTroopRoster();
			Func<TroopRoster> func5 = delegate
			{
				TroopRoster troopRoster3 = TroopRoster.CreateDummyTroopRoster();
				foreach (MobileParty mobileParty4 in settlement.Parties)
				{
					if (!FactionManager.IsAtWarAgainstFaction(mobileParty4.MapFaction, settlementAsParty.MapFaction) && (mobileParty4.Aggressiveness >= 0.01f || mobileParty4.IsGarrison || mobileParty4.IsMilitia) && !mobileParty4.IsMainParty)
					{
						for (int k = 0; k < mobileParty4.MemberRoster.Count; k++)
						{
							TroopRosterElement elementCopyAtIndex = mobileParty4.MemberRoster.GetElementCopyAtIndex(k);
							troopRoster3.AddToCounts(elementCopyAtIndex.Character, elementCopyAtIndex.Number, false, elementCopyAtIndex.WoundedNumber, 0, true, -1);
						}
					}
				}
				return troopRoster3;
			};
			Func<TroopRoster> func6 = delegate
			{
				TroopRoster troopRoster4 = TroopRoster.CreateDummyTroopRoster();
				foreach (MobileParty mobileParty5 in settlement.Parties)
				{
					if (!mobileParty5.IsMainParty && !FactionManager.IsAtWarAgainstFaction(mobileParty5.MapFaction, settlementAsParty.MapFaction))
					{
						for (int l = 0; l < mobileParty5.PrisonRoster.Count; l++)
						{
							TroopRosterElement elementCopyAtIndex2 = mobileParty5.PrisonRoster.GetElementCopyAtIndex(l);
							troopRoster4.AddToCounts(elementCopyAtIndex2.Character, elementCopyAtIndex2.Number, false, elementCopyAtIndex2.WoundedNumber, 0, true, -1);
						}
					}
				}
				for (int m = 0; m < settlementAsParty.PrisonRoster.Count; m++)
				{
					TroopRosterElement elementCopyAtIndex3 = settlementAsParty.PrisonRoster.GetElementCopyAtIndex(m);
					troopRoster4.AddToCounts(elementCopyAtIndex3.Character, elementCopyAtIndex3.Number, false, elementCopyAtIndex3.WoundedNumber, 0, true, -1);
				}
				return troopRoster4;
			};
			troopRoster2 = func6();
			if (!settlement.IsHideout && propertyBasedTooltipVM.IsExtended)
			{
				troopRoster = func5();
				if (troopRoster.Count > 0)
				{
					NavalTooltipRefresherCollection.AddPartyTroopProperties(propertyBasedTooltipVM, troopRoster, GameTexts.FindText("str_map_tooltip_troops", null), flag, func5);
				}
			}
			else if (!settlement.IsHideout)
			{
				propertyBasedTooltipVM.AddProperty(string.Empty, string.Empty, -1, 0);
				if (flag)
				{
					List<MobileParty> list = new List<MobileParty>();
					Town town2 = settlement.Town;
					bool flag2 = town2 == null || !town2.InRebelliousState;
					for (int j = 0; j < settlement.Parties.Count; j++)
					{
						MobileParty mobileParty = settlement.Parties[j];
						bool flag3 = flag2 && mobileParty.IsMilitia;
						if (DiplomacyHelper.IsSameFactionAndNotEliminated(settlementAsParty.MapFaction, mobileParty.MapFaction) && (mobileParty.IsLordParty || flag3 || mobileParty.IsGarrison))
						{
							list.Add(mobileParty);
						}
					}
					list.Sort(CampaignUIHelper.MobilePartyPrecedenceComparerInstance);
					List<MobileParty> list2 = settlement.Parties.Where<MobileParty>((MobileParty p) => !p.IsLordParty && !p.IsMilitia && !p.IsGarrison).ToList<MobileParty>();
					list2.Sort(CampaignUIHelper.MobilePartyPrecedenceComparerInstance);
					if (list.Count > 0)
					{
						int num3 = list.Sum<MobileParty>((MobileParty p) => p.Party.NumberOfHealthyMembers);
						int num4 = list.Sum<MobileParty>((MobileParty p) => p.Party.NumberOfWoundedTotalMembers);
						string text11 = num3 + ((num4 > 0) ? ("+" + num4 + GameTexts.FindText("str_party_nameplate_wounded_abbr", null).ToString()) : "");
						propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_map_tooltip_defenders", null).ToString(), text11, 0, 0);
						propertyBasedTooltipVM.AddProperty("", "", 0, 512);
						foreach (MobileParty mobileParty2 in list)
						{
							propertyBasedTooltipVM.AddProperty(mobileParty2.Name.ToString(), CampaignUIHelper.GetPartyNameplateText(mobileParty2, false), 0, 0);
						}
						propertyBasedTooltipVM.AddProperty(string.Empty, string.Empty, -1, 0);
					}
					if (list2.Count <= 0)
					{
						goto IL_0C73;
					}
					propertyBasedTooltipVM.AddProperty("", "", 0, 1024);
					using (List<MobileParty>.Enumerator enumerator = list2.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							MobileParty mobileParty3 = enumerator.Current;
							propertyBasedTooltipVM.AddProperty(mobileParty3.Name.ToString(), CampaignUIHelper.GetPartyNameplateText(mobileParty3, false), 0, 0);
						}
						goto IL_0C73;
					}
				}
				string text12 = GameTexts.FindText("str_missing_info_indicator", null).ToString();
				propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_map_tooltip_parties", null).ToString(), text12, 0, 0);
			}
			IL_0C73:
			if (!settlement.IsHideout && troopRoster2.Count > 0 && flag)
			{
				NavalTooltipRefresherCollection.AddPartyTroopProperties(propertyBasedTooltipVM, troopRoster2, GameTexts.FindText("str_map_tooltip_prisoners", null), flag, func6);
			}
			if (settlement.IsFortification && settlement.Town.InRebelliousState)
			{
				propertyBasedTooltipVM.AddProperty(string.Empty, GameTexts.FindText("str_settlement_rebellious_state", null).ToString(), -1, 0);
			}
			propertyBasedTooltipVM.AddProperty(string.Empty, string.Empty, -1, 0);
			if (!settlement.IsHideout && !propertyBasedTooltipVM.IsExtended && flag)
			{
				TextObject textObject7 = GameTexts.FindText("str_map_tooltip_info", null);
				textObject7.SetTextVariable("EXTEND_KEY", propertyBasedTooltipVM.GetKeyText(NavalTooltipRefresherCollection.ExtendKeyId));
				propertyBasedTooltipVM.AddProperty(string.Empty, textObject7.ToString(), -1, 0);
			}
			if (Campaign.Current.Models.EncounterModel.CanMainHeroDoParleyWithParty(settlementAsParty, ref textObject))
			{
				TextObject textObject8 = new TextObject("{=uEeLvYXT}Press '{MODIFIER_KEY}' + '{CLICK_KEY}' to parley.", null);
				textObject8.SetTextVariable("MODIFIER_KEY", propertyBasedTooltipVM.GetKeyText(NavalTooltipRefresherCollection.FollowModifierKeyId));
				textObject8.SetTextVariable("CLICK_KEY", propertyBasedTooltipVM.GetKeyText(NavalTooltipRefresherCollection.MapClickKeyId));
				propertyBasedTooltipVM.AddProperty(string.Empty, textObject8.ToString(), -1, 0);
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00003A20 File Offset: 0x00001C20
		private static void AddPartyTroopProperties(PropertyBasedTooltipVM propertyBasedTooltipVM, TroopRoster troopRoster, TextObject title, bool isInspected, Func<TroopRoster> funcToDoBeforeLambda = null)
		{
			propertyBasedTooltipVM.AddProperty(string.Empty, string.Empty, -1, 0);
			propertyBasedTooltipVM.AddProperty(title.ToString(), delegate
			{
				TroopRoster troopRoster2 = ((funcToDoBeforeLambda != null) ? funcToDoBeforeLambda() : troopRoster);
				int num2 = 0;
				int num3 = 0;
				for (int l = 0; l < troopRoster2.Count; l++)
				{
					TroopRosterElement elementCopyAtIndex3 = troopRoster2.GetElementCopyAtIndex(l);
					num2 += elementCopyAtIndex3.Number - elementCopyAtIndex3.WoundedNumber;
					num3 += elementCopyAtIndex3.WoundedNumber;
				}
				TextObject textObject3 = new TextObject("{=iXXTONWb} ({PARTY_SIZE})", null);
				textObject3.SetTextVariable("PARTY_SIZE", PartyBaseHelper.GetPartySizeText(num2, num3, isInspected));
				return textObject3.ToString();
			}, 0, 0);
			if (isInspected)
			{
				propertyBasedTooltipVM.AddProperty("", "", 0, 512);
			}
			if (isInspected)
			{
				Dictionary<FormationClass, Tuple<int, int>> dictionary = new Dictionary<FormationClass, Tuple<int, int>>();
				for (int i = 0; i < troopRoster.Count; i++)
				{
					TroopRosterElement elementCopyAtIndex = troopRoster.GetElementCopyAtIndex(i);
					if (dictionary.ContainsKey(elementCopyAtIndex.Character.DefaultFormationClass))
					{
						Tuple<int, int> tuple = dictionary[elementCopyAtIndex.Character.DefaultFormationClass];
						dictionary[elementCopyAtIndex.Character.DefaultFormationClass] = new Tuple<int, int>(tuple.Item1 + elementCopyAtIndex.Number - elementCopyAtIndex.WoundedNumber, tuple.Item2 + elementCopyAtIndex.WoundedNumber);
					}
					else
					{
						dictionary.Add(elementCopyAtIndex.Character.DefaultFormationClass, new Tuple<int, int>(elementCopyAtIndex.Number - elementCopyAtIndex.WoundedNumber, elementCopyAtIndex.WoundedNumber));
					}
				}
				foreach (KeyValuePair<FormationClass, Tuple<int, int>> keyValuePair in dictionary.OrderBy<KeyValuePair<FormationClass, Tuple<int, int>>, FormationClass>((KeyValuePair<FormationClass, Tuple<int, int>> x) => x.Key))
				{
					TextObject textObject = new TextObject("{=Dqydb21E} {PARTY_SIZE}", null);
					textObject.SetTextVariable("PARTY_SIZE", PartyBaseHelper.GetPartySizeText(keyValuePair.Value.Item1, keyValuePair.Value.Item2, true));
					TextObject textObject2 = GameTexts.FindText("str_troop_type_name", FormationClassExtensions.GetName(keyValuePair.Key));
					propertyBasedTooltipVM.AddProperty(textObject2.ToString(), textObject.ToString(), 0, 0);
				}
			}
			if (propertyBasedTooltipVM.IsExtended & isInspected)
			{
				propertyBasedTooltipVM.AddProperty(string.Empty, string.Empty, -1, 0);
				propertyBasedTooltipVM.AddProperty(GameTexts.FindText("str_troop_types", null).ToString(), " ", 0, 0);
				propertyBasedTooltipVM.AddProperty("", "", 0, 1024);
				for (int j = 0; j < troopRoster.Count; j++)
				{
					TroopRosterElement elementCopyAtIndex2 = troopRoster.GetElementCopyAtIndex(j);
					if (elementCopyAtIndex2.Character.IsHero)
					{
						CharacterObject hero = elementCopyAtIndex2.Character;
						propertyBasedTooltipVM.AddProperty(elementCopyAtIndex2.Character.Name.ToString(), delegate
						{
							TroopRoster troopRoster3 = ((funcToDoBeforeLambda != null) ? funcToDoBeforeLambda() : troopRoster);
							int num4 = troopRoster3.FindIndexOfTroop(hero);
							if (num4 == -1)
							{
								return string.Empty;
							}
							TroopRosterElement elementCopyAtIndex4 = troopRoster3.GetElementCopyAtIndex(num4);
							TextObject textObject4 = GameTexts.FindText("str_NUMBER_percent", null);
							textObject4.SetTextVariable("NUMBER", elementCopyAtIndex4.Character.HeroObject.HitPoints * 100 / elementCopyAtIndex4.Character.MaxHitPoints());
							return textObject4.ToString();
						}, 0, 0);
					}
				}
				for (int k = 0; k < troopRoster.Count; k++)
				{
					int num = k;
					CharacterObject character = troopRoster.GetElementCopyAtIndex(num).Character;
					if (!character.IsHero)
					{
						propertyBasedTooltipVM.AddProperty(character.Name.ToString(), delegate
						{
							TroopRoster troopRoster4 = ((funcToDoBeforeLambda != null) ? funcToDoBeforeLambda() : troopRoster);
							int num5 = troopRoster4.FindIndexOfTroop(character);
							if (num5 != -1)
							{
								if (num5 > troopRoster4.Count)
								{
									return string.Empty;
								}
								TroopRosterElement elementCopyAtIndex5 = troopRoster4.GetElementCopyAtIndex(num5);
								if (elementCopyAtIndex5.Character == null)
								{
									return string.Empty;
								}
								CharacterObject character2 = elementCopyAtIndex5.Character;
								if (character2 != null && !character2.IsHero)
								{
									TextObject textObject5 = new TextObject("{=!}{PARTY_SIZE}", null);
									textObject5.SetTextVariable("PARTY_SIZE", PartyBaseHelper.GetPartySizeText(elementCopyAtIndex5.Number - elementCopyAtIndex5.WoundedNumber, elementCopyAtIndex5.WoundedNumber, true));
									return textObject5.ToString();
								}
							}
							return string.Empty;
						}, 0, 0);
					}
				}
			}
		}

		// Token: 0x04000001 RID: 1
		private static string ExtendKeyId = "ExtendModifier";

		// Token: 0x04000002 RID: 2
		private static string FollowModifierKeyId = "FollowModifier";

		// Token: 0x04000003 RID: 3
		private static string MapClickKeyId = "MapClick";
	}
}
