using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.CustomBattle.CustomBattle.SelectionItem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace NavalDLC.CustomBattle.CustomBattle
{
	// Token: 0x02000015 RID: 21
	public class NavalCustomBattleShipSelectionGroupVM : ViewModel
	{
		// Token: 0x060000F9 RID: 249 RVA: 0x000064C4 File Offset: 0x000046C4
		public NavalCustomBattleShipSelectionGroupVM(bool isPlayerSide, NavalCustomBattleShipSelectionPopUpVM shipSelectionPopUp, Action onShipSelectedOrUpgraded, Action<NavalCustomBattleShipItemVM> onShipFocused)
		{
			this._onShipSelectedOrUpgraded = onShipSelectedOrUpgraded;
			this.ShipSelectionItems = new MBBindingList<NavalCustomBattleShipSelectionItemVM>();
			for (int i = 0; i < 8; i++)
			{
				this.ShipSelectionItems.Add(new NavalCustomBattleShipSelectionItemVM(isPlayerSide, shipSelectionPopUp, new Action(this.OnShipSelectedOrUpgraded), onShipFocused));
			}
			this.ShipSelectionItems[0].SelectedItem = new NavalCustomBattleShipItemVM(NavalCustomBattleData.ShipHulls.ElementAt<ShipHull>(0), isPlayerSide, new Action(this.OnShipSelectedOrUpgraded));
			this.UpdateCanShipsBecomeEmpty();
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00006549 File Offset: 0x00004749
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ShipSelectionItems.ApplyActionOnAllItems(delegate(NavalCustomBattleShipSelectionItemVM x)
			{
				x.RefreshValues();
			});
		}

		// Token: 0x060000FB RID: 251 RVA: 0x0000657C File Offset: 0x0000477C
		public void ExecuteRandomize(int targetDeckSize)
		{
			List<ShipHull> list = new List<ShipHull>();
			int num = int.MaxValue;
			for (int i = 0; i < 20; i++)
			{
				int num2;
				List<ShipHull> list2 = this.CreateRandomFleet(targetDeckSize, out num2);
				int num3 = Math.Abs(targetDeckSize - num2);
				if (num3 < num)
				{
					num = num3;
					list = list2;
					if (num3 == 0)
					{
						break;
					}
				}
			}
			for (int j = 0; j < this.ShipSelectionItems.Count; j++)
			{
				this.ShipSelectionItems[j].SetHull(list.ElementAtOrDefault<ShipHull>(j));
				NavalCustomBattleShipItemVM selectedItem = this.ShipSelectionItems[j].SelectedItem;
				if (selectedItem != null)
				{
					selectedItem.RandomizeUpgrades();
				}
			}
		}

		// Token: 0x060000FC RID: 252 RVA: 0x00006618 File Offset: 0x00004818
		private List<ShipHull> CreateRandomFleet(int targetDeckSize, out int deckSize)
		{
			List<ShipHull> list = new List<ShipHull>();
			deckSize = 0;
			int num = 0;
			for (;;)
			{
				if (num >= this.ShipSelectionItems.Count<NavalCustomBattleShipSelectionItemVM>((NavalCustomBattleShipSelectionItemVM x) => x.IsRelevant))
				{
					break;
				}
				ShipHull shipHull;
				if (!this.IsRaid)
				{
					shipHull = Extensions.GetRandomElementInefficiently<ShipHull>(NavalCustomBattleData.ShipHulls);
				}
				else
				{
					shipHull = Extensions.GetRandomElementWithPredicate<ShipHull>(NavalCustomBattleData.ShipHulls.ToArray<ShipHull>(), (ShipHull x) => NavalCustomBattleHelper.CanShipHullBeUsedInRaid(x));
				}
				ShipHull shipHull2 = shipHull;
				list.Add(shipHull2);
				deckSize += shipHull2.MainDeckCrewCapacity;
				if (deckSize >= targetDeckSize)
				{
					break;
				}
				num++;
			}
			return list;
		}

		// Token: 0x060000FD RID: 253 RVA: 0x000066C4 File Offset: 0x000048C4
		public List<IShipOrigin> GetSelectedShips()
		{
			List<IShipOrigin> list = new List<IShipOrigin>();
			foreach (NavalCustomBattleShipSelectionItemVM navalCustomBattleShipSelectionItemVM in this.ShipSelectionItems)
			{
				if (navalCustomBattleShipSelectionItemVM.IsRelevant && navalCustomBattleShipSelectionItemVM.HasSelectedItem)
				{
					list.Add(navalCustomBattleShipSelectionItemVM.SelectedItem.Ship);
				}
			}
			return list;
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00006734 File Offset: 0x00004934
		private void OnShipSelectedOrUpgraded()
		{
			Action onShipSelectedOrUpgraded = this._onShipSelectedOrUpgraded;
			if (onShipSelectedOrUpgraded != null)
			{
				onShipSelectedOrUpgraded();
			}
			this.UpdateCanShipsBecomeEmpty();
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00006750 File Offset: 0x00004950
		private void UpdateCanShipsBecomeEmpty()
		{
			int totalSelectedItemCount = this.ShipSelectionItems.Count<NavalCustomBattleShipSelectionItemVM>((NavalCustomBattleShipSelectionItemVM x) => x.IsRelevant && x.HasSelectedItem);
			this.ShipSelectionItems.ApplyActionOnAllItems(delegate(NavalCustomBattleShipSelectionItemVM x)
			{
				x.CanBecomeEmpty = x.HasSelectedItem && totalSelectedItemCount > 1;
			});
		}

		// Token: 0x06000100 RID: 256 RVA: 0x000067AA File Offset: 0x000049AA
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.ShipSelectionItems.ApplyActionOnAllItems(delegate(NavalCustomBattleShipSelectionItemVM x)
			{
				x.OnFinalize();
			});
		}

		// Token: 0x06000101 RID: 257 RVA: 0x000067DC File Offset: 0x000049DC
		public void SetCycleTierInputKey(HotKey hotkey)
		{
			foreach (NavalCustomBattleShipSelectionItemVM navalCustomBattleShipSelectionItemVM in this.ShipSelectionItems)
			{
				navalCustomBattleShipSelectionItemVM.SetCycleTierInputKey(hotkey);
			}
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x06000102 RID: 258 RVA: 0x00006828 File Offset: 0x00004A28
		// (set) Token: 0x06000103 RID: 259 RVA: 0x00006830 File Offset: 0x00004A30
		[DataSourceProperty]
		public bool IsRaid
		{
			get
			{
				return this._isRaid;
			}
			set
			{
				if (value != this._isRaid)
				{
					this._isRaid = value;
					base.OnPropertyChangedWithValue(value, "IsRaid");
					for (int i = 0; i < this.ShipSelectionItems.Count; i++)
					{
						this.ShipSelectionItems[i].IsRaid = value;
						this.ShipSelectionItems[i].IsRelevant = !value || i < 3;
					}
				}
			}
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x06000104 RID: 260 RVA: 0x0000689C File Offset: 0x00004A9C
		// (set) Token: 0x06000105 RID: 261 RVA: 0x000068A4 File Offset: 0x00004AA4
		[DataSourceProperty]
		public MBBindingList<NavalCustomBattleShipSelectionItemVM> ShipSelectionItems
		{
			get
			{
				return this._shipSelectionItems;
			}
			set
			{
				if (value != this._shipSelectionItems)
				{
					this._shipSelectionItems = value;
					base.OnPropertyChangedWithValue<MBBindingList<NavalCustomBattleShipSelectionItemVM>>(value, "ShipSelectionItems");
				}
			}
		}

		// Token: 0x0400008A RID: 138
		private readonly Action _onShipSelectedOrUpgraded;

		// Token: 0x0400008B RID: 139
		private bool _isRaid;

		// Token: 0x0400008C RID: 140
		private MBBindingList<NavalCustomBattleShipSelectionItemVM> _shipSelectionItems;
	}
}
