using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using MonoMod.Core.Interop;
using MonoMod.Core.Platforms.Memory;
using MonoMod.Core.Utils;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Systems
{
	// Token: 0x0200051C RID: 1308
	internal sealed class MacOSSystem : ISystem, IInitialize<IArchitecture>
	{
		// Token: 0x1700065D RID: 1629
		// (get) Token: 0x06001D5A RID: 7514 RVA: 0x00041313 File Offset: 0x0003F513
		public OSKind Target
		{
			get
			{
				return OSKind.OSX;
			}
		}

		// Token: 0x1700065E RID: 1630
		// (get) Token: 0x06001D5B RID: 7515 RVA: 0x0005E0ED File Offset: 0x0005C2ED
		public SystemFeature Features { get; }

		// Token: 0x1700065F RID: 1631
		// (get) Token: 0x06001D5C RID: 7516 RVA: 0x0005E0F5 File Offset: 0x0005C2F5
		public Abi? DefaultAbi { get; }

		// Token: 0x06001D5D RID: 7517 RVA: 0x0005E100 File Offset: 0x0005C300
		public MacOSSystem()
		{
			ArchitectureKind architecture = PlatformDetection.Architecture;
			if (architecture == ArchitectureKind.x86_64)
			{
				this.Features = SystemFeature.RWXPages | SystemFeature.RXPages;
				ReadOnlyMemory<SpecialArgumentKind> readOnlyMemory = new SpecialArgumentKind[]
				{
					SpecialArgumentKind.ReturnBuffer,
					SpecialArgumentKind.ThisPointer,
					SpecialArgumentKind.UserArguments
				};
				Classifier classifier;
				if ((classifier = MacOSSystem.<>O.<0>__ClassifyAMD64) == null)
				{
					classifier = (MacOSSystem.<>O.<0>__ClassifyAMD64 = new Classifier(SystemVABI.ClassifyAMD64));
				}
				this.DefaultAbi = new Abi?(new Abi(readOnlyMemory, classifier, true));
				return;
			}
			if (architecture != ArchitectureKind.Arm64)
			{
				throw new NotImplementedException();
			}
			this.Features = SystemFeature.RXPages | SystemFeature.MayUseNativeJitHooks;
			ReadOnlyMemory<SpecialArgumentKind> readOnlyMemory2 = new SpecialArgumentKind[]
			{
				SpecialArgumentKind.ThisPointer,
				SpecialArgumentKind.UserArguments
			};
			Classifier classifier2;
			if ((classifier2 = MacOSSystem.<>O.<1>__ClassifyARM64) == null)
			{
				classifier2 = (MacOSSystem.<>O.<1>__ClassifyARM64 = new Classifier(SystemVABI.ClassifyARM64));
			}
			this.DefaultAbi = new Abi?(new Abi(readOnlyMemory2, classifier2, false));
		}

		// Token: 0x06001D5E RID: 7518 RVA: 0x0005E1C8 File Offset: 0x0005C3C8
		[return: Nullable(new byte[] { 1, 2 })]
		public unsafe IEnumerable<string> EnumerateLoadedModuleFiles()
		{
			int count = OSX.task_dyld_info.Count;
			OSX.task_dyld_info task_dyld_info = default(OSX.task_dyld_info);
			if (!OSX.task_info(OSX.mach_task_self(), OSX.task_flavor_t.DyldInfo, &task_dyld_info, &count))
			{
				return ArrayEx.Empty<string>();
			}
			ReadOnlySpan<OSX.dyld_image_info> infoArray = task_dyld_info.all_image_infos->InfoArray;
			string[] array = new string[infoArray.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = infoArray[i].imageFilePath.ToString();
			}
			return array;
		}

		// Token: 0x06001D5F RID: 7519 RVA: 0x0005E24C File Offset: 0x0005C44C
		[return: NativeInteger]
		public IntPtr GetSizeOfReadableMemory(IntPtr start, [NativeInteger] IntPtr guess)
		{
			IntPtr intPtr = (IntPtr)0;
			IntPtr intPtr2;
			IntPtr intPtr3;
			OSX.vm_prot_t vm_prot_t;
			OSX.vm_prot_t vm_prot_t2;
			while (MacOSSystem.GetLocalRegionInfo(start, out intPtr2, out intPtr3, out vm_prot_t, out vm_prot_t2))
			{
				if (intPtr2 > start)
				{
					return intPtr;
				}
				if ((vm_prot_t & OSX.vm_prot_t.Read) <= OSX.vm_prot_t.None)
				{
					return intPtr;
				}
				intPtr += intPtr2 + intPtr3 - start;
				start = intPtr2 + intPtr3;
				if (intPtr >= guess)
				{
					return intPtr;
				}
			}
			return intPtr;
		}

		// Token: 0x06001D60 RID: 7520 RVA: 0x0005E298 File Offset: 0x0005C498
		public unsafe void PatchData(PatchTargetKind targetKind, IntPtr patchTarget, ReadOnlySpan<byte> data, Span<byte> backup)
		{
			int length = data.Length;
			OSX.vm_prot_t vm_prot_t;
			OSX.vm_prot_t vm_prot_t2;
			bool flag;
			bool flag2;
			bool flag4;
			bool flag5;
			if (MacOSSystem.TryGetProtForMem(patchTarget, length, out vm_prot_t, out vm_prot_t2, out flag, out flag2))
			{
				if (flag)
				{
					bool flag3;
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler debugLogWarningStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler(101, 2, out flag3);
					if (flag3)
					{
						debugLogWarningStringHandler.AppendLiteral("Patch requested for memory which spans multiple memory allocations. Failures may result. (0x");
						debugLogWarningStringHandler.AppendFormatted<IntPtr>(patchTarget, "x16");
						debugLogWarningStringHandler.AppendLiteral(" length ");
						debugLogWarningStringHandler.AppendFormatted<int>(length);
						debugLogWarningStringHandler.AppendLiteral(")");
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Warning(ref debugLogWarningStringHandler);
				}
				flag4 = vm_prot_t2.Has(OSX.vm_prot_t.Write);
				flag5 = vm_prot_t2.Has(OSX.vm_prot_t.Execute);
			}
			else
			{
				if (flag2)
				{
					bool flag3;
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(68, 2, out flag3);
					if (flag3)
					{
						debugLogErrorStringHandler.AppendLiteral("Requested patch of region which was not fully allocated (0x");
						debugLogErrorStringHandler.AppendFormatted<IntPtr>(patchTarget, "x16");
						debugLogErrorStringHandler.AppendLiteral(" length ");
						debugLogErrorStringHandler.AppendFormatted<int>(length);
						debugLogErrorStringHandler.AppendLiteral(")");
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler);
					throw new InvalidOperationException("Cannot patch unallocated region");
				}
				flag4 = false;
				flag5 = targetKind == PatchTargetKind.Executable;
			}
			if (!flag4)
			{
				Helpers.Assert(!flag, null, "!crossesBoundary");
				MacOSSystem.MakePageWritable(patchTarget);
			}
			Span<byte> span = new Span<byte>((void*)patchTarget, data.Length);
			span.TryCopyTo(backup);
			MacOSSystem.JitMemcpyHelper jitMemcpyHelper = this.NativeExceptionHelper as MacOSSystem.JitMemcpyHelper;
			if (jitMemcpyHelper != null && vm_prot_t2 == OSX.vm_prot_t.All)
			{
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace("RWX memory detected, doing memcpy for MAP_JIT");
				fixed (byte* pinnableReference = data.GetPinnableReference())
				{
					byte* ptr = pinnableReference;
					jitMemcpyHelper.JitMemCpy(patchTarget, (IntPtr)((void*)ptr), (ulong)((long)data.Length));
					bool flag3;
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(20, 2, out flag3);
					if (flag3)
					{
						debugLogTraceStringHandler.AppendFormatted<int>(data.Length);
						debugLogTraceStringHandler.AppendLiteral(" bytes written to 0x");
						debugLogTraceStringHandler.AppendFormatted<IntPtr>(patchTarget, "X16");
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler);
				}
			}
			else
			{
				data.CopyTo(span);
			}
			if (flag5)
			{
				OSX.sys_icache_invalidate((void*)patchTarget, (UIntPtr)((IntPtr)data.Length));
			}
		}

		// Token: 0x06001D61 RID: 7521 RVA: 0x0005E468 File Offset: 0x0005C668
		private unsafe static void MakePageWritable([NativeInteger] IntPtr addrInPage)
		{
			IntPtr intPtr;
			IntPtr intPtr2;
			OSX.vm_prot_t vm_prot_t;
			OSX.vm_prot_t vm_prot_t2;
			Helpers.Assert(MacOSSystem.GetLocalRegionInfo(addrInPage, out intPtr, out intPtr2, out vm_prot_t, out vm_prot_t2), null, "GetLocalRegionInfo(addrInPage, out var allocStart, out var allocSize, out var allocProt, out var allocMaxProt)");
			Helpers.Assert(intPtr <= addrInPage, null, "allocStart <= addrInPage");
			if (vm_prot_t.Has(OSX.vm_prot_t.Write))
			{
				return;
			}
			int num = OSX.mach_task_self();
			OSX.kern_return_t kern_return_t;
			bool flag;
			if (vm_prot_t2.Has(OSX.vm_prot_t.Write))
			{
				kern_return_t = OSX.mach_vm_protect(num, (ulong)((long)intPtr), (ulong)((long)intPtr2), false, vm_prot_t | OSX.vm_prot_t.Write);
				if (kern_return_t)
				{
					return;
				}
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(60, 6, out flag);
				if (flag)
				{
					debugLogErrorStringHandler.AppendLiteral("Could not vm_protect page 0x");
					debugLogErrorStringHandler.AppendFormatted<IntPtr>(intPtr, "x16");
					debugLogErrorStringHandler.AppendLiteral("+0x");
					debugLogErrorStringHandler.AppendFormatted<IntPtr>(intPtr2, "x");
					debugLogErrorStringHandler.AppendLiteral(" ");
					debugLogErrorStringHandler.AppendLiteral("from ");
					debugLogErrorStringHandler.AppendFormatted<OSX.VmProtFmtProxy>(OSX.P(vm_prot_t));
					debugLogErrorStringHandler.AppendLiteral(" to ");
					debugLogErrorStringHandler.AppendFormatted<OSX.VmProtFmtProxy>(OSX.P(vm_prot_t | OSX.vm_prot_t.Write));
					debugLogErrorStringHandler.AppendLiteral(" (max prot ");
					debugLogErrorStringHandler.AppendFormatted<OSX.VmProtFmtProxy>(OSX.P(vm_prot_t2));
					debugLogErrorStringHandler.AppendLiteral("): kr = ");
					debugLogErrorStringHandler.AppendFormatted<int>(kern_return_t.Value);
				}
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler);
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error("Trying copy/remap instead...");
			}
			if (!vm_prot_t.Has(OSX.vm_prot_t.Read))
			{
				if (!vm_prot_t2.Has(OSX.vm_prot_t.Read))
				{
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler2 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(66, 3, out flag);
					if (flag)
					{
						debugLogErrorStringHandler2.AppendLiteral("Requested 0x");
						debugLogErrorStringHandler2.AppendFormatted<IntPtr>(intPtr, "x16");
						debugLogErrorStringHandler2.AppendLiteral("+0x");
						debugLogErrorStringHandler2.AppendFormatted<IntPtr>(intPtr2, "x");
						debugLogErrorStringHandler2.AppendLiteral(" (max: ");
						debugLogErrorStringHandler2.AppendFormatted<OSX.VmProtFmtProxy>(OSX.P(vm_prot_t2));
						debugLogErrorStringHandler2.AppendLiteral(") to be made writable, but its not readable!");
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler2);
					throw new NotSupportedException("Cannot make page writable because its not readable");
				}
				kern_return_t = OSX.mach_vm_protect(num, (ulong)((long)intPtr), (ulong)((long)intPtr2), false, vm_prot_t | OSX.vm_prot_t.Read);
				if (!kern_return_t)
				{
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler3 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(60, 4, out flag);
					if (flag)
					{
						debugLogErrorStringHandler3.AppendLiteral("vm_protect of 0x");
						debugLogErrorStringHandler3.AppendFormatted<IntPtr>(intPtr, "x16");
						debugLogErrorStringHandler3.AppendLiteral("+0x");
						debugLogErrorStringHandler3.AppendFormatted<IntPtr>(intPtr2, "x");
						debugLogErrorStringHandler3.AppendLiteral(" (max: ");
						debugLogErrorStringHandler3.AppendFormatted<OSX.VmProtFmtProxy>(OSX.P(vm_prot_t2));
						debugLogErrorStringHandler3.AppendLiteral(") to become readable failed: kr = ");
						debugLogErrorStringHandler3.AppendFormatted<int>(kern_return_t.Value);
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler3);
					throw new NotSupportedException("Could not make page readable for remap");
				}
			}
			<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(41, 5, out flag);
			if (flag)
			{
				debugLogTraceStringHandler.AppendLiteral("Performing page remap on 0x");
				debugLogTraceStringHandler.AppendFormatted<IntPtr>(intPtr, "x16");
				debugLogTraceStringHandler.AppendLiteral("+0x");
				debugLogTraceStringHandler.AppendFormatted<IntPtr>(intPtr2, "x");
				debugLogTraceStringHandler.AppendLiteral(" from ");
				debugLogTraceStringHandler.AppendFormatted<OSX.VmProtFmtProxy>(OSX.P(vm_prot_t));
				debugLogTraceStringHandler.AppendLiteral("/");
				debugLogTraceStringHandler.AppendFormatted<OSX.VmProtFmtProxy>(OSX.P(vm_prot_t2));
				debugLogTraceStringHandler.AppendLiteral(" to ");
				debugLogTraceStringHandler.AppendFormatted<OSX.VmProtFmtProxy>(OSX.P(vm_prot_t | OSX.vm_prot_t.Write));
			}
			<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler);
			OSX.vm_prot_t vm_prot_t3 = vm_prot_t | OSX.vm_prot_t.Write;
			OSX.vm_prot_t vm_prot_t4 = vm_prot_t2 | OSX.vm_prot_t.Write;
			ulong num2;
			kern_return_t = OSX.mach_vm_map(num, &num2, (ulong)((long)intPtr2), 0UL, OSX.vm_flags.Anywhere, 0, 0UL, true, vm_prot_t3, vm_prot_t4, OSX.vm_inherit_t.Copy);
			if (!kern_return_t)
			{
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler4 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(36, 1, out flag);
				if (flag)
				{
					debugLogErrorStringHandler4.AppendLiteral("Could not allocate new memory! kr = ");
					debugLogErrorStringHandler4.AppendFormatted<int>(kern_return_t.Value);
				}
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler4);
				throw new OutOfMemoryException();
			}
			try
			{
				new Span<byte>(intPtr, (int)intPtr2).CopyTo(new Span<byte>(num2, (int)intPtr2));
				ulong num3 = (ulong)((long)intPtr2);
				int num4;
				kern_return_t = OSX.mach_make_memory_entry_64(num, &num3, num2, vm_prot_t4, &num4, 0);
				if (!kern_return_t)
				{
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler5 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(79, 4, out flag);
					if (flag)
					{
						debugLogErrorStringHandler5.AppendLiteral("make_memory_entry(task_self(), size: 0x");
						debugLogErrorStringHandler5.AppendFormatted<ulong>(num3, "x");
						debugLogErrorStringHandler5.AppendLiteral(", addr: ");
						debugLogErrorStringHandler5.AppendFormatted<ulong>(num2, "x16");
						debugLogErrorStringHandler5.AppendLiteral(", prot: ");
						debugLogErrorStringHandler5.AppendFormatted<OSX.VmProtFmtProxy>(OSX.P(vm_prot_t4));
						debugLogErrorStringHandler5.AppendLiteral(", &obj, 0) failed: kr = ");
						debugLogErrorStringHandler5.AppendFormatted<int>(kern_return_t.Value);
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler5);
					throw new NotSupportedException("make_memory_entry() failed");
				}
				ulong num5 = (ulong)((long)intPtr);
				kern_return_t = OSX.mach_vm_map(num, &num5, (ulong)((long)intPtr2), 0UL, OSX.vm_flags.Overwrite, num4, 0UL, true, vm_prot_t3, vm_prot_t4, OSX.vm_inherit_t.Copy);
				if (!kern_return_t)
				{
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler6 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(78, 10, out flag);
					if (flag)
					{
						debugLogErrorStringHandler6.AppendLiteral("vm_map() failed to map over target range: 0x");
						debugLogErrorStringHandler6.AppendFormatted<ulong>(num5, "x16");
						debugLogErrorStringHandler6.AppendLiteral("+0x");
						debugLogErrorStringHandler6.AppendFormatted<IntPtr>(intPtr2, "x");
						debugLogErrorStringHandler6.AppendLiteral(" (");
						debugLogErrorStringHandler6.AppendFormatted<OSX.VmProtFmtProxy>(OSX.P(vm_prot_t));
						debugLogErrorStringHandler6.AppendLiteral("/");
						debugLogErrorStringHandler6.AppendFormatted<OSX.VmProtFmtProxy>(OSX.P(vm_prot_t2));
						debugLogErrorStringHandler6.AppendLiteral(")");
						debugLogErrorStringHandler6.AppendLiteral(" <- (obj ");
						debugLogErrorStringHandler6.AppendFormatted<int>(num4);
						debugLogErrorStringHandler6.AppendLiteral(") 0x");
						debugLogErrorStringHandler6.AppendFormatted<ulong>(num2, "x16");
						debugLogErrorStringHandler6.AppendLiteral("+0x");
						debugLogErrorStringHandler6.AppendFormatted<IntPtr>(intPtr2, "x");
						debugLogErrorStringHandler6.AppendLiteral(" (");
						debugLogErrorStringHandler6.AppendFormatted<OSX.VmProtFmtProxy>(OSX.P(vm_prot_t3));
						debugLogErrorStringHandler6.AppendLiteral("/");
						debugLogErrorStringHandler6.AppendFormatted<OSX.VmProtFmtProxy>(OSX.P(vm_prot_t4));
						debugLogErrorStringHandler6.AppendLiteral("), kr = ");
						debugLogErrorStringHandler6.AppendFormatted<int>(kern_return_t.Value);
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler6);
					throw new NotSupportedException("vm_map() failed");
				}
			}
			finally
			{
				kern_return_t = OSX.mach_vm_deallocate(num, num2, (ulong)((long)intPtr2));
				if (!kern_return_t)
				{
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler7 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(53, 3, out flag);
					if (flag)
					{
						debugLogErrorStringHandler7.AppendLiteral("Could not deallocate created memory page 0x");
						debugLogErrorStringHandler7.AppendFormatted<ulong>(num2, "x16");
						debugLogErrorStringHandler7.AppendLiteral("+0x");
						debugLogErrorStringHandler7.AppendFormatted<IntPtr>(intPtr2, "x");
						debugLogErrorStringHandler7.AppendLiteral("! kr = ");
						debugLogErrorStringHandler7.AppendFormatted<int>(kern_return_t.Value);
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler7);
				}
			}
		}

		// Token: 0x06001D62 RID: 7522 RVA: 0x0005EAAC File Offset: 0x0005CCAC
		private static bool TryGetProtForMem([NativeInteger] IntPtr addr, int length, out OSX.vm_prot_t maxProt, out OSX.vm_prot_t prot, out bool crossesAllocBoundary, out bool notAllocated)
		{
			maxProt = (OSX.vm_prot_t)(-1);
			prot = (OSX.vm_prot_t)(-1);
			crossesAllocBoundary = false;
			notAllocated = false;
			IntPtr intPtr = addr;
			while (addr < intPtr + (IntPtr)length)
			{
				IntPtr intPtr2;
				IntPtr intPtr3;
				OSX.vm_prot_t vm_prot_t;
				OSX.vm_prot_t vm_prot_t2;
				OSX.kern_return_t localRegionInfo = MacOSSystem.GetLocalRegionInfo(addr, out intPtr2, out intPtr3, out vm_prot_t, out vm_prot_t2);
				if (localRegionInfo)
				{
					if (intPtr2 > addr)
					{
						notAllocated = true;
						return false;
					}
					prot &= vm_prot_t;
					maxProt &= vm_prot_t2;
					addr = intPtr2 + intPtr3;
					if (addr >= intPtr + (IntPtr)length)
					{
						break;
					}
					crossesAllocBoundary = true;
				}
				else
				{
					if (localRegionInfo == OSX.kern_return_t.NoSpace)
					{
						notAllocated = true;
						return false;
					}
					return false;
				}
			}
			return true;
		}

		// Token: 0x06001D63 RID: 7523 RVA: 0x0005EB28 File Offset: 0x0005CD28
		private unsafe static OSX.kern_return_t GetLocalRegionInfo([NativeInteger] IntPtr origAddr, [NativeInteger] out IntPtr startAddr, [NativeInteger] out IntPtr outSize, out OSX.vm_prot_t prot, out OSX.vm_prot_t maxProt)
		{
			int maxValue = int.MaxValue;
			int count = OSX.vm_region_submap_short_info_64.Count;
			ulong num = (ulong)((long)origAddr);
			ulong num2;
			OSX.vm_region_submap_short_info_64 vm_region_submap_short_info_;
			OSX.kern_return_t kern_return_t = OSX.mach_vm_region_recurse(OSX.mach_task_self(), &num, &num2, &maxValue, &vm_region_submap_short_info_, &count);
			if (!kern_return_t)
			{
				startAddr = (IntPtr)0;
				outSize = (IntPtr)0;
				prot = OSX.vm_prot_t.None;
				maxProt = OSX.vm_prot_t.None;
				return kern_return_t;
			}
			Helpers.Assert(!vm_region_submap_short_info_.is_submap, null, "!info.is_submap");
			startAddr = (IntPtr)num;
			outSize = (IntPtr)num2;
			prot = vm_region_submap_short_info_.protection;
			maxProt = vm_region_submap_short_info_.max_protection;
			return kern_return_t;
		}

		// Token: 0x17000660 RID: 1632
		// (get) Token: 0x06001D64 RID: 7524 RVA: 0x0005EBAD File Offset: 0x0005CDAD
		[Nullable(1)]
		public IMemoryAllocator MemoryAllocator
		{
			[NullableContext(1)]
			get;
		} = new QueryingPagedMemoryAllocator(new MacOSSystem.MacOsQueryingAllocator());

		// Token: 0x06001D65 RID: 7525 RVA: 0x0005EBB5 File Offset: 0x0005CDB5
		[NullableContext(1)]
		void IInitialize<IArchitecture>.Initialize(IArchitecture value)
		{
			this.arch = value;
		}

		// Token: 0x17000661 RID: 1633
		// (get) Token: 0x06001D66 RID: 7526 RVA: 0x0005EBC0 File Offset: 0x0005CDC0
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

		// Token: 0x06001D67 RID: 7527 RVA: 0x0005EBE8 File Offset: 0x0005CDE8
		public IntPtr GetNativeJitHookConfig(int runtimeMajMin)
		{
			MacOSSystem.JitMemcpyHelper jitMemcpyHelper = this.NativeExceptionHelper as MacOSSystem.JitMemcpyHelper;
			if (jitMemcpyHelper != null)
			{
				return jitMemcpyHelper.GetJitHookConfig(runtimeMajMin);
			}
			return IntPtr.Zero;
		}

		// Token: 0x17000662 RID: 1634
		// (get) Token: 0x06001D68 RID: 7528 RVA: 0x0005EC11 File Offset: 0x0005CE11
		private unsafe static ReadOnlySpan<byte> NEHTempl
		{
			get
			{
				return new ReadOnlySpan<byte>((void*)(&<24b3ba8a-00b7-40fc-a603-2711fa115297><PrivateImplementationDetails>.D2092BB8F0B39D69931977DCBF7F6A48B0209B37F74004EAB47FA529ECBA7672), 29);
			}
		}

		// Token: 0x06001D69 RID: 7529 RVA: 0x0005EC20 File Offset: 0x0005CE20
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
				text = "exhelper_macos_arm64.dylib";
			}
			else
			{
				text = "exhelper_macos_x86_64.dylib";
			}
			string text2 = text;
			string text3;
			using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(text2))
			{
				Helpers.Assert(manifestResourceStream != null, null, "embedded is not null");
				text3 = MacOSSystem.MacOSNativeLibDrop.Instance.DropLibrary(manifestResourceStream, MacOSSystem.NEHTempl);
			}
			if (this.arch.Target != ArchitectureKind.Arm64)
			{
				return PosixExceptionHelper.CreateHelper(this.arch, text3);
			}
			return MacOSSystem.JitMemcpyHelper.CreateHelper(this.arch, text3);
		}

		// Token: 0x0400121A RID: 4634
		[Nullable(2)]
		private IArchitecture arch;

		// Token: 0x0400121B RID: 4635
		[Nullable(2)]
		private PosixExceptionHelper lazyNativeExceptionHelper;

		// Token: 0x0200051D RID: 1309
		private sealed class MacOsQueryingAllocator : QueryingMemoryPageAllocatorBase
		{
			// Token: 0x17000663 RID: 1635
			// (get) Token: 0x06001D6A RID: 7530 RVA: 0x0005ECEC File Offset: 0x0005CEEC
			public override uint PageSize { get; }

			// Token: 0x06001D6B RID: 7531 RVA: 0x0005ECF4 File Offset: 0x0005CEF4
			public MacOsQueryingAllocator()
			{
				this.PageSize = (uint)OSX.GetPageSize();
			}

			// Token: 0x06001D6C RID: 7532 RVA: 0x0005ED08 File Offset: 0x0005CF08
			public unsafe override bool TryAllocatePage([NativeInteger] IntPtr size, bool executable, out IntPtr allocated)
			{
				Helpers.Assert((long)size == (long)((ulong)this.PageSize), null, "size == PageSize");
				OSX.vm_prot_t vm_prot_t = (executable ? OSX.vm_prot_t.Execute : OSX.vm_prot_t.None);
				vm_prot_t |= OSX.vm_prot_t.Default;
				if (PlatformDetection.Architecture == ArchitectureKind.Arm64 && vm_prot_t == OSX.vm_prot_t.All)
				{
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace("RWX memory detected, doing mmap with MAP_JIT");
					allocated = OSX.mmap(IntPtr.Zero, (ulong)((long)size), OSX.map_prot.Read | OSX.map_prot.Write | OSX.map_prot.Execute, OSX.map_flags.Private | OSX.map_flags.JIT | OSX.map_flags.Anonymous, -1, 0L);
					bool flag;
					if (allocated == (IntPtr)(-1))
					{
						int errno = OSX.Errno;
						Win32Exception ex = new Win32Exception(errno);
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(37, 2, out flag);
						if (flag)
						{
							debugLogErrorStringHandler.AppendLiteral("Error creating allocation anywhere! ");
							debugLogErrorStringHandler.AppendFormatted<int>(errno);
							debugLogErrorStringHandler.AppendLiteral(" ");
							debugLogErrorStringHandler.AppendFormatted<Win32Exception>(ex);
						}
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler);
						allocated = 0;
						return false;
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(37, 2, out flag);
					if (flag)
					{
						debugLogTraceStringHandler.AppendLiteral("RWX memory allocated to 0x");
						debugLogTraceStringHandler.AppendFormatted<IntPtr>(allocated, "X16");
						debugLogTraceStringHandler.AppendLiteral(" with size ");
						debugLogTraceStringHandler.AppendFormatted<IntPtr>(size);
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler);
					return true;
				}
				else
				{
					ulong num = 0UL;
					OSX.kern_return_t kern_return_t = OSX.mach_vm_map(OSX.mach_task_self(), &num, (ulong)((long)size), 0UL, OSX.vm_flags.Anywhere, 0, 0UL, true, vm_prot_t, vm_prot_t, OSX.vm_inherit_t.Copy);
					if (!kern_return_t)
					{
						bool flag;
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler2 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(41, 1, out flag);
						if (flag)
						{
							debugLogErrorStringHandler2.AppendLiteral("Error creating allocation anywhere! kr = ");
							debugLogErrorStringHandler2.AppendFormatted<int>(kern_return_t.Value);
						}
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler2);
						allocated = 0;
						return false;
					}
					allocated = (IntPtr)((long)num);
					return true;
				}
			}

			// Token: 0x06001D6D RID: 7533 RVA: 0x0005EE84 File Offset: 0x0005D084
			public unsafe override bool TryAllocatePage(IntPtr pageAddr, [NativeInteger] IntPtr size, bool executable, out IntPtr allocated)
			{
				Helpers.Assert((long)size == (long)((ulong)this.PageSize), null, "size == PageSize");
				OSX.vm_prot_t vm_prot_t = (executable ? OSX.vm_prot_t.Execute : OSX.vm_prot_t.None);
				vm_prot_t |= OSX.vm_prot_t.Default;
				if (PlatformDetection.Architecture == ArchitectureKind.Arm64 && vm_prot_t == OSX.vm_prot_t.All)
				{
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace("RWX memory detected, doing mmap with MAP_JIT");
					allocated = OSX.mmap(pageAddr, (ulong)((long)size), OSX.map_prot.Read | OSX.map_prot.Write | OSX.map_prot.Execute, OSX.map_flags.Private | OSX.map_flags.Fixed | OSX.map_flags.JIT | OSX.map_flags.Anonymous, -1, 0L);
					bool flag;
					if (allocated == (IntPtr)(-1))
					{
						int errno = OSX.Errno;
						Win32Exception ex = new Win32Exception(errno);
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(37, 2, out flag);
						if (flag)
						{
							debugLogErrorStringHandler.AppendLiteral("Error creating allocation anywhere! ");
							debugLogErrorStringHandler.AppendFormatted<int>(errno);
							debugLogErrorStringHandler.AppendLiteral(" ");
							debugLogErrorStringHandler.AppendFormatted<Win32Exception>(ex);
						}
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler);
						allocated = 0;
						return false;
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(45, 2, out flag);
					if (flag)
					{
						debugLogTraceStringHandler.AppendLiteral("RWX memory allocated to page at 0x");
						debugLogTraceStringHandler.AppendFormatted<IntPtr>(pageAddr, "X16");
						debugLogTraceStringHandler.AppendLiteral(" with size ");
						debugLogTraceStringHandler.AppendFormatted<IntPtr>(size);
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler);
					return true;
				}
				else
				{
					ulong num = (ulong)(long)pageAddr;
					OSX.kern_return_t kern_return_t = OSX.mach_vm_map(OSX.mach_task_self(), &num, (ulong)((long)size), 0UL, OSX.vm_flags.Fixed, 0, 0UL, true, vm_prot_t, vm_prot_t, OSX.vm_inherit_t.Copy);
					if (!kern_return_t)
					{
						bool flag;
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogSpamStringHandler debugLogSpamStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogSpamStringHandler(38, 2, out flag);
						if (flag)
						{
							debugLogSpamStringHandler.AppendLiteral("Error creating allocation at 0x");
							debugLogSpamStringHandler.AppendFormatted<ulong>(num, "x16");
							debugLogSpamStringHandler.AppendLiteral(": kr = ");
							debugLogSpamStringHandler.AppendFormatted<int>(kern_return_t.Value);
						}
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Spam(ref debugLogSpamStringHandler);
						allocated = 0;
						return false;
					}
					allocated = (IntPtr)((long)num);
					return true;
				}
			}

			// Token: 0x06001D6E RID: 7534 RVA: 0x0005F020 File Offset: 0x0005D220
			[NullableContext(2)]
			public override bool TryFreePage(IntPtr pageAddr, [<24b3ba8a-00b7-40fc-a603-2711fa115297>NotNullWhen(false)] out string errorMsg)
			{
				OSX.kern_return_t kern_return_t = OSX.mach_vm_deallocate(OSX.mach_task_self(), (ulong)(long)pageAddr, (ulong)this.PageSize);
				if (!kern_return_t)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Could not deallocate page: kr = ");
					defaultInterpolatedStringHandler.AppendFormatted<int>(kern_return_t.Value);
					errorMsg = defaultInterpolatedStringHandler.ToStringAndClear();
					return false;
				}
				errorMsg = null;
				return true;
			}

			// Token: 0x06001D6F RID: 7535 RVA: 0x0005F080 File Offset: 0x0005D280
			public override bool TryQueryPage(IntPtr pageAddr, out bool isFree, out IntPtr allocBase, [NativeInteger] out IntPtr allocSize)
			{
				OSX.vm_prot_t vm_prot_t;
				OSX.vm_prot_t vm_prot_t2;
				OSX.kern_return_t localRegionInfo = MacOSSystem.GetLocalRegionInfo(pageAddr, out allocBase, out allocSize, out vm_prot_t, out vm_prot_t2);
				if (localRegionInfo)
				{
					if (allocBase > pageAddr)
					{
						allocSize = allocBase - pageAddr;
						allocBase = pageAddr;
						isFree = true;
						return true;
					}
					isFree = false;
					return true;
				}
				else
				{
					if (localRegionInfo == OSX.kern_return_t.InvalidAddress)
					{
						isFree = true;
						return true;
					}
					isFree = false;
					return false;
				}
			}
		}

		// Token: 0x0200051E RID: 1310
		private sealed class MacOSNativeLibDrop : PosixNativeLibraryDrop
		{
			// Token: 0x06001D70 RID: 7536 RVA: 0x0005F0D2 File Offset: 0x0005D2D2
			protected override void CloseFileDescriptor([NativeInteger] IntPtr fd)
			{
				OSX.Close((int)fd);
			}

			// Token: 0x06001D71 RID: 7537 RVA: 0x0005F0DC File Offset: 0x0005D2DC
			[return: NativeInteger]
			protected unsafe override IntPtr Mkstemp(Span<byte> template)
			{
				int num;
				fixed (byte* pinnableReference = template.GetPinnableReference())
				{
					num = OSX.MkSTemp(pinnableReference);
				}
				if (num == -1)
				{
					int errno = OSX.Errno;
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

			// Token: 0x0400121D RID: 4637
			[Nullable(1)]
			public static readonly MacOSSystem.MacOSNativeLibDrop Instance = new MacOSSystem.MacOSNativeLibDrop();
		}

		// Token: 0x0200051F RID: 1311
		[NullableContext(1)]
		[Nullable(0)]
		private sealed class JitMemcpyHelper : PosixExceptionHelper
		{
			// Token: 0x06001D74 RID: 7540 RVA: 0x0005F159 File Offset: 0x0005D359
			private JitMemcpyHelper(IArchitecture arch, IntPtr getExPtr, IntPtr m2n, IntPtr n2m, IntPtr memcpy, IntPtr jitCfg)
				: base(arch, getExPtr, m2n, n2m)
			{
				this.mmch_jit_memcpy = memcpy;
				this.mmch_jit_hook_config = jitCfg;
			}

			// Token: 0x06001D75 RID: 7541 RVA: 0x0005F178 File Offset: 0x0005D378
			public new static MacOSSystem.JitMemcpyHelper CreateHelper(IArchitecture arch, string filename)
			{
				IntPtr intPtr = DynDll.OpenLibrary(filename);
				IntPtr export;
				IntPtr export2;
				IntPtr export3;
				IntPtr export4;
				IntPtr export5;
				try
				{
					export = intPtr.GetExport("eh_get_exception_ptr");
					export2 = intPtr.GetExport("eh_managed_to_native");
					export3 = intPtr.GetExport("eh_native_to_managed");
					export4 = intPtr.GetExport("mmch_jit_memcpy");
					export5 = intPtr.GetExport("mmch_jit_hook_config");
					Helpers.Assert(export != IntPtr.Zero, null, "eh_get_exception_ptr != IntPtr.Zero");
					Helpers.Assert(export2 != IntPtr.Zero, null, "eh_managed_to_native != IntPtr.Zero");
					Helpers.Assert(export3 != IntPtr.Zero, null, "eh_native_to_managed != IntPtr.Zero");
					Helpers.Assert(export3 != IntPtr.Zero, null, "eh_native_to_managed != IntPtr.Zero");
					Helpers.Assert(export4 != IntPtr.Zero, null, "mmch_jit_memcpy != IntPtr.Zero");
					Helpers.Assert(export5 != IntPtr.Zero, null, "mmch_jit_hook_config != IntPtr.Zero");
				}
				catch
				{
					DynDll.CloseLibrary(intPtr);
					throw;
				}
				return new MacOSSystem.JitMemcpyHelper(arch, export, export2, export3, export4, export5);
			}

			// Token: 0x06001D76 RID: 7542 RVA: 0x0005F278 File Offset: 0x0005D478
			public unsafe void JitMemCpy(IntPtr dst, IntPtr src, ulong size)
			{
				delegate* unmanaged[Cdecl]<IntPtr, IntPtr, ulong, void> system.Void_u0020(System.IntPtr,System.IntPtr,System.UInt64) = (void*)this.mmch_jit_memcpy;
				delegate* unmanaged[Cdecl]<IntPtr, IntPtr, ulong, void> system.Void_u0020(System.IntPtr,System.IntPtr,System.UInt64)2 = system.Void_u0020(System.IntPtr,System.IntPtr,System.UInt64);
				calli(System.Void(System.IntPtr,System.IntPtr,System.UInt64), dst, src, size, system.Void_u0020(System.IntPtr,System.IntPtr,System.UInt64)2);
			}

			// Token: 0x06001D77 RID: 7543 RVA: 0x0005F29C File Offset: 0x0005D49C
			internal unsafe IntPtr GetJitHookConfig(int runtimeMajMin)
			{
				delegate* unmanaged[Cdecl]<int, IntPtr> system.IntPtr_u0020(System.Int32) = (void*)this.mmch_jit_hook_config;
				delegate* unmanaged[Cdecl]<int, IntPtr> system.IntPtr_u0020(System.Int32)2 = system.IntPtr_u0020(System.Int32);
				return calli(System.IntPtr(System.Int32), runtimeMajMin, system.IntPtr_u0020(System.Int32)2);
			}

			// Token: 0x0400121E RID: 4638
			private readonly IntPtr mmch_jit_memcpy;

			// Token: 0x0400121F RID: 4639
			private readonly IntPtr mmch_jit_hook_config;
		}

		// Token: 0x02000520 RID: 1312
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04001220 RID: 4640
			public static Classifier <0>__ClassifyAMD64;

			// Token: 0x04001221 RID: 4641
			public static Classifier <1>__ClassifyARM64;
		}
	}
}
