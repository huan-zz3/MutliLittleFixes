using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace MissionLibrary.View.Widgets
{
	// Token: 0x0200000C RID: 12
	public class MissionLibraryGameKeyConfigWidget : Widget
	{
		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000032 RID: 50 RVA: 0x000022F2 File Offset: 0x000004F2
		// (set) Token: 0x06000033 RID: 51 RVA: 0x000022FA File Offset: 0x000004FA
		public RichTextWidget CurrentOptionDescriptionWidget { get; set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000034 RID: 52 RVA: 0x00002303 File Offset: 0x00000503
		// (set) Token: 0x06000035 RID: 53 RVA: 0x0000230B File Offset: 0x0000050B
		public RichTextWidget CurrentOptionNameWidget { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000036 RID: 54 RVA: 0x00002314 File Offset: 0x00000514
		// (set) Token: 0x06000037 RID: 55 RVA: 0x0000231C File Offset: 0x0000051C
		public Widget CurrentOptionImageWidget { get; set; }

		// Token: 0x06000038 RID: 56 RVA: 0x00002325 File Offset: 0x00000525
		public MissionLibraryGameKeyConfigWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002330 File Offset: 0x00000530
		public void SetCurrentOption(Widget currentOptionWidget, Sprite newGraphicsSprite)
		{
			if (this._currentOptionWidget != currentOptionWidget)
			{
				this._currentOptionWidget = currentOptionWidget;
				string text = "";
				string text2 = "";
				MissionLibraryGameKeyConfigItemWidget missionLibraryGameKeyConfigItemWidget = this._currentOptionWidget as MissionLibraryGameKeyConfigItemWidget;
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

		// Token: 0x04000009 RID: 9
		private Widget _currentOptionWidget;
	}
}
