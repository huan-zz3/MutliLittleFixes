using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.Localization;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 百科家族列表页「状态」筛选组新增「在流亡 / 不在流亡」两个筛选项（与 Eliminated/Active 并列）。
    ///
    /// 流亡定义：对齐原版 FactionDiscontinuationCampaignBehavior.CanClanBeDiscontinued
    /// （即本 Mod WanderingClanSurvivalPatch 所针对的"无国无地流浪家族"）：
    /// 无王国 + 无定居点 + 非叛军/土匪/小派系，并排除玩家家族。
    ///
    /// 实现：Postfix 拦截基类 EncyclopediaPage.GetFilterItems()——该方法在每次打开列表页、
    /// 每个列表项过滤检查、每次切换筛选时都会被调用（实时生效，无缓存）。
    /// 命中家族页时，向原版缓存的 Status 筛选组追加我们的两个筛选项；开关关闭时移除。
    ///
    /// 注意（已踩坑）：Postfix 必须是 **void**。若返回非 void（如 IEnumerable&lt;T&gt;），Harmony 2.4.x
    /// 会将其视为 pass-through postfix，要求第一个参数必须是 __result 且类型与返回类型一致，
    /// 否则 harmony.Patch() 在启动注册时直接抛 System.Exception（"Return type of pass through postfix
    /// ... does not match type of its first parameter"），游戏启动即崩溃。
    /// 本项目采用 void + 原地修改：筛选项直接挂到原版缓存的 Status 组上，Postfix 只改该组内层
    /// Filters（不动外层列表，无枚举期修改异常），返回的原版 __result 即已含新项。
    ///
    /// 说明：
    /// - Status 组识别：家族页原版筛选组为 Type(1 项)/Diplomacy(3 项)/Status(2 项)/Culture(≥3 项)，
    ///   唯独 Status 恰好 2 项（Eliminated/Active），故以 Count==2 识别；已追加后以"含我们的项"识别。
    /// - 筛选项挂在原版缓存的 Status 组上，实例跨页稳定，选择状态可像原版项一样跨页保存恢复；
    ///   关开关后移除（下一次打开列表页生效），再开则新建（选择状态重置，可接受）。
    ///
    /// 不带 [HarmonyPatch] 属性，由 HarmonyPatchRegistry 显式注册。
    /// </summary>
    internal static class EncyclopediaClanExileFilterPatch
    {
        private const string InExileText = "在流亡";
        private const string NotInExileText = "不在流亡";

        internal static void Postfix(EncyclopediaPage __instance, IEnumerable<EncyclopediaFilterGroup> __result)
        {
            // MCM 运行时开关 — 非家族页直接跳过（对其他百科页面零影响）
            if (!__instance.HasIdentifierType(typeof(Clan)))
            {
                return;
            }

            bool enabled = Settings.Instance?.EncyclopediaClanExileFilter == true;

            foreach (EncyclopediaFilterGroup group in __result)
            {
                if (IsStatusGroup(group))
                {
                    if (enabled)
                    {
                        AddExileFiltersIfMissing(group);
                    }
                    else
                    {
                        RemoveExileFilters(group);
                    }
                    break;
                }
            }
        }

        /// <summary>原版家族页 Status 组：唯独它恰好 2 个筛选项；追加后以是否已含我们的项识别。</summary>
        private static bool IsStatusGroup(EncyclopediaFilterGroup group)
        {
            return group.Filters.Count == 2 || group.Filters.Exists(IsExileFilter);
        }

        private static void AddExileFiltersIfMissing(EncyclopediaFilterGroup group)
        {
            if (group.Filters.Exists(IsExileFilter))
            {
                return;
            }
            group.Filters.Add(new EncyclopediaFilterItem(new TextObject(InExileText),
                f => f is Clan clan && IsClanInExile(clan)));
            group.Filters.Add(new EncyclopediaFilterItem(new TextObject(NotInExileText),
                f => f is Clan clan && !IsClanInExile(clan)));
        }

        private static void RemoveExileFilters(EncyclopediaFilterGroup group)
        {
            group.Filters.RemoveAll(IsExileFilter);
        }

        private static bool IsExileFilter(EncyclopediaFilterItem item)
        {
            return item.Name.Value == InExileText || item.Name.Value == NotInExileText;
        }

        /// <summary>
        /// 流亡判定：无王国 + 无定居点 + 非叛军/土匪/小派系 + 非玩家家族。
        /// 与原版 CanClanBeDiscontinued 的条件完全一致（按需求同样排除玩家家族）。
        /// </summary>
        private static bool IsClanInExile(Clan clan)
        {
            return clan != Clan.PlayerClan
                && clan.Kingdom == null
                && !clan.IsRebelClan
                && !clan.IsBanditFaction
                && !clan.IsMinorFaction
                && clan.Settlements.Count == 0;
        }
    }
}
