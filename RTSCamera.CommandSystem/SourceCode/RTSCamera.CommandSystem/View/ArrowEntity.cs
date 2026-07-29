using System;
using MissionSharedLibrary.Config;
using RTSCamera.CommandSystem.Config;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace RTSCamera.CommandSystem.View
{
	// Token: 0x02000050 RID: 80
	public class ArrowEntity
	{
		// Token: 0x06000281 RID: 641 RVA: 0x0000906C File Offset: 0x0000726C
		public void UpdateColor(OrderTargetType orderTargetType)
		{
			OrderTargetType? targetType = this.TargetType;
			if (!((orderTargetType == targetType.GetValueOrDefault()) & (targetType != null)))
			{
				uint colorForTargetType = ArrowEntity.GetColorForTargetType(orderTargetType);
				this.ArrowHead.SetFactorColor(colorForTargetType);
				this.ArrowHead.SetContourColor(new uint?(colorForTargetType), true);
				this.ArrowBody.SetFactorColor(colorForTargetType);
				this.ArrowBody.SetContourColor(new uint?(colorForTargetType), true);
				this.TargetType = new OrderTargetType?(orderTargetType);
			}
		}

		// Token: 0x06000282 RID: 642 RVA: 0x000090E3 File Offset: 0x000072E3
		public static uint GetColorForTargetType(OrderTargetType orderTargetType)
		{
			switch (orderTargetType)
			{
			case OrderTargetType.Move:
				return ArrowEntity.ArrowColor;
			case OrderTargetType.Focus:
				return ArrowEntity.FocusingArrowColor;
			case OrderTargetType.Attack:
				return ArrowEntity.AttackingArrowColor;
			case OrderTargetType.Facing:
				return ArrowEntity.FacingArrowColor;
			default:
				return ArrowEntity.ArrowColor;
			}
		}

		// Token: 0x06000283 RID: 643 RVA: 0x0000911C File Offset: 0x0000731C
		public void Hide(bool isPreviewShown)
		{
			if (isPreviewShown)
			{
				this.ArrowHead.SetVisibilityExcludeParents(false);
				this.ArrowBody.SetVisibilityExcludeParents(false);
			}
			else if (this._isShown)
			{
				if (this._alpha == -1f)
				{
					GameEntityExtensions.FadeOut(this.ArrowHead, MissionConfigBase<CommandSystemConfig>.Get().MovementTargetFadeOutDuration, false);
					GameEntityExtensions.FadeOut(this.ArrowBody, MissionConfigBase<CommandSystemConfig>.Get().MovementTargetFadeOutDuration, false);
				}
				else
				{
					GameEntityExtensions.HideIfNotFadingOut(this.ArrowHead);
					GameEntityExtensions.HideIfNotFadingOut(this.ArrowBody);
				}
			}
			this._isShown = false;
		}

		// Token: 0x04000109 RID: 265
		public static uint ArrowColor = new Color(0.4f, 0.8f, 0.4f, 1f).ToUnsignedInteger();

		// Token: 0x0400010A RID: 266
		public static uint FocusingArrowColor = new Color(0.7f, 0.3f, 0.2f, 1f).ToUnsignedInteger();

		// Token: 0x0400010B RID: 267
		public static uint AttackingArrowColor = new Color(0.95f, 0.1f, 0.1f, 1f).ToUnsignedInteger();

		// Token: 0x0400010C RID: 268
		public static uint FacingArrowColor = new Color(0.9f, 0.6f, 0.2f, 1f).ToUnsignedInteger();

		// Token: 0x0400010D RID: 269
		public GameEntity ArrowHead;

		// Token: 0x0400010E RID: 270
		public GameEntity ArrowBody;

		// Token: 0x0400010F RID: 271
		public OrderTargetType? TargetType;

		// Token: 0x04000110 RID: 272
		public bool _isShown;

		// Token: 0x04000111 RID: 273
		public float _alpha;
	}
}
