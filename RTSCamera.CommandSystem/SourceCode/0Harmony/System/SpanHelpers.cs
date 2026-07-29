using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MonoMod;

namespace System
{
	// Token: 0x02000477 RID: 1143
	[NullableContext(1)]
	[Nullable(0)]
	internal static class SpanHelpers
	{
		// Token: 0x06001944 RID: 6468 RVA: 0x000510BB File Offset: 0x0004F2BB
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int BinarySearch<[Nullable(2)] T, [Nullable(0)] TComparable>([Nullable(new byte[] { 0, 1 })] this ReadOnlySpan<T> span, TComparable comparable) where TComparable : IComparable<T>
		{
			if (comparable == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.comparable);
			}
			return SpanHelpers.BinarySearch<T, TComparable>(MemoryMarshal.GetReference<T>(span), span.Length, comparable);
		}

		// Token: 0x06001945 RID: 6469 RVA: 0x000510E0 File Offset: 0x0004F2E0
		public unsafe static int BinarySearch<[Nullable(2)] T, [Nullable(0)] TComparable>(ref T spanStart, int length, TComparable comparable) where TComparable : IComparable<T>
		{
			int i = 0;
			int num = length - 1;
			while (i <= num)
			{
				int num2 = (int)((uint)(num + i) >> 1);
				ref TComparable ptr = ref comparable;
				if (default(TComparable) == null)
				{
					TComparable tcomparable = comparable;
					ptr = ref tcomparable;
				}
				int num3 = ptr.CompareTo(*Unsafe.Add<T>(ref spanStart, num2));
				if (num3 == 0)
				{
					return num2;
				}
				if (num3 > 0)
				{
					i = num2 + 1;
				}
				else
				{
					num = num2 - 1;
				}
			}
			return ~i;
		}

		// Token: 0x06001946 RID: 6470 RVA: 0x00051148 File Offset: 0x0004F348
		public static int IndexOf(ref byte searchSpace, int searchSpaceLength, ref byte value, int valueLength)
		{
			if (valueLength == 0)
			{
				return 0;
			}
			byte b = value;
			ref byte ptr = ref Unsafe.Add<byte>(ref value, 1);
			int num = valueLength - 1;
			int num2 = 0;
			for (;;)
			{
				int num3 = searchSpaceLength - num2 - num;
				if (num3 <= 0)
				{
					return -1;
				}
				int num4 = SpanHelpers.IndexOf(Unsafe.Add<byte>(ref searchSpace, num2), b, num3);
				if (num4 == -1)
				{
					return -1;
				}
				num2 += num4;
				if (SpanHelpers.SequenceEqual<byte>(Unsafe.Add<byte>(ref searchSpace, num2 + 1), ref ptr, num))
				{
					break;
				}
				num2++;
			}
			return num2;
		}

		// Token: 0x06001947 RID: 6471 RVA: 0x000511B0 File Offset: 0x0004F3B0
		public unsafe static int IndexOfAny(ref byte searchSpace, int searchSpaceLength, ref byte value, int valueLength)
		{
			if (valueLength == 0)
			{
				return 0;
			}
			int num = -1;
			for (int i = 0; i < valueLength; i++)
			{
				int num2 = SpanHelpers.IndexOf(ref searchSpace, *Unsafe.Add<byte>(ref value, i), searchSpaceLength);
				if (num2 < num)
				{
					num = num2;
					searchSpaceLength = num2;
					if (num == 0)
					{
						break;
					}
				}
			}
			return num;
		}

		// Token: 0x06001948 RID: 6472 RVA: 0x000511F0 File Offset: 0x0004F3F0
		public unsafe static int LastIndexOfAny(ref byte searchSpace, int searchSpaceLength, ref byte value, int valueLength)
		{
			if (valueLength == 0)
			{
				return 0;
			}
			int num = -1;
			for (int i = 0; i < valueLength; i++)
			{
				int num2 = SpanHelpers.LastIndexOf(ref searchSpace, *Unsafe.Add<byte>(ref value, i), searchSpaceLength);
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x06001949 RID: 6473 RVA: 0x00051228 File Offset: 0x0004F428
		public unsafe static int IndexOf(ref byte searchSpace, byte value, int length)
		{
			IntPtr intPtr = (IntPtr)0;
			IntPtr intPtr2 = (IntPtr)length;
			while (intPtr2 >= (IntPtr)8)
			{
				intPtr2 -= (IntPtr)8;
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr))
				{
					IL_0106:
					return (int)intPtr;
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)1))
				{
					IL_0109:
					return (int)(intPtr + (IntPtr)1);
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)2))
				{
					IL_010F:
					return (int)(intPtr + (IntPtr)2);
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)3))
				{
					IL_0115:
					return (int)(intPtr + (IntPtr)3);
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)4))
				{
					return (int)(intPtr + (IntPtr)4);
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)5))
				{
					return (int)(intPtr + (IntPtr)5);
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)6))
				{
					return (int)(intPtr + (IntPtr)6);
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)7))
				{
					return (int)(intPtr + (IntPtr)7);
				}
				intPtr += (IntPtr)8;
			}
			if (intPtr2 >= (IntPtr)4)
			{
				intPtr2 -= (IntPtr)4;
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr))
				{
					goto IL_0106;
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)1))
				{
					goto IL_0109;
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)2))
				{
					goto IL_010F;
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)3))
				{
					goto IL_0115;
				}
				intPtr += (IntPtr)4;
			}
			while (intPtr2 > (IntPtr)0)
			{
				intPtr2 -= (IntPtr)1;
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr))
				{
					goto IL_0106;
				}
				intPtr += (IntPtr)1;
			}
			return -1;
		}

		// Token: 0x0600194A RID: 6474 RVA: 0x00051368 File Offset: 0x0004F568
		public static int LastIndexOf(ref byte searchSpace, int searchSpaceLength, ref byte value, int valueLength)
		{
			if (valueLength == 0)
			{
				return 0;
			}
			byte b = value;
			ref byte ptr = ref Unsafe.Add<byte>(ref value, 1);
			int num = valueLength - 1;
			int num2 = 0;
			int num4;
			for (;;)
			{
				int num3 = searchSpaceLength - num2 - num;
				if (num3 <= 0)
				{
					return -1;
				}
				num4 = SpanHelpers.LastIndexOf(ref searchSpace, b, num3);
				if (num4 == -1)
				{
					return -1;
				}
				if (SpanHelpers.SequenceEqual<byte>(Unsafe.Add<byte>(ref searchSpace, num4 + 1), ref ptr, num))
				{
					break;
				}
				num2 += num3 - num4;
			}
			return num4;
		}

		// Token: 0x0600194B RID: 6475 RVA: 0x000513C8 File Offset: 0x0004F5C8
		public unsafe static int LastIndexOf(ref byte searchSpace, byte value, int length)
		{
			IntPtr intPtr = (IntPtr)length;
			IntPtr intPtr2 = (IntPtr)length;
			while (intPtr2 >= (IntPtr)8)
			{
				intPtr2 -= (IntPtr)8;
				intPtr -= (IntPtr)8;
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)7))
				{
					return (int)(intPtr + (IntPtr)7);
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)6))
				{
					return (int)(intPtr + (IntPtr)6);
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)5))
				{
					return (int)(intPtr + (IntPtr)5);
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)4))
				{
					return (int)(intPtr + (IntPtr)4);
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)3))
				{
					IL_010F:
					return (int)(intPtr + (IntPtr)3);
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)2))
				{
					IL_0109:
					return (int)(intPtr + (IntPtr)2);
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)1))
				{
					IL_0103:
					return (int)(intPtr + (IntPtr)1);
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr))
				{
					IL_0100:
					return (int)intPtr;
				}
			}
			if (intPtr2 >= (IntPtr)4)
			{
				intPtr2 -= (IntPtr)4;
				intPtr -= (IntPtr)4;
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)3))
				{
					goto IL_010F;
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)2))
				{
					goto IL_0109;
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)1))
				{
					goto IL_0103;
				}
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr))
				{
					goto IL_0100;
				}
			}
			while (intPtr2 > (IntPtr)0)
			{
				intPtr2 -= (IntPtr)1;
				intPtr -= (IntPtr)1;
				if (value == *Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr))
				{
					goto IL_0100;
				}
			}
			return -1;
		}

		// Token: 0x0600194C RID: 6476 RVA: 0x00051504 File Offset: 0x0004F704
		public unsafe static int IndexOfAny(ref byte searchSpace, byte value0, byte value1, int length)
		{
			IntPtr intPtr = (IntPtr)0;
			IntPtr intPtr2 = (IntPtr)length;
			while (intPtr2 >= (IntPtr)8)
			{
				intPtr2 -= (IntPtr)8;
				uint num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					IL_0198:
					return (int)intPtr;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)1));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					IL_019B:
					return (int)(intPtr + (IntPtr)1);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)2));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					IL_01A1:
					return (int)(intPtr + (IntPtr)2);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)3));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					IL_01A7:
					return (int)(intPtr + (IntPtr)3);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)4));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					return (int)(intPtr + (IntPtr)4);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)5));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					return (int)(intPtr + (IntPtr)5);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)6));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					return (int)(intPtr + (IntPtr)6);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)7));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					return (int)(intPtr + (IntPtr)7);
				}
				intPtr += (IntPtr)8;
			}
			if (intPtr2 >= (IntPtr)4)
			{
				intPtr2 -= (IntPtr)4;
				uint num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					goto IL_0198;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)1));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					goto IL_019B;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)2));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					goto IL_01A1;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)3));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					goto IL_01A7;
				}
				intPtr += (IntPtr)4;
			}
			while (intPtr2 > (IntPtr)0)
			{
				intPtr2 -= (IntPtr)1;
				uint num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					goto IL_0198;
				}
				intPtr += (IntPtr)1;
			}
			return -1;
		}

		// Token: 0x0600194D RID: 6477 RVA: 0x000516D8 File Offset: 0x0004F8D8
		public unsafe static int IndexOfAny(ref byte searchSpace, byte value0, byte value1, byte value2, int length)
		{
			IntPtr intPtr = (IntPtr)0;
			IntPtr intPtr2 = (IntPtr)length;
			while (intPtr2 >= (IntPtr)8)
			{
				intPtr2 -= (IntPtr)8;
				uint num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					IL_0207:
					return (int)intPtr;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)1));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					IL_020A:
					return (int)(intPtr + (IntPtr)1);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)2));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					IL_0210:
					return (int)(intPtr + (IntPtr)2);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)3));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					IL_0216:
					return (int)(intPtr + (IntPtr)3);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)4));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					return (int)(intPtr + (IntPtr)4);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)5));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					return (int)(intPtr + (IntPtr)5);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)6));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					return (int)(intPtr + (IntPtr)6);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)7));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					return (int)(intPtr + (IntPtr)7);
				}
				intPtr += (IntPtr)8;
			}
			if (intPtr2 >= (IntPtr)4)
			{
				intPtr2 -= (IntPtr)4;
				uint num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					goto IL_0207;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)1));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					goto IL_020A;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)2));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					goto IL_0210;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)3));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					goto IL_0216;
				}
				intPtr += (IntPtr)4;
			}
			while (intPtr2 > (IntPtr)0)
			{
				intPtr2 -= (IntPtr)1;
				uint num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					goto IL_0207;
				}
				intPtr += (IntPtr)1;
			}
			return -1;
		}

		// Token: 0x0600194E RID: 6478 RVA: 0x00051918 File Offset: 0x0004FB18
		public unsafe static int LastIndexOfAny(ref byte searchSpace, byte value0, byte value1, int length)
		{
			IntPtr intPtr = (IntPtr)length;
			IntPtr intPtr2 = (IntPtr)length;
			while (intPtr2 >= (IntPtr)8)
			{
				intPtr2 -= (IntPtr)8;
				intPtr -= (IntPtr)8;
				uint num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)7));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					return (int)(intPtr + (IntPtr)7);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)6));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					return (int)(intPtr + (IntPtr)6);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)5));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					return (int)(intPtr + (IntPtr)5);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)4));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					return (int)(intPtr + (IntPtr)4);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)3));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					IL_01A7:
					return (int)(intPtr + (IntPtr)3);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)2));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					IL_01A1:
					return (int)(intPtr + (IntPtr)2);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)1));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					IL_019B:
					return (int)(intPtr + (IntPtr)1);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					IL_0198:
					return (int)intPtr;
				}
			}
			if (intPtr2 >= (IntPtr)4)
			{
				intPtr2 -= (IntPtr)4;
				intPtr -= (IntPtr)4;
				uint num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)3));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					goto IL_01A7;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)2));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					goto IL_01A1;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)1));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					goto IL_019B;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num)
				{
					goto IL_0198;
				}
				if ((uint)value1 == num)
				{
					goto IL_0198;
				}
			}
			while (intPtr2 > (IntPtr)0)
			{
				intPtr2 -= (IntPtr)1;
				intPtr -= (IntPtr)1;
				uint num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					goto IL_0198;
				}
			}
			return -1;
		}

		// Token: 0x0600194F RID: 6479 RVA: 0x00051AEC File Offset: 0x0004FCEC
		public unsafe static int LastIndexOfAny(ref byte searchSpace, byte value0, byte value1, byte value2, int length)
		{
			IntPtr intPtr = (IntPtr)length;
			IntPtr intPtr2 = (IntPtr)length;
			while (intPtr2 >= (IntPtr)8)
			{
				intPtr2 -= (IntPtr)8;
				intPtr -= (IntPtr)8;
				uint num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)7));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					return (int)(intPtr + (IntPtr)7);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)6));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					return (int)(intPtr + (IntPtr)6);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)5));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					return (int)(intPtr + (IntPtr)5);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)4));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					return (int)(intPtr + (IntPtr)4);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)3));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					IL_0217:
					return (int)(intPtr + (IntPtr)3);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)2));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					IL_0211:
					return (int)(intPtr + (IntPtr)2);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)1));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					IL_020B:
					return (int)(intPtr + (IntPtr)1);
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					IL_0208:
					return (int)intPtr;
				}
			}
			if (intPtr2 >= (IntPtr)4)
			{
				intPtr2 -= (IntPtr)4;
				intPtr -= (IntPtr)4;
				uint num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)3));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					goto IL_0217;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)2));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					goto IL_0211;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr + (IntPtr)1));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					goto IL_020B;
				}
				num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num)
				{
					goto IL_0208;
				}
				if ((uint)value2 == num)
				{
					goto IL_0208;
				}
			}
			while (intPtr2 > (IntPtr)0)
			{
				intPtr2 -= (IntPtr)1;
				intPtr -= (IntPtr)1;
				uint num = (uint)(*Unsafe.AddByteOffset<byte>(ref searchSpace, intPtr));
				if ((uint)value0 == num || (uint)value1 == num || (uint)value2 == num)
				{
					goto IL_0208;
				}
			}
			return -1;
		}

		// Token: 0x06001950 RID: 6480 RVA: 0x00051D30 File Offset: 0x0004FF30
		public unsafe static bool SequenceEqual(ref byte first, ref byte second, [NativeInteger] UIntPtr length)
		{
			if (!Unsafe.AreSame<byte>(ref first, ref second))
			{
				IntPtr intPtr = (IntPtr)0;
				if (length >= (UIntPtr)((IntPtr)sizeof(UIntPtr)))
				{
					IntPtr intPtr2 = (IntPtr)(length - (UIntPtr)((IntPtr)sizeof(UIntPtr)));
					while (intPtr2 > intPtr)
					{
						if (Unsafe.ReadUnaligned<UIntPtr>(Unsafe.AddByteOffset<byte>(ref first, intPtr)) != Unsafe.ReadUnaligned<UIntPtr>(Unsafe.AddByteOffset<byte>(ref second, intPtr)))
						{
							return false;
						}
						intPtr += (IntPtr)sizeof(UIntPtr);
					}
					return Unsafe.ReadUnaligned<UIntPtr>(Unsafe.AddByteOffset<byte>(ref first, intPtr2)) == Unsafe.ReadUnaligned<UIntPtr>(Unsafe.AddByteOffset<byte>(ref second, intPtr2));
				}
				while (length > (UIntPtr)intPtr)
				{
					if (*Unsafe.AddByteOffset<byte>(ref first, intPtr) != *Unsafe.AddByteOffset<byte>(ref second, intPtr))
					{
						return false;
					}
					intPtr += (IntPtr)1;
				}
				return true;
			}
			return true;
		}

		// Token: 0x06001951 RID: 6481 RVA: 0x00051DD0 File Offset: 0x0004FFD0
		public unsafe static int SequenceCompareTo(ref byte first, int firstLength, ref byte second, int secondLength)
		{
			if (!Unsafe.AreSame<byte>(ref first, ref second))
			{
				IntPtr intPtr = (IntPtr)((firstLength < secondLength) ? firstLength : secondLength);
				IntPtr intPtr2 = (IntPtr)0;
				IntPtr intPtr3 = intPtr;
				if (intPtr3 > (IntPtr)sizeof(UIntPtr))
				{
					intPtr3 -= (IntPtr)sizeof(UIntPtr);
					while (intPtr3 > intPtr2)
					{
						if (Unsafe.ReadUnaligned<UIntPtr>(Unsafe.AddByteOffset<byte>(ref first, intPtr2)) != Unsafe.ReadUnaligned<UIntPtr>(Unsafe.AddByteOffset<byte>(ref second, intPtr2)))
						{
							break;
						}
						intPtr2 += (IntPtr)sizeof(UIntPtr);
					}
				}
				while (intPtr > intPtr2)
				{
					int num = Unsafe.AddByteOffset<byte>(ref first, intPtr2).CompareTo(*Unsafe.AddByteOffset<byte>(ref second, intPtr2));
					if (num != 0)
					{
						return num;
					}
					intPtr2 += (IntPtr)1;
				}
			}
			return firstLength - secondLength;
		}

		// Token: 0x06001952 RID: 6482 RVA: 0x00051E60 File Offset: 0x00050060
		public unsafe static int SequenceCompareTo(ref char first, int firstLength, ref char second, int secondLength)
		{
			int num = firstLength - secondLength;
			if (!Unsafe.AreSame<char>(ref first, ref second))
			{
				IntPtr intPtr = (IntPtr)((firstLength < secondLength) ? firstLength : secondLength);
				IntPtr intPtr2 = (IntPtr)0;
				if (intPtr >= (IntPtr)(sizeof(UIntPtr) / 2))
				{
					while (intPtr >= intPtr2 + (IntPtr)(sizeof(UIntPtr) / 2) && !(Unsafe.ReadUnaligned<UIntPtr>(Unsafe.As<char, byte>(Unsafe.Add<char>(ref first, intPtr2))) != Unsafe.ReadUnaligned<UIntPtr>(Unsafe.As<char, byte>(Unsafe.Add<char>(ref second, intPtr2)))))
					{
						intPtr2 += (IntPtr)(sizeof(UIntPtr) / 2);
					}
				}
				if (sizeof(UIntPtr) > 4 && intPtr >= intPtr2 + (IntPtr)2 && Unsafe.ReadUnaligned<int>(Unsafe.As<char, byte>(Unsafe.Add<char>(ref first, intPtr2))) == Unsafe.ReadUnaligned<int>(Unsafe.As<char, byte>(Unsafe.Add<char>(ref second, intPtr2))))
				{
					intPtr2 += (IntPtr)2;
				}
				while (intPtr2 < intPtr)
				{
					int num2 = Unsafe.Add<char>(ref first, intPtr2).CompareTo(*Unsafe.Add<char>(ref second, intPtr2));
					if (num2 != 0)
					{
						return num2;
					}
					intPtr2 += (IntPtr)1;
				}
			}
			return num;
		}

		// Token: 0x06001953 RID: 6483 RVA: 0x00051F3C File Offset: 0x0005013C
		public unsafe static int IndexOf(ref char searchSpace, char value, int length)
		{
			fixed (char* ptr = &searchSpace)
			{
				char* ptr2 = ptr;
				char* ptr3 = ptr2;
				IntPtr intPtr = (IntPtr)length;
				while (length >= 4)
				{
					length -= 4;
					if (*ptr3 != value)
					{
						if (ptr3[1] != value)
						{
							if (ptr3[2] != value)
							{
								if (ptr3[3] != value)
								{
									ptr3 += 4;
									continue;
								}
								ptr3++;
							}
							ptr3++;
						}
						ptr3++;
					}
					IL_005E:
					return (int)((long)(ptr3 - ptr2));
				}
				while (length > 0)
				{
					length--;
					if (*ptr3 == value)
					{
						goto IL_005E;
					}
					ptr3++;
				}
				return -1;
			}
		}

		// Token: 0x06001954 RID: 6484 RVA: 0x00051FB0 File Offset: 0x000501B0
		public unsafe static int LastIndexOf(ref char searchSpace, char value, int length)
		{
			fixed (char* ptr = &searchSpace)
			{
				char* ptr2 = ptr;
				char* ptr3 = ptr2 + length;
				char* ptr4 = ptr2;
				while (length >= 4)
				{
					length -= 4;
					ptr3 -= 4;
					if (ptr3[3] == value)
					{
						return (int)((long)(ptr3 - ptr4)) + 3;
					}
					if (ptr3[2] == value)
					{
						return (int)((long)(ptr3 - ptr4)) + 2;
					}
					if (ptr3[1] == value)
					{
						return (int)((long)(ptr3 - ptr4)) + 1;
					}
					if (*ptr3 == value)
					{
						IL_0054:
						return (int)((long)(ptr3 - ptr4));
					}
				}
				while (length > 0)
				{
					length--;
					ptr3--;
					if (*ptr3 == value)
					{
						goto IL_0054;
					}
				}
				return -1;
			}
		}

		// Token: 0x06001955 RID: 6485 RVA: 0x00052038 File Offset: 0x00050238
		public unsafe static void CopyTo<[Nullable(2)] T>(ref T dst, int dstLength, ref T src, int srcLength)
		{
			IntPtr intPtr = Unsafe.ByteOffset<T>(ref src, Unsafe.Add<T>(ref src, srcLength));
			IntPtr intPtr2 = Unsafe.ByteOffset<T>(ref dst, Unsafe.Add<T>(ref dst, dstLength));
			IntPtr intPtr3 = Unsafe.ByteOffset<T>(ref src, ref dst);
			if (!((sizeof(IntPtr) == 4) ? ((int)intPtr3 < (int)intPtr || (int)intPtr3 > -(int)intPtr2) : ((long)intPtr3 < (long)intPtr || (long)intPtr3 > -(long)intPtr2)) && !SpanHelpers.IsReferenceOrContainsReferences<T>())
			{
				ref byte ptr = ref Unsafe.As<T, byte>(ref dst);
				ref byte ptr2 = ref Unsafe.As<T, byte>(ref src);
				ulong num = (ulong)(long)intPtr;
				uint num3;
				for (ulong num2 = 0UL; num2 < num; num2 += (ulong)num3)
				{
					num3 = ((num - num2 > (ulong)(-1)) ? uint.MaxValue : ((uint)(num - num2)));
					Unsafe.CopyBlock(Unsafe.Add<byte>(ref ptr, (IntPtr)((long)num2)), Unsafe.Add<byte>(ref ptr2, (IntPtr)((long)num2)), num3);
				}
				return;
			}
			bool flag = ((sizeof(IntPtr) == 4) ? ((int)intPtr3 > -(int)intPtr2) : ((long)intPtr3 > -(long)intPtr2));
			int num4 = (flag ? 1 : (-1));
			int num5 = (flag ? 0 : (srcLength - 1));
			int i;
			for (i = 0; i < (srcLength & -8); i += 8)
			{
				*Unsafe.Add<T>(ref dst, num5) = *Unsafe.Add<T>(ref src, num5);
				*Unsafe.Add<T>(ref dst, num5 + num4) = *Unsafe.Add<T>(ref src, num5 + num4);
				*Unsafe.Add<T>(ref dst, num5 + num4 * 2) = *Unsafe.Add<T>(ref src, num5 + num4 * 2);
				*Unsafe.Add<T>(ref dst, num5 + num4 * 3) = *Unsafe.Add<T>(ref src, num5 + num4 * 3);
				*Unsafe.Add<T>(ref dst, num5 + num4 * 4) = *Unsafe.Add<T>(ref src, num5 + num4 * 4);
				*Unsafe.Add<T>(ref dst, num5 + num4 * 5) = *Unsafe.Add<T>(ref src, num5 + num4 * 5);
				*Unsafe.Add<T>(ref dst, num5 + num4 * 6) = *Unsafe.Add<T>(ref src, num5 + num4 * 6);
				*Unsafe.Add<T>(ref dst, num5 + num4 * 7) = *Unsafe.Add<T>(ref src, num5 + num4 * 7);
				num5 += num4 * 8;
			}
			if (i < (srcLength & -4))
			{
				*Unsafe.Add<T>(ref dst, num5) = *Unsafe.Add<T>(ref src, num5);
				*Unsafe.Add<T>(ref dst, num5 + num4) = *Unsafe.Add<T>(ref src, num5 + num4);
				*Unsafe.Add<T>(ref dst, num5 + num4 * 2) = *Unsafe.Add<T>(ref src, num5 + num4 * 2);
				*Unsafe.Add<T>(ref dst, num5 + num4 * 3) = *Unsafe.Add<T>(ref src, num5 + num4 * 3);
				num5 += num4 * 4;
				i += 4;
			}
			while (i < srcLength)
			{
				*Unsafe.Add<T>(ref dst, num5) = *Unsafe.Add<T>(ref src, num5);
				num5 += num4;
				i++;
			}
		}

		// Token: 0x06001956 RID: 6486 RVA: 0x0005235C File Offset: 0x0005055C
		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static IntPtr Add<T>(this IntPtr start, int index)
		{
			if (sizeof(IntPtr) == 4)
			{
				uint num = (uint)(index * Unsafe.SizeOf<T>());
				return (IntPtr)((void*)((byte*)(void*)start + num));
			}
			ulong num2 = (ulong)((long)index * (long)Unsafe.SizeOf<T>());
			return (IntPtr)((void*)((byte*)(void*)start + num2));
		}

		// Token: 0x06001957 RID: 6487 RVA: 0x000523A1 File Offset: 0x000505A1
		[NullableContext(2)]
		public static bool IsReferenceOrContainsReferences<T>()
		{
			return SpanHelpers.PerTypeValues<T>.IsReferenceOrContainsReferences;
		}

		// Token: 0x06001958 RID: 6488 RVA: 0x000523A8 File Offset: 0x000505A8
		private static bool IsReferenceOrContainsReferencesCore(Type type)
		{
			if (type.GetTypeInfo().IsPrimitive)
			{
				return false;
			}
			if (!type.GetTypeInfo().IsValueType)
			{
				return true;
			}
			Type underlyingType = Nullable.GetUnderlyingType(type);
			if (underlyingType != null)
			{
				type = underlyingType;
			}
			if (type.GetTypeInfo().IsEnum)
			{
				return false;
			}
			foreach (FieldInfo fieldInfo in type.GetTypeInfo().DeclaredFields)
			{
				if (!fieldInfo.IsStatic && SpanHelpers.IsReferenceOrContainsReferencesCore(fieldInfo.FieldType))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001959 RID: 6489 RVA: 0x00052450 File Offset: 0x00050650
		[NullableContext(0)]
		public unsafe static void ClearLessThanPointerSized(byte* ptr, UIntPtr byteLength)
		{
			if (sizeof(UIntPtr) == 4)
			{
				Unsafe.InitBlockUnaligned((void*)ptr, 0, (uint)byteLength);
				return;
			}
			ulong num = (ulong)byteLength;
			uint num2 = (uint)(num & (ulong)(-1));
			Unsafe.InitBlockUnaligned((void*)ptr, 0, num2);
			num -= (ulong)num2;
			ptr += num2;
			while (num > 0UL)
			{
				num2 = ((num >= (ulong)(-1)) ? uint.MaxValue : ((uint)num));
				Unsafe.InitBlockUnaligned((void*)ptr, 0, num2);
				ptr += num2;
				num -= (ulong)num2;
			}
		}

		// Token: 0x0600195A RID: 6490 RVA: 0x000524BC File Offset: 0x000506BC
		public static void ClearLessThanPointerSized(ref byte b, UIntPtr byteLength)
		{
			if (sizeof(UIntPtr) == 4)
			{
				Unsafe.InitBlockUnaligned(ref b, 0, (uint)byteLength);
				return;
			}
			ulong num = (ulong)byteLength;
			uint num2 = (uint)(num & (ulong)(-1));
			Unsafe.InitBlockUnaligned(ref b, 0, num2);
			num -= (ulong)num2;
			long num3 = (long)((ulong)num2);
			while (num > 0UL)
			{
				num2 = ((num >= (ulong)(-1)) ? uint.MaxValue : ((uint)num));
				Unsafe.InitBlockUnaligned(Unsafe.Add<byte>(ref b, (IntPtr)num3), 0, num2);
				num3 += (long)((ulong)num2);
				num -= (ulong)num2;
			}
		}

		// Token: 0x0600195B RID: 6491 RVA: 0x0005252C File Offset: 0x0005072C
		public unsafe static void ClearPointerSizedWithoutReferences(ref byte b, [NativeInteger] UIntPtr byteLength)
		{
			IntPtr intPtr = (IntPtr)0;
			while (intPtr.LessThanEqual(byteLength - (UIntPtr)((IntPtr)sizeof(SpanHelpers.Reg64))))
			{
				*Unsafe.As<byte, SpanHelpers.Reg64>(Unsafe.Add<byte>(ref b, intPtr)) = default(SpanHelpers.Reg64);
				intPtr += (IntPtr)sizeof(SpanHelpers.Reg64);
			}
			if (intPtr.LessThanEqual(byteLength - (UIntPtr)((IntPtr)sizeof(SpanHelpers.Reg32))))
			{
				*Unsafe.As<byte, SpanHelpers.Reg32>(Unsafe.Add<byte>(ref b, intPtr)) = default(SpanHelpers.Reg32);
				intPtr += (IntPtr)sizeof(SpanHelpers.Reg32);
			}
			if (intPtr.LessThanEqual(byteLength - (UIntPtr)((IntPtr)sizeof(SpanHelpers.Reg16))))
			{
				*Unsafe.As<byte, SpanHelpers.Reg16>(Unsafe.Add<byte>(ref b, intPtr)) = default(SpanHelpers.Reg16);
				intPtr += (IntPtr)sizeof(SpanHelpers.Reg16);
			}
			if (intPtr.LessThanEqual(byteLength - (UIntPtr)((IntPtr)8)))
			{
				*Unsafe.As<byte, long>(Unsafe.Add<byte>(ref b, intPtr)) = 0L;
				intPtr += (IntPtr)8;
			}
			if (sizeof(IntPtr) == 4 && intPtr.LessThanEqual(byteLength - (UIntPtr)((IntPtr)4)))
			{
				*Unsafe.As<byte, int>(Unsafe.Add<byte>(ref b, intPtr)) = 0;
			}
		}

		// Token: 0x0600195C RID: 6492 RVA: 0x00052608 File Offset: 0x00050808
		public unsafe static void ClearPointerSizedWithReferences(ref IntPtr ip, [NativeInteger] UIntPtr pointerSizeLength)
		{
			IntPtr intPtr = (IntPtr)0;
			IntPtr intPtr2;
			while ((intPtr2 = intPtr + (IntPtr)8).LessThanEqual(pointerSizeLength))
			{
				*Unsafe.Add<IntPtr>(ref ip, intPtr + (IntPtr)0) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + (IntPtr)1) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + (IntPtr)2) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + (IntPtr)3) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + (IntPtr)4) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + (IntPtr)5) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + (IntPtr)6) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + (IntPtr)7) = 0;
				intPtr = intPtr2;
			}
			if ((intPtr2 = intPtr + (IntPtr)4).LessThanEqual(pointerSizeLength))
			{
				*Unsafe.Add<IntPtr>(ref ip, intPtr + (IntPtr)0) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + (IntPtr)1) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + (IntPtr)2) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + (IntPtr)3) = 0;
				intPtr = intPtr2;
			}
			if ((intPtr2 = intPtr + (IntPtr)2).LessThanEqual(pointerSizeLength))
			{
				*Unsafe.Add<IntPtr>(ref ip, intPtr + (IntPtr)0) = 0;
				*Unsafe.Add<IntPtr>(ref ip, intPtr + (IntPtr)1) = 0;
				intPtr = intPtr2;
			}
			if ((intPtr + (IntPtr)1).LessThanEqual(pointerSizeLength))
			{
				*Unsafe.Add<IntPtr>(ref ip, intPtr) = 0;
			}
		}

		// Token: 0x0600195D RID: 6493 RVA: 0x00052749 File Offset: 0x00050949
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool LessThanEqual(this IntPtr index, UIntPtr length)
		{
			if (sizeof(UIntPtr) != 4)
			{
				return (long)index <= (long)(ulong)length;
			}
			return (int)index <= (int)(uint)length;
		}

		// Token: 0x0600195E RID: 6494 RVA: 0x00052778 File Offset: 0x00050978
		public static int IndexOf<[Nullable(0)] T>(ref T searchSpace, int searchSpaceLength, ref T value, int valueLength) where T : IEquatable<T>
		{
			if (valueLength == 0)
			{
				return 0;
			}
			T t = value;
			ref T ptr = ref Unsafe.Add<T>(ref value, 1);
			int num = valueLength - 1;
			int num2 = 0;
			for (;;)
			{
				int num3 = searchSpaceLength - num2 - num;
				if (num3 <= 0)
				{
					return -1;
				}
				int num4 = SpanHelpers.IndexOf<T>(Unsafe.Add<T>(ref searchSpace, num2), t, num3);
				if (num4 == -1)
				{
					return -1;
				}
				num2 += num4;
				if (SpanHelpers.SequenceEqual<T>(Unsafe.Add<T>(ref searchSpace, num2 + 1), ref ptr, num))
				{
					break;
				}
				num2++;
			}
			return num2;
		}

		// Token: 0x0600195F RID: 6495 RVA: 0x000527E4 File Offset: 0x000509E4
		public unsafe static int IndexOf<[Nullable(0)] T>(ref T searchSpace, T value, int length) where T : IEquatable<T>
		{
			UIntPtr uintPtr = (UIntPtr)((IntPtr)0);
			while (length >= 8)
			{
				length -= 8;
				ref T ptr = ref value;
				if (default(T) == null)
				{
					T t = value;
					ptr = ref t;
				}
				if (ptr.Equals(*Unsafe.Add<T>(ref searchSpace, uintPtr)))
				{
					IL_0312:
					return (int)uintPtr;
				}
				ref T ptr2 = ref value;
				if (default(T) == null)
				{
					T t = value;
					ptr2 = ref t;
				}
				if (ptr2.Equals(*Unsafe.Add<T>(ref searchSpace, uintPtr + (UIntPtr)((IntPtr)1))))
				{
					IL_0315:
					return (int)(uintPtr + (UIntPtr)((IntPtr)1));
				}
				ref T ptr3 = ref value;
				if (default(T) == null)
				{
					T t = value;
					ptr3 = ref t;
				}
				if (ptr3.Equals(*Unsafe.Add<T>(ref searchSpace, uintPtr + (UIntPtr)((IntPtr)2))))
				{
					IL_031B:
					return (int)(uintPtr + (UIntPtr)((IntPtr)2));
				}
				ref T ptr4 = ref value;
				if (default(T) == null)
				{
					T t = value;
					ptr4 = ref t;
				}
				if (ptr4.Equals(*Unsafe.Add<T>(ref searchSpace, uintPtr + (UIntPtr)((IntPtr)3))))
				{
					IL_0321:
					return (int)(uintPtr + (UIntPtr)((IntPtr)3));
				}
				ref T ptr5 = ref value;
				if (default(T) == null)
				{
					T t = value;
					ptr5 = ref t;
				}
				if (ptr5.Equals(*Unsafe.Add<T>(ref searchSpace, uintPtr + (UIntPtr)((IntPtr)4))))
				{
					return (int)(uintPtr + (UIntPtr)((IntPtr)4));
				}
				ref T ptr6 = ref value;
				if (default(T) == null)
				{
					T t = value;
					ptr6 = ref t;
				}
				if (ptr6.Equals(*Unsafe.Add<T>(ref searchSpace, uintPtr + (UIntPtr)((IntPtr)5))))
				{
					return (int)(uintPtr + (UIntPtr)((IntPtr)5));
				}
				ref T ptr7 = ref value;
				if (default(T) == null)
				{
					T t = value;
					ptr7 = ref t;
				}
				if (ptr7.Equals(*Unsafe.Add<T>(ref searchSpace, uintPtr + (UIntPtr)((IntPtr)6))))
				{
					return (int)(uintPtr + (UIntPtr)((IntPtr)6));
				}
				ref T ptr8 = ref value;
				if (default(T) == null)
				{
					T t = value;
					ptr8 = ref t;
				}
				if (ptr8.Equals(*Unsafe.Add<T>(ref searchSpace, uintPtr + (UIntPtr)((IntPtr)7))))
				{
					return (int)(uintPtr + (UIntPtr)((IntPtr)7));
				}
				uintPtr += (UIntPtr)((IntPtr)8);
			}
			if (length >= 4)
			{
				length -= 4;
				ref T ptr9 = ref value;
				if (default(T) == null)
				{
					T t = value;
					ptr9 = ref t;
				}
				if (ptr9.Equals(*Unsafe.Add<T>(ref searchSpace, uintPtr)))
				{
					goto IL_0312;
				}
				ref T ptr10 = ref value;
				if (default(T) == null)
				{
					T t = value;
					ptr10 = ref t;
				}
				if (ptr10.Equals(*Unsafe.Add<T>(ref searchSpace, uintPtr + (UIntPtr)((IntPtr)1))))
				{
					goto IL_0315;
				}
				ref T ptr11 = ref value;
				if (default(T) == null)
				{
					T t = value;
					ptr11 = ref t;
				}
				if (ptr11.Equals(*Unsafe.Add<T>(ref searchSpace, uintPtr + (UIntPtr)((IntPtr)2))))
				{
					goto IL_031B;
				}
				ref T ptr12 = ref value;
				if (default(T) == null)
				{
					T t = value;
					ptr12 = ref t;
				}
				if (ptr12.Equals(*Unsafe.Add<T>(ref searchSpace, uintPtr + (UIntPtr)((IntPtr)3))))
				{
					goto IL_0321;
				}
				uintPtr += (UIntPtr)((IntPtr)4);
			}
			while (length > 0)
			{
				ref T ptr13 = ref value;
				if (default(T) == null)
				{
					T t = value;
					ptr13 = ref t;
				}
				if (ptr13.Equals(*Unsafe.Add<T>(ref searchSpace, uintPtr)))
				{
					goto IL_0312;
				}
				uintPtr += (UIntPtr)((IntPtr)1);
				length--;
			}
			return -1;
		}

		// Token: 0x06001960 RID: 6496 RVA: 0x00052B30 File Offset: 0x00050D30
		public unsafe static int IndexOfAny<[Nullable(0)] T>(ref T searchSpace, T value0, T value1, int length) where T : IEquatable<T>
		{
			int i = 0;
			while (length - i >= 8)
			{
				T t = *Unsafe.Add<T>(ref searchSpace, i);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return i;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 1);
				if (value0.Equals(t) || value1.Equals(t))
				{
					IL_02CB:
					return i + 1;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 2);
				if (value0.Equals(t) || value1.Equals(t))
				{
					IL_02CF:
					return i + 2;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 3);
				if (value0.Equals(t) || value1.Equals(t))
				{
					IL_02D3:
					return i + 3;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 4);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return i + 4;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 5);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return i + 5;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 6);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return i + 6;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 7);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return i + 7;
				}
				i += 8;
			}
			if (length - i >= 4)
			{
				T t = *Unsafe.Add<T>(ref searchSpace, i);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return i;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 1);
				if (value0.Equals(t) || value1.Equals(t))
				{
					goto IL_02CB;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 2);
				if (value0.Equals(t) || value1.Equals(t))
				{
					goto IL_02CF;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 3);
				if (value0.Equals(t) || value1.Equals(t))
				{
					goto IL_02D3;
				}
				i += 4;
			}
			while (i < length)
			{
				T t = *Unsafe.Add<T>(ref searchSpace, i);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return i;
				}
				i++;
			}
			return -1;
		}

		// Token: 0x06001961 RID: 6497 RVA: 0x00052E24 File Offset: 0x00051024
		public unsafe static int IndexOfAny<[Nullable(0)] T>(ref T searchSpace, T value0, T value1, T value2, int length) where T : IEquatable<T>
		{
			int i = 0;
			while (length - i >= 8)
			{
				T t = *Unsafe.Add<T>(ref searchSpace, i);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return i;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 1);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					IL_03C2:
					return i + 1;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 2);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					IL_03C6:
					return i + 2;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 3);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					IL_03CA:
					return i + 3;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 4);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return i + 4;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 5);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return i + 5;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 6);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return i + 6;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 7);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return i + 7;
				}
				i += 8;
			}
			if (length - i >= 4)
			{
				T t = *Unsafe.Add<T>(ref searchSpace, i);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return i;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 1);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					goto IL_03C2;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 2);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					goto IL_03C6;
				}
				t = *Unsafe.Add<T>(ref searchSpace, i + 3);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					goto IL_03CA;
				}
				i += 4;
			}
			while (i < length)
			{
				T t = *Unsafe.Add<T>(ref searchSpace, i);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return i;
				}
				i++;
			}
			return -1;
		}

		// Token: 0x06001962 RID: 6498 RVA: 0x00053210 File Offset: 0x00051410
		public unsafe static int IndexOfAny<[Nullable(0)] T>(ref T searchSpace, int searchSpaceLength, ref T value, int valueLength) where T : IEquatable<T>
		{
			if (valueLength == 0)
			{
				return 0;
			}
			int num = -1;
			for (int i = 0; i < valueLength; i++)
			{
				int num2 = SpanHelpers.IndexOf<T>(ref searchSpace, *Unsafe.Add<T>(ref value, i), searchSpaceLength);
				if (num2 < num)
				{
					num = num2;
					searchSpaceLength = num2;
					if (num == 0)
					{
						break;
					}
				}
			}
			return num;
		}

		// Token: 0x06001963 RID: 6499 RVA: 0x00053254 File Offset: 0x00051454
		public static int LastIndexOf<[Nullable(0)] T>(ref T searchSpace, int searchSpaceLength, ref T value, int valueLength) where T : IEquatable<T>
		{
			if (valueLength == 0)
			{
				return 0;
			}
			T t = value;
			ref T ptr = ref Unsafe.Add<T>(ref value, 1);
			int num = valueLength - 1;
			int num2 = 0;
			int num4;
			for (;;)
			{
				int num3 = searchSpaceLength - num2 - num;
				if (num3 <= 0)
				{
					return -1;
				}
				num4 = SpanHelpers.LastIndexOf<T>(ref searchSpace, t, num3);
				if (num4 == -1)
				{
					return -1;
				}
				if (SpanHelpers.SequenceEqual<T>(Unsafe.Add<T>(ref searchSpace, num4 + 1), ref ptr, num))
				{
					break;
				}
				num2 += num3 - num4;
			}
			return num4;
		}

		// Token: 0x06001964 RID: 6500 RVA: 0x000532B8 File Offset: 0x000514B8
		public unsafe static int LastIndexOf<[Nullable(0)] T>(ref T searchSpace, T value, int length) where T : IEquatable<T>
		{
			while (length >= 8)
			{
				length -= 8;
				ref T ptr = ref value;
				if (default(T) == null)
				{
					T t = value;
					ptr = ref t;
				}
				if (ptr.Equals(*Unsafe.Add<T>(ref searchSpace, length + 7)))
				{
					return length + 7;
				}
				ref T ptr2 = ref value;
				if (default(T) == null)
				{
					T t = value;
					ptr2 = ref t;
				}
				if (ptr2.Equals(*Unsafe.Add<T>(ref searchSpace, length + 6)))
				{
					return length + 6;
				}
				ref T ptr3 = ref value;
				if (default(T) == null)
				{
					T t = value;
					ptr3 = ref t;
				}
				if (ptr3.Equals(*Unsafe.Add<T>(ref searchSpace, length + 5)))
				{
					return length + 5;
				}
				ref T ptr4 = ref value;
				if (default(T) == null)
				{
					T t = value;
					ptr4 = ref t;
				}
				if (ptr4.Equals(*Unsafe.Add<T>(ref searchSpace, length + 4)))
				{
					return length + 4;
				}
				ref T ptr5 = ref value;
				if (default(T) == null)
				{
					T t = value;
					ptr5 = ref t;
				}
				if (ptr5.Equals(*Unsafe.Add<T>(ref searchSpace, length + 3)))
				{
					IL_02FD:
					return length + 3;
				}
				ref T ptr6 = ref value;
				if (default(T) == null)
				{
					T t = value;
					ptr6 = ref t;
				}
				if (ptr6.Equals(*Unsafe.Add<T>(ref searchSpace, length + 2)))
				{
					IL_02F9:
					return length + 2;
				}
				ref T ptr7 = ref value;
				if (default(T) == null)
				{
					T t = value;
					ptr7 = ref t;
				}
				if (ptr7.Equals(*Unsafe.Add<T>(ref searchSpace, length + 1)))
				{
					IL_02F5:
					return length + 1;
				}
				ref T ptr8 = ref value;
				if (default(T) == null)
				{
					T t = value;
					ptr8 = ref t;
				}
				if (ptr8.Equals(*Unsafe.Add<T>(ref searchSpace, length)))
				{
					return length;
				}
			}
			if (length >= 4)
			{
				length -= 4;
				ref T ptr9 = ref value;
				if (default(T) == null)
				{
					T t = value;
					ptr9 = ref t;
				}
				if (ptr9.Equals(*Unsafe.Add<T>(ref searchSpace, length + 3)))
				{
					goto IL_02FD;
				}
				ref T ptr10 = ref value;
				if (default(T) == null)
				{
					T t = value;
					ptr10 = ref t;
				}
				if (ptr10.Equals(*Unsafe.Add<T>(ref searchSpace, length + 2)))
				{
					goto IL_02F9;
				}
				ref T ptr11 = ref value;
				if (default(T) == null)
				{
					T t = value;
					ptr11 = ref t;
				}
				if (ptr11.Equals(*Unsafe.Add<T>(ref searchSpace, length + 1)))
				{
					goto IL_02F5;
				}
				ref T ptr12 = ref value;
				if (default(T) == null)
				{
					T t = value;
					ptr12 = ref t;
				}
				if (ptr12.Equals(*Unsafe.Add<T>(ref searchSpace, length)))
				{
					return length;
				}
			}
			while (length > 0)
			{
				length--;
				ref T ptr13 = ref value;
				if (default(T) == null)
				{
					T t = value;
					ptr13 = ref t;
				}
				if (ptr13.Equals(*Unsafe.Add<T>(ref searchSpace, length)))
				{
					return length;
				}
			}
			return -1;
		}

		// Token: 0x06001965 RID: 6501 RVA: 0x000535D8 File Offset: 0x000517D8
		public unsafe static int LastIndexOfAny<[Nullable(0)] T>(ref T searchSpace, T value0, T value1, int length) where T : IEquatable<T>
		{
			while (length >= 8)
			{
				length -= 8;
				T t = *Unsafe.Add<T>(ref searchSpace, length + 7);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return length + 7;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 6);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return length + 6;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 5);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return length + 5;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 4);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return length + 4;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 3);
				if (value0.Equals(t) || value1.Equals(t))
				{
					IL_02CD:
					return length + 3;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 2);
				if (value0.Equals(t) || value1.Equals(t))
				{
					IL_02C9:
					return length + 2;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 1);
				if (value0.Equals(t) || value1.Equals(t))
				{
					IL_02C5:
					return length + 1;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return length;
				}
			}
			if (length >= 4)
			{
				length -= 4;
				T t = *Unsafe.Add<T>(ref searchSpace, length + 3);
				if (value0.Equals(t) || value1.Equals(t))
				{
					goto IL_02CD;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 2);
				if (value0.Equals(t) || value1.Equals(t))
				{
					goto IL_02C9;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 1);
				if (value0.Equals(t) || value1.Equals(t))
				{
					goto IL_02C5;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length);
				if (value0.Equals(t))
				{
					return length;
				}
				if (value1.Equals(t))
				{
					return length;
				}
			}
			while (length > 0)
			{
				length--;
				T t = *Unsafe.Add<T>(ref searchSpace, length);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return length;
				}
			}
			return -1;
		}

		// Token: 0x06001966 RID: 6502 RVA: 0x000538C8 File Offset: 0x00051AC8
		public unsafe static int LastIndexOfAny<[Nullable(0)] T>(ref T searchSpace, T value0, T value1, T value2, int length) where T : IEquatable<T>
		{
			while (length >= 8)
			{
				length -= 8;
				T t = *Unsafe.Add<T>(ref searchSpace, length + 7);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return length + 7;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 6);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return length + 6;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 5);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return length + 5;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 4);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return length + 4;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 3);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					IL_03DA:
					return length + 3;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 2);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					IL_03D5:
					return length + 2;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 1);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					IL_03D0:
					return length + 1;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return length;
				}
			}
			if (length >= 4)
			{
				length -= 4;
				T t = *Unsafe.Add<T>(ref searchSpace, length + 3);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					goto IL_03DA;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 2);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					goto IL_03D5;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length + 1);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					goto IL_03D0;
				}
				t = *Unsafe.Add<T>(ref searchSpace, length);
				if (value0.Equals(t) || value1.Equals(t))
				{
					return length;
				}
				if (value2.Equals(t))
				{
					return length;
				}
			}
			while (length > 0)
			{
				length--;
				T t = *Unsafe.Add<T>(ref searchSpace, length);
				if (value0.Equals(t) || value1.Equals(t) || value2.Equals(t))
				{
					return length;
				}
			}
			return -1;
		}

		// Token: 0x06001967 RID: 6503 RVA: 0x00053CC8 File Offset: 0x00051EC8
		public unsafe static int LastIndexOfAny<[Nullable(0)] T>(ref T searchSpace, int searchSpaceLength, ref T value, int valueLength) where T : IEquatable<T>
		{
			if (valueLength == 0)
			{
				return 0;
			}
			int num = -1;
			for (int i = 0; i < valueLength; i++)
			{
				int num2 = SpanHelpers.LastIndexOf<T>(ref searchSpace, *Unsafe.Add<T>(ref value, i), searchSpaceLength);
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x06001968 RID: 6504 RVA: 0x00053D04 File Offset: 0x00051F04
		public unsafe static bool SequenceEqual<[Nullable(0)] T>(ref T first, ref T second, int length) where T : IEquatable<T>
		{
			if (!Unsafe.AreSame<T>(ref first, ref second))
			{
				UIntPtr uintPtr = (UIntPtr)((IntPtr)0);
				while (length >= 8)
				{
					length -= 8;
					ref T ptr2;
					ref T ptr = (ptr2 = Unsafe.Add<T>(ref first, uintPtr));
					if (default(T) == null)
					{
						T t = ptr;
						ptr2 = ref t;
					}
					if (ptr2.Equals(*Unsafe.Add<T>(ref second, uintPtr)))
					{
						ref T ptr4;
						ref T ptr3 = (ptr4 = Unsafe.Add<T>(ref first, uintPtr + (UIntPtr)((IntPtr)1)));
						if (default(T) == null)
						{
							T t = ptr3;
							ptr4 = ref t;
						}
						if (ptr4.Equals(*Unsafe.Add<T>(ref second, uintPtr + (UIntPtr)((IntPtr)1))))
						{
							ref T ptr6;
							ref T ptr5 = (ptr6 = Unsafe.Add<T>(ref first, uintPtr + (UIntPtr)((IntPtr)2)));
							if (default(T) == null)
							{
								T t = ptr5;
								ptr6 = ref t;
							}
							if (ptr6.Equals(*Unsafe.Add<T>(ref second, uintPtr + (UIntPtr)((IntPtr)2))))
							{
								ref T ptr8;
								ref T ptr7 = (ptr8 = Unsafe.Add<T>(ref first, uintPtr + (UIntPtr)((IntPtr)3)));
								if (default(T) == null)
								{
									T t = ptr7;
									ptr8 = ref t;
								}
								if (ptr8.Equals(*Unsafe.Add<T>(ref second, uintPtr + (UIntPtr)((IntPtr)3))))
								{
									ref T ptr10;
									ref T ptr9 = (ptr10 = Unsafe.Add<T>(ref first, uintPtr + (UIntPtr)((IntPtr)4)));
									if (default(T) == null)
									{
										T t = ptr9;
										ptr10 = ref t;
									}
									if (ptr10.Equals(*Unsafe.Add<T>(ref second, uintPtr + (UIntPtr)((IntPtr)4))))
									{
										ref T ptr12;
										ref T ptr11 = (ptr12 = Unsafe.Add<T>(ref first, uintPtr + (UIntPtr)((IntPtr)5)));
										if (default(T) == null)
										{
											T t = ptr11;
											ptr12 = ref t;
										}
										if (ptr12.Equals(*Unsafe.Add<T>(ref second, uintPtr + (UIntPtr)((IntPtr)5))))
										{
											ref T ptr14;
											ref T ptr13 = (ptr14 = Unsafe.Add<T>(ref first, uintPtr + (UIntPtr)((IntPtr)6)));
											if (default(T) == null)
											{
												T t = ptr13;
												ptr14 = ref t;
											}
											if (ptr14.Equals(*Unsafe.Add<T>(ref second, uintPtr + (UIntPtr)((IntPtr)6))))
											{
												ref T ptr16;
												ref T ptr15 = (ptr16 = Unsafe.Add<T>(ref first, uintPtr + (UIntPtr)((IntPtr)7)));
												if (default(T) == null)
												{
													T t = ptr15;
													ptr16 = ref t;
												}
												if (ptr16.Equals(*Unsafe.Add<T>(ref second, uintPtr + (UIntPtr)((IntPtr)7))))
												{
													uintPtr += (UIntPtr)((IntPtr)8);
													continue;
												}
											}
										}
									}
								}
							}
						}
					}
					return false;
				}
				if (length >= 4)
				{
					length -= 4;
					ref T ptr18;
					ref T ptr17 = (ptr18 = Unsafe.Add<T>(ref first, uintPtr));
					if (default(T) == null)
					{
						T t = ptr17;
						ptr18 = ref t;
					}
					if (!ptr18.Equals(*Unsafe.Add<T>(ref second, uintPtr)))
					{
						return false;
					}
					ref T ptr20;
					ref T ptr19 = (ptr20 = Unsafe.Add<T>(ref first, uintPtr + (UIntPtr)((IntPtr)1)));
					if (default(T) == null)
					{
						T t = ptr19;
						ptr20 = ref t;
					}
					if (!ptr20.Equals(*Unsafe.Add<T>(ref second, uintPtr + (UIntPtr)((IntPtr)1))))
					{
						return false;
					}
					ref T ptr22;
					ref T ptr21 = (ptr22 = Unsafe.Add<T>(ref first, uintPtr + (UIntPtr)((IntPtr)2)));
					if (default(T) == null)
					{
						T t = ptr21;
						ptr22 = ref t;
					}
					if (!ptr22.Equals(*Unsafe.Add<T>(ref second, uintPtr + (UIntPtr)((IntPtr)2))))
					{
						return false;
					}
					ref T ptr24;
					ref T ptr23 = (ptr24 = Unsafe.Add<T>(ref first, uintPtr + (UIntPtr)((IntPtr)3)));
					if (default(T) == null)
					{
						T t = ptr23;
						ptr24 = ref t;
					}
					if (!ptr24.Equals(*Unsafe.Add<T>(ref second, uintPtr + (UIntPtr)((IntPtr)3))))
					{
						return false;
					}
					uintPtr += (UIntPtr)((IntPtr)4);
				}
				while (length > 0)
				{
					ref T ptr26;
					ref T ptr25 = (ptr26 = Unsafe.Add<T>(ref first, uintPtr));
					if (default(T) == null)
					{
						T t = ptr25;
						ptr26 = ref t;
					}
					if (!ptr26.Equals(*Unsafe.Add<T>(ref second, uintPtr)))
					{
						return false;
					}
					uintPtr += (UIntPtr)((IntPtr)1);
					length--;
				}
			}
			return true;
		}

		// Token: 0x06001969 RID: 6505 RVA: 0x00054090 File Offset: 0x00052290
		public unsafe static int SequenceCompareTo<[Nullable(0)] T>(ref T first, int firstLength, ref T second, int secondLength) where T : IComparable<T>
		{
			int num = firstLength;
			if (num > secondLength)
			{
				num = secondLength;
			}
			for (int i = 0; i < num; i++)
			{
				ref T ptr2;
				ref T ptr = (ptr2 = Unsafe.Add<T>(ref first, i));
				if (default(T) == null)
				{
					T t = ptr;
					ptr2 = ref t;
				}
				int num2 = ptr2.CompareTo(*Unsafe.Add<T>(ref second, i));
				if (num2 != 0)
				{
					return num2;
				}
			}
			return firstLength.CompareTo(secondLength);
		}

		// Token: 0x02000478 RID: 1144
		[Nullable(0)]
		internal struct ComparerComparable<[Nullable(2)] T, [Nullable(0)] TComparer> : IComparable<T> where TComparer : IComparer<T>
		{
			// Token: 0x0600196A RID: 6506 RVA: 0x000540F5 File Offset: 0x000522F5
			public ComparerComparable(T value, TComparer comparer)
			{
				this._value = value;
				this._comparer = comparer;
			}

			// Token: 0x0600196B RID: 6507 RVA: 0x00054108 File Offset: 0x00052308
			[NullableContext(2)]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public int CompareTo(T other)
			{
				TComparer comparer = this._comparer;
				return comparer.Compare(this._value, other);
			}

			// Token: 0x040010A9 RID: 4265
			private readonly T _value;

			// Token: 0x040010AA RID: 4266
			private readonly TComparer _comparer;
		}

		// Token: 0x02000479 RID: 1145
		[NullableContext(0)]
		private struct Reg64
		{
		}

		// Token: 0x0200047A RID: 1146
		[NullableContext(0)]
		private struct Reg32
		{
		}

		// Token: 0x0200047B RID: 1147
		[NullableContext(0)]
		private struct Reg16
		{
		}

		// Token: 0x0200047C RID: 1148
		[NullableContext(0)]
		public static class PerTypeValues<[Nullable(2)] T>
		{
			// Token: 0x0600196C RID: 6508 RVA: 0x00054130 File Offset: 0x00052330
			private static IntPtr MeasureArrayAdjustment()
			{
				T[] array = new T[1];
				return Unsafe.ByteOffset<T>(ILHelpers.ObjectAsRef<T>(array), ref array[0]);
			}

			// Token: 0x040010AB RID: 4267
			public static readonly bool IsReferenceOrContainsReferences = SpanHelpers.IsReferenceOrContainsReferencesCore(typeof(T));

			// Token: 0x040010AC RID: 4268
			[Nullable(1)]
			public static readonly T[] EmptyArray = ArrayEx.Empty<T>();

			// Token: 0x040010AD RID: 4269
			public static readonly IntPtr ArrayAdjustment = SpanHelpers.PerTypeValues<T>.MeasureArrayAdjustment();
		}
	}
}
