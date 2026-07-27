using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions.AI.TeamAI;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.Objects.UsableMachines;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.AI.Behaviors
{
	// Token: 0x020000F4 RID: 244
	public sealed class BehaviorNavalDefendInLine : NavalBehaviorComponent
	{
		// Token: 0x06001259 RID: 4697 RVA: 0x00086140 File Offset: 0x00084340
		public BehaviorNavalDefendInLine(Formation formation)
			: base(formation)
		{
			base.BehaviorCoherence = 0.8f;
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this._formationMainShip = this._navalShipsLogic.GetShipAssignment(base.Formation.Team.TeamSide, base.Formation.FormationIndex).MissionShip;
			List<WeakGameEntity> list = new List<WeakGameEntity>();
			this._formationShipAttachmentMachines = Extensions.ToMBList<ShipAttachmentMachine>(from ce in list
				where ce.HasScriptOfType<ShipAttachmentMachine>()
				select ce.GetFirstScriptOfType<ShipAttachmentMachine>());
			Extensions.ToMBList<ShipAttachmentPointMachine>(from ce in list
				where ce.HasScriptOfType<ShipAttachmentPointMachine>()
				select ce.GetFirstScriptOfType<ShipAttachmentPointMachine>());
			this._navalTeamAI = base.Formation.Team.TeamAI as TeamAINavalComponent;
			this.CalculateCurrentOrder();
			this._boardShipSubtask = new NavalBehaviorBoardShipSubtask(this._formationMainShip);
		}

		// Token: 0x0600125A RID: 4698 RVA: 0x0008627C File Offset: 0x0008447C
		public override void RefreshShipReferences()
		{
			this._formationMainShip = this._navalShipsLogic.GetShipAssignment(base.Formation.Team.TeamSide, base.Formation.FormationIndex).MissionShip;
			this._leftAllyShip = null;
			this._rightAllyShip = null;
			if (this._navalLineOrder >= this._navalTeamAI.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.Count || this._navalLineOrder < 0)
			{
				this._navalLineOrder = 0;
			}
			this.SetTargetShipSideAndOrder(this._tacticallyOnRightSide, this._navalLineOrder, this._isAnchor);
			if (this._helpedAllyShip != null)
			{
				this._helpedAllyShip = (this._tacticallyOnRightSide ? this._leftAllyShip : this._rightAllyShip);
			}
		}

		// Token: 0x0600125B RID: 4699 RVA: 0x00086334 File Offset: 0x00084534
		public void SetTargetShipSideAndOrder(bool rightSide, int navalLineOrder, bool isAnchor)
		{
			if (this._navalTeamAI.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.Count > 0)
			{
				this._isAnchor = isAnchor;
				this._tacticallyOnRightSide = rightSide;
				this._actualRightSide = rightSide;
				this._navalLineOrder = navalLineOrder;
				Formation formation = ((navalLineOrder > 0) ? this._navalTeamAI.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.ElementAt<Formation>(this._navalLineOrder - 1) : null);
				Formation formation2 = ((navalLineOrder < this._navalTeamAI.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.Count - 1) ? this._navalTeamAI.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.ElementAt<Formation>(this._navalLineOrder + 1) : null);
				if (formation != null)
				{
					this._navalShipsLogic.GetShip(base.Formation.Team.TeamSide, formation.FormationIndex, out this._leftAllyShip);
				}
				if (formation2 != null)
				{
					this._navalShipsLogic.GetShip(base.Formation.Team.TeamSide, formation2.FormationIndex, out this._rightAllyShip);
				}
				if (this._tacticallyOnRightSide)
				{
					this._allyShip = this._leftAllyShip;
					return;
				}
				this._allyShip = this._rightAllyShip;
			}
		}

		// Token: 0x0600125C RID: 4700 RVA: 0x00086450 File Offset: 0x00084650
		protected override void CalculateCurrentOrder()
		{
			if (this._navalShipsLogic == null || base.Formation.CachedClosestEnemyFormation == null || this._allyShip == null)
			{
				base.CurrentOrder = MovementOrder.MovementOrderStop;
				return;
			}
			if (this._formationMainShip == null || !this._formationMainShip.SearchShipConnection(null, true, true, true, false))
			{
				base.CurrentOrder = MovementOrder.MovementOrderStop;
				return;
			}
			if (this._currentState == BehaviorNavalDefendInLine.ShipDefenseState.BeingBoarded)
			{
				base.CurrentOrder = this._formationMainShip.GetMovementOrderToRallyPoint();
				this.CurrentFacingOrder = this._formationMainShip.GetFacingOrderToRallyPoint();
				return;
			}
			base.CurrentOrder = MovementOrder.MovementOrderCharge;
			this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
		}

		// Token: 0x0600125D RID: 4701 RVA: 0x000864F0 File Offset: 0x000846F0
		private void CalculateAndSetShipOrders()
		{
			MatrixFrame globalFrame = this._formationMainShip.GlobalFrame;
			Vec2 vec = globalFrame.origin.AsVec2;
			Vec2 vec2 = globalFrame.rotation.f.AsVec2.Normalized();
			MissionShip missionShip = null;
			BehaviorNavalDefendInLine.ShipDefenseState currentState = this._currentState;
			if (currentState != BehaviorNavalDefendInLine.ShipDefenseState.StandInLine)
			{
				if (currentState - BehaviorNavalDefendInLine.ShipDefenseState.GoingToHelp <= 1)
				{
					this._boardShipSubtask.CalculateShipOrders(out vec, out vec2, out missionShip);
				}
			}
			else
			{
				Vec2 vec3 = (this._navalTeamAI.TeamNavalQuerySystem.AverageEnemyShipPosition - this._navalTeamAI.TeamNavalQuerySystem.AverageShipPosition).Normalized();
				if (this._isAnchor)
				{
					bool flag = false;
					if (this._navalTeamAI.UseSpawnPathApproachPosition)
					{
						this._navalTeamAI.GetRiverApproachPosition(out vec, out vec2);
						MatrixFrame matrixFrame = this._formationMainShip.GlobalFrame;
						bool flag2;
						if (matrixFrame.origin.AsVec2.DistanceSquared(vec) > 900f)
						{
							matrixFrame = this._formationMainShip.GlobalFrame;
							flag2 = matrixFrame.origin.AsVec2.Distance(this._navalTeamAI.TeamNavalQuerySystem.AverageEnemyShipPosition) - vec.Distance(this._navalTeamAI.TeamNavalQuerySystem.AverageEnemyShipPosition) >= 50f;
						}
						else
						{
							flag2 = false;
						}
						flag = flag2;
					}
					if (!flag)
					{
						vec2 = vec3;
						vec = globalFrame.origin.AsVec2 + vec2 * 15f;
					}
				}
				else
				{
					MatrixFrame globalFrame2 = this._allyShip.GlobalFrame;
					(globalFrame.origin - globalFrame2.origin).Normalize();
					MatrixFrame matrixFrame;
					Vec2 vec4;
					if (this._actualRightSide)
					{
						if (!this._navalTeamAI.UseSpawnPathApproachPosition)
						{
							vec4 = vec3.RightVec();
						}
						else
						{
							matrixFrame = this._allyShip.GlobalFrame;
							vec4 = matrixFrame.rotation.f.AsVec2.RightVec();
						}
					}
					else if (!this._navalTeamAI.UseSpawnPathApproachPosition)
					{
						vec4 = vec3.LeftVec();
					}
					else
					{
						matrixFrame = this._allyShip.GlobalFrame;
						vec4 = matrixFrame.rotation.f.AsVec2.LeftVec();
					}
					matrixFrame = this._allyShip.GlobalFrame;
					vec = matrixFrame.origin.AsVec2 + vec3 * 10f + vec4 * 30f;
					Vec2 vec5 = vec;
					matrixFrame = this._formationMainShip.GlobalFrame;
					float num = (vec5 - matrixFrame.origin.AsVec2).DotProduct(vec3);
					if (num < 0f)
					{
						vec += num * vec3;
					}
					vec2 = vec3;
				}
			}
			if (this._formationMainShip.IsFormationAndShipAIControlled)
			{
				this._formationMainShip.ShipOrder.SetShipMovementOrder(vec, in vec2);
				this._formationMainShip.ShipOrder.SetBoardingTargetShip(missionShip);
			}
		}

		// Token: 0x0600125E RID: 4702 RVA: 0x000867D8 File Offset: 0x000849D8
		private void CheckAndSwitchState()
		{
			if (base.Formation.CachedClosestEnemyFormation != null)
			{
				switch (this._currentState)
				{
				case BehaviorNavalDefendInLine.ShipDefenseState.StandInLine:
					if (this._formationMainShip.SearchShipConnection(null, true, true, true, true))
					{
						this._currentState = BehaviorNavalDefendInLine.ShipDefenseState.BeingBoarded;
						return;
					}
					if (this._leftAllyShip != null && this._leftAllyShip.SearchShipConnection(null, true, true, true, true))
					{
						this._currentState = BehaviorNavalDefendInLine.ShipDefenseState.GoingToHelp;
						this._helpedAllyShip = this._leftAllyShip;
						this._boardShipSubtask.SetTargetShipAndSide(this._helpedAllyShip, this._tacticallyOnRightSide);
						return;
					}
					if (this._rightAllyShip != null && this._rightAllyShip.SearchShipConnection(null, true, true, true, true))
					{
						this._currentState = BehaviorNavalDefendInLine.ShipDefenseState.GoingToHelp;
						this._helpedAllyShip = this._rightAllyShip;
						this._boardShipSubtask.SetTargetShipAndSide(this._helpedAllyShip, this._tacticallyOnRightSide);
						return;
					}
					if (this._formationMainShip.GetIsConnected())
					{
						this._currentState = BehaviorNavalDefendInLine.ShipDefenseState.HelpingFinishedStuckBoarded;
						return;
					}
					break;
				case BehaviorNavalDefendInLine.ShipDefenseState.BeingBoarded:
					if (!this._formationMainShip.GetIsConnected())
					{
						this._currentState = BehaviorNavalDefendInLine.ShipDefenseState.StandInLine;
						return;
					}
					if (!this._formationMainShip.SearchShipConnection(null, true, true, true, true))
					{
						this._currentState = BehaviorNavalDefendInLine.ShipDefenseState.HelpingFinishedStuckBoarded;
						return;
					}
					if (!this._formationMainShip.ShipOrder.IsEnemyOnShip)
					{
						this._formationMainShip.SearchShipConnection(null, true, false, true, true);
						return;
					}
					break;
				case BehaviorNavalDefendInLine.ShipDefenseState.GoingToHelp:
					if (this._formationMainShip.SearchShipConnection(this._helpedAllyShip, true, false, false, true))
					{
						this._currentState = BehaviorNavalDefendInLine.ShipDefenseState.HelpingFriend;
						return;
					}
					if (this._helpedAllyShip == null || !this._helpedAllyShip.SearchShipConnection(null, true, true, true, true))
					{
						this._currentState = BehaviorNavalDefendInLine.ShipDefenseState.StandInLine;
						this._helpedAllyShip = null;
						return;
					}
					if (this._formationMainShip.SearchShipConnection(null, true, true, true, true))
					{
						this._currentState = BehaviorNavalDefendInLine.ShipDefenseState.BeingBoarded;
						this._helpedAllyShip = null;
						return;
					}
					if (this._formationMainShip.GetIsConnected())
					{
						this._currentState = BehaviorNavalDefendInLine.ShipDefenseState.HelpingFinishedStuckBoarded;
						this._helpedAllyShip = null;
						return;
					}
					break;
				case BehaviorNavalDefendInLine.ShipDefenseState.HelpingFriend:
					if (!this._formationMainShip.SearchShipConnection(this._helpedAllyShip, true, false, false, true))
					{
						this._currentState = BehaviorNavalDefendInLine.ShipDefenseState.GoingToHelp;
						this._boardShipSubtask.SetTargetShipAndSide(this._helpedAllyShip, this._tacticallyOnRightSide);
					}
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x0600125F RID: 4703 RVA: 0x000869E0 File Offset: 0x00084BE0
		public override void OnDeploymentFinished()
		{
			base.OnDeploymentFinished();
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this._formationMainShip = this._navalShipsLogic.GetShipAssignment(base.Formation.Team.TeamSide, base.Formation.FormationIndex).MissionShip;
			this._currentState = BehaviorNavalDefendInLine.ShipDefenseState.StandInLine;
		}

		// Token: 0x06001260 RID: 4704 RVA: 0x00086A3B File Offset: 0x00084C3B
		public override void ResetBehavior()
		{
			base.ResetBehavior();
			this._formationMainShip = this._navalShipsLogic.GetShipAssignment(base.Formation.Team.TeamSide, base.Formation.FormationIndex).MissionShip;
			this._currentState = BehaviorNavalDefendInLine.ShipDefenseState.StandInLine;
		}

		// Token: 0x06001261 RID: 4705 RVA: 0x00086A7C File Offset: 0x00084C7C
		protected override void OnBehaviorActivatedAux()
		{
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this._formationMainShip = this._navalShipsLogic.GetShipAssignment(base.Formation.Team.TeamSide, base.Formation.FormationIndex).MissionShip;
			this._boardShipSubtask.SetOwnerShip(this._formationMainShip);
			this._boardShipSubtask.SetTargetShipAndSide(null, this._tacticallyOnRightSide ^ this._swapSide);
			this._currentState = BehaviorNavalDefendInLine.ShipDefenseState.StandInLine;
			this._formationMainShip.ShipOrder.SetBoardingTargetShip(null);
			this._formationMainShip.ShipOrder.SetCutLoose(false);
			this._formationMainShip.ShipOrder.SetOrderOarsmenLevel(2);
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.SetFacingOrder(this.CurrentFacingOrder);
			base.Formation.SetArrangementOrder(ArrangementOrder.ArrangementOrderLine);
			base.Formation.SetFiringOrder(FiringOrder.FiringOrderFireAtWill);
			base.Formation.SetFormOrder(FormOrder.FormOrderWide, true);
		}

		// Token: 0x06001262 RID: 4706 RVA: 0x00086B88 File Offset: 0x00084D88
		private void CancelPreferredTargetShipForAttachmentMachines()
		{
			foreach (ShipAttachmentMachine shipAttachmentMachine in this._formationShipAttachmentMachines)
			{
				shipAttachmentMachine.SetPreferredTargetShip(null);
			}
		}

		// Token: 0x06001263 RID: 4707 RVA: 0x00086BDC File Offset: 0x00084DDC
		public override void OnLostAIControl()
		{
			base.OnLostAIControl();
			this.CancelPreferredTargetShipForAttachmentMachines();
		}

		// Token: 0x06001264 RID: 4708 RVA: 0x00086BEA File Offset: 0x00084DEA
		public override void OnBehaviorCanceled()
		{
			base.OnBehaviorCanceled();
			this.CancelPreferredTargetShipForAttachmentMachines();
		}

		// Token: 0x06001265 RID: 4709 RVA: 0x00086BF8 File Offset: 0x00084DF8
		public override void TickOccasionally()
		{
			if (this._navalShipsLogic == null)
			{
				this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
				if (this._navalShipsLogic == null)
				{
					return;
				}
			}
			if (this._formationMainShip.Formation != base.Formation)
			{
				this._navalShipsLogic.GetShip(base.Formation.Team.TeamSide, base.Formation.FormationIndex, out this._formationMainShip);
			}
			this.CheckAndSwitchState();
			this.CalculateAndSetShipOrders();
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.SetFacingOrder(this.CurrentFacingOrder);
		}

		// Token: 0x06001266 RID: 4710 RVA: 0x00086C9C File Offset: 0x00084E9C
		protected override float GetAiWeight()
		{
			float num = 1f;
			if (base.Formation.CachedClosestEnemyFormation != null)
			{
				if (base.Formation.QuerySystem.FormationMeleeFightingPower > 0f)
				{
					float num2 = base.Formation.CachedClosestEnemyFormation.FormationMeleeFightingPower / base.Formation.QuerySystem.FormationMeleeFightingPower;
					num *= ((num2 >= 1f) ? num2 : 1f);
				}
				else
				{
					num = 2f;
				}
			}
			float num3 = 1f / base.Formation.Team.QuerySystem.TotalPowerRatio;
			num *= ((num3 >= 1f) ? num3 : 1f);
			return ((this._currentState == BehaviorNavalDefendInLine.ShipDefenseState.HelpingFinishedStuckBoarded) ? 0f : 1f) * num * 2f * ((this._currentState != BehaviorNavalDefendInLine.ShipDefenseState.HelpingFinishedStuckBoarded && this._currentState != BehaviorNavalDefendInLine.ShipDefenseState.StandInLine) ? 5f : 1f);
		}

		// Token: 0x04000A51 RID: 2641
		private const float DistanceToKeepWithAllyShip = 30f;

		// Token: 0x04000A52 RID: 2642
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x04000A53 RID: 2643
		private MissionShip _formationMainShip;

		// Token: 0x04000A54 RID: 2644
		private MBReadOnlyList<ShipAttachmentMachine> _formationShipAttachmentMachines;

		// Token: 0x04000A55 RID: 2645
		private TeamAINavalComponent _navalTeamAI;

		// Token: 0x04000A56 RID: 2646
		private BehaviorNavalDefendInLine.ShipDefenseState _currentState;

		// Token: 0x04000A57 RID: 2647
		private MissionShip _leftAllyShip;

		// Token: 0x04000A58 RID: 2648
		private MissionShip _rightAllyShip;

		// Token: 0x04000A59 RID: 2649
		private MissionShip _helpedAllyShip;

		// Token: 0x04000A5A RID: 2650
		private int _navalLineOrder;

		// Token: 0x04000A5B RID: 2651
		private bool _swapSide;

		// Token: 0x04000A5C RID: 2652
		private bool _actualRightSide;

		// Token: 0x04000A5D RID: 2653
		private MissionShip _allyShip;

		// Token: 0x04000A5E RID: 2654
		private bool _tacticallyOnRightSide;

		// Token: 0x04000A5F RID: 2655
		private bool _isAnchor;

		// Token: 0x04000A60 RID: 2656
		private NavalBehaviorBoardShipSubtask _boardShipSubtask;

		// Token: 0x0200026D RID: 621
		private enum ShipDefenseState
		{
			// Token: 0x04001099 RID: 4249
			StandInLine,
			// Token: 0x0400109A RID: 4250
			BeingBoarded,
			// Token: 0x0400109B RID: 4251
			GoingToHelp,
			// Token: 0x0400109C RID: 4252
			HelpingFriend,
			// Token: 0x0400109D RID: 4253
			HelpingFinishedStuckBoarded
		}
	}
}
