using System;
using System.Runtime.CompilerServices;

namespace MonoMod.Logs
{
	// Token: 0x02000816 RID: 2070
	[NullableContext(1)]
	[Nullable(0)]
	[InterpolatedStringHandler]
	internal ref struct FormatInterpolatedStringHandler
	{
		// Token: 0x0600279C RID: 10140 RVA: 0x00088E58 File Offset: 0x00087058
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public FormatInterpolatedStringHandler(int literalLen, int formattedCount)
		{
			bool flag;
			this.handler = new DebugLogInterpolatedStringHandler(literalLen, formattedCount, true, false, out flag);
		}

		// Token: 0x0600279D RID: 10141 RVA: 0x00088E76 File Offset: 0x00087076
		public override string ToString()
		{
			return this.handler.ToString();
		}

		// Token: 0x0600279E RID: 10142 RVA: 0x00088E89 File Offset: 0x00087089
		public string ToStringAndClear()
		{
			return this.handler.ToStringAndClear();
		}

		// Token: 0x0600279F RID: 10143 RVA: 0x00088E96 File Offset: 0x00087096
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendLiteral(string s)
		{
			this.handler.AppendLiteral(s);
		}

		// Token: 0x060027A0 RID: 10144 RVA: 0x00088EA4 File Offset: 0x000870A4
		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendFormatted(string s)
		{
			this.handler.AppendFormatted(s);
		}

		// Token: 0x060027A1 RID: 10145 RVA: 0x00088EB2 File Offset: 0x000870B2
		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendFormatted(string s, int alignment = 0, string format = null)
		{
			this.handler.AppendFormatted(s, alignment, format);
		}

		// Token: 0x060027A2 RID: 10146 RVA: 0x00088EC2 File Offset: 0x000870C2
		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendFormatted(ReadOnlySpan<char> s)
		{
			this.handler.AppendFormatted(s);
		}

		// Token: 0x060027A3 RID: 10147 RVA: 0x00088ED0 File Offset: 0x000870D0
		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendFormatted(ReadOnlySpan<char> s, int alignment = 0, [Nullable(2)] string format = null)
		{
			this.handler.AppendFormatted(s, alignment, format);
		}

		// Token: 0x060027A4 RID: 10148 RVA: 0x00088EE0 File Offset: 0x000870E0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendFormatted<[Nullable(2)] T>(T value)
		{
			this.handler.AppendFormatted<T>(value);
		}

		// Token: 0x060027A5 RID: 10149 RVA: 0x00088EEE File Offset: 0x000870EE
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendFormatted<[Nullable(2)] T>(T value, int alignment)
		{
			this.handler.AppendFormatted<T>(value, alignment);
		}

		// Token: 0x060027A6 RID: 10150 RVA: 0x00088EFD File Offset: 0x000870FD
		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendFormatted<T>([Nullable(1)] T value, string format)
		{
			this.handler.AppendFormatted<T>(value, format);
		}

		// Token: 0x060027A7 RID: 10151 RVA: 0x00088F0C File Offset: 0x0008710C
		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendFormatted<T>([Nullable(1)] T value, int alignment, string format)
		{
			this.handler.AppendFormatted<T>(value, alignment, format);
		}

		// Token: 0x040039E6 RID: 14822
		private DebugLogInterpolatedStringHandler handler;
	}
}
