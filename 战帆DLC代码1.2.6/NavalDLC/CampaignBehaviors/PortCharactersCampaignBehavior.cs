using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NavalDLC.Storyline;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;

namespace NavalDLC.CampaignBehaviors
{
	// Token: 0x02000172 RID: 370
	public class PortCharactersCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600184F RID: 6223 RVA: 0x000A598F File Offset: 0x000A3B8F
		public override void RegisterEvents()
		{
			CampaignEvents.OnAfterSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnAfterSessionLaunched));
			CampaignEvents.LocationCharactersAreReadyToSpawnEvent.AddNonSerializedListener(this, new Action<Dictionary<string, int>>(this.LocationCharactersAreReadyToSpawn));
		}

		// Token: 0x06001850 RID: 6224 RVA: 0x000A59BF File Offset: 0x000A3BBF
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06001851 RID: 6225 RVA: 0x000A59C1 File Offset: 0x000A3BC1
		private void OnAfterSessionLaunched(CampaignGameStarter campaignGameSystemStarter)
		{
			PortCharactersCampaignBehavior.AddDialogs(campaignGameSystemStarter);
		}

		// Token: 0x06001852 RID: 6226 RVA: 0x000A59CC File Offset: 0x000A3BCC
		private void LocationCharactersAreReadyToSpawn(Dictionary<string, int> unusedUsablePointCount)
		{
			Settlement currentSettlement = Settlement.CurrentSettlement;
			Location location = ((currentSettlement != null) ? currentSettlement.LocationComplex.GetLocationWithId("port") : null);
			if (location != null && !NavalStorylineData.IsNavalStoryLineActive())
			{
				int num;
				if (unusedUsablePointCount.TryGetValue("sp_shipwright", out num))
				{
					location.AddLocationCharacters(new CreateLocationCharacterDelegate(PortCharactersCampaignBehavior.CreateShipWright), Settlement.CurrentSettlement.Culture, 0, 1);
				}
				if (unusedUsablePointCount.TryGetValue("merchant_carpenter", out num))
				{
					int num2 = 1 + (int)((float)num * 0.35f);
					location.AddLocationCharacters(new CreateLocationCharacterDelegate(PortCharactersCampaignBehavior.CreatePortMerchant), Settlement.CurrentSettlement.Culture, 0, num2);
				}
				if (unusedUsablePointCount.TryGetValue("npc_common", out num))
				{
					float num3 = (float)num * 0.2f;
					location.AddLocationCharacters(new CreateLocationCharacterDelegate(PortCharactersCampaignBehavior.CreateTownsPeopleMale), Settlement.CurrentSettlement.Culture, 0, (int)num3);
					float num4 = (float)num * 0.1f;
					location.AddLocationCharacters(new CreateLocationCharacterDelegate(PortCharactersCampaignBehavior.CreateTownsPeopleFemale), Settlement.CurrentSettlement.Culture, 0, (int)num4);
				}
				if (unusedUsablePointCount.TryGetValue("npc_common_limited", out num))
				{
					float num5 = (float)num * 0.6f;
					location.AddLocationCharacters(new CreateLocationCharacterDelegate(PortCharactersCampaignBehavior.CreateTownsManCarryingStuff), Settlement.CurrentSettlement.Culture, 0, (int)num5);
				}
				if (unusedUsablePointCount.TryGetValue("shipyard_worker", out num))
				{
					float num6 = (float)num * 1f;
					location.AddLocationCharacters(new CreateLocationCharacterDelegate(PortCharactersCampaignBehavior.CreateShipyardWorker), Settlement.CurrentSettlement.Culture, 0, (int)num6);
				}
				if (unusedUsablePointCount.TryGetValue("market_worker", out num))
				{
					float num7 = (float)num * 0.75f;
					location.AddLocationCharacters(new CreateLocationCharacterDelegate(PortCharactersCampaignBehavior.CreatePortMarketWorker), Settlement.CurrentSettlement.Culture, 0, (int)num7);
				}
				if (unusedUsablePointCount.TryGetValue("static_npc", out num))
				{
					location.AddLocationCharacters(new CreateLocationCharacterDelegate(PortCharactersCampaignBehavior.CreateStaticTownsPeopleMale), Settlement.CurrentSettlement.Culture, 0, num);
				}
				if (unusedUsablePointCount.TryGetValue("musician", out num) && num > 0)
				{
					location.AddLocationCharacters(new CreateLocationCharacterDelegate(PortCharactersCampaignBehavior.CreateMusician), Settlement.CurrentSettlement.Culture, 0, num);
				}
			}
		}

		// Token: 0x06001853 RID: 6227 RVA: 0x000A5BD8 File Offset: 0x000A3DD8
		private static LocationCharacter CreateTownsManCarryingStuff(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject townsman = culture.Townsman;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(townsman.Race, "_settlement_slow");
			ValueTuple<string, string, bool> randomActionSetSuffixAndItem = PortCharactersCampaignBehavior.GetRandomActionSetSuffixAndItem();
			string item = randomActionSetSuffixAndItem.Item1;
			string item2 = randomActionSetSuffixAndItem.Item2;
			bool item3 = randomActionSetSuffixAndItem.Item3;
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(townsman, ref num, ref num2, "TownsfolkCarryingStuff");
			AgentData agentData = new AgentData(new SimpleAgentOrigin(townsman, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix).Age(MBRandom.RandomInt(num, num2));
			ItemObject @object = Game.Current.ObjectManager.GetObject<ItemObject>(item2);
			LocationCharacter locationCharacter = new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "npc_common_limited", false, relation, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, townsman.IsFemale, item), true, false, @object, false, false, true, null, false);
			if (@object == null)
			{
				locationCharacter.PrefabNamesForBones.Add(item3 ? agentData.AgentMonster.MainHandItemBoneIndex : agentData.AgentMonster.OffHandItemBoneIndex, item2);
			}
			return locationCharacter;
		}

		// Token: 0x06001854 RID: 6228 RVA: 0x000A5CE8 File Offset: 0x000A3EE8
		private static LocationCharacter CreateTownsPeopleMale(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject townsman = culture.Townsman;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(townsman.Race, "_settlement_slow");
			Tuple<string, Monster> tuple = new Tuple<string, Monster>(ActionSetCode.GenerateActionSetNameWithSuffix(monsterWithSuffix, false, "_villager_2"), monsterWithSuffix);
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(townsman, ref num, ref num2, "");
			return new LocationCharacter(new AgentData(new SimpleAgentOrigin(townsman, -1, null, default(UniqueTroopDescriptor))).Monster(tuple.Item2).Age(MBRandom.RandomInt(num, num2)), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddIndoorWandererBehaviors), "npc_common", false, relation, tuple.Item1, true, false, null, false, false, true, null, false);
		}

		// Token: 0x06001855 RID: 6229 RVA: 0x000A5DA0 File Offset: 0x000A3FA0
		private static LocationCharacter CreateStaticTownsPeopleMale(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject townsman = culture.Townsman;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(townsman.Race, "_settlement_slow");
			Tuple<string, Monster> tuple = new Tuple<string, Monster>(ActionSetCode.GenerateActionSetNameWithSuffix(monsterWithSuffix, false, "_villager_2"), monsterWithSuffix);
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(townsman, ref num, ref num2, "");
			return new LocationCharacter(new AgentData(new SimpleAgentOrigin(townsman, -1, null, default(UniqueTroopDescriptor))).Monster(tuple.Item2).Age(MBRandom.RandomInt(num, num2)), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddIndoorWandererBehaviors), "static_npc", false, relation, tuple.Item1, true, false, null, false, false, true, null, false);
		}

		// Token: 0x06001856 RID: 6230 RVA: 0x000A5E58 File Offset: 0x000A4058
		private static LocationCharacter CreateTownsPeopleFemale(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject townswoman = culture.Townswoman;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(townswoman.Race, "_settlement_slow");
			Tuple<string, Monster> tuple = new Tuple<string, Monster>(ActionSetCode.GenerateActionSetNameWithSuffix(monsterWithSuffix, true, "_villager_2"), monsterWithSuffix);
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(townswoman, ref num, ref num2, "");
			return new LocationCharacter(new AgentData(new SimpleAgentOrigin(townswoman, -1, null, default(UniqueTroopDescriptor))).Monster(tuple.Item2).Age(MBRandom.RandomInt(num, num2)).IsFemale(true), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddIndoorWandererBehaviors), "npc_common", false, relation, tuple.Item1, true, false, null, false, false, true, null, false);
		}

		// Token: 0x06001857 RID: 6231 RVA: 0x000A5F14 File Offset: 0x000A4114
		private static LocationCharacter CreateShipyardWorker(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject shipyardWorker = culture.ShipyardWorker;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(shipyardWorker.Race, "_settlement_slow");
			Tuple<string, Monster> tuple = new Tuple<string, Monster>(ActionSetCode.GenerateActionSetNameWithSuffix(monsterWithSuffix, false, "_villager_2"), monsterWithSuffix);
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(shipyardWorker, ref num, ref num2, "");
			return new LocationCharacter(new AgentData(new SimpleAgentOrigin(shipyardWorker, -1, null, default(UniqueTroopDescriptor))).Monster(tuple.Item2).Age(MBRandom.RandomInt(num, num2)), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddIndoorWandererBehaviors), "shipyard_worker", true, relation, tuple.Item1, true, false, null, false, false, true, null, false);
		}

		// Token: 0x06001858 RID: 6232 RVA: 0x000A5FCC File Offset: 0x000A41CC
		private static LocationCharacter CreatePortMarketWorker(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject shipyardWorker = culture.ShipyardWorker;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(shipyardWorker.Race, "_settlement_slow");
			Tuple<string, Monster> tuple = new Tuple<string, Monster>(ActionSetCode.GenerateActionSetNameWithSuffix(monsterWithSuffix, false, "_villager_2"), monsterWithSuffix);
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(shipyardWorker, ref num, ref num2, "");
			return new LocationCharacter(new AgentData(new SimpleAgentOrigin(shipyardWorker, -1, null, default(UniqueTroopDescriptor))).Monster(tuple.Item2).Age(MBRandom.RandomInt(num, num2)), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddIndoorWandererBehaviors), "market_worker", true, relation, tuple.Item1, true, false, null, false, false, true, null, false);
		}

		// Token: 0x06001859 RID: 6233 RVA: 0x000A6084 File Offset: 0x000A4284
		private static LocationCharacter CreatePortMerchant(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject merchant = culture.Merchant;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(merchant.Race, "_settlement_slow");
			Tuple<string, Monster> tuple = new Tuple<string, Monster>(ActionSetCode.GenerateActionSetNameWithSuffix(monsterWithSuffix, false, "_villager_2"), monsterWithSuffix);
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(merchant, ref num, ref num2, "");
			return new LocationCharacter(new AgentData(new SimpleAgentOrigin(merchant, -1, null, default(UniqueTroopDescriptor))).Monster(tuple.Item2).Age(MBRandom.RandomInt(num, num2)), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddFixedCharacterBehaviors), "shipyard_shop_worker", false, relation, tuple.Item1, true, false, null, false, false, true, null, false);
		}

		// Token: 0x0600185A RID: 6234 RVA: 0x000A613C File Offset: 0x000A433C
		private static LocationCharacter CreateMusician(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject musician = culture.Musician;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(musician.Race, "_settlement");
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(musician, ref num, ref num2, "");
			AgentData agentData = new AgentData(new SimpleAgentOrigin(musician, -1, null, default(UniqueTroopDescriptor))).Monster(monsterWithSuffix).Age(MBRandom.RandomInt(num, num2));
			return new LocationCharacter(agentData, new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors), "musician", true, relation, ActionSetCode.GenerateActionSetNameWithSuffix(agentData.AgentMonster, agentData.AgentIsFemale, "_musician"), true, false, null, false, false, true, null, false);
		}

		// Token: 0x0600185B RID: 6235 RVA: 0x000A61F0 File Offset: 0x000A43F0
		private static LocationCharacter CreateShipWright(CultureObject culture, LocationCharacter.CharacterRelations relation)
		{
			CharacterObject shipwright = culture.Shipwright;
			Monster monsterWithSuffix = FaceGen.GetMonsterWithSuffix(shipwright.Race, "_settlement_slow");
			Tuple<string, Monster> tuple = new Tuple<string, Monster>(ActionSetCode.GenerateActionSetNameWithSuffix(monsterWithSuffix, false, "_villager_2"), monsterWithSuffix);
			int num;
			int num2;
			Campaign.Current.Models.AgeModel.GetAgeLimitForLocation(shipwright, ref num, ref num2, "");
			return new LocationCharacter(new AgentData(new SimpleAgentOrigin(shipwright, -1, null, default(UniqueTroopDescriptor))).Monster(tuple.Item2).Age(MBRandom.RandomInt(num, num2)), new LocationCharacter.AddBehaviorsDelegate(SandBoxManager.Instance.AgentBehaviorManager.AddFixedCharacterBehaviors), "sp_shipwright", true, relation, tuple.Item1, true, false, null, false, false, true, null, false);
		}

		// Token: 0x0600185C RID: 6236 RVA: 0x000A62A8 File Offset: 0x000A44A8
		public static ValueTuple<string, string, bool> GetRandomActionSetSuffixAndItem()
		{
			string item = Extensions.GetRandomElement<ValueTuple<string, bool>>(PortCharactersCampaignBehavior._itemToCarryAndIsMainHandData).Item1;
			if (item == "wood_load")
			{
				return new ValueTuple<string, string, bool>("_worker_carry_wood_on_shoulder", item, true);
			}
			if (item == "bucket_filled")
			{
				return new ValueTuple<string, string, bool>("_villager_carry_bucket_on_lefthand", item, false);
			}
			if (!(item == "carry_fish_stick"))
			{
				return new ValueTuple<string, string, bool>("_worker_carry_wood_on_shoulder", item, true);
			}
			return new ValueTuple<string, string, bool>("_villager_carry_fish_buckets", item, false);
		}

		// Token: 0x0600185D RID: 6237 RVA: 0x000A6324 File Offset: 0x000A4524
		private static void AddDialogs(CampaignGameStarter campaignGameSystemStarter)
		{
			campaignGameSystemStarter.AddDialogLine("shipwright_dialog_start", "start", "close_window", "{=PZk5f99h}Greetings, {?PLAYER.GENDER}madam{?}sir{\\?}. This is where we lay the keels, fit the planks, and nail them all together.", new ConversationSentence.OnConditionDelegate(PortCharactersCampaignBehavior.shipwright_default_dialog_start), null, 100, null);
			campaignGameSystemStarter.AddDialogLine("shipyard_market_worker", "start", "close_window", "{=!}Greetings, {?PLAYER.GENDER}madam{?}sir{\\?}. This is where we pack the stores for all those sailors and travelers about to put to sea.", new ConversationSentence.OnConditionDelegate(PortCharactersCampaignBehavior.shipyard_marker_worker_default_dialog_start), null, 100, null);
		}

		// Token: 0x0600185E RID: 6238 RVA: 0x000A6388 File Offset: 0x000A4588
		[return: TupleElementNames(new string[] { "ConversationCharacterOccupation", "ConversationCharacterSpecialTag" })]
		private static ValueTuple<Occupation, string> GetConversationCharacterInfo()
		{
			if (Campaign.Current.ConversationManager.OneToOneConversationCharacter != null && Campaign.Current.ConversationManager.OneToOneConversationAgent != null && Settlement.CurrentSettlement != null && Settlement.CurrentSettlement.LocationComplex != null)
			{
				CharacterObject oneToOneConversationCharacter = Campaign.Current.ConversationManager.OneToOneConversationCharacter;
				IAgent oneToOneConversationAgent = Campaign.Current.ConversationManager.OneToOneConversationAgent;
				LocationCharacter locationCharacter = Settlement.CurrentSettlement.LocationComplex.FindCharacter(oneToOneConversationAgent);
				string text = ((locationCharacter != null) ? locationCharacter.SpecialTargetTag : null);
				return new ValueTuple<Occupation, string>(oneToOneConversationCharacter.Occupation, text);
			}
			return new ValueTuple<Occupation, string>(0, string.Empty);
		}

		// Token: 0x0600185F RID: 6239 RVA: 0x000A6420 File Offset: 0x000A4620
		private static bool shipwright_default_dialog_start()
		{
			ValueTuple<Occupation, string> conversationCharacterInfo = PortCharactersCampaignBehavior.GetConversationCharacterInfo();
			return conversationCharacterInfo.Item1 == 32 && (conversationCharacterInfo.Item2 == "shipyard_worker" || conversationCharacterInfo.Item2 == "sp_shipwright");
		}

		// Token: 0x06001860 RID: 6240 RVA: 0x000A6464 File Offset: 0x000A4664
		private static bool shipyard_marker_worker_default_dialog_start()
		{
			ValueTuple<Occupation, string> conversationCharacterInfo = PortCharactersCampaignBehavior.GetConversationCharacterInfo();
			return conversationCharacterInfo.Item1 == 32 && conversationCharacterInfo.Item2 == "market_worker";
		}

		// Token: 0x04000BF9 RID: 3065
		private const float PortTownsmanCarryingStuffSpawnPercentage = 0.6f;

		// Token: 0x04000BFA RID: 3066
		private const float PortTownsmanSpawnPercentageMale = 0.2f;

		// Token: 0x04000BFB RID: 3067
		private const float PortTownsmanSpawnPercentageFemale = 0.1f;

		// Token: 0x04000BFC RID: 3068
		private const float ShipyardWorkerSpawnPercentage = 1f;

		// Token: 0x04000BFD RID: 3069
		private const float MarketWorkerSpawnPercentage = 0.75f;

		// Token: 0x04000BFE RID: 3070
		private const float CarpenterSpawnPercentage = 0.35f;

		// Token: 0x04000BFF RID: 3071
		private static List<ValueTuple<string, bool>> _itemToCarryAndIsMainHandData = new List<ValueTuple<string, bool>>
		{
			new ValueTuple<string, bool>("wood_load", true),
			new ValueTuple<string, bool>("bucket_filled", false),
			new ValueTuple<string, bool>("carry_fish_stick", false)
		};
	}
}
