using System;
using NavalDLC.Storyline;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000119 RID: 281
	public class NavalDLCHeroAgentLocationModel : HeroAgentLocationModel
	{
		// Token: 0x06001419 RID: 5145 RVA: 0x0008FD0C File Offset: 0x0008DF0C
		public override Location GetLocationForHero(Hero hero, Settlement settlement, out HeroAgentLocationModel.HeroLocationDetail heroSpawnDetail)
		{
			if (NavalStorylineData.IsNavalStorylineHero(hero))
			{
				if (NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.Act3Quest5) && hero == NavalStorylineData.Gunnar && settlement.IsVillage && hero.Occupation == 31)
				{
					heroSpawnDetail = 6;
					return settlement.LocationComplex.GetLocationWithId("village_center");
				}
				heroSpawnDetail = 2;
				if (NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.None) && hero == NavalStorylineData.Purig && !hero.IsDead)
				{
					return settlement.LocationComplex.GetLocationWithId("tavern");
				}
				if (hero == NavalStorylineData.Gunnar && NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.None))
				{
					return null;
				}
				return settlement.LocationComplex.GetLocationWithId("port");
			}
			else
			{
				if (NavalStorylineData.IsNavalStoryLineActive())
				{
					heroSpawnDetail = 0;
					return null;
				}
				return base.BaseModel.GetLocationForHero(hero, settlement, ref heroSpawnDetail);
			}
		}

		// Token: 0x0600141A RID: 5146 RVA: 0x0008FDC3 File Offset: 0x0008DFC3
		public override bool WillBeListedInOverlay(LocationCharacter locationCharacter)
		{
			return (NavalStorylineData.IsNavalStoryLineActive() && locationCharacter.Character.IsHero && NavalStorylineData.IsNavalStorylineHero(locationCharacter.Character.HeroObject)) || base.BaseModel.WillBeListedInOverlay(locationCharacter);
		}
	}
}
