using System;
using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.MountAndBlade.View.CustomBattle;

namespace MutliLittleFixes.Patches
{
    // ──────────────────────────────────────────────────────────────
    // 补丁: CustomBattleFactory.StartCustomBattle (Prefix)
    // ──────────────────────────────────────────────────────────────
    // 原版 CustomBattleFactory.RegisterProvider<T> 会把类型名含 "naval" 的
    // 提供者插到 _providers 列表首位（战帆 DLC 的 NavalCustomBattleProvider
    // 即走这条路径），导致主菜单「自定义战斗」默认先打开海战配置。
    //
    // 本补丁在每次点击入口时调整 _providers 顺序：
    //   开启（默认）：把首个非海战（陆地战）提供者移到列表首位 → 陆地战优先；
    //   关闭：把海战提供者移回列表首位 → 还原原版 DLC 行为。
    // 顺序调整是幂等的，主菜单入口 StartCustomBattle() 只读 _providers[0]，
    // 配置界面内的「切换模式」按钮走 CollectNextProvider，共享同一列表，
    // 二者顺序一致，切换循环始终连贯。
    // ──────────────────────────────────────────────────────────────
    internal static class CustomBattleModeOrderPatch
    {
        internal static void Prefix()
        {
            var field = AccessTools.Field(typeof(CustomBattleFactory), "_providers");
            if (field == null)
                return;
            if (field.GetValue(null) is not List<Type> providers || providers.Count <= 1)
                return;

            if (Settings.Instance?.MainMenuCustomBattleLandFirstEnabled != true)
            {
                // 还原原版 DLC 行为：海战提供者回到首位
                int firstNavalIndex = providers.FindIndex(t => t.Name.ToLowerInvariant().Contains("naval"));
                if (firstNavalIndex <= 0)
                    return;
                var navalProvider = providers[firstNavalIndex];
                providers.RemoveAt(firstNavalIndex);
                providers.Insert(0, navalProvider);
                return;
            }

            // 陆地战优先：首个非海战提供者移到首位
            int firstLandIndex = providers.FindIndex(t => !t.Name.ToLowerInvariant().Contains("naval"));
            if (firstLandIndex <= 0)
                return;
            var landProvider = providers[firstLandIndex];
            providers.RemoveAt(firstLandIndex);
            providers.Insert(0, landProvider);
        }
    }
}
