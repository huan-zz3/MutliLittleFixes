using System;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.PirateBattle
{
	// Token: 0x02000061 RID: 97
	public class PirateBattlePhase1Objective : MissionObjective
	{
		// Token: 0x060005DF RID: 1503 RVA: 0x00022F28 File Offset: 0x00021128
		public PirateBattlePhase1Objective(Mission mission, PirateBattleMissionController missionController)
			: base(mission)
		{
			this._name = new TextObject("{=wKBtraSp}Defeat the Sea Hounds", null);
			this._description = new TextObject("{=uPJWFjM8}Board the enemy ship and defeat their troops.", null);
			this._missionController = missionController;
			this._cachedProgress = default(MissionObjectiveProgressInfo);
			this._cachedProgress.RequiredProgressAmount = 0;
		}

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x060005E0 RID: 1504 RVA: 0x00022F7D File Offset: 0x0002117D
		public override string UniqueId
		{
			get
			{
				return "PirateBattlePhase1Objective";
			}
		}

		// Token: 0x17000128 RID: 296
		// (get) Token: 0x060005E1 RID: 1505 RVA: 0x00022F84 File Offset: 0x00021184
		public override TextObject Name
		{
			get
			{
				return this._name;
			}
		}

		// Token: 0x17000129 RID: 297
		// (get) Token: 0x060005E2 RID: 1506 RVA: 0x00022F8C File Offset: 0x0002118C
		public override TextObject Description
		{
			get
			{
				return this._description;
			}
		}

		// Token: 0x060005E3 RID: 1507 RVA: 0x00022F94 File Offset: 0x00021194
		protected override bool IsActivationRequirementsMet()
		{
			return true;
		}

		// Token: 0x060005E4 RID: 1508 RVA: 0x00022F97 File Offset: 0x00021197
		protected override bool IsCompletionRequirementsMet()
		{
			return this._missionController.IsFirstShipCleared;
		}

		// Token: 0x060005E5 RID: 1509 RVA: 0x00022FA4 File Offset: 0x000211A4
		public override MissionObjectiveProgressInfo GetCurrentProgress()
		{
			return this._cachedProgress;
		}

		// Token: 0x040002D3 RID: 723
		private readonly PirateBattleMissionController _missionController;

		// Token: 0x040002D4 RID: 724
		private readonly TextObject _name;

		// Token: 0x040002D5 RID: 725
		private readonly TextObject _description;

		// Token: 0x040002D6 RID: 726
		private MissionObjectiveProgressInfo _cachedProgress;
	}
}
