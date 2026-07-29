using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using MonoMod.Utils;

namespace MonoMod.Core.Utils
{
	// Token: 0x020004EA RID: 1258
	internal sealed class BytePatternCollection : IEnumerable<BytePattern>, IEnumerable
	{
		// Token: 0x1700060F RID: 1551
		// (get) Token: 0x06001C1D RID: 7197 RVA: 0x00059FDF File Offset: 0x000581DF
		public int MinLength { get; }

		// Token: 0x17000610 RID: 1552
		// (get) Token: 0x06001C1E RID: 7198 RVA: 0x00059FE7 File Offset: 0x000581E7
		public int MaxMinLength { get; }

		// Token: 0x17000611 RID: 1553
		// (get) Token: 0x06001C1F RID: 7199 RVA: 0x00059FEF File Offset: 0x000581EF
		public int MaxAddressLength { get; }

		// Token: 0x06001C20 RID: 7200 RVA: 0x00059FF8 File Offset: 0x000581F8
		public BytePatternCollection([Nullable(new byte[] { 0, 2 })] ReadOnlyMemory<BytePattern> patterns)
		{
			int num;
			int num2;
			int num3;
			ValueTuple<BytePatternCollection.HomogenousPatternCollection[], BytePattern[]> valueTuple = BytePatternCollection.ComputeLut(patterns, out num, out num2, out num3);
			this.patternCollections = valueTuple.Item1;
			this.emptyPatterns = valueTuple.Item2;
			this.MinLength = num;
			this.MaxMinLength = num2;
			this.MaxAddressLength = num3;
			Helpers.Assert(this.MinLength > 0, null, "MinLength > 0");
		}

		// Token: 0x06001C21 RID: 7201 RVA: 0x0005A059 File Offset: 0x00058259
		public BytePatternCollection([Nullable(new byte[] { 1, 2 })] params BytePattern[] patterns)
			: this(patterns.AsMemory<BytePattern>())
		{
		}

		// Token: 0x06001C22 RID: 7202 RVA: 0x0005A06C File Offset: 0x0005826C
		[NullableContext(1)]
		public IEnumerator<BytePattern> GetEnumerator()
		{
			BytePatternCollection.<GetEnumerator>d__13 <GetEnumerator>d__ = new BytePatternCollection.<GetEnumerator>d__13(0);
			<GetEnumerator>d__.<>4__this = this;
			return <GetEnumerator>d__;
		}

		// Token: 0x06001C23 RID: 7203 RVA: 0x0005A07B File Offset: 0x0005827B
		[NullableContext(1)]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x06001C24 RID: 7204 RVA: 0x0005A084 File Offset: 0x00058284
		[return: Nullable(new byte[] { 0, 1, 2, 1 })]
		private unsafe static ValueTuple<BytePatternCollection.HomogenousPatternCollection[], BytePattern[]> ComputeLut([Nullable(new byte[] { 0, 2 })] ReadOnlyMemory<BytePattern> patterns, out int minLength, out int maxMinLength, out int maxAddrLength)
		{
			if (patterns.Length == 0)
			{
				minLength = 0;
				maxMinLength = 0;
				maxAddrLength = 0;
				return new ValueTuple<BytePatternCollection.HomogenousPatternCollection[], BytePattern[]>(ArrayEx.Empty<BytePatternCollection.HomogenousPatternCollection>(), null);
			}
			Span<int> span = new Span<int>(stackalloc byte[(UIntPtr)1024], 256);
			minLength = int.MaxValue;
			maxMinLength = int.MinValue;
			maxAddrLength = 0;
			int[][] array = null;
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < patterns.Length; i++)
			{
				BytePattern bytePattern = *patterns.Span[i];
				if (bytePattern != null)
				{
					if (bytePattern.MinLength < minLength)
					{
						minLength = bytePattern.MinLength;
					}
					if (bytePattern.MinLength > maxMinLength)
					{
						maxMinLength = bytePattern.MinLength;
					}
					if (bytePattern.AddressBytes > maxAddrLength)
					{
						maxAddrLength = bytePattern.AddressBytes;
					}
					ValueTuple<ReadOnlyMemory<byte>, int> firstLiteralSegment = bytePattern.FirstLiteralSegment;
					ReadOnlyMemory<byte> item = firstLiteralSegment.Item1;
					int item2 = firstLiteralSegment.Item2;
					if (item.Length == 0)
					{
						num++;
					}
					else
					{
						num2 = 1;
						if (item2 == 0)
						{
							(*span[(int)(*item.Span[0])])++;
						}
						else
						{
							if (array == null || array.Length < item2)
							{
								Array.Resize<int[]>(ref array, item2);
							}
							ref int[] ptr = ref array[item2 - 1];
							if (ptr == null)
							{
								ptr = new int[256];
							}
							ptr[(int)(*item.Span[0])]++;
						}
					}
				}
			}
			if (array != null)
			{
				int[][] array2 = array;
				for (int j = 0; j < array2.Length; j++)
				{
					if (array2[j] != null)
					{
						num2++;
					}
				}
			}
			BytePattern[] array3 = ((num > 0) ? new BytePattern[num] : null);
			int num3 = 0;
			BytePatternCollection.HomogenousPatternCollection[] array4 = new BytePatternCollection.HomogenousPatternCollection[num2];
			int num4 = 1;
			array4[0] = new BytePatternCollection.HomogenousPatternCollection(0);
			for (int k = 0; k < patterns.Length; k++)
			{
				BytePattern bytePattern2 = *patterns.Span[k];
				if (bytePattern2 != null)
				{
					ValueTuple<ReadOnlyMemory<byte>, int> firstLiteralSegment2 = bytePattern2.FirstLiteralSegment;
					ReadOnlyMemory<byte> item3 = firstLiteralSegment2.Item1;
					int item4 = firstLiteralSegment2.Item2;
					if (item3.Length == 0)
					{
						array3[num3++] = bytePattern2;
					}
					else
					{
						int num5 = -1;
						for (int l = 0; l < array4.Length; l++)
						{
							if (array4[l].Offset == item4)
							{
								num5 = l;
								break;
							}
						}
						if (num5 == -1)
						{
							num5 = num4++;
							array4[num5] = new BytePatternCollection.HomogenousPatternCollection(item4);
						}
						ReadOnlySpan<int> readOnlySpan = ((item4 == 0) ? span : array[item4 - 1].AsSpan<int>());
						BytePatternCollection.<ComputeLut>g__AddToPatternCollection|15_0(ref array4[num5], readOnlySpan, bytePattern2);
						if (num5 > 0 && array4[num5 - 1].Offset > array4[num5].Offset)
						{
							Helpers.Swap<BytePatternCollection.HomogenousPatternCollection>(ref array4[num5 - 1], ref array4[num5]);
						}
					}
				}
			}
			return new ValueTuple<BytePatternCollection.HomogenousPatternCollection[], BytePattern[]>(array4, array3);
		}

		// Token: 0x06001C25 RID: 7205 RVA: 0x0005A360 File Offset: 0x00058560
		public unsafe bool TryMatchAt(ReadOnlySpan<byte> data, out ulong address, [Nullable(1)] [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out BytePattern matchingPattern, out int length)
		{
			if (data.Length < this.MinLength)
			{
				length = 0;
				address = 0UL;
				matchingPattern = null;
				return false;
			}
			Span<byte> span = new Span<byte>(stackalloc byte[(UIntPtr)8], 8);
			bool flag = this.TryMatchAt(data, span, out matchingPattern, out length);
			address = Unsafe.ReadUnaligned<ulong>(span[0]);
			return flag;
		}

		// Token: 0x06001C26 RID: 7206 RVA: 0x0005A3B0 File Offset: 0x000585B0
		public unsafe bool TryMatchAt(ReadOnlySpan<byte> data, Span<byte> addrBuf, [Nullable(1)] [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out BytePattern matchingPattern, out int length)
		{
			if (data.Length < this.MinLength)
			{
				length = 0;
				matchingPattern = null;
				return false;
			}
			for (int i = 0; i < this.patternCollections.Length; i++)
			{
				ref BytePatternCollection.HomogenousPatternCollection ptr = ref this.patternCollections[i];
				if (data.Length >= ptr.Offset + ptr.MinLength)
				{
					byte b = *data[ptr.Offset];
					BytePattern[] array = ptr.Lut[(int)b];
					if (array != null)
					{
						foreach (BytePattern bytePattern in array)
						{
							if (bytePattern.TryMatchAt(data, addrBuf, out length))
							{
								matchingPattern = bytePattern;
								return true;
							}
						}
					}
				}
			}
			if (this.emptyPatterns != null)
			{
				foreach (BytePattern bytePattern2 in this.emptyPatterns)
				{
					if (bytePattern2.TryMatchAt(data, addrBuf, out length))
					{
						matchingPattern = bytePattern2;
						return true;
					}
				}
			}
			matchingPattern = null;
			length = 0;
			return false;
		}

		// Token: 0x06001C27 RID: 7207 RVA: 0x0005A49C File Offset: 0x0005869C
		public unsafe bool TryFindMatch(ReadOnlySpan<byte> data, out ulong address, [Nullable(1)] [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out BytePattern matchingPattern, out int offset, out int length)
		{
			if (data.Length < this.MinLength)
			{
				length = (offset = 0);
				address = 0UL;
				matchingPattern = null;
				return false;
			}
			Span<byte> span = new Span<byte>(stackalloc byte[(UIntPtr)8], 8);
			bool flag = this.TryFindMatch(data, span, out matchingPattern, out offset, out length);
			address = Unsafe.ReadUnaligned<ulong>(span[0]);
			return flag;
		}

		// Token: 0x06001C28 RID: 7208 RVA: 0x0005A4F4 File Offset: 0x000586F4
		public unsafe bool TryFindMatch(ReadOnlySpan<byte> data, Span<byte> addrBuf, [Nullable(1)] [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out BytePattern matchingPattern, out int offset, out int length)
		{
			if (data.Length < this.MinLength)
			{
				length = (offset = 0);
				matchingPattern = null;
				return false;
			}
			ReadOnlySpan<byte> span = this.PossibleFirstBytes.Span;
			int num = 0;
			ref BytePatternCollection.HomogenousPatternCollection ptr;
			for (;;)
			{
				int num2 = data.Slice(num).IndexOfAny<byte>(span);
				if (num2 < 0)
				{
					goto IL_011B;
				}
				offset = num + num2;
				byte b = *data[offset];
				for (int i = 0; i < this.patternCollections.Length; i++)
				{
					ptr = ref this.patternCollections[i];
					if (offset >= ptr.Offset && data.Length >= offset + ptr.MinLength)
					{
						BytePattern[] array = ptr.Lut[(int)b];
						if (array != null)
						{
							foreach (BytePattern bytePattern in array)
							{
								if ((offset == 0 || !bytePattern.MustMatchAtStart) && bytePattern.TryMatchAt(data.Slice(offset - ptr.Offset), addrBuf, out length))
								{
									goto Block_7;
								}
							}
						}
					}
				}
				num = offset + 1;
			}
			Block_7:
			offset -= ptr.Offset;
			BytePattern bytePattern;
			matchingPattern = bytePattern;
			return true;
			IL_011B:
			if (this.emptyPatterns != null)
			{
				foreach (BytePattern bytePattern2 in this.emptyPatterns)
				{
					if (bytePattern2.TryFindMatch(data, addrBuf, out offset, out length))
					{
						matchingPattern = bytePattern2;
						return true;
					}
				}
			}
			matchingPattern = null;
			offset = 0;
			length = 0;
			return false;
		}

		// Token: 0x17000612 RID: 1554
		// (get) Token: 0x06001C29 RID: 7209 RVA: 0x0005A664 File Offset: 0x00058864
		private ReadOnlyMemory<byte> PossibleFirstBytes
		{
			get
			{
				ReadOnlyMemory<byte> readOnlyMemory = this.lazyPossibleFirstBytes.GetValueOrDefault();
				if (this.lazyPossibleFirstBytes == null)
				{
					readOnlyMemory = this.GetPossibleFirstBytes();
					this.lazyPossibleFirstBytes = new ReadOnlyMemory<byte>?(readOnlyMemory);
					return readOnlyMemory;
				}
				return readOnlyMemory;
			}
		}

		// Token: 0x06001C2A RID: 7210 RVA: 0x0005A6A0 File Offset: 0x000588A0
		private ReadOnlyMemory<byte> GetPossibleFirstBytes()
		{
			Memory<byte> memory = new byte[512].AsMemory<byte>();
			BytePatternCollection.FirstByteCollection firstByteCollection = new BytePatternCollection.FirstByteCollection(memory.Span);
			for (int i = 0; i < this.patternCollections.Length; i++)
			{
				this.patternCollections[i].AddFirstBytes(ref firstByteCollection);
			}
			return memory.Slice(0, firstByteCollection.FirstBytes.Length);
		}

		// Token: 0x06001C2B RID: 7211 RVA: 0x0005A710 File Offset: 0x00058910
		[CompilerGenerated]
		internal unsafe static void <ComputeLut>g__AddToPatternCollection|15_0(ref BytePatternCollection.HomogenousPatternCollection collection, ReadOnlySpan<int> arrayCounts, [Nullable(1)] BytePattern pattern)
		{
			ReadOnlyMemory<byte> item = pattern.FirstLiteralSegment.Item1;
			if (collection.Lut == null)
			{
				BytePattern[][] array = new BytePattern[256][];
				for (int i = 0; i < arrayCounts.Length; i++)
				{
					if (*arrayCounts[i] > 0)
					{
						array[i] = new BytePattern[*arrayCounts[i]];
					}
				}
				collection.Lut = array;
			}
			BytePattern[] array2 = collection.Lut[(int)(*item.Span[0])];
			int num = Array.IndexOf<BytePattern>(array2, null);
			array2[num] = pattern;
			if (pattern.MinLength < collection.MinLength)
			{
				collection.MinLength = pattern.MinLength;
			}
		}

		// Token: 0x0400118E RID: 4494
		[Nullable(1)]
		private readonly BytePatternCollection.HomogenousPatternCollection[] patternCollections;

		// Token: 0x0400118F RID: 4495
		[Nullable(new byte[] { 2, 1 })]
		private readonly BytePattern[] emptyPatterns;

		// Token: 0x04001193 RID: 4499
		private ReadOnlyMemory<byte>? lazyPossibleFirstBytes;

		// Token: 0x020004EB RID: 1259
		private struct HomogenousPatternCollection
		{
			// Token: 0x06001C2C RID: 7212 RVA: 0x0005A7B4 File Offset: 0x000589B4
			public HomogenousPatternCollection(int offs)
			{
				this.Offset = offs;
				this.Lut = null;
				this.MinLength = int.MaxValue;
			}

			// Token: 0x06001C2D RID: 7213 RVA: 0x0005A7DC File Offset: 0x000589DC
			public void AddFirstBytes(ref BytePatternCollection.FirstByteCollection bytes)
			{
				for (int i = 0; i < this.Lut.Length; i++)
				{
					if (this.Lut[i] != null)
					{
						bytes.Add((byte)i);
					}
				}
			}

			// Token: 0x04001194 RID: 4500
			[Nullable(new byte[] { 1, 2, 1 })]
			public BytePattern[][] Lut;

			// Token: 0x04001195 RID: 4501
			public readonly int Offset;

			// Token: 0x04001196 RID: 4502
			public int MinLength;
		}

		// Token: 0x020004EC RID: 1260
		private ref struct FirstByteCollection
		{
			// Token: 0x17000613 RID: 1555
			// (get) Token: 0x06001C2E RID: 7214 RVA: 0x0005A80E File Offset: 0x00058A0E
			public ReadOnlySpan<byte> FirstBytes
			{
				get
				{
					return this.firstByteStore.Slice(0, this.firstBytesRecorded);
				}
			}

			// Token: 0x06001C2F RID: 7215 RVA: 0x0005A827 File Offset: 0x00058A27
			public FirstByteCollection(Span<byte> store)
			{
				this = new BytePatternCollection.FirstByteCollection(store.Slice(0, 256), store.Slice(256, 256));
			}

			// Token: 0x06001C30 RID: 7216 RVA: 0x0005A84D File Offset: 0x00058A4D
			public FirstByteCollection(Span<byte> store, Span<byte> indicies)
			{
				this.firstByteStore = store;
				this.byteIndicies = indicies;
				this.firstBytesRecorded = 0;
				this.byteIndicies.Fill(byte.MaxValue);
			}

			// Token: 0x06001C31 RID: 7217 RVA: 0x0005A874 File Offset: 0x00058A74
			public unsafe void Add(byte value)
			{
				ref byte ptr = ref this.byteIndicies[(int)value];
				if (ptr == 255)
				{
					ptr = (byte)this.firstBytesRecorded;
					*this.firstByteStore[(int)ptr] = value;
					this.firstBytesRecorded = Math.Min(this.firstBytesRecorded + 1, 256);
				}
			}

			// Token: 0x04001197 RID: 4503
			private Span<byte> firstByteStore;

			// Token: 0x04001198 RID: 4504
			private Span<byte> byteIndicies;

			// Token: 0x04001199 RID: 4505
			private int firstBytesRecorded;

			// Token: 0x0400119A RID: 4506
			public const int SingleAllocationSize = 512;
		}
	}
}
