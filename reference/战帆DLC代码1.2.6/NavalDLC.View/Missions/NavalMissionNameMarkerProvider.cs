using System;
using System.Collections.Generic;
using NavalDLC.Missions;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.Objects.UsableMachines;
using NavalDLC.ViewModelCollection.Missions.NameMarkers;
using SandBox.ViewModelCollection.Missions.NameMarker;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.View.Missions
{
	// Token: 0x0200002F RID: 47
	public class NavalMissionNameMarkerProvider : MissionNameMarkerProvider
	{
		// Token: 0x06000130 RID: 304 RVA: 0x00008FBD File Offset: 0x000071BD
		protected override void OnInitialize(Mission mission)
		{
			base.OnInitialize(mission);
			Agent main = Agent.Main;
			this._mainAgentNavalComponent = ((main != null) ? main.GetComponent<AgentNavalComponent>() : null);
			this._navalShipsLogic = mission.GetMissionBehavior<NavalShipsLogic>();
			mission.OnMainAgentChanged += new Mission.OnMainAgentChangedDelegate(this.OnMainAgentChanged);
		}

		// Token: 0x06000131 RID: 305 RVA: 0x00008FFB File Offset: 0x000071FB
		protected override void OnDestroy(Mission mission)
		{
			base.OnDestroy(mission);
			mission.OnMainAgentChanged -= new Mission.OnMainAgentChangedDelegate(this.OnMainAgentChanged);
		}

		// Token: 0x06000132 RID: 306 RVA: 0x00009016 File Offset: 0x00007216
		private void OnMainAgentChanged(Agent oldAgent)
		{
			Agent main = Agent.Main;
			this._mainAgentNavalComponent = ((main != null) ? main.GetComponent<AgentNavalComponent>() : null);
			base.SetMarkersDirty();
		}

		// Token: 0x06000133 RID: 307 RVA: 0x00009038 File Offset: 0x00007238
		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (this._mainAgentNavalComponent == null)
			{
				Agent main = Agent.Main;
				this._mainAgentNavalComponent = ((main != null) ? main.GetComponent<AgentNavalComponent>() : null);
			}
			AgentNavalComponent mainAgentNavalComponent = this._mainAgentNavalComponent;
			if (((mainAgentNavalComponent != null) ? mainAgentNavalComponent.SteppedShip : null) != this._lastSteppedShip)
			{
				AgentNavalComponent mainAgentNavalComponent2 = this._mainAgentNavalComponent;
				this._lastSteppedShip = ((mainAgentNavalComponent2 != null) ? mainAgentNavalComponent2.SteppedShip : null);
				base.SetMarkersDirty();
			}
			NavalShipsLogic navalShipsLogic = this._navalShipsLogic;
			if (((navalShipsLogic != null) ? navalShipsLogic.PlayerControlledShip : null) != this._lastControlledShip)
			{
				NavalShipsLogic navalShipsLogic2 = this._navalShipsLogic;
				this._lastControlledShip = ((navalShipsLogic2 != null) ? navalShipsLogic2.PlayerControlledShip : null);
				base.SetMarkersDirty();
			}
		}

		// Token: 0x06000134 RID: 308 RVA: 0x000090DC File Offset: 0x000072DC
		public override void CreateMarkers(List<MissionNameMarkerTargetBaseVM> markers)
		{
			if (this._lastSteppedShip != null && this._lastSteppedShip != this._lastControlledShip)
			{
				bool flag = false;
				ShipControllerMachine shipControllerMachine = this._lastSteppedShip.ShipControllerMachine;
				bool flag2 = false;
				for (int i = 0; i < markers.Count; i++)
				{
					NavalMissionShipControlPointMarkerTargetVM navalMissionShipControlPointMarkerTargetVM;
					if ((navalMissionShipControlPointMarkerTargetVM = markers[i] as NavalMissionShipControlPointMarkerTargetVM) != null && navalMissionShipControlPointMarkerTargetVM.Target == shipControllerMachine && navalMissionShipControlPointMarkerTargetVM.IsPersistent == flag)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					markers.Add(new NavalMissionShipControlPointMarkerTargetVM(shipControllerMachine)
					{
						IsPersistent = flag
					});
				}
			}
		}

		// Token: 0x0400007C RID: 124
		private MissionShip _lastSteppedShip;

		// Token: 0x0400007D RID: 125
		private MissionShip _lastControlledShip;

		// Token: 0x0400007E RID: 126
		private AgentNavalComponent _mainAgentNavalComponent;

		// Token: 0x0400007F RID: 127
		private NavalShipsLogic _navalShipsLogic;
	}
}
