using System;

namespace Mono.Cecil.Metadata
{
	// Token: 0x020002E4 RID: 740
	internal sealed class UserStringHeap : StringHeap
	{
		// Token: 0x060012E1 RID: 4833 RVA: 0x0003B648 File Offset: 0x00039848
		public UserStringHeap(byte[] data)
			: base(data)
		{
		}

		// Token: 0x060012E2 RID: 4834 RVA: 0x0003B654 File Offset: 0x00039854
		protected override string ReadStringAt(uint index)
		{
			int num = (int)index;
			uint num2 = (uint)((ulong)this.data.ReadCompressedUInt32(ref num) & 18446744073709551614UL);
			if (num2 < 1U)
			{
				return string.Empty;
			}
			char[] array = new char[num2 / 2U];
			int num3 = num;
			int num4 = 0;
			while ((long)num3 < (long)num + (long)((ulong)num2))
			{
				array[num4++] = (char)((int)this.data[num3] | ((int)this.data[num3 + 1] << 8));
				num3 += 2;
			}
			return new string(array);
		}
	}
}
