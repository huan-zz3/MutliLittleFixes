using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.ShipControl;
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
	// Token: 0x02000070 RID: 112
	public class Quest5WanderingShipsMissionLogic : MissionLogic
	{
		// Token: 0x0600078B RID: 1931 RVA: 0x00035214 File Offset: 0x00033414
		public override void EarlyStart()
		{
			base.Mission.Teams.Add(0, Clan.PlayerClan.Color, Clan.PlayerClan.Color2, Clan.PlayerClan.Banner, true, false, true);
			base.Mission.PlayerTeam = base.Mission.DefenderTeam;
		}

		// Token: 0x0600078C RID: 1932 RVA: 0x0003526A File Offset: 0x0003346A
		public override void AfterStart()
		{
			this._navalShipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
			this._navalAgentsLogic = base.Mission.GetMissionBehavior<NavalAgentsLogic>();
			this._navalAgentsLogic.UpdateTeamAgentsData();
			this.SetupPropShips();
		}

		// Token: 0x0600078D RID: 1933 RVA: 0x0003529F File Offset: 0x0003349F
		private void SetupPropShips()
		{
			this.InitializeWaypoints();
			this.SpawnPropShips();
		}

		// Token: 0x0600078E RID: 1934 RVA: 0x000352B0 File Offset: 0x000334B0
		private void InitializeWaypoints()
		{
			for (int i = 1; i <= 6; i++)
			{
				GameEntity gameEntity = Mission.Current.Scene.FindEntityWithTag("propship_1_waypoint_" + i);
				this._wayPoints1.Add(gameEntity);
			}
			for (int j = 1; j <= 6; j++)
			{
				GameEntity gameEntity2 = Mission.Current.Scene.FindEntityWithTag("propship_2_waypoint_" + j);
				this._wayPoints2.Add(gameEntity2);
			}
		}

		// Token: 0x0600078F RID: 1935 RVA: 0x00035330 File Offset: 0x00033530
		private void SpawnPropShips()
		{
			this._propShip1 = this.CreateShip("nord_medium_ship", "propship_1_waypoint_1", base.Mission.PlayerAllyTeam.GetFormation(0), false, null, null);
			this._propShip1.SetController(ShipControllerType.AI, true);
			this.SpawnPropShipAgents(this._propShip1, "gangster_1");
			this._propShip2 = this.CreateShip("eastern_heavy_ship", "propship_2_waypoint_1", base.Mission.PlayerAllyTeam.GetFormation(2), false, null, null);
			this._propShip2.SetController(ShipControllerType.AI, true);
			this.SpawnPropShipAgents(this._propShip2, "gangster_1");
		}

		// Token: 0x06000790 RID: 1936 RVA: 0x000353D0 File Offset: 0x000335D0
		private MissionShip CreateShip(string shipHullId, string spawnPointId, Formation formation, bool spawnAnchored = false, List<KeyValuePair<string, string>> additionalUpgradePieces = null, Figurehead figurehead = null)
		{
			GameEntity gameEntity = Mission.Current.Scene.FindEntityWithTag(spawnPointId);
			MatrixFrame globalFrame = gameEntity.GetGlobalFrame();
			float waterLevelAtPosition = Mission.Current.Scene.GetWaterLevelAtPosition(gameEntity.GlobalPosition.AsVec2, false, false);
			globalFrame.origin = new Vec3(gameEntity.GlobalPosition.x, gameEntity.GlobalPosition.y, waterLevelAtPosition, -1f);
			Ship ship = new Ship(Campaign.Current.ObjectManager.GetObject<ShipHull>(shipHullId));
			if (additionalUpgradePieces != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in additionalUpgradePieces)
				{
					ShipUpgradePiece @object = MBObjectManager.Instance.GetObject<ShipUpgradePiece>(keyValuePair.Value);
					ship.EquipUpgradePiece(keyValuePair.Key, @object);
				}
			}
			if (figurehead != null)
			{
				ship.ChangeFigurehead(figurehead);
			}
			MissionShip missionShip = this._navalShipsLogic.SpawnShip(ship, in globalFrame, formation.Team, formation, spawnAnchored, 8, true);
			missionShip.ShipOrder.FormationJoinShip(formation);
			return missionShip;
		}

		// Token: 0x06000791 RID: 1937 RVA: 0x000354E8 File Offset: 0x000336E8
		private void SpawnPropShipAgents(MissionShip ship, string troopType)
		{
			int num = ship.CrewSizeOnMainDeck / 2;
			NavalAgentsLogic missionBehavior = base.Mission.GetMissionBehavior<NavalAgentsLogic>();
			missionBehavior.SetDesiredTroopCountOfShip(ship, num);
			BasicCharacterObject @object = Campaign.Current.ObjectManager.GetObject<CharacterObject>(troopType);
			List<MatrixFrame> list = ship.OuterDeckLocalFrames.Concat<MatrixFrame>(ship.InnerDeckLocalFrames).ToList<MatrixFrame>();
			int num2 = 0;
			while (num2 < list.Count<MatrixFrame>() && num2 < num)
			{
				MatrixFrame matrixFrame = list[num2];
				Vec3 origin = matrixFrame.origin;
				Vec2 asVec = matrixFrame.rotation.f.AsVec2;
				AgentBuildData agentBuildData = new AgentBuildData(@object).TroopOrigin(new SimpleAgentOrigin(@object, -1, null, default(UniqueTroopDescriptor))).Team(ship.Team).InitialPosition(ref origin)
					.InitialDirection(ref asVec)
					.NoHorses(true)
					.NoWeapons(false);
				Agent agent = Mission.Current.SpawnAgent(agentBuildData, false);
				missionBehavior.AddAgentToShip(agent, ship);
				agent.SetAgentFlags(agent.GetAgentFlags() & -65537);
				agent.ToggleInvulnerable();
				num2++;
			}
		}

		// Token: 0x06000792 RID: 1938 RVA: 0x000355FA File Offset: 0x000337FA
		public override void OnMissionTick(float dt)
		{
			this.HandlePropShipOrders();
		}

		// Token: 0x06000793 RID: 1939 RVA: 0x00035604 File Offset: 0x00033804
		private void HandlePropShipOrders()
		{
			if (!Extensions.IsEmpty<GameEntity>(this._wayPoints1))
			{
				GameEntity gameEntity = this._wayPoints1[this._currentWaypointIndex1];
				if ((gameEntity.GlobalPosition - this._propShip1.GlobalFrame.origin).LengthSquared <= 100f)
				{
					this._currentWaypointIndex1 = (this._currentWaypointIndex1 + 1) % 6;
					gameEntity = this._wayPoints1[this._currentWaypointIndex1];
				}
				ShipOrder shipOrder = this._propShip1.ShipOrder;
				Vec2 vec = gameEntity.GlobalPosition.AsVec2;
				shipOrder.SetShipMovementOrder(in vec);
			}
			if (!Extensions.IsEmpty<GameEntity>(this._wayPoints2))
			{
				GameEntity gameEntity2 = this._wayPoints2[this._currentWaypointIndex2];
				if ((gameEntity2.GlobalPosition - this._propShip2.GlobalFrame.origin).LengthSquared <= 100f)
				{
					this._currentWaypointIndex2 = (this._currentWaypointIndex2 + 1) % 6;
					gameEntity2 = this._wayPoints2[this._currentWaypointIndex2];
				}
				ShipOrder shipOrder2 = this._propShip2.ShipOrder;
				Vec2 vec = gameEntity2.GlobalPosition.AsVec2;
				shipOrder2.SetShipMovementOrder(in vec);
			}
		}

		// Token: 0x06000794 RID: 1940 RVA: 0x00035729 File Offset: 0x00033929
		public void OnPhase2Started()
		{
			if (this._propShip1 != null)
			{
				this._navalShipsLogic.RemoveShip(this._propShip1);
			}
			if (this._propShip2 != null)
			{
				this._navalShipsLogic.RemoveShip(this._propShip2);
			}
		}

		// Token: 0x04000496 RID: 1174
		private const string PropShip1StringId = "nord_medium_ship";

		// Token: 0x04000497 RID: 1175
		private const string PropShip2StringId = "eastern_heavy_ship";

		// Token: 0x04000498 RID: 1176
		private const string PropShipTroopStringId = "gangster_1";

		// Token: 0x04000499 RID: 1177
		private const int WayPoint1Count = 6;

		// Token: 0x0400049A RID: 1178
		private const int WayPoint2Count = 6;

		// Token: 0x0400049B RID: 1179
		private const float WayPointSuccessDistance = 10f;

		// Token: 0x0400049C RID: 1180
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x0400049D RID: 1181
		private NavalAgentsLogic _navalAgentsLogic;

		// Token: 0x0400049E RID: 1182
		private MissionShip _propShip1;

		// Token: 0x0400049F RID: 1183
		private MissionShip _propShip2;

		// Token: 0x040004A0 RID: 1184
		private List<GameEntity> _wayPoints1 = new List<GameEntity>();

		// Token: 0x040004A1 RID: 1185
		private List<GameEntity> _wayPoints2 = new List<GameEntity>();

		// Token: 0x040004A2 RID: 1186
		private int _currentWaypointIndex1;

		// Token: 0x040004A3 RID: 1187
		private int _currentWaypointIndex2;
	}
}
