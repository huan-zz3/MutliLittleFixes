using System;
using TaleWorlds.Core;

namespace FormationFilter.Models
{
	// Token: 0x0200001B RID: 27
	public static class FilterTypeExtension
	{
		// Token: 0x060000F8 RID: 248 RVA: 0x000087B4 File Offset: 0x000069B4
		public static WeaponClass ToWeaponClass(this FilterTypeEnum filterTypeEnum)
		{
			switch (filterTypeEnum)
			{
			case FilterTypeEnum.HasMount:
				return 0;
			case FilterTypeEnum.HeavyArmor:
				return 0;
			case FilterTypeEnum.HasBow:
				return 16;
			case FilterTypeEnum.HasCrossBow:
				return 17;
			case FilterTypeEnum.HasSling:
				return 18;
			case FilterTypeEnum.HasOneHandedSword:
				return 2;
			case FilterTypeEnum.HasOneHandedAxe:
				return 4;
			case FilterTypeEnum.HasOneHandedMace:
				return 6;
			case FilterTypeEnum.HasTwoHandedSword:
				return 3;
			case FilterTypeEnum.HasTwoHandedAxe:
				return 5;
			case FilterTypeEnum.HasTwoHandedMace:
				return 8;
			case FilterTypeEnum.HasOneHandedPolearm:
				return 9;
			case FilterTypeEnum.HasTwoHandedPolearm:
				return 10;
			case FilterTypeEnum.HasThrowingAxe:
				return 21;
			case FilterTypeEnum.HasThrowingKnife:
				return 22;
			case FilterTypeEnum.HasJavelin:
				return 23;
			case FilterTypeEnum.HasSmallShield:
				return 28;
			case FilterTypeEnum.HasLargeShield:
				return 29;
			}
			return 0;
		}
	}
}
