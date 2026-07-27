using System;
using NavalDLC.Missions;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.Objects.UsableMachines;
using NavalDLC.View.MissionViews;
using NavalDLC.ViewModelCollection.Missions.CaptureShip;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace NavalDLC.GauntletUI.MissionViews
{
	// Token: 0x02000017 RID: 23
	[OverrideView(typeof(NavalMissionCaptureShipView))]
	public class MissionGauntletNavalCaptureShipView : MissionView
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000085 RID: 133 RVA: 0x000064F4 File Offset: 0x000046F4
		// (set) Token: 0x06000086 RID: 134 RVA: 0x000064FC File Offset: 0x000046FC
		public ShipControllerMachine ControllerMachine { get; private set; }

		// Token: 0x06000087 RID: 135 RVA: 0x00006508 File Offset: 0x00004708
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._dataSource = new NavalMissionCaptureShipVM(3f);
			this._gauntletLayer = new GauntletLayer("NavalMissionCaptureShip", 47, false);
			this._gauntletLayer.LoadMovie("NavalMissionCaptureShip", this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00006566 File Offset: 0x00004766
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			this._gauntletLayer = null;
			this._dataSource.OnFinalize();
			this._dataSource = null;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00006588 File Offset: 0x00004788
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			Agent main = Agent.Main;
			ShipControllerMachine shipControllerMachine;
			if (main == null)
			{
				shipControllerMachine = null;
			}
			else
			{
				AgentNavalComponent component = main.GetComponent<AgentNavalComponent>();
				if (component == null)
				{
					shipControllerMachine = null;
				}
				else
				{
					MissionShip steppedShip = component.SteppedShip;
					shipControllerMachine = ((steppedShip != null) ? steppedShip.ShipControllerMachine : null);
				}
			}
			this.ControllerMachine = shipControllerMachine;
			if (this.ControllerMachine != null && Agent.Main != null && this.ControllerMachine.PilotAgent == Agent.Main)
			{
				this._dataSource.UpdateCaptureTimer(this.ControllerMachine.CaptureTimer);
				return;
			}
			this._dataSource.UpdateCaptureTimer(-1f);
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00006613 File Offset: 0x00004813
		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			if (this._gauntletLayer != null)
			{
				this._gauntletLayer.UIContext.ContextAlpha = 0f;
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00006638 File Offset: 0x00004838
		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			if (this._gauntletLayer != null)
			{
				this._gauntletLayer.UIContext.ContextAlpha = 1f;
			}
		}

		// Token: 0x0400004D RID: 77
		private GauntletLayer _gauntletLayer;

		// Token: 0x0400004E RID: 78
		private NavalMissionCaptureShipVM _dataSource;
	}
}
