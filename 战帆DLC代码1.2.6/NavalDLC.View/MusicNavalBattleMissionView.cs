using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using psai.net;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace NavalDLC.View
{
	// Token: 0x02000003 RID: 3
	internal class MusicNavalBattleMissionView : MissionView, IMusicHandler
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600000E RID: 14 RVA: 0x00002103 File Offset: 0x00000303
		bool IMusicHandler.IsPausable
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000F RID: 15 RVA: 0x00002106 File Offset: 0x00000306
		private MatrixFrame _listenerGlobalFrame
		{
			get
			{
				return SoundManager.GetListenerFrame();
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002118 File Offset: 0x00000318
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this._navalAgentsLogic = Mission.Current.GetMissionBehavior<NavalAgentsLogic>();
			this._navalShipsLogic.ShipSunkEvent += this.OnShipSunk;
			this._navalShipsLogic.ShipRammingEvent += this.OnShipRamming;
			this._navalShipsLogic.ShipHookThrowEvent += this.OnShipHookThrow;
			this._waterStrengthIntensityMultiplier = 1f + MathF.Max(0f, (Mission.Current.Scene.GetWaterStrength() - 3f) * 0.07f);
			this._mainAgentBaseHealth = 0f;
			MBMusicManager.Current.DeactivateCurrentMode();
			MBMusicManager.Current.ActivateBattleMode();
			MBMusicManager.Current.OnBattleMusicHandlerInit(this);
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000021EC File Offset: 0x000003EC
		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
			this._navalShipsLogic.ShipSunkEvent -= this.OnShipSunk;
			this._navalShipsLogic.ShipRammingEvent -= this.OnShipRamming;
			this._navalShipsLogic.ShipHookThrowEvent -= this.OnShipHookThrow;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002244 File Offset: 0x00000444
		public override void OnMissionScreenFinalize()
		{
			MBMusicManager.Current.DeactivateBattleMode();
			MBMusicManager.Current.OnBattleMusicHandlerFinalize();
			base.Mission.PlayerTeam.PlayerOrderController.OnOrderIssued -= new OnOrderIssuedDelegate(this.PlayerOrderControllerOnOrderIssued);
		}

		// Token: 0x06000014 RID: 20 RVA: 0x0000227B File Offset: 0x0000047B
		public override void AfterStart()
		{
			this._nextPossibleTimeToIncreaseIntensityForChargeOrder = MissionTime.Now;
			base.Mission.PlayerTeam.PlayerOrderController.OnOrderIssued += new OnOrderIssuedDelegate(this.PlayerOrderControllerOnOrderIssued);
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000022AC File Offset: 0x000004AC
		private void PlayerOrderControllerOnOrderIssued(OrderType orderType, IEnumerable<Formation> appliedFormations, OrderController orderController, object[] parameters)
		{
			if ((orderType == 4 || orderType == 5) && this._nextPossibleTimeToIncreaseIntensityForChargeOrder.IsPast)
			{
				float currentIntensity = PsaiCore.Instance.GetCurrentIntensity();
				float num = currentIntensity * MusicParameters.PlayerChargeEffectMultiplierOnIntensity - currentIntensity;
				MBMusicManager.Current.ChangeCurrentThemeIntensity(num * this._waterStrengthIntensityMultiplier);
				this._nextPossibleTimeToIncreaseIntensityForChargeOrder = MissionTime.Now + MissionTime.Seconds(60f);
			}
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002310 File Offset: 0x00000510
		private void CheckIntensityFall()
		{
			PsaiInfo psaiInfo = PsaiCore.Instance.GetPsaiInfo();
			if (psaiInfo.effectiveThemeId >= 0)
			{
				if (float.IsNaN(psaiInfo.currentIntensity))
				{
					MBMusicManager.Current.ChangeCurrentThemeIntensity(MusicParameters.MinIntensity);
					return;
				}
				if (psaiInfo.currentIntensity < MusicParameters.MinIntensity)
				{
					MBMusicManager.Current.ChangeCurrentThemeIntensity(MusicParameters.MinIntensity - psaiInfo.currentIntensity);
				}
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002378 File Offset: 0x00000578
		public override void OnAgentRemoved(Agent affectedAgent, Agent affectorAgent, AgentState agentState, KillingBlow blow)
		{
			if (this._battleState != MusicNavalBattleMissionView.BattleState.Starting)
			{
				bool flag = affectedAgent.IsMine || (affectedAgent.RiderAgent != null && affectedAgent.RiderAgent.IsMine);
				Team team = affectedAgent.Team;
				BattleSideEnum battleSideEnum = ((team != null) ? team.Side : (-1));
				bool flag2;
				if (!flag)
				{
					if (battleSideEnum != -1)
					{
						Team playerTeam = Mission.Current.PlayerTeam;
						flag2 = ((playerTeam != null) ? playerTeam.Side : (-1)) == battleSideEnum;
					}
					else
					{
						flag2 = false;
					}
				}
				else
				{
					flag2 = true;
				}
				bool flag3 = flag2;
				if ((affectedAgent.IsHuman && affectedAgent.State != 2) || flag)
				{
					float num = (flag3 ? MusicParameters.FriendlyTroopDeadEffectOnIntensity : MusicParameters.EnemyTroopDeadEffectOnIntensity);
					if (flag)
					{
						num *= MusicParameters.PlayerTroopDeadEffectMultiplierOnIntensity;
					}
					MBMusicManager.Current.ChangeCurrentThemeIntensity(num * this._waterStrengthIntensityMultiplier);
				}
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002434 File Offset: 0x00000634
		public void OnShipSunk(MissionShip ship)
		{
			float num = this._listenerGlobalFrame.origin.DistanceSquared(ship.GameEntity.GlobalPosition);
			if (num < 62500f)
			{
				float num2 = MathF.Max(0.5f - MathF.Sqrt(num) * 0.002f, 0.1f);
				MBMusicManager.Current.ChangeCurrentThemeIntensity(num2 * this._waterStrengthIntensityMultiplier);
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x0000249C File Offset: 0x0000069C
		public void OnShipRamming(MissionShip rammingShip, MissionShip rammedShip, float damagePercent, bool isFirstImpact, CapsuleData capsuleData, int ramQuality)
		{
			float num = this._listenerGlobalFrame.origin.DistanceSquared(rammingShip.GameEntity.GetBodyWorldTransform().origin);
			if (num < 10000f)
			{
				float num2 = (isFirstImpact ? 0.2f : 0f);
				float num3 = MathF.Max(2f * damagePercent * (1f - MathF.Sqrt(num) * 0.01f), num2);
				MBMusicManager.Current.ChangeCurrentThemeIntensity(num3 * this._waterStrengthIntensityMultiplier);
			}
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002520 File Offset: 0x00000720
		public void OnShipHookThrow(MissionShip hookingShip, MissionShip hookedShip)
		{
			float num = this._listenerGlobalFrame.origin.DistanceSquared(hookingShip.GameEntity.GlobalPosition);
			if (num < 10000f)
			{
				float num2 = 0.05f - MathF.Sqrt(num) * 0.0005f;
				MBMusicManager.Current.ChangeCurrentThemeIntensity(num2 * this._waterStrengthIntensityMultiplier);
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x0000257C File Offset: 0x0000077C
		private void CheckForStarting()
		{
			if (this._startingTroopCounts == null || this._startingTroopCounts.Sum() == 0)
			{
				this._startingTroopCounts = new int[]
				{
					this._navalAgentsLogic.GetNumberOfSpawnedAgents(0),
					this._navalAgentsLogic.GetNumberOfSpawnedAgents(1)
				};
			}
			float num = (float)this._startingTroopCounts.Sum() / 500f;
			float num2 = MathF.Max(MusicParameters.DefaultStartIntensity, num * 0.8f) + (MBRandom.RandomFloat - 0.5f) * (MusicParameters.RandomEffectMultiplierOnStartIntensity * 2f);
			MusicNavalBattleMissionView.NavalBattleThemes navalBattleTheme = this.GetNavalBattleTheme(base.Mission.MusicCulture);
			MBMusicManager.Current.StartTheme(navalBattleTheme, num2, false);
			this._battleState = MusicNavalBattleMissionView.BattleState.Started;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x0000262C File Offset: 0x0000082C
		private MusicNavalBattleMissionView.NavalBattleThemes GetNavalBattleTheme(BasicCultureObject culture)
		{
			if (culture.StringId == "sturgia" || culture.StringId == "nord" || culture.StringId == "battania")
			{
				if ((double)MBRandom.NondeterministicRandomFloat <= 0.5)
				{
					return MusicNavalBattleMissionView.NavalBattleThemes.VikingSeaBattle2;
				}
				return MusicNavalBattleMissionView.NavalBattleThemes.VikingSeaBattle1;
			}
			else
			{
				if ((double)MBRandom.NondeterministicRandomFloat <= 0.5)
				{
					return MusicNavalBattleMissionView.NavalBattleThemes.MediterraneanSeaBattle2;
				}
				return MusicNavalBattleMissionView.NavalBattleThemes.MediterraneanSeaBattle1;
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000026A8 File Offset: 0x000008A8
		private void CheckForEnding()
		{
			if (Mission.Current.IsMissionEnding)
			{
				if (Mission.Current.MissionResult != null)
				{
					MusicTheme battleEndTheme = MBMusicManager.Current.GetBattleEndTheme(base.Mission.MusicCulture, Mission.Current.MissionResult.PlayerVictory);
					MBMusicManager.Current.StartTheme(battleEndTheme, PsaiCore.Instance.GetPsaiInfo().currentIntensity, true);
					this._battleState = MusicNavalBattleMissionView.BattleState.Ending;
					return;
				}
				MBMusicManager.Current.StartTheme(26, PsaiCore.Instance.GetPsaiInfo().currentIntensity, true);
				this._battleState = MusicNavalBattleMissionView.BattleState.Ending;
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00002740 File Offset: 0x00000940
		void IMusicHandler.OnUpdated(float dt)
		{
			if (this._battleState == MusicNavalBattleMissionView.BattleState.Starting)
			{
				if (base.Mission.MusicCulture == null)
				{
					KeyValuePair<BasicCultureObject, int> keyValuePair = new KeyValuePair<BasicCultureObject, int>(null, -1);
					Dictionary<BasicCultureObject, int> dictionary = new Dictionary<BasicCultureObject, int>();
					foreach (Team team in base.Mission.Teams)
					{
						foreach (Agent agent in team.ActiveAgents)
						{
							BasicCultureObject culture = agent.Character.Culture;
							if (culture != null && culture.IsMainCulture)
							{
								if (!dictionary.ContainsKey(agent.Character.Culture))
								{
									dictionary.Add(agent.Character.Culture, 0);
								}
								Dictionary<BasicCultureObject, int> dictionary2 = dictionary;
								BasicCultureObject culture2 = agent.Character.Culture;
								int num = dictionary2[culture2];
								dictionary2[culture2] = num + 1;
								if (dictionary[agent.Character.Culture] > keyValuePair.Value)
								{
									keyValuePair = new KeyValuePair<BasicCultureObject, int>(agent.Character.Culture, dictionary[agent.Character.Culture]);
								}
							}
						}
					}
					if (keyValuePair.Key != null)
					{
						base.Mission.MusicCulture = keyValuePair.Key;
					}
					else
					{
						base.Mission.MusicCulture = Game.Current.PlayerTroop.Culture;
					}
				}
				if (base.Mission.MusicCulture != null)
				{
					this.CheckForStarting();
				}
			}
			if (this._battleState == MusicNavalBattleMissionView.BattleState.Started && Mission.Current.MainAgent != null && Mission.Current.MainAgent.IsActive())
			{
				float num2 = 0f;
				if (this._mainAgentBaseHealth <= 0.01f)
				{
					this._mainAgentBaseHealth = Mission.Current.MainAgent.BaseHealthLimit;
				}
				float num3 = 1f - Mission.Current.MainAgent.Health / this._mainAgentBaseHealth;
				this._mainAgentBaseHealth = Mission.Current.MainAgent.Health;
				num2 += num3;
				float lengthSquared = (Mission.Current.MainAgent.GetAverageRealGlobalVelocity() - Mission.Current.MainAgent.AverageVelocity).LengthSquared;
				num2 += ((lengthSquared > 25f) ? (dt * 0.01f) : 0f);
				if (num2 > 0f)
				{
					MBMusicManager.Current.ChangeCurrentThemeIntensity(num2 * this._waterStrengthIntensityMultiplier);
				}
			}
			if (this._battleState == MusicNavalBattleMissionView.BattleState.Started || this._battleState == MusicNavalBattleMissionView.BattleState.TurnedOneSide)
			{
				this.CheckForEnding();
			}
			this.CheckIntensityFall();
		}

		// Token: 0x04000001 RID: 1
		private const float ChargeOrderIntensityIncreaseCooldownInSeconds = 60f;

		// Token: 0x04000002 RID: 2
		private const float BattleSizeEffectOnStartIntensity = 0.8f;

		// Token: 0x04000003 RID: 3
		private const string CultureSturgia = "sturgia";

		// Token: 0x04000004 RID: 4
		private const string CultureBattania = "battania";

		// Token: 0x04000005 RID: 5
		private const string CultureNord = "nord";

		// Token: 0x04000006 RID: 6
		private MusicNavalBattleMissionView.BattleState _battleState;

		// Token: 0x04000007 RID: 7
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x04000008 RID: 8
		private NavalAgentsLogic _navalAgentsLogic;

		// Token: 0x04000009 RID: 9
		private float _waterStrengthIntensityMultiplier;

		// Token: 0x0400000A RID: 10
		private float _mainAgentBaseHealth;

		// Token: 0x0400000B RID: 11
		private int[] _startingTroopCounts;

		// Token: 0x0400000C RID: 12
		private MissionTime _nextPossibleTimeToIncreaseIntensityForChargeOrder;

		// Token: 0x0200003E RID: 62
		private enum BattleState
		{
			// Token: 0x040000D2 RID: 210
			Starting,
			// Token: 0x040000D3 RID: 211
			Started,
			// Token: 0x040000D4 RID: 212
			TurnedOneSide,
			// Token: 0x040000D5 RID: 213
			Ending
		}

		// Token: 0x0200003F RID: 63
		private enum NavalBattleThemes
		{
			// Token: 0x040000D7 RID: 215
			VikingSeaBattle1 = 10241,
			// Token: 0x040000D8 RID: 216
			VikingSeaBattle2,
			// Token: 0x040000D9 RID: 217
			MediterraneanSeaBattle1,
			// Token: 0x040000DA RID: 218
			Maintheme,
			// Token: 0x040000DB RID: 219
			MediterraneanSeaBattle2
		}
	}
}
