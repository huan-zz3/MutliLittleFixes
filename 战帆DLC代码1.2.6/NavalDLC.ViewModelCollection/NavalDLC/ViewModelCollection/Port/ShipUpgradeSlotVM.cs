using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.ViewModelCollection.Port
{
	// Token: 0x02000016 RID: 22
	public class ShipUpgradeSlotVM : ShipUpgradeSlotBaseVM
	{
		// Token: 0x060001C2 RID: 450 RVA: 0x0000A970 File Offset: 0x00008B70
		public ShipUpgradeSlotVM(Ship ship, TextObject slotName, string shipSlotTag, string slotTypeId, Action<ShipUpgradeSlotBaseVM> onSelected)
			: base(ship, slotName, shipSlotTag, slotTypeId, onSelected)
		{
			this._initialSelectedPiece = this.Ship.GetPieceAtSlot(this.ShipSlotTag);
			List<ShipUpgradePiece> list = (from x in MBObjectManager.Instance.GetObjectTypeList<ShipUpgradePiece>()
				where !x.NotMerchandise && x.DoesPieceMatchSlot(this.Ship.ShipHull.AvailableSlots[this.ShipSlotTag])
				select x).ToList<ShipUpgradePiece>();
			List<ShipUpgradePiece> list2 = new List<ShipUpgradePiece>();
			Settlement currentSettlement = Settlement.CurrentSettlement;
			if (((currentSettlement != null) ? currentSettlement.Town : null) != null)
			{
				list2 = (from x in Settlement.CurrentSettlement.Town.GetAvailableShipUpgradePieces()
					where x.DoesPieceMatchSlot(this.Ship.ShipHull.AvailableSlots[this.ShipSlotTag])
					select x).ToList<ShipUpgradePiece>();
			}
			if (this._initialSelectedPiece != null && !list.Contains(this._initialSelectedPiece))
			{
				list.Add(this._initialSelectedPiece);
			}
			if (this.Ship.UnlockedUpgradePieces != null)
			{
				foreach (ShipUpgradePiece shipUpgradePiece in this.Ship.UnlockedUpgradePieces)
				{
					if (shipUpgradePiece.DoesPieceMatchSlot(this.Ship.ShipHull.AvailableSlots[this.ShipSlotTag]) && !list.Contains(shipUpgradePiece))
					{
						list.Add(shipUpgradePiece);
					}
				}
			}
			if (this._initialSelectedPiece != null && !list2.Contains(this._initialSelectedPiece))
			{
				list2.Add(this._initialSelectedPiece);
			}
			if (this.Ship.UnlockedUpgradePieces != null)
			{
				foreach (ShipUpgradePiece shipUpgradePiece2 in this.Ship.UnlockedUpgradePieces)
				{
					if (shipUpgradePiece2.DoesPieceMatchSlot(this.Ship.ShipHull.AvailableSlots[this.ShipSlotTag]) && !list2.Contains(shipUpgradePiece2))
					{
						list2.Add(shipUpgradePiece2);
					}
				}
			}
			ShipSlot shipSlot = this.Ship.ShipHull.AvailableSlots[this.ShipSlotTag];
			for (int i = 0; i < list.Count; i++)
			{
				ShipUpgradePiece shipUpgradePiece3 = list[i];
				ShipUpgradePieceVM shipUpgradePieceVM = new ShipUpgradePieceVM(shipUpgradePiece3, this.Ship, new Action<ShipUpgradePieceBaseVM>(this.OnPieceSelected))
				{
					IsDisabled = !list2.Contains(shipUpgradePiece3)
				};
				base.AvailablePieces.Add(shipUpgradePieceVM);
				if (shipUpgradePiece3 == this._initialSelectedPiece)
				{
					base.SelectedPiece = shipUpgradePieceVM;
				}
			}
			base.AvailablePieces.Sort(new ShipUpgradeSlotBaseVM.UpgradePieceComparer());
			base.UpdateAnyBetterPiecesAvailable();
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x0000ABEC File Offset: 0x00008DEC
		public override void ResetPieces()
		{
			base.SelectedPiece = base.AvailablePieces.FirstOrDefault<ShipUpgradePieceBaseVM>(delegate(ShipUpgradePieceBaseVM x)
			{
				ShipUpgradePieceVM shipUpgradePieceVM = x as ShipUpgradePieceVM;
				return ((shipUpgradePieceVM != null) ? shipUpgradePieceVM.Piece : null) == this._initialSelectedPiece;
			});
			base.IsChanged = false;
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x0000AC12 File Offset: 0x00008E12
		protected override bool GetIsChanged()
		{
			ShipUpgradePieceVM shipUpgradePieceVM = base.SelectedPiece as ShipUpgradePieceVM;
			return ((shipUpgradePieceVM != null) ? shipUpgradePieceVM.Piece : null) != this._initialSelectedPiece;
		}

		// Token: 0x040000AE RID: 174
		private readonly ShipUpgradePiece _initialSelectedPiece;
	}
}
