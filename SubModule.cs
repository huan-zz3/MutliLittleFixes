using HarmonyLib;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.MountAndBlade;

namespace ExampleMod
{
    public class SubModule : MBSubModuleBase
    {
        private Harmony? _harmony;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            // 注册 Harmony 补丁 — PatchAll 自动发现带 [HarmonyPatch] 的补丁
            _harmony = new Harmony("ExampleMod");
            _harmony.PatchAll();

            // 条件性补丁 — 仅 MCM 开关开启时才安装，关闭则完全不 patch 原版方法
            ApplyConditionalPatches();
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

            // 手动注册 PlayerCircleView（调试用圆圈/点渲染）
            // 取消注释下一行即可启用
            // mission.AddMissionBehavior(new PlayerCircleView());
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
