using System;
using NavalDLC.Storyline;
using NavalDLC.Storyline.Quests;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.GameComponents
{
	// Token: 0x0200012D RID: 301
	public class NavalDLCSettlementAccessModel : SettlementAccessModel
	{
		// Token: 0x060014C5 RID: 5317 RVA: 0x00092D24 File Offset: 0x00090F24
		public override bool CanMainHeroAccessLocation(Settlement settlement, string locationId, out bool disableOption, out TextObject disabledText)
		{
			if (locationId.Equals("center"))
			{
				if (NavalStorylineData.IsNavalStoryLineActive())
				{
					disableOption = true;
					disabledText = new TextObject("{=ILnr9eCQ}Door is locked!", null);
					return false;
				}
			}
			else if (locationId == "port")
			{
				return this.CanMainHeroEnterPort(settlement, out disabledText, out disableOption);
			}
			return base.BaseModel.CanMainHeroAccessLocation(settlement, locationId, ref disableOption, ref disabledText);
		}

		// Token: 0x060014C6 RID: 5318 RVA: 0x00092D80 File Offset: 0x00090F80
		public override void CanMainHeroEnterSettlement(Settlement settlement, out SettlementAccessModel.AccessDetails accessDetails)
		{
			base.BaseModel.CanMainHeroEnterSettlement(settlement, ref accessDetails);
		}

		// Token: 0x060014C7 RID: 5319 RVA: 0x00092D8F File Offset: 0x00090F8F
		public override void CanMainHeroEnterLordsHall(Settlement settlement, out SettlementAccessModel.AccessDetails accessDetails)
		{
			base.BaseModel.CanMainHeroEnterLordsHall(settlement, ref accessDetails);
		}

		// Token: 0x060014C8 RID: 5320 RVA: 0x00092D9E File Offset: 0x00090F9E
		public override void CanMainHeroEnterDungeon(Settlement settlement, out SettlementAccessModel.AccessDetails accessDetails)
		{
			base.BaseModel.CanMainHeroEnterDungeon(settlement, ref accessDetails);
		}

		// Token: 0x060014C9 RID: 5321 RVA: 0x00092DAD File Offset: 0x00090FAD
		public override bool CanMainHeroDoSettlementAction(Settlement settlement, SettlementAccessModel.SettlementAction settlementAction, out bool disableOption, out TextObject disabledText)
		{
			if (settlement.IsVillage && MobileParty.MainParty.IsCurrentlyAtSea && settlementAction == 6)
			{
				disableOption = true;
				disabledText = new TextObject("{=qVbAvzJM}You cannot wait in the village while you are at sea.", null);
				return false;
			}
			return base.BaseModel.CanMainHeroDoSettlementAction(settlement, settlementAction, ref disableOption, ref disabledText);
		}

		// Token: 0x060014CA RID: 5322 RVA: 0x00092DEA File Offset: 0x00090FEA
		public override bool IsRequestMeetingOptionAvailable(Settlement settlement, out bool disableOption, out TextObject disabledText)
		{
			return base.BaseModel.IsRequestMeetingOptionAvailable(settlement, ref disableOption, ref disabledText);
		}

		// Token: 0x060014CB RID: 5323 RVA: 0x00092DFC File Offset: 0x00090FFC
		private bool CanMainHeroEnterPort(Settlement settlement, out TextObject disabledText, out bool disableOption)
		{
			bool flag = true;
			disabledText = TextObject.GetEmpty();
			disableOption = false;
			if (Settlement.CurrentSettlement == NavalStorylineData.HomeSettlement && Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(SpeakToGunnarAndSisterQuest)) && Mission.Current != null)
			{
				flag = false;
				disableOption = true;
				disabledText = new TextObject("{=UjERCi2F}This feature is disabled.", null);
			}
			else if (Campaign.Current.IsMainHeroDisguised)
			{
				if (Mission.Current == null)
				{
					disabledText = new TextObject("{=i1npbbc4}You cannot enter the port while in disguise.", null);
				}
				else
				{
					disabledText = new TextObject("{=ILnr9eCQ}Door is locked!", null);
				}
				flag = false;
				disableOption = true;
			}
			return flag;
		}
	}
}
