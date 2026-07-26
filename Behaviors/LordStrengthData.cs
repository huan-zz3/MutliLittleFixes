using TaleWorlds.CampaignSystem;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace ExampleMod.Behaviors
{
    /// <summary>
    /// 追踪领主战败被俘释放后的部队/金币恢复进度。
    /// 该行为根据配置的等级比例，在数天内逐步向领主交付部队。
    /// </summary>
    public class PendingRestoration
    {
        [SaveableField(1)] public int DaysRemaining;
        [SaveableField(2)] public int TotalTroopsToDeliver;
        [SaveableField(3)] public int TroopsPerDay;
        [SaveableField(4)] public int GoldToDeliver;
        [SaveableField(5)] public float Tier12Ratio;
        [SaveableField(6)] public float Tier34Ratio;
        [SaveableField(7)] public float Tier56Ratio;
        [SaveableField(8)] public int PartySizeLimitAtRelease;

        /// <summary>
        /// 以字符串形式存储 CultureObject.StringId，因为 MBObjectBase
        /// 无法通过 SaveableField 直接序列化。
        /// 运行时通过 MBObjectManager.Instance.GetObject&lt;CultureObject&gt;(id) 解析。
        /// </summary>
        [SaveableField(9)] public string TroopCultureId = string.Empty;

        /// <summary>
        /// 自创建以来没有队伍的天数。超过 MCM 配置的放弃天数后会被清理。
        /// </summary>
        [SaveableField(10)] public int DaysWithoutParty;
    }

    /// <summary>
    /// 单次定居点丢失记录，用于栈式补偿计算。
    /// 每次丢失推入列表，每次征服弹出最新一条，补偿基于栈中剩余记录重算。
    /// </summary>
    public class SettlementLossRecord
    {
        [SaveableField(1)] public bool IsTown;
    }

    /// <summary>
    /// 王国领土动态数据。追踪失去/征服的定居点，
    /// 以及用于调整领主队伍规模的累积加成（领土丧失时增加，征服时减少）。
    /// </summary>
    public class KingdomTerritoryData
    {
        [SaveableField(1)] public float AccumulatedBonus;
        [SaveableField(2)] public int TownsLost;
        [SaveableField(3)] public int CastlesLost;
        [SaveableField(4)] public int TownsConquered;
        [SaveableField(5)] public int CastlesConquered;
    }
}
