using System;
using NavalDLC.CustomBattle.CustomBattle.SelectionItem;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace NavalDLC.CustomBattle.CustomBattle
{
	// Token: 0x02000010 RID: 16
	public class NavalCustomBattleFactionSelectionVM : ViewModel
	{
		// Token: 0x0600009C RID: 156 RVA: 0x000049D8 File Offset: 0x00002BD8
		public NavalCustomBattleFactionSelectionVM(Action<BasicCultureObject> onSelectionChanged)
		{
			this._onSelectionChanged = onSelectionChanged;
			this.Factions = new MBBindingList<NavalCustomBattleFactionItemVM>();
			foreach (BasicCultureObject basicCultureObject in NavalCustomBattleData.Factions)
			{
				this.Factions.Add(new NavalCustomBattleFactionItemVM(basicCultureObject, new Action<NavalCustomBattleFactionItemVM>(this.OnFactionSelected)));
			}
			this.SelectFaction(0);
			this.RefreshValues();
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00004A60 File Offset: 0x00002C60
		public override void RefreshValues()
		{
			base.RefreshValues();
			NavalCustomBattleFactionItemVM selectedItem = this.SelectedItem;
			this.SelectedFactionName = ((selectedItem != null) ? selectedItem.Faction.Name.ToString() : null);
			this.Factions.ApplyActionOnAllItems(delegate(NavalCustomBattleFactionItemVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00004ABF File Offset: 0x00002CBF
		public void SelectFaction(int index)
		{
			if (index >= 0 && index < this.Factions.Count)
			{
				this.SelectedItem = this.Factions[index];
			}
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00004AEC File Offset: 0x00002CEC
		public void ExecuteRandomize()
		{
			int num = MBRandom.RandomInt(this.Factions.Count);
			this.SelectFaction(num);
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00004B11 File Offset: 0x00002D11
		private void OnFactionSelected(NavalCustomBattleFactionItemVM faction)
		{
			this.SelectedItem = faction;
			this._onSelectionChanged(faction.Faction);
			this.SelectedFactionName = this.SelectedItem.Faction.Name.ToString();
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x060000A1 RID: 161 RVA: 0x00004B46 File Offset: 0x00002D46
		// (set) Token: 0x060000A2 RID: 162 RVA: 0x00004B4E File Offset: 0x00002D4E
		[DataSourceProperty]
		public MBBindingList<NavalCustomBattleFactionItemVM> Factions
		{
			get
			{
				return this._factions;
			}
			set
			{
				if (value != this._factions)
				{
					this._factions = value;
					base.OnPropertyChangedWithValue<MBBindingList<NavalCustomBattleFactionItemVM>>(value, "Factions");
				}
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x060000A3 RID: 163 RVA: 0x00004B6C File Offset: 0x00002D6C
		// (set) Token: 0x060000A4 RID: 164 RVA: 0x00004B74 File Offset: 0x00002D74
		[DataSourceProperty]
		public string SelectedFactionName
		{
			get
			{
				return this._selectedFactionName;
			}
			set
			{
				if (value != this._selectedFactionName)
				{
					this._selectedFactionName = value;
					base.OnPropertyChangedWithValue<string>(value, "SelectedFactionName");
				}
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x00004B97 File Offset: 0x00002D97
		// (set) Token: 0x060000A6 RID: 166 RVA: 0x00004BA0 File Offset: 0x00002DA0
		[DataSourceProperty]
		public NavalCustomBattleFactionItemVM SelectedItem
		{
			get
			{
				return this._selectedItem;
			}
			set
			{
				if (value != this._selectedItem)
				{
					if (this._selectedItem != null)
					{
						this._selectedItem.IsSelected = false;
					}
					this._selectedItem = value;
					base.OnPropertyChangedWithValue<NavalCustomBattleFactionItemVM>(value, "SelectedItem");
					if (this._selectedItem != null)
					{
						this._selectedItem.IsSelected = true;
					}
				}
			}
		}

		// Token: 0x0400004C RID: 76
		private Action<BasicCultureObject> _onSelectionChanged;

		// Token: 0x0400004D RID: 77
		private MBBindingList<NavalCustomBattleFactionItemVM> _factions;

		// Token: 0x0400004E RID: 78
		private string _selectedFactionName;

		// Token: 0x0400004F RID: 79
		private NavalCustomBattleFactionItemVM _selectedItem;
	}
}
