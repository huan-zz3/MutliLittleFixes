using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace ProjectileTrajectorySystem
{
	// Token: 0x02000008 RID: 8
	[NullableContext(1)]
	[Nullable(0)]
	public static class ProjectileTrajectorySystem
	{
		// Token: 0x06000030 RID: 48 RVA: 0x0000480C File Offset: 0x00002A0C
		private static void InitDebugRender()
		{
			bool flag = ProjectileTrajectorySystem._renderLineMethod != null;
			if (!flag)
			{
				try
				{
					Type type = Type.GetType("TaleWorlds.Engine.EngineApplicationInterface, TaleWorlds.Engine");
					bool flag2 = type == null;
					if (!flag2)
					{
						FieldInfo field = type.GetField("IDebug", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
						object obj = ((field != null) ? field.GetValue(null) : null);
						ProjectileTrajectorySystem._debugInterface = obj;
						ProjectileTrajectorySystem._renderLineMethod = ((obj != null) ? obj.GetType().GetMethod("RenderDebugLine", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
						{
							typeof(Vec3),
							typeof(Vec3),
							typeof(uint),
							typeof(bool),
							typeof(float)
						}, null) : null);
						ProjectileTrajectorySystem._renderSphereMethod = ((obj != null) ? obj.GetType().GetMethod("RenderDebugSphere", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
						{
							typeof(Vec3),
							typeof(float),
							typeof(uint),
							typeof(bool),
							typeof(float)
						}, null) : null);
					}
				}
				catch
				{
				}
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x0000495C File Offset: 0x00002B5C
		private static void RenderSphere(Vec3 pos, float radius, uint color)
		{
			bool flag = ProjectileTrajectorySystem._renderSphereMethod == null;
			if (flag)
			{
				ProjectileTrajectorySystem.InitDebugRender();
			}
			try
			{
				MethodInfo renderSphereMethod = ProjectileTrajectorySystem._renderSphereMethod;
				if (renderSphereMethod != null)
				{
					renderSphereMethod.Invoke(ProjectileTrajectorySystem._debugInterface, new object[] { pos, radius, color, false, 0f });
				}
			}
			catch
			{
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x000049E8 File Offset: 0x00002BE8
		public static void RenderOneLine(Vec3 start, Vec3 end, uint color)
		{
			bool flag = ProjectileTrajectorySystem._renderLineMethod == null;
			if (flag)
			{
				ProjectileTrajectorySystem.InitDebugRender();
			}
			try
			{
				MethodInfo renderLineMethod = ProjectileTrajectorySystem._renderLineMethod;
				if (renderLineMethod != null)
				{
					renderLineMethod.Invoke(ProjectileTrajectorySystem._debugInterface, new object[]
					{
						start,
						end - start,
						color,
						false,
						0f
					});
				}
			}
			catch
			{
			}
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000033 RID: 51 RVA: 0x00004A78 File Offset: 0x00002C78
		private static bool IsHandheldEnabled
		{
			get
			{
				PTSettings instance = GlobalSettings<PTSettings>.Instance;
				return instance == null || instance.EnableHandheld;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000034 RID: 52 RVA: 0x00004A8B File Offset: 0x00002C8B
		private static bool IsBallistaEnabled
		{
			get
			{
				PTSettings instance = GlobalSettings<PTSettings>.Instance;
				return instance == null || instance.EnableBallista;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000035 RID: 53 RVA: 0x00004A9E File Offset: 0x00002C9E
		private static bool IsMangonelEnabled
		{
			get
			{
				PTSettings instance = GlobalSettings<PTSettings>.Instance;
				return instance == null || instance.EnableMangonel;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000036 RID: 54 RVA: 0x00004AB1 File Offset: 0x00002CB1
		private static bool IsNavalAimEnabled
		{
			get
			{
				PTSettings instance = GlobalSettings<PTSettings>.Instance;
				return instance == null || instance.EnableNavalAutoAim;
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00004AC4 File Offset: 0x00002CC4
		private static void SimulateTrajectory(Vec3 start, Vec3 velocity, float friction, float mass, float ignoreTime, Action<Vec3> onHit, bool drawPath, bool useQuadraticDrag)
		{
			velocity *= 0.9f;
			Vec3 vec = start;
			Vec3 vec2;
			vec2..ctor(0f, 0f, -9.806f, -1f);
			float num = 0.02f;
			for (float num2 = 0f; num2 < 20f; num2 += num)
			{
				float length = velocity.Length;
				bool flag = length > 0.001f;
				if (flag)
				{
					if (useQuadraticDrag)
					{
						float num3 = MathF.Max(0.1f, mass);
						float num4 = friction * length * length;
						float num5 = num4 / num3;
						velocity -= velocity.NormalizedCopy() * (num5 * num);
					}
					else
					{
						velocity *= MathF.Max(0f, 1f - friction * num);
					}
				}
				velocity += vec2 * num;
				Vec3 vec3 = vec + velocity * num;
				bool flag2 = num2 > ignoreTime;
				if (flag2)
				{
					float num6;
					Vec3 vec4;
					WeakGameEntity weakGameEntity;
					bool flag3 = Mission.Current.Scene.RayCastForClosestEntityOrTerrain(vec, vec3, ref num6, ref vec4, ref weakGameEntity, 0.01f, 79617);
					if (flag3)
					{
						onHit(vec4);
						break;
					}
				}
				bool flag4 = drawPath && num2 > ignoreTime;
				if (flag4)
				{
					ProjectileTrajectorySystem.RenderOneLine(vec, vec3, uint.MaxValue);
				}
				vec = vec3;
				bool flag5 = vec.z < -100f;
				if (flag5)
				{
					break;
				}
			}
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00004C34 File Offset: 0x00002E34
		public static void UpdateShipCaptainModeTrajectory(RangedSiegeWeapon weapon, Vec3 forcedDirection, bool isRtsMode)
		{
			bool flag = weapon == null || !weapon.GameEntity.IsValid;
			if (!flag)
			{
				string text = weapon.GetType().Name.ToLower();
				bool isLobber = text.Contains("mangonel") || text.Contains("trebuchet") || text.Contains("onager");
				float shootingSpeed = ProjectileTrajectorySystem.GetShootingSpeed(weapon);
				float dynamicFriction = ProjectileTrajectorySystem.GetDynamicFriction(weapon, isLobber);
				float num = 1f;
				ItemObject ammoItem = ProjectileTrajectorySystem.GetAmmoItem(weapon);
				bool flag2 = ammoItem != null;
				if (flag2)
				{
					num = ammoItem.Weight;
				}
				bool flag3 = isLobber && num < 5f;
				if (flag3)
				{
					num = 40f;
				}
				Vec3 realMuzzlePosition = ProjectileTrajectorySystem.GetRealMuzzlePosition(weapon);
				Vec3 parentVelocity = ProjectileTrajectorySystem.GetParentVelocity(weapon.GameEntity);
				Vec3 vec = forcedDirection * shootingSpeed + parentVelocity;
				ProjectileTrajectorySystem.SimulateTrajectory(realMuzzlePosition, vec, dynamicFriction, num, 0.1f, delegate(Vec3 hitPos)
				{
					bool flag4 = isLobber | isRtsMode;
					if (flag4)
					{
						Vec3 vec2 = ProjectileTrajectorySystem.SampleSurfaceNormal(hitPos);
						ProjectileTrajectorySystem.DrawImprintedRing(hitPos, vec2, 3f, 4294919424U);
					}
					else
					{
						ProjectileTrajectorySystem.RenderSphere(hitPos, 0.4f, 4294901760U);
					}
				}, isRtsMode, isLobber);
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00004D58 File Offset: 0x00002F58
		public static void LogFiringDetails(RangedSiegeWeapon weapon, Vec3 direction)
		{
			bool flag = weapon == null;
			if (!flag)
			{
				float shootingSpeed = ProjectileTrajectorySystem.GetShootingSpeed(weapon);
				float dynamicFriction = ProjectileTrajectorySystem.GetDynamicFriction(weapon, false);
				float num = 1f;
				ItemObject ammoItem = ProjectileTrajectorySystem.GetAmmoItem(weapon);
				bool flag2 = ammoItem != null;
				if (flag2)
				{
					num = ammoItem.Weight;
				}
				Vec3 realMuzzlePosition = ProjectileTrajectorySystem.GetRealMuzzlePosition(weapon);
				float length = direction.AsVec2.Length;
				float num2 = ((length > 0.0001f) ? (MathF.Atan2(direction.z, length) * 57.295776f) : 90f);
				Vec3 predictedHit = Vec3.Zero;
				bool gotHit = false;
				ProjectileTrajectorySystem.SimulateTrajectory(realMuzzlePosition, direction * shootingSpeed, dynamicFriction, num, 0.1f, delegate(Vec3 hit)
				{
					predictedHit = hit;
					gotHit = true;
				}, false, false);
				bool gotHit2 = gotHit;
				if (gotHit2)
				{
					InformationManager.DisplayMessage(new InformationMessage(string.Format("[理论] 速:{0:F0} | 阻力:{1:F6} | 质量:{2:F1} | 仰角:{3:F1}° | 预测Z:{4:F1}", new object[] { shootingSpeed, dynamicFriction, num, num2, predictedHit.z }), Colors.Cyan));
				}
				else
				{
					InformationManager.DisplayMessage(new InformationMessage(string.Format("[理论] 速:{0:F0} | 阻力:{1:F6} | 质量:{2:F1} | 仰角:{3:F1}° | 预测:无碰撞", new object[] { shootingSpeed, dynamicFriction, num, num2 }), Colors.Cyan));
				}
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00004ED0 File Offset: 0x000030D0
		public static void UpdateTrajectoryRangeWeapon(Agent agent)
		{
			bool flag = !ProjectileTrajectorySystem.IsHandheldEnabled || agent == null;
			if (!flag)
			{
				EquipmentIndex primaryWieldedItemIndex = agent.GetPrimaryWieldedItemIndex();
				bool flag2 = primaryWieldedItemIndex == -1;
				if (!flag2)
				{
					MissionWeapon missionWeapon = agent.Equipment[primaryWieldedItemIndex];
					bool flag3 = missionWeapon.IsEmpty || missionWeapon.CurrentUsageItem == null || !missionWeapon.CurrentUsageItem.IsRangedWeapon;
					if (!flag3)
					{
						float num = (float)missionWeapon.GetModifiedMissileSpeedForCurrentUsage();
						AgentDrivenProperties agentDrivenProperties = agent.AgentDrivenProperties;
						float num2 = ((agentDrivenProperties != null) ? agentDrivenProperties.MissileSpeedMultiplier : 1f);
						num *= num2;
						Vec3 eyeGlobalPosition = agent.GetEyeGlobalPosition();
						Vec3 vec = agent.LookDirection * num + agent.Velocity;
						float airFriction = ProjectileTrajectorySystem.GetAirFriction(missionWeapon.Item.PrimaryWeapon.WeaponClass, missionWeapon.Item.PrimaryWeapon.WeaponFlags);
						ProjectileTrajectorySystem.SimulateTrajectory(eyeGlobalPosition, vec, airFriction, 1f, 0f, delegate(Vec3 hitPos)
						{
							ProjectileTrajectorySystem.RenderSphere(hitPos, 0.05f, 4294901760U);
						}, false, false);
					}
				}
			}
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00004FF0 File Offset: 0x000031F0
		public static void UpdateTrajectory(Agent agent, RangedSiegeWeapon siegeWeapon)
		{
			bool flag = siegeWeapon == null || !siegeWeapon.GameEntity.IsValid;
			if (!flag)
			{
				string text = siegeWeapon.GetType().Name.ToLower();
				bool isLobber = text.Contains("mangonel") || text.Contains("trebuchet") || text.Contains("onager");
				bool flag2 = !isLobber;
				bool flag3 = siegeWeapon.GameEntity.Parent != null;
				bool flag4 = flag2 && !ProjectileTrajectorySystem.IsBallistaEnabled;
				if (!flag4)
				{
					bool flag5 = isLobber && !ProjectileTrajectorySystem.IsMangonelEnabled;
					if (!flag5)
					{
						float shootingSpeed = ProjectileTrajectorySystem.GetShootingSpeed(siegeWeapon);
						float dynamicFriction = ProjectileTrajectorySystem.GetDynamicFriction(siegeWeapon, isLobber);
						Vec3 parentVelocity = ProjectileTrajectorySystem.GetParentVelocity(siegeWeapon.GameEntity);
						float num = 1f;
						ItemObject ammoItem = ProjectileTrajectorySystem.GetAmmoItem(siegeWeapon);
						bool flag6 = ammoItem != null;
						if (flag6)
						{
							num = ammoItem.Weight;
						}
						bool flag7 = isLobber && num < 5f;
						if (flag7)
						{
							num = 40f;
						}
						Vec3 vec = Vec3.Invalid;
						Vec3 vec2 = Vec3.Zero;
						bool flag8 = flag3 && flag2 && agent != null && agent.IsMainAgent;
						float num2;
						if (flag8)
						{
							vec2 = agent.LookDirection;
							vec = agent.GetEyeGlobalPosition() + vec2 * 0.5f + Vec3.Up * 0.15f;
							num2 = 0.3f;
							bool isNavalAimEnabled = ProjectileTrajectorySystem.IsNavalAimEnabled;
							if (isNavalAimEnabled)
							{
								ProjectileTrajectorySystem.UpdateSoftAutoAim(agent, vec, shootingSpeed, dynamicFriction);
								vec2 = agent.LookDirection;
							}
						}
						else
						{
							vec2 = ProjectileTrajectorySystem.GetSiegeDirection(siegeWeapon);
							vec = ProjectileTrajectorySystem.GetRealMuzzlePosition(siegeWeapon);
							num2 = (flag2 ? 0.15f : 0.3f);
						}
						bool flag9 = vec == Vec3.Invalid;
						if (!flag9)
						{
							Vec3 vec3 = vec2 * shootingSpeed + parentVelocity;
							ProjectileTrajectorySystem.SimulateTrajectory(vec, vec3, dynamicFriction, num, num2, delegate(Vec3 hitPos)
							{
								bool isLobber2 = isLobber;
								if (isLobber2)
								{
									Vec3 vec4 = ProjectileTrajectorySystem.SampleSurfaceNormal(hitPos);
									ProjectileTrajectorySystem.DrawImprintedRing(hitPos, vec4, 3f, 4294919424U);
								}
								else
								{
									ProjectileTrajectorySystem.RenderSphere(hitPos, 0.5f, 4294901760U);
								}
							}, isLobber, isLobber);
						}
					}
				}
			}
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00005224 File Offset: 0x00003424
		private static void UpdateSoftAutoAim(Agent player, Vec3 muzzlePos, float missileSpeed, float friction)
		{
			bool flag = !Input.IsKeyDown(226);
			if (!flag)
			{
				Agent agent = ProjectileTrajectorySystem.FindClosestEnemy(player, muzzlePos, 800f);
				bool flag2 = agent != null;
				if (flag2)
				{
					float num = muzzlePos.Distance(agent.Position) / missileSpeed;
					Vec3 vec = agent.Position + Vec3.Up * 1.5f + agent.Velocity * num;
					Vec3 vec2 = vec - muzzlePos;
					float length = vec2.AsVec2.Length;
					float z = vec2.z;
					float num2 = 0f;
					float num3 = 1.0471976f;
					for (int i = 0; i < 15; i++)
					{
						float num4 = (num2 + num3) * 0.5f;
						float num5 = ProjectileTrajectorySystem.SimulateTrajectory(missileSpeed, friction, num4, z);
						bool flag3 = num5 > length;
						if (flag3)
						{
							num3 = num4;
						}
						else
						{
							num2 = num4;
						}
					}
					float num6 = (num2 + num3) * 0.5f;
					Vec3 vec3 = vec2.AsVec2.Normalized().ToVec3(0f);
					Vec3 vec4 = vec3 * MathF.Cos(num6) + Vec3.Up * MathF.Sin(num6);
					player.LookDirection = vec4;
				}
			}
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00005378 File Offset: 0x00003578
		private static float SimulateTrajectory(float speed, float friction, float angle, float targetHeightDiff)
		{
			float num = speed * MathF.Cos(angle);
			float num2 = speed * MathF.Sin(angle);
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0.05f;
			float num6 = 9.81f;
			for (int i = 0; i < 400; i++)
			{
				float num7 = 1f - friction * num5;
				num *= num7;
				num2 *= num7;
				num2 -= num6 * num5;
				num3 += num2 * num5;
				num4 += num * num5;
				bool flag = num3 <= targetHeightDiff;
				if (flag)
				{
					return num4;
				}
			}
			return 0f;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00005418 File Offset: 0x00003618
		private static Agent FindClosestEnemy(Agent player, Vec3 center, float range)
		{
			Agent agent = null;
			float num = range * range;
			Vec3 lookDirection = player.LookDirection;
			foreach (Agent agent2 in Mission.Current.Agents)
			{
				bool flag = agent2 == player || !agent2.IsActive();
				if (!flag)
				{
					bool flag2 = agent2.Team != player.Team && agent2.IsHuman;
					if (flag2)
					{
						float num2 = agent2.Position.DistanceSquared(center);
						bool flag3 = num2 > num;
						if (!flag3)
						{
							bool flag4 = Vec3.DotProduct(lookDirection, (agent2.Position - center).NormalizedCopy()) < 0.6f;
							if (!flag4)
							{
								num = num2;
								agent = agent2;
							}
						}
					}
				}
			}
			return agent;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00005514 File Offset: 0x00003714
		public static Vec3 GetRealMuzzlePosition(RangedSiegeWeapon weapon)
		{
			bool flag = weapon == null || !weapon.GameEntity.IsValid;
			Vec3 vec;
			if (flag)
			{
				vec = Vec3.Invalid;
			}
			else
			{
				try
				{
					PropertyInfo property = typeof(RangedSiegeWeapon).GetProperty("MissileStartingGlobalPositionForSimulation", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					bool flag2 = property != null;
					if (flag2)
					{
						object value = property.GetValue(weapon);
						Vec3 vec2;
						bool flag3;
						if (value is Vec3)
						{
							vec2 = (Vec3)value;
							flag3 = vec2.IsValid;
						}
						else
						{
							flag3 = false;
						}
						bool flag4 = flag3;
						if (flag4)
						{
							return vec2;
						}
					}
				}
				catch
				{
				}
				WeakGameEntity weakGameEntity = weapon.GameEntity.GetChildren().FirstOrDefault<WeakGameEntity>((WeakGameEntity x) => x.Name == "clean");
				bool flag5 = weakGameEntity != null;
				if (flag5)
				{
					WeakGameEntity weakGameEntity2 = weakGameEntity.GetChildren().FirstOrDefault<WeakGameEntity>((WeakGameEntity x) => x.Name == "projectile_leaving_position");
					bool flag6 = weakGameEntity2 != null;
					if (flag6)
					{
						return weakGameEntity2.GlobalPosition;
					}
				}
				Vec3 f = weapon.GameEntity.GetGlobalFrame().rotation.f;
				vec = weapon.GameEntity.GlobalPosition + Vec3.Up * 2f + f * 2.5f;
			}
			return vec;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x0000569C File Offset: 0x0000389C
		private static float GetDynamicFriction(RangedSiegeWeapon weapon, bool isLobber = false)
		{
			ItemObject ammoItem = ProjectileTrajectorySystem.GetAmmoItem(weapon);
			bool flag = ammoItem == null;
			float num;
			if (flag)
			{
				num = 5E-05f;
			}
			else
			{
				bool flag2 = ammoItem.PrimaryWeapon != null;
				if (flag2)
				{
					num = ProjectileTrajectorySystem.GetAirFriction(ammoItem.PrimaryWeapon.WeaponClass, ammoItem.PrimaryWeapon.WeaponFlags);
				}
				else
				{
					num = 5E-05f;
				}
			}
			return num;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x000056F8 File Offset: 0x000038F8
		private static ItemObject GetAmmoItem(RangedSiegeWeapon weapon)
		{
			try
			{
				PropertyInfo property = weapon.GetType().GetProperty("LoadedProjectileItem", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				bool flag = property != null;
				if (flag)
				{
					ItemObject itemObject = property.GetValue(weapon) as ItemObject;
					bool flag2 = itemObject != null;
					if (flag2)
					{
						return itemObject;
					}
				}
				PropertyInfo property2 = weapon.GetType().GetProperty("OriginalAmmoItem", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				bool flag3 = property2 != null;
				if (flag3)
				{
					ItemObject itemObject2 = property2.GetValue(weapon) as ItemObject;
					bool flag4 = itemObject2 != null;
					if (flag4)
					{
						return itemObject2;
					}
				}
				FieldInfo field = weapon.GetType().GetField("_originalAmmoItem", BindingFlags.Instance | BindingFlags.NonPublic);
				bool flag5 = field != null;
				if (flag5)
				{
					ItemObject itemObject3 = field.GetValue(weapon) as ItemObject;
					bool flag6 = itemObject3 != null;
					if (flag6)
					{
						return itemObject3;
					}
				}
			}
			catch
			{
			}
			return null;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000057EC File Offset: 0x000039EC
		private static Vec3 GetParentVelocity(WeakGameEntity weakEntity)
		{
			return Vec3.Zero;
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000057F4 File Offset: 0x000039F4
		private static float GetAirFriction(WeaponClass wc, WeaponFlags flags)
		{
			try
			{
				bool flag = ProjectileTrajectorySystem._gameAirFrictionMethod == null;
				if (flag)
				{
					ProjectileTrajectorySystem._gameAirFrictionMethod = typeof(ItemObject).GetMethod("GetAirFrictionConstant", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				bool flag2 = ProjectileTrajectorySystem._gameAirFrictionMethod != null;
				if (flag2)
				{
					ParameterInfo[] parameters = ProjectileTrajectorySystem._gameAirFrictionMethod.GetParameters();
					object obj = ((parameters != null && parameters.Length == 2) ? ProjectileTrajectorySystem._gameAirFrictionMethod.Invoke(null, new object[] { wc, flags }) : ProjectileTrajectorySystem._gameAirFrictionMethod.Invoke(null, new object[] { wc }));
					float num;
					bool flag3;
					if (obj is float)
					{
						num = (float)obj;
						flag3 = true;
					}
					else
					{
						flag3 = false;
					}
					bool flag4 = flag3;
					if (flag4)
					{
						return num;
					}
				}
			}
			catch
			{
			}
			float num2;
			if (wc != 16)
			{
				if (wc != 17)
				{
					if (wc != 23)
					{
						num2 = 0.002f;
					}
					else
					{
						num2 = 0.01f;
					}
				}
				else
				{
					num2 = 0.005f;
				}
			}
			else
			{
				num2 = 0.003f;
			}
			return num2;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00005910 File Offset: 0x00003B10
		private static float GetShootingSpeed(RangedSiegeWeapon w)
		{
			try
			{
				PropertyInfo property = w.GetType().GetProperty("ShootingSpeed", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				bool flag = property != null;
				if (flag)
				{
					object value = property.GetValue(w);
					float num;
					bool flag2;
					if (value is float)
					{
						num = (float)value;
						flag2 = true;
					}
					else
					{
						flag2 = false;
					}
					bool flag3 = flag2;
					if (flag3)
					{
						return num;
					}
				}
			}
			catch
			{
			}
			return 150f;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00005988 File Offset: 0x00003B88
		private static Vec3 GetSiegeDirection(RangedSiegeWeapon w)
		{
			try
			{
				PropertyInfo property = w.GetType().GetProperty("ShootingDirection", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				bool flag = property != null;
				if (flag)
				{
					object value = property.GetValue(w);
					Vec3 vec;
					bool flag2;
					if (value is Vec3)
					{
						vec = (Vec3)value;
						flag2 = true;
					}
					else
					{
						flag2 = false;
					}
					bool flag3 = flag2;
					if (flag3)
					{
						return vec;
					}
				}
			}
			catch
			{
			}
			return w.GameEntity.GetGlobalFrame().rotation.f;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00005A14 File Offset: 0x00003C14
		private static void DrawImprintedRing(Vec3 center, Vec3 normal, float radius, uint color)
		{
			Mat3 mat = ProjectileTrajectorySystem.CreateRotationFromUp(normal);
			MatrixFrame matrixFrame = new MatrixFrame(ref mat, ref center);
			matrixFrame.origin += normal * 0.05f;
			int num = 24;
			float num2 = 6.2831855f / (float)num;
			float num3 = MBCommon.GetApplicationTime();
			float num4 = 1f + MathF.Sin(num3 * 5f) * 0.05f;
			float num5 = radius * num4;
			Vec3 vec = new Vec3(num5, 0f, 0f, -1f);
			Vec3 vec2 = matrixFrame.TransformToParent(ref vec);
			for (int i = 1; i <= num; i++)
			{
				float num6 = (float)i * num2;
				Vec3 vec3;
				vec3..ctor(MathF.Cos(num6) * num5, MathF.Sin(num6) * num5, 0f, -1f);
				Vec3 vec4 = matrixFrame.TransformToParent(ref vec3);
				ProjectileTrajectorySystem.RenderOneLine(vec2, vec4, color);
				vec2 = vec4;
			}
			float num7 = 0.3f * num4;
			vec = new Vec3(-num7, 0f, 0f, -1f);
			Vec3 vec5 = matrixFrame.TransformToParent(ref vec);
			Vec3 vec6 = new Vec3(num7, 0f, 0f, -1f);
			ProjectileTrajectorySystem.RenderOneLine(vec5, matrixFrame.TransformToParent(ref vec6), color);
			vec = new Vec3(0f, -num7, 0f, -1f);
			Vec3 vec7 = matrixFrame.TransformToParent(ref vec);
			vec6 = new Vec3(0f, num7, 0f, -1f);
			ProjectileTrajectorySystem.RenderOneLine(vec7, matrixFrame.TransformToParent(ref vec6), color);
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00005BAC File Offset: 0x00003DAC
		private static Mat3 CreateRotationFromUp(Vec3 up)
		{
			Vec3 vec = up;
			Vec3 vec2 = ((MathF.Abs(vec.z) < 0.99f) ? new Vec3(0f, 0f, 1f, -1f) : new Vec3(1f, 0f, 0f, -1f));
			Vec3 vec3 = Vec3.CrossProduct(vec2, vec);
			vec3.Normalize();
			Vec3 vec4 = Vec3.CrossProduct(vec, vec3);
			vec4.Normalize();
			return new Mat3(ref vec3, ref vec4, ref vec);
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00005C34 File Offset: 0x00003E34
		private static Vec3 SampleSurfaceNormal(Vec3 c)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			Mission.Current.Scene.GetHeightAtPoint(c.AsVec2, 0, ref num);
			Mission.Current.Scene.GetHeightAtPoint(c.AsVec2 + new Vec2(0.5f, 0f), 0, ref num2);
			Mission.Current.Scene.GetHeightAtPoint(c.AsVec2 + new Vec2(0f, 0.5f), 0, ref num3);
			bool flag = MathF.Abs(num - c.z) > 1.5f;
			Vec3 vec;
			if (flag)
			{
				vec = Vec3.Up;
			}
			else
			{
				vec = Vec3.CrossProduct(new Vec3(0.5f, 0f, num2 - num, -1f), new Vec3(0f, 0.5f, num3 - num, -1f)).NormalizedCopy();
			}
			return vec;
		}

		// Token: 0x04000043 RID: 67
		private static MethodInfo _gameAirFrictionMethod;

		// Token: 0x04000044 RID: 68
		private const uint Color_FlightLine = 4294967295U;

		// Token: 0x04000045 RID: 69
		private const uint Color_Impact = 4294901760U;

		// Token: 0x04000046 RID: 70
		private const float Sim_MaxTime = 20f;

		// Token: 0x04000047 RID: 71
		private const float Sim_Step = 0.02f;

		// Token: 0x04000048 RID: 72
		private static MethodInfo _renderLineMethod;

		// Token: 0x04000049 RID: 73
		private static object _debugInterface;

		// Token: 0x0400004A RID: 74
		private static MethodInfo _renderSphereMethod;
	}
}
