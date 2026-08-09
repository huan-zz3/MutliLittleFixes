using System;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Objects.Siege;

namespace NavalDLC.Missions.Objects
{
	// Token: 0x020000A8 RID: 168
	internal class ShipSpawnerEntityMissionHelper : SpawnerEntityMissionHelper
	{
		// Token: 0x06000CF6 RID: 3318 RVA: 0x000646D9 File Offset: 0x000628D9
		public ShipSpawnerEntityMissionHelper(SpawnerBase spawner, bool fireVersion = false)
			: base(spawner, fireVersion)
		{
		}

		// Token: 0x06000CF7 RID: 3319 RVA: 0x000646E3 File Offset: 0x000628E3
		protected override void InstantiateEntity(GameEntity parent, string entityName)
		{
			this.SpawnedEntity = GameEntity.Instantiate(parent.Scene, entityName, true, true, "");
		}
	}
}
