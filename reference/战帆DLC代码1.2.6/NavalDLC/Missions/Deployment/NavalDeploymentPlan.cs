using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.Deployment
{
	// Token: 0x020000DC RID: 220
	public class NavalDeploymentPlan
	{
		// Token: 0x170002FE RID: 766
		// (get) Token: 0x06001136 RID: 4406 RVA: 0x000802F7 File Offset: 0x0007E4F7
		public bool IsRiverPlan
		{
			get
			{
				return this._isRiverPlan;
			}
		}

		// Token: 0x170002FF RID: 767
		// (get) Token: 0x06001137 RID: 4407 RVA: 0x000802FF File Offset: 0x0007E4FF
		public bool IsRaidPlan
		{
			get
			{
				return this._isRaidPlan;
			}
		}

		// Token: 0x17000300 RID: 768
		// (get) Token: 0x06001138 RID: 4408 RVA: 0x00080307 File Offset: 0x0007E507
		public int PlanCount
		{
			get
			{
				return this._planCount;
			}
		}

		// Token: 0x17000301 RID: 769
		// (get) Token: 0x06001139 RID: 4409 RVA: 0x0008030F File Offset: 0x0007E50F
		// (set) Token: 0x0600113A RID: 4410 RVA: 0x00080317 File Offset: 0x0007E517
		public bool IsPlanMade { get; private set; }

		// Token: 0x17000302 RID: 770
		// (get) Token: 0x0600113B RID: 4411 RVA: 0x00080320 File Offset: 0x0007E520
		// (set) Token: 0x0600113C RID: 4412 RVA: 0x00080328 File Offset: 0x0007E528
		public float SpawnPathOffset { get; private set; }

		// Token: 0x17000303 RID: 771
		// (get) Token: 0x0600113D RID: 4413 RVA: 0x00080331 File Offset: 0x0007E531
		// (set) Token: 0x0600113E RID: 4414 RVA: 0x00080339 File Offset: 0x0007E539
		public float TargetOffset { get; private set; }

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x0600113F RID: 4415 RVA: 0x00080344 File Offset: 0x0007E544
		public int TroopCount
		{
			get
			{
				int num = 0;
				foreach (NavalFormationDeploymentPlan navalFormationDeploymentPlan in this._formationPlans)
				{
					num += navalFormationDeploymentPlan.PlannedTroopCount;
				}
				return num;
			}
		}

		// Token: 0x17000305 RID: 773
		// (get) Token: 0x06001140 RID: 4416 RVA: 0x00080378 File Offset: 0x0007E578
		public int ShipCount
		{
			get
			{
				int num = 0;
				NavalFormationDeploymentPlan[] formationPlans = this._formationPlans;
				for (int i = 0; i < formationPlans.Length; i++)
				{
					if (formationPlans[i].HasShipObject)
					{
						num++;
					}
				}
				return num;
			}
		}

		// Token: 0x17000306 RID: 774
		// (get) Token: 0x06001141 RID: 4417 RVA: 0x000803AB File Offset: 0x0007E5AB
		public Vec3 MeanPosition
		{
			get
			{
				return this._meanPosition;
			}
		}

		// Token: 0x06001142 RID: 4418 RVA: 0x000803B3 File Offset: 0x0007E5B3
		public static NavalDeploymentPlan CreatePlan(Mission mission, Team team, bool isRiverPlan, bool isRaidPlan)
		{
			return new NavalDeploymentPlan(mission, team, isRiverPlan, isRaidPlan);
		}

		// Token: 0x06001143 RID: 4419 RVA: 0x000803C0 File Offset: 0x0007E5C0
		private NavalDeploymentPlan(Mission mission, Team team, bool isRiverPlan, bool isRaidPlan)
		{
			this._mission = mission;
			this._planCount = 0;
			this.Team = team;
			this._formationPlans = new NavalFormationDeploymentPlan[11];
			this._isRiverPlan = isRiverPlan;
			this._isRaidPlan = isRaidPlan;
			this.IsPlanMade = false;
			this.SpawnPathOffset = 0f;
			for (int i = 0; i < this._formationPlans.Length; i++)
			{
				FormationClass formationClass = i;
				this._formationPlans[i] = new NavalFormationDeploymentPlan(formationClass, this._mission);
			}
			this.ClearAddedShips();
			this.ClearPlan();
		}

		// Token: 0x06001144 RID: 4420 RVA: 0x0008044B File Offset: 0x0007E64B
		public void MakeDeploymentPlan(float spawnPathOffset, float targetOffset, FormationSceneSpawnEntry[,] formationSceneSpawnEntries = null)
		{
			this.SpawnPathOffset = spawnPathOffset;
			this.TargetOffset = targetOffset;
			if (this._mission.HasSpawnPath)
			{
				this.PlanNavalBattleDeploymentFromSpawnPath(spawnPathOffset, targetOffset);
			}
			else
			{
				this.PlanNavalBattleDeploymentFromSceneData(formationSceneSpawnEntries);
			}
			this.ComputeMeanPosition();
		}

		// Token: 0x06001145 RID: 4421 RVA: 0x00080480 File Offset: 0x0007E680
		public void ClearPlan()
		{
			NavalFormationDeploymentPlan[] formationPlans = this._formationPlans;
			for (int i = 0; i < formationPlans.Length; i++)
			{
				formationPlans[i].Clear();
			}
			this.IsPlanMade = false;
		}

		// Token: 0x06001146 RID: 4422 RVA: 0x000804B4 File Offset: 0x0007E6B4
		public void ClearAddedShips()
		{
			NavalFormationDeploymentPlan[] formationPlans = this._formationPlans;
			for (int i = 0; i < formationPlans.Length; i++)
			{
				formationPlans[i].SetShipOrigin(null);
			}
		}

		// Token: 0x06001147 RID: 4423 RVA: 0x000804E0 File Offset: 0x0007E6E0
		public void AddShip(FormationClass formationClass, IShipOrigin shipOrigin)
		{
			this._formationPlans[formationClass].SetShipOrigin(shipOrigin);
		}

		// Token: 0x06001148 RID: 4424 RVA: 0x00080500 File Offset: 0x0007E700
		public bool RemoveShip(FormationClass formationIndex)
		{
			NavalFormationDeploymentPlan navalFormationDeploymentPlan = this._formationPlans[formationIndex];
			if (navalFormationDeploymentPlan.ShipObject != null)
			{
				navalFormationDeploymentPlan.SetShipOrigin(null);
				return true;
			}
			return false;
		}

		// Token: 0x06001149 RID: 4425 RVA: 0x00080528 File Offset: 0x0007E728
		public NavalFormationDeploymentPlan GetFormationPlan(FormationClass fClass)
		{
			return this._formationPlans[fClass];
		}

		// Token: 0x0600114A RID: 4426 RVA: 0x00080534 File Offset: 0x0007E734
		public bool GetFormationDeploymentFrame(FormationClass fClass, out MatrixFrame frame)
		{
			NavalFormationDeploymentPlan formationPlan = this.GetFormationPlan(fClass);
			if (formationPlan.HasFrame())
			{
				frame = formationPlan.GetFrame();
				return true;
			}
			frame = MatrixFrame.Identity;
			return false;
		}

		// Token: 0x0600114B RID: 4427 RVA: 0x0008056C File Offset: 0x0007E76C
		private void PlanNavalBattleDeploymentFromSpawnPath(float pathOffset, float targetOffset)
		{
			Vec2 vec;
			Vec2 vec2;
			this._mission.GetInitialSpawnPathData(this.Team.Side).GetSpawnPathFrameFacingTarget(pathOffset, targetOffset, this._isRiverPlan, ref vec, ref vec2, false, 0.2f);
			this.DeployShips(vec, vec2);
			this.IsPlanMade = true;
			this._planCount++;
		}

		// Token: 0x0600114C RID: 4428 RVA: 0x000805C4 File Offset: 0x0007E7C4
		private void PlanNavalBattleDeploymentFromSceneData(FormationSceneSpawnEntry[,] formationSceneSpawnEntries)
		{
			if (formationSceneSpawnEntries == null || formationSceneSpawnEntries.GetLength(0) != 2 || formationSceneSpawnEntries.GetLength(1) != this._formationPlans.Length)
			{
				return;
			}
			int side = this.Team.Side;
			for (int i = 0; i < this._formationPlans.Length; i++)
			{
				NavalFormationDeploymentPlan navalFormationDeploymentPlan = this._formationPlans[i];
				if (navalFormationDeploymentPlan.HasShipObject)
				{
					MatrixFrame globalFrame = formationSceneSpawnEntries[side, i].SpawnEntity.GetGlobalFrame();
					NavalFormationDeploymentPlan navalFormationDeploymentPlan2 = navalFormationDeploymentPlan;
					Vec2 asVec = globalFrame.origin.AsVec2;
					Vec2 vec = globalFrame.rotation.f.AsVec2;
					vec = vec.Normalized();
					navalFormationDeploymentPlan2.SetFrame(in asVec, in vec);
				}
			}
			this.IsPlanMade = true;
			this._planCount++;
		}

		// Token: 0x0600114D RID: 4429 RVA: 0x00080680 File Offset: 0x0007E880
		private void DeployShips(Vec2 deployPosition, Vec2 deployDirection)
		{
			List<ValueTuple<int, NavalFormationDeploymentPlan>> list = new List<ValueTuple<int, NavalFormationDeploymentPlan>>();
			for (int i = 0; i < this._formationPlans.Count<NavalFormationDeploymentPlan>(); i++)
			{
				NavalFormationDeploymentPlan navalFormationDeploymentPlan = this._formationPlans[i];
				if (navalFormationDeploymentPlan.HasShipObject)
				{
					int totalCrewCapacity = navalFormationDeploymentPlan.ShipOrigin.TotalCrewCapacity;
					list.Add(new ValueTuple<int, NavalFormationDeploymentPlan>(totalCrewCapacity, navalFormationDeploymentPlan));
				}
			}
			list.Sort(([TupleElementNames(new string[] { "crewCapacity", "plan" })] ValueTuple<int, NavalFormationDeploymentPlan> x, [TupleElementNames(new string[] { "crewCapacity", "plan" })] ValueTuple<int, NavalFormationDeploymentPlan> y) => y.Item1.CompareTo(x.Item1));
			float num = 0f;
			float num2 = 0f;
			Vec2 vec = deployDirection.LeftVec().Normalized();
			Vec2 vec2 = -vec;
			int j = 0;
			if (list.Count % 2 != 0)
			{
				NavalFormationDeploymentPlan item = list[j].Item2;
				item.SetFrame(in deployPosition, in deployDirection);
				float num3 = item.ShipObject.DeploymentArea.x / 2f;
				num += num3;
				num2 += num3;
				j++;
			}
			while (j < list.Count)
			{
				NavalFormationDeploymentPlan item2 = list[j].Item2;
				float num4 = item2.ShipObject.DeploymentArea.x / 2f;
				if (j % 2 == 0)
				{
					num2 += 20f + num4;
					Vec2 vec3 = deployPosition + vec2 * num2;
					item2.SetFrame(in vec3, in deployDirection);
					num2 += num4;
				}
				else
				{
					num += 20f + num4;
					Vec2 vec4 = deployPosition + vec * num;
					item2.SetFrame(in vec4, in deployDirection);
					num += num4;
				}
				j++;
			}
			list.Clear();
		}

		// Token: 0x0600114E RID: 4430 RVA: 0x0008081C File Offset: 0x0007EA1C
		private void ComputeMeanPosition()
		{
			this._meanPosition = Vec3.Zero;
			Vec2 vec = Vec2.Zero;
			int num = 0;
			foreach (NavalFormationDeploymentPlan navalFormationDeploymentPlan in this._formationPlans)
			{
				if (navalFormationDeploymentPlan.HasFrame())
				{
					vec += navalFormationDeploymentPlan.GetPosition().AsVec2;
					num++;
				}
			}
			if (num > 0)
			{
				vec..ctor(vec.X / (float)num, vec.Y / (float)num);
				this._meanPosition = vec.ToVec3(0f);
			}
		}

		// Token: 0x040009F7 RID: 2551
		public const float HorizontalShipGap = 20f;

		// Token: 0x040009F8 RID: 2552
		public readonly Team Team;

		// Token: 0x040009FC RID: 2556
		private readonly Mission _mission;

		// Token: 0x040009FD RID: 2557
		private int _planCount;

		// Token: 0x040009FE RID: 2558
		private bool _isRiverPlan;

		// Token: 0x040009FF RID: 2559
		private bool _isRaidPlan;

		// Token: 0x04000A00 RID: 2560
		private Vec3 _meanPosition;

		// Token: 0x04000A01 RID: 2561
		private readonly NavalFormationDeploymentPlan[] _formationPlans;
	}
}
