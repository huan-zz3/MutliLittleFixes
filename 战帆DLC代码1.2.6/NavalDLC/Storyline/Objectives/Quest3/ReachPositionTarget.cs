using System;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Quest3
{
	// Token: 0x0200005C RID: 92
	internal class ReachPositionTarget : MissionObjectiveTarget
	{
		// Token: 0x060005C2 RID: 1474 RVA: 0x00022A81 File Offset: 0x00020C81
		internal ReachPositionTarget(Vec3 escapePosition, TextObject name)
		{
			this._name = name;
			this._position = escapePosition;
		}

		// Token: 0x060005C3 RID: 1475 RVA: 0x00022A97 File Offset: 0x00020C97
		public override Vec3 GetGlobalPosition()
		{
			return this._position + Vec3.Up * 3f;
		}

		// Token: 0x060005C4 RID: 1476 RVA: 0x00022AB3 File Offset: 0x00020CB3
		public override TextObject GetName()
		{
			return this._name;
		}

		// Token: 0x060005C5 RID: 1477 RVA: 0x00022ABB File Offset: 0x00020CBB
		public override bool IsActive()
		{
			return true;
		}

		// Token: 0x040002C6 RID: 710
		private readonly Vec3 _position;

		// Token: 0x040002C7 RID: 711
		private readonly TextObject _name;
	}
}
