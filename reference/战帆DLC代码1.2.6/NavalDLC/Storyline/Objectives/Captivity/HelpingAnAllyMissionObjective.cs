using System;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Captivity
{
	// Token: 0x02000067 RID: 103
	public class HelpingAnAllyMissionObjective : MissionObjective
	{
		// Token: 0x0600060C RID: 1548 RVA: 0x0002349C File Offset: 0x0002169C
		public HelpingAnAllyMissionObjective(Mission mission)
			: base(mission)
		{
			this._name = new TextObject("{=J9ruJTIQ}Protect the Merchants", null);
			this._description = new TextObject("{=u2q4PdaI}Defeat all Sea Hounds before they capture the Vlandian merchantman", null);
			this._cachedProgress = default(MissionObjectiveProgressInfo);
		}

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x0600060D RID: 1549 RVA: 0x000234D3 File Offset: 0x000216D3
		public override string UniqueId
		{
			get
			{
				return "HelpingAnAllyMissionObjective";
			}
		}

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x0600060E RID: 1550 RVA: 0x000234DA File Offset: 0x000216DA
		public override TextObject Name
		{
			get
			{
				return this._name;
			}
		}

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x0600060F RID: 1551 RVA: 0x000234E2 File Offset: 0x000216E2
		public override TextObject Description
		{
			get
			{
				return this._description;
			}
		}

		// Token: 0x06000610 RID: 1552 RVA: 0x000234EA File Offset: 0x000216EA
		protected override bool IsActivationRequirementsMet()
		{
			return true;
		}

		// Token: 0x06000611 RID: 1553 RVA: 0x000234ED File Offset: 0x000216ED
		protected override bool IsCompletionRequirementsMet()
		{
			return false;
		}

		// Token: 0x06000612 RID: 1554 RVA: 0x000234F0 File Offset: 0x000216F0
		public override MissionObjectiveProgressInfo GetCurrentProgress()
		{
			return this._cachedProgress;
		}

		// Token: 0x040002EE RID: 750
		private readonly TextObject _name;

		// Token: 0x040002EF RID: 751
		private readonly TextObject _description;

		// Token: 0x040002F0 RID: 752
		private MissionObjectiveProgressInfo _cachedProgress;
	}
}
