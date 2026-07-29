using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MonoMod.Utils;

namespace HarmonyLib
{
	// Token: 0x0200002F RID: 47
	internal class MethodBodyReader
	{
		// Token: 0x060000E8 RID: 232 RVA: 0x00006168 File Offset: 0x00004368
		internal static List<ILInstruction> GetInstructions(ILGenerator generator, MethodBase method)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			MethodBodyReader methodBodyReader = new MethodBodyReader(method, generator);
			methodBodyReader.DeclareVariables(null);
			methodBodyReader.GenerateInstructions();
			return methodBodyReader.ilInstructions;
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x000061A0 File Offset: 0x000043A0
		internal MethodBodyReader(MethodBase method, ILGenerator generator)
		{
			this.generator = generator;
			this.method = method;
			this.module = method.Module;
			MethodBody methodBody = method.GetMethodBody();
			int? num;
			if (methodBody == null)
			{
				num = null;
			}
			else
			{
				byte[] ilasByteArray = methodBody.GetILAsByteArray();
				num = ((ilasByteArray != null) ? new int?(ilasByteArray.Length) : null);
			}
			int? num2 = num;
			if (num2.GetValueOrDefault() == 0)
			{
				this.ilBytes = new ByteBuffer(Array.Empty<byte>());
				this.ilInstructions = new List<ILInstruction>();
			}
			else
			{
				byte[] ilasByteArray2 = methodBody.GetILAsByteArray();
				if (ilasByteArray2 == null)
				{
					throw new ArgumentException("Can not get IL bytes of method " + method.FullDescription());
				}
				this.ilBytes = new ByteBuffer(ilasByteArray2);
				this.ilInstructions = new List<ILInstruction>((ilasByteArray2.Length + 1) / 2);
			}
			Type declaringType = method.DeclaringType;
			if (declaringType != null && declaringType.IsGenericType)
			{
				try
				{
					this.typeArguments = declaringType.GetGenericArguments();
				}
				catch
				{
					this.typeArguments = null;
				}
			}
			if (method.IsGenericMethod)
			{
				try
				{
					this.methodArguments = method.GetGenericArguments();
				}
				catch
				{
					this.methodArguments = null;
				}
			}
			if (!method.IsStatic)
			{
				this.this_parameter = new MethodBodyReader.ThisParameter(method);
			}
			this.parameters = method.GetParameters();
			List<LocalVariableInfo> list;
			if (methodBody == null)
			{
				list = null;
			}
			else
			{
				IList<LocalVariableInfo> list2 = methodBody.LocalVariables;
				list = ((list2 != null) ? list2.ToList<LocalVariableInfo>() : null);
			}
			this.localVariables = list ?? new List<LocalVariableInfo>();
			this.exceptions = ((methodBody != null) ? methodBody.ExceptionHandlingClauses : null) ?? new List<ExceptionHandlingClause>();
		}

		// Token: 0x060000EA RID: 234 RVA: 0x0000632C File Offset: 0x0000452C
		internal void SetDebugging(bool debug)
		{
			this.debug = debug;
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00006338 File Offset: 0x00004538
		internal void GenerateInstructions()
		{
			while (this.ilBytes.position < this.ilBytes.buffer.Length)
			{
				int position = this.ilBytes.position;
				ILInstruction ilinstruction = new ILInstruction(this.ReadOpCode(), null)
				{
					offset = position
				};
				this.ReadOperand(ilinstruction);
				this.ilInstructions.Add(ilinstruction);
			}
			this.HandleNativeMethod();
			this.ResolveBranches();
			this.ParseExceptions();
		}

		// Token: 0x060000EC RID: 236 RVA: 0x000063A8 File Offset: 0x000045A8
		internal void HandleNativeMethod()
		{
			MethodInfo methodInfo = this.method as MethodInfo;
			if (methodInfo == null)
			{
				return;
			}
			if (methodInfo.ReflectedType != null)
			{
				return;
			}
			DllImportAttribute dllImportAttribute = methodInfo.GetCustomAttributes(false).OfType<DllImportAttribute>().FirstOrDefault<DllImportAttribute>();
			if (dllImportAttribute == null)
			{
				return;
			}
			string[] array = (from p in methodInfo.GetParameters()
				select p.ParameterType.FullName ?? p.ParameterType.Name).ToArray<string>();
			string text = string.Join("_", array);
			string text2 = ((text.Length > 0) ? text.GetHashCode().ToString("X") : "0");
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 3);
			Type declaringType = methodInfo.DeclaringType;
			defaultInterpolatedStringHandler.AppendFormatted((((declaringType != null) ? declaringType.FullName : null) ?? "").Replace(".", "_"));
			defaultInterpolatedStringHandler.AppendLiteral("_");
			defaultInterpolatedStringHandler.AppendFormatted(methodInfo.Name);
			defaultInterpolatedStringHandler.AppendLiteral("_");
			defaultInterpolatedStringHandler.AppendFormatted(text2);
			string text3 = defaultInterpolatedStringHandler.ToStringAndClear();
			AssemblyName assemblyName = new AssemblyName(text3);
			AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);
			TypeBuilder typeBuilder = moduleBuilder.DefineType("NativeMethodHolder", TypeAttributes.Public | TypeAttributes.UnicodeClass);
			MethodBuilder methodBuilder = typeBuilder.DefinePInvokeMethod(methodInfo.Name, dllImportAttribute.Value, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Static | MethodAttributes.PinvokeImpl, CallingConventions.Standard, methodInfo.ReturnType, (from x in methodInfo.GetParameters()
				select x.ParameterType).ToArray<Type>(), dllImportAttribute.CallingConvention, dllImportAttribute.CharSet);
			methodBuilder.SetImplementationFlags(methodBuilder.GetMethodImplementationFlags() | MethodImplAttributes.PreserveSig);
			Type type = typeBuilder.CreateType();
			MethodInfo methodInfo2 = type.GetMethod(methodInfo.Name);
			int num = this.method.GetParameters().Length;
			for (int i = 0; i < num; i++)
			{
				this.ilInstructions.Add(new ILInstruction(OpCodes.Ldarg, i)
				{
					offset = 0
				});
			}
			this.ilInstructions.Add(new ILInstruction(OpCodes.Call, methodInfo2)
			{
				offset = num
			});
			this.ilInstructions.Add(new ILInstruction(OpCodes.Ret, null)
			{
				offset = num + 5
			});
		}

		// Token: 0x060000ED RID: 237 RVA: 0x000065F7 File Offset: 0x000047F7
		internal void DeclareVariables(LocalBuilder[] existingVariables)
		{
			if (this.generator == null)
			{
				return;
			}
			if (existingVariables != null)
			{
				this.variables = existingVariables;
				return;
			}
			this.variables = this.localVariables.Select<LocalVariableInfo, LocalBuilder>((LocalVariableInfo lvi) => this.generator.DeclareLocal(lvi.LocalType, lvi.IsPinned)).ToArray<LocalBuilder>();
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00006630 File Offset: 0x00004830
		private void ResolveBranches()
		{
			foreach (ILInstruction ilinstruction in this.ilInstructions)
			{
				OperandType operandType = ilinstruction.opcode.OperandType;
				if (operandType != OperandType.InlineBrTarget)
				{
					if (operandType == OperandType.InlineSwitch)
					{
						int[] array = (int[])ilinstruction.operand;
						ILInstruction[] array2 = new ILInstruction[array.Length];
						for (int i = 0; i < array.Length; i++)
						{
							array2[i] = this.GetInstruction(array[i], false);
						}
						ilinstruction.operand = array2;
						continue;
					}
					if (operandType != OperandType.ShortInlineBrTarget)
					{
						continue;
					}
				}
				ilinstruction.operand = this.GetInstruction((int)ilinstruction.operand, false);
			}
		}

		// Token: 0x060000EF RID: 239 RVA: 0x000066F4 File Offset: 0x000048F4
		private void ParseExceptions()
		{
			foreach (ExceptionHandlingClause exceptionHandlingClause in this.exceptions)
			{
				int tryOffset = exceptionHandlingClause.TryOffset;
				int handlerOffset = exceptionHandlingClause.HandlerOffset;
				int num = exceptionHandlingClause.HandlerOffset + exceptionHandlingClause.HandlerLength - 1;
				ILInstruction instruction = this.GetInstruction(tryOffset, false);
				instruction.blocks.Add(new ExceptionBlock(ExceptionBlockType.BeginExceptionBlock, null));
				ILInstruction instruction2 = this.GetInstruction(num, true);
				instruction2.blocks.Add(new ExceptionBlock(ExceptionBlockType.EndExceptionBlock, null));
				switch (exceptionHandlingClause.Flags)
				{
				case ExceptionHandlingClauseOptions.Clause:
				{
					ILInstruction instruction3 = this.GetInstruction(handlerOffset, false);
					instruction3.blocks.Add(new ExceptionBlock(ExceptionBlockType.BeginCatchBlock, exceptionHandlingClause.CatchType));
					break;
				}
				case ExceptionHandlingClauseOptions.Filter:
				{
					ILInstruction instruction4 = this.GetInstruction(exceptionHandlingClause.FilterOffset, false);
					instruction4.blocks.Add(new ExceptionBlock(ExceptionBlockType.BeginExceptFilterBlock, null));
					break;
				}
				case ExceptionHandlingClauseOptions.Finally:
				{
					ILInstruction instruction5 = this.GetInstruction(handlerOffset, false);
					instruction5.blocks.Add(new ExceptionBlock(ExceptionBlockType.BeginFinallyBlock, null));
					break;
				}
				case ExceptionHandlingClauseOptions.Fault:
				{
					ILInstruction instruction6 = this.GetInstruction(handlerOffset, false);
					instruction6.blocks.Add(new ExceptionBlock(ExceptionBlockType.BeginFaultBlock, null));
					break;
				}
				}
			}
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00006854 File Offset: 0x00004A54
		private bool EndsInDeadCode(List<CodeInstruction> list)
		{
			int count = list.Count;
			if (count < 2 || list.Last<CodeInstruction>().opcode != OpCodes.Throw)
			{
				return false;
			}
			return list.GetRange(0, count - 1).All<CodeInstruction>((CodeInstruction code) => code.opcode != OpCodes.Ret);
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x000068B4 File Offset: 0x00004AB4
		internal List<CodeInstruction> FinalizeILCodes(List<MethodInfo> transpilers, bool stripLastReturn, out bool hasReturnCode, out bool methodEndsInDeadCode, List<Label> endLabels)
		{
			hasReturnCode = false;
			methodEndsInDeadCode = false;
			if (this.generator == null)
			{
				return null;
			}
			foreach (ILInstruction ilinstruction in this.ilInstructions)
			{
				OperandType operandType = ilinstruction.opcode.OperandType;
				if (operandType != OperandType.InlineBrTarget)
				{
					if (operandType != OperandType.InlineSwitch)
					{
						if (operandType != OperandType.ShortInlineBrTarget)
						{
							continue;
						}
					}
					else
					{
						ILInstruction[] array = ilinstruction.operand as ILInstruction[];
						if (array != null)
						{
							List<Label> list = new List<Label>();
							foreach (ILInstruction ilinstruction2 in array)
							{
								Label label = this.generator.DefineLabel();
								ilinstruction2.labels.Add(label);
								list.Add(label);
							}
							ilinstruction.argument = list.ToArray();
							continue;
						}
						continue;
					}
				}
				ILInstruction ilinstruction3 = ilinstruction.operand as ILInstruction;
				if (ilinstruction3 != null)
				{
					Label label2 = this.generator.DefineLabel();
					ilinstruction3.labels.Add(label2);
					ilinstruction.argument = label2;
				}
			}
			CodeTranspiler codeTranspiler = new CodeTranspiler(this.ilInstructions);
			transpilers.Do<MethodInfo>(new Action<MethodInfo>(codeTranspiler.Add));
			List<CodeInstruction> result = codeTranspiler.GetResult(this.generator, this.method);
			hasReturnCode = result.Any<CodeInstruction>((CodeInstruction code) => code.opcode == OpCodes.Ret);
			methodEndsInDeadCode = this.EndsInDeadCode(result);
			while (stripLastReturn)
			{
				CodeInstruction codeInstruction = result.LastOrDefault<CodeInstruction>();
				if (codeInstruction == null || codeInstruction.opcode != OpCodes.Ret)
				{
					break;
				}
				if (endLabels != null)
				{
					endLabels.AddRange(codeInstruction.labels);
				}
				result.RemoveAt(result.Count - 1);
			}
			return result;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00006A88 File Offset: 0x00004C88
		private static void GetMemberInfoValue(MemberInfo info, out object result)
		{
			result = null;
			MemberTypes memberType = info.MemberType;
			if (memberType <= MemberTypes.Method)
			{
				switch (memberType)
				{
				case MemberTypes.Constructor:
					result = (ConstructorInfo)info;
					return;
				case MemberTypes.Event:
					result = (EventInfo)info;
					return;
				case MemberTypes.Constructor | MemberTypes.Event:
					break;
				case MemberTypes.Field:
					result = (FieldInfo)info;
					return;
				default:
					if (memberType != MemberTypes.Method)
					{
						return;
					}
					result = (MethodInfo)info;
					return;
				}
			}
			else if (memberType != MemberTypes.Property)
			{
				if (memberType != MemberTypes.TypeInfo && memberType != MemberTypes.NestedType)
				{
					return;
				}
				result = (Type)info;
				return;
			}
			else
			{
				result = (PropertyInfo)info;
			}
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00006B08 File Offset: 0x00004D08
		private void ReadOperand(ILInstruction instruction)
		{
			switch (instruction.opcode.OperandType)
			{
			case OperandType.InlineBrTarget:
			{
				int num = this.ilBytes.ReadInt32();
				instruction.operand = num + this.ilBytes.position;
				return;
			}
			case OperandType.InlineField:
			{
				int num2 = this.ilBytes.ReadInt32();
				instruction.operand = this.module.ResolveField(num2, this.typeArguments, this.methodArguments);
				Type declaringType = ((MemberInfo)instruction.operand).DeclaringType;
				if (declaringType != null)
				{
					declaringType.FixReflectionCacheAuto();
				}
				instruction.argument = (FieldInfo)instruction.operand;
				return;
			}
			case OperandType.InlineI:
			{
				int num3 = this.ilBytes.ReadInt32();
				instruction.operand = num3;
				instruction.argument = (int)instruction.operand;
				return;
			}
			case OperandType.InlineI8:
			{
				long num4 = this.ilBytes.ReadInt64();
				instruction.operand = num4;
				instruction.argument = (long)instruction.operand;
				return;
			}
			case OperandType.InlineMethod:
			{
				int num5 = this.ilBytes.ReadInt32();
				instruction.operand = this.module.ResolveMethod(num5, this.typeArguments, this.methodArguments);
				Type declaringType2 = ((MemberInfo)instruction.operand).DeclaringType;
				if (declaringType2 != null)
				{
					declaringType2.FixReflectionCacheAuto();
				}
				if (instruction.operand is ConstructorInfo)
				{
					instruction.argument = (ConstructorInfo)instruction.operand;
					return;
				}
				instruction.argument = (MethodInfo)instruction.operand;
				return;
			}
			case OperandType.InlineNone:
				instruction.argument = null;
				return;
			case OperandType.InlineR:
			{
				double num6 = this.ilBytes.ReadDouble();
				instruction.operand = num6;
				instruction.argument = (double)instruction.operand;
				return;
			}
			case OperandType.InlineSig:
			{
				int num7 = this.ilBytes.ReadInt32();
				byte[] array = this.module.ResolveSignature(num7);
				InlineSignature inlineSignature = InlineSignatureParser.ImportCallSite(this.module, array);
				instruction.operand = inlineSignature;
				instruction.argument = inlineSignature;
				return;
			}
			case OperandType.InlineString:
			{
				int num8 = this.ilBytes.ReadInt32();
				instruction.operand = this.module.ResolveString(num8);
				instruction.argument = (string)instruction.operand;
				return;
			}
			case OperandType.InlineSwitch:
			{
				int num9 = this.ilBytes.ReadInt32();
				int num10 = this.ilBytes.position + 4 * num9;
				int[] array2 = new int[num9];
				for (int i = 0; i < num9; i++)
				{
					array2[i] = this.ilBytes.ReadInt32() + num10;
				}
				instruction.operand = array2;
				return;
			}
			case OperandType.InlineTok:
			{
				int num11 = this.ilBytes.ReadInt32();
				instruction.operand = this.module.ResolveMember(num11, this.typeArguments, this.methodArguments);
				Type declaringType3 = ((MemberInfo)instruction.operand).DeclaringType;
				if (declaringType3 != null)
				{
					declaringType3.FixReflectionCacheAuto();
				}
				MethodBodyReader.GetMemberInfoValue((MemberInfo)instruction.operand, out instruction.argument);
				return;
			}
			case OperandType.InlineType:
			{
				int num12 = this.ilBytes.ReadInt32();
				instruction.operand = this.module.ResolveType(num12, this.typeArguments, this.methodArguments);
				((Type)instruction.operand).FixReflectionCacheAuto();
				instruction.argument = (Type)instruction.operand;
				return;
			}
			case OperandType.InlineVar:
			{
				short num13 = this.ilBytes.ReadInt16();
				if (!MethodBodyReader.TargetsLocalVariable(instruction.opcode))
				{
					instruction.operand = this.GetParameter((int)num13);
					instruction.argument = num13;
					return;
				}
				LocalVariableInfo localVariable = this.GetLocalVariable((int)num13);
				if (localVariable == null)
				{
					instruction.argument = num13;
					return;
				}
				instruction.operand = localVariable;
				LocalBuilder[] array3 = this.variables;
				instruction.argument = ((array3 != null) ? array3[localVariable.LocalIndex] : null) ?? localVariable;
				return;
			}
			case OperandType.ShortInlineBrTarget:
			{
				sbyte b = (sbyte)this.ilBytes.ReadByte();
				instruction.operand = (int)b + this.ilBytes.position;
				return;
			}
			case OperandType.ShortInlineI:
			{
				if (instruction.opcode == OpCodes.Ldc_I4_S)
				{
					sbyte b2 = (sbyte)this.ilBytes.ReadByte();
					instruction.operand = b2;
					instruction.argument = (sbyte)instruction.operand;
					return;
				}
				byte b3 = this.ilBytes.ReadByte();
				instruction.operand = b3;
				instruction.argument = (byte)instruction.operand;
				return;
			}
			case OperandType.ShortInlineR:
			{
				float num14 = this.ilBytes.ReadSingle();
				instruction.operand = num14;
				instruction.argument = (float)instruction.operand;
				return;
			}
			case OperandType.ShortInlineVar:
			{
				byte b4 = this.ilBytes.ReadByte();
				if (!MethodBodyReader.TargetsLocalVariable(instruction.opcode))
				{
					instruction.operand = this.GetParameter((int)b4);
					instruction.argument = b4;
					return;
				}
				LocalVariableInfo localVariable2 = this.GetLocalVariable((int)b4);
				if (localVariable2 == null)
				{
					instruction.argument = b4;
					return;
				}
				instruction.operand = localVariable2;
				LocalBuilder[] array4 = this.variables;
				instruction.argument = ((array4 != null) ? array4[localVariable2.LocalIndex] : null) ?? localVariable2;
				return;
			}
			}
			throw new NotSupportedException();
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x0000703C File Offset: 0x0000523C
		private ILInstruction GetInstruction(int offset, bool isEndOfInstruction)
		{
			if (offset < 0)
			{
				string text = "offset";
				object obj = offset;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Instruction offset ");
				defaultInterpolatedStringHandler.AppendFormatted<int>(offset);
				defaultInterpolatedStringHandler.AppendLiteral(" is less than 0");
				throw new ArgumentOutOfRangeException(text, obj, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			int num = this.ilInstructions.Count - 1;
			ILInstruction ilinstruction = this.ilInstructions[num];
			if (offset > ilinstruction.offset + ilinstruction.GetSize() - 1)
			{
				string text2 = "offset";
				object obj2 = offset;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(47, 2);
				defaultInterpolatedStringHandler2.AppendLiteral("Instruction offset ");
				defaultInterpolatedStringHandler2.AppendFormatted<int>(offset);
				defaultInterpolatedStringHandler2.AppendLiteral(" is outside valid range 0 - ");
				defaultInterpolatedStringHandler2.AppendFormatted<int>(ilinstruction.offset + ilinstruction.GetSize() - 1);
				throw new ArgumentOutOfRangeException(text2, obj2, defaultInterpolatedStringHandler2.ToStringAndClear());
			}
			int i = 0;
			int num2 = num;
			while (i <= num2)
			{
				int num3 = i + (num2 - i) / 2;
				ilinstruction = this.ilInstructions[num3];
				if (isEndOfInstruction)
				{
					if (offset == ilinstruction.offset + ilinstruction.GetSize() - 1)
					{
						return ilinstruction;
					}
				}
				else if (offset == ilinstruction.offset)
				{
					return ilinstruction;
				}
				if (offset < ilinstruction.offset)
				{
					num2 = num3 - 1;
				}
				else
				{
					i = num3 + 1;
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(28, 1);
			defaultInterpolatedStringHandler3.AppendLiteral("Cannot find instruction for ");
			defaultInterpolatedStringHandler3.AppendFormatted<int>(offset, "X4");
			throw new Exception(defaultInterpolatedStringHandler3.ToStringAndClear());
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x0000719B File Offset: 0x0000539B
		private static bool TargetsLocalVariable(OpCode opcode)
		{
			return opcode.Name.Contains("loc");
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x000071AE File Offset: 0x000053AE
		private LocalVariableInfo GetLocalVariable(int index)
		{
			List<LocalVariableInfo> list = this.localVariables;
			if (list == null)
			{
				return null;
			}
			return list[index];
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x000071C2 File Offset: 0x000053C2
		private ParameterInfo GetParameter(int index)
		{
			if (index == 0)
			{
				return this.this_parameter;
			}
			return this.parameters[index - 1];
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x000071D8 File Offset: 0x000053D8
		private OpCode ReadOpCode()
		{
			byte b = this.ilBytes.ReadByte();
			if (b == 254)
			{
				return MethodBodyReader.two_bytes_opcodes[(int)this.ilBytes.ReadByte()];
			}
			return MethodBodyReader.one_byte_opcodes[(int)b];
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x0000721C File Offset: 0x0000541C
		[MethodImpl(MethodImplOptions.Synchronized)]
		static MethodBodyReader()
		{
			FieldInfo[] fields = typeof(OpCodes).GetFields(BindingFlags.Static | BindingFlags.Public);
			foreach (FieldInfo fieldInfo in fields)
			{
				OpCode opCode = (OpCode)fieldInfo.GetValue(null);
				if (opCode.OpCodeType != OpCodeType.Nternal)
				{
					if (opCode.Size == 1)
					{
						MethodBodyReader.one_byte_opcodes[(int)opCode.Value] = opCode;
					}
					else
					{
						MethodBodyReader.two_bytes_opcodes[(int)(opCode.Value & 255)] = opCode;
					}
				}
			}
		}

		// Token: 0x0400008A RID: 138
		private readonly ILGenerator generator;

		// Token: 0x0400008B RID: 139
		private readonly MethodBase method;

		// Token: 0x0400008C RID: 140
		private bool debug;

		// Token: 0x0400008D RID: 141
		private readonly Module module;

		// Token: 0x0400008E RID: 142
		private readonly Type[] typeArguments;

		// Token: 0x0400008F RID: 143
		private readonly Type[] methodArguments;

		// Token: 0x04000090 RID: 144
		private readonly ByteBuffer ilBytes;

		// Token: 0x04000091 RID: 145
		private readonly ParameterInfo this_parameter;

		// Token: 0x04000092 RID: 146
		private readonly ParameterInfo[] parameters;

		// Token: 0x04000093 RID: 147
		private readonly IList<ExceptionHandlingClause> exceptions;

		// Token: 0x04000094 RID: 148
		private readonly List<ILInstruction> ilInstructions;

		// Token: 0x04000095 RID: 149
		private readonly List<LocalVariableInfo> localVariables;

		// Token: 0x04000096 RID: 150
		private LocalBuilder[] variables;

		// Token: 0x04000097 RID: 151
		private static readonly OpCode[] one_byte_opcodes = new OpCode[225];

		// Token: 0x04000098 RID: 152
		private static readonly OpCode[] two_bytes_opcodes = new OpCode[31];

		// Token: 0x02000030 RID: 48
		private class ThisParameter : ParameterInfo
		{
			// Token: 0x060000FB RID: 251 RVA: 0x000072D3 File Offset: 0x000054D3
			internal ThisParameter(MethodBase method)
			{
				this.MemberImpl = method;
				this.ClassImpl = method.DeclaringType;
				this.NameImpl = "this";
				this.PositionImpl = -1;
			}
		}
	}
}
