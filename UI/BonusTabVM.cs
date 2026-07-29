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
    //  ViewModelMixin — 向 KingdomManagementVM 注入加成标签页
    // ════════════════════════════════════════════════════════

    /// <summary>
    /// 为 KingdomManagementVM 注入"国家加成"标签页所需的全部属性和命令。
    /// 严格遵循 UIExtenderEx 教程的标签页模式：
    ///   - 注入子 VM Bonus（≈ 教程的 Agenda）
    ///   - XML 使用 DataSource='{Bonus}' 导航到子 VM
    ///   - ExecuteShowBonus 中显式调 ViewModel.OnPropertyChanged 通知父 VM
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

            // 步骤 1: 刷新数据（先于可见性，避免空帧）
            Bonus.RefreshKingdoms();

            // 步骤 2: 切换选中状态
            IsBonusTabSelected = true;

            // 步骤 3: 在父 VM 上显式通知（教程 §6.3 第 4 步）
            // 仅靠 Mixin 的 OnPropertyChangedWithValue 可能不传播到 Gauntlet 绑定系统
            ViewModel.OnPropertyChanged("IsBonusTabSelected");
            ViewModel.OnPropertyChanged("Bonus");

            LogDebug($"[UI刷新] 完成: IsBonusTabSelected={IsBonusTabSelected}, KingdomList.Count={Bonus.KingdomList?.Count ?? -1}, HasItems={Bonus.HasItems}");
            if (Bonus.KingdomList != null && Bonus.KingdomList.Count > 0)
                LogDebug($"[UI刷新] 首条: {Bonus.KingdomList[0].KingdomName} / {Bonus.KingdomList[0].TerritoryBonusText}");
        }

        /// <summary>Harmony 补丁入口：当其他标签被选中时清除本标签。</summary>
        internal static void TryClear(KingdomManagementVM vm)
        {
            if (_instances.TryGetValue(vm, out var mixin))
            {
                mixin.IsBonusTabSelected = false;
                mixin.ViewModel.OnPropertyChanged("IsBonusTabSelected");
            }
        }

        // ── 调试日志 ─────────────────────────────────────

        private static void LogDebug(string message)
        {
            if (Settings.Instance?.EnableDebugLogging != true) return;
            InformationManager.DisplayMessage(
                new InformationMessage(message, Color.FromUint(0x00FFFFu)));
        }
    }

    // ════════════════════════════════════════════════════════
    //  BonusTabVM — 标签页子 VM，持有国家列表
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
                    OnPropertyChanged(nameof(HasItems));
                }
            }
        }

        private bool _hasItems;
        [DataSourceProperty]
        public bool HasItems
        {
            get => _hasItems;
            set
            {
                if (value != _hasItems)
                {
                    _hasItems = value;
                    OnPropertyChangedWithValue(value, "HasItems");
                }
            }
        }

        public BonusTabVM()
        {
            KingdomList = new MBBindingList<BonusKingdomItemVM>();
        }

        // ── 调试日志 ─────────────────────────────────────

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

            var activeKingdoms = kingdoms
                .Where(k => !k.IsEliminated)
                .Select(k => new
                {
                    Kingdom = k,
                    Bonus = (int)(bonusBehavior?.GetTerritoryBonus(k) ?? 0f)
                })
                .OrderByDescending(x => x.Bonus)
                .ToList();

            // ── 移除已灭国的 ────────────────────────────
            var idsToKeep = new HashSet<string>(
                activeKingdoms.Select(x => x.Kingdom.StringId));

            for (int i = KingdomList.Count - 1; i >= 0; i--)
            {
                if (!idsToKeep.Contains(KingdomList[i].KingdomId))
                    KingdomList.RemoveAt(i);
            }

            // 刷新现有 item + 创建缺失的
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

            HasItems = KingdomList.Count > 0;

            LogDebug($"[UI刷新] RefreshKingdoms 完成: {KingdomList.Count} 个王国");
        }
    }

    // ════════════════════════════════════════════════════════
    //  BonusKingdomItemVM — 单个王国的加成数据行
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

        /// <summary>等候补兵人数（进入队列但尚未实际发兵）</summary>
        [DataSourceProperty]
        public string WaitingCountText { get; set; }

        /// <summary>正在补兵人数（有队伍且正在每日发兵）</summary>
        [DataSourceProperty]
        public string ActiveCountText { get; set; }

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
            int bonus = (int)(_bonusBehavior?.GetTerritoryBonus(_kingdom) ?? 0f);
            TerritoryBonusText = $"+{bonus}";

            int waiting = _restoreBehavior?.GetWaitingRestorationCount(_kingdom) ?? 0;
            int active  = _restoreBehavior?.GetActiveRestorationCount(_kingdom) ?? 0;
            int total   = waiting + active;

            RestorationCountText = $"{total}人";
            WaitingCountText     = $"{waiting}人";
            ActiveCountText      = $"{active}人";

            OnPropertyChanged(nameof(TerritoryBonusText));
            OnPropertyChanged(nameof(RestorationCountText));
            OnPropertyChanged(nameof(WaitingCountText));
            OnPropertyChanged(nameof(ActiveCountText));
        }
    }
}
