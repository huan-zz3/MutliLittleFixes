using System;
using NavalDLC.Missions.Objects;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD.FormationMarker;

namespace NavalDLC.ViewModelCollection.HUD.ShipMarker
{
	// Token: 0x02000034 RID: 52
	public class NavalShipMarkerItemVM : ViewModel
	{
		// Token: 0x060003FE RID: 1022 RVA: 0x000133F8 File Offset: 0x000115F8
		public NavalShipMarkerItemVM(Formation formation, MissionShip ship)
		{
			this.Formation = formation;
			this.Ship = ship;
			this._formationType = MissionFormationMarkerTargetVM.GetFormationType(this.Formation.RepresentativeClass);
			string text = "Ship_";
			MissionShip ship2 = this.Ship;
			this._shipType = text + ((ship2 != null) ? ship2.ShipOrigin.Hull.Type.ToString() : null);
			if (this.Formation.Team.IsPlayerTeam)
			{
				this.TeamType = 0;
			}
			else if (this.Formation.Team.IsPlayerAlly)
			{
				this.TeamType = 1;
			}
			else
			{
				this.TeamType = 2;
			}
			this.Refresh();
		}

		// Token: 0x060003FF RID: 1023 RVA: 0x000134AC File Offset: 0x000116AC
		public void Refresh()
		{
			this.Size = this.Formation.CountOfUnits;
			this.HasAnyTroops = this.Size > 0;
			this.MarkerType = (this.IsShipActive() ? this._shipType : this._formationType);
			this.HitPoints = (this.IsShipActive() ? this.Ship.HitPoints : 0f);
			MissionShip ship = this.Ship;
			this.MaxHitPoints = ((ship != null) ? ship.MaxHealth : 1f);
		}

		// Token: 0x06000400 RID: 1024 RVA: 0x00013531 File Offset: 0x00011731
		public void SetTargetedState(bool isFocused, bool isTargetingAShip)
		{
			this.IsCenterOfFocus = isFocused;
			this.IsTargetingAShip = isTargetingAShip;
		}

		// Token: 0x06000401 RID: 1025 RVA: 0x00013544 File Offset: 0x00011744
		public bool IsShipActive()
		{
			return this.Ship != null && !this.Ship.IsDisabled && !this.Ship.IsSinking && !this.Ship.IsRemoved && this.Ship.HitPoints > 0f;
		}

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x06000402 RID: 1026 RVA: 0x00013594 File Offset: 0x00011794
		// (set) Token: 0x06000403 RID: 1027 RVA: 0x0001359C File Offset: 0x0001179C
		[DataSourceProperty]
		public int TeamType
		{
			get
			{
				return this._teamType;
			}
			set
			{
				if (value != this._teamType)
				{
					this._teamType = value;
					base.OnPropertyChangedWithValue(value, "TeamType");
				}
			}
		}

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x06000404 RID: 1028 RVA: 0x000135BA File Offset: 0x000117BA
		// (set) Token: 0x06000405 RID: 1029 RVA: 0x000135C2 File Offset: 0x000117C2
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x06000406 RID: 1030 RVA: 0x000135E0 File Offset: 0x000117E0
		// (set) Token: 0x06000407 RID: 1031 RVA: 0x000135E8 File Offset: 0x000117E8
		[DataSourceProperty]
		public bool IsCenterOfFocus
		{
			get
			{
				return this._isCenterOfFocus;
			}
			set
			{
				if (this._isCenterOfFocus != value)
				{
					this._isCenterOfFocus = value;
					base.OnPropertyChangedWithValue(value, "IsCenterOfFocus");
				}
			}
		}

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x06000408 RID: 1032 RVA: 0x00013606 File Offset: 0x00011806
		// (set) Token: 0x06000409 RID: 1033 RVA: 0x0001360E File Offset: 0x0001180E
		[DataSourceProperty]
		public bool IsShipTargetRelevant
		{
			get
			{
				return this._isShipTargetRelevant;
			}
			set
			{
				if (this._isShipTargetRelevant != value)
				{
					this._isShipTargetRelevant = value;
					base.OnPropertyChangedWithValue(value, "IsShipTargetRelevant");
				}
			}
		}

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x0600040A RID: 1034 RVA: 0x0001362C File Offset: 0x0001182C
		// (set) Token: 0x0600040B RID: 1035 RVA: 0x00013634 File Offset: 0x00011834
		[DataSourceProperty]
		public bool IsTargetingAShip
		{
			get
			{
				return this._isTargetingAShip;
			}
			set
			{
				if (this._isTargetingAShip != value)
				{
					this._isTargetingAShip = value;
					base.OnPropertyChangedWithValue(value, "IsTargetingAShip");
				}
			}
		}

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x0600040C RID: 1036 RVA: 0x00013652 File Offset: 0x00011852
		// (set) Token: 0x0600040D RID: 1037 RVA: 0x0001365A File Offset: 0x0001185A
		[DataSourceProperty]
		public bool ShowDistanceTexts
		{
			get
			{
				return this._showDistanceTexts;
			}
			set
			{
				if (this._showDistanceTexts != value)
				{
					this._showDistanceTexts = value;
					base.OnPropertyChangedWithValue(value, "ShowDistanceTexts");
				}
			}
		}

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x0600040E RID: 1038 RVA: 0x00013678 File Offset: 0x00011878
		// (set) Token: 0x0600040F RID: 1039 RVA: 0x00013680 File Offset: 0x00011880
		[DataSourceProperty]
		public int Size
		{
			get
			{
				return this._size;
			}
			set
			{
				if (value != this._size)
				{
					this._size = value;
					base.OnPropertyChangedWithValue(value, "Size");
				}
			}
		}

		// Token: 0x17000111 RID: 273
		// (get) Token: 0x06000410 RID: 1040 RVA: 0x0001369E File Offset: 0x0001189E
		// (set) Token: 0x06000411 RID: 1041 RVA: 0x000136A6 File Offset: 0x000118A6
		[DataSourceProperty]
		public int WSign
		{
			get
			{
				return this._wSign;
			}
			set
			{
				if (value != this._wSign)
				{
					this._wSign = value;
					base.OnPropertyChangedWithValue(value, "WSign");
				}
			}
		}

		// Token: 0x17000112 RID: 274
		// (get) Token: 0x06000412 RID: 1042 RVA: 0x000136C4 File Offset: 0x000118C4
		// (set) Token: 0x06000413 RID: 1043 RVA: 0x000136CC File Offset: 0x000118CC
		[DataSourceProperty]
		public float Distance
		{
			get
			{
				return this._distance;
			}
			set
			{
				if (value != this._distance)
				{
					this._distance = value;
					base.OnPropertyChangedWithValue(value, "Distance");
				}
			}
		}

		// Token: 0x17000113 RID: 275
		// (get) Token: 0x06000414 RID: 1044 RVA: 0x000136EA File Offset: 0x000118EA
		// (set) Token: 0x06000415 RID: 1045 RVA: 0x000136F2 File Offset: 0x000118F2
		[DataSourceProperty]
		public string DistanceText
		{
			get
			{
				return this._distanceText;
			}
			set
			{
				if (value != this._distanceText)
				{
					this._distanceText = value;
					base.OnPropertyChangedWithValue<string>(value, "DistanceText");
				}
			}
		}

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x06000416 RID: 1046 RVA: 0x00013715 File Offset: 0x00011915
		// (set) Token: 0x06000417 RID: 1047 RVA: 0x0001371D File Offset: 0x0001191D
		[DataSourceProperty]
		public int CrewCount
		{
			get
			{
				return this._crewCount;
			}
			set
			{
				if (value != this._crewCount)
				{
					this._crewCount = value;
					base.OnPropertyChangedWithValue(value, "CrewCount");
				}
			}
		}

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x06000418 RID: 1048 RVA: 0x0001373B File Offset: 0x0001193B
		// (set) Token: 0x06000419 RID: 1049 RVA: 0x00013743 File Offset: 0x00011943
		[DataSourceProperty]
		public string MarkerType
		{
			get
			{
				return this._markerType;
			}
			set
			{
				if (value != this._markerType)
				{
					this._markerType = value;
					base.OnPropertyChangedWithValue<string>(value, "MarkerType");
				}
			}
		}

		// Token: 0x17000116 RID: 278
		// (get) Token: 0x0600041A RID: 1050 RVA: 0x00013766 File Offset: 0x00011966
		// (set) Token: 0x0600041B RID: 1051 RVA: 0x0001376E File Offset: 0x0001196E
		[DataSourceProperty]
		public Vec2 ScreenPosition
		{
			get
			{
				return this._screenPosition;
			}
			set
			{
				if (value != this._screenPosition)
				{
					this._screenPosition = value;
					base.OnPropertyChangedWithValue(value, "ScreenPosition");
				}
			}
		}

		// Token: 0x17000117 RID: 279
		// (get) Token: 0x0600041C RID: 1052 RVA: 0x00013791 File Offset: 0x00011991
		// (set) Token: 0x0600041D RID: 1053 RVA: 0x00013799 File Offset: 0x00011999
		[DataSourceProperty]
		public float HitPoints
		{
			get
			{
				return this._hitPoints;
			}
			set
			{
				if (value != this._hitPoints)
				{
					this._hitPoints = value;
					base.OnPropertyChangedWithValue(value, "HitPoints");
				}
			}
		}

		// Token: 0x17000118 RID: 280
		// (get) Token: 0x0600041E RID: 1054 RVA: 0x000137B7 File Offset: 0x000119B7
		// (set) Token: 0x0600041F RID: 1055 RVA: 0x000137BF File Offset: 0x000119BF
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

		// Token: 0x17000119 RID: 281
		// (get) Token: 0x06000420 RID: 1056 RVA: 0x000137DD File Offset: 0x000119DD
		// (set) Token: 0x06000421 RID: 1057 RVA: 0x000137E5 File Offset: 0x000119E5
		[DataSourceProperty]
		public bool HasAnyTroops
		{
			get
			{
				return this._hasAnyTroops;
			}
			set
			{
				if (value != this._hasAnyTroops)
				{
					this._hasAnyTroops = value;
					base.OnPropertyChangedWithValue(value, "HasAnyTroops");
				}
			}
		}

		// Token: 0x04000188 RID: 392
		public readonly Formation Formation;

		// Token: 0x04000189 RID: 393
		public readonly MissionShip Ship;

		// Token: 0x0400018A RID: 394
		private readonly string _formationType;

		// Token: 0x0400018B RID: 395
		private readonly string _shipType;

		// Token: 0x0400018C RID: 396
		private int _teamType;

		// Token: 0x0400018D RID: 397
		private bool _isEnabled;

		// Token: 0x0400018E RID: 398
		private bool _isCenterOfFocus;

		// Token: 0x0400018F RID: 399
		private bool _isShipTargetRelevant;

		// Token: 0x04000190 RID: 400
		private bool _isTargetingAShip;

		// Token: 0x04000191 RID: 401
		private bool _showDistanceTexts;

		// Token: 0x04000192 RID: 402
		private int _size;

		// Token: 0x04000193 RID: 403
		private int _wSign;

		// Token: 0x04000194 RID: 404
		private float _distance;

		// Token: 0x04000195 RID: 405
		private string _distanceText;

		// Token: 0x04000196 RID: 406
		private string _markerType;

		// Token: 0x04000197 RID: 407
		private Vec2 _screenPosition;

		// Token: 0x04000198 RID: 408
		private int _crewCount;

		// Token: 0x04000199 RID: 409
		private float _hitPoints;

		// Token: 0x0400019A RID: 410
		private float _maxHitPoints;

		// Token: 0x0400019B RID: 411
		private bool _hasAnyTroops;

		// Token: 0x02000075 RID: 117
		public enum TeamTypes
		{
			// Token: 0x04000235 RID: 565
			PlayerTeam,
			// Token: 0x04000236 RID: 566
			PlayerAllyTeam,
			// Token: 0x04000237 RID: 567
			EnemyTeam
		}
	}
}
