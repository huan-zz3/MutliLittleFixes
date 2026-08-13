using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace MutliLittleFixes.Behaviors
{
    /// <summary>
    /// 粮草运输队组件。
    /// 携带两套粮:
    /// - 抽象粮(FoodCarried): 出发时从源城 FoodStocks 扣除,交付时直接加入目标城 FoodStocks,
    ///   途中不消耗;未交付返程时全额退回源城。彻底不走市场消费转化。
    /// - 实物粮(队伍背包谷物): 仅供运输队自身每日消耗(由原版 FoodConsumptionBehavior 自动吃掉),
    ///   与抽象粮互不互通,交付/回收时销毁剩余。
    /// 状态机: TravelingToTarget(前往) → Returning(返回) → Done(已回收销毁)。
    /// </summary>
    public class FoodTransportPartyComponent : PartyComponent
    {
        public enum TransportPhase
        {
            TravelingToTarget = 0,
            Returning = 1,
            Done = 2,
        }

        [SaveableField(10)]
        private Settlement _sourceSettlement;

        [SaveableField(20)]
        private Settlement _targetSettlement;

        [SaveableField(30)]
        private TransportPhase _phase;

        [SaveableField(40)]
        private int _foodCarried;

        /// <summary>
        /// 创建时的一次性名册(仅 OnMobilePartySetOnCreation 使用,士兵随后转入 MobileParty.MemberRoster)。
        /// 不持久化: 士兵名册由 MobileParty 自身保存, 冗余序列化一份 TroopRoster 是隐患。
        /// </summary>
        private TroopRoster _troopRoster;

        public Settlement SourceSettlement => _sourceSettlement;

        public Settlement TargetSettlement => _targetSettlement;

        public TransportPhase Phase
        {
            get => _phase;
            set => _phase = value;
        }

        /// <summary>抽象粮:出发时从源城扣除、交付时加入目标城、未交付返程时退回源城的数量。</summary>
        public int FoodCarried => _foodCarried;

        public override Hero PartyOwner => _sourceSettlement?.OwnerClan?.Leader;

        public override Settlement HomeSettlement => _sourceSettlement;

        public override TextObject Name
        {
            get
            {
                TextObject name = new TextObject("{=mlf_food_party_name}{SOURCE_NAME} Food Transport");
                name.SetTextVariable("SOURCE_NAME", _sourceSettlement?.Name?.ToString() ?? "Unknown");
                return name;
            }
        }

        public FoodTransportPartyComponent(Settlement source, Settlement target, int foodCarried, TroopRoster troopRoster)
        {
            _sourceSettlement = source;
            _targetSettlement = target;
            _foodCarried = foodCarried;
            _troopRoster = troopRoster;
            _phase = TransportPhase.TravelingToTarget;
        }

        protected override void OnMobilePartySetOnCreation()
        {
            if (_sourceSettlement == null)
            {
                return;
            }
            base.MobileParty.Aggressiveness = 0f;
            base.MobileParty.ActualClan = _sourceSettlement.OwnerClan;
            if (_troopRoster != null)
            {
                base.MobileParty.InitializeMobilePartyAroundPosition(_troopRoster, null, _sourceSettlement.GatePosition, 0.5f, 0f, false);
            }
            base.MobileParty.InitializePartyTrade(0);
            base.MobileParty.Party.SetVisualAsDirty();
        }

        public override Banner GetDefaultComponentBanner()
        {
            return _sourceSettlement?.Banner;
        }
    }
}
