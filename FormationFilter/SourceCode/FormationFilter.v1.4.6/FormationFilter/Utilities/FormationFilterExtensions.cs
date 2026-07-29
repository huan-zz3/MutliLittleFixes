using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FormationFilter.Models;
using FormationFilter.View.ViewModels;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace FormationFilter.Utilities
{
	// Token: 0x02000011 RID: 17
	[NullableContext(1)]
	[Nullable(0)]
	public static class FormationFilterExtensions
	{
		// Token: 0x0600009B RID: 155 RVA: 0x00004E0C File Offset: 0x0000300C
		public static TextObject GetFilterName(this CustomFormationFilterType filterType)
		{
			return GameTexts.FindText("str_formation_filter_type", filterType.ToString());
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00004E25 File Offset: 0x00003025
		public static TextObject GetFilterDescription(this CustomFormationFilterType filterType)
		{
			return GameTexts.FindText("str_formation_filter_description", filterType.ToString());
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00004E3E File Offset: 0x0000303E
		public static TextObject GetFilterValueName(this FilterValueEnum filterValue)
		{
			return GameTexts.FindText("str_formation_filter_value", filterValue.ToString());
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00004E57 File Offset: 0x00003057
		public static TextObject GetFilterTypeAndValueDescription(this CustomFormationFilterType filterType, FilterValueEnum filterValue)
		{
			return GameTexts.FindText("str_STR1_space_STR2", null).SetTextVariable("STR1", filterType.GetFilterName()).SetTextVariable("STR2", filterValue.GetFilterValueName());
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00004E84 File Offset: 0x00003084
		public static FilterTypeEnum ToFilterType(this CustomFormationFilterType filterType)
		{
			switch (filterType)
			{
			case CustomFormationFilterType.OneHanded:
				return FilterTypeEnum.HasOneHanded;
			case CustomFormationFilterType.TwoHanded:
				return FilterTypeEnum.HasTwoHanded;
			case CustomFormationFilterType.Spear:
				return FilterTypeEnum.HasPolearm;
			case CustomFormationFilterType.Thrown:
				return FilterTypeEnum.HasThrowing;
			case CustomFormationFilterType.Shield:
				return FilterTypeEnum.HasShield;
			case CustomFormationFilterType.Heavy:
				return FilterTypeEnum.HeavyArmor;
			case CustomFormationFilterType.LowTier:
				return FilterTypeEnum.LowTier;
			case CustomFormationFilterType.HighTier:
				return FilterTypeEnum.HighTier;
			case CustomFormationFilterType.Bow:
				return FilterTypeEnum.HasBow;
			case CustomFormationFilterType.Crossbow:
				return FilterTypeEnum.HasCrossBow;
			case CustomFormationFilterType.Sling:
				return FilterTypeEnum.HasSling;
			default:
				Utility.DisplayInvalidCustomFormationFilterType(filterType);
				return FilterTypeEnum.Count;
			}
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00004EE9 File Offset: 0x000030E9
		public static bool IsBasicFilter(this FilterTypeEnum filterType)
		{
			return filterType == FilterTypeEnum.HasRanged || filterType == FilterTypeEnum.HasMount;
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00004EF8 File Offset: 0x000030F8
		public static void SetBasicFilterForFormationClass(this TroopFilter filter, FormationClass formationClass)
		{
			switch (formationClass)
			{
			case 0:
				filter.SetFilter(FilterTypeEnum.HasRanged, FilterValueEnum.No);
				filter.SetFilter(FilterTypeEnum.HasMount, FilterValueEnum.No);
				return;
			case 1:
				filter.SetFilter(FilterTypeEnum.HasRanged, FilterValueEnum.Yes);
				filter.SetFilter(FilterTypeEnum.HasMount, FilterValueEnum.No);
				return;
			case 2:
				filter.SetFilter(FilterTypeEnum.HasRanged, FilterValueEnum.No);
				filter.SetFilter(FilterTypeEnum.HasMount, FilterValueEnum.Yes);
				return;
			case 3:
				filter.SetFilter(FilterTypeEnum.HasRanged, FilterValueEnum.Yes);
				filter.SetFilter(FilterTypeEnum.HasMount, FilterValueEnum.Yes);
				break;
			default:
				if (formationClass == 10)
				{
					filter.SetFilter(FilterTypeEnum.HasRanged, FilterValueEnum.Invalid);
					filter.SetFilter(FilterTypeEnum.HasMount, FilterValueEnum.Invalid);
					return;
				}
				break;
			}
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00004F74 File Offset: 0x00003174
		public static void SetBasicFilterForDeploymentFormationClass(this TroopFilter filter, DeploymentFormationClass deploymentFormationClass)
		{
			switch (deploymentFormationClass)
			{
			case 0:
				filter.SetFilter(FilterTypeEnum.HasRanged, FilterValueEnum.Invalid);
				filter.SetFilter(FilterTypeEnum.HasMount, FilterValueEnum.Invalid);
				return;
			case 1:
				filter.SetFilter(FilterTypeEnum.HasRanged, FilterValueEnum.No);
				filter.SetFilter(FilterTypeEnum.HasMount, FilterValueEnum.No);
				return;
			case 2:
				filter.SetFilter(FilterTypeEnum.HasRanged, FilterValueEnum.Yes);
				filter.SetFilter(FilterTypeEnum.HasMount, FilterValueEnum.No);
				return;
			case 3:
				filter.SetFilter(FilterTypeEnum.HasRanged, FilterValueEnum.No);
				filter.SetFilter(FilterTypeEnum.HasMount, FilterValueEnum.Yes);
				return;
			case 4:
				filter.SetFilter(FilterTypeEnum.HasRanged, FilterValueEnum.Yes);
				filter.SetFilter(FilterTypeEnum.HasMount, FilterValueEnum.Yes);
				return;
			case 5:
				filter.SetFilter(FilterTypeEnum.HasRanged, FilterValueEnum.Any);
				filter.SetFilter(FilterTypeEnum.HasMount, FilterValueEnum.No);
				return;
			case 6:
				filter.SetFilter(FilterTypeEnum.HasRanged, FilterValueEnum.Any);
				filter.SetFilter(FilterTypeEnum.HasMount, FilterValueEnum.Yes);
				return;
			default:
				return;
			}
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x0000501C File Offset: 0x0000321C
		public static FormationClass DetectBasicFomrationClass(this TroopFilter filter)
		{
			FilterValueEnum filter2 = filter.GetFilter(FilterTypeEnum.HasRanged);
			FilterValueEnum filter3 = filter.GetFilter(FilterTypeEnum.HasMount);
			if (filter2 == FilterValueEnum.No && filter3 == FilterValueEnum.No)
			{
				return 0;
			}
			if (filter2 == FilterValueEnum.Yes && filter3 == FilterValueEnum.No)
			{
				return 1;
			}
			if (filter2 == FilterValueEnum.No && filter3 == FilterValueEnum.Yes)
			{
				return 2;
			}
			if (filter2 == FilterValueEnum.Yes && filter3 == FilterValueEnum.Yes)
			{
				return 3;
			}
			return 10;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00005064 File Offset: 0x00003264
		public static DeploymentFormationClass DetectDeploymentFormationClass(this TroopFilter filter)
		{
			FilterValueEnum filter2 = filter.GetFilter(FilterTypeEnum.HasRanged);
			FilterValueEnum filter3 = filter.GetFilter(FilterTypeEnum.HasMount);
			if (filter2 == FilterValueEnum.Any && filter3 == FilterValueEnum.No)
			{
				return 5;
			}
			if (filter2 == FilterValueEnum.Any && filter3 == FilterValueEnum.Yes)
			{
				return 6;
			}
			if (filter2 == FilterValueEnum.No && filter3 == FilterValueEnum.No)
			{
				return 1;
			}
			if (filter2 == FilterValueEnum.Yes && filter3 == FilterValueEnum.No)
			{
				return 2;
			}
			if (filter2 == FilterValueEnum.No && filter3 == FilterValueEnum.Yes)
			{
				return 3;
			}
			if (filter2 == FilterValueEnum.Yes && filter3 == FilterValueEnum.Yes)
			{
				return 4;
			}
			return 0;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x000050C0 File Offset: 0x000032C0
		public static void AddRange(this HashSet<ulong> hashSet, IEnumerable<ulong> range)
		{
			foreach (ulong num in range)
			{
				hashSet.Add(num);
			}
		}
	}
}
