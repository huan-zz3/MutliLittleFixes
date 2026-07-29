using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MonoMod.Core.Platforms.Memory
{
	// Token: 0x0200054E RID: 1358
	[NullableContext(1)]
	[Nullable(0)]
	internal abstract class PagedMemoryAllocator : IMemoryAllocator
	{
		// Token: 0x1700069E RID: 1694
		// (get) Token: 0x06001E71 RID: 7793 RVA: 0x0006351B File Offset: 0x0006171B
		[NativeInteger]
		protected IntPtr PageSize
		{
			[return: NativeInteger]
			get
			{
				return this.pageSize;
			}
		}

		// Token: 0x06001E72 RID: 7794 RVA: 0x00063524 File Offset: 0x00061724
		protected PagedMemoryAllocator([NativeInteger] IntPtr pageSize)
		{
			this.pageSize = pageSize;
			this.pageSizeIsPow2 = BitOperationsEx.IsPow2(pageSize);
			this.pageBaseMask = ~(IntPtr)0 << (BitOperationsEx.TrailingZeroCount(pageSize) & (sizeof(IntPtr) * 8 - 1));
		}

		// Token: 0x06001E73 RID: 7795 RVA: 0x00063588 File Offset: 0x00061788
		[return: NativeInteger]
		public IntPtr RoundDownToPageBoundary([NativeInteger] IntPtr addr)
		{
			if (this.pageSizeIsPow2)
			{
				return addr & this.pageBaseMask;
			}
			return addr - addr % this.pageSize;
		}

		// Token: 0x06001E74 RID: 7796 RVA: 0x000635A8 File Offset: 0x000617A8
		protected unsafe void InsertAllocatedPage(PagedMemoryAllocator.Page page)
		{
			if (this.pageCount == this.allocationList.Length)
			{
				int num = (int)BitOperationsEx.RoundUpToPowerOf2((uint)(this.allocationList.Length + 1));
				Array.Resize<PagedMemoryAllocator.Page>(ref this.allocationList, num);
			}
			Span<PagedMemoryAllocator.Page> span = this.allocationList.AsSpan<PagedMemoryAllocator.Page>();
			int num2 = span.Slice(0, this.pageCount).BinarySearch(page, default(PagedMemoryAllocator.PageComparer));
			if (num2 >= 0)
			{
				return;
			}
			num2 = ~num2;
			if (num2 + 1 < span.Length)
			{
				span.Slice(num2, this.pageCount - num2).CopyTo(span.Slice(num2 + 1));
			}
			*span[num2] = page;
			this.pageCount++;
		}

		// Token: 0x06001E75 RID: 7797 RVA: 0x0006365C File Offset: 0x0006185C
		private void RemoveAllocatedPage(PagedMemoryAllocator.Page page)
		{
			Span<PagedMemoryAllocator.Page> span = this.allocationList.AsSpan<PagedMemoryAllocator.Page>();
			int num = span.Slice(0, this.pageCount).BinarySearch(page, default(PagedMemoryAllocator.PageComparer));
			if (num < 0)
			{
				return;
			}
			span.Slice(num + 1).CopyTo(span.Slice(num));
			this.pageCount--;
		}

		// Token: 0x1700069F RID: 1695
		// (get) Token: 0x06001E76 RID: 7798 RVA: 0x000636C0 File Offset: 0x000618C0
		[Nullable(new byte[] { 0, 1 })]
		private ReadOnlySpan<PagedMemoryAllocator.Page> AllocList
		{
			[return: Nullable(new byte[] { 0, 1 })]
			get
			{
				return this.allocationList.AsSpan<PagedMemoryAllocator.Page>().Slice(0, this.pageCount);
			}
		}

		// Token: 0x06001E77 RID: 7799 RVA: 0x000636EC File Offset: 0x000618EC
		private int GetBoundIndex(IntPtr ptr)
		{
			int num = this.AllocList.BinarySearch(new PagedMemoryAllocator.PageAddrComparable(ptr));
			if (num < 0)
			{
				return ~num;
			}
			return num;
		}

		// Token: 0x06001E78 RID: 7800 RVA: 0x00063714 File Offset: 0x00061914
		protected void RegisterForCleanup(PagedMemoryAllocator.Page page)
		{
			if (Environment.HasShutdownStarted || AppDomain.CurrentDomain.IsFinalizingForUnload())
			{
				return;
			}
			this.pagesToClean.Add(page);
			if (Interlocked.CompareExchange(ref this.registeredForCleanup, 1, 0) == 0)
			{
				Gen2GcCallback.Register(new Func<bool>(this.DoCleanup));
			}
		}

		// Token: 0x06001E79 RID: 7801 RVA: 0x00063764 File Offset: 0x00061964
		private bool DoCleanup()
		{
			if (Environment.HasShutdownStarted || AppDomain.CurrentDomain.IsFinalizingForUnload())
			{
				return false;
			}
			Volatile.Write(ref this.registeredForCleanup, 0);
			PagedMemoryAllocator.Page page;
			while (this.pagesToClean.TryTake(out page))
			{
				object obj = this.sync;
				lock (obj)
				{
					if (!page.IsEmpty)
					{
						continue;
					}
					this.RemoveAllocatedPage(page);
				}
				string text;
				if (!this.TryFreePage(page, out text))
				{
					bool flag;
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(27, 1, out flag);
					if (flag)
					{
						debugLogErrorStringHandler.AppendLiteral("Could not deallocate page! ");
						debugLogErrorStringHandler.AppendFormatted(text);
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler);
				}
			}
			return false;
		}

		// Token: 0x06001E7A RID: 7802
		protected abstract bool TryFreePage(PagedMemoryAllocator.Page page, [Nullable(2)] [<24b3ba8a-00b7-40fc-a603-2711fa115297>NotNullWhen(false)] out string errorMsg);

		// Token: 0x170006A0 RID: 1696
		// (get) Token: 0x06001E7B RID: 7803 RVA: 0x00063818 File Offset: 0x00061A18
		public int MaxSize
		{
			get
			{
				return (int)this.pageSize;
			}
		}

		// Token: 0x06001E7C RID: 7804 RVA: 0x00063824 File Offset: 0x00061A24
		public unsafe bool TryAllocateInRange(PositionedAllocationRequest request, [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out IAllocatedMemory allocated)
		{
			if (request.Target < request.LowBound || request.Target > request.HighBound)
			{
				throw new ArgumentException("Target not between low and high", "request");
			}
			if (request.Base.Size < 0)
			{
				throw new ArgumentException("Size is negative", "request");
			}
			if (request.Base.Alignment <= 0)
			{
				throw new ArgumentException("Alignment is zero or negative", "request");
			}
			if ((IntPtr)request.Base.Size > this.pageSize)
			{
				throw new NotSupportedException("Single allocations cannot be larger than a page");
			}
			IntPtr intPtr = this.RoundDownToPageBoundary(request.LowBound + this.pageSize - (IntPtr)1);
			IntPtr intPtr2 = this.RoundDownToPageBoundary(request.HighBound);
			IntPtr intPtr3 = this.RoundDownToPageBoundary(request.Target);
			IntPtr target = request.Target;
			object obj = this.sync;
			bool flag2;
			lock (obj)
			{
				int boundIndex = this.GetBoundIndex(intPtr);
				int boundIndex2 = this.GetBoundIndex(intPtr2);
				if (boundIndex != boundIndex2)
				{
					int boundIndex3 = this.GetBoundIndex(intPtr3);
					int num = boundIndex3 - 1;
					int num2 = boundIndex3;
					while ((ulong)num2 <= (ulong)((long)this.AllocList.Length) && (ulong)num < (ulong)((long)this.AllocList.Length) && (num >= boundIndex || num2 < boundIndex2))
					{
						while ((ulong)num2 < (ulong)((long)this.AllocList.Length) && num2 < boundIndex2)
						{
							if (num >= boundIndex && target - this.AllocList[num]->BaseAddr <= this.AllocList[num2]->BaseAddr - target)
							{
								break;
							}
							if (PagedMemoryAllocator.TryAllocWithPage(*this.AllocList[num2], request, out allocated))
							{
								return true;
							}
							num2++;
						}
						while ((ulong)num < (ulong)((long)this.AllocList.Length) && num >= boundIndex && (num2 >= boundIndex2 || target - this.AllocList[num]->BaseAddr < this.AllocList[num2]->BaseAddr - target))
						{
							if (PagedMemoryAllocator.TryAllocWithPage(*this.AllocList[num], request, out allocated))
							{
								return true;
							}
							num++;
						}
					}
				}
				flag2 = this.TryAllocateNewPage(request, intPtr3, intPtr, intPtr2, out allocated);
			}
			return flag2;
		}

		// Token: 0x06001E7D RID: 7805 RVA: 0x00063ABC File Offset: 0x00061CBC
		private static bool TryAllocWithPage(PagedMemoryAllocator.Page page, PositionedAllocationRequest request, [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out IAllocatedMemory allocated)
		{
			PagedMemoryAllocator.PageAllocation pageAllocation;
			if (page.IsExecutable == request.Base.Executable && page.BaseAddr >= request.LowBound && page.BaseAddr < request.HighBound && page.TryAllocate((uint)request.Base.Size, (uint)request.Base.Alignment, out pageAllocation))
			{
				if (pageAllocation.BaseAddress >= request.LowBound && pageAllocation.BaseAddress < request.HighBound)
				{
					allocated = pageAllocation;
					return true;
				}
				pageAllocation.Dispose();
			}
			allocated = null;
			return false;
		}

		// Token: 0x06001E7E RID: 7806 RVA: 0x00063B54 File Offset: 0x00061D54
		public unsafe bool TryAllocate(AllocationRequest request, [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out IAllocatedMemory allocated)
		{
			if (request.Size < 0)
			{
				throw new ArgumentException("Size is negative", "request");
			}
			if (request.Alignment <= 0)
			{
				throw new ArgumentException("Alignment is zero or negative", "request");
			}
			if ((IntPtr)request.Size > this.pageSize)
			{
				throw new NotSupportedException("Single allocations cannot be larger than a page");
			}
			object obj = this.sync;
			bool flag2;
			lock (obj)
			{
				ReadOnlySpan<PagedMemoryAllocator.Page> allocList = this.AllocList;
				for (int i = 0; i < allocList.Length; i++)
				{
					PagedMemoryAllocator.Page page = *allocList[i];
					PagedMemoryAllocator.PageAllocation pageAllocation;
					if (page.IsExecutable == request.Executable && page.TryAllocate((uint)request.Size, (uint)request.Alignment, out pageAllocation))
					{
						allocated = pageAllocation;
						return true;
					}
				}
				flag2 = this.TryAllocateNewPage(request, out allocated);
			}
			return flag2;
		}

		// Token: 0x06001E7F RID: 7807
		protected abstract bool TryAllocateNewPage(AllocationRequest request, [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out IAllocatedMemory allocated);

		// Token: 0x06001E80 RID: 7808
		protected abstract bool TryAllocateNewPage(PositionedAllocationRequest request, [NativeInteger] IntPtr targetPage, [NativeInteger] IntPtr lowPageBound, [NativeInteger] IntPtr highPageBound, [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out IAllocatedMemory allocated);

		// Token: 0x040012A5 RID: 4773
		[NativeInteger]
		private readonly IntPtr pageBaseMask;

		// Token: 0x040012A6 RID: 4774
		[NativeInteger]
		private readonly IntPtr pageSize;

		// Token: 0x040012A7 RID: 4775
		private readonly bool pageSizeIsPow2;

		// Token: 0x040012A8 RID: 4776
		[Nullable(new byte[] { 1, 2 })]
		private PagedMemoryAllocator.Page[] allocationList = new PagedMemoryAllocator.Page[16];

		// Token: 0x040012A9 RID: 4777
		private int pageCount;

		// Token: 0x040012AA RID: 4778
		private readonly ConcurrentBag<PagedMemoryAllocator.Page> pagesToClean = new ConcurrentBag<PagedMemoryAllocator.Page>();

		// Token: 0x040012AB RID: 4779
		private int registeredForCleanup;

		// Token: 0x040012AC RID: 4780
		private readonly object sync = new object();

		// Token: 0x0200054F RID: 1359
		[NullableContext(0)]
		private sealed class FreeMem
		{
			// Token: 0x040012AD RID: 4781
			public uint BaseOffset;

			// Token: 0x040012AE RID: 4782
			public uint Size;

			// Token: 0x040012AF RID: 4783
			[Nullable(2)]
			public PagedMemoryAllocator.FreeMem NextFree;
		}

		// Token: 0x02000550 RID: 1360
		[NullableContext(0)]
		protected sealed class PageAllocation : IAllocatedMemory, IDisposable
		{
			// Token: 0x170006A1 RID: 1697
			// (get) Token: 0x06001E82 RID: 7810 RVA: 0x00063C40 File Offset: 0x00061E40
			public bool IsExecutable
			{
				get
				{
					return this.owner.IsExecutable;
				}
			}

			// Token: 0x06001E83 RID: 7811 RVA: 0x00063C4D File Offset: 0x00061E4D
			[NullableContext(1)]
			public PageAllocation(PagedMemoryAllocator.Page page, uint offset, int size)
			{
				this.owner = page;
				this.offset = offset;
				this.Size = size;
			}

			// Token: 0x170006A2 RID: 1698
			// (get) Token: 0x06001E84 RID: 7812 RVA: 0x00063C6A File Offset: 0x00061E6A
			public IntPtr BaseAddress
			{
				get
				{
					return this.owner.BaseAddr + (IntPtr)((UIntPtr)this.offset);
				}
			}

			// Token: 0x170006A3 RID: 1699
			// (get) Token: 0x06001E85 RID: 7813 RVA: 0x00063C7F File Offset: 0x00061E7F
			public int Size { get; }

			// Token: 0x170006A4 RID: 1700
			// (get) Token: 0x06001E86 RID: 7814 RVA: 0x00063C87 File Offset: 0x00061E87
			public unsafe Span<byte> Memory
			{
				get
				{
					return new Span<byte>((void*)this.BaseAddress, this.Size);
				}
			}

			// Token: 0x06001E87 RID: 7815 RVA: 0x00063C9F File Offset: 0x00061E9F
			private void Dispose(bool disposing)
			{
				if (!this.disposedValue)
				{
					this.owner.FreeMem(this.offset, (uint)this.Size);
					this.disposedValue = true;
				}
			}

			// Token: 0x06001E88 RID: 7816 RVA: 0x00063CC8 File Offset: 0x00061EC8
			~PageAllocation()
			{
				this.Dispose(false);
			}

			// Token: 0x06001E89 RID: 7817 RVA: 0x00063CF8 File Offset: 0x00061EF8
			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			// Token: 0x040012B0 RID: 4784
			[Nullable(1)]
			private readonly PagedMemoryAllocator.Page owner;

			// Token: 0x040012B1 RID: 4785
			private readonly uint offset;

			// Token: 0x040012B3 RID: 4787
			private bool disposedValue;
		}

		// Token: 0x02000551 RID: 1361
		[Nullable(0)]
		protected sealed class Page
		{
			// Token: 0x170006A5 RID: 1701
			// (get) Token: 0x06001E8A RID: 7818 RVA: 0x00063D08 File Offset: 0x00061F08
			public bool IsEmpty
			{
				get
				{
					PagedMemoryAllocator.FreeMem freeMem = this.freeList;
					return freeMem != null && freeMem.BaseOffset == 0U && freeMem.Size == this.Size;
				}
			}

			// Token: 0x170006A6 RID: 1702
			// (get) Token: 0x06001E8B RID: 7819 RVA: 0x00063D37 File Offset: 0x00061F37
			public IntPtr BaseAddr { get; }

			// Token: 0x170006A7 RID: 1703
			// (get) Token: 0x06001E8C RID: 7820 RVA: 0x00063D3F File Offset: 0x00061F3F
			public uint Size { get; }

			// Token: 0x170006A8 RID: 1704
			// (get) Token: 0x06001E8D RID: 7821 RVA: 0x00063D47 File Offset: 0x00061F47
			public bool IsExecutable { get; }

			// Token: 0x06001E8E RID: 7822 RVA: 0x00063D50 File Offset: 0x00061F50
			public Page(PagedMemoryAllocator owner, IntPtr baseAddr, uint size, bool isExecutable)
			{
				this.owner = owner;
				this.BaseAddr = baseAddr;
				this.Size = size;
				this.IsExecutable = isExecutable;
				this.freeList = new PagedMemoryAllocator.FreeMem
				{
					BaseOffset = 0U,
					Size = size,
					NextFree = null
				};
			}

			// Token: 0x06001E8F RID: 7823 RVA: 0x00063DB0 File Offset: 0x00061FB0
			public bool TryAllocate(uint size, uint align, [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out PagedMemoryAllocator.PageAllocation alloc)
			{
				object obj = this.sync;
				bool flag2;
				lock (obj)
				{
					ref PagedMemoryAllocator.FreeMem ptr = ref this.freeList;
					uint num = 0U;
					while (ptr != null)
					{
						uint num2 = ptr.BaseOffset % align;
						num2 = ((num2 != 0U) ? (align - num2) : num2);
						if (ptr.Size >= num2 + size)
						{
							num = num2;
							break;
						}
						ptr = ref ptr.NextFree;
					}
					if (ptr == null)
					{
						alloc = null;
						flag2 = false;
					}
					else
					{
						uint num3 = ptr.BaseOffset + num;
						if (num == 0U)
						{
							ptr.BaseOffset += size;
							ptr.Size -= size;
						}
						else
						{
							PagedMemoryAllocator.FreeMem freeMem = new PagedMemoryAllocator.FreeMem
							{
								BaseOffset = ptr.BaseOffset,
								Size = num,
								NextFree = ptr
							};
							ptr.BaseOffset += num + size;
							ptr.Size -= num + size;
							ptr = freeMem;
						}
						this.NormalizeFreeList();
						alloc = new PagedMemoryAllocator.PageAllocation(this, num3, (int)size);
						flag2 = true;
					}
				}
				return flag2;
			}

			// Token: 0x06001E90 RID: 7824 RVA: 0x00063EC8 File Offset: 0x000620C8
			private void NormalizeFreeList()
			{
				ref PagedMemoryAllocator.FreeMem ptr = ref this.freeList;
				while (ptr != null)
				{
					if (ptr.Size <= 0U)
					{
						ptr = ptr.NextFree;
					}
					else
					{
						PagedMemoryAllocator.FreeMem nextFree = ptr.NextFree;
						if (nextFree != null && nextFree.BaseOffset == ptr.BaseOffset + ptr.Size)
						{
							ptr.Size += nextFree.Size;
							ptr.NextFree = nextFree.NextFree;
						}
						else
						{
							ptr = ref ptr.NextFree;
						}
					}
				}
			}

			// Token: 0x06001E91 RID: 7825 RVA: 0x00063F44 File Offset: 0x00062144
			internal void FreeMem(uint offset, uint size)
			{
				object obj = this.sync;
				lock (obj)
				{
					ref PagedMemoryAllocator.FreeMem ptr = ref this.freeList;
					while (ptr != null && ptr.BaseOffset <= offset)
					{
						ptr = ref ptr.NextFree;
					}
					ptr = new PagedMemoryAllocator.FreeMem
					{
						BaseOffset = offset,
						Size = size,
						NextFree = ptr
					};
					this.NormalizeFreeList();
					if (this.IsEmpty)
					{
						this.owner.RegisterForCleanup(this);
					}
				}
			}

			// Token: 0x040012B4 RID: 4788
			private readonly PagedMemoryAllocator owner;

			// Token: 0x040012B5 RID: 4789
			private readonly object sync = new object();

			// Token: 0x040012B6 RID: 4790
			[Nullable(2)]
			private PagedMemoryAllocator.FreeMem freeList;
		}

		// Token: 0x02000552 RID: 1362
		[NullableContext(0)]
		private readonly struct PageComparer : IComparer<PagedMemoryAllocator.Page>
		{
			// Token: 0x06001E92 RID: 7826 RVA: 0x00063FD4 File Offset: 0x000621D4
			[NullableContext(2)]
			public int Compare(PagedMemoryAllocator.Page x, PagedMemoryAllocator.Page y)
			{
				if (x == y)
				{
					return 0;
				}
				if (x == null)
				{
					return 1;
				}
				if (y == null)
				{
					return -1;
				}
				return ((long)x.BaseAddr).CompareTo((long)y.BaseAddr);
			}
		}

		// Token: 0x02000553 RID: 1363
		[NullableContext(0)]
		private readonly struct PageAddrComparable : IComparable<PagedMemoryAllocator.Page>
		{
			// Token: 0x06001E93 RID: 7827 RVA: 0x0006400F File Offset: 0x0006220F
			public PageAddrComparable(IntPtr addr)
			{
				this.addr = addr;
			}

			// Token: 0x06001E94 RID: 7828 RVA: 0x00064018 File Offset: 0x00062218
			[NullableContext(2)]
			public int CompareTo(PagedMemoryAllocator.Page other)
			{
				if (other == null)
				{
					return 1;
				}
				return ((long)this.addr).CompareTo((long)other.BaseAddr);
			}

			// Token: 0x040012BA RID: 4794
			private readonly IntPtr addr;
		}
	}
}
