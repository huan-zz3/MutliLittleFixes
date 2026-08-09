using System;
using NavalDLC.Missions.AI.UsableMachineAIs;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Objects.Siege;

namespace NavalDLC.Missions.Objects
{
	// Token: 0x020000A2 RID: 162
	public class ShipBallistaSpawner : BallistaSpawner
	{
		// Token: 0x06000CAB RID: 3243 RVA: 0x000616D9 File Offset: 0x0005F8D9
		protected override void OnPreInit()
		{
			this._spawnerMissionHelper = new ShipSpawnerEntityMissionHelper(this, false);
		}

		// Token: 0x06000CAC RID: 3244 RVA: 0x000616E8 File Offset: 0x0005F8E8
		public override void AssignParameters(SpawnerEntityMissionHelper _spawnerMissionHelper)
		{
			base.AssignParameters(_spawnerMissionHelper);
			if (Mission.Current != null)
			{
				Ballista firstScriptOfType = _spawnerMissionHelper.SpawnedEntity.GetFirstScriptOfType<Ballista>();
				firstScriptOfType.SetAI(new ShipBallistaAI(firstScriptOfType));
			}
		}
	}
}
