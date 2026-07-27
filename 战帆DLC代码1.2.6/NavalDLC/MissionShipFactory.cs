using System;
using System.Collections.Generic;
using Helpers;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Objects;

namespace NavalDLC
{
	// Token: 0x0200001B RID: 27
	public class MissionShipFactory
	{
		// Token: 0x06000119 RID: 281 RVA: 0x000082D4 File Offset: 0x000064D4
		public static MissionObject CreateMissionShip(int shipIndex, ShipAssignment shipAssignment, NavalShipsLogic shipsLogic, in MatrixFrame initialFrame)
		{
			Debug.Print("MissionShipFactory.CreateMissionShip: " + shipAssignment.MissionShipObject.Prefab, 0, 12, 17592186044416UL);
			MissionObject missionObject = shipsLogic.Mission.CreateMissionObjectFromPrefab(shipAssignment.MissionShipObject.Prefab, initialFrame, true, -0.1f, delegate(GameEntity entity)
			{
				MissionShipFactory.CleanNonExistingUpgrades(entity.WeakEntity, shipAssignment.ShipOrigin.GetShipVisualSlotInfos());
				entity.CreateAndAddScriptComponent(typeof(ShipVisual).Name, false);
				ShipVisual firstScriptOfType2 = entity.GetFirstScriptOfType<ShipVisual>();
				firstScriptOfType2.SailColors = ShipHelper.GetSailColors(shipAssignment.ShipOrigin, null);
				firstScriptOfType2.Initialize(shipAssignment.ShipOrigin.RandomValue, shipAssignment.ShipOrigin.CustomSailPatternId);
				firstScriptOfType2.Health = shipAssignment.ShipOrigin.HitPoints / shipAssignment.ShipOrigin.MaxHitPoints;
			});
			MissionShip firstScriptOfType = missionObject.GameEntity.GetFirstScriptOfType<MissionShip>();
			firstScriptOfType.InitForMission(shipIndex, (ulong)MissionShipFactory._shipUniqueBitwiseIDNext, shipAssignment, shipsLogic);
			shipAssignment.SetMissionShip(firstScriptOfType);
			MissionShipFactory._shipUniqueBitwiseIDNext <<= 1;
			return missionObject;
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00008387 File Offset: 0x00006587
		public static void ResetShipUniqueBitwiseIDNext()
		{
			MissionShipFactory._shipUniqueBitwiseIDNext = 1U;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00008390 File Offset: 0x00006590
		public static void CleanNonExistingUpgrades(WeakGameEntity shipEntity, List<ShipVisualSlotInfo> upgrades)
		{
			List<WeakGameEntity> list = shipEntity.CollectChildrenEntitiesWithTag("upgrade_slot");
			List<WeakGameEntity> list2 = new List<WeakGameEntity>();
			for (int i = list.Count - 1; i >= 0; i--)
			{
				WeakGameEntity weakGameEntity = list[i];
				bool flag = false;
				foreach (ShipVisualSlotInfo shipVisualSlotInfo in upgrades)
				{
					if (weakGameEntity.HasTag(shipVisualSlotInfo.VisualSlotTag))
					{
						for (int j = weakGameEntity.ChildCount - 1; j >= 0; j--)
						{
							WeakGameEntity child = weakGameEntity.GetChild(j);
							if (!child.HasTag(shipVisualSlotInfo.VisualPieceId))
							{
								if (!child.HasTag("base"))
								{
									if (child.HasTag("platform"))
									{
										list2.Add(child);
									}
									else
									{
										child.Remove(77);
									}
								}
							}
							else
							{
								flag = true;
							}
						}
					}
				}
				bool flag2 = false;
				for (int k = weakGameEntity.ChildCount - 1; k >= 0; k--)
				{
					WeakGameEntity child2 = weakGameEntity.GetChild(k);
					if (child2.HasTag("base"))
					{
						if (flag)
						{
							child2.Remove(77);
						}
						flag2 = true;
					}
				}
				if (!flag)
				{
					foreach (WeakGameEntity weakGameEntity2 in list2)
					{
						weakGameEntity2.Remove(77);
					}
					if (flag2)
					{
						for (int l = weakGameEntity.ChildCount - 1; l >= 0; l--)
						{
							WeakGameEntity child3 = weakGameEntity.GetChild(l);
							if (!child3.HasTag("base"))
							{
								child3.Remove(77);
							}
						}
					}
					else
					{
						weakGameEntity.Remove(77);
					}
				}
				list2.Clear();
			}
		}

		// Token: 0x04000088 RID: 136
		private static uint _shipUniqueBitwiseIDNext = 1U;
	}
}
