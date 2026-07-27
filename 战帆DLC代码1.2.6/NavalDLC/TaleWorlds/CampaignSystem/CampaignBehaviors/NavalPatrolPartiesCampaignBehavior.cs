using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using NavalDLC;
using NavalDLC.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.CampaignSystem.CampaignBehaviors
{
	// Token: 0x02000016 RID: 22
	public class NavalPatrolPartiesCampaignBehavior : CampaignBehaviorBase, INavalPatrolPartiesCampaignBehavior
	{
		// Token: 0x060000E6 RID: 230 RVA: 0x00007884 File Offset: 0x00005A84
		public override void RegisterEvents()
		{
			CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.DailyTickSettlement));
			CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener(this, new Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail>(this.OnSettlementOwnerChangedEvent));
			CampaignEvents.OnNewGameCreatedPartialFollowUpEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter, int>(this.OnNewGameCreated));
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.SettlementEntered));
			CampaignEvents.AiHourlyTickEvent.AddNonSerializedListener(this, new Action<MobileParty, PartyThinkParams>(this.AiHourlyTick));
			CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnSettlementLeft));
			CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.OnMobilePartyDestroyed));
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00007932 File Offset: 0x00005B32
		private void OnMobilePartyDestroyed(MobileParty party, PartyBase destroyerParty)
		{
			if (party.IsPatrolParty && party.PatrolPartyComponent.IsNaval)
			{
				this._patrolParties.Remove(party.HomeSettlement);
			}
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x0000795C File Offset: 0x00005B5C
		private void SettlementEntered(MobileParty party, Settlement settlement, Hero hero)
		{
			if (party != null && party.IsPatrolParty && party.PatrolPartyComponent.IsNaval && settlement == party.HomeSettlement)
			{
				for (int i = 0; i < party.Ships.Count; i++)
				{
					RepairShipAction.ApplyForFree(party.Ships[i]);
				}
				foreach (ShipTemplateStack shipTemplateStack in Campaign.Current.Models.SettlementPatrolModel.GetPartyTemplateForPatrolParty(settlement, true).ShipHulls)
				{
					ShipHull shipHull = shipTemplateStack.ShipHull;
					int num = party.Ships.Count<Ship>((Ship x) => x.ShipHull == shipHull);
					if (num < shipTemplateStack.MaxValue)
					{
						for (int j = 0; j < shipTemplateStack.MaxValue - num; j++)
						{
							Ship ship = new Ship(shipHull);
							ChangeShipOwnerAction.ApplyByTransferring(party.Party, ship);
						}
					}
				}
			}
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00007A78 File Offset: 0x00005C78
		private void AiHourlyTick(MobileParty mobileParty, PartyThinkParams p)
		{
			if (!mobileParty.IsPatrolParty || mobileParty.IsDisbanding)
			{
				return;
			}
			if (!mobileParty.PatrolPartyComponent.IsNaval)
			{
				return;
			}
			Settlement currentSettlement = mobileParty.CurrentSettlement;
			if (((currentSettlement != null) ? currentSettlement.SiegeEvent : null) != null && mobileParty.CurrentSettlement.SiegeEvent.IsBlockadeActive)
			{
				return;
			}
			this.CalculateVisitHomeSettlementScoreDueToShipHealth(mobileParty, p);
		}

		// Token: 0x060000EA RID: 234 RVA: 0x00007AD4 File Offset: 0x00005CD4
		private void CalculateVisitHomeSettlementScoreDueToShipHealth(MobileParty mobileParty, PartyThinkParams p)
		{
			if (this.CanVisitSettlement(mobileParty, mobileParty.HomeSettlement))
			{
				float overallShipHealthRatio = this.GetOverallShipHealthRatio(mobileParty);
				if (overallShipHealthRatio < 0.95f)
				{
					float num = 1f / MathF.Max(overallShipHealthRatio, 0.01f);
					MobileParty.NavigationType navigationType;
					float num2;
					bool flag;
					AiHelper.GetBestNavigationTypeAndAdjustedDistanceOfSettlementForMobileParty(mobileParty, mobileParty.HomeSettlement, true, ref navigationType, ref num2, ref flag);
					AIBehaviorData aibehaviorData;
					aibehaviorData..ctor(mobileParty.HomeSettlement, 2, navigationType, false, flag, true);
					float num3;
					if (p.TryGetBehaviorScore(ref aibehaviorData, ref num3))
					{
						p.SetBehaviorScore(ref aibehaviorData, num + num3);
						return;
					}
					ValueTuple<AIBehaviorData, float> valueTuple = new ValueTuple<AIBehaviorData, float>(aibehaviorData, num);
					p.AddBehaviorScore(ref valueTuple);
				}
			}
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00007B63 File Offset: 0x00005D63
		private bool CanVisitSettlement(MobileParty mobileParty, Settlement settlement)
		{
			return settlement.SiegeEvent == null || !settlement.SiegeEvent.IsBlockadeActive;
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00007B80 File Offset: 0x00005D80
		private float GetOverallShipHealthRatio(MobileParty mobileParty)
		{
			float num = (float)Campaign.Current.Models.SettlementPatrolModel.GetPartyTemplateForPatrolParty(mobileParty.HomeSettlement, true).ShipHulls.Sum<ShipTemplateStack>((ShipTemplateStack x) => x.ShipHull.MaxHitPoints * x.MaxValue);
			return mobileParty.Ships.Sum<Ship>((Ship x) => MathF.Min(x.HitPoints, (float)x.ShipHull.MaxHitPoints)) / num;
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00007BFF File Offset: 0x00005DFF
		private void OnSettlementLeft(MobileParty party, Settlement settlement)
		{
			if (party.IsPatrolParty && party.PatrolPartyComponent.IsNaval)
			{
				Settlement homeSettlement = party.HomeSettlement;
			}
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00007C20 File Offset: 0x00005E20
		private void DailyTickSettlement(Settlement settlement)
		{
			TextObject textObject;
			if (this.CanSettlementSpawnNewPartyCurrently(settlement, false, out textObject))
			{
				CampaignTime campaignTime;
				if (!this._partyGenerationQueue.TryGetValue(settlement, out campaignTime))
				{
					this.UpdateSettlementQueue(settlement, CampaignTime.Now + Campaign.Current.Models.SettlementPatrolModel.GetPatrolPartySpawnDuration(settlement, true));
					return;
				}
				if (campaignTime.IsPast)
				{
					this.SpawnPatrolParty(settlement);
					return;
				}
			}
			else
			{
				this.UpdateSettlementParties(settlement);
			}
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00007C8C File Offset: 0x00005E8C
		private void OnNewGameCreated(CampaignGameStarter starter, int index)
		{
			if (index == 88)
			{
				foreach (Town town in Town.AllFiefs)
				{
					TextObject textObject;
					if (this.CanSettlementSpawnNewPartyCurrently(town.Settlement, false, out textObject))
					{
						this.SpawnPatrolParty(town.Settlement);
					}
				}
			}
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00007CF4 File Offset: 0x00005EF4
		private void OnSettlementOwnerChangedEvent(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementAction.ChangeOwnerOfSettlementDetail detail)
		{
			if (this.GetNavalPatrolParty(settlement) != null)
			{
				this.RemoveSettlementParties(settlement);
			}
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00007D08 File Offset: 0x00005F08
		private bool CanSettlementSpawnNewPartyCurrently(Settlement settlement, bool includeReason, out TextObject reason)
		{
			reason = null;
			if (!Campaign.Current.Models.SettlementPatrolModel.CanSettlementHavePatrolParties(settlement, true))
			{
				PolicyObject coastalGuardEdict = NavalPolicies.CoastalGuardEdict;
				if (includeReason)
				{
					reason = new TextObject("{=ipat9DbO}No {POLICY_NAME}", null);
					reason.SetTextVariable("POLICY_NAME", coastalGuardEdict.Name);
				}
				return false;
			}
			if (settlement.InRebelliousState)
			{
				if (includeReason)
				{
					reason = new TextObject("{=UHDv0qer}Rebellious", null);
				}
				return false;
			}
			if (settlement.Town.IsUnderSiege || settlement.Party.MapEvent != null)
			{
				if (includeReason)
				{
					reason = new TextObject("{=BhiOmgst}Under Siege", null);
				}
				return false;
			}
			if (includeReason)
			{
				reason = TextObject.GetEmpty();
			}
			return this.GetNavalPatrolParty(settlement) == null;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00007DB5 File Offset: 0x00005FB5
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<Settlement, CampaignTime>>("_partyGenerationQueue", ref this._partyGenerationQueue);
			dataStore.SyncData<Dictionary<Settlement, MobileParty>>("_patrolParties", ref this._patrolParties);
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00007DDB File Offset: 0x00005FDB
		private void UpdateSettlementParties(Settlement settlement)
		{
			if (!Campaign.Current.Models.SettlementPatrolModel.CanSettlementHavePatrolParties(settlement, true) && this.GetNavalPatrolParty(settlement) != null)
			{
				this.RemoveSettlementParties(settlement);
			}
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00007E08 File Offset: 0x00006008
		private void RemoveSettlementParties(Settlement settlement)
		{
			this._partyGenerationQueue.Remove(settlement);
			MobileParty navalPatrolParty = this.GetNavalPatrolParty(settlement);
			navalPatrolParty.MapEventSide = null;
			if (navalPatrolParty.IsActive)
			{
				DestroyPartyAction.Apply(null, navalPatrolParty);
			}
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00007E40 File Offset: 0x00006040
		private void UpdateSettlementQueue(Settlement settlement, CampaignTime time)
		{
			this._partyGenerationQueue[settlement] = time;
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00007E50 File Offset: 0x00006050
		private void SpawnPatrolParty(Settlement settlement)
		{
			this._partyGenerationQueue.Remove(settlement);
			PartyTemplateObject partyTemplateForPatrolParty = Campaign.Current.Models.SettlementPatrolModel.GetPartyTemplateForPatrolParty(settlement, true);
			MobileParty mobileParty = PatrolPartyComponent.CreatePatrolParty("naval_patrol_party_1", settlement.PortPosition, 8f * Campaign.Current.EstimatedAverageBanditPartySpeed, settlement, partyTemplateForPatrolParty);
			this._patrolParties[settlement] = mobileParty;
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x00007EB4 File Offset: 0x000060B4
		public TextObject GetSettlementPatrolStatus(Settlement settlement)
		{
			TextObject textObject = TextObject.GetEmpty();
			MobileParty navalPatrolParty = this.GetNavalPatrolParty(settlement);
			TextObject textObject2;
			CampaignTime campaignTime;
			if (navalPatrolParty != null)
			{
				textObject = new TextObject("{=sUb6FHIE}{REMAINING_TROOP_COUNT}/{TOTAL_TROOP_COUNT}", null);
				textObject.SetTextVariable("REMAINING_TROOP_COUNT", navalPatrolParty.MemberRoster.TotalManCount);
				textObject.SetTextVariable("TOTAL_TROOP_COUNT", navalPatrolParty.Party.PartySizeLimit);
			}
			else if (!this.CanSettlementSpawnNewPartyCurrently(settlement, true, out textObject2))
			{
				textObject = textObject2;
			}
			else if (this._partyGenerationQueue.TryGetValue(settlement, out campaignTime))
			{
				int num = ((campaignTime == CampaignTime.Zero) ? 1 : Math.Max((int)Math.Ceiling((double)campaignTime.RemainingDaysFromNow), 1));
				textObject = new TextObject("{=LvwUsZ9p}Ready in {DAYS} {?DAYS > 1}days{?}day{\\?}", null);
				textObject.SetTextVariable("DAYS", num);
			}
			else
			{
				textObject = new TextObject("{=trainingPatrolParties}Training", null);
			}
			return textObject;
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00007F80 File Offset: 0x00006180
		public MobileParty GetNavalPatrolParty(Settlement settlement)
		{
			MobileParty mobileParty;
			if (this._patrolParties.TryGetValue(settlement, out mobileParty))
			{
				return mobileParty;
			}
			return null;
		}

		// Token: 0x0400007D RID: 125
		private Dictionary<Settlement, CampaignTime> _partyGenerationQueue = new Dictionary<Settlement, CampaignTime>();

		// Token: 0x0400007E RID: 126
		private Dictionary<Settlement, MobileParty> _patrolParties = new Dictionary<Settlement, MobileParty>();
	}
}
