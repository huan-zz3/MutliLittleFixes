using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Mono.Cecil.Cil;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Runtimes
{
	// Token: 0x02000529 RID: 1321
	[NullableContext(1)]
	[Nullable(0)]
	internal class Core100Runtime : Core90Runtime
	{
		// Token: 0x06001DA7 RID: 7591 RVA: 0x00060023 File Offset: 0x0005E223
		public Core100Runtime(ISystem system, IArchitecture arch)
			: base(system, arch)
		{
		}

		// Token: 0x1700066E RID: 1646
		// (get) Token: 0x06001DA8 RID: 7592 RVA: 0x0006002D File Offset: 0x0005E22D
		protected override Guid ExpectedJitVersion
		{
			get
			{
				return Core100Runtime.JitVersionGuid;
			}
		}

		// Token: 0x1700066F RID: 1647
		// (get) Token: 0x06001DA9 RID: 7593 RVA: 0x00060034 File Offset: 0x0005E234
		protected override int VtableIndexICorJitInfoAllocMem
		{
			get
			{
				return 160;
			}
		}

		// Token: 0x17000670 RID: 1648
		// (get) Token: 0x06001DAA RID: 7594 RVA: 0x0006003B File Offset: 0x0005E23B
		protected override int ICorJitInfoFullVtableCount
		{
			get
			{
				return 176;
			}
		}

		// Token: 0x06001DAB RID: 7595 RVA: 0x00060044 File Offset: 0x0005E244
		protected unsafe override void MakeAssemblySystemAssembly(Assembly assembly)
		{
			IntPtr intPtr = (IntPtr)Core21Runtime.RuntimeAssemblyPtrField.GetValue(assembly);
			int num = IntPtr.Size + IntPtr.Size + IntPtr.Size;
			IntPtr intPtr2 = *(IntPtr*)((byte*)(void*)intPtr + num);
			int num2 = IntPtr.Size + (FxCoreBaseRuntime.IsDebugClr ? (IntPtr.Size + 4 + 4 + 4 + IntPtr.Size + 4) : 0) + IntPtr.Size + 4 + ((IntPtr.Size == 8) ? 4 : 0) + IntPtr.Size + IntPtr.Size + IntPtr.Size + 4;
			if (FxCoreBaseRuntime.IsDebugClr && IntPtr.Size == 8)
			{
				num2 += 8;
			}
			((byte*)(void*)intPtr2)[num2] = 1;
		}

		// Token: 0x06001DAC RID: 7596 RVA: 0x000600E4 File Offset: 0x0005E2E4
		protected override MethodInfo MakeCreateRuntimeMethodInfoStub(Type methodHandleInternal)
		{
			ConstructorInfo constructorInfo = methodHandleInternal.GetConstructors((BindingFlags)(-1))[0];
			Type type = typeof(RuntimeMethodHandle).Assembly.GetType("System.RuntimeMethodInfoStub");
			ConstructorInfo constructor = type.GetConstructor(new Type[]
			{
				methodHandleInternal,
				typeof(object)
			});
			MethodInfo methodInfo;
			using (DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition("new RuntimeMethodInfoStub", type, new Type[]
			{
				typeof(IntPtr),
				typeof(object)
			}))
			{
				ILProcessor ilprocessor = dynamicMethodDefinition.GetILProcessor();
				ilprocessor.Emit(OpCodes.Ldarg_0);
				ilprocessor.Emit(OpCodes.Newobj, constructorInfo);
				ilprocessor.Emit(OpCodes.Ldarg_1);
				ilprocessor.Emit(OpCodes.Newobj, constructor);
				ilprocessor.Emit(OpCodes.Ret);
				methodInfo = dynamicMethodDefinition.Generate();
			}
			return methodInfo;
		}

		// Token: 0x06001DAD RID: 7597 RVA: 0x000601C8 File Offset: 0x0005E3C8
		protected override MethodInfo GetOrCreateGetTypeFromHandleUnsafe()
		{
			MethodInfo method = typeof(RuntimeTypeHandle).GetMethod("GetRuntimeTypeFromHandleMaybeNull", (BindingFlags)(-1), null, new Type[] { typeof(IntPtr) }, null);
			Helpers.Assert(method != null, null, "method is not null");
			return method;
		}

		// Token: 0x04001232 RID: 4658
		private static readonly Guid JitVersionGuid = new Guid(2056043606U, 40473, 17185, 128, 185, 160, 210, 197, 120, 201, 69);
	}
}
