using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.ExtraWidgets;
using TaleWorlds.Library;

namespace NavalDLC.GauntletUI.Widgets.Widgets
{
	// Token: 0x0200000C RID: 12
	public class ShipFireContainerWidget : Widget
	{
		// Token: 0x0600005B RID: 91 RVA: 0x00003326 File Offset: 0x00001526
		public ShipFireContainerWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00003330 File Offset: 0x00001530
		private void OnFireDamageUpdated()
		{
			if (base.ChildCount > 0)
			{
				float num = ((this.MaxFireHitPoints != 0) ? ((float)(this.MaxFireHitPoints - this.FireHitPoints) / (float)this.MaxFireHitPoints * 100f) : 100f);
				num = MathF.Clamp(num, 0f, 100f);
				num = (float)MathF.Floor(num);
				float num2 = (float)(100 / base.ChildCount);
				for (int i = 0; i < base.ChildCount; i++)
				{
					float num3 = (num - (float)i * num2) / num2;
					num3 = MathF.Clamp(num3, 0f, 1f);
					Widget child = base.GetChild(i);
					if (num3 == 0f)
					{
						if (num == 0f)
						{
							child.SetState("Disabled");
						}
						else
						{
							child.SetState("Inactive");
						}
					}
					else if (num3 < 1f)
					{
						child.SetState("Default");
					}
					else if (num == 100f)
					{
						child.SetState("FastBurning");
					}
					else
					{
						child.SetState("SlowBurning");
					}
					FillBarVerticalWidget fillBarVerticalWidget;
					if ((fillBarVerticalWidget = child as FillBarVerticalWidget) != null)
					{
						fillBarVerticalWidget.InitialAmountAsFloat = num3;
						fillBarVerticalWidget.MaxAmountAsFloat = 1f;
					}
				}
				if (num == 100f)
				{
					Widget compassCenterWidget = this.CompassCenterWidget;
					if (compassCenterWidget == null)
					{
						return;
					}
					compassCenterWidget.SetState("Burning");
					return;
				}
				else
				{
					Widget compassCenterWidget2 = this.CompassCenterWidget;
					if (compassCenterWidget2 == null)
					{
						return;
					}
					compassCenterWidget2.SetState("Default");
				}
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600005D RID: 93 RVA: 0x0000348A File Offset: 0x0000168A
		// (set) Token: 0x0600005E RID: 94 RVA: 0x00003492 File Offset: 0x00001692
		[Editor(false)]
		public int FireHitPoints
		{
			get
			{
				return this._fireHitPoints;
			}
			set
			{
				if (this._fireHitPoints != value)
				{
					this._fireHitPoints = value;
					base.OnPropertyChanged(value, "FireHitPoints");
					this.OnFireDamageUpdated();
				}
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600005F RID: 95 RVA: 0x000034B6 File Offset: 0x000016B6
		// (set) Token: 0x06000060 RID: 96 RVA: 0x000034BE File Offset: 0x000016BE
		[Editor(false)]
		public int MaxFireHitPoints
		{
			get
			{
				return this._maxFireHitPoints;
			}
			set
			{
				if (this._maxFireHitPoints != value)
				{
					this._maxFireHitPoints = value;
					base.OnPropertyChanged(value, "MaxFireHitPoints");
					this.OnFireDamageUpdated();
				}
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000061 RID: 97 RVA: 0x000034E2 File Offset: 0x000016E2
		// (set) Token: 0x06000062 RID: 98 RVA: 0x000034EA File Offset: 0x000016EA
		[Editor(false)]
		public Widget CompassCenterWidget
		{
			get
			{
				return this._compassCenterWidget;
			}
			set
			{
				if (this._compassCenterWidget != value)
				{
					this._compassCenterWidget = value;
					base.OnPropertyChanged<Widget>(value, "CompassCenterWidget");
					this.OnFireDamageUpdated();
				}
			}
		}

		// Token: 0x0400002B RID: 43
		private int _fireHitPoints;

		// Token: 0x0400002C RID: 44
		private int _maxFireHitPoints;

		// Token: 0x0400002D RID: 45
		private Widget _compassCenterWidget;
	}
}
