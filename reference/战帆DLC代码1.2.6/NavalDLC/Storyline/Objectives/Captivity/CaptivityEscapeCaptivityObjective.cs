using System;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Captivity
{
	// Token: 0x02000064 RID: 100
	public class CaptivityEscapeCaptivityObjective : MissionObjective
	{
		// Token: 0x060005F5 RID: 1525 RVA: 0x00023178 File Offset: 0x00021378
		public CaptivityEscapeCaptivityObjective(Mission mission, NavalStorylineCaptivityMissionController captivityMissionController)
			: base(mission)
		{
			this._name = new TextObject("{=Kl4fHd5i}Escape Captivity", null);
			this._description = new TextObject("{=3Tvyyz7p}Unchain yourself from the oar bench.", null);
			this._captivityMissionController = captivityMissionController;
			this._cachedProgress.RequiredProgressAmount = 0;
			this._cachedProgress.CurrentProgressAmount = 0;
		}

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x060005F6 RID: 1526 RVA: 0x000231CD File Offset: 0x000213CD
		public override string UniqueId
		{
			get
			{
				return "CaptivityEscapeCaptivityObjective";
			}
		}

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x060005F7 RID: 1527 RVA: 0x000231D4 File Offset: 0x000213D4
		public override TextObject Name
		{
			get
			{
				return this._name;
			}
		}

		// Token: 0x17000132 RID: 306
		// (get) Token: 0x060005F8 RID: 1528 RVA: 0x000231DC File Offset: 0x000213DC
		public override TextObject Description
		{
			get
			{
				return this._description;
			}
		}

		// Token: 0x060005F9 RID: 1529 RVA: 0x000231E4 File Offset: 0x000213E4
		protected override bool IsActivationRequirementsMet()
		{
			return true;
		}

		// Token: 0x060005FA RID: 1530 RVA: 0x000231E7 File Offset: 0x000213E7
		protected override bool IsCompletionRequirementsMet()
		{
			return this._captivityMissionController.IsPlayerFree;
		}

		// Token: 0x060005FB RID: 1531 RVA: 0x000231F4 File Offset: 0x000213F4
		public override MissionObjectiveProgressInfo GetCurrentProgress()
		{
			return this._cachedProgress;
		}

		// Token: 0x040002E0 RID: 736
		private readonly NavalStorylineCaptivityMissionController _captivityMissionController;

		// Token: 0x040002E1 RID: 737
		private readonly TextObject _name;

		// Token: 0x040002E2 RID: 738
		private readonly TextObject _description;

		// Token: 0x040002E3 RID: 739
		private MissionObjectiveProgressInfo _cachedProgress;
	}
}
