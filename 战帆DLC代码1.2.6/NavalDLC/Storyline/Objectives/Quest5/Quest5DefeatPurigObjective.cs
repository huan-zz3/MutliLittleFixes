using System;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Quest5
{
	// Token: 0x0200004B RID: 75
	public class Quest5DefeatPurigObjective : MissionObjective
	{
		// Token: 0x170000EF RID: 239
		// (get) Token: 0x0600054F RID: 1359 RVA: 0x00022052 File Offset: 0x00020252
		public override string UniqueId
		{
			get
			{
				return "quest_5_defeat_purig_objective";
			}
		}

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x06000550 RID: 1360 RVA: 0x00022059 File Offset: 0x00020259
		public override TextObject Name
		{
			get
			{
				return new TextObject("{=lJ5BA3k4}Defeat Purig - Duel", null);
			}
		}

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x06000551 RID: 1361 RVA: 0x00022066 File Offset: 0x00020266
		public override TextObject Description
		{
			get
			{
				return new TextObject("{=oNBSZp8H}Defeat Purig in a duel.", null);
			}
		}

		// Token: 0x06000552 RID: 1362 RVA: 0x00022073 File Offset: 0x00020273
		public Quest5DefeatPurigObjective(Mission mission, Agent purigAgent)
			: base(mission)
		{
			this._purigAgent = purigAgent;
		}

		// Token: 0x06000553 RID: 1363 RVA: 0x00022083 File Offset: 0x00020283
		protected override bool IsActivationRequirementsMet()
		{
			return this._purigAgent != null;
		}

		// Token: 0x06000554 RID: 1364 RVA: 0x0002208E File Offset: 0x0002028E
		protected override bool IsCompletionRequirementsMet()
		{
			return this._purigAgent == null || !this._purigAgent.IsActive();
		}

		// Token: 0x040002AC RID: 684
		private readonly Agent _purigAgent;
	}
}
