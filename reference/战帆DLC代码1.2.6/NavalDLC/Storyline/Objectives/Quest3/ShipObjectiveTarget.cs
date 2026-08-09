using System;
using NavalDLC.Missions.Objects;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Quest3
{
	// Token: 0x0200005E RID: 94
	internal class ShipObjectiveTarget : MissionObjectiveTarget
	{
		// Token: 0x060005CE RID: 1486 RVA: 0x00022D09 File Offset: 0x00020F09
		internal ShipObjectiveTarget(MissionShip ship, TextObject name, bool showController = false)
		{
			this._ship = ship;
			this._name = name;
			this._showController = showController;
		}

		// Token: 0x060005CF RID: 1487 RVA: 0x00022D28 File Offset: 0x00020F28
		public override Vec3 GetGlobalPosition()
		{
			if (this._showController)
			{
				return this._ship.ShipControllerMachine.GameEntity.GlobalPosition + Vec3.Up;
			}
			return this._ship.GameEntity.GlobalPosition + Vec3.Up * 3f;
		}

		// Token: 0x060005D0 RID: 1488 RVA: 0x00022D87 File Offset: 0x00020F87
		public override TextObject GetName()
		{
			return this._name;
		}

		// Token: 0x060005D1 RID: 1489 RVA: 0x00022D8F File Offset: 0x00020F8F
		public override bool IsActive()
		{
			return this._ship != null && !this._ship.IsDisabled && (!this._showController || !this._ship.IsPlayerControlled);
		}

		// Token: 0x040002CB RID: 715
		private readonly MissionShip _ship;

		// Token: 0x040002CC RID: 716
		private readonly TextObject _name;

		// Token: 0x040002CD RID: 717
		private readonly bool _showController;
	}
}
