using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions.NavalPhysics;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.Missions.Objects
{
	// Token: 0x020000AB RID: 171
	[ScriptComponentParams("ship_visual_only", "ship_water_effects")]
	public class ShipWaterEffects : ScriptComponentBehavior
	{
		// Token: 0x06000D0F RID: 3343 RVA: 0x00064C44 File Offset: 0x00062E44
		public void DummyFunc()
		{
			Debug.Print(this._showWaterSimulationBoundingBox.ToString(), 0, 12, 17592186044416UL);
			Debug.Print(this._movementParticleHeightOffset.ToString(), 0, 12, 17592186044416UL);
			Debug.Print(this._splashParticleHeightOffset.ToString(), 0, 12, 17592186044416UL);
			Debug.Print(this._showMovementParticles.ToString(), 0, 12, 17592186044416UL);
			Debug.Print(this._showSplashParticles.ToString(), 0, 12, 17592186044416UL);
			Debug.Print(this._showHullWaterDebugPanel.ToString(), 0, 12, 17592186044416UL);
			Debug.Print(this._hullWaterResScale.ToString(), 0, 12, 17592186044416UL);
			Debug.Print(this._showWaterBalancePlane.ToString(), 0, 12, 17592186044416UL);
			Debug.Print(this._movementParticleSideSpeedVector.ToString(), 0, 12, 17592186044416UL);
			Debug.Print(this._showWetnessDecalValues.ToString(), 0, 12, 17592186044416UL);
			Debug.Print(this._forceWetnessDecalsToFull.ToString(), 0, 12, 17592186044416UL);
		}

		// Token: 0x06000D10 RID: 3344 RVA: 0x00064D8B File Offset: 0x00062F8B
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 6;
		}

		// Token: 0x06000D11 RID: 3345 RVA: 0x00064D90 File Offset: 0x00062F90
		protected override void OnInit()
		{
			base.OnInit();
			this._showMovementParticles = false;
			this._showSplashParticles = false;
			this._movementParticleEntity = GameEntity.CreateFromWeakEntity(base.GameEntity.GetFirstChildEntityWithTag("movement_particles"));
			this._splashParticleEntity = GameEntity.CreateFromWeakEntity(base.GameEntity.GetFirstChildEntityWithTag("splash_particles"));
			if (this._splashParticleEntity != null)
			{
				foreach (GameEntity gameEntity in this._splashParticleEntity.GetChildren().ToList<GameEntity>())
				{
					gameEntity.Remove(23);
				}
			}
			if (this._movementParticleEntity != null)
			{
				foreach (GameEntity gameEntity2 in this._movementParticleEntity.GetChildren().ToList<GameEntity>())
				{
					gameEntity2.Remove(23);
				}
			}
			this._inCampaignMode = base.GameEntity.Scene.GetName() == "Main_map";
			this._ownerSceneCached = base.GameEntity.Scene;
			this.FetchEntities();
			if (!this._inCampaignMode)
			{
				if (this._wakeAndParticlesEnabled)
				{
					float num = 0f;
					NavalPhysics firstScriptOfType = base.GameEntity.Root.GetFirstScriptOfType<NavalPhysics>();
					if (firstScriptOfType != null)
					{
						num = firstScriptOfType.StabilitySubmergedHeightOfShip;
					}
					this.PlaceParticles(ShipWaterEffects.ParticleType.Splash, num + this._splashParticleHeightOffset);
					this.PlaceParticles(ShipWaterEffects.ParticleType.Movement, num + this._movementParticleHeightOffset);
					if (this._waterVisualRecord == UIntPtr.Zero)
					{
						this.CheckWaterVisualRegistry();
					}
				}
				this._largeSplashParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_naval_ship_water_splash_large");
				this._mediumSplashParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_naval_ship_water_splash_mid");
				this._smallSplashParticleIndex = ParticleSystemManager.GetRuntimeIdByName("psys_naval_ship_water_splash_small");
				if (this._ownerSceneCached.HasDecalRenderer())
				{
					for (int i = 0; i < 50; i++)
					{
						this._splashFoamDecals[i] = new ShipWaterEffects.SplashFoamDecal();
					}
				}
				WeakGameEntity parent = base.GameEntity.Parent;
				this._wetnessDecals.Clear();
				MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
				if (parent != null && parent.Scene != null)
				{
					foreach (WeakGameEntity weakGameEntity in parent.GetFirstChildEntityWithTag("wetness_decals").GetChildren())
					{
						Decal decal = weakGameEntity.GetComponentAtIndex(0, 7) as Decal;
						if (decal != null)
						{
							ShipWaterEffects.WetnessDecalData wetnessDecalData = new ShipWaterEffects.WetnessDecalData();
							wetnessDecalData.Decal = decal;
							decal.CheckAndRegisterToDecalSet();
							wetnessDecalData.CurrentAlpha = 0f;
							ShipWaterEffects.WetnessDecalData wetnessDecalData2 = wetnessDecalData;
							MatrixFrame matrixFrame = weakGameEntity.GetLocalFrame();
							Vec3 vec = matrixFrame.rotation.u.NormalizedCopy();
							wetnessDecalData2.Normal = globalFrame.rotation.TransformToLocal(ref vec);
							ShipWaterEffects.WetnessDecalData wetnessDecalData3 = wetnessDecalData;
							matrixFrame = weakGameEntity.GetGlobalFrame();
							wetnessDecalData3.LocalPosition = globalFrame.TransformToLocalNonOrthogonal(ref matrixFrame.origin);
							this._wetnessDecals.Add(wetnessDecalData);
						}
					}
				}
			}
			this.ComputeWakeCapsuleParameters();
			this._previousShipFrame = base.GameEntity.Root.GetGlobalFrame();
		}

		// Token: 0x06000D12 RID: 3346 RVA: 0x00065108 File Offset: 0x00063308
		protected override void OnTick(float dt)
		{
			if (this._inCampaignMode)
			{
				return;
			}
			if (this._waterVisualRecord == UIntPtr.Zero && this._wakeAndParticlesEnabled)
			{
				this.CheckWaterVisualRegistry();
				this.ComputeWakeCapsuleParameters();
			}
		}

		// Token: 0x06000D13 RID: 3347 RVA: 0x00065139 File Offset: 0x00063339
		protected override void OnTickParallel(float dt)
		{
			this.OnMissionTick(dt);
		}

		// Token: 0x06000D14 RID: 3348 RVA: 0x00065144 File Offset: 0x00063344
		protected override void OnRemoved(int removeReason)
		{
			if (this._waterVisualRecord != UIntPtr.Zero)
			{
				base.GameEntity.Scene.DeRegisterShipVisual(this._waterVisualRecord);
			}
			if (this._ownerSceneCached != null)
			{
				if (this._ownerSceneCached.HasDecalRenderer())
				{
					foreach (ShipWaterEffects.SplashFoamDecal splashFoamDecal in this._splashFoamDecals)
					{
						if (splashFoamDecal != null && splashFoamDecal._splashFoamDecal != null)
						{
							this._ownerSceneCached.RemoveDecalInstance(splashFoamDecal._splashFoamDecal, "editor_set");
						}
					}
				}
				if (this._ownerSceneCached != null)
				{
					this._ownerSceneCached.ManualInvalidate();
					this._ownerSceneCached = null;
				}
			}
		}

		// Token: 0x06000D15 RID: 3349 RVA: 0x000651F8 File Offset: 0x000633F8
		private void OnMissionTick(float dt)
		{
			if (this._waterVisualRecord == UIntPtr.Zero)
			{
				return;
			}
			this._cumulativeDt += dt;
			if (!this._inCampaignMode)
			{
				if (this._wakeAndParticlesEnabled)
				{
					this.SnapMovementParticlePositionsToWater(dt);
					if (dt > 1E-06f)
					{
						this.CheckAndSpawnSplashes(dt);
					}
				}
				this.TickHullWater(dt, false);
				this.HandleWetnessDecals(dt);
				if (this._ownerSceneCached.HasDecalRenderer())
				{
					this.HandleSplashFoamDecals(dt);
				}
			}
			this._previousShipFrame = base.GameEntity.Root.GetGlobalFrame();
		}

		// Token: 0x06000D16 RID: 3350 RVA: 0x0006528B File Offset: 0x0006348B
		private GameEntity GetParticleParentEntity(ShipWaterEffects.ParticleType particleType)
		{
			if (particleType == ShipWaterEffects.ParticleType.Splash)
			{
				return this._splashParticleEntity;
			}
			if (particleType == ShipWaterEffects.ParticleType.Movement)
			{
				return this._movementParticleEntity;
			}
			return null;
		}

		// Token: 0x06000D17 RID: 3351 RVA: 0x000652A3 File Offset: 0x000634A3
		private List<ShipWaterEffects.ParticleData> GetParticleDataList(ShipWaterEffects.ParticleType particleType)
		{
			if (particleType == ShipWaterEffects.ParticleType.Splash)
			{
				return this._splashParticles;
			}
			if (particleType == ShipWaterEffects.ParticleType.Movement)
			{
				return this._movementParticles;
			}
			return null;
		}

		// Token: 0x06000D18 RID: 3352 RVA: 0x000652BC File Offset: 0x000634BC
		private ParticleSystem CreateMovementParticle(GameEntity parentEntity, MatrixFrame localFrame)
		{
			switch (this._movementParticleType)
			{
			case ShipWaterEffects.MovementParticleType.Small:
				return ParticleSystem.CreateParticleSystemAttachedToEntity("psys_naval_ship_emit_on_move_small", parentEntity, ref localFrame);
			case ShipWaterEffects.MovementParticleType.Medium:
				return ParticleSystem.CreateParticleSystemAttachedToEntity("psys_naval_ship_emit_on_move_mid", parentEntity, ref localFrame);
			case ShipWaterEffects.MovementParticleType.Large:
				return ParticleSystem.CreateParticleSystemAttachedToEntity("psys_naval_ship_emit_on_move_large", parentEntity, ref localFrame);
			default:
				return null;
			}
		}

		// Token: 0x06000D19 RID: 3353 RVA: 0x00065310 File Offset: 0x00063510
		private void RecomputeWaterSimulationBoundingBox()
		{
			List<WeakGameEntity> list = new List<WeakGameEntity>();
			base.GameEntity.Root.GetChildrenWithTagRecursive(list, "render_to_depth");
			BoundingBox boundingBox = default(BoundingBox);
			boundingBox.RecomputeRadius();
			MatrixFrame globalFrame = base.GameEntity.Root.GetGlobalFrame();
			foreach (WeakGameEntity weakGameEntity in list)
			{
				BoundingBox localBoundingBox = weakGameEntity.GetLocalBoundingBox();
				BoundingBox boundingBox2 = localBoundingBox;
				MatrixFrame globalFrame2 = weakGameEntity.GetGlobalFrame();
				boundingBox.RelaxWithChildBoundingBox(boundingBox2, globalFrame.TransformToLocalNonOrthogonal(ref globalFrame2));
			}
			float num = MathF.Max(boundingBox.max.x, boundingBox.min.x);
			float num2 = MathF.Max(boundingBox.max.y, boundingBox.min.y);
			float num3 = MathF.Max(boundingBox.max.z, boundingBox.min.z);
			float num4 = 1f;
			switch (this._hullWaterResScale)
			{
			case ShipWaterEffects.ResolutionScale.half:
				num4 = 0.5f;
				break;
			case ShipWaterEffects.ResolutionScale.quarter:
				num4 = 0.25f;
				break;
			case ShipWaterEffects.ResolutionScale.one_eight:
				num4 = 0.125f;
				break;
			case ShipWaterEffects.ResolutionScale.one_sixteenth:
				num4 = 0.0625f;
				break;
			}
			this._waterSimulationBoundingBox = new Vec3(num, num2, num3, -1f) * 2f;
			base.GameEntity.ChangeResolutionMultiplierOfWaterVisual(this._waterVisualRecord, num4, ref this._waterSimulationBoundingBox);
			base.GameEntity.RefreshMeshesToRenderToHullWater(this._waterVisualRecord, "render_to_depth");
		}

		// Token: 0x06000D1A RID: 3354 RVA: 0x000654C4 File Offset: 0x000636C4
		private void FetchEntities()
		{
			this._movementParticleEntity = GameEntity.CreateFromWeakEntity(base.GameEntity.GetFirstChildEntityWithTag("movement_particles"));
			this._splashParticleEntity = GameEntity.CreateFromWeakEntity(base.GameEntity.GetFirstChildEntityWithTag("splash_particles"));
			if (this._movementParticleEntity != null)
			{
				this._movementParticleEntity.EntityFlags |= 131072;
			}
			else
			{
				this._movementParticleEntity = GameEntity.CreateEmpty(base.GameEntity.Scene, true, true, true);
				this._movementParticleEntity.Name = "movement_parent";
				this._movementParticleEntity.AddTag("movement_particles");
				base.GameEntity.AddChild(this._movementParticleEntity.WeakEntity, false);
				MatrixFrame identity = MatrixFrame.Identity;
				this._movementParticleEntity.SetFrame(ref identity, true);
			}
			if (this._splashParticleEntity != null)
			{
				this._splashParticleEntity.EntityFlags |= 131072;
			}
			else
			{
				this._splashParticleEntity = GameEntity.CreateEmpty(base.GameEntity.Scene, true, true, true);
				this._splashParticleEntity.Name = "movement_parent";
				this._splashParticleEntity.AddTag("splash_particles");
				base.GameEntity.AddChild(this._splashParticleEntity.WeakEntity, false);
				MatrixFrame identity2 = MatrixFrame.Identity;
				this._splashParticleEntity.SetFrame(ref identity2, true);
			}
			MatrixFrame identity3 = MatrixFrame.Identity;
			this._movementParticleEntity.SetLocalFrame(ref identity3, true);
			this._splashParticleEntity.SetLocalFrame(ref identity3, true);
		}

		// Token: 0x06000D1B RID: 3355 RVA: 0x00065654 File Offset: 0x00063854
		private void ComputeWakeCapsuleParameters()
		{
			if (this._waterVisualRecord == UIntPtr.Zero)
			{
				return;
			}
			WeakGameEntity firstChildEntityWithTagRecursive = base.GameEntity.Root.GetFirstChildEntityWithTagRecursive("body_mesh");
			if (!firstChildEntityWithTagRecursive.IsValid)
			{
				return;
			}
			MatrixFrame globalFrame = base.GameEntity.Root.GetGlobalFrame();
			firstChildEntityWithTagRecursive.ValidateBoundingBox();
			BoundingBox globalBoundingBox = firstChildEntityWithTagRecursive.GetGlobalBoundingBox();
			this._bodyBB = firstChildEntityWithTagRecursive.GetLocalBoundingBox();
			float num = globalBoundingBox.radius + 1f;
			Vec3 center = globalBoundingBox.center;
			center.z = MBMath.Lerp(center.z, globalBoundingBox.min.z, 0.5f, 1E-05f);
			Vec3 vec = -globalFrame.rotation.f;
			Vec3 f = globalFrame.rotation.f;
			Vec3 s = globalFrame.rotation.s;
			Vec3 vec2 = -globalFrame.rotation.s;
			Vec3 vec3 = center - vec * num;
			Vec3 vec4 = center - f * num;
			Vec3 vec5 = center - s * num;
			Vec3 vec6 = center - vec2 * num;
			float num2 = 0f;
			float num3 = 0f;
			bool flag = firstChildEntityWithTagRecursive.RayHitEntity(vec3, vec, num * 2f, ref num2);
			bool flag2 = firstChildEntityWithTagRecursive.RayHitEntity(vec4, f, num * 2f, ref num3);
			float num4 = 0f;
			float num5 = 0f;
			bool flag3 = firstChildEntityWithTagRecursive.RayHitEntity(vec5, s, num * 2f, ref num4);
			bool flag4 = firstChildEntityWithTagRecursive.RayHitEntity(vec6, vec2, num * 2f, ref num5);
			if (flag && flag2 && flag3 && flag4)
			{
				float num6 = center.Distance(vec3 + vec * (num2 + 4.5f));
				float num7 = center.Distance(vec4 + f * num3);
				float num8 = center.Distance(vec5 + s * num4);
				float num9 = center.Distance(vec6 + vec2 * num5);
				base.GameEntity.SetVisualRecordWakeParams(this._waterVisualRecord, new Vec3(num6, num7, num8, num9));
			}
		}

		// Token: 0x06000D1C RID: 3356 RVA: 0x00065894 File Offset: 0x00063A94
		private bool RayCastToEntities(List<WeakGameEntity> rayCastEntities, Vec3 rayStart, Vec3 rayDirection, float maxLength, ref float resultLength, ref Vec3 surfaceNormal)
		{
			bool flag = false;
			resultLength = maxLength;
			foreach (WeakGameEntity weakGameEntity in rayCastEntities)
			{
				float num = maxLength;
				if (weakGameEntity.RayHitEntityWithNormal(rayStart, rayDirection, maxLength, ref surfaceNormal, ref num) && num < resultLength)
				{
					flag = true;
					resultLength = num;
				}
			}
			return flag;
		}

		// Token: 0x06000D1D RID: 3357 RVA: 0x00065904 File Offset: 0x00063B04
		private void PlaceParticles(ShipWaterEffects.ParticleType particleType, float waterLineHeight)
		{
			GameEntity particleParentEntity = this.GetParticleParentEntity(particleType);
			if (particleParentEntity == null)
			{
				return;
			}
			MatrixFrame globalFrame = particleParentEntity.GetGlobalFrame();
			List<ShipWaterEffects.ParticleData> particleDataList = this.GetParticleDataList(particleType);
			foreach (ShipWaterEffects.ParticleData particleData in particleDataList)
			{
				if (particleData.MovementParticleSystem != null)
				{
					particleParentEntity.RemoveComponent(particleData.MovementParticleSystem);
				}
			}
			particleDataList.Clear();
			WeakGameEntity root = base.GameEntity.Root;
			WeakGameEntity firstChildEntityWithTagRecursive = root.GetFirstChildEntityWithTagRecursive("body_mesh");
			if (!firstChildEntityWithTagRecursive.IsValid)
			{
				return;
			}
			MatrixFrame globalFrame2 = root.GetGlobalFrame();
			BoundingBox boundingBox = (firstChildEntityWithTagRecursive.GetComponentAtIndex(0, 0) as MetaMesh).GetBoundingBox();
			float radius = boundingBox.radius;
			Vec3 center = boundingBox.center;
			center.z = waterLineHeight;
			Vec3 vec = boundingBox.max - boundingBox.min;
			Vec3 vec2 = vec;
			float num = MathF.Min(MathF.Min(vec2.x, vec2.y), vec2.z);
			if (num > 0f)
			{
				vec2 /= num;
			}
			vec2 = Vec3.Lerp(vec2, Vec3.One, 0.5f);
			float num2 = ((particleType == ShipWaterEffects.ParticleType.Splash) ? this._splashParticleSurfaceDistanceOffset : this._movementParticleSurfaceDistanceOffset);
			List<WeakGameEntity> list = new List<WeakGameEntity>();
			list.Add(firstChildEntityWithTagRecursive);
			float num3 = 0f;
			WeakGameEntity parent = base.GameEntity.Parent;
			if (parent != null)
			{
				foreach (WeakGameEntity weakGameEntity in parent.GetChildren())
				{
					if (weakGameEntity.ChildCount > 0)
					{
						WeakGameEntity child = weakGameEntity.GetChild(0);
						if (child.HasTag("bow"))
						{
							using (IEnumerator<WeakGameEntity> enumerator3 = child.GetChildren().GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									WeakGameEntity weakGameEntity2 = enumerator3.Current;
									if (weakGameEntity2.IsVisibleIncludeParents())
									{
										MissionShipRam firstScriptOfType = weakGameEntity2.GetFirstScriptOfType<MissionShipRam>();
										if (firstScriptOfType != null)
										{
											num3 = MathF.Max(firstScriptOfType.RamLength, num3);
										}
									}
								}
								break;
							}
						}
					}
				}
			}
			float num4 = 0f;
			int num5 = 5;
			for (int i = 0; i < num5; i++)
			{
				float num6 = 0f;
				Vec3 zero = Vec3.Zero;
				Vec3 vec3 = new Vec3(0f, 1f, 0f, -1f) * vec.y;
				vec3.z = waterLineHeight - 0.5f + (float)i * 0.2f;
				Vec3 vec4;
				vec4..ctor(0f, -1f, 0f, -1f);
				vec3 = globalFrame2.TransformToParent(ref vec3);
				vec4 = globalFrame2.rotation.TransformToParent(ref vec4);
				vec4.Normalize();
				if (this.RayCastToEntities(list, vec3, vec4, radius * 8f, ref num6, ref zero))
				{
					num4 = MathF.Max(num4, vec.y - num6);
				}
			}
			num4 += num3;
			float num7 = 0f;
			int num8 = 5;
			for (int j = 0; j < num8; j++)
			{
				float num9 = 0f;
				Vec3 zero2 = Vec3.Zero;
				Vec3 vec5 = new Vec3(0f, -1f, 0f, -1f) * vec.y;
				vec5.z = waterLineHeight - 0.5f + (float)j * 0.2f;
				Vec3 vec6;
				vec6..ctor(0f, 1f, 0f, -1f);
				vec5 = globalFrame2.TransformToParent(ref vec5);
				vec6 = globalFrame2.rotation.TransformToParent(ref vec6);
				vec6.Normalize();
				if (this.RayCastToEntities(list, vec5, vec6, radius * 8f, ref num9, ref zero2))
				{
					num7 = MathF.Max(num7, vec.y - num9);
				}
			}
			float num10 = num4 + num7;
			float num11 = 1f;
			int num12 = (int)(vec.y / 5.5f);
			int num13;
			if (particleType == ShipWaterEffects.ParticleType.Movement)
			{
				num13 = num12 * 2 + 1;
			}
			else
			{
				float num14 = num10 - 3f;
				num13 = (int)(num14 / num11);
				num11 = num14 / (float)num13;
				num13 *= 2;
			}
			int num15 = num13 / 2;
			int num16 = 0;
			int num17 = 0;
			for (int k = 0; k < num13; k++)
			{
				bool flag = false;
				bool flag2 = false;
				Vec3 vec7;
				vec7..ctor(0f, 0f, 0f, -1f);
				Vec3 vec8;
				if (particleType == ShipWaterEffects.ParticleType.Splash)
				{
					float num18 = ((k >= num15) ? (-1f) : 1f);
					int num19 = k % num15;
					float num20 = num4 - 1.5f - (float)num19 * num11;
					vec7.x = vec.x * 2f * num18;
					vec7.y = num20;
					vec7.z = center.z;
					vec8..ctor(-num18, 0f, 0f, -1f);
				}
				else if (k == 0)
				{
					vec7.x = 0f;
					vec7.y = num4 + 4f;
					vec7.z = center.z;
					vec8..ctor(0f, -1f, 0f, -1f);
				}
				else
				{
					float num21 = ((k - 1 >= num12) ? (-1f) : 1f);
					int num22 = (k - 1) % num12;
					float num23 = num4 - (0.7f + (float)num22) * 2.05f;
					vec7.x = vec.x * 2f * num21;
					vec7.y = num23;
					vec7.z = center.z;
					vec8..ctor(-num21, 0f, 0f, -1f);
					flag = num22 == num16 && num21 == -1f;
					flag2 = num22 == num17 && num21 == 1f;
				}
				Vec3 vec9 = vec7;
				vec7 = globalFrame2.TransformToParent(ref vec7);
				vec8 = globalFrame2.rotation.TransformToParent(ref vec8);
				vec8.Normalize();
				float num24 = 0f;
				Vec3 zero3 = Vec3.Zero;
				int num25 = 5;
				bool flag3 = false;
				while (!flag3 && num25 > 0)
				{
					flag3 = this.RayCastToEntities(list, vec7, vec8, radius * 8f, ref num24, ref zero3);
					if (!flag3)
					{
						vec7.z += 0.05f;
					}
					num25--;
				}
				if (flag3)
				{
					Vec3 vec10 = -vec8;
					if (particleType == ShipWaterEffects.ParticleType.Movement && k == 0)
					{
						num24 -= num3;
					}
					vec10.z = 0f;
					vec10.Normalize();
					MatrixFrame identity = MatrixFrame.Identity;
					identity.origin = vec7 + num24 * vec8 + vec10 * num2;
					identity.rotation.s = vec10;
					identity.rotation.u = Vec3.Up;
					identity.rotation.f = -identity.rotation.s.CrossProductWithUp();
					ShipWaterEffects.ParticleData particleData2 = new ShipWaterEffects.ParticleData();
					particleData2.LocalFrame = globalFrame.TransformToLocalNonOrthogonal(ref identity);
					particleData2.SurfaceNormal = globalFrame.rotation.TransformToLocal(ref zero3);
					if (particleType == ShipWaterEffects.ParticleType.Movement)
					{
						particleData2.MovementParticleSystem = this.CreateMovementParticle(particleParentEntity, particleData2.LocalFrame);
					}
					particleData2.LastSpawnTime = 0f;
					if (flag)
					{
						this._leftDecalParticleIndex = particleDataList.Count;
					}
					if (flag2)
					{
						this._rightDecalParticleIndex = particleDataList.Count;
					}
					particleData2.PerSlicePositions = new List<KeyValuePair<float, ShipWaterEffects.SliceSampleData>>();
					for (float num26 = boundingBox.min.z; num26 < boundingBox.max.z; num26 += 0.25f)
					{
						Vec3 vec11 = vec9;
						vec11.z = num26;
						vec11 = globalFrame2.TransformToParent(ref vec11);
						Vec3 zero4 = Vec3.Zero;
						float num27 = 0f;
						if (this.RayCastToEntities(list, vec11, vec8, radius * 8f, ref num27, ref zero4))
						{
							Vec3 vec12 = vec11 + num27 * vec8 + zero4 * num2;
							Vec3 vec13 = globalFrame.TransformToLocalNonOrthogonal(ref vec12);
							Vec3 vec14 = Vec3.Up;
							Vec3 vec15 = Vec3.Zero;
							float num28 = 0f;
							if (firstChildEntityWithTagRecursive.RayHitEntity(vec12, vec14, 8f, ref num28))
							{
								Vec3 vec16 = (Vec3.Up + vec10) * 0.5f;
								vec15 = vec10;
								Vec3 vec17 = vec14;
								vec14 = vec16;
								do
								{
									float num29 = 0f;
									Vec3 zero5 = Vec3.Zero;
									if (!this.RayCastToEntities(list, vec12, vec14, 8f, ref num29, ref zero5))
									{
										Vec3 vec18 = (vec14 + vec17) * 0.5f;
										vec15 = vec14;
										vec14 = vec18;
									}
									else
									{
										Vec3 vec19 = (vec14 + vec15) * 0.5f;
										vec17 = vec14;
										vec14 = vec19;
									}
								}
								while (MathF.Abs(MathF.Asin(Vec3.CrossProduct(vec17, vec14).Length)) >= 0.05235988f);
							}
							Vec3 vec20 = Vec3.CrossProduct(vec10, vec14);
							vec20.Normalize();
							vec14 = vec14.RotateAboutAnArbitraryVector(vec20, -0.34906584f);
							Vec3 vec21 = globalFrame.rotation.TransformToLocal(ref vec14);
							ShipWaterEffects.SliceSampleData sliceSampleData = default(ShipWaterEffects.SliceSampleData);
							sliceSampleData.localPosition = vec13;
							sliceSampleData.limitingUpVector = vec21;
							particleData2.PerSlicePositions.Add(new KeyValuePair<float, ShipWaterEffects.SliceSampleData>(num26, sliceSampleData));
						}
					}
					particleDataList.Add(particleData2);
				}
				else
				{
					if (flag)
					{
						num16++;
					}
					if (flag2)
					{
						num17++;
					}
				}
			}
			if (particleType == ShipWaterEffects.ParticleType.Movement && this._movementParticles.Count > 0)
			{
				this._lastDecalLeftSpawnPosition = globalFrame.TransformToParent(ref this._movementParticles[0].LocalFrame.origin);
				this._lastDecalRightSpawnPosition = this._lastDecalLeftSpawnPosition;
				this._previousShipFrameForDecalSpawn = base.GameEntity.GetGlobalFrame().origin;
			}
		}

		// Token: 0x06000D1E RID: 3358 RVA: 0x000662EC File Offset: 0x000644EC
		private float GetFloaterForceMultiplier()
		{
			if (MBObjectManager.Instance != null)
			{
				MBReadOnlyList<MissionShipObject> objects = MBObjectManager.Instance.GetObjects<MissionShipObject>((MissionShipObject x) => x.Prefab == base.GameEntity.Root.Name);
				if (objects.Count > 0)
				{
					return objects[0].FloatingForceMultiplier;
				}
			}
			return 1f;
		}

		// Token: 0x06000D1F RID: 3359 RVA: 0x00066334 File Offset: 0x00064534
		private float CalculateWaterBalancePoint()
		{
			WeakGameEntity root = base.GameEntity.Root;
			MatrixFrame globalFrame = root.GetGlobalFrame();
			WeakGameEntity firstChildEntityWithName = MBExtensions.GetFirstChildEntityWithName(root, "floater_volume_holder");
			if (!firstChildEntityWithName.IsValid)
			{
				return 0f;
			}
			float floaterForceMultiplier = this.GetFloaterForceMultiplier();
			List<ShipWaterEffects.FloaterData> list = new List<ShipWaterEffects.FloaterData>();
			float num = 1000f;
			float num2 = -1000f;
			foreach (WeakGameEntity weakGameEntity in firstChildEntityWithName.GetChildren())
			{
				MatrixFrame globalFrame2 = weakGameEntity.GetGlobalFrame();
				MatrixFrame frame = weakGameEntity.GetFrame();
				Vec3 vec = globalFrame.TransformToLocalNonUnit(ref globalFrame2.origin);
				Vec3 scaleVector = frame.rotation.GetScaleVector();
				ShipWaterEffects.FloaterData floaterData = new ShipWaterEffects.FloaterData
				{
					HeightMin = vec.z,
					VerticalLength = scaleVector.z,
					HorizontalArea = scaleVector.x * scaleVector.y
				};
				list.Add(floaterData);
				num = MathF.Min(num, floaterData.HeightMin);
				num2 = MathF.Max(num2, floaterData.HeightMin + floaterData.VerticalLength);
			}
			float num3 = root.Mass * 9.806f;
			float num4 = 0.01f;
			float num5 = num;
			while (num2 > num5)
			{
				float num6 = 0f;
				foreach (ShipWaterEffects.FloaterData floaterData2 in list)
				{
					if (num5 > floaterData2.HeightMin)
					{
						float num7 = MathF.Min(num5 - floaterData2.HeightMin, floaterData2.VerticalLength) * floaterData2.HorizontalArea * 1020f * 9.806f * floaterForceMultiplier;
						num6 += num7;
					}
				}
				if (num6 > num3)
				{
					break;
				}
				num5 += num4;
			}
			return num5;
		}

		// Token: 0x06000D20 RID: 3360 RVA: 0x0006652C File Offset: 0x0006472C
		private void CheckAndSpawnSplashes(float dt)
		{
			base.GameEntity.GetGlobalWindVelocityOfScene().Normalize();
			base.GameEntity.Root.GetGlobalFrame();
			GameEntity particleParentEntity = this.GetParticleParentEntity(ShipWaterEffects.ParticleType.Splash);
			MatrixFrame globalFrame = particleParentEntity.GetGlobalFrame();
			Vec3 origin = SoundManager.GetListenerFrame().origin;
			foreach (ShipWaterEffects.ParticleData particleData in this._splashParticles)
			{
				if (particleData.SplashTimer > 0.001f)
				{
					particleData.SplashTimer -= dt;
				}
				else
				{
					particleData.SplashTimer -= dt;
					if (particleData.CurrentSplashParticle != null)
					{
						if (!particleData.CurrentSplashParticle.HasAliveParticles())
						{
							if (particleData.CurrentSplashParticle.GetEntity() == particleParentEntity)
							{
								particleParentEntity.RemoveComponent(particleData.CurrentSplashParticle);
							}
							particleData.CurrentSplashParticle = null;
						}
					}
					else
					{
						MatrixFrame localFrame = particleData.LocalFrame;
						Vec3 vec = Vec3.Zero;
						Vec3 vec2 = Vec3.Zero;
						Vec3 zero = Vec3.Zero;
						Vec3 vec3 = globalFrame.TransformToParent(ref particleData.LocalFrame.origin);
						float waterLevelAtPosition = base.GameEntity.GetWaterLevelAtPosition(vec3.AsVec2, true, false);
						vec3.z = waterLevelAtPosition;
						bool flag = false;
						vec = this.GetHeightCorrectedPosForSlice(particleData, globalFrame.TransformToLocalNonOrthogonal(ref vec3).z, ref flag, ref zero);
						if (flag)
						{
							vec2 = globalFrame.TransformToParent(ref vec);
							Vec3 linearVelocityAtGlobalPointForEntityWithDynamicBody = GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(base.GameEntity.Root, vec2);
							Vec3 waterSpeedAtPosition = this._ownerSceneCached.GetWaterSpeedAtPosition(vec.AsVec2, true);
							Vec3 vec4 = (particleData.SurfaceNormal + particleData.LocalFrame.rotation.s) * 0.5f;
							Vec3 vec5 = globalFrame.rotation.TransformToParent(ref vec4);
							Vec3 vec6 = linearVelocityAtGlobalPointForEntityWithDynamicBody - waterSpeedAtPosition;
							float num = MathF.Max(-vec6.z, 0f);
							float num2 = MathF.Max(Vec3.DotProduct(vec5, vec6), 0f);
							float num3 = num + num2;
							particleData.WasAboveWater = false;
							bool flag2 = false;
							int num4;
							float num5;
							if (num3 > 8f)
							{
								num4 = this._largeSplashParticleIndex;
								num5 = 3f;
							}
							else if (num3 > 5f)
							{
								num4 = this._mediumSplashParticleIndex;
								num5 = 2f;
							}
							else
							{
								if (num3 <= 2f)
								{
									continue;
								}
								num4 = this._smallSplashParticleIndex;
								num5 = 1f;
								flag2 = num3 > 4f;
							}
							MatrixFrame localFrame2 = particleData.LocalFrame;
							localFrame2.origin = vec;
							ParticleSystem particleSystem = ParticleSystem.CreateParticleSystemAttachedToEntity(num4, particleParentEntity, ref localFrame2);
							particleSystem.SetDontRemoveFromEntity(true);
							particleData.CurrentSplashParticle = particleSystem;
							particleData.LastSpawnTime = this._cumulativeDt;
							particleData.SplashPosition = particleData.PerSlicePositions[particleData.PerSlicePositions.Count - 1].Value.localPosition;
							particleData.SplashVelocity = -particleData.LocalFrame.rotation.s;
							particleData.SplashVelocity.Normalize();
							particleData.SplashVelocity *= (0.75f + this._splashRandom.NextFloat() * 0.5f) * 0.6f;
							particleData.SplashPosition -= particleData.LocalFrame.rotation.s * this._hullWaterSplashPointInitialOffset;
							MatrixFrame matrixFrame = this._previousShipFrame.TransformToParent(ref localFrame2);
							Vec3 vec7 = linearVelocityAtGlobalPointForEntityWithDynamicBody;
							vec7.z = MathF.Abs(vec7.z);
							Vec3 vec8 = globalFrame.rotation.TransformToParent(ref zero);
							vec8.z = 0f;
							vec8.Normalize();
							float num6 = MathF.Clamp(num3, 3f, 20f);
							if (num4 == this._smallSplashParticleIndex)
							{
								num3 *= 1.35f;
							}
							float num7 = num / num3;
							float num8 = num2 / num3;
							Vec3 vec9 = (num7 * 0.75f + 0.25f) * Vec3.Up + vec8 * (num8 * 0.75f + 0.25f);
							vec9.Normalize();
							float num9 = MathF.Clamp((num6 - 2f) / 8f, 0.01f, 1f);
							float num10 = MathF.Lerp(3.5f, 4.5f, num9, 1E-05f);
							Vec3 vec10 = vec9 * num6 * num10;
							Vec3 vec11 = Vec3.CrossProduct(particleData.LocalFrame.rotation.s, zero);
							if (vec11.LengthSquared > 0f)
							{
								vec11.Normalize();
								Vec3 vec12 = globalFrame.rotation.TransformToLocal(ref vec10);
								Vec3 vec13 = Vec3.DotProduct(vec12, vec11) * vec11;
								Vec3 vec14 = vec12 - vec13;
								Vec3 vec15 = Vec3.CrossProduct(vec14, zero);
								if (vec15.LengthSquared > 0f && Vec3.DotProduct(vec15, vec11) < 0f)
								{
									vec14 = zero * vec14.Length;
									vec12 = vec14 + vec13;
									vec10 = globalFrame.rotation.TransformToParent(ref vec12);
								}
							}
							matrixFrame.origin = vec2 - vec10 * dt;
							particleSystem.SetPreviousGlobalFrame(ref matrixFrame);
							particleData.SplashTimer = num5 * 0.5f;
							if (flag2 && origin.DistanceSquared(matrixFrame.origin) < 400f)
							{
								SoundManager.StartOneShotEvent("event:/mission/ambient/special/wash_splash_small", ref matrixFrame.origin);
							}
							particleData.Size = num5;
							if (this._splashRandom.NextFloat() < 0.5f * num5)
							{
								particleData.SplashWaterMultiplier = (0.5f + 0.5f * this._splashRandom.NextFloat()) * 0.53f * num5;
							}
							else
							{
								particleData.SplashWaterMultiplier = 0f;
							}
						}
					}
				}
			}
		}

		// Token: 0x06000D21 RID: 3361 RVA: 0x00066B44 File Offset: 0x00064D44
		private void SnapMovementParticlePositionsToWater(float dt)
		{
			float num = 1.5f;
			if (this._movementParticleType == ShipWaterEffects.MovementParticleType.Small)
			{
				num = 1f;
			}
			MatrixFrame globalFrame = base.GameEntity.Root.GetGlobalFrame();
			bool flag = true;
			foreach (ShipWaterEffects.ParticleData particleData in this._movementParticles)
			{
				if (!(particleData.MovementParticleSystem == null))
				{
					Vec3 vec = globalFrame.TransformToParent(ref particleData.LocalFrame.origin);
					float num2 = base.GameEntity.GetWaterLevelAtPosition(vec.AsVec2, true, false) + this._movementParticleHeightOffset;
					Vec3 vec2 = vec;
					vec2.z = num2;
					float z = globalFrame.TransformToLocal(ref vec2).z;
					bool flag2 = false;
					Vec3 zero = Vec3.Zero;
					Vec3 heightCorrectedPosForSlice = this.GetHeightCorrectedPosForSlice(particleData, z, ref flag2, ref zero);
					if (flag2)
					{
						particleData.MovementParticleSystem.SetEnable(true);
						MatrixFrame localFrame = particleData.LocalFrame;
						if (!flag)
						{
							localFrame.origin = heightCorrectedPosForSlice;
						}
						particleData.MovementParticleSystem.SetLocalFrame(ref localFrame);
						float num3 = 1f;
						MatrixFrame matrixFrame = globalFrame.TransformToParent(ref localFrame);
						Vec3 linearVelocityAtGlobalPointForEntityWithDynamicBody = GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(base.GameEntity.Root, matrixFrame.origin);
						MatrixFrame identity = MatrixFrame.Identity;
						float length = linearVelocityAtGlobalPointForEntityWithDynamicBody.Length;
						Vec3 vec3 = linearVelocityAtGlobalPointForEntityWithDynamicBody;
						vec3.z = 0f;
						identity.origin = matrixFrame.origin - vec3 * dt * 0.35f * num;
						Vec3 vec4 = globalFrame.rotation.TransformToParent(ref zero);
						vec4.Normalize();
						identity.origin -= length * vec4 * 0.06f * dt;
						if (!flag)
						{
							identity.origin -= length * matrixFrame.rotation.s * 0.25f * dt * num;
						}
						particleData.MovementParticleSystem.SetPreviousGlobalFrame(ref identity);
						flag = false;
						particleData.MovementParticleSystem.SetRuntimeEmissionRateMultiplier(num3);
					}
					else
					{
						particleData.MovementParticleSystem.SetEnable(false);
					}
				}
			}
		}

		// Token: 0x06000D22 RID: 3362 RVA: 0x00066DB8 File Offset: 0x00064FB8
		private void TickHullWater(float dt, bool fromEditor)
		{
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			base.GameEntity.SetWaterVisualRecordFrameAndDt(this._waterVisualRecord, globalFrame, dt);
			if (fromEditor)
			{
				base.GameEntity.UpdateHullWaterEffectFrames(this._waterVisualRecord);
			}
			else if (!this._hullLocalFramesSetForMission)
			{
				base.GameEntity.UpdateHullWaterEffectFrames(this._waterVisualRecord);
				this._hullLocalFramesSetForMission = true;
			}
			MatrixFrame globalFrame2 = this.GetParticleParentEntity(ShipWaterEffects.ParticleType.Splash).GetGlobalFrame();
			float num = 0.1f;
			foreach (ShipWaterEffects.ParticleData particleData in this._splashParticles)
			{
				float num2 = 0.4f * particleData.Size;
				float num3 = 1f;
				if (this._shipHullHeightType == ShipWaterEffects.ShipHullHeightType.Large)
				{
					if (particleData.Size == 1f)
					{
						num3 = 0.5f;
					}
					else if (particleData.Size == 0f)
					{
						num3 = 0f;
					}
				}
				if (num3 > 0f)
				{
					if (this._cumulativeDt - particleData.LastSpawnTime > num && this._cumulativeDt - particleData.LastSpawnTime < num2)
					{
						particleData.SplashPosition += particleData.SplashVelocity * dt * this._hullWaterSplashPointSpeedMultiplier;
						Vec3 vec = globalFrame2.TransformToParent(ref particleData.SplashPosition);
						vec.z = 1f;
						vec.w = particleData.SplashWaterMultiplier * this._hullWaterSplashWaterMultiplier * 2.75f * num3;
						base.GameEntity.AddSplashPositionToWaterVisualRecord(this._waterVisualRecord, vec);
					}
					else
					{
						particleData.SplashPosition += particleData.SplashVelocity * dt;
					}
				}
			}
			if (this._ownerSceneCached.GetFallDensity() > 0.5f)
			{
				float num4 = MathF.Clamp(0.016f / dt, 0f, 1f) * 0.9f;
				int num5 = 13;
				for (int i = 0; i < num5; i++)
				{
					Vec3 vec2 = this._bodyBB.max - this._bodyBB.min;
					Vec3 min = this._bodyBB.min;
					min.x += vec2.x * this._splashRandom.NextFloatRanged(0.1f, 0.9f);
					min.y += vec2.y * this._splashRandom.NextFloatRanged(0.1f, 0.9f);
					Vec3 vec3 = globalFrame2.TransformToParent(ref min);
					vec3.w = this._splashRandom.NextFloatRanged(3.25f, 10.65f) * num4;
					vec3.z = this._splashRandom.NextFloatRanged(0.05f, 0.07f);
					base.GameEntity.AddSplashPositionToWaterVisualRecord(this._waterVisualRecord, vec3);
				}
				if ((float)this._splashRandom.Next() > 0.2f)
				{
					Vec3 vec4 = this._bodyBB.max - this._bodyBB.min;
					Vec3 min2 = this._bodyBB.min;
					min2.x += vec4.x * this._splashRandom.NextFloatRanged(0.1f, 0.9f);
					min2.y += vec4.y * this._splashRandom.NextFloatRanged(0.1f, 0.9f);
					Vec3 vec5 = globalFrame2.TransformToParent(ref min2);
					vec5.w = this._splashRandom.NextFloatRanged(1.05f, 2.05f) * num4;
					vec5.z = this._splashRandom.NextFloatRanged(0.45f, 0.85f);
					base.GameEntity.AddSplashPositionToWaterVisualRecord(this._waterVisualRecord, vec5);
				}
			}
		}

		// Token: 0x06000D23 RID: 3363 RVA: 0x000671C0 File Offset: 0x000653C0
		private Vec3 GetHeightCorrectedPosForSlice(ShipWaterEffects.ParticleData particleData, float height, ref bool pointIsValid, ref Vec3 limitingVector)
		{
			int num = particleData.PerSlicePositions.BinarySearch(new KeyValuePair<float, ShipWaterEffects.SliceSampleData>(height, default(ShipWaterEffects.SliceSampleData)), ShipWaterEffects._cacheCompareDelegate);
			if (num >= 0)
			{
				pointIsValid = true;
				limitingVector = particleData.PerSlicePositions[num].Value.limitingUpVector;
				return particleData.PerSlicePositions[num].Value.localPosition;
			}
			int num2 = ~num;
			if (num2 > 0 && num2 < particleData.PerSlicePositions.Count)
			{
				int num3 = num2 - 1;
				KeyValuePair<float, ShipWaterEffects.SliceSampleData> keyValuePair = particleData.PerSlicePositions[num3];
				KeyValuePair<float, ShipWaterEffects.SliceSampleData> keyValuePair2 = particleData.PerSlicePositions[num2];
				float num4 = (height - keyValuePair.Key) / (keyValuePair2.Key - keyValuePair.Key);
				pointIsValid = true;
				limitingVector = Vec3.Lerp(keyValuePair.Value.limitingUpVector, keyValuePair2.Value.limitingUpVector, num4);
				return Vec3.Lerp(keyValuePair.Value.localPosition, keyValuePair2.Value.localPosition, num4);
			}
			pointIsValid = false;
			return Vec3.Zero;
		}

		// Token: 0x06000D24 RID: 3364 RVA: 0x000672D8 File Offset: 0x000654D8
		private void CheckWaterVisualRegistry()
		{
			this._waterVisualRecord = base.GameEntity.Scene.RegisterShipVisualToWaterRenderer(base.GameEntity, ref this._waterSimulationBoundingBox);
			if (this._waterVisualRecord != UIntPtr.Zero)
			{
				float num = 1f;
				switch (this._hullWaterResScale)
				{
				case ShipWaterEffects.ResolutionScale.half:
					num = 0.5f;
					break;
				case ShipWaterEffects.ResolutionScale.quarter:
					num = 0.25f;
					break;
				case ShipWaterEffects.ResolutionScale.one_eight:
					num = 0.125f;
					break;
				case ShipWaterEffects.ResolutionScale.one_sixteenth:
					num = 0.0625f;
					break;
				}
				base.GameEntity.ChangeResolutionMultiplierOfWaterVisual(this._waterVisualRecord, num, ref this._waterSimulationBoundingBox);
				this.SetMeshesToRenderForInHullWater();
			}
		}

		// Token: 0x06000D25 RID: 3365 RVA: 0x00067384 File Offset: 0x00065584
		private void SetMeshesToRenderForInHullWater()
		{
			base.GameEntity.RefreshMeshesToRenderToHullWater(this._waterVisualRecord, "render_to_depth");
		}

		// Token: 0x06000D26 RID: 3366 RVA: 0x000673AC File Offset: 0x000655AC
		public void EnableWakeAndParticles()
		{
			if (!this._wakeAndParticlesEnabled)
			{
				float num = 0f;
				NavalPhysics firstScriptOfType = base.GameEntity.Root.GetFirstScriptOfType<NavalPhysics>();
				if (firstScriptOfType != null)
				{
					num = firstScriptOfType.StabilitySubmergedHeightOfShip;
				}
				this.PlaceParticles(ShipWaterEffects.ParticleType.Splash, num + this._splashParticleHeightOffset);
				this.PlaceParticles(ShipWaterEffects.ParticleType.Movement, num + this._movementParticleHeightOffset);
			}
			this._wakeAndParticlesEnabled = true;
		}

		// Token: 0x06000D27 RID: 3367 RVA: 0x00067410 File Offset: 0x00065610
		public void DeregisterWaterMeshMaterials()
		{
			if (this._waterVisualRecord != UIntPtr.Zero)
			{
				base.GameEntity.DeRegisterWaterMeshMaterials(this._waterVisualRecord);
			}
		}

		// Token: 0x06000D28 RID: 3368 RVA: 0x00067444 File Offset: 0x00065644
		private void HandleSplashFoamDecals(float dt)
		{
			if (this._movementParticles.Count == 0)
			{
				return;
			}
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			Vec3 vec;
			vec..ctor(1.564f, 1.428f, 2f, -1f);
			Vec3 vec2;
			vec2..ctor(vec.x * 17.5f, vec.y * 17.5f, vec.z, -1f);
			foreach (ShipWaterEffects.SplashFoamDecal splashFoamDecal in this._splashFoamDecals)
			{
				float num = 11.5f;
				if (this._movementParticleType == ShipWaterEffects.MovementParticleType.Large)
				{
					num += 3f;
				}
				else if (this._movementParticleType == ShipWaterEffects.MovementParticleType.Medium)
				{
					num += 1.5f;
				}
				float num2 = num - 0.75f;
				if (splashFoamDecal._splashFoamDecal != null && splashFoamDecal._cumulativeDtTillStart < num)
				{
					splashFoamDecal._cumulativeDtTillStart += dt;
					if (splashFoamDecal._cumulativeDtTillStart > 0.75f)
					{
						float num3 = splashFoamDecal._cumulativeDtTillStart - 0.75f;
						float num4 = MathF.Clamp(1f - num3 / num2, 0f, 1f);
						float num5 = 4f;
						float num6 = 0.475f;
						float num7 = MathF.Pow(num4, num5) * (0.95f - num6) + num6;
						splashFoamDecal._splashFoamDecal.SetAlpha(num7);
					}
					else
					{
						float num8 = MathF.Clamp(splashFoamDecal._cumulativeDtTillStart / 0.75f, 0f, 1f);
						float num9 = 4f;
						float num10 = 0.475f;
						float num11 = (1f - MathF.Pow(1f - num8, num9)) * (0.95f - num10) + num10;
						splashFoamDecal._splashFoamDecal.SetAlpha(num11);
					}
					ShipWaterEffects.SplashFoamDecal splashFoamDecal2 = splashFoamDecal;
					splashFoamDecal2._currentFrame.origin = splashFoamDecal2._currentFrame.origin + splashFoamDecal._currentSpeed * dt;
					splashFoamDecal._currentFrame.origin.z = this._ownerSceneCached.GetWaterLevelAtPosition(splashFoamDecal._currentFrame.origin.AsVec2, true, false) - 0.15f;
					Vec3 currentSpeed = splashFoamDecal._currentSpeed;
					float num12 = currentSpeed.Normalize();
					num12 = MathF.Max(num12 - dt * 2.5f, 0f);
					splashFoamDecal._currentSpeed = num12 * currentSpeed;
					float num13 = MathF.Clamp(splashFoamDecal._cumulativeDtTillStart / num, 0f, 1f);
					Vec3 vec3 = Vec3.Lerp(vec, vec2, num13);
					vec3.x *= splashFoamDecal._randomScale.x;
					vec3.y *= splashFoamDecal._randomScale.y;
					vec3.z *= splashFoamDecal._randomScale.z;
					float num14 = num;
					float num15 = MathF.Clamp(splashFoamDecal._cumulativeDtTillStart / num14, 0f, 1f);
					Vec3 vec4 = Vec3.Slerp(splashFoamDecal._sideVectorStart, splashFoamDecal._sideVectorEnd, num15);
					vec4.Normalize();
					splashFoamDecal._currentFrame.rotation.s = vec4;
					splashFoamDecal._currentFrame.rotation.u = Vec3.Up;
					splashFoamDecal._currentFrame.rotation.f = -splashFoamDecal._currentFrame.rotation.s.CrossProductWithUp();
					splashFoamDecal._currentFrame.rotation.ApplyScaleLocal(ref vec3);
					splashFoamDecal._splashFoamDecal.Frame = splashFoamDecal._currentFrame;
				}
				else if (splashFoamDecal._splashFoamDecal != null)
				{
					splashFoamDecal._splashFoamDecal.SetIsVisible(false);
				}
			}
			Vec3 vec5 = globalFrame.TransformToParent(ref this._movementParticles[0].LocalFrame.origin);
			float num16 = this._lastDecalLeftSpawnPosition.DistanceSquared(vec5);
			if (this._nextDecalLeftSpawnMetersSq < num16)
			{
				Vec3 vec6 = (globalFrame.origin - this._previousShipFrameForDecalSpawn) / dt;
				Vec3 s = globalFrame.rotation.s;
				s.z = 0f;
				s.Normalize();
				ShipWaterEffects.SplashFoamDecal splashFoamDecal3 = this._splashFoamDecals[this._nextDecalToUse];
				if (splashFoamDecal3._splashFoamDecal == null)
				{
					Decal decal = Decal.CreateDecal(null);
					decal.SetMaterial(Material.GetFromResource("decal_water_foam"));
					this._ownerSceneCached.AddDecalInstance(decal, "editor_set", true);
					splashFoamDecal3._splashFoamDecal = decal;
				}
				Vec3 vec7 = this._movementParticles[this._leftDecalParticleIndex].LocalFrame.origin;
				bool flag = true;
				Vec3 vec8 = globalFrame.TransformToParent(ref vec7);
				float waterLevelAtPosition = base.GameEntity.GetWaterLevelAtPosition(vec8.AsVec2, true, false);
				vec8.z = waterLevelAtPosition + 2.5f;
				Vec3 zero = Vec3.Zero;
				vec7 = this.GetHeightCorrectedPosForSlice(this._movementParticles[this._leftDecalParticleIndex], globalFrame.TransformToLocalNonOrthogonal(ref vec8).z, ref flag, ref zero);
				if (flag)
				{
					float num17 = 4f + (MBRandom.RandomFloat - 0.5f) * 1.5f;
					this._nextDecalLeftSpawnMetersSq = num17 * num17;
					Vec3 surfaceNormal = this._movementParticles[this._leftDecalParticleIndex].SurfaceNormal;
					MatrixFrame identity = MatrixFrame.Identity;
					identity.origin = globalFrame.TransformToParent(ref vec7);
					identity.rotation.u = Vec3.Up;
					Vec3 vec9 = globalFrame.rotation.TransformToParent(ref surfaceNormal);
					vec9.z = 0f;
					vec9.Normalize();
					identity.rotation.s = vec9;
					identity.rotation.f = -identity.rotation.s.CrossProductWithUp();
					identity.rotation.f.Normalize();
					identity.origin -= 0.35f * vec9;
					splashFoamDecal3._cumulativeDtTillStart = 0f;
					splashFoamDecal3._splashFoamDecal.SetIsVisible(true);
					float num18 = MathF.Clamp((vec6.Length - 4f) / 8f, 0f, 1f);
					float num19 = 0.6f + num18 * 0.2f;
					splashFoamDecal3._randomScale = Vec3.One * (0.9f + MBRandom.RandomFloat * 0.2f) * num19;
					ShipWaterEffects.SplashFoamDecal splashFoamDecal4 = splashFoamDecal3;
					splashFoamDecal4._randomScale.x = splashFoamDecal4._randomScale.x * (1f * MBRandom.RandomFloat + 0.4f);
					identity.rotation.ApplyScaleLocal(ref vec);
					splashFoamDecal3._splashFoamDecal.Frame = identity;
					splashFoamDecal3._splashFoamDecal.SetAlpha(0f);
					splashFoamDecal3._currentFrame = identity;
					int num20 = MBRandom.RandomInt(3);
					float num21 = (float)(num20 % 2) * 0.5f;
					float num22 = (float)(num20 / 2) * 0.5f;
					splashFoamDecal3._splashFoamDecal.SetVectorArgument(num21, num22, -0.5f, -0.5f);
					float num23 = 0.16f * (0.8f + MBRandom.RandomFloat * 0.4f);
					float num24 = 0.45f * (0.8f + MBRandom.RandomFloat * 0.4f);
					splashFoamDecal3._currentSpeed = vec6 * num24 + identity.rotation.s * vec6.Length * num23;
					float num25 = -0.34906584f * (0.8f + MBRandom.RandomFloat * 0.4f);
					splashFoamDecal3._sideVectorStart = vec9;
					splashFoamDecal3._sideVectorStart.RotateAboutZ(1.5707964f);
					splashFoamDecal3._sideVectorEnd = splashFoamDecal3._sideVectorStart;
					splashFoamDecal3._sideVectorEnd.RotateAboutZ(num25);
					splashFoamDecal3._isLeft = true;
					Vec2 vec10;
					vec10..ctor(2.5f, 2.5f);
					splashFoamDecal3._splashFoamDecal.OverrideRoadBoundaryP0(vec10);
					Vec2 vec11;
					vec11..ctor(MBRandom.RandomFloat, MBRandom.RandomFloat);
					splashFoamDecal3._splashFoamDecal.OverrideRoadBoundaryP1(vec11);
					this._nextDecalToUse = (this._nextDecalToUse + 1) % 50;
					this._lastDecalLeftSpawnPosition = vec5;
				}
			}
			num16 = this._lastDecalRightSpawnPosition.DistanceSquared(vec5);
			if (this._nextDecalRightSpawnMetersSq < num16)
			{
				Vec3 vec12 = (globalFrame.origin - this._previousShipFrameForDecalSpawn) / dt;
				Vec3 s2 = globalFrame.rotation.s;
				s2.z = 0f;
				s2.Normalize();
				ShipWaterEffects.SplashFoamDecal splashFoamDecal5 = this._splashFoamDecals[this._nextDecalToUse];
				if (splashFoamDecal5._splashFoamDecal == null)
				{
					Decal decal2 = Decal.CreateDecal(null);
					decal2.SetMaterial(Material.GetFromResource("decal_water_foam"));
					this._ownerSceneCached.AddDecalInstance(decal2, "editor_set", true);
					splashFoamDecal5._splashFoamDecal = decal2;
				}
				Vec3 vec13 = this._movementParticles[this._rightDecalParticleIndex].LocalFrame.origin;
				bool flag2 = true;
				Vec3 vec14 = globalFrame.TransformToParent(ref vec13);
				float waterLevelAtPosition2 = base.GameEntity.GetWaterLevelAtPosition(vec14.AsVec2, true, false);
				vec14.z = waterLevelAtPosition2 + 2.5f;
				Vec3 zero2 = Vec3.Zero;
				vec13 = this.GetHeightCorrectedPosForSlice(this._movementParticles[this._rightDecalParticleIndex], globalFrame.TransformToLocalNonOrthogonal(ref vec14).z, ref flag2, ref zero2);
				if (flag2)
				{
					float num26 = 4f + (MBRandom.RandomFloat - 0.5f) * 1.5f;
					this._nextDecalRightSpawnMetersSq = num26 * num26;
					Vec3 surfaceNormal2 = this._movementParticles[this._rightDecalParticleIndex].SurfaceNormal;
					MatrixFrame identity2 = MatrixFrame.Identity;
					identity2.origin = globalFrame.TransformToParent(ref vec13);
					identity2.rotation.u = Vec3.Up;
					Vec3 vec15 = globalFrame.rotation.TransformToParent(ref surfaceNormal2);
					vec15.z = 0f;
					vec15.Normalize();
					identity2.rotation.s = vec15;
					identity2.rotation.f = -identity2.rotation.s.CrossProductWithUp();
					identity2.rotation.f.Normalize();
					identity2.origin -= 0.35f * vec15;
					splashFoamDecal5._cumulativeDtTillStart = 0f;
					splashFoamDecal5._splashFoamDecal.SetIsVisible(true);
					float num27 = MathF.Clamp((vec12.Length - 4f) / 8f, 0f, 1f);
					float num28 = 0.6f + num27 * 0.2f;
					splashFoamDecal5._randomScale = Vec3.One * (0.9f + MBRandom.RandomFloat * 0.2f) * num28;
					ShipWaterEffects.SplashFoamDecal splashFoamDecal6 = splashFoamDecal5;
					splashFoamDecal6._randomScale.x = splashFoamDecal6._randomScale.x * (1f * MBRandom.RandomFloat + 0.4f);
					identity2.rotation.ApplyScaleLocal(ref vec);
					splashFoamDecal5._splashFoamDecal.Frame = identity2;
					splashFoamDecal5._splashFoamDecal.SetAlpha(0f);
					splashFoamDecal5._currentFrame = identity2;
					float num29 = 0.16f * (0.8f + MBRandom.RandomFloat * 0.4f);
					float num30 = 0.45f * (0.8f + MBRandom.RandomFloat * 0.4f);
					splashFoamDecal5._currentSpeed = vec12 * num30 + identity2.rotation.s * vec12.Length * num29;
					int num31 = MBRandom.RandomInt(3);
					float num32 = (float)(num31 % 2) * 0.5f;
					float num33 = (float)(num31 / 2) * 0.5f;
					splashFoamDecal5._splashFoamDecal.SetVectorArgument(num32, num33, -0.5f, 0.5f);
					float num34 = 0.34906584f * (0.8f + MBRandom.RandomFloat * 0.4f);
					splashFoamDecal5._sideVectorStart = vec15;
					splashFoamDecal5._sideVectorStart.RotateAboutZ(-1.5707964f);
					splashFoamDecal5._sideVectorEnd = splashFoamDecal5._sideVectorStart;
					splashFoamDecal5._sideVectorEnd.RotateAboutZ(num34);
					splashFoamDecal5._isLeft = false;
					Vec2 vec16;
					vec16..ctor(2.5f, 2.5f);
					splashFoamDecal5._splashFoamDecal.OverrideRoadBoundaryP0(vec16);
					Vec2 vec17;
					vec17..ctor(MBRandom.RandomFloat, MBRandom.RandomFloat);
					splashFoamDecal5._splashFoamDecal.OverrideRoadBoundaryP1(vec17);
					this._nextDecalToUse = (this._nextDecalToUse + 1) % 50;
					this._lastDecalRightSpawnPosition = vec5;
				}
			}
			this._previousShipFrameForDecalSpawn = globalFrame.origin;
		}

		// Token: 0x06000D29 RID: 3369 RVA: 0x00068080 File Offset: 0x00066280
		private void HandleWetnessDecals(float dt)
		{
			base.GameEntity.IsInEditorScene();
			float num = dt / 6f;
			foreach (ShipWaterEffects.WetnessDecalData wetnessDecalData in this._wetnessDecals)
			{
				foreach (ShipWaterEffects.ParticleData particleData in this._splashParticles)
				{
					if (particleData.CurrentSplashParticle != null && particleData.CurrentSplashParticle.HasAliveParticles())
					{
						float num2 = 0.13f * particleData.Size * dt;
						float num3 = particleData.Size * 2.1f;
						if (Vec3.DotProduct(wetnessDecalData.Normal, particleData.LocalFrame.rotation.s) > 0f && wetnessDecalData.LocalPosition.AsVec2.Distance(particleData.LocalFrame.origin.AsVec2) < num3)
						{
							float num4 = 1f;
							wetnessDecalData.CurrentAlpha = Math.Min(wetnessDecalData.CurrentAlpha + num2 * num4, 1f);
						}
					}
				}
				wetnessDecalData.CurrentAlpha = MathF.Max(wetnessDecalData.CurrentAlpha - num, 0f);
				float num5 = MathF.Pow(wetnessDecalData.CurrentAlpha, 0.5f);
				float num6 = 0.2f + num5 * 0.8f;
				wetnessDecalData.Decal.SetAlpha(MathF.Min(num6, 1f));
			}
		}

		// Token: 0x040007E9 RID: 2025
		private const string FloaterHolderTag = "floater_volume_holder";

		// Token: 0x040007EA RID: 2026
		private const string FloaterTag = "floater_volume";

		// Token: 0x040007EB RID: 2027
		private const string BodyMeshTag = "body_mesh";

		// Token: 0x040007EC RID: 2028
		private const string SplashEntityTag = "splash_particles";

		// Token: 0x040007ED RID: 2029
		private const string MovementEntityTag = "movement_particles";

		// Token: 0x040007EE RID: 2030
		private const string WaterDepthRenderMeshTag = "render_to_depth";

		// Token: 0x040007EF RID: 2031
		private const float ParticleSliceHeightDx = 0.5f;

		// Token: 0x040007F0 RID: 2032
		private const int NumberOfSplashDecal = 50;

		// Token: 0x040007F1 RID: 2033
		private const float SmallSplashSoundEventMaxDistanceSquared = 400f;

		// Token: 0x040007F2 RID: 2034
		private static readonly Comparer<KeyValuePair<float, ShipWaterEffects.SliceSampleData>> _cacheCompareDelegate = Comparer<KeyValuePair<float, ShipWaterEffects.SliceSampleData>>.Create((KeyValuePair<float, ShipWaterEffects.SliceSampleData> x, KeyValuePair<float, ShipWaterEffects.SliceSampleData> y) => x.Key.CompareTo(y.Key));

		// Token: 0x040007F3 RID: 2035
		[EditableScriptComponentVariable(true, "Water Simulation Bounding Box")]
		private Vec3 _waterSimulationBoundingBox = Vec3.One;

		// Token: 0x040007F4 RID: 2036
		[EditableScriptComponentVariable(true, "Show Water Simulation Bounding Box")]
		private bool _showWaterSimulationBoundingBox;

		// Token: 0x040007F5 RID: 2037
		[EditableScriptComponentVariable(true, "Reset Water Simulation Bounding Box")]
		private SimpleButton _resetWaterSimulationBoundingBox = new SimpleButton();

		// Token: 0x040007F6 RID: 2038
		[EditableScriptComponentVariable(true, "Re-render Depth Texture")]
		private SimpleButton _reRenderDepthTexture = new SimpleButton();

		// Token: 0x040007F7 RID: 2039
		[EditableScriptComponentVariable(true, "Reset In-Hull Water")]
		private SimpleButton _resetInHullWater = new SimpleButton();

		// Token: 0x040007F8 RID: 2040
		[EditableScriptComponentVariable(true, "Show Hull Water Debug Panel")]
		private bool _showHullWaterDebugPanel;

		// Token: 0x040007F9 RID: 2041
		[EditableScriptComponentVariable(true, "Hull Water Simulation Resolution Scale")]
		private ShipWaterEffects.ResolutionScale _hullWaterResScale = ShipWaterEffects.ResolutionScale.half;

		// Token: 0x040007FA RID: 2042
		[EditableScriptComponentVariable(true, "Hull Water Splash Water Multiplier")]
		private float _hullWaterSplashWaterMultiplier = 1.75f;

		// Token: 0x040007FB RID: 2043
		[EditableScriptComponentVariable(true, "Hull Water Splash Point Initial Offset")]
		private float _hullWaterSplashPointInitialOffset = 0.5f;

		// Token: 0x040007FC RID: 2044
		[EditableScriptComponentVariable(true, "Hull Water Splash Point Speed Multiplier")]
		private float _hullWaterSplashPointSpeedMultiplier = 1f;

		// Token: 0x040007FD RID: 2045
		[EditableScriptComponentVariable(true, "Ship Hull Height Type")]
		private ShipWaterEffects.ShipHullHeightType _shipHullHeightType;

		// Token: 0x040007FE RID: 2046
		[EditableScriptComponentVariable(true, "Movement Particle Height Offset")]
		private float _movementParticleHeightOffset = 0.34f;

		// Token: 0x040007FF RID: 2047
		[EditableScriptComponentVariable(true, "Splash Particle Height Offset")]
		private float _splashParticleHeightOffset = 0.4f;

		// Token: 0x04000800 RID: 2048
		[EditableScriptComponentVariable(true, "Movement Particle Surface Distance Offset")]
		private float _movementParticleSurfaceDistanceOffset = 0.7f;

		// Token: 0x04000801 RID: 2049
		[EditableScriptComponentVariable(true, "Splash Particle Surface Distance Offset")]
		private float _splashParticleSurfaceDistanceOffset = 0.7f;

		// Token: 0x04000802 RID: 2050
		[EditableScriptComponentVariable(true, "Movement Particle Type")]
		private ShipWaterEffects.MovementParticleType _movementParticleType;

		// Token: 0x04000803 RID: 2051
		[EditableScriptComponentVariable(true, "Movement Particle Side Speed Vector")]
		private float _movementParticleSideSpeedVector = 0.5f;

		// Token: 0x04000804 RID: 2052
		[EditableScriptComponentVariable(true, "Show Movement Particles")]
		private bool _showMovementParticles;

		// Token: 0x04000805 RID: 2053
		[EditableScriptComponentVariable(true, "Show Splash Particles")]
		private bool _showSplashParticles;

		// Token: 0x04000806 RID: 2054
		[EditableScriptComponentVariable(true, "Show Water Balance Plane")]
		private bool _showWaterBalancePlane;

		// Token: 0x04000807 RID: 2055
		[EditableScriptComponentVariable(true, "Show Wetness Decal Values")]
		private bool _showWetnessDecalValues;

		// Token: 0x04000808 RID: 2056
		[EditableScriptComponentVariable(true, "Force Wetness Decal To Full")]
		private bool _forceWetnessDecalsToFull;

		// Token: 0x04000809 RID: 2057
		private UIntPtr _waterVisualRecord = UIntPtr.Zero;

		// Token: 0x0400080A RID: 2058
		private GameEntity _movementParticleEntity;

		// Token: 0x0400080B RID: 2059
		private GameEntity _splashParticleEntity;

		// Token: 0x0400080C RID: 2060
		private readonly List<ShipWaterEffects.ParticleData> _movementParticles = new List<ShipWaterEffects.ParticleData>();

		// Token: 0x0400080D RID: 2061
		private readonly List<ShipWaterEffects.ParticleData> _splashParticles = new List<ShipWaterEffects.ParticleData>();

		// Token: 0x0400080E RID: 2062
		private readonly MBFastRandom _splashRandom = new MBFastRandom();

		// Token: 0x0400080F RID: 2063
		private readonly List<ShipWaterEffects.WetnessDecalData> _wetnessDecals = new List<ShipWaterEffects.WetnessDecalData>();

		// Token: 0x04000810 RID: 2064
		private MatrixFrame _previousShipFrame = MatrixFrame.Identity;

		// Token: 0x04000811 RID: 2065
		private float _cumulativeDt;

		// Token: 0x04000812 RID: 2066
		private bool _inCampaignMode;

		// Token: 0x04000813 RID: 2067
		private Scene _ownerSceneCached;

		// Token: 0x04000814 RID: 2068
		private int _smallSplashParticleIndex = -1;

		// Token: 0x04000815 RID: 2069
		private int _mediumSplashParticleIndex = -1;

		// Token: 0x04000816 RID: 2070
		private int _largeSplashParticleIndex = -1;

		// Token: 0x04000817 RID: 2071
		private bool _hullLocalFramesSetForMission;

		// Token: 0x04000818 RID: 2072
		private bool _wakeAndParticlesEnabled;

		// Token: 0x04000819 RID: 2073
		private BoundingBox _bodyBB;

		// Token: 0x0400081A RID: 2074
		private readonly ShipWaterEffects.SplashFoamDecal[] _splashFoamDecals = new ShipWaterEffects.SplashFoamDecal[50];

		// Token: 0x0400081B RID: 2075
		private int _nextDecalToUse;

		// Token: 0x0400081C RID: 2076
		private Vec3 _lastDecalLeftSpawnPosition = Vec3.Zero;

		// Token: 0x0400081D RID: 2077
		private Vec3 _lastDecalRightSpawnPosition = Vec3.Zero;

		// Token: 0x0400081E RID: 2078
		private float _nextDecalLeftSpawnMetersSq = 49f;

		// Token: 0x0400081F RID: 2079
		private float _nextDecalRightSpawnMetersSq = 49f;

		// Token: 0x04000820 RID: 2080
		private Vec3 _previousShipFrameForDecalSpawn = Vec3.Zero;

		// Token: 0x04000821 RID: 2081
		private int _leftDecalParticleIndex = -1;

		// Token: 0x04000822 RID: 2082
		private int _rightDecalParticleIndex = -1;

		// Token: 0x02000231 RID: 561
		internal enum ParticleType
		{
			// Token: 0x04000F64 RID: 3940
			Movement,
			// Token: 0x04000F65 RID: 3941
			Splash
		}

		// Token: 0x02000232 RID: 562
		internal enum MovementParticleType
		{
			// Token: 0x04000F67 RID: 3943
			Small,
			// Token: 0x04000F68 RID: 3944
			Medium,
			// Token: 0x04000F69 RID: 3945
			Large
		}

		// Token: 0x02000233 RID: 563
		internal enum ShipHullHeightType
		{
			// Token: 0x04000F6B RID: 3947
			Small,
			// Token: 0x04000F6C RID: 3948
			Medium,
			// Token: 0x04000F6D RID: 3949
			Large
		}

		// Token: 0x02000234 RID: 564
		internal enum ResolutionScale
		{
			// Token: 0x04000F6F RID: 3951
			one,
			// Token: 0x04000F70 RID: 3952
			half,
			// Token: 0x04000F71 RID: 3953
			quarter,
			// Token: 0x04000F72 RID: 3954
			one_eight,
			// Token: 0x04000F73 RID: 3955
			one_sixteenth
		}

		// Token: 0x02000235 RID: 565
		private struct FloaterData
		{
			// Token: 0x04000F74 RID: 3956
			internal float HeightMin;

			// Token: 0x04000F75 RID: 3957
			internal float VerticalLength;

			// Token: 0x04000F76 RID: 3958
			internal float HorizontalArea;
		}

		// Token: 0x02000236 RID: 566
		private class WetnessDecalData
		{
			// Token: 0x04000F77 RID: 3959
			internal Decal Decal;

			// Token: 0x04000F78 RID: 3960
			internal Vec3 Normal;

			// Token: 0x04000F79 RID: 3961
			internal Vec3 LocalPosition;

			// Token: 0x04000F7A RID: 3962
			internal float CurrentAlpha;
		}

		// Token: 0x02000237 RID: 567
		private struct SliceSampleData
		{
			// Token: 0x04000F7B RID: 3963
			internal Vec3 localPosition;

			// Token: 0x04000F7C RID: 3964
			internal Vec3 limitingUpVector;
		}

		// Token: 0x02000238 RID: 568
		private class ParticleData
		{
			// Token: 0x04000F7D RID: 3965
			internal ParticleSystem MovementParticleSystem;

			// Token: 0x04000F7E RID: 3966
			internal MatrixFrame LocalFrame = MatrixFrame.Identity;

			// Token: 0x04000F7F RID: 3967
			internal Vec3 SurfaceNormal = Vec3.Zero;

			// Token: 0x04000F80 RID: 3968
			internal ParticleSystem CurrentSplashParticle;

			// Token: 0x04000F81 RID: 3969
			internal float SplashTimer;

			// Token: 0x04000F82 RID: 3970
			internal float LastSpawnTime;

			// Token: 0x04000F83 RID: 3971
			internal bool WasAboveWater = true;

			// Token: 0x04000F84 RID: 3972
			internal Vec3 SplashVelocity = Vec3.Zero;

			// Token: 0x04000F85 RID: 3973
			internal Vec3 SplashPosition = Vec3.Zero;

			// Token: 0x04000F86 RID: 3974
			internal float SplashWaterMultiplier;

			// Token: 0x04000F87 RID: 3975
			internal List<KeyValuePair<float, ShipWaterEffects.SliceSampleData>> PerSlicePositions;

			// Token: 0x04000F88 RID: 3976
			internal float Size;
		}

		// Token: 0x02000239 RID: 569
		private class SplashFoamDecal
		{
			// Token: 0x06001B1F RID: 6943 RVA: 0x000B2218 File Offset: 0x000B0418
			internal SplashFoamDecal()
			{
				this._splashFoamDecal = null;
				this._currentFrame = MatrixFrame.Identity;
				this._sideVectorStart = Vec3.Zero;
				this._sideVectorEnd = Vec3.Zero;
				this._cumulativeDtTillStart = 0f;
				this._randomScale = new Vec3(1f, 1f, 1f, -1f);
				this._currentSpeed = Vec3.Zero;
				this._isLeft = false;
			}

			// Token: 0x04000F89 RID: 3977
			internal Decal _splashFoamDecal;

			// Token: 0x04000F8A RID: 3978
			internal MatrixFrame _currentFrame;

			// Token: 0x04000F8B RID: 3979
			internal float _cumulativeDtTillStart;

			// Token: 0x04000F8C RID: 3980
			internal Vec3 _randomScale;

			// Token: 0x04000F8D RID: 3981
			internal Vec3 _currentSpeed;

			// Token: 0x04000F8E RID: 3982
			internal Vec3 _sideVectorStart;

			// Token: 0x04000F8F RID: 3983
			internal Vec3 _sideVectorEnd;

			// Token: 0x04000F90 RID: 3984
			internal bool _isLeft;
		}
	}
}
