using System;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Quest5
{
	// Token: 0x02000050 RID: 80
	public class Quest5ReachAlliesObjective : MissionObjective
	{
		// Token: 0x170000FE RID: 254
		// (get) Token: 0x0600056E RID: 1390 RVA: 0x0002223C File Offset: 0x0002043C
		public override string UniqueId
		{
			get
			{
				return "quest_5_reach_allies_objective";
			}
		}

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x0600056F RID: 1391 RVA: 0x00022243 File Offset: 0x00020443
		public override TextObject Name
		{
			get
			{
				return new TextObject("{=LBNwZ3HS}Keep Watch", null);
			}
		}

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x06000570 RID: 1392 RVA: 0x00022250 File Offset: 0x00020450
		public override TextObject Description
		{
			get
			{
				return new TextObject("{=araGPQbp}Keep watch for approaching enemy ships.", null);
			}
		}

		// Token: 0x06000571 RID: 1393 RVA: 0x0002225D File Offset: 0x0002045D
		public Quest5ReachAlliesObjective(Mission mission, VolumeBox targetVolumeBox)
			: base(mission)
		{
			this._targetVolumeBox = targetVolumeBox;
		}

		// Token: 0x06000572 RID: 1394 RVA: 0x0002226D File Offset: 0x0002046D
		protected override bool IsActivationRequirementsMet()
		{
			return true;
		}

		// Token: 0x06000573 RID: 1395 RVA: 0x00022270 File Offset: 0x00020470
		protected override bool IsCompletionRequirementsMet()
		{
			Agent main = Agent.Main;
			return main != null && main.IsActive() && this._targetVolumeBox.IsPointIn(Agent.Main.Position);
		}

		// Token: 0x040002B1 RID: 689
		private readonly VolumeBox _targetVolumeBox;
	}
}
