using System;

namespace NavalDLC.CustomBattle.CustomBattle
{
	// Token: 0x0200000F RID: 15
	public struct NavalCustomBattleCompositionData
	{
		// Token: 0x0600009B RID: 155 RVA: 0x000049B7 File Offset: 0x00002BB7
		public NavalCustomBattleCompositionData(float rangedPercentage, float cavalryPercentage, float rangedCavalryPercentage)
		{
			this.RangedPercentage = rangedPercentage;
			this.CavalryPercentage = cavalryPercentage;
			this.RangedCavalryPercentage = rangedCavalryPercentage;
			this.IsValid = true;
		}

		// Token: 0x04000048 RID: 72
		public readonly bool IsValid;

		// Token: 0x04000049 RID: 73
		public readonly float RangedPercentage;

		// Token: 0x0400004A RID: 74
		public readonly float CavalryPercentage;

		// Token: 0x0400004B RID: 75
		public readonly float RangedCavalryPercentage;
	}
}
