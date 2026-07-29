using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using FormationFilter.Models;
using FormationFilter.Utilities;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;

namespace FormationFilter.View.ViewModels
{
	// Token: 0x0200000D RID: 13
	[NullableContext(1)]
	[Nullable(new byte[] { 0, 1 })]
	[ViewModelMixin]
	public class CustomFormationClassVM : BaseViewModelMixin<OrderOfBattleFormationClassVM>
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000040 RID: 64 RVA: 0x0000291B File Offset: 0x00000B1B
		[Nullable(2)]
		public Formation Formation
		{
			[NullableContext(2)]
			get
			{
				CustomFormationItemVM belongedFormationItem = this.BelongedFormationItem;
				if (belongedFormationItem == null)
				{
					return null;
				}
				return belongedFormationItem.Formation;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000041 RID: 65 RVA: 0x0000292E File Offset: 0x00000B2E
		[Nullable(2)]
		private TeamFilter TeamFilter
		{
			[NullableContext(2)]
			get
			{
				CustomFormationItemVM belongedFormationItem = this.BelongedFormationItem;
				if (belongedFormationItem == null)
				{
					return null;
				}
				return belongedFormationItem.TeamFilter;
			}
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00002944 File Offset: 0x00000B44
		static CustomFormationClassVM()
		{
			Harmony harmony = new Harmony("FormationFilter");
			harmony.Patch(typeof(OrderOfBattleFormationClassVM).GetMethod("UpdateTroopCountText", BindingFlags.Instance | BindingFlags.Public), new HarmonyMethod(typeof(CustomFormationClassVM).GetMethod("Prefix_UpdateTroopCountText", BindingFlags.Static | BindingFlags.Public)), null, null, null);
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000029A4 File Offset: 0x00000BA4
		public static bool Prefix_UpdateTroopCountText(OrderOfBattleFormationClassVM __instance)
		{
			CustomFormationClassVM customFormationClassVM = CustomFormationClassVM.GetCustomFormationClassVM(__instance);
			if (customFormationClassVM == null)
			{
				return true;
			}
			CustomFormationClassVM customFormationClassVM2 = CustomFormationClassVM.GetCustomFormationClassVM(__instance);
			TeamFilter teamFilter = ((customFormationClassVM2 != null) ? customFormationClassVM2.TeamFilter : null);
			if (teamFilter == null)
			{
				__instance.TroopCountText = string.Empty;
				return false;
			}
			TroopFilter troopFilter = CustomFormationClassVM.GetTroopFilter(teamFilter, __instance);
			if (__instance.Class != 10 && troopFilter != null)
			{
				string text = GameTexts.FindText("str_LEFT_comma_RIGHT", null).SetTextVariable("LEFT", customFormationClassVM.GetCountOfMinimumUnitsInClass().ToString()).SetTextVariable("RIGHT", customFormationClassVM.GetTotalTroopCountOfFilter().ToString())
					.ToString();
				__instance.TroopCountText = GameTexts.FindText("str_LEFT_over_RIGHT", null).SetTextVariable("LEFT", customFormationClassVM.GetCountOfRealUnitsInClass()).SetTextVariable("RIGHT", "[" + text + "]")
					.ToString();
			}
			else
			{
				__instance.TroopCountText = string.Empty;
			}
			return false;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00002A8C File Offset: 0x00000C8C
		[return: Nullable(2)]
		public static CustomFormationClassVM GetCustomFormationClassVM(OrderOfBattleFormationClassVM formationClassVM)
		{
			CustomFormationClassVM customFormationClassVM;
			if (CustomFormationClassVM._mixinReverseDictionary.TryGetValue(formationClassVM, out customFormationClassVM))
			{
				return customFormationClassVM;
			}
			return null;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00002AAC File Offset: 0x00000CAC
		public CustomFormationClassVM(OrderOfBattleFormationClassVM vm)
			: base(vm)
		{
			CustomFormationClassVM._mixinReverseDictionary.Add(vm, this);
			CustomFormationItemVM customFormationItemVM = CustomFormationItemVM.GetCustomFormationItemVM(vm.BelongedFormationItem);
			this.BelongedFormationItem = customFormationItemVM;
			FormationClass? formationClass;
			if (customFormationItemVM == null)
			{
				formationClass = null;
			}
			else
			{
				Formation formation = customFormationItemVM.Formation;
				formationClass = ((formation != null) ? new FormationClass?(formation.FormationIndex) : null);
			}
			FormationClass? formationClass2 = formationClass;
			this._formationIndex = formationClass2.GetValueOrDefault(10);
			this._customFilterItems = new MBBindingList<CustomFilterSelectorItemVM>();
			for (CustomFormationFilterType customFormationFilterType = CustomFormationFilterType.OneHanded; customFormationFilterType < CustomFormationFilterType.NumberOfFilterTypes; customFormationFilterType++)
			{
				this.CustomFilterItems.Add(new CustomFilterSelectorItemVM(customFormationFilterType, new Action<CustomFilterSelectorItemVM>(this.OnCustomFilterToggled), FilterSelectorMode.Both));
			}
			foreach (CustomFilterSelectorItemVM customFilterSelectorItemVM in this.CustomFilterItems)
			{
				customFilterSelectorItemVM.IsEnabled = customFormationItemVM != null && customFormationItemVM.IsControlledByPlayer;
			}
			this._formationClassSelector = new SelectorVM<OrderOfBattleFormationClassSelectorItemVM>(0, new Action<SelectorVM<OrderOfBattleFormationClassSelectorItemVM>>(this.OnClassChangedFromInteractiveUI));
			for (DeploymentFormationClass deploymentFormationClass = 0; deploymentFormationClass <= 4; deploymentFormationClass++)
			{
				if (!Mission.Current.IsSiegeBattle || (deploymentFormationClass != 3 && deploymentFormationClass != 4 && deploymentFormationClass != 6))
				{
					this.FormationClassSelector.AddItem(new OrderOfBattleFormationClassSelectorItemVM(deploymentFormationClass));
				}
			}
			this.RestrictFormationClassSelectors(Mission.Current.IsSiegeBattle ? FormationClassSelectorMode.FootmanOnly : FormationClassSelectorMode.All);
			this.FormationClassSelector.SelectedIndex = 0;
			this.OnCustomWeightAdjusted();
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00002C30 File Offset: 0x00000E30
		public void RestrictFormationClassSelectors(FormationClassSelectorMode formationClassSelectorMode)
		{
			OrderOfBattleFormationClassSelectorItemVM selectedItem = this.FormationClassSelector.SelectedItem;
			this.FormationClassSelector.ItemList.Clear();
			for (DeploymentFormationClass deploymentFormationClass = 0; deploymentFormationClass <= 4; deploymentFormationClass++)
			{
				if (deploymentFormationClass == null || formationClassSelectorMode == FormationClassSelectorMode.All || (formationClassSelectorMode == FormationClassSelectorMode.FootmanOnly && (deploymentFormationClass == 1 || deploymentFormationClass == 2)) || (formationClassSelectorMode == FormationClassSelectorMode.HorsemanOnly && (deploymentFormationClass == 3 || deploymentFormationClass == 4)))
				{
					this.FormationClassSelector.AddItem(new OrderOfBattleFormationClassSelectorItemVM(deploymentFormationClass));
				}
			}
			this.FormationClassSelector.SelectedItem = selectedItem;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00002CA0 File Offset: 0x00000EA0
		public void OnClassChangedFromInteractiveUI(SelectorVM<OrderOfBattleFormationClassSelectorItemVM> formationClassSelector)
		{
			this.OnClassChanged(formationClassSelector, true);
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00002CAA File Offset: 0x00000EAA
		public void OnClassChangedFromDataModel(SelectorVM<OrderOfBattleFormationClassSelectorItemVM> formationClassSelector)
		{
			this.OnClassChanged(formationClassSelector, false);
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00002CB4 File Offset: 0x00000EB4
		public void OnClassChanged(SelectorVM<OrderOfBattleFormationClassSelectorItemVM> formationClassSelector, bool isFromInteractiveUI)
		{
			if (this.TeamFilter == null || this.BelongedFormationItem == null || this.Formation == null || base.ViewModel == null)
			{
				return;
			}
			OrderOfBattleFormationClassSelectorItemVM selectedItem = formationClassSelector.SelectedItem;
			DeploymentFormationClass deploymentFormationClass = ((selectedItem != null) ? selectedItem.FormationClass : 0);
			FormationClass formationClass = Utility.ToFormationClass(deploymentFormationClass);
			if (base.ViewModel.Class == formationClass)
			{
				return;
			}
			TroopFilter troopFilter = this.GetTroopFilter();
			if (troopFilter == null)
			{
				FormationFilters formationFilters = this.TeamFilter.GetFormationFilters(this.BelongedFormationItem.Formation);
				MBBindingList<OrderOfBattleFormationClassVM> classes = this.BelongedFormationItem.Classes;
				int num = ((classes != null) ? classes.IndexOf(base.ViewModel) : (-1));
				if (num == -1 || formationFilters == null)
				{
					return;
				}
				troopFilter = formationFilters.ForceGetTroopFilterAtIndex(num, formationClass);
				troopFilter.SetBasicFilterForFormationClass(formationClass);
			}
			if (deploymentFormationClass == null)
			{
				base.ViewModel.IsUnset = true;
				base.ViewModel.IsLocked = false;
				if (isFromInteractiveUI)
				{
					troopFilter.SetBasicFilterForDeploymentFormationClass(deploymentFormationClass);
					troopFilter.SetWeight(0f);
				}
			}
			else
			{
				base.ViewModel.IsUnset = false;
				base.ViewModel.IsLocked = false;
				if (isFromInteractiveUI)
				{
					troopFilter.SetBasicFilterForDeploymentFormationClass(deploymentFormationClass);
					troopFilter.SetWeight(1f);
				}
			}
			this.TeamFilter.OnFormationFilterUpdated();
			base.ViewModel.Class = formationClass;
			this.IsFormationClassValid = formationClass != 10;
			this.UpdateFiltersFromTeamFilter();
			CustomFormationItemVM belongedFormationItem = this.BelongedFormationItem;
			if (belongedFormationItem == null)
			{
				return;
			}
			belongedFormationItem.OnClassChanged(isFromInteractiveUI);
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00002E00 File Offset: 0x00001000
		public override void OnFinalize()
		{
			base.OnFinalize();
			this._customFilterItems.Clear();
			if (base.ViewModel != null)
			{
				CustomFormationClassVM._mixinReverseDictionary.Remove(base.ViewModel);
			}
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00002E2C File Offset: 0x0000102C
		[NullableContext(2)]
		private TroopFilter GetTroopFilter()
		{
			return CustomFormationClassVM.GetTroopFilter(this.TeamFilter, this);
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00002E3C File Offset: 0x0000103C
		[NullableContext(2)]
		private static TroopFilter GetTroopFilter(TeamFilter teamFilter, [Nullable(1)] OrderOfBattleFormationClassVM formationClassVM)
		{
			OrderOfBattleFormationItemVM belongedFormationItem = formationClassVM.BelongedFormationItem;
			CustomFormationItemVM customFormationItemVM = CustomFormationItemVM.GetCustomFormationItemVM(belongedFormationItem);
			if (teamFilter == null || customFormationItemVM == null || customFormationItemVM.Formation == null)
			{
				return null;
			}
			int num = belongedFormationItem.Classes.IndexOf(formationClassVM);
			if (num == -1)
			{
				return null;
			}
			FormationFilters formationFilters = teamFilter.GetFormationFilters(customFormationItemVM.Formation);
			if (((formationFilters != null) ? formationFilters.Filters : null) == null || formationFilters.Filters.Count <= num)
			{
				return null;
			}
			return formationFilters.Filters[num];
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00002EB4 File Offset: 0x000010B4
		[NullableContext(2)]
		private static TroopFilter GetTroopFilter(TeamFilter teamFilter, [Nullable(1)] CustomFormationClassVM vm)
		{
			CustomFormationItemVM belongedFormationItem = vm.BelongedFormationItem;
			if (teamFilter == null || belongedFormationItem == null || belongedFormationItem.Formation == null || vm.ViewModel == null)
			{
				return null;
			}
			MBBindingList<OrderOfBattleFormationClassVM> classes = belongedFormationItem.Classes;
			int num = ((classes != null) ? classes.IndexOf(vm.ViewModel) : (-1));
			if (num == -1)
			{
				return null;
			}
			FormationFilters formationFilters = teamFilter.GetFormationFilters(belongedFormationItem.Formation);
			if (((formationFilters != null) ? formationFilters.Filters : null) == null || formationFilters.Filters.Count <= num)
			{
				return null;
			}
			return formationFilters.Filters[num];
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00002F38 File Offset: 0x00001138
		private void OnCustomWeightAdjusted()
		{
			if (base.ViewModel == null)
			{
				return;
			}
			TroopFilter troopFilter = this.GetTroopFilter();
			if (troopFilter == null)
			{
				return;
			}
			if (troopFilter.Weight == this._customWeight)
			{
				return;
			}
			troopFilter.SetWeight(this._customWeight);
			TeamFilter.IsAdustingWeights = true;
			Action<OrderOfBattleFormationClassVM> onWeightAdjustedCallback = OrderOfBattleFormationClassVM.OnWeightAdjustedCallback;
			if (onWeightAdjustedCallback != null)
			{
				onWeightAdjustedCallback(base.ViewModel);
			}
			TeamFilter.IsAdustingWeights = false;
			base.ViewModel.UpdateTroopCountText();
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00002FA4 File Offset: 0x000011A4
		private int GetCountOfRealUnitsInClass()
		{
			CustomFormationItemVM belongedFormationItem = this.BelongedFormationItem;
			if (((belongedFormationItem != null) ? belongedFormationItem.Classes : null) == null || base.ViewModel == null)
			{
				return 0;
			}
			int num = this.BelongedFormationItem.Classes.IndexOf(base.ViewModel);
			if (this.TeamFilter == null || this.Formation == null)
			{
				return 0;
			}
			return this.TeamFilter.GetUnitCountOfTroopFilter(new TroopFilterIdentifier(this.Formation, num));
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00003010 File Offset: 0x00001210
		private int GetCountOfMinimumUnitsInClass()
		{
			TroopFilter troopFilter = this.GetTroopFilter();
			if (troopFilter == null || this.TeamFilter == null)
			{
				return 0;
			}
			return this.TeamFilter.GetMinimumTroopCountOfFilter(troopFilter.Bitmask);
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00003044 File Offset: 0x00001244
		public int GetTotalTroopCountOfFilter()
		{
			TroopFilter troopFilter = this.GetTroopFilter();
			if (troopFilter == null || this.TeamFilter == null)
			{
				return 0;
			}
			return this.TeamFilter.GetTotalTroopCountOfFilter(troopFilter.Bitmask);
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003078 File Offset: 0x00001278
		public void UpdateFiltersFromTeamFilter()
		{
			if (this.TeamFilter == null || this.Formation == null)
			{
				return;
			}
			TroopFilter troopFilter = this.GetTroopFilter();
			if (troopFilter == null)
			{
				foreach (CustomFilterSelectorItemVM customFilterSelectorItemVM in this.CustomFilterItems)
				{
					customFilterSelectorItemVM.UpdateFilterValueFromTeamFilter(FilterValueEnum.Any);
				}
				this.UpdateWeightFromTeamFilter(0f);
				return;
			}
			this.UpdateDeploymentFormationClassFromTeamFilter();
			for (CustomFormationFilterType customFormationFilterType = CustomFormationFilterType.OneHanded; customFormationFilterType < CustomFormationFilterType.NumberOfFilterTypes; customFormationFilterType++)
			{
				CustomFilterSelectorItemVM customFilterSelectorItemVM2 = this.CustomFilterItems[customFormationFilterType - CustomFormationFilterType.OneHanded];
				FilterValueEnum filter = troopFilter.GetFilter(customFormationFilterType.ToFilterType());
				customFilterSelectorItemVM2.UpdateFilterValueFromTeamFilter(filter);
			}
			this.UpdateWeightFromTeamFilter(troopFilter.Weight);
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00003134 File Offset: 0x00001334
		private void UpdateDeploymentFormationClassFromTeamFilter()
		{
			if (this.TeamFilter == null || this.Formation == null)
			{
				return;
			}
			TroopFilter troopFilter = this.GetTroopFilter();
			if (troopFilter == null)
			{
				return;
			}
			FormationClass formationClass = troopFilter.DetectBasicFomrationClass();
			DeploymentFormationClass deploymentFormationClass = Utility.ToDeploymentFormationClass(formationClass);
			if (Mission.Current.IsSiegeBattle)
			{
				deploymentFormationClass = Utility.ToFootmanDeploymentFormationClass(deploymentFormationClass);
			}
			int num = Extensions.FindIndex<OrderOfBattleFormationClassSelectorItemVM>(this.FormationClassSelector.ItemList, (OrderOfBattleFormationClassSelectorItemVM item) => item.FormationClass == deploymentFormationClass);
			if (num != -1)
			{
				this.FormationClassSelector.SetOnChangeAction(new Action<SelectorVM<OrderOfBattleFormationClassSelectorItemVM>>(this.OnClassChangedFromDataModel));
				this.FormationClassSelector.SelectedIndex = num;
				this.FormationClassSelector.SetOnChangeAction(new Action<SelectorVM<OrderOfBattleFormationClassSelectorItemVM>>(this.OnClassChangedFromInteractiveUI));
				return;
			}
			this.FormationClassSelector.SetOnChangeAction(new Action<SelectorVM<OrderOfBattleFormationClassSelectorItemVM>>(this.OnClassChangedFromDataModel));
			this.FormationClassSelector.SelectedIndex = 0;
			this.FormationClassSelector.SetOnChangeAction(new Action<SelectorVM<OrderOfBattleFormationClassSelectorItemVM>>(this.OnClassChangedFromInteractiveUI));
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00003229 File Offset: 0x00001429
		private void UpdateWeightFromTeamFilter(float weight)
		{
			this._customWeight = weight;
			base.OnPropertyChangedWithValue(weight, "CustomWeight");
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00003243 File Offset: 0x00001443
		public bool HasAnyActiveFitlers()
		{
			return this._customFilterItems.Any<CustomFilterSelectorItemVM>((CustomFilterSelectorItemVM item) => item.IsSelected);
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00003270 File Offset: 0x00001470
		public List<TooltipProperty> GetFilterTooltips()
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			CustomFormationItemVM belongedFormationItem = this.BelongedFormationItem;
			if (((belongedFormationItem != null) ? belongedFormationItem.Classes : null) == null || base.ViewModel == null)
			{
				return list;
			}
			FormationClass @class = base.ViewModel.Class;
			if (@class == 10)
			{
				return list;
			}
			int num = this.BelongedFormationItem.Classes.IndexOf(base.ViewModel);
			if (num < 0)
			{
				return list;
			}
			if (this.Formation == null || this.TeamFilter == null)
			{
				return list;
			}
			list.Add(new TooltipProperty(string.Empty, string.Empty, 0, false, 1024));
			TextObject textObject = GameTexts.FindText("str_formation_filter_squad_name", null).SetTextVariable("NUMBER", num + 1);
			string text = "str_troop_group_name";
			int num2 = @class;
			TextObject textObject2 = GameTexts.FindText(text, num2.ToString());
			TextObject textObject3 = GameTexts.FindText("str_STR1_space_STR2", null).SetTextVariable("STR1", textObject).SetTextVariable("STR2", textObject2);
			int countOfRealUnitsInClass = this.GetCountOfRealUnitsInClass();
			int totalTroopCountOfFilter = this.GetTotalTroopCountOfFilter();
			string text2 = new TextObject("{=9pCzjSTa}{PERCENTAGE}% of troop type", null).SetTextVariable("PERCENTAGE", MathF.Round((totalTroopCountOfFilter == 0) ? 0f : (100f * (float)countOfRealUnitsInClass / (float)totalTroopCountOfFilter))).ToString();
			string text3 = GameTexts.FindText("str_RANK_with_NUM_between_parenthesis", null).SetTextVariable("RANK", countOfRealUnitsInClass.ToString()).SetTextVariable("NUMBER", text2)
				.ToString();
			list.Add(new TooltipProperty(textObject3.ToString(), text3, 0, new Color(0.8f, 0.5f, 0.2f, 1f), false, 0));
			IOrderedEnumerable<KeyValuePair<TroopFilterIdentifier, int>> orderedEnumerable = from pair in this.TeamFilter.GetUnitCountOfRelatedTroopFilters(new TroopFilterIdentifier(this.Formation, num)).ToList<KeyValuePair<TroopFilterIdentifier, int>>()
				orderby pair.Value descending
				select pair;
			foreach (KeyValuePair<TroopFilterIdentifier, int> keyValuePair in orderedEnumerable)
			{
				TroopFilterIdentifier key = keyValuePair.Key;
				int value = keyValuePair.Value;
				int num3 = MathF.Round((totalTroopCountOfFilter == 0) ? 0f : (100f * (float)value / (float)totalTroopCountOfFilter));
				TextObject textObject4 = new TextObject("{=9pCzjSTa}{PERCENTAGE}% of troop type", null).SetTextVariable("PERCENTAGE", num3);
				string text4 = GameTexts.FindText("str_RANK_with_NUM_between_parenthesis", null).SetTextVariable("RANK", value.ToString()).SetTextVariable("NUMBER", textObject4)
					.ToString();
				string text5 = new TextObject("{=cZNA5Z6l}Formation {NUMBER}", null).SetTextVariable("NUMBER", key.Formation.Index + 1).ToString();
				string text6 = GameTexts.FindText("str_formation_filter_squad_name", null).SetTextVariable("NUMBER", key.Index + 1).ToString();
				string text7 = GameTexts.FindText("str_STR1_space_STR2", null).SetTextVariable("STR1", text5).SetTextVariable("STR2", text6)
					.ToString();
				list.Add(new TooltipProperty(text7, text4, 0, new Color(0.6f, 0.4f, 0.1f, 1f), true, 0));
			}
			list.Add(new TooltipProperty(string.Empty, string.Empty, -1, false, 0));
			for (CustomFormationFilterType customFormationFilterType = CustomFormationFilterType.OneHanded; customFormationFilterType < CustomFormationFilterType.NumberOfFilterTypes; customFormationFilterType++)
			{
				CustomFilterSelectorItemVM customFilterSelectorItemVM = this.CustomFilterItems[customFormationFilterType - CustomFormationFilterType.OneHanded];
				if (customFilterSelectorItemVM.IsSelected && this.Formation != null)
				{
					string text8 = customFormationFilterType.ToString();
					FilterValueEnum filterValue = this.GetFilterValue(customFilterSelectorItemVM);
					list.Add(new TooltipProperty(customFormationFilterType.GetFilterName().ToString(), filterValue.GetFilterValueName().ToString(), 0, false, 0));
				}
			}
			return list;
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00003634 File Offset: 0x00001834
		private void OnCustomFilterToggled(CustomFilterSelectorItemVM filterItem)
		{
			CustomFormationItemVM belongedFormationItem = this.BelongedFormationItem;
			if (((belongedFormationItem != null) ? belongedFormationItem.Classes : null) == null || base.ViewModel == null || base.ViewModel.Class == 10 || this.TeamFilter == null || this.Formation == null)
			{
				return;
			}
			int num = this.BelongedFormationItem.Classes.IndexOf(base.ViewModel);
			if (num == -1)
			{
				return;
			}
			FilterTypeEnum filterTypeEnum = filterItem.FilterType.ToFilterType();
			FilterValueEnum filterValue = this.GetFilterValue(filterItem);
			this.TeamFilter.UpdateFormationFilter(this.Formation, num, filterTypeEnum, filterValue);
			CustomFormationItemVM belongedFormationItem2 = this.BelongedFormationItem;
			if (belongedFormationItem2 == null)
			{
				return;
			}
			belongedFormationItem2.OnCustomFilterToggled();
		}

		// Token: 0x06000058 RID: 88 RVA: 0x000036D1 File Offset: 0x000018D1
		private FilterValueEnum GetFilterValue(CustomFilterSelectorItemVM filterItem)
		{
			if (!filterItem.IsEnabled)
			{
				return FilterValueEnum.Any;
			}
			if (filterItem.IsIncluded)
			{
				return FilterValueEnum.Yes;
			}
			if (!filterItem.IsExcluded)
			{
				return FilterValueEnum.Any;
			}
			return FilterValueEnum.No;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000036F2 File Offset: 0x000018F2
		public void OnClick()
		{
			if (this.BelongedFormationItem == null || base.ViewModel == null)
			{
				return;
			}
			this.BelongedFormationItem.SetAsCurrentFormationClass(base.ViewModel);
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600005A RID: 90 RVA: 0x00003716 File Offset: 0x00001916
		// (set) Token: 0x0600005B RID: 91 RVA: 0x0000371E File Offset: 0x0000191E
		[DataSourceProperty]
		public SelectorVM<OrderOfBattleFormationClassSelectorItemVM> FormationClassSelector
		{
			get
			{
				return this._formationClassSelector;
			}
			set
			{
				if (value == this._formationClassSelector)
				{
					return;
				}
				this._formationClassSelector = value;
				base.OnPropertyChangedWithValue(value, "FormationClassSelector");
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600005C RID: 92 RVA: 0x0000373D File Offset: 0x0000193D
		// (set) Token: 0x0600005D RID: 93 RVA: 0x00003745 File Offset: 0x00001945
		[DataSourceProperty]
		public float CustomWeight
		{
			get
			{
				return this._customWeight;
			}
			set
			{
				if (value == this._customWeight)
				{
					return;
				}
				this._customWeight = value;
				base.OnPropertyChangedWithValue(value, "CustomWeight");
				this.OnCustomWeightAdjusted();
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600005E RID: 94 RVA: 0x0000376F File Offset: 0x0000196F
		// (set) Token: 0x0600005F RID: 95 RVA: 0x00003777 File Offset: 0x00001977
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
				base.OnPropertyChangedWithValue(value, "CustomFilterItems");
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000060 RID: 96 RVA: 0x00003796 File Offset: 0x00001996
		// (set) Token: 0x06000061 RID: 97 RVA: 0x0000379E File Offset: 0x0000199E
		[DataSourceProperty]
		public bool IsFormationClassValid
		{
			get
			{
				return this._isFormationClassValid;
			}
			set
			{
				if (value == this._isFormationClassValid)
				{
					return;
				}
				this._isFormationClassValid = value;
				base.OnPropertyChangedWithValue(value, "IsFormationClassValid");
			}
		}

		// Token: 0x04000038 RID: 56
		private static Dictionary<OrderOfBattleFormationClassVM, CustomFormationClassVM> _mixinReverseDictionary = new Dictionary<OrderOfBattleFormationClassVM, CustomFormationClassVM>();

		// Token: 0x04000039 RID: 57
		private FormationClass _formationIndex;

		// Token: 0x0400003A RID: 58
		[Nullable(2)]
		public readonly CustomFormationItemVM BelongedFormationItem;

		// Token: 0x0400003B RID: 59
		private SelectorVM<OrderOfBattleFormationClassSelectorItemVM> _formationClassSelector;

		// Token: 0x0400003C RID: 60
		private MBBindingList<CustomFilterSelectorItemVM> _customFilterItems;

		// Token: 0x0400003D RID: 61
		private float _customWeight;

		// Token: 0x0400003E RID: 62
		public DeploymentFormationClass _class;

		// Token: 0x0400003F RID: 63
		private bool _isFormationClassValid;

		// Token: 0x04000040 RID: 64
		private readonly TextObject _filteredTroopCountInfoText = new TextObject("{=yRIPADWl}{TROOP_COUNT}/{TOTAL_TROOP_COUNT}", null);
	}
}
