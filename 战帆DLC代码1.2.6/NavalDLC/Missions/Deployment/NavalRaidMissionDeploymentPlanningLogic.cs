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
	// Token: 0x020000E0 RID: 224
	public class NavalRaidMissionDeploymentPlanningLogic : MissionDeploymentPlanningLogic
	{
		// Token: 0x0600119C RID: 4508 RVA: 0x00081558 File Offset: 0x0007F758
		public override void Initialize()
		{
			this._attackerSideTeamDeploymentPlans.Clear();
			this._defenderSideTeamDeploymentPlans.Clear();
			foreach (Team team in base.Mission.Teams)
			{
				if (team.Side == null)
				{
					DefaultTeamDeploymentPlan defaultTeamDeploymentPlan = new DefaultTeamDeploymentPlan(base.Mission, team);
					this._defenderSideTeamDeploymentPlans.Add(new ValueTuple<Team, DefaultTeamDeploymentPlan>(team, defaultTeamDeploymentPlan));
				}
				else
				{
					NavalTeamDeploymentPlan navalTeamDeploymentPlan = new NavalTeamDeploymentPlan(base.Mission, team);
					this._attackerSideTeamDeploymentPlans.Add(new ValueTuple<Team, NavalTeamDeploymentPlan>(team, navalTeamDeploymentPlan));
				}
			}
		}

		// Token: 0x0600119D RID: 4509 RVA: 0x00081608 File Offset: 0x0007F808
		public override void ClearDeploymentPlan(Team team)
		{
			this.GetTeamPlan<ITeamDeploymentPlan>(team).ClearPlan(false);
		}

		// Token: 0x0600119E RID: 4510 RVA: 0x00081617 File Offset: 0x0007F817
		public override bool SupportsReinforcements()
		{
			return true;
		}

		// Token: 0x0600119F RID: 4511 RVA: 0x0008161A File Offset: 0x0007F81A
		public override bool SupportsNavmesh(Team team)
		{
			return team.Side == null;
		}

		// Token: 0x060011A0 RID: 4512 RVA: 0x00081627 File Offset: 0x0007F827
		public override void UpdateReinforcementPlan(Team team)
		{
			this.GetTeamPlan<DefaultTeamDeploymentPlan>(team).UpdateReinforcementPlans();
		}

		// Token: 0x060011A1 RID: 4513 RVA: 0x00081635 File Offset: 0x0007F835
		public override bool HasPlayerSpawnFrame(BattleSideEnum battleSide)
		{
			return battleSide == null && this._defenderSidePlayerSpawnFrame != null;
		}

		// Token: 0x060011A2 RID: 4514 RVA: 0x00081648 File Offset: 0x0007F848
		public override bool GetPlayerSpawnFrame(BattleSideEnum battleSide, out WorldPosition position, out Vec2 direction)
		{
			if (battleSide == null && this._defenderSidePlayerSpawnFrame != null)
			{
				Scene scene = Mission.Current.Scene;
				UIntPtr zero = UIntPtr.Zero;
				WorldFrame worldFrame = this._defenderSidePlayerSpawnFrame.Value;
				position = new WorldPosition(scene, zero, worldFrame.Origin.GetGroundVec3(), false);
				worldFrame = this._defenderSidePlayerSpawnFrame.Value;
				direction = worldFrame.Rotation.f.AsVec2.Normalized();
				return true;
			}
			position = WorldPosition.Invalid;
			direction = Vec2.Invalid;
			return false;
		}

		// Token: 0x060011A3 RID: 4515 RVA: 0x000816DC File Offset: 0x0007F8DC
		public void ClearAddedShips(Team team)
		{
			this.GetTeamPlan<NavalTeamDeploymentPlan>(team).ClearAddedShips();
		}

		// Token: 0x060011A4 RID: 4516 RVA: 0x000816EA File Offset: 0x0007F8EA
		public void ClearAddedTroops(Team team)
		{
			this.GetTeamPlan<DefaultTeamDeploymentPlan>(team).ClearAddedTroops(false);
		}

		// Token: 0x060011A5 RID: 4517 RVA: 0x000816FC File Offset: 0x0007F8FC
		public override void ClearAll()
		{
			foreach (ValueTuple<Team, DefaultTeamDeploymentPlan> valueTuple in this._defenderSideTeamDeploymentPlans)
			{
				valueTuple.Item2.ClearAddedTroops(false);
				valueTuple.Item2.ClearPlan(false);
			}
			foreach (ValueTuple<Team, NavalTeamDeploymentPlan> valueTuple2 in this._attackerSideTeamDeploymentPlans)
			{
				valueTuple2.Item2.ClearAddedShips();
				valueTuple2.Item2.ClearPlan(false);
			}
		}

		// Token: 0x060011A6 RID: 4518 RVA: 0x000817B0 File Offset: 0x0007F9B0
		public void AddShip(Team team, FormationClass formationIndex, IShipOrigin shipOrigin)
		{
			this.GetTeamPlan<NavalTeamDeploymentPlan>(team).AddShip(formationIndex, shipOrigin);
		}

		// Token: 0x060011A7 RID: 4519 RVA: 0x000817C0 File Offset: 0x0007F9C0
		public bool RemoveShip(Team team, FormationClass formationIndex)
		{
			return this.GetTeamPlan<NavalTeamDeploymentPlan>(team).RemoveShip(formationIndex);
		}

		// Token: 0x060011A8 RID: 4520 RVA: 0x000817CF File Offset: 0x0007F9CF
		public void AddTroops(Team team, FormationClass formationClass, int footTroopCount, int mountedTroopCount = 0, bool isReinforcement = false)
		{
			BattleSideEnum side = team.Side;
			this.GetTeamPlan<DefaultTeamDeploymentPlan>(team).AddTroops(formationClass, footTroopCount, mountedTroopCount, isReinforcement);
		}

		// Token: 0x060011A9 RID: 4521 RVA: 0x000817EA File Offset: 0x0007F9EA
		public void SetSpawnWithHorses(Team team, bool spawnWithHorses)
		{
			this.GetTeamPlan<DefaultTeamDeploymentPlan>(team).SetSpawnWithHorses(spawnWithHorses);
		}

		// Token: 0x060011AA RID: 4522 RVA: 0x000817FC File Offset: 0x0007F9FC
		public override void MakeDeploymentPlan(Team team, float spawnPathOffset = 0f, float targetOffset = 0f)
		{
			if (!this.IsPlanMade(team))
			{
				this.MakeDeploymentPlanAux(team, false);
				bool flag;
				if (this.IsPlanMade(team, ref flag))
				{
					base.Mission.OnDeploymentPlanMade(team, flag);
				}
			}
		}

		// Token: 0x060011AB RID: 4523 RVA: 0x00081832 File Offset: 0x0007FA32
		public void MakeReinforcementDeploymentPlan(Team team)
		{
			if (!this.IsReinforcementPlanMade(team))
			{
				this.MakeDeploymentPlanAux(team, true);
			}
		}

		// Token: 0x060011AC RID: 4524 RVA: 0x00081848 File Offset: 0x0007FA48
		public override bool RemakeDeploymentPlan(Team team)
		{
			this.IsPlanMade(team);
			if (team.Side != null)
			{
				this.ClearAddedShips(team);
				this.ClearDeploymentPlan(team);
				NavalShipsLogic missionBehavior = base.Mission.GetMissionBehavior<NavalShipsLogic>();
				for (int i = 0; i < 11; i++)
				{
					FormationClass formationClass = i;
					ShipAssignment shipAssignment = missionBehavior.GetShipAssignment(team.TeamSide, formationClass);
					if (shipAssignment.IsSet)
					{
						this.AddShip(team, formationClass, shipAssignment.ShipOrigin);
					}
				}
				this.MakeDeploymentPlan(team, 0f, 0f);
				return this.IsPlanMade(team);
			}
			ValueTuple<int, int>[] array = new ValueTuple<int, int>[11];
			foreach (Agent agent2 in base.Mission.AllAgents.Where<Agent>((Agent agent) => agent.IsHuman && agent.Team != null && agent.Team == team && agent.Formation != null))
			{
				int formationIndex = agent2.Formation.FormationIndex;
				ValueTuple<int, int> valueTuple = array[formationIndex];
				array[formationIndex] = (agent2.HasMount ? new ValueTuple<int, int>(valueTuple.Item1, valueTuple.Item2 + 1) : new ValueTuple<int, int>(valueTuple.Item1 + 1, valueTuple.Item2));
			}
			if (!this.IsInitialPlanSuitableForFormations(team, array))
			{
				this.ClearAddedTroops(team);
				this.ClearDeploymentPlan(team);
				for (int j = 0; j < 11; j++)
				{
					ValueTuple<int, int> valueTuple2 = array[j];
					int item = valueTuple2.Item1;
					int item2 = valueTuple2.Item2;
					if (item + item2 > 0)
					{
						this.AddTroops(team, j, item, item2, false);
					}
				}
				this.MakeDeploymentPlan(team, 0f, 0f);
				return this.IsPlanMade(team);
			}
			return false;
		}

		// Token: 0x060011AD RID: 4525 RVA: 0x00081A4C File Offset: 0x0007FC4C
		public override bool IsPositionInsideDeploymentBoundaries(Team team, in Vec2 position)
		{
			ITeamDeploymentPlan teamPlan = this.GetTeamPlan<ITeamDeploymentPlan>(team);
			if (teamPlan.HasDeploymentBoundaries())
			{
				ValueTuple<string, MBList<Vec2>> valueTuple;
				return teamPlan.IsPositionInsideDeploymentBoundaries(ref position, ref valueTuple);
			}
			Debug.FailedAssert("Cannot check if position is within deployment boundaries as requested team " + team.TeamIndex + " does not have deployment boundaries.", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Missions\\Deployment\\NavalRaidMissionDeploymentPlanningLogic.cs", "IsPositionInsideDeploymentBoundaries", 278);
			return false;
		}

		// Token: 0x060011AE RID: 4526 RVA: 0x00081AA4 File Offset: 0x0007FCA4
		public override Vec2 GetClosestDeploymentBoundaryPosition(Team team, in Vec2 position)
		{
			ITeamDeploymentPlan teamPlan = this.GetTeamPlan<ITeamDeploymentPlan>(team);
			if (teamPlan.HasDeploymentBoundaries())
			{
				return teamPlan.GetClosestDeploymentBoundaryPosition(ref position);
			}
			Debug.FailedAssert("Cannot retrieve closest deployment boundary position as requested team (index: " + team.TeamIndex + ") does not have deployment boundaries.", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Missions\\Deployment\\NavalRaidMissionDeploymentPlanningLogic.cs", "GetClosestDeploymentBoundaryPosition", 290);
			return position;
		}

		// Token: 0x060011AF RID: 4527 RVA: 0x00081B00 File Offset: 0x0007FD00
		public override void ProjectPositionToDeploymentBoundaries(Team team, ref WorldPosition endPosition)
		{
			if (this.HasDeploymentBoundaries(team))
			{
				Vec2 asVec = endPosition.AsVec2;
				if (!this.IsPositionInsideDeploymentBoundaries(team, ref asVec))
				{
					MatrixFrame deploymentFrame = this.GetDeploymentFrame(team);
					WorldPosition worldPosition;
					worldPosition..ctor(Mission.Current.Scene, UIntPtr.Zero, deploymentFrame.origin, false);
					WorldPosition worldPosition2;
					if (this.GetPathDeploymentBoundaryIntersection(team, ref worldPosition, ref endPosition, ref worldPosition2))
					{
						endPosition = worldPosition2;
					}
				}
			}
		}

		// Token: 0x060011B0 RID: 4528 RVA: 0x00081B62 File Offset: 0x0007FD62
		public override bool GetPathDeploymentBoundaryIntersection(Team team, in WorldPosition startPosition, in WorldPosition endPosition, out WorldPosition intersection)
		{
			return this.GetTeamPlan<DefaultTeamDeploymentPlan>(team).GetPathDeploymentBoundaryIntersection(ref startPosition, ref endPosition, ref intersection);
		}

		// Token: 0x060011B1 RID: 4529 RVA: 0x00081B74 File Offset: 0x0007FD74
		public override float GetSpawnPathOffset(Team team)
		{
			return 0f;
		}

		// Token: 0x060011B2 RID: 4530 RVA: 0x00081B7C File Offset: 0x0007FD7C
		public override MatrixFrame GetZoomFocusFrame(Team team)
		{
			if (team.Side == null)
			{
				return this.GetDeploymentFrame(team);
			}
			NavalTeamDeploymentPlan teamPlan = this.GetTeamPlan<NavalTeamDeploymentPlan>(team);
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

		// Token: 0x060011B3 RID: 4531 RVA: 0x00081C04 File Offset: 0x0007FE04
		public override float GetZoomOffset(Team team, float fovAngle)
		{
			ITeamDeploymentPlan teamPlan = this.GetTeamPlan<ITeamDeploymentPlan>(team);
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

		// Token: 0x060011B4 RID: 4532 RVA: 0x00081CA0 File Offset: 0x0007FEA0
		public override IFormationDeploymentPlan GetFormationPlan(Team team, FormationClass fClass, bool isReinforcement = false)
		{
			ITeamDeploymentPlan teamPlan = this.GetTeamPlan<ITeamDeploymentPlan>(team);
			if (team.IsAttacker)
			{
				return teamPlan.GetFormationPlan(fClass, false);
			}
			return teamPlan.GetFormationPlan(fClass, isReinforcement);
		}

		// Token: 0x060011B5 RID: 4533 RVA: 0x00081CD0 File Offset: 0x0007FED0
		public override bool IsPlanMade(Team team)
		{
			ITeamDeploymentPlan teamPlanAux = this.GetTeamPlanAux(team);
			return teamPlanAux != null && teamPlanAux.IsPlanMade(false);
		}

		// Token: 0x060011B6 RID: 4534 RVA: 0x00081CF4 File Offset: 0x0007FEF4
		public bool IsReinforcementPlanMade(Team team)
		{
			ITeamDeploymentPlan teamPlanAux = this.GetTeamPlanAux(team);
			return teamPlanAux != null && teamPlanAux.IsPlanMade(true);
		}

		// Token: 0x060011B7 RID: 4535 RVA: 0x00081D18 File Offset: 0x0007FF18
		public override bool IsPlanMade(Team team, out bool isFirstPlan)
		{
			isFirstPlan = false;
			ITeamDeploymentPlan teamPlanAux = this.GetTeamPlanAux(team);
			if (teamPlanAux != null && teamPlanAux.IsPlanMade(false))
			{
				isFirstPlan = teamPlanAux.IsFirstPlan(false);
				return true;
			}
			return false;
		}

		// Token: 0x060011B8 RID: 4536 RVA: 0x00081D48 File Offset: 0x0007FF48
		public override bool HasDeploymentBoundaries(Team team)
		{
			ITeamDeploymentPlan teamPlanAux = this.GetTeamPlanAux(team);
			return teamPlanAux != null && teamPlanAux.HasDeploymentBoundaries();
		}

		// Token: 0x060011B9 RID: 4537 RVA: 0x00081D68 File Offset: 0x0007FF68
		public override MatrixFrame GetDeploymentFrame(Team team)
		{
			return this.GetTeamPlan<ITeamDeploymentPlan>(team).GetDeploymentFrame();
		}

		// Token: 0x060011BA RID: 4538 RVA: 0x00081D76 File Offset: 0x0007FF76
		public float GetTargetOffset(Team team)
		{
			return this.GetTeamPlan<ITeamDeploymentPlan>(team).GetTargetOffset(false);
		}

		// Token: 0x060011BB RID: 4539 RVA: 0x00081D85 File Offset: 0x0007FF85
		public override MBReadOnlyList<ValueTuple<string, MBList<Vec2>>> GetDeploymentBoundaries(Team team)
		{
			return this.GetTeamPlan<ITeamDeploymentPlan>(team).GetDeploymentBoundaries();
		}

		// Token: 0x060011BC RID: 4540 RVA: 0x00081D94 File Offset: 0x0007FF94
		public virtual bool GetMeanBoundaryPosition(Team team, out Vec2 meanPosition, int boundaryIndex = 0)
		{
			NavalTeamDeploymentPlan teamPlan = this.GetTeamPlan<NavalTeamDeploymentPlan>(team);
			if (teamPlan != null && teamPlan.HasDeploymentBoundaries())
			{
				meanPosition = teamPlan.GetMeanBoundaryPosition(boundaryIndex);
				return true;
			}
			meanPosition = Vec2.Invalid;
			return false;
		}

		// Token: 0x060011BD RID: 4541 RVA: 0x00081DD0 File Offset: 0x0007FFD0
		private T GetTeamPlan<T>(Team team) where T : ITeamDeploymentPlan
		{
			ITeamDeploymentPlan teamPlanAux;
			if ((teamPlanAux = this.GetTeamPlanAux(team)) is T)
			{
				return (T)((object)teamPlanAux);
			}
			Debug.FailedAssert("Unable to cast team plan to given type", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Missions\\Deployment\\NavalRaidMissionDeploymentPlanningLogic.cs", "GetTeamPlan", 514);
			return default(T);
		}

		// Token: 0x060011BE RID: 4542 RVA: 0x00081E18 File Offset: 0x00080018
		private ITeamDeploymentPlan GetTeamPlanAux(Team team)
		{
			if (team.Side == null)
			{
				return this._defenderSideTeamDeploymentPlans.FirstOrDefault<ValueTuple<Team, DefaultTeamDeploymentPlan>>(([TupleElementNames(new string[] { "team", "plan" })] ValueTuple<Team, DefaultTeamDeploymentPlan> t) => t.Item1 == team).Item2;
			}
			if (team.Side == 1)
			{
				return this._attackerSideTeamDeploymentPlans.FirstOrDefault<ValueTuple<Team, NavalTeamDeploymentPlan>>(([TupleElementNames(new string[] { "team", "plan" })] ValueTuple<Team, NavalTeamDeploymentPlan> t) => t.Item1 == team).Item2;
			}
			return null;
		}

		// Token: 0x060011BF RID: 4543 RVA: 0x00081E88 File Offset: 0x00080088
		private void MakeDeploymentPlanAux(Team team, bool isReinforcement)
		{
			ITeamDeploymentPlan teamPlan = this.GetTeamPlan<ITeamDeploymentPlan>(team);
			if (teamPlan.IsPlanMade(isReinforcement))
			{
				teamPlan.ClearPlan(false);
			}
			if (this._formationSceneSpawnEntries == null)
			{
				this.ReadSpawnEntitiesFromScene();
			}
			teamPlan.MakeDeploymentPlan(0f, 0f, this._formationSceneSpawnEntries, isReinforcement);
		}

		// Token: 0x060011C0 RID: 4544 RVA: 0x00081ED4 File Offset: 0x000800D4
		private void ReadSpawnEntitiesFromScene()
		{
			this._defenderSidePlayerSpawnFrame = null;
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("player_spawn_frame");
			if (gameEntity != null)
			{
				MatrixFrame globalFrame = gameEntity.GetGlobalFrame();
				WorldPosition worldPosition;
				worldPosition..ctor(base.Mission.Scene, UIntPtr.Zero, globalFrame.origin, false);
				this._defenderSidePlayerSpawnFrame = new WorldFrame?(new WorldFrame(globalFrame.rotation, worldPosition));
			}
			this._formationSceneSpawnEntries = new FormationSceneSpawnEntry[2, 11];
			Scene scene = base.Mission.Scene;
			for (int i = 0; i < 2; i++)
			{
				string text = ((i == 1) ? "attacker_" : "defender_");
				for (int j = 0; j < 11; j++)
				{
					FormationClass formationClass = j;
					string text2 = text + FormationClassExtensions.GetName(formationClass).ToLower();
					string text3 = text2 + "_reinforcement";
					WeakGameEntity weakGameEntity = scene.FindWeakEntityWithTag(text2);
					WeakGameEntity? weakGameEntity2 = null;
					if (weakGameEntity == null)
					{
						FormationClass formationClass2 = FormationClassExtensions.FallbackClass(formationClass);
						int num = formationClass2;
						FormationSceneSpawnEntry formationSceneSpawnEntry = this._formationSceneSpawnEntries[i, num];
						if (formationSceneSpawnEntry.SpawnEntity != null)
						{
							weakGameEntity = formationSceneSpawnEntry.SpawnEntity.WeakEntity;
							weakGameEntity2 = new WeakGameEntity?(formationSceneSpawnEntry.ReinforcementSpawnEntity.WeakEntity);
						}
						else
						{
							text2 = text + FormationClassExtensions.GetName(formationClass2).ToLower();
							text3 = text2 + "_reinforcement";
							weakGameEntity = scene.FindWeakEntityWithTag(text2);
							weakGameEntity2 = new WeakGameEntity?(scene.FindWeakEntityWithTag(text3));
						}
						formationClass = ((weakGameEntity != null) ? formationClass2 : 10);
					}
					else
					{
						weakGameEntity2 = new WeakGameEntity?(scene.FindWeakEntityWithTag(text3));
					}
					GameEntity gameEntity2 = null;
					GameEntity gameEntity3 = null;
					if (weakGameEntity.IsValid)
					{
						gameEntity2 = GameEntity.CreateFromWeakEntity(weakGameEntity);
						if (weakGameEntity2 != null && weakGameEntity2.Value.IsValid)
						{
							gameEntity3 = GameEntity.CreateFromWeakEntity(weakGameEntity2.Value);
						}
					}
					if (gameEntity3 == null)
					{
						gameEntity3 = gameEntity2;
					}
					this._formationSceneSpawnEntries[i, j] = new FormationSceneSpawnEntry(formationClass, gameEntity2, gameEntity3);
				}
			}
		}

		// Token: 0x060011C1 RID: 4545 RVA: 0x00082100 File Offset: 0x00080300
		private bool IsInitialPlanSuitableForFormations(Team team, [TupleElementNames(new string[] { "footTroopCount", "mountedTroopCount" })] ValueTuple<int, int>[] troopDataPerFormationClass)
		{
			return this.GetTeamPlan<DefaultTeamDeploymentPlan>(team).IsInitialPlanSuitableForFormations(troopDataPerFormationClass);
		}

		// Token: 0x04000A0F RID: 2575
		public const string DefenderPlayerSpawnEntityTag = "player_spawn_frame";

		// Token: 0x04000A10 RID: 2576
		[TupleElementNames(new string[] { "team", "plan" })]
		private MBList<ValueTuple<Team, NavalTeamDeploymentPlan>> _attackerSideTeamDeploymentPlans = new MBList<ValueTuple<Team, NavalTeamDeploymentPlan>>();

		// Token: 0x04000A11 RID: 2577
		[TupleElementNames(new string[] { "team", "plan" })]
		private MBList<ValueTuple<Team, DefaultTeamDeploymentPlan>> _defenderSideTeamDeploymentPlans = new MBList<ValueTuple<Team, DefaultTeamDeploymentPlan>>();

		// Token: 0x04000A12 RID: 2578
		private WorldFrame? _defenderSidePlayerSpawnFrame;

		// Token: 0x04000A13 RID: 2579
		private FormationSceneSpawnEntry[,] _formationSceneSpawnEntries;
	}
}
