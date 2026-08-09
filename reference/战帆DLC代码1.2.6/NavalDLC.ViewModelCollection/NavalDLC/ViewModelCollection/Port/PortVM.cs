using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.HotKeyCategories;
using NavalDLC.ViewModelCollection.Port.PortScreenHandlers;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.ViewModelCollection.Port
{
	// Token: 0x0200000B RID: 11
	public class PortVM : ViewModel
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600003A RID: 58 RVA: 0x00005678 File Offset: 0x00003878
		public MBReadOnlyList<ShipItemVM> AllShips
		{
			get
			{
				return this._allShips;
			}
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00005680 File Offset: 0x00003880
		public PortVM(PortScreenHandler portScreenHandler, PortScreenModes portScreenMode, Action<Ship> onShipSelected, Action onRostersRefreshed, Action<ShipItemVM> refreshShipVisual, Action onUpgradeSlotSelected)
		{
			this._portScreenHandler = portScreenHandler;
			this._portScreenMode = portScreenMode;
			this._onShipSelected = onShipSelected;
			this._onRostersRefreshed = onRostersRefreshed;
			this._refreshShipVisual = refreshShipVisual;
			this._onUpgradeSlotSelected = onUpgradeSlotSelected;
			ShipItemVM.OnSelected += this.OnShipSelected;
			ShipItemVM.OnRenamed += this.OnShipRenamed;
			ShipItemVM.OnNameReset += this.OnShipNameReset;
			ShipUpgradePieceBaseVM.OnInspected += this.OnShipPieceInspected;
			ShipUpgradeSlotBaseVM.OnShipPieceSelected += this.OnShipPieceSelected;
			ShipUpgradeContainerVM.OnSlotSelected = (ShipUpgradeContainerVM.ShipSlotSelectedDelegate)Delegate.Combine(ShipUpgradeContainerVM.OnSlotSelected, new ShipUpgradeContainerVM.ShipSlotSelectedDelegate(this.OnUpgradeSlotSelected));
			ShipFigureheadSlotVM.GetCurrentFigurehead += this.GetCurrentFigurehead;
			ShipFigureheadSlotVM.GetShipOfFigurehead += this.GetShipOfFigurehead;
			ShipFigureheadSlotVM.GetIsRightSide += this.GetIsShipRightSide;
			ShipUpgradePieceVM.GetUpgradePrice += this.GetUpgradePrice;
			this._allShips = new MBList<ShipItemVM>();
			for (int i = 0; i < this._portScreenHandler.LeftShips.Count; i++)
			{
				this._allShips.Add(new ShipItemVM(this._portScreenHandler.LeftShips[i]));
			}
			for (int j = 0; j < this._portScreenHandler.RightShips.Count; j++)
			{
				this._allShips.Add(new ShipItemVM(this._portScreenHandler.RightShips[j]));
			}
			for (int k = 0; k < this._allShips.Count; k++)
			{
				this._allShips[k].RefreshProperties(this._portScreenHandler);
			}
			this._cachedChanges = new List<PortChangeInfo>();
			this.CanConfirmHint = new HintViewModel();
			this.GoldCostHint = new BasicTooltipViewModel(() => this.GetGoldCostTooltip());
			this.LeftRoster = new ShipRosterVM(new Action(this.OnLeftRosterSelected));
			this.RightRoster = new ShipRosterVM(new Action(this.OnRightRosterSelected));
			this.BuyAction = new PortActionVM(new Action(this.ExecuteBuy));
			this.SellAction = new PortActionVM(new Action(this.ExecuteSell));
			this.RepairAction = new PortActionVM(new Action(this.ExecuteRepair));
			this.RepairAllAction = new PortActionVM(new Action(this.ExecuteRepairAll));
			this.SendToClanAction = new PortActionVM(new Action(this.ExecuteSendToClan));
			this.GamepadCameraControlKeys = new MBBindingList<InputKeyItemVM>();
			this.KeyboardMoveCameraInputKeys = new MBBindingList<InputKeyItemVM>();
			this.RefreshRosters();
			this.RefreshActionAvailabilities();
			this.RefreshValues();
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00005928 File Offset: 0x00003B28
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.CancelText = new TextObject("{=3CpNUnVl}Cancel", null).ToString();
			this.ConfirmText = new TextObject("{=5Unqsx3N}Confirm", null).ToString();
			this.LeftRoster.RefreshValues();
			this.RightRoster.RefreshValues();
			this.KeyboardMoveCameraText = GameTexts.FindText("str_key_name", typeof(PortHotKeyCategory).Name + "_MovementAxisX").ToString();
			InputKeyItemVM doneInputKey = this.DoneInputKey;
			if (doneInputKey != null)
			{
				doneInputKey.RefreshValues();
			}
			InputKeyItemVM resetInputKey = this.ResetInputKey;
			if (resetInputKey != null)
			{
				resetInputKey.RefreshValues();
			}
			InputKeyItemVM cancelInputKey = this.CancelInputKey;
			if (cancelInputKey != null)
			{
				cancelInputKey.RefreshValues();
			}
			foreach (InputKeyItemVM inputKeyItemVM in this.GamepadCameraControlKeys)
			{
				inputKeyItemVM.RefreshValues();
			}
			foreach (InputKeyItemVM inputKeyItemVM2 in this.KeyboardMoveCameraInputKeys)
			{
				inputKeyItemVM2.RefreshValues();
			}
			InputKeyItemVM keyboardRotateCameraInputKey = this.KeyboardRotateCameraInputKey;
			if (keyboardRotateCameraInputKey != null)
			{
				keyboardRotateCameraInputKey.RefreshValues();
			}
			this.UpdateTotalGoldCost();
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00005A6C File Offset: 0x00003C6C
		public override void OnFinalize()
		{
			base.OnFinalize();
			this.LeftRoster.OnFinalize();
			this.RightRoster.OnFinalize();
			ShipItemVM.OnSelected -= this.OnShipSelected;
			ShipItemVM.OnRenamed -= this.OnShipRenamed;
			ShipItemVM.OnNameReset -= this.OnShipNameReset;
			ShipUpgradePieceBaseVM.OnInspected -= this.OnShipPieceInspected;
			ShipUpgradeSlotBaseVM.OnShipPieceSelected -= this.OnShipPieceSelected;
			ShipUpgradeContainerVM.OnSlotSelected = (ShipUpgradeContainerVM.ShipSlotSelectedDelegate)Delegate.Remove(ShipUpgradeContainerVM.OnSlotSelected, new ShipUpgradeContainerVM.ShipSlotSelectedDelegate(this.OnUpgradeSlotSelected));
			ShipFigureheadSlotVM.GetCurrentFigurehead -= this.GetCurrentFigurehead;
			ShipFigureheadSlotVM.GetShipOfFigurehead -= this.GetShipOfFigurehead;
			ShipFigureheadSlotVM.GetIsRightSide -= this.GetIsShipRightSide;
			ShipUpgradePieceVM.GetUpgradePrice -= this.GetUpgradePrice;
			InputKeyItemVM doneInputKey = this.DoneInputKey;
			if (doneInputKey != null)
			{
				doneInputKey.OnFinalize();
			}
			InputKeyItemVM cancelInputKey = this.CancelInputKey;
			if (cancelInputKey != null)
			{
				cancelInputKey.OnFinalize();
			}
			InputKeyItemVM resetInputKey = this.ResetInputKey;
			if (resetInputKey != null)
			{
				resetInputKey.OnFinalize();
			}
			foreach (InputKeyItemVM inputKeyItemVM in this.GamepadCameraControlKeys)
			{
				inputKeyItemVM.OnFinalize();
			}
			foreach (InputKeyItemVM inputKeyItemVM2 in this.KeyboardMoveCameraInputKeys)
			{
				inputKeyItemVM2.OnFinalize();
			}
			InputKeyItemVM keyboardRotateCameraInputKey = this.KeyboardRotateCameraInputKey;
			if (keyboardRotateCameraInputKey == null)
			{
				return;
			}
			keyboardRotateCameraInputKey.OnFinalize();
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00005C08 File Offset: 0x00003E08
		public void OnTick(float dt)
		{
			for (int i = 0; i < this._allShips.Count; i++)
			{
				this._allShips[i].Upgrades.Update();
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00005C44 File Offset: 0x00003E44
		public void UpdateGamepadCameraControlButtonsVisibility()
		{
			bool? flag = null;
			bool? flag2 = null;
			if (!this.IsControllingCamera)
			{
				flag = new bool?(false);
			}
			if (!this.CanToggleCamera)
			{
				flag2 = new bool?(false);
			}
			for (int i = 0; i < this.GamepadCameraControlKeys.Count; i++)
			{
				InputKeyItemVM inputKeyItemVM = this.GamepadCameraControlKeys[i];
				if (inputKeyItemVM != this.GamepadToggleCameraInputKey)
				{
					inputKeyItemVM.SetForcedVisibility(flag);
				}
				else
				{
					inputKeyItemVM.SetForcedVisibility(flag2);
				}
			}
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00005CC0 File Offset: 0x00003EC0
		private void UpdateTotalGoldCost()
		{
			this.TotalGoldCost = this._portScreenHandler.GetTotalGoldCost();
			this._cachedChanges = this._portScreenHandler.GetChanges();
			if (this.TotalGoldCost > 0 || (this.TotalGoldCost == 0 && this._cachedChanges.Count > 0))
			{
				this.TotalGoldCostText = new TextObject("{=jM8XqvAD}You will pay {GOLD}{GOLD_ICON}", null).SetTextVariable("GOLD", this.TotalGoldCost).SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"6\">").ToString();
			}
			else if (this.TotalGoldCost < 0)
			{
				this.TotalGoldCostText = new TextObject("{=6ELEOERd}You will receive {GOLD}{GOLD_ICON}", null).SetTextVariable("GOLD", -this.TotalGoldCost).SetTextVariable("GOLD_ICON", "{=!}<img src=\"General\\Icons\\Coin@2x\" extend=\"6\">").ToString();
			}
			else
			{
				this.TotalGoldCostText = string.Empty;
			}
			this.UpdateCanConfirm();
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00005D98 File Offset: 0x00003F98
		private void UpdateCanConfirm()
		{
			TextObject textObject;
			if (this._portScreenHandler.GetCanConfirm(out textObject))
			{
				this.IsConfirmDisabled = false;
				return;
			}
			this.IsConfirmDisabled = true;
			this.CanConfirmHint.HintText = textObject;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00005DD0 File Offset: 0x00003FD0
		private List<TooltipProperty> GetGoldCostTooltip()
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			if (this.TotalGoldCost >= 0)
			{
				using (List<PortChangeInfo>.Enumerator enumerator = this._cachedChanges.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PortChangeInfo portChangeInfo = enumerator.Current;
						list.Add(new TooltipProperty(portChangeInfo.Description, ((int)portChangeInfo.GoldCost).ToString("+#;-#;0"), 0, false, 0));
					}
					return list;
				}
			}
			if (this.TotalGoldCost < 0)
			{
				foreach (PortChangeInfo portChangeInfo2 in this._cachedChanges)
				{
					list.Add(new TooltipProperty(portChangeInfo2.Description, (-(int)portChangeInfo2.GoldCost).ToString("+#;-#;0"), 0, false, 0));
				}
			}
			return list;
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00005EC4 File Offset: 0x000040C4
		public bool AreThereAnyChanges()
		{
			return this._portScreenHandler.AreThereAnyChanges();
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00005ED4 File Offset: 0x000040D4
		public void SelectFirstAvailableRosterAndShip()
		{
			ShipRosterVM shipRosterVM;
			ShipRosterVM shipRosterVM2;
			if (this._portScreenMode == 3)
			{
				shipRosterVM = this.LeftRoster;
				shipRosterVM2 = this.RightRoster;
			}
			else
			{
				shipRosterVM = this.RightRoster;
				shipRosterVM2 = this.LeftRoster;
			}
			if (shipRosterVM.HasAnyShips)
			{
				shipRosterVM.ExecuteSelectRoster();
				shipRosterVM.Ships[0].ExecuteSelect();
				return;
			}
			if (shipRosterVM2.HasAnyShips)
			{
				shipRosterVM2.ExecuteSelectRoster();
				shipRosterVM2.Ships[0].ExecuteSelect();
				return;
			}
			Debug.FailedAssert("There are no ships on either roster!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.ViewModelCollection\\Port\\PortVM.cs", "SelectFirstAvailableRosterAndShip", 280);
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00005F64 File Offset: 0x00004164
		private void SelectClosestShipFromActiveRoster(int previousSelectedIndex)
		{
			ShipRosterVM selectedRoster = this.GetSelectedRoster();
			if (!selectedRoster.HasAnyShips || previousSelectedIndex < 0)
			{
				this.SelectFirstAvailableRosterAndShip();
				return;
			}
			int num = MathF.Min(selectedRoster.Ships.Count - 1, previousSelectedIndex);
			selectedRoster.Ships[num].ExecuteSelect();
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00005FB0 File Offset: 0x000041B0
		private ShipRosterVM GetSelectedRoster()
		{
			if (!this.LeftRoster.IsSelected)
			{
				return this.RightRoster;
			}
			return this.LeftRoster;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00005FCC File Offset: 0x000041CC
		public void ExecuteCancelWithoutInquiry()
		{
			this.ExecuteCancel(false);
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00005FD8 File Offset: 0x000041D8
		public void ExecuteCancel(bool showCancelInquiry = false)
		{
			if (this._portScreenMode == 3)
			{
				if (this.AreThereAnyChanges())
				{
					InformationManager.ShowInquiry(new InquiryData("", GameTexts.FindText("str_cancelling_changes", null).ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(this.ExecuteCancelInternal), null, "", 0f, null, null, null), false, false);
					return;
				}
				if (this.LeftRoster.HasAnyShips)
				{
					InformationManager.ShowInquiry(new InquiryData("", GameTexts.FindText("str_leaving_ships_behind", null).ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(this.ExecuteCancelInternal), null, "", 0f, null, null, null), false, false);
					return;
				}
				this.ExecuteCancelInternal();
				return;
			}
			else
			{
				if (showCancelInquiry && this.AreThereAnyChanges())
				{
					InformationManager.ShowInquiry(new InquiryData("", GameTexts.FindText("str_cancelling_changes", null).ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(this.ExecuteCancelInternal), null, "", 0f, null, null, null), false, false);
					return;
				}
				this.ExecuteCancelInternal();
				return;
			}
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00006138 File Offset: 0x00004338
		private void ExecuteCancelInternal()
		{
			GameStateManager.Current.PopState(0);
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00006148 File Offset: 0x00004348
		public void ExecuteConfirm()
		{
			if (!this.IsConfirmDisabled)
			{
				if (this._portScreenMode == 3 && this.LeftRoster.HasAnyShips)
				{
					InformationManager.ShowInquiry(new InquiryData("", GameTexts.FindText("str_leaving_ships_behind", null).ToString(), true, true, GameTexts.FindText("str_yes", null).ToString(), GameTexts.FindText("str_no", null).ToString(), new Action(this.ExecuteConfirmInternal), null, "", 0f, null, null, null), false, false);
					return;
				}
				this.ExecuteConfirmInternal();
			}
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000061D7 File Offset: 0x000043D7
		private void ExecuteConfirmInternal()
		{
			this._portScreenHandler.OnConfirmChanges();
			GameStateManager.Current.PopState(0);
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000061F0 File Offset: 0x000043F0
		public void ExecuteReset()
		{
			int num = this.GetSelectedRoster().Ships.IndexOf(this.SelectedShip);
			this._portScreenHandler.ResetChanges();
			for (int i = 0; i < this._allShips.Count; i++)
			{
				this._allShips[i].Upgrades.ResetUpgradePieces();
			}
			this.RefreshRosters();
			this.SelectClosestShipFromActiveRoster(num);
			this.UpdateTotalGoldCost();
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00006260 File Offset: 0x00004460
		public void ExecuteRepair()
		{
			this._portScreenHandler.OnRepairShip(this.SelectedShip.Ship);
			this.SelectedShip.CurrentHp = this.SelectedShip.MaxHp;
			this.SelectedShip.IsRepaired = true;
			this.UpdateTotalGoldCost();
			this.RefreshRosters();
		}

		// Token: 0x0600004E RID: 78 RVA: 0x000062B4 File Offset: 0x000044B4
		public void ExecuteRepairAll()
		{
			foreach (ShipItemVM shipItemVM in this.RightRoster.Ships)
			{
				Ship ship = shipItemVM.Ship;
				PortActionInfo canRepairShip = this._portScreenHandler.GetCanRepairShip(ship);
				if (canRepairShip.IsRelevant && canRepairShip.IsEnabled)
				{
					this._portScreenHandler.OnRepairShip(ship);
					shipItemVM.CurrentHp = shipItemVM.MaxHp;
					shipItemVM.IsRepaired = true;
				}
			}
			this.UpdateTotalGoldCost();
			this.RefreshRosters();
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00006350 File Offset: 0x00004550
		public void ExecuteSendToClan()
		{
			int num = this.GetSelectedRoster().Ships.IndexOf(this.SelectedShip);
			this._portScreenHandler.OnSendToClan(this.SelectedShip.Ship);
			this.UpdateTotalGoldCost();
			this.RefreshRosters();
			this.SelectClosestShipFromActiveRoster(num);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x000063A0 File Offset: 0x000045A0
		public void ExecuteBuy()
		{
			int num = this.GetSelectedRoster().Ships.IndexOf(this.SelectedShip);
			this._portScreenHandler.OnBuyShip(this.SelectedShip.Ship);
			this.UpdateTotalGoldCost();
			this.RefreshRosters();
			this.SelectClosestShipFromActiveRoster(num);
		}

		// Token: 0x06000051 RID: 81 RVA: 0x000063F0 File Offset: 0x000045F0
		public void ExecuteSell()
		{
			bool flag = false;
			for (int i = 0; i < this._portScreenHandler.SelectedShipPieces.Count; i++)
			{
				if (this._portScreenHandler.SelectedShipPieces[i].Ship == this.SelectedShip.Ship)
				{
					flag = true;
				}
			}
			for (int j = 0; j < this._portScreenHandler.SelectedFigureheads.Count; j++)
			{
				if (this._portScreenHandler.SelectedFigureheads[j].Ship == this.SelectedShip.Ship)
				{
					flag = true;
				}
			}
			if (this.SelectedShip.IsRepaired || this.SelectedShip.IsRenamed || flag)
			{
				InformationManager.ShowInquiry(new InquiryData(new TextObject("{=2H95Y2vK}Sell Ship?", null).ToString(), new TextObject("{=baQh2cwb}Selling this ship will revert your previous changes to it. Are you sure?", null).ToString(), true, true, GameTexts.FindText("str_ok", null).ToString(), GameTexts.FindText("str_cancel", null).ToString(), new Action(this.ExecuteSellAux), null, "", 0f, null, null, null), false, false);
				return;
			}
			this.ExecuteSellAux();
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00006510 File Offset: 0x00004710
		private void ExecuteSellAux()
		{
			int num = this.GetSelectedRoster().Ships.IndexOf(this.SelectedShip);
			this._portScreenHandler.OnSellShip(this.SelectedShip.Ship);
			this.SelectedShip.Upgrades.ResetUpgradePieces();
			this.UpdateTotalGoldCost();
			this.RefreshRosters();
			this.SelectClosestShipFromActiveRoster(num);
		}

		// Token: 0x06000053 RID: 83 RVA: 0x0000656D File Offset: 0x0000476D
		public void ExecuteDeselectSlot()
		{
			ShipUpgradeSlotBaseVM selectedUpgradeSlot = this.SelectedUpgradeSlot;
			if (selectedUpgradeSlot == null)
			{
				return;
			}
			selectedUpgradeSlot.ExecuteDeselect();
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00006580 File Offset: 0x00004780
		public bool ExecuteSelectPreviousShip()
		{
			ShipRosterVM selectedRoster = this.GetSelectedRoster();
			if (!selectedRoster.HasAnyShips)
			{
				return false;
			}
			int num = selectedRoster.Ships.IndexOf(this.SelectedShip);
			if (num == -1)
			{
				Debug.FailedAssert("Selected ship not found in selected roster!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.ViewModelCollection\\Port\\PortVM.cs", "ExecuteSelectPreviousShip", 552);
				selectedRoster.Ships[0].ExecuteSelect();
			}
			else
			{
				int num2 = num - 1;
				if (num2 < 0)
				{
					num2 = selectedRoster.Ships.Count - 1;
				}
				selectedRoster.Ships[num2].ExecuteSelect();
			}
			return true;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00006608 File Offset: 0x00004808
		public bool ExecuteSelectNextShip()
		{
			ShipRosterVM selectedRoster = this.GetSelectedRoster();
			if (!selectedRoster.HasAnyShips)
			{
				return false;
			}
			int num = selectedRoster.Ships.IndexOf(this.SelectedShip);
			if (num == -1)
			{
				Debug.FailedAssert("Selected ship not found in selected roster!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.ViewModelCollection\\Port\\PortVM.cs", "ExecuteSelectNextShip", 581);
				selectedRoster.Ships[0].ExecuteSelect();
			}
			else
			{
				int num2 = num + 1;
				if (num2 >= selectedRoster.Ships.Count)
				{
					num2 = 0;
				}
				selectedRoster.Ships[num2].ExecuteSelect();
			}
			return true;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00006690 File Offset: 0x00004890
		private void OnLeftRosterSelected()
		{
			if (this.LeftRoster.IsSelected)
			{
				return;
			}
			this.LeftRoster.IsSelected = true;
			this.RightRoster.IsSelected = false;
			if (this.LeftRoster.HasAnyShips)
			{
				this.LeftRoster.Ships[0].ExecuteSelect();
			}
		}

		// Token: 0x06000057 RID: 87 RVA: 0x000066E8 File Offset: 0x000048E8
		private void OnRightRosterSelected()
		{
			if (this.RightRoster.IsSelected)
			{
				return;
			}
			this.LeftRoster.IsSelected = false;
			this.RightRoster.IsSelected = true;
			if (this.RightRoster.HasAnyShips)
			{
				this.RightRoster.Ships[0].ExecuteSelect();
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x0000673E File Offset: 0x0000493E
		private void OnShipPieceInspected(ShipUpgradePieceBaseVM piece)
		{
			if (this.InspectedUpgrade != null && this.InspectedUpgrade != piece)
			{
				this.InspectedUpgrade.IsInspected = false;
			}
			if (piece != null)
			{
				this.InspectedUpgrade = piece;
				this.InspectedUpgrade.IsInspected = true;
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00006774 File Offset: 0x00004974
		public void OnShipPieceSelected(Ship ship, string shipSlotTag, string slotTypeId, ShipUpgradePieceBaseVM pieceVM)
		{
			if (ship == null || string.IsNullOrEmpty(shipSlotTag))
			{
				Debug.FailedAssert("Ship piece selected in an invalid state!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.ViewModelCollection\\Port\\PortVM.cs", "OnShipPieceSelected", 647);
				return;
			}
			if (pieceVM != null && pieceVM.IsDisabled)
			{
				return;
			}
			if (string.Equals(slotTypeId, "figurehead", StringComparison.InvariantCultureIgnoreCase))
			{
				PortScreenHandler portScreenHandler = this._portScreenHandler;
				Ship ship2 = ship;
				ShipFigureheadVM shipFigureheadVM = pieceVM as ShipFigureheadVM;
				portScreenHandler.OnFigureheadSelected(ship2, (shipFigureheadVM != null) ? shipFigureheadVM.Figurehead : null);
				this.UpdateAvailableFigureheads();
			}
			else
			{
				PortScreenHandler portScreenHandler2 = this._portScreenHandler;
				Ship ship3 = ship;
				ShipUpgradePieceVM shipUpgradePieceVM = pieceVM as ShipUpgradePieceVM;
				portScreenHandler2.OnUpgradePieceSelected(ship3, shipSlotTag, (shipUpgradePieceVM != null) ? shipUpgradePieceVM.Piece : null);
			}
			this.RefreshSelectedShipProperties();
			this.UpdateTotalGoldCost();
			Action<ShipItemVM> refreshShipVisual = this._refreshShipVisual;
			if (refreshShipVisual == null)
			{
				return;
			}
			refreshShipVisual(this.AllShips.FirstOrDefault<ShipItemVM>((ShipItemVM x) => x.Ship == ship));
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00006858 File Offset: 0x00004A58
		public void UpdateAvailableFigureheads()
		{
			for (int i = 0; i < this._allShips.Count; i++)
			{
				ShipFigureheadSlotVM figureheadSlot = this.GetFigureheadSlot(this._allShips[i]);
				if (figureheadSlot != null)
				{
					figureheadSlot.UpdateAvailableFigureheads();
				}
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00006898 File Offset: 0x00004A98
		public Figurehead GetCurrentFigurehead(Ship ship)
		{
			foreach (PortScreenHandler.ShipFigureheadInfo shipFigureheadInfo in this._portScreenHandler.SelectedFigureheads)
			{
				if (shipFigureheadInfo.Ship == ship)
				{
					return shipFigureheadInfo.Figurehead;
				}
			}
			return ship.Figurehead;
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00006904 File Offset: 0x00004B04
		public Ship GetShipOfFigurehead(Figurehead figurehead, bool isRightSide)
		{
			MBReadOnlyList<Ship> mbreadOnlyList = (isRightSide ? this._portScreenHandler.RightShips : this._portScreenHandler.LeftShips);
			for (int i = 0; i < mbreadOnlyList.Count; i++)
			{
				Ship ship = mbreadOnlyList[i];
				if (this.GetCurrentFigurehead(ship) == figurehead)
				{
					return ship;
				}
			}
			return null;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00006953 File Offset: 0x00004B53
		private ShipFigureheadSlotVM GetFigureheadSlot(ShipItemVM ship)
		{
			return ship.Upgrades.UpgradeSlots.FirstOrDefault<ShipUpgradeSlotBaseVM>((ShipUpgradeSlotBaseVM x) => x is ShipFigureheadSlotVM) as ShipFigureheadSlotVM;
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00006989 File Offset: 0x00004B89
		private bool GetIsShipRightSide(Ship ship)
		{
			return this._portScreenHandler.RightShips.Contains(ship);
		}

		// Token: 0x0600005F RID: 95 RVA: 0x0000699C File Offset: 0x00004B9C
		public void OnUpgradeSlotSelected(ShipUpgradeSlotBaseVM slot)
		{
			this.SelectedUpgradeSlot = slot;
			if (this.SelectedUpgradeSlot == null)
			{
				InformationManager.HideTooltip();
				ShipUpgradePieceBaseVM inspectedUpgrade = this.InspectedUpgrade;
				if (inspectedUpgrade == null || !inspectedUpgrade.IsInspectedFromSlot)
				{
					this.OnShipPieceInspected(null);
				}
			}
			Action onUpgradeSlotSelected = this._onUpgradeSlotSelected;
			if (onUpgradeSlotSelected == null)
			{
				return;
			}
			onUpgradeSlotSelected();
		}

		// Token: 0x06000060 RID: 96 RVA: 0x000069EB File Offset: 0x00004BEB
		public int GetUpgradePrice(Ship ship, ShipUpgradePiece piece)
		{
			return this._portScreenHandler.GetUpgradeCostOfShip(ship, piece, true);
		}

		// Token: 0x06000061 RID: 97 RVA: 0x000069FB File Offset: 0x00004BFB
		private void OnShipRenamed(ShipItemVM ship, string newName)
		{
			this._portScreenHandler.OnRenameShip(ship.Ship, newName);
			ship.RefreshProperties(this._portScreenHandler);
			this.UpdateTotalGoldCost();
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00006A21 File Offset: 0x00004C21
		private void OnShipNameReset(ShipItemVM ship)
		{
			this._portScreenHandler.OnResetShipName(ship.Ship);
			ship.RefreshProperties(this._portScreenHandler);
			this.UpdateTotalGoldCost();
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00006A48 File Offset: 0x00004C48
		private void OnShipSelected(ShipItemVM ship)
		{
			if (this.SelectedShip == ship)
			{
				return;
			}
			ShipItemVM selectedShip = this.SelectedShip;
			if (selectedShip != null)
			{
				ShipUpgradeContainerVM upgrades = selectedShip.Upgrades;
				if (upgrades != null)
				{
					ShipUpgradeSlotBaseVM selectedSlot = upgrades.SelectedSlot;
					if (selectedSlot != null)
					{
						selectedSlot.ExecuteDeselect();
					}
				}
			}
			InformationManager.HideTooltip();
			this.OnShipPieceInspected(null);
			this.SelectedShip = ship;
			this.RefreshSelectedShipProperties();
			Action<Ship> onShipSelected = this._onShipSelected;
			if (onShipSelected == null)
			{
				return;
			}
			ShipItemVM selectedShip2 = this.SelectedShip;
			onShipSelected((selectedShip2 != null) ? selectedShip2.Ship : null);
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00006AC4 File Offset: 0x00004CC4
		private void RefreshSelectedShipProperties()
		{
			if (this.SelectedShip != null)
			{
				this.SelectedShip.RefreshProperties(this._portScreenHandler);
				MBList<ValueTuple<string, ShipUpgradePiece>> mblist = new MBList<ValueTuple<string, ShipUpgradePiece>>();
				for (int i = 0; i < this.SelectedShip.Upgrades.UpgradeSlots.Count; i++)
				{
					ShipUpgradeSlotBaseVM shipUpgradeSlotBaseVM = this.SelectedShip.Upgrades.UpgradeSlots[i];
					if (shipUpgradeSlotBaseVM.IsChanged && shipUpgradeSlotBaseVM is ShipUpgradeSlotVM)
					{
						List<ValueTuple<string, ShipUpgradePiece>> list = mblist;
						string shipSlotTag = shipUpgradeSlotBaseVM.ShipSlotTag;
						ShipUpgradePieceVM shipUpgradePieceVM = shipUpgradeSlotBaseVM.SelectedPiece as ShipUpgradePieceVM;
						list.Add(new ValueTuple<string, ShipUpgradePiece>(shipSlotTag, (shipUpgradePieceVM != null) ? shipUpgradePieceVM.Piece : null));
					}
				}
				this.SelectedShip.Stats.RefreshStats(this.SelectedShip.CurrentHp, mblist);
				this.RefreshActionAvailabilities();
			}
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00006B84 File Offset: 0x00004D84
		private void RefreshRosters()
		{
			this.LeftRoster.SetRosterName(this._portScreenHandler.GetLeftRosterName());
			this.RightRoster.SetRosterName(this._portScreenHandler.GetRightRosterName());
			this.LeftRoster.SetRosterOwner(this._portScreenHandler.GetLeftSideOwnerParty());
			this.RightRoster.SetRosterOwner(this._portScreenHandler.GetRightSideOwnerParty());
			MBReadOnlyList<ShipItemVM> mbreadOnlyList;
			MBReadOnlyList<ShipItemVM> mbreadOnlyList2;
			PortVM.GetRosterDifferences(this._allShips, this._portScreenHandler.LeftShips, this.LeftRoster.Ships, out mbreadOnlyList, out mbreadOnlyList2);
			MBReadOnlyList<ShipItemVM> mbreadOnlyList3;
			MBReadOnlyList<ShipItemVM> mbreadOnlyList4;
			PortVM.GetRosterDifferences(this._allShips, this._portScreenHandler.RightShips, this.RightRoster.Ships, out mbreadOnlyList3, out mbreadOnlyList4);
			this.LeftRoster.RefreshShips(mbreadOnlyList, mbreadOnlyList2, this._portScreenHandler.LeftShips);
			this.RightRoster.RefreshShips(mbreadOnlyList3, mbreadOnlyList4, this._portScreenHandler.RightShips);
			for (int i = 0; i < this._allShips.Count; i++)
			{
				this._allShips[i].RefreshProperties(this._portScreenHandler);
			}
			this.RefreshSelectedShipProperties();
			this.UpdateAvailableFigureheads();
			Action onRostersRefreshed = this._onRostersRefreshed;
			if (onRostersRefreshed == null)
			{
				return;
			}
			onRostersRefreshed();
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00006CB4 File Offset: 0x00004EB4
		private static void GetRosterDifferences(MBReadOnlyList<ShipItemVM> allShips, MBReadOnlyList<Ship> currentShips, MBBindingList<ShipItemVM> dataSourceShips, out MBReadOnlyList<ShipItemVM> removedShips, out MBReadOnlyList<ShipItemVM> addedShips)
		{
			MBList<ShipItemVM> mblist = new MBList<ShipItemVM>();
			MBList<ShipItemVM> mblist2 = new MBList<ShipItemVM>();
			for (int i = 0; i < dataSourceShips.Count; i++)
			{
				ShipItemVM shipItemVM = dataSourceShips[i];
				Ship ship = shipItemVM.Ship;
				if (!currentShips.Contains(ship))
				{
					mblist.Add(shipItemVM);
				}
			}
			for (int j = 0; j < currentShips.Count; j++)
			{
				Ship ship2 = currentShips[j];
				bool flag = false;
				for (int k = 0; k < dataSourceShips.Count; k++)
				{
					if (dataSourceShips[k].Ship == ship2)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					ShipItemVM shipItemVM2 = null;
					for (int l = 0; l < allShips.Count; l++)
					{
						if (allShips[l].Ship == ship2)
						{
							shipItemVM2 = allShips[l];
							break;
						}
					}
					if (shipItemVM2 == null)
					{
						Debug.FailedAssert(string.Format("Unable to find vm for ship: {0}", ship2), "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.ViewModelCollection\\Port\\PortVM.cs", "GetRosterDifferences", 870);
					}
					else
					{
						mblist2.Add(shipItemVM2);
					}
				}
			}
			removedShips = mblist;
			addedShips = mblist2;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00006DC0 File Offset: 0x00004FC0
		private void RefreshActionAvailabilities()
		{
			if (this.SelectedShip != null)
			{
				PortActionInfo canBuyShip = this._portScreenHandler.GetCanBuyShip(this.SelectedShip.Ship);
				this.BuyAction.RefreshWith(canBuyShip);
				this.BuyAction.AdditionalInfo = PortVM.GetGoldCostText(canBuyShip.GoldCost);
				PortActionInfo canSellShip = this._portScreenHandler.GetCanSellShip(this.SelectedShip.Ship);
				this.SellAction.RefreshWith(canSellShip);
				this.SellAction.AdditionalInfo = PortVM.GetGoldCostText(canSellShip.GoldCost);
				PortActionInfo canRepairShip = this._portScreenHandler.GetCanRepairShip(this.SelectedShip.Ship);
				this.RepairAction.RefreshWith(canRepairShip);
				this.RepairAction.AdditionalInfo = PortVM.GetGoldCostText(canRepairShip.GoldCost);
				PortActionInfo canRepairAll = this._portScreenHandler.GetCanRepairAll(this.SelectedShip.Ship);
				this.RepairAllAction.RefreshWith(canRepairAll);
				this.RepairAllAction.AdditionalInfo = PortVM.GetGoldCostText(canRepairAll.GoldCost);
				PortActionInfo canUpgradeShip = this._portScreenHandler.GetCanUpgradeShip(this.SelectedShip.Ship);
				this.SelectedShip.Upgrades.UpdateEnabledStatus(in canUpgradeShip);
				TextObject actionName = canUpgradeShip.ActionName;
				this.UpgradeText = ((actionName != null) ? actionName.ToString() : null);
				PortActionInfo canRenameShip = this._portScreenHandler.GetCanRenameShip(this.SelectedShip.Ship);
				this.SelectedShip.PlayerCanChangeShipName = canRenameShip.IsRelevant && canRenameShip.IsEnabled;
				this.SelectedShip.ChangeShipNameHint = new HintViewModel(canRenameShip.Tooltip, null);
				PortActionInfo canSendToClan = this._portScreenHandler.GetCanSendToClan(this.SelectedShip.Ship);
				this.SendToClanAction.RefreshWith(canSendToClan);
				this.SendToClanAction.AdditionalInfo = string.Empty;
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00006F82 File Offset: 0x00005182
		private static string GetGoldCostText(int cost)
		{
			if (cost == 0)
			{
				return string.Empty;
			}
			return new TextObject("{=ePmSvu1s}{AMOUNT}{GOLD_ICON}", null).SetTextVariable("AMOUNT", cost).ToString();
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000069 RID: 105 RVA: 0x00006FA8 File Offset: 0x000051A8
		// (set) Token: 0x0600006A RID: 106 RVA: 0x00006FB0 File Offset: 0x000051B0
		[DataSourceProperty]
		public PortActionVM BuyAction
		{
			get
			{
				return this._buyAction;
			}
			set
			{
				if (value != this._buyAction)
				{
					this._buyAction = value;
					base.OnPropertyChangedWithValue<PortActionVM>(value, "BuyAction");
				}
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600006B RID: 107 RVA: 0x00006FCE File Offset: 0x000051CE
		// (set) Token: 0x0600006C RID: 108 RVA: 0x00006FD6 File Offset: 0x000051D6
		[DataSourceProperty]
		public PortActionVM SellAction
		{
			get
			{
				return this._sellAction;
			}
			set
			{
				if (value != this._sellAction)
				{
					this._sellAction = value;
					base.OnPropertyChangedWithValue<PortActionVM>(value, "SellAction");
				}
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600006D RID: 109 RVA: 0x00006FF4 File Offset: 0x000051F4
		// (set) Token: 0x0600006E RID: 110 RVA: 0x00006FFC File Offset: 0x000051FC
		[DataSourceProperty]
		public PortActionVM RepairAction
		{
			get
			{
				return this._repairAction;
			}
			set
			{
				if (value != this._repairAction)
				{
					this._repairAction = value;
					base.OnPropertyChangedWithValue<PortActionVM>(value, "RepairAction");
				}
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600006F RID: 111 RVA: 0x0000701A File Offset: 0x0000521A
		// (set) Token: 0x06000070 RID: 112 RVA: 0x00007022 File Offset: 0x00005222
		[DataSourceProperty]
		public bool IsConfirmDisabled
		{
			get
			{
				return this._isConfirmDisabled;
			}
			set
			{
				if (value != this._isConfirmDisabled)
				{
					this._isConfirmDisabled = value;
					base.OnPropertyChangedWithValue(value, "IsConfirmDisabled");
				}
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000071 RID: 113 RVA: 0x00007040 File Offset: 0x00005240
		// (set) Token: 0x06000072 RID: 114 RVA: 0x00007048 File Offset: 0x00005248
		[DataSourceProperty]
		public PortActionVM SendToClanAction
		{
			get
			{
				return this._sendToClanAction;
			}
			set
			{
				if (value != this._sendToClanAction)
				{
					this._sendToClanAction = value;
					base.OnPropertyChangedWithValue<PortActionVM>(value, "SendToClanAction");
				}
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000073 RID: 115 RVA: 0x00007066 File Offset: 0x00005266
		// (set) Token: 0x06000074 RID: 116 RVA: 0x0000706E File Offset: 0x0000526E
		[DataSourceProperty]
		public PortActionVM RepairAllAction
		{
			get
			{
				return this._repairAllAction;
			}
			set
			{
				if (value != this._repairAllAction)
				{
					this._repairAllAction = value;
					base.OnPropertyChangedWithValue<PortActionVM>(value, "RepairAllAction");
				}
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000075 RID: 117 RVA: 0x0000708C File Offset: 0x0000528C
		// (set) Token: 0x06000076 RID: 118 RVA: 0x00007094 File Offset: 0x00005294
		[DataSourceProperty]
		public bool CanUseKeyboardInputs
		{
			get
			{
				return this._canUseKeyboardInputs;
			}
			set
			{
				if (value != this._canUseKeyboardInputs)
				{
					this._canUseKeyboardInputs = value;
					base.OnPropertyChangedWithValue(value, "CanUseKeyboardInputs");
				}
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000077 RID: 119 RVA: 0x000070B2 File Offset: 0x000052B2
		// (set) Token: 0x06000078 RID: 120 RVA: 0x000070BA File Offset: 0x000052BA
		[DataSourceProperty]
		public bool CanUseGamepadInputs
		{
			get
			{
				return this._canUseGamepadInputs;
			}
			set
			{
				if (value != this._canUseGamepadInputs)
				{
					this._canUseGamepadInputs = value;
					base.OnPropertyChangedWithValue(value, "CanUseGamepadInputs");
				}
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000079 RID: 121 RVA: 0x000070D8 File Offset: 0x000052D8
		// (set) Token: 0x0600007A RID: 122 RVA: 0x000070E0 File Offset: 0x000052E0
		[DataSourceProperty]
		public bool IsControllingCamera
		{
			get
			{
				return this._isControllingCamera;
			}
			set
			{
				if (value != this._isControllingCamera)
				{
					this._isControllingCamera = value;
					base.OnPropertyChangedWithValue(value, "IsControllingCamera");
					this.UpdateGamepadCameraControlButtonsVisibility();
				}
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600007B RID: 123 RVA: 0x00007104 File Offset: 0x00005304
		// (set) Token: 0x0600007C RID: 124 RVA: 0x0000710C File Offset: 0x0000530C
		[DataSourceProperty]
		public bool CanToggleCamera
		{
			get
			{
				return this._canToggleCamera;
			}
			set
			{
				if (value != this._canToggleCamera)
				{
					this._canToggleCamera = value;
					base.OnPropertyChangedWithValue(value, "CanToggleCamera");
					this.UpdateGamepadCameraControlButtonsVisibility();
				}
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600007D RID: 125 RVA: 0x00007130 File Offset: 0x00005330
		// (set) Token: 0x0600007E RID: 126 RVA: 0x00007138 File Offset: 0x00005338
		[DataSourceProperty]
		public bool IsMapBarExtended
		{
			get
			{
				return this._isMapBarExtended;
			}
			set
			{
				if (value != this._isMapBarExtended)
				{
					this._isMapBarExtended = value;
					base.OnPropertyChangedWithValue(value, "IsMapBarExtended");
				}
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600007F RID: 127 RVA: 0x00007156 File Offset: 0x00005356
		// (set) Token: 0x06000080 RID: 128 RVA: 0x0000715E File Offset: 0x0000535E
		[DataSourceProperty]
		public string KeyboardMoveCameraText
		{
			get
			{
				return this._keyboardMoveCameraText;
			}
			set
			{
				if (value != this._keyboardMoveCameraText)
				{
					this._keyboardMoveCameraText = value;
					base.OnPropertyChangedWithValue<string>(value, "KeyboardMoveCameraText");
				}
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000081 RID: 129 RVA: 0x00007181 File Offset: 0x00005381
		// (set) Token: 0x06000082 RID: 130 RVA: 0x00007189 File Offset: 0x00005389
		[DataSourceProperty]
		public string CancelText
		{
			get
			{
				return this._cancelText;
			}
			set
			{
				if (value != this._cancelText)
				{
					this._cancelText = value;
					base.OnPropertyChangedWithValue<string>(value, "CancelText");
				}
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000083 RID: 131 RVA: 0x000071AC File Offset: 0x000053AC
		// (set) Token: 0x06000084 RID: 132 RVA: 0x000071B4 File Offset: 0x000053B4
		[DataSourceProperty]
		public string ConfirmText
		{
			get
			{
				return this._confirmText;
			}
			set
			{
				if (value != this._confirmText)
				{
					this._confirmText = value;
					base.OnPropertyChangedWithValue<string>(value, "ConfirmText");
				}
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000085 RID: 133 RVA: 0x000071D7 File Offset: 0x000053D7
		// (set) Token: 0x06000086 RID: 134 RVA: 0x000071DF File Offset: 0x000053DF
		[DataSourceProperty]
		public int TotalGoldCost
		{
			get
			{
				return this._totalGoldCost;
			}
			set
			{
				if (value != this._totalGoldCost)
				{
					this._totalGoldCost = value;
					base.OnPropertyChangedWithValue(value, "TotalGoldCost");
				}
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000087 RID: 135 RVA: 0x000071FD File Offset: 0x000053FD
		// (set) Token: 0x06000088 RID: 136 RVA: 0x00007205 File Offset: 0x00005405
		[DataSourceProperty]
		public string TotalGoldCostText
		{
			get
			{
				return this._totalGoldCostText;
			}
			set
			{
				if (value != this._totalGoldCostText)
				{
					this._totalGoldCostText = value;
					base.OnPropertyChangedWithValue<string>(value, "TotalGoldCostText");
				}
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x06000089 RID: 137 RVA: 0x00007228 File Offset: 0x00005428
		// (set) Token: 0x0600008A RID: 138 RVA: 0x00007230 File Offset: 0x00005430
		[DataSourceProperty]
		public string RepairText
		{
			get
			{
				return this._repairText;
			}
			set
			{
				if (value != this._repairText)
				{
					this._repairText = value;
					base.OnPropertyChangedWithValue<string>(value, "RepairText");
				}
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600008B RID: 139 RVA: 0x00007253 File Offset: 0x00005453
		// (set) Token: 0x0600008C RID: 140 RVA: 0x0000725B File Offset: 0x0000545B
		[DataSourceProperty]
		public string UpgradeText
		{
			get
			{
				return this._upgradeText;
			}
			set
			{
				if (value != this._upgradeText)
				{
					this._upgradeText = value;
					base.OnPropertyChangedWithValue<string>(value, "UpgradeText");
				}
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600008D RID: 141 RVA: 0x0000727E File Offset: 0x0000547E
		// (set) Token: 0x0600008E RID: 142 RVA: 0x00007286 File Offset: 0x00005486
		[DataSourceProperty]
		public string BuyText
		{
			get
			{
				return this._buyText;
			}
			set
			{
				if (value != this._buyText)
				{
					this._buyText = value;
					base.OnPropertyChangedWithValue<string>(value, "BuyText");
				}
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600008F RID: 143 RVA: 0x000072A9 File Offset: 0x000054A9
		// (set) Token: 0x06000090 RID: 144 RVA: 0x000072B1 File Offset: 0x000054B1
		[DataSourceProperty]
		public string SellText
		{
			get
			{
				return this._sellText;
			}
			set
			{
				if (value != this._sellText)
				{
					this._sellText = value;
					base.OnPropertyChangedWithValue<string>(value, "SellText");
				}
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000091 RID: 145 RVA: 0x000072D4 File Offset: 0x000054D4
		// (set) Token: 0x06000092 RID: 146 RVA: 0x000072DC File Offset: 0x000054DC
		[DataSourceProperty]
		public bool IsAnyUpgradeSlotSelected
		{
			get
			{
				return this._isAnyUpgradeSlotSelected;
			}
			set
			{
				if (value != this._isAnyUpgradeSlotSelected)
				{
					this._isAnyUpgradeSlotSelected = value;
					base.OnPropertyChangedWithValue(value, "IsAnyUpgradeSlotSelected");
				}
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000093 RID: 147 RVA: 0x000072FA File Offset: 0x000054FA
		// (set) Token: 0x06000094 RID: 148 RVA: 0x00007304 File Offset: 0x00005504
		[DataSourceProperty]
		public bool IsNight
		{
			get
			{
				return this._isNight;
			}
			set
			{
				if (value != this._isNight)
				{
					this._isNight = value;
					base.OnPropertyChangedWithValue(value, "IsNight");
					foreach (ShipItemVM shipItemVM in this.AllShips)
					{
						shipItemVM.IsNight = value;
					}
				}
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000095 RID: 149 RVA: 0x00007374 File Offset: 0x00005574
		// (set) Token: 0x06000096 RID: 150 RVA: 0x0000737C File Offset: 0x0000557C
		[DataSourceProperty]
		public ShipRosterVM LeftRoster
		{
			get
			{
				return this._leftRoster;
			}
			set
			{
				if (value != this._leftRoster)
				{
					this._leftRoster = value;
					base.OnPropertyChangedWithValue<ShipRosterVM>(value, "LeftRoster");
				}
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000097 RID: 151 RVA: 0x0000739A File Offset: 0x0000559A
		// (set) Token: 0x06000098 RID: 152 RVA: 0x000073A2 File Offset: 0x000055A2
		[DataSourceProperty]
		public ShipRosterVM RightRoster
		{
			get
			{
				return this._rightRoster;
			}
			set
			{
				if (value != this._rightRoster)
				{
					this._rightRoster = value;
					base.OnPropertyChangedWithValue<ShipRosterVM>(value, "RightRoster");
				}
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000099 RID: 153 RVA: 0x000073C0 File Offset: 0x000055C0
		// (set) Token: 0x0600009A RID: 154 RVA: 0x000073C8 File Offset: 0x000055C8
		[DataSourceProperty]
		public ShipItemVM SelectedShip
		{
			get
			{
				return this._selectedShip;
			}
			set
			{
				if (value != this._selectedShip)
				{
					if (this._selectedShip != null)
					{
						this._selectedShip.IsSelected = false;
					}
					this._selectedShip = value;
					base.OnPropertyChangedWithValue<ShipItemVM>(value, "SelectedShip");
					if (this._selectedShip != null)
					{
						this._selectedShip.IsSelected = true;
					}
				}
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600009B RID: 155 RVA: 0x00007419 File Offset: 0x00005619
		// (set) Token: 0x0600009C RID: 156 RVA: 0x00007421 File Offset: 0x00005621
		[DataSourceProperty]
		public ShipUpgradeSlotBaseVM SelectedUpgradeSlot
		{
			get
			{
				return this._selectedUpgradeSlot;
			}
			set
			{
				if (value != this._selectedUpgradeSlot)
				{
					this._selectedUpgradeSlot = value;
					base.OnPropertyChangedWithValue<ShipUpgradeSlotBaseVM>(value, "SelectedUpgradeSlot");
					this.IsAnyUpgradeSlotSelected = this._selectedUpgradeSlot != null;
				}
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600009D RID: 157 RVA: 0x0000744E File Offset: 0x0000564E
		// (set) Token: 0x0600009E RID: 158 RVA: 0x00007456 File Offset: 0x00005656
		[DataSourceProperty]
		public ShipUpgradePieceBaseVM InspectedUpgrade
		{
			get
			{
				return this._inspectedUpgrade;
			}
			set
			{
				if (value != this._inspectedUpgrade)
				{
					this._inspectedUpgrade = value;
					base.OnPropertyChangedWithValue<ShipUpgradePieceBaseVM>(value, "InspectedUpgrade");
				}
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600009F RID: 159 RVA: 0x00007474 File Offset: 0x00005674
		// (set) Token: 0x060000A0 RID: 160 RVA: 0x0000747C File Offset: 0x0000567C
		[DataSourceProperty]
		public HintViewModel CanConfirmHint
		{
			get
			{
				return this._canConfirmHint;
			}
			set
			{
				if (value != this._canConfirmHint)
				{
					this._canConfirmHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "CanConfirmHint");
				}
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000A1 RID: 161 RVA: 0x0000749A File Offset: 0x0000569A
		// (set) Token: 0x060000A2 RID: 162 RVA: 0x000074A2 File Offset: 0x000056A2
		[DataSourceProperty]
		public BasicTooltipViewModel GoldCostHint
		{
			get
			{
				return this._goldCostHint;
			}
			set
			{
				if (value != this._goldCostHint)
				{
					this._goldCostHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "GoldCostHint");
				}
			}
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x000074C0 File Offset: 0x000056C0
		public void SetResetInputKey(HotKey hotKey)
		{
			this.ResetInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x000074CF File Offset: 0x000056CF
		public void SetCancelInputKey(HotKey hotKey)
		{
			this.CancelInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x000074DE File Offset: 0x000056DE
		public void SetDoneInputKey(HotKey hotKey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x000074ED File Offset: 0x000056ED
		public void SetSelectPreviousShipInputKey(HotKey hotKey)
		{
			this.SelectPreviousShipInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x000074FC File Offset: 0x000056FC
		public void SetSelectNextShipInputKey(HotKey hotKey)
		{
			this.SelectNextShipInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x0000750B File Offset: 0x0000570B
		public void SetSelectLeftRosterInputKey(HotKey hotKey)
		{
			this.SelectLeftRosterInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x0000751A File Offset: 0x0000571A
		public void SetSelectRightRosterInputKey(HotKey hotKey)
		{
			this.SelectRightRosterInputKey = InputKeyItemVM.CreateFromHotKey(hotKey, true);
		}

		// Token: 0x060000AA RID: 170 RVA: 0x0000752C File Offset: 0x0000572C
		public void SetGamepadToggleCameraInputKey(HotKey hotKey)
		{
			InputKeyItemVM inputKeyItemVM = InputKeyItemVM.CreateFromHotKey(hotKey, true);
			this.GamepadCameraControlKeys.Add(inputKeyItemVM);
			this.GamepadToggleCameraInputKey = inputKeyItemVM;
			this.UpdateGamepadCameraControlButtonsVisibility();
		}

		// Token: 0x060000AB RID: 171 RVA: 0x0000755C File Offset: 0x0000575C
		public void AddGamepadCameraControlInputKey(HotKey hotKey)
		{
			InputKeyItemVM inputKeyItemVM = InputKeyItemVM.CreateFromHotKey(hotKey, true);
			this.GamepadCameraControlKeys.Add(inputKeyItemVM);
			this.UpdateGamepadCameraControlButtonsVisibility();
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00007584 File Offset: 0x00005784
		public void AddGamepadCameraControlInputKey(GameAxisKey gameAxisKey)
		{
			TextObject textObject = GameTexts.FindText("str_key_name", typeof(PortHotKeyCategory).Name + "_" + gameAxisKey.Id);
			InputKeyItemVM inputKeyItemVM = InputKeyItemVM.CreateFromForcedID(gameAxisKey.AxisKey.ToString(), textObject, true);
			this.GamepadCameraControlKeys.Add(inputKeyItemVM);
			this.UpdateGamepadCameraControlButtonsVisibility();
		}

		// Token: 0x060000AD RID: 173 RVA: 0x000075E0 File Offset: 0x000057E0
		public void AddKeyboardMoveCameraInputKey(GameKey gameKey)
		{
			InputKeyItemVM inputKeyItemVM = InputKeyItemVM.CreateFromGameKey(gameKey, false);
			this.KeyboardMoveCameraInputKeys.Add(inputKeyItemVM);
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00007604 File Offset: 0x00005804
		public void SetKeyboardRotateCameraInputKey(HotKey hotKey)
		{
			TextObject textObject = GameTexts.FindText("str_key_name", typeof(PortHotKeyCategory).Name + "_CameraAxisX");
			InputKeyItemVM inputKeyItemVM = InputKeyItemVM.CreateFromForcedID(hotKey.ToString(), textObject, false);
			this.KeyboardRotateCameraInputKey = inputKeyItemVM;
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000AF RID: 175 RVA: 0x0000764A File Offset: 0x0000584A
		// (set) Token: 0x060000B0 RID: 176 RVA: 0x00007652 File Offset: 0x00005852
		[DataSourceProperty]
		public InputKeyItemVM ResetInputKey
		{
			get
			{
				return this._resetInputKey;
			}
			set
			{
				if (value != this._resetInputKey)
				{
					this._resetInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "ResetInputKey");
				}
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000B1 RID: 177 RVA: 0x00007670 File Offset: 0x00005870
		// (set) Token: 0x060000B2 RID: 178 RVA: 0x00007678 File Offset: 0x00005878
		[DataSourceProperty]
		public InputKeyItemVM CancelInputKey
		{
			get
			{
				return this._cancelInputKey;
			}
			set
			{
				if (value != this._cancelInputKey)
				{
					this._cancelInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "CancelInputKey");
				}
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x00007696 File Offset: 0x00005896
		// (set) Token: 0x060000B4 RID: 180 RVA: 0x0000769E File Offset: 0x0000589E
		[DataSourceProperty]
		public InputKeyItemVM DoneInputKey
		{
			get
			{
				return this._doneInputKey;
			}
			set
			{
				if (value != this._doneInputKey)
				{
					this._doneInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "DoneInputKey");
				}
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x000076BC File Offset: 0x000058BC
		// (set) Token: 0x060000B6 RID: 182 RVA: 0x000076C4 File Offset: 0x000058C4
		[DataSourceProperty]
		public InputKeyItemVM SelectPreviousShipInputKey
		{
			get
			{
				return this._selectPreviousShipInputKey;
			}
			set
			{
				if (value != this._selectPreviousShipInputKey)
				{
					this._selectPreviousShipInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "SelectPreviousShipInputKey");
				}
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000B7 RID: 183 RVA: 0x000076E2 File Offset: 0x000058E2
		// (set) Token: 0x060000B8 RID: 184 RVA: 0x000076EA File Offset: 0x000058EA
		[DataSourceProperty]
		public InputKeyItemVM SelectNextShipInputKey
		{
			get
			{
				return this._selectNextShipInputKey;
			}
			set
			{
				if (value != this._selectNextShipInputKey)
				{
					this._selectNextShipInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "SelectNextShipInputKey");
				}
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x00007708 File Offset: 0x00005908
		// (set) Token: 0x060000BA RID: 186 RVA: 0x00007710 File Offset: 0x00005910
		[DataSourceProperty]
		public InputKeyItemVM SelectLeftRosterInputKey
		{
			get
			{
				return this._selectLeftRosterInputKey;
			}
			set
			{
				if (value != this._selectLeftRosterInputKey)
				{
					this._selectLeftRosterInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "SelectLeftRosterInputKey");
				}
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000BB RID: 187 RVA: 0x0000772E File Offset: 0x0000592E
		// (set) Token: 0x060000BC RID: 188 RVA: 0x00007736 File Offset: 0x00005936
		[DataSourceProperty]
		public InputKeyItemVM SelectRightRosterInputKey
		{
			get
			{
				return this._selectRightRosterInputKey;
			}
			set
			{
				if (value != this._selectRightRosterInputKey)
				{
					this._selectRightRosterInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "SelectRightRosterInputKey");
				}
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000BD RID: 189 RVA: 0x00007754 File Offset: 0x00005954
		// (set) Token: 0x060000BE RID: 190 RVA: 0x0000775C File Offset: 0x0000595C
		[DataSourceProperty]
		public InputKeyItemVM GamepadToggleCameraInputKey
		{
			get
			{
				return this._gamepadToggleCameraInputKey;
			}
			set
			{
				if (value != this._gamepadToggleCameraInputKey)
				{
					this._gamepadToggleCameraInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "GamepadToggleCameraInputKey");
				}
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000BF RID: 191 RVA: 0x0000777A File Offset: 0x0000597A
		// (set) Token: 0x060000C0 RID: 192 RVA: 0x00007782 File Offset: 0x00005982
		[DataSourceProperty]
		public MBBindingList<InputKeyItemVM> GamepadCameraControlKeys
		{
			get
			{
				return this._gamepadCameraControlKeys;
			}
			set
			{
				if (value != this._gamepadCameraControlKeys)
				{
					this._gamepadCameraControlKeys = value;
					base.OnPropertyChangedWithValue<MBBindingList<InputKeyItemVM>>(value, "GamepadCameraControlKeys");
				}
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000C1 RID: 193 RVA: 0x000077A0 File Offset: 0x000059A0
		// (set) Token: 0x060000C2 RID: 194 RVA: 0x000077A8 File Offset: 0x000059A8
		[DataSourceProperty]
		public MBBindingList<InputKeyItemVM> KeyboardMoveCameraInputKeys
		{
			get
			{
				return this._keyboardMoveCameraInputKeys;
			}
			set
			{
				if (value != this._keyboardMoveCameraInputKeys)
				{
					this._keyboardMoveCameraInputKeys = value;
					base.OnPropertyChangedWithValue<MBBindingList<InputKeyItemVM>>(value, "KeyboardMoveCameraInputKeys");
				}
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000C3 RID: 195 RVA: 0x000077C6 File Offset: 0x000059C6
		// (set) Token: 0x060000C4 RID: 196 RVA: 0x000077CE File Offset: 0x000059CE
		[DataSourceProperty]
		public InputKeyItemVM KeyboardRotateCameraInputKey
		{
			get
			{
				return this._keyboardRotateCameraInputKey;
			}
			set
			{
				if (value != this._keyboardRotateCameraInputKey)
				{
					this._keyboardRotateCameraInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "KeyboardRotateCameraInputKey");
				}
			}
		}

		// Token: 0x04000011 RID: 17
		private readonly PortScreenHandler _portScreenHandler;

		// Token: 0x04000012 RID: 18
		private readonly PortScreenModes _portScreenMode;

		// Token: 0x04000013 RID: 19
		private readonly Action<Ship> _onShipSelected;

		// Token: 0x04000014 RID: 20
		private readonly Action _onRostersRefreshed;

		// Token: 0x04000015 RID: 21
		private readonly Action<ShipItemVM> _refreshShipVisual;

		// Token: 0x04000016 RID: 22
		private readonly Action _onUpgradeSlotSelected;

		// Token: 0x04000017 RID: 23
		private readonly MBList<ShipItemVM> _allShips;

		// Token: 0x04000018 RID: 24
		private List<PortChangeInfo> _cachedChanges;

		// Token: 0x04000019 RID: 25
		private PortActionVM _buyAction;

		// Token: 0x0400001A RID: 26
		private PortActionVM _sellAction;

		// Token: 0x0400001B RID: 27
		private PortActionVM _repairAction;

		// Token: 0x0400001C RID: 28
		private PortActionVM _sendToClanAction;

		// Token: 0x0400001D RID: 29
		private PortActionVM _repairAllAction;

		// Token: 0x0400001E RID: 30
		private bool _isConfirmDisabled;

		// Token: 0x0400001F RID: 31
		private bool _canUseKeyboardInputs;

		// Token: 0x04000020 RID: 32
		private bool _canUseGamepadInputs;

		// Token: 0x04000021 RID: 33
		private bool _isControllingCamera;

		// Token: 0x04000022 RID: 34
		private bool _canToggleCamera = true;

		// Token: 0x04000023 RID: 35
		private bool _isMapBarExtended;

		// Token: 0x04000024 RID: 36
		private bool _isAnyUpgradeSlotSelected;

		// Token: 0x04000025 RID: 37
		private bool _isNight;

		// Token: 0x04000026 RID: 38
		private int _totalGoldCost;

		// Token: 0x04000027 RID: 39
		private string _keyboardMoveCameraText;

		// Token: 0x04000028 RID: 40
		private string _cancelText;

		// Token: 0x04000029 RID: 41
		private string _confirmText;

		// Token: 0x0400002A RID: 42
		private string _totalGoldCostText;

		// Token: 0x0400002B RID: 43
		private string _repairText;

		// Token: 0x0400002C RID: 44
		private string _upgradeText;

		// Token: 0x0400002D RID: 45
		private string _buyText;

		// Token: 0x0400002E RID: 46
		private string _sellText;

		// Token: 0x0400002F RID: 47
		private HintViewModel _canConfirmHint;

		// Token: 0x04000030 RID: 48
		private BasicTooltipViewModel _goldCostHint;

		// Token: 0x04000031 RID: 49
		private ShipRosterVM _leftRoster;

		// Token: 0x04000032 RID: 50
		private ShipRosterVM _rightRoster;

		// Token: 0x04000033 RID: 51
		private ShipItemVM _selectedShip;

		// Token: 0x04000034 RID: 52
		private ShipUpgradePieceBaseVM _inspectedUpgrade;

		// Token: 0x04000035 RID: 53
		private ShipUpgradeSlotBaseVM _selectedUpgradeSlot;

		// Token: 0x04000036 RID: 54
		private InputKeyItemVM _resetInputKey;

		// Token: 0x04000037 RID: 55
		private InputKeyItemVM _cancelInputKey;

		// Token: 0x04000038 RID: 56
		private InputKeyItemVM _doneInputKey;

		// Token: 0x04000039 RID: 57
		private InputKeyItemVM _selectPreviousShipInputKey;

		// Token: 0x0400003A RID: 58
		private InputKeyItemVM _selectNextShipInputKey;

		// Token: 0x0400003B RID: 59
		private InputKeyItemVM _selectLeftRosterInputKey;

		// Token: 0x0400003C RID: 60
		private InputKeyItemVM _selectRightRosterInputKey;

		// Token: 0x0400003D RID: 61
		private InputKeyItemVM _gamepadToggleCameraInputKey;

		// Token: 0x0400003E RID: 62
		private MBBindingList<InputKeyItemVM> _gamepadCameraControlKeys;

		// Token: 0x0400003F RID: 63
		private InputKeyItemVM _keyboardRotateCameraInputKey;

		// Token: 0x04000040 RID: 64
		private MBBindingList<InputKeyItemVM> _keyboardMoveCameraInputKeys;
	}
}
