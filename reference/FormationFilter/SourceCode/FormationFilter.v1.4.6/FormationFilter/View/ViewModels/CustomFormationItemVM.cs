using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using FormationFilter.Logics;
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
using TaleWorlds.TwoDimension;

namespace FormationFilter.View.ViewModels
{
	// Token: 0x0200000E RID: 14
	[NullableContext(1)]
	[Nullable(new byte[] { 0, 1 })]
	[ViewModelMixin]
	public class CustomFormationItemVM : BaseViewModelMixin<OrderOfBattleFormationItemVM>
	{
		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000062 RID: 98 RVA: 0x000037C2 File Offset: 0x000019C2
		[Nullable(2)]
		public Formation Formation
		{
			[NullableContext(2)]
			get
			{
				OrderOfBattleFormationItemVM viewModel = base.ViewModel;
				if (viewModel == null)
				{
					return null;
				}
				return viewModel.Formation;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000063 RID: 99 RVA: 0x000037D5 File Offset: 0x000019D5
		[Nullable(new byte[] { 2, 1 })]
		public MBBindingList<OrderOfBattleFormationClassVM> Classes
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get
			{
				OrderOfBattleFormationItemVM viewModel = base.ViewModel;
				if (viewModel == null)
				{
					return null;
				}
				return viewModel.Classes;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000064 RID: 100 RVA: 0x000037E8 File Offset: 0x000019E8
		public bool IsControlledByPlayer
		{
			get
			{
				OrderOfBattleFormationItemVM viewModel = base.ViewModel;
				return viewModel != null && viewModel.IsControlledByPlayer;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000065 RID: 101 RVA: 0x000037FB File Offset: 0x000019FB
		// (set) Token: 0x06000066 RID: 102 RVA: 0x00003803 File Offset: 0x00001A03
		[Nullable(2)]
		public TeamFilter TeamFilter
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x0000380C File Offset: 0x00001A0C
		static CustomFormationItemVM()
		{
			Harmony harmony = new Harmony("FormationFilter");
			harmony.Patch(typeof(OrderOfBattleFormationItemVM).GetMethod("OnSizeChanged", BindingFlags.Instance | BindingFlags.Public), new HarmonyMethod(typeof(CustomFormationItemVM).GetMethod("Prefix_OnSizeChanged", BindingFlags.Static | BindingFlags.Public)), null, null, null);
			harmony.Patch(typeof(OrderOfBattleFormationItemVM).GetMethod("GetOrderOfBattleClass", BindingFlags.Instance | BindingFlags.Public), new HarmonyMethod(typeof(CustomFormationItemVM).GetMethod("Prefix_GetOrderOfBattleClass", BindingFlags.Static | BindingFlags.Public)), null, null, null);
			harmony.Patch(typeof(OrderOfBattleFormationItemVM).GetMethod("OnClassChanged", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(CustomFormationItemVM).GetMethod("Prefix_OnClassChanged", BindingFlags.Static | BindingFlags.Public)), null, null, null);
			harmony.Patch(typeof(OrderOfBattleFormationItemVM).GetMethod("RefreshFormation", BindingFlags.Instance | BindingFlags.Public), new HarmonyMethod(typeof(CustomFormationItemVM).GetMethod("Prefix_RefreshFormation", BindingFlags.Static | BindingFlags.Public)), null, null, null);
			harmony.Patch(typeof(OrderOfBattleFormationItemVM).GetMethod("GetTooltip", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(CustomFormationItemVM).GetMethod("Prefix_GetTooltip", BindingFlags.Static | BindingFlags.Public)), null, null, null);
			harmony.Patch(typeof(OrderOfBattleFormationItemVM).GetMethod("UpdateAdjustable", BindingFlags.Instance | BindingFlags.Public), new HarmonyMethod(typeof(CustomFormationItemVM).GetMethod("Prefix_UpdateAdjustable", BindingFlags.Static | BindingFlags.Public)), null, null, null);
		}

		// Token: 0x06000068 RID: 104 RVA: 0x000039C4 File Offset: 0x00001BC4
		public static bool Prefix_OnSizeChanged(OrderOfBattleFormationItemVM __instance, BannerBearerLogic ____bannerBearerLogic)
		{
			Formation formation = __instance.Formation;
			__instance.TroopCount = ((formation != null) ? formation.CountOfUnits : 0);
			__instance.BannerBearerCount = ((__instance.Formation != null) ? ____bannerBearerLogic.GetFormationBannerBearers(__instance.Formation).Count : 0);
			CustomFormationItemVM._refreshMarkerWorldPosition.Invoke(__instance, new object[0]);
			__instance.IsSelectable = __instance.OrderOfBattleFormationClassInt != 0 && __instance.IsControlledByPlayer && __instance.TroopCount > 0;
			if (!__instance.IsSelectable && __instance.IsSelected)
			{
				Action<OrderOfBattleFormationItemVM> onDeselection = OrderOfBattleFormationItemVM.OnDeselection;
				if (onDeselection != null)
				{
					onDeselection(__instance);
				}
			}
			foreach (OrderOfBattleFormationClassVM orderOfBattleFormationClassVM in __instance.Classes)
			{
				orderOfBattleFormationClassVM.UpdateTroopCountText();
			}
			__instance.UpdateAdjustable();
			return false;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00003AA8 File Offset: 0x00001CA8
		public static bool Prefix_GetOrderOfBattleClass(OrderOfBattleFormationItemVM __instance, ref DeploymentFormationClass __result)
		{
			__result = CustomFormationItemVM.GetDeploymentFormationClass(__instance);
			return false;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00003AB3 File Offset: 0x00001CB3
		public static bool Prefix_OnClassChanged(OrderOfBattleFormationItemVM __instance, SelectorVM<OrderOfBattleFormationClassSelectorItemVM> formationClassSelector)
		{
			return false;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00003AB8 File Offset: 0x00001CB8
		public static bool Prefix_RefreshFormation(OrderOfBattleFormationItemVM __instance, Formation formation, DeploymentFormationClass overriddenClass = 0, bool mustExist = false)
		{
			CustomFormationItemVM._formation.SetValue(__instance, formation);
			if (formation.CountOfUnits != 0 || mustExist)
			{
				DeploymentFormationClass deploymentFormationClass = 0;
				if (overriddenClass != null)
				{
					deploymentFormationClass = overriddenClass;
				}
				else
				{
					FormationClass formationClass = 10;
					if (formation.SecondaryLogicalClasses.Count<FormationClass>() > 0)
					{
						formationClass = formation.SecondaryLogicalClasses.FirstOrDefault<FormationClass>();
						if (formation.GetCountOfUnitsBelongingToLogicalClass(formationClass) == 0)
						{
							formationClass = 10;
						}
					}
					switch (formation.LogicalClass)
					{
					case 0:
						deploymentFormationClass = ((formationClass == 1) ? 5 : 1);
						break;
					case 1:
						deploymentFormationClass = ((formationClass == null) ? 5 : 2);
						break;
					case 2:
						deploymentFormationClass = ((formationClass == 3) ? 6 : 3);
						break;
					case 3:
						deploymentFormationClass = ((formationClass == 2) ? 6 : 4);
						break;
					}
				}
				CustomFormationItemVM.ForceSetToFormationClass(__instance, deploymentFormationClass);
			}
			else
			{
				CustomFormationItemVM.ForceSetToFormationClass(__instance, 0);
			}
			__instance.TitleText = (__instance.Formation.Index + 1).ToString();
			__instance.OnSizeChanged();
			return false;
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00003B90 File Offset: 0x00001D90
		private static void ForceSetToFormationClass(OrderOfBattleFormationItemVM __instance, DeploymentFormationClass deploymentFormationClass)
		{
			CustomFormationItemVM customFormationItemVM = CustomFormationItemVM.GetCustomFormationItemVM(__instance);
			if (customFormationItemVM == null)
			{
				return;
			}
			TeamFilter teamFilter = Mission.Current.GetMissionBehavior<FormationFilterLogic>().GetTeamFilter(__instance.Formation.Team);
			if (teamFilter == null)
			{
				return;
			}
			FormationFilters formationFilters = teamFilter.GetFormationFilters(__instance.Formation);
			if (formationFilters == null)
			{
				formationFilters = teamFilter.InitializeFormationFilters(__instance.Formation);
			}
			switch (deploymentFormationClass)
			{
			case 0:
				formationFilters.Filters.Clear();
				break;
			case 1:
				formationFilters.ForceGetTroopFilterAtIndex(0, 0);
				if (formationFilters.Filters.Count > 1)
				{
					formationFilters.Filters.RemoveRange(1, formationFilters.Filters.Count);
				}
				break;
			case 2:
				formationFilters.ForceGetTroopFilterAtIndex(0, 1);
				if (formationFilters.Filters.Count > 1)
				{
					formationFilters.Filters.RemoveRange(1, formationFilters.Filters.Count);
				}
				break;
			case 3:
				formationFilters.ForceGetTroopFilterAtIndex(0, 2);
				if (formationFilters.Filters.Count > 1)
				{
					formationFilters.Filters.RemoveRange(1, formationFilters.Filters.Count);
				}
				break;
			case 4:
				formationFilters.ForceGetTroopFilterAtIndex(0, 3);
				if (formationFilters.Filters.Count > 1)
				{
					formationFilters.Filters.RemoveRange(1, formationFilters.Filters.Count);
				}
				break;
			case 5:
				formationFilters.ForceGetTroopFilterAtIndex(0, 0);
				formationFilters.ForceGetTroopFilterAtIndex(1, 1);
				if (formationFilters.Filters.Count > 2)
				{
					formationFilters.Filters.RemoveRange(1, formationFilters.Filters.Count);
				}
				break;
			case 6:
				formationFilters.ForceGetTroopFilterAtIndex(0, 2);
				formationFilters.ForceGetTroopFilterAtIndex(1, 3);
				if (formationFilters.Filters.Count > 2)
				{
					formationFilters.Filters.RemoveRange(1, formationFilters.Filters.Count);
				}
				break;
			}
			if (customFormationItemVM.TeamFilter == null)
			{
				customFormationItemVM.TeamFilter = teamFilter;
			}
			customFormationItemVM.UpdateFiltersFromTeamFilter();
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00003D78 File Offset: 0x00001F78
		public static bool Prefix_GetTooltip(OrderOfBattleFormationItemVM __instance, ref List<TooltipProperty> __result, TextObject ____formationTooltipTitleText)
		{
			GameTexts.SetVariable("NUMBER", __instance.TitleText);
			List<TooltipProperty> list = new List<TooltipProperty>
			{
				new TooltipProperty(____formationTooltipTitleText.ToString(), string.Empty, 0, false, 4096)
			};
			CustomFormationItemVM customFormationItemVM = CustomFormationItemVM.GetCustomFormationItemVM(__instance);
			if (customFormationItemVM == null || !__instance.HasFormation)
			{
				__result = list;
				return false;
			}
			List<Agent> list2 = new List<Agent>();
			int[] array = new int[4];
			int[] array2 = new int[4];
			foreach (IFormationUnit formationUnit in __instance.Formation.Arrangement.GetAllUnits())
			{
				Agent agent = formationUnit as Agent;
				if (agent != null)
				{
					if (agent.IsHero)
					{
						list2.Add(agent);
					}
					FormationClass actualTroopType = Utility.GetActualTroopType(agent);
					if (actualTroopType <= 3)
					{
						array[actualTroopType]++;
						if (agent.Banner != null)
						{
							array2[actualTroopType]++;
						}
					}
				}
			}
			foreach (Agent agent2 in __instance.Formation.DetachedUnits)
			{
				if (agent2.IsHero)
				{
					list2.Add(agent2);
				}
				FormationClass actualTroopType2 = Utility.GetActualTroopType(agent2);
				if (actualTroopType2 <= 3)
				{
					array[actualTroopType2]++;
					if (agent2.Banner != null)
					{
						array2[actualTroopType2]++;
					}
				}
			}
			bool flag = false;
			for (FormationClass formationClass = 0; formationClass < 4; formationClass++)
			{
				int num = array[formationClass];
				int num2 = array2[formationClass];
				List<Agent> list3 = new List<Agent>();
				for (int i = 0; i < list2.Count; i++)
				{
					Agent agent3 = list2[i];
					if (formationClass == Utility.GetActualTroopType(agent3))
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
					int num3 = OrderOfBattleFormationClassVM.GetTotalCountOfTroopType(formationClass);
					if (num3 < num)
					{
						Debug.FailedAssert(string.Format("Total troop count of type {0} is lower than the individually calculated troopCount!", formationClass), "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade.ViewModelCollection\\OrderOfBattle\\OrderOfBattleFormationItemVM.cs", "GetTooltip", 537);
						num3 = num;
					}
					string text = new TextObject("{=9pCzjSTa}{PERCENTAGE}% of troop type", null).SetTextVariable("PERCENTAGE", MathF.Round((num3 == 0) ? 0f : (100f * (float)num / (float)num3))).ToString();
					string text2 = GameTexts.FindText("str_RANK_with_NUM_between_parenthesis", null).SetTextVariable("RANK", num.ToString()).SetTextVariable("NUMBER", text)
						.ToString();
					List<TooltipProperty> list4 = list;
					string text3 = "str_troop_group_name";
					int num4 = formationClass;
					list4.Add(new TooltipProperty(GameTexts.FindText(text3, num4.ToString()).ToString(), text2, 0, false, 0));
					if (list3.Count > 0 || num2 > 0)
					{
						list.Add(new TooltipProperty(string.Empty, string.Empty, 0, false, 512));
					}
					foreach (Agent agent4 in list3)
					{
						list.Add(new TooltipProperty(agent4.Name, " ", 0, false, 0));
					}
					if (num2 > 0)
					{
						list.Add(new TooltipProperty(new TextObject("{=scnSXrYC}Banner Bearers", null).ToString(), num2.ToString(), 0, false, 0));
					}
				}
			}
			IEnumerable<OrderOfBattleFormationClassVM> classes = customFormationItemVM.Classes;
			foreach (OrderOfBattleFormationClassVM orderOfBattleFormationClassVM in (classes ?? Enumerable.Empty<OrderOfBattleFormationClassVM>()))
			{
				CustomFormationClassVM customFormationClassVM = CustomFormationClassVM.GetCustomFormationClassVM(orderOfBattleFormationClassVM);
				if (customFormationClassVM != null)
				{
					list.AddRange(customFormationClassVM.GetFilterTooltips());
				}
			}
			__result = list;
			return false;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x0000416C File Offset: 0x0000236C
		public static bool Prefix_UpdateAdjustable(OrderOfBattleFormationItemVM __instance, TextObject ____cantAdjustNotCommanderText, TextObject ____cantAdjustSingledOutText)
		{
			CustomFormationItemVM customFormationItemVM = CustomFormationItemVM.GetCustomFormationItemVM(__instance);
			OrderOfBattleFormationClassVM orderOfBattleFormationClassVM = ((customFormationItemVM != null) ? customFormationItemVM.CurrentFormationClassVM : null);
			if (orderOfBattleFormationClassVM == null)
			{
				return true;
			}
			__instance.IsAdjustable = __instance.IsControlledByPlayer && (orderOfBattleFormationClassVM.Class == 10 || orderOfBattleFormationClassVM.IsAdjustable || !OrderOfBattleFormationItemVM.HasAnyTroopWithClass(orderOfBattleFormationClassVM.Class));
			if (!__instance.IsControlledByPlayer)
			{
				__instance.CantAdjustHint = new HintViewModel(____cantAdjustNotCommanderText, null);
			}
			else
			{
				if (orderOfBattleFormationClassVM.Class == 10 || orderOfBattleFormationClassVM.IsAdjustable)
				{
					return false;
				}
				__instance.CantAdjustHint = new HintViewModel(____cantAdjustSingledOutText, null);
			}
			return false;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00004204 File Offset: 0x00002404
		public CustomFormationItemVM(OrderOfBattleFormationItemVM vm)
			: base(vm)
		{
			CustomFormationItemVM._mixinReverseDictionary.Add(vm, this);
			FormationFilterLogic.OnTeamFilterConfigurationLoaded += this.OnTeamFilterConfigurationLoaded;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x0000422C File Offset: 0x0000242C
		[NullableContext(2)]
		public static CustomFormationItemVM GetCustomFormationItemVM(OrderOfBattleFormationItemVM formationItemVM)
		{
			if (formationItemVM == null)
			{
				return null;
			}
			CustomFormationItemVM customFormationItemVM;
			if (!CustomFormationItemVM._mixinReverseDictionary.TryGetValue(formationItemVM, out customFormationItemVM))
			{
				return null;
			}
			return customFormationItemVM;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00004250 File Offset: 0x00002450
		private void OnTeamFilterConfigurationLoaded(TeamFilter teamFilter)
		{
			this.TeamFilter = teamFilter;
			this.UpdateFiltersFromTeamFilter();
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00004260 File Offset: 0x00002460
		private void UpdateFiltersFromTeamFilter()
		{
			if (this.Formation == null || this.TeamFilter == null)
			{
				return;
			}
			int num = 1;
			MBBindingList<OrderOfBattleFormationClassVM> classes = this.Classes;
			int num2 = ((classes != null) ? classes.IndexOf(this.CurrentFormationClassVM) : (-1));
			MBBindingList<OrderOfBattleFormationClassVM> classes2 = this.Classes;
			if (classes2 != null)
			{
				classes2.Clear();
			}
			FormationFilters formationFilters = this.TeamFilter.GetFormationFilters(this.Formation);
			if (formationFilters != null)
			{
				num = formationFilters.Filters.Count + 1;
			}
			for (int i = 0; i < num; i++)
			{
				MBBindingList<OrderOfBattleFormationClassVM> classes3 = this.Classes;
				if (classes3 != null)
				{
					classes3.Add(new OrderOfBattleFormationClassVM(base.ViewModel, 10));
				}
			}
			MBBindingList<OrderOfBattleFormationClassVM> classes4 = this.Classes;
			IEnumerable<CustomFormationClassVM> enumerable;
			if (classes4 == null)
			{
				enumerable = null;
			}
			else
			{
				enumerable = classes4.Select<OrderOfBattleFormationClassVM, CustomFormationClassVM>((OrderOfBattleFormationClassVM classVM) => CustomFormationClassVM.GetCustomFormationClassVM(classVM));
			}
			IEnumerable<CustomFormationClassVM> enumerable2;
			if ((enumerable2 = enumerable) == null)
			{
				IEnumerable<CustomFormationClassVM> enumerable3 = Enumerable.Empty<CustomFormationClassVM>();
				enumerable2 = enumerable3;
			}
			foreach (CustomFormationClassVM customFormationClassVM in enumerable2)
			{
				if (customFormationClassVM != null)
				{
					customFormationClassVM.UpdateFiltersFromTeamFilter();
				}
			}
			if (num2 >= 0)
			{
				int num3 = num2;
				MBBindingList<OrderOfBattleFormationClassVM> classes5 = this.Classes;
				int? num4 = ((classes5 != null) ? new int?(classes5.Count) : null);
				if ((num3 < num4.GetValueOrDefault()) & (num4 != null))
				{
					this.SetAsCurrentFormationClass(num2);
					return;
				}
			}
			this.SetAsCurrentFormationClass(0);
		}

		// Token: 0x06000073 RID: 115 RVA: 0x000043C4 File Offset: 0x000025C4
		public override void OnFinalize()
		{
			base.OnFinalize();
			FormationFilterLogic.OnTeamFilterConfigurationLoaded -= this.OnTeamFilterConfigurationLoaded;
			this.TeamFilter = null;
			if (base.ViewModel != null)
			{
				CustomFormationItemVM._mixinReverseDictionary.Remove(base.ViewModel);
			}
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00004400 File Offset: 0x00002600
		public void OnClassChanged(bool isFromInteractiveUI)
		{
			if (base.ViewModel != null)
			{
				if (isFromInteractiveUI)
				{
					this.AddLastClassAndClearInvalidClass();
				}
				OrderOfBattleFormationItemVM viewModel = base.ViewModel;
				MBBindingList<OrderOfBattleFormationClassVM> classes = this.Classes;
				bool flag;
				if (classes == null)
				{
					flag = false;
				}
				else
				{
					flag = classes.Any<OrderOfBattleFormationClassVM>((OrderOfBattleFormationClassVM classVM) => classVM.Class != 10);
				}
				viewModel.HasFormation = flag;
				base.ViewModel.OrderOfBattleFormationClassInt = this.GetDeploymentFormationClass();
				if (!base.ViewModel.HasFormation)
				{
					this.ClearCaptainAndHeroTroopsBecauseOfEmpty();
				}
				base.ViewModel.UpdateAdjustable();
			}
		}

		// Token: 0x06000075 RID: 117 RVA: 0x0000448C File Offset: 0x0000268C
		private void AddLastClassAndClearInvalidClass()
		{
			if (this.Classes == null || this.TeamFilter == null || this.Formation == null)
			{
				return;
			}
			FormationFilters formationFilters = this.TeamFilter.GetFormationFilters(this.Formation);
			if (formationFilters == null)
			{
				return;
			}
			if (this.Classes.Count == 0 || this.Classes[this.Classes.Count - 1].Class != 10)
			{
				this.Classes.Add(new OrderOfBattleFormationClassVM(base.ViewModel, 10));
			}
			int num = this.Classes.IndexOf(this.CurrentFormationClassVM);
			if (num < 0 || num >= this.Classes.Count)
			{
				num = 0;
			}
			for (int i = 0; i < this.Classes.Count - 1; i++)
			{
				if (this.Classes[i].Class == 10)
				{
					this.Classes.RemoveAt(i);
					formationFilters.Filters.RemoveAt(i);
					i--;
				}
			}
			num = Mathf.Clamp(num, 0, this.Classes.Count - 1);
			this.SetAsCurrentFormationClass(num);
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00004599 File Offset: 0x00002799
		private DeploymentFormationClass GetDeploymentFormationClass()
		{
			if (base.ViewModel == null || !base.ViewModel.HasFormation || this.Classes == null)
			{
				return 0;
			}
			return CustomFormationItemVM.GetDeploymentFormationClass(base.ViewModel);
		}

		// Token: 0x06000077 RID: 119 RVA: 0x000045C8 File Offset: 0x000027C8
		private static DeploymentFormationClass GetDeploymentFormationClass(OrderOfBattleFormationItemVM formationItemVM)
		{
			if (formationItemVM == null || !formationItemVM.HasFormation || formationItemVM.Classes == null)
			{
				return 0;
			}
			bool flag = formationItemVM.Classes.Any<OrderOfBattleFormationClassVM>((OrderOfBattleFormationClassVM classVM) => classVM.Class == 0);
			bool flag2 = formationItemVM.Classes.Any<OrderOfBattleFormationClassVM>((OrderOfBattleFormationClassVM classVM) => classVM.Class == 1);
			bool flag3 = formationItemVM.Classes.Any<OrderOfBattleFormationClassVM>((OrderOfBattleFormationClassVM classVM) => classVM.Class == 2);
			bool flag4 = formationItemVM.Classes.Any<OrderOfBattleFormationClassVM>((OrderOfBattleFormationClassVM classVM) => classVM.Class == 3);
			return Utility.GetDeploymentFormationClass(flag, flag2, flag3, flag4);
		}

		// Token: 0x06000078 RID: 120 RVA: 0x000046A0 File Offset: 0x000028A0
		private void ClearCaptainAndHeroTroopsBecauseOfEmpty()
		{
			base.ViewModel.UnassignCaptain();
			foreach (OrderOfBattleHeroItemVM orderOfBattleHeroItemVM in base.ViewModel.HeroTroops.ToList<OrderOfBattleHeroItemVM>())
			{
				base.ViewModel.RemoveHeroTroop(orderOfBattleHeroItemVM);
			}
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00004710 File Offset: 0x00002910
		public void OnCustomFilterToggled()
		{
			Action<OrderOfBattleFormationItemVM> onFilterUseToggled = OrderOfBattleFormationItemVM.OnFilterUseToggled;
			if (onFilterUseToggled == null || base.ViewModel == null)
			{
				return;
			}
			onFilterUseToggled(base.ViewModel);
		}

		// Token: 0x0600007A RID: 122 RVA: 0x0000473C File Offset: 0x0000293C
		[DataSourceMethod]
		public void DeleteFormationClass()
		{
			if (this.Formation == null || this.Classes == null || this.Classes.Count == 1)
			{
				return;
			}
			int num = this.Classes.IndexOf(this._currentFormationClassVM);
			if (num < 0)
			{
				return;
			}
			TeamFilter teamFilter = this.TeamFilter;
			FormationFilters formationFilters = ((teamFilter != null) ? teamFilter.GetFormationFilters(this.Formation) : null);
			if (formationFilters == null)
			{
				return;
			}
			formationFilters.Filters.RemoveAt(num);
			this.Classes.RemoveAt(num);
			this.SetAsCurrentFormationClass(Math.Min(num, this.Classes.Count - 1));
		}

		// Token: 0x0600007B RID: 123 RVA: 0x000047D0 File Offset: 0x000029D0
		[DataSourceMethod]
		public void AddFormationClass()
		{
			if (this.Formation == null)
			{
				return;
			}
			TeamFilter teamFilter = this.TeamFilter;
			FormationFilters formationFilters = ((teamFilter != null) ? teamFilter.GetFormationFilters(this.Formation) : null);
			if (formationFilters == null)
			{
				return;
			}
			TroopFilter troopFilter = new TroopFilter();
			troopFilter.SetBasicFilterForDeploymentFormationClass(0);
			formationFilters.Filters.Add(troopFilter);
			OrderOfBattleFormationClassVM orderOfBattleFormationClassVM = new OrderOfBattleFormationClassVM(base.ViewModel, 10);
			MBBindingList<OrderOfBattleFormationClassVM> classes = this.Classes;
			if (classes != null)
			{
				classes.Add(orderOfBattleFormationClassVM);
			}
			this.SetAsCurrentFormationClass(orderOfBattleFormationClassVM);
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00004844 File Offset: 0x00002A44
		public void SetAsCurrentFormationClass(OrderOfBattleFormationClassVM customFormationClassVM)
		{
			MBBindingList<OrderOfBattleFormationClassVM> classes = this.Classes;
			int num = ((classes != null) ? classes.IndexOf(customFormationClassVM) : (-1));
			this.SetAsCurrentFormationClass(num);
		}

		// Token: 0x0600007D RID: 125 RVA: 0x0000486C File Offset: 0x00002A6C
		public void SetAsCurrentFormationClass(int index)
		{
			if (this.Classes == null || index < 0 || index >= this.Classes.Count)
			{
				return;
			}
			this.CurrentFormationClassNumberText = GameTexts.FindText("str_LEFT_over_RIGHT_no_space", null).SetTextVariable("LEFT", index + 1).SetTextVariable("RIGHT", (index + 1 == this.Classes.Count) ? this.Classes.Count : (this.Classes.Count - 1))
				.ToString();
			this.IsDeleteEnabled = this.Classes.Count > 1;
			this.IsPreviousFormationClassEnabled = index > 0;
			this.IsNextFormationClassEnabled = index < this.Classes.Count - 1;
			this.CurrentFormationClassVM = this.Classes[index];
			OrderOfBattleFormationItemVM viewModel = base.ViewModel;
			if (viewModel == null)
			{
				return;
			}
			viewModel.UpdateAdjustable();
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00004944 File Offset: 0x00002B44
		private bool HasAnyActiveFitlers()
		{
			if (this.Classes == null)
			{
				return false;
			}
			foreach (OrderOfBattleFormationClassVM orderOfBattleFormationClassVM in this.Classes)
			{
				CustomFormationClassVM customFormationClassVM = CustomFormationClassVM.GetCustomFormationClassVM(orderOfBattleFormationClassVM);
				if (customFormationClassVM != null && customFormationClassVM.HasAnyActiveFitlers())
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600007F RID: 127 RVA: 0x000049B0 File Offset: 0x00002BB0
		[DataSourceMethod]
		public void PreviousFormationClass()
		{
			if (this.Classes == null || this.Classes.Count <= 1)
			{
				return;
			}
			int num = this.Classes.IndexOf(this.CurrentFormationClassVM);
			if (num < 0)
			{
				return;
			}
			num--;
			if (num < 0)
			{
				num = this.Classes.Count - 1;
			}
			this.SetAsCurrentFormationClass(num);
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00004A08 File Offset: 0x00002C08
		[DataSourceMethod]
		public void NextFormationClass()
		{
			if (this.Classes == null || this.Classes.Count <= 1)
			{
				return;
			}
			int num = this.Classes.IndexOf(this.CurrentFormationClassVM);
			if (num < 0)
			{
				return;
			}
			num++;
			if (num >= this.Classes.Count)
			{
				num = 0;
			}
			this.SetAsCurrentFormationClass(num);
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000081 RID: 129 RVA: 0x00004A5E File Offset: 0x00002C5E
		// (set) Token: 0x06000082 RID: 130 RVA: 0x00004A66 File Offset: 0x00002C66
		[DataSourceProperty]
		public OrderOfBattleFormationClassVM CurrentFormationClassVM
		{
			get
			{
				return this._currentFormationClassVM;
			}
			set
			{
				if (value == this._currentFormationClassVM)
				{
					return;
				}
				this._currentFormationClassVM = value;
				base.OnPropertyChangedWithValue(value, "CurrentFormationClassVM");
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000083 RID: 131 RVA: 0x00004A85 File Offset: 0x00002C85
		// (set) Token: 0x06000084 RID: 132 RVA: 0x00004A8D File Offset: 0x00002C8D
		[DataSourceProperty]
		public string CurrentFormationClassNumberText
		{
			get
			{
				return this._currentFormationClassNumberText;
			}
			set
			{
				if (value == this._currentFormationClassNumberText)
				{
					return;
				}
				this._currentFormationClassNumberText = value;
				base.OnPropertyChangedWithValue(value, "_currentFormationClassNumberText");
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000085 RID: 133 RVA: 0x00004AB1 File Offset: 0x00002CB1
		// (set) Token: 0x06000086 RID: 134 RVA: 0x00004AB9 File Offset: 0x00002CB9
		[DataSourceProperty]
		public bool IsDeleteEnabled
		{
			get
			{
				return this._isDeleteEnabled;
			}
			set
			{
				if (value == this._isDeleteEnabled)
				{
					return;
				}
				this._isDeleteEnabled = value;
				base.OnPropertyChangedWithValue(value, "IsDeleteEnabled");
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000087 RID: 135 RVA: 0x00004ADD File Offset: 0x00002CDD
		// (set) Token: 0x06000088 RID: 136 RVA: 0x00004AE5 File Offset: 0x00002CE5
		[DataSourceProperty]
		public bool IsPreviousFormationClassEnabled
		{
			get
			{
				return this._isPreviousFormationClassEnabled;
			}
			set
			{
				if (value == this._isPreviousFormationClassEnabled)
				{
					return;
				}
				this._isPreviousFormationClassEnabled = value;
				base.OnPropertyChangedWithValue(value, "IsPreviousFormationClassEnabled");
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000089 RID: 137 RVA: 0x00004B09 File Offset: 0x00002D09
		// (set) Token: 0x0600008A RID: 138 RVA: 0x00004B11 File Offset: 0x00002D11
		[DataSourceProperty]
		public bool IsNextFormationClassEnabled
		{
			get
			{
				return this._isNextFormationClassEnabled;
			}
			set
			{
				if (value == this._isNextFormationClassEnabled)
				{
					return;
				}
				this._isNextFormationClassEnabled = value;
				base.OnPropertyChangedWithValue(value, "IsNextFormationClassEnabled");
			}
		}

		// Token: 0x04000041 RID: 65
		private static PropertyInfo _formation = AccessTools.Property(typeof(OrderOfBattleFormationItemVM), "Formation");

		// Token: 0x04000042 RID: 66
		private static MethodInfo _refreshMarkerWorldPosition = AccessTools.Method(typeof(OrderOfBattleFormationItemVM), "RefreshMarkerWorldPosition", null, null);

		// Token: 0x04000043 RID: 67
		private static Dictionary<OrderOfBattleFormationItemVM, CustomFormationItemVM> _mixinReverseDictionary = new Dictionary<OrderOfBattleFormationItemVM, CustomFormationItemVM>();

		// Token: 0x04000044 RID: 68
		private string _currentFormationClassNumberText;

		// Token: 0x04000045 RID: 69
		private OrderOfBattleFormationClassVM _currentFormationClassVM;

		// Token: 0x04000046 RID: 70
		public bool _isDeleteEnabled;

		// Token: 0x04000047 RID: 71
		public bool _isPreviousFormationClassEnabled;

		// Token: 0x04000048 RID: 72
		public bool _isNextFormationClassEnabled;
	}
}
