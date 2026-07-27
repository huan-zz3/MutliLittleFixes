using System;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.Library;

namespace NavalDLC.ViewModelCollection.GameMenus
{
	// Token: 0x02000036 RID: 54
	public class NavalGameMenuShipItemVM : ViewModel
	{
		// Token: 0x0600042E RID: 1070 RVA: 0x00013C87 File Offset: 0x00011E87
		public NavalGameMenuShipItemVM(Ship ship)
		{
			this.Ship = ship;
			this.RefreshValues();
		}

		// Token: 0x0600042F RID: 1071 RVA: 0x00013C9C File Offset: 0x00011E9C
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.PrefabId = NavalUIHelper.GetPrefabIdOfShipHull(this.Ship.ShipHull);
			this.Name = this.Ship.Name.ToString();
			this.HullName = this.Ship.ShipHull.Name.ToString();
			this.HasCustomName = this.Name != this.HullName;
			this.MaxHitPoints = this.Ship.MaxHitPoints;
			this.CurrentHitPoints = this.Ship.HitPoints;
		}

		// Token: 0x06000430 RID: 1072 RVA: 0x00013D2F File Offset: 0x00011F2F
		public void ExecuteBeginHint()
		{
			InformationManager.ShowTooltip(typeof(Ship), new object[] { this.Ship });
		}

		// Token: 0x06000431 RID: 1073 RVA: 0x00013D4F File Offset: 0x00011F4F
		public void ExecuteEndHint()
		{
			InformationManager.HideTooltip();
		}

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x06000432 RID: 1074 RVA: 0x00013D56 File Offset: 0x00011F56
		// (set) Token: 0x06000433 RID: 1075 RVA: 0x00013D5E File Offset: 0x00011F5E
		[DataSourceProperty]
		public bool HasCustomName
		{
			get
			{
				return this._hasCustomName;
			}
			set
			{
				if (value != this._hasCustomName)
				{
					this._hasCustomName = value;
					base.OnPropertyChangedWithValue(value, "HasCustomName");
				}
			}
		}

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x06000434 RID: 1076 RVA: 0x00013D7C File Offset: 0x00011F7C
		// (set) Token: 0x06000435 RID: 1077 RVA: 0x00013D84 File Offset: 0x00011F84
		[DataSourceProperty]
		public float MaxHitPoints
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

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x06000436 RID: 1078 RVA: 0x00013DA2 File Offset: 0x00011FA2
		// (set) Token: 0x06000437 RID: 1079 RVA: 0x00013DAA File Offset: 0x00011FAA
		[DataSourceProperty]
		public float CurrentHitPoints
		{
			get
			{
				return this._currentHitPoints;
			}
			set
			{
				if (value != this._currentHitPoints)
				{
					this._currentHitPoints = value;
					base.OnPropertyChangedWithValue(value, "CurrentHitPoints");
				}
			}
		}

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x06000438 RID: 1080 RVA: 0x00013DC8 File Offset: 0x00011FC8
		// (set) Token: 0x06000439 RID: 1081 RVA: 0x00013DD0 File Offset: 0x00011FD0
		[DataSourceProperty]
		public string PrefabId
		{
			get
			{
				return this._prefabId;
			}
			set
			{
				if (value != this._prefabId)
				{
					this._prefabId = value;
					base.OnPropertyChangedWithValue<string>(value, "PrefabId");
				}
			}
		}

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x0600043A RID: 1082 RVA: 0x00013DF3 File Offset: 0x00011FF3
		// (set) Token: 0x0600043B RID: 1083 RVA: 0x00013DFB File Offset: 0x00011FFB
		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue<string>(value, "Name");
				}
			}
		}

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x0600043C RID: 1084 RVA: 0x00013E1E File Offset: 0x0001201E
		// (set) Token: 0x0600043D RID: 1085 RVA: 0x00013E26 File Offset: 0x00012026
		[DataSourceProperty]
		public string HullName
		{
			get
			{
				return this._hullName;
			}
			set
			{
				if (value != this._hullName)
				{
					this._hullName = value;
					base.OnPropertyChangedWithValue<string>(value, "HullName");
				}
			}
		}

		// Token: 0x040001A3 RID: 419
		public readonly Ship Ship;

		// Token: 0x040001A4 RID: 420
		private bool _hasCustomName;

		// Token: 0x040001A5 RID: 421
		private float _maxHitPoints;

		// Token: 0x040001A6 RID: 422
		private float _currentHitPoints;

		// Token: 0x040001A7 RID: 423
		private string _prefabId;

		// Token: 0x040001A8 RID: 424
		private string _name;

		// Token: 0x040001A9 RID: 425
		private string _hullName;
	}
}
