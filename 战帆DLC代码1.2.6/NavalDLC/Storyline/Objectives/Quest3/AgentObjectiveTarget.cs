using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Quest3
{
	// Token: 0x02000058 RID: 88
	internal class AgentObjectiveTarget : MissionObjectiveTarget
	{
		// Token: 0x0600059E RID: 1438 RVA: 0x00022688 File Offset: 0x00020888
		internal AgentObjectiveTarget(Agent agent)
		{
			this._agent = agent;
		}

		// Token: 0x0600059F RID: 1439 RVA: 0x00022697 File Offset: 0x00020897
		public override Vec3 GetGlobalPosition()
		{
			return this._agent.Position;
		}

		// Token: 0x060005A0 RID: 1440 RVA: 0x000226A4 File Offset: 0x000208A4
		public override TextObject GetName()
		{
			return this._agent.NameTextObject;
		}

		// Token: 0x060005A1 RID: 1441 RVA: 0x000226B1 File Offset: 0x000208B1
		public override bool IsActive()
		{
			return this._agent != null && this._agent.IsActive();
		}

		// Token: 0x040002BC RID: 700
		private readonly Agent _agent;
	}
}
