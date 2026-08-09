using System;
using NavalDLC.Missions.MissionLogics;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.AI.TeamAI
{
	// Token: 0x020000EF RID: 239
	public class TeamAINavalRaidAttackerComponent : TeamAIComponent
	{
		// Token: 0x1700032B RID: 811
		// (get) Token: 0x06001235 RID: 4661 RVA: 0x00084188 File Offset: 0x00082388
		// (set) Token: 0x06001236 RID: 4662 RVA: 0x00084190 File Offset: 0x00082390
		public NavalQuerySystem TeamNavalQuerySystem { get; protected set; }

		// Token: 0x1700032C RID: 812
		// (get) Token: 0x06001237 RID: 4663 RVA: 0x00084199 File Offset: 0x00082399
		public bool UseSpawnPathApproachPosition
		{
			get
			{
				return this._isRiverBattle && this._spawnPathData.IsValid;
			}
		}

		// Token: 0x06001238 RID: 4664 RVA: 0x000841B0 File Offset: 0x000823B0
		public TeamAINavalRaidAttackerComponent(Mission currentMission, Team currentTeam, float thinkTimerTime, float applyTimerTime)
			: base(currentMission, currentTeam, thinkTimerTime, applyTimerTime)
		{
			this.TeamNavalQuerySystem = new NavalQuerySystem(currentTeam);
			this.Team.DisableDetachmentTicking();
			this._isRiverBattle = Mission.Current.Scene.GetNavmeshFaceCountBetweenTwoIds(1, 1) > 0;
		}

		// Token: 0x06001239 RID: 4665 RVA: 0x000841F0 File Offset: 0x000823F0
		public override void OnUnitAddedToFormationForTheFirstTime(Formation formation)
		{
			if (GameNetwork.IsServer)
			{
				formation.ForceCalculateCaches();
				if (formation.AI.GetBehavior<BehaviorCharge>() == null)
				{
					if (formation.FormationIndex == 8)
					{
						formation.AI.AddAiBehavior(new BehaviorGeneral(formation));
					}
					else if (formation.FormationIndex == 9)
					{
						formation.AI.AddAiBehavior(new BehaviorProtectGeneral(formation));
					}
					formation.AI.AddAiBehavior(new BehaviorCharge(formation));
					formation.AI.AddAiBehavior(new BehaviorPullBack(formation));
					formation.AI.AddAiBehavior(new BehaviorRegroup(formation));
					formation.AI.AddAiBehavior(new BehaviorReserve(formation));
					formation.AI.AddAiBehavior(new BehaviorRetreat(formation));
					formation.AI.AddAiBehavior(new BehaviorStop(formation));
					formation.AI.AddAiBehavior(new BehaviorTacticalCharge(formation));
					formation.AI.AddAiBehavior(new BehaviorSergeantMPInfantry(formation));
					formation.AI.AddAiBehavior(new BehaviorSergeantMPLastFlagLastStand(formation));
					formation.AI.AddAiBehavior(new BehaviorSergeantMPMounted(formation));
					formation.AI.AddAiBehavior(new BehaviorSergeantMPMountedRanged(formation));
					formation.AI.AddAiBehavior(new BehaviorSergeantMPRanged(formation));
					return;
				}
			}
			else if (!GameNetwork.IsClientOrReplay)
			{
				formation.ForceCalculateCaches();
				if (formation.AI.GetBehavior<BehaviorCharge>() == null)
				{
					if (formation.FormationIndex == 8)
					{
						formation.AI.AddAiBehavior(new BehaviorGeneral(formation));
					}
					else if (formation.FormationIndex == 9)
					{
						formation.AI.AddAiBehavior(new BehaviorProtectGeneral(formation));
					}
					formation.AI.AddAiBehavior(new BehaviorCharge(formation));
					formation.AI.AddAiBehavior(new BehaviorPullBack(formation));
					formation.AI.AddAiBehavior(new BehaviorRegroup(formation));
					formation.AI.AddAiBehavior(new BehaviorReserve(formation));
					formation.AI.AddAiBehavior(new BehaviorRetreat(formation));
					formation.AI.AddAiBehavior(new BehaviorStop(formation));
					formation.AI.AddAiBehavior(new BehaviorTacticalCharge(formation));
					formation.AI.AddAiBehavior(new BehaviorAdvance(formation));
					formation.AI.AddAiBehavior(new BehaviorCautiousAdvance(formation));
					formation.AI.AddAiBehavior(new BehaviorCavalryScreen(formation));
					formation.AI.AddAiBehavior(new BehaviorDefend(formation));
					formation.AI.AddAiBehavior(new BehaviorDefensiveRing(formation));
					formation.AI.AddAiBehavior(new BehaviorFireFromInfantryCover(formation));
					formation.AI.AddAiBehavior(new BehaviorFlank(formation));
					formation.AI.AddAiBehavior(new BehaviorHoldHighGround(formation));
					formation.AI.AddAiBehavior(new BehaviorHorseArcherSkirmish(formation));
					formation.AI.AddAiBehavior(new BehaviorMountedSkirmish(formation));
					formation.AI.AddAiBehavior(new BehaviorProtectFlank(formation));
					formation.AI.AddAiBehavior(new BehaviorScreenedSkirmish(formation));
					formation.AI.AddAiBehavior(new BehaviorSkirmish(formation));
					formation.AI.AddAiBehavior(new BehaviorSkirmishBehindFormation(formation));
					formation.AI.AddAiBehavior(new BehaviorSkirmishLine(formation));
					formation.AI.AddAiBehavior(new BehaviorVanguard(formation));
					formation.AI.AddAiBehavior(new BehaviorShootFromCliff(formation));
				}
			}
		}

		// Token: 0x0600123A RID: 4666 RVA: 0x00084510 File Offset: 0x00082710
		public override void OnDeploymentFinished()
		{
			foreach (Formation formation in this.Team.FormationsIncludingEmpty)
			{
				formation.OnDeploymentFinished();
			}
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			if (Mission.Current.IsBattleSpawnPathSelectorInitialized)
			{
				this._spawnPathData = Mission.Current.GetInitialSpawnPathData(this.Team.Side);
			}
		}

		// Token: 0x04000A34 RID: 2612
		private readonly bool _isRiverBattle;

		// Token: 0x04000A35 RID: 2613
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x04000A36 RID: 2614
		private SpawnPathData _spawnPathData;
	}
}
