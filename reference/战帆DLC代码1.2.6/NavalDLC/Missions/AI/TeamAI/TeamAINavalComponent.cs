using System;
using NavalDLC.Missions.AI.Behaviors;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.NavalPhysics;
using NavalDLC.Missions.Objects;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.AI.TeamAI
{
	// Token: 0x020000EE RID: 238
	public class TeamAINavalComponent : TeamAIComponent
	{
		// Token: 0x17000329 RID: 809
		// (get) Token: 0x0600122C RID: 4652 RVA: 0x00083EFC File Offset: 0x000820FC
		// (set) Token: 0x0600122D RID: 4653 RVA: 0x00083F04 File Offset: 0x00082104
		public NavalQuerySystem TeamNavalQuerySystem { get; protected set; }

		// Token: 0x1700032A RID: 810
		// (get) Token: 0x0600122E RID: 4654 RVA: 0x00083F0D File Offset: 0x0008210D
		public bool UseSpawnPathApproachPosition
		{
			get
			{
				return this._isRiverBattle && this._spawnPathData != null && this._spawnPathData.IsValid;
			}
		}

		// Token: 0x0600122F RID: 4655 RVA: 0x00083F2C File Offset: 0x0008212C
		public TeamAINavalComponent(Mission currentMission, Team currentTeam, float thinkTimerTime, float applyTimerTime)
			: base(currentMission, currentTeam, thinkTimerTime, applyTimerTime)
		{
			this.TeamNavalQuerySystem = new NavalQuerySystem(currentTeam);
			NavalOrderController navalOrderController = new NavalOrderController(this.Mission, this.Team, null);
			NavalOrderController navalOrderController2 = new NavalOrderController(this.Mission, this.Team, (this.Team.IsPlayerTeam && this.Team.IsPlayerGeneral) ? Mission.Current.MainAgent : null);
			this.Team.SetCustomOrderController(navalOrderController, navalOrderController2);
			this.Team.DisableDetachmentTicking();
			this._isRiverBattle = Mission.Current.Scene.GetNavmeshFaceCountBetweenTwoIds(1, 1) > 0;
		}

		// Token: 0x06001230 RID: 4656 RVA: 0x00083FD0 File Offset: 0x000821D0
		public override void OnUnitAddedToFormationForTheFirstTime(Formation formation)
		{
			if (formation.AI.GetBehavior<BehaviorNavalRemoveConnection>() == null)
			{
				formation.ForceCalculateCaches();
				formation.AI.AddAiBehavior(new BehaviorNavalRemoveConnection(formation));
				formation.AI.AddAiBehavior(new BehaviorNavalEngageCorrespondingEnemy(formation));
				formation.AI.AddAiBehavior(new BehaviorNavalDefendInLine(formation));
				formation.AI.AddAiBehavior(new BehaviorNavalSkirmish(formation));
				formation.AI.AddAiBehavior(new BehaviorNavalRamming(formation));
				formation.AI.AddAiBehavior(new BehaviorNavalApproachInLine(formation));
			}
		}

		// Token: 0x06001231 RID: 4657 RVA: 0x00084058 File Offset: 0x00082258
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

		// Token: 0x06001232 RID: 4658 RVA: 0x000840E4 File Offset: 0x000822E4
		public Formation GetConnectedAllyFormation(ulong shipUniqueBitwiseID)
		{
			MissionShip connectedTeamShip = this._navalShipsLogic.GetConnectedTeamShip(this.Team.TeamSide, shipUniqueBitwiseID);
			if (connectedTeamShip == null)
			{
				return null;
			}
			return connectedTeamShip.Formation;
		}

		// Token: 0x06001233 RID: 4659 RVA: 0x00084108 File Offset: 0x00082308
		public Formation GetNearestAllyShipFormation(Agent agent)
		{
			Vec3 origin = agent.Frame.origin;
			MissionShip nearestTeamShip = this._navalShipsLogic.GetNearestTeamShip(this.Team.TeamSide, in origin, float.MaxValue, (MissionShip ship) => ship.Physics.NavalSinkingState == NavalPhysics.SinkingState.Floating && !ship.BeingAbandoned);
			if (nearestTeamShip == null)
			{
				return null;
			}
			return nearestTeamShip.Formation;
		}

		// Token: 0x06001234 RID: 4660 RVA: 0x00084168 File Offset: 0x00082368
		public void GetRiverApproachPosition(out Vec2 position, out Vec2 direction)
		{
			this._spawnPathData.GetSpawnPathFrameFacingTarget(0f, 1f, false, ref position, ref direction, false, 0.2f);
		}

		// Token: 0x04000A30 RID: 2608
		private readonly bool _isRiverBattle;

		// Token: 0x04000A31 RID: 2609
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x04000A32 RID: 2610
		private SpawnPathData _spawnPathData;
	}
}
