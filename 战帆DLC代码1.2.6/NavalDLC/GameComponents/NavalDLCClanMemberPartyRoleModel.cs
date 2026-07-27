using System;
using System.Collections.Generic;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000111 RID: 273
	public class NavalDLCClanMemberPartyRoleModel : ClanMemberPartyRoleModel
	{
		// Token: 0x17000358 RID: 856
		// (get) Token: 0x060013B2 RID: 5042 RVA: 0x0008E41F File Offset: 0x0008C61F
		public override int MaximumPartyRoleAssignmentCount
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x060013B3 RID: 5043 RVA: 0x0008E422 File Offset: 0x0008C622
		public override IEnumerable<PartyRole> GetAssignablePartyRoles()
		{
			yield return 10;
			yield return 9;
			yield return 7;
			yield return 8;
			yield return 14;
			yield return 15;
			yield break;
		}

		// Token: 0x060013B4 RID: 5044 RVA: 0x0008E42B File Offset: 0x0008C62B
		public override SkillObject GetRelevantSkillForPartyRole(PartyRole role)
		{
			if (role == 14)
			{
				return NavalSkills.Boatswain;
			}
			if (role == 15)
			{
				return NavalSkills.Shipmaster;
			}
			return base.BaseModel.GetRelevantSkillForPartyRole(role);
		}

		// Token: 0x060013B5 RID: 5045 RVA: 0x0008E44F File Offset: 0x0008C64F
		public override bool IsHeroAssignableForPartyRole(Hero hero, PartyRole role, MobileParty party)
		{
			return base.BaseModel.IsHeroAssignableForPartyRole(hero, role, party);
		}

		// Token: 0x060013B6 RID: 5046 RVA: 0x0008E460 File Offset: 0x0008C660
		public override bool DoesHeroHaveEnoughSkillForPartyRole(Hero hero, PartyRole role, MobileParty party)
		{
			if (party.GetHeroPartyRoles(hero).Contains(role))
			{
				return true;
			}
			if (role == 14 || role == 15)
			{
				return Campaign.Current.Models.ClanMemberPartyRoleModel.IsHeroAssignableForPartyRoleInParty(role, hero, party);
			}
			return base.BaseModel.DoesHeroHaveEnoughSkillForPartyRole(hero, role, party);
		}

		// Token: 0x060013B7 RID: 5047 RVA: 0x0008E4AE File Offset: 0x0008C6AE
		public override bool IsHeroAssignableForPartyRoleInParty(PartyRole role, Hero hero, MobileParty party)
		{
			return base.BaseModel.IsHeroAssignableForPartyRoleInParty(role, hero, party);
		}
	}
}
