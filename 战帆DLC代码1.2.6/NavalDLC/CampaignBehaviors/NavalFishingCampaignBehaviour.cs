using System;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.CampaignBehaviors
{
	// Token: 0x02000167 RID: 359
	public class NavalFishingCampaignBehaviour : CampaignBehaviorBase
	{
		// Token: 0x060017A9 RID: 6057 RVA: 0x000A14F9 File Offset: 0x0009F6F9
		public override void RegisterEvents()
		{
			CampaignEvents.HourlyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.OnHourlyTickParty));
			CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, new Action<Settlement>(this.OnDailyTickSettlement));
		}

		// Token: 0x060017AA RID: 6058 RVA: 0x000A152C File Offset: 0x0009F72C
		private void OnDailyTickSettlement(Settlement settlement)
		{
			if (settlement.IsVillage && settlement.Village.TradeBound != null)
			{
				ExplainedNumber explainedNumber;
				explainedNumber..ctor(0f, false, null);
				PerkHelper.AddPerkBonusForTown(NavalPerks.Shipmaster.NightRaider, settlement.Village.TradeBound.Town, ref explainedNumber);
				if (explainedNumber.RoundedResultNumber > 0)
				{
					ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>("fish");
					int roundedResultNumber = explainedNumber.RoundedResultNumber;
					settlement.Village.Owner.ItemRoster.AddToCounts(@object, roundedResultNumber);
					CampaignEventDispatcher.Instance.OnItemProduced(@object, settlement.Village.Owner.Settlement, roundedResultNumber);
				}
			}
		}

		// Token: 0x060017AB RID: 6059 RVA: 0x000A15D4 File Offset: 0x0009F7D4
		private void OnHourlyTickParty(MobileParty party)
		{
			if (party.IsCurrentlyAtSea)
			{
				float num = 0f;
				if (party.HasPerk(NavalPerks.Shipmaster.MasterAngler, false))
				{
					num += NavalPerks.Shipmaster.MasterAngler.PrimaryBonus;
				}
				if (MBRandom.RandomFloat < num)
				{
					ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>("fish");
					party.ItemRoster.AddToCounts(@object, 1);
				}
			}
		}

		// Token: 0x060017AC RID: 6060 RVA: 0x000A1630 File Offset: 0x0009F830
		public override void SyncData(IDataStore dataStore)
		{
		}
	}
}
