using System;
using System.Linq;
using NavalDLC.Missions.Objects;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;
using TaleWorlds.MountAndBlade.Objects.Usables;

namespace NavalDLC.Storyline.Objectives.Quest5
{
	// Token: 0x02000052 RID: 82
	public class Quest5SwimObjective : MissionObjective
	{
		// Token: 0x17000104 RID: 260
		// (get) Token: 0x0600057A RID: 1402 RVA: 0x0002232E File Offset: 0x0002052E
		public override string UniqueId
		{
			get
			{
				return "quest_5_swim_objective";
			}
		}

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x0600057B RID: 1403 RVA: 0x00022335 File Offset: 0x00020535
		public override TextObject Name
		{
			get
			{
				return new TextObject("{=zcQhNQ7i}Reach the prisoner ship", null);
			}
		}

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x0600057C RID: 1404 RVA: 0x00022342 File Offset: 0x00020542
		public override TextObject Description
		{
			get
			{
				return new TextObject("{=lXv922C6}Swim with Gunnar to the ship where the captives are held.", null);
			}
		}

		// Token: 0x0600057D RID: 1405 RVA: 0x0002234F File Offset: 0x0002054F
		public Quest5SwimObjective(Mission mission, Agent targetAgent, MissionShip targetShip)
			: base(mission)
		{
			this._targetShip = targetShip;
			this._target = new Quest5SwimObjective.SwimObjectiveTarget(targetShip);
			base.AddTarget(this._target);
		}

		// Token: 0x0600057E RID: 1406 RVA: 0x00022377 File Offset: 0x00020577
		protected override bool IsActivationRequirementsMet()
		{
			return this._target != null;
		}

		// Token: 0x0600057F RID: 1407 RVA: 0x00022382 File Offset: 0x00020582
		protected override bool IsCompletionRequirementsMet()
		{
			return this._target != null && this._targetShip.GetIsAgentOnShip(Agent.Main, false);
		}

		// Token: 0x040002B4 RID: 692
		private Quest5SwimObjective.SwimObjectiveTarget _target;

		// Token: 0x040002B5 RID: 693
		private MissionShip _targetShip;

		// Token: 0x020001CD RID: 461
		private class SwimObjectiveTarget : MissionObjectiveTarget
		{
			// Token: 0x06001A19 RID: 6681 RVA: 0x000AE7A0 File Offset: 0x000AC9A0
			public SwimObjectiveTarget(MissionShip target)
			{
				this._target = target;
			}

			// Token: 0x06001A1A RID: 6682 RVA: 0x000AE7AF File Offset: 0x000AC9AF
			public override TextObject GetName()
			{
				return new TextObject("{=4hW7wMrj}Prisoner ship", null);
			}

			// Token: 0x06001A1B RID: 6683 RVA: 0x000AE7BC File Offset: 0x000AC9BC
			public override Vec3 GetGlobalPosition()
			{
				return this._target.ClimbingMachines.First<ClimbingMachine>().GameEntity.GlobalPosition + Vec3.Up;
			}

			// Token: 0x06001A1C RID: 6684 RVA: 0x000AE7F0 File Offset: 0x000AC9F0
			public override bool IsActive()
			{
				return true;
			}

			// Token: 0x04000D44 RID: 3396
			private readonly MissionShip _target;
		}
	}
}
