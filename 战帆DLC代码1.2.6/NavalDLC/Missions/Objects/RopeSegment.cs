using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace NavalDLC.Missions.Objects
{
	// Token: 0x0200009F RID: 159
	[ScriptComponentParams("ship_visual_only", "rope_segment")]
	internal class RopeSegment : ScriptComponentBehavior
	{
		// Token: 0x17000235 RID: 565
		// (get) Token: 0x06000C0D RID: 3085 RVA: 0x00057540 File Offset: 0x00055740
		// (set) Token: 0x06000C0E RID: 3086 RVA: 0x00057548 File Offset: 0x00055748
		public float RuntimeLooseMultiplier { get; private set; }

		// Token: 0x17000236 RID: 566
		// (get) Token: 0x06000C0F RID: 3087 RVA: 0x00057551 File Offset: 0x00055751
		// (set) Token: 0x06000C10 RID: 3088 RVA: 0x00057559 File Offset: 0x00055759
		public bool UseDistanceAsRopeLength { get; private set; }

		// Token: 0x17000237 RID: 567
		// (get) Token: 0x06000C11 RID: 3089 RVA: 0x00057562 File Offset: 0x00055762
		// (set) Token: 0x06000C12 RID: 3090 RVA: 0x0005756A File Offset: 0x0005576A
		public float BurnedClipFactor { get; set; }

		// Token: 0x17000238 RID: 568
		// (get) Token: 0x06000C13 RID: 3091 RVA: 0x00057573 File Offset: 0x00055773
		// (set) Token: 0x06000C14 RID: 3092 RVA: 0x0005757B File Offset: 0x0005577B
		public bool BurnedClipReverseMode { get; set; }

		// Token: 0x17000239 RID: 569
		// (get) Token: 0x06000C15 RID: 3093 RVA: 0x00057584 File Offset: 0x00055784
		// (set) Token: 0x06000C16 RID: 3094 RVA: 0x0005758C File Offset: 0x0005578C
		public Mesh RopeMesh
		{
			get
			{
				return this._ropeMesh;
			}
			private set
			{
				this._ropeMesh = value;
			}
		}

		// Token: 0x1700023A RID: 570
		// (get) Token: 0x06000C17 RID: 3095 RVA: 0x00057595 File Offset: 0x00055795
		// (set) Token: 0x06000C18 RID: 3096 RVA: 0x0005759D File Offset: 0x0005579D
		public float CurrentRopeLength { get; private set; }

		// Token: 0x1700023B RID: 571
		// (get) Token: 0x06000C19 RID: 3097 RVA: 0x000575A6 File Offset: 0x000557A6
		// (set) Token: 0x06000C1A RID: 3098 RVA: 0x000575AE File Offset: 0x000557AE
		public bool LinearMode { get; private set; }

		// Token: 0x1700023C RID: 572
		// (get) Token: 0x06000C1B RID: 3099 RVA: 0x000575B7 File Offset: 0x000557B7
		// (set) Token: 0x06000C1C RID: 3100 RVA: 0x000575BF File Offset: 0x000557BF
		public float LooseAmount
		{
			get
			{
				return this._looseAmount;
			}
			private set
			{
				this._looseAmount = value;
			}
		}

		// Token: 0x1700023D RID: 573
		// (get) Token: 0x06000C1D RID: 3101 RVA: 0x000575C8 File Offset: 0x000557C8
		// (set) Token: 0x06000C1E RID: 3102 RVA: 0x000575D0 File Offset: 0x000557D0
		public bool IsFixed
		{
			get
			{
				return this._isFixed;
			}
			private set
			{
				this._isFixed = value;
			}
		}

		// Token: 0x1700023E RID: 574
		// (get) Token: 0x06000C1F RID: 3103 RVA: 0x000575D9 File Offset: 0x000557D9
		// (set) Token: 0x06000C20 RID: 3104 RVA: 0x000575E1 File Offset: 0x000557E1
		public int SegmentIndex
		{
			get
			{
				return this._segmentIndex;
			}
			private set
			{
				this._segmentIndex = value;
			}
		}

		// Token: 0x1700023F RID: 575
		// (get) Token: 0x06000C21 RID: 3105 RVA: 0x000575EA File Offset: 0x000557EA
		// (set) Token: 0x06000C22 RID: 3106 RVA: 0x000575F2 File Offset: 0x000557F2
		public float DefaultRopeLength
		{
			get
			{
				return this._defaultRopeLength;
			}
			private set
			{
				this._defaultRopeLength = value;
			}
		}

		// Token: 0x17000240 RID: 576
		// (get) Token: 0x06000C23 RID: 3107 RVA: 0x000575FB File Offset: 0x000557FB
		// (set) Token: 0x06000C24 RID: 3108 RVA: 0x00057608 File Offset: 0x00055808
		public WeakGameEntity EndEntity
		{
			get
			{
				return this._endEntity.WeakEntity;
			}
			private set
			{
				this._endEntity = GameEntity.CreateFromWeakEntity(value);
				this._externalEndEntitySet = value != null;
			}
		}

		// Token: 0x06000C25 RID: 3109 RVA: 0x00057624 File Offset: 0x00055824
		private RopeSegment()
		{
			this.RuntimeLooseMultiplier = 1f;
			this.CurrentRopeLength = 12.95f;
			this.UseDistanceAsRopeLength = false;
			this.LinearMode = false;
			this.BurnedClipFactor = 0f;
			this.BurnedClipReverseMode = false;
		}

		// Token: 0x06000C26 RID: 3110 RVA: 0x000576F8 File Offset: 0x000558F8
		protected override void OnEditorInit()
		{
			this.FetchEntities();
			if (this._usesPhysicsBody)
			{
				this._physicsEntity = GameEntity.CreateFromWeakEntity(base.GameEntity.Root.GetFirstChildEntityWithTagRecursive("rope_physics_body"));
			}
		}

		// Token: 0x06000C27 RID: 3111 RVA: 0x0005773C File Offset: 0x0005593C
		protected override void OnEditorTick(float dt)
		{
			RopeSegment._physicsCheckPoints[0] = 0.15f;
			RopeSegment._physicsCheckPoints[1] = 0.5f;
			RopeSegment._physicsCheckPoints[2] = 0.85f;
			this.FetchEntities();
			if (base.GameEntity.IsVisibleIncludeParents())
			{
				this.TickAux(dt);
				return;
			}
			this._firstTick = true;
		}

		// Token: 0x06000C28 RID: 3112 RVA: 0x00057794 File Offset: 0x00055994
		protected override void OnInit()
		{
			this.FetchEntities();
			if (this._usesPhysicsBody)
			{
				this._physicsEntity = GameEntity.CreateFromWeakEntity(base.GameEntity.Root.GetFirstChildEntityWithTagRecursive("rope_physics_body"));
			}
		}

		// Token: 0x06000C29 RID: 3113 RVA: 0x000577D8 File Offset: 0x000559D8
		protected override void OnTickParallel3(float dt)
		{
			if (base.GameEntity.IsVisibleIncludeParents())
			{
				this.TickAux(dt);
				return;
			}
			this._firstTick = true;
		}

		// Token: 0x06000C2A RID: 3114 RVA: 0x00057804 File Offset: 0x00055A04
		protected override void OnEditorVariableChanged(string variableName)
		{
			if (variableName == "Default Rope Length")
			{
				this.CurrentRopeLength = this._defaultRopeLength * 0.5f;
			}
		}

		// Token: 0x06000C2B RID: 3115 RVA: 0x00057825 File Offset: 0x00055A25
		protected override void OnRemoved(int removeReason)
		{
			base.OnRemoved(removeReason);
			this._endEntity = null;
			this._physicsEntity = null;
			this._ropeMesh = null;
		}

		// Token: 0x06000C2C RID: 3116 RVA: 0x00057843 File Offset: 0x00055A43
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 64;
		}

		// Token: 0x06000C2D RID: 3117 RVA: 0x00057848 File Offset: 0x00055A48
		private void FetchEntities()
		{
			this._ropeSegmentCosmetics.Clear();
			this._physicsEntity = null;
			foreach (WeakGameEntity weakGameEntity in base.GameEntity.GetChildren())
			{
				RopeSegmentCosmetics firstScriptOfType = weakGameEntity.GetFirstScriptOfType<RopeSegmentCosmetics>();
				if (firstScriptOfType != null)
				{
					this._ropeSegmentCosmetics.Add(firstScriptOfType);
					firstScriptOfType.GameEntity.SetDoNotCheckVisibility(true);
				}
			}
			if (base.GameEntity.Parent != null && !this._externalEndEntitySet)
			{
				this._endEntity = GameEntity.CreateFromWeakEntity(base.GameEntity.Parent.GetFirstChildEntityWithTag("simple_rope_end"));
			}
			this._ropeMesh = base.GameEntity.GetFirstMesh();
			if (this._ropeMesh != null)
			{
				this._ropeMesh.SetupAdditionalBoneBuffer(2);
			}
			base.GameEntity.SetBoundingboxDirty();
			base.GameEntity.SetDoNotCheckVisibility(true);
			if (this._ropeMesh != null)
			{
				Mesh ropeMesh = this._ropeMesh;
				int num = 1;
				MatrixFrame matrixFrame = MatrixFrame.Identity;
				ropeMesh.SetAdditionalBoneFrame(num, ref matrixFrame);
			}
			if (this._ropeMesh != null && this._endEntity != null && this._ropeSegmentCosmetics.Count > 0)
			{
				this._ropeSegmentCosmeticsDxCached.Clear();
				Vec3 vec;
				vec..ctor(0f, 0f, 0f, -1f);
				MatrixFrame matrixFrame = base.GameEntity.GetGlobalFrame();
				Vec3 vec2 = matrixFrame.TransformToLocalNonOrthogonal(ref this._endEntity.GetGlobalFrame().origin);
				Vec3 vectorArgument = this._ropeMesh.GetVectorArgument();
				float num2 = vectorArgument.x * vectorArgument.z;
				this.FillBridgeCurveAccessData(in vec, in vec2, in num2);
				foreach (RopeSegmentCosmetics ropeSegmentCosmetics in this._ropeSegmentCosmetics)
				{
					float num3 = MathF.Clamp(ropeSegmentCosmetics.RopeLocalPosition, 0f, 1f) * num2;
					this._ropeSegmentCosmeticsDxCached.Add(this.GetCurveDxFromDt(vec, num3));
				}
			}
		}

		// Token: 0x06000C2E RID: 3118 RVA: 0x00057A9C File Offset: 0x00055C9C
		private void TickAux(float dt)
		{
			if (this._endEntity == null || this._ropeMesh == null)
			{
				return;
			}
			this._cumulativeTime += dt;
			Vec3 vec;
			vec..ctor(0f, 0f, 0f, -1f);
			Vec3 vec2 = base.GameEntity.GetGlobalFrame().TransformToLocalNonOrthogonal(ref this._endEntity.GetGlobalFrame().origin);
			this.SetRopeShaderParams(vec, vec2);
			this.TickSwingPhysics(dt, vec, vec2);
			this.TickCosmetics(vec, vec2);
		}

		// Token: 0x06000C2F RID: 3119 RVA: 0x00057B34 File Offset: 0x00055D34
		private void SetRopeShaderParams(Vec3 startPosition, Vec3 endPosition)
		{
			MatrixFrame identity = MatrixFrame.Identity;
			identity.rotation.s = startPosition;
			identity.origin = endPosition;
			float num = (endPosition - startPosition).Normalize();
			this._ropeMesh.SetAdditionalBoneFrame(0, ref identity);
			float num2 = 0f;
			if (!this.LinearMode)
			{
				num2 = this._looseAmount;
			}
			float x = base.GameEntity.GetGlobalFrame().rotation.GetScaleVector().x;
			num2 = num2 * this.RuntimeLooseMultiplier * x;
			num2 = MathF.Max(0.005f, num2);
			float w = this._ropeMesh.GetVectorArgument().w;
			if (this._isFixed || this.UseDistanceAsRopeLength)
			{
				this._ropeMesh.SetVectorArgument(num + num2, 25.9f, 1f, w);
				return;
			}
			float num3 = num + num2;
			float num4 = this._defaultRopeLength - num3;
			float num5 = 1f - num4 / this._defaultRopeLength;
			this._ropeMesh.SetVectorArgument(num3, 25.9f, num5, w);
		}

		// Token: 0x06000C30 RID: 3120 RVA: 0x00057C40 File Offset: 0x00055E40
		private float GetCurveDxFromDt(Vec3 startPosition, float currentLength)
		{
			int num = Array.BinarySearch<KeyValuePair<float, Vec3>>(this._bridgeCurveLinearAccessCache, new KeyValuePair<float, Vec3>(currentLength, Vec3.Zero), RopeSegment._cacheCompareDelegate);
			float num2 = 0.14285715f;
			if (num >= 0)
			{
				return (float)num * num2;
			}
			int num3 = ~num;
			int num4 = num3 - 1;
			KeyValuePair<float, Vec3> keyValuePair = this._bridgeCurveLinearAccessCache[num4];
			KeyValuePair<float, Vec3> keyValuePair2 = this._bridgeCurveLinearAccessCache[num3];
			return ((currentLength - keyValuePair.Key) / (keyValuePair2.Key - keyValuePair.Key) + (float)num4) * num2;
		}

		// Token: 0x06000C31 RID: 3121 RVA: 0x00057CBC File Offset: 0x00055EBC
		private Vec3 GetCurvePositionFromLength(Vec3 startPosition, float currentLength)
		{
			int num = Array.BinarySearch<KeyValuePair<float, Vec3>>(this._bridgeCurveLinearAccessCache, new KeyValuePair<float, Vec3>(currentLength, Vec3.Zero), RopeSegment._cacheCompareDelegate);
			if (num >= 0)
			{
				return this._bridgeCurveLinearAccessCache[num].Value;
			}
			int num2 = ~num;
			int num3 = num2 - 1;
			KeyValuePair<float, Vec3> keyValuePair = this._bridgeCurveLinearAccessCache[num3];
			KeyValuePair<float, Vec3> keyValuePair2 = this._bridgeCurveLinearAccessCache[num2];
			float num4 = (currentLength - keyValuePair.Key) / (keyValuePair2.Key - keyValuePair.Key);
			Vec3 vec = Vec3.Lerp(keyValuePair.Value, keyValuePair2.Value, num4);
			if (!this.LinearMode)
			{
				Vec3 vec2 = vec - startPosition;
				vec = this._currentFrameSwingFrame.TransformToLocal(ref vec2) + startPosition;
			}
			return vec;
		}

		// Token: 0x06000C32 RID: 3122 RVA: 0x00057D78 File Offset: 0x00055F78
		private void FillBridgeCurveAccessData(in Vec3 plankTargetOrigin, in Vec3 plankSourceOrigin, in float curvedLength)
		{
			this._bridgeCurveLinearAccessCache[0] = new KeyValuePair<float, Vec3>(0f, plankTargetOrigin);
			Vec3 vec = plankTargetOrigin;
			float num = 0.14285715f;
			float num2 = 0f;
			for (int i = 1; i < 7; i++)
			{
				Vec3 vec2 = RopeSegment.CalculateAutoCurvePosition(plankTargetOrigin, plankSourceOrigin, curvedLength, (float)i * num);
				float num3 = vec2.Distance(vec);
				num2 += num3;
				this._bridgeCurveLinearAccessCache[i] = new KeyValuePair<float, Vec3>(num2, vec2);
				vec = vec2;
			}
			this._bridgeCurveLinearAccessCache[7] = new KeyValuePair<float, Vec3>(curvedLength, plankSourceOrigin);
		}

		// Token: 0x06000C33 RID: 3123 RVA: 0x00057E18 File Offset: 0x00056018
		private void TickCosmetics(Vec3 startPoint, Vec3 endPoint)
		{
			Vec3 vectorArgument = this._ropeMesh.GetVectorArgument();
			float num = vectorArgument.x * vectorArgument.z;
			if (this._ropeSegmentCosmetics.Count > 0 && !this.LinearMode && this._dynamicMode)
			{
				this.FillBridgeCurveAccessData(in startPoint, in endPoint, in num);
			}
			for (int i = 0; i < this._ropeSegmentCosmetics.Count; i++)
			{
				RopeSegmentCosmetics ropeSegmentCosmetics = this._ropeSegmentCosmetics[i];
				WeakGameEntity gameEntity = ropeSegmentCosmetics.GameEntity;
				MatrixFrame globalFrame = gameEntity.GetGlobalFrame();
				Vec3 vec = Vec3.Zero;
				MatrixFrame matrixFrame;
				if (this.LinearMode)
				{
					vec = Vec3.Lerp(startPoint, endPoint, MathF.Clamp(ropeSegmentCosmetics.RopeLocalPosition, 0f, 1f));
					if (ropeSegmentCosmetics.IsBurningNode)
					{
						Vec3 vec2 = endPoint - startPoint;
						matrixFrame = base.GameEntity.GetGlobalFrame();
						vec2 = matrixFrame.rotation.TransformToParent(ref vec2);
						if ((double)vec2.LengthSquared > 0.0001)
						{
							vec2.Normalize();
							globalFrame.rotation.s = vec2;
							globalFrame.rotation.f = -globalFrame.rotation.s.CrossProductWithUp();
							globalFrame.rotation.f.Normalize();
							globalFrame.rotation.u = Vec3.CrossProduct(globalFrame.rotation.s, globalFrame.rotation.f);
						}
					}
				}
				else
				{
					float num2 = MathF.Clamp(ropeSegmentCosmetics.RopeLocalPosition, 0f, 1f) * num;
					if (this._dynamicMode)
					{
						vec = this.GetCurvePositionFromLength(startPoint, num2);
					}
					else
					{
						float num3 = this._ropeSegmentCosmeticsDxCached[i];
						vec = RopeSegment.CalculateAutoCurvePosition(startPoint, endPoint, num, num3);
						Vec3 vec3 = vec - startPoint;
						vec = this._currentFrameSwingFrame.TransformToLocal(ref vec3) + startPoint;
					}
					if (ropeSegmentCosmetics.IsBurningNode)
					{
						Vec3 vec4 = this.GetCurvePositionFromLength(startPoint, MathF.Min(num2 + 0.1f, num)) - vec;
						matrixFrame = base.GameEntity.GetGlobalFrame();
						vec4 = matrixFrame.rotation.TransformToParent(ref vec4);
						if ((double)vec4.LengthSquared > 1E-06)
						{
							vec4.Normalize();
							globalFrame.rotation.s = vec4;
							globalFrame.rotation.f = -globalFrame.rotation.s.CrossProductWithUp();
							globalFrame.rotation.f.Normalize();
							globalFrame.rotation.u = Vec3.CrossProduct(globalFrame.rotation.s, globalFrame.rotation.f);
						}
					}
				}
				matrixFrame = base.GameEntity.GetGlobalFrame();
				globalFrame.origin = matrixFrame.TransformToParent(ref vec);
				gameEntity.SetGlobalFrame(ref globalFrame, true);
			}
		}

		// Token: 0x06000C34 RID: 3124 RVA: 0x000580F8 File Offset: 0x000562F8
		private bool CheckPhysicsEntity(in Vec3 startPosition, in Vec3 endPosition, float currentRotation, float nextRotation, float ropeLength)
		{
			Vec3 vec = endPosition - startPosition;
			vec.Normalize();
			MatrixFrame identity = MatrixFrame.Identity;
			identity.rotation.RotateAboutAnArbitraryVector(ref vec, currentRotation);
			MatrixFrame identity2 = MatrixFrame.Identity;
			identity2.rotation.RotateAboutAnArbitraryVector(ref vec, nextRotation);
			foreach (float num in RopeSegment._physicsCheckPoints)
			{
				Vec3 vec2 = RopeSegment.CalculateAutoCurvePosition(startPosition, endPosition, ropeLength, num);
				Vec3 vec3 = vec2;
				Vec3 vec4 = vec2 - startPosition;
				vec2 = identity.TransformToParent(ref vec4) + startPosition;
				vec4 = vec3 - startPosition;
				vec3 = identity2.TransformToParent(ref vec4) + startPosition;
				Vec3 vec5 = vec3 - vec2;
				float num2 = vec5.Normalize();
				if (num2 >= 0.0001f)
				{
					num2 += 0.02f;
					float num3 = 0f;
					if (this._physicsEntity.RayHitEntity(base.GameEntity.GetGlobalFrame().TransformToParent(ref vec2), vec5, num2, ref num3) && num2 > num3)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06000C35 RID: 3125 RVA: 0x00058238 File Offset: 0x00056438
		private void TickSwingPhysics(float dt, Vec3 startPoint, Vec3 endPoint)
		{
			if (this._ropeMesh == null || this._endEntity == null || (double)this._looseAmount < 1E-07 || dt == 0f)
			{
				this._currentFrameSwingFrame = MatrixFrame.Identity;
				return;
			}
			if (this._tickRemainingForPhysics > 0)
			{
				this._tickRemainingForPhysics--;
				return;
			}
			WeakGameEntity parent = base.GameEntity.Parent;
			if (parent != null && dt > 0f && !this.LinearMode)
			{
				MatrixFrame globalFrame = parent.Root.GetGlobalFrame();
				Vec3 vec = endPoint - startPoint;
				Vec3 vec2 = base.GameEntity.GetLocalFrame().TransformToParent(ref startPoint);
				vec.Normalize();
				if ((double)vec.Length < 1E-09)
				{
					return;
				}
				Vec3 vectorArgument = this._ropeMesh.GetVectorArgument();
				float num = vectorArgument.x * vectorArgument.z;
				bool firstTick = this._firstTick;
				if (this._firstTick)
				{
					Vec3 vec3 = vec2;
					this._previousPosition = vec3;
					this._prevParentFrame = globalFrame;
					this._firstTick = false;
				}
				Vec3 vec4 = vec.CrossProductWithUp();
				vec4.Normalize();
				if (false)
				{
					Vec3 vec5 = this._prevParentFrame.TransformToLocalNonOrthogonal(ref globalFrame).TransformToParent(ref vec2) - vec2;
					if (firstTick)
					{
						this._previousChangeDueToShip = vec5;
					}
					Vec3 vec6 = (vec5 - this._previousChangeDueToShip) * 0.0003f / dt;
					this._previousChangeDueToShip = vec5;
					float num2 = vec6.Normalize();
					num2 = MathF.Clamp(num2, 0f, 1f);
					float num3 = Vec3.DotProduct(-vec6, vec4);
					if (MathF.IsValidValue(num3))
					{
						if (MathF.Abs(num3) > 0f)
						{
							float num4 = (float)MathF.Sign(num3);
							num3 = MathF.Max((MathF.Abs(num3) - 0.6f) * 2.5f, 0f) * num4 * num2;
						}
						this._pendulumVelocity -= num3 * this._swingMultiplier * 0.25f;
					}
				}
				if (this._pendulumCurrentRotation > 0f)
				{
					float num5 = MBMath.SmoothStep(0f, 0.1f, this._pendulumCurrentRotation);
					this._pendulumVelocity -= dt * 2f * num5 * 1.027f * 0.3f;
				}
				else
				{
					float num6 = MBMath.SmoothStep(0f, -0.1f, this._pendulumCurrentRotation);
					this._pendulumVelocity += dt * 2f * num6 * 1.027f * 0.3f;
				}
				float num7 = MathF.Lerp(1f, 0.5f, dt * 4f, 1E-05f);
				this._pendulumVelocity *= num7;
				Vec3 vec7;
				vec7..ctor(MathF.Pow(MathF.Cos(startPoint.x * 0.5f + this._cumulativeTime * 0.45f), 10f), MathF.Pow(MathF.Cos(startPoint.y * 1.2f + this._cumulativeTime * 0.65f), 10f), MathF.Pow(MathF.Cos(startPoint.z * 3.5f + this._cumulativeTime * 0.35f), 10f), -1f);
				vec7.Normalize();
				float num8 = MathF.Clamp(MathF.Cos(startPoint.x * 0.5f + this._cumulativeTime * 2.5f) - 0.95f, 0f, 1f) * 4.5f;
				num8 = MathF.Max(num8, 0f);
				float num9 = MathF.Clamp(MathF.Cos(startPoint.y * 0.9f + this._cumulativeTime * 2.5f) - 0.95f, 0f, 1f) * 4.9f;
				num9 = MathF.Max(num9, 0f);
				float num10 = 1f + MathF.Cos(startPoint.z * 0.3f + this._cumulativeTime * 0.345f);
				float num11 = MathF.Min(base.GameEntity.GetGlobalWindStrengthVectorOfScene().Length, 5f) * MathF.Max(num8, num9) * num10 * dt / MathF.Max(1f, num);
				this._pendulumVelocity += num11 * 6.8f * this._swingMultiplier;
				float num12 = this._pendulumVelocity * dt * 50f;
				if (this._physicsEntity != null && !this.CheckPhysicsEntity(in startPoint, in endPoint, this._pendulumCurrentRotation, this._pendulumCurrentRotation + num12, num))
				{
					this._pendulumVelocity *= -0.95f;
					num12 *= -1.25f;
				}
				float num13 = (float)MathF.Sign(this._pendulumVelocity);
				float num14 = MathF.Abs(this._pendulumVelocity);
				num14 = MathF.Min(num14, 0.06f);
				this._pendulumVelocity = num14 * num13;
				this._pendulumCurrentRotation += num12;
				if (!MathF.IsValidValue(this._pendulumCurrentRotation))
				{
					this._pendulumCurrentRotation = 0f;
				}
				if (!MathF.IsValidValue(this._pendulumVelocity))
				{
					this._pendulumVelocity = 0f;
				}
				for (;;)
				{
					if (this._pendulumCurrentRotation > 3.1415927f)
					{
						this._pendulumCurrentRotation -= 6.2831855f;
					}
					else
					{
						if (this._pendulumCurrentRotation >= -3.1415927f)
						{
							break;
						}
						this._pendulumCurrentRotation += 6.2831855f;
					}
				}
				this._previousVelocity = (startPoint - this._previousPosition) / dt;
				this._previousPosition = startPoint;
				this._prevParentFrame = globalFrame;
				Vec3 vec8 = startPoint - endPoint;
				vec8.Normalize();
				this._currentFrameSwingFrame = MatrixFrame.Identity;
				this._currentFrameSwingFrame.rotation.RotateAboutAnArbitraryVector(ref vec8, this._pendulumCurrentRotation);
			}
			else
			{
				this._currentFrameSwingFrame = MatrixFrame.Identity;
			}
			this._ropeMesh.SetAdditionalBoneFrame(1, ref this._currentFrameSwingFrame);
		}

		// Token: 0x06000C36 RID: 3126 RVA: 0x00058834 File Offset: 0x00056A34
		public void ShiftRope(float meters)
		{
			Vec3 vectorArgument = this._ropeMesh.GetVectorArgument();
			float num = vectorArgument.z * vectorArgument.x;
			if (num > 0f)
			{
				float num2 = meters / num;
				foreach (RopeSegmentCosmetics ropeSegmentCosmetics in this._ropeSegmentCosmetics)
				{
					ropeSegmentCosmetics.RopeLocalPosition += num2;
				}
			}
		}

		// Token: 0x06000C37 RID: 3127 RVA: 0x000588B4 File Offset: 0x00056AB4
		public void ApplyBoundingBox(MatrixFrame parentFrame, ref BoundingBox bb)
		{
			MatrixFrame matrixFrame = base.GameEntity.GetGlobalFrame();
			Vec3 vec = parentFrame.TransformToLocalNonOrthogonal(ref matrixFrame.origin);
			Vec3 vec2 = vec + Vec3.One * 0.25f;
			bb.RelaxMinMaxWithPoint(ref vec2);
			vec2 = vec - Vec3.One * 0.25f;
			bb.RelaxMinMaxWithPoint(ref vec2);
			if (this._endEntity != null)
			{
				matrixFrame = this._endEntity.GetGlobalFrame();
				Vec3 vec3 = parentFrame.TransformToLocalNonOrthogonal(ref matrixFrame.origin);
				vec2 = vec3 + Vec3.One * 0.25f;
				bb.RelaxMinMaxWithPoint(ref vec2);
				vec2 = vec3 - Vec3.One * 0.25f;
				bb.RelaxMinMaxWithPoint(ref vec2);
			}
		}

		// Token: 0x06000C38 RID: 3128 RVA: 0x00058983 File Offset: 0x00056B83
		public void SetUseDistanceAsRopeLength()
		{
			this.UseDistanceAsRopeLength = true;
		}

		// Token: 0x06000C39 RID: 3129 RVA: 0x0005898C File Offset: 0x00056B8C
		public void SetEndEntity(WeakGameEntity entity)
		{
			this._endEntity = GameEntity.CreateFromWeakEntity(entity);
			this._externalEndEntitySet = entity != null;
		}

		// Token: 0x06000C3A RID: 3130 RVA: 0x000589A7 File Offset: 0x00056BA7
		public void SetAsFixedEntity()
		{
			this._isFixed = true;
		}

		// Token: 0x06000C3B RID: 3131 RVA: 0x000589B0 File Offset: 0x00056BB0
		public void AddRope(float value)
		{
			this.CurrentRopeLength += value;
		}

		// Token: 0x06000C3C RID: 3132 RVA: 0x000589C0 File Offset: 0x00056BC0
		public void SetLinearMode(bool value)
		{
			this.LinearMode = value;
		}

		// Token: 0x06000C3D RID: 3133 RVA: 0x000589C9 File Offset: 0x00056BC9
		public void SetRuntimeLooseMultiplier(float value)
		{
			this.RuntimeLooseMultiplier = value;
		}

		// Token: 0x06000C3E RID: 3134 RVA: 0x000589D4 File Offset: 0x00056BD4
		public void FillBurningRecordForSegment(BurningSystem system, string prefabName, float nodeLength, bool reversePlacement)
		{
			float num = base.GameEntity.GetGlobalFrame().origin.Distance(this._endEntity.GetGlobalFrame().origin);
			int num2 = (int)(num / nodeLength);
			float num3 = nodeLength / (num * 2f);
			for (int i = 0; i < num2; i++)
			{
				GameEntity gameEntity = GameEntity.Instantiate(base.GameEntity.Scene, prefabName, true, true, "");
				if (!(gameEntity == null))
				{
					base.GameEntity.AddChild(gameEntity.WeakEntity, false);
					BurningNode firstScriptOfType = gameEntity.GetFirstScriptOfType<BurningNode>();
					if (firstScriptOfType != null)
					{
						system.AddNewNode(firstScriptOfType);
					}
					if (MBRandom.RandomFloat > 0.82f)
					{
						firstScriptOfType.EnableSparks();
					}
					gameEntity.CreateAndAddScriptComponent("rope_segment_cosmetics", true);
					RopeSegmentCosmetics firstScriptOfType2 = gameEntity.GetFirstScriptOfType<RopeSegmentCosmetics>();
					firstScriptOfType2.RopeLocalPosition = num3 + (float)i * nodeLength / num;
					this._ropeSegmentCosmetics.Add(firstScriptOfType2);
					if (reversePlacement)
					{
						firstScriptOfType2.RopeLocalPosition = 1f - firstScriptOfType2.RopeLocalPosition;
					}
				}
			}
			this._dynamicMode = true;
		}

		// Token: 0x06000C3F RID: 3135 RVA: 0x00058AED File Offset: 0x00056CED
		public bool DeregisterRopeSegmentCosmetics(RopeSegmentCosmetics cosmetics)
		{
			if (this._ropeSegmentCosmetics.IndexOf(cosmetics) != -1)
			{
				this._ropeSegmentCosmetics.Remove(cosmetics);
				return true;
			}
			return false;
		}

		// Token: 0x06000C40 RID: 3136 RVA: 0x00058B0E File Offset: 0x00056D0E
		public void SetAsDynamic()
		{
			this._dynamicMode = true;
		}

		// Token: 0x06000C41 RID: 3137 RVA: 0x00058B18 File Offset: 0x00056D18
		public void SetAlpha(float value)
		{
			if (this._ropeMesh != null)
			{
				if (value <= 0f)
				{
					base.GameEntity.SetVisibilityExcludeParents(false);
					return;
				}
				base.GameEntity.SetVisibilityExcludeParents(true);
				this._ropeMesh.SetColorAlpha((uint)(MathF.Clamp(value, 0f, 1f) * 255f));
			}
		}

		// Token: 0x06000C42 RID: 3138 RVA: 0x00058B7C File Offset: 0x00056D7C
		public static Vec3 CalculateAutoCurvePosition(Vec3 startPos, Vec3 endPos, float ropeLength, float dx)
		{
			Vec2 vec = startPos.AsVec2 - endPos.AsVec2;
			float num = MathF.Clamp((vec.Length - 0.4f) / 0.2f, 0f, 1f);
			Vec3 vec2 = Vec3.Lerp(startPos, endPos, dx);
			if (num < 1E-06f)
			{
				return vec2;
			}
			if (startPos.z > endPos.z)
			{
				Vec3 vec3 = startPos;
				startPos = endPos;
				endPos = vec3;
				dx = 1f - dx;
				vec *= -1f;
			}
			ropeLength = MathF.Max(ropeLength, vec.Length);
			float length = vec.Length;
			float num2 = (startPos.z - endPos.z) / length;
			ropeLength /= length;
			float num3 = MathF.Sqrt(ropeLength * ropeLength - num2 * num2);
			float num4 = 1f;
			for (int i = 0; i < 10; i++)
			{
				float num5 = num4;
				float num6 = (float)Math.Sinh((double)num5);
				float num7 = (float)Math.Cosh((double)num5);
				float num8 = num5 - (num3 - num6 / num5) / (num6 / (num5 * num5) - num7 / num5);
				if (!MathF.IsValidValue(num8))
				{
					break;
				}
				num4 = num8;
			}
			float num9 = 1f / (2f * num4);
			float num10 = (1f - MathF.Log((ropeLength - num2) / (ropeLength + num2)) * num9) * 0.5f;
			float num11 = -Math.Abs(num2) * 0.5f - ropeLength * 0.5f * (1f / (float)Math.Tanh((double)num4));
			float num12 = num9 * (float)Math.Cosh((double)((dx - num10) / num9)) + num11;
			Vec3 vec4 = Vec3.Lerp(startPos, endPos, dx);
			vec4.z = endPos.z + num12 * length;
			if (!vec4.IsValid)
			{
				return vec2;
			}
			return Vec3.Lerp(vec2, vec4, num);
		}

		// Token: 0x04000704 RID: 1796
		private const int BridgeCurveLinearSampleCount = 8;

		// Token: 0x04000705 RID: 1797
		private const string PhysicsEntityTag = "rope_physics_body";

		// Token: 0x04000706 RID: 1798
		private static readonly Comparer<KeyValuePair<float, Vec3>> _cacheCompareDelegate = Comparer<KeyValuePair<float, Vec3>>.Create((KeyValuePair<float, Vec3> x, KeyValuePair<float, Vec3> y) => x.Key.CompareTo(y.Key));

		// Token: 0x04000707 RID: 1799
		private static float[] _physicsCheckPoints = new float[] { 0.05f, 0.5f, 0.93f };

		// Token: 0x04000708 RID: 1800
		[EditableScriptComponentVariable(true, "Segment Index")]
		private int _segmentIndex;

		// Token: 0x04000709 RID: 1801
		[EditableScriptComponentVariable(true, "Is Fixed")]
		private bool _isFixed;

		// Token: 0x0400070A RID: 1802
		[EditableScriptComponentVariable(true, "Loose Amount")]
		private float _looseAmount = 0.1f;

		// Token: 0x0400070B RID: 1803
		[EditableScriptComponentVariable(true, "Default Rope Length")]
		private float _defaultRopeLength = 25.9f;

		// Token: 0x0400070C RID: 1804
		[EditableScriptComponentVariable(true, "Uses Physics Body")]
		private bool _usesPhysicsBody;

		// Token: 0x0400070D RID: 1805
		[EditableScriptComponentVariable(true, "Swing Multiplier")]
		private float _swingMultiplier = 1f;

		// Token: 0x0400070E RID: 1806
		private KeyValuePair<float, Vec3>[] _bridgeCurveLinearAccessCache = new KeyValuePair<float, Vec3>[8];

		// Token: 0x0400070F RID: 1807
		private bool _firstTick = true;

		// Token: 0x04000710 RID: 1808
		private Vec3 _previousPosition = Vec3.Zero;

		// Token: 0x04000711 RID: 1809
		private Vec3 _previousVelocity = Vec3.Zero;

		// Token: 0x04000712 RID: 1810
		private MatrixFrame _prevParentFrame = MatrixFrame.Identity;

		// Token: 0x04000713 RID: 1811
		private float _pendulumVelocity;

		// Token: 0x04000714 RID: 1812
		private float _pendulumCurrentRotation;

		// Token: 0x04000715 RID: 1813
		private int _tickRemainingForPhysics = 30;

		// Token: 0x04000716 RID: 1814
		private GameEntity _endEntity;

		// Token: 0x04000717 RID: 1815
		private GameEntity _physicsEntity;

		// Token: 0x04000718 RID: 1816
		private Mesh _ropeMesh;

		// Token: 0x04000719 RID: 1817
		private bool _externalEndEntitySet;

		// Token: 0x0400071A RID: 1818
		private float _cumulativeTime;

		// Token: 0x0400071B RID: 1819
		private MatrixFrame _currentFrameSwingFrame = MatrixFrame.Identity;

		// Token: 0x0400071C RID: 1820
		private Vec3 _previousChangeDueToShip = Vec3.Zero;

		// Token: 0x0400071D RID: 1821
		private List<RopeSegmentCosmetics> _ropeSegmentCosmetics = new List<RopeSegmentCosmetics>();

		// Token: 0x0400071E RID: 1822
		private bool _dynamicMode;

		// Token: 0x0400071F RID: 1823
		private List<float> _ropeSegmentCosmeticsDxCached = new List<float>();
	}
}
