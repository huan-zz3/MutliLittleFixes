using System;
using System.Collections.Generic;
using System.Text;

namespace Mono.Cecil.Metadata
{
	// Token: 0x020002E0 RID: 736
	internal class StringHeap : Heap
	{
		// Token: 0x060012DA RID: 4826 RVA: 0x0003B557 File Offset: 0x00039757
		public StringHeap(byte[] data)
			: base(data)
		{
		}

		// Token: 0x060012DB RID: 4827 RVA: 0x0003B56C File Offset: 0x0003976C
		public string Read(uint index)
		{
			if (index == 0U)
			{
				return string.Empty;
			}
			string text;
			if (this.strings.TryGetValue(index, out text))
			{
				return text;
			}
			if ((ulong)index > (ulong)((long)(this.data.Length - 1)))
			{
				return string.Empty;
			}
			text = this.ReadStringAt(index);
			if (text.Length != 0)
			{
				this.strings.Add(index, text);
			}
			return text;
		}

		// Token: 0x060012DC RID: 4828 RVA: 0x0003B5C8 File Offset: 0x000397C8
		protected virtual string ReadStringAt(uint index)
		{
			int num = 0;
			int num2 = (int)index;
			while (this.data[num2] != 0)
			{
				num++;
				num2++;
			}
			return Encoding.UTF8.GetString(this.data, (int)index, num);
		}

		// Token: 0x0400075B RID: 1883
		private readonly Dictionary<uint, string> strings = new Dictionary<uint, string>();
	}
}
