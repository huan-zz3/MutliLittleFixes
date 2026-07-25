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
