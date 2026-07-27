using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.ViewModelCollection.Port.PortScreenHandlers
{
	// Token: 0x02000019 RID: 25
	public abstract class PortScreenHandler
	{
		// Token: 0x17000082 RID: 130
		// (get) Token: 0x060001CC RID: 460 RVA: 0x0000ACFE File Offset: 0x00008EFE
		public MBReadOnlyList<Ship> LeftShips
		{
			get
			{
				return this._leftShips;
			}
		}

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x060001CD RID: 461 RVA: 0x0000AD06 File Offset: 0x00008F06
		public MBReadOnlyList<Ship> RightShips
		{
			get
			{
				return this._rightShips;
			}
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x060001CE RID: 462 RVA: 0x0000AD0E File Offset: 0x00008F0E
		public MBReadOnlyList<PortScreenHandler.ShipTradeInfo> ShipsToBuy
		{
			get
			{
				return this._shipsToBuy;
			}
		}

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x060001CF RID: 463 RVA: 0x0000AD16 File Offset: 0x00008F16
		public MBReadOnlyList<PortScreenHandler.ShipTradeInfo> ShipsToSell
		{
			get
			{
				return this._shipsToSell;
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x060001D0 RID: 464 RVA: 0x0000AD1E File Offset: 0x00008F1E
		public MBReadOnlyList<Ship> ShipsToRepair
		{
			get
			{
				return this._shipsToRepair;
			}
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x060001D1 RID: 465 RVA: 0x0000AD26 File Offset: 0x00008F26
		public MBReadOnlyList<Ship> ShipsToSend
		{
			get
			{
				return this._shipsToSend;
			}
		}

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x060001D2 RID: 466 RVA: 0x0000AD2E File Offset: 0x00008F2E
		public MBReadOnlyList<PortScreenHandler.ShipRenameInfo> ShipsToRename
		{
			get
			{
				return this._shipsToRename;
			}
		}

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x060001D3 RID: 467 RVA: 0x0000AD36 File Offset: 0x00008F36
		public MBReadOnlyList<PortScreenHandler.ShipUpgradePieceInfo> SelectedShipPieces
		{
			get
			{
				return this._selectedShipPieces;
			}
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x060001D4 RID: 468 RVA: 0x0000AD3E File Offset: 0x00008F3E
		public MBReadOnlyList<PortScreenHandler.ShipFigureheadInfo> SelectedFigureheads
		{
			get
			{
				return this._selectedFigureheads;
			}
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x0000AD48 File Offset: 0x00008F48
		public PortScreenHandler(MBReadOnlyList<Ship> initialLeftShips, MBReadOnlyList<Ship> initialRightShips)
		{
			this._initialLeftShips = initialLeftShips;
			this._initialRightShips = initialRightShips;
			this._leftShips = new MBList<Ship>(this._initialLeftShips);
			this._rightShips = new MBList<Ship>(this._initialRightShips);
			this._shipsToBuy = new MBList<PortScreenHandler.ShipTradeInfo>();
			this._shipsToSell = new MBList<PortScreenHandler.ShipTradeInfo>();
			this._shipsToRepair = new MBList<Ship>();
			this._shipsToRename = new MBList<PortScreenHandler.ShipRenameInfo>();
			this._shipsToSend = new MBList<Ship>();
			this._selectedShipPieces = new MBList<PortScreenHandler.ShipUpgradePieceInfo>();
			this._selectedFigureheads = new MBList<PortScreenHandler.ShipFigureheadInfo>();
		}

		// Token: 0x060001D6 RID: 470
		public abstract TextObject GetLeftRosterName();

		// Token: 0x060001D7 RID: 471
		public abstract TextObject GetRightRosterName();

		// Token: 0x060001D8 RID: 472
		public abstract PartyBase GetLeftSideOwnerParty();

		// Token: 0x060001D9 RID: 473
		public abstract PartyBase GetRightSideOwnerParty();

		// Token: 0x060001DA RID: 474 RVA: 0x0000ADD8 File Offset: 0x00008FD8
		public PortActionInfo GetCanBuyShip(Ship ship)
		{
			if (!this.LeftShips.Contains(ship))
			{
				return PortActionInfo.CreateInvalid(null);
			}
			PortActionInfo portActionInfo = this.CanBuyShip(ship);
			if (portActionInfo.IsRelevant && (!ship.IsTradeable || ship.IsUsedByQuest))
			{
				return PortActionInfo.CreateValid(false, 0, portActionInfo.ActionName, new TextObject("{=pWd0AQm8}You cannot buy this ship", null));
			}
			return portActionInfo;
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000AE34 File Offset: 0x00009034
		public PortActionInfo GetCanSellShip(Ship ship)
		{
			if (!this.RightShips.Contains(ship))
			{
				return PortActionInfo.CreateInvalid(null);
			}
			PortActionInfo portActionInfo = this.CanSellShip(ship);
			if (portActionInfo.IsRelevant && (!ship.IsTradeable || ship.IsUsedByQuest))
			{
				return PortActionInfo.CreateValid(false, 0, portActionInfo.ActionName, GameTexts.FindText("str_port_cant_take_action_quest_ship", null));
			}
			return portActionInfo;
		}

		// Token: 0x060001DC RID: 476 RVA: 0x0000AE90 File Offset: 0x00009090
		public PortActionInfo GetCanRepairShip(Ship ship)
		{
			if (!this.RightShips.Contains(ship) || ship.HitPoints >= ship.MaxHitPoints)
			{
				return PortActionInfo.CreateInvalid(null);
			}
			return this.CanRepairShip(ship);
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0000AEBC File Offset: 0x000090BC
		public PortActionInfo GetCanRepairAll(Ship selectedShip)
		{
			if (this.RightShips.Contains(selectedShip))
			{
				if (!this.RightShips.TrueForAll((Ship ship) => ship.HitPoints >= ship.MaxHitPoints))
				{
					return this.CanRepairAll();
				}
			}
			return PortActionInfo.CreateInvalid(null);
		}

		// Token: 0x060001DE RID: 478 RVA: 0x0000AF10 File Offset: 0x00009110
		public PortActionInfo GetCanUpgradeShip(Ship ship)
		{
			if (!this.RightShips.Contains(ship))
			{
				return PortActionInfo.CreateInvalid(new TextObject("{=hlBSanaL}You can't upgrade ships that don't belong to you", null));
			}
			if (ship.HitPoints < ship.MaxHitPoints && !this.ShipsToRepair.Contains(ship))
			{
				return PortActionInfo.CreateInvalid(new TextObject("{=8KEmXkaT}You can't upgrade ships that need repairs", null));
			}
			return this.CanUpgradeShip(ship);
		}

		// Token: 0x060001DF RID: 479 RVA: 0x0000AF70 File Offset: 0x00009170
		public PortActionInfo GetCanRenameShip(Ship ship)
		{
			if (!this.RightShips.Contains(ship))
			{
				return PortActionInfo.CreateInvalid(new TextObject("{=NmWkD50x}You can't rename ships that don't belong to you", null));
			}
			return this.CanRenameShip(ship);
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x0000AF98 File Offset: 0x00009198
		public PortActionInfo GetCanSendToClan(Ship ship)
		{
			if (!this.RightShips.Contains(ship))
			{
				return PortActionInfo.CreateInvalid(null);
			}
			PortActionInfo portActionInfo = this.CanSendToClan(ship);
			if (portActionInfo.IsRelevant && this.RightShips.Count == 1)
			{
				return PortActionInfo.CreateValid(false, 0, portActionInfo.ActionName, new TextObject("{=DSoB9VCu}You can't send your only ship to your clan", null));
			}
			return portActionInfo;
		}

		// Token: 0x060001E1 RID: 481
		public abstract int GetTradeCostOfShip(Ship ship, bool isRightSideSelling);

		// Token: 0x060001E2 RID: 482
		public abstract int GetRepairCostOfShip(Ship ship, bool isRightSideRepairing);

		// Token: 0x060001E3 RID: 483
		public abstract int GetUpgradeCostOfShip(Ship ship, ShipUpgradePiece piece, bool isRightSideUpgrading);

		// Token: 0x060001E4 RID: 484
		public abstract int GetTotalGoldCost();

		// Token: 0x060001E5 RID: 485
		public abstract bool GetCanConfirm(out TextObject disabledHint);

		// Token: 0x060001E6 RID: 486
		public abstract void OnConfirmChanges();

		// Token: 0x060001E7 RID: 487
		public abstract List<PortChangeInfo> GetChanges();

		// Token: 0x060001E8 RID: 488
		protected abstract PortActionInfo CanBuyShip(Ship ship);

		// Token: 0x060001E9 RID: 489
		protected abstract PortActionInfo CanSellShip(Ship ship);

		// Token: 0x060001EA RID: 490
		protected abstract PortActionInfo CanRepairShip(Ship ship);

		// Token: 0x060001EB RID: 491
		protected abstract PortActionInfo CanRepairAll();

		// Token: 0x060001EC RID: 492
		protected abstract PortActionInfo CanUpgradeShip(Ship ship);

		// Token: 0x060001ED RID: 493
		protected abstract PortActionInfo CanRenameShip(Ship ship);

		// Token: 0x060001EE RID: 494
		protected abstract PortActionInfo CanSendToClan(Ship ship);

		// Token: 0x060001EF RID: 495 RVA: 0x0000AFF4 File Offset: 0x000091F4
		public virtual bool AreThereAnyChanges()
		{
			return this.ShipsToBuy.Count > 0 || this.ShipsToSell.Count > 0 || this.ShipsToSend.Count > 0 || this.ShipsToRename.Count > 0 || this.ShipsToRepair.Count > 0 || this.SelectedShipPieces.Count > 0 || this.SelectedFigureheads.Count > 0;
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0000B068 File Offset: 0x00009268
		public void OnBuyShip(Ship ship)
		{
			bool flag = false;
			if (this._shipsToSell.Any<PortScreenHandler.ShipTradeInfo>((PortScreenHandler.ShipTradeInfo x) => x.Ship == ship))
			{
				flag = true;
				this._shipsToSell.RemoveAll((PortScreenHandler.ShipTradeInfo x) => x.Ship == ship);
			}
			else if (!this._shipsToBuy.Any<PortScreenHandler.ShipTradeInfo>((PortScreenHandler.ShipTradeInfo x) => x.Ship == ship))
			{
				this._shipsToBuy.Add(new PortScreenHandler.ShipTradeInfo(ship, this.GetTradeCostOfShip(ship, false)));
			}
			if (this._leftShips.Contains(ship))
			{
				this._leftShips.Remove(ship);
			}
			if (!this._rightShips.Contains(ship))
			{
				this._rightShips.Insert(0, ship);
			}
			this.ClearCurrentFigurehead(ship);
			if (flag)
			{
				this.ReequipPreviousFigurehead(ship);
			}
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x0000B15C File Offset: 0x0000935C
		public void OnSellShip(Ship ship)
		{
			this.OnResetShipName(ship);
			this.OnResetShipUpgrade(ship);
			bool flag = false;
			if (this._shipsToRepair.Contains(ship))
			{
				this._shipsToRepair.Remove(ship);
			}
			if (this._shipsToBuy.Any<PortScreenHandler.ShipTradeInfo>((PortScreenHandler.ShipTradeInfo x) => x.Ship == ship))
			{
				flag = true;
				this._shipsToBuy.RemoveAll((PortScreenHandler.ShipTradeInfo x) => x.Ship == ship);
			}
			else if (!this._shipsToSell.Any<PortScreenHandler.ShipTradeInfo>((PortScreenHandler.ShipTradeInfo x) => x.Ship == ship))
			{
				this._shipsToSell.Add(new PortScreenHandler.ShipTradeInfo(ship, this.GetTradeCostOfShip(ship, true)));
			}
			if (this._rightShips.Contains(ship))
			{
				this._rightShips.Remove(ship);
			}
			if (!this._leftShips.Contains(ship))
			{
				this._leftShips.Insert(0, ship);
			}
			if (!flag)
			{
				this.ClearCurrentFigurehead(ship);
			}
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x0000B27F File Offset: 0x0000947F
		public void OnRepairShip(Ship ship)
		{
			if (!this._shipsToRepair.Contains(ship))
			{
				this._shipsToRepair.Add(ship);
			}
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x0000B29B File Offset: 0x0000949B
		public void OnSendToClan(Ship ship)
		{
			if (!this._shipsToSend.Contains(ship))
			{
				this._shipsToSend.Add(ship);
				this._rightShips.Remove(ship);
			}
			this.ClearCurrentFigurehead(ship);
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x0000B2CC File Offset: 0x000094CC
		public void OnRenameShip(Ship ship, string newName)
		{
			bool flag = false;
			for (int i = 0; i < this._shipsToRename.Count; i++)
			{
				if (this._shipsToRename[i].Ship == ship)
				{
					flag = true;
					this._shipsToRename[i] = new PortScreenHandler.ShipRenameInfo(ship, newName);
					break;
				}
			}
			if (!flag)
			{
				this._shipsToRename.Add(new PortScreenHandler.ShipRenameInfo(ship, newName));
			}
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000B334 File Offset: 0x00009534
		public void OnResetShipName(Ship ship)
		{
			for (int i = this._shipsToRename.Count - 1; i >= 0; i--)
			{
				if (this._shipsToRename[i].Ship == ship)
				{
					this._shipsToRename.RemoveAt(i);
				}
			}
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x0000B37C File Offset: 0x0000957C
		public void OnResetShipUpgrade(Ship ship)
		{
			for (int i = this._selectedShipPieces.Count - 1; i >= 0; i--)
			{
				if (this._selectedShipPieces[i].Ship == ship)
				{
					this._selectedShipPieces.RemoveAt(i);
				}
			}
			for (int j = this._selectedFigureheads.Count - 1; j >= 0; j--)
			{
				if (this._selectedFigureheads[j].Ship == ship)
				{
					this._selectedFigureheads.RemoveAt(j);
				}
			}
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x0000B3FC File Offset: 0x000095FC
		public void OnUpgradePieceSelected(Ship ship, string shipSlotTag, ShipUpgradePiece piece)
		{
			bool flag = false;
			bool flag2 = ship.GetPieceAtSlot(shipSlotTag) == piece;
			int i = 0;
			while (i < this._selectedShipPieces.Count)
			{
				PortScreenHandler.ShipUpgradePieceInfo shipUpgradePieceInfo = this._selectedShipPieces[i];
				if (shipUpgradePieceInfo.Ship == ship && shipUpgradePieceInfo.ShipSlotTag == shipSlotTag)
				{
					flag = true;
					if (flag2)
					{
						this._selectedShipPieces.RemoveAt(i);
						break;
					}
					this._selectedShipPieces[i] = new PortScreenHandler.ShipUpgradePieceInfo(ship, shipSlotTag, piece);
					break;
				}
				else
				{
					i++;
				}
			}
			if (!flag && !flag2)
			{
				this._selectedShipPieces.Add(new PortScreenHandler.ShipUpgradePieceInfo(ship, shipSlotTag, piece));
			}
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x0000B494 File Offset: 0x00009694
		public void OnFigureheadSelected(Ship ship, Figurehead figurehead)
		{
			bool flag = false;
			bool flag2 = figurehead == ship.Figurehead;
			int i = 0;
			while (i < this._selectedFigureheads.Count)
			{
				if (this._selectedFigureheads[i].Ship == ship)
				{
					flag = true;
					if (flag2)
					{
						this._selectedFigureheads.RemoveAt(i);
						break;
					}
					this._selectedFigureheads[i] = new PortScreenHandler.ShipFigureheadInfo(ship, figurehead);
					break;
				}
				else
				{
					i++;
				}
			}
			if (!flag && !flag2)
			{
				this._selectedFigureheads.Add(new PortScreenHandler.ShipFigureheadInfo(ship, figurehead));
			}
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0000B518 File Offset: 0x00009718
		public void ResetChanges()
		{
			this._shipsToBuy.Clear();
			this._shipsToSell.Clear();
			this._shipsToRename.Clear();
			this._shipsToRepair.Clear();
			this._selectedShipPieces.Clear();
			this._selectedFigureheads.Clear();
			this._shipsToSend.Clear();
			this._leftShips.Clear();
			this._rightShips.Clear();
			this._leftShips.AddRange(this._initialLeftShips);
			this._rightShips.AddRange(this._initialRightShips);
		}

		// Token: 0x060001FA RID: 506 RVA: 0x0000B5AC File Offset: 0x000097AC
		private void ClearCurrentFigurehead(Ship ship)
		{
			Figurehead figurehead = ship.Figurehead;
			for (int i = 0; i < this._selectedFigureheads.Count; i++)
			{
				PortScreenHandler.ShipFigureheadInfo shipFigureheadInfo = this._selectedFigureheads[i];
				if (shipFigureheadInfo.Ship == ship)
				{
					figurehead = shipFigureheadInfo.Figurehead;
					break;
				}
			}
			if (figurehead != null)
			{
				this.OnFigureheadSelected(ship, null);
			}
		}

		// Token: 0x060001FB RID: 507 RVA: 0x0000B600 File Offset: 0x00009800
		private void ReequipPreviousFigurehead(Ship ship)
		{
			Figurehead figurehead = ship.Figurehead;
			bool flag = false;
			for (int i = 0; i < this._selectedFigureheads.Count; i++)
			{
				PortScreenHandler.ShipFigureheadInfo shipFigureheadInfo = this._selectedFigureheads[i];
				if (shipFigureheadInfo.Figurehead == figurehead && shipFigureheadInfo.Ship != null)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.OnFigureheadSelected(ship, figurehead);
			}
		}

		// Token: 0x040000B6 RID: 182
		protected MBReadOnlyList<Ship> _initialLeftShips;

		// Token: 0x040000B7 RID: 183
		protected MBReadOnlyList<Ship> _initialRightShips;

		// Token: 0x040000B8 RID: 184
		private MBList<Ship> _leftShips;

		// Token: 0x040000B9 RID: 185
		private MBList<Ship> _rightShips;

		// Token: 0x040000BA RID: 186
		private MBList<PortScreenHandler.ShipTradeInfo> _shipsToBuy;

		// Token: 0x040000BB RID: 187
		private MBList<PortScreenHandler.ShipTradeInfo> _shipsToSell;

		// Token: 0x040000BC RID: 188
		private MBList<Ship> _shipsToRepair;

		// Token: 0x040000BD RID: 189
		private MBList<Ship> _shipsToSend;

		// Token: 0x040000BE RID: 190
		private MBList<PortScreenHandler.ShipRenameInfo> _shipsToRename;

		// Token: 0x040000BF RID: 191
		private MBList<PortScreenHandler.ShipUpgradePieceInfo> _selectedShipPieces;

		// Token: 0x040000C0 RID: 192
		private MBList<PortScreenHandler.ShipFigureheadInfo> _selectedFigureheads;

		// Token: 0x02000054 RID: 84
		public readonly struct ShipUpgradePieceInfo
		{
			// Token: 0x060004A9 RID: 1193 RVA: 0x00014C29 File Offset: 0x00012E29
			public ShipUpgradePieceInfo(Ship ship, string shipSlotTag, ShipUpgradePiece piece)
			{
				this.Ship = ship;
				this.ShipSlotTag = shipSlotTag;
				this.Piece = piece;
			}

			// Token: 0x040001DC RID: 476
			public readonly Ship Ship;

			// Token: 0x040001DD RID: 477
			public readonly string ShipSlotTag;

			// Token: 0x040001DE RID: 478
			public readonly ShipUpgradePiece Piece;
		}

		// Token: 0x02000055 RID: 85
		public readonly struct ShipFigureheadInfo
		{
			// Token: 0x060004AA RID: 1194 RVA: 0x00014C40 File Offset: 0x00012E40
			public ShipFigureheadInfo(Ship ship, Figurehead figurehead)
			{
				this.Ship = ship;
				this.Figurehead = figurehead;
			}

			// Token: 0x040001DF RID: 479
			public readonly Ship Ship;

			// Token: 0x040001E0 RID: 480
			public readonly Figurehead Figurehead;
		}

		// Token: 0x02000056 RID: 86
		public readonly struct ShipRenameInfo
		{
			// Token: 0x060004AB RID: 1195 RVA: 0x00014C50 File Offset: 0x00012E50
			public ShipRenameInfo(Ship ship, string newName)
			{
				this.Ship = ship;
				this.NewName = newName;
			}

			// Token: 0x040001E1 RID: 481
			public readonly Ship Ship;

			// Token: 0x040001E2 RID: 482
			public readonly string NewName;
		}

		// Token: 0x02000057 RID: 87
		public readonly struct ShipTradeInfo
		{
			// Token: 0x060004AC RID: 1196 RVA: 0x00014C60 File Offset: 0x00012E60
			public ShipTradeInfo(Ship ship, int price)
			{
				this.Ship = ship;
				this.Price = price;
			}

			// Token: 0x040001E3 RID: 483
			public readonly Ship Ship;

			// Token: 0x040001E4 RID: 484
			public readonly int Price;
		}
	}
}
