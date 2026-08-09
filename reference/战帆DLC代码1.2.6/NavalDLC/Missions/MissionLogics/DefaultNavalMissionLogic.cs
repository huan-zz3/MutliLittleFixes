using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NavalDLC.Missions.Deployment;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.ShipActuators;
using NavalDLC.Missions.ShipControl;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.MissionLogics
{
	// Token: 0x020000C8 RID: 200
	public class DefaultNavalMissionLogic : MissionLogic, IAgentStateDecider, IMissionBehavior
	{
		// Token: 0x170002B9 RID: 697
		// (get) Token: 0x06000EED RID: 3821 RVA: 0x00074420 File Offset: 0x00072620
		public MBReadOnlyList<IShipOrigin> PlayerShips
		{
			get
			{
				return this._playerTeamShips;
			}
		}

		// Token: 0x170002BA RID: 698
		// (get) Token: 0x06000EEE RID: 3822 RVA: 0x00074428 File Offset: 0x00072628
		public MBReadOnlyList<IShipOrigin> PlayerAllyShips
		{
			get
			{
				return this._playerAllyTeamShips;
			}
		}

		// Token: 0x170002BB RID: 699
		// (get) Token: 0x06000EEF RID: 3823 RVA: 0x00074430 File Offset: 0x00072630
		public MBReadOnlyList<IShipOrigin> PlayerEnemyShips
		{
			get
			{
				return this._enemyTeamShips;
			}
		}

		// Token: 0x06000EF0 RID: 3824 RVA: 0x00074438 File Offset: 0x00072638
		public override void OnMissionStateFinalized()
		{
			SailWindProfile.FinalizeProfile();
		}

		// Token: 0x06000EF1 RID: 3825 RVA: 0x00074440 File Offset: 0x00072640
		public override void OnDeploymentFinished()
		{
			foreach (MissionShip missionShip in this._shipsLogic.AllShips)
			{
				missionShip.SetAnchor(false, false, 1f);
				if (!missionShip.IsPlayerShip)
				{
					missionShip.SetController(ShipControllerType.AI, true);
				}
			}
			this._shipsLogic.SetDeploymentMode(false);
		}

		// Token: 0x06000EF2 RID: 3826 RVA: 0x000744BC File Offset: 0x000726BC
		internal void DeployBattleSide(BattleSideEnum battleSide)
		{
			this.MakeDeploymentPlansForSide(battleSide);
			foreach (Team team in Mission.GetTeamsOfSide(battleSide))
			{
				foreach (Formation formation in team.FormationsIncludingEmpty)
				{
					FormationClass formationIndex = formation.FormationIndex;
					IFormationDeploymentPlan formationPlan = this._deploymentPlan.GetFormationPlan(team, formationIndex, false);
					if (formationPlan.HasFrame())
					{
						MatrixFrame frame = formationPlan.GetFrame();
						this._shipsLogic.SpawnShip(formation, in frame, true, false).SetController(ShipControllerType.None, true);
					}
				}
			}
		}

		// Token: 0x06000EF3 RID: 3827 RVA: 0x00074588 File Offset: 0x00072788
		public DefaultNavalMissionLogic(MBList<IShipOrigin> playerShips, MBList<IShipOrigin> playerAllyShips, MBList<IShipOrigin> enemyShips, NavalShipDeploymentLimit playerTeamShipDeploymentLimit, NavalShipDeploymentLimit playerAllyTeamShipDeploymentLimit, NavalShipDeploymentLimit enemyTeamShipDeploymentLimit)
		{
			this._playerTeamShips = playerShips;
			this._playerAllyTeamShips = playerAllyShips;
			this._enemyTeamShips = enemyShips;
			this._playerTeamShipDeploymentLimit = playerTeamShipDeploymentLimit;
			this._playerAllyTeamShipDeploymentLimit = playerAllyTeamShipDeploymentLimit;
			this._enemyTeamShipDeploymentLimit = enemyTeamShipDeploymentLimit;
		}

		// Token: 0x06000EF4 RID: 3828 RVA: 0x000745BD File Offset: 0x000727BD
		public override void AfterStart()
		{
			base.AfterStart();
			this._deploymentPlan = base.Mission.GetMissionBehavior<NavalMissionDeploymentPlanningLogic>();
			DefaultNavalMissionLogic.UpdateSceneWindDirection();
			if (base.Mission.TerrainType != 11)
			{
				DefaultNavalMissionLogic.UpdateSceneWaterStrength();
			}
			this.InitializeShipAssignments();
		}

		// Token: 0x06000EF5 RID: 3829 RVA: 0x000745F8 File Offset: 0x000727F8
		public override void OnBehaviorInitialize()
		{
			if (!SailWindProfile.IsSailWindProfileInitialized)
			{
				SailWindProfile.InitializeProfile();
			}
			this._shipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
			this._shipsLogic.SetDeploymentMode(true);
			this._shipsLogic.SetTeamShipDeploymentLimit(0, this._playerTeamShipDeploymentLimit);
			this._shipsLogic.SetTeamShipDeploymentLimit(1, this._playerAllyTeamShipDeploymentLimit);
			this._shipsLogic.SetTeamShipDeploymentLimit(2, this._enemyTeamShipDeploymentLimit);
			MissionGameModels.Current.BattleInitializationModel.InitializeModel();
		}

		// Token: 0x06000EF6 RID: 3830 RVA: 0x00074673 File Offset: 0x00072873
		public AgentState GetAgentState(Agent affectedAgent, float deathProbability, out bool usedSurgery)
		{
			return DefaultNavalMissionLogic.GetNavalAgentState(affectedAgent, deathProbability, out usedSurgery);
		}

		// Token: 0x06000EF7 RID: 3831 RVA: 0x00074680 File Offset: 0x00072880
		private void InitializeShipAssignments()
		{
			NavalAgentsLogic missionBehavior = base.Mission.GetMissionBehavior<NavalAgentsLogic>();
			this._shipsLogic.ClearShipAssignments();
			if (this._playerTeamShips.Count > 0)
			{
				int num = MathF.Min(this._playerTeamShipDeploymentLimit.NetDeploymentLimit, this._playerTeamShips.Count);
				num = MathF.Min(missionBehavior.GetTeamTroopOrigins(0).Count<IAgentOriginBase>(), num);
				foreach (ValueTuple<FormationClass, IShipOrigin> valueTuple in this.AssignShipsToFormations(this._playerTeamShips, num))
				{
					this._shipsLogic.SetShipAssignment(0, valueTuple.Item1, valueTuple.Item2);
				}
			}
			if (this._playerAllyTeamShips != null && this._playerAllyTeamShips.Count > 0)
			{
				int num2 = MathF.Min(this._playerAllyTeamShipDeploymentLimit.NetDeploymentLimit, this._playerAllyTeamShips.Count);
				num2 = MathF.Min(missionBehavior.GetTeamTroopOrigins(1).Count<IAgentOriginBase>(), num2);
				foreach (ValueTuple<FormationClass, IShipOrigin> valueTuple2 in this.AssignShipsToFormations(this._playerAllyTeamShips, num2))
				{
					this._shipsLogic.SetShipAssignment(1, valueTuple2.Item1, valueTuple2.Item2);
				}
			}
			if (this._enemyTeamShips.Count > 0)
			{
				int num3 = MathF.Min(this._enemyTeamShipDeploymentLimit.NetDeploymentLimit, this._enemyTeamShips.Count);
				num3 = MathF.Min(missionBehavior.GetTeamTroopOrigins(2).Count<IAgentOriginBase>(), num3);
				foreach (ValueTuple<FormationClass, IShipOrigin> valueTuple3 in this.AssignShipsToFormations(this._enemyTeamShips, num3))
				{
					this._shipsLogic.SetShipAssignment(2, valueTuple3.Item1, valueTuple3.Item2);
				}
			}
		}

		// Token: 0x06000EF8 RID: 3832 RVA: 0x000748A0 File Offset: 0x00072AA0
		private float GetTeamSpawnPathOffsetRange(Path initialSpawnPath, Team team)
		{
			float num = 0f;
			TeamSideEnum teamSide = team.TeamSide;
			for (int i = 0; i < 11; i++)
			{
				ShipAssignment shipAssignment = this._shipsLogic.GetShipAssignment(team.TeamSide, i);
				if (shipAssignment.IsSet)
				{
					num = Math.Max(shipAssignment.MissionShipObject.DeploymentArea.y, num);
				}
			}
			return 1.1f * num;
		}

		// Token: 0x06000EF9 RID: 3833 RVA: 0x00074900 File Offset: 0x00072B00
		[return: TupleElementNames(new string[] { "formationIndex", "ship" })]
		private List<ValueTuple<FormationClass, IShipOrigin>> AssignShipsToFormations(MBReadOnlyList<IShipOrigin> ships, int shipCount)
		{
			List<ValueTuple<FormationClass, IShipOrigin>> list = new List<ValueTuple<FormationClass, IShipOrigin>>();
			int num = 8;
			int num2 = 0;
			foreach (IShipOrigin shipOrigin in ships)
			{
				if (num2 >= num || num2 >= shipCount)
				{
					break;
				}
				list.Add(new ValueTuple<FormationClass, IShipOrigin>(num2, shipOrigin));
				num2++;
			}
			return list;
		}

		// Token: 0x06000EFA RID: 3834 RVA: 0x00074970 File Offset: 0x00072B70
		private void MakeDeploymentPlansForSide(BattleSideEnum battleSide)
		{
			MBReadOnlyList<ValueTuple<Team, int>> mbreadOnlyList = this.CollectSortedBattleSideTeamsData(battleSide);
			SpawnPathData initialSpawnPathData = Mission.Current.GetInitialSpawnPathData(battleSide);
			Path path = initialSpawnPathData.Path;
			float[] array = new float[mbreadOnlyList.Count];
			for (int i = 0; i < mbreadOnlyList.Count; i++)
			{
				Team item = mbreadOnlyList[i].Item1;
				this.AddTeamShipsToDeploymentPlan(item);
				array[i] = this.GetTeamSpawnPathOffsetRange(path, item);
			}
			float num = this._shipsLogic.ComputeSpawnPathDeploymentOffset(path);
			float num2;
			float num3;
			DefaultBattleMissionAgentSpawnLogic.ComputeDeploymentBaseOffsets(initialSpawnPathData, num, ref num2, ref num3);
			float[] array2;
			DefaultBattleMissionAgentSpawnLogic.ComputeTeamDeploymentOffsets(initialSpawnPathData, num2, 32f, array, ref array2);
			for (int j = 0; j < mbreadOnlyList.Count; j++)
			{
				this._deploymentPlan.MakeDeploymentPlan(mbreadOnlyList[j].Item1, array2[j], num3);
			}
		}

		// Token: 0x06000EFB RID: 3835 RVA: 0x00074A3C File Offset: 0x00072C3C
		private void AddTeamShipsToDeploymentPlan(Team team)
		{
			for (int i = 0; i < 11; i++)
			{
				ShipAssignment shipAssignment = this._shipsLogic.GetShipAssignment(team.TeamSide, i);
				if (shipAssignment.IsSet)
				{
					this._deploymentPlan.AddShip(team, shipAssignment.FormationIndex, shipAssignment.ShipOrigin);
				}
			}
		}

		// Token: 0x06000EFC RID: 3836 RVA: 0x00074A89 File Offset: 0x00072C89
		internal static AgentState GetNavalAgentState(Agent affectedAgent, float deathProbability, out bool usedSurgery)
		{
			if (!affectedAgent.IsInWater())
			{
				usedSurgery = false;
				return 0;
			}
			usedSurgery = true;
			if (affectedAgent.Character != null && affectedAgent.Character.IsHero)
			{
				return 3;
			}
			return 4;
		}

		// Token: 0x06000EFD RID: 3837 RVA: 0x00074AB4 File Offset: 0x00072CB4
		internal static void UpdateSceneWindDirection()
		{
			Vec2 globalWindVelocity = Mission.Current.Scene.GetGlobalWindVelocity();
			if (globalWindVelocity.IsNonZero())
			{
				float northRotation = Mission.Current.Scene.GetNorthRotation();
				globalWindVelocity.RotateCCW(northRotation);
				Mission.Current.Scene.SetGlobalWindVelocity(ref globalWindVelocity);
			}
		}

		// Token: 0x06000EFE RID: 3838 RVA: 0x00074B04 File Offset: 0x00072D04
		internal static void UpdateSceneWaterStrength()
		{
			float length = Mission.Current.Scene.GetGlobalWindVelocity().Length;
			float num = 30f;
			float num2 = 10f;
			Mission.Current.Scene.SetWaterStrength(length * num2 / num);
		}

		// Token: 0x06000EFF RID: 3839 RVA: 0x00074B4C File Offset: 0x00072D4C
		[return: TupleElementNames(new string[] { "team", "shipCount" })]
		private MBReadOnlyList<ValueTuple<Team, int>> CollectSortedBattleSideTeamsData(BattleSideEnum battleSide)
		{
			MBList<ValueTuple<Team, int>> mblist = new MBList<ValueTuple<Team, int>>();
			foreach (Team team in base.Mission.Teams)
			{
				if (team.Side == battleSide)
				{
					int countOfSetShipAssignments = this._shipsLogic.GetCountOfSetShipAssignments(team.TeamSide);
					if (countOfSetShipAssignments > 0)
					{
						mblist.Add(new ValueTuple<Team, int>(team, countOfSetShipAssignments));
					}
				}
			}
			mblist.Sort(delegate([TupleElementNames(new string[] { "team", "shipCount" })] ValueTuple<Team, int> t1, [TupleElementNames(new string[] { "team", "shipCount" })] ValueTuple<Team, int> t2)
			{
				bool flag = t1.Item1 == base.Mission.PlayerTeam || t1.Item1 == base.Mission.PlayerEnemyTeam;
				bool flag2 = t2.Item1 == base.Mission.PlayerTeam || t2.Item1 == base.Mission.PlayerEnemyTeam;
				if (!flag && !flag2)
				{
					if (t1.Item2 > t2.Item2)
					{
						return -1;
					}
					if (t1.Item2 < t2.Item2)
					{
						return 1;
					}
					return 0;
				}
				else
				{
					if (flag)
					{
						return 1;
					}
					return -1;
				}
			});
			return mblist;
		}

		// Token: 0x0400093D RID: 2365
		private const float InterTeamDeploymentGap = 32f;

		// Token: 0x0400093E RID: 2366
		private NavalShipsLogic _shipsLogic;

		// Token: 0x0400093F RID: 2367
		private NavalMissionDeploymentPlanningLogic _deploymentPlan;

		// Token: 0x04000940 RID: 2368
		private readonly MBList<IShipOrigin> _playerTeamShips;

		// Token: 0x04000941 RID: 2369
		private readonly MBList<IShipOrigin> _playerAllyTeamShips;

		// Token: 0x04000942 RID: 2370
		private readonly MBList<IShipOrigin> _enemyTeamShips;

		// Token: 0x04000943 RID: 2371
		private readonly NavalShipDeploymentLimit _playerTeamShipDeploymentLimit;

		// Token: 0x04000944 RID: 2372
		private readonly NavalShipDeploymentLimit _playerAllyTeamShipDeploymentLimit;

		// Token: 0x04000945 RID: 2373
		private readonly NavalShipDeploymentLimit _enemyTeamShipDeploymentLimit;
	}
}
