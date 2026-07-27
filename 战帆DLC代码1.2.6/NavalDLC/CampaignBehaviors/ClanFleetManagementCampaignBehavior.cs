using System;
using System.Collections.Generic;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace NavalDLC.CampaignBehaviors
{
	// Token: 0x0200015F RID: 351
	public class ClanFleetManagementCampaignBehavior : CampaignBehaviorBase, IFleetManagementCampaignBehavior
	{
		// Token: 0x060016DA RID: 5850 RVA: 0x0009BB8A File Offset: 0x00099D8A
		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
		}

		// Token: 0x060016DB RID: 5851 RVA: 0x0009BBBC File Offset: 0x00099DBC
		private void HourlyTick()
		{
			if (Campaign.Current.Models.FleetManagementModel.CanTroopsReturn())
			{
				for (int i = this._sentTroops.Count - 1; i >= 0; i--)
				{
					if (this._sentTroops[i].TroopReturnTime.IsPast)
					{
						this.MakeTroopsReturn(this._sentTroops[i]);
						this._sentTroops.RemoveAt(i);
					}
				}
			}
		}

		// Token: 0x060016DC RID: 5852 RVA: 0x0009BC30 File Offset: 0x00099E30
		private void OnSessionLaunched(CampaignGameStarter starter)
		{
			this.AddDialogs(starter);
		}

		// Token: 0x060016DD RID: 5853 RVA: 0x0009BC39 File Offset: 0x00099E39
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<List<ClanFleetManagementCampaignBehavior.SentTroopsData>>("_sentTroops", ref this._sentTroops);
		}

		// Token: 0x060016DE RID: 5854 RVA: 0x0009BC50 File Offset: 0x00099E50
		private void AddDialogs(CampaignGameStarter starter)
		{
			starter.AddPlayerLine("clan_party_manage_fleet", "hero_main_options", "clan_party_manage_fleet_screen", "{=7DdiFD9W}Let me inspect your ships.", new ConversationSentence.OnConditionDelegate(this.conversation_clan_member_manage_fleet_on_condition), null, 90, null, null);
			starter.AddDialogLine("clan_party_manage_fleet_screen", "clan_party_manage_fleet_screen", "lord_pretalk", "{=!}fleet screen goes here.", null, new ConversationSentence.OnConsequenceDelegate(this.conversation_clan_member_manage_fleet_on_consequence), 100, null);
		}

		// Token: 0x060016DF RID: 5855 RVA: 0x0009BCB4 File Offset: 0x00099EB4
		private void MakeTroopsReturn(ClanFleetManagementCampaignBehavior.SentTroopsData sentTroops)
		{
			MobileParty.MainParty.MemberRoster.Add(sentTroops.SentTroops);
			TextObject textObject = new TextObject("{=CC5Aa5VH}Your troops have returned from delivering {SHIP_NAME} to {PARTY_NAME}.", null);
			textObject.SetTextVariable("SHIP_NAME", sentTroops.ShipName);
			textObject.SetTextVariable("PARTY_NAME", sentTroops.PartyName);
			InformationManager.DisplayMessage(new InformationMessage(textObject.ToString(), new Color(0f, 1f, 0f, 1f)));
		}

		// Token: 0x060016E0 RID: 5856 RVA: 0x0009BD30 File Offset: 0x00099F30
		private bool conversation_clan_member_manage_fleet_on_condition()
		{
			Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
			return MobileParty.MainParty.MapEvent == null && oneToOneConversationHero != null && oneToOneConversationHero.Clan == Clan.PlayerClan && oneToOneConversationHero.PartyBelongedTo != null && oneToOneConversationHero.PartyBelongedTo.LeaderHero == oneToOneConversationHero && oneToOneConversationHero.PartyBelongedTo.MapEvent == null && !oneToOneConversationHero.PartyBelongedTo.IsCaravan && !oneToOneConversationHero.PartyBelongedTo.IsMilitia && !oneToOneConversationHero.PartyBelongedTo.IsVillager && !oneToOneConversationHero.PartyBelongedTo.IsPatrolParty && (oneToOneConversationHero.PartyBelongedTo.Ships.Count > 0 || MobileParty.MainParty.Ships.Count > 0);
		}

		// Token: 0x060016E1 RID: 5857 RVA: 0x0009BDE5 File Offset: 0x00099FE5
		private void conversation_clan_member_manage_fleet_on_consequence()
		{
			PortStateHelper.OpenAsManageOtherFleet(Hero.OneToOneConversationHero.PartyBelongedTo.Party, new Action(this.OnManageOtherFleetDone));
		}

		// Token: 0x060016E2 RID: 5858 RVA: 0x0009BE07 File Offset: 0x0009A007
		private void OnManageOtherFleetDone()
		{
			Campaign.Current.ConversationManager.ContinueConversation();
		}

		// Token: 0x060016E3 RID: 5859 RVA: 0x0009BE18 File Offset: 0x0009A018
		public void SendShipToParty(Ship ship, MobileParty mobileParty)
		{
			TroopRoster troopRoster = MobileParty.MainParty.MemberRoster.RemoveNumberOfNonHeroTroopsRandomly(Campaign.Current.Models.FleetManagementModel.MinimumTroopCountRequiredToSendShips);
			this._sentTroops.Add(ClanFleetManagementCampaignBehavior.SentTroopsData.GetSentTroops(troopRoster, Campaign.Current.Models.FleetManagementModel.GetReturnTimeForTroops(ship), ship.Name, mobileParty.Name));
			ChangeShipOwnerAction.ApplyByTransferring(mobileParty.Party, ship);
		}

		// Token: 0x060016E4 RID: 5860 RVA: 0x0009BE88 File Offset: 0x0009A088
		public void SendShipToClan(Ship ship, Clan clan)
		{
			float num = float.MinValue;
			MobileParty mobileParty = null;
			MBList<Ship> mblist = new MBList<Ship>();
			foreach (WarPartyComponent warPartyComponent in clan.WarPartyComponents)
			{
				if (NavalDLCManager.Instance.GameModels.ShipDistributionModel.CanSendShipToParty(ship, warPartyComponent.MobileParty) && (mobileParty == null || mobileParty.Ships.Count >= warPartyComponent.Party.Ships.Count))
				{
					mblist.Clear();
					mblist.AddRange(warPartyComponent.Party.Ships);
					float scoreForPartyShipComposition = NavalDLCManager.Instance.GameModels.ShipDistributionModel.GetScoreForPartyShipComposition(warPartyComponent.MobileParty, mblist);
					mblist.Add(ship);
					float num2 = NavalDLCManager.Instance.GameModels.ShipDistributionModel.GetScoreForPartyShipComposition(warPartyComponent.MobileParty, mblist) - scoreForPartyShipComposition;
					if (num2 > num)
					{
						mobileParty = warPartyComponent.MobileParty;
						num = num2;
					}
				}
			}
			if (mobileParty != null)
			{
				this.SendShipToParty(ship, mobileParty);
				return;
			}
			DestroyShipAction.Apply(ship);
		}

		// Token: 0x04000BBF RID: 3007
		private List<ClanFleetManagementCampaignBehavior.SentTroopsData> _sentTroops = new List<ClanFleetManagementCampaignBehavior.SentTroopsData>();

		// Token: 0x02000290 RID: 656
		private class SentTroopsData
		{
			// Token: 0x17000463 RID: 1123
			// (get) Token: 0x06001CB3 RID: 7347 RVA: 0x000B9C71 File Offset: 0x000B7E71
			// (set) Token: 0x06001CB4 RID: 7348 RVA: 0x000B9C79 File Offset: 0x000B7E79
			[SaveableProperty(0)]
			public TroopRoster SentTroops { get; private set; }

			// Token: 0x17000464 RID: 1124
			// (get) Token: 0x06001CB5 RID: 7349 RVA: 0x000B9C82 File Offset: 0x000B7E82
			// (set) Token: 0x06001CB6 RID: 7350 RVA: 0x000B9C8A File Offset: 0x000B7E8A
			[SaveableProperty(1)]
			public CampaignTime TroopReturnTime { get; private set; }

			// Token: 0x17000465 RID: 1125
			// (get) Token: 0x06001CB7 RID: 7351 RVA: 0x000B9C93 File Offset: 0x000B7E93
			// (set) Token: 0x06001CB8 RID: 7352 RVA: 0x000B9C9B File Offset: 0x000B7E9B
			[SaveableProperty(2)]
			public TextObject ShipName { get; private set; }

			// Token: 0x17000466 RID: 1126
			// (get) Token: 0x06001CB9 RID: 7353 RVA: 0x000B9CA4 File Offset: 0x000B7EA4
			// (set) Token: 0x06001CBA RID: 7354 RVA: 0x000B9CAC File Offset: 0x000B7EAC
			[SaveableProperty(3)]
			public TextObject PartyName { get; private set; }

			// Token: 0x06001CBB RID: 7355 RVA: 0x000B9CB5 File Offset: 0x000B7EB5
			public static ClanFleetManagementCampaignBehavior.SentTroopsData GetSentTroops(TroopRoster troops, CampaignTime returnTime, TextObject shipName, TextObject partyName)
			{
				ClanFleetManagementCampaignBehavior.SentTroopsData sentTroopsData = new ClanFleetManagementCampaignBehavior.SentTroopsData();
				sentTroopsData.SentTroops = TroopRoster.CreateDummyTroopRoster();
				sentTroopsData.SentTroops.Add(troops);
				sentTroopsData.TroopReturnTime = returnTime;
				sentTroopsData.ShipName = shipName;
				sentTroopsData.PartyName = partyName;
				return sentTroopsData;
			}
		}

		// Token: 0x02000291 RID: 657
		public class ClanFleetManagementCampaignBehaviorTypeDefiner : SaveableTypeDefiner
		{
			// Token: 0x06001CBD RID: 7357 RVA: 0x000B9CF0 File Offset: 0x000B7EF0
			public ClanFleetManagementCampaignBehaviorTypeDefiner()
				: base(612504)
			{
			}

			// Token: 0x06001CBE RID: 7358 RVA: 0x000B9CFD File Offset: 0x000B7EFD
			protected override void DefineContainerDefinitions()
			{
				base.ConstructContainerDefinition(typeof(List<ClanFleetManagementCampaignBehavior.SentTroopsData>));
			}

			// Token: 0x06001CBF RID: 7359 RVA: 0x000B9D0F File Offset: 0x000B7F0F
			protected override void DefineClassTypes()
			{
				base.AddClassDefinition(typeof(ClanFleetManagementCampaignBehavior.SentTroopsData), 2, null);
			}
		}
	}
}
