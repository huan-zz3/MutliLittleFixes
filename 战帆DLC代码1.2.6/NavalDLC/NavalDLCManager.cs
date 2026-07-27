using System;
using System.Collections.Generic;
using NavalDLC.CharacterDevelopment;
using NavalDLC.Map;
using NavalDLC.Settlements;
using NavalDLC.Settlements.Building;
using NavalDLC.Storyline;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace NavalDLC
{
	// Token: 0x02000020 RID: 32
	public class NavalDLCManager : GameHandler
	{
		// Token: 0x17000032 RID: 50
		// (get) Token: 0x0600014B RID: 331 RVA: 0x0000944B File Offset: 0x0000764B
		// (set) Token: 0x0600014C RID: 332 RVA: 0x00009453 File Offset: 0x00007653
		public GameModels GameModels { get; private set; }

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x0600014D RID: 333 RVA: 0x0000945C File Offset: 0x0000765C
		// (set) Token: 0x0600014E RID: 334 RVA: 0x00009464 File Offset: 0x00007664
		public NavalCulturalFeats NavalCulturalFeats { get; private set; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x0600014F RID: 335 RVA: 0x0000946D File Offset: 0x0000766D
		// (set) Token: 0x06000150 RID: 336 RVA: 0x00009475 File Offset: 0x00007675
		public NavalBuildingTypes NavalBuildingTypes { get; private set; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000151 RID: 337 RVA: 0x0000947E File Offset: 0x0000767E
		// (set) Token: 0x06000152 RID: 338 RVA: 0x00009486 File Offset: 0x00007686
		public NavalVillageTypes NavalVillageTypes { get; private set; }

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x06000153 RID: 339 RVA: 0x0000948F File Offset: 0x0000768F
		// (set) Token: 0x06000154 RID: 340 RVA: 0x00009497 File Offset: 0x00007697
		public NavalSkills NavalSkills { get; private set; }

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x06000155 RID: 341 RVA: 0x000094A0 File Offset: 0x000076A0
		// (set) Token: 0x06000156 RID: 342 RVA: 0x000094A8 File Offset: 0x000076A8
		public NavalSkillEffects NavalSkillEffects { get; private set; }

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000157 RID: 343 RVA: 0x000094B1 File Offset: 0x000076B1
		// (set) Token: 0x06000158 RID: 344 RVA: 0x000094B9 File Offset: 0x000076B9
		public NavalPerks NavalPerks { get; private set; }

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000159 RID: 345 RVA: 0x000094C2 File Offset: 0x000076C2
		// (set) Token: 0x0600015A RID: 346 RVA: 0x000094CA File Offset: 0x000076CA
		public NavalPolicies NavalPolicies { get; private set; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x0600015B RID: 347 RVA: 0x000094D3 File Offset: 0x000076D3
		// (set) Token: 0x0600015C RID: 348 RVA: 0x000094DB File Offset: 0x000076DB
		public NavalStorylineData NavalStorylineData { get; private set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x0600015D RID: 349 RVA: 0x000094E4 File Offset: 0x000076E4
		// (set) Token: 0x0600015E RID: 350 RVA: 0x000094EC File Offset: 0x000076EC
		public NavalDLCEvents NavalDLCEvents { get; private set; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x0600015F RID: 351 RVA: 0x000094F5 File Offset: 0x000076F5
		// (set) Token: 0x06000160 RID: 352 RVA: 0x000094FD File Offset: 0x000076FD
		public NavalItemCategories NavalItemCategories { get; private set; }

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x06000161 RID: 353 RVA: 0x00009506 File Offset: 0x00007706
		// (set) Token: 0x06000162 RID: 354 RVA: 0x0000950E File Offset: 0x0000770E
		public INavalMapSceneWrapper NavalMapSceneWrapper { get; set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000163 RID: 355 RVA: 0x00009517 File Offset: 0x00007717
		// (set) Token: 0x06000164 RID: 356 RVA: 0x0000951F File Offset: 0x0000771F
		public Dictionary<Village, List<FishingPartyComponent>> FishingParties { get; private set; }

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000165 RID: 357 RVA: 0x00009528 File Offset: 0x00007728
		// (set) Token: 0x06000166 RID: 358 RVA: 0x00009530 File Offset: 0x00007730
		public StormManager StormManager { get; internal set; }

		// Token: 0x06000167 RID: 359 RVA: 0x00009539 File Offset: 0x00007739
		public override void OnAfterSave()
		{
		}

		// Token: 0x06000168 RID: 360 RVA: 0x0000953B File Offset: 0x0000773B
		public override void OnBeforeSave()
		{
		}

		// Token: 0x06000169 RID: 361 RVA: 0x0000953D File Offset: 0x0000773D
		protected override void OnInitialize()
		{
		}

		// Token: 0x0600016A RID: 362 RVA: 0x0000953F File Offset: 0x0000773F
		protected override void OnTick(float dt)
		{
			INavalMapSceneWrapper navalMapSceneWrapper = this.NavalMapSceneWrapper;
			if (navalMapSceneWrapper == null)
			{
				return;
			}
			navalMapSceneWrapper.Tick(dt);
		}

		// Token: 0x0600016B RID: 363 RVA: 0x00009554 File Offset: 0x00007754
		public void OnGameStart(Game game, IGameStarter gameStarter)
		{
			this.GameModels = game.AddGameModelsManager<GameModels>(gameStarter.Models);
			if (game.GameType is Campaign)
			{
				this.NavalDLCEvents = new NavalDLCEvents();
				Campaign.Current.AddCampaignEventReceiver(this.NavalDLCEvents);
				if (Campaign.Current.CampaignGameLoadingType == 1 || Campaign.Current.CampaignGameLoadingType == null)
				{
					Campaign.Current.AddCustomManager<StormManager>();
					this.StormManager = Campaign.Current.GetCustomManager<StormManager>();
					return;
				}
				if (Campaign.Current.CampaignGameLoadingType == 2)
				{
					this.StormManager = Campaign.Current.GetCustomManager<StormManager>();
					this.StormManager.OnAfterLoad();
				}
			}
		}

		// Token: 0x0600016C RID: 364 RVA: 0x000095F7 File Offset: 0x000077F7
		public void OnGameEnd(Game game)
		{
			this.NavalMapSceneWrapper = null;
			NavalDLCManager.Instance = null;
		}

		// Token: 0x0600016D RID: 365 RVA: 0x00009608 File Offset: 0x00007808
		public void InitializeNavalGameObjects(Game game)
		{
			Campaign campaign = game.GameType as Campaign;
			if (campaign != null)
			{
				this.NavalBuildingTypes = new NavalBuildingTypes();
				this.NavalCulturalFeats = new NavalCulturalFeats();
				this.NavalItemCategories = new NavalItemCategories();
				this.NavalVillageTypes = new NavalVillageTypes();
				this.NavalStorylineData = new NavalStorylineData();
			}
			this.NavalSkills = new NavalSkills();
			if (campaign != null)
			{
				this.NavalSkillEffects = new NavalSkillEffects();
				this.NavalPerks = new NavalPerks();
				this.NavalPolicies = new NavalPolicies();
				campaign.SkillLevelingManager = new NavalSkillLevellingManager();
				this.FishingParties = new Dictionary<Village, List<FishingPartyComponent>>();
			}
		}

		// Token: 0x04000092 RID: 146
		public static NavalDLCManager Instance;
	}
}
