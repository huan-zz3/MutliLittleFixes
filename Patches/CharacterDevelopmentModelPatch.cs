using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace ExampleMod.Patches
{
    [HarmonyPatch(typeof(DefaultCharacterDevelopmentModel), "CalculateLearningRate")]
    internal static class CharacterDevelopmentModelPatch
    {
        private static readonly TextObject _modText = new TextObject("ExampleMod 属性倍率");

        internal static void Postfix(
            DefaultCharacterDevelopmentModel __instance,
            IReadOnlyPropertyOwner<CharacterAttribute> characterAttributes,
            int focusValue,
            int skillValue,
            SkillObject skill,
            bool includeDescriptions,
            ref ExplainedNumber __result)
        {
            // 仅对主角生效
            if (Hero.MainHero?.CharacterAttributes != characterAttributes)
                return;

            Settings? settings = Settings.Instance;
            if (settings == null)
                return;

            CharacterAttribute[] attributes = skill.Attributes;
            if (attributes == null || attributes.Length == 0)
                return;

            // 计算额外的属性倍率贡献因子
            // 原始: 0.4 * sum(attr[i]) / len(attrs)
            // 修正后: 0.4 * sum(attr[i] * mult[i]) / len(attrs)
            // 额外因子 = 0.4 * sum(attr[i] * (mult[i] - 1)) / len(attrs)
            float weightedExtra = 0f;
            foreach (CharacterAttribute attr in attributes)
            {
                float attrValue = characterAttributes.GetPropertyValue(attr);
                float multiplier = settings.GetAttributeMultiplier(attr);
                weightedExtra += attrValue * (multiplier - 1f);
            }
            float extraFactor = 0.4f * weightedExtra / attributes.Length;

            if (extraFactor > 0.001f || extraFactor < -0.001f)
            {
                __result.AddFactor(extraFactor, _modText);
            }
        }
    }
}
