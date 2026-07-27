using System;
using Helpers;
using SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Conversation;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.CampaignBehaviors
{
	// Token: 0x02000164 RID: 356
	public class NavalCompanionRolesCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06001780 RID: 6016 RVA: 0x000A07B1 File Offset: 0x0009E9B1
		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
		}

		// Token: 0x06001781 RID: 6017 RVA: 0x000A07CA File Offset: 0x0009E9CA
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06001782 RID: 6018 RVA: 0x000A07CC File Offset: 0x0009E9CC
		public void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.AddDialogs(campaignGameStarter);
		}

		// Token: 0x06001783 RID: 6019 RVA: 0x000A07D8 File Offset: 0x0009E9D8
		private void AddDialogs(CampaignGameStarter campaignGameStarter)
		{
			campaignGameStarter.AddPlayerLine("companion_becomes_first_mate", "companion_roles", "companion_okay", "{=FRTvNn9Q}I no longer need you as First Mate.", new ConversationSentence.OnConditionDelegate(this.companion_fire_first_mate_on_condition), new ConversationSentence.OnConsequenceDelegate(this.remove_first_mate_role_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("companion_becomes_navigator", "companion_roles", "companion_okay", "{=1dO4mgZI}I no longer need you as Navigator.", new ConversationSentence.OnConditionDelegate(this.companion_fire_navigator_on_condition), new ConversationSentence.OnConsequenceDelegate(this.remove_navigator_role_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("companion_becomes_first_mate_2", "companion_roles", "give_companion_roles", "{=fqva0OdY}First Mate {CURRENTLY_HELD_FIRST_MATE}", new ConversationSentence.OnConditionDelegate(this.companion_becomes_first_mate_on_condition), new ConversationSentence.OnConsequenceDelegate(this.companion_becomes_first_mate_on_consequence), 100, new ConversationSentence.OnClickableConditionDelegate(this.companion_becomes_first_mate_clickable_condition), null);
			campaignGameStarter.AddPlayerLine("companion_becomes_navigator_2", "companion_roles", "give_companion_roles", "{=jjISJIcf}Navigator {CURRENTLY_HELD_NAVIGATOR}", new ConversationSentence.OnConditionDelegate(this.companion_becomes_navigator_on_condition), new ConversationSentence.OnConsequenceDelegate(this.companion_becomes_navigator_on_consequence), 100, new ConversationSentence.OnClickableConditionDelegate(this.companion_becomes_navigator_clickable_condition), null);
			campaignGameStarter.AddPlayerLine("companion_becomes_first_mate_3", "too_many_roles_responses", "companion_okay_to_role_selection", "{=FRTvNn9Q}I no longer need you as First Mate.", new ConversationSentence.OnConditionDelegate(this.companion_fire_first_mate_on_condition), new ConversationSentence.OnConsequenceDelegate(this.remove_first_mate_role_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("companion_becomes_navigator_3", "too_many_roles_responses", "companion_okay_to_role_selection", "{=1dO4mgZI}I no longer need you as Navigator.", new ConversationSentence.OnConditionDelegate(this.companion_fire_navigator_on_condition), new ConversationSentence.OnConsequenceDelegate(this.remove_navigator_role_on_consequence), 100, null, null);
			campaignGameStarter.AddPlayerLine("tavernkeeper_companion_info_player_select_first_mate", "tavernkeeper_list_companion_types", "player_selected_companion_type", "{=bdMwsaY6}I need a first mate who can enforce discipline and keep the ship battle-ready.", null, new ConversationSentence.OnConsequenceDelegate(this.tavernkeeper_companion_info_player_select_first_mate_on_consequence), 100, new ConversationSentence.OnClickableConditionDelegate(NavalCompanionRolesCampaignBehavior.companion_type_select_clickable_condition), null);
			campaignGameStarter.AddPlayerLine("tavernkeeper_companion_info_player_select_navigator", "tavernkeeper_list_companion_types", "player_selected_companion_type", "{=bzoUl6DI}I need a navigator who knows winds, currents and coasts, and can help me sail swiftly.", null, new ConversationSentence.OnConsequenceDelegate(this.tavernkeeper_companion_info_player_select_navigator_on_consequence), 100, new ConversationSentence.OnClickableConditionDelegate(NavalCompanionRolesCampaignBehavior.companion_type_select_clickable_condition), null);
		}

		// Token: 0x06001784 RID: 6020 RVA: 0x000A09B3 File Offset: 0x0009EBB3
		private bool companion_becomes_first_mate_clickable_condition(out TextObject explanation)
		{
			return this.party_role_assignment_clickable_condition(14, out explanation);
		}

		// Token: 0x06001785 RID: 6021 RVA: 0x000A09C0 File Offset: 0x0009EBC0
		private bool companion_becomes_first_mate_on_condition()
		{
			Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
			Hero roleHolder = oneToOneConversationHero.PartyBelongedTo.GetRoleHolder(14);
			if (roleHolder != null)
			{
				TextObject textObject = new TextObject("{=QEp8t8u0}(Currently held by {COMPANION.LINK})", null);
				StringHelpers.SetCharacterProperties("COMPANION", roleHolder.CharacterObject, textObject, false);
				MBTextManager.SetTextVariable("CURRENTLY_HELD_FIRST_MATE", textObject, false);
			}
			else
			{
				MBTextManager.SetTextVariable("CURRENTLY_HELD_FIRST_MATE", "{=kNQMkh3j}(Currently unassigned)", false);
			}
			return roleHolder != oneToOneConversationHero;
		}

		// Token: 0x06001786 RID: 6022 RVA: 0x000A0A28 File Offset: 0x0009EC28
		private void companion_becomes_first_mate_on_consequence()
		{
			Hero.OneToOneConversationHero.PartyBelongedTo.SetPartyFirstMate(Hero.OneToOneConversationHero);
		}

		// Token: 0x06001787 RID: 6023 RVA: 0x000A0A3E File Offset: 0x0009EC3E
		private bool companion_becomes_navigator_clickable_condition(out TextObject explanation)
		{
			return this.party_role_assignment_clickable_condition(15, out explanation);
		}

		// Token: 0x06001788 RID: 6024 RVA: 0x000A0A4C File Offset: 0x0009EC4C
		private bool companion_becomes_navigator_on_condition()
		{
			Hero oneToOneConversationHero = Hero.OneToOneConversationHero;
			Hero roleHolder = oneToOneConversationHero.PartyBelongedTo.GetRoleHolder(15);
			if (roleHolder != null)
			{
				TextObject textObject = new TextObject("{=QEp8t8u0}(Currently held by {COMPANION.LINK})", null);
				StringHelpers.SetCharacterProperties("COMPANION", roleHolder.CharacterObject, textObject, false);
				MBTextManager.SetTextVariable("CURRENTLY_HELD_NAVIGATOR", textObject, false);
			}
			else
			{
				MBTextManager.SetTextVariable("CURRENTLY_HELD_NAVIGATOR", "{=kNQMkh3j}(Currently unassigned)", false);
			}
			return roleHolder != oneToOneConversationHero;
		}

		// Token: 0x06001789 RID: 6025 RVA: 0x000A0AB4 File Offset: 0x0009ECB4
		private void companion_becomes_navigator_on_consequence()
		{
			Hero.OneToOneConversationHero.PartyBelongedTo.SetPartyNavigator(Hero.OneToOneConversationHero);
		}

		// Token: 0x0600178A RID: 6026 RVA: 0x000A0ACA File Offset: 0x0009ECCA
		private bool companion_fire_first_mate_on_condition()
		{
			return this.CanFireHeroFromRole(14, Hero.OneToOneConversationHero);
		}

		// Token: 0x0600178B RID: 6027 RVA: 0x000A0AD9 File Offset: 0x0009ECD9
		private bool companion_fire_navigator_on_condition()
		{
			return this.CanFireHeroFromRole(15, Hero.OneToOneConversationHero);
		}

		// Token: 0x0600178C RID: 6028 RVA: 0x000A0AE8 File Offset: 0x0009ECE8
		private void remove_first_mate_role_on_consequence()
		{
			Hero.OneToOneConversationHero.PartyBelongedTo.RemovePartyRoleOfHero(Hero.OneToOneConversationHero, 14);
		}

		// Token: 0x0600178D RID: 6029 RVA: 0x000A0B00 File Offset: 0x0009ED00
		private void remove_navigator_role_on_consequence()
		{
			Hero.OneToOneConversationHero.PartyBelongedTo.RemovePartyRoleOfHero(Hero.OneToOneConversationHero, 15);
		}

		// Token: 0x0600178E RID: 6030 RVA: 0x000A0B18 File Offset: 0x0009ED18
		private bool party_role_assignment_clickable_condition(PartyRole role, out TextObject explanation)
		{
			bool flag = Campaign.Current.Models.ClanMemberPartyRoleModel.IsHeroAssignableForPartyRoleInParty(role, Hero.OneToOneConversationHero, Hero.OneToOneConversationHero.PartyBelongedTo);
			if (!flag)
			{
				explanation = new TextObject("{=zcTOL3gI}Not eligible for the role.", null);
				return flag;
			}
			explanation = TextObject.GetEmpty();
			return flag;
		}

		// Token: 0x0600178F RID: 6031 RVA: 0x000A0B56 File Offset: 0x0009ED56
		private bool CanFireHeroFromRole(PartyRole role, Hero hero)
		{
			return hero.PartyBelongedTo.GetRoleHolder(role) == hero && hero != hero.PartyBelongedTo.LeaderHero;
		}

		// Token: 0x06001790 RID: 6032 RVA: 0x000A0B7C File Offset: 0x0009ED7C
		private void tavernkeeper_companion_info_player_select_first_mate_on_consequence()
		{
			TavernEmployeesCampaignBehavior behavior = Campaign.Current.CampaignBehaviorManager.GetBehavior<TavernEmployeesCampaignBehavior>();
			if (behavior != null)
			{
				behavior.FindCompanionWithType(14);
				return;
			}
			Debug.FailedAssert("TavernEmployeesCampaignBehavior does not exist!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\CampaignBehaviors\\NavalCompanionRolesCampaignBehavior.cs", "tavernkeeper_companion_info_player_select_first_mate_on_consequence", 159);
		}

		// Token: 0x06001791 RID: 6033 RVA: 0x000A0BC0 File Offset: 0x0009EDC0
		private void tavernkeeper_companion_info_player_select_navigator_on_consequence()
		{
			TavernEmployeesCampaignBehavior behavior = Campaign.Current.CampaignBehaviorManager.GetBehavior<TavernEmployeesCampaignBehavior>();
			if (behavior != null)
			{
				behavior.FindCompanionWithType(15);
				return;
			}
			Debug.FailedAssert("TavernEmployeesCampaignBehavior does not exist!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\CampaignBehaviors\\NavalCompanionRolesCampaignBehavior.cs", "tavernkeeper_companion_info_player_select_navigator_on_consequence", 172);
		}

		// Token: 0x06001792 RID: 6034 RVA: 0x000A0C02 File Offset: 0x0009EE02
		private static bool companion_type_select_clickable_condition(out TextObject explanation)
		{
			explanation = new TextObject("{=!}{COMPANION_INQUIRY_COST}{GOLD_ICON}.", null);
			MBTextManager.SetTextVariable("COMPANION_INQUIRY_COST", 2);
			if (Hero.MainHero.Gold < 2)
			{
				explanation = new TextObject("{=xVZVYNan}You don't have enough{GOLD_ICON}.", null);
				return false;
			}
			return true;
		}
	}
}
