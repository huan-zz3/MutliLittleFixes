using System;
using System.Collections.Generic;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order;

namespace NavalDLC.ViewModelCollection.Order
{
	// Token: 0x02000021 RID: 33
	public class NavalMissionOrderVM : MissionOrderVM
	{
		// Token: 0x0600027A RID: 634 RVA: 0x0000DBA4 File Offset: 0x0000BDA4
		public NavalMissionOrderVM(OrderController orderController, bool isDeployment, bool isMultiplayer)
			: base(orderController, isDeployment, isMultiplayer)
		{
			this.RefreshValues();
		}

		// Token: 0x0600027B RID: 635 RVA: 0x0000DBB5 File Offset: 0x0000BDB5
		protected override MissionOrderTroopControllerVM CreateTroopController(OrderController orderController)
		{
			return new NavalMissionOrderTroopControllerVM(this, base.IsDeployment, new Action(base.OnTransferFinished));
		}

		// Token: 0x0600027C RID: 636 RVA: 0x0000DBCF File Offset: 0x0000BDCF
		public void OnClassesSet(List<MissionOrderVM.ClassConfiguration> classData)
		{
			this._classData = classData;
			(base.TroopController as NavalMissionOrderTroopControllerVM).OnClassesSet(this._classData);
		}

		// Token: 0x0600027D RID: 637 RVA: 0x0000DBEE File Offset: 0x0000BDEE
		public override void OnOrderLayoutTypeChanged()
		{
			base.OnOrderLayoutTypeChanged();
			(base.TroopController as NavalMissionOrderTroopControllerVM).OnClassesSet(this._classData);
		}

		// Token: 0x040000CD RID: 205
		private List<MissionOrderVM.ClassConfiguration> _classData;
	}
}
