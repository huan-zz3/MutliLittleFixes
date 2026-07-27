using System;
using System.Collections.Generic;
using NavalDLC.Missions.Deployment;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.Objects.UsableMachines;
using NavalDLC.Missions.ShipControl;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions
{
	// Token: 0x02000081 RID: 129
	public class ShipOrder
	{
		// Token: 0x17000183 RID: 387
		// (get) Token: 0x0600092F RID: 2351 RVA: 0x00040327 File Offset: 0x0003E527
		// (set) Token: 0x06000930 RID: 2352 RVA: 0x00040330 File Offset: 0x0003E530
		public MissionShip TargetShip
		{
			get
			{
				return this._targetShip;
			}
			private set
			{
				if (this._targetShip != value)
				{
					if (value == null)
					{
						this._targetShip = null;
						this.SetBoardingTargetShip(null);
						return;
					}
					this._targetShip = value;
					if (this.MovementOrderEnum == ShipOrder.ShipMovementOrderEnum.Engage)
					{
						this.SetBoardingTargetShip(this._targetShip);
						return;
					}
					this.SetBoardingTargetShip(null);
				}
			}
		}

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x06000931 RID: 2353 RVA: 0x0004037C File Offset: 0x0003E57C
		public bool HasAIController
		{
			get
			{
				return this._ownerShip.IsAIControlled;
			}
		}

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x06000932 RID: 2354 RVA: 0x00040389 File Offset: 0x0003E589
		// (set) Token: 0x06000933 RID: 2355 RVA: 0x00040391 File Offset: 0x0003E591
		public bool IsAIControllableWithoutTroops { get; private set; }

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x06000934 RID: 2356 RVA: 0x0004039A File Offset: 0x0003E59A
		public bool IsAIControllable
		{
			get
			{
				return this._ownerShip.IsAIControlled && (this._ownerShip.AnyActiveFormationTroopOnShip || this.IsAIControllableWithoutTroops);
			}
		}

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x06000935 RID: 2357 RVA: 0x000403C0 File Offset: 0x0003E5C0
		public bool HasStaticOrder
		{
			get
			{
				return this.MovementOrderEnum < ShipOrder.ShipMovementOrderEnum.StaticOrderCount;
			}
		}

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x06000936 RID: 2358 RVA: 0x000403CB File Offset: 0x0003E5CB
		public bool IsAutoSelectingTargetShip
		{
			get
			{
				return this._autoSelectTargetShip;
			}
		}

		// Token: 0x17000189 RID: 393
		// (get) Token: 0x06000937 RID: 2359 RVA: 0x000403D3 File Offset: 0x0003E5D3
		// (set) Token: 0x06000938 RID: 2360 RVA: 0x000403DB File Offset: 0x0003E5DB
		public int OarsmenLevel { get; private set; } = 2;

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x06000939 RID: 2361 RVA: 0x000403E4 File Offset: 0x0003E5E4
		// (set) Token: 0x0600093A RID: 2362 RVA: 0x000403EC File Offset: 0x0003E5EC
		public bool TickDetachmentsNeeded { get; private set; }

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x0600093B RID: 2363 RVA: 0x000403F5 File Offset: 0x0003E5F5
		// (set) Token: 0x0600093C RID: 2364 RVA: 0x000403FD File Offset: 0x0003E5FD
		public bool BoardAtWill { get; private set; }

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x0600093D RID: 2365 RVA: 0x00040406 File Offset: 0x0003E606
		// (set) Token: 0x0600093E RID: 2366 RVA: 0x0004040E File Offset: 0x0003E60E
		public bool IsBoardingAvailable { get; set; } = true;

		// Token: 0x1700018D RID: 397
		// (get) Token: 0x0600093F RID: 2367 RVA: 0x00040417 File Offset: 0x0003E617
		// (set) Token: 0x06000940 RID: 2368 RVA: 0x0004041F File Offset: 0x0003E61F
		public ShipOrder.ShipMovementOrderEnum MovementOrderEnum { get; private set; }

		// Token: 0x1700018E RID: 398
		// (get) Token: 0x06000941 RID: 2369 RVA: 0x00040428 File Offset: 0x0003E628
		public MissionShip ClosestEnemyShip
		{
			get
			{
				return this._closestEnemyShip.Value;
			}
		}

		// Token: 0x1700018F RID: 399
		// (get) Token: 0x06000942 RID: 2370 RVA: 0x00040435 File Offset: 0x0003E635
		public bool IsEnemyOnShip
		{
			get
			{
				return this._isEnemyOnShip.Value;
			}
		}

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x06000943 RID: 2371 RVA: 0x00040442 File Offset: 0x0003E642
		public int EnforceSailUsage
		{
			get
			{
				if (this._ownerFormation != null && this._ownerShip.IsAIControlled)
				{
					return this._enforceSailUsage;
				}
				return 0;
			}
		}

		// Token: 0x17000191 RID: 401
		// (get) Token: 0x06000944 RID: 2372 RVA: 0x00040461 File Offset: 0x0003E661
		public Vec2 TargetPosition
		{
			get
			{
				return this._orderGlobalPosition;
			}
		}

		// Token: 0x17000192 RID: 402
		// (get) Token: 0x06000945 RID: 2373 RVA: 0x00040469 File Offset: 0x0003E669
		public Vec2 TargetDirection
		{
			get
			{
				return this._orderGlobalDirection;
			}
		}

		// Token: 0x06000946 RID: 2374 RVA: 0x00040474 File Offset: 0x0003E674
		public ShipOrder(MissionShip missionShip, Formation ownerFormation)
		{
			this._ownerShip = missionShip;
			this.FormationJoinShip(ownerFormation);
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this._navalShipsLogic.ShipControllerChanged += this.OnShipControllerChanged;
			this._navalShipsLogic.ShipRemovedEvent += this.OnShipRemoved;
			this._isEnemyOnShip = new QueryData<bool>(delegate
			{
				if (this._ownerShip.Team == null)
				{
					return false;
				}
				foreach (Team team in Mission.Current.Teams)
				{
					if (team.IsEnemyOf(this._ownerShip.Team))
					{
						foreach (Agent agent in team.ActiveAgents)
						{
							if (this._ownerShip.GetIsAgentOnShip(agent, false))
							{
								return true;
							}
						}
					}
				}
				return false;
			}, 3f);
			this._closestEnemyShip = new QueryData<MissionShip>(delegate
			{
				float num = float.MaxValue;
				MissionShip missionShip2 = null;
				Vec3 origin = this._ownerShip.GlobalFrame.origin;
				foreach (Team team2 in Mission.Current.Teams)
				{
					if (MBExtensions.IsOpponentOf(this._ownerFormation.Team.Side, team2.Side))
					{
						foreach (Formation formation in team2.FormationsIncludingEmpty)
						{
							if (formation.CountOfUnits > 0)
							{
								MissionShip missionShip3;
								this._navalShipsLogic.GetShip(team2.TeamSide, formation.FormationIndex, out missionShip3);
								float num2 = missionShip3.GlobalFrame.origin.DistanceSquared(origin);
								if (num2 < num)
								{
									num = num2;
									missionShip2 = missionShip3;
								}
							}
						}
					}
				}
				return missionShip2;
			}, 5f);
			this.MovementOrderEnum = ShipOrder.ShipMovementOrderEnum.Stop;
			this._autoSelectTargetShip = false;
			this._shipIndependenceState = ShipOrder.ShipIndependenceState.Independent;
			this._detachmentTickTimer = new RandomTimer(Mission.Current.CurrentTime, 0.9f, 1.1f);
			this.TickDetachmentsNeeded = true;
			this._availableUnitList = new MBList<IFormationUnit>();
			this._orderTimer = new MissionTimer(1f);
			this._orderTimer.Set(MBRandom.RandomFloat * 1f);
			this._placementDetachmentTimer = new MissionTimer(5f);
			this._placementDetachmentTimer.Set(MBRandom.RandomFloat * 5f);
			Vec2 asVec = this._ownerShip.GlobalFrame.origin.AsVec2;
			this.SetTargetPosition(in asVec, true);
		}

		// Token: 0x06000947 RID: 2375 RVA: 0x00040600 File Offset: 0x0003E800
		public void MakeEnemyOnShipExpire()
		{
			this._isEnemyOnShip.Expire();
		}

		// Token: 0x06000948 RID: 2376 RVA: 0x0004060D File Offset: 0x0003E80D
		public void SetEnforcedSailUsage(int enforce)
		{
			this._enforceSailUsage = enforce;
		}

		// Token: 0x06000949 RID: 2377 RVA: 0x00040616 File Offset: 0x0003E816
		public void SetFormation(Formation formation)
		{
			this._ownerFormation = formation;
		}

		// Token: 0x0600094A RID: 2378 RVA: 0x00040620 File Offset: 0x0003E820
		public void OnShipCaptured(MissionShip ship1, MissionShip ship2)
		{
			this._closestEnemyShip.Expire();
			this._isEnemyOnShip.Expire();
			MissionShip targetShip = this.TargetShip;
			TeamSideEnum? teamSideEnum;
			if (targetShip == null)
			{
				teamSideEnum = null;
			}
			else
			{
				Team team = targetShip.Team;
				teamSideEnum = ((team != null) ? new TeamSideEnum?(team.TeamSide) : null);
			}
			TeamSideEnum? teamSideEnum2 = teamSideEnum;
			Team team2 = this._ownerShip.Team;
			TeamSideEnum? teamSideEnum3 = ((team2 != null) ? new TeamSideEnum?(team2.TeamSide) : null);
			if (!((teamSideEnum2.GetValueOrDefault() == teamSideEnum3.GetValueOrDefault()) & (teamSideEnum2 != null == (teamSideEnum3 != null))))
			{
				this.TargetShip = null;
				if (!this.HasStaticOrder)
				{
					this._orderTimer.Reset();
					this.UpdateDynamicMovementOrder();
				}
			}
			if (this._ownerShip == ship1 || this._ownerShip == ship2)
			{
				RangedSiegeWeapon shipSiegeWeapon = this._ownerShip.ShipSiegeWeapon;
				if (shipSiegeWeapon == null)
				{
					return;
				}
				shipSiegeWeapon.OnShipCaptured((this._ownerShip == ship1) ? ship1.BattleSide : ship2.BattleSide);
			}
		}

		// Token: 0x0600094B RID: 2379 RVA: 0x0004071D File Offset: 0x0003E91D
		public void SetAIControllableWithoutTroops(bool value)
		{
			this.IsAIControllableWithoutTroops = value;
		}

		// Token: 0x0600094C RID: 2380 RVA: 0x00040726 File Offset: 0x0003E926
		public void FormationJoinShip(Formation formation)
		{
			if (formation != null && formation != this._ownerFormation)
			{
				this._ownerFormation = formation;
				this.StartUsingMachines();
			}
		}

		// Token: 0x0600094D RID: 2381 RVA: 0x00040744 File Offset: 0x0003E944
		private void StartUsingMachines()
		{
			if (!this._ownerShip.BeingAbandoned)
			{
				this._ownerFormation.JoinDetachment(this._ownerShip.ClimbingMachineDetachment);
				foreach (ShipOarMachine shipOarMachine in this._ownerShip.LeftSideShipOarMachines)
				{
					if (!shipOarMachine.IsDisabled)
					{
						ModuleExtensions.StartUsingMachine(this._ownerFormation, shipOarMachine, true);
					}
				}
				foreach (ShipOarMachine shipOarMachine2 in this._ownerShip.RightSideShipOarMachines)
				{
					if (!shipOarMachine2.IsDisabled)
					{
						ModuleExtensions.StartUsingMachine(this._ownerFormation, shipOarMachine2, true);
					}
				}
				if (this._ownerShip.ShipSiegeWeapon != null && !this._ownerShip.ShipSiegeWeapon.IsDisabled)
				{
					ModuleExtensions.StartUsingMachine(this._ownerFormation, this._ownerShip.ShipSiegeWeapon, true);
				}
				if (!this._ownerShip.ShipControllerMachine.IsDisabled)
				{
					ModuleExtensions.StartUsingMachine(this._ownerFormation, this._ownerShip.ShipControllerMachine, true);
				}
				foreach (ShipAttachmentMachine shipAttachmentMachine in this._ownerShip.ShipAttachmentMachines)
				{
					if (!shipAttachmentMachine.IsDisabled)
					{
						ModuleExtensions.StartUsingMachine(this._ownerFormation, shipAttachmentMachine, true);
						shipAttachmentMachine.SetIsDisabledForAI(true);
					}
				}
				this._ownerFormation.JoinDetachment(this._ownerShip.ShipPlacementDetachment);
			}
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this._ownerShip.AttachmentPointMachines)
			{
				if (!shipAttachmentPointMachine.IsDisabled)
				{
					ModuleExtensions.StartUsingMachine(this._ownerFormation, shipAttachmentPointMachine, true);
					shipAttachmentPointMachine.SetIsDisabledForAI(true);
				}
			}
		}

		// Token: 0x0600094E RID: 2382 RVA: 0x0004095C File Offset: 0x0003EB5C
		public void StopUsingMachines(bool formationLeaving)
		{
			if (this._ownerFormation != null)
			{
				if (this._ownerFormation.Detachments.IndexOf(this._ownerShip.ClimbingMachineDetachment) >= 0)
				{
					this._ownerFormation.LeaveDetachment(this._ownerShip.ClimbingMachineDetachment);
				}
				foreach (ShipOarMachine shipOarMachine in this._ownerShip.LeftSideShipOarMachines)
				{
					if (this._ownerFormation.Detachments.IndexOf(shipOarMachine) >= 0)
					{
						ModuleExtensions.StopUsingMachine(this._ownerFormation, shipOarMachine, true);
					}
				}
				foreach (ShipOarMachine shipOarMachine2 in this._ownerShip.RightSideShipOarMachines)
				{
					if (this._ownerFormation.Detachments.IndexOf(shipOarMachine2) >= 0)
					{
						ModuleExtensions.StopUsingMachine(this._ownerFormation, shipOarMachine2, true);
					}
				}
				if (this._ownerShip.ShipSiegeWeapon != null && this._ownerFormation.Detachments.IndexOf(this._ownerShip.ShipSiegeWeapon) >= 0)
				{
					ModuleExtensions.StopUsingMachine(this._ownerFormation, this._ownerShip.ShipSiegeWeapon, true);
				}
				if (this._ownerFormation.Detachments.IndexOf(this._ownerShip.ShipControllerMachine) >= 0)
				{
					ModuleExtensions.StopUsingMachine(this._ownerFormation, this._ownerShip.ShipControllerMachine, true);
				}
				foreach (ShipAttachmentMachine shipAttachmentMachine in this._ownerShip.ShipAttachmentMachines)
				{
					if (this._ownerFormation.Detachments.IndexOf(shipAttachmentMachine) >= 0)
					{
						ModuleExtensions.StopUsingMachine(this._ownerFormation, shipAttachmentMachine, true);
						shipAttachmentMachine.SetIsDisabledForAI(true);
					}
				}
				if (formationLeaving || this._ownerShip.IsShipNavmeshDisabled || !this._ownerShip.BeingAbandoned)
				{
					foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this._ownerShip.AttachmentPointMachines)
					{
						if (this._ownerFormation.Detachments.IndexOf(shipAttachmentPointMachine) >= 0)
						{
							ModuleExtensions.StopUsingMachine(this._ownerFormation, shipAttachmentPointMachine, true);
							shipAttachmentPointMachine.SetIsDisabledForAI(true);
						}
					}
				}
				if (this._ownerFormation.Detachments.IndexOf(this._ownerShip.ShipPlacementDetachment) >= 0)
				{
					this._ownerFormation.LeaveDetachment(this._ownerShip.ShipPlacementDetachment);
				}
			}
		}

		// Token: 0x0600094F RID: 2383 RVA: 0x00040C10 File Offset: 0x0003EE10
		public void FormationLeaveShip()
		{
			if (this._ownerFormation != null)
			{
				this.StopUsingMachines(true);
				this._ownerFormation = null;
			}
		}

		// Token: 0x06000950 RID: 2384 RVA: 0x00040C28 File Offset: 0x0003EE28
		public bool GetIsChargeOrderOverridden()
		{
			return this._isChargeOrderOverridden;
		}

		// Token: 0x06000951 RID: 2385 RVA: 0x00040C30 File Offset: 0x0003EE30
		public bool IsOarsmenLevelLocked()
		{
			return this._oarLevelOverridden;
		}

		// Token: 0x06000952 RID: 2386 RVA: 0x00040C38 File Offset: 0x0003EE38
		public void SetOrderOarsmenLevel(int newOarsmenLevel)
		{
			this._originalOarsmenLevel = newOarsmenLevel;
			if (!this._oarLevelOverridden)
			{
				this.SetOarsmenLevel(this._originalOarsmenLevel);
			}
		}

		// Token: 0x06000953 RID: 2387 RVA: 0x00040C58 File Offset: 0x0003EE58
		private void SetOarsmenLevel(int newOarsmenLevel)
		{
			if (this.OarsmenLevel != newOarsmenLevel)
			{
				if (newOarsmenLevel > this.OarsmenLevel)
				{
					this.TickDetachmentsNeeded = true;
					int num = 0;
					int num2 = int.MaxValue;
					if (this.OarsmenLevel == 1)
					{
						num2 = (this._ownerFormation.Arrangement.UnitCount + this._ownerShip.ShipPlacementDetachment.CountOfAgents) / 2;
					}
					for (int i = 0; i < this._ownerShip.LeftSideShipOarMachines.Count; i++)
					{
						if (i >= this._ownerShip.RightSideShipOarMachines.Count)
						{
							break;
						}
						this._ownerShip.LeftSideShipOarMachines[i].SetIsDisabledForAI(false);
						this._ownerShip.RightSideShipOarMachines[i].SetIsDisabledForAI(false);
						if (newOarsmenLevel == 1)
						{
							num += 2;
							if (num >= num2)
							{
								break;
							}
						}
					}
				}
				else
				{
					int num3 = this._ownerShip.LeftSideShipOarMachines.Count + this._ownerShip.RightSideShipOarMachines.Count;
					int num4;
					if (newOarsmenLevel == 0)
					{
						num4 = 0;
					}
					else if (newOarsmenLevel == 1)
					{
						int num5 = this._ownerFormation.CountOfUnits;
						if (this._ownerFormation.HasPlayerControlledTroop)
						{
							num5--;
						}
						num4 = Math.Min(num3, num5) / 2;
					}
					else
					{
						num4 = num3;
					}
					this.LowerOarsmenLevelForOarMachines(this._ownerShip.LeftSideShipOarMachines, num4 / 2);
					this.LowerOarsmenLevelForOarMachines(this._ownerShip.RightSideShipOarMachines, num4 - num4 / 2);
				}
				this.OarsmenLevel = newOarsmenLevel;
			}
		}

		// Token: 0x06000954 RID: 2388 RVA: 0x00040DC0 File Offset: 0x0003EFC0
		private void LowerOarsmenLevelForOarMachines(MBReadOnlyList<ShipOarMachine> oars, int numberOfOarsNeedToBeActive)
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < oars.Count; i++)
			{
				ShipOarMachine shipOarMachine = oars[i];
				if (shipOarMachine.PilotStandingPoint.HasUser || shipOarMachine.PilotStandingPoint.HasAIMovingTo)
				{
					num++;
				}
				if (shipOarMachine.DestructionComponent.HitPoint <= 0f)
				{
					num2++;
				}
			}
			int num3 = oars.Count - numberOfOarsNeedToBeActive;
			int num4 = num - numberOfOarsNeedToBeActive;
			num3 -= num2;
			int num5 = 0;
			int num6 = 0;
			while (num6 < oars.Count && num3 > 0)
			{
				int num7 = ((num6 < (oars.Count + 1) / 2) ? (num6 * 2) : ((num6 - (oars.Count + 1) / 2) * 2 + 1));
				ShipOarMachine shipOarMachine2 = oars[num7];
				if (num5 == numberOfOarsNeedToBeActive)
				{
					shipOarMachine2.SetIsDisabledForAI(true);
					Agent pilotAgent = shipOarMachine2.PilotAgent;
					if (pilotAgent != null && this._navalShipsLogic.IsDeploymentMode)
					{
						pilotAgent.StopUsingGameObject(true, 1);
					}
					num3--;
				}
				else if (num4 <= 0)
				{
					if (!shipOarMachine2.PilotStandingPoint.HasUser && !shipOarMachine2.PilotStandingPoint.HasAIMovingTo)
					{
						shipOarMachine2.SetIsDisabledForAI(true);
						Agent pilotAgent2 = shipOarMachine2.PilotAgent;
						if (pilotAgent2 != null && this._navalShipsLogic.IsDeploymentMode)
						{
							pilotAgent2.StopUsingGameObject(true, 1);
						}
						if (shipOarMachine2.DestructionComponent.HitPoint > 0f)
						{
							num3--;
						}
					}
					else
					{
						num5++;
					}
				}
				else
				{
					if (shipOarMachine2.PilotStandingPoint.HasUser || shipOarMachine2.PilotStandingPoint.HasAIMovingTo)
					{
						num4--;
					}
					shipOarMachine2.SetIsDisabledForAI(true);
					Agent pilotAgent3 = shipOarMachine2.PilotAgent;
					if (pilotAgent3 != null && this._navalShipsLogic.IsDeploymentMode)
					{
						pilotAgent3.StopUsingGameObject(true, 1);
					}
					num3--;
				}
				num6++;
			}
		}

		// Token: 0x06000955 RID: 2389 RVA: 0x00040F82 File Offset: 0x0003F182
		public bool GetIsCuttingLoose()
		{
			return this._cutLooseOrderActive && this._ownerShip.GetIsAnyBridgeActive();
		}

		// Token: 0x06000956 RID: 2390 RVA: 0x00040F99 File Offset: 0x0003F199
		public void ToggleCutLoose()
		{
			this.SetCutLoose(!this._cutLooseOrderActive);
		}

		// Token: 0x06000957 RID: 2391 RVA: 0x00040FAC File Offset: 0x0003F1AC
		public void SetCutLoose(bool enable)
		{
			if (this._cutLooseOrderActive != enable)
			{
				if (enable)
				{
					this.SetBoardingTargetShip(null);
					foreach (ShipAttachmentMachine shipAttachmentMachine in this._ownerShip.ShipAttachmentMachines)
					{
						if (!shipAttachmentMachine.IsShipAttachmentMachineBridged())
						{
							if (shipAttachmentMachine.PilotAgent != null)
							{
								shipAttachmentMachine.PilotAgent.StopUsingGameObjectMT(true, 3);
							}
							else if (shipAttachmentMachine.PilotStandingPoint.MovingAgent != null)
							{
								shipAttachmentMachine.PilotStandingPoint.MovingAgent.StopUsingGameObjectMT(true, 3);
							}
						}
					}
					this.TickDetachmentsNeeded = true;
				}
				this._cutLooseOrderActive = enable;
				foreach (ShipAttachmentMachine shipAttachmentMachine2 in this._ownerShip.ShipAttachmentMachines)
				{
					if (this._cutLooseOrderActive)
					{
						if (!shipAttachmentMachine2.IsShipAttachmentMachineBridged())
						{
							if (shipAttachmentMachine2.PilotAgent != null)
							{
								shipAttachmentMachine2.PilotAgent.StopUsingGameObjectMT(true, 3);
							}
							else
							{
								Agent movingAgent = shipAttachmentMachine2.PilotStandingPoint.MovingAgent;
								if (movingAgent != null)
								{
									movingAgent.StopUsingGameObjectMT(true, 3);
								}
							}
						}
						shipAttachmentMachine2.SetIsDisabledForAI(false);
					}
					shipAttachmentMachine2.SetIsDisabledForAI(!enable);
				}
				foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this._ownerShip.AttachmentPointMachines)
				{
					shipAttachmentPointMachine.SetIsDisabledForAI(!enable);
				}
				this._navalShipsLogic.OnCutLooseOrder(this._ownerShip);
			}
		}

		// Token: 0x06000958 RID: 2392 RVA: 0x0004114C File Offset: 0x0003F34C
		public bool GetIsAttemptingBoarding()
		{
			return this._boardingTargetShip != null && !this._ownerShip.SearchShipConnection(this._boardingTargetShip, true, true, false, false);
		}

		// Token: 0x06000959 RID: 2393 RVA: 0x00041170 File Offset: 0x0003F370
		public MissionShip GetBoardingTargetShip()
		{
			return this._boardingTargetShip;
		}

		// Token: 0x0600095A RID: 2394 RVA: 0x00041178 File Offset: 0x0003F378
		public void SetBoardingTargetShip(MissionShip missionShip)
		{
			if (this._boardingTargetShip != missionShip && this.IsBoardingAvailable)
			{
				if (missionShip != null)
				{
					this._cutLooseOrderActive = false;
					foreach (ShipAttachmentMachine shipAttachmentMachine in this._ownerShip.ShipAttachmentMachines)
					{
						if (shipAttachmentMachine.IsShipAttachmentMachineBridged() || !shipAttachmentMachine.CalculateCanConnectToTargetShip(missionShip))
						{
							if (shipAttachmentMachine.PilotAgent != null)
							{
								shipAttachmentMachine.PilotAgent.StopUsingGameObjectMT(true, 3);
							}
							else if (shipAttachmentMachine.PilotStandingPoint.MovingAgent != null)
							{
								shipAttachmentMachine.PilotStandingPoint.MovingAgent.StopUsingGameObjectMT(true, 3);
							}
						}
					}
					foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this._ownerShip.AttachmentPointMachines)
					{
						if (shipAttachmentPointMachine.IsShipAttachmentPointBridged())
						{
							if (shipAttachmentPointMachine.PilotAgent != null)
							{
								shipAttachmentPointMachine.PilotAgent.StopUsingGameObjectMT(true, 3);
							}
							else if (shipAttachmentPointMachine.PilotStandingPoint.MovingAgent != null)
							{
								shipAttachmentPointMachine.PilotStandingPoint.MovingAgent.StopUsingGameObjectMT(true, 3);
							}
						}
					}
					this.TickDetachmentsNeeded = true;
					this._navalShipsLogic.OnBoardingOrder(this._ownerShip, missionShip);
				}
				foreach (ShipAttachmentMachine shipAttachmentMachine2 in this._ownerShip.ShipAttachmentMachines)
				{
					if (missionShip != null)
					{
						shipAttachmentMachine2.SetPreferredTargetShip(missionShip);
						shipAttachmentMachine2.SetIsDisabledForAI(false);
					}
					else
					{
						shipAttachmentMachine2.SetPreferredTargetShip(null);
						shipAttachmentMachine2.SetIsDisabledForAI(true);
					}
				}
				this._boardingTargetShip = missionShip;
			}
		}

		// Token: 0x0600095B RID: 2395 RVA: 0x00041338 File Offset: 0x0003F538
		public void SetShipStopOrder()
		{
			this.MovementOrderEnum = ShipOrder.ShipMovementOrderEnum.Stop;
			this._autoSelectTargetShip = false;
			this._orderTimer.Reset();
			this.SetStopShipAux();
		}

		// Token: 0x0600095C RID: 2396 RVA: 0x00041359 File Offset: 0x0003F559
		public void SetShipMovementOrder(in Vec2 targetPosition)
		{
			this.MovementOrderEnum = ShipOrder.ShipMovementOrderEnum.Move;
			this._autoSelectTargetShip = false;
			this._orderTimer.Reset();
			this.SetTargetPosition(in targetPosition, false);
		}

		// Token: 0x0600095D RID: 2397 RVA: 0x0004137C File Offset: 0x0003F57C
		public void SetShipRetreatOrder()
		{
			this.MovementOrderEnum = ShipOrder.ShipMovementOrderEnum.Retreat;
			this._autoSelectTargetShip = false;
			this._orderTimer.Reset();
			Vec2 asVec = Mission.Current.GetClosestFleePositionForFormation(this._ownerFormation).AsVec2;
			this.SetTargetPosition(in asVec, false);
		}

		// Token: 0x0600095E RID: 2398 RVA: 0x000413C4 File Offset: 0x0003F5C4
		public void Tick()
		{
			if (this.HasAIController && this._ownerShip.AIController.HasTarget && !this.IsAIControllable)
			{
				this._ownerShip.AIController.ClearTarget();
			}
			if (this._ownerFormation != null && this._ownerFormation.CountOfUnits > 0)
			{
				if (Mission.Current.IsDeploymentFinished)
				{
					if (this._ownerShip.GetIsConnected())
					{
						switch (this.MovementOrderEnum)
						{
						case ShipOrder.ShipMovementOrderEnum.Move:
						case ShipOrder.ShipMovementOrderEnum.Retreat:
						case ShipOrder.ShipMovementOrderEnum.StaticOrderCount:
						case ShipOrder.ShipMovementOrderEnum.Skirmish:
							if (this._boardingTargetShip == null || !this._ownerShip.SearchShipConnection(this._boardingTargetShip, true, false, false, true))
							{
								this.SetCutLoose(true);
							}
							break;
						case ShipOrder.ShipMovementOrderEnum.Engage:
							if (!this._autoSelectTargetShip)
							{
								MissionShip targetShip = this.TargetShip;
								if (((targetShip != null) ? targetShip.Formation : null) == null || this.TargetShip.Formation.CountOfUnits <= 0 || (!MBExtensions.IsOpponentOf(this.TargetShip.Formation.Team.Side, this._ownerFormation.Team.Side) && !this.TargetShip.GetIsConnectedToEnemy()))
								{
									this._autoSelectTargetShip = true;
								}
							}
							if ((!this._autoSelectTargetShip && (this.TargetShip == null || !this._ownerShip.SearchShipConnection(this.TargetShip, true, false, false, true)) && (this._boardingTargetShip == null || !this._ownerShip.SearchShipConnection(this._boardingTargetShip, true, false, false, true))) || (this._autoSelectTargetShip && !this._ownerShip.SearchShipConnection(null, true, true, true, true)))
							{
								this.SetCutLoose(true);
							}
							break;
						}
					}
					else if (!this.HasStaticOrder && this._orderTimer.Check(true))
					{
						this.UpdateDynamicMovementOrder();
					}
				}
				this.CheckAndChangeIndependenceState();
				if (this.HasAIController)
				{
					this.DecideOarsmenLevel();
				}
				this.TickClimbingMachines();
				if (this._ownerShip.ShipPlacementDetachment.IsUsedByFormation(this._ownerFormation))
				{
					Vec2 vec;
					if (this._ownerShip.GetIsConnectedToEnemyWithSide(out vec))
					{
						this._ownerShip.ShipPlacementDetachment.SetBoarding(true, vec);
					}
					else
					{
						this._ownerShip.ShipPlacementDetachment.SetBoarding(false, vec);
						this._ownerShip.ShipPlacementDetachment.SetUnderMissileFire(this._ownerFormation.QuerySystem.IsUnderRangedAttack);
					}
					if (this._ownerShip.ShipPlacementDetachment.IsTickRequired)
					{
						this._ownerShip.ShipPlacementDetachment.Tick();
					}
				}
				if (!this._ownerShip.IsSinking && (this.TickDetachmentsNeeded || this._detachmentTickTimer.Check(Mission.Current.CurrentTime)))
				{
					this.ManageShipDetachments();
					this._detachmentTickTimer.Reset(Mission.Current.CurrentTime);
				}
				if ((!this._ownerFormation.IsAIControlled || !this._ownerFormation.IsAIOwned) && this._ownerFormation.IsPlayerTroopInFormation)
				{
					MovementOrder.MovementOrderEnum orderEnum = this._ownerFormation.GetReadonlyMovementOrderReference().OrderEnum;
					if (orderEnum != 2)
					{
						if (orderEnum != 4)
						{
							if (!this._ownerShip.ShipPlacementDetachment.IsUsedByFormation(this._ownerFormation) && !this._ownerShip.IsShipNavmeshDisabled)
							{
								this._ownerFormation.JoinDetachment(this._ownerShip.ShipPlacementDetachment);
								return;
							}
							if (this._isChargeOrderOverridden && this._ownerShip.SearchShipConnection(null, true, true, true, true))
							{
								this.SetChargeOrder(true);
								this._ownerFormation.LeaveDetachment(this._ownerShip.ShipPlacementDetachment);
								this._isChargeOrderOverridden = false;
							}
						}
						else if (this._ownerShip.ShipPlacementDetachment.IsUsedByFormation(this._ownerFormation))
						{
							this._ownerFormation.LeaveDetachment(this._ownerShip.ShipPlacementDetachment);
							return;
						}
					}
					else if (this._ownerShip.ShipPlacementDetachment.IsUsedByFormation(this._ownerFormation))
					{
						if (this._ownerShip.SearchShipConnection(null, true, true, true, true))
						{
							this._ownerFormation.LeaveDetachment(this._ownerShip.ShipPlacementDetachment);
							return;
						}
					}
					else if (!this._ownerShip.SearchShipConnection(null, true, true, true, true))
					{
						if (!this._ownerShip.ShipPlacementDetachment.IsUsedByFormation(this._ownerFormation) && !this._ownerShip.IsShipNavmeshDisabled)
						{
							this._ownerFormation.JoinDetachment(this._ownerShip.ShipPlacementDetachment);
						}
						this._ownerShip.SetPositioningOrdersToRallyPoint(true, false);
						this._isChargeOrderOverridden = true;
						return;
					}
				}
			}
		}

		// Token: 0x0600095F RID: 2399 RVA: 0x00041818 File Offset: 0x0003FA18
		public void SetShipSkirmishOrder(bool autoTargetClosest = true)
		{
			MissionShip closestEnemyShip = this.ClosestEnemyShip;
			if (closestEnemyShip != null)
			{
				this.MovementOrderEnum = ShipOrder.ShipMovementOrderEnum.Skirmish;
				this._autoSelectTargetShip = autoTargetClosest;
				this._orderTimer.Reset();
				this.UpdateSkirmishOrder(closestEnemyShip);
				return;
			}
			this.SetShipStopOrder();
		}

		// Token: 0x06000960 RID: 2400 RVA: 0x00041858 File Offset: 0x0003FA58
		public void SetShipFollowOrder(MissionShip shipToFollow, float offsetDistance)
		{
			this.MovementOrderEnum = ShipOrder.ShipMovementOrderEnum.StaticOrderCount;
			this._autoSelectTargetShip = false;
			this._orderTimer.Reset();
			Vec2 vec;
			vec..ctor(offsetDistance, -15f);
			this.UpdateFollowOrder(shipToFollow, in vec);
		}

		// Token: 0x06000961 RID: 2401 RVA: 0x00041894 File Offset: 0x0003FA94
		public void SetShipMovementOrder(Vec2 targetPosition, in Vec2 targetDirection)
		{
			this.MovementOrderEnum = ShipOrder.ShipMovementOrderEnum.Move;
			this._autoSelectTargetShip = false;
			this._orderTimer.Reset();
			this.SetTargetState(in targetPosition, in targetDirection);
		}

		// Token: 0x06000962 RID: 2402 RVA: 0x000418B8 File Offset: 0x0003FAB8
		public void SetShipEngageOrder(bool autoTargetClosest = true)
		{
			MissionShip closestEnemyShip = this.ClosestEnemyShip;
			if (closestEnemyShip != null)
			{
				this.MovementOrderEnum = ShipOrder.ShipMovementOrderEnum.Engage;
				this._autoSelectTargetShip = autoTargetClosest;
				this._orderTimer.Reset();
				this._engageGivenTargetOrder = closestEnemyShip;
				this.UpdateEngageOrder(closestEnemyShip);
				return;
			}
			this.SetShipStopOrder();
		}

		// Token: 0x06000963 RID: 2403 RVA: 0x000418FD File Offset: 0x0003FAFD
		public void SetShipEngageOrder(MissionShip shipToEngage)
		{
			this.MovementOrderEnum = ShipOrder.ShipMovementOrderEnum.Engage;
			this._autoSelectTargetShip = false;
			this._orderTimer.Reset();
			this._engageGivenTargetOrder = shipToEngage;
			this.UpdateEngageOrder(shipToEngage);
		}

		// Token: 0x06000964 RID: 2404 RVA: 0x00041926 File Offset: 0x0003FB26
		public void SetShipSkirmishOrder(MissionShip shipToSkirmish)
		{
			this.MovementOrderEnum = ShipOrder.ShipMovementOrderEnum.Skirmish;
			this._autoSelectTargetShip = false;
			this._orderTimer.Reset();
			this._inSkirmishPosition = false;
			this.UpdateSkirmishOrder(shipToSkirmish);
		}

		// Token: 0x06000965 RID: 2405 RVA: 0x00041950 File Offset: 0x0003FB50
		private void ProjectOrderPositionToBoundaries(ref Vec2 orderPosition)
		{
			Mission mission = Mission.Current;
			bool flag = false;
			NavalMissionDeploymentPlanningLogic navalMissionDeploymentPlanningLogic;
			if (mission.IsDeploymentFinished)
			{
				flag = true;
			}
			else if (this._ownerShip.Team != null && mission.GetDeploymentPlan<NavalMissionDeploymentPlanningLogic>(ref navalMissionDeploymentPlanningLogic))
			{
				if (!navalMissionDeploymentPlanningLogic.IsPositionInsideDeploymentBoundaries(this._ownerShip.Team, ref orderPosition))
				{
					Vec2 closestDeploymentBoundaryPosition = navalMissionDeploymentPlanningLogic.GetClosestDeploymentBoundaryPosition(this._ownerShip.Team, ref orderPosition);
					orderPosition = closestDeploymentBoundaryPosition;
				}
			}
			else
			{
				flag = true;
			}
			if (flag && !Mission.Current.IsPositionInsideBoundaries(orderPosition))
			{
				Vec2 closestBoundaryPosition = Mission.Current.GetClosestBoundaryPosition(orderPosition);
				orderPosition = closestBoundaryPosition;
			}
		}

		// Token: 0x06000966 RID: 2406 RVA: 0x000419EC File Offset: 0x0003FBEC
		private Agent GetNextAgent(ref int currentIndex)
		{
			while (currentIndex >= 0)
			{
				List<IFormationUnit> availableUnitList = this._availableUnitList;
				int num = currentIndex;
				currentIndex = num - 1;
				Agent agent;
				if ((agent = availableUnitList[num] as Agent) != null && agent.IsAIControlled && agent.IsDetachableFromFormation && agent.CanBeAssignedForScriptedMovement() && (agent != Agent.Main || !this._ownerShip.HasPlayerStandingPointEntity))
				{
					return agent;
				}
			}
			return null;
		}

		// Token: 0x06000967 RID: 2407 RVA: 0x00041A4D File Offset: 0x0003FC4D
		private void UpdateStaticMovementOrder()
		{
			if (this.MovementOrderEnum == ShipOrder.ShipMovementOrderEnum.Stop)
			{
				this.SetShipStopOrder();
				return;
			}
			if (this.MovementOrderEnum == ShipOrder.ShipMovementOrderEnum.Move)
			{
				this.SetShipMovementOrder(this._orderGlobalPosition, in this._orderGlobalDirection);
				return;
			}
			if (this.MovementOrderEnum == ShipOrder.ShipMovementOrderEnum.Retreat)
			{
				this.SetShipRetreatOrder();
			}
		}

		// Token: 0x06000968 RID: 2408 RVA: 0x00041A89 File Offset: 0x0003FC89
		public void TickClimbingMachines()
		{
			this._ownerShip.ClimbingMachineDetachment.TickClimbingMachines();
		}

		// Token: 0x06000969 RID: 2409 RVA: 0x00041A9C File Offset: 0x0003FC9C
		private void UpdateDynamicMovementOrder()
		{
			switch (this.MovementOrderEnum)
			{
			case ShipOrder.ShipMovementOrderEnum.StaticOrderCount:
				if (this.TargetShip == null)
				{
					this.SetShipStopOrder();
					return;
				}
				this.UpdateFollowOrder(this.TargetShip, in this._offsetPosition);
				return;
			case ShipOrder.ShipMovementOrderEnum.Engage:
				if (this._autoSelectTargetShip && this.TargetShip == this._engageGivenTargetOrder)
				{
					this.TrySelectBetterTargetShip(4f);
				}
				else
				{
					MissionShip targetShip = this.TargetShip;
					if (((targetShip != null) ? targetShip.Formation : null) == null || this.TargetShip.Formation.CountOfUnits <= 0 || (!MBExtensions.IsOpponentOf(this.TargetShip.Formation.Team.Side, this._ownerFormation.Team.Side) && !this.TargetShip.GetIsConnectedToEnemy()))
					{
						this._autoSelectTargetShip = true;
						this.TargetShip = this.ClosestEnemyShip;
					}
				}
				if (this.TargetShip == null)
				{
					this.SetShipStopOrder();
					return;
				}
				this.UpdateEngageOrder(this._engageGivenTargetOrder);
				return;
			case ShipOrder.ShipMovementOrderEnum.Skirmish:
				if (this._autoSelectTargetShip)
				{
					this.TrySelectBetterTargetShip(4f);
				}
				if (this.TargetShip == null)
				{
					this.SetShipStopOrder();
					return;
				}
				this.UpdateSkirmishOrder(this.TargetShip);
				return;
			default:
				return;
			}
		}

		// Token: 0x0600096A RID: 2410 RVA: 0x00041BC8 File Offset: 0x0003FDC8
		private void TrySelectBetterTargetShip(float decisionDistance = 4f)
		{
			MissionShip targetShip = this.TargetShip;
			if (((targetShip != null) ? targetShip.Formation : null) == null || this.TargetShip.Formation.CountOfUnits <= 0 || (!MBExtensions.IsOpponentOf(this.TargetShip.Formation.Team.Side, this._ownerFormation.Team.Side) && !this.TargetShip.GetIsConnectedToEnemy()))
			{
				this._engageGivenTargetOrder = this.ClosestEnemyShip;
				this.TargetShip = this.ClosestEnemyShip;
				return;
			}
			MissionShip closestEnemyShip = this.ClosestEnemyShip;
			if (closestEnemyShip != null)
			{
				MatrixFrame matrixFrame = this._ownerShip.GlobalFrame;
				Vec2 asVec = matrixFrame.origin.AsVec2;
				matrixFrame = this.TargetShip.GlobalFrame;
				float num = asVec.Distance(matrixFrame.origin.AsVec2);
				matrixFrame = closestEnemyShip.GlobalFrame;
				if (asVec.Distance(matrixFrame.origin.AsVec2) + decisionDistance < num)
				{
					this._engageGivenTargetOrder = closestEnemyShip;
					this.TargetShip = closestEnemyShip;
				}
			}
		}

		// Token: 0x0600096B RID: 2411 RVA: 0x00041CC0 File Offset: 0x0003FEC0
		private void DecideOarsmenLevel()
		{
			ShipOrder.ShipMovementOrderEnum movementOrderEnum = this.MovementOrderEnum;
			if (movementOrderEnum != ShipOrder.ShipMovementOrderEnum.Stop)
			{
				if (movementOrderEnum == ShipOrder.ShipMovementOrderEnum.Engage)
				{
					this.SetOrderOarsmenLevel(2);
					return;
				}
				if (movementOrderEnum != ShipOrder.ShipMovementOrderEnum.Skirmish)
				{
					return;
				}
				if (this.TargetShip != null)
				{
					if (this._originalOarsmenLevel != 0)
					{
						float num = this.TargetShip.GameEntity.GlobalPosition.DistanceSquared(this._ownerShip.GameEntity.GlobalPosition);
						float num2 = 4356f;
						float num3 = 3240f;
						if (num <= num2 && num >= num3)
						{
							this.SetOrderOarsmenLevel(0);
							return;
						}
					}
					else
					{
						float num4 = this.TargetShip.GameEntity.GlobalPosition.DistanceSquared(this._ownerShip.GameEntity.GlobalPosition);
						float num5 = 5184f;
						float num6 = 2304f;
						if (num4 > num5 || num4 < num6)
						{
							this.SetOrderOarsmenLevel(2);
							return;
						}
					}
				}
			}
			else
			{
				this.SetOrderOarsmenLevel(2);
			}
		}

		// Token: 0x0600096C RID: 2412 RVA: 0x00041DA8 File Offset: 0x0003FFA8
		private void UpdateFollowOrder(MissionShip shipToFollow, in Vec2 offsetPosition)
		{
			this.SetMovementTargetShip(shipToFollow, in offsetPosition, 0f);
		}

		// Token: 0x0600096D RID: 2413 RVA: 0x00041DB8 File Offset: 0x0003FFB8
		private void UpdateSkirmishOrder(MissionShip shipToSkirmish)
		{
			this.TargetShip = shipToSkirmish;
			if (this._ownerShip.IsAIControlled)
			{
				Vec3 vec = this._ownerShip.GlobalFrame.origin - shipToSkirmish.GlobalFrame.origin;
				Vec3 vec2 = vec.NormalizedCopy();
				Vec3 vec3 = shipToSkirmish.GlobalFrame.origin + vec2 * 60f;
				Vec3 vec4 = vec3 - this._ownerShip.GlobalFrame.origin;
				Vec2 vec5;
				MatrixFrame matrixFrame;
				if (vec4.Length < 3f * (this._inSkirmishPosition ? 2f : 1f))
				{
					this._inSkirmishPosition = true;
					AIShipController aicontroller = this._ownerShip.AIController;
					vec5 = vec2.AsVec2;
					vec5 = vec5.LeftVec();
					matrixFrame = this._ownerShip.GlobalFrame;
					NavalState navalState;
					if (vec5.DotProduct(matrixFrame.rotation.f.AsVec2) <= 0f)
					{
						matrixFrame = this._ownerShip.GlobalFrame;
						vec5 = matrixFrame.origin.AsVec2;
						Vec2 vec6 = vec2.AsVec2;
						vec6 = vec6.RightVec();
						vec6 = vec6.Normalized();
						navalState = new NavalState(in vec5, in vec6, 0f);
					}
					else
					{
						matrixFrame = this._ownerShip.GlobalFrame;
						Vec2 asVec = matrixFrame.origin.AsVec2;
						Vec2 vec7 = vec2.AsVec2;
						vec7 = vec7.LeftVec();
						vec7 = vec7.Normalized();
						navalState = new NavalState(in asVec, in vec7, 0f);
					}
					NavalState navalState2 = navalState;
					aicontroller.SetTargetState(in navalState2, false);
					return;
				}
				this._inSkirmishPosition = false;
				vec5 = vec.AsVec2;
				Vec2 vec8 = -vec5.Normalized();
				vec5 = vec4.AsVec2;
				Vec2 vec9 = vec5.Normalized();
				float num = vec8.DotProduct(vec9);
				matrixFrame = this._ownerShip.GlobalFrame;
				vec5 = matrixFrame.rotation.f.AsVec2;
				Vec2 vec10 = vec5.Normalized();
				if (num >= 0f)
				{
					vec5 = vec4.AsVec2;
					if (vec5.Length >= 60f || vec8.DotProduct(vec10) < 0.5f)
					{
						NavalState navalState2;
						if (vec.Length >= 120f)
						{
							AIShipController aicontroller2 = this._ownerShip.AIController;
							vec5 = vec3.AsVec2;
							navalState2 = new NavalState(in vec5, in vec9, this._ownerShip.Physics.LinearVelocity.Length);
							aicontroller2.SetTargetState(in navalState2, false);
							return;
						}
						if (vec10.DotProduct(vec9) < 0.6f)
						{
							AIShipController aicontroller3 = this._ownerShip.AIController;
							vec5 = vec3.AsVec2;
							navalState2 = new NavalState(in vec5, in vec9, this._ownerShip.Physics.LinearVelocity.Length);
							aicontroller3.SetTargetState(in navalState2, false);
							return;
						}
						matrixFrame = this._ownerShip.GlobalFrame;
						vec5 = matrixFrame.rotation.f.AsVec2;
						vec5 = vec5.RightVec();
						Vec2 vec11 = vec5.Normalized();
						float num2 = vec9.DotProduct(vec11);
						if (MathF.Abs(num2) <= 0.1f)
						{
							AIShipController aicontroller4 = this._ownerShip.AIController;
							matrixFrame = this._ownerShip.GlobalFrame;
							Vec2 vec12 = matrixFrame.origin.AsVec2 + vec10 * 50f;
							float num3 = 10f;
							Vec2 vec13;
							if (num2 < 0f)
							{
								vec13 = vec11;
							}
							else
							{
								matrixFrame = this._ownerShip.GlobalFrame;
								vec5 = matrixFrame.rotation.f.AsVec2;
								vec5 = vec5.LeftVec();
								vec13 = vec5.Normalized();
							}
							vec5 = vec12 + num3 * vec13;
							navalState2 = new NavalState(in vec5, in vec10, this._ownerShip.Physics.LinearVelocity.Length);
							aicontroller4.SetTargetState(in navalState2, false);
							return;
						}
						AIShipController aicontroller5 = this._ownerShip.AIController;
						matrixFrame = this._ownerShip.GlobalFrame;
						vec5 = matrixFrame.origin.AsVec2 + vec10 * 50f;
						navalState2 = new NavalState(in vec5, in vec10, this._ownerShip.Physics.LinearVelocity.Length);
						aicontroller5.SetTargetState(in navalState2, false);
						return;
					}
					else
					{
						float num4 = vec8.DotProduct(vec10);
						NavalState navalState2;
						if (MathF.Abs(num4) <= 0.8f)
						{
							AIShipController aicontroller6 = this._ownerShip.AIController;
							matrixFrame = this._ownerShip.GlobalFrame;
							vec5 = matrixFrame.origin.AsVec2 + vec10 * 20f;
							navalState2 = new NavalState(in vec5, in vec10, this._ownerShip.Physics.LinearVelocity.Length);
							aicontroller6.SetTargetState(in navalState2, false);
							return;
						}
						matrixFrame = this._ownerShip.GlobalFrame;
						vec5 = matrixFrame.rotation.f.AsVec2;
						vec5 = vec5.LeftVec();
						Vec2 vec14 = vec5.Normalized();
						AIShipController aicontroller7 = this._ownerShip.AIController;
						matrixFrame = this._ownerShip.GlobalFrame;
						vec5 = matrixFrame.origin.AsVec2 + vec10 * 20f + 6.66f * (((vec8.DotProduct(vec14) > 0f) ^ (num4 > 0f)) ? vec14 : (-vec14));
						navalState2 = new NavalState(in vec5, in vec10, this._ownerShip.Physics.LinearVelocity.Length);
						aicontroller7.SetTargetState(in navalState2, false);
						return;
					}
				}
				else
				{
					float num5 = vec8.DotProduct(vec10);
					NavalState navalState2;
					if (MathF.Abs(num5) <= 0.5f)
					{
						AIShipController aicontroller8 = this._ownerShip.AIController;
						matrixFrame = this._ownerShip.GlobalFrame;
						vec5 = matrixFrame.origin.AsVec2 + vec10 * 20f;
						navalState2 = new NavalState(in vec5, in vec10, this._ownerShip.Physics.LinearVelocity.Length);
						aicontroller8.SetTargetState(in navalState2, false);
						return;
					}
					matrixFrame = this._ownerShip.GlobalFrame;
					vec5 = matrixFrame.rotation.f.AsVec2;
					vec5 = vec5.LeftVec();
					Vec2 vec15 = vec5.Normalized();
					AIShipController aicontroller9 = this._ownerShip.AIController;
					matrixFrame = this._ownerShip.GlobalFrame;
					vec5 = matrixFrame.origin.AsVec2 + vec10 * 20f + 20f * (((vec8.DotProduct(vec15) > 0f) ^ (num5 > 0f)) ? vec15 : (-vec15));
					navalState2 = new NavalState(in vec5, in vec10, this._ownerShip.Physics.LinearVelocity.Length);
					aicontroller9.SetTargetState(in navalState2, false);
				}
			}
		}

		// Token: 0x0600096E RID: 2414 RVA: 0x0004246C File Offset: 0x0004066C
		private void UpdateEngageOrder(MissionShip shipToEngage)
		{
			MatrixFrame globalFrame = shipToEngage.GlobalFrame;
			bool flag = (this._ownerShip.GlobalFrame.origin - globalFrame.origin).AsVec2.DotProduct(globalFrame.rotation.f.AsVec2.RightVec()) > 0f;
			shipToEngage = shipToEngage.GetOutermostConnectedShipFromSide(flag, out flag, 0UL);
			Vec2 vec;
			vec..ctor(flag ? 12f : (-12f), 0f);
			float num = ((Vec2.DotProduct(this._ownerShip.GlobalFrame.rotation.f.AsVec2.Normalized(), globalFrame.rotation.f.AsVec2.Normalized()) >= 0f) ? 0f : 3.1415927f);
			this.SetMovementTargetShip(shipToEngage, in vec, num);
		}

		// Token: 0x0600096F RID: 2415 RVA: 0x0004255C File Offset: 0x0004075C
		private void SetStopShipAux()
		{
			Vec2 asVec = this._ownerShip.GlobalFrame.origin.AsVec2;
			this.SetTargetPosition(in asVec, false);
		}

		// Token: 0x06000970 RID: 2416 RVA: 0x0004258C File Offset: 0x0004078C
		private void SetTargetPosition(in Vec2 targetPosition, bool isForced = false)
		{
			this.TargetShip = null;
			this._offsetPosition = Vec2.Zero;
			this._offsetDirection = 0f;
			Vec2 vec = this._ownerShip.GlobalFrame.rotation.f.AsVec2.Normalized();
			this._orderGlobalPosition = targetPosition;
			this._orderGlobalDirection = vec;
			if (!this._lastCheckedOrderPosition.IsValid || this._orderGlobalPosition.DistanceSquared(this._lastCheckedOrderPosition) >= 4f)
			{
				this.ProjectOrderPositionToBoundaries(ref this._orderGlobalPosition);
				this._lastCheckedOrderPosition = this._orderGlobalPosition;
			}
			if (this._navalShipsLogic.IsTeleportingShips)
			{
				this.TryTeleportShipAux(in this._orderGlobalPosition, in this._orderGlobalDirection);
			}
			if (this.IsAIControllable || (this.HasAIController && isForced))
			{
				AIShipController aicontroller = this._ownerShip.AIController;
				NavalState navalState = new NavalState(in this._orderGlobalPosition, in this._orderGlobalDirection, 0f);
				aicontroller.SetTargetState(in navalState, true);
			}
		}

		// Token: 0x06000971 RID: 2417 RVA: 0x00042688 File Offset: 0x00040888
		private void SetTargetState(in Vec2 targetPosition, in Vec2 targetDirection)
		{
			this.TargetShip = null;
			this._offsetPosition = Vec2.Zero;
			this._offsetDirection = 0f;
			this._orderGlobalPosition = targetPosition;
			this._orderGlobalDirection = targetDirection;
			if (!this._lastCheckedOrderPosition.IsValid || this._orderGlobalPosition.DistanceSquared(this._lastCheckedOrderPosition) >= 4f)
			{
				this.ProjectOrderPositionToBoundaries(ref this._orderGlobalPosition);
				this._lastCheckedOrderPosition = this._orderGlobalPosition;
			}
			if (this._navalShipsLogic.IsTeleportingShips)
			{
				this.TryTeleportShipAux(in this._orderGlobalPosition, in this._orderGlobalDirection);
			}
			if (this.IsAIControllable)
			{
				this._ownerShip.AIController.SetTargetState(in this._orderGlobalPosition, in this._orderGlobalDirection, false);
			}
		}

		// Token: 0x06000972 RID: 2418 RVA: 0x0004274C File Offset: 0x0004094C
		private void SetMovementTargetShip(MissionShip targetShip, in Vec2 positionOffset, float directionOffset = 0f)
		{
			this.TargetShip = targetShip;
			this._offsetPosition = positionOffset;
			directionOffset = MBMath.WrapAngle(directionOffset);
			this._offsetDirection = directionOffset;
			MatrixFrame globalFrame = this.TargetShip.GlobalFrame;
			Vec2 vec = globalFrame.rotation.s.AsVec2.Normalized();
			Vec2 vec2 = globalFrame.rotation.f.AsVec2.Normalized();
			this._orderGlobalPosition = globalFrame.origin.AsVec2 + this._offsetPosition.X * vec + this._offsetPosition.Y * vec2;
			this._orderGlobalDirection = globalFrame.rotation.f.AsVec2;
			this._orderGlobalDirection.RotateCCW(this._offsetDirection);
			this._orderGlobalDirection.Normalize();
			if (this._navalShipsLogic.IsTeleportingShips)
			{
				Vec2 orderGlobalPosition = this._orderGlobalPosition;
				Vec2 vec3 = (Mission.Current.IsDeploymentFinished ? this._orderGlobalDirection : (this._orderGlobalPosition - this._ownerShip.GlobalFrame.origin.AsVec2).Normalized());
				if (!Mission.Current.IsDeploymentFinished)
				{
					this.ProjectOrderPositionToBoundaries(ref orderGlobalPosition);
				}
				this.TryTeleportShipAux(in orderGlobalPosition, in vec3);
			}
			if (this.IsAIControllable)
			{
				AIShipController aicontroller = this._ownerShip.AIController;
				MissionShip targetShip2 = this.TargetShip;
				NavalVec navalVec = new NavalVec(in this._offsetPosition, this._offsetDirection, 0f);
				aicontroller.SetTargetShipWithOffset(in targetShip2, in navalVec, false, false);
			}
		}

		// Token: 0x06000973 RID: 2419 RVA: 0x000428E0 File Offset: 0x00040AE0
		public void ManageShipDetachments()
		{
			if (this._ownerShip.IsShipNavmeshDisabled && this._ownerFormation.Detachments.Count > 0)
			{
				this.StopUsingMachines(false);
				return;
			}
			if (!this._ownerShip.IsShipNavmeshDisabled && this._ownerFormation.Detachments.Count == 0)
			{
				this.StartUsingMachines();
			}
			bool hasPlayerStandingPointEntity = this._ownerShip.HasPlayerStandingPointEntity;
			Agent main = Agent.Main;
			if (hasPlayerStandingPointEntity && main != null && this._ownerShip.IsPlayerShip)
			{
				if (main.IsUsingGameObject)
				{
					Agent.StopUsingGameObjectFlags stopUsingGameObjectFlags = 2;
					if (main.IsAIControlled)
					{
						stopUsingGameObjectFlags |= 1;
					}
					main.StopUsingGameObject(true, stopUsingGameObjectFlags);
				}
				else if (main.IsAIControlled)
				{
					main.TryAttachToFormation();
				}
			}
			foreach (ShipAttachmentMachine shipAttachmentMachine in this._ownerShip.AttachmentMachines)
			{
				if (this._boardingTargetShip != null && MissionShip.AreShipsConnected(this._ownerShip, this._boardingTargetShip) && shipAttachmentMachine.GetBestEnemyAttachment(false, true) == null)
				{
					if (shipAttachmentMachine.PilotAgent != null && shipAttachmentMachine.PilotAgent.IsAIControlled)
					{
						shipAttachmentMachine.PilotAgent.StopUsingGameObject(true, 3);
					}
					else
					{
						Agent movingAgent = shipAttachmentMachine.PilotStandingPoint.MovingAgent;
						if (movingAgent != null)
						{
							movingAgent.StopUsingGameObject(true, 3);
						}
					}
				}
			}
			Agent captain = this._ownerFormation.Captain;
			ShipControllerMachine shipControllerMachine = this._ownerShip.ShipControllerMachine;
			if ((shipControllerMachine.PilotAgent == null || shipControllerMachine.PilotAgent.IsAIControlled || this._navalShipsLogic.IsDeploymentMode) && captain != null && captain.IsAIControlled && (captain != Agent.Main || !hasPlayerStandingPointEntity) && (this._navalShipsLogic.IsDeploymentMode || ((captain.MovementMode & 3) == 1 && (!captain.IsDetachedFromFormation || !(captain.Detachment is ClimbingMachineDetachment)))))
			{
				if (captain.IsDetachedFromFormation && captain.CurrentlyUsedGameObject != shipControllerMachine.PilotStandingPoint && captain.HumanAIComponent.GetCurrentlyMovingGameObject() != shipControllerMachine.PilotStandingPoint)
				{
					if (captain.IsUsingGameObject)
					{
						captain.StopUsingGameObject(true, 3);
					}
					else
					{
						captain.TryAttachToFormation();
					}
				}
				if (shipControllerMachine.PilotAgent != null && shipControllerMachine.PilotAgent != captain)
				{
					shipControllerMachine.PilotAgent.StopUsingGameObject(true, 3);
				}
				if (captain.Detachment == null && !shipControllerMachine.IsDisabledForAI)
				{
					shipControllerMachine.AddAgentAtSlotIndex(captain, shipControllerMachine.PilotStandingPointSlotIndex);
				}
			}
			if (this._ownerFormation.CountOfDetachableNonPlayerUnits > 0)
			{
				this._ownerFormation.Arrangement.GetAllUnits(ref this._availableUnitList);
				int i = this._availableUnitList.Count - 1;
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				if (this._boardingTargetShip != null)
				{
					while (i >= 0)
					{
						if (num >= this._ownerShip.AttachmentMachines.Count)
						{
							break;
						}
						ShipAttachmentMachine shipAttachmentMachine2 = this._ownerShip.AttachmentMachines[num++];
						if (shipAttachmentMachine2.PilotAgent == null && !shipAttachmentMachine2.PilotStandingPoint.HasAIMovingTo && shipAttachmentMachine2.CurrentAttachment == null && !shipAttachmentMachine2.IsDisabledForBattleSideAI(this._ownerFormation.Team.Side) && shipAttachmentMachine2.CalculateCanConnectToTargetShip(this._boardingTargetShip) && (!MissionShip.AreShipsConnected(this._ownerShip, this._boardingTargetShip) || shipAttachmentMachine2.GetBestEnemyAttachment(false, true) != null))
						{
							Agent agent = this.GetNextAgent(ref i);
							if (agent == null)
							{
								break;
							}
							shipAttachmentMachine2.AddAgentAtSlotIndex(agent, shipAttachmentMachine2.PilotStandingPointSlotIndex);
						}
					}
				}
				else if (this._cutLooseOrderActive)
				{
					while (i >= 0)
					{
						if (num >= this._ownerShip.AttachmentMachines.Count)
						{
							break;
						}
						ShipAttachmentMachine shipAttachmentMachine3 = this._ownerShip.AttachmentMachines[num++];
						if (shipAttachmentMachine3.IsShipAttachmentMachineBridged() && !shipAttachmentMachine3.IsDisabledForBattleSideAI(this._ownerFormation.Team.Side) && shipAttachmentMachine3.PilotAgent == null && !shipAttachmentMachine3.PilotStandingPoint.HasAIMovingTo)
						{
							Agent agent = this.GetNextAgent(ref i);
							if (agent == null)
							{
								break;
							}
							shipAttachmentMachine3.AddAgentAtSlotIndex(agent, shipAttachmentMachine3.PilotStandingPointSlotIndex);
						}
					}
					while (i >= 0 && num2 < this._ownerShip.AttachmentPointMachines.Count)
					{
						ShipAttachmentPointMachine shipAttachmentPointMachine = this._ownerShip.AttachmentPointMachines[num2++];
						if (shipAttachmentPointMachine.IsShipAttachmentPointBridged() && !shipAttachmentPointMachine.IsDisabledForBattleSideAI(this._ownerFormation.Team.Side) && shipAttachmentPointMachine.PilotAgent == null && !shipAttachmentPointMachine.PilotStandingPoint.HasAIMovingTo)
						{
							Agent agent = this.GetNextAgent(ref i);
							if (agent == null)
							{
								break;
							}
							shipAttachmentPointMachine.AddAgentAtSlotIndex(agent, shipAttachmentPointMachine.PilotStandingPointSlotIndex);
						}
					}
				}
				if (this._ownerShip.ShipSiegeWeapon != null)
				{
					RangedSiegeWeapon shipSiegeWeapon = this._ownerShip.ShipSiegeWeapon;
					if (shipSiegeWeapon.PilotAgent == null && !shipSiegeWeapon.PilotStandingPoint.HasAIMovingTo && !shipSiegeWeapon.IsDisabledForBattleSideAI(this._ownerFormation.Team.Side))
					{
						Agent agent = this.GetNextAgent(ref i);
						if (agent != null)
						{
							shipSiegeWeapon.AddAgentAtSlotIndex(agent, shipSiegeWeapon.PilotStandingPointSlotIndex);
						}
					}
				}
				if (this._ownerShip.ShipControllerMachine.PilotAgent == null && !this._ownerShip.ShipControllerMachine.PilotStandingPoint.HasAIMovingTo && !this._ownerShip.ShipControllerMachine.IsDisabledForBattleSideAI(this._ownerFormation.Team.Side) && (!this._ownerShip.IsPlayerShip || Mission.Current.MainAgent == null))
				{
					Agent agent = this.GetNextAgent(ref i);
					if (agent != null)
					{
						this._ownerShip.ShipControllerMachine.AddAgentAtSlotIndex(agent, this._ownerShip.ShipControllerMachine.PilotStandingPointSlotIndex);
					}
				}
				while (i >= 0 && (num3 < this._ownerShip.LeftSideShipOarMachines.Count || num3 < this._ownerShip.RightSideShipOarMachines.Count))
				{
					if (num3 < this._ownerShip.LeftSideShipOarMachines.Count)
					{
						ShipOarMachine shipOarMachine = this._ownerShip.LeftSideShipOarMachines[num3];
						if (shipOarMachine.PilotAgent == null && !shipOarMachine.PilotStandingPoint.HasAIMovingTo && !shipOarMachine.PilotStandingPoint.IsDeactivated && !shipOarMachine.IsDisabledForBattleSideAI(this._ownerFormation.Team.Side))
						{
							Agent agent = this.GetNextAgent(ref i);
							if (agent == null)
							{
								break;
							}
							shipOarMachine.AddAgentAtSlotIndex(agent, shipOarMachine.PilotStandingPointSlotIndex);
						}
					}
					if (num3 < this._ownerShip.RightSideShipOarMachines.Count)
					{
						ShipOarMachine shipOarMachine2 = this._ownerShip.RightSideShipOarMachines[num3];
						if (shipOarMachine2.PilotAgent == null && !shipOarMachine2.PilotStandingPoint.HasAIMovingTo && !shipOarMachine2.PilotStandingPoint.IsDeactivated && !shipOarMachine2.IsDisabledForBattleSideAI(this._ownerFormation.Team.Side))
						{
							Agent agent = this.GetNextAgent(ref i);
							if (agent == null)
							{
								break;
							}
							shipOarMachine2.AddAgentAtSlotIndex(agent, shipOarMachine2.PilotStandingPointSlotIndex);
						}
					}
					num3++;
				}
				if (this._ownerShip.ShipPlacementDetachment != null && this._ownerShip.ShipPlacementDetachment.IsUsedByFormation(this._ownerFormation))
				{
					while (i >= 0)
					{
						if (!this._ownerShip.ShipPlacementDetachment.HasAvailableSlots)
						{
							return;
						}
						Agent agent = this.GetNextAgent(ref i);
						if (agent == null)
						{
							break;
						}
						if (this._navalShipsLogic.IsDeploymentMode || (agent.MovementMode & 3) == 1)
						{
							this._ownerShip.ShipPlacementDetachment.AddAgent(agent);
						}
					}
				}
			}
			else
			{
				ShipOrder.ShipDetachmentPriority shipDetachmentPriority = ShipOrder.ShipDetachmentPriority.ConnectionMachine;
				IDetachment detachment = null;
				bool flag = false;
				if (this._cutLooseOrderActive)
				{
					foreach (ShipAttachmentPointMachine shipAttachmentPointMachine2 in this._ownerShip.AttachmentPointMachines)
					{
						if (shipAttachmentPointMachine2.IsShipAttachmentPointBridged() && !shipAttachmentPointMachine2.IsDisabledForBattleSideAI(this._ownerFormation.Team.Side) && shipAttachmentPointMachine2.PilotAgent == null && !shipAttachmentPointMachine2.PilotStandingPoint.HasAIMovingTo)
						{
							detachment = shipAttachmentPointMachine2;
							break;
						}
					}
					if (detachment != null)
					{
						goto IL_0AB2;
					}
				}
				if (this._cutLooseOrderActive || this._boardingTargetShip != null)
				{
					foreach (ShipAttachmentMachine shipAttachmentMachine4 in this._ownerShip.AttachmentMachines)
					{
						if (((this._cutLooseOrderActive && shipAttachmentMachine4.IsShipAttachmentMachineBridged()) || (this._boardingTargetShip != null && shipAttachmentMachine4.CurrentAttachment == null && shipAttachmentMachine4.CalculateCanConnectToTargetShip(this._boardingTargetShip) && (!MissionShip.AreShipsConnected(this._ownerShip, this._boardingTargetShip) || shipAttachmentMachine4.GetBestEnemyAttachment(false, true) != null))) && !shipAttachmentMachine4.IsDisabledForBattleSideAI(this._ownerFormation.Team.Side) && shipAttachmentMachine4.PilotAgent == null && !shipAttachmentMachine4.PilotStandingPoint.HasAIMovingTo)
						{
							detachment = shipAttachmentMachine4;
							break;
						}
					}
					if (detachment != null)
					{
						goto IL_0AB2;
					}
				}
				shipDetachmentPriority--;
				if (this._ownerShip.ShipSiegeWeapon != null)
				{
					RangedSiegeWeapon shipSiegeWeapon2 = this._ownerShip.ShipSiegeWeapon;
					if (shipSiegeWeapon2.PilotAgent == null && !shipSiegeWeapon2.PilotStandingPoint.HasAIMovingTo && !shipSiegeWeapon2.IsDisabledForBattleSideAI(this._ownerFormation.Team.Side))
					{
						detachment = shipSiegeWeapon2;
						goto IL_0AB2;
					}
				}
				if (detachment == null)
				{
					shipDetachmentPriority--;
					if (this._ownerShip.ShipControllerMachine.PilotAgent == null && !this._ownerShip.ShipControllerMachine.PilotStandingPoint.HasAIMovingTo && !this._ownerShip.ShipControllerMachine.IsDisabledForBattleSideAI(this._ownerFormation.Team.Side) && (!this._ownerShip.IsPlayerShip || Mission.Current.MainAgent == null))
					{
						detachment = this._ownerShip.ShipControllerMachine;
					}
					else
					{
						shipDetachmentPriority--;
						int num4 = 0;
						while (num4 < this._ownerShip.LeftSideShipOarMachines.Count || num4 < this._ownerShip.RightSideShipOarMachines.Count)
						{
							if (num4 < this._ownerShip.LeftSideShipOarMachines.Count)
							{
								ShipOarMachine shipOarMachine3 = this._ownerShip.LeftSideShipOarMachines[num4];
								if (shipOarMachine3.PilotAgent == null && !shipOarMachine3.PilotStandingPoint.HasAIMovingTo && !shipOarMachine3.IsDisabledForBattleSideAI(this._ownerFormation.Team.Side))
								{
									detachment = shipOarMachine3;
									break;
								}
							}
							if (num4 < this._ownerShip.RightSideShipOarMachines.Count)
							{
								ShipOarMachine shipOarMachine4 = this._ownerShip.RightSideShipOarMachines[num4];
								if (shipOarMachine4.PilotAgent == null && !shipOarMachine4.PilotStandingPoint.HasAIMovingTo && !shipOarMachine4.IsDisabledForBattleSideAI(this._ownerFormation.Team.Side))
								{
									detachment = shipOarMachine4;
									break;
								}
							}
							num4++;
						}
						if (detachment == null)
						{
							shipDetachmentPriority--;
						}
					}
				}
				IL_0AB2:
				if (shipDetachmentPriority > ShipOrder.ShipDetachmentPriority.PlacementDetachment)
				{
					UsableMachine usableMachine;
					int num5 = (((usableMachine = detachment as UsableMachine) != null) ? usableMachine.PilotStandingPointSlotIndex : 0);
					if (this._ownerShip.ShipPlacementDetachment.HasAgent)
					{
						Agent agent2 = this._ownerShip.ShipPlacementDetachment.PickLastAgent();
						detachment.AddAgentAtSlotIndex(agent2, num5);
						return;
					}
					if (shipDetachmentPriority > ShipOrder.ShipDetachmentPriority.Oar)
					{
						int num6 = 0;
						while (num6 < this._ownerShip.LeftSideShipOarMachines.Count || num6 < this._ownerShip.RightSideShipOarMachines.Count)
						{
							if (num6 < this._ownerShip.LeftSideShipOarMachines.Count)
							{
								ShipOarMachine shipOarMachine5 = this._ownerShip.LeftSideShipOarMachines[num6];
								if (shipOarMachine5.PilotAgent != null && shipOarMachine5.PilotAgent.IsAIControlled)
								{
									Agent pilotAgent = shipOarMachine5.PilotAgent;
									pilotAgent.StopUsingGameObject(true, 3);
									detachment.AddAgentAtSlotIndex(pilotAgent, num5);
									flag = true;
									break;
								}
							}
							if (num6 < this._ownerShip.RightSideShipOarMachines.Count)
							{
								ShipOarMachine shipOarMachine6 = this._ownerShip.RightSideShipOarMachines[num6];
								if (shipOarMachine6.PilotAgent != null && shipOarMachine6.PilotAgent.IsAIControlled)
								{
									Agent pilotAgent2 = shipOarMachine6.PilotAgent;
									pilotAgent2.StopUsingGameObject(true, 3);
									detachment.AddAgentAtSlotIndex(pilotAgent2, num5);
									flag = true;
									break;
								}
							}
							num6++;
						}
						if (flag)
						{
							return;
						}
						if (shipDetachmentPriority > ShipOrder.ShipDetachmentPriority.ControllerMachine && this._ownerShip.ShipControllerMachine.PilotAgent != null && this._ownerShip.ShipControllerMachine.PilotAgent.IsAIControlled)
						{
							Agent pilotAgent3 = this._ownerShip.ShipControllerMachine.PilotAgent;
							pilotAgent3.StopUsingGameObject(true, 3);
							detachment.AddAgentAtSlotIndex(pilotAgent3, num5);
							return;
						}
						if (shipDetachmentPriority > ShipOrder.ShipDetachmentPriority.SiegeWeapon)
						{
							RangedSiegeWeapon shipSiegeWeapon3 = this._ownerShip.ShipSiegeWeapon;
							if (((shipSiegeWeapon3 != null) ? shipSiegeWeapon3.PilotAgent : null) != null && shipSiegeWeapon3.PilotAgent.IsAIControlled)
							{
								Agent pilotAgent4 = shipSiegeWeapon3.PilotAgent;
								pilotAgent4.StopUsingGameObject(true, 3);
								detachment.AddAgentAtSlotIndex(pilotAgent4, num5);
								return;
							}
						}
					}
					this.TickDetachmentsNeeded = false;
					this._detachmentTickTimer.Reset(Mission.Current.CurrentTime);
					return;
				}
				else
				{
					this.TickDetachmentsNeeded = false;
					this._detachmentTickTimer.Reset(Mission.Current.CurrentTime);
				}
			}
		}

		// Token: 0x06000974 RID: 2420 RVA: 0x00043604 File Offset: 0x00041804
		private void TryTeleportShipAux(in Vec2 position, in Vec2 direction)
		{
			MatrixFrame globalFrame = this._ownerShip.GlobalFrame;
			Vec2 vec = position;
			if (vec.DistanceSquared(globalFrame.origin.AsVec2) < 0.01f)
			{
				vec = direction;
				if (vec.AngleBetween(globalFrame.rotation.f.AsVec2.Normalized()) < 0.1f)
				{
					return;
				}
			}
			Vec2 vec2 = position;
			vec = direction;
			Vec2 vec3 = (vec.IsValid ? direction : this._ownerShip.GameEntity.GetGlobalFrame().rotation.f.AsVec2.Normalized());
			Vec3 vec4 = vec2.ToVec3(0f);
			Vec3 vec5 = vec3.ToVec3(0f).NormalizedCopy();
			MatrixFrame identity = MatrixFrame.Identity;
			identity.rotation.f = vec5;
			identity.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			identity.origin = vec4;
			bool flag = this._ownerShip.Physics != null && this._ownerShip.Physics.IsAnchored;
			this._navalShipsLogic.TeleportShip(this._ownerShip, identity, true, flag, true);
		}

		// Token: 0x06000975 RID: 2421 RVA: 0x00043747 File Offset: 0x00041947
		private void SetChargeOrder(bool applyToPlayerFormation)
		{
			if (applyToPlayerFormation || this._ownerFormation.PlayerOwner != Mission.Current.MainAgent || !this._ownerFormation.HasPlayerControlledTroop)
			{
				this._ownerFormation.SetMovementOrder(MovementOrder.MovementOrderCharge);
			}
		}

		// Token: 0x06000976 RID: 2422 RVA: 0x00043780 File Offset: 0x00041980
		public void JoinPlayerFormationToPlacementDetachment(bool isPlayersOrder)
		{
			if (!this._ownerShip.ShipPlacementDetachment.IsUsedByFormation(this._ownerFormation) && !this._ownerShip.IsShipNavmeshDisabled)
			{
				this._ownerFormation.JoinDetachment(this._ownerShip.ShipPlacementDetachment);
			}
			if (isPlayersOrder)
			{
				this._isChargeOrderOverridden = false;
			}
		}

		// Token: 0x06000977 RID: 2423 RVA: 0x000437D2 File Offset: 0x000419D2
		internal void RefreshOrders()
		{
			if (!this.HasAIController)
			{
				this.SetShipStopOrder();
				return;
			}
			if (this.HasStaticOrder)
			{
				this.UpdateStaticMovementOrder();
				return;
			}
			this._orderTimer.Reset();
			this.UpdateDynamicMovementOrder();
		}

		// Token: 0x06000978 RID: 2424 RVA: 0x00043803 File Offset: 0x00041A03
		internal void OnOwnerShipRemoved()
		{
			this._navalShipsLogic.ShipControllerChanged -= this.OnShipControllerChanged;
			this._navalShipsLogic.ShipRemovedEvent -= this.OnShipRemoved;
		}

		// Token: 0x06000979 RID: 2425 RVA: 0x00043834 File Offset: 0x00041A34
		private void CheckAndChangeIndependenceState()
		{
			MissionShip boardingTargetShip = this._boardingTargetShip;
			bool flag = boardingTargetShip != null && boardingTargetShip.AnyActiveFormationTroopOnShip && MissionShip.AreShipsConnected(this._ownerShip, this._boardingTargetShip);
			bool flag2 = flag || this._isEnemyOnShip.Value;
			if (!flag2)
			{
				foreach (ShipAttachmentMachine shipAttachmentMachine in this._ownerShip.AttachmentMachines)
				{
					if (shipAttachmentMachine.IsShipAttachmentMachineBridged())
					{
						flag2 = true;
						flag = true;
						break;
					}
					if (!flag2 && ShipAttachmentMachine.DoesShipAttachmentMachineSatisfyOarsmenGetUpCondition(shipAttachmentMachine.CurrentAttachment))
					{
						flag2 = true;
					}
					if (shipAttachmentMachine.IsShipAttachmentMachineConnectedToEnemy())
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this._ownerShip.AttachmentPointMachines)
					{
						if (shipAttachmentPointMachine.IsShipAttachmentPointBridged())
						{
							flag2 = true;
							flag = true;
							break;
						}
						if (!flag2 && ShipAttachmentMachine.DoesShipAttachmentMachineSatisfyOarsmenGetUpCondition(shipAttachmentPointMachine.CurrentAttachment))
						{
							flag2 = true;
						}
						if (shipAttachmentPointMachine.IsShipAttachmentPointConnectedToEnemy())
						{
							flag = true;
							break;
						}
					}
				}
			}
			switch (this._shipIndependenceState)
			{
			case ShipOrder.ShipIndependenceState.Independent:
				if (flag || this._isEnemyOnShip.Value)
				{
					if (flag2)
					{
						this._oarLevelOverridden = true;
						this._originalOarsmenLevel = this.OarsmenLevel;
						this.SetOarsmenLevel(0);
					}
					this._ownerShip.ShipControllerMachine.SetIsDisabledForAI(true);
					Agent pilotAgent = this._ownerShip.ShipControllerMachine.PilotAgent;
					if (pilotAgent != null && this._navalShipsLogic.IsDeploymentMode)
					{
						pilotAgent.StopUsingGameObject(true, 1);
					}
					this._shipIndependenceState = ShipOrder.ShipIndependenceState.Connected;
				}
				if (this._isEnemyOnShip.Value)
				{
					foreach (ShipAttachmentMachine shipAttachmentMachine2 in this._ownerShip.ShipAttachmentMachines)
					{
						shipAttachmentMachine2.SetIsDisabledForAI(true);
						Agent pilotAgent2 = shipAttachmentMachine2.PilotAgent;
						if (pilotAgent2 != null && this._navalShipsLogic.IsDeploymentMode)
						{
							pilotAgent2.StopUsingGameObject(true, 1);
						}
					}
					foreach (ShipAttachmentPointMachine shipAttachmentPointMachine2 in this._ownerShip.AttachmentPointMachines)
					{
						shipAttachmentPointMachine2.SetIsDisabledForAI(true);
						Agent pilotAgent3 = shipAttachmentPointMachine2.PilotAgent;
						if (pilotAgent3 != null && this._navalShipsLogic.IsDeploymentMode)
						{
							pilotAgent3.StopUsingGameObject(true, 1);
						}
					}
					this._shipIndependenceState = ShipOrder.ShipIndependenceState.EnemyOnShip;
				}
				break;
			case ShipOrder.ShipIndependenceState.Connected:
				if (this._isEnemyOnShip.Value)
				{
					Agent agent;
					foreach (ShipAttachmentMachine shipAttachmentMachine3 in this._ownerShip.ShipAttachmentMachines)
					{
						shipAttachmentMachine3.SetIsDisabledForAI(true);
						agent = shipAttachmentMachine3.PilotAgent;
						if (agent != null && this._navalShipsLogic.IsDeploymentMode)
						{
							agent.StopUsingGameObject(true, 1);
						}
					}
					foreach (ShipAttachmentPointMachine shipAttachmentPointMachine3 in this._ownerShip.AttachmentPointMachines)
					{
						shipAttachmentPointMachine3.SetIsDisabledForAI(true);
						agent = shipAttachmentPointMachine3.PilotAgent;
						if (agent != null && this._navalShipsLogic.IsDeploymentMode)
						{
							agent.StopUsingGameObject(true, 1);
						}
					}
					this._ownerShip.ShipControllerMachine.SetIsDisabledForAI(true);
					agent = this._ownerShip.ShipControllerMachine.PilotAgent;
					if (agent != null && this._navalShipsLogic.IsDeploymentMode)
					{
						agent.StopUsingGameObject(true, 1);
					}
					this._shipIndependenceState = ShipOrder.ShipIndependenceState.EnemyOnShip;
					this.SetChargeOrder(false);
				}
				else if (!flag)
				{
					this._shipIndependenceState = ShipOrder.ShipIndependenceState.Independent;
					this.SetOarsmenLevel(this._originalOarsmenLevel);
					this._oarLevelOverridden = false;
					this._ownerShip.ShipControllerMachine.SetIsDisabledForAI(false);
				}
				else if (!this._oarLevelOverridden && flag2)
				{
					this._oarLevelOverridden = true;
					this._originalOarsmenLevel = this.OarsmenLevel;
					this.SetOarsmenLevel(0);
				}
				break;
			case ShipOrder.ShipIndependenceState.EnemyOnShip:
				if (!this._isEnemyOnShip.Value)
				{
					if (this._cutLooseOrderActive)
					{
						foreach (ShipAttachmentPointMachine shipAttachmentPointMachine4 in this._ownerShip.AttachmentPointMachines)
						{
							shipAttachmentPointMachine4.SetIsDisabledForAI(false);
						}
					}
					if (this._cutLooseOrderActive || this._boardingTargetShip != null)
					{
						foreach (ShipAttachmentMachine shipAttachmentMachine4 in this._ownerShip.ShipAttachmentMachines)
						{
							shipAttachmentMachine4.SetIsDisabledForAI(false);
						}
					}
					this._shipIndependenceState = ShipOrder.ShipIndependenceState.Connected;
					if (!flag)
					{
						this._shipIndependenceState = ShipOrder.ShipIndependenceState.Independent;
						this.SetOarsmenLevel(this._originalOarsmenLevel);
						this._oarLevelOverridden = false;
						this._ownerShip.ShipControllerMachine.SetIsDisabledForAI(false);
					}
				}
				break;
			}
			switch (this._shipIndependenceState)
			{
			case ShipOrder.ShipIndependenceState.Independent:
				if ((this._ownerFormation.IsAIControlled || this._ownerFormation.IsAIOwned || !this._ownerFormation.HasPlayerControlledTroop) && !this._ownerShip.ShipPlacementDetachment.IsUsedByFormation(this._ownerFormation) && !this._ownerShip.IsShipNavmeshDisabled)
				{
					this._ownerFormation.JoinDetachment(this._ownerShip.ShipPlacementDetachment);
				}
				this._ownerShip.SetPositioningOrdersToRallyPoint(false, false);
				return;
			case ShipOrder.ShipIndependenceState.Connected:
				if (this._ownerFormation.IsAIControlled)
				{
					if (this._boardingTargetShip != null && MissionShip.AreShipsConnected(this._boardingTargetShip, this._ownerShip) && this._boardingTargetShip.Formation != null && this._ownerShip.SearchShipConnection(null, true, true, true, true))
					{
						if ((this._ownerFormation.IsAIControlled || this._ownerFormation.IsAIOwned || !this._ownerFormation.HasPlayerControlledTroop) && this._ownerShip.ShipPlacementDetachment.IsUsedByFormation(this._ownerFormation))
						{
							this._ownerFormation.LeaveDetachment(this._ownerShip.ShipPlacementDetachment);
						}
						this.SetChargeOrder(false);
						return;
					}
					if ((this._ownerFormation.IsAIControlled || this._ownerFormation.IsAIOwned || !this._ownerFormation.HasPlayerControlledTroop) && !this._ownerShip.ShipPlacementDetachment.IsUsedByFormation(this._ownerFormation) && !this._ownerShip.IsShipNavmeshDisabled)
					{
						this._ownerFormation.JoinDetachment(this._ownerShip.ShipPlacementDetachment);
					}
					this._ownerShip.SetPositioningOrdersToRallyPoint(false, false);
					return;
				}
				else if (!this._ownerFormation.HasPlayerControlledTroop)
				{
					switch (this.MovementOrderEnum)
					{
					case ShipOrder.ShipMovementOrderEnum.Move:
					case ShipOrder.ShipMovementOrderEnum.Retreat:
					case ShipOrder.ShipMovementOrderEnum.StaticOrderCount:
					case ShipOrder.ShipMovementOrderEnum.Skirmish:
						if (this._boardingTargetShip == null || !MissionShip.AreShipsConnected(this._ownerShip, this._boardingTargetShip) || !this._boardingTargetShip.AnyActiveFormationTroopOnShip)
						{
							if (!this._ownerShip.ShipPlacementDetachment.IsUsedByFormation(this._ownerFormation) && !this._ownerShip.IsShipNavmeshDisabled)
							{
								this._ownerFormation.JoinDetachment(this._ownerShip.ShipPlacementDetachment);
							}
							this._ownerShip.SetPositioningOrdersToRallyPoint(false, false);
							return;
						}
						if (this._ownerShip.ShipPlacementDetachment.IsUsedByFormation(this._ownerFormation))
						{
							this._ownerFormation.LeaveDetachment(this._ownerShip.ShipPlacementDetachment);
						}
						this.SetChargeOrder(false);
						return;
					case ShipOrder.ShipMovementOrderEnum.Engage:
						if (!this._autoSelectTargetShip)
						{
							if (MissionShip.AreShipsConnected(this._ownerShip, this.TargetShip) && this._ownerShip.SearchShipConnection(null, true, true, true, true))
							{
								if (this._ownerShip.ShipPlacementDetachment.IsUsedByFormation(this._ownerFormation))
								{
									this._ownerFormation.LeaveDetachment(this._ownerShip.ShipPlacementDetachment);
								}
								this.SetChargeOrder(false);
								return;
							}
							if (!this._ownerShip.ShipPlacementDetachment.IsUsedByFormation(this._ownerFormation) && !this._ownerShip.IsShipNavmeshDisabled)
							{
								this._ownerFormation.JoinDetachment(this._ownerShip.ShipPlacementDetachment);
							}
							this._ownerShip.SetPositioningOrdersToRallyPoint(false, false);
							return;
						}
						else
						{
							if (this._ownerShip.SearchShipConnection(null, true, true, true, true))
							{
								if (this._ownerShip.ShipPlacementDetachment.IsUsedByFormation(this._ownerFormation))
								{
									this._ownerFormation.LeaveDetachment(this._ownerShip.ShipPlacementDetachment);
								}
								this.SetChargeOrder(false);
								return;
							}
							if (!this._ownerShip.ShipPlacementDetachment.IsUsedByFormation(this._ownerFormation) && !this._ownerShip.IsShipNavmeshDisabled)
							{
								this._ownerFormation.JoinDetachment(this._ownerShip.ShipPlacementDetachment);
							}
							this._ownerShip.SetPositioningOrdersToRallyPoint(false, false);
							return;
						}
						break;
					default:
						return;
					}
				}
				break;
			case ShipOrder.ShipIndependenceState.EnemyOnShip:
				if ((this._ownerFormation.IsAIControlled || this._ownerFormation.IsAIOwned || !this._ownerFormation.HasPlayerControlledTroop) && this._ownerShip.ShipPlacementDetachment.IsUsedByFormation(this._ownerFormation))
				{
					this._ownerFormation.LeaveDetachment(this._ownerShip.ShipPlacementDetachment);
				}
				this.SetChargeOrder(false);
				break;
			default:
				return;
			}
		}

		// Token: 0x0600097A RID: 2426 RVA: 0x0004417C File Offset: 0x0004237C
		private void OnShipControllerChanged(MissionShip ship)
		{
			if (this._ownerShip == ship)
			{
				this.RefreshOrders();
			}
		}

		// Token: 0x0600097B RID: 2427 RVA: 0x0004418D File Offset: 0x0004238D
		private void OnShipRemoved(MissionShip ship)
		{
			if (ship != this._ownerShip && this.TargetShip == ship)
			{
				this.TargetShip = null;
			}
			if (this._boardingTargetShip == ship)
			{
				this._boardingTargetShip = null;
			}
		}

		// Token: 0x0400056D RID: 1389
		private const float BoardingDistance = 12f;

		// Token: 0x0400056E RID: 1390
		private const float SkirmishDistance = 60f;

		// Token: 0x0400056F RID: 1391
		private const float TimerDuration = 1f;

		// Token: 0x04000570 RID: 1392
		private const float TargetCorrectionCheckDistance = 2f;

		// Token: 0x04000571 RID: 1393
		private readonly QueryData<bool> _isEnemyOnShip;

		// Token: 0x04000572 RID: 1394
		private readonly QueryData<MissionShip> _closestEnemyShip;

		// Token: 0x04000573 RID: 1395
		private readonly MissionShip _ownerShip;

		// Token: 0x04000574 RID: 1396
		private Vec2 _orderGlobalPosition = Vec2.Invalid;

		// Token: 0x04000575 RID: 1397
		private Vec2 _orderGlobalDirection = Vec2.Forward;

		// Token: 0x04000576 RID: 1398
		private bool _inSkirmishPosition;

		// Token: 0x04000577 RID: 1399
		private MissionShip _targetShip;

		// Token: 0x04000578 RID: 1400
		private MissionShip _engageGivenTargetOrder;

		// Token: 0x0400057A RID: 1402
		private float _offsetDirection;

		// Token: 0x0400057B RID: 1403
		private bool _autoSelectTargetShip;

		// Token: 0x0400057C RID: 1404
		private Vec2 _offsetPosition = Vec2.Zero;

		// Token: 0x0400057D RID: 1405
		private Formation _ownerFormation;

		// Token: 0x0400057F RID: 1407
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x04000580 RID: 1408
		private bool _cutLooseOrderActive;

		// Token: 0x04000581 RID: 1409
		private MissionShip _boardingTargetShip;

		// Token: 0x04000582 RID: 1410
		private ShipOrder.ShipIndependenceState _shipIndependenceState;

		// Token: 0x04000583 RID: 1411
		private RandomTimer _detachmentTickTimer;

		// Token: 0x04000585 RID: 1413
		private bool _oarLevelOverridden;

		// Token: 0x04000586 RID: 1414
		private int _originalOarsmenLevel = 2;

		// Token: 0x04000587 RID: 1415
		private bool _isChargeOrderOverridden;

		// Token: 0x04000588 RID: 1416
		private MBList<IFormationUnit> _availableUnitList;

		// Token: 0x04000589 RID: 1417
		private Vec2 _lastCheckedOrderPosition = Vec2.Invalid;

		// Token: 0x0400058A RID: 1418
		private int _enforceSailUsage;

		// Token: 0x0400058C RID: 1420
		private MissionTimer _orderTimer;

		// Token: 0x0400058D RID: 1421
		private MissionTimer _placementDetachmentTimer;

		// Token: 0x020001F8 RID: 504
		public enum ShipMovementOrderEnum
		{
			// Token: 0x04000E59 RID: 3673
			Stop,
			// Token: 0x04000E5A RID: 3674
			Move,
			// Token: 0x04000E5B RID: 3675
			Retreat,
			// Token: 0x04000E5C RID: 3676
			StaticOrderCount,
			// Token: 0x04000E5D RID: 3677
			Follow = 3,
			// Token: 0x04000E5E RID: 3678
			Engage,
			// Token: 0x04000E5F RID: 3679
			Skirmish
		}

		// Token: 0x020001F9 RID: 505
		private enum ShipIndependenceState
		{
			// Token: 0x04000E61 RID: 3681
			Independent,
			// Token: 0x04000E62 RID: 3682
			Connected,
			// Token: 0x04000E63 RID: 3683
			EnemyOnShip
		}

		// Token: 0x020001FA RID: 506
		private enum ShipDetachmentPriority
		{
			// Token: 0x04000E65 RID: 3685
			PlacementDetachment = 1,
			// Token: 0x04000E66 RID: 3686
			Oar,
			// Token: 0x04000E67 RID: 3687
			ControllerMachine,
			// Token: 0x04000E68 RID: 3688
			SiegeWeapon,
			// Token: 0x04000E69 RID: 3689
			ConnectionMachine
		}
	}
}
