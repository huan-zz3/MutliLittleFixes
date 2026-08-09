using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace NavalDLC.CampaignBehaviors
{
	// Token: 0x02000177 RID: 375
	public class ShipRepairCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600189C RID: 6300 RVA: 0x000AA6C4 File Offset: 0x000A88C4
		public override void RegisterEvents()
		{
			CampaignEvents.OnAfterSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.AfterSessionLaunched));
			CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.SettlementOwnerChanged));
			CampaignEvents.AfterSettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnAfterSettlementEnter));
			CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.DailyTickParty));
			CampaignEvents.OnClanChangedKingdomEvent.AddNonSerializedListener(this, new Action<Clan, Kingdom, Kingdom, ChangeKingdomAction.ChangeKingdomActionDetail, bool>(this.OnClanChangedKingdom));
			CampaignEvents.OnShipDestroyedEvent.AddNonSerializedListener(this, new Action<PartyBase, Ship, DestroyShipAction.ShipDestroyDetail>(this.OnShipDestroyed));
		}

		// Token: 0x0600189D RID: 6301 RVA: 0x000AA75C File Offset: 0x000A895C
		private void OnShipDestroyed(PartyBase party, Ship ship, DestroyShipAction.ShipDestroyDetail detail)
		{
			if (detail == 1 && party.IsMobile && party.MobileParty.HasPerk(NavalPerks.Boatswain.ShipwrightsHand, false))
			{
				float num = ship.HitPoints * 0.5f;
				foreach (Ship ship2 in party.Ships)
				{
					if (num <= 0f)
					{
						break;
					}
					float num2 = ship2.MaxHitPoints - ship2.HitPoints;
					float num3 = MathF.Min(num, num2);
					ship2.HitPoints += num3;
					num -= num3;
				}
			}
		}

		// Token: 0x0600189E RID: 6302 RVA: 0x000AA810 File Offset: 0x000A8A10
		private void AfterSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this._factionsThatDoNotHavePort = new List<IFaction>();
			foreach (Clan clan in Clan.All)
			{
				if (!clan.IsBanditFaction && !this._factionsThatDoNotHavePort.Contains(clan.MapFaction))
				{
					bool flag = false;
					using (List<Settlement>.Enumerator enumerator2 = clan.MapFaction.Settlements.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (enumerator2.Current.HasPort)
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						this._factionsThatDoNotHavePort.Add(clan.MapFaction);
					}
				}
			}
		}

		// Token: 0x0600189F RID: 6303 RVA: 0x000AA8E4 File Offset: 0x000A8AE4
		private void SettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
		{
			if (settlement.HasPort)
			{
				if (this._factionsThatDoNotHavePort.Contains(newOwner.MapFaction))
				{
					this._factionsThatDoNotHavePort.Remove(newOwner.MapFaction);
				}
				bool flag = false;
				using (List<Settlement>.Enumerator enumerator = oldOwner.MapFaction.Settlements.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.HasPort)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					this._factionsThatDoNotHavePort.Add(oldOwner.MapFaction);
				}
			}
		}

		// Token: 0x060018A0 RID: 6304 RVA: 0x000AA984 File Offset: 0x000A8B84
		private void OnClanChangedKingdom(Clan clan, Kingdom oldKingdom, Kingdom newKingdom, ChangeKingdomAction.ChangeKingdomActionDetail detail, bool showNotification = true)
		{
			if (oldKingdom != null)
			{
				bool flag = false;
				using (List<Settlement>.Enumerator enumerator = oldKingdom.Settlements.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.HasPort)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					this._factionsThatDoNotHavePort.Remove(oldKingdom);
				}
			}
			else if (newKingdom != null)
			{
				this._factionsThatDoNotHavePort.Remove(clan);
			}
			if (newKingdom != null)
			{
				bool flag2 = false;
				using (List<Settlement>.Enumerator enumerator = newKingdom.Settlements.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.HasPort)
						{
							flag2 = true;
							break;
						}
					}
				}
				if (flag2)
				{
					this._factionsThatDoNotHavePort.Remove(newKingdom);
					return;
				}
			}
			else
			{
				bool flag3 = false;
				using (List<Settlement>.Enumerator enumerator = clan.Settlements.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.HasPort)
						{
							flag3 = true;
							break;
						}
					}
				}
				if (!flag3)
				{
					this._factionsThatDoNotHavePort.Add(clan);
				}
			}
		}

		// Token: 0x060018A1 RID: 6305 RVA: 0x000AAAB4 File Offset: 0x000A8CB4
		private void DailyTickParty(MobileParty mobileParty)
		{
			if ((mobileParty.IsBandit || this._factionsThatDoNotHavePort.Contains(mobileParty.MapFaction)) && !mobileParty.IsMainParty && mobileParty.Ships.Any<Ship>() && mobileParty.IsCurrentlyAtSea && !mobileParty.IsInRaftState && mobileParty.MapEvent == null && MBRandom.RandomFloat < 0.1f)
			{
				if (mobileParty.IsBandit)
				{
					this.RepairBanditPartyShips(mobileParty);
					return;
				}
				this.RepairPortlessFactionPartyShips(mobileParty);
			}
		}

		// Token: 0x060018A2 RID: 6306 RVA: 0x000AAB2C File Offset: 0x000A8D2C
		private void OnAfterSettlementEnter(MobileParty mobileParty, Settlement settlement, Hero hero)
		{
			if (mobileParty != null && !mobileParty.IsMainParty && settlement.HasPort && settlement.IsFortification)
			{
				if (mobileParty.IsCaravan)
				{
					this.RepairCaravanPartyShips(mobileParty);
					return;
				}
				Hero leaderHero = mobileParty.LeaderHero;
				if (leaderHero != null && leaderHero.IsMinorFactionHero)
				{
					this.RepairMinorFactionLordPartyShips(mobileParty);
					return;
				}
				if (mobileParty.IsLordParty)
				{
					this.RepairLordPartyShips(mobileParty, settlement);
				}
			}
		}

		// Token: 0x060018A3 RID: 6307 RVA: 0x000AAB90 File Offset: 0x000A8D90
		private void RepairPortlessFactionPartyShips(MobileParty mobileParty)
		{
			foreach (Ship ship in mobileParty.Ships)
			{
				if (ship.HitPoints < ship.MaxHitPoints)
				{
					RepairShipAction.ApplyForFree(ship);
				}
			}
		}

		// Token: 0x060018A4 RID: 6308 RVA: 0x000AABF0 File Offset: 0x000A8DF0
		private void RepairCaravanPartyShips(MobileParty mobileParty)
		{
			foreach (Ship ship in mobileParty.Ships)
			{
				if (ship.HitPoints < ship.MaxHitPoints && (float)mobileParty.PartyTradeGold > Campaign.Current.Models.ShipCostModel.GetShipRepairCost(ship, null))
				{
					RepairShipAction.ApplyForFree(ship);
				}
			}
		}

		// Token: 0x060018A5 RID: 6309 RVA: 0x000AAC70 File Offset: 0x000A8E70
		private void RepairBanditPartyShips(MobileParty mobileParty)
		{
			foreach (Ship ship in mobileParty.Ships)
			{
				if (ship.HitPoints < ship.MaxHitPoints)
				{
					RepairShipAction.ApplyForBanditShip(ship);
				}
			}
		}

		// Token: 0x060018A6 RID: 6310 RVA: 0x000AACD0 File Offset: 0x000A8ED0
		private void RepairMinorFactionLordPartyShips(MobileParty mobileParty)
		{
			foreach (Ship ship in mobileParty.Ships)
			{
				if (ship.HitPoints < ship.MaxHitPoints)
				{
					RepairShipAction.ApplyForFree(ship);
				}
			}
		}

		// Token: 0x060018A7 RID: 6311 RVA: 0x000AAD30 File Offset: 0x000A8F30
		private void RepairLordPartyShips(MobileParty mobileParty, Settlement settlement)
		{
			if (mobileParty.LeaderHero != null)
			{
				foreach (Ship ship in mobileParty.Ships)
				{
					if (ship.HitPoints < ship.MaxHitPoints && (float)mobileParty.PartyTradeGold > Campaign.Current.Models.ShipCostModel.GetShipRepairCost(ship, mobileParty.Party))
					{
						RepairShipAction.Apply(ship, settlement);
					}
				}
			}
		}

		// Token: 0x060018A8 RID: 6312 RVA: 0x000AADC0 File Offset: 0x000A8FC0
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x04000C09 RID: 3081
		private List<IFaction> _factionsThatDoNotHavePort;
	}
}
