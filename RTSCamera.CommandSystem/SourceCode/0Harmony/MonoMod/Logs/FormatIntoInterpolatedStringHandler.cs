using System;
using System.Runtime.CompilerServices;

namespace MonoMod.Logs
{
	// Token: 0x02000823 RID: 2083
	[NullableContext(2)]
	[Nullable(0)]
	[InterpolatedStringHandler]
	internal ref struct FormatIntoInterpolatedStringHandler
	{
		// Token: 0x0600281E RID: 10270 RVA: 0x0008AD2F File Offset: 0x00088F2F
		[NullableContext(0)]
		public FormatIntoInterpolatedStringHandler(int literalLen, int numHoles, Span<char> into, out bool enabled)
		{
			this._chars = into;
			this.pos = 0;
			if (into.Length < literalLen)
			{
				this.incomplete = true;
				enabled = false;
				return;
			}
			this.incomplete = false;
			enabled = true;
		}

		// Token: 0x0600281F RID: 10271 RVA: 0x0008AD60 File Offset: 0x00088F60
		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe bool AppendLiteral(string value)
		{
			if (value.Length == 1)
			{
				Span<char> chars = this._chars;
				int num = this.pos;
				if (num < chars.Length)
				{
					*chars[num] = value[0];
					this.pos = num + 1;
					return true;
				}
				this.incomplete = true;
				return false;
			}
			else
			{
				if (value.Length != 2)
				{
					return this.AppendStringDirect(value);
				}
				Span<char> chars2 = this._chars;
				int num2 = this.pos;
				if ((ulong)num2 < (ulong)((long)(chars2.Length - 1)))
				{
					value.AsSpan().CopyTo(chars2.Slice(num2));
					this.pos = num2 + 2;
					return true;
				}
				this.incomplete = true;
				return false;
			}
		}

		// Token: 0x06002820 RID: 10272 RVA: 0x0008AE0C File Offset: 0x0008900C
		[NullableContext(1)]
		private bool AppendStringDirect(string value)
		{
			if (value.AsSpan().TryCopyTo(this._chars.Slice(this.pos)))
			{
				this.pos += value.Length;
				return true;
			}
			this.incomplete = true;
			return false;
		}

		// Token: 0x06002821 RID: 10273 RVA: 0x0008AE58 File Offset: 0x00089058
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool AppendFormatted(string value)
		{
			if (value == null)
			{
				return true;
			}
			if (value.AsSpan().TryCopyTo(this._chars.Slice(this.pos)))
			{
				this.pos += value.Length;
				return true;
			}
			this.incomplete = true;
			return false;
		}

		// Token: 0x06002822 RID: 10274 RVA: 0x0008AEA8 File Offset: 0x000890A8
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool AppendFormatted(string value, int alignment = 0, string format = null)
		{
			return this.AppendFormatted<string>(value, alignment, format);
		}

		// Token: 0x06002823 RID: 10275 RVA: 0x0008AEB3 File Offset: 0x000890B3
		[NullableContext(0)]
		public bool AppendFormatted(ReadOnlySpan<char> value)
		{
			if (value.TryCopyTo(this._chars.Slice(this.pos)))
			{
				this.pos += value.Length;
				return true;
			}
			this.incomplete = true;
			return false;
		}

		// Token: 0x06002824 RID: 10276 RVA: 0x0008AEF0 File Offset: 0x000890F0
		[NullableContext(0)]
		public bool AppendFormatted(ReadOnlySpan<char> value, int alignment = 0, [Nullable(2)] string format = null)
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
				return this.AppendFormatted(value);
			}
			if (this._chars.Slice(this.pos).Length < value.Length + num)
			{
				this.incomplete = true;
				return false;
			}
			if (flag)
			{
				value.CopyTo(this._chars.Slice(this.pos));
				this.pos += value.Length;
				this._chars.Slice(this.pos, num).Fill(' ');
				this.pos += num;
			}
			else
			{
				this._chars.Slice(this.pos, num).Fill(' ');
				this.pos += num;
				value.CopyTo(this._chars.Slice(this.pos));
				this.pos += value.Length;
			}
			return true;
		}

		// Token: 0x06002825 RID: 10277 RVA: 0x0008B000 File Offset: 0x00089200
		[NullableContext(1)]
		public unsafe bool AppendFormatted<[Nullable(2)] T>(T value)
		{
			if (typeof(T) == typeof(IntPtr))
			{
				return this.AppendFormatted(*Unsafe.As<T, IntPtr>(ref value));
			}
			if (typeof(T) == typeof(UIntPtr))
			{
				return this.AppendFormatted(*Unsafe.As<T, UIntPtr>(ref value));
			}
			object obj;
			if (!DebugFormatter.CanDebugFormat<T>(in value, out obj))
			{
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
							goto IL_00E8;
						}
					}
					text2 = ptr.ToString();
					IL_00E8:
					text = text2;
				}
				return text == null || this.AppendStringDirect(text);
			}
			int num;
			if (!DebugFormatter.TryFormatInto<T>(in value, obj, this._chars.Slice(this.pos), out num))
			{
				this.incomplete = true;
				return false;
			}
			this.pos += num;
			return true;
		}

		// Token: 0x06002826 RID: 10278 RVA: 0x0008B102 File Offset: 0x00089302
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool AppendFormatted(IntPtr value)
		{
			if (IntPtr.Size == 4)
			{
				return this.AppendFormatted<int>((int)value);
			}
			return this.AppendFormatted<long>((long)value);
		}

		// Token: 0x06002827 RID: 10279 RVA: 0x0008B125 File Offset: 0x00089325
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool AppendFormatted(IntPtr value, string format)
		{
			if (IntPtr.Size == 4)
			{
				return this.AppendFormatted<int>((int)value, format);
			}
			return this.AppendFormatted<long>((long)value, format);
		}

		// Token: 0x06002828 RID: 10280 RVA: 0x0008B14A File Offset: 0x0008934A
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool AppendFormatted(UIntPtr value)
		{
			if (UIntPtr.Size == 4)
			{
				return this.AppendFormatted<uint>((uint)value);
			}
			return this.AppendFormatted<ulong>((ulong)value);
		}

		// Token: 0x06002829 RID: 10281 RVA: 0x0008B16D File Offset: 0x0008936D
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool AppendFormatted(UIntPtr value, string format)
		{
			if (UIntPtr.Size == 4)
			{
				return this.AppendFormatted<uint>((uint)value, format);
			}
			return this.AppendFormatted<ulong>((ulong)value, format);
		}

		// Token: 0x0600282A RID: 10282 RVA: 0x0008B194 File Offset: 0x00089394
		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool AppendFormatted<[Nullable(2)] T>(T value, int alignment)
		{
			int num = this.pos;
			return this.AppendFormatted<T>(value) && (alignment == 0 || this.AppendOrInsertAlignmentIfNeeded(num, alignment));
		}

		// Token: 0x0600282B RID: 10283 RVA: 0x0008B1C0 File Offset: 0x000893C0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe bool AppendFormatted<T>([Nullable(1)] T value, string format)
		{
			if (typeof(T) == typeof(IntPtr))
			{
				return this.AppendFormatted(*Unsafe.As<T, IntPtr>(ref value), format);
			}
			if (typeof(T) == typeof(UIntPtr))
			{
				return this.AppendFormatted(*Unsafe.As<T, UIntPtr>(ref value), format);
			}
			object obj;
			if (!DebugFormatter.CanDebugFormat<T>(in value, out obj))
			{
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
							goto IL_00EA;
						}
					}
					text2 = ptr.ToString();
					IL_00EA:
					text = text2;
				}
				return text == null || this.AppendStringDirect(text);
			}
			int num;
			if (!DebugFormatter.TryFormatInto<T>(in value, obj, this._chars.Slice(this.pos), out num))
			{
				this.incomplete = true;
				return false;
			}
			this.pos += num;
			return true;
		}

		// Token: 0x0600282C RID: 10284 RVA: 0x0008B2C4 File Offset: 0x000894C4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool AppendFormatted<T>([Nullable(1)] T value, int alignment, string format)
		{
			int num = this.pos;
			return this.AppendFormatted<T>(value, format) && (alignment == 0 || this.AppendOrInsertAlignmentIfNeeded(num, alignment));
		}

		// Token: 0x0600282D RID: 10285 RVA: 0x0008B2F4 File Offset: 0x000894F4
		private bool AppendOrInsertAlignmentIfNeeded(int startingPos, int alignment)
		{
			int num = this.pos - startingPos;
			bool flag = false;
			if (alignment < 0)
			{
				flag = true;
				alignment = -alignment;
			}
			int num2 = alignment - num;
			if (num2 > 0)
			{
				if (this._chars.Slice(this.pos).Length < num2)
				{
					this.incomplete = true;
					return false;
				}
				if (flag)
				{
					this._chars.Slice(this.pos, num2).Fill(' ');
				}
				else
				{
					this._chars.Slice(startingPos, num).CopyTo(this._chars.Slice(startingPos + num2));
					this._chars.Slice(startingPos, num2).Fill(' ');
				}
				this.pos += num2;
			}
			return true;
		}

		// Token: 0x04003A17 RID: 14871
		[Nullable(0)]
		private readonly Span<char> _chars;

		// Token: 0x04003A18 RID: 14872
		internal int pos;

		// Token: 0x04003A19 RID: 14873
		internal bool incomplete;
	}
}
