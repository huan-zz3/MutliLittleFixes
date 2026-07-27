using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Quest5
{
	// Token: 0x02000048 RID: 72
	public class Quest5ClearGuardsObjective : MissionObjective
	{
		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x0600053A RID: 1338 RVA: 0x00021C96 File Offset: 0x0001FE96
		public override string UniqueId
		{
			get
			{
				return "quest_5_clear_guards_objective";
			}
		}

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x0600053B RID: 1339 RVA: 0x00021C9D File Offset: 0x0001FE9D
		public override TextObject Name
		{
			get
			{
				return new TextObject("{=qc5Ymr0P}Take out the guards", null);
			}
		}

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x0600053C RID: 1340 RVA: 0x00021CAA File Offset: 0x0001FEAA
		public override TextObject Description
		{
			get
			{
				return new TextObject("{=12lWaxfF}Take out the guards as stealthily as possible.", null);
			}
		}

		// Token: 0x0600053D RID: 1341 RVA: 0x00021CB7 File Offset: 0x0001FEB7
		public Quest5ClearGuardsObjective(Mission mission, List<Agent> stealthAgents)
			: base(mission)
		{
			this._stealthAgents = stealthAgents;
			this._requiredProgressAmount = this._stealthAgents.Count;
		}

		// Token: 0x0600053E RID: 1342 RVA: 0x00021CD8 File Offset: 0x0001FED8
		public override MissionObjectiveProgressInfo GetCurrentProgress()
		{
			MissionObjectiveProgressInfo missionObjectiveProgressInfo = default(MissionObjectiveProgressInfo);
			missionObjectiveProgressInfo.CurrentProgressAmount = this._requiredProgressAmount - this._stealthAgents.Count;
			missionObjectiveProgressInfo.RequiredProgressAmount = this._requiredProgressAmount;
			return missionObjectiveProgressInfo;
		}

		// Token: 0x0600053F RID: 1343 RVA: 0x00021D14 File Offset: 0x0001FF14
		protected override bool IsActivationRequirementsMet()
		{
			return this._stealthAgents != null;
		}

		// Token: 0x06000540 RID: 1344 RVA: 0x00021D20 File Offset: 0x0001FF20
		protected override bool IsCompletionRequirementsMet()
		{
			if (this._stealthAgents == null)
			{
				return false;
			}
			if (!Extensions.IsEmpty<Agent>(this._stealthAgents))
			{
				return !LinQuick.AnyQ<Agent>(this._stealthAgents, (Agent a) => a.IsActive());
			}
			return true;
		}

		// Token: 0x040002A6 RID: 678
		private readonly List<Agent> _stealthAgents;

		// Token: 0x040002A7 RID: 679
		private readonly int _requiredProgressAmount;

		// Token: 0x020001C5 RID: 453
		private class ClearGuardObjectiveTarget : MissionObjectiveTarget
		{
			// Token: 0x060019FA RID: 6650 RVA: 0x000AE567 File Offset: 0x000AC767
			public ClearGuardObjectiveTarget(Agent target)
			{
				this._target = target;
			}

			// Token: 0x060019FB RID: 6651 RVA: 0x000AE576 File Offset: 0x000AC776
			public override TextObject GetName()
			{
				return new TextObject("{=1sJcKkVP}Guard", null);
			}

			// Token: 0x060019FC RID: 6652 RVA: 0x000AE583 File Offset: 0x000AC783
			public override Vec3 GetGlobalPosition()
			{
				return this._target.Position + Vec3.Up * 2f;
			}

			// Token: 0x060019FD RID: 6653 RVA: 0x000AE5A4 File Offset: 0x000AC7A4
			public override bool IsActive()
			{
				return this._target != null && this._target.IsActive();
			}

			// Token: 0x04000D39 RID: 3385
			private readonly Agent _target;
		}
	}
}
