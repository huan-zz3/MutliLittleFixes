using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 在部队界面的俘虏标签页中，为特殊 NPC 英雄的名字后添加中文标注：
    /// - 一国统治者 → "统治者"
    /// - 一国领主（氏族族长） → "领主"
    /// - 雇佣兵头子 → "雇佣兵头"
    /// </summary>
    internal static class PrisonerSpecialLabelPatch
    {
        internal static void Postfix(PartyCharacterVM __instance)
        {
            // MCM 运行时开关 — 关闭时不执行任何操作
            if (Settings.Instance?.PrisonerSpecialLabel != true)
                return;

            // 只处理俘虏标签页中的英雄角色
            if (__instance.Type != PartyScreenLogic.TroopType.Prisoner)
                return;

            var character = __instance.Character;
            if (character?.IsHero != true)
                return;

            var hero = character.HeroObject;
            if (hero?.Clan == null)
                return;

            var label = GetSpecialLabel(hero);
            if (!string.IsNullOrEmpty(label))
            {
                // RefreshValues() 每次都会重新设置 Name = Troop.Character.Name.ToString()
                // 所以 postfix 追加不会重复累积
                __instance.Name += $" ({label})";
            }
        }

        /// <summary>
        /// 根据英雄的身份返回对应的中文标注。
        /// 只标注氏族族长（Clan.Leader），其他成员不标注。
        /// </summary>
        private static string? GetSpecialLabel(Hero hero)
        {
            var clan = hero.Clan;
            if (clan == null)
                return null;

            // 非族长不标注
            if (clan.Leader != hero)
                return null;

            // 统治者：该氏族是某个王国的统治氏族
            if (clan.Kingdom?.RulingClan == clan)
                return "统治者";

            // 雇佣兵头：该氏族是雇佣兵类型的小派系
            if (clan.IsClanTypeMercenary)
                return "雇佣兵头";

            // 领主：该氏族属于某个王国且是正规贵族氏族（非小派系）
            if (clan.Kingdom != null && clan.IsNoble && !clan.IsMinorFaction)
                return "领主";

            return null;
        }
    }
}
