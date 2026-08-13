using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Roster;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 部队界面直接移除俘虏时也给予好感度（走原版对话释放的 +4 好感流程）。
    ///
    /// 原版行为：在部队界面把英雄俘虏从右侧（主部队）拖到左侧空白处再点「完成」，
    /// PartyScreenHelper.DefaultDoneHandler → HandleReleasedAndTakenPrisoners 会调用
    /// EndCaptivityAction.ApplyByReleasedByChoice 释放俘虏，但全程不调用
    /// ChangeRelationAction —— 而对话释放（LordConversationsCampaignBehavior 的
    /// conversation_player_let_prisoner_go_on_consequence）在释放后显式 +4 好感。
    ///
    /// 本补丁在 HandleReleasedAndTakenPrisoners 之后（Postfix）对 releasedPrisonerRoster
    /// 中的每个英雄补上与对话释放完全一致的 ChangeRelationAction.ApplyPlayerRelation(hero, 4)。
    ///
    /// 说明：
    /// - releasedPrisonerRoster 只包含「从主部队移除并释放」的俘虏；捐赠到地牢/赎金/
    ///   战后战利品等界面各有独立的 DoneHandler，不会经过本方法，故不会误伤。
    /// - Postfix 声明为 void（Harmony 2.4.x pass-through 陷阱，见 AGENTS.md §1.4）。
    /// - 不缓存 MCM 开关，每次调用实时读取（AGENTS.md §2.1）。
    ///
    /// 不带 [HarmonyPatch] 属性，由 HarmonyPatchRegistry 显式注册。
    /// </summary>
    internal static class PrisonerRemoveRelationPatch
    {
        // 与原版 LordConversationsCampaignBehavior.PlayerReleasesPrisonerRelationChange 一致
        private const int PlayerReleasesPrisonerRelationChange = 4;

        internal static void Postfix(FlattenedTroopRoster releasedPrisonerRoster)
        {
            // MCM 运行时开关 — 关闭时不干预
            if (Settings.Instance?.PrisonerRemoveRelationEnabled != true)
                return;

            if (releasedPrisonerRoster == null)
                return;

            foreach (FlattenedTroopRosterElement element in releasedPrisonerRoster)
            {
                CharacterObject character = element.Troop;
                if (character != null
                    && character.IsHero
                    && character.HeroObject != null
                    && character.HeroObject != Hero.MainHero)
                {
                    ChangeRelationAction.ApplyPlayerRelation(character.HeroObject, PlayerReleasesPrisonerRelationChange);
                }
            }
        }
    }
}
