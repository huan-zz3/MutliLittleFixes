using System;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.PirateBattle
{
	// Token: 0x02000060 RID: 96
	public class PirateBattleCutLooseObjective : MissionObjective
	{
		// Token: 0x060005D8 RID: 1496 RVA: 0x00022EA4 File Offset: 0x000210A4
		public PirateBattleCutLooseObjective(Mission mission, PirateBattleMissionController missionController)
			: base(mission)
		{
			this._name = new TextObject("{=KVmdmC4B}Cut Ships Loose", null);
			this._description = new TextObject("{=Sx9IRFbl}Sever the ties between your ships.", null);
			this._missionController = missionController;
			this._cachedProgress = default(MissionObjectiveProgressInfo);
			this._cachedProgress.RequiredProgressAmount = 0;
		}

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x060005D9 RID: 1497 RVA: 0x00022EF9 File Offset: 0x000210F9
		public override string UniqueId
		{
			get
			{
				return "PirateBattleCutLooseObjective";
			}
		}

		// Token: 0x17000125 RID: 293
		// (get) Token: 0x060005DA RID: 1498 RVA: 0x00022F00 File Offset: 0x00021100
		public override TextObject Name
		{
			get
			{
				return this._name;
			}
		}

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x060005DB RID: 1499 RVA: 0x00022F08 File Offset: 0x00021108
		public override TextObject Description
		{
			get
			{
				return this._description;
			}
		}

		// Token: 0x060005DC RID: 1500 RVA: 0x00022F10 File Offset: 0x00021110
		protected override bool IsActivationRequirementsMet()
		{
			return true;
		}

		// Token: 0x060005DD RID: 1501 RVA: 0x00022F13 File Offset: 0x00021113
		protected override bool IsCompletionRequirementsMet()
		{
			return this._missionController.HaveAllyShipsBeenCutLoose();
		}

		// Token: 0x060005DE RID: 1502 RVA: 0x00022F20 File Offset: 0x00021120
		public override MissionObjectiveProgressInfo GetCurrentProgress()
		{
			return this._cachedProgress;
		}

		// Token: 0x040002CF RID: 719
		private readonly PirateBattleMissionController _missionController;

		// Token: 0x040002D0 RID: 720
		private readonly TextObject _name;

		// Token: 0x040002D1 RID: 721
		private readonly TextObject _description;

		// Token: 0x040002D2 RID: 722
		private MissionObjectiveProgressInfo _cachedProgress;
	}
}
