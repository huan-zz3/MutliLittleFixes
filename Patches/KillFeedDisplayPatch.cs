using System;
using System.Runtime.CompilerServices;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Mission.KillFeed.General;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD.KillFeed.General;
using TaleWorlds.TwoDimension;

namespace MutliLittleFixes.Patches
{
    /// <summary>
    /// 战场右上角全军击杀信息流（SP General Kill Feed）显示优化：
    ///
    /// 1) LimitNotificationListPostfix —— 限制同时显示的条目数上限。
    ///    原版 SP 版击杀信息流没有任何条目数量控制，一次大规模击杀会瞬间堆积
    ///    几十条（每条 29px+5px 间距、停留 3 秒），把信息流拉出屏幕。
    ///    本补丁在 VM 层（SPGeneralKillNotificationVM.OnAgentRemoved）追加后置：
    ///    超过上限时立即移除最旧条目（RemoveAt(0)），保证屏幕条数恒定。
    ///
    /// 2) ShrinkOldEntriesPostfix —— 旧条目文字与图标渐进缩小。
    ///    1.4.5 的 Widget 基类没有可写的逐条目 Scale 属性（_scaleToUse 只读，
    ///    仅跟随全局 UI 缩放），但存在两条独立通道：
    ///    a) 文字：TextWidget 继承 BrushWidget，其 Brush 属性是懒克隆的（首次
    ///       访问才 Clone，之后每次返回同一个克隆体），且 Brush.FontSize 可写、
    ///       TextWidget.OnRender 每帧重读 Brush.FontSize——可以安全地按条目
    ///       独立修改字体大小而不污染共享的 SPHUD.KillFeed.Text brush。
    ///    b) 图标：MurdererTypeWidget / VictimTypeWidget / ActionIconWidget 是
    ///       Fixed 尺寸 widget，Sprite 按布局尺寸（AreaRect）绘制，直接修改
    ///       SuggestedWidth/Height 即可等比例缩放图标。
    ///    触发规则：屏幕条目数超过阈值后，最旧的若干条按新旧程度渐进缩小
    ///    （最旧缩到最小比例，临界处接近 100%），最新条目保持全尺寸。
    ///
    /// 由 HarmonyPatchRegistry 显式注册（不使用 [HarmonyPatch] 属性）。
    /// MCM 开关（KillFeedItemLimitEnabled / KillFeedShrinkEnabled）实时生效。
    /// </summary>
    internal static class KillFeedDisplayPatch
    {
        /// <summary>记录每个图标 widget 的原始建议尺寸，供缩放/恢复使用。</summary>
        private static readonly ConditionalWeakTable<Widget, WidgetSizeRecord> _originalSizes = new();

        private sealed class WidgetSizeRecord
        {
            public float Width;
            public float Height;
        }

        // ── 补丁 1：条目数量上限（VM 层，超出立即移除最旧） ──────────────

        internal static void LimitNotificationListPostfix(SPGeneralKillNotificationVM __instance)
        {
            // MCM 运行时开关 — 关闭时不干预
            if (Settings.Instance?.KillFeedItemLimitEnabled != true)
                return;

            int maxItems = Settings.Instance?.KillFeedMaxItems ?? 6;
            if (maxItems <= 0)
                return;

            var list = __instance.NotificationList;
            while (list.Count > maxItems)
            {
                list.RemoveAt(0); // 最旧条目（sibling index 0 = 堆叠底部）立即移除
            }
        }

        // ── 补丁 2：旧条目文字+图标渐进缩小（Widget 层，每帧重算缩放比例） ─

        internal static void ShrinkOldEntriesPostfix(SingleplayerGeneralKillFeedWidget __instance)
        {
            // MCM 运行时开关 — 关闭时恢复所有条目为原始大小（幂等：值相同不写入）
            if (Settings.Instance?.KillFeedShrinkEnabled != true)
            {
                RestoreAllEntries(__instance);
                return;
            }

            int threshold = Settings.Instance?.KillFeedShrinkThreshold ?? 4;
            float minScale = Settings.Instance?.KillFeedShrinkMinScale ?? 0.7f;
            if (threshold <= 0 || minScale <= 0f || minScale > 1f)
            {
                RestoreAllEntries(__instance);
                return;
            }

            int total = __instance.ChildCount;
            // 需要缩小的最旧条目数：超出阈值部分（total <= threshold 时为 0，
            // 所有条目回到 100%——同时也负责把"曾缩小、后因旧条目移除而回到
            // 阈值内"的条目恢复原大小）
            int shrinkCount = total - threshold;

            for (int i = 0; i < total; i++)
            {
                float scale = 1f;
                if (i < shrinkCount)
                {
                    // 最旧（i=0）缩到 minScale；临界（i=shrinkCount-1）≈ 100%
                    float t = shrinkCount <= 1 ? 0f : (float)i / (float)(shrinkCount - 1);
                    scale = Mathf.Lerp(minScale, 1f, t);
                }

                if (__instance.GetChild(i) is SingleplayerGeneralKillFeedItemWidget item)
                {
                    ApplyItemScale(item, scale);
                }
            }
        }

        /// <summary>对单一条目同时应用文字与图标的缩放。</summary>
        private static void ApplyItemScale(SingleplayerGeneralKillFeedItemWidget item, float scale)
        {
            ApplyFontScale(item.MurdererNameWidget, scale);
            ApplyFontScale(item.VictimNameWidget, scale);
            ApplyIconScale(item.MurdererTypeWidget, scale);
            ApplyIconScale(item.VictimTypeWidget, scale);
            ApplyIconScale(item.ActionIconWidget, scale);
        }

        /// <summary>把全部条目恢复到原始大小（scale = 1）。</summary>
        private static void RestoreAllEntries(SingleplayerGeneralKillFeedWidget widget)
        {
            int total = widget.ChildCount;
            for (int i = 0; i < total; i++)
            {
                if (widget.GetChild(i) is SingleplayerGeneralKillFeedItemWidget item)
                {
                    ApplyItemScale(item, 1f);
                }
            }
        }

        /// <summary>
        /// 对单个 TextWidget 应用字体缩放。
        /// 通过 .Brush 属性（懒克隆）拿到独立克隆体，基准大小取自 ClonedFrom
        /// （指向共享的原 brush，FontSize 恒为原始值），避免多次缩放累积误差。
        /// </summary>
        private static void ApplyFontScale(TextWidget textWidget, float scale)
        {
            if (textWidget == null)
                return;

            var brush = textWidget.Brush; // 触发懒克隆；此后每次返回同一克隆体
            int baseSize = brush.ClonedFrom?.FontSize ?? brush.FontSize;
            int target = Math.Max(4, (int)(baseSize * scale));

            if (brush.FontSize != target)
            {
                brush.FontSize = target;
            }
        }

        /// <summary>
        /// 对单个图标 widget（及其子节点）应用尺寸缩放。
        /// 图标由 Fixed 尺寸 widget 承载（兵种图标 38×38、动作图标 27×29），
        /// Sprite 按布局尺寸绘制，因此改 SuggestedWidth/Height 即等比例缩放。
        /// 首次见到某 widget 时缓存其原始尺寸，后续始终以原始值为基准，避免
        /// 多次缩放累积误差；值未变化时不写入，避免每帧触发布局重算。
        /// </summary>
        private static void ApplyIconScale(Widget iconWidget, float scale)
        {
            if (iconWidget == null)
                return;

            ApplyIconScaleSingle(iconWidget, scale);
            // 兵种图标由「背景框 + 内部图标」两层组成（MurdererTypeWidget 属性
            // 指向背景框，内部图标是其子节点），需同步缩放避免错位。
            for (int i = 0; i < iconWidget.ChildCount; i++)
            {
                ApplyIconScaleSingle(iconWidget.GetChild(i), scale);
            }
        }

        private static void ApplyIconScaleSingle(Widget widget, float scale)
        {
            if (widget == null || widget.WidthSizePolicy != SizePolicy.Fixed)
                return;

            var record = _originalSizes.GetOrCreateValue(widget);
            if (record.Width <= 0f)
            {
                record.Width = widget.SuggestedWidth;
                record.Height = widget.SuggestedHeight;
            }
            if (record.Width <= 0f)
                return; // 无有效原始尺寸，跳过

            float targetWidth = record.Width * scale;
            float targetHeight = record.Height * scale;

            if (Math.Abs(widget.SuggestedWidth - targetWidth) > 0.5f)
            {
                widget.SuggestedWidth = targetWidth;
            }
            if (Math.Abs(widget.SuggestedHeight - targetHeight) > 0.5f)
            {
                widget.SuggestedHeight = targetHeight;
            }
        }
    }
}
