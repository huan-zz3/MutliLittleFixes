using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.Objects.UsableMachines;
using NavalDLC.Missions.ShipActuators;
using NavalDLC.Storyline.Objectives.Quest3;
using NavalDLC.Storyline.Objects;
using NavalDLC.Storyline.Quests;
using SandBox;
using SandBox.Missions.AgentBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.MissionLogics;
using TaleWorlds.MountAndBlade.Objects.Usables;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.Storyline.MissionControllers
{
	// Token: 0x02000069 RID: 105
	public class BlockedEstuaryMissionController : MissionLogic
	{
		// Token: 0x1700013C RID: 316
		// (get) Token: 0x06000616 RID: 1558 RVA: 0x0002352F File Offset: 0x0002172F
		public bool CanEndBattleNatively
		{
			get
			{
				return this.CurrentPhase == BlockedEstuaryMissionController.BattlePhase.Phase3;
			}
		}

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x06000617 RID: 1559 RVA: 0x0002353A File Offset: 0x0002173A
		// (set) Token: 0x06000618 RID: 1560 RVA: 0x00023542 File Offset: 0x00021742
		public BlockedEstuaryMissionController.BattlePhase CurrentPhase
		{
			get
			{
				return this._currentPhase;
			}
			private set
			{
				if (value != this._currentPhase)
				{
					this._currentPhase = value;
					Action onPhaseEnd = this.OnPhaseEnd;
					if (onPhaseEnd == null)
					{
						return;
					}
					onPhaseEnd();
				}
			}
		}

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x06000619 RID: 1561 RVA: 0x00023564 File Offset: 0x00021764
		// (set) Token: 0x0600061A RID: 1562 RVA: 0x0002356C File Offset: 0x0002176C
		public MissionShip BurningShip { get; private set; }

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x0600061B RID: 1563 RVA: 0x00023575 File Offset: 0x00021775
		// (set) Token: 0x0600061C RID: 1564 RVA: 0x00023580 File Offset: 0x00021780
		public bool IsShipBurning
		{
			get
			{
				return this._isShipBurning;
			}
			private set
			{
				this._isShipBurning = value;
				if (value && this._burningShipSoundEvent == null && this.BurningShip != null)
				{
					this._burningShipSoundEvent = SoundEvent.CreateEvent(BlockedEstuaryMissionController.BurningSoundEventId, base.Mission.Scene);
					this._burningShipSoundEvent.SetPosition(this.BurningShip.GlobalFrame.origin);
					this._burningShipSoundEvent.SetParameter("FireIntensity", 0.1f);
					this._burningShipSoundEvent.Play();
				}
			}
		}

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x0600061D RID: 1565 RVA: 0x000235FE File Offset: 0x000217FE
		public bool ShipsCollided
		{
			get
			{
				return this._shipsCollided;
			}
		}

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x0600061E RID: 1566 RVA: 0x00023606 File Offset: 0x00021806
		private bool IsEnding
		{
			get
			{
				return this._missionEndTimer != null;
			}
		}

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x0600061F RID: 1567 RVA: 0x00023611 File Offset: 0x00021811
		// (set) Token: 0x06000620 RID: 1568 RVA: 0x00023619 File Offset: 0x00021819
		public bool CollisionImminent { get; private set; }

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x06000621 RID: 1569 RVA: 0x00023622 File Offset: 0x00021822
		// (set) Token: 0x06000622 RID: 1570 RVA: 0x0002362A File Offset: 0x0002182A
		public bool LastExitZoneReached { get; private set; }

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x06000623 RID: 1571 RVA: 0x00023633 File Offset: 0x00021833
		// (set) Token: 0x06000624 RID: 1572 RVA: 0x0002363B File Offset: 0x0002183B
		private MissionShip TargetShip { get; set; }

		// Token: 0x06000625 RID: 1573 RVA: 0x00023644 File Offset: 0x00021844
		public BlockedEstuaryMissionController(MobileParty enemyParty, bool startFromCheckPoint)
		{
			this._enemyParty = enemyParty;
			this._startFromCheckPoint = startFromCheckPoint;
			this._checkPointReached = this._startFromCheckPoint;
			this.CollectShips();
		}

		// Token: 0x06000626 RID: 1574 RVA: 0x000236A4 File Offset: 0x000218A4
		private void CollectShips()
		{
			new MBList<IShipOrigin>();
			Ship ship = MobileParty.MainParty.Ships.FirstOrDefault<Ship>((Ship x) => x.ShipHull.StringId == "ship_trade_cog_q3") ?? MobileParty.MainParty.Ships.First<Ship>();
			Ship enemyBurningShip = this._enemyParty.Ships.FirstOrDefault<Ship>((Ship x) => x.ShipHull.StringId == "burning_cog_ship");
			this._enemyShipOrigins = Extensions.ToMBList<IShipOrigin>(this._enemyParty.Ships.Where<Ship>((Ship x) => x != enemyBurningShip).Cast<IShipOrigin>());
			this._playerBurningShipOrigin = MobileParty.MainParty.Ships.FirstOrDefault<Ship>((Ship x) => x.ShipHull.StringId == "burning_fishing_ship");
			this._enemyBurningShipOrigin = enemyBurningShip;
			this._playerShipOrigin = ship;
		}

		// Token: 0x06000627 RID: 1575 RVA: 0x000237A8 File Offset: 0x000219A8
		public override void OnMissionTick(float dt)
		{
			if (!this._initialized)
			{
				this.Initialize();
			}
			if (this._missionEndTimer != null && this._missionEndTimer.Check(false))
			{
				this.OnFinalize();
			}
			if ((Agent.Main == null || !Agent.Main.IsActive()) && !this.IsEnding)
			{
				this.OnFail(new TextObject("{=ay5y18aq}You pass out from the pain of your wounds.", null));
			}
			switch (this.CurrentPhase)
			{
			case BlockedEstuaryMissionController.BattlePhase.Phase1:
				this.TickMissionPhase1(dt);
				break;
			case BlockedEstuaryMissionController.BattlePhase.Phase2:
				this.TickMissionPhase2(dt);
				break;
			case BlockedEstuaryMissionController.BattlePhase.Phase3:
				this.TickMissionPhase3(dt);
				break;
			}
			this.TickParticlesAndBurningSystems(dt);
			this.TickGunnar(dt);
		}

		// Token: 0x06000628 RID: 1576 RVA: 0x00023850 File Offset: 0x00021A50
		private void TickMissionPhase1(float dt)
		{
			MatrixFrame globalFrame = this.BurningShip.GlobalFrame;
			if (this._collisionTimer != null && this._collisionTimer.Check(false) && !this.IsEnding)
			{
				this.OnFail(new TextObject("{=CAyVaV0Y}Your fireship missed its target! The enemy flagship is unscathed.", null));
			}
			else if (this.IsShipBurning && !this.IsEnding)
			{
				if (this._missionPhaseEndTimer != null && this._missionPhaseEndTimer.Check(false))
				{
					this.ProceedToPhase2();
					this._missionPhaseEndTimer = null;
				}
				else if (Agent.Main.IsInWater())
				{
					if (this._shipsCollided && this._missionPhaseEndTimer == null)
					{
						this.DestroyCollidingShips();
						this._missionPhaseEndTimer = new MissionTimer(6f);
						MBMusicManager.Current.ChangeCurrentThemeIntensity(1f);
					}
				}
				else if (this._jumpingZone.IsPointIn(globalFrame.origin))
				{
					this.OnFail(new TextObject("{=Uj6t6FES}You missed the oppurtunity to jump off the ship.", null));
				}
				if (this.BurningShip.IsDisabled && this._collisionTimer == null && !this._shipsCollided)
				{
					this.OnFail(new TextObject("{=S0L5Zi8a}Your ship is engulfed by flames.", null));
				}
				if (this._jumpingZone.IsPointIn(globalFrame.origin) && this._collisionTimer == null)
				{
					this._collisionTimer = new MissionTimer(15f);
					this.CollisionImminent = true;
				}
				if (this.CollisionImminent && !this._enemiesPanicked && BlockedEstuaryMissionController.WillHitBoundingBox(this.BurningShip.GameEntity.GlobalPosition, this.BurningShip.Physics.LinearVelocity.AsVec2 * 3f, this.TargetShip.GameEntity.GlobalPosition + this.TargetShip.GameEntity.GetBoundingBoxMin(), this.TargetShip.GameEntity.GlobalPosition + this.TargetShip.GameEntity.GetBoundingBoxMax()))
				{
					this.MakeEnemiesPanic(this.TargetShip);
				}
			}
			if ((this._fire3Zone.IsPointIn(globalFrame.origin) || this._shipBurnProgress >= 0.6f) && !this._shouldGunnarEscape)
			{
				this.ShowGunnarEscapeNotification();
				this._shouldGunnarEscape = true;
				if (this._gunnarAgent != null)
				{
					this.SetEscapePosition();
				}
			}
			if (!this.LastExitZoneReached && !this._showedLastWarning && !this.BurningShip.IsDisabled && !this.BurningShip.IsSinking && this.BurningShip.GameEntity.GlobalPosition.Distance(this.TargetShip.GameEntity.GlobalPosition) < 120f && !this.IsEnding && !Agent.Main.IsInWater())
			{
				BlockedEstuaryMissionController.ShowNotification(new TextObject("{=yYkI9ezi}Jump now! You want your breeks to catch fire?", null), true, 3);
				this._showedLastWarning = true;
			}
			if (this._jumpingZone.IsPointIn(globalFrame.origin) && !this.LastExitZoneReached)
			{
				this.LastExitZoneReached = true;
				Action onLastExitZoneReachedEvent = this.OnLastExitZoneReachedEvent;
				if (onLastExitZoneReachedEvent != null)
				{
					onLastExitZoneReachedEvent();
				}
				if (!this.IsShipBurning)
				{
					this.ActivateAllBurningSystems(0.5f);
				}
			}
			if (!this.CollisionImminent)
			{
				this.TickShipHealth(dt);
			}
			if (this._initialTriggerZone.IsPointIn(globalFrame.origin) && !this._initializeGunnarBurningShip)
			{
				this._initializeGunnarBurningShip = true;
			}
		}

		// Token: 0x06000629 RID: 1577 RVA: 0x00023B9C File Offset: 0x00021D9C
		private void TickShipHealth(float dt)
		{
			if (!this._shipsCollided && this.IsShipBurning && this.BurningShip.HitPoints > 0f && !this.LastExitZoneReached)
			{
				float num = 0f;
				foreach (KeyValuePair<BurnShipObject, ValueTuple<BurningSystem, float>> keyValuePair in this._playerShipBurningSystems)
				{
					if (keyValuePair.Value.Item1 != null)
					{
						num += keyValuePair.Value.Item1.GetFlameProgress();
					}
				}
				num = MathF.Clamp(num / (float)Math.Max(1, this._playerShipBurningSystems.Count<KeyValuePair<BurnShipObject, ValueTuple<BurningSystem, float>>>((KeyValuePair<BurnShipObject, ValueTuple<BurningSystem, float>> x) => x.Key.IsDeactivated)), 0f, 1f);
				this._shipDamageCheckTimer += dt;
				while (this._shipDamageCheckTimer > 0.1f)
				{
					this._shipDamageCheckTimer -= 0.1f;
					float num2 = (num - this._shipBurnProgress) * this.BurningShip.MaxHealth;
					this._shipBurnProgress = num;
					int num3;
					int num4;
					DamageTypes damageTypes;
					bool flag;
					this.BurningShip.DealDamage(num2, null, out num3, out num4, out damageTypes, out flag);
					float num5 = (num - (1f - this.BurningShip.FireHitPoints / this.BurningShip.MaxFireHealth)) * this.BurningShip.MaxFireHealth;
					if (num5 > 0f)
					{
						this.BurningShip.DealFireDamage(num5);
					}
				}
			}
		}

		// Token: 0x0600062A RID: 1578 RVA: 0x00023D34 File Offset: 0x00021F34
		private void EnableRamp(MissionShip targetShip)
		{
			targetShip.GameEntity.GetFirstChildEntityWithTagRecursive("ramp_holder").SetVisibilityExcludeParents(true);
		}

		// Token: 0x0600062B RID: 1579 RVA: 0x00023D60 File Offset: 0x00021F60
		private void MakeEnemiesPanic(MissionShip targetShip)
		{
			this.EnableRamp(targetShip);
			this._burntShipAgents = this._navalAgentsLogic.GetActiveAgentsOfShip(targetShip).ToList<Agent>();
			this._navalAgentsLogic.RemoveAllReservedTroopsFromShip(targetShip);
			targetShip.Formation.SetControlledByAI(true, false);
			targetShip.ShipOrder.FormationLeaveShip();
			for (int i = this._burntShipAgents.Count - 1; i >= 0; i--)
			{
				Agent agent = this._burntShipAgents[i];
				Vec3 globalPosition = targetShip.GameEntity.GlobalPosition;
				Vec2 vec;
				vec..ctor(MBRandom.RandomFloatRanged(60f, 110f), MBRandom.RandomFloatRanged(70f, 120f));
				Vec2 vec2 = globalPosition.AsVec2 + ((MBRandom.RandomFloat < 0.5f) ? vec : (-vec));
				Agent agent2 = agent;
				Vec3 vec3 = vec2.ToVec3(0f) - agent.Position;
				vec3 = vec3.NormalizedCopy();
				agent2.SetTargetPositionAndDirection(ref vec2, ref vec3);
			}
			this._enemiesPanicked = true;
		}

		// Token: 0x0600062C RID: 1580 RVA: 0x00023E68 File Offset: 0x00022068
		private void TickParticlesAndBurningSystems(float dt)
		{
			float num = 0f;
			if (this.IsShipBurning)
			{
				foreach (KeyValuePair<BurnShipObject, ValueTuple<BurningSystem, float>> keyValuePair in this._playerShipBurningSystems)
				{
					if (keyValuePair.Value.Item1 != null)
					{
						keyValuePair.Value.Item1.Tick(dt);
						num += keyValuePair.Value.Item1.GetFlameProgress();
						keyValuePair.Value.Item1.CheckWater();
					}
				}
			}
			if (!this.BurningShip.IsDisabled && (!this.LastExitZoneReached || this._shipsCollided))
			{
				bool flag = true;
				foreach (KeyValuePair<BurnShipObject, ValueTuple<BurningSystem, float>> keyValuePair2 in this._playerShipBurningSystems)
				{
					if (keyValuePair2.Value.Item1 != null && !keyValuePair2.Value.Item1.FlamesReachedEnd())
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					this.DisableShip(this.BurningShip, true);
				}
			}
			if (this._shipsCollided)
			{
				this._enemyShipBurningSystem.Tick(dt);
				this._enemyShipBurningSystem.CheckWater();
			}
			if (this._enemyShipBurningSystem.FlamesReachedEnd() && !this.TargetShip.IsDisabled)
			{
				this.DisableShip(this.TargetShip, true);
			}
			bool flag2 = false;
			for (int i = this._projectileParticles.Count - 1; i >= 0; i--)
			{
				BlockedEstuaryMissionController.BurningProjectile burningProjectile = this._projectileParticles[i];
				bool flag3;
				burningProjectile.Tick(dt, out flag3);
				if (flag3)
				{
					burningProjectile.Clear();
					this._projectileParticles.RemoveAt(i);
				}
				else if (!flag2 && this.CurrentPhase == BlockedEstuaryMissionController.BattlePhase.Phase1 && this.DoesShipCollideWithProjectile(this.BurningShip, burningProjectile))
				{
					flag2 = true;
					if (!this._firstCollisionFirePatch)
					{
						this._firstCollisionFirePatch = true;
						BlockedEstuaryMissionController.ShowNotification(new TextObject("{=xrdbaPop}Watch out! Let's not go up in flames until we reach them!", null), true, 2);
					}
				}
				else if (!this._firePatchSpawned && burningProjectile.GameEntity != null && !this.IsShipBurning && this.CurrentPhase == BlockedEstuaryMissionController.BattlePhase.Phase1)
				{
					this._firePatchSpawned = true;
					BlockedEstuaryMissionController.ShowNotification(new TextObject("{=dmyrUCZ3}Steer clear of those flames, eh?", null), true, 2);
				}
			}
			bool flag4 = this.BurningShip.FireHitPoints <= 0f;
			if (this.CurrentPhase == BlockedEstuaryMissionController.BattlePhase.Phase1)
			{
				if (this.IsShipBurning)
				{
					if (this.LastExitZoneReached)
					{
						goto IL_02FD;
					}
					using (Dictionary<BurnShipObject, ValueTuple<BurningSystem, float>>.Enumerator enumerator = this._playerShipBurningSystems.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<BurnShipObject, ValueTuple<BurningSystem, float>> keyValuePair3 = enumerator.Current;
							if (keyValuePair3.Value.Item1 != null)
							{
								float num2 = (flag2 ? (keyValuePair3.Value.Item2 * 20f) : keyValuePair3.Value.Item2);
								keyValuePair3.Value.Item1.SetSpreadRate(num2);
							}
						}
						goto IL_02FD;
					}
				}
				if (flag4)
				{
					this.ActivateAllBurningSystems(0.5f);
				}
				else if (flag2)
				{
					this.BurningShip.DealFireDamage(600f * dt);
				}
			}
			IL_02FD:
			if (this.IsShipBurning)
			{
				this._burningShipSoundEvent.SetParameter("FireIntensity", num * 20f);
				this._burningShipSoundEvent.SetPosition(this.BurningShip.GlobalFrame.origin);
			}
		}

		// Token: 0x0600062D RID: 1581 RVA: 0x000241D4 File Offset: 0x000223D4
		private void BurnSails(MissionShip ship)
		{
			foreach (MissionSail missionSail in ship.Sails)
			{
				if (!missionSail.IsBurning())
				{
					missionSail.StartFire();
				}
			}
		}

		// Token: 0x0600062E RID: 1582 RVA: 0x00024230 File Offset: 0x00022430
		private void ToggleShipBallistas(MissionShip ship, bool enabled)
		{
			if (ship.ShipSiegeWeapon != null)
			{
				foreach (StandingPoint standingPoint in ship.ShipSiegeWeapon.StandingPoints)
				{
					standingPoint.IsDeactivated = !enabled;
				}
			}
		}

		// Token: 0x0600062F RID: 1583 RVA: 0x00024294 File Offset: 0x00022494
		private void DisableShip(MissionShip ship, bool burnSails = true)
		{
			if (!ship.IsDisabled)
			{
				foreach (ShipAttachmentMachine shipAttachmentMachine in ship.AttachmentMachines)
				{
					shipAttachmentMachine.SetDisabled(false);
				}
				ship.ShipControllerMachine.SetDisabled(false);
				foreach (ClimbingMachine climbingMachine in ship.ClimbingMachines)
				{
					climbingMachine.SetDisabled(false);
				}
				foreach (ShipOarMachine shipOarMachine in ship.LeftSideShipOarMachines)
				{
					shipOarMachine.SetDisabled(false);
				}
				foreach (ShipOarMachine shipOarMachine2 in ship.RightSideShipOarMachines)
				{
					shipOarMachine2.SetDisabled(false);
				}
				this.ToggleShipBallistas(ship, false);
				if (ship.ShipControllerMachine.PilotAgent != null)
				{
					ship.ShipControllerMachine.PilotAgent.StopUsingGameObject(true, 1);
				}
				ship.ShipControllerMachine.SetDisabled(false);
				ship.SetDisabled(false);
				this.DisableTargetShipObject(ship);
				ship.SetAnchor(true, true, 1f);
				if (burnSails)
				{
					this.BurnSails(ship);
				}
			}
		}

		// Token: 0x06000630 RID: 1584 RVA: 0x00024414 File Offset: 0x00022614
		private void SetWindStrengthAndDirection(Vec2 direction, float strength)
		{
			Scene scene = Mission.Current.Scene;
			Vec2 vec = strength * direction;
			scene.SetGlobalWindVelocity(ref vec);
		}

		// Token: 0x06000631 RID: 1585 RVA: 0x0002443C File Offset: 0x0002263C
		private void ProceedToPhase2()
		{
			this.IsShipBurning = true;
			this._shipsCollided = true;
			this.SpawnPlayerTradeShip();
			this.FadeoutEnemyAgents();
			this.SpawnEnemyAgentsOnRoad();
			this.DisableShip(this.BurningShip, true);
			this.DisableShip(this.TargetShip, true);
			MBMusicManager.Current.ChangeCurrentThemeIntensity(-0.4f);
			this.CollisionImminent = false;
			this._playerHorse = this.SpawnPlayerHorse();
			Vec3 randomPositionAroundPoint = base.Mission.GetRandomPositionAroundPoint(this._playerHorse.Position, 2f, 4f, false);
			this._horse = this.SpawnHorse(randomPositionAroundPoint, (randomPositionAroundPoint - Agent.Main.Position).AsVec2);
			this.TeleportMainAgent("sp_player_mount");
			this.PrepareGunnarForSecondPhase();
			if (this.IsGunnarActive())
			{
				BlockedEstuaryMissionController.ShowNotification(new TextObject("{=NB2HCGUq}Head for shore! There are a pair of horses waiting for us. We must ride quickly back to the Sturgians before the Sea Hounds can reorganize the blockade.", null), true, 2);
			}
			else
			{
				BlockedEstuaryMissionController.ShowNotification(new TextObject("{=mlMbHCaG}Head for shore! There are a pair of horses waiting for you. Ride quickly back to the Sturgians before the Sea Hounds can reorganize the blockade.", null), true, 2);
			}
			this.CurrentPhase = BlockedEstuaryMissionController.BattlePhase.Phase2;
			this._missionObjectiveLogic.StartObjective(new SwimToShoreObjective(base.Mission, this._gunnarAgent));
		}

		// Token: 0x06000632 RID: 1586 RVA: 0x00024550 File Offset: 0x00022750
		public List<Agent> GetAgentsOfInterest()
		{
			List<Agent> list = new List<Agent>();
			if (this.CurrentPhase == BlockedEstuaryMissionController.BattlePhase.Phase2)
			{
				if (this._horse != null && this._horse.IsActive())
				{
					list.Add(this._horse);
				}
				if (this._playerHorse != null && this._playerHorse.IsActive())
				{
					list.Add(this._playerHorse);
				}
			}
			if (this.IsGunnarActive())
			{
				list.Add(this._gunnarAgent);
			}
			return list;
		}

		// Token: 0x06000633 RID: 1587 RVA: 0x000245C4 File Offset: 0x000227C4
		private void PrepareGunnarForSecondPhase()
		{
			Vec3 randomPositionAroundPoint = base.Mission.GetRandomPositionAroundPoint(Agent.Main.Position, 1f, 3f, false);
			if (this._gunnarAgent == null || !this._gunnarAgent.IsActive())
			{
				this.SpawnGunnar(randomPositionAroundPoint, true);
			}
			else if (Agent.Main.Position.Distance(this._gunnarAgent.Position) > 5f)
			{
				this._gunnarAgent.TeleportToPosition(randomPositionAroundPoint);
			}
			this._gunnarAgent.SetTeam(Team.Invalid, true);
			foreach (Agent agent in base.Mission.PlayerEnemyTeam.ActiveAgents)
			{
				agent.ResetEnemyCaches();
			}
			this._gunnarAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.ClearTarget();
			this._gunnarAgent.SetAgentFlags(this._gunnarAgent.GetAgentFlags() | 8192);
			this._gunnarAgent.SetRidingOrder(1);
			this._gunnarAgent.SetAlarmState(3);
			WorldPosition worldPosition;
			worldPosition..ctor(base.Mission.Scene, this._horse.Position);
			this._gunnarAgent.SetScriptedPositionAndDirection(ref worldPosition, (this._horse.Position - this._gunnarAgent.Position).AsVec2.RotationInRadians, true, 8);
		}

		// Token: 0x06000634 RID: 1588 RVA: 0x00024744 File Offset: 0x00022944
		private void SpawnEnemyAgentsOnRoad()
		{
			bool isNight = Campaign.Current.IsNight;
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("enemy_group_parent");
			if (gameEntity != null)
			{
				for (int i = 0; i < gameEntity.ChildCount; i++)
				{
					GameEntity child = gameEntity.GetChild(i);
					this._enemyAgentSpawnPoints.Add(new BlockedEstuaryMissionController.EnemySpawnPoint(child, MBObjectManager.Instance.GetObject<CharacterObject>(Extensions.GetRandomElement<string>(BlockedEstuaryMissionController._enemyAgentCharacterIds)), isNight));
				}
			}
		}

		// Token: 0x06000635 RID: 1589 RVA: 0x000247BC File Offset: 0x000229BC
		private void FadeoutEnemyAgents()
		{
			if (this._burntShipAgents != null)
			{
				foreach (Agent agent in this._burntShipAgents)
				{
					if (agent.IsActive())
					{
						agent.FadeOut(true, true);
					}
				}
			}
			this._burntShipAgents = null;
		}

		// Token: 0x06000636 RID: 1590 RVA: 0x00024828 File Offset: 0x00022A28
		private void TeleportMainAgent(string spawnPointId)
		{
			MatrixFrame globalFrame = base.Mission.Scene.FindEntityWithTag(spawnPointId).GetGlobalFrame();
			Agent.Main.TeleportToPosition(globalFrame.origin);
			Agent.Main.LookDirection = globalFrame.rotation.f.NormalizedCopy();
		}

		// Token: 0x06000637 RID: 1591 RVA: 0x00024877 File Offset: 0x00022A77
		private static void ShowNotification(TextObject text, bool isAnnouncedByGunnar, MBInformationManager.NotificationPriority priority = 2)
		{
			if (!isAnnouncedByGunnar)
			{
				MBInformationManager.AddQuickInformation(text, 0, null, null, "");
				return;
			}
			CampaignInformationManager.AddDialogLine(text, NavalStorylineData.Gunnar.CharacterObject, null, 0, priority);
		}

		// Token: 0x06000638 RID: 1592 RVA: 0x000248A0 File Offset: 0x00022AA0
		private void DestroyCollidingShips()
		{
			this.TargetShip.SetShipOrderActive(false);
			this.TargetShip.Formation.SetControlledByAI(false, false);
			this.TargetShip.SetAnchor(false, false, 1f);
			this.BurningShip.SetShipOrderActive(false);
			this.BurningShip.ShipOrder.SetFormation(null);
			this.TargetShip.ShipOrder.SetFormation(null);
			for (int i = this._burntShipAgents.Count - 1; i >= 0; i--)
			{
				Agent agent = this._burntShipAgents[i];
				MissionShip missionShip;
				if (!agent.IsInWater() && this._navalAgentsLogic.IsAgentOnAnyShip(agent, out missionShip, 2) && missionShip == this.TargetShip)
				{
					Blow blow = default(Blow);
					blow.InflictedDamage = 1000;
					blow.DamagedPercentage = 1f;
					Blow blow2 = blow;
					agent.Die(blow2, -1);
				}
			}
			this.BurnSails(this.TargetShip);
			this.BurnSails(this.BurningShip);
		}

		// Token: 0x06000639 RID: 1593 RVA: 0x00024995 File Offset: 0x00022B95
		public void OnBurningMachineUsed(BurnShipObject burnShipObject)
		{
			this.ActivateBurningSystem(burnShipObject, 0.5f);
		}

		// Token: 0x0600063A RID: 1594 RVA: 0x000249A4 File Offset: 0x00022BA4
		private void MakeGunnarEscapeShip()
		{
			if (this._gunnarAgent.IsAIControlled && AgentComponentExtensions.AIMoveToGameObjectIsEnabled(this._gunnarAgent))
			{
				AgentComponentExtensions.AIMoveToGameObjectDisable(this._gunnarAgent);
			}
			if (this._gunnarAgent.IsUsingGameObject)
			{
				this._gunnarAgent.StopUsingGameObject(true, 1);
			}
			this.EnableRamp(this.BurningShip);
			Vec2 asVec = this._escapePosition.AsVec2;
			Vec3 escapePosition = this.GetEscapePosition(this.BurningShip);
			if (escapePosition.Distance(asVec.ToVec3(escapePosition.z)) > 10f)
			{
				this.SetEscapePosition(escapePosition);
			}
		}

		// Token: 0x0600063B RID: 1595 RVA: 0x00024A37 File Offset: 0x00022C37
		private void ShowGunnarEscapeNotification()
		{
			BlockedEstuaryMissionController.ShowNotification(new TextObject("{=yXOnEQJ6}Our ship is ablaze! Get ready to jump!", null), this.IsGunnarActive(), 2);
		}

		// Token: 0x0600063C RID: 1596 RVA: 0x00024A50 File Offset: 0x00022C50
		private void SetEscapePosition()
		{
			this.SetEscapePosition(this.GetEscapePosition(this.BurningShip));
		}

		// Token: 0x0600063D RID: 1597 RVA: 0x00024A64 File Offset: 0x00022C64
		private void SetEscapePosition(Vec3 position)
		{
			this._escapePosition = position;
			Vec2 asVec = this._escapePosition.AsVec2;
			Agent gunnarAgent = this._gunnarAgent;
			Vec3 vec = position - this._gunnarAgent.Position;
			vec = vec.NormalizedCopy();
			gunnarAgent.SetTargetPositionAndDirection(ref asVec, ref vec);
		}

		// Token: 0x0600063E RID: 1598 RVA: 0x00024AB0 File Offset: 0x00022CB0
		private Vec3 GetEscapePosition(MissionShip ship)
		{
			return ship.GameEntity.GetGlobalFrame().rotation.f * 10f - ship.GameEntity.GetGlobalFrame().rotation.s * 15f + ship.GameEntity.GlobalPosition;
		}

		// Token: 0x0600063F RID: 1599 RVA: 0x00024B1C File Offset: 0x00022D1C
		public void ActivateAllBurningSystems(float spreadRate)
		{
			for (int i = 0; i < this._burningMachines.Count; i++)
			{
				this.ActivateBurningSystem(this._burningMachines[i], spreadRate);
			}
		}

		// Token: 0x06000640 RID: 1600 RVA: 0x00024B54 File Offset: 0x00022D54
		public void ActivateBurningSystem(BurnShipObject burnShipObject, float spreadRate)
		{
			if (burnShipObject != null)
			{
				ValueTuple<BurningSystem, float> valueTuple = this._playerShipBurningSystems[burnShipObject];
				this._playerShipBurningSystems[burnShipObject] = new ValueTuple<BurningSystem, float>(valueTuple.Item1, spreadRate);
			}
			this.IsShipBurning = true;
		}

		// Token: 0x06000641 RID: 1601 RVA: 0x00024B90 File Offset: 0x00022D90
		private void TickMissionPhase2(float dt)
		{
			if (!this.IsEnding)
			{
				if (!this._startFromCheckPoint)
				{
					if (this._checkPointZone.IsPointIn(Agent.Main.Position) && !this._checkPointReached)
					{
						if (!this._enemyAgentSpawnPoints.Any<BlockedEstuaryMissionController.EnemySpawnPoint>((BlockedEstuaryMissionController.EnemySpawnPoint x) => x.Agent.IsActive() && x.Agent.Position.Distance(Agent.Main.Position) < 50f))
						{
							this.OnCheckPointReached();
						}
					}
					if (this._checkPointReached)
					{
						if (Agent.Main.HasMount)
						{
							this.TickHorse(Agent.Main);
						}
					}
					else
					{
						if (Agent.Main.HasMount)
						{
							float stat = Agent.Main.MountAgent.AgentDrivenProperties.GetStat(93);
							float num = ((this.IsGunnarActive() && this._gunnarAgent.HasMount) ? this._gunnarAgent.MountAgent.AgentDrivenProperties.GetStat(93) : stat);
							if (!MBMath.ApproximatelyEqualsTo(stat, num, 1E-05f) && stat < num)
							{
								Agent.Main.MountAgent.AgentDrivenProperties.SetStat(93, num);
								Agent.Main.MountAgent.UpdateCustomDrivenProperties();
							}
						}
						bool flag = false;
						float stat2 = Agent.Main.AgentDrivenProperties.GetStat(66);
						float num2 = MathF.Max(stat2, 1f);
						if (!MBMath.ApproximatelyEqualsTo(stat2, num2, 1E-05f))
						{
							flag = true;
							Agent.Main.AgentDrivenProperties.SetStat(66, num2);
						}
						float stat3 = Agent.Main.AgentDrivenProperties.GetStat(67);
						float num3 = MathF.Max(stat3, 1f);
						if (!MBMath.ApproximatelyEqualsTo(stat3, num3, 1E-05f))
						{
							flag = true;
							Agent.Main.AgentDrivenProperties.SetStat(67, num3);
						}
						float stat4 = Agent.Main.AgentDrivenProperties.GetStat(97);
						float num4 = MathF.Max(stat4, 1f);
						if (!MBMath.ApproximatelyEqualsTo(stat4, num4, 1E-05f))
						{
							flag = true;
							Agent.Main.AgentDrivenProperties.SetStat(97, num4);
						}
						if (flag)
						{
							Agent.Main.UpdateCustomDrivenProperties();
						}
					}
				}
				if (!this._checkPointReached)
				{
					this.CheckEnemyGroups(dt);
				}
				else if (this._playerShip == this._mainAgentNavalComponent.SteppedShip && this._missionPhaseEndTimer == null)
				{
					this._missionPhaseEndTimer = new MissionTimer(1f);
				}
				else if (this._gunnarAgent == null && !Agent.Main.HasMount)
				{
					this.SpawnGunnarOnShip(this._playerShip);
				}
				if (this._missionPhaseEndTimer != null && this._missionPhaseEndTimer.Check(false) && this._talkedToGunnar)
				{
					this.ProceedToPhase3();
					this._missionPhaseEndTimer = null;
				}
			}
		}

		// Token: 0x06000642 RID: 1602 RVA: 0x00024E14 File Offset: 0x00023014
		private void TickGunnar(float dt)
		{
			if (this.IsEnding)
			{
				return;
			}
			bool flag = this.IsGunnarActive();
			if (this.CurrentPhase == BlockedEstuaryMissionController.BattlePhase.Phase1)
			{
				if (flag && !this._gunnarAgent.IsUsingGameObject && this._initializeGunnarBurningShip && !this.LastExitZoneReached && !this._shouldGunnarEscape)
				{
					BurnShipObject burnShipObject = this._burningMachines.FirstOrDefault<BurnShipObject>((BurnShipObject x) => !x.IsDeactivated && !x.HasUser);
					if (burnShipObject != null && !burnShipObject.PilotStandingPoint.HasAIMovingTo)
					{
						this._gunnarAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.SetTarget(burnShipObject, false, 0);
						return;
					}
				}
				else if (flag && this._shouldGunnarEscape && !this._gunnarAgent.IsInWater())
				{
					this.MakeGunnarEscapeShip();
					return;
				}
			}
			else if (this.CurrentPhase == BlockedEstuaryMissionController.BattlePhase.Phase2)
			{
				if (!this._checkPointReached && !this._talkedToGunnar)
				{
					if (this._missionPhaseEndTimer != null && this._missionPhaseEndTimer.Check(false))
					{
						if (flag && this._gunnarAgent.HasMount)
						{
							if (Agent.Main.Position.Distance(this._gunnarAgent.Position) <= 30f)
							{
								this.StartConversation();
								this._missionPhaseEndTimer = null;
								this._talkedToGunnar = true;
								return;
							}
							this.ProceedToRideWithoutTalkingToGunnar();
							return;
						}
					}
					else if (!flag && Agent.Main.HasMount)
					{
						this.ProceedToRideWithoutTalkingToGunnar();
						return;
					}
				}
				else if (!this._checkPointReached && this._talkedToGunnar && this._enemyAreaReached && flag && this._gunnarAgent.HasMount && this._missionPhaseEndTimer == null && this._gunnarHorsePhaseCheckTimer != null && this._gunnarHorsePhaseCheckTimer.Check(false))
				{
					this._gunnarHorsePhaseCheckTimer.Reset();
					Vec3 escapePosition = this._escapePosition;
					float pathDistanceToPoint = this._gunnarAgent.GetPathDistanceToPoint(ref escapePosition);
					float pathDistanceToPoint2 = Agent.Main.GetPathDistanceToPoint(ref escapePosition);
					if (pathDistanceToPoint2 < 150f && pathDistanceToPoint < 150f)
					{
						this._gunnarHorsePhaseCheckTimer = null;
						if (!this._enemyAgentSpawnPoints.Any<BlockedEstuaryMissionController.EnemySpawnPoint>((BlockedEstuaryMissionController.EnemySpawnPoint x) => x.Agent.IsActive() && x.Agent.Position.Distance(Agent.Main.Position) < 50f))
						{
							BlockedEstuaryMissionController.ShowNotification(new TextObject("{=NHS4NQdS}I think that's the last of them.", null), true, 2);
							return;
						}
					}
					else if (!this._playerLeftBehind && pathDistanceToPoint2 > pathDistanceToPoint + 40f)
					{
						this._playerLeftBehind = true;
						BlockedEstuaryMissionController.ShowNotification(new TextObject("{=AHShYsjD}Don't tarry! Keep up with me!", null), true, 2);
					}
				}
			}
		}

		// Token: 0x06000643 RID: 1603 RVA: 0x000250A4 File Offset: 0x000232A4
		private void ProceedToRideWithoutTalkingToGunnar()
		{
			if (this.IsGunnarActive())
			{
				this.OnTalkedToGunnarPhase2();
			}
			else
			{
				string text = "event:/alerts/horns/attack";
				Vec3 position = this._enemyAgentSpawnPoints[0].Position;
				SoundManager.StartOneShotEvent(text, ref position);
			}
			this._missionPhaseEndTimer = null;
			this._talkedToGunnar = true;
		}

		// Token: 0x06000644 RID: 1604 RVA: 0x000250EE File Offset: 0x000232EE
		private void StartConversation()
		{
			Campaign.Current.ConversationManager.SetupAndStartMissionConversation(this._gunnarAgent, Agent.Main, false);
			base.Mission.SetMissionMode(1, true);
		}

		// Token: 0x06000645 RID: 1605 RVA: 0x00025118 File Offset: 0x00023318
		private void SpawnGunnarOnShip(MissionShip ship)
		{
			this._navalAgentsLogic.AddReservedTroopToShip(new SimpleAgentOrigin(NavalStorylineData.Gunnar.CharacterObject, -1, null, default(UniqueTroopDescriptor)), ship);
			this._navalAgentsLogic.SpawnNextBatch(0, false, null);
			this._gunnarAgent = base.Mission.Agents.First<Agent>((Agent x) => x.Character == NavalStorylineData.Gunnar.CharacterObject);
			this._gunnarAgent.GetComponent<CampaignAgentComponent>().CreateAgentNavigator();
		}

		// Token: 0x06000646 RID: 1606 RVA: 0x000251A4 File Offset: 0x000233A4
		private void CheckEnemyGroups(float dt)
		{
			foreach (BlockedEstuaryMissionController.EnemySpawnPoint enemySpawnPoint in this._enemyAgentSpawnPoints)
			{
				enemySpawnPoint.Tick(dt, this);
				if (!this._enemyAreaReached && enemySpawnPoint.IsAlerted)
				{
					if (Agent.Main != null && Agent.Main.IsActive() && Agent.Main.HasMount)
					{
						BlockedEstuaryMissionController.ShowNotification(new TextObject("{=5McrRAZb}There they are! Ride fast! Ride through them!", null), true, 2);
					}
					this._enemyAreaReached = true;
				}
			}
		}

		// Token: 0x06000647 RID: 1607 RVA: 0x00025240 File Offset: 0x00023440
		private void ProceedToPhase3()
		{
			if (!this._checkPointReached)
			{
				this.OnCheckPointReached();
			}
			this.CurrentPhase = BlockedEstuaryMissionController.BattlePhase.Phase3;
			this._playerShip.SetAnchor(false, false, 1f);
			this._playerShip.Formation.SetControlledByAI(true, false);
			this._playerShip.ShipOrder.FormationJoinShip(this._playerShip.Formation);
			this._playerShip.SetShipOrderActive(true);
			if (Agent.Main != null)
			{
				MissionShip missionShip;
				if (this._navalAgentsLogic.IsAgentOnAnyShip(Agent.Main, out missionShip, -1))
				{
					this._navalAgentsLogic.TransferAgentToShip(Agent.Main, this._playerShip);
				}
				else
				{
					this._navalAgentsLogic.AddAgentToShip(Agent.Main, this._playerShip);
				}
				if (!this._startFromCheckPoint)
				{
					Agent.Main.UseGameObject(this._playerShip.ShipControllerMachine.PilotStandingPoint, -1);
					this._playerShip.ShipControllerMachine.OnPilotAssignedDuringSpawn();
				}
			}
			this._missionObjectiveLogic.StartObjective(new ReachEscapeZoneObjective(base.Mission, this._playerShip, this._escapeZone.GameEntity.GlobalPosition + new Vec3(0f, 0f, 5f, -1f)));
			this._gunnarHorsePhaseCheckTimer = null;
			BlockedEstuaryMissionController.ShowNotification(new TextObject("{=UUexHDKH}Well done! Now, let's run their blockade and reach the open sea.", null), true, 2);
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x00025394 File Offset: 0x00023594
		private void ActivateEnemyShips()
		{
			foreach (BlockedEstuaryMissionController.EnemyShipTrigger enemyShipTrigger in this._triggers)
			{
				enemyShipTrigger.SendToDestination();
			}
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x000253E4 File Offset: 0x000235E4
		private void InitializeShipTriggers()
		{
			for (int i = 0; i < 10; i++)
			{
				int num = i + 2;
				GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("sp_enemy_ship_" + num);
				GameEntity gameEntity2 = Mission.Current.Scene.FindEntityWithTag("sp_enemy_trigger_" + num);
				VolumeBox volumeBox = ((gameEntity2 != null) ? gameEntity2.GetFirstScriptOfType<VolumeBox>() : null);
				if (gameEntity == null)
				{
					break;
				}
				if (volumeBox == null)
				{
					Debug.FailedAssert("There is no volume box for spawn point: sp_enemy_trigger_" + num, "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Storyline\\MissionControllers\\BlockedEstuaryMissionController.cs", "InitializeShipTriggers", 1414);
					return;
				}
				if (num - 1 > this._enemyShipOrigins.Count)
				{
					Debug.FailedAssert("There are not enough ships in party", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Storyline\\MissionControllers\\BlockedEstuaryMissionController.cs", "InitializeShipTriggers", 1420);
					return;
				}
				if (!(gameEntity != null))
				{
					break;
				}
				this._triggers.Add(new BlockedEstuaryMissionController.EnemyShipTrigger(gameEntity, volumeBox, this._enemyShipOrigins[num - 2], "sp_enemy_ship_destination_" + num));
			}
		}

		// Token: 0x0600064A RID: 1610 RVA: 0x000254EC File Offset: 0x000236EC
		private void ClearEnemyGroups()
		{
			for (int i = this._enemyAgentSpawnPoints.Count - 1; i >= 0; i--)
			{
				this._enemyAgentSpawnPoints[i].Clear();
			}
			this._enemyAgentSpawnPoints = null;
		}

		// Token: 0x0600064B RID: 1611 RVA: 0x0002552C File Offset: 0x0002372C
		public override void OnAgentMount(Agent agent)
		{
			if (this.CurrentPhase == BlockedEstuaryMissionController.BattlePhase.Phase2 && !this._checkPointReached && !this._startFromCheckPoint && !this._talkedToGunnar && !this.IsEnding && this.IsGunnarActive() && this._gunnarAgent.HasMount && Agent.Main.HasMount)
			{
				this._missionPhaseEndTimer = new MissionTimer(1f);
			}
			if (this._gunnarAgent == agent)
			{
				this._gunnarAgent.SetAlarmState(0);
				this._gunnarAgent.SetTargetPosition(this._gunnarAgent.Position.AsVec2);
				this._gunnarAgent.MountAgent.SetTargetPosition(this._gunnarAgent.Position.AsVec2);
			}
		}

		// Token: 0x0600064C RID: 1612 RVA: 0x000255EC File Offset: 0x000237EC
		public void OnTalkedToGunnarPhase2()
		{
			this._gunnarHorsePhaseCheckTimer = new MissionTimer(3f);
			this._gunnarAgent.MountAgent.ClearTargetFrame();
			this._gunnarAgent.ClearTargetFrame();
			string text = "event:/alerts/horns/attack";
			Vec3 position = this._enemyAgentSpawnPoints[0].Position;
			SoundManager.StartOneShotEvent(text, ref position);
			this._horse.SetMortalityState(0);
			this._playerHorse.SetMortalityState(0);
			base.Mission.SetMissionMode(2, false);
			this.GetRandomPositionAroundCheckPoint();
			this._escapePosition = this.GetRandomPositionAroundCheckPoint();
			WorldPosition worldPosition;
			worldPosition..ctor(base.Mission.Scene, this._escapePosition);
			this._gunnarAgent.SetScriptedPosition(ref worldPosition, true, 9);
			this._missionObjectiveLogic.StartObjective(new ReachShipObjective(base.Mission, this._gunnarAgent, this._playerShip));
			this.ActivateEnemyShips();
		}

		// Token: 0x0600064D RID: 1613 RVA: 0x000256CC File Offset: 0x000238CC
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if (affectedAgent.IsHuman && this._enemyAgentSpawnPoints != null)
			{
				for (int i = this._enemyAgentSpawnPoints.Count - 1; i >= 0; i--)
				{
					if (this._enemyAgentSpawnPoints[i].IsDepleted())
					{
						this._enemyAgentSpawnPoints[i].Clear();
						this._enemyAgentSpawnPoints.RemoveAt(i);
					}
				}
			}
			if (affectedAgent == this._gunnarAgent)
			{
				this._gunnarAgent = null;
			}
		}

		// Token: 0x0600064E RID: 1614 RVA: 0x00025744 File Offset: 0x00023944
		private Vec3 GetRandomPositionAroundCheckPoint()
		{
			Vec3 globalPosition = this._checkPointZone.GameEntity.GlobalPosition;
			float z = globalPosition.z;
			base.Mission.Scene.GetHeightAtPoint(globalPosition.AsVec2, 0, ref z);
			globalPosition.z = z;
			return base.Mission.GetRandomPositionAroundPoint(globalPosition, 1f, 3f, false);
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x000257A8 File Offset: 0x000239A8
		public override void OnMissileHit(Agent attacker, Agent victim, bool isCanceled, AttackCollisionData collisionData)
		{
			if (collisionData.MissileGoneUnderWater && this._navalShipsLogic.IsMissileFromShipSiegeEngine(collisionData.AffectorWeaponSlotOrMissileIndex))
			{
				this._projectileParticles.Add(new BlockedEstuaryMissionController.BurningProjectile(collisionData.CollisionGlobalPosition, 300f, MBRandom.RandomFloatRanged(0.2f, 1.5f), () => this.CurrentPhase > BlockedEstuaryMissionController.BattlePhase.Phase1));
			}
		}

		// Token: 0x06000650 RID: 1616 RVA: 0x0002580C File Offset: 0x00023A0C
		public override void OnAgentDismount(Agent agent)
		{
			base.OnAgentDismount(agent);
			if (this._checkPointReached && agent.IsMainAgent)
			{
				Agent.Main.SetAgentFlags(Agent.Main.GetAgentFlags() & -8193);
				if (this.IsGunnarActive())
				{
					this._gunnarAgent.FadeOut(true, true);
				}
			}
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x00025860 File Offset: 0x00023A60
		private void TickHorse(Agent rider)
		{
			Vec2 currentVelocity = rider.GetCurrentVelocity();
			float num = ((MathF.Abs(currentVelocity.x) <= 0.2f) ? 0f : currentVelocity.x);
			float num2 = ((MathF.Abs(currentVelocity.y) <= 0.2f) ? 0f : currentVelocity.y);
			Vec2 vec;
			vec..ctor(-num, -num2);
			rider.MovementInputVector = vec;
			rider.EventControlFlags |= 1;
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x000258D4 File Offset: 0x00023AD4
		private void OnCheckPointReached()
		{
			if (!this._startFromCheckPoint)
			{
				this.GetQuest().OnCheckPointReached();
				this.ClearEnemyGroups();
			}
			InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=BWSp3Uyj}Checkpoint reached.", null).ToString(), new Color(0f, 1f, 0f, 1f)));
			BlockedEstuaryMissionController.ShowNotification(new TextObject("{=McvglMqm}Time to get back aboard. Get on the ship.", null), this.IsGunnarActive(), 2);
			Action onCheckPointReachedEvent = this.OnCheckPointReachedEvent;
			if (onCheckPointReachedEvent != null)
			{
				onCheckPointReachedEvent();
			}
			this._checkPointReached = true;
			if (this.IsGunnarActive())
			{
				this._gunnarAgent.SetTeam(base.Mission.PlayerTeam, true);
				DailyBehaviorGroup behaviorGroup = this._gunnarAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.GetBehaviorGroup<DailyBehaviorGroup>();
				if (behaviorGroup != null)
				{
					behaviorGroup.RemoveBehavior<FollowAgentBehavior>();
				}
			}
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("sp_wind_checkpoint");
			if (gameEntity != null)
			{
				this.SetWindStrengthAndDirection(gameEntity.GetGlobalFrame().rotation.f.AsVec2.Normalized(), gameEntity.GetGlobalScale().y);
			}
		}

		// Token: 0x06000653 RID: 1619 RVA: 0x000259EC File Offset: 0x00023BEC
		private void TickMissionPhase3(float dt)
		{
			if (this._escapeZone.IsPointIn(this._playerShip.GlobalFrame.origin) && !this._playerShip.GetIsConnected())
			{
				if (!this.IsEnding)
				{
					this.OnPlayerShipReachedDestination();
				}
			}
			else if (this.GetTroopCountOfShip(this._playerShip) == 0 && !this.IsEnding)
			{
				this.OnShipCaptured(this._playerShip);
			}
			this.TickEnemyShips(dt);
		}

		// Token: 0x06000654 RID: 1620 RVA: 0x00025A5C File Offset: 0x00023C5C
		private void OnShipCaptured(MissionShip ship)
		{
			ship.SetAnchor(true, true, 1f);
			ship.ShipOrder.SetShipStopOrder();
			ship.SetShipOrderActive(false);
			this.OnFail(new TextObject("{=EydY9CXU}The enemy has captured your ship!", null));
		}

		// Token: 0x06000655 RID: 1621 RVA: 0x00025A8E File Offset: 0x00023C8E
		private int GetTroopCountOfShip(MissionShip ship)
		{
			return this._navalAgentsLogic.GetActiveAgentCountOfShip(ship) - this._navalAgentsLogic.GetActiveHeroCountOfShip(ship);
		}

		// Token: 0x06000656 RID: 1622 RVA: 0x00025AAC File Offset: 0x00023CAC
		private void TickEnemyShips(float dt)
		{
			float num = float.MaxValue;
			foreach (BlockedEstuaryMissionController.EnemyShipTrigger enemyShipTrigger in this._triggers)
			{
				enemyShipTrigger.Tick(this._playerShip, dt);
				num = MathF.Min(num, enemyShipTrigger.Ship.GameEntity.GlobalPosition.Distance(this._playerShip.GameEntity.GlobalPosition));
				if (!this._sightedEnemies && Agent.Main != null && this.CanSeeShip(Agent.Main, enemyShipTrigger.Ship))
				{
					this._sightedEnemies = true;
					BlockedEstuaryMissionController.ShowNotification(new TextObject("{=XSobP84d}There they are! Get ready to evade them…", null), true, 2);
				}
			}
			if (this._sightedEnemies)
			{
				if (!this._playerShipHasLowHealth && (this._playerShip.HitPoints <= this._playerShip.MaxHealth * 0.4f || this._playerShip.FireHitPoints <= this._playerShip.MaxFireHealth * 0.3f))
				{
					this._playerShipHasLowHealth = true;
					this._shipHitNotificationTimer -= 4f;
					BlockedEstuaryMissionController.ShowNotification(new TextObject("{=FsT98D3x}We can't take much more!", null), true, 3);
				}
				else if (!this._enemyGotClose && num < 40f)
				{
					this._enemyGotClose = true;
					BlockedEstuaryMissionController.ShowNotification(new TextObject("{=SW0y8Rbp}Don't let them catch us! We need to get the silver out of here.", null), true, 3);
				}
				this._incomingShotNotificationTimer += dt;
				this._boardedNotificationTimer += dt;
				this._shipHitNotificationTimer += dt;
			}
		}

		// Token: 0x06000657 RID: 1623 RVA: 0x00025C54 File Offset: 0x00023E54
		private void OnPlayerShipReachedDestination()
		{
			this.OnSuccess(null);
			MBMusicManager.Current.ForceStopThemeWithFadeOut();
			BlockedEstuaryMissionController.ShowNotification(new TextObject("{=7arwZMka}Success! You have run the Sea Hound blockade and reached the sea.", null), false, 2);
		}

		// Token: 0x06000658 RID: 1624 RVA: 0x00025C7C File Offset: 0x00023E7C
		public override void OnBehaviorInitialize()
		{
			this._navalShipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
			this._navalAgentsLogic = base.Mission.GetMissionBehavior<NavalAgentsLogic>();
			this._shipAgentSpawnLogic = base.Mission.GetMissionBehavior<DefaultNavalMissionAgentSpawnLogic>();
			this._navalShipsLogic.SetDeploymentMode(true);
			this._navalShipsLogic.SetTeamShipDeploymentLimit(0, NavalShipDeploymentLimit.Max());
			this._navalShipsLogic.SetTeamShipDeploymentLimit(1, NavalShipDeploymentLimit.Max());
			this._navalShipsLogic.SetTeamShipDeploymentLimit(2, NavalShipDeploymentLimit.Max());
			this._navalShipsLogic.SetDeploymentMode(false);
			this._navalShipsLogic.ShipCollisionEvent += this.OnShipCollision;
			this._navalShipsLogic.ShipSunkEvent += this.OnShipSunk;
			this._navalShipsLogic.AddShipSiegeEngineMissileEvent += this.OnBallistaShot;
			this._navalShipsLogic.ShipHitEvent += this.OnShipHit;
			this._navalShipsLogic.BridgeConnectedEvent += this.OnBridgeConnected;
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("escape_zone");
			this._escapeZone = ((gameEntity != null) ? gameEntity.GetFirstScriptOfType<VolumeBox>() : null);
			GameEntity gameEntity2 = base.Mission.Scene.FindEntityWithTag("jumping_zone");
			this._jumpingZone = ((gameEntity2 != null) ? gameEntity2.GetFirstScriptOfType<VolumeBox>() : null);
			GameEntity gameEntity3 = base.Mission.Scene.FindEntityWithTag("fire_2_zone");
			this._fire2Zone = ((gameEntity3 != null) ? gameEntity3.GetFirstScriptOfType<VolumeBox>() : null);
			GameEntity gameEntity4 = base.Mission.Scene.FindEntityWithTag("burning_zone");
			this._initialTriggerZone = ((gameEntity4 != null) ? gameEntity4.GetFirstScriptOfType<VolumeBox>() : null);
			GameEntity gameEntity5 = base.Mission.Scene.FindEntityWithTag("fire_3_zone");
			this._fire3Zone = ((gameEntity5 != null) ? gameEntity5.GetFirstScriptOfType<VolumeBox>() : null);
			GameEntity gameEntity6 = base.Mission.Scene.FindEntityWithTag("dismount_zone");
			this._checkPointZone = ((gameEntity6 != null) ? gameEntity6.GetFirstScriptOfType<VolumeBox>() : null);
			if (!SailWindProfile.IsSailWindProfileInitialized)
			{
				SailWindProfile.InitializeProfile();
			}
		}

		// Token: 0x06000659 RID: 1625 RVA: 0x00025E70 File Offset: 0x00024070
		private void OnShipSunk(MissionShip ship)
		{
			if (this.CurrentPhase == BlockedEstuaryMissionController.BattlePhase.Phase1)
			{
				if (ship == this.BurningShip)
				{
					this.OnFail(new TextObject("{=Ctrq2rg7}Your ship has sunk!", null));
					string text = "event:/mission/movement/vessel/ship_sink";
					MatrixFrame matrixFrame = this.BurningShip.GlobalFrame;
					SoundManager.StartOneShotEvent(text, ref matrixFrame.origin);
					return;
				}
			}
			else if (ship == this._playerShip)
			{
				this.OnFail(new TextObject("{=Ctrq2rg7}Your ship has sunk!", null));
				string text2 = "event:/mission/movement/vessel/ship_sink";
				MatrixFrame matrixFrame = this._playerShip.GlobalFrame;
				SoundManager.StartOneShotEvent(text2, ref matrixFrame.origin);
			}
		}

		// Token: 0x0600065A RID: 1626 RVA: 0x00025EF6 File Offset: 0x000240F6
		private void CacheParticleEntities()
		{
			this._playerShipBurningSystems = this.CreateBurningSystemForPlayerShip(this.BurningShip);
			this._enemyShipBurningSystem = this.CreateBurningSystem(this.TargetShip.GameEntity);
		}

		// Token: 0x0600065B RID: 1627 RVA: 0x00025F24 File Offset: 0x00024124
		private void OnBridgeConnected(MissionShip source, MissionShip target)
		{
			if (this.CurrentPhase == BlockedEstuaryMissionController.BattlePhase.Phase3 && this._sightedEnemies && target == this._playerShip && !this._playerShip.IsSinking && !this._playerShip.IsDisabled && Agent.Main != null && Agent.Main.IsActive() && this._boardedNotificationTimer >= 15f)
			{
				this._boardedNotificationTimer = 0f;
				BlockedEstuaryMissionController.ShowNotification(new TextObject("{=s3PsXlsG}They've grappled us!", null), true, 2);
			}
		}

		// Token: 0x0600065C RID: 1628 RVA: 0x00025FA4 File Offset: 0x000241A4
		private void OnBallistaShot(Mission.Missile missile)
		{
			if (this.CurrentPhase == BlockedEstuaryMissionController.BattlePhase.Phase3 && this._sightedEnemies && this.IsShipActive(this._playerShip) && this._incomingShotNotificationTimer >= 15f && MBRandom.RandomFloat < 0.2f)
			{
				this._incomingShotNotificationTimer = 0f;
				BlockedEstuaryMissionController.ShowNotification(new TextObject("{=4qEPNXOn}Look out!", null), true, 1);
			}
		}

		// Token: 0x0600065D RID: 1629 RVA: 0x00026006 File Offset: 0x00024206
		private bool IsShipActive(MissionShip ship)
		{
			return ship != null && !ship.IsSinking && !ship.IsDisabled;
		}

		// Token: 0x0600065E RID: 1630 RVA: 0x00026020 File Offset: 0x00024220
		private void OnShipHit(MissionShip ship, Agent attackerAgent, int damage, Vec3 impactPosition, Vec3 impactDirection, MissionWeapon weapon, int affectorWeaponSlotOrMissileIndex)
		{
			if (ship == this._playerShip && this.CurrentPhase == BlockedEstuaryMissionController.BattlePhase.Phase3 && this._sightedEnemies && this.IsShipActive(this._playerShip) && this._navalShipsLogic.IsMissileFromShipSiegeEngine(affectorWeaponSlotOrMissileIndex) && !this._playerShipHasLowHealth && this._shipHitNotificationTimer >= 15f)
			{
				this._shipHitNotificationTimer = 0f;
				BlockedEstuaryMissionController.ShowNotification(new TextObject("{=xnV0CSK4}Oi! That was a direct hit! Not the end of us yet but let's be careful!", null), true, 2);
			}
		}

		// Token: 0x0600065F RID: 1631 RVA: 0x00026098 File Offset: 0x00024298
		private Dictionary<BurnShipObject, ValueTuple<BurningSystem, float>> CreateBurningSystemForPlayerShip(MissionShip burningShip)
		{
			Dictionary<BurnShipObject, ValueTuple<BurningSystem, float>> dictionary = new Dictionary<BurnShipObject, ValueTuple<BurningSystem, float>>();
			for (int i = 0; i < this._burningMachines.Count; i++)
			{
				BurnShipObject burnShipObject = this._burningMachines[i];
				WeakGameEntity gameEntity = burnShipObject.GameEntity;
				dictionary[burnShipObject] = new ValueTuple<BurningSystem, float>(this.CreateBurningSystem(gameEntity), 0f);
			}
			return dictionary;
		}

		// Token: 0x06000660 RID: 1632 RVA: 0x000260F0 File Offset: 0x000242F0
		private BurningSystem CreateBurningSystem(WeakGameEntity parent)
		{
			GameEntity gameEntity = GameEntity.CreateFromWeakEntity(parent.GetFirstChildEntityWithTagRecursive("fire_particles"));
			if (gameEntity == null)
			{
				return null;
			}
			gameEntity.SetVisibilityExcludeParents(true);
			List<GameEntity> list = gameEntity.GetChildren().ToList<GameEntity>();
			BurningSystem burningSystem = new BurningSystem(gameEntity, 0.5f);
			foreach (GameEntity gameEntity2 in list)
			{
				this.CreateBurningNode(burningSystem, gameEntity2);
			}
			burningSystem.SetExternalFlameMultiplier(2f);
			return burningSystem;
		}

		// Token: 0x06000661 RID: 1633 RVA: 0x0002618C File Offset: 0x0002438C
		private void CreateBurningNode(BurningSystem system, GameEntity newNode)
		{
			BurningNode firstScriptOfType = newNode.GetFirstScriptOfType<BurningNode>();
			if (firstScriptOfType != null)
			{
				system.AddNewNode(firstScriptOfType);
				if (MBRandom.RandomFloat > 0.9f)
				{
					firstScriptOfType.EnableSparks();
				}
			}
		}

		// Token: 0x06000662 RID: 1634 RVA: 0x000261BC File Offset: 0x000243BC
		private void OnShipCollision(MissionShip ship1, WeakGameEntity targetEntity, BodyFlags bodyFlags, Vec3 averageContactPoint, Vec3 totalImpulseOnShip, bool isFirstImpact)
		{
			if (this.CurrentPhase == BlockedEstuaryMissionController.BattlePhase.Phase1 && !this._shipsCollided && !this.IsEnding && ((this.IsShipBurning && targetEntity == this.TargetShip.GameEntity && ship1 == this.BurningShip) || (ship1 == this.TargetShip && targetEntity == this.BurningShip.GameEntity)))
			{
				BlockedEstuaryMissionController.ShowNotification(new TextObject("{=LZwFmIOY}You did it! Look at that ship go up in flames! Their whole blockade will be in disarray!", null), this.IsGunnarActive(), 2);
				this._shipsCollided = true;
				this._collisionTimer = null;
				string text = "event:/physics/vessel/ship_ramming";
				Vec3 vec = (ship1.GameEntity.GetBodyWorldTransform().origin + targetEntity.GetBodyWorldTransform().origin) * 0.5f;
				SoundManager.StartOneShotEvent(text, ref vec, "Force", 1f);
			}
		}

		// Token: 0x06000663 RID: 1635 RVA: 0x00026295 File Offset: 0x00024495
		public override void OnMissionStateFinalized()
		{
			this.Clear();
			SailWindProfile.FinalizeProfile();
		}

		// Token: 0x06000664 RID: 1636 RVA: 0x000262A2 File Offset: 0x000244A2
		private void Clear()
		{
			this.OnCheckPointReachedEvent = null;
			this.OnLastExitZoneReachedEvent = null;
			this.OnPhaseEnd = null;
		}

		// Token: 0x06000665 RID: 1637 RVA: 0x000262BC File Offset: 0x000244BC
		public override void AfterStart()
		{
			base.AfterStart();
			base.Mission.Scene.SetWaterStrength(1f);
			this._missionObjectiveLogic = base.Mission.GetMissionBehavior<MissionObjectiveLogic>();
			this.SpawnEnemyTargetShip();
			this.SpawnPlayerBurningShip();
			this.CacheParticleEntities();
			if (this._startFromCheckPoint)
			{
				this.SpawnPlayerTradeShip();
				this.SpawnPlayerOnShip(this._playerShip);
				this.SpawnGunnarOnShip(this._playerShip);
				this.CurrentPhase = BlockedEstuaryMissionController.BattlePhase.Phase3;
				this._playerShip.SetAnchor(false, false, 1f);
				this._playerShip.Formation.SetControlledByAI(true, false);
				this._playerShip.ShipOrder.FormationJoinShip(this._playerShip.Formation);
				this._playerShip.SetShipOrderActive(true);
				this.DisableShip(this.BurningShip, true);
				this.DisableShip(this.TargetShip, true);
				this._shipsCollided = true;
				this.IsShipBurning = true;
				this._missionObjectiveLogic.StartObjective(new ReachEscapeZoneObjective(base.Mission, this._playerShip, this._escapeZone.GameEntity.GlobalPosition + new Vec3(0f, 0f, 5f, -1f)));
			}
			else
			{
				this.SpawnPlayerOnShip(this.BurningShip);
				this.SpawnGunnar("sp_gangradir_burning_ship", true);
				this._shipAgentSpawnLogic.AllocateAndDeployInitialTroops(1);
				this._missionObjectiveLogic.StartObjective(new BurnShipObjective(base.Mission, this.TargetShip));
			}
			this.InitializeShipTriggers();
		}

		// Token: 0x06000666 RID: 1638 RVA: 0x00026440 File Offset: 0x00024640
		private void SpawnPlayerOnShip(MissionShip ship)
		{
			this._navalAgentsLogic.AddReservedTroopToShip(new PartyAgentOrigin(PartyBase.MainParty, CharacterObject.PlayerCharacter, -1, default(UniqueTroopDescriptor), false, false), ship);
			this._navalAgentsLogic.SpawnNextBatch(0, false, null);
			this._mainAgentNavalComponent = Agent.Main.GetComponent<AgentNavalComponent>();
			this._navalAgentsLogic.AssignCaptainToShipForDeploymentMode(Agent.Main, ship, null);
			Mission.Current.PlayerTeam.PlayerOrderController.Owner = Agent.Main;
		}

		// Token: 0x06000667 RID: 1639 RVA: 0x000264BF File Offset: 0x000246BF
		private bool IsGunnarActive()
		{
			return this._gunnarAgent != null && this._gunnarAgent.IsActive();
		}

		// Token: 0x06000668 RID: 1640 RVA: 0x000264D8 File Offset: 0x000246D8
		private void Initialize()
		{
			this._initialized = true;
			if (!this._startFromCheckPoint)
			{
				this.InitializeEnemyShip(this.TargetShip);
			}
			MatrixFrame matrixFrame = base.Mission.Scene.FindEntityWithTag("sp_player_ship").GetGlobalFrame();
			Vec2 vec = matrixFrame.rotation.f.AsVec2.Normalized();
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag(this._startFromCheckPoint ? "sp_wind_checkpoint" : "sp_wind");
			if (gameEntity != null)
			{
				matrixFrame = gameEntity.GetGlobalFrame();
				this.SetWindStrengthAndDirection(matrixFrame.rotation.f.AsVec2.Normalized(), gameEntity.GetGlobalScale().y);
			}
			else
			{
				this.SetWindStrengthAndDirection(vec, 4f);
			}
			base.Mission.OnDeploymentFinished();
			base.Mission.OnAfterDeploymentFinished();
			MBMusicManager.Current.StartThemeWithConstantIntensity(10241, false);
			MBMusicManager.Current.ChangeCurrentThemeIntensity(0.5f);
			if (!this._startFromCheckPoint)
			{
				BlockedEstuaryMissionController.ShowNotification(new TextObject("{=6ZiKOdbI}Once we get within range, their ballista will pelt us with fiery missiles. Avoid them – even if they just hit the water, the flames will keep burning and can spread to our hull.", null), true, 2);
				BlockedEstuaryMissionController.ShowNotification(new TextObject("{=b1KaR0Hk}When we get close, I will set fire to our ship and then we swim to shore.", null), true, 2);
			}
		}

		// Token: 0x06000669 RID: 1641 RVA: 0x00026603 File Offset: 0x00024803
		private void OnFail(TextObject notification)
		{
			PlayerEncounter.CampaignBattleResult = CampaignBattleResult.GetResult(2, false);
			this._missionEndTimer = new MissionTimer(2f);
			BlockedEstuaryMissionController.ShowNotification(notification, false, 2);
		}

		// Token: 0x0600066A RID: 1642 RVA: 0x00026629 File Offset: 0x00024829
		private void OnSuccess(TextObject notification = null)
		{
			PlayerEncounter.CampaignBattleResult = CampaignBattleResult.GetResult(1, false);
			this._missionEndTimer = new MissionTimer(2f);
			if (!TextObject.IsNullOrEmpty(notification))
			{
				BlockedEstuaryMissionController.ShowNotification(notification, false, 2);
			}
		}

		// Token: 0x0600066B RID: 1643 RVA: 0x00026658 File Offset: 0x00024858
		private void OnFinalize()
		{
			this._navalShipsLogic.ShipCollisionEvent -= this.OnShipCollision;
			this._navalShipsLogic.ShipSunkEvent -= this.OnShipSunk;
			this._navalShipsLogic.AddShipSiegeEngineMissileEvent -= this.OnBallistaShot;
			this._navalShipsLogic.ShipHitEvent -= this.OnShipHit;
			this._navalShipsLogic.BridgeConnectedEvent -= this.OnBridgeConnected;
			base.Mission.EndMission();
		}

		// Token: 0x0600066C RID: 1644 RVA: 0x000266E4 File Offset: 0x000248E4
		private void SpawnPlayerBurningShip()
		{
			Formation formation = base.Mission.PlayerTeam.GetFormation(1);
			string text = (this._startFromCheckPoint ? "sp_player_burning_ship_checkpoint" : "sp_player_burning_ship");
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag(text);
			this.BurningShip = this.CreateShip(this._playerBurningShipOrigin, base.Mission.PlayerTeam, formation, gameEntity);
			formation.SetControlledByAI(false, false);
			this.BurningShip.SetShipOrderActive(false);
			this.InitializeBurningMachines();
		}

		// Token: 0x0600066D RID: 1645 RVA: 0x00026763 File Offset: 0x00024963
		private void InitializeBurningMachines()
		{
			this._burningMachines = MBExtensions.CollectScriptComponentsIncludingChildrenRecursive<BurnShipObject>(this.BurningShip.GameEntity);
		}

		// Token: 0x0600066E RID: 1646 RVA: 0x0002677C File Offset: 0x0002497C
		private void SpawnPlayerTradeShip()
		{
			Formation formation = base.Mission.PlayerTeam.GetFormation(0);
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("sp_player_ship");
			this._playerShip = this.CreateShip(this._playerShipOrigin, base.Mission.PlayerTeam, formation, gameEntity);
			if (!this._startFromCheckPoint)
			{
				this._playerShip.OnDeploymentFinished();
			}
			this._playerShip.SetAnchor(true, true, 1f);
			this.SpawnPlayerTeamAgents();
			this._playerShip.ShipOrder.SetShipStopOrder();
			this.SetTargetPoint(this._playerShip, new Vec3(0f, -20f, 0f, -1f));
		}

		// Token: 0x0600066F RID: 1647 RVA: 0x00026830 File Offset: 0x00024A30
		private void SetTargetPoint(MissionShip playerShip, Vec3 localOffset)
		{
			ShipTargetMissionObject firstScriptInFamilyDescending = MBExtensions.GetFirstScriptInFamilyDescending<ShipTargetMissionObject>(playerShip.GameEntity);
			if (firstScriptInFamilyDescending != null)
			{
				firstScriptInFamilyDescending.GameEntity.SetLocalPosition(localOffset + firstScriptInFamilyDescending.GameEntity.GetLocalFrame().origin);
			}
		}

		// Token: 0x06000670 RID: 1648 RVA: 0x00026874 File Offset: 0x00024A74
		private void DisableTargetShipObject(MissionShip ship)
		{
			ShipTargetMissionObject firstScriptInFamilyDescending = MBExtensions.GetFirstScriptInFamilyDescending<ShipTargetMissionObject>(ship.GameEntity);
			if (firstScriptInFamilyDescending != null)
			{
				firstScriptInFamilyDescending.SetDisabled(false);
			}
		}

		// Token: 0x06000671 RID: 1649 RVA: 0x00026898 File Offset: 0x00024A98
		private void SpawnPlayerTeamAgents()
		{
			int num = this._playerShip.ShipOrigin.MainDeckCrewCapacity - 2;
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._playerShip, this._playerShip.ShipOrigin.MainDeckCrewCapacity);
			int num2 = 0;
			foreach (FlattenedTroopRosterElement flattenedTroopRosterElement in PartyBase.MainParty.MemberRoster.ToFlattenedRoster())
			{
				if (!flattenedTroopRosterElement.Troop.IsHero)
				{
					this._navalAgentsLogic.AddReservedTroopToShip(new PartyAgentOrigin(PartyBase.MainParty, flattenedTroopRosterElement.Troop, -1, default(UniqueTroopDescriptor), false, false), this._playerShip);
					num2++;
				}
				if (num2 >= num)
				{
					break;
				}
			}
			this._navalAgentsLogic.SpawnNextBatch(0, false, null);
		}

		// Token: 0x06000672 RID: 1650 RVA: 0x00026974 File Offset: 0x00024B74
		private void SpawnGunnar(string spawnId, bool noHorses)
		{
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag(spawnId);
			if (gameEntity != null)
			{
				this.SpawnGunnar(gameEntity.GlobalPosition, noHorses);
				return;
			}
			Debug.FailedAssert("Cant find entity.", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Storyline\\MissionControllers\\BlockedEstuaryMissionController.cs", "SpawnGunnar", 2092);
		}

		// Token: 0x06000673 RID: 1651 RVA: 0x000269C4 File Offset: 0x00024BC4
		private void SpawnGunnar(Vec3 position, bool noHorses)
		{
			Vec3 vec = position;
			Vec2 vec2 = (Agent.Main.Position - vec).AsVec2.Normalized();
			Equipment equipment = NavalStorylineData.Gunnar.BattleEquipment.Clone(false);
			if (!noHorses)
			{
				ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>("sturgia_horse_tournament");
				equipment[10] = new EquipmentElement(@object, null, null, false);
			}
			MissionEquipment missionEquipment = new MissionEquipment(equipment, null);
			AgentBuildData agentBuildData = new AgentBuildData(NavalStorylineData.Gunnar.CharacterObject).TroopOrigin(new SimpleAgentOrigin(NavalStorylineData.Gunnar.CharacterObject, -1, null, default(UniqueTroopDescriptor))).Team(base.Mission.PlayerTeam).InitialPosition(ref vec)
				.InitialDirection(ref vec2)
				.NoHorses(noHorses)
				.NoWeapons(true)
				.Equipment(equipment)
				.MissionEquipment(missionEquipment);
			this._gunnarAgent = Mission.Current.SpawnAgent(agentBuildData, false);
			this._gunnarAgent.GetComponent<CampaignAgentComponent>().CreateAgentNavigator();
			this._gunnarAgent.GetComponent<CampaignAgentComponent>().AgentNavigator.AddBehaviorGroup<DailyBehaviorGroup>();
			AgentNavalComponent component = this._gunnarAgent.GetComponent<AgentNavalComponent>();
			if (component == null)
			{
				return;
			}
			component.SetCanDrown(false);
		}

		// Token: 0x06000674 RID: 1652 RVA: 0x00026AEC File Offset: 0x00024CEC
		private void TriggerEnemyShip(MissionShip ship, MissionShip target = null)
		{
			ship.SetAnchor(false, false, 1f);
			ship.SetShipOrderActive(true);
			ship.ShipOrder.SetShipEngageOrder(target);
			ship.ShipOrder.SetBoardingTargetShip(target);
			this.ToggleShipBallistas(ship, true);
			ship.ShipOrder.FormationJoinShip(ship.Formation);
		}

		// Token: 0x06000675 RID: 1653 RVA: 0x00026B3E File Offset: 0x00024D3E
		private void InitializeEnemyShip(MissionShip ship)
		{
			ship.ShipOrder.FormationJoinShip(ship.Formation);
			ship.ShipOrder.SetShipStopOrder();
			ship.SetAnchor(true, true, 1f);
			ship.Formation.SetControlledByAI(false, false);
			ship.SetShipOrderActive(true);
		}

		// Token: 0x06000676 RID: 1654 RVA: 0x00026B80 File Offset: 0x00024D80
		private void SpawnEnemyTargetShip()
		{
			Formation formation = base.Mission.PlayerEnemyTeam.GetFormation(0);
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("sp_enemy_ship_1");
			this.TargetShip = this.CreateShip(this._enemyBurningShipOrigin, base.Mission.PlayerEnemyTeam, formation, gameEntity);
			this.TargetShip.SetCanBeTakenOver(false);
		}

		// Token: 0x06000677 RID: 1655 RVA: 0x00026BE0 File Offset: 0x00024DE0
		private MissionShip SpawnEnemyChaserShip(GameEntity spawnPoint, IShipOrigin shipOrigin)
		{
			Formation formation = base.Mission.PlayerEnemyTeam.FormationsIncludingEmpty.First<Formation>((Formation x) => x.CountOfUnits == 0 && x != this.TargetShip.Formation);
			MissionShip missionShip = this.CreateShip(shipOrigin, base.Mission.PlayerEnemyTeam, formation, spawnPoint);
			missionShip.SetCanBeTakenOver(false);
			int num = MBRandom.RandomInt(12, 14);
			int num2 = MBRandom.RandomInt(8, 10);
			CharacterObject @object = MBObjectManager.Instance.GetObject<CharacterObject>("vlandian_swordsman");
			CharacterObject object2 = MBObjectManager.Instance.GetObject<CharacterObject>("vlandian_marine_t5");
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(missionShip, missionShip.ShipOrigin.MainDeckCrewCapacity);
			for (int i = 0; i < num; i++)
			{
				this._navalAgentsLogic.AddReservedTroopToShip(new SimpleAgentOrigin(@object, -1, null, default(UniqueTroopDescriptor)), missionShip);
			}
			for (int j = 0; j < num2; j++)
			{
				this._navalAgentsLogic.AddReservedTroopToShip(new SimpleAgentOrigin(object2, -1, null, default(UniqueTroopDescriptor)), missionShip);
			}
			this._navalAgentsLogic.SpawnNextBatch(2, false, null);
			return missionShip;
		}

		// Token: 0x06000678 RID: 1656 RVA: 0x00026CE8 File Offset: 0x00024EE8
		private SpeakToTheSailorsQuest GetQuest()
		{
			using (List<QuestBase>.Enumerator enumerator = Campaign.Current.QuestManager.Quests.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SpeakToTheSailorsQuest speakToTheSailorsQuest;
					if ((speakToTheSailorsQuest = enumerator.Current as SpeakToTheSailorsQuest) != null)
					{
						return speakToTheSailorsQuest;
					}
				}
			}
			return null;
		}

		// Token: 0x06000679 RID: 1657 RVA: 0x00026D4C File Offset: 0x00024F4C
		private bool CanSeeShip(Agent agent, MissionShip ship)
		{
			if (agent.Position.Distance(ship.GameEntity.GlobalPosition) >= 200f)
			{
				if (!this._triggers.Any<BlockedEstuaryMissionController.EnemyShipTrigger>((BlockedEstuaryMissionController.EnemyShipTrigger x) => x.Ship.ShipOrder.TargetShip != null && x.Ship.ShipSiegeWeapon != null && x.Ship.ShipSiegeWeapon.State == 2))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600067A RID: 1658 RVA: 0x00026DAC File Offset: 0x00024FAC
		private MissionShip CreateShip(IShipOrigin ship, Team team, Formation formation, GameEntity spawnEntity)
		{
			MatrixFrame globalFrame = spawnEntity.GetGlobalFrame();
			float waterLevelAtPosition = Mission.Current.Scene.GetWaterLevelAtPosition(spawnEntity.GlobalPosition.AsVec2, true, false);
			globalFrame.origin = new Vec3(spawnEntity.GlobalPosition.x, spawnEntity.GlobalPosition.y, waterLevelAtPosition, -1f);
			return this._navalShipsLogic.SpawnShip(ship, in globalFrame, team, formation, false, 8, true);
		}

		// Token: 0x0600067B RID: 1659 RVA: 0x00026E20 File Offset: 0x00025020
		private Agent SpawnPlayerHorse()
		{
			MatrixFrame globalFrame = base.Mission.Scene.FindEntityWithTag("sp_horse").GetGlobalFrame();
			return this.SpawnHorse(globalFrame.origin, globalFrame.rotation.f.AsVec2);
		}

		// Token: 0x0600067C RID: 1660 RVA: 0x00026E68 File Offset: 0x00025068
		private Agent SpawnHorse(Vec3 position, Vec2 direction)
		{
			ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>("sturgia_horse_tournament");
			ItemRosterElement itemRosterElement;
			itemRosterElement..ctor(@object, 1, null);
			ItemObject object2 = MBObjectManager.Instance.GetObject<ItemObject>("light_harness");
			ItemRosterElement itemRosterElement2;
			itemRosterElement2..ctor(object2, 0, null);
			Mission mission = Mission.Current;
			ItemRosterElement itemRosterElement3 = itemRosterElement;
			ItemRosterElement itemRosterElement4 = itemRosterElement2;
			Vec2 vec = direction.Normalized();
			Agent agent = mission.SpawnMonster(itemRosterElement3, itemRosterElement4, ref position, ref vec, -1);
			agent.SetTargetPosition(position.AsVec2);
			agent.SetMortalityState(1);
			return agent;
		}

		// Token: 0x0600067D RID: 1661 RVA: 0x00026ED8 File Offset: 0x000250D8
		public static bool WillHitBoundingBox(Vec3 origin, Vec2 velocity2D, Vec3 boxMin, Vec3 boxMax)
		{
			if (velocity2D == Vec2.Zero)
			{
				return false;
			}
			Vec3 vec = velocity2D.ToVec3(0f);
			Vec3 vec2;
			vec2..ctor((vec.X == 0f) ? float.PositiveInfinity : (1f / vec.X), (vec.Y == 0f) ? float.PositiveInfinity : (1f / vec.Y), (vec.Z == 0f) ? float.PositiveInfinity : (1f / vec.Z), -1f);
			float num = (boxMin.X - origin.X) * vec2.X;
			float num2 = (boxMax.X - origin.X) * vec2.X;
			float num3 = (boxMin.Y - origin.Y) * vec2.Y;
			float num4 = (boxMax.Y - origin.Y) * vec2.Y;
			float num5 = (boxMin.Z - origin.Z) * vec2.Z;
			float num6 = (boxMax.Z - origin.Z) * vec2.Z;
			float num7 = Math.Max(Math.Max(Math.Min(num, num2), Math.Min(num3, num4)), Math.Min(num5, num6));
			float num8 = Math.Min(Math.Min(Math.Max(num, num2), Math.Max(num3, num4)), Math.Max(num5, num6));
			return num8 >= 0f && num7 <= num8 && Math.Max(0f, num7) <= Math.Min(1f, num8);
		}

		// Token: 0x0600067E RID: 1662 RVA: 0x0002707C File Offset: 0x0002527C
		private Vec2[] GetShipPhysicsBox(MissionShip ship)
		{
			float num = (ship.Physics.PhysicsBoundingBoxWithChildren.max.x - ship.Physics.PhysicsBoundingBoxWithChildren.min.x) / 2f - 6f;
			float num2 = (ship.Physics.PhysicsBoundingBoxWithChildren.max.y - ship.Physics.PhysicsBoundingBoxWithChildren.min.y) / 2f - 2f;
			MatrixFrame matrixFrame = ship.GameEntity.GetGlobalFrame();
			Vec2 asVec = matrixFrame.rotation.f.AsVec2;
			matrixFrame = ship.GameEntity.GetGlobalFrame();
			Vec2 asVec2 = matrixFrame.rotation.s.AsVec2;
			Vec2 asVec3 = ship.GameEntity.GlobalPosition.AsVec2;
			Vec2 vec = asVec2 * num;
			Vec2 vec2 = asVec * num2;
			Vec2 vec3 = asVec3 - vec - vec2;
			Vec2 vec4 = asVec3 + vec - vec2;
			Vec2 vec5 = asVec3 + vec + vec2;
			Vec2 vec6 = asVec3 - vec + vec2;
			return new Vec2[] { vec3, vec4, vec5, vec6 };
		}

		// Token: 0x0600067F RID: 1663 RVA: 0x000271D0 File Offset: 0x000253D0
		private bool DoesShipCollideWithProjectile(MissionShip ship, BlockedEstuaryMissionController.BurningProjectile projectile)
		{
			return projectile.Initialized && this.DoesShipCollideWithSphere(ship, projectile.GameEntity.GlobalPosition.AsVec2, 1f);
		}

		// Token: 0x06000680 RID: 1664 RVA: 0x00027206 File Offset: 0x00025406
		private bool DoesShipCollideWithSphere(MissionShip ship, Vec2 origin, float radius)
		{
			return this.PlaneIntersectsCircle(this.GetShipPhysicsBox(ship), origin, radius);
		}

		// Token: 0x06000681 RID: 1665 RVA: 0x00027218 File Offset: 0x00025418
		private bool PlaneIntersectsCircle(Vec2[] corners, Vec2 circleOrigin, float radius)
		{
			if (this.IsPointInPolygon(circleOrigin, corners))
			{
				return true;
			}
			float num = radius * radius;
			for (int i = 0; i < corners.Length; i++)
			{
				Vec2 vec = corners[i];
				Vec2 vec2 = corners[(i + 1) % corners.Length];
				float num2 = (vec2.X - vec.X) * (vec2.X - vec.X) + (vec2.Y - vec.Y) * (vec2.Y - vec.Y);
				float num3 = Math.Max(0f, Math.Min(1f, ((circleOrigin.X - vec.X) * (vec2.X - vec.X) + (circleOrigin.Y - vec.Y) * (vec2.Y - vec.Y)) / num2));
				float num4 = vec.X + num3 * (vec2.X - vec.X);
				float num5 = vec.Y + num3 * (vec2.Y - vec.Y);
				if ((circleOrigin.X - num4) * (circleOrigin.X - num4) + (circleOrigin.Y - num5) * (circleOrigin.Y - num5) <= num)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000682 RID: 1666 RVA: 0x00027364 File Offset: 0x00025564
		private bool IsPointInPolygon(Vec2 point, Vec2[] polygonCorners)
		{
			bool flag = false;
			int num = polygonCorners.Length;
			int i = 0;
			int num2 = num - 1;
			while (i < num)
			{
				if (polygonCorners[i].Y > point.Y != polygonCorners[num2].Y > point.Y && point.X < (polygonCorners[num2].X - polygonCorners[i].X) * (point.Y - polygonCorners[i].Y) / (polygonCorners[num2].Y - polygonCorners[i].Y) + polygonCorners[i].X)
				{
					flag = !flag;
				}
				num2 = i++;
			}
			return flag;
		}

		// Token: 0x06000683 RID: 1667 RVA: 0x0002741F File Offset: 0x0002561F
		// Note: this type is marked as 'beforefieldinit'.
		static BlockedEstuaryMissionController()
		{
			MBList<string> mblist = new MBList<string>();
			mblist.Add("vlandian_spearman");
			mblist.Add("vlandian_billman");
			mblist.Add("vlandian_marine_t4");
			BlockedEstuaryMissionController._enemyAgentCharacterIds = mblist;
		}

		// Token: 0x040002F2 RID: 754
		private const string EscapeZoneId = "escape_zone";

		// Token: 0x040002F3 RID: 755
		private const string JumpingZoneId = "jumping_zone";

		// Token: 0x040002F4 RID: 756
		private const string Fire2ZoneId = "fire_2_zone";

		// Token: 0x040002F5 RID: 757
		private const string InitialTriggerZoneId = "burning_zone";

		// Token: 0x040002F6 RID: 758
		private const string FireSystemId = "fire_particles";

		// Token: 0x040002F7 RID: 759
		private const string Fire3ZoneId = "fire_3_zone";

		// Token: 0x040002F8 RID: 760
		private const string CheckPointZoneId = "dismount_zone";

		// Token: 0x040002F9 RID: 761
		private const string RampHolderId = "ramp_holder";

		// Token: 0x040002FA RID: 762
		private const string EnemyShipSpawnIdBase = "sp_enemy_ship_";

		// Token: 0x040002FB RID: 763
		private const string EnemyShipTriggerSpawnIdBase = "sp_enemy_trigger_";

		// Token: 0x040002FC RID: 764
		private const string EnemyShipDestinationIdBase = "sp_enemy_ship_destination_";

		// Token: 0x040002FD RID: 765
		private const string TargetShipSpawnId = "sp_enemy_ship_1";

		// Token: 0x040002FE RID: 766
		private const string PlayerBurningShipSpawnId = "sp_player_burning_ship";

		// Token: 0x040002FF RID: 767
		private const string PlayerBurningShipCheckpointSpawnId = "sp_player_burning_ship_checkpoint";

		// Token: 0x04000300 RID: 768
		private const string PlayerShipSpawnId = "sp_player_ship";

		// Token: 0x04000301 RID: 769
		private const string PlayerWaterSpawnPointAfterFadeToBlackId = "sp_player_mount";

		// Token: 0x04000302 RID: 770
		private const string PlayerCheckPointSpawnPointId = "sp_player_checkpoint";

		// Token: 0x04000303 RID: 771
		private const string GunnarBurningShipSpawnId = "sp_gangradir_burning_ship";

		// Token: 0x04000304 RID: 772
		private const string HorseSpawnPointId = "sp_horse";

		// Token: 0x04000305 RID: 773
		private const string HorseItemId = "sturgia_horse_tournament";

		// Token: 0x04000306 RID: 774
		private const string EnemyAgentPatrolPointBaseId = "sp_guard_patrol";

		// Token: 0x04000307 RID: 775
		private const string EnemyAgentSpawnPointBaseId = "enemy_group_parent";

		// Token: 0x04000308 RID: 776
		private const float WindStrength = 4f;

		// Token: 0x04000309 RID: 777
		private const float BurningSpreadRateMultiplier = 20f;

		// Token: 0x0400030A RID: 778
		private static readonly int BurningSoundEventId = SoundManager.GetEventGlobalIndex("event:/mission/ambient/detail/fire/fire_dynamic");

		// Token: 0x0400030B RID: 779
		private const float FirePatchFireDamage = 600f;

		// Token: 0x0400030C RID: 780
		private const float DefaultSpreadRate = 0.5f;

		// Token: 0x0400030D RID: 781
		private const float EscapePhaseNotificationCooldown = 15f;

		// Token: 0x0400030E RID: 782
		private static MBList<string> _enemyAgentCharacterIds;

		// Token: 0x0400030F RID: 783
		private MissionObjectiveLogic _missionObjectiveLogic;

		// Token: 0x04000310 RID: 784
		public Action OnCheckPointReachedEvent;

		// Token: 0x04000311 RID: 785
		public Action OnLastExitZoneReachedEvent;

		// Token: 0x04000312 RID: 786
		public Action OnPhaseEnd;

		// Token: 0x04000313 RID: 787
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x04000314 RID: 788
		private NavalAgentsLogic _navalAgentsLogic;

		// Token: 0x04000315 RID: 789
		private DefaultNavalMissionAgentSpawnLogic _shipAgentSpawnLogic;

		// Token: 0x04000316 RID: 790
		private AgentNavalComponent _mainAgentNavalComponent;

		// Token: 0x04000317 RID: 791
		private VolumeBox _escapeZone;

		// Token: 0x04000318 RID: 792
		private VolumeBox _jumpingZone;

		// Token: 0x04000319 RID: 793
		private VolumeBox _fire2Zone;

		// Token: 0x0400031A RID: 794
		private VolumeBox _initialTriggerZone;

		// Token: 0x0400031B RID: 795
		private VolumeBox _fire3Zone;

		// Token: 0x0400031C RID: 796
		private VolumeBox _checkPointZone;

		// Token: 0x0400031D RID: 797
		private MBList<BlockedEstuaryMissionController.EnemyShipTrigger> _triggers = new MBList<BlockedEstuaryMissionController.EnemyShipTrigger>();

		// Token: 0x0400031E RID: 798
		private Dictionary<BurnShipObject, ValueTuple<BurningSystem, float>> _playerShipBurningSystems;

		// Token: 0x0400031F RID: 799
		private BurningSystem _enemyShipBurningSystem;

		// Token: 0x04000320 RID: 800
		private List<BlockedEstuaryMissionController.BurningProjectile> _projectileParticles = new List<BlockedEstuaryMissionController.BurningProjectile>();

		// Token: 0x04000321 RID: 801
		private float _shipDamageCheckTimer;

		// Token: 0x04000322 RID: 802
		private float _shipBurnProgress;

		// Token: 0x04000323 RID: 803
		private List<Agent> _burntShipAgents;

		// Token: 0x04000324 RID: 804
		private MBList<BurnShipObject> _burningMachines;

		// Token: 0x04000325 RID: 805
		private bool _initializeGunnarBurningShip;

		// Token: 0x04000326 RID: 806
		private bool _showedLastWarning;

		// Token: 0x04000327 RID: 807
		private SoundEvent _burningShipSoundEvent;

		// Token: 0x04000328 RID: 808
		private bool _sightedEnemies;

		// Token: 0x04000329 RID: 809
		private bool _firstCollisionFirePatch;

		// Token: 0x0400032A RID: 810
		private bool _firePatchSpawned;

		// Token: 0x0400032B RID: 811
		private float _boardedNotificationTimer;

		// Token: 0x0400032C RID: 812
		private float _incomingShotNotificationTimer;

		// Token: 0x0400032D RID: 813
		private float _shipHitNotificationTimer;

		// Token: 0x0400032E RID: 814
		private bool _playerShipHasLowHealth;

		// Token: 0x0400032F RID: 815
		private bool _enemyGotClose;

		// Token: 0x04000330 RID: 816
		private BlockedEstuaryMissionController.BattlePhase _currentPhase;

		// Token: 0x04000331 RID: 817
		private IShipOrigin _playerBurningShipOrigin;

		// Token: 0x04000332 RID: 818
		private MissionTimer _gunnarHorsePhaseCheckTimer;

		// Token: 0x04000333 RID: 819
		private IShipOrigin _enemyBurningShipOrigin;

		// Token: 0x04000334 RID: 820
		private bool _enemyAreaReached;

		// Token: 0x04000335 RID: 821
		private bool _playerLeftBehind;

		// Token: 0x04000336 RID: 822
		private IShipOrigin _playerShipOrigin;

		// Token: 0x04000337 RID: 823
		private MBList<IShipOrigin> _enemyShipOrigins = new MBList<IShipOrigin>();

		// Token: 0x04000339 RID: 825
		private bool _isShipBurning;

		// Token: 0x0400033A RID: 826
		private MissionShip _playerShip;

		// Token: 0x0400033B RID: 827
		private bool _initialized;

		// Token: 0x0400033C RID: 828
		private bool _enemiesPanicked;

		// Token: 0x0400033D RID: 829
		private bool _shipsCollided;

		// Token: 0x0400033E RID: 830
		private MissionTimer _missionEndTimer;

		// Token: 0x0400033F RID: 831
		private MissionTimer _missionPhaseEndTimer;

		// Token: 0x04000340 RID: 832
		private MissionTimer _collisionTimer;

		// Token: 0x04000341 RID: 833
		private bool _talkedToGunnar;

		// Token: 0x04000342 RID: 834
		private Agent _playerHorse;

		// Token: 0x04000343 RID: 835
		private Agent _horse;

		// Token: 0x04000344 RID: 836
		private Agent _gunnarAgent;

		// Token: 0x04000345 RID: 837
		private bool _shouldGunnarEscape;

		// Token: 0x04000346 RID: 838
		private Vec3 _escapePosition;

		// Token: 0x04000347 RID: 839
		private readonly MobileParty _enemyParty;

		// Token: 0x0400034A RID: 842
		private readonly bool _startFromCheckPoint;

		// Token: 0x0400034B RID: 843
		private bool _checkPointReached;

		// Token: 0x0400034D RID: 845
		private MBList<BlockedEstuaryMissionController.EnemySpawnPoint> _enemyAgentSpawnPoints = new MBList<BlockedEstuaryMissionController.EnemySpawnPoint>();

		// Token: 0x020001D2 RID: 466
		public enum BattlePhase
		{
			// Token: 0x04000D4A RID: 3402
			Phase1,
			// Token: 0x04000D4B RID: 3403
			Phase2,
			// Token: 0x04000D4C RID: 3404
			Phase3
		}

		// Token: 0x020001D3 RID: 467
		private class BurningProjectile
		{
			// Token: 0x170003FC RID: 1020
			// (get) Token: 0x06001A2D RID: 6701 RVA: 0x000AE97C File Offset: 0x000ACB7C
			// (set) Token: 0x06001A2E RID: 6702 RVA: 0x000AE984 File Offset: 0x000ACB84
			public bool Initialized { get; private set; }

			// Token: 0x170003FD RID: 1021
			// (get) Token: 0x06001A2F RID: 6703 RVA: 0x000AE98D File Offset: 0x000ACB8D
			// (set) Token: 0x06001A30 RID: 6704 RVA: 0x000AE995 File Offset: 0x000ACB95
			public GameEntity GameEntity { get; private set; }

			// Token: 0x06001A31 RID: 6705 RVA: 0x000AE99E File Offset: 0x000ACB9E
			public BurningProjectile(Vec3 position, float minLifeTime = 10f, float spawnAfterTime = 1f, Func<bool> enderFunction = null)
			{
				this._position = position;
				this._spawnTime = spawnAfterTime;
				this._endCondition = enderFunction;
				this._minLifeTime = minLifeTime;
			}

			// Token: 0x06001A32 RID: 6706 RVA: 0x000AE9C4 File Offset: 0x000ACBC4
			public void Tick(float dt, out bool shouldBeRemoved)
			{
				shouldBeRemoved = false;
				if (this.Initialized)
				{
					shouldBeRemoved = this._timer >= this._minLifeTime || (this._endCondition != null && this._endCondition());
				}
				else if (this._timer >= this._spawnTime)
				{
					this.SpawnEntity(this._position);
					this._timer = 0f;
				}
				this._timer += dt;
			}

			// Token: 0x06001A33 RID: 6707 RVA: 0x000AEA3C File Offset: 0x000ACC3C
			private void SpawnEntity(Vec3 position)
			{
				this.GameEntity = GameEntity.Instantiate(Mission.Current.Scene, "fire_obstacle", true, true, "");
				MatrixFrame globalFrame = this.GameEntity.GetGlobalFrame();
				globalFrame.origin = position;
				this.GameEntity.SetFrame(ref globalFrame, true);
				this.Initialized = true;
			}

			// Token: 0x06001A34 RID: 6708 RVA: 0x000AEA93 File Offset: 0x000ACC93
			public void Clear()
			{
				Mission.Current.Scene.RemoveEntity(this.GameEntity, 0);
				this.GameEntity = null;
				this.Initialized = false;
			}

			// Token: 0x04000D4D RID: 3405
			private const string ProjectileFireParticleId = "fire_obstacle";

			// Token: 0x04000D50 RID: 3408
			private float _minLifeTime;

			// Token: 0x04000D51 RID: 3409
			private float _timer;

			// Token: 0x04000D52 RID: 3410
			private float _spawnTime;

			// Token: 0x04000D53 RID: 3411
			private Vec3 _position;

			// Token: 0x04000D54 RID: 3412
			private Func<bool> _endCondition;
		}

		// Token: 0x020001D4 RID: 468
		private class EnemySpawnPoint
		{
			// Token: 0x170003FE RID: 1022
			// (get) Token: 0x06001A35 RID: 6709 RVA: 0x000AEAB9 File Offset: 0x000ACCB9
			// (set) Token: 0x06001A36 RID: 6710 RVA: 0x000AEAC1 File Offset: 0x000ACCC1
			public bool IsAlerted { get; private set; }

			// Token: 0x170003FF RID: 1023
			// (get) Token: 0x06001A37 RID: 6711 RVA: 0x000AEACA File Offset: 0x000ACCCA
			public Vec3 Position
			{
				get
				{
					return this._entity.GlobalPosition;
				}
			}

			// Token: 0x17000400 RID: 1024
			// (get) Token: 0x06001A38 RID: 6712 RVA: 0x000AEAD7 File Offset: 0x000ACCD7
			// (set) Token: 0x06001A39 RID: 6713 RVA: 0x000AEADF File Offset: 0x000ACCDF
			public Agent Agent { get; private set; }

			// Token: 0x06001A3A RID: 6714 RVA: 0x000AEAE8 File Offset: 0x000ACCE8
			public EnemySpawnPoint(string spawnId, CharacterObject character, bool isNight)
			{
				this.IsAlerted = false;
				this._entity = Mission.Current.Scene.FindEntityWithTag(spawnId);
				this.SpawnAgent(character, isNight);
			}

			// Token: 0x06001A3B RID: 6715 RVA: 0x000AEB15 File Offset: 0x000ACD15
			public EnemySpawnPoint(GameEntity spawnEntity, CharacterObject character, bool isNight)
			{
				this.IsAlerted = false;
				this._entity = spawnEntity;
				this.SpawnAgent(character, isNight);
			}

			// Token: 0x06001A3C RID: 6716 RVA: 0x000AEB34 File Offset: 0x000ACD34
			public void CalmDown()
			{
				this.Agent.SetAlarmState(1);
				if (this.Agent.Position.Distance(this.Position) >= 20f)
				{
					Vec3 randomPositionAroundPoint = Mission.Current.GetRandomPositionAroundPoint(this.Position, 1f, 3f, false);
					this.Agent.SetTargetPosition(randomPositionAroundPoint.AsVec2);
				}
				this.IsAlerted = false;
			}

			// Token: 0x06001A3D RID: 6717 RVA: 0x000AEBA3 File Offset: 0x000ACDA3
			public bool CanSeeAgent(Agent agent)
			{
				return this.Agent != null && this.Agent.IsActive() && this._navigator.CanSeeAgent(agent);
			}

			// Token: 0x06001A3E RID: 6718 RVA: 0x000AEBCC File Offset: 0x000ACDCC
			public void Alert()
			{
				this.Agent.SetTeam(Mission.Current.PlayerEnemyTeam, true);
				this.Agent.SetAgentFlags(this.Agent.GetAgentFlags() | 65536);
				this.Agent.SetAlarmState(3);
				this.Agent.ClearTargetFrame();
				this.IsAlerted = true;
			}

			// Token: 0x06001A3F RID: 6719 RVA: 0x000AEC2A File Offset: 0x000ACE2A
			public void Clear()
			{
				if (this.Agent != null && this.Agent.IsActive())
				{
					this.Agent.FadeOut(true, true);
				}
				this.IsAlerted = false;
				this.Agent = null;
				this._navigator = null;
			}

			// Token: 0x06001A40 RID: 6720 RVA: 0x000AEC64 File Offset: 0x000ACE64
			private void SpawnAgent(CharacterObject character, bool isNight)
			{
				Vec3 globalPosition = this._entity.GlobalPosition;
				Vec3 randomPositionAroundPoint = Mission.Current.GetRandomPositionAroundPoint(globalPosition, 1f, 3f, false);
				Vec2 vec = (randomPositionAroundPoint - globalPosition).AsVec2.Normalized();
				this.Agent = this.SpawnAgentAux(randomPositionAroundPoint, vec, character, isNight, null);
				this._navigator = this.Agent.GetComponent<CampaignAgentComponent>().AgentNavigator;
			}

			// Token: 0x06001A41 RID: 6721 RVA: 0x000AECD4 File Offset: 0x000ACED4
			private Agent SpawnAgentAux(Vec3 position, Vec2 direction, CharacterObject character, bool isNight, string patrolTag = null)
			{
				Equipment equipment = character.Equipment.Clone(false);
				if (isNight)
				{
					equipment[4] = new EquipmentElement(MBObjectManager.Instance.GetObject<ItemObject>("torch"), null, null, false);
				}
				AgentBuildData agentBuildData = new AgentBuildData(character).TroopOrigin(new SimpleAgentOrigin(character, -1, null, default(UniqueTroopDescriptor))).Team(Team.Invalid).InitialPosition(ref position)
					.InitialDirection(ref direction)
					.Equipment(equipment)
					.NoHorses(true)
					.NoWeapons(false)
					.Banner(NavalStorylineData.CorsairBanner);
				Agent agent = Mission.Current.SpawnAgent(agentBuildData, false);
				EquipmentIndex equipmentIndex;
				EquipmentIndex equipmentIndex2;
				bool flag;
				agent.SpawnEquipment.GetInitialWeaponIndicesToEquip(ref equipmentIndex, ref equipmentIndex2, ref flag, 0);
				if (equipmentIndex2 != -1)
				{
					agent.TryToWieldWeaponInSlot(equipmentIndex2, 2, true);
				}
				CampaignAgentComponent component = agent.GetComponent<CampaignAgentComponent>();
				component.CreateAgentNavigator();
				SandBoxManager.Instance.AgentBehaviorManager.AddFixedGuardBehaviors(agent);
				if (!string.IsNullOrEmpty(patrolTag))
				{
					component.AgentNavigator.SpecialTargetTag = patrolTag;
				}
				return agent;
			}

			// Token: 0x06001A42 RID: 6722 RVA: 0x000AEDC6 File Offset: 0x000ACFC6
			public bool IsDepleted()
			{
				return this.Agent == null || !this.Agent.IsActive();
			}

			// Token: 0x06001A43 RID: 6723 RVA: 0x000AEDE0 File Offset: 0x000ACFE0
			public void Tick(float dt, BlockedEstuaryMissionController controller)
			{
				if (!this.IsAlerted)
				{
					if (Agent.Main != null && Agent.Main.IsActive() && (this.Position.DistanceSquared(Agent.Main.Position) < 5625f || this.CanSeeAgent(Agent.Main)))
					{
						this.Alert();
						return;
					}
					if (controller.IsGunnarActive() && (this.Position.DistanceSquared(controller._gunnarAgent.Position) < 3600f || this.CanSeeAgent(controller._gunnarAgent)))
					{
						this.Alert();
					}
				}
			}

			// Token: 0x04000D55 RID: 3413
			private const float GroupRadius = 20f;

			// Token: 0x04000D57 RID: 3415
			private GameEntity _entity;

			// Token: 0x04000D59 RID: 3417
			private AgentNavigator _navigator;
		}

		// Token: 0x020001D5 RID: 469
		private class EnemyShipTrigger
		{
			// Token: 0x17000401 RID: 1025
			// (get) Token: 0x06001A44 RID: 6724 RVA: 0x000AEE7A File Offset: 0x000AD07A
			// (set) Token: 0x06001A45 RID: 6725 RVA: 0x000AEE82 File Offset: 0x000AD082
			public MissionShip Ship { get; private set; }

			// Token: 0x06001A46 RID: 6726 RVA: 0x000AEE8C File Offset: 0x000AD08C
			public EnemyShipTrigger(GameEntity spawnPoint, VolumeBox volumeBox, IShipOrigin shipOrigin, string destinationId = null)
			{
				this._trigger = volumeBox;
				this._shipOrigin = shipOrigin;
				if (!string.IsNullOrEmpty(destinationId))
				{
					this._destination = Mission.Current.Scene.FindEntityWithTag(destinationId);
				}
				this._spawnEntity = spawnPoint;
				this.SpawnShip();
			}

			// Token: 0x06001A47 RID: 6727 RVA: 0x000AEEDC File Offset: 0x000AD0DC
			public void Tick(MissionShip target, float dt)
			{
				if (!this._isTriggered && this._destination != null && this._destination.GlobalPosition.DistanceSquared(this.Ship.GameEntity.GlobalPosition) < 100f && !this.Ship.Physics.IsAnchored)
				{
					this.AnchorShip();
				}
				if (!this._isTriggered && (this._trigger.IsPointIn(target.GameEntity.GlobalPosition) || target.GameEntity.GlobalPosition.DistanceSquared(this.Ship.GameEntity.GlobalPosition) < 10000f))
				{
					this.TriggerShip();
				}
			}

			// Token: 0x06001A48 RID: 6728 RVA: 0x000AEFA0 File Offset: 0x000AD1A0
			private void SpawnShip()
			{
				BlockedEstuaryMissionController missionBehavior = Mission.Current.GetMissionBehavior<BlockedEstuaryMissionController>();
				this.Ship = missionBehavior.SpawnEnemyChaserShip(this._spawnEntity, this._shipOrigin);
				this.AnchorShip();
				missionBehavior.ToggleShipBallistas(this.Ship, false);
			}

			// Token: 0x06001A49 RID: 6729 RVA: 0x000AEFE4 File Offset: 0x000AD1E4
			private void AnchorShip()
			{
				this.Ship.SetAnchor(true, true, 1f);
				this.Ship.ShipOrder.SetShipStopOrder();
				this.Ship.SetShipOrderActive(false);
				this.Ship.Formation.SetControlledByAI(false, false);
			}

			// Token: 0x06001A4A RID: 6730 RVA: 0x000AF034 File Offset: 0x000AD234
			public void SendToDestination()
			{
				if (this._destination != null)
				{
					ShipOrder shipOrder = this.Ship.ShipOrder;
					Vec2 asVec = this._destination.GlobalPosition.AsVec2;
					Vec2 vec = this._destination.GlobalPosition.AsVec2 - this.Ship.GameEntity.GetGlobalFrame().rotation.f.AsVec2;
					vec = vec.Normalized();
					shipOrder.SetShipMovementOrder(asVec, in vec);
					this.Ship.Formation.SetControlledByAI(false, false);
				}
			}

			// Token: 0x06001A4B RID: 6731 RVA: 0x000AF0CC File Offset: 0x000AD2CC
			public void TriggerShip()
			{
				BlockedEstuaryMissionController missionBehavior = Mission.Current.GetMissionBehavior<BlockedEstuaryMissionController>();
				missionBehavior.TriggerEnemyShip(this.Ship, missionBehavior._playerShip);
				this._isTriggered = true;
			}

			// Token: 0x04000D5B RID: 3419
			private VolumeBox _trigger;

			// Token: 0x04000D5C RID: 3420
			private IShipOrigin _shipOrigin;

			// Token: 0x04000D5D RID: 3421
			private GameEntity _spawnEntity;

			// Token: 0x04000D5E RID: 3422
			private GameEntity _destination;

			// Token: 0x04000D5F RID: 3423
			private bool _isTriggered;
		}
	}
}
