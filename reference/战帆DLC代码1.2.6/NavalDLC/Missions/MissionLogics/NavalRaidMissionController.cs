using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions.NavalPhysics;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.Objects.UsableMachines;
using NavalDLC.Missions.ShipControl;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.MissionLogics
{
	// Token: 0x020000D1 RID: 209
	public class NavalRaidMissionController : MissionLogic
	{
		// Token: 0x06000FB5 RID: 4021 RVA: 0x00077BA0 File Offset: 0x00075DA0
		public override void OnBehaviorInitialize()
		{
			this._shipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this._agentsLogic = Mission.Current.GetMissionBehavior<NavalAgentsLogic>();
			this._shipCollisionOutcomeLogic = Mission.Current.GetMissionBehavior<ShipCollisionOutcomeLogic>();
			this._shipsLogic.ShipPreparedForAbandonmentEvent += this.OnShipPreparedForAbandonment;
			this._shipsLogic.ShipSpawnedEvent += this.OnShipSpawned;
			this._shipsLogic.ShipCollisionEvent += this.OnShipCollision;
			this._landingFrames = new MatrixFrame[4][];
			for (int i = 0; i < this._landingFrames.Length; i++)
			{
				this._landingFrames[i] = new MatrixFrame[8];
			}
			foreach (GameEntity gameEntity in Mission.Current.Scene.FindEntitiesWithTagExpression("landing(_\\d+)*"))
			{
				for (int j = 0; j < 8; j++)
				{
					string mainTag = string.Format("landing_00{0}", j + 1);
					string text = gameEntity.Tags.FirstOrDefault<string>((string tag) => tag.Contains(mainTag));
					if (!string.IsNullOrEmpty(text))
					{
						int num;
						if (int.TryParse(text.Replace(mainTag + "_", ""), out num))
						{
							this._landingFrames[j][num] = gameEntity.GetGlobalFrame();
							break;
						}
						if (gameEntity.HasTag(text))
						{
							this._landingFrames[j][0] = gameEntity.GetGlobalFrame();
							break;
						}
					}
				}
			}
			this._jumpingFrames = new MatrixFrame[4];
			foreach (GameEntity gameEntity2 in Mission.Current.Scene.FindEntitiesWithTagExpression("jumping(_\\d+)*"))
			{
				for (int k = 0; k < 8; k++)
				{
					if (gameEntity2.HasTag(string.Format("jumping_00{0}", k + 1)))
					{
						this._jumpingFrames[k] = gameEntity2.GetGlobalFrame();
						break;
					}
				}
			}
			this._shipNextPathNodeIndices = new int[4];
			for (int l = 0; l < this._shipNextPathNodeIndices.Length; l++)
			{
				this._shipNextPathNodeIndices[l] = 8;
			}
		}

		// Token: 0x06000FB6 RID: 4022 RVA: 0x00077E10 File Offset: 0x00076010
		private void OnShipCollision(MissionShip ship, WeakGameEntity targetEntity, BodyFlags bodyFlags, Vec3 averageContactPoint, Vec3 totalImpulseOnShip, bool isFirstImpact)
		{
			if (isFirstImpact && targetEntity == null && Extensions.HasAnyFlag<BodyFlags>(bodyFlags, 33554432))
			{
				this._shipCollisionOutcomeLogic.ActivateCooldownForShip(ship, float.MaxValue);
			}
		}

		// Token: 0x06000FB7 RID: 4023 RVA: 0x00077E40 File Offset: 0x00076040
		private void OnShipSpawned(MissionShip ship)
		{
			foreach (UsableMachine usableMachine in MBExtensions.CollectScriptComponentsIncludingChildrenRecursive<UsableMachine>(ship.GameEntity))
			{
				foreach (StandingPoint standingPoint in usableMachine.StandingPoints)
				{
					standingPoint.SetIsDisabledForPlayersSynched(true);
				}
			}
			ship.ShipOrder.SetEnforcedSailUsage(-1);
			if (ship.ShipOrigin.IsPlayerShip)
			{
				WeakGameEntity firstChildEntityWithTagRecursive = ship.GameEntity.GetFirstChildEntityWithTagRecursive("sp_naval_raid_player_spawn");
				if (firstChildEntityWithTagRecursive != null)
				{
					GameEntity gameEntity = GameEntity.CreateFromWeakEntity(firstChildEntityWithTagRecursive);
					ship.SetPlayerStandingPointEntity(gameEntity);
				}
			}
		}

		// Token: 0x06000FB8 RID: 4024 RVA: 0x00077F18 File Offset: 0x00076118
		private void OnShipPreparedForAbandonment(MissionShip ship)
		{
			Vec3 vec = ship.Physics.PhysicsBoundingBoxWithoutChildren.center;
			vec = ship.GlobalFrame.TransformToParent(ref vec);
			SortedList<float, ShipAttachmentPointMachine> sortedList = new SortedList<float, ShipAttachmentPointMachine>();
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in ship.AttachmentPointMachines)
			{
				Vec3 globalPosition = shipAttachmentPointMachine.GameEntity.GlobalPosition;
				Vec3 f = this._jumpingFrames[ship.Index].rotation.f;
				f.Normalize();
				Vec3 vec2 = globalPosition - vec;
				vec2 += shipAttachmentPointMachine.GameEntity.GetGlobalFrame().rotation.f;
				vec2.Normalize();
				float num = Vec3.DotProduct(vec2, f);
				sortedList.Add(num, shipAttachmentPointMachine);
			}
			int num2 = 0;
			switch (ship.ShipOrigin.Hull.Type)
			{
			case 0:
				num2 = 4;
				break;
			case 1:
				num2 = 6;
				break;
			case 2:
				num2 = 8;
				break;
			}
			for (int i = 0; i < sortedList.Count; i++)
			{
				ShipAttachmentPointMachine shipAttachmentPointMachine2 = sortedList.Values[i];
				if (i >= sortedList.Count - num2)
				{
					shipAttachmentPointMachine2.SetJumpOffAction(ActionIndexCache.act_raid_jump);
				}
				else
				{
					shipAttachmentPointMachine2.SetIsDisabledForAI(true);
					shipAttachmentPointMachine2.SetScriptComponentToTick(shipAttachmentPointMachine2.GetTickRequirement());
					foreach (StandingPoint standingPoint in shipAttachmentPointMachine2.StandingPoints)
					{
						standingPoint.SetIsDisabledForPlayersSynched(true);
					}
				}
			}
		}

		// Token: 0x06000FB9 RID: 4025 RVA: 0x000780E0 File Offset: 0x000762E0
		public override void OnDeploymentFinished()
		{
			foreach (MissionShip missionShip in this._shipsLogic.AllShips)
			{
				if (missionShip.IsPlayerShip)
				{
					missionShip.SetPlayerStandingPointEntity(null);
				}
				GameEntityPhysicsExtensions.UpdateBodyRestOffset(missionShip.GameEntity, -missionShip.MissionShipObject.LandingDepth);
				for (int i = 7; i >= 0; i--)
				{
					if (!this._landingFrames[missionShip.Index][i].IsZero)
					{
						this._shipNextPathNodeIndices[missionShip.Index] = i;
						break;
					}
				}
				missionShip.SetController(ShipControllerType.AI, false);
				ShipOrder shipOrder = missionShip.ShipOrder;
				Vec2 asVec = this._landingFrames[missionShip.Index][this._shipNextPathNodeIndices[missionShip.Index]].origin.AsVec2;
				Vec2 vec = this._landingFrames[missionShip.Index][this._shipNextPathNodeIndices[missionShip.Index]].rotation.f.AsVec2;
				vec = vec.Normalized();
				shipOrder.SetShipMovementOrder(asVec, in vec);
				missionShip.SetCanBeTakenOver(false);
				if (missionShip.ShipSiegeWeapon != null)
				{
					missionShip.ShipSiegeWeapon.SetDisabledSynched();
					WeakGameEntity weakGameEntity = missionShip.ShipSiegeWeapon.GameEntity;
					while (weakGameEntity != null && !weakGameEntity.HasTag("upgrade_slot"))
					{
						weakGameEntity = weakGameEntity.Parent;
					}
					missionShip.ShipSiegeWeapon.GameEntity.SetVisibilityExcludeParents(false);
					List<WeakGameEntity> list = new List<WeakGameEntity>();
					weakGameEntity.GetChildrenRecursive(ref list);
					foreach (WeakGameEntity weakGameEntity2 in list)
					{
						weakGameEntity2.SetVisibilityExcludeParents(false);
					}
				}
			}
			Formation formation = base.Mission.DefenderTeam.GetFormation(1);
			if (formation != null)
			{
				formation.SetArrangementOrder(ArrangementOrder.ArrangementOrderScatter);
			}
			GameEntity gameEntity = Mission.Current.Scene.FindEntityWithTag("player_spawn_frame");
			if (gameEntity != null)
			{
				this._warningBellsSoundEvent = SoundEvent.CreateEventFromString("event:/mission/ambient/detail/warning_bells", Mission.Current.Scene);
				this._warningBellsSoundEvent.PlayInPosition(gameEntity.GetGlobalFrame().origin);
			}
		}

		// Token: 0x06000FBA RID: 4026 RVA: 0x00078348 File Offset: 0x00076548
		public override void OnMissionTick(float dt)
		{
			foreach (MissionShip missionShip in this._shipsLogic.AllShips)
			{
				if (missionShip.ShipOrder.MovementOrderEnum == ShipOrder.ShipMovementOrderEnum.Move)
				{
					Vec2 vec = this._landingFrames[missionShip.Index][this._shipNextPathNodeIndices[missionShip.Index]].origin.AsVec2;
					MatrixFrame matrixFrame = missionShip.GlobalFrame;
					float num = vec.DistanceSquared(matrixFrame.origin.AsVec2);
					if (this._shipNextPathNodeIndices[missionShip.Index] == 0)
					{
						if (num > 225f && num < 400f)
						{
							missionShip.GlobalFrame.rotation.u = Vec3.Up;
							NavalPhysics physics = missionShip.Physics;
							vec = this._landingFrames[missionShip.Index][0].origin.AsVec2;
							Vec2 asVec = this._landingFrames[missionShip.Index][0].rotation.f.AsVec2;
							physics.SetAnchorFrame(in vec, in asVec, 1f);
							missionShip.SetAnchor(true, false, 1f);
							missionShip.EnableBlockers();
							if (this._approachingShoutsPlayed.Add(missionShip))
							{
								string text = "event:/alerts/naval/getting_rammed";
								matrixFrame = missionShip.GlobalFrame;
								SoundManager.StartOneShotEvent(text, ref matrixFrame.origin);
							}
						}
						else if (!missionShip.BeingAbandoned && num < 225f)
						{
							missionShip.ShipOrder.SetShipStopOrder();
							string text2 = ((missionShip.ShipOrigin.Hull.Type == 2) ? "event:/mission/movement/vessel/ship_ground_heavy" : "event:/mission/movement/vessel/ship_ground");
							matrixFrame = missionShip.GlobalFrame;
							SoundManager.StartOneShotEvent(text2, ref matrixFrame.origin);
							string text3 = "event:/alerts/report/battle_winning";
							matrixFrame = missionShip.GlobalFrame;
							SoundManager.StartOneShotEvent(text3, ref matrixFrame.origin);
							missionShip.PrepareForAbandonment();
							missionShip.Formation.SetMovementOrder(MovementOrder.MovementOrderCharge);
							this._hasLandingStarted = true;
							(base.Mission.DefenderTeam.TeamAI as TeamAINavalRaidDefenderComponent).OnShipLanded();
						}
					}
					else if (num < 2500f)
					{
						this._shipNextPathNodeIndices[missionShip.Index]--;
						ShipOrder shipOrder = missionShip.ShipOrder;
						Vec2 asVec2 = this._landingFrames[missionShip.Index][this._shipNextPathNodeIndices[missionShip.Index]].origin.AsVec2;
						vec = this._landingFrames[missionShip.Index][this._shipNextPathNodeIndices[missionShip.Index]].rotation.f.AsVec2;
						shipOrder.SetShipMovementOrder(asVec2, in vec);
					}
				}
			}
			if (this._hasLandingStarted && !this._hasLandingCompleted)
			{
				this._hasLandingCompleted = true;
				foreach (Agent agent in Mission.Current.AttackerTeam.ActiveAgents)
				{
					if (agent.IsAIControlled && agent.GetSteppedEntity() != null)
					{
						this._hasLandingCompleted = false;
						break;
					}
				}
				if (this._hasLandingCompleted)
				{
					(base.Mission.DefenderTeam.TeamAI as TeamAINavalRaidDefenderComponent).OnLandingCompleted();
				}
			}
		}

		// Token: 0x06000FBB RID: 4027 RVA: 0x000786BC File Offset: 0x000768BC
		public override void OnFixedMissionTick(float fixedDt)
		{
			foreach (MissionShip missionShip in this._shipsLogic.AllShips)
			{
				if (missionShip.ShipOrder.MovementOrderEnum == ShipOrder.ShipMovementOrderEnum.Stop && missionShip.BeingAbandoned)
				{
					missionShip.SetAnchor(false, false, 1f);
					MatrixFrame bodyWorldTransform = missionShip.GameEntity.GetBodyWorldTransform();
					Vec3 u = bodyWorldTransform.rotation.u;
					Vec3 f = bodyWorldTransform.rotation.f;
					Vec3 vec = u - f * Vec3.DotProduct(u, f);
					vec.Normalize();
					Vec3 vec2 = Vec3.Up - f * Vec3.DotProduct(Vec3.Up, f);
					vec2.Normalize();
					float num = MathF.Atan2(Vec3.DotProduct(f, Vec3.CrossProduct(vec2, vec)), Vec3.DotProduct(vec2, vec));
					float num2 = Vec3.DotProduct(missionShip.Physics.AngularVelocity, f);
					float num3 = 1.8f;
					float num4 = 1f;
					float num5 = 240f / fixedDt / num3;
					float num6 = num5 * num5;
					float num7 = 2f * num4 * num5;
					Vec3 vec3 = f * (-num * num6 - num2 * num7);
					vec3 /= 4200000f;
					missionShip.Physics.ApplyTorque(in vec3, 3);
				}
			}
		}

		// Token: 0x06000FBC RID: 4028 RVA: 0x00078840 File Offset: 0x00076A40
		public override void OnAgentBuild(Agent agent, Banner banner)
		{
			Team team = agent.Team;
			if (agent.IsAIControlled && team.IsAttacker)
			{
				AgentNavalComponent component = agent.GetComponent<AgentNavalComponent>();
				component.SetBlockOffShipConsideration(false);
				component.SetBlockFormationCleanupOnShipAdabandonment(false);
				AgentNavalAIComponent component2 = agent.GetComponent<AgentNavalAIComponent>();
				int index = agent.Formation.Index;
				component2.ActivateSwimToShore(this._jumpingFrames[index]);
			}
		}

		// Token: 0x06000FBD RID: 4029 RVA: 0x0007889A File Offset: 0x00076A9A
		public override void OnAgentControllerSetToPlayer(Agent agent)
		{
			agent.GetComponent<AgentNavalAIComponent>().DeactivateSwimToShore();
		}

		// Token: 0x06000FBE RID: 4030 RVA: 0x000788A8 File Offset: 0x00076AA8
		public override void OnMissionStateFinalized()
		{
			this._shipsLogic.ShipPreparedForAbandonmentEvent -= this.OnShipPreparedForAbandonment;
			this._shipsLogic.ShipSpawnedEvent -= this.OnShipSpawned;
			this._shipsLogic.ShipCollisionEvent -= this.OnShipCollision;
			if (this._warningBellsSoundEvent != null)
			{
				this._warningBellsSoundEvent.Stop();
				this._warningBellsSoundEvent = null;
			}
		}

		// Token: 0x06000FBF RID: 4031 RVA: 0x00078914 File Offset: 0x00076B14
		public override void OnMissionResultReady(MissionResult missionResult)
		{
			foreach (Agent agent in Mission.Current.Agents)
			{
				agent.SetAgentFlags(agent.GetAgentFlags() & -9);
			}
		}

		// Token: 0x04000976 RID: 2422
		public const string PlayerStandingPointEntityTag = "sp_naval_raid_player_spawn";

		// Token: 0x04000977 RID: 2423
		private const int MaxPathNodeCount = 8;

		// Token: 0x04000978 RID: 2424
		private const int MaxAllowedShipCount = 4;

		// Token: 0x04000979 RID: 2425
		public NavalShipsLogic _shipsLogic;

		// Token: 0x0400097A RID: 2426
		public NavalAgentsLogic _agentsLogic;

		// Token: 0x0400097B RID: 2427
		private ShipCollisionOutcomeLogic _shipCollisionOutcomeLogic;

		// Token: 0x0400097C RID: 2428
		public MatrixFrame[][] _landingFrames;

		// Token: 0x0400097D RID: 2429
		private int[] _shipNextPathNodeIndices;

		// Token: 0x0400097E RID: 2430
		public MatrixFrame[] _jumpingFrames;

		// Token: 0x0400097F RID: 2431
		private readonly HashSet<MissionShip> _approachingShoutsPlayed = new HashSet<MissionShip>();

		// Token: 0x04000980 RID: 2432
		private SoundEvent _warningBellsSoundEvent;

		// Token: 0x04000981 RID: 2433
		private bool _hasLandingStarted;

		// Token: 0x04000982 RID: 2434
		private bool _hasLandingCompleted;
	}
}
