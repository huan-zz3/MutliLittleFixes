using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace NavalDLC.GauntletUI.Widgets.Widgets
{
	// Token: 0x0200000B RID: 11
	public class ShipControlWindIndicatorParent : Widget
	{
		// Token: 0x06000053 RID: 83 RVA: 0x00003225 File Offset: 0x00001425
		public ShipControlWindIndicatorParent(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00003230 File Offset: 0x00001430
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this.WindHandle != null)
			{
				Vec2 vec = this.ProjectedWindDirection.Normalized();
				this.WindHandle.PivotX = 0.5f;
				this.WindHandle.PivotY = 0.5f;
				this.WindHandle.Rotation = Mathf.Atan2(vec.x, vec.y) * 57.295776f - 90f;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000055 RID: 85 RVA: 0x000032A3 File Offset: 0x000014A3
		// (set) Token: 0x06000056 RID: 86 RVA: 0x000032AB File Offset: 0x000014AB
		[Editor(false)]
		public Widget WindHandle
		{
			get
			{
				return this._windHandle;
			}
			set
			{
				if (value != this._windHandle)
				{
					this._windHandle = value;
					base.OnPropertyChanged<Widget>(value, "WindHandle");
				}
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000057 RID: 87 RVA: 0x000032C9 File Offset: 0x000014C9
		// (set) Token: 0x06000058 RID: 88 RVA: 0x000032D1 File Offset: 0x000014D1
		[Editor(false)]
		public string SailState
		{
			get
			{
				return this._sailState;
			}
			set
			{
				if (value != this._sailState)
				{
					this._sailState = value;
					base.OnPropertyChanged<string>(value, "SailState");
					this.SetState(value);
				}
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000059 RID: 89 RVA: 0x000032FB File Offset: 0x000014FB
		// (set) Token: 0x0600005A RID: 90 RVA: 0x00003303 File Offset: 0x00001503
		[Editor(false)]
		public Vec2 ProjectedWindDirection
		{
			get
			{
				return this._projectedWindDirection;
			}
			set
			{
				if (value != this._projectedWindDirection)
				{
					this._projectedWindDirection = value;
					base.OnPropertyChanged(value, "ProjectedWindDirection");
				}
			}
		}

		// Token: 0x04000028 RID: 40
		private Widget _windHandle;

		// Token: 0x04000029 RID: 41
		private string _sailState;

		// Token: 0x0400002A RID: 42
		private Vec2 _projectedWindDirection;
	}
}
