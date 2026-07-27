using System;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000115 RID: 277
	public class NavalDLCCombatXpModel : CombatXpModel
	{
		// Token: 0x060013D1 RID: 5073 RVA: 0x0008EDC4 File Offset: 0x0008CFC4
		public override SkillObject GetSkillForWeapon(WeaponComponentData weapon, bool isSiegeEngineHit)
		{
			return base.BaseModel.GetSkillForWeapon(weapon, isSiegeEngineHit);
		}

		// Token: 0x060013D2 RID: 5074 RVA: 0x0008EDD4 File Offset: 0x0008CFD4
		public override ExplainedNumber GetXpFromHit(CharacterObject attackerTroop, CharacterObject captain, CharacterObject attackedTroop, PartyBase attackerParty, int damage, bool isFatal, CombatXpModel.MissionTypeEnum missionType)
		{
			ExplainedNumber xpFromHit = base.BaseModel.GetXpFromHit(attackerTroop, captain, attackedTroop, attackerParty, damage, isFatal, missionType);
			if (((attackerParty != null) ? attackerParty.MapEvent : null) != null)
			{
				if (attackerParty.MapEvent.IsNavalMapEvent)
				{
					if (!attackerTroop.IsHero)
					{
						xpFromHit.AddFactor(0.5f, null);
					}
					else if (attackerTroop.HeroObject.CompanionOf != null && attackerParty.IsMobile)
					{
						PerkHelper.AddPerkBonusForParty(NavalPerks.Mariner.NavalFightingTraining, attackerParty.MobileParty, true, ref xpFromHit, false);
					}
				}
				Hero leaderHero = attackerParty.LeaderHero;
				bool flag;
				if (leaderHero == null)
				{
					flag = null != null;
				}
				else
				{
					Clan clan = leaderHero.Clan;
					flag = ((clan != null) ? clan.Kingdom : null) != null;
				}
				if (flag && attackerParty.LeaderHero.Clan.Kingdom.HasPolicy(NavalPolicies.FraternalFleetDoctrine))
				{
					xpFromHit.AddFactor(-0.15f, NavalPolicies.FraternalFleetDoctrine.Name);
				}
			}
			return xpFromHit;
		}

		// Token: 0x060013D3 RID: 5075 RVA: 0x0008EEB0 File Offset: 0x0008D0B0
		public override float GetXpMultiplierFromShotDifficulty(float shotDifficulty)
		{
			return base.BaseModel.GetXpMultiplierFromShotDifficulty(shotDifficulty);
		}

		// Token: 0x17000359 RID: 857
		// (get) Token: 0x060013D4 RID: 5076 RVA: 0x0008EEBE File Offset: 0x0008D0BE
		public override float CaptainRadius
		{
			get
			{
				return base.BaseModel.CaptainRadius;
			}
		}

		// Token: 0x04000AC6 RID: 2758
		private const float NavalXPBonusForNonHeroTroops = 0.5f;
	}
}
