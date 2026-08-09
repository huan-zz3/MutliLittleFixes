using System;
using NavalDLC.Missions;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD.FormationMarker;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order;

namespace NavalDLC.ViewModelCollection.Order
{
	// Token: 0x02000022 RID: 34
	public class NavalOrderTroopItemVM : OrderTroopItemVM
	{
		// Token: 0x0600027E RID: 638 RVA: 0x0000DC0C File Offset: 0x0000BE0C
		public NavalOrderTroopItemVM(Formation formation, Action<OrderTroopItemVM> setSelected, Func<Formation, int> getMorale)
			: base(formation, setSelected, getMorale)
		{
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this._troopCountTextObj = GameTexts.FindText("str_LEFT_over_RIGHT_no_space", null);
			this._healthTextObj = GameTexts.FindText("str_NUMBER_percent", null);
			this.UpdateVisuals();
		}

		// Token: 0x0600027F RID: 639 RVA: 0x0000DC64 File Offset: 0x0000BE64
		public override void UpdateVisuals()
		{
			base.UpdateVisuals();
			MissionShip missionShip;
			if (this.Formation != null && this._navalShipsLogic != null && this._navalShipsLogic.GetShip(0, this.Formation.FormationIndex, out missionShip))
			{
				if (string.IsNullOrEmpty(this.PrefabId) || missionShip != this._cachedShip)
				{
					this._cachedShip = missionShip;
					this.HasShip = this._cachedShip != null;
					MissionShip cachedShip = this._cachedShip;
					this.IsShipActive = cachedShip != null && cachedShip.HitPoints > 0f;
					this.PrefabId = ((this._cachedShip != null) ? NavalUIHelper.GetPrefabIdOfShipHull(this._cachedShip.ShipOrigin.Hull) : null);
					return;
				}
			}
			else
			{
				this.PrefabId = null;
				this.HasShip = false;
				this._cachedShip = null;
				this.IsShipActive = false;
			}
		}

		// Token: 0x06000280 RID: 640 RVA: 0x0000DD38 File Offset: 0x0000BF38
		public override void Update()
		{
			base.Update();
			MissionShip cachedShip = this._cachedShip;
			this.IsShipActive = cachedShip != null && cachedShip.HitPoints > 0f;
			if (this.IsShipActive)
			{
				this.TroopCountText = this._troopCountTextObj.SetTextVariable("LEFT", this.Formation.CountOfUnits.ToString()).SetTextVariable("RIGHT", this._cachedShip.CrewSizeOnMainDeck.ToString()).ToString();
				this.HealthText = this._healthTextObj.SetTextVariable("NUMBER", ((int)(this._cachedShip.HitPoints / this._cachedShip.MaxHealth * 100f)).ToString()).ToString();
				return;
			}
			this.TroopCountText = this.Formation.CountOfUnits.ToString();
			this.HealthText = this._healthTextObj.SetTextVariable("NUMBER", 0).ToString();
		}

		// Token: 0x06000281 RID: 641 RVA: 0x0000DE38 File Offset: 0x0000C038
		public override void RefreshTargetedOrderVisual()
		{
			if (!this.IsShipActive)
			{
				base.RefreshTargetedOrderVisual();
				return;
			}
			bool flag = false;
			string text = null;
			string text2 = null;
			if (this._cachedShip.ShipOrder.MovementOrderEnum == ShipOrder.ShipMovementOrderEnum.Engage && this._cachedShip.ShipOrder.TargetShip != null)
			{
				flag = true;
				text2 = "Ship_" + this._cachedShip.ShipOrder.TargetShip.ShipOrigin.Hull.Type.ToString();
				text = "order_movement_advance";
			}
			if (!flag)
			{
				for (int i = 0; i < base.ActiveOrders.Count; i++)
				{
					OrderItemVM orderItemVM = base.ActiveOrders[i];
					if (orderItemVM.Order.IsTargeted())
					{
						Formation targetFormation = this.Formation.TargetFormation;
						if (targetFormation != null)
						{
							MissionShip missionShip;
							this._navalShipsLogic.GetShip(targetFormation, out missionShip);
							if (missionShip != null)
							{
								text2 = "Ship_" + missionShip.ShipOrigin.Hull.Type.ToString();
							}
							else
							{
								text2 = MissionFormationMarkerTargetVM.GetFormationType(targetFormation.PhysicalClass);
							}
							flag = true;
						}
						text = orderItemVM.OrderIconId;
					}
				}
			}
			base.HasTarget = flag;
			base.CurrentOrderIconId = text;
			base.CurrentTargetFormationType = text2;
		}

		// Token: 0x06000282 RID: 642 RVA: 0x0000DF7F File Offset: 0x0000C17F
		public void UpdateClassData(DeploymentFormationClass formationClass)
		{
			this.FormationClassInt = formationClass;
		}

		// Token: 0x06000283 RID: 643 RVA: 0x0000DF88 File Offset: 0x0000C188
		public override TextObject GetVisibleNameOfFormationForMessage()
		{
			if (this.IsShipActive)
			{
				return this._cachedShip.ShipOrigin.Name;
			}
			return base.GetVisibleNameOfFormationForMessage();
		}

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x06000284 RID: 644 RVA: 0x0000DFA9 File Offset: 0x0000C1A9
		// (set) Token: 0x06000285 RID: 645 RVA: 0x0000DFB1 File Offset: 0x0000C1B1
		[DataSourceProperty]
		public string TroopCountText
		{
			get
			{
				return this._troopCountText;
			}
			set
			{
				if (value != this._troopCountText)
				{
					this._troopCountText = value;
					base.OnPropertyChangedWithValue<string>(value, "TroopCountText");
				}
			}
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000286 RID: 646 RVA: 0x0000DFD4 File Offset: 0x0000C1D4
		// (set) Token: 0x06000287 RID: 647 RVA: 0x0000DFDC File Offset: 0x0000C1DC
		[DataSourceProperty]
		public string HealthText
		{
			get
			{
				return this._healthText;
			}
			set
			{
				if (value != this._healthText)
				{
					this._healthText = value;
					base.OnPropertyChangedWithValue<string>(value, "HealthText");
				}
			}
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000288 RID: 648 RVA: 0x0000DFFF File Offset: 0x0000C1FF
		// (set) Token: 0x06000289 RID: 649 RVA: 0x0000E007 File Offset: 0x0000C207
		[DataSourceProperty]
		public int FormationClassInt
		{
			get
			{
				return this._formationClassInt;
			}
			set
			{
				if (value != this._formationClassInt)
				{
					this._formationClassInt = value;
					base.OnPropertyChangedWithValue(value, "FormationClassInt");
				}
			}
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x0600028A RID: 650 RVA: 0x0000E025 File Offset: 0x0000C225
		// (set) Token: 0x0600028B RID: 651 RVA: 0x0000E02D File Offset: 0x0000C22D
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

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x0600028C RID: 652 RVA: 0x0000E050 File Offset: 0x0000C250
		// (set) Token: 0x0600028D RID: 653 RVA: 0x0000E058 File Offset: 0x0000C258
		[DataSourceProperty]
		public bool HasShip
		{
			get
			{
				return this._hasShip;
			}
			set
			{
				if (value != this._hasShip)
				{
					this._hasShip = value;
					base.OnPropertyChangedWithValue(value, "HasShip");
				}
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x0600028E RID: 654 RVA: 0x0000E076 File Offset: 0x0000C276
		// (set) Token: 0x0600028F RID: 655 RVA: 0x0000E07E File Offset: 0x0000C27E
		[DataSourceProperty]
		public bool IsShipActive
		{
			get
			{
				return this._isShipActive;
			}
			set
			{
				if (value != this._isShipActive)
				{
					this._isShipActive = value;
					base.OnPropertyChangedWithValue(value, "IsShipActive");
				}
			}
		}

		// Token: 0x040000CE RID: 206
		private readonly NavalShipsLogic _navalShipsLogic;

		// Token: 0x040000CF RID: 207
		private readonly TextObject _troopCountTextObj;

		// Token: 0x040000D0 RID: 208
		private readonly TextObject _healthTextObj;

		// Token: 0x040000D1 RID: 209
		private MissionShip _cachedShip;

		// Token: 0x040000D2 RID: 210
		private string _troopCountText;

		// Token: 0x040000D3 RID: 211
		private string _healthText;

		// Token: 0x040000D4 RID: 212
		private int _formationClassInt = 5;

		// Token: 0x040000D5 RID: 213
		private string _prefabId;

		// Token: 0x040000D6 RID: 214
		private bool _hasShip;

		// Token: 0x040000D7 RID: 215
		private bool _isShipActive;
	}
}
