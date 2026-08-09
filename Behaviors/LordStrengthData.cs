using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace MutliLittleFixes.Behaviors
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

        /// <summary>
        /// 待交付的谷物总数。按天数每日平均交付到领主队伍背包，防止饥饿减员。
        /// </summary>
        [SaveableField(11)] public int FoodToDeliver;
    }

    /// <summary>
    /// 单次领土事件记录。全部追加，永不删除。
    /// 计算加成时先按 SettlementId 配对同城得失，再对未配对事件做过期过滤，
    /// 最后跨城抵消算出净丢失数，对净丢失序列应用衰减。
    /// 
    /// EventDay：事件发生的游戏天数（CampaignTime.Now.ToDays），用于过期判断。
    /// SettlementId：城池的 StringId，用于同城得失配对。
    /// 旧存档迁移时 EventDay=1, SettlementId=null（永不过期）。
    /// </summary>
    public class TerritoryEvent
    {
        [SaveableField(1)] public bool IsTown;
        [SaveableField(2)] public bool IsLoss; // true=丢失, false=征服
        [SaveableField(3)] public int EventDay;          // 事件发生的游戏天数
        [SaveableField(4)] public string SettlementId;   // 城池 StringId，用于配对；null=旧存档无法配对
    }

    /// <summary>
    /// 王国领土动态数据。记录全部领土事件历史，
    /// 并缓存计算后的队伍规模加成值。
    /// </summary>
    public class KingdomTerritoryData
    {
        /// <summary>缓存加成值。只在领土事件发生时重算，读取时直接返回。</summary>
        [SaveableField(1)] public float AccumulatedBonus;

#pragma warning disable 612,618 // 旧存档兼容字段
        /// <summary>旧存档兼容（v1 计数器，已废弃，仅用于加载旧存档数据供迁移）</summary>
        [SaveableField(2)] [Obsolete] public int TownsLost;
        /// <summary>旧存档兼容</summary>
        [SaveableField(3)] [Obsolete] public int CastlesLost;
        /// <summary>旧存档兼容</summary>
        [SaveableField(4)] [Obsolete] public int TownsConquered;
        /// <summary>旧存档兼容</summary>
        [SaveableField(5)] [Obsolete] public int CastlesConquered;
#pragma warning restore 612,618

        /// <summary>
        /// 按时间顺序排列的全部领土事件（最早在前，最新在后）。
        /// 丢失和征服全部追加，永不删除。计算时先算净丢失数，
        /// 再对净丢失序列按衰减累加（征服不直接扣减加成）。
        /// </summary>
        [SaveableField(6)] public List<TerritoryEvent> Events = new List<TerritoryEvent>();
    }
}
