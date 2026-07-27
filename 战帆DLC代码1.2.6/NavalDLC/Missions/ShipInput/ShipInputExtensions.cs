using System;

namespace NavalDLC.Missions.ShipInput
{
	// Token: 0x02000088 RID: 136
	public static class ShipInputExtensions
	{
		// Token: 0x060009A8 RID: 2472 RVA: 0x00044EB6 File Offset: 0x000430B6
		public static RowerLateralInput OppositeDirection(this RowerLateralInput input)
		{
			if (input == RowerLateralInput.Left)
			{
				return RowerLateralInput.Right;
			}
			if (input == RowerLateralInput.Right)
			{
				return RowerLateralInput.Left;
			}
			return input;
		}

		// Token: 0x060009A9 RID: 2473 RVA: 0x00044EC5 File Offset: 0x000430C5
		public static float RudderLateralInputOppositeDirection(float input)
		{
			return -input;
		}

		// Token: 0x060009AA RID: 2474 RVA: 0x00044EC9 File Offset: 0x000430C9
		public static float ToRudderInput(this RowerLateralInput input)
		{
			switch (input)
			{
			case RowerLateralInput.Right:
				return 1f;
			case RowerLateralInput.Left:
				return -1f;
			case RowerLateralInput.Stop:
				return 0f;
			}
			return 0f;
		}

		// Token: 0x060009AB RID: 2475 RVA: 0x00044EFC File Offset: 0x000430FC
		public static SailInput Lower(this SailInput input, bool hasHybridSails = false)
		{
			int num = Math.Min((int)(input + 1), 2);
			if (num == 1 && !hasHybridSails)
			{
				num = Math.Min(num + 1, 2);
			}
			return (SailInput)num;
		}

		// Token: 0x060009AC RID: 2476 RVA: 0x00044F28 File Offset: 0x00043128
		public static SailInput Raise(this SailInput input, bool hasHybridSails = false)
		{
			int num = Math.Max(input - SailInput.SquareSailsRaised, 0);
			if (num == 1 && !hasHybridSails)
			{
				num = Math.Max(num - 1, 0);
			}
			return (SailInput)num;
		}

		// Token: 0x060009AD RID: 2477 RVA: 0x00044F54 File Offset: 0x00043154
		public static SailInput Min(this SailInput input, bool hasHybridSails = false)
		{
			for (;;)
			{
				SailInput sailInput = input.Lower(hasHybridSails);
				if (sailInput == input)
				{
					break;
				}
				input = sailInput;
			}
			return input;
		}

		// Token: 0x060009AE RID: 2478 RVA: 0x00044F74 File Offset: 0x00043174
		public static SailInput Max(this SailInput input, bool hasHybridSails = false)
		{
			for (;;)
			{
				SailInput sailInput = input.Raise(hasHybridSails);
				if (sailInput == input)
				{
					break;
				}
				input = sailInput;
			}
			return input;
		}

		// Token: 0x060009AF RID: 2479 RVA: 0x00044F93 File Offset: 0x00043193
		public static bool IsMin(this SailInput input)
		{
			return input == input.Lower(false);
		}

		// Token: 0x060009B0 RID: 2480 RVA: 0x00044F9F File Offset: 0x0004319F
		public static bool IsMax(this SailInput input)
		{
			return input == input.Raise(false);
		}

		// Token: 0x060009B1 RID: 2481 RVA: 0x00044FAC File Offset: 0x000431AC
		public static string ToShortText(this RowerLongitudinalInput input)
		{
			if (input != RowerLongitudinalInput.None)
			{
				return input.ToString()[0].ToString() ?? "";
			}
			return "-";
		}

		// Token: 0x060009B2 RID: 2482 RVA: 0x00044FE8 File Offset: 0x000431E8
		public static string ToShortText(this RowerLateralInput input)
		{
			if (input != RowerLateralInput.None)
			{
				return input.ToString()[0].ToString() ?? "";
			}
			return "-";
		}

		// Token: 0x060009B3 RID: 2483 RVA: 0x00045022 File Offset: 0x00043222
		public static string RudderLateralInputToShortText(float input)
		{
			return input.ToString();
		}

		// Token: 0x060009B4 RID: 2484 RVA: 0x0004502C File Offset: 0x0004322C
		public static string ToShortText(this SailInput input)
		{
			return input.ToString()[0].ToString() ?? "";
		}
	}
}
