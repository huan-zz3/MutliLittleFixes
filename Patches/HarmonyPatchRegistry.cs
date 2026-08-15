using System;
using System.Reflection;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.KillFeed.General;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD.KillFeed.General;
using TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard;
using TaleWorlds.MountAndBlade.View.CustomBattle;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 集中注册所有 Harmony 补丁（显式注册，替代 PatchAll 自动发现）。
    ///
    /// 规则：
    /// - 所有补丁必须在本类 Register() 中逐条显式挂载，禁止在补丁类上使用 [HarmonyPatch] 属性。
    /// - 带 MCM 开关的补丁始终注册，由补丁方法在运行时检查开关（实时生效）。
    /// - 动态目标（DLC 类型、多目标方法）在此统一处理。
    /// </summary>
    internal static class HarmonyPatchRegistry
    {
        public static void Register(Harmony harmony)
        {
            RegisterBannerBearerPosition(harmony);
            RegisterBonusTabCoordination(harmony);
            RegisterCharacterDevelopmentModel(harmony);
            RegisterHeroDeveloper(harmony);
            RegisterPartySizeLimitTerritoryBonus(harmony);
            RegisterTransportPartySizeLimit(harmony);
            RegisterNpcClanPartyLimit(harmony);
            RegisterPlayerCapturedFief(harmony);
            RegisterPreventAIWarDeclaration(harmony);
            RegisterPreventClanPartyDonateTroop(harmony);
            RegisterPreventClanPartyRecruitment(harmony);
            RegisterPrisonerSpecialLabel(harmony);
            RegisterScoreboardSortOrder(harmony);
            RegisterShieldDirectionForCrouch(harmony);
            RegisterFormationFrontRankShieldSort(harmony);
            RegisterShipBattleLimit(harmony);
            RegisterCustomBattleModeOrder(harmony);
            RegisterSiegeTargetSelection(harmony);
            RegisterSiegeWeapon(harmony);
            RegisterCoordinateTargetAI(harmony);
            RegisterMountedKnockDown(harmony);
            RegisterUnitSpawnRatio(harmony);
            RegisterVolunteerRecruitRate(harmony);
            RegisterVolunteerUpgradeRate(harmony);
            RegisterFreeBattleRetreat(harmony);
            RegisterDatedSaveNaming(harmony);
            RegisterWanderingClanSurvival(harmony);
            RegisterPlayerDeathNoAITakeover(harmony);
            RegisterEncyclopediaClanExileFilter(harmony);
            RegisterPrisonerRemoveRelation(harmony);
            RegisterTransportPartyMapVisibility(harmony);
            RegisterKillFeedDisplay(harmony);
            RegisterAutoResolveRebalance(harmony);
        }

        /// <summary>解析补丁类中的静态方法（含非公开），包装为 HarmonyMethod。</summary>
        private static HarmonyMethod Patch(Type patchType, string methodName)
        {
            MethodInfo method = AccessTools.Method(patchType, methodName);
            if (method == null)
            {
                throw new MissingMethodException(
                    $"HarmonyPatchRegistry: 找不到补丁方法 {patchType.FullName}.{methodName}");
            }
            return new HarmonyMethod(method);
        }

        // ── 旗帜士兵站位 ────────────────────────────────────────────

        private static void RegisterBannerBearerPosition(Harmony harmony)
        {
            var original = AccessTools.Method(
                typeof(DefaultFormationArrangementModel), "GetBannerBearerPositions");
            harmony.Patch(original,
                postfix: Patch(typeof(BannerBearerPositionPatch), "RepositionBannerBearerToLastRowCenter"));
        }

        // ── 王国加成标签页协调（5 个原生标签） ───────────────────────

        private static void RegisterBonusTabCoordination(Harmony harmony)
        {
            RegisterBonusTabPostfix(harmony, "ExecuteShowClan", "ClearOnClan");
            RegisterBonusTabPostfix(harmony, "ExecuteShowFiefs", "ClearOnFiefs");
            RegisterBonusTabPostfix(harmony, "ExecuteShowPolicies", "ClearOnPolicies");
            RegisterBonusTabPostfix(harmony, "ExecuteShowArmy", "ClearOnArmy");
            RegisterBonusTabPostfix(harmony, "ExecuteShowDiplomacy", "ClearOnDiplomacy");
        }

        private static void RegisterBonusTabPostfix(Harmony harmony, string targetMethod, string patchMethod)
        {
            var original = AccessTools.Method(typeof(KingdomManagementVM), targetMethod);
            harmony.Patch(original, postfix: Patch(typeof(BonusTabCoordinationPatch), patchMethod));
        }

        // ── 属性红利学习倍率 ─────────────────────────────────────────

        private static void RegisterCharacterDevelopmentModel(Harmony harmony)
        {
            var original = AccessTools.Method(
                typeof(DefaultCharacterDevelopmentModel), "CalculateLearningRate");
            harmony.Patch(original,
                postfix: Patch(typeof(CharacterDevelopmentModelPatch), "Postfix"));
        }

        // ── 经验倍率 ────────────────────────────────────────────────

        private static void RegisterHeroDeveloper(Harmony harmony)
        {
            var original = AccessTools.Method(typeof(HeroDeveloper), "GainRawXp");
            harmony.Patch(original,
                prefix: Patch(typeof(HeroDeveloperPatch), "Prefix"));
        }

        // ── 领土带兵上限（补丁侧加成） ───────────────────────────────

        private static void RegisterPartySizeLimitTerritoryBonus(Harmony harmony)
        {
            var original = AccessTools.Method(
                typeof(DefaultPartySizeLimitModel), "GetPartyMemberSizeLimit");
            harmony.Patch(original,
                postfix: Patch(typeof(PartySizeLimitTerritoryBonusPatch), "Postfix"));
        }

        // ── 运粮队部队上限补足（防超编减速） ─────────────────────────

        private static void RegisterTransportPartySizeLimit(Harmony harmony)
        {
            var original = AccessTools.Method(
                typeof(DefaultPartySizeLimitModel), "GetPartyMemberSizeLimit");
            harmony.Patch(original,
                postfix: Patch(typeof(TransportPartySizeLimitPatch), "Postfix"));
        }

        // ── NPC 家族部队数量加成（部队上限数量核心） ───────────────────

        private static void RegisterNpcClanPartyLimit(Harmony harmony)
        {
            var original = AccessTools.Method(
                typeof(DefaultClanTierModel), "GetPartyLimitForTier");
            harmony.Patch(original,
                postfix: Patch(typeof(NpcClanPartyLimitPatch), "Postfix"));
        }

        // ── 玩家攻城候选（1 前缀 + 1 后缀） ──────────────────────────

        private static void RegisterPlayerCapturedFief(Harmony harmony)
        {
            var applyBySiege = AccessTools.Method(
                typeof(ChangeOwnerOfSettlementAction), "ApplyBySiege");
            harmony.Patch(applyBySiege,
                prefix: Patch(typeof(PlayerCapturedFiefPatch), "RecordPlayerCaptured"));

            var narrowDown = AccessTools.Method(typeof(KingdomDecision), "NarrowDownCandidates");
            harmony.Patch(narrowDown,
                postfix: Patch(typeof(PlayerCapturedFiefPatch), "EnsurePlayerIsCandidate"));
        }

        // ── 禁止 AI 自动宣战 ────────────────────────────────────────

        private static void RegisterPreventAIWarDeclaration(Harmony harmony)
        {
            var original = AccessTools.Method(typeof(DeclareWarDecision), "IsAllowed");
            harmony.Patch(original,
                postfix: Patch(typeof(PreventAIWarDeclarationPatch), "Postfix"));
        }

        // ── 禁止家族部队捐兵（原手动注册） ───────────────────────────

        private static void RegisterPreventClanPartyDonateTroop(Harmony harmony)
        {
            var original = AccessTools.Method(
                typeof(GarrisonTroopsCampaignBehavior), "ManageGarrisonForParty",
                new[] { typeof(MobileParty), typeof(Settlement) });
            harmony.Patch(original,
                prefix: Patch(typeof(PreventClanPartyDonateTroopPatch), "Prefix"));
        }

        // ── 禁止家族部队被征召（原手动注册） ─────────────────────────

        private static void RegisterPreventClanPartyRecruitment(Harmony harmony)
        {
            var original = AccessTools.Method(
                typeof(DefaultArmyManagementCalculationModel), "CanLordCreateArmy");
            harmony.Patch(original,
                postfix: Patch(typeof(PreventClanPartyRecruitmentPatch), "Postfix"));
        }

        // ── 俘虏特殊 NPC 标注 ───────────────────────────────────────

        private static void RegisterPrisonerSpecialLabel(Harmony harmony)
        {
            var original = AccessTools.Method(typeof(PartyCharacterVM), "RefreshValues");
            harmony.Patch(original,
                postfix: Patch(typeof(PrisonerSpecialLabelPatch), "Postfix"));
        }

        // ── 战斗结算排序（6 个目标方法，逐个注册） ───────────────────

        private static void RegisterScoreboardSortOrder(Harmony harmony)
        {
            var transpiler = Patch(typeof(ScoreboardSortOrderPatch), "Transpiler");
            foreach (var name in ScoreboardSortOrderPatch.TargetMethodNames)
            {
                var original = AccessTools.Method(typeof(SPScoreboardSortControllerVM), name);
                if (original != null)
                    harmony.Patch(original, transpiler: transpiler);
            }
        }

        // ── 蹲下时盾牌方向 ──────────────────────────────────────────

        private static void RegisterShieldDirectionForCrouch(Harmony harmony)
        {
            var original = AccessTools.Method(typeof(ArrangementOrder), "GetShieldDirectionOfUnit");
            harmony.Patch(original,
                postfix: Patch(typeof(ShieldDirectionForCrouchPatch), "AdjustForCrouch"));
        }

        // ── 首排持盾排序修复（Prefix 整体替换原版收敛缺陷算法） ───────

        private static void RegisterFormationFrontRankShieldSort(Harmony harmony)
        {
            var original = AccessTools.Method(
                typeof(LineFormation), "SwitchFrontUnitTypesToFrontRows");
            if (original != null) // 目标方法缺失（版本差异）时安全跳过
                harmony.Patch(original,
                    prefix: Patch(typeof(FormationFrontRankShieldSortPatch), "Prefix"));
        }

        // ── 海战船只上限（DLC 类型动态解析，未装 DLC 时跳过） ────────

        private static void RegisterShipBattleLimit(Harmony harmony)
        {
            var original = NavalDeployLimitPatch.TargetMethod();
            if (original != null) // 未安装战帆 DLC 时 TargetMethod 返回 null，安全跳过
                harmony.Patch(original, postfix: Patch(typeof(NavalDeployLimitPatch), "Postfix"));
        }

        // ── 自定义战斗陆地战优先（主菜单入口默认先开陆地战配置） ───────
        // 原版 CustomBattleFactory 会把类型名含 "naval" 的提供者插到列表首位，
        // 战帆 DLC 安装后主菜单「自定义战斗」默认先进入海战配置。
        // 在入口方法 StartCustomBattle() 上挂 Prefix，点击时实时调整提供者顺序
        // （开启 = 陆地战优先；关闭 = 还原 DLC 原版海战优先），MCM 开关实时生效。

        private static void RegisterCustomBattleModeOrder(Harmony harmony)
        {
            var original = AccessTools.Method(typeof(CustomBattleFactory), "StartCustomBattle");
            if (original != null)
            {
                harmony.Patch(original,
                    prefix: Patch(typeof(CustomBattleModeOrderPatch), "Prefix"));
            }
        }

        // ── 攻城目标选择 ────────────────────────────────────────────

        private static void RegisterSiegeTargetSelection(Harmony harmony)
        {
            var original = AccessTools.Method(typeof(BesiegerCamp), "GetAttackTarget");
            harmony.Patch(original,
                prefix: Patch(typeof(SiegeTargetSelectionPatch), "Prefix"));
        }

        // ── 玩家投石精准（2 个目标共 3 个方法） ──────────────────────

        private static void RegisterSiegeWeapon(Harmony harmony)
        {
            var shoot = AccessTools.Method(typeof(RangedSiegeWeapon), "Shoot");
            harmony.Patch(shoot,
                prefix: Patch(typeof(SiegeWeaponPatch), "Prefix_Shoot"),
                postfix: Patch(typeof(SiegeWeaponPatch), "Postfix_Shoot"));

            var maxError = AccessTools.PropertyGetter(
                typeof(RangedSiegeWeapon), "MaximumBallisticError");
            harmony.Patch(maxError,
                prefix: Patch(typeof(SiegeWeaponPatch), "Prefix_GetError"));
        }

        // ── 标定坐标指挥 AI 投石 ────────────────────────────────────

        private static void RegisterCoordinateTargetAI(Harmony harmony)
        {
            var original = AccessTools.Method(typeof(RangedSiegeWeaponAi), "UpdateAim");
            harmony.Patch(original,
                prefix: Patch(typeof(CoordinateTargetAIPatch), "Prefix_UpdateAim"));
        }

        // ── 骑马长杆/骑枪必定击倒（2 个 MCM 开关，Postfix 改写判定结果） ──

        private static void RegisterMountedKnockDown(Harmony harmony)
        {
            var original = AccessTools.Method(
                typeof(MissionCombatMechanicsHelper), "DecideAgentKnockedDownByBlow");
            harmony.Patch(original,
                postfix: Patch(typeof(MountedKnockDownPatch), "Postfix"));
        }

        // ── 自定义出场比例（Prefix 整体替换，仅 HighLevel 生效） ──────────

        private static void RegisterUnitSpawnRatio(Harmony harmony)
        {
            var original = AccessTools.Method(
                typeof(DefaultTroopSupplierProbabilityModel),
                "EnqueueTroopSpawnProbabilitiesAccordingToUnitSpawnPrioritization");
            harmony.Patch(original,
                prefix: Patch(typeof(UnitSpawnRatioPatch), "Prefix"));
        }

        // ── 招募补充概率倍率（Postfix 乘系数） ──────────────────────────

        private static void RegisterVolunteerRecruitRate(Harmony harmony)
        {
            var original = AccessTools.Method(
                typeof(DefaultVolunteerModel), "GetDailyVolunteerProductionProbability");
            harmony.Patch(original,
                postfix: Patch(typeof(VolunteerRecruitRatePatch), "Postfix"));
        }

        // ── 志愿者升级概率倍率（Transpiler 替换 0.01f 常量） ─────────────

        private static void RegisterVolunteerUpgradeRate(Harmony harmony)
        {
            var original = AccessTools.Method(
                typeof(RecruitmentCampaignBehavior), "UpdateVolunteersOfNotablesInSettlement");
            harmony.Patch(original,
                transpiler: Patch(typeof(VolunteerUpgradeRatePatch), "Transpiler"));
        }

        // ── 加入战斗自由撤退（Postfix 强制放行，仅玩家加入的战斗） ───────

        private static void RegisterFreeBattleRetreat(Harmony harmony)
        {
            var original = AccessTools.Method(
                typeof(MapEventHelper), "CanMainPartyLeaveBattleCommonCondition");
            harmony.Patch(original,
                postfix: Patch(typeof(FreeBattleRetreatPatch), "Postfix"));
        }

        // ── 日期时间并行存档命名（2 个 Prefix，快速存档 + 自动存档） ───────

        private static void RegisterDatedSaveNaming(Harmony harmony)
        {
            var quickSave = AccessTools.Method(typeof(MBSaveLoad), "QuickSaveCurrentGame");
            var autoSave = AccessTools.Method(typeof(MBSaveLoad), "AutoSaveCurrentGame");
            harmony.Patch(quickSave,
                prefix: Patch(typeof(DatedSaveNamingPatch), "Prefix_QuickSave"));
            harmony.Patch(autoSave,
                prefix: Patch(typeof(DatedSaveNamingPatch), "Prefix_AutoSave"));
        }

        // ── 流亡家族永不灭亡（Prefix 拦截 28 天倒计时灭族入口） ─────────

        private static void RegisterWanderingClanSurvival(Harmony harmony)
        {
            var original = AccessTools.Method(
                typeof(FactionDiscontinuationCampaignBehavior), "DailyTickClan");
            harmony.Patch(original,
                prefix: Patch(typeof(WanderingClanSurvivalPatch), "Prefix"));
        }

        // ── 玩家阵亡不托管部队（Prefix 拦截 DelegateCommandToAI，仅玩家阵亡场景生效） ──

        private static void RegisterPlayerDeathNoAITakeover(Harmony harmony)
        {
            var original = AccessTools.Method(typeof(Team), "DelegateCommandToAI");
            harmony.Patch(original,
                prefix: Patch(typeof(PlayerDeathNoAITakeoverPatch), "Prefix"));
        }

        // ── 百科家族页「状态」筛选组新增流亡筛选（Postfix 追加筛选项，实时开关） ──

        private static void RegisterEncyclopediaClanExileFilter(Harmony harmony)
        {
            var original = AccessTools.Method(typeof(EncyclopediaPage), "GetFilterItems");
            harmony.Patch(original,
                postfix: Patch(typeof(EncyclopediaClanExileFilterPatch), "Postfix"));
        }

        // ── 部队界面移除俘虏加好感度（Postfix 补原版缺失的 +4 好感，实时开关） ──

        private static void RegisterPrisonerRemoveRelation(Harmony harmony)
        {
            var original = AccessTools.Method(
                typeof(PartyScreenHelper), "HandleReleasedAndTakenPrisoners");
            harmony.Patch(original,
                postfix: Patch(typeof(PrisonerRemoveRelationPatch), "Postfix"));
        }

        // ── 运粮队大地图全局可见 ───────────────────────────────────────────
        // 可见性状态本身（IsVisible=setter 写入、VisualTrackerManager 注册、SetPartyUsedByQuest、每 tick 保活）
        // 由 Behaviors/FoodTransportSupportBehavior 维护。
        // 此处注册：PartyBase.UpdateVisibilityAndInspected prefix（防原版每帧视野计算回滚运粮队可见性，
        // 目标在 TaleWorlds.CampaignSystem.dll 核心程序集，启动时注册安全）。
        // UI 层兜底（PartyNameplateVM.RefreshBinding / MapTrackerProvider.CanAddMobileParty postfix）不在此注册——
        // 目标类型位于 SandBox.ViewModelCollection.dll（UI 程序集），在 SubModule 加载早期 patch 其方法可能
        // 触发程序集静态初始化在临界期执行导致读档挂起；改为由 FoodTransportSupportBehavior.OnCampaignTick
        // 在加载完成后调用 TransportPartyMapVisibilityPatch.EnsureUiPatchesRegistered() 延迟注册。

        private static void RegisterTransportPartyMapVisibility(Harmony harmony)
        {
            var original = AccessTools.Method(typeof(PartyBase), "UpdateVisibilityAndInspected");
            if (original != null)
            {
                harmony.Patch(original,
                    prefix: Patch(typeof(TransportPartyMapVisibilityPatch), "UpdateVisibilityAndInspectedPrefix"));
            }
        }

        // ── 战场击杀信息流显示优化（条目上限 + 旧条目文字渐进缩小） ───────
        // 目标 1：SPGeneralKillNotificationVM.OnAgentRemoved（VM 层）——限制
        //          NotificationList 同时显示的条目数，超出立即移除最旧。
        // 目标 2：SingleplayerGeneralKillFeedWidget.OnUpdate（Widget 层）——
        //          按条目索引渐进缩小旧条目文字（经 Brush 懒克隆独立改 FontSize）。
        // 均在 TaleWorlds.MountAndBlade.ViewModelCollection / GauntletUI.Widgets
        // 程序集（游戏主程序集，SubModule 加载后立即可用），启动注册安全。

        private static void RegisterKillFeedDisplay(Harmony harmony)
        {
            var vmOriginal = AccessTools.Method(
                typeof(SPGeneralKillNotificationVM), "OnAgentRemoved");
            if (vmOriginal != null)
            {
                harmony.Patch(vmOriginal,
                    postfix: Patch(typeof(KillFeedDisplayPatch), "LimitNotificationListPostfix"));
            }

            var widgetOriginal = AccessTools.Method(
                typeof(SingleplayerGeneralKillFeedWidget), "OnUpdate");
            if (widgetOriginal != null)
            {
                harmony.Patch(widgetOriginal,
                    postfix: Patch(typeof(KillFeedDisplayPatch), "ShrinkOldEntriesPostfix"));
            }
        }

        // ── 坐镇指挥模拟重平衡（7 个目标方法，移植自 AutoResolveRebalanced） ──
        // 1. MapEvent.SimulateBattleRound      Postfix 追加回合 —— 兵力悬殊 >10:1 且未分胜负时给大兵力侧补 10 轮
        // 2. DefaultCombatSimulationModel.SimulateHit  Postfix 伤害 —— 纯武器伤害模型（4×4 武器优先表 + 护甲减伤 + 盾牌格挡）
        // 3. MapEventSide.ApplySimulationDamageToSelectedTroop  Prefix 伤亡 —— 累计 HP 模型整体替换
        // 4. MapEventSide.AllocateTroops       Postfix 状态 —— 登记/更新每侧累计 HP 字典
        // 5. MapEventSide.EndSimulation        Prefix 状态 —— 回合结束前存剩余兵数与平均 HP 供续算
        // 6. DefaultCombatSimulationModel.GetSimulationTickInterval  Postfix 加速 —— 仅 AI 对 AI 战斗缩短结算间隔
        // 7. DefaultCombatSimulationModel.GetSimulationTicksForBattleRound  Postfix 上限 —— 攻击频次比 clamp 到 (兵力比上限)^0.6
        // 目标均为游戏核心程序集（TaleWorlds.CampaignSystem.dll）方法，启动注册安全。
        // 旧版目标名 SimulateBattleForRounds / GetSimulatedDamage 在 1.4.5 中已分别更名/下沉到模型类，按行为对齐。

        private static void RegisterAutoResolveRebalance(Harmony harmony)
        {
            var simulateBattleRound = AccessTools.Method(typeof(MapEvent), "SimulateBattleRound");
            if (simulateBattleRound != null)
            {
                harmony.Patch(simulateBattleRound,
                    postfix: Patch(typeof(AutoResolveExtraRoundsPatch), "Postfix"));
            }

            var simulateHit = AccessTools.Method(
                typeof(DefaultCombatSimulationModel), "SimulateHit",
                new[]
                {
                    typeof(CharacterObject), typeof(CharacterObject), typeof(PartyBase),
                    typeof(PartyBase), typeof(float), typeof(MapEvent), typeof(float), typeof(float)
                });
            if (simulateHit != null)
            {
                harmony.Patch(simulateHit,
                    postfix: Patch(typeof(AutoResolveDamagePatch), "Postfix"));
            }

            var applyDamage = AccessTools.Method(typeof(MapEventSide), "ApplySimulationDamageToSelectedTroop");
            if (applyDamage != null)
            {
                harmony.Patch(applyDamage,
                    prefix: Patch(typeof(AutoResolveTroopCasualtyPatch), "Prefix"));
            }

            var allocateTroops = AccessTools.Method(typeof(MapEventSide), "AllocateTroops");
            if (allocateTroops != null)
            {
                harmony.Patch(allocateTroops,
                    postfix: Patch(typeof(AutoResolveAllocateTroopsPatch), "Postfix"));
            }

            var endSimulation = AccessTools.Method(typeof(MapEventSide), "EndSimulation");
            if (endSimulation != null)
            {
                harmony.Patch(endSimulation,
                    prefix: Patch(typeof(AutoResolveEndSimulationPatch), "Prefix"));
            }

            var getSimulationTickInterval = AccessTools.Method(typeof(DefaultCombatSimulationModel), "GetSimulationTickInterval");
            if (getSimulationTickInterval != null)
            {
                harmony.Patch(getSimulationTickInterval,
                    postfix: Patch(typeof(AutoResolveSimulationSpeedPatch), "Postfix"));
            }

            var getSimulationTicksForBattleRound = AccessTools.Method(typeof(DefaultCombatSimulationModel), "GetSimulationTicksForBattleRound");
            if (getSimulationTicksForBattleRound != null)
            {
                harmony.Patch(getSimulationTicksForBattleRound,
                    postfix: Patch(typeof(AutoResolveAttackRatioCapPatch), "Postfix"));
            }
        }
    }
}
