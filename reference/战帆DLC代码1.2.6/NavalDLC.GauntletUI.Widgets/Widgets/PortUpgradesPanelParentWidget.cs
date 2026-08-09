using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace NavalDLC.GauntletUI.Widgets.Widgets
{
	// Token: 0x02000009 RID: 9
	public class PortUpgradesPanelParentWidget : Widget
	{
		// Token: 0x06000045 RID: 69 RVA: 0x00002DC6 File Offset: 0x00000FC6
		public PortUpgradesPanelParentWidget(UIContext context)
			: base(context)
		{
			base.IsVisible = false;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00002DE0 File Offset: 0x00000FE0
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._isFirstFrame)
			{
				this._fullMarginLeft = base.MarginLeft;
				this._isFirstFrame = false;
			}
			if (this.VisibilityCondition)
			{
				base.IsVisible = true;
				if (this._visibilityAnimationTimer < this._visibilityAnimationDuration)
				{
					float num = AnimationInterpolation.Ease(3, 4, MathF.Clamp(this._visibilityAnimationTimer / this._visibilityAnimationDuration, 0f, 1f));
					this.UpdateAnimation(num);
					this._visibilityAnimationTimer += dt;
					return;
				}
				this._visibilityAnimationTimer = this._visibilityAnimationDuration;
				this.UpdateAnimation(1f);
				return;
			}
			else
			{
				if (this._visibilityAnimationTimer > 0f)
				{
					float num2 = AnimationInterpolation.Ease(3, 4, MathF.Clamp(this._visibilityAnimationTimer / this._visibilityAnimationDuration, 0f, 1f));
					this.UpdateAnimation(num2);
					this._visibilityAnimationTimer -= dt;
					return;
				}
				this._visibilityAnimationTimer = 0f;
				this.UpdateAnimation(0f);
				base.IsVisible = false;
				return;
			}
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00002EE2 File Offset: 0x000010E2
		private void UpdateAnimation(float ratio)
		{
			base.MarginLeft = MathF.Lerp(this._fullMarginLeft / 2f, this._fullMarginLeft, ratio, 1E-05f);
			GauntletExtensions.SetGlobalAlphaRecursively(this, ratio * base.ParentWidget.AlphaFactor);
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000048 RID: 72 RVA: 0x00002F1A File Offset: 0x0000111A
		// (set) Token: 0x06000049 RID: 73 RVA: 0x00002F22 File Offset: 0x00001122
		[Editor(false)]
		public bool VisibilityCondition
		{
			get
			{
				return this._visibilityCondition;
			}
			set
			{
				if (value != this._visibilityCondition)
				{
					this._visibilityCondition = value;
					base.OnPropertyChanged(value, "VisibilityCondition");
				}
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600004A RID: 74 RVA: 0x00002F40 File Offset: 0x00001140
		// (set) Token: 0x0600004B RID: 75 RVA: 0x00002F48 File Offset: 0x00001148
		[Editor(false)]
		public float VisibilityAnimationDuration
		{
			get
			{
				return this._visibilityAnimationDuration;
			}
			set
			{
				if (value != this._visibilityAnimationDuration)
				{
					this._visibilityAnimationDuration = value;
					base.OnPropertyChanged(value, "VisibilityAnimationDuration");
				}
			}
		}

		// Token: 0x04000021 RID: 33
		private bool _isFirstFrame = true;

		// Token: 0x04000022 RID: 34
		private float _visibilityAnimationTimer;

		// Token: 0x04000023 RID: 35
		private float _fullMarginLeft;

		// Token: 0x04000024 RID: 36
		private bool _visibilityCondition;

		// Token: 0x04000025 RID: 37
		private float _visibilityAnimationDuration;
	}
}
