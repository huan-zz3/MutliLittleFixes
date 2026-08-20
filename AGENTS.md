# MutliLittleFixes — 开发规范

本文件记录本 Mod 的 Harmony 补丁注册规则、MCM 配置开关规则与多语言本地化规则。并且游戏本体源码已经放在本项目内“./reference/原版游戏本体代码1.4.5”，以该版本代码为准！即使实际是更新的也不换用更新的版本。
**任何新增/修改功能前必须先阅读本文件。**

---

## 1. Harmony 补丁注册规则

### 1.1 核心规则：禁止自动发现，一律显式注册

- **禁止**在补丁类上使用 `[HarmonyPatch]` / `[HarmonyPostfix]` / `[HarmonyPrefix]` / `[HarmonyTranspiler]` / `[HarmonyTargetMethod]` 等属性。
- `SubModule.OnSubModuleLoad` **不调用** `PatchAll()`，只调用 `Patches.HarmonyPatchRegistry.Register(_harmony)`。
- 所有补丁必须在 **`Patches/HarmonyPatchRegistry.cs`** 中逐条显式挂载：
  ```csharp
  var original = AccessTools.Method(typeof(目标类), "目标方法");
  harmony.Patch(original, postfix: Patch(typeof(补丁类), "补丁方法"));
  ```
- `Patch(Type patchType, string methodName)` 帮助方法位于注册器内，自动解析含非公开的静态方法，找不到时抛 `MissingMethodException` 便于定位。

### 1.2 新增补丁的步骤（必须全部完成）

1. 在 `Patches/`（或对应目录）新建补丁类，**不写任何 Harmony 属性**，方法名随意（建议语义化命名，如 `Postfix`/`Prefix`/`调整Xxx`）。
2. 在 `HarmonyPatchRegistry.Register()` 中调用新增的 `RegisterXxx(harmony)` 私有方法，加入注册清单。
3. 如目标类/方法依赖外部 DLC（如战帆 DLC 的 `NavalDLC.GameComponents.NavalDLCShipDeploymentModel`），**必须用 `AccessTools.TypeByName` + null 检查**，未安装 DLC 时安全跳过（参考 `RegisterShipBattleLimit`）。
4. 如目标方法是多方法（如 `[HarmonyTargetMethods]` 时代），改为在注册器中**循环注册**，找不到单个方法时跳过（参考 `RegisterScoreboardSortOrder`）。
5. 编译验证 + 核对注册清单（当前共 36 个补丁类：Patches/ 下 34 个 + TrajectorySystem/ 下 2 个）。

### 1.3 Transpiler 特别说明

`ScoreboardSortOrderPatch` 是唯一使用 Transpiler 的补丁。**实时 MCM 开关通过注入无分支 IL 实现**：在匹配的 `ldc.i4.1; ceq` 处将 `ldc.i4.1` 替换为 `call get_Enabled; conv.i4; ldc.i4.1; add`（等效于 `(int)Enabled + 1`：开启时推入 2 逆转，关闭时推入 1 原版），`ceq` 保持原位。**禁止**用 `new Label()` + `brfalse/br` 注入分支——所有 `new Label()` 内部编号都是 0（值相等），会与原方法已有标签塌缩成同一跳转目标，产生栈不平衡的 `InvalidProgramException`（启动即崩溃，已踩坑）。新写的 Transpiler 必须确保：不新建标签、不引入分支、保持栈平衡、原指令标签转移（如目标指令带标签，转移到注入序列第一条）。

### 1.4 Postfix 返回值特别说明（pass-through 陷阱）

**Postfix 一律声明为 `void`**，如需改写结果请原地修改 `__result` 指向的对象后返回原引用。**禁止**让 Postfix 返回非 void——Harmony 2.4.x 会把返回非 void 的 Postfix 视为 pass-through postfix，要求其**第一个参数**必须是 `__result` 且类型与返回类型一致；若第一个参数是 `__instance`（或其他类型），`harmony.Patch()` 在启动注册时直接抛 `System.Exception`："Return type of pass through postfix ... does not match type of its first parameter"（启动即崩溃，已踩坑，见 `EncyclopediaClanExileFilterPatch`）。若确实需要 pass-through 语义，第一参数必须是 `__result` 且与返回类型相同。

---

## 2. MCM 配置开关规则

### 2.1 总则

- 所有功能**必须**有独立的启用/禁用开关（`bool` 属性），`RequireRestart = false`，默认开启（调试类默认关闭）。
- 开关是**实时生效**的：游戏内修改立即生效，**不需要重启**。补丁/行为在每次调用时读取 `Settings.Instance?.X`，不缓存。

### 2.2 设置类

- 主设置：`Settings.cs`（`AttributeGlobalSettings<Settings>`，Id `MutliLittleFixes_v1`）。绝大多数开关放这里。
- 轨迹系统设置：`TrajectorySystem/SiegeTrajectoryConfig.cs`（Id `MutliLittleFixes_SiegeTrajectory_v1`）。仅放轨迹/攻城器械相关开关（`EnableBallista`/`EnableMangonel`/`CoordinateTargetingEnabled`）。
- 新增开关按功能归入合适的 `SettingPropertyGroup`，`Order` 顺序递增，`HintText` 写清功能描述。

### 2.3 实时门控模式（必须遵循）

**Postfix 补丁** —— 方法体最前：
```csharp
// MCM 运行时开关 — 关闭时不干预
if (Settings.Instance?.XxxEnabled != true)
    return;
```

**Prefix 补丁（返回 bool 跳过原方法型）** —— 方法体最前（注意先给 `out` 参数赋默认值）：
```csharp
if (Settings.Instance?.XxxEnabled != true)
    return true; // 放行原方法
```

**MissionLogic.OnMissionTick** —— 顶部早退；若功能有持续性状态（蹲姿/阵型转移），需在关闭时清理：
```csharp
if (Settings.Instance?.XxxEnabled != true)
{
    // 清理本功能造成的状态（如 ForceAllFormationsToStand / ReturnAllMovedAgents）
    return;
}
```
参考：`RangedNoAmmoDebugBehavior`（简单早退）、`RangedNoAmmoBehavior`（关闭时归队）、`AutoCrouchMissionLogic`（关闭时站起）。

**CampaignBehavior 事件回调** —— 回调内最前检查（参考 `SkillLevelCapBehavior.OnDailyTickHero`）。

**从轨迹配置读取**（`SiegeTrajectoryConfig`）：
```csharp
MCM.Abstractions.Base.Global.GlobalSettings<SiegeTrajectoryConfig>.Instance?.XxxEnabled != true
```

---

## 3. 多语言本地化规则

> 依据 `docs/多语言本地化实现指南.md`。本 Mod 已全部接入游戏原生本地化系统（`TaleWorlds.Localization`）：代码写 `{=ID}English fallback`，翻译放 `ModuleData/Languages/<语言>/`，游戏启动时自动加载合并，无需注册。**玩家可见功能文本必须中英双文；默认关闭的调试日志保持中文。**

### 3.1 核心规则

- **玩家可见功能文本**（MCM 设置名/提示/分组、UI 文案、菜单选项、toast、通知消息、标注、筛选项、ExplainedNumber 描述等）**必须**写成 `new TextObject("{=mlf_xxx}English fallback", null)` 形式，并在语言文件中提供中英词条。
- **fallback 一律写英文**：漏译时玩家看到可读英文而非空串。
- **调试日志保持中文，不本地化**：默认关闭的 `LogDebug`/`LogScreen`/`[Test]`/`[ORCA]`/`[第9队]` 调试输出（由 MCM 调试开关控制）是开发者排障工具，玩家正常游玩不可见，保持中文注释便利即可。
- **ID 前缀**：`mlf_`（主设置与功能文本）；`st_`（SiegeTrajectoryConfig 既有词条，勿改）。避免与其它 mod/游戏原生词条冲突。

### 3.2 词条文件结构（已就位，新增词条时同步维护）

```
ModuleData/Languages/
├── English/
│   ├── language_data.xml          (id="English", supported_iso="en-GB,en-US,en,eng,...")
│   └── std_module_strings.xml     (英文词条)
└── CNs/
    ├── language_data.xml          (id="简体中文", supported_iso="zh-HANS,zh,zho,chi,zh-cn,zh-sg")
    └── std_module_strings_zho-CN.xml
```

- 词条 XML 格式：`<base xmlns="http://schemas.taleworlds.com/2007/04/GameSystem/" type="string">` + `<tags><tag language="简体中文"/></tags>` + `<strings><string id="..." text="..."/></strings>`。`xmlns` 与 `type` 缺一不可。
- 文件用 **UTF-8 带 BOM** 保存（Windows 工具按 ANSI 读会乱码）。
- XML 转义：`&`→`&amp;`、`<`→`&lt;`、`>`→`&gt;`、换行→`&#10;`。
- **无需**写进 `SubModule.xml` 的 `<Xmls/>`，引擎自动扫描所有模块的 `ModuleData/Languages/`。
- 新增词条后**核对完整性**：代码中所有 `{=ID}` 必须同时存在于 EN 与 CN 词条文件（数量一致，可脚本对比）。

### 3.3 代码写法

- **带变量的文本**：`new TextObject("{=mlf_xxx}Text {VAR}", null)` + `SetTextVariable("VAR", value)`（`SetTextVariable` 返回 `TextObject`，可链式）。翻译词条内保留 `{VAR}` 占位符。
- **MCM 属性**（`[SettingProperty*]`/`[SettingPropertyGroup]`）：DisplayName、HintText、组名均支持 `{=ID}`，MCM 内部走 `TextObject.ToString()` 解析（已核实 MCM 5.x 源码：`SettingsPropertyVM.cs` 的 `Name = new TextObject(DisplayName).ToString()`、组名 `LocalizationUtils.Localize(...)`）。`AttributeGlobalSettings<T>` 覆写的 `DisplayName` 属性同样写成 `new TextObject("{=ID}...", null).ToString()`。
- **UI Prefab XML 不写死文案**：文案永远在 C# 里用 `{=ID}` 生成并暴露为 `[DataSourceProperty]`，Prefab 只做 `@属性名` 绑定（参考 `BonusTabVM`/`BonusTabInjectors`）。
- **本地化后文本比较**：不要用 `textObject.Value == "某文本"` 判断（Value 会随语言变化）；改用 `textObject.GetID() == "mlf_xxx"`（参考 `EncyclopediaClanExileFilterPatch.IsExileFilter`）。
- 允许复用游戏原生词条（`GameTexts.FindText`），但查 ID 前先确认语义吻合。

### 3.4 新增文本的步骤（必须全部完成）

1. 代码中玩家可见文本写成 `new TextObject("{=mlf_xxx}English fallback", null)`（变量用 `{VAR}` + `SetTextVariable`）。
2. 在 `ModuleData/Languages/English/std_module_strings.xml` 与 `ModuleData/Languages/CNs/std_module_strings_zho-CN.xml` 各加一条同 ID 词条（中文词条取原中文文案）。
3. 编译验证 + 脚本核对代码 ID 与两个词条文件数量一致。

---

## 4. 全局崩溃捕获（CrashLog）

> 本 Mod 常驻的托管异常全量捕获系统，**对用户透明、无 MCM 开关、始终生效**。新增/修改任何代码前请理解本节，确保新代码不会破坏捕获机制，且新功能引发的异常能被正常记录定位。

### 4.1 机制与覆盖范围

- 由 `SubModule.OnSubModuleLoad` 挂载三个 AppDomain 级钩子（`OnSubModuleUnloaded` 卸载）：
  - `AppDomain.CurrentDomain.FirstChanceException` —— 任何异常在抛出的瞬间触发（含被游戏高层 catch 吞掉的），过滤出本 Mod 相关帧后记录。**这是"全量捕获"的核心。**
  - `AppDomain.CurrentDomain.UnhandledException` —— 异常逃逸到 CLR、游戏即将崩溃前记录完整堆栈。
  - `TaskScheduler.UnobservedTaskException` —— Task 异步异常兜底（回调内调用 `SetObserved()`）。
- 覆盖范围：Harmony 补丁方法、内部算法（TrajectorySystem / OrcaSystem / AutoResolve）、MissionBehavior / CampaignBehavior 回调、UI ViewModel —— 只要异常堆栈含 `MutliLittleFixes` 帧或抛出点属于本程序集即被记录。
- **只记录、不吞异常**：异常仍按原路径传播。补丁方法内该有的 try-catch 仍需保留（尤其 Prefix 异常会中止原方法执行）。

### 4.2 日志位置与限流

- 日志文件：游戏标准用户目录（`PlatformFileType.User`，同 `rgl_log.txt` / `Configs` 所在目录）下
  `%USERPROFILE%\Documents\Mount and Blade II Bannerlord\Logs\MutliLittleFixes_Crash.log`
  （解析失败时兜底退回游戏安装目录）。UTF-8 无 BOM，追加写，线程安全。
- 限流规则（防单点刷爆日志）：
  - 同签名异常（异常类型 + 抛出位置）10 秒窗口内只写一次完整堆栈，期间只计数（`[xN]` 标注累计次数）；
  - 单次会话详细条目上限 5000 条，超出后仅计数不再落盘。
- 日志为**调试排障工具，保持中文，不本地化**（AGENTS.md §3.1）。

### 4.3 新代码要求

1. **无需**为新代码额外添加 try-catch 来"防崩溃记录"——异常会自动被捕获层记录。但**不得**依赖捕获层兜底来掩盖逻辑错误：被记录的异常 = 已有功能故障，应修复而非忽略。
2. **禁止**在补丁方法内 `catch (Exception) { }` 静默吞掉异常（会导致捕获层与开发者都看不到问题）；如需优雅降级，catch 后应调用 `CrashLog.Write("功能名", ex)` 显式记录（参考 `AutoResolveLog.PrintError` 用法）。
3. **禁止**修改 `CrashLog` 的常驻挂载逻辑、限流参数或过滤规则使其失效；`CrashLog` 自身全部静默兜底，绝不再抛异常（避免递归触发捕获钩子）。
4. 新代码可随时调用 `CrashLog.Write(source, ex)` 记录任意异常（线程安全，自动限流）。
5. 若需向玩家展示错误（toast 等），用本地化文本；崩溃日志本身保持中文不本地化。

---

## 5. 海战模式禁用规则（战帆 Warsail DLC）

> 士兵 AI 行为调整功能在战帆 DLC 海战模式中**一律显式禁用**，这是硬性原则，新增/修改任何士兵行为功能前必须遵守本节。

### 5.1 原则

- **禁用范围**（9 项士兵 AI 行为调整功能，全部必须海战禁用）：
  1. 自动蹲下（`AutoCrouchMissionLogic`）
  2. 蹲下时举盾向上（`ShieldDirectionForCrouchPatch`）
  3. 旗帜士兵站位优化（`BannerBearerPositionPatch`）
  4. 无弹药远程移交第 9 队（`RangedNoAmmoBehavior`）
  5. 骑马长杆必定击倒（`MountedKnockDownPatch`）
  6. 盾牌插地/收盾（`ShieldPlantingBehavior`）
  7. 远程盾兵站位重排（`ShieldBearerFormationBehavior`）
  8. 首排持盾排序修复（`FormationFrontRankShieldSortPatch`）
  9. 长矛兵近身换刀（`SpearMeleeSwitchBehavior`）
- **原因**：海战结构与陆战根本不同——士兵随船移动（静止判定/编队状态不成立）、编队绑定船只（每船一队，`NavalTeamAgents` 强管理，阵型转移会被还原）、甲板非地形（`GetTerrainHeight` 取到海面高度，静态实体无法随船移动）。上述功能在海上要么失效、要么产生异常行为（插盾实体落水、第 9 队无船、站位交换破坏船-编队绑定），故统一显式禁用、不干预。
- **海战专属功能不受影响**：海战船只上限（`NavalBattleShipLimit` / `NavalDeployLimitPatch`）、自定义战斗陆地战优先（`CustomBattleModeOrderPatch`）等海战功能按原逻辑运行，不在禁用范围。

### 5.2 检测方式（统一入口）

- 统一调用 `NavalBattleDetector.IsNavalBattle(Mission)`（MissionLogic 内，传入 `this.Mission`）或 `NavalBattleDetector.IsNavalBattle()`（Harmony 补丁内，取 `Mission.Current`）。
- 检测基于原版 `Mission.IsNavalBattle` / `Mission.IsNavalRaidBattle`（即 `MissionTeamAIType` 为 `NavalBattle` / `NavalRaid`，含自定义海战与战役沿海掠夺海战），**不依赖 DLC 程序集**——未安装战帆 DLC 时类型/属性恒为 false，安全无副作用。
- **禁止**自行用场景名/地形类型/硬编码判断海战；**禁止**绕开 `NavalBattleDetector` 直接访问 DLC 类型（除非功能本身就是 DLC 专属，如 `NavalDeployLimitPatch` 用 `AccessTools.TypeByName` + null 检查的既有模式）。
- **两条检测通道的分工（不得混用）**：
  - **判断"当前任务是否海战" → 一律走 `NavalBattleDetector`**（原版 Mission 属性）。禁止用 `AccessTools.TypeByName` 判断是否海战——它检测的是"DLC 是否安装"而非"当前是否海战"，装了 DLC 的陆战会误判为海战。
  - **解析 DLC 专属补丁目标 → 一律走 `AccessTools.TypeByName` + null 检查**（AGENTS.md §1.2 第 3 条，如 `NavalDeployLimitPatch`）。此类目标方法只在海战部署时被调用，注册后无需再叠加海战检测。

### 5.3 实现位置要求

- **MissionLogic 的 `OnMissionTick`**：在 `Mission == null` / `MissionMode.Deployment` 早退之后、MCM 开关检查之前插入：
  ```csharp
  // 海战禁用（战帆 DLC 海战/沿海掠夺海战）— <注明该功能海战失效原因>
  if (NavalBattleDetector.IsNavalBattle(Mission))
      return;
  ```
  海战内本功能不会产生任何状态（无蹲姿/无插盾/无第 9 队记录），直接 `return` 即可，无需清理逻辑。
- **Harmony 补丁方法（Postfix/Prefix）**：方法体最前、MCM 开关检查之前插入：
  ```csharp
  // 海战禁用（战帆 DLC 海战/沿海掠夺海战）— 统一原则：士兵 AI 行为调整在海战不干预
  if (NavalBattleDetector.IsNavalBattle())
      return;                    // Postfix：直接返回
      // return true;           // Prefix（放行原方法）时用 true
  ```
- **新增任何士兵行为功能时**，必须同步添加海战禁用并核对本节清单（当前 9 项）。