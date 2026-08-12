using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using MutliLittleFixes.Behaviors;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 运粮队部队上限补足补丁。
    /// 运粮队无英雄带队，原版 PartySizeLimit 只给予 20 人基础值（商队/村民分支均不适用），
    /// 超过 20 人即触发原版超编减速惩罚（Over Party Size，30 人时约 -33%）。
    /// 本补丁把运粮队的部队上限补足到「运输队人数」设置值，消除超编减速。
    /// </summary>
    internal static class TransportPartySizeLimitPatch
    {
        private static readonly TextObject _bonusText = new TextObject("运粮队部队上限");

        internal static void Postfix(PartyBase party, bool includeDescriptions, ref ExplainedNumber __result)
        {
            // MCM 运行时开关 — 跟随「启用粮草运输支援」，关闭时不干预
            if (Settings.Instance?.TransportSupportEnabled != true)
            {
                return;
            }

            if (!party.IsMobile)
            {
                return;
            }
            if (party.MobileParty?.PartyComponent is not FoodTransportPartyComponent)
            {
                return;
            }

            // 把上限补足到「运输队人数」设置值，保证新建运粮队不超编
            int targetLimit = Math.Max(1, Settings.Instance?.TransportPartySize ?? 30);
            float currentLimit = __result.ResultNumber;
            if (targetLimit > currentLimit)
            {
                __result.Add(targetLimit - currentLimit, _bonusText);
            }
        }
    }
}
