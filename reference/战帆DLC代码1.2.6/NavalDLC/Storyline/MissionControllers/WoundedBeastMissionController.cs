using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.Objects.UsableMachines;
using NavalDLC.Missions.ShipActuators;
using NavalDLC.Missions.ShipInput;
using NavalDLC.Storyline.Objectives.WoundedBeast;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.MissionLogics;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.Storyline.MissionControllers
{
	// Token: 0x02000071 RID: 113
	public class WoundedBeastMissionController : MissionLogic
	{
		// Token: 0x06000796 RID: 1942 RVA: 0x0003577C File Offset: 0x0003397C
		public WoundedBeastMissionController()
		{
			this._gunnarShipTroops.Add(new WoundedBeastMissionController.StorylineTroop("gangradirs_kin_melee", 15));
			this._gunnarShipTroops.Add(new WoundedBeastMissionController.StorylineTroop("gangradirs_kin_ranged", 18));
			this._laharShipTroops.Add(new WoundedBeastMissionController.StorylineTroop("southern_pirates_raider", 25));
			this._laharShipTroops.Add(new WoundedBeastMissionController.StorylineTroop("aserai_marine_t5", 18));
			this._fahdaShipTroops.Add(new WoundedBeastMissionController.StorylineTroop("southern_pirates_raider", 2));
			this._fahdaShipTroops.Add(new WoundedBeastMissionController.StorylineTroop("aserai_footman", 66));
			this._fahdaShipTroops.Add(new WoundedBeastMissionController.StorylineTroop("southern_pirates_bandit", 0));
			this._enemyReinforcementThirdShipTroops.Add(new WoundedBeastMissionController.StorylineTroop("southern_pirates_raider", 10));
			this._enemyReinforcementThirdShipTroops.Add(new WoundedBeastMissionController.StorylineTroop("aserai_footman", 13));
			this._enemyReinforcementThirdShipTroops.Add(new WoundedBeastMissionController.StorylineTroop("southern_pirates_bandit", 0));
			this._enemyReinforcementSecondShipTroops.Add(new WoundedBeastMissionController.StorylineTroop("southern_pirates_raider", 12));
			this._enemyReinforcementSecondShipTroops.Add(new WoundedBeastMissionController.StorylineTroop("aserai_footman", 7));
			this._enemyReinforcementSecondShipTroops.Add(new WoundedBeastMissionController.StorylineTroop("southern_pirates_bandit", 0));
			this._enemyReinforcementFirstShipTroops.Add(new WoundedBeastMissionController.StorylineTroop("southern_pirates_raider", 12));
			this._enemyReinforcementFirstShipTroops.Add(new WoundedBeastMissionController.StorylineTroop("aserai_footman", 8));
			this._enemyReinforcementFirstShipTroops.Add(new WoundedBeastMissionController.StorylineTroop("southern_pirates_bandit", 0));
		}

		// Token: 0x06000797 RID: 1943 RVA: 0x000359A4 File Offset: 0x00033BA4
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._navalShipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
			this._navalAgentsLogic = base.Mission.GetMissionBehavior<NavalAgentsLogic>();
			this._missionObjectiveLogic = base.Mission.GetMissionBehavior<MissionObjectiveLogic>();
			this._navalShipsLogic.SetDeploymentMode(true);
			this._navalShipsLogic.SetTeamShipDeploymentLimit(0, NavalShipDeploymentLimit.Max());
			this._navalShipsLogic.SetTeamShipDeploymentLimit(2, NavalShipDeploymentLimit.Max());
			this._navalShipsLogic.SetDeploymentMode(false);
			if (!SailWindProfile.IsSailWindProfileInitialized)
			{
				SailWindProfile.InitializeProfile();
			}
			this._navalShipsLogic.ShipRammingEvent += this.OnShipRammed;
		}

		// Token: 0x06000798 RID: 1944 RVA: 0x00035A47 File Offset: 0x00033C47
		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
			this._navalShipsLogic.ShipRammingEvent -= this.OnShipRammed;
		}

		// Token: 0x06000799 RID: 1945 RVA: 0x00035A66 File Offset: 0x00033C66
		public override void OnMissionStateFinalized()
		{
			SailWindProfile.FinalizeProfile();
		}

		// Token: 0x0600079A RID: 1946 RVA: 0x00035A70 File Offset: 0x00033C70
		public override void OnMissionTick(float dt)
		{
			if (!this._initialized)
			{
				this.Initialize();
			}
			if ((Agent.Main == null || !Agent.Main.IsActive()) && this._failingQuestTimer == null && this._inPhase1)
			{
				MBInformationManager.AddQuickInformation(new TextObject("{=ay5y18aq}You pass out from the pain of your wounds.", null), 0, null, null, "");
				this.OnFailed();
				this._failingQuestTimer = new MissionTimer(5f);
			}
			if (this._failingQuestTimer != null)
			{
				if (this._failingQuestTimer.Check(false))
				{
					base.Mission.EndMission();
					return;
				}
			}
			else
			{
				if (!this._fahdaMissionShip.IsSinking && this._fahdaMissionShip.GameEntity.GlobalPosition.AsVec2.Distance(this._fleePoint) < 100f)
				{
					CampaignInformationManager.AddDialogLine(new TextObject("{=9Y1iHrQ4}Ach. We couldn't catch Fahda in time.", null), NavalStorylineData.Lahar.CharacterObject, null, 0, 2);
					this.OnFailed();
					this._failingQuestTimer = new MissionTimer(5f);
					return;
				}
				if (this._inPhase1)
				{
					this.OnPhase1Tick(dt);
				}
				if (this.IsShipActive(this._fahdaMissionShip) && !this._fahdaMissionShip.GetIsConnected())
				{
					this._fahdaMissionShip.ShipOrder.SetShipMovementOrder(in this._fleePoint);
				}
				this.TickGunnarsShip();
				this.CheckTargetShipNearEscapePoint();
				this.CheckDrowningAgents(dt);
				this.CheckMissionEnd();
			}
		}

		// Token: 0x0600079B RID: 1947 RVA: 0x00035BCC File Offset: 0x00033DCC
		private void CheckMissionEnd()
		{
			if (!this._isMissionFailed && !this._isMissionSuccessful)
			{
				if (this.GetAgentCountOfSide(base.Mission.PlayerTeam.Side) == 0)
				{
					this.OnFailed();
					return;
				}
				if (this.GetAgentCountOfSide(Extensions.GetOppositeSide(base.Mission.PlayerTeam.Side)) == 0)
				{
					this.OnSuccess();
					return;
				}
				if (!this._enemyMissionShips.Any<MissionShip>((MissionShip x) => this.IsShipActive(x)))
				{
					this.OnSuccess();
				}
			}
		}

		// Token: 0x0600079C RID: 1948 RVA: 0x00035C4C File Offset: 0x00033E4C
		private void TickGunnarsShip()
		{
			if (this.IsShipActive(this._gunnarMissionShip) && !this._gunnarMissionShip.GetIsConnectedToEnemy())
			{
				if (this.IsShipAlerted(this._gunnarMissionShip))
				{
					if (this._gunnarMissionShip.ShipOrder.TargetShip == null || (this._gunnarMissionShip.ShipOrder.TargetShip == this._fahdaMissionShip && this.IsShipActive(this._fahdaMissionShip)) || !this.IsShipActive(this._gunnarMissionShip.ShipOrder.TargetShip))
					{
						MissionShip missionShip = (from x in this._enemyMissionShips
							where x != this._fahdaMissionShip && this.IsShipActive(x)
							select x into y
							orderby y.GameEntity.GlobalPosition.Distance(this._gunnarMissionShip.GameEntity.GlobalPosition)
							select y).FirstOrDefault<MissionShip>() ?? this._gunnarMissionShip.ShipOrder.ClosestEnemyShip;
						if (missionShip == null)
						{
							this._gunnarMissionShip.ShipOrder.SetShipStopOrder();
							return;
						}
						if (this._gunnarMissionShip.ShipOrder.TargetShip == null || missionShip != this._gunnarMissionShip.ShipOrder.TargetShip)
						{
							this._gunnarMissionShip.SetAnchor(false, false, 1f);
							this._gunnarMissionShip.ShipOrder.SetShipEngageOrder(missionShip);
							this._gunnarMissionShip.ShipOrder.SetBoardingTargetShip(missionShip);
							this._gunnarMissionShip.ShipOrder.IsBoardingAvailable = true;
							return;
						}
					}
				}
				else
				{
					if (this._gunnarMissionShip.GameEntity.GlobalPosition.Distance(this._gunnarInitialDestination.ToVec3(0f)) < 10f)
					{
						this._gunnarMissionShip.SetAnchor(true, true, 1f);
						return;
					}
					this._gunnarMissionShip.SetAnchor(false, false, 1f);
					this._gunnarMissionShip.ShipOrder.SetShipMovementOrder(in this._gunnarInitialDestination);
				}
			}
		}

		// Token: 0x0600079D RID: 1949 RVA: 0x00035E10 File Offset: 0x00034010
		private bool IsShipAlerted(MissionShip ship)
		{
			bool flag;
			return this._alertedShips.TryGetValue(ship, out flag) && flag;
		}

		// Token: 0x0600079E RID: 1950 RVA: 0x00035E2D File Offset: 0x0003402D
		private bool IsShipActive(MissionShip ship)
		{
			return !ship.IsDisabled && ship.Formation.CountOfUnits > 0 && !ship.IsSinking;
		}

		// Token: 0x0600079F RID: 1951 RVA: 0x00035E50 File Offset: 0x00034050
		private void OnPhase1Tick(float dt)
		{
			MissionShip fahdaMissionShip = this._fahdaMissionShip;
			if (fahdaMissionShip != null && fahdaMissionShip.IsSinking)
			{
				this.OnTargetShipSunk();
				this._inPhase1 = false;
				return;
			}
			foreach (MissionShip missionShip in this._enemyMissionShips)
			{
				MissionShip missionShip3;
				if (missionShip != this._fahdaMissionShip)
				{
					if (this.IsShipActive(missionShip))
					{
						if (!this.IsShipAlerted(missionShip))
						{
							MissionShip missionShip2;
							bool flag;
							if (this._laharMissionShip.GetIsConnectedToEnemy(out missionShip2))
							{
								if (missionShip2 == missionShip)
								{
									this.AlertShip(missionShip, this._laharMissionShip);
									this.AlertShip(this._gunnarMissionShip, missionShip);
									this.TriggerSmallerShipNotifications(true);
								}
								if (missionShip2 == this._fahdaMissionShip)
								{
									this.AlertShip(missionShip, this._laharMissionShip);
									this.TriggerTargetShipNotifications();
								}
							}
							else if (this._gunnarMissionShip.GetIsConnectedToEnemy(out missionShip2))
							{
								if (missionShip2 == missionShip)
								{
									this.AlertShip(missionShip, this._gunnarMissionShip);
								}
							}
							else if (this._shipsToAlert.TryGetValue(missionShip, out flag) && flag)
							{
								this.AlertShip(missionShip, null);
							}
						}
						else
						{
							this.TickEnemyShip(missionShip);
							if (missionShip.ShipOrder.GetIsAttemptingBoarding() && missionShip.ShipOrder.TargetShip == this._laharMissionShip)
							{
								this.TriggerSmallerShipNotifications(false);
							}
						}
					}
				}
				else if (this._laharMissionShip.GetIsConnectedToEnemy(out missionShip3) && this._fahdaMissionShip == missionShip3)
				{
					this.TriggerTargetShipNotifications();
					this.AlertShip(this._gunnarMissionShip, this._gunnarMissionShip.ShipOrder.ClosestEnemyShip ?? missionShip);
				}
			}
			this.MoveEscortShipsToTheirDefencePositions(dt);
		}

		// Token: 0x060007A0 RID: 1952 RVA: 0x00036008 File Offset: 0x00034208
		private void CheckDrowningAgents(float dt)
		{
			this._drownCheckTimer += dt;
			if (this._drownCheckTimer >= this._drownCheckDuration)
			{
				this._drownCheckTimer = 0f;
				for (int i = this._enemyMissionShips.Count - 1; i >= 0; i--)
				{
					this.CheckDrowningAgents(this._enemyMissionShips[i]);
				}
			}
		}

		// Token: 0x060007A1 RID: 1953 RVA: 0x00036068 File Offset: 0x00034268
		private void CheckDrowningAgents(MissionShip ship)
		{
			foreach (Agent agent in this._navalAgentsLogic.GetActiveAgentsOfShip(ship).ToList<Agent>())
			{
				if (!agent.IsHero && agent.CurrentMortalityState == null && agent.IsActive() && agent.IsInWater())
				{
					this.DrownAgent(agent, MBRandom.RandomInt(10, 100));
				}
			}
		}

		// Token: 0x060007A2 RID: 1954 RVA: 0x000360F0 File Offset: 0x000342F0
		private void DrownAgent(Agent agent, int inflictedDamage)
		{
			Blow blow;
			blow..ctor(agent.Index);
			blow.DamageType = 2;
			blow.BoneIndex = agent.Monster.HeadLookDirectionBoneIndex;
			blow.BaseMagnitude = (float)inflictedDamage;
			blow.GlobalPosition = agent.Position;
			blow.GlobalPosition.z = blow.GlobalPosition.z + agent.GetEyeGlobalHeight();
			blow.DamagedPercentage = 1f;
			blow.WeaponRecord.FillAsMeleeBlow(null, null, -1, -1);
			blow.SwingDirection = agent.LookDirection;
			blow.Direction = blow.SwingDirection;
			blow.InflictedDamage = inflictedDamage;
			blow.DamageCalculated = true;
			sbyte mainHandItemBoneIndex = agent.Monster.MainHandItemBoneIndex;
			AttackCollisionData attackCollisionDataForDebugPurpose = AttackCollisionData.GetAttackCollisionDataForDebugPurpose(false, false, false, true, false, false, false, false, false, false, false, false, 1, -1, 0, 2, blow.BoneIndex, 0, mainHandItemBoneIndex, 2, -1, 0, 0.5f, 1f, 0f, 0f, 0f, 0f, 0f, 0f, Vec3.Up, blow.Direction, blow.GlobalPosition, Vec3.Zero, Vec3.Zero, agent.Velocity, Vec3.Up);
			agent.RegisterBlow(blow, ref attackCollisionDataForDebugPurpose);
			agent.MakeVoice(SkinVoiceManager.VoiceType.Drown, 2);
			if (agent.Controller == 1)
			{
				Vec3 vec = new Vec3(0f, 0f, -20f, -1f);
				agent.AddAcceleration(ref vec);
			}
		}

		// Token: 0x060007A3 RID: 1955 RVA: 0x00036254 File Offset: 0x00034454
		private void TickEnemyShip(MissionShip ship)
		{
			if (this.IsShipActive(ship) && !ship.GetIsConnectedToEnemy() && this.IsShipAlerted(ship) && ship.ShipOrder.TargetShip == null)
			{
				MissionShip missionShip = (this.IsShipActive(this._laharMissionShip) ? this._laharMissionShip : ship.ShipOrder.ClosestEnemyShip);
				if (missionShip == null)
				{
					ship.ShipOrder.SetShipStopOrder();
					return;
				}
				ship.SetAnchor(false, false, 1f);
				ship.ShipOrder.SetShipEngageOrder(missionShip);
			}
		}

		// Token: 0x060007A4 RID: 1956 RVA: 0x000362D4 File Offset: 0x000344D4
		private void CheckTargetShipNearEscapePoint()
		{
			if (!this._nearFleePoint && this.IsShipActive(this._fahdaMissionShip) && !this._fahdaMissionShip.GetIsConnectedToEnemy() && this._fahdaMissionShip.GameEntity.GlobalPosition.AsVec2.Distance(this._fleePoint) < this._startDistanceToFleePoint * 0.33f)
			{
				this._nearFleePoint = true;
				if (!this._fahdaMissionShip.GetIsConnectedToEnemy())
				{
					CampaignInformationManager.AddDialogLine(new TextObject("{=KMNUcHJ5}The winds are still strong and a new squall could brew up at any time. If she gets much further we might lose sight of her.", null), NavalStorylineData.Lahar.CharacterObject, null, 0, 2);
				}
			}
		}

		// Token: 0x060007A5 RID: 1957 RVA: 0x00036370 File Offset: 0x00034570
		private void TriggerTargetShipNotifications()
		{
			if (!this._targetedBiggerVessel)
			{
				CampaignInformationManager.AddDialogLine(new TextObject("{=isa8iCbC}No! No! If you board that monster we’re finished! Cut loose!", null), NavalStorylineData.Lahar.CharacterObject, null, 0, 2);
				this._targetedBiggerVessel = true;
			}
		}

		// Token: 0x060007A6 RID: 1958 RVA: 0x000363A0 File Offset: 0x000345A0
		private void TriggerSmallerShipNotifications(bool hasPlayerAttemptedToBoard)
		{
			if (hasPlayerAttemptedToBoard && !this._targetedSmallerVessels && this.IsShipActive(this._fahdaMissionShip))
			{
				CampaignInformationManager.AddDialogLine(new TextObject("{=AFdg8UHM}Go for her flagship! We don’t want it to get away! We’ll deal with the lesser vessels later.", null), NavalStorylineData.Lahar.CharacterObject, null, 0, 2);
				this._targetedSmallerVessels = true;
			}
			if (!hasPlayerAttemptedToBoard && !this._targetedBySmallerVessels && this.IsShipActive(this._fahdaMissionShip))
			{
				CampaignInformationManager.AddDialogLine(new TextObject("{=HOAwSlCQ}One of the others is going to board us! Repel them and cut loose, or we’ll never catch her!", null), NavalStorylineData.Lahar.CharacterObject, null, 0, 2);
				this._targetedBySmallerVessels = true;
			}
		}

		// Token: 0x060007A7 RID: 1959 RVA: 0x0003642C File Offset: 0x0003462C
		private void OnTargetShipSunk()
		{
			this.AlertAllShips();
			List<MissionShip> list = this._enemyMissionShips.Where<MissionShip>((MissionShip x) => x != this._fahdaMissionShip).ToList<MissionShip>();
			if (list.Count > 0)
			{
				FinishOffConsortsObjective finishOffConsortsObjective = new FinishOffConsortsObjective(base.Mission, list);
				this._missionObjectiveLogic.StartObjective(finishOffConsortsObjective);
				CampaignInformationManager.AddDialogLine(new TextObject("{=CzYbzDM8}Good! You dealt her ship a mortal wound. It’s going down! Now, finish off its consorts.", null), NavalStorylineData.Lahar.CharacterObject, null, 3000, 2);
			}
		}

		// Token: 0x060007A8 RID: 1960 RVA: 0x000364A0 File Offset: 0x000346A0
		private void MoveEscortShipsToTheirDefencePositions(float dt)
		{
			this._fahdaMissionShip.ShipOrder.IsBoardingAvailable = false;
			Vec2 vec;
			Vec2 vec2;
			Vec2 vec3;
			this.GetDefencePositionsForReinforcementShips(out vec, out vec2, out vec3);
			foreach (MissionShip missionShip in this._enemyMissionShips)
			{
				if (missionShip != this._fahdaMissionShip && this.IsShipActive(missionShip) && !this.IsShipAlerted(missionShip))
				{
					Vec2 vec4 = missionShip.GameEntity.GlobalPosition.AsVec2;
					if (missionShip == this._enemyReinforcementFirstMissionShip)
					{
						vec4 = vec3;
					}
					else if (missionShip == this._enemyReinforcementSecondMissionShip)
					{
						vec4 = vec2;
					}
					else if (missionShip == this._enemyReinforcementThirdMissionShip)
					{
						vec4 = vec;
					}
					missionShip.ShipOrder.IsBoardingAvailable = false;
					missionShip.ShipOrder.SetShipMovementOrder(in vec4);
				}
			}
		}

		// Token: 0x060007A9 RID: 1961 RVA: 0x00036590 File Offset: 0x00034790
		private void OnSuccess()
		{
			this._isMissionSuccessful = true;
			PlayerEncounter.CampaignBattleResult = CampaignBattleResult.GetResult(1, false);
			MBInformationManager.AddQuickInformation(new TextObject("{=15aPhWar}Success! You defeated Fahda's fleet.", null), 2000, null, null, "");
		}

		// Token: 0x060007AA RID: 1962 RVA: 0x000365C1 File Offset: 0x000347C1
		private void OnFailed()
		{
			this._isMissionFailed = true;
			PlayerEncounter.CampaignBattleResult = CampaignBattleResult.GetResult(2, false);
		}

		// Token: 0x060007AB RID: 1963 RVA: 0x000365D8 File Offset: 0x000347D8
		public override bool MissionEnded(ref MissionResult missionResult)
		{
			bool flag = false;
			if (this._isMissionSuccessful)
			{
				missionResult = MissionResult.CreateSuccessful(base.Mission, false);
				flag = true;
			}
			else if (this._isMissionFailed)
			{
				missionResult = MissionResult.CreateDefeated(base.Mission);
				flag = true;
			}
			return flag;
		}

		// Token: 0x060007AC RID: 1964 RVA: 0x0003661C File Offset: 0x0003481C
		private void UpdateSceneWindDirectionAndWaterStrength()
		{
			Scene scene = Mission.Current.Scene;
			Vec2 vec = base.Mission.Scene.FindWeakEntityWithTag("sp_wind_direction").GetGlobalFrame().rotation.f.AsVec2 * 12f;
			scene.SetGlobalWindVelocity(ref vec);
			Mission.Current.Scene.SetWaterStrength(3f);
		}

		// Token: 0x060007AD RID: 1965 RVA: 0x00036688 File Offset: 0x00034888
		private MissionShip CreateShip(IShipOrigin ship, Team team, Formation formation, WeakGameEntity spawnEntity)
		{
			MatrixFrame globalFrame = spawnEntity.GetGlobalFrame();
			float waterLevelAtPosition = Mission.Current.Scene.GetWaterLevelAtPosition(spawnEntity.GlobalPosition.AsVec2, false, false);
			globalFrame.origin = new Vec3(spawnEntity.GlobalPosition.x, spawnEntity.GlobalPosition.y, waterLevelAtPosition, -1f);
			MissionShip missionShip = this._navalShipsLogic.SpawnShip(ship, in globalFrame, team, formation, false, 8, true);
			missionShip.ShipOrder.FormationJoinShip(formation);
			if (team.IsEnemyOf(base.Mission.PlayerTeam))
			{
				missionShip.ShipControllerMachine.PilotStandingPoint.IsDisabledForPlayers = true;
			}
			return missionShip;
		}

		// Token: 0x060007AE RID: 1966 RVA: 0x00036730 File Offset: 0x00034930
		public void AlertShip(MissionShip missionShip, MissionShip target = null)
		{
			if (this.CanAlertShip(missionShip))
			{
				bool flag;
				if (this._shipsToAlert.TryGetValue(missionShip, out flag) && flag)
				{
					this._shipsToAlert[missionShip] = false;
				}
				missionShip.ShipOrder.IsBoardingAvailable = true;
				this._alertedShips[missionShip] = true;
				missionShip.SetAnchor(false, false, 1f);
				target = target ?? missionShip.ShipOrder.ClosestEnemyShip;
				if (target != null)
				{
					missionShip.ShipOrder.SetShipEngageOrder(target);
				}
			}
		}

		// Token: 0x060007AF RID: 1967 RVA: 0x000367AC File Offset: 0x000349AC
		private void AlertAllEnemyShips()
		{
			foreach (MissionShip missionShip in this._enemyMissionShips)
			{
				if (missionShip != this._fahdaMissionShip)
				{
					this.AlertShip(missionShip, this._laharMissionShip);
				}
			}
		}

		// Token: 0x060007B0 RID: 1968 RVA: 0x00036810 File Offset: 0x00034A10
		private void AlertAllShips()
		{
			this.AlertAllEnemyShips();
			this.AlertShip(this._gunnarMissionShip, null);
		}

		// Token: 0x060007B1 RID: 1969 RVA: 0x00036825 File Offset: 0x00034A25
		private bool CanAlertShip(MissionShip missionShip)
		{
			return this.IsShipActive(missionShip) && !this.IsShipAlerted(missionShip);
		}

		// Token: 0x060007B2 RID: 1970 RVA: 0x0003683C File Offset: 0x00034A3C
		private void Initialize()
		{
			this._inPhase1 = true;
			this._fleePoint = base.Mission.Scene.FindWeakEntityWithTag("sp_flee_point").GlobalPosition.AsVec2;
			this._gunnarInitialDestination = base.Mission.Scene.FindWeakEntityWithTag("sp_gangradir_ship_destination").GlobalPosition.AsVec2;
			this._initialized = true;
			CampaignInformationManager.AddDialogLine(new TextObject("{=Gdaayb1y}Ha! It looks like her ship took a lot of damage. Her crew must not have furled the sails properly before the winds hit, and now she’s just limping along. Sink her!", null), NavalStorylineData.Lahar.CharacterObject, null, 0, 2);
			this._availailableEnemyFormations.AddRange(base.Mission.PlayerEnemyTeam.FormationsIncludingEmpty);
			this._navalShipsLogic.SetDeploymentMode(true);
			this.SpawnPlayerSide();
			this.SpawnEnemySide();
			foreach (MissionShip missionShip in this._playerMissionShips)
			{
				missionShip.SetShipOrderActive(true);
			}
			foreach (MissionShip missionShip2 in this._enemyMissionShips)
			{
				missionShip2.SetShipOrderActive(true);
				foreach (ShipAttachmentMachine shipAttachmentMachine in missionShip2.ShipAttachmentMachines)
				{
					shipAttachmentMachine.SetIsDisabledForAI(true);
				}
			}
			this._navalShipsLogic.TeleportShip(this._laharMissionShip, this._laharMissionShip.GameEntity.GetGlobalFrame(), true, false, true);
			this._navalShipsLogic.TeleportShip(this._gunnarMissionShip, this._gunnarMissionShip.GameEntity.GetGlobalFrame(), true, false, true);
			this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(0);
			this._navalAgentsLogic.AssignAndTeleportCrewToShipMachines(2);
			Mission.Current.OnDeploymentFinished();
			this._navalShipsLogic.SetDeploymentMode(false);
			this.UpdateSceneWindDirectionAndWaterStrength();
		}

		// Token: 0x060007B3 RID: 1971 RVA: 0x00036A44 File Offset: 0x00034C44
		private void OnShipRammed(MissionShip ship1, MissionShip ship2, float damagePercent, bool isFirstImpact, CapsuleData data, int ramQuality)
		{
			if (ship1 == this._laharMissionShip && ship2 != this._fahdaMissionShip && isFirstImpact && this._fahdaMissionShip.Formation.CountOfUnits > 0 && ship2.Team.IsEnemyOf(base.Mission.PlayerTeam))
			{
				this.TriggerSmallerShipNotifications(true);
				if (this.CanAlertShip(ship2) && damagePercent < 1f)
				{
					this._shipsToAlert[ship2] = true;
				}
			}
			if (ship1 == this._laharMissionShip && ship2 == this._fahdaMissionShip && isFirstImpact)
			{
				foreach (MissionShip missionShip in this._enemyMissionShips)
				{
					if (missionShip != this._fahdaMissionShip && this.CanAlertShip(missionShip))
					{
						this._shipsToAlert[missionShip] = true;
					}
				}
				if (this._fahdaMissionShip.Formation.CountOfUnits > 0 && damagePercent < 1f)
				{
					CampaignInformationManager.AddDialogLine(new TextObject("{=18qp71BY}Well done! Give her another one, for luck.", null), NavalStorylineData.Lahar.CharacterObject, null, 0, 2);
				}
				if (ship2 == this._fahdaMissionShip && isFirstImpact && damagePercent > 1f)
				{
					this._navalShipsLogic.ShipRammingEvent -= this.OnShipRammed;
				}
			}
		}

		// Token: 0x060007B4 RID: 1972 RVA: 0x00036BA0 File Offset: 0x00034DA0
		private void SpawnPlayerSide()
		{
			Team team = Mission.GetTeam(0);
			WeakGameEntity weakGameEntity = base.Mission.Scene.FindWeakEntityWithTag("sp_lahar_ship");
			ShipHull questShipHull = Campaign.Current.ObjectManager.GetObject<ShipHull>("ship_liburna_q2_storyline");
			Ship ship;
			if ((ship = MobileParty.MainParty.Ships.FirstOrDefault<Ship>((Ship x) => x.ShipHull == questShipHull)) == null)
			{
				Ship ship2 = new Ship(questShipHull);
				ship2.IsTradeable = false;
				ship2.IsUsedByQuest = true;
				ship = ship2;
				ship2.Owner = PartyBase.MainParty;
			}
			this._laharShip = ship;
			this._laharShip.ChangeFigurehead(DefaultFigureheads.Hawk);
			this.AddShipUpgradePieces(this._laharShip, WoundedBeastMissionController.LaharShipUpgradePieces);
			this._laharMissionShip = this.CreateShip(this._laharShip, base.Mission.PlayerTeam, team.GetFormation(0), weakGameEntity);
			this._playerMissionShips.Add(this._laharMissionShip);
			this._playerShips.Add(this._laharShip);
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._laharMissionShip, this._laharShipTroops.Sum<WoundedBeastMissionController.StorylineTroop>((WoundedBeastMissionController.StorylineTroop t) => t.TroopCount) + 2);
			this.SpawnNonHeroAgents(this._laharMissionShip, this._laharShipTroops, PartyBase.MainParty, null);
			this.SpawnHero(CharacterObject.PlayerCharacter, this._laharMissionShip, PartyBase.MainParty, null);
			this.SpawnHero(NavalStorylineData.Lahar.CharacterObject, this._laharMissionShip, PartyBase.MainParty, null);
			WeakGameEntity weakGameEntity2 = base.Mission.Scene.FindWeakEntityWithTag("sp_gangradir_ship");
			ShipHull northernMediumShipHull = Campaign.Current.ObjectManager.GetObject<ShipHull>("northern_medium_ship");
			Ship ship3;
			if ((ship3 = MobileParty.MainParty.Ships.FirstOrDefault<Ship>((Ship x) => x.ShipHull == northernMediumShipHull)) == null)
			{
				Ship ship4 = new Ship(Campaign.Current.ObjectManager.GetObject<ShipHull>("northern_medium_ship"));
				ship4.IsTradeable = false;
				ship4.IsUsedByQuest = true;
				ship3 = ship4;
				ship4.Owner = PartyBase.MainParty;
			}
			this._gunnarShip = ship3;
			this._gunnarShip.ChangeFigurehead(DefaultFigureheads.Dragon);
			this.AddShipUpgradePieces(this._gunnarShip, WoundedBeastMissionController.GunnarShipUpgradePieces);
			this._gunnarMissionShip = this.CreateShip(this._gunnarShip, base.Mission.PlayerTeam, team.GetFormation(1), weakGameEntity2);
			this._playerMissionShips.Add(this._gunnarMissionShip);
			this._playerShips.Add(this._gunnarShip);
			this._alertedShips[this._gunnarMissionShip] = false;
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._gunnarMissionShip, this._gunnarShipTroops.Sum<WoundedBeastMissionController.StorylineTroop>((WoundedBeastMissionController.StorylineTroop t) => t.TroopCount) + 1);
			this.SpawnNonHeroAgents(this._gunnarMissionShip, this._gunnarShipTroops, PartyBase.MainParty, null);
			this.SpawnHero(NavalStorylineData.Gunnar.CharacterObject, this._gunnarMissionShip, PartyBase.MainParty, null);
			this._navalAgentsLogic.SpawnNextBatch(0, false, null);
			Agent.Main.Controller = 2;
			Agent.Main.Formation.PlayerOwner = Agent.Main;
			Mission.Current.PlayerTeam.PlayerOrderController.Owner = Agent.Main;
			Agent agent = base.Mission.Agents.First<Agent>((Agent x) => x.IsHero && x.Character == NavalStorylineData.Gunnar.CharacterObject);
			agent.Formation.PlayerOwner = agent;
			base.Mission.PlayerTeam.PlayerOrderController.Owner = Agent.Main;
			this._navalAgentsLogic.AssignCaptainToShipForDeploymentMode(Agent.Main, this._laharMissionShip, null);
			this._navalAgentsLogic.AssignCaptainToShipForDeploymentMode(agent, this._gunnarMissionShip, null);
		}

		// Token: 0x060007B5 RID: 1973 RVA: 0x00036F60 File Offset: 0x00035160
		private void SpawnEnemySide()
		{
			PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
			Formation formation = this._availailableEnemyFormations.First<Formation>();
			this._availailableEnemyFormations.RemoveAt(0);
			ShipHull fahdaShipHull = Campaign.Current.ObjectManager.GetObject<ShipHull>("ship_meditheavy_storyline");
			Ship ship;
			if ((ship = encounteredParty.Ships.FirstOrDefault<Ship>((Ship x) => x.ShipHull == fahdaShipHull)) == null)
			{
				Ship ship2 = new Ship(fahdaShipHull);
				ship2.IsTradeable = false;
				ship2.IsUsedByQuest = true;
				ship = ship2;
				ship2.Owner = encounteredParty;
			}
			this._fahdaShip = ship;
			this._fahdaShip.ChangeFigurehead(DefaultFigureheads.Viper);
			this.AddShipUpgradePieces(this._gunnarShip, WoundedBeastMissionController.FahdaShipUpgradePieces);
			this._fahdaMissionShip = this.CreateShip(this._fahdaShip, base.Mission.PlayerEnemyTeam, formation, base.Mission.Scene.FindWeakEntityWithTag("sp_fahda_ship"));
			this._fahdaMissionShip.SetCustomSailSetting(true, SailInput.Raised);
			this._fahdaMissionShip.Formation.SetControlledByAI(false, false);
			this._fahdaMissionShip.SetCanBeTakenOver(false);
			if (this._missionObjectiveLogic != null)
			{
				SinkShipObjective sinkShipObjective = new SinkShipObjective(base.Mission, this._fahdaMissionShip);
				this._missionObjectiveLogic.StartObjective(sinkShipObjective);
			}
			this._enemyShips.Add(this._fahdaShip);
			this._enemyMissionShips.Add(this._fahdaMissionShip);
			List<WeakGameEntity> list = this._fahdaMissionShip.GameEntity.CollectChildrenEntitiesWithTag("targeting_entity");
			this.EnemyReinforcementThirdShipTargetEntity = list.FirstOrDefault<WeakGameEntity>((WeakGameEntity t) => t.HasTag("targeting_entity_3"));
			this.EnemyReinforcementSecondShipTargetEntity = list.FirstOrDefault<WeakGameEntity>((WeakGameEntity t) => t.HasTag("targeting_entity_2"));
			this.EnemyReinforcementFirstShipTargetEntity = list.FirstOrDefault<WeakGameEntity>((WeakGameEntity t) => t.HasTag("targeting_entity_1"));
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(this._fahdaMissionShip, this._fahdaShipTroops.Sum<WoundedBeastMissionController.StorylineTroop>((WoundedBeastMissionController.StorylineTroop t) => t.TroopCount) + 1);
			this._navalAgentsLogic.AddReservedTroopToShip(new PartyAgentOrigin(encounteredParty, NavalStorylineData.EmiraAlFahda.CharacterObject, -1, default(UniqueTroopDescriptor), false, true), this._fahdaMissionShip);
			this.SpawnNonHeroAgents(this._fahdaMissionShip, this._fahdaShipTroops, encounteredParty, NavalStorylineData.CorsairBanner);
			this._enemyReinforcementFirstMissionShip = this.SpawnReinforcementShip(this.EnemyReinforcementThirdShipTargetEntity, this._enemyReinforcementThirdShipTroops, "ship_liburna_storyline", WoundedBeastMissionController.MediumReinforcementShipUpgradePieces);
			this._enemyReinforcementSecondMissionShip = this.SpawnReinforcementShip(this.EnemyReinforcementSecondShipTargetEntity, this._enemyReinforcementSecondShipTroops, "ship_meditlight_storyline", WoundedBeastMissionController.FirstLightReinforcementShipUpgradePieces);
			this._enemyReinforcementThirdMissionShip = this.SpawnReinforcementShip(this.EnemyReinforcementFirstShipTargetEntity, this._enemyReinforcementFirstShipTroops, "ship_meditlight_storyline", WoundedBeastMissionController.SecondLightReinforcementShipUpgradePieces);
			this._navalAgentsLogic.SpawnNextBatch(2, false, this._enemySideAgents);
			this._startDistanceToFleePoint = this._fahdaMissionShip.GameEntity.GlobalPosition.AsVec2.Distance(this._fleePoint);
		}

		// Token: 0x060007B6 RID: 1974 RVA: 0x00037280 File Offset: 0x00035480
		private MissionShip SpawnReinforcementShip(WeakGameEntity targetEntity, List<WoundedBeastMissionController.StorylineTroop> troops, string shipHullId, Dictionary<string, string> upgradePieces)
		{
			PartyBase encounteredParty = PlayerEncounter.EncounteredParty;
			Formation formation = this._availailableEnemyFormations.First<Formation>();
			this._availailableEnemyFormations.RemoveAt(0);
			int num = troops.Sum<WoundedBeastMissionController.StorylineTroop>((WoundedBeastMissionController.StorylineTroop t) => t.TroopCount);
			ShipHull reinforcementShipHull = Campaign.Current.ObjectManager.GetObject<ShipHull>(shipHullId);
			Ship ship;
			if ((ship = PlayerEncounter.EncounteredParty.Ships.FirstOrDefault<Ship>((Ship x) => x.ShipHull == reinforcementShipHull)) == null)
			{
				Ship ship2 = new Ship(reinforcementShipHull);
				ship2.IsTradeable = false;
				ship2.IsUsedByQuest = true;
				ship = ship2;
				ship2.Owner = PlayerEncounter.EncounteredParty;
			}
			Ship ship3 = ship;
			this.AddShipUpgradePieces(ship3, upgradePieces);
			MissionShip missionShip = this.CreateShip(ship3, base.Mission.PlayerEnemyTeam, formation, targetEntity);
			missionShip.SetCanBeTakenOver(false);
			this._enemyShips.Add(ship3);
			this._enemyMissionShips.Add(missionShip);
			this._alertedShips[missionShip] = false;
			this._navalAgentsLogic.SetDesiredTroopCountOfShip(missionShip, num);
			this.SpawnNonHeroAgents(missionShip, troops, encounteredParty, NavalStorylineData.CorsairBanner);
			return missionShip;
		}

		// Token: 0x060007B7 RID: 1975 RVA: 0x000373A4 File Offset: 0x000355A4
		private void SpawnHero(CharacterObject character, MissionShip ship, PartyBase party, Banner customBanner = null)
		{
			Banner banner = customBanner ?? party.Banner;
			character.HeroObject.Heal(character.HeroObject.MaxHitPoints, false);
			PartyAgentOrigin partyAgentOrigin = new PartyAgentOrigin(party, character, -1, default(UniqueTroopDescriptor), false, true);
			partyAgentOrigin.SetBanner(banner);
			this._navalAgentsLogic.AddReservedTroopToShip(partyAgentOrigin, ship);
		}

		// Token: 0x060007B8 RID: 1976 RVA: 0x00037400 File Offset: 0x00035600
		private void SpawnNonHeroAgents(MissionShip ship, List<WoundedBeastMissionController.StorylineTroop> troopTypes, PartyBase party, Banner customBanner = null)
		{
			Banner banner = customBanner ?? party.Banner;
			foreach (WoundedBeastMissionController.StorylineTroop storylineTroop in troopTypes)
			{
				CharacterObject @object = Campaign.Current.ObjectManager.GetObject<CharacterObject>(storylineTroop.TroopId);
				for (int i = 0; i < storylineTroop.TroopCount; i++)
				{
					PartyAgentOrigin partyAgentOrigin = new PartyAgentOrigin(party, @object, -1, default(UniqueTroopDescriptor), false, true);
					partyAgentOrigin.SetBanner(banner);
					this._navalAgentsLogic.AddReservedTroopToShip(partyAgentOrigin, ship);
				}
			}
		}

		// Token: 0x060007B9 RID: 1977 RVA: 0x000374B0 File Offset: 0x000356B0
		private int GetAgentCountOfSide(BattleSideEnum side)
		{
			BattleSideEnum side2 = base.Mission.PlayerTeam.Side;
			int num = 0;
			if (side2 == side)
			{
				using (List<MissionShip>.Enumerator enumerator = this._playerMissionShips.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MissionShip missionShip = enumerator.Current;
						num += this._navalAgentsLogic.GetActiveAgentCountOfShip(missionShip);
					}
					return num;
				}
			}
			foreach (MissionShip missionShip2 in this._enemyMissionShips)
			{
				num += this._navalAgentsLogic.GetActiveAgentCountOfShip(missionShip2);
			}
			return num;
		}

		// Token: 0x060007BA RID: 1978 RVA: 0x0003756C File Offset: 0x0003576C
		private void GetDefencePositionsForReinforcementShips(out Vec2 leftSide, out Vec2 rightSide, out Vec2 behind)
		{
			Vec2 vec = (this._laharMissionShip.GameEntity.GlobalPosition.AsVec2 - this._fahdaMissionShip.GameEntity.GlobalPosition.AsVec2) / 2f;
			MatrixFrame matrixFrame = this._fahdaMissionShip.GameEntity.GetLocalFrame();
			Vec2 asVec = matrixFrame.rotation.f.AsVec2;
			float num = 300f;
			float num2 = 200f;
			float num3 = 0.62831855f;
			float num4 = 2.5132742f;
			behind = this.EnemyReinforcementThirdShipTargetEntity.GlobalPosition.AsVec2;
			if (asVec.AngleBetween(vec) < -num3 && asVec.AngleBetween(vec) > -num4)
			{
				if (vec.Length == 0f)
				{
					matrixFrame = this._fahdaMissionShip.GameEntity.GetLocalFrame();
					vec = matrixFrame.rotation.f.AsVec2 * 30f;
				}
				else if (vec.Length < num2)
				{
					vec *= num2 / vec.Length;
				}
				else if (vec.Length > num)
				{
					vec *= num / vec.Length;
				}
				rightSide = this._fahdaMissionShip.GameEntity.GlobalPosition.AsVec2 + vec;
			}
			else
			{
				rightSide = this.EnemyReinforcementSecondShipTargetEntity.GlobalPosition.AsVec2;
			}
			leftSide = rightSide + asVec * num2;
		}

		// Token: 0x060007BB RID: 1979 RVA: 0x00037714 File Offset: 0x00035914
		private void AddShipUpgradePieces(Ship ship, Dictionary<string, string> upgradePieces)
		{
			using (Dictionary<string, string>.Enumerator enumerator = upgradePieces.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, string> kv = enumerator.Current;
					ShipUpgradePiece @object = MBObjectManager.Instance.GetObject<ShipUpgradePiece>(kv.Value);
					if (ship.ShipHull.AvailableSlots.Any<KeyValuePair<string, ShipSlot>>((KeyValuePair<string, ShipSlot> slot) => slot.Key == kv.Key))
					{
						ship.EquipUpgradePiece(kv.Key, @object);
					}
				}
			}
		}

		// Token: 0x040004A4 RID: 1188
		private const string WindDirection = "sp_wind_direction";

		// Token: 0x040004A5 RID: 1189
		private const string TargetEntityTag = "targeting_entity";

		// Token: 0x040004A6 RID: 1190
		private const string GunnarInitialDestination = "sp_gangradir_ship_destination";

		// Token: 0x040004A7 RID: 1191
		private const string LaharShipSpawnId = "sp_lahar_ship";

		// Token: 0x040004A8 RID: 1192
		private const string GunnarShipSpawnId = "sp_gangradir_ship";

		// Token: 0x040004A9 RID: 1193
		private const string LaharShipHullId = "ship_liburna_q2_storyline";

		// Token: 0x040004AA RID: 1194
		private const string GunnarShipHullId = "northern_medium_ship";

		// Token: 0x040004AB RID: 1195
		private const string LaharMeleeTroopId = "southern_pirates_raider";

		// Token: 0x040004AC RID: 1196
		private const string LaharRangedTroopId = "aserai_marine_t5";

		// Token: 0x040004AD RID: 1197
		private const string GunnarMeleeTroopId = "gangradirs_kin_melee";

		// Token: 0x040004AE RID: 1198
		private const string GunnarRangedTroopId = "gangradirs_kin_ranged";

		// Token: 0x040004AF RID: 1199
		private readonly List<WoundedBeastMissionController.StorylineTroop> _laharShipTroops = new List<WoundedBeastMissionController.StorylineTroop>();

		// Token: 0x040004B0 RID: 1200
		private readonly List<WoundedBeastMissionController.StorylineTroop> _gunnarShipTroops = new List<WoundedBeastMissionController.StorylineTroop>();

		// Token: 0x040004B1 RID: 1201
		private readonly List<Ship> _playerShips = new List<Ship>();

		// Token: 0x040004B2 RID: 1202
		private readonly MBList<MissionShip> _playerMissionShips = new MBList<MissionShip>();

		// Token: 0x040004B3 RID: 1203
		private Ship _gunnarShip;

		// Token: 0x040004B4 RID: 1204
		private Ship _laharShip;

		// Token: 0x040004B5 RID: 1205
		private MissionShip _laharMissionShip;

		// Token: 0x040004B6 RID: 1206
		private MissionShip _gunnarMissionShip;

		// Token: 0x040004B7 RID: 1207
		private static readonly Dictionary<string, string> LaharShipUpgradePieces = new Dictionary<string, string>
		{
			{ "side", "side_southern_shields_lvl3" },
			{ "sail", "sails_lvl2" },
			{ "bow", "bow_northern_reinforced_ram_lvl3" }
		};

		// Token: 0x040004B8 RID: 1208
		private static readonly Dictionary<string, string> GunnarShipUpgradePieces = new Dictionary<string, string>
		{
			{ "side", "side_southern_shields_lvl2" },
			{ "sail", "sails_lvl2" }
		};

		// Token: 0x040004B9 RID: 1209
		private const string FahdaShipSpawnId = "sp_fahda_ship";

		// Token: 0x040004BA RID: 1210
		private const string FahdaShipHullId = "ship_meditheavy_storyline";

		// Token: 0x040004BB RID: 1211
		private const string MediumReinforcementShipHullId = "ship_liburna_storyline";

		// Token: 0x040004BC RID: 1212
		private const string LightReinforcementShipHullId = "ship_meditlight_storyline";

		// Token: 0x040004BD RID: 1213
		private const string EnemyMeleeTroopId1 = "southern_pirates_raider";

		// Token: 0x040004BE RID: 1214
		private const string EnemyMeleeTroopId2 = "aserai_footman";

		// Token: 0x040004BF RID: 1215
		private const string EnemyRangedTroopId = "southern_pirates_bandit";

		// Token: 0x040004C0 RID: 1216
		private readonly List<WoundedBeastMissionController.StorylineTroop> _fahdaShipTroops = new List<WoundedBeastMissionController.StorylineTroop>();

		// Token: 0x040004C1 RID: 1217
		private readonly List<WoundedBeastMissionController.StorylineTroop> _enemyReinforcementFirstShipTroops = new List<WoundedBeastMissionController.StorylineTroop>();

		// Token: 0x040004C2 RID: 1218
		private readonly List<WoundedBeastMissionController.StorylineTroop> _enemyReinforcementSecondShipTroops = new List<WoundedBeastMissionController.StorylineTroop>();

		// Token: 0x040004C3 RID: 1219
		private readonly List<WoundedBeastMissionController.StorylineTroop> _enemyReinforcementThirdShipTroops = new List<WoundedBeastMissionController.StorylineTroop>();

		// Token: 0x040004C4 RID: 1220
		private readonly MBList<Agent> _enemySideAgents = new MBList<Agent>();

		// Token: 0x040004C5 RID: 1221
		private readonly List<Formation> _availailableEnemyFormations = new List<Formation>();

		// Token: 0x040004C6 RID: 1222
		private readonly MBList<MissionShip> _enemyMissionShips = new MBList<MissionShip>();

		// Token: 0x040004C7 RID: 1223
		private readonly List<Ship> _enemyShips = new List<Ship>();

		// Token: 0x040004C8 RID: 1224
		private const string EnemyReinforcementFirstShipTargetEntityTag = "targeting_entity_1";

		// Token: 0x040004C9 RID: 1225
		private const string EnemyReinforcementSecondShipTargetEntityTag = "targeting_entity_2";

		// Token: 0x040004CA RID: 1226
		private const string EnemyReinforcementThirdShipTargetEntityTag = "targeting_entity_3";

		// Token: 0x040004CB RID: 1227
		private WeakGameEntity EnemyReinforcementFirstShipTargetEntity;

		// Token: 0x040004CC RID: 1228
		private WeakGameEntity EnemyReinforcementSecondShipTargetEntity;

		// Token: 0x040004CD RID: 1229
		private WeakGameEntity EnemyReinforcementThirdShipTargetEntity;

		// Token: 0x040004CE RID: 1230
		private Ship _fahdaShip;

		// Token: 0x040004CF RID: 1231
		private MissionShip _fahdaMissionShip;

		// Token: 0x040004D0 RID: 1232
		private MissionShip _enemyReinforcementFirstMissionShip;

		// Token: 0x040004D1 RID: 1233
		private MissionShip _enemyReinforcementSecondMissionShip;

		// Token: 0x040004D2 RID: 1234
		private MissionShip _enemyReinforcementThirdMissionShip;

		// Token: 0x040004D3 RID: 1235
		private static readonly Dictionary<string, string> FahdaShipUpgradePieces = new Dictionary<string, string> { { "side", "side_southern_shields_lvl2" } };

		// Token: 0x040004D4 RID: 1236
		private static readonly Dictionary<string, string> MediumReinforcementShipUpgradePieces = new Dictionary<string, string>
		{
			{ "side", "side_southern_shields_lvl2" },
			{ "sail", "sails_lvl2" }
		};

		// Token: 0x040004D5 RID: 1237
		private static readonly Dictionary<string, string> FirstLightReinforcementShipUpgradePieces = new Dictionary<string, string>
		{
			{ "side", "side_southern_shields_lvl2" },
			{ "sail", "sails_lvl2" }
		};

		// Token: 0x040004D6 RID: 1238
		private static readonly Dictionary<string, string> SecondLightReinforcementShipUpgradePieces = new Dictionary<string, string>
		{
			{ "side", "side_southern_shields_lvl2" },
			{ "sail", "sails_lvl2" }
		};

		// Token: 0x040004D7 RID: 1239
		private float _drownCheckTimer;

		// Token: 0x040004D8 RID: 1240
		private float _drownCheckDuration = 2f;

		// Token: 0x040004D9 RID: 1241
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x040004DA RID: 1242
		private NavalAgentsLogic _navalAgentsLogic;

		// Token: 0x040004DB RID: 1243
		private MissionObjectiveLogic _missionObjectiveLogic;

		// Token: 0x040004DC RID: 1244
		private Vec2 _fleePoint;

		// Token: 0x040004DD RID: 1245
		private Vec2 _gunnarInitialDestination;

		// Token: 0x040004DE RID: 1246
		private bool _initialized;

		// Token: 0x040004DF RID: 1247
		private bool _isMissionSuccessful;

		// Token: 0x040004E0 RID: 1248
		private bool _isMissionFailed;

		// Token: 0x040004E1 RID: 1249
		private bool _inPhase1 = true;

		// Token: 0x040004E2 RID: 1250
		private MissionTimer _failingQuestTimer;

		// Token: 0x040004E3 RID: 1251
		private float _startDistanceToFleePoint;

		// Token: 0x040004E4 RID: 1252
		private bool _nearFleePoint;

		// Token: 0x040004E5 RID: 1253
		private bool _targetedSmallerVessels;

		// Token: 0x040004E6 RID: 1254
		private bool _targetedBiggerVessel;

		// Token: 0x040004E7 RID: 1255
		private bool _targetedBySmallerVessels;

		// Token: 0x040004E8 RID: 1256
		private readonly Dictionary<MissionShip, bool> _shipsToAlert = new Dictionary<MissionShip, bool>();

		// Token: 0x040004E9 RID: 1257
		private readonly Dictionary<MissionShip, bool> _alertedShips = new Dictionary<MissionShip, bool>();

		// Token: 0x020001E4 RID: 484
		private struct StorylineTroop
		{
			// Token: 0x17000404 RID: 1028
			// (get) Token: 0x06001A7B RID: 6779 RVA: 0x000AF4D5 File Offset: 0x000AD6D5
			public string TroopId { get; }

			// Token: 0x17000405 RID: 1029
			// (get) Token: 0x06001A7C RID: 6780 RVA: 0x000AF4DD File Offset: 0x000AD6DD
			public int TroopCount { get; }

			// Token: 0x06001A7D RID: 6781 RVA: 0x000AF4E5 File Offset: 0x000AD6E5
			public StorylineTroop(string troopId, int troopCount)
			{
				this.TroopCount = troopCount;
				this.TroopId = troopId;
			}
		}
	}
}
