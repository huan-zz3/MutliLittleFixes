using System;
using NavalDLC.Missions.Objects.UsableMachines;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Quest4
{
	// Token: 0x02000057 RID: 87
	public class MangonelObjectiveTarget : MissionObjectiveTarget
	{
		// Token: 0x0600059A RID: 1434 RVA: 0x000225E7 File Offset: 0x000207E7
		public MangonelObjectiveTarget(ShipMangonel shipMangonel)
		{
			this._shipMangonel = shipMangonel;
		}

		// Token: 0x0600059B RID: 1435 RVA: 0x000225F8 File Offset: 0x000207F8
		public override bool IsActive()
		{
			ShipMangonel shipMangonel = this._shipMangonel;
			if (shipMangonel == null)
			{
				return false;
			}
			DestructableComponent destructionComponent = shipMangonel.DestructionComponent;
			bool? flag = ((destructionComponent != null) ? new bool?(destructionComponent.IsDestroyed) : null);
			bool flag2 = false;
			return (flag.GetValueOrDefault() == flag2) & (flag != null);
		}

		// Token: 0x0600059C RID: 1436 RVA: 0x00022644 File Offset: 0x00020844
		public override TextObject GetName()
		{
			return new TextObject("{=NbpcDXtJ}Mangonel", null);
		}

		// Token: 0x0600059D RID: 1437 RVA: 0x00022654 File Offset: 0x00020854
		public override Vec3 GetGlobalPosition()
		{
			return this._shipMangonel.GameEntity.GlobalPosition + Vec3.Up * 7f;
		}

		// Token: 0x040002BB RID: 699
		private readonly ShipMangonel _shipMangonel;
	}
}
