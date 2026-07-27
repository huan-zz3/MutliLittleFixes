using System;
using System.Collections.Generic;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace NavalDLC.CampaignBehaviors
{
	// Token: 0x0200016B RID: 363
	public class NavalNimbleSurgeCampaignBehaviour : CampaignBehaviorBase
	{
		// Token: 0x060017DD RID: 6109 RVA: 0x000A2F1F File Offset: 0x000A111F
		public override void RegisterEvents()
		{
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.OnMobilePartyDestroyed));
		}

		// Token: 0x060017DE RID: 6110 RVA: 0x000A2F4F File Offset: 0x000A114F
		private void OnMobilePartyDestroyed(MobileParty mobileParty, PartyBase destroyerParty)
		{
			this._lastTimeEntered.Remove(mobileParty);
		}

		// Token: 0x060017DF RID: 6111 RVA: 0x000A2F5E File Offset: 0x000A115E
		public override void SyncData(IDataStore dataStore)
		{
			this.DoCleanUp();
			dataStore.SyncData<Dictionary<MobileParty, Dictionary<Settlement, CampaignTime>>>("_lastTimeEntered", ref this._lastTimeEntered);
		}

		// Token: 0x060017E0 RID: 6112 RVA: 0x000A2F78 File Offset: 0x000A1178
		private void OnSettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero)
		{
			if (mobileParty != null && mobileParty.IsCaravan && mobileParty.HasNavalNavigationCapability && settlement.IsFortification && settlement.Town.Governor != null && settlement.Town.BuildingsInProgress.Count > 0)
			{
				Town town = settlement.Town;
				if (town.Governor.GetPerkValue(NavalPerks.Shipmaster.FavorableTide) && (!this._lastTimeEntered.ContainsKey(mobileParty) || !this._lastTimeEntered[mobileParty].ContainsKey(settlement) || this._lastTimeEntered[mobileParty][settlement].ElapsedDaysUntilNow > 1f))
				{
					if (!this._lastTimeEntered.ContainsKey(mobileParty))
					{
						this._lastTimeEntered[mobileParty] = new Dictionary<Settlement, CampaignTime>();
					}
					this._lastTimeEntered[mobileParty][settlement] = CampaignTime.Now;
					town.CurrentBuilding.BuildingProgress += 1f;
					BuildingHelper.CheckIfBuildingIsComplete(town.CurrentBuilding);
				}
			}
		}

		// Token: 0x060017E1 RID: 6113 RVA: 0x000A308C File Offset: 0x000A128C
		private void DoCleanUp()
		{
			foreach (KeyValuePair<MobileParty, Dictionary<Settlement, CampaignTime>> keyValuePair in this._lastTimeEntered)
			{
				List<Settlement> list = new List<Settlement>();
				foreach (KeyValuePair<Settlement, CampaignTime> keyValuePair2 in keyValuePair.Value)
				{
					if (keyValuePair2.Value.ElapsedDaysUntilNow > 1f)
					{
						list.Add(keyValuePair2.Key);
					}
				}
				foreach (Settlement settlement in list)
				{
					keyValuePair.Value.Remove(settlement);
				}
			}
			List<MobileParty> list2 = new List<MobileParty>();
			foreach (KeyValuePair<MobileParty, Dictionary<Settlement, CampaignTime>> keyValuePair3 in this._lastTimeEntered)
			{
				if (this._lastTimeEntered[keyValuePair3.Key] == null || this._lastTimeEntered[keyValuePair3.Key].Count == 0)
				{
					list2.Add(keyValuePair3.Key);
				}
			}
			foreach (MobileParty mobileParty in list2)
			{
				this._lastTimeEntered.Remove(mobileParty);
			}
		}

		// Token: 0x04000BED RID: 3053
		private Dictionary<MobileParty, Dictionary<Settlement, CampaignTime>> _lastTimeEntered = new Dictionary<MobileParty, Dictionary<Settlement, CampaignTime>>();
	}
}
