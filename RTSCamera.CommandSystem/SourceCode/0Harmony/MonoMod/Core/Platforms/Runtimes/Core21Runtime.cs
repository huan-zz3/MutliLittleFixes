using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mono.Cecil;
using MonoMod.Core.Interop;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Runtimes
{
	// Token: 0x0200052A RID: 1322
	[NullableContext(1)]
	[Nullable(0)]
	internal class Core21Runtime : CoreBaseRuntime
	{
		// Token: 0x17000671 RID: 1649
		// (get) Token: 0x06001DAF RID: 7599 RVA: 0x0006024C File Offset: 0x0005E44C
		public override RuntimeFeature Features
		{
			get
			{
				return base.Features | RuntimeFeature.CompileMethodHook;
			}
		}

		// Token: 0x06001DB0 RID: 7600 RVA: 0x00060256 File Offset: 0x0005E456
		public Core21Runtime(ISystem system)
			: base(system)
		{
		}

		// Token: 0x06001DB1 RID: 7601 RVA: 0x0006026A File Offset: 0x0005E46A
		private static Core21Runtime.JitHookHelpersHolder CreateJitHookHelpers(Core21Runtime self)
		{
			return new Core21Runtime.JitHookHelpersHolder(self);
		}

		// Token: 0x17000672 RID: 1650
		// (get) Token: 0x06001DB2 RID: 7602 RVA: 0x00060272 File Offset: 0x0005E472
		protected Core21Runtime.JitHookHelpersHolder JitHookHelpers
		{
			get
			{
				return Helpers.GetOrInitWithLock<Core21Runtime, Core21Runtime.JitHookHelpersHolder>(ref this.lazyJitHookHelpers, this.sync, Core21Runtime.createJitHookHelpersFunc, this);
			}
		}

		// Token: 0x17000673 RID: 1651
		// (get) Token: 0x06001DB3 RID: 7603 RVA: 0x0006028B File Offset: 0x0005E48B
		protected virtual Guid ExpectedJitVersion
		{
			get
			{
				return Core21Runtime.JitVersionGuid;
			}
		}

		// Token: 0x17000674 RID: 1652
		// (get) Token: 0x06001DB4 RID: 7604 RVA: 0x000411A5 File Offset: 0x0003F3A5
		protected virtual int VtableIndexICorJitCompilerGetVersionGuid
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x17000675 RID: 1653
		// (get) Token: 0x06001DB5 RID: 7605 RVA: 0x0001B69F File Offset: 0x0001989F
		protected virtual int VtableIndexICorJitCompilerCompileMethod
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000676 RID: 1654
		// (get) Token: 0x06001DB6 RID: 7606 RVA: 0x00060292 File Offset: 0x0005E492
		protected virtual CoreCLR.InvokeCompileMethodPtr InvokeCompileMethodPtr
		{
			get
			{
				return CoreCLR.V21.InvokeCompileMethodPtr;
			}
		}

		// Token: 0x06001DB7 RID: 7607 RVA: 0x00060299 File Offset: 0x0005E499
		protected virtual Delegate CastCompileHookToRealType(Delegate del)
		{
			return del.CastDelegate<CoreCLR.V21.CompileMethodDelegate>();
		}

		// Token: 0x06001DB8 RID: 7608 RVA: 0x000602A1 File Offset: 0x0005E4A1
		[NullableContext(0)]
		protected unsafe static IntPtr* GetVTableEntry(IntPtr @object, int index)
		{
			return *(IntPtr*)(void*)@object / (IntPtr)sizeof(IntPtr) + index * sizeof(IntPtr);
		}

		// Token: 0x06001DB9 RID: 7609 RVA: 0x000602B4 File Offset: 0x0005E4B4
		protected unsafe static IntPtr ReadObjectVTable(IntPtr @object, int index)
		{
			return *Core21Runtime.GetVTableEntry(@object, index);
		}

		// Token: 0x06001DBA RID: 7610 RVA: 0x000602C0 File Offset: 0x0005E4C0
		protected unsafe void CheckVersionGuid(IntPtr jit)
		{
			delegate* unmanaged[Thiscall]<IntPtr, Guid*, void> system.Void_u0020(System.IntPtr,System.Guid*) = (void*)Core21Runtime.ReadObjectVTable(jit, this.VtableIndexICorJitCompilerGetVersionGuid);
			delegate* unmanaged[Thiscall]<IntPtr, Guid*, void> system.Void_u0020(System.IntPtr,System.Guid*)2 = system.Void_u0020(System.IntPtr,System.Guid*);
			Guid guid;
			calli(System.Void(System.IntPtr,System.Guid*), jit, &guid, system.Void_u0020(System.IntPtr,System.Guid*)2);
			bool flag = guid == this.ExpectedJitVersion;
			bool flag2 = flag;
			bool flag3;
			AssertionInterpolatedStringHandler assertionInterpolatedStringHandler = new AssertionInterpolatedStringHandler(66, 2, flag, out flag3);
			if (flag3)
			{
				assertionInterpolatedStringHandler.AppendLiteral("JIT version does not match expected JIT version! ");
				assertionInterpolatedStringHandler.AppendLiteral("expected: ");
				assertionInterpolatedStringHandler.AppendFormatted<Guid>(this.ExpectedJitVersion);
				assertionInterpolatedStringHandler.AppendLiteral(", got: ");
				assertionInterpolatedStringHandler.AppendFormatted<Guid>(guid);
			}
			Helpers.Assert(flag2, ref assertionInterpolatedStringHandler, "guid == ExpectedJitVersion");
		}

		// Token: 0x06001DBB RID: 7611 RVA: 0x00060350 File Offset: 0x0005E550
		protected unsafe override void InstallManagedJitHook(IntPtr jit)
		{
			this.CheckVersionGuid(jit);
			IntPtr* vtableEntry = Core21Runtime.GetVTableEntry(jit, this.VtableIndexICorJitCompilerCompileMethod);
			IntPtr intPtr = base.EHManagedToNative(*vtableEntry, out this.m2nHookHelper);
			Delegate @delegate = this.CastCompileHookToRealType(this.CreateCompileMethodDelegate(intPtr));
			this.ourCompileMethod = @delegate;
			IntPtr intPtr2 = base.EHNativeToManaged(Marshal.GetFunctionPointerForDelegate(@delegate), out this.n2mHookHelper);
			this.InvokeCompileMethodToPrepare(intPtr2);
			int num = sizeof(IntPtr);
			Span<byte> span = new Span<byte>(stackalloc byte[(UIntPtr)num], num);
			MemoryMarshal.Write<IntPtr>(span, ref intPtr2);
			base.System.PatchData(PatchTargetKind.ReadOnly, (IntPtr)((void*)vtableEntry), span, default(Span<byte>));
		}

		// Token: 0x06001DBC RID: 7612 RVA: 0x000603F4 File Offset: 0x0005E5F4
		protected unsafe virtual void InvokeCompileMethodToPrepare(IntPtr method)
		{
			delegate*<IntPtr, IntPtr, IntPtr, CoreCLR.V21.CORINFO_METHOD_INFO*, uint, byte**, uint*, CoreCLR.CorJitResult> invokeCompileMethod = this.InvokeCompileMethodPtr.InvokeCompileMethod;
			CoreCLR.V21.CORINFO_METHOD_INFO corinfo_METHOD_INFO;
			byte* ptr;
			uint num;
			CoreCLR.CorJitResult corJitResult = calli(MonoMod.Core.Interop.CoreCLR/CorJitResult(System.IntPtr,System.IntPtr,System.IntPtr,MonoMod.Core.Interop.CoreCLR/V21/CORINFO_METHOD_INFO*,System.UInt32,System.Byte**,System.UInt32*), method, IntPtr.Zero, IntPtr.Zero, &corinfo_METHOD_INFO, 0U, &ptr, &num, invokeCompileMethod);
		}

		// Token: 0x06001DBD RID: 7613 RVA: 0x0006042E File Offset: 0x0005E62E
		protected virtual Delegate CreateCompileMethodDelegate(IntPtr compileMethod)
		{
			return new <>f__AnonymousDelegate0(new Core21Runtime.JitHookDelegateHolder(this, this.InvokeCompileMethodPtr, compileMethod).CompileMethodHook);
		}

		// Token: 0x06001DBE RID: 7614 RVA: 0x00060448 File Offset: 0x0005E648
		protected virtual MethodInfo MakeCreateRuntimeMethodInfoStub(Type methodHandleInternal)
		{
			Type[] array = new Type[]
			{
				typeof(IntPtr),
				typeof(object)
			};
			Type type = typeof(RuntimeMethodHandle).Assembly.GetType("System.RuntimeMethodInfoStub");
			ConstructorInfo constructor = type.GetConstructor(array);
			MethodInfo methodInfo;
			using (DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition("new RuntimeMethodInfoStub", type, array))
			{
				ILGenerator ilgenerator = dynamicMethodDefinition.GetILGenerator();
				ilgenerator.Emit(OpCodes.Ldarg_0);
				ilgenerator.Emit(OpCodes.Ldarg_1);
				ilgenerator.Emit(OpCodes.Newobj, constructor);
				ilgenerator.Emit(OpCodes.Ret);
				methodInfo = dynamicMethodDefinition.Generate();
			}
			return methodInfo;
		}

		// Token: 0x06001DBF RID: 7615 RVA: 0x00060500 File Offset: 0x0005E700
		protected virtual MethodInfo GetOrCreateGetTypeFromHandleUnsafe()
		{
			MethodInfo method = typeof(Type).GetMethod("GetTypeFromHandleUnsafe", (BindingFlags)(-1));
			if (method != null)
			{
				return method;
			}
			Assembly assembly;
			using (ModuleDefinition moduleDefinition = ModuleDefinition.CreateModule("MonoMod.Core.Platforms.Runtimes.Core21Runtime+Helpers", new ModuleParameters
			{
				Kind = ModuleKind.Dll
			}))
			{
				TypeDefinition typeDefinition = new TypeDefinition("System", "Type", Mono.Cecil.TypeAttributes.Abstract)
				{
					BaseType = moduleDefinition.TypeSystem.Object
				};
				moduleDefinition.Types.Add(typeDefinition);
				MethodDefinition methodDefinition = new MethodDefinition("GetTypeFromHandleUnsafe", Mono.Cecil.MethodAttributes.FamANDAssem | Mono.Cecil.MethodAttributes.Family | Mono.Cecil.MethodAttributes.Static, moduleDefinition.ImportReference(typeof(Type)))
				{
					IsInternalCall = true
				};
				methodDefinition.Parameters.Add(new ParameterDefinition(moduleDefinition.ImportReference(typeof(IntPtr))));
				typeDefinition.Methods.Add(methodDefinition);
				assembly = ReflectionHelper.Load(moduleDefinition);
			}
			this.MakeAssemblySystemAssembly(assembly);
			return assembly.GetType("System.Type").GetMethod("GetTypeFromHandleUnsafe", (BindingFlags)(-1));
		}

		// Token: 0x06001DC0 RID: 7616 RVA: 0x00060608 File Offset: 0x0005E808
		protected unsafe virtual void MakeAssemblySystemAssembly(Assembly assembly)
		{
			IntPtr intPtr = (IntPtr)Core21Runtime.RuntimeAssemblyPtrField.GetValue(assembly);
			int num = IntPtr.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size + 4 + IntPtr.Size + IntPtr.Size + 4 + 4 + IntPtr.Size + IntPtr.Size + 4 + 4 + IntPtr.Size;
			if (IntPtr.Size == 8)
			{
				num += 4;
			}
			IntPtr intPtr2 = *(IntPtr*)((byte*)(void*)intPtr + num);
			int num2 = IntPtr.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size;
			IntPtr intPtr3 = *(IntPtr*)((byte*)(void*)intPtr2 + num2);
			int num3 = IntPtr.Size + (FxCoreBaseRuntime.IsDebugClr ? (IntPtr.Size + 4 + 4 + 4 + IntPtr.Size + 4) : 0) + IntPtr.Size + IntPtr.Size + 4 + 4 + IntPtr.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size + 4;
			if (FxCoreBaseRuntime.IsDebugClr && IntPtr.Size == 8)
			{
				num3 += 8;
			}
			int* ptr = (int*)((byte*)(void*)intPtr3 + num3);
			*ptr |= 1;
		}

		// Token: 0x04001233 RID: 4659
		private static readonly Func<Core21Runtime, Core21Runtime.JitHookHelpersHolder> createJitHookHelpersFunc = new Func<Core21Runtime, Core21Runtime.JitHookHelpersHolder>(Core21Runtime.CreateJitHookHelpers);

		// Token: 0x04001234 RID: 4660
		private readonly object sync = new object();

		// Token: 0x04001235 RID: 4661
		[Nullable(2)]
		private Core21Runtime.JitHookHelpersHolder lazyJitHookHelpers;

		// Token: 0x04001236 RID: 4662
		private static readonly Guid JitVersionGuid = new Guid(195102408U, 33184, 16511, 153, 161, 146, 132, 72, 193, 235, 98);

		// Token: 0x04001237 RID: 4663
		[Nullable(2)]
		private Delegate ourCompileMethod;

		// Token: 0x04001238 RID: 4664
		[Nullable(2)]
		private IDisposable n2mHookHelper;

		// Token: 0x04001239 RID: 4665
		[Nullable(2)]
		private IDisposable m2nHookHelper;

		// Token: 0x0400123A RID: 4666
		private protected static readonly FieldInfo RuntimeAssemblyPtrField = Type.GetType("System.Reflection.RuntimeAssembly").GetField("m_assembly", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x0200052B RID: 1323
		[Nullable(0)]
		private sealed class JitHookDelegateHolder
		{
			// Token: 0x06001DC2 RID: 7618 RVA: 0x00060784 File Offset: 0x0005E984
			public unsafe JitHookDelegateHolder(Core21Runtime runtime, CoreCLR.InvokeCompileMethodPtr icmp, IntPtr compileMethod)
			{
				this.Runtime = runtime;
				this.NativeExceptionHelper = runtime.NativeExceptionHelper;
				this.JitHookHelpers = runtime.JitHookHelpers;
				this.InvokeCompileMethodPtr = icmp;
				this.CompileMethodPtr = compileMethod;
				delegate*<IntPtr, IntPtr, IntPtr, CoreCLR.V21.CORINFO_METHOD_INFO*, uint, byte**, uint*, CoreCLR.CorJitResult> invokeCompileMethod = icmp.InvokeCompileMethod;
				CoreCLR.V21.CORINFO_METHOD_INFO corinfo_METHOD_INFO;
				byte* ptr;
				uint num;
				CoreCLR.CorJitResult corJitResult = calli(MonoMod.Core.Interop.CoreCLR/CorJitResult(System.IntPtr,System.IntPtr,System.IntPtr,MonoMod.Core.Interop.CoreCLR/V21/CORINFO_METHOD_INFO*,System.UInt32,System.Byte**,System.UInt32*), IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, &corinfo_METHOD_INFO, 0U, &ptr, &num, invokeCompileMethod);
				MarshalEx.SetLastPInvokeError(MarshalEx.GetLastPInvokeError());
				INativeExceptionHelper nativeExceptionHelper = this.NativeExceptionHelper;
				if (nativeExceptionHelper != null)
				{
					this.GetNativeExceptionSlot = nativeExceptionHelper.GetExceptionSlot;
					this.GetNativeExceptionSlot();
				}
				int num2 = Core21Runtime.JitHookDelegateHolder.hookEntrancy;
				Core21Runtime.JitHookDelegateHolder.hookEntrancy = 0;
			}

			// Token: 0x06001DC3 RID: 7619 RVA: 0x00060828 File Offset: 0x0005EA28
			[NullableContext(0)]
			public unsafe CoreCLR.CorJitResult CompileMethodHook(IntPtr jit, IntPtr corJitInfo, CoreCLR.V21.CORINFO_METHOD_INFO* methodInfo, uint flags, byte** pNativeEntry, uint* pNativeSizeOfCode)
			{
				if (jit == IntPtr.Zero)
				{
					return CoreCLR.CorJitResult.CORJIT_OK;
				}
				*(IntPtr*)pNativeEntry = (IntPtr)((UIntPtr)0);
				*pNativeSizeOfCode = 0U;
				int lastPInvokeError = MarshalEx.GetLastPInvokeError();
				IntPtr intPtr = (IntPtr)0;
				GetExceptionSlot getNativeExceptionSlot = this.GetNativeExceptionSlot;
				IntPtr* ptr = ((getNativeExceptionSlot != null) ? getNativeExceptionSlot() : null);
				Core21Runtime.JitHookDelegateHolder.hookEntrancy++;
				CoreCLR.CorJitResult corJitResult2;
				try
				{
					delegate*<IntPtr, IntPtr, IntPtr, CoreCLR.V21.CORINFO_METHOD_INFO*, uint, byte**, uint*, CoreCLR.CorJitResult> invokeCompileMethod = this.InvokeCompileMethodPtr.InvokeCompileMethod;
					CoreCLR.CorJitResult corJitResult = calli(MonoMod.Core.Interop.CoreCLR/CorJitResult(System.IntPtr,System.IntPtr,System.IntPtr,MonoMod.Core.Interop.CoreCLR/V21/CORINFO_METHOD_INFO*,System.UInt32,System.Byte**,System.UInt32*), this.CompileMethodPtr, jit, corJitInfo, methodInfo, flags, pNativeEntry, pNativeSizeOfCode, invokeCompileMethod);
					if (ptr != null && (intPtr = *ptr) != 0)
					{
						bool flag;
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler debugLogWarningStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler(59, 1, out flag);
						if (flag)
						{
							debugLogWarningStringHandler.AppendLiteral("Native exception caught in JIT by exception helper (ex: 0x");
							debugLogWarningStringHandler.AppendFormatted<IntPtr>(intPtr, "x16");
							debugLogWarningStringHandler.AppendLiteral(")");
						}
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Warning(ref debugLogWarningStringHandler);
						corJitResult2 = corJitResult;
					}
					else
					{
						if (Core21Runtime.JitHookDelegateHolder.hookEntrancy == 1)
						{
							try
							{
								RuntimeTypeHandle[] array = null;
								RuntimeTypeHandle[] array2 = null;
								if (methodInfo->args.sigInst.classInst != null)
								{
									array = new RuntimeTypeHandle[methodInfo->args.sigInst.classInstCount];
									for (int i = 0; i < array.Length; i++)
									{
										array[i] = this.JitHookHelpers.GetTypeFromNativeHandle(methodInfo->args.sigInst.classInst[(IntPtr)i * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)]).TypeHandle;
									}
								}
								if (methodInfo->args.sigInst.methInst != null)
								{
									array2 = new RuntimeTypeHandle[methodInfo->args.sigInst.methInstCount];
									for (int j = 0; j < array2.Length; j++)
									{
										array2[j] = this.JitHookHelpers.GetTypeFromNativeHandle(methodInfo->args.sigInst.methInst[(IntPtr)j * (IntPtr)sizeof(IntPtr) / (IntPtr)sizeof(IntPtr)]).TypeHandle;
									}
								}
								RuntimeTypeHandle typeHandle = this.JitHookHelpers.GetDeclaringTypeOfMethodHandle(methodInfo->ftn).TypeHandle;
								RuntimeMethodHandle runtimeMethodHandle = this.JitHookHelpers.CreateHandleForHandlePointer(methodInfo->ftn);
								this.Runtime.OnMethodCompiledCore(typeHandle, runtimeMethodHandle, new ReadOnlyMemory<RuntimeTypeHandle>?(array), new ReadOnlyMemory<RuntimeTypeHandle>?(array2), (IntPtr)(*(IntPtr*)pNativeEntry), (IntPtr)(*(IntPtr*)pNativeEntry), (ulong)(*pNativeSizeOfCode));
							}
							catch
							{
							}
						}
						corJitResult2 = corJitResult;
					}
				}
				finally
				{
					Core21Runtime.JitHookDelegateHolder.hookEntrancy--;
					if (ptr != null)
					{
						*ptr = intPtr;
					}
					MarshalEx.SetLastPInvokeError(lastPInvokeError);
				}
				return corJitResult2;
			}

			// Token: 0x0400123B RID: 4667
			public readonly Core21Runtime Runtime;

			// Token: 0x0400123C RID: 4668
			[Nullable(2)]
			public readonly INativeExceptionHelper NativeExceptionHelper;

			// Token: 0x0400123D RID: 4669
			[Nullable(2)]
			public readonly GetExceptionSlot GetNativeExceptionSlot;

			// Token: 0x0400123E RID: 4670
			public readonly Core21Runtime.JitHookHelpersHolder JitHookHelpers;

			// Token: 0x0400123F RID: 4671
			public readonly CoreCLR.InvokeCompileMethodPtr InvokeCompileMethodPtr;

			// Token: 0x04001240 RID: 4672
			public readonly IntPtr CompileMethodPtr;

			// Token: 0x04001241 RID: 4673
			[ThreadStatic]
			private static int hookEntrancy;
		}

		// Token: 0x0200052C RID: 1324
		[NullableContext(0)]
		protected sealed class JitHookHelpersHolder
		{
			// Token: 0x06001DC4 RID: 7620 RVA: 0x00060AB4 File Offset: 0x0005ECB4
			public RuntimeMethodHandle CreateHandleForHandlePointer(IntPtr handle)
			{
				return this.CreateRuntimeMethodHandle(this.CreateRuntimeMethodInfoStub(handle, this.MethodHandle_GetLoaderAllocator(handle)));
			}

			// Token: 0x06001DC5 RID: 7621 RVA: 0x00060ADC File Offset: 0x0005ECDC
			[NullableContext(1)]
			public JitHookHelpersHolder(Core21Runtime runtime)
			{
				MethodInfo method = typeof(RuntimeMethodHandle).GetMethod("GetLoaderAllocator", BindingFlags.Static | BindingFlags.NonPublic);
				MethodInfo methodInfo;
				using (DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition("MethodHandle_GetLoaderAllocator", typeof(object), new Type[] { typeof(IntPtr) }))
				{
					ILGenerator ilgenerator = dynamicMethodDefinition.GetILGenerator();
					Type parameterType = method.GetParameters().First<ParameterInfo>().ParameterType;
					ilgenerator.Emit(OpCodes.Ldarga_S, 0);
					ilgenerator.Emit(OpCodes.Ldobj, parameterType);
					ilgenerator.Emit(OpCodes.Call, method);
					ilgenerator.Emit(OpCodes.Ret);
					methodInfo = dynamicMethodDefinition.Generate();
				}
				this.MethodHandle_GetLoaderAllocator = methodInfo.CreateDelegate<Core21Runtime.JitHookHelpersHolder.MethodHandle_GetLoaderAllocatorD>();
				MethodInfo orCreateGetTypeFromHandleUnsafe = runtime.GetOrCreateGetTypeFromHandleUnsafe();
				this.GetTypeFromNativeHandle = orCreateGetTypeFromHandleUnsafe.CreateDelegate<Core21Runtime.JitHookHelpersHolder.GetTypeFromNativeHandleD>();
				Type type = typeof(RuntimeMethodHandle).Assembly.GetType("System.RuntimeMethodHandleInternal");
				MethodInfo method2 = typeof(RuntimeMethodHandle).GetMethod("GetDeclaringType", BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] { type }, null);
				MethodInfo methodInfo2;
				using (DynamicMethodDefinition dynamicMethodDefinition2 = new DynamicMethodDefinition("GetDeclaringTypeOfMethodHandle", typeof(Type), new Type[] { typeof(IntPtr) }))
				{
					ILGenerator ilgenerator2 = dynamicMethodDefinition2.GetILGenerator();
					ilgenerator2.Emit(OpCodes.Ldarga_S, 0);
					ilgenerator2.Emit(OpCodes.Ldobj, type);
					ilgenerator2.Emit(OpCodes.Call, method2);
					ilgenerator2.Emit(OpCodes.Ret);
					methodInfo2 = dynamicMethodDefinition2.Generate();
				}
				this.GetDeclaringTypeOfMethodHandle = methodInfo2.CreateDelegate<Core21Runtime.JitHookHelpersHolder.GetDeclaringTypeOfMethodHandleD>();
				this.CreateRuntimeMethodInfoStub = runtime.MakeCreateRuntimeMethodInfoStub(type).CreateDelegate<Core21Runtime.JitHookHelpersHolder.CreateRuntimeMethodInfoStubD>();
				ConstructorInfo constructorInfo = typeof(RuntimeMethodHandle).GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).First<ConstructorInfo>();
				MethodInfo methodInfo3;
				using (DynamicMethodDefinition dynamicMethodDefinition3 = new DynamicMethodDefinition("new RuntimeMethodHandle", typeof(RuntimeMethodHandle), new Type[] { typeof(object) }))
				{
					ILGenerator ilgenerator3 = dynamicMethodDefinition3.GetILGenerator();
					ilgenerator3.Emit(OpCodes.Ldarg_0);
					ilgenerator3.Emit(OpCodes.Newobj, constructorInfo);
					ilgenerator3.Emit(OpCodes.Ret);
					methodInfo3 = dynamicMethodDefinition3.Generate();
				}
				this.CreateRuntimeMethodHandle = methodInfo3.CreateDelegate<Core21Runtime.JitHookHelpersHolder.CreateRuntimeMethodHandleD>();
			}

			// Token: 0x04001242 RID: 4674
			[Nullable(1)]
			public readonly Core21Runtime.JitHookHelpersHolder.MethodHandle_GetLoaderAllocatorD MethodHandle_GetLoaderAllocator;

			// Token: 0x04001243 RID: 4675
			[Nullable(1)]
			public readonly Core21Runtime.JitHookHelpersHolder.CreateRuntimeMethodInfoStubD CreateRuntimeMethodInfoStub;

			// Token: 0x04001244 RID: 4676
			[Nullable(1)]
			public readonly Core21Runtime.JitHookHelpersHolder.CreateRuntimeMethodHandleD CreateRuntimeMethodHandle;

			// Token: 0x04001245 RID: 4677
			[Nullable(1)]
			public readonly Core21Runtime.JitHookHelpersHolder.GetDeclaringTypeOfMethodHandleD GetDeclaringTypeOfMethodHandle;

			// Token: 0x04001246 RID: 4678
			[Nullable(1)]
			public readonly Core21Runtime.JitHookHelpersHolder.GetTypeFromNativeHandleD GetTypeFromNativeHandle;

			// Token: 0x0200052D RID: 1325
			// (Invoke) Token: 0x06001DC7 RID: 7623
			public delegate object MethodHandle_GetLoaderAllocatorD(IntPtr methodHandle);

			// Token: 0x0200052E RID: 1326
			// (Invoke) Token: 0x06001DCB RID: 7627
			public delegate object CreateRuntimeMethodInfoStubD(IntPtr methodHandle, object loaderAllocator);

			// Token: 0x0200052F RID: 1327
			// (Invoke) Token: 0x06001DCF RID: 7631
			public delegate RuntimeMethodHandle CreateRuntimeMethodHandleD(object runtimeMethodInfo);

			// Token: 0x02000530 RID: 1328
			// (Invoke) Token: 0x06001DD3 RID: 7635
			public delegate Type GetDeclaringTypeOfMethodHandleD(IntPtr methodHandle);

			// Token: 0x02000531 RID: 1329
			// (Invoke) Token: 0x06001DD7 RID: 7639
			public delegate Type GetTypeFromNativeHandleD(IntPtr handle);
		}
	}
}
