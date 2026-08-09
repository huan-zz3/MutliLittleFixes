using System;
using NavalDLC.Storyline;
using StoryMode;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.CampaignBehaviors
{
	// Token: 0x02000169 RID: 361
	public class NavalInitializationCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x060017C9 RID: 6089 RVA: 0x000A26E0 File Offset: 0x000A08E0
		public override void RegisterEvents()
		{
			if (Campaign.Current.CurrentGame.GameType is CampaignStoryMode)
			{
				StoryModeEvents.OnStealthTutorialActivatedEvent.AddNonSerializedListener(this, new Action(this.OnStealthTutorialActivated));
			}
			CampaignEvents.OnCharacterCreationIsOverEvent.AddNonSerializedListener(this, new Action(this.OnCharacterCreationIsOver));
			CampaignEvents.OnAfterSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnAfterSessionLaunched));
		}

		// Token: 0x060017CA RID: 6090 RVA: 0x000A2748 File Offset: 0x000A0948
		private void OnCharacterCreationIsOver()
		{
			if (!(Campaign.Current.CurrentGame.GameType is CampaignStoryMode) && !this._hasIntroductionPopUpBeenShown)
			{
				object obj = new TextObject("{=SJT8Nl5a}Call of the Oceans", null);
				TextObject textObject = new TextObject("{=XcaoQSjv}Often, when you were growing up, you wondered if your destiny might lie upon the sea. You listened closely to old sailors telling their tales of daring and peril: steering longships through the icy storms of the north, outfoxing corsairs along the pirate-infested coasts of the south, or standing in the forecastle of a dromon as it crashed through the enemy battle-line. You wonder what opportunities lie for you to seize on the seas of Calradia: a fortune made in the commerce and intrigues of a bustling port, or glory won on the bloodied deck of a foe's flagship.", null);
				TextObject textObject2 = new TextObject("{=DM6luo3c}Continue", null);
				InformationManager.ShowInquiry(new InquiryData(obj.ToString(), textObject.ToString(), true, false, textObject2.ToString(), null, null, null, "", 0f, null, null, null), false, false);
			}
			Hero.MainHero.HeroDeveloper.UnspentFocusPoints += 6;
		}

		// Token: 0x060017CB RID: 6091 RVA: 0x000A27DC File Offset: 0x000A09DC
		private void OnStealthTutorialActivated()
		{
			this.ShowIntroductionPopUp();
		}

		// Token: 0x060017CC RID: 6092 RVA: 0x000A27E4 File Offset: 0x000A09E4
		private void ShowIntroductionPopUp()
		{
			this._hasIntroductionPopUpBeenShown = true;
			object obj = new TextObject("{=F6qA5Mmo}Troubled Waters", null);
			TextObject textObject = new TextObject("{=Iq2YN7o3}Throughout your travels, you overheard hushed conversations about a new menace, a pirate confederacy terrorizing the coasts of Calradia. These northern corsairs have built a reputation for trading in captives from bandits and raiders such as the ones who attacked your family. Do you want to go to Ostican now to try to pick up your sister's trail and embark on an hunt across the seas of Calradia?", null);
			TextObject textObject2 = new TextObject("{=0aD2pdmB}Take me to Ostican now", null);
			TextObject textObject3 = new TextObject("{=fRRkHsZR}I'll go there myself", null);
			InformationManager.ShowInquiry(new InquiryData(obj.ToString(), textObject.ToString(), true, true, textObject2.ToString(), textObject3.ToString(), new Action(this.OnStorylinePopUpAccepted), new Action(this.OnStorylinePopUpDeclined), "", 0f, null, null, null), true, false);
		}

		// Token: 0x060017CD RID: 6093 RVA: 0x000A2871 File Offset: 0x000A0A71
		private void OnStorylinePopUpAccepted()
		{
			NavalStorylineData.StartNavalStoryline();
			NavalStorylineData.TeleportMainPartyBackToBase();
		}

		// Token: 0x060017CE RID: 6094 RVA: 0x000A287D File Offset: 0x000A0A7D
		private void OnStorylinePopUpDeclined()
		{
			NavalStorylineData.StartNavalStoryline();
		}

		// Token: 0x060017CF RID: 6095 RVA: 0x000A2884 File Offset: 0x000A0A84
		private void OnAfterSessionLaunched(CampaignGameStarter gameStarter)
		{
			if (MBSaveLoad.IsUpdatingGameVersion && MBSaveLoad.LastLoadedGameVersion.IsOlderThan(ApplicationVersion.FromString("v1.3.12", 0)))
			{
				foreach (Hero hero in Hero.AllAliveHeroes)
				{
					hero.HeroDeveloper.UnspentFocusPoints += 6;
				}
			}
		}

		// Token: 0x060017D0 RID: 6096 RVA: 0x000A2904 File Offset: 0x000A0B04
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<bool>("_hasIntroductionPopUpBeenShown", ref this._hasIntroductionPopUpBeenShown);
		}

		// Token: 0x04000BE7 RID: 3047
		private bool _hasIntroductionPopUpBeenShown;
	}
}
