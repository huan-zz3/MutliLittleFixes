using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.BlockEncoderInternal
{
	// Token: 0x020007E3 RID: 2019
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class JccInstr : Instr
	{
		// Token: 0x060026DA RID: 9946 RVA: 0x0004350F File Offset: 0x0004170F
		private static uint GetLongInstructionSize64(in Instruction instruction)
		{
			return 8U;
		}

		// Token: 0x060026DB RID: 9947 RVA: 0x00085E5C File Offset: 0x0008405C
		public JccInstr(BlockEncoder blockEncoder, Block block, in Instruction instruction)
			: base(block, instruction.IP)
		{
			this.bitness = (byte)blockEncoder.Bitness;
			this.instruction = instruction;
			this.instrKind = JccInstr.InstrKind.Uninitialized;
			this.longInstructionSize64 = (byte)JccInstr.GetLongInstructionSize64(in instruction);
			Instruction instruction2;
			if (!blockEncoder.FixBranches)
			{
				this.instrKind = JccInstr.InstrKind.Unchanged;
				instruction2 = instruction;
				instruction2.NearBranch64 = 0UL;
				this.Size = blockEncoder.GetInstructionSize(in instruction2, 0UL);
				return;
			}
			instruction2 = instruction;
			instruction2.InternalSetCodeNoCheck(instruction.Code.ToShortBranch());
			instruction2.NearBranch64 = 0UL;
			this.shortInstructionSize = (byte)blockEncoder.GetInstructionSize(in instruction2, 0UL);
			instruction2 = instruction;
			instruction2.InternalSetCodeNoCheck(instruction.Code.ToNearBranch());
			instruction2.NearBranch64 = 0UL;
			this.nearInstructionSize = (byte)blockEncoder.GetInstructionSize(in instruction2, 0UL);
			if (blockEncoder.Bitness == 64)
			{
				this.Size = (uint)Math.Max(this.nearInstructionSize, this.longInstructionSize64);
				return;
			}
			this.Size = (uint)this.nearInstructionSize;
		}

		// Token: 0x060026DC RID: 9948 RVA: 0x00085F67 File Offset: 0x00084167
		public override void Initialize(BlockEncoder blockEncoder)
		{
			this.targetInstr = blockEncoder.GetTarget(this.instruction.NearBranchTarget);
		}

		// Token: 0x060026DD RID: 9949 RVA: 0x00085F80 File Offset: 0x00084180
		public override bool Optimize(ulong gained)
		{
			return this.TryOptimize(gained);
		}

		// Token: 0x060026DE RID: 9950 RVA: 0x00085F8C File Offset: 0x0008418C
		private bool TryOptimize(ulong gained)
		{
			if (this.instrKind == JccInstr.InstrKind.Unchanged || this.instrKind == JccInstr.InstrKind.Short)
			{
				this.Done = true;
				return false;
			}
			long address = (long)this.targetInstr.GetAddress();
			ulong num = this.IP + (ulong)this.shortInstructionSize;
			long num2 = address - (long)num;
			num2 = Instr.ConvertDiffToBitnessDiff((int)this.bitness, Instr.CorrectDiff(this.targetInstr.IsInBlock(this.Block), num2, gained));
			if (-128L <= num2 && num2 <= 127L)
			{
				if (this.pointerData != null)
				{
					this.pointerData.IsValid = false;
				}
				this.instrKind = JccInstr.InstrKind.Short;
				this.Size = (uint)this.shortInstructionSize;
				this.Done = true;
				return true;
			}
			bool flag = this.bitness != 64 || this.targetInstr.IsInBlock(this.Block);
			if (!flag)
			{
				long address2 = (long)this.targetInstr.GetAddress();
				num = this.IP + (ulong)this.nearInstructionSize;
				num2 = address2 - (long)num;
				num2 = Instr.ConvertDiffToBitnessDiff((int)this.bitness, Instr.CorrectDiff(this.targetInstr.IsInBlock(this.Block), num2, gained));
				flag = -2147483648L <= num2 && num2 <= 2147483647L;
			}
			if (flag)
			{
				if (this.pointerData != null)
				{
					this.pointerData.IsValid = false;
				}
				if (num2 < -1920L || num2 > 1905L)
				{
					this.Done = true;
				}
				this.instrKind = JccInstr.InstrKind.Near;
				this.Size = (uint)this.nearInstructionSize;
				return true;
			}
			if (this.pointerData == null)
			{
				this.pointerData = this.Block.AllocPointerLocation();
			}
			this.instrKind = JccInstr.InstrKind.Long;
			return false;
		}

		// Token: 0x060026DF RID: 9951 RVA: 0x00086114 File Offset: 0x00084314
		[return: Nullable(2)]
		public override string TryEncode(Encoder encoder, out ConstantOffsets constantOffsets, out bool isOriginalInstruction)
		{
			switch (this.instrKind)
			{
			case JccInstr.InstrKind.Unchanged:
			case JccInstr.InstrKind.Short:
			case JccInstr.InstrKind.Near:
			{
				isOriginalInstruction = true;
				if (this.instrKind != JccInstr.InstrKind.Unchanged)
				{
					if (this.instrKind == JccInstr.InstrKind.Short)
					{
						this.instruction.InternalSetCodeNoCheck(this.instruction.Code.ToShortBranch());
					}
					else
					{
						this.instruction.InternalSetCodeNoCheck(this.instruction.Code.ToNearBranch());
					}
				}
				this.instruction.NearBranch64 = this.targetInstr.GetAddress();
				uint num;
				string text;
				if (!encoder.TryEncode(in this.instruction, this.IP, out num, out text))
				{
					constantOffsets = default(ConstantOffsets);
					return Instr.CreateErrorMessage(text, in this.instruction);
				}
				constantOffsets = encoder.GetConstantOffsets();
				return null;
			}
			case JccInstr.InstrKind.Long:
			{
				isOriginalInstruction = false;
				constantOffsets = default(ConstantOffsets);
				this.pointerData.Data = this.targetInstr.GetAddress();
				Instruction instruction = default(Instruction);
				instruction.InternalSetCodeNoCheck(JccInstr.ShortBrToNativeBr(this.instruction.Code.NegateConditionCode().ToShortBranch(), encoder.Bitness));
				if (this.instruction.OpCount != 1)
				{
					throw new InvalidOperationException();
				}
				instruction.Op0Kind = OpKind.NearBranch64;
				instruction.NearBranch64 = this.IP + (ulong)this.longInstructionSize64;
				string text;
				uint num2;
				if (!encoder.TryEncode(in instruction, this.IP, out num2, out text))
				{
					return Instr.CreateErrorMessage(text, in this.instruction);
				}
				uint num;
				text = base.EncodeBranchToPointerData(encoder, false, this.IP + (ulong)num2, this.pointerData, out num, this.Size - num2);
				if (text != null)
				{
					return Instr.CreateErrorMessage(text, in this.instruction);
				}
				return null;
			}
			}
			throw new InvalidOperationException();
		}

		// Token: 0x060026E0 RID: 9952 RVA: 0x000862C0 File Offset: 0x000844C0
		private static Code ShortBrToNativeBr(Code code, int bitness)
		{
			Code code2;
			Code code3;
			Code code4;
			switch (code)
			{
			case Code.Jo_rel8_16:
			case Code.Jo_rel8_32:
			case Code.Jo_rel8_64:
				code2 = Code.Jo_rel8_16;
				code3 = Code.Jo_rel8_32;
				code4 = Code.Jo_rel8_64;
				break;
			case Code.Jno_rel8_16:
			case Code.Jno_rel8_32:
			case Code.Jno_rel8_64:
				code2 = Code.Jno_rel8_16;
				code3 = Code.Jno_rel8_32;
				code4 = Code.Jno_rel8_64;
				break;
			case Code.Jb_rel8_16:
			case Code.Jb_rel8_32:
			case Code.Jb_rel8_64:
				code2 = Code.Jb_rel8_16;
				code3 = Code.Jb_rel8_32;
				code4 = Code.Jb_rel8_64;
				break;
			case Code.Jae_rel8_16:
			case Code.Jae_rel8_32:
			case Code.Jae_rel8_64:
				code2 = Code.Jae_rel8_16;
				code3 = Code.Jae_rel8_32;
				code4 = Code.Jae_rel8_64;
				break;
			case Code.Je_rel8_16:
			case Code.Je_rel8_32:
			case Code.Je_rel8_64:
				code2 = Code.Je_rel8_16;
				code3 = Code.Je_rel8_32;
				code4 = Code.Je_rel8_64;
				break;
			case Code.Jne_rel8_16:
			case Code.Jne_rel8_32:
			case Code.Jne_rel8_64:
				code2 = Code.Jne_rel8_16;
				code3 = Code.Jne_rel8_32;
				code4 = Code.Jne_rel8_64;
				break;
			case Code.Jbe_rel8_16:
			case Code.Jbe_rel8_32:
			case Code.Jbe_rel8_64:
				code2 = Code.Jbe_rel8_16;
				code3 = Code.Jbe_rel8_32;
				code4 = Code.Jbe_rel8_64;
				break;
			case Code.Ja_rel8_16:
			case Code.Ja_rel8_32:
			case Code.Ja_rel8_64:
				code2 = Code.Ja_rel8_16;
				code3 = Code.Ja_rel8_32;
				code4 = Code.Ja_rel8_64;
				break;
			case Code.Js_rel8_16:
			case Code.Js_rel8_32:
			case Code.Js_rel8_64:
				code2 = Code.Js_rel8_16;
				code3 = Code.Js_rel8_32;
				code4 = Code.Js_rel8_64;
				break;
			case Code.Jns_rel8_16:
			case Code.Jns_rel8_32:
			case Code.Jns_rel8_64:
				code2 = Code.Jns_rel8_16;
				code3 = Code.Jns_rel8_32;
				code4 = Code.Jns_rel8_64;
				break;
			case Code.Jp_rel8_16:
			case Code.Jp_rel8_32:
			case Code.Jp_rel8_64:
				code2 = Code.Jp_rel8_16;
				code3 = Code.Jp_rel8_32;
				code4 = Code.Jp_rel8_64;
				break;
			case Code.Jnp_rel8_16:
			case Code.Jnp_rel8_32:
			case Code.Jnp_rel8_64:
				code2 = Code.Jnp_rel8_16;
				code3 = Code.Jnp_rel8_32;
				code4 = Code.Jnp_rel8_64;
				break;
			case Code.Jl_rel8_16:
			case Code.Jl_rel8_32:
			case Code.Jl_rel8_64:
				code2 = Code.Jl_rel8_16;
				code3 = Code.Jl_rel8_32;
				code4 = Code.Jl_rel8_64;
				break;
			case Code.Jge_rel8_16:
			case Code.Jge_rel8_32:
			case Code.Jge_rel8_64:
				code2 = Code.Jge_rel8_16;
				code3 = Code.Jge_rel8_32;
				code4 = Code.Jge_rel8_64;
				break;
			case Code.Jle_rel8_16:
			case Code.Jle_rel8_32:
			case Code.Jle_rel8_64:
				code2 = Code.Jle_rel8_16;
				code3 = Code.Jle_rel8_32;
				code4 = Code.Jle_rel8_64;
				break;
			case Code.Jg_rel8_16:
			case Code.Jg_rel8_32:
			case Code.Jg_rel8_64:
				code2 = Code.Jg_rel8_16;
				code3 = Code.Jg_rel8_32;
				code4 = Code.Jg_rel8_64;
				break;
			default:
				throw new ArgumentOutOfRangeException("code");
			}
			Code code5;
			if (bitness != 16)
			{
				if (bitness != 32)
				{
					if (bitness != 64)
					{
						throw new ArgumentOutOfRangeException("bitness");
					}
					code5 = code4;
				}
				else
				{
					code5 = code3;
				}
			}
			else
			{
				code5 = code2;
			}
			return code5;
		}

		// Token: 0x0400397A RID: 14714
		private readonly byte bitness;

		// Token: 0x0400397B RID: 14715
		private Instruction instruction;

		// Token: 0x0400397C RID: 14716
		private TargetInstr targetInstr;

		// Token: 0x0400397D RID: 14717
		private BlockData pointerData;

		// Token: 0x0400397E RID: 14718
		private JccInstr.InstrKind instrKind;

		// Token: 0x0400397F RID: 14719
		private readonly byte shortInstructionSize;

		// Token: 0x04003980 RID: 14720
		private readonly byte nearInstructionSize;

		// Token: 0x04003981 RID: 14721
		private readonly byte longInstructionSize64;

		// Token: 0x020007E4 RID: 2020
		private enum InstrKind : byte
		{
			// Token: 0x04003983 RID: 14723
			Unchanged,
			// Token: 0x04003984 RID: 14724
			Short,
			// Token: 0x04003985 RID: 14725
			Near,
			// Token: 0x04003986 RID: 14726
			Long,
			// Token: 0x04003987 RID: 14727
			Uninitialized
		}
	}
}
