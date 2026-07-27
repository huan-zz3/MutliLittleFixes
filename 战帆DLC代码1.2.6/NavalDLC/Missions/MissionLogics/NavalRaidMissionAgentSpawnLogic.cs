using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NavalDLC.Missions.AI.TeamAI;
using NavalDLC.Missions.Deployment;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.ShipActuators;
using NavalDLC.Missions.ShipControl;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace NavalDLC.Missions.MissionLogics
{
	// Token: 0x020000D0 RID: 208
	public class NavalRaidMissionAgentSpawnLogic : MissionLogic, IBattleMissionAgentSpawnLogic, IMissionAgentSpawnLogic, IMissionBehavior, INavalMissionAgentSpawnLogic, IAgentStateDecider
	{
		// Token: 0x1400000C RID: 12
		// (add) Token: 0x06000F7B RID: 3963 RVA: 0x00076AE4 File Offset: 0x00074CE4
		// (remove) Token: 0x06000F7C RID: 3964 RVA: 0x00076B1C File Offset: 0x00074D1C
		public event Action PlayerShipsUpdated;

		// Token: 0x170002C2 RID: 706
		// (get) Token: 0x06000F7D RID: 3965 RVA: 0x00076B51 File Offset: 0x00074D51
		public BattleSideEnum PlayerSide
		{
			get
			{
				return this._playerSide;
			}
		}

		// Token: 0x170002C3 RID: 707
		// (get) Token: 0x06000F7E RID: 3966 RVA: 0x00076B59 File Offset: 0x00074D59
		public int TotalSpawnNumber
		{
			get
			{
				return ((this._defenderSpawnPhase != null) ? this._defenderSpawnPhase.TotalSpawnNumber : 0) + this._attackerTeamSpawnContext.TotalSpawnNumber;
			}
		}

		// Token: 0x170002C4 RID: 708
		// (get) Token: 0x06000F7F RID: 3967 RVA: 0x00076B7D File Offset: 0x00074D7D
		public int BattleSize
		{
			get
			{
				return this._battleSize;
			}
		}

		// Token: 0x170002C5 RID: 709
		// (get) Token: 0x06000F80 RID: 3968 RVA: 0x00076B85 File Offset: 0x00074D85
		public int NumberOfAgents
		{
			get
			{
				return base.Mission.AllAgents.Count;
			}
		}

		// Token: 0x170002C6 RID: 710
		// (get) Token: 0x06000F81 RID: 3969 RVA: 0x00076B97 File Offset: 0x00074D97
		public MissionSpawnPhase DefenderActivePhase
		{
			get
			{
				return this._defenderSpawnPhase;
			}
		}

		// Token: 0x170002C7 RID: 711
		// (get) Token: 0x06000F82 RID: 3970 RVA: 0x00076B9F File Offset: 0x00074D9F
		public MissionSpawnPhase AttackerActivePhase
		{
			get
			{
				Debug.FailedAssert("Naval raid missions does not use phase system for attacker (naval) side", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Missions\\MissionLogics\\NavalRaidMissionAgentSpawnLogic.cs", "AttackerActivePhase", 92);
				return null;
			}
		}

		// Token: 0x170002C8 RID: 712
		// (get) Token: 0x06000F83 RID: 3971 RVA: 0x00076BB8 File Offset: 0x00074DB8
		public readonly ref MissionSpawnSettings SpawnSettings
		{
			get
			{
				return ref this._defenderSpawnSettings;
			}
		}

		// Token: 0x170002C9 RID: 713
		// (get) Token: 0x06000F84 RID: 3972 RVA: 0x00076BC0 File Offset: 0x00074DC0
		public IMissionDeploymentPlan DeploymentPlan
		{
			get
			{
				return this._deploymentPlan;
			}
		}

		// Token: 0x170002CA RID: 714
		// (get) Token: 0x06000F85 RID: 3973 RVA: 0x00076BC8 File Offset: 0x00074DC8
		public bool ReassignCaptainsOfRemovedShips
		{
			get
			{
				return this._setReassignCaptainsOfRemovedShips;
			}
		}

		// Token: 0x170002CB RID: 715
		// (get) Token: 0x06000F86 RID: 3974 RVA: 0x00076BD0 File Offset: 0x00074DD0
		public int DeployablePlayerShipCount
		{
			get
			{
				if (this._playerSide != 1)
				{
					return 0;
				}
				return this._attackerTeamShipDeploymentLimit.NetDeploymentLimit;
			}
		}

		// Token: 0x170002CC RID: 716
		// (get) Token: 0x06000F87 RID: 3975 RVA: 0x00076BF6 File Offset: 0x00074DF6
		public bool IsInitialSpawnOver
		{
			get
			{
				return this.DefenderActivePhase.InitialSpawnNumber == 0 && this._attackerTeamSpawnContext.IsInitialSpawnOver;
			}
		}

		// Token: 0x170002CD RID: 717
		// (get) Token: 0x06000F88 RID: 3976 RVA: 0x00076C12 File Offset: 0x00074E12
		public bool IsDeploymentOver
		{
			get
			{
				return base.Mission.Mode != 6 && this.IsInitialSpawnOver;
			}
		}

		// Token: 0x170002CE RID: 718
		// (get) Token: 0x06000F89 RID: 3977 RVA: 0x00076C2A File Offset: 0x00074E2A
		public MBReadOnlyList<IShipOrigin> AttackerTeamShips
		{
			get
			{
				return this._attackerTeamShips;
			}
		}

		// Token: 0x170002CF RID: 719
		// (get) Token: 0x06000F8A RID: 3978 RVA: 0x00076C32 File Offset: 0x00074E32
		public MBReadOnlyList<IShipOrigin> PlayerShips
		{
			get
			{
				if (this._playerSide == 1)
				{
					return this._attackerTeamShips;
				}
				return null;
			}
		}

		// Token: 0x06000F8B RID: 3979 RVA: 0x00076C48 File Offset: 0x00074E48
		public NavalRaidMissionAgentSpawnLogic(IMissionTroopSupplier[] suppliers, BattleSideEnum playerSide, MBList<IShipOrigin> attackerSideShips, NavalShipDeploymentLimit attackerSideShipDeploymentLimit, int attackerTroopCount, int defenderTroopCount)
		{
			this._playerSide = playerSide;
			this._battleSize = BannerlordConfig.GetRealBattleSize();
			this._battleSize = MathF.Min(this._battleSize, DefaultBattleMissionAgentSpawnLogic.MaxNumberOfTroopsForMission);
			this._battleSideTroopSuppliers = suppliers;
			this._attackerTeamSide = ((this._playerSide == 1) ? 0 : 2);
			this._attackerTeamShips = attackerSideShips;
			this._attackerTeamShipDeploymentLimit = attackerSideShipDeploymentLimit;
			int num;
			int num2;
			NavalRaidMissionAgentSpawnLogic.ComputeInitialTroopCounts(attackerTroopCount, defenderTroopCount, out num, out num2);
			if (attackerTroopCount > num)
			{
				MBDebug.ShowWarning("Attacker deployable troop count is not supported by current battle size. Make sure UI side clamps this number w.r.t. battle size");
				this._attackerInitialTroopCount = num;
			}
			else
			{
				this._attackerInitialTroopCount = attackerTroopCount;
			}
			this._defenderInitialTroopCount = num2;
			this._defenderTotalTroopCount = defenderTroopCount;
			this._isAttackerSideDeployed = false;
			this._isDefenderSideDeployed = false;
		}

		// Token: 0x06000F8C RID: 3980 RVA: 0x00076D04 File Offset: 0x00074F04
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._navalAgentsLogic = base.Mission.GetMissionBehavior<NavalAgentsLogic>();
			this._navalAgentsLogic.SetDeploymentMode(true);
			this._navalShipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
			this._navalShipsLogic.SetDeploymentMode(true);
			this._navalShipsLogic.SetTeamShipDeploymentLimit(this._attackerTeamSide, this._attackerTeamShipDeploymentLimit);
			this._navalShipsLogic.BeforeShipRemovedEvent += this.OnBeforeShipRemoved;
			this._deploymentPlan = base.Mission.GetMissionBehavior<NavalRaidMissionDeploymentPlanningLogic>();
			if (!SailWindProfile.IsSailWindProfileInitialized)
			{
				SailWindProfile.InitializeProfile();
			}
			MissionGameModels.Current.BattleInitializationModel.InitializeModel();
			BattleInitializationModel.SetBypassPlayerDeployment(true);
		}

		// Token: 0x06000F8D RID: 3981 RVA: 0x00076DB1 File Offset: 0x00074FB1
		public override void OnMissionStateFinalized()
		{
			SailWindProfile.FinalizeProfile();
			this._navalShipsLogic.BeforeShipRemovedEvent -= this.OnBeforeShipRemoved;
			BattleInitializationModel.SetBypassPlayerDeployment(false);
		}

		// Token: 0x06000F8E RID: 3982 RVA: 0x00076DD5 File Offset: 0x00074FD5
		public override void EarlyStart()
		{
			base.EarlyStart();
			this.InitializeMissionTeamSides();
		}

		// Token: 0x06000F8F RID: 3983 RVA: 0x00076DE4 File Offset: 0x00074FE4
		public override void AfterStart()
		{
			base.AfterStart();
			DefaultNavalMissionLogic.UpdateSceneWindDirection();
			this.InitializeShipAssignments();
			this._defenderSpawnPhase = new MissionSpawnPhase
			{
				TotalSpawnNumber = this._defenderTotalTroopCount,
				InitialSpawnNumber = this._defenderInitialTroopCount,
				RemainingSpawnNumber = this._defenderTotalTroopCount - this._defenderInitialTroopCount
			};
			Team team = base.Mission.Teams.FirstOrDefault<Team>((Team t) => t.TeamSide != this._attackerTeamSide);
			this._deploymentPlan.SetSpawnWithHorses(team, false);
			base.Mission.SetBattleAgentCount(MathF.Min(this._defenderSpawnPhase.InitialSpawnNumber, this._attackerTeamSpawnContext.TotalSpawnNumber));
			base.Mission.SetInitialAgentCountForSide(0, this._defenderInitialTroopCount);
			base.Mission.SetInitialAgentCountForSide(1, this._attackerTeamSpawnContext.TotalSpawnNumber);
			this._bannerBearerLogic = base.Mission.GetMissionBehavior<BannerBearerLogic>();
			if (this._bannerBearerLogic != null)
			{
				for (int i = 0; i < 2; i++)
				{
					this._defenderSideSpawnContext.SetBannerBearerLogic(this._bannerBearerLogic);
				}
			}
			MissionGameModels.Current.BattleSpawnModel.OnMissionStart();
		}

		// Token: 0x06000F90 RID: 3984 RVA: 0x00076EF8 File Offset: 0x000750F8
		public override void OnDeploymentFinished()
		{
			foreach (MissionShip missionShip in this._navalShipsLogic.AllShips)
			{
				missionShip.SetAnchor(false, false, 1f);
				if (!missionShip.IsPlayerShip)
				{
					missionShip.SetController(ShipControllerType.AI, true);
				}
			}
			this._navalShipsLogic.SetDeploymentMode(false);
			this._attackerTeamSpawnContext.OnDeploymentFinished();
			this._navalAgentsLogic.SetIgnoreTroopCapacities(true);
			this._navalAgentsLogic.SetDeploymentMode(false);
		}

		// Token: 0x06000F91 RID: 3985 RVA: 0x00076F98 File Offset: 0x00075198
		public override void OnMissionTick(float dt)
		{
			if (!this._isAttackerSideDeployed || !this._isDefenderSideDeployed)
			{
				return;
			}
			if (!base.Mission.IsDeploymentFinished)
			{
				this._attackerTeamSpawnContext.OnDeploymentTick(dt);
				return;
			}
			if (this._defenderReinforcementSpawnEnabled)
			{
				this.CheckDefenderReinforcementBatch();
			}
			if (this._defenderSideSpawningReinforcements)
			{
				this.CheckDefenderReinforcementSpawn();
			}
		}

		// Token: 0x06000F92 RID: 3986 RVA: 0x00076FEF File Offset: 0x000751EF
		public AgentState GetAgentState(Agent affectedAgent, float deathProbability, out bool usedSurgery)
		{
			return DefaultNavalMissionLogic.GetNavalAgentState(affectedAgent, deathProbability, out usedSurgery);
		}

		// Token: 0x06000F93 RID: 3987 RVA: 0x00076FF9 File Offset: 0x000751F9
		public void StartSpawner(BattleSideEnum side)
		{
			if (side == 1)
			{
				this._attackerTeamSpawnContext.SetSpawnTroops(true, false);
				return;
			}
			if (side == null)
			{
				this._defenderSideSpawnContext.SetSpawnTroops(true);
			}
		}

		// Token: 0x06000F94 RID: 3988 RVA: 0x0007701C File Offset: 0x0007521C
		public void StopSpawner(BattleSideEnum side)
		{
			if (side == 1)
			{
				this._attackerTeamSpawnContext.SetSpawnTroops(false, false);
				return;
			}
			this._defenderSideSpawnContext.SetSpawnTroops(false);
		}

		// Token: 0x06000F95 RID: 3989 RVA: 0x0007703C File Offset: 0x0007523C
		public bool IsSideSpawnEnabled(BattleSideEnum side)
		{
			if (side == 1)
			{
				return this._attackerTeamSpawnContext.TroopSpawningActive;
			}
			return this._defenderSideSpawnContext.TroopSpawnActive;
		}

		// Token: 0x06000F96 RID: 3990 RVA: 0x0007705C File Offset: 0x0007525C
		public bool IsSideDepleted(BattleSideEnum side)
		{
			if (side == 1)
			{
				int num = 0;
				foreach (Team team in base.Mission.Teams)
				{
					if (team.Side == side)
					{
						num += team.ActiveAgents.Count;
					}
				}
				num += this._navalAgentsLogic.GetNumberOfReservedTroops(side, true);
				return num == 0;
			}
			return this._defenderSideSpawnContext.NumberOfActiveTroops == 0 && this._defenderSpawnPhase.RemainingSpawnNumber == 0;
		}

		// Token: 0x06000F97 RID: 3991 RVA: 0x000770FC File Offset: 0x000752FC
		internal void SetDefenderReinforcementSpawnEnabled(bool value, bool resetTimers = true)
		{
			if (this._defenderReinforcementSpawnEnabled != value)
			{
				this._defenderReinforcementSpawnEnabled = value;
				if (resetTimers)
				{
					this._defenderReinforcementSpawnTimer.Reset();
				}
			}
		}

		// Token: 0x06000F98 RID: 3992 RVA: 0x0007711C File Offset: 0x0007531C
		public float GetReinforcementInterval(BattleSideEnum battleSide)
		{
			if (battleSide == 1)
			{
				return NavalAgentsLogic.ComputeReinforcementSpawnDuration(0);
			}
			return this._defenderSpawnSettings.GlobalReinforcementInterval;
		}

		// Token: 0x06000F99 RID: 3993 RVA: 0x00077134 File Offset: 0x00075334
		public int GetNumberOfPlayerControllableTroops()
		{
			if (this._attackerTeamSide == null)
			{
				return this._attackerInitialTroopCount;
			}
			return this._defenderSideSpawnContext.GetNumberOfPlayerControllableTroops();
		}

		// Token: 0x06000F9A RID: 3994 RVA: 0x00077150 File Offset: 0x00075350
		public IEnumerable<IAgentOriginBase> GetAllTroopsForSide(BattleSideEnum side)
		{
			return this._battleSideTroopSuppliers[side].GetAllTroops();
		}

		// Token: 0x06000F9B RID: 3995 RVA: 0x0007716C File Offset: 0x0007536C
		public void SetSpawnTroops(BattleSideEnum battleSide, bool spawnTroops, bool enforceSpawning = false)
		{
			if (battleSide == null)
			{
				this._defenderSideSpawnContext.SetSpawnTroops(spawnTroops);
				return;
			}
			this._attackerTeamSpawnContext.SetSpawnTroops(spawnTroops, false);
		}

		// Token: 0x06000F9C RID: 3996 RVA: 0x0007718B File Offset: 0x0007538B
		public bool GetSpawnHorses(BattleSideEnum side)
		{
			return side == null && this._defenderSideSpawnContext.SpawnWithHorses;
		}

		// Token: 0x06000F9D RID: 3997 RVA: 0x000771A0 File Offset: 0x000753A0
		public void OnSideDeploymentOver(BattleSideEnum battleSide)
		{
			foreach (Team team in base.Mission.Teams)
			{
				if (team.Side == battleSide)
				{
					base.Mission.OnTeamDeployed(team);
				}
			}
			base.Mission.OnBattleSideDeployed(battleSide);
			foreach (Team team2 in base.Mission.Teams)
			{
				if (team2.Side == battleSide)
				{
					foreach (Formation formation in team2.FormationsIncludingEmpty)
					{
						if (formation.CountOfUnits > 0)
						{
							formation.QuerySystem.EvaluateAllPreliminaryQueryData();
						}
					}
				}
				if (team2.Side == null)
				{
					team2.MasterOrderController.OnOrderIssued += new OnOrderIssuedDelegate(this.OrderController_OnOrderIssued);
					for (int i = 8; i < 10; i++)
					{
						Formation formation2 = team2.FormationsIncludingSpecialAndEmpty[i];
						if (formation2.CountOfUnits > 0)
						{
							team2.MasterOrderController.SelectFormation(formation2);
							team2.MasterOrderController.SetOrderWithAgent(7, team2.GeneralAgent);
							team2.MasterOrderController.ClearSelectedFormations();
							formation2.SetControlledByAI(true, false);
						}
					}
					team2.MasterOrderController.OnOrderIssued -= new OnOrderIssuedDelegate(this.OrderController_OnOrderIssued);
				}
			}
			if (battleSide == 1 && battleSide == this._playerSide)
			{
				Team playerTeam = base.Mission.PlayerTeam;
				Formation formation3 = ((playerTeam != null) ? playerTeam.FormationsIncludingEmpty.FirstOrDefault<Formation>(new Func<Formation, bool>(NavalDLCHelpers.IsPlayerCaptainOfFormationShip)) : null);
				NavalOrderController navalOrderController;
				if (formation3 != null && (navalOrderController = base.Mission.PlayerTeam.PlayerOrderController as NavalOrderController) != null)
				{
					navalOrderController.SelectFormation(formation3);
					navalOrderController.SetOrder(34);
					navalOrderController.SetFormationUpdateEnabledAfterSetOrder(true);
					navalOrderController.ClearSelectedFormations();
				}
			}
		}

		// Token: 0x06000F9E RID: 3998 RVA: 0x000773E4 File Offset: 0x000755E4
		public void DeployAttackerSideShips()
		{
			this.MakeAttackerDeploymentPlans();
			Team team = base.Mission.Teams.FirstOrDefault<Team>((Team t) => t.Side == 1);
			foreach (Formation formation in team.FormationsIncludingEmpty)
			{
				FormationClass formationIndex = formation.FormationIndex;
				IFormationDeploymentPlan formationPlan = this._deploymentPlan.GetFormationPlan(team, formationIndex, false);
				if (formationPlan.HasFrame())
				{
					MatrixFrame frame = formationPlan.GetFrame();
					this._navalShipsLogic.SpawnShip(formation, in frame, true, false).SetController(ShipControllerType.None, true);
				}
			}
		}

		// Token: 0x06000F9F RID: 3999 RVA: 0x000774A8 File Offset: 0x000756A8
		public void DeployAttackerSideTroops()
		{
			this.SetSpawnTroops(1, true, false);
			this._attackerTeamSpawnContext.AllocateAndDeployInitialTroops(base.Mission);
			this._isAttackerSideDeployed = true;
		}

		// Token: 0x06000FA0 RID: 4000 RVA: 0x000774CB File Offset: 0x000756CB
		public void UpdateAttackerShips()
		{
			this._attackerTeamSpawnContext.UpdateShips();
		}

		// Token: 0x06000FA1 RID: 4001 RVA: 0x000774D8 File Offset: 0x000756D8
		public void OnPlayerShipsUpdated()
		{
			Action playerShipsUpdated = this.PlayerShipsUpdated;
			if (playerShipsUpdated == null)
			{
				return;
			}
			playerShipsUpdated();
		}

		// Token: 0x06000FA2 RID: 4002 RVA: 0x000774EA File Offset: 0x000756EA
		public void SetReassignCaptainsOfRemovedShips(bool value)
		{
			this._setReassignCaptainsOfRemovedShips = value;
		}

		// Token: 0x06000FA3 RID: 4003 RVA: 0x000774F4 File Offset: 0x000756F4
		private void InitializeShipAssignments()
		{
			this._navalShipsLogic.ClearShipAssignments();
			int num = MathF.Min(this._attackerTeamShipDeploymentLimit.NetDeploymentLimit, this._attackerTeamShips.Count);
			num = MathF.Min(this._navalAgentsLogic.GetTeamTroopOrigins(this._attackerTeamSide).Count<IAgentOriginBase>(), num);
			foreach (ValueTuple<FormationClass, IShipOrigin> valueTuple in this.AssignShipsToFormations(this._attackerTeamShips, num))
			{
				this._navalShipsLogic.SetShipAssignment(this._attackerTeamSide, valueTuple.Item1, valueTuple.Item2);
			}
		}

		// Token: 0x06000FA4 RID: 4004 RVA: 0x000775AC File Offset: 0x000757AC
		public bool HasPendingCaptainAssignment(Formation formation)
		{
			return this._attackerTeamSpawnContext.HasPendingCaptainAssignment(formation);
		}

		// Token: 0x06000FA5 RID: 4005 RVA: 0x000775BC File Offset: 0x000757BC
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

		// Token: 0x06000FA6 RID: 4006 RVA: 0x0007762C File Offset: 0x0007582C
		private void MakeAttackerDeploymentPlans()
		{
			Team team = base.Mission.Teams.Where<Team>((Team t) => t.Side == 1 && this._navalShipsLogic.GetCountOfSetShipAssignments(t.TeamSide) > 0).First<Team>();
			this.AddTeamShipsToDeploymentPlan(team);
			this._deploymentPlan.MakeDeploymentPlan(team, 0f, 0f);
		}

		// Token: 0x06000FA7 RID: 4007 RVA: 0x00077678 File Offset: 0x00075878
		private void AddTeamShipsToDeploymentPlan(Team team)
		{
			for (int i = 0; i < 11; i++)
			{
				ShipAssignment shipAssignment = this._navalShipsLogic.GetShipAssignment(team.TeamSide, i);
				if (shipAssignment.IsSet)
				{
					this._deploymentPlan.AddShip(team, shipAssignment.FormationIndex, shipAssignment.ShipOrigin);
				}
			}
		}

		// Token: 0x06000FA8 RID: 4008 RVA: 0x000776C5 File Offset: 0x000758C5
		private void OnBeforeShipRemoved(MissionShip ship)
		{
			if (ship.Team != null)
			{
				this._attackerTeamSpawnContext.OnBeforeShipRemoved(ship);
			}
		}

		// Token: 0x06000FA9 RID: 4009 RVA: 0x000776DC File Offset: 0x000758DC
		public void DeployDefenderSideTroops()
		{
			this.SetSpawnTroops(0, true, false);
			Team team = base.Mission.Teams.FirstOrDefault<Team>((Team t) => t.Side == 0);
			int num = MathF.Max(BannerlordConfig.GetRealBattleSize() - this._attackerInitialTroopCount, 0);
			int num2 = MathF.Min(this._defenderSpawnPhase.InitialSpawnNumber, num);
			this._defenderSideSpawnContext.SetSpawnWithHorses(false);
			this._defenderSideSpawnContext.ReserveTroops(num2);
			this.MakeDefenderDeploymentPlans(team);
			this._defenderSideSpawnContext.SpawnTroops(num2, false);
			this.DefenderActivePhase.OnInitialTroopsSpawned();
			this._defenderSideSpawnContext.OnInitialSpawnOver();
			this._isDefenderSideDeployed = true;
		}

		// Token: 0x06000FAA RID: 4010 RVA: 0x00077794 File Offset: 0x00075994
		private void CheckDefenderReinforcementSpawn()
		{
			if (this._defenderSideSpawnContext.HasSpawnableReinforcements && (float)this._defenderSideSpawnContext.ReinforcementsSpawnedInLastBatch < this._defenderSideSpawnContext.ReinforcementBatchSize)
			{
				int num = 0;
				int num2 = this._defenderSideSpawnContext.TryReinforcementSpawn();
				this.DefenderActivePhase.RemainingSpawnNumber -= num2;
				if (num + num2 > 0)
				{
					this.NotifyDefenderReinforcementTroopsSpawned(true);
				}
			}
		}

		// Token: 0x06000FAB RID: 4011 RVA: 0x000777F8 File Offset: 0x000759F8
		private void MakeDefenderDeploymentPlans(Team defenderTeam)
		{
			MBList<ValueTuple<Team, MissionFormationSpawnData[]>> mblist;
			this._defenderSideSpawnContext.GetTeamFormationsSpawnData(ref mblist);
			MissionFormationSpawnData[] item = mblist.First<ValueTuple<Team, MissionFormationSpawnData[]>>().Item2;
			for (int i = 0; i < item.Length; i++)
			{
				if (item[i].NumTroops > 0)
				{
					this._deploymentPlan.AddTroops(defenderTeam, i, item[i].FootTroopCount, item[i].MountedTroopCount, false);
				}
			}
			this._deploymentPlan.MakeDeploymentPlan(defenderTeam, 0f, 0f);
			if (!this._deploymentPlan.IsReinforcementPlanMade(defenderTeam))
			{
				int num = Math.Max(this._battleSize / (2 * item.Length), 1);
				for (int j = 0; j < item.Length; j++)
				{
					if (TroopClassExtensions.IsMounted(j))
					{
						this._deploymentPlan.AddTroops(defenderTeam, j, 0, num, true);
					}
					else
					{
						this._deploymentPlan.AddTroops(defenderTeam, j, num, 0, true);
					}
				}
				this._deploymentPlan.MakeReinforcementDeploymentPlan(defenderTeam);
			}
		}

		// Token: 0x06000FAC RID: 4012 RVA: 0x000778E4 File Offset: 0x00075AE4
		private void CheckDefenderReinforcementBatch()
		{
			if (this._defenderReinforcementSpawnTimer.ElapsedTime >= this._defenderSpawnSettings.GlobalReinforcementInterval)
			{
				this.NotifyDefenderReinforcementTroopsSpawned(false);
				bool flag = this._defenderSideSpawnContext.CheckReinforcementBatch();
				this._defenderSideSpawningReinforcements = flag && this.CheckDefenderMinimumBatchQuotaRequirement();
				this._defenderReinforcementSpawnTimer.Reset();
			}
		}

		// Token: 0x06000FAD RID: 4013 RVA: 0x0007793C File Offset: 0x00075B3C
		private bool CheckDefenderMinimumBatchQuotaRequirement()
		{
			int num = DefaultBattleMissionAgentSpawnLogic.MaxNumberOfAgentsForMission - this.NumberOfAgents;
			int num2 = 0;
			for (int i = 0; i < 2; i++)
			{
				num2 += this._defenderSideSpawnContext.ReinforcementQuotaRequirement;
			}
			return num >= num2;
		}

		// Token: 0x06000FAE RID: 4014 RVA: 0x0007797C File Offset: 0x00075B7C
		private void NotifyDefenderReinforcementTroopsSpawned(bool checkEmptyReserves)
		{
			int reinforcementsSpawnedInLastBatch = this._defenderSideSpawnContext.ReinforcementsSpawnedInLastBatch;
			if (!this._defenderSideSpawnContext.ReinforcementsNotifiedOnLastBatch && reinforcementsSpawnedInLastBatch > 0 && (!checkEmptyReserves || (checkEmptyReserves && !this._defenderSideSpawnContext.HasReservedTroops)))
			{
				this._defenderSideSpawnContext.SetReinforcementsNotifiedOnLastBatch(true);
			}
		}

		// Token: 0x06000FAF RID: 4015 RVA: 0x000779C5 File Offset: 0x00075BC5
		private void OrderController_OnOrderIssued(OrderType orderType, MBReadOnlyList<Formation> appliedFormations, OrderController orderController, params object[] delegateParams)
		{
			DeploymentHandler.OrderController_OnOrderIssued_Aux(orderType, appliedFormations, orderController, delegateParams);
		}

		// Token: 0x06000FB0 RID: 4016 RVA: 0x000779D4 File Offset: 0x00075BD4
		private void InitializeMissionTeamSides()
		{
			this._defenderReinforcementSpawnTimer = new BasicMissionTimer();
			this._defenderSpawnSettings = new MissionSpawnSettings(1, 0, 0, 3f, 0.1f, 0.2f, 0f, 0, 0f, 0f, 1f, 0.75f);
			this._defenderSideSpawnContext = new MissionBattleSideSpawnContext(this, 0, this._battleSideTroopSuppliers[0], this._playerSide == 0, false);
			MBList<IAgentOriginBase> mblist = new MBList<IAgentOriginBase>();
			foreach (IAgentOriginBase agentOriginBase in this._battleSideTroopSuppliers[1].SupplyTroops(this._attackerInitialTroopCount))
			{
				mblist.Add(agentOriginBase);
			}
			this._attackerTeamSpawnContext = new NavalTeamSideSpawnContext(base.Mission, this, 1, this._attackerTeamSide, mblist);
		}

		// Token: 0x06000FB1 RID: 4017 RVA: 0x00077AB0 File Offset: 0x00075CB0
		public static void ComputeInitialTroopCounts(int totalAttackerTroopCount, int totalDefenderTroopCount, out int initialAttackerTroopCount, out int initialDefenderTroopCount)
		{
			int realBattleSize = BannerlordConfig.GetRealBattleSize();
			int num = totalAttackerTroopCount + totalDefenderTroopCount;
			if (num <= realBattleSize)
			{
				initialAttackerTroopCount = totalAttackerTroopCount;
				initialDefenderTroopCount = totalDefenderTroopCount;
				return;
			}
			int minimumDeployableTroopCountPerSide = NavalRaidMissionAgentSpawnLogic.GetMinimumDeployableTroopCountPerSide(realBattleSize);
			initialAttackerTroopCount = MathF.Round((float)realBattleSize * ((float)totalAttackerTroopCount / (float)num));
			if (totalAttackerTroopCount >= minimumDeployableTroopCountPerSide)
			{
				initialAttackerTroopCount = Math.Max(initialAttackerTroopCount, minimumDeployableTroopCountPerSide);
			}
			if (totalDefenderTroopCount >= minimumDeployableTroopCountPerSide)
			{
				int num2 = realBattleSize - minimumDeployableTroopCountPerSide;
				initialAttackerTroopCount = Math.Min(initialAttackerTroopCount, num2);
			}
			initialAttackerTroopCount = Math.Min(initialAttackerTroopCount, totalAttackerTroopCount);
			initialAttackerTroopCount = Math.Max(0, initialAttackerTroopCount);
			initialDefenderTroopCount = realBattleSize - initialAttackerTroopCount;
			initialDefenderTroopCount = Math.Min(initialDefenderTroopCount, totalDefenderTroopCount);
			initialDefenderTroopCount = Math.Max(0, initialDefenderTroopCount);
			int num3 = realBattleSize - (initialAttackerTroopCount + initialDefenderTroopCount);
			if (num3 > 0)
			{
				int num4 = Math.Min(num3, totalAttackerTroopCount - initialAttackerTroopCount);
				initialAttackerTroopCount += num4;
			}
		}

		// Token: 0x06000FB2 RID: 4018 RVA: 0x00077B57 File Offset: 0x00075D57
		public static int GetMinimumDeployableTroopCountPerSide(int battleSize)
		{
			return Math.Max(1, MathF.Floor((float)battleSize * 0.2f));
		}

		// Token: 0x0400095C RID: 2396
		private const float DefenderGlobalReinforcementSpawnInterval = 3f;

		// Token: 0x0400095D RID: 2397
		private const float DefenderReinforcementBatchPercentage = 0.1f;

		// Token: 0x0400095E RID: 2398
		private const float DefenderDesiredReinforcementPercentage = 0.2f;

		// Token: 0x0400095F RID: 2399
		private NavalAgentsLogic _navalAgentsLogic;

		// Token: 0x04000960 RID: 2400
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x04000961 RID: 2401
		private BannerBearerLogic _bannerBearerLogic;

		// Token: 0x04000962 RID: 2402
		private NavalRaidMissionDeploymentPlanningLogic _deploymentPlan;

		// Token: 0x04000963 RID: 2403
		private IMissionTroopSupplier[] _battleSideTroopSuppliers;

		// Token: 0x04000964 RID: 2404
		private readonly int _battleSize;

		// Token: 0x04000965 RID: 2405
		private NavalTeamSideSpawnContext _attackerTeamSpawnContext;

		// Token: 0x04000966 RID: 2406
		private MissionBattleSideSpawnContext _defenderSideSpawnContext;

		// Token: 0x04000967 RID: 2407
		private readonly BattleSideEnum _playerSide;

		// Token: 0x04000968 RID: 2408
		private readonly TeamSideEnum _attackerTeamSide;

		// Token: 0x04000969 RID: 2409
		private readonly int _attackerInitialTroopCount;

		// Token: 0x0400096A RID: 2410
		private readonly int _defenderInitialTroopCount;

		// Token: 0x0400096B RID: 2411
		private readonly int _defenderTotalTroopCount;

		// Token: 0x0400096C RID: 2412
		private BasicMissionTimer _defenderReinforcementSpawnTimer;

		// Token: 0x0400096D RID: 2413
		private MissionSpawnSettings _defenderSpawnSettings;

		// Token: 0x0400096E RID: 2414
		private MissionSpawnPhase _defenderSpawnPhase;

		// Token: 0x0400096F RID: 2415
		private bool _defenderReinforcementSpawnEnabled = true;

		// Token: 0x04000970 RID: 2416
		private bool _defenderSideSpawningReinforcements;

		// Token: 0x04000971 RID: 2417
		private bool _setReassignCaptainsOfRemovedShips = true;

		// Token: 0x04000972 RID: 2418
		private bool _isAttackerSideDeployed;

		// Token: 0x04000973 RID: 2419
		private bool _isDefenderSideDeployed;

		// Token: 0x04000974 RID: 2420
		private readonly MBList<IShipOrigin> _attackerTeamShips;

		// Token: 0x04000975 RID: 2421
		private readonly NavalShipDeploymentLimit _attackerTeamShipDeploymentLimit;
	}
}
