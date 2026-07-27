using System;
using System.Collections.Generic;
using NavalDLC.Missions.MissionLogics;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.Objects
{
	// Token: 0x0200009D RID: 157
	public class MissionShipRam : MissionObject
	{
		// Token: 0x17000230 RID: 560
		// (get) Token: 0x06000BE5 RID: 3045 RVA: 0x00054A02 File Offset: 0x00052C02
		private static float ForwardSpeedThresholdToDamage
		{
			get
			{
				return MissionShipRam._ramQualityThresholds[MissionShipRam._ramQualityThresholds.Length - 1].Item1;
			}
		}

		// Token: 0x17000231 RID: 561
		// (get) Token: 0x06000BE6 RID: 3046 RVA: 0x00054A1C File Offset: 0x00052C1C
		private static float DistanceToShipCenterThresholdToDamage
		{
			get
			{
				return MissionShipRam._ramQualityThresholds[MissionShipRam._ramQualityThresholds.Length - 1].Item3;
			}
		}

		// Token: 0x17000232 RID: 562
		// (get) Token: 0x06000BE7 RID: 3047 RVA: 0x00054A36 File Offset: 0x00052C36
		public float RamLength
		{
			get
			{
				return this._ramLength;
			}
		}

		// Token: 0x06000BE8 RID: 3048 RVA: 0x00054A40 File Offset: 0x00052C40
		private CapsuleData GetRamCapsuleData(float fixedDt, bool getDataForNextFrame)
		{
			MatrixFrame matrixFrame = base.GameEntity.ComputePreciseGlobalFrameForFixedTickSlow();
			Vec3 f = matrixFrame.rotation.f;
			Vec3 vec = matrixFrame.TransformToParent(ref this._ramAttachmentPointOffset);
			Vec3 vec2 = vec + f * this._ramLength;
			float num = this._ramRadius * Math.Max(matrixFrame.rotation.u.Length, matrixFrame.rotation.s.Length);
			if (getDataForNextFrame)
			{
				vec += GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(this._ownerShip.GameEntity, vec) * fixedDt;
				vec2 += GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(this._ownerShip.GameEntity, vec2) * fixedDt;
			}
			return new CapsuleData(num, vec, vec2);
		}

		// Token: 0x06000BE9 RID: 3049 RVA: 0x00054B00 File Offset: 0x00052D00
		protected override void OnTick(float dt)
		{
			if (this._ramDamageData.IsValid)
			{
				float calculatedDamage = this._ramDamageData.CalculatedDamage;
				if (calculatedDamage > 0f)
				{
					this._ramDamageData.TargetShip.DealCollisionDamage(this._ownerShip, true, this._ramDamageData.SelectedIntersectionPoint, calculatedDamage);
					this._ownerShip.UpdateDamageCooldown(this._ramDamageData.TargetShip);
					Vec3 averageIntersectionPoint = this._ramDamageData.AverageIntersectionPoint;
					foreach (DestructableComponent destructableComponent in this._ramDamageData.TargetShip.AllDestructableComponents)
					{
						if (!destructableComponent.IsDestroyed && destructableComponent.GameEntity.GlobalPosition.DistanceSquared(averageIntersectionPoint) < 25f)
						{
							destructableComponent.DestroyOnAnyHit = true;
							destructableComponent.TriggerOnHit(null, 1, averageIntersectionPoint, this._ramDamageData.RamDirection, ref MissionWeapon.Invalid, -1, this);
						}
					}
					Agent agent = this._ownerShip.Captain;
					if (agent == null || !agent.IsMainAgent)
					{
						Agent agent2 = null;
						if (this._ownerShip.ShipControllerMachine.PilotAgent != null && (agent2 == null || this._ownerShip.ShipControllerMachine.PilotAgent.IsMainAgent))
						{
							agent2 = this._ownerShip.ShipControllerMachine.PilotAgent;
						}
						if (agent == null || (agent2 != null && agent2.IsMainAgent))
						{
							agent = agent2;
						}
					}
					for (int i = Mission.Current.Agents.Count - 1; i >= 0; i--)
					{
						Agent agent3 = Mission.Current.Agents[i];
						Vec3 position = agent3.Position;
						if (agent3.IsActive() && position.AsVec2.DistanceSquared(averageIntersectionPoint.AsVec2) < 4f)
						{
							Blow blow;
							blow..ctor((agent != null) ? agent.Index : agent3.Index);
							blow.DamageType = 2;
							blow.BaseMagnitude = 200f;
							blow.InflictedDamage = 200;
							blow.GlobalPosition = position;
							blow.DamagedPercentage = 1f;
							agent3.Die(blow, -1);
						}
					}
				}
				this.TriggerRamCollisionParticleAndSoundEffect(this._ramDamageData.TargetShip.Index, this._ramDamageData.TargetShip.GameEntity, this._ramDamageData.CapsuleData, calculatedDamage);
				this._ramDamageData = default(MissionShipRam.RamCollisionData);
			}
		}

		// Token: 0x06000BEA RID: 3050 RVA: 0x00054D7C File Offset: 0x00052F7C
		protected override void OnInit()
		{
			base.OnInit();
			this._ownerShip = base.GameEntity.Root.GetFirstScriptOfTypeInFamily<MissionShip>();
			CapsuleData ramCapsuleData = this.GetRamCapsuleData(0f, false);
			this._scaledRamRadius = ramCapsuleData.Radius;
			Vec3 vec = ramCapsuleData.P2 - ramCapsuleData.P1;
			this._scaledRamLength = vec.Length + this._scaledRamRadius;
			MatrixFrame globalFrame = this._ownerShip.GameEntity.GetGlobalFrame();
			WeakGameEntity gameEntity = this._ownerShip.GameEntity;
			vec = ramCapsuleData.P1;
			Vec3 vec2 = globalFrame.TransformToLocal(ref vec);
			Vec3 p = ramCapsuleData.P2;
			GameEntityPhysicsExtensions.PushCapsuleShapeToEntityBody(gameEntity, vec2, globalFrame.TransformToLocal(ref p), ramCapsuleData.Radius, "wood_ship");
			this._ownScene = base.GameEntity.Scene;
		}

		// Token: 0x06000BEB RID: 3051 RVA: 0x00054E54 File Offset: 0x00053054
		protected override void OnFixedTick(float fixedDt)
		{
			this.RamCollisionHandleFixedTick(fixedDt);
		}

		// Token: 0x06000BEC RID: 3052 RVA: 0x00054E5D File Offset: 0x0005305D
		protected override void OnParallelFixedTick(float fixedDt)
		{
			this.RamCollisionCheckTick(fixedDt);
		}

		// Token: 0x06000BED RID: 3053 RVA: 0x00054E66 File Offset: 0x00053066
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 50;
		}

		// Token: 0x06000BEE RID: 3054 RVA: 0x00054E6C File Offset: 0x0005306C
		private void TriggerRamCollisionParticleAndSoundEffect(int targetShipIndex, WeakGameEntity targetEntity, CapsuleData shipRamCapsule, float damage)
		{
			List<WeakGameEntity> list = targetEntity.CollectChildrenEntitiesWithTag("body_mesh");
			if (list.Count != 0)
			{
				WeakGameEntity weakGameEntity = list[0];
				Vec3 vec = shipRamCapsule.P2 - shipRamCapsule.P1;
				vec.Normalize();
				Vec3 p = shipRamCapsule.P1;
				float num = this._scaledRamLength * 3f;
				Vec3 vec2 = p;
				float num2 = num;
				Vec3 zero = Vec3.Zero;
				if (weakGameEntity.RayHitEntityWithNormal(p, vec, num, ref zero, ref num2))
				{
					MatrixFrame identity = MatrixFrame.Identity;
					identity.origin = p + vec * num2;
					identity.rotation.u = zero;
					identity.rotation.f = Vec3.Up;
					identity.rotation.s = Vec3.CrossProduct(identity.rotation.f, identity.rotation.u);
					identity.rotation.s.Normalize();
					identity.rotation.f = Vec3.CrossProduct(identity.rotation.u, identity.rotation.s);
					GameEntity gameEntity = GameEntity.Instantiate(Mission.Current.Scene, "decal_ship_damaged_b_heap", identity, true);
					targetEntity.AddChild(gameEntity.WeakEntity, true);
					vec2 = identity.origin;
					Color color = Colors.White;
					ColorAssigner firstScriptOfType = base.GameEntity.Root.GetFirstScriptOfType<ColorAssigner>();
					if (firstScriptOfType != null)
					{
						color = firstScriptOfType.RamDebrisColor;
					}
					using (IEnumerator<GameEntity> enumerator = gameEntity.GetChildren().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							GameEntity gameEntity2 = enumerator.Current;
							if (gameEntity2.HasTag("plank"))
							{
								MatrixFrame globalFrame = gameEntity2.GetGlobalFrame();
								Vec3 vec3 = globalFrame.origin + zero * 2f;
								float num3 = 0f;
								Vec3 vec4 = globalFrame.origin;
								bool flag = weakGameEntity.RayHitEntity(vec3, -zero, 2.5f, ref num3);
								if (flag)
								{
									vec4 = vec3 - zero * num3;
									Vec3 boundingBoxMax = gameEntity2.GetBoundingBoxMax();
									Vec3 boundingBoxMin = gameEntity2.GetBoundingBoxMin();
									vec4 + boundingBoxMax.z * globalFrame.rotation.u + boundingBoxMax.x * globalFrame.rotation.s;
									Vec3 vec5 = vec4 + zero;
									flag = weakGameEntity.RayHitEntity(vec5, -zero, 1.5f, ref num3);
									if (flag)
									{
										vec4 + boundingBoxMin.z * globalFrame.rotation.u + boundingBoxMax.x * globalFrame.rotation.s;
										Vec3 vec6 = vec4 + zero;
										flag = weakGameEntity.RayHitEntity(vec6, -zero, 1.5f, ref num3);
									}
									if (flag)
									{
										vec4 + boundingBoxMin.z * globalFrame.rotation.u + boundingBoxMin.x * globalFrame.rotation.s;
										Vec3 vec7 = vec4 + zero;
										flag = weakGameEntity.RayHitEntity(vec7, -zero, 1.5f, ref num3);
									}
									if (flag)
									{
										vec4 + boundingBoxMax.z * globalFrame.rotation.u + boundingBoxMin.x * globalFrame.rotation.s;
										Vec3 vec8 = vec4 + zero;
										flag = weakGameEntity.RayHitEntity(vec8, -zero, 1.5f, ref num3);
									}
								}
								if (flag)
								{
									globalFrame.origin = vec4;
									gameEntity2.SetGlobalFrame(ref globalFrame, true);
									gameEntity2.SetFactorColor(color.ToUnsignedInteger());
								}
								else
								{
									gameEntity2.SetVisibilityExcludeParents(false);
								}
							}
							else if (gameEntity2.HasTag("decal"))
							{
								gameEntity2.SetFactorColor(color.ToUnsignedInteger());
							}
						}
						goto IL_0423;
					}
				}
				MBDebug.Print("Could not hit body\n", 0, 12, 17592186044416UL);
				IL_0423:
				SoundEventParameter soundEventParameter;
				soundEventParameter..ctor("Force", MathF.Min(damage * 0.01f, 1f));
				MBSoundEvent.PlaySound(MissionShipRam.RamCollisionSoundEffectSoundId, ref soundEventParameter, vec2);
			}
		}

		// Token: 0x06000BEF RID: 3055 RVA: 0x000552E8 File Offset: 0x000534E8
		private void RamCollisionHandleFixedTick(float fixedDt)
		{
			MatrixFrame bodyWorldTransform = this._ownerShip.GameEntity.GetBodyWorldTransform();
			Vec3 vec = bodyWorldTransform.rotation.f.NormalizedCopy();
			MissionShip targetShip = this._ramCollisionData.TargetShip;
			CapsuleData capsuleData = this._ramCollisionData.CapsuleData;
			bool flag = this._ramCollisionData.RamWillBeHandled;
			if (this._ramCollisionData.HasPoint)
			{
				Vec3 averageIntersectionPoint = this._ramCollisionData.AverageIntersectionPoint;
				WeakGameEntity gameEntity = targetShip.GameEntity;
				MatrixFrame bodyWorldTransform2 = gameEntity.GetBodyWorldTransform();
				Vec3 vec2 = bodyWorldTransform2.rotation.f.NormalizedCopy();
				if (this._ramStuckTargetShip == null && this._lastRamHitQuality > 0 && MissionShipRam._ramQualityThresholds[MissionShipRam._ramQualityThresholds.Length - this._lastRamHitQuality].Item5 && this._ramCollisionData.PenetrationLength >= this._scaledRamLength * 0.33f && targetShip.HitPoints > 0f)
				{
					this._ramStuckTargetShip = targetShip;
				}
				if (this._ramStuckTargetShip != null)
				{
					if (this._ramStuckTargetShip.HitPoints <= 0f)
					{
						this._ramStuckTargetShip = null;
					}
					flag = true;
				}
				Vec3 pointVelocityOnOwner = this._ramCollisionData.PointVelocityOnOwner;
				Vec3 pointVelocityOnTarget = this._ramCollisionData.PointVelocityOnTarget;
				Vec3 vec3 = pointVelocityOnOwner - pointVelocityOnTarget;
				float num = vec3.Normalize();
				bool flag2 = true;
				float num2 = num * 0.03f;
				if (this._ramStuckTargetShip == null)
				{
					flag = true;
					float num3 = 1f / this._ownerShip.GameEntity.Mass + 1f / targetShip.GameEntity.Mass;
					Vec3 vec4 = -vec3 * num2 / num3;
					Vec3 vec5 = vec3 * num2 / num3;
					float num4 = Vec3.DotProduct(vec5, -vec);
					if (num4 > 0f)
					{
						Vec3 vec6 = -vec * num4;
						vec5 -= vec6;
					}
					GameEntityPhysicsExtensions.ApplyGlobalForceAtLocalPosToDynamicBody(gameEntity, bodyWorldTransform2.TransformToLocal(ref averageIntersectionPoint), vec5, 1);
					GameEntityPhysicsExtensions.ApplyGlobalForceAtLocalPosToDynamicBody(this._ownerShip.GameEntity, bodyWorldTransform.TransformToLocal(ref averageIntersectionPoint), vec4, 1);
					float num5 = MathF.Abs(Vec3.DotProduct(pointVelocityOnOwner - pointVelocityOnTarget, vec));
					BoundingBox localPhysicsBoundingBox = GameEntityPhysicsExtensions.GetLocalPhysicsBoundingBox(gameEntity, false);
					Vec3 vec7 = localPhysicsBoundingBox.center;
					vec7 = bodyWorldTransform2.TransformToParent(ref vec7);
					vec7.z = this._ramCollisionData.SelectedIntersectionPoint.z;
					float num6 = MathF.Abs(Vec3.DotProduct(this._ramCollisionData.SelectedIntersectionPoint - vec7, vec2));
					float num7 = localPhysicsBoundingBox.max.y - localPhysicsBoundingBox.min.y;
					int num8 = 1;
					float num9 = MathF.Acos(MathF.Abs(Vec3.DotProduct(vec, vec2))) * 57.295776f;
					float num10 = MissionShipRam._ramQualityThresholds[MissionShipRam._ramQualityThresholds.Length - 1].Item4;
					for (int i = 0; i < MissionShipRam._ramQualityThresholds.Length; i++)
					{
						ValueTuple<float, float, float, float, bool> valueTuple = MissionShipRam._ramQualityThresholds[i];
						if (num5 >= valueTuple.Item1 && num9 >= valueTuple.Item2 && num6 * 2f <= num7 * valueTuple.Item3)
						{
							if (valueTuple.Item5)
							{
								flag2 = true;
							}
							num10 = valueTuple.Item4;
							num8 = MissionShipRam._ramQualityThresholds.Length - i;
							break;
						}
					}
					float num11 = 12f * (float)Math.Sqrt((double)(this._ownerShip.Physics.Mass / 500f)) * this._ramTierDamageMultiplier * num10 * num5;
					bool flag3 = !this._ramCollisionBeingHandled && flag;
					if (flag3 && this._ownerShip.CanDealDamage(targetShip))
					{
						this._lastRamHitQuality = num8;
						if (!this._ramDamageData.IsValid)
						{
							this._ramDamageData = new MissionShipRam.RamCollisionData
							{
								TargetShip = this._ramCollisionData.TargetShip,
								CapsuleData = this._ramCollisionData.CapsuleData,
								RamWillBeHandled = this._ramCollisionData.RamWillBeHandled,
								SelectedIntersectionPoint = this._ramCollisionData.SelectedIntersectionPoint,
								AverageIntersectionPoint = this._ramCollisionData.AverageIntersectionPoint,
								RamDirection = this._ramCollisionData.RamDirection,
								PenetrationLength = this._ramCollisionData.PenetrationLength,
								HasPoint = this._ramCollisionData.HasPoint,
								CalculatedDamage = num11
							};
						}
					}
					this._ownerShip.ShipsLogic.OnShipRamming(this._ownerShip, targetShip, num11 / targetShip.HitPoints, flag3, capsuleData, num8);
				}
				if (flag && this._ramStuckTargetShip != null)
				{
					if (1f - Math.Abs(Vec3.DotProduct(vec, vec2)) < 0.3f)
					{
						this._ramStuckTargetShip = null;
					}
					else if (flag2)
					{
						Vec3 vec8 = pointVelocityOnOwner - pointVelocityOnTarget;
						float num12 = 1f / this._ownerShip.GameEntity.Mass + 1f / targetShip.GameEntity.Mass;
						Vec3 vec9 = -0.1f * vec8 / num12;
						Vec3 vec10 = 0.1f * vec8 / num12;
						float num13 = Vec3.DotProduct(vec10, -vec);
						if (num13 > 0f)
						{
							Vec3 vec11 = -vec.NormalizedCopy() * num13;
							vec10 -= vec11;
						}
						GameEntityPhysicsExtensions.ApplyGlobalForceAtLocalPosToDynamicBody(this._ownerShip.GameEntity, bodyWorldTransform.TransformToLocal(ref averageIntersectionPoint), vec9, 1);
						GameEntityPhysicsExtensions.ApplyGlobalForceAtLocalPosToDynamicBody(targetShip.GameEntity, bodyWorldTransform2.TransformToLocal(ref averageIntersectionPoint), vec10, 1);
					}
				}
			}
			else if (this._ramStuckTargetShip != null)
			{
				this._ramStuckTargetShip = null;
			}
			if (this._ramCollisionBeingHandled != flag)
			{
				if (flag)
				{
					GameEntityPhysicsExtensions.PopCapsuleShapeFromEntityBody(this._ownerShip.GameEntity);
				}
				else
				{
					this._lastRamHitQuality = 0;
					WeakGameEntity gameEntity2 = this._ownerShip.GameEntity;
					Vec3 p = capsuleData.P1;
					Vec3 vec12 = bodyWorldTransform.TransformToLocal(ref p);
					Vec3 p2 = capsuleData.P2;
					GameEntityPhysicsExtensions.PushCapsuleShapeToEntityBody(gameEntity2, vec12, bodyWorldTransform.TransformToLocal(ref p2), capsuleData.Radius, "wood_ship");
					ShipCollisionOutcomeLogic missionBehavior = Mission.Current.GetMissionBehavior<ShipCollisionOutcomeLogic>();
					if (missionBehavior != null)
					{
						missionBehavior.ActivateCooldownForShip(this._ownerShip, 0.2f);
					}
				}
				this._ramCollisionBeingHandled = flag;
			}
		}

		// Token: 0x06000BF0 RID: 3056 RVA: 0x00055924 File Offset: 0x00053B24
		private void RamCollisionCheckTick(float fixedDt)
		{
			bool flag = false;
			int num = -1;
			WeakGameEntity root = base.GameEntity.Root;
			Vec3 vec = this._ownerShip.GameEntity.GetBodyWorldTransform().rotation.f.NormalizedCopy();
			CapsuleData ramCapsuleData = this.GetRamCapsuleData(fixedDt, !this._ramCollisionBeingHandled);
			Vec3 vec2 = ramCapsuleData.P2 - ramCapsuleData.P1;
			Vec3 invalid = Vec3.Invalid;
			WeakGameEntity invalid2 = WeakGameEntity.Invalid;
			float num2 = -1f;
			BodyFlags bodyFlag = this._ownerShip.GameEntity.BodyFlag;
			Scene ownScene = this._ownScene;
			Vec3 vec3 = ramCapsuleData.P1;
			Vec3 vec4 = ramCapsuleData.P1 + vec2 * 2f;
			if (ownScene.RayCastForRamming(ref vec3, ref vec4, this._ownerShip.GameEntity, this._scaledRamRadius, ref num2, ref invalid, ref invalid2, -2147469567, bodyFlag))
			{
				float num3 = -1f;
				MissionShip missionShip = invalid2.GetFirstScriptWithNameHash(MissionShip.MissionShipScriptNameHash) as MissionShip;
				if (missionShip != null)
				{
					float num4 = 0f;
					int num5 = 0;
					int num6 = this._ownScene.GenerateContactsWithCapsule(ref ramCapsuleData, 896, true, this._intersectionsCache, this._entitiesCache, this._entityPointersCache);
					for (int i = 0; i < num6; i++)
					{
						WeakGameEntity weakGameEntity = this._entitiesCache[i];
						if (!(weakGameEntity == null))
						{
							WeakGameEntity root2 = weakGameEntity.Root;
							if (!(root == root2))
							{
								MissionShip firstScriptOfType = root2.GetFirstScriptOfType<MissionShip>();
								if (firstScriptOfType != null && firstScriptOfType != this._ownerShip && firstScriptOfType == missionShip)
								{
									if (this._ramCollisionBeingHandled && Extensions.HasAnyFlag<BodyFlags>(weakGameEntity.BodyFlag, 32))
									{
										flag = true;
									}
									else
									{
										this._selectedIntersectionsCache[num5] = this._intersectionsCache[i];
										num5++;
										num4 += Vec3.DotProduct(this._intersectionsCache[i].IntersectionPoint - ramCapsuleData.P1, vec);
									}
								}
							}
						}
					}
					if (num5 > 0)
					{
						num3 = num4 / (float)num5;
					}
					float num7 = float.MaxValue;
					for (int j = 0; j < num5; j++)
					{
						if (missionShip != null && missionShip != this._ownerShip)
						{
							float num8 = Math.Abs(this._selectedIntersectionsCache[j].IntersectionPoint.DistanceSquared(ramCapsuleData.P1) - num3 * num3);
							if (num8 < num7)
							{
								num7 = num8;
								num = j;
							}
						}
					}
				}
				int num9 = -1;
				Vec3 vec5 = Vec3.Invalid;
				Vec3 vec6 = Vec3.Invalid;
				Vec3 vec7 = Vec3.Invalid;
				Vec3 vec8 = Vec3.Invalid;
				if (num >= 0)
				{
					vec5 = ramCapsuleData.P1 + vec * num3;
					vec7 = GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(this._ownerShip.GameEntity, vec5);
					vec8 = GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(missionShip.GameEntity, vec5);
					vec3 = Vec3.DotProduct(vec7 - vec8, vec) * vec;
					if (vec3.Length > MissionShipRam.ForwardSpeedThresholdToDamage)
					{
						Vec3 intersectionPoint = this._selectedIntersectionsCache[num].IntersectionPoint;
						WeakGameEntity gameEntity = missionShip.GameEntity;
						MatrixFrame bodyWorldTransform = gameEntity.GetBodyWorldTransform();
						Vec3 vec9 = bodyWorldTransform.rotation.f.NormalizedCopy();
						BoundingBox localPhysicsBoundingBox = GameEntityPhysicsExtensions.GetLocalPhysicsBoundingBox(gameEntity, false);
						Vec3 vec10 = localPhysicsBoundingBox.center;
						vec10 = bodyWorldTransform.TransformToParent(ref vec10);
						vec10.z = intersectionPoint.z;
						float num10 = localPhysicsBoundingBox.max.y - localPhysicsBoundingBox.min.y;
						if (MathF.Abs(Vec3.DotProduct(intersectionPoint - vec10, vec9)) * 2f <= num10 * MissionShipRam.DistanceToShipCenterThresholdToDamage)
						{
							vec6 = intersectionPoint;
							num9 = num;
						}
					}
				}
				if (missionShip != null && !flag && !this._ramCollisionBeingHandled)
				{
					Vec2 asVec = vec.AsVec2;
					vec3 = this._ownerShip.Physics.LinearVelocity;
					float num11 = asVec.DotProduct(vec3.AsVec2);
					this._ownerShip.ShipsLogic.OnShipAboutToBeRammed(this._ownerShip, missionShip, num2, num11);
				}
				this._ramCollisionData = new MissionShipRam.RamCollisionData
				{
					TargetShip = missionShip,
					CapsuleData = ramCapsuleData,
					RamWillBeHandled = flag,
					SelectedIntersectionPoint = vec6,
					AverageIntersectionPoint = vec5,
					RamDirection = vec,
					PenetrationLength = MathF.Max(0f, this._scaledRamLength - num2),
					HasPoint = (num9 >= 0),
					PointVelocityOnOwner = vec7,
					PointVelocityOnTarget = vec8
				};
				return;
			}
			this._ramCollisionData = new MissionShipRam.RamCollisionData
			{
				CapsuleData = ramCapsuleData
			};
		}

		// Token: 0x06000BF1 RID: 3057 RVA: 0x00055DC0 File Offset: 0x00053FC0
		protected override bool CanPhysicsCollideBetweenTwoEntities(WeakGameEntity myEntity, BodyFlags myEntityBodyFlags, WeakGameEntity otherEntity, BodyFlags otherEntityBodyFlags)
		{
			return myEntity != base.GameEntity || !otherEntity.IsValid || !otherEntity.Root.HasScriptOfType<MissionShip>();
		}

		// Token: 0x040006D4 RID: 1748
		private const float SpeedFactorOnMagnitude = 0.03f;

		// Token: 0x040006D5 RID: 1749
		private const string ShipDebrisAndParticlePrefabName = "decal_ship_damaged_b_heap";

		// Token: 0x040006D6 RID: 1750
		private const string ShipBodyPhysicsEntityTag = "body_mesh";

		// Token: 0x040006D7 RID: 1751
		private const float RamHitDirectionThresholdPercentage = 0.3f;

		// Token: 0x040006D8 RID: 1752
		private const float RamStickThresholdPercentage = 0.33f;

		// Token: 0x040006D9 RID: 1753
		private const string PhysicsMaterialName = "wood_ship";

		// Token: 0x040006DA RID: 1754
		private static readonly int RamCollisionSoundEffectSoundId = SoundManager.GetEventGlobalIndex("event:/physics/vessel/ship_ramming");

		// Token: 0x040006DB RID: 1755
		private const BodyFlags RamRaycastExcludeFlags = -2147469567;

		// Token: 0x040006DC RID: 1756
		private static ValueTuple<float, float, float, float, bool>[] _ramQualityThresholds = new ValueTuple<float, float, float, float, bool>[]
		{
			new ValueTuple<float, float, float, float, bool>(10f, 70f, 0.2f, 5f, true),
			new ValueTuple<float, float, float, float, bool>(8f, 60f, 0.3f, 4f, true),
			new ValueTuple<float, float, float, float, bool>(6f, 45f, 0.45f, 2.5f, false),
			new ValueTuple<float, float, float, float, bool>(5f, 30f, 0.65f, 1.5f, false),
			new ValueTuple<float, float, float, float, bool>(3f, 0f, 0.9f, 0.5f, false)
		};

		// Token: 0x040006DD RID: 1757
		private Intersection[] _intersectionsCache = new Intersection[128];

		// Token: 0x040006DE RID: 1758
		private WeakGameEntity[] _entitiesCache = new WeakGameEntity[128];

		// Token: 0x040006DF RID: 1759
		private UIntPtr[] _entityPointersCache = new UIntPtr[128];

		// Token: 0x040006E0 RID: 1760
		private Intersection[] _selectedIntersectionsCache = new Intersection[128];

		// Token: 0x040006E1 RID: 1761
		private MissionShip _ownerShip;

		// Token: 0x040006E2 RID: 1762
		private MissionShip _ramStuckTargetShip;

		// Token: 0x040006E3 RID: 1763
		private bool _ramCollisionBeingHandled;

		// Token: 0x040006E4 RID: 1764
		private MissionShipRam.RamCollisionData _ramDamageData;

		// Token: 0x040006E5 RID: 1765
		private MissionShipRam.RamCollisionData _ramCollisionData;

		// Token: 0x040006E6 RID: 1766
		private Scene _ownScene;

		// Token: 0x040006E7 RID: 1767
		private int _lastRamHitQuality;

		// Token: 0x040006E8 RID: 1768
		[EditableScriptComponentVariable(true, "")]
		private float _ramLength = 5f;

		// Token: 0x040006E9 RID: 1769
		[EditableScriptComponentVariable(true, "")]
		private float _ramRadius = 0.5f;

		// Token: 0x040006EA RID: 1770
		[EditableScriptComponentVariable(true, "")]
		private Vec3 _ramAttachmentPointOffset = Vec3.Zero;

		// Token: 0x040006EB RID: 1771
		[EditableScriptComponentVariable(true, "")]
		private float _ramTierDamageMultiplier = 1f;

		// Token: 0x040006EC RID: 1772
		private float _scaledRamRadius = -1f;

		// Token: 0x040006ED RID: 1773
		private float _scaledRamLength = -1f;

		// Token: 0x0200021A RID: 538
		private struct RamCollisionData
		{
			// Token: 0x17000407 RID: 1031
			// (get) Token: 0x06001B10 RID: 6928 RVA: 0x000B2085 File Offset: 0x000B0285
			public bool IsValid
			{
				get
				{
					return this.TargetShip != null;
				}
			}

			// Token: 0x04000EE0 RID: 3808
			public MissionShip TargetShip;

			// Token: 0x04000EE1 RID: 3809
			public CapsuleData CapsuleData;

			// Token: 0x04000EE2 RID: 3810
			public bool RamWillBeHandled;

			// Token: 0x04000EE3 RID: 3811
			public Vec3 SelectedIntersectionPoint;

			// Token: 0x04000EE4 RID: 3812
			public Vec3 AverageIntersectionPoint;

			// Token: 0x04000EE5 RID: 3813
			public Vec3 RamDirection;

			// Token: 0x04000EE6 RID: 3814
			public float PenetrationLength;

			// Token: 0x04000EE7 RID: 3815
			public bool HasPoint;

			// Token: 0x04000EE8 RID: 3816
			public float CalculatedDamage;

			// Token: 0x04000EE9 RID: 3817
			public Vec3 PointVelocityOnOwner;

			// Token: 0x04000EEA RID: 3818
			public Vec3 PointVelocityOnTarget;
		}
	}
}
