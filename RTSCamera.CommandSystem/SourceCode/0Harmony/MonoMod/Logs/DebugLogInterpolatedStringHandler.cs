using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace MonoMod.Logs
{
	// Token: 0x02000822 RID: 2082
	[InterpolatedStringHandler]
	internal ref struct DebugLogInterpolatedStringHandler
	{
		// Token: 0x060027FC RID: 10236 RVA: 0x0008A1E8 File Offset: 0x000883E8
		public DebugLogInterpolatedStringHandler(int literalLength, int formattedCount, bool enabled, bool recordHoles, out bool isEnabled)
		{
			this._pos = (this.holeBegin = (this.holePos = 0));
			isEnabled = enabled;
			this.enabled = enabled;
			if (!enabled)
			{
				this._chars = (this._arrayToReturnToPool = null);
				this.holes = default(Memory<MessageHole>);
				return;
			}
			this._chars = (this._arrayToReturnToPool = ArrayPool<char>.Shared.Rent(DebugLogInterpolatedStringHandler.GetDefaultLength(literalLength, formattedCount)));
			if (recordHoles)
			{
				this.holes = new MessageHole[formattedCount];
				return;
			}
			this.holes = default(Memory<MessageHole>);
		}

		// Token: 0x060027FD RID: 10237 RVA: 0x0008A288 File Offset: 0x00088488
		public DebugLogInterpolatedStringHandler(int literalLength, int formattedCount, out bool isEnabled)
		{
			DebugLog instance = DebugLog.Instance;
			this._pos = (this.holeBegin = (this.holePos = 0));
			if (!instance.ShouldLog)
			{
				this.enabled = (isEnabled = false);
				this._chars = (this._arrayToReturnToPool = null);
				this.holes = default(Memory<MessageHole>);
				return;
			}
			this.enabled = (isEnabled = true);
			this._chars = (this._arrayToReturnToPool = ArrayPool<char>.Shared.Rent(DebugLogInterpolatedStringHandler.GetDefaultLength(literalLength, formattedCount)));
			if (instance.RecordHoles)
			{
				this.holes = new MessageHole[formattedCount];
				return;
			}
			this.holes = default(Memory<MessageHole>);
		}

		// Token: 0x060027FE RID: 10238 RVA: 0x0008A344 File Offset: 0x00088544
		public DebugLogInterpolatedStringHandler(int literalLength, int formattedCount, LogLevel level, out bool isEnabled)
		{
			DebugLog instance = DebugLog.Instance;
			this._pos = (this.holeBegin = (this.holePos = 0));
			if (!instance.ShouldLogLevel(level))
			{
				this.enabled = (isEnabled = false);
				this._chars = (this._arrayToReturnToPool = null);
				this.holes = default(Memory<MessageHole>);
				return;
			}
			this.enabled = (isEnabled = true);
			this._chars = (this._arrayToReturnToPool = ArrayPool<char>.Shared.Rent(DebugLogInterpolatedStringHandler.GetDefaultLength(literalLength, formattedCount)));
			if (instance.ShouldLevelRecordHoles(level))
			{
				this.holes = new MessageHole[formattedCount];
				return;
			}
			this.holes = default(Memory<MessageHole>);
		}

		// Token: 0x060027FF RID: 10239 RVA: 0x00057B87 File Offset: 0x00055D87
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int GetDefaultLength(int literalLength, int formattedCount)
		{
			return Math.Max(256, literalLength + formattedCount * 11);
		}

		// Token: 0x17000815 RID: 2069
		// (get) Token: 0x06002800 RID: 10240 RVA: 0x0008A401 File Offset: 0x00088601
		internal ReadOnlySpan<char> Text
		{
			get
			{
				return this._chars.Slice(0, this._pos);
			}
		}

		// Token: 0x06002801 RID: 10241 RVA: 0x0008A41C File Offset: 0x0008861C
		[NullableContext(1)]
		public override string ToString()
		{
			return this.Text.ToString();
		}

		// Token: 0x06002802 RID: 10242 RVA: 0x0008A440 File Offset: 0x00088640
		[NullableContext(1)]
		public string ToStringAndClear()
		{
			string text = this.Text.ToString();
			this.Clear();
			return text;
		}

		// Token: 0x06002803 RID: 10243 RVA: 0x0008A467 File Offset: 0x00088667
		[return: Nullable(1)]
		internal string ToStringAndClear(out ReadOnlyMemory<MessageHole> holes)
		{
			holes = this.holes;
			return this.ToStringAndClear();
		}

		// Token: 0x06002804 RID: 10244 RVA: 0x0008A480 File Offset: 0x00088680
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Clear()
		{
			char[] arrayToReturnToPool = this._arrayToReturnToPool;
			this = default(DebugLogInterpolatedStringHandler);
			if (arrayToReturnToPool != null)
			{
				ArrayPool<char>.Shared.Return(arrayToReturnToPool, false);
			}
		}

		// Token: 0x06002805 RID: 10245 RVA: 0x0008A4AC File Offset: 0x000886AC
		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void AppendLiteral(string value)
		{
			if (value.Length == 1)
			{
				Span<char> chars = this._chars;
				int pos = this._pos;
				if (pos < chars.Length)
				{
					*chars[pos] = value[0];
					this._pos = pos + 1;
					return;
				}
				this.GrowThenCopyString(value);
				return;
			}
			else
			{
				if (value.Length != 2)
				{
					this.AppendStringDirect(value);
					return;
				}
				Span<char> chars2 = this._chars;
				int pos2 = this._pos;
				if ((ulong)pos2 < (ulong)((long)(chars2.Length - 1)))
				{
					value.AsSpan().CopyTo(chars2.Slice(pos2));
					this._pos = pos2 + 2;
					return;
				}
				this.GrowThenCopyString(value);
				return;
			}
		}

		// Token: 0x06002806 RID: 10246 RVA: 0x0008A554 File Offset: 0x00088754
		[NullableContext(1)]
		private void AppendStringDirect(string value)
		{
			if (value.AsSpan().TryCopyTo(this._chars.Slice(this._pos)))
			{
				this._pos += value.Length;
				return;
			}
			this.GrowThenCopyString(value);
		}

		// Token: 0x06002807 RID: 10247 RVA: 0x0008A59D File Offset: 0x0008879D
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void BeginHole()
		{
			this.holeBegin = this._pos;
		}

		// Token: 0x06002808 RID: 10248 RVA: 0x0008A5AB File Offset: 0x000887AB
		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EndHole(object obj, bool reprd)
		{
			this.EndHole<object>(in obj, reprd);
		}

		// Token: 0x06002809 RID: 10249 RVA: 0x0008A5B8 File Offset: 0x000887B8
		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private unsafe void EndHole<[Nullable(2)] T>(in T obj, bool reprd)
		{
			if (!this.holes.IsEmpty)
			{
				Span<MessageHole> span = this.holes.Span;
				int num = this.holePos;
				this.holePos = num + 1;
				*span[num] = (reprd ? new MessageHole(this.holeBegin, this._pos, obj) : new MessageHole(this.holeBegin, this._pos));
			}
		}

		// Token: 0x0600280A RID: 10250 RVA: 0x0008A630 File Offset: 0x00088830
		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendFormatted(string value)
		{
			this.BeginHole();
			if (value != null && value.AsSpan().TryCopyTo(this._chars.Slice(this._pos)))
			{
				this._pos += value.Length;
			}
			else
			{
				this.AppendFormattedSlow(value);
			}
			this.EndHole<string>(in value, true);
		}

		// Token: 0x0600280B RID: 10251 RVA: 0x0008A68C File Offset: 0x0008888C
		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private void AppendFormattedSlow(string value)
		{
			if (value != null)
			{
				this.EnsureCapacityForAdditionalChars(value.Length);
				value.AsSpan().CopyTo(this._chars.Slice(this._pos));
				this._pos += value.Length;
			}
		}

		// Token: 0x0600280C RID: 10252 RVA: 0x0008A6DA File Offset: 0x000888DA
		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendFormatted(string value, int alignment = 0, string format = null)
		{
			this.AppendFormatted<string>(value, alignment, format);
		}

		// Token: 0x0600280D RID: 10253 RVA: 0x0008A6E8 File Offset: 0x000888E8
		public void AppendFormatted(ReadOnlySpan<char> value)
		{
			this.BeginHole();
			if (value.TryCopyTo(this._chars.Slice(this._pos)))
			{
				this._pos += value.Length;
			}
			else
			{
				this.GrowThenCopySpan(value);
			}
			this.EndHole(null, false);
		}

		// Token: 0x0600280E RID: 10254 RVA: 0x0008A73C File Offset: 0x0008893C
		public void AppendFormatted(ReadOnlySpan<char> value, int alignment = 0, [Nullable(2)] string format = null)
		{
			bool flag = false;
			if (alignment < 0)
			{
				flag = true;
				alignment = -alignment;
			}
			int num = alignment - value.Length;
			if (num <= 0)
			{
				this.AppendFormatted(value);
				return;
			}
			this.BeginHole();
			this.EnsureCapacityForAdditionalChars(value.Length + num);
			if (flag)
			{
				value.CopyTo(this._chars.Slice(this._pos));
				this._pos += value.Length;
				this._chars.Slice(this._pos, num).Fill(' ');
				this._pos += num;
			}
			else
			{
				this._chars.Slice(this._pos, num).Fill(' ');
				this._pos += num;
				value.CopyTo(this._chars.Slice(this._pos));
				this._pos += value.Length;
			}
			this.EndHole(null, false);
		}

		// Token: 0x0600280F RID: 10255 RVA: 0x0008A83C File Offset: 0x00088A3C
		[NullableContext(1)]
		public unsafe void AppendFormatted<[Nullable(2)] T>(T value)
		{
			if (typeof(T) == typeof(IntPtr))
			{
				this.AppendFormatted(*Unsafe.As<T, IntPtr>(ref value));
				return;
			}
			if (typeof(T) == typeof(UIntPtr))
			{
				this.AppendFormatted(*Unsafe.As<T, UIntPtr>(ref value));
				return;
			}
			this.BeginHole();
			object obj;
			if (DebugFormatter.CanDebugFormat<T>(in value, out obj))
			{
				int num;
				while (!DebugFormatter.TryFormatInto<T>(in value, obj, this._chars.Slice(this._pos), out num))
				{
					this.Grow();
				}
				this._pos += num;
				return;
			}
			string text;
			if (value is IFormattable)
			{
				text = ((IFormattable)((object)value)).ToString(null, null);
			}
			else
			{
				ref T ptr = ref value;
				T t = default(T);
				string text2;
				if (t == null)
				{
					t = value;
					ptr = ref t;
					if (t == null)
					{
						text2 = null;
						goto IL_00EC;
					}
				}
				text2 = ptr.ToString();
				IL_00EC:
				text = text2;
			}
			if (text != null)
			{
				this.AppendStringDirect(text);
			}
			this.EndHole<T>(in value, true);
		}

		// Token: 0x06002810 RID: 10256 RVA: 0x0008A949 File Offset: 0x00088B49
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AppendFormatted(IntPtr value)
		{
			if (IntPtr.Size == 4)
			{
				this.AppendFormatted<int>((int)value);
				return;
			}
			this.AppendFormatted<long>((long)value);
		}

		// Token: 0x06002811 RID: 10257 RVA: 0x0008A96C File Offset: 0x00088B6C
		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AppendFormatted(IntPtr value, string format)
		{
			if (IntPtr.Size == 4)
			{
				this.AppendFormatted<int>((int)value, format);
				return;
			}
			this.AppendFormatted<long>((long)value, format);
		}

		// Token: 0x06002812 RID: 10258 RVA: 0x0008A991 File Offset: 0x00088B91
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AppendFormatted(UIntPtr value)
		{
			if (UIntPtr.Size == 4)
			{
				this.AppendFormatted<uint>((uint)value);
				return;
			}
			this.AppendFormatted<ulong>((ulong)value);
		}

		// Token: 0x06002813 RID: 10259 RVA: 0x0008A9B4 File Offset: 0x00088BB4
		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AppendFormatted(UIntPtr value, string format)
		{
			if (UIntPtr.Size == 4)
			{
				this.AppendFormatted<uint>((uint)value, format);
				return;
			}
			this.AppendFormatted<ulong>((ulong)value, format);
		}

		// Token: 0x06002814 RID: 10260 RVA: 0x0008A9DC File Offset: 0x00088BDC
		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendFormatted<[Nullable(2)] T>(T value, int alignment)
		{
			int pos = this._pos;
			this.AppendFormatted<T>(value);
			if (alignment != 0)
			{
				this.AppendOrInsertAlignmentIfNeeded(pos, alignment);
			}
		}

		// Token: 0x06002815 RID: 10261 RVA: 0x0008AA04 File Offset: 0x00088C04
		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void AppendFormatted<T>([Nullable(1)] T value, string format)
		{
			if (typeof(T) == typeof(IntPtr))
			{
				this.AppendFormatted(*Unsafe.As<T, IntPtr>(ref value), format);
				return;
			}
			if (typeof(T) == typeof(UIntPtr))
			{
				this.AppendFormatted(*Unsafe.As<T, UIntPtr>(ref value), format);
				return;
			}
			this.BeginHole();
			object obj;
			if (DebugFormatter.CanDebugFormat<T>(in value, out obj))
			{
				int num;
				while (!DebugFormatter.TryFormatInto<T>(in value, obj, this._chars.Slice(this._pos), out num))
				{
					this.Grow();
				}
				this._pos += num;
				return;
			}
			string text;
			if (value is IFormattable)
			{
				text = ((IFormattable)((object)value)).ToString(format, null);
			}
			else
			{
				ref T ptr = ref value;
				T t = default(T);
				string text2;
				if (t == null)
				{
					t = value;
					ptr = ref t;
					if (t == null)
					{
						text2 = null;
						goto IL_00EE;
					}
				}
				text2 = ptr.ToString();
				IL_00EE:
				text = text2;
			}
			if (text != null)
			{
				this.AppendStringDirect(text);
			}
			this.EndHole<T>(in value, true);
		}

		// Token: 0x06002816 RID: 10262 RVA: 0x0008AB14 File Offset: 0x00088D14
		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendFormatted<T>([Nullable(1)] T value, int alignment, string format)
		{
			int pos = this._pos;
			this.AppendFormatted<T>(value, format);
			if (alignment != 0)
			{
				this.AppendOrInsertAlignmentIfNeeded(pos, alignment);
			}
		}

		// Token: 0x06002817 RID: 10263 RVA: 0x0008AB3C File Offset: 0x00088D3C
		private void AppendOrInsertAlignmentIfNeeded(int startingPos, int alignment)
		{
			int num = this._pos - startingPos;
			bool flag = false;
			if (alignment < 0)
			{
				flag = true;
				alignment = -alignment;
			}
			int num2 = alignment - num;
			if (num2 > 0)
			{
				this.EnsureCapacityForAdditionalChars(num2);
				if (flag)
				{
					this._chars.Slice(this._pos, num2).Fill(' ');
				}
				else
				{
					this._chars.Slice(startingPos, num).CopyTo(this._chars.Slice(startingPos + num2));
					this._chars.Slice(startingPos, num2).Fill(' ');
				}
				this._pos += num2;
			}
		}

		// Token: 0x06002818 RID: 10264 RVA: 0x0008ABD6 File Offset: 0x00088DD6
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureCapacityForAdditionalChars(int additionalChars)
		{
			if (this._chars.Length - this._pos < additionalChars)
			{
				this.Grow(additionalChars);
			}
		}

		// Token: 0x06002819 RID: 10265 RVA: 0x0008ABF4 File Offset: 0x00088DF4
		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private void GrowThenCopyString(string value)
		{
			this.Grow(value.Length);
			value.AsSpan().CopyTo(this._chars.Slice(this._pos));
			this._pos += value.Length;
		}

		// Token: 0x0600281A RID: 10266 RVA: 0x0008AC3F File Offset: 0x00088E3F
		[MethodImpl(MethodImplOptions.NoInlining)]
		private void GrowThenCopySpan(ReadOnlySpan<char> value)
		{
			this.Grow(value.Length);
			value.CopyTo(this._chars.Slice(this._pos));
			this._pos += value.Length;
		}

		// Token: 0x0600281B RID: 10267 RVA: 0x0008AC7A File Offset: 0x00088E7A
		[MethodImpl(MethodImplOptions.NoInlining)]
		private void Grow(int additionalChars)
		{
			this.GrowCore((uint)(this._pos + additionalChars));
		}

		// Token: 0x0600281C RID: 10268 RVA: 0x0008AC8A File Offset: 0x00088E8A
		[MethodImpl(MethodImplOptions.NoInlining)]
		private void Grow()
		{
			this.GrowCore((uint)(this._chars.Length + 1));
		}

		// Token: 0x0600281D RID: 10269 RVA: 0x0008ACA0 File Offset: 0x00088EA0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void GrowCore(uint requiredMinCapacity)
		{
			int num = (int)MathEx.Clamp(Math.Max(requiredMinCapacity, Math.Min((uint)(this._chars.Length * 2), uint.MaxValue)), 256U, 2147483647U);
			char[] array = ArrayPool<char>.Shared.Rent(num);
			this._chars.Slice(0, this._pos).CopyTo(array);
			char[] arrayToReturnToPool = this._arrayToReturnToPool;
			this._chars = (this._arrayToReturnToPool = array);
			if (arrayToReturnToPool != null)
			{
				ArrayPool<char>.Shared.Return(arrayToReturnToPool, false);
			}
		}

		// Token: 0x04003A0E RID: 14862
		private const int GuessedLengthPerHole = 11;

		// Token: 0x04003A0F RID: 14863
		private const int MinimumArrayPoolLength = 256;

		// Token: 0x04003A10 RID: 14864
		[Nullable(2)]
		private char[] _arrayToReturnToPool;

		// Token: 0x04003A11 RID: 14865
		private Span<char> _chars;

		// Token: 0x04003A12 RID: 14866
		private int _pos;

		// Token: 0x04003A13 RID: 14867
		private int holeBegin;

		// Token: 0x04003A14 RID: 14868
		private int holePos;

		// Token: 0x04003A15 RID: 14869
		private Memory<MessageHole> holes;

		// Token: 0x04003A16 RID: 14870
		internal readonly bool enabled;
	}
}
