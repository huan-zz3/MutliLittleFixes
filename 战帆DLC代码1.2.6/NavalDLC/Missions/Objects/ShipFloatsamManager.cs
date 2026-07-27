using System;
using System.Collections.Generic;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.NavalPhysics;
using NavalDLC.Missions.Objects.UsableMachines;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.Objects
{
	// Token: 0x020000A6 RID: 166
	public class ShipFloatsamManager : ScriptComponentBehavior
	{
		// Token: 0x06000CDB RID: 3291 RVA: 0x00062538 File Offset: 0x00060738
		internal ShipFloatsamManager()
		{
			Color color = Colors.White;
			this._shipColor = color.ToUnsignedInteger();
			color = Colors.White;
			this._shipDecalColor = color.ToUnsignedInteger();
			this._collisionDecals = new List<GameEntity>();
			this._shieldName = "";
			base..ctor();
		}

		// Token: 0x06000CDC RID: 3292 RVA: 0x000625D4 File Offset: 0x000607D4
		protected override void OnInit()
		{
			this._identityFrameParticleParent = GameEntity.CreateEmpty(base.GameEntity.Scene, false, false, false);
			this._identityFrameParticleParent.EntityFlags |= 131072;
			this._scrapeParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_game_ship_scrape_emit_on_move");
			this._collisionHitParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_game_ship_collision");
			this._midCollisionHitParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_naval_ship_hit_mid");
			this._bigCollisionHitParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_naval_ship_hit_large");
			WeakGameEntity firstChildEntityWithTagRecursive = base.GameEntity.GetFirstChildEntityWithTagRecursive("body_mesh");
			if (firstChildEntityWithTagRecursive != null)
			{
				this._bodyEntity = GameEntity.CreateFromWeakEntity(firstChildEntityWithTagRecursive);
			}
			ColorAssigner firstScriptOfType = base.GameEntity.GetFirstScriptOfType<ColorAssigner>();
			if (firstScriptOfType != null)
			{
				this._shipColor = firstScriptOfType.ShipColor.ToUnsignedInteger();
				this._shipDecalColor = firstScriptOfType.RamDebrisColor.ToUnsignedInteger();
			}
			this._floatsamMissionLogic = Mission.Current.GetMissionBehavior<NavalFloatsamLogic>();
			List<WeakGameEntity> list = new List<WeakGameEntity>();
			base.GameEntity.GetChildrenRecursive(ref list);
			foreach (WeakGameEntity weakGameEntity in list)
			{
				ShipShieldComponent firstScriptOfType2 = weakGameEntity.GetFirstScriptOfType<ShipShieldComponent>();
				if (firstScriptOfType2 != null)
				{
					firstScriptOfType2.OnDestroyed += new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnShieldDestroyed);
					this._shieldName = weakGameEntity.Name;
				}
			}
			this._ownMissionShipCached = base.GameEntity.GetFirstScriptOfType<MissionShip>();
			if (this._ownMissionShipCached != null)
			{
				NavalShipsLogic missionBehavior = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
				if (missionBehavior != null)
				{
					missionBehavior.ShipHitEvent += this.OnShipHit;
					missionBehavior.ShipRammingEvent += this.OnShipRamming;
				}
			}
		}

		// Token: 0x06000CDD RID: 3293 RVA: 0x0006279C File Offset: 0x0006099C
		protected override void OnTick(float dt)
		{
			if (!this._floatsamSystemEnabled)
			{
				return;
			}
			this.CheckSinking();
			this.ProcessImpulseEffects();
			this.ProcessShieldBreakRecords();
		}

		// Token: 0x06000CDE RID: 3294 RVA: 0x000627B9 File Offset: 0x000609B9
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 2;
		}

		// Token: 0x06000CDF RID: 3295 RVA: 0x000627BC File Offset: 0x000609BC
		protected override void OnPhysicsCollision(ref PhysicsContact contact, WeakGameEntity entity0, WeakGameEntity entity1)
		{
			if (!entity1.HasScriptComponent(MissionShip.MissionShipScriptNameHash))
			{
				return;
			}
			if (!this._floatsamSystemEnabled)
			{
				return;
			}
			MatrixFrame bodyWorldTransform = entity0.GetBodyWorldTransform();
			bool flag = true;
			Vec3 vec = Vec3.Zero;
			Vec3 vec2 = Vec3.Zero;
			float num = 0f;
			float num2 = 0f;
			for (int i = 0; i < contact.NumberOfContactPairs; i++)
			{
				PhysicsContactPair physicsContactPair = contact[i];
				for (int j = 0; j < physicsContactPair.NumberOfContacts; j++)
				{
					PhysicsContactInfo physicsContactInfo = physicsContactPair[j];
					vec += physicsContactInfo.Position;
					num += physicsContactInfo.Impulse.Length;
					vec2 += physicsContactInfo.Normal;
					Color white = Colors.White;
					if (physicsContactPair.ContactEventType == null)
					{
						flag = false;
					}
					else if (physicsContactPair.ContactEventType == 1)
					{
						flag = false;
					}
					num2 += 1f;
				}
			}
			if (num2 > 0f)
			{
				vec /= num2;
				vec2 /= num2;
				vec2.Normalize();
				vec2 *= -1f;
			}
			ShipFloatsamManager.ScrapeRecord scrapeRecord;
			if (this._scrapeRecords.TryGetValue(entity1, out scrapeRecord))
			{
				if (flag || num2 == 0f)
				{
					base.GameEntity.RemoveComponent(scrapeRecord.Particle);
					this._scrapeRecords.Remove(entity1);
					return;
				}
				MatrixFrame identity = MatrixFrame.Identity;
				identity.rotation.u = Vec3.Up;
				identity.rotation.s = vec2;
				identity.rotation.f = -identity.rotation.s.CrossProductWithUp();
				identity.rotation.s = Vec3.CrossProduct(identity.rotation.f, identity.rotation.u);
				identity.origin = vec;
				scrapeRecord.AccumulatedDistance += scrapeRecord.PreviousPosition.Distance(vec);
				scrapeRecord.PreviousPosition = vec;
				scrapeRecord.Particle.SetLocalFrame(ref identity);
				if (scrapeRecord.AccumulatedDistance > 2.5f)
				{
					scrapeRecord.AccumulatedDistance = 0f;
					if (this._numberOfPendingImpulseRecords < 10)
					{
						this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].AveragePosition = vec;
						this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].AverageNormal = vec2;
						this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].TotalImpulse = 150000f;
						Vec3 vec3 = Vec3.Zero;
						if (GameEntityPhysicsExtensions.HasDynamicRigidBody(entity0))
						{
							vec3 = GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(base.GameEntity, vec);
						}
						this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].Speed = vec3;
						this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].DebrisType = ShipFloatsamManager.DebrisType.Scrape;
						this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].DecalType = ShipFloatsamManager.DecalType.Scrape;
						this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].InitialSpeedMultiplier = 0.25f;
						this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].ShipLocalPosition = bodyWorldTransform.TransformToLocal(ref vec);
						this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].ShipLocalNormal = bodyWorldTransform.rotation.TransformToLocal(ref vec2);
						this._numberOfPendingImpulseRecords++;
						return;
					}
				}
			}
			else if (num2 > 0f)
			{
				ShipFloatsamManager.ScrapeRecord scrapeRecord2 = new ShipFloatsamManager.ScrapeRecord();
				MatrixFrame identity2 = MatrixFrame.Identity;
				identity2.rotation.u = Vec3.Up;
				identity2.rotation.s = vec2;
				identity2.rotation.f = -identity2.rotation.s.CrossProductWithUp();
				identity2.rotation.s = Vec3.CrossProduct(identity2.rotation.f, identity2.rotation.u);
				identity2.origin = vec;
				scrapeRecord2.Particle = ParticleSystem.CreateParticleSystemAttachedToEntity(this._scrapeParticleIndex, this._identityFrameParticleParent, ref identity2);
				scrapeRecord2.PreviousPosition = vec;
				this._scrapeRecords.Add(entity1, scrapeRecord2);
				if (num > 15000f)
				{
					base.GameEntity.Scene.CreateBurstParticle(this._collisionHitParticleIndex, identity2);
				}
				Vec3 vec4 = Vec3.Zero;
				if (GameEntityPhysicsExtensions.HasDynamicRigidBody(entity0))
				{
					vec4 = GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(base.GameEntity, vec);
				}
				Vec3 vec5 = Vec3.Zero;
				if (GameEntityPhysicsExtensions.HasDynamicRigidBody(entity1))
				{
					vec5 = GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(base.GameEntity, vec);
				}
				if (this._numberOfPendingImpulseRecords < 10)
				{
					this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].AveragePosition = vec;
					this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].AverageNormal = vec2;
					this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].TotalImpulse = num;
					this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].Speed = vec4 - vec5;
					this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].DebrisType = ShipFloatsamManager.DebrisType.Scrape;
					this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].DecalType = ShipFloatsamManager.DecalType.Collision;
					this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].InitialSpeedMultiplier = 1f;
					this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].ShipLocalPosition = bodyWorldTransform.TransformToLocal(ref vec);
					this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].ShipLocalNormal = bodyWorldTransform.rotation.TransformToLocal(ref vec2);
					this._numberOfPendingImpulseRecords++;
				}
			}
		}

		// Token: 0x06000CE0 RID: 3296 RVA: 0x00062D28 File Offset: 0x00060F28
		private void ProcessImpulseEffects()
		{
			while (this._numberOfPendingImpulseRecords > 0)
			{
				int num = this._numberOfPendingImpulseRecords - 1;
				this.ProcessImpactEffect(this._impulseRecordsToProcess[num]);
				this._numberOfPendingImpulseRecords--;
			}
		}

		// Token: 0x06000CE1 RID: 3297 RVA: 0x00062D6C File Offset: 0x00060F6C
		private void ProcessShieldBreakRecords()
		{
			while (this._numberOfPendingShieldBreakRecords > 0)
			{
				int num = this._numberOfPendingShieldBreakRecords - 1;
				this.SpawnBrokenShield(this._shieldBreakRecords[num]);
				this._numberOfPendingShieldBreakRecords--;
			}
		}

		// Token: 0x06000CE2 RID: 3298 RVA: 0x00062DB0 File Offset: 0x00060FB0
		private void SpawnBrokenShield(ShipFloatsamManager.ShieldBreakRecord record)
		{
			GameEntity gameEntity = GameEntity.Instantiate(base.GameEntity.Scene, record.PrefabName, true, true, "");
			MatrixFrame matrixFrame = base.GameEntity.GetGlobalFrame().TransformToParent(ref record.ShipLocalSpawnFrame);
			Vec3 vec = ShipFloatsamManager.ComputeRandomPositionOffset(in this._randomGenerator, 0.75f);
			matrixFrame.origin += vec;
			gameEntity.SetFrame(ref matrixFrame, true);
			GameEntityPhysicsExtensions.SetLinearVelocity(gameEntity, record.LinearVelocity);
			this.SetRandomAngularVelocityToEntity(gameEntity);
			if (record.BannerTexture != null)
			{
				foreach (Mesh mesh in gameEntity.GetFirstChildEntityWithTag("shield_mesh_entity").GetAllMeshesWithTag("banner_with_faction_color"))
				{
					Material material = mesh.GetMaterial().CreateCopy();
					material.SetTexture(1, record.BannerTexture);
					uint num = (uint)material.GetShader().GetMaterialShaderFlagMask("use_tableau_blending", true);
					ulong shaderFlags = material.GetShaderFlags();
					material.SetShaderFlags(shaderFlags | (ulong)num);
					mesh.SetMaterial(material);
				}
			}
			if (this._floatsamMissionLogic != null)
			{
				this._floatsamMissionLogic.RegisterFloatsamInstance(gameEntity);
			}
		}

		// Token: 0x06000CE3 RID: 3299 RVA: 0x00062F00 File Offset: 0x00061100
		private static Vec3 ComputeRandomPositionOffset(in MBFastRandom randGenerator, float halfRange)
		{
			Vec3 vec = default(Vec3);
			vec.x = randGenerator.NextFloatRanged(-halfRange, halfRange);
			vec.y = randGenerator.NextFloatRanged(-halfRange, halfRange);
			vec.z = randGenerator.NextFloatRanged(-halfRange, halfRange);
			return vec;
		}

		// Token: 0x06000CE4 RID: 3300 RVA: 0x00062F4C File Offset: 0x0006114C
		private void ProcessImpactEffect(ShipFloatsamManager.ImpulseRecord record)
		{
			int num = ((record.DebrisType == ShipFloatsamManager.DebrisType.Ramming) ? 10 : 7);
			int num2 = MathF.Min((int)(record.TotalImpulse / 150000f), num);
			for (int i = 0; i < num2; i++)
			{
				string randomDebrisPrefab = this.GetRandomDebrisPrefab(record.DebrisType);
				GameEntity gameEntity = GameEntity.Instantiate(base.GameEntity.Scene, randomDebrisPrefab, true, true, "");
				MatrixFrame identity = MatrixFrame.Identity;
				identity.rotation.RotateAboutSide(this._randomGenerator.NextFloatRanged(0f, 6.2831855f));
				identity.rotation.RotateAboutForward(this._randomGenerator.NextFloatRanged(0f, 6.2831855f));
				identity.rotation.RotateAboutUp(this._randomGenerator.NextFloatRanged(0f, 6.2831855f));
				identity.rotation.Orthonormalize();
				Vec3 vec = ShipFloatsamManager.ComputeRandomPositionOffset(in this._randomGenerator, 0.75f);
				identity.origin = record.AveragePosition + vec;
				gameEntity.SetFrame(ref identity, true);
				Vec3 vec2 = record.TotalImpulse * record.AverageNormal;
				float num3 = (0.27f + this._randomGenerator.NextFloatRanged(0f, 0.3f)) * 0.032f;
				Vec3 vec3 = record.Speed + vec2 / GameEntityPhysicsExtensions.GetMass(gameEntity);
				float num4 = vec3.Normalize();
				vec3 = vec3.RotateAboutAnArbitraryVector(record.AverageNormal, this._randomGenerator.NextFloatRanged(-1.5707964f, 1.5707964f));
				num4 *= num3;
				num4 = MathF.Min(num4, 30f);
				Vec3 vec4 = vec3 + Vec3.Up * 0.75f;
				vec3 = vec4.NormalizedCopy() * num4;
				GameEntityPhysicsExtensions.SetLinearVelocity(gameEntity, vec3);
				foreach (Mesh mesh in gameEntity.GetAllMeshesWithTag("auto_factor_color"))
				{
					mesh.Color = this._shipColor;
				}
				this.SetRandomAngularVelocityToEntity(gameEntity);
				if (this._floatsamMissionLogic != null)
				{
					this._floatsamMissionLogic.RegisterFloatsamInstance(gameEntity);
				}
			}
			if (this._collisionDecals.Count < 30)
			{
				MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
				Vec3 vec5 = record.ShipLocalPosition;
				Vec3 vec6 = record.ShipLocalNormal;
				if (this._bodyEntity != null)
				{
					float num5 = 2.5f;
					Vec3 vec4 = globalFrame.rotation.TransformToParent(ref record.ShipLocalNormal);
					Vec3 vec7 = -vec4.NormalizedCopy();
					Vec3 vec8 = globalFrame.TransformToParent(ref record.ShipLocalPosition) - vec7 * num5;
					Vec3 zero = Vec3.Zero;
					float num6 = 0f;
					if (this._bodyEntity.RayHitEntityWithNormal(vec8, vec7, num5, ref zero, ref num6))
					{
						vec4 = vec8 + vec7 * num6;
						vec5 = globalFrame.TransformToLocalNonOrthogonal(ref vec4);
						vec4 = globalFrame.rotation.TransformToLocal(ref zero);
						vec6 = vec4.NormalizedCopy();
					}
				}
				MatrixFrame identity2 = MatrixFrame.Identity;
				identity2.origin = vec5;
				identity2.rotation.u = vec6;
				identity2.rotation.f = Vec3.Up;
				identity2.rotation.s = Vec3.CrossProduct(identity2.rotation.u, identity2.rotation.s);
				identity2.rotation.f.Normalize();
				identity2.rotation.s = Vec3.CrossProduct(identity2.rotation.f, identity2.rotation.u);
				if (record.DecalType == ShipFloatsamManager.DecalType.Scrape)
				{
					float num7 = this._randomGenerator.NextFloatRanged(1.75f, 2.75f);
					float num8 = this._randomGenerator.NextFloatRanged(1.25f, 1.75f);
					Vec3 vec4 = new Vec3(num7, num8, 0.2f, -1f);
					identity2.rotation.ApplyScaleLocal(ref vec4);
				}
				else if (record.DecalType == ShipFloatsamManager.DecalType.Collision)
				{
					float num9 = this._randomGenerator.NextFloatRanged(1.55f, 2.55f);
					Vec3 vec4 = new Vec3(num9, 1f, 0.2f, -1f);
					identity2.rotation.ApplyScaleLocal(ref vec4);
				}
				string text = "";
				if (record.DecalType == ShipFloatsamManager.DecalType.Collision)
				{
					text = this.GetRandomCollisionDecalPrefab();
				}
				else if (record.DecalType == ShipFloatsamManager.DecalType.Scrape)
				{
					text = this.GetRandomScrapeDecalPrefab();
				}
				GameEntity gameEntity2 = GameEntity.Instantiate(base.GameEntity.Scene, text, MatrixFrame.Identity, true);
				base.GameEntity.AddChild(gameEntity2.WeakEntity, false);
				gameEntity2.SetFrame(ref identity2, true);
				gameEntity2.SetFactorColor(this._shipDecalColor);
				this._collisionDecals.Add(gameEntity2);
			}
		}

		// Token: 0x06000CE5 RID: 3301 RVA: 0x0006342C File Offset: 0x0006162C
		private string GetRandomDebrisPrefab(ShipFloatsamManager.DebrisType type)
		{
			switch (type)
			{
			case ShipFloatsamManager.DebrisType.Generic:
			{
				int num = this._randomGenerator.Next(ShipFloatsamManager.GenericPrefabNames.Length);
				return ShipFloatsamManager.GenericPrefabNames[num];
			}
			case ShipFloatsamManager.DebrisType.Scrape:
			{
				int num2 = this._randomGenerator.Next(ShipFloatsamManager.ScrapeDebrisPrefabNames.Length);
				return ShipFloatsamManager.ScrapeDebrisPrefabNames[num2];
			}
			case ShipFloatsamManager.DebrisType.Ramming:
			{
				int num3 = this._randomGenerator.Next(ShipFloatsamManager.RammingPrefabNames.Length);
				return ShipFloatsamManager.RammingPrefabNames[num3];
			}
			default:
				return "";
			}
		}

		// Token: 0x06000CE6 RID: 3302 RVA: 0x000634A4 File Offset: 0x000616A4
		private string GetRandomCollisionDecalPrefab()
		{
			int num = this._randomGenerator.Next(ShipFloatsamManager.CollisionDecalPrefabNames.Length);
			return ShipFloatsamManager.CollisionDecalPrefabNames[num];
		}

		// Token: 0x06000CE7 RID: 3303 RVA: 0x000634CC File Offset: 0x000616CC
		private string GetRandomScrapeDecalPrefab()
		{
			int num = this._randomGenerator.Next(ShipFloatsamManager.ScrapeDecalPrefabNames.Length);
			return ShipFloatsamManager.ScrapeDecalPrefabNames[num];
		}

		// Token: 0x06000CE8 RID: 3304 RVA: 0x000634F4 File Offset: 0x000616F4
		private void SetRandomAngularVelocityToEntity(GameEntity entity)
		{
			float num = 0.8f;
			GameEntityPhysicsExtensions.SetAngularVelocity(entity, new Vec3(this._randomGenerator.NextFloatRanged(-num, num), this._randomGenerator.NextFloatRanged(-num, num), this._randomGenerator.NextFloatRanged(-num, num), -1f));
		}

		// Token: 0x06000CE9 RID: 3305 RVA: 0x00063544 File Offset: 0x00061744
		private void CheckSinking()
		{
			if (!this._sinkingFloatsamSpawned && this._ownMissionShipCached.Physics.NavalSinkingState != NavalPhysics.SinkingState.Floating)
			{
				Vec3 globalPosition = base.GameEntity.GlobalPosition;
				BoundingBox physicsBoundingBoxWithoutChildren = this._ownMissionShipCached.Physics.PhysicsBoundingBoxWithoutChildren;
				float num = (physicsBoundingBoxWithoutChildren.max.z - physicsBoundingBoxWithoutChildren.min.z) * 0.75f;
				if (globalPosition.z + num < base.GameEntity.GetWaterLevelAtPosition(globalPosition.AsVec2, true, false))
				{
					Vec3 min = physicsBoundingBoxWithoutChildren.min;
					Vec3 max = physicsBoundingBoxWithoutChildren.max;
					max.z = min.z;
					Vec3 vec = max - min;
					float num2 = MathF.Max(Vec2.DotProduct(vec.AsVec2, vec.AsVec2) / 1000f, 1f);
					this._sinkingFloatsamSpawned = true;
					int num3 = (int)((float)this._randomGenerator.Next(7, 10) * num2);
					for (int i = 0; i < num3; i++)
					{
						GameEntity gameEntity = GameEntity.Instantiate(base.GameEntity.Scene, "floatable_debris_oar_a", true, true, "");
						if (gameEntity != null)
						{
							Vec3 vec2 = min + new Vec3(vec.x * this._randomGenerator.NextFloat(), vec.y * this._randomGenerator.NextFloat(), 0f, -1f);
							MatrixFrame identity = MatrixFrame.Identity;
							identity.origin = globalPosition + vec2;
							float waterLevelAtPosition = base.GameEntity.GetWaterLevelAtPosition(identity.origin.AsVec2, true, false);
							identity.origin.z = waterLevelAtPosition - 1.5f * this._randomGenerator.NextFloatRanged(1f, 4.5f);
							gameEntity.SetFrame(ref identity, true);
							gameEntity.SetFactorColor(this._shipColor);
							this.SetRandomAngularVelocityToEntity(gameEntity);
							if (this._floatsamMissionLogic != null)
							{
								this._floatsamMissionLogic.RegisterFloatsamInstance(gameEntity);
							}
						}
					}
					Vec3 vec3 = min + new Vec3(vec.x * this._randomGenerator.NextFloat(), vec.y * this._randomGenerator.NextFloat(), 0f, -1f);
					GameEntity gameEntity2 = GameEntity.Instantiate(base.GameEntity.Scene, "floatable_debris_rudder", true, true, "");
					MatrixFrame identity2 = MatrixFrame.Identity;
					identity2.origin = globalPosition + vec3;
					float waterLevelAtPosition2 = base.GameEntity.GetWaterLevelAtPosition(identity2.origin.AsVec2, true, false);
					identity2.origin.z = waterLevelAtPosition2 - 1.5f * this._randomGenerator.NextFloatRanged(1f, 4.5f);
					gameEntity2.SetFrame(ref identity2, true);
					gameEntity2.SetFactorColor(this._shipColor);
					this.SetRandomAngularVelocityToEntity(gameEntity2);
					if (this._floatsamMissionLogic != null)
					{
						this._floatsamMissionLogic.RegisterFloatsamInstance(gameEntity2);
					}
					GameEntity gameEntity3 = GameEntity.Instantiate(base.GameEntity.Scene, "floatable_debris_mast", true, true, "");
					if (gameEntity3 != null)
					{
						Vec3 vec4 = min + new Vec3(vec.x * this._randomGenerator.NextFloat(), vec.y * this._randomGenerator.NextFloat(), 0f, -1f);
						MatrixFrame identity3 = MatrixFrame.Identity;
						identity3.origin = globalPosition + vec4;
						float waterLevelAtPosition3 = base.GameEntity.GetWaterLevelAtPosition(identity3.origin.AsVec2, true, false);
						identity3.origin.z = waterLevelAtPosition3 - 1.5f * this._randomGenerator.NextFloatRanged(3.5f, 5.5f);
						gameEntity3.SetFrame(ref identity3, true);
						gameEntity3.SetFactorColor(this._shipColor);
						this.SetRandomAngularVelocityToEntity(gameEntity3);
						if (this._floatsamMissionLogic != null)
						{
							this._floatsamMissionLogic.RegisterFloatsamInstance(gameEntity3);
						}
					}
					int num4 = (int)((float)this._randomGenerator.Next(10, 16) * num2);
					for (int j = 0; j < num4; j++)
					{
						Vec3 vec5 = min + new Vec3(vec.x * this._randomGenerator.NextFloat(), vec.y * this._randomGenerator.NextFloat(), 0f, -1f);
						GameEntity gameEntity4 = GameEntity.Instantiate(base.GameEntity.Scene, this.GetRandomDebrisPrefab(ShipFloatsamManager.DebrisType.Generic), true, true, "");
						MatrixFrame identity4 = MatrixFrame.Identity;
						identity4.origin = globalPosition + vec5;
						float waterLevelAtPosition4 = base.GameEntity.GetWaterLevelAtPosition(identity4.origin.AsVec2, true, false);
						identity4.origin.z = waterLevelAtPosition4 - 1.5f * this._randomGenerator.NextFloatRanged(1f, 4.5f);
						gameEntity4.SetFrame(ref identity4, true);
						gameEntity4.SetFactorColor(this._shipColor);
						this.SetRandomAngularVelocityToEntity(gameEntity4);
						if (this._floatsamMissionLogic != null)
						{
							this._floatsamMissionLogic.RegisterFloatsamInstance(gameEntity4);
						}
					}
				}
			}
		}

		// Token: 0x06000CEA RID: 3306 RVA: 0x00063A64 File Offset: 0x00061C64
		private void OnShieldDestroyed(DestructableComponent target, Agent attackerAgent, in MissionWeapon weapon, ScriptComponentBehavior attackerScriptComponentBehavior, int inflictedDamage)
		{
			if (!this._floatsamSystemEnabled)
			{
				return;
			}
			if (this._numberOfPendingShieldBreakRecords < 10)
			{
				Texture texture = null;
				MetaMesh metaMesh = target.GameEntity.GetComponentAtIndex(0, 0) as MetaMesh;
				if (metaMesh != null && metaMesh.MeshCount > 0)
				{
					texture = metaMesh.GetMeshAtIndex(0).GetMaterial().GetTexture(1);
				}
				string text = "floatable_debris_";
				text += this._shieldName;
				if (this._randomGenerator.NextFloat() > 0.15f)
				{
					int num = this._randomGenerator.Next(0, 3);
					if (num == 0)
					{
						text += "_broken_a";
					}
					else if (num == 1)
					{
						text += "_broken_b";
					}
					else if (num == 2)
					{
						text += "_broken_c";
					}
				}
				Vec3 vec = GameEntityPhysicsExtensions.GetLinearVelocity(target.GameEntity.Root);
				vec += Vec3.Up * 1.5f;
				ShipFloatsamManager.ShieldBreakRecord[] shieldBreakRecords = this._shieldBreakRecords;
				int numberOfPendingShieldBreakRecords = this._numberOfPendingShieldBreakRecords;
				MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
				MatrixFrame globalFrame2 = target.GameEntity.GetGlobalFrame();
				shieldBreakRecords[numberOfPendingShieldBreakRecords].ShipLocalSpawnFrame = globalFrame.TransformToLocal(ref globalFrame2);
				this._shieldBreakRecords[this._numberOfPendingShieldBreakRecords].BannerTexture = texture;
				this._shieldBreakRecords[this._numberOfPendingShieldBreakRecords].LinearVelocity = vec;
				this._shieldBreakRecords[this._numberOfPendingShieldBreakRecords].PrefabName = text;
				this._numberOfPendingShieldBreakRecords++;
			}
		}

		// Token: 0x06000CEB RID: 3307 RVA: 0x00063BF0 File Offset: 0x00061DF0
		private void OnShipRamming(MissionShip rammingShip, MissionShip rammedShip, float damagePercent, bool isFirstImpact, CapsuleData capsuleData, int ramQuality)
		{
			if (isFirstImpact && rammedShip == this._ownMissionShipCached)
			{
				Vec3 linearVelocity = rammingShip.Physics.LinearVelocity;
				Vec3 vec = linearVelocity.NormalizedCopy();
				this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].AveragePosition = capsuleData.P2 + new Vec3(0f, 0f, 1f, -1f);
				ShipFloatsamManager.ImpulseRecord[] impulseRecordsToProcess = this._impulseRecordsToProcess;
				int numberOfPendingImpulseRecords = this._numberOfPendingImpulseRecords;
				Vec3 vec2 = -vec + new Vec3(0f, 0f, 1.75f, -1f);
				impulseRecordsToProcess[numberOfPendingImpulseRecords].AverageNormal = vec2.NormalizedCopy();
				this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].TotalImpulse = (float)(ramQuality + 5) * 150000f;
				this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].Speed = linearVelocity * 2f;
				this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].DebrisType = ShipFloatsamManager.DebrisType.Ramming;
				this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].DecalType = ShipFloatsamManager.DecalType.Collision;
				this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].InitialSpeedMultiplier = 1f;
				MatrixFrame bodyWorldTransform = rammedShip.GameEntity.GetBodyWorldTransform();
				ShipFloatsamManager.ImpulseRecord[] impulseRecordsToProcess2 = this._impulseRecordsToProcess;
				int numberOfPendingImpulseRecords2 = this._numberOfPendingImpulseRecords;
				vec2 = capsuleData.P2;
				impulseRecordsToProcess2[numberOfPendingImpulseRecords2].ShipLocalPosition = bodyWorldTransform.TransformToLocal(ref vec2);
				this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].ShipLocalNormal = bodyWorldTransform.rotation.TransformToLocal(ref linearVelocity);
				this._numberOfPendingImpulseRecords++;
			}
		}

		// Token: 0x06000CEC RID: 3308 RVA: 0x00063D98 File Offset: 0x00061F98
		private void OnShipHit(MissionShip ship, Agent attackerAgent, int damage, Vec3 impactPosition, Vec3 impactDirection, MissionWeapon weapon, int missileIndex)
		{
			if (!this._floatsamSystemEnabled)
			{
				return;
			}
			if (ship == this._ownMissionShipCached && weapon.CurrentUsageItem != null)
			{
				WeaponClass weaponClass = weapon.CurrentUsageItem.WeaponClass;
				if ((weaponClass == 20 || weaponClass == 19 || weaponClass == 26 || weaponClass == 27) && this._numberOfPendingImpulseRecords < 10)
				{
					MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
					Vec3 vec = -impactDirection;
					Vec3 vec2 = vec;
					if (this._bodyEntity != null)
					{
						Vec3 zero = Vec3.Zero;
						float num = 0f;
						if (this._bodyEntity.RayHitEntityWithNormal(impactPosition - impactDirection, impactDirection.NormalizedCopy(), 2f, ref zero, ref num))
						{
							vec = zero;
							vec2 = zero;
							vec.Normalize();
						}
					}
					int num2;
					if (weapon.Item.StringId.Contains("grape"))
					{
						num2 = this._collisionHitParticleIndex;
					}
					else
					{
						num2 = this._midCollisionHitParticleIndex;
					}
					MatrixFrame identity = MatrixFrame.Identity;
					identity.rotation.u = Vec3.Up;
					identity.rotation.s = vec;
					identity.rotation.f = -globalFrame.rotation.s.CrossProductWithUp();
					identity.rotation.s = Vec3.CrossProduct(globalFrame.rotation.f, globalFrame.rotation.u);
					identity.origin = impactPosition;
					base.GameEntity.Scene.CreateBurstParticle(num2, identity);
					Vec3 vec3 = Vec3.Zero;
					if (GameEntityPhysicsExtensions.HasDynamicRigidBody(base.GameEntity))
					{
						vec3 = GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(base.GameEntity, impactPosition);
					}
					float num3 = (float)damage / 150f;
					this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].AveragePosition = impactPosition;
					this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].AverageNormal = vec;
					this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].TotalImpulse = 150000f * num3;
					this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].Speed = vec3;
					this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].DebrisType = ShipFloatsamManager.DebrisType.Scrape;
					this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].DecalType = ShipFloatsamManager.DecalType.Collision;
					this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].InitialSpeedMultiplier = 1f;
					this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].ShipLocalPosition = globalFrame.TransformToLocal(ref impactPosition);
					this._impulseRecordsToProcess[this._numberOfPendingImpulseRecords].ShipLocalNormal = globalFrame.rotation.TransformToLocal(ref vec2);
					this._numberOfPendingImpulseRecords++;
				}
			}
		}

		// Token: 0x06000CED RID: 3309 RVA: 0x00064046 File Offset: 0x00062246
		public void EnableFloatsamSystem()
		{
			this._floatsamSystemEnabled = true;
		}

		// Token: 0x040007AE RID: 1966
		private static readonly string[] GenericPrefabNames = new string[] { "floatable_debris_broken_barrel", "floatable_debris_door", "floatable_debris_barrel_a" };

		// Token: 0x040007AF RID: 1967
		private static readonly string[] RammingPrefabNames = new string[] { "floatable_debris_plank_b", "floatable_debris_plank_e", "floatable_debris_plank_f", "floatable_debris_plank_g", "floatable_debris_plank_h", "floatable_debris_plank_j", "floatable_debris_plank_k" };

		// Token: 0x040007B0 RID: 1968
		private static readonly string[] ScrapeDebrisPrefabNames = new string[] { "floatable_debris_plank_b", "floatable_debris_plank_e", "floatable_debris_plank_f", "floatable_debris_plank_g", "floatable_debris_plank_h", "floatable_debris_plank_j", "floatable_debris_plank_k" };

		// Token: 0x040007B1 RID: 1969
		private static readonly string[] CollisionDecalPrefabNames = new string[] { "decal_ship_damaged_a", "decal_ship_damaged_b", "decal_ship_damaged_c" };

		// Token: 0x040007B2 RID: 1970
		private static readonly string[] ScrapeDecalPrefabNames = new string[] { "decal_ship_damage_02", "decal_ship_damage_03", "decal_ship_damage_04" };

		// Token: 0x040007B3 RID: 1971
		private const string RudderPrefabName = "floatable_debris_rudder";

		// Token: 0x040007B4 RID: 1972
		private const string ShieldPrefabName = "floatable_debris_";

		// Token: 0x040007B5 RID: 1973
		private const string OarPrefabName = "floatable_debris_oar_a";

		// Token: 0x040007B6 RID: 1974
		private const string MastPrefabName = "floatable_debris_mast";

		// Token: 0x040007B7 RID: 1975
		private const string BodyMeshTag = "body_mesh";

		// Token: 0x040007B8 RID: 1976
		private const string BannerTag = "banner_with_faction_color";

		// Token: 0x040007B9 RID: 1977
		private const int MaxNumberOfPendingImpulseRecords = 10;

		// Token: 0x040007BA RID: 1978
		private const float DebrisBreakImpulseThreshold = 150000f;

		// Token: 0x040007BB RID: 1979
		private const int MaxDecalCount = 30;

		// Token: 0x040007BC RID: 1980
		private Dictionary<WeakGameEntity, ShipFloatsamManager.ScrapeRecord> _scrapeRecords = new Dictionary<WeakGameEntity, ShipFloatsamManager.ScrapeRecord>();

		// Token: 0x040007BD RID: 1981
		private GameEntity _identityFrameParticleParent;

		// Token: 0x040007BE RID: 1982
		private int _scrapeParticleIndex = -1;

		// Token: 0x040007BF RID: 1983
		private int _collisionHitParticleIndex = -1;

		// Token: 0x040007C0 RID: 1984
		private int _midCollisionHitParticleIndex = -1;

		// Token: 0x040007C1 RID: 1985
		private int _bigCollisionHitParticleIndex = -1;

		// Token: 0x040007C2 RID: 1986
		private readonly MBFastRandom _randomGenerator = new MBFastRandom();

		// Token: 0x040007C3 RID: 1987
		private ShipFloatsamManager.ImpulseRecord[] _impulseRecordsToProcess = new ShipFloatsamManager.ImpulseRecord[10];

		// Token: 0x040007C4 RID: 1988
		private ShipFloatsamManager.ShieldBreakRecord[] _shieldBreakRecords = new ShipFloatsamManager.ShieldBreakRecord[10];

		// Token: 0x040007C5 RID: 1989
		private uint _shipColor;

		// Token: 0x040007C6 RID: 1990
		private int _numberOfPendingImpulseRecords;

		// Token: 0x040007C7 RID: 1991
		private int _numberOfPendingShieldBreakRecords;

		// Token: 0x040007C8 RID: 1992
		private uint _shipDecalColor;

		// Token: 0x040007C9 RID: 1993
		private bool _sinkingFloatsamSpawned;

		// Token: 0x040007CA RID: 1994
		private List<GameEntity> _collisionDecals;

		// Token: 0x040007CB RID: 1995
		private string _shieldName;

		// Token: 0x040007CC RID: 1996
		private NavalFloatsamLogic _floatsamMissionLogic;

		// Token: 0x040007CD RID: 1997
		private GameEntity _bodyEntity;

		// Token: 0x040007CE RID: 1998
		private MissionShip _ownMissionShipCached;

		// Token: 0x040007CF RID: 1999
		private bool _floatsamSystemEnabled;

		// Token: 0x0200022C RID: 556
		private enum DebrisType
		{
			// Token: 0x04000F4D RID: 3917
			Generic,
			// Token: 0x04000F4E RID: 3918
			Scrape,
			// Token: 0x04000F4F RID: 3919
			Ramming
		}

		// Token: 0x0200022D RID: 557
		private enum DecalType
		{
			// Token: 0x04000F51 RID: 3921
			Collision,
			// Token: 0x04000F52 RID: 3922
			Scrape
		}

		// Token: 0x0200022E RID: 558
		private struct ImpulseRecord
		{
			// Token: 0x04000F53 RID: 3923
			internal Vec3 AveragePosition;

			// Token: 0x04000F54 RID: 3924
			internal Vec3 AverageNormal;

			// Token: 0x04000F55 RID: 3925
			internal float TotalImpulse;

			// Token: 0x04000F56 RID: 3926
			internal Vec3 Speed;

			// Token: 0x04000F57 RID: 3927
			internal ShipFloatsamManager.DebrisType DebrisType;

			// Token: 0x04000F58 RID: 3928
			internal float InitialSpeedMultiplier;

			// Token: 0x04000F59 RID: 3929
			internal Vec3 ShipLocalPosition;

			// Token: 0x04000F5A RID: 3930
			internal Vec3 ShipLocalNormal;

			// Token: 0x04000F5B RID: 3931
			internal ShipFloatsamManager.DecalType DecalType;
		}

		// Token: 0x0200022F RID: 559
		private struct ShieldBreakRecord
		{
			// Token: 0x04000F5C RID: 3932
			internal Vec3 LinearVelocity;

			// Token: 0x04000F5D RID: 3933
			internal Texture BannerTexture;

			// Token: 0x04000F5E RID: 3934
			internal MatrixFrame ShipLocalSpawnFrame;

			// Token: 0x04000F5F RID: 3935
			internal string PrefabName;
		}

		// Token: 0x02000230 RID: 560
		private class ScrapeRecord
		{
			// Token: 0x04000F60 RID: 3936
			internal ParticleSystem Particle;

			// Token: 0x04000F61 RID: 3937
			internal float AccumulatedDistance;

			// Token: 0x04000F62 RID: 3938
			internal Vec3 PreviousPosition;
		}
	}
}
