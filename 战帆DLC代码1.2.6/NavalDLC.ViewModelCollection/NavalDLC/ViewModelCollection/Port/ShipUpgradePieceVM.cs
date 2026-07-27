using System;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.ViewModelCollection.Port
{
	// Token: 0x02000014 RID: 20
	public class ShipUpgradePieceVM : ShipUpgradePieceBaseVM
	{
		// Token: 0x14000008 RID: 8
		// (add) Token: 0x06000193 RID: 403 RVA: 0x0000A098 File Offset: 0x00008298
		// (remove) Token: 0x06000194 RID: 404 RVA: 0x0000A0CC File Offset: 0x000082CC
		public static event Func<Ship, ShipUpgradePiece, int> GetUpgradePrice;

		// Token: 0x06000195 RID: 405 RVA: 0x0000A100 File Offset: 0x00008300
		public ShipUpgradePieceVM(ShipUpgradePiece piece, Ship ship, Action<ShipUpgradePieceBaseVM> onSelected)
			: base(onSelected)
		{
			this.Piece = piece;
			this.Ship = ship;
			base.UpgradePieceTier = (ShipUpgradePieceBaseVM.ShipUpgradePieceTier)MathF.Clamp((float)this.Piece.RequiredPortLevel, 1f, 4f);
			base.Identifier = piece.StringId;
			this.RefreshValues();
		}

		// Token: 0x06000196 RID: 406 RVA: 0x0000A158 File Offset: 0x00008358
		public override void RefreshValues()
		{
			base.RefreshValues();
			base.Name = this.Piece.GetName().ToString();
			Func<Ship, ShipUpgradePiece, int> getUpgradePrice = ShipUpgradePieceVM.GetUpgradePrice;
			base.Price = ((getUpgradePrice != null) ? getUpgradePrice(this.Ship, this.Piece) : 0);
		}

		// Token: 0x06000197 RID: 407 RVA: 0x0000A1A4 File Offset: 0x000083A4
		protected override PropertyBasedTooltipVM GetProperties()
		{
			object[] array = new object[] { this.Piece };
			PropertyBasedTooltipVM propertyBasedTooltipVM = new PropertyBasedTooltipVM(typeof(ShipUpgradePiece), array);
			if (!TextObject.IsNullOrEmpty(this.Piece.Description))
			{
				TooltipProperty tooltipProperty = new TooltipProperty(this.Piece.Description.ToString(), string.Empty, 0, false, 0);
				propertyBasedTooltipVM.TooltipPropertyList.Insert(0, tooltipProperty);
				tooltipProperty = new TooltipProperty(" ", " ", 0, false, 0);
				propertyBasedTooltipVM.TooltipPropertyList.Insert(1, tooltipProperty);
			}
			if (!base.IsInspectedFromSlot)
			{
				propertyBasedTooltipVM.AddProperty(" ", " ", 0, 0);
				if (base.IsSelected)
				{
					propertyBasedTooltipVM.AddProperty(new TextObject("{=OSoAVlqc}Equipped", null).ToString(), string.Empty, 0, 0);
				}
				else if (base.IsDisabled)
				{
					propertyBasedTooltipVM.AddProperty(new TextObject("{=DovqkMg1}Not Available In Settlement", null).ToString(), string.Empty, 0, 0);
				}
				else if (base.Price > 0)
				{
					propertyBasedTooltipVM.AddProperty(new TextObject("{=ebUrBmHK}Price", null).ToString(), base.Price.ToString(), 0, 0);
				}
				else
				{
					propertyBasedTooltipVM.AddProperty(new TextObject("{=Ve1E1wXz}Unlocked", null).ToString(), string.Empty, 0, 0);
				}
			}
			else if (!TextObject.IsNullOrEmpty(this._slotHintText))
			{
				propertyBasedTooltipVM.AddProperty(" ", " ", 0, 0);
				propertyBasedTooltipVM.AddProperty(this._slotHintText.ToString(), string.Empty, 0, 0);
			}
			return propertyBasedTooltipVM;
		}

		// Token: 0x04000098 RID: 152
		public readonly ShipUpgradePiece Piece;

		// Token: 0x04000099 RID: 153
		public readonly Ship Ship;
	}
}
