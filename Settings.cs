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

        [SettingPropertyInteger("全局默认上限", 10, 1024, "0", Order = 7, RequireRestart = false, HintText = "所有技能的默认等级上限（1024=原版硬上限）")]
        [SettingPropertyGroup("技能等级上限")]
        public int SkillCapDefault { get; set; } = 1024;

        [SettingPropertyInteger("活力 (Vigor)", 10, 1024, "0", Order = 8, RequireRestart = false, HintText = "活力类技能（单手/双手/长杆）的等级上限")]
        [SettingPropertyGroup("技能等级上限")]
        public int VigorSkillCap { get; set; } = 1024;

        [SettingPropertyInteger("控制 (Control)", 10, 1024, "0", Order = 9, RequireRestart = false, HintText = "控制类技能（弓/弩/投掷）的等级上限")]
        [SettingPropertyGroup("技能等级上限")]
        public int ControlSkillCap { get; set; } = 1024;

        [SettingPropertyInteger("耐力 (Endurance)", 10, 1024, "0", Order = 10, RequireRestart = false, HintText = "耐力类技能（骑术/跑动/锻造）的等级上限")]
        [SettingPropertyGroup("技能等级上限")]
        public int EnduranceSkillCap { get; set; } = 1024;

        [SettingPropertyInteger("狡诈 (Cunning)", 10, 1024, "0", Order = 11, RequireRestart = false, HintText = "狡诈类技能（侦查/战术/流氓）的等级上限")]
        [SettingPropertyGroup("技能等级上限")]
        public int CunningSkillCap { get; set; } = 1024;

        [SettingPropertyInteger("社交 (Social)", 10, 1024, "0", Order = 12, RequireRestart = false, HintText = "社交类技能（魅力/统御/交易）的等级上限")]
        [SettingPropertyGroup("技能等级上限")]
        public int SocialSkillCap { get; set; } = 1024;

        [SettingPropertyInteger("智力 (Intelligence)", 10, 1024, "0", Order = 13, RequireRestart = false, HintText = "智力类技能（管理/医术/工程）的等级上限")]
        [SettingPropertyGroup("技能等级上限")]
        public int IntelligenceSkillCap { get; set; } = 1024;

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

        [SettingPropertyInteger("放弃补兵天数", 1, 60, "0", Order = 27, RequireRestart = false, HintText = "超过此天数领主仍无队伍则放弃补兵")]
        [SettingPropertyGroup("领主释放补兵")]
        public int RestorationAbandonDays { get; set; } = 15;

        [SettingPropertyBool("启用领土带兵上限", Order = 28, RequireRestart = false, HintText = "启用/禁用领土丧失补偿功能")]
        [SettingPropertyGroup("领土带兵上限")]
        public bool TerritoryBonusEnabled { get; set; } = true;

        [SettingPropertyInteger("城镇补偿值", 0, 50, "0", Order = 29, RequireRestart = false, HintText = "每丢失一座城镇增加的队伍上限(衰减前)")]
        [SettingPropertyGroup("领土带兵上限")]
        public int TerritoryBonusTownValue { get; set; } = 5;

        [SettingPropertyInteger("城堡补偿值", 0, 50, "0", Order = 30, RequireRestart = false, HintText = "每丢失一座城堡增加的队伍上限(衰减前)")]
        [SettingPropertyGroup("领土带兵上限")]
        public float TerritoryBonusCastleValue { get; set; } = 3.0f;

        [SettingPropertyFloatingInteger("衰减乘数", 0.0f, 1.0f, "0.0", Order = 31, RequireRestart = false, HintText = "连续丢失领土的衰减乘数(1.0=线性)")]
        [SettingPropertyGroup("领土带兵上限")]
        public float TerritoryBonusDiminishRate { get; set; } = 0.85f;

        [SettingPropertyInteger("最大补偿上限", 0, 500, "0", Order = 32, RequireRestart = false, HintText = "王国可累积的最大补偿值")]
        [SettingPropertyGroup("领土带兵上限")]
        public int TerritoryBonusMaxCap { get; set; } = 200;

        [SettingPropertyInteger("征服固化天数", 0, 365, "0", Order = 33, RequireRestart = false, HintText = "占领城池超过此天数后视作本国领土，不再抵消丢失产生的补偿(0=关闭，84=一年)")]
        [SettingPropertyGroup("领土带兵上限")]
        public int ConquestSolidifyDays { get; set; } = 21;

        [SettingPropertyInteger("丢失过期天数", 0, 365, "0", Order = 34, RequireRestart = false, HintText = "丢失城池超过此天数后视作他国领土，不再参与补偿计算(0=关闭，84=一年)")]
        [SettingPropertyGroup("领土带兵上限")]
        public int LossExpireDays { get; set; } = 21;

        [SettingPropertyBool("仅封臣生效", Order = 35, RequireRestart = false, HintText = "仅对封臣家族生效(不包括雇佣兵)")]
        [SettingPropertyGroup("领土带兵上限")]
        public bool TerritoryBonusVassalsOnly { get; set; } = true;

        [SettingPropertyInteger("海战船只上限", 3, 8, "0", Order = 36, RequireRestart = false,
            HintText = "参与海战/沿海掠夺时，玩家可同时出战的最大船只数量（至少3艘，最多8艘）")]
        [SettingPropertyGroup("海战设置")]
        public int NavalBattleShipLimit { get; set; } = 3;

        [SettingPropertyBool("家族成员可用提醒", Order = 37, RequireRestart = false, HintText = "家族成员从俘虏释放/逃脱变为可用状态时，在屏幕中央弹出 toast 提示")]
        [SettingPropertyGroup("通知")]
        public bool CompanionAutoRecallEnabled { get; set; } = true;

        [SettingPropertyBool("启用调试日志", Order = 38, RequireRestart = false, HintText = "在游戏界面左下角显示调试日志信息，用于排查功能问题")]
        [SettingPropertyGroup("调试")]
        public bool EnableDebugLogging { get; set; } = false;

        [SettingPropertyBool("远程弹药归零调试（按,键）", Order = 39, RequireRestart = false, HintText = "战斗中按 , 键随机将 5% 远程士兵弹药强制归零，用于测试第9队移交逻辑")]
        [SettingPropertyGroup("调试")]
        public bool RangedNoAmmoDebugEnabled { get; set; } = false;

        [SettingPropertyBool("禁止AI自动宣战", Order = 40, RequireRestart = false, HintText = "玩家是国王时，禁止属下领主（AI）自动发起宣战决策")]
        [SettingPropertyGroup("外交设置")]
        public bool PreventAIWarDeclaration { get; set; } = true;

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

        public int GetSkillCap(SkillObject skill)
        {
            int cap = SkillCapDefault;
            CharacterAttribute[]? attributes = skill.Attributes;
            if (attributes != null)
            {
                foreach (CharacterAttribute attr in attributes)
                {
                    int attrCap = GetAttributeCap(attr);
                    if (attrCap < cap)
                        cap = attrCap;
                }
            }
            return cap;
        }

        private int GetAttributeCap(CharacterAttribute attribute)
        {
            if (attribute == DefaultCharacterAttributes.Vigor)
                return VigorSkillCap;
            if (attribute == DefaultCharacterAttributes.Control)
                return ControlSkillCap;
            if (attribute == DefaultCharacterAttributes.Endurance)
                return EnduranceSkillCap;
            if (attribute == DefaultCharacterAttributes.Cunning)
                return CunningSkillCap;
            if (attribute == DefaultCharacterAttributes.Social)
                return SocialSkillCap;
            if (attribute == DefaultCharacterAttributes.Intelligence)
                return IntelligenceSkillCap;
            return SkillCapDefault;
        }
    }
}
