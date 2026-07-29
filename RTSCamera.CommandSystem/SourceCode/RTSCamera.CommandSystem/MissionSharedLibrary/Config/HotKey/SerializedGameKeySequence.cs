using System;
using System.Collections.Generic;

namespace MissionSharedLibrary.Config.HotKey
{
	// Token: 0x02000043 RID: 67
	public class SerializedGameKeySequence
	{
		// Token: 0x1700007F RID: 127
		// (get) Token: 0x0600024C RID: 588 RVA: 0x000088C3 File Offset: 0x00006AC3
		// (set) Token: 0x0600024D RID: 589 RVA: 0x000088CB File Offset: 0x00006ACB
		public string StringId { get; set; }

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x0600024E RID: 590 RVA: 0x000088D4 File Offset: 0x00006AD4
		// (set) Token: 0x0600024F RID: 591 RVA: 0x000088DC File Offset: 0x00006ADC
		public List<SerializedGameKeySequenceAlternative> GameKeyAlternatives { get; set; }
	}
}
