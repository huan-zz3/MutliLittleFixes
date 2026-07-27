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
	// Token: 0x0200001A RID: 26
	public class PortScreenLootModeHandler : PortScreenHandler
	{
		// Token: 0x060001FC RID: 508 RVA: 0x0000B659 File Offset: 0x00009859
		public PortScreenLootModeHandler(TextObject leftSideName, PartyBase rightSide, MBReadOnlyList<Ship> initialLeftShips, MBReadOnlyList<Ship> initialRightShips)
			: base(initialLeftShips, initialRightShips)
		{
			this._leftSideName = leftSideName;
			this._rightSide = rightSide;
		}

		// Token: 0x060001FD RID: 509 RVA: 0x0000B672 File Offset: 0x00009872
		protected override PortActionInfo CanBuyShip(Ship ship)
		{
			return PortActionInfo.CreateValid(true, 0, GameTexts.FindText("str_take", null), null);
		}

		// Token: 0x060001FE RID: 510 RVA: 0x0000B688 File Offset: 0x00009888
		protected override PortActionInfo CanSellShip(Ship ship)
		{
			if (MobileParty.MainParty.IsCurrentlyAtSea && base.RightShips.Count == 1)
			{
				return PortActionInfo.CreateValid(false, 0, GameTexts.FindText("str_port_discard_ship", null), GameTexts.FindText("str_cannot_give_all_ships", null));
			}
			return PortActionInfo.CreateValid(true, 0, GameTexts.FindText("str_port_discard_ship", null), null);
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000B6E0 File Offset: 0x000098E0
		protected override PortActionInfo CanRenameShip(Ship ship)
		{
			return PortActionInfo.CreateValid(true, 0, GameTexts.FindText("str_port_rename_ship", null), TextObject.GetEmpty());
		}

		// Token: 0x06000200 RID: 512 RVA: 0x0000B6F9 File Offset: 0x000098F9
		protected override PortActionInfo CanRepairShip(Ship ship)
		{
			return PortActionInfo.CreateValid(false, 0, GameTexts.FindText("str_port_repair_ship", null), new TextObject("{=Pm6JbaXa}You can't repair ships outside a port.", null));
		}

		// Token: 0x06000201 RID: 513 RVA: 0x0000B718 File Offset: 0x00009918
		protected override PortActionInfo CanRepairAll()
		{
			return PortActionInfo.CreateValid(false, 0, GameTexts.FindText("str_port_repair_all_ships", null), new TextObject("{=Pm6JbaXa}You can't repair ships outside a port.", null));
		}

		// Token: 0x06000202 RID: 514 RVA: 0x0000B737 File Offset: 0x00009937
		protected override PortActionInfo CanUpgradeShip(Ship ship)
		{
			return PortActionInfo.CreateInvalid(new TextObject("{=4d7XLElL}You can't upgrade ships outside a port.", null));
		}

		// Token: 0x06000203 RID: 515 RVA: 0x0000B74C File Offset: 0x0000994C
		protected override PortActionInfo CanSendToClan(Ship ship)
		{
			int num = base.ShipsToSend.Count * Campaign.Current.Models.FleetManagementModel.MinimumTroopCountRequiredToSendShips;
			TextObject textObject;
			return PortActionInfo.CreateValid(Campaign.Current.Models.FleetManagementModel.CanSendShipToPlayerClan(ship, base.RightShips.Count, num, ref textObject), 0, GameTexts.FindText("str_port_send_ship_to_clan", null), textObject);
		}

		// Token: 0x06000204 RID: 516 RVA: 0x0000B7AF File Offset: 0x000099AF
		public override bool GetCanConfirm(out TextObject disabledHint)
		{
			disabledHint = null;
			return true;
		}

		// Token: 0x06000205 RID: 517 RVA: 0x0000B7B5 File Offset: 0x000099B5
		public override TextObject GetLeftRosterName()
		{
			return this._leftSideName;
		}

		// Token: 0x06000206 RID: 518 RVA: 0x0000B7BD File Offset: 0x000099BD
		public override PartyBase GetLeftSideOwnerParty()
		{
			return null;
		}

		// Token: 0x06000207 RID: 519 RVA: 0x0000B7C0 File Offset: 0x000099C0
		public override TextObject GetRightRosterName()
		{
			return this._rightSide.Name;
		}

		// Token: 0x06000208 RID: 520 RVA: 0x0000B7CD File Offset: 0x000099CD
		public override PartyBase GetRightSideOwnerParty()
		{
			return this._rightSide;
		}

		// Token: 0x06000209 RID: 521 RVA: 0x0000B7D5 File Offset: 0x000099D5
		public override int GetTradeCostOfShip(Ship ship, bool isSelling)
		{
			return 0;
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0000B7D8 File Offset: 0x000099D8
		public override int GetRepairCostOfShip(Ship ship, bool isRightSideRepairing)
		{
			return 0;
		}

		// Token: 0x0600020B RID: 523 RVA: 0x0000B7DB File Offset: 0x000099DB
		public override int GetUpgradeCostOfShip(Ship ship, ShipUpgradePiece piece, bool isRightSideUpgrading)
		{
			return 0;
		}

		// Token: 0x0600020C RID: 524 RVA: 0x0000B7DE File Offset: 0x000099DE
		public override int GetTotalGoldCost()
		{
			return 0;
		}

		// Token: 0x0600020D RID: 525 RVA: 0x0000B7E4 File Offset: 0x000099E4
		public override void OnConfirmChanges()
		{
			foreach (PortScreenHandler.ShipTradeInfo shipTradeInfo in base.ShipsToBuy)
			{
				ChangeShipOwnerAction.ApplyByLooting(PartyBase.MainParty, shipTradeInfo.Ship);
			}
			foreach (PortScreenHandler.ShipTradeInfo shipTradeInfo2 in base.ShipsToSell)
			{
				DestroyShipAction.ApplyByDiscard(shipTradeInfo2.Ship);
			}
			IFleetManagementCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<IFleetManagementCampaignBehavior>();
			for (int i = 0; i < base.ShipsToSend.Count; i++)
			{
				campaignBehavior.SendShipToClan(base.ShipsToSend[i], Clan.PlayerClan);
			}
			for (int j = 0; j < base.ShipsToRename.Count; j++)
			{
				PortScreenHandler.ShipRenameInfo shipRenameInfo = base.ShipsToRename[j];
				shipRenameInfo.Ship.SetName(new TextObject("{=!}" + shipRenameInfo.NewName, null));
			}
			if (MobileParty.MainParty.Ships.Count == 0 && MobileParty.MainParty.Anchor.IsValid)
			{
				MobileParty.MainParty.Anchor.ResetPosition();
			}
		}

		// Token: 0x0600020E RID: 526 RVA: 0x0000B93C File Offset: 0x00009B3C
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

		// Token: 0x0600020F RID: 527 RVA: 0x0000BAC4 File Offset: 0x00009CC4
		private TextObject GetShipNameConsideringRenames(Ship ship)
		{
			TextObject textObject = ship.Name;
			if (base.ShipsToRename.Any<PortScreenHandler.ShipRenameInfo>((PortScreenHandler.ShipRenameInfo x) => x.Ship == ship))
			{
				textObject = new TextObject("{=!}" + base.ShipsToRename.First<PortScreenHandler.ShipRenameInfo>((PortScreenHandler.ShipRenameInfo x) => x.Ship == ship).NewName, null);
			}
			return textObject;
		}

		// Token: 0x040000C1 RID: 193
		private readonly TextObject _leftSideName;

		// Token: 0x040000C2 RID: 194
		private readonly PartyBase _rightSide;
	}
}
