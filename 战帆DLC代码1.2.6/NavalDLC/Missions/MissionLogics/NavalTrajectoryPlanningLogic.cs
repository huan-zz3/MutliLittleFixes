using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NavalDLC.DWA;
using NavalDLC.Missions.Objects;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.MissionLogics
{
	// Token: 0x020000D7 RID: 215
	public class NavalTrajectoryPlanningLogic : MissionLogic
	{
		// Token: 0x060010FF RID: 4351 RVA: 0x0007E920 File Offset: 0x0007CB20
		public override void OnBehaviorInitialize()
		{
			this._simulator = new DWASimulator();
			this._navalShipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
			this._navalShipsLogic.ShipSpawnedEvent += this.OnShipSpawned;
			this._navalShipsLogic.ShipRemovedEvent += this.OnShipRemoved;
			this._simulatorParameters = DWASimulatorParameters.Create();
		}

		// Token: 0x06001100 RID: 4352 RVA: 0x0007E982 File Offset: 0x0007CB82
		public override void OnDeploymentFinished()
		{
			this.Initialize();
		}

		// Token: 0x06001101 RID: 4353 RVA: 0x0007E98C File Offset: 0x0007CB8C
		public override void OnMissionStateFinalized()
		{
			this._navalShipsLogic.ShipSpawnedEvent -= this.OnShipSpawned;
			this._navalShipsLogic.ShipRemovedEvent -= this.OnShipRemoved;
			if (this._simulator.IsInitialized)
			{
				this._simulator.Clear();
			}
			this._simulator = null;
		}

		// Token: 0x06001102 RID: 4354 RVA: 0x0007E9E6 File Offset: 0x0007CBE6
		public override void OnMissionTick(float dt)
		{
			if (base.Mission.IsDeploymentFinished)
			{
				this._simulator.Tick(dt);
			}
		}

		// Token: 0x06001103 RID: 4355 RVA: 0x0007EA01 File Offset: 0x0007CC01
		public void ForceReinitialize()
		{
			this.Initialize();
		}

		// Token: 0x06001104 RID: 4356 RVA: 0x0007EA09 File Offset: 0x0007CC09
		public void OnShipSpawned(MissionShip ship)
		{
			if (base.Mission.IsDeploymentFinished)
			{
				this.AddShipAux(ship);
			}
		}

		// Token: 0x06001105 RID: 4357 RVA: 0x0007EA1F File Offset: 0x0007CC1F
		public void OnShipRemoved(MissionShip ship)
		{
			if (base.Mission.IsDeploymentFinished)
			{
				this.RemoveShipAux(ship);
			}
		}

		// Token: 0x06001106 RID: 4358 RVA: 0x0007EA38 File Offset: 0x0007CC38
		private void Initialize()
		{
			this._simulator.SetParameters(in this._simulatorParameters);
			if (this._simulator.IsInitialized)
			{
				this._simulator.Clear();
			}
			foreach (MissionShip missionShip in this._navalShipsLogic.AllShips)
			{
				this.AddShipAux(missionShip);
			}
			List<GameEntity> list = base.Mission.Scene.FindEntitiesWithTag("naval_static_obstacle").ToList<GameEntity>();
			this.AddStaticObstacles(list);
			this._simulator.Initialize();
		}

		// Token: 0x06001107 RID: 4359 RVA: 0x0007EAE8 File Offset: 0x0007CCE8
		private void AddStaticObstacles(IReadOnlyList<GameEntity> staticObstacles)
		{
			if (staticObstacles.Count == 0)
			{
				return;
			}
			MBList<Vec3> mblist = new MBList<Vec3>();
			mblist.Add(Vec3.Zero);
			mblist.Add(Vec3.Zero);
			mblist.Add(Vec3.Zero);
			mblist.Add(Vec3.Zero);
			MBList<Vec3> mblist2 = mblist;
			MatrixFrame[] array = null;
			foreach (GameEntity gameEntity in staticObstacles)
			{
				Path pathWithName = Mission.Current.Scene.GetPathWithName(gameEntity.Name);
				if (pathWithName != null)
				{
					int numberOfPoints = pathWithName.NumberOfPoints;
					if (array == null || array.Length < numberOfPoints)
					{
						array = new MatrixFrame[numberOfPoints];
					}
					pathWithName.GetPoints(array);
					Vec2 vec = (array[1].origin - array[0].origin).AsVec2;
					if (vec.Normalize() < 1E-05f)
					{
						vec = Vec2.Zero;
					}
					Vec2 vec2 = NavalTrajectoryPlanningLogic.<AddStaticObstacles>g__ComputeOffset|12_0(in Vec2.Zero, false, in vec, true, 1);
					Vec2 vec3 = NavalTrajectoryPlanningLogic.<AddStaticObstacles>g__ComputeOffset|12_0(in Vec2.Zero, false, in vec, true, -1);
					for (int i = 0; i < numberOfPoints - 1; i++)
					{
						Vec3 origin = array[i].origin;
						Vec3 origin2 = array[i + 1].origin;
						Vec2 vec4 = Vec2.Zero;
						if (i + 2 < numberOfPoints)
						{
							vec4 = (array[i + 2].origin - array[i + 1].origin).AsVec2;
							if (vec4.Normalize() < 1E-05f)
							{
								vec4 = Vec2.Zero;
							}
						}
						bool flag = vec.LengthSquared > 1E-05f;
						bool flag2 = vec4.LengthSquared > 1E-05f;
						Vec2 vec5 = NavalTrajectoryPlanningLogic.<AddStaticObstacles>g__ComputeOffset|12_0(in vec, flag, in vec4, flag2, 1);
						Vec2 vec6 = NavalTrajectoryPlanningLogic.<AddStaticObstacles>g__ComputeOffset|12_0(in vec, flag, in vec4, flag2, -1);
						Vec3 vec7 = origin + vec2.ToVec3(0f);
						Vec3 vec8 = origin + vec3.ToVec3(0f);
						Vec3 vec9 = origin2 + vec5.ToVec3(0f);
						Vec3 vec10 = origin2 + vec6.ToVec3(0f);
						mblist2[0] = vec8;
						mblist2[1] = vec7;
						mblist2[2] = vec9;
						mblist2[3] = vec10;
						this._simulator.AddObstacle(mblist2);
						vec2 = vec5;
						vec3 = vec6;
						vec = vec4;
					}
				}
				else
				{
					IEnumerable<GameEntity> enumerable = from entity in gameEntity.GetChildren()
						orderby entity.Name
						select entity;
					MBList<Vec3> mblist3 = new MBList<Vec3>();
					foreach (GameEntity gameEntity2 in enumerable)
					{
						Vec3 origin3 = gameEntity2.GetGlobalFrame().origin;
						mblist3.Add(origin3);
					}
					MBSceneUtilities.RadialSortBoundary(ref mblist3);
					this._simulator.AddObstacle(mblist3);
				}
			}
		}

		// Token: 0x06001108 RID: 4360 RVA: 0x0007EE14 File Offset: 0x0007D014
		private void AddShipAux(MissionShip ship)
		{
			IDWAAgentDelegate idwaagentDelegate = ship.CreateDWAAgent(this._simulator.Parameters);
			this._simulator.AddAgent(idwaagentDelegate);
		}

		// Token: 0x06001109 RID: 4361 RVA: 0x0007EE40 File Offset: 0x0007D040
		private void RemoveShipAux(MissionShip ship)
		{
			this._simulator.RemoveAgent(ship.DWAAgentId);
		}

		// Token: 0x0600110B RID: 4363 RVA: 0x0007EE5C File Offset: 0x0007D05C
		[CompilerGenerated]
		internal static Vec2 <AddStaticObstacles>g__ComputeOffset|12_0(in Vec2 prevDir, bool hasPrev, in Vec2 nextDir, bool hasNext, int sideSign)
		{
			if (!hasPrev && !hasNext)
			{
				return Vec2.Zero;
			}
			Vec2 vec;
			if (hasPrev && !hasNext)
			{
				vec = prevDir;
				Vec2 vec2 = vec.RightVec() * (float)sideSign;
				if (vec2.LengthSquared > 1E-05f)
				{
					vec2 = vec2.Normalized();
				}
				return vec2 * 8f;
			}
			if (!hasPrev && hasNext)
			{
				vec = nextDir;
				Vec2 vec3 = vec.RightVec() * (float)sideSign;
				if (vec3.LengthSquared > 1E-05f)
				{
					vec3 = vec3.Normalized();
				}
				return vec3 * 8f;
			}
			vec = prevDir;
			Vec2 vec4 = vec.RightVec() * (float)sideSign;
			vec = nextDir;
			Vec2 vec5 = vec.RightVec() * (float)sideSign;
			bool flag = vec4.LengthSquared > 1E-05f;
			bool flag2 = vec5.LengthSquared > 1E-05f;
			if (!flag && !flag2)
			{
				return Vec2.Zero;
			}
			if (!flag)
			{
				vec5 = vec5.Normalized();
				return vec5 * 8f;
			}
			if (!flag2)
			{
				vec4 = vec4.Normalized();
				return vec4 * 8f;
			}
			Vec2 vec6 = vec4 + vec5;
			float lengthSquared = vec6.LengthSquared;
			if (lengthSquared <= 1E-05f)
			{
				return vec5.Normalized() * 8f;
			}
			vec6 /= MathF.Sqrt(lengthSquared);
			Vec2 vec7 = vec5.Normalized();
			float num = MathF.Abs(Vec2.DotProduct(vec6, vec7));
			if (num <= 1E-05f)
			{
				return vec7 * 8f;
			}
			float num2 = 8f / num;
			float num3 = 32f;
			if (num2 > num3)
			{
				num2 = num3;
			}
			else if (num2 < -num3)
			{
				num2 = -num3;
			}
			return vec6 * num2;
		}

		// Token: 0x040009E0 RID: 2528
		public const string StaticObstacleTag = "naval_static_obstacle";

		// Token: 0x040009E1 RID: 2529
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x040009E2 RID: 2530
		private DWASimulator _simulator;

		// Token: 0x040009E3 RID: 2531
		private DWASimulatorParameters _simulatorParameters;
	}
}
