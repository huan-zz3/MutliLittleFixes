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
	// Token: 0x020000F3 RID: 243
	public sealed class BehaviorNavalApproachInLine : NavalBehaviorComponent
	{
		// Token: 0x0600124C RID: 4684 RVA: 0x00085454 File Offset: 0x00083654
		public BehaviorNavalApproachInLine(Formation formation)
			: base(formation)
		{
			base.BehaviorCoherence = 0.8f;
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this._formationMainShip = this._navalShipsLogic.GetShipAssignment(base.Formation.Team.TeamSide, base.Formation.FormationIndex).MissionShip;
			List<WeakGameEntity> list = new List<WeakGameEntity>();
			this._formationShipAttachmentMachines = Extensions.ToMBList<ShipAttachmentMachine>(from ce in list
				where ce.HasScriptOfType<ShipAttachmentMachine>()
				select ce.GetFirstScriptOfType<ShipAttachmentMachine>());
			this._formationShipAttachmentPointMachines = Extensions.ToMBList<ShipAttachmentPointMachine>(from ce in list
				where ce.HasScriptOfType<ShipAttachmentPointMachine>()
				select ce.GetFirstScriptOfType<ShipAttachmentPointMachine>());
			this._navalTeamAI = base.Formation.Team.TeamAI as TeamAINavalComponent;
			this.CalculateCurrentOrder();
			this._boardShipSubtask = new NavalBehaviorBoardShipSubtask(this._formationMainShip);
		}

		// Token: 0x0600124D RID: 4685 RVA: 0x00085594 File Offset: 0x00083794
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

		// Token: 0x0600124E RID: 4686 RVA: 0x0008564C File Offset: 0x0008384C
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

		// Token: 0x0600124F RID: 4687 RVA: 0x00085768 File Offset: 0x00083968
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
			if (this._currentState == BehaviorNavalApproachInLine.ShipDefenseState.BeingBoarded)
			{
				base.CurrentOrder = this._formationMainShip.GetMovementOrderToRallyPoint();
				this.CurrentFacingOrder = this._formationMainShip.GetFacingOrderToRallyPoint();
				return;
			}
			base.CurrentOrder = MovementOrder.MovementOrderCharge;
			this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
		}

		// Token: 0x06001250 RID: 4688 RVA: 0x00085808 File Offset: 0x00083A08
		private void CalculateAndSetShipOrders()
		{
			MatrixFrame matrixFrame = this._formationMainShip.GlobalFrame;
			Vec2 vec = matrixFrame.origin.AsVec2;
			matrixFrame = this._formationMainShip.GlobalFrame;
			Vec2 vec2 = matrixFrame.rotation.f.AsVec2;
			MissionShip missionShip = null;
			BehaviorNavalApproachInLine.ShipDefenseState currentState = this._currentState;
			if (currentState != BehaviorNavalApproachInLine.ShipDefenseState.StandInLine)
			{
				if (currentState - BehaviorNavalApproachInLine.ShipDefenseState.GoingToHelp <= 1)
				{
					this._boardShipSubtask.CalculateShipOrders(out vec, out vec2, out missionShip);
				}
			}
			else
			{
				Vec2 vec3 = (this._navalTeamAI.TeamNavalQuerySystem.AverageEnemyShipPosition - this._navalTeamAI.TeamNavalQuerySystem.AverageShipPosition).Normalized();
				if (this._isAnchor)
				{
					Vec2 vec4 = this._navalTeamAI.TeamNavalQuerySystem.AverageShipPosition * (float)this._navalTeamAI.TeamNavalQuerySystem.TeamShipsWithFormationsInLeftToRightOrder.Count;
					vec4 -= this._formationMainShip.GameEntity.GlobalPosition.AsVec2;
					vec4 /= (float)(this._navalTeamAI.TeamNavalQuerySystem.TeamShipsWithFormationsInLeftToRightOrder.Count - 1);
					Vec2 vec5 = this._formationMainShip.GameEntity.GlobalPosition.AsVec2 - vec4;
					float num = vec3.DotProduct(vec5);
					bool flag = false;
					if (this._navalTeamAI.UseSpawnPathApproachPosition)
					{
						this._navalTeamAI.GetRiverApproachPosition(out vec, out vec2);
						matrixFrame = this._formationMainShip.GlobalFrame;
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
						if (this._hasPulledAhead)
						{
							if (num <= 10f)
							{
								this._hasPulledAhead = false;
							}
						}
						else if (num >= 20f)
						{
							this._hasPulledAhead = true;
						}
						vec2 = vec3;
						if (this._hasPulledAhead)
						{
							matrixFrame = this._formationMainShip.GlobalFrame;
							vec = matrixFrame.origin.AsVec2 + vec2 * 15f;
						}
						else
						{
							matrixFrame = this._formationMainShip.GlobalFrame;
							vec = matrixFrame.origin.AsVec2 + vec2 * 450f;
						}
					}
				}
				else
				{
					(this._formationMainShip.GlobalFrame.origin - this._allyShip.GlobalFrame.origin).Normalize();
					Vec2 vec6;
					if (this._actualRightSide)
					{
						if (!this._navalTeamAI.UseSpawnPathApproachPosition)
						{
							vec6 = vec3.RightVec();
						}
						else
						{
							matrixFrame = this._allyShip.GlobalFrame;
							vec6 = matrixFrame.rotation.f.AsVec2.RightVec();
						}
					}
					else if (!this._navalTeamAI.UseSpawnPathApproachPosition)
					{
						vec6 = vec3.LeftVec();
					}
					else
					{
						matrixFrame = this._allyShip.GlobalFrame;
						vec6 = matrixFrame.rotation.f.AsVec2.LeftVec();
					}
					matrixFrame = this._allyShip.GlobalFrame;
					vec = matrixFrame.origin.AsVec2 + vec3 * 30f + vec6 * 30f;
					Vec2 vec7 = vec;
					matrixFrame = this._formationMainShip.GlobalFrame;
					float num2 = (vec7 - matrixFrame.origin.AsVec2).DotProduct(vec3);
					if (num2 < 0f)
					{
						vec += num2 * vec3;
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

		// Token: 0x06001251 RID: 4689 RVA: 0x00085C04 File Offset: 0x00083E04
		private void CheckAndSwitchState()
		{
			if (base.Formation.CachedClosestEnemyFormation != null)
			{
				switch (this._currentState)
				{
				case BehaviorNavalApproachInLine.ShipDefenseState.StandInLine:
					if (this._formationMainShip.SearchShipConnection(null, true, true, true, true))
					{
						this._currentState = BehaviorNavalApproachInLine.ShipDefenseState.BeingBoarded;
						return;
					}
					if (this._leftAllyShip != null && this._leftAllyShip.SearchShipConnection(null, true, true, true, true))
					{
						this._currentState = BehaviorNavalApproachInLine.ShipDefenseState.GoingToHelp;
						this._helpedAllyShip = this._leftAllyShip;
						this._boardShipSubtask.SetTargetShipAndSide(this._helpedAllyShip, this._tacticallyOnRightSide);
						return;
					}
					if (this._rightAllyShip != null && this._rightAllyShip.SearchShipConnection(null, true, true, true, true))
					{
						this._currentState = BehaviorNavalApproachInLine.ShipDefenseState.GoingToHelp;
						this._helpedAllyShip = this._rightAllyShip;
						this._boardShipSubtask.SetTargetShipAndSide(this._helpedAllyShip, this._tacticallyOnRightSide);
						return;
					}
					if (this._formationMainShip.GetIsConnected())
					{
						this._currentState = BehaviorNavalApproachInLine.ShipDefenseState.HelpingFinishedStuckBoarded;
						return;
					}
					break;
				case BehaviorNavalApproachInLine.ShipDefenseState.BeingBoarded:
					if (!this._formationMainShip.GetIsConnected())
					{
						this._currentState = BehaviorNavalApproachInLine.ShipDefenseState.StandInLine;
						return;
					}
					if (!this._formationMainShip.SearchShipConnection(null, true, true, true, true))
					{
						this._currentState = BehaviorNavalApproachInLine.ShipDefenseState.HelpingFinishedStuckBoarded;
						return;
					}
					break;
				case BehaviorNavalApproachInLine.ShipDefenseState.GoingToHelp:
					if (this._formationMainShip.SearchShipConnection(this._helpedAllyShip, true, false, false, true))
					{
						this._currentState = BehaviorNavalApproachInLine.ShipDefenseState.HelpingFriend;
						return;
					}
					if (this._helpedAllyShip == null || !this._helpedAllyShip.SearchShipConnection(null, true, true, true, true))
					{
						this._currentState = BehaviorNavalApproachInLine.ShipDefenseState.StandInLine;
						this._helpedAllyShip = null;
						return;
					}
					if (this._formationMainShip.SearchShipConnection(null, true, true, true, true))
					{
						this._currentState = BehaviorNavalApproachInLine.ShipDefenseState.BeingBoarded;
						this._helpedAllyShip = null;
						return;
					}
					if (this._formationMainShip.GetIsConnected())
					{
						this._currentState = BehaviorNavalApproachInLine.ShipDefenseState.HelpingFinishedStuckBoarded;
						this._helpedAllyShip = null;
						return;
					}
					break;
				case BehaviorNavalApproachInLine.ShipDefenseState.HelpingFriend:
					if (!this._formationMainShip.SearchShipConnection(this._helpedAllyShip, true, false, false, true))
					{
						this._currentState = BehaviorNavalApproachInLine.ShipDefenseState.GoingToHelp;
						this._boardShipSubtask.SetTargetShipAndSide(this._helpedAllyShip, this._tacticallyOnRightSide);
					}
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x06001252 RID: 4690 RVA: 0x00085DE8 File Offset: 0x00083FE8
		public override void OnDeploymentFinished()
		{
			base.OnDeploymentFinished();
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this._formationMainShip = this._navalShipsLogic.GetShipAssignment(base.Formation.Team.TeamSide, base.Formation.FormationIndex).MissionShip;
			this._currentState = BehaviorNavalApproachInLine.ShipDefenseState.StandInLine;
		}

		// Token: 0x06001253 RID: 4691 RVA: 0x00085E44 File Offset: 0x00084044
		protected override void OnBehaviorActivatedAux()
		{
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this._formationMainShip = this._navalShipsLogic.GetShipAssignment(base.Formation.Team.TeamSide, base.Formation.FormationIndex).MissionShip;
			this._boardShipSubtask.SetOwnerShip(this._formationMainShip);
			this._boardShipSubtask.SetTargetShipAndSide(null, this._tacticallyOnRightSide);
			this._currentState = BehaviorNavalApproachInLine.ShipDefenseState.StandInLine;
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

		// Token: 0x06001254 RID: 4692 RVA: 0x00085F48 File Offset: 0x00084148
		private void CancelPreferredTargetShipForAttachmentMachines()
		{
			foreach (ShipAttachmentMachine shipAttachmentMachine in this._formationShipAttachmentMachines)
			{
				shipAttachmentMachine.SetPreferredTargetShip(null);
			}
		}

		// Token: 0x06001255 RID: 4693 RVA: 0x00085F9C File Offset: 0x0008419C
		public override void OnLostAIControl()
		{
			base.OnLostAIControl();
			this.CancelPreferredTargetShipForAttachmentMachines();
		}

		// Token: 0x06001256 RID: 4694 RVA: 0x00085FAA File Offset: 0x000841AA
		public override void OnBehaviorCanceled()
		{
			base.OnBehaviorCanceled();
			this.CancelPreferredTargetShipForAttachmentMachines();
		}

		// Token: 0x06001257 RID: 4695 RVA: 0x00085FB8 File Offset: 0x000841B8
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
			MissionShip missionShip;
			if (this._formationMainShip.Formation != base.Formation && this._navalShipsLogic.GetShip(base.Formation.Team.TeamSide, base.Formation.FormationIndex, out missionShip))
			{
				this._formationMainShip = missionShip;
			}
			this.CheckAndSwitchState();
			this.CalculateAndSetShipOrders();
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.SetFacingOrder(this.CurrentFacingOrder);
		}

		// Token: 0x06001258 RID: 4696 RVA: 0x00086060 File Offset: 0x00084260
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
			return ((this._currentState == BehaviorNavalApproachInLine.ShipDefenseState.HelpingFinishedStuckBoarded) ? 0f : 1f) * num * 2f * ((this._currentState != BehaviorNavalApproachInLine.ShipDefenseState.HelpingFinishedStuckBoarded && this._currentState != BehaviorNavalApproachInLine.ShipDefenseState.StandInLine) ? 5f : 1f);
		}

		// Token: 0x04000A40 RID: 2624
		private const float DistanceToKeepWithAllyShip = 30f;

		// Token: 0x04000A41 RID: 2625
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x04000A42 RID: 2626
		private MissionShip _formationMainShip;

		// Token: 0x04000A43 RID: 2627
		private MBReadOnlyList<ShipAttachmentMachine> _formationShipAttachmentMachines;

		// Token: 0x04000A44 RID: 2628
		private MBReadOnlyList<ShipAttachmentPointMachine> _formationShipAttachmentPointMachines;

		// Token: 0x04000A45 RID: 2629
		private TeamAINavalComponent _navalTeamAI;

		// Token: 0x04000A46 RID: 2630
		private BehaviorNavalApproachInLine.ShipDefenseState _currentState;

		// Token: 0x04000A47 RID: 2631
		private MissionShip _leftAllyShip;

		// Token: 0x04000A48 RID: 2632
		private MissionShip _rightAllyShip;

		// Token: 0x04000A49 RID: 2633
		private MissionShip _helpedAllyShip;

		// Token: 0x04000A4A RID: 2634
		private int _navalLineOrder;

		// Token: 0x04000A4B RID: 2635
		private bool _actualRightSide;

		// Token: 0x04000A4C RID: 2636
		private MissionShip _allyShip;

		// Token: 0x04000A4D RID: 2637
		private bool _tacticallyOnRightSide;

		// Token: 0x04000A4E RID: 2638
		private bool _isAnchor;

		// Token: 0x04000A4F RID: 2639
		private bool _hasPulledAhead;

		// Token: 0x04000A50 RID: 2640
		private NavalBehaviorBoardShipSubtask _boardShipSubtask;

		// Token: 0x0200026B RID: 619
		private enum ShipDefenseState
		{
			// Token: 0x0400108E RID: 4238
			StandInLine,
			// Token: 0x0400108F RID: 4239
			BeingBoarded,
			// Token: 0x04001090 RID: 4240
			GoingToHelp,
			// Token: 0x04001091 RID: 4241
			HelpingFriend,
			// Token: 0x04001092 RID: 4242
			HelpingFinishedStuckBoarded
		}
	}
}
