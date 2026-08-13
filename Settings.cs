using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Core;

namespace MutliLittleFixes
{
    internal sealed class Settings : AttributeGlobalSettings<Settings>
    {
        public override string Id => "MutliLittleFixes_v1";
        public override string DisplayName => "MutliLittleFixes";
        public override string FolderName => "MutliLittleFixes";
        public override string FormatType => "json2";

        [SettingPropertyFloatingInteger("经验倍率", 0.1f, 1000.0f, "#0.0x", Order = 0, RequireRestart = false, HintText = "主角获取经验的倍率（影响所有技能经验获取和角色等级提升速度）")]
        [SettingPropertyGroup("经验设置")]
        public float ExperienceMultiplier { get; set; } = 1.0f;

        [SettingPropertyBool("启用经验倍率", Order = 1, RequireRestart = false, HintText = "实时开关：关闭后经验倍率功能整体失效（数值保持但不生效）")]
        [SettingPropertyGroup("经验设置")]
        public bool ExperienceMultiplierEnabled { get; set; } = false;

        [SettingPropertyFloatingInteger("活力 (Vigor)", 0.1f, 1000.0f, "#0.0x", Order = 1, RequireRestart = false, HintText = "活力对应技能的红利学习倍率")]
        [SettingPropertyGroup("属性增长倍率")]
        public float VigorMultiplier { get; set; } = 1.0f;

        [SettingPropertyBool("启用属性红利倍率", Order = 0, RequireRestart = false, HintText = "实时开关：关闭后属性红利学习倍率功能整体失效（数值保持但不生效）")]
        [SettingPropertyGroup("属性增长倍率")]
        public bool AttributeLearningBonusEnabled { get; set; } = false;

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
        public int SkillCapDefault { get; set; } = 500;

        [SettingPropertyBool("启用技能等级上限（原版硬上限=1024）", Order = 6, RequireRestart = false, HintText = "实时开关：关闭后技能上限功能整体失效（数值保持但不生效）")]
        [SettingPropertyGroup("技能等级上限")]
        public bool SkillLevelCapEnabled { get; set; } = false;

        [SettingPropertyInteger("活力 (Vigor)", 10, 1024, "0", Order = 8, RequireRestart = false, HintText = "活力类技能（单手/双手/长杆）的等级上限")]
        [SettingPropertyGroup("技能等级上限")]
        public int VigorSkillCap { get; set; } = 500;

        [SettingPropertyInteger("控制 (Control)", 10, 1024, "0", Order = 9, RequireRestart = false, HintText = "控制类技能（弓/弩/投掷）的等级上限")]
        [SettingPropertyGroup("技能等级上限")]
        public int ControlSkillCap { get; set; } = 500;

        [SettingPropertyInteger("耐力 (Endurance)", 10, 1024, "0", Order = 10, RequireRestart = false, HintText = "耐力类技能（骑术/跑动/锻造）的等级上限")]
        [SettingPropertyGroup("技能等级上限")]
        public int EnduranceSkillCap { get; set; } = 500;

        [SettingPropertyInteger("狡诈 (Cunning)", 10, 1024, "0", Order = 11, RequireRestart = false, HintText = "狡诈类技能（侦查/战术/流氓）的等级上限")]
        [SettingPropertyGroup("技能等级上限")]
        public int CunningSkillCap { get; set; } = 500;

        [SettingPropertyInteger("社交 (Social)", 10, 1024, "0", Order = 12, RequireRestart = false, HintText = "社交类技能（魅力/统御/交易）的等级上限")]
        [SettingPropertyGroup("技能等级上限")]
        public int SocialSkillCap { get; set; } = 500;

        [SettingPropertyInteger("智力 (Intelligence)", 10, 1024, "0", Order = 13, RequireRestart = false, HintText = "智力类技能（管理/医术/工程）的等级上限")]
        [SettingPropertyGroup("技能等级上限")]
        public int IntelligenceSkillCap { get; set; } = 500;

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

        [SettingPropertyBool("百科家族页流亡筛选", Order = 13, RequireRestart = false, HintText = "在百科全书的家族列表中，为「状态」筛选组新增「在流亡/不在流亡」选项（无国无地且非叛军/土匪/小派系、不含玩家家族）")]
        [SettingPropertyGroup("UI")]
        public bool EncyclopediaClanExileFilter { get; set; } = true;

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
        public bool RestorationEnabled { get; set; } = false;

        [SettingPropertyInteger("补兵所需天数", 1, 30, "0", Order = 21, RequireRestart = false, HintText = "释放后补兵所需天数")]
        [SettingPropertyGroup("领主释放补兵")]
        public int RestorationDays { get; set; } = 7;

        [SettingPropertyFloatingInteger("恢复兵力比例", 0.0f, 1.0f, "0.0", Order = 22, RequireRestart = false, HintText = "恢复兵力占队伍上限的比例(0=关闭)")]
        [SettingPropertyGroup("领主释放补兵")]
        public float RestorationPartySizeRatio { get; set; } = 0.4f;

        [SettingPropertyFloatingInteger("Tier1-2兵种占比", 0.0f, 1.0f, "0.0", Order = 23, RequireRestart = false, HintText = "Tier1-2兵种占比")]
        [SettingPropertyGroup("领主释放补兵")]
        public float RestorationTier12Ratio { get; set; } = 0.50f;

        [SettingPropertyFloatingInteger("Tier3-4兵种占比", 0.0f, 1.0f, "0.0", Order = 24, RequireRestart = false, HintText = "Tier3-4兵种占比")]
        [SettingPropertyGroup("领主释放补兵")]
        public float RestorationTier34Ratio { get; set; } = 0.30f;

        [SettingPropertyFloatingInteger("Tier5-6兵种占比", 0.0f, 1.0f, "0.0", Order = 25, RequireRestart = false, HintText = "Tier5-6兵种占比")]
        [SettingPropertyGroup("领主释放补兵")]
        public float RestorationTier56Ratio { get; set; } = 0.20f;

        [SettingPropertyInteger("每兵金币", 0, 100000, "0", Order = 26, RequireRestart = false, HintText = "每兵给予领主的金币数量(0=不给金币)")]
        [SettingPropertyGroup("领主释放补兵")]
        public int RestorationGoldPerTroop { get; set; } = 100;

        [SettingPropertyInteger("每兵谷物", 0, 100000, "0", Order = 27, RequireRestart = false, HintText = "每兵给予领主的谷物数量，避免队伍饥饿减员(0=不给谷物)")]
        [SettingPropertyGroup("领主释放补兵")]
        public int RestorationFoodPerTroop { get; set; } = 10;

        [SettingPropertyInteger("放弃补兵天数", 1, 60, "0", Order = 28, RequireRestart = false, HintText = "超过此天数领主仍无队伍则放弃补兵")]
        [SettingPropertyGroup("领主释放补兵")]
        public int RestorationAbandonDays { get; set; } = 14;

        [SettingPropertyBool("启用领土带兵上限", Order = 28, RequireRestart = false, HintText = "启用/禁用领土丧失补偿功能")]
        [SettingPropertyGroup("领土带兵上限")]
        public bool TerritoryBonusEnabled { get; set; } = false;

        [SettingPropertyInteger("城镇补偿值", 0, 50, "0", Order = 29, RequireRestart = false, HintText = "每丢失一座城镇增加的队伍上限(衰减前)")]
        [SettingPropertyGroup("领土带兵上限")]
        public int TerritoryBonusTownValue { get; set; } = 20;

        [SettingPropertyInteger("城堡补偿值", 0, 50, "0", Order = 30, RequireRestart = false, HintText = "每丢失一座城堡增加的队伍上限(衰减前)")]
        [SettingPropertyGroup("领土带兵上限")]
        public int TerritoryBonusCastleValue { get; set; } = 10;

        [SettingPropertyFloatingInteger("衰减乘数", 0.0f, 1.0f, "0.0", Order = 31, RequireRestart = false, HintText = "连续丢失领土的衰减乘数(1.0=线性)")]
        [SettingPropertyGroup("领土带兵上限")]
        public float TerritoryBonusDiminishRate { get; set; } = 0.7f;

        [SettingPropertyInteger("最大补偿上限", 0, 500, "0", Order = 32, RequireRestart = false, HintText = "王国可累积的最大补偿值")]
        [SettingPropertyGroup("领土带兵上限")]
        public int TerritoryBonusMaxCap { get; set; } = 200;

        [SettingPropertyInteger("征服固化天数", 0, 365, "0", Order = 33, RequireRestart = false, HintText = "占领城池超过此天数后视作本国领土，不再抵消丢失产生的补偿(0=关闭，84=一年)")]
        [SettingPropertyGroup("领土带兵上限")]
        public int ConquestSolidifyDays { get; set; } = 42;

        [SettingPropertyInteger("丢失过期天数", 0, 365, "0", Order = 34, RequireRestart = false, HintText = "丢失城池超过此天数后视作他国领土，不再参与补偿计算(0=关闭，84=一年)")]
        [SettingPropertyGroup("领土带兵上限")]
        public int LossExpireDays { get; set; } = 84;

        [SettingPropertyBool("仅封臣生效", Order = 35, RequireRestart = false, HintText = "仅对封臣家族生效(不包括雇佣兵)")]
        [SettingPropertyGroup("领土带兵上限")]
        public bool TerritoryBonusVassalsOnly { get; set; } = true;

        [SettingPropertyInteger("海战船只上限", 3, 8, "0", Order = 36, RequireRestart = false,
            HintText = "参与海战/沿海掠夺时，玩家可同时出战的最大船只数量（至少3艘，最多8艘）")]
        [SettingPropertyGroup("海战设置")]
        public int NavalBattleShipLimit { get; set; } = 8;

        [SettingPropertyBool("家族成员可用提醒", Order = 37, RequireRestart = false, HintText = "家族成员从俘虏释放/逃脱变为可用状态时，在屏幕中央弹出 toast 提示")]
        [SettingPropertyGroup("通知")]
        public bool CompanionAutoRecallEnabled { get; set; } = true;

        [SettingPropertyBool("领主释放补兵调试日志", Order = 38, RequireRestart = false, HintText = "在游戏界面左下角显示领主释放补兵功能的调试日志信息，用于排查补兵逻辑问题")]
        [SettingPropertyGroup("调试")]
        public bool EnableRestorationDebugLog { get; set; } = false;

        [SettingPropertyBool("领土丧失补偿调试日志", Order = 39, RequireRestart = false, HintText = "在游戏界面左下角显示领土丧失补偿功能的调试日志信息，用于排查补偿计算和UI刷新问题")]
        [SettingPropertyGroup("调试")]
        public bool EnableTerritoryBonusDebugLog { get; set; } = false;

        [SettingPropertyBool("家族成员提醒调试日志", Order = 40, RequireRestart = false, HintText = "在游戏界面左下角显示家族成员可用提醒功能的调试日志信息")]
        [SettingPropertyGroup("调试")]
        public bool EnableCompanionRecallDebugLog { get; set; } = false;

        [SettingPropertyBool("远程弹药归零调试（按,键）", Order = 41, RequireRestart = false, HintText = "战斗中按 , 键随机将 5% 远程士兵弹药强制归零，用于测试第9队移交逻辑")]
        [SettingPropertyGroup("调试")]
        public bool RangedNoAmmoDebugEnabled { get; set; } = false;

        [SettingPropertyBool("禁止AI自动宣战", Order = 42, RequireRestart = false, HintText = "玩家是国王时，禁止属下领主（AI）自动发起宣战决策")]
        [SettingPropertyGroup("外交设置")]
        public bool PreventAIWarDeclaration { get; set; } = true;

        [SettingPropertyBool("玩家攻城必定候选", Order = 43, RequireRestart = false, HintText = "玩家亲自率军攻下的城池，在分封投票中必定进入候选名单")]
        [SettingPropertyGroup("攻城")]
        public bool PlayerFiefCandidacyEnabled { get; set; } = true;

        [SettingPropertyBool("攻城器械优先攻击器械", Order = 44, RequireRestart = false, HintText = "玩家进攻方时，攻城器械优先攻击敌方远程器械")]
        [SettingPropertyGroup("攻城")]
        public bool SiegeTargetSelectionEnabled { get; set; } = true;

        [SettingPropertyBool("自动蹲下", Order = 45, RequireRestart = false, HintText = "纯步兵/纯远程小队在 Hold 静止时自动蹲下（线阵首排/远程前半排/松散阵远程全蹲）")]
        [SettingPropertyGroup("阵型与战斗")]
        public bool AutoCrouchEnabled { get; set; } = true;

        [SettingPropertyBool("蹲下时举盾向上", Order = 46, RequireRestart = false, HintText = "前排士兵蹲下时盾牌方向由防下改为防上")]
        [SettingPropertyGroup("阵型与战斗")]
        public bool CrouchShieldDirectionEnabled { get; set; } = true;

        [SettingPropertyBool("旗帜士兵站位优化", Order = 47, RequireRestart = false, HintText = "将旗手站位从最左前列调整到最后排中间")]
        [SettingPropertyGroup("阵型与战斗")]
        public bool BannerBearerPositionEnabled { get; set; } = true;

        [SettingPropertyBool("无弹药远程移交第9队", Order = 48, RequireRestart = false, HintText = "弹药射完的远程士兵自动移入第9队，恢复弹药后归队")]
        [SettingPropertyGroup("阵型与战斗")]
        public bool RangedNoAmmoEnabled { get; set; } = true;

        [SettingPropertyBool("战斗结算排序反转", Order = 49, RequireRestart = false, HintText = "结算窗口点击表头排序循环反转为：默认→降序→升序")]
        [SettingPropertyGroup("阵型与战斗")]
        public bool ScoreboardSortOrderEnabled { get; set; } = true;

        [SettingPropertyBool("调试圈/点渲染视图", Order = 50, RequireRestart = false, HintText = "玩家脚下显示红色圆圈+前方黄色点（调试用）")]
        [SettingPropertyGroup("调试")]
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

        [SettingPropertyBool("架矛骑枪必定击倒", Order = 51, RequireRestart = false, HintText = "骑乘架矛（被动攻击）的长杆武器命中未上马的步兵/远程单位时必定击倒（敌我双方对称生效；格挡化解时不生效）")]
        [SettingPropertyGroup("骑马长杆击倒")]
        public bool CouchLanceKnockDownEnabled { get; set; } = true;

        [SettingPropertyBool("马上长杆刺击必定击倒", Order = 52, RequireRestart = false, HintText = "骑乘状态下普通长杆刺击命中未上马的步兵/远程单位时必定击倒（敌我双方对称生效；格挡化解时不生效）")]
        [SettingPropertyGroup("骑马长杆击倒")]
        public bool MountedPolearmThrustKnockDownEnabled { get; set; } = true;

        [SettingPropertyFloatingInteger("刺击最小相对速度", 0.0f, 10.0f, "#0.0", Order = 53, RequireRestart = false, HintText = "马上长杆刺击触发必定击倒所需的最小相对速度（攻击者与目标的移动速度向量差长度，单位与游戏速度一致）。默认 2.0，避免原地刺击也击倒；设为 0 则取消速度要求")]
        [SettingPropertyGroup("骑马长杆击倒")]
        public float MountedPolearmThrustMinRelativeSpeed { get; set; } = 2.0f;

        [SettingPropertyFloatingInteger("刺击击倒伤害加成", 0.0f, 2.0f, "0%", Order = 54, RequireRestart = false, HintText = "马上长杆刺击触发必定击倒时，本次攻击造成的伤害加成比例（默认 0.3 = +30%）。设为 0 则无伤害加成")]
        [SettingPropertyGroup("骑马长杆击倒")]
        public float MountedPolearmThrustKnockDownDamageBonus { get; set; } = 0.3f;

        [SettingPropertyBool("启用自定义出场比例", Order = 55, RequireRestart = false, HintText = "游戏设置「单位生成优先级 = 高等级优先」时生效：按下方四项比例配额调度步兵/射手/骑兵/骑射手的出场节奏，兵种内部仍按等级从高到低，避免高等级兵种挤占所有出场名额。关闭后恢复原版高等级优先逻辑")]
        [SettingPropertyGroup("出场比例")]
        public bool UnitSpawnRatioEnabled { get; set; } = true;

        [SettingPropertyInteger("步兵比例", 0, 100, "0", Order = 56, RequireRestart = false, HintText = "步兵出场配额权重（相对值，越大出场越频繁；设为 0 则步兵不登场；四类总和建议 100）")]
        [SettingPropertyGroup("出场比例")]
        public int InfantryRatio { get; set; } = 15;

        [SettingPropertyInteger("射手比例", 0, 100, "0", Order = 57, RequireRestart = false, HintText = "射手出场配额权重（相对值，越大出场越频繁；设为 0 则射手不登场；四类总和建议 100）")]
        [SettingPropertyGroup("出场比例")]
        public int ArcherRatio { get; set; } = 65;

        [SettingPropertyInteger("骑兵比例", 0, 100, "0", Order = 58, RequireRestart = false, HintText = "骑兵出场配额权重（相对值，越大出场越频繁；设为 0 则骑兵不登场；四类总和建议 100）")]
        [SettingPropertyGroup("出场比例")]
        public int CavalryRatio { get; set; } = 15;

        [SettingPropertyInteger("骑射手比例", 0, 100, "0", Order = 59, RequireRestart = false, HintText = "骑射手出场配额权重（相对值，越大出场越频繁；设为 0 则骑射手不登场；四类总和建议 100）")]
        [SettingPropertyGroup("出场比例")]
        public int HorseArcherRatio { get; set; } = 5;

        [SettingPropertyBool("启用NPC家族部队数量加成", Order = 60, RequireRestart = false, HintText = "为所有NPC领主家族在原版部队数量上限基础上额外增加部队数（仅影响AI家族每日外派，不影响玩家家族）")]
        [SettingPropertyGroup("NPC领主家族调整")]
        public bool NpcClanPartyLimitBonusEnabled { get; set; } = false;

        [SettingPropertyInteger("NPC家族部队数量加成", 0, 10, "0", Order = 61, RequireRestart = false, HintText = "在所有NPC领主家族原版部队数量上限基础上额外增加的部队数（默认+2，即Tier0-2家族从1支变3支、Tier3-4从2支变4支、Tier5-6从3支变5支；设为0关闭加成）")]
        [SettingPropertyGroup("NPC领主家族调整")]
        public int NpcClanPartyLimitBonus { get; set; } = 1;

        [SettingPropertyBool("启用招募补充倍率", Order = 62, RequireRestart = false, HintText = "实时开关：关闭后招募补充倍率不生效（名人每日补充概率保持原版）")]
        [SettingPropertyGroup("招募补充")]
        public bool VolunteerRecruitRateEnabled { get; set; } = false;

        [SettingPropertyFloatingInteger("每日补充概率倍率", 0.5f, 5.0f, "#0.0x", Order = 63, RequireRestart = false, HintText = "城镇/村庄名人每日补充士兵的概率倍率（1.0=原版，2.0=期望翻倍；槽位越深原版概率越低，倍率按比例放大；倍率>1.9 时前几个槽位将必然补充，但升级仍需另外加速）")]
        [SettingPropertyGroup("招募补充")]
        public float VolunteerRecruitRateMultiplier { get; set; } = 2.0f;

        [SettingPropertyBool("启用升级加速", Order = 64, RequireRestart = false, HintText = "实时开关：关闭后志愿者升级概率倍率不生效（保持原版极低的升级概率）")]
        [SettingPropertyGroup("招募补充")]
        public bool VolunteerUpgradeRateEnabled { get; set; } = false;

        [SettingPropertyFloatingInteger("被招募士兵升级概率倍率", 1.0f, 100.0f, "#0.0x", Order = 65, RequireRestart = false, HintText = "城镇/村庄名人士兵的每日升级概率倍率（1.0=原版；原版升级概率=log2(影响力/等级)*0.01，如影响力30的2级名人每天约5%概率升级；倍率10则约50%）")]
        [SettingPropertyGroup("招募补充")]
        public float VolunteerUpgradeRateMultiplier { get; set; } = 2.0f;

        [SettingPropertyBool("加入战斗自由撤退", Order = 66, RequireRestart = false, HintText = "玩家加入大地图上已有的友方战斗后（无论友方是进攻方还是防守方），encounter 菜单始终提供「离开」选项，可随时带着部队撤出战场；玩家自己发起的守城/攻城战斗不受影响，保持原版规则")]
        [SettingPropertyGroup("加入战斗")]
        public bool FreeBattleRetreatEnabled { get; set; } = true;

        [SettingPropertyBool("存档以日期时间命名", Order = 67, RequireRestart = false, HintText = "快速存档与自动存档改用「存档时的日期时间」命名（save_qu_/save_au_ 前缀），每个战役各自独立轮转，满员后新档保存成功时自动淘汰该战役最旧的；另存为与铁人模式保持原版逻辑。关闭后完全恢复原版命名（saveNNN / saveauto1-3），已生成的日期档不会被自动删除")]
        [SettingPropertyGroup("存档设置（启用后必须先试用！确认无错后方可安心）")]
        public bool DatedSaveNamingEnabled { get; set; } = false;

        [SettingPropertyInteger("轮转池容量", 1, 50, "0", Order = 68, RequireRestart = false, HintText = "日期时间存档（自动+快速）每个战役各自轮转池的最大文件数：该战役满员后再次快速/自动存档时，先保存新档成功，再按时间淘汰该战役最旧的；不同战役的存档互不淘汰（1-50，默认 10）")]
        [SettingPropertyGroup("存档设置（启用后必须先试用！确认无错后方可安心）")]
        public int DatedSavePoolSize { get; set; } = 10;

        [SettingPropertyBool("日期时间存档调试日志", Order = 69, RequireRestart = false, HintText = "在游戏界面左下角显示日期时间存档功能的日志（保存的新档名、保存结果、轮转淘汰的旧档），用于排查命名与轮转逻辑")]
        [SettingPropertyGroup("调试")]
        public bool DatedSaveNamingDebugLogEnabled { get; set; } = false;

        [SettingPropertyBool("启用粮草运输支援", Order = 70, RequireRestart = false, HintText = "玩家家族富余城镇每 6 游戏小时检查一次，向缺粮的玩家家族城镇/城堡派出由驻军转化的运粮队，直接加减城镇粮草值（不走市场消费转化）")]
        [SettingPropertyGroup("粮草运输支援")]
        public bool TransportSupportEnabled { get; set; } = true;

        [SettingPropertyInteger("需被支援粮草阈值", 0, 300, "0", Order = 71, RequireRestart = false, HintText = "玩家家族城镇/城堡的粮草低于此值时列入缺粮名单，等待其他城镇支援（城镇上限300，城堡上限450）")]
        [SettingPropertyGroup("粮草运输支援")]
        public int TargetFoodThreshold { get; set; } = 60;

        [SettingPropertyInteger("可发起支援粮草阈值", 0, 450, "0", Order = 72, RequireRestart = false, HintText = "玩家家族城镇的粮草高于此值才允许派出运粮队（派出的粮不会把本城扣到低于此值）")]
        [SettingPropertyGroup("粮草运输支援")]
        public int SourceFoodThreshold { get; set; } = 200;

        [SettingPropertyInteger("可发起支援驻军阈值", 0, 2000, "0", Order = 73, RequireRestart = false, HintText = "玩家家族城镇的驻军人数高于此值才允许派出运粮队（原版最低驻军：城镇125/城堡75）")]
        [SettingPropertyGroup("粮草运输支援")]
        public int SourceGarrisonThreshold { get; set; } = 250;

        [SettingPropertyInteger("运输队人数", 10, 100, "0", Order = 74, RequireRestart = false, HintText = "每支运粮队从驻军转化的士兵数（高低级各半随机抽取；途中战损即永久损失，返回后归还驻军）")]
        [SettingPropertyGroup("粮草运输支援")]
        public int TransportPartySize { get; set; } = 30;

        [SettingPropertyInteger("每兵抽象粮", 0, 100, "0", Order = 75, RequireRestart = false, HintText = "每名士兵携带的抽象支援粮数量：总支援粮 = 运输队人数 × 此值，出发时直接从源城粮草扣除、到达后直接加入目标城粮草（不走市场）")]
        [SettingPropertyGroup("粮草运输支援")]
        public int FoodPerTroop { get; set; } = 2;

        [SettingPropertyInteger("每兵实物粮", 0, 100, "0", Order = 76, RequireRestart = false, HintText = "每名士兵携带的实物谷物数量（入队背包，仅供运输队途中自身消耗，与支援粮互不互通；剩余在交付/回收时销毁）")]
        [SettingPropertyGroup("粮草运输支援")]
        public int PhysicalFoodPerTroop { get; set; } = 2;

        [SettingPropertyInteger("单城被支援上限", 1, 10, "0", Order = 77, RequireRestart = false, HintText = "同一座缺粮城镇/城堡同时最多接受几支运粮队支援")]
        [SettingPropertyGroup("粮草运输支援")]
        public int MaxSupportingTownsPerTarget { get; set; } = 3;

        [SettingPropertyInteger("单城外派上限", 1, 10, "0", Order = 78, RequireRestart = false, HintText = "同一座富余城镇同时最多派出几支运粮队")]
        [SettingPropertyGroup("粮草运输支援")]
        public int MaxOutgoingTransportsPerTown { get; set; } = 2;

        [SettingPropertyBool("粮草运输支援调试日志", Order = 79, RequireRestart = false, HintText = "在游戏界面左下角显示粮草运输支援的调度/交付/异常日志（派队、交付、退款、被毁等）")]
        [SettingPropertyGroup("粮草运输支援")]
        public bool EnableSupportDebugLog { get; set; } = false;

        [SettingPropertyBool("流亡家族永不灭亡", Order = 80, RequireRestart = false, HintText = "去除无国无地流亡家族（灭国后流浪的家族）的 28 天生存倒计时灭亡机制，使其永久存续，直到加入其他王国或获得领地。关闭后恢复原版倒计时灭亡")]
        [SettingPropertyGroup("流亡家族")]
        public bool WanderingClanSurvivalEnabled { get; set; } = false;

        [SettingPropertyBool("玩家阵亡不托管部队", Order = 81, RequireRestart = false, HintText = "玩家角色阵亡后，阻止系统把玩家部队强制切换为 AI 全权指挥；部队将保持玩家阵亡瞬间的最后指令继续战斗（阵亡后命令界面仍会被原版关闭，无法再手动下令）。关闭后恢复原版：阵亡即 AI 全权接管")]
        [SettingPropertyGroup("阵型与战斗")]
        public bool PlayerDeathNoAITakeoverEnabled { get; set; } = false;

        [SettingPropertyBool("首排持盾排序修复", Order = 82, RequireRestart = false, HintText = "修复原版阵型整理的收敛缺陷：持盾兵（或架矛兵）反复向前冒泡至列稳定、跨列补位遇满排时跳过而非中止整个整理，保证首排在有持盾者可换时必然满盾（原版会出现首排非盾兵+第二排持盾兵并存的乱序）")]
        [SettingPropertyGroup("阵型与战斗")]
        public bool FormationFrontRankSortEnabled { get; set; } = true;

        [SettingPropertyBool("村庄出资重建", Order = 83, RequireRestart = false, HintText = "被彻底掠夺（荒废）的村庄菜单提供「出资重建」选项：花费 10000 第纳尔，3 天后村庄自动重建完成（恢复正常运转并奖励村庄名人好感 15~20）。关闭后菜单选项隐藏；已出资的进行中重建不受影响，仍会按时完成")]
        [SettingPropertyGroup("村庄重建")]
        public bool VillageRebuildEnabled { get; set; } = true;

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
