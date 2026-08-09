using System;
using System.Collections.Generic;
using NavalDLC.Missions;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.View.MissionViews;
using NavalDLC.ViewModelCollection.HUD.ShipMarker;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.ScreenSystem;

namespace NavalDLC.GauntletUI.MissionViews
{
	// Token: 0x0200001B RID: 27
	[OverrideView(typeof(NavalMissionShipMarkerUIHandler))]
	public class MissionGauntletNavalShipMarker : MissionBattleUIBaseView
	{
		// Token: 0x060000AF RID: 175 RVA: 0x000072C4 File Offset: 0x000054C4
		protected override void OnCreateView()
		{
			this._dataSource = new NavalShipMarkersVM(base.Mission);
			this._gauntletLayer = new GauntletLayer("NavalShipMarker", this.ViewOrderPriority, false);
			this._gauntletLayer.LoadMovie("NavalShipMarker", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
			this._shipTargetHandler = base.Mission.GetMissionBehavior<NavalShipTargetSelectionHandler>();
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			if (this._shipTargetHandler != null)
			{
				this._shipTargetHandler.OnShipsFocused += this.OnShipFocusedFromHandler;
			}
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Combine(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
			this.UpdateShowDistanceTexts();
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00007388 File Offset: 0x00005588
		protected override void OnDestroyView()
		{
			ManagedOptions.OnManagedOptionChanged = (ManagedOptions.OnManagedOptionChangedDelegate)Delegate.Remove(ManagedOptions.OnManagedOptionChanged, new ManagedOptions.OnManagedOptionChangedDelegate(this.OnManagedOptionChanged));
			if (this._shipTargetHandler != null)
			{
				this._shipTargetHandler.OnShipsFocused -= this.OnShipFocusedFromHandler;
			}
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x000073FE File Offset: 0x000055FE
		protected override void OnSuspendView()
		{
			if (this._gauntletLayer != null)
			{
				ScreenManager.SetSuspendLayer(this._gauntletLayer, true);
			}
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00007414 File Offset: 0x00005614
		protected override void OnResumeView()
		{
			if (this._gauntletLayer != null)
			{
				ScreenManager.SetSuspendLayer(this._gauntletLayer, false);
			}
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x0000742A File Offset: 0x0000562A
		private void OnManagedOptionChanged(ManagedOptions.ManagedOptionsType optionType)
		{
			if (optionType == 14)
			{
				this.UpdateShowDistanceTexts();
			}
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00007437 File Offset: 0x00005637
		private void UpdateShowDistanceTexts()
		{
			this._showDistanceTexts = ManagedOptions.GetConfig(14) > 1E-05f;
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00007450 File Offset: 0x00005650
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (base.IsViewCreated)
			{
				if (base.Mission.Mode != 6)
				{
					this._dataSource.IsEnabled = base.Input.IsGameKeyDown(5) || base.Mission.IsOrderMenuOpen;
				}
				this._dataSource.IsShipTargetingRelevant = this._shipTargetHandler != null && base.Mission.IsOrderMenuOpen;
				this._dataSource.ShowDistanceTexts = this._showDistanceTexts;
				if (this._dataSource.IsEnabled)
				{
					this._dataSource.RefreshShipMarkers();
					this.RefreshShipTargetProperties();
					this.UpdateMarkerPositions();
					this._fadeOutTimer = 2f;
					return;
				}
				if (this._fadeOutTimer >= 0f)
				{
					this._dataSource.RefreshShipMarkers();
					this._fadeOutTimer -= dt;
					this.UpdateMarkerPositions();
				}
			}
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00007534 File Offset: 0x00005734
		private void UpdateMarkerPositions()
		{
			for (int i = 0; i < this._dataSource.ShipMarkers.Count; i++)
			{
				NavalShipMarkerItemVM navalShipMarkerItemVM = this._dataSource.ShipMarkers[i];
				float num = 0f;
				float num2 = 0f;
				float num3 = 0f;
				Vec3 vec;
				if (navalShipMarkerItemVM.IsShipActive())
				{
					vec = navalShipMarkerItemVM.Ship.GlobalFrame.origin;
				}
				else
				{
					vec = navalShipMarkerItemVM.Formation.CachedMedianPosition.GetNavMeshVec3();
				}
				if (vec.IsValid)
				{
					MBWindowManager.WorldToScreen(base.MissionScreen.CombatCamera, vec + this._heightOffset, ref num, ref num2, ref num3);
					if (!MathF.IsValidValue(num3) || !MathF.IsValidValue(num) || !MathF.IsValidValue(num2))
					{
						num = -10000f;
						num2 = -10000f;
						num3 = -1f;
					}
					navalShipMarkerItemVM.WSign = ((num3 < 0f) ? (-1) : 1);
					navalShipMarkerItemVM.Distance = base.MissionScreen.CombatCamera.Position.Distance(vec);
					navalShipMarkerItemVM.ScreenPosition = new Vec2(num, num2);
					if (this._dataSource.ShowDistanceTexts)
					{
						NavalShipMarkerItemVM navalShipMarkerItemVM2 = navalShipMarkerItemVM;
						Agent main = Agent.Main;
						navalShipMarkerItemVM2.DistanceText = ((main != null && main.IsActive()) ? ((int)Agent.Main.Position.Distance(vec)).ToString() : ((int)navalShipMarkerItemVM.Distance).ToString());
					}
					else
					{
						navalShipMarkerItemVM.DistanceText = string.Empty;
					}
				}
				else
				{
					navalShipMarkerItemVM.WSign = -1;
					navalShipMarkerItemVM.Distance = 10000f;
					navalShipMarkerItemVM.DistanceText = string.Empty;
					navalShipMarkerItemVM.ScreenPosition = new Vec2(-10000f, -10000f);
				}
			}
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x000076F0 File Offset: 0x000058F0
		private unsafe void RefreshShipTargetProperties()
		{
			if (!this._dataSource.IsShipTargetingRelevant)
			{
				for (int i = 0; i < this._dataSource.ShipMarkers.Count; i++)
				{
					this._dataSource.ShipMarkers[i].SetTargetedState(false, false);
				}
				return;
			}
			List<MissionShip> list = new List<MissionShip>();
			List<Formation> list2 = new List<Formation>();
			Agent main = Agent.Main;
			MBReadOnlyList<Formation> mbreadOnlyList;
			if (main == null)
			{
				mbreadOnlyList = null;
			}
			else
			{
				OrderController playerOrderController = main.Team.PlayerOrderController;
				mbreadOnlyList = ((playerOrderController != null) ? playerOrderController.SelectedFormations : null);
			}
			MBReadOnlyList<Formation> mbreadOnlyList2 = mbreadOnlyList;
			if (mbreadOnlyList2 != null)
			{
				for (int j = 0; j < mbreadOnlyList2.Count; j++)
				{
					ShipAssignment shipAssignment = this._navalShipsLogic.GetShipAssignment(mbreadOnlyList2[j].Team.TeamSide, mbreadOnlyList2[j].FormationIndex);
					MissionShip missionShip = ((shipAssignment != null) ? shipAssignment.MissionShip : null);
					if (missionShip != null)
					{
						if (mbreadOnlyList2[j].TargetFormation != null)
						{
							MovementOrder movementOrder = *mbreadOnlyList2[j].GetReadonlyMovementOrderReference();
							if (movementOrder.OrderType == 4 || movementOrder.OrderType == 12)
							{
								list2.Add(mbreadOnlyList2[j].TargetFormation);
							}
						}
						if (missionShip.ShipOrder.MovementOrderEnum == ShipOrder.ShipMovementOrderEnum.Engage && missionShip.ShipOrder.TargetShip != null && !missionShip.ShipOrder.IsAutoSelectingTargetShip)
						{
							list.Add(missionShip.ShipOrder.TargetShip);
						}
					}
				}
			}
			for (int k = 0; k < this._dataSource.ShipMarkers.Count; k++)
			{
				NavalShipMarkerItemVM navalShipMarkerItemVM = this._dataSource.ShipMarkers[k];
				if (navalShipMarkerItemVM.TeamType == 2)
				{
					bool flag = list.Contains(navalShipMarkerItemVM.Ship) || list2.Contains(navalShipMarkerItemVM.Formation);
					NavalShipMarkerItemVM navalShipMarkerItemVM2 = navalShipMarkerItemVM;
					MBReadOnlyList<MissionShip> focusedShipsCache = this._focusedShipsCache;
					navalShipMarkerItemVM2.SetTargetedState(focusedShipsCache != null && focusedShipsCache.Contains(navalShipMarkerItemVM.Ship), flag);
				}
			}
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x000078D5 File Offset: 0x00005AD5
		private void OnShipFocusedFromHandler(MBReadOnlyList<MissionShip> focusedShips)
		{
			this._focusedShipsCache = focusedShips;
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x000078DE File Offset: 0x00005ADE
		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			if (base.IsViewCreated)
			{
				this._gauntletLayer.UIContext.ContextAlpha = 0f;
			}
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00007903 File Offset: 0x00005B03
		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			if (base.IsViewCreated)
			{
				this._gauntletLayer.UIContext.ContextAlpha = 1f;
			}
		}

		// Token: 0x0400005D RID: 93
		private NavalShipMarkersVM _dataSource;

		// Token: 0x0400005E RID: 94
		private GauntletLayer _gauntletLayer;

		// Token: 0x0400005F RID: 95
		private NavalShipTargetSelectionHandler _shipTargetHandler;

		// Token: 0x04000060 RID: 96
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x04000061 RID: 97
		private MBReadOnlyList<MissionShip> _focusedShipsCache;

		// Token: 0x04000062 RID: 98
		private readonly Vec3 _heightOffset = new Vec3(0f, 0f, 3f, -1f);

		// Token: 0x04000063 RID: 99
		private float _fadeOutTimer;

		// Token: 0x04000064 RID: 100
		private bool _showDistanceTexts;
	}
}
