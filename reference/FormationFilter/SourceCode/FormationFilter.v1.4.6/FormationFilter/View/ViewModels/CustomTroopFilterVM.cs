using System;
using System.Runtime.CompilerServices;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;

namespace FormationFilter.View.ViewModels
{
	// Token: 0x0200000F RID: 15
	[NullableContext(1)]
	[Nullable(0)]
	public class CustomTroopFilterVM : ViewModel
	{
		// Token: 0x0600008B RID: 139 RVA: 0x00004B38 File Offset: 0x00002D38
		public CustomTroopFilterVM(OrderOfBattleFormationItemVM formationItemVM)
		{
			for (CustomFormationFilterType customFormationFilterType = CustomFormationFilterType.OneHanded; customFormationFilterType < CustomFormationFilterType.NumberOfFilterTypes; customFormationFilterType++)
			{
				this.CustomFilterItems.Add(new CustomFilterSelectorItemVM(customFormationFilterType, new Action<CustomFilterSelectorItemVM>(this.OnCustomFilterToggled), FilterSelectorMode.Both));
			}
			this._orderOfBattleFormationItemVM = formationItemVM;
			foreach (CustomFilterSelectorItemVM customFilterSelectorItemVM in this.CustomFilterItems)
			{
				customFilterSelectorItemVM.IsEnabled = formationItemVM.IsControlledByPlayer;
			}
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00004BD0 File Offset: 0x00002DD0
		public override void OnFinalize()
		{
			base.OnFinalize();
			this._customFilterItems.Clear();
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00004BE4 File Offset: 0x00002DE4
		private void OnCustomFilterToggled(CustomFilterSelectorItemVM filterItem)
		{
			Action<OrderOfBattleFormationItemVM> onFilterUseToggled = OrderOfBattleFormationItemVM.OnFilterUseToggled;
			if (onFilterUseToggled == null || this._orderOfBattleFormationItemVM == null)
			{
				return;
			}
			onFilterUseToggled(this._orderOfBattleFormationItemVM);
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600008E RID: 142 RVA: 0x00004C0F File Offset: 0x00002E0F
		// (set) Token: 0x0600008F RID: 143 RVA: 0x00004C17 File Offset: 0x00002E17
		[DataSourceProperty]
		public MBBindingList<CustomFilterSelectorItemVM> CustomFilterItems
		{
			get
			{
				return this._customFilterItems;
			}
			set
			{
				if (value == this._customFilterItems)
				{
					return;
				}
				this._customFilterItems = value;
				base.OnPropertyChangedWithValue<MBBindingList<CustomFilterSelectorItemVM>>(value, "CustomFilterItems");
			}
		}

		// Token: 0x0400004A RID: 74
		private MBBindingList<CustomFilterSelectorItemVM> _customFilterItems = new MBBindingList<CustomFilterSelectorItemVM>();

		// Token: 0x0400004B RID: 75
		private OrderOfBattleFormationItemVM _orderOfBattleFormationItemVM;
	}
}
