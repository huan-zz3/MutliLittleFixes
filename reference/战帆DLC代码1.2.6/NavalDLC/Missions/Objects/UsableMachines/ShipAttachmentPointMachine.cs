using System;
using System.Collections.Generic;
using NavalDLC.Missions.AI.UsableMachineAIs;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.Objects.UsableMachines
{
	// Token: 0x020000B3 RID: 179
	public class ShipAttachmentPointMachine : UsableMachine
	{
		// Token: 0x17000270 RID: 624
		// (get) Token: 0x06000DA8 RID: 3496 RVA: 0x0006BD70 File Offset: 0x00069F70
		// (set) Token: 0x06000DA9 RID: 3497 RVA: 0x0006BD78 File Offset: 0x00069F78
		public MissionShip OwnerShip { get; private set; }

		// Token: 0x17000271 RID: 625
		// (get) Token: 0x06000DAA RID: 3498 RVA: 0x0006BD81 File Offset: 0x00069F81
		// (set) Token: 0x06000DAB RID: 3499 RVA: 0x0006BD89 File Offset: 0x00069F89
		public ShipAttachmentMachine.ShipAttachment CurrentAttachment { get; private set; }

		// Token: 0x17000272 RID: 626
		// (get) Token: 0x06000DAC RID: 3500 RVA: 0x0006BD92 File Offset: 0x00069F92
		// (set) Token: 0x06000DAD RID: 3501 RVA: 0x0006BD9A File Offset: 0x00069F9A
		public Vec3 HookAttachLocalPosition { get; private set; }

		// Token: 0x17000273 RID: 627
		// (get) Token: 0x06000DAE RID: 3502 RVA: 0x0006BDA3 File Offset: 0x00069FA3
		// (set) Token: 0x06000DAF RID: 3503 RVA: 0x0006BDAB File Offset: 0x00069FAB
		public GameEntity ConnectionClipPlaneEntity { get; private set; }

		// Token: 0x17000274 RID: 628
		// (get) Token: 0x06000DB0 RID: 3504 RVA: 0x0006BDB4 File Offset: 0x00069FB4
		// (set) Token: 0x06000DB1 RID: 3505 RVA: 0x0006BDBC File Offset: 0x00069FBC
		public GameEntity RampBarrier { get; private set; }

		// Token: 0x17000275 RID: 629
		// (get) Token: 0x06000DB2 RID: 3506 RVA: 0x0006BDC5 File Offset: 0x00069FC5
		internal MBReadOnlyList<GameEntity> RampPhysicsList
		{
			get
			{
				return this._rampPhysicsList;
			}
		}

		// Token: 0x17000276 RID: 630
		// (get) Token: 0x06000DB3 RID: 3507 RVA: 0x0006BDCD File Offset: 0x00069FCD
		// (set) Token: 0x06000DB4 RID: 3508 RVA: 0x0006BDD5 File Offset: 0x00069FD5
		public GameEntity RampVisualEntity { get; private set; }

		// Token: 0x17000277 RID: 631
		// (get) Token: 0x06000DB5 RID: 3509 RVA: 0x0006BDDE File Offset: 0x00069FDE
		// (set) Token: 0x06000DB6 RID: 3510 RVA: 0x0006BDE6 File Offset: 0x00069FE6
		public ShipAttachmentMachine LinkedAttachmentMachine { get; private set; }

		// Token: 0x06000DB7 RID: 3511 RVA: 0x0006BDF0 File Offset: 0x00069FF0
		protected override void OnInit()
		{
			base.OnInit();
			WeakGameEntity weakGameEntity = base.GameEntity.Parent;
			while (this.OwnerShip == null && weakGameEntity.IsValid)
			{
				this.OwnerShip = weakGameEntity.GetFirstScriptOfType<MissionShip>();
				weakGameEntity = weakGameEntity.Parent;
			}
			if (base.GameEntity.Parent.GetScriptCountOfTypeRecursive<ShipAttachmentMachine>() == 1)
			{
				this.LinkedAttachmentMachine = base.GameEntity.Parent.GetFirstScriptOfTypeRecursive<ShipAttachmentMachine>();
			}
			int childCount = base.GameEntity.ChildCount;
			WeakGameEntity weakGameEntity2 = WeakGameEntity.Invalid;
			for (int i = 0; i < childCount; i++)
			{
				WeakGameEntity child = base.GameEntity.GetChild(i);
				if (child.Name == "hook_attach_point")
				{
					this.HookAttachLocalPosition = child.GetFrame().origin + 0.5f * child.GetFrame().rotation.u.NormalizedCopy();
					weakGameEntity2 = child;
				}
				else if (child.Name == "focus_object")
				{
					this._focusObject = GameEntity.CreateFromWeakEntity(child);
				}
			}
			if (weakGameEntity2 != WeakGameEntity.Invalid)
			{
				weakGameEntity2.Remove(78);
			}
			this.ConnectionClipPlaneEntity = GameEntity.CreateFromWeakEntity(base.GameEntity.GetFirstChildEntityWithTagRecursive("connection_point"));
			this.RampBarrier = GameEntity.CreateFromWeakEntity(base.GameEntity.GetFirstChildEntityWithTag("connection_barrier"));
			List<WeakGameEntity> list = new List<WeakGameEntity>();
			base.GameEntity.GetChildrenWithTagRecursive(list, "step_capsule");
			this._rampPhysicsList = new MBList<GameEntity>();
			foreach (WeakGameEntity weakGameEntity3 in list)
			{
				if (weakGameEntity3.GetVisibilityExcludeParents())
				{
					GameEntity gameEntity = GameEntity.CreateFromWeakEntity(weakGameEntity3);
					gameEntity.SetVisibilityExcludeParents(false);
					this._rampPhysicsList.Add(gameEntity);
				}
			}
			this.RampVisualEntity = GameEntity.CreateFromWeakEntity(base.GameEntity.GetFirstChildEntityWithTagRecursive("bridge_target"));
			this.RampVisualEntity.SetVisibilityExcludeParents(false);
			base.SetScriptComponentToTick(this.GetTickRequirement());
			this.EnemyRangeToStopUsing = 5f;
			this.IsDisabledForAttackerAIDueToEnemyInRange = new QueryData<bool>(() => this.OwnerShip != null && this.OwnerShip.ShipOrder != null && this.OwnerShip.ShipOrder.IsEnemyOnShip, 1f);
			this.IsDisabledForDefenderAIDueToEnemyInRange = new QueryData<bool>(() => this.OwnerShip != null && this.OwnerShip.ShipOrder != null && this.OwnerShip.ShipOrder.IsEnemyOnShip, 1f);
		}

		// Token: 0x06000DB8 RID: 3512 RVA: 0x0006C080 File Offset: 0x0006A280
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 2 | base.GetTickRequirement();
		}

		// Token: 0x06000DB9 RID: 3513 RVA: 0x0006C08A File Offset: 0x0006A28A
		public override void OnDeploymentFinished()
		{
			base.PilotStandingPoint.AddComponent(new ResetAnimationOnStopUsageComponent(ActionIndexCache.act_none, false));
		}

		// Token: 0x06000DBA RID: 3514 RVA: 0x0006C0A4 File Offset: 0x0006A2A4
		protected override void OnTick(float dt)
		{
			bool flag;
			if (!this.OwnerShip.BeingAbandoned)
			{
				ShipAttachmentMachine linkedAttachmentMachine = this.LinkedAttachmentMachine;
				flag = ((linkedAttachmentMachine != null) ? linkedAttachmentMachine.CurrentAttachment : null) != null || this.CurrentAttachment == null || (base.PilotAgent == null && (this.CurrentAttachment.State != ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected || this.OwnerShip.IsDisconnectionBlocked()));
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			base.PilotStandingPoint.SetIsDeactivatedSynched(flag2);
			if (this._focusObject.GetVisibilityExcludeParents() == flag2)
			{
				this._focusObject.SetVisibilityExcludeParents(!flag2);
			}
			if (base.PilotAgent != null)
			{
				if (this.OwnerShip.BeingAbandoned)
				{
					WorldFrame userFrameForAgent = base.PilotStandingPoint.GetUserFrameForAgent(base.PilotAgent);
					Vec3 f = userFrameForAgent.Rotation.f;
					f.Normalize();
					if (base.PilotAgent.GetCurrentAction(0) != this._actionForJumpingOff)
					{
						MatrixFrame matrixFrame = base.PilotAgent.Frame;
						Vec2 vec = matrixFrame.origin.AsVec2;
						if (vec.DistanceSquared(userFrameForAgent.Origin.AsVec2) <= 0.3f)
						{
							matrixFrame = base.PilotAgent.Frame;
							if (Vec3.DotProduct(matrixFrame.rotation.f.NormalizedCopy(), f) > 0.95f)
							{
								Agent pilotAgent = base.PilotAgent;
								if (pilotAgent.Formation != null)
								{
									this.RemoveAgent(pilotAgent);
									pilotAgent.Formation.AttachUnit(pilotAgent);
								}
								else
								{
									base.PilotAgent.StopUsingGameObject(true, 1);
								}
								Vec3 vec2 = pilotAgent.Position + f * 10f;
								pilotAgent.GetComponent<AgentNavalComponent>().SetupAgentToAbandonShip();
								pilotAgent.SetActionChannel(0, ref this._actionForJumpingOff, false, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
								Agent agent = pilotAgent;
								vec = vec2.AsVec2;
								agent.SetTargetPositionAndDirection(ref vec, ref f);
								pilotAgent.ClearTargetFrame();
								return;
							}
						}
					}
				}
				else if (this.CurrentAttachment != null && this.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected)
				{
					if (base.PilotAgent.SetActionChannel(1, ref ActionIndexCache.act_ship_connection_break, false, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true))
					{
						if (base.PilotAgent.GetCurrentActionProgress(1) > 0.99f)
						{
							this.CurrentAttachment.AttachmentSource.DisconnectAttachment();
							base.PilotAgent.StopUsingGameObject(true, 1);
							return;
						}
					}
					else
					{
						base.PilotAgent.StopUsingGameObject(true, 1);
					}
				}
			}
		}

		// Token: 0x06000DBB RID: 3515 RVA: 0x0006C32A File Offset: 0x0006A52A
		protected override float GetDetachmentWeightAux(BattleSideEnum side)
		{
			return float.MinValue;
		}

		// Token: 0x06000DBC RID: 3516 RVA: 0x0006C331 File Offset: 0x0006A531
		public override bool ShouldAutoLeaveDetachmentWhenDisabled(BattleSideEnum sideEnum)
		{
			return false;
		}

		// Token: 0x06000DBD RID: 3517 RVA: 0x0006C334 File Offset: 0x0006A534
		public void AssignConnection(ShipAttachmentMachine.ShipAttachment shipAttachment)
		{
			this.CurrentAttachment = shipAttachment;
		}

		// Token: 0x06000DBE RID: 3518 RVA: 0x0006C33D File Offset: 0x0006A53D
		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			TextObject textObject = new TextObject("{=PUbT3s7W}{KEY} Cut Loose", null);
			textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13), 1f));
			return textObject;
		}

		// Token: 0x06000DBF RID: 3519 RVA: 0x0006C36C File Offset: 0x0006A56C
		public override TextObject GetDescriptionText(WeakGameEntity gameEntity)
		{
			if (this.CurrentAttachment == null || this.CurrentAttachment.State != ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected)
			{
				ShipAttachmentMachine linkedAttachmentMachine = this.LinkedAttachmentMachine;
				if (((linkedAttachmentMachine != null) ? linkedAttachmentMachine.CurrentAttachment : null) == null || this.LinkedAttachmentMachine.CurrentAttachment.State != ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected)
				{
					return new TextObject("{=7zCPG8TR}Hook", null);
				}
			}
			return new TextObject("{=kCMGJl1W}Bridge", null);
		}

		// Token: 0x06000DC0 RID: 3520 RVA: 0x0006C3D0 File Offset: 0x0006A5D0
		public bool IsShipAttachmentMachinePointBridgeWithEnemy()
		{
			if (this.CurrentAttachment != null)
			{
				ShipAttachmentMachine.ShipAttachment currentAttachment = this.CurrentAttachment;
				Team team;
				if (currentAttachment == null)
				{
					team = null;
				}
				else
				{
					ShipAttachmentMachine attachmentSource = currentAttachment.AttachmentSource;
					if (attachmentSource == null)
					{
						team = null;
					}
					else
					{
						MissionShip ownerShip = attachmentSource.OwnerShip;
						team = ((ownerShip != null) ? ownerShip.Team : null);
					}
				}
				Team team2 = team;
				ShipAttachmentMachine.ShipAttachment currentAttachment2 = this.CurrentAttachment;
				Team team3;
				if (currentAttachment2 == null)
				{
					team3 = null;
				}
				else
				{
					ShipAttachmentPointMachine attachmentTarget = currentAttachment2.AttachmentTarget;
					if (attachmentTarget == null)
					{
						team3 = null;
					}
					else
					{
						MissionShip ownerShip2 = attachmentTarget.OwnerShip;
						team3 = ((ownerShip2 != null) ? ownerShip2.Team : null);
					}
				}
				Team team4 = team3;
				return team2 != null && team4 != null && team2.IsEnemyOf(team4) && this.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected;
			}
			return false;
		}

		// Token: 0x06000DC1 RID: 3521 RVA: 0x0006C45C File Offset: 0x0006A65C
		public bool IsShipAttachmentPointBridged()
		{
			return this.CurrentAttachment != null && (this.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected || this.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeThrown);
		}

		// Token: 0x06000DC2 RID: 3522 RVA: 0x0006C488 File Offset: 0x0006A688
		public bool IsShipAttachmentPointConnectedToEnemy()
		{
			if (this.CurrentAttachment != null && (this.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopesPulling || this.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeThrown || this.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected) && this.CurrentAttachment.AttachmentSource.OwnerShip.Team != null && this.CurrentAttachment.AttachmentTarget.OwnerShip.Team != null && this.CurrentAttachment.AttachmentSource.OwnerShip.Team.IsEnemyOf(this.CurrentAttachment.AttachmentTarget.OwnerShip.Team))
			{
				Formation formation = this.CurrentAttachment.AttachmentSource.OwnerShip.Formation;
				return formation != null && formation.CountOfUnits > 0;
			}
			return false;
		}

		// Token: 0x06000DC3 RID: 3523 RVA: 0x0006C551 File Offset: 0x0006A751
		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new ShipAttachmentPointAI(this);
		}

		// Token: 0x06000DC4 RID: 3524 RVA: 0x0006C559 File Offset: 0x0006A759
		protected override bool OnCheckForProblems()
		{
			return true;
		}

		// Token: 0x06000DC5 RID: 3525 RVA: 0x0006C55C File Offset: 0x0006A75C
		public void SetJumpOffAction(ActionIndexCache action)
		{
			this._actionForJumpingOff = action;
		}

		// Token: 0x0400088B RID: 2187
		[EditableScriptComponentVariable(true, "")]
		public int RelatedShipNavmeshOffset;

		// Token: 0x0400088F RID: 2191
		private GameEntity _focusObject;

		// Token: 0x04000892 RID: 2194
		private MBList<GameEntity> _rampPhysicsList;

		// Token: 0x04000895 RID: 2197
		private ActionIndexCache _actionForJumpingOff = ActionIndexCache.act_escape_jump;
	}
}
