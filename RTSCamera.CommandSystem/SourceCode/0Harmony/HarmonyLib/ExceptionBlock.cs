using System;

namespace HarmonyLib
{
	// Token: 0x0200007B RID: 123
	public class ExceptionBlock
	{
		// Token: 0x06000236 RID: 566 RVA: 0x0000E225 File Offset: 0x0000C425
		public ExceptionBlock(ExceptionBlockType blockType, Type catchType = null)
		{
			this.blockType = blockType;
			this.catchType = catchType ?? typeof(object);
		}

		// Token: 0x0400018F RID: 399
		public ExceptionBlockType blockType;

		// Token: 0x04000190 RID: 400
		public Type catchType;
	}
}
