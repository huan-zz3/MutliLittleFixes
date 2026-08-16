using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace MutliLittleFixes
{
    internal sealed class Settings : AttributeGlobalSettings<Settings>
    {
        public override string Id => "MutliLittleFixes_v1";

        public override string DisplayName
        {
            get
            {
                return new TextObject("{=mlf_mod_name}MutliLittleFixes", null).ToString();
            }
        }

        public override string FolderName => "MutliLittleFixes";
        public override string FormatType => "json2";

        [SettingPropertyFloatingInteger("{=mlf_exp_rate}Experience Rate", 0.1f, 1000.0f, "#0.0x", Order = 0, RequireRestart = false, HintText = "{=mlf_exp_rate_hint}Multiplier for experience gained by the main hero (affects all skill XP gain and character leveling speed)")]
        [SettingPropertyGroup("{=mlf_group_exp}Experience Settings")]
        public float ExperienceMultiplier { get; set; } = 1.0f;

        [SettingPropertyBool("{=mlf_exp_enabled}Enable Experience Rate", Order = 1, RequireRestart = false, HintText = "{=mlf_exp_enabled_hint}Real-time toggle: when disabled, the experience rate feature is completely inactive (value is kept but not applied)")]
        [SettingPropertyGroup("{=mlf_group_exp}Experience Settings")]
        public bool ExperienceMultiplierEnabled { get; set; } = false;

        [SettingPropertyFloatingInteger("{=mlf_attr_vigor}Vigor (Vigor)", 0.1f, 1000.0f, "#0.0x", Order = 1, RequireRestart = false, HintText = "{=mlf_attr_vigor_hint}Attribute learning bonus multiplier for skills governed by Vigor")]
        [SettingPropertyGroup("{=mlf_group_attr}Attribute Growth Rates")]
        public float VigorMultiplier { get; set; } = 1.0f;

        [SettingPropertyBool("{=mlf_attr_enabled}Enable Attribute Growth Rates", Order = 0, RequireRestart = false, HintText = "{=mlf_attr_enabled_hint}Real-time toggle: when disabled, the attribute learning bonus feature is completely inactive (values are kept but not applied)")]
        [SettingPropertyGroup("{=mlf_group_attr}Attribute Growth Rates")]
        public bool AttributeLearningBonusEnabled { get; set; } = false;

        [SettingPropertyFloatingInteger("{=mlf_attr_control}Control (Control)", 0.1f, 1000.0f, "#0.0x", Order = 2, RequireRestart = false, HintText = "{=mlf_attr_control_hint}Attribute learning bonus multiplier for skills governed by Control")]
        [SettingPropertyGroup("{=mlf_group_attr}Attribute Growth Rates")]
        public float ControlMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger("{=mlf_attr_endurance}Endurance (Endurance)", 0.1f, 1000.0f, "#0.0x", Order = 3, RequireRestart = false, HintText = "{=mlf_attr_endurance_hint}Attribute learning bonus multiplier for skills governed by Endurance")]
        [SettingPropertyGroup("{=mlf_group_attr}Attribute Growth Rates")]
        public float EnduranceMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger("{=mlf_attr_cunning}Cunning (Cunning)", 0.1f, 1000.0f, "#0.0x", Order = 4, RequireRestart = false, HintText = "{=mlf_attr_cunning_hint}Attribute learning bonus multiplier for skills governed by Cunning")]
        [SettingPropertyGroup("{=mlf_group_attr}Attribute Growth Rates")]
        public float CunningMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger("{=mlf_attr_social}Social (Social)", 0.1f, 1000.0f, "#0.0x", Order = 5, RequireRestart = false, HintText = "{=mlf_attr_social_hint}Attribute learning bonus multiplier for skills governed by Social")]
        [SettingPropertyGroup("{=mlf_group_attr}Attribute Growth Rates")]
        public float SocialMultiplier { get; set; } = 1.0f;

        [SettingPropertyFloatingInteger("{=mlf_attr_intelligence}Intelligence (Intelligence)", 0.1f, 1000.0f, "#0.0x", Order = 6, RequireRestart = false, HintText = "{=mlf_attr_intelligence_hint}Attribute learning bonus multiplier for skills governed by Intelligence")]
        [SettingPropertyGroup("{=mlf_group_attr}Attribute Growth Rates")]
        public float IntelligenceMultiplier { get; set; } = 1.0f;

        [SettingPropertyInteger("{=mlf_skillcap_default}Global Default Cap", 10, 1024, "0", Order = 7, RequireRestart = false, HintText = "{=mlf_skillcap_default_hint}Default level cap for all skills (1024 = vanilla hard cap)")]
        [SettingPropertyGroup("{=mlf_group_skillcap}Skill Level Caps")]
        public int SkillCapDefault { get; set; } = 500;

        [SettingPropertyBool("{=mlf_skillcap_enabled}Enable Skill Level Caps (Vanilla Hard Cap = 1024)", Order = 6, RequireRestart = false, HintText = "{=mlf_skillcap_enabled_hint}Real-time toggle: when disabled, the skill cap feature is completely inactive (values are kept but not applied)")]
        [SettingPropertyGroup("{=mlf_group_skillcap}Skill Level Caps")]
        public bool SkillLevelCapEnabled { get; set; } = false;

        [SettingPropertyInteger("{=mlf_skillcap_vigor}Vigor", 10, 1024, "0", Order = 8, RequireRestart = false, HintText = "{=mlf_skillcap_vigor_hint}Level cap for Vigor skills (One Handed / Two Handed / Polearm)")]
        [SettingPropertyGroup("{=mlf_group_skillcap}Skill Level Caps")]
        public int VigorSkillCap { get; set; } = 500;

        [SettingPropertyInteger("{=mlf_skillcap_control}Control", 10, 1024, "0", Order = 9, RequireRestart = false, HintText = "{=mlf_skillcap_control_hint}Level cap for Control skills (Bow / Crossbow / Throwing)")]
        [SettingPropertyGroup("{=mlf_group_skillcap}Skill Level Caps")]
        public int ControlSkillCap { get; set; } = 500;

        [SettingPropertyInteger("{=mlf_skillcap_endurance}Endurance", 10, 1024, "0", Order = 10, RequireRestart = false, HintText = "{=mlf_skillcap_endurance_hint}Level cap for Endurance skills (Riding / Athletics / Smithing)")]
        [SettingPropertyGroup("{=mlf_group_skillcap}Skill Level Caps")]
        public int EnduranceSkillCap { get; set; } = 500;

        [SettingPropertyInteger("{=mlf_skillcap_cunning}Cunning", 10, 1024, "0", Order = 11, RequireRestart = false, HintText = "{=mlf_skillcap_cunning_hint}Level cap for Cunning skills (Scouting / Tactics / Roguery)")]
        [SettingPropertyGroup("{=mlf_group_skillcap}Skill Level Caps")]
        public int CunningSkillCap { get; set; } = 500;

        [SettingPropertyInteger("{=mlf_skillcap_social}Social", 10, 1024, "0", Order = 12, RequireRestart = false, HintText = "{=mlf_skillcap_social_hint}Level cap for Social skills (Charm / Leadership / Trade)")]
        [SettingPropertyGroup("{=mlf_group_skillcap}Skill Level Caps")]
        public int SocialSkillCap { get; set; } = 500;

        [SettingPropertyInteger("{=mlf_skillcap_intelligence}Intelligence", 10, 1024, "0", Order = 13, RequireRestart = false, HintText = "{=mlf_skillcap_intelligence_hint}Level cap for Intelligence skills (Steward / Medicine / Engineering)")]
        [SettingPropertyGroup("{=mlf_group_skillcap}Skill Level Caps")]
        public int IntelligenceSkillCap { get; set; } = 500;

        [SettingPropertyBool("{=mlf_clanparty_recruit}Prevent Clan Party Recruitment", Order = 10, RequireRestart = false, HintText = "{=mlf_clanparty_recruit_hint}Prevent AI lords from recruiting non-main-hero clan parties into armies")]
        [SettingPropertyGroup("{=mlf_group_clanparty}Clan Party Control")]
        public bool PreventClanPartyRecruitment { get; set; } = true;

        [SettingPropertyBool("{=mlf_clanparty_donate}Prevent Clan Party Troop Donation", Order = 11, RequireRestart = false, HintText = "{=mlf_clanparty_donate_hint}Prevent non-main-hero clan parties from donating troops to garrisons")]
        [SettingPropertyGroup("{=mlf_group_clanparty}Clan Party Control")]
        public bool PreventClanPartyDonateTroops { get; set; } = true;

        private bool _forceArmyCreationTest;
        [SettingPropertyBool("{=mlf_prisoner_label}Prisoner Special NPC Labels", Order = 12, RequireRestart = false, HintText = "{=mlf_prisoner_label_hint}In the prisoner tab of the party screen, mark rulers/lords/mercenary leaders with their identity")]
        [SettingPropertyGroup("{=mlf_group_ui}UI")]
        public bool PrisonerSpecialLabel { get; set; } = true;

        [SettingPropertyBool("{=mlf_exile_filter}Encyclopedia Clan Exile Filter", Order = 13, RequireRestart = false, HintText = "{=mlf_exile_filter_hint}In the encyclopedia clan list, add \"In Exile / Not in Exile\" options to the Status filter group (no kingdom, no fiefs, not rebel/bandit/minor faction, excluding player clan)")]
        [SettingPropertyGroup("{=mlf_group_ui}UI")]
        public bool EncyclopediaClanExileFilter { get; set; } = true;

        [SettingPropertyBool("{=mlf_army_test}Force Army Creation Test", Order = 13, RequireRestart = false, HintText = "{=mlf_army_test_hint}Make non-player clan leaders attempt to create armies to test the blocking effect")]
        [SettingPropertyGroup("{=mlf_group_clanparty}Clan Party Control")]
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

        [SettingPropertyBool("{=mlf_restore_enabled}Enable Lord Troop Restoration", Order = 20, RequireRestart = false, HintText = "{=mlf_restore_enabled_hint}Enable/disable the troop restoration feature after releasing lords")]
        [SettingPropertyGroup("{=mlf_group_restore}Lord Troop Restoration")]
        public bool RestorationEnabled { get; set; } = false;

        [SettingPropertyInteger("{=mlf_restore_days}Restoration Days", 1, 30, "0", Order = 21, RequireRestart = false, HintText = "{=mlf_restore_days_hint}Days needed to restore troops after release")]
        [SettingPropertyGroup("{=mlf_group_restore}Lord Troop Restoration")]
        public int RestorationDays { get; set; } = 7;

        [SettingPropertyFloatingInteger("{=mlf_restore_ratio}Restored Troop Ratio", 0.0f, 1.0f, "0.0", Order = 22, RequireRestart = false, HintText = "{=mlf_restore_ratio_hint}Restored troops as a ratio of the party size limit (0 = disabled)")]
        [SettingPropertyGroup("{=mlf_group_restore}Lord Troop Restoration")]
        public float RestorationPartySizeRatio { get; set; } = 0.4f;

        [SettingPropertyFloatingInteger("{=mlf_restore_tier12}Tier 1-2 Troop Ratio", 0.0f, 1.0f, "0.0", Order = 23, RequireRestart = false, HintText = "{=mlf_restore_tier12_hint}Tier 1-2 troop ratio")]
        [SettingPropertyGroup("{=mlf_group_restore}Lord Troop Restoration")]
        public float RestorationTier12Ratio { get; set; } = 0.50f;

        [SettingPropertyFloatingInteger("{=mlf_restore_tier34}Tier 3-4 Troop Ratio", 0.0f, 1.0f, "0.0", Order = 24, RequireRestart = false, HintText = "{=mlf_restore_tier34_hint}Tier 3-4 troop ratio")]
        [SettingPropertyGroup("{=mlf_group_restore}Lord Troop Restoration")]
        public float RestorationTier34Ratio { get; set; } = 0.30f;

        [SettingPropertyFloatingInteger("{=mlf_restore_tier56}Tier 5-6 Troop Ratio", 0.0f, 1.0f, "0.0", Order = 25, RequireRestart = false, HintText = "{=mlf_restore_tier56_hint}Tier 5-6 troop ratio")]
        [SettingPropertyGroup("{=mlf_group_restore}Lord Troop Restoration")]
        public float RestorationTier56Ratio { get; set; } = 0.20f;

        [SettingPropertyInteger("{=mlf_restore_gold}Gold per Troop", 0, 100000, "0", Order = 26, RequireRestart = false, HintText = "{=mlf_restore_gold_hint}Gold given to the lord per troop (0 = no gold)")]
        [SettingPropertyGroup("{=mlf_group_restore}Lord Troop Restoration")]
        public int RestorationGoldPerTroop { get; set; } = 100;

        [SettingPropertyInteger("{=mlf_restore_food}Food per Troop", 0, 100000, "0", Order = 27, RequireRestart = false, HintText = "{=mlf_restore_food_hint}Food given to the lord per troop to prevent starvation (0 = no food)")]
        [SettingPropertyGroup("{=mlf_group_restore}Lord Troop Restoration")]
        public int RestorationFoodPerTroop { get; set; } = 10;

        [SettingPropertyInteger("{=mlf_restore_abandon}Abandon Days", 1, 60, "0", Order = 28, RequireRestart = false, HintText = "{=mlf_restore_abandon_hint}Abandon restoration if the lord still has no party after this many days")]
        [SettingPropertyGroup("{=mlf_group_restore}Lord Troop Restoration")]
        public int RestorationAbandonDays { get; set; } = 14;

        [SettingPropertyBool("{=mlf_territory_enabled}Enable Territory Party Size Bonus", Order = 28, RequireRestart = false, HintText = "{=mlf_territory_enabled_hint}Enable/disable the territory loss compensation feature")]
        [SettingPropertyGroup("{=mlf_group_territory}Territory Party Size Bonus")]
        public bool TerritoryBonusEnabled { get; set; } = false;

        [SettingPropertyInteger("{=mlf_territory_town}Town Compensation Value", 0, 50, "0", Order = 29, RequireRestart = false, HintText = "{=mlf_territory_town_hint}Party size limit increase per lost town (before diminishing)")]
        [SettingPropertyGroup("{=mlf_group_territory}Territory Party Size Bonus")]
        public int TerritoryBonusTownValue { get; set; } = 20;

        [SettingPropertyInteger("{=mlf_territory_castle}Castle Compensation Value", 0, 50, "0", Order = 30, RequireRestart = false, HintText = "{=mlf_territory_castle_hint}Party size limit increase per lost castle (before diminishing)")]
        [SettingPropertyGroup("{=mlf_group_territory}Territory Party Size Bonus")]
        public int TerritoryBonusCastleValue { get; set; } = 10;

        [SettingPropertyFloatingInteger("{=mlf_territory_diminish}Diminishing Multiplier", 0.0f, 1.0f, "0.0", Order = 31, RequireRestart = false, HintText = "{=mlf_territory_diminish_hint}Diminishing multiplier for consecutive territory losses (1.0 = linear)")]
        [SettingPropertyGroup("{=mlf_group_territory}Territory Party Size Bonus")]
        public float TerritoryBonusDiminishRate { get; set; } = 0.7f;

        [SettingPropertyInteger("{=mlf_territory_cap}Maximum Compensation Cap", 0, 500, "0", Order = 32, RequireRestart = false, HintText = "{=mlf_territory_cap_hint}Maximum cumulative compensation a kingdom can accumulate")]
        [SettingPropertyGroup("{=mlf_group_territory}Territory Party Size Bonus")]
        public int TerritoryBonusMaxCap { get; set; } = 200;

        [SettingPropertyInteger("{=mlf_territory_solidify}Conquest Solidify Days", 0, 365, "0", Order = 33, RequireRestart = false, HintText = "{=mlf_territory_solidify_hint}After holding a settlement for this many days, it counts as home territory and no longer offsets compensation (0 = disabled, 84 = one year)")]
        [SettingPropertyGroup("{=mlf_group_territory}Territory Party Size Bonus")]
        public int ConquestSolidifyDays { get; set; } = 42;

        [SettingPropertyInteger("{=mlf_territory_expire}Loss Expire Days", 0, 365, "0", Order = 34, RequireRestart = false, HintText = "{=mlf_territory_expire_hint}After a lost settlement stays lost for this many days, it counts as foreign territory and no longer participates in compensation (0 = disabled, 84 = one year)")]
        [SettingPropertyGroup("{=mlf_group_territory}Territory Party Size Bonus")]
        public int LossExpireDays { get; set; } = 84;

        [SettingPropertyBool("{=mlf_territory_vassals}Vassals Only", Order = 35, RequireRestart = false, HintText = "{=mlf_territory_vassals_hint}Only apply to vassal clans (excluding mercenaries)")]
        [SettingPropertyGroup("{=mlf_group_territory}Territory Party Size Bonus")]
        public bool TerritoryBonusVassalsOnly { get; set; } = true;

        [SettingPropertyInteger("{=mlf_naval_limit}Naval Battle Ship Limit", 3, 8, "0", Order = 36, RequireRestart = false,
            HintText = "{=mlf_naval_limit_hint}Maximum number of ships the player can field simultaneously in naval battles/coastal raids (at least 3, at most 8)")]
        [SettingPropertyGroup("{=mlf_group_naval}Naval Battles")]
        public int NavalBattleShipLimit { get; set; } = 8;

        [SettingPropertyBool("{=mlf_custom_battle_land_first}Main Menu Custom Battle: Land First", Order = 37, RequireRestart = false, HintText = "{=mlf_custom_battle_land_first_hint}Open the land (non-naval) battle configuration first when entering Custom Battle from the main menu. Warsail DLC makes the naval configuration the default entry; enabling this restores land battle first (the switch button still cycles between both modes)")]
        [SettingPropertyGroup("{=mlf_group_naval}Naval Battles")]
        public bool MainMenuCustomBattleLandFirstEnabled { get; set; } = true;

        [SettingPropertyBool("{=mlf_companion_recall}Companion Available Reminder", Order = 37, RequireRestart = false, HintText = "{=mlf_companion_recall_hint}Show a toast notification in the center of the screen when a clan member becomes available after being released/escaping captivity")]
        [SettingPropertyGroup("{=mlf_group_notify}Notifications")]
        public bool CompanionAutoRecallEnabled { get; set; } = true;

        [SettingPropertyBool("{=mlf_debug_restore}Lord Restoration Debug Log", Order = 38, RequireRestart = false, HintText = "{=mlf_debug_restore_hint}Display lord troop restoration debug logs in the bottom-left corner of the screen to troubleshoot restoration logic")]
        [SettingPropertyGroup("{=mlf_group_debug}Debug")]
        public bool EnableRestorationDebugLog { get; set; } = false;

        [SettingPropertyBool("{=mlf_debug_territory}Territory Bonus Debug Log", Order = 39, RequireRestart = false, HintText = "{=mlf_debug_territory_hint}Display territory loss compensation debug logs in the bottom-left corner of the screen to troubleshoot compensation calculations and UI refresh")]
        [SettingPropertyGroup("{=mlf_group_debug}Debug")]
        public bool EnableTerritoryBonusDebugLog { get; set; } = false;

        [SettingPropertyBool("{=mlf_debug_companion}Companion Reminder Debug Log", Order = 40, RequireRestart = false, HintText = "{=mlf_debug_companion_hint}Display companion availability reminder debug logs in the bottom-left corner of the screen")]
        [SettingPropertyGroup("{=mlf_group_debug}Debug")]
        public bool EnableCompanionRecallDebugLog { get; set; } = false;

        [SettingPropertyBool("{=mlf_debug_noammo}Ranged No-Ammo Debug (Press ,)", Order = 41, RequireRestart = false, HintText = "{=mlf_debug_noammo_hint}In battle, press , to force-zero ammunition of 5% of ranged soldiers to test the formation 9 transfer logic")]
        [SettingPropertyGroup("{=mlf_group_debug}Debug")]
        public bool RangedNoAmmoDebugEnabled { get; set; } = false;

        [SettingPropertyBool("{=mlf_ai_war}Prevent AI War Declaration", Order = 42, RequireRestart = false, HintText = "{=mlf_ai_war_hint}When the player is king, prevent AI lords from automatically proposing war declarations")]
        [SettingPropertyGroup("{=mlf_group_diplomacy}Diplomacy")]
        public bool PreventAIWarDeclaration { get; set; } = true;

        [SettingPropertyBool("{=mlf_fief_candidacy}Player Fief Always a Candidate", Order = 43, RequireRestart = false, HintText = "{=mlf_fief_candidacy_hint}Fiefs conquered personally by the player are always included in the fief assignment vote")]
        [SettingPropertyGroup("{=mlf_group_siege}Siege")]
        public bool PlayerFiefCandidacyEnabled { get; set; } = true;

        [SettingPropertyBool("{=mlf_siege_target}Siege Engines Target Engines First", Order = 44, RequireRestart = false, HintText = "{=mlf_siege_target_hint}When attacking, siege engines prioritize enemy ranged siege engines")]
        [SettingPropertyGroup("{=mlf_group_siege}Siege")]
        public bool SiegeTargetSelectionEnabled { get; set; } = true;

        [SettingPropertyBool("{=mlf_siege_leadership}Keep Player Siege Leadership", Order = 45, RequireRestart = false, HintText = "{=mlf_siege_leadership_hint}When the player starts a siege first (alone or leading their own army), the siege command stays with the player — a friendly army joining later cannot take it over. When disabled, restores vanilla behavior (the highest-ranking leader present, e.g. a king or an army leader, takes command)")]
        [SettingPropertyGroup("{=mlf_group_siege}Siege")]
        public bool KeepPlayerSiegeLeadership { get; set; } = true;

        [SettingPropertyBool("{=mlf_crouch}Auto Crouch", Order = 45, RequireRestart = false, HintText = "{=mlf_crouch_hint}Pure infantry/ranged squads auto-crouch when holding still (first rank of line formations / front half of ranged / all ranged in loose formations)")]
        [SettingPropertyGroup("{=mlf_group_battle}Formations & Battle")]
        public bool AutoCrouchEnabled { get; set; } = true;

        [SettingPropertyBool("{=mlf_crouch_shield}Shield Up While Crouching", Order = 46, RequireRestart = false, HintText = "{=mlf_crouch_shield_hint}Front-rank soldiers raise shields upward instead of downward while crouching")]
        [SettingPropertyGroup("{=mlf_group_battle}Formations & Battle")]
        public bool CrouchShieldDirectionEnabled { get; set; } = true;

        [SettingPropertyBool("{=mlf_banner_position}Banner Bearer Position Optimization", Order = 47, RequireRestart = false, HintText = "{=mlf_banner_position_hint}Move banner bearers from the left front to the middle of the last rank")]
        [SettingPropertyGroup("{=mlf_group_battle}Formations & Battle")]
        public bool BannerBearerPositionEnabled { get; set; } = true;

        [SettingPropertyBool("{=mlf_noammo_formation}No-Ammo Ranged Transfer to Formation 9", Order = 48, RequireRestart = false, HintText = "{=mlf_noammo_formation_hint}Ranged soldiers with depleted ammunition are auto-moved to formation 9 and return when ammo is restored")]
        [SettingPropertyGroup("{=mlf_group_battle}Formations & Battle")]
        public bool RangedNoAmmoEnabled { get; set; } = true;

        [SettingPropertyBool("{=mlf_scoreboard}Scoreboard Sort Order Reversal", Order = 49, RequireRestart = false, HintText = "{=mlf_scoreboard_hint}Reverse the sort cycle when clicking scoreboard column headers: default → descending → ascending")]
        [SettingPropertyGroup("{=mlf_group_battle}Formations & Battle")]
        public bool ScoreboardSortOrderEnabled { get; set; } = true;

        [SettingPropertyBool("{=mlf_debug_circle}Debug Circle/Point Render View", Order = 50, RequireRestart = false, HintText = "{=mlf_debug_circle_hint}Show a red circle under the player and a yellow point ahead (debugging)")]
        [SettingPropertyGroup("{=mlf_group_debug}Debug")]
        public bool PlayerCircleViewEnabled { get; set; } = false;

        // ─────────────────────────────────────────────────────────────────────
        // 【已禁用】commit 304cc5d 新增的 ORCA 骑兵避障功能配置（MCM 中不再显示）。
        // 保留原始代码以备后续恢复；如要重新启用，取消下面的 /* */ 注释即可。
        // 注意：启用时必须同时恢复 SubModule.cs 中的 OrcaDebugBehavior 注册，
        //       并取消 OrcaSystem/*.cs 三个文件的 #if false 包裹。
        // ─────────────────────────────────────────────────────────────────────
        /*
        [SettingPropertyBool("ORCA避让调试视图", Order = 51, RequireRestart = false, HintText = "实时绘制玩家方骑兵的 ORCA 避让帧偏移（绿=无冲突/黄=轻调/红=强制绕行）+ 感知半径圈。仅可视化，不干预实际移动")]
        [SettingPropertyGroup("ORCA避让调试")]
        public bool OrcaDebugEnabled { get; set; } = false;

        [SettingPropertyFloatingInteger("感知半径(米)", 1.0f, 20.0f, "0.0", Order = 52, RequireRestart = false, HintText = "ORCA 邻居搜索半径：仅与该半径内的其他己方骑兵两两避让（真实参与求解，超出不建约束线，青色圈=实际范围）")]
        [SettingPropertyGroup("ORCA避让调试")]
        public float OrcaSenseRadius { get; set; } = 3.0f;

        [SettingPropertyInteger("参与数量上限", 10, 500, "0", Order = 52, RequireRestart = false, HintText = "参与 ORCA 求解的己方骑乘单位数量上限（超出按距玩家最近优先）。越大覆盖越全但每帧越慢（O(n²) 受感知半径截断缓解）")]
        [SettingPropertyGroup("ORCA避让调试")]
        public int OrcaMaxAgents { get; set; } = 80;

        [SettingPropertyFloatingInteger("参与半径(米)", 20.0f, 200.0f, "0", Order = 52, RequireRestart = false, HintText = "距玩家此半径内的己方骑乘单位才参与求解（第一道过滤，先于数量上限）")]
        [SettingPropertyGroup("ORCA避让调试")]
        public float OrcaMaxRadius { get; set; } = 60f;

        [SettingPropertyFloatingInteger("时间窗(秒)", 0.1f, 5.0f, "0.0", Order = 53, RequireRestart = false, HintText = "ORCA 时间窗：在此时间内保证不与邻居碰撞，越大越保守")]
        [SettingPropertyGroup("ORCA避让调试")]
        public float OrcaTimeHorizon { get; set; } = 1.5f;

        [SettingPropertyFloatingInteger("碰撞体半长轴(米)", 0.1f, 3.0f, "0.00", Order = 54, RequireRestart = false, HintText = "ORCA 使用的骑兵碰撞体半长轴（沿马身朝向，马全长≈2.4m 故半长约1.2m），不读引擎真实碰撞体。与半短轴组成有向椭圆：头对头时用长轴避让、肩并肩时用短轴")]
        [SettingPropertyGroup("ORCA避让调试")]
        public float OrcaHalfLength { get; set; } = 1.2f;

        [SettingPropertyFloatingInteger("碰撞体半短轴(米)", 0.1f, 2.0f, "0.00", Order = 54, RequireRestart = false, HintText = "ORCA 使用的骑兵碰撞体半短轴（垂直马身朝向，马宽≈0.9m 故半宽约0.45m），不读引擎真实碰撞体。设为与半长轴相等即退化为纯圆模型")]
        [SettingPropertyGroup("ORCA避让调试")]
        public float OrcaHalfWidth { get; set; } = 0.45f;

        [SettingPropertyBool("绘制感知半径圈", Order = 55, RequireRestart = false, HintText = "在每个己方骑兵脚下绘制青色感知半径圈（显示 ORCA 的邻居搜索范围）。仅控制感知半径圈；碰撞椭圆轮廓（深蓝）由「ORCA避让调试视图」总开关控制")]
        [SettingPropertyGroup("ORCA避让调试")]
        public bool OrcaShowSenseCircles { get; set; } = true;

        [SettingPropertyBool("应用ORCA到实际移动", Order = 56, RequireRestart = false, HintText = "将 ORCA 建议速度翻译成 native 输入：冲突时降低速度上限（含坐骑）并偏移目标帧绕行。仅作用于玩家方骑兵")]
        [SettingPropertyGroup("ORCA避让调试")]
        public bool OrcaApplyToNative { get; set; } = false;

        [SettingPropertyFloatingInteger("限速触发阈值", 0.0f, 1.0f, "0.00", Order = 57, RequireRestart = false, HintText = "冲突程度超过此值才开始降低速度上限（乘数=ORCA建议速度/全速，下限见'限速下限'）。越小越早减速，0.35≈绘制黄点阈值")]
        [SettingPropertyGroup("ORCA避让调试")]
        public float OrcaApplySpeedThreshold { get; set; } = 0.35f;

        [SettingPropertyFloatingInteger("目标帧偏移阈值", 0.0f, 1.0f, "0.00", Order = 58, RequireRestart = false, HintText = "冲突程度超过此值才把目标帧偏移到 ORCA 建议方向绕行。0.6 略早于绘制红点阈值(0.7)，让强冲突先减速再绕行")]
        [SettingPropertyGroup("ORCA避让调试")]
        public float OrcaApplyFrameThreshold { get; set; } = 0.6f;

        [SettingPropertyFloatingInteger("限速下限(乘数)", 0.05f, 1.0f, "0.00", Order = 59, RequireRestart = false, HintText = "ORCA 限速乘数的最低值。0.35=最多降到全速的35%，防止完全定死；调大则减速更温和")]
        [SettingPropertyGroup("ORCA避让调试")]
        public float OrcaApplyMinSpeedMultiplier { get; set; } = 0.35f;

        [SettingPropertyFloatingInteger("目标帧前瞻(秒)", 0.1f, 2.0f, "0.0", Order = 60, RequireRestart = false, HintText = "目标帧偏移的距离=NewVelocity×此秒数。越大绕行越早越激进，越小越贴着当前位置微调")]
        [SettingPropertyGroup("ORCA避让调试")]
        public float OrcaApplyOffsetTime { get; set; } = 0.4f;
        */

        [SettingPropertyBool("{=mlf_lance_couch}Couch Lance Knockdown", Order = 51, RequireRestart = false, HintText = "{=mlf_lance_couch_hint}Mounted couch lance (passive attack) hits on unmounted infantry/ranged units always knock them down (symmetric for both sides; negated by blocking)")]
        [SettingPropertyGroup("{=mlf_group_lance}Mounted Polearm Knockdown")]
        public bool CouchLanceKnockDownEnabled { get; set; } = true;

        [SettingPropertyBool("{=mlf_lance_thrust}Mounted Polearm Thrust Knockdown", Order = 52, RequireRestart = false, HintText = "{=mlf_lance_thrust_hint}Normal mounted polearm thrusts hitting unmounted infantry/ranged units always knock them down (symmetric for both sides; negated by blocking)")]
        [SettingPropertyGroup("{=mlf_group_lance}Mounted Polearm Knockdown")]
        public bool MountedPolearmThrustKnockDownEnabled { get; set; } = true;

        [SettingPropertyFloatingInteger("{=mlf_lance_min_speed}Thrust Minimum Relative Speed", 0.0f, 10.0f, "#0.0", Order = 53, RequireRestart = false, HintText = "{=mlf_lance_min_speed_hint}Minimum relative speed required to trigger guaranteed knockdown on a mounted polearm thrust (length of the velocity difference vector between attacker and target, same unit as game speed). Default 2.0 prevents stationary thrusts from knocking down; 0 removes the speed requirement")]
        [SettingPropertyGroup("{=mlf_group_lance}Mounted Polearm Knockdown")]
        public float MountedPolearmThrustMinRelativeSpeed { get; set; } = 2.0f;

        [SettingPropertyFloatingInteger("{=mlf_lance_damage}Thrust Knockdown Damage Bonus", 0.0f, 2.0f, "0%", Order = 54, RequireRestart = false, HintText = "{=mlf_lance_damage_hint}Damage bonus applied when a mounted polearm thrust triggers guaranteed knockdown (default 0.3 = +30%). 0 means no damage bonus")]
        [SettingPropertyGroup("{=mlf_group_lance}Mounted Polearm Knockdown")]
        public float MountedPolearmThrustKnockDownDamageBonus { get; set; } = 0.3f;

        [SettingPropertyBool("{=mlf_spawn_enabled}Enable Custom Spawn Ratios", Order = 55, RequireRestart = false, HintText = "{=mlf_spawn_enabled_hint}Active when game setting \"Unit Spawn Priority = High Tier First\": schedules spawn pacing by the four ratio weights below (infantry/archer/cavalry/horse archer), while within a troop type units still spawn highest tier first, preventing high-tier troops from monopolizing all spawn slots. When disabled, reverts to vanilla high-tier-first logic")]
        [SettingPropertyGroup("{=mlf_group_spawn}Spawn Ratios")]
        public bool UnitSpawnRatioEnabled { get; set; } = true;

        [SettingPropertyInteger("{=mlf_spawn_infantry}Infantry Ratio", 0, 100, "0", Order = 56, RequireRestart = false, HintText = "{=mlf_spawn_infantry_hint}Infantry spawn quota weight (relative value; higher = more frequent; 0 = infantry never spawns; total of all four is recommended to be 100)")]
        [SettingPropertyGroup("{=mlf_group_spawn}Spawn Ratios")]
        public int InfantryRatio { get; set; } = 15;

        [SettingPropertyInteger("{=mlf_spawn_archer}Archer Ratio", 0, 100, "0", Order = 57, RequireRestart = false, HintText = "{=mlf_spawn_archer_hint}Archer spawn quota weight (relative value; higher = more frequent; 0 = archers never spawn; total of all four is recommended to be 100)")]
        [SettingPropertyGroup("{=mlf_group_spawn}Spawn Ratios")]
        public int ArcherRatio { get; set; } = 65;

        [SettingPropertyInteger("{=mlf_spawn_cavalry}Cavalry Ratio", 0, 100, "0", Order = 58, RequireRestart = false, HintText = "{=mlf_spawn_cavalry_hint}Cavalry spawn quota weight (relative value; higher = more frequent; 0 = cavalry never spawns; total of all four is recommended to be 100)")]
        [SettingPropertyGroup("{=mlf_group_spawn}Spawn Ratios")]
        public int CavalryRatio { get; set; } = 15;

        [SettingPropertyInteger("{=mlf_spawn_horsearcher}Horse Archer Ratio", 0, 100, "0", Order = 59, RequireRestart = false, HintText = "{=mlf_spawn_horsearcher_hint}Horse archer spawn quota weight (relative value; higher = more frequent; 0 = horse archers never spawn; total of all four is recommended to be 100)")]
        [SettingPropertyGroup("{=mlf_group_spawn}Spawn Ratios")]
        public int HorseArcherRatio { get; set; } = 5;

        [SettingPropertyBool("{=mlf_npc_party_enabled}Enable NPC Clan Party Limit Bonus", Order = 60, RequireRestart = false, HintText = "{=mlf_npc_party_enabled_hint}Add extra parties on top of the vanilla party limit for all AI lord clans (only affects daily AI dispatch, not the player clan)")]
        [SettingPropertyGroup("{=mlf_group_npc}NPC Lord Clan Adjustments")]
        public bool NpcClanPartyLimitBonusEnabled { get; set; } = false;

        [SettingPropertyInteger("{=mlf_npc_party_bonus}NPC Clan Party Limit Bonus", 0, 10, "0", Order = 61, RequireRestart = false, HintText = "{=mlf_npc_party_bonus_hint}Extra parties added on top of the vanilla party limit for all NPC lord clans (default +2: Tier 0-2 clans from 1 to 3 parties, Tier 3-4 from 2 to 4, Tier 5-6 from 3 to 5; 0 disables the bonus)")]
        [SettingPropertyGroup("{=mlf_group_npc}NPC Lord Clan Adjustments")]
        public int NpcClanPartyLimitBonus { get; set; } = 1;

        [SettingPropertyBool("{=mlf_recruit_rate_enabled}Enable Recruitment Refill Rate", Order = 62, RequireRestart = false, HintText = "{=mlf_recruit_rate_enabled_hint}Real-time toggle: when disabled, the recruitment refill rate multiplier has no effect (notables keep vanilla daily refill)")]
        [SettingPropertyGroup("{=mlf_group_recruit}Recruitment")]
        public bool VolunteerRecruitRateEnabled { get; set; } = false;

        [SettingPropertyFloatingInteger("{=mlf_recruit_rate_multiplier}Daily Refill Rate Multiplier", 0.5f, 5.0f, "#0.0x", Order = 63, RequireRestart = false, HintText = "{=mlf_recruit_rate_multiplier_hint}Multiplier for the daily chance of town/village notables refilling recruits (1.0 = vanilla, 2.0 = expected doubling; deeper slots have lower vanilla rates and are scaled proportionally; above 1.9 the first slots will always refill, but upgrades still need separate acceleration)")]
        [SettingPropertyGroup("{=mlf_group_recruit}Recruitment")]
        public float VolunteerRecruitRateMultiplier { get; set; } = 2.0f;

        [SettingPropertyBool("{=mlf_recruit_upgrade_enabled}Enable Upgrade Acceleration", Order = 64, RequireRestart = false, HintText = "{=mlf_recruit_upgrade_enabled_hint}Real-time toggle: when disabled, the volunteer upgrade rate multiplier has no effect (keeps vanilla extremely low upgrade chance)")]
        [SettingPropertyGroup("{=mlf_group_recruit}Recruitment")]
        public bool VolunteerUpgradeRateEnabled { get; set; } = false;

        [SettingPropertyFloatingInteger("{=mlf_recruit_upgrade_multiplier}Recruit Upgrade Rate Multiplier", 1.0f, 100.0f, "#0.0x", Order = 65, RequireRestart = false, HintText = "{=mlf_recruit_upgrade_multiplier_hint}Multiplier for the daily upgrade chance of troops owned by town/village notables (1.0 = vanilla; vanilla chance = log2(influence/level)*0.01, e.g. a level-2 notable with 30 influence has ~5% daily upgrade chance; a 10x multiplier gives ~50%)")]
        [SettingPropertyGroup("{=mlf_group_recruit}Recruitment")]
        public float VolunteerUpgradeRateMultiplier { get; set; } = 2.0f;

        [SettingPropertyBool("{=mlf_retreat_free}Free Retreat When Joining Battles", Order = 66, RequireRestart = false, HintText = "{=mlf_retreat_free_hint}After joining an existing friendly battle on the campaign map (whether friendlies attack or defend), the encounter menu always offers a \"Leave\" option to withdraw with your party at any time; battles you started yourself (defending/attacking sieges) keep vanilla rules")]
        [SettingPropertyGroup("{=mlf_group_join}Joining Battles")]
        public bool FreeBattleRetreatEnabled { get; set; } = true;

        [SettingPropertyBool("{=mlf_save_dated}Name Saves with Date & Time", Order = 67, RequireRestart = false, HintText = "{=mlf_save_dated_hint}Quick save and auto save use \"date and time of saving\" as names (save_qu_/save_au_ prefixes), each campaign rotates its own pool; when full, the oldest save of that campaign is pruned after a new save succeeds. Save As and ironman keep vanilla logic. When disabled, fully reverts to vanilla naming (saveNNN / saveauto1-3); already-created dated saves are not auto-deleted")]
        [SettingPropertyGroup("{=mlf_group_save}Save Settings (test first before trusting!)")]
        public bool DatedSaveNamingEnabled { get; set; } = false;

        [SettingPropertyInteger("{=mlf_save_pool}Rotation Pool Size", 1, 50, "0", Order = 68, RequireRestart = false, HintText = "{=mlf_save_pool_hint}Maximum files per campaign in the rotation pool for dated saves (auto + quick): when the pool of a campaign is full, the next quick/auto save first saves the new file successfully, then prunes the oldest by time; saves of different campaigns never prune each other (1-50, default 10)")]
        [SettingPropertyGroup("{=mlf_group_save}Save Settings (test first before trusting!)")]
        public int DatedSavePoolSize { get; set; } = 10;

        [SettingPropertyBool("{=mlf_debug_save}Dated Save Debug Log", Order = 69, RequireRestart = false, HintText = "{=mlf_debug_save_hint}Display dated-save logs in the bottom-left corner of the screen (new save names, save results, pruned old saves) to troubleshoot naming and rotation logic")]
        [SettingPropertyGroup("{=mlf_group_debug}Debug")]
        public bool DatedSaveNamingDebugLogEnabled { get; set; } = false;

        [SettingPropertyBool("{=mlf_food_enabled}Enable Food Transport Support", Order = 70, RequireRestart = false, HintText = "{=mlf_food_enabled_hint}Every 6 in-game hours, surplus player-clan towns dispatch transport parties converted from garrisons to starving player-clan towns/castles, adding/subtracting town food values directly (bypassing market consumption)")]
        [SettingPropertyGroup("{=mlf_group_food}Food Transport Support")]
        public bool TransportSupportEnabled { get; set; } = true;

        [SettingPropertyInteger("{=mlf_food_target_threshold}Starving Food Threshold", 0, 300, "0", Order = 71, RequireRestart = false, HintText = "{=mlf_food_target_threshold_hint}Player-clan towns/castles with food below this value are listed as starving and await support from other towns (town cap 300, castle cap 450)")]
        [SettingPropertyGroup("{=mlf_group_food}Food Transport Support")]
        public int TargetFoodThreshold { get; set; } = 60;

        [SettingPropertyInteger("{=mlf_food_source_threshold}Surplus Food Threshold", 0, 450, "0", Order = 72, RequireRestart = false, HintText = "{=mlf_food_source_threshold_hint}Player-clan towns only dispatch transport parties when their food is above this value (dispatched food never takes the town below this value)")]
        [SettingPropertyGroup("{=mlf_group_food}Food Transport Support")]
        public int SourceFoodThreshold { get; set; } = 200;

        [SettingPropertyInteger("{=mlf_food_garrison_threshold}Surplus Garrison Threshold", 0, 2000, "0", Order = 73, RequireRestart = false, HintText = "{=mlf_food_garrison_threshold_hint}Player-clan towns only dispatch transport parties when garrison size is above this value (vanilla minimum garrison: town 125 / castle 75)")]
        [SettingPropertyGroup("{=mlf_group_food}Food Transport Support")]
        public int SourceGarrisonThreshold { get; set; } = 250;

        [SettingPropertyInteger("{=mlf_food_party_size}Transport Party Size", 10, 100, "0", Order = 74, RequireRestart = false, HintText = "{=mlf_food_party_size_hint}Number of soldiers converted from garrison per transport party (half high-tier/half low-tier randomly picked; battle losses are permanent, survivors return to garrison on return)")]
        [SettingPropertyGroup("{=mlf_group_food}Food Transport Support")]
        public int TransportPartySize { get; set; } = 30;

        [SettingPropertyInteger("{=mlf_food_per_troop}Abstract Food per Troop", 0, 100, "0", Order = 75, RequireRestart = false, HintText = "{=mlf_food_per_troop_hint}Abstract support food carried per soldier: total support food = transport party size × this value, deducted from source town food on departure and added to target town food on arrival (not via market)")]
        [SettingPropertyGroup("{=mlf_group_food}Food Transport Support")]
        public int FoodPerTroop { get; set; } = 2;

        [SettingPropertyInteger("{=mlf_food_physical}Physical Food per Troop", 0, 100, "0", Order = 76, RequireRestart = false, HintText = "{=mlf_food_physical_hint}Physical grain carried per soldier (into party inventory, only for the transport party's own consumption en route, not interchangeable with support food; leftovers destroyed on delivery/return)")]
        [SettingPropertyGroup("{=mlf_group_food}Food Transport Support")]
        public int PhysicalFoodPerTroop { get; set; } = 5;

        [SettingPropertyInteger("{=mlf_food_max_target}Max Support Parties per Target", 1, 10, "0", Order = 77, RequireRestart = false, HintText = "{=mlf_food_max_target_hint}Maximum number of transport parties the same starving town/castle can accept support from at the same time")]
        [SettingPropertyGroup("{=mlf_group_food}Food Transport Support")]
        public int MaxSupportingTownsPerTarget { get; set; } = 3;

        [SettingPropertyInteger("{=mlf_food_max_source}Max Outgoing Parties per Town", 1, 10, "0", Order = 78, RequireRestart = false, HintText = "{=mlf_food_max_source_hint}Maximum number of transport parties the same surplus town can dispatch at the same time")]
        [SettingPropertyGroup("{=mlf_group_food}Food Transport Support")]
        public int MaxOutgoingTransportsPerTown { get; set; } = 2;

        [SettingPropertyBool("{=mlf_debug_food}Food Transport Debug Log", Order = 79, RequireRestart = false, HintText = "{=mlf_debug_food_hint}Display food transport dispatch/delivery/exception logs in the bottom-left corner of the screen (dispatch, delivery, refund, destroyed, etc.)")]
        [SettingPropertyGroup("{=mlf_group_food}Food Transport Support")]
        public bool EnableSupportDebugLog { get; set; } = false;

        [SettingPropertyBool("{=mlf_food_map_visible}Food Transport Visible on Map", Order = 80, RequireRestart = false, HintText = "{=mlf_food_map_visible_hint}Show food transport parties on the campaign map globally, like your clan's armies, regardless of distance or fog of war")]
        [SettingPropertyGroup("{=mlf_group_food}Food Transport Support")]
        public bool TransportMapVisibilityEnabled { get; set; } = true;

        [SettingPropertyBool("{=mlf_exile_survival}Exiled Clans Never Die", Order = 80, RequireRestart = false, HintText = "{=mlf_exile_survival_hint}Remove the 28-day survival countdown extinction mechanic for landless exiled clans (clans wandering after their kingdom falls), letting them persist forever until joining another kingdom or gaining land. When disabled, restores vanilla countdown extinction")]
        [SettingPropertyGroup("{=mlf_group_exile}Exiled Clans")]
        public bool WanderingClanSurvivalEnabled { get; set; } = false;

        [SettingPropertyBool("{=mlf_death_noai}No AI Takeover on Player Death", Order = 81, RequireRestart = false, HintText = "{=mlf_death_noai_hint}After the player character dies, prevent the system from forcing full AI command of the player's party; the party keeps executing the last orders given before the player's death (the vanilla order UI is still closed after death, so no further manual orders can be issued). When disabled, restores vanilla: full AI takeover on death")]
        [SettingPropertyGroup("{=mlf_group_battle}Formations & Battle")]
        public bool PlayerDeathNoAITakeoverEnabled { get; set; } = false;

        [SettingPropertyBool("{=mlf_frontrank_sort}Front Rank Shield Sort Fix", Order = 82, RequireRestart = false, HintText = "{=mlf_frontrank_sort_hint}Fix the vanilla formation arrangement convergence defect: shielded (or polearm-bracing) units keep bubbling forward until the column stabilizes, and cross-column gap filling skips full ranks instead of aborting the whole arrangement, ensuring the front rank is fully shielded whenever shield units are available (vanilla can leave a disordered mix of non-shield front rank + shield second rank). Only applies to infantry formations (infantry share above 95%); ranged formations keep this front-rank bubbling disabled so the Shield Bearer Formation repositioning can take over")]
        [SettingPropertyGroup("{=mlf_group_battle}Formations & Battle")]
        public bool FormationFrontRankSortEnabled { get; set; } = true;

        [SettingPropertyBool("{=mlf_spear_switch_enabled}Spear Melee Switch", Order = 83, RequireRestart = false, HintText = "{=mlf_spear_switch_enabled_hint}AI foot soldiers carrying both a polearm and a one-handed melee weapon automatically switch to the one-handed weapon when enemies get within melee range (preventing the spear thrust from whiffing at point-blank range), and switch back to the polearm once enemies pull away. Symmetric for both sides; the player character is never affected. When disabled, soldiers keep vanilla weapon behavior")]
        [SettingPropertyGroup("{=mlf_group_battle}Formations & Battle")]
        public bool SpearMeleeSwitchEnabled { get; set; } = true;

        [SettingPropertyFloatingInteger("{=mlf_spear_switch_dist}Switch to Melee Distance", 1.0f, 4.0f, "0.0", Order = 84, RequireRestart = false, HintText = "{=mlf_spear_switch_dist_hint}Distance to the nearest enemy (meters) at which a polearm-wielding soldier switches to the one-handed weapon. Smaller = the spear is kept longer, larger = earlier switch. Must not exceed the switch-back distance")]
        [SettingPropertyGroup("{=mlf_group_battle}Formations & Battle")]
        public float SpearMeleeSwitchDistance { get; set; } = 2.0f;

        [SettingPropertyFloatingInteger("{=mlf_spear_switch_back}Switch Back to Polearm Distance", 2.0f, 8.0f, "0.0", Order = 85, RequireRestart = false, HintText = "{=mlf_spear_switch_back_hint}Distance from the nearest enemy (meters) at which a one-handed-wielding soldier switches back to the polearm (hysteresis band with the switch distance prevents weapon flickering). Smaller = quicker return to the spear, larger = the sword is kept longer")]
        [SettingPropertyGroup("{=mlf_group_battle}Formations & Battle")]
        public float SpearMeleeSwitchBackDistance { get; set; } = 4.0f;

        [SettingPropertyBool("{=mlf_village_rebuild}Village Funded Rebuild", Order = 83, RequireRestart = false, HintText = "{=mlf_village_rebuild_hint}The menu of fully raided (devastated) villages offers a \"Fund Reconstruction\" option: pay 10000 denars and the village rebuilds automatically in 3 days (returns to normal operation and rewards all notables with 15~20 relation). When disabled, the menu option is hidden; ongoing paid rebuilds are unaffected and still complete on time")]
        [SettingPropertyGroup("{=mlf_group_village}Village Rebuild")]
        public bool VillageRebuildEnabled { get; set; } = true;

        [SettingPropertyBool("{=mlf_prisoner_remove_relation}Prisoner Removal Grants Relation", Order = 84, RequireRestart = false, HintText = "{=mlf_prisoner_remove_relation_hint}Removing a hero prisoner directly in the party screen (dragging them out of the party roster) now grants the same +4 relation as releasing them through dialogue, following the vanilla relation flow")]
        [SettingPropertyGroup("{=mlf_group_prisoner}Prisoners")]
        public bool PrisonerRemoveRelationEnabled { get; set; } = true;

        // ── 战场击杀信息流（右上角全军击杀/阵亡提示） ─────────────────

        [SettingPropertyBool("{=mlf_killfeed_limit_enabled}Limit Kill Feed Items", Order = 85, RequireRestart = false, HintText = "{=mlf_killfeed_limit_enabled_hint}Cap the number of simultaneously visible kill feed entries in battle (top-right corner); when exceeded, the oldest entry is removed immediately so a mass casualty pileup never stretches the feed off-screen")]
        [SettingPropertyGroup("{=mlf_group_killfeed}Kill Feed")]
        public bool KillFeedItemLimitEnabled { get; set; } = false;

        [SettingPropertyInteger("{=mlf_killfeed_max_items}Max Kill Feed Items", 4, 20, "0", Order = 86, RequireRestart = false, HintText = "{=mlf_killfeed_max_items_hint}Maximum number of kill feed entries displayed at the same time (entries beyond this are removed oldest-first; default 6 keeps the top-right feed compact)")]
        [SettingPropertyGroup("{=mlf_group_killfeed}Kill Feed")]
        public int KillFeedMaxItems { get; set; } = 10;

        [SettingPropertyBool("{=mlf_killfeed_shrink_enabled}Shrink Old Kill Feed Text", Order = 87, RequireRestart = false, HintText = "{=mlf_killfeed_shrink_enabled_hint}Gradually shrink the text of older kill feed entries once the on-screen entry count exceeds the threshold below (newest stays full size, oldest shrinks down to the minimum scale), creating a clear visual hierarchy")]
        [SettingPropertyGroup("{=mlf_group_killfeed}Kill Feed")]
        public bool KillFeedShrinkEnabled { get; set; } = false;

        [SettingPropertyInteger("{=mlf_killfeed_shrink_threshold}Shrink Threshold", 2, 10, "0", Order = 88, RequireRestart = false, HintText = "{=mlf_killfeed_shrink_threshold_hint}Entries beyond this on-screen count start shrinking (e.g. 4 means the 5th+ entry shrinks; oldest shrinks most)")]
        [SettingPropertyGroup("{=mlf_group_killfeed}Kill Feed")]
        public int KillFeedShrinkThreshold { get; set; } = 4;

        [SettingPropertyFloatingInteger("{=mlf_killfeed_shrink_scale}Minimum Text Scale", 0.2f, 1.0f, "0%", Order = 89, RequireRestart = false, HintText = "{=mlf_killfeed_shrink_scale_hint}Smallest font scale applied to the oldest kill feed entry (0.7 = 70% of the normal font size; newer entries interpolate between this and 100%)")]
        [SettingPropertyGroup("{=mlf_group_killfeed}Kill Feed")]
        public float KillFeedShrinkMinScale { get; set; } = 0.5f;

        // ── 坐镇指挥模拟重平衡（Auto Resolve Rebalance） ──────────────
        // 移植自 AutoResolveRebalanced：累计 HP 伤亡模型 + 纯武器伤害（4×4 武器优先表）+ 兵力悬殊追加回合。
        // 总开关关闭时全部 6 个补丁均放行原版逻辑；子开关实时生效。

        [SettingPropertyBool("{=mlf_autoresolve_enabled}Enable Auto Resolve Rebalance", Order = 90, RequireRestart = false, HintText = "{=mlf_autoresolve_enabled_hint}Overhaul the auto-resolve (simulate battle) calculation: troops accumulate damage with individual HP instead of dying instantly, hit damage comes from the striker's actual weapon (4x4 class-vs-class priority table), armor reduces damage with the vanilla formula, shield-bearers can block, and lopsided battles get extra rounds to finish. When disabled, all auto-resolve logic reverts to vanilla")]
        [SettingPropertyGroup("{=mlf_group_autoresolve}Auto Resolve Rebalance")]
        public bool AutoResolveEnabled { get; set; } = true;

        [SettingPropertyBool("{=mlf_autoresolve_ai}Apply to AI vs AI Battles", Order = 91, RequireRestart = false, HintText = "{=mlf_autoresolve_ai_hint}Apply the rebalanced logic to battles between AI parties on the campaign map. When disabled, only player-simulated battles (auto-resolve) are affected")]
        [SettingPropertyGroup("{=mlf_group_autoresolve}Auto Resolve Rebalance")]
        public bool AutoResolveAiEnabled { get; set; } = true;

        [SettingPropertyFloatingInteger("{=mlf_autoresolve_ai_speed}AI vs AI Battle Speed", 1.0f, 10.0f, "0.0x", Order = 92, RequireRestart = false, HintText = "{=mlf_autoresolve_ai_speed_hint}Shorten the simulation interval between battle rounds for AI vs AI battles on the campaign map (default 1.0 = battles between AI parties resolve at normal speed). Hit damage, armor and weapon formulas are unchanged; player-simulated auto-resolve is never affected")]
        [SettingPropertyGroup("{=mlf_group_autoresolve}Auto Resolve Rebalance")]
        public float AutoResolveAiSimulationSpeed { get; set; } = 1.0f;

        [SettingPropertyBool("{=mlf_autoresolve_armor}Armor Reduces Damage", Order = 93, RequireRestart = false, HintText = "{=mlf_autoresolve_armor_hint}Apply the vanilla damage-reduction formula to the struck troop's armor (random hit location: head / arm / leg / torso): raw damage is reduced by 50/(50+armor) then further reduced by the armor value scaled by damage type (cut -50%, pierce -33%, blunt -20%), while a blunt-factor portion (blunt 60% / cut 10%) ignores armor entirely. When disabled, armor is ignored in auto-resolve")]
        [SettingPropertyGroup("{=mlf_group_autoresolve}Auto Resolve Rebalance")]
        public bool AutoResolveArmorEnabled { get; set; } = true;

        [SettingPropertyFloatingInteger("{=mlf_autoresolve_shield_block}Shield Block Chance", 0.0f, 1.0f, "0%", Order = 94, RequireRestart = false, HintText = "{=mlf_autoresolve_shield_block_hint}Chance that an attack against a shield-bearing infantry/cavalry troop is blocked by the shield, dealing no damage (default 0.1 = 10%). Ranged troops (archers/horse archers) never get shield blocks")]
        [SettingPropertyGroup("{=mlf_group_autoresolve}Auto Resolve Rebalance")]
        public float AutoResolveShieldBlockChance { get; set; } = 0.1f;

        [SettingPropertyFloatingInteger("{=mlf_autoresolve_javelin}Javelin Use Chance", 0.0f, 1.0f, "0%", Order = 95, RequireRestart = false, HintText = "{=mlf_autoresolve_javelin_hint}Chance that an infantry/cavalry troop carrying a javelin throws it instead of attacking in melee (default 0.05 = 5%). A troop carrying only javelins always throws (100%) and never fights unarmed. Ranged troops never use javelins")]
        [SettingPropertyGroup("{=mlf_group_autoresolve}Auto Resolve Rebalance")]
        public float AutoResolveJavelinChance { get; set; } = 0.05f;

        [SettingPropertyFloatingInteger("{=mlf_autoresolve_ranged_hit}Ranged Hit Chance", 0.0f, 1.0f, "0%", Order = 96, RequireRestart = false, HintText = "{=mlf_autoresolve_ranged_hit_hint}Hit chance for ranged attacks (bows/crossbows/slings and thrown javelins) in auto-resolve (default 0.8 = 80%). A miss deals no damage this hit")]
        [SettingPropertyGroup("{=mlf_group_autoresolve}Auto Resolve Rebalance")]
        public float AutoResolveRangedHitChance { get; set; } = 0.8f;

        [SettingPropertyFloatingInteger("{=mlf_autoresolve_attack_cap}Max Force Ratio (Attack Frequency Cap)", 1.0f, 10.0f, "0.0x", Order = 97, RequireRestart = false, HintText = "{=mlf_autoresolve_attack_cap_hint}Cap the attack-frequency ratio between the two sides in auto-resolve. Attack frequency scales with (force ratio)^0.6; with the default cap of 2.0 the frequency ratio never exceeds 2^0.6 ≈ 1.52, so a bigger army attacks at most ~1.5x more often no matter how lopsided the battle gets. Set to 1 to remove the cap")]
        [SettingPropertyGroup("{=mlf_group_autoresolve}Auto Resolve Rebalance")]
        public float AutoResolveAttackRatioCap { get; set; } = 2.0f;

        [SettingPropertyBool("{=mlf_autoresolve_battlelog}Auto Resolve Battle Log (CSV)", Order = 98, RequireRestart = false, HintText = "{=mlf_autoresolve_battlelog_hint}Write a full CSV log of every player-simulated auto-resolve battle for later analysis (battle summary, per-round stats, per-hit weapon/armor/damage details, and every casualty event). Files are saved to '<game root>\\MutliLittleFixes_AutoResolveLogs\\<timestamp>\\' with UTF-8 BOM encoding and can be opened directly in Excel/WPS. Only battles simulated by the player are logged; AI vs AI battles are never recorded")]
        [SettingPropertyGroup("{=mlf_group_autoresolve}Auto Resolve Rebalance")]
        public bool EnableAutoResolveBattleLog { get; set; } = false;

        [SettingPropertyBool("{=mlf_debug_autoresolve}Auto Resolve Debug Log", Order = 99, RequireRestart = false, HintText = "{=mlf_debug_autoresolve_hint}Display auto-resolve rebalance debug logs (per-hit HP changes, data rebuilds, errors) in the bottom-left corner of the screen")]
        [SettingPropertyGroup("{=mlf_group_debug}Debug")]
        public bool EnableAutoResolveDebugLog { get; set; } = false;

        [SettingPropertyBool("{=mlf_debug_shield_auto}Shield Auto Plant Debug Log", Order = 99, RequireRestart = false, HintText = "{=mlf_debug_shield_auto_hint}Display shield auto plant/pick-up debug logs (scan summary, formation order changes, auto plant/pick-up actions, skipped reasons) in the bottom-left corner of the screen")]
        [SettingPropertyGroup("{=mlf_group_debug}Debug")]
        public bool ShieldPlantingDebugLog { get; set; } = false;

        [SettingPropertyBool("{=mlf_shield_enabled}Plant Shields", Order = 0, RequireRestart = false, HintText = "{=mlf_shield_enabled_hint}Any ranged foot soldier carrying a shield (horse archers excluded) can plant their shield into the ground as an obstacle. In battle, press F11 to plant the shields of the selected formation (or all eligible soldiers if no formation is selected), and press J to pick them up. Planted troops keep fighting with their ranged weapons while the planted shield acts as cover. When disabled, all planted shields are picked up and the feature stops")]
        [SettingPropertyGroup("{=mlf_group_shield_planting}Shield Planting & Formation")]
        public bool ShieldPlantingEnabled { get; set; } = true;

        [SettingPropertyBool("{=mlf_shield_auto_deploy}Auto Plant / Pick Up on Orders", Order = 1, RequireRestart = false, HintText = "{=mlf_shield_auto_deploy_hint}Your shield-bearing ranged soldiers automatically plant their shields when they are standing still with a hold/move-to-position order (fully arrived at the target point, not charging/advancing/falling back), and automatically pick them up when they receive a moving combat order (charge, advance, fall back, retreat, follow, attack entity) or are ordered to a new position while planted. Manual F11/J actions always take priority; the auto logic stands down for 3 seconds after a manual action")]
        [SettingPropertyGroup("{=mlf_group_shield_planting}Shield Planting & Formation")]
        public bool ShieldPlantingAutoDeployEnabled { get; set; } = true;

        [SettingPropertyInteger("{=mlf_shield_max_per_scan}Max Auto-Plants per Scan", 1, 50, "0", Order = 2, RequireRestart = false, HintText = "{=mlf_shield_max_per_scan_hint}Maximum number of soldiers that can plant OR pick up their shields in a single auto-scan cycle (every 2 seconds) - planting and picking up are throttled independently, each up to this many per cycle. With many shield-bearing ranged soldiers, this prevents stuttering from many shields being spawned or removed at the same moment; the remaining soldiers are handled on the following cycles. Manual F11/J actions are never limited")]
        [SettingPropertyGroup("{=mlf_group_shield_planting}Shield Planting & Formation")]
        public int ShieldPlantingMaxAutoDeployPerScan { get; set; } = 20;

        [SettingPropertyBool("{=mlf_shield_formation_enabled}Shield Bearers on Front, Flanks and Rear", Order = 0, RequireRestart = false, HintText = "{=mlf_shield_formation_enabled_hint}In line and loose formations where ranged soldiers make up more than 95% of the unit, shield-bearing ranged soldiers are repositioned with this priority: first fill the front rank, then the left and right flank columns, then the last two ranks (all filled without gaps), and remaining shield bearers fill the ranks in between from front to back. Repositioning activates immediately when the formation layout changes (width/formation order) and recalculates every 1.5 seconds during battle; your own character is never moved. When disabled, soldiers return to vanilla positioning")]
        [SettingPropertyGroup("{=mlf_group_shield_planting}Shield Planting & Formation")]
        public bool ShieldBearerFormationEnabled { get; set; } = true;

        [SettingPropertyBool("{=mlf_shield_ai_enabled}Apply to AI Troops", Order = 3, RequireRestart = false, HintText = "{=mlf_shield_ai_enabled_hint}AI-controlled soldiers on all non-player teams (enemy armies and allied AI lords) also plant their shields automatically when holding a position, and shield-bearing ranged soldiers in their line/loose formations are rearranged to the front, flanks and rear, mirroring the player-side behavior. When disabled, only the player's own troops are affected")]
        [SettingPropertyGroup("{=mlf_group_shield_planting}Shield Planting & Formation")]
        public bool ShieldPlantingAiEnabled { get; set; } = true;

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
