using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace System
{
	// Token: 0x0200046C RID: 1132
	[NullableContext(1)]
	[Nullable(0)]
	internal struct HashCode
	{
		// Token: 0x06001860 RID: 6240 RVA: 0x0004D1E8 File Offset: 0x0004B3E8
		private unsafe static uint GenerateGlobalSeed()
		{
			Span<byte> span = new Span<byte>(stackalloc byte[(UIntPtr)4], 4);
			RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
			try
			{
				byte[] array = new byte[span.Length];
				randomNumberGenerator.GetBytes(array);
				array.AsSpan<byte>().CopyTo(span);
			}
			finally
			{
				RandomNumberGenerator randomNumberGenerator2 = randomNumberGenerator;
				if (randomNumberGenerator2 != null)
				{
					((IDisposable)randomNumberGenerator2).Dispose();
				}
			}
			return Unsafe.ReadUnaligned<uint>(span[0]);
		}

		// Token: 0x06001861 RID: 6241 RVA: 0x0004D258 File Offset: 0x0004B458
		public static int Combine<[Nullable(2)] T1>(T1 value1)
		{
			ref T1 ptr = ref value1;
			T1 t = default(T1);
			uint num;
			if (t == null)
			{
				t = value1;
				ptr = ref t;
				if (t == null)
				{
					num = 0U;
					goto IL_0031;
				}
			}
			num = (uint)ptr.GetHashCode();
			IL_0031:
			uint num2 = num;
			return (int)HashCode.MixFinal(HashCode.QueueRound(HashCode.MixEmptyState() + 4U, num2));
		}

		// Token: 0x06001862 RID: 6242 RVA: 0x0004D2AC File Offset: 0x0004B4AC
		public static int Combine<[Nullable(2)] T1, [Nullable(2)] T2>(T1 value1, T2 value2)
		{
			ref T1 ptr = ref value1;
			T1 t = default(T1);
			uint num;
			if (t == null)
			{
				t = value1;
				ptr = ref t;
				if (t == null)
				{
					num = 0U;
					goto IL_0031;
				}
			}
			num = (uint)ptr.GetHashCode();
			IL_0031:
			uint num2 = num;
			ref T2 ptr2 = ref value2;
			T2 t2 = default(T2);
			uint num3;
			if (t2 == null)
			{
				t2 = value2;
				ptr2 = ref t2;
				if (t2 == null)
				{
					num3 = 0U;
					goto IL_0063;
				}
			}
			num3 = (uint)ptr2.GetHashCode();
			IL_0063:
			uint num4 = num3;
			return (int)HashCode.MixFinal(HashCode.QueueRound(HashCode.QueueRound(HashCode.MixEmptyState() + 8U, num2), num4));
		}

		// Token: 0x06001863 RID: 6243 RVA: 0x0004D338 File Offset: 0x0004B538
		public static int Combine<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3>(T1 value1, T2 value2, T3 value3)
		{
			ref T1 ptr = ref value1;
			T1 t = default(T1);
			uint num;
			if (t == null)
			{
				t = value1;
				ptr = ref t;
				if (t == null)
				{
					num = 0U;
					goto IL_0031;
				}
			}
			num = (uint)ptr.GetHashCode();
			IL_0031:
			uint num2 = num;
			ref T2 ptr2 = ref value2;
			T2 t2 = default(T2);
			uint num3;
			if (t2 == null)
			{
				t2 = value2;
				ptr2 = ref t2;
				if (t2 == null)
				{
					num3 = 0U;
					goto IL_0066;
				}
			}
			num3 = (uint)ptr2.GetHashCode();
			IL_0066:
			uint num4 = num3;
			ref T3 ptr3 = ref value3;
			T3 t3 = default(T3);
			uint num5;
			if (t3 == null)
			{
				t3 = value3;
				ptr3 = ref t3;
				if (t3 == null)
				{
					num5 = 0U;
					goto IL_009B;
				}
			}
			num5 = (uint)ptr3.GetHashCode();
			IL_009B:
			uint num6 = num5;
			return (int)HashCode.MixFinal(HashCode.QueueRound(HashCode.QueueRound(HashCode.QueueRound(HashCode.MixEmptyState() + 12U, num2), num4), num6));
		}

		// Token: 0x06001864 RID: 6244 RVA: 0x0004D400 File Offset: 0x0004B600
		public static int Combine<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4>(T1 value1, T2 value2, T3 value3, T4 value4)
		{
			ref T1 ptr = ref value1;
			T1 t = default(T1);
			uint num;
			if (t == null)
			{
				t = value1;
				ptr = ref t;
				if (t == null)
				{
					num = 0U;
					goto IL_0034;
				}
			}
			num = (uint)ptr.GetHashCode();
			IL_0034:
			uint num2 = num;
			ref T2 ptr2 = ref value2;
			T2 t2 = default(T2);
			uint num3;
			if (t2 == null)
			{
				t2 = value2;
				ptr2 = ref t2;
				if (t2 == null)
				{
					num3 = 0U;
					goto IL_0069;
				}
			}
			num3 = (uint)ptr2.GetHashCode();
			IL_0069:
			uint num4 = num3;
			ref T3 ptr3 = ref value3;
			T3 t3 = default(T3);
			uint num5;
			if (t3 == null)
			{
				t3 = value3;
				ptr3 = ref t3;
				if (t3 == null)
				{
					num5 = 0U;
					goto IL_009E;
				}
			}
			num5 = (uint)ptr3.GetHashCode();
			IL_009E:
			uint num6 = num5;
			ref T4 ptr4 = ref value4;
			T4 t4 = default(T4);
			uint num7;
			if (t4 == null)
			{
				t4 = value4;
				ptr4 = ref t4;
				if (t4 == null)
				{
					num7 = 0U;
					goto IL_00D3;
				}
			}
			num7 = (uint)ptr4.GetHashCode();
			IL_00D3:
			uint num8 = num7;
			uint num9;
			uint num10;
			uint num11;
			uint num12;
			HashCode.Initialize(out num9, out num10, out num11, out num12);
			num9 = HashCode.Round(num9, num2);
			num10 = HashCode.Round(num10, num4);
			num11 = HashCode.Round(num11, num6);
			num12 = HashCode.Round(num12, num8);
			return (int)HashCode.MixFinal(HashCode.MixState(num9, num10, num11, num12) + 16U);
		}

		// Token: 0x06001865 RID: 6245 RVA: 0x0004D52C File Offset: 0x0004B72C
		public static int Combine<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
		{
			ref T1 ptr = ref value1;
			T1 t = default(T1);
			uint num;
			if (t == null)
			{
				t = value1;
				ptr = ref t;
				if (t == null)
				{
					num = 0U;
					goto IL_0034;
				}
			}
			num = (uint)ptr.GetHashCode();
			IL_0034:
			uint num2 = num;
			ref T2 ptr2 = ref value2;
			T2 t2 = default(T2);
			uint num3;
			if (t2 == null)
			{
				t2 = value2;
				ptr2 = ref t2;
				if (t2 == null)
				{
					num3 = 0U;
					goto IL_0069;
				}
			}
			num3 = (uint)ptr2.GetHashCode();
			IL_0069:
			uint num4 = num3;
			ref T3 ptr3 = ref value3;
			T3 t3 = default(T3);
			uint num5;
			if (t3 == null)
			{
				t3 = value3;
				ptr3 = ref t3;
				if (t3 == null)
				{
					num5 = 0U;
					goto IL_009E;
				}
			}
			num5 = (uint)ptr3.GetHashCode();
			IL_009E:
			uint num6 = num5;
			ref T4 ptr4 = ref value4;
			T4 t4 = default(T4);
			uint num7;
			if (t4 == null)
			{
				t4 = value4;
				ptr4 = ref t4;
				if (t4 == null)
				{
					num7 = 0U;
					goto IL_00D3;
				}
			}
			num7 = (uint)ptr4.GetHashCode();
			IL_00D3:
			uint num8 = num7;
			ref T5 ptr5 = ref value5;
			T5 t5 = default(T5);
			uint num9;
			if (t5 == null)
			{
				t5 = value5;
				ptr5 = ref t5;
				if (t5 == null)
				{
					num9 = 0U;
					goto IL_0108;
				}
			}
			num9 = (uint)ptr5.GetHashCode();
			IL_0108:
			uint num10 = num9;
			uint num11;
			uint num12;
			uint num13;
			uint num14;
			HashCode.Initialize(out num11, out num12, out num13, out num14);
			num11 = HashCode.Round(num11, num2);
			num12 = HashCode.Round(num12, num4);
			num13 = HashCode.Round(num13, num6);
			num14 = HashCode.Round(num14, num8);
			return (int)HashCode.MixFinal(HashCode.QueueRound(HashCode.MixState(num11, num12, num13, num14) + 20U, num10));
		}

		// Token: 0x06001866 RID: 6246 RVA: 0x0004D694 File Offset: 0x0004B894
		public static int Combine<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6)
		{
			ref T1 ptr = ref value1;
			T1 t = default(T1);
			uint num;
			if (t == null)
			{
				t = value1;
				ptr = ref t;
				if (t == null)
				{
					num = 0U;
					goto IL_0034;
				}
			}
			num = (uint)ptr.GetHashCode();
			IL_0034:
			uint num2 = num;
			ref T2 ptr2 = ref value2;
			T2 t2 = default(T2);
			uint num3;
			if (t2 == null)
			{
				t2 = value2;
				ptr2 = ref t2;
				if (t2 == null)
				{
					num3 = 0U;
					goto IL_0069;
				}
			}
			num3 = (uint)ptr2.GetHashCode();
			IL_0069:
			uint num4 = num3;
			ref T3 ptr3 = ref value3;
			T3 t3 = default(T3);
			uint num5;
			if (t3 == null)
			{
				t3 = value3;
				ptr3 = ref t3;
				if (t3 == null)
				{
					num5 = 0U;
					goto IL_009E;
				}
			}
			num5 = (uint)ptr3.GetHashCode();
			IL_009E:
			uint num6 = num5;
			ref T4 ptr4 = ref value4;
			T4 t4 = default(T4);
			uint num7;
			if (t4 == null)
			{
				t4 = value4;
				ptr4 = ref t4;
				if (t4 == null)
				{
					num7 = 0U;
					goto IL_00D3;
				}
			}
			num7 = (uint)ptr4.GetHashCode();
			IL_00D3:
			uint num8 = num7;
			ref T5 ptr5 = ref value5;
			T5 t5 = default(T5);
			uint num9;
			if (t5 == null)
			{
				t5 = value5;
				ptr5 = ref t5;
				if (t5 == null)
				{
					num9 = 0U;
					goto IL_0108;
				}
			}
			num9 = (uint)ptr5.GetHashCode();
			IL_0108:
			uint num10 = num9;
			ref T6 ptr6 = ref value6;
			T6 t6 = default(T6);
			uint num11;
			if (t6 == null)
			{
				t6 = value6;
				ptr6 = ref t6;
				if (t6 == null)
				{
					num11 = 0U;
					goto IL_013E;
				}
			}
			num11 = (uint)ptr6.GetHashCode();
			IL_013E:
			uint num12 = num11;
			uint num13;
			uint num14;
			uint num15;
			uint num16;
			HashCode.Initialize(out num13, out num14, out num15, out num16);
			num13 = HashCode.Round(num13, num2);
			num14 = HashCode.Round(num14, num4);
			num15 = HashCode.Round(num15, num6);
			num16 = HashCode.Round(num16, num8);
			return (int)HashCode.MixFinal(HashCode.QueueRound(HashCode.QueueRound(HashCode.MixState(num13, num14, num15, num16) + 24U, num10), num12));
		}

		// Token: 0x06001867 RID: 6247 RVA: 0x0004D83C File Offset: 0x0004BA3C
		public static int Combine<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7)
		{
			ref T1 ptr = ref value1;
			T1 t = default(T1);
			uint num;
			if (t == null)
			{
				t = value1;
				ptr = ref t;
				if (t == null)
				{
					num = 0U;
					goto IL_0034;
				}
			}
			num = (uint)ptr.GetHashCode();
			IL_0034:
			uint num2 = num;
			ref T2 ptr2 = ref value2;
			T2 t2 = default(T2);
			uint num3;
			if (t2 == null)
			{
				t2 = value2;
				ptr2 = ref t2;
				if (t2 == null)
				{
					num3 = 0U;
					goto IL_0069;
				}
			}
			num3 = (uint)ptr2.GetHashCode();
			IL_0069:
			uint num4 = num3;
			ref T3 ptr3 = ref value3;
			T3 t3 = default(T3);
			uint num5;
			if (t3 == null)
			{
				t3 = value3;
				ptr3 = ref t3;
				if (t3 == null)
				{
					num5 = 0U;
					goto IL_009E;
				}
			}
			num5 = (uint)ptr3.GetHashCode();
			IL_009E:
			uint num6 = num5;
			ref T4 ptr4 = ref value4;
			T4 t4 = default(T4);
			uint num7;
			if (t4 == null)
			{
				t4 = value4;
				ptr4 = ref t4;
				if (t4 == null)
				{
					num7 = 0U;
					goto IL_00D3;
				}
			}
			num7 = (uint)ptr4.GetHashCode();
			IL_00D3:
			uint num8 = num7;
			ref T5 ptr5 = ref value5;
			T5 t5 = default(T5);
			uint num9;
			if (t5 == null)
			{
				t5 = value5;
				ptr5 = ref t5;
				if (t5 == null)
				{
					num9 = 0U;
					goto IL_0108;
				}
			}
			num9 = (uint)ptr5.GetHashCode();
			IL_0108:
			uint num10 = num9;
			ref T6 ptr6 = ref value6;
			T6 t6 = default(T6);
			uint num11;
			if (t6 == null)
			{
				t6 = value6;
				ptr6 = ref t6;
				if (t6 == null)
				{
					num11 = 0U;
					goto IL_013E;
				}
			}
			num11 = (uint)ptr6.GetHashCode();
			IL_013E:
			uint num12 = num11;
			ref T7 ptr7 = ref value7;
			T7 t7 = default(T7);
			uint num13;
			if (t7 == null)
			{
				t7 = value7;
				ptr7 = ref t7;
				if (t7 == null)
				{
					num13 = 0U;
					goto IL_0174;
				}
			}
			num13 = (uint)ptr7.GetHashCode();
			IL_0174:
			uint num14 = num13;
			uint num15;
			uint num16;
			uint num17;
			uint num18;
			HashCode.Initialize(out num15, out num16, out num17, out num18);
			num15 = HashCode.Round(num15, num2);
			num16 = HashCode.Round(num16, num4);
			num17 = HashCode.Round(num17, num6);
			num18 = HashCode.Round(num18, num8);
			return (int)HashCode.MixFinal(HashCode.QueueRound(HashCode.QueueRound(HashCode.QueueRound(HashCode.MixState(num15, num16, num17, num18) + 28U, num10), num12), num14));
		}

		// Token: 0x06001868 RID: 6248 RVA: 0x0004DA20 File Offset: 0x0004BC20
		public static int Combine<[Nullable(2)] T1, [Nullable(2)] T2, [Nullable(2)] T3, [Nullable(2)] T4, [Nullable(2)] T5, [Nullable(2)] T6, [Nullable(2)] T7, [Nullable(2)] T8>(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, T7 value7, T8 value8)
		{
			ref T1 ptr = ref value1;
			T1 t = default(T1);
			uint num;
			if (t == null)
			{
				t = value1;
				ptr = ref t;
				if (t == null)
				{
					num = 0U;
					goto IL_0034;
				}
			}
			num = (uint)ptr.GetHashCode();
			IL_0034:
			uint num2 = num;
			ref T2 ptr2 = ref value2;
			T2 t2 = default(T2);
			uint num3;
			if (t2 == null)
			{
				t2 = value2;
				ptr2 = ref t2;
				if (t2 == null)
				{
					num3 = 0U;
					goto IL_0069;
				}
			}
			num3 = (uint)ptr2.GetHashCode();
			IL_0069:
			uint num4 = num3;
			ref T3 ptr3 = ref value3;
			T3 t3 = default(T3);
			uint num5;
			if (t3 == null)
			{
				t3 = value3;
				ptr3 = ref t3;
				if (t3 == null)
				{
					num5 = 0U;
					goto IL_009E;
				}
			}
			num5 = (uint)ptr3.GetHashCode();
			IL_009E:
			uint num6 = num5;
			ref T4 ptr4 = ref value4;
			T4 t4 = default(T4);
			uint num7;
			if (t4 == null)
			{
				t4 = value4;
				ptr4 = ref t4;
				if (t4 == null)
				{
					num7 = 0U;
					goto IL_00D3;
				}
			}
			num7 = (uint)ptr4.GetHashCode();
			IL_00D3:
			uint num8 = num7;
			ref T5 ptr5 = ref value5;
			T5 t5 = default(T5);
			uint num9;
			if (t5 == null)
			{
				t5 = value5;
				ptr5 = ref t5;
				if (t5 == null)
				{
					num9 = 0U;
					goto IL_0108;
				}
			}
			num9 = (uint)ptr5.GetHashCode();
			IL_0108:
			uint num10 = num9;
			ref T6 ptr6 = ref value6;
			T6 t6 = default(T6);
			uint num11;
			if (t6 == null)
			{
				t6 = value6;
				ptr6 = ref t6;
				if (t6 == null)
				{
					num11 = 0U;
					goto IL_013E;
				}
			}
			num11 = (uint)ptr6.GetHashCode();
			IL_013E:
			uint num12 = num11;
			ref T7 ptr7 = ref value7;
			T7 t7 = default(T7);
			uint num13;
			if (t7 == null)
			{
				t7 = value7;
				ptr7 = ref t7;
				if (t7 == null)
				{
					num13 = 0U;
					goto IL_0174;
				}
			}
			num13 = (uint)ptr7.GetHashCode();
			IL_0174:
			uint num14 = num13;
			ref T8 ptr8 = ref value8;
			T8 t8 = default(T8);
			uint num15;
			if (t8 == null)
			{
				t8 = value8;
				ptr8 = ref t8;
				if (t8 == null)
				{
					num15 = 0U;
					goto IL_01AA;
				}
			}
			num15 = (uint)ptr8.GetHashCode();
			IL_01AA:
			uint num16 = num15;
			uint num17;
			uint num18;
			uint num19;
			uint num20;
			HashCode.Initialize(out num17, out num18, out num19, out num20);
			num17 = HashCode.Round(num17, num2);
			num18 = HashCode.Round(num18, num4);
			num19 = HashCode.Round(num19, num6);
			num20 = HashCode.Round(num20, num8);
			num17 = HashCode.Round(num17, num10);
			num18 = HashCode.Round(num18, num12);
			num19 = HashCode.Round(num19, num14);
			num20 = HashCode.Round(num20, num16);
			return (int)HashCode.MixFinal(HashCode.MixState(num17, num18, num19, num20) + 32U);
		}

		// Token: 0x06001869 RID: 6249 RVA: 0x0004DC4F File Offset: 0x0004BE4F
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Initialize(out uint v1, out uint v2, out uint v3, out uint v4)
		{
			v1 = HashCode.s_seed + 2654435761U + 2246822519U;
			v2 = HashCode.s_seed + 2246822519U;
			v3 = HashCode.s_seed;
			v4 = HashCode.s_seed - 2654435761U;
		}

		// Token: 0x0600186A RID: 6250 RVA: 0x0004DC85 File Offset: 0x0004BE85
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static uint Round(uint hash, uint input)
		{
			return BitOperations.RotateLeft(hash + input * 2246822519U, 13) * 2654435761U;
		}

		// Token: 0x0600186B RID: 6251 RVA: 0x0004DC9D File Offset: 0x0004BE9D
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static uint QueueRound(uint hash, uint queuedValue)
		{
			return BitOperations.RotateLeft(hash + queuedValue * 3266489917U, 17) * 668265263U;
		}

		// Token: 0x0600186C RID: 6252 RVA: 0x0004DCB5 File Offset: 0x0004BEB5
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static uint MixState(uint v1, uint v2, uint v3, uint v4)
		{
			return BitOperations.RotateLeft(v1, 1) + BitOperations.RotateLeft(v2, 7) + BitOperations.RotateLeft(v3, 12) + BitOperations.RotateLeft(v4, 18);
		}

		// Token: 0x0600186D RID: 6253 RVA: 0x0004DCD8 File Offset: 0x0004BED8
		private static uint MixEmptyState()
		{
			return HashCode.s_seed + 374761393U;
		}

		// Token: 0x0600186E RID: 6254 RVA: 0x0004DCE5 File Offset: 0x0004BEE5
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static uint MixFinal(uint hash)
		{
			hash ^= hash >> 15;
			hash *= 2246822519U;
			hash ^= hash >> 13;
			hash *= 3266489917U;
			hash ^= hash >> 16;
			return hash;
		}

		// Token: 0x0600186F RID: 6255 RVA: 0x0004DD14 File Offset: 0x0004BF14
		public void Add<[Nullable(2)] T>(T value)
		{
			ref T ptr = ref value;
			T t = default(T);
			int num;
			if (t == null)
			{
				t = value;
				ptr = ref t;
				if (t == null)
				{
					num = 0;
					goto IL_0032;
				}
			}
			num = ptr.GetHashCode();
			IL_0032:
			this.Add(num);
		}

		// Token: 0x06001870 RID: 6256 RVA: 0x0004DD58 File Offset: 0x0004BF58
		public void Add<[Nullable(2)] T>(T value, [Nullable(new byte[] { 2, 1 })] IEqualityComparer<T> comparer)
		{
			this.Add((value == null) ? 0 : ((comparer != null) ? comparer.GetHashCode(value) : value.GetHashCode()));
		}

		// Token: 0x06001871 RID: 6257 RVA: 0x0004DD84 File Offset: 0x0004BF84
		[NullableContext(0)]
		public void AddBytes(ReadOnlySpan<byte> value)
		{
			ref byte ptr = ref MemoryMarshal.GetReference<byte>(value);
			ref byte ptr2 = ref Unsafe.Add<byte>(ref ptr, value.Length);
			while (Unsafe.ByteOffset<byte>(ref ptr, ref ptr2) >= (IntPtr)4)
			{
				this.Add(Unsafe.ReadUnaligned<int>(ref ptr));
				ptr = Unsafe.Add<byte>(ref ptr, 4);
			}
			while (Unsafe.IsAddressLessThan<byte>(ref ptr, ref ptr2))
			{
				this.Add((int)ptr);
				ptr = Unsafe.Add<byte>(ref ptr, 1);
			}
		}

		// Token: 0x06001872 RID: 6258 RVA: 0x0004DDE4 File Offset: 0x0004BFE4
		private void Add(int value)
		{
			uint length = this._length;
			this._length = length + 1U;
			uint num = length;
			uint num2 = num % 4U;
			if (num2 == 0U)
			{
				this._queue1 = (uint)value;
				return;
			}
			if (num2 == 1U)
			{
				this._queue2 = (uint)value;
				return;
			}
			if (num2 == 2U)
			{
				this._queue3 = (uint)value;
				return;
			}
			if (num == 3U)
			{
				HashCode.Initialize(out this._v1, out this._v2, out this._v3, out this._v4);
			}
			this._v1 = HashCode.Round(this._v1, this._queue1);
			this._v2 = HashCode.Round(this._v2, this._queue2);
			this._v3 = HashCode.Round(this._v3, this._queue3);
			this._v4 = HashCode.Round(this._v4, (uint)value);
		}

		// Token: 0x06001873 RID: 6259 RVA: 0x0004DEA4 File Offset: 0x0004C0A4
		public int ToHashCode()
		{
			uint length = this._length;
			uint num = length % 4U;
			uint num2 = ((length < 4U) ? HashCode.MixEmptyState() : HashCode.MixState(this._v1, this._v2, this._v3, this._v4));
			num2 += length * 4U;
			if (num > 0U)
			{
				num2 = HashCode.QueueRound(num2, this._queue1);
				if (num > 1U)
				{
					num2 = HashCode.QueueRound(num2, this._queue2);
					if (num > 2U)
					{
						num2 = HashCode.QueueRound(num2, this._queue3);
					}
				}
			}
			return (int)HashCode.MixFinal(num2);
		}

		// Token: 0x06001874 RID: 6260 RVA: 0x0004DF26 File Offset: 0x0004C126
		[Obsolete("HashCode is a mutable struct and should not be compared with other HashCodes. Use ToHashCode to retrieve the computed hash code.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int GetHashCode()
		{
			throw new NotSupportedException("GetHashCode on HashCode is not supported");
		}

		// Token: 0x06001875 RID: 6261 RVA: 0x0004DF32 File Offset: 0x0004C132
		[NullableContext(2)]
		[Obsolete("HashCode is a mutable struct and should not be compared with other HashCodes.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Equals(object obj)
		{
			throw new NotSupportedException("Equals on HashCode is not supported");
		}

		// Token: 0x04001087 RID: 4231
		private static readonly uint s_seed = HashCode.GenerateGlobalSeed();

		// Token: 0x04001088 RID: 4232
		private const uint Prime1 = 2654435761U;

		// Token: 0x04001089 RID: 4233
		private const uint Prime2 = 2246822519U;

		// Token: 0x0400108A RID: 4234
		private const uint Prime3 = 3266489917U;

		// Token: 0x0400108B RID: 4235
		private const uint Prime4 = 668265263U;

		// Token: 0x0400108C RID: 4236
		private const uint Prime5 = 374761393U;

		// Token: 0x0400108D RID: 4237
		private uint _v1;

		// Token: 0x0400108E RID: 4238
		private uint _v2;

		// Token: 0x0400108F RID: 4239
		private uint _v3;

		// Token: 0x04001090 RID: 4240
		private uint _v4;

		// Token: 0x04001091 RID: 4241
		private uint _queue1;

		// Token: 0x04001092 RID: 4242
		private uint _queue2;

		// Token: 0x04001093 RID: 4243
		private uint _queue3;

		// Token: 0x04001094 RID: 4244
		private uint _length;
	}
}
