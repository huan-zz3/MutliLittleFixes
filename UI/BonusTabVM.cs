using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement;
using TaleWorlds.Library;

using ExampleMod.Behaviors;

namespace ExampleMod.UI
{
    // ════════════════════════════════════════════════════════
    //  1. ViewModelMixin — 向 KingdomManagementVM 注入加成标签页
    // ════════════════════════════════════════════════════════

    /// <summary>
    /// 为 KingdomManagementVM 注入"国家加成"标签页所需的全部属性和命令。
    /// 对应教程 §6 ViewModelMixin + §7 Harmony 标签切换协调。
    /// </summary>
    [ViewModelMixin("RefreshValues", true)]
    internal sealed class BonusTabVMMixin
        : BaseViewModelMixin<KingdomManagementVM>
    {
        private static readonly ConditionalWeakTable<KingdomManagementVM, BonusTabVMMixin> _instances = new();

        private bool _isBonusTabSelected;

        [DataSourceProperty]
        public bool IsBonusTabSelected
        {
            get => _isBonusTabSelected;
            set
            {
                if (value != _isBonusTabSelected)
                {
                    _isBonusTabSelected = value;
                    OnPropertyChangedWithValue(value, "IsBonusTabSelected");
                }
            }
        }

        [DataSourceProperty]
        public string BonusTabText { get; set; }

        [DataSourceProperty]
        public BonusTabVM Bonus { get; set; }

        public BonusTabVMMixin(KingdomManagementVM vm) : base(vm)
        {
            BonusTabText = "国家加成";
            Bonus = new BonusTabVM();
            _instances.Add(vm, this);
        }

        public override void OnRefresh()
        {
            Bonus?.RefreshKingdoms();
        }

        [DataSourceMethod]
        public void ExecuteShowBonus()
        {
            LogDebug("[UI刷新] ExecuteShowBonus 被调用");
            ViewModel.Clan.Show = false;
            ViewModel.Settlement.Show = false;
            ViewModel.Policy.Show = false;
            ViewModel.Army.Show = false;
            ViewModel.Diplomacy.Show = false;

            IsBonusTabSelected = true;
            Bonus.RefreshKingdoms();
        }

        /// <summary>Harmony 补丁入口：当其他标签被选中时清除本标签。</summary>
        internal static void TryClear(KingdomManagementVM vm)
        {
            if (_instances.TryGetValue(vm, out var mixin))
                mixin.IsBonusTabSelected = false;
        }

        // ── 调试日志 ─────────────────────────────────────────────────────

        private static void LogDebug(string message)
        {
            if (Settings.Instance?.EnableDebugLogging != true) return;
            InformationManager.DisplayMessage(
                new InformationMessage(message, Color.FromUint(0x00FFFFu)));
        }

    }

    // ════════════════════════════════════════════════════════
    //  2. BonusTabVM — 标签页子 VM，持有国家列表
    // ════════════════════════════════════════════════════════

    internal class BonusTabVM : ViewModel
    {
        private MBBindingList<BonusKingdomItemVM> _kingdomList;

        [DataSourceProperty]
        public MBBindingList<BonusKingdomItemVM> KingdomList
        {
            get => _kingdomList;
            set
            {
                if (value != _kingdomList)
                {
                    _kingdomList = value;
                    OnPropertyChanged(nameof(KingdomList));
                }
            }
        }

        [DataSourceProperty]
        public bool HasItems => _kingdomList?.Count > 0;

        public BonusTabVM()
        {
            KingdomList = new MBBindingList<BonusKingdomItemVM>();
        }

        // ── 调试日志 ─────────────────────────────────────────────────────

        private static void LogDebug(string message)
        {
            if (Settings.Instance?.EnableDebugLogging != true) return;
            InformationManager.DisplayMessage(
                new InformationMessage(message, Color.FromUint(0x00FFFFu)));
        }

        public void RefreshKingdoms()
        {
            LogDebug("[UI刷新] RefreshKingdoms 开始");

            var bonusBehavior = Campaign.Current?.GetCampaignBehavior<KingdomTerritoryBonusBehavior>();
            var restoreBehavior = Campaign.Current?.GetCampaignBehavior<LordTroopRestorationBehavior>();
            var kingdoms = Campaign.Current?.Kingdoms ?? Enumerable.Empty<Kingdom>();

            // 过滤已灭国 + 按加成值降序排列
            var activeKingdoms = kingdoms
                .Where(k => !k.IsEliminated)
                .Select(k => new
                {
                    Kingdom = k,
                    Bonus = (int)(bonusBehavior?.GetTerritoryBonus(k) ?? 0f)
                })
                .OrderByDescending(x => x.Bonus)
                .ToList();

            // ── 原地更新已有 item，新增缺失的，移除已灭国的 ──────────────

            // ── 移除已灭国的 ──────────────────────────────────────────
            var idsToKeep = new HashSet<string>(
                activeKingdoms.Select(x => x.Kingdom.StringId));

            for (int i = KingdomList.Count - 1; i >= 0; i--)
            {
                if (!idsToKeep.Contains(KingdomList[i].KingdomId))
                    KingdomList.RemoveAt(i);
            }

            // 刷新现有 item + 创建缺失的（保持 activeKingdoms 的排序）
            var existingByKingdomId = KingdomList
                .ToDictionary(item => item.KingdomId, item => item);

            foreach (var x in activeKingdoms)
            {
                string kid = x.Kingdom.StringId;
                if (existingByKingdomId.TryGetValue(kid, out var existing))
                {
                    existing.Refresh(bonusBehavior, restoreBehavior);
                }
                else
                {
                    var item = new BonusKingdomItemVM(x.Kingdom, bonusBehavior, restoreBehavior);
                    KingdomList.Add(item);
                    existingByKingdomId[kid] = item;
                }
            }

            LogDebug($"[UI刷新] RefreshKingdoms 完成: {KingdomList.Count} 个王国");
        }
    }

    // ════════════════════════════════════════════════════════
    //  3. BonusKingdomItemVM — 单个王国的加成数据行
    // ════════════════════════════════════════════════════════

    internal class BonusKingdomItemVM : ViewModel
    {
        private readonly Kingdom _kingdom;
        private KingdomTerritoryBonusBehavior? _bonusBehavior;
        private LordTroopRestorationBehavior? _restoreBehavior;

        /// <summary>用于列表原地查找的稳定标识符。</summary>
        public string KingdomId => _kingdom.StringId;

        [DataSourceProperty]
        public string KingdomName { get; set; }

        [DataSourceProperty]
        public string TerritoryBonusText { get; set; }

        [DataSourceProperty]
        public string RestorationCountText { get; set; }

        public BonusKingdomItemVM(
            Kingdom kingdom,
            KingdomTerritoryBonusBehavior? bonusBehavior,
            LordTroopRestorationBehavior? restoreBehavior)
        {
            _kingdom = kingdom;
            _bonusBehavior = bonusBehavior;
            _restoreBehavior = restoreBehavior;

            KingdomName = kingdom.Name?.ToString() ?? kingdom.StringId;
            ApplyLiveData();
        }

        /// <summary>
        /// 原地刷新：从行为中重新读取实时数据并触发 UI 更新。
        /// 调用此方法比重新创建 VM 更高效，且能保持 UI 元素状态。
        /// </summary>
        public void Refresh(
            KingdomTerritoryBonusBehavior? bonusBehavior,
            LordTroopRestorationBehavior? restoreBehavior)
        {
            _bonusBehavior = bonusBehavior;
            _restoreBehavior = restoreBehavior;
            ApplyLiveData();
        }

        private void ApplyLiveData()
        {
            // ── 领土加成（每次从 Behavior 实时读取）──
            int bonus = (int)(_bonusBehavior?.GetTerritoryBonus(_kingdom) ?? 0f);
            TerritoryBonusText = $"+{bonus}";

            // ── 补兵统计（每次从 _pendingRestorations 实时读取）──
            int count = _restoreBehavior?.GetPendingRestorationCount(_kingdom) ?? 0;
            RestorationCountText = $"{count}人";

            // 通知 UI 绑定更新
            OnPropertyChanged(nameof(TerritoryBonusText));
            OnPropertyChanged(nameof(RestorationCountText));
        }
    }
}
