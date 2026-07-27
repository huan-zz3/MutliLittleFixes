using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using NavalDLC.MissionObjects;
using NavalDLC.Missions;
using NavalDLC.Missions.AI.Tactics;
using NavalDLC.Missions.AI.TeamAI;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.Objects.UsableMachines;
using NavalDLC.Missions.ShipActuators;
using NavalDLC.Missions.ShipControl;
using NavalDLC.Missions.ShipInput;
using NavalDLC.Storyline.Objectives.Quest5;
using SandBox;
using SandBox.Conversation.MissionLogics;
using SandBox.Missions.AgentBehaviors;
using SandBox.Objects;
using SandBox.Objects.Usables;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.MissionLogics;
using TaleWorlds.MountAndBlade.Missions.Objectives;
using TaleWorlds.MountAndBlade.Objects.Usables;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.Storyline.MissionControllers
{
	// Token: 0x0200006F RID: 111
	public class Quest5SetPieceBattleMissionController : MissionLogic, IMissionAgentSpawnLogic, IMissionBehavior
	{
		// Token: 0x17000145 RID: 325
		// (get) Token: 0x060006BA RID: 1722 RVA: 0x000282D8 File Offset: 0x000264D8
		private GameEntity JumpOffInitialPosition
		{
			get
			{
				if (this._jumpOffInitialPositionGameEntity == null)
				{
					this._jumpOffInitialPositionGameEntity = Mission.Current.Scene.FindEntityWithTag("gangradir_jump_off_initial");
				}
				return this._jumpOffInitialPositionGameEntity;
			}
		}

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x060006BB RID: 1723 RVA: 0x00028308 File Offset: 0x00026508
		private GameEntity JumpOffTargetPosition
		{
			get
			{
				if (this._jumpOffTargetPositionGameEntity == null)
				{
					this._jumpOffTargetPositionGameEntity = Mission.Current.Scene.FindEntityWithTag("gangradir_jump_off_target");
				}
				return this._jumpOffTargetPositionGameEntity;
			}
		}

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x060006BC RID: 1724 RVA: 0x00028338 File Offset: 0x00026538
		private GameEntity HidingSpot1Position
		{
			get
			{
				if (this._hidingSpot1PositionGameEntity == null)
				{
					this._hidingSpot1PositionGameEntity = Mission.Current.Scene.FindEntityWithTag("sp_gangradir_hiding_spot");
				}
				return this._hidingSpot1PositionGameEntity;
			}
		}

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x060006BD RID: 1725 RVA: 0x00028368 File Offset: 0x00026568
		private MatrixFrame GunnarShipUsePosition
		{
			get
			{
				return this.EscapeShip.GetCaptainSpawnGlobalFrame();
			}
		}

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x060006BE RID: 1726 RVA: 0x00028375 File Offset: 0x00026575
		// (set) Token: 0x060006BF RID: 1727 RVA: 0x0002837D File Offset: 0x0002657D
		public GameEntity Phase1InteriorCameraSisterEntity { get; private set; }

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x060006C0 RID: 1728 RVA: 0x00028386 File Offset: 0x00026586
		private MissionShip EscapeShip
		{
			get
			{
				return this._phase1EnemyShip3 ?? this._playerShip;
			}
		}

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x060006C1 RID: 1729 RVA: 0x00028398 File Offset: 0x00026598
		// (set) Token: 0x060006C2 RID: 1730 RVA: 0x000283A0 File Offset: 0x000265A0
		public bool IsEscapeShipStuck { get; private set; }

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x060006C3 RID: 1731 RVA: 0x000283A9 File Offset: 0x000265A9
		private int Phase2AllyShip1TroopCount
		{
			get
			{
				return this._phase2AllyShip1Troops.Sum<KeyValuePair<string, int>>((KeyValuePair<string, int> kvp) => kvp.Value);
			}
		}

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x060006C4 RID: 1732 RVA: 0x000283D5 File Offset: 0x000265D5
		private int Phase2AllyShip2TroopCount
		{
			get
			{
				return this._phase2AllyShip2Troops.Sum<KeyValuePair<string, int>>((KeyValuePair<string, int> kvp) => kvp.Value);
			}
		}

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x060006C5 RID: 1733 RVA: 0x00028401 File Offset: 0x00026601
		private int Phase2AllyShip3TroopCount
		{
			get
			{
				return this._phase2AllyShip3Troops.Sum<KeyValuePair<string, int>>((KeyValuePair<string, int> kvp) => kvp.Value);
			}
		}

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x060006C6 RID: 1734 RVA: 0x0002842D File Offset: 0x0002662D
		private int Phase2AllyShip4TroopCount
		{
			get
			{
				return this._phase2AllyShip4Troops.Sum<KeyValuePair<string, int>>((KeyValuePair<string, int> kvp) => kvp.Value);
			}
		}

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x060006C7 RID: 1735 RVA: 0x00028459 File Offset: 0x00026659
		private int Phase2AllyShip5TroopCount
		{
			get
			{
				return this._phase2AllyShip5Troops.Sum<KeyValuePair<string, int>>((KeyValuePair<string, int> kvp) => kvp.Value);
			}
		}

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x060006C8 RID: 1736 RVA: 0x00028485 File Offset: 0x00026685
		private int Phase2EnemyShip1TroopCount
		{
			get
			{
				return this._phase2EnemyShip1Troops.Sum<KeyValuePair<string, int>>((KeyValuePair<string, int> kvp) => kvp.Value);
			}
		}

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x060006C9 RID: 1737 RVA: 0x000284B1 File Offset: 0x000266B1
		private int Phase2EnemyShip2TroopCount
		{
			get
			{
				return this._phase2EnemyShip2Troops.Sum<KeyValuePair<string, int>>((KeyValuePair<string, int> kvp) => kvp.Value);
			}
		}

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x060006CA RID: 1738 RVA: 0x000284DD File Offset: 0x000266DD
		private int Phase2EnemyShip3TroopCount
		{
			get
			{
				return this._phase2EnemyShip3Troops.Sum<KeyValuePair<string, int>>((KeyValuePair<string, int> kvp) => kvp.Value);
			}
		}

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x060006CB RID: 1739 RVA: 0x00028509 File Offset: 0x00026709
		private int Phase2EnemyShip4TroopCount
		{
			get
			{
				return this._phase2EnemyShip4Troops.Sum<KeyValuePair<string, int>>((KeyValuePair<string, int> kvp) => kvp.Value);
			}
		}

		// Token: 0x17000155 RID: 341
		// (get) Token: 0x060006CC RID: 1740 RVA: 0x00028535 File Offset: 0x00026735
		private int Phase2EnemyShip5TroopCount
		{
			get
			{
				return this._phase2EnemyShip5Troops.Sum<KeyValuePair<string, int>>((KeyValuePair<string, int> kvp) => kvp.Value);
			}
		}

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x060006CD RID: 1741 RVA: 0x00028561 File Offset: 0x00026761
		private int Phase2EnemyShipStationary1TroopCount
		{
			get
			{
				return this._phase2EnemyShipStationary1Troops.Sum<KeyValuePair<string, int>>((KeyValuePair<string, int> kvp) => kvp.Value);
			}
		}

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x060006CE RID: 1742 RVA: 0x0002858D File Offset: 0x0002678D
		private int Phase3PlayerShipTroopCount
		{
			get
			{
				return this._phase3PlayerShipTroops.Sum<KeyValuePair<string, int>>((KeyValuePair<string, int> kvp) => kvp.Value);
			}
		}

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x060006CF RID: 1743 RVA: 0x000285B9 File Offset: 0x000267B9
		private int Phase3EnemyShip1TroopCount
		{
			get
			{
				return this._phase3EnemyShip1Troops.Sum<KeyValuePair<string, int>>((KeyValuePair<string, int> kvp) => kvp.Value);
			}
		}

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x060006D0 RID: 1744 RVA: 0x000285E5 File Offset: 0x000267E5
		private int Phase3EnemyShip2TroopCount
		{
			get
			{
				return this._phase3EnemyShip2Troops.Sum<KeyValuePair<string, int>>((KeyValuePair<string, int> kvp) => kvp.Value);
			}
		}

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x060006D1 RID: 1745 RVA: 0x00028611 File Offset: 0x00026811
		private int Phase3EnemyShip3TroopCount
		{
			get
			{
				return this._phase3EnemyShip3Troops.Sum<KeyValuePair<string, int>>((KeyValuePair<string, int> kvp) => kvp.Value);
			}
		}

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x060006D2 RID: 1746 RVA: 0x0002863D File Offset: 0x0002683D
		private int Phase3EnemyShip4TroopCount
		{
			get
			{
				return this._phase3EnemyShip4Troops.Sum<KeyValuePair<string, int>>((KeyValuePair<string, int> kvp) => kvp.Value);
			}
		}

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x060006D3 RID: 1747 RVA: 0x00028669 File Offset: 0x00026869
		private int Phase3EnemyShip5TroopCount
		{
			get
			{
				return this._phase3EnemyShip5Troops.Sum<KeyValuePair<string, int>>((KeyValuePair<string, int> kvp) => kvp.Value);
			}
		}

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x060006D4 RID: 1748 RVA: 0x00028695 File Offset: 0x00026895
		private int Phase3EnemyReinforcementShip1TroopCount
		{
			get
			{
				return this._phase3EnemyReinforcementShip1Troops.Sum<KeyValuePair<string, int>>((KeyValuePair<string, int> kvp) => kvp.Value);
			}
		}

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x060006D5 RID: 1749 RVA: 0x000286C1 File Offset: 0x000268C1
		private int Phase3EnemyReinforcementShip2TroopCount
		{
			get
			{
				return this._phase3EnemyReinforcementShip2Troops.Sum<KeyValuePair<string, int>>((KeyValuePair<string, int> kvp) => kvp.Value);
			}
		}

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x060006D6 RID: 1750 RVA: 0x000286ED File Offset: 0x000268ED
		// (set) Token: 0x060006D7 RID: 1751 RVA: 0x000286F5 File Offset: 0x000268F5
		public Quest5SetPieceBattleMissionController.BossFightOutComeEnum BossFightOutCome { get; private set; }

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x060006D8 RID: 1752 RVA: 0x000286FE File Offset: 0x000268FE
		// (set) Token: 0x060006D9 RID: 1753 RVA: 0x00028706 File Offset: 0x00026906
		public GameEntity BossFightConversationCameraGameEntity { get; private set; }

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x060006DA RID: 1754 RVA: 0x0002870F File Offset: 0x0002690F
		// (set) Token: 0x060006DB RID: 1755 RVA: 0x00028717 File Offset: 0x00026917
		public MissionShip Phase4PurigShip { get; private set; }

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x060006DC RID: 1756 RVA: 0x00028720 File Offset: 0x00026920
		// (set) Token: 0x060006DD RID: 1757 RVA: 0x00028728 File Offset: 0x00026928
		public Agent SisterAgent { get; private set; }

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x060006DE RID: 1758 RVA: 0x00028731 File Offset: 0x00026931
		// (set) Token: 0x060006DF RID: 1759 RVA: 0x00028739 File Offset: 0x00026939
		public Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState LastHitCheckpoint { get; private set; }

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x060006E0 RID: 1760 RVA: 0x00028742 File Offset: 0x00026942
		// (set) Token: 0x060006E1 RID: 1761 RVA: 0x0002874A File Offset: 0x0002694A
		public Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState State { get; private set; }

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x060006E2 RID: 1762 RVA: 0x00028753 File Offset: 0x00026953
		// (set) Token: 0x060006E3 RID: 1763 RVA: 0x0002875B File Offset: 0x0002695B
		public bool ShouldMissionContinueFromCheckpoint { get; private set; }

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x060006E4 RID: 1764 RVA: 0x00028764 File Offset: 0x00026964
		public BattleSideEnum PlayerSide
		{
			get
			{
				return -1;
			}
		}

		// Token: 0x060006E5 RID: 1765 RVA: 0x00028768 File Offset: 0x00026968
		public Quest5SetPieceBattleMissionController(Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState lastHitCheckpoint, MobileParty enemyParty)
		{
			this.BossFightOutCome = Quest5SetPieceBattleMissionController.BossFightOutComeEnum.None;
			this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.None;
			this.LastHitCheckpoint = lastHitCheckpoint;
			this.ShouldMissionContinueFromCheckpoint = false;
			this._enemyParty = enemyParty;
			Hero.MainHero.HitPoints = Hero.MainHero.MaxHitPoints;
			NavalStorylineData.Gunnar.HitPoints = NavalStorylineData.Gunnar.MaxHitPoints;
			NavalStorylineData.Prusas.HitPoints = NavalStorylineData.Prusas.MaxHitPoints;
			NavalStorylineData.Purig.HitPoints = NavalStorylineData.Purig.MaxHitPoints;
			NavalStorylineData.Bjolgur.HitPoints = NavalStorylineData.Bjolgur.MaxHitPoints;
			NavalStorylineData.Lahar.HitPoints = NavalStorylineData.Lahar.MaxHitPoints;
		}

		// Token: 0x060006E6 RID: 1766 RVA: 0x000298F8 File Offset: 0x00027AF8
		public override void AfterStart()
		{
			base.AfterStart();
			Mission.Current.Scene.SetAtmosphereWithName("TOD_02_00_SemiCloudy");
			this._slaveTraderCharacter = MBObjectManager.Instance.GetObject<CharacterObject>("sea_hounds");
			this.AddConversationSounds();
			this._navalAgentsLogic = base.Mission.GetMissionBehavior<NavalAgentsLogic>();
			this._navalShipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
			this._navalTrajectoryPlanningLogic = base.Mission.GetMissionBehavior<NavalTrajectoryPlanningLogic>();
			this._missionObjectiveLogic = base.Mission.GetMissionBehavior<MissionObjectiveLogic>();
			this._lightScriptedFiresMissionController = base.Mission.GetMissionBehavior<LightScriptedFiresMissionController>();
			Team team = Mission.GetTeam(0);
			this.AddAvailableAllyFormation(team.GetFormation(0));
			this.AddAvailableAllyFormation(team.GetFormation(1));
			this.AddAvailableAllyFormation(team.GetFormation(2));
			this.AddAvailableAllyFormation(team.GetFormation(3));
			this.AddAvailableAllyFormation(team.GetFormation(4));
			this.AddAvailableAllyFormation(team.GetFormation(5));
			this.AddAvailableAllyFormation(team.GetFormation(6));
			this.AddAvailableAllyFormation(team.GetFormation(7));
			Team team2 = Mission.GetTeam(2);
			this.AddAvailableEnemyFormation(team2.GetFormation(0));
			this.AddAvailableEnemyFormation(team2.GetFormation(1));
			this.AddAvailableEnemyFormation(team2.GetFormation(2));
			this.AddAvailableEnemyFormation(team2.GetFormation(3));
			this.AddAvailableEnemyFormation(team2.GetFormation(4));
			this.AddAvailableEnemyFormation(team2.GetFormation(5));
			this.AddAvailableEnemyFormation(team2.GetFormation(6));
			this.AddAvailableEnemyFormation(team2.GetFormation(7));
			this._phase1InteriorToEnemyShip3ShipDoorEntity = Mission.Current.Scene.FindEntityWithTag("phase_1_interior_to_enemy_ship_3_door_tag");
			this._phase1InteriorToEnemyShip3ShipDoorEntity.GetFirstScriptOfType<ShipDoorUsePoint>().SetShipDoorUsePointEnabled(false);
			foreach (GameEntity gameEntity in Mission.Current.Scene.FindEntitiesWithTagExpression("phase_2_escape_ship_target(_\\d+)*"))
			{
				this._phase2EscapeShipTargetPointEntities.Add(gameEntity);
			}
			GameEntity[] array = new GameEntity[this._phase2EscapeShipTargetPointEntities.Count];
			foreach (GameEntity gameEntity2 in this._phase2EscapeShipTargetPointEntities)
			{
				string[] array2 = gameEntity2.Tags.FirstOrDefault<string>().Split(new char[] { '_' });
				int num = int.Parse(array2[array2.Length - 1]);
				array[num - 1] = gameEntity2;
			}
			foreach (GameEntity gameEntity3 in array)
			{
				this._phase2EscapeShipTargetPoints.Enqueue(gameEntity3);
			}
			this._phase1EnemyShip1InitialSpawnEntity = Mission.Current.Scene.FindEntityWithTag("phase_1_enemy_ship_1_sp_initial");
			this._phase1EnemyShip1TargetEntity = Mission.Current.Scene.FindEntityWithTag("phase_1_enemy_ship_1_sp");
			this._phase3TriggerVolumeBox = Mission.Current.Scene.FindEntityWithTag("phase_3_trigger_volume_box_tag").GetFirstScriptOfType<VolumeBox>();
			this._phase4TriggerVolumeBox = Mission.Current.Scene.FindEntityWithTag("phase_4_purigs_entrance_trigger_box").GetFirstScriptOfType<VolumeBox>();
			this._approachPointEntity = Mission.Current.Scene.FindEntityWithTag("phase_1_approach_point");
			this._navalShipsLogic.SetDeploymentMode(true);
			this._navalShipsLogic.SetTeamShipDeploymentLimit(0, NavalShipDeploymentLimit.Max());
			this._navalShipsLogic.SetTeamShipDeploymentLimit(1, NavalShipDeploymentLimit.Max());
			this._navalShipsLogic.SetTeamShipDeploymentLimit(2, NavalShipDeploymentLimit.Max());
			this._navalShipsLogic.SetDeploymentMode(false);
			base.Mission.GetMissionBehavior<MissionConversationLogic>().DisableStartConversation(true);
			this._playerFormation = this.GetAvailableAllyFormation();
			Hero.MainHero.Heal(Hero.MainHero.MaxHitPoints, false);
			NavalStorylineData.Gunnar.Heal(NavalStorylineData.Gunnar.MaxHitPoints, false);
			NavalStorylineData.Prusas.Heal(NavalStorylineData.Prusas.MaxHitPoints, false);
			StoryModeHeroes.LittleSister.Heal(StoryModeHeroes.LittleSister.MaxHitPoints, false);
			this._sisterWoundedAnimationActionIndexCache = ActionIndexCache.Create("act_conversation_weary2_loop");
			this._slaveTraderShipOarsmanActionIndexCache = ActionIndexCache.Create("act_sit_2");
			this._navalAgentsLogic.SetSpawnReinforcementsOnTick(false, true);
			this.State = this.LastHitCheckpoint;
		}

		// Token: 0x060006E7 RID: 1767 RVA: 0x00029D18 File Offset: 0x00027F18
		public override void OnBehaviorInitialize()
		{
			if (!SailWindProfile.IsSailWindProfileInitialized)
			{
				SailWindProfile.InitializeProfile();
			}
			if (this._navalShipsLogic == null)
			{
				this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			}
			this._navalShipsLogic.ShipAttachmentBrokenEvent += this.OnAttachmentBroken;
		}

		// Token: 0x060006E8 RID: 1768 RVA: 0x00029D58 File Offset: 0x00027F58
		public override void OnFixedMissionTick(float fixedDt)
		{
			base.OnFixedMissionTick(fixedDt);
			Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState state = this.State;
			if (state == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase2InProgress)
			{
				this.HandlePirateShipGettingCloseToEscapeShip(this._phase2EnemyShip1, this._phase2EscapeShipPirateTargetFrame1, 5f, fixedDt);
				this.HandlePirateShipGettingCloseToEscapeShip(this._phase2EnemyShip2, this._phase2EscapeShipPirateTargetFrame2, 5f, fixedDt);
				this.HandlePirateShipGettingCloseToEscapeShip(this._phase2EnemyShip3, this._phase2EscapeShipPirateTargetFrame3, 5f, fixedDt);
				this.HandlePirateShipGettingCloseToEscapeShip(this._phase2EnemyShip4, this._phase2EscapeShipPirateTargetFrame4, 5f, fixedDt);
				this.HandlePirateShipGettingCloseToEscapeShip(this._phase2EnemyShip5, this._phase2EscapeShipPirateTargetFrame5, 5f, fixedDt);
				this.MoveEscapeShipAlongTheTrack(fixedDt);
			}
		}

		// Token: 0x060006E9 RID: 1769 RVA: 0x00029DF8 File Offset: 0x00027FF8
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			this.HandleStealthShipsBridgeConnections();
			switch (this.State)
			{
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.None:
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoToShipInteriorFadeOut:
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoToShipInteriorFadeIn:
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoBackToShipFadeOut:
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoBackToShipFadeIn:
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1ToPhase2FadeOut:
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1ToPhase2FadeIn:
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase2ToPhase3FadeOut:
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase2ToPhase3FadeIn:
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase3ToPhase4FadeOut:
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase3ToPhase4FadeIn:
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase4ToBossFightFadeOut:
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase4ToBossFightFadeIn:
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Exit:
				goto IL_0BF2;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase1Part1:
				this.InitializePhase1Part1();
				this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase1Part2;
				goto IL_0BF2;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase1Part2:
				this.InitializePhase1Part2();
				this.HandlePlayersBridgeAndControlPointUsagesForPhase1GoToEnemyShip();
				this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoToEnemyShip;
				goto IL_0BF2;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoToEnemyShip:
				if (this._instructionState == Quest5SetPieceBattleMissionController.Quest5InstructionState.None)
				{
					this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.Approach;
				}
				this.AdjustWindDirectionAccordingToTargetFrame(this._approachPointEntity.GetGlobalFrame(), 2f, true);
				if (this._playerShip.GameEntity.GetGlobalFrame().origin.Distance(this._approachPointEntity.GetGlobalFrame().origin) <= 30f)
				{
					this.DisableSlaveTraderShipAgents();
					this.OnPlayerShipReachedApproachDistance();
					this.HandlePlayersBridgeAndControlPointUsagesForPhase1SwimmingAndStealthPhase();
				}
				this._phase1EnemyShip3.SetAnchor(true, false, 1f);
				this._phase1EnemyShip3.ShipOrder.SetShipStopOrder();
				this.HandleStealthShipsBridgeConnections();
				this.MovePhase1EnemyShip1ToItsTargetPoint();
				goto IL_0BF2;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1SwimmingPhase:
				if (this._instructionState == Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForJump)
				{
					this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.Jump;
				}
				else if (this._instructionState == Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForSwim && Agent.Main.IsInWater())
				{
					this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.Swim;
				}
				this._playerShip.ShipOrder.SetShipStopOrder();
				this._playerShip.ShipOrder.SetOrderOarsmenLevel(0);
				this.CheckAndPlayCrusasAndSlaveTraderConversationSound();
				if (this._phase1EnemyShip4.GetIsAgentOnShip(Agent.Main, false))
				{
					this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializeStealthPhasePart1;
					this.SetLastCheckpoint(Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializeStealthPhasePart1);
					goto IL_0BF2;
				}
				goto IL_0BF2;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializeStealthPhasePart1:
				this.InitializeStealthPhasePart1();
				this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializeStealthPhasePart2;
				goto IL_0BF2;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializeStealthPhasePart2:
				this.InitializeStealthPhasePart2();
				this.HealMainHero();
				this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1StealthPhase;
				this.HandlePlayersBridgeAndControlPointUsagesForPhase1SwimmingAndStealthPhase();
				goto IL_0BF2;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1StealthPhase:
				this.HandleStealthShipsBridgeConnections();
				this.HandleEscapeShipInteriorDoorUsage();
				if (Agent.Main == null || !Agent.Main.IsActive())
				{
					this.EndMissionWithAutoContinueFromCheckpoint();
				}
				else
				{
					WorldPosition worldPosition;
					this._phase1EnemyShip2.GetWorldPositionOnDeck(out worldPosition);
					if (worldPosition.AsVec2.Distance(Agent.Main.Position.AsVec2) < 20f && this._instructionState == Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForClearGuards)
					{
						this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.ClearGuards;
					}
					if (Extensions.IsEmpty<Agent>(this._stealthAgents) && this._instructionState == Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForCheckInterior)
					{
						this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.CheckInterior;
					}
				}
				this._phase1EnemyShip3.SetAnchor(true, false, 1f);
				this._phase1EnemyShip3.ShipOrder.SetShipStopOrder();
				goto IL_0BF2;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1InitializeShipInteriorPhase:
				this.InitializeShipInteriorPhase();
				goto IL_0BF2;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1ShipInteriorPhase:
				if (this._talkedWithSister)
				{
					this._phase1InteriorToEnemyShip3ShipDoorEntity.GetFirstScriptOfType<ShipDoorUsePoint>().SetShipDoorUsePointEnabled(true);
				}
				else if (this.SisterAgent.Position.Distance(Agent.Main.Position) < 3f)
				{
					this.Phase1InteriorCameraSisterEntity = Mission.Current.Scene.FindEntityWithTag("phase_1_interior_camera_sister");
				}
				this.SisterAgent.SetActionChannel(0, ref this._sisterWoundedAnimationActionIndexCache, false, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
				goto IL_0BF2;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1InitializeGoBackToShip:
				this.InitializeGoBackToShip();
				if (Extensions.IsEmpty<Agent>(this._stealthAgents))
				{
					this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForCutLoose;
					goto IL_0BF2;
				}
				goto IL_0BF2;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1EscapePhase:
				if (this._talkedWithSister)
				{
					if (this._instructionState < Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForCutLoose)
					{
						this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForCutLoose;
					}
					bool isThereActiveBridgeTo = this._phase1EnemyShip3.GetIsThereActiveBridgeTo(this._phase1EnemyShip2);
					if (isThereActiveBridgeTo && this._instructionState == Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForCutLoose && Extensions.IsEmpty<Agent>(this._stealthAgents))
					{
						this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.CutLoose;
						this._escapeShipCutLooseTimer = new MissionTimer(300f);
					}
					else if (!isThereActiveBridgeTo && this._instructionState == Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForGunnarUsesShip && Extensions.IsEmpty<Agent>(this._stealthAgents))
					{
						this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.GunnarUsesShip;
					}
					else if (!isThereActiveBridgeTo)
					{
						this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1ToPhase2FadeOut;
					}
					this.HandleEscapeShipCutLoose();
					goto IL_0BF2;
				}
				goto IL_0BF2;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase2Part1:
				this.InitializePhase2Part1();
				this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase2Part2;
				goto IL_0BF2;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase2Part2:
				this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase2Part3;
				this.InitializePhase2Part2();
				goto IL_0BF2;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase2Part3:
				this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase2Part4;
				this.InitializePhase2Part3();
				goto IL_0BF2;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase2Part4:
				this.InitializePhase2Part4();
				this.HealMainHero();
				this.SetLastCheckpoint(Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase2Part1);
				goto IL_0BF2;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase2InProgress:
				this.UpdatePhase2MovingShipParameters(dt);
				if (this._isCheckpointInitialize)
				{
					this._isCheckpointInitialize = false;
				}
				this.CheckForEscapeShipStuck();
				this.HandleEscapeShipSpeed();
				this.HandleEscapeShipMovement();
				this.HandlePirateShipMovement(this._phase2EnemyShip1, this._phase2EscapeShipPirateTargetFrame1);
				this.HandlePirateShipMovement(this._phase2EnemyShip2, this._phase2EscapeShipPirateTargetFrame2);
				this.HandlePirateShipMovement(this._phase2EnemyShip3, this._phase2EscapeShipPirateTargetFrame3);
				this.HandlePirateShipMovement(this._phase2EnemyShip4, this._phase2EscapeShipPirateTargetFrame4);
				this.HandlePirateShipMovement(this._phase2EnemyShip5, this._phase2EscapeShipPirateTargetFrame5);
				this.HandlePirateShipSailModeAccordingToTheGlobalWindVelocity(this._phase2EnemyShip1);
				this.HandlePirateShipSailModeAccordingToTheGlobalWindVelocity(this._phase2EnemyShip2);
				this.HandlePirateShipSailModeAccordingToTheGlobalWindVelocity(this._phase2EnemyShip3);
				this.HandlePirateShipSailModeAccordingToTheGlobalWindVelocity(this._phase2EnemyShip4);
				this.HandlePirateShipSailModeAccordingToTheGlobalWindVelocity(this._phase2EnemyShip5);
				this.HandleStationaryShipMovement(this._phase2EnemyShipStationary1);
				this.CheckIfMainAgentLeftTheEscapeShip();
				this.AutoCutLooseEmptyPirateShipIfPlayerDoesNotForALongTime(this._phase2EnemyShip1);
				this.AutoCutLooseEmptyPirateShipIfPlayerDoesNotForALongTime(this._phase2EnemyShip2);
				this.AutoCutLooseEmptyPirateShipIfPlayerDoesNotForALongTime(this._phase2EnemyShip3);
				this.AutoCutLooseEmptyPirateShipIfPlayerDoesNotForALongTime(this._phase2EnemyShip4);
				this.AutoCutLooseEmptyPirateShipIfPlayerDoesNotForALongTime(this._phase2EnemyShip5);
				this.AutoEstablishConnectionsForPirateShips(this._phase2EnemyShip1, this._phase2EscapeShipPirateTargetFrame1);
				this.AutoEstablishConnectionsForPirateShips(this._phase2EnemyShip2, this._phase2EscapeShipPirateTargetFrame2);
				this.AutoEstablishConnectionsForPirateShips(this._phase2EnemyShip3, this._phase2EscapeShipPirateTargetFrame3);
				this.AutoEstablishConnectionsForPirateShips(this._phase2EnemyShip4, this._phase2EscapeShipPirateTargetFrame4);
				this.AutoEstablishConnectionsForPirateShips(this._phase2EnemyShip5, this._phase2EscapeShipPirateTargetFrame5);
				this.HandleAllyShipMovementDuringPhase2(this._phase2AllyShip1);
				this.HandleAllyShipMovementDuringPhase2(this._phase2AllyShip2);
				this.HandleAllyShipMovementDuringPhase2(this._phase2AllyShip3);
				this.HandleAllyShipMovementDuringPhase2(this._phase2AllyShip4);
				this.HandleAllyShipMovementDuringPhase2(this._phase2AllyShip5);
				this.HandlePirateShipBridgeConnectionCount(this._phase2EnemyShip1);
				this.HandlePirateShipBridgeConnectionCount(this._phase2EnemyShip2);
				this.HandlePirateShipBridgeConnectionCount(this._phase2EnemyShip3);
				this.HandlePirateShipBridgeConnectionCount(this._phase2EnemyShip4);
				this.HandlePirateShipBridgeConnectionCount(this._phase2EnemyShip5);
				if (this._instructionState == Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForReachAllies && this.AreAllPhase2PirateShipsEliminated())
				{
					this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.ReachAllies;
				}
				if (Agent.Main != null && this._phase3TriggerVolumeBox.IsPointIn(Agent.Main.Position))
				{
					this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase2ToPhase3FadeOut;
					goto IL_0BF2;
				}
				goto IL_0BF2;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase3Part1:
				this.InitializePhase3Part1();
				this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase3Part2;
				this.SetLastCheckpoint(Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase3Part1);
				goto IL_0BF2;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase3Part2:
				this.InitializePhase3Part2();
				this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase3Part3;
				goto IL_0BF2;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase3Part3:
			{
				this.InitializePhase3Part3();
				this.HealMainHero();
				using (List<MissionShip>.Enumerator enumerator = this._navalShipsLogic.AllShips.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MissionShip missionShip = enumerator.Current;
						if (missionShip != this._playerShip)
						{
							missionShip.ShipOrder.SetShipEngageOrder(true);
						}
					}
					goto IL_0BF2;
				}
				break;
			}
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase3InProgress:
				break;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase4Part1:
				this.InitializePhase4Part1();
				goto IL_0BF2;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase4Part2:
				this.InitializePhase4Part2();
				this.HealMainHero();
				goto IL_0BF2;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase4InProgress:
				if (this._isCheckpointInitialize)
				{
					this._isCheckpointInitialize = false;
				}
				if (this._isPurigCutsceneStarted)
				{
					this.CheckAndPlayPurigCutsceneNotifications();
				}
				if (this._purigShipAgents.Count == 0)
				{
					this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase4ToBossFightFadeOut;
					this._playerShip.SetAnchor(true, false, 1f);
					this._playerShip.ShipOrder.SetShipStopOrder();
					this.Phase4PurigShip.SetAnchor(true, false, 1f);
					this.DisableAllShipOrderControllers();
					goto IL_0BF2;
				}
				goto IL_0BF2;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializeBossFightPart1:
				this.InitializeNavalBossFightPart1();
				this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializeBossFightPart2;
				goto IL_0BF2;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializeBossFightPart2:
				this.InitializeNavalBossFightPart2();
				this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase4ToBossFightFadeIn;
				goto IL_0BF2;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.StartBossFightConversation:
				this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.BossFightConversationInProgress;
				this.StartBossFightConversation();
				goto IL_0BF2;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.BossFightConversationInProgress:
				if (ActionIndexCache.act_conversation_naval_start == this._purigAgent.GetCurrentAction(0) || ActionIndexCache.act_conversation_naval_idle_loop == this._purigAgent.GetCurrentAction(0))
				{
					this._purigAgent.SetCurrentActionProgress(0, 1f);
					this._purigAgent.SetActionChannel(0, ref ActionIndexCache.act_conversation_normal_loop, false, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
					goto IL_0BF2;
				}
				goto IL_0BF2;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.BossFightInProgressAsDuel:
				if (this._purigAgent == null || !this._purigAgent.IsActive())
				{
					this.OnDuelOver(base.Mission.PlayerTeam.Side);
					goto IL_0BF2;
				}
				if (Agent.Main == null || !Agent.Main.IsActive())
				{
					this.OnDuelOver(base.Mission.PlayerEnemyTeam.Side);
					goto IL_0BF2;
				}
				goto IL_0BF2;
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.BossFightInProgressAsAll:
			{
				bool flag = false;
				for (int i = 0; i < this._duelPhaseEnemyAgents.Count; i++)
				{
					if (this._duelPhaseEnemyAgents[i].IsActive())
					{
						flag = true;
						break;
					}
				}
				if (!flag && (this._purigAgent == null || !this._purigAgent.IsActive()))
				{
					this.OnDuelOver(base.Mission.PlayerTeam.Side);
					goto IL_0BF2;
				}
				bool flag2 = false;
				for (int j = 0; j < this._duelPhaseAllyAgents.Count; j++)
				{
					if (this._duelPhaseAllyAgents[j].IsActive())
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2 && (Agent.Main == null || !Agent.Main.IsActive()))
				{
					this.OnDuelOver(base.Mission.PlayerEnemyTeam.Side);
					goto IL_0BF2;
				}
				goto IL_0BF2;
			}
			case Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.End:
				if (this._endMissionTimer == null)
				{
					this._endMissionTimer = new MissionTimer(2f);
					goto IL_0BF2;
				}
				if (this._endMissionTimer.Check(false) || this._isMissionFailPopUpTriggered)
				{
					foreach (MBInformationManager.DialogNotificationHandle dialogNotificationHandle in this._dialogNotificationHandleCache)
					{
						CampaignInformationManager.ClearDialogNotification(dialogNotificationHandle, true);
					}
					this._dialogNotificationHandleCache.Clear();
					if (this._winnerSide == base.Mission.PlayerTeam.Side && !this.ShouldMissionContinueFromCheckpoint)
					{
						this.TriggerPurigsDeadPopUp();
					}
					else
					{
						base.Mission.EndMission();
					}
					this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Exit;
					goto IL_0BF2;
				}
				goto IL_0BF2;
			default:
				goto IL_0BF2;
			}
			if (this._isCheckpointInitialize)
			{
				this._isCheckpointInitialize = false;
			}
			if (this._instructionState == Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForDefeatEnemies)
			{
				this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.DefeatEnemies;
			}
			int count = Mission.Current.PlayerEnemyTeam.ActiveAgents.Count;
			if (this._isReinforcementCalled && this._isReinforcementInitialized && this.CanProceedToPhase4())
			{
				if (Agent.Main.IsUsingGameObject && Agent.Main.CurrentlyUsedGameObject is StandingPoint && this._playerShip.ShipControllerMachine.PilotStandingPoint == Agent.Main.CurrentlyUsedGameObject)
				{
					this._isPlayerUsingShipAtTheStartOfThePurigCutscene = true;
					this._playerStandingPointAtTheStartOfThePurigCutscene = Agent.Main.CurrentlyUsedGameObject as StandingPoint;
				}
				this._playerShip.ShipOrder.SetShipStopOrder();
				this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase3ToPhase4FadeOut;
			}
			else if ((float)count <= (float)this._phase3TotalEnemyCount * 0.5f)
			{
				if (!this._isReinforcementCalled && !this._isReinforcementInitialized)
				{
					this.CallReinforcement();
				}
				else if (this._isReinforcementCalled && !this._isReinforcementInitialized)
				{
					this.InitializeReinforcement();
				}
			}
			if (this._isReinforcementCalled && this._isReinforcementInitialized)
			{
				if (this._phase3EnemyReinforcementShip1.ShipOrder.MovementOrderEnum != ShipOrder.ShipMovementOrderEnum.Engage)
				{
					this._phase3EnemyReinforcementShip1.ShipOrder.SetShipEngageOrder(true);
				}
				if (this._phase3EnemyReinforcementShip2.ShipOrder.MovementOrderEnum != ShipOrder.ShipMovementOrderEnum.Engage)
				{
					this._phase3EnemyReinforcementShip2.ShipOrder.SetShipEngageOrder(true);
				}
			}
			this.CheckIfEnemyAgentFallIntoTheWater();
			IL_0BF2:
			this.CheckAndPrintInstructionNotification();
			this.HandleGunnarMovement();
			this.HandleIfGunnarFallsIntoTheWater();
		}

		// Token: 0x060006EA RID: 1770 RVA: 0x0002AA28 File Offset: 0x00028C28
		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
			this._navalShipsLogic.ShipAttachmentBrokenEvent -= this.OnAttachmentBroken;
		}

		// Token: 0x060006EB RID: 1771 RVA: 0x0002AA48 File Offset: 0x00028C48
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, blow);
			if (base.Mission.Mode == 4 && this._stealthAgents.Contains(affectedAgent))
			{
				this._stealthAgents.Remove(affectedAgent);
			}
			foreach (MissionShip missionShip in this._navalShipsLogic.AllShips)
			{
				if (((missionShip != this._phase3EnemyReinforcementShip1 && missionShip != this._phase3EnemyReinforcementShip2) || this._isReinforcementInitialized) && missionShip != this._playerShip && this._navalAgentsLogic.GetActiveAgentCountOfShip(missionShip) <= 0 && missionShip.HasController)
				{
					this.DisableShipOrderController(missionShip);
				}
			}
		}

		// Token: 0x060006EC RID: 1772 RVA: 0x0002AB10 File Offset: 0x00028D10
		public override void OnObjectUsed(Agent userAgent, UsableMissionObject usedObject)
		{
			base.OnObjectUsed(userAgent, usedObject);
			if (userAgent.IsMainAgent && usedObject is ShipDoorUsePoint)
			{
				if (this.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1StealthPhase)
				{
					this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoToShipInteriorFadeOut;
					this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoToShipInteriorFadeOut;
					this._phase1EnemyShipToInteriorShipDoorEntity.GetFirstScriptOfType<ShipDoorUsePoint>().SetShipDoorUsePointEnabled(false);
					return;
				}
				if (this.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1ShipInteriorPhase)
				{
					this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoBackToShipFadeOut;
					this._phase1InteriorToEnemyShip3ShipDoorEntity.GetFirstScriptOfType<ShipDoorUsePoint>().SetShipDoorUsePointEnabled(false);
				}
			}
		}

		// Token: 0x060006ED RID: 1773 RVA: 0x0002AB84 File Offset: 0x00028D84
		public override void OnAgentTeamChanged(Team prevTeam, Team newTeam, Agent agent)
		{
			base.OnAgentTeamChanged(prevTeam, newTeam, agent);
			if (newTeam == base.Mission.PlayerEnemyTeam && this.State < Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1ToPhase2FadeOut)
			{
				AgentFlag agentFlags = agent.GetAgentFlags();
				agent.SetAgentFlags(agentFlags | 65536);
				agent.GetComponent<CampaignAgentComponent>().CreateAgentNavigator().AddBehaviorGroup<AlarmedBehaviorGroup>()
					.AddBehavior<CautiousBehavior>();
			}
		}

		// Token: 0x060006EE RID: 1774 RVA: 0x0002ABDC File Offset: 0x00028DDC
		public override void OnEarlyAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if (affectedAgent.IsMainAgent)
			{
				if (this.State <= Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.BossFightConversationInProgress)
				{
					Agent.Main.Health = Agent.Main.HealthLimit;
					this.EndMissionWithAutoContinueFromCheckpoint();
				}
				this.MakeGunnarStopUsingGameObjectBeforeMissionEnd();
			}
			if (this._purigShipAgents.Contains(affectedAgent))
			{
				this._purigShipAgents.Remove(affectedAgent);
			}
		}

		// Token: 0x060006EF RID: 1775 RVA: 0x0002AC36 File Offset: 0x00028E36
		public override InquiryData OnEndMissionRequest(out bool canLeave)
		{
			this.MakeGunnarStopUsingGameObjectBeforeMissionEnd();
			return base.OnEndMissionRequest(ref canLeave);
		}

		// Token: 0x060006F0 RID: 1776 RVA: 0x0002AC48 File Offset: 0x00028E48
		protected override void OnEndMission()
		{
			foreach (MBInformationManager.DialogNotificationHandle dialogNotificationHandle in this._dialogNotificationHandleCache)
			{
				CampaignInformationManager.ClearDialogNotification(dialogNotificationHandle, true);
			}
			this._dialogNotificationHandleCache.Clear();
			this.MakeGunnarStopUsingGameObjectBeforeMissionEnd();
			base.OnEndMission();
			MissionShip playerShip = this._playerShip;
			((Ship)((playerShip != null) ? playerShip.ShipOrigin : null)).Owner = null;
			if (this._phase2AllyShip1 != null)
			{
				((Ship)this._phase2AllyShip1.ShipOrigin).Owner = null;
			}
			if (this._phase2AllyShip2 != null)
			{
				((Ship)this._phase2AllyShip2.ShipOrigin).Owner = null;
			}
			if (this._phase2AllyShip3 != null)
			{
				((Ship)this._phase2AllyShip3.ShipOrigin).Owner = null;
			}
			if (this._phase2AllyShip4 != null)
			{
				((Ship)this._phase2AllyShip4.ShipOrigin).Owner = null;
			}
			if (this._phase2AllyShip5 != null)
			{
				((Ship)this._phase2AllyShip5.ShipOrigin).Owner = null;
			}
		}

		// Token: 0x060006F1 RID: 1777 RVA: 0x0002AD64 File Offset: 0x00028F64
		public override void OnRetreatMission()
		{
			this.MakeGunnarStopUsingGameObjectBeforeMissionEnd();
			base.OnRetreatMission();
		}

		// Token: 0x060006F2 RID: 1778 RVA: 0x0002AD72 File Offset: 0x00028F72
		public override void OnSurrenderMission()
		{
			this.MakeGunnarStopUsingGameObjectBeforeMissionEnd();
			base.OnSurrenderMission();
		}

		// Token: 0x060006F3 RID: 1779 RVA: 0x0002AD80 File Offset: 0x00028F80
		private void DeactivateObjectiveIfItIsActive(MissionObjective objective)
		{
			if (objective != null && objective.IsActive)
			{
				this._missionObjectiveLogic.CompleteCurrentObjective();
			}
		}

		// Token: 0x060006F4 RID: 1780 RVA: 0x0002AD98 File Offset: 0x00028F98
		private void CheckAndPrintInstructionNotification()
		{
			switch (this._instructionState)
			{
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.Approach:
				this.DisplayCurrentInstructionNotification();
				if (this._missionObjectiveLogic != null)
				{
					this._approachObjective = new Quest5ApproachObjective(Mission.Current, this._playerShip, this._approachPointEntity.GetGlobalFrame(), 30f);
					this._missionObjectiveLogic.StartObjective(this._approachObjective);
				}
				this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForJump;
				return;
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForJump:
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForSwim:
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForClearGuards:
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForCheckInterior:
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForTalkSister:
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForReturnToDeck:
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForCutLoose:
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForGunnarUsesShip:
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForEscapeQuietly:
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForReachAllies:
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForDefeatEnemies:
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForDefeatPurigsShip:
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForDefeatPurig:
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForEnd:
				break;
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.Jump:
				this.DisplayCurrentInstructionNotification();
				if (this._missionObjectiveLogic != null)
				{
					this.DeactivateObjectiveIfItIsActive(this._approachObjective);
					this._jumpObjective = new Quest5JumpObjective(Mission.Current, this._gunnarAgent);
					this._missionObjectiveLogic.StartObjective(this._jumpObjective);
				}
				this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForSwim;
				return;
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.Swim:
				this.DisplayCurrentInstructionNotification();
				if (this._missionObjectiveLogic != null)
				{
					this.DeactivateObjectiveIfItIsActive(this._jumpObjective);
					this._swimObjective = new Quest5SwimObjective(Mission.Current, this._gunnarAgent, this._phase1EnemyShip4);
					this._missionObjectiveLogic.StartObjective(this._swimObjective);
				}
				this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForClearGuards;
				return;
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.ClearGuards:
				this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForCheckInterior;
				if (this._missionObjectiveLogic != null)
				{
					this.DeactivateObjectiveIfItIsActive(this._talkWithYourSisterObjective);
					this._clearGuardsObjective = new Quest5ClearGuardsObjective(Mission.Current, this._stealthAgents);
					this._missionObjectiveLogic.StartObjective(this._clearGuardsObjective);
					return;
				}
				break;
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.CheckInterior:
				this.DisplayCurrentInstructionNotification();
				if (this._missionObjectiveLogic != null)
				{
					this.DeactivateObjectiveIfItIsActive(this._swimObjective);
					GameEntity gameEntity = Mission.Current.Scene.FindEntityWithTag("phase_1_interior_player_sp");
					this._checkInteriorObjective = new Quest5CheckInteriorObjective(Mission.Current, this._phase1EnemyShipToInteriorShipDoorEntity, gameEntity);
					this._missionObjectiveLogic.StartObjective(this._checkInteriorObjective);
				}
				this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForTalkSister;
				return;
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.TalkSister:
				if (this._missionObjectiveLogic != null)
				{
					this.DeactivateObjectiveIfItIsActive(this._checkInteriorObjective);
					this._talkWithYourSisterObjective = new Quest5TalkWithYourSisterObjective(Mission.Current, this.SisterAgent);
					this._missionObjectiveLogic.StartObjective(this._talkWithYourSisterObjective);
				}
				this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForReturnToDeck;
				return;
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.ReturnToDeck:
				if (this._missionObjectiveLogic != null)
				{
					this.DeactivateObjectiveIfItIsActive(this._talkWithYourSisterObjective);
					this._returnToDeckObjective = new Quest5ReturnToDeckObjective(Mission.Current, this._phase1InteriorToEnemyShip3ShipDoorEntity, this._phase1EnemyShipToInteriorShipDoorEntity);
					this._missionObjectiveLogic.StartObjective(this._returnToDeckObjective);
				}
				this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForCutLoose;
				return;
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.CutLoose:
				this.DisplayCurrentInstructionNotification();
				if (this._missionObjectiveLogic != null)
				{
					this.DeactivateObjectiveIfItIsActive(this._returnToDeckObjective);
					this._cutLooseObjective = new Quest5CutLooseObjective(base.Mission, this._phase1EnemyShip3.AttachmentMachines, this._phase1EnemyShip3.AttachmentPointMachines);
					this._missionObjectiveLogic.StartObjective(this._cutLooseObjective);
				}
				this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForGunnarUsesShip;
				return;
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.GunnarUsesShip:
				if (this.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase2InProgress)
				{
					this.DisplayCurrentInstructionNotification();
					if (this._missionObjectiveLogic != null)
					{
						this.DeactivateObjectiveIfItIsActive(this._cutLooseObjective);
						this._gunnarUsesShipObjective = new Quest5GunnarUsesShipObjective(Mission.Current);
						this._missionObjectiveLogic.StartObjective(this._gunnarUsesShipObjective);
					}
					this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForEscapeQuietly;
					return;
				}
				break;
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.EscapeQuietly:
				this.DisplayCurrentInstructionNotification();
				if (this._missionObjectiveLogic != null)
				{
					this.DeactivateObjectiveIfItIsActive(this._gunnarUsesShipObjective);
					this._escapeObjective = new Quest5EscapeObjective(Mission.Current, this.GetCurrentGunnarInstructionText(Quest5SetPieceBattleMissionController.Quest5InstructionState.EscapeQuietly));
					this._missionObjectiveLogic.StartObjective(this._escapeObjective);
				}
				this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForReachAllies;
				return;
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.ReachAllies:
				this.DisplayCurrentInstructionNotification();
				if (this._missionObjectiveLogic != null)
				{
					this.DeactivateObjectiveIfItIsActive(this._escapeObjective);
					this._reachAlliesObjective = new Quest5ReachAlliesObjective(Mission.Current, this._phase3TriggerVolumeBox);
					this._missionObjectiveLogic.StartObjective(this._reachAlliesObjective);
				}
				this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForDefeatEnemies;
				return;
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.DefeatEnemies:
				if (this._missionObjectiveLogic != null)
				{
					this.DeactivateObjectiveIfItIsActive(this._reachAlliesObjective);
					this._defeatEnemiesObjective = new Quest5DefeatEnemiesObjective(Mission.Current, this._phase3TotalEnemyCount);
					this._missionObjectiveLogic.StartObjective(this._defeatEnemiesObjective);
				}
				this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForDefeatPurigsShip;
				return;
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.DefeatPurigsShip:
				if (this._missionObjectiveLogic != null)
				{
					this.DeactivateObjectiveIfItIsActive(this._defeatEnemiesObjective);
					this._defeatPurigsShipObjective = new Quest5DefeatPurigsShipObjective(Mission.Current, this._purigShipAgents, this.Phase4PurigShip);
					this._missionObjectiveLogic.StartObjective(this._defeatPurigsShipObjective);
				}
				this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForDefeatPurig;
				return;
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.DefeatPurig:
				if (this._missionObjectiveLogic != null)
				{
					this.DeactivateObjectiveIfItIsActive(this._defeatPurigsShipObjective);
					this._defeatPurigObjective = new Quest5DefeatPurigObjective(Mission.Current, this._purigAgent);
					this._missionObjectiveLogic.StartObjective(this._defeatPurigObjective);
				}
				this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForEnd;
				return;
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.End:
				this.DeactivateObjectiveIfItIsActive(this._defeatPurigObjective);
				break;
			default:
				return;
			}
		}

		// Token: 0x060006F5 RID: 1781 RVA: 0x0002B264 File Offset: 0x00029464
		private TextObject GetCurrentGunnarInstructionText(Quest5SetPieceBattleMissionController.Quest5InstructionState instructionState)
		{
			switch (instructionState)
			{
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.Approach:
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForJump:
				return new TextObject("{=Gap3mlD3}Do you see that big cluster of ships back there? That's got to be where they're holding the prisoners.", null);
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.Jump:
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForSwim:
				return new TextObject("{=DQNbUvkL}Into the water! Let's go, while Purig's men are distracted. Swim fast, but keep your distance from any lookouts.", null);
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.ClearGuards:
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForCheckInterior:
				return new TextObject("{=uQjanqh7}Be careful of the guards! Try to take them out without raising an alarm.", null);
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.CheckInterior:
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForCutLoose:
				return new TextObject("{=vOXiHDxu}Very good! Now, get to the hold.", null);
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.CutLoose:
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForGunnarUsesShip:
				return new TextObject("{=Ju7ku4LZ}Well done! But your sister is still within, and we need to get her to safety. Cut the lines tying us to the other ship, and let's be away.", null);
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.GunnarUsesShip:
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForEscapeQuietly:
				return new TextObject("{=P1nDlx4L}Good work! Now, let's get back to our people. The wind and current are in our favor. Even though it's just the two of us, I think we can rejoin Bjolgur and Lahar before they catch us. I'll look to the sails [and take the helm], and you can cut us loose.", null);
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.EscapeQuietly:
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForReachAllies:
				return new TextObject("{=wnhaoGoW}Gods' blood! We can't get past them! They're going to board. Shoot those bastards, cut them down as they come over the side, whatever it takes!", null);
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.ReachAllies:
			case Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForDefeatEnemies:
				return new TextObject("{=igHojAHJ}Hah! We went through their net like a slippery old eel. Bjolgur and Lahar are right over there. Let's turn the tables on those bastards!", null);
			}
			return TextObject.GetEmpty();
		}

		// Token: 0x060006F6 RID: 1782 RVA: 0x0002B368 File Offset: 0x00029568
		private void DisplayCurrentInstructionNotification()
		{
			TextObject currentGunnarInstructionText = this.GetCurrentGunnarInstructionText(this._instructionState);
			if (!currentGunnarInstructionText.IsEmpty())
			{
				MBInformationManager.DialogNotificationHandle dialogNotificationHandle = CampaignInformationManager.AddDialogLine(currentGunnarInstructionText, NavalStorylineData.Gunnar.CharacterObject, null, 1000, 3);
				this._dialogNotificationHandleCache.Add(dialogNotificationHandle);
			}
		}

		// Token: 0x060006F7 RID: 1783 RVA: 0x0002B3B0 File Offset: 0x000295B0
		private void HandleGunnarMovement()
		{
			switch (this._gunnarMovementState)
			{
			case Quest5SetPieceBattleMissionController.GunnarMovementState.None:
			case Quest5SetPieceBattleMissionController.GunnarMovementState.End:
				break;
			case Quest5SetPieceBattleMissionController.GunnarMovementState.GoToInitialJumpingPosition:
			{
				Agent gunnarAgent = this._gunnarAgent;
				if (gunnarAgent != null && gunnarAgent.IsUsingGameObject)
				{
					this._gunnarAgent.StopUsingGameObjectMT(true, 1);
				}
				this.EnableRamp();
				this._gunnarAgent.ClearTargetFrame();
				new WorldPosition(base.Mission.Scene, this.JumpOffInitialPosition.GlobalPosition);
				Vec3 vec = this.JumpOffInitialPosition.GlobalPosition - this._gunnarAgent.Position;
				Agent gunnarAgent2 = this._gunnarAgent;
				Vec3 vec2 = this.JumpOffInitialPosition.GlobalPosition;
				Vec2 vec3 = vec2.AsVec2;
				gunnarAgent2.SetTargetPositionAndDirection(ref vec3, ref vec);
				this._gunnarAgent.LookDirection = vec.NormalizedCopy();
				this._gunnarMovementState = Quest5SetPieceBattleMissionController.GunnarMovementState.WaitForReachingInitialJumpingPosition;
				return;
			}
			case Quest5SetPieceBattleMissionController.GunnarMovementState.WaitForReachingInitialJumpingPosition:
			{
				Vec3 vec2 = this._gunnarAgent.Position;
				Vec3 vec4 = this.JumpOffInitialPosition.GlobalPosition;
				if (vec2.NearlyEquals(ref vec4, 1f))
				{
					this._gunnarMovementState = Quest5SetPieceBattleMissionController.GunnarMovementState.GoToJumpingTargetPosition;
					return;
				}
				Vec3 vec5 = this.JumpOffInitialPosition.GlobalPosition - this._gunnarAgent.Position;
				Agent gunnarAgent3 = this._gunnarAgent;
				vec4 = this.JumpOffInitialPosition.GlobalPosition;
				Vec2 vec3 = vec4.AsVec2;
				gunnarAgent3.SetTargetPositionAndDirection(ref vec3, ref vec5);
				return;
			}
			case Quest5SetPieceBattleMissionController.GunnarMovementState.GoToJumpingTargetPosition:
			{
				this._gunnarAgent.ClearTargetFrame();
				new WorldPosition(base.Mission.Scene, this.JumpOffTargetPosition.GlobalPosition);
				Vec3 vec6 = this.JumpOffTargetPosition.GlobalPosition - this._gunnarAgent.Position;
				Agent gunnarAgent4 = this._gunnarAgent;
				Vec3 vec4 = this.JumpOffTargetPosition.GlobalPosition;
				Vec2 vec3 = vec4.AsVec2;
				gunnarAgent4.SetTargetPositionAndDirection(ref vec3, ref vec6);
				this._gunnarAgent.LookDirection = vec6.NormalizedCopy();
				this._gunnarMovementState = Quest5SetPieceBattleMissionController.GunnarMovementState.WaitForReachingJumpingTargetPosition;
				return;
			}
			case Quest5SetPieceBattleMissionController.GunnarMovementState.WaitForReachingJumpingTargetPosition:
			{
				Vec3 vec4 = this._gunnarAgent.Position;
				Vec3 vec2 = this.JumpOffTargetPosition.GlobalPosition;
				if (vec4.NearlyEquals(ref vec2, 3f))
				{
					if (Agent.Main.IsInWater())
					{
						this._gunnarMovementState = Quest5SetPieceBattleMissionController.GunnarMovementState.SwimToTheHidingSpot;
					}
					else
					{
						Agent gunnarAgent5 = this._gunnarAgent;
						vec2 = this._gunnarAgent.Position;
						gunnarAgent5.SetTargetPosition(vec2.AsVec2);
					}
				}
				this._gunnarAgentNavalComponent.SetCanDrown(false);
				return;
			}
			case Quest5SetPieceBattleMissionController.GunnarMovementState.SwimToTheHidingSpot:
			{
				Agent gunnarAgent6 = this._gunnarAgent;
				if (gunnarAgent6 != null && gunnarAgent6.IsUsingGameObject)
				{
					this._gunnarAgent.StopUsingGameObjectMT(true, 1);
				}
				this._gunnarAgent.ClearTargetFrame();
				Vec3 vec7 = this.HidingSpot1Position.GlobalPosition - this._gunnarAgent.Position;
				Agent gunnarAgent7 = this._gunnarAgent;
				Vec3 vec2 = this.HidingSpot1Position.GlobalPosition;
				Vec2 vec3 = vec2.AsVec2;
				gunnarAgent7.SetTargetPositionAndDirection(ref vec3, ref vec7);
				this._gunnarAgent.LookDirection = vec7.NormalizedCopy();
				this._targetClimbingMachine = this._phase1EnemyShip4.ClimbingMachines.First<ClimbingMachine>();
				this._gunnarMovementStateForClimbingShip = Quest5SetPieceBattleMissionController.GunnarMovementStateForClimbingShip.Start;
				this._gunnarMovementState = Quest5SetPieceBattleMissionController.GunnarMovementState.WaitForTeleportingToTheHidingSpot;
				return;
			}
			case Quest5SetPieceBattleMissionController.GunnarMovementState.WaitForTeleportingToTheHidingSpot:
				this.MakeGunnarClimbToDeck();
				if (this._gunnarMovementStateForClimbingShip == Quest5SetPieceBattleMissionController.GunnarMovementStateForClimbingShip.End)
				{
					this._gunnarAgent.SetCrouchMode(true);
					this._gunnarAgent.Controller = 0;
					this._gunnarAgent.SetActionChannel(0, ref ActionIndexCache.act_crouch_walk_idle_unarmed, false, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
					this._gunnarMovementState = Quest5SetPieceBattleMissionController.GunnarMovementState.WaitAtTheHidingSpot;
					return;
				}
				break;
			case Quest5SetPieceBattleMissionController.GunnarMovementState.TeleportToTargetPosition:
			{
				Vec3 globalPosition = this.HidingSpot1Position.GlobalPosition;
				this._gunnarAgent.TeleportToPosition(globalPosition);
				Agent gunnarAgent8 = this._gunnarAgent;
				Vec2 vec3 = globalPosition.AsVec2;
				Vec3 vec2 = globalPosition - this._gunnarAgent.Position;
				gunnarAgent8.SetTargetPositionAndDirection(ref vec3, ref vec2);
				this._gunnarAgent.SetCrouchMode(true);
				this._gunnarAgent.Controller = 0;
				this._gunnarAgent.SetActionChannel(0, ref ActionIndexCache.act_crouch_walk_idle_unarmed, false, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
				this._gunnarMovementState = Quest5SetPieceBattleMissionController.GunnarMovementState.WaitAtTheHidingSpot;
				return;
			}
			case Quest5SetPieceBattleMissionController.GunnarMovementState.WaitAtTheHidingSpot:
			{
				this._gunnarAgent.SetCrouchMode(true);
				Agent gunnarAgent9 = this._gunnarAgent;
				Vec3 vec2 = this.HidingSpot1Position.GlobalPosition;
				gunnarAgent9.SetTargetPosition(vec2.AsVec2);
				this._gunnarAgent.SetActionChannel(0, ref ActionIndexCache.act_crouch_walk_idle_unarmed, false, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
				if (Extensions.IsEmpty<Agent>(this._stealthAgents))
				{
					this._gunnarAgent.ClearTargetFrame();
					this._gunnarAgent.SetCrouchMode(false);
					this._gunnarAgent.Controller = 1;
					this._gunnarAgent.SetActionChannel(0, ref ActionIndexCache.act_none, true, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
					this._gunnarMovementState = Quest5SetPieceBattleMissionController.GunnarMovementState.GoToTheEscapeShip;
				}
				this.CheckIfAnEnemyIsAttackingGunnar();
				return;
			}
			case Quest5SetPieceBattleMissionController.GunnarMovementState.GoToTheEscapeShip:
			{
				this._gunnarAgent.ClearTargetFrame();
				WorldPosition worldPosition;
				worldPosition..ctor(base.Mission.Scene, this.GunnarShipUsePosition.origin);
				Vec3 vec8 = this.GunnarShipUsePosition.origin - this._gunnarAgent.Position;
				this._gunnarAgent.SetScriptedPositionAndDirection(ref worldPosition, MBMath.ToRadians(vec8.RotationX), false, 0);
				this._gunnarAgent.LookDirection = vec8.NormalizedCopy();
				this._gunnarMovementState = Quest5SetPieceBattleMissionController.GunnarMovementState.WaitForReachingToTheEscapeShip;
				return;
			}
			case Quest5SetPieceBattleMissionController.GunnarMovementState.WaitForReachingToTheEscapeShip:
				if ((this._phase1EnemyShip2 == null || !this.EscapeShip.GetIsThereActiveBridgeTo(this._phase1EnemyShip2)) && this.EscapeShip.Captain == this._gunnarAgent && this.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase2InProgress && this._gunnarAgent.CurrentlyUsedGameObject == this.EscapeShip.ShipControllerMachine.PilotStandingPoint)
				{
					this._gunnarMovementState = Quest5SetPieceBattleMissionController.GunnarMovementState.UseTheEscapeShip;
					return;
				}
				break;
			case Quest5SetPieceBattleMissionController.GunnarMovementState.UseTheEscapeShip:
				this.HandleEscapeShipMovement();
				this.EscapeShip.Formation.SetControlledByAI(false, false);
				break;
			default:
				return;
			}
		}

		// Token: 0x060006F8 RID: 1784 RVA: 0x0002B979 File Offset: 0x00029B79
		private void EnableRamp()
		{
			Mission.Current.Scene.FindEntityWithTag("ramp_holder").SetVisibilityExcludeParents(true);
		}

		// Token: 0x060006F9 RID: 1785 RVA: 0x0002B998 File Offset: 0x00029B98
		private void HandleIfGunnarFallsIntoTheWater()
		{
			if (this._gunnarAgent != null && this._gunnarAgent.IsActive())
			{
				Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState state = this.State;
				if (state != Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1StealthPhase)
				{
					if (state != Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1EscapePhase && state != Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase2InProgress)
					{
						return;
					}
					if (this._gunnarFellIntoTheWaterTimer == null)
					{
						if (this._gunnarAgent.IsInWater())
						{
							this._gunnarFellIntoTheWaterTimer = new MissionTimer(10f);
							return;
						}
					}
					else if (this._gunnarFellIntoTheWaterTimer.Check(false))
					{
						Vec3 vec = this._gunnarAgent.Position - this.GunnarShipUsePosition.origin;
						if (vec.LengthSquared > 1f)
						{
							this._gunnarAgent.TeleportToPosition(this.GunnarShipUsePosition.origin);
						}
					}
				}
				else if (this._gunnarFellIntoTheWaterTimer == null)
				{
					if (this._gunnarAgent.IsInWater())
					{
						this._gunnarFellIntoTheWaterTimer = new MissionTimer(10f);
						return;
					}
				}
				else if (this._gunnarFellIntoTheWaterTimer.Check(false) && !Extensions.IsEmpty<Agent>(this._stealthAgents))
				{
					Vec3 globalPosition = this.HidingSpot1Position.GlobalPosition;
					Vec3 vec = this._gunnarAgent.Position - globalPosition;
					if (vec.LengthSquared > 1f)
					{
						this._gunnarAgent.TeleportToPosition(globalPosition);
					}
					Agent gunnarAgent = this._gunnarAgent;
					Vec2 asVec = globalPosition.AsVec2;
					vec = this.GunnarShipUsePosition.origin - this._gunnarAgent.Position;
					vec = vec.NormalizedCopy();
					gunnarAgent.SetTargetPositionAndDirection(ref asVec, ref vec);
					this._gunnarAgent.SetCrouchMode(true);
					return;
				}
			}
		}

		// Token: 0x060006FA RID: 1786 RVA: 0x0002BB1C File Offset: 0x00029D1C
		private void MakeGunnarClimbToDeck()
		{
			switch (this._gunnarMovementStateForClimbingShip)
			{
			case Quest5SetPieceBattleMissionController.GunnarMovementStateForClimbingShip.None:
			case Quest5SetPieceBattleMissionController.GunnarMovementStateForClimbingShip.End:
				break;
			case Quest5SetPieceBattleMissionController.GunnarMovementStateForClimbingShip.Start:
			{
				WorldPosition worldPosition;
				worldPosition..ctor(base.Mission.Scene, this._targetClimbingMachine.PilotStandingPoint.GameEntity.GlobalPosition);
				this._gunnarAgent.SetScriptedPosition(ref worldPosition, true, 0);
				this._gunnarMovementStateForClimbingShip = Quest5SetPieceBattleMissionController.GunnarMovementStateForClimbingShip.GoingToTheTargetClimbingMachine;
				return;
			}
			case Quest5SetPieceBattleMissionController.GunnarMovementStateForClimbingShip.GoingToTheTargetClimbingMachine:
				if (this._gunnarAgent.Position.Distance(this._targetClimbingMachine.GameEntity.GlobalPosition) < 2.5f)
				{
					this._gunnarMovementStateForClimbingShip = Quest5SetPieceBattleMissionController.GunnarMovementStateForClimbingShip.TargetReached;
					return;
				}
				if (this._phase1EnemyShip4.GetIsAgentOnShip(this._gunnarAgent, false))
				{
					this._gunnarAgent.SetCrouchMode(true);
					this._gunnarMovementStateForClimbingShip = Quest5SetPieceBattleMissionController.GunnarMovementStateForClimbingShip.OnDeck;
					return;
				}
				this._gunnarAgent.SetTargetPosition(this._targetClimbingMachine.PilotStandingPoint.GameEntity.GlobalPosition.AsVec2);
				return;
			case Quest5SetPieceBattleMissionController.GunnarMovementStateForClimbingShip.TargetReached:
				if (!this._targetClimbingMachine.PilotStandingPoint.HasUser)
				{
					this._gunnarAgent.UseGameObject(this._targetClimbingMachine.PilotStandingPoint, -1);
					this._gunnarMovementStateForClimbingShip = Quest5SetPieceBattleMissionController.GunnarMovementStateForClimbingShip.UsingClimbingMachine;
					return;
				}
				break;
			case Quest5SetPieceBattleMissionController.GunnarMovementStateForClimbingShip.UsingClimbingMachine:
				if (this._gunnarAgent.Position.Distance(this._targetClimbingMachine.GameEntity.GlobalPosition) > 2.5f)
				{
					if (!this._gunnarAgent.IsUsingGameObject)
					{
						this._gunnarMovementStateForClimbingShip = Quest5SetPieceBattleMissionController.GunnarMovementStateForClimbingShip.GoingToTheTargetClimbingMachine;
						return;
					}
				}
				else if (this._phase1EnemyShip4.GetIsAgentOnShip(this._gunnarAgent, false))
				{
					this._gunnarAgent.SetCrouchMode(true);
					this._gunnarMovementStateForClimbingShip = Quest5SetPieceBattleMissionController.GunnarMovementStateForClimbingShip.OnDeck;
					return;
				}
				break;
			case Quest5SetPieceBattleMissionController.GunnarMovementStateForClimbingShip.OnDeck:
			{
				this._gunnarAgent.ClearTargetFrame();
				Vec3 vec = this.HidingSpot1Position.GlobalPosition - this._gunnarAgent.Position;
				Agent gunnarAgent = this._gunnarAgent;
				Vec2 asVec = this.HidingSpot1Position.GlobalPosition.AsVec2;
				gunnarAgent.SetTargetPositionAndDirection(ref asVec, ref vec);
				this._gunnarMovementStateForClimbingShip = Quest5SetPieceBattleMissionController.GunnarMovementStateForClimbingShip.GoToFinalTargetPoint;
				return;
			}
			case Quest5SetPieceBattleMissionController.GunnarMovementStateForClimbingShip.GoToFinalTargetPoint:
			{
				Vec3 position = this._gunnarAgent.Position;
				Vec3 globalPosition = this.HidingSpot1Position.GlobalPosition;
				if (position.NearlyEquals(ref globalPosition, 1f))
				{
					this._gunnarMovementStateForClimbingShip = Quest5SetPieceBattleMissionController.GunnarMovementStateForClimbingShip.End;
				}
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x060006FB RID: 1787 RVA: 0x0002BD50 File Offset: 0x00029F50
		private void InitializePhase1Part1()
		{
			TeamAINavalComponent teamAINavalComponent = new TeamAINavalComponent(base.Mission, base.Mission.AttackerTeam, 5f, 1f);
			base.Mission.AttackerTeam.AddTeamAI(teamAINavalComponent, false);
			base.Mission.AttackerTeam.AddTacticOption(new TacticNavalBalancedOffense(base.Mission.AttackerTeam));
			TeamAINavalComponent teamAINavalComponent2 = new TeamAINavalComponent(base.Mission, base.Mission.DefenderTeam, 5f, 1f);
			base.Mission.DefenderTeam.AddTeamAI(teamAINavalComponent2, false);
			base.Mission.DefenderTeam.AddTacticOption(new TacticNavalBalancedOffense(base.Mission.DefenderTeam));
			this._playerShip = this.CreateShip("crusas_roundship_nested_q5", "phase_1_player_ship_sp", this._playerFormation, false, null, null, true);
			this._phase1EnemyShip1 = this.CreateShip("sturgia_heavy_ship", "phase_1_enemy_ship_1_sp_initial", this.GetAvailableEnemyFormation(), false, null, null, false);
			this._phase1EnemyShip2 = this.CreateShip("ship_lodya_storyline", "phase_1_enemy_ship_2_sp", this.GetAvailableEnemyFormation(), true, this._phase1EnemyShip2UpgradePieceList, null, false);
			this._phase1EnemyShip3 = this.CreateShip("ship_dromon_storyline", "phase_1_enemy_ship_3_sp", this.GetAvailableEnemyFormation(), true, this._escapeShipUpgradePieceList, null, false);
			this._phase1EnemyShip4 = this.CreateShip("ship_birlinn_storyline", "phase_1_enemy_ship_4_sp", this.GetAvailableEnemyFormation(), true, null, null, false);
			this._phase1EnemyShip1.SetCanBeTakenOver(false);
			this._phase1EnemyShip2.SetCanBeTakenOver(false);
			this._phase1EnemyShip3.SetCanBeTakenOver(false);
			this._phase1EnemyShip4.SetCanBeTakenOver(false);
			this._phase1EnemyShipToInteriorShipDoorEntity = Mission.Current.Scene.FindEntityWithTag("phase_1_enemy_ship_3_to_interior_door_tag");
			this._phase1EnemyShipToInteriorShipDoorEntity.GetFirstScriptOfType<ShipDoorUsePoint>().SetShipDoorUsePointEnabled(false);
			this.HandleStealthShipsBridgeConnections();
			base.Mission.SetMissionMode(4, true);
			foreach (ShipAttachmentMachine shipAttachmentMachine in this._phase1EnemyShip3.AttachmentMachines)
			{
				if (!shipAttachmentMachine.GameEntity.Parent.HasTag("bridge_a") && !shipAttachmentMachine.GameEntity.Parent.HasTag("bridge_b"))
				{
					foreach (StandingPoint standingPoint in shipAttachmentMachine.StandingPoints)
					{
						standingPoint.IsDisabledForPlayers = true;
					}
				}
			}
		}

		// Token: 0x060006FC RID: 1788 RVA: 0x0002BFE4 File Offset: 0x0002A1E4
		private void CheckIfAnEnemyIsAttackingGunnar()
		{
			if (!this._isMissionFailPopUpTriggered)
			{
				bool flag = false;
				foreach (Agent agent in this._stealthAgents)
				{
					if (agent.IsAlarmed() && agent.Position.Distance(this._gunnarAgent.Position) < 2f)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					this.TriggerMissionFailPopup();
				}
			}
		}

		// Token: 0x060006FD RID: 1789 RVA: 0x0002C070 File Offset: 0x0002A270
		private void InitializePhase1Part2()
		{
			this._phase1PlayerShipSpawnPosition = this._playerShip.GlobalFrame.origin;
			this._phase1EnemyShip1.SetAnchor(true, false, 1f);
			this._phase1EnemyShip1.ShipOrder.SetShipStopOrder();
			this._phase1EnemyShip1.SetController(ShipControllerType.AI, true);
			this._phase1EnemyShip1.SetShipOrderActive(false);
			this._phase1EnemyShip2.SetAnchor(true, false, 1f);
			this._phase1EnemyShip2.ShipOrder.SetShipStopOrder();
			this._phase1EnemyShip2.SetController(ShipControllerType.AI, true);
			this._phase1EnemyShip2.SetShipOrderActive(false);
			this._phase1EnemyShip3.SetAnchor(true, false, 1f);
			this._phase1EnemyShip3.ShipOrder.SetShipStopOrder();
			this._phase1EnemyShip3.SetController(ShipControllerType.AI, true);
			this._phase1EnemyShip3.SetShipOrderActive(false);
			this._phase1EnemyShip4.SetAnchor(true, false, 1f);
			this._phase1EnemyShip4.ShipOrder.SetShipStopOrder();
			this._phase1EnemyShip4.SetController(ShipControllerType.AI, true);
			this._phase1EnemyShip4.SetShipOrderActive(false);
			foreach (ShipAttachmentMachine shipAttachmentMachine in this._playerShip.AttachmentMachines)
			{
				foreach (StandingPoint standingPoint in shipAttachmentMachine.StandingPoints)
				{
					standingPoint.IsDisabledForPlayers = true;
				}
			}
			foreach (ShipAttachmentMachine shipAttachmentMachine2 in this._phase1EnemyShip1.AttachmentMachines)
			{
				foreach (StandingPoint standingPoint2 in shipAttachmentMachine2.StandingPoints)
				{
					standingPoint2.IsDisabledForPlayers = true;
				}
			}
			foreach (ShipAttachmentMachine shipAttachmentMachine3 in this._phase1EnemyShip2.AttachmentMachines)
			{
				if (!shipAttachmentMachine3.GameEntity.Parent.HasTag("bridge_a") && !shipAttachmentMachine3.GameEntity.Parent.HasTag("bridge_b") && !shipAttachmentMachine3.GameEntity.Parent.HasTag("bridge_c"))
				{
					foreach (StandingPoint standingPoint3 in shipAttachmentMachine3.StandingPoints)
					{
						standingPoint3.IsDisabledForPlayers = true;
					}
				}
			}
			foreach (ClimbingMachine climbingMachine in this._phase1EnemyShip1.ClimbingMachines)
			{
				foreach (StandingPoint standingPoint4 in climbingMachine.StandingPoints)
				{
					standingPoint4.IsDisabledForPlayers = true;
				}
			}
			this._navalAgentsLogic.SetDeploymentMode(true);
			this._navalShipsLogic.SetDeploymentMode(true);
			this.SpawnPhase1AllyTroops();
			this.SpawnPhase1EnemyTroops();
			base.Mission.PlayerTeam.SetPlayerRole(true, true);
			Agent.Main.SetClothingColor1(4279111698U);
			Agent.Main.SetClothingColor2(4279111698U);
			Agent.Main.UpdateSpawnEquipmentAndRefreshVisuals(this.GetScriptedStealthEquipment());
			this._gunnarAgent.UpdateSpawnEquipmentAndRefreshVisuals(this.GetScriptedStealthEquipment());
			this._navalAgentsLogic.AssignCaptainToShipForDeploymentMode(Agent.Main, this._playerShip, this._playerShip);
			this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this._phase1EnemyShip1);
			this._navalAgentsLogic.SetDeploymentMode(false);
			this._navalShipsLogic.SetDeploymentMode(false);
			Mission.Current.OnDeploymentFinished();
			Mission.Current.Scene.FindEntityWithTag("phase_2_barricade").SetVisibilityExcludeParents(false);
			this.RemoveShipControlPointDescriptionOfAllEnemyShips();
		}

		// Token: 0x060006FE RID: 1790 RVA: 0x0002C4C0 File Offset: 0x0002A6C0
		private void SpawnPhase1AllyTroops()
		{
			this._navalAgentsLogic.AddReservedTroopToShip(new PartyAgentOrigin(PartyBase.MainParty, CharacterObject.PlayerCharacter, -1, default(UniqueTroopDescriptor), false, false), this._playerShip);
			this._navalAgentsLogic.AddReservedTroopToShip(new PartyAgentOrigin(PartyBase.MainParty, Hero.MainHero.Culture.BasicTroop, -1, default(UniqueTroopDescriptor), false, false), this._playerShip);
			this._navalAgentsLogic.AddReservedTroopToShip(new PartyAgentOrigin(PartyBase.MainParty, Hero.MainHero.Culture.BasicTroop, -1, default(UniqueTroopDescriptor), false, false), this._playerShip);
			this._navalAgentsLogic.AddReservedTroopToShip(new PartyAgentOrigin(PartyBase.MainParty, Hero.MainHero.Culture.BasicTroop, -1, default(UniqueTroopDescriptor), false, false), this._playerShip);
			this._navalAgentsLogic.SpawnNextBatch(0, false, null);
			this.SpawnGunnarOnShip(this._playerShip);
			this.SpawnCrusasOnShip(this._playerShip);
			this._crusasAgent.UpdateSpawnEquipmentAndRefreshVisuals(MBObjectManager.Instance.GetObject<MBEquipmentRoster>("npc_merchant_equipment_empire").DefaultEquipment);
			this._gunnarAgent.SetMortalityState(2);
			this._playerShip.Formation.PlayerOwner = Agent.Main;
		}

		// Token: 0x060006FF RID: 1791 RVA: 0x0002C608 File Offset: 0x0002A808
		private void SpawnPhase1EnemyTroops()
		{
			base.Mission.Scene.GetAllEntitiesWithScriptComponent<DynamicPatrolAreaParent>(ref this._dynamicPatrolAreas);
			CharacterObject @object = MBObjectManager.Instance.GetObject<CharacterObject>("sea_hound_captivity");
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._phase1EnemyShip1, 7);
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._phase1EnemyShip2, 6);
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._phase1EnemyShip3, 100);
			Vec2 vec;
			foreach (GameEntity gameEntity in this._dynamicPatrolAreas)
			{
				if (!gameEntity.GetFirstScriptOfType<DynamicPatrolAreaParent>().IsDisabled)
				{
					IEnumerable<GameEntity> children = gameEntity.GetChildren();
					bool flag = false;
					MissionShip shipOfDynamicPartolArea = this.GetShipOfDynamicPartolArea(gameEntity);
					foreach (GameEntity gameEntity2 in children)
					{
						PatrolPoint firstScriptOfType = gameEntity2.GetChild(0).GetFirstScriptOfType<PatrolPoint>();
						shipOfDynamicPartolArea.Formation.JoinDetachment(gameEntity2.GetFirstScriptOfType<UsablePlace>());
						if (firstScriptOfType != null && !flag && !firstScriptOfType.IsDisabled && !string.IsNullOrEmpty(firstScriptOfType.SpawnGroupTag))
						{
							Equipment equipment = Extensions.GetRandomElementInefficiently<Equipment>(@object.BattleEquipments).Clone(false);
							for (int i = 0; i < 12; i++)
							{
								if ((i == 0 || i == 1 || i == 2 || i == 3 || i == 4) && !equipment[i].IsEmpty && equipment[i].Item.WeaponComponent != null && equipment[i].Item.WeaponComponent.PrimaryWeapon.IsShield)
								{
									equipment[i] = EquipmentElement.Invalid;
								}
							}
							AgentBuildData agentBuildData = new AgentBuildData(@object).TroopOrigin(new SimpleAgentOrigin(@object, -1, null, default(UniqueTroopDescriptor))).Team(base.Mission.PlayerEnemyTeam).InitialPosition(ref gameEntity2.GetGlobalFrame().origin);
							vec = gameEntity2.GetGlobalFrame().rotation.f.AsVec2;
							AgentBuildData agentBuildData2 = agentBuildData.InitialDirection(ref vec).NoHorses(true).NoWeapons(false)
								.Equipment(equipment);
							Agent agent = base.Mission.SpawnAgent(agentBuildData2, false);
							MBActionSet actionSet = MBGlobals.GetActionSet("as_human_hideout_bandit");
							AnimationSystemData animationSystemData = MonsterExtensions.FillAnimationSystemData(agentBuildData2.AgentMonster, actionSet, @object.GetStepSize(), false);
							agent.SetActionSet(ref animationSystemData);
							AgentFlag agentFlags = agent.GetAgentFlags();
							agent.SetAgentFlags(agentFlags | 65536);
							AgentNavigator agentNavigator = agent.GetComponent<CampaignAgentComponent>().CreateAgentNavigator();
							agentNavigator.AddBehaviorGroup<AlarmedBehaviorGroup>().AddBehavior<CautiousBehavior>();
							agentNavigator.AddBehaviorGroup<DailyBehaviorGroup>().AddBehavior<PatrolAgentBehavior>().SetDynamicPatrolArea(gameEntity);
							this._stealthAgents.Add(agent);
							flag = true;
						}
					}
				}
			}
			MatrixFrame globalFrame = this._phase1EnemyShip1.ShipControllerMachine.PilotStandingPoint.GameEntity.GetGlobalFrame();
			AgentBuildData agentBuildData3 = new AgentBuildData(this._slaveTraderCharacter).TroopOrigin(new SimpleAgentOrigin(this._slaveTraderCharacter, -1, null, default(UniqueTroopDescriptor))).Team(base.Mission.PlayerEnemyTeam).InitialPosition(ref globalFrame.origin);
			vec = globalFrame.rotation.f.AsVec2;
			AgentBuildData agentBuildData4 = agentBuildData3.InitialDirection(ref vec).NoHorses(true).NoWeapons(false);
			this._slaveTraderAgent = base.Mission.SpawnAgent(agentBuildData4, false);
			this._navalAgentsLogic.AddAgentToShip(this._slaveTraderAgent, this._phase1EnemyShip1);
			MBActionSet actionSet2 = MBGlobals.GetActionSet("as_human_hideout_bandit");
			AnimationSystemData animationSystemData2 = MonsterExtensions.FillAnimationSystemData(agentBuildData4.AgentMonster, actionSet2, @object.GetStepSize(), false);
			this._slaveTraderAgent.SetActionSet(ref animationSystemData2);
			this._slaveTraderAgent.SetAgentFlags(this._slaveTraderAgent.GetAgentFlags() & -65561);
			Queue<MatrixFrame> queue = new Queue<MatrixFrame>();
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this._phase1EnemyShip1.AttachmentPointMachines)
			{
				queue.Enqueue(shipAttachmentPointMachine.StandingPoints.First<StandingPoint>().GameEntity.GetGlobalFrame());
			}
			for (int j = 0; j < this._slaveTraderShipOarsmen.Length; j++)
			{
				MatrixFrame matrixFrame = queue.Dequeue();
				AgentBuildData agentBuildData5 = new AgentBuildData(this._slaveTraderCharacter).TroopOrigin(new SimpleAgentOrigin(this._slaveTraderCharacter, -1, null, default(UniqueTroopDescriptor))).Team(base.Mission.PlayerEnemyTeam).InitialPosition(ref matrixFrame.origin);
				vec = matrixFrame.rotation.f.AsVec2;
				AgentBuildData agentBuildData6 = agentBuildData5.InitialDirection(ref vec).NoHorses(true).NoWeapons(false);
				Agent agent2 = base.Mission.SpawnAgent(agentBuildData6, false);
				this._slaveTraderShipOarsmen[j] = agent2;
				this._navalAgentsLogic.AddAgentToShip(agent2, this._phase1EnemyShip1);
				agent2.SetActionSet(ref animationSystemData2);
				agent2.SetAgentFlags(agent2.GetAgentFlags() & -65561);
			}
		}

		// Token: 0x06000700 RID: 1792 RVA: 0x0002CB70 File Offset: 0x0002AD70
		private void DisableSlaveTraderShipAgents()
		{
			this._slaveTraderAgent.SetTeam(Team.Invalid, true);
			for (int i = 0; i < this._slaveTraderShipOarsmen.Length; i++)
			{
				this._slaveTraderShipOarsmen[i].SetTeam(Team.Invalid, true);
			}
		}

		// Token: 0x06000701 RID: 1793 RVA: 0x0002CBB4 File Offset: 0x0002ADB4
		private MissionShip GetShipOfDynamicPartolArea(GameEntity dynamicPatrolArea)
		{
			if (dynamicPatrolArea.Parent.Parent.Name.Equals(this._phase1EnemyShip2.MissionShipObject.Prefab))
			{
				return this._phase1EnemyShip2;
			}
			if (dynamicPatrolArea.Parent.Parent.Name.Equals(this._phase1EnemyShip3.MissionShipObject.Prefab))
			{
				return this._phase1EnemyShip3;
			}
			if (dynamicPatrolArea.Parent.Parent.Name.Equals(this._phase1EnemyShip4.MissionShipObject.Prefab))
			{
				return this._phase1EnemyShip4;
			}
			return null;
		}

		// Token: 0x06000702 RID: 1794 RVA: 0x0002CC4C File Offset: 0x0002AE4C
		private void HandleStealthShipsBridgeConnections()
		{
			if (this._phase1EnemyShip2 != null && this._phase1EnemyShip3 != null && this._phase1EnemyShip4 != null && !this._talkedWithSister)
			{
				this._phase1EnemyShip3.TryToMaintainConnectionToAnotherShip(this._phase1EnemyShip2, true, true);
				this._phase1EnemyShip4.TryToMaintainConnectionToAnotherShip(this._phase1EnemyShip2, true, true);
			}
		}

		// Token: 0x06000703 RID: 1795 RVA: 0x0002CC9F File Offset: 0x0002AE9F
		private void HandleEscapeShipInteriorDoorUsage()
		{
			this._phase1EnemyShipToInteriorShipDoorEntity.GetFirstScriptOfType<ShipDoorUsePoint>().SetShipDoorUsePointEnabled(Extensions.IsEmpty<Agent>(this._stealthAgents));
		}

		// Token: 0x06000704 RID: 1796 RVA: 0x0002CCBC File Offset: 0x0002AEBC
		private void OnPlayerShipReachedApproachDistance()
		{
			this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1SwimmingPhase;
			this._gunnarMovementState = Quest5SetPieceBattleMissionController.GunnarMovementState.GoToInitialJumpingPosition;
			if (Agent.Main.IsUsingGameObject)
			{
				Agent.Main.StopUsingGameObject(true, 1);
			}
			Agent crusasAgent = this._crusasAgent;
			if (crusasAgent != null && crusasAgent.IsUsingGameObject)
			{
				this._crusasAgent.StopUsingGameObject(true, 1);
			}
			Agent slaveTraderAgent = this._slaveTraderAgent;
			if (slaveTraderAgent != null && slaveTraderAgent.IsUsingGameObject)
			{
				this._slaveTraderAgent.StopUsingGameObject(true, 1);
			}
			this._playerShip.SetCustomSailSetting(true, SailInput.Raised);
			this._playerShip.ShipOrder.SetShipStopOrder();
			this._playerShip.SetAnchor(true, false, 1f);
			this.CalculateBuySlaveConversationPoint();
			this._crusasAgent.ClearTargetFrame();
			this._slaveTraderAgent.ClearTargetFrame();
			WorldPosition worldPosition;
			worldPosition..ctor(base.Mission.Scene, this._crusasConversationPointFrame.GetGlobalFrame().origin);
			float num = MBMath.ToRadians((this._crusasConversationPointFrame.GetGlobalFrame().origin - this._crusasAgent.Position).RotationX);
			this._crusasAgent.SetScriptedPositionAndDirection(ref worldPosition, num, true, 0);
			WorldPosition worldPosition2;
			worldPosition2..ctor(base.Mission.Scene, this._slaveTraderConversationPointFrame.GetGlobalFrame().origin);
			float num2 = MBMath.ToRadians((this._slaveTraderConversationPointFrame.GetGlobalFrame().origin - this._slaveTraderAgent.Position).RotationX);
			this._slaveTraderAgent.SetScriptedPositionAndDirection(ref worldPosition2, num2, false, 0);
			this._crusasAgent.SetLookAgent(this._slaveTraderAgent);
			this._slaveTraderAgent.SetLookAgent(this._crusasAgent);
			this.MakeShipOarsInvisible(this._playerShip);
		}

		// Token: 0x06000705 RID: 1797 RVA: 0x0002CE6C File Offset: 0x0002B06C
		private void InitializeStealthPhasePart1()
		{
			if (this._playerShip == null)
			{
				this._isCheckpointInitialize = true;
				TeamAINavalComponent teamAINavalComponent = new TeamAINavalComponent(base.Mission, base.Mission.AttackerTeam, 5f, 1f);
				base.Mission.AttackerTeam.AddTeamAI(teamAINavalComponent, false);
				base.Mission.AttackerTeam.AddTacticOption(new TacticNavalBalancedOffense(base.Mission.AttackerTeam));
				TeamAINavalComponent teamAINavalComponent2 = new TeamAINavalComponent(base.Mission, base.Mission.DefenderTeam, 5f, 1f);
				base.Mission.DefenderTeam.AddTeamAI(teamAINavalComponent2, false);
				base.Mission.DefenderTeam.AddTacticOption(new TacticNavalBalancedOffense(base.Mission.DefenderTeam));
				this._playerShip = this.CreateShip("crusas_roundship_nested_q5", "phase_1_player_ship_sp", this._playerFormation, false, null, null, true);
				this._phase1EnemyShip1 = this.CreateShip("sturgia_heavy_ship", "phase_1_enemy_ship_1_sp", this.GetAvailableEnemyFormation(), true, null, null, false);
				this._phase1EnemyShip2 = this.CreateShip("ship_lodya_storyline", "phase_1_enemy_ship_2_sp", this.GetAvailableEnemyFormation(), true, this._phase1EnemyShip2UpgradePieceList, null, false);
				this._phase1EnemyShip3 = this.CreateShip("ship_dromon_storyline", "phase_1_enemy_ship_3_sp", this.GetAvailableEnemyFormation(), true, this._escapeShipUpgradePieceList, null, false);
				this._phase1EnemyShip4 = this.CreateShip("ship_birlinn_storyline", "phase_1_enemy_ship_4_sp", this.GetAvailableEnemyFormation(), true, null, null, false);
				this._phase1EnemyShip1.SetCanBeTakenOver(false);
				this._phase1EnemyShip2.SetCanBeTakenOver(false);
				this._phase1EnemyShip3.SetCanBeTakenOver(false);
				this._phase1EnemyShip4.SetCanBeTakenOver(false);
				this._phase1EnemyShipToInteriorShipDoorEntity = Mission.Current.Scene.FindEntityWithTag("phase_1_enemy_ship_3_to_interior_door_tag");
				this._phase1EnemyShipToInteriorShipDoorEntity.GetFirstScriptOfType<ShipDoorUsePoint>().SetShipDoorUsePointEnabled(false);
				this.HandleStealthShipsBridgeConnections();
				base.Mission.SetMissionMode(4, true);
			}
		}

		// Token: 0x06000706 RID: 1798 RVA: 0x0002D04C File Offset: 0x0002B24C
		private void InitializeStealthPhasePart2()
		{
			if (this._isCheckpointInitialize)
			{
				this._phase1EnemyShip1.SetAnchor(true, false, 1f);
				this._phase1EnemyShip1.ShipOrder.SetShipStopOrder();
				this._phase1EnemyShip1.SetController(ShipControllerType.AI, true);
				this._phase1EnemyShip1.SetShipOrderActive(false);
				this._phase1EnemyShip2.SetAnchor(true, false, 1f);
				this._phase1EnemyShip2.ShipOrder.SetShipStopOrder();
				this._phase1EnemyShip2.SetController(ShipControllerType.AI, true);
				this._phase1EnemyShip2.SetShipOrderActive(false);
				this._phase1EnemyShip3.SetAnchor(true, false, 1f);
				this._phase1EnemyShip3.ShipOrder.SetShipStopOrder();
				this._phase1EnemyShip3.SetController(ShipControllerType.AI, true);
				this._phase1EnemyShip3.SetShipOrderActive(false);
				this._phase1EnemyShip4.SetAnchor(true, false, 1f);
				this._phase1EnemyShip4.ShipOrder.SetShipStopOrder();
				this._phase1EnemyShip4.SetController(ShipControllerType.AI, true);
				this._phase1EnemyShip4.SetShipOrderActive(false);
				foreach (ShipAttachmentMachine shipAttachmentMachine in this._phase1EnemyShip1.AttachmentMachines)
				{
					foreach (StandingPoint standingPoint in shipAttachmentMachine.StandingPoints)
					{
						standingPoint.IsDisabledForPlayers = true;
					}
				}
				foreach (ShipAttachmentMachine shipAttachmentMachine2 in this._phase1EnemyShip2.AttachmentMachines)
				{
					if (!shipAttachmentMachine2.GameEntity.Parent.HasTag("bridge_a") && !shipAttachmentMachine2.GameEntity.Parent.HasTag("bridge_b") && !shipAttachmentMachine2.GameEntity.Parent.HasTag("bridge_c"))
					{
						foreach (StandingPoint standingPoint2 in shipAttachmentMachine2.StandingPoints)
						{
							standingPoint2.IsDisabledForPlayers = true;
						}
					}
				}
				foreach (ClimbingMachine climbingMachine in this._phase1EnemyShip1.ClimbingMachines)
				{
					foreach (StandingPoint standingPoint3 in climbingMachine.StandingPoints)
					{
						standingPoint3.IsDisabledForPlayers = true;
					}
				}
				Mission.Current.OnDeploymentFinished();
				this.SpawnPhase1AllyTroops();
				this.SpawnPhase1EnemyTroops();
				base.Mission.PlayerTeam.SetPlayerRole(true, true);
				this._playerShip.Formation.PlayerOwner = Agent.Main;
				Agent.Main.SetClothingColor1(4279111698U);
				Agent.Main.SetClothingColor2(4279111698U);
				Agent.Main.UpdateSpawnEquipmentAndRefreshVisuals(this.GetScriptedStealthEquipment());
				this._gunnarAgent.UpdateSpawnEquipmentAndRefreshVisuals(this.GetScriptedStealthEquipment());
				this._gunnarMovementState = Quest5SetPieceBattleMissionController.GunnarMovementState.TeleportToTargetPosition;
				this.HandleGunnarMovement();
				GameEntity gameEntity = Mission.Current.Scene.FindEntityWithTag("sp_player_stealth_checkpoint");
				Agent.Main.TeleportToPosition(gameEntity.GlobalPosition);
				Mission.Current.Scene.FindEntityWithTag("phase_2_barricade").SetVisibilityExcludeParents(false);
				this._isCheckpointInitialize = false;
				this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.ClearGuards;
				Agent.Main.SetCrouchMode(true);
				this.RemoveShipControlPointDescriptionOfAllEnemyShips();
			}
			foreach (MBInformationManager.DialogNotificationHandle dialogNotificationHandle in this._dialogNotificationHandleCache)
			{
				CampaignInformationManager.ClearDialogNotification(dialogNotificationHandle, true);
			}
			this._dialogNotificationHandleCache.Clear();
		}

		// Token: 0x06000707 RID: 1799 RVA: 0x0002D468 File Offset: 0x0002B668
		private void MovePhase1EnemyShip1ToItsTargetPoint()
		{
			MatrixFrame matrixFrame = this._phase1EnemyShip1TargetEntity.GetGlobalFrame();
			if (matrixFrame.origin.Distance(this._phase1EnemyShip1.GlobalFrame.origin) <= 2f)
			{
				this._phase1EnemyShip1.ShipOrder.SetShipStopOrder();
				this._phase1EnemyShip1.SetAnchor(true, false, 1f);
				matrixFrame = this._phase1EnemyShip1TargetEntity.GetGlobalFrame();
				Vec2 asVec = matrixFrame.origin.AsVec2;
				Vec2 vec = (this._phase1EnemyShip1TargetEntity.GetGlobalFrame().origin - this._phase1EnemyShip1InitialSpawnEntity.GetGlobalFrame().origin).AsVec2.Normalized();
				this._phase1EnemyShip1.SetAnchorFrame(in asVec, in vec, 1f);
				this._phase1EnemyShip1.ShipOrder.SetOrderOarsmenLevel(0);
				return;
			}
			matrixFrame = this._phase1EnemyShip1TargetEntity.GetGlobalFrame();
			Vec2 asVec2 = matrixFrame.origin.AsVec2;
			Vec2 vec2 = (this._phase1EnemyShip1TargetEntity.GetGlobalFrame().origin - this._phase1EnemyShip1InitialSpawnEntity.GetGlobalFrame().origin).AsVec2.Normalized();
			this._phase1EnemyShip1.ShipOrder.SetShipMovementOrder(asVec2, in vec2);
		}

		// Token: 0x06000708 RID: 1800 RVA: 0x0002D5A8 File Offset: 0x0002B7A8
		private void InitializeShipInteriorPhase()
		{
			Mission.Current.Scene.SetAtmosphereWithName("TOD_01_00_SemiCloudy");
			base.Mission.GetMissionBehavior<MissionConversationLogic>().DisableStartConversation(true);
			GameEntity gameEntity = Mission.Current.Scene.FindEntityWithTag("phase_1_interior_player_sp");
			GameEntity gameEntity2 = Mission.Current.Scene.FindEntityWithTag("phase_1_interior_sister_sp");
			Agent.Main.TeleportToPosition(gameEntity.GlobalPosition);
			Vec3 globalPosition = gameEntity2.GlobalPosition;
			Vec2 asVec = gameEntity2.GetGlobalFrame().rotation.f.AsVec2;
			Equipment equipment = StoryModeHeroes.LittleSister.CivilianEquipment.Clone(false);
			for (int i = 0; i < 5; i++)
			{
				equipment[i] = EquipmentElement.Invalid;
			}
			equipment[5] = EquipmentElement.Invalid;
			equipment[9] = EquipmentElement.Invalid;
			StoryModeHeroes.LittleSister.HitPoints = StoryModeHeroes.LittleSister.WoundedHealthLimit - 1;
			AgentBuildData agentBuildData = new AgentBuildData(StoryModeHeroes.LittleSister.CharacterObject).TroopOrigin(new PartyAgentOrigin(PartyBase.MainParty, StoryModeHeroes.LittleSister.CharacterObject, -1, default(UniqueTroopDescriptor), false, true)).Team(base.Mission.PlayerTeam).InitialPosition(ref globalPosition)
				.InitialDirection(ref asVec)
				.Equipment(equipment)
				.NoHorses(true)
				.NoWeapons(false);
			this.SisterAgent = Mission.Current.SpawnAgent(agentBuildData, false);
			this.SisterAgent.SetMortalityState(2);
			this._mainAgentEquipmentCopyForInteriorMission = Agent.Main.SpawnEquipment.Clone(false);
			Equipment equipment2 = Agent.Main.SpawnEquipment.Clone(false);
			for (int j = 0; j < 12; j++)
			{
				if (j == 0 || j == 1 || j == 2 || j == 3 || j == 4)
				{
					equipment2[j] = EquipmentElement.Invalid;
				}
			}
			Agent.Main.TryToSheathWeaponInHand(0, 1);
			Agent.Main.UpdateSpawnEquipmentAndRefreshVisuals(equipment2);
			this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.TalkSister;
			Mission.Current.SetMissionMode(0, false);
			this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoToShipInteriorFadeIn;
		}

		// Token: 0x06000709 RID: 1801 RVA: 0x0002D7AC File Offset: 0x0002B9AC
		private void InitializeGoBackToShip()
		{
			if (Extensions.IsEmpty<Agent>(this._stealthAgents))
			{
				this._gunnarAgent.TeleportToPosition(this.GunnarShipUsePosition.origin);
				this._gunnarMovementState = Quest5SetPieceBattleMissionController.GunnarMovementState.WaitForReachingToTheEscapeShip;
			}
			Mission.Current.Scene.SetAtmosphereWithName("TOD_naval_03_00_sunset");
			base.Mission.GetMissionBehavior<MissionConversationLogic>().DisableStartConversation(true);
			this.SisterAgent.SetMortalityState(0);
			this.SisterAgent.FadeOut(true, false);
			Agent.Main.TeleportToPosition(this._phase1EnemyShipToInteriorShipDoorEntity.GlobalPosition);
			Mission.Current.Scene.FindEntityWithTag("phase_2_barricade").SetVisibilityExcludeParents(true);
			base.Mission.SetMissionMode(4, false);
			Agent.Main.UpdateSpawnEquipmentAndRefreshVisuals(this._mainAgentEquipmentCopyForInteriorMission);
			this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoBackToShipFadeIn;
		}

		// Token: 0x0600070A RID: 1802 RVA: 0x0002D876 File Offset: 0x0002BA76
		public void GetIntendedMainAgentDirectionForPhase1InteriorTeleport(out Vec3 mainAgentDirection)
		{
			mainAgentDirection = this.SisterAgent.Position - Agent.Main.Position;
		}

		// Token: 0x0600070B RID: 1803 RVA: 0x0002D898 File Offset: 0x0002BA98
		public void GetIntendedMainAgentDirectionForPhase1EscapeShipTeleport(out Vec3 mainAgentDirection)
		{
			mainAgentDirection = Agent.Main.Position - this._gunnarAgent.Position;
		}

		// Token: 0x0600070C RID: 1804 RVA: 0x0002D8BA File Offset: 0x0002BABA
		public void TriggerPhase1InitializeShipInteriorPhase()
		{
			this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1InitializeShipInteriorPhase;
		}

		// Token: 0x0600070D RID: 1805 RVA: 0x0002D8C4 File Offset: 0x0002BAC4
		public void CompletePhase1GoToShipInteriorTransition()
		{
			this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1ShipInteriorPhase;
		}

		// Token: 0x0600070E RID: 1806 RVA: 0x0002D8CE File Offset: 0x0002BACE
		public void TriggerPhase1InitializeGoBackToShipPhase()
		{
			this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1InitializeGoBackToShip;
		}

		// Token: 0x0600070F RID: 1807 RVA: 0x0002D8D8 File Offset: 0x0002BAD8
		public void CompletePhase1InitializeGoBackToShipTransition()
		{
			this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1EscapePhase;
			this.HandlePlayersBridgeAndControlPointUsagesForPhase1EscapePhase();
		}

		// Token: 0x06000710 RID: 1808 RVA: 0x0002D8E8 File Offset: 0x0002BAE8
		public void SetTalkedWithSister()
		{
			this._talkedWithSister = true;
			this.DeactivateObjectiveIfItIsActive(this._talkWithYourSisterObjective);
			base.Mission.GetMissionBehavior<MissionConversationLogic>().DisableStartConversation(true);
			this.Phase1InteriorCameraSisterEntity = null;
			this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.ReturnToDeck;
		}

		// Token: 0x06000711 RID: 1809 RVA: 0x0002D920 File Offset: 0x0002BB20
		private void CalculateBuySlaveConversationPoint()
		{
			float num = float.MaxValue;
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this._playerShip.AttachmentPointMachines)
			{
				foreach (ShipAttachmentPointMachine shipAttachmentPointMachine2 in this._phase1EnemyShip1.AttachmentPointMachines)
				{
					float num2 = shipAttachmentPointMachine.GameEntity.GetGlobalFrame().origin.Distance(shipAttachmentPointMachine2.GameEntity.GetGlobalFrame().origin);
					if (num > num2)
					{
						this._crusasConversationPointFrame = shipAttachmentPointMachine.GameEntity;
						this._slaveTraderConversationPointFrame = shipAttachmentPointMachine2.GameEntity;
						num = num2;
					}
				}
			}
		}

		// Token: 0x06000712 RID: 1810 RVA: 0x0002DA14 File Offset: 0x0002BC14
		private void AddConversationSounds()
		{
			this._conversationSounds.Enqueue(new Quest5SetPieceBattleMissionController.ConversationSound(new TextObject("{=kAAkgKFB}Ahoy! Who approaches?", null), 2, this._slaveTraderCharacter));
			this._conversationSounds.Enqueue(new Quest5SetPieceBattleMissionController.ConversationSound(new TextObject("{=61hcBa4X}I am Crusas Salautas. I seek Purig of Agilting.", null), 2, NavalStorylineData.Prusas.CharacterObject));
			this._conversationSounds.Enqueue(new Quest5SetPieceBattleMissionController.ConversationSound(new TextObject("{=JAtDE00L}This is his ship, but he's away. Should be back shortly, though - we signalled him. Keep your distance for now.", null), 2, this._slaveTraderCharacter));
			this._conversationSounds.Enqueue(new Quest5SetPieceBattleMissionController.ConversationSound(new TextObject("{=JPVD5sfc}I am one of Purig's longtime customers, and I am in a bit of a hurry. I made arrangements weeks ago to buy his merchandise. How long is Purig going to be? Can I come aboard?", null), 2, NavalStorylineData.Prusas.CharacterObject));
			this._conversationSounds.Enqueue(new Quest5SetPieceBattleMissionController.ConversationSound(new TextObject("{=MNnk6LAa}You'll need to be patient, friend. Purig's instructions were to let no one aboard. But he won't be long. He's just offshore, out looking for prey.", null), 2, this._slaveTraderCharacter));
			this._conversationSounds.Enqueue(new Quest5SetPieceBattleMissionController.ConversationSound(new TextObject("{=wJZTakoT}How many do you have to sell?", null), 2, NavalStorylineData.Prusas.CharacterObject));
			this._conversationSounds.Enqueue(new Quest5SetPieceBattleMissionController.ConversationSound(new TextObject("{=4Z7a0Kre}Several score, all in good health. We've been feeding them well, sparing no expense. We take pride in our work.", null), 2, this._slaveTraderCharacter));
			this._conversationSounds.Enqueue(new Quest5SetPieceBattleMissionController.ConversationSound(new TextObject("{=XEbbugis}That's fine, but I was expecting more.", null), 2, NavalStorylineData.Prusas.CharacterObject));
			this._conversationSounds.Enqueue(new Quest5SetPieceBattleMissionController.ConversationSound(new TextObject("{=AXz58qHq}You're not the only buyer, my friend! Mines, buildings, repairs... Even on the mainland, mix a handful of our fellows in with some convicts or war captives, and who's to notice?", null), 2, this._slaveTraderCharacter));
			this._conversationSounds.Enqueue(new Quest5SetPieceBattleMissionController.ConversationSound(new TextObject("{=Zu3lj2s1}So... Can we talk price?", null), 2, NavalStorylineData.Prusas.CharacterObject));
			this._conversationSounds.Enqueue(new Quest5SetPieceBattleMissionController.ConversationSound(new TextObject("{=aYmx5ODE}You'll need to wait for our master to return before you start bargaining. Don't push your friendship with Purig too much, though - he's got expensive tastes. He likes to see the envy in other men's eyes when the sun sparkles off his fine golden helm.", null), 2, this._slaveTraderCharacter));
			this._conversationSounds.Enqueue(new Quest5SetPieceBattleMissionController.ConversationSound(new TextObject("{=dndcy626}I don't like just to sit here idly. Maybe I can come aboard and inspect some of the captives? I can conclude the deal more quickly when your master arrives, and let him get back to his hunting.", null), 2, NavalStorylineData.Prusas.CharacterObject));
			this._conversationSounds.Enqueue(new Quest5SetPieceBattleMissionController.ConversationSound(new TextObject("{=ediTKoqo}My instructions were clear. No one aboard the ship.", null), 2, this._slaveTraderCharacter));
			this._conversationSounds.Enqueue(new Quest5SetPieceBattleMissionController.ConversationSound(new TextObject("{=csvVz5f2}The air is stifling. I hope you've been letting the captives up on deck? No signs of disease?", null), 2, NavalStorylineData.Prusas.CharacterObject));
			this._conversationSounds.Enqueue(new Quest5SetPieceBattleMissionController.ConversationSound(new TextObject("{=aKq3AMpG}If you think they're sick you're welcome not to buy any.", null), 2, this._slaveTraderCharacter));
		}

		// Token: 0x06000713 RID: 1811 RVA: 0x0002DC3C File Offset: 0x0002BE3C
		private void CheckAndPlayCrusasAndSlaveTraderConversationSound()
		{
			if (this._crusasAndSeaHoundMovedToTheConversationPoints)
			{
				if (Agent.Main.Position.Distance(this._playerShip.GetCaptainSpawnGlobalFrame().origin) >= 30f)
				{
					foreach (MBInformationManager.DialogNotificationHandle dialogNotificationHandle in this._dialogNotificationHandleCache)
					{
						CampaignInformationManager.ClearDialogNotification(dialogNotificationHandle, true);
					}
					this._dialogNotificationHandleCache.Clear();
					return;
				}
				if (!Extensions.IsEmpty<Quest5SetPieceBattleMissionController.ConversationSound>(this._conversationSounds))
				{
					Quest5SetPieceBattleMissionController.ConversationSound conversationSound = this._conversationSounds.Dequeue();
					MBInformationManager.DialogNotificationHandle dialogNotificationHandle2 = CampaignInformationManager.AddDialogLine(conversationSound.Line, conversationSound.Character, null, 0, conversationSound.Priority);
					this._dialogNotificationHandleCache.Add(dialogNotificationHandle2);
					return;
				}
			}
			else
			{
				Vec2 vec = this._crusasAgent.Position.AsVec2;
				MatrixFrame matrixFrame = this._crusasConversationPointFrame.GetGlobalFrame();
				if (vec.NearlyEquals(matrixFrame.origin.AsVec2, 3f))
				{
					vec = this._slaveTraderAgent.Position.AsVec2;
					matrixFrame = this._slaveTraderConversationPointFrame.GetGlobalFrame();
					if (vec.NearlyEquals(matrixFrame.origin.AsVec2, 3f))
					{
						this._crusasAndSeaHoundMovedToTheConversationPoints = true;
						return;
					}
				}
				WorldPosition worldPosition;
				worldPosition..ctor(base.Mission.Scene, this._crusasConversationPointFrame.GetGlobalFrame().origin);
				Vec3 vec2 = this._crusasConversationPointFrame.GetGlobalFrame().origin - this._crusasAgent.Position;
				this._crusasAgent.SetScriptedPositionAndDirection(ref worldPosition, MBMath.ToRadians(vec2.RotationX), true, 0);
				WorldPosition worldPosition2;
				worldPosition2..ctor(base.Mission.Scene, this._slaveTraderConversationPointFrame.GetGlobalFrame().origin);
				float num = MBMath.ToRadians((this._slaveTraderConversationPointFrame.GetGlobalFrame().origin - this._slaveTraderAgent.Position).RotationX);
				this._slaveTraderAgent.SetScriptedPositionAndDirection(ref worldPosition2, num, true, 0);
			}
		}

		// Token: 0x06000714 RID: 1812 RVA: 0x0002DE54 File Offset: 0x0002C054
		private Equipment GetScriptedStealthEquipment()
		{
			Equipment equipment = MBObjectManager.Instance.GetObject<MBEquipmentRoster>("naval_storyline_quest5_stealth_set").DefaultEquipment.Clone(false);
			if (equipment == null)
			{
				equipment = Campaign.Current.DefaultStealthEquipment.Clone(false);
				for (int i = 0; i < 12; i++)
				{
					if (i == 5)
					{
						ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>("assassin_hood");
						if (@object != null)
						{
							equipment[i] = new EquipmentElement(@object, null, null, false);
						}
					}
					else if (i == 9)
					{
						ItemObject object2 = MBObjectManager.Instance.GetObject<ItemObject>("assassin_shoulder");
						if (object2 != null)
						{
							equipment[i] = new EquipmentElement(object2, null, null, false);
						}
					}
					else if (i == 6)
					{
						ItemObject object3 = MBObjectManager.Instance.GetObject<ItemObject>("assassin_armor");
						if (object3 != null)
						{
							equipment[i] = new EquipmentElement(object3, null, null, false);
						}
					}
					else if (i == 7)
					{
						ItemObject object4 = MBObjectManager.Instance.GetObject<ItemObject>("assassin_boot");
						if (object4 != null)
						{
							equipment[i] = new EquipmentElement(object4, null, null, false);
						}
					}
					if ((i == 0 || i == 1 || i == 2 || i == 3 || i == 4) && !equipment[i].IsEmpty && equipment[i].Item.WeaponComponent != null && equipment[i].Item.WeaponComponent.PrimaryWeapon.WeaponClass == 19)
					{
						equipment[i] = EquipmentElement.Invalid;
					}
				}
			}
			return equipment;
		}

		// Token: 0x06000715 RID: 1813 RVA: 0x0002DFC4 File Offset: 0x0002C1C4
		private void HandleEscapeShipCutLoose()
		{
			if (this._escapeShipCutLooseTimer != null && this._escapeShipCutLooseTimer.Check(false))
			{
				this._escapeShipCutLooseTimer = null;
				foreach (ShipAttachmentMachine shipAttachmentMachine in this._phase1EnemyShip3.AttachmentMachines)
				{
					if (shipAttachmentMachine.IsShipAttachmentMachineBridged())
					{
						shipAttachmentMachine.DisconnectAttachment();
					}
				}
				foreach (ShipAttachmentMachine shipAttachmentMachine2 in this._phase1EnemyShip2.AttachmentMachines)
				{
					if (shipAttachmentMachine2.IsShipAttachmentMachineBridged() && shipAttachmentMachine2.CurrentAttachment.AttachmentTarget.OwnerShip == this._phase1EnemyShip3)
					{
						shipAttachmentMachine2.DisconnectAttachment();
					}
				}
			}
		}

		// Token: 0x06000716 RID: 1814 RVA: 0x0002E0AC File Offset: 0x0002C2AC
		public bool ShouldTeleportPlayerBetweenTargetPositionAndHidingSpot()
		{
			if (Agent.Main != null && Agent.Main.IsActive() && !Agent.Main.IsInWater())
			{
				return false;
			}
			if (this._allowedSwimRadiusCheckTimer == null)
			{
				this._allowedSwimRadiusCheckTimer = new MissionTimer(5f);
			}
			else if (Agent.Main != null && Agent.Main.IsActive() && this._allowedSwimRadiusCheckTimer.Check(false))
			{
				this._allowedSwimRadiusCheckTimer.Reset();
				if (Agent.Main.Position.Distance(this.HidingSpot1Position.GlobalPosition) > 200f)
				{
					MBInformationManager.DialogNotificationHandle dialogNotificationHandle = CampaignInformationManager.AddDialogLine(new TextObject("{=4O6feRM9}Hey! Over here! Let's not get separated.", null), NavalStorylineData.Gunnar.CharacterObject, null, 0, 3);
					this._dialogNotificationHandleCache.Add(dialogNotificationHandle);
					return true;
				}
				if (Agent.Main.Position.Distance(this._phase1EnemyShip1.GameEntity.GlobalPosition) < 25f)
				{
					MBInformationManager.DialogNotificationHandle dialogNotificationHandle2 = CampaignInformationManager.AddDialogLine(new TextObject("{=y0EgxaLN}Keep away from those lookouts!", null), NavalStorylineData.Gunnar.CharacterObject, null, 0, 3);
					this._dialogNotificationHandleCache.Add(dialogNotificationHandle2);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000717 RID: 1815 RVA: 0x0002E1D8 File Offset: 0x0002C3D8
		public void TeleportPlayerBetweenTargetPositionAndHidingSpot(out Vec3 mainAgentDirection)
		{
			mainAgentDirection = Agent.Main.LookDirection;
			if (this.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1GoToEnemyShip)
			{
				StandingPoint pilotStandingPoint = this._playerShip.ShipControllerMachine.PilotStandingPoint;
				Agent.Main.TeleportToPosition(pilotStandingPoint.GameEntity.GlobalPosition);
				Agent.Main.HandleStartUsingAction(pilotStandingPoint, -1);
				return;
			}
			Vec3 vec = (this._approachPointEntity.GlobalPosition + this.HidingSpot1Position.GlobalPosition) * 0.5f;
			mainAgentDirection = (this.HidingSpot1Position.GlobalPosition - vec).NormalizedCopy();
			Agent.Main.TeleportToPosition(vec);
		}

		// Token: 0x06000718 RID: 1816 RVA: 0x0002E284 File Offset: 0x0002C484
		public bool ShouldTeleportPlayerShipToStartingPosition()
		{
			if (this._playerShip != null)
			{
				MatrixFrame matrixFrame = this._playerShip.GlobalFrame;
				if (matrixFrame.origin.NearlyEquals(ref this._phase1PlayerShipSpawnPosition, 2f))
				{
					return false;
				}
				if (MBMath.ApproximatelyEqualsTo(this._lastCachedPlayerShipDistanceToTargetApproachPoint, 0f, 1E-05f))
				{
					matrixFrame = this._playerShip.GlobalFrame;
					this._lastCachedPlayerShipDistanceToTargetApproachPoint = matrixFrame.origin.Distance(this._approachPointEntity.GlobalPosition);
					this._playerShipsTargetApproachPointDistanceCheckTimer = new MissionTimer(6f);
				}
				else
				{
					MissionTimer playerShipsTargetApproachPointDistanceCheckTimer = this._playerShipsTargetApproachPointDistanceCheckTimer;
					if (playerShipsTargetApproachPointDistanceCheckTimer != null && playerShipsTargetApproachPointDistanceCheckTimer.Check(false))
					{
						matrixFrame = this._playerShip.GlobalFrame;
						float num = matrixFrame.origin.Distance(this._approachPointEntity.GlobalPosition);
						if (num > this._lastCachedPlayerShipDistanceToTargetApproachPoint)
						{
							this._lastCachedPlayerShipDistanceToTargetApproachPoint = 0f;
							this._playerShipsTargetApproachPointDistanceCheckTimer = null;
							return true;
						}
						this._lastCachedPlayerShipDistanceToTargetApproachPoint = num;
					}
				}
			}
			return false;
		}

		// Token: 0x06000719 RID: 1817 RVA: 0x0002E374 File Offset: 0x0002C574
		public void TeleportPlayerShipToStartingPosition(out Vec3 mainAgentDirection)
		{
			GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("phase_1_player_ship_sp");
			this._navalShipsLogic.TeleportShip(this._playerShip, gameEntity.GetGlobalFrame(), true, false, true);
			mainAgentDirection = Agent.Main.LookDirection;
		}

		// Token: 0x0600071A RID: 1818 RVA: 0x0002E3C4 File Offset: 0x0002C5C4
		public Vec3 CalculateMissionStartDirection()
		{
			return (this._approachPointEntity.GetGlobalFrame().origin - Agent.Main.Frame.origin).NormalizedCopy();
		}

		// Token: 0x0600071B RID: 1819 RVA: 0x0002E400 File Offset: 0x0002C600
		private void HandlePlayersBridgeAndControlPointUsagesForPhase1GoToEnemyShip()
		{
			foreach (MissionShip missionShip in this._navalShipsLogic.AllShips)
			{
				if (missionShip != this._playerShip)
				{
					foreach (ClimbingMachine climbingMachine in missionShip.ClimbingMachines)
					{
						foreach (StandingPoint standingPoint in climbingMachine.StandingPoints)
						{
							standingPoint.IsDisabledForPlayers = true;
						}
					}
				}
				foreach (ShipAttachmentMachine shipAttachmentMachine in missionShip.AttachmentMachines)
				{
					foreach (StandingPoint standingPoint2 in shipAttachmentMachine.StandingPoints)
					{
						standingPoint2.IsDisabledForPlayers = true;
					}
				}
				foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in missionShip.AttachmentPointMachines)
				{
					foreach (StandingPoint standingPoint3 in shipAttachmentPointMachine.StandingPoints)
					{
						standingPoint3.IsDisabledForPlayers = true;
					}
				}
			}
		}

		// Token: 0x0600071C RID: 1820 RVA: 0x0002E624 File Offset: 0x0002C824
		private void HandlePlayersBridgeAndControlPointUsagesForPhase1SwimmingAndStealthPhase()
		{
			foreach (MissionShip missionShip in this._navalShipsLogic.AllShips)
			{
				if (missionShip != this._phase1EnemyShip4)
				{
					using (List<ClimbingMachine>.Enumerator enumerator2 = missionShip.ClimbingMachines.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							ClimbingMachine climbingMachine = enumerator2.Current;
							foreach (StandingPoint standingPoint in climbingMachine.StandingPoints)
							{
								standingPoint.IsDisabledForPlayers = true;
							}
						}
						goto IL_00E9;
					}
					goto IL_0088;
				}
				goto IL_0088;
				IL_00E9:
				foreach (ShipAttachmentMachine shipAttachmentMachine in missionShip.AttachmentMachines)
				{
					foreach (StandingPoint standingPoint2 in shipAttachmentMachine.StandingPoints)
					{
						standingPoint2.IsDisabledForPlayers = true;
					}
				}
				foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in missionShip.AttachmentPointMachines)
				{
					foreach (StandingPoint standingPoint3 in shipAttachmentPointMachine.StandingPoints)
					{
						standingPoint3.IsDisabledForPlayers = true;
					}
				}
				continue;
				IL_0088:
				foreach (ClimbingMachine climbingMachine2 in missionShip.ClimbingMachines)
				{
					foreach (StandingPoint standingPoint4 in climbingMachine2.StandingPoints)
					{
						standingPoint4.IsDisabledForPlayers = false;
					}
				}
				goto IL_00E9;
			}
		}

		// Token: 0x0600071D RID: 1821 RVA: 0x0002E8D8 File Offset: 0x0002CAD8
		private void HandlePlayersBridgeAndControlPointUsagesForPhase1EscapePhase()
		{
			foreach (MissionShip missionShip in this._navalShipsLogic.AllShips)
			{
				foreach (ClimbingMachine climbingMachine in missionShip.ClimbingMachines)
				{
					foreach (StandingPoint standingPoint in climbingMachine.StandingPoints)
					{
						standingPoint.IsDisabledForPlayers = false;
					}
				}
				if (missionShip != this._phase1EnemyShip3)
				{
					foreach (ShipAttachmentMachine shipAttachmentMachine in missionShip.AttachmentMachines)
					{
						foreach (StandingPoint standingPoint2 in shipAttachmentMachine.StandingPoints)
						{
							standingPoint2.IsDisabledForPlayers = true;
						}
					}
					using (List<ShipAttachmentPointMachine>.Enumerator enumerator5 = missionShip.AttachmentPointMachines.GetEnumerator())
					{
						while (enumerator5.MoveNext())
						{
							ShipAttachmentPointMachine shipAttachmentPointMachine = enumerator5.Current;
							foreach (StandingPoint standingPoint3 in shipAttachmentPointMachine.StandingPoints)
							{
								standingPoint3.IsDisabledForPlayers = true;
							}
						}
						continue;
					}
				}
				foreach (ShipAttachmentMachine shipAttachmentMachine2 in missionShip.AttachmentMachines)
				{
					if (shipAttachmentMachine2.CurrentAttachment == null)
					{
						using (List<StandingPoint>.Enumerator enumerator3 = shipAttachmentMachine2.StandingPoints.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								StandingPoint standingPoint4 = enumerator3.Current;
								standingPoint4.IsDisabledForPlayers = true;
							}
							continue;
						}
					}
					foreach (StandingPoint standingPoint5 in shipAttachmentMachine2.StandingPoints)
					{
						standingPoint5.IsDisabledForPlayers = false;
					}
				}
				foreach (ShipAttachmentPointMachine shipAttachmentPointMachine2 in missionShip.AttachmentPointMachines)
				{
					if (shipAttachmentPointMachine2.CurrentAttachment == null)
					{
						using (List<StandingPoint>.Enumerator enumerator3 = shipAttachmentPointMachine2.StandingPoints.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								StandingPoint standingPoint6 = enumerator3.Current;
								standingPoint6.IsDisabledForPlayers = true;
							}
							continue;
						}
					}
					foreach (StandingPoint standingPoint7 in shipAttachmentPointMachine2.StandingPoints)
					{
						standingPoint7.IsDisabledForPlayers = false;
					}
				}
			}
		}

		// Token: 0x0600071E RID: 1822 RVA: 0x0002ECE0 File Offset: 0x0002CEE0
		private void ClearPhase1OnPhaseTransition()
		{
			this._phase1EnemyShip1 = null;
			this._phase1EnemyShip2 = null;
			this._phase1EnemyShip4 = null;
			this._dynamicPatrolAreas = null;
			this._stealthAgents = null;
			this._crusasConversationPointFrame = WeakGameEntity.Invalid;
			this._slaveTraderConversationPointFrame = WeakGameEntity.Invalid;
			this._approachPointEntity = null;
			this._phase1EnemyShipToInteriorShipDoorEntity = null;
			this._phase1InteriorToEnemyShip3ShipDoorEntity = null;
			this._phase1EnemyShip1InitialSpawnEntity = null;
			this._phase1EnemyShip1TargetEntity = null;
			this._conversationSounds = null;
			this._dialogNotificationHandleCache.Clear();
			this._sisterWoundedAnimationActionIndexCache = ActionIndexCache.act_none;
			this._slaveTraderShipOarsmanActionIndexCache = ActionIndexCache.act_none;
			this.Phase1InteriorCameraSisterEntity = null;
			GC.Collect();
		}

		// Token: 0x0600071F RID: 1823 RVA: 0x0002ED7D File Offset: 0x0002CF7D
		public void TriggerInitializePhase2()
		{
			this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase2Part1;
			MBMusicManager.Current.StartTheme(10246, 0.3f, false);
		}

		// Token: 0x06000720 RID: 1824 RVA: 0x0002ED9C File Offset: 0x0002CF9C
		public void CompletePhase1ToPhase2Transition()
		{
			this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase2InProgress;
		}

		// Token: 0x06000721 RID: 1825 RVA: 0x0002EDA8 File Offset: 0x0002CFA8
		private void InitializePhase2Part1()
		{
			Mission.Current.Scene.SetAtmosphereWithName("TOD_naval_03_00_sunset");
			if (this._gunnarAgent != null && this._gunnarAgent.IsUsingGameObject)
			{
				this._gunnarAgent.StopUsingGameObjectMT(true, 1);
			}
			if (Agent.Main != null && Agent.Main.IsActive() && Agent.Main.IsUsingGameObject)
			{
				Agent.Main.StopUsingGameObject(true, 1);
			}
			if (this._slaveTraderAgent != null && this._slaveTraderAgent.IsActive())
			{
				this._slaveTraderAgent.FadeOut(true, false);
				for (int i = 0; i < this._slaveTraderShipOarsmen.Length; i++)
				{
					Agent agent = this._slaveTraderShipOarsmen[i];
					if (agent != null)
					{
						agent.FadeOut(true, false);
					}
				}
				this._navalTrajectoryPlanningLogic.ForceReinitialize();
			}
			Quest5WanderingShipsMissionLogic missionBehavior = base.Mission.GetMissionBehavior<Quest5WanderingShipsMissionLogic>();
			if (missionBehavior != null)
			{
				missionBehavior.OnPhase2Started();
			}
			foreach (MBInformationManager.DialogNotificationHandle dialogNotificationHandle in this._dialogNotificationHandleCache)
			{
				CampaignInformationManager.ClearDialogNotification(dialogNotificationHandle, true);
			}
			this._dialogNotificationHandleCache.Clear();
		}

		// Token: 0x06000722 RID: 1826 RVA: 0x0002EED0 File Offset: 0x0002D0D0
		private void InitializePhase2Part2()
		{
			this._phase2AllyShip1 = this.CreateShip("aserai_heavy_ship", "phase_2_ally_ship_1_sp", this.GetAvailableAllyFormation(), false, this._phase2AllyShip1UpgradePieceList, null, true);
			this._phase2AllyShip2 = this.CreateShip("nord_medium_ship", "phase_2_ally_ship_2_sp", this.GetAvailableAllyFormation(), false, this._phase2AllyShip2UpgradePieceList, null, true);
			this._phase2AllyShip3 = this.CreateShip("northern_medium_ship", "phase_2_ally_ship_3_sp", this.GetAvailableAllyFormation(), false, this._phase2AllyShip3UpgradePieceList, null, true);
			this._phase2AllyShip4 = this.CreateShip("sturgia_heavy_ship", "phase_2_ally_ship_4_sp", this.GetAvailableAllyFormation(), false, this._phase2AllyShip4UpgradePieceList, null, true);
			this._phase2AllyShip5 = this.CreateShip("northern_medium_ship", "phase_2_ally_ship_5_sp", this.GetAvailableAllyFormation(), false, this._phase2AllyShip5UpgradePieceList, null, true);
			if (this._phase1EnemyShip3 == null)
			{
				this._isCheckpointInitialize = true;
				TeamAINavalComponent teamAINavalComponent = new TeamAINavalComponent(base.Mission, base.Mission.AttackerTeam, 5f, 1f);
				base.Mission.AttackerTeam.AddTeamAI(teamAINavalComponent, false);
				base.Mission.AttackerTeam.AddTacticOption(new TacticNavalBalancedOffense(base.Mission.AttackerTeam));
				TeamAINavalComponent teamAINavalComponent2 = new TeamAINavalComponent(base.Mission, base.Mission.DefenderTeam, 5f, 1f);
				base.Mission.DefenderTeam.AddTeamAI(teamAINavalComponent2, false);
				base.Mission.DefenderTeam.AddTacticOption(new TacticNavalBalancedOffense(base.Mission.DefenderTeam));
				this._navalAgentsLogic.SetDeploymentMode(true);
				this._navalShipsLogic.SetDeploymentMode(true);
				this._playerShip = this.CreateShip("ship_dromon_storyline", "phase_1_enemy_ship_3_sp", this._playerFormation, false, this._escapeShipUpgradePieceList, null, true);
				this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._playerShip, 2);
				Hero.MainHero.Heal(Hero.MainHero.MaxHitPoints, false);
				this._navalAgentsLogic.AddReservedTroopToShip(new PartyAgentOrigin(PartyBase.MainParty, CharacterObject.PlayerCharacter, -1, default(UniqueTroopDescriptor), false, false), this._playerShip);
				this._navalAgentsLogic.SetDeploymentMode(false);
				this._navalShipsLogic.SetDeploymentMode(false);
				this._navalAgentsLogic.SpawnNextBatch(0, false, null);
				this.SpawnGunnarOnShip(this._playerShip);
				this._gunnarAgent.Controller = 0;
				this._gunnarAgent.SetMortalityState(2);
				this._playerShip.SetController(ShipControllerType.AI, true);
				this._phase1EnemyShipToInteriorShipDoorEntity = Mission.Current.Scene.FindEntityWithTag("phase_1_enemy_ship_3_to_interior_door_tag");
				this._phase1EnemyShipToInteriorShipDoorEntity.GetFirstScriptOfType<ShipDoorUsePoint>().SetShipDoorUsePointEnabled(false);
				Agent.Main.TeleportToPosition(this._playerShip.GetMiddleInnerSpawnGlobalFrame().origin);
				this._playerShip.SetAnchor(false, false, 1f);
				this._playerShip.SetShipOrderActive(true);
				this._playerShip.ShipControllerMachine.PilotStandingPoint.IsDisabledForPlayers = true;
			}
			else
			{
				Formation availableAllyFormation = this.GetAvailableAllyFormation();
				this._navalShipsLogic.TransferShipToTeam(this.EscapeShip, base.Mission.PlayerTeam, availableAllyFormation, 8);
				this._navalAgentsLogic.AddAgentToShip(this._gunnarAgent, this.EscapeShip);
				this._navalAgentsLogic.TransferAgentToShip(Agent.Main, this.EscapeShip);
				this.RemoveShipInternal(this._playerShip);
				this.AddAvailableAllyFormation(availableAllyFormation);
				this._navalShipsLogic.TransferShipToFormation(this.EscapeShip, this._playerFormation);
				this._playerShip = this.EscapeShip;
				this._navalAgentsLogic.AssignCaptainToShip(this._gunnarAgent, this.EscapeShip, null);
				this.EscapeShip.ShipOrder.ManageShipDetachments();
				this._gunnarAgent.TeleportToPosition(this.GunnarShipUsePosition.origin);
				this.EscapeShip.ShipControllerMachine.PilotStandingPoint.IsDisabledForPlayers = true;
				this._navalAgentsLogic.SetDeploymentMode(true);
				this._navalShipsLogic.SetDeploymentMode(true);
				this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this.EscapeShip);
				this._navalAgentsLogic.SetDeploymentMode(false);
				this._navalShipsLogic.SetDeploymentMode(false);
				Vec3 position = Agent.Main.Position;
				Agent.Main.TeleportToPosition(position);
				this.AddAvailableEnemyFormation(this._phase1EnemyShip1.Formation);
				this.RemoveShipInternal(this._phase1EnemyShip1);
				this.AddAvailableEnemyFormation(this._phase1EnemyShip4.Formation);
				this.RemoveShipInternal(this._phase1EnemyShip4);
				this._navalTrajectoryPlanningLogic.ForceReinitialize();
			}
			this._phase2EnemyShip1 = this.CreateShip("ship_meditlight_storyline_q5", "phase_2_enemy_ship_1_sp", this.GetAvailableEnemyFormation(), false, null, null, true);
			this._phase2EnemyShip2 = this.CreateShip("ship_meditlight_storyline_q5", "phase_2_enemy_ship_2_sp", this.GetAvailableEnemyFormation(), false, null, null, true);
			this._phase2EnemyShip3 = this.CreateShip("ship_meditlight_storyline_q5", "phase_2_enemy_ship_3_sp", this.GetAvailableEnemyFormation(), false, null, null, true);
			this._phase2EnemyShip4 = this.CreateShip("ship_meditlight_storyline_q5", "phase_2_enemy_ship_4_sp", this.GetAvailableEnemyFormation(), false, null, null, true);
			this._phase2EnemyShip5 = this.CreateShip("ship_meditlight_storyline_q5", "phase_2_enemy_ship_5_sp", this.GetAvailableEnemyFormation(), false, null, null, true);
			this._phase2EnemyShip1.SetCanBeTakenOver(false);
			this._phase2EnemyShip2.SetCanBeTakenOver(false);
			this._phase2EnemyShip3.SetCanBeTakenOver(false);
			this._phase2EnemyShip4.SetCanBeTakenOver(false);
			this._phase2EnemyShip5.SetCanBeTakenOver(false);
			this._phase2EnemyShipStationary1 = this.CreateShip("western_medium_ship", "phase_2_enemy_ship_stationary_1", this.GetAvailableEnemyFormation(), false, null, null, true);
			this._phase2EnemyShipStationary1.SetCanBeTakenOver(false);
			this.AddTriggerPointForPirateShip(this._phase2EnemyShip1, base.Mission.Scene.FindEntityWithTag("phase_2_enemy_ship_1_target"));
			this.AddTriggerPointForPirateShip(this._phase2EnemyShip2, base.Mission.Scene.FindEntityWithTag("phase_2_enemy_ship_2_target"));
			this.AddTriggerPointForPirateShip(this._phase2EnemyShip3, base.Mission.Scene.FindEntityWithTag("phase_2_enemy_ship_3_target"));
			this.AddTriggerPointForPirateShip(this._phase2EnemyShip4, base.Mission.Scene.FindEntityWithTag("phase_2_enemy_ship_4_target"));
			this.AddTriggerPointForPirateShip(this._phase2EnemyShip5, base.Mission.Scene.FindEntityWithTag("phase_2_enemy_ship_5_target"));
			this._phase2EnemyShip1.SetFoldSailsOnBridgeConnection(false);
			this._phase2EnemyShip2.SetFoldSailsOnBridgeConnection(false);
			this._phase2EnemyShip3.SetFoldSailsOnBridgeConnection(false);
			this._phase2EnemyShip4.SetFoldSailsOnBridgeConnection(false);
			this._phase2EnemyShip5.SetFoldSailsOnBridgeConnection(false);
			this._autoCutLooseTimersForPirateShips.Add(this._phase2EnemyShip1, null);
			this._autoCutLooseTimersForPirateShips.Add(this._phase2EnemyShip2, null);
			this._autoCutLooseTimersForPirateShips.Add(this._phase2EnemyShip3, null);
			this._autoCutLooseTimersForPirateShips.Add(this._phase2EnemyShip4, null);
			this._autoCutLooseTimersForPirateShips.Add(this._phase2EnemyShip5, null);
			this._autoEstablishConnectionsForPirateShips.Add(this._phase2EnemyShip1, null);
			this._autoEstablishConnectionsForPirateShips.Add(this._phase2EnemyShip2, null);
			this._autoEstablishConnectionsForPirateShips.Add(this._phase2EnemyShip3, null);
			this._autoEstablishConnectionsForPirateShips.Add(this._phase2EnemyShip4, null);
			this._autoEstablishConnectionsForPirateShips.Add(this._phase2EnemyShip5, null);
			this.EscapeShip.SetFoldSailsOnBridgeConnection(false);
			foreach (ShipAttachmentMachine shipAttachmentMachine in this.EscapeShip.AttachmentMachines)
			{
				if (shipAttachmentMachine.IsDisabled)
				{
					shipAttachmentMachine.SetEnabledAndMakeVisible(false, false);
				}
			}
			this.SetShipAttachmentJointPhysicsEnabledForShip(this._phase2EnemyShip1, false);
			this.SetShipAttachmentJointPhysicsEnabledForShip(this._phase2EnemyShip2, false);
			this.SetShipAttachmentJointPhysicsEnabledForShip(this._phase2EnemyShip3, false);
			this.SetShipAttachmentJointPhysicsEnabledForShip(this._phase2EnemyShip4, false);
			this.SetShipAttachmentJointPhysicsEnabledForShip(this._phase2EnemyShip5, false);
			this.EscapeShip.SetController(ShipControllerType.AI, true);
			base.Mission.SetMissionMode(2, true);
			this._escapeShipTargetSpeed = 0f;
			this._escapeShipSpeed = 0f;
			MatrixFrame matrixFrame = this.EscapeShip.GameEntity.GetBodyWorldTransform();
			this._escapeShipTargetDirection = matrixFrame.rotation.f.AsVec2.Normalized();
			matrixFrame = this.EscapeShip.GameEntity.GetBodyWorldTransform();
			this._escapeShipDirection = matrixFrame.rotation.f.AsVec2.Normalized();
		}

		// Token: 0x06000723 RID: 1827 RVA: 0x0002F710 File Offset: 0x0002D910
		private void InitializePhase2Part3()
		{
			this.SetDisableShipAttachmentMachinesForPlayer(this.EscapeShip, true);
			this.SpawnPhase2AllyTroops();
			this.SpawnPhase2EnemyTroops();
			if (this._isCheckpointInitialize)
			{
				Mission.Current.OnDeploymentFinished();
			}
			else
			{
				this._phase2EnemyShip1.OnDeploymentFinished();
				this._phase2EnemyShip2.OnDeploymentFinished();
				this._phase2EnemyShip3.OnDeploymentFinished();
				this._phase2EnemyShip4.OnDeploymentFinished();
				this._phase2EnemyShip5.OnDeploymentFinished();
				this._phase2EnemyShipStationary1.OnDeploymentFinished();
				this._phase2AllyShip1.OnDeploymentFinished();
				this._phase2AllyShip2.OnDeploymentFinished();
				this._phase2AllyShip3.OnDeploymentFinished();
				this._phase2AllyShip4.OnDeploymentFinished();
				this._phase2AllyShip5.OnDeploymentFinished();
				this._navalTrajectoryPlanningLogic.ForceReinitialize();
			}
			this._lightScriptedFiresMissionController.TriggerFiring();
			this._gunnarAgent.Controller = 0;
			this.HandlePlayersBridgeAndControlPointUsagesForPhase2InProgress();
			this.RemoveShipControlPointDescriptionOfAllEnemyShips();
			this._isMissionShipBoardedToTheEscapeShip.Add(this._phase2EnemyShip1, false);
			this._isMissionShipBoardedToTheEscapeShip.Add(this._phase2EnemyShip2, false);
			this._isMissionShipBoardedToTheEscapeShip.Add(this._phase2EnemyShip3, false);
			this._isMissionShipBoardedToTheEscapeShip.Add(this._phase2EnemyShip4, false);
			this._isMissionShipBoardedToTheEscapeShip.Add(this._phase2EnemyShip5, false);
			this._phase2EscapeShipPirateTargetFrame1 = Mission.Current.Scene.FindEntityWithTag("phase_2_anchor_1");
			this._phase2EscapeShipPirateTargetFrame2 = Mission.Current.Scene.FindEntityWithTag("phase_2_anchor_2");
			this._phase2EscapeShipPirateTargetFrame3 = Mission.Current.Scene.FindEntityWithTag("phase_2_anchor_3");
			this._phase2EscapeShipPirateTargetFrame4 = Mission.Current.Scene.FindEntityWithTag("phase_2_anchor_4");
			this._phase2EscapeShipPirateTargetFrame5 = Mission.Current.Scene.FindEntityWithTag("phase_2_anchor_5");
			this.EscapeShip.SetCustomSailSetting(true, SailInput.Full);
			if (!this._isCheckpointInitialize)
			{
				this.ClearPhase1OnPhaseTransition();
			}
		}

		// Token: 0x06000724 RID: 1828 RVA: 0x0002F8EC File Offset: 0x0002DAEC
		private void InitializePhase2Part4()
		{
			if (this._isCheckpointInitialize)
			{
				this._gunnarAgent.Controller = 1;
				this._navalAgentsLogic.AssignCaptainToShip(this._gunnarAgent, this.EscapeShip, null);
				this._navalAgentsLogic.TransferAgentToShip(Agent.Main, this.EscapeShip);
				this.EscapeShip.ShipOrder.ManageShipDetachments();
				this._navalAgentsLogic.SetDeploymentMode(true);
				this._navalShipsLogic.SetDeploymentMode(true);
				this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this.EscapeShip);
				this._navalAgentsLogic.SetDeploymentMode(false);
				this._navalShipsLogic.SetDeploymentMode(false);
				this._gunnarAgent.Controller = 0;
				this._gunnarMovementState = Quest5SetPieceBattleMissionController.GunnarMovementState.UseTheEscapeShip;
				MatrixFrame matrixFrame;
				this._playerShip.GetNextCrewSpawnGlobalFrame(out matrixFrame);
				Agent.Main.TeleportToPosition(matrixFrame.origin);
				Agent.Main.SetClothingColor1(4279111698U);
				Agent.Main.SetClothingColor2(4279111698U);
				Agent.Main.UpdateSpawnEquipmentAndRefreshVisuals(this.GetScriptedStealthEquipment());
				this._gunnarAgent.UpdateSpawnEquipmentAndRefreshVisuals(this.GetScriptedStealthEquipment());
				this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.GunnarUsesShip;
				this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase2InProgress;
			}
			else
			{
				this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase1ToPhase2FadeIn;
			}
			this.RemoveGunnarsHelmet();
			this.ModifyMainAgentEquipmentForPhase2();
		}

		// Token: 0x06000725 RID: 1829 RVA: 0x0002FA28 File Offset: 0x0002DC28
		private void AddTriggerPointForPirateShip(MissionShip ship, GameEntity triggerPoint)
		{
			this._pirateShipTriggerPoints[ship] = triggerPoint;
			this._isPirateShipTriggered[ship] = false;
			this._isPirateShipMovementDisabled[ship] = false;
			this._pirateShipEnabledAttachmentMachine[ship] = null;
			this._isPirateShipMovingToTheEscapeShip[ship] = false;
			this._isPirateShipLostItsCrew[ship] = false;
			this._limitPirateShipChasingSpeed[ship] = false;
		}

		// Token: 0x06000726 RID: 1830 RVA: 0x0002FA90 File Offset: 0x0002DC90
		private void SpawnPhase2AllyTroops()
		{
			this._navalAgentsLogic.SetDeploymentMode(true);
			this._navalShipsLogic.SetDeploymentMode(true);
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._phase2AllyShip1, this.Phase2AllyShip1TroopCount);
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._phase2AllyShip2, this.Phase2AllyShip2TroopCount);
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._phase2AllyShip3, this.Phase2AllyShip3TroopCount);
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._phase2AllyShip4, this.Phase2AllyShip4TroopCount);
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._phase2AllyShip5, this.Phase2AllyShip5TroopCount);
			this.AddMissionShipTroops(this._phase2AllyShip1Troops, this._phase2AllyShip1, PartyBase.MainParty);
			this.AddMissionShipTroops(this._phase2AllyShip2Troops, this._phase2AllyShip2, PartyBase.MainParty);
			this.AddMissionShipTroops(this._phase2AllyShip3Troops, this._phase2AllyShip3, PartyBase.MainParty);
			this.AddMissionShipTroops(this._phase2AllyShip4Troops, this._phase2AllyShip4, PartyBase.MainParty);
			this.AddMissionShipTroops(this._phase2AllyShip5Troops, this._phase2AllyShip5, PartyBase.MainParty);
			this._navalAgentsLogic.SpawnNextBatch(0, false, null);
			this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this._phase2AllyShip1);
			this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this._phase2AllyShip2);
			this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this._phase2AllyShip3);
			this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this._phase2AllyShip4);
			this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this._phase2AllyShip5);
			this._navalAgentsLogic.SetDeploymentMode(false);
			this._navalShipsLogic.SetDeploymentMode(false);
		}

		// Token: 0x06000727 RID: 1831 RVA: 0x0002FC18 File Offset: 0x0002DE18
		private void SpawnPhase2EnemyTroops()
		{
			this._navalAgentsLogic.SetDeploymentMode(true);
			this._navalShipsLogic.SetDeploymentMode(true);
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._phase2EnemyShip1, this.Phase2EnemyShip1TroopCount);
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._phase2EnemyShip2, this.Phase2EnemyShip2TroopCount);
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._phase2EnemyShip3, this.Phase2EnemyShip3TroopCount);
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._phase2EnemyShip4, this.Phase2EnemyShip4TroopCount);
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._phase2EnemyShip5, this.Phase2EnemyShip5TroopCount);
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._phase2EnemyShipStationary1, this.Phase2EnemyShipStationary1TroopCount);
			this.AddMissionShipTroops(this._phase2EnemyShip1Troops, this._phase2EnemyShip1, null);
			this.AddMissionShipTroops(this._phase2EnemyShip2Troops, this._phase2EnemyShip2, null);
			this.AddMissionShipTroops(this._phase2EnemyShip3Troops, this._phase2EnemyShip3, null);
			this.AddMissionShipTroops(this._phase2EnemyShip4Troops, this._phase2EnemyShip4, null);
			this.AddMissionShipTroops(this._phase2EnemyShip5Troops, this._phase2EnemyShip5, null);
			this.AddMissionShipTroops(this._phase2EnemyShipStationary1Troops, this._phase2EnemyShipStationary1, null);
			this._navalAgentsLogic.SpawnNextBatch(2, false, null);
			this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this._phase2EnemyShip1);
			this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this._phase2EnemyShip2);
			this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this._phase2EnemyShip3);
			this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this._phase2EnemyShip4);
			this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this._phase2EnemyShip5);
			this._navalAgentsLogic.SetDeploymentMode(false);
			this._navalShipsLogic.SetDeploymentMode(false);
		}

		// Token: 0x06000728 RID: 1832 RVA: 0x0002FDB8 File Offset: 0x0002DFB8
		private void HandleEscapeShipMovement()
		{
			if (!this.EscapeShip.IsAIControlled)
			{
				this.EscapeShip.SetController(ShipControllerType.AI, true);
			}
			if (this._currentPhase2EscapeShipTargetPoint == null)
			{
				if (!Extensions.IsEmpty<GameEntity>(this._phase2EscapeShipTargetPoints))
				{
					this._currentPhase2EscapeShipTargetPoint = this._phase2EscapeShipTargetPoints.Dequeue();
				}
				else
				{
					this._currentPhase2EscapeShipTargetPoint = base.Mission.Scene.FindEntityWithTag("phase_3_enemy_ship_2_sp");
				}
				Vec2 vec;
				if (!this._isPirateShipMovementDisabled[this._phase2EnemyShip5])
				{
					vec = (this._currentPhase2EscapeShipTargetPoint.GetGlobalFrame().origin - this.EscapeShip.GameEntity.GetBodyWorldTransform().origin).AsVec2;
					this._escapeShipTargetDirection = vec.Normalized();
				}
				else
				{
					MatrixFrame matrixFrame = this.EscapeShip.GameEntity.GetBodyWorldTransform();
					vec = matrixFrame.rotation.f.AsVec2;
					this._escapeShipTargetDirection = vec.Normalized();
				}
				ShipOrder shipOrder = this.EscapeShip.ShipOrder;
				vec = this._currentPhase2EscapeShipTargetPoint.GlobalPosition.AsVec2;
				shipOrder.SetShipMovementOrder(in vec);
				this.EscapeShip.ShipOrder.SetOrderOarsmenLevel(2);
			}
			else
			{
				Vec3 globalPosition = this._currentPhase2EscapeShipTargetPoint.GlobalPosition;
				MatrixFrame matrixFrame = this.EscapeShip.GameEntity.GetBodyWorldTransform();
				if (globalPosition.NearlyEquals(ref matrixFrame.origin, 35f))
				{
					this._currentPhase2EscapeShipTargetPoint = null;
				}
			}
			if (this._currentPhase2EscapeShipTargetPoint != null)
			{
				this.EscapeShip.ShipOrder.SetOrderOarsmenLevel(2);
			}
		}

		// Token: 0x06000729 RID: 1833 RVA: 0x0002FF4A File Offset: 0x0002E14A
		private void HandleEscapeShipSpeed()
		{
			if (this.State == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase2InProgress)
			{
				this.AdjustWindDirectionAccordingToTargetFrame(this.EscapeShip.GlobalFrame, 1f, false);
				this._escapeShipTargetSpeed = (this.GetIsThereActiveBridgeToBetweenEscapeShipAndAnyPirateShips() ? 2.7f : 5f);
			}
		}

		// Token: 0x0600072A RID: 1834 RVA: 0x0002FF88 File Offset: 0x0002E188
		private void HandlePirateShipGettingCloseToEscapeShip(MissionShip pirateShip, GameEntity finalTargetFrameEntity, float gettingCloseSpeed, float fixedDt)
		{
			if (this._navalAgentsLogic.GetActiveAgentCountOfShip(pirateShip) > 0 && this._isPirateShipMovingToTheEscapeShip[pirateShip])
			{
				MatrixFrame globalFrameImpreciseForFixedTick = finalTargetFrameEntity.GetGlobalFrameImpreciseForFixedTick();
				MatrixFrame bodyWorldTransform = pirateShip.GameEntity.GetBodyWorldTransform();
				Vec2 asVec = bodyWorldTransform.origin.AsVec2;
				Vec2 vec = globalFrameImpreciseForFixedTick.origin.AsVec2 - asVec;
				float length = vec.Length;
				Vec2 asVec2 = this.EscapeShip.Physics.LinearVelocity.AsVec2;
				float num = ((length > 1E-06f) ? MathF.Min(gettingCloseSpeed, length / fixedDt) : 0f);
				Vec2 vec2 = ((length > 1E-06f) ? (vec / length) : new Vec2(1f, 0f));
				Vec2 vec3 = asVec2 + vec2 * num;
				if (this._limitPirateShipChasingSpeed[pirateShip])
				{
					vec3.ClampMagnitude(0f, vec3.Length * 0.5f);
				}
				Vec2 vec4 = asVec + vec3 * fixedDt;
				Vec2 vec5 = ((vec3.Length > 1E-06f) ? vec3.Normalized() : bodyWorldTransform.rotation.f.AsVec2.Normalized());
				float num2 = 1f - MathF.Min(length, 200f) / 200f;
				Vec2 vec6 = Vec2.Zero;
				if (length <= 4f)
				{
					vec6 = globalFrameImpreciseForFixedTick.rotation.f.AsVec2.Normalized();
				}
				else
				{
					vec6 = Vec2.Lerp(vec5, globalFrameImpreciseForFixedTick.rotation.f.AsVec2.Normalized(), num2);
				}
				pirateShip.MoveShipToTheTargetWithDirection(bodyWorldTransform, vec4, vec6, 5f, 2.5f, fixedDt);
			}
		}

		// Token: 0x0600072B RID: 1835 RVA: 0x00030154 File Offset: 0x0002E354
		private void HandlePirateShipMovement(MissionShip pirateShip, GameEntity finalTargetFrameEntity)
		{
			if (this._navalAgentsLogic.GetActiveAgentCountOfShip(pirateShip) > 0)
			{
				pirateShip.ShipOrder.SetCutLoose(false);
				if (this._isPirateShipMovingToTheEscapeShip[pirateShip])
				{
					MatrixFrame matrixFrame = pirateShip.GlobalFrame;
					if (matrixFrame.origin.Distance(finalTargetFrameEntity.GetGlobalFrame().origin) <= 60f)
					{
						pirateShip.Formation.SetMovementOrder(MovementOrder.MovementOrderCharge);
						pirateShip.Formation.SetTargetFormation(Agent.Main.Formation);
						pirateShip.ShipOrder.SetShipEngageOrder(this.EscapeShip);
						pirateShip.ShipOrder.SetBoardingTargetShip(this.EscapeShip);
					}
					matrixFrame = pirateShip.GameEntity.GetBodyWorldTransform();
					Vec2 vec = matrixFrame.origin.AsVec2;
					matrixFrame = finalTargetFrameEntity.GetGlobalFrame();
					if (vec.DistanceSquared(matrixFrame.origin.AsVec2) <= 2f)
					{
						if (this._pirateShipEnabledAttachmentMachine[pirateShip] != null)
						{
							this._pirateShipEnabledAttachmentMachine[pirateShip].SetEnabled(true);
							this._pirateShipEnabledAttachmentMachine[pirateShip].SetIsDisabledForAI(false);
							return;
						}
						ShipAttachmentMachine shipAttachmentMachine = null;
						float num = -1f;
						foreach (ShipAttachmentMachine shipAttachmentMachine2 in pirateShip.AttachmentMachines)
						{
							if (Vec3.DotProduct(shipAttachmentMachine2.GameEntity.GetGlobalFrame().rotation.f, this.EscapeShip.GameEntity.GetBodyWorldTransform().origin - shipAttachmentMachine2.GameEntity.GetGlobalFrame().origin) > 0f)
							{
								foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this.EscapeShip.AttachmentPointMachines)
								{
									float num2 = ShipAttachmentMachine.ComputePotentialAttachmentValue(shipAttachmentMachine2, shipAttachmentPointMachine, true, true, false);
									if (num2 > num)
									{
										num = num2;
										shipAttachmentMachine = shipAttachmentMachine2;
									}
								}
							}
						}
						if (shipAttachmentMachine != null)
						{
							this._pirateShipEnabledAttachmentMachine[pirateShip] = shipAttachmentMachine;
							return;
						}
						return;
					}
					else
					{
						pirateShip.Formation.SetMovementOrder(MovementOrder.MovementOrderCharge);
						pirateShip.Formation.SetTargetFormation(Agent.Main.Formation);
						pirateShip.ShipOrder.SetShipEngageOrder(this.EscapeShip);
						using (List<ShipAttachmentMachine>.Enumerator enumerator = pirateShip.AttachmentMachines.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								ShipAttachmentMachine shipAttachmentMachine3 = enumerator.Current;
								if (shipAttachmentMachine3.CurrentAttachment == null)
								{
									if (shipAttachmentMachine3.PilotAgent != null)
									{
										shipAttachmentMachine3.PilotAgent.StopUsingGameObject(true, 1);
									}
									shipAttachmentMachine3.SetDisabled(true);
								}
								else if (shipAttachmentMachine3.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopesPulling || shipAttachmentMachine3.CurrentAttachment.State == ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.RopeThrown)
								{
									shipAttachmentMachine3.CurrentAttachment.SetAttachmentState(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BrokenAndWaitingForRemoval);
									if (shipAttachmentMachine3.PilotAgent != null)
									{
										shipAttachmentMachine3.PilotAgent.StopUsingGameObject(true, 1);
									}
									shipAttachmentMachine3.SetDisabled(true);
								}
							}
							return;
						}
					}
				}
				if (this._isPirateShipTriggered[pirateShip])
				{
					MatrixFrame matrixFrame = pirateShip.GameEntity.GetBodyWorldTransform();
					float num3 = matrixFrame.origin.Distance(this._pirateShipTriggerPoints[pirateShip].GlobalPosition);
					matrixFrame = pirateShip.GlobalFrame;
					float num4 = matrixFrame.origin.Distance(this.EscapeShip.GlobalFrame.origin);
					if (num3 <= 40f || num4 < 40f)
					{
						pirateShip.ShipOrder.SetShipEngageOrder(this.EscapeShip);
						pirateShip.Formation.SetTargetFormation(this.EscapeShip.Formation);
						foreach (ShipAttachmentMachine shipAttachmentMachine4 in pirateShip.AttachmentMachines)
						{
							if (shipAttachmentMachine4.PilotAgent != null)
							{
								shipAttachmentMachine4.PilotAgent.StopUsingGameObject(true, 1);
							}
							shipAttachmentMachine4.SetDisabled(true);
						}
						foreach (ShipAttachmentPointMachine shipAttachmentPointMachine2 in pirateShip.AttachmentPointMachines)
						{
							if (shipAttachmentPointMachine2.PilotAgent != null)
							{
								shipAttachmentPointMachine2.PilotAgent.StopUsingGameObject(true, 1);
							}
							shipAttachmentPointMachine2.SetDisabled(false);
							foreach (StandingPoint standingPoint in shipAttachmentPointMachine2.StandingPoints)
							{
								standingPoint.SetDisabled(false);
							}
						}
						this._isPirateShipMovingToTheEscapeShip[pirateShip] = true;
						return;
					}
					pirateShip.SetShipOrderActive(true);
					ShipOrder shipOrder = pirateShip.ShipOrder;
					Vec2 vec = this._pirateShipTriggerPoints[pirateShip].GlobalPosition.AsVec2;
					shipOrder.SetShipMovementOrder(in vec);
					return;
				}
				else
				{
					if (this._isPirateShipLostItsCrew[pirateShip])
					{
						this._isPirateShipMovementDisabled[pirateShip] = true;
						this._isPirateShipTriggered[pirateShip] = false;
						this._isPirateShipMovingToTheEscapeShip[pirateShip] = false;
						pirateShip.SetAnchor(true, false, 1f);
						pirateShip.ShipOrder.SetShipStopOrder();
						pirateShip.Formation.SetMovementOrder(MovementOrder.MovementOrderStop);
						pirateShip.Formation.SetTargetFormation(null);
						foreach (ShipAttachmentMachine shipAttachmentMachine5 in pirateShip.AttachmentMachines)
						{
							shipAttachmentMachine5.SetDisabled(true);
						}
						using (List<ShipAttachmentPointMachine>.Enumerator enumerator2 = pirateShip.AttachmentPointMachines.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								ShipAttachmentPointMachine shipAttachmentPointMachine3 = enumerator2.Current;
								shipAttachmentPointMachine3.SetDisabled(false);
								foreach (StandingPoint standingPoint2 in shipAttachmentPointMachine3.StandingPoints)
								{
									standingPoint2.SetDisabled(false);
								}
							}
							return;
						}
					}
					if (this._isPirateShipMovementDisabled[pirateShip])
					{
						pirateShip.SetAnchor(true, false, 1f);
						pirateShip.ShipOrder.SetShipStopOrder();
						pirateShip.Formation.SetMovementOrder(MovementOrder.MovementOrderStop);
						foreach (ShipAttachmentMachine shipAttachmentMachine6 in pirateShip.AttachmentMachines)
						{
							shipAttachmentMachine6.SetDisabled(true);
						}
						using (List<ShipAttachmentPointMachine>.Enumerator enumerator2 = pirateShip.AttachmentPointMachines.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								ShipAttachmentPointMachine shipAttachmentPointMachine4 = enumerator2.Current;
								shipAttachmentPointMachine4.SetDisabled(false);
								foreach (StandingPoint standingPoint3 in shipAttachmentPointMachine4.StandingPoints)
								{
									standingPoint3.SetDisabled(false);
								}
							}
							return;
						}
					}
					if (this._pirateShipTriggerPoints[pirateShip].GlobalPosition.Distance(this.EscapeShip.GlobalFrame.origin) < 170f)
					{
						this._isPirateShipTriggered[pirateShip] = true;
						pirateShip.SetController(ShipControllerType.None, true);
						pirateShip.SetAnchor(false, false, 1f);
						pirateShip.SetShipOrderActive(true);
						ShipOrder shipOrder2 = pirateShip.ShipOrder;
						Vec2 vec = this._pirateShipTriggerPoints[pirateShip].GlobalPosition.AsVec2;
						shipOrder2.SetShipMovementOrder(in vec);
						if (this._instructionState == Quest5SetPieceBattleMissionController.Quest5InstructionState.WaitForEscapeQuietly)
						{
							this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.EscapeQuietly;
							return;
						}
					}
					else
					{
						pirateShip.SetAnchor(true, false, 1f);
						pirateShip.ShipOrder.SetShipStopOrder();
						pirateShip.SetShipOrderActive(false);
					}
				}
			}
		}

		// Token: 0x0600072C RID: 1836 RVA: 0x00030930 File Offset: 0x0002EB30
		private void HandlePirateShipSailModeAccordingToTheGlobalWindVelocity(MissionShip ship)
		{
			if (this._navalAgentsLogic.GetActiveAgentCountOfShip(ship) <= 0)
			{
				ship.SetAnchor(true, false, 1f);
				return;
			}
			MatrixFrame matrixFrame = this.EscapeShip.GlobalFrame;
			if (matrixFrame.origin.Distance(ship.GlobalFrame.origin) < 40f)
			{
				ship.SetCustomSailSetting(true, SailInput.Raised);
				return;
			}
			matrixFrame = ship.GlobalFrame;
			Vec2 vec = matrixFrame.rotation.f.AsVec2.Normalized();
			Vec2 vec2 = base.Mission.Scene.GetGlobalWindVelocity().Normalized();
			float num = MathF.Abs(Vec2.DotProduct(vec, vec2));
			ship.SetCustomSailSetting(true, (num > 0.75f) ? SailInput.Full : SailInput.Raised);
		}

		// Token: 0x0600072D RID: 1837 RVA: 0x000309E8 File Offset: 0x0002EBE8
		private bool GetIsThereActiveBridgeToBetweenEscapeShipAndAnyPirateShips()
		{
			return this.EscapeShip.GetIsThereActiveBridgeTo(this._phase2EnemyShip1) || this.EscapeShip.GetIsThereActiveBridgeTo(this._phase2EnemyShip2) || this.EscapeShip.GetIsThereActiveBridgeTo(this._phase2EnemyShip3) || this.EscapeShip.GetIsThereActiveBridgeTo(this._phase2EnemyShip4) || this.EscapeShip.GetIsThereActiveBridgeTo(this._phase2EnemyShip5);
		}

		// Token: 0x0600072E RID: 1838 RVA: 0x00030A54 File Offset: 0x0002EC54
		private void HandleStationaryShipMovement(MissionShip stationaryShip)
		{
			stationaryShip.SetAnchor(true, false, 1f);
			stationaryShip.ShipOrder.SetShipStopOrder();
			stationaryShip.SetShipOrderActive(false);
			stationaryShip.SetCustomSailSetting(true, SailInput.Raised);
			foreach (Agent agent in this._navalAgentsLogic.GetActiveAgentsOfShip(stationaryShip))
			{
				if (agent.IsUsingGameObject)
				{
					agent.StopUsingGameObject(true, 1);
				}
			}
			stationaryShip.Formation.SetTargetFormation(this._playerFormation);
		}

		// Token: 0x0600072F RID: 1839 RVA: 0x00030AF0 File Offset: 0x0002ECF0
		private void AutoEstablishConnectionsForPirateShips(MissionShip ship, GameEntity finalTargetFrameEntity)
		{
			if (!this._isPirateShipMovementDisabled[ship] && this._isPirateShipMovingToTheEscapeShip[ship])
			{
				if (this._autoEstablishConnectionsForPirateShips[ship] == null)
				{
					if (!this.EscapeShip.GetIsThereActiveBridgeTo(ship))
					{
						MatrixFrame matrixFrame = ship.GameEntity.GetBodyWorldTransform();
						Vec2 asVec = matrixFrame.origin.AsVec2;
						matrixFrame = finalTargetFrameEntity.GetGlobalFrame();
						if (asVec.DistanceSquared(matrixFrame.origin.AsVec2) <= 2f)
						{
							this._autoEstablishConnectionsForPirateShips[ship] = new MissionTimer(7f);
							return;
						}
					}
				}
				else if (this._autoEstablishConnectionsForPirateShips[ship].Check(false) && !this.EscapeShip.GetIsThereActiveBridgeTo(ship))
				{
					this.EscapeShip.TryToConnectionToAttachmentMachine(this._pirateShipEnabledAttachmentMachine[ship], true, false);
					this._autoEstablishConnectionsForPirateShips[ship] = null;
				}
			}
		}

		// Token: 0x06000730 RID: 1840 RVA: 0x00030BDC File Offset: 0x0002EDDC
		private void AutoCutLooseEmptyPirateShipIfPlayerDoesNotForALongTime(MissionShip ship)
		{
			if (this._autoCutLooseTimersForPirateShips[ship] == null)
			{
				if (this.EscapeShip.GetIsThereActiveBridgeTo(ship))
				{
					this._autoCutLooseTimersForPirateShips[ship] = new MissionTimer(25f);
					return;
				}
			}
			else if (this._autoCutLooseTimersForPirateShips[ship].Check(false))
			{
				this._isPirateShipLostItsCrew[ship] = true;
				this._isPirateShipMovingToTheEscapeShip[ship] = false;
				this._isPirateShipTriggered[ship] = false;
				this._isPirateShipMovementDisabled[ship] = true;
				foreach (ShipAttachmentMachine shipAttachmentMachine in ship.AttachmentMachines)
				{
					if (shipAttachmentMachine.CurrentAttachment != null)
					{
						shipAttachmentMachine.CurrentAttachment.SetAttachmentState(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BrokenAndWaitingForRemoval);
					}
				}
				foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in ship.AttachmentPointMachines)
				{
					if (shipAttachmentPointMachine.CurrentAttachment != null)
					{
						shipAttachmentPointMachine.CurrentAttachment.SetAttachmentState(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BrokenAndWaitingForRemoval);
					}
				}
				this._autoCutLooseTimersForPirateShips[ship] = null;
			}
		}

		// Token: 0x06000731 RID: 1841 RVA: 0x00030D1C File Offset: 0x0002EF1C
		private void SetShipAttachmentJointPhysicsEnabledForShip(MissionShip ship, bool enabled)
		{
			foreach (ShipAttachmentMachine shipAttachmentMachine in ship.ShipAttachmentMachines)
			{
				shipAttachmentMachine.SetShipAttachmentJointPhysicsEnabled(enabled);
			}
		}

		// Token: 0x06000732 RID: 1842 RVA: 0x00030D70 File Offset: 0x0002EF70
		private void SetDisableShipAttachmentMachinesForPlayer(MissionShip ship, bool isDisabled)
		{
			foreach (ShipAttachmentMachine shipAttachmentMachine in ship.ShipAttachmentMachines)
			{
				if (isDisabled)
				{
					shipAttachmentMachine.SetDisabled(false);
				}
				else
				{
					shipAttachmentMachine.SetEnabled(false);
				}
			}
		}

		// Token: 0x06000733 RID: 1843 RVA: 0x00030DD0 File Offset: 0x0002EFD0
		private void OnAttachmentBroken(ShipAttachmentMachine attachmentMachine, ShipAttachmentPointMachine attachmentPointMachine)
		{
			MissionShip ownerShip = attachmentMachine.OwnerShip;
			bool flag;
			if (ownerShip != this.EscapeShip && attachmentPointMachine != null && attachmentPointMachine.PilotAgent != null && attachmentPointMachine.PilotAgent == Agent.Main && this._isPirateShipMovingToTheEscapeShip.TryGetValue(ownerShip, out flag))
			{
				this._isPirateShipMovingToTheEscapeShip[ownerShip] = false;
				this._isPirateShipLostItsCrew[ownerShip] = true;
				foreach (ShipAttachmentMachine shipAttachmentMachine in ownerShip.ShipAttachmentMachines)
				{
					if (attachmentMachine.CurrentAttachment != null)
					{
						attachmentMachine.CurrentAttachment.SetAttachmentState(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BrokenAndWaitingForRemoval);
					}
					attachmentMachine.SetDisabled(true);
				}
				ownerShip.ShipControllerMachine.SetDisabled(true);
				foreach (ShipOarMachine shipOarMachine in ownerShip.LeftSideShipOarMachines)
				{
					shipOarMachine.SetDisabled(true);
				}
				foreach (ShipOarMachine shipOarMachine2 in ownerShip.RightSideShipOarMachines)
				{
					shipOarMachine2.SetDisabled(true);
				}
			}
		}

		// Token: 0x06000734 RID: 1844 RVA: 0x00030F24 File Offset: 0x0002F124
		private void HandleAllyShipMovementDuringPhase2(MissionShip ship)
		{
			ship.SetAnchor(true, false, 1f);
			ship.ShipOrder.SetShipStopOrder();
			ship.SetController(ShipControllerType.None, true);
		}

		// Token: 0x06000735 RID: 1845 RVA: 0x00030F48 File Offset: 0x0002F148
		private void HandlePirateShipBridgeConnectionCount(MissionShip pirateShip)
		{
			if (this.EscapeShip.GetIsThereActiveBridgeTo(pirateShip))
			{
				if (!this._isMissionShipBoardedToTheEscapeShip[pirateShip])
				{
					this._isMissionShipBoardedToTheEscapeShip[pirateShip] = true;
					MBInformationManager.DialogNotificationHandle dialogNotificationHandle = CampaignInformationManager.AddDialogLine(new TextObject("{=s3PsXlsG}They've grappled us!", null), NavalStorylineData.Gunnar.CharacterObject, null, 0, 3);
					this._dialogNotificationHandleCache.Add(dialogNotificationHandle);
				}
				bool flag = true;
				pirateShip.Formation.SetMovementOrder(MovementOrder.MovementOrderCharge);
				foreach (Agent agent in this._navalAgentsLogic.GetActiveAgentsOfShip(pirateShip))
				{
					agent.SetAutomaticTargetSelection(false);
					agent.SetTargetAgent(Agent.Main);
					flag = flag && this.EscapeShip.GetIsAgentOnShip(agent, false);
				}
				if (!flag || !Agent.Main.IsActive() || !this.EscapeShip.GetIsAgentOnShip(Agent.Main, false))
				{
					return;
				}
				MBInformationManager.DialogNotificationHandle dialogNotificationHandle2 = CampaignInformationManager.AddDialogLine(new TextObject("{=RUavLWSF}They're on deck!", null), NavalStorylineData.Gunnar.CharacterObject, null, 0, 3);
				this._dialogNotificationHandleCache.Add(dialogNotificationHandle2);
				this._isPirateShipMovingToTheEscapeShip[pirateShip] = false;
				this._isPirateShipLostItsCrew[pirateShip] = true;
				this._isPirateShipTriggered[pirateShip] = false;
				pirateShip.SetAnchor(true, false, 1f);
				pirateShip.Formation.SetMovementOrder(MovementOrder.MovementOrderCharge);
				foreach (ShipAttachmentMachine shipAttachmentMachine in pirateShip.ShipAttachmentMachines)
				{
					if (shipAttachmentMachine.CurrentAttachment != null)
					{
						shipAttachmentMachine.CurrentAttachment.SetAttachmentState(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BrokenAndWaitingForRemoval);
					}
					shipAttachmentMachine.SetDisabled(true);
				}
				pirateShip.ShipControllerMachine.SetDisabled(true);
				foreach (ShipOarMachine shipOarMachine in pirateShip.LeftSideShipOarMachines)
				{
					shipOarMachine.SetDisabled(true);
				}
				using (List<ShipOarMachine>.Enumerator enumerator3 = pirateShip.RightSideShipOarMachines.GetEnumerator())
				{
					while (enumerator3.MoveNext())
					{
						ShipOarMachine shipOarMachine2 = enumerator3.Current;
						shipOarMachine2.SetDisabled(true);
					}
					return;
				}
			}
			if (this._isMissionShipBoardedToTheEscapeShip[pirateShip])
			{
				this._isMissionShipBoardedToTheEscapeShip[pirateShip] = false;
			}
		}

		// Token: 0x06000736 RID: 1846 RVA: 0x000311C8 File Offset: 0x0002F3C8
		private bool AreAllPhase2PirateShipsEliminated()
		{
			return this._phase2EnemyShip1.Formation.CountOfUnits <= 0 && this._phase2EnemyShip2.Formation.CountOfUnits <= 0 && this._phase2EnemyShip3.Formation.CountOfUnits <= 0 && this._phase2EnemyShip4.Formation.CountOfUnits <= 0 && this._phase2EnemyShip5.Formation.CountOfUnits <= 0;
		}

		// Token: 0x06000737 RID: 1847 RVA: 0x0003123C File Offset: 0x0002F43C
		private void HandlePlayersBridgeAndControlPointUsagesForPhase2InProgress()
		{
			foreach (MissionShip missionShip in this._navalShipsLogic.AllShips)
			{
				foreach (ShipAttachmentMachine shipAttachmentMachine in missionShip.AttachmentMachines)
				{
					foreach (StandingPoint standingPoint in shipAttachmentMachine.StandingPoints)
					{
						standingPoint.IsDisabledForPlayers = false;
					}
				}
				foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in missionShip.AttachmentPointMachines)
				{
					foreach (StandingPoint standingPoint2 in shipAttachmentPointMachine.StandingPoints)
					{
						standingPoint2.IsDisabledForPlayers = false;
					}
				}
			}
		}

		// Token: 0x06000738 RID: 1848 RVA: 0x00031388 File Offset: 0x0002F588
		private void CheckForEscapeShipStuck()
		{
			if (this.CheckIfThereIsAnActiveAgentOfShip(this._phase2EnemyShip1) && this.CheckIfThereIsAnActiveAgentOfShip(this._phase2EnemyShip2) && this.CheckIfThereIsAnActiveAgentOfShip(this._phase2EnemyShip3) && this.CheckIfThereIsAnActiveAgentOfShip(this._phase2EnemyShip4) && this.CheckIfThereIsAnActiveAgentOfShip(this._phase2EnemyShip5))
			{
				if (this._phase2EscapeShipStuckTimer == null)
				{
					this._phase2EscapeShipStuckTimer = new MissionTimer(10f);
					this._phase2EscapeShipStuckCheckPosition = this.EscapeShip.GlobalFrame.origin;
					return;
				}
				if (this._phase2EscapeShipStuckTimer.Check(false))
				{
					if (this.EscapeShip.GlobalFrame.origin.NearlyEquals(ref this._phase2EscapeShipStuckCheckPosition, 3f))
					{
						this.IsEscapeShipStuck = true;
						return;
					}
					this._phase2EscapeShipStuckTimer = null;
					this._phase2EscapeShipStuckCheckPosition = Vec3.Invalid;
				}
			}
		}

		// Token: 0x06000739 RID: 1849 RVA: 0x00031463 File Offset: 0x0002F663
		private bool CheckIfThereIsAnActiveAgentOfShip(MissionShip ship)
		{
			return ship == null || !this._isPirateShipTriggered.ContainsKey(ship) || !this._isPirateShipTriggered[ship] || this._navalAgentsLogic.GetActiveAgentCountOfShip(ship) <= 0;
		}

		// Token: 0x0600073A RID: 1850 RVA: 0x00031496 File Offset: 0x0002F696
		public void HandleEscapeShipStuck()
		{
			this.IsEscapeShipStuck = false;
			this._phase2EscapeShipStuckTimer = null;
			this._phase2EscapeShipStuckCheckPosition = Vec3.Invalid;
			this._navalShipsLogic.TeleportShip(this.EscapeShip, this._currentPhase2EscapeShipTargetPoint.GetGlobalFrame(), true, false, true);
		}

		// Token: 0x0600073B RID: 1851 RVA: 0x000314D0 File Offset: 0x0002F6D0
		private void MoveEscapeShipAlongTheTrack(float fixedDt)
		{
			if (this._escapeShipSpeed != 0f)
			{
				Vec2 vec = this._escapeShipDirection * this._escapeShipSpeed * fixedDt;
				MatrixFrame bodyWorldTransform = this.EscapeShip.GameEntity.GetBodyWorldTransform();
				Vec2 vec2 = bodyWorldTransform.origin.AsVec2 + vec;
				this.EscapeShip.MoveShipToTheTargetWithDirection(bodyWorldTransform, vec2, this._escapeShipDirection, 100f, 2.5f, fixedDt);
			}
		}

		// Token: 0x0600073C RID: 1852 RVA: 0x00031548 File Offset: 0x0002F748
		private void UpdatePhase2MovingShipParameters(float dt)
		{
			this._escapeShipSpeed = MathF.Lerp(this._escapeShipSpeed, this._escapeShipTargetSpeed, dt * 0.25f, 1E-05f);
			this._escapeShipDirection = Vec2.Slerp(this._escapeShipDirection, this._escapeShipTargetDirection, dt * 0.15f);
		}

		// Token: 0x0600073D RID: 1853 RVA: 0x00031598 File Offset: 0x0002F798
		private void ModifyMainAgentEquipmentForPhase2()
		{
			ItemObject @object = MBObjectManager.Instance.GetObject<ItemObject>("nord_shield_tier_2_d");
			Equipment equipment = Agent.Main.SpawnEquipment.Clone(false);
			for (int i = 0; i < 12; i++)
			{
				ItemObject item = equipment[i].Item;
				if (item != null && item.StringId.Equals("Broad_Skaen"))
				{
					equipment[i] = new EquipmentElement(@object, null, null, false);
					break;
				}
			}
			Agent.Main.UpdateSpawnEquipmentAndRefreshVisuals(equipment);
		}

		// Token: 0x0600073E RID: 1854 RVA: 0x00031617 File Offset: 0x0002F817
		public void TriggerInitializePhase3()
		{
			this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase3Part1;
		}

		// Token: 0x0600073F RID: 1855 RVA: 0x00031621 File Offset: 0x0002F821
		public void CompletePhase2ToPhase3Transition()
		{
			this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase3InProgress;
		}

		// Token: 0x06000740 RID: 1856 RVA: 0x0003162C File Offset: 0x0002F82C
		private void InitializePhase3Part1()
		{
			this._gunnarMovementState = Quest5SetPieceBattleMissionController.GunnarMovementState.End;
			if (this._phase2EnemyShip1 != null)
			{
				this._phase2EnemyShip1.BreakAllExistingConnections();
				this.AddAvailableEnemyFormation(this._phase2EnemyShip1.Formation);
				this.RemoveShipInternal(this._phase2EnemyShip1);
				this._phase2EnemyShip1 = null;
			}
			else
			{
				this._isCheckpointInitialize = true;
			}
			if (this._isCheckpointInitialize)
			{
				TeamAINavalComponent teamAINavalComponent = new TeamAINavalComponent(base.Mission, base.Mission.AttackerTeam, 5f, 1f);
				base.Mission.AttackerTeam.AddTeamAI(teamAINavalComponent, false);
				base.Mission.AttackerTeam.AddTacticOption(new TacticNavalBalancedOffense(base.Mission.AttackerTeam));
				TeamAINavalComponent teamAINavalComponent2 = new TeamAINavalComponent(base.Mission, base.Mission.DefenderTeam, 5f, 1f);
				base.Mission.DefenderTeam.AddTeamAI(teamAINavalComponent2, false);
				base.Mission.DefenderTeam.AddTacticOption(new TacticNavalBalancedOffense(base.Mission.DefenderTeam));
			}
			if (this._phase2EnemyShip2 != null)
			{
				this._phase2EnemyShip2.BreakAllExistingConnections();
				this.AddAvailableEnemyFormation(this._phase2EnemyShip2.Formation);
				this.RemoveShipInternal(this._phase2EnemyShip2);
				this._phase2EnemyShip2 = null;
			}
			else
			{
				this._isCheckpointInitialize = true;
			}
			if (this._phase2EnemyShip3 != null)
			{
				this._phase2EnemyShip3.BreakAllExistingConnections();
				this.AddAvailableEnemyFormation(this._phase2EnemyShip3.Formation);
				this.RemoveShipInternal(this._phase2EnemyShip3);
				this._phase2EnemyShip3 = null;
			}
			else
			{
				this._isCheckpointInitialize = true;
			}
			if (this._phase2EnemyShip4 != null)
			{
				this._phase2EnemyShip4.BreakAllExistingConnections();
				this.AddAvailableEnemyFormation(this._phase2EnemyShip4.Formation);
				this.RemoveShipInternal(this._phase2EnemyShip4);
				this._phase2EnemyShip4 = null;
			}
			else
			{
				this._isCheckpointInitialize = true;
			}
			if (this._phase2EnemyShip5 != null)
			{
				this._phase2EnemyShip5.BreakAllExistingConnections();
				this.AddAvailableEnemyFormation(this._phase2EnemyShip5.Formation);
				this.RemoveShipInternal(this._phase2EnemyShip5);
				this._phase2EnemyShip5 = null;
			}
			else
			{
				this._isCheckpointInitialize = true;
			}
			if (this._phase1EnemyShip2 != null)
			{
				this._phase1EnemyShip2.BreakAllExistingConnections();
				this.AddAvailableEnemyFormation(this._phase1EnemyShip2.Formation);
				this.RemoveShipInternal(this._phase1EnemyShip2);
			}
			if (this._phase1EnemyShip4 != null)
			{
				this._phase1EnemyShip4.BreakAllExistingConnections();
				this.AddAvailableEnemyFormation(this._phase1EnemyShip4.Formation);
				this.RemoveShipInternal(this._phase1EnemyShip4);
			}
			if (this._phase2EnemyShipStationary1 != null)
			{
				this._phase2EnemyShipStationary1.BreakAllExistingConnections();
				this.AddAvailableEnemyFormation(this._phase2EnemyShipStationary1.Formation);
				this.RemoveShipInternal(this._phase2EnemyShipStationary1);
			}
			this._phase3EnemyShip1 = this.CreateShip("eastern_heavy_ship", "phase_3_enemy_ship_1_sp", this.GetAvailableEnemyFormation(), false, this._phase3EnemyShip1UpgradePieceList, null, true);
			this._phase3EnemyShip2 = this.CreateShip("aserai_heavy_ship", "phase_3_enemy_ship_2_sp", this.GetAvailableEnemyFormation(), false, this._phase3EnemyShip2UpgradePieceList, null, true);
			this._phase3EnemyShip3 = this.CreateShip("nord_medium_ship", "phase_3_enemy_ship_3_sp", this.GetAvailableEnemyFormation(), false, this._phase3EnemyShip3UpgradePieceList, null, true);
			this._phase3EnemyShip4 = this.CreateShip("nord_medium_ship", "phase_3_enemy_ship_4_sp", this.GetAvailableEnemyFormation(), false, this._phase3EnemyShip4UpgradePieceList, null, true);
			this._phase3EnemyShip5 = this.CreateShip("khuzait_heavy_ship", "phase_3_enemy_ship_5_sp", this.GetAvailableEnemyFormation(), false, this._phase3EnemyShip5UpgradePieceList, null, true);
			this._phase3EnemyShip1.SetCanBeTakenOver(false);
			this._phase3EnemyShip2.SetCanBeTakenOver(false);
			this._phase3EnemyShip3.SetCanBeTakenOver(false);
			this._phase3EnemyShip4.SetCanBeTakenOver(false);
			this._phase3EnemyShip5.SetCanBeTakenOver(false);
			if (this._phase2AllyShip1 != null)
			{
				GameEntity gameEntity = Mission.Current.Scene.FindEntityWithTag("phase_3_ally_ship_1_sp");
				this._navalShipsLogic.TeleportShip(this._phase2AllyShip1, gameEntity.GetGlobalFrame(), true, false, true);
			}
			else
			{
				this._phase2AllyShip1 = this.CreateShip("aserai_heavy_ship", "phase_3_ally_ship_1_sp", this.GetAvailableAllyFormation(), false, this._phase2AllyShip1UpgradePieceList, null, true);
			}
			if (this._phase2AllyShip2 != null)
			{
				GameEntity gameEntity2 = Mission.Current.Scene.FindEntityWithTag("phase_3_ally_ship_2_sp");
				this._navalShipsLogic.TeleportShip(this._phase2AllyShip2, gameEntity2.GetGlobalFrame(), true, false, true);
			}
			else
			{
				this._phase2AllyShip2 = this.CreateShip("nord_medium_ship", "phase_3_ally_ship_2_sp", this.GetAvailableAllyFormation(), false, this._phase2AllyShip2UpgradePieceList, null, true);
			}
			if (this._phase2AllyShip3 != null)
			{
				GameEntity gameEntity3 = Mission.Current.Scene.FindEntityWithTag("phase_3_ally_ship_3_sp");
				this._navalShipsLogic.TeleportShip(this._phase2AllyShip3, gameEntity3.GetGlobalFrame(), true, false, true);
			}
			else
			{
				this._phase2AllyShip3 = this.CreateShip("northern_medium_ship", "phase_3_ally_ship_3_sp", this.GetAvailableAllyFormation(), false, this._phase2AllyShip3UpgradePieceList, null, true);
			}
			if (this._phase2AllyShip4 != null)
			{
				GameEntity gameEntity4 = Mission.Current.Scene.FindEntityWithTag("phase_3_ally_ship_4_sp");
				this._navalShipsLogic.TeleportShip(this._phase2AllyShip4, gameEntity4.GetGlobalFrame(), true, false, true);
			}
			else
			{
				this._phase2AllyShip4 = this.CreateShip("sturgia_heavy_ship", "phase_3_ally_ship_4_sp", this.GetAvailableAllyFormation(), false, this._phase2AllyShip4UpgradePieceList, null, true);
			}
			if (this._phase2AllyShip5 != null)
			{
				GameEntity gameEntity5 = Mission.Current.Scene.FindEntityWithTag("phase_3_ally_ship_5_sp");
				this._navalShipsLogic.TeleportShip(this._phase2AllyShip5, gameEntity5.GetGlobalFrame(), true, false, true);
			}
			else
			{
				this._phase2AllyShip5 = this.CreateShip("northern_medium_ship", "phase_3_ally_ship_5_sp", this.GetAvailableAllyFormation(), false, this._phase2AllyShip5UpgradePieceList, null, true);
			}
			this._navalTrajectoryPlanningLogic.ForceReinitialize();
			foreach (MBInformationManager.DialogNotificationHandle dialogNotificationHandle in this._dialogNotificationHandleCache)
			{
				CampaignInformationManager.ClearDialogNotification(dialogNotificationHandle, true);
			}
			this._dialogNotificationHandleCache.Clear();
			if (!this._isCheckpointInitialize)
			{
				this._lightScriptedFiresMissionController.PutOutFires();
			}
		}

		// Token: 0x06000741 RID: 1857 RVA: 0x00031C04 File Offset: 0x0002FE04
		private void InitializePhase3Part2()
		{
			Mission.Current.Scene.SetAtmosphereWithName("TOD_naval_05_30_sunset");
			if (this._playerShip != null)
			{
				foreach (ShipAttachmentMachine shipAttachmentMachine in this._playerShip.AttachmentMachines)
				{
					if (shipAttachmentMachine.CurrentAttachment != null)
					{
						shipAttachmentMachine.CurrentAttachment.SetAttachmentState(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BrokenAndWaitingForRemoval);
					}
				}
				if (Agent.Main.IsUsingGameObject)
				{
					Agent.Main.StopUsingGameObject(true, 1);
				}
				this._navalAgentsLogic.TransferAgentToShip(Agent.Main, this._phase2AllyShip1);
				Agent gunnarAgent = this._gunnarAgent;
				if (gunnarAgent != null && gunnarAgent.IsActive())
				{
					this._gunnarAgent.Controller = 1;
					this._navalAgentsLogic.TransferAgentToShip(this._gunnarAgent, this._phase2AllyShip1);
				}
				if (this._playerShip != null)
				{
					if (this._gunnarAgent.IsUsingGameObject)
					{
						this._gunnarAgent.Controller = 1;
						this._gunnarAgent.StopUsingGameObject(true, 1);
					}
					this.RemoveShipInternal(this._playerShip);
				}
				if (this._phase1EnemyShip3 != null && this._phase1EnemyShip3.Team != null)
				{
					Agent gunnarAgent2 = this._gunnarAgent;
					if (gunnarAgent2 != null && gunnarAgent2.IsUsingGameObject)
					{
						this._gunnarAgent.StopUsingGameObjectMT(true, 1);
					}
					this._navalAgentsLogic.UnassignCaptainOfShip(this._phase1EnemyShip3);
					this.RemoveShipInternal(this._phase1EnemyShip3);
				}
				this._navalShipsLogic.SetDeploymentMode(true);
				this._navalAgentsLogic.SetDeploymentMode(true);
				this._playerShip = this.CreateShip("empire_heavy_ship", "phase_3_player_ship_sp", this._playerFormation, false, this._escapeShipUpgradePieceList, this.EscapeShipFigurehead, true);
				this._navalShipsLogic.SetDeploymentMode(false);
				this._navalAgentsLogic.SetDeploymentMode(false);
				this._navalAgentsLogic.TransferAgentToShip(Agent.Main, this._playerShip);
				Agent gunnarAgent3 = this._gunnarAgent;
				if (gunnarAgent3 != null && gunnarAgent3.IsActive())
				{
					this._navalAgentsLogic.TransferAgentToShip(this._gunnarAgent, this._playerShip);
				}
				this._playerShip.ShipOrder.SetShipStopOrder();
				this._playerShip.Formation.PlayerOwner = Agent.Main;
			}
			else
			{
				this._playerShip = this.CreateShip("empire_heavy_ship", "phase_3_player_ship_sp", this._playerFormation, false, this._escapeShipUpgradePieceList, this.EscapeShipFigurehead, true);
				this._navalAgentsLogic.SetDeploymentMode(true);
				this._navalShipsLogic.SetDeploymentMode(true);
				this._navalAgentsLogic.AddReservedTroopToShip(new PartyAgentOrigin(PartyBase.MainParty, CharacterObject.PlayerCharacter, -1, default(UniqueTroopDescriptor), false, false), this._playerShip);
				this._navalAgentsLogic.SetDeploymentMode(false);
				this._navalShipsLogic.SetDeploymentMode(false);
				this._navalAgentsLogic.SpawnNextBatch(0, false, null);
				this._playerShip.Formation.PlayerOwner = Agent.Main;
			}
			this._navalTrajectoryPlanningLogic.ForceReinitialize();
			Agent.Main.TeleportToPosition(this._playerShip.GetCaptainSpawnGlobalFrame().origin);
		}

		// Token: 0x06000742 RID: 1858 RVA: 0x00031F14 File Offset: 0x00030114
		private void ClearPhase2OnPhaseTransition()
		{
			this._phase2EnemyShip1 = null;
			this._phase2EnemyShip2 = null;
			this._phase2EnemyShip3 = null;
			this._phase2EnemyShip4 = null;
			this._phase2EnemyShip5 = null;
			this._phase2EnemyShipStationary1 = null;
			this._phase2EscapeShipPirateTargetFrame1 = null;
			this._phase2EscapeShipPirateTargetFrame2 = null;
			this._phase2EscapeShipPirateTargetFrame3 = null;
			this._phase2EscapeShipPirateTargetFrame4 = null;
			this._phase2EscapeShipPirateTargetFrame5 = null;
			this._currentPhase2EscapeShipTargetPoint = null;
			this._pirateShipTriggerPoints.Clear();
			this._isPirateShipTriggered.Clear();
			this._isPirateShipMovingToTheEscapeShip.Clear();
			this._isPirateShipLostItsCrew.Clear();
			this._limitPirateShipChasingSpeed.Clear();
			this._autoCutLooseTimersForPirateShips.Clear();
			this._isMissionShipBoardedToTheEscapeShip.Clear();
			this._phase2EscapeShipTargetPointEntities.Clear();
			this._phase2EscapeShipTargetPoints.Clear();
			this._playerLeftTheEscapeShipTimer = null;
			this._phase2EscapeShipStuckTimer = null;
			GC.Collect();
		}

		// Token: 0x06000743 RID: 1859 RVA: 0x00031FEC File Offset: 0x000301EC
		private void InitializePhase3Part3()
		{
			this._navalAgentsLogic.AssignCaptainToShip(Agent.Main, this._playerShip, null);
			this._playerShip.Formation.PlayerOwner = Agent.Main;
			this.SpawnPhase3EnemyTroops();
			this.SpawnPhase3AllyTroops();
			Agent.Main.SetClothingColor1(4279111698U);
			Agent.Main.SetClothingColor2(4279111698U);
			Agent.Main.UpdateSpawnEquipmentAndRefreshVisuals(Hero.MainHero.BattleEquipment);
			this._gunnarAgent.UpdateSpawnEquipmentAndRefreshVisuals(NavalStorylineData.Gunnar.CharacterObject.Equipment);
			this._gunnarAgent.TeleportToPosition(this._playerShip.GetCaptainSpawnGlobalFrame().origin);
			if (this._isCheckpointInitialize)
			{
				Mission.Current.OnDeploymentFinished();
			}
			else
			{
				this._phase3EnemyShip1.OnDeploymentFinished();
				this._phase3EnemyShip2.OnDeploymentFinished();
				this._phase3EnemyShip3.OnDeploymentFinished();
				this._phase3EnemyShip4.OnDeploymentFinished();
				this._phase3EnemyShip5.OnDeploymentFinished();
				this._playerShip.OnDeploymentFinished();
				this._navalTrajectoryPlanningLogic.ForceReinitialize();
			}
			this.TriggerShip(this._phase3EnemyShip1);
			this.TriggerShip(this._phase3EnemyShip2);
			this.TriggerShip(this._phase3EnemyShip3);
			this.TriggerShip(this._phase3EnemyShip4);
			this.TriggerShip(this._phase3EnemyShip5);
			this.TriggerShip(this._phase2AllyShip1);
			this.TriggerShip(this._phase2AllyShip2);
			this.TriggerShip(this._phase2AllyShip3);
			this.TriggerShip(this._phase2AllyShip4);
			this.TriggerShip(this._phase2AllyShip5);
			this._gunnarAgent.Controller = 1;
			this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.DefeatEnemies;
			this.State = (this._isCheckpointInitialize ? Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase3InProgress : Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase2ToPhase3FadeIn);
			this._playerShip.SetController(ShipControllerType.Player, true);
			this.HandlePlayersBridgeAndControlPointUsagesForPhase3InProgress();
			this.AdjustWindDirectionAccordingToTargetFrame(this._playerShip.GlobalFrame, 3f, false);
			this.ShowStartNotifications();
			this.RemoveShipControlPointDescriptionOfAllEnemyShips();
			this._phase3TotalEnemyCount = this.Phase3EnemyShip1TroopCount + this.Phase3EnemyShip2TroopCount + this.Phase3EnemyShip3TroopCount + this.Phase3EnemyShip4TroopCount + this.Phase3EnemyShip5TroopCount;
			foreach (Formation formation in base.Mission.PlayerTeam.FormationsIncludingEmpty)
			{
				formation.PlayerOwner = Agent.Main;
			}
			if (!this._gunnarAgent.IsAlarmed())
			{
				this._gunnarAgent.SetAlarmState(3);
			}
			if (!this._isCheckpointInitialize)
			{
				this.ClearPhase2OnPhaseTransition();
			}
		}

		// Token: 0x06000744 RID: 1860 RVA: 0x00032274 File Offset: 0x00030474
		private void SpawnPhase3EnemyTroops()
		{
			this._navalAgentsLogic.SetDeploymentMode(true);
			this._navalShipsLogic.SetDeploymentMode(true);
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._phase3EnemyShip1, this.Phase3EnemyShip1TroopCount);
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._phase3EnemyShip2, this.Phase3EnemyShip2TroopCount);
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._phase3EnemyShip3, this.Phase3EnemyShip3TroopCount);
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._phase3EnemyShip4, this.Phase3EnemyShip4TroopCount);
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._phase3EnemyShip5, this.Phase3EnemyShip5TroopCount);
			this.AddMissionShipTroops(this._phase3EnemyShip1Troops, this._phase3EnemyShip1, null);
			this.AddMissionShipTroops(this._phase3EnemyShip2Troops, this._phase3EnemyShip2, null);
			this.AddMissionShipTroops(this._phase3EnemyShip3Troops, this._phase3EnemyShip3, null);
			this.AddMissionShipTroops(this._phase3EnemyShip4Troops, this._phase3EnemyShip4, null);
			this.AddMissionShipTroops(this._phase3EnemyShip5Troops, this._phase3EnemyShip5, null);
			this._navalAgentsLogic.SpawnNextBatch(2, false, null);
			this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this._phase3EnemyShip1);
			this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this._phase3EnemyShip2);
			this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this._phase3EnemyShip3);
			this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this._phase3EnemyShip4);
			this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this._phase3EnemyShip5);
			this._navalAgentsLogic.SetDeploymentMode(false);
			this._navalShipsLogic.SetDeploymentMode(false);
		}

		// Token: 0x06000745 RID: 1861 RVA: 0x000323E8 File Offset: 0x000305E8
		private void SpawnPhase3AllyTroops()
		{
			this._navalAgentsLogic.SetDeploymentMode(true);
			this._navalShipsLogic.SetDeploymentMode(true);
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._playerShip, this.Phase3PlayerShipTroopCount + 2);
			this.AddMissionShipTroops(this._phase3PlayerShipTroops, this._playerShip, PartyBase.MainParty);
			if (this._isCheckpointInitialize)
			{
				this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._phase2AllyShip1, this.Phase2AllyShip1TroopCount + 2);
				this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._phase2AllyShip2, this.Phase2AllyShip2TroopCount + 2);
				this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._phase2AllyShip3, this.Phase2AllyShip3TroopCount + 2);
				this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._phase2AllyShip4, this.Phase2AllyShip4TroopCount + 2);
				this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._phase2AllyShip5, this.Phase2AllyShip5TroopCount + 2);
				this.AddMissionShipTroops(this._phase2AllyShip1Troops, this._phase2AllyShip1, PartyBase.MainParty);
				this.AddMissionShipTroops(this._phase2AllyShip2Troops, this._phase2AllyShip2, PartyBase.MainParty);
				this.AddMissionShipTroops(this._phase2AllyShip3Troops, this._phase2AllyShip3, PartyBase.MainParty);
				this.AddMissionShipTroops(this._phase2AllyShip4Troops, this._phase2AllyShip4, PartyBase.MainParty);
				this.AddMissionShipTroops(this._phase2AllyShip5Troops, this._phase2AllyShip5, PartyBase.MainParty);
			}
			this.SpawnBjolgurOnShip(this._phase2AllyShip2);
			this.SpawnLaharOnShip(this._phase2AllyShip3);
			if (this._gunnarAgent == null || !this._gunnarAgent.IsActive())
			{
				this.SpawnGunnarOnShip(this._playerShip);
			}
			this._gunnarAgent.SetMortalityState(2);
			this._navalAgentsLogic.SpawnNextBatch(0, false, null);
			this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this._playerShip);
			if (this._isCheckpointInitialize)
			{
				this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this._phase2AllyShip1);
				this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this._phase2AllyShip2);
				this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this._phase2AllyShip3);
				this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this._phase2AllyShip4);
				this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this._phase2AllyShip5);
			}
			this._navalAgentsLogic.SetDeploymentMode(false);
			this._navalShipsLogic.SetDeploymentMode(false);
		}

		// Token: 0x06000746 RID: 1862 RVA: 0x00032614 File Offset: 0x00030814
		private void CallReinforcement()
		{
			this._isReinforcementCalled = true;
			this._phase3EnemyReinforcementShip1 = this.CreateShip("empire_medium_ship", "phase_3_enemy_reinforcement_1_sp", this.GetAvailableEnemyFormation(), false, null, null, true);
			this._phase3EnemyReinforcementShip2 = this.CreateShip("nord_medium_ship", "phase_3_enemy_reinforcement_2_sp", this.GetAvailableEnemyFormation(), false, null, null, true);
			this._phase3EnemyReinforcementShip1.SetCanBeTakenOver(false);
			this._phase3EnemyReinforcementShip2.SetCanBeTakenOver(false);
		}

		// Token: 0x06000747 RID: 1863 RVA: 0x00032680 File Offset: 0x00030880
		private void InitializeReinforcement()
		{
			this._isReinforcementInitialized = true;
			this._navalAgentsLogic.SetDeploymentMode(true);
			this._navalShipsLogic.SetDeploymentMode(true);
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._phase3EnemyReinforcementShip1, this.Phase3EnemyReinforcementShip1TroopCount);
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._phase3EnemyReinforcementShip2, this.Phase3EnemyReinforcementShip2TroopCount);
			this._phase3TotalEnemyCount += this.Phase3EnemyReinforcementShip1TroopCount + this.Phase3EnemyReinforcementShip2TroopCount;
			this.AddMissionShipTroops(this._phase3EnemyReinforcementShip1Troops, this._phase3EnemyReinforcementShip1, null);
			this.AddMissionShipTroops(this._phase3EnemyReinforcementShip2Troops, this._phase3EnemyReinforcementShip2, null);
			this._navalAgentsLogic.SpawnNextBatch(2, false, null);
			this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this._phase3EnemyReinforcementShip1);
			this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this._phase3EnemyReinforcementShip2);
			this._navalAgentsLogic.SetDeploymentMode(false);
			this._navalShipsLogic.SetDeploymentMode(false);
			this._phase3EnemyReinforcementShip1.OnDeploymentFinished();
			this._phase3EnemyReinforcementShip2.OnDeploymentFinished();
			this._navalTrajectoryPlanningLogic.ForceReinitialize();
			base.Mission.PlayerEnemyTeam.MasterOrderController.SelectAllFormations(false);
			base.Mission.PlayerEnemyTeam.MasterOrderController.SetOrder(4);
			MBInformationManager.DialogNotificationHandle dialogNotificationHandle = CampaignInformationManager.AddDialogLine(new TextObject("{=jxQc5JVQ}Ah, gods - I see more of them coming up... No rest for my sword-arm today!", null), NavalStorylineData.Gunnar.CharacterObject, null, 0, 3);
			this._dialogNotificationHandleCache.Add(dialogNotificationHandle);
			this._phase3EnemyReinforcementShip1.ShipOrder.SetShipEngageOrder(true);
			this._phase3EnemyReinforcementShip2.ShipOrder.SetShipEngageOrder(true);
		}

		// Token: 0x06000748 RID: 1864 RVA: 0x000327FC File Offset: 0x000309FC
		private bool CanProceedToPhase4()
		{
			MBReadOnlyList<Agent> activeAgents = base.Mission.PlayerEnemyTeam.ActiveAgents;
			bool flag = activeAgents.Count <= 0;
			if (!flag)
			{
				bool flag2 = true;
				using (List<Agent>.Enumerator enumerator = activeAgents.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.Formation != null)
						{
							flag2 = false;
							break;
						}
					}
				}
				flag = flag2;
			}
			return flag;
		}

		// Token: 0x06000749 RID: 1865 RVA: 0x00032874 File Offset: 0x00030A74
		public void TriggerInitializePhase4()
		{
			if (Agent.Main.IsUsingGameObject)
			{
				Agent.Main.StopUsingGameObject(true, 1);
			}
			this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase4Part1;
		}

		// Token: 0x0600074A RID: 1866 RVA: 0x00032896 File Offset: 0x00030A96
		public void CompletePhase3ToPhase4Transition()
		{
			this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase4InProgress;
		}

		// Token: 0x0600074B RID: 1867 RVA: 0x000328A0 File Offset: 0x00030AA0
		private void ShowStartNotifications()
		{
			MBInformationManager.DialogNotificationHandle dialogNotificationHandle = CampaignInformationManager.AddDialogLine(new TextObject("{=a1IqRXcx}Ahoy to you, Gunnar! An exemplary escape! Is the captive safe?", null), NavalStorylineData.Lahar.CharacterObject, null, 0, 3);
			this._dialogNotificationHandleCache.Add(dialogNotificationHandle);
			MBInformationManager.DialogNotificationHandle dialogNotificationHandle2 = CampaignInformationManager.AddDialogLine(new TextObject("{=EdYmUbcM}You two snatched their ship right out from under their noses! A fine story to tell my brothers, if we survive this.", null), NavalStorylineData.Bjolgur.CharacterObject, null, 0, 3);
			this._dialogNotificationHandleCache.Add(dialogNotificationHandle2);
			MBInformationManager.DialogNotificationHandle dialogNotificationHandle3 = CampaignInformationManager.AddDialogLine(new TextObject("{=HgdLgYtA}Ahoy to you, Bjolgur! And ahoy to you, Lahar! She is indeed safe, with us. But now it looks like the whole pack of Hounds are coming baying out to meet us. You two brave fellows get on our flanks, and we'll meet them prow to prow", null), NavalStorylineData.Gunnar.CharacterObject, null, 0, 3);
			this._dialogNotificationHandleCache.Add(dialogNotificationHandle3);
		}

		// Token: 0x0600074C RID: 1868 RVA: 0x0003292C File Offset: 0x00030B2C
		private void ClearPhase4OnPhaseTransition()
		{
			if (this._phase2AllyShip1 != null)
			{
				((Ship)this._phase2AllyShip1.ShipOrigin).Owner = null;
			}
			if (this._phase2AllyShip2 != null)
			{
				((Ship)this._phase2AllyShip2.ShipOrigin).Owner = null;
			}
			if (this._phase2AllyShip3 != null)
			{
				((Ship)this._phase2AllyShip3.ShipOrigin).Owner = null;
			}
			if (this._phase2AllyShip4 != null)
			{
				((Ship)this._phase2AllyShip4.ShipOrigin).Owner = null;
			}
			if (this._phase2AllyShip5 != null)
			{
				((Ship)this._phase2AllyShip5.ShipOrigin).Owner = null;
			}
			this._phase2AllyShip1 = null;
			this._phase2AllyShip2 = null;
			this._phase2AllyShip3 = null;
			this._phase2AllyShip4 = null;
			this._phase2AllyShip5 = null;
			this._phase3EnemyShip1 = null;
			this._phase3EnemyShip2 = null;
			this._phase3EnemyShip3 = null;
			this._phase3EnemyShip4 = null;
			this._phase3EnemyShip5 = null;
			this._phase3EnemyReinforcementShip1 = null;
			this._phase3EnemyReinforcementShip2 = null;
			this._phase3TriggerVolumeBox = null;
			this._allyShipTargetKeysBuffer.Clear();
			this._assignedEnemyShips.Clear();
			GC.Collect();
		}

		// Token: 0x0600074D RID: 1869 RVA: 0x00032A45 File Offset: 0x00030C45
		public void TriggerInitializeBossFight()
		{
			this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializeBossFightPart1;
		}

		// Token: 0x0600074E RID: 1870 RVA: 0x00032A4F File Offset: 0x00030C4F
		public void CompletePhase4ToBossFightTransition()
		{
			this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.StartBossFightConversation;
		}

		// Token: 0x0600074F RID: 1871 RVA: 0x00032A5C File Offset: 0x00030C5C
		private void HandlePlayersBridgeAndControlPointUsagesForPhase3InProgress()
		{
			foreach (MissionShip missionShip in this._navalShipsLogic.AllShips)
			{
				foreach (ShipAttachmentMachine shipAttachmentMachine in missionShip.AttachmentMachines)
				{
					foreach (StandingPoint standingPoint in shipAttachmentMachine.StandingPoints)
					{
						standingPoint.IsDisabledForPlayers = false;
					}
				}
				foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in missionShip.AttachmentPointMachines)
				{
					foreach (StandingPoint standingPoint2 in shipAttachmentPointMachine.StandingPoints)
					{
						standingPoint2.IsDisabledForPlayers = false;
					}
				}
			}
		}

		// Token: 0x06000750 RID: 1872 RVA: 0x00032BA8 File Offset: 0x00030DA8
		public void OnPurigCutsceneStarted()
		{
			this._isPurigCutsceneStarted = true;
			this._playerShip.ShipOrder.SetShipStopOrder();
			this._playerShip.SetAnchor(true, false, 1f);
		}

		// Token: 0x06000751 RID: 1873 RVA: 0x00032BD4 File Offset: 0x00030DD4
		public void OnPurigShipCutsceneEnded()
		{
			this._playerShip.SetAnchor(false, false, 1f);
			if (this._isPlayerUsingShipAtTheStartOfThePurigCutscene)
			{
				Agent.Main.HandleStartUsingAction(this._playerStandingPointAtTheStartOfThePurigCutscene, -1);
				this._isPlayerUsingShipAtTheStartOfThePurigCutscene = false;
				this._playerStandingPointAtTheStartOfThePurigCutscene = null;
			}
			this._playerShip.ShipOrder.SetShipEngageOrder(this.Phase4PurigShip);
			this.Phase4PurigShip.ShipOrder.SetShipEngageOrder(this._playerShip);
			this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.DefeatPurigsShip;
		}

		// Token: 0x06000752 RID: 1874 RVA: 0x00032C50 File Offset: 0x00030E50
		private void CheckIfEnemyAgentFallIntoTheWater()
		{
			MBReadOnlyList<Agent> activeAgents = base.Mission.PlayerEnemyTeam.ActiveAgents;
			if (activeAgents.Count < 10)
			{
				for (int i = activeAgents.Count - 1; i >= 0; i--)
				{
					Agent agent = activeAgents[i];
					if (agent.IsInWater())
					{
						agent.FadeOut(true, false);
					}
				}
			}
		}

		// Token: 0x06000753 RID: 1875 RVA: 0x00032CA4 File Offset: 0x00030EA4
		public void GetIntendedMainAgentDirectionForBossFight(out Vec3 direction)
		{
			GameEntity gameEntity = Mission.Current.Scene.FindEntityWithTag("naval_boss_fight_player_sp");
			GameEntity gameEntity2 = Mission.Current.Scene.FindEntityWithTag("naval_boss_fight_enemy_boss_sp");
			direction = (gameEntity2.GlobalPosition - gameEntity.GlobalPosition).NormalizedCopy();
		}

		// Token: 0x06000754 RID: 1876 RVA: 0x00032CFC File Offset: 0x00030EFC
		private void CollectPurigCutsceneNotifications()
		{
			StringHelpers.SetCharacterProperties("QUEST_5_COMPANION", NavalStorylineData.Gunnar.CharacterObject, null, false);
			this._purigNotifications.Enqueue(new Quest5SetPieceBattleMissionController.ConversationSound(new TextObject("{=jm8pWVv6}Who dares provoke the Hounds in their lair? Is that you, {QUEST_5_COMPANION.NAME}? You and your companion? I will fall upon you like an eagle and tear out your livers, I will shatter your ships to splinters!", null), 2, NavalStorylineData.Purig.CharacterObject));
			this._purigNotifications.Enqueue(new Quest5SetPieceBattleMissionController.ConversationSound(new TextObject("{=qPaqVlQX}I will spill your blood upon the waters, I will send your corpses to the slimy depths!", null), 2, NavalStorylineData.Purig.CharacterObject));
			this._purigNotifications.Enqueue(new Quest5SetPieceBattleMissionController.ConversationSound(new TextObject("{=SdqOuRuL}Your skull will be a home for scuttling things and Ran shall make a toothpick of your shin-bone! Do you hear me!", null), 2, NavalStorylineData.Purig.CharacterObject));
		}

		// Token: 0x06000755 RID: 1877 RVA: 0x00032D94 File Offset: 0x00030F94
		private void CheckAndPlayPurigCutsceneNotifications()
		{
			if (this._isPurigCutsceneStarted && !Extensions.IsEmpty<Quest5SetPieceBattleMissionController.ConversationSound>(this._purigNotifications))
			{
				Quest5SetPieceBattleMissionController.ConversationSound conversationSound = this._purigNotifications.Dequeue();
				MBInformationManager.DialogNotificationHandle dialogNotificationHandle = CampaignInformationManager.AddDialogLine(conversationSound.Line, conversationSound.Character, null, 0, conversationSound.Priority);
				this._dialogNotificationHandleCache.Add(dialogNotificationHandle);
			}
		}

		// Token: 0x06000756 RID: 1878 RVA: 0x00032DE8 File Offset: 0x00030FE8
		private void InitializePhase4Part1()
		{
			this._playerShip.ShipOrder.SetShipStopOrder();
			this.Phase4PurigShip = this.CreateShip("purigs_roundship_storyline", "phase_4_purig_ship_sp", this.GetAvailableEnemyFormation(), false, this._phase4PurigsShipUpgradePieceList, null, true);
			this.Phase4PurigShip.SetCanBeTakenOver(false);
			if (this._playerShip == null)
			{
				this._isCheckpointInitialize = true;
				this._playerShip = this.CreateShip("ship_dromon_storyline", "phase_3_player_ship_sp", this._playerFormation, false, this._escapeShipUpgradePieceList, null, true);
			}
			this.CollectPurigCutsceneNotifications();
			this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase4Part2;
		}

		// Token: 0x06000757 RID: 1879 RVA: 0x00032E78 File Offset: 0x00031078
		private void InitializePhase4Part2()
		{
			this.Phase4PurigShip.SetController(ShipControllerType.AI, true);
			ShipOrder shipOrder = this.Phase4PurigShip.ShipOrder;
			Vec2 asVec = base.Mission.Scene.FindEntityWithTag("phase_3_enemy_ship_5_sp").GlobalPosition.AsVec2;
			shipOrder.SetShipMovementOrder(in asVec);
			this.SpawnPhase4EnemyTroops();
			this.Phase4PurigShip.OnDeploymentFinished();
			this._navalTrajectoryPlanningLogic.ForceReinitialize();
			if (this._isCheckpointInitialize)
			{
				this._navalAgentsLogic.SetDeploymentMode(true);
				this._navalShipsLogic.SetDeploymentMode(true);
				this._playerShip.Formation.PlayerOwner = Agent.Main;
				this._navalAgentsLogic.AddReservedTroopToShip(new PartyAgentOrigin(PartyBase.MainParty, CharacterObject.PlayerCharacter, -1, default(UniqueTroopDescriptor), false, false), this._playerShip);
				this.SpawnGunnarOnShip(this._playerShip);
				this._navalAgentsLogic.SpawnNextBatch(0, false, null);
				this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this._playerShip);
				this._navalAgentsLogic.SetDeploymentMode(false);
				this._navalShipsLogic.SetDeploymentMode(false);
				this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase4InProgress;
			}
			else
			{
				this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.Phase3ToPhase4FadeIn;
			}
			this.RemoveShipControlPointDescriptionOfAllEnemyShips();
			this._purigShipAgents = new List<Agent>(this._navalAgentsLogic.GetActiveAgentsOfShip(this.Phase4PurigShip));
		}

		// Token: 0x06000758 RID: 1880 RVA: 0x00032FC0 File Offset: 0x000311C0
		private void SpawnPhase4EnemyTroops()
		{
			this._navalAgentsLogic.SetDeploymentMode(true);
			this._navalShipsLogic.SetDeploymentMode(true);
			CharacterObject @object = MBObjectManager.Instance.GetObject<CharacterObject>("sea_hounds");
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this.Phase4PurigShip, 40);
			for (int i = 0; i < 40; i++)
			{
				this._navalAgentsLogic.AddReservedTroopToShip(new SimpleAgentOrigin(@object, -1, null, default(UniqueTroopDescriptor)), this.Phase4PurigShip);
			}
			this._navalAgentsLogic.SpawnNextBatch(2, false, null);
			this.SpawnImmortalAgents();
			this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(this.Phase4PurigShip);
			this._navalAgentsLogic.SetDeploymentMode(false);
			this._navalShipsLogic.SetDeploymentMode(false);
		}

		// Token: 0x06000759 RID: 1881 RVA: 0x00033078 File Offset: 0x00031278
		private void SpawnImmortalAgents()
		{
			GameEntity gameEntity = Mission.Current.Scene.FindEntityWithTag("sp_immortal_purig");
			AgentBuildData agentBuildData = new AgentBuildData(NavalStorylineData.Purig.CharacterObject).TroopOrigin(new PartyAgentOrigin(this._enemyParty.Party, NavalStorylineData.Purig.CharacterObject, -1, default(UniqueTroopDescriptor), false, false)).Team(base.Mission.PlayerEnemyTeam);
			Vec3 vec = gameEntity.GlobalPosition;
			AgentBuildData agentBuildData2 = agentBuildData.InitialPosition(ref vec);
			MatrixFrame matrixFrame = gameEntity.GetGlobalFrame();
			Vec2 vec2 = matrixFrame.rotation.f.AsVec2;
			AgentBuildData agentBuildData3 = agentBuildData2.InitialDirection(ref vec2).NoHorses(true).NoWeapons(false);
			this._purigAgent = Mission.Current.SpawnAgent(agentBuildData3, false);
			this._purigAgent.SetTeam(Team.Invalid, true);
			this._purigAgent.SetAlarmState(0);
			this._purigAgent.SetIsAIPaused(true);
			this._purigAgent.SetMortalityState(2);
			foreach (Agent agent in base.Mission.Agents)
			{
				if (agent.Team == base.Mission.PlayerEnemyTeam && this.Phase4PurigShip.GetIsAgentOnShip(agent, false))
				{
					this._purigShipAgents.Add(agent);
				}
			}
			CharacterObject @object = MBObjectManager.Instance.GetObject<CharacterObject>("sea_hounds");
			GameEntity gameEntity2 = Mission.Current.Scene.FindEntityWithTag("sp_immortal_bodyguard_1");
			AgentBuildData agentBuildData4 = new AgentBuildData(@object).TroopOrigin(new SimpleAgentOrigin(@object, -1, null, default(UniqueTroopDescriptor))).Team(base.Mission.PlayerEnemyTeam);
			vec = gameEntity2.GlobalPosition;
			AgentBuildData agentBuildData5 = agentBuildData4.InitialPosition(ref vec);
			matrixFrame = gameEntity2.GetGlobalFrame();
			vec2 = matrixFrame.rotation.f.AsVec2;
			AgentBuildData agentBuildData6 = agentBuildData5.InitialDirection(ref vec2).NoHorses(true).NoWeapons(false);
			this._purigBodyguard1 = Mission.Current.SpawnAgent(agentBuildData6, false);
			this._purigBodyguard1.SetTeam(Team.Invalid, true);
			this._purigBodyguard1.SetAlarmState(0);
			this._purigBodyguard1.SetIsAIPaused(true);
			this._purigBodyguard1.SetMortalityState(2);
			GameEntity gameEntity3 = Mission.Current.Scene.FindEntityWithTag("sp_immortal_bodyguard_2");
			AgentBuildData agentBuildData7 = new AgentBuildData(@object).TroopOrigin(new SimpleAgentOrigin(@object, -1, null, default(UniqueTroopDescriptor))).Team(base.Mission.PlayerEnemyTeam);
			vec = gameEntity3.GlobalPosition;
			AgentBuildData agentBuildData8 = agentBuildData7.InitialPosition(ref vec);
			matrixFrame = gameEntity3.GetGlobalFrame();
			vec2 = matrixFrame.rotation.f.AsVec2;
			AgentBuildData agentBuildData9 = agentBuildData8.InitialDirection(ref vec2).NoHorses(true).NoWeapons(false);
			this._purigBodyguard2 = Mission.Current.SpawnAgent(agentBuildData9, false);
			this._purigBodyguard2.SetTeam(Team.Invalid, true);
			this._purigBodyguard2.SetAlarmState(0);
			this._purigBodyguard2.SetIsAIPaused(true);
			this._purigBodyguard2.SetMortalityState(2);
		}

		// Token: 0x0600075A RID: 1882 RVA: 0x0003338C File Offset: 0x0003158C
		private void InitializeNavalBossFightPart1()
		{
			this._navalAgentsLogic.SetDeploymentMode(true);
			this._navalShipsLogic.SetDeploymentMode(true);
			this.Phase4PurigShip.ShipOrder.SetShipStopOrder();
			this.Phase4PurigShip.SetShipOrderActive(false);
			this.Phase4PurigShip.SetAnchor(true, false, 1f);
			this.BossFightConversationCameraGameEntity = Mission.Current.Scene.FindEntityWithTag("sp_boss_fight_camera");
			MBObjectManager.Instance.GetObject<CharacterObject>("gangradirs_kin_melee");
			MBObjectManager.Instance.GetObject<CharacterObject>("sea_hounds");
			this._duelPhaseAllyAgents = new List<Agent>();
			this._duelPhaseEnemyAgents = new List<Agent>();
			this._playerSpawnPointEntity = Mission.Current.Scene.FindEntityWithTag("naval_boss_fight_player_sp");
			if (Agent.Main.IsUsingGameObject)
			{
				Agent.Main.StopUsingGameObject(true, 1);
			}
			foreach (MissionShip missionShip in this._navalShipsLogic.AllShips)
			{
				missionShip.SetController(ShipControllerType.None, false);
			}
			Agent.Main.TeleportToPosition(this._playerSpawnPointEntity.GlobalPosition);
			List<GameEntity> list = new List<GameEntity>();
			this.GetAllyFrames(out list);
			if (this._gunnarAgent != null && this._gunnarAgent.IsActive())
			{
				this._gunnarAgent.ClearTargetFrame();
				GameEntity gameEntity = list.First<GameEntity>();
				this._gunnarAgent.TeleportToPosition(gameEntity.GlobalPosition);
				list.Remove(gameEntity);
				this._duelPhaseAllyAgents.Add(this._gunnarAgent);
			}
			if (this._bjolgurAgent == null || !this._bjolgurAgent.IsActive())
			{
				this.SpawnBjolgurOnShip(this._playerShip);
			}
			if (this._bjolgurAgent != null && this._bjolgurAgent.IsActive())
			{
				this._bjolgurAgent.ClearTargetFrame();
				GameEntity gameEntity2 = list.First<GameEntity>();
				this._bjolgurAgent.TeleportToPosition(gameEntity2.GlobalPosition);
				list.Remove(gameEntity2);
				this._duelPhaseAllyAgents.Add(this._bjolgurAgent);
			}
			this._enemyBossSpawnPointEntity = Mission.Current.Scene.FindEntityWithTag("naval_boss_fight_enemy_boss_sp");
			this._purigAgent.SetTeam(base.Mission.PlayerEnemyTeam, true);
			this._purigAgent.TeleportToPosition(this._enemyBossSpawnPointEntity.GlobalPosition);
			this._purigAgent.SetIsAIPaused(false);
			this._purigAgent.SetMortalityState(0);
			this._duelPhaseEnemyAgents.Add(this._purigAgent);
			this._navalAgentsLogic.AddAgentToShip(this._purigAgent, this.Phase4PurigShip);
			List<GameEntity> list2 = new List<GameEntity>();
			this.GetEnemyFrames(out list2);
			this._purigBodyguard1.SetTeam(base.Mission.PlayerEnemyTeam, true);
			this._purigBodyguard1.TeleportToPosition(list2[0].GlobalPosition);
			this._purigBodyguard1.SetIsAIPaused(false);
			this._purigBodyguard1.SetMortalityState(0);
			this._duelPhaseEnemyAgents.Add(this._purigBodyguard1);
			this._navalAgentsLogic.AddAgentToShip(this._purigBodyguard1, this.Phase4PurigShip);
			this._purigBodyguard2.SetTeam(base.Mission.PlayerEnemyTeam, true);
			this._purigBodyguard2.TeleportToPosition(list2[1].GlobalPosition);
			this._purigBodyguard2.SetIsAIPaused(false);
			this._purigBodyguard2.SetMortalityState(0);
			this._duelPhaseEnemyAgents.Add(this._purigBodyguard2);
			this._navalAgentsLogic.AddAgentToShip(this._purigBodyguard2, this.Phase4PurigShip);
			this.RemoveAllAgentsExcept(new List<Agent>
			{
				Agent.Main,
				this._gunnarAgent,
				this._bjolgurAgent,
				this._purigAgent,
				this._purigBodyguard1,
				this._purigBodyguard2
			});
			foreach (ShipAttachmentMachine shipAttachmentMachine in this.Phase4PurigShip.AttachmentMachines)
			{
				if (shipAttachmentMachine.IsShipAttachmentMachineBridged())
				{
					shipAttachmentMachine.DisconnectAttachment();
				}
				foreach (StandingPoint standingPoint in shipAttachmentMachine.StandingPoints)
				{
					standingPoint.IsDisabledForPlayers = true;
				}
			}
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in this.Phase4PurigShip.AttachmentPointMachines)
			{
				if (shipAttachmentPointMachine.CurrentAttachment != null)
				{
					shipAttachmentPointMachine.CurrentAttachment.SetAttachmentState(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BrokenAndWaitingForRemoval);
				}
				foreach (StandingPoint standingPoint2 in shipAttachmentPointMachine.StandingPoints)
				{
					standingPoint2.IsDisabledForPlayers = true;
				}
			}
			this.Phase4PurigShip.SetCustomSailSetting(true, SailInput.Raised);
			this.Phase4PurigShip.ShipOrder.SetShipStopOrder();
			this.Phase4PurigShip.SetAnchor(true, false, 1f);
			this._playerShip.ShipOrder.SetShipStopOrder();
			this._navalAgentsLogic.SetDeploymentMode(false);
			this._navalShipsLogic.SetDeploymentMode(false);
			this.ClearPhase4OnPhaseTransition();
			this._navalTrajectoryPlanningLogic.ForceReinitialize();
		}

		// Token: 0x0600075B RID: 1883 RVA: 0x000338F0 File Offset: 0x00031AF0
		private void InitializeNavalBossFightPart2()
		{
			foreach (Agent agent in this._duelPhaseAllyAgents.Concat<Agent>(this._duelPhaseEnemyAgents))
			{
				if (agent != Agent.Main)
				{
					this.ResetAgentForBossFight(agent);
				}
			}
			if (this._gunnarAgent != null && this._gunnarAgent.IsActive())
			{
				this._gunnarAgent.SetTargetPosition(this._gunnarAgent.Position.AsVec2);
				this._gunnarAgent.SetAlarmState(0);
			}
			if (this._bjolgurAgent != null && this._bjolgurAgent.IsActive())
			{
				this._bjolgurAgent.SetTargetPosition(this._bjolgurAgent.Position.AsVec2);
				this._bjolgurAgent.SetAlarmState(0);
			}
			Agent.Main.SetLookAgent(this._purigAgent);
			this._purigAgent.SetLookAgent(Agent.Main);
			foreach (Formation formation in base.Mission.Teams.Attacker.FormationsIncludingEmpty)
			{
				if (formation.CountOfUnits > 0)
				{
					formation.SetMovementOrder(MovementOrder.MovementOrderStop);
				}
			}
			foreach (Formation formation2 in base.Mission.Teams.Defender.FormationsIncludingEmpty)
			{
				if (formation2.CountOfUnits > 0)
				{
					formation2.SetMovementOrder(MovementOrder.MovementOrderStop);
				}
			}
		}

		// Token: 0x0600075C RID: 1884 RVA: 0x00033AB4 File Offset: 0x00031CB4
		private void RemoveAllAgentsExcept(List<Agent> exceptionAgents)
		{
			for (int i = base.Mission.Agents.Count - 1; i >= 0; i--)
			{
				Agent agent = base.Mission.Agents[i];
				if (agent.IsActive() && !exceptionAgents.Contains(agent))
				{
					agent.FadeOut(true, false);
				}
			}
		}

		// Token: 0x0600075D RID: 1885 RVA: 0x00033B09 File Offset: 0x00031D09
		public void StartBossFight(bool isDuel)
		{
			this._instructionState = Quest5SetPieceBattleMissionController.Quest5InstructionState.DefeatPurig;
			this.BossFightConversationCameraGameEntity = null;
			if (isDuel)
			{
				this.BossFightState = Quest5SetPieceBattleMissionController.BossFightStateEnum.Duel;
				this.StartBossFightDuelModeInternal();
				return;
			}
			this.BossFightState = Quest5SetPieceBattleMissionController.BossFightStateEnum.All;
			this.BossFightOutCome = Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerRefusedTheDuel;
			this.StartBossFightBattleModeInternal();
		}

		// Token: 0x0600075E RID: 1886 RVA: 0x00033B40 File Offset: 0x00031D40
		private void StartBossFightDuelModeInternal()
		{
			this.ResetAgentForBossFight(this._purigAgent);
			base.Mission.GetMissionBehavior<MissionConversationLogic>().DisableStartConversation(true);
			foreach (Agent agent in this._duelPhaseAllyAgents)
			{
				if (!agent.IsMainAgent)
				{
					agent.SetTeam(Team.Invalid, true);
					WorldPosition worldPosition = agent.GetWorldPosition();
					agent.SetScriptedPosition(ref worldPosition, false, 0);
					agent.SetLookAgent(Agent.Main);
				}
			}
			foreach (Agent agent2 in this._duelPhaseEnemyAgents)
			{
				if (agent2 != this._purigAgent)
				{
					agent2.SetTeam(Team.Invalid, true);
					WorldPosition worldPosition2 = agent2.GetWorldPosition();
					agent2.SetScriptedPosition(ref worldPosition2, false, 0);
					agent2.SetLookAgent(this._purigAgent);
					agent2.SetTargetPosition(agent2.Position.AsVec2);
				}
			}
			this._purigAgent.SetTargetAgent(Agent.Main);
			this._purigAgent.Formation.AI.ResetBehaviorWeights();
			this._purigAgent.HumanAIComponent.RefreshBehaviorValues(2, 2);
			this._purigAgent.SetAlarmState(3);
			this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.BossFightInProgressAsDuel;
		}

		// Token: 0x0600075F RID: 1887 RVA: 0x00033CA8 File Offset: 0x00031EA8
		private void StartBossFightBattleModeInternal()
		{
			foreach (Agent agent in this._duelPhaseAllyAgents.Concat<Agent>(this._duelPhaseEnemyAgents))
			{
				if (agent != Agent.Main)
				{
					this.ResetAgentForBossFight(agent);
				}
			}
			base.Mission.GetMissionBehavior<MissionConversationLogic>().DisableStartConversation(true);
			this._purigAgent.Formation.AI.ResetBehaviorWeights();
			this._purigAgent.HumanAIComponent.RefreshBehaviorValues(2, 2);
			this._purigAgent.SetAlarmState(3);
			base.Mission.PlayerTeam.SetIsEnemyOf(base.Mission.PlayerEnemyTeam, true);
			this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.BossFightInProgressAsAll;
			foreach (Agent agent2 in this._duelPhaseEnemyAgents)
			{
				agent2.Formation.AI.ResetBehaviorWeights();
				agent2.HumanAIComponent.RefreshBehaviorValues(2, 2);
				agent2.SetAlarmState(3);
			}
			foreach (Agent agent3 in this._duelPhaseAllyAgents)
			{
				if (!agent3.IsMainAgent)
				{
					agent3.SetAlarmState(3);
				}
			}
			base.Mission.PlayerTeam.PlayerOrderController.SelectAllFormations(false);
			base.Mission.PlayerTeam.PlayerOrderController.SetOrder(4);
			base.Mission.PlayerEnemyTeam.MasterOrderController.SelectAllFormations(false);
			base.Mission.PlayerEnemyTeam.MasterOrderController.SetOrder(4);
		}

		// Token: 0x06000760 RID: 1888 RVA: 0x00033E74 File Offset: 0x00032074
		private void ResetAgentForBossFight(Agent agent)
		{
			if (agent.IsUsingGameObject)
			{
				agent.StopUsingGameObject(true, 1);
			}
			agent.ClearTargetFrame();
			ActionIndexCache act_none = ActionIndexCache.act_none;
			float num = -0.2f;
			agent.SetActionChannel(1, ref act_none, false, 72L, 0f, 1f, num, 0.4f, 0f, false, -0.2f, 0, true);
			agent.SetActionChannel(0, ref act_none, false, 72L, 0f, 1f, num, 0.4f, 0f, false, -0.2f, 0, true);
		}

		// Token: 0x06000761 RID: 1889 RVA: 0x00033EF9 File Offset: 0x000320F9
		private void StartBossFightConversation()
		{
			this._gunnarAgent.SetMortalityState(0);
			MissionConversationLogic missionBehavior = base.Mission.GetMissionBehavior<MissionConversationLogic>();
			missionBehavior.DisableStartConversation(false);
			missionBehavior.StartConversation(this._purigAgent, false, false);
		}

		// Token: 0x06000762 RID: 1890 RVA: 0x00033F28 File Offset: 0x00032128
		private void GetAllyFrames(out List<GameEntity> allyFrames)
		{
			allyFrames = new List<GameEntity>();
			for (int i = 0; i < 2; i++)
			{
				GameEntity gameEntity = Mission.Current.Scene.FindEntityWithTag("naval_boss_fight_player_ally_sp_" + (i + 1));
				allyFrames.Add(gameEntity);
			}
		}

		// Token: 0x06000763 RID: 1891 RVA: 0x00033F74 File Offset: 0x00032174
		private void GetEnemyFrames(out List<GameEntity> enemyFrames)
		{
			enemyFrames = new List<GameEntity>();
			for (int i = 0; i < 2; i++)
			{
				GameEntity gameEntity = Mission.Current.Scene.FindEntityWithTag("naval_boss_fight_player_enemy_sp_" + (i + 1));
				enemyFrames.Add(gameEntity);
			}
		}

		// Token: 0x06000764 RID: 1892 RVA: 0x00033FC0 File Offset: 0x000321C0
		private void OnDuelOver(BattleSideEnum winnerSide)
		{
			AgentVictoryLogic missionBehavior = base.Mission.GetMissionBehavior<AgentVictoryLogic>();
			if (missionBehavior != null)
			{
				missionBehavior.SetCheerActionGroup(3);
			}
			if (missionBehavior != null)
			{
				missionBehavior.SetCheerReactionTimerSettings(0.25f, 3f);
			}
			this._winnerSide = winnerSide;
			if (winnerSide == base.Mission.PlayerTeam.Side)
			{
				if (this.BossFightState == Quest5SetPieceBattleMissionController.BossFightStateEnum.Duel)
				{
					this.BossFightOutCome = Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerAcceptedAndWonTheDuel;
				}
				MapEvent.PlayerMapEvent.SetOverrideWinner(base.Mission.PlayerTeam.Side);
			}
			else
			{
				this.BossFightOutCome = Quest5SetPieceBattleMissionController.BossFightOutComeEnum.PlayerDefeatedWaitingForConversation;
				MapEvent.PlayerMapEvent.SetOverrideWinner(base.Mission.PlayerEnemyTeam.Side);
			}
			this.LastHitCheckpoint = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.End;
			this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.End;
		}

		// Token: 0x06000765 RID: 1893 RVA: 0x00034074 File Offset: 0x00032274
		private MissionShip CreateShip(string shipHullId, string spawnPointId, Formation formation, bool spawnAnchored = false, List<KeyValuePair<string, string>> additionalUpgradePieces = null, Figurehead figurehead = null, bool checkForFreeArea = true)
		{
			GameEntity gameEntity = Mission.Current.Scene.FindEntityWithTag(spawnPointId);
			MatrixFrame globalFrame = gameEntity.GetGlobalFrame();
			float waterLevelAtPosition = Mission.Current.Scene.GetWaterLevelAtPosition(gameEntity.GlobalPosition.AsVec2, false, false);
			globalFrame.origin = new Vec3(gameEntity.GlobalPosition.x, gameEntity.GlobalPosition.y, waterLevelAtPosition, -1f);
			Ship ship = new Ship(Campaign.Current.ObjectManager.GetObject<ShipHull>(shipHullId));
			if (formation.Team == base.Mission.PlayerEnemyTeam)
			{
				ship.Owner = this._enemyParty.Party;
			}
			else if (formation.Team == base.Mission.PlayerTeam)
			{
				ship.Owner = PartyBase.MainParty;
			}
			if (additionalUpgradePieces != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in additionalUpgradePieces)
				{
					if (!string.IsNullOrEmpty(keyValuePair.Value))
					{
						ShipUpgradePiece @object = MBObjectManager.Instance.GetObject<ShipUpgradePiece>(keyValuePair.Value);
						ship.EquipUpgradePiece(keyValuePair.Key, @object);
					}
				}
			}
			if (figurehead != null)
			{
				ship.ChangeFigurehead(figurehead);
			}
			MatrixFrame identity = MatrixFrame.Identity;
			Vec3 globalPosition = gameEntity.GlobalPosition;
			globalPosition.z = base.Mission.Scene.GetWaterLevelAtPosition(gameEntity.GlobalPosition.AsVec2, true, false);
			identity.origin = globalPosition;
			identity.rotation.f = globalFrame.rotation.f.AsVec2.Normalized().ToVec3(0f);
			identity.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			MissionShip missionShip = this._navalShipsLogic.SpawnShip(ship, in identity, formation.Team, formation, spawnAnchored, 8, checkForFreeArea);
			missionShip.ShipOrder.FormationJoinShip(formation);
			return missionShip;
		}

		// Token: 0x06000766 RID: 1894 RVA: 0x00034260 File Offset: 0x00032460
		private Formation GetAvailableAllyFormation()
		{
			Formation formation = this._availableAllyFormations.FirstOrDefault<Formation>();
			if (formation != null)
			{
				this._availableAllyFormations.Remove(formation);
			}
			else
			{
				MBReadOnlyList<MissionShip> allShips = this._navalShipsLogic.AllShips;
				for (int i = allShips.Count - 1; i >= 0; i--)
				{
					MissionShip missionShip = allShips[i];
					if (missionShip.Formation.Team == base.Mission.PlayerTeam)
					{
						MBReadOnlyList<Agent> activeAgentsOfShip = this._navalAgentsLogic.GetActiveAgentsOfShip(missionShip);
						if (activeAgentsOfShip == null || Extensions.IsEmpty<Agent>(activeAgentsOfShip))
						{
							formation = missionShip.Formation;
							this.RemoveShipInternal(missionShip);
							this._navalTrajectoryPlanningLogic.ForceReinitialize();
							break;
						}
					}
				}
			}
			return formation;
		}

		// Token: 0x06000767 RID: 1895 RVA: 0x00034304 File Offset: 0x00032504
		private void SpawnGunnarOnShip(MissionShip ship)
		{
			WeakGameEntity gameEntity = Extensions.GetRandomElement<ShipOarMachine>(ship.LeftSideShipOarMachines).GameEntity;
			AgentBuildData agentBuildData = new AgentBuildData(NavalStorylineData.Gunnar.CharacterObject).TroopOrigin(new PartyAgentOrigin(PartyBase.MainParty, NavalStorylineData.Gunnar.CharacterObject, -1, default(UniqueTroopDescriptor), false, true)).Team(base.Mission.PlayerTeam);
			Vec3 globalPosition = gameEntity.GlobalPosition;
			AgentBuildData agentBuildData2 = agentBuildData.InitialPosition(ref globalPosition);
			Vec2 vec = gameEntity.GetGlobalFrame().rotation.f.AsVec2;
			vec = vec.Normalized();
			AgentBuildData agentBuildData3 = agentBuildData2.InitialDirection(ref vec).NoHorses(true).NoWeapons(false)
				.Banner(base.Mission.PlayerTeam.Banner);
			this._gunnarAgent = Mission.Current.SpawnAgent(agentBuildData3, false);
			this._navalAgentsLogic.SetIgnoreTroopCapacities(true);
			this._navalAgentsLogic.AddAgentToShip(this._gunnarAgent, ship);
			this._gunnarAgentNavalComponent = this._gunnarAgent.GetComponent<AgentNavalComponent>();
		}

		// Token: 0x06000768 RID: 1896 RVA: 0x00034404 File Offset: 0x00032604
		private void TriggerShip(MissionShip ship)
		{
			ship.SetAnchor(false, false, 1f);
			ship.Formation.SetControlledByAI(true, false);
			ship.SetShipOrderActive(true);
			ship.ShipOrder.SetShipEngageOrder(true);
		}

		// Token: 0x06000769 RID: 1897 RVA: 0x00034434 File Offset: 0x00032634
		private void SpawnCrusasOnShip(MissionShip ship)
		{
			WeakGameEntity gameEntity = Extensions.GetRandomElement<ShipOarMachine>(ship.LeftSideShipOarMachines).GameEntity;
			AgentBuildData agentBuildData = new AgentBuildData(NavalStorylineData.Prusas.CharacterObject).TroopOrigin(new PartyAgentOrigin(PartyBase.MainParty, NavalStorylineData.Prusas.CharacterObject, -1, default(UniqueTroopDescriptor), false, true)).Team(base.Mission.PlayerTeam);
			Vec3 globalPosition = gameEntity.GlobalPosition;
			AgentBuildData agentBuildData2 = agentBuildData.InitialPosition(ref globalPosition);
			Vec2 vec = gameEntity.GetGlobalFrame().rotation.f.AsVec2;
			vec = vec.Normalized();
			AgentBuildData agentBuildData3 = agentBuildData2.InitialDirection(ref vec).NoHorses(true).NoWeapons(false);
			this._crusasAgent = Mission.Current.SpawnAgent(agentBuildData3, false);
			this._navalAgentsLogic.AddAgentToShip(this._crusasAgent, ship);
		}

		// Token: 0x0600076A RID: 1898 RVA: 0x00034504 File Offset: 0x00032704
		private void SpawnLaharOnShip(MissionShip ship)
		{
			WeakGameEntity gameEntity = Extensions.GetRandomElement<ShipOarMachine>(ship.LeftSideShipOarMachines).GameEntity;
			AgentBuildData agentBuildData = new AgentBuildData(NavalStorylineData.Lahar.CharacterObject).TroopOrigin(new PartyAgentOrigin(PartyBase.MainParty, NavalStorylineData.Lahar.CharacterObject, -1, default(UniqueTroopDescriptor), false, true)).Team(base.Mission.PlayerTeam);
			Vec3 globalPosition = gameEntity.GlobalPosition;
			AgentBuildData agentBuildData2 = agentBuildData.InitialPosition(ref globalPosition);
			Vec2 vec = gameEntity.GetGlobalFrame().rotation.f.AsVec2;
			vec = vec.Normalized();
			AgentBuildData agentBuildData3 = agentBuildData2.InitialDirection(ref vec).NoHorses(true).NoWeapons(false);
			this._laharAgent = Mission.Current.SpawnAgent(agentBuildData3, false);
			this._navalAgentsLogic.AddAgentToShip(this._laharAgent, ship);
		}

		// Token: 0x0600076B RID: 1899 RVA: 0x000345D4 File Offset: 0x000327D4
		private void SpawnBjolgurOnShip(MissionShip ship)
		{
			WeakGameEntity gameEntity = Extensions.GetRandomElement<ShipOarMachine>(ship.LeftSideShipOarMachines).GameEntity;
			AgentBuildData agentBuildData = new AgentBuildData(NavalStorylineData.Bjolgur.CharacterObject).TroopOrigin(new PartyAgentOrigin(PartyBase.MainParty, NavalStorylineData.Bjolgur.CharacterObject, -1, default(UniqueTroopDescriptor), false, true)).Team(base.Mission.PlayerTeam);
			Vec3 globalPosition = gameEntity.GlobalPosition;
			AgentBuildData agentBuildData2 = agentBuildData.InitialPosition(ref globalPosition);
			Vec2 vec = gameEntity.GetGlobalFrame().rotation.f.AsVec2;
			vec = vec.Normalized();
			AgentBuildData agentBuildData3 = agentBuildData2.InitialDirection(ref vec).NoHorses(true).NoWeapons(false);
			this._bjolgurAgent = Mission.Current.SpawnAgent(agentBuildData3, false);
			this._navalAgentsLogic.AddAgentToShip(this._bjolgurAgent, ship);
		}

		// Token: 0x0600076C RID: 1900 RVA: 0x000346A2 File Offset: 0x000328A2
		private void AddAvailableAllyFormation(Formation formation)
		{
			if (!this._availableAllyFormations.Contains(formation))
			{
				this._availableAllyFormations.Add(formation);
				return;
			}
			Debug.FailedAssert("Formation has been already added.", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Storyline\\MissionControllers\\Quest5SetPieceBattleMissionController.cs", "AddAvailableAllyFormation", 6263);
		}

		// Token: 0x0600076D RID: 1901 RVA: 0x000346D8 File Offset: 0x000328D8
		private Formation GetAvailableEnemyFormation()
		{
			Formation formation = this._availableEnemyFormations.FirstOrDefault<Formation>();
			if (formation != null)
			{
				this._availableEnemyFormations.Remove(formation);
			}
			else
			{
				foreach (Formation formation2 in base.Mission.PlayerEnemyTeam.FormationsIncludingEmpty)
				{
					if (!this._navalShipsLogic.IsAShipAssignedToFormation(formation2))
					{
						formation = formation2;
						break;
					}
				}
				if (formation == null)
				{
					MissionShip missionShip = null;
					int num = 0;
					MBReadOnlyList<MissionShip> allShips = this._navalShipsLogic.AllShips;
					for (int i = allShips.Count - 1; i >= 0; i--)
					{
						MissionShip missionShip2 = allShips[i];
						if (missionShip2.Formation.Team == base.Mission.PlayerEnemyTeam)
						{
							MBReadOnlyList<Agent> activeAgentsOfShip = this._navalAgentsLogic.GetActiveAgentsOfShip(missionShip2);
							if (missionShip2 != this._phase3EnemyReinforcementShip1 && missionShip2 != this._phase3EnemyReinforcementShip2)
							{
								if (activeAgentsOfShip == null || Extensions.IsEmpty<Agent>(activeAgentsOfShip))
								{
									formation = missionShip2.Formation;
									this.RemoveShipInternal(missionShip2);
									this._navalTrajectoryPlanningLogic.ForceReinitialize();
									break;
								}
								if (missionShip == null || activeAgentsOfShip.Count < num)
								{
									missionShip = missionShip2;
									num = activeAgentsOfShip.Count;
								}
							}
						}
					}
					if (formation == null && missionShip != null)
					{
						formation = missionShip.Formation;
						this.RemoveShipInternal(missionShip);
						this._navalTrajectoryPlanningLogic.ForceReinitialize();
					}
				}
			}
			return formation;
		}

		// Token: 0x0600076E RID: 1902 RVA: 0x00034844 File Offset: 0x00032A44
		private void AddAvailableEnemyFormation(Formation formation)
		{
			if (!this._availableEnemyFormations.Contains(formation))
			{
				this._availableEnemyFormations.Add(formation);
				return;
			}
			Debug.FailedAssert("Formation has been already added.", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Storyline\\MissionControllers\\Quest5SetPieceBattleMissionController.cs", "AddAvailableEnemyFormation", 6349);
		}

		// Token: 0x0600076F RID: 1903 RVA: 0x0003487C File Offset: 0x00032A7C
		private void AdjustWindDirectionAccordingToTargetFrame(MatrixFrame frame, float windPowerMultiplier, bool addRandomRotation = false)
		{
			Vec2 vec = frame.rotation.f.AsVec2;
			Vec2 vec2 = vec.Normalized();
			Scene scene = Mission.Current.Scene;
			vec = vec2 * windPowerMultiplier;
			scene.SetGlobalWindVelocity(ref vec);
			Scene scene2 = Mission.Current.Scene;
			vec = vec2 * windPowerMultiplier;
			scene2.SetGlobalWindStrengthVector(ref vec);
		}

		// Token: 0x06000770 RID: 1904 RVA: 0x000348D8 File Offset: 0x00032AD8
		private void TriggerMissionFailPopup()
		{
			this._isMissionFailPopUpTriggered = true;
			InformationManager.ShowInquiry(new InquiryData(new TextObject("{=wQbfWNZO}Mission Failed!", null).ToString(), new TextObject("{=xOhvBfoE}You have been caught.", null).ToString(), true, false, GameTexts.FindText("str_ok", null).ToString(), string.Empty, new Action(this.EndMissionWithAutoContinueFromCheckpoint), null, "", 0f, null, null, null), true, false);
		}

		// Token: 0x06000771 RID: 1905 RVA: 0x0003494C File Offset: 0x00032B4C
		private void CheckIfMainAgentLeftTheEscapeShip()
		{
			if (Agent.Main.IsActive())
			{
				if (this.EscapeShip.GetIsAgentOnShip(Agent.Main, false))
				{
					this._playerLeftTheEscapeShipTimer = null;
					return;
				}
				if (this._playerLeftTheEscapeShipTimer == null)
				{
					MBInformationManager.DialogNotificationHandle dialogNotificationHandle = CampaignInformationManager.AddDialogLine(new TextObject("{=n17xuLkd*}Get back on our ship! Don't risk getting left behind!", null), NavalStorylineData.Gunnar.CharacterObject, null, 0, 3);
					this._dialogNotificationHandleCache.Add(dialogNotificationHandle);
					this._playerLeftTheEscapeShipTimer = new MissionTimer(10f);
					return;
				}
				if (!this._isMissionFailPopUpTriggered && this._playerLeftTheEscapeShipTimer.Check(false))
				{
					this.TriggerMissionFailPopup();
					this._playerLeftTheEscapeShipTimer = null;
				}
			}
		}

		// Token: 0x06000772 RID: 1906 RVA: 0x000349EC File Offset: 0x00032BEC
		private void EndMissionWithAutoContinueFromCheckpoint()
		{
			this.ShouldMissionContinueFromCheckpoint = true;
			this.MakeGunnarStopUsingGameObjectBeforeMissionEnd();
			foreach (MBInformationManager.DialogNotificationHandle dialogNotificationHandle in this._dialogNotificationHandleCache)
			{
				CampaignInformationManager.ClearDialogNotification(dialogNotificationHandle, true);
			}
			this._dialogNotificationHandleCache.Clear();
			this.State = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.End;
		}

		// Token: 0x06000773 RID: 1907 RVA: 0x00034A60 File Offset: 0x00032C60
		private void RemoveGunnarsHelmet()
		{
			if (this._gunnarAgent != null && this._gunnarAgent.IsActive())
			{
				Equipment equipment = this.GetScriptedStealthEquipment().Clone(false);
				for (int i = 0; i < 12; i++)
				{
					if (i == 5)
					{
						equipment[i] = EquipmentElement.Invalid;
						break;
					}
				}
				this._gunnarAgent.UpdateSpawnEquipmentAndRefreshVisuals(equipment);
			}
		}

		// Token: 0x06000774 RID: 1908 RVA: 0x00034ABC File Offset: 0x00032CBC
		private void AddMissionShipTroops(List<KeyValuePair<string, int>> troops, MissionShip ship, PartyBase party = null)
		{
			this._navalAgentsLogic.SetIgnoreTroopCapacities(ship, true);
			foreach (KeyValuePair<string, int> keyValuePair in troops)
			{
				CharacterObject @object = MBObjectManager.Instance.GetObject<CharacterObject>(keyValuePair.Key);
				int value = keyValuePair.Value;
				for (int i = 0; i < value; i++)
				{
					if (party != null)
					{
						this._navalAgentsLogic.AddReservedTroopToShip(new PartyAgentOrigin(party, @object, -1, default(UniqueTroopDescriptor), false, true), ship);
					}
					else
					{
						this._navalAgentsLogic.AddReservedTroopToShip(new SimpleAgentOrigin(@object, -1, null, default(UniqueTroopDescriptor)), ship);
					}
				}
			}
		}

		// Token: 0x06000775 RID: 1909 RVA: 0x00034B80 File Offset: 0x00032D80
		private void HealMainHero()
		{
			Hero.MainHero.Heal(Hero.MainHero.MaxHitPoints, false);
			if (Agent.Main != null && Agent.Main.IsActive())
			{
				Agent.Main.Health = Agent.Main.HealthLimit;
			}
		}

		// Token: 0x06000776 RID: 1910 RVA: 0x00034BBE File Offset: 0x00032DBE
		private void RemoveShipInternal(MissionShip ship)
		{
			ship.BreakAllExistingConnections();
			Formation formation = ship.Formation;
			this._navalShipsLogic.RemoveShip(ship.Formation);
			formation.AI.ResetBehaviorWeights();
		}

		// Token: 0x06000777 RID: 1911 RVA: 0x00034BE8 File Offset: 0x00032DE8
		private void CutLooseAllBridgesOfTheShip(MissionShip ship)
		{
			foreach (ShipAttachmentMachine shipAttachmentMachine in ship.ShipAttachmentMachines)
			{
				if (shipAttachmentMachine.CurrentAttachment != null)
				{
					shipAttachmentMachine.CurrentAttachment.SetAttachmentState(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BrokenAndWaitingForRemoval);
				}
			}
			foreach (ShipAttachmentPointMachine shipAttachmentPointMachine in ship.AttachmentPointMachines)
			{
				if (shipAttachmentPointMachine.CurrentAttachment != null)
				{
					shipAttachmentPointMachine.CurrentAttachment.SetAttachmentState(ShipAttachmentMachine.ShipAttachment.ShipAttachmentState.BrokenAndWaitingForRemoval);
				}
			}
		}

		// Token: 0x06000778 RID: 1912 RVA: 0x00034C98 File Offset: 0x00032E98
		private void MakeGunnarStopUsingGameObjectBeforeMissionEnd()
		{
			if (this._gunnarAgent != null && this._gunnarAgent.IsActive())
			{
				this._gunnarAgent.Controller = 1;
				if (this._gunnarAgent.IsUsingGameObject)
				{
					this._gunnarAgent.StopUsingGameObjectMT(true, 1);
					return;
				}
				this._gunnarAgent.DisableScriptedMovement();
				if (this._gunnarAgent.IsAIControlled && AgentComponentExtensions.AIMoveToGameObjectIsEnabled(this._gunnarAgent))
				{
					AgentComponentExtensions.AIMoveToGameObjectDisable(this._gunnarAgent);
					Formation formation = this._gunnarAgent.Formation;
					if (formation == null)
					{
						return;
					}
					formation.Team.DetachmentManager.RemoveScoresOfAgentFromDetachments(this._gunnarAgent);
				}
			}
		}

		// Token: 0x06000779 RID: 1913 RVA: 0x00034D3C File Offset: 0x00032F3C
		private void SetLastCheckpoint(Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState state)
		{
			if (state == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializeStealthPhasePart1 || state == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase2Part1 || state == Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.InitializePhase3Part1)
			{
				this.LastHitCheckpoint = state;
				InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=BWSp3Uyj}Checkpoint reached.", null).ToString(), new Color(0f, 1f, 0f, 1f)));
				return;
			}
			Debug.FailedAssert("Unexpected checkpoint set!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Storyline\\MissionControllers\\Quest5SetPieceBattleMissionController.cs", "SetLastCheckpoint", 6537);
		}

		// Token: 0x0600077A RID: 1914 RVA: 0x00034DAC File Offset: 0x00032FAC
		private void TriggerPurigsDeadPopUp()
		{
			InformationManager.ShowInquiry(new InquiryData(new TextObject("{=dS3R9lW7}Success", null).ToString(), new TextObject("{=suHWcRSn}As you cut Purig down, there is a moment of silence. Then a great cheer wells up from your men. Gunnar closes his eyes and offers a muttered prayer to his gods. Meanwhile, with your sister foremost in your mind, you hurry back to the roundship.", null).ToString(), true, false, GameTexts.FindText("str_ok", null).ToString(), string.Empty, delegate
			{
				this.LastHitCheckpoint = Quest5SetPieceBattleMissionController.Quest5SetPieceBattleMissionState.End;
				MapEvent.PlayerMapEvent.SetOverrideWinner(base.Mission.PlayerTeam.Side);
				foreach (MBInformationManager.DialogNotificationHandle dialogNotificationHandle in this._dialogNotificationHandleCache)
				{
					CampaignInformationManager.ClearDialogNotification(dialogNotificationHandle, true);
				}
				this._dialogNotificationHandleCache.Clear();
				base.Mission.EndMission();
			}, null, "", 0f, null, null, null), true, false);
		}

		// Token: 0x0600077B RID: 1915 RVA: 0x00034E18 File Offset: 0x00033018
		private void MakeShipOarsInvisible(MissionShip ship)
		{
			foreach (WeakGameEntity weakGameEntity in ship.GameEntity.GetChildren())
			{
				if (weakGameEntity.Name.Equals("oars_holder"))
				{
					weakGameEntity.SetVisibilityExcludeParents(false);
					break;
				}
			}
		}

		// Token: 0x0600077C RID: 1916 RVA: 0x00034E84 File Offset: 0x00033084
		private void DisableAllShipOrderControllers()
		{
			foreach (MissionShip missionShip in this._navalShipsLogic.AllShips)
			{
				if (missionShip != this._playerShip)
				{
					this.DisableShipOrderController(missionShip);
				}
			}
		}

		// Token: 0x0600077D RID: 1917 RVA: 0x00034EE8 File Offset: 0x000330E8
		private void DisableShipOrderController(MissionShip ship)
		{
			ship.ShipOrder.SetShipStopOrder();
			ship.SetController(ShipControllerType.None, true);
			ship.SetShipOrderActive(false);
			foreach (MissionShip missionShip in this._navalShipsLogic.AllShips)
			{
				if (missionShip != ship && missionShip.ShipOrder.TargetShip == ship)
				{
					missionShip.ShipOrder.SetShipStopOrder();
					missionShip.ShipOrder.SetShipEngageOrder(true);
				}
			}
		}

		// Token: 0x0600077E RID: 1918 RVA: 0x00034F7C File Offset: 0x0003317C
		private void RemoveShipControlPointDescriptionOfAllEnemyShips()
		{
			foreach (MissionShip missionShip in this._navalShipsLogic.AllShips)
			{
				if (missionShip.Team == base.Mission.PlayerEnemyTeam)
				{
					this.RemoveShipControlPointDescriptionOfShip(missionShip);
				}
			}
		}

		// Token: 0x0600077F RID: 1919 RVA: 0x00034FE8 File Offset: 0x000331E8
		private void RemoveShipControlPointDescriptionOfShip(MissionShip ship)
		{
			ship.ShipControllerMachine.SetOverridenDescriptionForActiveEnemyShipControllerMachine(TextObject.GetEmpty());
		}

		// Token: 0x06000780 RID: 1920 RVA: 0x00034FFC File Offset: 0x000331FC
		private bool IsThereAnyShipBoardedToThePlayerShip()
		{
			bool flag = false;
			using (List<ShipAttachmentMachine>.Enumerator enumerator = this._playerShip.AttachmentMachines.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsShipAttachmentMachineBridged())
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				using (List<ShipAttachmentPointMachine>.Enumerator enumerator2 = this._playerShip.AttachmentPointMachines.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.CurrentAttachment != null)
						{
							flag = true;
							break;
						}
					}
				}
			}
			return flag;
		}

		// Token: 0x06000781 RID: 1921 RVA: 0x000350A8 File Offset: 0x000332A8
		private bool IsThereAnyEnemyShipsWithinRange(MissionShip missionShip, float range)
		{
			bool flag = false;
			foreach (MissionShip missionShip2 in this._navalShipsLogic.AllShips)
			{
				if (missionShip2.Team != missionShip.Team && missionShip2.Team != Team.Invalid && this._navalAgentsLogic.GetActiveAgentCountOfShip(missionShip2) > 0 && missionShip2.GameEntity.GlobalPosition.Distance(missionShip.GameEntity.GlobalPosition) <= range)
				{
					flag = true;
					break;
				}
			}
			return flag;
		}

		// Token: 0x06000782 RID: 1922 RVA: 0x00035154 File Offset: 0x00033354
		public void StartSpawner(BattleSideEnum side)
		{
		}

		// Token: 0x06000783 RID: 1923 RVA: 0x00035156 File Offset: 0x00033356
		public void StopSpawner(BattleSideEnum side)
		{
		}

		// Token: 0x06000784 RID: 1924 RVA: 0x00035158 File Offset: 0x00033358
		public bool IsSideSpawnEnabled(BattleSideEnum side)
		{
			return true;
		}

		// Token: 0x06000785 RID: 1925 RVA: 0x0003515B File Offset: 0x0003335B
		public float GetReinforcementInterval(BattleSideEnum side = -1)
		{
			return 0f;
		}

		// Token: 0x06000786 RID: 1926 RVA: 0x00035162 File Offset: 0x00033362
		public bool IsSideDepleted(BattleSideEnum side)
		{
			return false;
		}

		// Token: 0x06000787 RID: 1927 RVA: 0x00035165 File Offset: 0x00033365
		public IEnumerable<IAgentOriginBase> GetAllTroopsForSide(BattleSideEnum side)
		{
			return new List<IAgentOriginBase>();
		}

		// Token: 0x06000788 RID: 1928 RVA: 0x0003516C File Offset: 0x0003336C
		public bool GetSpawnHorses(BattleSideEnum side)
		{
			return true;
		}

		// Token: 0x06000789 RID: 1929 RVA: 0x0003516F File Offset: 0x0003336F
		public int GetNumberOfPlayerControllableTroops()
		{
			return base.Mission.PlayerTeam.ActiveAgents.Count - 1;
		}

		// Token: 0x04000364 RID: 868
		private const string SceneStealthPhaseAtmosphereName = "TOD_02_00_SemiCloudy";

		// Token: 0x04000365 RID: 869
		private const string SceneInteriorAtmosphereName = "TOD_01_00_SemiCloudy";

		// Token: 0x04000366 RID: 870
		private const string ScenePhase2AtmosphereName = "TOD_naval_03_00_sunset";

		// Token: 0x04000367 RID: 871
		private const string ScenePhase3AtmosphereName = "TOD_naval_05_30_sunset";

		// Token: 0x04000368 RID: 872
		private const string MainOarPrefabName = "oars_holder";

		// Token: 0x04000369 RID: 873
		private const float GunnarFellIntoTheWaterTimer = 10f;

		// Token: 0x0400036A RID: 874
		private const string RampHolderId = "ramp_holder";

		// Token: 0x0400036B RID: 875
		private const string GunnarInitialJumpOffPositionTag = "gangradir_jump_off_initial";

		// Token: 0x0400036C RID: 876
		private const string GunnarJumpOffTargetPositionTag = "gangradir_jump_off_target";

		// Token: 0x0400036D RID: 877
		private const string Phase1EnemyShip4GunnarHidingSpotStringId = "sp_gangradir_hiding_spot";

		// Token: 0x0400036E RID: 878
		private const float MaximumAllowedReachDistanceToPhase1EnemyShip1 = 25f;

		// Token: 0x0400036F RID: 879
		private const float AllowedSwimRadius = 200f;

		// Token: 0x04000370 RID: 880
		private const float AllowedSwimRadiusCheckFrequencyAsSeconds = 5f;

		// Token: 0x04000371 RID: 881
		private const string Phase1CustomStealthEquipmentId = "naval_storyline_quest5_stealth_set";

		// Token: 0x04000372 RID: 882
		private const string Phase1ApproachPointTag = "phase_1_approach_point";

		// Token: 0x04000373 RID: 883
		private const float Phase1ApproachDistance = 30f;

		// Token: 0x04000374 RID: 884
		private const float Phase1EscapePhaseAutoCutLooseTimer = 300f;

		// Token: 0x04000375 RID: 885
		private const string Phase1SlaveTraderAgentCharacterStringId = "sea_hounds";

		// Token: 0x04000376 RID: 886
		private const string Phase1StealthAgentCharacterStringId = "sea_hound_captivity";

		// Token: 0x04000377 RID: 887
		private const string Phase1PlayerShipStringId = "crusas_roundship_nested_q5";

		// Token: 0x04000378 RID: 888
		private const string Phase1PlayerShipSpawnPointTag = "phase_1_player_ship_sp";

		// Token: 0x04000379 RID: 889
		private const string Phase1EnemyShip1StringId = "sturgia_heavy_ship";

		// Token: 0x0400037A RID: 890
		private const string Phase1EnemyShip1SpawnPointTag = "phase_1_enemy_ship_1_sp_initial";

		// Token: 0x0400037B RID: 891
		private const string Phase1EnemyShip1TargetPointTag = "phase_1_enemy_ship_1_sp";

		// Token: 0x0400037C RID: 892
		private const int Phase1EnemyShip1TroopCount = 7;

		// Token: 0x0400037D RID: 893
		private const string Phase1EnemyShip2StringId = "ship_lodya_storyline";

		// Token: 0x0400037E RID: 894
		private const string Phase1EnemyShip2SpawnPointTag = "phase_1_enemy_ship_2_sp";

		// Token: 0x0400037F RID: 895
		private const int Phase1EnemyShip2TroopCount = 6;

		// Token: 0x04000380 RID: 896
		private const string Phase1EnemyShip2AttachmentPoint1Tag = "bridge_a";

		// Token: 0x04000381 RID: 897
		private const string Phase1EnemyShip2AttachmentPoint2Tag = "bridge_b";

		// Token: 0x04000382 RID: 898
		private const string Phase1EnemyShip2AttachmentPoint3Tag = "bridge_c";

		// Token: 0x04000383 RID: 899
		private const string Phase1EnemyShip3StringId = "ship_dromon_storyline";

		// Token: 0x04000384 RID: 900
		private const string Phase1EnemyShip3SpawnPointTag = "phase_1_enemy_ship_3_sp";

		// Token: 0x04000385 RID: 901
		private const int Phase1EnemyShip3TroopCount = 100;

		// Token: 0x04000386 RID: 902
		private const string Phase1EnemyShip3AttachmentPoint1Tag = "bridge_a";

		// Token: 0x04000387 RID: 903
		private const string Phase1EnemyShip3AttachmentPoint2Tag = "bridge_b";

		// Token: 0x04000388 RID: 904
		private const string Phase1EnemyShip3ToInteriorDoorTag = "phase_1_enemy_ship_3_to_interior_door_tag";

		// Token: 0x04000389 RID: 905
		private const string Phase1EnemyShip4StringId = "ship_birlinn_storyline";

		// Token: 0x0400038A RID: 906
		private const string Phase1EnemyShip4AttachmentPoint1Tag = "bridge_d";

		// Token: 0x0400038B RID: 907
		private const string Phase1EnemyShip4SpawnPointTag = "phase_1_enemy_ship_4_sp";

		// Token: 0x0400038C RID: 908
		private const int Phase1EnemyShip4TroopCount = 6;

		// Token: 0x0400038D RID: 909
		private const string Phase1EnemyShip4StealthCheckpointSpawnPointStringId = "sp_player_stealth_checkpoint";

		// Token: 0x0400038E RID: 910
		private const string Phase1InteriorMissionPlayerSpawnPointTag = "phase_1_interior_player_sp";

		// Token: 0x0400038F RID: 911
		private const string Phase1InteriorMissionSisterSpawnPointTag = "phase_1_interior_sister_sp";

		// Token: 0x04000390 RID: 912
		private const string Phase1InteriorToEnemyShip3DoorTag = "phase_1_interior_to_enemy_ship_3_door_tag";

		// Token: 0x04000391 RID: 913
		private const string CrusasPhase1EquipmentStringId = "npc_merchant_equipment_empire";

		// Token: 0x04000392 RID: 914
		private const string EscapeShipRoofUpgradeId = "roof_5";

		// Token: 0x04000393 RID: 915
		private const string EscapeShipDeckUpgradeId = "deck_large_arrow_and_javelin_crates_lvl3";

		// Token: 0x04000394 RID: 916
		private const string SlaveTraderShipOarsmanActionId = "act_sit_2";

		// Token: 0x04000395 RID: 917
		private const string SisterWoundedActionId = "act_conversation_weary2_loop";

		// Token: 0x04000396 RID: 918
		private const string Phase1InteriorCameraSisterTag = "phase_1_interior_camera_sister";

		// Token: 0x04000397 RID: 919
		private const string Phase2EscapeShipPirateTargetFrame1Tag = "phase_2_anchor_1";

		// Token: 0x04000398 RID: 920
		private const string Phase2EscapeShipPirateTargetFrame2Tag = "phase_2_anchor_2";

		// Token: 0x04000399 RID: 921
		private const string Phase2EscapeShipPirateTargetFrame3Tag = "phase_2_anchor_3";

		// Token: 0x0400039A RID: 922
		private const string Phase2EscapeShipPirateTargetFrame4Tag = "phase_2_anchor_4";

		// Token: 0x0400039B RID: 923
		private const string Phase2EscapeShipPirateTargetFrame5Tag = "phase_2_anchor_5";

		// Token: 0x0400039C RID: 924
		private const string Phase2EnemyShip1SpawnPointTag = "phase_2_enemy_ship_1_sp";

		// Token: 0x0400039D RID: 925
		private const string Phase2EnemyShip2SpawnPointTag = "phase_2_enemy_ship_2_sp";

		// Token: 0x0400039E RID: 926
		private const string Phase2EnemyShip3SpawnPointTag = "phase_2_enemy_ship_3_sp";

		// Token: 0x0400039F RID: 927
		private const string Phase2EnemyShip4SpawnPointTag = "phase_2_enemy_ship_4_sp";

		// Token: 0x040003A0 RID: 928
		private const string Phase2EnemyShip5SpawnPointTag = "phase_2_enemy_ship_5_sp";

		// Token: 0x040003A1 RID: 929
		private const string Phase2EnemyShipStationary1SpawnPointTag = "phase_2_enemy_ship_stationary_1";

		// Token: 0x040003A2 RID: 930
		private const string Phase2EnemyShip1TargetPointTag = "phase_2_enemy_ship_1_target";

		// Token: 0x040003A3 RID: 931
		private const string Phase2EnemyShip2TargetPointTag = "phase_2_enemy_ship_2_target";

		// Token: 0x040003A4 RID: 932
		private const string Phase2EnemyShip3TargetPointTag = "phase_2_enemy_ship_3_target";

		// Token: 0x040003A5 RID: 933
		private const string Phase2EnemyShip4TargetPointTag = "phase_2_enemy_ship_4_target";

		// Token: 0x040003A6 RID: 934
		private const string Phase2EnemyShip5TargetPointTag = "phase_2_enemy_ship_5_target";

		// Token: 0x040003A7 RID: 935
		private const string Phase2EnemyShip1StringId = "ship_meditlight_storyline_q5";

		// Token: 0x040003A8 RID: 936
		private const string Phase2EnemyShip2StringId = "ship_meditlight_storyline_q5";

		// Token: 0x040003A9 RID: 937
		private const string Phase2EnemyShip3StringId = "ship_meditlight_storyline_q5";

		// Token: 0x040003AA RID: 938
		private const string Phase2EnemyShip4StringId = "ship_meditlight_storyline_q5";

		// Token: 0x040003AB RID: 939
		private const string Phase2EnemyShip5StringId = "ship_meditlight_storyline_q5";

		// Token: 0x040003AC RID: 940
		private const string Phase2EnemyShipStationary1StringId = "western_medium_ship";

		// Token: 0x040003AD RID: 941
		private const string Phase2AllyShip1SpawnPointTag = "phase_2_ally_ship_1_sp";

		// Token: 0x040003AE RID: 942
		private const string Phase2AllyShip2SpawnPointTag = "phase_2_ally_ship_2_sp";

		// Token: 0x040003AF RID: 943
		private const string Phase2AllyShip3SpawnPointTag = "phase_2_ally_ship_3_sp";

		// Token: 0x040003B0 RID: 944
		private const string Phase2AllyShip4SpawnPointTag = "phase_2_ally_ship_4_sp";

		// Token: 0x040003B1 RID: 945
		private const string Phase2AllyShip5SpawnPointTag = "phase_2_ally_ship_5_sp";

		// Token: 0x040003B2 RID: 946
		private const string Phase2AllyShip1StringId = "aserai_heavy_ship";

		// Token: 0x040003B3 RID: 947
		private const string Phase2AllyShip2StringId = "nord_medium_ship";

		// Token: 0x040003B4 RID: 948
		private const string Phase2AllyShip3StringId = "northern_medium_ship";

		// Token: 0x040003B5 RID: 949
		private const string Phase2AllyShip4StringId = "sturgia_heavy_ship";

		// Token: 0x040003B6 RID: 950
		private const string Phase2AllyShip5StringId = "northern_medium_ship";

		// Token: 0x040003B7 RID: 951
		private const float AutoCutLoosePirateShipTimer = 25f;

		// Token: 0x040003B8 RID: 952
		private const float AutoEstablishConnectionsForPirateShipsTimer = 7f;

		// Token: 0x040003B9 RID: 953
		private const string Phase2EscapeShipTargetPointPrefix = "phase_2_escape_ship_target";

		// Token: 0x040003BA RID: 954
		private const string Phase2EscapeShipTargetPointExpression = "phase_2_escape_ship_target(_\\d+)*";

		// Token: 0x040003BB RID: 955
		private const string Phase2EscapeShipBarrierTag = "phase_2_barricade";

		// Token: 0x040003BC RID: 956
		private const string Phase3TriggerVolumeBoxTag = "phase_3_trigger_volume_box_tag";

		// Token: 0x040003BD RID: 957
		private const string Phase3EnemyShip1StringId = "eastern_heavy_ship";

		// Token: 0x040003BE RID: 958
		private const string Phase3EnemyShip2StringId = "aserai_heavy_ship";

		// Token: 0x040003BF RID: 959
		private const string Phase3EnemyShip3StringId = "nord_medium_ship";

		// Token: 0x040003C0 RID: 960
		private const string Phase3EnemyShip4StringId = "nord_medium_ship";

		// Token: 0x040003C1 RID: 961
		private const string Phase3EnemyShip5StringId = "khuzait_heavy_ship";

		// Token: 0x040003C2 RID: 962
		private const string Phase3EnemyShip1SpawnPointTag = "phase_3_enemy_ship_1_sp";

		// Token: 0x040003C3 RID: 963
		private const string Phase3EnemyShip2SpawnPointTag = "phase_3_enemy_ship_2_sp";

		// Token: 0x040003C4 RID: 964
		private const string Phase3EnemyShip3SpawnPointTag = "phase_3_enemy_ship_3_sp";

		// Token: 0x040003C5 RID: 965
		private const string Phase3EnemyShip4SpawnPointTag = "phase_3_enemy_ship_4_sp";

		// Token: 0x040003C6 RID: 966
		private const string Phase3EnemyShip5SpawnPointTag = "phase_3_enemy_ship_5_sp";

		// Token: 0x040003C7 RID: 967
		private const string Phase3EnemyShipReinforcementSpawnPoint1Tag = "phase_3_enemy_reinforcement_1_sp";

		// Token: 0x040003C8 RID: 968
		private const string Phase3EnemyShipReinforcementSpawnPoint2Tag = "phase_3_enemy_reinforcement_2_sp";

		// Token: 0x040003C9 RID: 969
		private const string Phase3EnemyReinforcementShip1StringId = "empire_medium_ship";

		// Token: 0x040003CA RID: 970
		private const string Phase3EnemyReinforcementShip2StringId = "nord_medium_ship";

		// Token: 0x040003CB RID: 971
		private const string Phase3EnemyReinforcementShip3StringId = "sturgia_heavy_ship";

		// Token: 0x040003CC RID: 972
		private const string Phase3AllyShip1SpawnPointTag = "phase_3_ally_ship_1_sp";

		// Token: 0x040003CD RID: 973
		private const string Phase3AllyShip2SpawnPointTag = "phase_3_ally_ship_2_sp";

		// Token: 0x040003CE RID: 974
		private const string Phase3AllyShip3SpawnPointTag = "phase_3_ally_ship_3_sp";

		// Token: 0x040003CF RID: 975
		private const string Phase3AllyShip4SpawnPointTag = "phase_3_ally_ship_4_sp";

		// Token: 0x040003D0 RID: 976
		private const string Phase3AllyShip5SpawnPointTag = "phase_3_ally_ship_5_sp";

		// Token: 0x040003D1 RID: 977
		private const string Phase3PlayerShipSpawnPointTag = "phase_3_player_ship_sp";

		// Token: 0x040003D2 RID: 978
		private const string Phase3PlayerShipStringId = "empire_heavy_ship";

		// Token: 0x040003D3 RID: 979
		private const string Phase3PlayerShipUsePointStringId = "sp_troop_captain";

		// Token: 0x040003D4 RID: 980
		private const string PurigsEnterenceTriggerBoxTag = "phase_4_purigs_entrance_trigger_box";

		// Token: 0x040003D5 RID: 981
		private const string PurigImmortalShipSpawnPointTag = "sp_immortal_purig";

		// Token: 0x040003D6 RID: 982
		private const string PurigBodyguard1ImmortalShipSpawnPointTag = "sp_immortal_bodyguard_1";

		// Token: 0x040003D7 RID: 983
		private const string PurigBodyguard2ImmortalShipSpawnPointTag = "sp_immortal_bodyguard_2";

		// Token: 0x040003D8 RID: 984
		private const string PurigShipSpawnPointTag = "phase_4_purig_ship_sp";

		// Token: 0x040003D9 RID: 985
		private const string PurigShipStringId = "purigs_roundship_storyline";

		// Token: 0x040003DA RID: 986
		private const string PurigShipTroopStringId = "sea_hounds";

		// Token: 0x040003DB RID: 987
		private const int PurigShipTroopCount = 40;

		// Token: 0x040003DC RID: 988
		private const string NavalBossFightPlayerSpawnPointTag = "naval_boss_fight_player_sp";

		// Token: 0x040003DD RID: 989
		private const string NavalBossFightPlayerAllySpawnPointTagPrefix = "naval_boss_fight_player_ally_sp_";

		// Token: 0x040003DE RID: 990
		private const string NavalBossFightEnemyBossSpawnPointTag = "naval_boss_fight_enemy_boss_sp";

		// Token: 0x040003DF RID: 991
		private const string NavalBossFightEnemyTroopSpawnPointTagPrefix = "naval_boss_fight_player_enemy_sp_";

		// Token: 0x040003E0 RID: 992
		private const int NavalBossFightAllyTroopCount = 2;

		// Token: 0x040003E1 RID: 993
		private const int NavalBossFightEnemyTroopCount = 2;

		// Token: 0x040003E2 RID: 994
		private const string NavalBossFightPlayerBodyguardTroopStringId = "gangradirs_kin_melee";

		// Token: 0x040003E3 RID: 995
		private const string NavalBossFightEnemyBodyguardTroopStringId = "sea_hounds";

		// Token: 0x040003E4 RID: 996
		private const string BossFightConversationCameraTag = "sp_boss_fight_camera";

		// Token: 0x040003E5 RID: 997
		private readonly List<KeyValuePair<string, string>> _phase1EnemyShip2UpgradePieceList = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("fore", ""),
			new KeyValuePair<string, string>("aft", ""),
			new KeyValuePair<string, string>("hull", ""),
			new KeyValuePair<string, string>("deck", ""),
			new KeyValuePair<string, string>("oars", ""),
			new KeyValuePair<string, string>("sail", ""),
			new KeyValuePair<string, string>("roof", "roof_7"),
			new KeyValuePair<string, string>("bow", ""),
			new KeyValuePair<string, string>("side", "")
		};

		// Token: 0x040003E6 RID: 998
		private readonly List<KeyValuePair<string, string>> _escapeShipUpgradePieceList = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("fore", ""),
			new KeyValuePair<string, string>("aft", ""),
			new KeyValuePair<string, string>("hull", ""),
			new KeyValuePair<string, string>("deck", "deck_large_arrow_and_javelin_crates_lvl3"),
			new KeyValuePair<string, string>("oars", ""),
			new KeyValuePair<string, string>("sail", ""),
			new KeyValuePair<string, string>("roof", "roof_5"),
			new KeyValuePair<string, string>("bow", ""),
			new KeyValuePair<string, string>("side", "")
		};

		// Token: 0x040003E7 RID: 999
		private readonly List<KeyValuePair<string, string>> _phase2AllyShip1UpgradePieceList = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("aft", "aft_battlement_lvl3_wbarracks"),
			new KeyValuePair<string, string>("hull", ""),
			new KeyValuePair<string, string>("deck", "deck_arrow_and_javelin_crates_lvl2"),
			new KeyValuePair<string, string>("oars", ""),
			new KeyValuePair<string, string>("sail", "sails_lvl2"),
			new KeyValuePair<string, string>("roof", ""),
			new KeyValuePair<string, string>("bow", ""),
			new KeyValuePair<string, string>("side", "side_southern_shields_lvl1")
		};

		// Token: 0x040003E8 RID: 1000
		private readonly List<KeyValuePair<string, string>> _phase2AllyShip2UpgradePieceList = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("fore", ""),
			new KeyValuePair<string, string>("aft", ""),
			new KeyValuePair<string, string>("hull", ""),
			new KeyValuePair<string, string>("deck", "deck_arrow_and_javelin_crates_lvl2"),
			new KeyValuePair<string, string>("oars", ""),
			new KeyValuePair<string, string>("sail", "sails_lvl3"),
			new KeyValuePair<string, string>("roof", ""),
			new KeyValuePair<string, string>("bow", ""),
			new KeyValuePair<string, string>("side", "side_northern_shields_lvl1")
		};

		// Token: 0x040003E9 RID: 1001
		private readonly List<KeyValuePair<string, string>> _phase2AllyShip3UpgradePieceList = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("fore", ""),
			new KeyValuePair<string, string>("aft", ""),
			new KeyValuePair<string, string>("hull", ""),
			new KeyValuePair<string, string>("deck", "deck_large_arrow_and_javelin_crates_lvl3"),
			new KeyValuePair<string, string>("oars", ""),
			new KeyValuePair<string, string>("sail", "sails_lvl3"),
			new KeyValuePair<string, string>("roof", ""),
			new KeyValuePair<string, string>("bow", ""),
			new KeyValuePair<string, string>("side", "side_northern_shields_lvl2")
		};

		// Token: 0x040003EA RID: 1002
		private readonly List<KeyValuePair<string, string>> _phase2AllyShip4UpgradePieceList = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("fore", ""),
			new KeyValuePair<string, string>("aft", ""),
			new KeyValuePair<string, string>("hull", ""),
			new KeyValuePair<string, string>("deck", "deck_large_arrow_and_javelin_crates_lvl3"),
			new KeyValuePair<string, string>("oars", ""),
			new KeyValuePair<string, string>("sail", "sails_lvl2"),
			new KeyValuePair<string, string>("roof", ""),
			new KeyValuePair<string, string>("bow", ""),
			new KeyValuePair<string, string>("side", "side_northern_shields_lvl2")
		};

		// Token: 0x040003EB RID: 1003
		private readonly List<KeyValuePair<string, string>> _phase2AllyShip5UpgradePieceList = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("fore", ""),
			new KeyValuePair<string, string>("aft", ""),
			new KeyValuePair<string, string>("hull", ""),
			new KeyValuePair<string, string>("deck", "deck_arrow_and_javelin_crates_lvl2"),
			new KeyValuePair<string, string>("oars", ""),
			new KeyValuePair<string, string>("sail", "sails_lvl2"),
			new KeyValuePair<string, string>("roof", ""),
			new KeyValuePair<string, string>("bow", ""),
			new KeyValuePair<string, string>("side", "side_northern_shields_lvl1")
		};

		// Token: 0x040003EC RID: 1004
		private readonly List<KeyValuePair<string, string>> _phase3EnemyShip1UpgradePieceList = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("fore", ""),
			new KeyValuePair<string, string>("aft", ""),
			new KeyValuePair<string, string>("hull", ""),
			new KeyValuePair<string, string>("deck", "deck_arrow_and_javelin_crates_lvl2"),
			new KeyValuePair<string, string>("oars", ""),
			new KeyValuePair<string, string>("sail", "sails_lvl3"),
			new KeyValuePair<string, string>("roof", ""),
			new KeyValuePair<string, string>("bow", ""),
			new KeyValuePair<string, string>("side", "side_southern_shields_lvl2")
		};

		// Token: 0x040003ED RID: 1005
		private readonly List<KeyValuePair<string, string>> _phase3EnemyShip2UpgradePieceList = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("fore", ""),
			new KeyValuePair<string, string>("aft", ""),
			new KeyValuePair<string, string>("hull", ""),
			new KeyValuePair<string, string>("deck", "deck_ammo_crates_lvl2"),
			new KeyValuePair<string, string>("oars", ""),
			new KeyValuePair<string, string>("sail", "sails_lvl2"),
			new KeyValuePair<string, string>("roof", ""),
			new KeyValuePair<string, string>("bow", ""),
			new KeyValuePair<string, string>("side", "side_southern_shields_lvl2")
		};

		// Token: 0x040003EE RID: 1006
		private readonly List<KeyValuePair<string, string>> _phase3EnemyShip3UpgradePieceList = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("fore", ""),
			new KeyValuePair<string, string>("aft", ""),
			new KeyValuePair<string, string>("hull", ""),
			new KeyValuePair<string, string>("deck", "deck_ammo_crates_lvl2"),
			new KeyValuePair<string, string>("oars", ""),
			new KeyValuePair<string, string>("sail", "sails_lvl2"),
			new KeyValuePair<string, string>("roof", ""),
			new KeyValuePair<string, string>("bow", ""),
			new KeyValuePair<string, string>("side", "")
		};

		// Token: 0x040003EF RID: 1007
		private readonly List<KeyValuePair<string, string>> _phase3EnemyShip4UpgradePieceList = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("fore", ""),
			new KeyValuePair<string, string>("aft", ""),
			new KeyValuePair<string, string>("hull", ""),
			new KeyValuePair<string, string>("deck", "deck_ammo_bins_lvl1"),
			new KeyValuePair<string, string>("oars", ""),
			new KeyValuePair<string, string>("sail", "sails_lvl3"),
			new KeyValuePair<string, string>("roof", ""),
			new KeyValuePair<string, string>("bow", ""),
			new KeyValuePair<string, string>("side", "side_northern_shields_lvl1")
		};

		// Token: 0x040003F0 RID: 1008
		private readonly List<KeyValuePair<string, string>> _phase3EnemyShip5UpgradePieceList = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("aft", ""),
			new KeyValuePair<string, string>("hull", ""),
			new KeyValuePair<string, string>("deck", "deck_ammo_bins_lvl1"),
			new KeyValuePair<string, string>("oars", ""),
			new KeyValuePair<string, string>("sail", "sails_lvl2"),
			new KeyValuePair<string, string>("roof", ""),
			new KeyValuePair<string, string>("bow", ""),
			new KeyValuePair<string, string>("side", "side_southern_shields_lvl1")
		};

		// Token: 0x040003F1 RID: 1009
		private readonly List<KeyValuePair<string, string>> _phase3EnemyReinforcementShip1UpgradePieceList = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("fore", ""),
			new KeyValuePair<string, string>("aft", ""),
			new KeyValuePair<string, string>("hull", ""),
			new KeyValuePair<string, string>("deck", "deck_boarding_weapons_lvl3"),
			new KeyValuePair<string, string>("oars", ""),
			new KeyValuePair<string, string>("sail", "sails_lvl2"),
			new KeyValuePair<string, string>("roof", ""),
			new KeyValuePair<string, string>("bow", ""),
			new KeyValuePair<string, string>("side", "side_southern_shields_lvl2")
		};

		// Token: 0x040003F2 RID: 1010
		private readonly List<KeyValuePair<string, string>> _phase3EnemyReinforcementShip2UpgradePieceList = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("fore", ""),
			new KeyValuePair<string, string>("aft", ""),
			new KeyValuePair<string, string>("hull", ""),
			new KeyValuePair<string, string>("deck", "deck_boarding_weapons_lvl3"),
			new KeyValuePair<string, string>("oars", ""),
			new KeyValuePair<string, string>("sail", "sails_lvl3"),
			new KeyValuePair<string, string>("roof", ""),
			new KeyValuePair<string, string>("bow", ""),
			new KeyValuePair<string, string>("side", "side_northern_shields_lvl1")
		};

		// Token: 0x040003F3 RID: 1011
		private readonly List<KeyValuePair<string, string>> _phase3EnemyReinforcementShip3UpgradePieceList = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("fore", ""),
			new KeyValuePair<string, string>("aft", ""),
			new KeyValuePair<string, string>("hull", ""),
			new KeyValuePair<string, string>("deck", "deck_boarding_weapons_lvl3"),
			new KeyValuePair<string, string>("oars", ""),
			new KeyValuePair<string, string>("sail", "sails_lvl2"),
			new KeyValuePair<string, string>("roof", ""),
			new KeyValuePair<string, string>("bow", ""),
			new KeyValuePair<string, string>("side", "side_northern_shields_lvl2")
		};

		// Token: 0x040003F4 RID: 1012
		private readonly List<KeyValuePair<string, string>> _phase4PurigsShipUpgradePieceList = new List<KeyValuePair<string, string>>
		{
			new KeyValuePair<string, string>("fore", "fore_battlement_lvl3_wbarracks"),
			new KeyValuePair<string, string>("aft", "aft_battlement_lvl3_wbarracks"),
			new KeyValuePair<string, string>("hull", ""),
			new KeyValuePair<string, string>("deck", ""),
			new KeyValuePair<string, string>("oars", ""),
			new KeyValuePair<string, string>("sail", "sails_lvl3"),
			new KeyValuePair<string, string>("roof", ""),
			new KeyValuePair<string, string>("bow", ""),
			new KeyValuePair<string, string>("side", "")
		};

		// Token: 0x040003F5 RID: 1013
		private Quest5SetPieceBattleMissionController.Quest5InstructionState _instructionState;

		// Token: 0x040003F6 RID: 1014
		private Quest5ApproachObjective _approachObjective;

		// Token: 0x040003F7 RID: 1015
		private Quest5JumpObjective _jumpObjective;

		// Token: 0x040003F8 RID: 1016
		private Quest5SwimObjective _swimObjective;

		// Token: 0x040003F9 RID: 1017
		private Quest5ClearGuardsObjective _clearGuardsObjective;

		// Token: 0x040003FA RID: 1018
		private Quest5CheckInteriorObjective _checkInteriorObjective;

		// Token: 0x040003FB RID: 1019
		private Quest5TalkWithYourSisterObjective _talkWithYourSisterObjective;

		// Token: 0x040003FC RID: 1020
		private Quest5ReturnToDeckObjective _returnToDeckObjective;

		// Token: 0x040003FD RID: 1021
		private Quest5CutLooseObjective _cutLooseObjective;

		// Token: 0x040003FE RID: 1022
		private Quest5GunnarUsesShipObjective _gunnarUsesShipObjective;

		// Token: 0x040003FF RID: 1023
		private Quest5EscapeObjective _escapeObjective;

		// Token: 0x04000400 RID: 1024
		private Quest5ReachAlliesObjective _reachAlliesObjective;

		// Token: 0x04000401 RID: 1025
		private Quest5DefeatEnemiesObjective _defeatEnemiesObjective;

		// Token: 0x04000402 RID: 1026
		private Quest5DefeatPurigsShipObjective _defeatPurigsShipObjective;

		// Token: 0x04000403 RID: 1027
		private Quest5DefeatPurigObjective _defeatPurigObjective;

		// Token: 0x04000404 RID: 1028
		private Quest5SetPieceBattleMissionController.GunnarMovementState _gunnarMovementState;

		// Token: 0x04000405 RID: 1029
		private Quest5SetPieceBattleMissionController.GunnarMovementStateForClimbingShip _gunnarMovementStateForClimbingShip;

		// Token: 0x04000406 RID: 1030
		private ClimbingMachine _targetClimbingMachine;

		// Token: 0x04000407 RID: 1031
		private MissionTimer _gunnarFellIntoTheWaterTimer;

		// Token: 0x04000408 RID: 1032
		private GameEntity _jumpOffInitialPositionGameEntity;

		// Token: 0x04000409 RID: 1033
		private GameEntity _jumpOffTargetPositionGameEntity;

		// Token: 0x0400040A RID: 1034
		private GameEntity _hidingSpot1PositionGameEntity;

		// Token: 0x0400040B RID: 1035
		private MissionShip _phase1EnemyShip1;

		// Token: 0x0400040C RID: 1036
		private MissionShip _phase1EnemyShip2;

		// Token: 0x0400040D RID: 1037
		private MissionShip _phase1EnemyShip3;

		// Token: 0x0400040E RID: 1038
		private MissionShip _phase1EnemyShip4;

		// Token: 0x0400040F RID: 1039
		private Figurehead EscapeShipFigurehead = DefaultFigureheads.Lion;

		// Token: 0x04000410 RID: 1040
		private bool _talkedWithSister;

		// Token: 0x04000411 RID: 1041
		private bool _crusasAndSeaHoundMovedToTheConversationPoints;

		// Token: 0x04000412 RID: 1042
		private List<GameEntity> _dynamicPatrolAreas = new List<GameEntity>();

		// Token: 0x04000413 RID: 1043
		private List<Agent> _stealthAgents = new List<Agent>();

		// Token: 0x04000414 RID: 1044
		private WeakGameEntity _crusasConversationPointFrame;

		// Token: 0x04000415 RID: 1045
		private WeakGameEntity _slaveTraderConversationPointFrame;

		// Token: 0x04000416 RID: 1046
		private GameEntity _approachPointEntity;

		// Token: 0x04000417 RID: 1047
		private GameEntity _phase1EnemyShipToInteriorShipDoorEntity;

		// Token: 0x04000418 RID: 1048
		private GameEntity _phase1InteriorToEnemyShip3ShipDoorEntity;

		// Token: 0x04000419 RID: 1049
		private GameEntity _phase1EnemyShip1InitialSpawnEntity;

		// Token: 0x0400041A RID: 1050
		private GameEntity _phase1EnemyShip1TargetEntity;

		// Token: 0x0400041B RID: 1051
		private Queue<Quest5SetPieceBattleMissionController.ConversationSound> _conversationSounds = new Queue<Quest5SetPieceBattleMissionController.ConversationSound>();

		// Token: 0x0400041C RID: 1052
		private List<MBInformationManager.DialogNotificationHandle> _dialogNotificationHandleCache = new List<MBInformationManager.DialogNotificationHandle>();

		// Token: 0x0400041D RID: 1053
		private float _lastCachedPlayerShipDistanceToTargetApproachPoint;

		// Token: 0x0400041E RID: 1054
		private MissionTimer _playerShipsTargetApproachPointDistanceCheckTimer;

		// Token: 0x0400041F RID: 1055
		private MissionTimer _escapeShipCutLooseTimer;

		// Token: 0x04000420 RID: 1056
		private MissionTimer _allowedSwimRadiusCheckTimer;

		// Token: 0x04000421 RID: 1057
		private ActionIndexCache _sisterWoundedAnimationActionIndexCache;

		// Token: 0x04000422 RID: 1058
		private ActionIndexCache _slaveTraderShipOarsmanActionIndexCache;

		// Token: 0x04000423 RID: 1059
		private Vec3 _phase1PlayerShipSpawnPosition = Vec3.Invalid;

		// Token: 0x04000424 RID: 1060
		private Equipment _mainAgentEquipmentCopyForInteriorMission;

		// Token: 0x04000426 RID: 1062
		private MissionShip _phase2EnemyShip1;

		// Token: 0x04000427 RID: 1063
		private MissionShip _phase2EnemyShip2;

		// Token: 0x04000428 RID: 1064
		private MissionShip _phase2EnemyShip3;

		// Token: 0x04000429 RID: 1065
		private MissionShip _phase2EnemyShip4;

		// Token: 0x0400042A RID: 1066
		private MissionShip _phase2EnemyShip5;

		// Token: 0x0400042B RID: 1067
		private MissionShip _phase2EnemyShipStationary1;

		// Token: 0x0400042C RID: 1068
		private GameEntity _phase2EscapeShipPirateTargetFrame1;

		// Token: 0x0400042D RID: 1069
		private GameEntity _phase2EscapeShipPirateTargetFrame2;

		// Token: 0x0400042E RID: 1070
		private GameEntity _phase2EscapeShipPirateTargetFrame3;

		// Token: 0x0400042F RID: 1071
		private GameEntity _phase2EscapeShipPirateTargetFrame4;

		// Token: 0x04000430 RID: 1072
		private GameEntity _phase2EscapeShipPirateTargetFrame5;

		// Token: 0x04000431 RID: 1073
		private GameEntity _currentPhase2EscapeShipTargetPoint;

		// Token: 0x04000432 RID: 1074
		private MissionShip _phase2AllyShip1;

		// Token: 0x04000433 RID: 1075
		private MissionShip _phase2AllyShip2;

		// Token: 0x04000434 RID: 1076
		private MissionShip _phase2AllyShip3;

		// Token: 0x04000435 RID: 1077
		private MissionShip _phase2AllyShip4;

		// Token: 0x04000436 RID: 1078
		private MissionShip _phase2AllyShip5;

		// Token: 0x04000437 RID: 1079
		private Dictionary<MissionShip, GameEntity> _pirateShipTriggerPoints = new Dictionary<MissionShip, GameEntity>();

		// Token: 0x04000438 RID: 1080
		private Dictionary<MissionShip, bool> _isPirateShipMovementDisabled = new Dictionary<MissionShip, bool>();

		// Token: 0x04000439 RID: 1081
		private Dictionary<MissionShip, ShipAttachmentMachine> _pirateShipEnabledAttachmentMachine = new Dictionary<MissionShip, ShipAttachmentMachine>();

		// Token: 0x0400043A RID: 1082
		private Dictionary<MissionShip, bool> _isPirateShipTriggered = new Dictionary<MissionShip, bool>();

		// Token: 0x0400043B RID: 1083
		private Dictionary<MissionShip, bool> _isPirateShipMovingToTheEscapeShip = new Dictionary<MissionShip, bool>();

		// Token: 0x0400043C RID: 1084
		private Dictionary<MissionShip, bool> _isPirateShipLostItsCrew = new Dictionary<MissionShip, bool>();

		// Token: 0x0400043D RID: 1085
		private Dictionary<MissionShip, bool> _limitPirateShipChasingSpeed = new Dictionary<MissionShip, bool>();

		// Token: 0x0400043E RID: 1086
		private Dictionary<MissionShip, MissionTimer> _autoCutLooseTimersForPirateShips = new Dictionary<MissionShip, MissionTimer>();

		// Token: 0x0400043F RID: 1087
		private Dictionary<MissionShip, MissionTimer> _autoEstablishConnectionsForPirateShips = new Dictionary<MissionShip, MissionTimer>();

		// Token: 0x04000440 RID: 1088
		private Dictionary<MissionShip, bool> _isMissionShipBoardedToTheEscapeShip = new Dictionary<MissionShip, bool>();

		// Token: 0x04000441 RID: 1089
		private List<GameEntity> _phase2EscapeShipTargetPointEntities = new List<GameEntity>(32);

		// Token: 0x04000442 RID: 1090
		private Queue<GameEntity> _phase2EscapeShipTargetPoints = new Queue<GameEntity>();

		// Token: 0x04000443 RID: 1091
		private MissionTimer _playerLeftTheEscapeShipTimer;

		// Token: 0x04000444 RID: 1092
		private MissionTimer _phase2EscapeShipStuckTimer;

		// Token: 0x04000445 RID: 1093
		private Vec3 _phase2EscapeShipStuckCheckPosition = Vec3.Invalid;

		// Token: 0x04000446 RID: 1094
		private float _escapeShipTargetSpeed;

		// Token: 0x04000447 RID: 1095
		private float _escapeShipSpeed;

		// Token: 0x04000448 RID: 1096
		private Vec2 _escapeShipTargetDirection;

		// Token: 0x04000449 RID: 1097
		private Vec2 _escapeShipDirection;

		// Token: 0x0400044B RID: 1099
		private readonly List<KeyValuePair<string, int>> _phase2AllyShip1Troops = new List<KeyValuePair<string, int>>
		{
			new KeyValuePair<string, int>("aserai_marine_t5", 54),
			new KeyValuePair<string, int>("southern_pirates_chief", 18)
		};

		// Token: 0x0400044C RID: 1100
		private readonly List<KeyValuePair<string, int>> _phase2AllyShip2Troops = new List<KeyValuePair<string, int>>
		{
			new KeyValuePair<string, int>("skolderbrotva_tier_2", 5),
			new KeyValuePair<string, int>("skolderbrotva_tier_3", 34)
		};

		// Token: 0x0400044D RID: 1101
		private readonly List<KeyValuePair<string, int>> _phase2AllyShip3Troops = new List<KeyValuePair<string, int>>
		{
			new KeyValuePair<string, int>("gangradirs_kin_ranged", 18),
			new KeyValuePair<string, int>("gangradirs_kin_melee", 19)
		};

		// Token: 0x0400044E RID: 1102
		private readonly List<KeyValuePair<string, int>> _phase2AllyShip4Troops = new List<KeyValuePair<string, int>>
		{
			new KeyValuePair<string, int>("skolderbrotva_tier_2", 32),
			new KeyValuePair<string, int>("skolderbrotva_tier_3", 34)
		};

		// Token: 0x0400044F RID: 1103
		private readonly List<KeyValuePair<string, int>> _phase2AllyShip5Troops = new List<KeyValuePair<string, int>>
		{
			new KeyValuePair<string, int>("skolderbrotva_tier_3", 18),
			new KeyValuePair<string, int>("skolderbrotva_tier_2", 17)
		};

		// Token: 0x04000450 RID: 1104
		private readonly List<KeyValuePair<string, int>> _phase2EnemyShip1Troops = new List<KeyValuePair<string, int>>
		{
			new KeyValuePair<string, int>("sea_hound_captivity", 4),
			new KeyValuePair<string, int>("sea_hound_captivity", 1)
		};

		// Token: 0x04000451 RID: 1105
		private readonly List<KeyValuePair<string, int>> _phase2EnemyShip2Troops = new List<KeyValuePair<string, int>>
		{
			new KeyValuePair<string, int>("sea_hound_captivity", 3),
			new KeyValuePair<string, int>("sea_hound_captivity", 2)
		};

		// Token: 0x04000452 RID: 1106
		private readonly List<KeyValuePair<string, int>> _phase2EnemyShip3Troops = new List<KeyValuePair<string, int>>
		{
			new KeyValuePair<string, int>("sea_hound_captivity", 3),
			new KeyValuePair<string, int>("sea_hound_captivity", 2)
		};

		// Token: 0x04000453 RID: 1107
		private readonly List<KeyValuePair<string, int>> _phase2EnemyShip4Troops = new List<KeyValuePair<string, int>>
		{
			new KeyValuePair<string, int>("sea_hound_captivity", 3),
			new KeyValuePair<string, int>("sea_hound_captivity", 2)
		};

		// Token: 0x04000454 RID: 1108
		private readonly List<KeyValuePair<string, int>> _phase2EnemyShip5Troops = new List<KeyValuePair<string, int>>
		{
			new KeyValuePair<string, int>("sea_hound_captivity", 3),
			new KeyValuePair<string, int>("sea_hound_captivity", 2)
		};

		// Token: 0x04000455 RID: 1109
		private readonly List<KeyValuePair<string, int>> _phase2EnemyShipStationary1Troops = new List<KeyValuePair<string, int>>
		{
			new KeyValuePair<string, int>("sea_hounds_marksman", 8)
		};

		// Token: 0x04000456 RID: 1110
		private MissionShip _phase3EnemyShip1;

		// Token: 0x04000457 RID: 1111
		private MissionShip _phase3EnemyShip2;

		// Token: 0x04000458 RID: 1112
		private MissionShip _phase3EnemyShip3;

		// Token: 0x04000459 RID: 1113
		private MissionShip _phase3EnemyShip4;

		// Token: 0x0400045A RID: 1114
		private MissionShip _phase3EnemyShip5;

		// Token: 0x0400045B RID: 1115
		private MissionShip _phase3EnemyReinforcementShip1;

		// Token: 0x0400045C RID: 1116
		private MissionShip _phase3EnemyReinforcementShip2;

		// Token: 0x0400045D RID: 1117
		private VolumeBox _phase3TriggerVolumeBox;

		// Token: 0x0400045E RID: 1118
		private readonly List<MissionShip> _allyShipTargetKeysBuffer = new List<MissionShip>(16);

		// Token: 0x0400045F RID: 1119
		private readonly HashSet<MissionShip> _assignedEnemyShips = new HashSet<MissionShip>();

		// Token: 0x04000460 RID: 1120
		private bool _isReinforcementCalled;

		// Token: 0x04000461 RID: 1121
		private bool _isReinforcementInitialized;

		// Token: 0x04000462 RID: 1122
		private readonly List<KeyValuePair<string, int>> _phase3PlayerShipTroops = new List<KeyValuePair<string, int>>
		{
			new KeyValuePair<string, int>("gangradirs_kin_melee", 40),
			new KeyValuePair<string, int>("gangradirs_kin_melee", 40)
		};

		// Token: 0x04000463 RID: 1123
		private readonly List<KeyValuePair<string, int>> _phase3EnemyShip1Troops = new List<KeyValuePair<string, int>>
		{
			new KeyValuePair<string, int>("sea_hounds", 52),
			new KeyValuePair<string, int>("sea_hounds_marksman", 10)
		};

		// Token: 0x04000464 RID: 1124
		private readonly List<KeyValuePair<string, int>> _phase3EnemyShip2Troops = new List<KeyValuePair<string, int>>
		{
			new KeyValuePair<string, int>("sea_hounds_pups", 64),
			new KeyValuePair<string, int>("sea_hounds_marksman", 10)
		};

		// Token: 0x04000465 RID: 1125
		private readonly List<KeyValuePair<string, int>> _phase3EnemyShip3Troops = new List<KeyValuePair<string, int>>
		{
			new KeyValuePair<string, int>("sea_hounds_pups", 25),
			new KeyValuePair<string, int>("sea_hounds", 44)
		};

		// Token: 0x04000466 RID: 1126
		private readonly List<KeyValuePair<string, int>> _phase3EnemyShip4Troops = new List<KeyValuePair<string, int>>
		{
			new KeyValuePair<string, int>("sea_hounds_pups", 15),
			new KeyValuePair<string, int>("sea_hounds", 50)
		};

		// Token: 0x04000467 RID: 1127
		private readonly List<KeyValuePair<string, int>> _phase3EnemyShip5Troops = new List<KeyValuePair<string, int>>
		{
			new KeyValuePair<string, int>("sea_hounds_marksman", 16),
			new KeyValuePair<string, int>("sea_hounds", 50)
		};

		// Token: 0x04000468 RID: 1128
		private readonly List<KeyValuePair<string, int>> _phase3EnemyReinforcementShip1Troops = new List<KeyValuePair<string, int>>
		{
			new KeyValuePair<string, int>("sea_hounds_marksman", 15),
			new KeyValuePair<string, int>("sea_hound_captivity", 30)
		};

		// Token: 0x04000469 RID: 1129
		private readonly List<KeyValuePair<string, int>> _phase3EnemyReinforcementShip2Troops = new List<KeyValuePair<string, int>>
		{
			new KeyValuePair<string, int>("sea_hounds_marksman", 15),
			new KeyValuePair<string, int>("sea_hounds", 30)
		};

		// Token: 0x0400046A RID: 1130
		private int _phase3TotalEnemyCount;

		// Token: 0x0400046B RID: 1131
		private Quest5SetPieceBattleMissionController.BossFightStateEnum BossFightState;

		// Token: 0x0400046D RID: 1133
		private List<Agent> _purigShipAgents = new List<Agent>();

		// Token: 0x0400046E RID: 1134
		private List<Agent> _duelPhaseAllyAgents;

		// Token: 0x0400046F RID: 1135
		private List<Agent> _duelPhaseEnemyAgents;

		// Token: 0x04000470 RID: 1136
		private Queue<Quest5SetPieceBattleMissionController.ConversationSound> _purigNotifications = new Queue<Quest5SetPieceBattleMissionController.ConversationSound>();

		// Token: 0x04000471 RID: 1137
		private Agent _purigBodyguard1;

		// Token: 0x04000472 RID: 1138
		private Agent _purigBodyguard2;

		// Token: 0x04000473 RID: 1139
		private bool _isPurigCutsceneStarted;

		// Token: 0x04000474 RID: 1140
		private bool _isPlayerUsingShipAtTheStartOfThePurigCutscene;

		// Token: 0x04000475 RID: 1141
		private StandingPoint _playerStandingPointAtTheStartOfThePurigCutscene;

		// Token: 0x04000476 RID: 1142
		private VolumeBox _phase4TriggerVolumeBox;

		// Token: 0x04000477 RID: 1143
		private GameEntity _playerSpawnPointEntity;

		// Token: 0x04000478 RID: 1144
		private GameEntity _enemyBossSpawnPointEntity;

		// Token: 0x04000479 RID: 1145
		private BattleSideEnum _winnerSide = -1;

		// Token: 0x0400047C RID: 1148
		private NavalAgentsLogic _navalAgentsLogic;

		// Token: 0x0400047D RID: 1149
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x0400047E RID: 1150
		private NavalTrajectoryPlanningLogic _navalTrajectoryPlanningLogic;

		// Token: 0x0400047F RID: 1151
		private MissionObjectiveLogic _missionObjectiveLogic;

		// Token: 0x04000480 RID: 1152
		private LightScriptedFiresMissionController _lightScriptedFiresMissionController;

		// Token: 0x04000481 RID: 1153
		private List<Formation> _availableAllyFormations = new List<Formation>();

		// Token: 0x04000482 RID: 1154
		private List<Formation> _availableEnemyFormations = new List<Formation>();

		// Token: 0x04000483 RID: 1155
		private MissionTimer _endMissionTimer;

		// Token: 0x04000484 RID: 1156
		private Formation _playerFormation;

		// Token: 0x04000485 RID: 1157
		private MissionShip _playerShip;

		// Token: 0x04000486 RID: 1158
		private readonly MobileParty _enemyParty;

		// Token: 0x04000487 RID: 1159
		private Agent _laharAgent;

		// Token: 0x04000488 RID: 1160
		private Agent _bjolgurAgent;

		// Token: 0x04000489 RID: 1161
		private Agent _crusasAgent;

		// Token: 0x0400048A RID: 1162
		private Agent _gunnarAgent;

		// Token: 0x0400048B RID: 1163
		private Agent _purigAgent;

		// Token: 0x0400048C RID: 1164
		private Agent _slaveTraderAgent;

		// Token: 0x0400048D RID: 1165
		private CharacterObject _slaveTraderCharacter;

		// Token: 0x0400048E RID: 1166
		private Agent[] _slaveTraderShipOarsmen = new Agent[6];

		// Token: 0x0400048F RID: 1167
		private AgentNavalComponent _gunnarAgentNavalComponent;

		// Token: 0x04000490 RID: 1168
		private bool _isCheckpointInitialize;

		// Token: 0x04000491 RID: 1169
		private bool _isMissionFailPopUpTriggered;

		// Token: 0x020001DC RID: 476
		public class ConversationSound
		{
			// Token: 0x06001A65 RID: 6757 RVA: 0x000AF3F9 File Offset: 0x000AD5F9
			public ConversationSound(TextObject line, MBInformationManager.NotificationPriority priority, CharacterObject character)
			{
				this.Line = line;
				this.Priority = priority;
				this.Character = character;
			}

			// Token: 0x04000D79 RID: 3449
			public TextObject Line;

			// Token: 0x04000D7A RID: 3450
			public MBInformationManager.NotificationPriority Priority;

			// Token: 0x04000D7B RID: 3451
			public CharacterObject Character;
		}

		// Token: 0x020001DD RID: 477
		public enum Quest5SetPieceBattleMissionState
		{
			// Token: 0x04000D7D RID: 3453
			None,
			// Token: 0x04000D7E RID: 3454
			InitializePhase1Part1,
			// Token: 0x04000D7F RID: 3455
			InitializePhase1Part2,
			// Token: 0x04000D80 RID: 3456
			Phase1GoToEnemyShip,
			// Token: 0x04000D81 RID: 3457
			Phase1SwimmingPhase,
			// Token: 0x04000D82 RID: 3458
			InitializeStealthPhasePart1,
			// Token: 0x04000D83 RID: 3459
			InitializeStealthPhasePart2,
			// Token: 0x04000D84 RID: 3460
			Phase1StealthPhase,
			// Token: 0x04000D85 RID: 3461
			Phase1GoToShipInteriorFadeOut,
			// Token: 0x04000D86 RID: 3462
			Phase1InitializeShipInteriorPhase,
			// Token: 0x04000D87 RID: 3463
			Phase1GoToShipInteriorFadeIn,
			// Token: 0x04000D88 RID: 3464
			Phase1ShipInteriorPhase,
			// Token: 0x04000D89 RID: 3465
			Phase1GoBackToShipFadeOut,
			// Token: 0x04000D8A RID: 3466
			Phase1InitializeGoBackToShip,
			// Token: 0x04000D8B RID: 3467
			Phase1GoBackToShipFadeIn,
			// Token: 0x04000D8C RID: 3468
			Phase1EscapePhase,
			// Token: 0x04000D8D RID: 3469
			Phase1ToPhase2FadeOut,
			// Token: 0x04000D8E RID: 3470
			InitializePhase2Part1,
			// Token: 0x04000D8F RID: 3471
			InitializePhase2Part2,
			// Token: 0x04000D90 RID: 3472
			InitializePhase2Part3,
			// Token: 0x04000D91 RID: 3473
			InitializePhase2Part4,
			// Token: 0x04000D92 RID: 3474
			Phase1ToPhase2FadeIn,
			// Token: 0x04000D93 RID: 3475
			Phase2InProgress,
			// Token: 0x04000D94 RID: 3476
			Phase2ToPhase3FadeOut,
			// Token: 0x04000D95 RID: 3477
			InitializePhase3Part1,
			// Token: 0x04000D96 RID: 3478
			InitializePhase3Part2,
			// Token: 0x04000D97 RID: 3479
			InitializePhase3Part3,
			// Token: 0x04000D98 RID: 3480
			Phase2ToPhase3FadeIn,
			// Token: 0x04000D99 RID: 3481
			Phase3InProgress,
			// Token: 0x04000D9A RID: 3482
			Phase3ToPhase4FadeOut,
			// Token: 0x04000D9B RID: 3483
			InitializePhase4Part1,
			// Token: 0x04000D9C RID: 3484
			InitializePhase4Part2,
			// Token: 0x04000D9D RID: 3485
			Phase3ToPhase4FadeIn,
			// Token: 0x04000D9E RID: 3486
			Phase4InProgress,
			// Token: 0x04000D9F RID: 3487
			Phase4ToBossFightFadeOut,
			// Token: 0x04000DA0 RID: 3488
			InitializeBossFightPart1,
			// Token: 0x04000DA1 RID: 3489
			InitializeBossFightPart2,
			// Token: 0x04000DA2 RID: 3490
			Phase4ToBossFightFadeIn,
			// Token: 0x04000DA3 RID: 3491
			StartBossFightConversation,
			// Token: 0x04000DA4 RID: 3492
			BossFightConversationInProgress,
			// Token: 0x04000DA5 RID: 3493
			BossFightInProgressAsDuel,
			// Token: 0x04000DA6 RID: 3494
			BossFightInProgressAsAll,
			// Token: 0x04000DA7 RID: 3495
			End,
			// Token: 0x04000DA8 RID: 3496
			Exit
		}

		// Token: 0x020001DE RID: 478
		private enum Quest5InstructionState
		{
			// Token: 0x04000DAA RID: 3498
			None,
			// Token: 0x04000DAB RID: 3499
			Approach,
			// Token: 0x04000DAC RID: 3500
			WaitForJump,
			// Token: 0x04000DAD RID: 3501
			Jump,
			// Token: 0x04000DAE RID: 3502
			WaitForSwim,
			// Token: 0x04000DAF RID: 3503
			Swim,
			// Token: 0x04000DB0 RID: 3504
			WaitForClearGuards,
			// Token: 0x04000DB1 RID: 3505
			ClearGuards,
			// Token: 0x04000DB2 RID: 3506
			WaitForCheckInterior,
			// Token: 0x04000DB3 RID: 3507
			CheckInterior,
			// Token: 0x04000DB4 RID: 3508
			WaitForTalkSister,
			// Token: 0x04000DB5 RID: 3509
			TalkSister,
			// Token: 0x04000DB6 RID: 3510
			WaitForReturnToDeck,
			// Token: 0x04000DB7 RID: 3511
			ReturnToDeck,
			// Token: 0x04000DB8 RID: 3512
			WaitForCutLoose,
			// Token: 0x04000DB9 RID: 3513
			CutLoose,
			// Token: 0x04000DBA RID: 3514
			WaitForGunnarUsesShip,
			// Token: 0x04000DBB RID: 3515
			GunnarUsesShip,
			// Token: 0x04000DBC RID: 3516
			WaitForEscapeQuietly,
			// Token: 0x04000DBD RID: 3517
			EscapeQuietly,
			// Token: 0x04000DBE RID: 3518
			WaitForReachAllies,
			// Token: 0x04000DBF RID: 3519
			ReachAllies,
			// Token: 0x04000DC0 RID: 3520
			WaitForDefeatEnemies,
			// Token: 0x04000DC1 RID: 3521
			DefeatEnemies,
			// Token: 0x04000DC2 RID: 3522
			WaitForDefeatPurigsShip,
			// Token: 0x04000DC3 RID: 3523
			DefeatPurigsShip,
			// Token: 0x04000DC4 RID: 3524
			WaitForDefeatPurig,
			// Token: 0x04000DC5 RID: 3525
			DefeatPurig,
			// Token: 0x04000DC6 RID: 3526
			WaitForEnd,
			// Token: 0x04000DC7 RID: 3527
			End
		}

		// Token: 0x020001DF RID: 479
		private enum GunnarMovementState
		{
			// Token: 0x04000DC9 RID: 3529
			None,
			// Token: 0x04000DCA RID: 3530
			GoToInitialJumpingPosition,
			// Token: 0x04000DCB RID: 3531
			WaitForReachingInitialJumpingPosition,
			// Token: 0x04000DCC RID: 3532
			GoToJumpingTargetPosition,
			// Token: 0x04000DCD RID: 3533
			WaitForReachingJumpingTargetPosition,
			// Token: 0x04000DCE RID: 3534
			SwimToTheHidingSpot,
			// Token: 0x04000DCF RID: 3535
			WaitForTeleportingToTheHidingSpot,
			// Token: 0x04000DD0 RID: 3536
			TeleportToTargetPosition,
			// Token: 0x04000DD1 RID: 3537
			WaitAtTheHidingSpot,
			// Token: 0x04000DD2 RID: 3538
			GoToTheEscapeShip,
			// Token: 0x04000DD3 RID: 3539
			WaitForReachingToTheEscapeShip,
			// Token: 0x04000DD4 RID: 3540
			UseTheEscapeShip,
			// Token: 0x04000DD5 RID: 3541
			End
		}

		// Token: 0x020001E0 RID: 480
		private enum GunnarMovementStateForClimbingShip
		{
			// Token: 0x04000DD7 RID: 3543
			None,
			// Token: 0x04000DD8 RID: 3544
			Start,
			// Token: 0x04000DD9 RID: 3545
			GoingToTheTargetClimbingMachine,
			// Token: 0x04000DDA RID: 3546
			TargetReached,
			// Token: 0x04000DDB RID: 3547
			UsingClimbingMachine,
			// Token: 0x04000DDC RID: 3548
			OnDeck,
			// Token: 0x04000DDD RID: 3549
			GoToFinalTargetPoint,
			// Token: 0x04000DDE RID: 3550
			End
		}

		// Token: 0x020001E1 RID: 481
		public enum BossFightOutComeEnum
		{
			// Token: 0x04000DE0 RID: 3552
			None,
			// Token: 0x04000DE1 RID: 3553
			PlayerRefusedTheDuel,
			// Token: 0x04000DE2 RID: 3554
			PlayerAcceptedAndWonTheDuel,
			// Token: 0x04000DE3 RID: 3555
			PlayerDefeatedWaitingForConversation,
			// Token: 0x04000DE4 RID: 3556
			PlayerAcceptedTheDuelLostItAndLetPurigGo,
			// Token: 0x04000DE5 RID: 3557
			PlayerAcceptedTheDuelLostItAndHadPurigKilledAnyway
		}

		// Token: 0x020001E2 RID: 482
		private enum BossFightStateEnum
		{
			// Token: 0x04000DE7 RID: 3559
			None,
			// Token: 0x04000DE8 RID: 3560
			Duel,
			// Token: 0x04000DE9 RID: 3561
			All
		}
	}
}
