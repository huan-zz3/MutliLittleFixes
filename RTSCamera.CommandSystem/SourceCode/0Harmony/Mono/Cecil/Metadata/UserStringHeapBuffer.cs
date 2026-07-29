using System;

namespace Mono.Cecil.Metadata
{
	// Token: 0x020002D2 RID: 722
	internal sealed class UserStringHeapBuffer : StringHeapBuffer
	{
		// Token: 0x060012C3 RID: 4803 RVA: 0x0003B260 File Offset: 0x00039460
		public override uint GetStringIndex(string @string)
		{
			uint position;
			if (this.strings.TryGetValue(@string, out position))
			{
				return position;
			}
			position = (uint)this.position;
			this.WriteString(@string);
			this.strings.Add(@string, position);
			return position;
		}

		// Token: 0x060012C4 RID: 4804 RVA: 0x0003B29C File Offset: 0x0003949C
		protected override void WriteString(string @string)
		{
			base.WriteCompressedUInt32((uint)(@string.Length * 2 + 1));
			byte b = 0;
			foreach (char c in @string)
			{
				base.WriteUInt16((ushort)c);
				if (b != 1 && (c < ' ' || c > '~') && (c > '~' || (c >= '\u0001' && c <= '\b') || (c >= '\u000e' && c <= '\u001f') || c == '\'' || c == '-'))
				{
					b = 1;
				}
			}
			base.WriteByte(b);
		}
	}
}
