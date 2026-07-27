using System;
using NavalDLC.Missions.Objects.UsableMachines;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Quest4
{
	// Token: 0x02000056 RID: 86
	public class DestroyMangonelsObjective : MissionObjective
	{
		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06000592 RID: 1426 RVA: 0x000224DB File Offset: 0x000206DB
		public override string UniqueId
		{
			get
			{
				return "naval_storyline_quest_4_destroy_targets_objective";
			}
		}

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x06000593 RID: 1427 RVA: 0x000224E2 File Offset: 0x000206E2
		public override TextObject Name
		{
			get
			{
				return new TextObject("{=ZpuppygP}Destroy the Mangonels", null);
			}
		}

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x06000594 RID: 1428 RVA: 0x000224EF File Offset: 0x000206EF
		public override TextObject Description
		{
			get
			{
				return new TextObject("{=OrI07kdd}Steer the Wasp and destroy the mangonels with your ballista without getting hit yourself", null);
			}
		}

		// Token: 0x06000595 RID: 1429 RVA: 0x000224FC File Offset: 0x000206FC
		public DestroyMangonelsObjective(Mission mission, MBList<ShipMangonel> targets)
			: base(mission)
		{
			this._initialTargets = targets.Count;
			this._remainingTargets = targets.Count;
			foreach (ShipMangonel shipMangonel in targets)
			{
				base.AddTarget(new MangonelObjectiveTarget(shipMangonel));
				shipMangonel.DestructionComponent.OnDestroyed += new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnMangonelDestroyed);
			}
		}

		// Token: 0x06000596 RID: 1430 RVA: 0x00022588 File Offset: 0x00020788
		private void OnMangonelDestroyed(DestructableComponent target, Agent attackerAgent, in MissionWeapon weapon, ScriptComponentBehavior attackerScriptComponentBehavior, int inflictedDamage)
		{
			this._remainingTargets--;
		}

		// Token: 0x06000597 RID: 1431 RVA: 0x00022598 File Offset: 0x00020798
		protected override bool IsActivationRequirementsMet()
		{
			return this._remainingTargets > 0;
		}

		// Token: 0x06000598 RID: 1432 RVA: 0x000225A3 File Offset: 0x000207A3
		protected override bool IsCompletionRequirementsMet()
		{
			return this._remainingTargets == 0;
		}

		// Token: 0x06000599 RID: 1433 RVA: 0x000225B0 File Offset: 0x000207B0
		public override MissionObjectiveProgressInfo GetCurrentProgress()
		{
			MissionObjectiveProgressInfo missionObjectiveProgressInfo = default(MissionObjectiveProgressInfo);
			missionObjectiveProgressInfo.CurrentProgressAmount = this._initialTargets - this._remainingTargets;
			missionObjectiveProgressInfo.RequiredProgressAmount = this._initialTargets;
			return missionObjectiveProgressInfo;
		}

		// Token: 0x040002B9 RID: 697
		private readonly int _initialTargets;

		// Token: 0x040002BA RID: 698
		private int _remainingTargets;
	}
}
