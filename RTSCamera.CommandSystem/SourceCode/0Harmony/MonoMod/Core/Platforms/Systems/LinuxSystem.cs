using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using MonoMod.Core.Interop;
using MonoMod.Core.Platforms.Memory;
using MonoMod.Core.Utils;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Systems
{
	// Token: 0x02000516 RID: 1302
	internal sealed class LinuxSystem : ISystem, IInitialize<IArchitecture>
	{
		// Token: 0x17000657 RID: 1623
		// (get) Token: 0x06001D3B RID: 7483 RVA: 0x0005D907 File Offset: 0x0005BB07
		public OSKind Target
		{
			get
			{
				return OSKind.Linux;
			}
		}

		// Token: 0x17000658 RID: 1624
		// (get) Token: 0x06001D3C RID: 7484 RVA: 0x0005D90B File Offset: 0x0005BB0B
		public SystemFeature Features
		{
			get
			{
				return SystemFeature.RWXPages | SystemFeature.RXPages;
			}
		}

		// Token: 0x17000659 RID: 1625
		// (get) Token: 0x06001D3D RID: 7485 RVA: 0x0005D90E File Offset: 0x0005BB0E
		public Abi? DefaultAbi
		{
			get
			{
				return new Abi?(this.defaultAbi);
			}
		}

		// Token: 0x06001D3E RID: 7486 RVA: 0x0005D91B File Offset: 0x0005BB1B
		[return: Nullable(new byte[] { 1, 2 })]
		public IEnumerable<string> EnumerateLoadedModuleFiles()
		{
			return from ProcessModule m in Process.GetCurrentProcess().Modules
				select m.FileName;
		}

		// Token: 0x1700065A RID: 1626
		// (get) Token: 0x06001D3F RID: 7487 RVA: 0x0005D950 File Offset: 0x0005BB50
		[Nullable(1)]
		public IMemoryAllocator MemoryAllocator
		{
			[NullableContext(1)]
			get
			{
				return this.allocator;
			}
		}

		// Token: 0x06001D40 RID: 7488 RVA: 0x0005D958 File Offset: 0x0005BB58
		public LinuxSystem()
		{
			this.PageSize = (IntPtr)Unix.Sysconf(Unix.SysconfName.PageSize);
			this.allocator = new LinuxSystem.MmapPagedMemoryAllocator(this.PageSize);
			ArchitectureKind architecture = PlatformDetection.Architecture;
			if (architecture == ArchitectureKind.x86_64)
			{
				ReadOnlyMemory<SpecialArgumentKind> readOnlyMemory = new SpecialArgumentKind[]
				{
					SpecialArgumentKind.ReturnBuffer,
					SpecialArgumentKind.ThisPointer,
					SpecialArgumentKind.UserArguments
				};
				Classifier classifier;
				if ((classifier = LinuxSystem.<>O.<0>__ClassifyAMD64) == null)
				{
					classifier = (LinuxSystem.<>O.<0>__ClassifyAMD64 = new Classifier(SystemVABI.ClassifyAMD64));
				}
				this.defaultAbi = new Abi(readOnlyMemory, classifier, true);
				return;
			}
			if (architecture != ArchitectureKind.Arm64)
			{
				throw new NotImplementedException();
			}
			ReadOnlyMemory<SpecialArgumentKind> readOnlyMemory2 = new SpecialArgumentKind[]
			{
				SpecialArgumentKind.ThisPointer,
				SpecialArgumentKind.UserArguments
			};
			Classifier classifier2;
			if ((classifier2 = LinuxSystem.<>O.<1>__ClassifyARM64) == null)
			{
				classifier2 = (LinuxSystem.<>O.<1>__ClassifyARM64 = new Classifier(SystemVABI.ClassifyARM64));
			}
			this.defaultAbi = new Abi(readOnlyMemory2, classifier2, false);
		}

		// Token: 0x06001D41 RID: 7489 RVA: 0x0005DA14 File Offset: 0x0005BC14
		[return: NativeInteger]
		public IntPtr GetSizeOfReadableMemory(IntPtr start, [NativeInteger] IntPtr guess)
		{
			IntPtr intPtr = this.allocator.RoundDownToPageBoundary(start);
			if (!LinuxSystem.MmapPagedMemoryAllocator.PageReadable(intPtr))
			{
				return (IntPtr)0;
			}
			intPtr += this.PageSize;
			IntPtr intPtr2 = intPtr - start;
			while (intPtr2 < guess)
			{
				if (!LinuxSystem.MmapPagedMemoryAllocator.PageReadable(intPtr))
				{
					return intPtr2;
				}
				intPtr2 += this.PageSize;
				intPtr += this.PageSize;
			}
			return intPtr2;
		}

		// Token: 0x06001D42 RID: 7490 RVA: 0x0005DA6C File Offset: 0x0005BC6C
		public unsafe void PatchData(PatchTargetKind patchKind, IntPtr patchTarget, ReadOnlySpan<byte> data, Span<byte> backup)
		{
			if (patchKind == PatchTargetKind.Executable)
			{
				this.ProtectRWX(patchTarget, (IntPtr)data.Length);
			}
			else
			{
				this.ProtectRW(patchTarget, (IntPtr)data.Length);
			}
			Span<byte> span = new Span<byte>((void*)patchTarget, data.Length);
			span.TryCopyTo(backup);
			data.CopyTo(span);
		}

		// Token: 0x06001D43 RID: 7491 RVA: 0x0005DAC4 File Offset: 0x0005BCC4
		private void RoundToPageBoundary([NativeInteger] ref IntPtr addr, [NativeInteger] ref IntPtr size)
		{
			IntPtr intPtr = this.allocator.RoundDownToPageBoundary(addr);
			size += addr - intPtr;
			addr = intPtr;
		}

		// Token: 0x06001D44 RID: 7492 RVA: 0x0005DAEB File Offset: 0x0005BCEB
		private void ProtectRW(IntPtr addr, [NativeInteger] IntPtr size)
		{
			this.RoundToPageBoundary(ref addr, ref size);
			if (Unix.Mprotect(addr, (UIntPtr)size, Unix.Protection.Read | Unix.Protection.Write) != 0)
			{
				throw new Win32Exception(Unix.Errno);
			}
		}

		// Token: 0x06001D45 RID: 7493 RVA: 0x0005DB0C File Offset: 0x0005BD0C
		private void ProtectRWX(IntPtr addr, [NativeInteger] IntPtr size)
		{
			this.RoundToPageBoundary(ref addr, ref size);
			if (Unix.Mprotect(addr, (UIntPtr)size, Unix.Protection.Read | Unix.Protection.Write | Unix.Protection.Execute) != 0)
			{
				throw new Win32Exception(Unix.Errno);
			}
		}

		// Token: 0x06001D46 RID: 7494 RVA: 0x0005DB2D File Offset: 0x0005BD2D
		[NullableContext(1)]
		void IInitialize<IArchitecture>.Initialize(IArchitecture value)
		{
			this.arch = value;
		}

		// Token: 0x1700065B RID: 1627
		// (get) Token: 0x06001D47 RID: 7495 RVA: 0x0005DB38 File Offset: 0x0005BD38
		[Nullable(2)]
		public INativeExceptionHelper NativeExceptionHelper
		{
			[NullableContext(2)]
			get
			{
				PosixExceptionHelper posixExceptionHelper;
				if ((posixExceptionHelper = this.lazyNativeExceptionHelper) == null)
				{
					posixExceptionHelper = (this.lazyNativeExceptionHelper = this.CreateNativeExceptionHelper());
				}
				return posixExceptionHelper;
			}
		}

		// Token: 0x1700065C RID: 1628
		// (get) Token: 0x06001D48 RID: 7496 RVA: 0x0005DB5E File Offset: 0x0005BD5E
		private unsafe static ReadOnlySpan<byte> NEHTempl
		{
			get
			{
				return new ReadOnlySpan<byte>((void*)(&<24b3ba8a-00b7-40fc-a603-2711fa115297><PrivateImplementationDetails>.93CA2598B856E56332AD9FCA06FE4AE26CF02ED7D37C8C4D790EDFC53FB9DA81), 26);
			}
		}

		// Token: 0x06001D49 RID: 7497 RVA: 0x0005DB6C File Offset: 0x0005BD6C
		[NullableContext(1)]
		private PosixExceptionHelper CreateNativeExceptionHelper()
		{
			Helpers.Assert(this.arch != null, null, "arch is not null");
			ArchitectureKind target = this.arch.Target;
			string text;
			if (target != ArchitectureKind.x86_64)
			{
				if (target != ArchitectureKind.Arm64)
				{
					throw new NotImplementedException("No exception helper for current arch");
				}
				text = "exhelper_linux_arm64.so";
			}
			else
			{
				text = "exhelper_linux_x86_64.so";
			}
			string text2 = text;
			string text3;
			using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(text2))
			{
				Helpers.Assert(manifestResourceStream != null, null, "embedded is not null");
				text3 = LinuxSystem.LinuxNativeLibDrop.Instance.DropLibrary(manifestResourceStream, LinuxSystem.NEHTempl);
			}
			return PosixExceptionHelper.CreateHelper(this.arch, text3);
		}

		// Token: 0x06001D4A RID: 7498 RVA: 0x00045AA1 File Offset: 0x00043CA1
		public IntPtr GetNativeJitHookConfig(int runtimeMajMin)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0400120A RID: 4618
		private readonly Abi defaultAbi;

		// Token: 0x0400120B RID: 4619
		[NativeInteger]
		private readonly IntPtr PageSize;

		// Token: 0x0400120C RID: 4620
		[Nullable(1)]
		private readonly LinuxSystem.MmapPagedMemoryAllocator allocator;

		// Token: 0x0400120D RID: 4621
		[Nullable(2)]
		private IArchitecture arch;

		// Token: 0x0400120E RID: 4622
		[Nullable(2)]
		private PosixExceptionHelper lazyNativeExceptionHelper;

		// Token: 0x02000517 RID: 1303
		[NullableContext(1)]
		[Nullable(0)]
		private sealed class MmapPagedMemoryAllocator : PagedMemoryAllocator
		{
			// Token: 0x06001D4B RID: 7499 RVA: 0x0005DC1C File Offset: 0x0005BE1C
			public MmapPagedMemoryAllocator([NativeInteger] IntPtr pageSize)
				: base(pageSize)
			{
			}

			// Token: 0x06001D4C RID: 7500 RVA: 0x0005DC2C File Offset: 0x0005BE2C
			unsafe static MmapPagedMemoryAllocator()
			{
				IntPtr intPtr = stackalloc byte[(UIntPtr)8];
				if (Unix.Pipe2(intPtr, Unix.PipeFlags.CloseOnExec) == -1)
				{
					throw new Win32Exception(Unix.Errno, "Failed to create pipe for page probes");
				}
				LinuxSystem.MmapPagedMemoryAllocator.PageProbePipeReadFD = *intPtr;
				LinuxSystem.MmapPagedMemoryAllocator.PageProbePipeWriteFD = *(intPtr + 4);
			}

			// Token: 0x06001D4D RID: 7501 RVA: 0x0005DC6C File Offset: 0x0005BE6C
			public unsafe static bool PageAllocated([NativeInteger] IntPtr page)
			{
				byte b;
				if (Unix.Mincore(page, (UIntPtr)((IntPtr)1), &b) != -1)
				{
					return true;
				}
				int errno = Unix.Errno;
				if (errno == 12)
				{
					return false;
				}
				if (errno == 38)
				{
					throw new LinuxSystem.MmapPagedMemoryAllocator.SyscallNotImplementedException();
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(48, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Got unimplemented errno for mincore(2); errno = ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(errno);
				throw new NotImplementedException(defaultInterpolatedStringHandler.ToStringAndClear());
			}

			// Token: 0x06001D4E RID: 7502 RVA: 0x0005DCCC File Offset: 0x0005BECC
			public unsafe static bool PageReadable([NativeInteger] IntPtr page)
			{
				if (Unix.Write(LinuxSystem.MmapPagedMemoryAllocator.PageProbePipeWriteFD, page, (IntPtr)1) == (IntPtr)(-1))
				{
					int errno = Unix.Errno;
					if (errno == 14)
					{
						return false;
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(46, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Got unimplemented errno for write(2); errno = ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(errno);
					throw new NotImplementedException(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				else
				{
					byte b;
					if (Unix.Read(LinuxSystem.MmapPagedMemoryAllocator.PageProbePipeReadFD, new IntPtr((void*)(&b)), (IntPtr)1) == (IntPtr)(-1))
					{
						throw new Win32Exception("Failed to clean up page probe pipe after successful page probe");
					}
					return true;
				}
			}

			// Token: 0x06001D4F RID: 7503 RVA: 0x0005DD48 File Offset: 0x0005BF48
			protected override bool TryAllocateNewPage(AllocationRequest request, [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out IAllocatedMemory allocated)
			{
				Unix.Protection protection = (request.Executable ? Unix.Protection.Execute : Unix.Protection.None);
				protection |= Unix.Protection.Read | Unix.Protection.Write;
				IntPtr intPtr = Unix.Mmap(IntPtr.Zero, (UIntPtr)base.PageSize, protection, Unix.MmapFlags.Private | Unix.MmapFlags.Anonymous, -1, 0);
				long num = (long)intPtr;
				bool flag = num - -1L <= 1L;
				if (flag)
				{
					int errno = Unix.Errno;
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(28, 2, out flag);
					if (flag)
					{
						debugLogErrorStringHandler.AppendLiteral("Error creating allocation: ");
						debugLogErrorStringHandler.AppendFormatted<int>(errno);
						debugLogErrorStringHandler.AppendLiteral(" ");
						debugLogErrorStringHandler.AppendFormatted(new Win32Exception(errno).Message);
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler);
					allocated = null;
					return false;
				}
				PagedMemoryAllocator.Page page = new PagedMemoryAllocator.Page(this, intPtr, (uint)base.PageSize, request.Executable);
				base.InsertAllocatedPage(page);
				PagedMemoryAllocator.PageAllocation pageAllocation;
				if (!page.TryAllocate((uint)request.Size, (uint)request.Alignment, out pageAllocation))
				{
					base.RegisterForCleanup(page);
					allocated = null;
					return false;
				}
				allocated = pageAllocation;
				return true;
			}

			// Token: 0x06001D50 RID: 7504 RVA: 0x0005DE34 File Offset: 0x0005C034
			protected override bool TryAllocateNewPage(PositionedAllocationRequest request, [NativeInteger] IntPtr targetPage, [NativeInteger] IntPtr lowPageBound, [NativeInteger] IntPtr highPageBound, [<24b3ba8a-00b7-40fc-a603-2711fa115297>MaybeNullWhen(false)] out IAllocatedMemory allocated)
			{
				if (!this.canTestPageAllocation)
				{
					allocated = null;
					return false;
				}
				Unix.Protection protection = (request.Base.Executable ? Unix.Protection.Execute : Unix.Protection.None);
				protection |= Unix.Protection.Read | Unix.Protection.Write;
				IntPtr intPtr = (IntPtr)request.Base.Size / base.PageSize + (IntPtr)1;
				IntPtr intPtr2 = targetPage - base.PageSize;
				IntPtr intPtr3 = targetPage;
				IntPtr intPtr4 = (IntPtr)(-1);
				try
				{
					IL_00C5:
					while (intPtr2 >= lowPageBound || intPtr3 <= highPageBound)
					{
						if (intPtr3 <= highPageBound)
						{
							for (IntPtr intPtr5 = (IntPtr)0; intPtr5 < intPtr; intPtr5++)
							{
								if (LinuxSystem.MmapPagedMemoryAllocator.PageAllocated(intPtr3 + base.PageSize * intPtr5))
								{
									intPtr3 += base.PageSize;
									goto IL_008E;
								}
							}
							intPtr4 = intPtr3;
							break;
						}
						IL_008E:
						if (intPtr2 >= lowPageBound)
						{
							for (IntPtr intPtr6 = (IntPtr)0; intPtr6 < intPtr; intPtr6++)
							{
								if (LinuxSystem.MmapPagedMemoryAllocator.PageAllocated(intPtr2 + base.PageSize * intPtr6))
								{
									intPtr2 -= base.PageSize;
									goto IL_00C5;
								}
							}
							intPtr4 = intPtr2;
							break;
						}
					}
				}
				catch (LinuxSystem.MmapPagedMemoryAllocator.SyscallNotImplementedException)
				{
					this.canTestPageAllocation = false;
					allocated = null;
					return false;
				}
				if (intPtr4 == (IntPtr)(-1))
				{
					allocated = null;
					return false;
				}
				IntPtr intPtr7 = Unix.Mmap(intPtr4, (UIntPtr)base.PageSize, protection, Unix.MmapFlags.Private | Unix.MmapFlags.Anonymous | Unix.MmapFlags.FixedNoReplace, -1, 0);
				long num = (long)intPtr7;
				bool flag = num - -1L <= 1L;
				if (flag)
				{
					allocated = null;
					return false;
				}
				PagedMemoryAllocator.Page page = new PagedMemoryAllocator.Page(this, intPtr7, (uint)base.PageSize, request.Base.Executable);
				base.InsertAllocatedPage(page);
				PagedMemoryAllocator.PageAllocation pageAllocation;
				if (!page.TryAllocate((uint)request.Base.Size, (uint)request.Base.Alignment, out pageAllocation))
				{
					base.RegisterForCleanup(page);
					allocated = null;
					return false;
				}
				if (pageAllocation.BaseAddress < request.LowBound || pageAllocation.BaseAddress + (IntPtr)pageAllocation.Size >= request.HighBound)
				{
					pageAllocation.Dispose();
					allocated = null;
					return false;
				}
				allocated = pageAllocation;
				return true;
			}

			// Token: 0x06001D51 RID: 7505 RVA: 0x0005E01C File Offset: 0x0005C21C
			protected override bool TryFreePage(PagedMemoryAllocator.Page page, [Nullable(2)] [<24b3ba8a-00b7-40fc-a603-2711fa115297>NotNullWhen(false)] out string errorMsg)
			{
				if (Unix.Munmap(page.BaseAddr, (UIntPtr)page.Size) != 0)
				{
					errorMsg = new Win32Exception(Unix.Errno).Message;
					return false;
				}
				errorMsg = null;
				return true;
			}

			// Token: 0x0400120F RID: 4623
			private static int PageProbePipeReadFD;

			// Token: 0x04001210 RID: 4624
			private static int PageProbePipeWriteFD;

			// Token: 0x04001211 RID: 4625
			private bool canTestPageAllocation = true;

			// Token: 0x02000518 RID: 1304
			[NullableContext(0)]
			private sealed class SyscallNotImplementedException : Exception
			{
			}
		}

		// Token: 0x02000519 RID: 1305
		private sealed class LinuxNativeLibDrop : PosixNativeLibraryDrop
		{
			// Token: 0x06001D53 RID: 7507 RVA: 0x0005E049 File Offset: 0x0005C249
			protected override void CloseFileDescriptor([NativeInteger] IntPtr fd)
			{
				Unix.Close((int)fd);
			}

			// Token: 0x06001D54 RID: 7508 RVA: 0x0005E054 File Offset: 0x0005C254
			[return: NativeInteger]
			protected unsafe override IntPtr Mkstemp(Span<byte> template)
			{
				int num;
				fixed (byte* pinnableReference = template.GetPinnableReference())
				{
					num = Unix.MkSTemp(pinnableReference);
				}
				if (num == -1)
				{
					int errno = Unix.Errno;
					Win32Exception ex = new Win32Exception(errno);
					bool flag;
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(29, 2, out flag);
					if (flag)
					{
						debugLogErrorStringHandler.AppendLiteral("Could not create temp file: ");
						debugLogErrorStringHandler.AppendFormatted<int>(errno);
						debugLogErrorStringHandler.AppendLiteral(" ");
						debugLogErrorStringHandler.AppendFormatted<Win32Exception>(ex);
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler);
					throw ex;
				}
				return (IntPtr)num;
			}

			// Token: 0x04001212 RID: 4626
			[Nullable(1)]
			public static readonly LinuxSystem.LinuxNativeLibDrop Instance = new LinuxSystem.LinuxNativeLibDrop();
		}

		// Token: 0x0200051A RID: 1306
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04001213 RID: 4627
			public static Classifier <0>__ClassifyAMD64;

			// Token: 0x04001214 RID: 4628
			public static Classifier <1>__ClassifyARM64;
		}
	}
}
