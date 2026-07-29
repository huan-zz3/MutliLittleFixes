using System;
using System.Collections.Generic;
using System.IO;

namespace Mono.CompilerServices.SymbolWriter
{
	// Token: 0x02000342 RID: 834
	internal class CompileUnitEntry : ICompileUnit
	{
		// Token: 0x17000569 RID: 1385
		// (get) Token: 0x06001541 RID: 5441 RVA: 0x0004350F File Offset: 0x0004170F
		public static int Size
		{
			get
			{
				return 8;
			}
		}

		// Token: 0x1700056A RID: 1386
		// (get) Token: 0x06001542 RID: 5442 RVA: 0x0001B6A2 File Offset: 0x000198A2
		CompileUnitEntry ICompileUnit.Entry
		{
			get
			{
				return this;
			}
		}

		// Token: 0x06001543 RID: 5443 RVA: 0x00043512 File Offset: 0x00041712
		public CompileUnitEntry(MonoSymbolFile file, SourceFileEntry source)
		{
			this.file = file;
			this.source = source;
			this.Index = file.AddCompileUnit(this);
			this.creating = true;
			this.namespaces = new List<NamespaceEntry>();
		}

		// Token: 0x06001544 RID: 5444 RVA: 0x00043547 File Offset: 0x00041747
		public void AddFile(SourceFileEntry file)
		{
			if (!this.creating)
			{
				throw new InvalidOperationException();
			}
			if (this.include_files == null)
			{
				this.include_files = new List<SourceFileEntry>();
			}
			this.include_files.Add(file);
		}

		// Token: 0x1700056B RID: 1387
		// (get) Token: 0x06001545 RID: 5445 RVA: 0x00043576 File Offset: 0x00041776
		public SourceFileEntry SourceFile
		{
			get
			{
				if (this.creating)
				{
					return this.source;
				}
				this.ReadData();
				return this.source;
			}
		}

		// Token: 0x06001546 RID: 5446 RVA: 0x00043594 File Offset: 0x00041794
		public int DefineNamespace(string name, string[] using_clauses, int parent)
		{
			if (!this.creating)
			{
				throw new InvalidOperationException();
			}
			int nextNamespaceIndex = this.file.GetNextNamespaceIndex();
			NamespaceEntry namespaceEntry = new NamespaceEntry(name, nextNamespaceIndex, using_clauses, parent);
			this.namespaces.Add(namespaceEntry);
			return nextNamespaceIndex;
		}

		// Token: 0x06001547 RID: 5447 RVA: 0x000435D4 File Offset: 0x000417D4
		internal void WriteData(MyBinaryWriter bw)
		{
			this.DataOffset = (int)bw.BaseStream.Position;
			bw.WriteLeb128(this.source.Index);
			int num = ((this.include_files != null) ? this.include_files.Count : 0);
			bw.WriteLeb128(num);
			if (this.include_files != null)
			{
				foreach (SourceFileEntry sourceFileEntry in this.include_files)
				{
					bw.WriteLeb128(sourceFileEntry.Index);
				}
			}
			bw.WriteLeb128(this.namespaces.Count);
			foreach (NamespaceEntry namespaceEntry in this.namespaces)
			{
				namespaceEntry.Write(this.file, bw);
			}
		}

		// Token: 0x06001548 RID: 5448 RVA: 0x000436D0 File Offset: 0x000418D0
		internal void Write(BinaryWriter bw)
		{
			bw.Write(this.Index);
			bw.Write(this.DataOffset);
		}

		// Token: 0x06001549 RID: 5449 RVA: 0x000436EA File Offset: 0x000418EA
		internal CompileUnitEntry(MonoSymbolFile file, MyBinaryReader reader)
		{
			this.file = file;
			this.Index = reader.ReadInt32();
			this.DataOffset = reader.ReadInt32();
		}

		// Token: 0x0600154A RID: 5450 RVA: 0x00043711 File Offset: 0x00041911
		public void ReadAll()
		{
			this.ReadData();
		}

		// Token: 0x0600154B RID: 5451 RVA: 0x0004371C File Offset: 0x0004191C
		private void ReadData()
		{
			if (this.creating)
			{
				throw new InvalidOperationException();
			}
			MonoSymbolFile monoSymbolFile = this.file;
			lock (monoSymbolFile)
			{
				if (this.namespaces == null)
				{
					MyBinaryReader binaryReader = this.file.BinaryReader;
					int num = (int)binaryReader.BaseStream.Position;
					binaryReader.BaseStream.Position = (long)this.DataOffset;
					int num2 = binaryReader.ReadLeb128();
					this.source = this.file.GetSourceFile(num2);
					int num3 = binaryReader.ReadLeb128();
					if (num3 > 0)
					{
						this.include_files = new List<SourceFileEntry>();
						for (int i = 0; i < num3; i++)
						{
							this.include_files.Add(this.file.GetSourceFile(binaryReader.ReadLeb128()));
						}
					}
					int num4 = binaryReader.ReadLeb128();
					this.namespaces = new List<NamespaceEntry>();
					for (int j = 0; j < num4; j++)
					{
						this.namespaces.Add(new NamespaceEntry(this.file, binaryReader));
					}
					binaryReader.BaseStream.Position = (long)num;
				}
			}
		}

		// Token: 0x1700056C RID: 1388
		// (get) Token: 0x0600154C RID: 5452 RVA: 0x00043844 File Offset: 0x00041A44
		public NamespaceEntry[] Namespaces
		{
			get
			{
				this.ReadData();
				NamespaceEntry[] array = new NamespaceEntry[this.namespaces.Count];
				this.namespaces.CopyTo(array, 0);
				return array;
			}
		}

		// Token: 0x1700056D RID: 1389
		// (get) Token: 0x0600154D RID: 5453 RVA: 0x00043878 File Offset: 0x00041A78
		public SourceFileEntry[] IncludeFiles
		{
			get
			{
				this.ReadData();
				if (this.include_files == null)
				{
					return new SourceFileEntry[0];
				}
				SourceFileEntry[] array = new SourceFileEntry[this.include_files.Count];
				this.include_files.CopyTo(array, 0);
				return array;
			}
		}

		// Token: 0x04000AC9 RID: 2761
		public readonly int Index;

		// Token: 0x04000ACA RID: 2762
		private int DataOffset;

		// Token: 0x04000ACB RID: 2763
		private MonoSymbolFile file;

		// Token: 0x04000ACC RID: 2764
		private SourceFileEntry source;

		// Token: 0x04000ACD RID: 2765
		private List<SourceFileEntry> include_files;

		// Token: 0x04000ACE RID: 2766
		private List<NamespaceEntry> namespaces;

		// Token: 0x04000ACF RID: 2767
		private bool creating;
	}
}
