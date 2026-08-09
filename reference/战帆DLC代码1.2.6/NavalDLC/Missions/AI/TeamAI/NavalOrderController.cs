using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NetworkMessages.FromClient;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.AI.TeamAI
{
	// Token: 0x020000EC RID: 236
	internal class NavalOrderController : OrderController
	{
		// Token: 0x060011FE RID: 4606 RVA: 0x00082A8B File Offset: 0x00080C8B
		public NavalOrderController(Mission mission, Team team, Agent owner)
			: base(mission, team, owner)
		{
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
		}

		// Token: 0x060011FF RID: 4607 RVA: 0x00082AA8 File Offset: 0x00080CA8
		protected override void SelectAllFormations(Agent selectorAgent, bool uiFeedback)
		{
			if (GameNetwork.IsClient)
			{
				GameNetwork.BeginModuleEventAsClient();
				GameNetwork.WriteMessage(new SelectAllFormations());
				GameNetwork.EndModuleEventAsClient();
			}
			if (uiFeedback && selectorAgent != null && base.AreGesturesEnabled())
			{
				selectorAgent.MakeVoice(SkinVoiceManager.VoiceType.Everyone, 2);
			}
			this._selectedFormations.Clear();
			IEnumerable<Formation> enumerable = this.Team.FormationsIncludingEmpty.Where<Formation>((Formation f) => this.IsFormationSelectable(f, selectorAgent));
			if (enumerable.Count<Formation>() == 1)
			{
				this._selectedFormations.Add(enumerable.First<Formation>());
			}
			else
			{
				foreach (Formation formation in enumerable)
				{
					if (!NavalDLCHelpers.IsAgentCaptainOfFormationShip(selectorAgent, formation))
					{
						this._selectedFormations.Add(formation);
					}
				}
			}
			base.OnSelectedFormationsCollectionChanged();
		}

		// Token: 0x06001200 RID: 4608 RVA: 0x00082BA0 File Offset: 0x00080DA0
		protected override void SelectFormation(Formation formation, Agent selectorAgent)
		{
			if (!this._selectedFormations.Contains(formation) && base.IsFormationSelectable(formation, selectorAgent))
			{
				if (GameNetwork.IsClient)
				{
					GameNetwork.BeginModuleEventAsClient();
					GameNetwork.WriteMessage(new SelectFormation(formation.Index));
					GameNetwork.EndModuleEventAsClient();
				}
				if (selectorAgent != null && base.AreGesturesEnabled())
				{
					OrderController.PlayFormationSelectedGesture(formation, selectorAgent);
				}
				if (NavalDLCHelpers.IsAgentCaptainOfFormationShip(selectorAgent, formation))
				{
					this._selectedFormations.Clear();
				}
				else
				{
					this._selectedFormations.RemoveAll((Formation x) => NavalDLCHelpers.IsAgentCaptainOfFormationShip(selectorAgent, x));
				}
				this._selectedFormations.Add(formation);
				base.OnSelectedFormationsCollectionChanged();
			}
		}

		// Token: 0x06001201 RID: 4609 RVA: 0x00082C5F File Offset: 0x00080E5F
		public override void SetOrderWithTwoPositions(OrderType orderType, WorldPosition position1, WorldPosition position2)
		{
			this.SetOrderWithPosition(orderType, position1);
		}

		// Token: 0x06001202 RID: 4610 RVA: 0x00082C6C File Offset: 0x00080E6C
		public override void SetOrderWithPosition(OrderType orderType, WorldPosition position)
		{
			base.BeforeSetOrder(orderType);
			this.SetSkirmishState(false);
			this.SetDefensiveState(false);
			MBList<Formation> mblist = Extensions.ToMBList<Formation>((base.SelectedFormations[0].Team.TeamAI as TeamAINavalComponent).TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.Where<Formation>((Formation sf) => base.SelectedFormations.Contains(sf)));
			for (int i = 0; i < mblist.Count; i++)
			{
				Formation formation = mblist[i];
				float num = (-(((float)mblist.Count - 1f) * 0.5f) + (float)i) * 20f;
				Vec2 vec = position.AsVec2 + num * ((base.SelectedFormations[0].Team.TeamAI as TeamAINavalComponent).TeamNavalQuerySystem.AverageEnemyShipPosition - (base.SelectedFormations[0].Team.TeamAI as TeamAINavalComponent).TeamNavalQuerySystem.AverageShipPosition).RightVec().Normalized();
				MissionShip missionShip;
				this._navalShipsLogic.GetShip(formation.Team.TeamSide, formation.FormationIndex, out missionShip);
				if (!missionShip.IsPlayerControlled)
				{
					missionShip.ShipOrder.SetShipMovementOrder(in vec);
				}
			}
			base.FireOnOrderIssued(orderType, mblist, this, Array.Empty<object>());
		}

		// Token: 0x06001203 RID: 4611 RVA: 0x00082DC0 File Offset: 0x00080FC0
		public override void SetOrder(OrderType orderType)
		{
			switch (orderType)
			{
			case 4:
			case 5:
				base.SetOrder(orderType);
				this.SetDefensiveState(false);
				goto IL_00EA;
			case 6:
				base.BeforeSetOrder(orderType);
				this.SetNavalStop();
				goto IL_00EA;
			case 7:
				base.BeforeSetOrder(orderType);
				this.SetNavalFollowMeOrder();
				this.SetSkirmishState(false);
				this.SetDefensiveState(false);
				goto IL_00EA;
			case 8:
			case 10:
			case 11:
			case 13:
				break;
			case 9:
				base.BeforeSetOrder(orderType);
				this.SetNavalRetreat();
				goto IL_00EA;
			case 12:
				base.BeforeSetOrder(orderType);
				this.SetNavalEngageWithTargetFormation(null);
				goto IL_00EA;
			case 14:
			case 15:
				goto IL_00EA;
			default:
				if (orderType == 34)
				{
					base.BeforeSetOrder(orderType);
					this.SetNavalTroopsDefensive();
					this.SetSkirmishState(false);
					this.SetDefensiveState(true);
					goto IL_00EA;
				}
				if (orderType == 35)
				{
					base.BeforeSetOrder(orderType);
					this.SetNavalSkirmishWithTargetFormation(null);
					this.SetSkirmishState(true);
					this.SetDefensiveState(false);
					goto IL_00EA;
				}
				break;
			}
			base.SetOrder(orderType);
			IL_00EA:
			base.FireOnOrderIssued(orderType, base.SelectedFormations, this, Array.Empty<object>());
		}

		// Token: 0x06001204 RID: 4612 RVA: 0x00082ECA File Offset: 0x000810CA
		public override void SetOrderWithAgent(OrderType orderType, Agent agent)
		{
			base.SetOrderWithAgent(orderType, agent);
			if (!NavalDLCHelpers.IsShipOrdersAvailable())
			{
				this.SetSkirmishState(false);
				this.SetDefensiveState(false);
			}
		}

		// Token: 0x06001205 RID: 4613 RVA: 0x00082EEC File Offset: 0x000810EC
		private void SetSkirmishState(bool isSkirmishing)
		{
			for (int i = 0; i < base.SelectedFormations.Count; i++)
			{
				base.SelectedFormations[i].SetRidingOrder(isSkirmishing ? RidingOrder.RidingOrderDismount : RidingOrder.RidingOrderFree);
			}
		}

		// Token: 0x06001206 RID: 4614 RVA: 0x00082F30 File Offset: 0x00081130
		private void SetDefensiveState(bool isDefensive)
		{
			for (int i = 0; i < base.SelectedFormations.Count; i++)
			{
				base.SelectedFormations[i].SetRidingOrder(isDefensive ? RidingOrder.RidingOrderMount : RidingOrder.RidingOrderFree);
			}
		}

		// Token: 0x06001207 RID: 4615 RVA: 0x00082F74 File Offset: 0x00081174
		public override void SetOrderWithFormation(OrderType orderType, Formation orderFormation)
		{
			if (orderType == 12)
			{
				base.BeforeSetOrder(orderType);
				this.SetNavalEngageWithTargetFormation(orderFormation);
				base.FireOnOrderIssued(orderType, base.SelectedFormations, this, Array.Empty<object>());
			}
			else if (orderType == 35)
			{
				base.BeforeSetOrder(orderType);
				this.SetNavalSkirmishWithTargetFormation(orderFormation);
				base.FireOnOrderIssued(orderType, base.SelectedFormations, this, Array.Empty<object>());
			}
			else
			{
				base.SetOrderWithFormation(orderType, orderFormation);
			}
			this.SetSkirmishState(false);
			this.SetDefensiveState(false);
		}

		// Token: 0x06001208 RID: 4616 RVA: 0x00082FE7 File Offset: 0x000811E7
		public override void SetOrderWithOrderableObject(IOrderable target)
		{
			base.BeforeSetOrder(7);
			this.SetNavalFollowOrder(target as MissionShip);
			base.FireOnOrderIssued(7, base.SelectedFormations, this, Array.Empty<object>());
			this.SetSkirmishState(false);
			this.SetDefensiveState(false);
		}

		// Token: 0x06001209 RID: 4617 RVA: 0x00083020 File Offset: 0x00081220
		private void SetNavalFollowOrder(MissionShip targetShip)
		{
			MBList<Formation> mblist = Extensions.ToMBList<Formation>((base.SelectedFormations[0].Team.TeamAI as TeamAINavalComponent).TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.Where<Formation>((Formation sf) => base.SelectedFormations.Contains(sf)));
			for (int i = 0; i < mblist.Count; i++)
			{
				Formation formation = base.SelectedFormations[i];
				float num = (-(((float)mblist.Count - 1f) * 0.5f) + (float)i) * 20f;
				MissionShip missionShip;
				this._navalShipsLogic.GetShip(formation.Team.TeamSide, formation.FormationIndex, out missionShip);
				if (missionShip != targetShip)
				{
					missionShip.ShipOrder.SetShipFollowOrder(targetShip, num);
					missionShip.ShipOrder.SetCutLoose(true);
				}
			}
		}

		// Token: 0x0600120A RID: 4618 RVA: 0x000830E4 File Offset: 0x000812E4
		private void SetNavalFollowMeOrder()
		{
			MissionShip formationShip = Agent.Main.GetComponent<AgentNavalComponent>().FormationShip;
			this.SetNavalFollowOrder(formationShip);
		}

		// Token: 0x0600120B RID: 4619 RVA: 0x00083108 File Offset: 0x00081308
		private void SetNavalEngageWithTargetFormation(Formation targetFormation)
		{
			foreach (Formation formation in base.SelectedFormations)
			{
				if (targetFormation != null || formation.CachedClosestEnemyFormation != null)
				{
					bool flag = targetFormation != null;
					MissionShip missionShip;
					this._navalShipsLogic.GetShip(formation.Team.TeamSide, formation.FormationIndex, out missionShip);
					if (flag)
					{
						MissionShip missionShip2;
						this._navalShipsLogic.GetShip(targetFormation.Team.TeamSide, targetFormation.FormationIndex, out missionShip2);
						missionShip.ShipOrder.SetShipEngageOrder(missionShip2);
						missionShip.ShipOrder.SetBoardingTargetShip(missionShip2);
					}
					else
					{
						missionShip.ShipOrder.SetShipEngageOrder(true);
						if (missionShip.ShipOrder.TargetShip != null)
						{
							missionShip.ShipOrder.SetBoardingTargetShip(missionShip.ShipOrder.TargetShip);
						}
					}
				}
			}
		}

		// Token: 0x0600120C RID: 4620 RVA: 0x000831F8 File Offset: 0x000813F8
		private void SetNavalSkirmishWithTargetFormation(Formation targetFormation)
		{
			foreach (Formation formation in base.SelectedFormations)
			{
				if (targetFormation != null || formation.CachedClosestEnemyFormation != null)
				{
					MissionShip missionShip;
					this._navalShipsLogic.GetShip(formation.Team.TeamSide, formation.FormationIndex, out missionShip);
					if (targetFormation != null)
					{
						MissionShip missionShip2;
						this._navalShipsLogic.GetShip(targetFormation.Team.TeamSide, targetFormation.FormationIndex, out missionShip2);
						missionShip.ShipOrder.SetShipSkirmishOrder(missionShip2);
					}
					else
					{
						missionShip.ShipOrder.SetShipSkirmishOrder(true);
					}
					missionShip.ShipOrder.SetCutLoose(true);
				}
			}
		}

		// Token: 0x0600120D RID: 4621 RVA: 0x000832BC File Offset: 0x000814BC
		private void SetNavalStop()
		{
			foreach (Formation formation in base.SelectedFormations)
			{
				MissionShip missionShip;
				this._navalShipsLogic.GetShip(formation.Team.TeamSide, formation.FormationIndex, out missionShip);
				missionShip.ShipOrder.SetShipStopOrder();
				missionShip.ShipOrder.SetBoardingTargetShip(null);
				missionShip.ShipOrder.SetCutLoose(false);
			}
		}

		// Token: 0x0600120E RID: 4622 RVA: 0x0008334C File Offset: 0x0008154C
		private void SetNavalRetreat()
		{
			foreach (Formation formation in base.SelectedFormations)
			{
				MissionShip missionShip;
				this._navalShipsLogic.GetShip(formation.Team.TeamSide, formation.FormationIndex, out missionShip);
				missionShip.ShipOrder.SetShipRetreatOrder();
				missionShip.ShipOrder.SetCutLoose(true);
			}
		}

		// Token: 0x0600120F RID: 4623 RVA: 0x000833D0 File Offset: 0x000815D0
		private void SetNavalTroopsAggressive()
		{
			foreach (Formation formation in base.SelectedFormations)
			{
				formation.SetMovementOrder(MovementOrder.MovementOrderCharge);
				formation.SetRidingOrder(RidingOrder.RidingOrderDismount);
			}
		}

		// Token: 0x06001210 RID: 4624 RVA: 0x00083430 File Offset: 0x00081630
		public static MovementOrder GetNavalDefensiveMovementOrder(MissionShip missionShip)
		{
			WorldPosition worldPosition;
			missionShip.GetWorldPositionOnDeck(out worldPosition);
			return MovementOrder.MovementOrderMove(worldPosition);
		}

		// Token: 0x06001211 RID: 4625 RVA: 0x0008344C File Offset: 0x0008164C
		private void SetNavalTroopsDefensive()
		{
			foreach (Formation formation in base.SelectedFormations)
			{
				MissionShip missionShip;
				this._navalShipsLogic.GetShip(formation.Team.TeamSide, formation.FormationIndex, out missionShip);
				if (missionShip != null)
				{
					missionShip.SetPositioningOrdersToRallyPoint(true, true);
				}
				formation.SetRidingOrder(RidingOrder.RidingOrderMount);
			}
		}

		// Token: 0x04000A22 RID: 2594
		private readonly NavalShipsLogic _navalShipsLogic;
	}
}
