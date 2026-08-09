using System;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.AI.Behaviors
{
	// Token: 0x020000F6 RID: 246
	public sealed class BehaviorNavalRamming : NavalBehaviorComponent
	{
		// Token: 0x06001278 RID: 4728 RVA: 0x00087CEC File Offset: 0x00085EEC
		public BehaviorNavalRamming(Formation formation)
			: base(formation)
		{
			base.BehaviorCoherence = 0.8f;
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this._formationMainShip = this._navalShipsLogic.GetShipAssignment(base.Formation.Team.TeamSide, base.Formation.FormationIndex).MissionShip;
		}

		// Token: 0x06001279 RID: 4729 RVA: 0x00087D4C File Offset: 0x00085F4C
		private void CalculateAndSetShipOrders()
		{
			if (base.Formation.CachedClosestEnemyFormation != null && this._formationMainShip.IsFormationAndShipAIControlled)
			{
				MissionShip missionShip = this._navalShipsLogic.GetShipAssignment(base.Formation.CachedClosestEnemyFormation.Team.Team.TeamSide, base.Formation.CachedClosestEnemyFormation.Formation.FormationIndex).MissionShip;
				Vec3 origin = missionShip.GlobalFrame.origin;
				ShipOrder shipOrder = this._formationMainShip.ShipOrder;
				Vec2 asVec = (origin + (origin - this._formationMainShip.GlobalFrame.origin) * 2f).AsVec2;
				shipOrder.SetShipMovementOrder(in asVec);
				if (this._ignoredShip != missionShip)
				{
					if (this._ignoredShip != null)
					{
						this._formationMainShip.AIController.RemoveShipFromCollisionIgnoreListOnAccountOfRamming(this._ignoredShip);
					}
					this._formationMainShip.AIController.AddShipToCollisionIgnoreListOnAccountOfRamming(missionShip);
					this._ignoredShip = missionShip;
				}
			}
		}

		// Token: 0x0600127A RID: 4730 RVA: 0x00087E48 File Offset: 0x00086048
		public override void OnDeploymentFinished()
		{
			base.OnDeploymentFinished();
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this._formationMainShip = this._navalShipsLogic.GetShipAssignment(base.Formation.Team.TeamSide, base.Formation.FormationIndex).MissionShip;
		}

		// Token: 0x0600127B RID: 4731 RVA: 0x00087E9C File Offset: 0x0008609C
		public override void RefreshShipReferences()
		{
			this._formationMainShip = this._navalShipsLogic.GetShipAssignment(base.Formation.Team.TeamSide, base.Formation.FormationIndex).MissionShip;
		}

		// Token: 0x0600127C RID: 4732 RVA: 0x00087ECF File Offset: 0x000860CF
		public override void ResetBehavior()
		{
			base.ResetBehavior();
			this._formationMainShip = this._navalShipsLogic.GetShipAssignment(base.Formation.Team.TeamSide, base.Formation.FormationIndex).MissionShip;
		}

		// Token: 0x0600127D RID: 4733 RVA: 0x00087F08 File Offset: 0x00086108
		protected override void OnBehaviorActivatedAux()
		{
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this._formationMainShip = this._navalShipsLogic.GetShipAssignment(base.Formation.Team.TeamSide, base.Formation.FormationIndex).MissionShip;
			this._formationMainShip.ShipOrder.SetBoardingTargetShip(null);
			this._formationMainShip.ShipOrder.SetCutLoose(false);
			this._formationMainShip.ShipOrder.SetOrderOarsmenLevel(2);
			base.Formation.SetMovementOrder(base.CurrentOrder);
			base.Formation.SetFacingOrder(this.CurrentFacingOrder);
			base.Formation.SetArrangementOrder(ArrangementOrder.ArrangementOrderLine);
			base.Formation.SetFiringOrder(FiringOrder.FiringOrderFireAtWill);
			base.Formation.SetFormOrder(FormOrder.FormOrderWide, true);
			this._isRammingActive = true;
			this._ignoredShip = null;
		}

		// Token: 0x0600127E RID: 4734 RVA: 0x00087FEA File Offset: 0x000861EA
		public override void OnBehaviorCanceled()
		{
			this._isRammingActive = false;
			if (this._ignoredShip != null)
			{
				this._formationMainShip.AIController.RemoveShipFromCollisionIgnoreListOnAccountOfRamming(this._ignoredShip);
			}
		}

		// Token: 0x0600127F RID: 4735 RVA: 0x00088011 File Offset: 0x00086211
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
			this.CalculateAndSetShipOrders();
		}

		// Token: 0x06001280 RID: 4736 RVA: 0x0008803C File Offset: 0x0008623C
		protected override float GetAiWeight()
		{
			if (this._formationMainShip.Formation != base.Formation)
			{
				this._navalShipsLogic.GetShip(base.Formation.Team.TeamSide, base.Formation.FormationIndex, out this._formationMainShip);
			}
			if (base.Formation.CachedClosestEnemyFormation != null)
			{
				MatrixFrame globalFrame = this._navalShipsLogic.GetShipAssignment(base.Formation.CachedClosestEnemyFormation.Team.Team.TeamSide, base.Formation.CachedClosestEnemyFormation.Formation.FormationIndex).MissionShip.GlobalFrame;
				Vec3 vec = globalFrame.origin - this._formationMainShip.GlobalFrame.origin;
				float num = vec.AsVec2.Normalized().DotProduct(this._formationMainShip.Physics.LinearVelocity.AsVec2.Normalized());
				if (num > 0.9f * (this._isRammingActive ? 0.5f : 1f))
				{
					float num2 = num * 1.5f;
					num = Math.Abs(this._formationMainShip.Physics.LinearVelocity.AsVec2.Normalized().DotProduct(globalFrame.rotation.f.AsVec2.Normalized()));
					if (num <= 0.1f * (this._isRammingActive ? 2f : 1f))
					{
						float length = this._formationMainShip.Physics.LinearVelocity.Length;
						if (length > 3f * (this._isRammingActive ? 0.5f : 1f))
						{
							float num3 = vec.AsVec2.Length / length;
							if (num3 < 30f * (this._isRammingActive ? 2f : 1f))
							{
								if (num3 <= 10f)
								{
									num3 = 10f;
								}
								float num4 = 1.5f - num;
								float num5 = length / 3f;
								float num6 = 30f / num3;
								return num2 * num4 * num5 * num6;
							}
						}
					}
				}
			}
			return 0f;
		}

		// Token: 0x04000A70 RID: 2672
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x04000A71 RID: 2673
		private MissionShip _formationMainShip;

		// Token: 0x04000A72 RID: 2674
		private MissionShip _ignoredShip;

		// Token: 0x04000A73 RID: 2675
		private bool _isRammingActive;
	}
}
