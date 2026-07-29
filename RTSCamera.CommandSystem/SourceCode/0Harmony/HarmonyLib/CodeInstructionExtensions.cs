using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;

namespace HarmonyLib
{
	// Token: 0x020001C1 RID: 449
	public static class CodeInstructionExtensions
	{
		// Token: 0x060007BA RID: 1978 RVA: 0x00018FA0 File Offset: 0x000171A0
		internal static int GetSize(this CodeInstruction instruction)
		{
			int num = instruction.opcode.Size;
			switch (instruction.opcode.OperandType)
			{
			case OperandType.InlineBrTarget:
			case OperandType.InlineField:
			case OperandType.InlineI:
			case OperandType.InlineMethod:
			case OperandType.InlineSig:
			case OperandType.InlineString:
			case OperandType.InlineTok:
			case OperandType.InlineType:
			case OperandType.ShortInlineR:
				num += 4;
				break;
			case OperandType.InlineI8:
			case OperandType.InlineR:
				num += 8;
				break;
			case OperandType.InlineSwitch:
				num += (1 + ((Array)instruction.operand).Length) * 4;
				break;
			case OperandType.InlineVar:
				num += 2;
				break;
			case OperandType.ShortInlineBrTarget:
			case OperandType.ShortInlineI:
			case OperandType.ShortInlineVar:
				num++;
				break;
			}
			return num;
		}

		// Token: 0x060007BB RID: 1979 RVA: 0x00019049 File Offset: 0x00017249
		public static bool IsValid(this OpCode code)
		{
			return code.Size > 0;
		}

		// Token: 0x060007BC RID: 1980 RVA: 0x00019058 File Offset: 0x00017258
		public static bool OperandIs(this CodeInstruction code, object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (code.operand == null)
			{
				return false;
			}
			Type type = value.GetType();
			Type type2 = code.operand.GetType();
			if (AccessTools.IsInteger(type) && AccessTools.IsNumber(type2))
			{
				return Convert.ToInt64(code.operand) == Convert.ToInt64(value);
			}
			if (AccessTools.IsFloatingPoint(type) && AccessTools.IsNumber(type2))
			{
				return Convert.ToDouble(code.operand) == Convert.ToDouble(value);
			}
			return object.Equals(code.operand, value);
		}

		// Token: 0x060007BD RID: 1981 RVA: 0x000190E4 File Offset: 0x000172E4
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool OperandIs(this CodeInstruction code, MemberInfo value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			return object.Equals(code.operand, value);
		}

		// Token: 0x060007BE RID: 1982 RVA: 0x00019100 File Offset: 0x00017300
		public static bool Is(this CodeInstruction code, OpCode opcode, object operand)
		{
			return code.opcode == opcode && code.OperandIs(operand);
		}

		// Token: 0x060007BF RID: 1983 RVA: 0x00019119 File Offset: 0x00017319
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool Is(this CodeInstruction code, OpCode opcode, MemberInfo operand)
		{
			return code.opcode == opcode && code.OperandIs(operand);
		}

		// Token: 0x060007C0 RID: 1984 RVA: 0x00019134 File Offset: 0x00017334
		public static bool IsLdarg(this CodeInstruction code, int? n = null)
		{
			return ((n == null || n.Value == 0) && code.opcode == OpCodes.Ldarg_0) || ((n == null || n.Value == 1) && code.opcode == OpCodes.Ldarg_1) || ((n == null || n.Value == 2) && code.opcode == OpCodes.Ldarg_2) || ((n == null || n.Value == 3) && code.opcode == OpCodes.Ldarg_3) || (code.opcode == OpCodes.Ldarg && (n == null || n.Value == Convert.ToInt32(code.operand))) || (code.opcode == OpCodes.Ldarg_S && (n == null || n.Value == Convert.ToInt32(code.operand)));
		}

		// Token: 0x060007C1 RID: 1985 RVA: 0x00019240 File Offset: 0x00017440
		public static bool IsLdarga(this CodeInstruction code, int? n = null)
		{
			return (!(code.opcode != OpCodes.Ldarga) || !(code.opcode != OpCodes.Ldarga_S)) && (n == null || n.Value == Convert.ToInt32(code.operand));
		}

		// Token: 0x060007C2 RID: 1986 RVA: 0x00019294 File Offset: 0x00017494
		public static bool IsStarg(this CodeInstruction code, int? n = null)
		{
			return (!(code.opcode != OpCodes.Starg) || !(code.opcode != OpCodes.Starg_S)) && (n == null || n.Value == Convert.ToInt32(code.operand));
		}

		// Token: 0x060007C3 RID: 1987 RVA: 0x000192E6 File Offset: 0x000174E6
		public static bool IsLdloc(this CodeInstruction code, LocalBuilder variable = null)
		{
			return (CodeInstructionExtensions.opcodesLoadingLocalNormal.Contains(code.opcode) || CodeInstructionExtensions.opcodesLoadingLocalByAddress.Contains(code.opcode)) && (variable == null || object.Equals(variable, code.operand));
		}

		// Token: 0x060007C4 RID: 1988 RVA: 0x0001931F File Offset: 0x0001751F
		public static bool IsStloc(this CodeInstruction code, LocalBuilder variable = null)
		{
			return CodeInstructionExtensions.opcodesStoringLocal.Contains(code.opcode) && (variable == null || object.Equals(variable, code.operand));
		}

		// Token: 0x060007C5 RID: 1989 RVA: 0x00019346 File Offset: 0x00017546
		public static bool Branches(this CodeInstruction code, out Label? label)
		{
			if (CodeInstructionExtensions.opcodesBranching.Contains(code.opcode))
			{
				label = new Label?((Label)code.operand);
				return true;
			}
			label = null;
			return false;
		}

		// Token: 0x060007C6 RID: 1990 RVA: 0x0001937C File Offset: 0x0001757C
		public static bool Calls(this CodeInstruction code, MethodInfo method)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			return (!(code.opcode != OpCodes.Call) || !(code.opcode != OpCodes.Callvirt)) && object.Equals(code.operand, method);
		}

		// Token: 0x060007C7 RID: 1991 RVA: 0x000193C9 File Offset: 0x000175C9
		public static bool LoadsConstant(this CodeInstruction code)
		{
			return CodeInstructionExtensions.constantLoadingCodes.Contains(code.opcode);
		}

		// Token: 0x060007C8 RID: 1992 RVA: 0x000193DC File Offset: 0x000175DC
		public static bool LoadsConstant(this CodeInstruction code, long number)
		{
			OpCode opcode = code.opcode;
			return (number == -1L && opcode == OpCodes.Ldc_I4_M1) || (number == 0L && opcode == OpCodes.Ldc_I4_0) || (number == 1L && opcode == OpCodes.Ldc_I4_1) || (number == 2L && opcode == OpCodes.Ldc_I4_2) || (number == 3L && opcode == OpCodes.Ldc_I4_3) || (number == 4L && opcode == OpCodes.Ldc_I4_4) || (number == 5L && opcode == OpCodes.Ldc_I4_5) || (number == 6L && opcode == OpCodes.Ldc_I4_6) || (number == 7L && opcode == OpCodes.Ldc_I4_7) || (number == 8L && opcode == OpCodes.Ldc_I4_8) || ((!(opcode != OpCodes.Ldc_I4) || !(opcode != OpCodes.Ldc_I4_S) || !(opcode != OpCodes.Ldc_I8)) && Convert.ToInt64(code.operand) == number);
		}

		// Token: 0x060007C9 RID: 1993 RVA: 0x000194F0 File Offset: 0x000176F0
		public static bool LoadsConstant(this CodeInstruction code, double number)
		{
			if (code.opcode != OpCodes.Ldc_R4 && code.opcode != OpCodes.Ldc_R8)
			{
				return false;
			}
			double num = Convert.ToDouble(code.operand);
			return num == number;
		}

		// Token: 0x060007CA RID: 1994 RVA: 0x00019533 File Offset: 0x00017733
		public static bool LoadsConstant(this CodeInstruction code, Enum e)
		{
			return code.LoadsConstant(Convert.ToInt64(e));
		}

		// Token: 0x060007CB RID: 1995 RVA: 0x00019544 File Offset: 0x00017744
		public static bool LoadsConstant(this CodeInstruction code, string str)
		{
			if (code.opcode != OpCodes.Ldstr)
			{
				return false;
			}
			string text = Convert.ToString(code.operand);
			return text == str;
		}

		// Token: 0x060007CC RID: 1996 RVA: 0x00019578 File Offset: 0x00017778
		public static bool LoadsField(this CodeInstruction code, FieldInfo field, bool byAddress = false)
		{
			if (field == null)
			{
				throw new ArgumentNullException("field");
			}
			OpCode opCode = (field.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld);
			if (!byAddress && code.opcode == opCode && object.Equals(code.operand, field))
			{
				return true;
			}
			OpCode opCode2 = (field.IsStatic ? OpCodes.Ldsflda : OpCodes.Ldflda);
			return byAddress && code.opcode == opCode2 && object.Equals(code.operand, field);
		}

		// Token: 0x060007CD RID: 1997 RVA: 0x00019600 File Offset: 0x00017800
		public static bool StoresField(this CodeInstruction code, FieldInfo field)
		{
			if (field == null)
			{
				throw new ArgumentNullException("field");
			}
			OpCode opCode = (field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld);
			return code.opcode == opCode && object.Equals(code.operand, field);
		}

		// Token: 0x060007CE RID: 1998 RVA: 0x0001964C File Offset: 0x0001784C
		public static int LocalIndex(this CodeInstruction code)
		{
			if (code.opcode == OpCodes.Ldloc_0 || code.opcode == OpCodes.Stloc_0)
			{
				return 0;
			}
			if (code.opcode == OpCodes.Ldloc_1 || code.opcode == OpCodes.Stloc_1)
			{
				return 1;
			}
			if (code.opcode == OpCodes.Ldloc_2 || code.opcode == OpCodes.Stloc_2)
			{
				return 2;
			}
			if (code.opcode == OpCodes.Ldloc_3 || code.opcode == OpCodes.Stloc_3)
			{
				return 3;
			}
			if (code.opcode == OpCodes.Ldloc_S || code.opcode == OpCodes.Ldloc)
			{
				LocalBuilder localBuilder = code.operand as LocalBuilder;
				if (localBuilder != null)
				{
					return localBuilder.LocalIndex;
				}
				return Convert.ToInt32(code.operand);
			}
			else if (code.opcode == OpCodes.Stloc_S || code.opcode == OpCodes.Stloc)
			{
				LocalBuilder localBuilder2 = code.operand as LocalBuilder;
				if (localBuilder2 != null)
				{
					return localBuilder2.LocalIndex;
				}
				return Convert.ToInt32(code.operand);
			}
			else
			{
				if (!(code.opcode == OpCodes.Ldloca_S) && !(code.opcode == OpCodes.Ldloca))
				{
					throw new ArgumentException("Instruction is not a load or store", "code");
				}
				LocalBuilder localBuilder3 = code.operand as LocalBuilder;
				if (localBuilder3 != null)
				{
					return localBuilder3.LocalIndex;
				}
				return Convert.ToInt32(code.operand);
			}
		}

		// Token: 0x060007CF RID: 1999 RVA: 0x000197D4 File Offset: 0x000179D4
		public static int ArgumentIndex(this CodeInstruction code)
		{
			if (code.opcode == OpCodes.Ldarg_0)
			{
				return 0;
			}
			if (code.opcode == OpCodes.Ldarg_1)
			{
				return 1;
			}
			if (code.opcode == OpCodes.Ldarg_2)
			{
				return 2;
			}
			if (code.opcode == OpCodes.Ldarg_3)
			{
				return 3;
			}
			if (code.opcode == OpCodes.Ldarg_S || code.opcode == OpCodes.Ldarg)
			{
				return Convert.ToInt32(code.operand);
			}
			if (code.opcode == OpCodes.Starg_S || code.opcode == OpCodes.Starg)
			{
				return Convert.ToInt32(code.operand);
			}
			if (code.opcode == OpCodes.Ldarga_S || code.opcode == OpCodes.Ldarga)
			{
				return Convert.ToInt32(code.operand);
			}
			throw new ArgumentException("Instruction is not a load or store", "code");
		}

		// Token: 0x060007D0 RID: 2000 RVA: 0x000198D0 File Offset: 0x00017AD0
		public static CodeInstruction WithLabels(this CodeInstruction code, params Label[] labels)
		{
			code.labels.AddRange(labels);
			return code;
		}

		// Token: 0x060007D1 RID: 2001 RVA: 0x000198D0 File Offset: 0x00017AD0
		public static CodeInstruction WithLabels(this CodeInstruction code, IEnumerable<Label> labels)
		{
			code.labels.AddRange(labels);
			return code;
		}

		// Token: 0x060007D2 RID: 2002 RVA: 0x000198E0 File Offset: 0x00017AE0
		public static List<Label> ExtractLabels(this CodeInstruction code)
		{
			List<Label> list = new List<Label>(code.labels);
			code.labels.Clear();
			return list;
		}

		// Token: 0x060007D3 RID: 2003 RVA: 0x00019905 File Offset: 0x00017B05
		public static CodeInstruction MoveLabelsTo(this CodeInstruction code, CodeInstruction other)
		{
			other.WithLabels(code.ExtractLabels());
			return code;
		}

		// Token: 0x060007D4 RID: 2004 RVA: 0x00019915 File Offset: 0x00017B15
		public static CodeInstruction MoveLabelsFrom(this CodeInstruction code, CodeInstruction other)
		{
			return code.WithLabels(other.ExtractLabels());
		}

		// Token: 0x060007D5 RID: 2005 RVA: 0x00019923 File Offset: 0x00017B23
		public static CodeInstruction WithBlocks(this CodeInstruction code, params ExceptionBlock[] blocks)
		{
			code.blocks.AddRange(blocks);
			return code;
		}

		// Token: 0x060007D6 RID: 2006 RVA: 0x00019923 File Offset: 0x00017B23
		public static CodeInstruction WithBlocks(this CodeInstruction code, IEnumerable<ExceptionBlock> blocks)
		{
			code.blocks.AddRange(blocks);
			return code;
		}

		// Token: 0x060007D7 RID: 2007 RVA: 0x00019934 File Offset: 0x00017B34
		public static List<ExceptionBlock> ExtractBlocks(this CodeInstruction code)
		{
			List<ExceptionBlock> list = new List<ExceptionBlock>(code.blocks);
			code.blocks.Clear();
			return list;
		}

		// Token: 0x060007D8 RID: 2008 RVA: 0x00019959 File Offset: 0x00017B59
		public static CodeInstruction MoveBlocksTo(this CodeInstruction code, CodeInstruction other)
		{
			other.WithBlocks(code.ExtractBlocks());
			return code;
		}

		// Token: 0x060007D9 RID: 2009 RVA: 0x00019969 File Offset: 0x00017B69
		public static CodeInstruction MoveBlocksFrom(this CodeInstruction code, CodeInstruction other)
		{
			return code.WithBlocks(other.ExtractBlocks());
		}

		// Token: 0x040002AC RID: 684
		internal static readonly HashSet<OpCode> opcodesCalling = new HashSet<OpCode>
		{
			OpCodes.Call,
			OpCodes.Callvirt
		};

		// Token: 0x040002AD RID: 685
		internal static readonly HashSet<OpCode> opcodesLoadingLocalByAddress = new HashSet<OpCode>
		{
			OpCodes.Ldloca_S,
			OpCodes.Ldloca
		};

		// Token: 0x040002AE RID: 686
		internal static readonly HashSet<OpCode> opcodesLoadingLocalNormal = new HashSet<OpCode>
		{
			OpCodes.Ldloc_0,
			OpCodes.Ldloc_1,
			OpCodes.Ldloc_2,
			OpCodes.Ldloc_3,
			OpCodes.Ldloc_S,
			OpCodes.Ldloc
		};

		// Token: 0x040002AF RID: 687
		internal static readonly HashSet<OpCode> opcodesStoringLocal = new HashSet<OpCode>
		{
			OpCodes.Stloc_0,
			OpCodes.Stloc_1,
			OpCodes.Stloc_2,
			OpCodes.Stloc_3,
			OpCodes.Stloc_S,
			OpCodes.Stloc
		};

		// Token: 0x040002B0 RID: 688
		internal static readonly HashSet<OpCode> opcodesLoadingArgumentByAddress = new HashSet<OpCode>
		{
			OpCodes.Ldarga_S,
			OpCodes.Ldarga
		};

		// Token: 0x040002B1 RID: 689
		internal static readonly HashSet<OpCode> opcodesLoadingArgumentNormal = new HashSet<OpCode>
		{
			OpCodes.Ldarg_0,
			OpCodes.Ldarg_1,
			OpCodes.Ldarg_2,
			OpCodes.Ldarg_3,
			OpCodes.Ldarg_S,
			OpCodes.Ldarg
		};

		// Token: 0x040002B2 RID: 690
		internal static readonly HashSet<OpCode> opcodesStoringArgument = new HashSet<OpCode>
		{
			OpCodes.Starg_S,
			OpCodes.Starg
		};

		// Token: 0x040002B3 RID: 691
		internal static readonly HashSet<OpCode> opcodesBranching = new HashSet<OpCode>
		{
			OpCodes.Br_S,
			OpCodes.Brfalse_S,
			OpCodes.Brtrue_S,
			OpCodes.Beq_S,
			OpCodes.Bge_S,
			OpCodes.Bgt_S,
			OpCodes.Ble_S,
			OpCodes.Blt_S,
			OpCodes.Bne_Un_S,
			OpCodes.Bge_Un_S,
			OpCodes.Bgt_Un_S,
			OpCodes.Ble_Un_S,
			OpCodes.Blt_Un_S,
			OpCodes.Br,
			OpCodes.Brfalse,
			OpCodes.Brtrue,
			OpCodes.Beq,
			OpCodes.Bge,
			OpCodes.Bgt,
			OpCodes.Ble,
			OpCodes.Blt,
			OpCodes.Bne_Un,
			OpCodes.Bge_Un,
			OpCodes.Bgt_Un,
			OpCodes.Ble_Un,
			OpCodes.Blt_Un
		};

		// Token: 0x040002B4 RID: 692
		private static readonly HashSet<OpCode> constantLoadingCodes = new HashSet<OpCode>
		{
			OpCodes.Ldc_I4_M1,
			OpCodes.Ldc_I4_0,
			OpCodes.Ldc_I4_1,
			OpCodes.Ldc_I4_2,
			OpCodes.Ldc_I4_3,
			OpCodes.Ldc_I4_4,
			OpCodes.Ldc_I4_5,
			OpCodes.Ldc_I4_6,
			OpCodes.Ldc_I4_7,
			OpCodes.Ldc_I4_8,
			OpCodes.Ldc_I4,
			OpCodes.Ldc_I4_S,
			OpCodes.Ldc_I8,
			OpCodes.Ldc_R4,
			OpCodes.Ldc_R8,
			OpCodes.Ldstr
		};
	}
}
