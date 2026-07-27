using System;
using NavalDLC.Missions;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200000F RID: 15
	public class NavalBattleEndLogic : MissionLogic, IBattleEndLogic
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600007C RID: 124 RVA: 0x00005561 File Offset: 0x00003761
		public bool PlayerVictory
		{
			get
			{
				return this.IsEnemySideRetreating || this._isEnemySideDepleted;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600007D RID: 125 RVA: 0x00005573 File Offset: 0x00003773
		public bool EnemyVictory
		{
			get
			{
				return this._isPlayerSideRetreating || this._isPlayerSideDepleted;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600007E RID: 126 RVA: 0x00005585 File Offset: 0x00003785
		// (set) Token: 0x0600007F RID: 127 RVA: 0x0000558D File Offset: 0x0000378D
		public bool IsEnemySideRetreating { get; private set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000080 RID: 128 RVA: 0x00005596 File Offset: 0x00003796
		// (set) Token: 0x06000081 RID: 129 RVA: 0x0000559E File Offset: 0x0000379E
		public bool CanCheckForEndCondition { get; private set; }

		// Token: 0x06000082 RID: 130 RVA: 0x000055A8 File Offset: 0x000037A8
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._checkDepletionOrRetreatingTimer = new BasicMissionTimer();
			this._missionSpawnLogic = base.Mission.GetMissionBehavior<IMissionAgentSpawnLogic>();
			this._navalShipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
			this._navalAgentsLogic = base.Mission.GetMissionBehavior<NavalAgentsLogic>();
			this._navalShipsLogic.MissionEndEvent += this.OnMissionEnd;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00005610 File Offset: 0x00003810
		public override void OnDeploymentFinished()
		{
			this._contestedIslandCheckTimer = MissionTime.Now;
			this._mainAgentIsDeadTimer = MissionTime.Now;
			this.CanCheckForEndCondition = true;
		}

		// Token: 0x06000084 RID: 132 RVA: 0x0000562F File Offset: 0x0000382F
		public override void OnEarlyAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if (base.Mission.IsDeploymentFinished && affectedAgent == Agent.Main)
			{
				this._mainAgentIsDeadTimer = MissionTime.Now;
			}
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00005651 File Offset: 0x00003851
		public override void OnAgentControllerSetToPlayer(Agent agent)
		{
			if (base.Mission.IsDeploymentFinished && agent.IsActive())
			{
				this._mainAgentIsDeadTimer = MissionTime.Now;
			}
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00005674 File Offset: 0x00003874
		public override void OnMissionTick(float dt)
		{
			if (!base.Mission.IsDeploymentFinished)
			{
				return;
			}
			if (base.Mission.IsMissionEnding)
			{
				if (this._notificationsDisabled)
				{
					this._scoreBoardOpenedOnceOnMissionEnd = true;
				}
				if (this._missionEndedMessageShown && !this._scoreBoardOpenedOnceOnMissionEnd)
				{
					if (this._checkDepletionOrRetreatingTimer.ElapsedTime > 7f)
					{
						this.CheckIsEnemySideRetreatingOrOneSideDepleted(true);
						this._checkDepletionOrRetreatingTimer.Reset();
						if (base.Mission.MissionResult != null && base.Mission.MissionResult.PlayerDefeated)
						{
							GameTexts.SetVariable("leave_key", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("Generic", 4), 1f));
							MBInformationManager.AddQuickInformation(GameTexts.FindText("str_battle_lost_press_tab_to_view_results", null), 0, null, null, "");
						}
						else if (base.Mission.MissionResult != null && base.Mission.MissionResult.PlayerVictory)
						{
							if (this._isEnemySideDepleted)
							{
								GameTexts.SetVariable("leave_key", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("Generic", 4), 1f));
								MBInformationManager.AddQuickInformation(GameTexts.FindText("str_battle_won_press_tab_to_view_results", null), 0, null, null, "");
							}
						}
						else
						{
							GameTexts.SetVariable("leave_key", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("Generic", 4), 1f));
							MBInformationManager.AddQuickInformation(GameTexts.FindText("str_battle_finished_press_tab_to_view_results", null), 0, null, null, "");
						}
					}
				}
				else if (this._checkDepletionOrRetreatingTimer.ElapsedTime > 3f && !this._scoreBoardOpenedOnceOnMissionEnd)
				{
					if (base.Mission.MissionResult != null && base.Mission.MissionResult.PlayerDefeated)
					{
						if (this._isPlayerSideDepleted)
						{
							MBInformationManager.AddQuickInformation(GameTexts.FindText("str_battle_lost", null), 0, null, null, "");
						}
						else if (this._isPlayerSideRetreating)
						{
							MBInformationManager.AddQuickInformation(GameTexts.FindText("str_friendlies_are_fleeing_you_lost", null), 0, null, null, "");
						}
					}
					else if (base.Mission.MissionResult != null && base.Mission.MissionResult.PlayerVictory)
					{
						if (this._isEnemySideDepleted)
						{
							MBInformationManager.AddQuickInformation(GameTexts.FindText("str_battle_won", null), 0, null, null, "");
						}
						else if (this.IsEnemySideRetreating)
						{
							MBInformationManager.AddQuickInformation(GameTexts.FindText("str_enemies_are_fleeing_you_won", null), 0, null, null, "");
						}
					}
					else
					{
						MBInformationManager.AddQuickInformation(GameTexts.FindText("str_battle_finished", null), 0, null, null, "");
					}
					this._missionEndedMessageShown = true;
					this._checkDepletionOrRetreatingTimer.Reset();
				}
				if (!this._victoryReactionsActivated)
				{
					AgentVictoryLogic missionBehavior = base.Mission.GetMissionBehavior<AgentVictoryLogic>();
					if (missionBehavior != null)
					{
						this.CheckIsEnemySideRetreatingOrOneSideDepleted(true);
						if (this._isEnemySideDepleted)
						{
							missionBehavior.SetTimersOfVictoryReactionsOnBattleEnd(base.Mission.PlayerTeam.Side);
							this._victoryReactionsActivated = true;
							return;
						}
						if (this._isPlayerSideDepleted)
						{
							missionBehavior.SetTimersOfVictoryReactionsOnBattleEnd(base.Mission.PlayerEnemyTeam.Side);
							this._victoryReactionsActivated = true;
							return;
						}
						if (this.IsEnemySideRetreating && !this._victoryReactionsActivatedForRetreating)
						{
							missionBehavior.SetTimersOfVictoryReactionsOnRetreat(base.Mission.PlayerTeam.Side);
							this._victoryReactionsActivatedForRetreating = true;
							return;
						}
						if (this._isPlayerSideRetreating && !this._victoryReactionsActivatedForRetreating)
						{
							missionBehavior.SetTimersOfVictoryReactionsOnRetreat(base.Mission.PlayerEnemyTeam.Side);
							this._victoryReactionsActivatedForRetreating = true;
							return;
						}
					}
				}
			}
			else if (this._checkDepletionOrRetreatingTimer.ElapsedTime > 1f)
			{
				this.CheckIsEnemySideRetreatingOrOneSideDepleted(false);
				if (this._isInContestedIslandsCheckPhase)
				{
					this._contestedIslandsCheckDuration = 5f;
				}
				else
				{
					this._contestedIslandsCheckDuration = 20f;
				}
				this._checkDepletionOrRetreatingTimer.Reset();
			}
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00005A10 File Offset: 0x00003C10
		public override bool MissionEnded(ref MissionResult missionResult)
		{
			bool flag = false;
			if (this.IsEnemySideRetreating || this._isEnemySideDepleted)
			{
				missionResult = MissionResult.CreateSuccessful(base.Mission, this.IsEnemySideRetreating);
				flag = true;
			}
			else if (this._isPlayerSideRetreating || this._isPlayerSideDepleted)
			{
				missionResult = MissionResult.CreateDefeated(base.Mission);
				flag = true;
			}
			if (flag)
			{
				this._missionSpawnLogic.StopSpawner(1);
				this._missionSpawnLogic.StopSpawner(0);
			}
			return flag;
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00005A81 File Offset: 0x00003C81
		public override void OnMissionStateFinalized()
		{
			this._navalShipsLogic.MissionEndEvent -= this.OnMissionEnd;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00005A9C File Offset: 0x00003C9C
		private void OnMissionEnd()
		{
			if (this.IsEnemySideRetreating)
			{
				foreach (Agent agent in base.Mission.PlayerEnemyTeam.ActiveAgents)
				{
					IAgentOriginBase origin = agent.Origin;
					if (origin != null)
					{
						origin.SetRouted(true);
					}
				}
				MBList<MissionShip> mblist = new MBList<MissionShip>();
				this._navalShipsLogic.FillTeamShips(2, mblist);
				MBList<IAgentOriginBase> mblist2 = new MBList<IAgentOriginBase>();
				foreach (MissionShip missionShip in mblist)
				{
					this._navalAgentsLogic.FillReservedTroopsOfShip(missionShip, mblist2);
				}
				foreach (IAgentOriginBase agentOriginBase in mblist2)
				{
					agentOriginBase.SetRouted(true);
				}
			}
			if (Campaign.Current != null && PlayerEncounter.Current != null)
			{
				MBReadOnlyList<MapEventParty> mbreadOnlyList = new MBReadOnlyList<MapEventParty>();
				if (this.IsEnemySideRetreating || this._isEnemySideDepleted)
				{
					mbreadOnlyList = PlayerEncounter.Battle.PartiesOnSide(Extensions.GetOppositeSide(PlayerEncounter.Battle.PlayerSide));
				}
				else if (this._isPlayerSideRetreating || this._isPlayerSideDepleted)
				{
					mbreadOnlyList = PlayerEncounter.Battle.PartiesOnSide(PlayerEncounter.Battle.PlayerSide);
				}
				foreach (MissionShip missionShip2 in this._navalShipsLogic.AllShips)
				{
					Ship shipToCapture;
					if ((shipToCapture = missionShip2.ShipOrigin as Ship) != null && LinQuick.ContainsQ<MapEventParty>(mbreadOnlyList, (MapEventParty x) => x.Party == shipToCapture.Owner))
					{
						PlayerEncounter.Current.CapturedShipsInEncounter.Add(shipToCapture);
					}
				}
			}
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00005CA4 File Offset: 0x00003EA4
		public NavalBattleEndLogic.ExitResult TryExit()
		{
			if (GameNetwork.IsClientOrReplay)
			{
				return NavalBattleEndLogic.ExitResult.False;
			}
			Agent mainAgent = base.Mission.MainAgent;
			if ((mainAgent != null && mainAgent.IsActive() && base.Mission.IsPlayerCloseToAnEnemy(5f)) || (!base.Mission.MissionEnded && (this.PlayerVictory || this.EnemyVictory)))
			{
				return NavalBattleEndLogic.ExitResult.False;
			}
			if (!base.Mission.MissionEnded && !this.IsEnemySideRetreating)
			{
				return NavalBattleEndLogic.ExitResult.NeedsPlayerConfirmation;
			}
			base.Mission.EndMission();
			return NavalBattleEndLogic.ExitResult.True;
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00005D27 File Offset: 0x00003F27
		public void SetNotificationDisabled(bool value)
		{
			this._notificationsDisabled = value;
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00005D30 File Offset: 0x00003F30
		private void CheckIsEnemySideRetreatingOrOneSideDepleted(bool forceCheckContestedIslands = false)
		{
			if (this.CanCheckForEndCondition)
			{
				BattleSideEnum side = base.Mission.PlayerTeam.Side;
				BattleSideEnum oppositeSide = Extensions.GetOppositeSide(side);
				if (this._missionSpawnLogic.IsSideDepleted(side))
				{
					this._isPlayerSideDepleted = true;
				}
				if (this._missionSpawnLogic.IsSideDepleted(oppositeSide))
				{
					this._isEnemySideDepleted = true;
				}
				if (!this._isEnemySideDepleted && !this._isPlayerSideDepleted)
				{
					bool flag;
					bool flag2;
					if (this.AreAnySideShipsOutOfAction(side, oppositeSide, out flag, out flag2))
					{
						this._isInContestedIslandsCheckPhase = this._contestedIslandCheckTimer.ElapsedSeconds > this._contestedIslandsCheckDuration;
						if (forceCheckContestedIslands || this._isInContestedIslandsCheckPhase)
						{
							if (!this.HasAnyContestedIslands(side, oppositeSide))
							{
								Agent main = Agent.Main;
								bool flag3 = (main == null || !main.IsActive()) && this._mainAgentIsDeadTimer.ElapsedSeconds > 20f;
								if (flag && flag3)
								{
									this._isPlayerSideDepleted = true;
								}
								if (flag2)
								{
									this._isEnemySideDepleted = true;
								}
							}
							this._contestedIslandCheckTimer = MissionTime.Now;
						}
					}
					else
					{
						this._isInContestedIslandsCheckPhase = false;
						this._contestedIslandCheckTimer = MissionTime.Now;
					}
					if (!this._isEnemySideDepleted && !this._isPlayerSideDepleted)
					{
						if (base.Mission.MainAgent != null && base.Mission.MainAgent.IsPlayerControlled && base.Mission.MainAgent.IsActive())
						{
							this._playerSideNotYetRetreatingTime = MissionTime.Now;
						}
						else
						{
							bool flag4 = true;
							foreach (MissionShip missionShip in this._navalShipsLogic.AllShips)
							{
								if (missionShip.Team != null && missionShip.Team.Side == side && !missionShip.IsRetreating)
								{
									flag4 = false;
									break;
								}
							}
							if (!flag4)
							{
								this._playerSideNotYetRetreatingTime = MissionTime.Now;
							}
						}
						if (this._playerSideNotYetRetreatingTime.ElapsedSeconds > 5f)
						{
							this._isPlayerSideRetreating = true;
						}
						bool flag5 = true;
						foreach (MissionShip missionShip2 in this._navalShipsLogic.AllShips)
						{
							if (missionShip2.Team != null && missionShip2.Team.Side == oppositeSide && !missionShip2.IsRetreating)
							{
								flag5 = false;
								break;
							}
						}
						if (!flag5)
						{
							this._enemySideNotYetRetreatingTime = MissionTime.Now;
						}
						if (this._enemySideNotYetRetreatingTime.ElapsedSeconds > 5f)
						{
							this.IsEnemySideRetreating = true;
						}
					}
				}
			}
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00005FC4 File Offset: 0x000041C4
		private bool AreAnySideShipsOutOfAction(BattleSideEnum playerSide, BattleSideEnum enemySide, out bool playerShipsOutOfAction, out bool enemyShipsOutOfAction)
		{
			playerShipsOutOfAction = false;
			enemyShipsOutOfAction = false;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			foreach (MissionShip missionShip in this._navalShipsLogic.AllShips)
			{
				if (missionShip.Team != null)
				{
					if (missionShip.Team.Side == playerSide)
					{
						num++;
						bool flag = false;
						if (missionShip.IsSunk)
						{
							flag = true;
						}
						else if (this._navalAgentsLogic.GetTotalTroopCountOfShip(missionShip, true) <= 3)
						{
							flag = true;
						}
						if (flag)
						{
							num3++;
						}
					}
					else if (missionShip.Team.Side == enemySide)
					{
						num2++;
						bool flag2 = false;
						if (missionShip.IsSunk)
						{
							flag2 = true;
						}
						else if (this._navalAgentsLogic.GetTotalTroopCountOfShip(missionShip, true) <= 3)
						{
							flag2 = true;
						}
						if (flag2)
						{
							num4++;
						}
					}
				}
			}
			if (num > 0)
			{
				playerShipsOutOfAction = num3 == num;
			}
			if (num2 > 0)
			{
				enemyShipsOutOfAction = num4 == num2;
			}
			return playerShipsOutOfAction | enemyShipsOutOfAction;
		}

		// Token: 0x0600008E RID: 142 RVA: 0x000060D8 File Offset: 0x000042D8
		private bool HasAnyContestedIslands(BattleSideEnum playerSide, BattleSideEnum enemySide)
		{
			ulong num = 0UL;
			ulong num2 = 0UL;
			foreach (Agent agent in base.Mission.AllAgents)
			{
				if (agent.IsActive() && agent.IsHuman && agent.Team != null)
				{
					AgentNavalComponent component = agent.GetComponent<AgentNavalComponent>();
					if (component != null)
					{
						ulong steppedCombinedShipIsland = component.GetSteppedCombinedShipIsland();
						if (steppedCombinedShipIsland != 0UL)
						{
							BattleSideEnum side = agent.Team.Side;
							if (side == playerSide)
							{
								num |= steppedCombinedShipIsland;
							}
							else if (side == enemySide)
							{
								num2 |= steppedCombinedShipIsland;
							}
							if ((num & num2) != 0UL)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00006194 File Offset: 0x00004394
		public override void OnMissionResultReady(MissionResult missionResult)
		{
			foreach (Agent agent in Mission.Current.Agents)
			{
				agent.SetAgentFlags(agent.GetAgentFlags() & -9);
			}
		}

		// Token: 0x04000044 RID: 68
		public const float DefaultContestedIslandsCheckDuration = 20f;

		// Token: 0x04000045 RID: 69
		public const float RetreatCheckDuration = 5f;

		// Token: 0x04000046 RID: 70
		public const float MainAgentConsideredDeadDuration = 20f;

		// Token: 0x04000047 RID: 71
		public const int MinTroopCountForOutOfActionCheck = 3;

		// Token: 0x0400004A RID: 74
		private IMissionAgentSpawnLogic _missionSpawnLogic;

		// Token: 0x0400004B RID: 75
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x0400004C RID: 76
		private NavalAgentsLogic _navalAgentsLogic;

		// Token: 0x0400004D RID: 77
		private bool _notificationsDisabled;

		// Token: 0x0400004E RID: 78
		private MissionTime _enemySideNotYetRetreatingTime;

		// Token: 0x0400004F RID: 79
		private MissionTime _playerSideNotYetRetreatingTime;

		// Token: 0x04000050 RID: 80
		private MissionTime _contestedIslandCheckTimer;

		// Token: 0x04000051 RID: 81
		private MissionTime _mainAgentIsDeadTimer;

		// Token: 0x04000052 RID: 82
		private float _contestedIslandsCheckDuration = 20f;

		// Token: 0x04000053 RID: 83
		private bool _isInContestedIslandsCheckPhase;

		// Token: 0x04000054 RID: 84
		private BasicMissionTimer _checkDepletionOrRetreatingTimer;

		// Token: 0x04000055 RID: 85
		private bool _isPlayerSideRetreating;

		// Token: 0x04000056 RID: 86
		private bool _isEnemySideDepleted;

		// Token: 0x04000057 RID: 87
		private bool _isPlayerSideDepleted;

		// Token: 0x04000058 RID: 88
		private bool _missionEndedMessageShown;

		// Token: 0x04000059 RID: 89
		private bool _victoryReactionsActivated;

		// Token: 0x0400005A RID: 90
		private bool _victoryReactionsActivatedForRetreating;

		// Token: 0x0400005B RID: 91
		private bool _scoreBoardOpenedOnceOnMissionEnd;

		// Token: 0x0200017F RID: 383
		public enum ExitResult
		{
			// Token: 0x04000C22 RID: 3106
			False,
			// Token: 0x04000C23 RID: 3107
			NeedsPlayerConfirmation,
			// Token: 0x04000C24 RID: 3108
			True
		}
	}
}
