using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	// Token: 0x02000727 RID: 1831
	internal sealed class OpCodeHandler_C_R : OpCodeHandlerModRM
	{
		// Token: 0x06002535 RID: 9525 RVA: 0x0007CBA2 File Offset: 0x0007ADA2
		public OpCodeHandler_C_R(Code code32, Code code64, Register baseReg)
		{
			this.code32 = code32;
			this.code64 = code64;
			this.baseReg = baseReg;
		}

		// Token: 0x06002536 RID: 9526 RVA: 0x0007CBC0 File Offset: 0x0007ADC0
		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if (decoder.is64bMode)
			{
				instruction.InternalSetCodeNoCheck(this.code64);
				instruction.Op1Register = (int)(decoder.state.rm + decoder.state.zs.extraBaseRegisterBase) + Register.RAX;
			}
			else
			{
				instruction.InternalSetCodeNoCheck(this.code32);
				instruction.Op1Register = (int)(decoder.state.rm + decoder.state.zs.extraBaseRegisterBase) + Register.EAX;
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
			instruction.Op0Register = num2 + this.baseReg;
		}

		// Token: 0x040037B0 RID: 14256
		private readonly Code code32;

		// Token: 0x040037B1 RID: 14257
		private readonly Code code64;

		// Token: 0x040037B2 RID: 14258
		private readonly Register baseReg;
	}
}
