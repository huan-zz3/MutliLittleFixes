using System;
using NavalDLC.Missions;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual;

namespace NavalDLC.View.VisualOrders.Orders
{
	// Token: 0x02000013 RID: 19
	public class NavalSkirmishOrder : VisualOrder
	{
		// Token: 0x0600007A RID: 122 RVA: 0x0000578B File Offset: 0x0000398B
		public NavalSkirmishOrder(string stringId)
			: base(stringId)
		{
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00005794 File Offset: 0x00003994
		public override void ExecuteOrder(OrderController orderController, VisualOrderExecutionParameters executionParameters)
		{
			if (!executionParameters.HasFormation)
			{
				orderController.SetOrder(35);
				return;
			}
			orderController.SetOrderWithFormation(35, executionParameters.Formation);
		}

		// Token: 0x0600007C RID: 124 RVA: 0x000057B5 File Offset: 0x000039B5
		public override TextObject GetName(OrderController orderController)
		{
			return new TextObject("{=skirmishOrder}Skirmish", null);
		}

		// Token: 0x0600007D RID: 125 RVA: 0x000057C2 File Offset: 0x000039C2
		public override bool IsTargeted()
		{
			return true;
		}

		// Token: 0x0600007E RID: 126 RVA: 0x000057C8 File Offset: 0x000039C8
		protected override bool? OnGetFormationHasOrder(Formation formation)
		{
			if (this._shipsLogic == null)
			{
				this._shipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			}
			if (this._shipsLogic != null)
			{
				MissionShip missionShip;
				this._shipsLogic.GetShip(formation.Team.TeamSide, formation.FormationIndex, out missionShip);
				if (missionShip != null)
				{
					return new bool?(missionShip.ShipOrder.MovementOrderEnum == ShipOrder.ShipMovementOrderEnum.Skirmish);
				}
			}
			return new bool?(VisualOrderHelper.DoesFormationHaveOrderType(formation, 35));
		}

		// Token: 0x04000024 RID: 36
		private NavalShipsLogic _shipsLogic;
	}
}
