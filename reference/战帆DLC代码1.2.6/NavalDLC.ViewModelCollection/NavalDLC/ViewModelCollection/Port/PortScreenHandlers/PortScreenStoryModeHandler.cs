using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace NavalDLC.ViewModelCollection.Port.PortScreenHandlers
{
	// Token: 0x0200001E RID: 30
	public class PortScreenStoryModeHandler : PortScreenHandler
	{
		// Token: 0x0600024C RID: 588 RVA: 0x0000C71A File Offset: 0x0000A91A
		public PortScreenStoryModeHandler(PartyBase leftParty, PartyBase rightParty)
			: base(leftParty.Ships, rightParty.Ships)
		{
			this._leftParty = leftParty;
			this._rightParty = rightParty;
		}

		// Token: 0x0600024D RID: 589 RVA: 0x0000C73C File Offset: 0x0000A93C
		public override TextObject GetLeftRosterName()
		{
			if (this._leftParty.IsSettlement)
			{
				return new TextObject("{=UeUkbDVz}Port of {SETTLEMENT}", null).SetTextVariable("SETTLEMENT", this._leftParty.Name);
			}
			return this._leftParty.Name;
		}

		// Token: 0x0600024E RID: 590 RVA: 0x0000C777 File Offset: 0x0000A977
		public override TextObject GetRightRosterName()
		{
			return this._rightParty.Name;
		}

		// Token: 0x0600024F RID: 591 RVA: 0x0000C784 File Offset: 0x0000A984
		public override PartyBase GetLeftSideOwnerParty()
		{
			return this._leftParty;
		}

		// Token: 0x06000250 RID: 592 RVA: 0x0000C78C File Offset: 0x0000A98C
		public override PartyBase GetRightSideOwnerParty()
		{
			return this._rightParty;
		}

		// Token: 0x06000251 RID: 593 RVA: 0x0000C794 File Offset: 0x0000A994
		public override int GetTradeCostOfShip(Ship ship, bool isRightSideSelling)
		{
			PartyBase partyBase = (isRightSideSelling ? this._rightParty : this._leftParty);
			PartyBase partyBase2 = (isRightSideSelling ? this._leftParty : this._rightParty);
			return (int)Campaign.Current.Models.ShipCostModel.GetShipTradeValue(ship, partyBase, partyBase2);
		}

		// Token: 0x06000252 RID: 594 RVA: 0x0000C7DD File Offset: 0x0000A9DD
		public override int GetRepairCostOfShip(Ship ship, bool isRightSideRepairing)
		{
			return 0;
		}

		// Token: 0x06000253 RID: 595 RVA: 0x0000C7E0 File Offset: 0x0000A9E0
		public override int GetUpgradeCostOfShip(Ship ship, ShipUpgradePiece piece, bool isRightSideUpgrading)
		{
			return 0;
		}

		// Token: 0x06000254 RID: 596 RVA: 0x0000C7E4 File Offset: 0x0000A9E4
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
			return num;
		}

		// Token: 0x06000255 RID: 597 RVA: 0x0000C848 File Offset: 0x0000AA48
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

		// Token: 0x06000256 RID: 598 RVA: 0x0000C870 File Offset: 0x0000AA70
		public override void OnConfirmChanges()
		{
			for (int i = 0; i < base.ShipsToBuy.Count; i++)
			{
				Ship ship = base.ShipsToBuy[i].Ship;
				ChangeShipOwnerAction.ApplyByTrade(this._rightParty, ship);
			}
			for (int j = 0; j < base.ShipsToSell.Count; j++)
			{
				Ship ship2 = base.ShipsToSell[j].Ship;
				ChangeShipOwnerAction.ApplyByTrade(this._leftParty, ship2);
			}
			if (MobileParty.MainParty.Ships.Count == 0 && MobileParty.MainParty.Anchor.IsValid)
			{
				MobileParty.MainParty.Anchor.ResetPosition();
				return;
			}
			if (MobileParty.MainParty.Ships.Count > 0 && !MobileParty.MainParty.Anchor.IsValid && this._leftParty.IsSettlement)
			{
				MobileParty.MainParty.Anchor.SetPosition(this._leftParty.Settlement.PortPosition);
			}
		}

		// Token: 0x06000257 RID: 599 RVA: 0x0000C968 File Offset: 0x0000AB68
		protected override PortActionInfo CanBuyShip(Ship ship)
		{
			bool flag = base.ShipsToSell.Any<PortScreenHandler.ShipTradeInfo>((PortScreenHandler.ShipTradeInfo x) => x.Ship == ship);
			int num = (flag ? base.ShipsToSell.FirstOrDefault<PortScreenHandler.ShipTradeInfo>((PortScreenHandler.ShipTradeInfo x) => x.Ship == ship).Price : this.GetTradeCostOfShip(ship, false));
			TextObject textObject = (flag ? GameTexts.FindText("str_port_buy_ship_back", null) : GameTexts.FindText("str_port_buy_ship", null));
			return PortActionInfo.CreateValid(true, num, textObject, TextObject.GetEmpty());
		}

		// Token: 0x06000258 RID: 600 RVA: 0x0000C9F0 File Offset: 0x0000ABF0
		protected override PortActionInfo CanSellShip(Ship ship)
		{
			bool flag = base.ShipsToBuy.Any<PortScreenHandler.ShipTradeInfo>((PortScreenHandler.ShipTradeInfo x) => x.Ship == ship);
			int num = (flag ? base.ShipsToBuy.FirstOrDefault<PortScreenHandler.ShipTradeInfo>((PortScreenHandler.ShipTradeInfo x) => x.Ship == ship).Price : this.GetTradeCostOfShip(ship, true));
			TextObject textObject = (flag ? GameTexts.FindText("str_port_sell_ship_back", null) : GameTexts.FindText("str_port_sell_ship", null));
			return PortActionInfo.CreateValid(true, num, textObject, TextObject.GetEmpty());
		}

		// Token: 0x06000259 RID: 601 RVA: 0x0000CA78 File Offset: 0x0000AC78
		protected override PortActionInfo CanRenameShip(Ship ship)
		{
			return PortActionInfo.CreateValid(false, 0, GameTexts.FindText("str_port_rename_ship", null), new TextObject("{=i6BBEAXI}You can't rename ships at this stage", null));
		}

		// Token: 0x0600025A RID: 602 RVA: 0x0000CA97 File Offset: 0x0000AC97
		protected override PortActionInfo CanRepairShip(Ship ship)
		{
			return PortActionInfo.CreateValid(false, 0, GameTexts.FindText("str_port_repair_ship", null), new TextObject("{=HqraYjwT}You can't repair ships at this stage", null));
		}

		// Token: 0x0600025B RID: 603 RVA: 0x0000CAB6 File Offset: 0x0000ACB6
		protected override PortActionInfo CanRepairAll()
		{
			return PortActionInfo.CreateValid(false, 0, GameTexts.FindText("str_port_repair_all_ships", null), new TextObject("{=HqraYjwT}You can't repair ships at this stage", null));
		}

		// Token: 0x0600025C RID: 604 RVA: 0x0000CAD5 File Offset: 0x0000ACD5
		protected override PortActionInfo CanUpgradeShip(Ship ship)
		{
			return PortActionInfo.CreateValid(false, 0, GameTexts.FindText("str_port_upgrade_ship", null), new TextObject("{=b3eIbvr0}You can't upgrade ships at this stage", null));
		}

		// Token: 0x0600025D RID: 605 RVA: 0x0000CAF4 File Offset: 0x0000ACF4
		protected override PortActionInfo CanSendToClan(Ship ship)
		{
			return PortActionInfo.CreateInvalid(null);
		}

		// Token: 0x0600025E RID: 606 RVA: 0x0000CAFC File Offset: 0x0000ACFC
		public override List<PortChangeInfo> GetChanges()
		{
			List<PortChangeInfo> list = new List<PortChangeInfo>();
			for (int i = 0; i < base.ShipsToBuy.Count; i++)
			{
				list.Add(new PortChangeInfo((float)base.ShipsToBuy[i].Price, new TextObject("{=9AIOcUuH}Buy {SHIP_NAME}", null).SetTextVariable("SHIP_NAME", base.ShipsToBuy[i].Ship.Name).ToString()));
			}
			for (int j = 0; j < base.ShipsToSell.Count; j++)
			{
				list.Add(new PortChangeInfo((float)base.ShipsToSell[j].Price, new TextObject("{=1Yaq0qy1}Sell {SHIP_NAME}", null).SetTextVariable("SHIP_NAME", base.ShipsToSell[j].Ship.Name).ToString()));
			}
			return list;
		}

		// Token: 0x040000C8 RID: 200
		private readonly PartyBase _leftParty;

		// Token: 0x040000C9 RID: 201
		private readonly PartyBase _rightParty;
	}
}
