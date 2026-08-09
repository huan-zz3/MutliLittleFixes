using System;
using System.Collections.Generic;
using NavalDLC.Missions.MissionLogics;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.View
{
	// Token: 0x0200000A RID: 10
	public class NavalShipsPreloadView : MissionView
	{
		// Token: 0x0600004D RID: 77 RVA: 0x00003790 File Offset: 0x00001990
		public override void OnBehaviorInitialize()
		{
			Mission.Current.Scene.SetDoNotAddEntitiesToTickList(true);
			DefaultNavalMissionLogic missionBehavior = base.Mission.GetMissionBehavior<DefaultNavalMissionLogic>();
			if (missionBehavior != null)
			{
				if (missionBehavior.PlayerShips != null)
				{
					foreach (IShipOrigin shipOrigin in missionBehavior.PlayerShips)
					{
						this.PreloadShip(shipOrigin);
					}
				}
				if (missionBehavior.PlayerAllyShips != null)
				{
					foreach (IShipOrigin shipOrigin2 in missionBehavior.PlayerAllyShips)
					{
						this.PreloadShip(shipOrigin2);
					}
				}
				if (missionBehavior.PlayerEnemyShips != null)
				{
					foreach (IShipOrigin shipOrigin3 in missionBehavior.PlayerEnemyShips)
					{
						this.PreloadShip(shipOrigin3);
					}
				}
				this._helperInstance.PreloadMeshesAndPhysics();
			}
			Mission.Current.Scene.SetDoNotAddEntitiesToTickList(false);
		}

		// Token: 0x0600004E RID: 78 RVA: 0x000038C0 File Offset: 0x00001AC0
		public override void OnSceneRenderingStarted()
		{
			this._helperInstance.WaitForMeshesToBeLoaded();
		}

		// Token: 0x0600004F RID: 79 RVA: 0x000038D0 File Offset: 0x00001AD0
		public void PreloadShip(IShipOrigin ship)
		{
			MissionShipObject @object = MBObjectManager.Instance.GetObject<MissionShipObject>(ship.OriginShipId);
			GameEntity gameEntity = GameEntity.InstantiateWithRestOffset(base.Mission.Scene, @object.Prefab, true, MatrixFrame.Identity, -0.1f, false, "");
			MissionShipFactory.CleanNonExistingUpgrades(gameEntity.WeakEntity, ship.GetShipVisualSlotInfos());
			List<WeakGameEntity> list = new List<WeakGameEntity>();
			gameEntity.WeakEntity.GetChildrenRecursive(ref list);
			list.Add(gameEntity.WeakEntity);
			this._helperInstance.PreloadEntities(list);
			gameEntity.Remove(76);
		}

		// Token: 0x0400001C RID: 28
		private PreloadHelper _helperInstance = new PreloadHelper();
	}
}
