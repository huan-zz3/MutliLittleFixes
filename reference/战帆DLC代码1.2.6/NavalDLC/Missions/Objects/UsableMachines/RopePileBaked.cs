using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.Objects.UsableMachines
{
	// Token: 0x020000B0 RID: 176
	public class RopePileBaked : ScriptComponentBehavior
	{
		// Token: 0x06000D45 RID: 3397 RVA: 0x00068C80 File Offset: 0x00066E80
		protected override void OnEditorInit()
		{
			base.OnEditorInit();
			this._ropeMesh = base.GameEntity.GetFirstMesh();
		}

		// Token: 0x06000D46 RID: 3398 RVA: 0x00068CA8 File Offset: 0x00066EA8
		protected override void OnInit()
		{
			base.OnInit();
			this._ropeMesh = base.GameEntity.GetFirstMesh();
			this._ropeMesh.SetupAdditionalBoneBuffer(7);
			this._ropePileBaseBoundingBox = base.GameEntity.GetLocalBoundingBox();
			this._localUpdatedBoundingBox = this._ropePileBaseBoundingBox;
			base.GameEntity.SetHasCustomBoundingBoxValidationSystem(true);
		}

		// Token: 0x06000D47 RID: 3399 RVA: 0x00068D0C File Offset: 0x00066F0C
		protected override void OnBoundingBoxValidate()
		{
			BoundingBox boundingBox = default(BoundingBox);
			boundingBox.BeginRelaxation();
			if (base.GameEntity.ChildCount > 0)
			{
				boundingBox = base.GameEntity.ComputeBoundingBoxIncludeChildren();
			}
			boundingBox.RelaxWithBoundingBox(this._localUpdatedBoundingBox);
			boundingBox.RecomputeRadius();
			base.GameEntity.RelaxLocalBoundingBox(ref boundingBox);
		}

		// Token: 0x06000D48 RID: 3400 RVA: 0x00068D6C File Offset: 0x00066F6C
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 0;
		}

		// Token: 0x06000D49 RID: 3401 RVA: 0x00068D6F File Offset: 0x00066F6F
		public MatrixFrame UpdateRopeMeshVisualAccordingToTargetPoint(in Vec3 sourceGlobalPosition, in Vec3 targetGlobalPosition, in Vec3 globalVelocity, float time)
		{
			return this.ComputeFreeFallPoints(in sourceGlobalPosition, in targetGlobalPosition, in globalVelocity, time);
		}

		// Token: 0x06000D4A RID: 3402 RVA: 0x00068D7C File Offset: 0x00066F7C
		public Vec3 UpdateRopeMeshVisualAccordingToTargetPointLinear(in Vec3 sourceGlobalPosition, in Vec3 targetGlobalPosition)
		{
			return this.ComputeFreeFallPointsLinear(in sourceGlobalPosition, in targetGlobalPosition);
		}

		// Token: 0x06000D4B RID: 3403 RVA: 0x00068D86 File Offset: 0x00066F86
		public Vec3 UpdateRopeMeshVisualAccordingToTargetPointLinearWithoutBoundingBoxUpdate(in Vec3 sourceGlobalPosition, in Vec3 targetGlobalPosition)
		{
			return this.ComputeFreeFallPointsLinearWithoutBoundingBoxUpdate(in sourceGlobalPosition, in targetGlobalPosition);
		}

		// Token: 0x06000D4C RID: 3404 RVA: 0x00068D90 File Offset: 0x00066F90
		private Vec3 GetPositionAtProjectileCurveProgress(in Vec3 globalVelocity, in Vec3 sourceGlobalPosition, float time, int progressInterval)
		{
			if (progressInterval < 64)
			{
				time *= (float)progressInterval / 63f;
				return sourceGlobalPosition + globalVelocity * time + 0.5f * MBGlobals.GravitationalAcceleration * time * time;
			}
			return Vec3.Zero;
		}

		// Token: 0x06000D4D RID: 3405 RVA: 0x00068DF0 File Offset: 0x00066FF0
		private Vec3 ComputeFreeFallPointsLinearWithoutBoundingBoxUpdate(in Vec3 sourceGlobalPosition, in Vec3 targetGlobalPosition)
		{
			Vec3 vec = targetGlobalPosition;
			Vec3 vec2 = targetGlobalPosition - sourceGlobalPosition;
			Vec3 vec3 = vec - vec2.NormalizedCopy() * 0.5f;
			Vec3 vec4 = base.GameEntity.GetGlobalFrame().TransformToLocalNonOrthogonal(ref sourceGlobalPosition);
			Vec3 vec5 = base.GameEntity.GetGlobalFrame().TransformToLocalNonOrthogonal(ref vec3);
			vec2 = new Vec3(2f, 0f, 0f, -1f);
			Mat3 mat = new Mat3(ref vec4, ref vec2, ref vec5);
			MatrixFrame matrixFrame = new MatrixFrame(ref mat, ref vec5);
			this._ropeMesh.SetAdditionalBoneFrame(0, ref matrixFrame);
			vec2 = new Vec3(vec4.z, vec5.z, 0f, 1f);
			Vec3 vec6 = new Vec3(0f, 0f, 0f, 1f);
			Vec3 vec7 = new Vec3(0f, 0f, 0f, 1f);
			mat = new Mat3(ref vec2, ref vec6, ref vec7);
			Vec3 vec8 = new Vec3(0f, 0f, 0f, 1f);
			matrixFrame = new MatrixFrame(ref mat, ref vec8);
			this._ropeMesh.SetAdditionalBoneFrame(1, ref matrixFrame);
			Vec3 vectorArgument = this._ropeMesh.GetVectorArgument();
			float num = 1f;
			float x = vectorArgument.x;
			vec2 = sourceGlobalPosition;
			vectorArgument.z = num - MathF.Max((x - vec2.Distance(vec3)) / vectorArgument.x, 0f);
			this._ropeMesh.SetVectorArgument(vectorArgument.x, vectorArgument.y, vectorArgument.z, vectorArgument.w);
			return vec3;
		}

		// Token: 0x06000D4E RID: 3406 RVA: 0x00068FA4 File Offset: 0x000671A4
		private Vec3 ComputeFreeFallPointsLinear(in Vec3 sourceGlobalPosition, in Vec3 targetGlobalPosition)
		{
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			Vec3 vec = targetGlobalPosition;
			Vec3 vec2 = targetGlobalPosition - sourceGlobalPosition;
			Vec3 vec3 = vec - vec2.NormalizedCopy() * 0.5f;
			Vec3 vec4 = base.GameEntity.GetGlobalFrame().TransformToLocalNonOrthogonal(ref sourceGlobalPosition);
			Vec3 vec5 = base.GameEntity.GetGlobalFrame().TransformToLocalNonOrthogonal(ref vec3);
			vec2 = new Vec3(2f, 0f, 0f, -1f);
			Mat3 mat = new Mat3(ref vec4, ref vec2, ref vec5);
			MatrixFrame matrixFrame = new MatrixFrame(ref mat, ref vec5);
			this._ropeMesh.SetAdditionalBoneFrame(0, ref matrixFrame);
			vec2 = globalFrame.TransformToLocal(ref vec3);
			BoundingBox boundingBox = new BoundingBox(ref vec2);
			vec2 = globalFrame.TransformToLocal(ref sourceGlobalPosition);
			boundingBox.RelaxMinMaxWithPointAndRadius(ref vec2, 1f);
			vec2 = new Vec3(vec4.z, vec5.z, 0f, 1f);
			Vec3 vec6 = new Vec3(0f, 0f, 0f, 1f);
			Vec3 vec7 = new Vec3(0f, 0f, 0f, 1f);
			mat = new Mat3(ref vec2, ref vec6, ref vec7);
			Vec3 vec8 = new Vec3(0f, 0f, 0f, 1f);
			matrixFrame = new MatrixFrame(ref mat, ref vec8);
			this._ropeMesh.SetAdditionalBoneFrame(1, ref matrixFrame);
			Vec3 vectorArgument = this._ropeMesh.GetVectorArgument();
			float num = 1f;
			float x = vectorArgument.x;
			vec2 = sourceGlobalPosition;
			vectorArgument.z = num - MathF.Max((x - vec2.Distance(vec3)) / vectorArgument.x, 0f);
			this._ropeMesh.SetVectorArgument(vectorArgument.x, vectorArgument.y, vectorArgument.z, vectorArgument.w);
			this.UpdateRopeLocalBoundingBox(in boundingBox);
			return vec3;
		}

		// Token: 0x06000D4F RID: 3407 RVA: 0x0006919C File Offset: 0x0006739C
		private void UpdateRopeLocalBoundingBox(in BoundingBox candidateLocalBoundingBox)
		{
			BoundingBox localBoundingBox = base.GameEntity.GetLocalBoundingBox();
			if (BoundingBox.ArrangeWithAnotherBoundingBox(ref localBoundingBox, candidateLocalBoundingBox, 10f))
			{
				this._localUpdatedBoundingBox = localBoundingBox;
				base.GameEntity.SetBoundingboxDirty();
				MissionShip firstScriptOfType = base.GameEntity.Root.GetFirstScriptOfType<MissionShip>();
				if (firstScriptOfType == null)
				{
					return;
				}
				firstScriptOfType.InvalidateLocalBoundingBoxCache();
			}
		}

		// Token: 0x06000D50 RID: 3408 RVA: 0x00069204 File Offset: 0x00067404
		public void SetRopeBoundingBoxToInitialState()
		{
			base.GameEntity.SetManualLocalBoundingBox(ref this._ropePileBaseBoundingBox);
			WeakGameEntity parent = base.GameEntity.Parent;
			if (parent.IsValid)
			{
				parent.SetBoundingboxDirty();
			}
		}

		// Token: 0x06000D51 RID: 3409 RVA: 0x00069244 File Offset: 0x00067444
		private MatrixFrame ComputeFreeFallPoints(in Vec3 sourceGlobalPosition, in Vec3 targetGlobalPosition, in Vec3 globalVelocity, float time)
		{
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			MatrixFrame identity = MatrixFrame.Identity;
			Vec3 vec = globalVelocity + MBGlobals.GravitationalAcceleration * time;
			time -= 0.5f / vec.Length;
			identity.origin = this.GetPositionAtProjectileCurveProgress(in globalVelocity, in sourceGlobalPosition, time, 63);
			identity.rotation.f = vec.NormalizedCopy();
			Vec3 vec2 = Vec3.CrossProduct(identity.rotation.f, identity.rotation.u);
			identity.rotation.s = vec2.NormalizedCopy();
			identity.rotation.u = Vec3.CrossProduct(identity.rotation.s, identity.rotation.f);
			identity.rotation.RotateAboutSide(-1.5707964f);
			Vec3 vec3 = globalFrame.TransformToLocalNonOrthogonal(ref sourceGlobalPosition);
			Vec3 vec4 = globalFrame.TransformToLocalNonOrthogonal(ref identity.origin);
			vec2 = new Vec3(64f, 0f, 0f, -1f);
			Mat3 mat = new Mat3(ref vec3, ref vec2, ref vec4);
			MatrixFrame matrixFrame = new MatrixFrame(ref mat, ref vec4);
			this._ropeMesh.SetAdditionalBoneFrame(0, ref matrixFrame);
			vec2 = globalFrame.TransformToLocal(ref identity.origin);
			BoundingBox boundingBox = new BoundingBox(ref vec2);
			vec2 = globalFrame.TransformToLocal(ref sourceGlobalPosition);
			boundingBox.RelaxMinMaxWithPointAndRadius(ref vec2, 1f);
			for (int i = 0; i < 72; i += 12)
			{
				Vec3 positionAtProjectileCurveProgress = this.GetPositionAtProjectileCurveProgress(in globalVelocity, in sourceGlobalPosition, time, i);
				Vec3 vec5 = globalFrame.TransformToLocal(ref positionAtProjectileCurveProgress);
				if (i < 64)
				{
					boundingBox.RelaxMinMaxWithPoint(ref vec5);
				}
				Vec3 positionAtProjectileCurveProgress2 = this.GetPositionAtProjectileCurveProgress(in globalVelocity, in sourceGlobalPosition, time, i + 1);
				Vec3 vec6 = globalFrame.TransformToLocal(ref positionAtProjectileCurveProgress2);
				if (i + 1 < 64)
				{
					boundingBox.RelaxMinMaxWithPoint(ref vec6);
				}
				Vec3 positionAtProjectileCurveProgress3 = this.GetPositionAtProjectileCurveProgress(in globalVelocity, in sourceGlobalPosition, time, i + 2);
				Vec3 vec7 = globalFrame.TransformToLocal(ref positionAtProjectileCurveProgress3);
				if (i + 2 < 64)
				{
					vec2 = globalFrame.TransformToLocal(ref vec7);
					boundingBox.RelaxMinMaxWithPoint(ref vec2);
				}
				Vec3 positionAtProjectileCurveProgress4 = this.GetPositionAtProjectileCurveProgress(in globalVelocity, in sourceGlobalPosition, time, i + 3);
				Vec3 vec8 = globalFrame.TransformToLocal(ref positionAtProjectileCurveProgress4);
				if (i + 3 < 64)
				{
					boundingBox.RelaxMinMaxWithPoint(ref vec8);
				}
				Vec3 positionAtProjectileCurveProgress5 = this.GetPositionAtProjectileCurveProgress(in globalVelocity, in sourceGlobalPosition, time, i + 4);
				Vec3 vec9 = globalFrame.TransformToLocal(ref positionAtProjectileCurveProgress5);
				if (i + 4 < 64)
				{
					boundingBox.RelaxMinMaxWithPoint(ref vec9);
				}
				Vec3 positionAtProjectileCurveProgress6 = this.GetPositionAtProjectileCurveProgress(in globalVelocity, in sourceGlobalPosition, time, i + 5);
				Vec3 vec10 = globalFrame.TransformToLocal(ref positionAtProjectileCurveProgress6);
				if (i + 5 < 64)
				{
					boundingBox.RelaxMinMaxWithPoint(ref vec10);
				}
				Vec3 positionAtProjectileCurveProgress7 = this.GetPositionAtProjectileCurveProgress(in globalVelocity, in sourceGlobalPosition, time, i + 6);
				Vec3 vec11 = globalFrame.TransformToLocal(ref positionAtProjectileCurveProgress7);
				if (i + 6 < 64)
				{
					boundingBox.RelaxMinMaxWithPoint(ref vec11);
				}
				Vec3 positionAtProjectileCurveProgress8 = this.GetPositionAtProjectileCurveProgress(in globalVelocity, in sourceGlobalPosition, time, i + 7);
				Vec3 vec12 = globalFrame.TransformToLocal(ref positionAtProjectileCurveProgress8);
				if (i + 7 < 64)
				{
					boundingBox.RelaxMinMaxWithPoint(ref vec12);
				}
				Vec3 positionAtProjectileCurveProgress9 = this.GetPositionAtProjectileCurveProgress(in globalVelocity, in sourceGlobalPosition, time, i + 8);
				Vec3 vec13 = globalFrame.TransformToLocal(ref positionAtProjectileCurveProgress9);
				if (i + 8 < 64)
				{
					boundingBox.RelaxMinMaxWithPoint(ref vec13);
				}
				Vec3 positionAtProjectileCurveProgress10 = this.GetPositionAtProjectileCurveProgress(in globalVelocity, in sourceGlobalPosition, time, i + 9);
				Vec3 vec14 = globalFrame.TransformToLocal(ref positionAtProjectileCurveProgress10);
				if (i + 9 < 64)
				{
					boundingBox.RelaxMinMaxWithPoint(ref vec14);
				}
				Vec3 positionAtProjectileCurveProgress11 = this.GetPositionAtProjectileCurveProgress(in globalVelocity, in sourceGlobalPosition, time, i + 10);
				Vec3 vec15 = globalFrame.TransformToLocal(ref positionAtProjectileCurveProgress11);
				if (i + 10 < 64)
				{
					boundingBox.RelaxMinMaxWithPoint(ref vec15);
				}
				Vec3 positionAtProjectileCurveProgress12 = this.GetPositionAtProjectileCurveProgress(in globalVelocity, in sourceGlobalPosition, time, i + 11);
				Vec3 vec16 = globalFrame.TransformToLocal(ref positionAtProjectileCurveProgress12);
				if (i + 11 < 64)
				{
					boundingBox.RelaxMinMaxWithPoint(ref vec16);
				}
				vec2 = new Vec3(vec5.z, vec6.z, vec7.z, 1f);
				Vec3 vec17 = new Vec3(vec8.z, vec9.z, vec10.z, 1f);
				Vec3 vec18 = new Vec3(vec11.z, vec12.z, vec13.z, 1f);
				mat = new Mat3(ref vec2, ref vec17, ref vec18);
				Vec3 vec19 = new Vec3(vec14.z, vec15.z, vec16.z, 1f);
				MatrixFrame matrixFrame2 = new MatrixFrame(ref mat, ref vec19);
				this._ropeMesh.SetAdditionalBoneFrame(i / 12 + 1, ref matrixFrame2);
			}
			Vec3 vectorArgument = this._ropeMesh.GetVectorArgument();
			float num = 1f;
			float x = vectorArgument.x;
			vec2 = sourceGlobalPosition;
			vectorArgument.z = num - MathF.Max((x - vec2.Distance(identity.origin)) / vectorArgument.x, 0f);
			this._ropeMesh.SetVectorArgument(vectorArgument.x, vectorArgument.y, vectorArgument.z, vectorArgument.w);
			this.UpdateRopeLocalBoundingBox(in boundingBox);
			return identity;
		}

		// Token: 0x04000839 RID: 2105
		public const float HookLength = 0.5f;

		// Token: 0x0400083A RID: 2106
		private const int NumberOfPoints = 64;

		// Token: 0x0400083B RID: 2107
		private const int PaddedNumberOfPoints = 72;

		// Token: 0x0400083C RID: 2108
		private const int NumberOfDataPerFrame = 12;

		// Token: 0x0400083D RID: 2109
		private Mesh _ropeMesh;

		// Token: 0x0400083E RID: 2110
		private BoundingBox _localUpdatedBoundingBox;

		// Token: 0x0400083F RID: 2111
		private BoundingBox _ropePileBaseBoundingBox;
	}
}
