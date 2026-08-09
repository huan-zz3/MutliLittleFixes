using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order;

namespace NavalDLC.ViewModelCollection.Order
{
	// Token: 0x02000020 RID: 32
	public class NavalMissionOrderTroopControllerVM : MissionOrderTroopControllerVM
	{
		// Token: 0x06000273 RID: 627 RVA: 0x0000D865 File Offset: 0x0000BA65
		public NavalMissionOrderTroopControllerVM(MissionOrderVM missionOrder, bool isDeployment, Action onTransferFinised)
			: base(missionOrder, isDeployment, onTransferFinised)
		{
		}

		// Token: 0x06000274 RID: 628 RVA: 0x0000D870 File Offset: 0x0000BA70
		protected override OrderTroopItemVM CreateTroopItemVM(Formation formation, Action<OrderTroopItemVM> onSelectFormation, Func<Formation, int> getFormationMorale)
		{
			return new NavalOrderTroopItemVM(formation, onSelectFormation, getFormationMorale);
		}

		// Token: 0x06000275 RID: 629 RVA: 0x0000D87C File Offset: 0x0000BA7C
		public void OnClassesSet(List<MissionOrderVM.ClassConfiguration> classData)
		{
			if (classData == null)
			{
				return;
			}
			this._classData = classData;
			using (List<MissionOrderVM.ClassConfiguration>.Enumerator enumerator = classData.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					MissionOrderVM.ClassConfiguration classItem = enumerator.Current;
					NavalOrderTroopItemVM navalOrderTroopItemVM;
					if ((navalOrderTroopItemVM = base.TroopList.FirstOrDefault<OrderTroopItemVM>((OrderTroopItemVM f) => f.Formation.Index == classItem.FormationIndex) as NavalOrderTroopItemVM) != null)
					{
						navalOrderTroopItemVM.UpdateClassData(classItem.FormationClass);
					}
					NavalOrderTroopItemVM navalOrderTroopItemVM2;
					if ((navalOrderTroopItemVM2 = base.TransferTargetList.FirstOrDefault<OrderTroopItemVM>((OrderTroopItemVM f) => f.Formation.Index == classItem.FormationIndex) as NavalOrderTroopItemVM) != null)
					{
						navalOrderTroopItemVM2.UpdateClassData(classItem.FormationClass);
					}
				}
			}
		}

		// Token: 0x06000276 RID: 630 RVA: 0x0000D93C File Offset: 0x0000BB3C
		protected override void OnAfterNewTroopItemAdded()
		{
			base.OnAfterNewTroopItemAdded();
			this.OnClassesSet(this._classData);
		}

		// Token: 0x06000277 RID: 631 RVA: 0x0000D950 File Offset: 0x0000BB50
		public override void SelectAllFormations(bool uiFeedback = true)
		{
			foreach (OrderSetVM orderSetVM in this.MissionOrder.OrderSets)
			{
				orderSetVM.ExecuteDeSelect();
			}
			if (base.TroopList.Count<OrderTroopItemVM>((OrderTroopItemVM x) => x.IsSelectable) == 1)
			{
				base.OnSelectFormation(base.TroopList.FirstOrDefault<OrderTroopItemVM>((OrderTroopItemVM x) => x.IsSelectable));
				return;
			}
			if (base.TroopList.Any<OrderTroopItemVM>((OrderTroopItemVM t) => t.IsSelectable))
			{
				base.OrderController.ClearSelectedFormations();
				if (Mission.Current.IsNavalBattle)
				{
					for (int i = 0; i < base.TroopList.Count; i++)
					{
						OrderTroopItemVM orderTroopItemVM = base.TroopList[i];
						if (!NavalDLCHelpers.IsPlayerCaptainOfFormationShip(orderTroopItemVM.Formation))
						{
							this.AddSelectedFormation(orderTroopItemVM);
						}
					}
				}
				else
				{
					base.OrderController.SelectAllFormations(uiFeedback);
				}
				if (uiFeedback && base.OrderController.SelectedFormations.Count > 0)
				{
					InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=xTv4tCbZ}Everybody!! Listen to me", null).ToString()));
				}
			}
			this.MissionOrder.SetActiveOrders();
		}

		// Token: 0x06000278 RID: 632 RVA: 0x0000DAC4 File Offset: 0x0000BCC4
		public override void AddSelectedFormation(OrderTroopItemVM item)
		{
			if (!item.IsSelectable)
			{
				return;
			}
			if (Mission.Current.IsNavalBattle)
			{
				if (this.IsOnlyPlayerFormationSelected() && !NavalDLCHelpers.IsPlayerCaptainOfFormationShip(item.Formation))
				{
					base.SetSelectedFormation(item);
					return;
				}
				if (NavalDLCHelpers.IsPlayerCaptainOfFormationShip(item.Formation))
				{
					base.OrderController.ClearSelectedFormations();
				}
			}
			Formation formation = base.Team.GetFormation(item.InitialFormationClass);
			base.OrderController.SelectFormation(formation);
			this.MissionOrder.SetActiveOrders();
		}

		// Token: 0x06000279 RID: 633 RVA: 0x0000DB44 File Offset: 0x0000BD44
		private bool IsOnlyPlayerFormationSelected()
		{
			int num = 0;
			for (int i = 0; i < base.TroopList.Count; i++)
			{
				if (base.TroopList[i].IsSelected)
				{
					num++;
					if (!NavalDLCHelpers.IsPlayerCaptainOfFormationShip(base.TroopList[i].Formation))
					{
						return false;
					}
				}
				if (num > 1)
				{
					return false;
				}
			}
			return num == 1;
		}

		// Token: 0x040000CC RID: 204
		private List<MissionOrderVM.ClassConfiguration> _classData;
	}
}
