using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.ViewModelCollection.Missions.CaptureShip
{
	// Token: 0x0200002B RID: 43
	public class NavalMissionCaptureShipVM : ViewModel
	{
		// Token: 0x060003D2 RID: 978 RVA: 0x00012D0E File Offset: 0x00010F0E
		public NavalMissionCaptureShipVM(float totalCaptureTime)
		{
			this.MaxTime = totalCaptureTime;
			this.IsCapturing = false;
			this.RefreshValues();
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x00012D2A File Offset: 0x00010F2A
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.CaptureShipText = new TextObject("{=5qSIhAfx}Transferring troops and control", null).ToString();
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x00012D48 File Offset: 0x00010F48
		public void UpdateCaptureTimer(float timeLeftToCapture)
		{
			this.IsCapturing = timeLeftToCapture >= 0f;
			if (this.IsCapturing)
			{
				this.CurrentTime = this.MaxTime - timeLeftToCapture;
			}
		}

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x060003D5 RID: 981 RVA: 0x00012D71 File Offset: 0x00010F71
		// (set) Token: 0x060003D6 RID: 982 RVA: 0x00012D79 File Offset: 0x00010F79
		[DataSourceProperty]
		public float MaxTime
		{
			get
			{
				return this._maxTime;
			}
			set
			{
				if (value != this._maxTime)
				{
					this._maxTime = value;
					base.OnPropertyChangedWithValue(value, "MaxTime");
				}
			}
		}

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x060003D7 RID: 983 RVA: 0x00012D97 File Offset: 0x00010F97
		// (set) Token: 0x060003D8 RID: 984 RVA: 0x00012D9F File Offset: 0x00010F9F
		[DataSourceProperty]
		public float CurrentTime
		{
			get
			{
				return this._currentTime;
			}
			set
			{
				if (value != this._currentTime)
				{
					this._currentTime = value;
					base.OnPropertyChangedWithValue(value, "CurrentTime");
				}
			}
		}

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x060003D9 RID: 985 RVA: 0x00012DBD File Offset: 0x00010FBD
		// (set) Token: 0x060003DA RID: 986 RVA: 0x00012DC5 File Offset: 0x00010FC5
		[DataSourceProperty]
		public string CaptureShipText
		{
			get
			{
				return this._captureShipText;
			}
			set
			{
				if (value != this._captureShipText)
				{
					this._captureShipText = value;
					base.OnPropertyChangedWithValue<string>(value, "CaptureShipText");
				}
			}
		}

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x060003DB RID: 987 RVA: 0x00012DE8 File Offset: 0x00010FE8
		// (set) Token: 0x060003DC RID: 988 RVA: 0x00012DF0 File Offset: 0x00010FF0
		[DataSourceProperty]
		public bool IsCapturing
		{
			get
			{
				return this._isCapturing;
			}
			set
			{
				if (value != this._isCapturing)
				{
					this._isCapturing = value;
					base.OnPropertyChangedWithValue(value, "IsCapturing");
				}
			}
		}

		// Token: 0x0400017C RID: 380
		private float _maxTime;

		// Token: 0x0400017D RID: 381
		private float _currentTime;

		// Token: 0x0400017E RID: 382
		private string _captureShipText;

		// Token: 0x0400017F RID: 383
		private bool _isCapturing;
	}
}
