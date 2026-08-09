using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Captivity
{
	// Token: 0x02000066 RID: 102
	public class CaptivitySaveTheCrewmenObjective : MissionObjective
	{
		// Token: 0x06000604 RID: 1540 RVA: 0x0002334C File Offset: 0x0002154C
		public CaptivitySaveTheCrewmenObjective(Mission mission, NavalStorylineCaptivityMissionController captivityMissionController)
			: base(mission)
		{
			this._name = new TextObject("{=tvGCC1BF}Save the Crewmen", null);
			this._description = new TextObject("{=Ed0TIDfv}Steer the ship to save the crewmen in the water.", null);
			this._targetName = new TextObject("{=i0ELqRca}Rescue", null);
			this._captivityMissionController = captivityMissionController;
			foreach (Agent agent in this._captivityMissionController.GetScatteredCrewmen())
			{
				CaptivitySaveTheCrewmenObjective.CaptivityCrewmenTarget captivityCrewmenTarget = new CaptivitySaveTheCrewmenObjective.CaptivityCrewmenTarget(this._targetName, agent);
				base.AddTarget(captivityCrewmenTarget);
			}
		}

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x06000605 RID: 1541 RVA: 0x000233F4 File Offset: 0x000215F4
		public override string UniqueId
		{
			get
			{
				return "CaptivitySaveTheCrewmenObjective";
			}
		}

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x06000606 RID: 1542 RVA: 0x000233FB File Offset: 0x000215FB
		public override TextObject Name
		{
			get
			{
				return this._name;
			}
		}

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x06000607 RID: 1543 RVA: 0x00023403 File Offset: 0x00021603
		public override TextObject Description
		{
			get
			{
				return this._description;
			}
		}

		// Token: 0x06000608 RID: 1544 RVA: 0x0002340B File Offset: 0x0002160B
		protected override bool IsActivationRequirementsMet()
		{
			return true;
		}

		// Token: 0x06000609 RID: 1545 RVA: 0x0002340E File Offset: 0x0002160E
		protected override bool IsCompletionRequirementsMet()
		{
			return this._cachedProgress.CurrentProgressAmount == this._cachedProgress.RequiredProgressAmount;
		}

		// Token: 0x0600060A RID: 1546 RVA: 0x00023428 File Offset: 0x00021628
		public override MissionObjectiveProgressInfo GetCurrentProgress()
		{
			return this._cachedProgress;
		}

		// Token: 0x0600060B RID: 1547 RVA: 0x00023430 File Offset: 0x00021630
		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			MBReadOnlyList<CaptivitySaveTheCrewmenObjective.CaptivityCrewmenTarget> targetsCopy = base.GetTargetsCopy<CaptivitySaveTheCrewmenObjective.CaptivityCrewmenTarget>();
			this._cachedProgress.CurrentProgressAmount = 0;
			this._cachedProgress.RequiredProgressAmount = targetsCopy.Count;
			for (int i = 0; i < targetsCopy.Count; i++)
			{
				if (targetsCopy[i].Target.IsOnLand())
				{
					this._cachedProgress.CurrentProgressAmount = this._cachedProgress.CurrentProgressAmount + 1;
				}
			}
		}

		// Token: 0x040002E9 RID: 745
		private readonly NavalStorylineCaptivityMissionController _captivityMissionController;

		// Token: 0x040002EA RID: 746
		private readonly TextObject _name;

		// Token: 0x040002EB RID: 747
		private readonly TextObject _description;

		// Token: 0x040002EC RID: 748
		private readonly TextObject _targetName;

		// Token: 0x040002ED RID: 749
		private MissionObjectiveProgressInfo _cachedProgress;

		// Token: 0x020001D1 RID: 465
		private class CaptivityCrewmenTarget : MissionObjectiveTarget<Agent>
		{
			// Token: 0x06001A29 RID: 6697 RVA: 0x000AE916 File Offset: 0x000ACB16
			public CaptivityCrewmenTarget(TextObject name, Agent agent)
				: base(agent)
			{
				this._name = name;
			}

			// Token: 0x06001A2A RID: 6698 RVA: 0x000AE926 File Offset: 0x000ACB26
			public override Vec3 GetGlobalPosition()
			{
				if (Agent.Main == null)
				{
					return Vec3.Invalid;
				}
				return base.Target.Position + base.Target.Frame.rotation.u * 1.5f;
			}

			// Token: 0x06001A2B RID: 6699 RVA: 0x000AE964 File Offset: 0x000ACB64
			public override TextObject GetName()
			{
				return this._name;
			}

			// Token: 0x06001A2C RID: 6700 RVA: 0x000AE96C File Offset: 0x000ACB6C
			public override bool IsActive()
			{
				return !base.Target.IsOnLand();
			}

			// Token: 0x04000D48 RID: 3400
			private readonly TextObject _name;
		}
	}
}
