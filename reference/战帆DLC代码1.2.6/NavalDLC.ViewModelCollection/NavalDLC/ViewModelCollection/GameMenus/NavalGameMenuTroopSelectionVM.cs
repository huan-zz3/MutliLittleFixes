using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TroopSelection;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.ViewModelCollection.GameMenus
{
	// Token: 0x02000037 RID: 55
	public class NavalGameMenuTroopSelectionVM : GameMenuTroopSelectionVM
	{
		// Token: 0x0600043E RID: 1086 RVA: 0x00013E4C File Offset: 0x0001204C
		public NavalGameMenuTroopSelectionVM(TroopRoster fullRoster, TroopRoster initialSelections, List<Ship> eligibleShips, Func<CharacterObject, bool> canChangeChangeStatusOfTroop, Action<TroopRoster> onDone, int maxSelectableTroopCount, int minSelectableTroopCount)
			: base(fullRoster, initialSelections, canChangeChangeStatusOfTroop, onDone, maxSelectableTroopCount, minSelectableTroopCount)
		{
			this.Ships = new MBBindingList<NavalGameMenuShipItemVM>();
			for (int i = 0; i < eligibleShips.Count; i++)
			{
				NavalGameMenuShipItemVM navalGameMenuShipItemVM = new NavalGameMenuShipItemVM(eligibleShips[i]);
				this.Ships.Add(navalGameMenuShipItemVM);
			}
		}

		// Token: 0x0600043F RID: 1087 RVA: 0x00013E9E File Offset: 0x0001209E
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ShipSelectionDescriptionText = new TextObject("{=ikRW0ELi}Your most eligible ships have been automatically selected", null).ToString();
		}

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x06000440 RID: 1088 RVA: 0x00013EBC File Offset: 0x000120BC
		// (set) Token: 0x06000441 RID: 1089 RVA: 0x00013EC4 File Offset: 0x000120C4
		[DataSourceProperty]
		public string ShipSelectionDescriptionText
		{
			get
			{
				return this._shipSelectionDescriptionText;
			}
			set
			{
				if (value != this._shipSelectionDescriptionText)
				{
					this._shipSelectionDescriptionText = value;
					base.OnPropertyChangedWithValue<string>(value, "ShipSelectionDescriptionText");
				}
			}
		}

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x06000442 RID: 1090 RVA: 0x00013EE7 File Offset: 0x000120E7
		// (set) Token: 0x06000443 RID: 1091 RVA: 0x00013EEF File Offset: 0x000120EF
		[DataSourceProperty]
		public MBBindingList<NavalGameMenuShipItemVM> Ships
		{
			get
			{
				return this._ships;
			}
			set
			{
				if (value != this._ships)
				{
					this._ships = value;
					base.OnPropertyChangedWithValue<MBBindingList<NavalGameMenuShipItemVM>>(value, "Ships");
				}
			}
		}

		// Token: 0x040001AA RID: 426
		private string _shipSelectionDescriptionText;

		// Token: 0x040001AB RID: 427
		private MBBindingList<NavalGameMenuShipItemVM> _ships;
	}
}
