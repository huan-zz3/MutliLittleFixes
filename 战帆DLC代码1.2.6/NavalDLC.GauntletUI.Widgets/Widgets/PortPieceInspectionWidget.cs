using System;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace NavalDLC.GauntletUI.Widgets.Widgets
{
	// Token: 0x02000004 RID: 4
	public class PortPieceInspectionWidget : BrushWidget
	{
		// Token: 0x06000006 RID: 6 RVA: 0x00002119 File Offset: 0x00000319
		public PortPieceInspectionWidget(UIContext context)
			: base(context)
		{
			GauntletExtensions.SetGlobalAlphaRecursively(this, 0f);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x0000212D File Offset: 0x0000032D
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._targetPiece != null)
			{
				this.UpdateAnimation(dt);
			}
			this.HandleAlphaFactor(dt);
		}

		// Token: 0x06000008 RID: 8 RVA: 0x0000214C File Offset: 0x0000034C
		private void HandleAlphaFactor(float dt)
		{
			bool flag = this._targetPiece != null && this.IsInspected;
			if (this.FadeInOutDuration <= 0f)
			{
				this._currentAlpha = (flag ? 1f : 0f);
			}
			else
			{
				if (flag)
				{
					this._fadeInOutDelta += dt;
				}
				else
				{
					this._fadeInOutDelta -= dt;
				}
				this._fadeInOutDelta = MathF.Clamp(this._fadeInOutDelta, 0f, this.FadeInOutDuration + this.FadeOutDelay);
				float num = MathF.Clamp(this._fadeInOutDelta / this.FadeInOutDuration, 0f, 1f);
				float num2 = AnimationInterpolation.Ease(3, 2, num);
				this._currentAlpha = MathF.Lerp(0f, 1f, num2, 1E-05f);
			}
			GauntletExtensions.SetGlobalAlphaRecursively(this, this._currentAlpha);
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002224 File Offset: 0x00000424
		private void UpdateAnimation(float dt)
		{
			bool flag = base.PositionXOffset == 0f && base.PositionYOffset == 0f;
			base.VerticalAlignment = 0;
			base.HorizontalAlignment = 0;
			float num = ((this.AnimationSpeed != 0f) ? MBMath.ClampFloat(this.AnimationSpeed * dt, 0f, 1f) : 1f);
			Vector2 center = this._targetPiece.AreaRect.GetCenter();
			Vector2 vector;
			vector..ctor(base.PositionXOffset, base.PositionYOffset);
			Vector2 vector2 = center * base._inverseScaleToUse + new Vector2(this.OffsetFromTarget, -base.Size.Y * base._inverseScaleToUse * 0.5f);
			Vector2 vector3 = Vector2.Lerp(vector, vector2, num);
			base.PositionXOffset = vector3.X;
			base.PositionYOffset = this.ClampYPosition(vector3.Y);
			Vector2 vector4 = center * base._inverseScaleToUse;
			float num2 = this.ClampYPosition(vector2.Y);
			float num3 = this.AreaRect.GetBoundingBox().Y - this.TopFrameWidget.AreaRect.GetBoundingBox().Y;
			float num4 = vector4.Y - num2 + num3;
			this.TopFrameWidget.SuggestedHeight = MathF.Max(0f, MathF.Lerp(this.TopFrameWidget.SuggestedHeight, num4, num, 1E-05f));
			if (flag)
			{
				base.PositionXOffset = vector2.X;
				base.PositionYOffset = this.ClampYPosition(vector2.Y);
				this.TopFrameWidget.SuggestedHeight = MathF.Max(0f, num4);
			}
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000023BA File Offset: 0x000005BA
		private float ClampYPosition(float positionToClamp)
		{
			return MBMath.ClampFloat(positionToClamp, 0f, (base.EventManager.PageSize.Y - base.Size.Y) * base._inverseScaleToUse - 70f);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000023F0 File Offset: 0x000005F0
		public void SetTargetPiece(PortInspectionParentWidget targetPiece)
		{
			if (this._targetPiece != targetPiece && this.IsInspected)
			{
				this._targetPiece = targetPiece;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000C RID: 12 RVA: 0x0000240A File Offset: 0x0000060A
		// (set) Token: 0x0600000D RID: 13 RVA: 0x00002412 File Offset: 0x00000612
		[Editor(false)]
		public bool IsInspected
		{
			get
			{
				return this._isInspected;
			}
			set
			{
				if (value != this._isInspected)
				{
					this._isInspected = value;
					base.OnPropertyChanged(value, "IsInspected");
				}
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000E RID: 14 RVA: 0x00002430 File Offset: 0x00000630
		// (set) Token: 0x0600000F RID: 15 RVA: 0x00002438 File Offset: 0x00000638
		[Editor(false)]
		public float AnimationSpeed
		{
			get
			{
				return this._animationSpeed;
			}
			set
			{
				if (value != this._animationSpeed)
				{
					this._animationSpeed = value;
					base.OnPropertyChanged(value, "AnimationSpeed");
				}
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000010 RID: 16 RVA: 0x00002456 File Offset: 0x00000656
		// (set) Token: 0x06000011 RID: 17 RVA: 0x0000245E File Offset: 0x0000065E
		[Editor(false)]
		public float FadeInOutDuration
		{
			get
			{
				return this._fadeInOutDuration;
			}
			set
			{
				if (value != this._fadeInOutDuration)
				{
					this._fadeInOutDuration = value;
					base.OnPropertyChanged(value, "FadeInOutDuration");
				}
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000012 RID: 18 RVA: 0x0000247C File Offset: 0x0000067C
		// (set) Token: 0x06000013 RID: 19 RVA: 0x00002484 File Offset: 0x00000684
		[Editor(false)]
		public float FadeOutDelay
		{
			get
			{
				return this._fadeOutDelay;
			}
			set
			{
				if (value != this._fadeOutDelay)
				{
					this._fadeOutDelay = value;
					base.OnPropertyChanged(value, "FadeOutDelay");
				}
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000014 RID: 20 RVA: 0x000024A2 File Offset: 0x000006A2
		// (set) Token: 0x06000015 RID: 21 RVA: 0x000024AA File Offset: 0x000006AA
		[Editor(false)]
		public float OffsetFromTarget
		{
			get
			{
				return this._offsetFromTarget;
			}
			set
			{
				if (value != this._offsetFromTarget)
				{
					this._offsetFromTarget = value;
					base.OnPropertyChanged(value, "OffsetFromTarget");
				}
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000016 RID: 22 RVA: 0x000024C8 File Offset: 0x000006C8
		// (set) Token: 0x06000017 RID: 23 RVA: 0x000024D0 File Offset: 0x000006D0
		[Editor(false)]
		public Widget TopFrameWidget
		{
			get
			{
				return this._topFrameWidget;
			}
			set
			{
				if (value != this._topFrameWidget)
				{
					this._topFrameWidget = value;
					base.OnPropertyChanged<Widget>(value, "TopFrameWidget");
				}
			}
		}

		// Token: 0x04000002 RID: 2
		private PortInspectionParentWidget _targetPiece;

		// Token: 0x04000003 RID: 3
		private float _fadeInOutDelta;

		// Token: 0x04000004 RID: 4
		private float _currentAlpha;

		// Token: 0x04000005 RID: 5
		private bool _isInspected;

		// Token: 0x04000006 RID: 6
		private float _animationSpeed;

		// Token: 0x04000007 RID: 7
		private float _fadeInOutDuration;

		// Token: 0x04000008 RID: 8
		private float _fadeOutDelay;

		// Token: 0x04000009 RID: 9
		private float _offsetFromTarget;

		// Token: 0x0400000A RID: 10
		private Widget _topFrameWidget;
	}
}
