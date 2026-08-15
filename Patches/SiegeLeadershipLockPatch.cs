using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 玩家攻城指挥权锁定：玩家先发起围攻（单独或带领军团）时，围城指挥权始终留在玩家手上，
    /// 中途加入的我方军团/国王无法把指挥权夺走。
    ///
    /// 原版行为（BesiegerCamp.AddSiegePartyInternal，BesiegerCamp.cs:200-210）：每次有部队
    /// 加入围攻，都用 EncounterModel.GetLeaderOfSiegeEvent 重新计算 _leaderParty —— 其内部
    /// GetLeaderOfEventInternal（DefaultEncounterModel.cs:138-196）按
    /// 国王 &gt; 军团领队 &gt; 普通领主（同级比军团战力/单队兵力）的优先级选指挥者。
    /// 玩家单独围城时属于「普通领主」档位，我方军团领队一到就会被夺走指挥权
    /// （围城菜单「发起进攻」禁用、大地图围城界面锁死、描述变为「XXX 正在指挥围攻部队」）。
    ///
    /// 本补丁（3 个挂钩，均显式注册于 HarmonyPatchRegistry）：
    /// 1. AddSiegePartyInternal Postfix —— 数据层兜底：玩家是第一个围攻者时，把
    ///    _leaderParty / _faction 强制覆写为玩家（即使原版重算/其他路径先写了 AI 领主）。
    /// 2. GetLeaderOfSiegeEvent Postfix —— UI/菜单层：围城菜单条件、大地图围城界面、
    ///    围城描述文本都通过该方法判断「谁是指挥官」，强制返回玩家；
    ///    同时 AddSiegePartyInternal 的原版重算也走此方法，双保险。
    /// 3. ChangeSiegeStrategyIfNeeded Prefix —— 跳过原版在 leader 被 AI 重算后顺带执行的
    ///    AI 策略重选，玩家的围攻策略（或默认 Custom）不被 AI 改掉。
    ///
    /// 「玩家是第一个围攻者」判定：_besiegerParties 按加入顺序 append（AddSiegePartyInternal
    /// 里 _besiegerParties.Add(mobileParty)），列表第一位 = 最先加入且仍未离开的围攻者；
    /// 玩家离开后第一位自动易主，补丁随之失效（玩家不会去抢别人发起的围城指挥权）。
    /// 无静态状态，读档后判定依然成立。
    ///
    /// Postfix 一律声明为 void（Harmony 2.4.x pass-through 陷阱，见 AGENTS.md §1.4）。
    /// 不缓存 MCM 开关，每次调用实时读取（AGENTS.md §2.1）。
    /// 不带 [HarmonyPatch] 属性，由 HarmonyPatchRegistry 显式注册。
    /// </summary>
    internal static class SiegeLeadershipLockPatch
    {
        private static readonly FieldInfo _besiegerPartiesField = AccessTools.Field(typeof(BesiegerCamp), "_besiegerParties");
        private static readonly FieldInfo _leaderPartyField = AccessTools.Field(typeof(BesiegerCamp), "_leaderParty");
        private static readonly FieldInfo _factionField = AccessTools.Field(typeof(BesiegerCamp), "_faction");

        /// <summary>BesiegerCamp.AddSiegePartyInternal 之后：玩家先发起围攻时锁定数据层指挥权。</summary>
        internal static void LockLeadershipOnPartyJoin(BesiegerCamp __instance)
        {
            // MCM 运行时开关 — 关闭时不干预
            if (Settings.Instance?.KeepPlayerSiegeLeadership != true)
                return;

            if (__instance == null || !IsPlayerFirstBesieger(__instance))
                return;

            _leaderPartyField.SetValue(__instance, MobileParty.MainParty);
            _factionField.SetValue(__instance, MobileParty.MainParty.MapFaction);
        }

        /// <summary>DefaultEncounterModel.GetLeaderOfSiegeEvent 之后：UI/菜单层强制玩家为指挥官。</summary>
        internal static void ForcePlayerAsLeader(SiegeEvent siegeEvent, BattleSideEnum side, ref Hero __result)
        {
            // MCM 运行时开关 — 关闭时不干预
            if (Settings.Instance?.KeepPlayerSiegeLeadership != true)
                return;

            if (side != BattleSideEnum.Attacker)
                return;

            if (siegeEvent?.BesiegerCamp == null || !IsPlayerFirstBesieger(siegeEvent.BesiegerCamp))
                return;

            __result = Hero.MainHero;
        }

        /// <summary>BesiegerCamp.ChangeSiegeStrategyIfNeeded 之前：玩家指挥时跳过 AI 策略重选。</summary>
        internal static bool SkipAiStrategyChange(BesiegerCamp __instance)
        {
            // MCM 运行时开关 — 关闭时放行原方法
            if (Settings.Instance?.KeepPlayerSiegeLeadership != true)
                return true;

            if (__instance == null || !IsPlayerFirstBesieger(__instance))
                return true; // 非玩家指挥，放行原方法

            // 玩家指挥：跳过 AI 重选策略，保留玩家选择的策略（或默认 Custom）
            return false;
        }

        /// <summary>玩家是否仍是最先加入且未离开的围攻者。</summary>
        private static bool IsPlayerFirstBesieger(BesiegerCamp camp)
        {
            // _besiegerParties 按加入顺序 append：第一位 = 最先加入且未离开的围攻者
            if (_besiegerPartiesField.GetValue(camp) is System.Collections.IList parties && parties.Count > 0)
            {
                return ReferenceEquals(parties[0], MobileParty.MainParty);
            }
            return false;
        }
    }
}
