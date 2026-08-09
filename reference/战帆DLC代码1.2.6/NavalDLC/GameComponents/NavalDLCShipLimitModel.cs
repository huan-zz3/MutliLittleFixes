using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000135 RID: 309
	public class NavalDLCShipLimitModel : PartyShipLimitModel
	{
		// Token: 0x06001509 RID: 5385 RVA: 0x0009492C File Offset: 0x00092B2C
		public override int GetIdealShipNumber(MobileParty mobileParty)
		{
			if (mobileParty.IsCaravan)
			{
				return 3;
			}
			if (mobileParty.IsLordParty)
			{
				return 3;
			}
			if (mobileParty.IsBandit)
			{
				return 3;
			}
			Debug.FailedAssert("Unhandled case", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\GameComponents\\NavalDLCShipLimitModel.cs", "GetIdealShipNumber", 34);
			return base.BaseModel.GetIdealShipNumber(mobileParty);
		}

		// Token: 0x0600150A RID: 5386 RVA: 0x00094979 File Offset: 0x00092B79
		public override int GetIdealShipNumber(Clan clan)
		{
			return clan.WarPartyComponents.Count * 3;
		}

		// Token: 0x0600150B RID: 5387 RVA: 0x00094988 File Offset: 0x00092B88
		public override float GetShipPriority(MobileParty mobileParty, Ship ship, bool isSelling)
		{
			if (mobileParty.IsBandit)
			{
				return MBMath.ClampFloat(ship.HitPoints / ship.MaxHitPoints, 0f, 1f);
			}
			if (!mobileParty.IsCaravan)
			{
				return 1f;
			}
			if (ship.ShipHull.Type != 2 && (ship.ShipHull.Type != 1 || mobileParty.CaravanPartyComponent.IsElite))
			{
				float baseSpeed = ship.ShipHull.BaseSpeed;
				float inventoryCapacity = ship.InventoryCapacity;
				float num = (float)ship.SeaWorthiness;
				float maxHitPoints = ship.MaxHitPoints;
				return inventoryCapacity * 2f + baseSpeed * 10f + num + maxHitPoints * 0.1f;
			}
			if (!isSelling)
			{
				return float.MinValue;
			}
			return float.MaxValue;
		}

		// Token: 0x04000B09 RID: 2825
		private const int LordPartyShipBaseLimit = 3;

		// Token: 0x04000B0A RID: 2826
		private const int ConvoyPartyShipBaseLimit = 3;

		// Token: 0x04000B0B RID: 2827
		private const int BanditPartyShipBaseLimit = 3;

		// Token: 0x04000B0C RID: 2828
		private const float MustSellPriorityValue = 3.4028235E+38f;

		// Token: 0x04000B0D RID: 2829
		private const float MustDiscardPriorityValue = -3.4028235E+38f;
	}
}
