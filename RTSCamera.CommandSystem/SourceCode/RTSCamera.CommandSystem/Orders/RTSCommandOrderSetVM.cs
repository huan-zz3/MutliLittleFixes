using System;
using RTSCamera.CommandSystem.Patch;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual;

namespace RTSCamera.CommandSystem.Orders
{
	// Token: 0x02000069 RID: 105
	public class RTSCommandOrderSetVM : OrderSetVM
	{
		// Token: 0x060003FE RID: 1022 RVA: 0x00018136 File Offset: 0x00016336
		public RTSCommandOrderSetVM(OrderController orderController, VisualOrderSet collection)
			: base(orderController, collection)
		{
		}

		// Token: 0x060003FF RID: 1023 RVA: 0x00018140 File Offset: 0x00016340
		public void ExecuteClickAction()
		{
			Patch_OrderTroopPlacer.Reset();
			if (base.OrderSet.IsSoloOrder)
			{
				if (base.Orders.Count > 0)
				{
					RTSCommandOrderItemVM rtscommandOrderItemVM = base.Orders[0] as RTSCommandOrderItemVM;
					if (rtscommandOrderItemVM == null)
					{
						return;
					}
					rtscommandOrderItemVM.ExecuteClickAction();
					return;
				}
			}
			else
			{
				base.ExecuteAction(new VisualOrderExecutionParameters(Agent.Main, null, null));
			}
		}

		// Token: 0x06000400 RID: 1024 RVA: 0x000181A3 File Offset: 0x000163A3
		public void OnEscape()
		{
			GauntletOrderUIHandler missionBehavior = Mission.Current.GetMissionBehavior<GauntletOrderUIHandler>();
			if (missionBehavior == null)
			{
				return;
			}
			missionBehavior.OnEscape();
		}
	}
}
