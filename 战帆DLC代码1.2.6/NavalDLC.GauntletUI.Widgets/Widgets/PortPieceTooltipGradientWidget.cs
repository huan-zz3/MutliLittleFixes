using System;
using System.Numerics;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace NavalDLC.GauntletUI.Widgets.Widgets
{
	// Token: 0x02000005 RID: 5
	public class PortPieceTooltipGradientWidget : BrushWidget
	{
		// Token: 0x06000018 RID: 24 RVA: 0x000024EE File Offset: 0x000006EE
		public PortPieceTooltipGradientWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000024F8 File Offset: 0x000006F8
		protected override void OnRender(TwoDimensionContext twoDimensionContext, TwoDimensionDrawContext drawContext)
		{
			if (this.ContainerWidget == null)
			{
				return;
			}
			Vector2 vector = this.ContainerWidget.Size * base._inverseScaleToUse;
			base.BrushRenderer.Render(drawContext, ref this.AreaRect, base._scaleToUse, base.Context.ContextAlpha, new Vector2(0f, this.IsBottomHalf ? (-vector.Y) : 0f), new Vector2(vector.X + base.Brush.DefaultLayer.ExtendLeft, this.IsBottomHalf ? (-vector.Y) : vector.Y));
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600001A RID: 26 RVA: 0x0000259B File Offset: 0x0000079B
		// (set) Token: 0x0600001B RID: 27 RVA: 0x000025A3 File Offset: 0x000007A3
		[Editor(false)]
		public bool IsBottomHalf
		{
			get
			{
				return this._isBottomHalf;
			}
			set
			{
				if (value != this._isBottomHalf)
				{
					this._isBottomHalf = value;
					base.OnPropertyChanged(value, "IsBottomHalf");
				}
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600001C RID: 28 RVA: 0x000025C1 File Offset: 0x000007C1
		// (set) Token: 0x0600001D RID: 29 RVA: 0x000025C9 File Offset: 0x000007C9
		[Editor(false)]
		public Widget ContainerWidget
		{
			get
			{
				return this._containerWidget;
			}
			set
			{
				if (value != this._containerWidget)
				{
					this._containerWidget = value;
					base.OnPropertyChanged<Widget>(value, "ContainerWidget");
				}
			}
		}

		// Token: 0x0400000B RID: 11
		private bool _isBottomHalf;

		// Token: 0x0400000C RID: 12
		private Widget _containerWidget;
	}
}
