using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MonoMod.Logs
{
	// Token: 0x02000817 RID: 2071
	internal readonly struct MessageHole : IEquatable<MessageHole>
	{
		// Token: 0x17000807 RID: 2055
		// (get) Token: 0x060027A8 RID: 10152 RVA: 0x00088F1C File Offset: 0x0008711C
		public int Start { get; }

		// Token: 0x17000808 RID: 2056
		// (get) Token: 0x060027A9 RID: 10153 RVA: 0x00088F24 File Offset: 0x00087124
		public int End { get; }

		// Token: 0x17000809 RID: 2057
		// (get) Token: 0x060027AA RID: 10154 RVA: 0x00088F2C File Offset: 0x0008712C
		[Nullable(2)]
		public object Value
		{
			[NullableContext(2)]
			get;
		}

		// Token: 0x1700080A RID: 2058
		// (get) Token: 0x060027AB RID: 10155 RVA: 0x00088F34 File Offset: 0x00087134
		public bool IsValueUnrepresentable { get; }

		// Token: 0x060027AC RID: 10156 RVA: 0x00088F3C File Offset: 0x0008713C
		public MessageHole(int start, int end)
		{
			this.Value = null;
			this.IsValueUnrepresentable = true;
			this.Start = start;
			this.End = end;
		}

		// Token: 0x060027AD RID: 10157 RVA: 0x00088F5A File Offset: 0x0008715A
		[NullableContext(2)]
		public MessageHole(int start, int end, object value)
		{
			this.Value = value;
			this.IsValueUnrepresentable = false;
			this.Start = start;
			this.End = end;
		}

		// Token: 0x060027AE RID: 10158 RVA: 0x00088F78 File Offset: 0x00087178
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("MessageHole");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x060027AF RID: 10159 RVA: 0x00088FC4 File Offset: 0x000871C4
		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("Start = ");
			builder.Append(this.Start.ToString());
			builder.Append(", End = ");
			builder.Append(this.End.ToString());
			builder.Append(", Value = ");
			builder.Append(this.Value);
			builder.Append(", IsValueUnrepresentable = ");
			builder.Append(this.IsValueUnrepresentable.ToString());
			return true;
		}

		// Token: 0x060027B0 RID: 10160 RVA: 0x00089060 File Offset: 0x00087260
		[CompilerGenerated]
		public static bool operator !=(MessageHole left, MessageHole right)
		{
			return !(left == right);
		}

		// Token: 0x060027B1 RID: 10161 RVA: 0x0008906C File Offset: 0x0008726C
		[CompilerGenerated]
		public static bool operator ==(MessageHole left, MessageHole right)
		{
			return left.Equals(right);
		}

		// Token: 0x060027B2 RID: 10162 RVA: 0x00089078 File Offset: 0x00087278
		[CompilerGenerated]
		public override int GetHashCode()
		{
			return ((EqualityComparer<int>.Default.GetHashCode(this.<Start>k__BackingField) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.<End>k__BackingField)) * -1521134295 + EqualityComparer<object>.Default.GetHashCode(this.<Value>k__BackingField)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.<IsValueUnrepresentable>k__BackingField);
		}

		// Token: 0x060027B3 RID: 10163 RVA: 0x000890DA File Offset: 0x000872DA
		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return obj is MessageHole && this.Equals((MessageHole)obj);
		}

		// Token: 0x060027B4 RID: 10164 RVA: 0x000890F4 File Offset: 0x000872F4
		[CompilerGenerated]
		public bool Equals(MessageHole other)
		{
			return EqualityComparer<int>.Default.Equals(this.<Start>k__BackingField, other.<Start>k__BackingField) && EqualityComparer<int>.Default.Equals(this.<End>k__BackingField, other.<End>k__BackingField) && EqualityComparer<object>.Default.Equals(this.<Value>k__BackingField, other.<Value>k__BackingField) && EqualityComparer<bool>.Default.Equals(this.<IsValueUnrepresentable>k__BackingField, other.<IsValueUnrepresentable>k__BackingField);
		}
	}
}
