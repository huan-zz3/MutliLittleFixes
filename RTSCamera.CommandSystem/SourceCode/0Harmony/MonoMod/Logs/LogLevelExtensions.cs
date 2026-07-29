using System;
using System.Runtime.CompilerServices;

namespace MonoMod.Logs
{
	// Token: 0x02000827 RID: 2087
	internal static class LogLevelExtensions
	{
		// Token: 0x0600282F RID: 10287 RVA: 0x0008B3B0 File Offset: 0x000895B0
		[NullableContext(1)]
		public static string FastToString(this LogLevel level, [Nullable(2)] IFormatProvider provider = null)
		{
			string text;
			switch (level)
			{
			case LogLevel.Spam:
				text = "Spam";
				break;
			case LogLevel.Trace:
				text = "Trace";
				break;
			case LogLevel.Info:
				text = "Info";
				break;
			case LogLevel.Warning:
				text = "Warning";
				break;
			case LogLevel.Error:
				text = "Error";
				break;
			case LogLevel.Assert:
				text = "Assert";
				break;
			default:
			{
				int num = (int)level;
				text = num.ToString(provider);
				break;
			}
			}
			return text;
		}

		// Token: 0x04003A2A RID: 14890
		public const LogLevel MaxLevel = LogLevel.Assert;
	}
}
