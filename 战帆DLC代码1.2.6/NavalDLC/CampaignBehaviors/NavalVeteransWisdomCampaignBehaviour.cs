using System;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace NavalDLC.CampaignBehaviors
{
	// Token: 0x02000170 RID: 368
	public class NavalVeteransWisdomCampaignBehaviour : CampaignBehaviorBase
	{
		// Token: 0x06001824 RID: 6180 RVA: 0x000A49BA File Offset: 0x000A2BBA
		public override void RegisterEvents()
		{
			CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.OnDailyTickParty));
			CampaignEvents.PerkOpenedEvent.AddNonSerializedListener(this, new Action<Hero, PerkObject>(this.OnPerkOpened));
		}

		// Token: 0x06001825 RID: 6181 RVA: 0x000A49EA File Offset: 0x000A2BEA
		private void OnPerkOpened(Hero hero, PerkObject perk)
		{
			if (hero == Hero.MainHero && (perk == NavalPerks.Boatswain.NavalHorde || perk == NavalPerks.Boatswain.Optimization || perk == NavalPerks.Boatswain.GildedPurse))
			{
				MobileParty.MainParty.ItemRoster.UpdateVersion();
			}
		}

		// Token: 0x06001826 RID: 6182 RVA: 0x000A4A1C File Offset: 0x000A2C1C
		private void OnDailyTickParty(MobileParty party)
		{
			if (party.HasPerk(NavalPerks.Boatswain.VeteransWisdom, false))
			{
				int level = party.GetEffectiveRoleHolder(5).Level;
				foreach (TroopRosterElement troopRosterElement in party.MemberRoster.GetTroopRoster())
				{
					if (troopRosterElement.Character.IsHero && troopRosterElement.Character.HeroObject.CompanionOf == party.ActualClan)
					{
						float randomFloat = MBRandom.RandomFloat;
						SkillObject skillObject;
						if (randomFloat < 0.33f)
						{
							skillObject = NavalSkills.Mariner;
						}
						else if (randomFloat < 0.66f)
						{
							skillObject = NavalSkills.Boatswain;
						}
						else
						{
							skillObject = NavalSkills.Shipmaster;
						}
						troopRosterElement.Character.HeroObject.AddSkillXp(skillObject, NavalPerks.Boatswain.VeteransWisdom.PrimaryBonus * (float)level);
					}
				}
			}
		}

		// Token: 0x06001827 RID: 6183 RVA: 0x000A4B04 File Offset: 0x000A2D04
		public override void SyncData(IDataStore dataStore)
		{
		}
	}
}
