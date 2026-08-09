using System;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects.UsableMachines;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions
{
	// Token: 0x0200007F RID: 127
	public class AgentNavalAIComponent : AgentComponent
	{
		// Token: 0x060008FF RID: 2303 RVA: 0x0003EF04 File Offset: 0x0003D104
		public AgentNavalAIComponent(Agent agent)
			: base(agent)
		{
			this._tauntTimer = 0f;
			this._barkTimer = 0f;
			this._checkBridgesAndTargetingAgentTimer = 0f;
			this._tauntDelay = 0f;
			this._barkDelay = 0f;
			this._tauntDelayTimer = 0f;
			this._barkDelayTimer = 0f;
			this._tauntFired = false;
			this._barkFired = false;
			this._isConnectedToEnemyWithoutBridges = false;
			this._currentActionIndexCache = ActionIndexCache.act_none;
			this._agentNavalComponent = this.Agent.GetComponent<AgentNavalComponent>();
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
		}

		// Token: 0x06000900 RID: 2304 RVA: 0x0003EFBD File Offset: 0x0003D1BD
		public bool UnderMeleeAttack(float timeLimit = 1f)
		{
			return MBCommon.GetTotalMissionTime() - this.Agent.LastMeleeHitTime < timeLimit;
		}

		// Token: 0x06000901 RID: 2305 RVA: 0x0003EFD3 File Offset: 0x0003D1D3
		public bool UnderRangedAttack(float timeLimit = 1f)
		{
			return MBCommon.GetTotalMissionTime() - this.Agent.LastMeleeHitTime < timeLimit;
		}

		// Token: 0x06000902 RID: 2306 RVA: 0x0003EFE9 File Offset: 0x0003D1E9
		public bool RangeAttacking(float timeLimit = 1f)
		{
			return MBCommon.GetTotalMissionTime() - this.Agent.LastRangedAttackTime < timeLimit;
		}

		// Token: 0x06000903 RID: 2307 RVA: 0x0003EFFF File Offset: 0x0003D1FF
		public bool MeleeAttacking(float timeLimit = 1f)
		{
			return MBCommon.GetTotalMissionTime() - this.Agent.LastMeleeHitTime < timeLimit;
		}

		// Token: 0x06000904 RID: 2308 RVA: 0x0003F018 File Offset: 0x0003D218
		private bool DecideBoardingTaunts()
		{
			bool flag = false;
			float morale = AgentComponentExtensions.GetMorale(this.Agent);
			if (!this.Agent.IsUsingGameObject && morale > 70f && this._agentNavalComponent.SteppedShip != null)
			{
				float randomFloat = MBRandom.RandomFloat;
				if (this._isConnectedToEnemyWithoutBridges)
				{
					if (randomFloat < 0.33f)
					{
						this.TryToTriggerTaunt(AgentNavalAIComponent.AgentNavalTaunts.Invite, 0.1f + MBRandom.RandomFloat * 1.5f, 0.1f, false);
					}
					else if (randomFloat < 0.66f)
					{
						this.TryToTriggerTaunt(AgentNavalAIComponent.AgentNavalTaunts.Invite2, 0.1f + MBRandom.RandomFloat * 1.5f, 0.1f, false);
					}
					else
					{
						this.TryToTriggerTaunt(AgentNavalAIComponent.AgentNavalTaunts.Point, 0.1f + MBRandom.RandomFloat * 1.5f, 0.1f, false);
					}
					flag = true;
				}
			}
			return flag;
		}

		// Token: 0x06000905 RID: 2309 RVA: 0x0003F0DC File Offset: 0x0003D2DC
		private bool DecideTaunt()
		{
			bool flag = false;
			if (this.Agent.IsAIControlled)
			{
				flag = this.DecideBoardingTaunts();
			}
			return flag;
		}

		// Token: 0x06000906 RID: 2310 RVA: 0x0003F100 File Offset: 0x0003D300
		public override void OnTickParallel(float dt)
		{
			this._tauntTimer += dt;
			this._tauntDelayTimer += dt;
			if (this._tauntTimer >= this._tauntCooldown)
			{
				this.DecideTaunt();
				this._tauntTimer = 0f;
			}
			this.ExecuteTaunt();
		}

		// Token: 0x06000907 RID: 2311 RVA: 0x0003F150 File Offset: 0x0003D350
		public override void OnTick(float dt)
		{
			if (this._jumpOffDecisionType != AgentNavalAIComponent.AgentJumpOffDecisionType.None && this._agentNavalComponent.SteppedShip == null && (this.Agent.IsOnLand() || this.Agent.IsInWater()))
			{
				if (this.Agent.HumanAIComponent.GetCurrentlyMovingGameObject() != null)
				{
					AgentNavalAIComponent.AgentJumpOffDecisionType jumpOffDecisionType = this._jumpOffDecisionType;
					if (jumpOffDecisionType != AgentNavalAIComponent.AgentJumpOffDecisionType.MovingWithoutDetachment)
					{
						if (jumpOffDecisionType != AgentNavalAIComponent.AgentJumpOffDecisionType.MovingWithDetachment)
						{
							Debug.FailedAssert("Invalid AgentJumpOffDecisionType state while moving to the machine.", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Missions\\AgentNavalAIComponent.cs", "OnTick", 182);
						}
						else if (this.Agent.Detachment != null)
						{
							this.Agent.TryAttachToFormation();
						}
					}
					else
					{
						AgentComponentExtensions.AIMoveToGameObjectDisable(this.Agent);
					}
				}
				this._jumpOffDecisionType = AgentNavalAIComponent.AgentJumpOffDecisionType.None;
			}
			if (this.Agent.IsAIControlled && !this._agentNavalComponent.IsJumpingOffOnCooldown && this._agentNavalComponent.SteppedShip != null && this._agentNavalComponent.SteppedShip.BeingAbandoned && !this.Agent.IsUsingGameObject && this.Agent.HumanAIComponent.GetCurrentlyMovingGameObject() == null && this.Agent.IsOnLand())
			{
				foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this._agentNavalComponent.SteppedShip.AttachmentPointMachines)
				{
					if (!shipAttachmentPointMachine.IsDisabledForAI && !shipAttachmentPointMachine.PilotStandingPoint.HasAIMovingTo && shipAttachmentPointMachine.PilotAgent == null && shipAttachmentPointMachine.CurrentAttachment == null)
					{
						if (this.Agent.Formation == null)
						{
							AgentComponentExtensions.AIMoveToGameObjectEnable(this.Agent, shipAttachmentPointMachine.PilotStandingPoint, shipAttachmentPointMachine, 2);
							this._jumpOffDecisionType = AgentNavalAIComponent.AgentJumpOffDecisionType.MovingWithoutDetachment;
							break;
						}
						if (this.Agent.Formation == this._agentNavalComponent.SteppedShip.Formation)
						{
							if (this.Agent.Detachment != null)
							{
								this.Agent.TryAttachToFormation();
							}
							shipAttachmentPointMachine.AddAgentAtSlotIndex(this.Agent, 0);
							this._jumpOffDecisionType = AgentNavalAIComponent.AgentJumpOffDecisionType.MovingWithDetachment;
							break;
						}
						break;
					}
				}
			}
			if (this._shouldTrySwimmingToShore && this._agentNavalComponent.SteppedShip == null)
			{
				if (!Extensions.HasAnyFlag<Agent.AIScriptedFrameFlags>(this.Agent.GetScriptedFlags(), 1) && this.Agent.IsInWater())
				{
					WorldPosition worldPosition = ModuleExtensions.ToWorldPosition(this._targetFrameForSwimmingToShore.origin + this._targetFrameForSwimmingToShore.rotation.f * 1f);
					this.Agent.SetScriptedPosition(ref worldPosition, false, 0);
				}
				else if (Extensions.HasAnyFlag<Agent.AIScriptedFrameFlags>(this.Agent.GetScriptedFlags(), 1) && this.Agent.IsOnLand())
				{
					this.Agent.DisableScriptedMovement();
				}
			}
			this._barkTimer += dt;
			this._barkDelayTimer += dt;
			this._checkBridgesAndTargetingAgentTimer += dt;
			this.ExecuteBark();
			if (this._checkBridgesAndTargetingAgentTimer >= 3f)
			{
				this._isConnectedToEnemyWithoutBridges = this._agentNavalComponent.SteppedShip != null && this._agentNavalComponent.SteppedShip.GetIsConnectedToEnemyWithoutBridges();
				this._checkBridgesAndTargetingAgentTimer = 0f;
			}
		}

		// Token: 0x06000908 RID: 2312 RVA: 0x0003F47C File Offset: 0x0003D67C
		private void ExecuteTaunt()
		{
			if (this._tauntFired && this._tauntDelayTimer >= this._tauntDelay)
			{
				this.Agent.SetActionChannel(1, ref this._currentActionIndexCache, false, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
				this._tauntDelayTimer = 0f;
				this._tauntFired = false;
			}
		}

		// Token: 0x06000909 RID: 2313 RVA: 0x0003F4E8 File Offset: 0x0003D6E8
		private void ExecuteBark()
		{
			if (this._barkFired && this._barkDelayTimer >= this._barkDelay)
			{
				this.Agent.MakeVoice(this._currentVoiceType, 2);
				this._barkDelayTimer = 0f;
				this._barkFired = false;
			}
		}

		// Token: 0x0600090A RID: 2314 RVA: 0x0003F524 File Offset: 0x0003D724
		public void TryToTriggerTaunt(AgentNavalAIComponent.AgentNavalTaunts navalTaunt, float delay, float chanceToTrigger = 1f, bool makeTimerZeroIfSuccessful = false)
		{
			if (chanceToTrigger >= MBRandom.RandomFloat && !this.Agent.IsInBeingStruckAction && this.Agent.IsOnLand() && (makeTimerZeroIfSuccessful || (this._tauntTimer >= this._tauntCooldown && !this._tauntFired)) && !this.UnderMeleeAttack(1f) && !this.UnderRangedAttack(1f) && !this.RangeAttacking(1f) && !this.MeleeAttacking(1f))
			{
				this._currentActionIndexCache = this.SelectActionForTaunt(navalTaunt);
				this._tauntDelay = delay;
				if (makeTimerZeroIfSuccessful)
				{
					this.Agent.SetActionChannel(1, ref this._currentActionIndexCache, false, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
					this._tauntFired = false;
					return;
				}
				this._tauntDelayTimer = 0f;
				this._tauntFired = true;
			}
		}

		// Token: 0x0600090B RID: 2315 RVA: 0x0003F624 File Offset: 0x0003D824
		public void TryToTriggerBark(SkinVoiceManager.SkinVoiceType voiceType, float delay, float chanceToTrigger = 1f, bool makeTimerZeroIfSuccessful = false)
		{
			if (this._barkTimer >= 1.5f && chanceToTrigger >= MBRandom.RandomFloat && (Mission.Current.MainAgent == null || Mission.Current.MainAgent.Position.DistanceSquared(this.Agent.Position) < 625f))
			{
				this._barkTimer = 0f;
				this._barkDelay = delay;
				this._barkDelayTimer = 0f;
				this._currentVoiceType = voiceType;
				if (makeTimerZeroIfSuccessful)
				{
					this.Agent.MakeVoice(this._currentVoiceType, 2);
					this._barkFired = false;
					return;
				}
				this._barkFired = true;
			}
		}

		// Token: 0x0600090C RID: 2316 RVA: 0x0003F6CC File Offset: 0x0003D8CC
		private ActionIndexCache SelectActionForTaunt(AgentNavalAIComponent.AgentNavalTaunts navalTaunt)
		{
			ActionIndexCache actionIndexCache = ActionIndexCache.act_none;
			EquipmentIndex primaryWieldedItemIndex = this.Agent.GetPrimaryWieldedItemIndex();
			EquipmentIndex offhandWieldedItemIndex = this.Agent.GetOffhandWieldedItemIndex();
			WeaponComponentData weaponComponentData = ((primaryWieldedItemIndex != -1) ? this.Agent.Equipment[primaryWieldedItemIndex].CurrentUsageItem : null);
			WeaponComponentData weaponComponentData2 = ((offhandWieldedItemIndex != -1) ? this.Agent.Equipment[offhandWieldedItemIndex].CurrentUsageItem : null);
			bool hasMount = this.Agent.HasMount;
			bool isLeftStance = this.Agent.GetIsLeftStance();
			int num = -1;
			switch (navalTaunt)
			{
			case AgentNavalAIComponent.AgentNavalTaunts.Invite:
				if (weaponComponentData2 != null && weaponComponentData2.IsShield)
				{
					num = TauntUsageManager.Instance.GetIndexOfAction("taunt_13");
				}
				else
				{
					num = TauntUsageManager.Instance.GetIndexOfAction("taunt_10");
				}
				break;
			case AgentNavalAIComponent.AgentNavalTaunts.Invite2:
				num = TauntUsageManager.Instance.GetIndexOfAction("taunt_11");
				break;
			case AgentNavalAIComponent.AgentNavalTaunts.Point:
				num = TauntUsageManager.Instance.GetIndexOfAction("taunt_17");
				break;
			}
			if (num != -1)
			{
				actionIndexCache = ActionIndexCache.Create(TauntUsageManager.Instance.GetAction(num, isLeftStance, !hasMount, weaponComponentData, weaponComponentData2));
			}
			return actionIndexCache;
		}

		// Token: 0x0600090D RID: 2317 RVA: 0x0003F7E6 File Offset: 0x0003D9E6
		public void ActivateSwimToShore(MatrixFrame targetFrame)
		{
			this._targetFrameForSwimmingToShore = targetFrame;
			this._shouldTrySwimmingToShore = true;
		}

		// Token: 0x0600090E RID: 2318 RVA: 0x0003F7F6 File Offset: 0x0003D9F6
		public void DeactivateSwimToShore()
		{
			this._shouldTrySwimmingToShore = false;
		}

		// Token: 0x0400053F RID: 1343
		private const float CheckBridgeAndTargetingAgentCooldown = 3f;

		// Token: 0x04000540 RID: 1344
		private const float BarkCooldown = 1.5f;

		// Token: 0x04000541 RID: 1345
		private const float MediumMoraleThreshold = 70f;

		// Token: 0x04000542 RID: 1346
		private float _tauntTimer;

		// Token: 0x04000543 RID: 1347
		private float _barkTimer;

		// Token: 0x04000544 RID: 1348
		private float _checkBridgesAndTargetingAgentTimer;

		// Token: 0x04000545 RID: 1349
		private float _tauntCooldown = 12f + MBRandom.RandomFloat * 2f;

		// Token: 0x04000546 RID: 1350
		private float _tauntDelayTimer;

		// Token: 0x04000547 RID: 1351
		private float _barkDelayTimer;

		// Token: 0x04000548 RID: 1352
		private float _tauntDelay;

		// Token: 0x04000549 RID: 1353
		private float _barkDelay;

		// Token: 0x0400054A RID: 1354
		private bool _tauntFired;

		// Token: 0x0400054B RID: 1355
		private bool _barkFired;

		// Token: 0x0400054C RID: 1356
		private AgentNavalComponent _agentNavalComponent;

		// Token: 0x0400054D RID: 1357
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x0400054E RID: 1358
		private ActionIndexCache _currentActionIndexCache;

		// Token: 0x0400054F RID: 1359
		private SkinVoiceManager.SkinVoiceType _currentVoiceType;

		// Token: 0x04000550 RID: 1360
		private bool _isConnectedToEnemyWithoutBridges;

		// Token: 0x04000551 RID: 1361
		private AgentNavalAIComponent.AgentJumpOffDecisionType _jumpOffDecisionType;

		// Token: 0x04000552 RID: 1362
		private bool _shouldTrySwimmingToShore;

		// Token: 0x04000553 RID: 1363
		private MatrixFrame _targetFrameForSwimmingToShore;

		// Token: 0x020001F6 RID: 502
		public enum AgentNavalTaunts
		{
			// Token: 0x04000E51 RID: 3665
			Invite,
			// Token: 0x04000E52 RID: 3666
			Invite2,
			// Token: 0x04000E53 RID: 3667
			Point
		}

		// Token: 0x020001F7 RID: 503
		private enum AgentJumpOffDecisionType
		{
			// Token: 0x04000E55 RID: 3669
			None,
			// Token: 0x04000E56 RID: 3670
			MovingWithoutDetachment,
			// Token: 0x04000E57 RID: 3671
			MovingWithDetachment
		}
	}
}
