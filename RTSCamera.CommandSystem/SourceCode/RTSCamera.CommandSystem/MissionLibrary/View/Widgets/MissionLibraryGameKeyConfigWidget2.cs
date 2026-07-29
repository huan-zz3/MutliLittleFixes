using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace MissionLibrary.View.Widgets
{
	// Token: 0x02000003 RID: 3
	public class MissionLibraryGameKeyConfigWidget2 : Widget
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000016 RID: 22 RVA: 0x00002307 File Offset: 0x00000507
		// (set) Token: 0x06000017 RID: 23 RVA: 0x0000230F File Offset: 0x0000050F
		public RichTextWidget CurrentOptionDescriptionWidget { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000018 RID: 24 RVA: 0x00002318 File Offset: 0x00000518
		// (set) Token: 0x06000019 RID: 25 RVA: 0x00002320 File Offset: 0x00000520
		public RichTextWidget CurrentOptionNameWidget { get; set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600001A RID: 26 RVA: 0x00002329 File Offset: 0x00000529
		// (set) Token: 0x0600001B RID: 27 RVA: 0x00002331 File Offset: 0x00000531
		public Widget CurrentOptionImageWidget { get; set; }

		// Token: 0x0600001C RID: 28 RVA: 0x0000233A File Offset: 0x0000053A
		public MissionLibraryGameKeyConfigWidget2(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002344 File Offset: 0x00000544
		public void SetCurrentOption(Widget currentOptionWidget, Sprite newGraphicsSprite)
		{
			if (this._currentOptionWidget != currentOptionWidget)
			{
				this._currentOptionWidget = currentOptionWidget;
				string text = "";
				string text2 = "";
				MissionLibraryGameKeyConfigItemWidget2 missionLibraryGameKeyConfigItemWidget = this._currentOptionWidget as MissionLibraryGameKeyConfigItemWidget2;
				if (missionLibraryGameKeyConfigItemWidget != null)
				{
					text = missionLibraryGameKeyConfigItemWidget.OptionDescription;
					text2 = missionLibraryGameKeyConfigItemWidget.OptionTitle;
				}
				if (this.CurrentOptionDescriptionWidget != null)
				{
					this.CurrentOptionDescriptionWidget.Text = text;
				}
				if (this.CurrentOptionDescriptionWidget != null)
				{
					this.CurrentOptionNameWidget.Text = text2;
				}
			}
			if (this.CurrentOptionImageWidget == null || this.CurrentOptionImageWidget.Sprite == newGraphicsSprite)
			{
				return;
			}
			this.CurrentOptionImageWidget.Sprite = newGraphicsSprite;
			if (newGraphicsSprite == null)
			{
				return;
			}
			float num = this.CurrentOptionImageWidget.SuggestedWidth / (float)newGraphicsSprite.Width;
			this.CurrentOptionImageWidget.SuggestedHeight = (float)newGraphicsSprite.Height * num;
		}

		// Token: 0x04000008 RID: 8
		private Widget _currentOptionWidget;
	}
}
