using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions.AI.UsableMachineAIs;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.Objects.UsableMachines;
using NavalDLC.Missions.ShipActuators;
using NavalDLC.Missions.ShipControl;
using NavalDLC.Missions.ShipInput;
using NavalDLC.Storyline.Objectives.Quest4;
using NavalDLC.Storyline.Quests;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.MissionLogics;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.Storyline
{
	// Token: 0x0200002B RID: 43
	public class FloatingFortressSetPieceBattleMissionController : MissionLogic
	{
		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060001DF RID: 479 RVA: 0x0000B4C0 File Offset: 0x000096C0
		// (set) Token: 0x060001E0 RID: 480 RVA: 0x0000B4C8 File Offset: 0x000096C8
		public bool IsPhaseOneCompleted { get; private set; }

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060001E1 RID: 481 RVA: 0x0000B4D1 File Offset: 0x000096D1
		public bool IsStartedFromCheckpoint { get; }

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060001E2 RID: 482 RVA: 0x0000B4D9 File Offset: 0x000096D9
		public MBReadOnlyList<MissionShip> EnemyShipsOrdered
		{
			get
			{
				return this._enemyMissionShipsOrdered;
			}
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x0000B4E4 File Offset: 0x000096E4
		public FloatingFortressSetPieceBattleMissionController(bool startFromCheckpoint)
		{
			this.IsStartedFromCheckpoint = startFromCheckpoint;
			this._playerShipStandingStillLine = new FloatingFortressSetPieceBattleMissionController.VariantConversationLine(new FloatingFortressSetPieceBattleMissionController.ConversationLine[]
			{
				new FloatingFortressSetPieceBattleMissionController.SimpleConversationLine(NavalStorylineData.Bjolgur.CharacterObject, "{=PRzT0o1t}Keep rowing! The next hit might punch right through our deck!", 0f, 2),
				new FloatingFortressSetPieceBattleMissionController.SimpleConversationLine(NavalStorylineData.Bjolgur.CharacterObject, "{=3067dlpE}Keep moving! That last hit made our timbers groan!", 0f, 2),
				new FloatingFortressSetPieceBattleMissionController.SimpleConversationLine(NavalStorylineData.Bjolgur.CharacterObject, "{=jaKW2HIJ}Unless you want to swim, I suggest you keep moving!", 0f, 2),
				new FloatingFortressSetPieceBattleMissionController.SimpleConversationLine(NavalStorylineData.Bjolgur.CharacterObject, "{=BV06pwuU}Standing still? You planning to go down with the mast?", 0f, 2)
			}, FloatingFortressSetPieceBattleMissionController.VariantConversationLine.VariationType.Ordered, 10f, false);
			this._playerShipHitLine = new FloatingFortressSetPieceBattleMissionController.VariantConversationLine(new FloatingFortressSetPieceBattleMissionController.ConversationLine[]
			{
				new FloatingFortressSetPieceBattleMissionController.SimpleConversationLine(NavalStorylineData.Bjolgur.CharacterObject, "{=qA4pYH6z}That hit us! We’re still afloat, but the next time we might not be so lucky", 0f, 3),
				new FloatingFortressSetPieceBattleMissionController.SimpleConversationLine(NavalStorylineData.Bjolgur.CharacterObject, "{=Yv3BQ7cT}Stamp out those sparks, lads! Let’s not get hit again.", 0f, 3)
			}, FloatingFortressSetPieceBattleMissionController.VariantConversationLine.VariationType.Ordered, 15f, false);
			this._playerTookMangonelDownLine = new FloatingFortressSetPieceBattleMissionController.VariantConversationLine(new FloatingFortressSetPieceBattleMissionController.ConversationLine[]
			{
				new FloatingFortressSetPieceBattleMissionController.SimpleConversationLine(NavalStorylineData.Bjolgur.CharacterObject, "{=bdpsa5CC}One mangonel down!", 0f, 4),
				new FloatingFortressSetPieceBattleMissionController.SimpleConversationLine(NavalStorylineData.Bjolgur.CharacterObject, "{=k5NjdC48}You smashed that mangonel! Look at it, like a broken toy!", 0f, 4)
			}, FloatingFortressSetPieceBattleMissionController.VariantConversationLine.VariationType.Ordered, 0f, true);
			this._playerTookAllMangonelsDownLine = new FloatingFortressSetPieceBattleMissionController.SequencedConversationLine(new FloatingFortressSetPieceBattleMissionController.ConversationLine[]
			{
				new FloatingFortressSetPieceBattleMissionController.SimpleConversationLine(NavalStorylineData.Bjolgur.CharacterObject, "{=75khXDaR}You silenced those mangonels! Now let’s all move in and board them!", 0f, 2),
				new FloatingFortressSetPieceBattleMissionController.SimpleConversationLine(NavalStorylineData.Gunnar.CharacterObject, "{=4r2IhSCi}We’re right behind you! Row, lads, row!", 0f, 2)
			}, 10000f);
			this._playerShipTooCloseLine = new FloatingFortressSetPieceBattleMissionController.SimpleConversationLine(NavalStorylineData.Bjolgur.CharacterObject, "{=tl473Yje}Let’s keep our distance! Their decks are packed with bowmen!", 15f, 2);
			this._playerShipLowHpLine = new FloatingFortressSetPieceBattleMissionController.SimpleConversationLine(NavalStorylineData.Bjolgur.CharacterObject, "{=eAabzGkE}Our timbers are groaning like a sick man.", 10000f, 2);
			this._playerShipSailDestroyedLine = new FloatingFortressSetPieceBattleMissionController.SimpleConversationLine(NavalStorylineData.Bjolgur.CharacterObject, "{=gzvtND1s}Our sail is down!", 10000f, 3);
			this._playerShipRemainingAmmoLine = new FloatingFortressSetPieceBattleMissionController.SimpleConversationLine(NavalStorylineData.Bjolgur.CharacterObject, "{=O4oqNTAl}Choose your targets! Take out the mangonels before we run out of bolts!", 10000f, 3);
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x0000BD40 File Offset: 0x00009F40
		public override void AfterStart()
		{
			this._navalShipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
			this._navalAgentsLogic = base.Mission.GetMissionBehavior<NavalAgentsLogic>();
			this._missionObjectiveLogic = base.Mission.GetMissionBehavior<MissionObjectiveLogic>();
			base.Mission.Scene.SetAtmosphereWithName("TOD_naval_09_00_Overcast");
			this._navalShipsLogic.ShipHitEvent += this.OnShipHit;
			base.Mission.Teams.Add(1, base.Mission.PlayerTeam.Color, base.Mission.PlayerTeam.Color2, base.Mission.PlayerTeam.Banner, true, false, true);
			this._navalAgentsLogic.UpdateTeamAgentsData();
			MBMusicManager.Current.StartTheme(10243, 0.5f, false);
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x0000BE14 File Offset: 0x0000A014
		public override void OnMissionTick(float dt)
		{
			if (!this._isPhaseOneInitialized)
			{
				this.TickPhaseOneInitialization();
			}
			if (this._shouldStartPhaseTwo && !this._isPhaseTwoInitialized)
			{
				this.TickPhaseTwoInitialization();
			}
			if (this._isPhaseOneInitialized && !this._isPhaseTwoInitialized)
			{
				this.TickPhaseOneLogic(dt);
			}
			if (this._isPhaseTwoInitialized)
			{
				this.TickPhaseTwoLogic(dt);
			}
			if (this._isPhaseOneInitialized && this.IsStartedFromCheckpoint && !this._isPhaseTwoInitialized)
			{
				Agent.Main.Controller = 2;
				this._shouldStartPhaseTwo = true;
			}
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x0000BE98 File Offset: 0x0000A098
		private void TickPhaseOneInitialization()
		{
			this._currentPhaseOneInitializationTick++;
			if (this._currentPhaseOneInitializationTick == 1)
			{
				this.UpdateEntityReferences();
				GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("sp_wind");
				if (gameEntity != null)
				{
					this.SetWindStrengthAndDirection(gameEntity.GetGlobalFrame().rotation.f.AsVec2.Normalized(), gameEntity.GetGlobalScale().z);
				}
				base.Mission.Scene.SetWaterStrength(2f);
				this.SpawnPlayerShip();
				this.SpawnEnemyShips();
				this.ConnectEnemyShips();
				using (List<MissionShip>.Enumerator enumerator = this._enemyMissionShipsOrdered.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Ship ship;
						if ((ship = enumerator.Current.ShipOrigin as Ship) != null)
						{
							ship.IsInvulnerable = true;
						}
					}
				}
				base.Mission.PlayerTeam.SetPlayerRole(true, true);
				this.UpdateEntityReferences();
			}
			if (this._currentPhaseOneInitializationTick == 2)
			{
				this.SpawnPlayerShipAgents();
				this.SpawnPlayer();
				for (int i = 0; i < this._enemyMissionShipsOrdered.Count; i++)
				{
					ValueTuple<string, int>[] array = this._initialEnemyShipAgents[i];
					this.SpawnEnemyShipAgents(this._enemyMissionShipsOrdered[i], array);
				}
				this._navalAgentsLogic.AssignCaptainToShipForDeploymentMode(Agent.Main, this._playerShip, this._playerShip);
				this._navalShipsLogic.SetDeploymentMode(true);
				this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(0);
				this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(2);
				if (Agent.Main != null && Agent.Main.IsUsingGameObject)
				{
					Agent.Main.StopUsingGameObject(true, 1);
				}
				this._navalShipsLogic.SetDeploymentMode(false);
				Mission.Current.OnDeploymentFinished();
				Mission.Current.OnAfterDeploymentFinished();
				foreach (MissionShip missionShip in this._enemyMissionShipsOrdered)
				{
					missionShip.SetAnchor(true, true, 1f);
					missionShip.BlockConnection();
					if (missionShip.ShipSiegeWeapon != null)
					{
						this._cachedMangonelAgents[missionShip.ShipSiegeWeapon] = missionShip.ShipSiegeWeapon.PilotAgent;
						missionShip.ShipSiegeWeapon.PilotAgent.StopUsingGameObject(true, 1);
						missionShip.ShipSiegeWeapon.SetIsDisabledForAI(true);
					}
				}
				this._playerShip.OnSetRangedWeaponControlMode(true);
				this._isPhaseOneInitialized = true;
			}
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x0000C12C File Offset: 0x0000A32C
		private void TickPhaseOneLogic(float dt)
		{
			if (this._playerShip.IsSinking)
			{
				this.OnMissionFailed();
				return;
			}
			if (this._playerShip.SailHitPoints <= 0f)
			{
				this._playerShipSailDestroyedLine.TryPlayLine();
			}
			if (this._playerShip.HitPoints <= this._playerShip.MaxHealth * 0.65f)
			{
				this._playerShipLowHpLine.TryPlayLine();
			}
			if (this._enemySiegeWeaponDestructables.Count == 0)
			{
				return;
			}
			if (this._playerShip.ShipSiegeWeapon.DestructionComponent.IsDestroyed || this._playerShip.ShipSiegeWeapon.AmmoCount == 0)
			{
				this._playerLoseRemainingTime -= dt;
				if (this._playerLoseRemainingTime <= 0f)
				{
					this.OnMissionFailed();
					return;
				}
			}
			bool flag = this._playerShip.GameEntity.GlobalPosition.Distance(this._trailingTargetObject.GlobalPosition) < 15f;
			foreach (MissionShip missionShip in this._enemyMissionShipsOrdered)
			{
				if (Agent.Main != null && missionShip.GetIsAgentOnShip(Agent.Main, false))
				{
					this.OnMissionFailed();
				}
				if (this._playerShip.GameEntity.GlobalPosition.DistanceSquared(missionShip.GameEntity.GlobalPosition) <= 10000f)
				{
					this._playerShipTooCloseLine.TryPlayLine();
				}
				if (missionShip.ShipSiegeWeapon != null && !missionShip.ShipSiegeWeapon.IsDisabledForAI)
				{
					RangedSiegeWeapon shipSiegeWeapon = missionShip.ShipSiegeWeapon;
					if (!shipSiegeWeapon.IsDestroyed)
					{
						shipSiegeWeapon.GameEntity.SetContourColor(new uint?(new Color(1f, 0.68f, 0.44f, (MathF.Sin(base.Mission.CurrentTime * 2f) + 1f) / 2f).ToUnsignedInteger()), true);
					}
					if (flag && !shipSiegeWeapon.PilotStandingPoint.IsDisabled && shipSiegeWeapon.PilotAgent != null && shipSiegeWeapon.CanShootAtPoint(this._trailingTargetObject.GlobalPosition))
					{
						this._playerShipStandingStillLine.TryPlayLine();
					}
					if (!shipSiegeWeapon.IsDestroyed && !shipSiegeWeapon.PilotStandingPoint.IsDisabled && (shipSiegeWeapon.PilotStandingPoint.UserAgent == null || !shipSiegeWeapon.PilotStandingPoint.UserAgent.IsActive()) && !shipSiegeWeapon.PilotStandingPoint.HasAIMovingTo && shipSiegeWeapon.State == null)
					{
						float num = 1000000f;
						Agent agent = null;
						foreach (Agent agent2 in this._navalAgentsLogic.GetActiveAgentsOfShip(missionShip))
						{
							if (!agent2.IsHero && agent2.Detachment == null)
							{
								float num2 = agent2.Position.DistanceSquared(shipSiegeWeapon.GameEntity.GlobalPosition);
								if (num2 < num)
								{
									num = num2;
									agent = agent2;
								}
							}
						}
						if (agent != null)
						{
							shipSiegeWeapon.AddAgentAtSlotIndex(agent, shipSiegeWeapon.PilotStandingPointSlotIndex);
						}
					}
				}
			}
			RangedSiegeWeapon shipSiegeWeapon2 = this._playerShip.ShipSiegeWeapon;
			if (shipSiegeWeapon2 != null)
			{
				if ((float)shipSiegeWeapon2.AmmoCount <= (float)this._enemySiegeWeaponDestructables.Count * 3f)
				{
					this._playerShipRemainingAmmoLine.TryPlayLine();
				}
				if (shipSiegeWeapon2.AmmoCount == 0)
				{
					this.OnMissionFailed();
				}
				if (!shipSiegeWeapon2.IsDestroyed && (shipSiegeWeapon2.PilotStandingPoint.UserAgent == null || !shipSiegeWeapon2.PilotStandingPoint.UserAgent.IsActive()) && !shipSiegeWeapon2.PilotStandingPoint.HasAIMovingTo && shipSiegeWeapon2.State == null)
				{
					float num3 = 1000000f;
					Agent agent3 = null;
					foreach (Agent agent4 in this._navalAgentsLogic.GetActiveAgentsOfShip(this._playerShip))
					{
						if (!agent4.IsHero && agent4.Detachment == null)
						{
							float num4 = agent4.Position.DistanceSquared(shipSiegeWeapon2.GameEntity.GlobalPosition);
							if (num4 < num3)
							{
								num3 = num4;
								agent3 = agent4;
							}
						}
					}
					if (agent3 != null)
					{
						shipSiegeWeapon2.AddAgentAtSlotIndex(agent3, shipSiegeWeapon2.PilotStandingPointSlotIndex);
					}
				}
			}
			this._playerShipTargetObjectTrailController.RecordPosition(this._playerShip.GameEntity.GlobalPosition, base.Mission.CurrentTime);
			this._trailingTargetObject.WeakEntity.SetGlobalPosition(this._playerShipTargetObjectTrailController.GetTrailEndPosition(base.Mission.CurrentTime));
			if (flag)
			{
				this._playerShipTargetObject.GameEntity.SetGlobalPosition(this._playerShip.GameEntity.GlobalPosition);
				return;
			}
			if (this._lastRandomAttackPointPickTime + 1f < base.Mission.CurrentTime)
			{
				this._randomAttackPoint = this.GetRandomPointOnCircle(Vec3.Zero, 15f);
				this._lastRandomAttackPointPickTime = base.Mission.CurrentTime;
			}
			Vec3 vec = this._playerShip.GameEntity.GlobalPosition + this._randomAttackPoint;
			this._playerShipTargetObject.GameEntity.SetGlobalPosition(vec);
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x0000C6BC File Offset: 0x0000A8BC
		private void TickPhaseTwoLogic(float dt)
		{
			if (this._boardFloatingFortressObjective.IsCompleted && this._defeatTheEnemyCrewObjective == null)
			{
				this._defeatTheEnemyCrewObjective = new DefeatTheEnemyCrewObjective(base.Mission);
				this._missionObjectiveLogic.StartObjective(this._defeatTheEnemyCrewObjective);
			}
			for (int i = 0; i < this._playerAllyShipAnchorState.Count; i++)
			{
				ValueTuple<MissionShip, bool> valueTuple = this._playerAllyShipAnchorState[i];
				Vec3 globalPosition = valueTuple.Item1.GameEntity.GlobalPosition;
				if (valueTuple.Item2)
				{
					if (valueTuple.Item1.GetIsConnectedToEnemy() && valueTuple.Item1.Physics.IsAnchored)
					{
						valueTuple.Item1.SetAnchor(false, false, 1f);
					}
				}
				else if (valueTuple.Item1.Physics.IsAnchored)
				{
					if (valueTuple.Item1.Physics.AnchorGlobalFrame.origin.DistanceSquared(globalPosition) < 200f)
					{
						valueTuple.Item1.SetAnchor(true, true, 1f);
						valueTuple.Item2 = true;
						this._playerAllyShipAnchorState[i] = valueTuple;
					}
				}
				else
				{
					if (valueTuple.Item1.ShipOrder.TargetShip == null)
					{
						MissionShip missionShip = Extensions.MinBy<MissionShip, float>(this._enemyMissionShipsOrdered, (MissionShip x) => x.GameEntity.GlobalPosition.DistanceSquared(valueTuple.Item1.GameEntity.GlobalPosition));
						valueTuple.Item1.ShipOrder.SetShipEngageOrder(missionShip);
						valueTuple.Item1.ShipOrder.SetBoardingTargetShip(missionShip);
					}
					Vec3 globalPosition2 = valueTuple.Item1.ShipOrder.TargetShip.GameEntity.GlobalPosition;
					if (globalPosition.DistanceSquared(globalPosition2) < 900f)
					{
						Vec3 vec = (globalPosition2 - globalPosition).NormalizedCopy();
						valueTuple.Item1.SetAnchor(true, false, 1f);
						MissionShip item = valueTuple.Item1;
						Vec2 asVec = globalPosition2.AsVec2;
						Vec2 asVec2 = vec.AsVec2;
						item.SetAnchorFrame(in asVec, in asVec2, 0.2f);
					}
				}
			}
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x0000C90C File Offset: 0x0000AB0C
		private void SpawnPlayerShip()
		{
			Formation formation = Mission.GetTeam(0).GetFormation(0);
			Ship ship = new Ship(Campaign.Current.ObjectManager.GetObject<ShipHull>("naval_storyline_quest_4_player_medit_ship"))
			{
				IsTradeable = false,
				IsUsedByQuest = true,
				Owner = PartyBase.MainParty
			};
			foreach (KeyValuePair<string, string> keyValuePair in this._playerShipUpgradePieces)
			{
				ship.EquipUpgradePiece(keyValuePair.Key, Campaign.Current.ObjectManager.GetObject<ShipUpgradePiece>(keyValuePair.Value));
			}
			this._playerShip = this.CreateMissionShip(ship, this.IsStartedFromCheckpoint ? "sp_player_phase_two_start" : "sp_player_ship", formation);
			this._playerShip.SetShipOrderActive(false);
			this._trailingTargetObject = GameEntity.CreateEmpty(base.Mission.Scene, true, true, true);
			this._playerShipTargetObject = MBExtensions.GetFirstScriptInFamilyDescending<ShipTargetMissionObject>(this._playerShip.GameEntity);
			((ShipBallistaAI)this._playerShip.ShipSiegeWeapon.Ai).SetCanAiUpdateAim(false);
			this._playerShip.ShipSiegeWeapon.SetStartAmmo(30);
		}

		// Token: 0x060001EA RID: 490 RVA: 0x0000CA48 File Offset: 0x0000AC48
		private void TickPhaseTwoInitialization()
		{
			this._currentPhaseTwoInitializationTick++;
			if (this._currentPhaseTwoInitializationTick == 1)
			{
				if (!this.IsStartedFromCheckpoint)
				{
					InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=BWSp3Uyj}Checkpoint reached.", null).ToString(), new Color(0f, 1f, 0f, 1f)));
					GameEntity gameEntity = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("sp_player_phase_two_start"));
					this._navalShipsLogic.TeleportShip(this._playerShip, gameEntity.GetGlobalFrame(), false, false, true);
				}
				((ShipBallistaAI)this._playerShip.ShipSiegeWeapon.Ai).SetCanAiUpdateAim(true);
				using (List<MissionShip>.Enumerator enumerator = this._enemyMissionShipsOrdered.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Ship ship;
						if ((ship = enumerator.Current.ShipOrigin as Ship) != null)
						{
							ship.IsInvulnerable = false;
						}
					}
				}
				this.SpawnAllyShips();
				if (Agent.Main.CurrentlyUsedGameObject != null)
				{
					Agent.Main.StopUsingGameObject(true, 1);
				}
				this._playerShip.SetShipOrderActive(true);
			}
			if (this._currentPhaseTwoInitializationTick == 2)
			{
				for (int i = 0; i < this._playerAllyMissionShips.Count; i++)
				{
					this.SpawnAllyShipAgents(this._playerAllyMissionShips[i], this._allyShipAgents[i]);
				}
				for (int j = 0; j < this._enemyMissionShipsOrdered.Count; j++)
				{
					ValueTuple<string, int>[] array = this._reinforcementEnemyShipAgents[j];
					this.SpawnEnemyShipAgents(this._enemyMissionShipsOrdered[j], array);
				}
				foreach (MissionShip missionShip in this._enemyMissionShipsOrdered)
				{
					missionShip.ResetConnectionBlock();
					missionShip.ShipOrder.SetOrderOarsmenLevel(0);
					missionShip.ShipOrder.SetCutLoose(false);
				}
				List<MissionShip> list = this._enemyMissionShipsOrdered.ToList<MissionShip>();
				using (List<MissionShip>.Enumerator enumerator = this._playerAllyMissionShips.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MissionShip playerAllyMissionShip = enumerator.Current;
						MissionShip missionShip2 = Extensions.MinBy<MissionShip, float>(list, (MissionShip x) => x.GameEntity.GlobalPosition.DistanceSquared(playerAllyMissionShip.GameEntity.GlobalPosition));
						list.Remove(missionShip2);
						playerAllyMissionShip.ShipOrder.SetShipEngageOrder(missionShip2);
						playerAllyMissionShip.ShipOrder.SetBoardingTargetShip(missionShip2);
					}
				}
				this._navalAgentsLogic.SetDeploymentMode(true);
				this._navalShipsLogic.SetDeploymentMode(true);
				this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(0);
				this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(2);
				this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(1);
				this._navalAgentsLogic.SetDeploymentMode(false);
				this._navalShipsLogic.SetDeploymentMode(false);
				CampaignInformationManager.ClearAllDialogNotifications(false);
				this._playerTookAllMangonelsDownLine.TryPlayLine();
				this._boardFloatingFortressObjective = new BoardFloatingFortressObjective(base.Mission, this._playerShip, this._enemyMissionShipsOrdered);
				this._missionObjectiveLogic.StartObjective(this._boardFloatingFortressObjective);
				this._isPhaseTwoInitialized = true;
			}
			Agent.Main.Health = Agent.Main.HealthLimit;
		}

		// Token: 0x060001EB RID: 491 RVA: 0x0000CDA8 File Offset: 0x0000AFA8
		private void SpawnAllyShips()
		{
			List<Formation> list = Mission.GetTeam(1).FormationsIncludingEmpty.Where<Formation>((Formation x) => x != this._playerShip.Formation).ToList<Formation>();
			for (int i = 0; i < this._allyShipHulls.Count; i++)
			{
				ShipHull hull = Campaign.Current.ObjectManager.GetObject<ShipHull>(this._allyShipHulls[i]);
				Ship ship;
				if ((ship = PartyBase.MainParty.Ships.FirstOrDefault<Ship>((Ship x) => x.ShipHull == hull)) == null)
				{
					Ship ship2 = new Ship(hull);
					ship2.IsTradeable = false;
					ship2.IsUsedByQuest = true;
					ship = ship2;
					ship2.Owner = PartyBase.MainParty;
				}
				Ship ship3 = ship;
				foreach (ValueTuple<string, string> valueTuple in FloatingFortressSetPieceBattleMissionController.AllyShipUpgrades[i])
				{
					string item = valueTuple.Item1;
					string item2 = valueTuple.Item2;
					if (ship3.HasSlot(item))
					{
						ship3.EquipUpgradePiece(item, MBObjectManager.Instance.GetObject<ShipUpgradePiece>(item2));
					}
				}
				ship3.ChangeFigurehead(this._allyShipFigureheads[i]);
				string allySpawnPoint = FloatingFortressSetPieceBattleMissionController.GetAllySpawnPoint(i);
				MissionShip missionShip = this.CreateMissionShip(ship3, allySpawnPoint, list[i]);
				GameEntity gameEntity = Mission.Current.Scene.FindEntityWithTag(allySpawnPoint);
				this._navalShipsLogic.TeleportShip(missionShip, gameEntity.GetGlobalFrame(), false, false, true);
				this._playerAllyMissionShips.Add(missionShip);
				this._playerAllyShipAnchorState.Add(new ValueTuple<MissionShip, bool>(missionShip, false));
			}
			foreach (MissionShip missionShip2 in this._playerAllyMissionShips)
			{
				missionShip2.OnDeploymentFinished();
			}
		}

		// Token: 0x060001EC RID: 492 RVA: 0x0000CF70 File Offset: 0x0000B170
		private void OnEnemyShipBallistaDestroyed(DestructableComponent target, Agent attackerAgent, in MissionWeapon weapon, ScriptComponentBehavior attackerScriptComponentBehavior, int inflictedDamage)
		{
			if (this.IsPhaseOneCompleted)
			{
				return;
			}
			this._enemySiegeWeaponDestructables.Remove(target);
			this._playerTookMangonelDownLine.TryPlayLine();
			target.GameEntity.SetContourColor(null, true);
			if (this._enemySiegeWeaponDestructables.Count == 0)
			{
				this.IsPhaseOneCompleted = true;
			}
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0000CFCC File Offset: 0x0000B1CC
		private void OnEnemyShipBallistaCoverDestroyed(DestructableComponent target, Agent attackerAgent, in MissionWeapon weapon, ScriptComponentBehavior attackerScriptComponentBehavior, int inflictedDamage)
		{
			DestructableComponent destructableComponent = this._enemySiegeWeaponByCover[target];
			if (!destructableComponent.IsDestroyed)
			{
				int internalValue = (int)Game.Current.ObjectManager.GetObject<ItemObject>("ballista_projectile").Id.InternalValue;
				DestructableComponent destructableComponent2 = destructableComponent;
				Agent main = Agent.Main;
				int num = 10000;
				Vec3 globalPosition = destructableComponent.GameEntity.GlobalPosition;
				Vec3 one = Vec3.One;
				MissionWeapon missionWeapon = new MissionWeapon(ItemObject.GetItemFromWeaponKind(internalValue), null, null);
				destructableComponent2.TriggerOnHit(main, num, globalPosition, one, ref missionWeapon, -1, null);
			}
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000D048 File Offset: 0x0000B248
		public override void OnBehaviorInitialize()
		{
			if (!SailWindProfile.IsSailWindProfileInitialized)
			{
				SailWindProfile.InitializeProfile();
			}
			Team team = Mission.GetTeam(0);
			base.Mission.Teams.Add(team.Side, team.Color, team.Color2, team.Banner, true, false, true);
		}

		// Token: 0x060001EF RID: 495 RVA: 0x0000D094 File Offset: 0x0000B294
		private void UpdateEntityReferences()
		{
			base.Mission.Scene.GetEntities(ref this._entities);
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0000D0AC File Offset: 0x0000B2AC
		private MissionShip CreateMissionShip(Ship ship, string spawnPointId, Formation formation)
		{
			NavalShipsLogic missionBehavior = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			GameEntity gameEntity = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag(spawnPointId));
			MatrixFrame globalFrame = gameEntity.GetGlobalFrame();
			float waterLevelAtPosition = Mission.Current.Scene.GetWaterLevelAtPosition(gameEntity.GlobalPosition.AsVec2, true, false);
			globalFrame.origin = new Vec3(gameEntity.GlobalPosition.x, gameEntity.GlobalPosition.y, waterLevelAtPosition, -1f);
			MissionShip missionShip = missionBehavior.SpawnShip(ship, in globalFrame, formation.Team, formation, false, 8, true);
			missionShip.ShipOrder.FormationJoinShip(formation);
			return missionShip;
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x0000D158 File Offset: 0x0000B358
		private void SpawnEnemyShips()
		{
			MBList<Formation> formationsIncludingEmpty = Mission.GetTeam(2).FormationsIncludingEmpty;
			this._enemyMissionShipsOrdered = new MBList<MissionShip>();
			for (int i = 0; i < this._enemyShipHulls.Length; i++)
			{
				ValueTuple<string, string> valueTuple = this._enemyShipHulls[i];
				string item = valueTuple.Item1;
				string item2 = valueTuple.Item2;
				ShipHull shipHullObject = Campaign.Current.ObjectManager.GetObject<ShipHull>(item);
				Ship ship;
				if ((ship = MapEvent.PlayerMapEvent.GetLeaderParty(Mission.Current.PlayerEnemyTeam.Side).Ships.FirstOrDefault<Ship>((Ship x) => x.ShipHull == shipHullObject)) == null)
				{
					Ship ship2 = new Ship(shipHullObject);
					ship2.IsTradeable = false;
					ship2.IsUsedByQuest = true;
					ship = ship2;
					ship2.Owner = MapEvent.PlayerMapEvent.GetLeaderParty(Mission.Current.PlayerEnemyTeam.Side);
				}
				Ship ship3 = ship;
				if (ship3.HasSlot("fore"))
				{
					bool flag = !this.IsStartedFromCheckpoint && this._enemyShipsToAddBallista.ContainsKey(i + 1);
					ship3.EquipUpgradePiece("fore", flag ? Campaign.Current.ObjectManager.GetObject<ShipUpgradePiece>(this._enemyShipsToAddBallista[i + 1]) : null);
				}
				MissionShip missionShip = this.CreateMissionShip(ship3, item2, formationsIncludingEmpty[i]);
				GameEntity gameEntity = Mission.Current.Scene.FindEntityWithTag(item2);
				missionShip.SetShipOrderActive(false);
				missionShip.ShipOrder.SetOrderOarsmenLevel(0);
				missionShip.SetCustomSailSetting(true, SailInput.Raised);
				missionShip.SetController(ShipControllerType.None, false);
				missionShip.ShipControllerMachine.PilotStandingPoint.SetDisabled(false);
				missionShip.SetCanBeTakenOver(false);
				this._navalShipsLogic.TeleportShip(missionShip, gameEntity.GetGlobalFrame(), false, true, true);
				if (missionShip.ShipSiegeWeapon != null)
				{
					this._enemySiegeWeaponDestructables.Add(missionShip.ShipSiegeWeapon.DestructionComponent);
				}
				this._enemyMissionShipsOrdered.Add(missionShip);
			}
			foreach (DestructableComponent destructableComponent in this._enemySiegeWeaponDestructables)
			{
				destructableComponent.OnDestroyed += new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnEnemyShipBallistaDestroyed);
				DestructableComponent firstScriptOfType = destructableComponent.GameEntity.GetFirstChildEntityWithTag("ballista_cover").GetFirstScriptOfType<DestructableComponent>();
				if (firstScriptOfType != null)
				{
					this._enemySiegeWeaponByCover.Add(firstScriptOfType, destructableComponent);
					firstScriptOfType.OnDestroyed += new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnEnemyShipBallistaCoverDestroyed);
				}
			}
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x0000D3D8 File Offset: 0x0000B5D8
		private void ConnectEnemyShips()
		{
			for (int i = 0; i < this._enemyMissionShipsOrdered.Count; i++)
			{
				int num = i + 1;
				if (i == this._enemyMissionShipsOrdered.Count - 1)
				{
					num = 0;
				}
				this.TryMaintainConnection(this._enemyMissionShipsOrdered[i], this._enemyMissionShipsOrdered[num]);
			}
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x0000D430 File Offset: 0x0000B630
		private void TryMaintainConnection(MissionShip ship, MissionShip otherShip)
		{
			int num = 0;
			foreach (ShipAttachmentMachine shipAttachmentMachine in ship.AttachmentMachines)
			{
				if (shipAttachmentMachine.CurrentAttachment != null && shipAttachmentMachine.CurrentAttachment.AttachmentTarget.OwnerShip == otherShip)
				{
					num++;
				}
			}
			if (num >= 1)
			{
				return;
			}
			Vec3 fortressCenter = Vec3.Zero;
			foreach (MissionShip missionShip in this._enemyMissionShipsOrdered)
			{
				fortressCenter += missionShip.GameEntity.GlobalPosition;
			}
			fortressCenter /= (float)this._enemyMissionShipsOrdered.Count;
			foreach (ShipAttachmentMachine shipAttachmentMachine2 in ship.AttachmentMachines.OrderBy<ShipAttachmentMachine, float>((ShipAttachmentMachine x) => x.GameEntity.GlobalPosition.DistanceSquared(fortressCenter)))
			{
				if (shipAttachmentMachine2.CurrentAttachment == null)
				{
					shipAttachmentMachine2.SetPreferredTargetShip(otherShip);
					if (shipAttachmentMachine2.LinkedAttachmentPointMachine.CurrentAttachment == null)
					{
						shipAttachmentMachine2.SetCanConnectToFriends(true);
						ShipAttachmentPointMachine bestEnemyAttachment = shipAttachmentMachine2.GetBestEnemyAttachment(true, true);
						if (bestEnemyAttachment != null)
						{
							shipAttachmentMachine2.ConnectWithAttachmentPointMachine(bestEnemyAttachment, true, true, false);
							num++;
							if (num >= 1)
							{
								break;
							}
						}
					}
				}
			}
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x0000D5C8 File Offset: 0x0000B7C8
		private void SpawnPlayer()
		{
			this._navalAgentsLogic.AddReservedTroopToShip(new PartyAgentOrigin(PartyBase.MainParty, CharacterObject.PlayerCharacter, -1, default(UniqueTroopDescriptor), false, false), this._playerShip);
			this._navalAgentsLogic.SpawnNextBatch(0, false, null);
			Agent main = Agent.Main;
			this._navalAgentsLogic.AssignCaptainToShipForDeploymentMode(main, this._playerShip, null);
			Mission.Current.PlayerTeam.PlayerOrderController.Owner = main;
			base.Mission.PlayerTeam.GetFormation(0).PlayerOwner = main;
			main.OnAgentHealthChanged += new Agent.OnAgentHealthChangedDelegate(this.OnMainAgentHealthChanged);
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000D668 File Offset: 0x0000B868
		private void SpawnPlayerShipAgents()
		{
			List<CharacterObject> list = new List<CharacterObject>();
			foreach (ValueTuple<string, int> valueTuple in this._playerShipTroops)
			{
				string item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				list.AddRange(Enumerable.Repeat<CharacterObject>(Campaign.Current.ObjectManager.GetObject<CharacterObject>(item), item2));
			}
			NavalAgentsLogic missionBehavior = base.Mission.GetMissionBehavior<NavalAgentsLogic>();
			missionBehavior.SetDesiredTroopCountOfShip(this._playerShip, list.Count + 1);
			int deckFrameCount = this._playerShip.DeckFrameCount;
			Extensions.Shuffle<CharacterObject>(list);
			int num = 0;
			while (num < deckFrameCount && num < list.Count)
			{
				MatrixFrame nextOuterInnerSpawnGlobalFrame = this._playerShip.GetNextOuterInnerSpawnGlobalFrame();
				CharacterObject characterObject = list.ElementAtOrDefault<CharacterObject>(num);
				if (characterObject == null)
				{
					return;
				}
				AgentBuildData agentBuildData = new AgentBuildData(characterObject).TroopOrigin(new SimpleAgentOrigin(characterObject, -1, null, default(UniqueTroopDescriptor))).Team(base.Mission.PlayerTeam).Formation(this._playerShip.Formation)
					.InitialPosition(ref nextOuterInnerSpawnGlobalFrame.origin);
				Vec2 vec = nextOuterInnerSpawnGlobalFrame.rotation.f.AsVec2;
				vec = vec.Normalized();
				AgentBuildData agentBuildData2 = agentBuildData.InitialDirection(ref vec).NoHorses(true).NoWeapons(false);
				Agent agent = Mission.Current.SpawnAgent(agentBuildData2, false);
				agent.SetAlarmState(3);
				missionBehavior.AddAgentToShip(agent, this._playerShip);
				num++;
			}
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x0000D7FC File Offset: 0x0000B9FC
		private void SetWindStrengthAndDirection(Vec2 direction, float strength)
		{
			Scene scene = Mission.Current.Scene;
			Vec2 vec = strength * direction;
			scene.SetGlobalWindVelocity(ref vec);
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x0000D824 File Offset: 0x0000BA24
		private void SpawnEnemyShipAgents(MissionShip ship, ValueTuple<string, int>[] source)
		{
			NavalAgentsLogic missionBehavior = base.Mission.GetMissionBehavior<NavalAgentsLogic>();
			missionBehavior.SetDesiredTroopCountOfShip(ship, source.Sum<ValueTuple<string, int>>((ValueTuple<string, int> x) => x.Item2));
			List<CharacterObject> list = new List<CharacterObject>();
			foreach (ValueTuple<string, int> valueTuple in source)
			{
				string item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				list.AddRange(Enumerable.Repeat<CharacterObject>(Campaign.Current.ObjectManager.GetObject<CharacterObject>(item), item2));
			}
			Extensions.Shuffle<CharacterObject>(list);
			int deckFrameCount = ship.DeckFrameCount;
			int num = 0;
			while (num < deckFrameCount && num < list.Count)
			{
				CharacterObject characterObject = list[num];
				if (characterObject == null)
				{
					return;
				}
				MatrixFrame nextOuterInnerSpawnGlobalFrame = ship.GetNextOuterInnerSpawnGlobalFrame();
				AgentBuildData agentBuildData = new AgentBuildData(characterObject).TroopOrigin(new SimpleAgentOrigin(characterObject, -1, null, default(UniqueTroopDescriptor))).Team(base.Mission.PlayerEnemyTeam).InitialPosition(ref nextOuterInnerSpawnGlobalFrame.origin);
				Vec2 vec = nextOuterInnerSpawnGlobalFrame.rotation.f.AsVec2;
				vec = vec.Normalized();
				AgentBuildData agentBuildData2 = agentBuildData.InitialDirection(ref vec).Formation(ship.Formation).NoHorses(true)
					.NoWeapons(false);
				Agent agent = Mission.Current.SpawnAgent(agentBuildData2, false);
				agent.SetAlarmState(3);
				missionBehavior.AddAgentToShip(agent, ship);
				num++;
			}
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x0000D998 File Offset: 0x0000BB98
		private void SpawnAllyShipAgents(MissionShip ship, ValueTuple<string, int>[] source)
		{
			NavalAgentsLogic missionBehavior = base.Mission.GetMissionBehavior<NavalAgentsLogic>();
			missionBehavior.SetDesiredTroopCountOfShip(ship, source.Sum<ValueTuple<string, int>>((ValueTuple<string, int> x) => x.Item2));
			List<CharacterObject> list = new List<CharacterObject>();
			foreach (ValueTuple<string, int> valueTuple in source)
			{
				string item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				list.AddRange(Enumerable.Repeat<CharacterObject>(Campaign.Current.ObjectManager.GetObject<CharacterObject>(item), item2));
			}
			Extensions.Shuffle<CharacterObject>(list);
			int deckFrameCount = ship.DeckFrameCount;
			int num = 0;
			while (num < deckFrameCount && num < list.Count)
			{
				MatrixFrame nextOuterInnerSpawnGlobalFrame = ship.GetNextOuterInnerSpawnGlobalFrame();
				CharacterObject characterObject = list[num];
				if (characterObject == null)
				{
					return;
				}
				AgentBuildData agentBuildData = new AgentBuildData(characterObject).TroopOrigin(new SimpleAgentOrigin(characterObject, -1, null, default(UniqueTroopDescriptor))).Team(base.Mission.PlayerAllyTeam).InitialPosition(ref nextOuterInnerSpawnGlobalFrame.origin);
				Vec2 vec = nextOuterInnerSpawnGlobalFrame.rotation.f.AsVec2;
				vec = vec.Normalized();
				AgentBuildData agentBuildData2 = agentBuildData.InitialDirection(ref vec).NoHorses(true).NoWeapons(false);
				Agent agent = Mission.Current.SpawnAgent(agentBuildData2, false);
				agent.SetAlarmState(3);
				missionBehavior.AddAgentToShip(agent, ship);
				num++;
			}
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0000DB00 File Offset: 0x0000BD00
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if ((Extensions.IsEmpty<Agent>(base.Mission.PlayerTeam.ActiveAgents) || (affectedAgent.IsMainAgent && !this._shouldStartPhaseTwo)) && !this._isMissionSuccessful)
			{
				this.OnMissionFailed();
				return;
			}
			if (Extensions.IsEmpty<Agent>(base.Mission.PlayerEnemyTeam.ActiveAgents) && !this._isMissionFailed && !this._isMissionSuccessful)
			{
				this.OnMissionSucceeded();
			}
		}

		// Token: 0x060001FA RID: 506 RVA: 0x0000DB70 File Offset: 0x0000BD70
		private void OnMainAgentHealthChanged(Agent agent, float oldHealth, float newHealth)
		{
			if (!this._shouldStartPhaseTwo && newHealth <= 0f)
			{
				this.OnMissionFailed();
			}
		}

		// Token: 0x060001FB RID: 507 RVA: 0x0000DB88 File Offset: 0x0000BD88
		private void OnMissionSucceeded()
		{
			if (this._isMissionSuccessful || this._isMissionFailed || Mission.Current.CurrentState == 3)
			{
				return;
			}
			this._isMissionSuccessful = true;
			PlayerEncounter.CampaignBattleResult = CampaignBattleResult.GetResult(2, false);
		}

		// Token: 0x060001FC RID: 508 RVA: 0x0000DBBB File Offset: 0x0000BDBB
		private void OnMissionFailed()
		{
			if (this._isMissionFailed || this._isMissionSuccessful || Mission.Current.CurrentState == 3)
			{
				return;
			}
			this._isMissionFailed = true;
			PlayerEncounter.CampaignBattleResult = CampaignBattleResult.GetResult(1, false);
			base.Mission.EndMission();
		}

		// Token: 0x060001FD RID: 509 RVA: 0x0000DBFC File Offset: 0x0000BDFC
		public override bool MissionEnded(ref MissionResult missionResult)
		{
			bool flag = false;
			if (this._isMissionSuccessful)
			{
				missionResult = MissionResult.CreateSuccessful(base.Mission, true);
				flag = true;
			}
			else if (this._isMissionFailed)
			{
				missionResult = MissionResult.CreateDefeated(base.Mission);
				flag = true;
			}
			MBMusicManager.Current.ForceStopThemeWithFadeOut();
			return flag;
		}

		// Token: 0x060001FE RID: 510 RVA: 0x0000DC48 File Offset: 0x0000BE48
		public void OnViewFadeOut(int reason)
		{
			switch (reason)
			{
			case 0:
				break;
			case 1:
			{
				this._playerShip.SetShipOrderActive(true);
				MBList<ShipMangonel> mblist = new MBList<ShipMangonel>();
				foreach (MissionShip missionShip in this._enemyMissionShipsOrdered)
				{
					if (missionShip.ShipSiegeWeapon != null)
					{
						mblist.Add(missionShip.ShipSiegeWeapon as ShipMangonel);
						missionShip.ShipSiegeWeapon.SetIsDisabledForAI(false);
						Agent agent;
						if (this._cachedMangonelAgents.TryGetValue(missionShip.ShipSiegeWeapon, out agent))
						{
							ModuleExtensions.StartUsingMachine(agent.Formation, missionShip.ShipSiegeWeapon, false);
							missionShip.ShipSiegeWeapon.AddAgentAtSlotIndex(agent, missionShip.ShipSiegeWeapon.PilotStandingPointSlotIndex);
						}
					}
				}
				this._missionObjectiveLogic.StartObjective(new DestroyMangonelsObjective(base.Mission, mblist));
				Agent.Main.Controller = 2;
				return;
			}
			case 2:
			{
				CaptureTheImperialMerchantPrusas captureTheImperialMerchantPrusas = Campaign.Current.QuestManager.Quests.FirstOrDefault<QuestBase>((QuestBase x) => x is CaptureTheImperialMerchantPrusas) as CaptureTheImperialMerchantPrusas;
				if (captureTheImperialMerchantPrusas != null)
				{
					captureTheImperialMerchantPrusas.OnCheckPointReached();
				}
				this._shouldStartPhaseTwo = true;
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000DD8C File Offset: 0x0000BF8C
		public override void OnRetreatMission()
		{
			this._isMissionFailed = true;
			PlayerEncounter.CampaignBattleResult = CampaignBattleResult.GetResult(1, false);
		}

		// Token: 0x06000200 RID: 512 RVA: 0x0000DDA4 File Offset: 0x0000BFA4
		private void OnShipHit(MissionShip ship, Agent attackerAgent, int damage, Vec3 impactPosition, Vec3 impactDirection, MissionWeapon weapon, int affectorWeaponSlotOrMissileIndex)
		{
			if (weapon.CurrentUsageItem == null)
			{
				return;
			}
			if (Extensions.HasAnyFlag<WeaponFlags>(weapon.CurrentUsageItem.WeaponFlags, 131072L) && ship == this._playerShip && !this._isPhaseTwoInitialized)
			{
				this._playerShipHitLine.TryPlayLine();
			}
		}

		// Token: 0x06000201 RID: 513 RVA: 0x0000DDF0 File Offset: 0x0000BFF0
		private void DestroyEnemyBallistas()
		{
			int internalValue = (int)Game.Current.ObjectManager.GetObject<ItemObject>("ballista_projectile").Id.InternalValue;
			for (int i = this._enemySiegeWeaponDestructables.Count - 1; i >= 0; i--)
			{
				DestructableComponent destructableComponent = this._enemySiegeWeaponDestructables[i];
				Agent main = Agent.Main;
				int num = 1000;
				Vec3 globalPosition = this._enemySiegeWeaponDestructables[i].GameEntity.GlobalPosition;
				Vec3 one = Vec3.One;
				MissionWeapon missionWeapon = new MissionWeapon(ItemObject.GetItemFromWeaponKind(internalValue), null, null);
				destructableComponent.TriggerOnHit(main, num, globalPosition, one, ref missionWeapon, -1, null);
			}
		}

		// Token: 0x06000202 RID: 514 RVA: 0x0000DE83 File Offset: 0x0000C083
		private static string GetAllySpawnPoint(int i)
		{
			return string.Format("sp_player_reinforcement_{0}", i + 1);
		}

		// Token: 0x06000203 RID: 515 RVA: 0x0000DE98 File Offset: 0x0000C098
		private Vec3 GetRandomPointOnCircle(Vec3 center, float radius)
		{
			float num = MBRandom.RandomFloat * 3.1415927f * 2f;
			float num2 = center.x + radius * MathF.Cos(num);
			float num3 = center.y + radius * MathF.Sin(num);
			return new Vec3(num2, num3, center.z, -1f);
		}

		// Token: 0x040000BD RID: 189
		private const float PlayerShipTargetingWarningDistance = 15f;

		// Token: 0x040000BE RID: 190
		private const float TimeToLoseAfterLastBallistaShot = 5f;

		// Token: 0x040000BF RID: 191
		private const float BallistaRandomAttackRadius = 15f;

		// Token: 0x040000C0 RID: 192
		private const float BallistaRandomAttackPointSelectionTime = 1f;

		// Token: 0x040000C1 RID: 193
		private const string PlayerPhaseOneSpawnPointTag = "sp_player_ship";

		// Token: 0x040000C2 RID: 194
		private const string PlayerPhaseTwoSpawnPointTag = "sp_player_phase_two_start";

		// Token: 0x040000C3 RID: 195
		private const float PlayerShipTooCloseThresholdDistanceSquared = 10000f;

		// Token: 0x040000C4 RID: 196
		private const float PlayerShipLowHpThresholdRatio = 0.65f;

		// Token: 0x040000C5 RID: 197
		private const float PlayerRemainingAmmoThresholdRatio = 3f;

		// Token: 0x040000C6 RID: 198
		private const float AllyShipAnchorFrameConnectionDistanceSquared = 900f;

		// Token: 0x040000C7 RID: 199
		private const string PlayerStartingShipHull = "naval_storyline_quest_4_player_medit_ship";

		// Token: 0x040000C8 RID: 200
		private const float AllyShipDistanceToSelfAnchor = 200f;

		// Token: 0x040000C9 RID: 201
		private const int PlayerBallistaStartingAmmo = 30;

		// Token: 0x040000CA RID: 202
		private static readonly List<ValueTuple<string, string>[]> AllyShipUpgrades = new List<ValueTuple<string, string>[]>
		{
			new ValueTuple<string, string>[]
			{
				new ValueTuple<string, string>("sail", "sails_lvl2"),
				new ValueTuple<string, string>("side", "side_northern_shields_lvl1")
			},
			new ValueTuple<string, string>[]
			{
				new ValueTuple<string, string>("sail", "sails_lvl3"),
				new ValueTuple<string, string>("side", "side_northern_shields_lvl2")
			},
			new ValueTuple<string, string>[]
			{
				new ValueTuple<string, string>("sail", "sails_lvl2"),
				new ValueTuple<string, string>("side", "side_northern_shields_lvl2")
			},
			new ValueTuple<string, string>[]
			{
				new ValueTuple<string, string>("sail", "sails_lvl3"),
				new ValueTuple<string, string>("side", "side_northern_shields_lvl3")
			}
		};

		// Token: 0x040000CB RID: 203
		private const int BridgesBetweenEnemyShips = 1;

		// Token: 0x040000CC RID: 204
		private readonly List<Figurehead> _allyShipFigureheads = new List<Figurehead>
		{
			DefaultFigureheads.Raven,
			DefaultFigureheads.Turtle,
			DefaultFigureheads.Boar,
			DefaultFigureheads.Dragon
		};

		// Token: 0x040000CD RID: 205
		private readonly Dictionary<string, string> _playerShipUpgradePieces = new Dictionary<string, string>
		{
			{ "fore", "fore_heavy_ballista_stone" },
			{ "aft", "" },
			{ "hull", "" },
			{ "deck", "" },
			{ "oars", "" },
			{ "sail", "sails_lvl3" },
			{ "roof", "roof_8" }
		};

		// Token: 0x040000CE RID: 206
		private readonly List<string> _allyShipHulls = new List<string> { "northern_medium_ship", "northern_medium_ship", "northern_light_ship", "northern_medium_ship" };

		// Token: 0x040000CF RID: 207
		private readonly List<ValueTuple<string, int>> _playerShipTroops = new List<ValueTuple<string, int>>
		{
			new ValueTuple<string, int>("skolderbrotva_tier_2", 2),
			new ValueTuple<string, int>("skolderbrotva_tier_3", 28)
		};

		// Token: 0x040000D0 RID: 208
		private readonly List<ValueTuple<string, int>[]> _allyShipAgents = new List<ValueTuple<string, int>[]>
		{
			new ValueTuple<string, int>[]
			{
				new ValueTuple<string, int>("skolderbrotva_tier_3", 40)
			},
			new ValueTuple<string, int>[]
			{
				new ValueTuple<string, int>("skolderbrotva_tier_3", 39)
			},
			new ValueTuple<string, int>[]
			{
				new ValueTuple<string, int>("skolderbrotva_tier_3", 16),
				new ValueTuple<string, int>("skolderbrotva_tier_2", 3)
			},
			new ValueTuple<string, int>[]
			{
				new ValueTuple<string, int>("gangradirs_kin_melee", 20),
				new ValueTuple<string, int>("gangradirs_kin_ranged", 20)
			}
		};

		// Token: 0x040000D1 RID: 209
		private readonly ValueTuple<string, string>[] _enemyShipHulls = new ValueTuple<string, string>[]
		{
			new ValueTuple<string, string>("naval_storyline_quest_4_boss_light_ship", "sp_enemy_ship_1"),
			new ValueTuple<string, string>("ship_storyline_quest_4_boss_cog_ship", "sp_enemy_ship_2"),
			new ValueTuple<string, string>("naval_storyline_quest_4_boss_light_ship", "sp_enemy_ship_3"),
			new ValueTuple<string, string>("naval_storyline_quest_4_boss_round_ship", "sp_enemy_ship_4"),
			new ValueTuple<string, string>("naval_storyline_quest_4_boss_light_ship", "sp_enemy_ship_5"),
			new ValueTuple<string, string>("ship_storyline_quest_4_boss_cog_ship", "sp_enemy_ship_7"),
			new ValueTuple<string, string>("naval_storyline_quest_4_boss_light_ship", "sp_enemy_ship_6"),
			new ValueTuple<string, string>("ship_storyline_quest_4_boss_cog_ship", "sp_enemy_ship_8")
		};

		// Token: 0x040000D2 RID: 210
		private readonly List<ValueTuple<string, int>[]> _initialEnemyShipAgents = new List<ValueTuple<string, int>[]>
		{
			new ValueTuple<string, int>[]
			{
				new ValueTuple<string, int>("sea_hounds_marksman", 5),
				new ValueTuple<string, int>("sea_hounds", 10)
			},
			new ValueTuple<string, int>[]
			{
				new ValueTuple<string, int>("sea_hounds_marksman", 2),
				new ValueTuple<string, int>("sea_hounds_pups", 9)
			},
			new ValueTuple<string, int>[]
			{
				new ValueTuple<string, int>("sea_hounds_marksman", 6),
				new ValueTuple<string, int>("sea_hounds_pups", 9)
			},
			new ValueTuple<string, int>[]
			{
				new ValueTuple<string, int>("sea_hounds_marksman", 9),
				new ValueTuple<string, int>("sea_hounds_pups", 14)
			},
			new ValueTuple<string, int>[]
			{
				new ValueTuple<string, int>("sea_hounds_marksman", 4),
				new ValueTuple<string, int>("sea_hounds", 11)
			},
			new ValueTuple<string, int>[]
			{
				new ValueTuple<string, int>("sea_hounds_marksman", 2),
				new ValueTuple<string, int>("sea_hounds_pups", 4)
			},
			new ValueTuple<string, int>[]
			{
				new ValueTuple<string, int>("sea_hounds_marksman", 3),
				new ValueTuple<string, int>("sea_hounds", 9)
			},
			new ValueTuple<string, int>[]
			{
				new ValueTuple<string, int>("sea_hounds_marksman", 3),
				new ValueTuple<string, int>("sea_hounds_pups", 6)
			}
		};

		// Token: 0x040000D3 RID: 211
		private readonly List<ValueTuple<string, int>[]> _reinforcementEnemyShipAgents = new List<ValueTuple<string, int>[]>
		{
			new ValueTuple<string, int>[]
			{
				new ValueTuple<string, int>("sea_hounds_marksman", 2),
				new ValueTuple<string, int>("sea_hounds", 2)
			},
			new ValueTuple<string, int>[]
			{
				new ValueTuple<string, int>("sea_hounds_marksman", 5),
				new ValueTuple<string, int>("sea_hounds_pups", 2)
			},
			new ValueTuple<string, int>[]
			{
				new ValueTuple<string, int>("sea_hounds_marksman", 1),
				new ValueTuple<string, int>("sea_hounds_pups", 3)
			},
			new ValueTuple<string, int>[]
			{
				new ValueTuple<string, int>("sea_hounds_marksman", 2),
				new ValueTuple<string, int>("sea_hounds_pups", 4)
			},
			new ValueTuple<string, int>[]
			{
				new ValueTuple<string, int>("sea_hounds_marksman", 6),
				new ValueTuple<string, int>("sea_hounds", 2)
			},
			new ValueTuple<string, int>[]
			{
				new ValueTuple<string, int>("sea_hounds_marksman", 1),
				new ValueTuple<string, int>("sea_hounds_pups", 2)
			},
			new ValueTuple<string, int>[]
			{
				new ValueTuple<string, int>("sea_hounds_marksman", 2),
				new ValueTuple<string, int>("sea_hounds", 4)
			},
			new ValueTuple<string, int>[]
			{
				new ValueTuple<string, int>("sea_hounds_marksman", 2),
				new ValueTuple<string, int>("sea_hounds_pups", 4)
			}
		};

		// Token: 0x040000D4 RID: 212
		private readonly Dictionary<int, string> _enemyShipsToAddBallista = new Dictionary<int, string>
		{
			{ 2, "fore_mangonel" },
			{ 4, "fore_mangonel" },
			{ 6, "fore_mangonel" },
			{ 8, "fore_mangonel" }
		};

		// Token: 0x040000D5 RID: 213
		private MissionShip _playerShip;

		// Token: 0x040000D8 RID: 216
		private GameEntity _trailingTargetObject;

		// Token: 0x040000D9 RID: 217
		private ShipTargetMissionObject _playerShipTargetObject;

		// Token: 0x040000DA RID: 218
		private readonly FloatingFortressSetPieceBattleMissionController.TrailController _playerShipTargetObjectTrailController = new FloatingFortressSetPieceBattleMissionController.TrailController(6f, 0.25f);

		// Token: 0x040000DB RID: 219
		private MBList<MissionShip> _enemyMissionShipsOrdered;

		// Token: 0x040000DC RID: 220
		private bool _isPhaseOneInitialized;

		// Token: 0x040000DD RID: 221
		private int _currentPhaseOneInitializationTick;

		// Token: 0x040000DE RID: 222
		private float _playerLoseRemainingTime = 5f;

		// Token: 0x040000DF RID: 223
		private float _lastRandomAttackPointPickTime;

		// Token: 0x040000E0 RID: 224
		private Vec3 _randomAttackPoint;

		// Token: 0x040000E1 RID: 225
		private bool _shouldStartPhaseTwo;

		// Token: 0x040000E2 RID: 226
		private bool _isPhaseTwoInitialized;

		// Token: 0x040000E3 RID: 227
		private int _currentPhaseTwoInitializationTick;

		// Token: 0x040000E4 RID: 228
		private bool _isMissionSuccessful;

		// Token: 0x040000E5 RID: 229
		private bool _isMissionFailed;

		// Token: 0x040000E6 RID: 230
		private List<GameEntity> _entities = new List<GameEntity>();

		// Token: 0x040000E7 RID: 231
		private readonly MBList<MissionShip> _playerAllyMissionShips = new MBList<MissionShip>();

		// Token: 0x040000E8 RID: 232
		private readonly MBList<ValueTuple<MissionShip, bool>> _playerAllyShipAnchorState = new MBList<ValueTuple<MissionShip, bool>>();

		// Token: 0x040000E9 RID: 233
		private readonly MBList<DestructableComponent> _enemySiegeWeaponDestructables = new MBList<DestructableComponent>();

		// Token: 0x040000EA RID: 234
		private readonly Dictionary<DestructableComponent, DestructableComponent> _enemySiegeWeaponByCover = new Dictionary<DestructableComponent, DestructableComponent>();

		// Token: 0x040000EB RID: 235
		private readonly Dictionary<RangedSiegeWeapon, Agent> _cachedMangonelAgents = new Dictionary<RangedSiegeWeapon, Agent>();

		// Token: 0x040000EC RID: 236
		private NavalAgentsLogic _navalAgentsLogic;

		// Token: 0x040000ED RID: 237
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x040000EE RID: 238
		private readonly FloatingFortressSetPieceBattleMissionController.ConversationLine _playerShipTooCloseLine;

		// Token: 0x040000EF RID: 239
		private readonly FloatingFortressSetPieceBattleMissionController.ConversationLine _playerShipLowHpLine;

		// Token: 0x040000F0 RID: 240
		private readonly FloatingFortressSetPieceBattleMissionController.ConversationLine _playerShipRemainingAmmoLine;

		// Token: 0x040000F1 RID: 241
		private readonly FloatingFortressSetPieceBattleMissionController.ConversationLine _playerShipStandingStillLine;

		// Token: 0x040000F2 RID: 242
		private readonly FloatingFortressSetPieceBattleMissionController.ConversationLine _playerShipHitLine;

		// Token: 0x040000F3 RID: 243
		private readonly FloatingFortressSetPieceBattleMissionController.ConversationLine _playerShipSailDestroyedLine;

		// Token: 0x040000F4 RID: 244
		private readonly FloatingFortressSetPieceBattleMissionController.ConversationLine _playerTookMangonelDownLine;

		// Token: 0x040000F5 RID: 245
		private readonly FloatingFortressSetPieceBattleMissionController.ConversationLine _playerTookAllMangonelsDownLine;

		// Token: 0x040000F6 RID: 246
		private MissionObjectiveLogic _missionObjectiveLogic;

		// Token: 0x040000F7 RID: 247
		private BoardFloatingFortressObjective _boardFloatingFortressObjective;

		// Token: 0x040000F8 RID: 248
		private DefeatTheEnemyCrewObjective _defeatTheEnemyCrewObjective;

		// Token: 0x0200018D RID: 397
		private abstract class ConversationLine
		{
			// Token: 0x06001915 RID: 6421 RVA: 0x000AD1B7 File Offset: 0x000AB3B7
			public void TryPlayLine()
			{
				if (!this.CanBePlayed())
				{
					return;
				}
				this.Play();
			}

			// Token: 0x06001916 RID: 6422
			protected abstract void Play();

			// Token: 0x06001917 RID: 6423
			public abstract void Stop();

			// Token: 0x06001918 RID: 6424
			public abstract bool IsPlaying();

			// Token: 0x06001919 RID: 6425
			protected abstract bool CanBePlayed();
		}

		// Token: 0x0200018E RID: 398
		private class SimpleConversationLine : FloatingFortressSetPieceBattleMissionController.ConversationLine
		{
			// Token: 0x0600191B RID: 6427 RVA: 0x000AD1D0 File Offset: 0x000AB3D0
			public SimpleConversationLine(CharacterObject speaker, string line, float cooldown, MBInformationManager.NotificationPriority priority)
			{
				this._speaker = speaker;
				this._cooldown = cooldown;
				this._priority = priority;
				this._line = new TextObject(line, null);
				this._blockedTime = 0f;
			}

			// Token: 0x0600191C RID: 6428 RVA: 0x000AD206 File Offset: 0x000AB406
			protected override void Play()
			{
				this._handle = CampaignInformationManager.AddDialogLine(this._line, this._speaker, null, 0, this._priority);
				this._blockedTime = Mission.Current.CurrentTime + this._cooldown;
			}

			// Token: 0x0600191D RID: 6429 RVA: 0x000AD23E File Offset: 0x000AB43E
			public override void Stop()
			{
				CampaignInformationManager.ClearDialogNotification(this._handle, false);
			}

			// Token: 0x0600191E RID: 6430 RVA: 0x000AD24C File Offset: 0x000AB44C
			public override bool IsPlaying()
			{
				return this._handle != null && CampaignInformationManager.GetStatusOfDialogNotification(this._handle) == 1;
			}

			// Token: 0x0600191F RID: 6431 RVA: 0x000AD266 File Offset: 0x000AB466
			protected override bool CanBePlayed()
			{
				return this._blockedTime <= Mission.Current.CurrentTime;
			}

			// Token: 0x04000C4B RID: 3147
			private readonly TextObject _line;

			// Token: 0x04000C4C RID: 3148
			private readonly CharacterObject _speaker;

			// Token: 0x04000C4D RID: 3149
			private readonly float _cooldown;

			// Token: 0x04000C4E RID: 3150
			private readonly MBInformationManager.NotificationPriority _priority;

			// Token: 0x04000C4F RID: 3151
			private MBInformationManager.DialogNotificationHandle _handle;

			// Token: 0x04000C50 RID: 3152
			private float _blockedTime;
		}

		// Token: 0x0200018F RID: 399
		private class VariantConversationLine : FloatingFortressSetPieceBattleMissionController.ConversationLine
		{
			// Token: 0x06001920 RID: 6432 RVA: 0x000AD280 File Offset: 0x000AB480
			public VariantConversationLine(FloatingFortressSetPieceBattleMissionController.ConversationLine[] lines, FloatingFortressSetPieceBattleMissionController.VariantConversationLine.VariationType variationType, float cooldown, bool canShowEachLineOnce = false)
			{
				this._lines = lines.ToList<FloatingFortressSetPieceBattleMissionController.ConversationLine>();
				this._variationType = variationType;
				this._cooldown = cooldown;
				this._canShowEachLineOnce = canShowEachLineOnce;
				this._current = -1;
				this._active = null;
				this._blockedTime = 0f;
			}

			// Token: 0x06001921 RID: 6433 RVA: 0x000AD2D0 File Offset: 0x000AB4D0
			protected override void Play()
			{
				FloatingFortressSetPieceBattleMissionController.VariantConversationLine.VariationType variationType = this._variationType;
				if (variationType != FloatingFortressSetPieceBattleMissionController.VariantConversationLine.VariationType.Ordered)
				{
					if (variationType != FloatingFortressSetPieceBattleMissionController.VariantConversationLine.VariationType.Random)
					{
						Debug.FailedAssert("Unknown variation type!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Storyline\\MissionControllers\\FloatingFortressSetPieceBattleMissionController.cs", "Play", 137);
						throw new ArgumentOutOfRangeException();
					}
					this._current = MBRandom.RandomInt(0, this._lines.Count);
				}
				else
				{
					this._current = (this._current + 1) % this._lines.Count;
				}
				this._active = this._lines[this._current];
				this._active.TryPlayLine();
				if (this._canShowEachLineOnce)
				{
					this._lines.RemoveAt(this._current);
				}
				this._blockedTime = Mission.Current.CurrentTime + this._cooldown;
			}

			// Token: 0x06001922 RID: 6434 RVA: 0x000AD393 File Offset: 0x000AB593
			public override void Stop()
			{
				this._active.Stop();
				this._active = null;
			}

			// Token: 0x06001923 RID: 6435 RVA: 0x000AD3A7 File Offset: 0x000AB5A7
			public override bool IsPlaying()
			{
				return this._active != null && this._active.IsPlaying();
			}

			// Token: 0x06001924 RID: 6436 RVA: 0x000AD3BE File Offset: 0x000AB5BE
			protected override bool CanBePlayed()
			{
				return this._lines.Count > 0 && this._blockedTime <= Mission.Current.CurrentTime;
			}

			// Token: 0x04000C51 RID: 3153
			private int _current;

			// Token: 0x04000C52 RID: 3154
			private FloatingFortressSetPieceBattleMissionController.ConversationLine _active;

			// Token: 0x04000C53 RID: 3155
			private float _blockedTime;

			// Token: 0x04000C54 RID: 3156
			private readonly List<FloatingFortressSetPieceBattleMissionController.ConversationLine> _lines;

			// Token: 0x04000C55 RID: 3157
			private readonly float _cooldown;

			// Token: 0x04000C56 RID: 3158
			private readonly FloatingFortressSetPieceBattleMissionController.VariantConversationLine.VariationType _variationType;

			// Token: 0x04000C57 RID: 3159
			private readonly bool _canShowEachLineOnce;

			// Token: 0x020002AD RID: 685
			public enum VariationType
			{
				// Token: 0x04001163 RID: 4451
				Ordered,
				// Token: 0x04001164 RID: 4452
				Random
			}
		}

		// Token: 0x02000190 RID: 400
		private class SequencedConversationLine : FloatingFortressSetPieceBattleMissionController.ConversationLine
		{
			// Token: 0x06001925 RID: 6437 RVA: 0x000AD3E5 File Offset: 0x000AB5E5
			public SequencedConversationLine(FloatingFortressSetPieceBattleMissionController.ConversationLine[] lines, float cooldown)
			{
				this._lines = lines;
				this._cooldown = cooldown;
				this._blockedTime = 0f;
			}

			// Token: 0x06001926 RID: 6438 RVA: 0x000AD408 File Offset: 0x000AB608
			protected override void Play()
			{
				FloatingFortressSetPieceBattleMissionController.ConversationLine[] lines = this._lines;
				for (int i = 0; i < lines.Length; i++)
				{
					lines[i].TryPlayLine();
				}
				this._blockedTime = Mission.Current.CurrentTime + this._cooldown;
			}

			// Token: 0x06001927 RID: 6439 RVA: 0x000AD44C File Offset: 0x000AB64C
			public override void Stop()
			{
				FloatingFortressSetPieceBattleMissionController.ConversationLine[] lines = this._lines;
				for (int i = 0; i < lines.Length; i++)
				{
					lines[i].Stop();
				}
			}

			// Token: 0x06001928 RID: 6440 RVA: 0x000AD476 File Offset: 0x000AB676
			public override bool IsPlaying()
			{
				return this._lines.Any<FloatingFortressSetPieceBattleMissionController.ConversationLine>((FloatingFortressSetPieceBattleMissionController.ConversationLine x) => x.IsPlaying());
			}

			// Token: 0x06001929 RID: 6441 RVA: 0x000AD4A2 File Offset: 0x000AB6A2
			protected override bool CanBePlayed()
			{
				return this._blockedTime <= Mission.Current.CurrentTime;
			}

			// Token: 0x04000C58 RID: 3160
			private float _blockedTime;

			// Token: 0x04000C59 RID: 3161
			private readonly float _cooldown;

			// Token: 0x04000C5A RID: 3162
			private readonly FloatingFortressSetPieceBattleMissionController.ConversationLine[] _lines;
		}

		// Token: 0x02000191 RID: 401
		private class CircularBuffer<T>
		{
			// Token: 0x0600192A RID: 6442 RVA: 0x000AD4B9 File Offset: 0x000AB6B9
			public CircularBuffer(int capacity)
			{
				this._capacity = capacity;
				this._buffer = new T[capacity];
				this._head = 0;
				this._tail = 0;
				this.Count = 0;
			}

			// Token: 0x170003FA RID: 1018
			// (get) Token: 0x0600192B RID: 6443 RVA: 0x000AD4E9 File Offset: 0x000AB6E9
			// (set) Token: 0x0600192C RID: 6444 RVA: 0x000AD4F1 File Offset: 0x000AB6F1
			public int Count { get; private set; }

			// Token: 0x0600192D RID: 6445 RVA: 0x000AD4FC File Offset: 0x000AB6FC
			public void Add(T item)
			{
				this._buffer[this._tail] = item;
				this._tail = (this._tail + 1) % this._capacity;
				if (this.Count < this._capacity)
				{
					int count = this.Count;
					this.Count = count + 1;
					return;
				}
				this._head = (this._head + 1) % this._capacity;
			}

			// Token: 0x170003FB RID: 1019
			public T this[int index]
			{
				get
				{
					int num = (this._head + index) % this._capacity;
					return this._buffer[num];
				}
				set
				{
					int num = (this._head + index) % this._capacity;
					this._buffer[num] = value;
				}
			}

			// Token: 0x04000C5B RID: 3163
			private readonly T[] _buffer;

			// Token: 0x04000C5C RID: 3164
			private int _head;

			// Token: 0x04000C5D RID: 3165
			private int _tail;

			// Token: 0x04000C5E RID: 3166
			private readonly int _capacity;
		}

		// Token: 0x02000192 RID: 402
		private class TrailController
		{
			// Token: 0x06001930 RID: 6448 RVA: 0x000AD5BC File Offset: 0x000AB7BC
			public TrailController(float trailDelay, float recordInterval)
			{
				this._trailDelay = trailDelay;
				this._recordInterval = recordInterval;
				this._lastRecordTime = 0f;
				int num = (int)Math.Ceiling((double)(trailDelay / recordInterval));
				num = Math.Max(num, 10) + 1;
				this._positions = new FloatingFortressSetPieceBattleMissionController.CircularBuffer<Vec3>(num);
				this._timestamps = new FloatingFortressSetPieceBattleMissionController.CircularBuffer<float>(num);
			}

			// Token: 0x06001931 RID: 6449 RVA: 0x000AD616 File Offset: 0x000AB816
			public void RecordPosition(Vec3 position, float currentTime)
			{
				if (currentTime - this._lastRecordTime >= this._recordInterval)
				{
					this._positions.Add(position);
					this._timestamps.Add(currentTime);
					this._lastRecordTime = currentTime;
				}
			}

			// Token: 0x06001932 RID: 6450 RVA: 0x000AD648 File Offset: 0x000AB848
			public Vec3 GetTrailEndPosition(float currentTime)
			{
				if (this._positions.Count == 0)
				{
					return default(Vec3);
				}
				float num = currentTime - this._trailDelay;
				for (int i = this._timestamps.Count - 1; i >= 1; i--)
				{
					float num2 = this._timestamps[i - 1];
					float num3 = this._timestamps[i];
					if (num >= num2 && num <= num3)
					{
						float num4 = (num - num2) / (num3 - num2);
						Vec3 vec = this._positions[i - 1];
						Vec3 vec2 = this._positions[i];
						return Vec3.Lerp(vec, vec2, num4);
					}
				}
				if (num > this._timestamps[0])
				{
					return this._positions[this._positions.Count - 1];
				}
				return this._positions[0];
			}

			// Token: 0x04000C60 RID: 3168
			private readonly FloatingFortressSetPieceBattleMissionController.CircularBuffer<Vec3> _positions;

			// Token: 0x04000C61 RID: 3169
			private readonly FloatingFortressSetPieceBattleMissionController.CircularBuffer<float> _timestamps;

			// Token: 0x04000C62 RID: 3170
			private readonly float _trailDelay;

			// Token: 0x04000C63 RID: 3171
			private readonly float _recordInterval;

			// Token: 0x04000C64 RID: 3172
			private float _lastRecordTime;
		}
	}
}
