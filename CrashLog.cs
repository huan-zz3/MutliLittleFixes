using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace MutliLittleFixes
{
    /// <summary>
    /// 全局崩溃捕获 —— 日志落盘层（常驻生效，对用户透明，无 MCM 开关）。
    ///
    /// 机制：由 SubModule.OnSubModuleLoad 挂载三个 AppDomain 级钩子，捕获本 Mod
    /// 代码（Harmony 补丁 / 内部算法 / Behavior 回调 / UI ViewModel）抛出的
    /// 全部托管异常并落盘：
    ///   - FirstChanceException   任何异常在抛出的瞬间触发（含被游戏高层 catch 吞掉的），
    ///                            过滤出本 Mod 相关的帧后记录 —— 这是"全量捕获"的核心。
    ///   - UnhandledException     异常逃逸到 CLR、游戏即将崩溃前记录完整堆栈。
    ///   - UnobservedTaskException Task 异步异常兜底。
    ///
    /// 日志位置：游戏标准用户目录（PlatformFileType.User）下
    ///   %USERPROFILE%\Documents\Mount and Blade II Bannerlord\Logs\MutliLittleFixes_Crash.log
    ///
    /// 限流规则（防单点刷爆日志）：
    ///   - 同签名异常（异常类型 + 抛出位置）10 秒窗口内只写一次完整堆栈，期间只计数；
    ///   - 单次会话详细条目上限 5000 条，超出后仅计数不再落盘。
    ///
    /// 注意：本类只"记录"不"吞异常"——异常仍按原路径传播。补丁方法内该自己
    /// try-catch 的仍需自行处理（尤其 Prefix 异常会中止原方法执行）。
    /// 本类自身绝不再抛任何异常（全部静默兜底），避免递归触发捕获钩子。
    /// </summary>
    internal static class CrashLog
    {
        private const string ModuleName = "MutliLittleFixes";
        private const int ThrottleWindowMs = 10000; // 同签名异常 10 秒内只写一次完整堆栈
        private const int MaxDetailedEntries = 5000; // 单次会话详细条目上限
        private const int MaxThrottleKeys = 20000; // 限流字典上限，防止内存无限膨胀

        private static readonly object Sync = new object();
        private static readonly Dictionary<string, ThrottleState> ThrottleMap = new Dictionary<string, ThrottleState>();
        private static int _detailedCount;
        private static bool _capacityReached;
        private static string _logPath;

        private sealed class ThrottleState
        {
            public DateTime FirstSeenUtc;
            public int Count;
        }

        /// <summary>日志文件完整路径（首次访问时解析，游戏标准用户目录）。</summary>
        public static string LogPath
        {
            get
            {
                lock (Sync)
                {
                    if (_logPath == null)
                    {
                        _logPath = ResolveLogPath();
                    }
                    return _logPath;
                }
            }
        }

        // ── 事件回调（由 SubModule 挂载/卸载） ────────────────────────

        public static void OnFirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            try
            {
                Exception ex = e.Exception;
                if (ex != null && IsModRelated(ex))
                {
                    Write("FirstChance", ex);
                }
            }
            catch
            {
                // 捕获层自身绝不外抛，避免递归
            }
        }

        public static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Write("Unhandled", e.ExceptionObject as Exception);
            }
            catch
            {
            }
        }

        public static void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                Write("TaskUnobserved", e.Exception);
                e.SetObserved(); // 已处理，防止后续被再次视为未观察
            }
            catch
            {
            }
        }

        /// <summary>会话开始标记（SubModule.OnSubModuleLoad 挂载钩子后调用）。</summary>
        public static void LogSessionStart()
        {
            try
            {
                AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}][Session] ===== 游戏启动，崩溃捕获已启用 ===== 日志文件: {LogPath}");
            }
            catch
            {
            }
        }

        // ── 核心写入 ───────────────────────────────────────────────

        /// <summary>记录一次异常（自动限流，线程安全）。任何代码可调用。</summary>
        public static void Write(string source, Exception ex)
        {
            if (ex == null)
            {
                return;
            }

            string signature = BuildSignature(ex);
            string content = BuildMessage(source, ex);

            lock (Sync)
            {
                if (_capacityReached)
                {
                    return;
                }

                bool shouldWrite = false;
                if (!ThrottleMap.TryGetValue(signature, out ThrottleState state))
                {
                    state = new ThrottleState { FirstSeenUtc = DateTime.UtcNow, Count = 1 };
                    ThrottleMap[signature] = state;
                    shouldWrite = true;
                }
                else
                {
                    state.Count++;
                    double elapsedMs = (DateTime.UtcNow - state.FirstSeenUtc).TotalMilliseconds;
                    if (elapsedMs >= ThrottleWindowMs)
                    {
                        state.FirstSeenUtc = DateTime.UtcNow; // 重置窗口，再次写完整堆栈
                        shouldWrite = true;
                    }
                }

                if (!shouldWrite)
                {
                    return; // 冷却期内重复异常：只计数
                }

                if (_detailedCount >= MaxDetailedEntries)
                {
                    _capacityReached = true;
                    AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}][Session] 详细条目已达上限({MaxDetailedEntries})，后续异常仅计数不再落盘。");
                    return;
                }

                _detailedCount++;
                string header = state.Count > 1
                    ? $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}][{source}][x{state.Count}]"
                    : $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}][{source}]";
                AppendLine(header + content);
            }
        }

        // ── 内部辅助 ───────────────────────────────────────────────

        /// <summary>判断异常是否"发生在本 Mod 中"：堆栈含本 Mod 帧，或抛出点属于本 Mod 程序集。</summary>
        private static bool IsModRelated(Exception ex)
        {
            // 抛出点直接在本 Mod 程序集内（最廉价判断）
            var site = ex.TargetSite?.DeclaringType?.Assembly;
            if (site != null && site == typeof(CrashLog).Assembly)
            {
                return true;
            }

            // 堆栈含本 Mod 帧（补丁方法调用游戏代码时游戏代码抛出的异常，帧上仍有本 Mod 调用链）
            string stack = ex.StackTrace;
            if (stack != null && (stack.Contains(ModuleName) || stack.Contains("HarmonyLib")))
            {
                return true;
            }

            return false;
        }

        /// <summary>异常签名 = 类型 + 抛出位置，用于限流分组。</summary>
        private static string BuildSignature(Exception ex)
        {
            var site = ex.TargetSite;
            string where = site != null ? $"{site.DeclaringType?.FullName}.{site.Name}" : "unknown";
            return ex.GetType().FullName + " @ " + where;
        }

        /// <summary>格式化异常消息（含内部异常链，最多 5 层）。</summary>
        private static string BuildMessage(string source, Exception ex)
        {
            var sb = new StringBuilder();
            sb.Append(' ').Append(ex.GetType().FullName).Append(": ").Append(ex.Message);
            sb.Append("  抛出位置: ").Append(BuildSignature(ex));
            sb.AppendLine();
            sb.Append(ex.StackTrace ?? "(无堆栈)");

            Exception inner = ex.InnerException;
            int depth = 0;
            while (inner != null && depth < 5)
            {
                depth++;
                sb.AppendLine();
                sb.Append($"  └─ Inner[{depth}] ").Append(inner.GetType().FullName).Append(": ").Append(inner.Message);
                sb.AppendLine();
                sb.Append(inner.StackTrace ?? "(无堆栈)");
                inner = inner.InnerException;
            }

            return sb.ToString();
        }

        /// <summary>解析日志路径：游戏标准用户目录（同 rgl_log.txt / Configs 所在目录）下的 Logs 子目录。</summary>
        private static string ResolveLogPath()
        {
            try
            {
                string docs = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                return Path.Combine(docs, "Mount and Blade II Bannerlord", "Logs", "MutliLittleFixes_Crash.log");
            }
            catch
            {
                // 兜底：退回游戏安装目录
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MutliLittleFixes_Crash.log");
            }
        }

        /// <summary>追加写日志（UTF-8 无 BOM），自身静默兜底。</summary>
        private static void AppendLine(string line)
        {
            try
            {
                string dir = Path.GetDirectoryName(LogPath);
                if (!string.IsNullOrEmpty(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                File.AppendAllText(LogPath, line + Environment.NewLine, new UTF8Encoding(false));
            }
            catch
            {
                // 磁盘故障 / 权限问题：静默，绝不影响游戏
            }
        }
    }
}