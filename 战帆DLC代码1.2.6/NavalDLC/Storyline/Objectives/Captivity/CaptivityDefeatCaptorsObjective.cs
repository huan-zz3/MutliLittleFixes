using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Captivity
{
	// Token: 0x02000063 RID: 99
	public class CaptivityDefeatCaptorsObjective : MissionObjective
	{
		// Token: 0x060005ED RID: 1517 RVA: 0x00023028 File Offset: 0x00021228
		public CaptivityDefeatCaptorsObjective(Mission mission, NavalStorylineCaptivityMissionController captivityMissionController)
			: base(mission)
		{
			this._name = new TextObject("{=Kl4fHd5i}Escape Captivity", null);
			this._description = new TextObject("{=sHQ5b9fV}Defeat your captors.", null);
			this._targetName = new TextObject("{=defeatVerb}Defeat", null);
			this._captivityMissionController = captivityMissionController;
			foreach (Agent agent in this._captivityMissionController.GetCaptorAgents())
			{
				CaptivityDefeatCaptorsObjective.CaptivityEnemyTarget captivityEnemyTarget = new CaptivityDefeatCaptorsObjective.CaptivityEnemyTarget(this._targetName, agent);
				base.AddTarget(captivityEnemyTarget);
			}
		}

		// Token: 0x1700012D RID: 301
		// (get) Token: 0x060005EE RID: 1518 RVA: 0x000230D0 File Offset: 0x000212D0
		public override string UniqueId
		{
			get
			{
				return "CaptivityDefeatCaptorsObjective";
			}
		}

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x060005EF RID: 1519 RVA: 0x000230D7 File Offset: 0x000212D7
		public override TextObject Name
		{
			get
			{
				return this._name;
			}
		}

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x060005F0 RID: 1520 RVA: 0x000230DF File Offset: 0x000212DF
		public override TextObject Description
		{
			get
			{
				return this._description;
			}
		}

		// Token: 0x060005F1 RID: 1521 RVA: 0x000230E7 File Offset: 0x000212E7
		protected override bool IsActivationRequirementsMet()
		{
			return true;
		}

		// Token: 0x060005F2 RID: 1522 RVA: 0x000230EA File Offset: 0x000212EA
		protected override bool IsCompletionRequirementsMet()
		{
			return this._cachedProgress.CurrentProgressAmount == this._cachedProgress.RequiredProgressAmount;
		}

		// Token: 0x060005F3 RID: 1523 RVA: 0x00023104 File Offset: 0x00021304
		public override MissionObjectiveProgressInfo GetCurrentProgress()
		{
			return this._cachedProgress;
		}

		// Token: 0x060005F4 RID: 1524 RVA: 0x0002310C File Offset: 0x0002130C
		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			MBReadOnlyList<CaptivityDefeatCaptorsObjective.CaptivityEnemyTarget> targetsCopy = base.GetTargetsCopy<CaptivityDefeatCaptorsObjective.CaptivityEnemyTarget>();
			this._cachedProgress.CurrentProgressAmount = 0;
			this._cachedProgress.RequiredProgressAmount = targetsCopy.Count;
			for (int i = 0; i < targetsCopy.Count; i++)
			{
				if (!targetsCopy[i].Target.IsActive())
				{
					this._cachedProgress.CurrentProgressAmount = this._cachedProgress.CurrentProgressAmount + 1;
				}
			}
		}

		// Token: 0x040002DB RID: 731
		private readonly NavalStorylineCaptivityMissionController _captivityMissionController;

		// Token: 0x040002DC RID: 732
		private readonly TextObject _name;

		// Token: 0x040002DD RID: 733
		private readonly TextObject _description;

		// Token: 0x040002DE RID: 734
		private readonly TextObject _targetName;

		// Token: 0x040002DF RID: 735
		private MissionObjectiveProgressInfo _cachedProgress;

		// Token: 0x020001CF RID: 463
		private class CaptivityEnemyTarget : MissionObjectiveTarget<Agent>
		{
			// Token: 0x06001A21 RID: 6689 RVA: 0x000AE833 File Offset: 0x000ACA33
			public CaptivityEnemyTarget(TextObject name, Agent agent)
				: base(agent)
			{
				this._name = name;
			}

			// Token: 0x06001A22 RID: 6690 RVA: 0x000AE843 File Offset: 0x000ACA43
			public override Vec3 GetGlobalPosition()
			{
				if (Agent.Main == null)
				{
					return Vec3.Invalid;
				}
				return base.Target.Position + base.Target.Frame.rotation.u * 1.5f;
			}

			// Token: 0x06001A23 RID: 6691 RVA: 0x000AE881 File Offset: 0x000ACA81
			public override TextObject GetName()
			{
				return this._name;
			}

			// Token: 0x06001A24 RID: 6692 RVA: 0x000AE889 File Offset: 0x000ACA89
			public override bool IsActive()
			{
				return base.Target.IsActive();
			}

			// Token: 0x04000D46 RID: 3398
			private readonly TextObject _name;
		}
	}
}
