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
