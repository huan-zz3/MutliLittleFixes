using System;
using System.Linq;
using System.Reflection;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace MutliLittleFixes
{
	public static class ProjectileTrajectorySystem
	{
		private static bool IsBallistaEnabled
		{
			get
			{
				SiegeTrajectoryConfig instance = GlobalSettings<SiegeTrajectoryConfig>.Instance;
				return instance == null || instance.EnableBallista;
			}
		}

		private static bool IsMangonelEnabled
		{
			get
			{
				SiegeTrajectoryConfig instance = GlobalSettings<SiegeTrajectoryConfig>.Instance;
				return instance == null || instance.EnableMangonel;
			}
		}


		private static void SimulateTrajectory(Vec3 start, Vec3 velocity, float friction, float mass, float ignoreTime, Action<Vec3> onHit, bool useQuadraticDrag)
		{
			velocity *= 0.9f;
			Vec3 vec = start;
			Vec3 vec2;
			vec2 = new Vec3(0f, 0f, -9.806f, -1f);
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
					bool flag3 = Mission.Current.Scene.RayCastForClosestEntityOrTerrain(vec, vec3, out float num6, out Vec3 vec4, out WeakGameEntity weakGameEntity, 0.01f, BodyFlags.CommonFocusRayCastExcludeFlags);
					if (flag3)
					{
						onHit(vec4);
						break;
					}
				}
				vec = vec3;
				bool flag5 = vec.z < -100f;
				if (flag5)
				{
					break;
				}
			}
		}


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
				}, false);
				bool gotHit2 = gotHit;
				if (gotHit2)
				{
					InformationManager.DisplayMessage(new InformationMessage(string.Format("[����] ��:{0:F0} | ����:{1:F6} | ����:{2:F1} | ����:{3:F1}�� | Ԥ��Z:{4:F1}", new object[] { shootingSpeed, dynamicFriction, num, num2, predictedHit.z }), Colors.Cyan));
				}
				else
				{
					InformationManager.DisplayMessage(new InformationMessage(string.Format("[����] ��:{0:F0} | ����:{1:F6} | ����:{2:F1} | ����:{3:F1}�� | Ԥ��:����ײ", new object[] { shootingSpeed, dynamicFriction, num, num2 }), Colors.Cyan));
				}
			}
		}


		/// <summary>
		/// 轨迹模拟的命中结果。
		/// </summary>
		public struct TrajectoryHitResult
		{
			/// <summary>是否命中（模拟到了碰撞）</summary>
			public bool HasHit;

			/// <summary>命中位置（世界坐标）</summary>
			public Vec3 HitPosition;

			/// <summary>命中点的地形法线（仅对 lobber 有效）</summary>
			public Vec3 SurfaceNormal;

			/// <summary>射弹类型（true = 抛射类, false = 直射类）</summary>
			public bool IsLobber;

			/// <summary>是否为弩炮类</summary>
			public bool IsBallista => !IsLobber;
		}

		/// <summary>
		/// 模拟轨迹并返回命中结果（不执行任何渲染）。
		/// </summary>
		public static TrajectoryHitResult UpdateTrajectory(Agent agent, RangedSiegeWeapon siegeWeapon)
		{
			TrajectoryHitResult result = default;
			result.HasHit = false;

			bool flag = siegeWeapon == null || !siegeWeapon.GameEntity.IsValid;
			if (!flag)
			{
				string text = siegeWeapon.GetType().Name.ToLower();
				bool isLobber = text.Contains("mangonel") || text.Contains("trebuchet") || text.Contains("onager");
				result.IsLobber = isLobber;

				bool flag2 = !isLobber;
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
						Vec3 vec2 = ProjectileTrajectorySystem.GetSiegeDirection(siegeWeapon);
						Vec3 vec = ProjectileTrajectorySystem.GetRealMuzzlePosition(siegeWeapon);
						float num2 = (flag2 ? 0.15f : 0.3f);
						bool flag9 = vec == Vec3.Invalid;
						if (!flag9)
						{
							Vec3 vec3 = vec2 * shootingSpeed + parentVelocity;
							ProjectileTrajectorySystem.SimulateTrajectory(vec, vec3, dynamicFriction, num, num2, delegate(Vec3 hitPos)
							{
								result.HasHit = true;
								result.HitPosition = hitPos;
								if (isLobber)
								{
									result.SurfaceNormal = ProjectileTrajectorySystem.SampleSurfaceNormal(hitPos);
								}
							}, isLobber);
						}
					}
				}
			}

			return result;
		}


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
			Vec3 vec2 = Vec3.Invalid;
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

		private static Vec3 GetParentVelocity(WeakGameEntity weakEntity)
		{
			return Vec3.Zero;
		}

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
					float num = 0f;
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
			if ((int)wc != 16)
			{
				if ((int)wc != 17)
				{
					if ((int)wc != 23)
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

		private static float GetShootingSpeed(RangedSiegeWeapon w)
		{
			try
			{
				PropertyInfo property = w.GetType().GetProperty("ShootingSpeed", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				bool flag = property != null;
				if (flag)
				{
					object value = property.GetValue(w);
					float num = 150f;
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

		private static Vec3 GetSiegeDirection(RangedSiegeWeapon w)
		{
			try
			{
				PropertyInfo property = w.GetType().GetProperty("ShootingDirection", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				bool flag = property != null;
				if (flag)
				{
					object value = property.GetValue(w);
					Vec3 vec = Vec3.Invalid;
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

		private static MethodInfo _gameAirFrictionMethod;

		private const float Sim_MaxTime = 20f;

		private const float Sim_Step = 0.02f;
	}
}
