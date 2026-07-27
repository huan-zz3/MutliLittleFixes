using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace NavalDLC.GauntletUI.Widgets.Widgets
{
	// Token: 0x02000007 RID: 7
	public class PortScreenWidget : Widget
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000022 RID: 34 RVA: 0x000027E8 File Offset: 0x000009E8
		// (set) Token: 0x06000023 RID: 35 RVA: 0x000027F0 File Offset: 0x000009F0
		public float AlphaChangeDuration { get; set; } = 0.15f;

		// Token: 0x06000024 RID: 36 RVA: 0x000027F9 File Offset: 0x000009F9
		public PortScreenWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002830 File Offset: 0x00000A30
		protected override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this.IsAnyUpgradeSlotSelected)
			{
				Widget upgradesPanel = this.UpgradesPanel;
				if (upgradesPanel == null || !upgradesPanel.IsPointInsideMeasuredArea(base.EventManager.MousePosition))
				{
					Widget slotsPanel = this.SlotsPanel;
					if (slotsPanel == null || !slotsPanel.IsPointInsideMeasuredArea(base.EventManager.MousePosition))
					{
						this.HandleClickOutside();
					}
				}
			}
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002898 File Offset: 0x00000A98
		private void HandleClickOutside()
		{
			InputKey[] clickKeys = base.Context.InputContext.GetClickKeys();
			for (int i = 0; i < clickKeys.Length; i++)
			{
				if (Input.IsKeyPressed(clickKeys[i]))
				{
					base.EventFired("DeselectSlot", Array.Empty<object>());
					return;
				}
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x000028E0 File Offset: 0x00000AE0
		protected override void OnLateUpdate(float dt)
		{
			if (this._isTransitioning)
			{
				if (this._alphaChangeTimeElapsed < this.AlphaChangeDuration)
				{
					this._currentAlpha = MathF.Lerp(this._initialAlpha, this._targetAlpha, this._alphaChangeTimeElapsed / this.AlphaChangeDuration, 1E-05f);
					Widget topPanel = this.TopPanel;
					if (topPanel != null)
					{
						GauntletExtensions.SetGlobalAlphaRecursively(topPanel, this._currentAlpha);
					}
					Widget bottomPanel = this.BottomPanel;
					if (bottomPanel != null)
					{
						GauntletExtensions.SetGlobalAlphaRecursively(bottomPanel, this._currentAlpha);
					}
					Widget leftPanel = this.LeftPanel;
					if (leftPanel != null)
					{
						GauntletExtensions.SetGlobalAlphaRecursively(leftPanel, this._currentAlpha);
					}
					Widget rightPanel = this.RightPanel;
					if (rightPanel != null)
					{
						GauntletExtensions.SetGlobalAlphaRecursively(rightPanel, this._currentAlpha);
					}
					this._alphaChangeTimeElapsed += dt;
				}
				else
				{
					this._currentAlpha = this._targetAlpha;
					Widget topPanel2 = this.TopPanel;
					if (topPanel2 != null)
					{
						GauntletExtensions.SetGlobalAlphaRecursively(topPanel2, this._currentAlpha);
					}
					Widget bottomPanel2 = this.BottomPanel;
					if (bottomPanel2 != null)
					{
						GauntletExtensions.SetGlobalAlphaRecursively(bottomPanel2, this._currentAlpha);
					}
					Widget leftPanel2 = this.LeftPanel;
					if (leftPanel2 != null)
					{
						GauntletExtensions.SetGlobalAlphaRecursively(leftPanel2, this._currentAlpha);
					}
					Widget rightPanel2 = this.RightPanel;
					if (rightPanel2 != null)
					{
						GauntletExtensions.SetGlobalAlphaRecursively(rightPanel2, this._currentAlpha);
					}
					this._isTransitioning = false;
				}
			}
			if (this.InspectionPanelWidget != null)
			{
				this.UpdateInspectionPanelWidget();
			}
			if (this.UpgradesPanel != null && this.UpgradesPanelArrowWidget != null)
			{
				this.UpdateUpgradesPanelArrowWidget();
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002A34 File Offset: 0x00000C34
		private void UpdateInspectionPanelWidget()
		{
			List<Widget> mouseOveredWidgets = base.EventManager.MouseOveredWidgets;
			for (int i = 0; i < mouseOveredWidgets.Count; i++)
			{
				PortInspectionParentWidget portInspectionParentWidget;
				if ((portInspectionParentWidget = mouseOveredWidgets[i] as PortInspectionParentWidget) != null)
				{
					this.InspectionPanelWidget.SetTargetPiece(portInspectionParentWidget);
					return;
				}
			}
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002A7C File Offset: 0x00000C7C
		private void UpdateUpgradesPanelArrowWidget()
		{
			Widget widget = null;
			List<PortInspectionParentWidget> allChildrenOfTypeRecursive = this.SlotsPanel.GetAllChildrenOfTypeRecursive<PortInspectionParentWidget>(null);
			for (int i = 0; i < allChildrenOfTypeRecursive.Count; i++)
			{
				PortInspectionParentWidget portInspectionParentWidget = allChildrenOfTypeRecursive[i];
				if (portInspectionParentWidget.GetFirstInChildrenRecursive(delegate(Widget x)
				{
					ButtonWidget buttonWidget;
					return (buttonWidget = x as ButtonWidget) != null && buttonWidget.IsSelected;
				}) != null)
				{
					widget = portInspectionParentWidget;
					break;
				}
			}
			this.UpgradesPanelArrowWidget.SetTargetSlot(widget);
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002AE8 File Offset: 0x00000CE8
		private void OnCameraControlsEnabledChanged()
		{
			this._alphaChangeTimeElapsed = 0f;
			this._targetAlpha = (this.IsControllingCamera ? this.CameraEnabledAlpha : 1f);
			this._initialAlpha = this._currentAlpha;
			this._isTransitioning = true;
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600002B RID: 43 RVA: 0x00002B23 File Offset: 0x00000D23
		// (set) Token: 0x0600002C RID: 44 RVA: 0x00002B2B File Offset: 0x00000D2B
		[Editor(false)]
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
					base.OnPropertyChanged(value, "IsAnyUpgradeSlotSelected");
				}
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00002B49 File Offset: 0x00000D49
		// (set) Token: 0x0600002E RID: 46 RVA: 0x00002B51 File Offset: 0x00000D51
		[Editor(false)]
		public Widget UpgradesPanel
		{
			get
			{
				return this._upgradesPanel;
			}
			set
			{
				if (value != this._upgradesPanel)
				{
					this._upgradesPanel = value;
					base.OnPropertyChanged<Widget>(value, "UpgradesPanel");
				}
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600002F RID: 47 RVA: 0x00002B6F File Offset: 0x00000D6F
		// (set) Token: 0x06000030 RID: 48 RVA: 0x00002B77 File Offset: 0x00000D77
		[Editor(false)]
		public Widget SlotsPanel
		{
			get
			{
				return this._slotsPanel;
			}
			set
			{
				if (value != this._slotsPanel)
				{
					this._slotsPanel = value;
					base.OnPropertyChanged<Widget>(value, "SlotsPanel");
				}
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000031 RID: 49 RVA: 0x00002B95 File Offset: 0x00000D95
		// (set) Token: 0x06000032 RID: 50 RVA: 0x00002B9D File Offset: 0x00000D9D
		[Editor(false)]
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
					base.OnPropertyChanged(value, "IsControllingCamera");
					this.OnCameraControlsEnabledChanged();
				}
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000033 RID: 51 RVA: 0x00002BC1 File Offset: 0x00000DC1
		// (set) Token: 0x06000034 RID: 52 RVA: 0x00002BC9 File Offset: 0x00000DC9
		[Editor(false)]
		public float CameraEnabledAlpha
		{
			get
			{
				return this._cameraEnabledAlpha;
			}
			set
			{
				if (value != this._cameraEnabledAlpha)
				{
					this._cameraEnabledAlpha = value;
					base.OnPropertyChanged(value, "CameraEnabledAlpha");
				}
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000035 RID: 53 RVA: 0x00002BE7 File Offset: 0x00000DE7
		// (set) Token: 0x06000036 RID: 54 RVA: 0x00002BEF File Offset: 0x00000DEF
		[Editor(false)]
		public Widget TopPanel
		{
			get
			{
				return this._topPanel;
			}
			set
			{
				if (value != this._topPanel)
				{
					this._topPanel = value;
					base.OnPropertyChanged<Widget>(value, "TopPanel");
				}
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000037 RID: 55 RVA: 0x00002C0D File Offset: 0x00000E0D
		// (set) Token: 0x06000038 RID: 56 RVA: 0x00002C15 File Offset: 0x00000E15
		[Editor(false)]
		public Widget BottomPanel
		{
			get
			{
				return this._bottomPanel;
			}
			set
			{
				if (value != this._bottomPanel)
				{
					this._bottomPanel = value;
					base.OnPropertyChanged<Widget>(value, "BottomPanel");
				}
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000039 RID: 57 RVA: 0x00002C33 File Offset: 0x00000E33
		// (set) Token: 0x0600003A RID: 58 RVA: 0x00002C3B File Offset: 0x00000E3B
		[Editor(false)]
		public Widget LeftPanel
		{
			get
			{
				return this._leftPanel;
			}
			set
			{
				if (value != this._leftPanel)
				{
					this._leftPanel = value;
					base.OnPropertyChanged<Widget>(value, "LeftPanel");
				}
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600003B RID: 59 RVA: 0x00002C59 File Offset: 0x00000E59
		// (set) Token: 0x0600003C RID: 60 RVA: 0x00002C61 File Offset: 0x00000E61
		[Editor(false)]
		public Widget RightPanel
		{
			get
			{
				return this._rightPanel;
			}
			set
			{
				if (value != this._rightPanel)
				{
					this._rightPanel = value;
					base.OnPropertyChanged<Widget>(value, "RightPanel");
				}
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600003D RID: 61 RVA: 0x00002C7F File Offset: 0x00000E7F
		// (set) Token: 0x0600003E RID: 62 RVA: 0x00002C87 File Offset: 0x00000E87
		[Editor(false)]
		public PortPieceInspectionWidget InspectionPanelWidget
		{
			get
			{
				return this._inspectionPanelWidget;
			}
			set
			{
				if (value != this._inspectionPanelWidget)
				{
					this._inspectionPanelWidget = value;
					base.OnPropertyChanged<PortPieceInspectionWidget>(value, "InspectionPanelWidget");
				}
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600003F RID: 63 RVA: 0x00002CA5 File Offset: 0x00000EA5
		// (set) Token: 0x06000040 RID: 64 RVA: 0x00002CAD File Offset: 0x00000EAD
		[Editor(false)]
		public PortUpgradesPanelArrowWidget UpgradesPanelArrowWidget
		{
			get
			{
				return this._upgradesPanelArrowWidget;
			}
			set
			{
				if (value != this._upgradesPanelArrowWidget)
				{
					this._upgradesPanelArrowWidget = value;
					base.OnPropertyChanged<PortUpgradesPanelArrowWidget>(value, "UpgradesPanelArrowWidget");
				}
			}
		}

		// Token: 0x0400000F RID: 15
		private float _alphaChangeTimeElapsed;

		// Token: 0x04000010 RID: 16
		private float _initialAlpha = 1f;

		// Token: 0x04000011 RID: 17
		private float _targetAlpha;

		// Token: 0x04000012 RID: 18
		private float _currentAlpha = 1f;

		// Token: 0x04000013 RID: 19
		private bool _isTransitioning;

		// Token: 0x04000014 RID: 20
		private bool _isAnyUpgradeSlotSelected;

		// Token: 0x04000015 RID: 21
		private Widget _upgradesPanel;

		// Token: 0x04000016 RID: 22
		private Widget _slotsPanel;

		// Token: 0x04000017 RID: 23
		private bool _isControllingCamera;

		// Token: 0x04000018 RID: 24
		private float _cameraEnabledAlpha = 0.2f;

		// Token: 0x04000019 RID: 25
		private Widget _topPanel;

		// Token: 0x0400001A RID: 26
		private Widget _bottomPanel;

		// Token: 0x0400001B RID: 27
		private Widget _leftPanel;

		// Token: 0x0400001C RID: 28
		private Widget _rightPanel;

		// Token: 0x0400001D RID: 29
		private PortPieceInspectionWidget _inspectionPanelWidget;

		// Token: 0x0400001E RID: 30
		private PortUpgradesPanelArrowWidget _upgradesPanelArrowWidget;
	}
}
