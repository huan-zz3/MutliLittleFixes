using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using MonoMod.Utils;

namespace MonoMod.Logs
{
	// Token: 0x02000818 RID: 2072
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class DebugLog
	{
		// Token: 0x1700080B RID: 2059
		// (get) Token: 0x060027B5 RID: 10165 RVA: 0x00089161 File Offset: 0x00087361
		public static bool IsFinalizing
		{
			get
			{
				return Environment.HasShutdownStarted || AppDomain.CurrentDomain.IsFinalizingForUnload();
			}
		}

		// Token: 0x060027B6 RID: 10166 RVA: 0x00089178 File Offset: 0x00087378
		private DebugLog.LogMessage MakeMessage(string source, DateTime time, LogLevel level, string formatted, [Nullable(0)] ReadOnlyMemory<MessageHole> holes)
		{
			try
			{
				if (this.replayQueue == null && !DebugLog.IsFinalizing)
				{
					WeakReference<DebugLog.LogMessage> weakReference;
					while (DebugLog.messageObjectCache.TryTake(out weakReference))
					{
						DebugLog.LogMessage logMessage;
						if (weakReference.TryGetTarget(out logMessage))
						{
							logMessage.Init(source, time, level, formatted, holes);
							DebugLog.weakRefCache.Add(weakReference);
							return logMessage;
						}
						DebugLog.weakRefCache.Add(weakReference);
					}
				}
			}
			catch
			{
			}
			return new DebugLog.LogMessage(source, time, level, formatted, holes);
		}

		// Token: 0x060027B7 RID: 10167 RVA: 0x000891F8 File Offset: 0x000873F8
		private void ReturnMessage(DebugLog.LogMessage message)
		{
			message.Clear();
			try
			{
				if (this.replayQueue == null && !DebugLog.IsFinalizing)
				{
					WeakReference<DebugLog.LogMessage> weakReference;
					if (DebugLog.weakRefCache.TryTake(out weakReference))
					{
						weakReference.SetTarget(message);
						DebugLog.messageObjectCache.Add(weakReference);
					}
					else
					{
						DebugLog.messageObjectCache.Add(new WeakReference<DebugLog.LogMessage>(message));
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x1700080C RID: 2060
		// (get) Token: 0x060027B8 RID: 10168 RVA: 0x00089264 File Offset: 0x00087464
		public static bool IsWritingLog
		{
			get
			{
				return DebugLog.Instance.ShouldLog;
			}
		}

		// Token: 0x1700080D RID: 2061
		// (get) Token: 0x060027B9 RID: 10169 RVA: 0x00089270 File Offset: 0x00087470
		internal bool AlwaysLog
		{
			get
			{
				return this.replayQueue != null || Debugger.IsAttached;
			}
		}

		// Token: 0x1700080E RID: 2062
		// (get) Token: 0x060027BA RID: 10170 RVA: 0x00089281 File Offset: 0x00087481
		internal bool ShouldLog
		{
			get
			{
				return this.subscriptions.ActiveLevels != LogLevelFilter.None || this.AlwaysLog;
			}
		}

		// Token: 0x1700080F RID: 2063
		// (get) Token: 0x060027BB RID: 10171 RVA: 0x00089298 File Offset: 0x00087498
		internal bool RecordHoles
		{
			get
			{
				return this.recordHoles || this.subscriptions.DetailLevels != LogLevelFilter.None;
			}
		}

		// Token: 0x060027BC RID: 10172 RVA: 0x000892B8 File Offset: 0x000874B8
		private void PostMessage(DebugLog.LogMessage message)
		{
			if (Debugger.IsAttached)
			{
				try
				{
					int level = (int)message.Level;
					string source = message.Source;
					FormatInterpolatedStringHandler formatInterpolatedStringHandler = new FormatInterpolatedStringHandler(6, 3);
					formatInterpolatedStringHandler.AppendLiteral("[");
					formatInterpolatedStringHandler.AppendFormatted(message.Source);
					formatInterpolatedStringHandler.AppendLiteral("] ");
					formatInterpolatedStringHandler.AppendFormatted(message.Level.FastToString(null));
					formatInterpolatedStringHandler.AppendLiteral(": ");
					formatInterpolatedStringHandler.AppendFormatted(message.FormattedMessage);
					formatInterpolatedStringHandler.AppendLiteral("\n");
					Debugger.Log(level, source, DebugFormatter.Format(ref formatInterpolatedStringHandler));
				}
				catch
				{
				}
			}
			try
			{
				DebugLog.LevelSubscriptions levelSubscriptions = this.subscriptions;
				int level2 = (int)message.Level;
				DebugLog.OnLogMessage onLogMessage = levelSubscriptions.SimpleRegs[level2];
				if (onLogMessage != null)
				{
					message.ReportTo(onLogMessage);
				}
				DebugLog.OnLogMessageDetailed onLogMessageDetailed = levelSubscriptions.DetailedRegs[level2];
				if (onLogMessageDetailed != null)
				{
					message.ReportTo(onLogMessageDetailed);
				}
				if (!DebugLog.IsFinalizing)
				{
					ConcurrentQueue<DebugLog.LogMessage> concurrentQueue = this.replayQueue;
					if (concurrentQueue != null)
					{
						concurrentQueue.Enqueue(message);
						while (concurrentQueue.Count > this.replayQueueLength)
						{
							DebugLog.LogMessage logMessage;
							if (!concurrentQueue.TryDequeue(out logMessage))
							{
								break;
							}
						}
					}
					else
					{
						this.ReturnMessage(message);
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x060027BD RID: 10173 RVA: 0x000893E4 File Offset: 0x000875E4
		internal bool ShouldLogLevel(LogLevel level)
		{
			return ((1 << (int)level) & (int)this.subscriptions.ActiveLevels) != 0 || (((1 << (int)level) & (int)this.globalFilter) != 0 && this.AlwaysLog);
		}

		// Token: 0x060027BE RID: 10174 RVA: 0x00089413 File Offset: 0x00087613
		internal bool ShouldLevelRecordHoles(LogLevel level)
		{
			return this.recordHoles || ((1 << (int)level) & (int)this.subscriptions.DetailLevels) != 0;
		}

		// Token: 0x060027BF RID: 10175 RVA: 0x00089438 File Offset: 0x00087638
		public void Write(string source, DateTime time, LogLevel level, string message)
		{
			if (!this.ShouldLogLevel(level))
			{
				return;
			}
			this.PostMessage(this.MakeMessage(source, time, level, message, default(ReadOnlyMemory<MessageHole>)));
		}

		// Token: 0x060027C0 RID: 10176 RVA: 0x0008946C File Offset: 0x0008766C
		public void Write(string source, DateTime time, LogLevel level, [InterpolatedStringHandlerArgument("level")] ref DebugLogInterpolatedStringHandler message)
		{
			if (!message.enabled)
			{
				return;
			}
			if (!this.ShouldLogLevel(level))
			{
				return;
			}
			ReadOnlyMemory<MessageHole> readOnlyMemory;
			string text = message.ToStringAndClear(out readOnlyMemory);
			this.PostMessage(this.MakeMessage(source, time, level, text, readOnlyMemory));
		}

		// Token: 0x060027C1 RID: 10177 RVA: 0x000894A8 File Offset: 0x000876A8
		internal void LogCore(string source, LogLevel level, string message)
		{
			if (!this.ShouldLogLevel(level))
			{
				return;
			}
			this.Write(source, DateTime.UtcNow, level, message);
		}

		// Token: 0x060027C2 RID: 10178 RVA: 0x000894C2 File Offset: 0x000876C2
		internal void LogCore(string source, LogLevel level, [InterpolatedStringHandlerArgument("level")] ref DebugLogInterpolatedStringHandler message)
		{
			if (!message.enabled)
			{
				return;
			}
			if (!this.ShouldLogLevel(level))
			{
				return;
			}
			this.Write(source, DateTime.UtcNow, level, ref message);
		}

		// Token: 0x060027C3 RID: 10179 RVA: 0x000894E8 File Offset: 0x000876E8
		public static void Log(string source, LogLevel level, string message)
		{
			DebugLog instance = DebugLog.Instance;
			if (!instance.ShouldLogLevel(level))
			{
				return;
			}
			instance.Write(source, DateTime.UtcNow, level, message);
		}

		// Token: 0x060027C4 RID: 10180 RVA: 0x00089514 File Offset: 0x00087714
		public static void Log(string source, LogLevel level, [InterpolatedStringHandlerArgument("level")] ref DebugLogInterpolatedStringHandler message)
		{
			DebugLog instance = DebugLog.Instance;
			if (!message.enabled)
			{
				return;
			}
			if (!instance.ShouldLogLevel(level))
			{
				return;
			}
			instance.Write(source, DateTime.UtcNow, level, ref message);
		}

		// Token: 0x060027C5 RID: 10181 RVA: 0x00089548 File Offset: 0x00087748
		[return: Nullable(new byte[] { 2, 1 })]
		private static string[] GetListEnvVar(string text)
		{
			string text2 = text.Trim();
			if (string.IsNullOrEmpty(text2))
			{
				return null;
			}
			string[] array = text2.Split(DebugLog.listEnvSeparator, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].Trim();
			}
			return array;
		}

		// Token: 0x060027C6 RID: 10182 RVA: 0x00089590 File Offset: 0x00087790
		private DebugLog()
		{
			bool flag;
			this.recordHoles = Switches.TryGetSwitchEnabled("LogRecordHoles", out flag) && flag;
			this.replayQueueLength = 0;
			object obj;
			if (Switches.TryGetSwitchValue("LogReplayQueueLength", out obj))
			{
				this.replayQueueLength = (obj as int?).GetValueOrDefault();
			}
			if (Switches.TryGetSwitchEnabled("LogSpam", out flag) && flag)
			{
				this.globalFilter |= LogLevelFilter.Spam;
			}
			if (this.replayQueueLength > 0)
			{
				this.replayQueue = new ConcurrentQueue<DebugLog.LogMessage>();
			}
			string text = (Switches.TryGetSwitchValue("LogToFile", out obj) ? (obj as string) : null);
			string[] array = null;
			if (Switches.TryGetSwitchValue("LogToFileFilter", out obj))
			{
				string[] array2 = obj as string[];
				string[] array3;
				if (array2 == null)
				{
					string text2 = obj as string;
					if (text2 == null)
					{
						array3 = null;
					}
					else
					{
						array3 = DebugLog.GetListEnvVar(text2);
					}
				}
				else
				{
					array3 = array2;
				}
				array = array3;
			}
			if (text != null)
			{
				this.TryInitializeLogToFile(text, array, this.globalFilter);
			}
			if (Switches.TryGetSwitchEnabled("LogInMemory", out flag) && flag)
			{
				this.TryInitializeMemoryLog(this.globalFilter);
			}
		}

		// Token: 0x060027C7 RID: 10183 RVA: 0x000896B4 File Offset: 0x000878B4
		private void TryInitializeLogToFile(string file, [Nullable(new byte[] { 2, 1 })] string[] sourceFilter, LogLevelFilter filter)
		{
			try
			{
				StringComparer comparer = StringComparerEx.FromComparison(StringComparison.OrdinalIgnoreCase);
				if (sourceFilter != null)
				{
					Array.Sort<string>(sourceFilter, comparer);
				}
				object sync = new object();
				TextWriter writer;
				if (file == "-")
				{
					writer = Console.Out;
				}
				else
				{
					FileStream fileStream = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.Write);
					writer = new StreamWriter(fileStream, Encoding.UTF8)
					{
						AutoFlush = true
					};
				}
				this.SubscribeCore(filter, delegate(string source, DateTime time, LogLevel level, string msg)
				{
					if (sourceFilter != null && sourceFilter.AsSpan<string>().BinarySearch(source, comparer) < 0)
					{
						return;
					}
					DateTime dateTime = time.ToLocalTime();
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 4);
					defaultInterpolatedStringHandler.AppendLiteral("[");
					defaultInterpolatedStringHandler.AppendFormatted(source);
					defaultInterpolatedStringHandler.AppendLiteral("](");
					defaultInterpolatedStringHandler.AppendFormatted<DateTime>(dateTime);
					defaultInterpolatedStringHandler.AppendLiteral(") ");
					defaultInterpolatedStringHandler.AppendFormatted(level.FastToString(null));
					defaultInterpolatedStringHandler.AppendLiteral(": ");
					defaultInterpolatedStringHandler.AppendFormatted(msg);
					string text = defaultInterpolatedStringHandler.ToStringAndClear();
					object sync2 = sync;
					lock (sync2)
					{
						writer.WriteLine(text);
					}
				});
			}
			catch (Exception ex)
			{
				LogLevel logLevel = LogLevel.Error;
				LogLevel logLevel2 = logLevel;
				bool flag;
				DebugLogInterpolatedStringHandler debugLogInterpolatedStringHandler = new DebugLogInterpolatedStringHandler(61, 1, logLevel, out flag);
				if (flag)
				{
					debugLogInterpolatedStringHandler.AppendLiteral("Exception while trying to initialize writing logs to a file: ");
					debugLogInterpolatedStringHandler.AppendFormatted<Exception>(ex);
				}
				DebugLog.Instance.LogCore("DebugLog", logLevel2, ref debugLogInterpolatedStringHandler);
			}
		}

		// Token: 0x060027C8 RID: 10184 RVA: 0x000897A0 File Offset: 0x000879A0
		private void TryInitializeMemoryLog(LogLevelFilter filter)
		{
			try
			{
				DebugLog.memlogPos = 0;
				DebugLog.memlog = new byte[4096];
				object sync = new object();
				Encoding utf = Encoding.UTF8;
				this.SubscribeCore(filter, delegate(string source, DateTime time, LogLevel level, string msg)
				{
					byte b = (byte)level;
					long ticks = time.Ticks;
					if (source.Length > 255)
					{
						source = source.Substring(0, 255);
					}
					byte b2 = (byte)source.Length;
					int length = msg.Length;
					int num = (int)(14 + b2 * 2) + length * 2;
					object sync2 = sync;
					lock (sync2)
					{
						if (DebugLog.memlog.Length - DebugLog.memlogPos < num)
						{
							int num2 = DebugLog.memlog.Length * 4;
							while (num2 - DebugLog.memlogPos < num)
							{
								num2 *= 4;
							}
							Array.Resize<byte>(ref DebugLog.memlog, num2);
						}
						ref byte reference = ref MemoryMarshal.GetReference<byte>(DebugLog.memlog.AsSpan<byte>().Slice(DebugLog.memlogPos));
						int num3 = 0;
						Unsafe.WriteUnaligned<byte>(Unsafe.Add<byte>(ref reference, num3), b);
						num3++;
						Unsafe.WriteUnaligned<long>(Unsafe.Add<byte>(ref reference, num3), ticks);
						num3 += 8;
						Unsafe.WriteUnaligned<byte>(Unsafe.Add<byte>(ref reference, num3), b2);
						num3++;
						Unsafe.CopyBlock(Unsafe.Add<byte>(ref reference, num3), Unsafe.As<char, byte>(MemoryMarshal.GetReference<char>(source.AsSpan())), (uint)(b2 * 2));
						num3 += (int)(b2 * 2);
						Unsafe.WriteUnaligned<int>(Unsafe.Add<byte>(ref reference, num3), length);
						num3 += 4;
						Unsafe.CopyBlock(Unsafe.Add<byte>(ref reference, num3), Unsafe.As<char, byte>(MemoryMarshal.GetReference<char>(msg.AsSpan())), (uint)(length * 2));
						num3 += length * 2;
						DebugLog.memlogPos += num3;
					}
				});
			}
			catch (Exception ex)
			{
				LogLevel logLevel = LogLevel.Error;
				LogLevel logLevel2 = logLevel;
				bool flag;
				DebugLogInterpolatedStringHandler debugLogInterpolatedStringHandler = new DebugLogInterpolatedStringHandler(45, 1, logLevel, out flag);
				if (flag)
				{
					debugLogInterpolatedStringHandler.AppendLiteral("Exception while initializing the memory log: ");
					debugLogInterpolatedStringHandler.AppendFormatted<Exception>(ex);
				}
				DebugLog.Instance.LogCore("DebugLog", logLevel2, ref debugLogInterpolatedStringHandler);
			}
		}

		// Token: 0x060027C9 RID: 10185 RVA: 0x00089840 File Offset: 0x00087A40
		private void MaybeReplayTo(LogLevelFilter filter, DebugLog.OnLogMessage del)
		{
			if (this.replayQueue == null || filter == LogLevelFilter.None)
			{
				return;
			}
			foreach (DebugLog.LogMessage logMessage in this.replayQueue.ToArray())
			{
				if (((1 << (int)logMessage.Level) & (int)filter) != 0)
				{
					logMessage.ReportTo(del);
				}
			}
		}

		// Token: 0x060027CA RID: 10186 RVA: 0x00089890 File Offset: 0x00087A90
		private void MaybeReplayTo(LogLevelFilter filter, DebugLog.OnLogMessageDetailed del)
		{
			if (this.replayQueue == null || filter == LogLevelFilter.None)
			{
				return;
			}
			foreach (DebugLog.LogMessage logMessage in this.replayQueue.ToArray())
			{
				if (((1 << (int)logMessage.Level) & (int)filter) != 0)
				{
					logMessage.ReportTo(del);
				}
			}
		}

		// Token: 0x060027CB RID: 10187 RVA: 0x000898DD File Offset: 0x00087ADD
		public static IDisposable Subscribe(LogLevelFilter filter, DebugLog.OnLogMessage value)
		{
			return DebugLog.Instance.SubscribeCore(filter, value);
		}

		// Token: 0x060027CC RID: 10188 RVA: 0x000898EC File Offset: 0x00087AEC
		private IDisposable SubscribeCore(LogLevelFilter filter, DebugLog.OnLogMessage value)
		{
			DebugLog.LevelSubscriptions levelSubscriptions;
			DebugLog.LevelSubscriptions levelSubscriptions2;
			do
			{
				levelSubscriptions = this.subscriptions;
				levelSubscriptions2 = levelSubscriptions.AddSimple(filter, value);
			}
			while (Interlocked.CompareExchange<DebugLog.LevelSubscriptions>(ref this.subscriptions, levelSubscriptions2, levelSubscriptions) != levelSubscriptions);
			this.MaybeReplayTo(filter, value);
			return new DebugLog.LogSubscriptionSimple(this, value, filter);
		}

		// Token: 0x060027CD RID: 10189 RVA: 0x00089929 File Offset: 0x00087B29
		public static IDisposable Subscribe(LogLevelFilter filter, DebugLog.OnLogMessageDetailed value)
		{
			return DebugLog.Instance.SubscribeCore(filter, value);
		}

		// Token: 0x060027CE RID: 10190 RVA: 0x00089938 File Offset: 0x00087B38
		private IDisposable SubscribeCore(LogLevelFilter filter, DebugLog.OnLogMessageDetailed value)
		{
			DebugLog.LevelSubscriptions levelSubscriptions;
			DebugLog.LevelSubscriptions levelSubscriptions2;
			do
			{
				levelSubscriptions = this.subscriptions;
				levelSubscriptions2 = levelSubscriptions.AddDetailed(filter, value);
			}
			while (Interlocked.CompareExchange<DebugLog.LevelSubscriptions>(ref this.subscriptions, levelSubscriptions2, levelSubscriptions) != levelSubscriptions);
			this.MaybeReplayTo(filter, value);
			return new DebugLog.LogSubscriptionDetailed(this, value, filter);
		}

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x060027CF RID: 10191 RVA: 0x00089978 File Offset: 0x00087B78
		// (remove) Token: 0x060027D0 RID: 10192 RVA: 0x000899C0 File Offset: 0x00087BC0
		public static event DebugLog.OnLogMessage OnLog
		{
			add
			{
				IDisposable res = DebugLog.Subscribe(DebugLog.Instance.globalFilter, value);
				DebugLog.simpleRegDict.AddOrUpdate(value, res, delegate(DebugLog.OnLogMessage _, IDisposable d)
				{
					d.Dispose();
					return res;
				});
			}
			remove
			{
				IDisposable disposable;
				if (DebugLog.simpleRegDict.TryRemove(value, out disposable))
				{
					disposable.Dispose();
				}
			}
		}

		// Token: 0x040039EB RID: 14827
		internal static readonly DebugLog Instance = new DebugLog();

		// Token: 0x040039EC RID: 14828
		private static readonly ConcurrentBag<WeakReference<DebugLog.LogMessage>> weakRefCache = new ConcurrentBag<WeakReference<DebugLog.LogMessage>>();

		// Token: 0x040039ED RID: 14829
		private static readonly ConcurrentBag<WeakReference<DebugLog.LogMessage>> messageObjectCache = new ConcurrentBag<WeakReference<DebugLog.LogMessage>>();

		// Token: 0x040039EE RID: 14830
		private static readonly char[] listEnvSeparator = new char[] { ' ', ';', ',' };

		// Token: 0x040039EF RID: 14831
		private readonly bool recordHoles;

		// Token: 0x040039F0 RID: 14832
		private readonly int replayQueueLength;

		// Token: 0x040039F1 RID: 14833
		[Nullable(new byte[] { 2, 1 })]
		private readonly ConcurrentQueue<DebugLog.LogMessage> replayQueue;

		// Token: 0x040039F2 RID: 14834
		private LogLevelFilter globalFilter = LogLevelFilter.DefaultFilter;

		// Token: 0x040039F3 RID: 14835
		[Nullable(2)]
		private static byte[] memlog;

		// Token: 0x040039F4 RID: 14836
		private static int memlogPos;

		// Token: 0x040039F5 RID: 14837
		private DebugLog.LevelSubscriptions subscriptions = DebugLog.LevelSubscriptions.None;

		// Token: 0x040039F6 RID: 14838
		private static readonly ConcurrentDictionary<DebugLog.OnLogMessage, IDisposable> simpleRegDict = new ConcurrentDictionary<DebugLog.OnLogMessage, IDisposable>();

		// Token: 0x02000819 RID: 2073
		// (Invoke) Token: 0x060027D3 RID: 10195
		[NullableContext(0)]
		public delegate void OnLogMessage(string source, DateTime time, LogLevel level, string message);

		// Token: 0x0200081A RID: 2074
		// (Invoke) Token: 0x060027D7 RID: 10199
		[NullableContext(0)]
		public delegate void OnLogMessageDetailed(string source, DateTime time, LogLevel level, string formattedMessage, [Nullable(0)] ReadOnlyMemory<MessageHole> holes);

		// Token: 0x0200081B RID: 2075
		[Nullable(0)]
		private sealed class LogMessage
		{
			// Token: 0x17000810 RID: 2064
			// (get) Token: 0x060027DA RID: 10202 RVA: 0x00089A22 File Offset: 0x00087C22
			// (set) Token: 0x060027DB RID: 10203 RVA: 0x00089A2A File Offset: 0x00087C2A
			public string Source { get; private set; }

			// Token: 0x17000811 RID: 2065
			// (get) Token: 0x060027DC RID: 10204 RVA: 0x00089A33 File Offset: 0x00087C33
			// (set) Token: 0x060027DD RID: 10205 RVA: 0x00089A3B File Offset: 0x00087C3B
			public DateTime Time { get; private set; }

			// Token: 0x17000812 RID: 2066
			// (get) Token: 0x060027DE RID: 10206 RVA: 0x00089A44 File Offset: 0x00087C44
			// (set) Token: 0x060027DF RID: 10207 RVA: 0x00089A4C File Offset: 0x00087C4C
			public LogLevel Level { get; private set; }

			// Token: 0x17000813 RID: 2067
			// (get) Token: 0x060027E0 RID: 10208 RVA: 0x00089A55 File Offset: 0x00087C55
			// (set) Token: 0x060027E1 RID: 10209 RVA: 0x00089A5D File Offset: 0x00087C5D
			public string FormattedMessage { get; private set; }

			// Token: 0x17000814 RID: 2068
			// (get) Token: 0x060027E2 RID: 10210 RVA: 0x00089A66 File Offset: 0x00087C66
			// (set) Token: 0x060027E3 RID: 10211 RVA: 0x00089A6E File Offset: 0x00087C6E
			[Nullable(0)]
			public ReadOnlyMemory<MessageHole> FormatHoles
			{
				[NullableContext(0)]
				get;
				[NullableContext(0)]
				private set;
			}

			// Token: 0x060027E4 RID: 10212 RVA: 0x00089A77 File Offset: 0x00087C77
			public LogMessage(string source, DateTime time, LogLevel level, string formatted, [Nullable(0)] ReadOnlyMemory<MessageHole> holes)
			{
				this.Source = source;
				this.Time = time;
				this.Level = level;
				this.FormattedMessage = formatted;
				this.FormatHoles = holes;
			}

			// Token: 0x060027E5 RID: 10213 RVA: 0x00089AA4 File Offset: 0x00087CA4
			public void Clear()
			{
				this.Source = "";
				this.Time = default(DateTime);
				this.Level = LogLevel.Spam;
				this.FormattedMessage = "";
				this.FormatHoles = default(ReadOnlyMemory<MessageHole>);
			}

			// Token: 0x060027E6 RID: 10214 RVA: 0x00089AEC File Offset: 0x00087CEC
			public void Init(string source, DateTime time, LogLevel level, string formatted, [Nullable(0)] ReadOnlyMemory<MessageHole> holes)
			{
				this.Source = source;
				this.Time = time;
				this.Level = level;
				this.FormattedMessage = formatted;
				this.FormatHoles = holes;
			}

			// Token: 0x060027E7 RID: 10215 RVA: 0x00089B14 File Offset: 0x00087D14
			public void ReportTo(DebugLog.OnLogMessage del)
			{
				try
				{
					del(this.Source, this.Time, this.Level, this.FormattedMessage);
				}
				catch (Exception ex)
				{
					Debugger.Log(int.MaxValue, "MonoMod.DebugLog", "Exception caught while reporting to message handler");
					Debugger.Log(int.MaxValue, "MonoMod.DebugLog", ex.ToString());
				}
			}

			// Token: 0x060027E8 RID: 10216 RVA: 0x00089B80 File Offset: 0x00087D80
			public void ReportTo(DebugLog.OnLogMessageDetailed del)
			{
				try
				{
					del(this.Source, this.Time, this.Level, this.FormattedMessage, this.FormatHoles);
				}
				catch (Exception ex)
				{
					Debugger.Log(int.MaxValue, "MonoMod.DebugLog", "Exception caught while reporting to message handler");
					Debugger.Log(int.MaxValue, "MonoMod.DebugLog", ex.ToString());
				}
			}
		}

		// Token: 0x0200081C RID: 2076
		[Nullable(0)]
		private sealed class LevelSubscriptions
		{
			// Token: 0x060027E9 RID: 10217 RVA: 0x00089BF0 File Offset: 0x00087DF0
			private LevelSubscriptions(LogLevelFilter active, LogLevelFilter detail, [Nullable(new byte[] { 1, 2 })] DebugLog.OnLogMessage[] simple, [Nullable(new byte[] { 1, 2 })] DebugLog.OnLogMessageDetailed[] detailed)
			{
				this.ActiveLevels = active | detail;
				this.DetailLevels = detail;
				this.SimpleRegs = simple;
				this.DetailedRegs = detailed;
			}

			// Token: 0x060027EA RID: 10218 RVA: 0x00089C17 File Offset: 0x00087E17
			private LevelSubscriptions()
			{
				this.ActiveLevels = LogLevelFilter.None;
				this.DetailLevels = LogLevelFilter.None;
				this.SimpleRegs = new DebugLog.OnLogMessage[6];
				this.DetailedRegs = new DebugLog.OnLogMessageDetailed[this.SimpleRegs.Length];
			}

			// Token: 0x060027EB RID: 10219 RVA: 0x00089C4C File Offset: 0x00087E4C
			private DebugLog.LevelSubscriptions Clone(bool changingDetail)
			{
				DebugLog.OnLogMessage[] array = this.SimpleRegs;
				DebugLog.OnLogMessageDetailed[] array2 = this.DetailedRegs;
				if (!changingDetail)
				{
					array = new DebugLog.OnLogMessage[this.SimpleRegs.Length];
					Array.Copy(this.SimpleRegs, array, array.Length);
				}
				else
				{
					array2 = new DebugLog.OnLogMessageDetailed[this.DetailedRegs.Length];
					Array.Copy(this.DetailedRegs, array2, array2.Length);
				}
				return new DebugLog.LevelSubscriptions(this.ActiveLevels, this.DetailLevels, array, array2);
			}

			// Token: 0x060027EC RID: 10220 RVA: 0x00089CB9 File Offset: 0x00087EB9
			private void FixFilters()
			{
				this.ActiveLevels &= LogLevelFilter.Spam | LogLevelFilter.Trace | LogLevelFilter.Info | LogLevelFilter.Warning | LogLevelFilter.Error | LogLevelFilter.Assert;
				this.DetailLevels &= LogLevelFilter.Spam | LogLevelFilter.Trace | LogLevelFilter.Info | LogLevelFilter.Warning | LogLevelFilter.Error | LogLevelFilter.Assert;
			}

			// Token: 0x060027ED RID: 10221 RVA: 0x00089CDC File Offset: 0x00087EDC
			public DebugLog.LevelSubscriptions AddSimple(LogLevelFilter filter, DebugLog.OnLogMessage del)
			{
				DebugLog.LevelSubscriptions levelSubscriptions = this.Clone(false);
				levelSubscriptions.ActiveLevels |= filter;
				for (int i = 0; i < levelSubscriptions.SimpleRegs.Length; i++)
				{
					if ((filter & (LogLevelFilter)(1 << i)) != LogLevelFilter.None)
					{
						Helpers.EventAdd<DebugLog.OnLogMessage>(ref levelSubscriptions.SimpleRegs[i], del);
					}
				}
				levelSubscriptions.FixFilters();
				return levelSubscriptions;
			}

			// Token: 0x060027EE RID: 10222 RVA: 0x00089D38 File Offset: 0x00087F38
			public DebugLog.LevelSubscriptions RemoveSimple(LogLevelFilter filter, DebugLog.OnLogMessage del)
			{
				DebugLog.LevelSubscriptions levelSubscriptions = this.Clone(false);
				for (int i = 0; i < levelSubscriptions.SimpleRegs.Length; i++)
				{
					if ((filter & (LogLevelFilter)(1 << i)) != LogLevelFilter.None && Helpers.EventRemove<DebugLog.OnLogMessage>(ref levelSubscriptions.SimpleRegs[i], del) == null)
					{
						levelSubscriptions.ActiveLevels &= (LogLevelFilter)(~(LogLevelFilter)(1 << i));
					}
				}
				levelSubscriptions.ActiveLevels |= levelSubscriptions.DetailLevels;
				levelSubscriptions.FixFilters();
				return levelSubscriptions;
			}

			// Token: 0x060027EF RID: 10223 RVA: 0x00089DB0 File Offset: 0x00087FB0
			public DebugLog.LevelSubscriptions AddDetailed(LogLevelFilter filter, DebugLog.OnLogMessageDetailed del)
			{
				DebugLog.LevelSubscriptions levelSubscriptions = this.Clone(true);
				levelSubscriptions.DetailLevels |= filter;
				for (int i = 0; i < levelSubscriptions.DetailedRegs.Length; i++)
				{
					if ((filter & (LogLevelFilter)(1 << i)) != LogLevelFilter.None)
					{
						Helpers.EventAdd<DebugLog.OnLogMessageDetailed>(ref levelSubscriptions.DetailedRegs[i], del);
					}
				}
				levelSubscriptions.ActiveLevels |= levelSubscriptions.DetailLevels;
				levelSubscriptions.FixFilters();
				return levelSubscriptions;
			}

			// Token: 0x060027F0 RID: 10224 RVA: 0x00089E20 File Offset: 0x00088020
			public DebugLog.LevelSubscriptions RemoveDetailed(LogLevelFilter filter, DebugLog.OnLogMessageDetailed del)
			{
				DebugLog.LevelSubscriptions levelSubscriptions = this.Clone(true);
				for (int i = 0; i < levelSubscriptions.DetailedRegs.Length; i++)
				{
					if ((filter & (LogLevelFilter)(1 << i)) != LogLevelFilter.None && Helpers.EventRemove<DebugLog.OnLogMessageDetailed>(ref levelSubscriptions.DetailedRegs[i], del) == null)
					{
						levelSubscriptions.DetailLevels &= (LogLevelFilter)(~(LogLevelFilter)(1 << i));
					}
				}
				levelSubscriptions.ActiveLevels |= levelSubscriptions.DetailLevels;
				levelSubscriptions.FixFilters();
				return levelSubscriptions;
			}

			// Token: 0x040039FC RID: 14844
			public LogLevelFilter ActiveLevels;

			// Token: 0x040039FD RID: 14845
			public LogLevelFilter DetailLevels;

			// Token: 0x040039FE RID: 14846
			[Nullable(new byte[] { 1, 2 })]
			public readonly DebugLog.OnLogMessage[] SimpleRegs;

			// Token: 0x040039FF RID: 14847
			[Nullable(new byte[] { 1, 2 })]
			public readonly DebugLog.OnLogMessageDetailed[] DetailedRegs;

			// Token: 0x04003A00 RID: 14848
			private const LogLevelFilter ValidFilter = LogLevelFilter.Spam | LogLevelFilter.Trace | LogLevelFilter.Info | LogLevelFilter.Warning | LogLevelFilter.Error | LogLevelFilter.Assert;

			// Token: 0x04003A01 RID: 14849
			public static readonly DebugLog.LevelSubscriptions None = new DebugLog.LevelSubscriptions();
		}

		// Token: 0x0200081D RID: 2077
		[Nullable(0)]
		private sealed class LogSubscriptionSimple : IDisposable
		{
			// Token: 0x060027F2 RID: 10226 RVA: 0x00089EA2 File Offset: 0x000880A2
			public LogSubscriptionSimple(DebugLog log, DebugLog.OnLogMessage del, LogLevelFilter filter)
			{
				this.log = log;
				this.del = del;
				this.filter = filter;
			}

			// Token: 0x060027F3 RID: 10227 RVA: 0x00089EC0 File Offset: 0x000880C0
			public void Dispose()
			{
				DebugLog.LevelSubscriptions subscriptions;
				DebugLog.LevelSubscriptions levelSubscriptions;
				do
				{
					subscriptions = this.log.subscriptions;
					levelSubscriptions = subscriptions.RemoveSimple(this.filter, this.del);
				}
				while (Interlocked.CompareExchange<DebugLog.LevelSubscriptions>(ref this.log.subscriptions, levelSubscriptions, subscriptions) != subscriptions);
			}

			// Token: 0x04003A02 RID: 14850
			private readonly DebugLog log;

			// Token: 0x04003A03 RID: 14851
			private readonly DebugLog.OnLogMessage del;

			// Token: 0x04003A04 RID: 14852
			private readonly LogLevelFilter filter;
		}

		// Token: 0x0200081E RID: 2078
		[Nullable(0)]
		private sealed class LogSubscriptionDetailed : IDisposable
		{
			// Token: 0x060027F4 RID: 10228 RVA: 0x00089F01 File Offset: 0x00088101
			public LogSubscriptionDetailed(DebugLog log, DebugLog.OnLogMessageDetailed del, LogLevelFilter filter)
			{
				this.log = log;
				this.del = del;
				this.filter = filter;
			}

			// Token: 0x060027F5 RID: 10229 RVA: 0x00089F20 File Offset: 0x00088120
			public void Dispose()
			{
				DebugLog.LevelSubscriptions subscriptions;
				DebugLog.LevelSubscriptions levelSubscriptions;
				do
				{
					subscriptions = this.log.subscriptions;
					levelSubscriptions = subscriptions.RemoveDetailed(this.filter, this.del);
				}
				while (Interlocked.CompareExchange<DebugLog.LevelSubscriptions>(ref this.log.subscriptions, levelSubscriptions, subscriptions) != subscriptions);
			}

			// Token: 0x04003A05 RID: 14853
			private readonly DebugLog log;

			// Token: 0x04003A06 RID: 14854
			private readonly DebugLog.OnLogMessageDetailed del;

			// Token: 0x04003A07 RID: 14855
			private readonly LogLevelFilter filter;
		}
	}
}
