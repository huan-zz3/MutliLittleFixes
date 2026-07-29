using System;
using System.Collections.Generic;
using TaleWorlds.Localization;

namespace MissionSharedLibrary.Usage
{
	// Token: 0x02000010 RID: 16
	public class UsageCategoryData
	{
		// Token: 0x0600009A RID: 154 RVA: 0x000045C6 File Offset: 0x000027C6
		public UsageCategoryData(TextObject name, List<TextObject> texts)
		{
			this.Name = name;
			this.UsageList = texts;
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600009B RID: 155 RVA: 0x000045DC File Offset: 0x000027DC
		public TextObject Name { get; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600009C RID: 156 RVA: 0x000045E4 File Offset: 0x000027E4
		public List<TextObject> UsageList { get; }
	}
}
