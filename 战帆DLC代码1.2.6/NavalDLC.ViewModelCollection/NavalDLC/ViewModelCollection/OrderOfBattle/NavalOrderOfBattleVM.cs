using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.CampaignBehaviors;
using NavalDLC.Missions.Deployment;
using NavalDLC.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;

namespace NavalDLC.ViewModelCollection.OrderOfBattle
{
	// Token: 0x02000026 RID: 38
	public class NavalOrderOfBattleVM : ViewModel
	{
		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x0600031A RID: 794 RVA: 0x00010105 File Offset: 0x0000E305
		public MBReadOnlyList<NavalOrderOfBattleFormationItemVM> AllFormations
		{
			get
			{
				return this._allFormations;
			}
		}

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x0600031B RID: 795 RVA: 0x0001010D File Offset: 0x0000E30D
		// (set) Token: 0x0600031C RID: 796 RVA: 0x00010115 File Offset: 0x0000E315
		public List<MissionOrderVM.FormationConfiguration> CurrentFilterConfiguration { get; private set; }

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x0600031D RID: 797 RVA: 0x0001011E File Offset: 0x0000E31E
		// (set) Token: 0x0600031E RID: 798 RVA: 0x00010126 File Offset: 0x0000E326
		public List<MissionOrderVM.ClassConfiguration> CurrentClassConfiguration { get; private set; }

		// Token: 0x0600031F RID: 799 RVA: 0x00010130 File Offset: 0x0000E330
		public NavalOrderOfBattleVM(Mission mission, Action<NavalOrderOfBattleFormationItemVM> onFormationSelected, Action clearFormationSelection, Action onAutoDeploy, Action onBeginMission)
		{
			this._mission = mission;
			this._onFormationSelected = onFormationSelected;
			this._clearFormationSelection = clearFormationSelection;
			this._onAutoDeploy = onAutoDeploy;
			this._onBeginMission = onBeginMission;
			this._allFormations = new MBList<NavalOrderOfBattleFormationItemVM>();
			this.LeftFormations = new MBBindingList<NavalOrderOfBattleFormationItemVM>();
			this.RightFormations = new MBBindingList<NavalOrderOfBattleFormationItemVM>();
			this._allHeroes = new List<NavalOrderOfBattleHeroItemVM>();
			this._allShips = new List<NavalOrderOfBattleShipItemVM>();
			this.UnassignedHeroes = new MBBindingList<NavalOrderOfBattleHeroItemVM>();
			this.UnassignedShips = new MBBindingList<NavalOrderOfBattleShipItemVM>();
			this.RefreshValues();
		}

		// Token: 0x06000320 RID: 800 RVA: 0x00010200 File Offset: 0x0000E400
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.BeginMissionText = new TextObject("{=SYYOSOoa}Ready", null).ToString();
			this.AutoDeployText = GameTexts.FindText("str_auto_deploy", null).ToString();
			this._allHeroes.ForEach(delegate(NavalOrderOfBattleHeroItemVM h)
			{
				h.RefreshValues();
			});
			this._allShips.ForEach(delegate(NavalOrderOfBattleShipItemVM s)
			{
				s.RefreshValues();
			});
			this.LeftFormations.ApplyActionOnAllItems(delegate(NavalOrderOfBattleFormationItemVM f)
			{
				f.RefreshValues();
			});
			this.RightFormations.ApplyActionOnAllItems(delegate(NavalOrderOfBattleFormationItemVM f)
			{
				f.RefreshValues();
			});
		}

		// Token: 0x06000321 RID: 801 RVA: 0x000102E8 File Offset: 0x0000E4E8
		public override void OnFinalize()
		{
			base.OnFinalize();
			if (this.IsEnabled)
			{
				this.SaveConfiguration();
			}
			if (this._navalDeploymentController != null)
			{
				this._navalDeploymentController.PlayerShipsUpdated -= this.OnPlayerShipsUpdated;
				this._navalDeploymentController = null;
			}
			if (this._orderController != null)
			{
				this._orderController.OnSelectedFormationsChanged -= this.OnSelectedFormationsChanged;
				this._orderController = null;
			}
			NavalOrderOfBattleFormationItemVM.OnAcceptCaptain = (Action<NavalOrderOfBattleFormationItemVM>)Delegate.Remove(NavalOrderOfBattleFormationItemVM.OnAcceptCaptain, new Action<NavalOrderOfBattleFormationItemVM>(this.OnFormationAcceptCaptain));
			NavalOrderOfBattleFormationItemVM.OnAcceptShip = (Action<NavalOrderOfBattleFormationItemVM>)Delegate.Remove(NavalOrderOfBattleFormationItemVM.OnAcceptShip, new Action<NavalOrderOfBattleFormationItemVM>(this.OnFormationAcceptShip));
			NavalOrderOfBattleFormationItemVM.GetTotalTroopCountWithFilter = (Func<DeploymentFormationClass, FormationFilterType, int>)Delegate.Remove(NavalOrderOfBattleFormationItemVM.GetTotalTroopCountWithFilter, new Func<DeploymentFormationClass, FormationFilterType, int>(this.GetTroopCountWithFilter));
			this.IsEnabled = false;
			InputKeyItemVM doneInputKey = this.DoneInputKey;
			if (doneInputKey != null)
			{
				doneInputKey.OnFinalize();
			}
			this.DoneInputKey = null;
			InputKeyItemVM resetInputKey = this.ResetInputKey;
			if (resetInputKey != null)
			{
				resetInputKey.OnFinalize();
			}
			this.ResetInputKey = null;
			this.LeftFormations.ApplyActionOnAllItems(delegate(NavalOrderOfBattleFormationItemVM f)
			{
				f.OnFinalize();
			});
			this.LeftFormations.Clear();
			this.RightFormations.ApplyActionOnAllItems(delegate(NavalOrderOfBattleFormationItemVM f)
			{
				f.OnFinalize();
			});
			this.RightFormations.Clear();
			this._allFormations.Clear();
			this._allHeroes.ForEach(delegate(NavalOrderOfBattleHeroItemVM h)
			{
				h.OnFinalize();
			});
			this._allShips.ForEach(delegate(NavalOrderOfBattleShipItemVM s)
			{
				s.OnFinalize();
			});
			this._allHeroes.Clear();
			this._allShips.Clear();
			this.UnassignedHeroes.Clear();
			this.UnassignedShips.Clear();
		}

		// Token: 0x06000322 RID: 802 RVA: 0x000104E4 File Offset: 0x0000E6E4
		public void Initialize()
		{
			this._navalShipsLogic = this._mission.GetMissionBehavior<NavalShipsLogic>();
			this._navalDeploymentController = this._mission.GetMissionBehavior<NavalDeploymentMissionController>();
			this._assignPlayerRoleInTeamMissioncontroller = this._mission.GetMissionBehavior<AssignPlayerRoleInTeamMissionController>();
			this._navalDeploymentController.PlayerShipsUpdated += this.OnPlayerShipsUpdated;
			this._orderController = this._mission.PlayerTeam.PlayerOrderController;
			this._orderController.OnSelectedFormationsChanged += this.OnSelectedFormationsChanged;
			NavalOrderOfBattleFormationItemVM.OnAcceptCaptain = (Action<NavalOrderOfBattleFormationItemVM>)Delegate.Combine(NavalOrderOfBattleFormationItemVM.OnAcceptCaptain, new Action<NavalOrderOfBattleFormationItemVM>(this.OnFormationAcceptCaptain));
			NavalOrderOfBattleFormationItemVM.OnAcceptShip = (Action<NavalOrderOfBattleFormationItemVM>)Delegate.Combine(NavalOrderOfBattleFormationItemVM.OnAcceptShip, new Action<NavalOrderOfBattleFormationItemVM>(this.OnFormationAcceptShip));
			NavalOrderOfBattleFormationItemVM.GetTotalTroopCountWithFilter = (Func<DeploymentFormationClass, FormationFilterType, int>)Delegate.Combine(NavalOrderOfBattleFormationItemVM.GetTotalTroopCountWithFilter, new Func<DeploymentFormationClass, FormationFilterType, int>(this.GetTroopCountWithFilter));
			this.IsPlayerGeneral = this._mission.PlayerTeam.IsPlayerGeneral;
			this.CurrentFilterConfiguration = new List<MissionOrderVM.FormationConfiguration>();
			this.CurrentClassConfiguration = new List<MissionOrderVM.ClassConfiguration>();
			this.RefreshAll();
			Campaign campaign = Campaign.Current;
			this._navalOrderOfBattleCampaignBehavior = ((campaign != null) ? campaign.GetCampaignBehavior<NavalOrderOfBattleCampaignBehavior>() : null);
			this.LoadConfigurationShips();
			if (this.IsAssignmentDirty)
			{
				this._finalizeInitializationOnNextUpdate = true;
			}
			else
			{
				this.FinalizeInitialization();
			}
			this.IsEnabled = true;
		}

		// Token: 0x06000323 RID: 803 RVA: 0x00010635 File Offset: 0x0000E835
		public void ExecuteAutoDeploy()
		{
			if (this.IsAssignmentDirty)
			{
				return;
			}
			this.IsAssignmentDirty = true;
			Action onAutoDeploy = this._onAutoDeploy;
			if (onAutoDeploy == null)
			{
				return;
			}
			onAutoDeploy();
		}

		// Token: 0x06000324 RID: 804 RVA: 0x00010658 File Offset: 0x0000E858
		public void ExecuteBeginMission()
		{
			if (this.IsAssignmentDirty || !this.CanStartMission)
			{
				return;
			}
			List<MissionOrderVM.FormationConfiguration> currentFilterConfiguration = this.CurrentFilterConfiguration;
			if (currentFilterConfiguration != null)
			{
				currentFilterConfiguration.Clear();
			}
			List<MissionOrderVM.ClassConfiguration> currentClassConfiguration = this.CurrentClassConfiguration;
			if (currentClassConfiguration != null)
			{
				currentClassConfiguration.Clear();
			}
			foreach (NavalOrderOfBattleFormationItemVM navalOrderOfBattleFormationItemVM in this.AllFormations)
			{
				if (navalOrderOfBattleFormationItemVM.Formation.CountOfUnits > 0)
				{
					List<MissionOrderVM.FormationConfiguration> currentFilterConfiguration2 = this.CurrentFilterConfiguration;
					if (currentFilterConfiguration2 != null)
					{
						currentFilterConfiguration2.Add(new MissionOrderVM.FormationConfiguration(navalOrderOfBattleFormationItemVM.Formation.Index, (from f in navalOrderOfBattleFormationItemVM.FilterItems
							where f.IsActive
							select f.FilterType).ToList<FormationFilterType>()));
					}
					List<MissionOrderVM.ClassConfiguration> currentClassConfiguration2 = this.CurrentClassConfiguration;
					if (currentClassConfiguration2 != null)
					{
						currentClassConfiguration2.Add(new MissionOrderVM.ClassConfiguration(navalOrderOfBattleFormationItemVM.Formation.Index, navalOrderOfBattleFormationItemVM.SelectedClass));
					}
				}
				else
				{
					List<MissionOrderVM.FormationConfiguration> currentFilterConfiguration3 = this.CurrentFilterConfiguration;
					if (currentFilterConfiguration3 != null)
					{
						currentFilterConfiguration3.Add(new MissionOrderVM.FormationConfiguration(navalOrderOfBattleFormationItemVM.Formation.Index, new List<FormationFilterType>()));
					}
					List<MissionOrderVM.ClassConfiguration> currentClassConfiguration3 = this.CurrentClassConfiguration;
					if (currentClassConfiguration3 != null)
					{
						currentClassConfiguration3.Add(new MissionOrderVM.ClassConfiguration(navalOrderOfBattleFormationItemVM.Formation.Index, 1));
					}
				}
			}
			Action onBeginMission = this._onBeginMission;
			if (onBeginMission != null)
			{
				onBeginMission();
			}
			MBInformationManager.HideInformations();
		}

		// Token: 0x06000325 RID: 805 RVA: 0x000107F8 File Offset: 0x0000E9F8
		public void ExecuteClearHeroAndShipSelection()
		{
			this.SelectedHero = null;
			this.SelectedShip = null;
		}

		// Token: 0x06000326 RID: 806 RVA: 0x00010808 File Offset: 0x0000EA08
		public bool OnEscape()
		{
			bool flag = false;
			if (this.AllFormations.Any<NavalOrderOfBattleFormationItemVM>((NavalOrderOfBattleFormationItemVM x) => x.IsSelected))
			{
				Action clearFormationSelection = this._clearFormationSelection;
				if (clearFormationSelection != null)
				{
					clearFormationSelection();
				}
				flag = true;
			}
			return flag;
		}

		// Token: 0x06000327 RID: 807 RVA: 0x00010858 File Offset: 0x0000EA58
		private void RefreshFormations()
		{
			if (this.AllFormations.Count == 0)
			{
				MBReadOnlyList<Formation> usableFormations = this._navalDeploymentController.GetUsableFormations();
				for (int i = 0; i < usableFormations.Count; i++)
				{
					NavalOrderOfBattleFormationItemVM navalOrderOfBattleFormationItemVM = new NavalOrderOfBattleFormationItemVM(usableFormations[i], new Action<NavalOrderOfBattleFormationItemVM>(this.OnFormationSelected), new Action<NavalOrderOfBattleFormationItemVM>(this.OnClassChanged), new Action<NavalOrderOfBattleFormationItemVM>(this.OnFilterUseToggled));
					if (i < usableFormations.Count / 2)
					{
						this.LeftFormations.Add(navalOrderOfBattleFormationItemVM);
					}
					else
					{
						this.RightFormations.Add(navalOrderOfBattleFormationItemVM);
					}
					this._allFormations.Add(navalOrderOfBattleFormationItemVM);
				}
			}
		}

		// Token: 0x06000328 RID: 808 RVA: 0x000108F4 File Offset: 0x0000EAF4
		private void RefreshShips()
		{
			if (this._allShips.Count == 0)
			{
				foreach (IShipOrigin shipOrigin in this._navalDeploymentController.GetAllPlayerShips())
				{
					this._allShips.Add(new NavalOrderOfBattleShipItemVM(shipOrigin, new Action<NavalOrderOfBattleShipItemVM, bool>(this.OnShipSelected), new Func<NavalOrderOfBattleShipItemVM, NavalOrderOfBattleFormationItemVM>(this.FindFormationOfShip)));
				}
			}
			for (int i = 0; i < this._allShips.Count; i++)
			{
				NavalOrderOfBattleShipItemVM navalOrderOfBattleShipItemVM = this._allShips[i];
				ShipAssignment shipAssignment;
				bool flag = this._navalShipsLogic.FindAssignmentOfShipOrigin(navalOrderOfBattleShipItemVM.ShipOrigin, out shipAssignment);
				NavalOrderOfBattleShipItemVM navalOrderOfBattleShipItemVM2 = navalOrderOfBattleShipItemVM;
				bool flag2;
				if (!this.IsPlayerGeneral)
				{
					if (PartyBase.MainParty.Ships.Contains(navalOrderOfBattleShipItemVM.ShipOrigin))
					{
						if (flag)
						{
							Agent captain = shipAssignment.Formation.Captain;
							flag2 = captain == null || !captain.IsMainAgent;
						}
						else
						{
							flag2 = false;
						}
					}
					else
					{
						flag2 = true;
					}
				}
				else
				{
					flag2 = false;
				}
				navalOrderOfBattleShipItemVM2.IsDisabled = flag2;
				if (flag)
				{
					navalOrderOfBattleShipItemVM.MissionShip = shipAssignment.MissionShip;
					if (this.UnassignedShips.Contains(navalOrderOfBattleShipItemVM))
					{
						this.UnassignedShips.Remove(navalOrderOfBattleShipItemVM);
					}
					for (int j = 0; j < this.AllFormations.Count; j++)
					{
						if (this.AllFormations[j].Formation == shipAssignment.Formation && this.AllFormations[j].Ship != navalOrderOfBattleShipItemVM)
						{
							this.AllFormations[j].Ship = navalOrderOfBattleShipItemVM;
						}
						else if (this.AllFormations[j].Formation != shipAssignment.Formation && this.AllFormations[j].Ship == navalOrderOfBattleShipItemVM)
						{
							this.AllFormations[j].Ship = null;
						}
					}
				}
				else
				{
					navalOrderOfBattleShipItemVM.MissionShip = null;
					for (int k = 0; k < this.AllFormations.Count; k++)
					{
						if (this.AllFormations[k].Ship == navalOrderOfBattleShipItemVM)
						{
							this.AllFormations[k].Ship = null;
						}
					}
					if (!navalOrderOfBattleShipItemVM.IsDisabled && !this.UnassignedShips.Contains(navalOrderOfBattleShipItemVM))
					{
						this.UnassignedShips.Add(navalOrderOfBattleShipItemVM);
					}
					else if (navalOrderOfBattleShipItemVM.IsDisabled && this.UnassignedShips.Contains(navalOrderOfBattleShipItemVM))
					{
						this.UnassignedShips.Remove(navalOrderOfBattleShipItemVM);
					}
				}
			}
		}

		// Token: 0x06000329 RID: 809 RVA: 0x00010B74 File Offset: 0x0000ED74
		private void RefreshHeroes()
		{
			if (this._allHeroes.Count == 0)
			{
				foreach (IAgentOriginBase agentOriginBase in this._navalDeploymentController.GetAllPlayerTeamHeroes())
				{
					this._allHeroes.Add(new NavalOrderOfBattleHeroItemVM(agentOriginBase, new Action<NavalOrderOfBattleHeroItemVM, bool>(this.OnHeroSelected)));
				}
			}
			for (int i = 0; i < this._allHeroes.Count; i++)
			{
				NavalOrderOfBattleHeroItemVM heroVM = this._allHeroes[i];
				NavalOrderOfBattleFormationItemVM navalOrderOfBattleFormationItemVM = this.AllFormations.FirstOrDefault<NavalOrderOfBattleFormationItemVM>(delegate(NavalOrderOfBattleFormationItemVM x)
				{
					Agent captain = x.Formation.Captain;
					return ((captain != null) ? captain.Origin : null) == heroVM.AgentOrigin;
				});
				heroVM.IsDisabled = !this.IsPlayerGeneral && !heroVM.IsMainHero;
				if (navalOrderOfBattleFormationItemVM != null)
				{
					if (this.UnassignedHeroes.Contains(heroVM))
					{
						this.UnassignedHeroes.Remove(heroVM);
					}
					for (int j = 0; j < this.AllFormations.Count; j++)
					{
						NavalOrderOfBattleFormationItemVM navalOrderOfBattleFormationItemVM2 = this.AllFormations[j];
						if (navalOrderOfBattleFormationItemVM2 == navalOrderOfBattleFormationItemVM && navalOrderOfBattleFormationItemVM2.Captain != heroVM)
						{
							navalOrderOfBattleFormationItemVM2.Captain = heroVM;
						}
						else if (navalOrderOfBattleFormationItemVM2 != navalOrderOfBattleFormationItemVM && navalOrderOfBattleFormationItemVM2.Captain == heroVM)
						{
							navalOrderOfBattleFormationItemVM2.Captain = null;
						}
					}
				}
				else
				{
					for (int k = 0; k < this.AllFormations.Count; k++)
					{
						if (this.AllFormations[k].Captain == heroVM)
						{
							this.AllFormations[k].Captain = null;
						}
					}
					if (!heroVM.IsDisabled && !this.UnassignedHeroes.Contains(heroVM))
					{
						this.UnassignedHeroes.Add(heroVM);
					}
					else if (heroVM.IsDisabled && this.UnassignedHeroes.Contains(heroVM))
					{
						this.UnassignedHeroes.Remove(heroVM);
					}
				}
			}
		}

		// Token: 0x0600032A RID: 810 RVA: 0x00010DA4 File Offset: 0x0000EFA4
		private void RefreshFormationsDisabledAndReason()
		{
			NavalShipDeploymentLimit navalShipDeploymentLimit;
			this._navalShipsLogic.GetShipDeploymentLimit(0, out navalShipDeploymentLimit);
			NavalShipDeploymentLimit navalShipDeploymentLimit2;
			int shipDeploymentLimit = this._navalShipsLogic.GetShipDeploymentLimit(1, out navalShipDeploymentLimit2);
			int num = this._allShips.Count<NavalOrderOfBattleShipItemVM>((NavalOrderOfBattleShipItemVM x) => !x.IsDisabled);
			for (int i = 0; i < this.AllFormations.Count; i++)
			{
				NavalOrderOfBattleFormationItemVM navalOrderOfBattleFormationItemVM = this.AllFormations[i];
				int num2 = i + 1;
				if (navalOrderOfBattleFormationItemVM.Formation.PlayerOwner != Agent.Main)
				{
					navalOrderOfBattleFormationItemVM.IsEnabled = false;
					navalOrderOfBattleFormationItemVM.DisabledHint = new HintViewModel(this._formationsDisabledHintGeneral, null);
				}
				else if (num2 > 8 - shipDeploymentLimit)
				{
					navalOrderOfBattleFormationItemVM.IsEnabled = false;
					navalOrderOfBattleFormationItemVM.DisabledHint = new HintViewModel(this._formationsDisabledHintAllies, null);
				}
				else if (num2 > navalShipDeploymentLimit.PartiesLimit)
				{
					navalOrderOfBattleFormationItemVM.IsEnabled = false;
					navalOrderOfBattleFormationItemVM.DisabledHint = new HintViewModel(this._formationsDisabledHintSkills, null);
				}
				else if (num2 > num)
				{
					navalOrderOfBattleFormationItemVM.IsEnabled = false;
					navalOrderOfBattleFormationItemVM.DisabledHint = new HintViewModel(this._formationsDisabledHintShips, null);
				}
				else
				{
					navalOrderOfBattleFormationItemVM.IsEnabled = true;
					navalOrderOfBattleFormationItemVM.DisabledHint = null;
				}
			}
		}

		// Token: 0x0600032B RID: 811 RVA: 0x00010EDC File Offset: 0x0000F0DC
		private void RefreshCanStartMission()
		{
			if (!this.IsPlayerGeneral)
			{
				this.CanStartMission = true;
				this.CanStartHint = null;
			}
			if (this.AllFormations.Any<NavalOrderOfBattleFormationItemVM>((NavalOrderOfBattleFormationItemVM x) => x.HasShip && x.TroopCount == 0))
			{
				this.CanStartMission = false;
				this.CanStartHint = new HintViewModel(new TextObject("{=UL3x9GoP}There is a ship without any troops!", null), null);
				return;
			}
			this.CanStartMission = true;
			this.CanStartHint = null;
		}

		// Token: 0x0600032C RID: 812 RVA: 0x00010F58 File Offset: 0x0000F158
		private void FinalizeInitialization()
		{
			this.LoadConfigurationAgents();
			if (!this.IsPlayerGeneral)
			{
				this._assignPlayerRoleInTeamMissioncontroller.OnPlayerChoiceFinalized();
				this.RefreshAll();
			}
		}

		// Token: 0x0600032D RID: 813 RVA: 0x00010F7C File Offset: 0x0000F17C
		private void RefreshAll()
		{
			this.ExecuteClearHeroAndShipSelection();
			Action clearFormationSelection = this._clearFormationSelection;
			if (clearFormationSelection != null)
			{
				clearFormationSelection();
			}
			this.RefreshFormations();
			this.RefreshShips();
			this.RefreshHeroes();
			this.RefreshFormationsDisabledAndReason();
			this.RefreshValues();
			this.RefreshCanStartMission();
			this.IsAssignmentDirty = false;
		}

		// Token: 0x0600032E RID: 814 RVA: 0x00010FCC File Offset: 0x0000F1CC
		private void LoadConfigurationShips()
		{
			if (this._navalOrderOfBattleCampaignBehavior == null || !this.IsPlayerGeneral)
			{
				return;
			}
			for (int i = 0; i < this.AllFormations.Count; i++)
			{
				NavalOrderOfBattleFormationItemVM navalOrderOfBattleFormationItemVM = this.AllFormations[i];
				NavalOrderOfBattleCampaignBehavior.NavalOrderOfBattleFormationData formationInfo = this._navalOrderOfBattleCampaignBehavior.GetFormationDataAtIndex(i, MobileParty.MainParty.Army != null);
				if (formationInfo != null && navalOrderOfBattleFormationItemVM.IsEnabled)
				{
					if (formationInfo.Ship != null)
					{
						NavalOrderOfBattleShipItemVM navalOrderOfBattleShipItemVM = this._allShips.FirstOrDefault<NavalOrderOfBattleShipItemVM>((NavalOrderOfBattleShipItemVM x) => x.ShipOrigin == formationInfo.Ship);
						if (navalOrderOfBattleShipItemVM != null && navalOrderOfBattleShipItemVM.GetCanBeUnassignedOrMoved() && navalOrderOfBattleFormationItemVM.GetCanAcceptShip())
						{
							this.AssignShipToFormation(navalOrderOfBattleShipItemVM, navalOrderOfBattleFormationItemVM, false);
						}
						else
						{
							NavalOrderOfBattleShipItemVM ship = navalOrderOfBattleFormationItemVM.Ship;
							if (ship == null || ship.GetCanBeUnassignedOrMoved())
							{
								this.AssignShipToFormation(null, navalOrderOfBattleFormationItemVM, false);
							}
						}
					}
					else
					{
						NavalOrderOfBattleShipItemVM ship2 = navalOrderOfBattleFormationItemVM.Ship;
						if (ship2 == null || ship2.GetCanBeUnassignedOrMoved())
						{
							this.AssignShipToFormation(null, navalOrderOfBattleFormationItemVM, false);
						}
					}
				}
			}
		}

		// Token: 0x0600032F RID: 815 RVA: 0x000110D0 File Offset: 0x0000F2D0
		private void LoadConfigurationAgents()
		{
			if (this._navalOrderOfBattleCampaignBehavior == null || !this.IsPlayerGeneral)
			{
				return;
			}
			this._isLoadingConfigurationAgents = true;
			for (int i = 0; i < this.AllFormations.Count; i++)
			{
				NavalOrderOfBattleFormationItemVM navalOrderOfBattleFormationItemVM = this.AllFormations[i];
				NavalOrderOfBattleCampaignBehavior.NavalOrderOfBattleFormationData formationInfo = this._navalOrderOfBattleCampaignBehavior.GetFormationDataAtIndex(i, MobileParty.MainParty.Army != null);
				if (formationInfo != null && navalOrderOfBattleFormationItemVM.IsEnabled)
				{
					if (formationInfo.Captain != null)
					{
						NavalOrderOfBattleHeroItemVM navalOrderOfBattleHeroItemVM = this._allHeroes.FirstOrDefault<NavalOrderOfBattleHeroItemVM>((NavalOrderOfBattleHeroItemVM x) => x.AgentOrigin.Troop == formationInfo.Captain.CharacterObject);
						if (navalOrderOfBattleHeroItemVM != null && navalOrderOfBattleHeroItemVM.GetCanBeUnassignedOrMoved() && navalOrderOfBattleFormationItemVM.GetCanAcceptCaptain())
						{
							this.AssignCaptainToFormation(navalOrderOfBattleHeroItemVM, navalOrderOfBattleFormationItemVM);
						}
						else
						{
							NavalOrderOfBattleHeroItemVM captain = navalOrderOfBattleFormationItemVM.Captain;
							if (captain == null || captain.GetCanBeUnassignedOrMoved())
							{
								this.AssignCaptainToFormation(null, navalOrderOfBattleFormationItemVM);
							}
						}
					}
					else
					{
						NavalOrderOfBattleHeroItemVM captain2 = navalOrderOfBattleFormationItemVM.Captain;
						if (captain2 == null || captain2.GetCanBeUnassignedOrMoved())
						{
							this.AssignCaptainToFormation(null, navalOrderOfBattleFormationItemVM);
						}
					}
					if (formationInfo.FormationClass != null && navalOrderOfBattleFormationItemVM.IsSelectable)
					{
						if (formationInfo.FormationClass == 1)
						{
							navalOrderOfBattleFormationItemVM.ExecuteSelectInfantry();
						}
						else if (formationInfo.FormationClass == 2)
						{
							navalOrderOfBattleFormationItemVM.ExecuteSelectRanged();
						}
						else if (formationInfo.FormationClass == 5)
						{
							navalOrderOfBattleFormationItemVM.ExecuteSelectInfantryAndRanged();
						}
						bool flag;
						formationInfo.Filters.TryGetValue(1, out flag);
						bool flag2;
						formationInfo.Filters.TryGetValue(4, out flag2);
						bool flag3;
						formationInfo.Filters.TryGetValue(3, out flag3);
						bool flag4;
						formationInfo.Filters.TryGetValue(5, out flag4);
						bool flag5;
						formationInfo.Filters.TryGetValue(6, out flag5);
						navalOrderOfBattleFormationItemVM.FilterItems.FirstOrDefault<OrderOfBattleFormationFilterSelectorItemVM>((OrderOfBattleFormationFilterSelectorItemVM f) => f.FilterType == 1).IsActive = flag;
						navalOrderOfBattleFormationItemVM.FilterItems.FirstOrDefault<OrderOfBattleFormationFilterSelectorItemVM>((OrderOfBattleFormationFilterSelectorItemVM f) => f.FilterType == 3).IsActive = flag3;
						navalOrderOfBattleFormationItemVM.FilterItems.FirstOrDefault<OrderOfBattleFormationFilterSelectorItemVM>((OrderOfBattleFormationFilterSelectorItemVM f) => f.FilterType == 4).IsActive = flag2;
						navalOrderOfBattleFormationItemVM.FilterItems.FirstOrDefault<OrderOfBattleFormationFilterSelectorItemVM>((OrderOfBattleFormationFilterSelectorItemVM f) => f.FilterType == 5).IsActive = flag4;
						navalOrderOfBattleFormationItemVM.FilterItems.FirstOrDefault<OrderOfBattleFormationFilterSelectorItemVM>((OrderOfBattleFormationFilterSelectorItemVM f) => f.FilterType == 6).IsActive = flag5;
					}
				}
			}
			this._navalDeploymentController.UpdateShips(0);
			this.IsAssignmentDirty = true;
			this._isLoadingConfigurationAgents = false;
		}

		// Token: 0x06000330 RID: 816 RVA: 0x000113A4 File Offset: 0x0000F5A4
		private void SaveConfiguration()
		{
			if (this._navalOrderOfBattleCampaignBehavior == null || !this.IsPlayerGeneral || !MissionGameModels.Current.BattleInitializationModel.CanPlayerSideDeployWithOrderOfBattle())
			{
				return;
			}
			List<NavalOrderOfBattleCampaignBehavior.NavalOrderOfBattleFormationData> list = new List<NavalOrderOfBattleCampaignBehavior.NavalOrderOfBattleFormationData>();
			for (int i = 0; i < this.AllFormations.Count; i++)
			{
				NavalOrderOfBattleFormationItemVM formationItemVM = this.AllFormations[i];
				IShipOrigin shipOrigin = null;
				Hero hero = null;
				bool isSelectable = formationItemVM.IsSelectable;
				if (isSelectable)
				{
					NavalOrderOfBattleShipItemVM ship = formationItemVM.Ship;
					if (((ship != null) ? ship.ShipOrigin : null) != null && !formationItemVM.Ship.IsDisabled)
					{
						shipOrigin = formationItemVM.Ship.ShipOrigin;
					}
					NavalOrderOfBattleHeroItemVM captain = formationItemVM.Captain;
					if (((captain != null) ? captain.AgentOrigin : null) != null && !formationItemVM.Captain.IsDisabled)
					{
						hero = Hero.FindFirst((Hero h) => h.CharacterObject == formationItemVM.Captain.AgentOrigin.Troop);
					}
				}
				DeploymentFormationClass deploymentFormationClass = (isSelectable ? formationItemVM.FormationClassInt : 0);
				Dictionary<FormationFilterType, bool> dictionary = new Dictionary<FormationFilterType, bool>();
				dictionary[1] = isSelectable && formationItemVM.HasFilter(1);
				dictionary[3] = isSelectable && formationItemVM.HasFilter(3);
				dictionary[4] = isSelectable && formationItemVM.HasFilter(4);
				dictionary[5] = isSelectable && formationItemVM.HasFilter(5);
				dictionary[6] = isSelectable && formationItemVM.HasFilter(6);
				Dictionary<FormationFilterType, bool> dictionary2 = dictionary;
				list.Add(new NavalOrderOfBattleCampaignBehavior.NavalOrderOfBattleFormationData(hero, shipOrigin as Ship, deploymentFormationClass, dictionary2));
			}
			this._navalOrderOfBattleCampaignBehavior.SetFormationInfos(list, MobileParty.MainParty.Army != null);
		}

		// Token: 0x06000331 RID: 817 RVA: 0x0001156C File Offset: 0x0000F76C
		private void OnClassChanged(NavalOrderOfBattleFormationItemVM formationItem)
		{
			if (this.IsAssignmentDirty)
			{
				return;
			}
			TroopTraitsMask filter = TroopFilteringUtilities.GetFilter(OrderOfBattleFormationExtensions.GetFormationClasses(formationItem.SelectedClass).ToArray());
			if (this._navalDeploymentController.SetTroopClassFilter(filter, formationItem.Formation, !this._isLoadingConfigurationAgents))
			{
				this.IsAssignmentDirty = true;
			}
		}

		// Token: 0x06000332 RID: 818 RVA: 0x000115BC File Offset: 0x0000F7BC
		private void OnFilterUseToggled(NavalOrderOfBattleFormationItemVM formationItem)
		{
			if (this.IsAssignmentDirty)
			{
				return;
			}
			TroopTraitsMask filter = TroopFilteringUtilities.GetFilter((from f in formationItem.FilterItems
				where f.IsActive
				select f.FilterType).ToArray<FormationFilterType>());
			if (this._navalDeploymentController.SetTroopTraitsFilter(filter, formationItem.Formation, !this._isLoadingConfigurationAgents))
			{
				this.IsAssignmentDirty = true;
			}
		}

		// Token: 0x06000333 RID: 819 RVA: 0x0001164F File Offset: 0x0000F84F
		private void OnFormationSelected(NavalOrderOfBattleFormationItemVM formation)
		{
			if (this.IsAssignmentDirty)
			{
				return;
			}
			Action<NavalOrderOfBattleFormationItemVM> onFormationSelected = this._onFormationSelected;
			if (onFormationSelected != null)
			{
				onFormationSelected(formation);
			}
			this.ExecuteClearHeroAndShipSelection();
		}

		// Token: 0x06000334 RID: 820 RVA: 0x00011674 File Offset: 0x0000F874
		private void OnSelectedFormationsChanged()
		{
			for (int i = 0; i < this.AllFormations.Count; i++)
			{
				NavalOrderOfBattleFormationItemVM navalOrderOfBattleFormationItemVM = this.AllFormations[i];
				navalOrderOfBattleFormationItemVM.IsSelected = this._orderController.IsFormationListening(navalOrderOfBattleFormationItemVM.Formation);
			}
		}

		// Token: 0x06000335 RID: 821 RVA: 0x000116BC File Offset: 0x0000F8BC
		private void OnShipSelected(NavalOrderOfBattleShipItemVM ship, bool isSelected)
		{
			if (this.IsAssignmentDirty)
			{
				return;
			}
			if (isSelected)
			{
				this.SelectedShip = ship;
				this.SelectedHero = null;
				Action clearFormationSelection = this._clearFormationSelection;
				if (clearFormationSelection == null)
				{
					return;
				}
				clearFormationSelection();
				return;
			}
			else
			{
				if (this.SelectedShip == ship)
				{
					this.SelectedShip = null;
					return;
				}
				Debug.FailedAssert("Trying to deselect ship that isn't SelectedShip!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.ViewModelCollection\\OrderOfBattle\\NavalOrderOfBattleVM.cs", "OnShipSelected", 793);
				return;
			}
		}

		// Token: 0x06000336 RID: 822 RVA: 0x00011720 File Offset: 0x0000F920
		private void OnHeroSelected(NavalOrderOfBattleHeroItemVM hero, bool isSelected)
		{
			if (this.IsAssignmentDirty)
			{
				return;
			}
			if (isSelected)
			{
				this.SelectedHero = hero;
				this.SelectedShip = null;
				Action clearFormationSelection = this._clearFormationSelection;
				if (clearFormationSelection == null)
				{
					return;
				}
				clearFormationSelection();
				return;
			}
			else
			{
				if (this.SelectedHero == hero)
				{
					this.SelectedHero = null;
					return;
				}
				Debug.FailedAssert("Trying to deselect hero that isn't SelectedHero!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.ViewModelCollection\\OrderOfBattle\\NavalOrderOfBattleVM.cs", "OnHeroSelected", 818);
				return;
			}
		}

		// Token: 0x06000337 RID: 823 RVA: 0x00011784 File Offset: 0x0000F984
		private void OnFormationAcceptCaptain(NavalOrderOfBattleFormationItemVM formation)
		{
			if (this.IsAssignmentDirty)
			{
				return;
			}
			if (this.SelectedHero != null)
			{
				this.AssignCaptainToFormation(this.SelectedHero, formation);
				this.SelectedHero = null;
				return;
			}
			Debug.FailedAssert("OnFormationAcceptCaptain called without a selected hero!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.ViewModelCollection\\OrderOfBattle\\NavalOrderOfBattleVM.cs", "OnFormationAcceptCaptain", 836);
		}

		// Token: 0x06000338 RID: 824 RVA: 0x000117D0 File Offset: 0x0000F9D0
		private void OnFormationAcceptShip(NavalOrderOfBattleFormationItemVM formation)
		{
			if (this.IsAssignmentDirty)
			{
				return;
			}
			if (this.SelectedShip != null)
			{
				this.AssignShipToFormation(this.SelectedShip, formation, false);
				this.SelectedShip = null;
				return;
			}
			Debug.FailedAssert("OnFormationAcceptShip called without a selected ship!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.ViewModelCollection\\OrderOfBattle\\NavalOrderOfBattleVM.cs", "OnFormationAcceptShip", 854);
		}

		// Token: 0x06000339 RID: 825 RVA: 0x00011820 File Offset: 0x0000FA20
		public void ExecuteReturnHeroToPool()
		{
			if (this.IsAssignmentDirty)
			{
				return;
			}
			if (this.SelectedHero != null)
			{
				NavalOrderOfBattleFormationItemVM navalOrderOfBattleFormationItemVM = this.FindFormationOfCaptain(this.SelectedHero);
				if (navalOrderOfBattleFormationItemVM != null)
				{
					this.AssignCaptainToFormation(null, navalOrderOfBattleFormationItemVM);
				}
				this.SelectedHero = null;
				return;
			}
			Debug.FailedAssert("ExecuteReturnHeroToPool called without a selected hero!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.ViewModelCollection\\OrderOfBattle\\NavalOrderOfBattleVM.cs", "ExecuteReturnHeroToPool", 877);
		}

		// Token: 0x0600033A RID: 826 RVA: 0x00011878 File Offset: 0x0000FA78
		public void ExecuteReturnShipToPool()
		{
			if (this.IsAssignmentDirty)
			{
				return;
			}
			if (this.SelectedShip != null)
			{
				NavalOrderOfBattleFormationItemVM navalOrderOfBattleFormationItemVM = this.FindFormationOfShip(this.SelectedShip);
				if (navalOrderOfBattleFormationItemVM != null)
				{
					this.AssignShipToFormation(null, navalOrderOfBattleFormationItemVM, false);
				}
				this.SelectedShip = null;
				return;
			}
			Debug.FailedAssert("ExecuteReturnShipToPool called without a selected ship!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.ViewModelCollection\\OrderOfBattle\\NavalOrderOfBattleVM.cs", "ExecuteReturnShipToPool", 900);
		}

		// Token: 0x0600033B RID: 827 RVA: 0x000118D4 File Offset: 0x0000FAD4
		private void AssignCaptainToFormation(NavalOrderOfBattleHeroItemVM hero, NavalOrderOfBattleFormationItemVM formation)
		{
			if (formation == null)
			{
				Debug.FailedAssert("Trying to assign hero to null formation!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.ViewModelCollection\\OrderOfBattle\\NavalOrderOfBattleVM.cs", "AssignCaptainToFormation", 908);
				return;
			}
			bool flag = false;
			if (this._navalDeploymentController.IsShipAssignedToFormation(formation.Formation))
			{
				flag = this._navalDeploymentController.TryAssignCaptainToFormation((hero != null) ? hero.AgentOrigin : null, formation.Formation);
			}
			if (flag)
			{
				this.RefreshAll();
			}
		}

		// Token: 0x0600033C RID: 828 RVA: 0x0001193C File Offset: 0x0000FB3C
		private bool AssignShipToFormation(NavalOrderOfBattleShipItemVM ship, NavalOrderOfBattleFormationItemVM formation, bool isBatch = false)
		{
			if (formation == null)
			{
				Debug.FailedAssert("Trying to assign ship to null formation!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.ViewModelCollection\\OrderOfBattle\\NavalOrderOfBattleVM.cs", "AssignShipToFormation", 934);
				return false;
			}
			bool flag = this._navalDeploymentController.TryAssignShipToFormation((ship != null) ? ship.ShipOrigin : null, formation.Formation, !isBatch);
			if (flag)
			{
				this.IsAssignmentDirty = true;
			}
			return flag;
		}

		// Token: 0x0600033D RID: 829 RVA: 0x00011994 File Offset: 0x0000FB94
		private void OnSelectionUpdated()
		{
			for (int i = 0; i < this.AllFormations.Count; i++)
			{
				NavalOrderOfBattleFormationItemVM navalOrderOfBattleFormationItemVM = this.AllFormations[i];
				navalOrderOfBattleFormationItemVM.IsAcceptingCaptain = this.HasSelectedHero && this.SelectedHero != navalOrderOfBattleFormationItemVM.Captain && navalOrderOfBattleFormationItemVM.GetCanAcceptCaptain() && this.SelectedHero.GetCanBeUnassignedOrMoved();
				navalOrderOfBattleFormationItemVM.IsAcceptingShip = this.HasSelectedShip && this.SelectedShip != navalOrderOfBattleFormationItemVM.Ship && navalOrderOfBattleFormationItemVM.GetCanAcceptShip() && this.SelectedShip.GetCanBeUnassignedOrMoved();
			}
			this.IsPoolAcceptingHero = this.HasSelectedHero && !this.UnassignedHeroes.Contains(this.SelectedHero) && this.SelectedHero.GetCanBeUnassignedOrMoved();
			this.IsPoolAcceptingShip = this.HasSelectedShip && !this.UnassignedShips.Contains(this.SelectedShip) && this.SelectedShip.GetCanBeUnassignedOrMoved();
		}

		// Token: 0x0600033E RID: 830 RVA: 0x00011A89 File Offset: 0x0000FC89
		private void OnPlayerShipsUpdated()
		{
			this.RefreshAll();
			if (this._finalizeInitializationOnNextUpdate)
			{
				this.FinalizeInitialization();
				this._finalizeInitializationOnNextUpdate = false;
			}
		}

		// Token: 0x0600033F RID: 831 RVA: 0x00011AA8 File Offset: 0x0000FCA8
		private NavalOrderOfBattleFormationItemVM FindFormationOfCaptain(NavalOrderOfBattleHeroItemVM hero)
		{
			for (int i = 0; i < this.AllFormations.Count; i++)
			{
				if (this.AllFormations[i].Captain == hero)
				{
					return this.AllFormations[i];
				}
			}
			return null;
		}

		// Token: 0x06000340 RID: 832 RVA: 0x00011AF0 File Offset: 0x0000FCF0
		private NavalOrderOfBattleFormationItemVM FindFormationOfShip(NavalOrderOfBattleShipItemVM ship)
		{
			for (int i = 0; i < this.AllFormations.Count; i++)
			{
				if (this.AllFormations[i].Ship == ship)
				{
					return this.AllFormations[i];
				}
			}
			return null;
		}

		// Token: 0x06000341 RID: 833 RVA: 0x00011B38 File Offset: 0x0000FD38
		private int GetTroopCountWithFilter(DeploymentFormationClass orderOfBattleFormationClass, FormationFilterType filterType)
		{
			int num = 0;
			List<FormationClass> formationClasses = OrderOfBattleFormationExtensions.GetFormationClasses(orderOfBattleFormationClass);
			foreach (NavalOrderOfBattleFormationItemVM navalOrderOfBattleFormationItemVM in this.AllFormations)
			{
				List<FormationClass> formationClasses2 = OrderOfBattleFormationExtensions.GetFormationClasses(navalOrderOfBattleFormationItemVM.SelectedClass);
				if (formationClasses.Intersect<FormationClass>(formationClasses2).Any<FormationClass>())
				{
					switch (filterType)
					{
					case 1:
						num += navalOrderOfBattleFormationItemVM.Formation.GetCountOfUnitsWithCondition((Agent a) => a.HasShieldCached);
						break;
					case 3:
						num += navalOrderOfBattleFormationItemVM.Formation.GetCountOfUnitsWithCondition((Agent a) => a.HasThrownCached);
						break;
					case 4:
						num += navalOrderOfBattleFormationItemVM.Formation.GetCountOfUnitsWithCondition((Agent a) => MissionGameModels.Current.AgentStatCalculateModel.HasHeavyArmor(a));
						break;
					case 5:
						num += navalOrderOfBattleFormationItemVM.Formation.GetCountOfUnitsWithCondition((Agent a) => a.Character.GetBattleTier() >= 4);
						break;
					case 6:
						num += navalOrderOfBattleFormationItemVM.Formation.GetCountOfUnitsWithCondition((Agent a) => a.Character.GetBattleTier() <= 3);
						break;
					}
				}
			}
			return num;
		}

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x06000342 RID: 834 RVA: 0x00011CD4 File Offset: 0x0000FED4
		// (set) Token: 0x06000343 RID: 835 RVA: 0x00011CDC File Offset: 0x0000FEDC
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x06000344 RID: 836 RVA: 0x00011CFA File Offset: 0x0000FEFA
		// (set) Token: 0x06000345 RID: 837 RVA: 0x00011D02 File Offset: 0x0000FF02
		[DataSourceProperty]
		public bool IsAssignmentDirty
		{
			get
			{
				return this._isAssignmentDirty;
			}
			set
			{
				if (value != this._isAssignmentDirty)
				{
					this._isAssignmentDirty = value;
					base.OnPropertyChangedWithValue(value, "IsAssignmentDirty");
				}
			}
		}

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x06000346 RID: 838 RVA: 0x00011D20 File Offset: 0x0000FF20
		// (set) Token: 0x06000347 RID: 839 RVA: 0x00011D28 File Offset: 0x0000FF28
		[DataSourceProperty]
		public bool CanStartMission
		{
			get
			{
				return this._canStartMission;
			}
			set
			{
				if (value != this._canStartMission)
				{
					this._canStartMission = value;
					base.OnPropertyChangedWithValue(value, "CanStartMission");
				}
			}
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000348 RID: 840 RVA: 0x00011D46 File Offset: 0x0000FF46
		// (set) Token: 0x06000349 RID: 841 RVA: 0x00011D4E File Offset: 0x0000FF4E
		[DataSourceProperty]
		public bool IsPlayerGeneral
		{
			get
			{
				return this._isPlayerGeneral;
			}
			set
			{
				if (value != this._isPlayerGeneral)
				{
					this._isPlayerGeneral = value;
					base.OnPropertyChangedWithValue(value, "IsPlayerGeneral");
				}
			}
		}

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x0600034A RID: 842 RVA: 0x00011D6C File Offset: 0x0000FF6C
		// (set) Token: 0x0600034B RID: 843 RVA: 0x00011D74 File Offset: 0x0000FF74
		[DataSourceProperty]
		public bool AreCameraControlsEnabled
		{
			get
			{
				return this._areCameraControlsEnabled;
			}
			set
			{
				if (value != this._areCameraControlsEnabled)
				{
					this._areCameraControlsEnabled = value;
					base.OnPropertyChangedWithValue(value, "AreCameraControlsEnabled");
				}
			}
		}

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x0600034C RID: 844 RVA: 0x00011D92 File Offset: 0x0000FF92
		// (set) Token: 0x0600034D RID: 845 RVA: 0x00011D9A File Offset: 0x0000FF9A
		[DataSourceProperty]
		public bool HasSelectedHero
		{
			get
			{
				return this._hasSelectedHero;
			}
			set
			{
				if (value != this._hasSelectedHero)
				{
					this._hasSelectedHero = value;
					base.OnPropertyChangedWithValue(value, "HasSelectedHero");
				}
			}
		}

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x0600034E RID: 846 RVA: 0x00011DB8 File Offset: 0x0000FFB8
		// (set) Token: 0x0600034F RID: 847 RVA: 0x00011DC0 File Offset: 0x0000FFC0
		[DataSourceProperty]
		public bool HasSelectedShip
		{
			get
			{
				return this._hasSelectedShip;
			}
			set
			{
				if (value != this._hasSelectedShip)
				{
					this._hasSelectedShip = value;
					base.OnPropertyChangedWithValue(value, "HasSelectedShip");
				}
			}
		}

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x06000350 RID: 848 RVA: 0x00011DDE File Offset: 0x0000FFDE
		// (set) Token: 0x06000351 RID: 849 RVA: 0x00011DE6 File Offset: 0x0000FFE6
		[DataSourceProperty]
		public string BeginMissionText
		{
			get
			{
				return this._beginMissionText;
			}
			set
			{
				if (value != this._beginMissionText)
				{
					this._beginMissionText = value;
					base.OnPropertyChangedWithValue<string>(value, "BeginMissionText");
				}
			}
		}

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x06000352 RID: 850 RVA: 0x00011E09 File Offset: 0x00010009
		// (set) Token: 0x06000353 RID: 851 RVA: 0x00011E11 File Offset: 0x00010011
		[DataSourceProperty]
		public string AutoDeployText
		{
			get
			{
				return this._autoDeployText;
			}
			set
			{
				if (value != this._autoDeployText)
				{
					this._autoDeployText = value;
					base.OnPropertyChangedWithValue<string>(value, "AutoDeployText");
				}
			}
		}

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x06000354 RID: 852 RVA: 0x00011E34 File Offset: 0x00010034
		// (set) Token: 0x06000355 RID: 853 RVA: 0x00011E3C File Offset: 0x0001003C
		[DataSourceProperty]
		public NavalOrderOfBattleShipItemVM SelectedShip
		{
			get
			{
				return this._selectedShip;
			}
			set
			{
				if (value != this._selectedShip)
				{
					if (this._selectedShip != null)
					{
						this._selectedShip.IsSelected = false;
					}
					this._selectedShip = value;
					base.OnPropertyChangedWithValue<NavalOrderOfBattleShipItemVM>(value, "SelectedShip");
					this.HasSelectedShip = this._selectedShip != null;
					if (this._selectedShip != null)
					{
						this._selectedShip.IsSelected = true;
					}
					this.OnSelectionUpdated();
				}
			}
		}

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x06000356 RID: 854 RVA: 0x00011EA2 File Offset: 0x000100A2
		// (set) Token: 0x06000357 RID: 855 RVA: 0x00011EAC File Offset: 0x000100AC
		[DataSourceProperty]
		public NavalOrderOfBattleHeroItemVM SelectedHero
		{
			get
			{
				return this._selectedHero;
			}
			set
			{
				if (value != this._selectedHero)
				{
					if (this._selectedHero != null)
					{
						this._selectedHero.IsSelected = false;
					}
					this._selectedHero = value;
					base.OnPropertyChangedWithValue<NavalOrderOfBattleHeroItemVM>(value, "SelectedHero");
					this.HasSelectedHero = this._selectedHero != null;
					if (this._selectedHero != null)
					{
						this._selectedHero.IsSelected = true;
					}
					this.OnSelectionUpdated();
				}
			}
		}

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06000358 RID: 856 RVA: 0x00011F12 File Offset: 0x00010112
		// (set) Token: 0x06000359 RID: 857 RVA: 0x00011F1A File Offset: 0x0001011A
		[DataSourceProperty]
		public MBBindingList<NavalOrderOfBattleFormationItemVM> LeftFormations
		{
			get
			{
				return this._leftFormations;
			}
			set
			{
				if (value != this._leftFormations)
				{
					this._leftFormations = value;
					base.OnPropertyChangedWithValue<MBBindingList<NavalOrderOfBattleFormationItemVM>>(value, "LeftFormations");
				}
			}
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x0600035A RID: 858 RVA: 0x00011F38 File Offset: 0x00010138
		// (set) Token: 0x0600035B RID: 859 RVA: 0x00011F40 File Offset: 0x00010140
		[DataSourceProperty]
		public MBBindingList<NavalOrderOfBattleFormationItemVM> RightFormations
		{
			get
			{
				return this._rightFormations;
			}
			set
			{
				if (value != this._rightFormations)
				{
					this._rightFormations = value;
					base.OnPropertyChangedWithValue<MBBindingList<NavalOrderOfBattleFormationItemVM>>(value, "RightFormations");
				}
			}
		}

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x0600035C RID: 860 RVA: 0x00011F5E File Offset: 0x0001015E
		// (set) Token: 0x0600035D RID: 861 RVA: 0x00011F66 File Offset: 0x00010166
		[DataSourceProperty]
		public MBBindingList<NavalOrderOfBattleHeroItemVM> UnassignedHeroes
		{
			get
			{
				return this._unassignedHeroes;
			}
			set
			{
				if (value != this._unassignedHeroes)
				{
					this._unassignedHeroes = value;
					base.OnPropertyChangedWithValue<MBBindingList<NavalOrderOfBattleHeroItemVM>>(value, "UnassignedHeroes");
				}
			}
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x0600035E RID: 862 RVA: 0x00011F84 File Offset: 0x00010184
		// (set) Token: 0x0600035F RID: 863 RVA: 0x00011F8C File Offset: 0x0001018C
		[DataSourceProperty]
		public MBBindingList<NavalOrderOfBattleShipItemVM> UnassignedShips
		{
			get
			{
				return this._unassignedShips;
			}
			set
			{
				if (value != this._unassignedShips)
				{
					this._unassignedShips = value;
					base.OnPropertyChangedWithValue<MBBindingList<NavalOrderOfBattleShipItemVM>>(value, "UnassignedShips");
				}
			}
		}

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x06000360 RID: 864 RVA: 0x00011FAA File Offset: 0x000101AA
		// (set) Token: 0x06000361 RID: 865 RVA: 0x00011FB2 File Offset: 0x000101B2
		[DataSourceProperty]
		public bool AreHotkeysEnabled
		{
			get
			{
				return this._areHotkeysEnabled;
			}
			set
			{
				if (value != this._areHotkeysEnabled)
				{
					this._areHotkeysEnabled = value;
					base.OnPropertyChangedWithValue(value, "AreHotkeysEnabled");
				}
			}
		}

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x06000362 RID: 866 RVA: 0x00011FD0 File Offset: 0x000101D0
		// (set) Token: 0x06000363 RID: 867 RVA: 0x00011FD8 File Offset: 0x000101D8
		[DataSourceProperty]
		public bool IsPoolAcceptingHero
		{
			get
			{
				return this._isPoolAcceptingHero;
			}
			set
			{
				if (value != this._isPoolAcceptingHero)
				{
					this._isPoolAcceptingHero = value;
					base.OnPropertyChangedWithValue(value, "IsPoolAcceptingHero");
				}
			}
		}

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x06000364 RID: 868 RVA: 0x00011FF6 File Offset: 0x000101F6
		// (set) Token: 0x06000365 RID: 869 RVA: 0x00011FFE File Offset: 0x000101FE
		[DataSourceProperty]
		public bool IsPoolAcceptingShip
		{
			get
			{
				return this._isPoolAcceptingShip;
			}
			set
			{
				if (value != this._isPoolAcceptingShip)
				{
					this._isPoolAcceptingShip = value;
					base.OnPropertyChangedWithValue(value, "IsPoolAcceptingShip");
				}
			}
		}

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x06000366 RID: 870 RVA: 0x0001201C File Offset: 0x0001021C
		// (set) Token: 0x06000367 RID: 871 RVA: 0x00012024 File Offset: 0x00010224
		[DataSourceProperty]
		public HintViewModel CanStartHint
		{
			get
			{
				return this._canStartHint;
			}
			set
			{
				if (value != this._canStartHint)
				{
					this._canStartHint = value;
					base.OnPropertyChangedWithValue<HintViewModel>(value, "CanStartHint");
				}
			}
		}

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x06000368 RID: 872 RVA: 0x00012042 File Offset: 0x00010242
		// (set) Token: 0x06000369 RID: 873 RVA: 0x0001204A File Offset: 0x0001024A
		[DataSourceProperty]
		public bool CanToggleHeroOrShipSelection
		{
			get
			{
				return this._canToggleHeroOrShipSelection;
			}
			set
			{
				if (value != this._canToggleHeroOrShipSelection)
				{
					this._canToggleHeroOrShipSelection = value;
					base.OnPropertyChangedWithValue(value, "CanToggleHeroOrShipSelection");
				}
			}
		}

		// Token: 0x0600036A RID: 874 RVA: 0x00012068 File Offset: 0x00010268
		public void SetDoneInputKey(HotKey hotkey)
		{
			this.DoneInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x0600036B RID: 875 RVA: 0x00012077 File Offset: 0x00010277
		public void SetResetInputKey(HotKey hotkey)
		{
			this.ResetInputKey = InputKeyItemVM.CreateFromHotKey(hotkey, true);
		}

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x0600036C RID: 876 RVA: 0x00012086 File Offset: 0x00010286
		// (set) Token: 0x0600036D RID: 877 RVA: 0x0001208E File Offset: 0x0001028E
		public InputKeyItemVM DoneInputKey
		{
			get
			{
				return this._doneInputKey;
			}
			set
			{
				if (value != this._doneInputKey)
				{
					this._doneInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "DoneInputKey");
				}
			}
		}

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x0600036E RID: 878 RVA: 0x000120AC File Offset: 0x000102AC
		// (set) Token: 0x0600036F RID: 879 RVA: 0x000120B4 File Offset: 0x000102B4
		public InputKeyItemVM ResetInputKey
		{
			get
			{
				return this._resetInputKey;
			}
			set
			{
				if (value != this._resetInputKey)
				{
					this._resetInputKey = value;
					base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "ResetInputKey");
				}
			}
		}

		// Token: 0x04000128 RID: 296
		private readonly MBList<NavalOrderOfBattleFormationItemVM> _allFormations;

		// Token: 0x04000129 RID: 297
		private readonly List<NavalOrderOfBattleHeroItemVM> _allHeroes;

		// Token: 0x0400012A RID: 298
		private readonly List<NavalOrderOfBattleShipItemVM> _allShips;

		// Token: 0x0400012D RID: 301
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x0400012E RID: 302
		private NavalDeploymentMissionController _navalDeploymentController;

		// Token: 0x0400012F RID: 303
		private OrderController _orderController;

		// Token: 0x04000130 RID: 304
		private NavalOrderOfBattleCampaignBehavior _navalOrderOfBattleCampaignBehavior;

		// Token: 0x04000131 RID: 305
		private AssignPlayerRoleInTeamMissionController _assignPlayerRoleInTeamMissioncontroller;

		// Token: 0x04000132 RID: 306
		private readonly Action<NavalOrderOfBattleFormationItemVM> _onFormationSelected;

		// Token: 0x04000133 RID: 307
		private readonly Action _clearFormationSelection;

		// Token: 0x04000134 RID: 308
		private readonly Action _onAutoDeploy;

		// Token: 0x04000135 RID: 309
		private readonly Action _onBeginMission;

		// Token: 0x04000136 RID: 310
		private readonly Mission _mission;

		// Token: 0x04000137 RID: 311
		private readonly TextObject _formationsDisabledHintGeneral = new TextObject("{=ZixS1b4u}You're not leading this battle.", null);

		// Token: 0x04000138 RID: 312
		private readonly TextObject _formationsDisabledHintAllies = new TextObject("{=O4n4SAqo}Formation is reserved for allied parties.", null);

		// Token: 0x04000139 RID: 313
		private readonly TextObject _formationsDisabledHintSkills = new TextObject("{=Vs5NavCd}You do not have enough skills/perks for this formation.", null);

		// Token: 0x0400013A RID: 314
		private readonly TextObject _formationsDisabledHintShips = new TextObject("{=bID6axoH}You do not have enough ships for this formation.", null);

		// Token: 0x0400013B RID: 315
		private bool _finalizeInitializationOnNextUpdate;

		// Token: 0x0400013C RID: 316
		private bool _isLoadingConfigurationAgents;

		// Token: 0x0400013D RID: 317
		private bool _isEnabled;

		// Token: 0x0400013E RID: 318
		private bool _isAssignmentDirty;

		// Token: 0x0400013F RID: 319
		private bool _canStartMission;

		// Token: 0x04000140 RID: 320
		private bool _isPlayerGeneral;

		// Token: 0x04000141 RID: 321
		private bool _areCameraControlsEnabled;

		// Token: 0x04000142 RID: 322
		private bool _hasSelectedHero;

		// Token: 0x04000143 RID: 323
		private bool _hasSelectedShip;

		// Token: 0x04000144 RID: 324
		private string _beginMissionText;

		// Token: 0x04000145 RID: 325
		private string _autoDeployText;

		// Token: 0x04000146 RID: 326
		private NavalOrderOfBattleShipItemVM _selectedShip;

		// Token: 0x04000147 RID: 327
		private NavalOrderOfBattleHeroItemVM _selectedHero;

		// Token: 0x04000148 RID: 328
		private MBBindingList<NavalOrderOfBattleFormationItemVM> _leftFormations;

		// Token: 0x04000149 RID: 329
		private MBBindingList<NavalOrderOfBattleFormationItemVM> _rightFormations;

		// Token: 0x0400014A RID: 330
		private MBBindingList<NavalOrderOfBattleHeroItemVM> _unassignedHeroes;

		// Token: 0x0400014B RID: 331
		private MBBindingList<NavalOrderOfBattleShipItemVM> _unassignedShips;

		// Token: 0x0400014C RID: 332
		private bool _areHotkeysEnabled;

		// Token: 0x0400014D RID: 333
		private bool _isPoolAcceptingHero;

		// Token: 0x0400014E RID: 334
		private bool _isPoolAcceptingShip;

		// Token: 0x0400014F RID: 335
		private HintViewModel _canStartHint;

		// Token: 0x04000150 RID: 336
		private bool _canToggleHeroOrShipSelection;

		// Token: 0x04000151 RID: 337
		private InputKeyItemVM _doneInputKey;

		// Token: 0x04000152 RID: 338
		private InputKeyItemVM _resetInputKey;
	}
}
