using System;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace NavalDLC.GauntletUI.Widgets.Widgets
{
	// Token: 0x0200000A RID: 10
	public class ShipControlFocusedShipParentWidget : Widget
	{
		// Token: 0x0600004C RID: 76 RVA: 0x00002F66 File Offset: 0x00001166
		public ShipControlFocusedShipParentWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00002F6F File Offset: 0x0000116F
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (base.IsVisible)
			{
				this.UpdateScreenPosition();
			}
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00002F88 File Offset: 0x00001188
		private void UpdateScreenPosition()
		{
			float num = this.Position.X - base.Size.X / 2f;
			float num2 = this.Position.X + base.Size.X / 2f;
			float num3 = this.Position.Y - base.Size.Y;
			float y = this.Position.Y;
			if (this.WSign <= 0 || num <= 0f || num2 >= base.Context.EventManager.PageSize.X || num3 <= 0f || y >= base.Context.EventManager.PageSize.Y)
			{
				Vec2 vec;
				vec..ctor(num, num3);
				Vector2 vector = base.Context.EventManager.PageSize - base.Size;
				Vec2 vec2 = vector / 2f;
				vec -= vec2;
				if (this.WSign < 0)
				{
					vec *= -1f;
				}
				float num4 = Mathf.Atan2(vec.y, vec.x) - 1.5707964f;
				float num5 = Mathf.Cos(num4);
				float num6 = Mathf.Sin(num4);
				float num7 = num5 / num6;
				Vec2 vec3 = vec2 * 1f;
				vec = ((num5 > 0f) ? new Vec2(-vec3.y / num7, vec2.y) : new Vec2(vec3.y / num7, -vec2.y));
				if (vec.x > vec3.x)
				{
					vec..ctor(vec3.x, -vec3.x * num7);
				}
				else if (vec.x < -vec3.x)
				{
					vec..ctor(-vec3.x, vec3.x * num7);
				}
				vec += vec2;
				base.ScaledPositionXOffset = Mathf.Clamp(vec.x, 0f, vector.X);
				base.ScaledPositionYOffset = Mathf.Clamp(vec.y, 0f, vector.Y);
				return;
			}
			base.ScaledPositionXOffset = num;
			base.ScaledPositionYOffset = num3;
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600004F RID: 79 RVA: 0x000031D4 File Offset: 0x000013D4
		// (set) Token: 0x06000050 RID: 80 RVA: 0x000031DC File Offset: 0x000013DC
		[DataSourceProperty]
		public int WSign
		{
			get
			{
				return this._wSign;
			}
			set
			{
				if (this._wSign != value)
				{
					this._wSign = value;
					base.OnPropertyChanged(value, "WSign");
				}
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000051 RID: 81 RVA: 0x000031FA File Offset: 0x000013FA
		// (set) Token: 0x06000052 RID: 82 RVA: 0x00003202 File Offset: 0x00001402
		[DataSourceProperty]
		public Vec2 Position
		{
			get
			{
				return this._position;
			}
			set
			{
				if (this._position != value)
				{
					this._position = value;
					base.OnPropertyChanged(value, "Position");
				}
			}
		}

		// Token: 0x04000026 RID: 38
		private int _wSign;

		// Token: 0x04000027 RID: 39
		private Vec2 _position;
	}
}
