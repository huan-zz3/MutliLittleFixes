using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NavalDLC.Missions.AI.TeamAI;
using NavalDLC.Missions.AI.UsableMachineAIs;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.NavalPhysics;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Objects.Usables;

namespace NavalDLC.Missions.Objects.UsableMachines
{
	// Token: 0x020000B1 RID: 177
	public class ShipAttachmentMachine : UsableMachine
	{
		// Token: 0x1700025D RID: 605
		// (get) Token: 0x06000D53 RID: 3411 RVA: 0x00069710 File Offset: 0x00067910
		// (set) Token: 0x06000D54 RID: 3412 RVA: 0x00069718 File Offset: 0x00067918
		public float BridgeConnectionLengthSquared { get; private set; }

		// Token: 0x1700025E RID: 606
		// (get) Token: 0x06000D55 RID: 3413 RVA: 0x00069721 File Offset: 0x00067921
		// (set) Token: 0x06000D56 RID: 3414 RVA: 0x00069729 File Offset: 0x00067929
		public MissionShip OwnerShip { get; private set; }

		// Token: 0x1700025F RID: 607
		// (get) Token: 0x06000D57 RID: 3415 RVA: 0x00069732 File Offset: 0x00067932
		// (set) Token: 0x06000D58 RID: 3416 RVA: 0x0006973A File Offset: 0x0006793A
		public ShipAttachmentMachine.ShipAttachment CurrentAttachment { get; private set; }

		// Token: 0x17000260 RID: 608
		// (get) Token: 0x06000D59 RID: 3417 RVA: 0x00069743 File Offset: 0x00067943
		// (set) Token: 0x06000D5A RID: 3418 RVA: 0x0006974B File Offset: 0x0006794B
		public RopePileBaked RopeVisual { get; private set; }

		// Token: 0x17000261 RID: 609
		// (get) Token: 0x06000D5B RID: 3419 RVA: 0x00069754 File Offset: 0x00067954
		// (set) Token: 0x06000D5C RID: 3420 RVA: 0x0006975C File Offset: 0x0006795C
		public ShipAttachmentPointMachine LinkedAttachmentPointMachine { get; private set; }

		// Token: 0x17000262 RID: 610
		// (get) Token: 0x06000D5D RID: 3421 RVA: 0x00069765 File Offset: 0x00067965
		// (set) Token: 0x06000D5E RID: 3422 RVA: 0x0006976D File Offset: 0x0006796D
		public GameEntity ConnectionClipPlaneEntity { get; private set; }

		// Token: 0x17000263 RID: 611
		// (get) Token: 0x06000D5F RID: 3423 RVA: 0x00069776 File Offset: 0x00067976
		// (set) Token: 0x06000D60 RID: 3424 RVA: 0x0006977E File Offset: 0x0006797E
		public GameEntity RampBarrier { get; private set; }

		// Token: 0x17000264 RID: 612
		// (get) Token: 0x06000D61 RID: 3425 RVA: 0x00069787 File Offset: 0x00067987
		// (set) Token: 0x06000D62 RID: 3426 RVA: 0x0006978F File Offset: 0x0006798F
		public float RopeMinLength { get; private set; }

		// Token: 0x17000265 RID: 613
		// (get) Token: 0x06000D63 RID: 3427 RVA: 0x00069798 File Offset: 0x00067998
		internal MBReadOnlyList<GameEntity> RampPhysicsList
		{
			get
			{
				return this._rampPhysicsList;
			}
		}

		// Token: 0x17000266 RID: 614
		// (get) Token: 0x06000D64 RID: 3428 RVA: 0x000697A0 File Offset: 0x000679A0
		// (set) Token: 0x06000D65 RID: 3429 RVA: 0x000697A8 File Offset: 0x000679A8
		internal GameEntity RampVisualEntity { get; private set; }

		// Token: 0x17000267 RID: 615
		// (get) Token: 0x06000D66 RID: 3430 RVA: 0x000697B1 File Offset: 0x000679B1
		// (set) Token: 0x06000D67 RID: 3431 RVA: 0x000697B9 File Offset: 0x000679B9
		public GameEntity BarrierSource { get; private set; }

		// Token: 0x17000268 RID: 616
		// (get) Token: 0x06000D68 RID: 3432 RVA: 0x000697C2 File Offset: 0x000679C2
		// (set) Token: 0x06000D69 RID: 3433 RVA: 0x000697CA File Offset: 0x000679CA
		public GameEntity BarrierTarget { get; private set; }

		// Token: 0x17000269 RID: 617
		// (get) Token: 0x06000D6A RID: 3434 RVA: 0x000697D3 File Offset: 0x000679D3
		// (set) Token: 0x06000D6B RID: 3435 RVA: 0x000697DB File Offset: 0x000679DB
		public GameEntity VFoldSource { get; private set; }

		// Token: 0x1700026A RID: 618
		// (get) Token: 0x06000D6C RID: 3436 RVA: 0x000697E4 File Offset: 0x000679E4
		// (set) Token: 0x06000D6D RID: 3437 RVA: 0x000697EC File Offset: 0x000679EC
		public GameEntity Hook { get; private set; }

		// Token: 0x1700026B RID: 619
		// (get) Token: 0x06000D6E RID: 3438 RVA: 0x000697F5 File Offset: 0x000679F5
		// (set) Token: 0x06000D6F RID: 3439 RVA: 0x000697FD File Offset: 0x000679FD
		public GameEntity VFoldTarget { get; private set; }

		// Token: 0x1700026C RID: 620
		// (get) Token: 0x06000D70 RID: 3440 RVA: 0x00069806 File Offset: 0x00067A06
		// (set) Token: 0x06000D71 RID: 3441 RVA: 0x0006980E File Offset: 0x00067A0E
		public GameEntity PlankBridgePhysicsEntity { get; private set; }

		// Token: 0x1700026D RID: 621
		// (get) Token: 0x06000D72 RID: 3442 RVA: 0x00069817 File Offset: 0x00067A17
		// (set) Token: 0x06000D73 RID: 3443 RVA: 0x0006981F File Offset: 0x00067A1F
		public PlankBridgeSteppedAgentManager SteppedAgentManager { get; private set; }

		// Token: 0x1700026E RID: 622
		// (get) Token: 0x06000D74 RID: 3444 RVA: 0x00069828 File Offset: 0x00067A28
		// (set) Token: 0x06000D75 RID: 3445 RVA: 0x00069830 File Offset: 0x00067A30
		public bool IsShipAttachmentJointPhysicsEnabled { get; private set; }

		// Token: 0x1700026F RID: 623
		// (get) Token: 0x06000D76 RID: 3446 RVA: 0x00069839 File Offset: 0x00067A39
		public NavalShipsLogic NavalShipsLogicCached
		{
			get
			{
				if (this._navalShipsLogicCached == null)
				{
					this._navalShipsLogicCached = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
				}
				return this._navalShipsLogicCached;
			}
		}

		// Token: 0x06000D77 RID: 3447 RVA: 0x00069859 File Offset: 0x00067A59
		public void SetShipAttachmentJointPhysicsEnabled(bool enabled)
		{
			this.IsShipAttachmentJointPhysicsEnabled = enabled;
		}

		// Token: 0x06000D78 RID: 3448 RVA: 0x00069862 File Offset: 0x00067A62
		public bool IsShipAttachmentMachineBridged()
		{
			return this.CurrentAttachment != null && (this.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected || this.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeThrown);
		}

		// Token: 0x06000D79 RID: 3449 RVA: 0x0006988C File Offset: 0x00067A8C
		public bool IsShipAttachmentMachineBridgeWithEnemy()
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

		// Token: 0x06000D7A RID: 3450 RVA: 0x00069918 File Offset: 0x00067B18
		public bool IsShipAttachmentMachineConnectedToEnemy()
		{
			return this.CurrentAttachment != null && (this.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopesPulling || this.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeThrown || this.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected) && this.CurrentAttachment.AttachmentSource.OwnerShip.Team != null && this.CurrentAttachment.AttachmentTarget.OwnerShip.Team != null && this.CurrentAttachment.AttachmentSource.OwnerShip.Team.IsEnemyOf(this.CurrentAttachment.AttachmentTarget.OwnerShip.Team);
		}

		// Token: 0x06000D7B RID: 3451 RVA: 0x000699BC File Offset: 0x00067BBC
		public static bool DoesShipAttachmentMachineSatisfyOarsmenGetUpCondition(ShipAttachmentMachine.ShipAttachment currentAttachment)
		{
			if (currentAttachment != null && (currentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopesPulling || currentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeThrown) && currentAttachment.AttachmentSource.OwnerShip.Team != null && currentAttachment.AttachmentTarget.OwnerShip.Team != null && currentAttachment.AttachmentSource.OwnerShip.Team.IsEnemyOf(currentAttachment.AttachmentTarget.OwnerShip.Team))
			{
				MissionShip ownerShip = currentAttachment.AttachmentSource.OwnerShip;
				MissionShip ownerShip2 = currentAttachment.AttachmentTarget.OwnerShip;
				Vec3 angularVelocity = ownerShip.Physics.AngularVelocity;
				Vec3 angularVelocity2 = ownerShip2.Physics.AngularVelocity;
				Vec3 origin = ownerShip.GameEntity.GetBodyWorldTransform().origin;
				Vec3 origin2 = ownerShip2.GameEntity.GetBodyWorldTransform().origin;
				Vec3 origin3 = currentAttachment.AttachmentSource.GameEntity.GetGlobalFrame().origin;
				Vec3 origin4 = currentAttachment.AttachmentTarget.GameEntity.GetGlobalFrame().origin;
				Vec3 vec = (origin3 - origin).NormalizedCopy();
				Vec3 vec2 = (origin4 - origin2).NormalizedCopy();
				Vec3 vec3 = ownerShip.Physics.LinearVelocity + Vec3.CrossProduct(vec, angularVelocity);
				Vec3 vec4 = ownerShip2.Physics.LinearVelocity + Vec3.CrossProduct(vec2, angularVelocity2) - vec3;
				float lengthSquared = (origin4 - origin3).LengthSquared;
				if (vec4.LengthSquared <= 16f && lengthSquared <= 64f)
				{
					foreach (ShipOarMachine shipOarMachine in ownerShip.LeftSideShipOarMachines)
					{
						if (MBRandom.RandomFloat > 0.6f)
						{
							Agent pilotAgent = shipOarMachine.PilotAgent;
							if (pilotAgent != null)
							{
								pilotAgent.YellAfterDelay(0.25f + MBRandom.RandomFloat);
							}
						}
					}
					foreach (ShipOarMachine shipOarMachine2 in ownerShip.RightSideShipOarMachines)
					{
						if (MBRandom.RandomFloat > 0.6f)
						{
							Agent pilotAgent2 = shipOarMachine2.PilotAgent;
							if (pilotAgent2 != null)
							{
								pilotAgent2.YellAfterDelay(0.25f + MBRandom.RandomFloat);
							}
						}
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000D7C RID: 3452 RVA: 0x00069C30 File Offset: 0x00067E30
		public override bool ShouldAutoLeaveDetachmentWhenDisabled(BattleSideEnum sideEnum)
		{
			return false;
		}

		// Token: 0x06000D7D RID: 3453 RVA: 0x00069C33 File Offset: 0x00067E33
		public override void Disable()
		{
			if (this.CurrentAttachment != null)
			{
				this.CurrentAttachment.Destroy();
				this.CurrentAttachment = null;
			}
			this.RemoveConnectionPhysicsEntities();
			base.Disable();
		}

		// Token: 0x06000D7E RID: 3454 RVA: 0x00069C5C File Offset: 0x00067E5C
		public void SetConnectionPhysicsEntitiesVisibility(bool visible)
		{
			if (this._physicsEntitiesVisibility != visible)
			{
				this.BarrierSource.SetVisibilityExcludeParents(visible);
				this.BarrierTarget.SetVisibilityExcludeParents(visible);
				this.VFoldSource.SetVisibilityExcludeParents(visible);
				this.VFoldTarget.SetVisibilityExcludeParents(visible);
				this.PlankBridgePhysicsEntity.SetVisibilityExcludeParents(visible);
				GameEntityPhysicsExtensions.SetPhysicsStateOnlyVariable(this.BarrierSource, visible, false);
				GameEntityPhysicsExtensions.SetPhysicsStateOnlyVariable(this.BarrierTarget, visible, false);
				GameEntityPhysicsExtensions.SetPhysicsStateOnlyVariable(this.VFoldSource, visible, false);
				GameEntityPhysicsExtensions.SetPhysicsStateOnlyVariable(this.VFoldTarget, visible, false);
				GameEntityPhysicsExtensions.SetPhysicsStateOnlyVariable(this.PlankBridgePhysicsEntity, visible, false);
				this._physicsEntitiesVisibility = visible;
			}
		}

		// Token: 0x06000D7F RID: 3455 RVA: 0x00069CFC File Offset: 0x00067EFC
		private void RemoveConnectionPhysicsEntities()
		{
			this.BarrierSource.Remove(78);
			this.BarrierTarget.Remove(78);
			this.VFoldSource.Remove(78);
			this.VFoldTarget.Remove(78);
			this.PlankBridgePhysicsEntity.Remove(35);
		}

		// Token: 0x06000D80 RID: 3456 RVA: 0x00069D4C File Offset: 0x00067F4C
		private void InitializeConnectionPhysicsEntities()
		{
			PhysicsMaterial.GetFromName("wood_nonstick");
			this._defaultPhysicsQuad = new Vec3[4];
			this._defaultPhysicsQuad[0] = new Vec3(-0.5f, -0.5f, 0f, -1f);
			this._defaultPhysicsQuad[1] = new Vec3(0.5f, -0.5f, 0f, -1f);
			this._defaultPhysicsQuad[2] = new Vec3(0.5f, 0.5f, 0f, -1f);
			this._defaultPhysicsQuad[3] = new Vec3(-0.5f, 0.5f, 0f, -1f);
			this._defaultIndicesCached = new int[6];
			this._defaultIndicesCached[0] = 0;
			this._defaultIndicesCached[1] = 1;
			this._defaultIndicesCached[2] = 2;
			this._defaultIndicesCached[3] = 0;
			this._defaultIndicesCached[4] = 2;
			this._defaultIndicesCached[5] = 3;
			this.BarrierSource = GameEntity.CreateEmpty(Mission.Current.Scene, true, true, true);
			this.BarrierSource.Name = "Bridge_barrier_source";
			this.BarrierTarget = GameEntity.CreateEmpty(Mission.Current.Scene, true, true, true);
			this.BarrierTarget.Name = "Bridge_barrier_target";
			this.VFoldSource = GameEntity.CreateEmpty(Mission.Current.Scene, true, true, true);
			this.VFoldSource.Name = "Bridge_vFold_source";
			this.VFoldTarget = GameEntity.CreateEmpty(Mission.Current.Scene, true, true, true);
			this.VFoldTarget.Name = "Bridge_vFold_target";
			this.PlankBridgePhysicsEntity = GameEntity.CreateEmpty(Mission.Current.Scene, false, true, true);
			GameEntity plankBridgePhysicsEntity = this.PlankBridgePhysicsEntity;
			MatrixFrame identity = MatrixFrame.Identity;
			plankBridgePhysicsEntity.SetGlobalFrame(ref identity, true);
			this.PlankBridgePhysicsEntity.Name = "Plank Bridge Physics";
			this.PlankBridgePhysicsEntity.CreateAndAddScriptComponent("PlankBridgeSteppedAgentManager", true);
			this.SteppedAgentManager = this.PlankBridgePhysicsEntity.GetFirstScriptOfType<PlankBridgeSteppedAgentManager>();
			this.SetConnectionPhysicsEntitiesVisibility(false);
		}

		// Token: 0x06000D81 RID: 3457 RVA: 0x00069F4C File Offset: 0x0006814C
		public bool CheckAttachmentMachineFlags(bool editMode)
		{
			IEnumerable<WeakGameEntity> children = base.GameEntity.GetChildren();
			string[] array = new string[] { "hook", "pilot", "pile" };
			foreach (WeakGameEntity weakGameEntity in children)
			{
				if (!Extensions.HasAnyFlag<EntityFlags>(weakGameEntity.EntityFlags, 131072) && array.Contains(weakGameEntity.Name) && !Extensions.HasAnyFlag<EntityFlags>(weakGameEntity.EntityFlags, 4096))
				{
					string text = string.Format("Root Entity: {0} {1}'s child {2} must have Does not Affect Parent's Local Bounding Box flag.", base.GameEntity.Root.Name, base.GameEntity.Name, weakGameEntity.Name);
					if (editMode)
					{
						MBEditor.AddEntityWarning(weakGameEntity, text);
					}
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000D82 RID: 3458 RVA: 0x0006A044 File Offset: 0x00068244
		protected override void OnRemoved(int removeReason)
		{
			this._navalShipsLogicCached = null;
		}

		// Token: 0x06000D83 RID: 3459 RVA: 0x0006A050 File Offset: 0x00068250
		protected override void OnInit()
		{
			base.OnInit();
			this.IsShipAttachmentJointPhysicsEnabled = true;
			this.BridgeConnectionLengthSquared = 20.25f;
			this.InitializeConnectionPhysicsEntities();
			WeakGameEntity weakGameEntity = base.GameEntity.Parent;
			while (this.OwnerShip == null && weakGameEntity.IsValid)
			{
				this.OwnerShip = weakGameEntity.GetFirstScriptOfType<MissionShip>();
				weakGameEntity = weakGameEntity.Parent;
			}
			if (base.GameEntity.Parent.GetScriptCountOfTypeRecursive<ShipAttachmentPointMachine>() == 1)
			{
				this.LinkedAttachmentPointMachine = base.GameEntity.Parent.GetFirstScriptOfTypeRecursive<ShipAttachmentPointMachine>();
			}
			int childCount = base.GameEntity.ChildCount;
			for (int i = 0; i < childCount; i++)
			{
				WeakGameEntity child = base.GameEntity.GetChild(i);
				if (child.Name == "hook")
				{
					this.Hook = GameEntity.CreateFromWeakEntity(child);
					MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
					MatrixFrame globalFrame2 = child.GetGlobalFrame();
					this._initialHookLocalFrame = globalFrame.TransformToLocalNonOrthogonal(ref globalFrame2);
				}
				else if (child.Name == "focus_object")
				{
					this._focusObject = GameEntity.CreateFromWeakEntity(child);
				}
			}
			this._hookItem = Game.Current.ObjectManager.GetObject<ItemObject>("hook");
			base.SetScriptComponentToTick(this.GetTickRequirement());
			this.RopeVisual = MBExtensions.GetFirstScriptInFamilyDescending<RopePileBaked>(base.GameEntity);
			this._staticRopeVisual = base.GameEntity.GetFirstChildEntityWithTagRecursive("pile_hanged_static");
			if (this._staticRopeVisual == null)
			{
				this._staticRopeVisual = base.GameEntity.GetFirstChildEntityWithTagRecursive("pile_floor_static");
			}
			this.EnemyRangeToStopUsing = 5f;
			this.RampBarrier = GameEntity.CreateFromWeakEntity(this.LinkedAttachmentPointMachine.GameEntity.GetFirstChildEntityWithTag("connection_barrier"));
			this.ConnectionClipPlaneEntity = GameEntity.CreateFromWeakEntity(this.LinkedAttachmentPointMachine.GameEntity.GetFirstChildEntityWithTagRecursive("connection_point"));
			List<WeakGameEntity> list = new List<WeakGameEntity>();
			this.LinkedAttachmentPointMachine.GameEntity.GetChildrenWithTagRecursive(list, "step_capsule");
			this._rampPhysicsList = new MBList<GameEntity>();
			foreach (WeakGameEntity weakGameEntity2 in list)
			{
				if (weakGameEntity2.GetVisibilityExcludeParents())
				{
					this._rampPhysicsList.Add(GameEntity.CreateFromWeakEntity(weakGameEntity2));
				}
			}
			this.RampVisualEntity = GameEntity.CreateFromWeakEntity(this.LinkedAttachmentPointMachine.GameEntity.GetFirstChildEntityWithTagRecursive("bridge_source"));
			this.RampVisualEntity.SetVisibilityExcludeParents(false);
			this.IsDisabledForAttackerAIDueToEnemyInRange = new QueryData<bool>(delegate
			{
				MissionShip ownerShip = this.OwnerShip;
				return ((ownerShip != null) ? ownerShip.ShipOrder : null) != null && this.OwnerShip.ShipOrder.IsEnemyOnShip;
			}, 1f);
			this.IsDisabledForDefenderAIDueToEnemyInRange = new QueryData<bool>(delegate
			{
				MissionShip ownerShip2 = this.OwnerShip;
				return ((ownerShip2 != null) ? ownerShip2.ShipOrder : null) != null && this.OwnerShip.ShipOrder.IsEnemyOnShip;
			}, 1f);
		}

		// Token: 0x06000D84 RID: 3460 RVA: 0x0006A338 File Offset: 0x00068538
		public void CheckCurrentAttachmentAndInitializeRopeBoundingBox()
		{
			if (this.CurrentAttachment == null)
			{
				this.RopeVisual.SetRopeBoundingBoxToInitialState();
			}
		}

		// Token: 0x06000D85 RID: 3461 RVA: 0x0006A34D File Offset: 0x0006854D
		protected override float GetDetachmentWeightAux(BattleSideEnum side)
		{
			return float.MinValue;
		}

		// Token: 0x06000D86 RID: 3462 RVA: 0x0006A354 File Offset: 0x00068554
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 22 | base.GetTickRequirement();
		}

		// Token: 0x06000D87 RID: 3463 RVA: 0x0006A35F File Offset: 0x0006855F
		public void SetPreferredTargetShip(MissionShip newTarget)
		{
			this._preferredTargetShip = newTarget;
		}

		// Token: 0x06000D88 RID: 3464 RVA: 0x0006A368 File Offset: 0x00068568
		public MissionShip GetPreferredTargetShip()
		{
			return this._preferredTargetShip;
		}

		// Token: 0x06000D89 RID: 3465 RVA: 0x0006A370 File Offset: 0x00068570
		public bool CalculateCanConnectToTargetShip(MissionShip targetShip)
		{
			if ((targetShip != null && targetShip.Physics.NavalSinkingState == NavalPhysics.SinkingState.Sinking) || (targetShip != null && targetShip.Physics.NavalSinkingState == NavalPhysics.SinkingState.Sunk))
			{
				return false;
			}
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in targetShip.AttachmentPointMachines)
			{
				if (shipAttachmentPointMachine.CurrentAttachment == null && ShipAttachmentMachine.ComputePotentialAttachmentValue(this, shipAttachmentPointMachine, false, false, true) > 0f)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000D8A RID: 3466 RVA: 0x0006A400 File Offset: 0x00068600
		public bool IsOnCorrectSide(MissionShip targetShip)
		{
			MatrixFrame matrixFrame = this.OwnerShip.GameEntity.GetFrame();
			Vec3 globalPosition = targetShip.GameEntity.GlobalPosition;
			Vec3 vec = matrixFrame.TransformToLocal(ref globalPosition);
			Vec2 asVec = vec.AsVec2;
			matrixFrame = this.OwnerShip.GameEntity.GetFrame();
			vec = base.GameEntity.GlobalPosition;
			return asVec.DotProduct(matrixFrame.TransformToLocal(ref vec).AsVec2) >= 0f;
		}

		// Token: 0x06000D8B RID: 3467 RVA: 0x0006A489 File Offset: 0x00068689
		public void SetCanConnectToFriends(bool canConnectToFriends)
		{
			this._checkedInitialConnections = false;
		}

		// Token: 0x06000D8C RID: 3468 RVA: 0x0006A492 File Offset: 0x00068692
		public bool HasCheckedInitialConnections()
		{
			return this._checkedInitialConnections;
		}

		// Token: 0x06000D8D RID: 3469 RVA: 0x0006A49C File Offset: 0x0006869C
		public void ConnectWithAttachmentPointMachine(ShipAttachmentPointMachine attachmentPointMachine, bool forceBridge = false, bool unbreakableBridge = false, bool connectionInitializedByPlayer = false)
		{
			Vec3 vec;
			if (base.PilotAgent != null)
			{
				MatrixFrame frame = base.PilotAgent.Frame;
				MatrixFrame matrixFrame = base.PilotAgent.GetBoneEntitialFrame(base.PilotAgent.Monster.MainHandItemBoneIndex, false);
				vec = frame.TransformToParent(ref matrixFrame.origin);
			}
			else
			{
				vec = base.GameEntity.GlobalPosition;
			}
			Vec3 vec2 = vec;
			Vec3 vec3 = vec - this.RopeVisual.GameEntity.GlobalPosition;
			vec3 = vec2 + vec3.NormalizedCopy() * 0.5f;
			Agent pilotAgent = base.PilotAgent;
			Vec3 vec4 = ((pilotAgent != null) ? pilotAgent.LookDirection : Vec3.Zero);
			ShipAttachmentMachine.ShipAttachment shipAttachment = new ShipAttachmentMachine.ShipAttachment(this, attachmentPointMachine, in vec3, in vec4, false, connectionInitializedByPlayer);
			this.CurrentAttachment = shipAttachment;
			if (attachmentPointMachine != null)
			{
				attachmentPointMachine.AssignConnection(shipAttachment);
			}
			if (forceBridge)
			{
				Vec3 globalPosition = this.RopeVisual.GameEntity.GlobalPosition;
				MatrixFrame matrixFrame = attachmentPointMachine.GameEntity.GetGlobalFrame();
				vec3 = attachmentPointMachine.HookAttachLocalPosition;
				Vec3 vec5 = matrixFrame.TransformToParent(ref vec3);
				shipAttachment.InitializeShipAttachmentJoint(globalPosition, vec5, unbreakableBridge);
				shipAttachment.CheckAndConnectBridge(true);
			}
		}

		// Token: 0x06000D8E RID: 3470 RVA: 0x0006A5B8 File Offset: 0x000687B8
		public ShipAttachmentPointMachine GetBestEnemyAttachment(bool checkAttachmentAlreadyExists = false, bool checkInteractionDistance = true)
		{
			ShipAttachmentPointMachine shipAttachmentPointMachine = null;
			float num = 0f;
			Vec3 origin = this.OwnerShip.GlobalFrame.origin;
			if (this._preferredTargetShip != null)
			{
				MatrixFrame matrixFrame = this._preferredTargetShip.GlobalFrame;
				if (matrixFrame.origin.DistanceSquared(origin) > 14400f)
				{
					return shipAttachmentPointMachine;
				}
				if (!this._preferredTargetShip.IsConnectionBlocked())
				{
					foreach (ShipAttachmentPointMachine shipAttachmentPointMachine2 in this._preferredTargetShip.AttachmentPointMachines)
					{
						if (shipAttachmentPointMachine2.CurrentAttachment == null)
						{
							ShipAttachmentMachine linkedAttachmentMachine = shipAttachmentPointMachine2.LinkedAttachmentMachine;
							if (((linkedAttachmentMachine != null) ? linkedAttachmentMachine.CurrentAttachment : null) == null)
							{
								float num2 = ShipAttachmentMachine.ComputePotentialAttachmentValue(this, shipAttachmentPointMachine2, checkInteractionDistance, false, true);
								if (num2 > num && (!checkAttachmentAlreadyExists || shipAttachmentPointMachine2.CurrentAttachment == null))
								{
									num = num2;
									shipAttachmentPointMachine = shipAttachmentPointMachine2;
								}
							}
						}
					}
				}
				if (shipAttachmentPointMachine != null)
				{
					return shipAttachmentPointMachine;
				}
				using (List<MissionShip>.Enumerator enumerator2 = this.OwnerShip.ShipsLogic.AllShips.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						MissionShip missionShip = enumerator2.Current;
						if (missionShip != this.OwnerShip && missionShip != this._preferredTargetShip && MissionShip.AreShipsConnected(missionShip, this._preferredTargetShip))
						{
							matrixFrame = missionShip.GlobalFrame;
							if (matrixFrame.origin.DistanceSquared(origin) <= 14400f && !missionShip.IsConnectionBlocked() && !this.OwnerShip.SearchShipConnection(missionShip, false, false, false, false))
							{
								foreach (ShipAttachmentPointMachine shipAttachmentPointMachine3 in missionShip.AttachmentPointMachines)
								{
									if (shipAttachmentPointMachine3.CurrentAttachment == null)
									{
										ShipAttachmentMachine linkedAttachmentMachine2 = shipAttachmentPointMachine3.LinkedAttachmentMachine;
										if (((linkedAttachmentMachine2 != null) ? linkedAttachmentMachine2.CurrentAttachment : null) == null)
										{
											float num3 = ShipAttachmentMachine.ComputePotentialAttachmentValue(this, shipAttachmentPointMachine3, true, false, true);
											if (num3 > num && (!checkAttachmentAlreadyExists || shipAttachmentPointMachine3.CurrentAttachment == null))
											{
												num = num3;
												shipAttachmentPointMachine = shipAttachmentPointMachine3;
											}
										}
									}
								}
							}
						}
					}
					return shipAttachmentPointMachine;
				}
			}
			foreach (MissionShip missionShip2 in this.OwnerShip.ShipsLogic.AllShips)
			{
				if (missionShip2 != this.OwnerShip)
				{
					MatrixFrame matrixFrame = missionShip2.GlobalFrame;
					if (matrixFrame.origin.DistanceSquared(origin) <= 14400f && !missionShip2.IsConnectionBlocked() && !this.OwnerShip.SearchShipConnection(missionShip2, false, false, false, false))
					{
						foreach (ShipAttachmentPointMachine shipAttachmentPointMachine4 in missionShip2.AttachmentPointMachines)
						{
							if (shipAttachmentPointMachine4.CurrentAttachment == null)
							{
								ShipAttachmentMachine linkedAttachmentMachine3 = shipAttachmentPointMachine4.LinkedAttachmentMachine;
								if (((linkedAttachmentMachine3 != null) ? linkedAttachmentMachine3.CurrentAttachment : null) == null && ((base.PilotAgent != null && !base.PilotAgent.IsAIControlled) || missionShip2 == this._preferredTargetShip || (this._preferredTargetShip == null && missionShip2.BattleSide != this.OwnerShip.BattleSide) || (this._preferredTargetShip != null && this._preferredTargetShip.ShipIslandCombinedID == missionShip2.ShipIslandCombinedID)))
								{
									float num4 = ShipAttachmentMachine.ComputePotentialAttachmentValue(this, shipAttachmentPointMachine4, true, false, true);
									if (num4 > num && (!checkAttachmentAlreadyExists || shipAttachmentPointMachine4.CurrentAttachment == null))
									{
										num = num4;
										shipAttachmentPointMachine = shipAttachmentPointMachine4;
									}
								}
							}
						}
					}
				}
			}
			return shipAttachmentPointMachine;
		}

		// Token: 0x06000D8F RID: 3471 RVA: 0x0006A9A4 File Offset: 0x00068BA4
		public override void OnDeploymentFinished()
		{
			base.PilotStandingPoint.AddComponent(new ResetAnimationOnStopUsageComponent(ActionIndexCache.act_none, false));
			base.PilotStandingPoint.AddComponent(new RemoveExtraWeaponOnStopUsageComponent());
			base.PilotStandingPoint.LockUserFrames = false;
			base.PilotStandingPoint.LockUserPositions = true;
		}

		// Token: 0x06000D90 RID: 3472 RVA: 0x0006A9E4 File Offset: 0x00068BE4
		protected override void OnTickParallel(float dt)
		{
			if (Mission.Current == null)
			{
				return;
			}
			if (this.CurrentAttachment != null && this.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected)
			{
				this.CurrentAttachment.OnParallelTick(dt);
			}
			if (this.CurrentAttachment == null && base.PilotAgent == null)
			{
				RopePileBaked ropeVisual = this.RopeVisual;
				Vec3 globalPosition = this.RopeVisual.GameEntity.GlobalPosition;
				Vec3 globalPosition2 = this.RopeVisual.GameEntity.GlobalPosition;
				ropeVisual.UpdateRopeMeshVisualAccordingToTargetPointLinearWithoutBoundingBoxUpdate(in globalPosition, in globalPosition2);
			}
		}

		// Token: 0x06000D91 RID: 3473 RVA: 0x0006AA64 File Offset: 0x00068C64
		protected override void OnTick(float dt)
		{
			if (Mission.Current == null)
			{
				return;
			}
			if (this.OwnerShip == null)
			{
				return;
			}
			if (!Mission.Current.MissionEnded)
			{
				ShipAttachmentPointMachine linkedAttachmentPointMachine = this.LinkedAttachmentPointMachine;
				bool flag = ((linkedAttachmentPointMachine != null) ? linkedAttachmentPointMachine.CurrentAttachment : null) != null || (base.PilotAgent == null && this.CurrentAttachment != null && (this.CurrentAttachment.State != ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected || this.OwnerShip.IsDisconnectionBlocked()));
				base.PilotStandingPoint.SetIsDeactivatedSynched(flag);
				base.PilotStandingPoint.AutoSheathWeapons = this.CurrentAttachment != null && this.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected;
				if (this._focusObject.GetVisibilityExcludeParents() == flag)
				{
					this._focusObject.SetVisibilityExcludeParents(!flag);
				}
			}
			if (base.PilotAgent != null)
			{
				if (this.CurrentAttachment == null)
				{
					base.PilotAgent.AgentVisuals.SetAttachedPositionForMeshAfterAnimationPostIntegrate(this.RopeVisual.GameEntity, base.PilotAgent.Monster.MainHandItemBoneIndex);
					if (base.PilotAgent.SetActionChannel(1, ref ActionIndexCache.act_usage_hook_ready, false, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true))
					{
						this.RopeVisual.GameEntity.SetVisibilityExcludeParents(true);
						this.Hook.SetVisibilityExcludeParents(false);
						this._staticRopeVisual.SetVisibilityExcludeParents(false);
						if (base.PilotAgent.WieldedWeapon.Item != this._hookItem)
						{
							string text = "event:/mission/movement/vessel/hook_grab";
							Vec3 position = base.PilotAgent.Position;
							SoundManager.StartOneShotEvent(text, ref position);
							MissionWeapon missionWeapon;
							missionWeapon..ctor(this._hookItem, null, null);
							base.PilotAgent.EquipWeaponToExtraSlotAndWield(ref missionWeapon);
						}
						if (base.PilotAgent.IsAIControlled)
						{
							if (this.GetBestEnemyAttachment(false, true) != null && !base.PilotAgent.SetActionChannel(1, ref ActionIndexCache.act_usage_hook_release, false, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true))
							{
								base.PilotAgent.StopUsingGameObject(true, 1);
							}
						}
						else if (base.PilotAgent.Mission.InputManager.IsGameKeyReleased(9) && Vec3.DotProduct(base.GameEntity.GetGlobalFrame().rotation.f, base.PilotAgent.LookRotation.f) >= 0f && !base.PilotAgent.SetActionChannel(1, ref ActionIndexCache.act_usage_hook_release, false, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true))
						{
							base.PilotAgent.StopUsingGameObject(true, 1);
						}
						this._checkedInitialConnections = true;
					}
					else if (base.PilotAgent.GetCurrentAction(1) == ActionIndexCache.act_usage_hook_release)
					{
						if (base.PilotAgent.IsAIControlled)
						{
							ShipAttachmentPointMachine bestEnemyAttachment = this.GetBestEnemyAttachment(false, true);
							if (bestEnemyAttachment == null)
							{
								if (!base.PilotAgent.SetActionChannel(1, ref ActionIndexCache.act_none, false, 12L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true) || !base.PilotAgent.SetActionChannel(1, ref ActionIndexCache.act_usage_hook_ready, false, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true))
								{
									base.PilotAgent.StopUsingGameObject(true, 1);
								}
							}
							else if (base.PilotAgent.GetCurrentActionProgress(1) > MBAnimation.GetAnimationParameter1("usage_hook_release"))
							{
								this.ConnectWithAttachmentPointMachine(bestEnemyAttachment, false, false, false);
								base.PilotAgent.RemoveEquippedWeapon(4);
								this.Hook.SetVisibilityExcludeParents(true);
							}
						}
						else if (base.PilotAgent.GetCurrentActionProgress(1) > MBAnimation.GetAnimationParameter1("usage_hook_release"))
						{
							this.ConnectWithAttachmentPointMachine(null, false, false, true);
							base.PilotAgent.RemoveEquippedWeapon(4);
							this.Hook.SetVisibilityExcludeParents(true);
						}
					}
					else if (!base.PilotAgent.IsInBeingStruckAction)
					{
						base.PilotAgent.StopUsingGameObject(true, 1);
					}
				}
				else if (base.PilotAgent.GetCurrentAction(1) == ActionIndexCache.act_usage_hook_release)
				{
					if (base.PilotAgent.GetCurrentActionProgress(1) > 0.99f)
					{
						base.PilotAgent.StopUsingGameObject(true, 1);
					}
				}
				else if (this.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected)
				{
					if (base.PilotAgent.SetActionChannel(1, ref ActionIndexCache.act_ship_connection_break, false, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true))
					{
						if (base.PilotAgent == Agent.Main && base.PilotAgent.GetCurrentActionProgress(1) < 0.1f)
						{
							MissionShip ownerShip = this.OwnerShip;
							bool flag2;
							if (ownerShip == null)
							{
								flag2 = false;
							}
							else
							{
								Team team = ownerShip.Team;
								bool? flag3 = ((team != null) ? new bool?(team.IsPlayerTeam) : null);
								bool flag4 = true;
								flag2 = (flag3.GetValueOrDefault() == flag4) & (flag3 != null);
							}
							if (flag2)
							{
								MissionShip ownerShip2 = this.OwnerShip;
								if (ownerShip2 != null)
								{
									ShipOrder shipOrder = ownerShip2.ShipOrder;
									if (shipOrder != null)
									{
										shipOrder.SetCutLoose(true);
									}
								}
							}
						}
						if (base.PilotAgent.GetCurrentActionProgress(1) > 0.99f)
						{
							this.DisconnectAttachment();
							base.PilotAgent.StopUsingGameObject(true, 1);
						}
					}
					else
					{
						base.PilotAgent.StopUsingGameObject(true, 1);
					}
				}
			}
			else if (this.CurrentAttachment == null)
			{
				this.RopeVisual.GameEntity.SetVisibilityExcludeParents(false);
				this.Hook.SetVisibilityExcludeParents(true);
				this._staticRopeVisual.SetVisibilityExcludeParents(true);
			}
			if (this.CurrentAttachment != null)
			{
				bool flag5 = this.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected;
				this.CurrentAttachment.OnTick(dt);
				if (!flag5 && this.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected)
				{
					this.CurrentAttachment.AttachmentSource.OwnerShip.OnShipConnected(this.CurrentAttachment);
					this.CurrentAttachment.AttachmentTarget.OwnerShip.OnShipConnected(this.CurrentAttachment);
				}
				if (this.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BrokenAndWaitingForRemoval)
				{
					this.CurrentAttachment.Destroy();
					this.CheckCurrentAttachmentAndInitializeRopeBoundingBox();
				}
			}
			if (this.Hook.GetVisibilityExcludeParents())
			{
				if (this.CurrentAttachment != null && (this.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopeThrown || this.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopesPulling || this.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected || this.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopeFailedAndReloading))
				{
					GameEntity hook = this.Hook;
					MatrixFrame matrixFrame = this.CurrentAttachment.HookGlobalFrame;
					hook.SetGlobalFrame(ref matrixFrame, true);
				}
				else
				{
					GameEntity hook2 = this.Hook;
					MatrixFrame matrixFrame = base.GameEntity.GetGlobalFrame();
					matrixFrame = matrixFrame.TransformToParent(ref this._initialHookLocalFrame);
					hook2.SetGlobalFrame(ref matrixFrame, true);
				}
			}
			if (Extensions.HasAllFlags<BodyFlags>(base.GameEntity.BodyFlag, 1073741824))
			{
				float num = base.GameEntity.GetGlobalFrame().origin.z + this.SinkingReferenceOffset;
				Scene scene = base.Scene;
				MatrixFrame matrixFrame = base.GameEntity.GetFrame();
				if (num < scene.GetWaterLevelAtPosition(matrixFrame.origin.AsVec2, true, false))
				{
					this.Disable();
				}
			}
		}

		// Token: 0x06000D92 RID: 3474 RVA: 0x0006B18C File Offset: 0x0006938C
		public void DisconnectAttachment()
		{
			this.CurrentAttachment.SetAttachmentState(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BrokenAndWaitingForRemoval);
			this.CurrentAttachment.AttachmentSource.OwnerShip.OnShipDisconnected(this.CurrentAttachment);
			this.CurrentAttachment.AttachmentTarget.OwnerShip.OnShipDisconnected(this.CurrentAttachment);
		}

		// Token: 0x06000D93 RID: 3475 RVA: 0x0006B1DC File Offset: 0x000693DC
		private static bool CheckIntersectionsBetweenConnectionsAux(Vec2 attachmentMachineSourcePosition, Vec2 attachmentMachineTargetPosition, ShipAttachmentMachine.ShipAttachment testAttachment)
		{
			return MBMath.CheckLineSegmentToLineSegmentIntersection(attachmentMachineSourcePosition, attachmentMachineTargetPosition, testAttachment.AttachmentSource.GameEntity.GlobalPosition.AsVec2, testAttachment.AttachmentTarget.GameEntity.GlobalPosition.AsVec2);
		}

		// Token: 0x06000D94 RID: 3476 RVA: 0x0006B228 File Offset: 0x00069428
		private static bool CheckIntersectionsBetweenConnectionsWithState(ShipAttachmentMachine attachmentMachine, ShipAttachmentPointMachine attachmentPointMachine, ShipAttachmentMachine.ShipAttachment.ShipAttachmentState state)
		{
			Vec2 asVec = attachmentMachine.GameEntity.GlobalPosition.AsVec2;
			Vec2 asVec2 = attachmentPointMachine.GameEntity.GlobalPosition.AsVec2;
			MissionShip ownerShip = attachmentMachine.OwnerShip;
			MissionShip ownerShip2 = attachmentPointMachine.OwnerShip;
			foreach (ShipAttachmentMachine shipAttachmentMachine in ownerShip.AttachmentMachines)
			{
				if (shipAttachmentMachine != attachmentMachine && shipAttachmentMachine.CurrentAttachment != null && shipAttachmentMachine.CurrentAttachment.State == state && shipAttachmentMachine.CurrentAttachment.AttachmentTarget != null && ShipAttachmentMachine.CheckIntersectionsBetweenConnectionsAux(asVec, asVec2, shipAttachmentMachine.CurrentAttachment))
				{
					return true;
				}
			}
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in ownerShip.AttachmentPointMachines)
			{
				if (shipAttachmentPointMachine.CurrentAttachment != null && shipAttachmentPointMachine.CurrentAttachment.State == state && ShipAttachmentMachine.CheckIntersectionsBetweenConnectionsAux(asVec, asVec2, shipAttachmentPointMachine.CurrentAttachment))
				{
					return true;
				}
			}
			foreach (ShipAttachmentMachine shipAttachmentMachine2 in ownerShip2.AttachmentMachines)
			{
				if (shipAttachmentMachine2.CurrentAttachment != null && shipAttachmentMachine2.CurrentAttachment.State == state && shipAttachmentMachine2.CurrentAttachment.AttachmentTarget != null && ShipAttachmentMachine.CheckIntersectionsBetweenConnectionsAux(asVec, asVec2, shipAttachmentMachine2.CurrentAttachment))
				{
					return true;
				}
			}
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine2 in ownerShip2.AttachmentPointMachines)
			{
				if (shipAttachmentPointMachine2 != attachmentPointMachine && shipAttachmentPointMachine2.CurrentAttachment != null && shipAttachmentPointMachine2.CurrentAttachment.State == state && ShipAttachmentMachine.CheckIntersectionsBetweenConnectionsAux(asVec, asVec2, shipAttachmentPointMachine2.CurrentAttachment))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000D95 RID: 3477 RVA: 0x0006B454 File Offset: 0x00069654
		private static bool CheckAttachmentsFacingEachOther(ShipAttachmentMachine attachmentMachine, ShipAttachmentPointMachine attachmentPointMachine)
		{
			MatrixFrame globalFrame = attachmentMachine.GameEntity.GetGlobalFrame();
			MatrixFrame globalFrame2 = attachmentPointMachine.GameEntity.GetGlobalFrame();
			Vec2 asVec = globalFrame.rotation.f.AsVec2;
			Vec2 asVec2 = globalFrame2.rotation.f.AsVec2;
			Vec2 vec = globalFrame2.origin.AsVec2 - globalFrame.origin.AsVec2;
			return Vec2.DotProduct(asVec, asVec2) < 0f && Vec2.DotProduct(vec, asVec2) < 0f;
		}

		// Token: 0x06000D96 RID: 3478 RVA: 0x0006B4E0 File Offset: 0x000696E0
		private static bool CheckIntersectionsBetweenConnections(ShipAttachmentMachine attachmentMachine, ShipAttachmentPointMachine attachmentPointMachine)
		{
			Vec2 asVec = attachmentMachine.GameEntity.GlobalPosition.AsVec2;
			Vec2 asVec2 = attachmentPointMachine.GameEntity.GlobalPosition.AsVec2;
			MissionShip ownerShip = attachmentMachine.OwnerShip;
			MissionShip ownerShip2 = attachmentPointMachine.OwnerShip;
			foreach (ShipAttachmentMachine shipAttachmentMachine in ownerShip.AttachmentMachines)
			{
				if (shipAttachmentMachine != attachmentMachine && shipAttachmentMachine.CurrentAttachment != null && shipAttachmentMachine.CurrentAttachment.AttachmentTarget != null && shipAttachmentMachine.CurrentAttachment.AttachmentTarget != attachmentPointMachine && ShipAttachmentMachine.CheckIntersectionsBetweenConnectionsAux(asVec, asVec2, shipAttachmentMachine.CurrentAttachment))
				{
					return true;
				}
			}
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in ownerShip.AttachmentPointMachines)
			{
				if (shipAttachmentPointMachine.CurrentAttachment != null && ShipAttachmentMachine.CheckIntersectionsBetweenConnectionsAux(asVec, asVec2, shipAttachmentPointMachine.CurrentAttachment))
				{
					return true;
				}
			}
			foreach (ShipAttachmentMachine shipAttachmentMachine2 in ownerShip2.AttachmentMachines)
			{
				if (shipAttachmentMachine2.CurrentAttachment != null && shipAttachmentMachine2.CurrentAttachment.AttachmentTarget != null && ShipAttachmentMachine.CheckIntersectionsBetweenConnectionsAux(asVec, asVec2, shipAttachmentMachine2.CurrentAttachment))
				{
					return true;
				}
			}
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine2 in ownerShip2.AttachmentPointMachines)
			{
				if (shipAttachmentPointMachine2 != attachmentPointMachine && shipAttachmentPointMachine2.CurrentAttachment != null && shipAttachmentPointMachine2.CurrentAttachment.AttachmentSource != attachmentMachine && ShipAttachmentMachine.CheckIntersectionsBetweenConnectionsAux(asVec, asVec2, shipAttachmentPointMachine2.CurrentAttachment))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000D97 RID: 3479 RVA: 0x0006B6EC File Offset: 0x000698EC
		public static bool IsShipNearAttachmentMachines(MissionShip ship, MatrixFrame shipFrame, Vec2 sourceGlobalPos, Vec2 targetGlobalPos)
		{
			float radius = ship.Physics.PhysicsBoundingBoxWithoutChildren.radius;
			Vec3 center = ship.Physics.PhysicsBoundingBoxWithoutChildren.center;
			Vec2 asVec = shipFrame.TransformToParent(ref center).AsVec2;
			Vec2 vec = (sourceGlobalPos + targetGlobalPos) * 0.5f;
			float num = vec.Distance(sourceGlobalPos) + radius;
			return asVec.DistanceSquared(vec) <= num * num;
		}

		// Token: 0x06000D98 RID: 3480 RVA: 0x0006B760 File Offset: 0x00069960
		public static bool IsShipBetweenAttachments(ShipAttachmentMachine attachmentMachineSource, ShipAttachmentPointMachine attachmentMachineTarget)
		{
			Vec2 asVec = attachmentMachineSource.GameEntity.GlobalPosition.AsVec2;
			Vec2 asVec2 = attachmentMachineTarget.GameEntity.GlobalPosition.AsVec2;
			foreach (MissionShip missionShip in attachmentMachineSource.NavalShipsLogicCached.AllShips)
			{
				if (missionShip != attachmentMachineSource.OwnerShip && missionShip != attachmentMachineTarget.OwnerShip)
				{
					MatrixFrame globalFrame = missionShip.GameEntity.GetGlobalFrame();
					Vec2[] array = missionShip.CalculateBoundingXYGlobalPlaneFromLocal(in globalFrame, 1f);
					if (ShipAttachmentMachine.EarlyCrossCheckForShipIntersectingAttachmentMachine(array, asVec, asVec2) && ShipAttachmentMachine.IsShipNearAttachmentMachines(missionShip, globalFrame, asVec, asVec2) && ShipAttachmentMachine.IsLineSegmentIntersectingShipBoundingXYPlane(array, asVec, asVec2))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000D99 RID: 3481 RVA: 0x0006B844 File Offset: 0x00069A44
		private static bool EarlyCrossCheckForShipIntersectingAttachmentMachine(Vec2[] physicsBoundingBoxPointsOfShip, Vec2 attachmentSourceGlobalPosition, Vec2 attachmentTargetGlobalPosition)
		{
			Vec2 vec = attachmentSourceGlobalPosition - attachmentTargetGlobalPosition;
			float num = Vec2.CCW(physicsBoundingBoxPointsOfShip[0] - attachmentTargetGlobalPosition, vec);
			for (int i = 1; i < physicsBoundingBoxPointsOfShip.Length; i++)
			{
				float num2 = Vec2.CCW(physicsBoundingBoxPointsOfShip[i] - attachmentTargetGlobalPosition, vec);
				if (num * num2 <= 0f)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000D9A RID: 3482 RVA: 0x0006B89C File Offset: 0x00069A9C
		public static bool IsLineSegmentIntersectingShipBoundingXYPlane(Vec2[] physicsBoundingBoxPointsOfShip, Vec2 attachment0Position, Vec2 attachment1Position)
		{
			return MBMath.CheckLineSegmentToLineSegmentIntersection(physicsBoundingBoxPointsOfShip[0], physicsBoundingBoxPointsOfShip[1], attachment0Position, attachment1Position) || MBMath.CheckLineSegmentToLineSegmentIntersection(physicsBoundingBoxPointsOfShip[1], physicsBoundingBoxPointsOfShip[2], attachment0Position, attachment1Position) || MBMath.CheckLineSegmentToLineSegmentIntersection(physicsBoundingBoxPointsOfShip[2], physicsBoundingBoxPointsOfShip[3], attachment0Position, attachment1Position) || MBMath.CheckLineSegmentToLineSegmentIntersection(physicsBoundingBoxPointsOfShip[3], physicsBoundingBoxPointsOfShip[0], attachment0Position, attachment1Position) || (MBMath.CheckPointInsidePolygon(ref physicsBoundingBoxPointsOfShip[0], ref physicsBoundingBoxPointsOfShip[1], ref physicsBoundingBoxPointsOfShip[2], ref physicsBoundingBoxPointsOfShip[3], ref attachment0Position) || MBMath.CheckPointInsidePolygon(ref physicsBoundingBoxPointsOfShip[0], ref physicsBoundingBoxPointsOfShip[1], ref physicsBoundingBoxPointsOfShip[2], ref physicsBoundingBoxPointsOfShip[3], ref attachment1Position));
		}

		// Token: 0x06000D9B RID: 3483 RVA: 0x0006B95C File Offset: 0x00069B5C
		public static float ComputePotentialAttachmentValue(ShipAttachmentMachine attachmentSource, ShipAttachmentPointMachine attachmentTarget, bool checkInteractionDistance, bool checkConnectionBlock, bool allowWiderAngleBetweenConnections)
		{
			if (!checkConnectionBlock || !attachmentSource.OwnerShip.IsConnectionBlocked())
			{
				MatrixFrame globalFrame = attachmentSource.GameEntity.GetGlobalFrame();
				Vec3 vec = globalFrame.rotation.f.NormalizedCopy();
				MatrixFrame globalFrame2 = attachmentTarget.GameEntity.GetGlobalFrame();
				Vec3 vec2 = globalFrame2.origin - globalFrame.origin;
				float num = vec2.Normalize();
				if (!checkInteractionDistance || num <= 40f)
				{
					float num2 = Vec3.DotProduct(vec2, vec);
					if (num2 > (allowWiderAngleBetweenConnections ? 0.1736f : 0.4226f))
					{
						if (ShipAttachmentMachine.IsShipBetweenAttachments(attachmentSource, attachmentTarget))
						{
							return -1f;
						}
						if (ShipAttachmentMachine.CheckIntersectionsBetweenConnections(attachmentSource, attachmentTarget))
						{
							return -1f;
						}
						if (!ShipAttachmentMachine.CheckAttachmentsFacingEachOther(attachmentSource, attachmentTarget))
						{
							return -1f;
						}
						Vec3 vec3 = globalFrame2.rotation.f.NormalizedCopy();
						float num3 = Vec3.DotProduct(-vec2, vec3);
						if (num3 > 0.1736f)
						{
							return 10000f * num2 * num3 / num;
						}
					}
				}
			}
			return -1f;
		}

		// Token: 0x06000D9C RID: 3484 RVA: 0x0006BA61 File Offset: 0x00069C61
		protected override void OnFixedTick(float fixedDt)
		{
			if (this.CurrentAttachment != null)
			{
				this.CurrentAttachment.OnFixedTick(fixedDt);
			}
		}

		// Token: 0x06000D9D RID: 3485 RVA: 0x0006BA78 File Offset: 0x00069C78
		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			TextObject textObject;
			if (this.CurrentAttachment == null || this.CurrentAttachment.State != ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected)
			{
				ShipAttachmentPointMachine linkedAttachmentPointMachine = this.LinkedAttachmentPointMachine;
				if (((linkedAttachmentPointMachine != null) ? linkedAttachmentPointMachine.CurrentAttachment : null) == null || this.LinkedAttachmentPointMachine.CurrentAttachment.State != ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected)
				{
					textObject = new TextObject("{=fEQAPJ2e}{KEY} Use", null);
					goto IL_0057;
				}
			}
			textObject = new TextObject("{=PUbT3s7W}{KEY} Cut Loose", null);
			IL_0057:
			textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13), 1f));
			return textObject;
		}

		// Token: 0x06000D9E RID: 3486 RVA: 0x0006BB00 File Offset: 0x00069D00
		public override TextObject GetDescriptionText(WeakGameEntity gameEntity)
		{
			if (this.CurrentAttachment == null || this.CurrentAttachment.State != ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected)
			{
				ShipAttachmentPointMachine linkedAttachmentPointMachine = this.LinkedAttachmentPointMachine;
				if (((linkedAttachmentPointMachine != null) ? linkedAttachmentPointMachine.CurrentAttachment : null) == null || this.LinkedAttachmentPointMachine.CurrentAttachment.State != ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected)
				{
					return new TextObject("{=7zCPG8TR}Hook", null);
				}
			}
			return new TextObject("{=kCMGJl1W}Bridge", null);
		}

		// Token: 0x06000D9F RID: 3487 RVA: 0x0006BB61 File Offset: 0x00069D61
		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new ShipAttachmentMachineAI(this);
		}

		// Token: 0x04000840 RID: 2112
		public const float AgentOarLeaveAttachmentLengthSquared = 64f;

		// Token: 0x04000841 RID: 2113
		public const float AgentOarLeaveRelativeSpeedThreshold = 4f;

		// Token: 0x04000842 RID: 2114
		public const float MaximumRopeLength = 40f;

		// Token: 0x04000843 RID: 2115
		public const float MinimumBridgeDistanceToKeep = 2.2f;

		// Token: 0x04000844 RID: 2116
		public const float MaximumRopesPullingDuration = 30f;

		// Token: 0x04000845 RID: 2117
		public const float BridgeConnectionRelativeSpeedThreshold = 4f;

		// Token: 0x04000846 RID: 2118
		public const float RopesPullingFrequency = 1f;

		// Token: 0x04000847 RID: 2119
		public const float RopesPullingRelaxSpeed = 0.05f;

		// Token: 0x04000848 RID: 2120
		public const float RopesPullingRelaxThresholdRatio = 0.75f;

		// Token: 0x04000849 RID: 2121
		public const float RopesPullingPullSpeed = 0.65f;

		// Token: 0x0400084A RID: 2122
		public const float RopesPullingPullAcceleration = 0.25f;

		// Token: 0x0400084B RID: 2123
		public const float RopesPullingWaveAmp = 0.6f;

		// Token: 0x0400084C RID: 2124
		public const float StiffnessRampTime = 5f;

		// Token: 0x0400084D RID: 2125
		public const float MaxDistanceError = 10f;

		// Token: 0x0400084E RID: 2126
		public const float MaxDistanceErrorBridge = 5f;

		// Token: 0x0400084F RID: 2127
		public const float MaxXYError = 2.75f;

		// Token: 0x04000850 RID: 2128
		public const float MaxAlignmentError = 0.95f;

		// Token: 0x04000851 RID: 2129
		public const float MaxAccumulatedAlignmentError = 20f;

		// Token: 0x04000852 RID: 2130
		public const float InteractionDistance = 40f;

		// Token: 0x04000853 RID: 2131
		public const float FatigueRate = 4f;

		// Token: 0x04000854 RID: 2132
		public const float RopeBeta = 0.1f;

		// Token: 0x04000855 RID: 2133
		public const float StretchLimit = 2f;

		// Token: 0x04000856 RID: 2134
		public const float Damping = 0.1f;

		// Token: 0x04000857 RID: 2135
		public const float RopeMaxAccelerationLowTension = 1.2f;

		// Token: 0x04000858 RID: 2136
		public const float RopeMaxAccelerationHighTension = 5f;

		// Token: 0x04000859 RID: 2137
		public const float BridgeDirectionDampingRatio = 0.3f;

		// Token: 0x0400085A RID: 2138
		public const float BridgeDirectionTargetPeriod = 2f;

		// Token: 0x0400085B RID: 2139
		public const float BridgeDirectionMaxAcceleration = 5f;

		// Token: 0x0400085C RID: 2140
		public const float AlignmentDampingRatio = 0.8f;

		// Token: 0x0400085D RID: 2141
		private const bool CanConnectToFriends = false;

		// Token: 0x0400085E RID: 2142
		public const float AlignmentTargetPeriod = 1.75f;

		// Token: 0x0400085F RID: 2143
		public const float AlignmentMaxAcceleration = 5f;

		// Token: 0x04000860 RID: 2144
		public const float XYDampingRatio = 0.5f;

		// Token: 0x04000861 RID: 2145
		public const float XYTargetPeriod = 0.75f;

		// Token: 0x04000862 RID: 2146
		public const float XYMaxAcceleration = 15f;

		// Token: 0x04000863 RID: 2147
		public const float MaxInclineAngle = 1.134464f;

		// Token: 0x04000864 RID: 2148
		private const string HookItemID = "hook";

		// Token: 0x04000865 RID: 2149
		private const string HookGrabSoundEvent = "event:/mission/movement/vessel/hook_grab";

		// Token: 0x04000866 RID: 2150
		public const string ConnectionClipPointTag = "connection_point";

		// Token: 0x04000867 RID: 2151
		public const string RampBarrierTag = "connection_barrier";

		// Token: 0x04000868 RID: 2152
		public const string RampCapsulePhysicsTag = "step_capsule";

		// Token: 0x04000869 RID: 2153
		public const string RampSourceVisualTag = "bridge_source";

		// Token: 0x0400086A RID: 2154
		public const string RampTargetVisualTag = "bridge_target";

		// Token: 0x0400086B RID: 2155
		public const string PileHangedStaticVisualTag = "pile_hanged_static";

		// Token: 0x0400086C RID: 2156
		public const string PileFloorStaticVisualTag = "pile_floor_static";

		// Token: 0x0400086D RID: 2157
		[EditableScriptComponentVariable(true, "")]
		public int RelatedShipNavmeshOffset;

		// Token: 0x04000870 RID: 2160
		private MissionShip _preferredTargetShip;

		// Token: 0x04000871 RID: 2161
		private bool _checkedInitialConnections;

		// Token: 0x04000874 RID: 2164
		private WeakGameEntity _staticRopeVisual;

		// Token: 0x04000875 RID: 2165
		private ItemObject _hookItem;

		// Token: 0x04000876 RID: 2166
		private GameEntity _focusObject;

		// Token: 0x0400087A RID: 2170
		private MatrixFrame _initialHookLocalFrame;

		// Token: 0x0400087B RID: 2171
		private MBList<GameEntity> _rampPhysicsList;

		// Token: 0x0400087E RID: 2174
		private bool _physicsEntitiesVisibility;

		// Token: 0x04000886 RID: 2182
		private Vec3[] _defaultPhysicsQuad;

		// Token: 0x04000887 RID: 2183
		private int[] _defaultIndicesCached;

		// Token: 0x04000888 RID: 2184
		private NavalShipsLogic _navalShipsLogicCached;

		// Token: 0x0200023B RID: 571
		public class ShipBridgeNavmeshHolder : MissionObject
		{
			// Token: 0x17000408 RID: 1032
			// (get) Token: 0x06001B23 RID: 6947 RVA: 0x000B22C7 File Offset: 0x000B04C7
			// (set) Token: 0x06001B24 RID: 6948 RVA: 0x000B22CF File Offset: 0x000B04CF
			public int BridgeNavmeshId { get; private set; }

			// Token: 0x17000409 RID: 1033
			// (get) Token: 0x06001B25 RID: 6949 RVA: 0x000B22D8 File Offset: 0x000B04D8
			// (set) Token: 0x06001B26 RID: 6950 RVA: 0x000B22E0 File Offset: 0x000B04E0
			public ShipAttachmentMachine.ShipAttachment CurrentAttachment { get; private set; }

			// Token: 0x06001B27 RID: 6951 RVA: 0x000B22E9 File Offset: 0x000B04E9
			public int GetFace1GroupIndex()
			{
				return this._face1PathFaceRecord.FaceGroupIndex;
			}

			// Token: 0x06001B28 RID: 6952 RVA: 0x000B22F6 File Offset: 0x000B04F6
			public int GetFace2GroupIndex()
			{
				return this._face2PathFaceRecord.FaceGroupIndex;
			}

			// Token: 0x06001B29 RID: 6953 RVA: 0x000B2304 File Offset: 0x000B0504
			public void Initialize(int bridgeNavmeshId, ShipAttachmentMachine attachmentSource)
			{
				this._face1PathFaceRecord = PathFaceRecord.NullFaceRecord;
				this._face2PathFaceRecord = PathFaceRecord.NullFaceRecord;
				this.BridgeNavmeshId = bridgeNavmeshId;
				this.CurrentAttachment = attachmentSource.CurrentAttachment;
				base.GameEntity.Scene.ImportNavigationMeshPrefab("ship_connection_plank_navmesh_1", this.BridgeNavmeshId);
				base.GameEntity.AttachNavigationMeshFaces(this.BridgeNavmeshId, false, false, false, false, false);
				base.GameEntity.AttachNavigationMeshFaces(this.BridgeNavmeshId + 1, false, false, false, false, false);
				base.GameEntity.AttachNavigationMeshFaces(this.BridgeNavmeshId + 2, false, false, false, false, false);
				base.GameEntity.AttachNavigationMeshFaces(this.BridgeNavmeshId + 3, false, false, false, false, false);
				base.GameEntity.AttachNavigationMeshFaces(this.BridgeNavmeshId + 4, false, false, false, false, true);
				base.GameEntity.SetUpdateValidtyOnFrameChangedOfFacesWithId(this.BridgeNavmeshId + 1, true);
				base.GameEntity.SetUpdateValidtyOnFrameChangedOfFacesWithId(this.BridgeNavmeshId + 2, true);
				Mission.Current.Scene.SetAbilityOfFacesWithId(this.BridgeNavmeshId + 3, false);
				Mission.Current.Scene.SetAbilityOfFacesWithId(this.BridgeNavmeshId + 4, false);
				this._customVertexIndices = new int[6];
				this._bridgeCustomVertexPositionsArray = new Vec3[6];
				this._attachedFaceCount = base.GameEntity.GetAttachedNavmeshFaceCount();
				PathFaceRecord[] array = new PathFaceRecord[this._attachedFaceCount];
				base.GameEntity.GetAttachedNavmeshFaceRecords(array);
				foreach (PathFaceRecord pathFaceRecord in array)
				{
					if (pathFaceRecord.FaceGroupIndex == this.BridgeNavmeshId + 1)
					{
						this._face1PathFaceRecord = pathFaceRecord;
					}
					else if (pathFaceRecord.FaceGroupIndex == this.BridgeNavmeshId + 2)
					{
						this._face2PathFaceRecord = pathFaceRecord;
					}
				}
				int[] array3 = new int[4];
				int[] array4 = new int[4];
				base.GameEntity.GetAttachedNavmeshFaceVertexIndices(ref this._face1PathFaceRecord, array3);
				base.GameEntity.GetAttachedNavmeshFaceVertexIndices(ref this._face2PathFaceRecord, array4);
				int num = -1;
				int num2 = -1;
				int num3 = -1;
				int num4 = -1;
				for (int j = 0; j < 4; j++)
				{
					int k = 0;
					while (k < 4)
					{
						if (array3[j] == array4[k])
						{
							if (num == -1 && num3 == -1)
							{
								num = j;
								num3 = k;
								break;
							}
							num2 = j;
							num4 = k;
							break;
						}
						else
						{
							k++;
						}
					}
				}
				int num5 = (num + 1) % 4;
				int num6 = (num + 2) % 4;
				int num7 = (num4 + 1) % 4;
				int num8 = (num4 + 2) % 4;
				this.SetCustomNavmeshVertexIndices(array4[num7], array3[num2], array3[num6], array4[num8], array3[num], array3[num5]);
				this.CurrentAttachment.AttachmentSource.SteppedAgentManager.SetNavmeshHolder(this);
			}

			// Token: 0x06001B2A RID: 6954 RVA: 0x000B25C0 File Offset: 0x000B07C0
			public void SetCustomNavmeshVertexIndices(int v1, int v2, int v3, int v4, int v5, int v6)
			{
				this._customVertexIndices[0] = v1;
				this._customVertexIndices[1] = v2;
				this._customVertexIndices[2] = v3;
				this._customVertexIndices[3] = v4;
				this._customVertexIndices[4] = v5;
				this._customVertexIndices[5] = v6;
				base.GameEntity.SetCustomVertexPositionEnabled(true);
			}

			// Token: 0x06001B2B RID: 6955 RVA: 0x000B2618 File Offset: 0x000B0818
			public void SetShipBridgeStartEndPositions(Vec3 startLeftPosition, Vec3 startRightPosition, Vec3 endLeftPosition, Vec3 endRightPosition)
			{
				this._startLeftPosition = startLeftPosition;
				this._startRightPosition = startRightPosition;
				this._endLeftPosition = endLeftPosition;
				this._endRightPosition = endRightPosition;
				this._rightVector = this._endRightPosition - this._startRightPosition;
				this._leftVector = this._endLeftPosition - this._startLeftPosition;
			}

			// Token: 0x06001B2C RID: 6956 RVA: 0x000B2670 File Offset: 0x000B0870
			protected override void OnDynamicNavmeshVertexUpdate()
			{
				float num = 0.25f;
				for (int i = 1; i < 4; i++)
				{
					Vec3 vec = this._startRightPosition + this._rightVector * num;
					Vec3 vec2 = this._startLeftPosition + this._leftVector * num;
					Vec3 vec3 = (vec + vec2) * 0.5f;
					Vec3 vec4 = (vec2 - vec) * 0.5f;
					this._bridgeCustomVertexPositionsArray[i - 1] = vec3 - vec4 * 0.8f;
					this._bridgeCustomVertexPositionsArray[i + 2] = vec3 + vec4 * 0.8f;
					num += 0.25f;
				}
				base.GameEntity.SetPositionsForAttachedNavmeshVertices(this._customVertexIndices, 6, this._bridgeCustomVertexPositionsArray);
			}

			// Token: 0x04000F92 RID: 3986
			private const float StepWidth = 0.8f;

			// Token: 0x04000F95 RID: 3989
			private Vec3 _startLeftPosition;

			// Token: 0x04000F96 RID: 3990
			private Vec3 _startRightPosition;

			// Token: 0x04000F97 RID: 3991
			private Vec3 _endLeftPosition;

			// Token: 0x04000F98 RID: 3992
			private Vec3 _endRightPosition;

			// Token: 0x04000F99 RID: 3993
			private int[] _customVertexIndices;

			// Token: 0x04000F9A RID: 3994
			private Vec3[] _bridgeCustomVertexPositionsArray;

			// Token: 0x04000F9B RID: 3995
			private PathFaceRecord _face1PathFaceRecord;

			// Token: 0x04000F9C RID: 3996
			private PathFaceRecord _face2PathFaceRecord;

			// Token: 0x04000F9D RID: 3997
			private Vec3 _rightVector;

			// Token: 0x04000F9E RID: 3998
			private Vec3 _leftVector;

			// Token: 0x04000F9F RID: 3999
			private int _attachedFaceCount;
		}

		// Token: 0x0200023C RID: 572
		public class ShipBridge : MissionObject
		{
		}

		// Token: 0x0200023D RID: 573
		public class ShipAttachmentJoint
		{
			// Token: 0x1700040A RID: 1034
			// (get) Token: 0x06001B2F RID: 6959 RVA: 0x000B2761 File Offset: 0x000B0961
			// (set) Token: 0x06001B30 RID: 6960 RVA: 0x000B2769 File Offset: 0x000B0969
			public float AccumulatedDistanceError { get; private set; }

			// Token: 0x1700040B RID: 1035
			// (get) Token: 0x06001B31 RID: 6961 RVA: 0x000B2772 File Offset: 0x000B0972
			// (set) Token: 0x06001B32 RID: 6962 RVA: 0x000B277A File Offset: 0x000B097A
			public float AccumulatedXYError { get; private set; }

			// Token: 0x1700040C RID: 1036
			// (get) Token: 0x06001B33 RID: 6963 RVA: 0x000B2783 File Offset: 0x000B0983
			// (set) Token: 0x06001B34 RID: 6964 RVA: 0x000B278B File Offset: 0x000B098B
			public float AccumulatedAlignmentError { get; private set; }

			// Token: 0x1700040D RID: 1037
			// (get) Token: 0x06001B35 RID: 6965 RVA: 0x000B2794 File Offset: 0x000B0994
			// (set) Token: 0x06001B36 RID: 6966 RVA: 0x000B279C File Offset: 0x000B099C
			public float CurrentXYError { get; private set; }

			// Token: 0x1700040E RID: 1038
			// (get) Token: 0x06001B37 RID: 6967 RVA: 0x000B27A5 File Offset: 0x000B09A5
			// (set) Token: 0x06001B38 RID: 6968 RVA: 0x000B27AD File Offset: 0x000B09AD
			public float CurrentAlignmentError { get; private set; }

			// Token: 0x1700040F RID: 1039
			// (get) Token: 0x06001B39 RID: 6969 RVA: 0x000B27B6 File Offset: 0x000B09B6
			// (set) Token: 0x06001B3A RID: 6970 RVA: 0x000B27BE File Offset: 0x000B09BE
			public bool IsBroken { get; private set; }

			// Token: 0x17000410 RID: 1040
			// (get) Token: 0x06001B3B RID: 6971 RVA: 0x000B27C7 File Offset: 0x000B09C7
			// (set) Token: 0x06001B3C RID: 6972 RVA: 0x000B27CF File Offset: 0x000B09CF
			public float CurrentDistanceError { get; private set; }

			// Token: 0x06001B3D RID: 6973 RVA: 0x000B27D8 File Offset: 0x000B09D8
			public ShipAttachmentJoint(ShipAttachmentMachine attachmentSource, ShipAttachmentPointMachine attachmentTarget, bool unbreakableJoint = false)
			{
				this._shipSource = GameEntity.CreateFromWeakEntity(attachmentSource.GameEntity.Root);
				this._shipTarget = GameEntity.CreateFromWeakEntity(attachmentTarget.GameEntity.Root);
				this._attachmentEntitySource = attachmentSource;
				this._attachmentEntityTarget = attachmentTarget;
				this._shipSourceScript = this._shipSource.GetFirstScriptOfType<MissionShip>();
				this._shipTargetScript = this._shipTarget.GetFirstScriptOfType<MissionShip>();
				this._unbreakableJoint = unbreakableJoint;
				this.InitializeJointParameters();
				this.UpdateRopeMinLength();
				this._currentPullSpeed = 0f;
				this._prevDistanceLambda = 0f;
				this._ropesPullDt = 0f;
				this._ropeStressSoundEvent = SoundEvent.CreateEvent(this.RopeStressSoundEventId, Mission.Current.Scene);
				this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			}

			// Token: 0x06001B3E RID: 6974 RVA: 0x000B28BC File Offset: 0x000B0ABC
			public void OnBreak()
			{
				if (this._currentAttachmentState == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopesPulling)
				{
					if (Agent.Main != null && Agent.Main.IsActive())
					{
						if (this._attachmentEntitySource.OwnerShip.GetIsAgentOnShip(Agent.Main, false))
						{
							string text = "event:/mission/movement/vessel/rope_snap";
							MatrixFrame matrixFrame = this._attachmentEntitySource.GameEntity.GetGlobalFrameImpreciseForFixedTick();
							SoundManager.StartOneShotEvent(text, ref matrixFrame.origin, "isPlayer", 1f);
						}
						else if (this._attachmentEntitySource.OwnerShip.GetIsAgentOnShip(Agent.Main, false))
						{
							string text2 = "event:/mission/movement/vessel/rope_snap";
							MatrixFrame matrixFrame = this._attachmentEntityTarget.GameEntity.GetGlobalFrameImpreciseForFixedTick();
							SoundManager.StartOneShotEvent(text2, ref matrixFrame.origin, "isPlayer", 1f);
						}
					}
					else
					{
						Vec3 vec = (this._attachmentEntityTarget.GameEntity.GetGlobalFrameImpreciseForFixedTick().origin + this._attachmentEntityTarget.GameEntity.GetGlobalFrameImpreciseForFixedTick().origin) * 0.5f;
						SoundManager.StartOneShotEvent("event:/mission/movement/vessel/rope_snap", ref vec, "isPlayer", 0f);
					}
					if (this._ropeStressSoundEvent != null)
					{
						this._ropeStressSoundEvent.Stop();
						this._ropeStressSoundEvent = null;
					}
				}
				this._navalShipsLogic.OnShipAttachmentLost(this._attachmentEntitySource.OwnerShip, this._attachmentEntityTarget.OwnerShip);
			}

			// Token: 0x06001B3F RID: 6975 RVA: 0x000B2A1C File Offset: 0x000B0C1C
			public void OnFixedTick(float fixedDt, ShipAttachmentMachine.ShipAttachment currentAttachment, ref float currentRopeLength)
			{
				if (this._attachmentEntitySource.IsShipAttachmentJointPhysicsEnabled)
				{
					this.StabilizeShipUps(15f);
					this.AlignShips();
					this.Update(fixedDt, ref currentRopeLength, currentAttachment);
					this.ReduceRelativeDrift(1f, 15f);
				}
				this.UpdateRopeLength(fixedDt, ref currentRopeLength, currentAttachment);
			}

			// Token: 0x06001B40 RID: 6976 RVA: 0x000B2A6C File Offset: 0x000B0C6C
			private void StabilizeShipUps(float correctionTorqueCoefficient)
			{
				int num = this._shipSourceScript.ComputeActiveShipAttachmentCount();
				int num2 = this._shipTargetScript.ComputeActiveShipAttachmentCount();
				Mat3 rotation = this._shipSource.GetBodyWorldTransform().rotation;
				Mat3 rotation2 = this._shipTarget.GetBodyWorldTransform().rotation;
				float mass = this._shipSourceScript.Physics.Mass;
				float mass2 = this._shipTargetScript.Physics.Mass;
				Vec3 u = rotation.u;
				Vec3 u2 = rotation2.u;
				Vec3 f = rotation.f;
				Vec3 f2 = rotation2.f;
				Vec3 vec = u.CrossProductWithUp() * (correctionTorqueCoefficient * mass * this._stiffness);
				Vec3 vec2 = u2.CrossProductWithUp() * (correctionTorqueCoefficient * mass2 * this._stiffness);
				vec = Vec3.DotProduct(vec, f) * f;
				vec2 = Vec3.DotProduct(vec2, f2) * f2;
				NavalPhysics physics = this._shipSourceScript.Physics;
				Vec3 vec3 = vec / (float)num;
				physics.ApplyTorque(in vec3, 0);
				NavalPhysics physics2 = this._shipTargetScript.Physics;
				vec3 = vec2 / (float)num2;
				physics2.ApplyTorque(in vec3, 0);
			}

			// Token: 0x06001B41 RID: 6977 RVA: 0x000B2B88 File Offset: 0x000B0D88
			public void UpdateRopeMinLength()
			{
				this._attachmentEntitySource.RopeMinLength = ShipAttachmentMachine.ShipAttachmentJoint.CalculatePossibleRopeMinLength(this._attachmentEntitySource, this._attachmentEntityTarget);
				if (this._attachmentEntitySource.BridgeConnectionLengthSquared < this._attachmentEntitySource.RopeMinLength * this._attachmentEntitySource.RopeMinLength)
				{
					float num = this._attachmentEntitySource.RopeMinLength + 1f;
					this._attachmentEntitySource.BridgeConnectionLengthSquared = num * num;
				}
			}

			// Token: 0x06001B42 RID: 6978 RVA: 0x000B2BF5 File Offset: 0x000B0DF5
			public static float CalculatePossibleBridgeConnectionLengthSquared(ShipAttachmentMachine attachmentMachine, ShipAttachmentPointMachine attachmentPointMachine)
			{
				float num = ShipAttachmentMachine.ShipAttachmentJoint.CalculatePossibleRopeMinLength(attachmentMachine, attachmentPointMachine) + 2.5f;
				return num * num;
			}

			// Token: 0x06001B43 RID: 6979 RVA: 0x000B2C08 File Offset: 0x000B0E08
			public static float CalculatePossibleRopeMinLength(ShipAttachmentMachine attachmentMachine, ShipAttachmentPointMachine attachmentPointMachine)
			{
				MissionShip ownerShip = attachmentMachine.OwnerShip;
				MissionShip ownerShip2 = attachmentPointMachine.OwnerShip;
				MatrixFrame globalFrame = ownerShip.GameEntity.GetGlobalFrame();
				MatrixFrame globalFrame2 = ownerShip2.GameEntity.GetGlobalFrame();
				Vec3 origin = attachmentMachine.ConnectionClipPlaneEntity.GetGlobalFrame().origin;
				Vec3 origin2 = attachmentPointMachine.ConnectionClipPlaneEntity.GetGlobalFrame().origin;
				MatrixFrame globalFrame3 = attachmentMachine.GameEntity.GetGlobalFrame();
				MatrixFrame globalFrame4 = attachmentPointMachine.GameEntity.GetGlobalFrame();
				float num = Vec3.DotProduct(globalFrame.rotation.f, globalFrame2.rotation.f);
				Vec3 vec = globalFrame.TransformToLocal(ref origin);
				Vec3 vec2 = globalFrame2.TransformToLocal(ref origin2);
				float num2 = vec.z - ownerShip.Physics.StabilitySubmergedHeightOfShip;
				float num3 = vec2.z - ownerShip2.Physics.StabilitySubmergedHeightOfShip;
				float num4 = MathF.Abs(num2 - num3);
				MatrixFrame matrixFrame = globalFrame.TransformToLocal(ref globalFrame3);
				MatrixFrame matrixFrame2 = globalFrame2.TransformToLocal(ref globalFrame4);
				Vec2[] localPhysicsBoundingBoxXYPlaneVertices = ownerShip.GetLocalPhysicsBoundingBoxXYPlaneVertices(0.9f);
				Vec2[] localPhysicsBoundingBoxXYPlaneVertices2 = ownerShip2.GetLocalPhysicsBoundingBoxXYPlaneVertices(0.9f);
				float num5 = Vec2.DotProduct(globalFrame3.rotation.f.AsVec2, globalFrame.rotation.s.AsVec2);
				float num6 = Vec2.DotProduct(globalFrame4.rotation.f.AsVec2, globalFrame2.rotation.s.AsVec2);
				Vec2 vec3 = ((num5 > 0f) ? localPhysicsBoundingBoxXYPlaneVertices[3] : localPhysicsBoundingBoxXYPlaneVertices[0]);
				Vec2 vec4 = ((num5 > 0f) ? localPhysicsBoundingBoxXYPlaneVertices[2] : localPhysicsBoundingBoxXYPlaneVertices[1]);
				Vec2 vec5 = ((num5 > 0f) ? Vec2.Side : (-Vec2.Side));
				Vec2 vec6 = ((num6 > 0f) ? localPhysicsBoundingBoxXYPlaneVertices2[3] : localPhysicsBoundingBoxXYPlaneVertices2[0]);
				Vec2 vec7 = ((num6 > 0f) ? localPhysicsBoundingBoxXYPlaneVertices2[2] : localPhysicsBoundingBoxXYPlaneVertices2[1]);
				Vec2 vec8 = ((num6 > 0f) ? Vec2.Side : (-Vec2.Side));
				float num7;
				Vec2 vec9;
				MBMath.CheckLineToLineSegmentIntersection(matrixFrame.origin.AsVec2, vec5, vec3, vec4, ref num7, ref vec9);
				float num8;
				Vec2 vec10;
				MBMath.CheckLineToLineSegmentIntersection(matrixFrame2.origin.AsVec2, vec8, vec6, vec7, ref num8, ref vec10);
				float num9 = num7;
				float num10 = num8;
				float num11 = MathF.Abs(vec3.y - vec4.y);
				float num12 = (vec.y - vec3.y) / num11;
				float num13 = MathF.Abs(vec6.y - vec7.y);
				float num14 = (vec2.y - vec6.y) / num13;
				if (num < 0f)
				{
					num14 = 1f - num14;
				}
				float num15 = MathF.Abs(num12 - num14);
				float num16 = 1.5f + (num9 + num10) * (1f - num15);
				return MathF.Sqrt(num16 * num16 + num4 * num4);
			}

			// Token: 0x06001B44 RID: 6980 RVA: 0x000B2EE8 File Offset: 0x000B10E8
			public void InitializeJointParameters()
			{
				this._age = 0f;
				this._stiffness = 0f;
				this.AccumulatedDistanceError = 0f;
				this.AccumulatedXYError = 0f;
				this.AccumulatedAlignmentError = 0f;
				this.CurrentDistanceError = 0f;
				this.CurrentXYError = 0f;
				this.CurrentAlignmentError = 0f;
				this.IsBroken = false;
				this._ropeLeftoverImpulse = new Vec3(0f, 0f, 0f, -1f);
				this._bridgeDirectionLeftoverImpulse = new Vec3(0f, 0f, 0f, -1f);
				this._bridgeAlignmentLeftoverImpulse = new Vec3(0f, 0f, 0f, -1f);
				this._bridgeXYLeftoverImpulse = new Vec3(0f, 0f, 0f, -1f);
			}

			// Token: 0x06001B45 RID: 6981 RVA: 0x000B2FD0 File Offset: 0x000B11D0
			private void SmoothApproachRopeLength(float dt, ref float currentLength, float target)
			{
				this._ropesPullDt += dt;
				float num = MathF.Sin(this._ropesPullDt * 2f * 3.1415927f * 1f) * 0.5f + 0.5f;
				float num2 = 0.25f * (1f + 0.6f * num);
				this._currentPullSpeed = MathF.Min(this._currentPullSpeed + num2 * dt, 0.65f);
				float num3 = this._currentPullSpeed * dt;
				currentLength = Math.Max(target, currentLength - num3);
			}

			// Token: 0x06001B46 RID: 6982 RVA: 0x000B3058 File Offset: 0x000B1258
			private void UpdateRopeLength(float fixedDt, ref float currentRopeLength, ShipAttachmentMachine.ShipAttachment currentAttachment)
			{
				if (currentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopesPulling)
				{
					float currentDistanceError = this.CurrentDistanceError;
					float num = 10f;
					if (currentDistanceError > num * 0.75f)
					{
						float num2 = currentDistanceError / num;
						currentRopeLength = Math.Max(this._attachmentEntitySource.RopeMinLength, currentRopeLength + 0.05f * num2 * fixedDt);
						this._currentPullSpeed = 0f;
						return;
					}
					float ropeMinLength = this._attachmentEntitySource.RopeMinLength;
					this.SmoothApproachRopeLength(fixedDt, ref currentRopeLength, ropeMinLength);
					return;
				}
				else
				{
					if (currentRopeLength < this._attachmentEntitySource.RopeMinLength)
					{
						currentRopeLength = MathF.Min(this._attachmentEntitySource.RopeMinLength, currentRopeLength + 0.25f * fixedDt);
						return;
					}
					currentRopeLength = Math.Max(this._attachmentEntitySource.RopeMinLength, currentRopeLength - 0.25f * fixedDt);
					return;
				}
			}

			// Token: 0x06001B47 RID: 6983 RVA: 0x000B3114 File Offset: 0x000B1314
			private void Update(float fixedDt, ref float currentRopeLength, ShipAttachmentMachine.ShipAttachment currentAttachment)
			{
				if (!this.IsBroken)
				{
					this._ropeLeftoverImpulse *= 0.9f;
					this._bridgeDirectionLeftoverImpulse *= 0.9f;
					this._bridgeAlignmentLeftoverImpulse *= 0.9f;
					this._bridgeXYLeftoverImpulse *= 0.9f;
					if (currentAttachment.State != this._currentAttachmentState)
					{
						if (this._ropeStressSoundEvent != null && this._currentAttachmentState != ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopesPulling)
						{
							this._ropeStressSoundEvent.Stop();
							this._ropeStressSoundEvent = null;
						}
						this.InitializeJointParameters();
						this._currentAttachmentState = currentAttachment.State;
						if (this._currentAttachmentState == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected)
						{
							this._navalShipsLogic.OnBridgeConnected(this._shipSourceScript, this._shipTargetScript);
						}
					}
					this._age += fixedDt;
					this._stiffness = MathF.Min(this._age / 5f, 1f);
					this.CurrentDistanceError = 0f;
					this.CurrentXYError = 0f;
					this.CurrentAlignmentError = 0f;
					MatrixFrame globalMassFrame = this._shipSourceScript.Physics.GetGlobalMassFrame();
					MatrixFrame globalMassFrame2 = this._shipTargetScript.Physics.GetGlobalMassFrame();
					Vec3 origin = this._attachmentEntitySource.ConnectionClipPlaneEntity.GetGlobalFrameImpreciseForFixedTick().origin;
					Vec3 origin2 = this._attachmentEntityTarget.ConnectionClipPlaneEntity.GetGlobalFrameImpreciseForFixedTick().origin;
					Vec3 linearVelocityAtGlobalPointForEntityWithDynamicBody = GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(this._shipSource, origin);
					Vec3 vec = GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(this._shipTarget, origin2) - linearVelocityAtGlobalPointForEntityWithDynamicBody;
					float mass = this._shipSourceScript.Physics.Mass;
					float mass2 = this._shipTargetScript.Physics.Mass;
					if (this._currentAttachmentState == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopesPulling)
					{
						this.UpdateRopeConstraint(fixedDt, currentRopeLength, globalMassFrame, globalMassFrame2, origin, origin2, mass, mass2, vec);
					}
					else if (this._currentAttachmentState == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected)
					{
						this.UpdateBridgeConstraints(fixedDt, currentRopeLength, globalMassFrame, globalMassFrame2, origin, origin2, mass, mass2, vec);
					}
					else if (this._currentAttachmentState == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeThrown)
					{
						this.UpdateBridgeConstraints(fixedDt, currentRopeLength, globalMassFrame, globalMassFrame2, origin, origin2, mass, mass2, vec);
					}
					if (!this._unbreakableJoint)
					{
						this.CheckBreaking(fixedDt, currentAttachment);
					}
				}
			}

			// Token: 0x06001B48 RID: 6984 RVA: 0x000B3334 File Offset: 0x000B1534
			private void AlignShips()
			{
				Mat3 rotation = this._shipSource.GetBodyWorldTransform().rotation;
				Mat3 rotation2 = this._shipTarget.GetBodyWorldTransform().rotation;
				float num = MathF.Atan2(rotation.f.y, rotation.f.x);
				float num2 = MathF.Atan2(rotation2.f.y, rotation2.f.x) - num;
				num2 = MBMath.WrapAngle(num2);
				if (MathF.Abs(num2) > 1.5707964f)
				{
					if (num2 > 0f)
					{
						num2 -= 3.1415927f;
					}
					else
					{
						num2 += 3.1415927f;
					}
				}
				if (MathF.Abs(num2) >= 0.017f)
				{
					int num3 = this._shipSourceScript.ComputeActiveShipAttachmentCount();
					int num4 = this._shipTargetScript.ComputeActiveShipAttachmentCount();
					float num5 = num2 * 0.5f;
					float num6 = -num2 * 0.5f;
					float num7 = (this._shipSourceScript.Physics.Mass + this._shipTargetScript.Physics.Mass) * 0.5f;
					float num8 = num5 * num7 * 25f * this._stiffness;
					float num9 = num6 * num7 * 25f * this._stiffness;
					num8 -= this._shipSourceScript.Physics.AngularVelocity.z * num7 * 50f;
					num9 -= this._shipTargetScript.Physics.AngularVelocity.z * num7 * 50f;
					float num10 = ((this._currentAttachmentState != ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopesPulling) ? 1f : 0.25f);
					NavalPhysics physics = this._shipSourceScript.Physics;
					Vec3 vec = new Vec3(0f, 0f, num8 / (float)num3 * num10, -1f);
					physics.ApplyTorque(in vec, 0);
					NavalPhysics physics2 = this._shipTargetScript.Physics;
					vec = new Vec3(0f, 0f, num9 / (float)num4 * num10, -1f);
					physics2.ApplyTorque(in vec, 0);
				}
			}

			// Token: 0x06001B49 RID: 6985 RVA: 0x000B3518 File Offset: 0x000B1718
			private void UpdateRopeConstraint(float fixedDt, float currentRopeLength, MatrixFrame shipSourceGlobalFrame, MatrixFrame shipTargetGlobalFrame, Vec3 sourceAttachmentPosition, Vec3 targetAttachmentPosition, float sourceShipMass, float targetShipMass, Vec3 relativeVelocityVector)
			{
				Vec3 vec = targetAttachmentPosition - sourceAttachmentPosition;
				if (vec.LengthSquared > currentRopeLength * currentRopeLength)
				{
					float num = vec.Normalize();
					float num2 = Vec3.DotProduct(relativeVelocityVector, vec);
					float num3 = num - currentRopeLength;
					this.CurrentDistanceError = num3;
					float num4 = 2f;
					float num5 = MathF.Clamp(num3 / num4, 0f, 1f);
					float num6 = MBMath.SmoothStep(0f, num4, num5);
					num6 = (float)MathF.Sign(num3) * num6;
					if (this._ropeStressSoundEvent != null)
					{
						if (num5 > 2f)
						{
							if (!this._ropeStressSoundEvent.IsPlaying())
							{
								this._ropeStressSoundEvent.Play();
							}
							else if (this._ropeStressSoundEvent.IsPaused())
							{
								this._ropeStressSoundEvent.Resume();
							}
							this._ropeStressSoundEvent.SetPosition((this._attachmentEntitySource.GameEntity.GetGlobalFrameImpreciseForFixedTick().origin + this._attachmentEntityTarget.GameEntity.GetGlobalFrameImpreciseForFixedTick().origin) * 0.5f);
						}
						else if (this._ropeStressSoundEvent.IsPlaying())
						{
							this._ropeStressSoundEvent.Pause();
						}
					}
					float num7 = sourceShipMass * targetShipMass / (sourceShipMass + targetShipMass);
					float num8 = 0.1f * this._stiffness;
					float num9 = 0.1f;
					float num10 = MathF.Min(this.CurrentDistanceError / 10f, 1f);
					float num11 = sourceShipMass + targetShipMass;
					float num12 = MathF.Min(sourceShipMass, targetShipMass) * 2f / num11;
					float num13 = MathF.Lerp(1.2f, 5f, num10 * (1f - num12), 1E-05f);
					float num14 = this.SolveImpulseConstraint(num2, num6, num7, num8, num9, fixedDt);
					num14 = MathF.Abs(num14) * (float)MathF.Sign(num6);
					float num15 = MathF.Lerp(this._prevDistanceLambda, num14, fixedDt * 2f, 1E-05f);
					this._prevDistanceLambda = num15;
					this.ApplyConstraintImpulse(vec * num15, shipSourceGlobalFrame, shipTargetGlobalFrame, sourceAttachmentPosition, targetAttachmentPosition, num13, sourceShipMass, targetShipMass, fixedDt, ref this._ropeLeftoverImpulse);
				}
			}

			// Token: 0x06001B4A RID: 6986 RVA: 0x000B371C File Offset: 0x000B191C
			public float SolveSpringMassSystemFromTargetPeriod(float dt, float reducedMass, float targetPeriod, float dampingRatio, float distance, float relativeSpeed)
			{
				float num = 6.2831855f / targetPeriod;
				float num2 = reducedMass * num * num;
				float num3 = 2f * reducedMass * dampingRatio * num;
				return (-num2 * distance - num3 * relativeSpeed) * dt;
			}

			// Token: 0x06001B4B RID: 6987 RVA: 0x000B3750 File Offset: 0x000B1950
			private void UpdateBridgeConstraints(float dt, float currentRopeLength, MatrixFrame shipSourceGlobalFrame, MatrixFrame shipTargetGlobalFrame, Vec3 sourceAttachmentPosition, Vec3 targetAttachmentPosition, float sourceShipMass, float targetShipMass, Vec3 relativeVelocityVector)
			{
				float num = sourceShipMass * targetShipMass / (sourceShipMass + targetShipMass);
				Vec3 vec = targetAttachmentPosition - sourceAttachmentPosition;
				float num2 = vec.Normalize() - currentRopeLength;
				this.CurrentDistanceError = num2;
				float num3 = Vec3.DotProduct(relativeVelocityVector, vec);
				float num4 = this.SolveSpringMassSystemFromTargetPeriod(dt, num, 2f, 0.3f, num2, num3);
				float num5 = MathF.Lerp(0.01f, 5f, MathF.Min(1f, MathF.Abs(num2)), 1E-05f);
				this.ApplyConstraintImpulse(-num4 * vec * this._stiffness, shipSourceGlobalFrame, shipTargetGlobalFrame, sourceAttachmentPosition, targetAttachmentPosition, num5, sourceShipMass, targetShipMass, dt, ref this._bridgeDirectionLeftoverImpulse);
				float num6 = Vec3.DotProduct(shipSourceGlobalFrame.rotation.f, shipTargetGlobalFrame.rotation.f);
				Vec3 vec2 = shipTargetGlobalFrame.rotation.f;
				if (num6 < 1E-05f)
				{
					vec2 = -1f * shipTargetGlobalFrame.rotation.f;
				}
				Vec3 vec3 = (shipSourceGlobalFrame.rotation.f.AsVec2.Normalized() + vec2.AsVec2.Normalized()).Normalized().ToVec3(0f);
				float num7 = Vec3.DotProduct(vec, vec3);
				this.CurrentAlignmentError = num7;
				float num8 = Vec3.DotProduct(relativeVelocityVector, vec3);
				float num9 = this.SolveSpringMassSystemFromTargetPeriod(dt, num, 1.75f, 0.8f, num7, num8);
				float num10 = MathF.Lerp(0.01f, 5f, MathF.Min(1f, MathF.Abs(num7)), 1E-05f);
				this.ApplyConstraintImpulse(-num9 * vec3 * this._stiffness, shipSourceGlobalFrame, shipTargetGlobalFrame, sourceAttachmentPosition, targetAttachmentPosition, num10, sourceShipMass, targetShipMass, dt, ref this._bridgeAlignmentLeftoverImpulse);
				Vec3 vec4 = targetAttachmentPosition - sourceAttachmentPosition;
				Vec2 vec5;
				vec5..ctor(vec4.x, vec4.y);
				float num11 = vec5.Normalize();
				float num12 = currentRopeLength * MathF.Sin(1.134464f) - num11;
				this.CurrentXYError = num12;
				if (num12 > 0f)
				{
					float num13 = Vec2.DotProduct(relativeVelocityVector.AsVec2, vec5);
					float num14 = this.SolveSpringMassSystemFromTargetPeriod(dt, num, 0.75f, 0.5f, num12, -num13);
					float num15 = MathF.Lerp(0.01f, 15f, MathF.Min(1f, MathF.Abs(num12)), 1E-05f);
					this.ApplyConstraintImpulse(num14 * vec5.ToVec3(0f) * this._stiffness, shipSourceGlobalFrame, shipTargetGlobalFrame, sourceAttachmentPosition, targetAttachmentPosition, num15, sourceShipMass, targetShipMass, dt, ref this._bridgeXYLeftoverImpulse);
				}
			}

			// Token: 0x06001B4C RID: 6988 RVA: 0x000B39E9 File Offset: 0x000B1BE9
			private float SolveImpulseConstraint(float relativeVelocity, float positionError, float reducedMass, float beta, float damping, float fixedDt)
			{
				return (-(beta / fixedDt) * positionError - damping * relativeVelocity) * reducedMass;
			}

			// Token: 0x06001B4D RID: 6989 RVA: 0x000B39FC File Offset: 0x000B1BFC
			private void ApplyConstraintImpulse(Vec3 impulse, MatrixFrame shipSourceGlobalFrame, MatrixFrame shipTargetGlobalFrame, Vec3 attachmentSourceGlobalPosition, Vec3 attachmentTargetGlobalPosition, float maxAcceleration, float sourceShipMass, float targetShipMass, float fixedDt, ref Vec3 leftoverImpulse)
			{
				float num = impulse.Normalize();
				Vec3 vec = impulse;
				float num2 = MathF.Abs(num);
				float num3 = sourceShipMass * maxAcceleration * fixedDt;
				float num4 = targetShipMass * maxAcceleration * fixedDt;
				float num5 = MathF.Min(num3, num4);
				float num6 = MathF.Min(num2, num5);
				float num7 = num6 * (float)MathF.Sign(num);
				float num8 = num2 - num6;
				leftoverImpulse += num8 * 0.5f * vec;
				Vec3 vec2 = vec * num7;
				Vec3 vec3 = -vec2;
				NavalPhysics physics = this._shipSourceScript.Physics;
				Vec3 vec4 = shipSourceGlobalFrame.TransformToLocal(ref attachmentSourceGlobalPosition);
				physics.ApplyGlobalForceAtLocalPos(in vec4, in vec2, 1);
				NavalPhysics physics2 = this._shipTargetScript.Physics;
				vec4 = shipTargetGlobalFrame.TransformToLocal(ref attachmentTargetGlobalPosition);
				physics2.ApplyGlobalForceAtLocalPos(in vec4, in vec3, 1);
			}

			// Token: 0x06001B4E RID: 6990 RVA: 0x000B3AC0 File Offset: 0x000B1CC0
			private void CheckBreaking(float dt, ShipAttachmentMachine.ShipAttachment currentAttachment)
			{
				float num = ((this._currentAttachmentState == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeThrown || this._currentAttachmentState == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected) ? 5f : 10f);
				if (this.CurrentDistanceError > num * 0.5f)
				{
					this.AccumulatedDistanceError += this.CurrentDistanceError * 4f * dt;
					if (this.CurrentDistanceError > num || this.AccumulatedDistanceError > num)
					{
						this.IsBroken = true;
					}
				}
				if (this.CurrentAlignmentError > 0.95f)
				{
					this.AccumulatedAlignmentError += this.CurrentAlignmentError * 4f * dt;
					if (this.AccumulatedAlignmentError > 20f)
					{
						this.IsBroken = true;
					}
				}
				if (this.CurrentXYError > 2.0625f)
				{
					this.AccumulatedXYError += this.CurrentXYError * 4f * dt;
					if (this.CurrentXYError > 2.75f || this.AccumulatedXYError > 2.75f)
					{
						this.IsBroken = true;
					}
				}
				if (this.IsBroken)
				{
					this.OnBreak();
				}
			}

			// Token: 0x06001B4F RID: 6991 RVA: 0x000B3BC4 File Offset: 0x000B1DC4
			private void ReduceRelativeDrift(float linearDamping, float angularDamping)
			{
				int num = this._shipSourceScript.ComputeActiveShipAttachmentCount();
				int num2 = this._shipTargetScript.ComputeActiveShipAttachmentCount();
				int num3 = num + num2;
				Vec3 linearVelocity = this._shipSourceScript.Physics.LinearVelocity;
				Vec3 linearVelocity2 = this._shipTargetScript.Physics.LinearVelocity;
				Vec3 angularVelocity = this._shipSourceScript.Physics.AngularVelocity;
				Vec3 angularVelocity2 = this._shipTargetScript.Physics.AngularVelocity;
				float mass = this._shipSourceScript.Physics.Mass;
				float mass2 = this._shipTargetScript.Physics.Mass;
				Vec2 vec = (linearVelocity.AsVec2 * mass + linearVelocity2.AsVec2 * mass2) / (mass + mass2);
				Vec2 vec2 = vec * mass;
				Vec2 vec3 = vec * mass2;
				float num4 = 2f * mass * 9.806f;
				float num5 = 2f * mass2 * 9.806f;
				vec2.ClampMagnitude(0f, num4);
				vec3.ClampMagnitude(0f, num5);
				NavalPhysics physics = this._shipSourceScript.Physics;
				Vec3 vec4 = (-vec2 * linearDamping * this._stiffness / (float)num3).ToVec3(0f);
				physics.ApplyForceToDynamicBody(in vec4, 0);
				NavalPhysics physics2 = this._shipTargetScript.Physics;
				vec4 = (-vec3 * linearDamping * this._stiffness / (float)num3).ToVec3(0f);
				physics2.ApplyForceToDynamicBody(in vec4, 0);
				float num6 = (angularVelocity.z * mass + angularVelocity2.z * mass2) / (mass + mass2);
				if (num6 != 0f)
				{
					float num7 = num6 * mass;
					float num8 = num6 * mass2;
					float num9 = 0.34906587f * mass;
					float num10 = 0.34906587f * mass2;
					num7 = MathF.Clamp(num7, -num9, num9);
					num8 = MathF.Clamp(num8, -num10, num10);
					NavalPhysics physics3 = this._shipSourceScript.Physics;
					vec4 = new Vec3(0f, 0f, -num7, -1f) * angularDamping * this._stiffness / (float)num3;
					physics3.ApplyTorque(in vec4, 0);
					NavalPhysics physics4 = this._shipTargetScript.Physics;
					vec4 = new Vec3(0f, 0f, -num8, -1f) * angularDamping * this._stiffness / (float)num3;
					physics4.ApplyTorque(in vec4, 0);
				}
			}

			// Token: 0x04000FA0 RID: 4000
			private const string RopeSnapSoundEvent = "event:/mission/movement/vessel/rope_snap";

			// Token: 0x04000FA1 RID: 4001
			private const float LeftoverImpulseDecay = 0.9f;

			// Token: 0x04000FA2 RID: 4002
			private readonly int RopeStressSoundEventId = SoundManager.GetEventGlobalIndex("event:/mission/movement/vessel/rope_stress");

			// Token: 0x04000FAA RID: 4010
			private readonly GameEntity _shipSource;

			// Token: 0x04000FAB RID: 4011
			private readonly GameEntity _shipTarget;

			// Token: 0x04000FAC RID: 4012
			private readonly MissionShip _shipSourceScript;

			// Token: 0x04000FAD RID: 4013
			private readonly MissionShip _shipTargetScript;

			// Token: 0x04000FAE RID: 4014
			private readonly ShipAttachmentMachine _attachmentEntitySource;

			// Token: 0x04000FAF RID: 4015
			private readonly ShipAttachmentPointMachine _attachmentEntityTarget;

			// Token: 0x04000FB0 RID: 4016
			private float _age;

			// Token: 0x04000FB1 RID: 4017
			private float _stiffness;

			// Token: 0x04000FB2 RID: 4018
			private bool _unbreakableJoint;

			// Token: 0x04000FB3 RID: 4019
			private Vec3 _ropeLeftoverImpulse;

			// Token: 0x04000FB4 RID: 4020
			private Vec3 _bridgeDirectionLeftoverImpulse;

			// Token: 0x04000FB5 RID: 4021
			private Vec3 _bridgeAlignmentLeftoverImpulse;

			// Token: 0x04000FB6 RID: 4022
			private Vec3 _bridgeXYLeftoverImpulse;

			// Token: 0x04000FB7 RID: 4023
			private ShipAttachmentMachine.ShipAttachment.ShipAttachmentState _currentAttachmentState;

			// Token: 0x04000FB8 RID: 4024
			private float _currentPullSpeed;

			// Token: 0x04000FB9 RID: 4025
			private float _prevDistanceLambda;

			// Token: 0x04000FBA RID: 4026
			private float _ropesPullDt;

			// Token: 0x04000FBB RID: 4027
			private NavalShipsLogic _navalShipsLogic;

			// Token: 0x04000FBC RID: 4028
			private SoundEvent _ropeStressSoundEvent;
		}

		// Token: 0x0200023E RID: 574
		public class ShipAttachment
		{
			// Token: 0x17000411 RID: 1041
			// (get) Token: 0x06001B50 RID: 6992 RVA: 0x000B3E3E File Offset: 0x000B203E
			// (set) Token: 0x06001B51 RID: 6993 RVA: 0x000B3E46 File Offset: 0x000B2046
			public ShipAttachmentMachine AttachmentSource { get; private set; }

			// Token: 0x17000412 RID: 1042
			// (get) Token: 0x06001B52 RID: 6994 RVA: 0x000B3E4F File Offset: 0x000B204F
			// (set) Token: 0x06001B53 RID: 6995 RVA: 0x000B3E57 File Offset: 0x000B2057
			public ShipAttachmentPointMachine AttachmentTarget { get; private set; }

			// Token: 0x17000413 RID: 1043
			// (get) Token: 0x06001B54 RID: 6996 RVA: 0x000B3E60 File Offset: 0x000B2060
			// (set) Token: 0x06001B55 RID: 6997 RVA: 0x000B3E68 File Offset: 0x000B2068
			public Vec3 CommittedWeightedPosition { get; private set; }

			// Token: 0x17000414 RID: 1044
			// (get) Token: 0x06001B56 RID: 6998 RVA: 0x000B3E71 File Offset: 0x000B2071
			// (set) Token: 0x06001B57 RID: 6999 RVA: 0x000B3E79 File Offset: 0x000B2079
			public float CommittedTotalMass { get; private set; }

			// Token: 0x17000415 RID: 1045
			// (get) Token: 0x06001B58 RID: 7000 RVA: 0x000B3E82 File Offset: 0x000B2082
			// (set) Token: 0x06001B59 RID: 7001 RVA: 0x000B3E8A File Offset: 0x000B208A
			public float CommittedAgentCount { get; private set; }

			// Token: 0x17000416 RID: 1046
			// (get) Token: 0x06001B5A RID: 7002 RVA: 0x000B3E93 File Offset: 0x000B2093
			// (set) Token: 0x06001B5B RID: 7003 RVA: 0x000B3E9B File Offset: 0x000B209B
			public bool BridgeConnectionInteractionDistanceCheck { get; private set; }

			// Token: 0x17000417 RID: 1047
			// (get) Token: 0x06001B5C RID: 7004 RVA: 0x000B3EA4 File Offset: 0x000B20A4
			public ShipAttachmentMachine.ShipAttachment.ShipAttachmentState State
			{
				get
				{
					return this._state;
				}
			}

			// Token: 0x17000418 RID: 1048
			// (get) Token: 0x06001B5D RID: 7005 RVA: 0x000B3EAC File Offset: 0x000B20AC
			public MatrixFrame HookGlobalFrame
			{
				get
				{
					return this._hookGlobalFrame;
				}
			}

			// Token: 0x17000419 RID: 1049
			// (get) Token: 0x06001B5E RID: 7006 RVA: 0x000B3EB4 File Offset: 0x000B20B4
			public bool IsNavmeshConnected
			{
				get
				{
					return this._state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected && this._faceSwapSideOneDone && this._faceSwapSideTwoDone;
				}
			}

			// Token: 0x1700041A RID: 1050
			// (get) Token: 0x06001B5F RID: 7007 RVA: 0x000B3ECF File Offset: 0x000B20CF
			// (set) Token: 0x06001B60 RID: 7008 RVA: 0x000B3ED7 File Offset: 0x000B20D7
			public bool ShipIslandsConnected { get; private set; } = true;

			// Token: 0x1700041B RID: 1051
			// (get) Token: 0x06001B61 RID: 7009 RVA: 0x000B3EE0 File Offset: 0x000B20E0
			// (set) Token: 0x06001B62 RID: 7010 RVA: 0x000B3EE8 File Offset: 0x000B20E8
			public ShipAttachmentMachine.ShipAttachmentJoint ShipAttachmentJoint { get; private set; }

			// Token: 0x06001B63 RID: 7011 RVA: 0x000B3EF1 File Offset: 0x000B20F1
			public void ClearCommittedAgentInformation()
			{
				this.CommittedTotalMass = 0f;
				this.CommittedWeightedPosition = Vec3.Zero;
				this.CommittedAgentCount = 0f;
			}

			// Token: 0x06001B64 RID: 7012 RVA: 0x000B3F14 File Offset: 0x000B2114
			public void SetAttachmentState(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState state)
			{
				if (this._state != state)
				{
					ShipAttachmentMachine.ShipAttachment.ShipAttachmentState state2 = this._state;
					this._state = state;
					this.UpdateAttachmentMachineEntityVisibilities(state2);
					if (state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BrokenAndWaitingForRemoval)
					{
						this.AttachmentSource.OwnerShip.ShipsLogic.OnAttachmentBroken(this.AttachmentSource, this.AttachmentTarget);
					}
				}
			}

			// Token: 0x06001B65 RID: 7013 RVA: 0x000B3F64 File Offset: 0x000B2164
			public ShipAttachment(ShipAttachmentMachine attachmentSource, ShipAttachmentPointMachine attachmentTarget, in Vec3 globalPosition, in Vec3 globalDirection, bool bridgeConnectionInteractionDistanceCheck = true, bool attachmentInitializedByPlayer = false)
			{
				this._state = ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopeThrown;
				this.AttachmentSource = attachmentSource;
				this.AttachmentTarget = attachmentTarget;
				this._ropesPullingTimer = new MissionTimer(30f);
				this._shipBetweenAttachmentsCheckTimer = 0.1f;
				this._attachmentInitializedByPlayer = attachmentInitializedByPlayer;
				this.BridgeConnectionInteractionDistanceCheck = bridgeConnectionInteractionDistanceCheck;
				if (this.AttachmentTarget != null)
				{
					MatrixFrame globalFrame = this.AttachmentTarget.GameEntity.GetGlobalFrame();
					Vec3 hookAttachLocalPosition = this.AttachmentTarget.HookAttachLocalPosition;
					Vec3 vec = globalFrame.TransformToParent(ref hookAttachLocalPosition);
					this.InitializeRopeFlightDataAccordingToTargetPoint(in globalPosition, in vec);
				}
				else
				{
					this.InitializeRopeFlightDataAccordingToTargetDirection(in globalPosition, in globalDirection);
				}
				this.AttachmentSource.RopeVisual.GameEntity.SetVisibilityExcludeParents(true);
				SoundManager.StartOneShotEvent("event:/mission/movement/vessel/hook_throw", ref globalPosition);
				this.SpawnPlankEntities();
				this._woodPhysicsMaterialCached = PhysicsMaterial.GetFromName("wood_nonstick");
				this._defaultPhysicsMaterialCached = PhysicsMaterial.GetFromName("default");
				this._currentFramePlankPhysicsVerticesPinnedGCHandler = GCHandle.Alloc(this._currentFramePlankPhysicsVertices, GCHandleType.Pinned);
				this._currentFramePlankPhysicsVerticesPinnedPointer = (UIntPtr)((ulong)(long)this._currentFramePlankPhysicsVerticesPinnedGCHandler.AddrOfPinnedObject());
				this._currentFramePlankPhysicsIndicesPinnedGCHandler = GCHandle.Alloc(this._currentFramePlankPhysicsIndices, GCHandleType.Pinned);
				this._currentFramePlankPhysicsIndicesPinnedPointer = (UIntPtr)((ulong)(long)this._currentFramePlankPhysicsIndicesPinnedGCHandler.AddrOfPinnedObject());
				this._sideBarriersQuadPinnedGCHandler = GCHandle.Alloc(this._sideBarrierQuadsCached, GCHandleType.Pinned);
				this._sideBarriersQuadPinnedPointer = (UIntPtr)((ulong)(long)this._sideBarriersQuadPinnedGCHandler.AddrOfPinnedObject());
				this._sideBarriersIndicesPinnedGCHandler = GCHandle.Alloc(this._sideBarrierIndicesCached, GCHandleType.Pinned);
				this._sideBarriersIndicesPinnedPointer = (UIntPtr)((ulong)(long)this._sideBarriersIndicesPinnedGCHandler.AddrOfPinnedObject());
				this._vFoldQuadPinnedGCHandler = GCHandle.Alloc(this._vFoldQuadsCached, GCHandleType.Pinned);
				this._vFoldQuadPinnedPointer = (UIntPtr)((ulong)(long)this._vFoldQuadPinnedGCHandler.AddrOfPinnedObject());
				this._vFoldIndicesPinnedGCHandler = GCHandle.Alloc(this._vFoldQuadsIndicesCached, GCHandleType.Pinned);
				this._vFoldIndicesPinnedPointer = (UIntPtr)((ulong)(long)this._vFoldIndicesPinnedGCHandler.AddrOfPinnedObject());
				this.ClearCommittedAgentInformation();
			}

			// Token: 0x06001B66 RID: 7014 RVA: 0x000B4278 File Offset: 0x000B2478
			private void UpdateAttachmentMachineEntityVisibilities(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState oldState)
			{
				bool flag;
				bool flag2;
				bool flag3;
				bool flag4;
				switch (this._state)
				{
				case ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopeThrown:
				case ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopesPulling:
					flag = false;
					flag2 = true;
					flag3 = true;
					flag4 = false;
					break;
				case ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeThrown:
					flag = true;
					flag2 = false;
					flag3 = false;
					flag4 = true;
					break;
				case ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected:
					flag = true;
					flag2 = false;
					flag3 = false;
					flag4 = true;
					this.SetOarsAvailability(false);
					this.SetShieldsVisibility(false);
					break;
				case ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BrokenAndWaitingForRemoval:
					flag = false;
					flag2 = false;
					flag3 = true;
					flag4 = false;
					break;
				case ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopeFailedAndReloading:
					flag = false;
					flag2 = true;
					flag3 = true;
					flag4 = false;
					break;
				default:
					flag = false;
					flag2 = false;
					flag3 = false;
					flag4 = false;
					break;
				}
				if (oldState == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected)
				{
					this.SetShieldsVisibility(true);
					this.SetOarsAvailability(true);
				}
				foreach (GameEntity gameEntity in this.AttachmentSource.RampPhysicsList)
				{
					gameEntity.SetVisibilityExcludeParents(flag);
				}
				this.AttachmentSource.RampVisualEntity.SetVisibilityExcludeParents(flag);
				this.AttachmentSource.RampBarrier.SetVisibilityExcludeParents(!flag);
				this.AttachmentSource.RopeVisual.GameEntity.SetVisibilityExcludeParents(flag2);
				this.AttachmentSource.Hook.SetVisibilityExcludeParents(flag3);
				this.AttachmentSource.SetConnectionPhysicsEntitiesVisibility(flag4);
				if (this.AttachmentTarget != null)
				{
					this.AttachmentTarget.RampVisualEntity.SetVisibilityExcludeParents(flag);
					foreach (GameEntity gameEntity2 in this.AttachmentTarget.RampPhysicsList)
					{
						gameEntity2.SetVisibilityExcludeParents(flag);
					}
					this.AttachmentTarget.RampBarrier.SetVisibilityExcludeParents(!flag);
				}
			}

			// Token: 0x06001B67 RID: 7015 RVA: 0x000B4424 File Offset: 0x000B2624
			public bool ShouldLookForBetterConnections()
			{
				return this.AttachmentTarget != null;
			}

			// Token: 0x06001B68 RID: 7016 RVA: 0x000B442F File Offset: 0x000B262F
			public void OnParallelTick(float dt)
			{
				if (this._state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected)
				{
					this.ArrangePlanksMT();
				}
			}

			// Token: 0x06001B69 RID: 7017 RVA: 0x000B4440 File Offset: 0x000B2640
			public void OnTick(float dt)
			{
				this.ClearCommittedAgentInformation();
				if (this._state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BrokenAndWaitingForRemoval)
				{
					RopePileBaked ropeVisual = this.AttachmentSource.RopeVisual;
					MatrixFrame matrixFrame = this.AttachmentSource.RopeVisual.GameEntity.GetGlobalFrame();
					ropeVisual.UpdateRopeMeshVisualAccordingToTargetPointLinear(in matrixFrame.origin, in this.AttachmentSource.RopeVisual.GameEntity.GetGlobalFrame().origin);
					return;
				}
				if (this._state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopeThrown || this._state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopeFailedAndReloading)
				{
					this.UpdateRopeThrowingBehavior(dt);
				}
				else
				{
					MatrixFrame matrixFrame = this.AttachmentTarget.GameEntity.GetGlobalFrame();
					Vec3 vec = this.AttachmentTarget.HookAttachLocalPosition;
					Vec3 vec2 = matrixFrame.TransformToParent(ref vec);
					Vec3 origin = this.AttachmentSource.RopeVisual.GameEntity.GetGlobalFrame().origin;
					this._hookGlobalFrame.origin = this.AttachmentSource.RopeVisual.UpdateRopeMeshVisualAccordingToTargetPointLinear(in origin, in vec2);
					vec = vec2 - origin;
					this._hookGlobalFrame.rotation.f = vec.NormalizedCopy();
					vec = this._hookGlobalFrame.rotation.f.CrossProductWithUp();
					this._hookGlobalFrame.rotation.s = vec.NormalizedCopy();
					this._hookGlobalFrame.rotation.u = Vec3.CrossProduct(this._hookGlobalFrame.rotation.s, this._hookGlobalFrame.rotation.f);
					this._hookGlobalFrame.rotation.RotateAboutSide(-1.5707964f);
					if (this._currentRopeLengthFirstReachedFinalValue && MBMath.ApproximatelyEquals(this._currentRopeLength, this.AttachmentSource.RopeMinLength, 0.05f))
					{
						this._ropesPullingTimer.Reset();
						this._currentRopeLengthFirstReachedFinalValue = false;
					}
					if (this._state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopesPulling)
					{
						this.CheckAndConnectBridge(false);
					}
					else if (this._state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeThrown)
					{
						this.TickThrownBridge(dt);
						this.ArrangeNavmeshBridgeSideBarriersAndVFoldQuads();
					}
					else if (this._state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected)
					{
						this.ArrangePlanks();
						this.ArrangeNavmeshBridgeSideBarriersAndVFoldQuads();
					}
				}
				if (this.AttachmentTarget != null)
				{
					this.CheckAndBreakAttachment(dt);
				}
				if (this._state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected || this._state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeThrown)
				{
					if ((!this._faceSwapSideOneDone || !this._faceSwapSideTwoDone) && this._bridgeSwapTimer.Check(Mission.Current.CurrentTime))
					{
						if (!this._faceSwapSideOneDone && Mission.Current.Scene.SwapFaceConnectionsWithID(this._bridgeNavmeshId + 1, this._bridgeNavmeshId + 3, this.AttachmentTarget.RelatedShipNavmeshOffset + this.AttachmentTarget.OwnerShip.GetDynamicNavmeshIdStart(), true))
						{
							this._faceSwapSideOneDone = true;
						}
						if (!this._faceSwapSideTwoDone && Mission.Current.Scene.SwapFaceConnectionsWithID(this._bridgeNavmeshId + 2, this._bridgeNavmeshId + 4, this.AttachmentSource.RelatedShipNavmeshOffset + this.AttachmentSource.OwnerShip.GetDynamicNavmeshIdStart(), true))
						{
							this._faceSwapSideTwoDone = true;
						}
						this._bridgeCreated = true;
					}
					if (this._faceSwapSideOneDone && this._faceSwapSideTwoDone && !this.ShipIslandsConnected)
					{
						this.ShipIslandsConnected = true;
						MissionShip.MergeShipIslands(this.AttachmentSource.OwnerShip, this.AttachmentTarget.OwnerShip);
					}
				}
				if (this._state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeThrown || this._state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected)
				{
					this.CommittedWeightedPosition = this.AttachmentSource.SteppedAgentManager.WeightedPosition;
					this.CommittedAgentCount = (float)this.AttachmentSource.SteppedAgentManager.AgentCount;
					this.CommittedTotalMass = this.AttachmentSource.SteppedAgentManager.TotalMass;
					this.AttachmentSource.SteppedAgentManager.ClearAgentWeightAndPositionInformation();
				}
			}

			// Token: 0x06001B6A RID: 7018 RVA: 0x000B47D4 File Offset: 0x000B29D4
			private void CheckAndBreakAttachment(float dt)
			{
				this._shipBetweenAttachmentsCheckTimer -= dt;
				MatrixFrame globalFrame = this.AttachmentSource.GameEntity.GetGlobalFrame();
				MatrixFrame globalFrame2 = this.AttachmentTarget.GameEntity.GetGlobalFrame();
				if (globalFrame.rotation.u.z < 0.17364818f || globalFrame2.rotation.u.z < 0.17364818f)
				{
					this.SetAttachmentState(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BrokenAndWaitingForRemoval);
					return;
				}
				if (this._shipBetweenAttachmentsCheckTimer <= 0f)
				{
					this._shipBetweenAttachmentsCheckTimer = MBRandom.RandomFloatRanged(0.1f, 0.15f);
					if (ShipAttachmentMachine.IsShipBetweenAttachments(this.AttachmentSource, this.AttachmentTarget))
					{
						this.SetAttachmentState(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BrokenAndWaitingForRemoval);
						return;
					}
				}
				if (!ShipAttachmentMachine.CheckAttachmentsFacingEachOther(this.AttachmentSource, this.AttachmentTarget))
				{
					this.SetAttachmentState(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BrokenAndWaitingForRemoval);
					return;
				}
				ShipAttachmentMachine attachmentSource = this.AttachmentSource;
				bool flag;
				if (attachmentSource == null)
				{
					flag = true;
				}
				else
				{
					MissionShip ownerShip = attachmentSource.OwnerShip;
					NavalPhysics.SinkingState? sinkingState = ((ownerShip != null) ? new NavalPhysics.SinkingState?(ownerShip.Physics.NavalSinkingState) : null);
					NavalPhysics.SinkingState sinkingState2 = NavalPhysics.SinkingState.Floating;
					flag = !((sinkingState.GetValueOrDefault() == sinkingState2) & (sinkingState != null));
				}
				if (!flag)
				{
					ShipAttachmentPointMachine attachmentTarget = this.AttachmentTarget;
					bool flag2;
					if (attachmentTarget == null)
					{
						flag2 = true;
					}
					else
					{
						MissionShip ownerShip2 = attachmentTarget.OwnerShip;
						NavalPhysics.SinkingState? sinkingState = ((ownerShip2 != null) ? new NavalPhysics.SinkingState?(ownerShip2.Physics.NavalSinkingState) : null);
						NavalPhysics.SinkingState sinkingState2 = NavalPhysics.SinkingState.Floating;
						flag2 = !((sinkingState.GetValueOrDefault() == sinkingState2) & (sinkingState != null));
					}
					if (!flag2)
					{
						if (this._state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopesPulling && ((this._ropesPullingTimer.Check(false) && (MBMath.ApproximatelyEquals(this._currentRopeLength, this.AttachmentSource.RopeMinLength, 0.05f) || (this.AttachmentSource.OwnerShip.Team != null && (this.AttachmentSource.OwnerShip.Team.TeamAI as TeamAINavalComponent).TeamNavalQuerySystem.IsAnyShipInCriticalZoneBetween(this.AttachmentSource.OwnerShip, this.AttachmentTarget.OwnerShip)))) || ShipAttachmentMachine.CheckIntersectionsBetweenConnectionsWithState(this.AttachmentSource, this.AttachmentTarget, ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected)))
						{
							this.SetAttachmentState(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BrokenAndWaitingForRemoval);
							return;
						}
						if (this.ShipAttachmentJoint != null && this.ShipAttachmentJoint.IsBroken)
						{
							this.SetAttachmentState(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BrokenAndWaitingForRemoval);
							return;
						}
						return;
					}
				}
				this.SetAttachmentState(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BrokenAndWaitingForRemoval);
			}

			// Token: 0x06001B6B RID: 7019 RVA: 0x000B4A00 File Offset: 0x000B2C00
			public void InitializeRopeFlightDataAccordingToTargetPoint(in Vec3 sourceGlobalPosition, in Vec3 targetGlobalPosition)
			{
				float num = ShipAttachmentMachine.ShipAttachment.CalculateLaunchAngleDegree(sourceGlobalPosition, targetGlobalPosition, 20f);
				if (num == -3.4028235E+38f)
				{
					num = MathF.Clamp(num, Math.Min(44.9999f, ShipAttachmentMachine.ShipAttachment.CalculateDifferenceVectorAngle(in sourceGlobalPosition, in targetGlobalPosition) + 0.1f), 45f);
				}
				ValueTuple<Vec3, float> valueTuple = ShipAttachmentMachine.ShipAttachment.CalculateInitialVelocityAndTime(sourceGlobalPosition, targetGlobalPosition, num);
				this._launchFlightData = new ShipAttachmentMachine.ShipAttachment.FlightData(in sourceGlobalPosition, in targetGlobalPosition, in valueTuple.Item1, num, valueTuple.Item2);
			}

			// Token: 0x06001B6C RID: 7020 RVA: 0x000B4A80 File Offset: 0x000B2C80
			public void InitializeRopeFlightDataAccordingToTargetDirection(in Vec3 sourceGlobalPosition, in Vec3 targetGlobalDirection)
			{
				Vec3 vec = targetGlobalDirection * 25f;
				this._launchFlightData = new ShipAttachmentMachine.ShipAttachment.FlightData(in sourceGlobalPosition, in Vec3.Zero, in vec, MathF.Asin(targetGlobalDirection.z) * 180f / 3.1415927f, 0f);
			}

			// Token: 0x06001B6D RID: 7021 RVA: 0x000B4AD0 File Offset: 0x000B2CD0
			private Vec3 CalculateRelativeVelocityBetweenAttachments()
			{
				MissionShip ownerShip = this.AttachmentSource.OwnerShip;
				MissionShip ownerShip2 = this.AttachmentTarget.OwnerShip;
				MatrixFrame globalFrame = ownerShip.GameEntity.GetGlobalFrame();
				MatrixFrame globalFrame2 = ownerShip2.GameEntity.GetGlobalFrame();
				Vec3 vec = ownerShip.Physics.LocalCenterOfMass;
				Vec3 vec2 = globalFrame.TransformToParent(ref vec);
				vec = ownerShip2.Physics.LocalCenterOfMass;
				Vec3 vec3 = globalFrame2.TransformToParent(ref vec);
				MatrixFrame globalFrame3 = this.AttachmentSource.ConnectionClipPlaneEntity.GetGlobalFrame();
				MatrixFrame globalFrame4 = this.AttachmentTarget.ConnectionClipPlaneEntity.GetGlobalFrame();
				Vec3 vec4 = ownerShip.Physics.LinearVelocity + Vec3.CrossProduct(ownerShip.Physics.AngularVelocity, globalFrame3.origin - vec2);
				return ownerShip2.Physics.LinearVelocity + Vec3.CrossProduct(ownerShip2.Physics.AngularVelocity, globalFrame4.origin - vec3) - vec4;
			}

			// Token: 0x06001B6E RID: 7022 RVA: 0x000B4BD0 File Offset: 0x000B2DD0
			private void UpdateRopeMeshVisualAccordingToTargetPoint(in Vec3 sourceGlobalPosition, in Vec3 targetGlobalPosition, float throwingAngleDegree)
			{
				throwingAngleDegree = MathF.Clamp(throwingAngleDegree, Math.Min(89.99f, ShipAttachmentMachine.ShipAttachment.CalculateDifferenceVectorAngle(in sourceGlobalPosition, in targetGlobalPosition) + 0.1f), 89.999f);
				ValueTuple<Vec3, float> valueTuple = ShipAttachmentMachine.ShipAttachment.CalculateInitialVelocityAndTime(sourceGlobalPosition, targetGlobalPosition, throwingAngleDegree);
				this._hookGlobalFrame = this.AttachmentSource.RopeVisual.UpdateRopeMeshVisualAccordingToTargetPoint(in sourceGlobalPosition, in targetGlobalPosition, in valueTuple.Item1, valueTuple.Item2);
			}

			// Token: 0x06001B6F RID: 7023 RVA: 0x000B4C3C File Offset: 0x000B2E3C
			public void CheckAndConnectBridge(bool forceBridge = false)
			{
				MatrixFrame globalFrame = this.AttachmentSource.ConnectionClipPlaneEntity.GetGlobalFrame();
				MatrixFrame globalFrame2 = this.AttachmentTarget.ConnectionClipPlaneEntity.GetGlobalFrame();
				float num = globalFrame.origin.DistanceSquared(globalFrame2.origin);
				Vec3 vec = this.CalculateRelativeVelocityBetweenAttachments();
				float lengthSquared = vec.LengthSquared;
				Vec3 vec2 = globalFrame2.origin - globalFrame.origin;
				vec2.Normalize();
				float num2 = Vec2.DotProduct(globalFrame.rotation.f.AsVec2.Normalized(), vec2.AsVec2);
				float num3 = Vec2.DotProduct(globalFrame2.rotation.f.AsVec2.Normalized(), -vec2.AsVec2);
				float num4 = (num2 + num3) * 0.5f;
				ShipAttachmentPointMachine shipAttachmentPointMachine = null;
				if (!forceBridge)
				{
					MissionShip ownerShip = this.AttachmentTarget.OwnerShip;
					foreach (ShipAttachmentPointMachine shipAttachmentPointMachine2 in ((ownerShip != null) ? ownerShip.AttachmentPointMachines : null))
					{
						MatrixFrame globalFrame3 = shipAttachmentPointMachine2.GameEntity.GetGlobalFrame();
						Vec3 vec3 = globalFrame3.origin - globalFrame.origin;
						float lengthSquared2 = vec3.LengthSquared;
						vec3.Normalize();
						float num5 = Vec2.DotProduct(globalFrame.rotation.f.AsVec2.Normalized(), vec3.AsVec2);
						float num6 = Vec2.DotProduct(globalFrame3.rotation.f.AsVec2.Normalized(), -vec3.AsVec2);
						float num7 = (num5 + num6) * 0.5f;
						if (shipAttachmentPointMachine2.CurrentAttachment == null)
						{
							ShipAttachmentMachine linkedAttachmentMachine = shipAttachmentPointMachine2.LinkedAttachmentMachine;
							if (((linkedAttachmentMachine != null) ? linkedAttachmentMachine.CurrentAttachment : null) == null && lengthSquared2 < ShipAttachmentMachine.ShipAttachmentJoint.CalculatePossibleBridgeConnectionLengthSquared(this.AttachmentSource, shipAttachmentPointMachine2) && lengthSquared <= 4f && num5 > 0.18f && num6 > 0.18f && num7 > num4 && !ShipAttachmentMachine.CheckIntersectionsBetweenConnections(this.AttachmentSource, shipAttachmentPointMachine2))
							{
								shipAttachmentPointMachine = shipAttachmentPointMachine2;
							}
						}
					}
				}
				if (shipAttachmentPointMachine != null)
				{
					this.Destroy();
					this.AttachmentSource.ConnectWithAttachmentPointMachine(shipAttachmentPointMachine, true, false, false);
					return;
				}
				if (forceBridge || (num < this.AttachmentSource.BridgeConnectionLengthSquared && lengthSquared <= 4f && num2 > 0.18f && num3 > 0.18f))
				{
					this.StartBridgeThrowAnimation();
					string text = "event:/mission/movement/vessel/bridge_connect";
					vec = (globalFrame.origin + globalFrame2.origin) / 2f;
					SoundManager.StartOneShotEvent(text, ref vec);
				}
			}

			// Token: 0x06001B70 RID: 7024 RVA: 0x000B4EEC File Offset: 0x000B30EC
			public void InitializeShipAttachmentJoint(Vec3 attachmentSourceGlobalPosition, Vec3 attachmentTargetGlobalPosition, bool unbreakableJoint = false)
			{
				this._currentRopeLength = attachmentSourceGlobalPosition.AsVec2.Distance(attachmentTargetGlobalPosition.AsVec2) + 0.1f;
				this.ShipAttachmentJoint = new ShipAttachmentMachine.ShipAttachmentJoint(this.AttachmentSource, this.AttachmentTarget, unbreakableJoint);
				this.SetAttachmentState(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopesPulling);
				GameEntity gameEntity = GameEntity.Instantiate(Mission.Current.Scene, ShipAttachmentMachine.ShipAttachment._shipConnectionPlankVariations[0], false, true, "");
				Vec3 vec = gameEntity.GetBoundingBoxMax() - gameEntity.GetBoundingBoxMin();
				this._plankVerticalSize = gameEntity.GetLocalScale().y * vec.y;
				this._plankHorizontalSize = gameEntity.GetLocalScale().x * vec.x;
				gameEntity.Remove(78);
				this._bridgeSwapTimer = new Timer(Mission.Current.CurrentTime, 0f, true);
				if (!unbreakableJoint && !this._hookAttachSoundAlreadyTriggered)
				{
					bool flag = Agent.Main != null && Agent.Main.IsActive() && (this.AttachmentSource.OwnerShip.GetIsAgentOnShip(Agent.Main, false) || this.AttachmentTarget.OwnerShip.GetIsAgentOnShip(Agent.Main, false));
					SoundManager.StartOneShotEvent("event:/mission/movement/vessel/hook_impact_attach", ref attachmentTargetGlobalPosition, "isPlayer", flag ? 1f : 0f);
				}
				this._hookAttachSoundAlreadyTriggered = false;
				this.AttachmentSource.OwnerShip.ShipsLogic.OnSuccessfulHookThrow(this.AttachmentSource.OwnerShip, this.AttachmentTarget.OwnerShip);
				this._sideBarrierIndicesCached[0] = 0;
				this._sideBarrierIndicesCached[1] = 1;
				this._sideBarrierIndicesCached[2] = 2;
				this._sideBarrierIndicesCached[3] = 0;
				this._sideBarrierIndicesCached[4] = 2;
				this._sideBarrierIndicesCached[5] = 3;
				this._vFoldQuadsIndicesCached[0] = 2;
				this._vFoldQuadsIndicesCached[1] = 1;
				this._vFoldQuadsIndicesCached[2] = 0;
				this._vFoldQuadsIndicesCached[3] = 3;
				this._vFoldQuadsIndicesCached[4] = 2;
				this._vFoldQuadsIndicesCached[5] = 0;
			}

			// Token: 0x06001B71 RID: 7025 RVA: 0x000B50D4 File Offset: 0x000B32D4
			private void UpdateRopeThrowingBehavior(float dt)
			{
				this._ropeThrownTimer += dt;
				if (this._launchFlightData.GlobalPositionError.LengthSquared > 1.0000001E-06f)
				{
					this._launchFlightData.GlobalPositionError = this._launchFlightData.GlobalPositionError * (1f - dt * 8f);
				}
				else
				{
					this._launchFlightData.GlobalPositionError = Vec3.Zero;
				}
				Vec3 globalPosition = this.AttachmentSource.RopeVisual.GameEntity.GlobalPosition;
				if (this._state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopeFailedAndReloading)
				{
					this._launchFlightData.GlobalVelocity = this._launchFlightData.GlobalVelocity * (1f - dt * (this._launchFlightData.IsUnderWater ? 8f : 1f)) + MBGlobals.GravitationalAcceleration * dt;
					Vec3 sourceGlobalPosition = this._launchFlightData.SourceGlobalPosition;
					this._launchFlightData.SourceGlobalPosition = this._launchFlightData.SourceGlobalPosition + this._launchFlightData.GlobalVelocity * dt;
					if (this._launchFlightData.SourceGlobalPosition.DistanceSquared(globalPosition) > 1600f)
					{
						Vec3 vec = globalPosition;
						Vec3 vec2 = this._launchFlightData.SourceGlobalPosition - globalPosition;
						this._launchFlightData.SourceGlobalPosition = vec + vec2.NormalizedCopy() * 40f;
						this._launchFlightData.GlobalVelocity = (this._launchFlightData.SourceGlobalPosition - sourceGlobalPosition) / dt;
					}
					if (this._launchFlightData.IsUnderWater)
					{
						this._launchFlightData.SourceGlobalPosition.z = Math.Min(this.AttachmentSource.Scene.GetWaterLevelAtPosition(this._launchFlightData.SourceGlobalPosition.AsVec2, true, false), this._launchFlightData.SourceGlobalPosition.z);
					}
					else if (this.AttachmentSource.Scene.GetWaterLevelAtPosition(this._launchFlightData.SourceGlobalPosition.AsVec2, true, false) > this._launchFlightData.SourceGlobalPosition.z)
					{
						this._launchFlightData.IsUnderWater = true;
					}
					if (this._currentRopeLength <= 0f)
					{
						if (!this._launchFlightData.IsUnderWater)
						{
							this._ropeThrownTimer -= dt * 0.8f;
						}
						float num = MathF.Clamp(MathF.Pow(this._ropeThrownTimer / this._launchFlightData.Time, 1.3f), 0f, 1f);
						if (num >= 1f)
						{
							this._currentRopeLength = globalPosition.Distance(this._launchFlightData.SourceGlobalPosition);
							this._hookGlobalFrame.origin = this.AttachmentSource.RopeVisual.UpdateRopeMeshVisualAccordingToTargetPointLinear(in globalPosition, in this._launchFlightData.SourceGlobalPosition);
							Vec3 vec2 = this._launchFlightData.SourceGlobalPosition - globalPosition;
							this._hookGlobalFrame.rotation.f = vec2.NormalizedCopy();
							vec2 = this._hookGlobalFrame.rotation.f.CrossProductWithUp();
							this._hookGlobalFrame.rotation.s = vec2.NormalizedCopy();
							this._hookGlobalFrame.rotation.u = Vec3.CrossProduct(this._hookGlobalFrame.rotation.s, this._hookGlobalFrame.rotation.f);
							this._hookGlobalFrame.rotation.RotateAboutSide(-1.5707964f);
							return;
						}
						this.UpdateRopeMeshVisualAccordingToTargetPoint(in globalPosition, in this._launchFlightData.SourceGlobalPosition, this._launchFlightData.AngleDegree - num * (this._launchFlightData.AngleDegree - ShipAttachmentMachine.ShipAttachment.CalculateDifferenceVectorAngle(in globalPosition, in this._launchFlightData.SourceGlobalPosition) - 0.1f));
						return;
					}
					else
					{
						this._currentRopeLength -= dt * 4f;
						Vec3 vec3 = globalPosition;
						Vec3 vec2 = this._launchFlightData.SourceGlobalPosition - globalPosition;
						this._launchFlightData.SourceGlobalPosition = vec3 + vec2.NormalizedCopy() * this._currentRopeLength;
						this._hookGlobalFrame.origin = this.AttachmentSource.RopeVisual.UpdateRopeMeshVisualAccordingToTargetPointLinear(in globalPosition, in this._launchFlightData.SourceGlobalPosition);
						vec2 = this._launchFlightData.SourceGlobalPosition - globalPosition;
						this._hookGlobalFrame.rotation.f = vec2.NormalizedCopy();
						vec2 = this._hookGlobalFrame.rotation.f.CrossProductWithUp();
						this._hookGlobalFrame.rotation.s = vec2.NormalizedCopy();
						this._hookGlobalFrame.rotation.u = Vec3.CrossProduct(this._hookGlobalFrame.rotation.s, this._hookGlobalFrame.rotation.f);
						this._hookGlobalFrame.rotation.RotateAboutSide(-1.5707964f);
						if (this._currentRopeLength <= 0f)
						{
							this._currentRopeLength = 0f;
							this.SetAttachmentState(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BrokenAndWaitingForRemoval);
							return;
						}
					}
				}
				else
				{
					float num2 = this._launchFlightData.AngleDegree - this._ropeThrownTimer * 5f;
					if (this._launchFlightData.Time > 0f)
					{
						MatrixFrame matrixFrame = this.AttachmentTarget.GameEntity.GetGlobalFrame();
						Vec3 vec2 = this.AttachmentTarget.HookAttachLocalPosition;
						Vec3 vec4 = matrixFrame.TransformToParent(ref vec2);
						Vec3 vec5 = vec4 + this._launchFlightData.GlobalPositionError;
						if (this._ropeThrownTimer < this._launchFlightData.Time)
						{
							Vec3 vec6 = this.GetLaunchProjectileCurrentGlobalPosition(this._ropeThrownTimer);
							vec6 += vec5 - this._launchFlightData.TargetGlobalPosition;
							this.UpdateRopeMeshVisualAccordingToTargetPoint(in globalPosition, in vec6, num2);
							return;
						}
						float num3 = MathF.Clamp(MathF.Pow((this._ropeThrownTimer - this._launchFlightData.Time) / this._launchFlightData.Time, 1.3f), 0f, 1f);
						this.UpdateRopeMeshVisualAccordingToTargetPoint(in globalPosition, in vec5, num2 - num3 * (num2 - ShipAttachmentMachine.ShipAttachment.CalculateDifferenceVectorAngle(in globalPosition, in vec5) - 0.1f));
						if (num3 >= 1f)
						{
							this.InitializeShipAttachmentJoint(globalPosition, vec4, false);
							return;
						}
					}
					else
					{
						Vec3 launchProjectileCurrentGlobalPosition = this.GetLaunchProjectileCurrentGlobalPosition(this._ropeThrownTimer);
						this.UpdateRopeMeshVisualAccordingToTargetPoint(in globalPosition, in launchProjectileCurrentGlobalPosition, num2);
						if (this.AttachmentSource.Scene.GetWaterLevelAtPosition(launchProjectileCurrentGlobalPosition.AsVec2, true, false) > launchProjectileCurrentGlobalPosition.z)
						{
							this.SetAttachmentState(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopeFailedAndReloading);
							this._launchFlightData.SourceGlobalPosition = launchProjectileCurrentGlobalPosition;
							this._launchFlightData.GlobalVelocity = this._launchFlightData.GlobalVelocity + MBGlobals.GravitationalAcceleration * this._ropeThrownTimer;
							this._launchFlightData.AngleDegree = num2;
							this._launchFlightData.Time = Math.Min(2.5f, this._ropeThrownTimer);
							this._launchFlightData.IsUnderWater = true;
							this._ropeThrownTimer = 0f;
							this._currentRopeLength = 0f;
							SoundManager.StartOneShotEvent("event:/mission/movement/vessel/hook_impact_fail_water_splash", ref launchProjectileCurrentGlobalPosition);
							return;
						}
						if (launchProjectileCurrentGlobalPosition.DistanceSquared(globalPosition) > 1600f)
						{
							this.SetAttachmentState(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopeFailedAndReloading);
							this._launchFlightData.SourceGlobalPosition = launchProjectileCurrentGlobalPosition;
							this._launchFlightData.GlobalVelocity = new Vec3(0f, 0f, this._launchFlightData.GlobalVelocity.z - 9.806f * this._ropeThrownTimer, -1f);
							this._launchFlightData.AngleDegree = num2;
							this._launchFlightData.Time = Math.Min(2.5f, this._ropeThrownTimer);
							this._ropeThrownTimer = 0f;
							this._currentRopeLength = 0f;
							SoundManager.StartOneShotEvent("event:/mission/movement/vessel/hook_impact_fail_to_attach", ref launchProjectileCurrentGlobalPosition);
							return;
						}
						WeakGameEntity attachmentSourceHolderEntity = this.AttachmentSource.GameEntity.Parent;
						IEnumerable<WeakGameEntity> enumerable = from x in Mission.Current.GetActiveEntitiesWithScriptComponentOfType<ShipAttachmentPointMachine>()
							where x.Parent != attachmentSourceHolderEntity
							select x;
						ShipAttachmentPointMachine shipAttachmentPointMachine = null;
						foreach (WeakGameEntity weakGameEntity in enumerable)
						{
							ShipAttachmentPointMachine firstScriptOfType = weakGameEntity.GetFirstScriptOfType<ShipAttachmentPointMachine>();
							if (firstScriptOfType.CurrentAttachment == null)
							{
								ShipAttachmentMachine linkedAttachmentMachine = firstScriptOfType.LinkedAttachmentMachine;
								if (((linkedAttachmentMachine != null) ? linkedAttachmentMachine.CurrentAttachment : null) == null)
								{
									MatrixFrame matrixFrame = weakGameEntity.GetGlobalFrame();
									Vec3 vec2 = firstScriptOfType.HookAttachLocalPosition;
									if (launchProjectileCurrentGlobalPosition.DistanceSquared(matrixFrame.TransformToParent(ref vec2)) < 9f)
									{
										Vec3 f = firstScriptOfType.GameEntity.GetGlobalFrame().rotation.f;
										Vec3 vec7 = launchProjectileCurrentGlobalPosition;
										matrixFrame = weakGameEntity.GetGlobalFrame();
										Vec3 hookAttachLocalPosition = firstScriptOfType.HookAttachLocalPosition;
										if (Vec3.DotProduct(f, vec7 - matrixFrame.TransformToParent(ref hookAttachLocalPosition)) < 0f && ShipAttachmentMachine.ComputePotentialAttachmentValue(this.AttachmentSource, firstScriptOfType, false, true, true) > 0f)
										{
											shipAttachmentPointMachine = firstScriptOfType;
											matrixFrame = weakGameEntity.GetGlobalFrame();
											vec2 = firstScriptOfType.HookAttachLocalPosition;
											launchProjectileCurrentGlobalPosition.DistanceSquared(matrixFrame.TransformToParent(ref vec2));
											break;
										}
									}
								}
							}
						}
						if (shipAttachmentPointMachine != null)
						{
							if (this._attachmentInitializedByPlayer && this.AttachmentSource.OwnerShip != null && this.AttachmentSource.OwnerShip.Team != null && this.AttachmentSource.OwnerShip.Team.IsPlayerTeam)
							{
								MissionShip ownerShip = this.AttachmentSource.OwnerShip;
								if (ownerShip != null)
								{
									ShipOrder shipOrder = ownerShip.ShipOrder;
									if (shipOrder != null)
									{
										shipOrder.SetBoardingTargetShip(shipAttachmentPointMachine.OwnerShip);
									}
								}
							}
							shipAttachmentPointMachine.AssignConnection(this);
							this.AttachmentTarget = shipAttachmentPointMachine;
							this.UpdateAttachmentMachineEntityVisibilities(this._state);
							this._launchFlightData.Time = this._ropeThrownTimer;
							MatrixFrame matrixFrame = shipAttachmentPointMachine.GameEntity.GetGlobalFrame();
							Vec3 vec2 = shipAttachmentPointMachine.HookAttachLocalPosition;
							Vec3 vec8 = matrixFrame.TransformToParent(ref vec2);
							this._launchFlightData.GlobalPositionError = launchProjectileCurrentGlobalPosition - vec8;
							if ((this.AttachmentSource.PilotStandingPoint.UserAgent != null && this.AttachmentSource.PilotStandingPoint.UserAgent.IsMainAgent) || (this.AttachmentSource.PilotStandingPoint.UserAgent == null && this.AttachmentSource.PilotStandingPoint.PreviousUserAgent != null && this.AttachmentSource.PilotStandingPoint.PreviousUserAgent.IsMainAgent))
							{
								this._hookAttachSoundAlreadyTriggered = SoundManager.StartOneShotEvent("event:/mission/movement/vessel/hook_impact_attach", ref vec8, "isPlayer", 1f);
							}
						}
						if (this.AttachmentTarget != null && ShipAttachmentMachine.CheckIntersectionsBetweenConnectionsWithState(this.AttachmentSource, this.AttachmentTarget, ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected))
						{
							this.SetAttachmentState(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopeFailedAndReloading);
							this._launchFlightData.SourceGlobalPosition = launchProjectileCurrentGlobalPosition;
							this._launchFlightData.GlobalVelocity = new Vec3(0f, 0f, this._launchFlightData.GlobalVelocity.z - 9.806f * this._ropeThrownTimer, -1f);
							this._launchFlightData.AngleDegree = num2;
							this._launchFlightData.Time = Math.Min(2.5f, this._ropeThrownTimer);
							this._ropeThrownTimer = 0f;
							this._currentRopeLength = 0f;
							SoundManager.StartOneShotEvent("event:/mission/movement/vessel/hook_impact_fail_to_attach", ref launchProjectileCurrentGlobalPosition);
						}
					}
				}
			}

			// Token: 0x06001B72 RID: 7026 RVA: 0x000B5BD0 File Offset: 0x000B3DD0
			public void OnFixedTick(float fixedDt)
			{
				if (this._state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopesPulling || this._state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected || this._state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeThrown)
				{
					this.ShipAttachmentJoint.OnFixedTick(fixedDt, this, ref this._currentRopeLength);
				}
				if ((this._state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected || this._state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeThrown) && this.CommittedAgentCount > 0f && this.CommittedTotalMass > 0f && this.CommittedWeightedPosition != Vec3.Zero)
				{
					Vec3 vec = this.CommittedWeightedPosition / this.CommittedTotalMass;
					if (vec.DistanceSquared(this.AttachmentSource.ConnectionClipPlaneEntity.GetGlobalFrameImpreciseForFixedTick().origin) < 25f)
					{
						MatrixFrame globalFrameImpreciseForFixedTick = this.AttachmentSource.ConnectionClipPlaneEntity.GetGlobalFrameImpreciseForFixedTick();
						Vec3 vec2 = this.AttachmentTarget.ConnectionClipPlaneEntity.GetGlobalFrameImpreciseForFixedTick().origin - globalFrameImpreciseForFixedTick.origin;
						float num = vec2.Normalize();
						Vec3 vec3 = vec - globalFrameImpreciseForFixedTick.origin;
						float num2 = Vec3.DotProduct(vec2, vec3) / num;
						MissionShip ownerShip = this.AttachmentSource.OwnerShip;
						MissionShip ownerShip2 = this.AttachmentSource.OwnerShip;
						Vec3 vec4 = ownerShip.GameEntity.GetBodyWorldTransform().TransformToLocal(ref vec);
						Vec3 vec5 = ownerShip2.GameEntity.GetBodyWorldTransform().TransformToLocal(ref vec);
						float stepAgentWeightMultiplier = ownerShip.Physics.PhysicsParameters.StepAgentWeightMultiplier;
						float stepAgentWeightMultiplier2 = ownerShip2.Physics.PhysicsParameters.StepAgentWeightMultiplier;
						Vec3 vec6 = this.CommittedTotalMass * MBGlobals.GravitationalAcceleration;
						NavalPhysics physics = ownerShip.Physics;
						Vec3 vec7 = vec6 * ((1f - num2) * stepAgentWeightMultiplier);
						physics.ApplyGlobalForceAtLocalPos(in vec4, in vec7, 0);
						NavalPhysics physics2 = ownerShip2.Physics;
						vec7 = vec6 * (num2 * stepAgentWeightMultiplier2);
						physics2.ApplyGlobalForceAtLocalPos(in vec5, in vec7, 0);
					}
				}
				this.ClearCommittedAgentInformation();
			}

			// Token: 0x06001B73 RID: 7027 RVA: 0x000B5DB4 File Offset: 0x000B3FB4
			private void ArrangeBarrier(GameEntity barrier, Vec3 startPosition, Vec3 endPosition, float height)
			{
				MatrixFrame matrixFrame;
				matrixFrame.origin = Vec3.Zero;
				matrixFrame.rotation = Mat3.Identity;
				Vec3[] sideBarrierQuadsCached = this._sideBarrierQuadsCached;
				int num = 0;
				Vec3 vec = startPosition + new Vec3(0f, 0f, height, -1f);
				sideBarrierQuadsCached[num] = matrixFrame.TransformToLocal(ref vec);
				Vec3[] sideBarrierQuadsCached2 = this._sideBarrierQuadsCached;
				int num2 = 1;
				vec = endPosition + new Vec3(0f, 0f, height, -1f);
				sideBarrierQuadsCached2[num2] = matrixFrame.TransformToLocal(ref vec);
				this._sideBarrierQuadsCached[2] = matrixFrame.TransformToLocal(ref endPosition);
				this._sideBarrierQuadsCached[3] = matrixFrame.TransformToLocal(ref startPosition);
				GameEntityPhysicsExtensions.ReplacePhysicsBodyWithQuadPhysicsBody(barrier, this._sideBarriersQuadPinnedPointer, 4, this._woodPhysicsMaterialCached, 272, this._sideBarriersIndicesPinnedPointer, 6);
				barrier.SetGlobalFrame(ref matrixFrame, true);
			}

			// Token: 0x06001B74 RID: 7028 RVA: 0x000B5E90 File Offset: 0x000B4090
			private void ConnectBridge()
			{
				for (int i = 0; i < 4; i++)
				{
					string text = ShipAttachmentMachine.ShipAttachment._shipConnectionPlankVariations[MBRandom.RandomInt(0, ShipAttachmentMachine.ShipAttachment._shipConnectionPlankVariations.Count - 1)];
					GameEntity gameEntity = GameEntity.Instantiate(Mission.Current.Scene, text, MatrixFrame.Identity, true);
					this._bridge.AddChild(gameEntity, false);
					this._targetSafetyPlanks.Add(gameEntity);
				}
				for (int j = 0; j < 4; j++)
				{
					string text2 = ShipAttachmentMachine.ShipAttachment._shipConnectionPlankVariations[MBRandom.RandomInt(0, ShipAttachmentMachine.ShipAttachment._shipConnectionPlankVariations.Count - 1)];
					GameEntity gameEntity2 = GameEntity.Instantiate(Mission.Current.Scene, text2, MatrixFrame.Identity, true);
					this._bridge.AddChild(gameEntity2, false);
					this._sourceSafetyPlanks.Add(gameEntity2);
				}
				this._bridgeNavmeshId = Mission.Current.GetNextDynamicNavMeshIdStart();
				this._navMeshBridge = GameEntity.Instantiate(Mission.Current.Scene, "ship_connection_nav_mesh_plank", MatrixFrame.Identity, true);
				this._navMeshBridgeNavMeshHolder = this._navMeshBridge.GetFirstChildEntityWithTag("navmesh_holder");
				this._navMeshBridgeNavMeshHolder.CreateAndAddScriptComponent("ShipBridgeNavmeshHolder", true);
				this._shipBridgeNavmeshHolder = this._navMeshBridgeNavMeshHolder.GetFirstScriptOfType<ShipAttachmentMachine.ShipBridgeNavmeshHolder>();
				this._shipBridgeNavmeshHolder.Initialize(this._bridgeNavmeshId, this.AttachmentSource);
				this.SetAttachmentState(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected);
				this.ArrangePlanksMT();
				this.ArrangePlanks();
				this.ArrangeNavmeshBridgeSideBarriersAndVFoldQuads();
				this.AddRopesToBridge();
				this._bridge.CreateAndAddScriptComponent("ShipBridge", true);
				this._shipBridgeNavmeshHolder.GameEntity.UpdateAttachedNavigationMeshFaces();
				this._bridgeSwapTimer.Reset(Mission.Current.CurrentTime, 0.05f);
				this._faceSwapSideOneDone = false;
				this._faceSwapSideTwoDone = false;
				this.ShipIslandsConnected = false;
				this.AttachmentSource.OwnerShip.ShipsLogic.OnShipsConnected(this.AttachmentSource.OwnerShip, this.AttachmentTarget.OwnerShip);
			}

			// Token: 0x06001B75 RID: 7029 RVA: 0x000B6074 File Offset: 0x000B4274
			private void SetShieldsVisibility(bool visible)
			{
				MBReadOnlyList<ShipShieldComponent> shields = this.AttachmentSource.OwnerShip.Shields;
				if (shields.Count > 0)
				{
					Vec3 origin = this.AttachmentSource.ConnectionClipPlaneEntity.ComputePreciseGlobalFrameForFixedTickSlow().origin;
					foreach (ShipShieldComponent shipShieldComponent in shields)
					{
						if (shipShieldComponent.GameEntity.IsValid)
						{
							if (visible)
							{
								shipShieldComponent.DeregisterRampEntityDisablingShield(this.AttachmentSource.ConnectionClipPlaneEntity);
							}
							else
							{
								Vec3 origin2 = shipShieldComponent.GameEntity.ComputePreciseGlobalFrameForFixedTickSlow().origin;
								if (origin2.DistanceSquared(origin) < 3f)
								{
									shipShieldComponent.RegisterRampEntityDisablingShield(this.AttachmentSource.ConnectionClipPlaneEntity);
								}
							}
						}
					}
				}
				if (this.AttachmentTarget != null)
				{
					MBReadOnlyList<ShipShieldComponent> shields2 = this.AttachmentTarget.OwnerShip.Shields;
					if (shields2.Count > 0)
					{
						Vec3 origin3 = this.AttachmentTarget.ConnectionClipPlaneEntity.ComputePreciseGlobalFrameForFixedTickSlow().origin;
						foreach (ShipShieldComponent shipShieldComponent2 in shields2)
						{
							if (shipShieldComponent2.GameEntity.IsValid)
							{
								if (visible)
								{
									shipShieldComponent2.DeregisterRampEntityDisablingShield(this.AttachmentTarget.ConnectionClipPlaneEntity);
								}
								else
								{
									Vec3 origin4 = shipShieldComponent2.GameEntity.ComputePreciseGlobalFrameForFixedTickSlow().origin;
									if (origin4.DistanceSquared(origin3) < 3f)
									{
										shipShieldComponent2.RegisterRampEntityDisablingShield(this.AttachmentTarget.ConnectionClipPlaneEntity);
									}
								}
							}
						}
					}
				}
			}

			// Token: 0x06001B76 RID: 7030 RVA: 0x000B6228 File Offset: 0x000B4428
			private void ArrangeNavmeshBridgeSideBarriersAndVFoldQuads()
			{
				MatrixFrame globalFrame = this.AttachmentSource.ConnectionClipPlaneEntity.GetGlobalFrame();
				MatrixFrame globalFrame2 = this.AttachmentTarget.ConnectionClipPlaneEntity.GetGlobalFrame();
				Vec3 s = globalFrame.rotation.s;
				s.Normalize();
				Vec3 s2 = globalFrame2.rotation.s;
				s2.Normalize();
				Vec3 vec = globalFrame2.origin - s2 * this._plankHorizontalSize * 0.5f;
				Vec3 vec2 = globalFrame2.origin + s2 * this._plankHorizontalSize * 0.5f;
				Vec3 vec3 = globalFrame.origin + s * this._plankHorizontalSize * 0.5f;
				Vec3 vec4 = globalFrame.origin - s * this._plankHorizontalSize * 0.5f;
				Vec3 vec5 = vec - vec3;
				vec5.Normalize();
				Vec3 vec6 = vec2 - vec4;
				vec6.Normalize();
				vec += vec5 * 0.05f;
				vec2 += vec6 * 0.05f;
				vec3 -= vec5 * 0.05f;
				vec4 -= vec6 * 0.05f;
				this.ArrangeBarrier(this.AttachmentSource.BarrierSource, vec2, vec4, 6f);
				this.ArrangeBarrier(this.AttachmentSource.BarrierTarget, vec3, vec, 6f);
				this.ArrangeVFoldQuads(vec3, vec4, vec2, vec);
				this.ArrangeNavMeshBridge(vec3, vec4, vec, vec2);
			}

			// Token: 0x06001B77 RID: 7031 RVA: 0x000B63CC File Offset: 0x000B45CC
			private void ArrangeVFoldQuads(Vec3 leftSource, Vec3 rightSource, Vec3 rightTarget, Vec3 leftTarget)
			{
				Vec3 vec = (leftSource + rightSource) * 0.5f - Vec3.Up * 0.5f;
				Vec3 vec2 = (leftTarget + rightTarget) * 0.5f - Vec3.Up * 0.5f;
				MatrixFrame matrixFrame;
				matrixFrame.origin = (leftSource + leftTarget + rightSource + rightTarget) * 0.25f;
				matrixFrame.rotation = Mat3.Identity;
				this._vFoldQuadsCached[0] = matrixFrame.TransformToLocal(ref leftSource);
				this._vFoldQuadsCached[1] = matrixFrame.TransformToLocal(ref leftTarget);
				this._vFoldQuadsCached[2] = matrixFrame.TransformToLocal(ref vec2);
				this._vFoldQuadsCached[3] = matrixFrame.TransformToLocal(ref vec);
				GameEntityPhysicsExtensions.ReplacePhysicsBodyWithQuadPhysicsBody(this.AttachmentSource.VFoldSource, this._vFoldQuadPinnedPointer, 4, this._defaultPhysicsMaterialCached, 2097168, this._vFoldIndicesPinnedPointer, 6);
				this.AttachmentSource.VFoldSource.SetGlobalFrame(ref matrixFrame, true);
				this._vFoldQuadsCached[0] = matrixFrame.TransformToLocal(ref rightSource);
				this._vFoldQuadsCached[1] = matrixFrame.TransformToLocal(ref vec);
				this._vFoldQuadsCached[2] = matrixFrame.TransformToLocal(ref vec2);
				this._vFoldQuadsCached[3] = matrixFrame.TransformToLocal(ref rightTarget);
				GameEntityPhysicsExtensions.ReplacePhysicsBodyWithQuadPhysicsBody(this.AttachmentSource.VFoldTarget, this._vFoldQuadPinnedPointer, 4, this._defaultPhysicsMaterialCached, 2097168, this._vFoldIndicesPinnedPointer, 6);
				this.AttachmentSource.VFoldTarget.SetGlobalFrame(ref matrixFrame, true);
			}

			// Token: 0x06001B78 RID: 7032 RVA: 0x000B6578 File Offset: 0x000B4778
			private void StartBridgeThrowAnimation()
			{
				this._targetSafetyPlanks.Clear();
				this._sourceSafetyPlanks.Clear();
				this._bridgeFlightData.DtSinceFlightStart = 0f;
				this._bridgeFlightData.CurveLerpVelocity = 0f;
				this._bridgeFlightData.CurveLerpValue = 0f;
				this._bridgeFlightData.ThrowFinishValue = 7f;
				this._currentRopeLength = this.AttachmentSource.ConnectionClipPlaneEntity.ComputePreciseGlobalFrameForFixedTickSlow().origin.Distance(this.AttachmentTarget.ConnectionClipPlaneEntity.ComputePreciseGlobalFrameForFixedTickSlow().origin);
				this.SetAttachmentState(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeThrown);
			}

			// Token: 0x06001B79 RID: 7033 RVA: 0x000B661C File Offset: 0x000B481C
			private void TickThrownBridge(float dt)
			{
				MatrixFrame globalFrame = this.AttachmentSource.ConnectionClipPlaneEntity.GetGlobalFrame();
				Vec3 origin = globalFrame.origin;
				Vec3 vec = this.AttachmentTarget.ConnectionClipPlaneEntity.GetGlobalFrame().origin;
				if (MBMath.ApproximatelyEquals(vec.DistanceSquared(origin), 0f, 1E-05f))
				{
					vec += globalFrame.rotation.f * 0.1f + globalFrame.rotation.u * 0.1f;
				}
				float num = 10.327f;
				float num2 = ShipAttachmentMachine.ShipAttachment.CalculateLaunchAngleDegree(origin, vec, num);
				if (num2 == -3.4028235E+38f)
				{
					num2 = MathF.Clamp(num2, MathF.Min(44.9999f, ShipAttachmentMachine.ShipAttachment.CalculateDifferenceVectorAngle(in origin, in vec) + 0.1f), 45f);
				}
				ValueTuple<Vec3, float> valueTuple = ShipAttachmentMachine.ShipAttachment.CalculateInitialVelocityAndTime(origin, vec, num2);
				this._bridgeFlightData.CurrentFrameInitialVelocity = valueTuple.Item1;
				this._bridgeFlightData.CurrentFrameTotalLightTime = valueTuple.Item2;
				this._bridgeFlightData.DtSinceFlightStart = this._bridgeFlightData.DtSinceFlightStart + dt;
				this._bridgeFlightData.CurveLerpVelocity = this._bridgeFlightData.CurveLerpVelocity + dt * 3f;
				if (this._bridgeFlightData.CurrentFrameTotalLightTime <= this._bridgeFlightData.DtSinceFlightStart)
				{
					this._bridgeFlightData.CurveLerpValue = this._bridgeFlightData.CurveLerpValue + this._bridgeFlightData.CurveLerpVelocity * dt;
					if (this._bridgeFlightData.CurveLerpValue > this._bridgeFlightData.ThrowFinishValue)
					{
						this.ConnectBridge();
						return;
					}
				}
				this.ArrangePlanksMT();
				this.ArrangePlanks();
			}

			// Token: 0x06001B7A RID: 7034 RVA: 0x000B679C File Offset: 0x000B499C
			private void SetOarsAvailability(bool value)
			{
				Vec3 origin = this.AttachmentSource.ConnectionClipPlaneEntity.ComputePreciseGlobalFrameForFixedTickSlow().origin;
				foreach (ShipOarMachine shipOarMachine in this.AttachmentSource.OwnerShip.LeftSideShipOarMachines)
				{
					if (value)
					{
						shipOarMachine.DeregisterRampEntityDisablingOar(this.AttachmentSource.ConnectionClipPlaneEntity);
					}
					else
					{
						Vec3 origin2 = shipOarMachine.GameEntity.ComputePreciseGlobalFrameForFixedTickSlow().origin;
						if (origin2.DistanceSquared(origin) < 9f)
						{
							shipOarMachine.RegisterRampEntityDisablingOar(this.AttachmentSource.ConnectionClipPlaneEntity);
						}
					}
				}
				foreach (ShipOarMachine shipOarMachine2 in this.AttachmentSource.OwnerShip.RightSideShipOarMachines)
				{
					if (value)
					{
						shipOarMachine2.DeregisterRampEntityDisablingOar(this.AttachmentSource.ConnectionClipPlaneEntity);
					}
					else
					{
						Vec3 origin3 = shipOarMachine2.GameEntity.ComputePreciseGlobalFrameForFixedTickSlow().origin;
						if (origin3.DistanceSquared(origin) < 9f)
						{
							shipOarMachine2.RegisterRampEntityDisablingOar(this.AttachmentSource.ConnectionClipPlaneEntity);
						}
					}
				}
				if (this.AttachmentTarget != null)
				{
					Vec3 origin4 = this.AttachmentTarget.ConnectionClipPlaneEntity.ComputePreciseGlobalFrameForFixedTickSlow().origin;
					foreach (ShipOarMachine shipOarMachine3 in this.AttachmentTarget.OwnerShip.LeftSideShipOarMachines)
					{
						if (value)
						{
							shipOarMachine3.DeregisterRampEntityDisablingOar(this.AttachmentTarget.ConnectionClipPlaneEntity);
						}
						else
						{
							Vec3 origin5 = shipOarMachine3.GameEntity.ComputePreciseGlobalFrameForFixedTickSlow().origin;
							if (origin5.DistanceSquared(origin4) < 9f)
							{
								shipOarMachine3.RegisterRampEntityDisablingOar(this.AttachmentTarget.ConnectionClipPlaneEntity);
							}
						}
					}
					foreach (ShipOarMachine shipOarMachine4 in this.AttachmentTarget.OwnerShip.RightSideShipOarMachines)
					{
						if (value)
						{
							shipOarMachine4.DeregisterRampEntityDisablingOar(this.AttachmentTarget.ConnectionClipPlaneEntity);
						}
						else
						{
							Vec3 origin6 = shipOarMachine4.GameEntity.ComputePreciseGlobalFrameForFixedTickSlow().origin;
							if (origin6.DistanceSquared(origin4) < 9f)
							{
								shipOarMachine4.RegisterRampEntityDisablingOar(this.AttachmentTarget.ConnectionClipPlaneEntity);
							}
						}
					}
				}
			}

			// Token: 0x06001B7B RID: 7035 RVA: 0x000B6A38 File Offset: 0x000B4C38
			private void AddRopesToBridge()
			{
				int numberOfPlanksNeeded = this._numberOfPlanksNeeded;
				int num = (int)((float)this._numberOfPlanksNeeded * 0.16f + MBRandom.RandomFloat * (float)this._numberOfPlanksNeeded * 0.16f);
				for (int i = 0; i < num; i++)
				{
					ShipAttachmentMachine.ShipAttachment.RopeSegment ropeSegment = default(ShipAttachmentMachine.ShipAttachment.RopeSegment);
					int num2 = 1 + MBRandom.RandomInt(3);
					int num3 = this._numberOfPlanksNeeded - 5;
					ropeSegment.StartSegmentIndex = (int)(3f + MBRandom.RandomFloat * (float)(num3 - 3));
					ropeSegment.EndSegmentIndex = ropeSegment.StartSegmentIndex + num2;
					ropeSegment.SideStartShift = MBRandom.RandomFloat - 0.5f;
					ropeSegment.SideEndShift = MBRandom.RandomFloat - 0.5f;
					if (ropeSegment.StartSegmentIndex < ropeSegment.EndSegmentIndex && ropeSegment.StartSegmentIndex > 0 && ropeSegment.EndSegmentIndex > 0 && ropeSegment.StartSegmentIndex < this._numberOfPlanksNeeded && ropeSegment.EndSegmentIndex < this._numberOfPlanksNeeded)
					{
						GameEntity gameEntity = GameEntity.Instantiate(Mission.Current.Scene, "simple_rope_nested", MatrixFrame.Identity, true);
						this._bridge.AddChild(gameEntity, false);
						ropeSegment.ParentEntity = gameEntity;
						ropeSegment.ParentEntity.SetDoNotCheckVisibility(true);
						ropeSegment.RopeStart = gameEntity.GetFirstChildEntityWithTag("simple_rope_start");
						ropeSegment.RopeEnd = gameEntity.GetFirstChildEntityWithTag("simple_rope_end");
						if (ropeSegment.RopeStart != null && ropeSegment.RopeEnd != null)
						{
							NavalDLC.Missions.Objects.RopeSegment firstScriptOfType = ropeSegment.RopeStart.GetFirstScriptOfType<NavalDLC.Missions.Objects.RopeSegment>();
							if (firstScriptOfType != null)
							{
								firstScriptOfType.SetAsFixedEntity();
								firstScriptOfType.SetRuntimeLooseMultiplier(2f);
							}
							this._ropes.Add(ropeSegment);
							if (MBRandom.RandomFloat > 0.6f)
							{
								int num4 = MBRandom.RandomInt(1, 2);
								for (int j = 0; j < num4; j++)
								{
									string text = ShipAttachmentMachine.ShipAttachment._ropeClothFragmentPrefabList[MBRandom.RandomInt(0, ShipAttachmentMachine.ShipAttachment._ropeClothFragmentPrefabList.Count - 1)];
									GameEntity gameEntity2 = GameEntity.Instantiate(Mission.Current.Scene, text, MatrixFrame.Identity, true);
									ropeSegment.RopeStart.AddChild(gameEntity2, false);
								}
							}
						}
					}
				}
			}

			// Token: 0x06001B7C RID: 7036 RVA: 0x000B6C58 File Offset: 0x000B4E58
			private void ArrangeNavMeshBridge(Vec3 leftSource, Vec3 rightSource, Vec3 leftTarget, Vec3 rightTarget)
			{
				if (this._navMeshBridge == null || this.AttachmentSource == null || this.AttachmentTarget == null)
				{
					return;
				}
				Vec3 globalPosition = this.AttachmentSource.GameEntity.GlobalPosition;
				Vec3 globalPosition2 = this.AttachmentTarget.GameEntity.GlobalPosition;
				globalPosition.Distance(globalPosition2);
				MatrixFrame matrixFrame = MatrixFrame.CenterFrameOfTwoPoints(ref globalPosition, ref globalPosition2, Vec3.Up);
				matrixFrame.origin.z = matrixFrame.origin.z + 1.1f;
				matrixFrame.rotation.Orthonormalize();
				this._navMeshBridge.SetFrame(ref matrixFrame, true);
				this._shipBridgeNavmeshHolder.SetShipBridgeStartEndPositions(leftSource, rightSource, leftTarget, rightTarget);
				bool flag = this.IsNavmeshBridgeEntityUpsideDown();
				if (flag != this._isNavmeshBridgeDisabled)
				{
					this.SetAbilityOfNavmeshBridgeFaces(!flag);
					this._isNavmeshBridgeDisabled = flag;
				}
			}

			// Token: 0x06001B7D RID: 7037 RVA: 0x000B6D28 File Offset: 0x000B4F28
			public void Destroy()
			{
				if (this._bridgeCreated)
				{
					bool flag = this._faceSwapSideOneDone || this._faceSwapSideTwoDone;
					if (this._faceSwapSideOneDone)
					{
						Mission.Current.Scene.SwapFaceConnectionsWithID(this._bridgeNavmeshId + 1, this.AttachmentTarget.RelatedShipNavmeshOffset + this.AttachmentTarget.OwnerShip.GetDynamicNavmeshIdStart(), this._bridgeNavmeshId + 3, true);
						this._faceSwapSideOneDone = false;
					}
					if (this._faceSwapSideTwoDone)
					{
						Mission.Current.Scene.SwapFaceConnectionsWithID(this._bridgeNavmeshId + 2, this.AttachmentSource.RelatedShipNavmeshOffset + this.AttachmentSource.OwnerShip.GetDynamicNavmeshIdStart(), this._bridgeNavmeshId + 4, true);
						this._faceSwapSideTwoDone = false;
					}
					if (flag)
					{
						this.AttachmentSource.OwnerShip.SeparateFromShip(this.AttachmentTarget.OwnerShip);
					}
					SoundManager.StartOneShotEvent("event:/mission/movement/vessel/bridge_fall", ref this.AttachmentSource.PlankBridgePhysicsEntity.GetGlobalFrame().origin);
				}
				this.AttachmentSource.CurrentAttachment = null;
				ShipAttachmentPointMachine attachmentTarget = this.AttachmentTarget;
				if (attachmentTarget != null)
				{
					attachmentTarget.AssignConnection(null);
				}
				if (this._planks != null)
				{
					foreach (GameEntity gameEntity in this._planks)
					{
						gameEntity.Remove(78);
					}
					this._planks = null;
				}
				if (this._targetSafetyPlanks != null)
				{
					foreach (GameEntity gameEntity2 in this._targetSafetyPlanks)
					{
						gameEntity2.Remove(35);
					}
					this._targetSafetyPlanks = null;
				}
				if (this._sourceSafetyPlanks != null)
				{
					foreach (GameEntity gameEntity3 in this._sourceSafetyPlanks)
					{
						gameEntity3.Remove(35);
					}
					this._sourceSafetyPlanks = null;
				}
				if (this._navMeshBridge != null)
				{
					this._navMeshBridge.Remove(78);
					Mission.Current.Scene.SetAbilityOfFacesWithId(this._bridgeNavmeshId, false);
					Mission.Current.Scene.SetAbilityOfFacesWithId(this._bridgeNavmeshId + 1, false);
					Mission.Current.Scene.SetAbilityOfFacesWithId(this._bridgeNavmeshId + 2, false);
					Mission.Current.Scene.SetAbilityOfFacesWithId(this._bridgeNavmeshId + 3, false);
					Mission.Current.Scene.SetAbilityOfFacesWithId(this._bridgeNavmeshId + 4, false);
					this._navMeshBridge = null;
				}
				this.AttachmentSource.SetConnectionPhysicsEntitiesVisibility(false);
				if (this._ropes != null)
				{
					foreach (ShipAttachmentMachine.ShipAttachment.RopeSegment ropeSegment in this._ropes)
					{
						ropeSegment.ParentEntity.Remove(45);
					}
					this._ropes = null;
				}
				if (this._bridge != null)
				{
					this._bridge.Remove(78);
					this._bridge = null;
				}
				this._bridgeCurveLinearAccessCache = null;
				if (this._currentFramePlankPhysicsVerticesPinnedPointer != UIntPtr.Zero)
				{
					this._currentFramePlankPhysicsVerticesPinnedGCHandler.Free();
					this._currentFramePlankPhysicsVerticesPinnedPointer = UIntPtr.Zero;
				}
				if (this._currentFramePlankPhysicsIndicesPinnedPointer != UIntPtr.Zero)
				{
					this._currentFramePlankPhysicsIndicesPinnedGCHandler.Free();
					this._currentFramePlankPhysicsIndicesPinnedPointer = UIntPtr.Zero;
				}
				if (this._sideBarriersQuadPinnedPointer != UIntPtr.Zero)
				{
					this._sideBarriersQuadPinnedGCHandler.Free();
					this._sideBarriersQuadPinnedPointer = UIntPtr.Zero;
				}
				if (this._sideBarriersIndicesPinnedPointer != UIntPtr.Zero)
				{
					this._sideBarriersIndicesPinnedGCHandler.Free();
					this._sideBarriersIndicesPinnedPointer = UIntPtr.Zero;
				}
				if (this._vFoldQuadPinnedPointer != UIntPtr.Zero)
				{
					this._vFoldQuadPinnedGCHandler.Free();
					this._vFoldQuadPinnedPointer = UIntPtr.Zero;
				}
				if (this._vFoldIndicesPinnedPointer != UIntPtr.Zero)
				{
					this._vFoldIndicesPinnedGCHandler.Free();
					this._vFoldIndicesPinnedPointer = UIntPtr.Zero;
				}
			}

			// Token: 0x06001B7E RID: 7038 RVA: 0x000B7158 File Offset: 0x000B5358
			private Vec3 GetCurvePositionFromLength(float currentLength)
			{
				int num = Array.BinarySearch<KeyValuePair<float, Vec3>>(this._bridgeCurveLinearAccessCache, new KeyValuePair<float, Vec3>(currentLength, Vec3.Zero), ShipAttachmentMachine.ShipAttachment._cacheCompareDelegate);
				if (num >= 0)
				{
					return this._bridgeCurveLinearAccessCache[num].Value;
				}
				int num2 = ~num;
				int num3 = num2 - 1;
				KeyValuePair<float, Vec3> keyValuePair = this._bridgeCurveLinearAccessCache[num3];
				KeyValuePair<float, Vec3> keyValuePair2 = this._bridgeCurveLinearAccessCache[num2];
				float num4 = (currentLength - keyValuePair.Key) / (keyValuePair2.Key - keyValuePair.Key);
				return Vec3.Lerp(keyValuePair.Value, keyValuePair2.Value, num4);
			}

			// Token: 0x06001B7F RID: 7039 RVA: 0x000B71EC File Offset: 0x000B53EC
			private void SetRopeMeshParams(Mesh ropeMesh, Vec3 start, Vec3 end, float length)
			{
				if (ropeMesh != null)
				{
					MatrixFrame identity = MatrixFrame.Identity;
					identity.rotation.s = start;
					identity.origin = end;
					ropeMesh.SetAdditionalBoneFrame(0, ref identity);
					MatrixFrame identity2 = MatrixFrame.Identity;
					ropeMesh.SetAdditionalBoneFrame(1, ref identity2);
					Vec3 vectorArgument = ropeMesh.GetVectorArgument();
					vectorArgument.x = length;
					vectorArgument.y = 25.9f;
					vectorArgument.z = 1f;
					ropeMesh.SetVectorArgument(vectorArgument.x, vectorArgument.y, vectorArgument.z, vectorArgument.w);
				}
			}

			// Token: 0x06001B80 RID: 7040 RVA: 0x000B727B File Offset: 0x000B547B
			private static Vec3 GetPositionAtProjectileCurveProgress(in Vec3 globalVelocity, in Vec3 sourceGlobalPosition, float time, float progressInterval)
			{
				time *= progressInterval;
				return sourceGlobalPosition + globalVelocity * time + 0.5f * MBGlobals.GravitationalAcceleration * time * time;
			}

			// Token: 0x06001B81 RID: 7041 RVA: 0x000B72BC File Offset: 0x000B54BC
			private void SetAbilityOfNavmeshBridgeFaces(bool enable)
			{
				Mission.Current.Scene.SetAbilityOfFacesWithId(this._bridgeNavmeshId, enable);
				Mission.Current.Scene.SetAbilityOfFacesWithId(this._bridgeNavmeshId + 1, enable);
				Mission.Current.Scene.SetAbilityOfFacesWithId(this._bridgeNavmeshId + 2, enable);
				Mission.Current.Scene.SetAbilityOfFacesWithId(this._bridgeNavmeshId + 3, enable);
				Mission.Current.Scene.SetAbilityOfFacesWithId(this._bridgeNavmeshId + 4, enable);
			}

			// Token: 0x06001B82 RID: 7042 RVA: 0x000B7344 File Offset: 0x000B5544
			private bool IsNavmeshBridgeEntityUpsideDown()
			{
				return this._navMeshBridge.GetGlobalFrame().rotation.u.z <= 0.35f;
			}

			// Token: 0x06001B83 RID: 7043 RVA: 0x000B736A File Offset: 0x000B556A
			private void AddNewClipPlaneIntersectionPoint(ref int numberOfValidVertices, in Vec3 currentCorner)
			{
				if (numberOfValidVertices < 5)
				{
					this._registeredVerticesAfterPhysicsClipPlaneIntersection[numberOfValidVertices] = currentCorner;
					numberOfValidVertices++;
				}
			}

			// Token: 0x06001B84 RID: 7044 RVA: 0x000B738C File Offset: 0x000B558C
			private void ArrangePlankPhysicsWithClipPlanes(Vec3[] quadVerticesCCW, MatrixFrame firstClipFrame, MatrixFrame secondClipFrame)
			{
				this._alreadyAddedVertexDataForPhysicsClipPlaneIntersection[0] = 0;
				this._alreadyAddedVertexDataForPhysicsClipPlaneIntersection[1] = 0;
				this._alreadyAddedVertexDataForPhysicsClipPlaneIntersection[2] = 0;
				this._alreadyAddedVertexDataForPhysicsClipPlaneIntersection[3] = 0;
				int num = 0;
				bool flag = false;
				for (int i = 0; i < 4; i++)
				{
					Vec3 vec = quadVerticesCCW[i];
					int num2 = (i + 1) % 4;
					Vec3 vec2 = quadVerticesCCW[num2];
					if (MBMath.PointLiesAheadOfPlane(ref firstClipFrame.rotation.f, ref firstClipFrame.origin, ref vec))
					{
						Vec3 vec3 = vec2 - vec;
						float num3 = vec3.Normalize();
						Vec3 vec4 = -firstClipFrame.rotation.f;
						float num4;
						if (MBMath.GetRayPlaneIntersectionPoint(ref vec4, ref firstClipFrame.origin, ref vec, ref vec3, ref num4) && num4 < num3)
						{
							Vec3 vec5 = vec + vec3 * num4;
							if (this._alreadyAddedVertexDataForPhysicsClipPlaneIntersection[i] == 0)
							{
								this.AddNewClipPlaneIntersectionPoint(ref num, in vec);
								this._alreadyAddedVertexDataForPhysicsClipPlaneIntersection[i] = 1;
							}
							this.AddNewClipPlaneIntersectionPoint(ref num, in vec5);
							flag = true;
						}
						else
						{
							if (this._alreadyAddedVertexDataForPhysicsClipPlaneIntersection[i] == 0)
							{
								this.AddNewClipPlaneIntersectionPoint(ref num, in vec);
								this._alreadyAddedVertexDataForPhysicsClipPlaneIntersection[i] = 1;
							}
							if (this._alreadyAddedVertexDataForPhysicsClipPlaneIntersection[num2] == 0)
							{
								this.AddNewClipPlaneIntersectionPoint(ref num, in vec2);
								this._alreadyAddedVertexDataForPhysicsClipPlaneIntersection[num2] = 1;
							}
						}
					}
					else
					{
						flag = true;
						Vec3 vec6 = vec - vec2;
						float num5 = vec6.Normalize();
						Vec3 vec4 = -firstClipFrame.rotation.f;
						float num6;
						if (MBMath.GetRayPlaneIntersectionPoint(ref vec4, ref firstClipFrame.origin, ref vec2, ref vec6, ref num6) && num6 < num5)
						{
							Vec3 vec7 = vec2 + vec6 * num6;
							this.AddNewClipPlaneIntersectionPoint(ref num, in vec7);
							if (this._alreadyAddedVertexDataForPhysicsClipPlaneIntersection[num2] == 0)
							{
								this.AddNewClipPlaneIntersectionPoint(ref num, in vec2);
								this._alreadyAddedVertexDataForPhysicsClipPlaneIntersection[num2] = 1;
							}
						}
					}
				}
				if (!flag)
				{
					this._alreadyAddedVertexDataForPhysicsClipPlaneIntersection[0] = 0;
					this._alreadyAddedVertexDataForPhysicsClipPlaneIntersection[1] = 0;
					this._alreadyAddedVertexDataForPhysicsClipPlaneIntersection[2] = 0;
					this._alreadyAddedVertexDataForPhysicsClipPlaneIntersection[3] = 0;
					num = 0;
					for (int j = 0; j < 4; j++)
					{
						Vec3 vec8 = quadVerticesCCW[j];
						int num7 = (j + 1) % 4;
						Vec3 vec9 = quadVerticesCCW[num7];
						if (MBMath.PointLiesAheadOfPlane(ref secondClipFrame.rotation.f, ref secondClipFrame.origin, ref vec8))
						{
							Vec3 vec10 = vec9 - vec8;
							float num8 = vec10.Normalize();
							Vec3 vec4 = -secondClipFrame.rotation.f;
							float num9;
							if (MBMath.GetRayPlaneIntersectionPoint(ref vec4, ref secondClipFrame.origin, ref vec8, ref vec10, ref num9) && num9 < num8)
							{
								Vec3 vec11 = vec8 + vec10 * num9;
								if (this._alreadyAddedVertexDataForPhysicsClipPlaneIntersection[j] == 0)
								{
									this.AddNewClipPlaneIntersectionPoint(ref num, in vec8);
									this._alreadyAddedVertexDataForPhysicsClipPlaneIntersection[j] = 1;
								}
								this.AddNewClipPlaneIntersectionPoint(ref num, in vec11);
							}
							else
							{
								if (this._alreadyAddedVertexDataForPhysicsClipPlaneIntersection[j] == 0)
								{
									this.AddNewClipPlaneIntersectionPoint(ref num, in vec8);
									this._alreadyAddedVertexDataForPhysicsClipPlaneIntersection[j] = 1;
								}
								if (this._alreadyAddedVertexDataForPhysicsClipPlaneIntersection[num7] == 0)
								{
									this.AddNewClipPlaneIntersectionPoint(ref num, in vec9);
									this._alreadyAddedVertexDataForPhysicsClipPlaneIntersection[num7] = 1;
								}
							}
						}
						else
						{
							Vec3 vec12 = vec8 - vec9;
							float num10 = vec12.Normalize();
							Vec3 vec4 = -secondClipFrame.rotation.f;
							float num11;
							if (MBMath.GetRayPlaneIntersectionPoint(ref vec4, ref secondClipFrame.origin, ref vec9, ref vec12, ref num11) && num11 < num10)
							{
								Vec3 vec13 = vec9 + vec12 * num11;
								this.AddNewClipPlaneIntersectionPoint(ref num, in vec13);
								if (this._alreadyAddedVertexDataForPhysicsClipPlaneIntersection[num7] == 0)
								{
									this.AddNewClipPlaneIntersectionPoint(ref num, in vec9);
									this._alreadyAddedVertexDataForPhysicsClipPlaneIntersection[num7] = 1;
								}
							}
						}
					}
				}
				if (num >= 3)
				{
					bool flag2 = true;
					for (int k = 0; k < num; k++)
					{
						Vec3 vec14 = this._registeredVerticesAfterPhysicsClipPlaneIntersection[k];
						Vec3 vec15 = this._registeredVerticesAfterPhysicsClipPlaneIntersection[(k + 1) % num];
						if (vec14.DistanceSquared(vec15) < 1E-06f)
						{
							flag2 = false;
							break;
						}
					}
					if (flag2)
					{
						int num12 = 0;
						for (int l = 0; l < num; l++)
						{
							int num13 = this.AddNewVertexToPlankPhysics(this._registeredVerticesAfterPhysicsClipPlaneIntersection[l]);
							if (num13 == -1)
							{
								return;
							}
							if (l == 0)
							{
								num12 = num13;
							}
						}
						int num14 = num - 2;
						for (int m = 0; m < num14; m++)
						{
							this.AddNewIndexToPlankPhysics(num12);
							this.AddNewIndexToPlankPhysics(num12 + m + 1);
							this.AddNewIndexToPlankPhysics(num12 + m + 2);
						}
					}
				}
			}

			// Token: 0x06001B85 RID: 7045 RVA: 0x000B77CC File Offset: 0x000B59CC
			private int AddNewVertexToPlankPhysics(Vec3 vertex)
			{
				if (this._currentFramePlankPhysicsVertices.Length > this._currentFramePlankPhysicsVertexCount)
				{
					this._currentFramePlankPhysicsVertices[this._currentFramePlankPhysicsVertexCount] = vertex;
					int currentFramePlankPhysicsVertexCount = this._currentFramePlankPhysicsVertexCount;
					this._currentFramePlankPhysicsVertexCount++;
					return currentFramePlankPhysicsVertexCount;
				}
				return -1;
			}

			// Token: 0x06001B86 RID: 7046 RVA: 0x000B7806 File Offset: 0x000B5A06
			private void AddNewIndexToPlankPhysics(int index)
			{
				if (this._currentFramePlankPhysicsIndices.Length > this._currentFramePlankPhysicsIndexCount)
				{
					this._currentFramePlankPhysicsIndices[this._currentFramePlankPhysicsIndexCount] = index;
					this._currentFramePlankPhysicsIndexCount++;
				}
			}

			// Token: 0x06001B87 RID: 7047 RVA: 0x000B7834 File Offset: 0x000B5A34
			private void TransformCurrentFramePlankPhysicsVerticesToPhysicsEntityLocal(Vec3 physicsEntityGlobalPosition)
			{
				for (int i = 0; i < this._currentFramePlankPhysicsVertices.Length; i++)
				{
					this._currentFramePlankPhysicsVertices[i] -= physicsEntityGlobalPosition;
				}
			}

			// Token: 0x06001B88 RID: 7048 RVA: 0x000B7874 File Offset: 0x000B5A74
			private void SpawnPlankEntities()
			{
				this._bridge = GameEntity.CreateEmpty(Mission.Current.Scene, true, true, true);
				for (int i = this._planks.Count; i < 80; i++)
				{
					string text = ShipAttachmentMachine.ShipAttachment._shipConnectionPlankVariations[MBRandom.RandomInt(0, ShipAttachmentMachine.ShipAttachment._shipConnectionPlankVariations.Count - 1)];
					GameEntity gameEntity = GameEntity.Instantiate(Mission.Current.Scene, text, MatrixFrame.Identity, true);
					this._bridge.AddChild(gameEntity, false);
					this._planks.Add(gameEntity);
					gameEntity.SetupAdditionalBoneBufferForMeshes(1);
				}
			}

			// Token: 0x06001B89 RID: 7049 RVA: 0x000B7904 File Offset: 0x000B5B04
			private void FillBridgeCurveAccessData(in Vec3 plankTargetOrigin, in Vec3 plankSourceOrigin, in float curvedLength)
			{
				this._bridgeCurveLinearAccessCache[0] = new KeyValuePair<float, Vec3>(0f, plankTargetOrigin);
				Vec3 vec = plankTargetOrigin;
				float num = 0.06666667f;
				float num2 = 0f;
				for (int i = 1; i < 15; i++)
				{
					Vec3 vec2 = NavalDLC.Missions.Objects.RopeSegment.CalculateAutoCurvePosition(plankTargetOrigin, plankSourceOrigin, curvedLength, (float)i * num);
					float num3 = vec2.Distance(vec);
					num2 += num3;
					this._bridgeCurveLinearAccessCache[i] = new KeyValuePair<float, Vec3>(num2, vec2);
					vec = vec2;
				}
				this._bridgeCurveLinearAccessCache[15] = new KeyValuePair<float, Vec3>(curvedLength, plankSourceOrigin);
			}

			// Token: 0x06001B8A RID: 7050 RVA: 0x000B79A8 File Offset: 0x000B5BA8
			private void ArrangePlanksMT()
			{
				Vec3 vec = (this.AttachmentSource.GameEntity.GetGlobalFrame().origin + this.AttachmentTarget.GameEntity.GetGlobalFrame().origin) * 0.5f;
				this.AttachmentSource.PlankBridgePhysicsEntity.SetLocalPosition(vec);
				this._currentFramePlankPhysicsIndexCount = 0;
				this._currentFramePlankPhysicsVertexCount = 0;
				MatrixFrame globalFrame = this.AttachmentSource.ConnectionClipPlaneEntity.GetGlobalFrame();
				Vec3 origin = globalFrame.origin;
				MatrixFrame globalFrame2 = this.AttachmentTarget.ConnectionClipPlaneEntity.GetGlobalFrame();
				Vec3 origin2 = globalFrame2.origin;
				Vec3 vec2 = origin - origin2;
				vec2.Normalize();
				MatrixFrame identity = MatrixFrame.Identity;
				identity.rotation.f = vec2;
				identity.rotation.s = vec2.CrossProductWithUp();
				identity.rotation.s.Normalize();
				identity.rotation.u = Vec3.CrossProduct(identity.rotation.s, identity.rotation.f);
				identity.rotation.u.Normalize();
				float num = origin.Distance(origin2);
				float num2 = 1.035f;
				if (this._state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeThrown)
				{
					float num3 = MathF.Sin(this._bridgeFlightData.CurveLerpVelocity * 3.1415927f);
					float num4 = (this._bridgeFlightData.ThrowFinishValue - this._bridgeFlightData.CurveLerpValue) / this._bridgeFlightData.ThrowFinishValue;
					float num5 = Math.Min((this._bridgeFlightData.CurveLerpValue - 0.5f) * 2f, 1f);
					num2 += num3 * num4 * num5 * 0.028f;
				}
				this._previousNumberOfPlanksNeeded = this._numberOfPlanksNeeded;
				float num6 = num * num2;
				this._numberOfPlanksNeeded = MathF.Max(MathF.Ceiling(num6 / this._plankVerticalSize), 2);
				this._numberOfPlanksNeeded = Math.Min(this._numberOfPlanksNeeded, 80);
				this.FillBridgeCurveAccessData(in origin2, in origin, in num6);
				Vec3 vec3 = -globalFrame.rotation.s;
				MatrixFrame identity2 = MatrixFrame.Identity;
				identity2.origin = this.GetCurvePositionFromLength(0f);
				Vec3 curvePositionFromLength = this.GetCurvePositionFromLength(MathF.Min(this._plankVerticalSize, num6));
				identity2.rotation.f = curvePositionFromLength - identity2.origin;
				identity2.rotation.f.Normalize();
				identity2.rotation.f.CrossProductWithUp().Normalize();
				Vec3 s = globalFrame2.rotation.s;
				s.Normalize();
				Vec3 vec4 = Vec3.CrossProduct(globalFrame.rotation.f, vec3);
				vec4.Normalize();
				vec3 = Vec3.CrossProduct(vec4, globalFrame.rotation.f);
				vec3.Normalize();
				float num7 = (float)Math.Acos((double)Vec3.DotProduct(vec3, s));
				if (Vec3.DotProduct(Vec3.CrossProduct(s, vec3), globalFrame.rotation.f) < 0f)
				{
					num7 *= -1f;
				}
				float num8 = num7 / (float)this._numberOfPlanksNeeded;
				Vec3 vec5 = s;
				for (int i = 0; i < this._numberOfPlanksNeeded; i++)
				{
					bool flag = true;
					GameEntity gameEntity = this._planks[i];
					MatrixFrame matrixFrame = MatrixFrame.Identity;
					matrixFrame.origin = this.GetCurvePositionFromLength(MathF.Min((float)i * this._plankVerticalSize, num6));
					Vec3 curvePositionFromLength2 = this.GetCurvePositionFromLength(MathF.Min((float)(i + 1) * this._plankVerticalSize, num6));
					matrixFrame.rotation.f = curvePositionFromLength2 - matrixFrame.origin;
					if (matrixFrame.rotation.f.LengthSquared > 0f)
					{
						matrixFrame.rotation.f.Normalize();
					}
					else
					{
						matrixFrame.rotation.f = vec2;
					}
					matrixFrame.rotation.f = matrixFrame.rotation.f * 1.06f;
					matrixFrame.rotation.s = vec5;
					matrixFrame.rotation.s.Normalize();
					matrixFrame.rotation.u = Vec3.CrossProduct(matrixFrame.rotation.s, matrixFrame.rotation.f);
					matrixFrame.rotation.u.Normalize();
					MatrixFrame identity3 = MatrixFrame.Identity;
					identity3.rotation.RotateAboutForward(num8);
					gameEntity.SetBoneFrameToAllMeshes(0, ref identity3);
					gameEntity.SetVectorArgument(1f / this._plankVerticalSize, 0f, 0f, 0f);
					vec5 = Vec3.Lerp(s, vec3, (float)i / (float)this._numberOfPlanksNeeded);
					if (this._state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeThrown)
					{
						MatrixFrame identity4 = MatrixFrame.Identity;
						float num9 = MathF.Min(this._bridgeFlightData.DtSinceFlightStart, this._bridgeFlightData.CurrentFrameTotalLightTime);
						int num10 = this._numberOfPlanksNeeded - i - 1;
						float num11 = (float)num10 / (float)(this._numberOfPlanksNeeded - 1);
						identity4.origin = ShipAttachmentMachine.ShipAttachment.GetPositionAtProjectileCurveProgress(in this._bridgeFlightData.CurrentFrameInitialVelocity, in origin, num9, num11);
						float num12 = (float)(num10 - 1) / (float)(this._numberOfPlanksNeeded - 1);
						Vec3 positionAtProjectileCurveProgress = ShipAttachmentMachine.ShipAttachment.GetPositionAtProjectileCurveProgress(in this._bridgeFlightData.CurrentFrameInitialVelocity, in origin, num9, num12);
						identity4.rotation.f = positionAtProjectileCurveProgress - identity4.origin;
						if ((double)identity4.rotation.f.LengthSquared < 0.1)
						{
							flag = false;
						}
						else
						{
							identity4.rotation.f.Normalize();
							identity4.rotation.s = identity4.rotation.f.CrossProductWithUp();
							identity4.rotation.s.Normalize();
							identity4.rotation.u = Vec3.CrossProduct(identity4.rotation.s, identity4.rotation.f);
							identity4.rotation.u.Normalize();
						}
						float num13 = Math.Min(this._bridgeFlightData.CurveLerpValue, 1f);
						matrixFrame = MatrixFrame.Lerp(ref identity4, ref matrixFrame, num13);
					}
					gameEntity.SetGlobalFrame(ref matrixFrame, true);
					gameEntity.SetVisibilityExcludeParents(flag);
					gameEntity.SetCustomClipPlane(Vec3.Zero, Vec3.Zero, true);
					if (this._state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected || this._state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeThrown)
					{
						Vec3 vec6;
						vec6..ctor(-this._plankHorizontalSize * 0.5f, -0.2f, 0f, -1f);
						vec6 = matrixFrame.TransformToParent(ref vec6);
						Vec3 vec7;
						vec7..ctor(this._plankHorizontalSize * 0.5f, -0.2f, 0f, -1f);
						vec7 = matrixFrame.TransformToParent(ref vec7);
						Vec3 vec8;
						vec8..ctor(-this._plankHorizontalSize * 0.5f, 0.2f + this._plankVerticalSize, 0f, -1f);
						vec8 = matrixFrame.TransformToParent(ref vec8);
						Vec3 vec9;
						vec9..ctor(this._plankHorizontalSize * 0.5f, 0.2f + this._plankVerticalSize, 0f, -1f);
						vec9 = matrixFrame.TransformToParent(ref vec9);
						this._quadVerticesCCWCached[0] = vec6;
						this._quadVerticesCCWCached[1] = vec7;
						this._quadVerticesCCWCached[2] = vec9;
						this._quadVerticesCCWCached[3] = vec8;
						this.ArrangePlankPhysicsWithClipPlanes(this._quadVerticesCCWCached, globalFrame, globalFrame2);
					}
				}
				for (int j = this._numberOfPlanksNeeded; j < this._previousNumberOfPlanksNeeded; j++)
				{
					this._planks[j].SetVisibilityExcludeParents(false);
				}
				if ((this._state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeConnected || this._state == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BridgeThrown) && this._numberOfPlanksNeeded > 0)
				{
					MatrixFrame globalFrame3 = this._planks[this._numberOfPlanksNeeded - 1].GetGlobalFrame();
					Vec3 vec10 = globalFrame3.origin + globalFrame3.rotation.f * this._plankVerticalSize;
					MatrixFrame identity5 = MatrixFrame.Identity;
					identity5.rotation.u = globalFrame3.rotation.u;
					identity5.rotation.u.Normalize();
					identity5.rotation.s = Vec3.CrossProduct(globalFrame3.rotation.f, identity5.rotation.u);
					identity5.rotation.s.Normalize();
					identity5.rotation.f = Vec3.CrossProduct(identity5.rotation.u, identity5.rotation.s);
					identity5.rotation.f.Normalize();
					for (int k = 0; k < this._sourceSafetyPlanks.Count; k++)
					{
						GameEntity gameEntity2 = this._sourceSafetyPlanks[k];
						gameEntity2.SetVisibilityExcludeParents(false);
						MatrixFrame identity6 = MatrixFrame.Identity;
						identity6.origin = vec10 + identity5.rotation.f * this._plankVerticalSize * (float)k;
						identity6.rotation = identity5.rotation;
						gameEntity2.SetGlobalFrame(ref identity6, true);
						gameEntity2.SetCustomClipPlane(origin, globalFrame.rotation.f, true);
						Vec3 vec11;
						vec11..ctor(-this._plankHorizontalSize * 0.5f, -0.2f, 0f, -1f);
						vec11 = identity6.TransformToParent(ref vec11);
						Vec3 vec12;
						vec12..ctor(this._plankHorizontalSize * 0.5f, -0.2f, 0f, -1f);
						vec12 = identity6.TransformToParent(ref vec12);
						Vec3 vec13;
						vec13..ctor(-this._plankHorizontalSize * 0.5f, 0.2f + this._plankVerticalSize, 0f, -1f);
						vec13 = identity6.TransformToParent(ref vec13);
						Vec3 vec14;
						vec14..ctor(this._plankHorizontalSize * 0.5f, 0.2f + this._plankVerticalSize, 0f, -1f);
						vec14 = identity6.TransformToParent(ref vec14);
						this._quadVerticesCCWCached[0] = vec11;
						this._quadVerticesCCWCached[1] = vec12;
						this._quadVerticesCCWCached[2] = vec14;
						this._quadVerticesCCWCached[3] = vec13;
						this.ArrangePlankPhysicsWithClipPlanes(this._quadVerticesCCWCached, globalFrame, globalFrame2);
					}
					MatrixFrame globalFrame4 = this._planks[0].GetGlobalFrame();
					for (int l = 0; l < this._targetSafetyPlanks.Count; l++)
					{
						GameEntity gameEntity3 = this._targetSafetyPlanks[l];
						gameEntity3.SetVisibilityExcludeParents(false);
						MatrixFrame identity7 = MatrixFrame.Identity;
						identity7.origin = globalFrame4.origin - globalFrame4.rotation.f * this._plankVerticalSize * (float)(l + 1);
						identity7.rotation = globalFrame4.rotation;
						gameEntity3.SetGlobalFrame(ref identity7, true);
						gameEntity3.SetCustomClipPlane(origin2, globalFrame2.rotation.f, true);
						Vec3 vec15;
						vec15..ctor(-this._plankHorizontalSize * 0.5f, -0.2f, 0f, -1f);
						vec15 = identity7.TransformToParent(ref vec15);
						Vec3 vec16;
						vec16..ctor(this._plankHorizontalSize * 0.5f, -0.2f, 0f, -1f);
						vec16 = identity7.TransformToParent(ref vec16);
						Vec3 vec17;
						vec17..ctor(-this._plankHorizontalSize * 0.5f, 0.2f + this._plankVerticalSize, 0f, -1f);
						vec17 = identity7.TransformToParent(ref vec17);
						Vec3 vec18;
						vec18..ctor(this._plankHorizontalSize * 0.5f, 0.2f + this._plankVerticalSize, 0f, -1f);
						vec18 = identity7.TransformToParent(ref vec18);
						this.ArrangePlankPhysicsWithClipPlanes(new Vec3[] { vec15, vec16, vec18, vec17 }, globalFrame, globalFrame2);
					}
				}
				int num14 = 0;
				while (num14 < 3 && num14 < this._planks.Count)
				{
					this._planks[num14].SetCustomClipPlane(origin2, globalFrame2.rotation.f, true);
					num14++;
				}
				for (int m = 0; m < 3; m++)
				{
					int num15 = this._numberOfPlanksNeeded - 1 - m;
					if (num15 >= 0)
					{
						this._planks[num15].SetCustomClipPlane(origin, globalFrame.rotation.f, true);
					}
				}
				foreach (ShipAttachmentMachine.ShipAttachment.RopeSegment ropeSegment in this._ropes)
				{
					Vec3 vec19 = ropeSegment.SideStartShift * identity.rotation.s * this._plankHorizontalSize;
					Vec3 vec20 = ropeSegment.SideEndShift * identity.rotation.s * this._plankHorizontalSize;
					int startSegmentIndex = ropeSegment.StartSegmentIndex;
					int num16 = Math.Min(ropeSegment.EndSegmentIndex, this._numberOfPlanksNeeded - 1);
					if (startSegmentIndex >= num16)
					{
						ropeSegment.ParentEntity.SetVisibilityExcludeParents(false);
					}
					else
					{
						MatrixFrame globalFrame5 = ropeSegment.RopeStart.GetGlobalFrame();
						globalFrame5.origin = this._planks[startSegmentIndex].GetGlobalFrame().origin + vec19;
						ropeSegment.RopeStart.SetGlobalFrame(ref globalFrame5, true);
						MatrixFrame globalFrame6 = ropeSegment.RopeEnd.GetGlobalFrame();
						globalFrame6.origin = this._planks[num16].GetGlobalFrame().origin + vec20;
						ropeSegment.RopeEnd.SetGlobalFrame(ref globalFrame6, true);
						ropeSegment.ParentEntity.SetVisibilityExcludeParents(true);
					}
				}
				if (this._currentFramePlankPhysicsIndexCount > 0)
				{
					this.TransformCurrentFramePlankPhysicsVerticesToPhysicsEntityLocal(this.AttachmentSource.PlankBridgePhysicsEntity.GlobalPosition);
				}
			}

			// Token: 0x06001B8B RID: 7051 RVA: 0x000B877C File Offset: 0x000B697C
			private void ArrangePlanks()
			{
				if (this._currentFramePlankPhysicsIndexCount > 0)
				{
					GameEntityPhysicsExtensions.ReplacePhysicsBodyWithQuadPhysicsBody(this.AttachmentSource.PlankBridgePhysicsEntity, this._currentFramePlankPhysicsVerticesPinnedPointer, this._currentFramePlankPhysicsVertexCount, this._woodPhysicsMaterialCached, 2099220, this._currentFramePlankPhysicsIndicesPinnedPointer, this._currentFramePlankPhysicsIndexCount);
					BodyFlags physicsDescBodyFlag = this.AttachmentSource.PlankBridgePhysicsEntity.PhysicsDescBodyFlag;
					if (Extensions.HasAnyFlag<BodyFlags>(physicsDescBodyFlag, 1))
					{
						this.AttachmentSource.PlankBridgePhysicsEntity.SetBodyFlags(physicsDescBodyFlag & -2);
						return;
					}
				}
				else
				{
					BodyFlags physicsDescBodyFlag2 = this.AttachmentSource.PlankBridgePhysicsEntity.PhysicsDescBodyFlag;
					if (!Extensions.HasAnyFlag<BodyFlags>(physicsDescBodyFlag2, 1))
					{
						this.AttachmentSource.PlankBridgePhysicsEntity.SetBodyFlags(physicsDescBodyFlag2 | 1);
					}
				}
			}

			// Token: 0x06001B8C RID: 7052 RVA: 0x000B8824 File Offset: 0x000B6A24
			public Vec3 GetLaunchProjectileCurrentGlobalPosition(float time)
			{
				return this._launchFlightData.SourceGlobalPosition + this._launchFlightData.GlobalVelocity * time + 0.5f * MBGlobals.GravitationalAcceleration * time * time;
			}

			// Token: 0x06001B8D RID: 7053 RVA: 0x000B8874 File Offset: 0x000B6A74
			private static ValueTuple<Vec3, float> CalculateInitialVelocityAndTime(Vec3 initialPosition, Vec3 destination, float verticalLaunchAngleDegree)
			{
				float num = destination.x - initialPosition.x;
				float num2 = destination.y - initialPosition.y;
				float num3 = destination.z - initialPosition.z;
				float num4 = verticalLaunchAngleDegree * 3.1415927f / 180f;
				float num5 = (float)Math.Sqrt((double)(num * num + num2 * num2));
				float num6 = ShipAttachmentMachine.ShipAttachment.CalculateInitialVelocityMagnitude(num5, num3, num4);
				float num7 = (float)Math.Atan2((double)num2, (double)num);
				float num8 = num6 * (float)Math.Cos((double)num4) * (float)Math.Cos((double)num7);
				float num9 = num6 * (float)Math.Cos((double)num4) * (float)Math.Sin((double)num7);
				float num10 = num6 * (float)Math.Sin((double)num4);
				Vec3 vec = new Vec3(num8, num9, num10, -1f);
				float num11 = num5 / (num6 * (float)Math.Cos((double)num4));
				return new ValueTuple<Vec3, float>(vec, num11);
			}

			// Token: 0x06001B8E RID: 7054 RVA: 0x000B8944 File Offset: 0x000B6B44
			private static float CalculateLaunchAngleDegree(Vec3 initialPosition, Vec3 targetPosition, float launchSpeed)
			{
				Vec3 vec = targetPosition - initialPosition;
				float num = launchSpeed * launchSpeed;
				float length = vec.AsVec2.Length;
				float z = vec.z;
				float num2 = num * num;
				float num3 = 9.806f * (9.806f * length * length + 2f * z * num);
				if (num2 >= num3)
				{
					float num4 = MathF.Sqrt(num2 - num3);
					return MathF.Atan((num - num4) / (9.806f * length)) * 180f / 3.1415927f;
				}
				return float.MinValue;
			}

			// Token: 0x06001B8F RID: 7055 RVA: 0x000B89CC File Offset: 0x000B6BCC
			private static float CalculateInitialVelocityMagnitude(float distanceXY, float deltaZ, float thetaZ)
			{
				float num = (float)Math.Tan((double)thetaZ);
				float num2 = (float)Math.Cos((double)thetaZ);
				double num3 = (double)(9.806f * distanceXY * distanceXY);
				float num4 = 2f * num2 * num2 * (distanceXY * num - deltaZ);
				return (float)Math.Sqrt(num3 / (double)num4);
			}

			// Token: 0x06001B90 RID: 7056 RVA: 0x000B8A10 File Offset: 0x000B6C10
			private static float CalculateDifferenceVectorAngle(in Vec3 initialPosition, in Vec3 destination)
			{
				Vec3 vec = destination - initialPosition;
				float length = vec.AsVec2.Length;
				return (float)Math.Atan2((double)vec.z, (double)length) * 57.295776f;
			}

			// Token: 0x04000FBD RID: 4029
			private const string NavMeshHolderTag = "navmesh_holder";

			// Token: 0x04000FBE RID: 4030
			private const string HookImpactWater = "event:/mission/movement/vessel/hook_impact_fail_water_splash";

			// Token: 0x04000FBF RID: 4031
			private const string HookImpactAttachSuccess = "event:/mission/movement/vessel/hook_impact_attach";

			// Token: 0x04000FC0 RID: 4032
			private const string HookImpactAttachFail = "event:/mission/movement/vessel/hook_impact_fail_to_attach";

			// Token: 0x04000FC1 RID: 4033
			private const string HookThrowingSoundEvent = "event:/mission/movement/vessel/hook_throw";

			// Token: 0x04000FC2 RID: 4034
			private const string BridgeThrownSoundEvent = "event:/mission/movement/vessel/bridge_connect";

			// Token: 0x04000FC3 RID: 4035
			private const string BridgeBrokenSoundEvent = "event:/mission/movement/vessel/bridge_fall";

			// Token: 0x04000FC4 RID: 4036
			private const string HookBeforeAttachmentSoundEvent = "event:/mission/movement/vessel/hook_attach_point_snap";

			// Token: 0x04000FC5 RID: 4037
			private const float ForwardRotationLimitAngleCos = 0.17364818f;

			// Token: 0x04000FC6 RID: 4038
			private const float RopesPullingInteractionDistanceSquared = 2500f;

			// Token: 0x04000FC7 RID: 4039
			private const float BridgeConnectedInteractionDistanceSquared = 100f;

			// Token: 0x04000FC8 RID: 4040
			private const float BridgeConnectedAngleCosLimit = 0.18f;

			// Token: 0x04000FC9 RID: 4041
			private const int BridgeCurveLinearSampleCount = 16;

			// Token: 0x04000FCA RID: 4042
			private const int MaximumPlankCount = 80;

			// Token: 0x04000FCB RID: 4043
			private static readonly Comparer<KeyValuePair<float, Vec3>> _cacheCompareDelegate = Comparer<KeyValuePair<float, Vec3>>.Create((KeyValuePair<float, Vec3> x, KeyValuePair<float, Vec3> y) => x.Key.CompareTo(y.Key));

			// Token: 0x04000FD2 RID: 4050
			private bool _attachmentInitializedByPlayer;

			// Token: 0x04000FD3 RID: 4051
			private static List<string> _shipConnectionPlankVariations = new List<string> { "ship_connection_plank_no_physics_a", "ship_connection_plank_no_physics_b", "ship_connection_plank_no_physics_c", "ship_connection_plank_no_physics_d" };

			// Token: 0x04000FD4 RID: 4052
			private static List<string> _ropeClothFragmentPrefabList = new List<string> { "cloth_fragment_a", "cloth_fragment_b", "cloth_fragment_c", "cloth_fragment_g", "cloth_fragment_i", "cloth_fragment_d" };

			// Token: 0x04000FD5 RID: 4053
			private float _shipBetweenAttachmentsCheckTimer;

			// Token: 0x04000FD6 RID: 4054
			private MissionTimer _ropesPullingTimer;

			// Token: 0x04000FD7 RID: 4055
			private GameEntity _bridge;

			// Token: 0x04000FD8 RID: 4056
			private GameEntity _navMeshBridge;

			// Token: 0x04000FD9 RID: 4057
			private GameEntity _navMeshBridgeNavMeshHolder;

			// Token: 0x04000FDA RID: 4058
			private ShipAttachmentMachine.ShipBridgeNavmeshHolder _shipBridgeNavmeshHolder;

			// Token: 0x04000FDB RID: 4059
			private int _bridgeNavmeshId;

			// Token: 0x04000FDC RID: 4060
			private List<GameEntity> _planks = new List<GameEntity>();

			// Token: 0x04000FDD RID: 4061
			private List<GameEntity> _targetSafetyPlanks = new List<GameEntity>();

			// Token: 0x04000FDE RID: 4062
			private List<GameEntity> _sourceSafetyPlanks = new List<GameEntity>();

			// Token: 0x04000FDF RID: 4063
			private KeyValuePair<float, Vec3>[] _bridgeCurveLinearAccessCache = new KeyValuePair<float, Vec3>[16];

			// Token: 0x04000FE0 RID: 4064
			private int _previousNumberOfPlanksNeeded = 80;

			// Token: 0x04000FE1 RID: 4065
			private int _numberOfPlanksNeeded = 80;

			// Token: 0x04000FE2 RID: 4066
			private List<ShipAttachmentMachine.ShipAttachment.RopeSegment> _ropes = new List<ShipAttachmentMachine.ShipAttachment.RopeSegment>();

			// Token: 0x04000FE3 RID: 4067
			private ShipAttachmentMachine.ShipAttachment.BridgeFlightData _bridgeFlightData;

			// Token: 0x04000FE4 RID: 4068
			private bool _isNavmeshBridgeDisabled;

			// Token: 0x04000FE5 RID: 4069
			private float _plankVerticalSize;

			// Token: 0x04000FE6 RID: 4070
			private float _plankHorizontalSize;

			// Token: 0x04000FE7 RID: 4071
			private ShipAttachmentMachine.ShipAttachment.ShipAttachmentState _state;

			// Token: 0x04000FE8 RID: 4072
			private PhysicsMaterial _woodPhysicsMaterialCached;

			// Token: 0x04000FE9 RID: 4073
			private PhysicsMaterial _defaultPhysicsMaterialCached;

			// Token: 0x04000FEA RID: 4074
			private Vec3[] _sideBarrierQuadsCached = new Vec3[4];

			// Token: 0x04000FEB RID: 4075
			private UIntPtr _sideBarriersQuadPinnedPointer = UIntPtr.Zero;

			// Token: 0x04000FEC RID: 4076
			private GCHandle _sideBarriersQuadPinnedGCHandler;

			// Token: 0x04000FED RID: 4077
			private UIntPtr _sideBarriersIndicesPinnedPointer = UIntPtr.Zero;

			// Token: 0x04000FEE RID: 4078
			private GCHandle _sideBarriersIndicesPinnedGCHandler;

			// Token: 0x04000FEF RID: 4079
			private int[] _sideBarrierIndicesCached = new int[6];

			// Token: 0x04000FF0 RID: 4080
			private Vec3[] _vFoldQuadsCached = new Vec3[4];

			// Token: 0x04000FF1 RID: 4081
			private UIntPtr _vFoldQuadPinnedPointer = UIntPtr.Zero;

			// Token: 0x04000FF2 RID: 4082
			private GCHandle _vFoldQuadPinnedGCHandler;

			// Token: 0x04000FF3 RID: 4083
			private UIntPtr _vFoldIndicesPinnedPointer = UIntPtr.Zero;

			// Token: 0x04000FF4 RID: 4084
			private GCHandle _vFoldIndicesPinnedGCHandler;

			// Token: 0x04000FF5 RID: 4085
			private int[] _vFoldQuadsIndicesCached = new int[6];

			// Token: 0x04000FF6 RID: 4086
			private int[] _alreadyAddedVertexDataForPhysicsClipPlaneIntersection = new int[4];

			// Token: 0x04000FF7 RID: 4087
			private Vec3[] _registeredVerticesAfterPhysicsClipPlaneIntersection = new Vec3[5];

			// Token: 0x04000FF8 RID: 4088
			private Vec3[] _quadVerticesCCWCached = new Vec3[4];

			// Token: 0x04000FF9 RID: 4089
			private Vec3[] _currentFramePlankPhysicsVertices = new Vec3[200];

			// Token: 0x04000FFA RID: 4090
			private UIntPtr _currentFramePlankPhysicsVerticesPinnedPointer = UIntPtr.Zero;

			// Token: 0x04000FFB RID: 4091
			private GCHandle _currentFramePlankPhysicsVerticesPinnedGCHandler;

			// Token: 0x04000FFC RID: 4092
			private int _currentFramePlankPhysicsVertexCount;

			// Token: 0x04000FFD RID: 4093
			private int[] _currentFramePlankPhysicsIndices = new int[300];

			// Token: 0x04000FFE RID: 4094
			private int _currentFramePlankPhysicsIndexCount;

			// Token: 0x04000FFF RID: 4095
			private UIntPtr _currentFramePlankPhysicsIndicesPinnedPointer = UIntPtr.Zero;

			// Token: 0x04001000 RID: 4096
			private GCHandle _currentFramePlankPhysicsIndicesPinnedGCHandler;

			// Token: 0x04001001 RID: 4097
			private bool _faceSwapSideOneDone = true;

			// Token: 0x04001002 RID: 4098
			private bool _faceSwapSideTwoDone = true;

			// Token: 0x04001003 RID: 4099
			private bool _bridgeCreated;

			// Token: 0x04001004 RID: 4100
			private bool _hookAttachSoundAlreadyTriggered;

			// Token: 0x04001005 RID: 4101
			private Timer _bridgeSwapTimer;

			// Token: 0x04001006 RID: 4102
			private float _ropeThrownTimer;

			// Token: 0x04001007 RID: 4103
			private MatrixFrame _hookGlobalFrame;

			// Token: 0x04001008 RID: 4104
			private ShipAttachmentMachine.ShipAttachment.FlightData _launchFlightData;

			// Token: 0x04001009 RID: 4105
			private bool _currentRopeLengthFirstReachedFinalValue = true;

			// Token: 0x0400100A RID: 4106
			private float _currentRopeLength;

			// Token: 0x020002AF RID: 687
			public struct FlightData
			{
				// Token: 0x06001D09 RID: 7433 RVA: 0x000BA2D0 File Offset: 0x000B84D0
				public FlightData(in Vec3 sourceGlobalPosition, in Vec3 targetGlobalPosition, in Vec3 globalVelocity, float angleDegree, float time)
				{
					this.SourceGlobalPosition = sourceGlobalPosition;
					this.TargetGlobalPosition = targetGlobalPosition;
					this.GlobalVelocity = globalVelocity;
					this.AngleDegree = angleDegree;
					this.Time = time;
					this.GlobalPositionError = Vec3.Zero;
					this.IsUnderWater = false;
				}

				// Token: 0x04001167 RID: 4455
				public Vec3 SourceGlobalPosition;

				// Token: 0x04001168 RID: 4456
				public Vec3 TargetGlobalPosition;

				// Token: 0x04001169 RID: 4457
				public Vec3 GlobalPositionError;

				// Token: 0x0400116A RID: 4458
				public Vec3 GlobalVelocity;

				// Token: 0x0400116B RID: 4459
				public float AngleDegree;

				// Token: 0x0400116C RID: 4460
				public float Time;

				// Token: 0x0400116D RID: 4461
				public bool IsUnderWater;
			}

			// Token: 0x020002B0 RID: 688
			internal struct BridgeFlightData
			{
				// Token: 0x0400116E RID: 4462
				internal float DtSinceFlightStart;

				// Token: 0x0400116F RID: 4463
				internal float CurveLerpVelocity;

				// Token: 0x04001170 RID: 4464
				internal float CurveLerpValue;

				// Token: 0x04001171 RID: 4465
				internal float ThrowFinishValue;

				// Token: 0x04001172 RID: 4466
				internal float CurrentFrameTotalLightTime;

				// Token: 0x04001173 RID: 4467
				internal Vec3 CurrentFrameInitialVelocity;
			}

			// Token: 0x020002B1 RID: 689
			internal struct RopeSegment
			{
				// Token: 0x04001174 RID: 4468
				internal GameEntity ParentEntity;

				// Token: 0x04001175 RID: 4469
				internal GameEntity RopeStart;

				// Token: 0x04001176 RID: 4470
				internal GameEntity RopeEnd;

				// Token: 0x04001177 RID: 4471
				internal int StartSegmentIndex;

				// Token: 0x04001178 RID: 4472
				internal int EndSegmentIndex;

				// Token: 0x04001179 RID: 4473
				internal float SideStartShift;

				// Token: 0x0400117A RID: 4474
				internal float SideEndShift;
			}

			// Token: 0x020002B2 RID: 690
			public enum ShipAttachmentState
			{
				// Token: 0x0400117C RID: 4476
				RopeThrown,
				// Token: 0x0400117D RID: 4477
				RopesPulling,
				// Token: 0x0400117E RID: 4478
				BridgeThrown,
				// Token: 0x0400117F RID: 4479
				BridgeConnected,
				// Token: 0x04001180 RID: 4480
				BrokenAndWaitingForRemoval,
				// Token: 0x04001181 RID: 4481
				RopeFailedAndReloading
			}
		}
	}
}
