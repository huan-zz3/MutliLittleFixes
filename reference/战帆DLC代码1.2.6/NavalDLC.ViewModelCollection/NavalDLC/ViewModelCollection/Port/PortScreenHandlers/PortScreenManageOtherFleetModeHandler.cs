using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace NavalDLC.ViewModelCollection.Port.PortScreenHandlers
{
	// Token: 0x0200001C RID: 28
	public class PortScreenManageOtherFleetModeHandler : PortScreenHandler
	{
		// Token: 0x06000224 RID: 548 RVA: 0x0000C01D File Offset: 0x0000A21D
		public PortScreenManageOtherFleetModeHandler(PartyBase other)
			: base(other.Ships, MobileParty.MainParty.Ships)
		{
			this._other = other;
		}

		// Token: 0x06000225 RID: 549 RVA: 0x0000C03C File Offset: 0x0000A23C
		public override bool GetCanConfirm(out TextObject disabledHint)
		{
			disabledHint = null;
			return true;
		}

		// Token: 0x06000226 RID: 550 RVA: 0x0000C042 File Offset: 0x0000A242
		public override PartyBase GetLeftSideOwnerParty()
		{
			return this._other;
		}

		// Token: 0x06000227 RID: 551 RVA: 0x0000C04A File Offset: 0x0000A24A
		public override PartyBase GetRightSideOwnerParty()
		{
			return MobileParty.MainParty.Party;
		}

		// Token: 0x06000228 RID: 552 RVA: 0x0000C056 File Offset: 0x0000A256
		public override TextObject GetLeftRosterName()
		{
			if (this._other.IsSettlement)
			{
				return new TextObject("{=UeUkbDVz}Port of {SETTLEMENT}", null).SetTextVariable("SETTLEMENT", this._other.Name);
			}
			return this._other.Name;
		}

		// Token: 0x06000229 RID: 553 RVA: 0x0000C091 File Offset: 0x0000A291
		public override int GetTradeCostOfShip(Ship ship, bool isRightSideSelling)
		{
			return 0;
		}

		// Token: 0x0600022A RID: 554 RVA: 0x0000C094 File Offset: 0x0000A294
		public override int GetRepairCostOfShip(Ship ship, bool isRightSideRepairing)
		{
			return 0;
		}

		// Token: 0x0600022B RID: 555 RVA: 0x0000C097 File Offset: 0x0000A297
		public override int GetUpgradeCostOfShip(Ship ship, ShipUpgradePiece piece, bool isRightSideUpgrading)
		{
			return 0;
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0000C09A File Offset: 0x0000A29A
		public override TextObject GetRightRosterName()
		{
			return MobileParty.MainParty.Name;
		}

		// Token: 0x0600022D RID: 557 RVA: 0x0000C0A6 File Offset: 0x0000A2A6
		public override int GetTotalGoldCost()
		{
			return 0;
		}

		// Token: 0x0600022E RID: 558 RVA: 0x0000C0AC File Offset: 0x0000A2AC
		public override void OnConfirmChanges()
		{
			for (int i = 0; i < base.ShipsToBuy.Count; i++)
			{
				Ship ship = base.ShipsToBuy[i].Ship;
				ChangeShipOwnerAction.ApplyByTransferring(MobileParty.MainParty.Party, ship);
			}
			for (int j = 0; j < base.ShipsToSell.Count; j++)
			{
				Ship ship2 = base.ShipsToSell[j].Ship;
				ChangeShipOwnerAction.ApplyByTransferring(this._other, ship2);
			}
			if (MobileParty.MainParty.Ships.Count == 0 && MobileParty.MainParty.Anchor.IsValid)
			{
				MobileParty.MainParty.Anchor.ResetPosition();
			}
			if (this._other.Ships.Count == 0 && this._other.IsMobile && this._other.MobileParty.Anchor.IsValid)
			{
				this._other.MobileParty.Anchor.ResetPosition();
			}
			for (int k = 0; k < base.ShipsToRename.Count; k++)
			{
				PortScreenHandler.ShipRenameInfo shipRenameInfo = base.ShipsToRename[k];
				shipRenameInfo.Ship.SetName(new TextObject("{=!}" + shipRenameInfo.NewName, null));
			}
		}

		// Token: 0x0600022F RID: 559 RVA: 0x0000C1F0 File Offset: 0x0000A3F0
		protected override PortActionInfo CanBuyShip(Ship ship)
		{
			TextObject textObject = (base.ShipsToSell.Any<PortScreenHandler.ShipTradeInfo>((PortScreenHandler.ShipTradeInfo x) => x.Ship == ship) ? GameTexts.FindText("str_take_ship_back", null) : GameTexts.FindText("str_take", null));
			TextObject textObject2;
			return PortActionInfo.CreateValid(this.CanBuyShip(ship, out textObject2), 0, textObject, textObject2);
		}

		// Token: 0x06000230 RID: 560 RVA: 0x0000C254 File Offset: 0x0000A454
		protected override PortActionInfo CanSellShip(Ship ship)
		{
			TextObject textObject = (base.ShipsToBuy.Any<PortScreenHandler.ShipTradeInfo>((PortScreenHandler.ShipTradeInfo x) => x.Ship == ship) ? GameTexts.FindText("str_give_ship_back", null) : GameTexts.FindText("str_give", null));
			TextObject textObject2;
			return PortActionInfo.CreateValid(this.CanSellShip(ship, out textObject2), 0, textObject, textObject2);
		}

		// Token: 0x06000231 RID: 561 RVA: 0x0000C2B6 File Offset: 0x0000A4B6
		protected override PortActionInfo CanUpgradeShip(Ship ship)
		{
			return PortActionInfo.CreateInvalid(new TextObject("{=4d7XLElL}You can't upgrade ships outside a port.", null));
		}

		// Token: 0x06000232 RID: 562 RVA: 0x0000C2C8 File Offset: 0x0000A4C8
		protected override PortActionInfo CanRenameShip(Ship ship)
		{
			return PortActionInfo.CreateValid(true, 0, GameTexts.FindText("str_port_rename_ship", null), TextObject.GetEmpty());
		}

		// Token: 0x06000233 RID: 563 RVA: 0x0000C2E1 File Offset: 0x0000A4E1
		protected override PortActionInfo CanRepairShip(Ship ship)
		{
			return PortActionInfo.CreateValid(false, 0, GameTexts.FindText("str_port_repair_ship", null), new TextObject("{=Pm6JbaXa}You can't repair ships outside a port.", null));
		}

		// Token: 0x06000234 RID: 564 RVA: 0x0000C300 File Offset: 0x0000A500
		protected override PortActionInfo CanRepairAll()
		{
			return PortActionInfo.CreateValid(false, 0, GameTexts.FindText("str_port_repair_all_ships", null), new TextObject("{=Pm6JbaXa}You can't repair ships outside a port.", null));
		}

		// Token: 0x06000235 RID: 565 RVA: 0x0000C31F File Offset: 0x0000A51F
		protected override PortActionInfo CanSendToClan(Ship ship)
		{
			return PortActionInfo.CreateInvalid(null);
		}

		// Token: 0x06000236 RID: 566 RVA: 0x0000C328 File Offset: 0x0000A528
		private bool CanSellShip(Ship ship, out TextObject disabledHint)
		{
			disabledHint = TextObject.GetEmpty();
			if (base.ShipsToSell.Any<PortScreenHandler.ShipTradeInfo>((PortScreenHandler.ShipTradeInfo x) => x.Ship == ship))
			{
				return false;
			}
			if (MobileParty.MainParty.IsCurrentlyAtSea && base.RightShips.Count == 1)
			{
				disabledHint = GameTexts.FindText("str_cannot_give_all_ships", null);
				return false;
			}
			return true;
		}

		// Token: 0x06000237 RID: 567 RVA: 0x0000C390 File Offset: 0x0000A590
		private bool CanBuyShip(Ship ship, out TextObject disabledHint)
		{
			disabledHint = TextObject.GetEmpty();
			if (base.ShipsToBuy.Any<PortScreenHandler.ShipTradeInfo>((PortScreenHandler.ShipTradeInfo x) => x.Ship == ship))
			{
				return false;
			}
			if (this._other.MobileParty.IsCurrentlyAtSea && this._other.Ships.Count + base.ShipsToSell.Count - base.ShipsToBuy.Count <= 1)
			{
				disabledHint = GameTexts.FindText("str_cannot_take_all_ships", null);
				return false;
			}
			return true;
		}

		// Token: 0x06000238 RID: 568 RVA: 0x0000C41C File Offset: 0x0000A61C
		public override List<PortChangeInfo> GetChanges()
		{
			List<PortChangeInfo> list = new List<PortChangeInfo>();
			for (int i = 0; i < base.ShipsToBuy.Count; i++)
			{
				list.Add(new PortChangeInfo(0f, new TextObject("{=TsQzdjvd}Take {SHIP_NAME}", null).SetTextVariable("SHIP_NAME", base.ShipsToBuy[i].Ship.Name).ToString()));
			}
			for (int j = 0; j < base.ShipsToSell.Count; j++)
			{
				list.Add(new PortChangeInfo(0f, new TextObject("{=LZsY5SyD}Give {SHIP_NAME}", null).SetTextVariable("SHIP_NAME", base.ShipsToSell[j].Ship.Name).ToString()));
			}
			for (int k = 0; k < base.ShipsToRename.Count; k++)
			{
				list.Add(new PortChangeInfo(0f, new TextObject("{=Fidoxgd1}Rename {SHIP_NAME} to {NEW_SHIP_NAME}", null).SetTextVariable("SHIP_NAME", base.ShipsToRename[k].Ship.Name).SetTextVariable("NEW_SHIP_NAME", base.ShipsToRename[k].NewName).ToString()));
			}
			return list;
		}

		// Token: 0x040000C5 RID: 197
		private readonly PartyBase _other;
	}
}
