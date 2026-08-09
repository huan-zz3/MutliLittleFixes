using System;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.PirateBattle
{
	// Token: 0x02000062 RID: 98
	public class PirateBattlePhase2Objective : MissionObjective
	{
		// Token: 0x060005E6 RID: 1510 RVA: 0x00022FAC File Offset: 0x000211AC
		public PirateBattlePhase2Objective(Mission mission, PirateBattleMissionController missionController)
			: base(mission)
		{
			this._name = new TextObject("{=0uxtZE36}Defeat the Reinforcements", null);
			this._description = new TextObject("{=rqhEyQ5L}Attack the second Sea Hounds ship with your allies.", null);
			this._missionController = missionController;
			this._cachedProgress = default(MissionObjectiveProgressInfo);
			this._cachedProgress.RequiredProgressAmount = 0;
		}

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x060005E7 RID: 1511 RVA: 0x00023001 File Offset: 0x00021201
		public override string UniqueId
		{
			get
			{
				return "PirateBattlePhase2Objective";
			}
		}

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x060005E8 RID: 1512 RVA: 0x00023008 File Offset: 0x00021208
		public override TextObject Name
		{
			get
			{
				return this._name;
			}
		}

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x060005E9 RID: 1513 RVA: 0x00023010 File Offset: 0x00021210
		public override TextObject Description
		{
			get
			{
				return this._description;
			}
		}

		// Token: 0x060005EA RID: 1514 RVA: 0x00023018 File Offset: 0x00021218
		protected override bool IsActivationRequirementsMet()
		{
			return true;
		}

		// Token: 0x060005EB RID: 1515 RVA: 0x0002301B File Offset: 0x0002121B
		protected override bool IsCompletionRequirementsMet()
		{
			return false;
		}

		// Token: 0x060005EC RID: 1516 RVA: 0x0002301E File Offset: 0x0002121E
		public override MissionObjectiveProgressInfo GetCurrentProgress()
		{
			return this._cachedProgress;
		}

		// Token: 0x040002D7 RID: 727
		private readonly PirateBattleMissionController _missionController;

		// Token: 0x040002D8 RID: 728
		private readonly TextObject _name;

		// Token: 0x040002D9 RID: 729
		private readonly TextObject _description;

		// Token: 0x040002DA RID: 730
		private MissionObjectiveProgressInfo _cachedProgress;
	}
}
