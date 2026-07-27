using System;
using NavalDLC.Missions.Objects.UsableMachines;
using SandBox.ViewModelCollection.Missions.NameMarker;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.ViewModelCollection.Missions.NameMarkers
{
	// Token: 0x02000029 RID: 41
	public class AgentBindsMachineMarkerTargetVM : MissionNameMarkerTargetVM<AgentBindsMachine>
	{
		// Token: 0x060003CC RID: 972 RVA: 0x00012AEC File Offset: 0x00010CEC
		public AgentBindsMachineMarkerTargetVM(AgentBindsMachine target)
			: base(target)
		{
			base.NameType = "Normal";
			base.IconType = "prisoner";
			this.RefreshValues();
		}

		// Token: 0x060003CD RID: 973 RVA: 0x00012B14 File Offset: 0x00010D14
		public override void UpdatePosition(Camera missionCamera)
		{
			if (Agent.Main == null || !base.Target.IsStandingPointAvailableForAgent(Agent.Main))
			{
				base.ScreenPosition = new Vec2(-5000f, -5000f);
				base.Distance = -1;
				return;
			}
			base.UpdatePositionWith(missionCamera, base.Target.GameEntity.GlobalPosition + base.Target.GameEntity.GetGlobalFrame().rotation.u * 1.5f);
		}

		// Token: 0x060003CE RID: 974 RVA: 0x00012B9D File Offset: 0x00010D9D
		protected override TextObject GetName()
		{
			return new TextObject("{=mx9zqEzQ}Unchain", null);
		}
	}
}
