using System;
using System.IO;
using Mono.Cecil.Cil;
using Mono.Cecil.PE;

namespace Mono.Cecil
{
	// Token: 0x020001F3 RID: 499
	internal static class ModuleWriter
	{
		// Token: 0x06000A9E RID: 2718 RVA: 0x00024750 File Offset: 0x00022950
		public static void WriteModule(ModuleDefinition module, Disposable<Stream> stream, WriterParameters parameters)
		{
			using (stream)
			{
				ModuleWriter.Write(module, stream, parameters);
			}
		}

		// Token: 0x06000A9F RID: 2719 RVA: 0x00024788 File Offset: 0x00022988
		private static void Write(ModuleDefinition module, Disposable<Stream> stream, WriterParameters parameters)
		{
			if ((module.Attributes & ModuleAttributes.ILOnly) == (ModuleAttributes)0)
			{
				throw new NotSupportedException("Writing mixed-mode assemblies is not supported");
			}
			if (module.HasImage && module.ReadingMode == ReadingMode.Deferred)
			{
				ImmediateModuleReader immediateModuleReader = new ImmediateModuleReader(module.Image);
				immediateModuleReader.ReadModule(module, false);
				immediateModuleReader.ReadSymbols(module);
			}
			module.MetadataSystem.Clear();
			if (module.symbol_reader != null)
			{
				module.symbol_reader.Dispose();
			}
			AssemblyNameDefinition assemblyNameDefinition = ((module.assembly != null && module.kind != ModuleKind.NetModule) ? module.assembly.Name : null);
			string fileName = stream.value.GetFileName();
			uint num = parameters.Timestamp ?? module.timestamp;
			ISymbolWriterProvider symbolWriterProvider = parameters.SymbolWriterProvider;
			if (symbolWriterProvider == null && parameters.WriteSymbols)
			{
				symbolWriterProvider = new DefaultSymbolWriterProvider();
			}
			if (parameters.HasStrongNameKey && assemblyNameDefinition != null)
			{
				assemblyNameDefinition.PublicKey = CryptoService.GetPublicKey(parameters);
				module.Attributes |= ModuleAttributes.StrongNameSigned;
			}
			if (parameters.DeterministicMvid)
			{
				module.Mvid = Guid.Empty;
			}
			MetadataBuilder metadataBuilder = new MetadataBuilder(module, fileName, num, symbolWriterProvider);
			try
			{
				module.metadata_builder = metadataBuilder;
				using (ISymbolWriter symbolWriter = ModuleWriter.GetSymbolWriter(module, fileName, symbolWriterProvider, parameters))
				{
					metadataBuilder.SetSymbolWriter(symbolWriter);
					ModuleWriter.BuildMetadata(module, metadataBuilder);
					if (symbolWriter != null)
					{
						symbolWriter.Write();
					}
					ImageWriter imageWriter = ImageWriter.CreateWriter(module, metadataBuilder, stream);
					stream.value.SetLength(0L);
					imageWriter.WriteImage();
					if (parameters.DeterministicMvid)
					{
						ModuleWriter.ComputeDeterministicMvid(imageWriter, module);
					}
					if (parameters.HasStrongNameKey)
					{
						CryptoService.StrongName(stream.value, imageWriter, parameters);
					}
				}
			}
			finally
			{
				module.metadata_builder = null;
			}
		}

		// Token: 0x06000AA0 RID: 2720 RVA: 0x00024940 File Offset: 0x00022B40
		private static void BuildMetadata(ModuleDefinition module, MetadataBuilder metadata)
		{
			if (!module.HasImage)
			{
				metadata.BuildMetadata();
				return;
			}
			module.Read<MetadataBuilder, MetadataBuilder>(metadata, delegate(MetadataBuilder builder, MetadataReader _)
			{
				builder.BuildMetadata();
				return builder;
			});
		}

		// Token: 0x06000AA1 RID: 2721 RVA: 0x00024978 File Offset: 0x00022B78
		private static ISymbolWriter GetSymbolWriter(ModuleDefinition module, string fq_name, ISymbolWriterProvider symbol_writer_provider, WriterParameters parameters)
		{
			if (symbol_writer_provider == null)
			{
				return null;
			}
			if (parameters.SymbolStream != null)
			{
				return symbol_writer_provider.GetSymbolWriter(module, parameters.SymbolStream);
			}
			return symbol_writer_provider.GetSymbolWriter(module, fq_name);
		}

		// Token: 0x06000AA2 RID: 2722 RVA: 0x000249A0 File Offset: 0x00022BA0
		private static void ComputeDeterministicMvid(ImageWriter writer, ModuleDefinition module)
		{
			long position = writer.BaseStream.Position;
			writer.BaseStream.Seek(0L, SeekOrigin.Begin);
			Guid guid = CryptoService.ComputeGuid(CryptoService.ComputeHash(writer.BaseStream));
			writer.MoveToRVA(TextSegment.GuidHeap);
			writer.WriteBytes(guid.ToByteArray());
			writer.Flush();
			module.Mvid = guid;
			writer.BaseStream.Seek(position, SeekOrigin.Begin);
		}
	}
}
