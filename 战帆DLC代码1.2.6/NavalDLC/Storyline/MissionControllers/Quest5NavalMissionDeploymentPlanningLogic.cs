using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NavalDLC.Missions.Deployment;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Storyline.MissionControllers
{
	// Token: 0x0200006E RID: 110
	public class Quest5NavalMissionDeploymentPlanningLogic : NavalMissionDeploymentPlanningLogic
	{
		// Token: 0x0600069D RID: 1693 RVA: 0x0002809B File Offset: 0x0002629B
		public Quest5NavalMissionDeploymentPlanningLogic(Mission mission)
			: base(mission)
		{
			this._mission = mission;
		}

		// Token: 0x0600069E RID: 1694 RVA: 0x000280B8 File Offset: 0x000262B8
		public override void Initialize()
		{
			this._teamDeploymentPlans.Clear();
			foreach (Team team in this._mission.Teams)
			{
				DefaultDeploymentPlan defaultDeploymentPlan = DefaultDeploymentPlan.CreateInitialPlan(this._mission, team);
				this._teamDeploymentPlans.Add(new ValueTuple<Team, DefaultDeploymentPlan>(team, defaultDeploymentPlan));
			}
		}

		// Token: 0x0600069F RID: 1695 RVA: 0x00028134 File Offset: 0x00026334
		public override void ClearDeploymentPlan(Team team)
		{
			this.GetTeamPlan(team).ClearPlan();
		}

		// Token: 0x060006A0 RID: 1696 RVA: 0x00028142 File Offset: 0x00026342
		public override bool SupportsReinforcements()
		{
			return false;
		}

		// Token: 0x060006A1 RID: 1697 RVA: 0x00028145 File Offset: 0x00026345
		public override bool SupportsNavmesh(Team team)
		{
			return false;
		}

		// Token: 0x060006A2 RID: 1698 RVA: 0x00028148 File Offset: 0x00026348
		public override bool HasPlayerSpawnFrame(BattleSideEnum battleSide)
		{
			return false;
		}

		// Token: 0x060006A3 RID: 1699 RVA: 0x0002814B File Offset: 0x0002634B
		public override bool GetPlayerSpawnFrame(BattleSideEnum battleSide, out WorldPosition position, out Vec2 direction)
		{
			position = WorldPosition.Invalid;
			direction = Vec2.Invalid;
			return false;
		}

		// Token: 0x060006A4 RID: 1700 RVA: 0x00028164 File Offset: 0x00026364
		public new void ClearAddedShips(Team team)
		{
		}

		// Token: 0x060006A5 RID: 1701 RVA: 0x00028168 File Offset: 0x00026368
		public override void ClearAll()
		{
			foreach (ValueTuple<Team, DefaultDeploymentPlan> valueTuple in this._teamDeploymentPlans)
			{
				valueTuple.Item2.ClearPlan();
			}
		}

		// Token: 0x060006A6 RID: 1702 RVA: 0x000281C0 File Offset: 0x000263C0
		public new void AddShip(Team team, FormationClass formationIndex, IShipOrigin shipOrigin)
		{
		}

		// Token: 0x060006A7 RID: 1703 RVA: 0x000281C2 File Offset: 0x000263C2
		public new bool RemoveShip(Team team, FormationClass formationIndex)
		{
			return true;
		}

		// Token: 0x060006A8 RID: 1704 RVA: 0x000281C5 File Offset: 0x000263C5
		public override void MakeDeploymentPlan(Team team, float spawnPathOffset = 0f, float targetOffset = 0f)
		{
		}

		// Token: 0x060006A9 RID: 1705 RVA: 0x000281C7 File Offset: 0x000263C7
		public override bool RemakeDeploymentPlan(Team team)
		{
			return true;
		}

		// Token: 0x060006AA RID: 1706 RVA: 0x000281CA File Offset: 0x000263CA
		public override bool IsPositionInsideDeploymentBoundaries(Team team, in Vec2 position)
		{
			return true;
		}

		// Token: 0x060006AB RID: 1707 RVA: 0x000281CD File Offset: 0x000263CD
		public override Vec2 GetClosestDeploymentBoundaryPosition(Team team, in Vec2 position)
		{
			return position;
		}

		// Token: 0x060006AC RID: 1708 RVA: 0x000281D5 File Offset: 0x000263D5
		public override void ProjectPositionToDeploymentBoundaries(Team team, ref WorldPosition position)
		{
		}

		// Token: 0x060006AD RID: 1709 RVA: 0x000281D7 File Offset: 0x000263D7
		public override bool GetPathDeploymentBoundaryIntersection(Team team, in WorldPosition startPosition, in WorldPosition endPosition, out WorldPosition intersection)
		{
			intersection = WorldPosition.Invalid;
			return true;
		}

		// Token: 0x060006AE RID: 1710 RVA: 0x000281E6 File Offset: 0x000263E6
		public override float GetSpawnPathOffset(Team team)
		{
			return 1f;
		}

		// Token: 0x060006AF RID: 1711 RVA: 0x000281ED File Offset: 0x000263ED
		public override MatrixFrame GetZoomFocusFrame(Team team)
		{
			return MatrixFrame.Identity;
		}

		// Token: 0x060006B0 RID: 1712 RVA: 0x000281F4 File Offset: 0x000263F4
		public override float GetZoomOffset(Team team, float fovAngle)
		{
			return 1f;
		}

		// Token: 0x060006B1 RID: 1713 RVA: 0x000281FB File Offset: 0x000263FB
		public override IFormationDeploymentPlan GetFormationPlan(Team team, FormationClass fClass, bool isReinforcement = false)
		{
			if (!isReinforcement)
			{
				return this.GetTeamPlan(team).GetFormationPlan(fClass);
			}
			Debug.FailedAssert("Reinforcement plans are not supported by naval deployment plans", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Storyline\\MissionControllers\\Quest5NavalMissionDeploymentPlanningLogic.cs", "GetFormationPlan", 149);
			return null;
		}

		// Token: 0x060006B2 RID: 1714 RVA: 0x00028228 File Offset: 0x00026428
		public override bool IsPlanMade(Team team)
		{
			return this.GetTeamPlan(team) != null;
		}

		// Token: 0x060006B3 RID: 1715 RVA: 0x00028234 File Offset: 0x00026434
		public override bool IsPlanMade(Team team, out bool isFirstPlan)
		{
			isFirstPlan = false;
			if (this.GetTeamPlan(team) != null)
			{
				isFirstPlan = true;
				return true;
			}
			return false;
		}

		// Token: 0x060006B4 RID: 1716 RVA: 0x00028248 File Offset: 0x00026448
		public override bool HasDeploymentBoundaries(Team team)
		{
			return false;
		}

		// Token: 0x060006B5 RID: 1717 RVA: 0x0002824B File Offset: 0x0002644B
		public override MatrixFrame GetDeploymentFrame(Team team)
		{
			return MatrixFrame.Identity;
		}

		// Token: 0x060006B6 RID: 1718 RVA: 0x00028252 File Offset: 0x00026452
		public new float GetTargetOffset(Team team)
		{
			return this.GetTeamPlan(team).TargetOffset;
		}

		// Token: 0x060006B7 RID: 1719 RVA: 0x00028260 File Offset: 0x00026460
		public override MBReadOnlyList<ValueTuple<string, MBList<Vec2>>> GetDeploymentBoundaries(Team team)
		{
			return null;
		}

		// Token: 0x060006B8 RID: 1720 RVA: 0x00028263 File Offset: 0x00026463
		public override bool GetMeanBoundaryPosition(Team team, out Vec2 meanPosition, int boundaryIndex = 0)
		{
			meanPosition = Vec2.Invalid;
			return true;
		}

		// Token: 0x060006B9 RID: 1721 RVA: 0x00028274 File Offset: 0x00026474
		private DefaultDeploymentPlan GetTeamPlan(Team team)
		{
			DefaultDeploymentPlan defaultDeploymentPlan = this._teamDeploymentPlans.FirstOrDefault<ValueTuple<Team, DefaultDeploymentPlan>>(([TupleElementNames(new string[] { "team", "plan" })] ValueTuple<Team, DefaultDeploymentPlan> t) => t.Item1 == team).Item2;
			if (defaultDeploymentPlan == null)
			{
				defaultDeploymentPlan = DefaultDeploymentPlan.CreateInitialPlan(this._mission, team);
				this._teamDeploymentPlans.Add(new ValueTuple<Team, DefaultDeploymentPlan>(team, defaultDeploymentPlan));
			}
			return defaultDeploymentPlan;
		}

		// Token: 0x04000362 RID: 866
		private Mission _mission;

		// Token: 0x04000363 RID: 867
		[TupleElementNames(new string[] { "team", "plan" })]
		private List<ValueTuple<Team, DefaultDeploymentPlan>> _teamDeploymentPlans = new List<ValueTuple<Team, DefaultDeploymentPlan>>();
	}
}
