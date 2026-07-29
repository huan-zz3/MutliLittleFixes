using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Mono.Cecil.Cil;

namespace HarmonyLib
{
	// Token: 0x020001C5 RID: 453
	public static class FileLog
	{
		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x060007E4 RID: 2020 RVA: 0x00019F34 File Offset: 0x00018134
		// (set) Token: 0x060007E5 RID: 2021 RVA: 0x00019F3B File Offset: 0x0001813B
		public static StreamWriter LogWriter { get; set; }

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x060007E6 RID: 2022 RVA: 0x00019F44 File Offset: 0x00018144
		public static string LogPath
		{
			get
			{
				object obj = FileLog.fileLock;
				string logPath;
				lock (obj)
				{
					if (!FileLog._logPathInited)
					{
						FileLog._logPathInited = true;
						string environmentVariable = Environment.GetEnvironmentVariable("HARMONY_NO_LOG");
						if (!string.IsNullOrEmpty(environmentVariable))
						{
							return null;
						}
						FileLog._logPath = Environment.GetEnvironmentVariable("HARMONY_LOG_FILE");
						if (string.IsNullOrEmpty(FileLog._logPath))
						{
							string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
							Directory.CreateDirectory(folderPath);
							FileLog._logPath = Path.Combine(folderPath, "harmony.log.txt");
						}
					}
					logPath = FileLog._logPath;
				}
				return logPath;
			}
		}

		// Token: 0x060007E7 RID: 2023 RVA: 0x00019FE4 File Offset: 0x000181E4
		private static string IndentString()
		{
			return new string(FileLog.indentChar, FileLog.indentLevel);
		}

		// Token: 0x060007E8 RID: 2024 RVA: 0x00003C93 File Offset: 0x00001E93
		private static string CodePos(int offset)
		{
			return string.Format("IL_{0:X4}: ", offset);
		}

		// Token: 0x060007E9 RID: 2025 RVA: 0x00019FF8 File Offset: 0x000181F8
		public static void ChangeIndent(int delta)
		{
			object obj = FileLog.fileLock;
			lock (obj)
			{
				FileLog.indentLevel = Math.Max(0, FileLog.indentLevel + delta);
			}
		}

		// Token: 0x060007EA RID: 2026 RVA: 0x0001A044 File Offset: 0x00018244
		public static void LogBuffered(string str)
		{
			object obj = FileLog.fileLock;
			lock (obj)
			{
				FileLog.buffer.Add(FileLog.IndentString() + str);
			}
		}

		// Token: 0x060007EB RID: 2027 RVA: 0x0001A094 File Offset: 0x00018294
		public static void LogBuffered(List<string> strings)
		{
			object obj = FileLog.fileLock;
			lock (obj)
			{
				FileLog.buffer.AddRange(strings);
			}
		}

		// Token: 0x060007EC RID: 2028 RVA: 0x0001A0D8 File Offset: 0x000182D8
		public static List<string> GetBuffer(bool clear)
		{
			object obj = FileLog.fileLock;
			List<string> list2;
			lock (obj)
			{
				List<string> list = FileLog.buffer;
				if (clear)
				{
					FileLog.buffer = new List<string>();
				}
				list2 = list;
			}
			return list2;
		}

		// Token: 0x060007ED RID: 2029 RVA: 0x0001A128 File Offset: 0x00018328
		public static void SetBuffer(List<string> buffer)
		{
			object obj = FileLog.fileLock;
			lock (obj)
			{
				FileLog.buffer = buffer;
			}
		}

		// Token: 0x060007EE RID: 2030 RVA: 0x0001A168 File Offset: 0x00018368
		public static void FlushBuffer()
		{
			object obj = FileLog.fileLock;
			lock (obj)
			{
				if (FileLog.LogWriter != null)
				{
					foreach (string text in FileLog.buffer)
					{
						FileLog.LogWriter.WriteLine(text);
					}
					FileLog.buffer.Clear();
				}
				else if (FileLog.LogPath != null)
				{
					if (FileLog.buffer.Count > 0)
					{
						using (FileStream fileStream = new FileStream(FileLog.LogPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
						{
							using (StreamWriter streamWriter = new StreamWriter(fileStream))
							{
								foreach (string text2 in FileLog.buffer)
								{
									streamWriter.WriteLine(text2);
								}
								FileLog.buffer.Clear();
							}
						}
					}
				}
			}
		}

		// Token: 0x060007EF RID: 2031 RVA: 0x0001A2B0 File Offset: 0x000184B0
		public static void Log(string str)
		{
			object obj = FileLog.fileLock;
			lock (obj)
			{
				if (FileLog.LogWriter != null)
				{
					FileLog.LogWriter.WriteLine(FileLog.IndentString() + str);
				}
				else if (FileLog.LogPath != null)
				{
					using (FileStream fileStream = new FileStream(FileLog.LogPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
					{
						using (StreamWriter streamWriter = new StreamWriter(fileStream))
						{
							streamWriter.WriteLine(FileLog.IndentString() + str);
						}
					}
				}
			}
		}

		// Token: 0x060007F0 RID: 2032 RVA: 0x0001A364 File Offset: 0x00018564
		public static void LogILComment(int codePos, string comment)
		{
			FileLog.LogBuffered(string.Format("{0}// {1}", FileLog.CodePos(codePos), comment));
		}

		// Token: 0x060007F1 RID: 2033 RVA: 0x0001A37C File Offset: 0x0001857C
		public static void LogIL(int codePos, global::System.Reflection.Emit.OpCode opcode)
		{
			FileLog.LogBuffered(string.Format("{0}{1}", FileLog.CodePos(codePos), opcode));
		}

		// Token: 0x060007F2 RID: 2034 RVA: 0x0001A39C File Offset: 0x0001859C
		public static void LogIL(int codePos, global::System.Reflection.Emit.OpCode opcode, object arg)
		{
			string text = Emitter.FormatOperand(arg);
			string text2 = ((text.Length > 0) ? " " : "");
			string text3 = opcode.ToString();
			if (opcode.FlowControl == global::System.Reflection.Emit.FlowControl.Branch || opcode.FlowControl == global::System.Reflection.Emit.FlowControl.Cond_Branch)
			{
				text3 += " =>";
			}
			text3 = text3.PadRight(10);
			FileLog.LogBuffered(string.Format("{0}{1}{2}{3}", new object[]
			{
				FileLog.CodePos(codePos),
				text3,
				text2,
				text
			}));
		}

		// Token: 0x060007F3 RID: 2035 RVA: 0x0001A428 File Offset: 0x00018628
		internal static void LogIL(VariableDefinition variable)
		{
			FileLog.LogBuffered(string.Format("{0}Local var {1}: {2}{3}", new object[]
			{
				FileLog.CodePos(0),
				variable.Index,
				variable.VariableType.FullName,
				variable.IsPinned ? "(pinned)" : ""
			}));
		}

		// Token: 0x060007F4 RID: 2036 RVA: 0x0001A486 File Offset: 0x00018686
		public static void LogIL(int codePos, Label label)
		{
			FileLog.LogBuffered(FileLog.CodePos(codePos) + Emitter.FormatOperand(label));
		}

		// Token: 0x060007F5 RID: 2037 RVA: 0x0001A4A4 File Offset: 0x000186A4
		public static void LogILBlockBegin(int codePos, ExceptionBlock block)
		{
			switch (block.blockType)
			{
			case ExceptionBlockType.BeginExceptionBlock:
				FileLog.LogBuffered(".try");
				FileLog.LogBuffered("{");
				FileLog.ChangeIndent(1);
				return;
			case ExceptionBlockType.BeginCatchBlock:
			{
				FileLog.LogIL(codePos, global::System.Reflection.Emit.OpCodes.Leave, new LeaveTry());
				FileLog.ChangeIndent(-1);
				FileLog.LogBuffered("} // end try");
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
				defaultInterpolatedStringHandler.AppendLiteral(".catch ");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(block.catchType);
				FileLog.LogBuffered(defaultInterpolatedStringHandler.ToStringAndClear());
				FileLog.LogBuffered("{");
				FileLog.ChangeIndent(1);
				return;
			}
			case ExceptionBlockType.BeginExceptFilterBlock:
				FileLog.LogIL(codePos, global::System.Reflection.Emit.OpCodes.Leave, new LeaveTry());
				FileLog.ChangeIndent(-1);
				FileLog.LogBuffered("} // end try");
				FileLog.LogBuffered(".filter");
				FileLog.LogBuffered("{");
				FileLog.ChangeIndent(1);
				return;
			case ExceptionBlockType.BeginFaultBlock:
				FileLog.LogIL(codePos, global::System.Reflection.Emit.OpCodes.Leave, new LeaveTry());
				FileLog.ChangeIndent(-1);
				FileLog.LogBuffered("} // end try");
				FileLog.LogBuffered(".fault");
				FileLog.LogBuffered("{");
				FileLog.ChangeIndent(1);
				return;
			case ExceptionBlockType.BeginFinallyBlock:
				FileLog.LogIL(codePos, global::System.Reflection.Emit.OpCodes.Leave, new LeaveTry());
				FileLog.ChangeIndent(-1);
				FileLog.LogBuffered("} // end try");
				FileLog.LogBuffered(".finally");
				FileLog.LogBuffered("{");
				FileLog.ChangeIndent(1);
				return;
			default:
				return;
			}
		}

		// Token: 0x060007F6 RID: 2038 RVA: 0x0001A600 File Offset: 0x00018800
		public static void LogILBlockEnd(int codePos, ExceptionBlock block)
		{
			ExceptionBlockType blockType = block.blockType;
			if (blockType == ExceptionBlockType.EndExceptionBlock)
			{
				FileLog.LogIL(codePos, global::System.Reflection.Emit.OpCodes.Leave, new LeaveTry());
				FileLog.ChangeIndent(-1);
				FileLog.LogBuffered("} // end handler");
			}
		}

		// Token: 0x060007F7 RID: 2039 RVA: 0x0001A638 File Offset: 0x00018838
		public static void Debug(string str)
		{
			if (Harmony.DEBUG)
			{
				FileLog.Log(str);
			}
		}

		// Token: 0x060007F8 RID: 2040 RVA: 0x0001A648 File Offset: 0x00018848
		public static void Reset()
		{
			object obj = FileLog.fileLock;
			lock (obj)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(15, 2);
				defaultInterpolatedStringHandler.AppendFormatted(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
				defaultInterpolatedStringHandler.AppendFormatted<char>(Path.DirectorySeparatorChar);
				defaultInterpolatedStringHandler.AppendLiteral("harmony.log.txt");
				string text = defaultInterpolatedStringHandler.ToStringAndClear();
				File.Delete(text);
			}
		}

		// Token: 0x060007F9 RID: 2041 RVA: 0x0001A6C0 File Offset: 0x000188C0
		public unsafe static void LogBytes(long ptr, int len)
		{
			object obj = FileLog.fileLock;
			lock (obj)
			{
				byte* ptr2 = ptr;
				string text = "";
				for (int i = 1; i <= len; i++)
				{
					if (text.Length == 0)
					{
						text = "#  ";
					}
					string text2 = text;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 1);
					defaultInterpolatedStringHandler.AppendFormatted<byte>(*ptr2, "X2");
					defaultInterpolatedStringHandler.AppendLiteral(" ");
					text = text2 + defaultInterpolatedStringHandler.ToStringAndClear();
					if (i > 1 || len == 1)
					{
						if (i % 8 == 0 || i == len)
						{
							FileLog.Log(text);
							text = "";
						}
						else if (i % 4 == 0)
						{
							text += " ";
						}
					}
					ptr2++;
				}
				byte[] array = new byte[len];
				Marshal.Copy((IntPtr)ptr, array, 0, len);
				MD5 md = MD5.Create();
				byte[] array2 = md.ComputeHash(array);
				StringBuilder stringBuilder = new StringBuilder();
				for (int j = 0; j < array2.Length; j++)
				{
					stringBuilder.Append(array2[j].ToString("X2"));
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(6, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("HASH: ");
				defaultInterpolatedStringHandler2.AppendFormatted<StringBuilder>(stringBuilder);
				FileLog.Log(defaultInterpolatedStringHandler2.ToStringAndClear());
			}
		}

		// Token: 0x040002B5 RID: 693
		private static readonly object fileLock = new object();

		// Token: 0x040002B6 RID: 694
		private static bool _logPathInited;

		// Token: 0x040002B7 RID: 695
		private static string _logPath;

		// Token: 0x040002B9 RID: 697
		public static char indentChar = '\t';

		// Token: 0x040002BA RID: 698
		public static int indentLevel = 0;

		// Token: 0x040002BB RID: 699
		private static List<string> buffer = new List<string>();
	}
}
