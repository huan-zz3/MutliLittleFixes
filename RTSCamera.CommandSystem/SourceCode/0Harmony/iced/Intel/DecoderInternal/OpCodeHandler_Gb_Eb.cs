using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	// Token: 0x0200076E RID: 1902
	internal sealed class OpCodeHandler_Gb_Eb : OpCodeHandlerModRM
	{
		// Token: 0x060025C9 RID: 9673 RVA: 0x0007FD03 File Offset: 0x0007DF03
		public OpCodeHandler_Gb_Eb(Code code)
		{
			this.code = code;
		}

		// Token: 0x060025CA RID: 9674 RVA: 0x0007FD14 File Offset: 0x0007DF14
		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.InternalSetCodeNoCheck(this.code);
			uint num = decoder.state.reg + decoder.state.zs.extraRegisterBase;
			if ((decoder.state.zs.flags & StateFlags.HasRex) != (StateFlags)0U && num >= 4U)
			{
				num += 4U;
			}
			instruction.Op0Register = (int)num + Register.AL;
			if (decoder.state.mod == 3U)
			{
				num = decoder.state.rm + decoder.state.zs.extraBaseRegisterBase;
				if ((decoder.state.zs.flags & StateFlags.HasRex) != (StateFlags)0U && num >= 4U)
				{
					num += 4U;
				}
				instruction.Op1Register = (int)num + Register.AL;
				return;
			}
			instruction.Op1Kind = OpKind.Memory;
			decoder.ReadOpMem(ref instruction);
		}

		// Token: 0x0400383D RID: 14397
		private readonly Code code;
	}
}
