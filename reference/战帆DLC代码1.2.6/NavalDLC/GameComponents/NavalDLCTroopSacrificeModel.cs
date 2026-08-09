using System;
using System.Collections.Generic;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.GameComponents
{
	// Token: 0x0200013A RID: 314
	public class NavalDLCTroopSacrificeModel : TroopSacrificeModel
	{
		// Token: 0x17000390 RID: 912
		// (get) Token: 0x06001530 RID: 5424 RVA: 0x00095067 File Offset: 0x00093267
		public override int BreakOutArmyLeaderRelationPenalty
		{
			get
			{
				return base.BaseModel.BreakOutArmyLeaderRelationPenalty;
			}
		}

		// Token: 0x17000391 RID: 913
		// (get) Token: 0x06001531 RID: 5425 RVA: 0x00095074 File Offset: 0x00093274
		public override int BreakOutArmyMemberRelationPenalty
		{
			get
			{
				return base.BaseModel.BreakOutArmyMemberRelationPenalty;
			}
		}

		// Token: 0x06001532 RID: 5426 RVA: 0x00095084 File Offset: 0x00093284
		public override ExplainedNumber GetLostTroopCountForBreakingInBesiegedSettlement(MobileParty party, SiegeEvent siegeEvent)
		{
			ExplainedNumber lostTroopCountForBreakingInBesiegedSettlement = base.BaseModel.GetLostTroopCountForBreakingInBesiegedSettlement(party, siegeEvent);
			if (party.IsCurrentlyAtSea && party.HasPerk(NavalPerks.Shipmaster.GhostShip, false))
			{
				lostTroopCountForBreakingInBesiegedSettlement.AddFactor(NavalPerks.Shipmaster.GhostShip.PrimaryBonus * -1f, NavalPerks.Shipmaster.GhostShip.Name);
			}
			return lostTroopCountForBreakingInBesiegedSettlement;
		}

		// Token: 0x06001533 RID: 5427 RVA: 0x000950D8 File Offset: 0x000932D8
		public override ExplainedNumber GetLostTroopCountForBreakingOutOfBesiegedSettlement(MobileParty party, SiegeEvent siegeEvent, bool isBreakingOutFromPort)
		{
			ExplainedNumber lostTroopCountForBreakingOutOfBesiegedSettlement = base.BaseModel.GetLostTroopCountForBreakingOutOfBesiegedSettlement(party, siegeEvent, isBreakingOutFromPort);
			if (isBreakingOutFromPort && party.HasPerk(NavalPerks.Shipmaster.GhostShip, false))
			{
				lostTroopCountForBreakingOutOfBesiegedSettlement.AddFactor(NavalPerks.Shipmaster.GhostShip.PrimaryBonus * -1f, NavalPerks.Shipmaster.GhostShip.Name);
			}
			return lostTroopCountForBreakingOutOfBesiegedSettlement;
		}

		// Token: 0x06001534 RID: 5428 RVA: 0x00095127 File Offset: 0x00093327
		public override int GetNumberOfTroopsSacrificedForTryingToGetAway(BattleSideEnum battleSide, MapEvent mapEvent)
		{
			return base.BaseModel.GetNumberOfTroopsSacrificedForTryingToGetAway(battleSide, mapEvent);
		}

		// Token: 0x06001535 RID: 5429 RVA: 0x00095138 File Offset: 0x00093338
		private static bool CanPlayerSideTryToGetAwayWithTheirShipStats(out float totalDamageToApply)
		{
			totalDamageToApply = 0f;
			BattleSideEnum playerSide = PlayerEncounter.Current.PlayerSide;
			MapEvent battle = PlayerEncounter.Battle;
			float num = 0f;
			foreach (MapEventParty mapEventParty in battle.PartiesOnSide(playerSide))
			{
				foreach (Ship ship in mapEventParty.Ships)
				{
					num += ship.HitPoints;
				}
			}
			float num2 = 0f;
			foreach (MapEventParty mapEventParty2 in battle.PartiesOnSide(Extensions.GetOppositeSide(playerSide)))
			{
				foreach (Ship ship2 in mapEventParty2.Ships)
				{
					num2 += ship2.HitPoints;
				}
			}
			float num3 = num2 / num;
			totalDamageToApply = num * MathF.Pow(MathF.Min(num3, 3f), 1.3f) * 0.1f;
			if (totalDamageToApply > 0f)
			{
				ExplainedNumber explainedNumber;
				explainedNumber..ctor(totalDamageToApply, false, null);
				SkillHelper.AddSkillBonusForParty(NavalSkillEffects.ShipDamageReduction, MobileParty.MainParty, ref explainedNumber);
				float num4 = explainedNumber.ResultNumber;
				if (MobileParty.MainParty.HasPerk(NavalPerks.Shipmaster.GhostShip, false))
				{
					num4 -= num4 * 0.5f;
				}
				ExplainedNumber explainedNumber2 = Campaign.Current.Models.PartySpeedCalculatingModel.CalculateBaseSpeed(MobileParty.MainParty, false, 0, 0);
				PartyBase leaderParty = battle.GetLeaderParty(Extensions.GetOppositeSide(playerSide));
				ExplainedNumber explainedNumber3 = Campaign.Current.Models.PartySpeedCalculatingModel.CalculateBaseSpeed(leaderParty.MobileParty, false, 0, 0);
				if (explainedNumber2.ResultNumber > explainedNumber3.ResultNumber)
				{
					float num5 = MBMath.ClampFloat(explainedNumber2.ResultNumber / explainedNumber3.ResultNumber, 1f, 5f) * 0.1f;
					num4 -= num4 * num5;
				}
				totalDamageToApply = num4;
			}
			return totalDamageToApply < num;
		}

		// Token: 0x06001536 RID: 5430 RVA: 0x00095380 File Offset: 0x00093580
		public override void GetShipsToSacrificeForTryingToGetAway(BattleSideEnum playerBattleSide, MapEvent mapEvent, out MBList<Ship> shipsToCapture, out Ship shipToTakeDamage, out float damageToApplyForLastShip)
		{
			damageToApplyForLastShip = float.MinValue;
			shipsToCapture = new MBList<Ship>();
			shipToTakeDamage = null;
			List<MapEventParty> list = mapEvent.PartiesOnSide(playerBattleSide);
			mapEvent.RecalculateStrengthOfSides();
			List<Ship> list2 = new List<Ship>();
			foreach (MapEventParty mapEventParty in list)
			{
				foreach (Ship ship in mapEventParty.Ships)
				{
					list2.Add(ship);
				}
			}
			float num;
			if (!NavalDLCTroopSacrificeModel.CanPlayerSideTryToGetAwayWithTheirShipStats(out num))
			{
				Debug.FailedAssert("This can't be possible anymore (Should already handled in previous menu)", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\GameComponents\\NavalDLCTroopSacrificeModel.cs", "GetShipsToSacrificeForTryingToGetAway", 174);
				return;
			}
			float maxHitPoints = Extensions.MaxBy<Ship, float>(list2, (Ship x) => x.MaxHitPoints).MaxHitPoints;
			if (num <= Extensions.MinBy<Ship, float>(list2, (Ship x) => x.HitPoints).HitPoints)
			{
				shipsToCapture.Add(Extensions.MinBy<Ship, float>(list2, (Ship x) => x.HitPoints));
				return;
			}
			while (num > 0f)
			{
				Ship shipToSacrifice = NavalDLCTroopSacrificeModel.GetShipToSacrifice(maxHitPoints, list2);
				if (num < shipToSacrifice.HitPoints)
				{
					shipToTakeDamage = shipToSacrifice;
					damageToApplyForLastShip = num;
					num = 0f;
					return;
				}
				shipsToCapture.Add(shipToSacrifice);
				num -= shipToSacrifice.HitPoints;
				list2.Remove(shipToSacrifice);
			}
		}

		// Token: 0x06001537 RID: 5431 RVA: 0x00095524 File Offset: 0x00093724
		private static Ship GetShipToSacrifice(float maxHitPointScore, List<Ship> shipsToSacrifice)
		{
			Dictionary<PartyBase, int> partyShipCounts = new Dictionary<PartyBase, int>();
			foreach (Ship ship in shipsToSacrifice)
			{
				int num;
				if (partyShipCounts.TryGetValue(ship.Owner, out num))
				{
					Dictionary<PartyBase, int> partyShipCounts2 = partyShipCounts;
					PartyBase owner = ship.Owner;
					partyShipCounts2[owner]++;
				}
				else
				{
					partyShipCounts.Add(ship.Owner, 1);
				}
			}
			int maxOwnedShipCount = Extensions.MaxBy<KeyValuePair<PartyBase, int>, int>(partyShipCounts, (KeyValuePair<PartyBase, int> x) => x.Value).Value;
			return Extensions.MinBy<Ship, float>(shipsToSacrifice, (Ship x) => NavalDLCTroopSacrificeModel.GetShipSacrificeScore(x, maxOwnedShipCount, partyShipCounts[x.Owner], maxHitPointScore));
		}

		// Token: 0x06001538 RID: 5432 RVA: 0x0009561C File Offset: 0x0009381C
		private static float GetShipSacrificeScore(Ship shipToConsider, int maxOwnedShipCount, int ownerCurrentShipCount, float maxHitPointScore)
		{
			float num = shipToConsider.HitPoints;
			num += (float)(maxOwnedShipCount - ownerCurrentShipCount) * maxHitPointScore;
			if (shipToConsider.Owner.MobileParty.LeaderHero.IsKingdomLeader)
			{
				num += 50000f;
			}
			else if (shipToConsider.Owner.MobileParty.LeaderHero.IsClanLeader)
			{
				num += 20000f;
			}
			return num;
		}

		// Token: 0x06001539 RID: 5433 RVA: 0x0009567C File Offset: 0x0009387C
		public override bool CanPlayerGetAwayFromEncounter(out TextObject explanation)
		{
			if (!base.BaseModel.CanPlayerGetAwayFromEncounter(ref explanation))
			{
				return false;
			}
			if (MobileParty.MainParty.IsCurrentlyAtSea)
			{
				int num = MobileParty.MainParty.Ships.Count;
				if (MobileParty.MainParty.Army != null && (MobileParty.MainParty.Army.LeaderParty == MobileParty.MainParty || MobileParty.MainParty.AttachedTo != null))
				{
					foreach (MobileParty mobileParty in MobileParty.MainParty.Army.LeaderParty.AttachedParties)
					{
						num += mobileParty.Ships.Count;
					}
				}
				float num2;
				if (num < 2 || !NavalDLCTroopSacrificeModel.CanPlayerSideTryToGetAwayWithTheirShipStats(out num2))
				{
					explanation = new TextObject("{=uafBbokT}You don't have enough room on your surviving ships to escape.", null);
					return false;
				}
			}
			return true;
		}

		// Token: 0x04000B0E RID: 2830
		private const int MinNumberOfShipsForSacrificeShips = 2;
	}
}
