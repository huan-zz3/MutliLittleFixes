using System;
using System.Linq;
using System.Runtime.CompilerServices;
using NavalDLC.Missions.MissionLogics;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.Deployment
{
	// Token: 0x020000DE RID: 222
	public class NavalMissionDeploymentPlanningLogic : MissionDeploymentPlanningLogic
	{
		// Token: 0x06001165 RID: 4453 RVA: 0x00080B49 File Offset: 0x0007ED49
		public NavalMissionDeploymentPlanningLogic(Mission mission)
		{
			this._mission = mission;
		}

		// Token: 0x06001166 RID: 4454 RVA: 0x00080B64 File Offset: 0x0007ED64
		public override void Initialize()
		{
			this._teamDeploymentPlans.Clear();
			foreach (Team team in this._mission.Teams)
			{
				NavalTeamDeploymentPlan navalTeamDeploymentPlan = new NavalTeamDeploymentPlan(this._mission, team);
				this._teamDeploymentPlans.Add(new ValueTuple<Team, NavalTeamDeploymentPlan>(team, navalTeamDeploymentPlan));
			}
		}

		// Token: 0x06001167 RID: 4455 RVA: 0x00080BE0 File Offset: 0x0007EDE0
		public override void ClearDeploymentPlan(Team team)
		{
			this.GetTeamPlan(team).ClearPlan(false);
		}

		// Token: 0x06001168 RID: 4456 RVA: 0x00080BEF File Offset: 0x0007EDEF
		public override bool SupportsReinforcements()
		{
			return false;
		}

		// Token: 0x06001169 RID: 4457 RVA: 0x00080BF2 File Offset: 0x0007EDF2
		public override void UpdateReinforcementPlan(Team team)
		{
			Debug.FailedAssert("Naval mission deployment planning logic does not support reinforcements plans that can be updated", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Missions\\Deployment\\NavalMissionDeploymentPlanningLogic.cs", "UpdateReinforcementPlan", 43);
		}

		// Token: 0x0600116A RID: 4458 RVA: 0x00080C0A File Offset: 0x0007EE0A
		public override bool SupportsNavmesh(Team team)
		{
			return false;
		}

		// Token: 0x0600116B RID: 4459 RVA: 0x00080C0D File Offset: 0x0007EE0D
		public override bool HasPlayerSpawnFrame(BattleSideEnum battleSide)
		{
			return false;
		}

		// Token: 0x0600116C RID: 4460 RVA: 0x00080C10 File Offset: 0x0007EE10
		public override bool GetPlayerSpawnFrame(BattleSideEnum battleSide, out WorldPosition position, out Vec2 direction)
		{
			position = WorldPosition.Invalid;
			direction = Vec2.Invalid;
			return false;
		}

		// Token: 0x0600116D RID: 4461 RVA: 0x00080C29 File Offset: 0x0007EE29
		public void ClearAddedShips(Team team)
		{
			this.GetTeamPlan(team).ClearAddedShips();
		}

		// Token: 0x0600116E RID: 4462 RVA: 0x00080C38 File Offset: 0x0007EE38
		public override void ClearAll()
		{
			foreach (ValueTuple<Team, NavalTeamDeploymentPlan> valueTuple in this._teamDeploymentPlans)
			{
				valueTuple.Item2.ClearAddedShips();
				valueTuple.Item2.ClearPlan(false);
			}
		}

		// Token: 0x0600116F RID: 4463 RVA: 0x00080C9C File Offset: 0x0007EE9C
		public void AddShip(Team team, FormationClass formationIndex, IShipOrigin shipOrigin)
		{
			this.GetTeamPlan(team).AddShip(formationIndex, shipOrigin);
		}

		// Token: 0x06001170 RID: 4464 RVA: 0x00080CAC File Offset: 0x0007EEAC
		public bool RemoveShip(Team team, FormationClass formationIndex)
		{
			return this.GetTeamPlan(team).RemoveShip(formationIndex);
		}

		// Token: 0x06001171 RID: 4465 RVA: 0x00080CBC File Offset: 0x0007EEBC
		public override void MakeDeploymentPlan(Team team, float spawnPathOffset = 0f, float targetOffset = 0f)
		{
			NavalTeamDeploymentPlan teamPlan = this.GetTeamPlan(team);
			if (!this.IsPlanMade(team))
			{
				teamPlan.MakeDeploymentPlan(spawnPathOffset, targetOffset, null, false);
				bool flag;
				if (this.IsPlanMade(team, ref flag))
				{
					this._mission.OnDeploymentPlanMade(team, flag);
				}
			}
		}

		// Token: 0x06001172 RID: 4466 RVA: 0x00080CFC File Offset: 0x0007EEFC
		public override bool RemakeDeploymentPlan(Team team)
		{
			this.IsPlanMade(team);
			float spawnPathOffset = this.GetSpawnPathOffset(team);
			float targetOffset = this.GetTargetOffset(team);
			this.ClearAddedShips(team);
			this.ClearDeploymentPlan(team);
			NavalShipsLogic missionBehavior = this._mission.GetMissionBehavior<NavalShipsLogic>();
			for (int i = 0; i < 11; i++)
			{
				FormationClass formationClass = i;
				ShipAssignment shipAssignment = missionBehavior.GetShipAssignment(team.TeamSide, formationClass);
				if (shipAssignment.IsSet)
				{
					this.AddShip(team, formationClass, shipAssignment.ShipOrigin);
				}
			}
			this.MakeDeploymentPlan(team, spawnPathOffset, targetOffset);
			return this.IsPlanMade(team);
		}

		// Token: 0x06001173 RID: 4467 RVA: 0x00080D84 File Offset: 0x0007EF84
		public override bool IsPositionInsideDeploymentBoundaries(Team team, in Vec2 position)
		{
			ValueTuple<string, MBList<Vec2>> valueTuple;
			return this.GetTeamPlan(team).IsPositionInsideDeploymentBoundaries(in position, out valueTuple);
		}

		// Token: 0x06001174 RID: 4468 RVA: 0x00080DA0 File Offset: 0x0007EFA0
		public override Vec2 GetClosestDeploymentBoundaryPosition(Team team, in Vec2 position)
		{
			return this.GetTeamPlan(team).GetClosestDeploymentBoundaryPosition(in position);
		}

		// Token: 0x06001175 RID: 4469 RVA: 0x00080DAF File Offset: 0x0007EFAF
		public override void ProjectPositionToDeploymentBoundaries(Team team, ref WorldPosition position)
		{
			Debug.FailedAssert("Naval deployment plan does not support projection of position to deployment boundaries as it does not support a navmesh", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Missions\\Deployment\\NavalMissionDeploymentPlanningLogic.cs", "ProjectPositionToDeploymentBoundaries", 161);
		}

		// Token: 0x06001176 RID: 4470 RVA: 0x00080DCA File Offset: 0x0007EFCA
		public override bool GetPathDeploymentBoundaryIntersection(Team team, in WorldPosition startPosition, in WorldPosition endPosition, out WorldPosition intersection)
		{
			Debug.FailedAssert("Naval deployment plan does not support finding boundary intersection between positions as it does not support a navmesh", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Missions\\Deployment\\NavalMissionDeploymentPlanningLogic.cs", "GetPathDeploymentBoundaryIntersection", 166);
			intersection = WorldPosition.Invalid;
			return false;
		}

		// Token: 0x06001177 RID: 4471 RVA: 0x00080DF2 File Offset: 0x0007EFF2
		public override float GetSpawnPathOffset(Team team)
		{
			return this.GetTeamPlan(team).GetSpawnPathOffset(false);
		}

		// Token: 0x06001178 RID: 4472 RVA: 0x00080E04 File Offset: 0x0007F004
		public override MatrixFrame GetZoomFocusFrame(Team team)
		{
			NavalTeamDeploymentPlan teamPlan = this.GetTeamPlan(team);
			MatrixFrame deploymentFrame = teamPlan.GetDeploymentFrame();
			Vec3 vec = Vec3.Zero;
			int num = 0;
			for (int i = 0; i < 11; i++)
			{
				IFormationDeploymentPlan formationPlan = teamPlan.GetFormationPlan(i, false);
				if (formationPlan.HasFrame())
				{
					MatrixFrame frame = formationPlan.GetFrame();
					vec += frame.origin;
					num++;
				}
			}
			vec /= (float)num;
			deploymentFrame.origin = vec;
			return deploymentFrame;
		}

		// Token: 0x06001179 RID: 4473 RVA: 0x00080E7C File Offset: 0x0007F07C
		public override float GetZoomOffset(Team team, float fovAngle)
		{
			NavalTeamDeploymentPlan teamPlan = this.GetTeamPlan(team);
			MatrixFrame deploymentFrame = teamPlan.GetDeploymentFrame();
			float num = float.MinValue;
			for (int i = 0; i < 11; i++)
			{
				IFormationDeploymentPlan formationPlan = teamPlan.GetFormationPlan(i, false);
				if (formationPlan.HasFrame())
				{
					float num2 = formationPlan.GetFrame().origin.AsVec2.DistanceSquared(deploymentFrame.origin.AsVec2);
					num = MathF.Max(num, num2);
				}
			}
			return (MathF.Sqrt(num) + 20f) / MathF.Max(MathF.Tan(fovAngle / 2f), 0.01f);
		}

		// Token: 0x0600117A RID: 4474 RVA: 0x00080F16 File Offset: 0x0007F116
		public override IFormationDeploymentPlan GetFormationPlan(Team team, FormationClass fClass, bool isReinforcement = false)
		{
			return this.GetTeamPlan(team).GetFormationPlan(fClass, false);
		}

		// Token: 0x0600117B RID: 4475 RVA: 0x00080F28 File Offset: 0x0007F128
		public override bool IsPlanMade(Team team)
		{
			NavalTeamDeploymentPlan teamPlanAux = this.GetTeamPlanAux(team);
			return teamPlanAux != null && teamPlanAux.IsPlanMade(false);
		}

		// Token: 0x0600117C RID: 4476 RVA: 0x00080F4C File Offset: 0x0007F14C
		public override bool IsPlanMade(Team team, out bool isFirstPlan)
		{
			isFirstPlan = false;
			NavalTeamDeploymentPlan teamPlanAux = this.GetTeamPlanAux(team);
			if (teamPlanAux != null && teamPlanAux.IsPlanMade(false))
			{
				isFirstPlan = teamPlanAux.IsFirstPlan(false);
				return true;
			}
			return false;
		}

		// Token: 0x0600117D RID: 4477 RVA: 0x00080F7C File Offset: 0x0007F17C
		public override bool HasDeploymentBoundaries(Team team)
		{
			NavalTeamDeploymentPlan teamPlanAux = this.GetTeamPlanAux(team);
			return teamPlanAux != null && teamPlanAux.HasDeploymentBoundaries();
		}

		// Token: 0x0600117E RID: 4478 RVA: 0x00080F9C File Offset: 0x0007F19C
		public override MatrixFrame GetDeploymentFrame(Team team)
		{
			return this.GetTeamPlan(team).GetDeploymentFrame();
		}

		// Token: 0x0600117F RID: 4479 RVA: 0x00080FAA File Offset: 0x0007F1AA
		public float GetTargetOffset(Team team)
		{
			return this.GetTeamPlan(team).GetTargetOffset(false);
		}

		// Token: 0x06001180 RID: 4480 RVA: 0x00080FB9 File Offset: 0x0007F1B9
		public override MBReadOnlyList<ValueTuple<string, MBList<Vec2>>> GetDeploymentBoundaries(Team team)
		{
			return this.GetTeamPlan(team).GetDeploymentBoundaries();
		}

		// Token: 0x06001181 RID: 4481 RVA: 0x00080FC8 File Offset: 0x0007F1C8
		public virtual bool GetMeanBoundaryPosition(Team team, out Vec2 meanPosition, int boundaryIndex = 0)
		{
			NavalTeamDeploymentPlan teamPlan = this.GetTeamPlan(team);
			if (teamPlan.HasDeploymentBoundaries())
			{
				meanPosition = teamPlan.GetMeanBoundaryPosition(boundaryIndex);
				return true;
			}
			meanPosition = Vec2.Invalid;
			return false;
		}

		// Token: 0x06001182 RID: 4482 RVA: 0x00081000 File Offset: 0x0007F200
		private NavalTeamDeploymentPlan GetTeamPlan(Team team)
		{
			return this.GetTeamPlanAux(team);
		}

		// Token: 0x06001183 RID: 4483 RVA: 0x0008100C File Offset: 0x0007F20C
		private NavalTeamDeploymentPlan GetTeamPlanAux(Team team)
		{
			return this._teamDeploymentPlans.FirstOrDefault<ValueTuple<Team, NavalTeamDeploymentPlan>>(([TupleElementNames(new string[] { "team", "plan" })] ValueTuple<Team, NavalTeamDeploymentPlan> tdp) => tdp.Item1 == team).Item2;
		}

		// Token: 0x04000A08 RID: 2568
		private Mission _mission;

		// Token: 0x04000A09 RID: 2569
		[TupleElementNames(new string[] { "team", "plan" })]
		private MBList<ValueTuple<Team, NavalTeamDeploymentPlan>> _teamDeploymentPlans = new MBList<ValueTuple<Team, NavalTeamDeploymentPlan>>();
	}
}
