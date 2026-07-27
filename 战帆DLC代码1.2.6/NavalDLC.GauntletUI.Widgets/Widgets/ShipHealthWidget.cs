using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.Library;

namespace NavalDLC.GauntletUI.Widgets.Widgets
{
	// Token: 0x0200000D RID: 13
	public class ShipHealthWidget : Widget
	{
		// Token: 0x06000063 RID: 99 RVA: 0x0000350E File Offset: 0x0000170E
		public ShipHealthWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00003530 File Offset: 0x00001730
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this.HealthBar != null && base.IsVisible)
			{
				this.HealthBar.MaxAmount = this.MaxHealth;
				this.HealthBar.InitialAmount = this.Health;
				if (this.ChangeVisualWidget != null && this.HealthBar.ChangeWidget != null)
				{
					this.ChangeVisualWidget.PositionYOffset = -this.HealthBar.ChangeWidget.PositionYOffset;
				}
				if (this.DividerWidget != null && this.DividerVisualWidget != null && this.HealthBar.FillWidget != null)
				{
					this.DividerWidget.PositionYOffset = this.DividerWidget.Size.Y * base._inverseScaleToUse - this.HealthBar.FillWidget.Size.Y * base._inverseScaleToUse;
					this.DividerVisualWidget.PositionYOffset = -this.DividerWidget.PositionYOffset;
				}
				this.AnimateHealthDrop(dt);
			}
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00003627 File Offset: 0x00001827
		private void OnHealthDrop(int previousValue)
		{
			if (this._smoothedCurrentAmount == (float)previousValue)
			{
				this._animationStartHealth = (float)previousValue;
			}
			else
			{
				this._animationStartHealth = this._smoothedCurrentAmount;
			}
			this._currentAmountAnimationDelta = 0f;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00003654 File Offset: 0x00001854
		private void AnimateHealthDrop(float dt)
		{
			if (this._currentAmountAnimationDelta < this.AnimationDelay + this.AnimationDuration)
			{
				this._currentAmountAnimationDelta += dt;
				float num = MathF.Clamp((this._currentAmountAnimationDelta - this.AnimationDelay) / this.AnimationDuration, 0f, 1f);
				num = AnimationInterpolation.Ease(2, 0, num);
				this._smoothedCurrentAmount = MathF.Lerp(this._animationStartHealth, (float)this.Health, num, 1E-05f);
			}
			else
			{
				this._smoothedCurrentAmount = (float)this.Health;
			}
			this.HealthBar.CurrentAmount = (int)this._smoothedCurrentAmount;
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000067 RID: 103 RVA: 0x000036F0 File Offset: 0x000018F0
		// (set) Token: 0x06000068 RID: 104 RVA: 0x000036F8 File Offset: 0x000018F8
		[Editor(false)]
		public int Health
		{
			get
			{
				return this._health;
			}
			set
			{
				if (this._health != value)
				{
					int health = this._health;
					this._health = value;
					base.OnPropertyChanged(value, "Health");
					if (this._health < health)
					{
						this.OnHealthDrop(health);
					}
				}
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000069 RID: 105 RVA: 0x00003738 File Offset: 0x00001938
		// (set) Token: 0x0600006A RID: 106 RVA: 0x00003740 File Offset: 0x00001940
		[Editor(false)]
		public int MaxHealth
		{
			get
			{
				return this._maxHealth;
			}
			set
			{
				if (this._maxHealth != value)
				{
					this._maxHealth = value;
					base.OnPropertyChanged(value, "MaxHealth");
				}
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600006B RID: 107 RVA: 0x0000375E File Offset: 0x0000195E
		// (set) Token: 0x0600006C RID: 108 RVA: 0x00003766 File Offset: 0x00001966
		[Editor(false)]
		public FillBarVerticalWidget HealthBar
		{
			get
			{
				return this._healthBar;
			}
			set
			{
				if (this._healthBar != value)
				{
					this._healthBar = value;
					base.OnPropertyChanged<FillBarVerticalWidget>(value, "HealthBar");
				}
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600006D RID: 109 RVA: 0x00003784 File Offset: 0x00001984
		// (set) Token: 0x0600006E RID: 110 RVA: 0x0000378C File Offset: 0x0000198C
		[Editor(false)]
		public Widget ChangeVisualWidget
		{
			get
			{
				return this._changeVisualWidget;
			}
			set
			{
				if (this._changeVisualWidget != value)
				{
					this._changeVisualWidget = value;
					base.OnPropertyChanged<Widget>(value, "ChangeVisualWidget");
				}
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600006F RID: 111 RVA: 0x000037AA File Offset: 0x000019AA
		// (set) Token: 0x06000070 RID: 112 RVA: 0x000037B2 File Offset: 0x000019B2
		[Editor(false)]
		public Widget DividerWidget
		{
			get
			{
				return this._dividerWidget;
			}
			set
			{
				if (this._dividerWidget != value)
				{
					this._dividerWidget = value;
					base.OnPropertyChanged<Widget>(value, "DividerWidget");
				}
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000071 RID: 113 RVA: 0x000037D0 File Offset: 0x000019D0
		// (set) Token: 0x06000072 RID: 114 RVA: 0x000037D8 File Offset: 0x000019D8
		[Editor(false)]
		public Widget DividerVisualWidget
		{
			get
			{
				return this._dividerVisualWidget;
			}
			set
			{
				if (this._dividerVisualWidget != value)
				{
					this._dividerVisualWidget = value;
					base.OnPropertyChanged<Widget>(value, "DividerVisualWidget");
				}
			}
		}

		// Token: 0x0400002E RID: 46
		public float AnimationDelay = 0.2f;

		// Token: 0x0400002F RID: 47
		public float AnimationDuration = 0.8f;

		// Token: 0x04000030 RID: 48
		private float _animationStartHealth;

		// Token: 0x04000031 RID: 49
		private float _smoothedCurrentAmount;

		// Token: 0x04000032 RID: 50
		private float _currentAmountAnimationDelta;

		// Token: 0x04000033 RID: 51
		private int _health;

		// Token: 0x04000034 RID: 52
		private int _maxHealth;

		// Token: 0x04000035 RID: 53
		private FillBarVerticalWidget _healthBar;

		// Token: 0x04000036 RID: 54
		private Widget _changeVisualWidget;

		// Token: 0x04000037 RID: 55
		private Widget _dividerWidget;

		// Token: 0x04000038 RID: 56
		private Widget _dividerVisualWidget;
	}
}
