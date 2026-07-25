using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Core;

namespace ExampleMod
{
    internal sealed class Settings : AttributeGlobalSettings<Settings>
    {
        public override string Id => "ExampleMod_v1";
        public override string DisplayName => "ExampleMod";
        public override string FolderName => "ExampleMod";
        public override string FormatType => "json2";

        [SettingPropertyFloatingInteger("经验倍率", 0.1f, 1000.0f, "#0.0x", Order = 0, RequireRestart = false, HintText = "主角获取经验的倍率（影响所有技能经验获取和角色等级提升速度）")]
        [SettingPropertyGroup("经验设置")]
        public float ExperienceMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger("活力 (Vigor)", 0.1f, 1000.0f, "#0.0x", Order = 1, RequireRestart = false, HintText = "活力对应技能的红利学习倍率")]
        [SettingPropertyGroup("属性增长倍率")]
        public float VigorMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger("控制 (Control)", 0.1f, 1000.0f, "#0.0x", Order = 2, RequireRestart = false, HintText = "控制对应技能的红利学习倍率")]
        [SettingPropertyGroup("属性增长倍率")]
        public float ControlMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger("耐力 (Endurance)", 0.1f, 1000.0f, "#0.0x", Order = 3, RequireRestart = false, HintText = "耐力对应技能的红利学习倍率")]
        [SettingPropertyGroup("属性增长倍率")]
        public float EnduranceMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger("狡诈 (Cunning)", 0.1f, 1000.0f, "#0.0x", Order = 4, RequireRestart = false, HintText = "狡诈对应技能的红利学习倍率")]
        [SettingPropertyGroup("属性增长倍率")]
        public float CunningMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger("社交 (Social)", 0.1f, 1000.0f, "#0.0x", Order = 5, RequireRestart = false, HintText = "社交对应技能的红利学习倍率")]
        [SettingPropertyGroup("属性增长倍率")]
        public float SocialMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger("智力 (Intelligence)", 0.1f, 1000.0f, "#0.0x", Order = 6, RequireRestart = false, HintText = "智力对应技能的红利学习倍率")]
        [SettingPropertyGroup("属性增长倍率")]
        public float IntelligenceMultiplier { get; set; } = 1.0f;

        [SettingPropertyBool("禁止家族部队被征召", Order = 10, RequireRestart = false, HintText = "阻止AI领主将玩家家族的非主角部队征召入军团")]
        [SettingPropertyGroup("家族部队控制")]
        public bool PreventClanPartyRecruitment { get; set; } = true;

        [SettingPropertyBool("禁止家族部队捐兵", Order = 11, RequireRestart = false, HintText = "阻止玩家家族的非主角部队向要塞捐兵")]
        [SettingPropertyGroup("家族部队控制")]
        public bool PreventClanPartyDonateTroops { get; set; } = true;

        private bool _forceArmyCreationTest;
        [SettingPropertyBool("俘虏特殊NPC标注", Order = 12, RequireRestart = false, HintText = "在部队界面的俘虏标签页中，为统治者/领主/雇佣兵头子标注身份")]
        [SettingPropertyGroup("UI")]
        public bool PrisonerSpecialLabel { get; set; } = true;

        [SettingPropertyBool("强制创建军团测试", Order = 13, RequireRestart = false, HintText = "让我方非玩家家族领袖尝试创建军团，用于测试屏蔽效果")]
        [SettingPropertyGroup("家族部队控制")]
        public bool ForceArmyCreationTest
        {
            get => _forceArmyCreationTest;
            set
            {
                if (value)
                {
                    _forceArmyCreationTest = false;
                    Patches.TestArmyCreationHelper.TriggerTestArmyCreation();
                }
            }
        }

        [SettingPropertyBool("启用领主释放补兵", Order = 20, RequireRestart = false, HintText = "启用/禁用领主释放后补兵功能")]
        [SettingPropertyGroup("领主释放补兵")]
        public bool RestorationEnabled { get; set; } = true;

        [SettingPropertyInteger("补兵所需天数", 1, 30, "0", Order = 21, RequireRestart = false, HintText = "释放后补兵所需天数")]
        [SettingPropertyGroup("领主释放补兵")]
        public int RestorationDays { get; set; } = 7;

        [SettingPropertyFloatingInteger("恢复兵力比例", 0.0f, 1.0f, "0.0", Order = 22, RequireRestart = false, HintText = "恢复兵力占队伍上限的比例(0=关闭)")]
        [SettingPropertyGroup("领主释放补兵")]
        public float RestorationPartySizeRatio { get; set; } = 0.6f;

        [SettingPropertyFloatingInteger("Tier1-2兵种占比", 0.0f, 1.0f, "0.0", Order = 23, RequireRestart = false, HintText = "Tier1-2兵种占比")]
        [SettingPropertyGroup("领主释放补兵")]
        public float RestorationTier12Ratio { get; set; } = 0.50f;

        [SettingPropertyFloatingInteger("Tier3-4兵种占比", 0.0f, 1.0f, "0.0", Order = 24, RequireRestart = false, HintText = "Tier3-4兵种占比")]
        [SettingPropertyGroup("领主释放补兵")]
        public float RestorationTier34Ratio { get; set; } = 0.35f;

        [SettingPropertyFloatingInteger("Tier5-6兵种占比", 0.0f, 1.0f, "0.0", Order = 25, RequireRestart = false, HintText = "Tier5-6兵种占比")]
        [SettingPropertyGroup("领主释放补兵")]
        public float RestorationTier56Ratio { get; set; } = 0.15f;

        [SettingPropertyInteger("每兵金币", 0, 100000, "0", Order = 26, RequireRestart = false, HintText = "每兵给予领主的金币数量(0=不给金币)")]
        [SettingPropertyGroup("领主释放补兵")]
        public int RestorationGoldPerTroop { get; set; } = 0;

        [SettingPropertyBool("启用领土带兵上限", Order = 27, RequireRestart = false, HintText = "启用/禁用领土丧失补偿功能")]
        [SettingPropertyGroup("领土带兵上限")]
        public bool TerritoryBonusEnabled { get; set; } = true;

        [SettingPropertyInteger("城镇补偿值", 0, 50, "0", Order = 28, RequireRestart = false, HintText = "每丢失一座城镇增加的队伍上限(衰减前)")]
        [SettingPropertyGroup("领土带兵上限")]
        public int TerritoryBonusTownValue { get; set; } = 5;

        [SettingPropertyInteger("城堡补偿值", 0, 50, "0", Order = 29, RequireRestart = false, HintText = "每丢失一座城堡增加的队伍上限(衰减前)")]
        [SettingPropertyGroup("领土带兵上限")]
        public float TerritoryBonusCastleValue { get; set; } = 3.0f;

        [SettingPropertyInteger("城镇削减值", 0, 50, "0", Order = 30, RequireRestart = false, HintText = "每征服一座城镇减少的补偿值")]
        [SettingPropertyGroup("领土带兵上限")]
        public int TerritoryBonusTownReduction { get; set; } = 5;

        [SettingPropertyInteger("城堡削减值", 0, 50, "0", Order = 31, RequireRestart = false, HintText = "每征服一座城堡减少的补偿值")]
        [SettingPropertyGroup("领土带兵上限")]
        public int TerritoryBonusCastleReduction { get; set; } = 3;

        [SettingPropertyFloatingInteger("衰减乘数", 0.0f, 1.0f, "0.0", Order = 32, RequireRestart = false, HintText = "连续丢失领土的衰减乘数(1.0=线性)")]
        [SettingPropertyGroup("领土带兵上限")]
        public float TerritoryBonusDiminishRate { get; set; } = 0.85f;

        [SettingPropertyInteger("最大补偿上限", 0, 500, "0", Order = 33, RequireRestart = false, HintText = "王国可累积的最大补偿值")]
        [SettingPropertyGroup("领土带兵上限")]
        public int TerritoryBonusMaxCap { get; set; } = 200;

        [SettingPropertyBool("仅封臣生效", Order = 34, RequireRestart = false, HintText = "仅对封臣家族生效(不包括雇佣兵)")]
        [SettingPropertyGroup("领土带兵上限")]
        public bool TerritoryBonusVassalsOnly { get; set; } = true;

        [SettingPropertyBool("启用调试日志", Order = 35, RequireRestart = false, HintText = "在游戏界面左下角显示调试日志信息，用于排查功能问题")]
        [SettingPropertyGroup("调试")]
        public bool EnableDebugLogging { get; set; } = false;

        public float GetAttributeMultiplier(CharacterAttribute attribute)
        {
            if (attribute == DefaultCharacterAttributes.Vigor)
                return VigorMultiplier;
            if (attribute == DefaultCharacterAttributes.Control)
                return ControlMultiplier;
            if (attribute == DefaultCharacterAttributes.Endurance)
                return EnduranceMultiplier;
            if (attribute == DefaultCharacterAttributes.Cunning)
                return CunningMultiplier;
            if (attribute == DefaultCharacterAttributes.Social)
                return SocialMultiplier;
            if (attribute == DefaultCharacterAttributes.Intelligence)
                return IntelligenceMultiplier;
            return 1.0f;
        }
    }
}
