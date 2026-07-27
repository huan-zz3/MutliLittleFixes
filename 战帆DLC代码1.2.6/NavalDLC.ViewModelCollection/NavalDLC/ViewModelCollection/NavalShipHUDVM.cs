using System;
using TaleWorlds.Library;

namespace NavalDLC.ViewModelCollection
{
	// Token: 0x02000007 RID: 7
	public class NavalShipHUDVM : ViewModel
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600001D RID: 29 RVA: 0x000051D8 File Offset: 0x000033D8
		// (set) Token: 0x0600001E RID: 30 RVA: 0x000051E0 File Offset: 0x000033E0
		[DataSourceProperty]
		public bool IsControllingShip
		{
			get
			{
				return this._isControllingShip;
			}
			set
			{
				if (value != this._isControllingShip)
				{
					this._isControllingShip = value;
					base.OnPropertyChangedWithValue(value, "IsControllingShip");
				}
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600001F RID: 31 RVA: 0x000051FE File Offset: 0x000033FE
		// (set) Token: 0x06000020 RID: 32 RVA: 0x00005206 File Offset: 0x00003406
		[DataSourceProperty]
		public float ShipHealth
		{
			get
			{
				return this._shipHealth;
			}
			set
			{
				if (value != this._shipHealth)
				{
					this._shipHealth = value;
					base.OnPropertyChangedWithValue(value, "ShipHealth");
				}
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000021 RID: 33 RVA: 0x00005224 File Offset: 0x00003424
		// (set) Token: 0x06000022 RID: 34 RVA: 0x0000522C File Offset: 0x0000342C
		[DataSourceProperty]
		public float MaxShipHealth
		{
			get
			{
				return this._maxShipHealth;
			}
			set
			{
				if (value != this._maxShipHealth)
				{
					this._maxShipHealth = value;
					base.OnPropertyChangedWithValue(value, "MaxShipHealth");
				}
			}
		}

		// Token: 0x04000008 RID: 8
		private bool _isControllingShip;

		// Token: 0x04000009 RID: 9
		private float _shipHealth;

		// Token: 0x0400000A RID: 10
		private float _maxShipHealth;
	}
}
