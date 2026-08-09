using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Quest5
{
	// Token: 0x0200004F RID: 79
	public class Quest5JumpObjective : MissionObjective
	{
		// Token: 0x170000FB RID: 251
		// (get) Token: 0x06000568 RID: 1384 RVA: 0x000221D9 File Offset: 0x000203D9
		public override string UniqueId
		{
			get
			{
				return "quest_5_jump_objective";
			}
		}

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x06000569 RID: 1385 RVA: 0x000221E0 File Offset: 0x000203E0
		public override TextObject Name
		{
			get
			{
				return new TextObject("{=tbHD7j4G}Follow Gunnar into the water", null);
			}
		}

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x0600056A RID: 1386 RVA: 0x000221ED File Offset: 0x000203ED
		public override TextObject Description
		{
			get
			{
				return new TextObject("{=bNX3b3Ry}Jump off the ship, following Gunnar into the water.", null);
			}
		}

		// Token: 0x0600056B RID: 1387 RVA: 0x000221FA File Offset: 0x000203FA
		public Quest5JumpObjective(Mission mission, Agent targetAgent)
			: base(mission)
		{
			this._target = new Quest5JumpObjective.JumpObjectiveTarget(targetAgent);
			base.AddTarget(this._target);
		}

		// Token: 0x0600056C RID: 1388 RVA: 0x0002221B File Offset: 0x0002041B
		protected override bool IsActivationRequirementsMet()
		{
			return this._target != null;
		}

		// Token: 0x0600056D RID: 1389 RVA: 0x00022226 File Offset: 0x00020426
		protected override bool IsCompletionRequirementsMet()
		{
			return this._target != null && Agent.Main.IsInWater();
		}

		// Token: 0x040002B0 RID: 688
		private Quest5JumpObjective.JumpObjectiveTarget _target;

		// Token: 0x020001CB RID: 459
		private class JumpObjectiveTarget : MissionObjectiveTarget
		{
			// Token: 0x06001A11 RID: 6673 RVA: 0x000AE726 File Offset: 0x000AC926
			public JumpObjectiveTarget(Agent target)
			{
				this._target = target;
			}

			// Token: 0x06001A12 RID: 6674 RVA: 0x000AE735 File Offset: 0x000AC935
			public override TextObject GetName()
			{
				return this._target.Character.Name;
			}

			// Token: 0x06001A13 RID: 6675 RVA: 0x000AE747 File Offset: 0x000AC947
			public override Vec3 GetGlobalPosition()
			{
				return this._target.Position + Vec3.Up * 2f;
			}

			// Token: 0x06001A14 RID: 6676 RVA: 0x000AE768 File Offset: 0x000AC968
			public override bool IsActive()
			{
				return true;
			}

			// Token: 0x04000D43 RID: 3395
			private readonly Agent _target;
		}
	}
}
