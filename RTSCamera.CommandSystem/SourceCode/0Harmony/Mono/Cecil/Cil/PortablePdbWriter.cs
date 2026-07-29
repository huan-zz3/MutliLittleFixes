using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Mono.Cecil.Metadata;
using Mono.Cecil.PE;

namespace Mono.Cecil.Cil
{
	// Token: 0x02000301 RID: 769
	internal sealed class PortablePdbWriter : ISymbolWriter, IDisposable
	{
		// Token: 0x1700050B RID: 1291
		// (get) Token: 0x06001403 RID: 5123 RVA: 0x0004010F File Offset: 0x0003E30F
		private bool IsEmbedded
		{
			get
			{
				return this.writer == null;
			}
		}

		// Token: 0x06001404 RID: 5124 RVA: 0x0004011C File Offset: 0x0003E31C
		internal PortablePdbWriter(MetadataBuilder pdb_metadata, ModuleDefinition module)
		{
			this.pdb_metadata = pdb_metadata;
			this.module = module;
			this.module_metadata = module.metadata_builder;
			if (this.module_metadata != pdb_metadata)
			{
				this.pdb_metadata.metadata_builder = this.module_metadata;
			}
			pdb_metadata.AddCustomDebugInformations(module);
		}

		// Token: 0x06001405 RID: 5125 RVA: 0x0004016A File Offset: 0x0003E36A
		internal PortablePdbWriter(MetadataBuilder pdb_metadata, ModuleDefinition module, ImageWriter writer, Disposable<Stream> final_stream)
			: this(pdb_metadata, module)
		{
			this.writer = writer;
			this.final_stream = final_stream;
		}

		// Token: 0x06001406 RID: 5126 RVA: 0x00040183 File Offset: 0x0003E383
		public ISymbolReaderProvider GetReaderProvider()
		{
			return new PortablePdbReaderProvider();
		}

		// Token: 0x06001407 RID: 5127 RVA: 0x0004018A File Offset: 0x0003E38A
		public void Write(MethodDebugInformation info)
		{
			this.CheckMethodDebugInformationTable();
			this.pdb_metadata.AddMethodDebugInformation(info);
		}

		// Token: 0x06001408 RID: 5128 RVA: 0x000401A0 File Offset: 0x0003E3A0
		public void Write()
		{
			if (this.IsEmbedded)
			{
				return;
			}
			this.WritePdbFile();
			if (this.final_stream.value != null)
			{
				this.writer.BaseStream.Seek(0L, SeekOrigin.Begin);
				byte[] array = new byte[8192];
				CryptoService.CopyStreamChunk(this.writer.BaseStream, this.final_stream.value, array, (int)this.writer.BaseStream.Length);
			}
		}

		// Token: 0x06001409 RID: 5129 RVA: 0x00040215 File Offset: 0x0003E415
		public void Write(ICustomDebugInformationProvider provider)
		{
			this.pdb_metadata.AddCustomDebugInformations(provider);
		}

		// Token: 0x0600140A RID: 5130 RVA: 0x00040224 File Offset: 0x0003E424
		public ImageDebugHeader GetDebugHeader()
		{
			if (this.IsEmbedded)
			{
				return new ImageDebugHeader();
			}
			ImageDebugDirectory imageDebugDirectory = new ImageDebugDirectory
			{
				MajorVersion = 256,
				MinorVersion = 20557,
				Type = ImageDebugType.CodeView,
				TimeDateStamp = (int)this.pdb_id_stamp
			};
			ByteBuffer byteBuffer = new ByteBuffer();
			byteBuffer.WriteUInt32(1396986706U);
			byteBuffer.WriteBytes(this.pdb_id_guid.ToByteArray());
			byteBuffer.WriteUInt32(1U);
			string text = this.writer.BaseStream.GetFileName();
			if (string.IsNullOrEmpty(text))
			{
				text = this.module.Assembly.Name.Name + ".pdb";
			}
			byteBuffer.WriteBytes(Encoding.UTF8.GetBytes(text));
			byteBuffer.WriteByte(0);
			byte[] array = new byte[byteBuffer.length];
			Buffer.BlockCopy(byteBuffer.buffer, 0, array, 0, byteBuffer.length);
			imageDebugDirectory.SizeOfData = array.Length;
			ImageDebugHeaderEntry imageDebugHeaderEntry = new ImageDebugHeaderEntry(imageDebugDirectory, array);
			ImageDebugDirectory imageDebugDirectory2 = new ImageDebugDirectory
			{
				MajorVersion = 1,
				MinorVersion = 0,
				Type = ImageDebugType.PdbChecksum,
				TimeDateStamp = 0
			};
			ByteBuffer byteBuffer2 = new ByteBuffer();
			byteBuffer2.WriteBytes(Encoding.UTF8.GetBytes("SHA256"));
			byteBuffer2.WriteByte(0);
			byteBuffer2.WriteBytes(this.pdb_checksum);
			byte[] array2 = new byte[byteBuffer2.length];
			Buffer.BlockCopy(byteBuffer2.buffer, 0, array2, 0, byteBuffer2.length);
			imageDebugDirectory2.SizeOfData = array2.Length;
			ImageDebugHeaderEntry imageDebugHeaderEntry2 = new ImageDebugHeaderEntry(imageDebugDirectory2, array2);
			return new ImageDebugHeader(new ImageDebugHeaderEntry[] { imageDebugHeaderEntry, imageDebugHeaderEntry2 });
		}

		// Token: 0x0600140B RID: 5131 RVA: 0x000403D8 File Offset: 0x0003E5D8
		private void CheckMethodDebugInformationTable()
		{
			MethodDebugInformationTable table = this.pdb_metadata.table_heap.GetTable<MethodDebugInformationTable>(Table.MethodDebugInformation);
			if (table.length > 0)
			{
				return;
			}
			table.rows = new Row<uint, uint>[this.module_metadata.method_rid - 1U];
			table.length = table.rows.Length;
		}

		// Token: 0x0600140C RID: 5132 RVA: 0x00040428 File Offset: 0x0003E628
		public void Dispose()
		{
			this.writer.stream.Dispose();
			this.final_stream.Dispose();
		}

		// Token: 0x0600140D RID: 5133 RVA: 0x00040458 File Offset: 0x0003E658
		private void WritePdbFile()
		{
			this.WritePdbHeap();
			this.WriteTableHeap();
			this.writer.BuildMetadataTextMap();
			this.writer.WriteMetadataHeader();
			this.writer.WriteMetadata();
			this.writer.Flush();
			this.ComputeChecksumAndPdbId();
			this.WritePdbId();
		}

		// Token: 0x0600140E RID: 5134 RVA: 0x000404AC File Offset: 0x0003E6AC
		private void WritePdbHeap()
		{
			PdbHeapBuffer pdb_heap = this.pdb_metadata.pdb_heap;
			pdb_heap.WriteBytes(20);
			pdb_heap.WriteUInt32(this.module_metadata.entry_point.ToUInt32());
			MetadataTable[] tables = this.module_metadata.table_heap.tables;
			ulong num = 0UL;
			for (int i = 0; i < tables.Length; i++)
			{
				if (tables[i] != null && tables[i].Length != 0)
				{
					num |= 1UL << i;
				}
			}
			pdb_heap.WriteUInt64(num);
			for (int j = 0; j < tables.Length; j++)
			{
				if (tables[j] != null && tables[j].Length != 0)
				{
					pdb_heap.WriteUInt32((uint)tables[j].Length);
				}
			}
		}

		// Token: 0x0600140F RID: 5135 RVA: 0x00040558 File Offset: 0x0003E758
		private void WriteTableHeap()
		{
			this.pdb_metadata.table_heap.string_offsets = this.pdb_metadata.string_heap.WriteStrings();
			this.pdb_metadata.table_heap.ComputeTableInformations();
			this.pdb_metadata.table_heap.WriteTableHeap();
		}

		// Token: 0x06001410 RID: 5136 RVA: 0x000405A8 File Offset: 0x0003E7A8
		private void ComputeChecksumAndPdbId()
		{
			byte[] array = new byte[8192];
			this.writer.BaseStream.Seek(0L, SeekOrigin.Begin);
			SHA256 sha = SHA256.Create();
			using (CryptoStream cryptoStream = new CryptoStream(Stream.Null, sha, CryptoStreamMode.Write))
			{
				CryptoService.CopyStreamChunk(this.writer.BaseStream, cryptoStream, array, (int)this.writer.BaseStream.Length);
			}
			this.pdb_checksum = sha.Hash;
			ByteBuffer byteBuffer = new ByteBuffer(this.pdb_checksum);
			this.pdb_id_guid = new Guid(byteBuffer.ReadBytes(16));
			this.pdb_id_stamp = byteBuffer.ReadUInt32();
		}

		// Token: 0x06001411 RID: 5137 RVA: 0x00040660 File Offset: 0x0003E860
		private void WritePdbId()
		{
			this.writer.MoveToRVA(TextSegment.PdbHeap);
			this.writer.WriteBytes(this.pdb_id_guid.ToByteArray());
			this.writer.WriteUInt32(this.pdb_id_stamp);
		}

		// Token: 0x040009F0 RID: 2544
		private readonly MetadataBuilder pdb_metadata;

		// Token: 0x040009F1 RID: 2545
		private readonly ModuleDefinition module;

		// Token: 0x040009F2 RID: 2546
		private readonly ImageWriter writer;

		// Token: 0x040009F3 RID: 2547
		private readonly Disposable<Stream> final_stream;

		// Token: 0x040009F4 RID: 2548
		private MetadataBuilder module_metadata;

		// Token: 0x040009F5 RID: 2549
		internal byte[] pdb_checksum;

		// Token: 0x040009F6 RID: 2550
		internal Guid pdb_id_guid;

		// Token: 0x040009F7 RID: 2551
		internal uint pdb_id_stamp;
	}
}
