using System;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace ExampleMod.Behaviors
{
    /// <summary>
    /// 自动召回玩家家族中处于 Active 状态但没有部队的成员。
    /// 当同伴/家族成员被释放、逃脱后从 Released/Fugitive 变为 Active 时，
    /// 自动为其创建一支家族部队，并通过屏幕中央 toast 提示玩家。
    /// </summary>
    public class CompanionAutoRecallBehavior : CampaignBehaviorBase
    {

        public override void RegisterEvents()
        {
            // 主触发：英雄从 Released/Fugitive → Active 时自动创建部队
            CampaignEvents.OnHeroActivatedEvent.AddNonSerializedListener(this, OnHeroActivated);

            // 兜底触发：每日检查漏掉的成员（如游戏暂停/加载等边缘情况）
            CampaignEvents.DailyTickHeroEvent.AddNonSerializedListener(this, OnDailyTickHero);
        }

        public override void SyncData(IDataStore dataStore)
        {
            // 无需持久化状态
        }

        private void OnHeroActivated(Hero hero, Hero.CharacterStates previousState)
        {
            TryAutoRecallHero(hero);
        }

        private void OnDailyTickHero(Hero hero)
        {
            TryAutoRecallHero(hero);
        }

        private void TryAutoRecallHero(Hero hero)
        {
            if (!(Settings.Instance?.CompanionAutoRecallEnabled ?? true))
                return;

            // ── 条件检查（与 ClanPartiesVM.GetCanCreateNewParty 保持一致）──

            // 1. 必须是玩家家族成员
            if (hero.Clan != Clan.PlayerClan)
                return;

            // 2. 不能是玩家本人
            if (hero == Hero.MainHero)
                return;

            // 3. 必须是 Active 状态（重组完成，立即可用）
            if (!hero.IsActive)
                return;

            // 4. 不能是儿童
            if (hero.IsChild)
                return;

            // 5. 不能已被俘虏
            if (hero.PartyBelongedToAsPrisoner != null)
                return;

            // 6. 不能已有部队（已是一支部队的领袖或成员）
            if (hero.PartyBelongedTo != null)
                return;

            // 7. 必须有能力带领部队
            if (!hero.CanLeadParty())
                return;

            // 8. 不能是总督（总督不能同时带领部队）
            if (hero.GovernorOf != null)
                return;

            // 9. 检查部队上限是否已满
            Clan playerClan = Clan.PlayerClan;
            int currentWarParties = playerClan.WarPartyComponents.Count;
            int warPartyLimit = playerClan.WarPartyLimit;
            if (currentWarParties >= warPartyLimit)
                return;

            // ── 执行自动召回 ──
            CreateAndNotifyParty(hero, playerClan);
        }

        private void CreateAndNotifyParty(Hero hero, Clan clan)
        {
            try
            {
                MobileParty newParty = MobilePartyHelper.CreateNewClanMobileParty(hero, clan);
                newParty.SetMoveModeHold();

                // 屏幕中央弹出 toast 提示（和原版"xxx的部队被xxx攻击"同款）
                TextObject toast = new TextObject("{=!}{HERO_NAME} 已被自动召回，正带领一支部队");
                toast.SetTextVariable("HERO_NAME", hero.Name?.ToString() ?? "???");
                MBInformationManager.AddQuickInformation(
                    toast,
                    extraTimeInMs: 0,
                    announcerCharacter: hero.CharacterObject,
                    soundEventPath: "event:/ui/notification/quest_start");

                LogDebug($"[自动召回] {hero.Name} 已自动召回并创建部队");
            }
            catch (Exception ex)
            {
                LogDebug($"[自动召回] {hero.Name} 创建部队失败: {ex.Message}");
            }
        }

        // ── 调试日志（左下角消息，仅在启用调试时显示）──

        private static void LogDebug(string message)
        {
            if (Settings.Instance?.EnableDebugLogging != true)
                return;

            InformationManager.DisplayMessage(
                new InformationMessage(message, Color.FromUint(0x00FF00u)));
        }
    }
}
