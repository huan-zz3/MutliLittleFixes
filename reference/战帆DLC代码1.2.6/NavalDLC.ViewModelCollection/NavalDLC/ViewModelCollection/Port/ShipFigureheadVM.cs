using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Localization;

namespace NavalDLC.ViewModelCollection.Port
{
	// Token: 0x0200000D RID: 13
	public class ShipFigureheadVM : ShipUpgradePieceBaseVM
	{
		// Token: 0x060000D0 RID: 208 RVA: 0x00007BC2 File Offset: 0x00005DC2
		public ShipFigureheadVM(Figurehead figurehead, Action<ShipUpgradePieceBaseVM> onSelected)
			: base(onSelected)
		{
			this.Figurehead = figurehead;
			base.Price = 0;
			base.UpgradePieceTier = ShipUpgradePieceBaseVM.ShipUpgradePieceTier.Diamond;
			base.Identifier = figurehead.StringId;
			this.RefreshValues();
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00007C02 File Offset: 0x00005E02
		public override void RefreshValues()
		{
			base.RefreshValues();
			base.Name = this.Figurehead.Name.ToString();
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00007C20 File Offset: 0x00005E20
		protected override PropertyBasedTooltipVM GetProperties()
		{
			object[] array = new object[] { this.Figurehead };
			PropertyBasedTooltipVM propertyBasedTooltipVM = new PropertyBasedTooltipVM(typeof(Figurehead), array);
			if (base.IsHiddenFromPlayer)
			{
				propertyBasedTooltipVM.TooltipPropertyList.Clear();
				propertyBasedTooltipVM.AddProperty(new TextObject("{=4RUs8Cfu}Not Unlocked", null).ToString(), string.Empty, 0, 0);
				return propertyBasedTooltipVM;
			}
			if (!base.IsInspectedFromSlot)
			{
				propertyBasedTooltipVM.AddProperty(" ", " ", 0, 0);
				if (base.IsSelected)
				{
					propertyBasedTooltipVM.AddProperty(new TextObject("{=OSoAVlqc}Equipped", null).ToString(), string.Empty, 0, 0);
				}
				else if (this.EquippedShip != null)
				{
					propertyBasedTooltipVM.AddProperty(new TextObject("{=bQzObjHj}Attached Ship", null).ToString(), this.EquippedShip.Name.ToString(), 0, 0);
				}
				else if (base.IsDisabled)
				{
					propertyBasedTooltipVM.AddProperty(new TextObject("{=4RUs8Cfu}Not Unlocked", null).ToString(), string.Empty, 0, 0);
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

		// Token: 0x060000D3 RID: 211 RVA: 0x00007D77 File Offset: 0x00005F77
		public override void InspectPiece(bool isInspectedFromSlot = false, TextObject slotHintText = null)
		{
			base.InspectPiece(isInspectedFromSlot, slotHintText);
			if (base.IsUnexamined)
			{
				this._viewDataTracker.OnFigureheadExamined(this.Figurehead);
			}
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00007D9A File Offset: 0x00005F9A
		public override void Update()
		{
			base.Update();
			base.IsUnexamined = !base.IsDisabled && this._viewDataTracker.UnexaminedFigureheads.Contains(this.Figurehead);
		}

		// Token: 0x04000046 RID: 70
		public Ship EquippedShip;

		// Token: 0x04000047 RID: 71
		public readonly Figurehead Figurehead;

		// Token: 0x04000048 RID: 72
		private readonly IViewDataTracker _viewDataTracker = Campaign.Current.GetCampaignBehavior<IViewDataTracker>();
	}
}
