using HarmonyLib;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.GameComponents;
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
            Settings? settings = Settings.Instance;
            if (settings == null)
                return;

            if (settings.PreventClanPartyRecruitment)
            {
                var original = AccessTools.Method(
                    typeof(DefaultArmyManagementCalculationModel), "CanLordCreateArmy");
                var postfix = new HarmonyMethod(
                    typeof(Patches.PreventClanPartyRecruitmentPatch), "Postfix");
                _harmony!.Patch(original, postfix: postfix);
            }

            if (settings.PreventClanPartyDonateTroops)
            {
                var original = AccessTools.Method(
                    typeof(GarrisonTroopsCampaignBehavior), "OnSettlementEntered");
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
