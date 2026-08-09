using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Quest5
{
	// Token: 0x02000053 RID: 83
	public class Quest5TalkWithYourSisterObjective : MissionObjective
	{
		// Token: 0x17000107 RID: 263
		// (get) Token: 0x06000580 RID: 1408 RVA: 0x0002239F File Offset: 0x0002059F
		public override string UniqueId
		{
			get
			{
				return "quest_5_talk_with_your_sister_objective";
			}
		}

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x06000581 RID: 1409 RVA: 0x000223A6 File Offset: 0x000205A6
		public override TextObject Name
		{
			get
			{
				return new TextObject("{=btfAQ47G}Find your sister", null);
			}
		}

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x06000582 RID: 1410 RVA: 0x000223B3 File Offset: 0x000205B3
		public override TextObject Description
		{
			get
			{
				return new TextObject("{=VTjKuGYw}Find your sister in the hold of the prisoner ship.", null);
			}
		}

		// Token: 0x06000583 RID: 1411 RVA: 0x000223C0 File Offset: 0x000205C0
		public Quest5TalkWithYourSisterObjective(Mission mission, Agent sister)
			: base(mission)
		{
			this._target = new Quest5TalkWithYourSisterObjective.TalkWithYourSisterObjectiveTarget(sister);
			base.AddTarget(this._target);
		}

		// Token: 0x06000584 RID: 1412 RVA: 0x000223E1 File Offset: 0x000205E1
		protected override bool IsActivationRequirementsMet()
		{
			return this._target != null;
		}

		// Token: 0x06000585 RID: 1413 RVA: 0x000223EC File Offset: 0x000205EC
		protected override bool IsCompletionRequirementsMet()
		{
			return false;
		}

		// Token: 0x040002B6 RID: 694
		private Quest5TalkWithYourSisterObjective.TalkWithYourSisterObjectiveTarget _target;

		// Token: 0x020001CE RID: 462
		private class TalkWithYourSisterObjectiveTarget : MissionObjectiveTarget
		{
			// Token: 0x06001A1D RID: 6685 RVA: 0x000AE7F3 File Offset: 0x000AC9F3
			public TalkWithYourSisterObjectiveTarget(Agent sister)
			{
				this.TargetAgent = sister;
			}

			// Token: 0x06001A1E RID: 6686 RVA: 0x000AE802 File Offset: 0x000ACA02
			public override TextObject GetName()
			{
				return new TextObject("{=pY5bft0t}Cage for prisoners", null);
			}

			// Token: 0x06001A1F RID: 6687 RVA: 0x000AE80F File Offset: 0x000ACA0F
			public override Vec3 GetGlobalPosition()
			{
				return this.TargetAgent.GetEyeGlobalPosition();
			}

			// Token: 0x06001A20 RID: 6688 RVA: 0x000AE81C File Offset: 0x000ACA1C
			public override bool IsActive()
			{
				return this.TargetAgent != null && this.TargetAgent.IsActive();
			}

			// Token: 0x04000D45 RID: 3397
			public readonly Agent TargetAgent;
		}
	}
}
