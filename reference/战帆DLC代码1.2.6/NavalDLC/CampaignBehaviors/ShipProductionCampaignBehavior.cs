using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace NavalDLC.CampaignBehaviors
{
	// Token: 0x02000176 RID: 374
	public class ShipProductionCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600188B RID: 6283 RVA: 0x000A9EF4 File Offset: 0x000A80F4
		public override void RegisterEvents()
		{
			CampaignEvents.OnNewGameCreatedPartialFollowUpEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter, int>(this.OnNewGameCreatedPartialFollowUp));
			CampaignEvents.DailyTickTownEvent.AddNonSerializedListener(this, new Action<Town>(this.DailyTickTown));
			CampaignEvents.OnShipCreatedEvent.AddNonSerializedListener(this, new Action<Ship, Settlement>(this.OnShipCreated));
			CampaignEvents.OnShipDestroyedEvent.AddNonSerializedListener(this, new Action<PartyBase, Ship, DestroyShipAction.ShipDestroyDetail>(this.OnShipDestroyed));
			CampaignEvents.OnShipOwnerChangedEvent.AddNonSerializedListener(this, new Action<Ship, PartyBase, ChangeShipOwnerAction.ShipOwnerChangeDetail>(this.OnShipOwnerChanged));
			CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(this.Tick));
		}

		// Token: 0x0600188C RID: 6284 RVA: 0x000A9F8C File Offset: 0x000A818C
		private void Tick(float obj)
		{
			if (ShipProductionCampaignBehavior.DebugShipyards)
			{
				foreach (Town town in Town.AllTowns)
				{
					if (town.Settlement.HasPort)
					{
						Vec3 vec = town.Settlement.Position.AsVec3() + Vec3.Up * 3.75f;
						vec.x -= 1f;
						Building shipyard = town.GetShipyard();
						string text = string.Format("Ship Count: {0}\nShipyard level: {1}", town.AvailableShips.Count, shipyard.CurrentLevel);
						foreach (Ship ship in town.AvailableShips)
						{
							text = text + "\n" + ship.ShipHull.Name.ToString();
							vec = new Vec3(vec.x, vec.y + 0.25f, vec.z, -1f);
						}
					}
				}
				List<MobileParty> list = Extensions.ToMBList<MobileParty>(MobileParty.AllLordParties.OrderByDescending<MobileParty, int>((MobileParty x) => x.Ships.Count).Take<MobileParty>(5));
				int num = 140;
				foreach (MobileParty mobileParty in list)
				{
					num += 20;
				}
			}
		}

		// Token: 0x0600188D RID: 6285 RVA: 0x000AA178 File Offset: 0x000A8378
		private void DailyTickTown(Town town)
		{
			if (town.IsTown && !town.IsUnderSiege && town.Settlement.Party.MapEvent == null && town.Settlement.HasPort)
			{
				ExplainedNumber explainedNumber;
				explainedNumber..ctor(0.5f, false, null);
				PerkHelper.AddPerkBonusForTown(NavalPerks.Boatswain.StreamlinedOperations, town, ref explainedNumber);
				int maxShipCountForTown = ShipProductionCampaignBehavior.GetMaxShipCountForTown(town);
				if (town.AvailableShips.Count < maxShipCountForTown && MBRandom.RandomFloat < explainedNumber.ResultNumber)
				{
					int num = 0;
					while (num < 10 && town.AvailableShips.Count < maxShipCountForTown)
					{
						this.CreateShip(town);
						num++;
					}
				}
				int idealShipCountForTown = ShipProductionCampaignBehavior.GetIdealShipCountForTown(town);
				if (town.AvailableShips.Count >= idealShipCountForTown)
				{
					ShipProductionCampaignBehavior.TryRemoveExcessShipsFromTown(town);
				}
			}
		}

		// Token: 0x0600188E RID: 6286 RVA: 0x000AA238 File Offset: 0x000A8438
		private static void TryRemoveExcessShipsFromTown(Town town)
		{
			int idealShipCountForTown = ShipProductionCampaignBehavior.GetIdealShipCountForTown(town);
			int num = town.AvailableShips.Count - idealShipCountForTown;
			if (num <= 0)
			{
				return;
			}
			List<Ship> shipsOfOtherCulture = town.AvailableShips.Where<Ship>((Ship x) => !town.Culture.AvailableShipHulls.Contains(x.ShipHull)).ToList<Ship>();
			foreach (Ship ship in shipsOfOtherCulture)
			{
				if (MBRandom.RandomFloat < 0.7f)
				{
					DestroyShipAction.Apply(ship);
					num--;
					if (num < 0)
					{
						break;
					}
				}
			}
			if (num > 0)
			{
				foreach (Ship ship2 in town.AvailableShips.Where<Ship>((Ship x) => !shipsOfOtherCulture.Contains(x)).ToList<Ship>())
				{
					if (MBRandom.RandomFloat < 0.3f)
					{
						DestroyShipAction.Apply(ship2);
						num--;
						if (num < 0)
						{
							break;
						}
					}
				}
			}
		}

		// Token: 0x0600188F RID: 6287 RVA: 0x000AA374 File Offset: 0x000A8574
		private void CreateShip(Town town)
		{
			ShipHull randomShipHull = this.GetRandomShipHull(town);
			if (randomShipHull != null)
			{
				Ship ship = new Ship(randomShipHull);
				List<ShipUpgradePiece> availableShipUpgradePieces = town.GetAvailableShipUpgradePieces();
				Extensions.Shuffle<ShipUpgradePiece>(availableShipUpgradePieces);
				foreach (KeyValuePair<string, ShipSlot> keyValuePair in ship.ShipHull.AvailableSlots)
				{
					if (MBRandom.RandomFloat > 0.5f)
					{
						int num = MBRandom.RandomInt(availableShipUpgradePieces.Count);
						for (int i = 0; i < availableShipUpgradePieces.Count; i++)
						{
							ShipUpgradePiece shipUpgradePiece = availableShipUpgradePieces[(i + num) % availableShipUpgradePieces.Count];
							if (shipUpgradePiece.DoesPieceMatchSlot(keyValuePair.Value))
							{
								ship.EquipUpgradePiece(keyValuePair.Key, shipUpgradePiece);
								break;
							}
						}
					}
				}
				ChangeShipOwnerAction.ApplyByProduction(town.Settlement.Party, ship);
				CampaignEventDispatcher.Instance.OnShipCreated(ship, town.Settlement);
			}
		}

		// Token: 0x06001890 RID: 6288 RVA: 0x000AA470 File Offset: 0x000A8670
		private void OnShipOwnerChanged(Ship ship, PartyBase oldOwner, ChangeShipOwnerAction.ShipOwnerChangeDetail changeDetail)
		{
			if (ship.Owner.IsSettlement)
			{
				RepairShipAction.ApplyForFree(ship);
			}
		}

		// Token: 0x06001891 RID: 6289 RVA: 0x000AA488 File Offset: 0x000A8688
		private void OnNewGameCreatedPartialFollowUp(CampaignGameStarter starter, int index)
		{
			foreach (Town town in Town.AllTowns)
			{
				this.DailyTickTown(town);
			}
		}

		// Token: 0x06001892 RID: 6290 RVA: 0x000AA4DC File Offset: 0x000A86DC
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06001893 RID: 6291 RVA: 0x000AA4E0 File Offset: 0x000A86E0
		private void OnShipCreated(Ship ship, Settlement settlement)
		{
			if (settlement.IsFortification && settlement.Town.Governor != null && settlement.Town.Governor.GetPerkValue(NavalPerks.Boatswain.MerchantFleet))
			{
				float secondaryBonus = NavalPerks.Boatswain.MerchantFleet.SecondaryBonus;
				if (secondaryBonus > 0f)
				{
					GainKingdomInfluenceAction.ApplyForDefault(settlement.Owner, secondaryBonus);
				}
			}
		}

		// Token: 0x06001894 RID: 6292 RVA: 0x000AA538 File Offset: 0x000A8738
		private void OnShipDestroyed(PartyBase party, Ship ship, DestroyShipAction.ShipDestroyDetail detail)
		{
			if (detail == 1 && party.IsMobile && party.MobileParty.HasPerk(NavalPerks.Boatswain.Salvage, false))
			{
				float num = ship.HitPoints * 0.01f;
				if (num > 0f)
				{
					GainKingdomInfluenceAction.ApplyForDefault(party.LeaderHero, num);
				}
			}
		}

		// Token: 0x06001895 RID: 6293 RVA: 0x000AA585 File Offset: 0x000A8785
		private ShipHull GetRandomShipHull(Town town)
		{
			MBList<ValueTuple<ShipHull, float>> availableShipHullsForTown = this.GetAvailableShipHullsForTown(town);
			if (availableShipHullsForTown.Count == 0)
			{
				Debug.FailedAssert("Could not find ships to create.", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\CampaignBehaviors\\ShipProductionCampaignBehavior.cs", "GetRandomShipHull", 231);
			}
			return MBRandom.ChooseWeighted<ShipHull>(availableShipHullsForTown);
		}

		// Token: 0x06001896 RID: 6294 RVA: 0x000AA5B4 File Offset: 0x000A87B4
		private MBList<ValueTuple<ShipHull, float>> GetAvailableShipHullsForTown(Town town)
		{
			MBList<ValueTuple<ShipHull, float>> mblist = new MBList<ValueTuple<ShipHull, float>>();
			foreach (ShipHull shipHull in town.Culture.AvailableShipHulls)
			{
				if (this.CanTownCreateShipFromHull(town, shipHull))
				{
					mblist.Add(new ValueTuple<ShipHull, float>(shipHull, shipHull.ProductionBuildWeight));
				}
			}
			return mblist;
		}

		// Token: 0x06001897 RID: 6295 RVA: 0x000AA628 File Offset: 0x000A8828
		private bool CanTownCreateShipFromHull(Town town, ShipHull shipHull)
		{
			switch (shipHull.Type)
			{
			case 0:
				return town.GetShipyard().CurrentLevel > 0;
			case 1:
				return town.GetShipyard().CurrentLevel > 1;
			case 2:
				return town.GetShipyard().CurrentLevel == 3;
			default:
				return false;
			}
		}

		// Token: 0x06001898 RID: 6296 RVA: 0x000AA680 File Offset: 0x000A8880
		private static int GetMaxShipCountForTown(Town town)
		{
			ExplainedNumber explainedNumber = default(ExplainedNumber);
			town.AddEffectOfBuildings(29, ref explainedNumber);
			return (int)explainedNumber.ResultNumber;
		}

		// Token: 0x06001899 RID: 6297 RVA: 0x000AA6A7 File Offset: 0x000A88A7
		private static int GetIdealShipCountForTown(Town town)
		{
			return MathF.Max(ShipProductionCampaignBehavior.GetMaxShipCountForTown(town) - 2, 0);
		}

		// Token: 0x04000C05 RID: 3077
		private const float ShipGenerationChance = 0.5f;

		// Token: 0x04000C06 RID: 3078
		private const float ShipGenerationUpgradePieceAddingChance = 0.5f;

		// Token: 0x04000C07 RID: 3079
		private const int ShipGenerationDailyCount = 10;

		// Token: 0x04000C08 RID: 3080
		public static bool DebugShipyards;
	}
}
