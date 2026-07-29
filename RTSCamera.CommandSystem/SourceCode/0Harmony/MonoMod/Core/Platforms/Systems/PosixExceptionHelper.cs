using System;
using System.Runtime.CompilerServices;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Systems
{
	// Token: 0x02000521 RID: 1313
	[NullableContext(1)]
	[Nullable(0)]
	internal class PosixExceptionHelper : INativeExceptionHelper
	{
		// Token: 0x06001D78 RID: 7544 RVA: 0x0005F2BE File Offset: 0x0005D4BE
		protected PosixExceptionHelper(IArchitecture arch, IntPtr getExPtr, IntPtr m2n, IntPtr n2m)
		{
			this.arch = arch;
			this.eh_get_exception_ptr = getExPtr;
			this.eh_managed_to_native = m2n;
			this.eh_native_to_managed = n2m;
		}

		// Token: 0x06001D79 RID: 7545 RVA: 0x0005F2E4 File Offset: 0x0005D4E4
		public static PosixExceptionHelper CreateHelper(IArchitecture arch, string filename)
		{
			IntPtr intPtr = DynDll.OpenLibrary(filename);
			IntPtr export;
			IntPtr export2;
			IntPtr export3;
			try
			{
				export = intPtr.GetExport("eh_get_exception_ptr");
				export2 = intPtr.GetExport("eh_managed_to_native");
				export3 = intPtr.GetExport("eh_native_to_managed");
				Helpers.Assert(export != IntPtr.Zero, null, "eh_get_exception_ptr != IntPtr.Zero");
				Helpers.Assert(export2 != IntPtr.Zero, null, "eh_managed_to_native != IntPtr.Zero");
				Helpers.Assert(export3 != IntPtr.Zero, null, "eh_native_to_managed != IntPtr.Zero");
			}
			catch
			{
				DynDll.CloseLibrary(intPtr);
				throw;
			}
			return new PosixExceptionHelper(arch, export, export2, export3);
		}

		// Token: 0x17000664 RID: 1636
		// (get) Token: 0x06001D7A RID: 7546 RVA: 0x0005F384 File Offset: 0x0005D584
		// (set) Token: 0x06001D7B RID: 7547 RVA: 0x0005F397 File Offset: 0x0005D597
		public unsafe IntPtr NativeException
		{
			get
			{
				return *calli(System.IntPtr*(), (void*)this.eh_get_exception_ptr);
			}
			set
			{
				*calli(System.IntPtr*(), (void*)this.eh_get_exception_ptr) = value;
			}
		}

		// Token: 0x17000665 RID: 1637
		// (get) Token: 0x06001D7C RID: 7548 RVA: 0x0005F3AB File Offset: 0x0005D5AB
		public unsafe GetExceptionSlot GetExceptionSlot
		{
			get
			{
				return () => calli(System.IntPtr*(), (void*)this.eh_get_exception_ptr);
			}
		}

		// Token: 0x06001D7D RID: 7549 RVA: 0x0005F3BC File Offset: 0x0005D5BC
		[NullableContext(2)]
		public IntPtr CreateManagedToNativeHelper(IntPtr target, out IDisposable handle)
		{
			IAllocatedMemory allocatedMemory = this.arch.CreateSpecialEntryStub(this.eh_managed_to_native, target);
			handle = allocatedMemory;
			return allocatedMemory.BaseAddress;
		}

		// Token: 0x06001D7E RID: 7550 RVA: 0x0005F3E8 File Offset: 0x0005D5E8
		[NullableContext(2)]
		public IntPtr CreateNativeToManagedHelper(IntPtr target, out IDisposable handle)
		{
			IAllocatedMemory allocatedMemory = this.arch.CreateSpecialEntryStub(this.eh_native_to_managed, target);
			handle = allocatedMemory;
			return allocatedMemory.BaseAddress;
		}

		// Token: 0x04001222 RID: 4642
		private readonly IArchitecture arch;

		// Token: 0x04001223 RID: 4643
		private readonly IntPtr eh_get_exception_ptr;

		// Token: 0x04001224 RID: 4644
		private readonly IntPtr eh_managed_to_native;

		// Token: 0x04001225 RID: 4645
		private readonly IntPtr eh_native_to_managed;
	}
}
