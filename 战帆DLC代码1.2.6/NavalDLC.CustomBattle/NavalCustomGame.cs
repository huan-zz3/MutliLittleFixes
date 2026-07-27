using System;
using System.Collections.Generic;
using System.Xml;
using NavalDLC.ComponentInterfaces;
using NavalDLC.CustomBattle.CustomBattleObjects;
using NavalDLC.GameComponents;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.CustomBattle
{
	// Token: 0x02000007 RID: 7
	public class NavalCustomGame : GameType
	{
		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600003B RID: 59 RVA: 0x00003004 File Offset: 0x00001204
		public IEnumerable<NavalCustomBattleSceneData> CustomNavalBattleScenes
		{
			get
			{
				return this._customNavalBattleScenes;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600003C RID: 60 RVA: 0x0000300C File Offset: 0x0000120C
		public IEnumerable<NavalCustomBattleSceneData> CustomNavalRaidScenes
		{
			get
			{
				return this._customNavalRaidScenes;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600003D RID: 61 RVA: 0x00003014 File Offset: 0x00001214
		public override string GameTypeStringId
		{
			get
			{
				return "CustomGame";
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600003E RID: 62 RVA: 0x0000301B File Offset: 0x0000121B
		public override bool IsCoreOnlyGameMode
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600003F RID: 63 RVA: 0x0000301E File Offset: 0x0000121E
		// (set) Token: 0x06000040 RID: 64 RVA: 0x00003026 File Offset: 0x00001226
		public NavalCustomBattleBannerEffects NavalCustomBattleBannerEffects { get; private set; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000041 RID: 65 RVA: 0x0000302F File Offset: 0x0000122F
		public static NavalCustomGame Current
		{
			get
			{
				return Game.Current.GameType as NavalCustomGame;
			}
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00003040 File Offset: 0x00001240
		public NavalCustomGame()
		{
			this._customNavalBattleScenes = new List<NavalCustomBattleSceneData>();
			this._customNavalRaidScenes = new List<NavalCustomBattleSceneData>();
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00003060 File Offset: 0x00001260
		protected override void OnInitialize()
		{
			this.InitializeScenes();
			Game currentGame = base.CurrentGame;
			IGameStarter gameStarter = new BasicGameStarter();
			this.InitializeGameModels(gameStarter);
			base.GameManager.InitializeGameStarter(currentGame, gameStarter);
			base.GameManager.OnGameStart(base.CurrentGame, gameStarter);
			MBObjectManager objectManager = currentGame.ObjectManager;
			currentGame.SetBasicModels(gameStarter.Models);
			currentGame.CreateGameManager();
			base.GameManager.BeginGameStart(base.CurrentGame);
			currentGame.InitializeDefaultGameObjects();
			currentGame.LoadBasicFiles();
			this.LoadCustomGameXmls();
			objectManager.UnregisterNonReadyObjects();
			currentGame.SetDefaultEquipments(new Dictionary<string, Equipment>());
			objectManager.UnregisterNonReadyObjects();
			base.GameManager.OnNewCampaignStart(base.CurrentGame, null);
			base.GameManager.OnAfterCampaignStart(base.CurrentGame);
			base.GameManager.OnGameInitializationFinished(base.CurrentGame);
		}

		// Token: 0x06000044 RID: 68 RVA: 0x0000312C File Offset: 0x0000132C
		private void InitializeGameModels(IGameStarter basicGameStarter)
		{
			basicGameStarter.AddModel<AgentStatCalculateModel>(new CustomBattleAgentStatCalculateModel());
			basicGameStarter.AddModel<AgentStatCalculateModel>(new NavalCustomBattleAgentStatCalculateModel());
			basicGameStarter.AddModel<AgentApplyDamageModel>(new NavalDLCCustomAgentApplyDamageModel());
			basicGameStarter.AddModel<ApplyWeatherEffectsModel>(new CustomBattleApplyWeatherEffectsModel());
			basicGameStarter.AddModel<AutoBlockModel>(new CustomBattleAutoBlockModel());
			basicGameStarter.AddModel<BattleMoraleModel>(new CustomBattleMoraleModel());
			basicGameStarter.AddModel<BattleInitializationModel>(new CustomBattleInitializationModel());
			basicGameStarter.AddModel<BattleSpawnModel>(new CustomBattleSpawnModel());
			basicGameStarter.AddModel<AgentDecideKilledOrUnconsciousModel>(new DefaultAgentDecideKilledOrUnconsciousModel());
			basicGameStarter.AddModel<MissionDifficultyModel>(new DefaultMissionDifficultyModel());
			basicGameStarter.AddModel<RidingModel>(new DefaultRidingModel());
			basicGameStarter.AddModel<StrikeMagnitudeCalculationModel>(new DefaultStrikeMagnitudeModel());
			basicGameStarter.AddModel<BattleBannerBearersModel>(new CustomBattleBannerBearersModel());
			basicGameStarter.AddModel<FormationArrangementModel>(new DefaultFormationArrangementModel());
			basicGameStarter.AddModel<DamageParticleModel>(new DefaultDamageParticleModel());
			basicGameStarter.AddModel<ItemPickupModel>(new DefaultItemPickupModel());
			basicGameStarter.AddModel<ItemValueModel>(new DefaultItemValueModel());
			basicGameStarter.AddModel<MissionSiegeEngineCalculationModel>(new DefaultSiegeEngineCalculationModel());
			basicGameStarter.AddModel<CampaignShipParametersModel>(new NavalDLCCampaignShipParametersModel());
			basicGameStarter.AddModel<ShipPhysicsParametersModel>(new NavalDLCShipPhysicsParametersModel());
			basicGameStarter.AddModel<ClanShipOwnershipModel>(new NavalDLCClanShipOwnershipModel());
			basicGameStarter.AddModel<ShipDistributionModel>(new NavalDLCShipDistributionModel());
			basicGameStarter.AddModel<ShipDeploymentModel>(new NavalDLCShipDeploymentModel());
			basicGameStarter.AddModel<MissionShipParametersModel>(new NavalCustomBattleMissionShipParametersModel());
			basicGameStarter.AddModel<BattleInitializationModel>(new NavalCustomBattleInitializationModel());
		}

		// Token: 0x06000045 RID: 69 RVA: 0x0000324C File Offset: 0x0000144C
		private void InitializeScenes()
		{
			XmlDocument mergedXmlForManaged = MBObjectManager.GetMergedXmlForManaged("CustomBattleScenes", true, true, "");
			this.LoadCustomBattleScenes(mergedXmlForManaged);
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00003274 File Offset: 0x00001474
		private void LoadCustomGameXmls()
		{
			this.NavalCustomBattleBannerEffects = new NavalCustomBattleBannerEffects();
			MBObjectManagerExtensions.LoadXML(base.ObjectManager, "Items", false);
			MBObjectManagerExtensions.LoadXML(base.ObjectManager, "EquipmentRosters", false);
			MBObjectManagerExtensions.LoadXML(base.ObjectManager, "NPCCharacters", false);
			MBObjectManagerExtensions.LoadXML(base.ObjectManager, "SPCultures", false);
			MBObjectManagerExtensions.LoadXML(base.ObjectManager, "ShipUpgradePieces", false);
			MBObjectManagerExtensions.LoadXML(base.ObjectManager, "ShipSlots", false);
			MBObjectManagerExtensions.LoadXML(base.ObjectManager, "ShipHulls", false);
			MBObjectManagerExtensions.LoadXML(base.ObjectManager, "ShipPhysicsReferences", false);
			MBObjectManagerExtensions.LoadXML(base.ObjectManager, "MissionShips", false);
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00003325 File Offset: 0x00001525
		protected override void BeforeRegisterTypes(MBObjectManager objectManager)
		{
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00003328 File Offset: 0x00001528
		protected override void OnRegisterTypes(MBObjectManager objectManager)
		{
			objectManager.RegisterType<BasicCharacterObject>("NPCCharacter", "NPCCharacters", 43U, true, false);
			objectManager.RegisterType<BasicCultureObject>("Culture", "SPCultures", 17U, true, false);
			objectManager.RegisterType<ShipUpgradePiece>("ShipUpgradePiece", "ShipUpgradePieces", 60U, true, false);
			objectManager.RegisterType<ShipSlot>("ShipSlot", "ShipSlots", 59U, true, false);
			objectManager.RegisterType<ShipHull>("ShipHull", "ShipHulls", 58U, true, false);
			objectManager.RegisterType<ShipPhysicsReference>("ShipPhysicsReference", "ShipPhysicsReferences", 64U, true, false);
			objectManager.RegisterType<MissionShipObject>("MissionShip", "MissionShips", 57U, true, false);
		}

		// Token: 0x06000049 RID: 73 RVA: 0x000033C1 File Offset: 0x000015C1
		protected override void DoLoadingForGameType(GameTypeLoadingStates gameTypeLoadingState, out GameTypeLoadingStates nextState)
		{
			nextState = -1;
			switch (gameTypeLoadingState)
			{
			case 0:
				base.CurrentGame.Initialize();
				nextState = 1;
				return;
			case 1:
				nextState = 2;
				return;
			case 2:
				nextState = 3;
				break;
			case 3:
				break;
			default:
				return;
			}
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000033F3 File Offset: 0x000015F3
		public override void OnDestroy()
		{
		}

		// Token: 0x0600004B RID: 75 RVA: 0x000033F8 File Offset: 0x000015F8
		private void LoadCustomBattleScenes(XmlDocument doc)
		{
			if (doc.ChildNodes.Count == 0)
			{
				throw new TWXmlLoadException("Incorrect XML document format. XML document has no nodes.");
			}
			bool flag = doc.ChildNodes[0].Name.ToLower().Equals("xml");
			if (flag && doc.ChildNodes.Count == 1)
			{
				throw new TWXmlLoadException("Incorrect XML document format. XML document must have at least one child node");
			}
			XmlNode xmlNode = (flag ? doc.ChildNodes[1] : doc.ChildNodes[0]);
			if (xmlNode.Name != "CustomBattleScenes")
			{
				throw new TWXmlLoadException("Incorrect XML document format. Root node's name must be CustomBattleScenes.");
			}
			if (xmlNode.Name == "CustomBattleScenes")
			{
				foreach (object obj in xmlNode.ChildNodes)
				{
					XmlNode xmlNode2 = (XmlNode)obj;
					if (xmlNode2.NodeType != XmlNodeType.Comment)
					{
						bool flag2 = false;
						bool flag3 = false;
						string text = "";
						string text2 = null;
						TextObject textObject = null;
						TerrainType terrainType = 19;
						for (int i = 0; i < xmlNode2.Attributes.Count; i++)
						{
							if (xmlNode2.Attributes[i].Name == "id")
							{
								text2 = xmlNode2.Attributes[i].InnerText;
							}
							else if (xmlNode2.Attributes[i].Name == "name")
							{
								textObject = new TextObject(xmlNode2.Attributes[i].InnerText, null);
							}
							else if (xmlNode2.Attributes[i].Name == "is_naval_map")
							{
								bool.TryParse(xmlNode2.Attributes[i].InnerText, out flag2);
							}
							else if (xmlNode2.Attributes[i].Name == "is_naval_raid_map")
							{
								bool.TryParse(xmlNode2.Attributes[i].InnerText, out flag3);
							}
							else if (xmlNode2.Attributes[i].Name == "terrain")
							{
								if (!Enum.TryParse<TerrainType>(xmlNode2.Attributes[i].InnerText, out terrainType))
								{
									terrainType = 19;
								}
							}
							else if (xmlNode2.Attributes[i].Name == "forced_scene_level")
							{
								text = xmlNode2.Attributes[i].InnerText;
							}
						}
						if (flag2)
						{
							this._customNavalBattleScenes.Add(new NavalCustomBattleSceneData(text2, textObject, terrainType, text));
						}
						else if (flag3)
						{
							this._customNavalRaidScenes.Add(new NavalCustomBattleSceneData(text2, textObject, terrainType, text));
						}
					}
				}
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000036DC File Offset: 0x000018DC
		public override void OnStateChanged(GameState oldState)
		{
		}

		// Token: 0x0400000C RID: 12
		private List<NavalCustomBattleSceneData> _customNavalBattleScenes;

		// Token: 0x0400000D RID: 13
		private List<NavalCustomBattleSceneData> _customNavalRaidScenes;

		// Token: 0x0400000E RID: 14
		private const TerrainType DefaultTerrain = 19;
	}
}
