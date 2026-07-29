using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	// Token: 0x02000726 RID: 1830
	internal sealed class OpCodeHandler_R_C : OpCodeHandlerModRM
	{
		// Token: 0x06002533 RID: 9523 RVA: 0x0007CA4C File Offset: 0x0007AC4C
		public OpCodeHandler_R_C(Code code32, Code code64, Register baseReg)
		{
			this.code32 = code32;
			this.code64 = code64;
			this.baseReg = baseReg;
		}

		// Token: 0x06002534 RID: 9524 RVA: 0x0007CA6C File Offset: 0x0007AC6C
		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if (decoder.is64bMode)
			{
				instruction.InternalSetCodeNoCheck(this.code64);
				instruction.Op0Register = (int)(decoder.state.rm + decoder.state.zs.extraBaseRegisterBase) + Register.RAX;
			}
			else
			{
				instruction.InternalSetCodeNoCheck(this.code32);
				instruction.Op0Register = (int)(decoder.state.rm + decoder.state.zs.extraBaseRegisterBase) + Register.EAX;
			}
			uint num = decoder.state.zs.extraRegisterBase;
			if (this.baseReg == Register.CR0 && instruction.HasLockPrefix && (decoder.options & DecoderOptions.AMD) != DecoderOptions.None)
			{
				if ((num & decoder.invalidCheckMask) != 0U)
				{
					decoder.SetInvalidInstruction();
				}
				num = 8U;
				instruction.InternalClearHasLockPrefix();
				decoder.state.zs.flags = decoder.state.zs.flags & ~StateFlags.Lock;
			}
			int num2 = (int)(decoder.state.reg + num);
			if (decoder.invalidCheckMask != 0U)
			{
				if (this.baseReg == Register.CR0)
				{
					if (num2 == 1 || (num2 != 8 && num2 >= 5))
					{
						decoder.SetInvalidInstruction();
					}
				}
				else if (this.baseReg == Register.DR0 && num2 > 7)
				{
					decoder.SetInvalidInstruction();
				}
			}
			instruction.Op1Register = num2 + this.baseReg;
		}

		// Token: 0x040037AD RID: 14253
		private readonly Code code32;

		// Token: 0x040037AE RID: 14254
		private readonly Code code64;

		// Token: 0x040037AF RID: 14255
		private readonly Register baseReg;
	}
}
