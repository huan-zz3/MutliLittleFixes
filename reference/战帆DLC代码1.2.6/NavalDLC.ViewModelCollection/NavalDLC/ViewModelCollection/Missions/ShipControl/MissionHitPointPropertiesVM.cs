using System;
using TaleWorlds.Library;

namespace NavalDLC.ViewModelCollection.Missions.ShipControl
{
	// Token: 0x02000027 RID: 39
	public class MissionHitPointPropertiesVM : ViewModel
	{
		// Token: 0x170000DD RID: 221
		// (get) Token: 0x06000370 RID: 880 RVA: 0x000120D2 File Offset: 0x000102D2
		// (set) Token: 0x06000371 RID: 881 RVA: 0x000120DA File Offset: 0x000102DA
		[DataSourceProperty]
		public bool IsRelevant
		{
			get
			{
				return this._isRelevant;
			}
			set
			{
				if (value != this._isRelevant)
				{
					this._isRelevant = value;
					base.OnPropertyChangedWithValue(value, "IsRelevant");
				}
			}
		}

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x06000372 RID: 882 RVA: 0x000120F8 File Offset: 0x000102F8
		// (set) Token: 0x06000373 RID: 883 RVA: 0x00012100 File Offset: 0x00010300
		[DataSourceProperty]
		public int ActiveHitPoints
		{
			get
			{
				return this._activeHitPoints;
			}
			set
			{
				if (value != this._activeHitPoints)
				{
					this._activeHitPoints = value;
					base.OnPropertyChangedWithValue(value, "ActiveHitPoints");
				}
			}
		}

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x06000374 RID: 884 RVA: 0x0001211E File Offset: 0x0001031E
		// (set) Token: 0x06000375 RID: 885 RVA: 0x00012126 File Offset: 0x00010326
		[DataSourceProperty]
		public int MaxHitPoints
		{
			get
			{
				return this._maxHitPoints;
			}
			set
			{
				if (value != this._maxHitPoints)
				{
					this._maxHitPoints = value;
					base.OnPropertyChangedWithValue(value, "MaxHitPoints");
				}
			}
		}

		// Token: 0x04000153 RID: 339
		private bool _isRelevant;

		// Token: 0x04000154 RID: 340
		private int _activeHitPoints;

		// Token: 0x04000155 RID: 341
		private int _maxHitPoints;
	}
}
