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
	// Token: 0x020000F5 RID: 245
	public sealed class BehaviorNavalEngageCorrespondingEnemy : NavalBehaviorComponent
	{
		// Token: 0x06001267 RID: 4711 RVA: 0x00086D7C File Offset: 0x00084F7C
		public BehaviorNavalEngageCorrespondingEnemy(Formation formation)
			: base(formation)
		{
			base.BehaviorCoherence = 0.8f;
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this._navalShipsLogic.GetShip(base.Formation.Team.TeamSide, base.Formation.FormationIndex, out this._formationMainShip);
			List<WeakGameEntity> list = new List<WeakGameEntity>();
			this._formationMainShip.GameEntity.GetChildrenRecursive(ref list);
			this._formationShipAttachmentMachines = Extensions.ToMBList<ShipAttachmentMachine>(from ce in list
				where ce.HasScriptOfType<ShipAttachmentMachine>()
				select ce.GetFirstScriptOfType<ShipAttachmentMachine>());
			this._formationShipAttachmentPointMachines = Extensions.ToMBList<ShipAttachmentPointMachine>(from ce in list
				where ce.HasScriptOfType<ShipAttachmentPointMachine>()
				select ce.GetFirstScriptOfType<ShipAttachmentPointMachine>());
			this._navalTeamAI = base.Formation.Team.TeamAI as TeamAINavalComponent;
			this._currentState = BehaviorNavalEngageCorrespondingEnemy.ShipBoardingState.ApproachFromFarAway;
			this.CalculateCurrentOrder();
			this._boardShipSubtask = new NavalBehaviorBoardShipSubtask(this._formationMainShip);
		}

		// Token: 0x06001268 RID: 4712 RVA: 0x00086EDC File Offset: 0x000850DC
		public override void RefreshShipReferences()
		{
			this._formationMainShip = this._navalShipsLogic.GetShipAssignment(base.Formation.Team.TeamSide, base.Formation.FormationIndex).MissionShip;
			this.SetTargetShipSideAndOrder(this._tacticallyOnRightSide, this._navalLineOrder);
		}

		// Token: 0x06001269 RID: 4713 RVA: 0x00086F2C File Offset: 0x0008512C
		public void SetTargetShipSideAndOrder(bool rightSide, int navalLineOrder)
		{
			this._tacticallyOnRightSide = rightSide;
			this._actualRightSide = rightSide;
			this._navalLineOrder = navalLineOrder;
			this._targetShip = this.FindCorrespondingEnemyShip();
			this._boardShipSubtask.SetTargetShipAndSide(this._targetShip, this._tacticallyOnRightSide);
		}

		// Token: 0x0600126A RID: 4714 RVA: 0x00086F68 File Offset: 0x00085168
		private MissionShip FindCorrespondingEnemyShip()
		{
			if (this._formationMainShip == null || this._navalTeamAI.TeamNavalQuerySystem.EnemyShipsWithFormationsInLeftToRightOrder.Count <= 0)
			{
				return null;
			}
			MissionShip missionShip;
			if (this._formationMainShip.GetIsConnectedToEnemy(out missionShip))
			{
				return missionShip;
			}
			float num = ((float)this._navalTeamAI.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.Count - 1f) * 0.5f;
			float num2 = ((float)this._navalTeamAI.TeamNavalQuerySystem.EnemyShipsWithFormationsInLeftToRightOrder.Count - 1f) * 0.5f;
			bool flag = num > num2;
			if ((int)num == this._navalLineOrder && (float)((int)num) + 0.1f > num)
			{
				if (num2 >= (float)((int)num2) + 0.1f)
				{
					this._actualRightSide = flag;
					num2 += (this._actualRightSide ? 0.5f : (-0.5f));
				}
				if (num2 < 0f)
				{
					num2 = 0f;
					this._actualRightSide = true;
				}
				else if (num2 >= (float)this._navalTeamAI.TeamNavalQuerySystem.EnemyShipsWithFormationsInLeftToRightOrder.Count)
				{
					num2 = (float)(this._navalTeamAI.TeamNavalQuerySystem.EnemyShipsWithFormationsInLeftToRightOrder.Count - 1);
					this._actualRightSide = false;
				}
				return this._navalTeamAI.TeamNavalQuerySystem.EnemyShipsWithFormationsInLeftToRightOrder[(int)num2];
			}
			int num3;
			int num4;
			float num5;
			float num6;
			if ((float)((int)num) + 0.1f > num)
			{
				num3 = (int)(num - 1f);
				num4 = (int)(num + 1f);
				num5 = num2 + 1f;
				num6 = num2 - 1f;
			}
			else
			{
				num3 = (int)(num - 0.5f);
				num4 = (int)(num + 0.5f);
				num5 = num2 + 0.5f;
				num6 = num2 - 0.5f;
			}
			while (num3 >= 0 || num4 < this._navalTeamAI.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.Count)
			{
				if (num3 == this._navalLineOrder)
				{
					if (num5 >= (float)((int)num5) + 0.1f)
					{
						num5 += (flag ? (-0.5f) : 0.5f);
						this._actualRightSide = !flag;
					}
					else
					{
						this._actualRightSide = flag;
					}
					if ((int)num5 >= this._navalTeamAI.TeamNavalQuerySystem.EnemyShipsWithFormationsInLeftToRightOrder.Count)
					{
						this._actualRightSide = false;
						num5 = (float)(this._navalTeamAI.TeamNavalQuerySystem.EnemyShipsWithFormationsInLeftToRightOrder.Count - 1);
					}
					return this._navalTeamAI.TeamNavalQuerySystem.EnemyShipsWithFormationsInLeftToRightOrder[(int)num5];
				}
				if (num4 == this._navalLineOrder)
				{
					if (num6 >= (float)((int)num6) + 0.1f)
					{
						num6 += (flag ? 0.5f : (-0.5f));
						this._actualRightSide = flag;
					}
					else
					{
						this._actualRightSide = !flag;
					}
					if (num6 < 0f)
					{
						this._actualRightSide = true;
						num6 = 0f;
					}
					return this._navalTeamAI.TeamNavalQuerySystem.EnemyShipsWithFormationsInLeftToRightOrder[(int)num6];
				}
				num3--;
				num4++;
				num5 += 1f;
				num6 -= 1f;
			}
			return null;
		}

		// Token: 0x0600126B RID: 4715 RVA: 0x00087250 File Offset: 0x00085450
		private void RefreshTargetShip()
		{
			MissionShip missionShip2;
			MissionShip missionShip = (missionShip2 = this._boardShipSubtask.GetCurrentEffectiveTargetShip());
			FormationQuerySystem cachedClosestEnemyFormation = base.Formation.CachedClosestEnemyFormation;
			Formation formation = ((cachedClosestEnemyFormation != null) ? cachedClosestEnemyFormation.Formation : null);
			MissionShip missionShip3 = null;
			if (formation != null)
			{
				this._navalShipsLogic.GetShip(formation.Team.TeamSide, formation.FormationIndex, out missionShip3);
			}
			if (missionShip3 != null)
			{
				float num = missionShip3.GameEntity.GlobalPosition.DistanceSquared(this._formationMainShip.GameEntity.GlobalPosition);
				if (num <= 3600f)
				{
					double num2 = Math.Sqrt((double)num);
					if (this._targetShip == null || ((double)this._targetShip.GameEntity.GlobalPosition.Distance(this._formationMainShip.GameEntity.GlobalPosition) - num2 > 30.0 && (double)this._boardShipSubtask.GetEffectiveDistanceToObjective() - num2 > 30.0))
					{
						missionShip2 = missionShip3;
					}
				}
			}
			if (missionShip != missionShip2 && this._targetShip != missionShip2)
			{
				MissionShip targetShip = this._targetShip;
				if ((targetShip != null && !targetShip.AnyActiveFormationTroopOnShip) || missionShip2 == null)
				{
					this._targetShip = missionShip2;
					return;
				}
				if (this._boardShipSubtask.GetEffectiveDistanceToObjective() > 60f || this._boardShipSubtask.GetEffectiveDistanceToObjective() > this._formationMainShip.GameEntity.GlobalPosition.Distance(missionShip2.GameEntity.GlobalPosition) * 1.2f)
				{
					this._targetShip = missionShip2;
					this._boardShipSubtask.SetTargetShipAndSide(this._targetShip, this._tacticallyOnRightSide);
				}
			}
		}

		// Token: 0x0600126C RID: 4716 RVA: 0x000873F4 File Offset: 0x000855F4
		protected override void CalculateCurrentOrder()
		{
			if (this._navalShipsLogic == null || base.Formation.CachedClosestEnemyFormation == null || this._targetShip == null)
			{
				base.CurrentOrder = MovementOrder.MovementOrderStop;
				return;
			}
			if (this._formationMainShip != null && (this._formationMainShip.SearchShipConnection(null, true, true, false, false) || this._currentState == BehaviorNavalEngageCorrespondingEnemy.ShipBoardingState.Connected))
			{
				base.CurrentOrder = MovementOrder.MovementOrderCharge;
				return;
			}
			base.CurrentOrder = MovementOrder.MovementOrderStop;
		}

		// Token: 0x0600126D RID: 4717 RVA: 0x00087464 File Offset: 0x00085664
		private void CalculateAndSetShipOrders()
		{
			if (base.Formation.CachedClosestEnemyFormation != null && this._targetShip != null && this._formationMainShip != null && this._formationMainShip.IsFormationAndShipAIControlled)
			{
				Vec2 vec;
				Vec2 vec2;
				MissionShip missionShip;
				this._boardShipSubtask.CalculateShipOrders(out vec, out vec2, out missionShip);
				this._formationMainShip.ShipOrder.SetShipMovementOrder(vec, in vec2);
				this._formationMainShip.ShipOrder.SetBoardingTargetShip(missionShip);
			}
		}

		// Token: 0x0600126E RID: 4718 RVA: 0x000874D0 File Offset: 0x000856D0
		private void CheckAndRefreshTargetIfNecessary()
		{
			if (this._targetShip == null || !this._targetShip.AnyActiveFormationTroopOnShip)
			{
				this._targetShip = this.FindCorrespondingEnemyShip();
				this._boardShipSubtask.SetTargetShipAndSide(this._targetShip, this._tacticallyOnRightSide);
				return;
			}
			NavalBehaviorBoardShipSubtask.ShipBoardingState state = this._boardShipSubtask.State;
			if (state <= NavalBehaviorBoardShipSubtask.ShipBoardingState.GettingClose)
			{
				this.RefreshTargetShip();
				return;
			}
			if (state != NavalBehaviorBoardShipSubtask.ShipBoardingState.InactiveStuck)
			{
				return;
			}
			MissionShip missionShip = this.FindCorrespondingEnemyShip();
			if (missionShip != this._boardShipSubtask.GetCurrentGivenTarget())
			{
				this._targetShip = missionShip;
				this._boardShipSubtask.SetTargetShipAndSide(this._targetShip, this._tacticallyOnRightSide);
			}
		}

		// Token: 0x0600126F RID: 4719 RVA: 0x00087568 File Offset: 0x00085768
		private void CheckAndSwitchState()
		{
			if (base.Formation.CachedClosestEnemyFormation != null && this._targetShip != null && this._targetShip.AnyActiveFormationTroopOnShip && this._formationMainShip != null)
			{
				MatrixFrame globalFrame = this._targetShip.GlobalFrame;
				MatrixFrame globalFrame2 = this._formationMainShip.GlobalFrame;
				Vec2 vec = (this._actualRightSide ? globalFrame.rotation.f.AsVec2.LeftVec().Normalized() : globalFrame.rotation.f.AsVec2.RightVec().Normalized());
				switch (this._currentState)
				{
				case BehaviorNavalEngageCorrespondingEnemy.ShipBoardingState.ApproachFromFarAway:
					if ((globalFrame.origin.AsVec2 - globalFrame2.origin.AsVec2).LengthSquared < 900f || (globalFrame.origin.AsVec2 + vec * 12f - globalFrame2.origin.AsVec2).LengthSquared < 2500f)
					{
						this._currentState = BehaviorNavalEngageCorrespondingEnemy.ShipBoardingState.GettingClose;
						return;
					}
					break;
				case BehaviorNavalEngageCorrespondingEnemy.ShipBoardingState.GettingClose:
					if ((globalFrame.origin.AsVec2 - globalFrame2.origin.AsVec2).LengthSquared < 900f || (globalFrame.origin.AsVec2 + vec * 12f - globalFrame2.origin.AsVec2).LengthSquared < 900f)
					{
						this._currentState = BehaviorNavalEngageCorrespondingEnemy.ShipBoardingState.AdjustingOrientation;
						return;
					}
					break;
				case BehaviorNavalEngageCorrespondingEnemy.ShipBoardingState.AdjustingOrientation:
					if ((globalFrame.origin.AsVec2 + vec * 12f - globalFrame2.origin.AsVec2).LengthSquared > 2500f)
					{
						this._currentState = BehaviorNavalEngageCorrespondingEnemy.ShipBoardingState.GettingClose;
						return;
					}
					if (Math.Abs(globalFrame2.rotation.f.AsVec2.Normalized().DotProduct(globalFrame.rotation.f.AsVec2.Normalized())) > 0.8f)
					{
						this._currentState = BehaviorNavalEngageCorrespondingEnemy.ShipBoardingState.InPosition;
						return;
					}
					break;
				case BehaviorNavalEngageCorrespondingEnemy.ShipBoardingState.InPosition:
				{
					bool flag = false;
					using (List<ShipAttachmentMachine>.Enumerator enumerator = this._formationShipAttachmentMachines.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.CurrentAttachment != null)
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						using (List<ShipAttachmentPointMachine>.Enumerator enumerator2 = this._formationShipAttachmentPointMachines.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								if (enumerator2.Current.CurrentAttachment != null)
								{
									flag = true;
									break;
								}
							}
						}
					}
					if (flag)
					{
						this._currentState = BehaviorNavalEngageCorrespondingEnemy.ShipBoardingState.Connected;
						return;
					}
					if (Math.Abs(globalFrame2.rotation.f.AsVec2.Normalized().DotProduct(globalFrame.rotation.f.AsVec2.Normalized())) < 0.6f)
					{
						this._currentState = BehaviorNavalEngageCorrespondingEnemy.ShipBoardingState.AdjustingOrientation;
						return;
					}
					if ((globalFrame.origin.AsVec2 + vec * 12f - globalFrame2.origin.AsVec2).LengthSquared > 2500f)
					{
						this._currentState = BehaviorNavalEngageCorrespondingEnemy.ShipBoardingState.GettingClose;
						return;
					}
					break;
				}
				case BehaviorNavalEngageCorrespondingEnemy.ShipBoardingState.Connected:
				{
					bool flag2 = false;
					using (List<ShipAttachmentMachine>.Enumerator enumerator = this._formationShipAttachmentMachines.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.CurrentAttachment != null)
							{
								flag2 = true;
								break;
							}
						}
					}
					if (!flag2)
					{
						using (List<ShipAttachmentPointMachine>.Enumerator enumerator2 = this._formationShipAttachmentPointMachines.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								if (enumerator2.Current.CurrentAttachment != null)
								{
									flag2 = true;
									break;
								}
							}
						}
					}
					if (!flag2)
					{
						this._currentState = BehaviorNavalEngageCorrespondingEnemy.ShipBoardingState.GettingClose;
						return;
					}
					break;
				}
				default:
					return;
				}
			}
			else
			{
				this.RefreshTargetShip();
			}
		}

		// Token: 0x06001270 RID: 4720 RVA: 0x00087994 File Offset: 0x00085B94
		public override void OnDeploymentFinished()
		{
			base.OnDeploymentFinished();
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this._navalShipsLogic.GetShip(base.Formation.Team.TeamSide, base.Formation.FormationIndex, out this._formationMainShip);
			this._currentState = BehaviorNavalEngageCorrespondingEnemy.ShipBoardingState.ApproachFromFarAway;
		}

		// Token: 0x06001271 RID: 4721 RVA: 0x000879EB File Offset: 0x00085BEB
		public override void ResetBehavior()
		{
			base.ResetBehavior();
			this._navalShipsLogic.GetShip(base.Formation.Team.TeamSide, base.Formation.FormationIndex, out this._formationMainShip);
			this._currentState = BehaviorNavalEngageCorrespondingEnemy.ShipBoardingState.ApproachFromFarAway;
		}

		// Token: 0x06001272 RID: 4722 RVA: 0x00087A28 File Offset: 0x00085C28
		protected override void OnBehaviorActivatedAux()
		{
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this._navalShipsLogic.GetShip(base.Formation.Team.TeamSide, base.Formation.FormationIndex, out this._formationMainShip);
			this.RefreshTargetShip();
			this._boardShipSubtask.SetOwnerShip(this._formationMainShip);
			this._targetShip = this.FindCorrespondingEnemyShip();
			this._boardShipSubtask.SetTargetShipAndSide(this._targetShip, this._tacticallyOnRightSide);
			this._currentState = BehaviorNavalEngageCorrespondingEnemy.ShipBoardingState.ApproachFromFarAway;
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

		// Token: 0x06001273 RID: 4723 RVA: 0x00087B40 File Offset: 0x00085D40
		private void CancelPreferredTargetShipForAttachmentMachines()
		{
			foreach (ShipAttachmentMachine shipAttachmentMachine in this._formationShipAttachmentMachines)
			{
				shipAttachmentMachine.SetPreferredTargetShip(null);
			}
		}

		// Token: 0x06001274 RID: 4724 RVA: 0x00087B94 File Offset: 0x00085D94
		public override void OnLostAIControl()
		{
			base.OnLostAIControl();
			this.CancelPreferredTargetShipForAttachmentMachines();
		}

		// Token: 0x06001275 RID: 4725 RVA: 0x00087BA2 File Offset: 0x00085DA2
		public override void OnBehaviorCanceled()
		{
			base.OnBehaviorCanceled();
			this.CancelPreferredTargetShipForAttachmentMachines();
		}

		// Token: 0x06001276 RID: 4726 RVA: 0x00087BB0 File Offset: 0x00085DB0
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
			this.CheckAndRefreshTargetIfNecessary();
			this.CalculateAndSetShipOrders();
			this.CalculateCurrentOrder();
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.SetFacingOrder(this.CurrentFacingOrder);
		}

		// Token: 0x06001277 RID: 4727 RVA: 0x00087C54 File Offset: 0x00085E54
		protected override float GetAiWeight()
		{
			float num = 0f;
			if (base.Formation.CachedClosestEnemyFormation != null)
			{
				if (base.Formation.CachedClosestEnemyFormation.FormationMeleeFightingPower > 0f)
				{
					num = base.Formation.QuerySystem.FormationMeleeFightingPower / base.Formation.CachedClosestEnemyFormation.FormationMeleeFightingPower;
				}
				else
				{
					num = 20f;
				}
			}
			return (this._perfectMatch ? 1.5f : 1.25f) * MathF.Clamp(num, 0f, 20f) * base.Formation.QuerySystem.InfantryUnitRatio;
		}

		// Token: 0x04000A61 RID: 2657
		private const float IdealBoardingDistance = 12f;

		// Token: 0x04000A62 RID: 2658
		private const float MaximumBoardingDistance = 30f;

		// Token: 0x04000A63 RID: 2659
		private const float DriftedAwayDistance = 50f;

		// Token: 0x04000A64 RID: 2660
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x04000A65 RID: 2661
		private MissionShip _formationMainShip;

		// Token: 0x04000A66 RID: 2662
		private MBReadOnlyList<ShipAttachmentMachine> _formationShipAttachmentMachines;

		// Token: 0x04000A67 RID: 2663
		private MBReadOnlyList<ShipAttachmentPointMachine> _formationShipAttachmentPointMachines;

		// Token: 0x04000A68 RID: 2664
		private TeamAINavalComponent _navalTeamAI;

		// Token: 0x04000A69 RID: 2665
		private BehaviorNavalEngageCorrespondingEnemy.ShipBoardingState _currentState;

		// Token: 0x04000A6A RID: 2666
		private bool _tacticallyOnRightSide;

		// Token: 0x04000A6B RID: 2667
		private MissionShip _targetShip;

		// Token: 0x04000A6C RID: 2668
		private int _navalLineOrder;

		// Token: 0x04000A6D RID: 2669
		private bool _perfectMatch = true;

		// Token: 0x04000A6E RID: 2670
		private bool _actualRightSide;

		// Token: 0x04000A6F RID: 2671
		private NavalBehaviorBoardShipSubtask _boardShipSubtask;

		// Token: 0x0200026F RID: 623
		private enum ShipBoardingState
		{
			// Token: 0x040010A4 RID: 4260
			ApproachFromFarAway,
			// Token: 0x040010A5 RID: 4261
			GettingClose,
			// Token: 0x040010A6 RID: 4262
			AdjustingOrientation,
			// Token: 0x040010A7 RID: 4263
			InPosition,
			// Token: 0x040010A8 RID: 4264
			Connected
		}
	}
}
