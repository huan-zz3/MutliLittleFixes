using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel
{
	// Token: 0x0200065C RID: 1628
	internal static class MemorySizeExtensions
	{
		// Token: 0x06002369 RID: 9065 RVA: 0x000728EA File Offset: 0x00070AEA
		public static bool IsBroadcast(this MemorySize memorySize)
		{
			return memorySize >= MemorySize.Broadcast32_Float16;
		}

		// Token: 0x0600236A RID: 9066 RVA: 0x000728F4 File Offset: 0x00070AF4
		private unsafe static MemorySizeInfo[] GetMemorySizeInfos()
		{
			ReadOnlySpan<byte> readOnlySpan = new ReadOnlySpan<byte>((void*)(&<b37590d4-39fb-478a-88de-d293f3364852><PrivateImplementationDetails>.9F8A914BAE36A263FB33BFC11BF247BB2AACC1F65E65F5B3AB22B8DC6991FD68), 486);
			ushort[] array = new ushort[]
			{
				0, 1, 2, 4, 6, 8, 10, 14, 16, 28,
				32, 48, 64, 94, 108, 512
			};
			MemorySizeInfo[] array2 = new MemorySizeInfo[162];
			int i = 0;
			int num = 0;
			while (i < array2.Length)
			{
				MemorySize memorySize = (MemorySize)(*readOnlySpan[num]);
				uint num2 = (uint)(((int)(*readOnlySpan[num + 2]) << 8) | (int)(*readOnlySpan[num + 1]));
				ushort num3 = array[(int)(num2 & 31U)];
				ushort num4 = array[(int)((num2 >> 5) & 31U)];
				array2[i] = new MemorySizeInfo((MemorySize)i, (int)num3, (int)num4, memorySize, (num2 & 32768U) > 0U, i >= 112);
				i++;
				num += 3;
			}
			return array2;
		}

		// Token: 0x0600236B RID: 9067 RVA: 0x000729AC File Offset: 0x00070BAC
		public static MemorySizeInfo GetInfo(this MemorySize memorySize)
		{
			MemorySizeInfo[] memorySizeInfos = MemorySizeExtensions.MemorySizeInfos;
			if (memorySize >= (MemorySize)memorySizeInfos.Length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_memorySize();
			}
			return memorySizeInfos[(int)memorySize];
		}

		// Token: 0x0600236C RID: 9068 RVA: 0x000729D4 File Offset: 0x00070BD4
		public static int GetSize(this MemorySize memorySize)
		{
			return memorySize.GetInfo().Size;
		}

		// Token: 0x0600236D RID: 9069 RVA: 0x000729F0 File Offset: 0x00070BF0
		public static int GetElementSize(this MemorySize memorySize)
		{
			return memorySize.GetInfo().ElementSize;
		}

		// Token: 0x0600236E RID: 9070 RVA: 0x00072A0C File Offset: 0x00070C0C
		public static MemorySize GetElementType(this MemorySize memorySize)
		{
			return memorySize.GetInfo().ElementType;
		}

		// Token: 0x0600236F RID: 9071 RVA: 0x00072A28 File Offset: 0x00070C28
		public static MemorySizeInfo GetElementTypeInfo(this MemorySize memorySize)
		{
			return memorySize.GetInfo().ElementType.GetInfo();
		}

		// Token: 0x06002370 RID: 9072 RVA: 0x00072A48 File Offset: 0x00070C48
		public static bool IsSigned(this MemorySize memorySize)
		{
			return memorySize.GetInfo().IsSigned;
		}

		// Token: 0x06002371 RID: 9073 RVA: 0x00072A64 File Offset: 0x00070C64
		public static bool IsPacked(this MemorySize memorySize)
		{
			return memorySize.GetInfo().IsPacked;
		}

		// Token: 0x06002372 RID: 9074 RVA: 0x00072A80 File Offset: 0x00070C80
		public static int GetElementCount(this MemorySize memorySize)
		{
			return memorySize.GetInfo().ElementCount;
		}

		// Token: 0x04002BA4 RID: 11172
		[Nullable(1)]
		internal static readonly MemorySizeInfo[] MemorySizeInfos = MemorySizeExtensions.GetMemorySizeInfos();
	}
}
