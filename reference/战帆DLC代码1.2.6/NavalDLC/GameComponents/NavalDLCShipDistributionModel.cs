using System;
using NavalDLC.ComponentInterfaces;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000134 RID: 308
	public class NavalDLCShipDistributionModel : ShipDistributionModel
	{
		// Token: 0x06001505 RID: 5381 RVA: 0x00094671 File Offset: 0x00092871
		public override bool CanPartyTakeShip(PartyBase party, Ship ship)
		{
			return !party.IsMobile || !party.MobileParty.IsBandit || ship.ShipHull.Type != 2;
		}

		// Token: 0x06001506 RID: 5382 RVA: 0x0009469C File Offset: 0x0009289C
		public override bool CanSendShipToParty(Ship ship, MobileParty mobileParty)
		{
			return mobileParty != MobileParty.MainParty && mobileParty.IsActive && (!mobileParty.IsCurrentlyAtSea || mobileParty.MapEvent == null) && !mobileParty.IsDisbanding && !mobileParty.IsCaravan && !mobileParty.IsCurrentlyUsedByAQuest && !mobileParty.IsMilitia && !mobileParty.IsPatrolParty && !mobileParty.IsVillager;
		}

		// Token: 0x06001507 RID: 5383 RVA: 0x000946FC File Offset: 0x000928FC
		public override float GetScoreForPartyShipComposition(MobileParty party, MBReadOnlyList<Ship> shipsToConsider)
		{
			if (shipsToConsider.Count == 0)
			{
				return 0f;
			}
			float num = 1f;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			CultureObject cultureObject = null;
			if (party.ActualClan != null)
			{
				cultureObject = party.ActualClan.Culture;
			}
			else if (party.MapFaction != null)
			{
				cultureObject = party.MapFaction.Culture;
			}
			foreach (Ship ship in shipsToConsider)
			{
				if (!LinQuick.ContainsQ<ShipHull>(cultureObject.AvailableShipHulls, ship.ShipHull))
				{
					num *= 0.96f;
				}
				switch (ship.ShipHull.Type)
				{
				case 0:
					num2++;
					break;
				case 1:
					num3++;
					break;
				case 2:
					num4++;
					break;
				}
			}
			if (num2 < 1)
			{
				num *= 0.85f;
			}
			if (num3 < 1)
			{
				num *= 0.9f;
			}
			if (num4 < 1)
			{
				num *= 0.95f;
			}
			int num5 = LinQuick.SumQ<Ship>(shipsToConsider, (Ship x) => x.SkeletalCrewCapacity);
			int num6 = (int)Campaign.Current.Models.PartySizeLimitModel.GetPartyMemberSizeLimit(party.Party, false).ResultNumber;
			float num7 = (float)num6 * 0.5f;
			int idealShipNumber = Campaign.Current.Models.PartyShipLimitModel.GetIdealShipNumber(party);
			if (num7 < (float)num5)
			{
				float num8 = 1f - ((float)num5 - num7) / (float)num5 * 0.5f;
				num *= num8;
			}
			else if (shipsToConsider.Count > idealShipNumber)
			{
				num *= 2f / (float)(shipsToConsider.Count - idealShipNumber + 1);
			}
			int num9 = LinQuick.SumQ<Ship>(shipsToConsider, (Ship x) => x.ShipHull.TotalCrewCapacity);
			if ((float)num9 < (float)num6 * 0.85f)
			{
				num *= (float)num9 / (float)num6 * 15f / 85f + 0.85f;
			}
			else if ((float)num9 > (float)num6 * 1.3f)
			{
				num *= 0.8f;
			}
			return num;
		}

		// Token: 0x04000B08 RID: 2824
		private const float CulturePenalty = 0.96f;
	}
}
