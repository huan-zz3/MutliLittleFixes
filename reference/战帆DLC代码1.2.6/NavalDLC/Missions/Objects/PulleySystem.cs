using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace NavalDLC.Missions.Objects
{
	// Token: 0x0200009E RID: 158
	[ScriptComponentParams("ship_visual_only", "pulley_system")]
	internal class PulleySystem : ScriptComponentBehavior
	{
		// Token: 0x17000233 RID: 563
		// (get) Token: 0x06000BF4 RID: 3060 RVA: 0x00055F60 File Offset: 0x00054160
		public WeakGameEntity FirstFixedEntity
		{
			get
			{
				if (this._fixedSegments.Count > 0)
				{
					return this._fixedSegments[0].RopeEntity.WeakEntity;
				}
				return WeakGameEntity.Invalid;
			}
		}

		// Token: 0x17000234 RID: 564
		// (get) Token: 0x06000BF5 RID: 3061 RVA: 0x00055F8C File Offset: 0x0005418C
		public List<RopeSegment> TiedToYardSegments
		{
			get
			{
				return this._tiedToYardSegments;
			}
		}

		// Token: 0x06000BF6 RID: 3062 RVA: 0x00055F94 File Offset: 0x00054194
		private PulleySystem()
		{
		}

		// Token: 0x06000BF7 RID: 3063 RVA: 0x00055FE5 File Offset: 0x000541E5
		protected override void OnEditorInit()
		{
			this.FetchEntities();
		}

		// Token: 0x06000BF8 RID: 3064 RVA: 0x00055FF0 File Offset: 0x000541F0
		protected override void OnEditorTick(float dt)
		{
			if (base.GameEntity.IsVisibleIncludeParents())
			{
				this.FetchEntities();
				this.TickAux();
			}
		}

		// Token: 0x06000BF9 RID: 3065 RVA: 0x00056019 File Offset: 0x00054219
		protected override void OnInit()
		{
			this.FetchEntities();
		}

		// Token: 0x06000BFA RID: 3066 RVA: 0x00056024 File Offset: 0x00054224
		protected override void OnTickParallel2(float dt)
		{
			if (base.GameEntity.IsVisibleIncludeParents())
			{
				this.TickAux();
			}
		}

		// Token: 0x06000BFB RID: 3067 RVA: 0x00056048 File Offset: 0x00054248
		protected override void OnRemoved(int removeReason)
		{
			base.OnRemoved(removeReason);
			this._pulleyEntity = null;
			this._pulleyWheelEntity = null;
			this._pulleyLeftRopeConnectionEntity = null;
			this._pulleyRightRopeConnectionEntity = null;
			this._tiedToYardSegments.Clear();
			this._freeSegments.Clear();
			this._fixedSegments.Clear();
			this._endPointRope.RopeEntity = null;
			this._endPointRope.RopeSegment = null;
			this._endTargetEntity = null;
		}

		// Token: 0x06000BFC RID: 3068 RVA: 0x000560B8 File Offset: 0x000542B8
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 8;
		}

		// Token: 0x06000BFD RID: 3069 RVA: 0x000560BC File Offset: 0x000542BC
		private void FetchEntities()
		{
			this._tiedToYardSegments.Clear();
			this.FetchRopeSegmentsForSide(base.GameEntity, true, ref this._fixedSegments);
			this.FetchRopeSegmentsForSide(base.GameEntity, false, ref this._freeSegments);
			foreach (PulleySystem.SegmentData segmentData in this._freeSegments)
			{
				segmentData.RopeSegment.SetUseDistanceAsRopeLength();
				segmentData.RopeSegment.SetAsDynamic();
			}
			foreach (PulleySystem.SegmentData segmentData2 in this._fixedSegments)
			{
				segmentData2.RopeSegment.SetAsDynamic();
			}
			List<WeakGameEntity> list = new List<WeakGameEntity>();
			base.GameEntity.GetChildrenRecursive(ref list);
			foreach (WeakGameEntity weakGameEntity in list)
			{
				if (weakGameEntity.HasTag("pulley"))
				{
					this._pulleyEntity = GameEntity.CreateFromWeakEntity(weakGameEntity);
				}
				else if (weakGameEntity.HasTag("end_point_rope"))
				{
					this._endPointRope.RopeEntity = GameEntity.CreateFromWeakEntity(weakGameEntity);
					this._endPointRope.RopeSegment = this._endPointRope.RopeEntity.GetFirstScriptOfType<RopeSegment>();
				}
				else if (weakGameEntity.HasTag("end_point_target"))
				{
					this._endTargetEntity = GameEntity.CreateFromWeakEntity(weakGameEntity);
				}
			}
			if (this._pulleyEntity != null)
			{
				this._pulleyRightRopeConnectionEntity = this._pulleyEntity.GetFirstChildEntityWithTag("pulley_right_point");
				this._pulleyLeftRopeConnectionEntity = this._pulleyEntity.GetFirstChildEntityWithTag("pulley_left_point");
				this._pulleyWheelEntity = this._pulleyEntity.GetFirstChildEntityWithTag("pulley_wheel");
				Mesh firstMesh = this._pulleyEntity.GetFirstMesh();
				if (firstMesh != null)
				{
					Vec3 vec = firstMesh.GetBoundingBoxMax() - firstMesh.GetBoundingBoxMin();
					this._endRopeConnectionOffset = vec.z;
				}
			}
			if (this._freeSegments.Count > 0)
			{
				int count = this._freeSegments.Count;
				for (int i = 0; i < count - 1; i++)
				{
					this._freeSegments[i].RopeSegment.SetEndEntity(this._freeSegments[i + 1].RopeEntity.WeakEntity);
				}
				this._freeSegments[count - 1].RopeSegment.SetEndEntity(this._pulleyLeftRopeConnectionEntity.WeakEntity);
			}
			if (this._fixedSegments.Count > 0)
			{
				int count2 = this._fixedSegments.Count;
				for (int j = 0; j < count2 - 1; j++)
				{
					this._fixedSegments[j].RopeSegment.SetEndEntity(this._fixedSegments[j + 1].RopeEntity.WeakEntity);
				}
				this._fixedSegments[count2 - 1].RopeSegment.SetEndEntity(this._pulleyRightRopeConnectionEntity.WeakEntity);
			}
			if (this._endPointRope.RopeSegment != null)
			{
				this._endPointRope.RopeSegment.SetEndEntity(this._endTargetEntity.WeakEntity);
				this._endPointRope.RopeSegment.SetAsFixedEntity();
				this._endPointRope.RopeSegment.SetAsDynamic();
			}
			base.GameEntity.SetDoNotCheckVisibility(true);
		}

		// Token: 0x06000BFE RID: 3070 RVA: 0x0005643C File Offset: 0x0005463C
		private void TickAux()
		{
			if (this._pulleyEntity == null || this._freeSegments.Count == 0 || this._fixedSegments.Count == 0 || this._pulleyLeftRopeConnectionEntity == null || this._pulleyRightRopeConnectionEntity == null || this._endTargetEntity == null)
			{
				return;
			}
			Vec3 origin = this._endTargetEntity.GetGlobalFrame().origin;
			MatrixFrame globalFrame = base.GameEntity.Root.GetGlobalFrame();
			Vec3 zero = Vec3.Zero;
			Vec3 vec = (this._freeSegments[this._freeSegments.Count - 1].RopeEntity.GetGlobalFrame().origin + this._fixedSegments[this._fixedSegments.Count - 1].RopeEntity.GetGlobalFrame().origin) * 0.5f - origin;
			vec.Normalize();
			MatrixFrame globalFrame2 = this._pulleyEntity.GetGlobalFrame();
			float x = globalFrame2.rotation.GetScaleVector().x;
			float num = this._endRopeLength * x;
			globalFrame2.origin = origin + vec * num;
			this._pulleyEntity.SetGlobalFrame(ref globalFrame2, true);
			MatrixFrame identity = MatrixFrame.Identity;
			identity.rotation = globalFrame.rotation;
			Vec3 vec2 = identity.TransformToLocalNonOrthogonal(ref vec);
			Vec3 vec3 = globalFrame.TransformToLocalNonOrthogonal(ref origin);
			if (this._firstTick)
			{
				this._targetPositionLocalPrevFrame = vec3;
				this._firstTick = false;
			}
			Vec3 vec4 = vec3 - this._targetPositionLocalPrevFrame;
			float num2 = vec4.Length;
			if (Vec3.DotProduct(vec2, vec4) < 0f)
			{
				num2 *= -1f;
			}
			float num3 = 0f;
			float num4 = 0f;
			for (int i = 0; i < this._freeSegments.Count - 1; i++)
			{
				WeakGameEntity weakEntity = this._freeSegments[i + 1].RopeEntity.WeakEntity;
				this.SetRopeParamsForSegment(weakEntity, this._freeSegments[i], true, num2 * 2f, true, false);
			}
			num3 += this.SetRopeParamsForSegment(this._pulleyLeftRopeConnectionEntity.WeakEntity, this._freeSegments[this._freeSegments.Count - 1], true, num2 * 2f, true, false);
			for (int j = 0; j < this._fixedSegments.Count - 1; j++)
			{
				WeakGameEntity weakEntity2 = this._fixedSegments[j + 1].RopeEntity.WeakEntity;
				this.SetRopeParamsForSegment(weakEntity2, this._fixedSegments[j], true, num2 * 2f, false, false);
			}
			num4 += this.SetRopeParamsForSegment(this._pulleyLeftRopeConnectionEntity.WeakEntity, this._fixedSegments[this._fixedSegments.Count - 1], true, num2 * 2f, false, false);
			this.ComputePulleyFrame(0f, (num3 + num4) * 0.5f);
			int num5 = 5;
			for (int k = 0; k < num5; k++)
			{
				this.SetRopeParamsForSegment(this._pulleyLeftRopeConnectionEntity.WeakEntity, this._freeSegments[this._freeSegments.Count - 1], true, 0f, false, false);
				this.SetRopeParamsForSegment(this._pulleyRightRopeConnectionEntity.WeakEntity, this._fixedSegments[this._fixedSegments.Count - 1], true, 0f, false, false);
			}
			if (this._endPointRope.RopeEntity != null)
			{
				MatrixFrame globalFrame3 = this._pulleyEntity.GetGlobalFrame();
				Vec3 vec5 = globalFrame3.origin;
				vec5 += (this._endRopeConnectionOffset - 0.165f) * globalFrame3.rotation.u;
				MatrixFrame globalFrame4 = this._endPointRope.RopeEntity.GetGlobalFrame();
				globalFrame4.origin = vec5;
				this._endPointRope.RopeEntity.SetGlobalFrame(ref globalFrame4, true);
				this.SetRopeParamsForSegment(this._endTargetEntity.WeakEntity, this._endPointRope, true, 0f, false, true);
			}
			this._targetPositionLocalPrevFrame = vec3;
		}

		// Token: 0x06000BFF RID: 3071 RVA: 0x00056858 File Offset: 0x00054A58
		private void ComputePulleyFrame(float move_amount, float total_rope_length)
		{
			Vec3 vec = Vec3.Zero;
			float num = 0f;
			int num2 = 0;
			float num3 = 0f;
			RopeSegment ropeSegment = this._endPointRope.RopeSegment;
			Vec3 origin = this._freeSegments[this._freeSegments.Count - 1].RopeEntity.WeakEntity.GetGlobalFrame().origin;
			Vec3 origin2 = this._freeSegments[this._freeSegments.Count - 1].RopeEntity.WeakEntity.GetGlobalFrame().origin;
			vec += origin;
			RopeSegment ropeSegment2 = this._freeSegments[this._freeSegments.Count - 1].RopeSegment;
			if (ropeSegment2 != null)
			{
				num += MathF.Max(0.0005f, ropeSegment2.LooseAmount * this._looseAmountMultiplier);
				num2++;
			}
			vec += origin2;
			RopeSegment ropeSegment3 = this._fixedSegments[this._fixedSegments.Count - 1].RopeSegment;
			if (ropeSegment3 != null)
			{
				num += MathF.Max(0.0005f, ropeSegment3.LooseAmount * this._looseAmountMultiplier);
				num2++;
			}
			if (ropeSegment != null)
			{
				num3 = MathF.Max(0.0005f, ropeSegment.LooseAmount * this._looseAmountMultiplier);
			}
			vec *= 0.5f;
			if (num2 > 0)
			{
				num /= (float)num2;
			}
			Vec3 origin3 = this._endTargetEntity.GetGlobalFrame().origin;
			float num4 = vec.Distance(origin3);
			num4 += num + num3;
			float x = this._pulleyEntity.GetGlobalFrame().rotation.GetScaleVector().x;
			float num5 = this._endRopeLength * x;
			float num6 = 1f - num5 / num4;
			num6 = MathF.Clamp(num6, 0f, 1f);
			Vec3 vec2 = RopeSegment.CalculateAutoCurvePosition(vec, origin3, num4, num6);
			float num7 = MathF.Min(num6 + 0.01f, 1f);
			Vec3 vec3 = RopeSegment.CalculateAutoCurvePosition(vec, origin3, num4, num7) - vec2;
			if (vec3.LengthSquared > 0f)
			{
				vec3.Normalize();
			}
			vec3 = vec3 * 0.5f + (origin3 - vec2) * 0.5f;
			vec3.Normalize();
			WeakGameEntity weakEntity = this._fixedSegments[this._fixedSegments.Count - 1].RopeEntity.WeakEntity;
			WeakGameEntity weakEntity2 = this._freeSegments[this._freeSegments.Count - 1].RopeEntity.WeakEntity;
			Vec3 vec4 = weakEntity.GetGlobalFrame().origin - weakEntity2.GetGlobalFrame().origin;
			if (vec4.Length < 1E-06f)
			{
				return;
			}
			vec4.Normalize();
			MatrixFrame frame = this._pulleyEntity.GetFrame();
			frame.rotation.u = vec3;
			frame.rotation.s = vec4;
			frame.rotation.f = Vec3.CrossProduct(frame.rotation.s, frame.rotation.u);
			frame.rotation.f.Normalize();
			frame.rotation.s = Vec3.CrossProduct(frame.rotation.f, frame.rotation.u);
			frame.rotation.s.Normalize();
			WeakGameEntity parent = this._pulleyEntity.WeakEntity.Parent;
			if (parent != null)
			{
				frame.rotation = parent.GetGlobalFrame().TransformToLocalNonOrthogonal(ref frame).rotation;
			}
			frame.rotation.Orthonormalize();
			this._pulleyEntity.SetFrame(ref frame, true);
			MatrixFrame globalFrame = this._pulleyEntity.GetGlobalFrame();
			globalFrame.origin = vec2;
			this._pulleyEntity.SetGlobalFrame(ref globalFrame, true);
		}

		// Token: 0x06000C00 RID: 3072 RVA: 0x00056C38 File Offset: 0x00054E38
		private float SetRopeParamsForSegment(WeakGameEntity pulleyRopeConnectPoint, PulleySystem.SegmentData segmentData, bool isFixed, float pull_amount, bool moveUV, bool is_end_rope)
		{
			pulleyRopeConnectPoint.GetGlobalFrame();
			segmentData.RopeEntity.GetGlobalFrame();
			if (moveUV)
			{
				Vec3 vectorArgument = segmentData.RopeSegment.RopeMesh.GetVectorArgument2();
				vectorArgument.w += pull_amount * 25.9f;
				segmentData.RopeSegment.RopeMesh.SetVectorArgument2(vectorArgument.x, vectorArgument.y, vectorArgument.z, vectorArgument.w);
			}
			if (!isFixed || moveUV)
			{
				segmentData.RopeSegment.ShiftRope(-pull_amount);
			}
			return 25.9f;
		}

		// Token: 0x06000C01 RID: 3073 RVA: 0x00056CC8 File Offset: 0x00054EC8
		public void SetEndTargetPosition(Vec3 position)
		{
			if (this._endTargetEntity != null)
			{
				MatrixFrame globalFrame = this._endTargetEntity.GetGlobalFrame();
				globalFrame.origin = position;
				this._endTargetEntity.SetGlobalFrame(ref globalFrame, true);
			}
		}

		// Token: 0x06000C02 RID: 3074 RVA: 0x00056D08 File Offset: 0x00054F08
		public void SetLinearMode(bool value)
		{
			foreach (PulleySystem.SegmentData segmentData in this._freeSegments)
			{
				segmentData.RopeSegment.SetLinearMode(value);
			}
			foreach (PulleySystem.SegmentData segmentData2 in this._fixedSegments)
			{
				segmentData2.RopeSegment.SetLinearMode(value);
			}
			if (this._endPointRope.RopeSegment != null)
			{
				this._endPointRope.RopeSegment.SetLinearMode(value);
			}
		}

		// Token: 0x06000C03 RID: 3075 RVA: 0x00056DC4 File Offset: 0x00054FC4
		public bool DeregisterRopeSegmentCosmetics(RopeSegmentCosmetics cosmetics)
		{
			bool flag = false;
			using (List<PulleySystem.SegmentData>.Enumerator enumerator = this._fixedSegments.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.RopeSegment.DeregisterRopeSegmentCosmetics(cosmetics))
					{
						flag = true;
					}
				}
			}
			using (List<PulleySystem.SegmentData>.Enumerator enumerator = this._freeSegments.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.RopeSegment.DeregisterRopeSegmentCosmetics(cosmetics))
					{
						flag = true;
					}
				}
			}
			if (this._endPointRope.RopeSegment != null && this._endPointRope.RopeSegment.DeregisterRopeSegmentCosmetics(cosmetics))
			{
				flag = true;
			}
			return flag;
		}

		// Token: 0x06000C04 RID: 3076 RVA: 0x00056E8C File Offset: 0x0005508C
		private void FetchRopeSegmentsForSide(WeakGameEntity parentEntity, bool isFixed, ref List<PulleySystem.SegmentData> output)
		{
			output.Clear();
			foreach (WeakGameEntity weakGameEntity in base.GameEntity.GetChildren())
			{
				RopeSegment firstScriptOfType = weakGameEntity.GetFirstScriptOfType<RopeSegment>();
				if (firstScriptOfType != null && firstScriptOfType.IsFixed == isFixed && !weakGameEntity.HasTag("end_point_rope"))
				{
					PulleySystem.SegmentData segmentData = default(PulleySystem.SegmentData);
					segmentData.RopeSegment = firstScriptOfType;
					segmentData.RopeEntity = GameEntity.CreateFromWeakEntity(weakGameEntity);
					output.Add(segmentData);
					if (weakGameEntity.HasTag("attached_to_yard"))
					{
						this._tiedToYardSegments.Add(firstScriptOfType);
					}
				}
			}
			output.Sort((PulleySystem.SegmentData a, PulleySystem.SegmentData b) => a.RopeSegment.SegmentIndex.CompareTo(b.RopeSegment.SegmentIndex));
		}

		// Token: 0x06000C05 RID: 3077 RVA: 0x00056F6C File Offset: 0x0005516C
		public void SetRuntimeLooseMultiplier(float value)
		{
			this._looseAmountMultiplier = value;
			foreach (PulleySystem.SegmentData segmentData in this._freeSegments)
			{
				segmentData.RopeSegment.SetRuntimeLooseMultiplier(value);
			}
			foreach (PulleySystem.SegmentData segmentData2 in this._fixedSegments)
			{
				segmentData2.RopeSegment.SetRuntimeLooseMultiplier(value);
			}
			this._endPointRope.RopeSegment.SetRuntimeLooseMultiplier(value * 0.25f);
		}

		// Token: 0x06000C06 RID: 3078 RVA: 0x00057028 File Offset: 0x00055228
		public void ApplyBoundingBox(MatrixFrame parentFrame, ref BoundingBox bb)
		{
			foreach (PulleySystem.SegmentData segmentData in this._freeSegments)
			{
				MatrixFrame matrixFrame = segmentData.RopeEntity.GetGlobalFrame();
				Vec3 vec = parentFrame.TransformToLocalNonOrthogonal(ref matrixFrame.origin);
				Vec3 vec2 = vec + Vec3.One * 0.25f;
				bb.RelaxMinMaxWithPoint(ref vec2);
				vec2 = vec - Vec3.One * 0.25f;
				bb.RelaxMinMaxWithPoint(ref vec2);
			}
			foreach (PulleySystem.SegmentData segmentData2 in this._fixedSegments)
			{
				MatrixFrame matrixFrame = segmentData2.RopeEntity.GetGlobalFrame();
				Vec3 vec3 = parentFrame.TransformToLocalNonOrthogonal(ref matrixFrame.origin);
				Vec3 vec2 = vec3 + Vec3.One * 0.25f;
				bb.RelaxMinMaxWithPoint(ref vec2);
				vec2 = vec3 - Vec3.One * 0.25f;
				bb.RelaxMinMaxWithPoint(ref vec2);
			}
			if (this._endTargetEntity != null)
			{
				MatrixFrame matrixFrame = this._endTargetEntity.GetGlobalFrame();
				Vec3 vec4 = parentFrame.TransformToLocalNonOrthogonal(ref matrixFrame.origin);
				Vec3 vec2 = vec4 + Vec3.One * 0.25f;
				bb.RelaxMinMaxWithPoint(ref vec2);
				vec2 = vec4 - Vec3.One * 0.25f;
				bb.RelaxMinMaxWithPoint(ref vec2);
			}
		}

		// Token: 0x06000C07 RID: 3079 RVA: 0x000571D0 File Offset: 0x000553D0
		public Vec3 GetTiePointCenter()
		{
			if (this._freeSegments.Count == 0 || this._fixedSegments.Count == 0)
			{
				return Vec3.Zero;
			}
			return (this._freeSegments[this._freeSegments.Count - 1].RopeEntity.GetGlobalFrame().origin + this._fixedSegments[this._fixedSegments.Count - 1].RopeEntity.GetGlobalFrame().origin) * 0.5f;
		}

		// Token: 0x06000C08 RID: 3080 RVA: 0x0005725C File Offset: 0x0005545C
		public void SetFirstFreeGlobalPosition(Vec3 position)
		{
			if (this._freeSegments.Count > 0)
			{
				MatrixFrame globalFrame = this._freeSegments[0].RopeEntity.GetGlobalFrame();
				globalFrame.origin = position;
				this._freeSegments[0].RopeEntity.SetGlobalFrame(ref globalFrame, true);
			}
		}

		// Token: 0x06000C09 RID: 3081 RVA: 0x000572B0 File Offset: 0x000554B0
		public void SetFirstFixedGlobalPosition(Vec3 position)
		{
			if (this._fixedSegments.Count > 0)
			{
				MatrixFrame globalFrame = this._fixedSegments[0].RopeEntity.GetGlobalFrame();
				globalFrame.origin = position;
				this._fixedSegments[0].RopeEntity.SetGlobalFrame(ref globalFrame, true);
			}
		}

		// Token: 0x06000C0A RID: 3082 RVA: 0x00057304 File Offset: 0x00055504
		public void FillBurningRecord(BurningSystem system)
		{
			float num = 2f;
			string text = "burning_node_rope";
			if (this._endPointRope.RopeSegment != null)
			{
				this._endPointRope.RopeSegment.FillBurningRecordForSegment(system, text, num, true);
				foreach (PulleySystem.SegmentData segmentData in ((MBRandom.RandomFloat > 0.5f) ? this._freeSegments : this._fixedSegments))
				{
					segmentData.RopeSegment.FillBurningRecordForSegment(system, text, num, true);
				}
			}
		}

		// Token: 0x06000C0B RID: 3083 RVA: 0x000573A0 File Offset: 0x000555A0
		public void SetAlpha(float value)
		{
			if (value <= 0f)
			{
				base.GameEntity.SetVisibilityExcludeParents(false);
				return;
			}
			base.GameEntity.SetVisibilityExcludeParents(true);
			base.GameEntity.SetAlpha(MathF.Clamp(value, 0f, 1f));
		}

		// Token: 0x06000C0C RID: 3084 RVA: 0x000573F4 File Offset: 0x000555F4
		public void GetAllRopeSegments(ref List<RopeSegment> segments, float maximumRopeThickness)
		{
			foreach (PulleySystem.SegmentData segmentData in this._freeSegments)
			{
				if (segmentData.RopeSegment.RopeMesh != null && segmentData.RopeSegment.RopeMesh.GetVectorArgument().w < maximumRopeThickness)
				{
					segments.Add(segmentData.RopeSegment);
				}
			}
			foreach (PulleySystem.SegmentData segmentData2 in this._fixedSegments)
			{
				if (segmentData2.RopeSegment.RopeMesh != null && segmentData2.RopeSegment.RopeMesh.GetVectorArgument().w < maximumRopeThickness)
				{
					segments.Add(segmentData2.RopeSegment);
				}
			}
			if (this._endPointRope.RopeSegment != null && this._endPointRope.RopeSegment.RopeMesh != null && this._endPointRope.RopeSegment.RopeMesh.GetVectorArgument().w < maximumRopeThickness)
			{
				segments.Add(this._endPointRope.RopeSegment);
			}
		}

		// Token: 0x040006EE RID: 1774
		private const string PulleyTag = "pulley";

		// Token: 0x040006EF RID: 1775
		private const string PulleyWheelTag = "pulley_wheel";

		// Token: 0x040006F0 RID: 1776
		private const string PulleyLeftPointTag = "pulley_left_point";

		// Token: 0x040006F1 RID: 1777
		private const string PulleyRightPointTag = "pulley_right_point";

		// Token: 0x040006F2 RID: 1778
		private const string EndPointRopeTag = "end_point_rope";

		// Token: 0x040006F3 RID: 1779
		private const string EndPointTargetTag = "end_point_target";

		// Token: 0x040006F4 RID: 1780
		private const string AttachedToYardTag = "attached_to_yard";

		// Token: 0x040006F5 RID: 1781
		private const string FreePileTag = "free_pile";

		// Token: 0x040006F6 RID: 1782
		[EditableScriptComponentVariable(true, "End Rope Length")]
		private float _endRopeLength = 2f;

		// Token: 0x040006F7 RID: 1783
		private GameEntity _pulleyEntity;

		// Token: 0x040006F8 RID: 1784
		private GameEntity _pulleyWheelEntity;

		// Token: 0x040006F9 RID: 1785
		private GameEntity _pulleyLeftRopeConnectionEntity;

		// Token: 0x040006FA RID: 1786
		private GameEntity _pulleyRightRopeConnectionEntity;

		// Token: 0x040006FB RID: 1787
		private List<RopeSegment> _tiedToYardSegments = new List<RopeSegment>();

		// Token: 0x040006FC RID: 1788
		private List<PulleySystem.SegmentData> _fixedSegments = new List<PulleySystem.SegmentData>();

		// Token: 0x040006FD RID: 1789
		private List<PulleySystem.SegmentData> _freeSegments = new List<PulleySystem.SegmentData>();

		// Token: 0x040006FE RID: 1790
		private PulleySystem.SegmentData _endPointRope;

		// Token: 0x040006FF RID: 1791
		private GameEntity _endTargetEntity;

		// Token: 0x04000700 RID: 1792
		private Vec3 _targetPositionLocalPrevFrame = Vec3.Zero;

		// Token: 0x04000701 RID: 1793
		private float _endRopeConnectionOffset;

		// Token: 0x04000702 RID: 1794
		private float _looseAmountMultiplier;

		// Token: 0x04000703 RID: 1795
		private bool _firstTick = true;

		// Token: 0x0200021B RID: 539
		private struct SegmentData
		{
			// Token: 0x04000EEB RID: 3819
			internal RopeSegment RopeSegment;

			// Token: 0x04000EEC RID: 3820
			internal GameEntity RopeEntity;
		}
	}
}
