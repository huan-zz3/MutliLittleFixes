using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;

namespace ExampleMod.Patches
{
    /// <summary>
    /// 禁止 AI 领主自动发起宣战决策：
    /// 玩家是国王时，禁止属下领主的 DeclareWarDecision 通过 IsAllowed 检查，
    /// 使该决策不会被添加到未决议程中。
    ///
    /// 带 [HarmonyPatch] 属性，由 SubModule 的 PatchAll() 自动发现并安装。
    /// MCM 开关实时生效，无需重启。
    /// </summary>
    [HarmonyPatch(typeof(DeclareWarDecision), "IsAllowed")]
    internal static class PreventAIWarDeclarationPatch
    {
        internal static void Postfix(DeclareWarDecision __instance, ref bool __result)
        {
            // 运行时检查 MCM 开关，关闭时不干预
            if (Settings.Instance?.PreventAIWarDeclaration == false)
                return;

            // 游戏本身已经不允许了，不干预
            if (!__result)
                return;

            Kingdom? playerKingdom = Clan.PlayerClan?.Kingdom;
            if (playerKingdom == null || playerKingdom.RulingClan?.Leader != Hero.MainHero)
                return; // 玩家不是国王

            // 只拦截玩家王国的宣战决策，不影响 AI 王国互相宣战
            if (__instance.Kingdom == playerKingdom)
            {
                __result = false;
            }
        }
    }
}
