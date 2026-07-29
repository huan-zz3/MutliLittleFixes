using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MonoMod.Utils.Cil
{
	// Token: 0x020008FC RID: 2300
	[NullableContext(1)]
	[Nullable(0)]
	internal abstract class ILGeneratorShim
	{
		// Token: 0x17000867 RID: 2151
		// (get) Token: 0x0600304E RID: 12366
		public abstract int ILOffset { get; }

		// Token: 0x0600304F RID: 12367
		public abstract void BeginCatchBlock(Type exceptionType);

		// Token: 0x06003050 RID: 12368
		public abstract void BeginExceptFilterBlock();

		// Token: 0x06003051 RID: 12369
		public abstract Label BeginExceptionBlock();

		// Token: 0x06003052 RID: 12370
		public abstract void BeginFaultBlock();

		// Token: 0x06003053 RID: 12371
		public abstract void BeginFinallyBlock();

		// Token: 0x06003054 RID: 12372
		public abstract void BeginScope();

		// Token: 0x06003055 RID: 12373
		public abstract LocalBuilder DeclareLocal(Type localType);

		// Token: 0x06003056 RID: 12374
		public abstract LocalBuilder DeclareLocal(Type localType, bool pinned);

		// Token: 0x06003057 RID: 12375
		public abstract Label DefineLabel();

		// Token: 0x06003058 RID: 12376
		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode);

		// Token: 0x06003059 RID: 12377
		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, byte arg);

		// Token: 0x0600305A RID: 12378
		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, double arg);

		// Token: 0x0600305B RID: 12379
		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, short arg);

		// Token: 0x0600305C RID: 12380
		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, int arg);

		// Token: 0x0600305D RID: 12381
		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, long arg);

		// Token: 0x0600305E RID: 12382
		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, ConstructorInfo con);

		// Token: 0x0600305F RID: 12383
		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, Label label);

		// Token: 0x06003060 RID: 12384
		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, Label[] labels);

		// Token: 0x06003061 RID: 12385
		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, LocalBuilder local);

		// Token: 0x06003062 RID: 12386
		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, SignatureHelper signature);

		// Token: 0x06003063 RID: 12387
		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, FieldInfo field);

		// Token: 0x06003064 RID: 12388
		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, MethodInfo meth);

		// Token: 0x06003065 RID: 12389
		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, sbyte arg);

		// Token: 0x06003066 RID: 12390
		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, float arg);

		// Token: 0x06003067 RID: 12391
		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, string str);

		// Token: 0x06003068 RID: 12392
		public abstract void Emit(global::System.Reflection.Emit.OpCode opcode, Type cls);

		// Token: 0x06003069 RID: 12393
		public abstract void EmitCall(global::System.Reflection.Emit.OpCode opcode, MethodInfo methodInfo, [Nullable(new byte[] { 2, 1 })] Type[] optionalParameterTypes);

		// Token: 0x0600306A RID: 12394
		[NullableContext(2)]
		public abstract void EmitCalli(global::System.Reflection.Emit.OpCode opcode, CallingConventions callingConvention, Type returnType, [Nullable(new byte[] { 2, 1 })] Type[] parameterTypes, [Nullable(new byte[] { 2, 1 })] Type[] optionalParameterTypes);

		// Token: 0x0600306B RID: 12395
		[NullableContext(2)]
		public abstract void EmitCalli(global::System.Reflection.Emit.OpCode opcode, CallingConvention unmanagedCallConv, Type returnType, [Nullable(new byte[] { 2, 1 })] Type[] parameterTypes);

		// Token: 0x0600306C RID: 12396
		public abstract void EmitWriteLine(LocalBuilder localBuilder);

		// Token: 0x0600306D RID: 12397
		public abstract void EmitWriteLine(FieldInfo fld);

		// Token: 0x0600306E RID: 12398
		public abstract void EmitWriteLine(string value);

		// Token: 0x0600306F RID: 12399
		public abstract void EndExceptionBlock();

		// Token: 0x06003070 RID: 12400
		public abstract void EndScope();

		// Token: 0x06003071 RID: 12401
		public abstract void MarkLabel(Label loc);

		// Token: 0x06003072 RID: 12402
		public abstract void ThrowException(Type excType);

		// Token: 0x06003073 RID: 12403
		public abstract void UsingNamespace(string usingNamespace);

		// Token: 0x06003074 RID: 12404 RVA: 0x000A6C9F File Offset: 0x000A4E9F
		public ILGenerator GetProxy()
		{
			return (ILGenerator)ILGeneratorShim.ILGeneratorBuilder.GenerateProxy().MakeGenericType(new Type[] { base.GetType() }).GetConstructors()[0].Invoke(new object[] { this });
		}

		// Token: 0x06003075 RID: 12405 RVA: 0x000A6CD5 File Offset: 0x000A4ED5
		public static Type GetProxyType<[Nullable(0)] TShim>() where TShim : ILGeneratorShim
		{
			return ILGeneratorShim.GetProxyType(typeof(TShim));
		}

		// Token: 0x06003076 RID: 12406 RVA: 0x000A6CE6 File Offset: 0x000A4EE6
		public static Type GetProxyType(Type tShim)
		{
			return ILGeneratorShim.GenericProxyType.MakeGenericType(new Type[] { tShim });
		}

		// Token: 0x17000868 RID: 2152
		// (get) Token: 0x06003077 RID: 12407 RVA: 0x000A6CFC File Offset: 0x000A4EFC
		public static Type GenericProxyType
		{
			get
			{
				return ILGeneratorShim.ILGeneratorBuilder.GenerateProxy();
			}
		}

		// Token: 0x020008FD RID: 2301
		[Nullable(0)]
		internal static class ILGeneratorBuilder
		{
			// Token: 0x06003079 RID: 12409 RVA: 0x000A6D04 File Offset: 0x000A4F04
			public static Type GenerateProxy()
			{
				if (ILGeneratorShim.ILGeneratorBuilder.ProxyType != null)
				{
					return ILGeneratorShim.ILGeneratorBuilder.ProxyType;
				}
				Type typeFromHandle = typeof(ILGenerator);
				Type typeFromHandle2 = typeof(ILGeneratorShim);
				Assembly assembly;
				using (ModuleDefinition moduleDefinition = ModuleDefinition.CreateModule("MonoMod.Utils.Cil.ILGeneratorProxy", new ModuleParameters
				{
					Kind = ModuleKind.Dll,
					ReflectionImporterProvider = MMReflectionImporter.Provider
				}))
				{
					CustomAttribute customAttribute = new CustomAttribute(moduleDefinition.ImportReference(DynamicMethodDefinition.c_IgnoresAccessChecksToAttribute));
					customAttribute.ConstructorArguments.Add(new CustomAttributeArgument(moduleDefinition.TypeSystem.String, typeof(ILGeneratorShim).Assembly.GetName().Name));
					moduleDefinition.Assembly.CustomAttributes.Add(customAttribute);
					TypeDefinition typeDefinition = new TypeDefinition("MonoMod.Utils.Cil", "ILGeneratorProxy", Mono.Cecil.TypeAttributes.Public)
					{
						BaseType = moduleDefinition.ImportReference(typeFromHandle)
					};
					moduleDefinition.Types.Add(typeDefinition);
					TypeReference typeReference = moduleDefinition.ImportReference(typeFromHandle2);
					GenericParameter genericParameter = new GenericParameter("TTarget", typeDefinition);
					genericParameter.Constraints.Add(new GenericParameterConstraint(typeReference));
					typeDefinition.GenericParameters.Add(genericParameter);
					FieldDefinition fieldDefinition = new FieldDefinition("Target", Mono.Cecil.FieldAttributes.Public, genericParameter);
					typeDefinition.Fields.Add(fieldDefinition);
					FieldReference fieldReference = new FieldReference("Target", genericParameter, new GenericInstanceType(typeDefinition)
					{
						GenericArguments = { genericParameter }
					});
					MethodDefinition methodDefinition = new MethodDefinition(".ctor", Mono.Cecil.MethodAttributes.FamANDAssem | Mono.Cecil.MethodAttributes.Family | Mono.Cecil.MethodAttributes.HideBySig | Mono.Cecil.MethodAttributes.SpecialName | Mono.Cecil.MethodAttributes.RTSpecialName, moduleDefinition.TypeSystem.Void);
					methodDefinition.Parameters.Add(new ParameterDefinition(genericParameter));
					typeDefinition.Methods.Add(methodDefinition);
					ILProcessor ilprocessor = methodDefinition.Body.GetILProcessor();
					ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_0);
					ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_1);
					ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Stfld, fieldReference);
					ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ret);
					foreach (MethodInfo methodInfo in typeFromHandle.GetMethods(BindingFlags.Instance | BindingFlags.Public))
					{
						MethodInfo method = typeFromHandle2.GetMethod(methodInfo.Name, (from p in methodInfo.GetParameters()
							select p.ParameterType).ToArray<Type>());
						if (!(method == null))
						{
							MethodDefinition methodDefinition2 = new MethodDefinition(methodInfo.Name, Mono.Cecil.MethodAttributes.FamANDAssem | Mono.Cecil.MethodAttributes.Family | Mono.Cecil.MethodAttributes.Virtual | Mono.Cecil.MethodAttributes.HideBySig, moduleDefinition.ImportReference(methodInfo.ReturnType))
							{
								HasThis = true
							};
							foreach (ParameterInfo parameterInfo in methodInfo.GetParameters())
							{
								methodDefinition2.Parameters.Add(new ParameterDefinition(moduleDefinition.ImportReference(parameterInfo.ParameterType)));
							}
							typeDefinition.Methods.Add(methodDefinition2);
							ilprocessor = methodDefinition2.Body.GetILProcessor();
							ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_0);
							ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ldfld, fieldReference);
							foreach (ParameterDefinition parameterDefinition in methodDefinition2.Parameters)
							{
								ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ldarg, parameterDefinition);
							}
							ilprocessor.Emit(method.IsVirtual ? Mono.Cecil.Cil.OpCodes.Callvirt : Mono.Cecil.Cil.OpCodes.Call, ilprocessor.Body.Method.Module.ImportReference(method));
							ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ret);
						}
					}
					assembly = ReflectionHelper.Load(moduleDefinition);
					assembly.SetMonoCorlibInternal(true);
				}
				ResolveEventHandler resolveEventHandler = delegate(object asmSender, ResolveEventArgs asmArgs)
				{
					if (new AssemblyName(asmArgs.Name).Name == typeof(ILGeneratorShim.ILGeneratorBuilder).Assembly.GetName().Name)
					{
						return typeof(ILGeneratorShim.ILGeneratorBuilder).Assembly;
					}
					return null;
				};
				AppDomain.CurrentDomain.AssemblyResolve += resolveEventHandler;
				try
				{
					ILGeneratorShim.ILGeneratorBuilder.ProxyType = assembly.GetType("MonoMod.Utils.Cil.ILGeneratorProxy");
				}
				finally
				{
					AppDomain.CurrentDomain.AssemblyResolve -= resolveEventHandler;
				}
				if (ILGeneratorShim.ILGeneratorBuilder.ProxyType == null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append("Couldn't find ILGeneratorShim proxy \"").Append("MonoMod.Utils.Cil.ILGeneratorProxy").Append("\" in autogenerated \"")
						.Append(assembly.FullName)
						.AppendLine("\"");
					Type[] array;
					Exception[] array2;
					try
					{
						array = assembly.GetTypes();
						array2 = null;
					}
					catch (ReflectionTypeLoadException ex)
					{
						array = ex.Types;
						array2 = new Exception[ex.LoaderExceptions.Length + 1];
						array2[0] = ex;
						for (int k = 0; k < ex.LoaderExceptions.Length; k++)
						{
							array2[k + 1] = ex.LoaderExceptions[k];
						}
					}
					stringBuilder.AppendLine("Listing all types in autogenerated assembly:");
					foreach (Type type in array)
					{
						stringBuilder.AppendLine(((type != null) ? type.FullName : null) ?? "<NULL>");
					}
					if (array2 != null && array2.Length != 0)
					{
						stringBuilder.AppendLine("Listing all exceptions:");
						for (int l = 0; l < array2.Length; l++)
						{
							StringBuilder stringBuilder2 = stringBuilder.Append('#').Append(l).Append(": ");
							Exception ex2 = array2[l];
							stringBuilder2.AppendLine(((ex2 != null) ? ex2.ToString() : null) ?? "NULL");
						}
					}
					throw new InvalidOperationException(stringBuilder.ToString());
				}
				return ILGeneratorShim.ILGeneratorBuilder.ProxyType;
			}

			// Token: 0x04003C04 RID: 15364
			public const string Namespace = "MonoMod.Utils.Cil";

			// Token: 0x04003C05 RID: 15365
			public const string Name = "ILGeneratorProxy";

			// Token: 0x04003C06 RID: 15366
			public const string FullName = "MonoMod.Utils.Cil.ILGeneratorProxy";

			// Token: 0x04003C07 RID: 15367
			public const string TargetName = "Target";

			// Token: 0x04003C08 RID: 15368
			[Nullable(2)]
			private static Type ProxyType;
		}
	}
}
