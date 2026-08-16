using Bannerlord.UIExtenderEx;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

using MutliLittleFixes.Behaviors;

namespace MutliLittleFixes
{
    public class SubModule : MBSubModuleBase
    {
        private Harmony? _harmony;
        private UIExtender? _uiExtender;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            // 注册 Harmony 补丁 — 全部显式注册（见 Patches/HarmonyPatchRegistry.cs），
            // 不使用 PatchAll 自动发现。新补丁必须在注册器中逐条登记。
            _harmony = new Harmony("MutliLittleFixes");
            Patches.HarmonyPatchRegistry.Register(_harmony);

            // LordStrengthTypeDefiner 由存档系统自动发现。
            // 存档系统会扫描所有程序集，查找非抽象的 SaveableTypeDefiner 子类，
            // 并通过 Activator.CreateInstance 自动实例化它们（详见 DefinitionContext.cs:297-299）。
            // 无需显式注册。

            // ── 启用 UIExtenderEx UI 注入 ──────────────────────────────────
            // 扫描当前程序集，自动发现 [PrefabExtension] 和 [ViewModelMixin] 并注册。
            // 在 OnBeforeInitialModuleScreenSetAsRoot 时生效，对当前运行无影响。
            _uiExtender = UIExtender.Create("MutliLittleFixes");
            _uiExtender.Register(typeof(SubModule).Assembly);
            _uiExtender.Enable();
        }

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);
            mission.AddMissionBehavior(new SiegeTrajectoryBehavior());
            mission.AddMissionBehavior(new AutoCrouchMissionLogic());
            mission.AddMissionBehavior(new RangedNoAmmoBehavior());

            // 盾牌插地：带盾远程步兵把盾插在地上作障碍物（F11 插盾 / J 收盾，仅玩家手动操作）
        // 由 MCM "Formations & Battle 类 → Shield Planting" 分组开关实时控制
        mission.AddMissionBehavior(new ShieldPlantingBehavior());

        // 由 MCM "Formations & Battle 类 → Ranged Shield Formation" 分组开关实时控制
        mission.AddMissionBehavior(new ShieldBearerFormationBehavior());

            // 调试: 按 , 键随机 5% 远程士兵弹药归零（测试 RangedNoAmmoBehavior 用）
            // 由 MCM "调试 → 远程弹药归零调试" 开关控制，游戏中实时启用/禁用
            mission.AddMissionBehavior(new RangedNoAmmoDebugBehavior());

            // 调试圈/点渲染视图（由 MCM "调试 → 调试圈/点渲染视图" 开关实时控制）
            mission.AddMissionBehavior(new PlayerCircleView());

            // ORCA 避让调试（【已禁用】commit 304cc5d 新增，功能整体注释掉，不再实例化）
            // 启用条件：取消下方注释 + 恢复 Settings.cs 中 ORCA 配置 + 取消 OrcaSystem/*.cs 的 #if false 包裹
            // mission.AddMissionBehavior(new OrcaDebugBehavior());
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
                campaignStarter.AddBehavior(new FoodTransportSupportBehavior());
                campaignStarter.AddBehavior(new VillageRebuildBehavior());
            }
        }

        protected override void OnSubModuleUnloaded()
        {
            // 卸载 Harmony 补丁
            _harmony?.UnpatchAll("MutliLittleFixes");
            _harmony = null;

            base.OnSubModuleUnloaded();
        }
    }
}
