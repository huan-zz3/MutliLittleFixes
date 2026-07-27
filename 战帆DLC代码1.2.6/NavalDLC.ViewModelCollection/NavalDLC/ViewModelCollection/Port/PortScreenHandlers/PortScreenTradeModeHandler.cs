using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.CampaignBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.ViewModelCollection.Port.PortScreenHandlers
{
	// Token: 0x0200001F RID: 31
	public class PortScreenTradeModeHandler : PortScreenHandler
	{
		// Token: 0x0600025F RID: 607 RVA: 0x0000CBD6 File Offset: 0x0000ADD6
		public PortScreenTradeModeHandler(PartyBase leftOwner, PartyBase rightOwner)
			: base(leftOwner.Ships, rightOwner.Ships)
		{
			this._leftOwner = leftOwner;
			this._rightOwner = rightOwner;
		}

		// Token: 0x06000260 RID: 608 RVA: 0x0000CBF8 File Offset: 0x0000ADF8
		public override TextObject GetLeftRosterName()
		{
			if (this._leftOwner.IsSettlement)
			{
				return new TextObject("{=UeUkbDVz}Port of {SETTLEMENT}", null).SetTextVariable("SETTLEMENT", this._leftOwner.Name);
			}
			return this._leftOwner.Name;
		}

		// Token: 0x06000261 RID: 609 RVA: 0x0000CC33 File Offset: 0x0000AE33
		public override TextObject GetRightRosterName()
		{
			return this._rightOwner.Name;
		}

		// Token: 0x06000262 RID: 610 RVA: 0x0000CC40 File Offset: 0x0000AE40
		public override PartyBase GetLeftSideOwnerParty()
		{
			return this._leftOwner;
		}

		// Token: 0x06000263 RID: 611 RVA: 0x0000CC48 File Offset: 0x0000AE48
		public override PartyBase GetRightSideOwnerParty()
		{
			return this._rightOwner;
		}

		// Token: 0x06000264 RID: 612 RVA: 0x0000CC50 File Offset: 0x0000AE50
		protected override PortActionInfo CanBuyShip(Ship ship)
		{
			bool flag = base.ShipsToSell.Any<PortScreenHandler.ShipTradeInfo>((PortScreenHandler.ShipTradeInfo x) => x.Ship == ship);
			int num = (flag ? base.ShipsToSell.FirstOrDefault<PortScreenHandler.ShipTradeInfo>((PortScreenHandler.ShipTradeInfo x) => x.Ship == ship).Price : this.GetTradeCostOfShip(ship, false));
			TextObject textObject = (flag ? GameTexts.FindText("str_port_buy_ship_back", null) : GameTexts.FindText("str_port_buy_ship", null));
			return PortActionInfo.CreateValid(true, num, textObject, TextObject.GetEmpty());
		}

		// Token: 0x06000265 RID: 613 RVA: 0x0000CCD8 File Offset: 0x0000AED8
		protected override PortActionInfo CanSellShip(Ship ship)
		{
			bool flag = base.ShipsToBuy.Any<PortScreenHandler.ShipTradeInfo>((PortScreenHandler.ShipTradeInfo x) => x.Ship == ship);
			int num = (flag ? base.ShipsToBuy.FirstOrDefault<PortScreenHandler.ShipTradeInfo>((PortScreenHandler.ShipTradeInfo x) => x.Ship == ship).Price : this.GetTradeCostOfShip(ship, true));
			TextObject textObject = (flag ? GameTexts.FindText("str_port_sell_ship_back", null) : GameTexts.FindText("str_port_sell_ship", null));
			if (MobileParty.MainParty.IsCurrentlyAtSea && base.RightShips.Count == 1)
			{
				Debug.FailedAssert("Trade mode should not be accessible from the sea!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.ViewModelCollection\\Port\\PortScreenHandlers\\PortScreenTradeModeHandler.cs", "CanSellShip", 67);
				PortActionInfo.CreateValid(false, num, textObject, GameTexts.FindText("str_cannot_give_all_ships", null));
			}
			return PortActionInfo.CreateValid(true, num, textObject, TextObject.GetEmpty());
		}

		// Token: 0x06000266 RID: 614 RVA: 0x0000CDA4 File Offset: 0x0000AFA4
		protected override PortActionInfo CanRepairShip(Ship ship)
		{
			if (base.ShipsToRepair.Contains(ship))
			{
				return PortActionInfo.CreateValid(false, 0, GameTexts.FindText("str_port_repair_ship", null), new TextObject("{=Ma26nyeo}Already repaired", null));
			}
			return PortActionInfo.CreateValid(true, this.GetRepairCostOfShip(ship, true), GameTexts.FindText("str_port_repair_ship", null), TextObject.GetEmpty());
		}

		// Token: 0x06000267 RID: 615 RVA: 0x0000CDFC File Offset: 0x0000AFFC
		protected override PortActionInfo CanRepairAll()
		{
			MBList<Ship> mblist = new MBList<Ship>();
			int num = 0;
			foreach (Ship ship in base.RightShips)
			{
				if (!base.ShipsToRepair.Contains(ship) && ship.HitPoints < ship.MaxHitPoints)
				{
					mblist.Add(ship);
					num += this.GetRepairCostOfShip(ship, true);
				}
			}
			if (mblist.Count == 0)
			{
				return PortActionInfo.CreateValid(false, 0, GameTexts.FindText("str_port_repair_all_ships", null), new TextObject("{=Ma26nyeo}Already repaired", null));
			}
			return PortActionInfo.CreateValid(true, num, GameTexts.FindText("str_port_repair_all_ships", null), TextObject.GetEmpty());
		}

		// Token: 0x06000268 RID: 616 RVA: 0x0000CEBC File Offset: 0x0000B0BC
		protected override PortActionInfo CanUpgradeShip(Ship ship)
		{
			return PortActionInfo.CreateValid(true, 0, GameTexts.FindText("str_port_upgrade_ship", null), TextObject.GetEmpty());
		}

		// Token: 0x06000269 RID: 617 RVA: 0x0000CED5 File Offset: 0x0000B0D5
		protected override PortActionInfo CanRenameShip(Ship ship)
		{
			return PortActionInfo.CreateValid(true, 0, GameTexts.FindText("str_port_rename_ship", null), TextObject.GetEmpty());
		}

		// Token: 0x0600026A RID: 618 RVA: 0x0000CEF0 File Offset: 0x0000B0F0
		protected override PortActionInfo CanSendToClan(Ship ship)
		{
			int num = base.ShipsToSend.Count * Campaign.Current.Models.FleetManagementModel.MinimumTroopCountRequiredToSendShips;
			TextObject textObject;
			return PortActionInfo.CreateValid(Campaign.Current.Models.FleetManagementModel.CanSendShipToPlayerClan(ship, base.RightShips.Count, num, ref textObject), 0, GameTexts.FindText("str_port_send_ship_to_clan", null), textObject);
		}

		// Token: 0x0600026B RID: 619 RVA: 0x0000CF54 File Offset: 0x0000B154
		public override int GetTradeCostOfShip(Ship ship, bool isRightSideSelling)
		{
			PartyBase partyBase = (isRightSideSelling ? this._rightOwner : this._leftOwner);
			PartyBase partyBase2 = (isRightSideSelling ? this._leftOwner : this._rightOwner);
			return (int)Campaign.Current.Models.ShipCostModel.GetShipTradeValue(ship, partyBase, partyBase2);
		}

		// Token: 0x0600026C RID: 620 RVA: 0x0000CFA0 File Offset: 0x0000B1A0
		public override int GetRepairCostOfShip(Ship ship, bool isRightSideRepairing)
		{
			PartyBase partyBase = (isRightSideRepairing ? this._rightOwner : this._leftOwner);
			return (int)Campaign.Current.Models.ShipCostModel.GetShipRepairCost(ship, partyBase);
		}

		// Token: 0x0600026D RID: 621 RVA: 0x0000CFD8 File Offset: 0x0000B1D8
		public override int GetUpgradeCostOfShip(Ship ship, ShipUpgradePiece piece, bool isRightSideUpgrading)
		{
			PartyBase partyBase = (isRightSideUpgrading ? this._rightOwner : this._leftOwner);
			return Campaign.Current.Models.ShipCostModel.GetShipUpgradePieceCost(ship, piece, partyBase);
		}

		// Token: 0x0600026E RID: 622 RVA: 0x0000D010 File Offset: 0x0000B210
		public override int GetTotalGoldCost()
		{
			int num = 0;
			for (int i = 0; i < base.ShipsToBuy.Count; i++)
			{
				num += base.ShipsToBuy[i].Price;
			}
			for (int j = 0; j < base.ShipsToSell.Count; j++)
			{
				num -= base.ShipsToSell[j].Price;
			}
			for (int k = 0; k < base.ShipsToRepair.Count; k++)
			{
				Ship ship = base.ShipsToRepair[k];
				num += this.GetRepairCostOfShip(ship, true);
			}
			for (int l = 0; l < base.SelectedShipPieces.Count; l++)
			{
				Ship ship2 = base.SelectedShipPieces[l].Ship;
				ShipUpgradePiece piece = base.SelectedShipPieces[l].Piece;
				if (piece != null)
				{
					num += this.GetUpgradeCostOfShip(ship2, piece, true);
				}
			}
			return num;
		}

		// Token: 0x0600026F RID: 623 RVA: 0x0000D0F8 File Offset: 0x0000B2F8
		public override bool GetCanConfirm(out TextObject disabledHint)
		{
			if (this.GetTotalGoldCost() > Hero.MainHero.Gold)
			{
				disabledHint = new TextObject("{=RYJdU43V}Not Enough Gold", null);
				return false;
			}
			disabledHint = null;
			return true;
		}

		// Token: 0x06000270 RID: 624 RVA: 0x0000D120 File Offset: 0x0000B320
		public override void OnConfirmChanges()
		{
			for (int i = 0; i < base.ShipsToBuy.Count; i++)
			{
				Ship ship = base.ShipsToBuy[i].Ship;
				ChangeShipOwnerAction.ApplyByTrade(this._rightOwner, ship);
			}
			for (int j = 0; j < base.ShipsToSell.Count; j++)
			{
				Ship ship2 = base.ShipsToSell[j].Ship;
				ChangeShipOwnerAction.ApplyByTrade(this._leftOwner, ship2);
			}
			for (int k = 0; k < base.ShipsToRepair.Count; k++)
			{
				RepairShipAction.Apply(base.ShipsToRepair[k], Settlement.CurrentSettlement);
			}
			for (int l = 0; l < base.ShipsToRename.Count; l++)
			{
				PortScreenHandler.ShipRenameInfo shipRenameInfo = base.ShipsToRename[l];
				shipRenameInfo.Ship.SetName(new TextObject("{=!}" + shipRenameInfo.NewName, null));
			}
			for (int m = 0; m < base.SelectedShipPieces.Count; m++)
			{
				Ship ship3 = base.SelectedShipPieces[m].Ship;
				string shipSlotTag = base.SelectedShipPieces[m].ShipSlotTag;
				ShipUpgradePiece piece = base.SelectedShipPieces[m].Piece;
				int num = 0;
				if (piece != null)
				{
					num += this.GetUpgradeCostOfShip(ship3, piece, true);
				}
				ship3.EquipUpgradePiece(shipSlotTag, piece);
				if (num > 0)
				{
					GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, this._leftOwner.Settlement, num, false);
				}
				else
				{
					GiveGoldAction.ApplyForSettlementToCharacter(this._leftOwner.Settlement, Hero.MainHero, -num, false);
				}
			}
			for (int n = 0; n < base.SelectedFigureheads.Count; n++)
			{
				Ship ship4 = base.SelectedFigureheads[n].Ship;
				Figurehead figurehead = base.SelectedFigureheads[n].Figurehead;
				ship4.ChangeFigurehead(figurehead);
			}
			IFleetManagementCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<IFleetManagementCampaignBehavior>();
			for (int num2 = 0; num2 < base.ShipsToSend.Count; num2++)
			{
				campaignBehavior.SendShipToClan(base.ShipsToSend[num2], Clan.PlayerClan);
			}
			if (MobileParty.MainParty.Ships.Count == 0 && MobileParty.MainParty.Anchor.IsValid)
			{
				MobileParty.MainParty.Anchor.ResetPosition();
				return;
			}
			if (MobileParty.MainParty.Ships.Count > 0 && !MobileParty.MainParty.Anchor.IsValid && this._leftOwner.IsSettlement)
			{
				MobileParty.MainParty.Anchor.SetSettlement(this._leftOwner.Settlement);
			}
		}

		// Token: 0x06000271 RID: 625 RVA: 0x0000D3CC File Offset: 0x0000B5CC
		public override List<PortChangeInfo> GetChanges()
		{
			List<PortChangeInfo> list = new List<PortChangeInfo>();
			for (int i = 0; i < base.ShipsToBuy.Count; i++)
			{
				list.Add(new PortChangeInfo((float)base.ShipsToBuy[i].Price, new TextObject("{=9AIOcUuH}Buy {SHIP_NAME}", null).SetTextVariable("SHIP_NAME", base.ShipsToBuy[i].Ship.Name).ToString()));
			}
			for (int j = 0; j < base.ShipsToRename.Count; j++)
			{
				list.Add(new PortChangeInfo(0f, new TextObject("{=Fidoxgd1}Rename {SHIP_NAME} to {NEW_SHIP_NAME}", null).SetTextVariable("SHIP_NAME", base.ShipsToRename[j].Ship.Name).SetTextVariable("NEW_SHIP_NAME", base.ShipsToRename[j].NewName).ToString()));
			}
			for (int k = 0; k < base.ShipsToRepair.Count; k++)
			{
				list.Add(new PortChangeInfo((float)this.GetRepairCostOfShip(base.ShipsToRepair[k], true), new TextObject("{=HQK9kUD9}Repair {SHIP_NAME}", null).SetTextVariable("SHIP_NAME", this.GetShipNameConsideringRenames(base.ShipsToRepair[k])).ToString()));
			}
			for (int l = 0; l < base.ShipsToSend.Count; l++)
			{
				list.Add(new PortChangeInfo(0f, new TextObject("{=L1x30kUJ}Send {SHIP_NAME} to clan", null).SetTextVariable("SHIP_NAME", this.GetShipNameConsideringRenames(base.ShipsToSend[l])).ToString()));
			}
			for (int m = 0; m < base.SelectedShipPieces.Count; m++)
			{
				ShipUpgradePiece piece = base.SelectedShipPieces[m].Piece;
				ShipUpgradePiece pieceAtSlot = base.SelectedShipPieces[m].Ship.GetPieceAtSlot(base.SelectedShipPieces[m].ShipSlotTag);
				if (pieceAtSlot != null)
				{
					list.Add(new PortChangeInfo(0f, new TextObject("{=PniFsE6M}Remove {PIECE_NAME} from {SHIP_NAME}", null).SetTextVariable("SHIP_NAME", this.GetShipNameConsideringRenames(base.SelectedShipPieces[m].Ship)).SetTextVariable("PIECE_NAME", pieceAtSlot.GetName()).ToString()));
				}
				if (piece != null)
				{
					list.Add(new PortChangeInfo((float)this.GetUpgradeCostOfShip(base.SelectedShipPieces[m].Ship, piece, true), new TextObject("{=jwgUwyKO}Add {PIECE_NAME} to {SHIP_NAME}", null).SetTextVariable("SHIP_NAME", this.GetShipNameConsideringRenames(base.SelectedShipPieces[m].Ship)).SetTextVariable("PIECE_NAME", piece.GetName()).ToString()));
				}
			}
			for (int n = 0; n < base.SelectedFigureheads.Count; n++)
			{
				Figurehead figurehead = base.SelectedFigureheads[n].Figurehead;
				Figurehead figurehead2 = base.SelectedFigureheads[n].Ship.Figurehead;
				if (figurehead2 != null)
				{
					list.Add(new PortChangeInfo(0f, new TextObject("{=PniFsE6M}Remove {PIECE_NAME} from {SHIP_NAME}", null).SetTextVariable("SHIP_NAME", this.GetShipNameConsideringRenames(base.SelectedFigureheads[n].Ship)).SetTextVariable("PIECE_NAME", figurehead2.GetName()).ToString()));
				}
				if (figurehead != null)
				{
					list.Add(new PortChangeInfo(0f, new TextObject("{=jwgUwyKO}Add {PIECE_NAME} to {SHIP_NAME}", null).SetTextVariable("SHIP_NAME", this.GetShipNameConsideringRenames(base.SelectedFigureheads[n].Ship)).SetTextVariable("PIECE_NAME", figurehead.GetName()).ToString()));
				}
			}
			for (int num = 0; num < base.ShipsToSell.Count; num++)
			{
				list.Add(new PortChangeInfo((float)(-(float)base.ShipsToSell[num].Price), new TextObject("{=1Yaq0qy1}Sell {SHIP_NAME}", null).SetTextVariable("SHIP_NAME", base.ShipsToSell[num].Ship.Name).ToString()));
			}
			return list;
		}

		// Token: 0x06000272 RID: 626 RVA: 0x0000D7F8 File Offset: 0x0000B9F8
		private TextObject GetShipNameConsideringRenames(Ship ship)
		{
			TextObject textObject = ship.Name;
			if (base.ShipsToRename.Any<PortScreenHandler.ShipRenameInfo>((PortScreenHandler.ShipRenameInfo x) => x.Ship == ship))
			{
				textObject = new TextObject("{=!}" + base.ShipsToRename.First<PortScreenHandler.ShipRenameInfo>((PortScreenHandler.ShipRenameInfo x) => x.Ship == ship).NewName, null);
			}
			return textObject;
		}

		// Token: 0x040000CA RID: 202
		private readonly PartyBase _leftOwner;

		// Token: 0x040000CB RID: 203
		private readonly PartyBase _rightOwner;
	}
}
