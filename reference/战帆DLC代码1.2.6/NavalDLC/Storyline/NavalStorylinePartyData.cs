using System;
using TaleWorlds.CampaignSystem.Party;

namespace NavalDLC.Storyline
{
	// Token: 0x02000032 RID: 50
	public class NavalStorylinePartyData
	{
		// Token: 0x1700006F RID: 111
		// (get) Token: 0x060002EE RID: 750 RVA: 0x000162C0 File Offset: 0x000144C0
		// (set) Token: 0x060002EF RID: 751 RVA: 0x000162C8 File Offset: 0x000144C8
		public bool IsQuestParty { get; set; }

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x060002F0 RID: 752 RVA: 0x000162D1 File Offset: 0x000144D1
		// (set) Token: 0x060002F1 RID: 753 RVA: 0x000162D9 File Offset: 0x000144D9
		public int PartySize { get; set; }

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x060002F2 RID: 754 RVA: 0x000162E2 File Offset: 0x000144E2
		// (set) Token: 0x060002F3 RID: 755 RVA: 0x000162EA File Offset: 0x000144EA
		public PartyTemplateObject Template { get; set; }
	}
}
