using System;
using System.Collections.Generic;

namespace MissionSharedLibrary.Config.HotKey
{
	// Token: 0x02000044 RID: 68
	public class SerializedGameKeyCategory
	{
		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000251 RID: 593 RVA: 0x000088ED File Offset: 0x00006AED
		// (set) Token: 0x06000252 RID: 594 RVA: 0x000088F5 File Offset: 0x00006AF5
		public string CategoryId { get; set; } = "DefaultGameKeyCategory";

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000253 RID: 595 RVA: 0x000088FE File Offset: 0x00006AFE
		// (set) Token: 0x06000254 RID: 596 RVA: 0x00008906 File Offset: 0x00006B06
		public List<SerializedGameKeySequence> GameKeySequences { get; set; } = new List<SerializedGameKeySequence>();

		// Token: 0x06000255 RID: 597 RVA: 0x00008910 File Offset: 0x00006B10
		public SerializedGameKeySequence GetGameKey(string gameKeyId)
		{
			for (int i = 0; i < this.GameKeySequences.Count; i++)
			{
				SerializedGameKeySequence serializedGameKeySequence = this.GameKeySequences[i];
				if (serializedGameKeySequence != null && serializedGameKeySequence.StringId == gameKeyId)
				{
					return serializedGameKeySequence;
				}
			}
			return null;
		}
	}
}
