# MutliLittleFixes — 开发规范

本文件记录本 Mod 的 Harmony 补丁注册规则与 MCM 配置开关规则。并且游戏本体源码已经放在本项目内“./原版游戏本体代码1.4.5”，以该版本代码为准！即使实际是更新的也不换用更新的版本。
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
5. 编译验证 + 核对注册清单（当前共 23 个补丁类：Patches/ 下 21 个 + TrajectorySystem/ 下 2 个）。

### 1.3 Transpiler 特别说明

`ScoreboardSortOrderPatch` 是唯一使用 Transpiler 的补丁。**实时 MCM 开关通过注入无分支 IL 实现**：在匹配的 `ldc.i4.1; ceq` 处将 `ldc.i4.1` 替换为 `call get_Enabled; conv.i4; ldc.i4.1; add`（等效于 `(int)Enabled + 1`：开启时推入 2 逆转，关闭时推入 1 原版），`ceq` 保持原位。**禁止**用 `new Label()` + `brfalse/br` 注入分支——所有 `new Label()` 内部编号都是 0（值相等），会与原方法已有标签塌缩成同一跳转目标，产生栈不平衡的 `InvalidProgramException`（启动即崩溃，已踩坑）。新写的 Transpiler 必须确保：不新建标签、不引入分支、保持栈平衡、原指令标签转移（如目标指令带标签，转移到注入序列第一条）。

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