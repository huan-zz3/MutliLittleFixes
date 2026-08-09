using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.CampaignBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.ViewModelCollection.Port.PortScreenHandlers
{
	// Token: 0x0200001B RID: 27
	public class PortScreenManageFleetModeHandler : PortScreenHandler
	{
		// Token: 0x06000210 RID: 528 RVA: 0x0000BB31 File Offset: 0x00009D31
		public PortScreenManageFleetModeHandler(TextObject leftSideName, PartyBase rightSide, MBReadOnlyList<Ship> initialLeftShips, MBReadOnlyList<Ship> initialRightShips)
			: base(initialLeftShips, initialRightShips)
		{
			this._leftSideName = leftSideName;
			this._rightSide = rightSide;
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0000BB4A File Offset: 0x00009D4A
		public override bool GetCanConfirm(out TextObject disabledHint)
		{
			disabledHint = null;
			return true;
		}

		// Token: 0x06000212 RID: 530 RVA: 0x0000BB50 File Offset: 0x00009D50
		public override PartyBase GetLeftSideOwnerParty()
		{
			return null;
		}

		// Token: 0x06000213 RID: 531 RVA: 0x0000BB53 File Offset: 0x00009D53
		public override PartyBase GetRightSideOwnerParty()
		{
			return this._rightSide;
		}

		// Token: 0x06000214 RID: 532 RVA: 0x0000BB5B File Offset: 0x00009D5B
		public override TextObject GetLeftRosterName()
		{
			return this._leftSideName;
		}

		// Token: 0x06000215 RID: 533 RVA: 0x0000BB63 File Offset: 0x00009D63
		public override TextObject GetRightRosterName()
		{
			return this._rightSide.Name;
		}

		// Token: 0x06000216 RID: 534 RVA: 0x0000BB70 File Offset: 0x00009D70
		public override int GetTradeCostOfShip(Ship ship, bool isSelling)
		{
			return 0;
		}

		// Token: 0x06000217 RID: 535 RVA: 0x0000BB73 File Offset: 0x00009D73
		public override int GetRepairCostOfShip(Ship ship, bool isRightSideRepairing)
		{
			return 0;
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000BB76 File Offset: 0x00009D76
		public override int GetUpgradeCostOfShip(Ship ship, ShipUpgradePiece piece, bool isRightSideUpgrading)
		{
			return 0;
		}

		// Token: 0x06000219 RID: 537 RVA: 0x0000BB79 File Offset: 0x00009D79
		public override int GetTotalGoldCost()
		{
			return 0;
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0000BB7C File Offset: 0x00009D7C
		public override void OnConfirmChanges()
		{
			for (int i = 0; i < base.ShipsToSell.Count; i++)
			{
				DestroyShipAction.ApplyByDiscard(base.ShipsToSell[i].Ship);
			}
			for (int j = 0; j < base.ShipsToBuy.Count; j++)
			{
				PortScreenHandler.ShipTradeInfo shipTradeInfo = base.ShipsToBuy[j];
				ChangeShipOwnerAction.ApplyByTransferring(this._rightSide, shipTradeInfo.Ship);
			}
			for (int k = 0; k < base.ShipsToRename.Count; k++)
			{
				PortScreenHandler.ShipRenameInfo shipRenameInfo = base.ShipsToRename[k];
				shipRenameInfo.Ship.SetName(new TextObject("{=!}" + shipRenameInfo.NewName, null));
			}
			IFleetManagementCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<IFleetManagementCampaignBehavior>();
			for (int l = 0; l < base.ShipsToSend.Count; l++)
			{
				campaignBehavior.SendShipToClan(base.ShipsToSend[l], Clan.PlayerClan);
			}
			if (MobileParty.MainParty.Ships.Count == 0 && MobileParty.MainParty.Anchor.IsValid)
			{
				MobileParty.MainParty.Anchor.ResetPosition();
			}
		}

		// Token: 0x0600021B RID: 539 RVA: 0x0000BCA4 File Offset: 0x00009EA4
		protected override PortActionInfo CanBuyShip(Ship ship)
		{
			if (base.ShipsToSell.Any<PortScreenHandler.ShipTradeInfo>((PortScreenHandler.ShipTradeInfo x) => x.Ship == ship))
			{
				return PortActionInfo.CreateValid(true, 0, GameTexts.FindText("str_take_ship_back", null), null);
			}
			return PortActionInfo.CreateValid(true, 0, GameTexts.FindText("str_take", null), null);
		}

		// Token: 0x0600021C RID: 540 RVA: 0x0000BD00 File Offset: 0x00009F00
		protected override PortActionInfo CanSellShip(Ship ship)
		{
			if (MobileParty.MainParty.IsCurrentlyAtSea && base.RightShips.Count == 1)
			{
				return PortActionInfo.CreateValid(false, 0, GameTexts.FindText("str_port_discard_ship", null), GameTexts.FindText("str_cannot_give_all_ships", null));
			}
			return PortActionInfo.CreateValid(true, 0, GameTexts.FindText("str_port_discard_ship", null), null);
		}

		// Token: 0x0600021D RID: 541 RVA: 0x0000BD58 File Offset: 0x00009F58
		protected override PortActionInfo CanUpgradeShip(Ship ship)
		{
			return PortActionInfo.CreateInvalid(new TextObject("{=4d7XLElL}You can't upgrade ships outside a port.", null));
		}

		// Token: 0x0600021E RID: 542 RVA: 0x0000BD6A File Offset: 0x00009F6A
		protected override PortActionInfo CanRenameShip(Ship ship)
		{
			return PortActionInfo.CreateValid(true, 0, GameTexts.FindText("str_port_rename_ship", null), TextObject.GetEmpty());
		}

		// Token: 0x0600021F RID: 543 RVA: 0x0000BD83 File Offset: 0x00009F83
		protected override PortActionInfo CanRepairShip(Ship ship)
		{
			return PortActionInfo.CreateValid(false, 0, GameTexts.FindText("str_port_repair_ship", null), new TextObject("{=Pm6JbaXa}You can't repair ships outside a port.", null));
		}

		// Token: 0x06000220 RID: 544 RVA: 0x0000BDA4 File Offset: 0x00009FA4
		protected override PortActionInfo CanSendToClan(Ship ship)
		{
			int num = base.ShipsToSend.Count * Campaign.Current.Models.FleetManagementModel.MinimumTroopCountRequiredToSendShips;
			TextObject textObject;
			return PortActionInfo.CreateValid(Campaign.Current.Models.FleetManagementModel.CanSendShipToPlayerClan(ship, base.RightShips.Count, num, ref textObject), 0, GameTexts.FindText("str_port_send_ship_to_clan", null), textObject);
		}

		// Token: 0x06000221 RID: 545 RVA: 0x0000BE07 File Offset: 0x0000A007
		protected override PortActionInfo CanRepairAll()
		{
			return PortActionInfo.CreateValid(false, 0, GameTexts.FindText("str_port_repair_all_ships", null), new TextObject("{=Pm6JbaXa}You can't repair ships outside a port.", null));
		}

		// Token: 0x06000222 RID: 546 RVA: 0x0000BE28 File Offset: 0x0000A028
		public override List<PortChangeInfo> GetChanges()
		{
			List<PortChangeInfo> list = new List<PortChangeInfo>();
			for (int i = 0; i < base.ShipsToBuy.Count; i++)
			{
				list.Add(new PortChangeInfo(0f, new TextObject("{=TsQzdjvd}Take {SHIP_NAME}", null).SetTextVariable("SHIP_NAME", base.ShipsToBuy[i].Ship.Name).ToString()));
			}
			for (int j = 0; j < base.ShipsToSell.Count; j++)
			{
				list.Add(new PortChangeInfo(0f, new TextObject("{=cItrQpwh}Discard {SHIP_NAME}", null).SetTextVariable("SHIP_NAME", base.ShipsToSell[j].Ship.Name).ToString()));
			}
			for (int k = 0; k < base.ShipsToRename.Count; k++)
			{
				list.Add(new PortChangeInfo(0f, new TextObject("{=Fidoxgd1}Rename {SHIP_NAME} to {NEW_SHIP_NAME}", null).SetTextVariable("SHIP_NAME", base.ShipsToRename[k].Ship.Name).SetTextVariable("NEW_SHIP_NAME", base.ShipsToRename[k].NewName).ToString()));
			}
			for (int l = 0; l < base.ShipsToSend.Count; l++)
			{
				list.Add(new PortChangeInfo(0f, new TextObject("{=L1x30kUJ}Send {SHIP_NAME} to clan", null).SetTextVariable("SHIP_NAME", this.GetShipNameConsideringRenames(base.ShipsToSend[l])).ToString()));
			}
			return list;
		}

		// Token: 0x06000223 RID: 547 RVA: 0x0000BFB0 File Offset: 0x0000A1B0
		private TextObject GetShipNameConsideringRenames(Ship ship)
		{
			TextObject textObject = ship.Name;
			if (base.ShipsToRename.Any<PortScreenHandler.ShipRenameInfo>((PortScreenHandler.ShipRenameInfo x) => x.Ship == ship))
			{
				textObject = new TextObject("{=!}" + base.ShipsToRename.First<PortScreenHandler.ShipRenameInfo>((PortScreenHandler.ShipRenameInfo x) => x.Ship == ship).NewName, null);
			}
			return textObject;
		}

		// Token: 0x040000C3 RID: 195
		private readonly TextObject _leftSideName;

		// Token: 0x040000C4 RID: 196
		private readonly PartyBase _rightSide;
	}
}
