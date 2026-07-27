using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace NavalDLC.GauntletUI.Widgets.Widgets
{
	// Token: 0x02000008 RID: 8
	public class PortUpgradesPanelArrowWidget : Widget
	{
		// Token: 0x06000041 RID: 65 RVA: 0x00002CCB File Offset: 0x00000ECB
		public PortUpgradesPanelArrowWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00002CDF File Offset: 0x00000EDF
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._targetSlot != null)
			{
				this.UpdateAnimation(dt);
			}
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00002CF8 File Offset: 0x00000EF8
		private void UpdateAnimation(float dt)
		{
			base.VerticalAlignment = 0;
			float y = this._targetSlot.AreaRect.GetCenter().Y;
			float y2 = this.AreaRect.GetCenter().Y;
			float num = y * base._inverseScaleToUse - y2 * base._inverseScaleToUse;
			if (this._currentLerpSpeed > 0f)
			{
				float num2 = MathF.Lerp(0f, num, this._currentLerpSpeed * dt, 1E-05f);
				if (MathF.Abs(num - num2) < 1f)
				{
					this._currentLerpSpeed = -1f;
				}
				else
				{
					this._currentLerpSpeed += 10f * dt;
				}
				num = num2;
			}
			base.PositionYOffset += num;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00002DA9 File Offset: 0x00000FA9
		public void SetTargetSlot(Widget targetSlot)
		{
			if (this._targetSlot != targetSlot)
			{
				this._targetSlot = targetSlot;
				this._currentLerpSpeed = 10f;
			}
		}

		// Token: 0x0400001F RID: 31
		private Widget _targetSlot;

		// Token: 0x04000020 RID: 32
		private float _currentLerpSpeed = -1f;
	}
}
