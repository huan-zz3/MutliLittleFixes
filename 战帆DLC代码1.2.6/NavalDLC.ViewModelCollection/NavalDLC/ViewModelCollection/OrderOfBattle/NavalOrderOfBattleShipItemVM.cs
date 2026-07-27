using System;
using System.Collections.Generic;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.ViewModelCollection.OrderOfBattle
{
	// Token: 0x02000025 RID: 37
	public class NavalOrderOfBattleShipItemVM : ViewModel
	{
		// Token: 0x060002F4 RID: 756 RVA: 0x0000F700 File Offset: 0x0000D900
		public NavalOrderOfBattleShipItemVM(IShipOrigin shipOrigin, Action<NavalOrderOfBattleShipItemVM, bool> onSelected, Func<NavalOrderOfBattleShipItemVM, NavalOrderOfBattleFormationItemVM> findFormationOfShip)
		{
			this._onSelected = onSelected;
			this._findFormationOfShip = findFormationOfShip;
			this.ShipOrigin = shipOrigin;
			this.PrefabId = NavalUIHelper.GetPrefabIdOfShipHull(shipOrigin.Hull);
			Ship ship;
			this.IsFlagship = (ship = this.ShipOrigin as Ship) != null && ship == NavalUIHelper.GetFlagship(ship.Owner);
			this.Tooltip = new BasicTooltipViewModel(() => this._cachedTooltipProperties);
			this.RefreshValues();
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x0000F77C File Offset: 0x0000D97C
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ShipName = this.ShipOrigin.Name.ToString();
			if (this.MissionShip != null)
			{
				this.HealthRatio = this.MissionShip.HitPoints / this.MissionShip.MaxHealth;
				this.MainDeckCrewCount = this.MissionShip.Formation.CountOfUnits;
				this.ReserveCrewCount = Mission.Current.GetMissionBehavior<NavalAgentsLogic>().GetReservedTroopsCountOfShip(this.MissionShip);
				this.MainDeckCrewCapacity = this.MissionShip.CrewSizeOnMainDeck;
				this.MainDeckCrewRatio = (float)this.MainDeckCrewCount / (float)(this.MainDeckCrewCapacity + this.ReserveCrewCount);
				this.TotalCrewRatio = (float)(this.MainDeckCrewCount + this.ReserveCrewCount) / (float)(this.MainDeckCrewCapacity + this.ReserveCrewCount);
			}
			else
			{
				this.HealthRatio = this.ShipOrigin.HitPoints / this.ShipOrigin.MaxHitPoints;
				this.MainDeckCrewCount = 0;
				this.ReserveCrewCount = 0;
				this.MainDeckCrewCapacity = this.ShipOrigin.MainDeckCrewCapacity;
				this.MainDeckCrewRatio = 0f;
				this.TotalCrewRatio = 0f;
			}
			this.HealthPercentageAsString = new TextObject("{=gYATKZJp}{NUMBER}%", null).SetTextVariable("NUMBER", ((int)(this.HealthRatio * 100f)).ToString()).ToString();
			this.CrewCountAsString = GameTexts.FindText("str_LEFT_over_RIGHT_no_space", null).SetTextVariable("LEFT", this.MainDeckCrewCount).SetTextVariable("RIGHT", this.MainDeckCrewCapacity)
				.ToString();
			if (this.ReserveCrewCount > 0)
			{
				string text = GameTexts.FindText("str_plus_with_number", null).SetTextVariable("NUMBER", this.ReserveCrewCount).ToString();
				this.ReserveCrewCountAsString = GameTexts.FindText("str_STR_in_parentheses", null).SetTextVariable("STR", text).ToString();
			}
			else
			{
				this.ReserveCrewCountAsString = string.Empty;
			}
			this._cachedTooltipProperties = this.GetTooltip();
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x0000F971 File Offset: 0x0000DB71
		public void ExecuteSelect()
		{
			if (!this.IsDisabled)
			{
				Action<NavalOrderOfBattleShipItemVM, bool> onSelected = this._onSelected;
				if (onSelected == null)
				{
					return;
				}
				onSelected(this, true);
			}
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x0000F98D File Offset: 0x0000DB8D
		public void ExecuteToggleSelect()
		{
			if (!this.IsDisabled)
			{
				Action<NavalOrderOfBattleShipItemVM, bool> onSelected = this._onSelected;
				if (onSelected == null)
				{
					return;
				}
				onSelected(this, !this.IsSelected);
			}
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x0000F9B1 File Offset: 0x0000DBB1
		public void ExecuteDeselect()
		{
			if (!this.IsDisabled)
			{
				Action<NavalOrderOfBattleShipItemVM, bool> onSelected = this._onSelected;
				if (onSelected == null)
				{
					return;
				}
				onSelected(this, false);
			}
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x0000F9D0 File Offset: 0x0000DBD0
		private List<TooltipProperty> GetTooltip()
		{
			List<TooltipProperty> list = new List<TooltipProperty>
			{
				new TooltipProperty(this.ShipName, string.Empty, 0, false, 4096)
			};
			if (this.IsDisabled)
			{
				list.Add(new TooltipProperty(string.Empty, new TextObject("{=cIpPMkry}You can only change your formation's ship when you are not the general.", null).ToString(), 0, false, 0));
				list.Add(new TooltipProperty(string.Empty, string.Empty, 0, false, 0));
			}
			Ship ship;
			if ((ship = this.ShipOrigin as Ship) != null)
			{
				list.Add(new TooltipProperty(GameTexts.FindText("str_owner", null).ToString(), ship.Owner.Name.ToString(), 0, false, 0));
				list.Add(new TooltipProperty(new TextObject("{=wEmx6fZi}Hull", null).ToString(), ship.ShipHull.Name.ToString(), 0, false, 0));
			}
			list.Add(new TooltipProperty(new TextObject("{=sqdzHOPe}Class", null).ToString(), GameTexts.FindText("str_ship_type", this.ShipOrigin.Hull.Type.ToString().ToLowerInvariant()).ToString(), 0, false, 0));
			if (this.MissionShip == null)
			{
				string text = GameTexts.FindText("str_LEFT_over_RIGHT_no_space", null).SetTextVariable("LEFT", (int)this.ShipOrigin.HitPoints).SetTextVariable("RIGHT", (int)this.ShipOrigin.MaxHitPoints)
					.ToString();
				list.Add(new TooltipProperty(new TextObject("{=oBbiVeKE}Hit Points", null).ToString(), text, 0, false, 0));
				list.Add(new TooltipProperty(new TextObject("{=TrbfOCyF}Main Deck Crew Capacity", null).ToString(), this.ShipOrigin.MainDeckCrewCapacity.ToString(), 0, false, 0));
				int num = this.ShipOrigin.TotalCrewCapacity - this.ShipOrigin.MainDeckCrewCapacity;
				if (num > 0)
				{
					list.Add(new TooltipProperty(new TextObject("{=saS6Sub2}Reserve Crew Capacity", null).ToString(), num.ToString(), 0, false, 0));
				}
			}
			else
			{
				string text2 = GameTexts.FindText("str_LEFT_over_RIGHT_no_space", null).SetTextVariable("LEFT", (int)this.MissionShip.HitPoints).SetTextVariable("RIGHT", (int)this.MissionShip.MaxHealth)
					.ToString();
				list.Add(new TooltipProperty(new TextObject("{=oBbiVeKE}Hit Points", null).ToString(), text2, 0, false, 0));
				list.Add(new TooltipProperty(new TextObject("{=LfOIa8eh}Troops On Deck", null).ToString(), this.CrewCountAsString, 0, false, 0));
				if (this.ReserveCrewCount > 0)
				{
					string text3 = GameTexts.FindText("str_LEFT_over_RIGHT_no_space", null).SetTextVariable("LEFT", this.ReserveCrewCount).SetTextVariable("RIGHT", this.MissionShip.CrewSizeOnLowerDeck)
						.ToString();
					list.Add(new TooltipProperty(new TextObject("{=25fleLuY}Troops In Reserve", null).ToString(), text3, 0, false, 0));
				}
			}
			List<ShipSlotAndPieceName> shipSlotAndPieceNames = this.ShipOrigin.GetShipSlotAndPieceNames();
			if (shipSlotAndPieceNames.Count > 0)
			{
				list.Add(new TooltipProperty(string.Empty, string.Empty, 0, false, 1024)
				{
					OnlyShowWhenExtended = true
				});
				list.Add(new TooltipProperty(string.Empty, new TextObject("{=zMvUzdKR}Ship Upgrades", null).ToString(), -1, false, 0)
				{
					OnlyShowWhenExtended = true
				});
				foreach (ShipSlotAndPieceName shipSlotAndPieceName in shipSlotAndPieceNames)
				{
					list.Add(new TooltipProperty(shipSlotAndPieceName.SlotName, shipSlotAndPieceName.PieceName, 0, false, 0)
					{
						OnlyShowWhenExtended = true
					});
				}
			}
			if (shipSlotAndPieceNames.Count > 0)
			{
				if (Input.IsGamepadActive)
				{
					GameTexts.SetVariable("EXTEND_KEY", GameKeyTextExtensions.GetHotKeyGameText(Game.Current.GameTextManager, "MapHotKeyCategory", "MapFollowModifier").ToString());
				}
				else
				{
					GameTexts.SetVariable("EXTEND_KEY", Game.Current.GameTextManager.FindText("str_game_key_text", "anyalt").ToString());
				}
				list.Add(new TooltipProperty(string.Empty, string.Empty, 0, false, 0)
				{
					OnlyShowWhenNotExtended = true
				});
				list.Add(new TooltipProperty(string.Empty, GameTexts.FindText("str_map_tooltip_info", null).ToString(), -1, false, 0)
				{
					OnlyShowWhenNotExtended = true
				});
			}
			return list;
		}

		// Token: 0x060002FA RID: 762 RVA: 0x0000FE3C File Offset: 0x0000E03C
		public bool GetCanBeUnassignedOrMoved()
		{
			if (this.IsDisabled)
			{
				return false;
			}
			Func<NavalOrderOfBattleShipItemVM, NavalOrderOfBattleFormationItemVM> findFormationOfShip = this._findFormationOfShip;
			if (findFormationOfShip == null)
			{
				return true;
			}
			NavalOrderOfBattleFormationItemVM navalOrderOfBattleFormationItemVM = findFormationOfShip(this);
			bool? flag;
			if (navalOrderOfBattleFormationItemVM == null)
			{
				flag = null;
			}
			else
			{
				NavalOrderOfBattleHeroItemVM captain = navalOrderOfBattleFormationItemVM.Captain;
				flag = ((captain != null) ? new bool?(captain.IsMainHero) : null);
			}
			bool? flag2 = flag;
			bool flag3 = true;
			return !((flag2.GetValueOrDefault() == flag3) & (flag2 != null));
		}

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x060002FB RID: 763 RVA: 0x0000FEAA File Offset: 0x0000E0AA
		// (set) Token: 0x060002FC RID: 764 RVA: 0x0000FEB2 File Offset: 0x0000E0B2
		[DataSourceProperty]
		public bool IsDisabled
		{
			get
			{
				return this._isDisabled;
			}
			set
			{
				if (value != this._isDisabled)
				{
					this._isDisabled = value;
					base.OnPropertyChangedWithValue(value, "IsDisabled");
				}
			}
		}

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x060002FD RID: 765 RVA: 0x0000FED0 File Offset: 0x0000E0D0
		// (set) Token: 0x060002FE RID: 766 RVA: 0x0000FED8 File Offset: 0x0000E0D8
		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x060002FF RID: 767 RVA: 0x0000FEF6 File Offset: 0x0000E0F6
		// (set) Token: 0x06000300 RID: 768 RVA: 0x0000FEFE File Offset: 0x0000E0FE
		[DataSourceProperty]
		public bool IsFlagship
		{
			get
			{
				return this._isFlagship;
			}
			set
			{
				if (value != this._isFlagship)
				{
					this._isFlagship = value;
					base.OnPropertyChangedWithValue(value, "IsFlagship");
				}
			}
		}

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000301 RID: 769 RVA: 0x0000FF1C File Offset: 0x0000E11C
		// (set) Token: 0x06000302 RID: 770 RVA: 0x0000FF24 File Offset: 0x0000E124
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

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000303 RID: 771 RVA: 0x0000FF47 File Offset: 0x0000E147
		// (set) Token: 0x06000304 RID: 772 RVA: 0x0000FF4F File Offset: 0x0000E14F
		[DataSourceProperty]
		public string ShipName
		{
			get
			{
				return this._shipName;
			}
			set
			{
				if (value != this._shipName)
				{
					this._shipName = value;
					base.OnPropertyChangedWithValue<string>(value, "ShipName");
				}
			}
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000305 RID: 773 RVA: 0x0000FF72 File Offset: 0x0000E172
		// (set) Token: 0x06000306 RID: 774 RVA: 0x0000FF7A File Offset: 0x0000E17A
		[DataSourceProperty]
		public float HealthRatio
		{
			get
			{
				return this._healthRatio;
			}
			set
			{
				if (value != this._healthRatio)
				{
					this._healthRatio = value;
					base.OnPropertyChangedWithValue(value, "HealthRatio");
				}
			}
		}

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000307 RID: 775 RVA: 0x0000FF98 File Offset: 0x0000E198
		// (set) Token: 0x06000308 RID: 776 RVA: 0x0000FFA0 File Offset: 0x0000E1A0
		[DataSourceProperty]
		public string HealthPercentageAsString
		{
			get
			{
				return this._healthPercentageAsString;
			}
			set
			{
				if (value != this._healthPercentageAsString)
				{
					this._healthPercentageAsString = value;
					base.OnPropertyChangedWithValue<string>(value, "HealthPercentageAsString");
				}
			}
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000309 RID: 777 RVA: 0x0000FFC3 File Offset: 0x0000E1C3
		// (set) Token: 0x0600030A RID: 778 RVA: 0x0000FFCB File Offset: 0x0000E1CB
		[DataSourceProperty]
		public int MainDeckCrewCount
		{
			get
			{
				return this._mainDeckCrewCount;
			}
			set
			{
				if (value != this._mainDeckCrewCount)
				{
					this._mainDeckCrewCount = value;
					base.OnPropertyChangedWithValue(value, "MainDeckCrewCount");
				}
			}
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x0600030B RID: 779 RVA: 0x0000FFE9 File Offset: 0x0000E1E9
		// (set) Token: 0x0600030C RID: 780 RVA: 0x0000FFF1 File Offset: 0x0000E1F1
		[DataSourceProperty]
		public int ReserveCrewCount
		{
			get
			{
				return this._reserveCrewCount;
			}
			set
			{
				if (value != this._reserveCrewCount)
				{
					this._reserveCrewCount = value;
					base.OnPropertyChangedWithValue(value, "ReserveCrewCount");
				}
			}
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x0600030D RID: 781 RVA: 0x0001000F File Offset: 0x0000E20F
		// (set) Token: 0x0600030E RID: 782 RVA: 0x00010017 File Offset: 0x0000E217
		[DataSourceProperty]
		public int MainDeckCrewCapacity
		{
			get
			{
				return this._mainDeckCrewCapacity;
			}
			set
			{
				if (value != this._mainDeckCrewCapacity)
				{
					this._mainDeckCrewCapacity = value;
					base.OnPropertyChangedWithValue(value, "MainDeckCrewCapacity");
				}
			}
		}

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x0600030F RID: 783 RVA: 0x00010035 File Offset: 0x0000E235
		// (set) Token: 0x06000310 RID: 784 RVA: 0x0001003D File Offset: 0x0000E23D
		[DataSourceProperty]
		public string CrewCountAsString
		{
			get
			{
				return this._crewCountAsString;
			}
			set
			{
				if (value != this._crewCountAsString)
				{
					this._crewCountAsString = value;
					base.OnPropertyChangedWithValue<string>(value, "CrewCountAsString");
				}
			}
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x06000311 RID: 785 RVA: 0x00010060 File Offset: 0x0000E260
		// (set) Token: 0x06000312 RID: 786 RVA: 0x00010068 File Offset: 0x0000E268
		[DataSourceProperty]
		public string ReserveCrewCountAsString
		{
			get
			{
				return this._reserveCrewCountAsString;
			}
			set
			{
				if (value != this._reserveCrewCountAsString)
				{
					this._reserveCrewCountAsString = value;
					base.OnPropertyChangedWithValue<string>(value, "ReserveCrewCountAsString");
				}
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06000313 RID: 787 RVA: 0x0001008B File Offset: 0x0000E28B
		// (set) Token: 0x06000314 RID: 788 RVA: 0x00010093 File Offset: 0x0000E293
		[DataSourceProperty]
		public float MainDeckCrewRatio
		{
			get
			{
				return this._mainDeckCrewRatio;
			}
			set
			{
				if (value != this._mainDeckCrewRatio)
				{
					this._mainDeckCrewRatio = value;
					base.OnPropertyChangedWithValue(value, "MainDeckCrewRatio");
				}
			}
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06000315 RID: 789 RVA: 0x000100B1 File Offset: 0x0000E2B1
		// (set) Token: 0x06000316 RID: 790 RVA: 0x000100B9 File Offset: 0x0000E2B9
		[DataSourceProperty]
		public float TotalCrewRatio
		{
			get
			{
				return this._totalCrewRatio;
			}
			set
			{
				if (value != this._totalCrewRatio)
				{
					this._totalCrewRatio = value;
					base.OnPropertyChangedWithValue(value, "TotalCrewRatio");
				}
			}
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x06000317 RID: 791 RVA: 0x000100D7 File Offset: 0x0000E2D7
		// (set) Token: 0x06000318 RID: 792 RVA: 0x000100DF File Offset: 0x0000E2DF
		[DataSourceProperty]
		public BasicTooltipViewModel Tooltip
		{
			get
			{
				return this._tooltip;
			}
			set
			{
				if (value != this._tooltip)
				{
					this._tooltip = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "Tooltip");
				}
			}
		}

		// Token: 0x04000114 RID: 276
		public readonly IShipOrigin ShipOrigin;

		// Token: 0x04000115 RID: 277
		public MissionShip MissionShip;

		// Token: 0x04000116 RID: 278
		private readonly Action<NavalOrderOfBattleShipItemVM, bool> _onSelected;

		// Token: 0x04000117 RID: 279
		private readonly Func<NavalOrderOfBattleShipItemVM, NavalOrderOfBattleFormationItemVM> _findFormationOfShip;

		// Token: 0x04000118 RID: 280
		private List<TooltipProperty> _cachedTooltipProperties;

		// Token: 0x04000119 RID: 281
		private bool _isDisabled;

		// Token: 0x0400011A RID: 282
		private bool _isSelected;

		// Token: 0x0400011B RID: 283
		private bool _isFlagship;

		// Token: 0x0400011C RID: 284
		private string _prefabId;

		// Token: 0x0400011D RID: 285
		private string _shipName;

		// Token: 0x0400011E RID: 286
		private float _healthRatio;

		// Token: 0x0400011F RID: 287
		private string _healthPercentageAsString;

		// Token: 0x04000120 RID: 288
		private int _mainDeckCrewCount;

		// Token: 0x04000121 RID: 289
		private int _reserveCrewCount;

		// Token: 0x04000122 RID: 290
		private int _mainDeckCrewCapacity;

		// Token: 0x04000123 RID: 291
		private string _crewCountAsString;

		// Token: 0x04000124 RID: 292
		private string _reserveCrewCountAsString;

		// Token: 0x04000125 RID: 293
		private float _mainDeckCrewRatio;

		// Token: 0x04000126 RID: 294
		private float _totalCrewRatio;

		// Token: 0x04000127 RID: 295
		private BasicTooltipViewModel _tooltip;
	}
}
