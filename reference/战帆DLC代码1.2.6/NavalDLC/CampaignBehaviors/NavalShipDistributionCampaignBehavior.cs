using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;

namespace NavalDLC.CampaignBehaviors
{
	// Token: 0x0200016D RID: 365
	public class NavalShipDistributionCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x060017EA RID: 6122 RVA: 0x000A3465 File Offset: 0x000A1665
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x060017EB RID: 6123 RVA: 0x000A3467 File Offset: 0x000A1667
		public override void RegisterEvents()
		{
			CampaignEvents.OnPartyDisbandedEvent.AddNonSerializedListener(this, new Action<MobileParty, Settlement>(this.OnPartyDisbanded));
			CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.OnMobilePartyDestroyed));
		}

		// Token: 0x060017EC RID: 6124 RVA: 0x000A3497 File Offset: 0x000A1697
		private void OnMobilePartyDestroyed(MobileParty party, PartyBase destroyerParty)
		{
			if (party.ActualClan != null && !party.IsCurrentlyAtSea)
			{
				this.DistributePartyShipsAndRecoverGold(party);
			}
		}

		// Token: 0x060017ED RID: 6125 RVA: 0x000A34B0 File Offset: 0x000A16B0
		private void DistributePartyShipsAndRecoverGold(MobileParty mobileParty)
		{
			this.DistributeShips(mobileParty);
			this.RecoverGoldFromRemainingShipsAfterDistribution(mobileParty);
		}

		// Token: 0x060017EE RID: 6126 RVA: 0x000A34C0 File Offset: 0x000A16C0
		private void OnPartyDisbanded(MobileParty disbandParty, Settlement relatedSettlement)
		{
			if (disbandParty.ActualClan != null && !disbandParty.ActualClan.IsBanditFaction)
			{
				this.DistributePartyShipsAndRecoverGold(disbandParty);
			}
		}

		// Token: 0x060017EF RID: 6127 RVA: 0x000A34E0 File Offset: 0x000A16E0
		private void RecoverGoldFromRemainingShipsAfterDistribution(MobileParty party)
		{
			if (party.ActualClan != null && !party.ActualClan.IsBanditFaction && party.ActualClan.Leader != null && party.ActualClan.Leader.IsActive && party.Ships.Count > 0)
			{
				int num = (int)LinQuick.SumQ<Ship>(party.Ships, (Ship x) => Campaign.Current.Models.ShipCostModel.GetShipTradeValue(x, party.Party, null));
				if (party.ActualClan == Clan.PlayerClan)
				{
					float shipSellingPenalty = Campaign.Current.Models.ShipCostModel.GetShipSellingPenalty();
					num = (int)((float)num * shipSellingPenalty);
					if (party.Owner != null)
					{
						MBTextManager.SetTextVariable("GOLD_AMOUNT", num);
						MBTextManager.SetTextVariable("LEADER_NAME", party.Owner.Name, false);
						MBTextManager.SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"6\">", false);
						MBInformationManager.AddQuickInformation(new TextObject("{=YaSnA9j0}{LEADER_NAME}'s party has disbanded. You recovered {GOLD_AMOUNT}{GOLD_ICON} from its ships.", null), 0, null, null, "");
					}
				}
				GiveGoldAction.ApplyBetweenCharacters(null, party.ActualClan.Leader, num, false);
			}
		}

		// Token: 0x060017F0 RID: 6128 RVA: 0x000A3628 File Offset: 0x000A1828
		private void DistributeShips(MobileParty party)
		{
			for (int i = party.Ships.Count - 1; i >= 0; i--)
			{
				Ship shipToSend = party.Ships[i];
				if (LinQuick.AnyQ<WarPartyComponent>(party.ActualClan.WarPartyComponents, (WarPartyComponent x) => x.MobileParty != party && NavalDLCManager.Instance.GameModels.ShipDistributionModel.CanSendShipToParty(shipToSend, x.MobileParty)))
				{
					MobileParty clanPartyToGetShipOfDisbandingParty = this.GetClanPartyToGetShipOfDisbandingParty(shipToSend, party.ActualClan);
					if (clanPartyToGetShipOfDisbandingParty != null && clanPartyToGetShipOfDisbandingParty != party)
					{
						ChangeShipOwnerAction.ApplyByTransferring(clanPartyToGetShipOfDisbandingParty.Party, shipToSend);
					}
				}
			}
		}

		// Token: 0x060017F1 RID: 6129 RVA: 0x000A36F8 File Offset: 0x000A18F8
		private MobileParty GetClanPartyToGetShipOfDisbandingParty(Ship ship, Clan clan)
		{
			MobileParty mobileParty = null;
			float num = 0f;
			MBList<Ship> mblist = new MBList<Ship>();
			foreach (WarPartyComponent warPartyComponent in clan.WarPartyComponents)
			{
				if (warPartyComponent.Party != ship.Owner && NavalDLCManager.Instance.GameModels.ShipDistributionModel.CanSendShipToParty(ship, warPartyComponent.MobileParty) && (mobileParty == null || warPartyComponent.Party.Ships.Count <= mobileParty.Ships.Count))
				{
					mblist.Clear();
					mblist.AddRange(warPartyComponent.Party.Ships);
					float scoreForPartyShipComposition = NavalDLCManager.Instance.GameModels.ShipDistributionModel.GetScoreForPartyShipComposition(warPartyComponent.MobileParty, mblist);
					mblist.Add(ship);
					float num2 = NavalDLCManager.Instance.GameModels.ShipDistributionModel.GetScoreForPartyShipComposition(warPartyComponent.MobileParty, mblist) - scoreForPartyShipComposition;
					if (num2 > num)
					{
						mobileParty = warPartyComponent.MobileParty;
						num = num2;
					}
				}
			}
			return mobileParty;
		}
	}
}
