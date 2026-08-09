using System;
using System.Collections.Generic;
using NavalDLC.CharacterDevelopment;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.Objects.UsableMachines;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.MissionLogics
{
	// Token: 0x020000D9 RID: 217
	public class ShipCollisionOutcomeLogic : MissionLogic
	{
		// Token: 0x06001114 RID: 4372 RVA: 0x0007F0D8 File Offset: 0x0007D2D8
		public ShipCollisionOutcomeLogic(Mission mission)
		{
			this._mission = mission;
			this._shipCollisionEffectCooldowns = new Dictionary<MissionShip, float>();
			this._agentActionQueue = new Queue<ValueTuple<MissionShip, Vec3, Vec2, float>>();
		}

		// Token: 0x06001115 RID: 4373 RVA: 0x0007F100 File Offset: 0x0007D300
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._effectRandom = new MBFastRandom();
			this._navalShipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
			this._navalShipsLogic.ShipRammingEvent += this.OnShipRamming;
			this._navalShipsLogic.ShipCollisionEvent += this.OnShipCollision;
		}

		// Token: 0x06001116 RID: 4374 RVA: 0x0007F15D File Offset: 0x0007D35D
		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
			this._navalShipsLogic.ShipRammingEvent -= this.OnShipRamming;
			this._navalShipsLogic.ShipCollisionEvent -= this.OnShipCollision;
		}

		// Token: 0x06001117 RID: 4375 RVA: 0x0007F194 File Offset: 0x0007D394
		public override void OnMissionTick(float dt)
		{
			while (this._agentActionQueue.Count > 0)
			{
				ValueTuple<MissionShip, Vec3, Vec2, float> valueTuple = this._agentActionQueue.Dequeue();
				this.HandleAgentActions(valueTuple.Item1, valueTuple.Item2, valueTuple.Item3, valueTuple.Item4);
			}
			if (this._cameraShakeStartTime > 0f)
			{
				float currentTime = Mission.Current.CurrentTime;
				if (this._cameraShakeStartTime > currentTime - 2f)
				{
					float num = 1f - MathF.Pow((currentTime - this._cameraShakeStartTime) / 2f, 0.4f);
					float num2 = num * this._cameraShakeIntensity * 0.6f;
					float num3 = num2 * 0.02f;
					this._cameraShakeCurrentTimeWithFrequency += dt * 15f * num;
					if (num2 > 0f)
					{
						Vec3 vec = MBPerlin.NoiseVec3(this._cameraShakeCurrentTimeWithFrequency);
						float num4 = (currentTime - this._cameraShakeStartTime) / 2f;
						this._mission.SetCustomCameraLocalOffset2(new Vec3(vec.x * num2, 0f, vec.z * num2, -1f));
						this._mission.SetCustomCameraGlobalOffset(new Vec3(this._cameraShakeInitialVelocity * (9.821568f * num4 - 32.17632f * num4 * num4 + 41.68837f * num4 * num4 * num4 - 25.76999f * num4 * num4 * num4 * num4 + 6.436929f * num4 * num4 * num4 * num4 * num4), 0f, -1f));
						this._mission.SetCustomCameraLocalRotationalOffset(new Vec3(vec.x * num3, vec.y * num3, 0f, -1f));
						return;
					}
				}
				else
				{
					this._cameraShakeStartTime = 0f;
					this._mission.SetCustomCameraLocalOffset2(Vec3.Zero);
					this._mission.SetCustomCameraGlobalOffset(Vec3.Zero);
					this._mission.SetCustomCameraLocalRotationalOffset(Vec3.Zero);
				}
			}
		}

		// Token: 0x06001118 RID: 4376 RVA: 0x0007F384 File Offset: 0x0007D584
		private void OnShipRamming(MissionShip rammingShip, MissionShip rammedShip, float damagePercent, bool isFirstImpact, CapsuleData capsuleData, int ramQuality)
		{
			if (isFirstImpact)
			{
				Vec3 linearVelocityAtGlobalPointForEntityWithDynamicBody = GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(rammingShip.GameEntity, capsuleData.P2);
				Vec3 linearVelocityAtGlobalPointForEntityWithDynamicBody2 = GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(rammedShip.GameEntity, capsuleData.P2);
				Vec3 vec = linearVelocityAtGlobalPointForEntityWithDynamicBody - linearVelocityAtGlobalPointForEntityWithDynamicBody2;
				vec.Normalize();
				this.ShipCollisionEffect(rammingShip, rammedShip.GameEntity, capsuleData.P2, vec, false);
				this.ShipCollisionEffect(rammedShip, rammingShip.GameEntity, capsuleData.P2, vec, false);
			}
		}

		// Token: 0x06001119 RID: 4377 RVA: 0x0007F3F8 File Offset: 0x0007D5F8
		private void OnShipCollision(MissionShip ship, WeakGameEntity targetEntity, BodyFlags bodyFlags, Vec3 averageContactPoint, Vec3 totalImpulseOnShip, bool isFirstImpact)
		{
			if (isFirstImpact)
			{
				Vec3 linearVelocityAtGlobalPointForEntityWithDynamicBody = GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(ship.GameEntity, averageContactPoint);
				Vec3 vec = ((!targetEntity.IsValid || !Extensions.HasAnyFlag<BodyFlags>(targetEntity.BodyFlag, 40)) ? Vec3.Zero : GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(targetEntity, averageContactPoint));
				Vec3 vec2 = linearVelocityAtGlobalPointForEntityWithDynamicBody - vec;
				vec2.Normalize();
				this.ShipCollisionEffect(ship, targetEntity, averageContactPoint, -vec2, true);
			}
		}

		// Token: 0x0600111A RID: 4378 RVA: 0x0007F464 File Offset: 0x0007D664
		private void ShipCollisionEffect(MissionShip ship, WeakGameEntity targetEntity, Vec3 collisionGlobalPosition, Vec3 collisionDirection, bool shouldMakeSound)
		{
			float currentTime = Mission.Current.CurrentTime;
			float num;
			if (!this._shipCollisionEffectCooldowns.TryGetValue(ship, out num) || currentTime - num >= 2f)
			{
				object obj = targetEntity.IsValid && Extensions.HasAnyFlag<BodyFlags>(targetEntity.BodyFlag, 40);
				Vec3 linearVelocityAtGlobalPointForEntityWithDynamicBody = GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(ship.GameEntity, collisionGlobalPosition);
				object obj2 = obj;
				Vec3 vec = ((obj2 == null) ? Vec3.Zero : GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(targetEntity, collisionGlobalPosition));
				float num2 = (linearVelocityAtGlobalPointForEntityWithDynamicBody - vec).Normalize();
				float num3 = ((obj2 != null) ? GameEntityPhysicsExtensions.GetMass(targetEntity) : float.MaxValue);
				float mass = ship.GameEntity.Mass;
				float num4 = 1f / mass + 1f / num3;
				float num5 = num2 * num2 * (1f / num4);
				float num6 = 0.15f * (num5 / mass);
				if (num6 >= 1f)
				{
					this._shipCollisionEffectCooldowns[ship] = currentTime;
					Vec2 asVec = ship.Physics.LinearVelocity.AsVec2;
					asVec.Normalize();
					Agent mainAgent = this._mission.MainAgent;
					if (mainAgent != null && mainAgent.IsActive() && ship.GetIsAgentOnShip(this._mission.MainAgent, false))
					{
						this._cameraShakeStartTime = currentTime;
						this._cameraShakeIntensity = MathF.Clamp(num6 * 0.3f, 1f, 3f);
						this._cameraShakeInitialVelocity = asVec * num6 * 0.5f;
						this._cameraShakeCurrentTimeWithFrequency = 0f;
					}
					MissionShip firstScriptOfType = targetEntity.GetFirstScriptOfType<MissionShip>();
					shouldMakeSound = shouldMakeSound && (firstScriptOfType == null || !this._shipCollisionEffectCooldowns.TryGetValue(firstScriptOfType, out num) || currentTime - num >= 2f);
					if (shouldMakeSound)
					{
						SoundEventParameter soundEventParameter;
						soundEventParameter..ctor("Force", MathF.Min(num6 * 0.1f, 0.5f));
						MBSoundEvent.PlaySound(ShipCollisionOutcomeLogic._ramCollisionSoundEffectSoundId, ref soundEventParameter, collisionGlobalPosition);
					}
					this._agentActionQueue.Enqueue(new ValueTuple<MissionShip, Vec3, Vec2, float>(ship, collisionGlobalPosition, collisionDirection.AsVec2, num6));
					foreach (ShipUnmannedOar shipUnmannedOar in ship.ShipUnmannedOars)
					{
						MatrixFrame matrixFrame = shipUnmannedOar.GameEntity.GetGlobalFrameImpreciseForFixedTick();
						float num7 = matrixFrame.origin.DistanceSquared(collisionGlobalPosition);
						if (num7 < 900f)
						{
							float num8 = num6 * 0.04f * (30f / (MathF.Sqrt(num7) + 0.1f)) * this._effectRandom.NextFloat();
							if (num8 > 1f)
							{
								shipUnmannedOar.SetSlowDownPhaseForDuration(Math.Max(1f - num8 * 0.3f, 0f), Math.Min(num8, 3f));
							}
						}
					}
					foreach (ShipOarMachine shipOarMachine in ship.LeftSideShipOarMachines)
					{
						MatrixFrame matrixFrame = shipOarMachine.GameEntity.GetGlobalFrameImpreciseForFixedTick();
						float num9 = matrixFrame.origin.DistanceSquared(collisionGlobalPosition);
						if (num9 < 900f)
						{
							float num10 = num6 * 0.04f * (30f / (MathF.Sqrt(num9) + 0.1f)) * this._effectRandom.NextFloat();
							if (num10 > 1f)
							{
								shipOarMachine.SetSlowDownPhaseForDuration(Math.Max(1f - num10 * 0.3f, 0f), Math.Min(num10, 3f));
							}
						}
					}
					foreach (ShipOarMachine shipOarMachine2 in ship.RightSideShipOarMachines)
					{
						MatrixFrame matrixFrame = shipOarMachine2.GameEntity.GetGlobalFrameImpreciseForFixedTick();
						float num11 = matrixFrame.origin.DistanceSquared(collisionGlobalPosition);
						if (num11 < 900f)
						{
							float num12 = num6 * 0.04f * (30f / (MathF.Sqrt(num11) + 0.1f)) * this._effectRandom.NextFloat();
							if (num12 > 1f)
							{
								shipOarMachine2.SetSlowDownPhaseForDuration(Math.Max(1f - num12 * 0.3f, 0f), Math.Min(num12, 3f));
							}
						}
					}
				}
			}
		}

		// Token: 0x0600111B RID: 4379 RVA: 0x0007F8CC File Offset: 0x0007DACC
		public void ActivateCooldownForShip(MissionShip ship, float cooldown)
		{
			float currentTime = Mission.Current.CurrentTime;
			float num;
			if (!this._shipCollisionEffectCooldowns.TryGetValue(ship, out num) || currentTime - num > -cooldown)
			{
				this._shipCollisionEffectCooldowns[ship] = currentTime - (2f - cooldown);
			}
		}

		// Token: 0x0600111C RID: 4380 RVA: 0x0007F910 File Offset: 0x0007DB10
		private void HandleAgentActions(MissionShip ship, Vec3 collisionGlobalPosition, Vec2 shipDirection, float impactFactor)
		{
			foreach (Agent agent in this._mission.Agents)
			{
				if (!agent.IsUsingGameObject || !Extensions.HasAnyFlag<AnimFlags>(agent.GetCurrentAnimationFlag(0), 17592186044416L))
				{
					float num = agent.Position.DistanceSquared(collisionGlobalPosition);
					if (num < 900f && ship.GetIsAgentOnShip(agent, false))
					{
						int effectiveSkill = MissionGameModels.Current.AgentStatCalculateModel.GetEffectiveSkill(agent, NavalSkills.Mariner);
						float num2 = impactFactor * 0.15f * (30f / (MathF.Sqrt(num) + 0.1f)) * (0.5f + this._effectRandom.NextFloat() * 0.5f) * (100f / ((float)effectiveSkill + 100f));
						if (num2 > 1f)
						{
							ShipControllerMachine shipControllerMachine = ship.ShipControllerMachine;
							if (((shipControllerMachine != null) ? shipControllerMachine.PilotAgent : null) == agent)
							{
								num2 = Math.Min(num2, 2f);
							}
							float num3 = agent.GetMovementDirection().DotProduct(shipDirection);
							if (num3 > 0.7f)
							{
								Agent agent2 = agent;
								int num4 = 0;
								ActionIndexCache actionIndexCache = ((num2 >= 3f) ? ActionIndexCache.act_stagger_backward_3 : ((num2 >= 2f) ? ActionIndexCache.act_stagger_backward_2 : ActionIndexCache.act_stagger_backward));
								agent2.SetActionChannel(num4, ref actionIndexCache, false, 0L, 0f, this._effectRandom.NextFloatRanged(0.7f, 1.3f), -0.2f, 0.4f, this._effectRandom.NextFloatRanged(0f, 0.3f), false, -0.2f, 0, true);
							}
							else if (num3 < -0.7f)
							{
								Agent agent3 = agent;
								int num5 = 0;
								ActionIndexCache actionIndexCache = ((num2 >= 3f) ? ActionIndexCache.act_stagger_forward_3 : ((num2 >= 2f) ? ActionIndexCache.act_stagger_forward_2 : ActionIndexCache.act_stagger_forward));
								agent3.SetActionChannel(num5, ref actionIndexCache, false, 0L, 0f, this._effectRandom.NextFloatRanged(0.7f, 1.3f), -0.2f, 0.4f, this._effectRandom.NextFloatRanged(0f, 0.3f), false, -0.2f, 0, true);
							}
							else if (agent.GetMovementDirection().RightVec().DotProduct(shipDirection) > 0f)
							{
								Agent agent4 = agent;
								int num6 = 0;
								ActionIndexCache actionIndexCache = ((num2 >= 3f) ? ActionIndexCache.act_stagger_left_3 : ((num2 >= 2f) ? ActionIndexCache.act_stagger_left_2 : ActionIndexCache.act_stagger_left));
								agent4.SetActionChannel(num6, ref actionIndexCache, false, 0L, 0f, this._effectRandom.NextFloatRanged(0.7f, 1.3f), -0.2f, 0.4f, this._effectRandom.NextFloatRanged(0f, 0.3f), false, -0.2f, 0, true);
							}
							else
							{
								Agent agent5 = agent;
								int num7 = 0;
								ActionIndexCache actionIndexCache = ((num2 >= 3f) ? ActionIndexCache.act_stagger_right_3 : ((num2 >= 2f) ? ActionIndexCache.act_stagger_right_2 : ActionIndexCache.act_stagger_right));
								agent5.SetActionChannel(num7, ref actionIndexCache, false, 0L, 0f, this._effectRandom.NextFloatRanged(0.7f, 1.3f), -0.2f, 0.4f, this._effectRandom.NextFloatRanged(0f, 0.3f), false, -0.2f, 0, true);
							}
						}
					}
				}
			}
		}

		// Token: 0x040009E6 RID: 2534
		private const float EffectCooldownForShipInSeconds = 2f;

		// Token: 0x040009E7 RID: 2535
		private static readonly int _ramCollisionSoundEffectSoundId = SoundManager.GetEventGlobalIndex("event:/physics/vessel/ship_ramming");

		// Token: 0x040009E8 RID: 2536
		private readonly Mission _mission;

		// Token: 0x040009E9 RID: 2537
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x040009EA RID: 2538
		private float _cameraShakeStartTime;

		// Token: 0x040009EB RID: 2539
		private float _cameraShakeCurrentTimeWithFrequency;

		// Token: 0x040009EC RID: 2540
		private float _cameraShakeIntensity;

		// Token: 0x040009ED RID: 2541
		private Vec2 _cameraShakeInitialVelocity;

		// Token: 0x040009EE RID: 2542
		private readonly Dictionary<MissionShip, float> _shipCollisionEffectCooldowns;

		// Token: 0x040009EF RID: 2543
		private readonly Queue<ValueTuple<MissionShip, Vec3, Vec2, float>> _agentActionQueue;

		// Token: 0x040009F0 RID: 2544
		private MBFastRandom _effectRandom;
	}
}
