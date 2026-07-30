using Bannerlord.UIExtenderEx;
using HarmonyLib;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;

using ExampleMod.Behaviors;

namespace ExampleMod
{
    public class SubModule : MBSubModuleBase
    {
        private Harmony? _harmony;
        private UIExtender? _uiExtender;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            // 注册 Harmony 补丁 — PatchAll 自动发现带 [HarmonyPatch] 的补丁
            _harmony = new Harmony("ExampleMod");
            _harmony.PatchAll();

            // 条件性补丁 — 仅 MCM 开关开启时才安装，关闭则完全不 patch 原版方法
            ApplyConditionalPatches();

            // LordStrengthTypeDefiner 由存档系统自动发现。
            // 存档系统会扫描所有程序集，查找非抽象的 SaveableTypeDefiner 子类，
            // 并通过 Activator.CreateInstance 自动实例化它们（详见 DefinitionContext.cs:297-299）。
            // 无需显式注册。

            // ── 启用 UIExtenderEx UI 注入 ──────────────────────────────────
            // 扫描当前程序集，自动发现 [PrefabExtension] 和 [ViewModelMixin] 并注册。
            // 在 OnBeforeInitialModuleScreenSetAsRoot 时生效，对当前运行无影响。
            _uiExtender = UIExtender.Create("ExampleMod");
            _uiExtender.Register(typeof(SubModule).Assembly);
            _uiExtender.Enable();
        }

        private void ApplyConditionalPatches()
        {
            // PreventClanPartyRecruitmentPatch 内部有运行时 MCM 开关检查（null-safe），
            // 所以始终安装 Patch，由运行时根据设置决定是否执行过滤。
            {
                var original = AccessTools.Method(
                    typeof(DefaultArmyManagementCalculationModel), "CanLordCreateArmy");
                var postfix = new HarmonyMethod(
                    typeof(Patches.PreventClanPartyRecruitmentPatch), "Postfix");
                _harmony!.Patch(original, postfix: postfix);
            }

            // PreventClanPartyDonateTroopPatch Patch ManageGarrisonForParty（而非 OnSettlementEntered），
            // 只阻断驻军管理（含捐兵），不影响 OnSettlementEntered 中的军团主管理驻军等关键逻辑。
            {
                var original = AccessTools.Method(
                    typeof(GarrisonTroopsCampaignBehavior), "ManageGarrisonForParty",
                    new[] { typeof(MobileParty), typeof(Settlement) });
                var prefix = new HarmonyMethod(
                    typeof(Patches.PreventClanPartyDonateTroopPatch), "Prefix");
                _harmony!.Patch(original, prefix: prefix);
            }
        }

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);
            mission.AddMissionBehavior(new SiegeTrajectoryBehavior());
            mission.AddMissionBehavior(new AutoCrouchMissionLogic());
            mission.AddMissionBehavior(new RangedNoAmmoBehavior());

            // 调试: 按 , 键随机 5% 远程士兵弹药归零（测试 RangedNoAmmoBehavior 用）
            // 正式发布前可删除或注释掉此行
            mission.AddMissionBehavior(new RangedNoAmmoDebugBehavior());

            // 手动注册 PlayerCircleView（调试用圆圈/点渲染）
            // 取消注释下一行即可启用
            // mission.AddMissionBehavior(new PlayerCircleView());
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);

            // 注册领主强度行为 — 使用 CampaignGameStarter 标准模式
            if (game.GameType is Campaign && gameStarterObject is CampaignGameStarter campaignStarter)
            {
                campaignStarter.AddBehavior(new LordTroopRestorationBehavior());
                campaignStarter.AddBehavior(new KingdomTerritoryBonusBehavior());
                campaignStarter.AddBehavior(new SkillLevelCapBehavior());
                campaignStarter.AddBehavior(new CompanionAutoRecallBehavior());
            }
        }

        protected override void OnSubModuleUnloaded()
        {
            // 卸载 Harmony 补丁
            _harmony?.UnpatchAll("ExampleMod");
            _harmony = null;

            base.OnSubModuleUnloaded();
        }
    }
}
