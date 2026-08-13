using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace MutliLittleFixes.Behaviors
{
    /// <summary>
    /// 村庄出资重建
    ///
    /// 被彻底掠夺（VillageState == Looted，荒废）的村庄，在「village_looted」菜单中提供
    /// 「出资重建」选项：花费 10000 第纳尔，3 天后村庄自动重建完成——
    /// 调用 IncreaseSettlementHealthAction.Apply(1f) 将村庄血量回满（触发恢复正常状态 + 民兵+20），
    /// 并奖励村庄全部名人好感 25~35。
    ///
    /// MCM 开关实时生效：关闭后菜单选项隐藏，无法再发起新的出资重建；
    /// 已出资的进行中重建不受开关影响，仍会按时完成（玩家已付款，不白吞金币）。
    /// </summary>
    public class VillageRebuildBehavior : CampaignBehaviorBase
    {
        private const int ReconstructionCost = 10000;
        private const int ReconstructionDays = 3; // 重建期：出资后 3 天完成
        private const int MinNotableRelationReward = 25;
        private const int MaxNotableRelationReward = 35;
        private const string PendingRebuildsSaveKey = "_villageRebuildPendingEndDaysBySettlementId";

        private Dictionary<string, float> _pendingEndDaysBySettlementId = new Dictionary<string, float>();

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, OnSessionLaunched);
            CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener(this, OnDailyTickSettlement);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData(PendingRebuildsSaveKey, ref _pendingEndDaysBySettlementId);

            if (dataStore.IsLoading && _pendingEndDaysBySettlementId == null)
            {
                _pendingEndDaysBySettlementId = new Dictionary<string, float>();
            }
        }

        private void OnSessionLaunched(CampaignGameStarter starter)
        {
            starter.AddGameMenuOption(
                "village_looted",
                "village_rebuild_fund_reconstruction",
                "出资重建",
                FundReconstructionOnCondition,
                FundReconstructionOnConsequence,
                isLeave: false,
                index: 0);

            ProcessCompletedRebuilds();
        }

        private void OnDailyTickSettlement(Settlement settlement)
        {
            if (settlement == null || !settlement.IsVillage)
            {
                return;
            }

            ProcessCompletedRebuild(settlement);
        }

        // ── 菜单选项条件：仅荒废村庄 + MCM 开关开启时显示 ──────────────────

        private bool FundReconstructionOnCondition(MenuCallbackArgs args)
        {
            args.optionLeaveType = GameMenuOption.LeaveType.Manage;

            // MCM 运行时开关 — 关闭时隐藏菜单选项（无法发起新的出资重建）
            if (Settings.Instance?.VillageRebuildEnabled != true)
            {
                return false;
            }

            Settlement settlement = Settlement.CurrentSettlement;
            if (settlement == null || !settlement.IsVillage || settlement.Village.VillageState != Village.VillageStates.Looted)
            {
                return false;
            }

            string settlementId = settlement.StringId;
            if (_pendingEndDaysBySettlementId.TryGetValue(settlementId, out float endDay))
            {
                int remainingDays = Math.Max(1, (int)Math.Ceiling(endDay - CampaignTime.Now.ToDays));
                args.Text = new TextObject("重建进行中（剩余 {REMAINING_DAYS} 天）");
                args.Text.SetTextVariable("REMAINING_DAYS", remainingDays);
                args.IsEnabled = false;
                args.Tooltip = new TextObject("该村庄正在重建，预计 {REMAINING_DAYS} 天后完工。");
                args.Tooltip.SetTextVariable("REMAINING_DAYS", remainingDays);
                return true;
            }

            args.Text = new TextObject("出资重建（{COST} 第纳尔）");
            args.Text.SetTextVariable("COST", ReconstructionCost);

            if (Hero.MainHero.Gold < ReconstructionCost)
            {
                args.IsEnabled = false;
                args.Tooltip = new TextObject("需要 {COST} 第纳尔才能出资重建。");
                args.Tooltip.SetTextVariable("COST", ReconstructionCost);
            }

            return true;
        }

        // ── 菜单选项后果：扣金币 + 登记重建结束日 ─────────────────────────

        private void FundReconstructionOnConsequence(MenuCallbackArgs args)
        {
            Settlement settlement = Settlement.CurrentSettlement;
            if (settlement == null || !settlement.IsVillage || settlement.Village.VillageState != Village.VillageStates.Looted)
            {
                return;
            }

            if (_pendingEndDaysBySettlementId.ContainsKey(settlement.StringId) || Hero.MainHero.Gold < ReconstructionCost)
            {
                return;
            }

            GiveGoldAction.ApplyBetweenCharacters(Hero.MainHero, null, ReconstructionCost);
            _pendingEndDaysBySettlementId[settlement.StringId] = (float)CampaignTime.DaysFromNow(ReconstructionDays).ToDays;

            TextObject message = new TextObject("你出资重建了 {SETTLEMENT}，工程将在 {DAYS} 天后完工。");
            message.SetTextVariable("SETTLEMENT", settlement.Name);
            message.SetTextVariable("DAYS", ReconstructionDays);
            InformationManager.DisplayMessage(new InformationMessage(message.ToString()));
        }

        // ── 处理到期重建（会话启动时全量扫描） ─────────────────────────────

        private void ProcessCompletedRebuilds()
        {
            List<string> pendingSettlementIds = new List<string>(_pendingEndDaysBySettlementId.Keys);
            foreach (string settlementId in pendingSettlementIds)
            {
                Settlement settlement = Settlement.Find(settlementId);
                if (settlement == null || !settlement.IsVillage)
                {
                    _pendingEndDaysBySettlementId.Remove(settlementId);
                    continue;
                }

                ProcessCompletedRebuild(settlement);
            }
        }

        // ── 处理单个村庄到期重建（已付款的进行中重建不受 MCM 开关影响） ────

        private void ProcessCompletedRebuild(Settlement settlement)
        {
            if (!_pendingEndDaysBySettlementId.TryGetValue(settlement.StringId, out float endDay))
            {
                return;
            }

            if (CampaignTime.Now.ToDays < endDay)
            {
                return;
            }

            _pendingEndDaysBySettlementId.Remove(settlement.StringId);

            if (settlement.IsVillage && settlement.Village.VillageState != Village.VillageStates.Normal)
            {
                // 血量回满即触发原版恢复正常状态（ApplyBySettingToNormal + 民兵+20）
                IncreaseSettlementHealthAction.Apply(settlement, 1f);
            }

            foreach (Hero notable in settlement.Notables)
            {
                if (notable != null && notable.IsAlive)
                {
                    int relationReward = MBRandom.RandomInt(MinNotableRelationReward, MaxNotableRelationReward + 1);
                    ChangeRelationAction.ApplyPlayerRelation(notable, relationReward);
                }
            }

            TextObject message = new TextObject("{SETTLEMENT}的重建已经完成，当地村民对你的帮助感激不尽。");
            message.SetTextVariable("SETTLEMENT", settlement.Name);
            InformationManager.DisplayMessage(new InformationMessage(message.ToString()));
        }
    }
}
