using System;
using NavalDLC.Missions.Objects;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Quest5
{
	// Token: 0x02000046 RID: 70
	public class Quest5ApproachObjective : MissionObjective
	{
		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x0600052E RID: 1326 RVA: 0x00021B51 File Offset: 0x0001FD51
		public override string UniqueId
		{
			get
			{
				return "quest_5_approach_objective";
			}
		}

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x0600052F RID: 1327 RVA: 0x00021B58 File Offset: 0x0001FD58
		public override TextObject Name
		{
			get
			{
				return new TextObject("{=s8t5kclT}Approach the meeting zone", null);
			}
		}

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x06000530 RID: 1328 RVA: 0x00021B65 File Offset: 0x0001FD65
		public override TextObject Description
		{
			get
			{
				return new TextObject("{=EmIS3tfC}Sail to within hailing distance of the Sea Hound ship.", null);
			}
		}

		// Token: 0x06000531 RID: 1329 RVA: 0x00021B72 File Offset: 0x0001FD72
		public Quest5ApproachObjective(Mission mission, MissionShip playerShip, MatrixFrame approachTargetFrame, float completionDistance)
			: base(mission)
		{
			this._playerShip = playerShip;
			this._completionDistance = completionDistance;
			this._target = new Quest5ApproachObjective.ApproachObjectiveTarget(approachTargetFrame);
			base.AddTarget(this._target);
		}

		// Token: 0x06000532 RID: 1330 RVA: 0x00021BA2 File Offset: 0x0001FDA2
		protected override bool IsActivationRequirementsMet()
		{
			return this._target != null;
		}

		// Token: 0x06000533 RID: 1331 RVA: 0x00021BB0 File Offset: 0x0001FDB0
		protected override bool IsCompletionRequirementsMet()
		{
			if (this._target != null)
			{
				Vec3 origin = this._target.ApproachTargetFrame.origin;
				return origin.Distance(this._playerShip.GameEntity.GetGlobalFrame().origin) <= this._completionDistance;
			}
			return false;
		}

		// Token: 0x040002A1 RID: 673
		private readonly MissionShip _playerShip;

		// Token: 0x040002A2 RID: 674
		private readonly float _completionDistance;

		// Token: 0x040002A3 RID: 675
		private Quest5ApproachObjective.ApproachObjectiveTarget _target;

		// Token: 0x020001C3 RID: 451
		private class ApproachObjectiveTarget : MissionObjectiveTarget
		{
			// Token: 0x060019F2 RID: 6642 RVA: 0x000AE506 File Offset: 0x000AC706
			public ApproachObjectiveTarget(MatrixFrame approachTargetFrame)
			{
				this.ApproachTargetFrame = approachTargetFrame;
			}

			// Token: 0x060019F3 RID: 6643 RVA: 0x000AE515 File Offset: 0x000AC715
			public override TextObject GetName()
			{
				return new TextObject("{=9pyEoT2i}Hailing point", null);
			}

			// Token: 0x060019F4 RID: 6644 RVA: 0x000AE522 File Offset: 0x000AC722
			public override Vec3 GetGlobalPosition()
			{
				return this.ApproachTargetFrame.origin;
			}

			// Token: 0x060019F5 RID: 6645 RVA: 0x000AE52F File Offset: 0x000AC72F
			public override bool IsActive()
			{
				return true;
			}

			// Token: 0x04000D38 RID: 3384
			public readonly MatrixFrame ApproachTargetFrame;
		}
	}
}
