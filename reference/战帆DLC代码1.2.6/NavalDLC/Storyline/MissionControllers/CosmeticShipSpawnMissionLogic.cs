using System;
using System.Collections.Generic;
using NavalDLC.Missions.Objects;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Objects;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.Storyline.MissionControllers
{
	// Token: 0x0200006A RID: 106
	public class CosmeticShipSpawnMissionLogic : MissionLogic
	{
		// Token: 0x06000686 RID: 1670 RVA: 0x00027484 File Offset: 0x00025684
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			foreach (GameEntity gameEntity in Mission.Current.Scene.FindEntitiesWithTag("cosmetic_ship_spawn_point"))
			{
				this._cosmeticShipSpawnPointEntities.Enqueue(gameEntity);
			}
		}

		// Token: 0x06000687 RID: 1671 RVA: 0x000274EC File Offset: 0x000256EC
		public override void AfterStart()
		{
			base.AfterStart();
			while (!Extensions.IsEmpty<GameEntity>(this._cosmeticShipSpawnPointEntities))
			{
				ShipHull @object = MBObjectManager.Instance.GetObject<ShipHull>(Extensions.GetRandomElement<string>(this._cosmeticShipIdList));
				this.SpawnShip(@object);
			}
		}

		// Token: 0x06000688 RID: 1672 RVA: 0x0002752C File Offset: 0x0002572C
		private void SpawnShip(ShipHull shipHull)
		{
			MissionShipObject @object = MBObjectManager.Instance.GetObject<MissionShipObject>(shipHull.MissionShipObjectId);
			uint num = 4291609515U;
			uint num2 = 4291609515U;
			GameEntity gameEntity = VisualShipFactory.CreateVisualShip(@object.Prefab, base.Mission.Scene, new List<ShipVisualSlotInfo>(), MBRandom.RandomInt(), 1f, num, num2, true);
			MatrixFrame globalFrame = this._cosmeticShipSpawnPointEntities.Dequeue().GetGlobalFrame();
			globalFrame.rotation.MakeUnit();
			float waterLevelAtPosition = base.Mission.Scene.GetWaterLevelAtPosition(globalFrame.origin.AsVec2, true, true);
			globalFrame.origin.z = waterLevelAtPosition;
			gameEntity.SetFrame(ref globalFrame, true);
			List<SailVisual> list = new List<SailVisual>();
			this.CollectSailVisuals(gameEntity.WeakEntity, list);
			this.FoldSails(list);
			this._spawnedShipVisuals.Add(gameEntity, globalFrame);
		}

		// Token: 0x06000689 RID: 1673 RVA: 0x000275FC File Offset: 0x000257FC
		private void CollectSailVisuals(WeakGameEntity shipEntity, List<SailVisual> sailVisuals)
		{
			sailVisuals.Clear();
			ShipVisual firstScriptOfType = shipEntity.GetFirstScriptOfType<ShipVisual>();
			if (firstScriptOfType != null)
			{
				using (List<ScriptComponentBehavior>.Enumerator enumerator = firstScriptOfType.SailVisuals.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SailVisual sailVisual;
						if ((sailVisual = enumerator.Current as SailVisual) != null)
						{
							sailVisual.SailEnabled = false;
							sailVisual.SetFoldSailStepMultiplier(0.3f);
							sailVisual.SetFoldSailDuration(0.4f);
							sailVisual.SetUnfoldSailDuration(0.2f);
							sailVisual.FoldAnimationEnabled = false;
							sailVisuals.Add(sailVisual);
						}
					}
				}
			}
		}

		// Token: 0x0600068A RID: 1674 RVA: 0x00027698 File Offset: 0x00025898
		private void FoldSails(List<SailVisual> sailVisuals)
		{
			foreach (SailVisual sailVisual in sailVisuals)
			{
				sailVisual.SailEnabled = false;
			}
		}

		// Token: 0x0400034E RID: 846
		private const string CosmeticShipSpawnPointTag = "cosmetic_ship_spawn_point";

		// Token: 0x0400034F RID: 847
		private const float AnimationSpeedMultiplier = 0.1f;

		// Token: 0x04000350 RID: 848
		private List<string> _cosmeticShipIdList = new List<string> { "nord_medium_ship", "khuzait_heavy_ship", "eastern_medium_ship", "empire_trade_ship" };

		// Token: 0x04000351 RID: 849
		private Queue<GameEntity> _cosmeticShipSpawnPointEntities = new Queue<GameEntity>();

		// Token: 0x04000352 RID: 850
		private Dictionary<GameEntity, MatrixFrame> _spawnedShipVisuals = new Dictionary<GameEntity, MatrixFrame>();
	}
}
