using System;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Library;

namespace NavalDLC.ViewModelCollection.Map
{
	// Token: 0x0200002C RID: 44
	public class MapAnchorTrackerVM : ViewModel
	{
		// Token: 0x060003DD RID: 989 RVA: 0x00012E0E File Offset: 0x0001100E
		public MapAnchorTrackerVM(Action onMoveCameraToPosition)
		{
			this._onMoveCameraToPosition = onMoveCameraToPosition;
		}

		// Token: 0x060003DE RID: 990 RVA: 0x00012E1D File Offset: 0x0001101D
		public void ExecuteGoToPosition()
		{
			Action onMoveCameraToPosition = this._onMoveCameraToPosition;
			if (onMoveCameraToPosition == null)
			{
				return;
			}
			onMoveCameraToPosition();
		}

		// Token: 0x060003DF RID: 991 RVA: 0x00012E2F File Offset: 0x0001102F
		public void ExecuteShowTooltip()
		{
			AnchorPoint anchor = MobileParty.MainParty.Anchor;
			if (anchor != null && anchor.IsValid)
			{
				InformationManager.ShowTooltip(typeof(AnchorPoint), new object[] { MobileParty.MainParty.Anchor });
			}
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x00012E6B File Offset: 0x0001106B
		public void ExecuteHideTooltip()
		{
			InformationManager.HideTooltip();
		}

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x060003E1 RID: 993 RVA: 0x00012E72 File Offset: 0x00011072
		// (set) Token: 0x060003E2 RID: 994 RVA: 0x00012E7A File Offset: 0x0001107A
		[DataSourceProperty]
		public bool IsVisible
		{
			get
			{
				return this._isVisible;
			}
			set
			{
				if (value != this._isVisible)
				{
					this._isVisible = value;
					base.OnPropertyChangedWithValue(value, "IsVisible");
				}
			}
		}

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x060003E3 RID: 995 RVA: 0x00012E98 File Offset: 0x00011098
		// (set) Token: 0x060003E4 RID: 996 RVA: 0x00012EA0 File Offset: 0x000110A0
		[DataSourceProperty]
		public float PositionX
		{
			get
			{
				return this._positionX;
			}
			set
			{
				if (value != this._positionX)
				{
					this._positionX = value;
					base.OnPropertyChangedWithValue(value, "PositionX");
				}
			}
		}

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x060003E5 RID: 997 RVA: 0x00012EBE File Offset: 0x000110BE
		// (set) Token: 0x060003E6 RID: 998 RVA: 0x00012EC6 File Offset: 0x000110C6
		[DataSourceProperty]
		public float PositionY
		{
			get
			{
				return this._positionY;
			}
			set
			{
				if (value != this._positionY)
				{
					this._positionY = value;
					base.OnPropertyChangedWithValue(value, "PositionY");
				}
			}
		}

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x060003E7 RID: 999 RVA: 0x00012EE4 File Offset: 0x000110E4
		// (set) Token: 0x060003E8 RID: 1000 RVA: 0x00012EEC File Offset: 0x000110EC
		[DataSourceProperty]
		public float PositionW
		{
			get
			{
				return this._positionW;
			}
			set
			{
				if (value != this._positionW)
				{
					this._positionW = value;
					base.OnPropertyChangedWithValue(value, "PositionW");
				}
			}
		}

		// Token: 0x04000180 RID: 384
		private readonly Action _onMoveCameraToPosition;

		// Token: 0x04000181 RID: 385
		private bool _isVisible;

		// Token: 0x04000182 RID: 386
		private float _positionX;

		// Token: 0x04000183 RID: 387
		private float _positionY;

		// Token: 0x04000184 RID: 388
		private float _positionW;
	}
}
