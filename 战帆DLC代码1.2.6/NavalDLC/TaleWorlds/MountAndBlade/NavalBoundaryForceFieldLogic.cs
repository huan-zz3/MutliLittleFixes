using System;
using NavalDLC.Missions;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.NavalPhysics;
using NavalDLC.Missions.Objects;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000010 RID: 16
	public class NavalBoundaryForceFieldLogic : MissionLogic
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000091 RID: 145 RVA: 0x00006207 File Offset: 0x00004407
		public MBReadOnlyList<Vec2> HardBoundaryPoints
		{
			get
			{
				return this._hardBoundaryPoints;
			}
		}

		// Token: 0x06000092 RID: 146 RVA: 0x0000620F File Offset: 0x0000440F
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._hardBoundaryPoints = new MBList<Vec2>();
			this._navalShipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00006233 File Offset: 0x00004433
		public override void OnAfterDeploymentFinished()
		{
			this._hardBoundaryPoints = MBSceneUtilities.GetHardBoundaryPoints(Mission.Current.Scene);
		}

		// Token: 0x06000094 RID: 148 RVA: 0x0000624C File Offset: 0x0000444C
		public override void OnFixedMissionTick(float fixedDt)
		{
			if (base.Mission.IsDeploymentFinished)
			{
				float num = 0f;
				foreach (MissionShip missionShip in this._navalShipsLogic.AllShips)
				{
					num = MathF.Max(num, missionShip.Physics.PhysicsBoundingBoxWithChildren.radius);
				}
				float num2 = 20f + num;
				float num3 = num2 * num2;
				foreach (MissionShip missionShip2 in this._navalShipsLogic.AllShips)
				{
					if (missionShip2.IsShipOrderActive && missionShip2.ShipOrder.MovementOrderEnum != ShipOrder.ShipMovementOrderEnum.Retreat)
					{
						Vec3 origin = missionShip2.GameEntity.GetBodyWorldTransform().origin;
						Vec2 asVec = origin.AsVec2;
						Vec2 vec;
						bool flag;
						float num4 = MBSceneUtilities.FindClosestPointToBoundariesReturnDistanceSquared(ref asVec, this._hardBoundaryPoints, ref vec, ref flag);
						Vec3 vec2 = (asVec - vec).ToVec3(0f);
						if (num4 >= 1E-05f && num4 <= num3)
						{
							float num5 = vec2.Normalize();
							float radius = missionShip2.Physics.PhysicsBoundingBoxWithoutChildren.radius;
							float length = ((origin - vec2 * radius).AsVec2 - vec).Length;
							float num6 = MathF.Max(19.75f, 0.001f);
							if (length <= 20f)
							{
								float mass = missionShip2.Physics.Mass;
								float num7 = Vec3.DotProduct(missionShip2.Physics.LinearVelocity, -vec2);
								float num8 = 20f - (length - 0.25f);
								float num9 = MathF.Clamp(num8 / num6, 0f, 1f);
								float num10 = MathF.Clamp(num7 / 3f, 0f, 1f);
								float num11 = num9 * (0.5f + 0.5f * num10);
								if (num8 >= num6)
								{
									if (num7 > 0f)
									{
										Vec3 vec3 = vec2 * (num7 * mass);
										missionShip2.Physics.ApplyForceToDynamicBody(in vec3, 1);
										num7 = 0f;
									}
									float num12 = 4f * (num8 - num6);
									if (num12 > 0f)
									{
										float num13 = num12 - num7;
										if (num13 > 0f)
										{
											Vec3 vec4 = vec2 * (mass * num13);
											missionShip2.Physics.ApplyForceToDynamicBody(in vec4, 1);
										}
									}
								}
								if (num8 > 0f || num5 <= radius + 20f)
								{
									float num14 = 6f * (0.25f + 0.75f * num11);
									Vec3 vec5 = vec2 * (num14 * mass);
									missionShip2.Physics.ApplyForceToDynamicBody(in vec5, 0);
								}
								if (num7 > 0f)
								{
									NavalPhysics physics = missionShip2.Physics;
									Vec3 vec6 = vec2 * (2f * num7 * mass);
									physics.ApplyForceToDynamicBody(in vec6, 0);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0400005C RID: 92
		private const float SoftStart = 20f;

		// Token: 0x0400005D RID: 93
		private const float HardStop = 0.25f;

		// Token: 0x0400005E RID: 94
		private const float MaxAcceleleration = 6f;

		// Token: 0x0400005F RID: 95
		private const float VRef = 3f;

		// Token: 0x04000060 RID: 96
		private const float SeparationVelocityGain = 4f;

		// Token: 0x04000061 RID: 97
		private const float Damping = 2f;

		// Token: 0x04000062 RID: 98
		private MBList<Vec2> _hardBoundaryPoints;

		// Token: 0x04000063 RID: 99
		private NavalShipsLogic _navalShipsLogic;
	}
}
