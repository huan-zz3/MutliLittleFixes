using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.CustomBattle.CustomBattle.SelectionItem
{
	// Token: 0x02000023 RID: 35
	public class NavalCustomBattleShipItemVM : NavalCustomBattleShipHullItemVM
	{
		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000206 RID: 518 RVA: 0x00009387 File Offset: 0x00007587
		// (set) Token: 0x06000207 RID: 519 RVA: 0x0000938F File Offset: 0x0000758F
		public CustomBattleShip Ship { get; private set; }

		// Token: 0x06000208 RID: 520 RVA: 0x00009398 File Offset: 0x00007598
		public NavalCustomBattleShipItemVM(ShipHull shipHull, bool isPlayerShip, Action onUpgraded)
			: base(shipHull, null, null)
		{
			this.Ship = new CustomBattleShip(this.ShipHull, isPlayerShip);
			this._onUpgraded = onUpgraded;
			this.CycleTierHint = new HintViewModel(new TextObject("{=zbkzFaWE}Change upgrade tier", null), null);
		}

		// Token: 0x06000209 RID: 521 RVA: 0x000093D3 File Offset: 0x000075D3
		public void ExecuteCycleUpgradeTier()
		{
			this.Tier = (this.Tier + 1) % 4;
		}

		// Token: 0x0600020A RID: 522 RVA: 0x000093E5 File Offset: 0x000075E5
		public void RandomizeUpgrades()
		{
			this.Tier = MBRandom.RandomInt(0, 4);
		}

		// Token: 0x0600020B RID: 523 RVA: 0x000093F4 File Offset: 0x000075F4
		private void OnTierSelection()
		{
			if (this.Tier == 0)
			{
				using (Dictionary<string, ShipSlot>.Enumerator enumerator = this.ShipHull.AvailableSlots.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, ShipSlot> keyValuePair = enumerator.Current;
						this.Ship.SetPieceAtSlot(keyValuePair.Key, null);
					}
					goto IL_0134;
				}
			}
			IEnumerable<ShipUpgradePiece> enumerable = from x in MBObjectManager.Instance.GetObjectTypeList<ShipUpgradePiece>()
				where !x.NotMerchandise
				select x;
			IEnumerable<ShipUpgradePiece> enumerable2 = enumerable.Where<ShipUpgradePiece>((ShipUpgradePiece x) => x.RequiredPortLevel == this.Tier);
			using (Dictionary<string, ShipSlot>.Enumerator enumerator = this.ShipHull.AvailableSlots.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, ShipSlot> slot = enumerator.Current;
					ShipUpgradePiece shipUpgradePiece;
					if (enumerable2.Count<ShipUpgradePiece>() == 0)
					{
						shipUpgradePiece = Extensions.GetRandomElementInefficiently<ShipUpgradePiece>(enumerable.Where<ShipUpgradePiece>((ShipUpgradePiece x) => x.RequiredPortLevel <= this.Tier && x.DoesPieceMatchSlot(slot.Value)));
					}
					else
					{
						shipUpgradePiece = Extensions.GetRandomElementInefficiently<ShipUpgradePiece>(enumerable2.Where<ShipUpgradePiece>((ShipUpgradePiece x) => x.DoesPieceMatchSlot(slot.Value)));
					}
					this.Ship.SetPieceAtSlot(slot.Key, shipUpgradePiece);
				}
			}
			IL_0134:
			Action onUpgraded = this._onUpgraded;
			if (onUpgraded == null)
			{
				return;
			}
			onUpgraded();
		}

		// Token: 0x0600020C RID: 524 RVA: 0x00009564 File Offset: 0x00007764
		protected override List<TooltipProperty> GetTooltip()
		{
			List<TooltipProperty> list = new List<TooltipProperty>
			{
				new TooltipProperty(base.Name.ToString(), string.Empty, 0, false, 4096),
				new TooltipProperty(new TextObject("{=sqdzHOPe}Class", null).ToString(), GameTexts.FindText("str_ship_type", this.ShipHull.Type.ToString().ToLowerInvariant()).ToString(), 0, false, 0),
				new TooltipProperty(new TextObject("{=UbZL2BJQ}Hitpoints", null).ToString(), ((int)this.Ship.MaxHitPoints).ToString(), 0, false, 0)
			};
			int num = this.Ship.TotalCrewCapacity - this.Ship.MainDeckCrewCapacity;
			string text;
			if (num > 0)
			{
				text = new TextObject("{=r2fvxfwZ}{TOTAL} ({MAIN_DECK}+{RESERVE})", null).SetTextVariable("TOTAL", this.Ship.TotalCrewCapacity.ToString()).SetTextVariable("MAIN_DECK", this.Ship.MainDeckCrewCapacity.ToString()).SetTextVariable("RESERVE", num.ToString())
					.ToString();
			}
			else
			{
				text = this.Ship.TotalCrewCapacity.ToString();
			}
			list.Add(new TooltipProperty(new TextObject("{=oqVVGxgb}Crew Capacity", null).ToString(), text, 0, false, 0));
			List<ShipSlotAndPieceName> shipSlotAndPieceNames = this.Ship.GetShipSlotAndPieceNames();
			if (shipSlotAndPieceNames.Count > 0)
			{
				list.Add(new TooltipProperty(string.Empty, string.Empty, 0, false, 1024));
				list.Add(new TooltipProperty(string.Empty, new TextObject("{=zMvUzdKR}Ship Upgrades", null).ToString(), -1, false, 0));
				foreach (ShipSlotAndPieceName shipSlotAndPieceName in shipSlotAndPieceNames)
				{
					list.Add(new TooltipProperty(shipSlotAndPieceName.SlotName, shipSlotAndPieceName.PieceName, 0, false, 0));
				}
			}
			return list;
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x0600020D RID: 525 RVA: 0x00009778 File Offset: 0x00007978
		// (set) Token: 0x0600020E RID: 526 RVA: 0x00009780 File Offset: 0x00007980
		[DataSourceProperty]
		public int Tier
		{
			get
			{
				return this._tier;
			}
			set
			{
				if (value != this._tier)
				{
					this._tier = value;
					base.OnPropertyChangedWithValue(value, "Tier");
					this.OnTierSelection();
				}
			}
		}

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x0600020F RID: 527 RVA: 0x000097A4 File Offset: 0x000079A4
		// (set) Token: 0x06000210 RID: 528 RVA: 0x000097AC File Offset: 0x000079AC
		[DataSourceProperty]
		public HintViewModel CycleTierHint
		{
			get
			{
				return this._cycleTierHint;
			}
			set
			{
				if (value != this._cycleTierHint)
				{
					this._cycleTierHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "CycleTierHint");
				}
			}
		}

		// Token: 0x040000FA RID: 250
		private readonly Action _onUpgraded;

		// Token: 0x040000FB RID: 251
		private int _tier;

		// Token: 0x040000FC RID: 252
		private HintViewModel _cycleTierHint;
	}
}
