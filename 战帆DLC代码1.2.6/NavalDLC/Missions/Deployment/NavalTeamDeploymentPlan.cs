using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.Deployment
{
	// Token: 0x020000E1 RID: 225
	public class NavalTeamDeploymentPlan : ITeamDeploymentPlan
	{
		// Token: 0x17000310 RID: 784
		// (get) Token: 0x060011C2 RID: 4546 RVA: 0x0008210F File Offset: 0x0008030F
		// (set) Token: 0x060011C3 RID: 4547 RVA: 0x00082117 File Offset: 0x00080317
		public Team Team { get; private set; }

		// Token: 0x060011C4 RID: 4548 RVA: 0x00082120 File Offset: 0x00080320
		internal NavalTeamDeploymentPlan(Mission mission, Team team)
		{
			this._mission = mission;
			this.Team = team;
			this._deploymentFrame = MatrixFrame.Identity;
			this._deploymentWidth = 0f;
			this._deploymentDepth = 0f;
			this._meanBoundaryPositions = new MBList<Vec2>();
			bool flag = mission.MissionTeamAIType == 4 && this._mission.Scene.GetNavmeshFaceCountBetweenTwoIds(1, 1) > 0;
			bool flag2 = mission.MissionTeamAIType == 5;
			this._initialPlan = NavalDeploymentPlan.CreatePlan(this._mission, team, flag, flag2);
			this._deploymentBoundaries.Clear();
		}

		// Token: 0x060011C5 RID: 4549 RVA: 0x000821C3 File Offset: 0x000803C3
		public void MakeDeploymentPlan(float spawnPathOffset, float targetOffset = 0f, FormationSceneSpawnEntry[,] formationSpawnEntries = null, bool isReinforcement = false)
		{
			this._initialPlan.MakeDeploymentPlan(spawnPathOffset, targetOffset, formationSpawnEntries);
			this.PlanDeploymentZone();
		}

		// Token: 0x060011C6 RID: 4550 RVA: 0x000821D9 File Offset: 0x000803D9
		public void ClearPlan(bool isReinforcement = false)
		{
			this._initialPlan.ClearPlan();
			this._meanBoundaryPositions.Clear();
		}

		// Token: 0x060011C7 RID: 4551 RVA: 0x000821F1 File Offset: 0x000803F1
		public void ClearAddedShips()
		{
			this._initialPlan.ClearAddedShips();
		}

		// Token: 0x060011C8 RID: 4552 RVA: 0x000821FE File Offset: 0x000803FE
		internal void AddShip(FormationClass formationClass, IShipOrigin shipOrigin)
		{
			this._initialPlan.AddShip(formationClass, shipOrigin);
		}

		// Token: 0x060011C9 RID: 4553 RVA: 0x0008220D File Offset: 0x0008040D
		internal bool RemoveShip(FormationClass formationIndex)
		{
			return this._initialPlan.RemoveShip(formationIndex);
		}

		// Token: 0x060011CA RID: 4554 RVA: 0x0008221B File Offset: 0x0008041B
		public int GetShipCount()
		{
			return this._initialPlan.ShipCount;
		}

		// Token: 0x060011CB RID: 4555 RVA: 0x00082228 File Offset: 0x00080428
		public bool IsFirstPlan(bool isReinforcement = false)
		{
			return this._initialPlan.PlanCount == 1;
		}

		// Token: 0x060011CC RID: 4556 RVA: 0x00082238 File Offset: 0x00080438
		public bool IsPlanMade(bool isReinforcement = false)
		{
			return this._initialPlan.IsPlanMade;
		}

		// Token: 0x060011CD RID: 4557 RVA: 0x00082245 File Offset: 0x00080445
		[return: TupleElementNames(new string[] { "id", "points" })]
		public MBReadOnlyList<ValueTuple<string, MBList<Vec2>>> GetDeploymentBoundaries()
		{
			return this._deploymentBoundaries;
		}

		// Token: 0x060011CE RID: 4558 RVA: 0x0008224D File Offset: 0x0008044D
		public float GetSpawnPathOffset(bool isReinforcement = false)
		{
			return this._initialPlan.SpawnPathOffset;
		}

		// Token: 0x060011CF RID: 4559 RVA: 0x0008225A File Offset: 0x0008045A
		public float GetTargetOffset(bool isReinforcement = false)
		{
			return this._initialPlan.TargetOffset;
		}

		// Token: 0x060011D0 RID: 4560 RVA: 0x00082267 File Offset: 0x00080467
		public MatrixFrame GetDeploymentFrame()
		{
			return this._deploymentFrame;
		}

		// Token: 0x060011D1 RID: 4561 RVA: 0x0008226F File Offset: 0x0008046F
		public bool HasDeploymentBoundaries()
		{
			return !Extensions.IsEmpty<ValueTuple<string, MBList<Vec2>>>(this._deploymentBoundaries);
		}

		// Token: 0x060011D2 RID: 4562 RVA: 0x0008227F File Offset: 0x0008047F
		public IFormationDeploymentPlan GetFormationPlan(FormationClass fClass, bool isReinforcement = false)
		{
			return this._initialPlan.GetFormationPlan(fClass);
		}

		// Token: 0x060011D3 RID: 4563 RVA: 0x0008228D File Offset: 0x0008048D
		public Vec3 GetMeanPosition(bool isReinforcement = false)
		{
			return this._initialPlan.MeanPosition;
		}

		// Token: 0x060011D4 RID: 4564 RVA: 0x0008229C File Offset: 0x0008049C
		public bool IsPositionInsideDeploymentBoundaries(in Vec2 position, [TupleElementNames(new string[] { "id", "points" })] out ValueTuple<string, MBList<Vec2>> containingBoundaryTuple)
		{
			bool flag = false;
			containingBoundaryTuple = new ValueTuple<string, MBList<Vec2>>("", null);
			foreach (ValueTuple<string, MBList<Vec2>> valueTuple in this._deploymentBoundaries)
			{
				MBList<Vec2> item = valueTuple.Item2;
				if (MBSceneUtilities.IsPointInsideBoundaries(ref position, item, 0.05f))
				{
					containingBoundaryTuple = valueTuple;
					flag = true;
					break;
				}
			}
			return flag;
		}

		// Token: 0x060011D5 RID: 4565 RVA: 0x0008231C File Offset: 0x0008051C
		public Vec2 GetClosestDeploymentBoundaryPosition(in Vec2 position)
		{
			Vec2 vec = position;
			float num = float.MaxValue;
			foreach (ValueTuple<string, MBList<Vec2>> valueTuple in this._deploymentBoundaries)
			{
				MBList<Vec2> item = valueTuple.Item2;
				if (item.Count > 2)
				{
					Vec2 vec2;
					float num2 = MBSceneUtilities.FindClosestPointToBoundaries(ref position, item, ref vec2);
					if (num2 < num)
					{
						num = num2;
						vec = vec2;
					}
				}
			}
			return vec;
		}

		// Token: 0x060011D6 RID: 4566 RVA: 0x0008239C File Offset: 0x0008059C
		public Vec2 GetMeanBoundaryPosition(int boundaryIndex = 0)
		{
			return this._meanBoundaryPositions[boundaryIndex];
		}

		// Token: 0x060011D7 RID: 4567 RVA: 0x000823AC File Offset: 0x000805AC
		private void PlanDeploymentZone()
		{
			Vec3 vec = Vec3.Zero;
			Vec2 vec2 = Vec2.Zero;
			int num = 0;
			for (int i = 0; i < 10; i++)
			{
				FormationClass formationClass = i;
				NavalFormationDeploymentPlan formationPlan = this._initialPlan.GetFormationPlan(formationClass);
				if (formationPlan.HasFrame())
				{
					vec += formationPlan.GetPosition();
					vec2 += formationPlan.GetDirection();
					num++;
				}
			}
			vec /= (float)num;
			Vec3 vec3 = vec2.ToVec3(0f);
			vec3 = vec3.NormalizedCopy();
			Mat3 mat = Mat3.CreateMat3WithForward(ref vec3);
			this._deploymentFrame = new MatrixFrame(ref mat, ref vec);
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			for (int j = 0; j < 10; j++)
			{
				FormationClass formationClass2 = j;
				IFormationDeploymentPlan formationPlan2 = this.GetFormationPlan(formationClass2, false);
				float num6 = formationPlan2.PlannedDepth / 2f;
				float num7 = formationPlan2.PlannedWidth / 2f;
				if (formationPlan2.HasFrame())
				{
					MatrixFrame frame = formationPlan2.GetFrame();
					MatrixFrame matrixFrame = this._deploymentFrame.TransformToLocal(ref frame);
					num2 = Math.Max(matrixFrame.origin.y + num6, num2);
					num3 = Math.Min(matrixFrame.origin.y - num6, num3);
					num4 = Math.Max(matrixFrame.origin.x + num7, num4);
					num5 = Math.Min(matrixFrame.origin.x - num7, num5);
				}
			}
			float num8 = num4 + MathF.Abs(num5);
			float num9 = num2 + MathF.Abs(num3);
			this._deploymentFrame.Advance(num2 + 50f);
			this._deploymentBoundaries.Clear();
			this._meanBoundaryPositions.Clear();
			if (this._initialPlan.IsRiverPlan)
			{
				this._deploymentWidth = 200f;
			}
			else
			{
				this._deploymentWidth = Math.Max(num8, 400f);
			}
			this._deploymentDepth = 50f + MathF.Max(100f, num9);
			foreach (KeyValuePair<string, ICollection<Vec2>> keyValuePair in this._mission.Boundaries)
			{
				string key = keyValuePair.Key;
				ICollection<Vec2> value = keyValuePair.Value;
				MBList<Vec2> mblist = this.ComputeDeploymentBoundariesFromMissionBoundaries(value);
				this._deploymentBoundaries.Add(new ValueTuple<string, MBList<Vec2>>(key, mblist));
				Vec2 vec4;
				vec4..ctor(mblist.Average<Vec2>((Vec2 v) => v.x), mblist.Average<Vec2>((Vec2 v) => v.y));
				this._meanBoundaryPositions.Add(vec4);
			}
			this._deploymentFrame.origin.z = this._mission.Scene.GetWaterLevelAtPosition(this._deploymentFrame.origin.AsVec2, true, false);
		}

		// Token: 0x060011D8 RID: 4568 RVA: 0x000826B8 File Offset: 0x000808B8
		private MBList<Vec2> ComputeDeploymentBoundariesFromMissionBoundaries(ICollection<Vec2> missionBoundaries)
		{
			MBList<Vec2> mblist = new MBList<Vec2>();
			if (missionBoundaries.Count > 2)
			{
				Vec2 asVec = this._deploymentFrame.origin.AsVec2;
				Vec2 vec = this._deploymentFrame.rotation.s.AsVec2.Normalized();
				Vec2 vec2 = this._deploymentFrame.rotation.f.AsVec2.Normalized();
				Vec2 vec3 = asVec - this._deploymentDepth / 2f * vec2;
				MBList<Vec2> mblist2 = new MBList<Vec2>();
				Vec2 vec4 = asVec - this._deploymentWidth / 2f * vec;
				mblist2.Add(vec4);
				Vec2 vec5 = vec4 - vec2 * this._deploymentDepth;
				mblist2.Add(vec5);
				Vec2 vec6 = vec5 + vec * this._deploymentWidth;
				mblist2.Add(vec6);
				Vec2 vec7 = vec6 + vec2 * this._deploymentDepth;
				mblist2.Add(vec7);
				MBList<Vec2> mblist3 = Extensions.ToMBList<Vec2>(missionBoundaries);
				foreach (Vec2 vec8 in mblist2)
				{
					if (MBSceneUtilities.IsPointInsideBoundaries(ref vec8, mblist3, 0.05f))
					{
						this.AddDeploymentBoundaryPoint(mblist, vec8);
					}
					else
					{
						Vec2 vec9 = (vec3 - vec8).Normalized();
						Vec2 vec10 = ((Vec2.DotProduct(vec9, vec) >= 0f) ? vec : (-vec));
						Vec2 vec11;
						if (MBMath.IntersectRayWithPolygon(vec8, vec10, mblist3, ref vec11))
						{
							this.AddDeploymentBoundaryPoint(mblist, vec11);
						}
						Vec2 vec12 = ((Vec2.DotProduct(vec9, vec2) >= 0f) ? vec2 : (-vec2));
						Vec2 vec13;
						if (MBMath.IntersectRayWithPolygon(vec8, vec12, mblist3, ref vec13))
						{
							this.AddDeploymentBoundaryPoint(mblist, vec13);
						}
					}
				}
				foreach (Vec2 vec14 in mblist3)
				{
					if (MBSceneUtilities.IsPointInsideBoundaries(ref vec14, mblist2, 0.05f))
					{
						this.AddDeploymentBoundaryPoint(mblist, vec14);
					}
				}
				MBSceneUtilities.RadialSortBoundary(ref mblist);
				MBSceneUtilities.FindConvexHull(ref mblist);
			}
			return mblist;
		}

		// Token: 0x060011D9 RID: 4569 RVA: 0x000828FC File Offset: 0x00080AFC
		private void AddDeploymentBoundaryPoint(MBList<Vec2> deploymentBoundaries, Vec2 point)
		{
			if (!deploymentBoundaries.Exists((Vec2 boundaryPoint) => boundaryPoint.Distance(point) <= 0.1f))
			{
				deploymentBoundaries.Add(point);
			}
		}

		// Token: 0x060011DA RID: 4570 RVA: 0x00082936 File Offset: 0x00080B36
		bool ITeamDeploymentPlan.IsPositionInsideDeploymentBoundaries(in Vec2 position, [TupleElementNames(new string[] { "id", "points" })] out ValueTuple<string, MBList<Vec2>> containingBoundaryTuple)
		{
			return this.IsPositionInsideDeploymentBoundaries(in position, out containingBoundaryTuple);
		}

		// Token: 0x060011DB RID: 4571 RVA: 0x00082940 File Offset: 0x00080B40
		Vec2 ITeamDeploymentPlan.GetClosestDeploymentBoundaryPosition(in Vec2 position)
		{
			return this.GetClosestDeploymentBoundaryPosition(in position);
		}

		// Token: 0x04000A14 RID: 2580
		public const float DeployZoneMinimumWidth = 400f;

		// Token: 0x04000A15 RID: 2581
		public const float RiverSceneDeployZoneFixedWidth = 200f;

		// Token: 0x04000A16 RID: 2582
		public const float DeployZoneForwardMargin = 50f;

		// Token: 0x04000A17 RID: 2583
		public const float DeployZoneBackwardMargin = 100f;

		// Token: 0x04000A19 RID: 2585
		private Mission _mission;

		// Token: 0x04000A1A RID: 2586
		private readonly NavalDeploymentPlan _initialPlan;

		// Token: 0x04000A1B RID: 2587
		[TupleElementNames(new string[] { "id", "points" })]
		private readonly MBList<ValueTuple<string, MBList<Vec2>>> _deploymentBoundaries = new MBList<ValueTuple<string, MBList<Vec2>>>();

		// Token: 0x04000A1C RID: 2588
		private MatrixFrame _deploymentFrame;

		// Token: 0x04000A1D RID: 2589
		private float _deploymentWidth;

		// Token: 0x04000A1E RID: 2590
		private float _deploymentDepth;

		// Token: 0x04000A1F RID: 2591
		private MBList<Vec2> _meanBoundaryPositions;
	}
}
