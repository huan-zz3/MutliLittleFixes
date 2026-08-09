using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.Core;

namespace MutliLittleFixes.Behaviors
{
    /// <summary>
    /// 每日检查主角技能等级，若超过 MCM 配置的对应属性上限则钳位。
    /// 纯运行时检查，无持久化状态。
    /// </summary>
    public class SkillLevelCapBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickHeroEvent.AddNonSerializedListener(this, OnDailyTickHero);
        }

        public override void SyncData(IDataStore dataStore)
        {
            // 无持久化状态，留空
        }

        private void OnDailyTickHero(Hero hero)
        {
            // 仅对主角生效
            if (hero != Hero.MainHero)
                return;

            // MCM 实时开关 — 关闭时不执行钳位
            if (Settings.Instance?.SkillLevelCapEnabled != true)
                return;

            Settings? settings = Settings.Instance;
            if (settings == null)
                return;

            foreach (SkillObject skill in Skills.All)
            {
                int currentValue = hero.GetSkillValue(skill);
                int cap = settings.GetSkillCap(skill);
                if (currentValue > cap)
                {
                    hero.SetSkillValue(skill, cap);
                    hero.HeroDeveloper.InitializeSkillXp(skill);
                }
            }
        }
    }
}
