using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace MutliLittleFixes.Behaviors
{
    /// <summary>
    /// 监控玩家家族成员状态，当成员从俘虏→Released→变为 Active（可指派为部队领袖）
    /// 时，通过屏幕中央 toast 告知玩家该成员现已可用。
    /// </summary>
    public class CompanionAutoRecallBehavior : CampaignBehaviorBase
    {
        /// <summary>用于防重复通知的内存标记</summary>
        private readonly HashSet<Hero> _notifiedHeroes = new HashSet<Hero>();

        public override void RegisterEvents()
        {
            // 英雄从 Released/Fugitive → Active 时，说明重组完毕，可以指派了
            CampaignEvents.OnHeroActivatedEvent.AddNonSerializedListener(this, OnHeroActivated);
        }

        public override void SyncData(IDataStore dataStore)
        {
            // toast 是即时的，无需持久化
        }

        private void OnHeroActivated(Hero hero, Hero.CharacterStates previousState)
        {
            TryNotifyHeroAvailable(hero);
        }

        private void TryNotifyHeroAvailable(Hero hero)
        {
            if (!(Settings.Instance?.CompanionAutoRecallEnabled ?? true))
                return;

            // ── 条件检查 ──

            // 1. 必须是玩家家族成员
            if (hero.Clan != Clan.PlayerClan)
                return;

            // 2. 不能是玩家本人
            if (hero == Hero.MainHero)
                return;

            // 3. 必须是 Active 状态（重组完成）
            if (!hero.IsActive)
                return;

            // 4. 不能是儿童
            if (hero.IsChild)
                return;

            // 5. 不能是俘虏
            if (hero.PartyBelongedToAsPrisoner != null)
                return;

            // 6. 不能已有部队
            if (hero.PartyBelongedTo != null)
                return;

            // 7. 必须有资格带领部队
            if (!hero.CanLeadParty())
                return;

            // 8. 不能是总督
            if (hero.GovernorOf != null)
                return;

            // 9. 防重复通知
            if (!_notifiedHeroes.Add(hero))
                return;

            // ── 弹出 toast ──
            TextObject toast = new TextObject("{=!}{HERO_NAME} 现可被召回");
            toast.SetTextVariable("HERO_NAME", hero.Name?.ToString() ?? "???");

            MBInformationManager.AddQuickInformation(
                toast,
                extraTimeInMs: 0,
                announcerCharacter: hero.CharacterObject,
                soundEventPath: "event:/ui/notification/quest_start");

            LogDebug($"[可用提醒] {hero.Name} 现可被召回");
        }

        // ── 调试日志 ──

        private static void LogDebug(string message)
        {
            if (Settings.Instance?.EnableCompanionRecallDebugLog != true)
                return;

            InformationManager.DisplayMessage(
                new InformationMessage(message, Color.FromUint(0x00FF00u)));
        }
    }
}
