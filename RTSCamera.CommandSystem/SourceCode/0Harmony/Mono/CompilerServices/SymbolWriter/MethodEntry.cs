using System;
using System.Collections.Generic;

namespace Mono.CompilerServices.SymbolWriter
{
	// Token: 0x02000345 RID: 837
	internal class MethodEntry : IComparable
	{
		// Token: 0x17000573 RID: 1395
		// (get) Token: 0x06001563 RID: 5475 RVA: 0x00044128 File Offset: 0x00042328
		public MethodEntry.Flags MethodFlags
		{
			get
			{
				return this.flags;
			}
		}

		// Token: 0x17000574 RID: 1396
		// (get) Token: 0x06001564 RID: 5476 RVA: 0x00044130 File Offset: 0x00042330
		// (set) Token: 0x06001565 RID: 5477 RVA: 0x00044138 File Offset: 0x00042338
		public int Index
		{
			get
			{
				return this.index;
			}
			set
			{
				this.index = value;
			}
		}

		// Token: 0x06001566 RID: 5478 RVA: 0x00044144 File Offset: 0x00042344
		internal MethodEntry(MonoSymbolFile file, MyBinaryReader reader, int index)
		{
			this.SymbolFile = file;
			this.index = index;
			this.Token = reader.ReadInt32();
			this.DataOffset = reader.ReadInt32();
			this.LineNumberTableOffset = reader.ReadInt32();
			long position = reader.BaseStream.Position;
			reader.BaseStream.Position = (long)this.DataOffset;
			this.CompileUnitIndex = reader.ReadLeb128();
			this.LocalVariableTableOffset = reader.ReadLeb128();
			this.NamespaceID = reader.ReadLeb128();
			this.CodeBlockTableOffset = reader.ReadLeb128();
			this.ScopeVariableTableOffset = reader.ReadLeb128();
			this.RealNameOffset = reader.ReadLeb128();
			this.flags = (MethodEntry.Flags)reader.ReadLeb128();
			reader.BaseStream.Position = position;
			this.CompileUnit = file.GetCompileUnit(this.CompileUnitIndex);
		}

		// Token: 0x06001567 RID: 5479 RVA: 0x0004421C File Offset: 0x0004241C
		internal MethodEntry(MonoSymbolFile file, CompileUnitEntry comp_unit, int token, ScopeVariable[] scope_vars, LocalVariableEntry[] locals, LineNumberEntry[] lines, CodeBlockEntry[] code_blocks, string real_name, MethodEntry.Flags flags, int namespace_id)
		{
			this.SymbolFile = file;
			this.real_name = real_name;
			this.locals = locals;
			this.code_blocks = code_blocks;
			this.scope_vars = scope_vars;
			this.flags = flags;
			this.index = -1;
			this.Token = token;
			this.CompileUnitIndex = comp_unit.Index;
			this.CompileUnit = comp_unit;
			this.NamespaceID = namespace_id;
			MethodEntry.CheckLineNumberTable(lines);
			this.lnt = new LineNumberTable(file, lines);
			file.NumLineNumbers += lines.Length;
			int num = ((locals != null) ? locals.Length : 0);
			if (num <= 32)
			{
				for (int i = 0; i < num; i++)
				{
					string name = locals[i].Name;
					for (int j = i + 1; j < num; j++)
					{
						if (locals[j].Name == name)
						{
							flags |= MethodEntry.Flags.LocalNamesAmbiguous;
							return;
						}
					}
				}
				return;
			}
			Dictionary<string, LocalVariableEntry> dictionary = new Dictionary<string, LocalVariableEntry>();
			foreach (LocalVariableEntry localVariableEntry in locals)
			{
				if (dictionary.ContainsKey(localVariableEntry.Name))
				{
					flags |= MethodEntry.Flags.LocalNamesAmbiguous;
					return;
				}
				dictionary.Add(localVariableEntry.Name, localVariableEntry);
			}
		}

		// Token: 0x06001568 RID: 5480 RVA: 0x00044354 File Offset: 0x00042554
		private static void CheckLineNumberTable(LineNumberEntry[] line_numbers)
		{
			int num = -1;
			int num2 = -1;
			if (line_numbers == null)
			{
				return;
			}
			foreach (LineNumberEntry lineNumberEntry in line_numbers)
			{
				if (lineNumberEntry.Equals(LineNumberEntry.Null))
				{
					throw new MonoSymbolFileException();
				}
				if (lineNumberEntry.Offset < num)
				{
					throw new MonoSymbolFileException();
				}
				if (lineNumberEntry.Offset > num)
				{
					num2 = lineNumberEntry.Row;
					num = lineNumberEntry.Offset;
				}
				else if (lineNumberEntry.Row > num2)
				{
					num2 = lineNumberEntry.Row;
				}
			}
		}

		// Token: 0x06001569 RID: 5481 RVA: 0x000443C6 File Offset: 0x000425C6
		internal void Write(MyBinaryWriter bw)
		{
			if (this.index <= 0 || this.DataOffset == 0)
			{
				throw new InvalidOperationException();
			}
			bw.Write(this.Token);
			bw.Write(this.DataOffset);
			bw.Write(this.LineNumberTableOffset);
		}

		// Token: 0x0600156A RID: 5482 RVA: 0x00044404 File Offset: 0x00042604
		internal void WriteData(MonoSymbolFile file, MyBinaryWriter bw)
		{
			if (this.index <= 0)
			{
				throw new InvalidOperationException();
			}
			this.LocalVariableTableOffset = (int)bw.BaseStream.Position;
			int num = ((this.locals != null) ? this.locals.Length : 0);
			bw.WriteLeb128(num);
			for (int i = 0; i < num; i++)
			{
				this.locals[i].Write(file, bw);
			}
			file.LocalCount += num;
			this.CodeBlockTableOffset = (int)bw.BaseStream.Position;
			int num2 = ((this.code_blocks != null) ? this.code_blocks.Length : 0);
			bw.WriteLeb128(num2);
			for (int j = 0; j < num2; j++)
			{
				this.code_blocks[j].Write(bw);
			}
			this.ScopeVariableTableOffset = (int)bw.BaseStream.Position;
			int num3 = ((this.scope_vars != null) ? this.scope_vars.Length : 0);
			bw.WriteLeb128(num3);
			for (int k = 0; k < num3; k++)
			{
				this.scope_vars[k].Write(bw);
			}
			if (this.real_name != null)
			{
				this.RealNameOffset = (int)bw.BaseStream.Position;
				bw.Write(this.real_name);
			}
			foreach (LineNumberEntry lineNumberEntry in this.lnt.LineNumbers)
			{
				if (lineNumberEntry.EndRow != -1 || lineNumberEntry.EndColumn != -1)
				{
					this.flags |= MethodEntry.Flags.EndInfoIncluded;
				}
			}
			this.LineNumberTableOffset = (int)bw.BaseStream.Position;
			this.lnt.Write(file, bw, (this.flags & MethodEntry.Flags.ColumnsInfoIncluded) > (MethodEntry.Flags)0, (this.flags & MethodEntry.Flags.EndInfoIncluded) > (MethodEntry.Flags)0);
			this.DataOffset = (int)bw.BaseStream.Position;
			bw.WriteLeb128(this.CompileUnitIndex);
			bw.WriteLeb128(this.LocalVariableTableOffset);
			bw.WriteLeb128(this.NamespaceID);
			bw.WriteLeb128(this.CodeBlockTableOffset);
			bw.WriteLeb128(this.ScopeVariableTableOffset);
			bw.WriteLeb128(this.RealNameOffset);
			bw.WriteLeb128((int)this.flags);
		}

		// Token: 0x0600156B RID: 5483 RVA: 0x00044620 File Offset: 0x00042820
		public void ReadAll()
		{
			this.GetLineNumberTable();
			this.GetLocals();
			this.GetCodeBlocks();
			this.GetScopeVariables();
			this.GetRealName();
		}

		// Token: 0x0600156C RID: 5484 RVA: 0x00044648 File Offset: 0x00042848
		public LineNumberTable GetLineNumberTable()
		{
			MonoSymbolFile symbolFile = this.SymbolFile;
			LineNumberTable lineNumberTable;
			lock (symbolFile)
			{
				if (this.lnt != null)
				{
					lineNumberTable = this.lnt;
				}
				else if (this.LineNumberTableOffset == 0)
				{
					lineNumberTable = null;
				}
				else
				{
					MyBinaryReader binaryReader = this.SymbolFile.BinaryReader;
					long position = binaryReader.BaseStream.Position;
					binaryReader.BaseStream.Position = (long)this.LineNumberTableOffset;
					this.lnt = LineNumberTable.Read(this.SymbolFile, binaryReader, (this.flags & MethodEntry.Flags.ColumnsInfoIncluded) > (MethodEntry.Flags)0, (this.flags & MethodEntry.Flags.EndInfoIncluded) > (MethodEntry.Flags)0);
					binaryReader.BaseStream.Position = position;
					lineNumberTable = this.lnt;
				}
			}
			return lineNumberTable;
		}

		// Token: 0x0600156D RID: 5485 RVA: 0x0004470C File Offset: 0x0004290C
		public LocalVariableEntry[] GetLocals()
		{
			MonoSymbolFile symbolFile = this.SymbolFile;
			LocalVariableEntry[] array;
			lock (symbolFile)
			{
				if (this.locals != null)
				{
					array = this.locals;
				}
				else if (this.LocalVariableTableOffset == 0)
				{
					array = null;
				}
				else
				{
					MyBinaryReader binaryReader = this.SymbolFile.BinaryReader;
					long position = binaryReader.BaseStream.Position;
					binaryReader.BaseStream.Position = (long)this.LocalVariableTableOffset;
					int num = binaryReader.ReadLeb128();
					this.locals = new LocalVariableEntry[num];
					for (int i = 0; i < num; i++)
					{
						this.locals[i] = new LocalVariableEntry(this.SymbolFile, binaryReader);
					}
					binaryReader.BaseStream.Position = position;
					array = this.locals;
				}
			}
			return array;
		}

		// Token: 0x0600156E RID: 5486 RVA: 0x000447EC File Offset: 0x000429EC
		public CodeBlockEntry[] GetCodeBlocks()
		{
			MonoSymbolFile symbolFile = this.SymbolFile;
			CodeBlockEntry[] array;
			lock (symbolFile)
			{
				if (this.code_blocks != null)
				{
					array = this.code_blocks;
				}
				else if (this.CodeBlockTableOffset == 0)
				{
					array = null;
				}
				else
				{
					MyBinaryReader binaryReader = this.SymbolFile.BinaryReader;
					long position = binaryReader.BaseStream.Position;
					binaryReader.BaseStream.Position = (long)this.CodeBlockTableOffset;
					int num = binaryReader.ReadLeb128();
					this.code_blocks = new CodeBlockEntry[num];
					for (int i = 0; i < num; i++)
					{
						this.code_blocks[i] = new CodeBlockEntry(i, binaryReader);
					}
					binaryReader.BaseStream.Position = position;
					array = this.code_blocks;
				}
			}
			return array;
		}

		// Token: 0x0600156F RID: 5487 RVA: 0x000448C4 File Offset: 0x00042AC4
		public ScopeVariable[] GetScopeVariables()
		{
			MonoSymbolFile symbolFile = this.SymbolFile;
			ScopeVariable[] array;
			lock (symbolFile)
			{
				if (this.scope_vars != null)
				{
					array = this.scope_vars;
				}
				else if (this.ScopeVariableTableOffset == 0)
				{
					array = null;
				}
				else
				{
					MyBinaryReader binaryReader = this.SymbolFile.BinaryReader;
					long position = binaryReader.BaseStream.Position;
					binaryReader.BaseStream.Position = (long)this.ScopeVariableTableOffset;
					int num = binaryReader.ReadLeb128();
					this.scope_vars = new ScopeVariable[num];
					for (int i = 0; i < num; i++)
					{
						this.scope_vars[i] = new ScopeVariable(binaryReader);
					}
					binaryReader.BaseStream.Position = position;
					array = this.scope_vars;
				}
			}
			return array;
		}

		// Token: 0x06001570 RID: 5488 RVA: 0x0004499C File Offset: 0x00042B9C
		public string GetRealName()
		{
			MonoSymbolFile symbolFile = this.SymbolFile;
			string text;
			lock (symbolFile)
			{
				if (this.real_name != null)
				{
					text = this.real_name;
				}
				else if (this.RealNameOffset == 0)
				{
					text = null;
				}
				else
				{
					this.real_name = this.SymbolFile.BinaryReader.ReadString(this.RealNameOffset);
					text = this.real_name;
				}
			}
			return text;
		}

		// Token: 0x06001571 RID: 5489 RVA: 0x00044A18 File Offset: 0x00042C18
		public int CompareTo(object obj)
		{
			MethodEntry methodEntry = (MethodEntry)obj;
			if (methodEntry.Token < this.Token)
			{
				return 1;
			}
			if (methodEntry.Token > this.Token)
			{
				return -1;
			}
			return 0;
		}

		// Token: 0x06001572 RID: 5490 RVA: 0x00044A50 File Offset: 0x00042C50
		public override string ToString()
		{
			return string.Format("[Method {0}:{1:x}:{2}:{3}]", new object[] { this.index, this.Token, this.CompileUnitIndex, this.CompileUnit });
		}

		// Token: 0x04000AEA RID: 2794
		public readonly int CompileUnitIndex;

		// Token: 0x04000AEB RID: 2795
		public readonly int Token;

		// Token: 0x04000AEC RID: 2796
		public readonly int NamespaceID;

		// Token: 0x04000AED RID: 2797
		private int DataOffset;

		// Token: 0x04000AEE RID: 2798
		private int LocalVariableTableOffset;

		// Token: 0x04000AEF RID: 2799
		private int LineNumberTableOffset;

		// Token: 0x04000AF0 RID: 2800
		private int CodeBlockTableOffset;

		// Token: 0x04000AF1 RID: 2801
		private int ScopeVariableTableOffset;

		// Token: 0x04000AF2 RID: 2802
		private int RealNameOffset;

		// Token: 0x04000AF3 RID: 2803
		private MethodEntry.Flags flags;

		// Token: 0x04000AF4 RID: 2804
		private int index;

		// Token: 0x04000AF5 RID: 2805
		public readonly CompileUnitEntry CompileUnit;

		// Token: 0x04000AF6 RID: 2806
		private LocalVariableEntry[] locals;

		// Token: 0x04000AF7 RID: 2807
		private CodeBlockEntry[] code_blocks;

		// Token: 0x04000AF8 RID: 2808
		private ScopeVariable[] scope_vars;

		// Token: 0x04000AF9 RID: 2809
		private LineNumberTable lnt;

		// Token: 0x04000AFA RID: 2810
		private string real_name;

		// Token: 0x04000AFB RID: 2811
		public readonly MonoSymbolFile SymbolFile;

		// Token: 0x04000AFC RID: 2812
		public const int Size = 12;

		// Token: 0x02000346 RID: 838
		[Flags]
		public enum Flags
		{
			// Token: 0x04000AFE RID: 2814
			LocalNamesAmbiguous = 1,
			// Token: 0x04000AFF RID: 2815
			ColumnsInfoIncluded = 2,
			// Token: 0x04000B00 RID: 2816
			EndInfoIncluded = 4
		}
	}
}
