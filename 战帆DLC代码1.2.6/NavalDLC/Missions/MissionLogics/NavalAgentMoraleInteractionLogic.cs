using System;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.Objects.UsableMachines;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.MissionLogics
{
	// Token: 0x020000CA RID: 202
	public class NavalAgentMoraleInteractionLogic : MissionLogic
	{
		// Token: 0x06000F06 RID: 3846 RVA: 0x00074C7B File Offset: 0x00072E7B
		public override void OnBehaviorInitialize()
		{
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this._navalShipsLogic.ShipsConnectedEvent += this.OnShipConnected;
		}

		// Token: 0x06000F07 RID: 3847 RVA: 0x00074CA4 File Offset: 0x00072EA4
		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
			this._navalShipsLogic.ShipsConnectedEvent -= this.OnShipConnected;
		}

		// Token: 0x06000F08 RID: 3848 RVA: 0x00074CC4 File Offset: 0x00072EC4
		private void OnShipConnected(MissionShip ownerShip, MissionShip targetShip)
		{
			int num = 0;
			foreach (ShipAttachmentMachine shipAttachmentMachine in ownerShip.ShipAttachmentMachines)
			{
				ShipAttachmentMachine.ShipAttachment currentAttachment = shipAttachmentMachine.CurrentAttachment;
				if (currentAttachment != null && currentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected)
				{
					num++;
				}
				if (num > 1)
				{
					break;
				}
			}
			if (num == 1 && ownerShip.Team != null && ((targetShip != null) ? targetShip.Team : null) != null && ownerShip.Team.IsEnemyOf(targetShip.Team))
			{
				foreach (Agent agent in targetShip.Formation.GetUnitsWithoutDetachedOnes())
				{
					if (agent.IsAIControlled)
					{
						float num2 = MissionGameModels.Current.BattleMoraleModel.CalculateMoraleOnShipsConnected(agent, ownerShip.ShipOrigin, targetShip.ShipOrigin);
						AgentComponentExtensions.ChangeMorale(agent, num2);
					}
				}
			}
		}

		// Token: 0x06000F09 RID: 3849 RVA: 0x00074DCC File Offset: 0x00072FCC
		public void OnShipSunk(MissionShip ship)
		{
			float num = MissionGameModels.Current.BattleMoraleModel.CalculateMoraleChangeOnShipSunk(ship.ShipOrigin);
			if (ship.Team != null)
			{
				foreach (Agent agent in ship.Team.ActiveAgents)
				{
					if (agent.IsAIControlled)
					{
						AgentComponentExtensions.ChangeMorale(agent, num);
					}
				}
			}
		}

		// Token: 0x06000F0A RID: 3850 RVA: 0x00074E4C File Offset: 0x0007304C
		public void OnShipRammed(MissionShip rammingShip, MissionShip rammedShip)
		{
			if (((rammingShip != null) ? rammingShip.Team : null) != null && rammedShip.Team != null && rammingShip.Team.IsEnemyOf(rammedShip.Team))
			{
				foreach (Agent agent in rammingShip.Team.ActiveAgents)
				{
					if (agent.IsAIControlled)
					{
						float num = MissionGameModels.Current.BattleMoraleModel.CalculateMoraleOnRamming(agent, rammingShip.ShipOrigin, rammedShip.ShipOrigin);
						AgentComponentExtensions.ChangeMorale(agent, num);
					}
				}
			}
		}

		// Token: 0x04000946 RID: 2374
		private NavalShipsLogic _navalShipsLogic;
	}
}
