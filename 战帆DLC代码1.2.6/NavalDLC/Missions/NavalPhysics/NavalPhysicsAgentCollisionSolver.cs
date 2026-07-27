using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.NavalPhysics
{
	// Token: 0x020000C1 RID: 193
	public class NavalPhysicsAgentCollisionSolver : ScriptComponentBehavior
	{
		// Token: 0x06000EAB RID: 3755 RVA: 0x0007282C File Offset: 0x00070A2C
		protected override void OnInit()
		{
			this._nearbyAgentsCache = new MBList<Agent>(5);
			this._floatableEntityNavalPhysicsScript = base.GameEntity.GetFirstScriptOfType<NavalPhysics>();
			this._floatableMeshBoundingBoxGlobalVertices = new Vec3[8];
			this._forceToBeAppliedOnFixedTick = Vec3.Zero;
			this._torqueToBeAppliedOnFixedTick = Vec3.Zero;
		}

		// Token: 0x06000EAC RID: 3756 RVA: 0x0007287B File Offset: 0x00070A7B
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 48 | base.GetTickRequirement();
		}

		// Token: 0x06000EAD RID: 3757 RVA: 0x00072888 File Offset: 0x00070A88
		private bool IsPointInsideLocalBoundingBox(MatrixFrame globalFrame, Vec3 point, float margin)
		{
			Vec3 vec = globalFrame.TransformToLocal(ref point);
			BoundingBox physicsBoundingBoxWithChildren = this._floatableEntityNavalPhysicsScript.PhysicsBoundingBoxWithChildren;
			return vec.x > physicsBoundingBoxWithChildren.min.x - margin && vec.y > physicsBoundingBoxWithChildren.min.y - margin && vec.z > physicsBoundingBoxWithChildren.min.z - margin && vec.x - margin < physicsBoundingBoxWithChildren.max.x && vec.y - margin < physicsBoundingBoxWithChildren.max.y && vec.z - margin < physicsBoundingBoxWithChildren.max.z;
		}

		// Token: 0x06000EAE RID: 3758 RVA: 0x0007292C File Offset: 0x00070B2C
		private void UpdateFloatableMeshBoundingBoxGlobalVertices(MatrixFrame globalFrame)
		{
			BoundingBox physicsBoundingBoxWithChildren = this._floatableEntityNavalPhysicsScript.PhysicsBoundingBoxWithChildren;
			Vec3[] floatableMeshBoundingBoxGlobalVertices = this._floatableMeshBoundingBoxGlobalVertices;
			int num = 0;
			Vec3 vec = new Vec3(physicsBoundingBoxWithChildren.min.x, physicsBoundingBoxWithChildren.min.y, physicsBoundingBoxWithChildren.min.z, -1f);
			floatableMeshBoundingBoxGlobalVertices[num] = globalFrame.TransformToParent(ref vec);
			Vec3[] floatableMeshBoundingBoxGlobalVertices2 = this._floatableMeshBoundingBoxGlobalVertices;
			int num2 = 1;
			vec = new Vec3(physicsBoundingBoxWithChildren.min.x, physicsBoundingBoxWithChildren.max.y, physicsBoundingBoxWithChildren.min.z, -1f);
			floatableMeshBoundingBoxGlobalVertices2[num2] = globalFrame.TransformToParent(ref vec);
			Vec3[] floatableMeshBoundingBoxGlobalVertices3 = this._floatableMeshBoundingBoxGlobalVertices;
			int num3 = 2;
			vec = new Vec3(physicsBoundingBoxWithChildren.max.x, physicsBoundingBoxWithChildren.max.y, physicsBoundingBoxWithChildren.min.z, -1f);
			floatableMeshBoundingBoxGlobalVertices3[num3] = globalFrame.TransformToParent(ref vec);
			Vec3[] floatableMeshBoundingBoxGlobalVertices4 = this._floatableMeshBoundingBoxGlobalVertices;
			int num4 = 3;
			vec = new Vec3(physicsBoundingBoxWithChildren.max.x, physicsBoundingBoxWithChildren.min.y, physicsBoundingBoxWithChildren.min.z, -1f);
			floatableMeshBoundingBoxGlobalVertices4[num4] = globalFrame.TransformToParent(ref vec);
			Vec3[] floatableMeshBoundingBoxGlobalVertices5 = this._floatableMeshBoundingBoxGlobalVertices;
			int num5 = 4;
			vec = new Vec3(physicsBoundingBoxWithChildren.min.x, physicsBoundingBoxWithChildren.min.y, physicsBoundingBoxWithChildren.max.z, -1f);
			floatableMeshBoundingBoxGlobalVertices5[num5] = globalFrame.TransformToParent(ref vec);
			Vec3[] floatableMeshBoundingBoxGlobalVertices6 = this._floatableMeshBoundingBoxGlobalVertices;
			int num6 = 5;
			vec = new Vec3(physicsBoundingBoxWithChildren.min.x, physicsBoundingBoxWithChildren.max.y, physicsBoundingBoxWithChildren.max.z, -1f);
			floatableMeshBoundingBoxGlobalVertices6[num6] = globalFrame.TransformToParent(ref vec);
			Vec3[] floatableMeshBoundingBoxGlobalVertices7 = this._floatableMeshBoundingBoxGlobalVertices;
			int num7 = 6;
			vec = new Vec3(physicsBoundingBoxWithChildren.max.x, physicsBoundingBoxWithChildren.max.y, physicsBoundingBoxWithChildren.max.z, -1f);
			floatableMeshBoundingBoxGlobalVertices7[num7] = globalFrame.TransformToParent(ref vec);
			Vec3[] floatableMeshBoundingBoxGlobalVertices8 = this._floatableMeshBoundingBoxGlobalVertices;
			int num8 = 7;
			vec = new Vec3(physicsBoundingBoxWithChildren.max.x, physicsBoundingBoxWithChildren.min.y, physicsBoundingBoxWithChildren.max.z, -1f);
			floatableMeshBoundingBoxGlobalVertices8[num8] = globalFrame.TransformToParent(ref vec);
		}

		// Token: 0x06000EAF RID: 3759 RVA: 0x00072B4D File Offset: 0x00070D4D
		protected override void OnFixedTick(float fixedDt)
		{
			if (this._forceToBeAppliedOnFixedTick.LengthSquared > 0f)
			{
				this._floatableEntityNavalPhysicsScript.ApplyForceToDynamicBody(in this._forceToBeAppliedOnFixedTick, 0);
				this._floatableEntityNavalPhysicsScript.ApplyTorque(in this._torqueToBeAppliedOnFixedTick, 0);
			}
		}

		// Token: 0x06000EB0 RID: 3760 RVA: 0x00072B88 File Offset: 0x00070D88
		protected override void OnParallelFixedTick(float fixedDt)
		{
			this._forceToBeAppliedOnFixedTick = Vec3.Zero;
			this._torqueToBeAppliedOnFixedTick = Vec3.Zero;
			BoundingBox physicsBoundingBoxWithChildren = this._floatableEntityNavalPhysicsScript.PhysicsBoundingBoxWithChildren;
			MatrixFrame globalMassFrame = this._floatableEntityNavalPhysicsScript.GetGlobalMassFrame();
			MatrixFrame bodyWorldTransform = base.GameEntity.GetBodyWorldTransform();
			this.UpdateFloatableMeshBoundingBoxGlobalVertices(bodyWorldTransform);
			Vec3 vec = Vec3.Vec3Max(physicsBoundingBoxWithChildren.min, physicsBoundingBoxWithChildren.max);
			Mission.Current.GetNearbyAgents(bodyWorldTransform.origin.AsVec2, vec.Length + 0.6f, this._nearbyAgentsCache);
			foreach (Agent agent in this._nearbyAgentsCache)
			{
				if (agent.IsInWater())
				{
					Vec3 eyeGlobalPosition = agent.GetEyeGlobalPosition();
					Vec3 vec2 = Vec3.Invalid;
					float num = float.MaxValue;
					if (this.IsPointInsideLocalBoundingBox(bodyWorldTransform, eyeGlobalPosition, -0.05f))
					{
						Vec3 vec3 = -agent.Frame.rotation.f;
						float num2 = MathF.Min(this._floatableEntityNavalPhysicsScript.Mass, agent.GetTotalMass());
						Vec3 vec4 = vec3 * num2 * 2f * 5f;
						this._forceToBeAppliedOnFixedTick += vec4;
					}
					else
					{
						for (int i = 0; i < 4; i++)
						{
							Vec3 vec5 = this._floatableMeshBoundingBoxGlobalVertices[i];
							Vec3 vec6 = this._floatableMeshBoundingBoxGlobalVertices[(i + 1) % 4];
							Vec3 closestPointOnLineSegmentToPoint = MBMath.GetClosestPointOnLineSegmentToPoint(ref vec5, ref vec6, ref eyeGlobalPosition);
							float num3 = closestPointOnLineSegmentToPoint.DistanceSquared(eyeGlobalPosition);
							if (num3 < num)
							{
								num = num3;
								vec2 = closestPointOnLineSegmentToPoint;
							}
							Vec3 vec7 = this._floatableMeshBoundingBoxGlobalVertices[i + 4];
							Vec3 vec8 = this._floatableMeshBoundingBoxGlobalVertices[(i + 1) % 4 + 4];
							Vec3 closestPointOnLineSegmentToPoint2 = MBMath.GetClosestPointOnLineSegmentToPoint(ref vec7, ref vec8, ref eyeGlobalPosition);
							float num4 = closestPointOnLineSegmentToPoint2.DistanceSquared(eyeGlobalPosition);
							if (num4 < num)
							{
								num = num4;
								vec2 = closestPointOnLineSegmentToPoint2;
							}
							Vec3 closestPointOnLineSegmentToPoint3 = MBMath.GetClosestPointOnLineSegmentToPoint(ref vec5, ref vec7, ref eyeGlobalPosition);
							float num5 = closestPointOnLineSegmentToPoint3.DistanceSquared(eyeGlobalPosition);
							if (num5 < num)
							{
								num = num5;
								vec2 = closestPointOnLineSegmentToPoint3;
							}
						}
						if (num < 0.36f)
						{
							Vec3 vec9 = vec2 - eyeGlobalPosition;
							float num6 = vec9.Normalize();
							float num7 = 0.6f - num6;
							float num8 = MathF.Min(this._floatableEntityNavalPhysicsScript.Mass, agent.GetTotalMass());
							Vec3 vec10 = vec9 * num8 * 2f / MathF.Max(0.25f, num7);
							Vec3 vec11 = Vec3.CrossProduct(vec2 - globalMassFrame.origin, vec10);
							this._forceToBeAppliedOnFixedTick += vec10;
							this._torqueToBeAppliedOnFixedTick += vec11;
						}
					}
				}
			}
		}

		// Token: 0x0400091F RID: 2335
		private const float CutoffDistance = 0.6f;

		// Token: 0x04000920 RID: 2336
		private const float CollisionAcceleration = 2f;

		// Token: 0x04000921 RID: 2337
		private NavalPhysics _floatableEntityNavalPhysicsScript;

		// Token: 0x04000922 RID: 2338
		private MBList<Agent> _nearbyAgentsCache;

		// Token: 0x04000923 RID: 2339
		private Vec3[] _floatableMeshBoundingBoxGlobalVertices;

		// Token: 0x04000924 RID: 2340
		private Vec3 _forceToBeAppliedOnFixedTick;

		// Token: 0x04000925 RID: 2341
		private Vec3 _torqueToBeAppliedOnFixedTick;
	}
}
