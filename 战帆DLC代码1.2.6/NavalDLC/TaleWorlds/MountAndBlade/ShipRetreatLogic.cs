using System;
using System.Collections.Generic;
using NavalDLC.Missions;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000011 RID: 17
	public class ShipRetreatLogic : MissionLogic
	{
		// Token: 0x06000096 RID: 150 RVA: 0x00006580 File Offset: 0x00004780
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._checkRetreatingTimer = new BasicMissionTimer();
			this._navalShipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
			this._navalAgentsLogic = base.Mission.GetMissionBehavior<NavalAgentsLogic>();
			this._navalBattleEndLogic = base.Mission.GetMissionBehavior<NavalBattleEndLogic>();
		}

		// Token: 0x06000097 RID: 151 RVA: 0x000065D1 File Offset: 0x000047D1
		public override void OnDeploymentFinished()
		{
			this._checkRetreatingTimer.Reset();
		}

		// Token: 0x06000098 RID: 152 RVA: 0x000065E0 File Offset: 0x000047E0
		public override void OnMissionTick(float dt)
		{
			if (base.Mission.IsDeploymentFinished && this._checkRetreatingTimer.ElapsedTime > 5f)
			{
				using (List<MissionShip>.Enumerator enumerator = this._navalShipsLogic.AllShips.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MissionShip missionShip = enumerator.Current;
						if (missionShip.IsShipOrderActive && missionShip.ShipOrder.MovementOrderEnum == ShipOrder.ShipMovementOrderEnum.Retreat)
						{
							Vec2 asVec = missionShip.GlobalFrame.origin.AsVec2;
							float num = missionShip.Physics.PhysicsBoundingBoxWithChildrenSize.y / 2f + 0.5f;
							if (asVec.DistanceSquared(base.Mission.GetClosestBoundaryPosition(asVec)) < num * num || !base.Mission.IsPositionInsideBoundaries(asVec))
							{
								this._tempRetreatedShips.Add(missionShip);
							}
						}
					}
					goto IL_0218;
				}
				IL_00DC:
				MissionShip missionShip2 = this._tempRetreatedShips[this._tempRetreatedShips.Count - 1];
				this._tempRetreatedShips.RemoveAt(this._tempRetreatedShips.Count - 1);
				using (List<Agent>.Enumerator enumerator2 = this._navalAgentsLogic.GetActiveAgentsOfShip(missionShip2).GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Agent agent = enumerator2.Current;
						if (agent.GetComponent<AgentNavalComponent>().SteppedShip != missionShip2)
						{
							this._tempOffShipAgents.Add(agent);
						}
					}
					goto IL_01A4;
				}
				IL_0163:
				Agent agent2 = this._tempOffShipAgents[this._tempOffShipAgents.Count - 1];
				this._tempOffShipAgents.RemoveAt(this._tempOffShipAgents.Count - 1);
				this._navalAgentsLogic.RemoveAgentFromShip(agent2, missionShip2);
				IL_01A4:
				if (this._tempOffShipAgents.Count > 0)
				{
					goto IL_0163;
				}
				this._navalAgentsLogic.FillReservedTroopsOfShip(missionShip2, this._tempRoutedReservedTroops);
				while (this._tempRoutedReservedTroops.Count > 0)
				{
					IAgentOriginBase agentOriginBase = this._tempRoutedReservedTroops[this._tempRoutedReservedTroops.Count - 1];
					this._tempRoutedReservedTroops.RemoveAt(this._tempRoutedReservedTroops.Count - 1);
					agentOriginBase.SetRouted(true);
				}
				this._navalShipsLogic.RemoveShip(missionShip2);
				IL_0218:
				if (this._tempRetreatedShips.Count > 0)
				{
					goto IL_00DC;
				}
				this._checkRetreatingTimer.Reset();
			}
		}

		// Token: 0x04000064 RID: 100
		private const float RetreatCheckInterval = 5f;

		// Token: 0x04000065 RID: 101
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x04000066 RID: 102
		private NavalAgentsLogic _navalAgentsLogic;

		// Token: 0x04000067 RID: 103
		private NavalBattleEndLogic _navalBattleEndLogic;

		// Token: 0x04000068 RID: 104
		private BasicMissionTimer _checkRetreatingTimer;

		// Token: 0x04000069 RID: 105
		private MBList<MissionShip> _tempRetreatedShips = new MBList<MissionShip>();

		// Token: 0x0400006A RID: 106
		private MBList<Agent> _tempOffShipAgents = new MBList<Agent>();

		// Token: 0x0400006B RID: 107
		private MBList<IAgentOriginBase> _tempRoutedReservedTroops = new MBList<IAgentOriginBase>();
	}
}
