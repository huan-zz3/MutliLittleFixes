using System;

namespace NavalDLC.DWA
{
	// Token: 0x0200014A RID: 330
	internal struct DWAFloatPair
	{
		// Token: 0x060015AA RID: 5546 RVA: 0x000977FB File Offset: 0x000959FB
		internal DWAFloatPair(float a, float b)
		{
			this._a = a;
			this._b = b;
		}

		// Token: 0x060015AB RID: 5547 RVA: 0x0009780B File Offset: 0x00095A0B
		public static bool operator <(DWAFloatPair pair1, DWAFloatPair pair2)
		{
			return pair1._a < pair2._a || (pair2._a >= pair1._a && pair1._b < pair2._b);
		}

		// Token: 0x060015AC RID: 5548 RVA: 0x0009783B File Offset: 0x00095A3B
		public static bool operator <=(DWAFloatPair pair1, DWAFloatPair pair2)
		{
			return (pair1._a == pair2._a && pair1._b == pair2._b) || pair1 < pair2;
		}

		// Token: 0x060015AD RID: 5549 RVA: 0x00097862 File Offset: 0x00095A62
		public static bool operator >(DWAFloatPair pair1, DWAFloatPair pair2)
		{
			return !(pair1 <= pair2);
		}

		// Token: 0x060015AE RID: 5550 RVA: 0x0009786E File Offset: 0x00095A6E
		public static bool operator >=(DWAFloatPair pair1, DWAFloatPair pair2)
		{
			return !(pair1 < pair2);
		}

		// Token: 0x04000B2F RID: 2863
		private float _a;

		// Token: 0x04000B30 RID: 2864
		private float _b;
	}
}
