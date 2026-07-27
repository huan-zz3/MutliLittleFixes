using System;
using NavalDLC.Missions.Objects;
using TaleWorlds.Library;

namespace NavalDLC.Missions.AI.Behaviors
{
	// Token: 0x020000F9 RID: 249
	internal class NavalBehaviorBoardShipSubtask
	{
		// Token: 0x1700032D RID: 813
		// (get) Token: 0x06001291 RID: 4753 RVA: 0x0008894C File Offset: 0x00086B4C
		// (set) Token: 0x06001292 RID: 4754 RVA: 0x00088954 File Offset: 0x00086B54
		public NavalBehaviorBoardShipSubtask.ShipBoardingState State { get; private set; }

		// Token: 0x06001293 RID: 4755 RVA: 0x0008895D File Offset: 0x00086B5D
		public NavalBehaviorBoardShipSubtask(MissionShip selfShip)
		{
			this._selfShip = selfShip;
		}

		// Token: 0x06001294 RID: 4756 RVA: 0x00088977 File Offset: 0x00086B77
		public void OnBehaviorActivatedAux()
		{
			this.State = NavalBehaviorBoardShipSubtask.ShipBoardingState.ApproachFromFarAway;
		}

		// Token: 0x06001295 RID: 4757 RVA: 0x00088980 File Offset: 0x00086B80
		public void SetOwnerShip(MissionShip selfShip)
		{
			this._selfShip = selfShip;
			this.SetTargetShipAndSide(this._givenTargetToBoard, this._givenSideToBoardIsRight);
		}

		// Token: 0x06001296 RID: 4758 RVA: 0x0008899C File Offset: 0x00086B9C
		public void SetTargetShipAndSide(MissionShip targetShip, bool rightSide)
		{
			if (this._givenTargetToBoard != targetShip || this._effectiveTarget != targetShip || this._givenSideToBoardIsRight != rightSide || this._effectiveSideToBoardIsRight != rightSide || this.State == NavalBehaviorBoardShipSubtask.ShipBoardingState.InactiveStuck)
			{
				this._givenTargetToBoard = targetShip;
				this._effectiveTarget = targetShip;
				this._givenSideToBoardIsRight = rightSide;
				this._effectiveSideToBoardIsRight = rightSide;
				this.State = NavalBehaviorBoardShipSubtask.ShipBoardingState.ApproachFromFarAway;
			}
		}

		// Token: 0x06001297 RID: 4759 RVA: 0x000889F9 File Offset: 0x00086BF9
		public MissionShip GetCurrentGivenTarget()
		{
			return this._givenTargetToBoard;
		}

		// Token: 0x06001298 RID: 4760 RVA: 0x00088A01 File Offset: 0x00086C01
		public MissionShip GetCurrentEffectiveTargetShip()
		{
			return this._effectiveTarget;
		}

		// Token: 0x06001299 RID: 4761 RVA: 0x00088A09 File Offset: 0x00086C09
		public float GetEffectiveDistanceToObjective()
		{
			if (this.State == NavalBehaviorBoardShipSubtask.ShipBoardingState.Connected)
			{
				return 0f;
			}
			if (this.State == NavalBehaviorBoardShipSubtask.ShipBoardingState.InactiveStuck)
			{
				return float.MaxValue;
			}
			return this._cachedEffectiveDistance;
		}

		// Token: 0x0600129A RID: 4762 RVA: 0x00088A30 File Offset: 0x00086C30
		private void CheckAndSwitchState()
		{
			if (this._givenTargetToBoard != null && this._effectiveTarget != null)
			{
				if (this.State != NavalBehaviorBoardShipSubtask.ShipBoardingState.Connected && this.State != NavalBehaviorBoardShipSubtask.ShipBoardingState.InactiveStuck && this._selfShip.GetIsConnected())
				{
					if (this._selfShip.SearchShipConnection(this._givenTargetToBoard, true, false, false, true))
					{
						this.State = NavalBehaviorBoardShipSubtask.ShipBoardingState.Connected;
						return;
					}
					this.State = NavalBehaviorBoardShipSubtask.ShipBoardingState.InactiveStuck;
					return;
				}
				else
				{
					MatrixFrame globalFrame = this._effectiveTarget.GlobalFrame;
					MatrixFrame globalFrame2 = this._selfShip.GlobalFrame;
					Vec2 vec = (this._effectiveSideToBoardIsRight ? globalFrame.rotation.f.AsVec2.LeftVec().Normalized() : globalFrame.rotation.f.AsVec2.RightVec().Normalized());
					if (this.State == NavalBehaviorBoardShipSubtask.ShipBoardingState.ApproachFromFarAway && ((globalFrame.origin.AsVec2 - globalFrame2.origin.AsVec2).LengthSquared < 900f || (globalFrame.origin.AsVec2 + vec * 12f - globalFrame2.origin.AsVec2).LengthSquared < 2500f))
					{
						this.State = NavalBehaviorBoardShipSubtask.ShipBoardingState.GettingClose;
					}
					if (this.State == NavalBehaviorBoardShipSubtask.ShipBoardingState.GettingClose && ((globalFrame.origin.AsVec2 - globalFrame2.origin.AsVec2).LengthSquared < 900f || (globalFrame.origin.AsVec2 + vec * 12f - globalFrame2.origin.AsVec2).LengthSquared < 900f))
					{
						this.State = NavalBehaviorBoardShipSubtask.ShipBoardingState.AdjustingOrientation;
					}
					if (this.State == NavalBehaviorBoardShipSubtask.ShipBoardingState.AdjustingOrientation)
					{
						if ((globalFrame.origin.AsVec2 + vec * 12f - globalFrame2.origin.AsVec2).LengthSquared > 2500f)
						{
							this.State = NavalBehaviorBoardShipSubtask.ShipBoardingState.GettingClose;
						}
						else if (Math.Abs(globalFrame2.rotation.f.AsVec2.Normalized().DotProduct(globalFrame.rotation.f.AsVec2.Normalized())) > 0.8f)
						{
							this.State = NavalBehaviorBoardShipSubtask.ShipBoardingState.InPosition;
						}
					}
					if (this.State == NavalBehaviorBoardShipSubtask.ShipBoardingState.InPosition)
					{
						if (this._selfShip.GetIsConnected())
						{
							this.State = NavalBehaviorBoardShipSubtask.ShipBoardingState.Connected;
						}
						else if (Math.Abs(globalFrame2.rotation.f.AsVec2.Normalized().DotProduct(globalFrame.rotation.f.AsVec2.Normalized())) < 0.6f)
						{
							this.State = NavalBehaviorBoardShipSubtask.ShipBoardingState.AdjustingOrientation;
						}
						else if ((globalFrame.origin.AsVec2 + vec * 12f - globalFrame2.origin.AsVec2).LengthSquared > 2500f)
						{
							this.State = NavalBehaviorBoardShipSubtask.ShipBoardingState.GettingClose;
						}
					}
					if (this.State == NavalBehaviorBoardShipSubtask.ShipBoardingState.Connected)
					{
						if (!this._selfShip.GetIsConnected())
						{
							this.State = NavalBehaviorBoardShipSubtask.ShipBoardingState.GettingClose;
						}
						else if (!this._selfShip.SearchShipConnection(this._givenTargetToBoard, true, false, false, true))
						{
							this.State = NavalBehaviorBoardShipSubtask.ShipBoardingState.InactiveStuck;
						}
					}
					if (this.State == NavalBehaviorBoardShipSubtask.ShipBoardingState.InactiveStuck && !this._selfShip.GetIsConnected())
					{
						this.State = NavalBehaviorBoardShipSubtask.ShipBoardingState.ApproachFromFarAway;
					}
				}
			}
		}

		// Token: 0x0600129B RID: 4763 RVA: 0x00088DA0 File Offset: 0x00086FA0
		private bool IsEffectivelyRightSide()
		{
			if (this.State == NavalBehaviorBoardShipSubtask.ShipBoardingState.ApproachFromFarAway)
			{
				return this._givenSideToBoardIsRight;
			}
			return (this._selfShip.GameEntity.GlobalPosition.AsVec2 - this._givenTargetToBoard.GameEntity.GlobalPosition.AsVec2).DotProduct(this._givenTargetToBoard.GlobalFrame.rotation.f.AsVec2.LeftVec()) >= 0f;
		}

		// Token: 0x0600129C RID: 4764 RVA: 0x00088E30 File Offset: 0x00087030
		private bool IsRelevantSideOfEnemyShipRight(MissionShip testedShip)
		{
			Vec2 vec;
			MatrixFrame matrixFrame;
			if (this.State != NavalBehaviorBoardShipSubtask.ShipBoardingState.ApproachFromFarAway)
			{
				vec = this._selfShip.GameEntity.GlobalPosition.AsVec2 - testedShip.GameEntity.GlobalPosition.AsVec2;
				matrixFrame = testedShip.GlobalFrame;
				return vec.DotProduct(matrixFrame.rotation.f.AsVec2.RightVec()) >= 0f;
			}
			vec = this._selfShip.GameEntity.GlobalPosition.AsVec2 - testedShip.GameEntity.GlobalPosition.AsVec2;
			matrixFrame = testedShip.GlobalFrame;
			if (vec.DotProduct(matrixFrame.rotation.f.AsVec2) < 0f)
			{
				return this._givenSideToBoardIsRight;
			}
			return !this._givenSideToBoardIsRight;
		}

		// Token: 0x0600129D RID: 4765 RVA: 0x00088F1C File Offset: 0x0008711C
		private void DetermineEffectiveTargetShip()
		{
			this._effectiveSideToBoardIsRight = this.IsRelevantSideOfEnemyShipRight(this._givenTargetToBoard);
			this._effectiveTarget = this._givenTargetToBoard.GetOutermostConnectedShipFromSide(this._effectiveSideToBoardIsRight, out this._effectiveSideToBoardIsRight, 0UL);
		}

		// Token: 0x0600129E RID: 4766 RVA: 0x00088F50 File Offset: 0x00087150
		private void ApproachFromDistance(MissionShip enemyShip, out Vec2 desiredPosition)
		{
			Vec2 vec = (enemyShip.GameEntity.GlobalPosition.AsVec2 - this._selfShip.GameEntity.GlobalPosition.AsVec2).Normalized();
			desiredPosition = enemyShip.GlobalFrame.origin.AsVec2 + (this._effectiveSideToBoardIsRight ? vec.RightVec().Normalized() : vec.LeftVec().Normalized()) * 12f;
		}

		// Token: 0x0600129F RID: 4767 RVA: 0x00088FF0 File Offset: 0x000871F0
		private void GettingCloseCase(MissionShip enemyShip, out Vec2 desiredPosition, out Vec2 desiredDirection)
		{
			Vec2 vec = this._selfShip.GameEntity.GlobalPosition.AsVec2 - enemyShip.GameEntity.GlobalPosition.AsVec2;
			if (enemyShip == this._givenTargetToBoard)
			{
				MatrixFrame globalFrame = enemyShip.GlobalFrame;
				desiredPosition = globalFrame.origin.AsVec2 + ((vec.DotProduct(globalFrame.rotation.f.AsVec2.LeftVec()) >= 0f) ? globalFrame.rotation.f.AsVec2.LeftVec().Normalized() : globalFrame.rotation.f.AsVec2.RightVec().Normalized()) * 12f;
			}
			else
			{
				this.ApproachFromDistance(enemyShip, out desiredPosition);
			}
			enemyShip.GlobalFrame.origin - this._selfShip.GlobalFrame.origin;
			MatrixFrame matrixFrame = enemyShip.GlobalFrame;
			Vec2 asVec = matrixFrame.rotation.f.AsVec2;
			matrixFrame = this._selfShip.GlobalFrame;
			if (asVec.DotProduct(matrixFrame.rotation.f.AsVec2) >= 0f)
			{
				matrixFrame = enemyShip.GlobalFrame;
				desiredDirection = matrixFrame.rotation.f.AsVec2.Normalized();
				return;
			}
			matrixFrame = enemyShip.GlobalFrame;
			desiredDirection = -matrixFrame.rotation.f.AsVec2.Normalized();
		}

		// Token: 0x060012A0 RID: 4768 RVA: 0x000891A0 File Offset: 0x000873A0
		public void CalculateShipOrders(out Vec2 desiredPosition, out Vec2 desiredDirection, out MissionShip boardingTargetShip)
		{
			this.CheckAndSwitchState();
			MatrixFrame globalFrame = this._selfShip.GlobalFrame;
			desiredPosition = globalFrame.origin.AsVec2;
			desiredDirection = this._selfShip.GlobalFrame.rotation.f.AsVec2.Normalized();
			boardingTargetShip = null;
			if (this._givenTargetToBoard != null && this._effectiveTarget != null)
			{
				this.DetermineEffectiveTargetShip();
				switch (this.State)
				{
				case NavalBehaviorBoardShipSubtask.ShipBoardingState.ApproachFromFarAway:
					this.ApproachFromDistance(this._effectiveTarget, out desiredPosition);
					boardingTargetShip = null;
					break;
				case NavalBehaviorBoardShipSubtask.ShipBoardingState.GettingClose:
					this.GettingCloseCase(this._effectiveTarget, out desiredPosition, out desiredDirection);
					boardingTargetShip = null;
					break;
				case NavalBehaviorBoardShipSubtask.ShipBoardingState.AdjustingOrientation:
				case NavalBehaviorBoardShipSubtask.ShipBoardingState.InPosition:
					this.GettingCloseCase(this._effectiveTarget, out desiredPosition, out desiredDirection);
					boardingTargetShip = this._effectiveTarget;
					break;
				case NavalBehaviorBoardShipSubtask.ShipBoardingState.Connected:
					boardingTargetShip = this._givenTargetToBoard;
					break;
				case NavalBehaviorBoardShipSubtask.ShipBoardingState.InactiveStuck:
					boardingTargetShip = null;
					break;
				}
				this._cachedEffectiveDistance = desiredPosition.Distance(globalFrame.origin.AsVec2);
			}
		}

		// Token: 0x04000A79 RID: 2681
		private const float MinimumBoardingDistance = 3f;

		// Token: 0x04000A7A RID: 2682
		private const float IdealBoardingDistance = 12f;

		// Token: 0x04000A7B RID: 2683
		private const float MaximumBoardingDistance = 30f;

		// Token: 0x04000A7C RID: 2684
		private const float DriftedAwayDistance = 50f;

		// Token: 0x04000A7D RID: 2685
		private MissionShip _selfShip;

		// Token: 0x04000A7E RID: 2686
		private MissionShip _givenTargetToBoard;

		// Token: 0x04000A7F RID: 2687
		private MissionShip _effectiveTarget;

		// Token: 0x04000A80 RID: 2688
		private bool _givenSideToBoardIsRight;

		// Token: 0x04000A81 RID: 2689
		private bool _effectiveSideToBoardIsRight;

		// Token: 0x04000A82 RID: 2690
		private float _cachedEffectiveDistance = float.MaxValue;

		// Token: 0x02000271 RID: 625
		public enum ShipBoardingState
		{
			// Token: 0x040010AF RID: 4271
			ApproachFromFarAway,
			// Token: 0x040010B0 RID: 4272
			GettingClose,
			// Token: 0x040010B1 RID: 4273
			AdjustingOrientation,
			// Token: 0x040010B2 RID: 4274
			InPosition,
			// Token: 0x040010B3 RID: 4275
			Connected,
			// Token: 0x040010B4 RID: 4276
			InactiveStuck
		}
	}
}
