using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Objects;

namespace NavalDLC.Missions.Objects
{
	// Token: 0x020000A1 RID: 161
	[ScriptComponentParams("ship_visual_only", "sail_visual")]
	public class SailVisual : ScriptComponentBehavior
	{
		// Token: 0x17000243 RID: 579
		// (get) Token: 0x06000C4D RID: 3149 RVA: 0x00058DE5 File Offset: 0x00056FE5
		public float TotalFoldDuration
		{
			get
			{
				return this._totalFoldDuration;
			}
		}

		// Token: 0x17000244 RID: 580
		// (get) Token: 0x06000C4E RID: 3150 RVA: 0x00058DED File Offset: 0x00056FED
		public float TotalUnfoldDuration
		{
			get
			{
				return this._totalUnfoldDuration;
			}
		}

		// Token: 0x17000245 RID: 581
		// (get) Token: 0x06000C4F RID: 3151 RVA: 0x00058DF5 File Offset: 0x00056FF5
		// (set) Token: 0x06000C50 RID: 3152 RVA: 0x00058DFD File Offset: 0x00056FFD
		public ClothSimulatorComponent SailClothComponent { get; private set; }

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x06000C51 RID: 3153 RVA: 0x00058E06 File Offset: 0x00057006
		// (set) Token: 0x06000C52 RID: 3154 RVA: 0x00058E0E File Offset: 0x0005700E
		public GameEntity SailSkeletonEntity { get; private set; }

		// Token: 0x17000247 RID: 583
		// (get) Token: 0x06000C53 RID: 3155 RVA: 0x00058E17 File Offset: 0x00057017
		// (set) Token: 0x06000C54 RID: 3156 RVA: 0x00058E1F File Offset: 0x0005701F
		public GameEntity SailYawRotationEntity { get; private set; }

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x06000C55 RID: 3157 RVA: 0x00058E28 File Offset: 0x00057028
		// (set) Token: 0x06000C56 RID: 3158 RVA: 0x00058E30 File Offset: 0x00057030
		public ClothSimulatorComponent SailTopBannerClothComponent { get; private set; }

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x06000C57 RID: 3159 RVA: 0x00058E39 File Offset: 0x00057039
		// (set) Token: 0x06000C58 RID: 3160 RVA: 0x00058E41 File Offset: 0x00057041
		public GameEntity SailTopBannerEntity { get; private set; }

		// Token: 0x1700024A RID: 586
		// (get) Token: 0x06000C59 RID: 3161 RVA: 0x00058E4A File Offset: 0x0005704A
		public SailVisual.SailType Type
		{
			get
			{
				return this._sailType;
			}
		}

		// Token: 0x1700024B RID: 587
		// (get) Token: 0x06000C5A RID: 3162 RVA: 0x00058E52 File Offset: 0x00057052
		// (set) Token: 0x06000C5B RID: 3163 RVA: 0x00058E5A File Offset: 0x0005705A
		public ShipVisual ShipVisual { get; private set; }

		// Token: 0x1700024C RID: 588
		// (get) Token: 0x06000C5C RID: 3164 RVA: 0x00058E63 File Offset: 0x00057063
		// (set) Token: 0x06000C5D RID: 3165 RVA: 0x00058E6B File Offset: 0x0005706B
		public bool SailEnabled { get; set; } = true;

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x06000C5E RID: 3166 RVA: 0x00058E74 File Offset: 0x00057074
		// (set) Token: 0x06000C5F RID: 3167 RVA: 0x00058E7C File Offset: 0x0005707C
		public bool SoundsEnabled { get; set; }

		// Token: 0x1700024E RID: 590
		// (get) Token: 0x06000C60 RID: 3168 RVA: 0x00058E85 File Offset: 0x00057085
		// (set) Token: 0x06000C61 RID: 3169 RVA: 0x00058E8D File Offset: 0x0005708D
		public bool FoldAnimationEnabled { get; set; } = true;

		// Token: 0x06000C62 RID: 3170 RVA: 0x00058E98 File Offset: 0x00057098
		internal SailVisual()
		{
			this._ongoingAnimationData.CurrentProgress = 0f;
			this._ongoingAnimationData.RealProgress = 0f;
			this._ongoingAnimationData.FoldIsOngoing = false;
			this._ongoingAnimationData.UnfoldIsOngoing = false;
			this._ongoingAnimationData.LeftVertexPositions = null;
			this._ongoingAnimationData.RightVertexPositions = null;
			this._ongoingAnimationData.CenterVertexPositions = null;
			this._ongoingAnimationData.NumberOfMorphKeys = -1;
			this._ongoingAnimationData.CurrentLeftFreeBonePosition = Vec3.Zero;
			this._ongoingAnimationData.CurrentRightFreeBonePosition = Vec3.Zero;
			this._ongoingAnimationData.CurrentCenterFreeBonePosition = Vec3.Zero;
			this._ongoingAnimationData.FoldUnfoldSoundEvent = null;
			this._ongoingAnimationData.ShouldMakeFoldUnfoldSound = false;
			this._ongoingAnimationData.ShouldStopFoldUnfoldSound = false;
			this._lateenSailData = default(SailVisual.LateenSailData);
			this._lateenSailData.RollRotationEntity = null;
			this._lateenSailData.YardShiftEntity = null;
			this._lateenSailData.LastYawSection = 0f;
			this._lateenSailData.RollRotationAnimProgress = 0f;
			this._lateenSailData.RollRotationRealDt = 0f;
			this._lateenSailData.RollRotationInProgress = false;
			this._lateenSailData.RollRotationInitial = 0f;
			this._lateenSailData.RollRotationTarget = 0f;
			this._lateenSailData.YardShiftInitial = 0f;
			this._lateenSailData.YardShiftTarget = 0f;
			this._lateenSailData.RollAnimationSoundEvent = null;
			this._captureTheFlagAnimation = new SailVisual.FlagCaptureAnimation();
			this._captureTheFlagAnimation.AnimationInProgress = false;
			this._captureTheFlagAnimation.NewBannerTexture = null;
			this._captureTheFlagAnimation.DtTillStart = 0f;
			this._captureTheFlagAnimation.MaterialSet = false;
			this._captureTheFlagAnimation.BannerWindFactor = 1f;
			this._topFlagRope = default(SailVisual.SimpleRopeRecord);
		}

		// Token: 0x06000C63 RID: 3171 RVA: 0x00059220 File Offset: 0x00057420
		protected override void OnEditorInit()
		{
			this.SoundsEnabled = true;
			this._editorOnlyLevelSelection = SailVisual.LevelForEditor.None;
			this._editorOnlyShipHealth = 1f;
			this.FetchEntities();
			this.UpdatePreviousYardFrame();
			if (this._yardEntity != null)
			{
				MatrixFrame globalFrame = this._yardEntity.GetGlobalFrame();
				this._previousSailYardFrame = base.GameEntity.GetGlobalFrame().TransformToLocalNonOrthogonal(ref globalFrame);
			}
			if (this._sailType == SailVisual.SailType.LateenSail)
			{
				this.InitLateenSailData();
			}
			this.InitSailFoldAnimationResources();
			if (this._sailSkeleton != null)
			{
				this._sailSkeleton.EnableScriptDrivenPostIntegrateCallback();
			}
			ClothSimulatorComponent sailTopBannerClothComponent = this.SailTopBannerClothComponent;
			if (sailTopBannerClothComponent != null)
			{
				sailTopBannerClothComponent.SetForcedGustStrength(0f);
			}
			ClothSimulatorComponent sailClothComponent = this.SailClothComponent;
			if (sailClothComponent != null)
			{
				sailClothComponent.SetForcedGustStrength(0f);
			}
			this.UpdateTotalFoldDuration();
			this.UpdateTotalUnfoldDuration();
			this.ComputeMastClipPlane();
			this.PlaceClothFragmentsRandomly((int)(Time.ApplicationTime * 100f));
			this.PlaceTopFlag(this._topFlagRopePosition);
		}

		// Token: 0x06000C64 RID: 3172 RVA: 0x00059314 File Offset: 0x00057514
		protected override void OnEditorTick(float dt)
		{
			this._cumulativeDt += dt;
			this.FetchEntities();
			if (this._ongoingAnimationData.NumberOfMorphKeys == -1)
			{
				this.InitSailFoldAnimationResources();
			}
			this.ComputeMastClipPlane();
			if (!this._isBurning)
			{
				this.HandleLOD();
			}
			this.CheckFoldAnimationState(dt);
			if (this._sailType == SailVisual.SailType.LateenSail)
			{
				this.TickLateenSail(dt);
			}
			if (this.SailSkeletonEntity != null)
			{
				this.SetButtomRopePositions(dt, false);
			}
			if (!this._ropesAreInvisibleThisFrame)
			{
				this.TickRopesAndPulleys();
			}
			if (Input.IsKeyReleased(61))
			{
				this.SailEnabled = false;
			}
			else if (Input.IsKeyReleased(62))
			{
				this.SailEnabled = true;
			}
			this.FoldUnfoldSoundEventTick();
			if (this._editorOnlyLevelSelection == SailVisual.LevelForEditor.None)
			{
				int num = this.FetchSailLevel();
				if (num != this._currentSailLevelUsed)
				{
					this.AdjustLevelOfSail(num);
					this._currentSailLevelUsed = num;
				}
			}
			this.CheckClothResetTimer();
			if (this._sailSkeleton != null)
			{
				this._sailSkeleton.EnableScriptDrivenPostIntegrateCallback();
			}
			if (this._isBurning && !this._burningRecord.BurningFinished)
			{
				this.TickFire(dt);
			}
			this.UpdateMastClipPlane();
			if (this._captureTheFlagAnimation.AnimationInProgress)
			{
				this.TickFlagCaptureAnimation(dt);
			}
		}

		// Token: 0x06000C65 RID: 3173 RVA: 0x0005943C File Offset: 0x0005763C
		protected override void OnInit()
		{
			this.ShipVisual = base.GameEntity.Root.GetFirstScriptOfType<ShipVisual>();
			ShipVisual shipVisual = this.ShipVisual;
			if (shipVisual != null)
			{
				List<ScriptComponentBehavior> sailVisuals = shipVisual.SailVisuals;
				if (sailVisuals != null)
				{
					sailVisuals.Add(this);
				}
			}
			this._editorOnlyLevelSelection = SailVisual.LevelForEditor.None;
			this._editorOnlyShipHealth = 1f;
			this.FetchEntities();
			this.UpdatePreviousYardFrame();
			if (this._yardEntity != null)
			{
				MatrixFrame globalFrame = this._yardEntity.GetGlobalFrame();
				this._previousSailYardFrame = base.GameEntity.GetGlobalFrame().TransformToLocalNonOrthogonal(ref globalFrame);
			}
			if (this._sailType == SailVisual.SailType.LateenSail)
			{
				this.InitLateenSailData();
			}
			this.InitSailFoldAnimationResources();
			int num = this.FetchSailLevel();
			this.AdjustLevelOfSail(num);
			this._currentSailLevelUsed = num;
			if (this._sailSkeleton != null)
			{
				this._sailSkeleton.EnableScriptDrivenPostIntegrateCallback();
			}
			ClothSimulatorComponent sailTopBannerClothComponent = this.SailTopBannerClothComponent;
			if (sailTopBannerClothComponent != null)
			{
				sailTopBannerClothComponent.SetForcedGustStrength(0f);
			}
			ClothSimulatorComponent sailClothComponent = this.SailClothComponent;
			if (sailClothComponent != null)
			{
				sailClothComponent.SetForcedGustStrength(0f);
			}
			this.UpdateTotalFoldDuration();
			this.UpdateTotalUnfoldDuration();
			this.ComputeMastClipPlane();
			int num2 = (int)(Time.ApplicationTime * 100f);
			if (this.ShipVisual != null)
			{
				num2 = this.ShipVisual.Seed;
			}
			this.PlaceClothFragmentsRandomly(num2);
		}

		// Token: 0x06000C66 RID: 3174 RVA: 0x00059584 File Offset: 0x00057784
		protected override void OnTickParallel(float dt)
		{
			this._cumulativeDt += dt;
			if (this._ongoingAnimationData.NumberOfMorphKeys == -1)
			{
				this.InitSailFoldAnimationResources();
			}
			this.HandleLOD();
			if (this._remainingFramesForAnimation == 0)
			{
				this.CheckFoldAnimationState(dt);
			}
			else
			{
				this._remainingFramesForAnimation--;
			}
			if (this._sailType == SailVisual.SailType.LateenSail)
			{
				this.TickLateenSail(dt);
			}
			if (this.SailSkeletonEntity != null)
			{
				this.SetButtomRopePositions(dt, false);
			}
			if (!this._ropesAreInvisibleThisFrame)
			{
				this.TickRopesAndPulleys();
			}
			this.CheckClothResetTimer();
			if (this._isBurning && !this._burningRecord.BurningFinished)
			{
				this.TickFire(dt);
			}
			this.UpdateMastClipPlane();
			if (this._ballistaRopeEnableFrameCounter > 0)
			{
				this._ballistaRopeEnableFrameCounter--;
				if (this._ballistaRopeEnableFrameCounter == 0)
				{
					foreach (WeakGameEntity weakGameEntity in this._ballistaVisibilityRopes)
					{
						weakGameEntity.SetVisibilityExcludeParents(true);
					}
				}
			}
			if (this._captureTheFlagAnimation.AnimationInProgress)
			{
				this.TickFlagCaptureAnimation(dt);
			}
		}

		// Token: 0x06000C67 RID: 3175 RVA: 0x000596B0 File Offset: 0x000578B0
		protected override void OnTick(float dt)
		{
			this.FoldUnfoldSoundEventTick();
		}

		// Token: 0x06000C68 RID: 3176 RVA: 0x000596B8 File Offset: 0x000578B8
		protected override void OnEditorVariableChanged(string variableName)
		{
		}

		// Token: 0x06000C69 RID: 3177 RVA: 0x000596BC File Offset: 0x000578BC
		protected override bool SkeletonPostIntegrateCallback(AnimResult result)
		{
			foreach (SailVisual.FreeBoneRecord freeBoneRecord in this._freeBones)
			{
				if (freeBoneRecord.BoneIndex != -1)
				{
					Vec3 origin = freeBoneRecord.CurrentLocalFrame.origin;
					sbyte parentBoneIndex = this._sailSkeleton.GetParentBoneIndex(freeBoneRecord.BoneIndex);
					Transformation entitialOutTransform = result.GetEntitialOutTransform(parentBoneIndex, this._sailSkeleton);
					result.SetOutBoneDisplacement(freeBoneRecord.BoneIndex, entitialOutTransform.TransformToLocal(origin), this._sailSkeleton);
				}
			}
			if (this._sailType == SailVisual.SailType.LateenSail && this._mastEntity != null)
			{
				sbyte b = 2;
				MatrixFrame globalFrame = this.SailSkeletonEntity.GetGlobalFrame();
				MatrixFrame globalFrame2 = this._mastEntity.GetGlobalFrame();
				Vec3 u = globalFrame2.rotation.u;
				bool flag;
				MBMath.FindPlaneLineIntersectionPointWithNormal(globalFrame2.origin, u, globalFrame.origin, globalFrame.origin - u * 100f, ref flag);
				MatrixFrame globalFrame3 = this._yardEntity.GetGlobalFrame();
				globalFrame2.origin += globalFrame3.rotation.f * 0.25f * globalFrame.rotation.f.Length;
				MatrixFrame matrixFrame = globalFrame;
				matrixFrame.rotation.MakeUnit();
				MatrixFrame identity = MatrixFrame.Identity;
				identity.origin = globalFrame.TransformToLocalNonOrthogonal(ref globalFrame2.origin);
				identity.rotation = matrixFrame.TransformToLocal(ref globalFrame2).rotation;
				sbyte parentBoneIndex2 = this._sailSkeleton.GetParentBoneIndex(b);
				Transformation transformation = result.GetEntitialOutTransform(parentBoneIndex2, this._sailSkeleton).TransformToLocal(Transformation.CreateFromMatrixFrame(identity));
				transformation.Rotate(-1.5707964f, Vec3.Forward);
				result.SetOutBoneDisplacement(b, transformation.Origin, this._sailSkeleton);
				result.SetOutQuat(b, transformation.Rotation, this._sailSkeleton);
			}
			return true;
		}

		// Token: 0x06000C6A RID: 3178 RVA: 0x000598D4 File Offset: 0x00057AD4
		protected override void OnBoundingBoxValidate()
		{
			if (this._yardEntity == null || this._sailSkeleton == null)
			{
				return;
			}
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			BoundingBox boundingBox = default(BoundingBox);
			boundingBox.BeginRelaxation();
			BoundingBox localBoundingBox = this._yardEntity.GetLocalBoundingBox();
			Vec3 vec = this._yardEntity.GetGlobalFrame().origin;
			vec = globalFrame.TransformToLocalNonOrthogonal(ref vec);
			float num = localBoundingBox.radius * 1.1f;
			Vec3 vec2 = vec + Vec3.One * num;
			boundingBox.RelaxMinMaxWithPoint(ref vec2);
			vec2 = vec - Vec3.One * num;
			boundingBox.RelaxMinMaxWithPoint(ref vec2);
			foreach (SailVisual.FreeBoneRecord freeBoneRecord in this._freeBones)
			{
				if (freeBoneRecord.FoldSailPulley.PulleySystem != null)
				{
					freeBoneRecord.FoldSailPulley.PulleySystem.ApplyBoundingBox(globalFrame, ref boundingBox);
				}
				if (freeBoneRecord.RotatorPulleys != null)
				{
					foreach (SailVisual.PulleyDataCache pulleyDataCache in freeBoneRecord.RotatorPulleys)
					{
						if (pulleyDataCache.PulleySystem != null)
						{
							pulleyDataCache.PulleySystem.ApplyBoundingBox(globalFrame, ref boundingBox);
						}
					}
				}
				if (freeBoneRecord.StabilityPulleys != null)
				{
					foreach (SailVisual.PulleyDataCache pulleyDataCache2 in freeBoneRecord.StabilityPulleys)
					{
						if (pulleyDataCache2.PulleySystem != null)
						{
							pulleyDataCache2.PulleySystem.ApplyBoundingBox(globalFrame, ref boundingBox);
						}
					}
				}
				if (freeBoneRecord.StabilityRopes != null)
				{
					foreach (SailVisual.SimpleRopeRecord simpleRopeRecord in freeBoneRecord.StabilityRopes)
					{
						if (simpleRopeRecord.RopeSegment != null)
						{
							simpleRopeRecord.RopeSegment.ApplyBoundingBox(globalFrame, ref boundingBox);
						}
					}
				}
			}
			if (this._simpleRopes != null)
			{
				foreach (SailVisual.SimpleRopeRecord simpleRopeRecord2 in this._simpleRopes)
				{
					if (simpleRopeRecord2.RopeSegment != null)
					{
						simpleRopeRecord2.RopeSegment.ApplyBoundingBox(globalFrame, ref boundingBox);
					}
				}
			}
			base.GameEntity.RelaxLocalBoundingBox(ref boundingBox);
		}

		// Token: 0x06000C6B RID: 3179 RVA: 0x00059BB8 File Offset: 0x00057DB8
		protected override void OnRemoved(int removeReason)
		{
			base.OnRemoved(removeReason);
			if (this._lateenSailData.RollAnimationSoundEvent != null)
			{
				this._lateenSailData.RollAnimationSoundEvent.Stop();
				this._lateenSailData.RollAnimationSoundEvent = null;
			}
			this._ongoingAnimationData.LeftVertexPositions = null;
			this._ongoingAnimationData.RightVertexPositions = null;
			this._ongoingAnimationData.CenterVertexPositions = null;
			this._ongoingAnimationData.ShouldMakeFoldUnfoldSound = false;
			this._ongoingAnimationData.ShouldStopFoldUnfoldSound = true;
			if (this._ongoingAnimationData.FoldUnfoldSoundEvent != null)
			{
				this._ongoingAnimationData.FoldUnfoldSoundEvent.Stop();
				this._ongoingAnimationData.FoldUnfoldSoundEvent = null;
			}
			this._freeBones.Clear();
			this._simpleRopes.Clear();
			this._mastRopes.Clear();
			bool flag = base.GameEntity.IsGhostObject();
			if (this._sailSkeleton != null && !flag)
			{
				base.GameEntity.Scene.RemoveAlwaysRenderedSkeleton(this._sailSkeleton);
			}
			this._sailSkeleton = null;
			this._yardEntity = null;
			this._foldedStaticSailEntity = null;
			this.SailClothComponent = null;
			this.SailTopBannerClothComponent = null;
			this.SailTopBannerEntity = null;
			this.SailSkeletonEntity = null;
			this.SailYawRotationEntity = null;
			this._mastEntity = null;
			this._isBurning = false;
		}

		// Token: 0x06000C6C RID: 3180 RVA: 0x00059CFA File Offset: 0x00057EFA
		protected override bool OnCheckForProblems()
		{
			return this.CheckForProblemsInternal();
		}

		// Token: 0x06000C6D RID: 3181 RVA: 0x00059D02 File Offset: 0x00057F02
		protected override void OnSaveAsPrefab()
		{
			this.CheckForProblemsInternal();
		}

		// Token: 0x06000C6E RID: 3182 RVA: 0x00059D0B File Offset: 0x00057F0B
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 6;
		}

		// Token: 0x06000C6F RID: 3183 RVA: 0x00059D10 File Offset: 0x00057F10
		public void RefreshSailVisual()
		{
			int num = this.FetchSailLevel();
			this.AdjustLevelOfSail(num);
			this._currentSailLevelUsed = num;
		}

		// Token: 0x06000C70 RID: 3184 RVA: 0x00059D34 File Offset: 0x00057F34
		public void UpdateForcedWindOfSailsAndTopBanner(float dt, Vec3 globalBannerRelativeWindVelocity, in Vec3 sailRelativeGlobalWindVelocity, in Vec3 globalSailForce)
		{
			if (globalBannerRelativeWindVelocity.LengthSquared >= 100f)
			{
				globalBannerRelativeWindVelocity = globalBannerRelativeWindVelocity.NormalizedCopy() * 10f;
			}
			globalBannerRelativeWindVelocity /= Scene.MaximumWindSpeed;
			globalBannerRelativeWindVelocity *= this._captureTheFlagAnimation.BannerWindFactor;
			ClothSimulatorComponent sailTopBannerClothComponent = this.SailTopBannerClothComponent;
			if (sailTopBannerClothComponent != null)
			{
				sailTopBannerClothComponent.SetForcedWind(globalBannerRelativeWindVelocity, false);
			}
			Vec3 vec = globalSailForce;
			Vec3 vec2 = vec.RotateVectorToXYPlane().NormalizedCopy();
			vec = sailRelativeGlobalWindVelocity;
			Vec3 vec3 = vec.AsVec2.Length * vec2 * this._foldAnimWindReductionFactor;
			vec3 *= 2f;
			this._currentFrameGlobalWind = Vec3.Lerp(this._currentFrameGlobalWind, vec3, dt);
			if (this._currentFrameGlobalWind.LengthSquared >= 100f)
			{
				this._currentFrameGlobalWind = this._currentFrameGlobalWind.NormalizedCopy() * 10f;
			}
			this.SailClothComponent.SetForcedWind(this._currentFrameGlobalWind / Scene.MaximumWindSpeed, false);
		}

		// Token: 0x06000C71 RID: 3185 RVA: 0x00059E3D File Offset: 0x0005803D
		public void SetFoldSailDuration(float foldSailDuration)
		{
			this._foldSailDuration = foldSailDuration;
			this.UpdateTotalFoldDuration();
		}

		// Token: 0x06000C72 RID: 3186 RVA: 0x00059E4C File Offset: 0x0005804C
		public void SetFoldSailStepMultiplier(float foldSailStepMultiplier)
		{
			this._foldSailStepMultiplier = foldSailStepMultiplier;
			this.UpdateTotalFoldDuration();
		}

		// Token: 0x06000C73 RID: 3187 RVA: 0x00059E5B File Offset: 0x0005805B
		public void SetUnfoldSailDuration(float unfoldSailDuration)
		{
			this._unfoldSailDuration = unfoldSailDuration;
			this.UpdateTotalUnfoldDuration();
		}

		// Token: 0x06000C74 RID: 3188 RVA: 0x00059E6C File Offset: 0x0005806C
		public void SetSailEntityAlpha(float alpha)
		{
			this._sailEntityAlpha = alpha;
			base.GameEntity.SetAlpha(alpha);
		}

		// Token: 0x06000C75 RID: 3189 RVA: 0x00059E8F File Offset: 0x0005808F
		public void InstantCloseSails()
		{
			this.SailEnabled = false;
			this._ongoingAnimationData.FoldIsOngoing = true;
			this._ongoingAnimationData.CurrentProgress = this._foldSailDuration + this._foldFreeBoneResetDuration + this._foldedSailTransitionDuration;
			this._ongoingAnimationData.UnfoldIsOngoing = false;
		}

		// Token: 0x06000C76 RID: 3190 RVA: 0x00059ED0 File Offset: 0x000580D0
		private bool CheckForProblemsInternal()
		{
			bool flag = true;
			if (this.SailTopBannerClothComponent != null)
			{
				MetaMesh firstMetaMesh = this.SailTopBannerClothComponent.GetFirstMetaMesh();
				for (int i = 0; i < firstMetaMesh.MeshCount; i++)
				{
					Mesh meshAtIndex = firstMetaMesh.GetMeshAtIndex(i);
					if (meshAtIndex.HasCloth() && meshAtIndex.GetClothLinearVelocityMultiplier() != 0f)
					{
						string text = ((base.GameEntity.Root != base.GameEntity) ? (base.GameEntity.Root.Name + "|" + base.GameEntity.Name) : base.GameEntity.Name);
						string text2 = string.Concat(new string[] { "Top banner (", meshAtIndex.Name, ") of Sail Entity (", text, ") has non-zero linear velocity cloth parameter." });
						MBEditor.AddEntityWarning(this.SailTopBannerClothComponent.GetEntity(), text2);
						flag = false;
					}
				}
			}
			return flag;
		}

		// Token: 0x06000C77 RID: 3191 RVA: 0x00059FE0 File Offset: 0x000581E0
		private void PlaceKnobs()
		{
			string text = "";
			if (this._knobType == SailVisual.KnobTypeEnum.Bollard)
			{
				text = "bollard_a";
			}
			else if (this._knobType == SailVisual.KnobTypeEnum.Cleat)
			{
				text = "cleat_a";
			}
			else if (this._knobType == SailVisual.KnobTypeEnum.Belaying)
			{
				text = "belaying_pins_a";
			}
			List<WeakGameEntity> list = new List<WeakGameEntity>();
			List<WeakGameEntity> list2 = new List<WeakGameEntity>();
			foreach (SailVisual.FreeBoneRecord freeBoneRecord in this._freeBones)
			{
				if (freeBoneRecord.RotatorPulleys != null)
				{
					foreach (SailVisual.PulleyDataCache pulleyDataCache in freeBoneRecord.RotatorPulleys)
					{
						WeakGameEntity firstFixedEntity = pulleyDataCache.PulleySystem.FirstFixedEntity;
						if (firstFixedEntity.IsValid)
						{
							list2.Add(firstFixedEntity);
						}
					}
				}
				if (freeBoneRecord.StabilityPulleys != null)
				{
					foreach (SailVisual.PulleyDataCache pulleyDataCache2 in freeBoneRecord.StabilityPulleys)
					{
						WeakGameEntity firstFixedEntity2 = pulleyDataCache2.PulleySystem.FirstFixedEntity;
						if (firstFixedEntity2.IsValid)
						{
							list2.Add(firstFixedEntity2);
						}
					}
				}
				if (freeBoneRecord.StabilityRopes != null)
				{
					foreach (SailVisual.SimpleRopeRecord simpleRopeRecord in freeBoneRecord.StabilityRopes)
					{
						if (simpleRopeRecord.RopeEntity != null)
						{
							list2.Add(simpleRopeRecord.RopeEntity.WeakEntity);
						}
					}
				}
			}
			foreach (WeakGameEntity weakGameEntity in list2)
			{
				int num = weakGameEntity.ChildCount - 1;
				while (num >= 0 && num < weakGameEntity.ChildCount)
				{
					WeakGameEntity child = weakGameEntity.GetChild(num);
					if (!child.HasScriptComponent("rope_segment_cosmetics"))
					{
						weakGameEntity.RemoveChild(child, false, false, true, 37);
					}
					num--;
				}
				GameEntity gameEntity = GameEntity.Instantiate(base.GameEntity.Scene, text, true, true, "");
				if (gameEntity != null)
				{
					weakGameEntity.AddChild(gameEntity.WeakEntity, false);
					list.Clear();
					foreach (GameEntity gameEntity2 in gameEntity.GetChildren())
					{
						if (gameEntity2.HasTag("knot_point"))
						{
							list.Add(gameEntity2.WeakEntity);
						}
					}
					if (list.Count > 0)
					{
						MatrixFrame frame = list[MBRandom.RandomInt(list.Count)].GetFrame();
						frame.Fill();
						MatrixFrame matrixFrame = frame.Inverse();
						weakGameEntity.SetFrame(ref matrixFrame, true);
					}
					foreach (Mesh mesh in weakGameEntity.GetAllMeshesWithTag("auto_factor_color"))
					{
						mesh.Color = this._placeKnobColor.ToUnsignedInteger();
					}
				}
			}
		}

		// Token: 0x06000C78 RID: 3192 RVA: 0x0005A3A4 File Offset: 0x000585A4
		private void SetKnobColors()
		{
			List<WeakGameEntity> list = new List<WeakGameEntity>();
			foreach (SailVisual.FreeBoneRecord freeBoneRecord in this._freeBones)
			{
				if (freeBoneRecord.RotatorPulleys != null)
				{
					foreach (SailVisual.PulleyDataCache pulleyDataCache in freeBoneRecord.RotatorPulleys)
					{
						WeakGameEntity firstFixedEntity = pulleyDataCache.PulleySystem.FirstFixedEntity;
						if (firstFixedEntity != null)
						{
							list.Add(firstFixedEntity);
						}
					}
				}
				if (freeBoneRecord.StabilityPulleys != null)
				{
					foreach (SailVisual.PulleyDataCache pulleyDataCache2 in freeBoneRecord.StabilityPulleys)
					{
						WeakGameEntity firstFixedEntity2 = pulleyDataCache2.PulleySystem.FirstFixedEntity;
						if (firstFixedEntity2 != null)
						{
							list.Add(firstFixedEntity2);
						}
					}
				}
				if (freeBoneRecord.StabilityRopes != null)
				{
					foreach (SailVisual.SimpleRopeRecord simpleRopeRecord in freeBoneRecord.StabilityRopes)
					{
						if (simpleRopeRecord.RopeEntity != null)
						{
							list.Add(simpleRopeRecord.RopeEntity.WeakEntity);
						}
					}
				}
			}
			foreach (WeakGameEntity weakGameEntity in list)
			{
				foreach (Mesh mesh in weakGameEntity.GetAllMeshesWithTag("auto_factor_color"))
				{
					mesh.Color = this._placeKnobColor.ToUnsignedInteger();
				}
			}
		}

		// Token: 0x06000C79 RID: 3193 RVA: 0x0005A5F4 File Offset: 0x000587F4
		private int FetchSailLevel()
		{
			int num = -1;
			WeakGameEntity firstChildEntityWithTagRecursive = base.GameEntity.GetFirstChildEntityWithTagRecursive("upgrade_slot");
			if (firstChildEntityWithTagRecursive != null)
			{
				using (IEnumerator<WeakGameEntity> enumerator = firstChildEntityWithTagRecursive.GetChildren().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						WeakGameEntity weakGameEntity = enumerator.Current;
						if (weakGameEntity.GetVisibilityExcludeParents())
						{
							if (weakGameEntity.HasTag("base"))
							{
								if (num != -1)
								{
									return -1;
								}
								num = 1;
							}
							else if (weakGameEntity.HasTag("lvl2"))
							{
								if (num != -1)
								{
									return -1;
								}
								num = 2;
							}
							else if (weakGameEntity.HasTag("lvl3"))
							{
								if (num != -1)
								{
									return -1;
								}
								num = 3;
							}
						}
					}
					return num;
				}
				return 1;
			}
			return 1;
		}

		// Token: 0x06000C7A RID: 3194 RVA: 0x0005A6BC File Offset: 0x000588BC
		private void CheckClothResetTimer()
		{
			if (this._resetClothMeshFrameCounter > 0)
			{
				this._resetClothMeshFrameCounter--;
				if (this._resetClothMeshFrameCounter == 0 && this.SailClothComponent != null)
				{
					this.SailClothComponent.SetResetRequired();
				}
			}
		}

		// Token: 0x06000C7B RID: 3195 RVA: 0x0005A6F8 File Offset: 0x000588F8
		private void SetSailMaterialWrtLevel(Mesh mesh, int sailLevel, bool isEditorScene)
		{
			if (sailLevel == -1 && !isEditorScene)
			{
				return;
			}
			List<string> list = new List<string>();
			WeakGameEntity root = base.GameEntity.Root;
			int num = 0;
			if (isEditorScene)
			{
				num += (int)(this._cumulativeDt * 5f);
			}
			else
			{
				ShipVisual shipVisual = this.ShipVisual;
				num = ((shipVisual != null) ? shipVisual.Seed : ((int)((ulong)root.Pointer & (ulong)(-1))));
			}
			float num2 = 1f;
			if (this.ShipVisual != null)
			{
				num2 = this.ShipVisual.Health;
			}
			else if (isEditorScene)
			{
				num2 = this._editorOnlyShipHealth;
			}
			Material material = null;
			if (this.ShipVisual != null && !string.IsNullOrEmpty(this.ShipVisual.CustomSailPatternId))
			{
				material = Material.GetFromResource(this.ShipVisual.CustomSailPatternId);
			}
			if (material == null)
			{
				Random random = new Random(num);
				if (this._sailType == SailVisual.SailType.SquareSail)
				{
					if (sailLevel == 1)
					{
						list.Add("00");
					}
					else if (sailLevel == 2)
					{
						list.Add("04");
						list.Add("05");
						list.Add("06");
						list.Add("10");
					}
					else if (sailLevel == 3)
					{
						list.Add("01");
						list.Add("02");
						list.Add("03");
						list.Add("07");
						list.Add("08");
						list.Add("09");
						list.Add("11");
					}
				}
				else if (sailLevel == 1)
				{
					list.Add("00");
				}
				else if (sailLevel == 2)
				{
					list.Add("04");
					list.Add("06");
				}
				else if (sailLevel == 3)
				{
					list.Add("01");
					list.Add("02");
					list.Add("03");
					list.Add("05");
					list.Add("07");
					list.Add("08");
					list.Add("09");
				}
				string text = "generated_";
				text += ((this._sailType == SailVisual.SailType.SquareSail) ? "square_" : "lateen_");
				if (sailLevel == 1)
				{
					if (num2 > 0.75f)
					{
						text += "l1_h4_";
					}
					else if (num2 > 0.5f)
					{
						text += "l1_h3_";
					}
					else if (num2 > 0.25f)
					{
						text += "l1_h2_";
					}
					else
					{
						text += "l1_h1_";
					}
				}
				else if (num2 > 0.75f)
				{
					text += "_h4_";
				}
				else if (num2 > 0.5f)
				{
					text += "_h3_";
				}
				else if (num2 > 0.25f)
				{
					text += "_h2_";
				}
				else
				{
					text += "_h1_";
				}
				if (list.Count > 0)
				{
					text += list[random.Next(list.Count)];
				}
				material = Material.GetFromResource(text);
			}
			if (mesh.HasTag("faction_color"))
			{
				if (material != null)
				{
					mesh.SetMaterial(material);
				}
				if (this.ShipVisual != null)
				{
					mesh.Color = this.ShipVisual.SailColors.Item1;
					mesh.Color2 = this.ShipVisual.SailColors.Item2;
				}
			}
		}

		// Token: 0x06000C7C RID: 3196 RVA: 0x0005AA48 File Offset: 0x00058C48
		private void AdjustSquareSailSpecificLevelData(int sailLevel, bool isEditorScene)
		{
			bool flag = sailLevel == 3;
			if ((flag && this._currentSailLevelUsed != 3) || (!flag && this._currentSailLevelUsed == 3))
			{
				float num = this._squareLvl3MastShift;
				if (this._currentSailLevelUsed == 3 && !flag)
				{
					num *= -1f;
				}
				List<WeakGameEntity> list = new List<WeakGameEntity>();
				base.GameEntity.GetChildrenWithTagRecursive(list, "lvl3_shift_entity");
				foreach (WeakGameEntity weakGameEntity in list)
				{
					MatrixFrame frame = weakGameEntity.GetFrame();
					frame.origin.z = frame.origin.z + num;
					weakGameEntity.SetLocalFrame(ref frame, true);
				}
			}
			List<WeakGameEntity> list2 = new List<WeakGameEntity>();
			base.GameEntity.GetChildrenWithTagRecursive(list2, "lvl3_lateens");
			foreach (WeakGameEntity weakGameEntity2 in list2)
			{
				weakGameEntity2.SetDoNotCheckVisibility(true);
				weakGameEntity2.SetVisibilityExcludeParents(flag);
			}
			foreach (WeakGameEntity weakGameEntity3 in this._topLateenSails)
			{
				weakGameEntity3.SetDoNotCheckVisibility(true);
				foreach (Mesh mesh in weakGameEntity3.GetAllMeshesWithTag("faction_color"))
				{
					this.SetSailMaterialWrtLevel(mesh, sailLevel, isEditorScene);
				}
			}
			foreach (WeakGameEntity weakGameEntity4 in this._topLateenFoldedSails)
			{
				weakGameEntity4.SetDoNotCheckVisibility(true);
				foreach (Mesh mesh2 in weakGameEntity4.GetAllMeshesWithTag("faction_color"))
				{
					this.SetSailMaterialWrtLevel(mesh2, sailLevel, isEditorScene);
				}
			}
		}

		// Token: 0x06000C7D RID: 3197 RVA: 0x0005AC94 File Offset: 0x00058E94
		private void AdjustLevelOfSail(int sailLevel)
		{
			if (this._sailSkeleton == null)
			{
				return;
			}
			bool flag = base.GameEntity.Scene.IsEditorScene();
			foreach (Mesh mesh in this._sailSkeleton.GetAllMeshes())
			{
				if (mesh.HasTag("faction_color"))
				{
					this.SetSailMaterialWrtLevel(mesh, sailLevel, flag);
				}
			}
			if (this._sailType == SailVisual.SailType.SquareSail)
			{
				this.AdjustSquareSailSpecificLevelData(sailLevel, flag);
			}
			if (this._foldedStaticSailEntity != null)
			{
				foreach (Mesh mesh2 in this._foldedStaticSailEntity.GetAllMeshesWithTag("faction_color"))
				{
					this.SetSailMaterialWrtLevel(mesh2, sailLevel, flag);
				}
			}
		}

		// Token: 0x06000C7E RID: 3198 RVA: 0x0005AD84 File Offset: 0x00058F84
		private void ApplyRandomWindToRope(ref Vec3 position, float factor)
		{
			Vec3 vec;
			vec..ctor((float)Math.Cos((double)(position.x * 2.5f + this._cumulativeDt * 4.5f)), (float)Math.Cos((double)(position.y * 1.2f + this._cumulativeDt * 6.5f)), (float)Math.Cos((double)(position.z * 3.5f + this._cumulativeDt * 3.5f)), -1f);
			position += vec * 0.1f * factor;
		}

		// Token: 0x06000C7F RID: 3199 RVA: 0x0005AE20 File Offset: 0x00059020
		private void SetButtomRopePositions(float dt, bool disableWind)
		{
			Vec2 globalWindVelocityOfScene = base.GameEntity.GetGlobalWindVelocityOfScene();
			float num = MathF.Min(globalWindVelocityOfScene.Normalize(), 8f);
			float num2 = MathF.Min(num, 4f);
			float num3 = (float)Math.Pow((double)(num / 8f), 0.44999998807907104) * 8f;
			num2 = (float)Math.Pow((double)(num / 4f), 0.44999998807907104) * 4f;
			Vec3 vec;
			vec..ctor(globalWindVelocityOfScene, 0f, -1f);
			MatrixFrame globalFrame = this.SailSkeletonEntity.GetGlobalFrame();
			globalFrame.rotation.Orthonormalize();
			Vec3 vec2 = globalFrame.rotation.TransformToLocal(ref vec);
			if (vec2.Length > 0f)
			{
				vec2.Normalize();
			}
			if (this._yardEntity != null)
			{
				Vec3 f = this._yardEntity.GetGlobalFrame().rotation.f;
				f.Normalize();
				float num4 = MathF.Clamp(Vec3.DotProduct(vec, f), 0f, 1f);
				num3 *= 0.5f + 0.5f * num4;
				num2 *= 0.5f + 0.5f * num4;
			}
			float num5 = 0f;
			if (this._ongoingAnimationData.FoldIsOngoing)
			{
				num5 = this._ongoingAnimationData.CurrentProgress / this._foldFreeBoneResetDuration;
				num5 = MathF.Clamp(num5, 0f, 1f);
				num3 = MathF.Lerp(num3, 0f, num5, 1E-05f);
			}
			if (this._ongoingAnimationData.UnfoldIsOngoing)
			{
				num5 = MathF.Clamp((this._ongoingAnimationData.CurrentProgress - (this._unfoldSailDuration + this._foldedSailTransitionDuration)) / this._foldFreeBoneResetDuration, 0f, 1f);
				num3 = MathF.Lerp(0f, num3, num5, 1E-05f);
			}
			Vec3 vec3 = -Vec3.Up;
			Vec3 vec4 = globalFrame.rotation.TransformToLocal(ref vec3);
			vec4.Normalize();
			foreach (SailVisual.FreeBoneRecord freeBoneRecord in this._freeBones)
			{
				MatrixFrame initialLocalFrame = freeBoneRecord.InitialLocalFrame;
				Vec3 vec5 = freeBoneRecord.InitialLocalFrame.origin;
				if (!disableWind && freeBoneRecord.BoneIndex != -1)
				{
					initialLocalFrame.origin += vec2 * num3 * 0.07f;
					if (this._sailType == SailVisual.SailType.SquareSail)
					{
						num2 = MathF.Lerp(num2, 0f, num5, 1E-05f);
						initialLocalFrame.origin += vec4 * num2 * 0.08f;
					}
					vec5 = initialLocalFrame.origin;
					this.ApplyRandomWindToRope(ref initialLocalFrame.origin, 0.1f);
				}
				if (freeBoneRecord.BoneIndex != -1)
				{
					bool flag = false;
					if (this._ongoingAnimationData.FoldIsOngoing && this._ongoingAnimationData.CurrentProgress > this._foldFreeBoneResetDuration)
					{
						flag = true;
					}
					else if (this._ongoingAnimationData.UnfoldIsOngoing && this._ongoingAnimationData.CurrentProgress < this._unfoldSailDuration)
					{
						flag = true;
					}
					else if (this._sailType == SailVisual.SailType.LateenSail && this._lateenSailData.RollRotationInProgress)
					{
						flag = true;
					}
					if (flag)
					{
						if (this._sailType == SailVisual.SailType.SquareSail)
						{
							if (freeBoneRecord.BoneType == SailVisual.FreeBoneType.Left)
							{
								initialLocalFrame.origin = this._ongoingAnimationData.CurrentLeftFreeBonePosition;
							}
							else
							{
								initialLocalFrame.origin = this._ongoingAnimationData.CurrentRightFreeBonePosition;
							}
						}
						else if (freeBoneRecord.BoneType == SailVisual.FreeBoneType.Left)
						{
							initialLocalFrame.origin = this._ongoingAnimationData.CurrentLeftFreeBonePosition;
						}
						else if (freeBoneRecord.BoneType == SailVisual.FreeBoneType.Right)
						{
							initialLocalFrame.origin = this._ongoingAnimationData.CurrentRightFreeBonePosition;
						}
						else
						{
							initialLocalFrame.origin = this._ongoingAnimationData.CurrentCenterFreeBonePosition;
						}
						vec5 = initialLocalFrame.origin;
					}
				}
				freeBoneRecord.CurrentLocalFrame = initialLocalFrame;
				freeBoneRecord.CurrentFrameWithoutRandomWind = vec5;
			}
		}

		// Token: 0x06000C80 RID: 3200 RVA: 0x0005B234 File Offset: 0x00059434
		private void FoldUnfoldSoundEventTick()
		{
			if (this._ongoingAnimationData.FoldUnfoldSoundEvent != null && this._ongoingAnimationData.FoldUnfoldSoundEvent.IsPlaying())
			{
				this._ongoingAnimationData.FoldUnfoldSoundEvent.SetPosition(base.GameEntity.GetGlobalFrame().origin);
			}
			if (this._ongoingAnimationData.ShouldMakeFoldUnfoldSound)
			{
				int num = (this._ongoingAnimationData.UnfoldIsOngoing ? SailVisual.SailUnfoldSoundEventId : SailVisual.SailFoldSoundEventId);
				if (this._ongoingAnimationData.FoldUnfoldSoundEvent != null)
				{
					this._ongoingAnimationData.FoldUnfoldSoundEvent.Stop();
					this._ongoingAnimationData.FoldUnfoldSoundEvent = null;
				}
				this._ongoingAnimationData.ShouldMakeFoldUnfoldSound = false;
				this._ongoingAnimationData.FoldUnfoldSoundEvent = SoundEvent.CreateEvent(num, base.GameEntity.Scene);
				this._ongoingAnimationData.FoldUnfoldSoundEvent.SetPosition(base.GameEntity.GetGlobalFrame().origin);
				this._ongoingAnimationData.FoldUnfoldSoundEvent.Play();
			}
			if (this._ongoingAnimationData.ShouldStopFoldUnfoldSound)
			{
				this._ongoingAnimationData.FoldUnfoldSoundEvent.Stop();
				this._ongoingAnimationData.FoldUnfoldSoundEvent = null;
				this._ongoingAnimationData.ShouldStopFoldUnfoldSound = false;
			}
		}

		// Token: 0x06000C81 RID: 3201 RVA: 0x0005B36C File Offset: 0x0005956C
		private void TickRopesAndPulleys()
		{
			MatrixFrame matrixFrame = ((this._yardEntity != null) ? this._yardEntity.GetGlobalFrame() : MatrixFrame.Identity);
			Vec2 globalWindVelocityOfScene = base.GameEntity.GetGlobalWindVelocityOfScene();
			globalWindVelocityOfScene.Normalize();
			bool flag = false;
			MatrixFrame globalFrame = base.GameEntity.Root.GetGlobalFrame();
			flag = Vec2.DotProduct(globalWindVelocityOfScene, matrixFrame.rotation.f.AsVec2) < 0f;
			if (this._knobParent != null)
			{
				MatrixFrame globalFrame2 = this._knobParent.GetGlobalFrame();
				for (int i = 0; i < this._knobConnectionPoints.Count; i++)
				{
					SailVisual.KnobConnectionPoint knobConnectionPoint = this._knobConnectionPoints[i];
					Vec3 vec = globalFrame2.TransformToParent(ref knobConnectionPoint.ShipLocalPosition);
					knobConnectionPoint.UpdateGlobalPosition(vec);
					bool flag2 = Vec3.DotProduct(vec - matrixFrame.origin, matrixFrame.rotation.f) > 0f;
					knobConnectionPoint.UpdateRightOfYard(flag2);
					this._knobConnectionPoints[i] = knobConnectionPoint;
				}
			}
			if (this.SailSkeletonEntity != null && this._yardEntity != null)
			{
				if (!this.SailYawRotationEntity.GetLocalFrame().NearlyEquals(this._previousYawEntityFrame, 0.0001f))
				{
					MatrixFrame globalFrame3 = base.GameEntity.GetGlobalFrame();
					MatrixFrame matrixFrame2 = globalFrame3.TransformToLocalNonOrthogonal(ref matrixFrame);
					foreach (SailVisual.SimpleRopeRecord simpleRopeRecord in this._simpleRopes)
					{
						if (simpleRopeRecord.StartPointAttachedToYard)
						{
							MatrixFrame globalFrame4 = simpleRopeRecord.RopeEntity.GetGlobalFrame();
							Vec3 vec2 = globalFrame3.TransformToLocalNonOrthogonal(ref globalFrame4.origin);
							Vec3 vec3 = this._previousSailYardFrame.TransformToLocalNonOrthogonal(ref vec2);
							globalFrame4.origin = matrixFrame2.TransformToParent(ref vec3);
							globalFrame4.origin = globalFrame3.TransformToParent(ref globalFrame4.origin);
							simpleRopeRecord.RopeEntity.SetGlobalFrame(ref globalFrame4, false);
						}
						if (simpleRopeRecord.EndPointAttachedToYard)
						{
							MatrixFrame globalFrame5 = simpleRopeRecord.TargetEntity.GetGlobalFrame();
							Vec3 vec4 = globalFrame3.TransformToLocalNonOrthogonal(ref globalFrame5.origin);
							Vec3 vec5 = this._previousSailYardFrame.TransformToLocalNonOrthogonal(ref vec4);
							globalFrame5.origin = matrixFrame2.TransformToParent(ref vec5);
							globalFrame5.origin = globalFrame3.TransformToParent(ref globalFrame5.origin);
							simpleRopeRecord.TargetEntity.SetGlobalFrame(ref globalFrame5, false);
						}
					}
					foreach (SailVisual.FreeBoneRecord freeBoneRecord in this._freeBones)
					{
						if (freeBoneRecord.FoldSailPulley.PulleySystem != null)
						{
							foreach (RopeSegment ropeSegment in freeBoneRecord.FoldSailPulley.PulleySystem.TiedToYardSegments)
							{
								MatrixFrame globalFrame6 = ropeSegment.GameEntity.GetGlobalFrame();
								Vec3 vec6 = globalFrame3.TransformToLocalNonOrthogonal(ref globalFrame6.origin);
								Vec3 vec7 = this._previousSailYardFrame.TransformToLocalNonOrthogonal(ref vec6);
								Vec3 vec8 = matrixFrame2.TransformToParent(ref vec7);
								globalFrame6.origin = globalFrame3.TransformToParent(ref vec8);
								ropeSegment.GameEntity.SetGlobalFrame(ref globalFrame6, false);
							}
						}
					}
					this._previousSailYardFrame = matrixFrame2;
				}
				bool flag3 = !this._ongoingAnimationData.FoldIsOngoing && !this._ongoingAnimationData.UnfoldIsOngoing;
				MatrixFrame globalFrame7 = this.SailSkeletonEntity.GetGlobalFrame();
				foreach (SailVisual.FreeBoneRecord freeBoneRecord2 in this._freeBones)
				{
					Vec3 origin = globalFrame7.TransformToParent(ref freeBoneRecord2.CurrentLocalFrame).origin;
					Vec3 vec9 = globalFrame7.TransformToParent(ref freeBoneRecord2.CurrentFrameWithoutRandomWind);
					Vec3 vec10 = globalFrame.TransformToLocalNonOrthogonal(ref origin);
					if (freeBoneRecord2.ConnectionType != SailVisual.FreeBoneConnectionType.Closest)
					{
						bool flag4 = freeBoneRecord2.ConnectionType == SailVisual.FreeBoneConnectionType.ClosestTwo;
					}
					if (freeBoneRecord2.FoldSailPulley.PulleySystem != null)
					{
						freeBoneRecord2.FoldSailPulley.PulleySystem.SetEndTargetPosition(origin);
						if (this._ongoingAnimationData.FoldIsOngoing)
						{
							float num = MathF.Min(this._ongoingAnimationData.CurrentProgress / this._foldFreeBoneResetDuration, 1f);
							freeBoneRecord2.FoldSailPulley.PulleySystem.SetRuntimeLooseMultiplier(1f - num);
						}
						else if (this._ongoingAnimationData.UnfoldIsOngoing)
						{
							float num2 = MathF.Clamp((this._ongoingAnimationData.CurrentProgress - this._unfoldSailDuration) / this._foldFreeBoneResetDuration, 0f, 1f);
							freeBoneRecord2.FoldSailPulley.PulleySystem.SetRuntimeLooseMultiplier(1f - num2);
						}
						else
						{
							freeBoneRecord2.FoldSailPulley.PulleySystem.SetRuntimeLooseMultiplier(1f);
						}
					}
					if (freeBoneRecord2.RotatorPulleys != null)
					{
						foreach (SailVisual.PulleyDataCache pulleyDataCache in freeBoneRecord2.RotatorPulleys)
						{
							if (pulleyDataCache.PulleySystem != null)
							{
								pulleyDataCache.PulleySystem.SetEndTargetPosition(origin);
							}
						}
						if (this._knobConnectionPoints.Count > 1 && flag3)
						{
							if (freeBoneRecord2.RotatorPulleys.Count > 0)
							{
								ValueTuple<int, int> valueTuple = this.FindClosestTwoKnobPoint(vec9, vec10, this._knobConnectionPoints, true);
								if (valueTuple.Item1 != -1)
								{
									freeBoneRecord2.RotatorPulleys[0].PulleySystem.SetFirstFixedGlobalPosition(this._knobConnectionPoints[valueTuple.Item1].GlobalPosition);
								}
								else
								{
									int num3 = this.FindClosestPointFallback(vec9, this._knobConnectionPoints);
									if (num3 != -1)
									{
										freeBoneRecord2.RotatorPulleys[0].PulleySystem.SetFirstFixedGlobalPosition(this._knobConnectionPoints[num3].GlobalPosition);
									}
								}
								if (valueTuple.Item2 != -1)
								{
									freeBoneRecord2.RotatorPulleys[0].PulleySystem.SetFirstFreeGlobalPosition(this._knobConnectionPoints[valueTuple.Item2].GlobalPosition);
								}
								else
								{
									int num4 = this.FindClosestPointFallback(vec9, this._knobConnectionPoints);
									if (num4 != -1)
									{
										freeBoneRecord2.RotatorPulleys[0].PulleySystem.SetFirstFreeGlobalPosition(this._knobConnectionPoints[num4].GlobalPosition);
									}
								}
							}
							if (freeBoneRecord2.RotatorPulleys.Count > 1)
							{
								ValueTuple<int, int> valueTuple2 = this.FindClosestTwoKnobPoint(vec9, vec10, this._knobConnectionPoints, false);
								if (valueTuple2.Item1 != -1)
								{
									freeBoneRecord2.RotatorPulleys[1].PulleySystem.SetFirstFixedGlobalPosition(this._knobConnectionPoints[valueTuple2.Item1].GlobalPosition);
								}
								else
								{
									int num5 = this.FindClosestPointFallback(vec9, this._knobConnectionPoints);
									if (num5 != -1)
									{
										freeBoneRecord2.RotatorPulleys[1].PulleySystem.SetFirstFixedGlobalPosition(this._knobConnectionPoints[num5].GlobalPosition);
									}
								}
								if (valueTuple2.Item2 != -1)
								{
									freeBoneRecord2.RotatorPulleys[1].PulleySystem.SetFirstFreeGlobalPosition(this._knobConnectionPoints[valueTuple2.Item2].GlobalPosition);
								}
								else
								{
									int num6 = this.FindClosestPointFallback(vec9, this._knobConnectionPoints);
									if (num6 != -1)
									{
										freeBoneRecord2.RotatorPulleys[1].PulleySystem.SetFirstFreeGlobalPosition(this._knobConnectionPoints[num6].GlobalPosition);
									}
								}
								int num7 = (flag ? 0 : 1);
								freeBoneRecord2.RotatorPulleys[num7].PulleySystem.SetRuntimeLooseMultiplier(0.0023f);
								freeBoneRecord2.RotatorPulleys[(num7 + 1) % 2].PulleySystem.SetRuntimeLooseMultiplier(0.1f);
							}
						}
					}
					if (freeBoneRecord2.StabilityPulleys != null)
					{
						foreach (SailVisual.PulleyDataCache pulleyDataCache2 in freeBoneRecord2.StabilityPulleys)
						{
							if (pulleyDataCache2.PulleySystem != null)
							{
								pulleyDataCache2.PulleySystem.SetEndTargetPosition(vec9);
								if (this._ongoingAnimationData.FoldIsOngoing)
								{
									float num8 = MathF.Clamp(this._ongoingAnimationData.CurrentProgress / (this._foldFreeBoneResetDuration + this._foldSailDuration), 0f, 1f);
									pulleyDataCache2.PulleySystem.SetRuntimeLooseMultiplier(0.5f * num8);
								}
								else if (this._ongoingAnimationData.UnfoldIsOngoing)
								{
									float num9 = MathF.Clamp(this._ongoingAnimationData.CurrentProgress / (this._foldFreeBoneResetDuration + this._unfoldSailDuration), 0f, 1f);
									pulleyDataCache2.PulleySystem.SetRuntimeLooseMultiplier(0.5f * (1f - num9));
								}
								else
								{
									pulleyDataCache2.PulleySystem.SetRuntimeLooseMultiplier(0.05f);
								}
							}
						}
						if (this._knobConnectionPoints.Count > 1 && flag3)
						{
							if (freeBoneRecord2.StabilityPulleys.Count > 0)
							{
								ValueTuple<int, int> valueTuple3 = this.FindClosestTwoKnobPoint(vec9, vec10, this._knobConnectionPoints, true);
								if (valueTuple3.Item1 != -1)
								{
									freeBoneRecord2.StabilityPulleys[0].PulleySystem.SetFirstFixedGlobalPosition(this._knobConnectionPoints[valueTuple3.Item1].GlobalPosition);
								}
								else
								{
									int num10 = this.FindClosestPointFallback(vec9, this._knobConnectionPoints);
									if (num10 != -1)
									{
										freeBoneRecord2.StabilityPulleys[0].PulleySystem.SetFirstFixedGlobalPosition(this._knobConnectionPoints[num10].GlobalPosition);
									}
								}
								if (valueTuple3.Item2 != -1)
								{
									freeBoneRecord2.StabilityPulleys[0].PulleySystem.SetFirstFreeGlobalPosition(this._knobConnectionPoints[valueTuple3.Item2].GlobalPosition);
								}
								else
								{
									int num11 = this.FindClosestPointFallback(vec9, this._knobConnectionPoints);
									if (num11 != -1)
									{
										freeBoneRecord2.StabilityPulleys[0].PulleySystem.SetFirstFreeGlobalPosition(this._knobConnectionPoints[num11].GlobalPosition);
									}
								}
							}
							if (freeBoneRecord2.StabilityPulleys.Count > 1)
							{
								ValueTuple<int, int> valueTuple4 = this.FindClosestTwoKnobPoint(vec9, vec10, this._knobConnectionPoints, false);
								if (valueTuple4.Item1 != -1)
								{
									freeBoneRecord2.StabilityPulleys[1].PulleySystem.SetFirstFixedGlobalPosition(this._knobConnectionPoints[valueTuple4.Item1].GlobalPosition);
								}
								else
								{
									int num12 = this.FindClosestPointFallback(vec9, this._knobConnectionPoints);
									if (num12 != -1)
									{
										freeBoneRecord2.StabilityPulleys[1].PulleySystem.SetFirstFixedGlobalPosition(this._knobConnectionPoints[num12].GlobalPosition);
									}
								}
								if (valueTuple4.Item2 != -1)
								{
									freeBoneRecord2.StabilityPulleys[1].PulleySystem.SetFirstFreeGlobalPosition(this._knobConnectionPoints[valueTuple4.Item2].GlobalPosition);
								}
								else
								{
									int num13 = this.FindClosestPointFallback(vec9, this._knobConnectionPoints);
									if (num13 != -1)
									{
										freeBoneRecord2.StabilityPulleys[1].PulleySystem.SetFirstFreeGlobalPosition(this._knobConnectionPoints[num13].GlobalPosition);
									}
								}
								int num14 = (flag ? 0 : 1);
								freeBoneRecord2.StabilityPulleys[num14].PulleySystem.SetRuntimeLooseMultiplier(0.0023f);
								freeBoneRecord2.StabilityPulleys[(num14 + 1) % 2].PulleySystem.SetRuntimeLooseMultiplier(0.1f);
							}
						}
					}
					if (freeBoneRecord2.StabilityRopes != null)
					{
						foreach (SailVisual.SimpleRopeRecord simpleRopeRecord2 in freeBoneRecord2.StabilityRopes)
						{
							MatrixFrame globalFrame8 = simpleRopeRecord2.TargetEntity.GetGlobalFrame();
							globalFrame8.origin = origin;
							simpleRopeRecord2.TargetEntity.SetGlobalFrame(ref globalFrame8, false);
						}
						if (this._knobConnectionPoints.Count > 0 && flag3)
						{
							if (freeBoneRecord2.StabilityRopes.Count > 0)
							{
								int num15 = this.FindClosestKnobPoint(vec9, vec10, this._knobConnectionPoints, true);
								if (num15 != -1)
								{
									MatrixFrame globalFrame9 = freeBoneRecord2.StabilityRopes[0].RopeEntity.GetGlobalFrame();
									globalFrame9.origin = this._knobConnectionPoints[num15].GlobalPosition;
									freeBoneRecord2.StabilityRopes[0].RopeEntity.SetGlobalFrame(ref globalFrame9, true);
								}
								else
								{
									int num16 = this.FindClosestPointFallback(vec9, this._knobConnectionPoints);
									if (num16 != -1)
									{
										MatrixFrame globalFrame10 = freeBoneRecord2.StabilityRopes[0].RopeEntity.GetGlobalFrame();
										globalFrame10.origin = this._knobConnectionPoints[num16].GlobalPosition;
										freeBoneRecord2.StabilityRopes[0].RopeEntity.SetGlobalFrame(ref globalFrame10, true);
									}
								}
							}
							if (freeBoneRecord2.StabilityRopes.Count > 1)
							{
								int num17 = this.FindClosestKnobPoint(vec9, vec10, this._knobConnectionPoints, false);
								if (num17 != -1)
								{
									MatrixFrame globalFrame11 = freeBoneRecord2.StabilityRopes[1].RopeEntity.GetGlobalFrame();
									globalFrame11.origin = this._knobConnectionPoints[num17].GlobalPosition;
									freeBoneRecord2.StabilityRopes[1].RopeEntity.SetGlobalFrame(ref globalFrame11, true);
								}
								else
								{
									int num18 = this.FindClosestPointFallback(vec9, this._knobConnectionPoints);
									if (num18 != -1)
									{
										MatrixFrame globalFrame12 = freeBoneRecord2.StabilityRopes[1].RopeEntity.GetGlobalFrame();
										globalFrame12.origin = this._knobConnectionPoints[num18].GlobalPosition;
										freeBoneRecord2.StabilityRopes[1].RopeEntity.SetGlobalFrame(ref globalFrame12, true);
									}
								}
								int num19 = (flag ? 0 : 1);
								freeBoneRecord2.StabilityRopes[num19].RopeSegment.SetRuntimeLooseMultiplier(0.005f);
								freeBoneRecord2.StabilityRopes[(num19 + 1) % 2].RopeSegment.SetRuntimeLooseMultiplier(0.2f);
							}
						}
					}
				}
			}
			this.UpdatePreviousYardFrame();
		}

		// Token: 0x06000C82 RID: 3202 RVA: 0x0005C1F0 File Offset: 0x0005A3F0
		private int FindClosestPointFallback(Vec3 position, List<SailVisual.KnobConnectionPoint> records)
		{
			int num = -1;
			float num2 = 1E+12f;
			for (int i = 0; i < records.Count; i++)
			{
				float lengthSquared = (position - records[i].GlobalPosition).AsVec2.LengthSquared;
				if (lengthSquared < num2)
				{
					num = i;
					num2 = lengthSquared;
				}
			}
			return num;
		}

		// Token: 0x06000C83 RID: 3203 RVA: 0x0005C248 File Offset: 0x0005A448
		private int FindClosestKnobPointWind(Vec3 position, Vec3 shipLocalPosition, List<SailVisual.KnobConnectionPoint> records, bool sideOfYard, Vec2 windDirection)
		{
			int num = -1;
			float num2 = 0f;
			for (int i = 0; i < records.Count; i++)
			{
				if (records[i].RightOfYard == sideOfYard && (MathF.Sign(records[i].ShipLocalPosition.x) == MathF.Sign(shipLocalPosition.x) || this._sailType == SailVisual.SailType.LateenSail))
				{
					Vec3 vec = position - records[i].GlobalPosition;
					float length = vec.AsVec2.Length;
					vec.Normalize();
					float num3 = MathF.Abs(Vec2.DotProduct(vec.AsVec2, windDirection));
					if (num3 > num2 && length < this._ropeConnectionMaxDistance)
					{
						num = i;
						num2 = num3;
					}
				}
			}
			return num;
		}

		// Token: 0x06000C84 RID: 3204 RVA: 0x0005C30C File Offset: 0x0005A50C
		private ValueTuple<int, int> FindClosestTwoKnobPointWind(Vec3 position, Vec3 shipLocalPosition, List<SailVisual.KnobConnectionPoint> records, bool sideOfYard, Vec2 windDirection)
		{
			ValueTuple<int, int> valueTuple = new ValueTuple<int, int>(-1, -1);
			ValueTuple<float, float> valueTuple2 = new ValueTuple<float, float>(0f, 0f);
			for (int i = 0; i < records.Count; i++)
			{
				if (records[i].RightOfYard == sideOfYard && (MathF.Sign(records[i].ShipLocalPosition.x) == MathF.Sign(shipLocalPosition.x) || this._sailType == SailVisual.SailType.LateenSail))
				{
					Vec3 vec = position - records[i].GlobalPosition;
					float length = vec.AsVec2.Length;
					vec.Normalize();
					float num = MathF.Abs(Vec2.DotProduct(vec.AsVec2, windDirection));
					if (length < this._ropeConnectionMaxDistance)
					{
						if (num > valueTuple2.Item1)
						{
							valueTuple2.Item2 = valueTuple2.Item1;
							valueTuple.Item2 = valueTuple.Item1;
							valueTuple2.Item1 = num;
							valueTuple.Item1 = i;
						}
						else if (num > valueTuple2.Item2)
						{
							valueTuple2.Item2 = num;
							valueTuple.Item2 = i;
						}
					}
				}
			}
			return valueTuple;
		}

		// Token: 0x06000C85 RID: 3205 RVA: 0x0005C428 File Offset: 0x0005A628
		private int FindClosestKnobPoint(Vec3 position, Vec3 shipLocalPosition, List<SailVisual.KnobConnectionPoint> records, bool sideOfYard)
		{
			float num = this._ropeConnectionMaxDistance * this._ropeConnectionMaxDistance;
			int num2 = -1;
			float num3 = 1E+12f;
			for (int i = 0; i < records.Count; i++)
			{
				if (records[i].RightOfYard == sideOfYard && (MathF.Sign(records[i].ShipLocalPosition.x) == MathF.Sign(shipLocalPosition.x) || this._sailType == SailVisual.SailType.LateenSail))
				{
					Vec3 vec = position - records[i].GlobalPosition;
					float lengthSquared = vec.LengthSquared;
					float lengthSquared2 = vec.AsVec2.LengthSquared;
					if (lengthSquared < num3 && lengthSquared < num)
					{
						num2 = i;
						num3 = lengthSquared2;
					}
				}
			}
			return num2;
		}

		// Token: 0x06000C86 RID: 3206 RVA: 0x0005C4DC File Offset: 0x0005A6DC
		private ValueTuple<int, int> FindClosestTwoKnobPoint(Vec3 position, Vec3 shipLocalPosition, List<SailVisual.KnobConnectionPoint> records, bool sideOfYard)
		{
			float num = this._ropeConnectionMaxDistance * this._ropeConnectionMaxDistance;
			ValueTuple<int, int> valueTuple = new ValueTuple<int, int>(-1, -1);
			ValueTuple<float, float> valueTuple2 = new ValueTuple<float, float>(1E+12f, 1E+12f);
			for (int i = 0; i < records.Count; i++)
			{
				if (records[i].RightOfYard == sideOfYard && (MathF.Sign(records[i].ShipLocalPosition.x) == MathF.Sign(shipLocalPosition.x) || this._sailType == SailVisual.SailType.LateenSail))
				{
					Vec3 vec = position - records[i].GlobalPosition;
					float lengthSquared = vec.LengthSquared;
					if (vec.AsVec2.LengthSquared < num)
					{
						if (lengthSquared < valueTuple2.Item1)
						{
							valueTuple2.Item2 = valueTuple2.Item1;
							valueTuple.Item2 = valueTuple.Item1;
							valueTuple2.Item1 = lengthSquared;
							valueTuple.Item1 = i;
						}
						else if (lengthSquared < valueTuple2.Item2)
						{
							valueTuple2.Item2 = lengthSquared;
							valueTuple.Item2 = i;
						}
					}
				}
			}
			return valueTuple;
		}

		// Token: 0x06000C87 RID: 3207 RVA: 0x0005C5F0 File Offset: 0x0005A7F0
		private void CheckFoldAnimationState(float dt)
		{
			if (!this._ongoingAnimationData.FoldIsOngoing && !this._ongoingAnimationData.UnfoldIsOngoing && !this.SailEnabled)
			{
				this.StartFoldAnimation();
			}
			if (this.HasFoldFinished() && !this._ongoingAnimationData.UnfoldIsOngoing && this.SailEnabled)
			{
				this.StartUnfoldAnimation();
			}
			if (!this._ongoingAnimationData.FoldIsOngoing)
			{
				if (this._ongoingAnimationData.UnfoldIsOngoing)
				{
					if (!this.SailEnabled)
					{
						this.CancelAnimation();
						this.TickFoldAnimation(dt);
						return;
					}
					this.TickUnfoldAnimation(dt);
				}
				return;
			}
			if (!this.HasFoldFinished() && this.SailEnabled)
			{
				this.CancelAnimation();
				this.TickUnfoldAnimation(dt);
				return;
			}
			this.TickFoldAnimation(dt);
		}

		// Token: 0x06000C88 RID: 3208 RVA: 0x0005C6A5 File Offset: 0x0005A8A5
		private void DisableMorphAnimation()
		{
			if (this.SailClothComponent != null)
			{
				this.SailClothComponent.DisableMorphAnimation();
			}
			this._lastMorphAnimKeySet = -1f;
		}

		// Token: 0x06000C89 RID: 3209 RVA: 0x0005C6CC File Offset: 0x0005A8CC
		private void SetMorphAnimToCloth(float currentMorphKey)
		{
			if (this._lastMorphAnimKeySet == currentMorphKey)
			{
				return;
			}
			if (this.SailClothComponent != null)
			{
				this.SailClothComponent.SetMorphBuffer(currentMorphKey);
				int num = (int)currentMorphKey;
				int num2 = Math.Min(num + 1, this._ongoingAnimationData.NumberOfMorphKeys - 1);
				float num3 = currentMorphKey - (float)num;
				if (this._sailType == SailVisual.SailType.LateenSail)
				{
					if (this._ongoingAnimationData.CenterVertexPositions != null)
					{
						Vec3 vec = this._ongoingAnimationData.CenterVertexPositions[num];
						Vec3 vec2 = this._ongoingAnimationData.CenterVertexPositions[num2];
						this._ongoingAnimationData.CurrentCenterFreeBonePosition = Vec3.Lerp(vec, vec2, num3);
					}
				}
				else if (this._sailType == SailVisual.SailType.SquareSail)
				{
					if (this._ongoingAnimationData.LeftVertexPositions != null)
					{
						Vec3 vec3 = this._ongoingAnimationData.LeftVertexPositions[num];
						Vec3 vec4 = this._ongoingAnimationData.LeftVertexPositions[num2];
						this._ongoingAnimationData.CurrentLeftFreeBonePosition = Vec3.Lerp(vec3, vec4, num3);
					}
					if (this._ongoingAnimationData.RightVertexPositions != null)
					{
						Vec3 vec5 = this._ongoingAnimationData.RightVertexPositions[num];
						Vec3 vec6 = this._ongoingAnimationData.RightVertexPositions[num2];
						this._ongoingAnimationData.CurrentRightFreeBonePosition = Vec3.Lerp(vec5, vec6, num3);
					}
				}
			}
			this._lastMorphAnimKeySet = currentMorphKey;
		}

		// Token: 0x06000C8A RID: 3210 RVA: 0x0005C818 File Offset: 0x0005AA18
		private void TickLateenSail(float dt)
		{
			if (this._lateenRollDegrees < 1E-06f && this._lateenYardShift < 1E-06f)
			{
				if (this._lateenSailData.RollRotationEntity != null)
				{
					MatrixFrame frame = this._lateenSailData.RollRotationEntity.GetFrame();
					if (Math.Abs(frame.rotation.GetEulerAngles().y * 57.29578f - this._lateenRollDegrees) > 0.001f)
					{
						float num = this._lateenRollDegrees * 0.017453292f;
						frame.rotation = Mat3.Identity;
						Vec3 vec = new Vec3(0f, num, 0f, -1f);
						frame.rotation.ApplyEulerAngles(ref vec);
						this._lateenSailData.RollRotationEntity.SetFrame(ref frame, true);
					}
				}
				if (this._lateenSailData.YardShiftEntity != null)
				{
					MatrixFrame frame2 = this._lateenSailData.YardShiftEntity.GetFrame();
					if (Math.Abs(frame2.origin.x - this._lateenYardShift) > 0.001f)
					{
						frame2.origin.x = this._lateenYardShift;
						this._lateenSailData.YardShiftEntity.SetFrame(ref frame2, true);
					}
				}
				return;
			}
			if (this._lateenSailData.RollRotationEntity != null && this.SailYawRotationEntity != null && this._lateenSailData.YardShiftEntity != null)
			{
				if (this._lateenSailData.RollRotationInProgress)
				{
					this._lateenSailData.RollRotationRealDt = this._lateenSailData.RollRotationRealDt + dt;
					float num2 = this._lateenSailData.RollRotationRealDt * this._lateenRollChangeAnimationStepMultiplier;
					num2 -= (float)((int)num2);
					float num3 = MathF.Lerp(0.35f, 2f, (float)Math.Pow((double)num2, 1.5), 1E-05f);
					num3 = Math.Min(num3, 1f);
					num3 = MathF.Clamp(num3 - 0.2f, 0f, 1f) * 1.6f;
					this._lateenSailData.RollRotationAnimProgress = this._lateenSailData.RollRotationAnimProgress + dt * num3 / this._lateenRollChangeAnimationStepMultiplier;
					float num4 = MathF.Clamp(this._lateenSailData.RollRotationAnimProgress / this._lateenRollChangeAnimationDuration, 0f, 1f);
					float num5 = MathF.Lerp(this._lateenSailData.RollRotationInitial, this._lateenSailData.RollRotationTarget, num4, 1E-05f);
					MatrixFrame frame3 = this._lateenSailData.RollRotationEntity.GetFrame();
					frame3.rotation = Mat3.Identity;
					Vec3 vec = new Vec3(0f, num5, 0f, -1f);
					frame3.rotation.ApplyEulerAngles(ref vec);
					this._lateenSailData.RollRotationEntity.SetFrame(ref frame3, true);
					float num6 = this._lateenRollChangeAnimationDuration - this._lateenRollChangeYardShiftStart;
					float num7 = (float)Math.Pow((double)MathF.Clamp((this._lateenSailData.RollRotationRealDt - num6) / this._lateenRollChangeYardShiftDuration, 0f, 1f), (double)this._lateenRollChangeYardShiftAcceleration);
					float num8 = MathF.Lerp(this._lateenSailData.YardShiftInitial, this._lateenSailData.YardShiftTarget, num7, 1E-05f);
					MatrixFrame frame4 = this._lateenSailData.YardShiftEntity.GetFrame();
					frame4.origin.x = num8;
					this._lateenSailData.YardShiftEntity.SetFrame(ref frame4, true);
					if (this._lateenSailData.RollRotationAnimProgress >= this._lateenRollChangeAnimationDuration && num7 >= 1f)
					{
						this._lateenSailData.RollRotationInProgress = false;
					}
					if (this._lateenSailData.RollAnimationSoundEvent != null)
					{
						if (this._lateenSailData.RollRotationAnimProgress >= this._lateenRollChangeAnimationDuration * 0.9f && num7 >= 0.1f)
						{
							this._lateenSailData.RollAnimationSoundEvent.Stop();
							this._lateenSailData.RollAnimationSoundEvent = null;
						}
						else
						{
							this._lateenSailData.RollAnimationSoundEvent.SetPosition(base.GameEntity.GetGlobalFrame().origin);
						}
					}
					SailVisual.SailFoldProgress ongoingAnimationData = this._ongoingAnimationData;
					if (!this._lateenSailData.RollRotationInProgress)
					{
						this.DisableMorphAnimation();
						return;
					}
				}
				else
				{
					float num9 = this._lateenRollDegrees * 0.017453292f;
					float num10 = 0f;
					MatrixFrame matrixFrame = this.SailYawRotationEntity.GetFrame();
					float num11;
					for (num11 = matrixFrame.rotation.GetEulerAngles().z * 57.29578f; num11 > 180f; num11 -= 180f)
					{
					}
					while (num11 < -180f)
					{
						num11 += 180f;
					}
					float num12 = this._lateenRollChangeDegreeLimit - 90f;
					float num13 = -this._lateenRollChangeDegreeLimit - 90f;
					float num14 = -this._lateenRollChangeDegreeLimit + 90f;
					float num15 = this._lateenRollChangeDegreeLimit + 90f;
					if (num11 < num13 || num11 > num15)
					{
						num10 = -1f;
					}
					else if (num11 > num12 && num11 < num14)
					{
						num10 = 1f;
					}
					matrixFrame = this._lateenSailData.RollRotationEntity.GetFrame();
					float num16 = matrixFrame.rotation.GetEulerAngles().y * 57.29578f;
					float num17 = ((num16 > 0f) ? 1f : (-1f));
					if (num10 != 0f && num17 != num10)
					{
						this._lateenSailData.RollRotationInProgress = true;
						this._lateenSailData.RollRotationInitial = num16 * 0.017453292f;
						this._lateenSailData.RollRotationTarget = num10 * num9;
						this._lateenSailData.YardShiftInitial = this._lateenSailData.YardShiftEntity.GetFrame().origin.x;
						this._lateenSailData.YardShiftTarget = this._lateenYardShift * num10;
						this._lateenSailData.RollRotationAnimProgress = 0f;
						this._lateenSailData.RollRotationRealDt = 0f;
						if (this.SoundsEnabled)
						{
							this._lateenSailData.RollAnimationSoundEvent = SoundEvent.CreateEvent(SailVisual.LateenSailRollSoundEventId, base.GameEntity.Scene);
							this._lateenSailData.RollAnimationSoundEvent.Play();
						}
					}
				}
			}
		}

		// Token: 0x06000C8B RID: 3211 RVA: 0x0005CE08 File Offset: 0x0005B008
		private void SetClothMeshMaxDistance(float value)
		{
			if (this.SailClothComponent != null)
			{
				this.SailClothComponent.SetMaxDistanceMultiplier(value);
			}
		}

		// Token: 0x06000C8C RID: 3212 RVA: 0x0005CE24 File Offset: 0x0005B024
		private void TickFoldAnimation(float dt)
		{
			if (this._ongoingAnimationData.CurrentProgress < this._foldFreeBoneResetDuration || this._ongoingAnimationData.CurrentProgress > this._foldSailDuration + this._foldFreeBoneResetDuration)
			{
				this._ongoingAnimationData.CurrentProgress = this._ongoingAnimationData.CurrentProgress + dt;
			}
			else
			{
				this._ongoingAnimationData.RealProgress = this._ongoingAnimationData.RealProgress + dt;
				if (this._sailType == SailVisual.SailType.LateenSail || !this.FoldAnimationEnabled)
				{
					this._ongoingAnimationData.CurrentProgress = this._ongoingAnimationData.CurrentProgress + dt;
				}
				else
				{
					this._ongoingAnimationData.CurrentProgress = this._ongoingAnimationData.CurrentProgress + dt * this.ComputeSquareSailProgressMultiplier(this._ongoingAnimationData.RealProgress);
				}
			}
			this._ongoingAnimationData.CurrentProgress = Math.Min(this._ongoingAnimationData.CurrentProgress, this._foldSailDuration + this._foldFreeBoneResetDuration + this._foldedSailTransitionDuration);
			if (this._ongoingAnimationData.CurrentProgress >= this._foldFreeBoneResetDuration)
			{
				float num = (this._ongoingAnimationData.CurrentProgress - this._foldFreeBoneResetDuration) / this._foldSailDuration;
				num = MathF.Clamp(num, 0f, 1f);
				float num2 = num * (float)(this._ongoingAnimationData.NumberOfMorphKeys - 1);
				this.SetMorphAnimToCloth(num2);
				float num3 = 0f;
				float num4 = 1f;
				float num5 = 1f - (num - num3) / MathF.Max(num4 - num3, 0.01f);
				num5 = MathF.Clamp(num5, 0f, 1f);
				this.SetClothMeshMaxDistance(num5);
				float num6 = 0f;
				float num7 = 0.75f;
				float num8 = 1f - (num - num6) / MathF.Max(num7 - num6, 0.01f);
				num8 = MathF.Clamp(num8, 0f, 1f);
				this._foldAnimWindReductionFactor = num8;
				if (this._ongoingAnimationData.FoldUnfoldSoundEvent != null && num > 0.875f)
				{
					this._ongoingAnimationData.ShouldStopFoldUnfoldSound = true;
				}
				if (this._ongoingAnimationData.CurrentProgress > this._foldSailDuration + this._foldFreeBoneResetDuration)
				{
					float num9 = (this._ongoingAnimationData.CurrentProgress - (this._foldSailDuration + this._foldFreeBoneResetDuration)) / this._foldedSailTransitionDuration;
					num9 = MathF.Clamp(num9, 0f, 1f);
					if (this._foldedStaticSailEntity != null)
					{
						if (!this._isBurning)
						{
							this._foldedStaticSailEntity.SetVisibilityExcludeParents(true);
						}
						this._foldedStaticSailEntity.SetAlpha(num9 * this._sailEntityAlpha);
						this.SailSkeletonEntity.SetAlpha(num9);
						if (this._foldedStaticSailMesh != null && !this._isBurning)
						{
							this._foldedStaticSailMesh.SetVectorArgument(1f, 0f, 0f, 0f);
						}
						this.SailClothComponent.SetVectorArgument(-1f, 0f, 0f, 0f);
						if (num9 >= 0.99999f)
						{
							this.SailSkeletonEntity.SetVisibilityExcludeParents(false);
						}
					}
					if (this._currentSailLevelUsed != 3 || this._sailType != SailVisual.SailType.SquareSail)
					{
						return;
					}
					if (num9 < 0.99999f)
					{
						using (List<WeakGameEntity>.Enumerator enumerator = this._topLateenSails.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								WeakGameEntity weakGameEntity = enumerator.Current;
								weakGameEntity.SetVisibilityExcludeParents(true);
								weakGameEntity.SetAlpha(1f - num9);
							}
							goto IL_0364;
						}
					}
					foreach (WeakGameEntity weakGameEntity2 in this._topLateenSails)
					{
						weakGameEntity2.SetVisibilityExcludeParents(false);
					}
					IL_0364:
					using (List<WeakGameEntity>.Enumerator enumerator = this._topLateenFoldedSails.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							WeakGameEntity weakGameEntity3 = enumerator.Current;
							weakGameEntity3.SetVisibilityExcludeParents(true);
							weakGameEntity3.SetAlpha(num9);
						}
						return;
					}
				}
				foreach (WeakGameEntity weakGameEntity4 in this._topLateenSails)
				{
					weakGameEntity4.SetVisibilityExcludeParents(true);
					weakGameEntity4.SetAlpha(1f);
				}
				foreach (WeakGameEntity weakGameEntity5 in this._topLateenFoldedSails)
				{
					weakGameEntity5.SetVisibilityExcludeParents(false);
					weakGameEntity5.SetAlpha(0f);
				}
			}
		}

		// Token: 0x06000C8D RID: 3213 RVA: 0x0005D2A4 File Offset: 0x0005B4A4
		private void TickUnfoldAnimation(float dt)
		{
			this._ongoingAnimationData.CurrentProgress = this._ongoingAnimationData.CurrentProgress + dt;
			this._ongoingAnimationData.CurrentProgress = MathF.Min(this._ongoingAnimationData.CurrentProgress, this._unfoldSailDuration + this._foldFreeBoneResetDuration + this._foldedSailTransitionDuration);
			if (this.HasUnfoldFinished())
			{
				this._ongoingAnimationData.CurrentProgress = 0f;
				this._ongoingAnimationData.RealProgress = 0f;
				this._ongoingAnimationData.UnfoldIsOngoing = false;
				this._foldAnimWindReductionFactor = 1f;
				this.DisableMorphAnimation();
				this.SetClothMeshMaxDistance(1f);
				return;
			}
			if (this._ongoingAnimationData.CurrentProgress < this._foldedSailTransitionDuration)
			{
				float num = this._ongoingAnimationData.CurrentProgress / this._foldedSailTransitionDuration;
				num = MathF.Clamp(num, 0f, 1f) * this._sailEntityAlpha;
				this.SailSkeletonEntity.SetVisibilityExcludeParents(true);
				this.SailSkeletonEntity.SetAlpha(num);
				this.SailClothComponent.SetVectorArgument(1f, 0f, 0f, 0f);
				if (this._foldedStaticSailEntity != null)
				{
					this._foldedStaticSailEntity.SetVisibilityExcludeParents(true);
					this._foldedStaticSailEntity.SetAlpha(num);
					if (this._foldedStaticSailMesh != null)
					{
						this._foldedStaticSailMesh.SetVectorArgument(-1f, 0f, 0f, 0f);
					}
				}
				if (this._currentSailLevelUsed != 3 || this._sailType != SailVisual.SailType.SquareSail)
				{
					return;
				}
				if (num < 0.99999f)
				{
					foreach (WeakGameEntity weakGameEntity in this._topLateenSails)
					{
						weakGameEntity.SetVisibilityExcludeParents(true);
						weakGameEntity.SetAlpha(num);
					}
					using (List<WeakGameEntity>.Enumerator enumerator = this._topLateenFoldedSails.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							WeakGameEntity weakGameEntity2 = enumerator.Current;
							weakGameEntity2.SetVisibilityExcludeParents(true);
							weakGameEntity2.SetAlpha(1f - num);
						}
						return;
					}
				}
				foreach (WeakGameEntity weakGameEntity3 in this._topLateenSails)
				{
					weakGameEntity3.SetVisibilityExcludeParents(false);
				}
				using (List<WeakGameEntity>.Enumerator enumerator = this._topLateenFoldedSails.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						WeakGameEntity weakGameEntity4 = enumerator.Current;
						weakGameEntity4.SetVisibilityExcludeParents(true);
						weakGameEntity4.SetAlpha(1f);
					}
					return;
				}
			}
			if (this._foldedStaticSailEntity != null)
			{
				this._foldedStaticSailEntity.SetVisibilityExcludeParents(false);
			}
			foreach (WeakGameEntity weakGameEntity5 in this._topLateenFoldedSails)
			{
				weakGameEntity5.SetVisibilityExcludeParents(false);
				weakGameEntity5.SetAlpha(0f);
			}
			foreach (WeakGameEntity weakGameEntity6 in this._topLateenSails)
			{
				weakGameEntity6.SetVisibilityExcludeParents(true);
				weakGameEntity6.SetAlpha(1f);
			}
			this.SailSkeletonEntity.SetAlpha(this._sailEntityAlpha);
			float num2 = MathF.Clamp((this._ongoingAnimationData.CurrentProgress - this._foldedSailTransitionDuration) / this._unfoldSailDuration, 0f, 1f);
			if (num2 >= 1f)
			{
				if (this._ongoingAnimationData.FoldUnfoldSoundEvent != null)
				{
					this._ongoingAnimationData.ShouldStopFoldUnfoldSound = true;
				}
				this.DisableMorphAnimation();
			}
			else
			{
				float num3 = (1f - num2) * (float)(this._ongoingAnimationData.NumberOfMorphKeys - 1);
				this.SetMorphAnimToCloth(num3);
			}
			float num4 = 0f;
			float num5 = 1f;
			float num6 = 1f - (1f - num2 - num4) / MathF.Max(num5 - num4, 0.01f);
			num6 = MathF.Clamp(num6, 0f, 1f);
			this.SetClothMeshMaxDistance(num6);
			float num7 = 0.25f;
			float num8 = 1f;
			float num9 = 1f - (1f - num2 - num7) / MathF.Max(num8 - num7, 0.01f);
			num9 = MathF.Clamp(num9, 0f, 1f);
			this._foldAnimWindReductionFactor = num9;
		}

		// Token: 0x06000C8E RID: 3214 RVA: 0x0005D740 File Offset: 0x0005B940
		private void InitSailFoldAnimationResources()
		{
			if (this.SailClothComponent != null)
			{
				this._ongoingAnimationData.NumberOfMorphKeys = this.SailClothComponent.GetNumberOfMorphKeys();
				if (this._ongoingAnimationData.NumberOfMorphKeys > 0)
				{
					if (this._sailType == SailVisual.SailType.SquareSail)
					{
						this._ongoingAnimationData.LeftVertexPositions = new Vec3[this._ongoingAnimationData.NumberOfMorphKeys];
						this.SailClothComponent.GetMorphAnimLeftPoints(this._ongoingAnimationData.LeftVertexPositions);
						this._ongoingAnimationData.RightVertexPositions = new Vec3[this._ongoingAnimationData.NumberOfMorphKeys];
						this.SailClothComponent.GetMorphAnimRightPoints(this._ongoingAnimationData.RightVertexPositions);
						return;
					}
					this._ongoingAnimationData.CenterVertexPositions = new Vec3[this._ongoingAnimationData.NumberOfMorphKeys];
					this.SailClothComponent.GetMorphAnimCenterPoints(this._ongoingAnimationData.CenterVertexPositions);
				}
			}
		}

		// Token: 0x06000C8F RID: 3215 RVA: 0x0005D824 File Offset: 0x0005BA24
		private void StartFoldAnimation()
		{
			this._ongoingAnimationData.CurrentProgress = 0f;
			this._ongoingAnimationData.RealProgress = 0f;
			this._ongoingAnimationData.FoldIsOngoing = true;
			this._ongoingAnimationData.UnfoldIsOngoing = false;
			if (this.SoundsEnabled)
			{
				this._ongoingAnimationData.ShouldMakeFoldUnfoldSound = true;
			}
		}

		// Token: 0x06000C90 RID: 3216 RVA: 0x0005D880 File Offset: 0x0005BA80
		private void StartUnfoldAnimation()
		{
			this._ongoingAnimationData.CurrentProgress = 0f;
			this._ongoingAnimationData.RealProgress = 0f;
			this._ongoingAnimationData.FoldIsOngoing = false;
			this._ongoingAnimationData.UnfoldIsOngoing = true;
			if (this.SoundsEnabled)
			{
				this._ongoingAnimationData.ShouldMakeFoldUnfoldSound = true;
			}
		}

		// Token: 0x06000C91 RID: 3217 RVA: 0x0005D8DC File Offset: 0x0005BADC
		private void CancelAnimation()
		{
			if (this._ongoingAnimationData.UnfoldIsOngoing)
			{
				float num;
				if (this._ongoingAnimationData.CurrentProgress < this._foldedSailTransitionDuration)
				{
					num = this._foldSailDuration + this._foldFreeBoneResetDuration + (this._foldedSailTransitionDuration - this._ongoingAnimationData.CurrentProgress);
				}
				else if (this._ongoingAnimationData.CurrentProgress < this._foldedSailTransitionDuration + this._unfoldSailDuration)
				{
					float num2 = (this._ongoingAnimationData.CurrentProgress - this._foldedSailTransitionDuration) / this._unfoldSailDuration;
					num = this._foldFreeBoneResetDuration + this._foldSailDuration * (1f - num2);
				}
				else
				{
					num = (this._unfoldSailDuration + this._foldFreeBoneResetDuration + this._foldedSailTransitionDuration - this._ongoingAnimationData.CurrentProgress) * this._foldFreeBoneResetDuration / this._foldedSailTransitionDuration;
				}
				this.StartFoldAnimation();
				this._ongoingAnimationData.CurrentProgress = num;
				return;
			}
			if (this._ongoingAnimationData.FoldIsOngoing)
			{
				float num3;
				if (this._ongoingAnimationData.CurrentProgress < this._foldFreeBoneResetDuration)
				{
					num3 = this._unfoldSailDuration + this._foldFreeBoneResetDuration + (this._foldedSailTransitionDuration - this._ongoingAnimationData.CurrentProgress);
				}
				else if (this._ongoingAnimationData.CurrentProgress < this._foldSailDuration + this._foldFreeBoneResetDuration)
				{
					float num4 = (this._ongoingAnimationData.CurrentProgress - this._foldFreeBoneResetDuration) / this._foldSailDuration;
					num3 = this._foldedSailTransitionDuration + this._unfoldSailDuration * (1f - num4);
				}
				else
				{
					num3 = (this._foldSailDuration + this._foldFreeBoneResetDuration + this._foldedSailTransitionDuration - this._ongoingAnimationData.CurrentProgress) * this._foldedSailTransitionDuration / this._foldFreeBoneResetDuration;
				}
				this.StartUnfoldAnimation();
				this._ongoingAnimationData.CurrentProgress = num3;
			}
		}

		// Token: 0x06000C92 RID: 3218 RVA: 0x0005DA9C File Offset: 0x0005BC9C
		private bool HasFoldFinished()
		{
			return this._ongoingAnimationData.CurrentProgress >= this._foldSailDuration + this._foldFreeBoneResetDuration + this._foldedSailTransitionDuration;
		}

		// Token: 0x06000C93 RID: 3219 RVA: 0x0005DAC2 File Offset: 0x0005BCC2
		private bool HasUnfoldFinished()
		{
			return this._ongoingAnimationData.CurrentProgress >= this._unfoldSailDuration + this._foldFreeBoneResetDuration + this._foldedSailTransitionDuration;
		}

		// Token: 0x06000C94 RID: 3220 RVA: 0x0005DAE8 File Offset: 0x0005BCE8
		private void UpdateTotalFoldDuration()
		{
			this._totalFoldDuration = this._foldFreeBoneResetDuration + this._foldedSailTransitionDuration;
			if (this._sailType == SailVisual.SailType.LateenSail)
			{
				this._totalFoldDuration += this._foldSailDuration;
				return;
			}
			float num = this.EstimateSquareSailFoldAnimationDuration();
			this._totalFoldDuration += num;
		}

		// Token: 0x06000C95 RID: 3221 RVA: 0x0005DB3A File Offset: 0x0005BD3A
		private void UpdateTotalUnfoldDuration()
		{
			this._totalUnfoldDuration = this._unfoldSailDuration + this._foldFreeBoneResetDuration + this._foldedSailTransitionDuration;
		}

		// Token: 0x06000C96 RID: 3222 RVA: 0x0005DB58 File Offset: 0x0005BD58
		private void HandleLOD()
		{
			Vec3 lastFinalRenderCameraPositionOfScene = base.GameEntity.GetLastFinalRenderCameraPositionOfScene();
			Vec3 origin = base.GameEntity.GetGlobalFrame().origin;
			float num = lastFinalRenderCameraPositionOfScene.DistanceSquared(origin);
			this._ropesAreInvisibleThisFrame = num > 22500f;
			bool flag = num > 2025f;
			if (this._ropesWereInvisibleLastFrame != this._ropesAreInvisibleThisFrame || this._lodCheckFirstFrame)
			{
				foreach (SailVisual.FreeBoneRecord freeBoneRecord in this._freeBones)
				{
					if (freeBoneRecord.FoldSailPulley.Entity != null)
					{
						freeBoneRecord.FoldSailPulley.Entity.SetVisibilityExcludeParents(!this._ropesAreInvisibleThisFrame);
					}
					if (freeBoneRecord.RotatorPulleys != null)
					{
						foreach (SailVisual.PulleyDataCache pulleyDataCache in freeBoneRecord.RotatorPulleys)
						{
							pulleyDataCache.Entity.SetVisibilityExcludeParents(!this._ropesAreInvisibleThisFrame);
						}
					}
					if (freeBoneRecord.StabilityPulleys != null)
					{
						foreach (SailVisual.PulleyDataCache pulleyDataCache2 in freeBoneRecord.StabilityPulleys)
						{
							pulleyDataCache2.Entity.SetVisibilityExcludeParents(!this._ropesAreInvisibleThisFrame);
						}
					}
					if (freeBoneRecord.StabilityRopes != null)
					{
						foreach (SailVisual.SimpleRopeRecord simpleRopeRecord in freeBoneRecord.StabilityRopes)
						{
							simpleRopeRecord.ParentEntity.SetVisibilityExcludeParents(!this._ropesAreInvisibleThisFrame);
						}
					}
				}
				foreach (SailVisual.SimpleRopeRecord simpleRopeRecord2 in this._simpleRopes)
				{
					if (!simpleRopeRecord2.IsBigRope)
					{
						simpleRopeRecord2.RopeEntity.SetVisibilityExcludeParents(!this._ropesAreInvisibleThisFrame);
					}
				}
				foreach (SailVisual.SimpleRopeRecord simpleRopeRecord3 in this._mastRopes)
				{
					if (!simpleRopeRecord3.IsBigRope)
					{
						simpleRopeRecord3.RopeEntity.SetVisibilityExcludeParents(!this._ropesAreInvisibleThisFrame);
					}
				}
			}
			if (this._ropesWereLinearLastFrame != flag || this._lodCheckFirstFrame)
			{
				foreach (SailVisual.FreeBoneRecord freeBoneRecord2 in this._freeBones)
				{
					if (freeBoneRecord2.FoldSailPulley.Entity != null)
					{
						freeBoneRecord2.FoldSailPulley.PulleySystem.SetLinearMode(flag);
					}
					if (freeBoneRecord2.RotatorPulleys != null)
					{
						foreach (SailVisual.PulleyDataCache pulleyDataCache3 in freeBoneRecord2.RotatorPulleys)
						{
							pulleyDataCache3.PulleySystem.SetLinearMode(flag);
						}
					}
					if (freeBoneRecord2.StabilityPulleys != null)
					{
						foreach (SailVisual.PulleyDataCache pulleyDataCache4 in freeBoneRecord2.StabilityPulleys)
						{
							pulleyDataCache4.PulleySystem.SetLinearMode(flag);
						}
					}
					if (freeBoneRecord2.StabilityRopes != null)
					{
						foreach (SailVisual.SimpleRopeRecord simpleRopeRecord4 in freeBoneRecord2.StabilityRopes)
						{
							simpleRopeRecord4.RopeSegment.SetLinearMode(flag);
						}
					}
				}
				foreach (SailVisual.SimpleRopeRecord simpleRopeRecord5 in this._simpleRopes)
				{
					simpleRopeRecord5.RopeSegment.SetLinearMode(flag);
				}
				foreach (SailVisual.SimpleRopeRecord simpleRopeRecord6 in this._mastRopes)
				{
					simpleRopeRecord6.RopeSegment.SetLinearMode(flag);
				}
			}
			this._ropesWereInvisibleLastFrame = this._ropesAreInvisibleThisFrame;
			this._ropesWereLinearLastFrame = flag;
			this._lodCheckFirstFrame = false;
		}

		// Token: 0x06000C97 RID: 3223 RVA: 0x0005E09C File Offset: 0x0005C29C
		private float ComputeSquareSailProgressMultiplier(float progress)
		{
			float num = progress * this._foldSailStepMultiplier;
			num -= (float)((int)num);
			return MathF.Clamp(Math.Min(MathF.Lerp(0f, 1f, num, 1E-05f), 1f) - 0.2f, 0f, 1f) * 1.6f / this._foldSailStepMultiplier;
		}

		// Token: 0x06000C98 RID: 3224 RVA: 0x0005E0FC File Offset: 0x0005C2FC
		public float EstimateSquareSailFoldAnimationDuration()
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0.01f;
			while (num < this._foldSailDuration)
			{
				num += num3 * this.ComputeSquareSailProgressMultiplier(num2);
				num2 += num3;
				if (num2 > this._foldSailDuration * 10f)
				{
					break;
				}
			}
			return num2;
		}

		// Token: 0x06000C99 RID: 3225 RVA: 0x0005E148 File Offset: 0x0005C348
		private SailVisual.SimpleRopeRecord FillSimpleRopeRecord(WeakGameEntity parentEntity)
		{
			SailVisual.SimpleRopeRecord simpleRopeRecord = new SailVisual.SimpleRopeRecord
			{
				StartPointAttachedToYard = false,
				EndPointAttachedToYard = false,
				ParentEntity = GameEntity.CreateFromWeakEntity(parentEntity),
				RopeSegment = null,
				IsBigRope = parentEntity.HasTag("big_rope"),
				RopeEntity = GameEntity.CreateFromWeakEntity(parentEntity.GetFirstChildEntityWithTagRecursive("simple_rope_start"))
			};
			if (simpleRopeRecord.RopeEntity != null)
			{
				simpleRopeRecord.StartPointAttachedToYard = simpleRopeRecord.RopeEntity.HasTag("attached_to_yard");
				simpleRopeRecord.RopeSegment = simpleRopeRecord.RopeEntity.GetFirstScriptOfType<RopeSegment>();
			}
			simpleRopeRecord.TargetEntity = GameEntity.CreateFromWeakEntity(parentEntity.GetFirstChildEntityWithTagRecursive("simple_rope_end"));
			if (simpleRopeRecord.TargetEntity != null)
			{
				simpleRopeRecord.EndPointAttachedToYard = simpleRopeRecord.TargetEntity.HasTag("attached_to_yard");
			}
			if (simpleRopeRecord.RopeSegment != null)
			{
				simpleRopeRecord.RopeSegment.SetUseDistanceAsRopeLength();
			}
			return simpleRopeRecord;
		}

		// Token: 0x06000C9A RID: 3226 RVA: 0x0005E238 File Offset: 0x0005C438
		private void PlaceClothFragmentsRandomly(int seed)
		{
			MBFastRandom mbfastRandom = new MBFastRandom();
			mbfastRandom.SetSeed((uint)seed, 0U);
			List<RopeSegment> list = new List<RopeSegment>();
			float num = 0.04f;
			foreach (SailVisual.FreeBoneRecord freeBoneRecord in this._freeBones)
			{
				if (freeBoneRecord.FoldSailPulley.PulleySystem != null)
				{
					freeBoneRecord.FoldSailPulley.PulleySystem.GetAllRopeSegments(ref list, num);
				}
				if (freeBoneRecord.RotatorPulleys != null)
				{
					foreach (SailVisual.PulleyDataCache pulleyDataCache in freeBoneRecord.RotatorPulleys)
					{
						pulleyDataCache.PulleySystem.GetAllRopeSegments(ref list, num);
					}
				}
				if (freeBoneRecord.StabilityPulleys != null)
				{
					foreach (SailVisual.PulleyDataCache pulleyDataCache2 in freeBoneRecord.StabilityPulleys)
					{
						pulleyDataCache2.PulleySystem.GetAllRopeSegments(ref list, num);
					}
				}
				if (freeBoneRecord.StabilityRopes != null)
				{
					foreach (SailVisual.SimpleRopeRecord simpleRopeRecord in freeBoneRecord.StabilityRopes)
					{
						if (simpleRopeRecord.RopeSegment.RopeMesh != null && simpleRopeRecord.RopeSegment.RopeMesh.GetVectorArgument().w < num)
						{
							list.Add(simpleRopeRecord.RopeSegment);
						}
					}
				}
			}
			foreach (SailVisual.SimpleRopeRecord simpleRopeRecord2 in this._simpleRopes)
			{
				if (simpleRopeRecord2.RopeSegment.RopeMesh != null && simpleRopeRecord2.RopeSegment.RopeMesh.GetVectorArgument().w < num)
				{
					list.Add(simpleRopeRecord2.RopeSegment);
				}
			}
			for (int i = MathF.Min(6, list.Count); i > 0; i--)
			{
				int num2 = mbfastRandom.Next(0, list.Count);
				RopeSegment ropeSegment = list[num2];
				int num3 = 2 + (int)(mbfastRandom.NextFloat() * 1.5f);
				for (int j = 0; j < num3; j++)
				{
					string text = SailVisual.ClothFragmentPrefabs[mbfastRandom.Next(0, SailVisual.ClothFragmentPrefabs.Count<string>() - 1)];
					GameEntity gameEntity = GameEntity.Instantiate(base.GameEntity.Scene, text, true, true, "");
					ropeSegment.GameEntity.AddChild(gameEntity.WeakEntity, false);
					gameEntity.EntityFlags |= 131072;
					float num4 = 1f + mbfastRandom.NextFloat() * 1f;
					MatrixFrame identity = MatrixFrame.Identity;
					identity.rotation.ApplyScaleLocal(num4);
					gameEntity.SetLocalFrame(ref identity, false);
					RopeSegmentCosmetics firstScriptOfType = gameEntity.GetFirstScriptOfType<RopeSegmentCosmetics>();
					if (firstScriptOfType != null)
					{
						firstScriptOfType.RopeLocalPosition = mbfastRandom.NextFloat();
					}
				}
				list[num2] = list[list.Count - 1];
				list.RemoveAt(list.Count - 1);
			}
		}

		// Token: 0x06000C9B RID: 3227 RVA: 0x0005E5D4 File Offset: 0x0005C7D4
		private void FetchEntities()
		{
			bool flag = this._yardEntity == null;
			bool flag2 = base.GameEntity.Scene.IsEditorScene();
			this._freeBones.Clear();
			this._simpleRopes.Clear();
			this._mastRopes.Clear();
			this._knobConnectionPoints.Clear();
			this.SailSkeletonEntity = null;
			this.SailClothComponent = null;
			this._sailSkeleton = null;
			this._yardEntity = null;
			this._foldedStaticSailEntity = null;
			this._foldedStaticSailMesh = null;
			this._burningSailEntity = null;
			this._burningSailMesh = null;
			this._topLateenSails.Clear();
			this._topLateenFoldedSails.Clear();
			this._ballistaVisibilityRopes.Clear();
			this.SailYawRotationEntity = GameEntity.CreateFromWeakEntity(base.GameEntity.GetFirstChildEntityWithTag("yaw_rotation_entity"));
			if (this.SailYawRotationEntity != null)
			{
				WeakGameEntity weakGameEntity = this.SailYawRotationEntity.WeakEntity;
				if (this._sailType == SailVisual.SailType.LateenSail)
				{
					this._lateenSailData.RollRotationEntity = this.SailYawRotationEntity.GetFirstChildEntityWithTagRecursive("roll_rotation_entity");
					if (this._lateenSailData.RollRotationEntity != null)
					{
						this._lateenSailData.YardShiftEntity = this._lateenSailData.RollRotationEntity.GetFirstChildEntityWithTag("yard_shift");
						if (this._lateenSailData.YardShiftEntity != null)
						{
							weakGameEntity = this._lateenSailData.YardShiftEntity.WeakEntity;
						}
					}
				}
				WeakGameEntity firstChildEntityWithTagRecursive = base.GameEntity.GetFirstChildEntityWithTagRecursive("mast_entity");
				this._mastEntity = GameEntity.CreateFromWeakEntity(firstChildEntityWithTagRecursive);
				if (firstChildEntityWithTagRecursive.IsValid && firstChildEntityWithTagRecursive.Parent.IsValid)
				{
					foreach (WeakGameEntity weakGameEntity2 in firstChildEntityWithTagRecursive.Parent.GetChildren())
					{
						if (weakGameEntity2.HasTag("simple_rope"))
						{
							SailVisual.SimpleRopeRecord simpleRopeRecord = this.FillSimpleRopeRecord(weakGameEntity2);
							this._mastRopes.Add(simpleRopeRecord);
						}
					}
				}
				this.SailSkeletonEntity = GameEntity.CreateFromWeakEntity(weakGameEntity.GetFirstChildEntityWithTag("sail_mesh_entity"));
				if (this.SailSkeletonEntity != null)
				{
					this._sailSkeleton = this.SailSkeletonEntity.Skeleton;
					if (this._sailSkeleton != null)
					{
						this.SailClothComponent = this._sailSkeleton.GetComponentAtIndex(3, 0) as ClothSimulatorComponent;
						this._sailSkeleton.EnableScriptDrivenPostIntegrateCallback();
						if (this._sailSkeleton != null)
						{
							base.GameEntity.Scene.AddAlwaysRenderedSkeleton(this._sailSkeleton);
						}
					}
				}
				this._burningSailEntity = GameEntity.CreateFromWeakEntity(weakGameEntity.GetFirstChildEntityWithTag("sail_mesh_free_entity"));
				if (this._burningSailEntity != null && this._burningSailEntity.Skeleton != null)
				{
					ClothSimulatorComponent clothSimulatorComponent = this._burningSailEntity.Skeleton.GetComponentAtIndex(3, 0) as ClothSimulatorComponent;
					if (clothSimulatorComponent != null)
					{
						this._burningSailMesh = clothSimulatorComponent.GetFirstMetaMesh().GetMeshAtIndex(0);
					}
				}
				this.SailTopBannerEntity = GameEntity.CreateFromWeakEntity(base.GameEntity.GetFirstChildEntityWithTagRecursive("bd_banner_b"));
				if (this.SailTopBannerEntity != null)
				{
					this.SailTopBannerClothComponent = this.SailTopBannerEntity.GetClothSimulator(0);
					this.SailTopBannerEntity.SetDoNotCheckVisibility(true);
				}
				this._yardEntity = GameEntity.CreateFromWeakEntity(weakGameEntity.GetFirstChildEntityWithTag("sail_yard"));
				this._foldedStaticSailEntity = GameEntity.CreateFromWeakEntity(weakGameEntity.GetFirstChildEntityWithTag("folded_static_entity"));
				weakGameEntity.GetChildrenWithTagRecursive(this._topLateenSails, "lvl3_lateens_entity");
				weakGameEntity.GetChildrenWithTagRecursive(this._topLateenFoldedSails, "lvl3_lateens_folded");
				if (this._foldedStaticSailEntity != null)
				{
					this._foldedStaticSailMesh = this._foldedStaticSailEntity.GetFirstMesh();
				}
				if (this._yardEntity != null)
				{
					WeakGameEntity firstChildEntityWithTagRecursive2 = this._yardEntity.WeakEntity.GetFirstChildEntityWithTagRecursive("yard_mesh");
					this._yardMesh = ((firstChildEntityWithTagRecursive2 != null) ? firstChildEntityWithTagRecursive2.GetFirstMesh() : null);
				}
				if (flag && this._yardEntity != null)
				{
					this.UpdatePreviousYardFrame();
					MatrixFrame globalFrame = this._yardEntity.GetGlobalFrame();
					this._previousSailYardFrame = base.GameEntity.GetGlobalFrame().TransformToLocalNonOrthogonal(ref globalFrame);
				}
			}
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			if (this.SailSkeletonEntity != null)
			{
				Skeleton skeleton = this.SailSkeletonEntity.Skeleton;
				foreach (GameEntity gameEntity in this.SailSkeletonEntity.GetChildren())
				{
					if (gameEntity.HasTag("free_bone") && !dictionary.ContainsKey(gameEntity.Name))
					{
						SailVisual.FreeBoneRecord freeBoneRecord = new SailVisual.FreeBoneRecord();
						freeBoneRecord.InitialLocalFrame = gameEntity.GetFrame();
						freeBoneRecord.CurrentLocalFrame = freeBoneRecord.InitialLocalFrame;
						freeBoneRecord.BoneIndex = -1;
						freeBoneRecord.ConnectionType = SailVisual.FreeBoneConnectionType.All;
						freeBoneRecord.Entity = gameEntity;
						freeBoneRecord.FoldSailPulley.Entity = null;
						freeBoneRecord.FoldSailPulley.PulleySystem = null;
						if (gameEntity.HasTag("closest_pulley"))
						{
							freeBoneRecord.ConnectionType = SailVisual.FreeBoneConnectionType.Closest;
						}
						else if (gameEntity.HasTag("closest_two_pulleys"))
						{
							freeBoneRecord.ConnectionType = SailVisual.FreeBoneConnectionType.ClosestTwo;
						}
						if (this._sailType == SailVisual.SailType.SquareSail)
						{
							if (gameEntity.Name.Contains("_l"))
							{
								freeBoneRecord.BoneType = SailVisual.FreeBoneType.Left;
							}
							else if (gameEntity.Name.Contains("_r"))
							{
								freeBoneRecord.BoneType = SailVisual.FreeBoneType.Right;
							}
						}
						else if (this._sailType == SailVisual.SailType.LateenSail)
						{
							if (gameEntity.Name.Contains("_l"))
							{
								freeBoneRecord.BoneType = SailVisual.FreeBoneType.Left;
							}
							else if (gameEntity.Name.Contains("_r"))
							{
								freeBoneRecord.BoneType = SailVisual.FreeBoneType.Right;
							}
							else if (gameEntity.Name.Contains("_c"))
							{
								freeBoneRecord.BoneType = SailVisual.FreeBoneType.Center;
							}
						}
						if (skeleton != null)
						{
							string name = gameEntity.Name;
							for (int i = 0; i < (int)skeleton.GetBoneCount(); i++)
							{
								if (skeleton.GetBoneName((sbyte)i) == name)
								{
									freeBoneRecord.InitialLocalFrame = skeleton.GetBoneEntitialRestFrame((sbyte)i, false);
									freeBoneRecord.BoneIndex = (sbyte)i;
									break;
								}
							}
						}
						dictionary.Add(gameEntity.Name, this._freeBones.Count);
						this._freeBones.Add(freeBoneRecord);
					}
				}
			}
			WeakGameEntity firstChildEntityWithTag = base.GameEntity.GetFirstChildEntityWithTag("pulley_systems_parent");
			if (firstChildEntityWithTag != null)
			{
				firstChildEntityWithTag.SetDoNotCheckVisibility(true);
				WeakGameEntity firstChildEntityWithTag2 = firstChildEntityWithTag.GetFirstChildEntityWithTag("sail_fold_pulleys");
				if (firstChildEntityWithTag2 != null)
				{
					firstChildEntityWithTag2.SetDoNotCheckVisibility(true);
					foreach (WeakGameEntity weakGameEntity3 in firstChildEntityWithTag2.GetChildren())
					{
						foreach (string text in weakGameEntity3.Tags)
						{
							if (text != "fold_pulley_system")
							{
								int num = -1;
								if (dictionary.TryGetValue(text, out num))
								{
									this._freeBones[num].FoldSailPulley.Entity = GameEntity.CreateFromWeakEntity(weakGameEntity3);
									this._freeBones[num].FoldSailPulley.PulleySystem = weakGameEntity3.GetFirstScriptOfType<PulleySystem>();
									break;
								}
							}
						}
					}
				}
				WeakGameEntity firstChildEntityWithTag3 = firstChildEntityWithTag.GetFirstChildEntityWithTag("sail_rotate_pulleys");
				if (firstChildEntityWithTag3 != null)
				{
					firstChildEntityWithTag3.SetDoNotCheckVisibility(true);
					foreach (WeakGameEntity weakGameEntity4 in firstChildEntityWithTag3.GetChildren())
					{
						foreach (string text2 in weakGameEntity4.Tags)
						{
							if (text2 != "pulley_system")
							{
								int num2 = -1;
								if (dictionary.TryGetValue(text2, out num2))
								{
									SailVisual.PulleyDataCache pulleyDataCache = default(SailVisual.PulleyDataCache);
									pulleyDataCache.Entity = GameEntity.CreateFromWeakEntity(weakGameEntity4);
									pulleyDataCache.PulleySystem = weakGameEntity4.GetFirstScriptOfType<PulleySystem>();
									if (this._freeBones[num2].RotatorPulleys == null)
									{
										this._freeBones[num2].RotatorPulleys = new List<SailVisual.PulleyDataCache>();
									}
									this._freeBones[num2].RotatorPulleys.Add(pulleyDataCache);
									break;
								}
							}
						}
					}
				}
				WeakGameEntity firstChildEntityWithTag4 = firstChildEntityWithTag.GetFirstChildEntityWithTag("stability_ropes_parent");
				if (firstChildEntityWithTag4 != null)
				{
					firstChildEntityWithTag4.SetDoNotCheckVisibility(true);
					foreach (WeakGameEntity weakGameEntity5 in firstChildEntityWithTag4.GetChildren())
					{
						bool flag3 = weakGameEntity5.HasTag("simple_rope");
						string text3 = (flag3 ? "simple_rope" : "pulley_system");
						foreach (string text4 in weakGameEntity5.Tags)
						{
							if (text4 != text3)
							{
								int num3 = -1;
								if (dictionary.TryGetValue(text4, out num3))
								{
									if (flag3)
									{
										SailVisual.SimpleRopeRecord simpleRopeRecord2 = this.FillSimpleRopeRecord(weakGameEntity5);
										if (simpleRopeRecord2.RopeSegment != null)
										{
											simpleRopeRecord2.RopeSegment.SetAsDynamic();
										}
										if (this._freeBones[num3].StabilityRopes == null)
										{
											this._freeBones[num3].StabilityRopes = new List<SailVisual.SimpleRopeRecord>();
										}
										this._freeBones[num3].StabilityRopes.Add(simpleRopeRecord2);
										break;
									}
									SailVisual.PulleyDataCache pulleyDataCache2 = default(SailVisual.PulleyDataCache);
									pulleyDataCache2.Entity = GameEntity.CreateFromWeakEntity(weakGameEntity5);
									pulleyDataCache2.PulleySystem = weakGameEntity5.GetFirstScriptOfType<PulleySystem>();
									if (this._freeBones[num3].StabilityPulleys == null)
									{
										this._freeBones[num3].StabilityPulleys = new List<SailVisual.PulleyDataCache>();
									}
									this._freeBones[num3].StabilityPulleys.Add(pulleyDataCache2);
									break;
								}
							}
						}
					}
				}
				WeakGameEntity firstChildEntityWithTag5 = firstChildEntityWithTag.GetFirstChildEntityWithTag("static_ropes_parent");
				if (firstChildEntityWithTag5 != null)
				{
					firstChildEntityWithTag5.SetDoNotCheckVisibility(true);
					foreach (WeakGameEntity weakGameEntity6 in firstChildEntityWithTag5.GetChildren())
					{
						if (weakGameEntity6.HasTag("simple_rope"))
						{
							SailVisual.SimpleRopeRecord simpleRopeRecord3 = this.FillSimpleRopeRecord(weakGameEntity6);
							if (simpleRopeRecord3.RopeSegment != null)
							{
								simpleRopeRecord3.RopeSegment.SetUseDistanceAsRopeLength();
							}
							this._simpleRopes.Add(simpleRopeRecord3);
						}
						if (weakGameEntity6.HasTag("ballista_visibility"))
						{
							this._ballistaVisibilityRopes.Add(weakGameEntity6);
						}
					}
				}
			}
			WeakGameEntity root = base.GameEntity.Root;
			this._knobParent = GameEntity.CreateFromWeakEntity(root.GetFirstChildEntityWithTagRecursive("knob_points_parent"));
			if (this._knobParent != null)
			{
				MatrixFrame globalFrame2 = root.GetGlobalFrame();
				List<WeakGameEntity> list = new List<WeakGameEntity>();
				this._knobParent.WeakEntity.GetChildrenWithTagRecursive(list, "knot_point");
				foreach (WeakGameEntity weakGameEntity7 in list)
				{
					SailVisual.KnobConnectionPoint knobConnectionPoint = default(SailVisual.KnobConnectionPoint);
					knobConnectionPoint.GlobalPosition = weakGameEntity7.GetGlobalFrame().origin;
					knobConnectionPoint.ShipLocalPosition = globalFrame2.TransformToLocalNonOrthogonal(ref knobConnectionPoint.GlobalPosition);
					knobConnectionPoint.IsFixed = weakGameEntity7.HasTag("dynamic_knob");
					this._knobConnectionPoints.Add(knobConnectionPoint);
					if (!flag2)
					{
						weakGameEntity7.Remove(79);
					}
				}
			}
			WeakGameEntity firstChildEntityWithTagRecursive3 = base.GameEntity.GetFirstChildEntityWithTagRecursive("flag_capture_rope");
			if (firstChildEntityWithTagRecursive3 != null)
			{
				this._topFlagRope = this.FillSimpleRopeRecord(firstChildEntityWithTagRecursive3);
			}
			base.GameEntity.SetHasCustomBoundingBoxValidationSystem(true);
			base.GameEntity.SetBoundingboxDirty();
		}

		// Token: 0x06000C9C RID: 3228 RVA: 0x0005F270 File Offset: 0x0005D470
		private void InitLateenSailData()
		{
			if (this.SailYawRotationEntity == null || this._lateenSailData.RollRotationEntity == null || this._lateenSailData.YardShiftEntity == null)
			{
				return;
			}
			float z = this.SailYawRotationEntity.GetFrame().rotation.GetEulerAngles().z;
			if (z > this._lateenRollChangeDegreeLimit)
			{
				float num = ((z > 0.01f) ? 1f : (-1f));
				float num2 = this._lateenRollDegrees * 0.017453292f;
				float num3 = num * num2;
				MatrixFrame frame = this._lateenSailData.RollRotationEntity.GetFrame();
				frame.rotation = Mat3.Identity;
				Vec3 vec = new Vec3(0f, num3, 0f, -1f);
				frame.rotation.ApplyEulerAngles(ref vec);
				this._lateenSailData.RollRotationEntity.SetFrame(ref frame, true);
				MatrixFrame frame2 = this._lateenSailData.YardShiftEntity.GetFrame();
				frame2.origin.x = num * this._lateenYardShift;
				this._lateenSailData.YardShiftEntity.SetFrame(ref frame2, true);
			}
		}

		// Token: 0x06000C9D RID: 3229 RVA: 0x0005F393 File Offset: 0x0005D593
		private void UpdatePreviousYardFrame()
		{
			this._previousYawEntityFrame = this.SailYawRotationEntity.GetLocalFrame();
		}

		// Token: 0x06000C9E RID: 3230 RVA: 0x0005F3A8 File Offset: 0x0005D5A8
		private void TickFire(float dt)
		{
			this._burningRecord.FireDt = this._burningRecord.FireDt + dt;
			float num = this._burningRecord.FireDt / this._burningAnimationDuration;
			bool flag = true;
			foreach (BurningSystem burningSystem in this._burningRecord.SailFires)
			{
				burningSystem.Tick(dt);
			}
			if (this.SailClothComponent != null)
			{
				float num2 = 0.99f + MathF.Min(num, 1f) * 0.55f;
				if (this.SailEnabled)
				{
					if (this._burningSailMesh != null)
					{
						this._burningSailMesh.SetVectorArgument(num2, 0f, 0f, 0f);
					}
				}
				else
				{
					this._foldedStaticSailMesh.SetVectorArgument(num2, 0f, 0f, 0f);
				}
				if (num2 < 1.52f)
				{
					flag = false;
				}
			}
			if (this._topLateenFireMaterial != null && this._currentSailLevelUsed == 3 && this._sailType == SailVisual.SailType.SquareSail)
			{
				float num3 = 0.99f + num * 1.01f;
				foreach (WeakGameEntity weakGameEntity in this._topLateenSails)
				{
					foreach (Mesh mesh in weakGameEntity.GetAllMeshesWithTag("faction_color"))
					{
						mesh.SetVectorArgument(num3, 0f, 0f, 0f);
					}
				}
				foreach (WeakGameEntity weakGameEntity2 in this._topLateenFoldedSails)
				{
					weakGameEntity2.SetDoNotCheckVisibility(true);
					foreach (Mesh mesh2 in weakGameEntity2.GetAllMeshesWithTag("faction_color"))
					{
						mesh2.SetVectorArgument(num3, 0f, 0f, 0f);
					}
				}
			}
			foreach (BurningSystem burningSystem2 in this._burningRecord.SailFires)
			{
				MBReadOnlyList<BurningNode> burningNodes = burningSystem2.BurningNodes;
				int num4 = (int)((num - 0.2f) * (float)burningNodes.Count);
				int num5 = 0;
				while (num5 < num4 && num5 < burningNodes.Count)
				{
					burningNodes[num5].CurrentFireProgress = 0f;
					num5++;
				}
			}
			if (this._burningRecord.MastFire != null)
			{
				this._burningRecord.MastFire.Tick(dt);
				float flameProgress = this._burningRecord.MastFire.GetFlameProgress();
				Color color = Color.Lerp(this._burningRecord.InitialYardMastColor, this._burningRecord.InitialYardMastColor * 0.75f, flameProgress);
				color.Alpha = 1f;
				this._mastEntity.SetFactorColor(color.ToUnsignedInteger());
				float num6 = this._burningAnimationDuration * 9f;
				float num7 = this._burningRecord.FireDt / num6;
				float num8 = MathF.Clamp(1f - (num7 - 0.75f) * 4f, 0f, 1f);
				this._burningRecord.MastFire.SetExternalFlameMultiplier(num8);
				if (num8 > 0f)
				{
					this._burningRecord.MastFire.CheckWater();
					flag = false;
				}
			}
			if (this._burningRecord.FireDt > this._burningAnimationDuration * 0.5f)
			{
				if (this._burningRecord.YardFireStartDt == 0f)
				{
					this._burningRecord.YardFireStartDt = this._burningRecord.FireDt;
				}
				float num9 = 0f;
				if (this._burningRecord.YardRightFire != null)
				{
					this._burningRecord.YardRightFire.Tick(dt);
					num9 = Math.Max(num9, this._burningRecord.YardRightFire.GetFlameProgress());
					float num10 = this._burningAnimationDuration * 9f;
					float num11 = (this._burningRecord.FireDt - this._burningRecord.YardFireStartDt) / num10;
					float num12 = MathF.Clamp(1f - (num11 - 0.75f) * 4f, 0f, 1f);
					this._burningRecord.YardRightFire.SetExternalFlameMultiplier(num12);
					if (num12 > 0f)
					{
						this._burningRecord.YardRightFire.CheckWater();
						flag = false;
					}
				}
				if (this._burningRecord.YardLeftFire != null)
				{
					this._burningRecord.YardLeftFire.Tick(dt);
					num9 = Math.Max(num9, this._burningRecord.YardLeftFire.GetFlameProgress());
					float num13 = this._burningAnimationDuration * 9f;
					float num14 = (this._burningRecord.FireDt - this._burningRecord.YardFireStartDt) / num13;
					float num15 = MathF.Clamp(1f - (num14 - 0.75f) * 4f, 0f, 1f);
					this._burningRecord.YardLeftFire.SetExternalFlameMultiplier(num15);
					if (num15 > 0f)
					{
						this._burningRecord.YardLeftFire.CheckWater();
						flag = false;
					}
				}
				foreach (BurningSystem burningSystem3 in this._burningRecord.StaticRopeFires)
				{
					burningSystem3.Tick(dt);
					if (burningSystem3.BurnedRope != null)
					{
						float num16 = (this._burningRecord.FireDt - this._burningRecord.YardFireStartDt) / burningSystem3.GetBurningAnimationDuration();
						float num17 = 1f - (num16 - 0.4f);
						burningSystem3.BurnedRope.SetAlpha(MathF.Max(num17, 0.01f));
						burningSystem3.SetExternalFlameMultiplier(num17);
						if (num17 > 0f)
						{
							flag = false;
							burningSystem3.CheckWater();
						}
					}
				}
				if (this._yardMesh != null)
				{
					Color color2 = Color.Lerp(this._burningRecord.InitialYardMastColor, this._burningRecord.InitialYardMastColor * 0.75f, num9);
					color2.Alpha = 1f;
					this._yardMesh.Color = color2.ToUnsignedInteger();
				}
			}
			if (this._burningRecord.FireDt > this._burningAnimationDuration * 0.25f)
			{
				foreach (BurningSystem burningSystem4 in this._burningRecord.RotatorFires)
				{
					burningSystem4.Tick(dt);
					if (burningSystem4.BurnedPulley != null)
					{
						float num18 = this._burningRecord.FireDt / burningSystem4.GetBurningAnimationDuration();
						float num19 = 1f - (num18 - 0.4f);
						burningSystem4.BurnedPulley.SetAlpha(MathF.Max(num19, 0.01f));
						burningSystem4.SetExternalFlameMultiplier(num19);
						if (num19 > 0f)
						{
							burningSystem4.CheckWater();
							flag = false;
						}
					}
				}
			}
			foreach (BurningSystem burningSystem5 in this._burningRecord.FoldFires)
			{
				burningSystem5.Tick(dt);
				if (burningSystem5.BurnedPulley != null)
				{
					float num20 = this._burningRecord.FireDt / burningSystem5.GetBurningAnimationDuration();
					float num21 = 1f - (num20 - 0.4f);
					burningSystem5.BurnedPulley.SetAlpha(MathF.Max(num21, 0.01f));
					burningSystem5.SetExternalFlameMultiplier(num21);
					if (num21 > 0f)
					{
						burningSystem5.CheckWater();
						flag = false;
					}
				}
			}
			foreach (BurningSystem burningSystem6 in this._burningRecord.StabilizerFires)
			{
				burningSystem6.Tick(dt);
				if (burningSystem6.BurnedRope != null)
				{
					float num22 = this._burningRecord.FireDt / burningSystem6.GetBurningAnimationDuration();
					float num23 = 1f - (num22 - 0.4f);
					burningSystem6.BurnedRope.SetAlpha(MathF.Max(num23, 0.01f));
					burningSystem6.SetExternalFlameMultiplier(num23);
					if (num23 > 0f)
					{
						burningSystem6.CheckWater();
						flag = false;
					}
				}
			}
			this._burningRecord.BurningFinished = flag;
		}

		// Token: 0x06000C9F RID: 3231 RVA: 0x0005FC68 File Offset: 0x0005DE68
		private void PositionSailFireParticles()
		{
			Vec3 vec = Vec3.Zero;
			Vec3 vec2 = Vec3.Zero;
			Vec3 vec3 = Vec3.Zero;
			Vec3 vec4 = Vec3.Zero;
			foreach (SailVisual.FreeBoneRecord freeBoneRecord in this._freeBones)
			{
				if (freeBoneRecord.BoneIndex != -1)
				{
					if (freeBoneRecord.BoneType == SailVisual.FreeBoneType.Left)
					{
						vec = freeBoneRecord.CurrentLocalFrame.origin;
						vec3 = freeBoneRecord.InitialLocalFrame.origin;
					}
					else if (freeBoneRecord.BoneType == SailVisual.FreeBoneType.Right)
					{
						vec2 = freeBoneRecord.CurrentLocalFrame.origin;
						vec4 = freeBoneRecord.InitialLocalFrame.origin;
					}
				}
			}
			MatrixFrame globalFrame = this.SailSkeletonEntity.GetGlobalFrame();
			Vec3 vec5;
			vec5..ctor(-this._burningRecord.SailLengthX * 0.5f, 0f, this._burningRecord.SailLengthZ * 0.5f, -1f);
			Vec3 vec6 = new Vec3(this._burningRecord.SailLengthX * 0.5f, 0f, this._burningRecord.SailLengthZ * 0.5f, -1f);
			Vec3 vec7 = vec5 - vec3;
			Vec3 vec8 = vec6 - vec5;
			Vec3 vec9 = vec - vec3;
			Vec3 vec10 = vec2 - vec4;
			float num = 1f / (float)this._burningRecord.SailFires.Count;
			float num2 = 3.45f;
			float num3 = 0.62f;
			foreach (BurningSystem burningSystem in this._burningRecord.SailFires)
			{
				foreach (BurningNode burningNode in burningSystem.BurningNodes)
				{
					Vec3 vec11 = vec3;
					Vec2 sailStripLocation = burningNode.SailStripLocation;
					vec11 += vec8 * sailStripLocation.x;
					vec11 += vec7 * sailStripLocation.y;
					sailStripLocation.y = 1f - sailStripLocation.y;
					Vec3 vec12 = Vec3.Lerp(vec9, vec10, sailStripLocation.x);
					Vec3 vec13 = Vec3.Zero;
					if (sailStripLocation.y > num3)
					{
						float num4 = 1f - (sailStripLocation.y - num3) / MathF.Max(1f - num3, 0.01f);
						vec13 += vec12 * (1f + num2 * num4);
					}
					else
					{
						float num5 = sailStripLocation.y / MathF.Max(num3, 0.01f);
						vec13 += vec12 * num2 * num5;
					}
					if (sailStripLocation.x > 0.5f)
					{
						float num6 = 1f - (sailStripLocation.x - 0.5f) / 0.5f;
						vec13 += vec12 * 1.83f * num6;
					}
					else
					{
						float num7 = sailStripLocation.x / 0.5f;
						vec13 += vec12 * 1.83f * num7;
					}
					vec11 += vec13;
					MatrixFrame globalFrame2 = burningNode.GameEntity.GetGlobalFrame();
					globalFrame2.origin = globalFrame.TransformToParent(ref vec11);
					burningNode.GameEntity.SetGlobalFrame(ref globalFrame2, true);
				}
			}
		}

		// Token: 0x06000CA0 RID: 3232 RVA: 0x0006001C File Offset: 0x0005E21C
		private void PlaceTopFlag(float dx)
		{
			if (this._topFlagRope.RopeSegment != null && this.SailTopBannerEntity != null)
			{
				Vec3 origin = this._topFlagRope.RopeEntity.GetGlobalFrame().origin;
				Vec3 origin2 = this._topFlagRope.TargetEntity.GetGlobalFrame().origin;
				Vec3 vec = RopeSegment.CalculateAutoCurvePosition(origin, origin2, this._topFlagRope.RopeSegment.CurrentRopeLength, dx);
				MatrixFrame localFrame = this.SailTopBannerEntity.GetLocalFrame();
				Vec3 vec2 = this.SailTopBannerEntity.Parent.GetGlobalFrame().TransformToLocalNonOrthogonal(ref vec);
				localFrame.origin.z = vec2.z;
				this.SailTopBannerEntity.SetLocalFrame(ref localFrame, false);
			}
		}

		// Token: 0x06000CA1 RID: 3233 RVA: 0x000600D8 File Offset: 0x0005E2D8
		private void TickFlagCaptureAnimation(float dt)
		{
			this._captureTheFlagAnimation.DtTillStart += dt;
			if (this._captureTheFlagAnimation.DtTillStart < 4f)
			{
				float num = MathF.Clamp(this._captureTheFlagAnimation.DtTillStart / 4f, 0f, 1f);
				float num2 = MathF.Lerp(this._topFlagRopePosition, this._captureTheFlagBottomPosition, num, 1E-05f);
				this.PlaceTopFlag(num2);
				this._captureTheFlagAnimation.BannerWindFactor = 0.15f;
				if (base.GameEntity.IsInEditorScene())
				{
					Vec3 vec = new Vec3(base.Scene.GetGlobalWindVelocity() * 0.15f, 0f, -1f) / Scene.MaximumWindSpeed;
					this.SailTopBannerClothComponent.SetForcedWind(vec, false);
					return;
				}
			}
			else if (this._captureTheFlagAnimation.DtTillStart < 5f)
			{
				if (this.SailTopBannerClothComponent != null && !this._captureTheFlagAnimation.MaterialSet)
				{
					Mesh meshAtIndex = this.SailTopBannerClothComponent.GetFirstMetaMesh().GetMeshAtIndex(0);
					Material material = meshAtIndex.GetMaterial();
					material = material.CreateCopy();
					material.SetTexture(1, this._captureTheFlagAnimation.NewBannerTexture);
					meshAtIndex.SetMaterial(material);
					this._captureTheFlagAnimation.MaterialSet = true;
				}
				this._captureTheFlagAnimation.BannerWindFactor = 0.15f;
				if (base.GameEntity.IsInEditorScene())
				{
					Vec3 vec2 = new Vec3(base.Scene.GetGlobalWindVelocity() * 0.15f, 0f, -1f) / Scene.MaximumWindSpeed;
					this.SailTopBannerClothComponent.SetForcedWind(vec2, false);
					return;
				}
			}
			else if (this._captureTheFlagAnimation.DtTillStart < 9f)
			{
				float num3 = MathF.Clamp((this._captureTheFlagAnimation.DtTillStart - 5f) / 4f, 0f, 1f);
				float num4 = MathF.Lerp(this._captureTheFlagBottomPosition, this._topFlagRopePosition, num3, 1E-05f);
				this.PlaceTopFlag(num4);
				float num5 = MathF.Clamp((num3 - 0.8f) / 0.2f, 0.15f, 1f);
				this._captureTheFlagAnimation.BannerWindFactor = num5;
				if (base.GameEntity.IsInEditorScene())
				{
					Vec3 vec3 = new Vec3(base.Scene.GetGlobalWindVelocity() * num5, 0f, -1f) / Scene.MaximumWindSpeed;
					this.SailTopBannerClothComponent.SetForcedWind(vec3, false);
					return;
				}
			}
			else
			{
				this._captureTheFlagAnimation.AnimationInProgress = false;
				this._captureTheFlagAnimation.BannerWindFactor = 1f;
				this.SailTopBannerClothComponent.DisableForcedWind();
			}
		}

		// Token: 0x06000CA2 RID: 3234 RVA: 0x00060385 File Offset: 0x0005E585
		public bool IsBurningFinished()
		{
			return this._burningRecord.BurningFinished;
		}

		// Token: 0x06000CA3 RID: 3235 RVA: 0x00060392 File Offset: 0x0005E592
		public bool IsBurning()
		{
			return this._isBurning;
		}

		// Token: 0x06000CA4 RID: 3236 RVA: 0x0006039C File Offset: 0x0005E59C
		public void StartFire()
		{
			if (this._isBurning)
			{
				foreach (BurningSystem burningSystem in this._burningRecord.SailFires)
				{
					burningSystem.Remove();
				}
				if (this._burningRecord.YardLeftFire != null)
				{
					this._burningRecord.YardLeftFire.Remove();
				}
				if (this._burningRecord.YardRightFire != null)
				{
					this._burningRecord.YardRightFire.Remove();
				}
				if (this._burningRecord.MastFire != null)
				{
					this._burningRecord.MastFire.Remove();
				}
				foreach (BurningSystem burningSystem2 in this._burningRecord.RotatorFires)
				{
					burningSystem2.Remove();
				}
				foreach (BurningSystem burningSystem3 in this._burningRecord.StabilizerFires)
				{
					burningSystem3.Remove();
				}
				foreach (BurningSystem burningSystem4 in this._burningRecord.FoldFires)
				{
					burningSystem4.Remove();
				}
				foreach (BurningSystem burningSystem5 in this._burningRecord.StaticRopeFires)
				{
					burningSystem5.Remove();
				}
			}
			this._isBurning = true;
			this._burningRecord = new SailVisual.BurningRecord(true);
			Scene scene = base.GameEntity.Scene;
			bool flag = false;
			if (this.SailSkeletonEntity != null && this._sailSkeleton != null && flag)
			{
				MatrixFrame globalFrame = this.SailSkeletonEntity.GetGlobalFrame();
				Mesh mesh = null;
				using (IEnumerator<Mesh> enumerator2 = this._sailSkeleton.GetAllMeshes().GetEnumerator())
				{
					if (enumerator2.MoveNext())
					{
						mesh = enumerator2.Current;
					}
				}
				if (mesh != null)
				{
					int num = 6;
					Vec3 boundingBoxMax = mesh.GetBoundingBoxMax();
					Vec3 boundingBoxMin = mesh.GetBoundingBoxMin();
					Vec3 vec = boundingBoxMax - boundingBoxMin;
					float num2 = 1f / (float)num;
					float num3 = 1f / (float)num;
					this._burningRecord.SailLengthX = vec.x;
					this._burningRecord.SailLengthZ = vec.z;
					string text = "burning_node";
					float num4 = this._burningAnimationDuration / (float)num;
					for (int i = 0; i < num; i++)
					{
						GameEntity gameEntity = GameEntity.CreateEmpty(scene, true, true, true);
						gameEntity.Name = string.Format("sail_strip_root_{0}", i);
						this.SailSkeletonEntity.AddChild(gameEntity, false);
						BurningSystem burningSystem6 = new BurningSystem(gameEntity, 1f / num4);
						for (int j = 0; j < num; j++)
						{
							Vec2 zero = Vec2.Zero;
							zero.x = ((float)i + 0.1f + MBRandom.RandomFloat * 0.8f) * num2;
							zero.y = ((float)j + 0.1f + MBRandom.RandomFloat * 0.8f) * num3;
							float num5 = zero.x * vec.x + boundingBoxMin.x;
							float num6 = zero.y * -vec.z + boundingBoxMax.z;
							GameEntity gameEntity2 = GameEntity.Instantiate(base.GameEntity.Scene, text, true, true, "");
							gameEntity2.EntityFlags |= 131072;
							gameEntity.AddChild(gameEntity2, false);
							MatrixFrame matrixFrame = MatrixFrame.Identity;
							matrixFrame.origin.x = num5;
							matrixFrame.origin.z = num6;
							matrixFrame = globalFrame.TransformToParent(ref matrixFrame);
							gameEntity2.SetGlobalFrame(ref matrixFrame, true);
							gameEntity2.UpdateTriadFrameForEditor();
							BurningNode firstScriptOfType = gameEntity2.GetFirstScriptOfType<BurningNode>();
							if (firstScriptOfType != null)
							{
								firstScriptOfType.SetSailStripLocation(zero);
								burningSystem6.AddNewNode(firstScriptOfType);
								if (MBRandom.RandomFloat > 0.82f)
								{
									firstScriptOfType.EnableSparks();
								}
							}
						}
						this._burningRecord.SailFires.Add(burningSystem6);
					}
				}
			}
			if (this._mastEntity != null)
			{
				MatrixFrame globalFrame2 = this._mastEntity.GetGlobalFrame();
				Mesh firstMesh = this._mastEntity.GetFirstMesh();
				if (firstMesh != null)
				{
					GameEntity gameEntity3 = GameEntity.CreateEmpty(scene, true, true, true);
					gameEntity3.Name = "mastFireRoot";
					this._mastEntity.AddChild(gameEntity3, false);
					float num7 = this._burningAnimationDuration * 0.25f;
					this._burningRecord.InitialYardMastColor = Color.FromUint(firstMesh.Color);
					this._burningRecord.InitialYardMastColor.Alpha = 1f;
					Vec3 boundingBoxMin2 = firstMesh.GetBoundingBoxMin();
					Vec3 boundingBoxMax2 = firstMesh.GetBoundingBoxMax();
					Vec3 vec2;
					vec2..ctor(0f, 0f, boundingBoxMax2.z, -1f);
					Vec3 vec3;
					vec3..ctor(0f, 0f, boundingBoxMin2.z, -1f);
					float num8 = vec3.z + 4.35f;
					WeakGameEntity firstChildEntityWithTagRecursive = base.GameEntity.Root.GetFirstChildEntityWithTagRecursive("body_mesh");
					if (firstChildEntityWithTagRecursive != null)
					{
						Vec3 vec4 = globalFrame2.TransformToParent(ref vec2);
						Vec3 vec5 = globalFrame2.TransformToParent(ref vec3);
						Vec3 vec6 = vec4 - vec5;
						float num9 = vec6.Normalize();
						float num10 = -1f;
						if (firstChildEntityWithTagRecursive.RayHitEntity(vec2, vec6, num9, ref num10))
						{
							Vec3 vec7 = vec2 + vec6 * num10;
							vec3 = globalFrame2.TransformToLocalNonOrthogonal(ref vec7);
							num8 = vec3.z + 3f;
						}
					}
					float num11 = (vec3 - vec2).Normalize();
					float num12 = 2f;
					int num13 = (int)(num11 / num12);
					num13 = MathF.Max(0, num13 - 2);
					float num14 = num7 / (float)num13;
					this._burningRecord.MastFire = new BurningSystem(gameEntity3, 1f / num14);
					string text2 = "burning_node_yard";
					for (int k = 0; k < num13; k++)
					{
						GameEntity gameEntity4 = GameEntity.Instantiate(base.GameEntity.Scene, text2, true, true, "");
						if (!(gameEntity4 == null))
						{
							gameEntity3.AddChild(gameEntity4, false);
							BurningNode firstScriptOfType2 = gameEntity4.GetFirstScriptOfType<BurningNode>();
							if (firstScriptOfType2 != null)
							{
								this._burningRecord.MastFire.AddNewNode(firstScriptOfType2);
							}
							if (MBRandom.RandomFloat > 0.82f)
							{
								firstScriptOfType2.EnableSparks();
							}
							MatrixFrame identity = MatrixFrame.Identity;
							identity.origin.z = num8 + (float)k * num12;
							identity.rotation.RotateAboutForward(1.5707964f);
							gameEntity4.SetFrame(ref identity, true);
						}
					}
				}
			}
			if (this._yardMesh != null)
			{
				this._mastEntity.GetGlobalFrame();
				Vec3 boundingBoxMin3 = this._yardMesh.GetBoundingBoxMin();
				Vec3 boundingBoxMax3 = this._yardMesh.GetBoundingBoxMax();
				Vec3 vec8 = (boundingBoxMin3 + boundingBoxMax3) * 0.5f;
				string text3 = "burning_node_yard";
				GameEntity gameEntity5 = GameEntity.CreateEmpty(scene, true, true, true);
				gameEntity5.Name = "mastFireRootLeft";
				if (this._sailType == SailVisual.SailType.LateenSail)
				{
					this._lateenSailData.RollRotationEntity.AddChild(gameEntity5, false);
				}
				else
				{
					this._yardEntity.AddChild(gameEntity5, false);
				}
				float num15 = 2f;
				int num16 = (int)((vec8.x - boundingBoxMin3.x) / num15);
				float num17 = this._burningAnimationDuration * 0.25f / (float)num16;
				this._burningRecord.YardLeftFire = new BurningSystem(gameEntity5, 1f / num17);
				float y = vec8.y;
				for (int l = 0; l < num16; l++)
				{
					GameEntity gameEntity6 = GameEntity.Instantiate(base.GameEntity.Scene, text3, true, true, "");
					if (!(gameEntity6 == null))
					{
						gameEntity5.AddChild(gameEntity6, false);
						BurningNode firstScriptOfType3 = gameEntity6.GetFirstScriptOfType<BurningNode>();
						if (firstScriptOfType3 != null)
						{
							this._burningRecord.YardLeftFire.AddNewNode(firstScriptOfType3);
						}
						if (MBRandom.RandomFloat > 0.62f)
						{
							firstScriptOfType3.EnableSparks();
						}
						MatrixFrame identity2 = MatrixFrame.Identity;
						identity2.origin.x = y - (float)l * num15;
						gameEntity6.SetFrame(ref identity2, true);
					}
				}
				GameEntity gameEntity7 = GameEntity.CreateEmpty(scene, true, true, true);
				gameEntity7.Name = "mastFireRootRight";
				if (this._sailType == SailVisual.SailType.LateenSail)
				{
					this._lateenSailData.RollRotationEntity.AddChild(gameEntity7, false);
				}
				else
				{
					this._yardEntity.AddChild(gameEntity7, false);
				}
				float num18 = 2f;
				int num19 = (int)((boundingBoxMax3.x - vec8.x) / num18);
				float num20 = this._burningAnimationDuration * 0.25f / (float)num19;
				this._burningRecord.YardRightFire = new BurningSystem(gameEntity7, 1f / num20);
				float y2 = vec8.y;
				for (int m = 0; m < num19; m++)
				{
					GameEntity gameEntity8 = GameEntity.Instantiate(base.GameEntity.Scene, text3, true, true, "");
					if (!(gameEntity8 == null))
					{
						gameEntity7.AddChild(gameEntity8, false);
						BurningNode firstScriptOfType4 = gameEntity8.GetFirstScriptOfType<BurningNode>();
						if (firstScriptOfType4 != null)
						{
							this._burningRecord.YardRightFire.AddNewNode(firstScriptOfType4);
						}
						if (MBRandom.RandomFloat > 0.62f)
						{
							firstScriptOfType4.EnableSparks();
						}
						MatrixFrame identity3 = MatrixFrame.Identity;
						identity3.origin.x = y2 + (float)m * num18;
						gameEntity8.SetFrame(ref identity3, true);
					}
				}
			}
			this._burningRecord.RotatorFires = new List<BurningSystem>();
			foreach (SailVisual.FreeBoneRecord freeBoneRecord in this._freeBones)
			{
				if (freeBoneRecord.RotatorPulleys != null)
				{
					foreach (SailVisual.PulleyDataCache pulleyDataCache in freeBoneRecord.RotatorPulleys)
					{
						BurningSystem burningSystem7 = new BurningSystem(null, 2.7f, pulleyDataCache.PulleySystem);
						this._burningRecord.RotatorFires.Add(burningSystem7);
						pulleyDataCache.PulleySystem.FillBurningRecord(burningSystem7);
						float num21 = this._burningAnimationDuration * 0.5f / (float)burningSystem7.BurningNodes.Count;
						burningSystem7.SpreadRate = 1f / num21;
					}
				}
				if (freeBoneRecord.StabilityPulleys != null)
				{
					foreach (SailVisual.PulleyDataCache pulleyDataCache2 in freeBoneRecord.StabilityPulleys)
					{
						BurningSystem burningSystem8 = new BurningSystem(null, 2.7f, pulleyDataCache2.PulleySystem);
						this._burningRecord.StabilizerFires.Add(burningSystem8);
						pulleyDataCache2.PulleySystem.FillBurningRecord(burningSystem8);
						float num22 = this._burningAnimationDuration * 0.5f / (float)burningSystem8.BurningNodes.Count;
						burningSystem8.SpreadRate = 1f / num22;
					}
				}
				if (freeBoneRecord.FoldSailPulley.PulleySystem != null)
				{
					BurningSystem burningSystem9 = new BurningSystem(null, 4.7f, freeBoneRecord.FoldSailPulley.PulleySystem);
					this._burningRecord.FoldFires.Add(burningSystem9);
					freeBoneRecord.FoldSailPulley.PulleySystem.FillBurningRecord(burningSystem9);
					float num23 = this._burningAnimationDuration * 0.5f / (float)burningSystem9.BurningNodes.Count;
					burningSystem9.SpreadRate = 1f / num23;
				}
				if (freeBoneRecord.StabilityRopes != null)
				{
					float num24 = 2f;
					string text4 = "burning_node_rope";
					foreach (SailVisual.SimpleRopeRecord simpleRopeRecord in freeBoneRecord.StabilityRopes)
					{
						BurningSystem burningSystem10 = new BurningSystem(null, 1.2f, simpleRopeRecord.RopeSegment);
						simpleRopeRecord.RopeSegment.FillBurningRecordForSegment(burningSystem10, text4, num24, true);
						simpleRopeRecord.RopeSegment.BurnedClipReverseMode = true;
						if (burningSystem10.BurningNodes.Count > 0)
						{
							this._burningRecord.StabilizerFires.Add(burningSystem10);
							float num25 = this._burningAnimationDuration * 0.5f / (float)burningSystem10.BurningNodes.Count;
							burningSystem10.SpreadRate = 1f / num25;
						}
					}
				}
			}
			float num26 = 2f;
			string text5 = "burning_node_rope";
			foreach (SailVisual.SimpleRopeRecord simpleRopeRecord2 in this._simpleRopes)
			{
				if (MBRandom.RandomFloat >= 0.3f)
				{
					BurningSystem burningSystem11 = new BurningSystem(null, 1.4f, simpleRopeRecord2.RopeSegment);
					simpleRopeRecord2.RopeSegment.FillBurningRecordForSegment(burningSystem11, text5, num26, false);
					if (burningSystem11.BurningNodes.Count > 0)
					{
						this._burningRecord.StaticRopeFires.Add(burningSystem11);
						float num27 = this._burningAnimationDuration * 0.5f / (float)burningSystem11.BurningNodes.Count;
						burningSystem11.SpreadRate = 1f / num27;
					}
				}
			}
			if (this.SailEnabled)
			{
				if (this._burningSailEntity != null)
				{
					this._burningSailEntity.SetVisibilityExcludeParents(true);
					this.SailSkeletonEntity.SetVisibilityExcludeParents(false);
				}
			}
			else if (this._burningSailMesh != null)
			{
				this._foldedStaticSailMesh.SetMaterial(this._burningSailMesh.GetMaterial());
				foreach (Mesh mesh2 in this._foldedStaticSailEntity.GetAllMeshesWithTag("static_ropes"))
				{
					mesh2.SetVisibilityMask(0);
				}
			}
			if (this._topLateenFireMaterial != null && this._currentSailLevelUsed == 3 && this._sailType == SailVisual.SailType.SquareSail)
			{
				foreach (WeakGameEntity weakGameEntity in this._topLateenSails)
				{
					weakGameEntity.SetDoNotCheckVisibility(true);
					foreach (Mesh mesh3 in weakGameEntity.GetAllMeshesWithTag("faction_color"))
					{
						mesh3.SetMaterial(this._topLateenFireMaterial);
					}
				}
				foreach (WeakGameEntity weakGameEntity2 in this._topLateenFoldedSails)
				{
					weakGameEntity2.SetDoNotCheckVisibility(true);
					foreach (Mesh mesh4 in weakGameEntity2.GetAllMeshesWithTag("faction_color"))
					{
						mesh4.SetMaterial(this._topLateenFireMaterial);
					}
				}
			}
		}

		// Token: 0x06000CA5 RID: 3237 RVA: 0x000613CC File Offset: 0x0005F5CC
		private void ComputeMastClipPlane()
		{
			WeakGameEntity firstChildEntityWithTagRecursive = base.GameEntity.Root.GetFirstChildEntityWithTagRecursive("body_mesh");
			if (firstChildEntityWithTagRecursive != null && this._mastEntity != null)
			{
				float num = 30f;
				MatrixFrame globalFrame = this._mastEntity.GetGlobalFrame();
				Vec3 u = globalFrame.rotation.u;
				Vec3 vec = globalFrame.origin - num * u;
				float num2 = -1f;
				if (firstChildEntityWithTagRecursive.RayHitEntity(vec, u, num * 2f, ref num2))
				{
					this._mastClipDistanceFromOrigin = num - num2;
				}
			}
		}

		// Token: 0x06000CA6 RID: 3238 RVA: 0x00061464 File Offset: 0x0005F664
		private void UpdateMastClipPlane()
		{
			if (this._mastEntity != null)
			{
				MatrixFrame globalFrame = this._mastEntity.GetGlobalFrame();
				Vec3 vec = globalFrame.origin - globalFrame.rotation.u * this._mastClipDistanceFromOrigin;
				this._mastEntity.SetCustomClipPlane(vec, globalFrame.rotation.u, false);
			}
		}

		// Token: 0x06000CA7 RID: 3239 RVA: 0x000614C8 File Offset: 0x0005F6C8
		public void GetDimensions(in MatrixFrame shipFrame, bool isLateen, out float width, out float height, out Vec3 center)
		{
			MatrixFrame globalFrame = this.SailSkeletonEntity.GetGlobalFrame();
			Vec3 scaleVector = globalFrame.rotation.GetScaleVector();
			BoundingBox boundingBox = this.SailClothComponent.GetFirstMetaMesh().GetBoundingBox();
			Vec3 vec = boundingBox.max - boundingBox.min;
			width = vec.x * scaleVector.x;
			height = vec.z * scaleVector.z;
			if (isLateen)
			{
				height = MathF.Sqrt(width * width + height * height) * 0.88f;
			}
			MatrixFrame matrixFrame = shipFrame;
			center = matrixFrame.TransformToLocalNonOrthogonal(ref globalFrame).TransformToParent(ref boundingBox.center);
		}

		// Token: 0x06000CA8 RID: 3240 RVA: 0x00061578 File Offset: 0x0005F778
		public void SetBallistaRopeVisibility(bool value)
		{
			if (value)
			{
				this._ballistaRopeEnableFrameCounter = 2;
				return;
			}
			foreach (WeakGameEntity weakGameEntity in this._ballistaVisibilityRopes)
			{
				weakGameEntity.SetVisibilityExcludeParents(value);
			}
			this._ballistaRopeEnableFrameCounter = 0;
		}

		// Token: 0x06000CA9 RID: 3241 RVA: 0x000615E0 File Offset: 0x0005F7E0
		public void StartFlagCaptureAnimation(Texture newTexture)
		{
			if (this.SailTopBannerClothComponent != null && this.SailTopBannerClothComponent.GetFirstMetaMesh().GetMeshAtIndex(0).GetMaterial()
				.GetTexture(1) != newTexture)
			{
				this._captureTheFlagAnimation.AnimationInProgress = true;
				this._captureTheFlagAnimation.NewBannerTexture = newTexture;
				this._captureTheFlagAnimation.DtTillStart = 0f;
				this._captureTheFlagAnimation.MaterialSet = false;
			}
		}

		// Token: 0x04000728 RID: 1832
		private const string SailMeshEntityTag = "sail_mesh_entity";

		// Token: 0x04000729 RID: 1833
		private const string StaticFoldedSailMeshEntityTag = "folded_static_entity";

		// Token: 0x0400072A RID: 1834
		private const string SailTopBannerTag = "bd_banner_b";

		// Token: 0x0400072B RID: 1835
		private const string FreeBoneTag = "free_bone";

		// Token: 0x0400072C RID: 1836
		private const string RollRotationEntityTag = "roll_rotation_entity";

		// Token: 0x0400072D RID: 1837
		private const string YawRotationEntityTag = "yaw_rotation_entity";

		// Token: 0x0400072E RID: 1838
		private const string YardShiftEntityTag = "yard_shift";

		// Token: 0x0400072F RID: 1839
		private const string SailYardEntityTag = "sail_yard";

		// Token: 0x04000730 RID: 1840
		private const string PulleySystemsParentTag = "pulley_systems_parent";

		// Token: 0x04000731 RID: 1841
		private const string FoldPulleysParentTag = "sail_fold_pulleys";

		// Token: 0x04000732 RID: 1842
		private const string RotatePulleysParentTag = "sail_rotate_pulleys";

		// Token: 0x04000733 RID: 1843
		private const string StabilityRopesParentTag = "stability_ropes_parent";

		// Token: 0x04000734 RID: 1844
		private const string StaticRopesParentTag = "static_ropes_parent";

		// Token: 0x04000735 RID: 1845
		private const string MastEntityTag = "mast_entity";

		// Token: 0x04000736 RID: 1846
		private const string SimpleRopeTag = "simple_rope";

		// Token: 0x04000737 RID: 1847
		private const string SimpleRopeStartTag = "simple_rope_start";

		// Token: 0x04000738 RID: 1848
		private const string SimpleRopeEndTag = "simple_rope_end";

		// Token: 0x04000739 RID: 1849
		private const string AttachedToYardTag = "attached_to_yard";

		// Token: 0x0400073A RID: 1850
		private const string KnobPointsParentTag = "knob_points_parent";

		// Token: 0x0400073B RID: 1851
		private const string KnobPointTag = "knot_point";

		// Token: 0x0400073C RID: 1852
		private const string KnobPointDynamicTag = "dynamic_knob";

		// Token: 0x0400073D RID: 1853
		private const string YardMeshEntity = "yard_mesh";

		// Token: 0x0400073E RID: 1854
		private const string SailMeshBurningEntity = "sail_mesh_free_entity";

		// Token: 0x0400073F RID: 1855
		private const string SquareSailLvl3ShiftEntityTag = "lvl3_shift_entity";

		// Token: 0x04000740 RID: 1856
		private const string SquareSailLvl3Visibilitytag = "lvl3_lateens";

		// Token: 0x04000741 RID: 1857
		private const string SquareSailLvl3MeshHoldertag = "lvl3_lateens_entity";

		// Token: 0x04000742 RID: 1858
		private const string SquareSailLvl3FoldedParentTag = "lvl3_lateens_folded";

		// Token: 0x04000743 RID: 1859
		private const string BallistaVisibilityRopeTag = "ballista_visibility";

		// Token: 0x04000744 RID: 1860
		private const string TopFlagRopeTag = "flag_capture_rope";

		// Token: 0x04000745 RID: 1861
		private static readonly string[] ClothFragmentPrefabs = new string[] { "cloth_fragment_a", "cloth_fragment_b", "cloth_fragment_c", "cloth_fragment_e", "cloth_fragment_g", "cloth_fragment_i", "cloth_fragment_d", "cloth_fragment_h" };

		// Token: 0x04000746 RID: 1862
		private const float InvisibleDistanceSquared = 22500f;

		// Token: 0x04000747 RID: 1863
		private const float LinearDistanceSquared = 2025f;

		// Token: 0x04000748 RID: 1864
		private static readonly int SailUnfoldSoundEventId = SoundManager.GetEventGlobalIndex("event:/mission/movement/vessel/sail/sail_open");

		// Token: 0x04000749 RID: 1865
		private static readonly int SailFoldSoundEventId = SoundManager.GetEventGlobalIndex("event:/mission/movement/vessel/sail/sail_close");

		// Token: 0x0400074A RID: 1866
		private static readonly int LateenSailRollSoundEventId = SoundManager.GetEventGlobalIndex("event:/mission/movement/vessel/sail/lateen_rotation");

		// Token: 0x0400074B RID: 1867
		private List<SailVisual.KnobConnectionPoint> _knobConnectionPoints = new List<SailVisual.KnobConnectionPoint>();

		// Token: 0x0400074C RID: 1868
		[EditableScriptComponentVariable(true, "Fold Sail Duration")]
		private float _foldSailDuration = 3f;

		// Token: 0x0400074D RID: 1869
		[EditableScriptComponentVariable(true, "Folded Sail Transition Duration")]
		private float _foldedSailTransitionDuration = 0.5f;

		// Token: 0x0400074E RID: 1870
		[EditableScriptComponentVariable(true, "Fold Free Bone Reset Duration")]
		private float _foldFreeBoneResetDuration = 1.2f;

		// Token: 0x0400074F RID: 1871
		[EditableScriptComponentVariable(true, "Unfold Sail Duration")]
		private float _unfoldSailDuration = 4f;

		// Token: 0x04000750 RID: 1872
		[EditableScriptComponentVariable(true, "Fold Sail Step Multiplier")]
		private float _foldSailStepMultiplier = 2f;

		// Token: 0x04000751 RID: 1873
		[EditableScriptComponentVariable(true, "Lateen Yard Shift")]
		private float _lateenYardShift;

		// Token: 0x04000752 RID: 1874
		[EditableScriptComponentVariable(true, "Lateen Roll Change Degree Limit")]
		private float _lateenRollChangeDegreeLimit = 20f;

		// Token: 0x04000753 RID: 1875
		[EditableScriptComponentVariable(true, "Lateen Roll Change Animation Duration")]
		private float _lateenRollChangeAnimationDuration = 3f;

		// Token: 0x04000754 RID: 1876
		[EditableScriptComponentVariable(true, "Lateen Roll Change Animation Step Multiplier")]
		private float _lateenRollChangeAnimationStepMultiplier = 1f;

		// Token: 0x04000755 RID: 1877
		[EditableScriptComponentVariable(true, "Lateen Roll Change Yard Shift Start")]
		private float _lateenRollChangeYardShiftStart = 3f;

		// Token: 0x04000756 RID: 1878
		[EditableScriptComponentVariable(true, "Lateen Roll Change Yard Shift Duration")]
		private float _lateenRollChangeYardShiftDuration = 4f;

		// Token: 0x04000757 RID: 1879
		[EditableScriptComponentVariable(true, "Lateen Roll Change Yard Shift Acceleration")]
		private float _lateenRollChangeYardShiftAcceleration = 8f;

		// Token: 0x04000758 RID: 1880
		[EditableScriptComponentVariable(true, "Lateen Roll Degrees")]
		private float _lateenRollDegrees = 45f;

		// Token: 0x04000759 RID: 1881
		[EditableScriptComponentVariable(true, "Rope Connection Max Distance")]
		private float _ropeConnectionMaxDistance = 7f;

		// Token: 0x0400075A RID: 1882
		[EditableScriptComponentVariable(true, "Knob Type")]
		private SailVisual.KnobTypeEnum _knobType;

		// Token: 0x0400075B RID: 1883
		[EditableScriptComponentVariable(true, "Place Knobs")]
		private SimpleButton _placeKnobButton = new SimpleButton();

		// Token: 0x0400075C RID: 1884
		[EditableScriptComponentVariable(true, "Knob Color")]
		private Color _placeKnobColor = Color.White;

		// Token: 0x0400075D RID: 1885
		[EditableScriptComponentVariable(true, "Start Fire")]
		private SimpleButton _startFireButton = new SimpleButton();

		// Token: 0x0400075E RID: 1886
		[EditableScriptComponentVariable(true, "Place Cloth Fragments")]
		private SimpleButton _placeClothFragments = new SimpleButton();

		// Token: 0x0400075F RID: 1887
		[EditableScriptComponentVariable(true, "Sail Type")]
		private SailVisual.SailType _sailType;

		// Token: 0x04000760 RID: 1888
		[EditableScriptComponentVariable(true, "Burning Animation Duration")]
		private float _burningAnimationDuration = 20f;

		// Token: 0x04000761 RID: 1889
		private SailVisual.LateenSailData _lateenSailData;

		// Token: 0x04000762 RID: 1890
		[EditableScriptComponentVariable(true, "Square Lvl3 Mast Shift")]
		private float _squareLvl3MastShift;

		// Token: 0x04000763 RID: 1891
		[EditableScriptComponentVariable(true, "Editor Only Level Selection")]
		private SailVisual.LevelForEditor _editorOnlyLevelSelection;

		// Token: 0x04000764 RID: 1892
		[EditableScriptComponentVariable(true, "Top Lateen Fire Material")]
		private Material _topLateenFireMaterial;

		// Token: 0x04000765 RID: 1893
		[EditableScriptComponentVariable(true, "Editor Only Ship Health")]
		private float _editorOnlyShipHealth = 1f;

		// Token: 0x04000766 RID: 1894
		[EditableScriptComponentVariable(true, "Top Flag Rope Position")]
		private float _topFlagRopePosition = 0.8f;

		// Token: 0x04000767 RID: 1895
		[EditableScriptComponentVariable(true, "Capture Flag Bottom Rope Position")]
		private float _captureTheFlagBottomPosition = 0.25f;

		// Token: 0x04000768 RID: 1896
		[EditableScriptComponentVariable(true, "Start Capture The Flag Animation")]
		private SimpleButton _startCaptureTheFlagAnimation = new SimpleButton();

		// Token: 0x04000769 RID: 1897
		private SailVisual.SailFoldProgress _ongoingAnimationData;

		// Token: 0x0400076A RID: 1898
		private readonly List<SailVisual.FreeBoneRecord> _freeBones = new List<SailVisual.FreeBoneRecord>();

		// Token: 0x0400076B RID: 1899
		private readonly List<SailVisual.SimpleRopeRecord> _simpleRopes = new List<SailVisual.SimpleRopeRecord>();

		// Token: 0x0400076C RID: 1900
		private readonly List<SailVisual.SimpleRopeRecord> _mastRopes = new List<SailVisual.SimpleRopeRecord>();

		// Token: 0x0400076D RID: 1901
		private Skeleton _sailSkeleton;

		// Token: 0x0400076E RID: 1902
		private float _totalFoldDuration;

		// Token: 0x0400076F RID: 1903
		private float _totalUnfoldDuration;

		// Token: 0x04000770 RID: 1904
		private float _mastClipDistanceFromOrigin = 100f;

		// Token: 0x04000775 RID: 1909
		private GameEntity _mastEntity;

		// Token: 0x04000776 RID: 1910
		private GameEntity _yardEntity;

		// Token: 0x04000778 RID: 1912
		private Mesh _foldedStaticSailMesh;

		// Token: 0x04000779 RID: 1913
		private GameEntity _foldedStaticSailEntity;

		// Token: 0x0400077A RID: 1914
		private GameEntity _knobParent;

		// Token: 0x0400077B RID: 1915
		private SailVisual.SimpleRopeRecord _topFlagRope;

		// Token: 0x0400077C RID: 1916
		private GameEntity _burningSailEntity;

		// Token: 0x0400077D RID: 1917
		private Mesh _burningSailMesh;

		// Token: 0x0400077E RID: 1918
		private Vec3 _currentFrameGlobalWind = Vec3.Zero;

		// Token: 0x0400077F RID: 1919
		private Mesh _yardMesh;

		// Token: 0x04000780 RID: 1920
		private MatrixFrame _previousYawEntityFrame = MatrixFrame.Identity;

		// Token: 0x04000781 RID: 1921
		private MatrixFrame _previousSailYardFrame = MatrixFrame.Identity;

		// Token: 0x04000782 RID: 1922
		private float _cumulativeDt;

		// Token: 0x04000783 RID: 1923
		private int _resetClothMeshFrameCounter;

		// Token: 0x04000784 RID: 1924
		private bool _ropesAreInvisibleThisFrame;

		// Token: 0x04000785 RID: 1925
		private bool _ropesWereInvisibleLastFrame;

		// Token: 0x04000786 RID: 1926
		private bool _ropesWereLinearLastFrame;

		// Token: 0x04000787 RID: 1927
		private bool _lodCheckFirstFrame = true;

		// Token: 0x04000788 RID: 1928
		private List<WeakGameEntity> _topLateenSails = new List<WeakGameEntity>();

		// Token: 0x04000789 RID: 1929
		private List<WeakGameEntity> _topLateenFoldedSails = new List<WeakGameEntity>();

		// Token: 0x0400078A RID: 1930
		private List<WeakGameEntity> _ballistaVisibilityRopes = new List<WeakGameEntity>();

		// Token: 0x0400078B RID: 1931
		private int _ballistaRopeEnableFrameCounter;

		// Token: 0x0400078C RID: 1932
		private int _currentSailLevelUsed = -1;

		// Token: 0x0400078D RID: 1933
		private SailVisual.BurningRecord _burningRecord;

		// Token: 0x0400078E RID: 1934
		private bool _isBurning;

		// Token: 0x0400078F RID: 1935
		private float _sailEntityAlpha = 1f;

		// Token: 0x04000790 RID: 1936
		private float _lastMorphAnimKeySet = -1f;

		// Token: 0x04000791 RID: 1937
		private int _remainingFramesForAnimation = 3;

		// Token: 0x04000796 RID: 1942
		private float _foldAnimWindReductionFactor = 1f;

		// Token: 0x04000797 RID: 1943
		private SailVisual.FlagCaptureAnimation _captureTheFlagAnimation;

		// Token: 0x0200021E RID: 542
		internal struct BurningRecord
		{
			// Token: 0x06001B17 RID: 6935 RVA: 0x000B2108 File Offset: 0x000B0308
			internal BurningRecord(bool _ = false)
			{
				this.SailFires = new List<BurningSystem>();
				this.MastFire = null;
				this.YardLeftFire = null;
				this.YardRightFire = null;
				this.RotatorFires = new List<BurningSystem>();
				this.StabilizerFires = new List<BurningSystem>();
				this.FoldFires = new List<BurningSystem>();
				this.StaticRopeFires = new List<BurningSystem>();
				this.SailLengthX = 0f;
				this.SailLengthZ = 0f;
				this.FireDt = 0f;
				this.YardFireStartDt = 0f;
				this.RotatorFireStartDt = 0f;
				this.InitialYardMastColor = Color.White;
				this.BurningFinished = false;
			}

			// Token: 0x04000EF0 RID: 3824
			internal List<BurningSystem> SailFires;

			// Token: 0x04000EF1 RID: 3825
			internal BurningSystem MastFire;

			// Token: 0x04000EF2 RID: 3826
			internal float SailLengthZ;

			// Token: 0x04000EF3 RID: 3827
			internal BurningSystem YardLeftFire;

			// Token: 0x04000EF4 RID: 3828
			internal float FireDt;

			// Token: 0x04000EF5 RID: 3829
			internal BurningSystem YardRightFire;

			// Token: 0x04000EF6 RID: 3830
			internal float YardFireStartDt;

			// Token: 0x04000EF7 RID: 3831
			internal List<BurningSystem> RotatorFires;

			// Token: 0x04000EF8 RID: 3832
			internal float RotatorFireStartDt;

			// Token: 0x04000EF9 RID: 3833
			internal Color InitialYardMastColor;

			// Token: 0x04000EFA RID: 3834
			internal List<BurningSystem> StabilizerFires;

			// Token: 0x04000EFB RID: 3835
			internal bool BurningFinished;

			// Token: 0x04000EFC RID: 3836
			internal List<BurningSystem> FoldFires;

			// Token: 0x04000EFD RID: 3837
			internal List<BurningSystem> StaticRopeFires;

			// Token: 0x04000EFE RID: 3838
			internal float SailLengthX;
		}

		// Token: 0x0200021F RID: 543
		internal struct SailFoldProgress
		{
			// Token: 0x04000EFF RID: 3839
			internal const float FoldUnfoldSoundEventAnimationDxStopThreshold = 0.875f;

			// Token: 0x04000F00 RID: 3840
			internal float CurrentProgress;

			// Token: 0x04000F01 RID: 3841
			internal float RealProgress;

			// Token: 0x04000F02 RID: 3842
			internal bool FoldIsOngoing;

			// Token: 0x04000F03 RID: 3843
			internal bool UnfoldIsOngoing;

			// Token: 0x04000F04 RID: 3844
			internal int NumberOfMorphKeys;

			// Token: 0x04000F05 RID: 3845
			internal Vec3[] LeftVertexPositions;

			// Token: 0x04000F06 RID: 3846
			internal Vec3[] RightVertexPositions;

			// Token: 0x04000F07 RID: 3847
			internal Vec3[] CenterVertexPositions;

			// Token: 0x04000F08 RID: 3848
			internal Vec3 CurrentLeftFreeBonePosition;

			// Token: 0x04000F09 RID: 3849
			internal Vec3 CurrentRightFreeBonePosition;

			// Token: 0x04000F0A RID: 3850
			internal Vec3 CurrentCenterFreeBonePosition;

			// Token: 0x04000F0B RID: 3851
			internal SoundEvent FoldUnfoldSoundEvent;

			// Token: 0x04000F0C RID: 3852
			internal bool ShouldMakeFoldUnfoldSound;

			// Token: 0x04000F0D RID: 3853
			internal bool ShouldStopFoldUnfoldSound;
		}

		// Token: 0x02000220 RID: 544
		internal struct LateenSailData
		{
			// Token: 0x04000F0E RID: 3854
			internal GameEntity RollRotationEntity;

			// Token: 0x04000F0F RID: 3855
			internal GameEntity YardShiftEntity;

			// Token: 0x04000F10 RID: 3856
			internal float LastYawSection;

			// Token: 0x04000F11 RID: 3857
			internal float RollRotationAnimProgress;

			// Token: 0x04000F12 RID: 3858
			internal float RollRotationRealDt;

			// Token: 0x04000F13 RID: 3859
			internal bool RollRotationInProgress;

			// Token: 0x04000F14 RID: 3860
			internal float RollRotationInitial;

			// Token: 0x04000F15 RID: 3861
			internal float RollRotationTarget;

			// Token: 0x04000F16 RID: 3862
			internal float YardShiftInitial;

			// Token: 0x04000F17 RID: 3863
			internal float YardShiftTarget;

			// Token: 0x04000F18 RID: 3864
			internal SoundEvent RollAnimationSoundEvent;
		}

		// Token: 0x02000221 RID: 545
		internal struct PulleyDataCache
		{
			// Token: 0x04000F19 RID: 3865
			internal GameEntity Entity;

			// Token: 0x04000F1A RID: 3866
			internal PulleySystem PulleySystem;
		}

		// Token: 0x02000222 RID: 546
		internal struct SimpleRopeRecord
		{
			// Token: 0x04000F1B RID: 3867
			internal GameEntity ParentEntity;

			// Token: 0x04000F1C RID: 3868
			internal GameEntity RopeEntity;

			// Token: 0x04000F1D RID: 3869
			internal GameEntity TargetEntity;

			// Token: 0x04000F1E RID: 3870
			internal RopeSegment RopeSegment;

			// Token: 0x04000F1F RID: 3871
			internal bool StartPointAttachedToYard;

			// Token: 0x04000F20 RID: 3872
			internal bool EndPointAttachedToYard;

			// Token: 0x04000F21 RID: 3873
			internal bool IsBigRope;
		}

		// Token: 0x02000223 RID: 547
		internal struct KnobConnectionPoint
		{
			// Token: 0x06001B18 RID: 6936 RVA: 0x000B21AA File Offset: 0x000B03AA
			internal void UpdateGlobalPosition(Vec3 pos)
			{
				this.GlobalPosition = pos;
			}

			// Token: 0x06001B19 RID: 6937 RVA: 0x000B21B3 File Offset: 0x000B03B3
			internal void UpdateRightOfYard(bool value)
			{
				this.RightOfYard = value;
			}

			// Token: 0x04000F22 RID: 3874
			internal Vec3 ShipLocalPosition;

			// Token: 0x04000F23 RID: 3875
			internal Vec3 GlobalPosition;

			// Token: 0x04000F24 RID: 3876
			internal bool IsFixed;

			// Token: 0x04000F25 RID: 3877
			internal bool RightOfYard;
		}

		// Token: 0x02000224 RID: 548
		internal class FreeBoneRecord
		{
			// Token: 0x04000F26 RID: 3878
			internal MatrixFrame InitialLocalFrame;

			// Token: 0x04000F27 RID: 3879
			internal MatrixFrame CurrentLocalFrame;

			// Token: 0x04000F28 RID: 3880
			internal Vec3 CurrentFrameWithoutRandomWind;

			// Token: 0x04000F29 RID: 3881
			internal GameEntity Entity;

			// Token: 0x04000F2A RID: 3882
			internal SailVisual.PulleyDataCache FoldSailPulley;

			// Token: 0x04000F2B RID: 3883
			internal List<SailVisual.PulleyDataCache> RotatorPulleys;

			// Token: 0x04000F2C RID: 3884
			internal List<SailVisual.PulleyDataCache> StabilityPulleys;

			// Token: 0x04000F2D RID: 3885
			internal List<SailVisual.SimpleRopeRecord> StabilityRopes;

			// Token: 0x04000F2E RID: 3886
			internal sbyte BoneIndex;

			// Token: 0x04000F2F RID: 3887
			internal SailVisual.FreeBoneConnectionType ConnectionType;

			// Token: 0x04000F30 RID: 3888
			internal SailVisual.FreeBoneType BoneType;
		}

		// Token: 0x02000225 RID: 549
		internal class FlagCaptureAnimation
		{
			// Token: 0x04000F31 RID: 3889
			internal bool AnimationInProgress;

			// Token: 0x04000F32 RID: 3890
			internal Texture NewBannerTexture;

			// Token: 0x04000F33 RID: 3891
			internal float DtTillStart;

			// Token: 0x04000F34 RID: 3892
			internal bool MaterialSet;

			// Token: 0x04000F35 RID: 3893
			internal float BannerWindFactor;
		}

		// Token: 0x02000226 RID: 550
		internal enum FreeBoneConnectionType
		{
			// Token: 0x04000F37 RID: 3895
			All,
			// Token: 0x04000F38 RID: 3896
			Closest,
			// Token: 0x04000F39 RID: 3897
			ClosestTwo
		}

		// Token: 0x02000227 RID: 551
		public enum SailType
		{
			// Token: 0x04000F3B RID: 3899
			SquareSail,
			// Token: 0x04000F3C RID: 3900
			LateenSail
		}

		// Token: 0x02000228 RID: 552
		internal enum KnobTypeEnum
		{
			// Token: 0x04000F3E RID: 3902
			Bollard,
			// Token: 0x04000F3F RID: 3903
			Cleat,
			// Token: 0x04000F40 RID: 3904
			Belaying
		}

		// Token: 0x02000229 RID: 553
		internal enum FreeBoneType
		{
			// Token: 0x04000F42 RID: 3906
			Left,
			// Token: 0x04000F43 RID: 3907
			Right,
			// Token: 0x04000F44 RID: 3908
			Center
		}

		// Token: 0x0200022A RID: 554
		internal enum LevelForEditor
		{
			// Token: 0x04000F46 RID: 3910
			None,
			// Token: 0x04000F47 RID: 3911
			Lvl1,
			// Token: 0x04000F48 RID: 3912
			Lvl2,
			// Token: 0x04000F49 RID: 3913
			Lvl3
		}
	}
}
