using System;

namespace Iced.Intel
{
	// Token: 0x02000652 RID: 1618
	internal static class InstructionInfoExtensions
	{
		// Token: 0x0600230D RID: 8973 RVA: 0x00071A88 File Offset: 0x0006FC88
		public static Code NegateConditionCode(this Code code)
		{
			uint num;
			if ((num = (uint)(code - Code.Jo_rel16)) <= 47U || (num = (uint)(code - Code.Jo_rel8_16)) <= 47U || (num = (uint)(code - Code.Cmovo_r16_rm16)) <= 47U)
			{
				if (((num / 3U) & 1U) != 0U)
				{
					return code - 3;
				}
				return code + 3;
			}
			else
			{
				num = (uint)(code - Code.Seto_rm8);
				if (num <= 15U)
				{
					return (int)(num ^ 1U) + Code.Seto_rm8;
				}
				num = (uint)(code - Code.Loopne_rel8_16_CX);
				if (num <= 13U)
				{
					return Code.Loopne_rel8_16_CX + (int)((num + 7U) % 14U);
				}
				if ((num = (uint)(code - Code.VEX_Cmpoxadd_m32_r32_r32)) > 31U)
				{
					return code;
				}
				if ((num & 2U) != 0U)
				{
					return code - 2;
				}
				return code + 2;
			}
		}

		// Token: 0x0600230E RID: 8974 RVA: 0x00071B18 File Offset: 0x0006FD18
		public static Code ToShortBranch(this Code code)
		{
			uint num = (uint)(code - Code.Jo_rel16);
			if (num <= 47U)
			{
				return (int)num + Code.Jo_rel8_16;
			}
			num = (uint)(code - Code.Jmp_rel16);
			if (num <= 2U)
			{
				return (int)num + Code.Jmp_rel8_16;
			}
			return code;
		}

		// Token: 0x0600230F RID: 8975 RVA: 0x00071B50 File Offset: 0x0006FD50
		public static Code ToNearBranch(this Code code)
		{
			uint num = (uint)(code - Code.Jo_rel8_16);
			if (num <= 47U)
			{
				return (int)num + Code.Jo_rel16;
			}
			num = (uint)(code - Code.Jmp_rel8_16);
			if (num <= 2U)
			{
				return (int)num + Code.Jmp_rel16;
			}
			return code;
		}
	}
}
