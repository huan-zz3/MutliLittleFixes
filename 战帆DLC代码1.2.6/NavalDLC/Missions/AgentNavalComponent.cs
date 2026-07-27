using System;
using NavalDLC.Missions.AI.TeamAI;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.NavalPhysics;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.Objects.UsableMachines;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions
{
	// Token: 0x02000080 RID: 128
	public class AgentNavalComponent : AgentComponent
	{
		// Token: 0x1700017C RID: 380
		// (get) Token: 0x0600090F RID: 2319 RVA: 0x0003F7FF File Offset: 0x0003D9FF
		// (set) Token: 0x06000910 RID: 2320 RVA: 0x0003F807 File Offset: 0x0003DA07
		public MissionShip SteppedShip { get; private set; }

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x06000911 RID: 2321 RVA: 0x0003F810 File Offset: 0x0003DA10
		// (set) Token: 0x06000912 RID: 2322 RVA: 0x0003F818 File Offset: 0x0003DA18
		public MissionShip FormationShip { get; private set; }

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x06000913 RID: 2323 RVA: 0x0003F821 File Offset: 0x0003DA21
		// (set) Token: 0x06000914 RID: 2324 RVA: 0x0003F829 File Offset: 0x0003DA29
		public bool BlockDrowning { get; private set; }

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x06000915 RID: 2325 RVA: 0x0003F832 File Offset: 0x0003DA32
		// (set) Token: 0x06000916 RID: 2326 RVA: 0x0003F83A File Offset: 0x0003DA3A
		public bool BlockBurning { get; private set; }

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x06000917 RID: 2327 RVA: 0x0003F843 File Offset: 0x0003DA43
		// (set) Token: 0x06000918 RID: 2328 RVA: 0x0003F84B File Offset: 0x0003DA4B
		public bool BlockCheckingOffShipConsideration { get; private set; }

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x06000919 RID: 2329 RVA: 0x0003F854 File Offset: 0x0003DA54
		// (set) Token: 0x0600091A RID: 2330 RVA: 0x0003F85C File Offset: 0x0003DA5C
		public bool BlockFormationCleanupOnShipAdabandonment { get; private set; }

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x0600091B RID: 2331 RVA: 0x0003F865 File Offset: 0x0003DA65
		public bool IsJumpingOffOnCooldown
		{
			get
			{
				return this._lastJumpOffTime >= 0f;
			}
		}

		// Token: 0x0600091C RID: 2332 RVA: 0x0003F878 File Offset: 0x0003DA78
		public AgentNavalComponent(Agent agent)
			: base(agent)
		{
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this._navalAgentsLogic = Mission.Current.GetMissionBehavior<NavalAgentsLogic>();
			this._lastMovementMode = 0;
			this._lastBreatheTime = agent.Mission.CurrentTime;
		}

		// Token: 0x0600091D RID: 2333 RVA: 0x0003F8D0 File Offset: 0x0003DAD0
		public override void Initialize()
		{
			this._breatheHoldMaxDurationFinal = MissionGameModels.Current.AgentStatCalculateModel.GetBreatheHoldMaxDuration(this.Agent, 60f) * (MBRandom.RandomFloat + 0.5f);
			Mission.Current.DeploymentFinishedEvent += this.OnDeploymentFinished;
			this._lastBreatheCheckTime = this.Agent.Mission.CurrentTime + MBRandom.RandomFloat * 5f;
			this._lastOffShipCheckTime = this.Agent.Mission.CurrentTime + MBRandom.RandomFloat * 5f;
			this._lastBurnCheckTime = this.Agent.Mission.CurrentTime + MBRandom.RandomFloat * 1f;
		}

		// Token: 0x0600091E RID: 2334 RVA: 0x0003F985 File Offset: 0x0003DB85
		public override void OnComponentRemoved()
		{
			Mission.Current.DeploymentFinishedEvent -= this.OnDeploymentFinished;
		}

		// Token: 0x0600091F RID: 2335 RVA: 0x0003F9A0 File Offset: 0x0003DBA0
		public override void OnFormationSet()
		{
			base.OnFormationSet();
			if (this.Agent.Formation == null)
			{
				this.FormationShip = null;
				this._parentShipUniqueBitwiseID = 0UL;
				return;
			}
			Team team = this.Agent.Formation.Team;
			this._teamAINavalComponent = ((team != null) ? team.TeamAI : null) as TeamAINavalComponent;
			MissionShip missionShip;
			if (this._navalShipsLogic.GetShip(this.Agent.Team.TeamSide, this.Agent.Formation.FormationIndex, out missionShip))
			{
				this.FormationShip = missionShip;
				this._parentShipUniqueBitwiseID = this.FormationShip.ShipUniqueBitwiseID;
				return;
			}
			this.FormationShip = null;
			this._parentShipUniqueBitwiseID = 0UL;
		}

		// Token: 0x06000920 RID: 2336 RVA: 0x0003FA50 File Offset: 0x0003DC50
		public void OnShipCaptured()
		{
			MissionShip missionShip;
			if (this._navalShipsLogic.GetShip(this.Agent.Team.TeamSide, this.Agent.Formation.FormationIndex, out missionShip))
			{
				this.FormationShip = missionShip;
				this._parentShipUniqueBitwiseID = this.FormationShip.ShipUniqueBitwiseID;
				return;
			}
			this.FormationShip = null;
			this._parentShipUniqueBitwiseID = 0UL;
		}

		// Token: 0x06000921 RID: 2337 RVA: 0x0003FAB4 File Offset: 0x0003DCB4
		public void SetCanDrown(bool canDrown)
		{
			this.BlockDrowning = !canDrown;
		}

		// Token: 0x06000922 RID: 2338 RVA: 0x0003FAC0 File Offset: 0x0003DCC0
		public void SetCanBurn(bool canBurn)
		{
			this.BlockBurning = !canBurn;
		}

		// Token: 0x06000923 RID: 2339 RVA: 0x0003FACC File Offset: 0x0003DCCC
		public void SetBlockOffShipConsideration(bool canCheckOffShipConsideration)
		{
			this.BlockCheckingOffShipConsideration = !canCheckOffShipConsideration;
		}

		// Token: 0x06000924 RID: 2340 RVA: 0x0003FAD8 File Offset: 0x0003DCD8
		public void SetBlockFormationCleanupOnShipAdabandonment(bool canCleanFormationOnShipAdabandonment)
		{
			this.BlockFormationCleanupOnShipAdabandonment = !canCleanFormationOnShipAdabandonment;
		}

		// Token: 0x06000925 RID: 2341 RVA: 0x0003FAE4 File Offset: 0x0003DCE4
		public float GetBreath()
		{
			float num = ((this.Agent.Controller != 1) ? (this._breatheHoldMaxDurationFinal * 2f) : this._breatheHoldMaxDurationFinal);
			return MBMath.ClampFloat((this._lastBreatheTime + 5f + num - this.Agent.Mission.CurrentTime) / num, 0f, 1f);
		}

		// Token: 0x06000926 RID: 2342 RVA: 0x0003FB44 File Offset: 0x0003DD44
		public ulong GetSteppedCombinedShipIsland()
		{
			if (this.SteppedShip != null)
			{
				return this.SteppedShip.ShipIslandCombinedID;
			}
			if (this._steppedPlankBridgeSteppedAgentManagerCached != null)
			{
				PlankBridgeSteppedAgentManager steppedPlankBridgeSteppedAgentManagerCached = this._steppedPlankBridgeSteppedAgentManagerCached;
				ShipAttachmentMachine.ShipAttachment shipAttachment;
				if (steppedPlankBridgeSteppedAgentManagerCached == null)
				{
					shipAttachment = null;
				}
				else
				{
					ShipAttachmentMachine.ShipBridgeNavmeshHolder navmeshHolder = steppedPlankBridgeSteppedAgentManagerCached.NavmeshHolder;
					shipAttachment = ((navmeshHolder != null) ? navmeshHolder.CurrentAttachment : null);
				}
				ShipAttachmentMachine.ShipAttachment shipAttachment2 = shipAttachment;
				if (shipAttachment2 != null)
				{
					return shipAttachment2.AttachmentSource.OwnerShip.ShipIslandCombinedID;
				}
			}
			return 0UL;
		}

		// Token: 0x06000927 RID: 2343 RVA: 0x0003FBA4 File Offset: 0x0003DDA4
		public override void OnTickParallel(float dt)
		{
			AgentMovementMode agentMovementMode = this.Agent.MovementMode & 3;
			if (agentMovementMode == 2 || agentMovementMode == 3)
			{
				if (this._lastMovementMode != 2 && this._lastMovementMode != 3 && this.Agent.IsHuman)
				{
					this.Agent.SaveEquipmentsOnHand();
					if (this.Agent.GetPrimaryWieldedItemIndex() != -1)
					{
						this.Agent.Mission.AddTickActionMT(0, this.Agent, 0, 1);
					}
					if (this.Agent.GetOffhandWieldedItemIndex() != -1)
					{
						this.Agent.Mission.AddTickActionMT(0, this.Agent, 1, 1);
					}
					if (Mission.Current.MissionResult != null)
					{
						this.Agent.SetAgentFlags(this.Agent.GetAgentFlags() & -17);
					}
					else
					{
						this.Agent.SetAgentFlags(this.Agent.GetAgentFlags() & -25);
					}
				}
			}
			else if (this._lastMovementMode != 1 && agentMovementMode == 1 && this.Agent.IsHuman)
			{
				if (Mission.Current.MissionResult != null)
				{
					this.Agent.SetAgentFlags(this.Agent.GetAgentFlags() | 16);
				}
				else
				{
					this.Agent.SetAgentFlags(this.Agent.GetAgentFlags() | 8 | 16);
				}
			}
			if (!this.IsJumpingOffOnCooldown)
			{
				WeakGameEntity steppedEntity = this.Agent.GetSteppedEntity();
				if (steppedEntity != this._steppedEntityCache)
				{
					this._steppedEntityCache = GameEntity.CreateFromWeakEntity(steppedEntity);
					WeakGameEntity root = steppedEntity.Root;
					this.SteppedShip = ((this._steppedEntityCache != null) ? (root.GetFirstScriptWithNameHash(MissionShip.MissionShipScriptNameHash) as MissionShip) : null);
					this._navalAgentsLogic.OnAgentSteppedShipChanged(this.Agent, this.SteppedShip);
					this._steppedNavalPhysicsCached = root.GetFirstScriptOfType<NavalPhysics>();
					this._steppedPlankBridgeSteppedAgentManagerCached = root.GetFirstScriptOfType<PlankBridgeSteppedAgentManager>();
				}
			}
			this._lastMovementMode = agentMovementMode;
		}

		// Token: 0x06000928 RID: 2344 RVA: 0x0003FD84 File Offset: 0x0003DF84
		public override void OnTick(float dt)
		{
			if (dt > 0f)
			{
				if (this._lastJumpOffTime >= 0f && Mission.Current.CurrentTime - this._lastJumpOffTime >= 2f)
				{
					this._lastJumpOffTime = ((this.Agent.IsOnLand() || this.Agent.IsInWater()) ? (-1f) : (Mission.Current.CurrentTime - 1.9f));
				}
				if (!this.BlockCheckingOffShipConsideration && this.Agent.IsAIControlled && this._lastOffShipCheckTime + 5f <= this.Agent.Mission.CurrentTime && this._navalAgentsLogic.IsDeploymentFinished)
				{
					this._lastOffShipCheckTime += 5f;
					this.CheckAgentOffShip();
				}
				if (!this.BlockDrowning && !this.Agent.Mission.MissionEnded && this._lastBreatheCheckTime + 5f <= this.Agent.Mission.CurrentTime && this._navalAgentsLogic.IsDeploymentFinished)
				{
					this._lastBreatheCheckTime += 5f;
					this.CheckAgentDrown();
				}
				if (!this.BlockBurning && !this.Agent.Mission.MissionEnded && this._lastBurnCheckTime + 1f <= this.Agent.Mission.CurrentTime && this._navalAgentsLogic.IsDeploymentFinished)
				{
					this._lastBurnCheckTime += 1f;
					this.CheckAgentBurn();
				}
				if (this.SteppedShip != null)
				{
					NavalPhysics steppedNavalPhysicsCached = this._steppedNavalPhysicsCached;
					if (steppedNavalPhysicsCached != null)
					{
						steppedNavalPhysicsCached.AddAgentWeightAndPositionInformation(this.Agent);
					}
					PlankBridgeSteppedAgentManager steppedPlankBridgeSteppedAgentManagerCached = this._steppedPlankBridgeSteppedAgentManagerCached;
					if (steppedPlankBridgeSteppedAgentManagerCached == null)
					{
						return;
					}
					steppedPlankBridgeSteppedAgentManagerCached.AddAgentWeightAndPositionInformation(this.Agent);
				}
			}
		}

		// Token: 0x06000929 RID: 2345 RVA: 0x0003FF40 File Offset: 0x0003E140
		private void CheckAgentOffShip()
		{
			if (this._lastMovementMode == 2 || this._lastMovementMode == 3)
			{
				if (this.FormationShip != null && (this.FormationShip.Physics.NavalSinkingState != NavalPhysics.SinkingState.Floating || this.FormationShip.BeingAbandoned))
				{
					TeamAINavalComponent teamAINavalComponent = this._teamAINavalComponent;
					Formation formation = ((teamAINavalComponent != null) ? teamAINavalComponent.GetNearestAllyShipFormation(this.Agent) : null);
					MissionShip missionShip;
					if (formation != null && this._navalShipsLogic.GetShip(formation, out missionShip) && missionShip != this.FormationShip)
					{
						this._navalAgentsLogic.TransferAgentToShip(this.Agent, missionShip);
						this.FormationShip = missionShip;
						this._parentShipUniqueBitwiseID = missionShip.ShipUniqueBitwiseID;
						return;
					}
					this._navalAgentsLogic.RemoveAgentFromShip(this.Agent, this.FormationShip);
					this.FormationShip = null;
					this._parentShipUniqueBitwiseID = 0UL;
					return;
				}
				else if (this.FormationShip == null)
				{
					TeamAINavalComponent teamAINavalComponent2 = this._teamAINavalComponent;
					Formation formation2 = ((teamAINavalComponent2 != null) ? teamAINavalComponent2.GetNearestAllyShipFormation(this.Agent) : null);
					MissionShip missionShip2;
					if (formation2 != null && this._navalShipsLogic.GetShip(formation2, out missionShip2))
					{
						if (this.Agent.IsUsingGameObject || AgentComponentExtensions.AIMoveToGameObjectIsEnabled(this.Agent))
						{
							this.Agent.StopUsingGameObjectMT(true, 1);
						}
						this._navalAgentsLogic.AddAgentToShip(this.Agent, missionShip2);
						this.FormationShip = missionShip2;
						this._parentShipUniqueBitwiseID = missionShip2.ShipUniqueBitwiseID;
						return;
					}
				}
			}
			else if (this.SteppedShip != null && !this.SteppedShip.BeingAbandoned && (this._parentShipUniqueBitwiseID & this.SteppedShip.ShipIslandCombinedID) == 0UL)
			{
				TeamAINavalComponent teamAINavalComponent3 = this._teamAINavalComponent;
				Formation formation3 = ((teamAINavalComponent3 != null) ? teamAINavalComponent3.GetConnectedAllyFormation(this.SteppedShip.ShipUniqueBitwiseID) : null);
				MissionShip missionShip3;
				if (formation3 != null && this._navalShipsLogic.GetShip(formation3, out missionShip3) && this._parentShipUniqueBitwiseID != missionShip3.ShipUniqueBitwiseID)
				{
					if (this.FormationShip != null)
					{
						this._navalAgentsLogic.TransferAgentToShip(this.Agent, missionShip3);
					}
					else
					{
						if (this.Agent.IsUsingGameObject || AgentComponentExtensions.AIMoveToGameObjectIsEnabled(this.Agent))
						{
							this.Agent.StopUsingGameObject(true, 1);
						}
						this._navalAgentsLogic.AddAgentToShip(this.Agent, missionShip3);
					}
					this.FormationShip = missionShip3;
					this._parentShipUniqueBitwiseID = missionShip3.ShipUniqueBitwiseID;
					return;
				}
				if (this.FormationShip != null)
				{
					this._navalAgentsLogic.RemoveAgentFromShip(this.Agent, this.FormationShip);
					this.FormationShip = null;
					this._parentShipUniqueBitwiseID = 0UL;
				}
			}
		}

		// Token: 0x0600092A RID: 2346 RVA: 0x000401B0 File Offset: 0x0003E3B0
		private void CheckAgentDrown()
		{
			AgentMovementMode agentMovementMode = this.Agent.MovementMode & 3;
			if (agentMovementMode != 3 && (this.Agent.Controller != 1 || agentMovementMode != 2))
			{
				this._lastBreatheTime = this.Agent.Mission.CurrentTime;
			}
			if (this.GetBreath() <= 0f)
			{
				this.DrownAgent();
			}
		}

		// Token: 0x0600092B RID: 2347 RVA: 0x0004020C File Offset: 0x0003E40C
		private void CheckAgentBurn()
		{
			if (this.Agent.IsMainAgent && this.SteppedShip != null && this.SteppedShip.FireHitPoints <= 0f)
			{
				this.Agent.Mission.AddTickAction(5, this.Agent, 0, 0);
			}
		}

		// Token: 0x0600092C RID: 2348 RVA: 0x00040259 File Offset: 0x0003E459
		public void DrownAgent()
		{
			this.Agent.Mission.AddTickAction(4, this.Agent, 0, 0);
		}

		// Token: 0x0600092D RID: 2349 RVA: 0x00040274 File Offset: 0x0003E474
		public void SetupAgentToAbandonShip()
		{
			if (!this.BlockFormationCleanupOnShipAdabandonment && this.FormationShip != null)
			{
				this._navalAgentsLogic.RemoveAgentFromShip(this.Agent, this.FormationShip);
			}
			this.SteppedShip = null;
			this._lastJumpOffTime = Mission.Current.CurrentTime;
		}

		// Token: 0x0600092E RID: 2350 RVA: 0x000402B4 File Offset: 0x0003E4B4
		private void OnDeploymentFinished()
		{
			this._lastBreatheCheckTime = this.Agent.Mission.CurrentTime + MBRandom.RandomFloat * 5f;
			this._lastOffShipCheckTime = this.Agent.Mission.CurrentTime + MBRandom.RandomFloat * 5f;
			this._lastBurnCheckTime = this.Agent.Mission.CurrentTime + MBRandom.RandomFloat * 1f;
		}

		// Token: 0x04000554 RID: 1364
		private const float OffShipCheckInterval = 5f;

		// Token: 0x04000555 RID: 1365
		private const float BreatheCheckInterval = 5f;

		// Token: 0x04000556 RID: 1366
		private const float BurnCheckInterval = 1f;

		// Token: 0x04000557 RID: 1367
		private const float BreatheHoldMaxDurationBase = 60f;

		// Token: 0x04000558 RID: 1368
		private const float JumpOffTimerInSeconds = 2f;

		// Token: 0x0400055A RID: 1370
		private AgentMovementMode _lastMovementMode;

		// Token: 0x0400055B RID: 1371
		private float _lastBreatheTime;

		// Token: 0x0400055C RID: 1372
		private float _lastBreatheCheckTime;

		// Token: 0x0400055D RID: 1373
		private float _lastBurnCheckTime;

		// Token: 0x0400055E RID: 1374
		private float _lastOffShipCheckTime;

		// Token: 0x0400055F RID: 1375
		private float _breatheHoldMaxDurationFinal;

		// Token: 0x04000560 RID: 1376
		private ulong _parentShipUniqueBitwiseID;

		// Token: 0x04000562 RID: 1378
		private TeamAINavalComponent _teamAINavalComponent;

		// Token: 0x04000563 RID: 1379
		private GameEntity _steppedEntityCache;

		// Token: 0x04000564 RID: 1380
		private NavalPhysics _steppedNavalPhysicsCached;

		// Token: 0x04000565 RID: 1381
		private PlankBridgeSteppedAgentManager _steppedPlankBridgeSteppedAgentManagerCached;

		// Token: 0x0400056A RID: 1386
		private readonly NavalShipsLogic _navalShipsLogic;

		// Token: 0x0400056B RID: 1387
		private readonly NavalAgentsLogic _navalAgentsLogic;

		// Token: 0x0400056C RID: 1388
		private float _lastJumpOffTime = -1f;
	}
}
