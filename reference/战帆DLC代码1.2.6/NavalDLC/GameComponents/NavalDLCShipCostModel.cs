using System;
using System.Collections.Generic;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000132 RID: 306
	public class NavalDLCShipCostModel : ShipCostModel
	{
		// Token: 0x060014F1 RID: 5361 RVA: 0x00093258 File Offset: 0x00091458
		public override float GetShipTradeValue(Ship ship, PartyBase seller, PartyBase buyer)
		{
			bool flag = buyer != null && buyer.IsMobile && buyer.MobileParty.ActualClan != Clan.PlayerClan && seller.IsSettlement;
			float num = NavalDLCShipCostModel.GetShipBaseValue(ship, flag, seller, buyer) * 1.5f;
			if (buyer != null)
			{
				Clan clan = null;
				Kingdom kingdom = null;
				if (buyer.IsMobile)
				{
					clan = buyer.MobileParty.ActualClan;
					kingdom = ((clan != null) ? clan.Kingdom : null);
				}
				else if (buyer.IsSettlement)
				{
					clan = buyer.Settlement.OwnerClan;
					kingdom = ((clan != null) ? clan.Kingdom : null);
				}
				if (kingdom != null)
				{
					if (kingdom.HasPolicy(NavalPolicies.RoyalNavyPrerogative) && kingdom.RulingClan == clan)
					{
						num *= 0.9f;
					}
					if (ship.Owner.IsSettlement && ship.Owner.Settlement.OwnerClan.Kingdom != null && ship.Owner.Settlement.OwnerClan.Kingdom == kingdom && kingdom.HasPolicy(NavalPolicies.ArsenalDepositoryAct))
					{
						num *= 0.85f;
					}
				}
				if (seller.IsMobile && buyer.IsSettlement)
				{
					num = num * 0.3f - Campaign.Current.Models.ShipCostModel.GetShipRepairCost(ship, ship.Owner);
				}
			}
			return num;
		}

		// Token: 0x060014F2 RID: 5362 RVA: 0x00093394 File Offset: 0x00091594
		private static float GetShipBaseValue(Ship ship, bool applyAiDiscount, PartyBase owner, PartyBase buyer)
		{
			float num = (float)ship.ShipHull.Value;
			if (applyAiDiscount)
			{
				num *= 0.01f;
			}
			int num2 = 0;
			foreach (KeyValuePair<string, ShipSlot> keyValuePair in ship.ShipHull.AvailableSlots)
			{
				int num3 = 0;
				ShipUpgradePiece pieceAtSlot = ship.GetPieceAtSlot(keyValuePair.Key);
				if (pieceAtSlot != null)
				{
					num3 = NavalDLCShipCostModel.GetShipUpgradePieceValueInternal(ship, pieceAtSlot, owner, buyer);
				}
				if (ship.UnlockedUpgradePieces != null)
				{
					for (int i = 0; i < ship.UnlockedUpgradePieces.Count; i++)
					{
						ShipUpgradePiece shipUpgradePiece = ship.UnlockedUpgradePieces[i];
						if (shipUpgradePiece.DoesPieceMatchSlot(keyValuePair.Value))
						{
							int shipUpgradePieceValueInternal = NavalDLCShipCostModel.GetShipUpgradePieceValueInternal(ship, shipUpgradePiece, owner, buyer);
							if (shipUpgradePieceValueInternal > num3)
							{
								num3 = shipUpgradePieceValueInternal;
							}
						}
					}
				}
				num2 += num3;
			}
			num += (float)num2 * 0.3f;
			return num;
		}

		// Token: 0x060014F3 RID: 5363 RVA: 0x00093490 File Offset: 0x00091690
		public override float GetShipRepairCost(Ship ship, PartyBase owner)
		{
			float num = (ship.MaxHitPoints - ship.HitPoints) / ship.MaxHitPoints;
			Clan clan;
			if (owner == null)
			{
				clan = null;
			}
			else
			{
				MobileParty mobileParty = owner.MobileParty;
				clan = ((mobileParty != null) ? mobileParty.ActualClan : null);
			}
			bool flag = clan != Clan.PlayerClan;
			ExplainedNumber explainedNumber;
			explainedNumber..ctor(NavalDLCShipCostModel.GetShipBaseValue(ship, flag, owner, owner) * num * 0.25f, false, null);
			if (owner != null && owner.MobileParty != null)
			{
				PerkHelper.AddPerkBonusForParty(NavalPerks.Boatswain.MerchantPrince, owner.MobileParty, true, ref explainedNumber, false);
			}
			return explainedNumber.ResultNumber;
		}

		// Token: 0x060014F4 RID: 5364 RVA: 0x00093518 File Offset: 0x00091718
		public override int GetShipUpgradePieceCost(Ship ship, ShipUpgradePiece piece, PartyBase owner)
		{
			MBReadOnlyList<ShipUpgradePiece> unlockedUpgradePieces = ship.UnlockedUpgradePieces;
			if (unlockedUpgradePieces != null && unlockedUpgradePieces.Contains(piece))
			{
				return 0;
			}
			foreach (KeyValuePair<string, ShipSlot> keyValuePair in ship.ShipHull.AvailableSlots)
			{
				if (ship.GetPieceAtSlot(keyValuePair.Key) == piece)
				{
					return 0;
				}
			}
			return NavalDLCShipCostModel.GetShipUpgradePieceValueInternal(ship, piece, owner, owner);
		}

		// Token: 0x060014F5 RID: 5365 RVA: 0x000935A0 File Offset: 0x000917A0
		private static int GetShipUpgradePieceValueInternal(Ship ship, ShipUpgradePiece piece, PartyBase owner, PartyBase buyer)
		{
			float num = (float)piece.LightValue;
			if (ship.ShipHull.Type == 1)
			{
				num = (float)piece.MediumValue;
			}
			else if (ship.ShipHull.Type == 2)
			{
				num = (float)piece.HeavyValue;
			}
			if (owner != null)
			{
				if (owner.IsMobile)
				{
					ExplainedNumber explainedNumber;
					explainedNumber..ctor(num, false, null);
					PerkHelper.AddPerkBonusForParty(NavalPerks.Boatswain.MasterShipwright, owner.MobileParty, true, ref explainedNumber, false);
					num = explainedNumber.ResultNumber;
				}
				MobileParty mobileParty = owner.MobileParty;
				Clan clan = ((mobileParty != null) ? mobileParty.ActualClan : null);
				Kingdom kingdom = ((clan != null) ? clan.Kingdom : null);
				if (kingdom != null && kingdom.RulingClan == clan && kingdom.HasPolicy(NavalPolicies.RoyalNavyPrerogative))
				{
					num *= 0.9f;
				}
			}
			Clan clan2;
			if (owner == null)
			{
				clan2 = null;
			}
			else
			{
				MobileParty mobileParty2 = owner.MobileParty;
				clan2 = ((mobileParty2 != null) ? mobileParty2.ActualClan : null);
			}
			if (clan2 != Clan.PlayerClan)
			{
				Clan clan3;
				if (buyer == null)
				{
					clan3 = null;
				}
				else
				{
					MobileParty mobileParty3 = buyer.MobileParty;
					clan3 = ((mobileParty3 != null) ? mobileParty3.ActualClan : null);
				}
				if (clan3 != Clan.PlayerClan)
				{
					num *= 0.01f;
				}
			}
			return MathF.Round(num);
		}

		// Token: 0x060014F6 RID: 5366 RVA: 0x000936A1 File Offset: 0x000918A1
		public override float GetShipSellingPenalty()
		{
			return 0.3f;
		}

		// Token: 0x04000B00 RID: 2816
		private const float BuyPenalty = 1.5f;

		// Token: 0x04000B01 RID: 2817
		private const float RepairPenalty = 0.25f;

		// Token: 0x04000B02 RID: 2818
		private const float SellPenalty = 0.3f;

		// Token: 0x04000B03 RID: 2819
		private const float UpgradePiecePenalty = 0.3f;

		// Token: 0x04000B04 RID: 2820
		private const float AIClansShipValueDiscountRatio = 0.01f;

		// Token: 0x04000B05 RID: 2821
		private const float RoyalNavyPrerogativeMultiplier = 0.9f;
	}
}
