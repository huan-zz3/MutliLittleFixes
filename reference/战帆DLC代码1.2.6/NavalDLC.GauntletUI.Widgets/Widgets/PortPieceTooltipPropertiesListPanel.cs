using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace NavalDLC.GauntletUI.Widgets.Widgets
{
	// Token: 0x02000006 RID: 6
	public class PortPieceTooltipPropertiesListPanel : ListPanel
	{
		// Token: 0x0600001E RID: 30 RVA: 0x000025E7 File Offset: 0x000007E7
		public PortPieceTooltipPropertiesListPanel(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600001F RID: 31 RVA: 0x000025F8 File Offset: 0x000007F8
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (base.ChildCount == 0 || !this._isDirty)
			{
				return;
			}
			float num = 0f;
			float num2 = 0f;
			for (int i = 0; i < base.ChildCount; i++)
			{
				Widget widget = base.Children[i].Children[0];
				Widget widget2 = base.Children[i].Children[1];
				float num3 = widget.Size.X + widget.ScaledMarginLeft + widget.ScaledMarginRight;
				float num4 = widget2.Size.X + widget2.ScaledMarginLeft + widget2.ScaledMarginRight;
				if (num < num3)
				{
					num = num3;
				}
				if (num2 < num4)
				{
					num2 = num4;
				}
			}
			float num5 = 0.5f;
			if (num2 > 0f || num > 0f)
			{
				num5 = num2 / (num2 + num);
			}
			for (int j = 0; j < base.ChildCount; j++)
			{
				Widget widget3 = base.Children[j].Children[0];
				Widget widget4 = base.Children[j].Children[1];
				widget3.WidthSizePolicy = 1;
				widget4.WidthSizePolicy = 0;
				widget4.ScaledSuggestedWidth = base.Size.X * num5;
				widget4.MinWidth = base.Size.X * 1f / 6f * base._inverseScaleToUse;
				widget4.MaxWidth = base.Size.X * 2f / 3f * base._inverseScaleToUse;
				if (widget4.IsHidden)
				{
					(widget3 as TextWidget).Brush.TextHorizontalAlignment = 2;
					(widget3 as TextWidget).Brush.TextColorFactor = 0.9f;
				}
			}
			this._isDirty = false;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000027C7 File Offset: 0x000009C7
		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			this._isDirty = true;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000027D7 File Offset: 0x000009D7
		protected override void OnAfterChildRemoved(Widget child, int previousIndexOfChild)
		{
			base.OnAfterChildRemoved(child, previousIndexOfChild);
			this._isDirty = true;
		}

		// Token: 0x0400000D RID: 13
		private bool _isDirty = true;
	}
}
