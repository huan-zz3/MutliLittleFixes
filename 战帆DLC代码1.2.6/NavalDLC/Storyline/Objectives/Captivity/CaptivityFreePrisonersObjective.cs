using System;
using NavalDLC.Missions.Objects.UsableMachines;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Captivity
{
	// Token: 0x02000065 RID: 101
	public class CaptivityFreePrisonersObjective : MissionObjective
	{
		// Token: 0x060005FC RID: 1532 RVA: 0x000231FC File Offset: 0x000213FC
		public CaptivityFreePrisonersObjective(Mission mission, NavalStorylineCaptivityMissionController captivityMissionController)
			: base(mission)
		{
			this._name = new TextObject("{=Kl4fHd5i}Escape Captivity", null);
			this._description = new TextObject("{=57iHCBz9}Set all prisoners on the ship free.", null);
			this._targetName = new TextObject("{=mx9zqEzQ}Unchain", null);
			this._captivityMissionController = captivityMissionController;
			foreach (AgentBindsMachine agentBindsMachine in this._captivityMissionController.GetMarkedAgentBinds())
			{
				CaptivityFreePrisonersObjective.CaptivityPrisonerTarget captivityPrisonerTarget = new CaptivityFreePrisonersObjective.CaptivityPrisonerTarget(this._targetName, agentBindsMachine);
				base.AddTarget(captivityPrisonerTarget);
			}
		}

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x060005FD RID: 1533 RVA: 0x000232A4 File Offset: 0x000214A4
		public override string UniqueId
		{
			get
			{
				return "CaptivityFreePrisonersObjective";
			}
		}

		// Token: 0x17000134 RID: 308
		// (get) Token: 0x060005FE RID: 1534 RVA: 0x000232AB File Offset: 0x000214AB
		public override TextObject Name
		{
			get
			{
				return this._name;
			}
		}

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x060005FF RID: 1535 RVA: 0x000232B3 File Offset: 0x000214B3
		public override TextObject Description
		{
			get
			{
				return this._description;
			}
		}

		// Token: 0x06000600 RID: 1536 RVA: 0x000232BB File Offset: 0x000214BB
		protected override bool IsActivationRequirementsMet()
		{
			return true;
		}

		// Token: 0x06000601 RID: 1537 RVA: 0x000232BE File Offset: 0x000214BE
		protected override bool IsCompletionRequirementsMet()
		{
			return this._cachedProgress.CurrentProgressAmount == this._cachedProgress.RequiredProgressAmount;
		}

		// Token: 0x06000602 RID: 1538 RVA: 0x000232D8 File Offset: 0x000214D8
		public override MissionObjectiveProgressInfo GetCurrentProgress()
		{
			return this._cachedProgress;
		}

		// Token: 0x06000603 RID: 1539 RVA: 0x000232E0 File Offset: 0x000214E0
		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			MBReadOnlyList<CaptivityFreePrisonersObjective.CaptivityPrisonerTarget> targetsCopy = base.GetTargetsCopy<CaptivityFreePrisonersObjective.CaptivityPrisonerTarget>();
			this._cachedProgress.CurrentProgressAmount = 0;
			this._cachedProgress.RequiredProgressAmount = targetsCopy.Count;
			for (int i = 0; i < targetsCopy.Count; i++)
			{
				if (!targetsCopy[i].Target.HasCaptive)
				{
					this._cachedProgress.CurrentProgressAmount = this._cachedProgress.CurrentProgressAmount + 1;
				}
			}
		}

		// Token: 0x040002E4 RID: 740
		private readonly NavalStorylineCaptivityMissionController _captivityMissionController;

		// Token: 0x040002E5 RID: 741
		private readonly TextObject _name;

		// Token: 0x040002E6 RID: 742
		private readonly TextObject _description;

		// Token: 0x040002E7 RID: 743
		private readonly TextObject _targetName;

		// Token: 0x040002E8 RID: 744
		private MissionObjectiveProgressInfo _cachedProgress;

		// Token: 0x020001D0 RID: 464
		private class CaptivityPrisonerTarget : MissionObjectiveTarget<AgentBindsMachine>
		{
			// Token: 0x06001A25 RID: 6693 RVA: 0x000AE896 File Offset: 0x000ACA96
			public CaptivityPrisonerTarget(TextObject name, AgentBindsMachine agentBindMachine)
				: base(agentBindMachine)
			{
				this._name = name;
			}

			// Token: 0x06001A26 RID: 6694 RVA: 0x000AE8A8 File Offset: 0x000ACAA8
			public override Vec3 GetGlobalPosition()
			{
				if (Agent.Main == null)
				{
					return Vec3.Invalid;
				}
				return base.Target.GameEntity.GlobalPosition + base.Target.GameEntity.GetGlobalFrame().rotation.u * 1.5f;
			}

			// Token: 0x06001A27 RID: 6695 RVA: 0x000AE901 File Offset: 0x000ACB01
			public override TextObject GetName()
			{
				return this._name;
			}

			// Token: 0x06001A28 RID: 6696 RVA: 0x000AE909 File Offset: 0x000ACB09
			public override bool IsActive()
			{
				return base.Target.HasCaptive;
			}

			// Token: 0x04000D47 RID: 3399
			private readonly TextObject _name;
		}
	}
}
