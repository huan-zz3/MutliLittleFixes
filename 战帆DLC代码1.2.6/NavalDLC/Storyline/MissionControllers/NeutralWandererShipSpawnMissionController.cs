using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions;
using NavalDLC.Missions.AI.Tactics;
using NavalDLC.Missions.AI.TeamAI;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.Storyline.MissionControllers
{
	// Token: 0x0200006C RID: 108
	public class NeutralWandererShipSpawnMissionController : MissionLogic
	{
		// Token: 0x06000692 RID: 1682 RVA: 0x00027995 File Offset: 0x00025B95
		public override void OnAfterMissionCreated()
		{
			base.OnAfterMissionCreated();
		}

		// Token: 0x06000693 RID: 1683 RVA: 0x000279A0 File Offset: 0x00025BA0
		public override void AfterStart()
		{
			base.AfterStart();
			this._navalShipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
			this._navalAgentsLogic = base.Mission.GetMissionBehavior<NavalAgentsLogic>();
			Team playerAllyTeam = base.Mission.PlayerAllyTeam;
			this._availableNeutralFormations.Enqueue(playerAllyTeam.GetFormation(0));
			this._availableNeutralFormations.Enqueue(playerAllyTeam.GetFormation(1));
			this._availableNeutralFormations.Enqueue(playerAllyTeam.GetFormation(2));
			this._availableNeutralFormations.Enqueue(playerAllyTeam.GetFormation(3));
			this._availableNeutralFormations.Enqueue(playerAllyTeam.GetFormation(4));
			this._availableNeutralFormations.Enqueue(playerAllyTeam.GetFormation(5));
			this._availableNeutralFormations.Enqueue(playerAllyTeam.GetFormation(6));
			this._availableNeutralFormations.Enqueue(playerAllyTeam.GetFormation(7));
			this._availableNeutralFormations.Enqueue(playerAllyTeam.GetFormation(8));
			this._availableNeutralFormations.Enqueue(playerAllyTeam.GetFormation(9));
			playerAllyTeam.SetIsEnemyOf(Mission.GetTeam(2), false);
			playerAllyTeam.SetIsEnemyOf(Mission.GetTeam(0), false);
			this.CollectWandererShipData();
			this._currentState = NeutralWandererShipSpawnMissionController.WandererShipControllerState.SpawnShips;
		}

		// Token: 0x06000694 RID: 1684 RVA: 0x00027AC0 File Offset: 0x00025CC0
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			switch (this._currentState)
			{
			case NeutralWandererShipSpawnMissionController.WandererShipControllerState.None:
			case NeutralWandererShipSpawnMissionController.WandererShipControllerState.End:
				break;
			case NeutralWandererShipSpawnMissionController.WandererShipControllerState.SpawnShips:
				this.SpawnWandererShips();
				this._currentState = NeutralWandererShipSpawnMissionController.WandererShipControllerState.SpawnTroops;
				return;
			case NeutralWandererShipSpawnMissionController.WandererShipControllerState.SpawnTroops:
				this.SpawnWandererShipTroops();
				this._currentState = NeutralWandererShipSpawnMissionController.WandererShipControllerState.MoveShips;
				return;
			case NeutralWandererShipSpawnMissionController.WandererShipControllerState.MoveShips:
				this.HandleWandererShipMovements();
				break;
			default:
				return;
			}
		}

		// Token: 0x06000695 RID: 1685 RVA: 0x00027B18 File Offset: 0x00025D18
		private void CollectWandererShipData()
		{
			foreach (GameEntity gameEntity in Mission.Current.Scene.FindEntitiesWithTagExpression("wanderer_ship(_\\d+)*_spawnpoint"))
			{
				int num = int.Parse(gameEntity.Tags.FirstOrDefault<string>().Split(new char[] { '_' })[2]);
				this._wandererShipData.Add(new NeutralWandererShipSpawnMissionController.WandererShipData(num, gameEntity));
			}
			Dictionary<int, List<GameEntity>> dictionary = new Dictionary<int, List<GameEntity>>();
			foreach (GameEntity gameEntity2 in Mission.Current.Scene.FindEntitiesWithTagExpression("wanderer_ship(_\\d+)*_target(_\\d+)*"))
			{
				int num2 = int.Parse(gameEntity2.Tags.FirstOrDefault<string>().Split(new char[] { '_' })[2]);
				if (!dictionary.ContainsKey(num2))
				{
					dictionary[num2] = new List<GameEntity>();
				}
				dictionary[num2].Add(gameEntity2);
			}
			using (Dictionary<int, List<GameEntity>>.Enumerator enumerator2 = dictionary.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					KeyValuePair<int, List<GameEntity>> targetKvp = enumerator2.Current;
					GameEntity[] array = new GameEntity[targetKvp.Value.Count];
					foreach (GameEntity gameEntity3 in targetKvp.Value)
					{
						string[] array2 = gameEntity3.Tags.FirstOrDefault<string>().Split(new char[] { '_' });
						int num3 = int.Parse(array2[array2.Length - 1]);
						array[num3 - 1] = gameEntity3;
					}
					NeutralWandererShipSpawnMissionController.WandererShipData wandererShipData = this._wandererShipData.First<NeutralWandererShipSpawnMissionController.WandererShipData>((NeutralWandererShipSpawnMissionController.WandererShipData d) => d.TagNumber == targetKvp.Key);
					foreach (GameEntity gameEntity4 in array)
					{
						wandererShipData.AddTargetPoint(gameEntity4);
					}
				}
			}
		}

		// Token: 0x06000696 RID: 1686 RVA: 0x00027D50 File Offset: 0x00025F50
		private void SpawnWandererShips()
		{
			foreach (NeutralWandererShipSpawnMissionController.WandererShipData wandererShipData in this._wandererShipData)
			{
				if (!Extensions.IsEmpty<Formation>(this._availableNeutralFormations))
				{
					MissionShip missionShip = this.CreateShip(Extensions.GetRandomElement<string>(this._wandererShipIdList), wandererShipData.SpawnPointEntity, this._availableNeutralFormations.Dequeue());
					wandererShipData.SetWandererShip(missionShip);
				}
			}
		}

		// Token: 0x06000697 RID: 1687 RVA: 0x00027DD4 File Offset: 0x00025FD4
		private MissionShip CreateShip(string shipHullId, GameEntity spawnPoint, Formation formation)
		{
			MatrixFrame globalFrame = spawnPoint.GetGlobalFrame();
			float waterLevelAtPosition = Mission.Current.Scene.GetWaterLevelAtPosition(spawnPoint.GlobalPosition.AsVec2, false, false);
			globalFrame.origin = new Vec3(spawnPoint.GlobalPosition.x, spawnPoint.GlobalPosition.y, waterLevelAtPosition, -1f);
			Ship ship = new Ship(Campaign.Current.ObjectManager.GetObject<ShipHull>(shipHullId));
			MissionShip missionShip = this._navalShipsLogic.SpawnShip(ship, in globalFrame, formation.Team, formation, false, 8, true);
			missionShip.ShipOrder.FormationJoinShip(formation);
			return missionShip;
		}

		// Token: 0x06000698 RID: 1688 RVA: 0x00027E6C File Offset: 0x0002606C
		private void SpawnWandererShipTroops()
		{
			Team playerAllyTeam = base.Mission.PlayerAllyTeam;
			TeamAINavalComponent teamAINavalComponent = new TeamAINavalComponent(base.Mission, playerAllyTeam, 5f, 1f);
			playerAllyTeam.AddTeamAI(teamAINavalComponent, false);
			playerAllyTeam.AddTacticOption(new TacticNavalBalancedOffense(playerAllyTeam));
			this._navalAgentsLogic.SetDeploymentMode(true);
			this._navalShipsLogic.SetDeploymentMode(true);
			foreach (NeutralWandererShipSpawnMissionController.WandererShipData wandererShipData in this._wandererShipData)
			{
				CharacterObject @object = MBObjectManager.Instance.GetObject<CharacterObject>(Extensions.GetRandomElement<string>(this._wandererShipTroopIdList));
				int num = MBRandom.RandomInt(7, 13);
				this._navalAgentsLogic.SetDesiredTroopCountOfShip(wandererShipData.WandererShip, num);
				for (int i = 0; i < num; i++)
				{
					this._navalAgentsLogic.AddReservedTroopToShip(new SimpleAgentOrigin(@object, -1, null, default(UniqueTroopDescriptor)), wandererShipData.WandererShip);
				}
			}
			this._navalAgentsLogic.SetDeploymentMode(false);
			this._navalShipsLogic.SetDeploymentMode(false);
		}

		// Token: 0x06000699 RID: 1689 RVA: 0x00027F8C File Offset: 0x0002618C
		private void HandleWandererShipMovements()
		{
			foreach (NeutralWandererShipSpawnMissionController.WandererShipData wandererShipData in this._wandererShipData)
			{
				if (wandererShipData.CurrentTarget == null || wandererShipData.WandererShip.GlobalFrame.origin.Distance(wandererShipData.CurrentTarget.GlobalPosition) <= 100f)
				{
					wandererShipData.ChangeToNextTarget();
				}
				else
				{
					ShipOrder shipOrder = wandererShipData.WandererShip.ShipOrder;
					Vec2 asVec = wandererShipData.CurrentTarget.GlobalPosition.AsVec2;
					shipOrder.SetShipMovementOrder(in asVec);
				}
			}
		}

		// Token: 0x04000358 RID: 856
		private const string WandererShipSpawnPointTagExpression = "wanderer_ship(_\\d+)*_spawnpoint";

		// Token: 0x04000359 RID: 857
		private const string WandererShipTargetPointTagExpression = "wanderer_ship(_\\d+)*_target(_\\d+)*";

		// Token: 0x0400035A RID: 858
		private readonly List<string> _wandererShipIdList = new List<string> { "western_trade_ship_storyline", "sturgia_heavy_ship", "ship_lodya_storyline", "ship_birlinn_storyline" };

		// Token: 0x0400035B RID: 859
		private readonly List<string> _wandererShipTroopIdList = new List<string> { "sea_hounds", "gangradirs_kin_melee" };

		// Token: 0x0400035C RID: 860
		private readonly List<NeutralWandererShipSpawnMissionController.WandererShipData> _wandererShipData = new List<NeutralWandererShipSpawnMissionController.WandererShipData>();

		// Token: 0x0400035D RID: 861
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x0400035E RID: 862
		private NavalAgentsLogic _navalAgentsLogic;

		// Token: 0x0400035F RID: 863
		private Queue<Formation> _availableNeutralFormations = new Queue<Formation>();

		// Token: 0x04000360 RID: 864
		private NeutralWandererShipSpawnMissionController.WandererShipControllerState _currentState;

		// Token: 0x020001D8 RID: 472
		private class WandererShipData
		{
			// Token: 0x17000402 RID: 1026
			// (get) Token: 0x06001A59 RID: 6745 RVA: 0x000AF25E File Offset: 0x000AD45E
			// (set) Token: 0x06001A5A RID: 6746 RVA: 0x000AF266 File Offset: 0x000AD466
			public MissionShip WandererShip { get; private set; }

			// Token: 0x17000403 RID: 1027
			// (get) Token: 0x06001A5B RID: 6747 RVA: 0x000AF26F File Offset: 0x000AD46F
			// (set) Token: 0x06001A5C RID: 6748 RVA: 0x000AF277 File Offset: 0x000AD477
			public GameEntity CurrentTarget { get; private set; }

			// Token: 0x06001A5D RID: 6749 RVA: 0x000AF280 File Offset: 0x000AD480
			public WandererShipData(int tagNumber, GameEntity spawnPointEntity)
			{
				this.TagNumber = tagNumber;
				this.SpawnPointEntity = spawnPointEntity;
			}

			// Token: 0x06001A5E RID: 6750 RVA: 0x000AF2A1 File Offset: 0x000AD4A1
			public void AddTargetPoint(GameEntity targetPoint)
			{
				this._targetPoints.Add(targetPoint);
			}

			// Token: 0x06001A5F RID: 6751 RVA: 0x000AF2AF File Offset: 0x000AD4AF
			public void SetWandererShip(MissionShip ship)
			{
				this.WandererShip = ship;
			}

			// Token: 0x06001A60 RID: 6752 RVA: 0x000AF2B8 File Offset: 0x000AD4B8
			public void ChangeToNextTarget()
			{
				if (this.CurrentTarget == null)
				{
					this.CurrentTarget = this._targetPoints[0];
					return;
				}
				if (this._isTargetReversed)
				{
					int i = this._targetPoints.Count - 1;
					while (i >= 0)
					{
						if (this._targetPoints[i] == this.CurrentTarget)
						{
							if (i == 0)
							{
								this._isTargetReversed = false;
								this.CurrentTarget = this._targetPoints[i + 1];
								return;
							}
							this.CurrentTarget = this._targetPoints[i - 1];
							return;
						}
						else
						{
							i--;
						}
					}
					return;
				}
				int j = 0;
				while (j < this._targetPoints.Count)
				{
					if (this._targetPoints[j] == this.CurrentTarget)
					{
						if (j == this._targetPoints.Count - 1)
						{
							this._isTargetReversed = true;
							this.CurrentTarget = this._targetPoints[j - 1];
							return;
						}
						this.CurrentTarget = this._targetPoints[j + 1];
						return;
					}
					else
					{
						j++;
					}
				}
			}

			// Token: 0x04000D6B RID: 3435
			public readonly int TagNumber;

			// Token: 0x04000D6C RID: 3436
			public readonly GameEntity SpawnPointEntity;

			// Token: 0x04000D6D RID: 3437
			private readonly List<GameEntity> _targetPoints = new List<GameEntity>();

			// Token: 0x04000D70 RID: 3440
			private bool _isTargetReversed;
		}

		// Token: 0x020001D9 RID: 473
		private enum WandererShipControllerState
		{
			// Token: 0x04000D72 RID: 3442
			None,
			// Token: 0x04000D73 RID: 3443
			SpawnShips,
			// Token: 0x04000D74 RID: 3444
			SpawnTroops,
			// Token: 0x04000D75 RID: 3445
			MoveShips,
			// Token: 0x04000D76 RID: 3446
			End
		}
	}
}
