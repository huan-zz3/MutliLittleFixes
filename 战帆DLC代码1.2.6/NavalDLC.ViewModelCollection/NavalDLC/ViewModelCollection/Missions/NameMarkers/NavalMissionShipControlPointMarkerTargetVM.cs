using System;
using NavalDLC.Missions.Objects.UsableMachines;
using SandBox.ViewModelCollection.Missions.NameMarker;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.ViewModelCollection.Missions.NameMarkers
{
	// Token: 0x0200002A RID: 42
	public class NavalMissionShipControlPointMarkerTargetVM : MissionNameMarkerTargetVM<ShipControllerMachine>
	{
		// Token: 0x060003CF RID: 975 RVA: 0x00012BAA File Offset: 0x00010DAA
		public NavalMissionShipControlPointMarkerTargetVM(ShipControllerMachine target)
			: base(target)
		{
			base.NameType = "Normal";
			base.IconType = "control_point";
			this.RefreshValues();
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x00012BD0 File Offset: 0x00010DD0
		public override void UpdatePosition(Camera missionCamera)
		{
			if (Agent.Main == null || !base.Target.IsStandingPointAvailableForAgent(Agent.Main))
			{
				base.ScreenPosition = new Vec2(-5000f, -5000f);
				base.Distance = -1;
				return;
			}
			if (base.Target.HandTargetEntity != null)
			{
				base.UpdatePositionWith(missionCamera, base.Target.HandTargetEntity.GlobalPosition + base.Target.HandTargetEntity.GetGlobalFrame().rotation.u * 1.5f);
				return;
			}
			if (base.Target.ControllerEntity != null)
			{
				base.UpdatePositionWith(missionCamera, base.Target.ControllerEntity.GlobalPosition + base.Target.ControllerEntity.GetGlobalFrame().rotation.u * 1.5f);
				return;
			}
			base.UpdatePositionWith(missionCamera, base.Target.GameEntity.GlobalPosition + base.Target.GameEntity.GetGlobalFrame().rotation.u * 1.5f);
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x00012D01 File Offset: 0x00010F01
		protected override TextObject GetName()
		{
			return new TextObject("{=OGY9BKOM}Control the Ship", null);
		}
	}
}
