using System;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual;

namespace NavalDLC.View.VisualOrders.Orders.TroopOrders
{
	// Token: 0x02000015 RID: 21
	public class NavalTroopDefendShipOrder : VisualOrder
	{
		// Token: 0x06000085 RID: 133 RVA: 0x0000596E File Offset: 0x00003B6E
		public NavalTroopDefendShipOrder(string iconId)
			: base(iconId)
		{
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00005977 File Offset: 0x00003B77
		public override void ExecuteOrder(OrderController orderController, VisualOrderExecutionParameters executionParameters)
		{
			orderController.SetOrder(34);
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00005981 File Offset: 0x00003B81
		public override TextObject GetName(OrderController orderController)
		{
			return new TextObject("{=FUeeV5aO}Defend Ship", null);
		}

		// Token: 0x06000088 RID: 136 RVA: 0x0000598E File Offset: 0x00003B8E
		public override bool IsTargeted()
		{
			return false;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00005991 File Offset: 0x00003B91
		protected override bool? OnGetFormationHasOrder(Formation formation)
		{
			return new bool?(VisualOrderHelper.DoesFormationHaveOrderType(formation, 34));
		}
	}
}
