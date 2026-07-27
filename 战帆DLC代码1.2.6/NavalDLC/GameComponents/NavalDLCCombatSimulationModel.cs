using System;
using System.Runtime.CompilerServices;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000114 RID: 276
	public class NavalDLCCombatSimulationModel : CombatSimulationModel
	{
		// Token: 0x060013C1 RID: 5057 RVA: 0x0008E64C File Offset: 0x0008C84C
		public override CampaignTime GetSimulationTickInterval(MapEvent mapEvent)
		{
			if (mapEvent.IsNavalMapEvent)
			{
				return CampaignTime.Minutes(60L);
			}
			return base.BaseModel.GetSimulationTickInterval(mapEvent);
		}

		// Token: 0x060013C2 RID: 5058 RVA: 0x0008E66C File Offset: 0x0008C86C
		public override void GetBattleAdvantage(MapEvent mapEvent, out ExplainedNumber defenderAdvantage, out ExplainedNumber attackerAdvantage)
		{
			base.BaseModel.GetBattleAdvantage(mapEvent, ref defenderAdvantage, ref attackerAdvantage);
			if (mapEvent.IsNavalMapEvent)
			{
				PartyBase leaderParty = mapEvent.GetLeaderParty(0);
				PartyBase leaderParty2 = mapEvent.GetLeaderParty(1);
				if (leaderParty.IsMobile)
				{
					SkillHelper.AddSkillBonusForParty(NavalSkillEffects.NavalAutoBattleSimulationAdvantage, leaderParty.MobileParty, ref defenderAdvantage);
					if (leaderParty2.IsMobile)
					{
						SkillHelper.AddSkillBonusForParty(NavalSkillEffects.NavalAutoBattleSimulationAdvantage, leaderParty.MobileParty, ref attackerAdvantage);
						if (leaderParty.MobileParty.IsBandit)
						{
							PerkHelper.AddPerkBonusForParty(NavalPerks.Mariner.PirateHunter, leaderParty2.MobileParty, true, ref attackerAdvantage, false);
						}
					}
				}
			}
		}

		// Token: 0x060013C3 RID: 5059 RVA: 0x0008E6F1 File Offset: 0x0008C8F1
		public override int GetPursuitRoundCount(MapEvent mapEvent)
		{
			return base.BaseModel.GetPursuitRoundCount(mapEvent);
		}

		// Token: 0x060013C4 RID: 5060 RVA: 0x0008E6FF File Offset: 0x0008C8FF
		public override float GetMaximumSiegeEquipmentProgress(Settlement settlement)
		{
			return base.BaseModel.GetMaximumSiegeEquipmentProgress(settlement);
		}

		// Token: 0x060013C5 RID: 5061 RVA: 0x0008E70D File Offset: 0x0008C90D
		public override int GetNumberOfEquipmentsBuilt(Settlement settlement)
		{
			return base.BaseModel.GetNumberOfEquipmentsBuilt(settlement);
		}

		// Token: 0x060013C6 RID: 5062 RVA: 0x0008E71B File Offset: 0x0008C91B
		public override float GetSettlementAdvantage(Settlement settlement)
		{
			return base.BaseModel.GetSettlementAdvantage(settlement);
		}

		// Token: 0x060013C7 RID: 5063 RVA: 0x0008E72C File Offset: 0x0008C92C
		public override float GetShipSiegeEngineHitChance(Ship ship, SiegeEngineType siegeEngineType, BattleSideEnum battleSide)
		{
			ExplainedNumber explainedNumber;
			explainedNumber..ctor(0.3f, false, null);
			ShipHull.ShipType type = ship.ShipHull.Type;
			if (!siegeEngineType.IsRanged)
			{
				if (battleSide == 1)
				{
					if (type == null)
					{
						explainedNumber.Add(0.05f, null, null);
					}
					else if (type == 2)
					{
						explainedNumber.Add(-0.05f, null, null);
					}
				}
				else if (type == null)
				{
					explainedNumber.Add(-0.05f, null, null);
				}
				else if (type == 2)
				{
					explainedNumber.Add(0.05f, null, null);
				}
			}
			else if (battleSide == null)
			{
				if (type == null)
				{
					explainedNumber.Add(-0.1f, null, null);
				}
				else if (type == 2)
				{
					explainedNumber.Add(0.1f, null, null);
				}
			}
			return explainedNumber.ResultNumber;
		}

		// Token: 0x060013C8 RID: 5064 RVA: 0x0008E7DC File Offset: 0x0008C9DC
		[return: TupleElementNames(new string[] { "defenderRounds", "attackerRounds" })]
		public override ValueTuple<int, int> GetSimulationTicksForBattleRound(MapEvent mapEvent)
		{
			if (mapEvent.IsNavalMapEvent)
			{
				MapEvent.BattleTypes eventType = mapEvent.EventType;
				Settlement mapEventSettlement = mapEvent.MapEventSettlement;
				int num = 0;
				int num2 = 0;
				if (!mapEvent.IsInvulnerable)
				{
					int totalCrewCapacity = this.GetTotalCrewCapacity(mapEvent.DefenderSide);
					int totalCrewCapacity2 = this.GetTotalCrewCapacity(mapEvent.AttackerSide);
					int num3 = Math.Min(mapEvent.DefenderSide.NumRemainingSimulationTroops, totalCrewCapacity);
					int num4 = Math.Min(mapEvent.AttackerSide.NumRemainingSimulationTroops, totalCrewCapacity2);
					if (eventType == 5 && ((mapEventSettlement.IsTown && num3 > 100) || (mapEventSettlement.IsCastle && num3 > 30)))
					{
						float num5 = this.GetSettlementAdvantage(mapEventSettlement) * 0.7f;
						num2 = MathF.Round(1.5f + MathF.Pow((float)num3, 0.3f)) * 2;
						num = MathF.Round(0.5f + MathF.Max(1f + MathF.Pow((float)num3, 0.3f) * num5, (float)((num3 + 1) / (num4 + 1)))) * 2;
					}
					else if (num3 <= 10)
					{
						num = Math.Max(MathF.Round(MathF.Min((float)num4 * 3f, (float)num3 * 0.3f)), 1);
						num2 = Math.Max(MathF.Round(MathF.Min((float)num3 * 3f, (float)num4 * 0.3f)), 1);
					}
					else
					{
						num = MathF.Round(MathF.Min((float)num4 * 2f, MathF.Pow((float)num3, 0.6f)));
						num2 = MathF.Round(MathF.Min((float)num3 * 2f, MathF.Pow((float)num4, 0.6f)));
					}
					if (mapEvent.RetreatingSide != -1)
					{
						if (mapEvent.RetreatingSide == 1)
						{
							num2 = 0;
						}
						else
						{
							num = 0;
						}
					}
				}
				return new ValueTuple<int, int>(num, num2);
			}
			if (mapEvent.IsRaid)
			{
				MobileParty mobileParty = mapEvent.AttackerSide.LeaderParty.MobileParty;
				if (mobileParty != null && mobileParty.IsCurrentlyAtSea)
				{
					int totalCrewCapacity3 = this.GetTotalCrewCapacity(mapEvent.AttackerSide);
					int num6 = Math.Min(mapEvent.AttackerSide.NumRemainingSimulationTroops, totalCrewCapacity3);
					int numRemainingSimulationTroops = mapEvent.DefenderSide.NumRemainingSimulationTroops;
					int num7 = Math.Max(MathF.Round(MathF.Min((float)num6 * 3f, (float)numRemainingSimulationTroops * 0.3f)), 1);
					int num8 = Math.Max(MathF.Round(MathF.Min((float)numRemainingSimulationTroops * 3f, (float)num6 * 0.3f)), 1);
					if (mapEvent.RetreatingSide != -1)
					{
						if (mapEvent.RetreatingSide == 1)
						{
							num8 = 0;
						}
						else
						{
							Debug.FailedAssert("Defender cant retreat in raid", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\GameComponents\\NavalDLCCombatSimulationModel.cs", "GetSimulationTicksForBattleRound", 205);
							num7 = 0;
						}
					}
					return new ValueTuple<int, int>(num7, num8);
				}
			}
			return base.BaseModel.GetSimulationTicksForBattleRound(mapEvent);
		}

		// Token: 0x060013C9 RID: 5065 RVA: 0x0008EA7C File Offset: 0x0008CC7C
		public override ExplainedNumber SimulateHit(CharacterObject strikerTroop, CharacterObject struckTroop, PartyBase strikerParty, PartyBase struckParty, float strikerAdvantage, MapEvent battle, float strikerSideMorale, float struckSideMorale)
		{
			ExplainedNumber explainedNumber = base.BaseModel.SimulateHit(strikerTroop, struckTroop, strikerParty, struckParty, strikerAdvantage, battle, strikerSideMorale, struckSideMorale);
			if (battle.IsNavalMapEvent)
			{
				float weightedShipCombatFactor = battle.GetMapEventSide(strikerParty.Side).WeightedShipCombatFactor;
				explainedNumber.AddFactor(weightedShipCombatFactor, null);
			}
			return explainedNumber;
		}

		// Token: 0x060013CA RID: 5066 RVA: 0x0008EAC8 File Offset: 0x0008CCC8
		public override ExplainedNumber SimulateHit(Ship strikerShip, Ship struckShip, PartyBase strikerParty, PartyBase struckParty, SiegeEngineType siegeEngine, float strikerAdvantage, MapEvent battle, out int troopCasualties)
		{
			troopCasualties = 0;
			ExplainedNumber explainedNumber;
			if (siegeEngine.IsRanged)
			{
				explainedNumber..ctor((float)siegeEngine.Damage, false, null);
				troopCasualties = 1;
			}
			else
			{
				int num = 1;
				switch (strikerShip.ShipHull.Type)
				{
				case 0:
					num = 1;
					break;
				case 1:
					num = 2;
					break;
				case 2:
					num = 3;
					break;
				default:
					Debug.FailedAssert("Unhandled ship type", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\GameComponents\\NavalDLCCombatSimulationModel.cs", "SimulateHit", 257);
					break;
				}
				explainedNumber..ctor((float)(siegeEngine.Damage * num), false, null);
				if (struckParty.IsMobile)
				{
					PerkHelper.AddPerkBonusForParty(NavalPerks.Shipmaster.SeaborneFortress, struckParty.MobileParty, true, ref explainedNumber, false);
				}
			}
			if (strikerParty.IsMobile && !strikerParty.MobileParty.IsCurrentlyAtSea && strikerParty.MobileParty.HasPerk(DefaultPerks.Crossbow.Terror, false) && RandomOwnerExtensions.RandomFloatWithSeed(strikerParty, (uint)battle.UpdateCount) < DefaultPerks.Crossbow.Terror.PrimaryBonus)
			{
				troopCasualties++;
			}
			return explainedNumber;
		}

		// Token: 0x060013CB RID: 5067 RVA: 0x0008EBBC File Offset: 0x0008CDBC
		private int GetTotalCrewCapacity(MapEventSide side)
		{
			int num = 0;
			for (int i = 0; i < side.SimulationShipList.Count; i++)
			{
				Ship ship = side.SimulationShipList[i];
				num += ship.MainDeckCrewCapacity;
			}
			return num;
		}

		// Token: 0x060013CC RID: 5068 RVA: 0x0008EBF8 File Offset: 0x0008CDF8
		public override float GetBluntDamageChance(CharacterObject strikerTroop, CharacterObject strikedTroop, PartyBase strikerParty, PartyBase strikedParty, MapEvent battle)
		{
			return base.BaseModel.GetBluntDamageChance(strikerTroop, strikedTroop, strikerParty, strikedParty, battle);
		}

		// Token: 0x060013CD RID: 5069 RVA: 0x0008EC0C File Offset: 0x0008CE0C
		public override MBList<ValueTuple<Ship, MapEventParty>> GetSimulationShips(MapEvent mapEvent, MBList<MapEventParty> battleParties)
		{
			MBList<ValueTuple<Ship, MapEventParty>> mblist = new MBList<ValueTuple<Ship, MapEventParty>>();
			bool flag = mapEvent.SimulationContext == 12;
			if (mapEvent.IsNavalMapEvent || flag)
			{
				foreach (MapEventParty mapEventParty in battleParties)
				{
					foreach (Ship ship in mapEventParty.Ships)
					{
						if (!flag || ship.ShipHull.CanNavigateShallowWater)
						{
							mblist.Add(new ValueTuple<Ship, MapEventParty>(ship, mapEventParty));
						}
					}
				}
			}
			return mblist;
		}

		// Token: 0x060013CE RID: 5070 RVA: 0x0008ECCC File Offset: 0x0008CECC
		public override int GetParticipatingTroopCount(MapEventSide side)
		{
			int participatingTroopCount = base.BaseModel.GetParticipatingTroopCount(side);
			if (MapEventHelper.IsNavalRaid(side.MapEvent) && side.MissionSide == 1 && side.MapEvent.SimulationContext == 12)
			{
				return Math.Min(this.GetShallowShipDeckCrewCapacity(side), participatingTroopCount);
			}
			return participatingTroopCount;
		}

		// Token: 0x060013CF RID: 5071 RVA: 0x0008ED1C File Offset: 0x0008CF1C
		private int GetShallowShipDeckCrewCapacity(MapEventSide side)
		{
			int num = 0;
			foreach (MapEventParty mapEventParty in side.Parties)
			{
				foreach (Ship ship in mapEventParty.Ships)
				{
					if (ship.ShipHull.CanNavigateShallowWater)
					{
						num += ship.MainDeckCrewCapacity;
					}
				}
			}
			return num;
		}
	}
}
