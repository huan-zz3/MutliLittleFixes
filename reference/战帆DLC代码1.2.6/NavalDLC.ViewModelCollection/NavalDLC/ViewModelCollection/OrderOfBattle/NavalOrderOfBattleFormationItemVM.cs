using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions.MissionLogics;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;

namespace NavalDLC.ViewModelCollection.OrderOfBattle
{
	// Token: 0x02000023 RID: 35
	public class NavalOrderOfBattleFormationItemVM : ViewModel
	{
		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000290 RID: 656 RVA: 0x0000E09C File Offset: 0x0000C29C
		// (set) Token: 0x06000291 RID: 657 RVA: 0x0000E0A4 File Offset: 0x0000C2A4
		public DeploymentFormationClass SelectedClass { get; private set; }

		// Token: 0x06000292 RID: 658 RVA: 0x0000E0B0 File Offset: 0x0000C2B0
		public NavalOrderOfBattleFormationItemVM(Formation formation, Action<NavalOrderOfBattleFormationItemVM> onSelected, Action<NavalOrderOfBattleFormationItemVM> onClassChanged, Action<NavalOrderOfBattleFormationItemVM> onFilterToggled)
		{
			this.Formation = formation;
			this._onSelected = onSelected;
			this._onClassChanged = onClassChanged;
			this._onFilterToggled = onFilterToggled;
			this.FilterItems = new MBBindingList<OrderOfBattleFormationFilterSelectorItemVM>();
			for (FormationFilterType formationFilterType = 1; formationFilterType < 7; formationFilterType++)
			{
				if (formationFilterType != 2)
				{
					this.FilterItems.Add(new OrderOfBattleFormationFilterSelectorItemVM(formationFilterType, new Action<OrderOfBattleFormationFilterSelectorItemVM>(this.OnFilterToggled)));
				}
			}
			this.FilterItems.ApplyActionOnAllItems(delegate(OrderOfBattleFormationFilterSelectorItemVM x)
			{
				x.IsEnabled = this.IsSelectable;
			});
			this.Tooltip = new BasicTooltipViewModel(() => this.GetTooltip());
			this.ExecuteSelectInfantryAndRanged();
			this.RefreshValues();
		}

		// Token: 0x06000293 RID: 659 RVA: 0x0000E1DC File Offset: 0x0000C3DC
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.FormationName = (this.Formation.Index + 1).ToString();
			this.FormationIsEmptyText = new TextObject("{=P3IWytsr}Formation is currently empty", null).ToString();
			this.CaptainSlotHint = new HintViewModel(this._captainSlotHintText, null);
			this.ShipSlotHint = new HintViewModel(this._shipSlotHintText, null);
			this.AssignCaptainHint = new HintViewModel(this._assignCaptainHintText, null);
			this.AssignShipHint = new HintViewModel(this._assignShipHintText, null);
			this.InfantryHint = new HintViewModel(this._infantryHintText, null);
			this.RangedHint = new HintViewModel(this._rangedHintText, null);
			this.InfantryAndRangedHint = new HintViewModel(this._infantryAndRangedHintText, null);
			this.TroopCount = this.Formation.CountOfUnits;
			this.SkeletalCrewCountWarning = new TextObject("{=JEwakKND}Ship is undercrewed!", null).ToString();
		}

		// Token: 0x06000294 RID: 660 RVA: 0x0000E2C8 File Offset: 0x0000C4C8
		public override void OnFinalize()
		{
			base.OnFinalize();
			foreach (OrderOfBattleFormationFilterSelectorItemVM orderOfBattleFormationFilterSelectorItemVM in this.FilterItems)
			{
				orderOfBattleFormationFilterSelectorItemVM.OnFinalize();
			}
			this.FilterItems.Clear();
		}

		// Token: 0x06000295 RID: 661 RVA: 0x0000E324 File Offset: 0x0000C524
		public void ExecuteSelect()
		{
			Action<NavalOrderOfBattleFormationItemVM> onSelected = this._onSelected;
			if (onSelected == null)
			{
				return;
			}
			onSelected(this);
		}

		// Token: 0x06000296 RID: 662 RVA: 0x0000E337 File Offset: 0x0000C537
		public void ExecuteAcceptShip()
		{
			if (this.GetCanAcceptShip())
			{
				Action<NavalOrderOfBattleFormationItemVM> onAcceptShip = NavalOrderOfBattleFormationItemVM.OnAcceptShip;
				if (onAcceptShip == null)
				{
					return;
				}
				onAcceptShip(this);
			}
		}

		// Token: 0x06000297 RID: 663 RVA: 0x0000E351 File Offset: 0x0000C551
		public void ExecuteAcceptCaptain()
		{
			if (this.GetCanAcceptCaptain())
			{
				Action<NavalOrderOfBattleFormationItemVM> onAcceptCaptain = NavalOrderOfBattleFormationItemVM.OnAcceptCaptain;
				if (onAcceptCaptain == null)
				{
					return;
				}
				onAcceptCaptain(this);
			}
		}

		// Token: 0x06000298 RID: 664 RVA: 0x0000E36B File Offset: 0x0000C56B
		private void OnFilterToggled(OrderOfBattleFormationFilterSelectorItemVM filterItem)
		{
			if (this.IsSelectable)
			{
				Action<NavalOrderOfBattleFormationItemVM> onFilterToggled = this._onFilterToggled;
				if (onFilterToggled == null)
				{
					return;
				}
				onFilterToggled(this);
			}
		}

		// Token: 0x06000299 RID: 665 RVA: 0x0000E386 File Offset: 0x0000C586
		private bool HasAnyActiveFilter()
		{
			return this.FilterItems.Any<OrderOfBattleFormationFilterSelectorItemVM>((OrderOfBattleFormationFilterSelectorItemVM f) => f.IsActive);
		}

		// Token: 0x0600029A RID: 666 RVA: 0x0000E3B4 File Offset: 0x0000C5B4
		public bool HasFilter(FormationFilterType filter)
		{
			return this.FilterItems.Any<OrderOfBattleFormationFilterSelectorItemVM>((OrderOfBattleFormationFilterSelectorItemVM f) => f.IsActive && f.FilterType == filter);
		}

		// Token: 0x0600029B RID: 667 RVA: 0x0000E3E5 File Offset: 0x0000C5E5
		public void ExecuteSelectInfantry()
		{
			this.SelectedClass = 1;
			this.OnClassSelectionUpdated();
		}

		// Token: 0x0600029C RID: 668 RVA: 0x0000E3F4 File Offset: 0x0000C5F4
		public void ExecuteSelectRanged()
		{
			this.SelectedClass = 2;
			this.OnClassSelectionUpdated();
		}

		// Token: 0x0600029D RID: 669 RVA: 0x0000E403 File Offset: 0x0000C603
		public void ExecuteSelectInfantryAndRanged()
		{
			this.SelectedClass = 5;
			this.OnClassSelectionUpdated();
		}

		// Token: 0x0600029E RID: 670 RVA: 0x0000E414 File Offset: 0x0000C614
		private void OnClassSelectionUpdated()
		{
			this.IsInfantrySelected = this.SelectedClass == 1;
			this.IsRangedSelected = this.SelectedClass == 2;
			this.IsInfantryAndRangedSelected = this.SelectedClass == 5;
			this.FormationClassInt = this.SelectedClass;
			if (this.IsSelectable)
			{
				Action<NavalOrderOfBattleFormationItemVM> onClassChanged = this._onClassChanged;
				if (onClassChanged == null)
				{
					return;
				}
				onClassChanged(this);
			}
		}

		// Token: 0x0600029F RID: 671 RVA: 0x0000E473 File Offset: 0x0000C673
		public bool GetCanAcceptShip()
		{
			if (!this.IsEnabled)
			{
				NavalOrderOfBattleHeroItemVM captain = this.Captain;
				return captain != null && captain.IsMainHero;
			}
			return true;
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x0000E490 File Offset: 0x0000C690
		public bool GetCanAcceptCaptain()
		{
			if (this.IsEnabled && this.HasShip)
			{
				NavalOrderOfBattleHeroItemVM captain = this.Captain;
				return captain == null || !captain.IsMainHero;
			}
			return false;
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x0000E4B8 File Offset: 0x0000C6B8
		private List<TooltipProperty> GetTooltip()
		{
			List<TooltipProperty> list = new List<TooltipProperty>
			{
				new TooltipProperty(new TextObject("{=cZNA5Z6l}Formation {NUMBER}", null).SetTextVariable("NUMBER", this.FormationName).ToString(), string.Empty, 0, false, 4096)
			};
			if (!this.HasShip)
			{
				return list;
			}
			List<Agent> list2 = new List<Agent>();
			int[] array = new int[4];
			using (List<IFormationUnit>.Enumerator enumerator = this.Formation.Arrangement.GetAllUnits().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Agent agent4;
					if ((agent4 = enumerator.Current as Agent) != null)
					{
						if (agent4.IsHero)
						{
							list2.Add(agent4);
						}
						FormationClass actualTroopType = this.GetActualTroopType(agent4);
						if (actualTroopType >= 0 && actualTroopType < 4)
						{
							array[actualTroopType]++;
						}
					}
				}
			}
			foreach (Agent agent2 in this.Formation.DetachedUnits)
			{
				if (agent2.IsHero)
				{
					list2.Add(agent2);
				}
				FormationClass actualTroopType2 = this.GetActualTroopType(agent2);
				if (actualTroopType2 >= 0 && actualTroopType2 < 4)
				{
					array[actualTroopType2]++;
				}
			}
			bool flag = false;
			for (FormationClass formationClass = 0; formationClass < 4; formationClass++)
			{
				int num = array[formationClass];
				List<Agent> list3 = new List<Agent>();
				for (int i = 0; i < list2.Count; i++)
				{
					Agent agent3 = list2[i];
					if (formationClass == this.GetActualTroopType(agent3))
					{
						list3.Add(agent3);
					}
				}
				if (num > 0)
				{
					if (flag)
					{
						list.Add(new TooltipProperty(string.Empty, string.Empty, -1, false, 0));
					}
					else
					{
						flag = true;
					}
					List<TooltipProperty> list4 = list;
					string text = "str_troop_group_name";
					int num2 = formationClass;
					list4.Add(new TooltipProperty(GameTexts.FindText(text, num2.ToString()).ToString(), num.ToString(), 0, false, 0));
					if (list3.Count > 0)
					{
						list.Add(new TooltipProperty(string.Empty, string.Empty, 0, false, 512));
					}
					for (int j = 0; j < list3.Count; j++)
					{
						list.Add(new TooltipProperty(list3[j].Name, " ", 0, false, 0));
					}
				}
			}
			if (this.HasAnyActiveFilter())
			{
				list.Add(new TooltipProperty(string.Empty, string.Empty, 0, false, 1024));
			}
			if (this.HasFilter(1))
			{
				GameTexts.SetVariable("TROOP_COUNT", this.Formation.GetCountOfUnitsWithCondition((Agent agent) => agent.HasShieldCached));
				GameTexts.SetVariable("TOTAL_TROOP_COUNT", NavalOrderOfBattleFormationItemVM.GetTotalTroopCountWithFilter(this.SelectedClass, 1));
				list.Add(new TooltipProperty(OrderOfBattleFormationExtensions.GetFilterName(1).ToString(), this._filteredTroopCountInfoText.ToString(), 0, false, 0));
			}
			if (this.HasFilter(3))
			{
				GameTexts.SetVariable("TROOP_COUNT", this.Formation.GetCountOfUnitsWithCondition((Agent agent) => agent.HasThrownCached));
				GameTexts.SetVariable("TOTAL_TROOP_COUNT", NavalOrderOfBattleFormationItemVM.GetTotalTroopCountWithFilter(this.SelectedClass, 3));
				list.Add(new TooltipProperty(OrderOfBattleFormationExtensions.GetFilterName(3).ToString(), this._filteredTroopCountInfoText.ToString(), 0, false, 0));
			}
			if (this.HasFilter(4))
			{
				GameTexts.SetVariable("TROOP_COUNT", this.Formation.GetCountOfUnitsWithCondition((Agent agent) => MissionGameModels.Current.AgentStatCalculateModel.HasHeavyArmor(agent)));
				GameTexts.SetVariable("TOTAL_TROOP_COUNT", NavalOrderOfBattleFormationItemVM.GetTotalTroopCountWithFilter(this.SelectedClass, 4));
				list.Add(new TooltipProperty(OrderOfBattleFormationExtensions.GetFilterName(4).ToString(), this._filteredTroopCountInfoText.ToString(), 0, false, 0));
			}
			if (this.HasFilter(5))
			{
				GameTexts.SetVariable("TROOP_COUNT", this.Formation.GetCountOfUnitsWithCondition((Agent agent) => agent.Character.GetBattleTier() >= 4));
				GameTexts.SetVariable("TOTAL_TROOP_COUNT", NavalOrderOfBattleFormationItemVM.GetTotalTroopCountWithFilter(this.SelectedClass, 5));
				list.Add(new TooltipProperty(OrderOfBattleFormationExtensions.GetFilterName(5).ToString(), this._filteredTroopCountInfoText.ToString(), 0, false, 0));
			}
			if (this.HasFilter(6))
			{
				GameTexts.SetVariable("TROOP_COUNT", this.Formation.GetCountOfUnitsWithCondition((Agent agent) => agent.Character.GetBattleTier() <= 3));
				GameTexts.SetVariable("TOTAL_TROOP_COUNT", NavalOrderOfBattleFormationItemVM.GetTotalTroopCountWithFilter(this.SelectedClass, 6));
				list.Add(new TooltipProperty(OrderOfBattleFormationExtensions.GetFilterName(6).ToString(), this._filteredTroopCountInfoText.ToString(), 0, false, 0));
			}
			NavalOrderOfBattleShipItemVM ship = this.Ship;
			if (((ship != null) ? ship.MissionShip : null) != null)
			{
				int reservedTroopsCountOfShip = Mission.Current.GetMissionBehavior<NavalAgentsLogic>().GetReservedTroopsCountOfShip(this.Ship.MissionShip);
				if (reservedTroopsCountOfShip > 0)
				{
					list.Add(new TooltipProperty(string.Empty, string.Empty, 0, false, 1024));
					list.Add(new TooltipProperty(new TextObject("{=25fleLuY}Troops In Reserve", null).ToString(), reservedTroopsCountOfShip.ToString(), 0, false, 0));
				}
			}
			return list;
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x0000EA28 File Offset: 0x0000CC28
		private FormationClass GetActualTroopType(Agent agent)
		{
			if (QueryLibrary.IsInfantry(agent))
			{
				return 0;
			}
			if (QueryLibrary.IsRanged(agent))
			{
				return 1;
			}
			if (QueryLibrary.IsCavalry(agent))
			{
				return 2;
			}
			if (QueryLibrary.IsRangedCavalry(agent))
			{
				return 3;
			}
			return 10;
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060002A3 RID: 675 RVA: 0x0000EA54 File Offset: 0x0000CC54
		// (set) Token: 0x060002A4 RID: 676 RVA: 0x0000EA5C File Offset: 0x0000CC5C
		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060002A5 RID: 677 RVA: 0x0000EA7A File Offset: 0x0000CC7A
		// (set) Token: 0x060002A6 RID: 678 RVA: 0x0000EA82 File Offset: 0x0000CC82
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
					this.IsSelectable = this.HasShip && this.IsEnabled;
				}
			}
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060002A7 RID: 679 RVA: 0x0000EAB7 File Offset: 0x0000CCB7
		// (set) Token: 0x060002A8 RID: 680 RVA: 0x0000EABF File Offset: 0x0000CCBF
		[DataSourceProperty]
		public bool IsSelectable
		{
			get
			{
				return this._isSelectable;
			}
			set
			{
				if (value != this._isSelectable)
				{
					this._isSelectable = value;
					base.OnPropertyChangedWithValue(value, "IsSelectable");
					this.FilterItems.ApplyActionOnAllItems(delegate(OrderOfBattleFormationFilterSelectorItemVM x)
					{
						x.IsEnabled = this.IsSelectable;
					});
				}
			}
		}

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x060002A9 RID: 681 RVA: 0x0000EAF4 File Offset: 0x0000CCF4
		// (set) Token: 0x060002AA RID: 682 RVA: 0x0000EAFC File Offset: 0x0000CCFC
		[DataSourceProperty]
		public bool HasCaptain
		{
			get
			{
				return this._hasCaptain;
			}
			set
			{
				if (value != this._hasCaptain)
				{
					this._hasCaptain = value;
					base.OnPropertyChangedWithValue(value, "HasCaptain");
				}
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x060002AB RID: 683 RVA: 0x0000EB1A File Offset: 0x0000CD1A
		// (set) Token: 0x060002AC RID: 684 RVA: 0x0000EB24 File Offset: 0x0000CD24
		[DataSourceProperty]
		public bool HasShip
		{
			get
			{
				return this._hasShip;
			}
			set
			{
				if (value != this._hasShip)
				{
					this._hasShip = value;
					base.OnPropertyChangedWithValue(value, "HasShip");
					this.IsSelectable = this.HasShip && this.IsEnabled;
					this.IsSkeletalCrewCountWarningActive = this.HasShip && this.TroopCount < this.Ship.ShipOrigin.SkeletalCrewCapacity;
				}
			}
		}

		// Token: 0x17000097 RID: 151
		// (get) Token: 0x060002AD RID: 685 RVA: 0x0000EB8D File Offset: 0x0000CD8D
		// (set) Token: 0x060002AE RID: 686 RVA: 0x0000EB95 File Offset: 0x0000CD95
		[DataSourceProperty]
		public bool IsAcceptingCaptain
		{
			get
			{
				return this._isAcceptingCaptain;
			}
			set
			{
				if (value != this._isAcceptingCaptain)
				{
					this._isAcceptingCaptain = value;
					base.OnPropertyChangedWithValue(value, "IsAcceptingCaptain");
				}
			}
		}

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x060002AF RID: 687 RVA: 0x0000EBB3 File Offset: 0x0000CDB3
		// (set) Token: 0x060002B0 RID: 688 RVA: 0x0000EBBB File Offset: 0x0000CDBB
		[DataSourceProperty]
		public bool IsAcceptingShip
		{
			get
			{
				return this._isAcceptingShip;
			}
			set
			{
				if (value != this._isAcceptingShip)
				{
					this._isAcceptingShip = value;
					base.OnPropertyChangedWithValue(value, "IsAcceptingShip");
				}
			}
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x060002B1 RID: 689 RVA: 0x0000EBD9 File Offset: 0x0000CDD9
		// (set) Token: 0x060002B2 RID: 690 RVA: 0x0000EBE1 File Offset: 0x0000CDE1
		[DataSourceProperty]
		public bool IsInfantrySelected
		{
			get
			{
				return this._isInfantrySelected;
			}
			set
			{
				if (value != this._isInfantrySelected)
				{
					this._isInfantrySelected = value;
					base.OnPropertyChangedWithValue(value, "IsInfantrySelected");
				}
			}
		}

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x060002B3 RID: 691 RVA: 0x0000EBFF File Offset: 0x0000CDFF
		// (set) Token: 0x060002B4 RID: 692 RVA: 0x0000EC07 File Offset: 0x0000CE07
		[DataSourceProperty]
		public bool IsRangedSelected
		{
			get
			{
				return this._isRangedSelected;
			}
			set
			{
				if (value != this._isRangedSelected)
				{
					this._isRangedSelected = value;
					base.OnPropertyChangedWithValue(value, "IsRangedSelected");
				}
			}
		}

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x060002B5 RID: 693 RVA: 0x0000EC25 File Offset: 0x0000CE25
		// (set) Token: 0x060002B6 RID: 694 RVA: 0x0000EC2D File Offset: 0x0000CE2D
		[DataSourceProperty]
		public bool IsInfantryAndRangedSelected
		{
			get
			{
				return this._isInfantryAndRangedSelected;
			}
			set
			{
				if (value != this._isInfantryAndRangedSelected)
				{
					this._isInfantryAndRangedSelected = value;
					base.OnPropertyChangedWithValue(value, "IsInfantryAndRangedSelected");
				}
			}
		}

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x060002B7 RID: 695 RVA: 0x0000EC4B File Offset: 0x0000CE4B
		// (set) Token: 0x060002B8 RID: 696 RVA: 0x0000EC53 File Offset: 0x0000CE53
		[DataSourceProperty]
		public string FormationName
		{
			get
			{
				return this._formationName;
			}
			set
			{
				if (value != this._formationName)
				{
					this._formationName = value;
					base.OnPropertyChangedWithValue<string>(value, "FormationName");
				}
			}
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x060002B9 RID: 697 RVA: 0x0000EC76 File Offset: 0x0000CE76
		// (set) Token: 0x060002BA RID: 698 RVA: 0x0000EC7E File Offset: 0x0000CE7E
		[DataSourceProperty]
		public string FormationIsEmptyText
		{
			get
			{
				return this._formationIsEmptyText;
			}
			set
			{
				if (value != this._formationIsEmptyText)
				{
					this._formationIsEmptyText = value;
					base.OnPropertyChangedWithValue<string>(value, "FormationIsEmptyText");
				}
			}
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x060002BB RID: 699 RVA: 0x0000ECA1 File Offset: 0x0000CEA1
		// (set) Token: 0x060002BC RID: 700 RVA: 0x0000ECAC File Offset: 0x0000CEAC
		[DataSourceProperty]
		public int TroopCount
		{
			get
			{
				return this._troopCount;
			}
			set
			{
				if (value != this._troopCount)
				{
					this._troopCount = value;
					base.OnPropertyChangedWithValue(value, "TroopCount");
					this.IsSkeletalCrewCountWarningActive = this.HasShip && this.TroopCount < this.Ship.ShipOrigin.SkeletalCrewCapacity;
				}
			}
		}

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x060002BD RID: 701 RVA: 0x0000ECFE File Offset: 0x0000CEFE
		// (set) Token: 0x060002BE RID: 702 RVA: 0x0000ED06 File Offset: 0x0000CF06
		[DataSourceProperty]
		public int FormationClassInt
		{
			get
			{
				return this._formationClassInt;
			}
			set
			{
				if (value != this._formationClassInt)
				{
					this._formationClassInt = value;
					base.OnPropertyChangedWithValue(value, "FormationClassInt");
				}
			}
		}

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x060002BF RID: 703 RVA: 0x0000ED24 File Offset: 0x0000CF24
		// (set) Token: 0x060002C0 RID: 704 RVA: 0x0000ED2C File Offset: 0x0000CF2C
		[DataSourceProperty]
		public bool IsSkeletalCrewCountWarningActive
		{
			get
			{
				return this._isSkeletalCrewCountWarningActive;
			}
			set
			{
				if (value != this._isSkeletalCrewCountWarningActive)
				{
					this._isSkeletalCrewCountWarningActive = value;
					base.OnPropertyChangedWithValue(value, "IsSkeletalCrewCountWarningActive");
				}
			}
		}

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x060002C1 RID: 705 RVA: 0x0000ED4A File Offset: 0x0000CF4A
		// (set) Token: 0x060002C2 RID: 706 RVA: 0x0000ED52 File Offset: 0x0000CF52
		[DataSourceProperty]
		public string SkeletalCrewCountWarning
		{
			get
			{
				return this._skeletalCrewCountWarning;
			}
			set
			{
				if (value != this._skeletalCrewCountWarning)
				{
					this._skeletalCrewCountWarning = value;
					base.OnPropertyChangedWithValue<string>(value, "SkeletalCrewCountWarning");
				}
			}
		}

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x060002C3 RID: 707 RVA: 0x0000ED75 File Offset: 0x0000CF75
		// (set) Token: 0x060002C4 RID: 708 RVA: 0x0000ED7D File Offset: 0x0000CF7D
		[DataSourceProperty]
		public HintViewModel CaptainSlotHint
		{
			get
			{
				return this._captainSlotHint;
			}
			set
			{
				if (value != this._captainSlotHint)
				{
					this._captainSlotHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "CaptainSlotHint");
				}
			}
		}

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x060002C5 RID: 709 RVA: 0x0000ED9B File Offset: 0x0000CF9B
		// (set) Token: 0x060002C6 RID: 710 RVA: 0x0000EDA3 File Offset: 0x0000CFA3
		[DataSourceProperty]
		public HintViewModel ShipSlotHint
		{
			get
			{
				return this._shipSlotHint;
			}
			set
			{
				if (value != this._shipSlotHint)
				{
					this._shipSlotHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "ShipSlotHint");
				}
			}
		}

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x060002C7 RID: 711 RVA: 0x0000EDC1 File Offset: 0x0000CFC1
		// (set) Token: 0x060002C8 RID: 712 RVA: 0x0000EDC9 File Offset: 0x0000CFC9
		[DataSourceProperty]
		public HintViewModel AssignCaptainHint
		{
			get
			{
				return this._assignCaptainHint;
			}
			set
			{
				if (value != this._assignCaptainHint)
				{
					this._assignCaptainHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "AssignCaptainHint");
				}
			}
		}

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x060002C9 RID: 713 RVA: 0x0000EDE7 File Offset: 0x0000CFE7
		// (set) Token: 0x060002CA RID: 714 RVA: 0x0000EDEF File Offset: 0x0000CFEF
		[DataSourceProperty]
		public HintViewModel AssignShipHint
		{
			get
			{
				return this._assignShipHint;
			}
			set
			{
				if (value != this._assignShipHint)
				{
					this._assignShipHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "AssignShipHint");
				}
			}
		}

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x060002CB RID: 715 RVA: 0x0000EE0D File Offset: 0x0000D00D
		// (set) Token: 0x060002CC RID: 716 RVA: 0x0000EE15 File Offset: 0x0000D015
		[DataSourceProperty]
		public NavalOrderOfBattleHeroItemVM Captain
		{
			get
			{
				return this._captain;
			}
			set
			{
				if (value != this._captain)
				{
					this._captain = value;
					base.OnPropertyChangedWithValue<NavalOrderOfBattleHeroItemVM>(value, "Captain");
					this.HasCaptain = this.Captain != null;
				}
			}
		}

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x060002CD RID: 717 RVA: 0x0000EE42 File Offset: 0x0000D042
		// (set) Token: 0x060002CE RID: 718 RVA: 0x0000EE4C File Offset: 0x0000D04C
		[DataSourceProperty]
		public NavalOrderOfBattleShipItemVM Ship
		{
			get
			{
				return this._ship;
			}
			set
			{
				if (value != this._ship)
				{
					this._ship = value;
					base.OnPropertyChangedWithValue<NavalOrderOfBattleShipItemVM>(value, "Ship");
					this.HasShip = this.Ship != null;
					if (!this.HasShip)
					{
						foreach (OrderOfBattleFormationFilterSelectorItemVM orderOfBattleFormationFilterSelectorItemVM in this.FilterItems)
						{
							orderOfBattleFormationFilterSelectorItemVM.IsActive = false;
						}
					}
					this.IsSkeletalCrewCountWarningActive = this.HasShip && this.TroopCount < this.Ship.ShipOrigin.SkeletalCrewCapacity;
				}
			}
		}

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x060002CF RID: 719 RVA: 0x0000EEF8 File Offset: 0x0000D0F8
		// (set) Token: 0x060002D0 RID: 720 RVA: 0x0000EF00 File Offset: 0x0000D100
		[DataSourceProperty]
		public int WSign
		{
			get
			{
				return this._wSign;
			}
			set
			{
				if (value != this._wSign)
				{
					this._wSign = value;
					base.OnPropertyChangedWithValue(value, "WSign");
				}
			}
		}

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x060002D1 RID: 721 RVA: 0x0000EF1E File Offset: 0x0000D11E
		// (set) Token: 0x060002D2 RID: 722 RVA: 0x0000EF26 File Offset: 0x0000D126
		[DataSourceProperty]
		public Vec2 ScreenPosition
		{
			get
			{
				return this._screenPosition;
			}
			set
			{
				if (value != this._screenPosition)
				{
					this._screenPosition = value;
					base.OnPropertyChangedWithValue(value, "ScreenPosition");
				}
			}
		}

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x060002D3 RID: 723 RVA: 0x0000EF49 File Offset: 0x0000D149
		// (set) Token: 0x060002D4 RID: 724 RVA: 0x0000EF51 File Offset: 0x0000D151
		[DataSourceProperty]
		public MBBindingList<OrderOfBattleFormationFilterSelectorItemVM> FilterItems
		{
			get
			{
				return this._filterItems;
			}
			set
			{
				if (value != this._filterItems)
				{
					this._filterItems = value;
					base.OnPropertyChangedWithValue<MBBindingList<OrderOfBattleFormationFilterSelectorItemVM>>(value, "FilterItems");
				}
			}
		}

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x060002D5 RID: 725 RVA: 0x0000EF6F File Offset: 0x0000D16F
		// (set) Token: 0x060002D6 RID: 726 RVA: 0x0000EF77 File Offset: 0x0000D177
		[DataSourceProperty]
		public HintViewModel InfantryHint
		{
			get
			{
				return this._infantryHint;
			}
			set
			{
				if (value != this._infantryHint)
				{
					this._infantryHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "InfantryHint");
				}
			}
		}

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x060002D7 RID: 727 RVA: 0x0000EF95 File Offset: 0x0000D195
		// (set) Token: 0x060002D8 RID: 728 RVA: 0x0000EF9D File Offset: 0x0000D19D
		[DataSourceProperty]
		public HintViewModel RangedHint
		{
			get
			{
				return this._rangedHint;
			}
			set
			{
				if (value != this._rangedHint)
				{
					this._rangedHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "RangedHint");
				}
			}
		}

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x060002D9 RID: 729 RVA: 0x0000EFBB File Offset: 0x0000D1BB
		// (set) Token: 0x060002DA RID: 730 RVA: 0x0000EFC3 File Offset: 0x0000D1C3
		[DataSourceProperty]
		public HintViewModel InfantryAndRangedHint
		{
			get
			{
				return this._infantryAndRangedHint;
			}
			set
			{
				if (value != this._infantryAndRangedHint)
				{
					this._infantryAndRangedHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "InfantryAndRangedHint");
				}
			}
		}

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x060002DB RID: 731 RVA: 0x0000EFE1 File Offset: 0x0000D1E1
		// (set) Token: 0x060002DC RID: 732 RVA: 0x0000EFE9 File Offset: 0x0000D1E9
		[DataSourceProperty]
		public HintViewModel DisabledHint
		{
			get
			{
				return this._disabledHint;
			}
			set
			{
				if (value != this._disabledHint)
				{
					this._disabledHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "DisabledHint");
				}
			}
		}

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x060002DD RID: 733 RVA: 0x0000F007 File Offset: 0x0000D207
		// (set) Token: 0x060002DE RID: 734 RVA: 0x0000F00F File Offset: 0x0000D20F
		[DataSourceProperty]
		public BasicTooltipViewModel Tooltip
		{
			get
			{
				return this._tooltip;
			}
			set
			{
				if (value != this._tooltip)
				{
					this._tooltip = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "Tooltip");
				}
			}
		}

		// Token: 0x040000D8 RID: 216
		public readonly Formation Formation;

		// Token: 0x040000D9 RID: 217
		private readonly Action<NavalOrderOfBattleFormationItemVM> _onSelected;

		// Token: 0x040000DA RID: 218
		private readonly Action<NavalOrderOfBattleFormationItemVM> _onClassChanged;

		// Token: 0x040000DB RID: 219
		private readonly Action<NavalOrderOfBattleFormationItemVM> _onFilterToggled;

		// Token: 0x040000DC RID: 220
		public static Action<NavalOrderOfBattleFormationItemVM> OnAcceptCaptain;

		// Token: 0x040000DD RID: 221
		public static Action<NavalOrderOfBattleFormationItemVM> OnAcceptShip;

		// Token: 0x040000DE RID: 222
		public static Func<DeploymentFormationClass, FormationFilterType, int> GetTotalTroopCountWithFilter;

		// Token: 0x040000DF RID: 223
		private readonly TextObject _captainSlotHintText = new TextObject("{=shipcaptain}Captain", null);

		// Token: 0x040000E0 RID: 224
		private readonly TextObject _shipSlotHintText = new TextObject("{=1nbU1tV5}Ship", null);

		// Token: 0x040000E1 RID: 225
		private readonly TextObject _assignCaptainHintText = new TextObject("{=rHEi6aVz}Assign as Captain", null);

		// Token: 0x040000E2 RID: 226
		private readonly TextObject _assignShipHintText = new TextObject("{=6o2JKNbt}Assign as Ship", null);

		// Token: 0x040000E3 RID: 227
		private readonly TextObject _infantryHintText = new TextObject("{=IxI1HecC}Give preference to infantry troops", null);

		// Token: 0x040000E4 RID: 228
		private readonly TextObject _rangedHintText = new TextObject("{=I9X4VvhG}Give preference to ranged troops", null);

		// Token: 0x040000E5 RID: 229
		private readonly TextObject _infantryAndRangedHintText = new TextObject("{=e9nO59x4}Give equal preference to infantry and ranged troops", null);

		// Token: 0x040000E6 RID: 230
		private readonly TextObject _filteredTroopCountInfoText = new TextObject("{=yRIPADWl}{TROOP_COUNT}/{TOTAL_TROOP_COUNT}", null);

		// Token: 0x040000E8 RID: 232
		private bool _isSelected;

		// Token: 0x040000E9 RID: 233
		private bool _isEnabled;

		// Token: 0x040000EA RID: 234
		private bool _isSelectable;

		// Token: 0x040000EB RID: 235
		private bool _hasCaptain;

		// Token: 0x040000EC RID: 236
		private bool _hasShip;

		// Token: 0x040000ED RID: 237
		private bool _isAcceptingCaptain;

		// Token: 0x040000EE RID: 238
		private bool _isAcceptingShip;

		// Token: 0x040000EF RID: 239
		private bool _isInfantrySelected;

		// Token: 0x040000F0 RID: 240
		private bool _isRangedSelected;

		// Token: 0x040000F1 RID: 241
		private bool _isInfantryAndRangedSelected;

		// Token: 0x040000F2 RID: 242
		private string _formationName;

		// Token: 0x040000F3 RID: 243
		private string _formationIsEmptyText;

		// Token: 0x040000F4 RID: 244
		private int _troopCount;

		// Token: 0x040000F5 RID: 245
		private int _formationClassInt;

		// Token: 0x040000F6 RID: 246
		private bool _isSkeletalCrewCountWarningActive;

		// Token: 0x040000F7 RID: 247
		private string _skeletalCrewCountWarning;

		// Token: 0x040000F8 RID: 248
		private HintViewModel _captainSlotHint;

		// Token: 0x040000F9 RID: 249
		private HintViewModel _shipSlotHint;

		// Token: 0x040000FA RID: 250
		private HintViewModel _assignCaptainHint;

		// Token: 0x040000FB RID: 251
		private HintViewModel _assignShipHint;

		// Token: 0x040000FC RID: 252
		private NavalOrderOfBattleHeroItemVM _captain;

		// Token: 0x040000FD RID: 253
		private NavalOrderOfBattleShipItemVM _ship;

		// Token: 0x040000FE RID: 254
		private int _wSign;

		// Token: 0x040000FF RID: 255
		private Vec2 _screenPosition;

		// Token: 0x04000100 RID: 256
		private MBBindingList<OrderOfBattleFormationFilterSelectorItemVM> _filterItems;

		// Token: 0x04000101 RID: 257
		private HintViewModel _infantryHint;

		// Token: 0x04000102 RID: 258
		private HintViewModel _rangedHint;

		// Token: 0x04000103 RID: 259
		private HintViewModel _infantryAndRangedHint;

		// Token: 0x04000104 RID: 260
		private HintViewModel _disabledHint;

		// Token: 0x04000105 RID: 261
		private BasicTooltipViewModel _tooltip;
	}
}
