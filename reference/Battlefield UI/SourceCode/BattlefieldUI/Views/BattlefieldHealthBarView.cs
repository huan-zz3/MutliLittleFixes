using System;
using System.Collections.Generic;
using BattlefieldUI.Settings;
using BattlefieldUI.UI;
using BattlefieldUI.ViewModels;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.ScreenSystem;

namespace BattlefieldUI.Views
{
	// Token: 0x02000005 RID: 5
	[DefaultView]
	public sealed class BattlefieldHealthBarView : MissionView
	{
		// Token: 0x06000006 RID: 6 RVA: 0x00002118 File Offset: 0x00000318
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			if (GameNetwork.IsSessionActive)
			{
				Debug.Print("[BattlefieldUI] Mission view skipped for multiplayer session.", 0, 12, 17592186044416UL);
				return;
			}
			bool flag = false;
			try
			{
				this._dataSource = new BattlefieldUIVM();
				this._layer = new GauntletLayer("BattlefieldUI", 4, false);
				this._layer.LoadMovie("BattlefieldUI", this._dataSource);
				base.MissionScreen.AddLayer(this._layer);
				flag = true;
				this._refreshTimer = 0f;
				Debug.Print("[BattlefieldUI] Mission view initialized and movie loaded.", 0, 12, 17592186044416UL);
			}
			catch (Exception ex)
			{
				if (flag && this._layer != null && base.MissionScreen != null)
				{
					base.MissionScreen.RemoveLayer(this._layer);
				}
				this._layer = null;
				if (this._dataSource != null)
				{
					this._dataSource.OnFinalize();
					this._dataSource = null;
				}
				string text = "[BattlefieldUI] Mission view initialization failed: ";
				Exception ex2 = ex;
				Debug.Print(text + ((ex2 != null) ? ex2.ToString() : null), 0, 12, 17592186044416UL);
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002238 File Offset: 0x00000438
		public override void OnMissionScreenFinalize()
		{
			if (this._layer != null && base.MissionScreen != null)
			{
				base.MissionScreen.RemoveLayer(this._layer);
			}
			this._layer = null;
			if (this._dataSource != null)
			{
				this._dataSource.OnFinalize();
				this._dataSource = null;
			}
			this._candidates.Clear();
			this._visibleAgents.Clear();
			this._damageNumbers.Clear();
			base.OnMissionScreenFinalize();
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000022B0 File Offset: 0x000004B0
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (this._dataSource == null || base.MissionScreen == null || base.MissionScreen.CombatCamera == null)
			{
				return;
			}
			BattlefieldUISettings battlefieldUISettings = BattlefieldUISettings.Current;
			if (this._photoModeActive || !BattlefieldHealthBarView.IsSupportedMissionMode(base.Mission.Mode))
			{
				this.DeactivateDamageNumbers();
				this._dataSource.HideAll();
				return;
			}
			this._refreshTimer -= dt;
			if (this._refreshTimer <= 0f)
			{
				this.RefreshVisibleAgents(battlefieldUISettings);
				this._refreshTimer = Math.Max(0.05f, battlefieldUISettings.RefreshInterval);
			}
			int num = this.UpdateVisibleItems(battlefieldUISettings);
			int num2 = this.UpdateDamageNumbers(dt, battlefieldUISettings);
			this._dataSource.IsEnabled = num > 0 || num2 > 0;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002379 File Offset: 0x00000579
		protected override void OnSuspendView()
		{
			base.OnSuspendView();
			if (this._layer != null)
			{
				ScreenManager.SetSuspendLayer(this._layer, true);
			}
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002395 File Offset: 0x00000595
		protected override void OnResumeView()
		{
			base.OnResumeView();
			if (this._layer != null)
			{
				ScreenManager.SetSuspendLayer(this._layer, false);
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000023B1 File Offset: 0x000005B1
		public override void OnPhotoModeActivated()
		{
			base.OnPhotoModeActivated();
			this._photoModeActive = true;
			this.DeactivateDamageNumbers();
			if (this._layer != null)
			{
				this._layer.UIContext.ContextAlpha = 0f;
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000023E3 File Offset: 0x000005E3
		public override void OnPhotoModeDeactivated()
		{
			base.OnPhotoModeDeactivated();
			this._photoModeActive = false;
			if (this._layer != null)
			{
				this._layer.UIContext.ContextAlpha = 1f;
			}
			this._refreshTimer = 0f;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x0000241A File Offset: 0x0000061A
		public override void OnClearScene()
		{
			this._candidates.Clear();
			this._visibleAgents.Clear();
			this.DeactivateDamageNumbers();
			if (this._dataSource != null)
			{
				this._dataSource.HideAll();
			}
			base.OnClearScene();
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002454 File Offset: 0x00000654
		public override void OnScoreHit(Agent affectedAgent, Agent affectorAgent, WeaponComponentData attackerWeapon, bool isBlocked, bool isSiegeEngineHit, in Blow blow, in AttackCollisionData collisionData, float damagedHp, float hitDistance, float shotDifficulty)
		{
			base.OnScoreHit(affectedAgent, affectorAgent, attackerWeapon, isBlocked, isSiegeEngineHit, ref blow, ref collisionData, damagedHp, hitDistance, shotDifficulty);
			if (this._dataSource == null || base.MissionScreen == null || base.MissionScreen.CombatCamera == null || this._photoModeActive || affectedAgent == null || !affectedAgent.IsHuman || blow.InflictedDamage <= 0 || !BattlefieldHealthBarView.IsSupportedMissionMode(base.Mission.Mode))
			{
				return;
			}
			BattlefieldUISettings battlefieldUISettings = BattlefieldUISettings.Current;
			if (!battlefieldUISettings.ShowDamageNumbers)
			{
				return;
			}
			bool flag;
			if (!this.ShouldDisplayDamageFor(affectedAgent, battlefieldUISettings, out flag))
			{
				return;
			}
			Vec3 vec = affectedAgent.GetEyeGlobalPosition() + new Vec3(0f, 0f, 0.12f, -1f);
			float num = Math.Max(1f, (float)battlefieldUISettings.DamageNumberMaximumDistance);
			if ((vec - base.MissionScreen.CombatCamera.Position).LengthSquared > num * num)
			{
				return;
			}
			int displayedDamage = BattlefieldUIDisplayRules.GetDisplayedDamage(blow.InflictedDamage);
			if (displayedDamage > 0)
			{
				this.AddOrMergeDamageNumber(affectedAgent, vec, displayedDamage, flag, battlefieldUISettings);
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002564 File Offset: 0x00000764
		private void RefreshVisibleAgents(BattlefieldUISettings settings)
		{
			this._candidates.Clear();
			this._visibleAgents.Clear();
			if (base.Mission == null || base.Mission.Agents == null)
			{
				return;
			}
			int num = BattlefieldHealthBarView.ResolveDisplayMode(settings);
			float num2 = (float)(settings.MaximumDistance * settings.MaximumDistance);
			this._sortOrigin = base.MissionScreen.CombatCamera.Position;
			foreach (Agent agent in base.Mission.Agents)
			{
				bool flag;
				if (this.IsEligibleAgent(agent, settings, num, out flag) && (agent.GetEyeGlobalPosition() - this._sortOrigin).LengthSquared <= num2)
				{
					this._candidates.Add(agent);
				}
			}
			this._candidates.Sort(new Comparison<Agent>(this.CompareCandidateDistance));
			int num3 = Math.Max(1, settings.MaximumVisibleBars);
			int num4 = 0;
			while (num4 < this._candidates.Count && this._visibleAgents.Count < num3)
			{
				if (this.IsInFrontOfCamera(this._candidates[num4], settings.HeightOffset))
				{
					this._visibleAgents.Add(this._candidates[num4]);
				}
				num4++;
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000026C8 File Offset: 0x000008C8
		private int UpdateVisibleItems(BattlefieldUISettings settings)
		{
			string text = BattlefieldUIColor.ApplyOpacity(settings.FriendlyColor, "#49B86EFF", settings.OpacityPercent);
			string text2 = BattlefieldUIColor.ApplyOpacity(settings.EnemyColor, "#D9534FFF", settings.OpacityPercent);
			string text3 = BattlefieldUIColor.ApplyOpacity(settings.BackgroundColor, "#181818CC", settings.OpacityPercent);
			int num = BattlefieldHealthBarView.ResolveDisplayMode(settings);
			int num2 = 0;
			for (int i = 0; i < this._visibleAgents.Count; i++)
			{
				Agent agent = this._visibleAgents[i];
				bool flag;
				if (this.IsEligibleAgent(agent, settings, num, out flag))
				{
					Vec3 vec = agent.GetEyeGlobalPosition() + new Vec3(0f, 0f, settings.HeightOffset, -1f);
					float length = (vec - base.MissionScreen.CombatCamera.Position).Length;
					float num3 = BattlefieldUIDisplayRules.CalculateAlpha(length, (float)settings.FadeStartDistance, (float)settings.MaximumDistance);
					if (num3 > 0f)
					{
						float num4 = -10000f;
						float num5 = -10000f;
						float num6 = -1f;
						MBWindowManager.WorldToScreen(base.MissionScreen.CombatCamera, vec, ref num4, ref num5, ref num6);
						if (num6 > 0f && MathF.IsValidValue(num4) && MathF.IsValidValue(num5))
						{
							BattlefieldHealthBarItemVM orCreateItem = this._dataSource.GetOrCreateItem(num2++);
							bool flag2 = BattlefieldHealthBarView.ShouldShowHealthBar(agent, num);
							float num7 = BattlefieldUIDisplayRules.CalculateMarkerScale(length, (float)settings.MaximumDistance);
							int num8 = BattlefieldHealthBarView.ResolveCornerStyle(settings);
							orCreateItem.ScreenPositionX = num4 - 0.5f;
							orCreateItem.ScreenPositionY = num5 - 18f;
							orCreateItem.HealthRatio = MathF.Clamp(agent.Health / agent.HealthLimit, 0f, 1f);
							orCreateItem.Alpha = num3;
							orCreateItem.BarColor = (flag ? text2 : text);
							orCreateItem.BackgroundColor = text3;
							orCreateItem.BarWidth = Math.Max(1f, (float)settings.HealthBarWidth * num7);
							orCreateItem.BarHeight = Math.Max(3f, (float)settings.HealthBarHeight * num7);
							orCreateItem.BarPositionX = (float)settings.HealthBarOffsetX;
							orCreateItem.BarPositionY = (float)settings.HealthBarOffsetY;
							orCreateItem.ShowSquareBar = flag2 && num8 == 0;
							orCreateItem.ShowSmallRoundedBar = flag2 && num8 == 1;
							orCreateItem.ShowLargeRoundedBar = flag2 && num8 == 2;
							orCreateItem.ShowName = settings.ShowHeroNames && agent != base.Mission.MainAgent && BattlefieldHealthBarView.IsHero(agent);
							orCreateItem.Name = (orCreateItem.ShowName ? BattlefieldHealthBarView.ResolveHeroName(agent) : string.Empty);
							orCreateItem.IsVisible = true;
						}
					}
				}
			}
			this._dataSource.HideFrom(num2);
			return num2;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002990 File Offset: 0x00000B90
		private int UpdateDamageNumbers(float dt, BattlefieldUISettings settings)
		{
			if (!settings.ShowDamageNumbers)
			{
				this.DeactivateDamageNumbers();
				return 0;
			}
			string text = BattlefieldUIColor.ApplyOpacity(settings.FriendlyDamageColor, "#FF6B6BFF", settings.OpacityPercent);
			string text2 = BattlefieldUIColor.ApplyOpacity(settings.EnemyDamageColor, "#FFD166FF", settings.OpacityPercent);
			int num = Math.Max(1, settings.MaximumActiveDamageNumbers);
			float num2 = Math.Max(1f, (float)settings.DamageNumberMaximumDistance);
			float num3 = num2 * 0.75f;
			int num4 = 0;
			for (int i = 0; i < this._damageNumbers.Count; i++)
			{
				BattlefieldHealthBarView.DamageNumberEntry damageNumberEntry = this._damageNumbers[i];
				if (damageNumberEntry.IsActive)
				{
					if (num4 >= num)
					{
						BattlefieldHealthBarView.DeactivateDamageNumber(damageNumberEntry);
					}
					else
					{
						damageNumberEntry.Age += dt;
						if (damageNumberEntry.Age >= damageNumberEntry.Lifetime)
						{
							BattlefieldHealthBarView.DeactivateDamageNumber(damageNumberEntry);
						}
						else
						{
							if (damageNumberEntry.Target != null && damageNumberEntry.Target.IsActive())
							{
								damageNumberEntry.WorldPosition = damageNumberEntry.Target.GetEyeGlobalPosition() + new Vec3(0f, 0f, 0.12f, -1f);
							}
							float num5 = BattlefieldUIDisplayRules.CalculateAlpha((damageNumberEntry.WorldPosition - base.MissionScreen.CombatCamera.Position).Length, num3, num2);
							float num6 = BattlefieldUIDisplayRules.CalculateDamageNumberAlpha(damageNumberEntry.Age, damageNumberEntry.Lifetime);
							float num7 = num5 * num6;
							if (num7 <= 0f)
							{
								damageNumberEntry.Item.Hide();
								num4++;
							}
							else
							{
								float num8 = -10000f;
								float num9 = -10000f;
								float num10 = -1f;
								MBWindowManager.WorldToScreen(base.MissionScreen.CombatCamera, damageNumberEntry.WorldPosition, ref num8, ref num9, ref num10);
								if (num10 <= 0f || !MathF.IsValidValue(num8) || !MathF.IsValidValue(num9))
								{
									damageNumberEntry.Item.Hide();
									num4++;
								}
								else
								{
									float num11 = MathF.Clamp(damageNumberEntry.Age / damageNumberEntry.Lifetime, 0f, 1f);
									float num12 = 1f - (1f - num11) * (1f - num11);
									damageNumberEntry.Item.ScreenPositionX = num8 + damageNumberEntry.HorizontalDrift * num11 - 0.5f;
									damageNumberEntry.Item.ScreenPositionY = num9 - (float)settings.DamageNumberRiseDistance * num12 - 21f;
									damageNumberEntry.Item.Alpha = num7;
									damageNumberEntry.Item.Color = (damageNumberEntry.IsEnemy ? text2 : text);
									damageNumberEntry.Item.FontSize = settings.DamageNumberFontSize;
									damageNumberEntry.Item.IsVisible = true;
									num4++;
								}
							}
						}
					}
				}
			}
			return num4;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002C56 File Offset: 0x00000E56
		private bool ShouldDisplayDamageFor(Agent affectedAgent, BattlefieldUISettings settings, out bool isEnemy)
		{
			isEnemy = false;
			if (affectedAgent == base.Mission.MainAgent)
			{
				return settings.ShowMainAgentDamageNumbers;
			}
			if (!this.TryResolveEnemyState(affectedAgent, out isEnemy))
			{
				return false;
			}
			if (!isEnemy)
			{
				return settings.ShowFriendlyDamageNumbers;
			}
			return settings.ShowEnemyDamageNumbers;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002C90 File Offset: 0x00000E90
		private void AddOrMergeDamageNumber(Agent affectedAgent, Vec3 worldPosition, int damage, bool isEnemy, BattlefieldUISettings settings)
		{
			float num = Math.Max(0f, settings.DamageNumberMergeWindow);
			for (int i = 0; i < this._damageNumbers.Count; i++)
			{
				BattlefieldHealthBarView.DamageNumberEntry damageNumberEntry = this._damageNumbers[i];
				if (damageNumberEntry.IsActive && damageNumberEntry.Target == affectedAgent && damageNumberEntry.Age <= num)
				{
					long num2 = (long)damageNumberEntry.Damage + (long)damage;
					damageNumberEntry.Damage = ((num2 > 2147483647L) ? int.MaxValue : ((int)num2));
					damageNumberEntry.Age = 0f;
					damageNumberEntry.Lifetime = Math.Max(0.1f, settings.DamageNumberLifetime);
					damageNumberEntry.WorldPosition = worldPosition;
					damageNumberEntry.IsEnemy = isEnemy;
					damageNumberEntry.Item.Text = damageNumberEntry.Damage.ToString();
					return;
				}
			}
			int num3 = Math.Max(1, settings.MaximumActiveDamageNumbers);
			BattlefieldHealthBarView.DamageNumberEntry damageNumberEntry2 = this.AcquireDamageNumberEntry(num3);
			damageNumberEntry2.Target = affectedAgent;
			damageNumberEntry2.WorldPosition = worldPosition;
			damageNumberEntry2.Age = 0f;
			damageNumberEntry2.Lifetime = Math.Max(0.1f, settings.DamageNumberLifetime);
			damageNumberEntry2.Damage = damage;
			damageNumberEntry2.IsEnemy = isEnemy;
			damageNumberEntry2.IsActive = true;
			int damageNumberSequence = this._damageNumberSequence;
			this._damageNumberSequence = damageNumberSequence + 1;
			damageNumberEntry2.HorizontalDrift = (float)(damageNumberSequence % 5 - 2) * 7f;
			damageNumberEntry2.Item.Text = damage.ToString();
			damageNumberEntry2.Item.Alpha = 0f;
			damageNumberEntry2.Item.IsVisible = true;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002E10 File Offset: 0x00001010
		private BattlefieldHealthBarView.DamageNumberEntry AcquireDamageNumberEntry(int maximumActive)
		{
			BattlefieldHealthBarView.DamageNumberEntry damageNumberEntry = null;
			int num = 0;
			for (int i = 0; i < this._damageNumbers.Count; i++)
			{
				BattlefieldHealthBarView.DamageNumberEntry damageNumberEntry2 = this._damageNumbers[i];
				if (!damageNumberEntry2.IsActive)
				{
					return damageNumberEntry2;
				}
				num++;
				if (damageNumberEntry == null || damageNumberEntry2.Age > damageNumberEntry.Age)
				{
					damageNumberEntry = damageNumberEntry2;
				}
			}
			if (num < maximumActive)
			{
				BattlefieldHealthBarView.DamageNumberEntry damageNumberEntry3 = new BattlefieldHealthBarView.DamageNumberEntry
				{
					Item = this._dataSource.GetOrCreateDamageNumberItem(this._damageNumbers.Count)
				};
				this._damageNumbers.Add(damageNumberEntry3);
				return damageNumberEntry3;
			}
			return damageNumberEntry;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002EA0 File Offset: 0x000010A0
		private void DeactivateDamageNumbers()
		{
			for (int i = 0; i < this._damageNumbers.Count; i++)
			{
				BattlefieldHealthBarView.DeactivateDamageNumber(this._damageNumbers[i]);
			}
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002ED4 File Offset: 0x000010D4
		private static void DeactivateDamageNumber(BattlefieldHealthBarView.DamageNumberEntry entry)
		{
			entry.IsActive = false;
			entry.Target = null;
			entry.Item.Hide();
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002EF0 File Offset: 0x000010F0
		private bool IsEligibleAgent(Agent agent, BattlefieldUISettings settings, int displayMode, out bool isEnemy)
		{
			isEnemy = false;
			if (agent == null || !agent.IsHuman || !agent.IsActive())
			{
				return false;
			}
			if (agent == base.Mission.MainAgent)
			{
				return settings.ShowMainAgent && BattlefieldHealthBarView.ShouldShowHealthBar(agent, displayMode);
			}
			return this.TryResolveEnemyState(agent, out isEnemy) && (isEnemy ? settings.ShowEnemyAgents : settings.ShowFriendlyAgents) && ((settings.ShowHeroNames && BattlefieldHealthBarView.IsHero(agent)) || BattlefieldHealthBarView.ShouldShowHealthBar(agent, displayMode));
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002F76 File Offset: 0x00001176
		private static int ResolveDisplayMode(BattlefieldUISettings settings)
		{
			if (settings.DisplayMode != null)
			{
				return settings.DisplayMode.SelectedIndex;
			}
			return 1;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002F8D File Offset: 0x0000118D
		private static int ResolveCornerStyle(BattlefieldUISettings settings)
		{
			return BattlefieldUIDisplayRules.NormalizeCornerStyle((settings.HealthBarCornerStyle == null) ? 2 : settings.HealthBarCornerStyle.SelectedIndex);
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002FAA File Offset: 0x000011AA
		private static bool ShouldShowHealthBar(Agent agent, int displayMode)
		{
			return BattlefieldUIDisplayRules.ShouldDisplay(displayMode, agent.Health, agent.HealthLimit);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002FC0 File Offset: 0x000011C0
		private bool TryResolveEnemyState(Agent agent, out bool isEnemy)
		{
			isEnemy = false;
			Team playerTeam = base.Mission.PlayerTeam;
			if (playerTeam == null || agent.Team == null || !playerTeam.IsValid || !agent.Team.IsValid)
			{
				Agent mainAgent = base.Mission.MainAgent;
				if (mainAgent == null || !mainAgent.IsActive())
				{
					return false;
				}
				isEnemy = agent.IsEnemyOf(mainAgent);
				return true;
			}
			else
			{
				if (agent.Team == playerTeam || agent.Team == base.Mission.PlayerAllyTeam)
				{
					return true;
				}
				if (agent.Team.IsEnemyOf(playerTeam))
				{
					isEnemy = true;
					return true;
				}
				return false;
			}
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00003054 File Offset: 0x00001254
		private bool IsInFrontOfCamera(Agent agent, float heightOffset)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = -1f;
			Vec3 vec = agent.GetEyeGlobalPosition() + new Vec3(0f, 0f, heightOffset, -1f);
			MBWindowManager.WorldToScreen(base.MissionScreen.CombatCamera, vec, ref num, ref num2, ref num3);
			return num3 > 0f && MathF.IsValidValue(num) && MathF.IsValidValue(num2);
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000030C4 File Offset: 0x000012C4
		private int CompareCandidateDistance(Agent left, Agent right)
		{
			float lengthSquared = (left.GetEyeGlobalPosition() - this._sortOrigin).LengthSquared;
			float lengthSquared2 = (right.GetEyeGlobalPosition() - this._sortOrigin).LengthSquared;
			return lengthSquared.CompareTo(lengthSquared2);
		}

		// Token: 0x0600001E RID: 30 RVA: 0x0000310D File Offset: 0x0000130D
		private static bool IsSupportedMissionMode(MissionMode mode)
		{
			return mode == 2 || mode == 3 || mode == 6 || mode == 7;
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00003124 File Offset: 0x00001324
		private static bool IsHero(Agent agent)
		{
			CharacterObject characterObject = agent.Character as CharacterObject;
			return characterObject != null && characterObject.HeroObject != null;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x0000314C File Offset: 0x0000134C
		private static string ResolveHeroName(Agent agent)
		{
			CharacterObject characterObject = agent.Character as CharacterObject;
			if (characterObject != null && characterObject.HeroObject != null && characterObject.HeroObject.Name != null)
			{
				return characterObject.HeroObject.Name.ToString();
			}
			if (!(agent.NameTextObject == null))
			{
				return agent.NameTextObject.ToString();
			}
			return string.Empty;
		}

		// Token: 0x04000002 RID: 2
		private const string FriendlyFallbackColor = "#49B86EFF";

		// Token: 0x04000003 RID: 3
		private const string EnemyFallbackColor = "#D9534FFF";

		// Token: 0x04000004 RID: 4
		private const string BackgroundFallbackColor = "#181818CC";

		// Token: 0x04000005 RID: 5
		private const string FriendlyDamageFallbackColor = "#FF6B6BFF";

		// Token: 0x04000006 RID: 6
		private const string EnemyDamageFallbackColor = "#FFD166FF";

		// Token: 0x04000007 RID: 7
		private readonly List<Agent> _candidates = new List<Agent>(512);

		// Token: 0x04000008 RID: 8
		private readonly List<Agent> _visibleAgents = new List<Agent>(256);

		// Token: 0x04000009 RID: 9
		private readonly List<BattlefieldHealthBarView.DamageNumberEntry> _damageNumbers = new List<BattlefieldHealthBarView.DamageNumberEntry>(128);

		// Token: 0x0400000A RID: 10
		private GauntletLayer _layer;

		// Token: 0x0400000B RID: 11
		private BattlefieldUIVM _dataSource;

		// Token: 0x0400000C RID: 12
		private Vec3 _sortOrigin;

		// Token: 0x0400000D RID: 13
		private float _refreshTimer;

		// Token: 0x0400000E RID: 14
		private bool _photoModeActive;

		// Token: 0x0400000F RID: 15
		private int _damageNumberSequence;

		// Token: 0x0200000D RID: 13
		private sealed class DamageNumberEntry
		{
			// Token: 0x0400004F RID: 79
			public BattlefieldDamageNumberItemVM Item;

			// Token: 0x04000050 RID: 80
			public Agent Target;

			// Token: 0x04000051 RID: 81
			public Vec3 WorldPosition;

			// Token: 0x04000052 RID: 82
			public float Age;

			// Token: 0x04000053 RID: 83
			public float Lifetime;

			// Token: 0x04000054 RID: 84
			public float HorizontalDrift;

			// Token: 0x04000055 RID: 85
			public int Damage;

			// Token: 0x04000056 RID: 86
			public bool IsEnemy;

			// Token: 0x04000057 RID: 87
			public bool IsActive;
		}
	}
}
