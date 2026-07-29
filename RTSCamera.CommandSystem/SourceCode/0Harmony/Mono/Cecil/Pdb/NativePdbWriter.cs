using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil.Cil;
using Mono.Cecil.PE;
using Mono.Collections.Generic;

namespace Mono.Cecil.Pdb
{
	// Token: 0x0200035D RID: 861
	internal class NativePdbWriter : ISymbolWriter, IDisposable
	{
		// Token: 0x06001701 RID: 5889 RVA: 0x00046B0A File Offset: 0x00044D0A
		internal NativePdbWriter(ModuleDefinition module, SymWriter writer)
		{
			this.module = module;
			this.metadata = module.metadata_builder;
			this.writer = writer;
			this.documents = new Dictionary<string, SymDocumentWriter>();
			this.import_info_to_parent = new Dictionary<ImportDebugInformation, MetadataToken>();
		}

		// Token: 0x06001702 RID: 5890 RVA: 0x00046B42 File Offset: 0x00044D42
		public ISymbolReaderProvider GetReaderProvider()
		{
			return new NativePdbReaderProvider();
		}

		// Token: 0x06001703 RID: 5891 RVA: 0x00046B49 File Offset: 0x00044D49
		public ImageDebugHeader GetDebugHeader()
		{
			return new ImageDebugHeader(new ImageDebugHeaderEntry(this.debug_directory, this.debug_info));
		}

		// Token: 0x06001704 RID: 5892 RVA: 0x00046B64 File Offset: 0x00044D64
		public void Write(MethodDebugInformation info)
		{
			int num = info.method.MetadataToken.ToInt32();
			if (!info.HasSequencePoints && info.scope == null && !info.HasCustomDebugInformations && info.StateMachineKickOffMethod == null)
			{
				return;
			}
			this.writer.OpenMethod(num);
			if (!info.sequence_points.IsNullOrEmpty<SequencePoint>())
			{
				this.DefineSequencePoints(info.sequence_points);
			}
			MetadataToken metadataToken = default(MetadataToken);
			if (info.scope != null)
			{
				this.DefineScope(info.scope, info, out metadataToken);
			}
			this.DefineCustomMetadata(info, metadataToken);
			this.writer.CloseMethod();
		}

		// Token: 0x06001705 RID: 5893 RVA: 0x00046C00 File Offset: 0x00044E00
		private void DefineCustomMetadata(MethodDebugInformation info, MetadataToken import_parent)
		{
			CustomMetadataWriter customMetadataWriter = new CustomMetadataWriter(this.writer);
			if (import_parent.RID != 0U)
			{
				customMetadataWriter.WriteForwardInfo(import_parent);
			}
			else if (info.scope != null && info.scope.Import != null && info.scope.Import.HasTargets)
			{
				customMetadataWriter.WriteUsingInfo(info.scope.Import);
			}
			if (info.Method.HasCustomAttributes)
			{
				foreach (CustomAttribute customAttribute in info.Method.CustomAttributes)
				{
					TypeReference attributeType = customAttribute.AttributeType;
					if (attributeType.IsTypeOf("System.Runtime.CompilerServices", "IteratorStateMachineAttribute") || attributeType.IsTypeOf("System.Runtime.CompilerServices", "AsyncStateMachineAttribute"))
					{
						TypeReference typeReference = customAttribute.ConstructorArguments[0].Value as TypeReference;
						if (typeReference != null)
						{
							customMetadataWriter.WriteForwardIterator(typeReference);
						}
					}
				}
			}
			if (info.HasCustomDebugInformations)
			{
				StateMachineScopeDebugInformation stateMachineScopeDebugInformation = info.CustomDebugInformations.FirstOrDefault<CustomDebugInformation>((CustomDebugInformation cdi) => cdi.Kind == CustomDebugInformationKind.StateMachineScope) as StateMachineScopeDebugInformation;
				if (stateMachineScopeDebugInformation != null)
				{
					customMetadataWriter.WriteIteratorScopes(stateMachineScopeDebugInformation, info);
				}
			}
			customMetadataWriter.WriteCustomMetadata();
			this.DefineAsyncCustomMetadata(info);
		}

		// Token: 0x06001706 RID: 5894 RVA: 0x00046D60 File Offset: 0x00044F60
		private void DefineAsyncCustomMetadata(MethodDebugInformation info)
		{
			if (!info.HasCustomDebugInformations)
			{
				return;
			}
			foreach (CustomDebugInformation customDebugInformation in info.CustomDebugInformations)
			{
				AsyncMethodBodyDebugInformation asyncMethodBodyDebugInformation = customDebugInformation as AsyncMethodBodyDebugInformation;
				if (asyncMethodBodyDebugInformation != null)
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						BinaryStreamWriter binaryStreamWriter = new BinaryStreamWriter(memoryStream);
						binaryStreamWriter.WriteUInt32((info.StateMachineKickOffMethod != null) ? info.StateMachineKickOffMethod.MetadataToken.ToUInt32() : 0U);
						binaryStreamWriter.WriteUInt32((uint)asyncMethodBodyDebugInformation.CatchHandler.Offset);
						binaryStreamWriter.WriteUInt32((uint)asyncMethodBodyDebugInformation.Resumes.Count);
						for (int i = 0; i < asyncMethodBodyDebugInformation.Resumes.Count; i++)
						{
							binaryStreamWriter.WriteUInt32((uint)asyncMethodBodyDebugInformation.Yields[i].Offset);
							binaryStreamWriter.WriteUInt32(asyncMethodBodyDebugInformation.resume_methods[i].MetadataToken.ToUInt32());
							binaryStreamWriter.WriteUInt32((uint)asyncMethodBodyDebugInformation.Resumes[i].Offset);
						}
						this.writer.DefineCustomMetadata("asyncMethodInfo", memoryStream.ToArray());
					}
				}
			}
		}

		// Token: 0x06001707 RID: 5895 RVA: 0x00046ED8 File Offset: 0x000450D8
		private void DefineScope(ScopeDebugInformation scope, MethodDebugInformation info, out MetadataToken import_parent)
		{
			int offset = scope.Start.Offset;
			int num = (scope.End.IsEndOfMethod ? info.code_size : scope.End.Offset);
			import_parent = new MetadataToken(0U);
			this.writer.OpenScope(offset);
			if (scope.Import != null && scope.Import.HasTargets && !this.import_info_to_parent.TryGetValue(info.scope.Import, out import_parent))
			{
				foreach (ImportTarget importTarget in scope.Import.Targets)
				{
					ImportTargetKind kind = importTarget.Kind;
					if (kind <= ImportTargetKind.ImportType)
					{
						if (kind != ImportTargetKind.ImportNamespace)
						{
							if (kind == ImportTargetKind.ImportType)
							{
								this.writer.UsingNamespace("T" + TypeParser.ToParseable(importTarget.type, true));
							}
						}
						else
						{
							this.writer.UsingNamespace("U" + importTarget.@namespace);
						}
					}
					else if (kind != ImportTargetKind.DefineNamespaceAlias)
					{
						if (kind == ImportTargetKind.DefineTypeAlias)
						{
							this.writer.UsingNamespace("A" + importTarget.Alias + " T" + TypeParser.ToParseable(importTarget.type, true));
						}
					}
					else
					{
						this.writer.UsingNamespace("A" + importTarget.Alias + " U" + importTarget.@namespace);
					}
				}
				this.import_info_to_parent.Add(info.scope.Import, info.method.MetadataToken);
			}
			int num2 = info.local_var_token.ToInt32();
			if (!scope.variables.IsNullOrEmpty<VariableDebugInformation>())
			{
				for (int i = 0; i < scope.variables.Count; i++)
				{
					VariableDebugInformation variableDebugInformation = scope.variables[i];
					this.DefineLocalVariable(variableDebugInformation, num2, offset, num);
				}
			}
			if (!scope.constants.IsNullOrEmpty<ConstantDebugInformation>())
			{
				for (int j = 0; j < scope.constants.Count; j++)
				{
					ConstantDebugInformation constantDebugInformation = scope.constants[j];
					this.DefineConstant(constantDebugInformation);
				}
			}
			if (!scope.scopes.IsNullOrEmpty<ScopeDebugInformation>())
			{
				for (int k = 0; k < scope.scopes.Count; k++)
				{
					MetadataToken metadataToken;
					this.DefineScope(scope.scopes[k], info, out metadataToken);
				}
			}
			this.writer.CloseScope(num);
		}

		// Token: 0x06001708 RID: 5896 RVA: 0x00047174 File Offset: 0x00045374
		private void DefineSequencePoints(Collection<SequencePoint> sequence_points)
		{
			for (int i = 0; i < sequence_points.Count; i++)
			{
				SequencePoint sequencePoint = sequence_points[i];
				this.writer.DefineSequencePoints(this.GetDocument(sequencePoint.Document), new int[] { sequencePoint.Offset }, new int[] { sequencePoint.StartLine }, new int[] { sequencePoint.StartColumn }, new int[] { sequencePoint.EndLine }, new int[] { sequencePoint.EndColumn });
			}
		}

		// Token: 0x06001709 RID: 5897 RVA: 0x000471FC File Offset: 0x000453FC
		private void DefineLocalVariable(VariableDebugInformation variable, int local_var_token, int start_offset, int end_offset)
		{
			this.writer.DefineLocalVariable2(variable.Name, variable.Attributes, local_var_token, variable.Index, 0, 0, start_offset, end_offset);
		}

		// Token: 0x0600170A RID: 5898 RVA: 0x0004722C File Offset: 0x0004542C
		private void DefineConstant(ConstantDebugInformation constant)
		{
			uint num = this.metadata.AddStandAloneSignature(this.metadata.GetConstantTypeBlobIndex(constant.ConstantType));
			MetadataToken metadataToken = new MetadataToken(TokenType.Signature, num);
			this.writer.DefineConstant2(constant.Name, constant.Value, metadataToken.ToInt32());
		}

		// Token: 0x0600170B RID: 5899 RVA: 0x00047284 File Offset: 0x00045484
		private SymDocumentWriter GetDocument(Document document)
		{
			if (document == null)
			{
				return null;
			}
			SymDocumentWriter symDocumentWriter;
			if (this.documents.TryGetValue(document.Url, out symDocumentWriter))
			{
				return symDocumentWriter;
			}
			symDocumentWriter = this.writer.DefineDocument(document.Url, document.LanguageGuid, document.LanguageVendorGuid, document.TypeGuid);
			if (!document.Hash.IsNullOrEmpty<byte>())
			{
				symDocumentWriter.SetCheckSum(document.HashAlgorithmGuid, document.Hash);
			}
			this.documents[document.Url] = symDocumentWriter;
			return symDocumentWriter;
		}

		// Token: 0x0600170C RID: 5900 RVA: 0x00047304 File Offset: 0x00045504
		public void Write()
		{
			MethodDefinition entryPoint = this.module.EntryPoint;
			if (entryPoint != null)
			{
				this.writer.SetUserEntryPoint(entryPoint.MetadataToken.ToInt32());
			}
			this.debug_info = this.writer.GetDebugInfo(out this.debug_directory);
			this.debug_directory.TimeDateStamp = (int)this.module.timestamp;
			this.writer.Close();
		}

		// Token: 0x0600170D RID: 5901 RVA: 0x0001B842 File Offset: 0x00019A42
		public void Write(ICustomDebugInformationProvider provider)
		{
		}

		// Token: 0x0600170E RID: 5902 RVA: 0x00047371 File Offset: 0x00045571
		public void Dispose()
		{
			this.writer.Close();
		}

		// Token: 0x04000B39 RID: 2873
		private readonly ModuleDefinition module;

		// Token: 0x04000B3A RID: 2874
		private readonly MetadataBuilder metadata;

		// Token: 0x04000B3B RID: 2875
		private readonly SymWriter writer;

		// Token: 0x04000B3C RID: 2876
		private readonly Dictionary<string, SymDocumentWriter> documents;

		// Token: 0x04000B3D RID: 2877
		private readonly Dictionary<ImportDebugInformation, MetadataToken> import_info_to_parent;

		// Token: 0x04000B3E RID: 2878
		private ImageDebugDirectory debug_directory;

		// Token: 0x04000B3F RID: 2879
		private byte[] debug_info;
	}
}
