using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace NavalDLC.GauntletUI.Widgets.Widgets
{
	// Token: 0x02000003 RID: 3
	public class PortPieceImageBrushWidget : BrushWidget
	{
		// Token: 0x06000002 RID: 2 RVA: 0x00002051 File Offset: 0x00000251
		public PortPieceImageBrushWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000003 RID: 3 RVA: 0x0000205C File Offset: 0x0000025C
		private void UpdateIcon()
		{
			if (base.Brush == null)
			{
				return;
			}
			Sprite sprite = base.Context.SpriteData.GetSprite("PieceThumbnails\\" + this.Identifier);
			base.Brush.Sprite = sprite;
			foreach (BrushLayer brushLayer in base.Brush.Layers)
			{
				brushLayer.Sprite = sprite;
			}
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000004 RID: 4 RVA: 0x000020E8 File Offset: 0x000002E8
		// (set) Token: 0x06000005 RID: 5 RVA: 0x000020F0 File Offset: 0x000002F0
		public string Identifier
		{
			get
			{
				return this._identifier;
			}
			set
			{
				if (value != this._identifier)
				{
					this._identifier = value;
					base.OnPropertyChanged<string>(value, "Identifier");
					this.UpdateIcon();
				}
			}
		}

		// Token: 0x04000001 RID: 1
		private string _identifier;
	}
}
