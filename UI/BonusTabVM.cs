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
            KingdomList.Clear();

            var bonusBehavior = Campaign.Current?.GetCampaignBehavior<KingdomTerritoryBonusBehavior>();
            var restoreBehavior = Campaign.Current?.GetCampaignBehavior<LordTroopRestorationBehavior>();
            var kingdoms = Campaign.Current?.Kingdoms ?? Enumerable.Empty<Kingdom>();

            // 过滤已灭国 + 按加成值降序排列
            var items = kingdoms
                .Where(k => !k.IsEliminated)
                .Select(k => new
                {
                    Kingdom = k,
                    Bonus = (int)(bonusBehavior?.GetTerritoryBonus(k) ?? 0f)
                })
                .OrderByDescending(x => x.Bonus)
                .Select(x => new BonusKingdomItemVM(x.Kingdom, bonusBehavior, restoreBehavior))
                .ToList();

            foreach (var item in items)
                KingdomList.Add(item);

            LogDebug($"[UI刷新] RefreshKingdoms 完成: {items.Count} 个王国");
        }
    }

    // ════════════════════════════════════════════════════════
    //  3. BonusKingdomItemVM — 单个王国的加成数据行
    // ════════════════════════════════════════════════════════

    internal class BonusKingdomItemVM : ViewModel
    {
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
            KingdomName = kingdom.Name?.ToString() ?? kingdom.StringId;

            int bonus = (int)(bonusBehavior?.GetTerritoryBonus(kingdom) ?? 0f);
            TerritoryBonusText = $"领土丧失补偿: +{bonus}";

            int count = restoreBehavior?.GetPendingRestorationCount(kingdom) ?? 0;
            RestorationCountText = $"领主恢复中: {count}人";
        }
    }
}
