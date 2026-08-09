using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Core;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000106 RID: 262
	public class NavalCharacterDevelopmentModel : DefaultCharacterDevelopmentModel
	{
		// Token: 0x17000338 RID: 824
		// (get) Token: 0x0600131C RID: 4892 RVA: 0x0008BFFB File Offset: 0x0008A1FB
		public override int MaxAttribute
		{
			get
			{
				return base.BaseModel.MaxAttribute;
			}
		}

		// Token: 0x17000339 RID: 825
		// (get) Token: 0x0600131D RID: 4893 RVA: 0x0008C008 File Offset: 0x0008A208
		public override int MaxFocusPerSkill
		{
			get
			{
				return base.BaseModel.MaxFocusPerSkill;
			}
		}

		// Token: 0x1700033A RID: 826
		// (get) Token: 0x0600131E RID: 4894 RVA: 0x0008C015 File Offset: 0x0008A215
		public override int MaxSkillRequiredForEpicPerkBonus
		{
			get
			{
				return base.BaseModel.MaxSkillRequiredForEpicPerkBonus;
			}
		}

		// Token: 0x1700033B RID: 827
		// (get) Token: 0x0600131F RID: 4895 RVA: 0x0008C022 File Offset: 0x0008A222
		public override int MinSkillRequiredForEpicPerkBonus
		{
			get
			{
				return base.BaseModel.MinSkillRequiredForEpicPerkBonus;
			}
		}

		// Token: 0x1700033C RID: 828
		// (get) Token: 0x06001320 RID: 4896 RVA: 0x0008C02F File Offset: 0x0008A22F
		public override int FocusPointsPerLevel
		{
			get
			{
				return base.BaseModel.FocusPointsPerLevel;
			}
		}

		// Token: 0x1700033D RID: 829
		// (get) Token: 0x06001321 RID: 4897 RVA: 0x0008C03C File Offset: 0x0008A23C
		public override int FocusPointsAtStart
		{
			get
			{
				return base.BaseModel.FocusPointsAtStart + 6;
			}
		}

		// Token: 0x1700033E RID: 830
		// (get) Token: 0x06001322 RID: 4898 RVA: 0x0008C04B File Offset: 0x0008A24B
		public override int AttributePointsAtStart
		{
			get
			{
				return base.BaseModel.AttributePointsAtStart;
			}
		}

		// Token: 0x1700033F RID: 831
		// (get) Token: 0x06001323 RID: 4899 RVA: 0x0008C058 File Offset: 0x0008A258
		public override int LevelsPerAttributePoint
		{
			get
			{
				return base.BaseModel.LevelsPerAttributePoint;
			}
		}

		// Token: 0x06001324 RID: 4900 RVA: 0x0008C065 File Offset: 0x0008A265
		public override ExplainedNumber CalculateLearningLimit(IReadOnlyPropertyOwner<CharacterAttribute> characterAttributes, int focusValue, SkillObject skill, bool includeDescriptions = false)
		{
			return base.BaseModel.CalculateLearningLimit(characterAttributes, focusValue, skill, includeDescriptions);
		}

		// Token: 0x06001325 RID: 4901 RVA: 0x0008C077 File Offset: 0x0008A277
		public override ExplainedNumber CalculateLearningRate(IReadOnlyPropertyOwner<CharacterAttribute> characterAttributes, int focusValue, int skillValue, SkillObject skill, bool includeDescriptions = false)
		{
			return base.BaseModel.CalculateLearningRate(characterAttributes, focusValue, skillValue, skill, includeDescriptions);
		}

		// Token: 0x06001326 RID: 4902 RVA: 0x0008C08B File Offset: 0x0008A28B
		public override int GetMaxSkillPoint()
		{
			return base.BaseModel.GetMaxSkillPoint();
		}

		// Token: 0x06001327 RID: 4903 RVA: 0x0008C098 File Offset: 0x0008A298
		public override CharacterAttribute GetNextAttributeToUpgrade(Hero hero)
		{
			return base.BaseModel.GetNextAttributeToUpgrade(hero);
		}

		// Token: 0x06001328 RID: 4904 RVA: 0x0008C0A6 File Offset: 0x0008A2A6
		public override PerkObject GetNextPerkToChoose(Hero hero, PerkObject perk)
		{
			return base.BaseModel.GetNextPerkToChoose(hero, perk);
		}

		// Token: 0x06001329 RID: 4905 RVA: 0x0008C0B5 File Offset: 0x0008A2B5
		public override SkillObject GetNextSkillToAddFocus(Hero hero)
		{
			return base.BaseModel.GetNextSkillToAddFocus(hero);
		}

		// Token: 0x0600132A RID: 4906 RVA: 0x0008C0C3 File Offset: 0x0008A2C3
		public override int GetSkillLevelChange(Hero hero, SkillObject skill, float skillXp)
		{
			return base.BaseModel.GetSkillLevelChange(hero, skill, skillXp);
		}

		// Token: 0x0600132B RID: 4907 RVA: 0x0008C0D3 File Offset: 0x0008A2D3
		public override void GetTraitLevelForTraitXp(Hero hero, TraitObject trait, int newValue, out int traitLevel, out int traitXp)
		{
			base.BaseModel.GetTraitLevelForTraitXp(hero, trait, newValue, ref traitLevel, ref traitXp);
		}

		// Token: 0x0600132C RID: 4908 RVA: 0x0008C0E7 File Offset: 0x0008A2E7
		public override int GetTraitXpRequiredForTraitLevel(TraitObject trait, int traitLevel)
		{
			return base.BaseModel.GetTraitXpRequiredForTraitLevel(trait, traitLevel);
		}

		// Token: 0x0600132D RID: 4909 RVA: 0x0008C0F6 File Offset: 0x0008A2F6
		public override int GetXpAmountForSkillLevelChange(Hero hero, SkillObject skill, int skillLevelChange)
		{
			return base.BaseModel.GetXpAmountForSkillLevelChange(hero, skill, skillLevelChange);
		}

		// Token: 0x0600132E RID: 4910 RVA: 0x0008C106 File Offset: 0x0008A306
		public override int GetXpRequiredForSkillLevel(int skillLevel)
		{
			return base.BaseModel.GetXpRequiredForSkillLevel(skillLevel);
		}

		// Token: 0x0600132F RID: 4911 RVA: 0x0008C114 File Offset: 0x0008A314
		public override int SkillsRequiredForLevel(int level)
		{
			return base.BaseModel.SkillsRequiredForLevel(level);
		}

		// Token: 0x04000ABD RID: 2749
		public const int AdditionalFocusPointsAtStart = 6;
	}
}
