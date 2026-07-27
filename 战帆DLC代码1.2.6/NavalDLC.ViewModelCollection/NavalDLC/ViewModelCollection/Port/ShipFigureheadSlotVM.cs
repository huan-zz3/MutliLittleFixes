using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.ViewModelCollection.Port
{
	// Token: 0x0200000C RID: 12
	public class ShipFigureheadSlotVM : ShipUpgradeSlotBaseVM
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x060000C6 RID: 198 RVA: 0x000077F4 File Offset: 0x000059F4
		// (remove) Token: 0x060000C7 RID: 199 RVA: 0x00007828 File Offset: 0x00005A28
		public static event ShipFigureheadSlotVM.GetCurrentFigureheadDelegate GetCurrentFigurehead;

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x060000C8 RID: 200 RVA: 0x0000785C File Offset: 0x00005A5C
		// (remove) Token: 0x060000C9 RID: 201 RVA: 0x00007890 File Offset: 0x00005A90
		public static event ShipFigureheadSlotVM.GetShipOfFigureheadDelegate GetShipOfFigurehead;

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x060000CA RID: 202 RVA: 0x000078C4 File Offset: 0x00005AC4
		// (remove) Token: 0x060000CB RID: 203 RVA: 0x000078F8 File Offset: 0x00005AF8
		public static event ShipFigureheadSlotVM.GetIsRightSideDelegate GetIsRightSide;

		// Token: 0x060000CC RID: 204 RVA: 0x0000792C File Offset: 0x00005B2C
		public ShipFigureheadSlotVM(Ship ship, TextObject slotName, string shipSlotTag, string slotTypeId, Action<ShipUpgradeSlotBaseVM> onSelected)
			: base(ship, slotName, shipSlotTag, slotTypeId, onSelected)
		{
			this._initialSelectedFigurehead = this.Ship.Figurehead;
			List<Figurehead> list = MBObjectManager.Instance.GetObjectTypeList<Figurehead>().ToList<Figurehead>();
			List<Figurehead> unlockedFigureheadsByMainHero = Campaign.Current.UnlockedFigureheadsByMainHero;
			this._enabledFigureheads = ((unlockedFigureheadsByMainHero != null) ? unlockedFigureheadsByMainHero.ToList<Figurehead>() : null) ?? new List<Figurehead>();
			if (this._initialSelectedFigurehead != null && !list.Contains(this._initialSelectedFigurehead))
			{
				list.Add(this._initialSelectedFigurehead);
			}
			foreach (Figurehead figurehead in this._enabledFigureheads)
			{
				if (!list.Contains(figurehead))
				{
					list.Add(figurehead);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				Figurehead figurehead2 = list[i];
				ShipFigureheadVM shipFigureheadVM = new ShipFigureheadVM(figurehead2, new Action<ShipUpgradePieceBaseVM>(this.OnPieceSelected))
				{
					IsDisabled = !this._enabledFigureheads.Contains(figurehead2)
				};
				base.AvailablePieces.Add(shipFigureheadVM);
				if (figurehead2 == this._initialSelectedFigurehead)
				{
					base.SelectedPiece = shipFigureheadVM;
				}
			}
			base.AvailablePieces.Sort(new ShipUpgradeSlotBaseVM.UpgradePieceComparer());
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00007A74 File Offset: 0x00005C74
		public override void ResetPieces()
		{
			this.UpdateAvailableFigureheads();
			base.IsChanged = false;
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00007A83 File Offset: 0x00005C83
		protected override bool GetIsChanged()
		{
			ShipFigureheadVM shipFigureheadVM = base.SelectedPiece as ShipFigureheadVM;
			return ((shipFigureheadVM != null) ? shipFigureheadVM.Figurehead : null) != this._initialSelectedFigurehead;
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00007AA8 File Offset: 0x00005CA8
		public void UpdateAvailableFigureheads()
		{
			ShipFigureheadSlotVM.<>c__DisplayClass17_0 CS$<>8__locals1 = new ShipFigureheadSlotVM.<>c__DisplayClass17_0();
			ShipFigureheadSlotVM.<>c__DisplayClass17_0 CS$<>8__locals2 = CS$<>8__locals1;
			ShipFigureheadSlotVM.GetCurrentFigureheadDelegate getCurrentFigurehead = ShipFigureheadSlotVM.GetCurrentFigurehead;
			CS$<>8__locals2.currentFigurehead = ((getCurrentFigurehead != null) ? getCurrentFigurehead(this.Ship) : null);
			base.SelectedPiece = base.AvailablePieces.FirstOrDefault<ShipUpgradePieceBaseVM>(delegate(ShipUpgradePieceBaseVM x)
			{
				ShipFigureheadVM shipFigureheadVM3 = x as ShipFigureheadVM;
				return ((shipFigureheadVM3 != null) ? shipFigureheadVM3.Figurehead : null) == CS$<>8__locals1.currentFigurehead;
			});
			ShipFigureheadSlotVM.GetIsRightSideDelegate getIsRightSide = ShipFigureheadSlotVM.GetIsRightSide;
			bool flag = getIsRightSide == null || getIsRightSide(this.Ship);
			for (int i = 0; i < base.AvailablePieces.Count; i++)
			{
				ShipUpgradePieceBaseVM shipUpgradePieceBaseVM = base.AvailablePieces[i];
				ShipFigureheadVM shipFigureheadVM = shipUpgradePieceBaseVM as ShipFigureheadVM;
				ShipFigureheadVM shipFigureheadVM2 = shipFigureheadVM;
				ShipFigureheadSlotVM.GetShipOfFigureheadDelegate getShipOfFigurehead = ShipFigureheadSlotVM.GetShipOfFigurehead;
				shipFigureheadVM2.EquippedShip = ((getShipOfFigurehead != null) ? getShipOfFigurehead(shipFigureheadVM.Figurehead, flag) : null);
				shipUpgradePieceBaseVM.IsDisabled = (shipFigureheadVM.EquippedShip != null && shipFigureheadVM.EquippedShip != this.Ship) || (shipFigureheadVM.EquippedShip == null && !this._enabledFigureheads.Contains(shipFigureheadVM.Figurehead));
				shipUpgradePieceBaseVM.IsHiddenFromPlayer = shipFigureheadVM.EquippedShip == null && !this._enabledFigureheads.Contains(shipFigureheadVM.Figurehead);
			}
			base.UpdateAnyBetterPiecesAvailable();
		}

		// Token: 0x04000044 RID: 68
		private readonly Figurehead _initialSelectedFigurehead;

		// Token: 0x04000045 RID: 69
		private readonly List<Figurehead> _enabledFigureheads;

		// Token: 0x02000046 RID: 70
		// (Invoke) Token: 0x06000479 RID: 1145
		public delegate Figurehead GetCurrentFigureheadDelegate(Ship ship);

		// Token: 0x02000047 RID: 71
		// (Invoke) Token: 0x0600047D RID: 1149
		public delegate Ship GetShipOfFigureheadDelegate(Figurehead figurehead, bool isRightSide);

		// Token: 0x02000048 RID: 72
		// (Invoke) Token: 0x06000481 RID: 1153
		public delegate bool GetIsRightSideDelegate(Ship ship);
	}
}
