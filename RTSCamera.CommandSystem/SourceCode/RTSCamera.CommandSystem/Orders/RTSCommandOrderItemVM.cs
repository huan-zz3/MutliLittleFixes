using System;
using RTSCamera.CommandSystem.Patch;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual;

namespace RTSCamera.CommandSystem.Orders
{
	// Token: 0x02000068 RID: 104
	public class RTSCommandOrderItemVM : OrderItemVM
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x060003F6 RID: 1014 RVA: 0x00017FE0 File Offset: 0x000161E0
		// (remove) Token: 0x060003F7 RID: 1015 RVA: 0x00018014 File Offset: 0x00016214
		public static event Action<OrderItemVM> OnExecuteOrder;

		// Token: 0x060003F8 RID: 1016 RVA: 0x00018047 File Offset: 0x00016247
		public static void RegisterEvent(MissionOrderVM missionOrderVM)
		{
			if (missionOrderVM == null)
			{
				return;
			}
			RTSCommandOrderItemVM.OnExecuteOrder += missionOrderVM.OnOrderExecuted;
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x0001805E File Offset: 0x0001625E
		public static void UnregisterEvent(MissionOrderVM missionOrderVM)
		{
			if (missionOrderVM == null)
			{
				return;
			}
			RTSCommandOrderItemVM.OnExecuteOrder -= missionOrderVM.OnOrderExecuted;
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x00018075 File Offset: 0x00016275
		public RTSCommandOrderItemVM(OrderController orderController, VisualOrder order)
			: base(orderController, order)
		{
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x00018080 File Offset: 0x00016280
		public void ExecuteClickAction()
		{
			Patch_OrderTroopPlacer.Reset();
			RTSCommandVisualOrder.IsFromClicking = true;
			base.ExecuteAction(new VisualOrderExecutionParameters(Agent.Main, null, null));
			RTSCommandVisualOrder.IsFromClicking = false;
		}

		// Token: 0x060003FC RID: 1020 RVA: 0x000180B8 File Offset: 0x000162B8
		protected override void OnExecuteAction(VisualOrderExecutionParameters executionParameters)
		{
			if (RTSCommandVisualOrder.IsFromClicking && this.Order.StringId == "order_movement_move")
			{
				return;
			}
			this.Order.BeforeExecuteOrder(this._orderController, executionParameters);
			this.Order.ExecuteOrder(this._orderController, executionParameters);
			if (RTSCommandVisualOrder.OrderToSelectTarget == SelectTargetMode.None)
			{
				Action<OrderItemVM> onExecuteOrder = RTSCommandOrderItemVM.OnExecuteOrder;
				if (onExecuteOrder == null)
				{
					return;
				}
				onExecuteOrder(this);
			}
		}

		// Token: 0x060003FD RID: 1021 RVA: 0x0001811F File Offset: 0x0001631F
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
