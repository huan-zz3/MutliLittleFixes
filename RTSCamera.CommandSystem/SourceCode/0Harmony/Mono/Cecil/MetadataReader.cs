using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Mono.Cecil.Cil;
using Mono.Cecil.Metadata;
using Mono.Cecil.PE;
using Mono.Collections.Generic;

namespace Mono.Cecil
{
	// Token: 0x020001F0 RID: 496
	internal sealed class MetadataReader : ByteBuffer
	{
		// Token: 0x060009C3 RID: 2499 RVA: 0x0001F9B8 File Offset: 0x0001DBB8
		public MetadataReader(ModuleDefinition module)
			: base(module.Image.TableHeap.data)
		{
			this.image = module.Image;
			this.module = module;
			this.metadata = module.MetadataSystem;
			this.code = new CodeReader(this);
		}

		// Token: 0x060009C4 RID: 2500 RVA: 0x0001FA06 File Offset: 0x0001DC06
		public MetadataReader(Image image, ModuleDefinition module, MetadataReader metadata_reader)
			: base(image.TableHeap.data)
		{
			this.image = image;
			this.module = module;
			this.metadata = module.MetadataSystem;
			this.metadata_reader = metadata_reader;
		}

		// Token: 0x060009C5 RID: 2501 RVA: 0x0001FA3A File Offset: 0x0001DC3A
		private int GetCodedIndexSize(CodedIndex index)
		{
			return this.image.GetCodedIndexSize(index);
		}

		// Token: 0x060009C6 RID: 2502 RVA: 0x0001FA48 File Offset: 0x0001DC48
		private uint ReadByIndexSize(int size)
		{
			if (size == 4)
			{
				return base.ReadUInt32();
			}
			return (uint)base.ReadUInt16();
		}

		// Token: 0x060009C7 RID: 2503 RVA: 0x0001FA5C File Offset: 0x0001DC5C
		private byte[] ReadBlob()
		{
			BlobHeap blobHeap = this.image.BlobHeap;
			if (blobHeap == null)
			{
				this.position += 2;
				return Empty<byte>.Array;
			}
			return blobHeap.Read(this.ReadBlobIndex());
		}

		// Token: 0x060009C8 RID: 2504 RVA: 0x0001FA98 File Offset: 0x0001DC98
		private byte[] ReadBlob(uint signature)
		{
			BlobHeap blobHeap = this.image.BlobHeap;
			if (blobHeap == null)
			{
				return Empty<byte>.Array;
			}
			return blobHeap.Read(signature);
		}

		// Token: 0x060009C9 RID: 2505 RVA: 0x0001FAC4 File Offset: 0x0001DCC4
		private uint ReadBlobIndex()
		{
			BlobHeap blobHeap = this.image.BlobHeap;
			return this.ReadByIndexSize((blobHeap != null) ? blobHeap.IndexSize : 2);
		}

		// Token: 0x060009CA RID: 2506 RVA: 0x0001FAF0 File Offset: 0x0001DCF0
		private void GetBlobView(uint signature, out byte[] blob, out int index, out int count)
		{
			BlobHeap blobHeap = this.image.BlobHeap;
			if (blobHeap == null)
			{
				blob = null;
				index = (count = 0);
				return;
			}
			blobHeap.GetView(signature, out blob, out index, out count);
		}

		// Token: 0x060009CB RID: 2507 RVA: 0x0001FB24 File Offset: 0x0001DD24
		private string ReadString()
		{
			return this.image.StringHeap.Read(this.ReadByIndexSize(this.image.StringHeap.IndexSize));
		}

		// Token: 0x060009CC RID: 2508 RVA: 0x0001FB4C File Offset: 0x0001DD4C
		private uint ReadStringIndex()
		{
			return this.ReadByIndexSize(this.image.StringHeap.IndexSize);
		}

		// Token: 0x060009CD RID: 2509 RVA: 0x0001FB64 File Offset: 0x0001DD64
		private Guid ReadGuid()
		{
			return this.image.GuidHeap.Read(this.ReadByIndexSize(this.image.GuidHeap.IndexSize));
		}

		// Token: 0x060009CE RID: 2510 RVA: 0x0001FB8C File Offset: 0x0001DD8C
		private uint ReadTableIndex(Table table)
		{
			return this.ReadByIndexSize(this.image.GetTableIndexSize(table));
		}

		// Token: 0x060009CF RID: 2511 RVA: 0x0001FBA0 File Offset: 0x0001DDA0
		private MetadataToken ReadMetadataToken(CodedIndex index)
		{
			return index.GetMetadataToken(this.ReadByIndexSize(this.GetCodedIndexSize(index)));
		}

		// Token: 0x060009D0 RID: 2512 RVA: 0x0001FBB8 File Offset: 0x0001DDB8
		private int MoveTo(Table table)
		{
			TableInformation tableInformation = this.image.TableHeap[table];
			if (tableInformation.Length != 0U)
			{
				this.position = (int)tableInformation.Offset;
			}
			return (int)tableInformation.Length;
		}

		// Token: 0x060009D1 RID: 2513 RVA: 0x0001FBF4 File Offset: 0x0001DDF4
		private bool MoveTo(Table table, uint row)
		{
			TableInformation tableInformation = this.image.TableHeap[table];
			uint length = tableInformation.Length;
			if (length == 0U || row > length)
			{
				return false;
			}
			this.position = (int)(tableInformation.Offset + tableInformation.RowSize * (row - 1U));
			return true;
		}

		// Token: 0x060009D2 RID: 2514 RVA: 0x0001FC3C File Offset: 0x0001DE3C
		public AssemblyNameDefinition ReadAssemblyNameDefinition()
		{
			if (this.MoveTo(Table.Assembly) == 0)
			{
				return null;
			}
			AssemblyNameDefinition assemblyNameDefinition = new AssemblyNameDefinition();
			assemblyNameDefinition.HashAlgorithm = (AssemblyHashAlgorithm)base.ReadUInt32();
			this.PopulateVersionAndFlags(assemblyNameDefinition);
			assemblyNameDefinition.PublicKey = this.ReadBlob();
			this.PopulateNameAndCulture(assemblyNameDefinition);
			return assemblyNameDefinition;
		}

		// Token: 0x060009D3 RID: 2515 RVA: 0x0001FC82 File Offset: 0x0001DE82
		public ModuleDefinition Populate(ModuleDefinition module)
		{
			if (this.MoveTo(Table.Module) == 0)
			{
				return module;
			}
			base.Advance(2);
			module.Name = this.ReadString();
			module.Mvid = this.ReadGuid();
			return module;
		}

		// Token: 0x060009D4 RID: 2516 RVA: 0x0001FCB0 File Offset: 0x0001DEB0
		private void InitializeAssemblyReferences()
		{
			if (this.metadata.AssemblyReferences != null)
			{
				return;
			}
			int num = this.MoveTo(Table.AssemblyRef);
			AssemblyNameReference[] array = (this.metadata.AssemblyReferences = new AssemblyNameReference[num]);
			uint num2 = 0U;
			while ((ulong)num2 < (ulong)((long)num))
			{
				AssemblyNameReference assemblyNameReference = new AssemblyNameReference();
				assemblyNameReference.token = new MetadataToken(TokenType.AssemblyRef, num2 + 1U);
				this.PopulateVersionAndFlags(assemblyNameReference);
				byte[] array2 = this.ReadBlob();
				if (assemblyNameReference.HasPublicKey)
				{
					assemblyNameReference.PublicKey = array2;
				}
				else
				{
					assemblyNameReference.PublicKeyToken = array2;
				}
				this.PopulateNameAndCulture(assemblyNameReference);
				assemblyNameReference.Hash = this.ReadBlob();
				array[(int)num2] = assemblyNameReference;
				num2 += 1U;
			}
		}

		// Token: 0x060009D5 RID: 2517 RVA: 0x0001FD5C File Offset: 0x0001DF5C
		public Collection<AssemblyNameReference> ReadAssemblyReferences()
		{
			this.InitializeAssemblyReferences();
			Collection<AssemblyNameReference> collection = new Collection<AssemblyNameReference>(this.metadata.AssemblyReferences);
			if (this.module.IsWindowsMetadata())
			{
				this.module.Projections.AddVirtualReferences(collection);
			}
			return collection;
		}

		// Token: 0x060009D6 RID: 2518 RVA: 0x0001FDA0 File Offset: 0x0001DFA0
		public MethodDefinition ReadEntryPoint()
		{
			if (this.module.Image.EntryPointToken == 0U)
			{
				return null;
			}
			MetadataToken metadataToken = new MetadataToken(this.module.Image.EntryPointToken);
			return this.GetMethodDefinition(metadataToken.RID);
		}

		// Token: 0x060009D7 RID: 2519 RVA: 0x0001FDE8 File Offset: 0x0001DFE8
		public Collection<ModuleDefinition> ReadModules()
		{
			Collection<ModuleDefinition> collection = new Collection<ModuleDefinition>(1);
			collection.Add(this.module);
			int num = this.MoveTo(Table.File);
			uint num2 = 1U;
			while ((ulong)num2 <= (ulong)((long)num))
			{
				bool flag = base.ReadUInt32() != 0U;
				string text = this.ReadString();
				this.ReadBlobIndex();
				if (!flag)
				{
					ReaderParameters readerParameters = new ReaderParameters
					{
						ReadingMode = this.module.ReadingMode,
						SymbolReaderProvider = this.module.SymbolReaderProvider,
						AssemblyResolver = this.module.AssemblyResolver
					};
					ModuleDefinition moduleDefinition = ModuleDefinition.ReadModule(this.GetModuleFileName(text), readerParameters);
					moduleDefinition.assembly = this.module.assembly;
					collection.Add(moduleDefinition);
				}
				num2 += 1U;
			}
			return collection;
		}

		// Token: 0x060009D8 RID: 2520 RVA: 0x0001FE9D File Offset: 0x0001E09D
		private string GetModuleFileName(string name)
		{
			if (this.module.FileName == null)
			{
				throw new NotSupportedException();
			}
			return Path.Combine(Path.GetDirectoryName(this.module.FileName), name);
		}

		// Token: 0x060009D9 RID: 2521 RVA: 0x0001FEC8 File Offset: 0x0001E0C8
		private void InitializeModuleReferences()
		{
			if (this.metadata.ModuleReferences != null)
			{
				return;
			}
			int num = this.MoveTo(Table.ModuleRef);
			ModuleReference[] array = (this.metadata.ModuleReferences = new ModuleReference[num]);
			uint num2 = 0U;
			while ((ulong)num2 < (ulong)((long)num))
			{
				array[(int)num2] = new ModuleReference(this.ReadString())
				{
					token = new MetadataToken(TokenType.ModuleRef, num2 + 1U)
				};
				num2 += 1U;
			}
		}

		// Token: 0x060009DA RID: 2522 RVA: 0x0001FF35 File Offset: 0x0001E135
		public Collection<ModuleReference> ReadModuleReferences()
		{
			this.InitializeModuleReferences();
			return new Collection<ModuleReference>(this.metadata.ModuleReferences);
		}

		// Token: 0x060009DB RID: 2523 RVA: 0x0001FF50 File Offset: 0x0001E150
		public bool HasFileResource()
		{
			int num = this.MoveTo(Table.File);
			if (num == 0)
			{
				return false;
			}
			uint num2 = 1U;
			while ((ulong)num2 <= (ulong)((long)num))
			{
				if (this.ReadFileRecord(num2).Col1 == FileAttributes.ContainsNoMetaData)
				{
					return true;
				}
				num2 += 1U;
			}
			return false;
		}

		// Token: 0x060009DC RID: 2524 RVA: 0x0001FF8C File Offset: 0x0001E18C
		public Collection<Resource> ReadResources()
		{
			int num = this.MoveTo(Table.ManifestResource);
			Collection<Resource> collection = new Collection<Resource>(num);
			int i = 1;
			while (i <= num)
			{
				uint num2 = base.ReadUInt32();
				ManifestResourceAttributes manifestResourceAttributes = (ManifestResourceAttributes)base.ReadUInt32();
				string text = this.ReadString();
				MetadataToken metadataToken = this.ReadMetadataToken(CodedIndex.Implementation);
				Resource resource;
				if (metadataToken.RID == 0U)
				{
					resource = new EmbeddedResource(text, manifestResourceAttributes, num2, this);
					goto IL_00C6;
				}
				if (metadataToken.TokenType == TokenType.AssemblyRef)
				{
					resource = new AssemblyLinkedResource(text, manifestResourceAttributes)
					{
						Assembly = (AssemblyNameReference)this.GetTypeReferenceScope(metadataToken)
					};
					goto IL_00C6;
				}
				if (metadataToken.TokenType == TokenType.File)
				{
					Row<FileAttributes, string, uint> row = this.ReadFileRecord(metadataToken.RID);
					resource = new LinkedResource(text, manifestResourceAttributes)
					{
						File = row.Col2,
						hash = this.ReadBlob(row.Col3)
					};
					goto IL_00C6;
				}
				IL_00CE:
				i++;
				continue;
				IL_00C6:
				collection.Add(resource);
				goto IL_00CE;
			}
			return collection;
		}

		// Token: 0x060009DD RID: 2525 RVA: 0x00020074 File Offset: 0x0001E274
		private Row<FileAttributes, string, uint> ReadFileRecord(uint rid)
		{
			int position = this.position;
			if (!this.MoveTo(Table.File, rid))
			{
				throw new ArgumentException();
			}
			Row<FileAttributes, string, uint> row = new Row<FileAttributes, string, uint>((FileAttributes)base.ReadUInt32(), this.ReadString(), this.ReadBlobIndex());
			this.position = position;
			return row;
		}

		// Token: 0x060009DE RID: 2526 RVA: 0x000200B8 File Offset: 0x0001E2B8
		public byte[] GetManagedResource(uint offset)
		{
			return this.image.GetReaderAt<uint, byte[]>(this.image.Resources.VirtualAddress, offset, delegate(uint o, BinaryStreamReader reader)
			{
				reader.Advance((int)o);
				return reader.ReadBytes(reader.ReadInt32());
			}) ?? Empty<byte>.Array;
		}

		// Token: 0x060009DF RID: 2527 RVA: 0x00020109 File Offset: 0x0001E309
		private void PopulateVersionAndFlags(AssemblyNameReference name)
		{
			name.Version = new Version((int)base.ReadUInt16(), (int)base.ReadUInt16(), (int)base.ReadUInt16(), (int)base.ReadUInt16());
			name.Attributes = (AssemblyAttributes)base.ReadUInt32();
		}

		// Token: 0x060009E0 RID: 2528 RVA: 0x0002013A File Offset: 0x0001E33A
		private void PopulateNameAndCulture(AssemblyNameReference name)
		{
			name.Name = this.ReadString();
			name.Culture = this.ReadString();
		}

		// Token: 0x060009E1 RID: 2529 RVA: 0x00020154 File Offset: 0x0001E354
		public TypeDefinitionCollection ReadTypes()
		{
			this.InitializeTypeDefinitions();
			TypeDefinition[] types = this.metadata.Types;
			int num = types.Length - this.metadata.NestedTypes.Count;
			TypeDefinitionCollection typeDefinitionCollection = new TypeDefinitionCollection(this.module, num);
			foreach (TypeDefinition typeDefinition in types)
			{
				if (!MetadataReader.IsNested(typeDefinition.Attributes))
				{
					typeDefinitionCollection.Add(typeDefinition);
				}
			}
			if (this.image.HasTable(Table.MethodPtr) || this.image.HasTable(Table.FieldPtr))
			{
				this.CompleteTypes();
			}
			return typeDefinitionCollection;
		}

		// Token: 0x060009E2 RID: 2530 RVA: 0x000201E4 File Offset: 0x0001E3E4
		private void CompleteTypes()
		{
			foreach (TypeDefinition typeDefinition in this.metadata.Types)
			{
				Mixin.Read(typeDefinition.Fields);
				Mixin.Read(typeDefinition.Methods);
			}
		}

		// Token: 0x060009E3 RID: 2531 RVA: 0x00020224 File Offset: 0x0001E424
		private void InitializeTypeDefinitions()
		{
			if (this.metadata.Types != null)
			{
				return;
			}
			this.InitializeNestedTypes();
			this.InitializeFields();
			this.InitializeMethods();
			int num = this.MoveTo(Table.TypeDef);
			TypeDefinition[] array = (this.metadata.Types = new TypeDefinition[num]);
			uint num2 = 0U;
			while ((ulong)num2 < (ulong)((long)num))
			{
				if (array[(int)num2] == null)
				{
					array[(int)num2] = this.ReadType(num2 + 1U);
				}
				num2 += 1U;
			}
			if (this.module.IsWindowsMetadata())
			{
				uint num3 = 0U;
				while ((ulong)num3 < (ulong)((long)num))
				{
					WindowsRuntimeProjections.Project(array[(int)num3]);
					num3 += 1U;
				}
			}
		}

		// Token: 0x060009E4 RID: 2532 RVA: 0x000202B8 File Offset: 0x0001E4B8
		private static bool IsNested(TypeAttributes attributes)
		{
			TypeAttributes typeAttributes = attributes & TypeAttributes.VisibilityMask;
			return typeAttributes - TypeAttributes.NestedPublic <= 5U;
		}

		// Token: 0x060009E5 RID: 2533 RVA: 0x000202D4 File Offset: 0x0001E4D4
		public bool HasNestedTypes(TypeDefinition type)
		{
			this.InitializeNestedTypes();
			Collection<uint> collection;
			return this.metadata.TryGetNestedTypeMapping(type, out collection) && collection.Count > 0;
		}

		// Token: 0x060009E6 RID: 2534 RVA: 0x00020304 File Offset: 0x0001E504
		public Collection<TypeDefinition> ReadNestedTypes(TypeDefinition type)
		{
			this.InitializeNestedTypes();
			Collection<uint> collection;
			if (!this.metadata.TryGetNestedTypeMapping(type, out collection))
			{
				return new MemberDefinitionCollection<TypeDefinition>(type);
			}
			MemberDefinitionCollection<TypeDefinition> memberDefinitionCollection = new MemberDefinitionCollection<TypeDefinition>(type, collection.Count);
			for (int i = 0; i < collection.Count; i++)
			{
				TypeDefinition typeDefinition = this.GetTypeDefinition(collection[i]);
				if (typeDefinition != null)
				{
					memberDefinitionCollection.Add(typeDefinition);
				}
			}
			return memberDefinitionCollection;
		}

		// Token: 0x060009E7 RID: 2535 RVA: 0x00020368 File Offset: 0x0001E568
		private void InitializeNestedTypes()
		{
			if (this.metadata.NestedTypes != null)
			{
				return;
			}
			int num = this.MoveTo(Table.NestedClass);
			this.metadata.NestedTypes = new Dictionary<uint, Collection<uint>>(num);
			this.metadata.ReverseNestedTypes = new Dictionary<uint, uint>(num);
			if (num == 0)
			{
				return;
			}
			for (int i = 1; i <= num; i++)
			{
				uint num2 = this.ReadTableIndex(Table.TypeDef);
				uint num3 = this.ReadTableIndex(Table.TypeDef);
				this.AddNestedMapping(num3, num2);
			}
		}

		// Token: 0x060009E8 RID: 2536 RVA: 0x000203D6 File Offset: 0x0001E5D6
		private void AddNestedMapping(uint declaring, uint nested)
		{
			this.metadata.SetNestedTypeMapping(declaring, MetadataReader.AddMapping<uint, uint>(this.metadata.NestedTypes, declaring, nested));
			this.metadata.SetReverseNestedTypeMapping(nested, declaring);
		}

		// Token: 0x060009E9 RID: 2537 RVA: 0x00020404 File Offset: 0x0001E604
		private static Collection<TValue> AddMapping<TKey, TValue>(Dictionary<TKey, Collection<TValue>> cache, TKey key, TValue value)
		{
			Collection<TValue> collection;
			if (!cache.TryGetValue(key, out collection))
			{
				collection = new Collection<TValue>();
			}
			collection.Add(value);
			return collection;
		}

		// Token: 0x060009EA RID: 2538 RVA: 0x0002042C File Offset: 0x0001E62C
		private TypeDefinition ReadType(uint rid)
		{
			if (!this.MoveTo(Table.TypeDef, rid))
			{
				return null;
			}
			TypeAttributes typeAttributes = (TypeAttributes)base.ReadUInt32();
			string text = this.ReadString();
			TypeDefinition typeDefinition = new TypeDefinition(this.ReadString(), text, typeAttributes);
			typeDefinition.token = new MetadataToken(TokenType.TypeDef, rid);
			typeDefinition.scope = this.module;
			typeDefinition.module = this.module;
			this.metadata.AddTypeDefinition(typeDefinition);
			this.context = typeDefinition;
			typeDefinition.BaseType = this.GetTypeDefOrRef(this.ReadMetadataToken(CodedIndex.TypeDefOrRef));
			typeDefinition.fields_range = this.ReadListRange(rid, Table.TypeDef, Table.Field);
			typeDefinition.methods_range = this.ReadListRange(rid, Table.TypeDef, Table.Method);
			if (MetadataReader.IsNested(typeAttributes))
			{
				typeDefinition.DeclaringType = this.GetNestedTypeDeclaringType(typeDefinition);
			}
			return typeDefinition;
		}

		// Token: 0x060009EB RID: 2539 RVA: 0x000204E4 File Offset: 0x0001E6E4
		private TypeDefinition GetNestedTypeDeclaringType(TypeDefinition type)
		{
			uint num;
			if (!this.metadata.TryGetReverseNestedTypeMapping(type, out num))
			{
				return null;
			}
			return this.GetTypeDefinition(num);
		}

		// Token: 0x060009EC RID: 2540 RVA: 0x0002050C File Offset: 0x0001E70C
		private Range ReadListRange(uint current_index, Table current, Table target)
		{
			Range range = default(Range);
			uint num = this.ReadTableIndex(target);
			if (num == 0U)
			{
				return range;
			}
			TableInformation tableInformation = this.image.TableHeap[current];
			uint num2;
			if (current_index == tableInformation.Length)
			{
				num2 = this.image.TableHeap[target].Length + 1U;
			}
			else
			{
				int position = this.position;
				this.position += (int)((ulong)tableInformation.RowSize - (ulong)((long)this.image.GetTableIndexSize(target)));
				num2 = this.ReadTableIndex(target);
				this.position = position;
			}
			range.Start = num;
			range.Length = num2 - num;
			return range;
		}

		// Token: 0x060009ED RID: 2541 RVA: 0x000205B4 File Offset: 0x0001E7B4
		public Row<short, int> ReadTypeLayout(TypeDefinition type)
		{
			this.InitializeTypeLayouts();
			uint rid = type.token.RID;
			Row<ushort, uint> row;
			if (!this.metadata.ClassLayouts.TryGetValue(rid, out row))
			{
				return new Row<short, int>(-1, -1);
			}
			type.PackingSize = (short)row.Col1;
			type.ClassSize = (int)row.Col2;
			this.metadata.ClassLayouts.Remove(rid);
			return new Row<short, int>((short)row.Col1, (int)row.Col2);
		}

		// Token: 0x060009EE RID: 2542 RVA: 0x00020630 File Offset: 0x0001E830
		private void InitializeTypeLayouts()
		{
			if (this.metadata.ClassLayouts != null)
			{
				return;
			}
			int num = this.MoveTo(Table.ClassLayout);
			Dictionary<uint, Row<ushort, uint>> dictionary = (this.metadata.ClassLayouts = new Dictionary<uint, Row<ushort, uint>>(num));
			uint num2 = 0U;
			while ((ulong)num2 < (ulong)((long)num))
			{
				ushort num3 = base.ReadUInt16();
				uint num4 = base.ReadUInt32();
				uint num5 = this.ReadTableIndex(Table.TypeDef);
				dictionary.Add(num5, new Row<ushort, uint>(num3, num4));
				num2 += 1U;
			}
		}

		// Token: 0x060009EF RID: 2543 RVA: 0x000206A1 File Offset: 0x0001E8A1
		public TypeReference GetTypeDefOrRef(MetadataToken token)
		{
			return (TypeReference)this.LookupToken(token);
		}

		// Token: 0x060009F0 RID: 2544 RVA: 0x000206B0 File Offset: 0x0001E8B0
		public TypeDefinition GetTypeDefinition(uint rid)
		{
			this.InitializeTypeDefinitions();
			TypeDefinition typeDefinition = this.metadata.GetTypeDefinition(rid);
			if (typeDefinition != null)
			{
				return typeDefinition;
			}
			typeDefinition = this.ReadTypeDefinition(rid);
			if (this.module.IsWindowsMetadata())
			{
				WindowsRuntimeProjections.Project(typeDefinition);
			}
			return typeDefinition;
		}

		// Token: 0x060009F1 RID: 2545 RVA: 0x000206F1 File Offset: 0x0001E8F1
		private TypeDefinition ReadTypeDefinition(uint rid)
		{
			if (!this.MoveTo(Table.TypeDef, rid))
			{
				return null;
			}
			return this.ReadType(rid);
		}

		// Token: 0x060009F2 RID: 2546 RVA: 0x00020706 File Offset: 0x0001E906
		private void InitializeTypeReferences()
		{
			if (this.metadata.TypeReferences != null)
			{
				return;
			}
			this.metadata.TypeReferences = new TypeReference[this.image.GetTableLength(Table.TypeRef)];
		}

		// Token: 0x060009F3 RID: 2547 RVA: 0x00020734 File Offset: 0x0001E934
		public TypeReference GetTypeReference(string scope, string full_name)
		{
			this.InitializeTypeReferences();
			int num = this.metadata.TypeReferences.Length;
			uint num2 = 1U;
			while ((ulong)num2 <= (ulong)((long)num))
			{
				TypeReference typeReference = this.GetTypeReference(num2);
				if (!(typeReference.FullName != full_name))
				{
					if (string.IsNullOrEmpty(scope))
					{
						return typeReference;
					}
					if (typeReference.Scope.Name == scope)
					{
						return typeReference;
					}
				}
				num2 += 1U;
			}
			return null;
		}

		// Token: 0x060009F4 RID: 2548 RVA: 0x0002079C File Offset: 0x0001E99C
		private TypeReference GetTypeReference(uint rid)
		{
			this.InitializeTypeReferences();
			TypeReference typeReference = this.metadata.GetTypeReference(rid);
			if (typeReference != null)
			{
				return typeReference;
			}
			return this.ReadTypeReference(rid);
		}

		// Token: 0x060009F5 RID: 2549 RVA: 0x000207C8 File Offset: 0x0001E9C8
		private TypeReference ReadTypeReference(uint rid)
		{
			if (!this.MoveTo(Table.TypeRef, rid))
			{
				return null;
			}
			TypeReference typeReference = null;
			MetadataToken metadataToken = this.ReadMetadataToken(CodedIndex.ResolutionScope);
			string text = this.ReadString();
			TypeReference typeReference2 = new TypeReference(this.ReadString(), text, this.module, null);
			typeReference2.token = new MetadataToken(TokenType.TypeRef, rid);
			this.metadata.AddTypeReference(typeReference2);
			IMetadataScope metadataScope3;
			if (metadataToken.TokenType == TokenType.TypeRef)
			{
				if (metadataToken.RID != rid)
				{
					typeReference = this.GetTypeDefOrRef(metadataToken);
					IMetadataScope metadataScope2;
					if (typeReference == null)
					{
						IMetadataScope metadataScope = this.module;
						metadataScope2 = metadataScope;
					}
					else
					{
						metadataScope2 = typeReference.Scope;
					}
					metadataScope3 = metadataScope2;
				}
				else
				{
					metadataScope3 = this.module;
				}
			}
			else
			{
				metadataScope3 = this.GetTypeReferenceScope(metadataToken);
			}
			typeReference2.scope = metadataScope3;
			typeReference2.DeclaringType = typeReference;
			MetadataSystem.TryProcessPrimitiveTypeReference(typeReference2);
			if (typeReference2.Module.IsWindowsMetadata())
			{
				WindowsRuntimeProjections.Project(typeReference2);
			}
			return typeReference2;
		}

		// Token: 0x060009F6 RID: 2550 RVA: 0x000208A0 File Offset: 0x0001EAA0
		private IMetadataScope GetTypeReferenceScope(MetadataToken scope)
		{
			if (scope.TokenType == TokenType.Module)
			{
				return this.module;
			}
			TokenType tokenType = scope.TokenType;
			IMetadataScope[] array2;
			if (tokenType != TokenType.ModuleRef)
			{
				if (tokenType != TokenType.AssemblyRef)
				{
					throw new NotSupportedException();
				}
				this.InitializeAssemblyReferences();
				IMetadataScope[] array = this.metadata.AssemblyReferences;
				array2 = array;
			}
			else
			{
				this.InitializeModuleReferences();
				IMetadataScope[] array = this.metadata.ModuleReferences;
				array2 = array;
			}
			uint num = scope.RID - 1U;
			if (num < 0U || (ulong)num >= (ulong)((long)array2.Length))
			{
				return null;
			}
			return array2[(int)num];
		}

		// Token: 0x060009F7 RID: 2551 RVA: 0x00020924 File Offset: 0x0001EB24
		public IEnumerable<TypeReference> GetTypeReferences()
		{
			this.InitializeTypeReferences();
			int tableLength = this.image.GetTableLength(Table.TypeRef);
			TypeReference[] array = new TypeReference[tableLength];
			uint num = 1U;
			while ((ulong)num <= (ulong)((long)tableLength))
			{
				array[(int)(num - 1U)] = this.GetTypeReference(num);
				num += 1U;
			}
			return array;
		}

		// Token: 0x060009F8 RID: 2552 RVA: 0x00020968 File Offset: 0x0001EB68
		private TypeReference GetTypeSpecification(uint rid)
		{
			if (!this.MoveTo(Table.TypeSpec, rid))
			{
				return null;
			}
			TypeReference typeReference = this.ReadSignature(this.ReadBlobIndex()).ReadTypeSignature();
			if (typeReference.token.RID == 0U)
			{
				typeReference.token = new MetadataToken(TokenType.TypeSpec, rid);
			}
			return typeReference;
		}

		// Token: 0x060009F9 RID: 2553 RVA: 0x000209B3 File Offset: 0x0001EBB3
		private SignatureReader ReadSignature(uint signature)
		{
			return new SignatureReader(signature, this);
		}

		// Token: 0x060009FA RID: 2554 RVA: 0x000209BC File Offset: 0x0001EBBC
		public bool HasInterfaces(TypeDefinition type)
		{
			this.InitializeInterfaces();
			Collection<Row<uint, MetadataToken>> collection;
			return this.metadata.TryGetInterfaceMapping(type, out collection);
		}

		// Token: 0x060009FB RID: 2555 RVA: 0x000209E0 File Offset: 0x0001EBE0
		public InterfaceImplementationCollection ReadInterfaces(TypeDefinition type)
		{
			this.InitializeInterfaces();
			Collection<Row<uint, MetadataToken>> collection;
			if (!this.metadata.TryGetInterfaceMapping(type, out collection))
			{
				return new InterfaceImplementationCollection(type);
			}
			InterfaceImplementationCollection interfaceImplementationCollection = new InterfaceImplementationCollection(type, collection.Count);
			this.context = type;
			for (int i = 0; i < collection.Count; i++)
			{
				interfaceImplementationCollection.Add(new InterfaceImplementation(this.GetTypeDefOrRef(collection[i].Col2), new MetadataToken(TokenType.InterfaceImpl, collection[i].Col1)));
			}
			return interfaceImplementationCollection;
		}

		// Token: 0x060009FC RID: 2556 RVA: 0x00020A64 File Offset: 0x0001EC64
		private void InitializeInterfaces()
		{
			if (this.metadata.Interfaces != null)
			{
				return;
			}
			int num = this.MoveTo(Table.InterfaceImpl);
			this.metadata.Interfaces = new Dictionary<uint, Collection<Row<uint, MetadataToken>>>(num);
			uint num2 = 1U;
			while ((ulong)num2 <= (ulong)((long)num))
			{
				uint num3 = this.ReadTableIndex(Table.TypeDef);
				MetadataToken metadataToken = this.ReadMetadataToken(CodedIndex.TypeDefOrRef);
				this.AddInterfaceMapping(num3, new Row<uint, MetadataToken>(num2, metadataToken));
				num2 += 1U;
			}
		}

		// Token: 0x060009FD RID: 2557 RVA: 0x00020AC5 File Offset: 0x0001ECC5
		private void AddInterfaceMapping(uint type, Row<uint, MetadataToken> @interface)
		{
			this.metadata.SetInterfaceMapping(type, MetadataReader.AddMapping<uint, Row<uint, MetadataToken>>(this.metadata.Interfaces, type, @interface));
		}

		// Token: 0x060009FE RID: 2558 RVA: 0x00020AE8 File Offset: 0x0001ECE8
		public Collection<FieldDefinition> ReadFields(TypeDefinition type)
		{
			Range fields_range = type.fields_range;
			if (fields_range.Length == 0U)
			{
				return new MemberDefinitionCollection<FieldDefinition>(type);
			}
			MemberDefinitionCollection<FieldDefinition> memberDefinitionCollection = new MemberDefinitionCollection<FieldDefinition>(type, (int)fields_range.Length);
			this.context = type;
			if (!this.MoveTo(Table.FieldPtr, fields_range.Start))
			{
				if (!this.MoveTo(Table.Field, fields_range.Start))
				{
					return memberDefinitionCollection;
				}
				for (uint num = 0U; num < fields_range.Length; num += 1U)
				{
					this.ReadField(fields_range.Start + num, memberDefinitionCollection);
				}
			}
			else
			{
				this.ReadPointers<FieldDefinition>(Table.FieldPtr, Table.Field, fields_range, memberDefinitionCollection, new Action<uint, Collection<FieldDefinition>>(this.ReadField));
			}
			return memberDefinitionCollection;
		}

		// Token: 0x060009FF RID: 2559 RVA: 0x00020B78 File Offset: 0x0001ED78
		private void ReadField(uint field_rid, Collection<FieldDefinition> fields)
		{
			FieldAttributes fieldAttributes = (FieldAttributes)base.ReadUInt16();
			string text = this.ReadString();
			uint num = this.ReadBlobIndex();
			FieldDefinition fieldDefinition = new FieldDefinition(text, fieldAttributes, this.ReadFieldType(num));
			fieldDefinition.token = new MetadataToken(TokenType.Field, field_rid);
			this.metadata.AddFieldDefinition(fieldDefinition);
			if (MetadataReader.IsDeleted(fieldDefinition))
			{
				return;
			}
			fields.Add(fieldDefinition);
			if (this.module.IsWindowsMetadata())
			{
				WindowsRuntimeProjections.Project(fieldDefinition);
			}
		}

		// Token: 0x06000A00 RID: 2560 RVA: 0x00020BE7 File Offset: 0x0001EDE7
		private void InitializeFields()
		{
			if (this.metadata.Fields != null)
			{
				return;
			}
			this.metadata.Fields = new FieldDefinition[this.image.GetTableLength(Table.Field)];
		}

		// Token: 0x06000A01 RID: 2561 RVA: 0x00020C13 File Offset: 0x0001EE13
		private TypeReference ReadFieldType(uint signature)
		{
			SignatureReader signatureReader = this.ReadSignature(signature);
			if (signatureReader.ReadByte() != 6)
			{
				throw new NotSupportedException();
			}
			return signatureReader.ReadTypeSignature();
		}

		// Token: 0x06000A02 RID: 2562 RVA: 0x00020C30 File Offset: 0x0001EE30
		public int ReadFieldRVA(FieldDefinition field)
		{
			this.InitializeFieldRVAs();
			uint rid = field.token.RID;
			uint num;
			if (!this.metadata.FieldRVAs.TryGetValue(rid, out num))
			{
				return 0;
			}
			int fieldTypeSize = MetadataReader.GetFieldTypeSize(field.FieldType);
			if (fieldTypeSize == 0 || num == 0U)
			{
				return 0;
			}
			this.metadata.FieldRVAs.Remove(rid);
			field.InitialValue = this.GetFieldInitializeValue(fieldTypeSize, num);
			return (int)num;
		}

		// Token: 0x06000A03 RID: 2563 RVA: 0x00020C9B File Offset: 0x0001EE9B
		private byte[] GetFieldInitializeValue(int size, uint rva)
		{
			return this.image.GetReaderAt<int, byte[]>(rva, size, (int s, BinaryStreamReader reader) => reader.ReadBytes(s)) ?? Empty<byte>.Array;
		}

		// Token: 0x06000A04 RID: 2564 RVA: 0x00020CD4 File Offset: 0x0001EED4
		private static int GetFieldTypeSize(TypeReference type)
		{
			int num = 0;
			switch (type.etype)
			{
			case ElementType.Boolean:
			case ElementType.I1:
			case ElementType.U1:
				return 1;
			case ElementType.Char:
			case ElementType.I2:
			case ElementType.U2:
				return 2;
			case ElementType.I4:
			case ElementType.U4:
			case ElementType.R4:
				return 4;
			case ElementType.I8:
			case ElementType.U8:
			case ElementType.R8:
				return 8;
			case ElementType.Ptr:
			case ElementType.FnPtr:
				return IntPtr.Size;
			case ElementType.CModReqD:
			case ElementType.CModOpt:
				return MetadataReader.GetFieldTypeSize(((IModifierType)type).ElementType);
			}
			TypeDefinition typeDefinition = type.Resolve();
			if (typeDefinition != null && typeDefinition.HasLayoutInfo)
			{
				num = typeDefinition.ClassSize;
			}
			return num;
		}

		// Token: 0x06000A05 RID: 2565 RVA: 0x00020DB4 File Offset: 0x0001EFB4
		private void InitializeFieldRVAs()
		{
			if (this.metadata.FieldRVAs != null)
			{
				return;
			}
			int num = this.MoveTo(Table.FieldRVA);
			Dictionary<uint, uint> dictionary = (this.metadata.FieldRVAs = new Dictionary<uint, uint>(num));
			for (int i = 0; i < num; i++)
			{
				uint num2 = base.ReadUInt32();
				uint num3 = this.ReadTableIndex(Table.Field);
				dictionary.Add(num3, num2);
			}
		}

		// Token: 0x06000A06 RID: 2566 RVA: 0x00020E14 File Offset: 0x0001F014
		public int ReadFieldLayout(FieldDefinition field)
		{
			this.InitializeFieldLayouts();
			uint rid = field.token.RID;
			uint num;
			if (!this.metadata.FieldLayouts.TryGetValue(rid, out num))
			{
				return -1;
			}
			this.metadata.FieldLayouts.Remove(rid);
			return (int)num;
		}

		// Token: 0x06000A07 RID: 2567 RVA: 0x00020E60 File Offset: 0x0001F060
		private void InitializeFieldLayouts()
		{
			if (this.metadata.FieldLayouts != null)
			{
				return;
			}
			int num = this.MoveTo(Table.FieldLayout);
			Dictionary<uint, uint> dictionary = (this.metadata.FieldLayouts = new Dictionary<uint, uint>(num));
			for (int i = 0; i < num; i++)
			{
				uint num2 = base.ReadUInt32();
				uint num3 = this.ReadTableIndex(Table.Field);
				dictionary.Add(num3, num2);
			}
		}

		// Token: 0x06000A08 RID: 2568 RVA: 0x00020EC0 File Offset: 0x0001F0C0
		public bool HasEvents(TypeDefinition type)
		{
			this.InitializeEvents();
			Range range;
			return this.metadata.TryGetEventsRange(type, out range) && range.Length > 0U;
		}

		// Token: 0x06000A09 RID: 2569 RVA: 0x00020EF0 File Offset: 0x0001F0F0
		public Collection<EventDefinition> ReadEvents(TypeDefinition type)
		{
			this.InitializeEvents();
			Range range;
			if (!this.metadata.TryGetEventsRange(type, out range))
			{
				return new MemberDefinitionCollection<EventDefinition>(type);
			}
			MemberDefinitionCollection<EventDefinition> memberDefinitionCollection = new MemberDefinitionCollection<EventDefinition>(type, (int)range.Length);
			if (range.Length == 0U)
			{
				return memberDefinitionCollection;
			}
			this.context = type;
			if (!this.MoveTo(Table.EventPtr, range.Start))
			{
				if (!this.MoveTo(Table.Event, range.Start))
				{
					return memberDefinitionCollection;
				}
				for (uint num = 0U; num < range.Length; num += 1U)
				{
					this.ReadEvent(range.Start + num, memberDefinitionCollection);
				}
			}
			else
			{
				this.ReadPointers<EventDefinition>(Table.EventPtr, Table.Event, range, memberDefinitionCollection, new Action<uint, Collection<EventDefinition>>(this.ReadEvent));
			}
			return memberDefinitionCollection;
		}

		// Token: 0x06000A0A RID: 2570 RVA: 0x00020F98 File Offset: 0x0001F198
		private void ReadEvent(uint event_rid, Collection<EventDefinition> events)
		{
			EventAttributes eventAttributes = (EventAttributes)base.ReadUInt16();
			string text = this.ReadString();
			TypeReference typeDefOrRef = this.GetTypeDefOrRef(this.ReadMetadataToken(CodedIndex.TypeDefOrRef));
			EventDefinition eventDefinition = new EventDefinition(text, eventAttributes, typeDefOrRef);
			eventDefinition.token = new MetadataToken(TokenType.Event, event_rid);
			if (MetadataReader.IsDeleted(eventDefinition))
			{
				return;
			}
			events.Add(eventDefinition);
		}

		// Token: 0x06000A0B RID: 2571 RVA: 0x00020FEC File Offset: 0x0001F1EC
		private void InitializeEvents()
		{
			if (this.metadata.Events != null)
			{
				return;
			}
			int num = this.MoveTo(Table.EventMap);
			this.metadata.Events = new Dictionary<uint, Range>(num);
			uint num2 = 1U;
			while ((ulong)num2 <= (ulong)((long)num))
			{
				uint num3 = this.ReadTableIndex(Table.TypeDef);
				Range range = this.ReadListRange(num2, Table.EventMap, Table.Event);
				this.metadata.AddEventsRange(num3, range);
				num2 += 1U;
			}
		}

		// Token: 0x06000A0C RID: 2572 RVA: 0x00021050 File Offset: 0x0001F250
		public bool HasProperties(TypeDefinition type)
		{
			this.InitializeProperties();
			Range range;
			return this.metadata.TryGetPropertiesRange(type, out range) && range.Length > 0U;
		}

		// Token: 0x06000A0D RID: 2573 RVA: 0x00021080 File Offset: 0x0001F280
		public Collection<PropertyDefinition> ReadProperties(TypeDefinition type)
		{
			this.InitializeProperties();
			Range range;
			if (!this.metadata.TryGetPropertiesRange(type, out range))
			{
				return new MemberDefinitionCollection<PropertyDefinition>(type);
			}
			MemberDefinitionCollection<PropertyDefinition> memberDefinitionCollection = new MemberDefinitionCollection<PropertyDefinition>(type, (int)range.Length);
			if (range.Length == 0U)
			{
				return memberDefinitionCollection;
			}
			this.context = type;
			if (!this.MoveTo(Table.PropertyPtr, range.Start))
			{
				if (!this.MoveTo(Table.Property, range.Start))
				{
					return memberDefinitionCollection;
				}
				for (uint num = 0U; num < range.Length; num += 1U)
				{
					this.ReadProperty(range.Start + num, memberDefinitionCollection);
				}
			}
			else
			{
				this.ReadPointers<PropertyDefinition>(Table.PropertyPtr, Table.Property, range, memberDefinitionCollection, new Action<uint, Collection<PropertyDefinition>>(this.ReadProperty));
			}
			return memberDefinitionCollection;
		}

		// Token: 0x06000A0E RID: 2574 RVA: 0x00021128 File Offset: 0x0001F328
		private void ReadProperty(uint property_rid, Collection<PropertyDefinition> properties)
		{
			PropertyAttributes propertyAttributes = (PropertyAttributes)base.ReadUInt16();
			string text = this.ReadString();
			uint num = this.ReadBlobIndex();
			SignatureReader signatureReader = this.ReadSignature(num);
			byte b = signatureReader.ReadByte();
			if ((b & 8) == 0)
			{
				throw new NotSupportedException();
			}
			bool flag = (b & 32) > 0;
			signatureReader.ReadCompressedUInt32();
			PropertyDefinition propertyDefinition = new PropertyDefinition(text, propertyAttributes, signatureReader.ReadTypeSignature());
			propertyDefinition.HasThis = flag;
			propertyDefinition.token = new MetadataToken(TokenType.Property, property_rid);
			if (MetadataReader.IsDeleted(propertyDefinition))
			{
				return;
			}
			properties.Add(propertyDefinition);
		}

		// Token: 0x06000A0F RID: 2575 RVA: 0x000211AC File Offset: 0x0001F3AC
		private void InitializeProperties()
		{
			if (this.metadata.Properties != null)
			{
				return;
			}
			int num = this.MoveTo(Table.PropertyMap);
			this.metadata.Properties = new Dictionary<uint, Range>(num);
			uint num2 = 1U;
			while ((ulong)num2 <= (ulong)((long)num))
			{
				uint num3 = this.ReadTableIndex(Table.TypeDef);
				Range range = this.ReadListRange(num2, Table.PropertyMap, Table.Property);
				this.metadata.AddPropertiesRange(num3, range);
				num2 += 1U;
			}
		}

		// Token: 0x06000A10 RID: 2576 RVA: 0x00021210 File Offset: 0x0001F410
		private MethodSemanticsAttributes ReadMethodSemantics(MethodDefinition method)
		{
			this.InitializeMethodSemantics();
			Row<MethodSemanticsAttributes, MetadataToken> row;
			if (!this.metadata.Semantics.TryGetValue(method.token.RID, out row))
			{
				return MethodSemanticsAttributes.None;
			}
			TypeDefinition declaringType = method.DeclaringType;
			MethodSemanticsAttributes col = row.Col1;
			if (col <= MethodSemanticsAttributes.AddOn)
			{
				switch (col)
				{
				case MethodSemanticsAttributes.Setter:
					MetadataReader.GetProperty(declaringType, row.Col2).set_method = method;
					goto IL_016B;
				case MethodSemanticsAttributes.Getter:
					MetadataReader.GetProperty(declaringType, row.Col2).get_method = method;
					goto IL_016B;
				case MethodSemanticsAttributes.Setter | MethodSemanticsAttributes.Getter:
					break;
				case MethodSemanticsAttributes.Other:
				{
					TokenType tokenType = row.Col2.TokenType;
					if (tokenType == TokenType.Event)
					{
						EventDefinition @event = MetadataReader.GetEvent(declaringType, row.Col2);
						if (@event.other_methods == null)
						{
							@event.other_methods = new Collection<MethodDefinition>();
						}
						@event.other_methods.Add(method);
						goto IL_016B;
					}
					if (tokenType != TokenType.Property)
					{
						throw new NotSupportedException();
					}
					PropertyDefinition property = MetadataReader.GetProperty(declaringType, row.Col2);
					if (property.other_methods == null)
					{
						property.other_methods = new Collection<MethodDefinition>();
					}
					property.other_methods.Add(method);
					goto IL_016B;
				}
				default:
					if (col == MethodSemanticsAttributes.AddOn)
					{
						MetadataReader.GetEvent(declaringType, row.Col2).add_method = method;
						goto IL_016B;
					}
					break;
				}
			}
			else
			{
				if (col == MethodSemanticsAttributes.RemoveOn)
				{
					MetadataReader.GetEvent(declaringType, row.Col2).remove_method = method;
					goto IL_016B;
				}
				if (col == MethodSemanticsAttributes.Fire)
				{
					MetadataReader.GetEvent(declaringType, row.Col2).invoke_method = method;
					goto IL_016B;
				}
			}
			throw new NotSupportedException();
			IL_016B:
			this.metadata.Semantics.Remove(method.token.RID);
			return row.Col1;
		}

		// Token: 0x06000A11 RID: 2577 RVA: 0x000213AA File Offset: 0x0001F5AA
		private static EventDefinition GetEvent(TypeDefinition type, MetadataToken token)
		{
			if (token.TokenType != TokenType.Event)
			{
				throw new ArgumentException();
			}
			return MetadataReader.GetMember<EventDefinition>(type.Events, token);
		}

		// Token: 0x06000A12 RID: 2578 RVA: 0x000213CC File Offset: 0x0001F5CC
		private static PropertyDefinition GetProperty(TypeDefinition type, MetadataToken token)
		{
			if (token.TokenType != TokenType.Property)
			{
				throw new ArgumentException();
			}
			return MetadataReader.GetMember<PropertyDefinition>(type.Properties, token);
		}

		// Token: 0x06000A13 RID: 2579 RVA: 0x000213F0 File Offset: 0x0001F5F0
		private static TMember GetMember<TMember>(Collection<TMember> members, MetadataToken token) where TMember : IMemberDefinition
		{
			for (int i = 0; i < members.Count; i++)
			{
				TMember tmember = members[i];
				if (tmember.MetadataToken == token)
				{
					return tmember;
				}
			}
			throw new ArgumentException();
		}

		// Token: 0x06000A14 RID: 2580 RVA: 0x00021434 File Offset: 0x0001F634
		private void InitializeMethodSemantics()
		{
			if (this.metadata.Semantics != null)
			{
				return;
			}
			int num = this.MoveTo(Table.MethodSemantics);
			Dictionary<uint, Row<MethodSemanticsAttributes, MetadataToken>> dictionary = (this.metadata.Semantics = new Dictionary<uint, Row<MethodSemanticsAttributes, MetadataToken>>(0));
			uint num2 = 0U;
			while ((ulong)num2 < (ulong)((long)num))
			{
				MethodSemanticsAttributes methodSemanticsAttributes = (MethodSemanticsAttributes)base.ReadUInt16();
				uint num3 = this.ReadTableIndex(Table.Method);
				MetadataToken metadataToken = this.ReadMetadataToken(CodedIndex.HasSemantics);
				dictionary[num3] = new Row<MethodSemanticsAttributes, MetadataToken>(methodSemanticsAttributes, metadataToken);
				num2 += 1U;
			}
		}

		// Token: 0x06000A15 RID: 2581 RVA: 0x000214A6 File Offset: 0x0001F6A6
		public void ReadMethods(PropertyDefinition property)
		{
			this.ReadAllSemantics(property.DeclaringType);
		}

		// Token: 0x06000A16 RID: 2582 RVA: 0x000214B4 File Offset: 0x0001F6B4
		public void ReadMethods(EventDefinition @event)
		{
			this.ReadAllSemantics(@event.DeclaringType);
		}

		// Token: 0x06000A17 RID: 2583 RVA: 0x000214C2 File Offset: 0x0001F6C2
		public void ReadAllSemantics(MethodDefinition method)
		{
			this.ReadAllSemantics(method.DeclaringType);
		}

		// Token: 0x06000A18 RID: 2584 RVA: 0x000214D0 File Offset: 0x0001F6D0
		private void ReadAllSemantics(TypeDefinition type)
		{
			Collection<MethodDefinition> methods = type.Methods;
			for (int i = 0; i < methods.Count; i++)
			{
				MethodDefinition methodDefinition = methods[i];
				if (!methodDefinition.sem_attrs_ready)
				{
					methodDefinition.sem_attrs = this.ReadMethodSemantics(methodDefinition);
					methodDefinition.sem_attrs_ready = true;
				}
			}
		}

		// Token: 0x06000A19 RID: 2585 RVA: 0x00021520 File Offset: 0x0001F720
		public Collection<MethodDefinition> ReadMethods(TypeDefinition type)
		{
			Range methods_range = type.methods_range;
			if (methods_range.Length == 0U)
			{
				return new MemberDefinitionCollection<MethodDefinition>(type);
			}
			MemberDefinitionCollection<MethodDefinition> memberDefinitionCollection = new MemberDefinitionCollection<MethodDefinition>(type, (int)methods_range.Length);
			if (!this.MoveTo(Table.MethodPtr, methods_range.Start))
			{
				if (!this.MoveTo(Table.Method, methods_range.Start))
				{
					return memberDefinitionCollection;
				}
				for (uint num = 0U; num < methods_range.Length; num += 1U)
				{
					this.ReadMethod(methods_range.Start + num, memberDefinitionCollection);
				}
			}
			else
			{
				this.ReadPointers<MethodDefinition>(Table.MethodPtr, Table.Method, methods_range, memberDefinitionCollection, new Action<uint, Collection<MethodDefinition>>(this.ReadMethod));
			}
			return memberDefinitionCollection;
		}

		// Token: 0x06000A1A RID: 2586 RVA: 0x000215AC File Offset: 0x0001F7AC
		private void ReadPointers<TMember>(Table ptr, Table table, Range range, Collection<TMember> members, Action<uint, Collection<TMember>> reader) where TMember : IMemberDefinition
		{
			for (uint num = 0U; num < range.Length; num += 1U)
			{
				this.MoveTo(ptr, range.Start + num);
				uint num2 = this.ReadTableIndex(table);
				this.MoveTo(table, num2);
				reader(num2, members);
			}
		}

		// Token: 0x06000A1B RID: 2587 RVA: 0x000215F5 File Offset: 0x0001F7F5
		private static bool IsDeleted(IMemberDefinition member)
		{
			return member.IsSpecialName && member.Name == "_Deleted";
		}

		// Token: 0x06000A1C RID: 2588 RVA: 0x00021611 File Offset: 0x0001F811
		private void InitializeMethods()
		{
			if (this.metadata.Methods != null)
			{
				return;
			}
			this.metadata.Methods = new MethodDefinition[this.image.GetTableLength(Table.Method)];
		}

		// Token: 0x06000A1D RID: 2589 RVA: 0x00021640 File Offset: 0x0001F840
		private void ReadMethod(uint method_rid, Collection<MethodDefinition> methods)
		{
			MethodDefinition methodDefinition = new MethodDefinition();
			methodDefinition.rva = base.ReadUInt32();
			methodDefinition.ImplAttributes = (MethodImplAttributes)base.ReadUInt16();
			methodDefinition.Attributes = (MethodAttributes)base.ReadUInt16();
			methodDefinition.Name = this.ReadString();
			methodDefinition.token = new MetadataToken(TokenType.Method, method_rid);
			if (MetadataReader.IsDeleted(methodDefinition))
			{
				return;
			}
			methods.Add(methodDefinition);
			uint num = this.ReadBlobIndex();
			Range range = this.ReadListRange(method_rid, Table.Method, Table.Param);
			this.context = methodDefinition;
			this.ReadMethodSignature(num, methodDefinition);
			this.metadata.AddMethodDefinition(methodDefinition);
			if (range.Length != 0U)
			{
				int position = this.position;
				this.ReadParameters(methodDefinition, range);
				this.position = position;
			}
			if (this.module.IsWindowsMetadata())
			{
				WindowsRuntimeProjections.Project(methodDefinition);
			}
		}

		// Token: 0x06000A1E RID: 2590 RVA: 0x00021704 File Offset: 0x0001F904
		private void ReadParameters(MethodDefinition method, Range param_range)
		{
			if (this.MoveTo(Table.ParamPtr, param_range.Start))
			{
				this.ReadParameterPointers(method, param_range);
				return;
			}
			if (!this.MoveTo(Table.Param, param_range.Start))
			{
				return;
			}
			for (uint num = 0U; num < param_range.Length; num += 1U)
			{
				this.ReadParameter(param_range.Start + num, method);
			}
		}

		// Token: 0x06000A1F RID: 2591 RVA: 0x0002175C File Offset: 0x0001F95C
		private void ReadParameterPointers(MethodDefinition method, Range range)
		{
			for (uint num = 0U; num < range.Length; num += 1U)
			{
				this.MoveTo(Table.ParamPtr, range.Start + num);
				uint num2 = this.ReadTableIndex(Table.Param);
				this.MoveTo(Table.Param, num2);
				this.ReadParameter(num2, method);
			}
		}

		// Token: 0x06000A20 RID: 2592 RVA: 0x000217A4 File Offset: 0x0001F9A4
		private void ReadParameter(uint param_rid, MethodDefinition method)
		{
			ParameterAttributes parameterAttributes = (ParameterAttributes)base.ReadUInt16();
			ushort num = base.ReadUInt16();
			string text = this.ReadString();
			ParameterDefinition parameterDefinition = ((num == 0) ? method.MethodReturnType.Parameter : method.Parameters[(int)(num - 1)]);
			parameterDefinition.token = new MetadataToken(TokenType.Param, param_rid);
			parameterDefinition.Name = text;
			parameterDefinition.Attributes = parameterAttributes;
		}

		// Token: 0x06000A21 RID: 2593 RVA: 0x00021802 File Offset: 0x0001FA02
		private void ReadMethodSignature(uint signature, IMethodSignature method)
		{
			this.ReadSignature(signature).ReadMethodSignature(method);
		}

		// Token: 0x06000A22 RID: 2594 RVA: 0x00021814 File Offset: 0x0001FA14
		public PInvokeInfo ReadPInvokeInfo(MethodDefinition method)
		{
			this.InitializePInvokes();
			uint rid = method.token.RID;
			Row<PInvokeAttributes, uint, uint> row;
			if (!this.metadata.PInvokes.TryGetValue(rid, out row))
			{
				return null;
			}
			this.metadata.PInvokes.Remove(rid);
			return new PInvokeInfo(row.Col1, this.image.StringHeap.Read(row.Col2), this.module.ModuleReferences[(int)(row.Col3 - 1U)]);
		}

		// Token: 0x06000A23 RID: 2595 RVA: 0x00021898 File Offset: 0x0001FA98
		private void InitializePInvokes()
		{
			if (this.metadata.PInvokes != null)
			{
				return;
			}
			int num = this.MoveTo(Table.ImplMap);
			Dictionary<uint, Row<PInvokeAttributes, uint, uint>> dictionary = (this.metadata.PInvokes = new Dictionary<uint, Row<PInvokeAttributes, uint, uint>>(num));
			for (int i = 1; i <= num; i++)
			{
				PInvokeAttributes pinvokeAttributes = (PInvokeAttributes)base.ReadUInt16();
				MetadataToken metadataToken = this.ReadMetadataToken(CodedIndex.MemberForwarded);
				uint num2 = this.ReadStringIndex();
				uint num3 = this.ReadTableIndex(Table.File);
				if (metadataToken.TokenType == TokenType.Method)
				{
					dictionary.Add(metadataToken.RID, new Row<PInvokeAttributes, uint, uint>(pinvokeAttributes, num2, num3));
				}
			}
		}

		// Token: 0x06000A24 RID: 2596 RVA: 0x00021928 File Offset: 0x0001FB28
		public bool HasGenericParameters(IGenericParameterProvider provider)
		{
			this.InitializeGenericParameters();
			Range[] array;
			return this.metadata.TryGetGenericParameterRanges(provider, out array) && MetadataReader.RangesSize(array) > 0;
		}

		// Token: 0x06000A25 RID: 2597 RVA: 0x00021958 File Offset: 0x0001FB58
		public Collection<GenericParameter> ReadGenericParameters(IGenericParameterProvider provider)
		{
			this.InitializeGenericParameters();
			Range[] array;
			if (!this.metadata.TryGetGenericParameterRanges(provider, out array))
			{
				return new GenericParameterCollection(provider);
			}
			GenericParameterCollection genericParameterCollection = new GenericParameterCollection(provider, MetadataReader.RangesSize(array));
			for (int i = 0; i < array.Length; i++)
			{
				this.ReadGenericParametersRange(array[i], provider, genericParameterCollection);
			}
			return genericParameterCollection;
		}

		// Token: 0x06000A26 RID: 2598 RVA: 0x000219B0 File Offset: 0x0001FBB0
		private void ReadGenericParametersRange(Range range, IGenericParameterProvider provider, GenericParameterCollection generic_parameters)
		{
			if (!this.MoveTo(Table.GenericParam, range.Start))
			{
				return;
			}
			for (uint num = 0U; num < range.Length; num += 1U)
			{
				base.ReadUInt16();
				GenericParameterAttributes genericParameterAttributes = (GenericParameterAttributes)base.ReadUInt16();
				this.ReadMetadataToken(CodedIndex.TypeOrMethodDef);
				generic_parameters.Add(new GenericParameter(this.ReadString(), provider)
				{
					token = new MetadataToken(TokenType.GenericParam, range.Start + num),
					Attributes = genericParameterAttributes
				});
			}
		}

		// Token: 0x06000A27 RID: 2599 RVA: 0x00021A29 File Offset: 0x0001FC29
		private void InitializeGenericParameters()
		{
			if (this.metadata.GenericParameters != null)
			{
				return;
			}
			this.metadata.GenericParameters = this.InitializeRanges(Table.GenericParam, delegate
			{
				base.Advance(4);
				MetadataToken metadataToken = this.ReadMetadataToken(CodedIndex.TypeOrMethodDef);
				this.ReadStringIndex();
				return metadataToken;
			});
		}

		// Token: 0x06000A28 RID: 2600 RVA: 0x00021A58 File Offset: 0x0001FC58
		private Dictionary<MetadataToken, Range[]> InitializeRanges(Table table, Func<MetadataToken> get_next)
		{
			int num = this.MoveTo(table);
			Dictionary<MetadataToken, Range[]> dictionary = new Dictionary<MetadataToken, Range[]>(num);
			if (num == 0)
			{
				return dictionary;
			}
			MetadataToken metadataToken = MetadataToken.Zero;
			Range range = new Range(1U, 0U);
			uint num2 = 1U;
			while ((ulong)num2 <= (ulong)((long)num))
			{
				MetadataToken metadataToken2 = get_next();
				if (num2 == 1U)
				{
					metadataToken = metadataToken2;
					range.Length += 1U;
				}
				else if (metadataToken2 != metadataToken)
				{
					MetadataReader.AddRange(dictionary, metadataToken, range);
					range = new Range(num2, 1U);
					metadataToken = metadataToken2;
				}
				else
				{
					range.Length += 1U;
				}
				num2 += 1U;
			}
			MetadataReader.AddRange(dictionary, metadataToken, range);
			return dictionary;
		}

		// Token: 0x06000A29 RID: 2601 RVA: 0x00021AF0 File Offset: 0x0001FCF0
		private static void AddRange(Dictionary<MetadataToken, Range[]> ranges, MetadataToken owner, Range range)
		{
			if (owner.RID == 0U)
			{
				return;
			}
			Range[] array;
			if (!ranges.TryGetValue(owner, out array))
			{
				ranges.Add(owner, new Range[] { range });
				return;
			}
			ranges[owner] = array.Add(range);
		}

		// Token: 0x06000A2A RID: 2602 RVA: 0x00021B38 File Offset: 0x0001FD38
		public bool HasGenericConstraints(GenericParameter generic_parameter)
		{
			this.InitializeGenericConstraints();
			Collection<Row<uint, MetadataToken>> collection;
			return this.metadata.TryGetGenericConstraintMapping(generic_parameter, out collection) && collection.Count > 0;
		}

		// Token: 0x06000A2B RID: 2603 RVA: 0x00021B68 File Offset: 0x0001FD68
		public GenericParameterConstraintCollection ReadGenericConstraints(GenericParameter generic_parameter)
		{
			this.InitializeGenericConstraints();
			Collection<Row<uint, MetadataToken>> collection;
			if (!this.metadata.TryGetGenericConstraintMapping(generic_parameter, out collection))
			{
				return new GenericParameterConstraintCollection(generic_parameter);
			}
			GenericParameterConstraintCollection genericParameterConstraintCollection = new GenericParameterConstraintCollection(generic_parameter, collection.Count);
			this.context = (IGenericContext)generic_parameter.Owner;
			for (int i = 0; i < collection.Count; i++)
			{
				genericParameterConstraintCollection.Add(new GenericParameterConstraint(this.GetTypeDefOrRef(collection[i].Col2), new MetadataToken(TokenType.GenericParamConstraint, collection[i].Col1)));
			}
			return genericParameterConstraintCollection;
		}

		// Token: 0x06000A2C RID: 2604 RVA: 0x00021BF8 File Offset: 0x0001FDF8
		private void InitializeGenericConstraints()
		{
			if (this.metadata.GenericConstraints != null)
			{
				return;
			}
			int num = this.MoveTo(Table.GenericParamConstraint);
			this.metadata.GenericConstraints = new Dictionary<uint, Collection<Row<uint, MetadataToken>>>(num);
			uint num2 = 1U;
			while ((ulong)num2 <= (ulong)((long)num))
			{
				this.AddGenericConstraintMapping(this.ReadTableIndex(Table.GenericParam), new Row<uint, MetadataToken>(num2, this.ReadMetadataToken(CodedIndex.TypeDefOrRef)));
				num2 += 1U;
			}
		}

		// Token: 0x06000A2D RID: 2605 RVA: 0x00021C56 File Offset: 0x0001FE56
		private void AddGenericConstraintMapping(uint generic_parameter, Row<uint, MetadataToken> constraint)
		{
			this.metadata.SetGenericConstraintMapping(generic_parameter, MetadataReader.AddMapping<uint, Row<uint, MetadataToken>>(this.metadata.GenericConstraints, generic_parameter, constraint));
		}

		// Token: 0x06000A2E RID: 2606 RVA: 0x00021C78 File Offset: 0x0001FE78
		public bool HasOverrides(MethodDefinition method)
		{
			this.InitializeOverrides();
			Collection<MetadataToken> collection;
			return this.metadata.TryGetOverrideMapping(method, out collection) && collection.Count > 0;
		}

		// Token: 0x06000A2F RID: 2607 RVA: 0x00021CA8 File Offset: 0x0001FEA8
		public Collection<MethodReference> ReadOverrides(MethodDefinition method)
		{
			this.InitializeOverrides();
			Collection<MetadataToken> collection;
			if (!this.metadata.TryGetOverrideMapping(method, out collection))
			{
				return new Collection<MethodReference>();
			}
			Collection<MethodReference> collection2 = new Collection<MethodReference>(collection.Count);
			this.context = method;
			for (int i = 0; i < collection.Count; i++)
			{
				collection2.Add((MethodReference)this.LookupToken(collection[i]));
			}
			return collection2;
		}

		// Token: 0x06000A30 RID: 2608 RVA: 0x00021D10 File Offset: 0x0001FF10
		private void InitializeOverrides()
		{
			if (this.metadata.Overrides != null)
			{
				return;
			}
			int num = this.MoveTo(Table.MethodImpl);
			this.metadata.Overrides = new Dictionary<uint, Collection<MetadataToken>>(num);
			for (int i = 1; i <= num; i++)
			{
				this.ReadTableIndex(Table.TypeDef);
				MetadataToken metadataToken = this.ReadMetadataToken(CodedIndex.MethodDefOrRef);
				if (metadataToken.TokenType != TokenType.Method)
				{
					throw new NotSupportedException();
				}
				MetadataToken metadataToken2 = this.ReadMetadataToken(CodedIndex.MethodDefOrRef);
				this.AddOverrideMapping(metadataToken.RID, metadataToken2);
			}
		}

		// Token: 0x06000A31 RID: 2609 RVA: 0x00021D8B File Offset: 0x0001FF8B
		private void AddOverrideMapping(uint method_rid, MetadataToken @override)
		{
			this.metadata.SetOverrideMapping(method_rid, MetadataReader.AddMapping<uint, MetadataToken>(this.metadata.Overrides, method_rid, @override));
		}

		// Token: 0x06000A32 RID: 2610 RVA: 0x00021DAB File Offset: 0x0001FFAB
		public MethodBody ReadMethodBody(MethodDefinition method)
		{
			return this.code.ReadMethodBody(method);
		}

		// Token: 0x06000A33 RID: 2611 RVA: 0x00021DB9 File Offset: 0x0001FFB9
		public int ReadCodeSize(MethodDefinition method)
		{
			return this.code.ReadCodeSize(method);
		}

		// Token: 0x06000A34 RID: 2612 RVA: 0x00021DC8 File Offset: 0x0001FFC8
		public CallSite ReadCallSite(MetadataToken token)
		{
			if (!this.MoveTo(Table.StandAloneSig, token.RID))
			{
				return null;
			}
			uint num = this.ReadBlobIndex();
			CallSite callSite = new CallSite();
			this.ReadMethodSignature(num, callSite);
			callSite.MetadataToken = token;
			return callSite;
		}

		// Token: 0x06000A35 RID: 2613 RVA: 0x00021E08 File Offset: 0x00020008
		public VariableDefinitionCollection ReadVariables(MetadataToken local_var_token, MethodDefinition method = null)
		{
			if (!this.MoveTo(Table.StandAloneSig, local_var_token.RID))
			{
				return null;
			}
			SignatureReader signatureReader = this.ReadSignature(this.ReadBlobIndex());
			if (signatureReader.ReadByte() != 7)
			{
				throw new NotSupportedException();
			}
			uint num = signatureReader.ReadCompressedUInt32();
			if (num == 0U)
			{
				return null;
			}
			VariableDefinitionCollection variableDefinitionCollection = new VariableDefinitionCollection(method, (int)num);
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				variableDefinitionCollection.Add(new VariableDefinition(signatureReader.ReadTypeSignature()));
				num2++;
			}
			return variableDefinitionCollection;
		}

		// Token: 0x06000A36 RID: 2614 RVA: 0x00021E78 File Offset: 0x00020078
		public IMetadataTokenProvider LookupToken(MetadataToken token)
		{
			uint rid = token.RID;
			if (rid == 0U)
			{
				return null;
			}
			if (this.metadata_reader != null)
			{
				return this.metadata_reader.LookupToken(token);
			}
			int position = this.position;
			IGenericContext genericContext = this.context;
			TokenType tokenType = token.TokenType;
			IMetadataTokenProvider metadataTokenProvider;
			if (tokenType <= TokenType.Field)
			{
				if (tokenType == TokenType.TypeRef)
				{
					metadataTokenProvider = this.GetTypeReference(rid);
					goto IL_00D8;
				}
				if (tokenType == TokenType.TypeDef)
				{
					metadataTokenProvider = this.GetTypeDefinition(rid);
					goto IL_00D8;
				}
				if (tokenType == TokenType.Field)
				{
					metadataTokenProvider = this.GetFieldDefinition(rid);
					goto IL_00D8;
				}
			}
			else if (tokenType <= TokenType.MemberRef)
			{
				if (tokenType == TokenType.Method)
				{
					metadataTokenProvider = this.GetMethodDefinition(rid);
					goto IL_00D8;
				}
				if (tokenType == TokenType.MemberRef)
				{
					metadataTokenProvider = this.GetMemberReference(rid);
					goto IL_00D8;
				}
			}
			else
			{
				if (tokenType == TokenType.TypeSpec)
				{
					metadataTokenProvider = this.GetTypeSpecification(rid);
					goto IL_00D8;
				}
				if (tokenType == TokenType.MethodSpec)
				{
					metadataTokenProvider = this.GetMethodSpecification(rid);
					goto IL_00D8;
				}
			}
			return null;
			IL_00D8:
			this.position = position;
			this.context = genericContext;
			return metadataTokenProvider;
		}

		// Token: 0x06000A37 RID: 2615 RVA: 0x00021F6C File Offset: 0x0002016C
		public FieldDefinition GetFieldDefinition(uint rid)
		{
			this.InitializeTypeDefinitions();
			FieldDefinition fieldDefinition = this.metadata.GetFieldDefinition(rid);
			if (fieldDefinition != null)
			{
				return fieldDefinition;
			}
			return this.LookupField(rid);
		}

		// Token: 0x06000A38 RID: 2616 RVA: 0x00021F98 File Offset: 0x00020198
		private FieldDefinition LookupField(uint rid)
		{
			TypeDefinition fieldDeclaringType = this.metadata.GetFieldDeclaringType(rid);
			if (fieldDeclaringType == null)
			{
				return null;
			}
			Mixin.Read(fieldDeclaringType.Fields);
			return this.metadata.GetFieldDefinition(rid);
		}

		// Token: 0x06000A39 RID: 2617 RVA: 0x00021FD0 File Offset: 0x000201D0
		public MethodDefinition GetMethodDefinition(uint rid)
		{
			this.InitializeTypeDefinitions();
			MethodDefinition methodDefinition = this.metadata.GetMethodDefinition(rid);
			if (methodDefinition != null)
			{
				return methodDefinition;
			}
			return this.LookupMethod(rid);
		}

		// Token: 0x06000A3A RID: 2618 RVA: 0x00021FFC File Offset: 0x000201FC
		private MethodDefinition LookupMethod(uint rid)
		{
			TypeDefinition methodDeclaringType = this.metadata.GetMethodDeclaringType(rid);
			if (methodDeclaringType == null)
			{
				return null;
			}
			Mixin.Read(methodDeclaringType.Methods);
			return this.metadata.GetMethodDefinition(rid);
		}

		// Token: 0x06000A3B RID: 2619 RVA: 0x00022034 File Offset: 0x00020234
		private MethodSpecification GetMethodSpecification(uint rid)
		{
			if (!this.MoveTo(Table.MethodSpec, rid))
			{
				return null;
			}
			MethodReference methodReference = (MethodReference)this.LookupToken(this.ReadMetadataToken(CodedIndex.MethodDefOrRef));
			uint num = this.ReadBlobIndex();
			MethodSpecification methodSpecification = this.ReadMethodSpecSignature(num, methodReference);
			methodSpecification.token = new MetadataToken(TokenType.MethodSpec, rid);
			return methodSpecification;
		}

		// Token: 0x06000A3C RID: 2620 RVA: 0x00022084 File Offset: 0x00020284
		private MethodSpecification ReadMethodSpecSignature(uint signature, MethodReference method)
		{
			SignatureReader signatureReader = this.ReadSignature(signature);
			if (signatureReader.ReadByte() != 10)
			{
				throw new NotSupportedException();
			}
			uint num = signatureReader.ReadCompressedUInt32();
			GenericInstanceMethod genericInstanceMethod = new GenericInstanceMethod(method, (int)num);
			signatureReader.ReadGenericInstanceSignature(method, genericInstanceMethod, num);
			return genericInstanceMethod;
		}

		// Token: 0x06000A3D RID: 2621 RVA: 0x000220C0 File Offset: 0x000202C0
		private MemberReference GetMemberReference(uint rid)
		{
			this.InitializeMemberReferences();
			MemberReference memberReference = this.metadata.GetMemberReference(rid);
			if (memberReference != null)
			{
				return memberReference;
			}
			memberReference = this.ReadMemberReference(rid);
			if (memberReference != null && !memberReference.ContainsGenericParameter)
			{
				this.metadata.AddMemberReference(memberReference);
			}
			return memberReference;
		}

		// Token: 0x06000A3E RID: 2622 RVA: 0x00022108 File Offset: 0x00020308
		private MemberReference ReadMemberReference(uint rid)
		{
			if (!this.MoveTo(Table.MemberRef, rid))
			{
				return null;
			}
			MetadataToken metadataToken = this.ReadMetadataToken(CodedIndex.MemberRefParent);
			string text = this.ReadString();
			uint num = this.ReadBlobIndex();
			TokenType tokenType = metadataToken.TokenType;
			MemberReference memberReference;
			if (tokenType <= TokenType.TypeDef)
			{
				if (tokenType != TokenType.TypeRef && tokenType != TokenType.TypeDef)
				{
					goto IL_0073;
				}
			}
			else
			{
				if (tokenType == TokenType.Method)
				{
					memberReference = this.ReadMethodMemberReference(metadataToken, text, num);
					goto IL_0079;
				}
				if (tokenType != TokenType.TypeSpec)
				{
					goto IL_0073;
				}
			}
			memberReference = this.ReadTypeMemberReference(metadataToken, text, num);
			goto IL_0079;
			IL_0073:
			throw new NotSupportedException();
			IL_0079:
			memberReference.token = new MetadataToken(TokenType.MemberRef, rid);
			return memberReference;
		}

		// Token: 0x06000A3F RID: 2623 RVA: 0x000221A0 File Offset: 0x000203A0
		private MemberReference ReadTypeMemberReference(MetadataToken type, string name, uint signature)
		{
			TypeReference typeDefOrRef = this.GetTypeDefOrRef(type);
			if (!typeDefOrRef.IsArray)
			{
				this.context = typeDefOrRef;
			}
			MemberReference memberReference = this.ReadMemberReferenceSignature(signature, typeDefOrRef);
			memberReference.Name = name;
			return memberReference;
		}

		// Token: 0x06000A40 RID: 2624 RVA: 0x000221D4 File Offset: 0x000203D4
		private MemberReference ReadMemberReferenceSignature(uint signature, TypeReference declaring_type)
		{
			SignatureReader signatureReader = this.ReadSignature(signature);
			if (signatureReader.buffer[signatureReader.position] == 6)
			{
				signatureReader.position++;
				return new FieldReference
				{
					DeclaringType = declaring_type,
					FieldType = signatureReader.ReadTypeSignature()
				};
			}
			MethodReference methodReference = new MethodReference();
			methodReference.DeclaringType = declaring_type;
			signatureReader.ReadMethodSignature(methodReference);
			return methodReference;
		}

		// Token: 0x06000A41 RID: 2625 RVA: 0x00022238 File Offset: 0x00020438
		private MemberReference ReadMethodMemberReference(MetadataToken token, string name, uint signature)
		{
			MethodDefinition methodDefinition = this.GetMethodDefinition(token.RID);
			this.context = methodDefinition;
			MemberReference memberReference = this.ReadMemberReferenceSignature(signature, methodDefinition.DeclaringType);
			memberReference.Name = name;
			return memberReference;
		}

		// Token: 0x06000A42 RID: 2626 RVA: 0x0002226E File Offset: 0x0002046E
		private void InitializeMemberReferences()
		{
			if (this.metadata.MemberReferences != null)
			{
				return;
			}
			this.metadata.MemberReferences = new MemberReference[this.image.GetTableLength(Table.MemberRef)];
		}

		// Token: 0x06000A43 RID: 2627 RVA: 0x0002229C File Offset: 0x0002049C
		public IEnumerable<MemberReference> GetMemberReferences()
		{
			this.InitializeMemberReferences();
			int tableLength = this.image.GetTableLength(Table.MemberRef);
			TypeSystem typeSystem = this.module.TypeSystem;
			MethodDefinition methodDefinition = new MethodDefinition(string.Empty, MethodAttributes.Static, typeSystem.Void);
			methodDefinition.DeclaringType = new TypeDefinition(string.Empty, string.Empty, TypeAttributes.Public);
			MemberReference[] array = new MemberReference[tableLength];
			uint num = 1U;
			while ((ulong)num <= (ulong)((long)tableLength))
			{
				this.context = methodDefinition;
				array[(int)(num - 1U)] = this.GetMemberReference(num);
				num += 1U;
			}
			return array;
		}

		// Token: 0x06000A44 RID: 2628 RVA: 0x00022324 File Offset: 0x00020524
		private void InitializeConstants()
		{
			if (this.metadata.Constants != null)
			{
				return;
			}
			int num = this.MoveTo(Table.Constant);
			Dictionary<MetadataToken, Row<ElementType, uint>> dictionary = (this.metadata.Constants = new Dictionary<MetadataToken, Row<ElementType, uint>>(num));
			uint num2 = 1U;
			while ((ulong)num2 <= (ulong)((long)num))
			{
				ElementType elementType = (ElementType)base.ReadUInt16();
				MetadataToken metadataToken = this.ReadMetadataToken(CodedIndex.HasConstant);
				uint num3 = this.ReadBlobIndex();
				dictionary.Add(metadataToken, new Row<ElementType, uint>(elementType, num3));
				num2 += 1U;
			}
		}

		// Token: 0x06000A45 RID: 2629 RVA: 0x00022396 File Offset: 0x00020596
		public TypeReference ReadConstantSignature(MetadataToken token)
		{
			if (token.TokenType != TokenType.Signature)
			{
				throw new NotSupportedException();
			}
			if (token.RID == 0U)
			{
				return null;
			}
			if (!this.MoveTo(Table.StandAloneSig, token.RID))
			{
				return null;
			}
			return this.ReadFieldType(this.ReadBlobIndex());
		}

		// Token: 0x06000A46 RID: 2630 RVA: 0x000223D8 File Offset: 0x000205D8
		public object ReadConstant(IConstantProvider owner)
		{
			this.InitializeConstants();
			Row<ElementType, uint> row;
			if (!this.metadata.Constants.TryGetValue(owner.MetadataToken, out row))
			{
				return Mixin.NoValue;
			}
			this.metadata.Constants.Remove(owner.MetadataToken);
			return this.ReadConstantValue(row.Col1, row.Col2);
		}

		// Token: 0x06000A47 RID: 2631 RVA: 0x00022434 File Offset: 0x00020634
		private object ReadConstantValue(ElementType etype, uint signature)
		{
			if (etype == ElementType.String)
			{
				return this.ReadConstantString(signature);
			}
			if (etype == ElementType.Class || etype == ElementType.Object)
			{
				return null;
			}
			return this.ReadConstantPrimitive(etype, signature);
		}

		// Token: 0x06000A48 RID: 2632 RVA: 0x00022458 File Offset: 0x00020658
		private string ReadConstantString(uint signature)
		{
			byte[] array;
			int num;
			int num2;
			this.GetBlobView(signature, out array, out num, out num2);
			if (num2 == 0)
			{
				return string.Empty;
			}
			if ((num2 & 1) == 1)
			{
				num2--;
			}
			return Encoding.Unicode.GetString(array, num, num2);
		}

		// Token: 0x06000A49 RID: 2633 RVA: 0x00022492 File Offset: 0x00020692
		private object ReadConstantPrimitive(ElementType type, uint signature)
		{
			return this.ReadSignature(signature).ReadConstantSignature(type);
		}

		// Token: 0x06000A4A RID: 2634 RVA: 0x000224A1 File Offset: 0x000206A1
		internal void InitializeCustomAttributes()
		{
			if (this.metadata.CustomAttributes != null)
			{
				return;
			}
			this.metadata.CustomAttributes = this.InitializeRanges(Table.CustomAttribute, delegate
			{
				MetadataToken metadataToken = this.ReadMetadataToken(CodedIndex.HasCustomAttribute);
				this.ReadMetadataToken(CodedIndex.CustomAttributeType);
				this.ReadBlobIndex();
				return metadataToken;
			});
		}

		// Token: 0x06000A4B RID: 2635 RVA: 0x000224D0 File Offset: 0x000206D0
		public bool HasCustomAttributes(ICustomAttributeProvider owner)
		{
			this.InitializeCustomAttributes();
			Range[] array;
			return this.metadata.TryGetCustomAttributeRanges(owner, out array) && MetadataReader.RangesSize(array) > 0;
		}

		// Token: 0x06000A4C RID: 2636 RVA: 0x00022500 File Offset: 0x00020700
		public Collection<CustomAttribute> ReadCustomAttributes(ICustomAttributeProvider owner)
		{
			this.InitializeCustomAttributes();
			Range[] array;
			if (!this.metadata.TryGetCustomAttributeRanges(owner, out array))
			{
				return new Collection<CustomAttribute>();
			}
			Collection<CustomAttribute> collection = new Collection<CustomAttribute>(MetadataReader.RangesSize(array));
			for (int i = 0; i < array.Length; i++)
			{
				this.ReadCustomAttributeRange(array[i], collection);
			}
			if (this.module.IsWindowsMetadata())
			{
				foreach (CustomAttribute customAttribute in collection)
				{
					WindowsRuntimeProjections.Project(owner, collection, customAttribute);
				}
			}
			return collection;
		}

		// Token: 0x06000A4D RID: 2637 RVA: 0x000225A4 File Offset: 0x000207A4
		private void ReadCustomAttributeRange(Range range, Collection<CustomAttribute> custom_attributes)
		{
			if (!this.MoveTo(Table.CustomAttribute, range.Start))
			{
				return;
			}
			int num = 0;
			while ((long)num < (long)((ulong)range.Length))
			{
				this.ReadMetadataToken(CodedIndex.HasCustomAttribute);
				MethodReference methodReference = (MethodReference)this.LookupToken(this.ReadMetadataToken(CodedIndex.CustomAttributeType));
				uint num2 = this.ReadBlobIndex();
				custom_attributes.Add(new CustomAttribute(num2, methodReference));
				num++;
			}
		}

		// Token: 0x06000A4E RID: 2638 RVA: 0x00022608 File Offset: 0x00020808
		private static int RangesSize(Range[] ranges)
		{
			uint num = 0U;
			for (int i = 0; i < ranges.Length; i++)
			{
				num += ranges[i].Length;
			}
			return (int)num;
		}

		// Token: 0x06000A4F RID: 2639 RVA: 0x00022638 File Offset: 0x00020838
		public IEnumerable<CustomAttribute> GetCustomAttributes()
		{
			this.InitializeTypeDefinitions();
			uint length = this.image.TableHeap[Table.CustomAttribute].Length;
			Collection<CustomAttribute> collection = new Collection<CustomAttribute>((int)length);
			this.ReadCustomAttributeRange(new Range(1U, length), collection);
			return collection;
		}

		// Token: 0x06000A50 RID: 2640 RVA: 0x00022679 File Offset: 0x00020879
		public byte[] ReadCustomAttributeBlob(uint signature)
		{
			return this.ReadBlob(signature);
		}

		// Token: 0x06000A51 RID: 2641 RVA: 0x00022684 File Offset: 0x00020884
		public void ReadCustomAttributeSignature(CustomAttribute attribute)
		{
			SignatureReader signatureReader = this.ReadSignature(attribute.signature);
			if (!signatureReader.CanReadMore())
			{
				return;
			}
			if (signatureReader.ReadUInt16() != 1)
			{
				throw new InvalidOperationException();
			}
			MethodReference constructor = attribute.Constructor;
			if (constructor.HasParameters)
			{
				signatureReader.ReadCustomAttributeConstructorArguments(attribute, constructor.Parameters);
			}
			if (!signatureReader.CanReadMore())
			{
				return;
			}
			ushort num = signatureReader.ReadUInt16();
			if (num == 0)
			{
				return;
			}
			signatureReader.ReadCustomAttributeNamedArguments(num, ref attribute.fields, ref attribute.properties);
		}

		// Token: 0x06000A52 RID: 2642 RVA: 0x000226FC File Offset: 0x000208FC
		private void InitializeMarshalInfos()
		{
			if (this.metadata.FieldMarshals != null)
			{
				return;
			}
			int num = this.MoveTo(Table.FieldMarshal);
			Dictionary<MetadataToken, uint> dictionary = (this.metadata.FieldMarshals = new Dictionary<MetadataToken, uint>(num));
			for (int i = 0; i < num; i++)
			{
				MetadataToken metadataToken = this.ReadMetadataToken(CodedIndex.HasFieldMarshal);
				uint num2 = this.ReadBlobIndex();
				if (metadataToken.RID != 0U)
				{
					dictionary.Add(metadataToken, num2);
				}
			}
		}

		// Token: 0x06000A53 RID: 2643 RVA: 0x00022765 File Offset: 0x00020965
		public bool HasMarshalInfo(IMarshalInfoProvider owner)
		{
			this.InitializeMarshalInfos();
			return this.metadata.FieldMarshals.ContainsKey(owner.MetadataToken);
		}

		// Token: 0x06000A54 RID: 2644 RVA: 0x00022784 File Offset: 0x00020984
		public MarshalInfo ReadMarshalInfo(IMarshalInfoProvider owner)
		{
			this.InitializeMarshalInfos();
			uint num;
			if (!this.metadata.FieldMarshals.TryGetValue(owner.MetadataToken, out num))
			{
				return null;
			}
			SignatureReader signatureReader = this.ReadSignature(num);
			this.metadata.FieldMarshals.Remove(owner.MetadataToken);
			return signatureReader.ReadMarshalInfo();
		}

		// Token: 0x06000A55 RID: 2645 RVA: 0x000227D6 File Offset: 0x000209D6
		private void InitializeSecurityDeclarations()
		{
			if (this.metadata.SecurityDeclarations != null)
			{
				return;
			}
			this.metadata.SecurityDeclarations = this.InitializeRanges(Table.DeclSecurity, delegate
			{
				base.ReadUInt16();
				MetadataToken metadataToken = this.ReadMetadataToken(CodedIndex.HasDeclSecurity);
				this.ReadBlobIndex();
				return metadataToken;
			});
		}

		// Token: 0x06000A56 RID: 2646 RVA: 0x00022808 File Offset: 0x00020A08
		public bool HasSecurityDeclarations(ISecurityDeclarationProvider owner)
		{
			this.InitializeSecurityDeclarations();
			Range[] array;
			return this.metadata.TryGetSecurityDeclarationRanges(owner, out array) && MetadataReader.RangesSize(array) > 0;
		}

		// Token: 0x06000A57 RID: 2647 RVA: 0x00022838 File Offset: 0x00020A38
		public Collection<SecurityDeclaration> ReadSecurityDeclarations(ISecurityDeclarationProvider owner)
		{
			this.InitializeSecurityDeclarations();
			Range[] array;
			if (!this.metadata.TryGetSecurityDeclarationRanges(owner, out array))
			{
				return new Collection<SecurityDeclaration>();
			}
			Collection<SecurityDeclaration> collection = new Collection<SecurityDeclaration>(MetadataReader.RangesSize(array));
			for (int i = 0; i < array.Length; i++)
			{
				this.ReadSecurityDeclarationRange(array[i], collection);
			}
			return collection;
		}

		// Token: 0x06000A58 RID: 2648 RVA: 0x0002288C File Offset: 0x00020A8C
		private void ReadSecurityDeclarationRange(Range range, Collection<SecurityDeclaration> security_declarations)
		{
			if (!this.MoveTo(Table.DeclSecurity, range.Start))
			{
				return;
			}
			int num = 0;
			while ((long)num < (long)((ulong)range.Length))
			{
				SecurityAction securityAction = (SecurityAction)base.ReadUInt16();
				this.ReadMetadataToken(CodedIndex.HasDeclSecurity);
				uint num2 = this.ReadBlobIndex();
				security_declarations.Add(new SecurityDeclaration(securityAction, num2, this.module));
				num++;
			}
		}

		// Token: 0x06000A59 RID: 2649 RVA: 0x00022679 File Offset: 0x00020879
		public byte[] ReadSecurityDeclarationBlob(uint signature)
		{
			return this.ReadBlob(signature);
		}

		// Token: 0x06000A5A RID: 2650 RVA: 0x000228E8 File Offset: 0x00020AE8
		public void ReadSecurityDeclarationSignature(SecurityDeclaration declaration)
		{
			uint signature = declaration.signature;
			SignatureReader signatureReader = this.ReadSignature(signature);
			if (signatureReader.buffer[signatureReader.position] != 46)
			{
				this.ReadXmlSecurityDeclaration(signature, declaration);
				return;
			}
			signatureReader.position++;
			uint num = signatureReader.ReadCompressedUInt32();
			Collection<SecurityAttribute> collection = new Collection<SecurityAttribute>((int)num);
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				collection.Add(signatureReader.ReadSecurityAttribute());
				num2++;
			}
			declaration.security_attributes = collection;
		}

		// Token: 0x06000A5B RID: 2651 RVA: 0x00022960 File Offset: 0x00020B60
		private void ReadXmlSecurityDeclaration(uint signature, SecurityDeclaration declaration)
		{
			declaration.security_attributes = new Collection<SecurityAttribute>(1)
			{
				new SecurityAttribute(this.module.TypeSystem.LookupType("System.Security.Permissions", "PermissionSetAttribute"))
				{
					properties = new Collection<CustomAttributeNamedArgument>(1),
					properties = 
					{
						new CustomAttributeNamedArgument("XML", new CustomAttributeArgument(this.module.TypeSystem.String, this.ReadUnicodeStringBlob(signature)))
					}
				}
			};
		}

		// Token: 0x06000A5C RID: 2652 RVA: 0x000229E0 File Offset: 0x00020BE0
		public Collection<ExportedType> ReadExportedTypes()
		{
			int num = this.MoveTo(Table.ExportedType);
			if (num == 0)
			{
				return new Collection<ExportedType>();
			}
			Collection<ExportedType> collection = new Collection<ExportedType>(num);
			for (int i = 1; i <= num; i++)
			{
				TypeAttributes typeAttributes = (TypeAttributes)base.ReadUInt32();
				uint num2 = base.ReadUInt32();
				string text = this.ReadString();
				string text2 = this.ReadString();
				MetadataToken metadataToken = this.ReadMetadataToken(CodedIndex.Implementation);
				ExportedType exportedType = null;
				IMetadataScope metadataScope = null;
				TokenType tokenType = metadataToken.TokenType;
				if (tokenType != TokenType.AssemblyRef && tokenType != TokenType.File)
				{
					if (tokenType == TokenType.ExportedType)
					{
						exportedType = collection[(int)(metadataToken.RID - 1U)];
					}
				}
				else
				{
					metadataScope = this.GetExportedTypeScope(metadataToken);
				}
				ExportedType exportedType2 = new ExportedType(text2, text, this.module, metadataScope)
				{
					Attributes = typeAttributes,
					Identifier = (int)num2,
					DeclaringType = exportedType
				};
				exportedType2.token = new MetadataToken(TokenType.ExportedType, i);
				collection.Add(exportedType2);
			}
			return collection;
		}

		// Token: 0x06000A5D RID: 2653 RVA: 0x00022AD0 File Offset: 0x00020CD0
		private IMetadataScope GetExportedTypeScope(MetadataToken token)
		{
			int position = this.position;
			TokenType tokenType = token.TokenType;
			IMetadataScope metadataScope;
			if (tokenType != TokenType.AssemblyRef)
			{
				if (tokenType != TokenType.File)
				{
					throw new NotSupportedException();
				}
				this.InitializeModuleReferences();
				metadataScope = this.GetModuleReferenceFromFile(token);
			}
			else
			{
				this.InitializeAssemblyReferences();
				metadataScope = this.metadata.GetAssemblyNameReference(token.RID);
			}
			this.position = position;
			return metadataScope;
		}

		// Token: 0x06000A5E RID: 2654 RVA: 0x00022B38 File Offset: 0x00020D38
		private ModuleReference GetModuleReferenceFromFile(MetadataToken token)
		{
			if (!this.MoveTo(Table.File, token.RID))
			{
				return null;
			}
			base.ReadUInt32();
			string text = this.ReadString();
			Collection<ModuleReference> moduleReferences = this.module.ModuleReferences;
			ModuleReference moduleReference;
			for (int i = 0; i < moduleReferences.Count; i++)
			{
				moduleReference = moduleReferences[i];
				if (moduleReference.Name == text)
				{
					return moduleReference;
				}
			}
			moduleReference = new ModuleReference(text);
			moduleReferences.Add(moduleReference);
			return moduleReference;
		}

		// Token: 0x06000A5F RID: 2655 RVA: 0x00022BAC File Offset: 0x00020DAC
		private void InitializeDocuments()
		{
			if (this.metadata.Documents != null)
			{
				return;
			}
			int num = this.MoveTo(Table.Document);
			Document[] array = (this.metadata.Documents = new Document[num]);
			uint num2 = 1U;
			while ((ulong)num2 <= (ulong)((long)num))
			{
				uint num3 = this.ReadBlobIndex();
				Guid guid = this.ReadGuid();
				byte[] array2 = this.ReadBlob();
				Guid guid2 = this.ReadGuid();
				string text = this.ReadSignature(num3).ReadDocumentName();
				array[(int)(num2 - 1U)] = new Document(text)
				{
					HashAlgorithmGuid = guid,
					Hash = array2,
					LanguageGuid = guid2,
					token = new MetadataToken(TokenType.Document, num2)
				};
				num2 += 1U;
			}
		}

		// Token: 0x06000A60 RID: 2656 RVA: 0x00022C58 File Offset: 0x00020E58
		public Collection<SequencePoint> ReadSequencePoints(MethodDefinition method)
		{
			this.InitializeDocuments();
			if (!this.MoveTo(Table.MethodDebugInformation, method.MetadataToken.RID))
			{
				return new Collection<SequencePoint>(0);
			}
			uint num = this.ReadTableIndex(Table.Document);
			uint num2 = this.ReadBlobIndex();
			if (num2 == 0U)
			{
				return new Collection<SequencePoint>(0);
			}
			Document document = this.GetDocument(num);
			return this.ReadSignature(num2).ReadSequencePoints(document);
		}

		// Token: 0x06000A61 RID: 2657 RVA: 0x00022CBC File Offset: 0x00020EBC
		public Document GetDocument(uint rid)
		{
			Document document = this.metadata.GetDocument(rid);
			if (document == null)
			{
				return null;
			}
			document.custom_infos = this.GetCustomDebugInformation(document);
			return document;
		}

		// Token: 0x06000A62 RID: 2658 RVA: 0x00022CEC File Offset: 0x00020EEC
		private void InitializeLocalScopes()
		{
			if (this.metadata.LocalScopes != null)
			{
				return;
			}
			this.InitializeMethods();
			int num = this.MoveTo(Table.LocalScope);
			this.metadata.LocalScopes = new Dictionary<uint, Collection<Row<uint, Range, Range, uint, uint, uint>>>();
			uint num2 = 1U;
			while ((ulong)num2 <= (ulong)((long)num))
			{
				uint num3 = this.ReadTableIndex(Table.Method);
				uint num4 = this.ReadTableIndex(Table.ImportScope);
				Range range = this.ReadListRange(num2, Table.LocalScope, Table.LocalVariable);
				Range range2 = this.ReadListRange(num2, Table.LocalScope, Table.LocalConstant);
				uint num5 = base.ReadUInt32();
				uint num6 = base.ReadUInt32();
				this.metadata.SetLocalScopes(num3, MetadataReader.AddMapping<uint, Row<uint, Range, Range, uint, uint, uint>>(this.metadata.LocalScopes, num3, new Row<uint, Range, Range, uint, uint, uint>(num4, range, range2, num5, num6, num2)));
				num2 += 1U;
			}
		}

		// Token: 0x06000A63 RID: 2659 RVA: 0x00022D9C File Offset: 0x00020F9C
		public ScopeDebugInformation ReadScope(MethodDefinition method)
		{
			this.InitializeLocalScopes();
			this.InitializeImportScopes();
			Collection<Row<uint, Range, Range, uint, uint, uint>> collection;
			if (!this.metadata.TryGetLocalScopes(method, out collection))
			{
				return null;
			}
			ScopeDebugInformation scopeDebugInformation = null;
			for (int i = 0; i < collection.Count; i++)
			{
				ScopeDebugInformation scopeDebugInformation2 = this.ReadLocalScope(collection[i]);
				if (i == 0)
				{
					scopeDebugInformation = scopeDebugInformation2;
				}
				else if (!MetadataReader.AddScope(scopeDebugInformation.scopes, scopeDebugInformation2))
				{
					scopeDebugInformation.Scopes.Add(scopeDebugInformation2);
				}
			}
			return scopeDebugInformation;
		}

		// Token: 0x06000A64 RID: 2660 RVA: 0x00022E0C File Offset: 0x0002100C
		private static bool AddScope(Collection<ScopeDebugInformation> scopes, ScopeDebugInformation scope)
		{
			if (scopes.IsNullOrEmpty<ScopeDebugInformation>())
			{
				return false;
			}
			foreach (ScopeDebugInformation scopeDebugInformation in scopes)
			{
				if (scopeDebugInformation.HasScopes && MetadataReader.AddScope(scopeDebugInformation.Scopes, scope))
				{
					return true;
				}
				if (scope.Start.Offset >= scopeDebugInformation.Start.Offset && scope.End.Offset <= scopeDebugInformation.End.Offset)
				{
					scopeDebugInformation.Scopes.Add(scope);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000A65 RID: 2661 RVA: 0x00022EC8 File Offset: 0x000210C8
		private ScopeDebugInformation ReadLocalScope(Row<uint, Range, Range, uint, uint, uint> record)
		{
			ScopeDebugInformation scopeDebugInformation = new ScopeDebugInformation
			{
				start = new InstructionOffset((int)record.Col4),
				end = new InstructionOffset((int)(record.Col4 + record.Col5)),
				token = new MetadataToken(TokenType.LocalScope, record.Col6)
			};
			if (record.Col1 > 0U)
			{
				scopeDebugInformation.import = this.metadata.GetImportScope(record.Col1);
			}
			if (record.Col2.Length > 0U)
			{
				scopeDebugInformation.variables = new Collection<VariableDebugInformation>((int)record.Col2.Length);
				for (uint num = 0U; num < record.Col2.Length; num += 1U)
				{
					VariableDebugInformation variableDebugInformation = this.ReadLocalVariable(record.Col2.Start + num);
					if (variableDebugInformation != null)
					{
						scopeDebugInformation.variables.Add(variableDebugInformation);
					}
				}
			}
			if (record.Col3.Length > 0U)
			{
				scopeDebugInformation.constants = new Collection<ConstantDebugInformation>((int)record.Col3.Length);
				for (uint num2 = 0U; num2 < record.Col3.Length; num2 += 1U)
				{
					ConstantDebugInformation constantDebugInformation = this.ReadLocalConstant(record.Col3.Start + num2);
					if (constantDebugInformation != null)
					{
						scopeDebugInformation.constants.Add(constantDebugInformation);
					}
				}
			}
			return scopeDebugInformation;
		}

		// Token: 0x06000A66 RID: 2662 RVA: 0x00022FF8 File Offset: 0x000211F8
		private VariableDebugInformation ReadLocalVariable(uint rid)
		{
			if (!this.MoveTo(Table.LocalVariable, rid))
			{
				return null;
			}
			VariableAttributes variableAttributes = (VariableAttributes)base.ReadUInt16();
			int num = (int)base.ReadUInt16();
			string text = this.ReadString();
			VariableDebugInformation variableDebugInformation = new VariableDebugInformation(num, text)
			{
				Attributes = variableAttributes,
				token = new MetadataToken(TokenType.LocalVariable, rid)
			};
			variableDebugInformation.custom_infos = this.GetCustomDebugInformation(variableDebugInformation);
			return variableDebugInformation;
		}

		// Token: 0x06000A67 RID: 2663 RVA: 0x00023054 File Offset: 0x00021254
		private ConstantDebugInformation ReadLocalConstant(uint rid)
		{
			if (!this.MoveTo(Table.LocalConstant, rid))
			{
				return null;
			}
			string text = this.ReadString();
			SignatureReader signatureReader = this.ReadSignature(this.ReadBlobIndex());
			TypeReference typeReference = signatureReader.ReadTypeSignature();
			object obj;
			if (typeReference.etype == ElementType.String)
			{
				if (!signatureReader.CanReadMore())
				{
					obj = "";
				}
				else if (signatureReader.buffer[signatureReader.position] != 255)
				{
					byte[] array = signatureReader.ReadBytes((int)((ulong)signatureReader.sig_length - (ulong)((long)signatureReader.position - (long)((ulong)signatureReader.start))));
					obj = Encoding.Unicode.GetString(array, 0, array.Length);
				}
				else
				{
					obj = null;
				}
			}
			else if (typeReference.IsTypeOf("System", "Decimal"))
			{
				byte b = signatureReader.ReadByte();
				obj = new decimal(signatureReader.ReadInt32(), signatureReader.ReadInt32(), signatureReader.ReadInt32(), (b & 128) > 0, b & 127);
			}
			else if (typeReference.IsTypeOf("System", "DateTime"))
			{
				obj = new DateTime(signatureReader.ReadInt64());
			}
			else if (typeReference.etype == ElementType.Object || typeReference.etype == ElementType.None || typeReference.etype == ElementType.Class || typeReference.etype == ElementType.Array || typeReference.etype == ElementType.GenericInst)
			{
				obj = null;
			}
			else
			{
				obj = signatureReader.ReadConstantSignature(typeReference.etype);
			}
			ConstantDebugInformation constantDebugInformation = new ConstantDebugInformation(text, typeReference, obj)
			{
				token = new MetadataToken(TokenType.LocalConstant, rid)
			};
			constantDebugInformation.custom_infos = this.GetCustomDebugInformation(constantDebugInformation);
			return constantDebugInformation;
		}

		// Token: 0x06000A68 RID: 2664 RVA: 0x000231D0 File Offset: 0x000213D0
		private void InitializeImportScopes()
		{
			if (this.metadata.ImportScopes != null)
			{
				return;
			}
			int num = this.MoveTo(Table.ImportScope);
			this.metadata.ImportScopes = new ImportDebugInformation[num];
			for (int i = 1; i <= num; i++)
			{
				this.ReadTableIndex(Table.ImportScope);
				ImportDebugInformation importDebugInformation = new ImportDebugInformation();
				importDebugInformation.token = new MetadataToken(TokenType.ImportScope, i);
				SignatureReader signatureReader = this.ReadSignature(this.ReadBlobIndex());
				while (signatureReader.CanReadMore())
				{
					importDebugInformation.Targets.Add(this.ReadImportTarget(signatureReader));
				}
				this.metadata.ImportScopes[i - 1] = importDebugInformation;
			}
			this.MoveTo(Table.ImportScope);
			for (int j = 0; j < num; j++)
			{
				uint num2 = this.ReadTableIndex(Table.ImportScope);
				this.ReadBlobIndex();
				if (num2 != 0U)
				{
					this.metadata.ImportScopes[j].Parent = this.metadata.GetImportScope(num2);
				}
			}
		}

		// Token: 0x06000A69 RID: 2665 RVA: 0x000232B8 File Offset: 0x000214B8
		public string ReadUTF8StringBlob(uint signature)
		{
			return this.ReadStringBlob(signature, Encoding.UTF8);
		}

		// Token: 0x06000A6A RID: 2666 RVA: 0x000232C6 File Offset: 0x000214C6
		private string ReadUnicodeStringBlob(uint signature)
		{
			return this.ReadStringBlob(signature, Encoding.Unicode);
		}

		// Token: 0x06000A6B RID: 2667 RVA: 0x000232D4 File Offset: 0x000214D4
		private string ReadStringBlob(uint signature, Encoding encoding)
		{
			byte[] array;
			int num;
			int num2;
			this.GetBlobView(signature, out array, out num, out num2);
			if (num2 == 0)
			{
				return string.Empty;
			}
			return encoding.GetString(array, num, num2);
		}

		// Token: 0x06000A6C RID: 2668 RVA: 0x00023300 File Offset: 0x00021500
		private ImportTarget ReadImportTarget(SignatureReader signature)
		{
			AssemblyNameReference assemblyNameReference = null;
			string text = null;
			string text2 = null;
			TypeReference typeReference = null;
			ImportTargetKind importTargetKind = (ImportTargetKind)signature.ReadCompressedUInt32();
			switch (importTargetKind)
			{
			case ImportTargetKind.ImportNamespace:
				text = this.ReadUTF8StringBlob(signature.ReadCompressedUInt32());
				break;
			case ImportTargetKind.ImportNamespaceInAssembly:
				assemblyNameReference = this.metadata.GetAssemblyNameReference(signature.ReadCompressedUInt32());
				text = this.ReadUTF8StringBlob(signature.ReadCompressedUInt32());
				break;
			case ImportTargetKind.ImportType:
				typeReference = signature.ReadTypeToken();
				break;
			case ImportTargetKind.ImportXmlNamespaceWithAlias:
				text2 = this.ReadUTF8StringBlob(signature.ReadCompressedUInt32());
				text = this.ReadUTF8StringBlob(signature.ReadCompressedUInt32());
				break;
			case ImportTargetKind.ImportAlias:
				text2 = this.ReadUTF8StringBlob(signature.ReadCompressedUInt32());
				break;
			case ImportTargetKind.DefineAssemblyAlias:
				text2 = this.ReadUTF8StringBlob(signature.ReadCompressedUInt32());
				assemblyNameReference = this.metadata.GetAssemblyNameReference(signature.ReadCompressedUInt32());
				break;
			case ImportTargetKind.DefineNamespaceAlias:
				text2 = this.ReadUTF8StringBlob(signature.ReadCompressedUInt32());
				text = this.ReadUTF8StringBlob(signature.ReadCompressedUInt32());
				break;
			case ImportTargetKind.DefineNamespaceInAssemblyAlias:
				text2 = this.ReadUTF8StringBlob(signature.ReadCompressedUInt32());
				assemblyNameReference = this.metadata.GetAssemblyNameReference(signature.ReadCompressedUInt32());
				text = this.ReadUTF8StringBlob(signature.ReadCompressedUInt32());
				break;
			case ImportTargetKind.DefineTypeAlias:
				text2 = this.ReadUTF8StringBlob(signature.ReadCompressedUInt32());
				typeReference = signature.ReadTypeToken();
				break;
			}
			return new ImportTarget(importTargetKind)
			{
				alias = text2,
				type = typeReference,
				@namespace = text,
				reference = assemblyNameReference
			};
		}

		// Token: 0x06000A6D RID: 2669 RVA: 0x00023464 File Offset: 0x00021664
		private void InitializeStateMachineMethods()
		{
			if (this.metadata.StateMachineMethods != null)
			{
				return;
			}
			int num = this.MoveTo(Table.StateMachineMethod);
			this.metadata.StateMachineMethods = new Dictionary<uint, uint>(num);
			for (int i = 0; i < num; i++)
			{
				this.metadata.StateMachineMethods.Add(this.ReadTableIndex(Table.Method), this.ReadTableIndex(Table.Method));
			}
		}

		// Token: 0x06000A6E RID: 2670 RVA: 0x000234C4 File Offset: 0x000216C4
		public MethodDefinition ReadStateMachineKickoffMethod(MethodDefinition method)
		{
			this.InitializeStateMachineMethods();
			uint num;
			if (!this.metadata.TryGetStateMachineKickOffMethod(method, out num))
			{
				return null;
			}
			return this.GetMethodDefinition(num);
		}

		// Token: 0x06000A6F RID: 2671 RVA: 0x000234F0 File Offset: 0x000216F0
		private void InitializeCustomDebugInformations()
		{
			if (this.metadata.CustomDebugInformations != null)
			{
				return;
			}
			int num = this.MoveTo(Table.CustomDebugInformation);
			this.metadata.CustomDebugInformations = new Dictionary<MetadataToken, Row<Guid, uint, uint>[]>();
			uint num2 = 1U;
			while ((ulong)num2 <= (ulong)((long)num))
			{
				MetadataToken metadataToken = this.ReadMetadataToken(CodedIndex.HasCustomDebugInformation);
				Row<Guid, uint, uint> row = new Row<Guid, uint, uint>(this.ReadGuid(), this.ReadBlobIndex(), num2);
				Row<Guid, uint, uint>[] array;
				this.metadata.CustomDebugInformations.TryGetValue(metadataToken, out array);
				this.metadata.CustomDebugInformations[metadataToken] = array.Add(row);
				num2 += 1U;
			}
		}

		// Token: 0x06000A70 RID: 2672 RVA: 0x0002357C File Offset: 0x0002177C
		public bool HasCustomDebugInformation(ICustomDebugInformationProvider provider)
		{
			this.InitializeCustomDebugInformations();
			Row<Guid, uint, uint>[] array;
			return this.metadata.CustomDebugInformations.TryGetValue(provider.MetadataToken, out array) && array.Length != 0;
		}

		// Token: 0x06000A71 RID: 2673 RVA: 0x000235B0 File Offset: 0x000217B0
		public Collection<CustomDebugInformation> GetCustomDebugInformation(ICustomDebugInformationProvider provider)
		{
			this.InitializeCustomDebugInformations();
			Row<Guid, uint, uint>[] array;
			if (!this.metadata.CustomDebugInformations.TryGetValue(provider.MetadataToken, out array))
			{
				return null;
			}
			Collection<CustomDebugInformation> collection = new Collection<CustomDebugInformation>(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Col1 == StateMachineScopeDebugInformation.KindIdentifier)
				{
					SignatureReader signatureReader = this.ReadSignature(array[i].Col2);
					Collection<StateMachineScope> collection2 = new Collection<StateMachineScope>();
					while (signatureReader.CanReadMore())
					{
						int num = signatureReader.ReadInt32();
						int num2 = num + signatureReader.ReadInt32();
						collection2.Add(new StateMachineScope(num, num2));
					}
					collection.Add(new StateMachineScopeDebugInformation
					{
						scopes = collection2
					});
				}
				else if (array[i].Col1 == AsyncMethodBodyDebugInformation.KindIdentifier)
				{
					SignatureReader signatureReader2 = this.ReadSignature(array[i].Col2);
					int num3 = signatureReader2.ReadInt32() - 1;
					Collection<InstructionOffset> collection3 = new Collection<InstructionOffset>();
					Collection<InstructionOffset> collection4 = new Collection<InstructionOffset>();
					Collection<MethodDefinition> collection5 = new Collection<MethodDefinition>();
					while (signatureReader2.CanReadMore())
					{
						collection3.Add(new InstructionOffset(signatureReader2.ReadInt32()));
						collection4.Add(new InstructionOffset(signatureReader2.ReadInt32()));
						collection5.Add(this.GetMethodDefinition(signatureReader2.ReadCompressedUInt32()));
					}
					collection.Add(new AsyncMethodBodyDebugInformation(num3)
					{
						yields = collection3,
						resumes = collection4,
						resume_methods = collection5
					});
				}
				else if (array[i].Col1 == EmbeddedSourceDebugInformation.KindIdentifier)
				{
					collection.Add(new EmbeddedSourceDebugInformation(array[i].Col2, this));
				}
				else if (array[i].Col1 == SourceLinkDebugInformation.KindIdentifier)
				{
					collection.Add(new SourceLinkDebugInformation(Encoding.UTF8.GetString(this.ReadBlob(array[i].Col2))));
				}
				else
				{
					collection.Add(new BinaryCustomDebugInformation(array[i].Col1, this.ReadBlob(array[i].Col2)));
				}
				collection[i].token = new MetadataToken(TokenType.CustomDebugInformation, array[i].Col3);
			}
			return collection;
		}

		// Token: 0x06000A72 RID: 2674 RVA: 0x000237FC File Offset: 0x000219FC
		public byte[] ReadRawEmbeddedSourceDebugInformation(uint index)
		{
			SignatureReader signatureReader = this.ReadSignature(index);
			return signatureReader.ReadBytes((int)signatureReader.sig_length);
		}

		// Token: 0x06000A73 RID: 2675 RVA: 0x00023810 File Offset: 0x00021A10
		public Row<byte[], bool> ReadEmbeddedSourceDebugInformation(uint index)
		{
			SignatureReader signatureReader = this.ReadSignature(index);
			int num = signatureReader.ReadInt32();
			uint num2 = signatureReader.sig_length - 4U;
			if (num == 0)
			{
				return new Row<byte[], bool>(signatureReader.ReadBytes((int)num2), false);
			}
			if (num > 0)
			{
				Stream stream = new MemoryStream(signatureReader.ReadBytes((int)num2));
				byte[] array = new byte[num];
				MemoryStream memoryStream = new MemoryStream(array);
				using (DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress, true))
				{
					deflateStream.CopyTo(memoryStream);
				}
				return new Row<byte[], bool>(array, true);
			}
			throw new NotSupportedException();
		}

		// Token: 0x0400033D RID: 829
		internal readonly Image image;

		// Token: 0x0400033E RID: 830
		internal readonly ModuleDefinition module;

		// Token: 0x0400033F RID: 831
		internal readonly MetadataSystem metadata;

		// Token: 0x04000340 RID: 832
		internal CodeReader code;

		// Token: 0x04000341 RID: 833
		internal IGenericContext context;

		// Token: 0x04000342 RID: 834
		private readonly MetadataReader metadata_reader;
	}
}
