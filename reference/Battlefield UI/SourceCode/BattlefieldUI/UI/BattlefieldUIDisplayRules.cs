using System;

namespace BattlefieldUI.UI
{
	// Token: 0x0200000A RID: 10
	public static class BattlefieldUIDisplayRules
	{
		// Token: 0x06000063 RID: 99 RVA: 0x00003A6F File Offset: 0x00001C6F
		public static bool ShouldDisplay(int displayMode, float health, float maximumHealth)
		{
			return health > 0f && maximumHealth > 0f && (displayMode == 0 || health < maximumHealth - 0.01f);
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00003A94 File Offset: 0x00001C94
		public static float CalculateAlpha(float distance, float fadeStartDistance, float maximumDistance)
		{
			if (distance < 0f || maximumDistance <= 0f || distance >= maximumDistance)
			{
				return 0f;
			}
			float num = Math.Max(0f, Math.Min(fadeStartDistance, maximumDistance - 0.01f));
			if (distance <= num)
			{
				return 1f;
			}
			return Math.Max(0f, Math.Min(1f, (maximumDistance - distance) / (maximumDistance - num)));
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00003AF8 File Offset: 0x00001CF8
		public static int GetDisplayedDamage(int inflictedDamage)
		{
			return Math.Max(0, inflictedDamage);
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00003B04 File Offset: 0x00001D04
		public static float CalculateDamageNumberAlpha(float age, float lifetime)
		{
			if (age < 0f || lifetime <= 0f || age >= lifetime)
			{
				return 0f;
			}
			float num = age / lifetime;
			float num2 = ((num < 0.12f) ? (num / 0.12f) : 1f);
			float num3 = ((num > 0.65f) ? ((1f - num) / 0.35f) : 1f);
			return Math.Max(0f, Math.Min(1f, Math.Min(num2, num3)));
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00003B80 File Offset: 0x00001D80
		public static float CalculateMarkerScale(float distance, float maximumDistance)
		{
			if (distance <= 10f || maximumDistance <= 10f)
			{
				return 1.15f;
			}
			float num = (distance - 10f) / (maximumDistance - 10f);
			num = Math.Max(0f, Math.Min(1f, num));
			return 1.15f + -0.65f * num;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00003BD6 File Offset: 0x00001DD6
		public static int NormalizeCornerStyle(int cornerStyle)
		{
			return Math.Max(0, Math.Min(2, cornerStyle));
		}

		// Token: 0x0400002A RID: 42
		public const int AlwaysVisibleMode = 0;

		// Token: 0x0400002B RID: 43
		public const int InjuredOnlyMode = 1;

		// Token: 0x0400002C RID: 44
		public const int SquareCornerStyle = 0;

		// Token: 0x0400002D RID: 45
		public const int SmallCornerStyle = 1;

		// Token: 0x0400002E RID: 46
		public const int LargeCornerStyle = 2;
	}
}
