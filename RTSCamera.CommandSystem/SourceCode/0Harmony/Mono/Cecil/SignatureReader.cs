using System;
using System.Text;
using Mono.Cecil.Cil;
using Mono.Cecil.Metadata;
using Mono.Cecil.PE;
using Mono.Collections.Generic;

namespace Mono.Cecil
{
	// Token: 0x020001F2 RID: 498
	internal sealed class SignatureReader : ByteBuffer
	{
		// Token: 0x1700024C RID: 588
		// (get) Token: 0x06000A7B RID: 2683 RVA: 0x00023916 File Offset: 0x00021B16
		private TypeSystem TypeSystem
		{
			get
			{
				return this.reader.module.TypeSystem;
			}
		}

		// Token: 0x06000A7C RID: 2684 RVA: 0x00023928 File Offset: 0x00021B28
		public SignatureReader(uint blob, MetadataReader reader)
			: base(reader.image.BlobHeap.data)
		{
			this.reader = reader;
			this.position = (int)blob;
			this.sig_length = base.ReadCompressedUInt32();
			this.start = (uint)this.position;
		}

		// Token: 0x06000A7D RID: 2685 RVA: 0x00023966 File Offset: 0x00021B66
		private MetadataToken ReadTypeTokenSignature()
		{
			return CodedIndex.TypeDefOrRef.GetMetadataToken(base.ReadCompressedUInt32());
		}

		// Token: 0x06000A7E RID: 2686 RVA: 0x00023974 File Offset: 0x00021B74
		private GenericParameter GetGenericParameter(GenericParameterType type, uint var)
		{
			IGenericContext context = this.reader.context;
			if (context == null)
			{
				return this.GetUnboundGenericParameter(type, (int)var);
			}
			IGenericParameterProvider genericParameterProvider;
			if (type != GenericParameterType.Type)
			{
				if (type != GenericParameterType.Method)
				{
					throw new NotSupportedException();
				}
				genericParameterProvider = context.Method;
			}
			else
			{
				genericParameterProvider = context.Type;
			}
			if (!context.IsDefinition)
			{
				SignatureReader.CheckGenericContext(genericParameterProvider, (int)var);
			}
			if (var >= (uint)genericParameterProvider.GenericParameters.Count)
			{
				return this.GetUnboundGenericParameter(type, (int)var);
			}
			return genericParameterProvider.GenericParameters[(int)var];
		}

		// Token: 0x06000A7F RID: 2687 RVA: 0x000239EE File Offset: 0x00021BEE
		private GenericParameter GetUnboundGenericParameter(GenericParameterType type, int index)
		{
			return new GenericParameter(index, type, this.reader.module);
		}

		// Token: 0x06000A80 RID: 2688 RVA: 0x00023A04 File Offset: 0x00021C04
		private static void CheckGenericContext(IGenericParameterProvider owner, int index)
		{
			Collection<GenericParameter> genericParameters = owner.GenericParameters;
			for (int i = genericParameters.Count; i <= index; i++)
			{
				genericParameters.Add(new GenericParameter(owner));
			}
		}

		// Token: 0x06000A81 RID: 2689 RVA: 0x00023A38 File Offset: 0x00021C38
		public void ReadGenericInstanceSignature(IGenericParameterProvider provider, IGenericInstance instance, uint arity)
		{
			if (!provider.IsDefinition)
			{
				SignatureReader.CheckGenericContext(provider, (int)(arity - 1U));
			}
			Collection<TypeReference> genericArguments = instance.GenericArguments;
			int num = 0;
			while ((long)num < (long)((ulong)arity))
			{
				genericArguments.Add(this.ReadTypeSignature());
				num++;
			}
		}

		// Token: 0x06000A82 RID: 2690 RVA: 0x00023A78 File Offset: 0x00021C78
		private ArrayType ReadArrayTypeSignature()
		{
			ArrayType arrayType = new ArrayType(this.ReadTypeSignature());
			uint num = base.ReadCompressedUInt32();
			uint[] array = new uint[base.ReadCompressedUInt32()];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = base.ReadCompressedUInt32();
			}
			int[] array2 = new int[base.ReadCompressedUInt32()];
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j] = base.ReadCompressedInt32();
			}
			arrayType.Dimensions.Clear();
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				int? num3 = null;
				int? num4 = null;
				if (num2 < array2.Length)
				{
					num3 = new int?(array2[num2]);
				}
				if (num2 < array.Length)
				{
					int? num5 = num3;
					int num6 = (int)array[num2];
					num4 = ((num5 != null) ? new int?(num5.GetValueOrDefault() + num6 - 1) : null);
				}
				arrayType.Dimensions.Add(new ArrayDimension(num3, num4));
				num2++;
			}
			return arrayType;
		}

		// Token: 0x06000A83 RID: 2691 RVA: 0x00023B75 File Offset: 0x00021D75
		private TypeReference GetTypeDefOrRef(MetadataToken token)
		{
			return this.reader.GetTypeDefOrRef(token);
		}

		// Token: 0x06000A84 RID: 2692 RVA: 0x00023B83 File Offset: 0x00021D83
		public TypeReference ReadTypeSignature()
		{
			return this.ReadTypeSignature((ElementType)base.ReadByte());
		}

		// Token: 0x06000A85 RID: 2693 RVA: 0x00023B91 File Offset: 0x00021D91
		public TypeReference ReadTypeToken()
		{
			return this.GetTypeDefOrRef(this.ReadTypeTokenSignature());
		}

		// Token: 0x06000A86 RID: 2694 RVA: 0x00023BA0 File Offset: 0x00021DA0
		private TypeReference ReadTypeSignature(ElementType etype)
		{
			switch (etype)
			{
			case ElementType.Void:
				return this.TypeSystem.Void;
			case ElementType.Boolean:
			case ElementType.Char:
			case ElementType.I1:
			case ElementType.U1:
			case ElementType.I2:
			case ElementType.U2:
			case ElementType.I4:
			case ElementType.U4:
			case ElementType.I8:
			case ElementType.U8:
			case ElementType.R4:
			case ElementType.R8:
			case ElementType.String:
			case (ElementType)23:
			case (ElementType)26:
				break;
			case ElementType.Ptr:
				return new PointerType(this.ReadTypeSignature());
			case ElementType.ByRef:
				return new ByReferenceType(this.ReadTypeSignature());
			case ElementType.ValueType:
			{
				TypeReference typeDefOrRef = this.GetTypeDefOrRef(this.ReadTypeTokenSignature());
				typeDefOrRef.KnownValueType();
				return typeDefOrRef;
			}
			case ElementType.Class:
				return this.GetTypeDefOrRef(this.ReadTypeTokenSignature());
			case ElementType.Var:
				return this.GetGenericParameter(GenericParameterType.Type, base.ReadCompressedUInt32());
			case ElementType.Array:
				return this.ReadArrayTypeSignature();
			case ElementType.GenericInst:
			{
				bool flag = base.ReadByte() == 17;
				TypeReference typeDefOrRef2 = this.GetTypeDefOrRef(this.ReadTypeTokenSignature());
				uint num = base.ReadCompressedUInt32();
				GenericInstanceType genericInstanceType = new GenericInstanceType(typeDefOrRef2, (int)num);
				this.ReadGenericInstanceSignature(typeDefOrRef2, genericInstanceType, num);
				if (flag)
				{
					genericInstanceType.KnownValueType();
					typeDefOrRef2.GetElementType().KnownValueType();
				}
				return genericInstanceType;
			}
			case ElementType.TypedByRef:
				return this.TypeSystem.TypedReference;
			case ElementType.I:
				return this.TypeSystem.IntPtr;
			case ElementType.U:
				return this.TypeSystem.UIntPtr;
			case ElementType.FnPtr:
			{
				FunctionPointerType functionPointerType = new FunctionPointerType();
				this.ReadMethodSignature(functionPointerType);
				return functionPointerType;
			}
			case ElementType.Object:
				return this.TypeSystem.Object;
			case ElementType.SzArray:
				return new ArrayType(this.ReadTypeSignature());
			case ElementType.MVar:
				return this.GetGenericParameter(GenericParameterType.Method, base.ReadCompressedUInt32());
			case ElementType.CModReqD:
				return new RequiredModifierType(this.GetTypeDefOrRef(this.ReadTypeTokenSignature()), this.ReadTypeSignature());
			case ElementType.CModOpt:
				return new OptionalModifierType(this.GetTypeDefOrRef(this.ReadTypeTokenSignature()), this.ReadTypeSignature());
			default:
				if (etype == ElementType.Sentinel)
				{
					return new SentinelType(this.ReadTypeSignature());
				}
				if (etype == ElementType.Pinned)
				{
					return new PinnedType(this.ReadTypeSignature());
				}
				break;
			}
			return this.GetPrimitiveType(etype);
		}

		// Token: 0x06000A87 RID: 2695 RVA: 0x00023D8C File Offset: 0x00021F8C
		public void ReadMethodSignature(IMethodSignature method)
		{
			byte b = base.ReadByte();
			if ((b & 32) != 0)
			{
				method.HasThis = true;
				b = (byte)((int)b & -33);
			}
			if ((b & 64) != 0)
			{
				method.ExplicitThis = true;
				b = (byte)((int)b & -65);
			}
			method.CallingConvention = (MethodCallingConvention)b;
			MethodReference methodReference = method as MethodReference;
			if (methodReference != null && !methodReference.DeclaringType.IsArray)
			{
				this.reader.context = methodReference;
			}
			if ((b & 16) != 0)
			{
				uint num = base.ReadCompressedUInt32();
				if (methodReference != null && !methodReference.IsDefinition)
				{
					SignatureReader.CheckGenericContext(methodReference, (int)(num - 1U));
				}
			}
			uint num2 = base.ReadCompressedUInt32();
			method.MethodReturnType.ReturnType = this.ReadTypeSignature();
			if (num2 == 0U)
			{
				return;
			}
			MethodReference methodReference2 = method as MethodReference;
			Collection<ParameterDefinition> collection;
			if (methodReference2 != null)
			{
				collection = (methodReference2.parameters = new ParameterDefinitionCollection(method, (int)num2));
			}
			else
			{
				collection = method.Parameters;
			}
			int num3 = 0;
			while ((long)num3 < (long)((ulong)num2))
			{
				collection.Add(new ParameterDefinition(this.ReadTypeSignature()));
				num3++;
			}
		}

		// Token: 0x06000A88 RID: 2696 RVA: 0x00023E7B File Offset: 0x0002207B
		public object ReadConstantSignature(ElementType type)
		{
			return this.ReadPrimitiveValue(type);
		}

		// Token: 0x06000A89 RID: 2697 RVA: 0x00023E84 File Offset: 0x00022084
		public void ReadCustomAttributeConstructorArguments(CustomAttribute attribute, Collection<ParameterDefinition> parameters)
		{
			int count = parameters.Count;
			if (count == 0)
			{
				return;
			}
			attribute.arguments = new Collection<CustomAttributeArgument>(count);
			for (int i = 0; i < count; i++)
			{
				TypeReference typeReference = GenericParameterResolver.ResolveParameterTypeIfNeeded(attribute.Constructor, parameters[i]);
				attribute.arguments.Add(this.ReadCustomAttributeFixedArgument(typeReference));
			}
		}

		// Token: 0x06000A8A RID: 2698 RVA: 0x00023ED9 File Offset: 0x000220D9
		private CustomAttributeArgument ReadCustomAttributeFixedArgument(TypeReference type)
		{
			if (type.IsArray)
			{
				return this.ReadCustomAttributeFixedArrayArgument((ArrayType)type);
			}
			return this.ReadCustomAttributeElement(type);
		}

		// Token: 0x06000A8B RID: 2699 RVA: 0x00023EF8 File Offset: 0x000220F8
		public void ReadCustomAttributeNamedArguments(ushort count, ref Collection<CustomAttributeNamedArgument> fields, ref Collection<CustomAttributeNamedArgument> properties)
		{
			for (int i = 0; i < (int)count; i++)
			{
				if (!this.CanReadMore())
				{
					return;
				}
				this.ReadCustomAttributeNamedArgument(ref fields, ref properties);
			}
		}

		// Token: 0x06000A8C RID: 2700 RVA: 0x00023F24 File Offset: 0x00022124
		private void ReadCustomAttributeNamedArgument(ref Collection<CustomAttributeNamedArgument> fields, ref Collection<CustomAttributeNamedArgument> properties)
		{
			byte b = base.ReadByte();
			TypeReference typeReference = this.ReadCustomAttributeFieldOrPropType();
			string text = this.ReadUTF8String();
			Collection<CustomAttributeNamedArgument> collection;
			if (b != 83)
			{
				if (b != 84)
				{
					throw new NotSupportedException();
				}
				collection = SignatureReader.GetCustomAttributeNamedArgumentCollection(ref properties);
			}
			else
			{
				collection = SignatureReader.GetCustomAttributeNamedArgumentCollection(ref fields);
			}
			collection.Add(new CustomAttributeNamedArgument(text, this.ReadCustomAttributeFixedArgument(typeReference)));
		}

		// Token: 0x06000A8D RID: 2701 RVA: 0x00023F80 File Offset: 0x00022180
		private static Collection<CustomAttributeNamedArgument> GetCustomAttributeNamedArgumentCollection(ref Collection<CustomAttributeNamedArgument> collection)
		{
			if (collection != null)
			{
				return collection;
			}
			Collection<CustomAttributeNamedArgument> collection2;
			collection = (collection2 = new Collection<CustomAttributeNamedArgument>());
			return collection2;
		}

		// Token: 0x06000A8E RID: 2702 RVA: 0x00023FA0 File Offset: 0x000221A0
		private CustomAttributeArgument ReadCustomAttributeFixedArrayArgument(ArrayType type)
		{
			uint num = base.ReadUInt32();
			if (num == 4294967295U)
			{
				return new CustomAttributeArgument(type, null);
			}
			if (num == 0U)
			{
				return new CustomAttributeArgument(type, Empty<CustomAttributeArgument>.Array);
			}
			CustomAttributeArgument[] array = new CustomAttributeArgument[num];
			TypeReference elementType = type.ElementType;
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				array[num2] = this.ReadCustomAttributeElement(elementType);
				num2++;
			}
			return new CustomAttributeArgument(type, array);
		}

		// Token: 0x06000A8F RID: 2703 RVA: 0x00024000 File Offset: 0x00022200
		private CustomAttributeArgument ReadCustomAttributeElement(TypeReference type)
		{
			if (type.IsArray)
			{
				return this.ReadCustomAttributeFixedArrayArgument((ArrayType)type);
			}
			return new CustomAttributeArgument(type, (type.etype == ElementType.Object) ? this.ReadCustomAttributeElement(this.ReadCustomAttributeFieldOrPropType()) : this.ReadCustomAttributeElementValue(type));
		}

		// Token: 0x06000A90 RID: 2704 RVA: 0x0002404C File Offset: 0x0002224C
		private object ReadCustomAttributeElementValue(TypeReference type)
		{
			ElementType elementType = type.etype;
			if (elementType == ElementType.GenericInst)
			{
				type = type.GetElementType();
				elementType = type.etype;
			}
			if (elementType != ElementType.None)
			{
				if (elementType == ElementType.String)
				{
					return this.ReadUTF8String();
				}
				return this.ReadPrimitiveValue(elementType);
			}
			else
			{
				if (type.IsTypeOf("System", "Type"))
				{
					return this.ReadTypeReference();
				}
				return this.ReadCustomAttributeEnum(type);
			}
		}

		// Token: 0x06000A91 RID: 2705 RVA: 0x000240AC File Offset: 0x000222AC
		private object ReadPrimitiveValue(ElementType type)
		{
			switch (type)
			{
			case ElementType.Boolean:
				return base.ReadByte() == 1;
			case ElementType.Char:
				return (char)base.ReadUInt16();
			case ElementType.I1:
				return (sbyte)base.ReadByte();
			case ElementType.U1:
				return base.ReadByte();
			case ElementType.I2:
				return base.ReadInt16();
			case ElementType.U2:
				return base.ReadUInt16();
			case ElementType.I4:
				return base.ReadInt32();
			case ElementType.U4:
				return base.ReadUInt32();
			case ElementType.I8:
				return base.ReadInt64();
			case ElementType.U8:
				return base.ReadUInt64();
			case ElementType.R4:
				return base.ReadSingle();
			case ElementType.R8:
				return base.ReadDouble();
			default:
				throw new NotImplementedException(type.ToString());
			}
		}

		// Token: 0x06000A92 RID: 2706 RVA: 0x0002419C File Offset: 0x0002239C
		private TypeReference GetPrimitiveType(ElementType etype)
		{
			switch (etype)
			{
			case ElementType.Boolean:
				return this.TypeSystem.Boolean;
			case ElementType.Char:
				return this.TypeSystem.Char;
			case ElementType.I1:
				return this.TypeSystem.SByte;
			case ElementType.U1:
				return this.TypeSystem.Byte;
			case ElementType.I2:
				return this.TypeSystem.Int16;
			case ElementType.U2:
				return this.TypeSystem.UInt16;
			case ElementType.I4:
				return this.TypeSystem.Int32;
			case ElementType.U4:
				return this.TypeSystem.UInt32;
			case ElementType.I8:
				return this.TypeSystem.Int64;
			case ElementType.U8:
				return this.TypeSystem.UInt64;
			case ElementType.R4:
				return this.TypeSystem.Single;
			case ElementType.R8:
				return this.TypeSystem.Double;
			case ElementType.String:
				return this.TypeSystem.String;
			default:
				throw new NotImplementedException(etype.ToString());
			}
		}

		// Token: 0x06000A93 RID: 2707 RVA: 0x00024298 File Offset: 0x00022498
		private TypeReference ReadCustomAttributeFieldOrPropType()
		{
			ElementType elementType = (ElementType)base.ReadByte();
			if (elementType <= ElementType.Type)
			{
				if (elementType == ElementType.SzArray)
				{
					return new ArrayType(this.ReadCustomAttributeFieldOrPropType());
				}
				if (elementType == ElementType.Type)
				{
					return this.TypeSystem.LookupType("System", "Type");
				}
			}
			else
			{
				if (elementType == ElementType.Boxed)
				{
					return this.TypeSystem.Object;
				}
				if (elementType == ElementType.Enum)
				{
					return this.ReadTypeReference();
				}
			}
			return this.GetPrimitiveType(elementType);
		}

		// Token: 0x06000A94 RID: 2708 RVA: 0x00024305 File Offset: 0x00022505
		public TypeReference ReadTypeReference()
		{
			return TypeParser.ParseType(this.reader.module, this.ReadUTF8String(), false);
		}

		// Token: 0x06000A95 RID: 2709 RVA: 0x00024320 File Offset: 0x00022520
		private object ReadCustomAttributeEnum(TypeReference enum_type)
		{
			TypeDefinition typeDefinition = enum_type.CheckedResolve();
			if (!typeDefinition.IsEnum)
			{
				throw new ArgumentException();
			}
			return this.ReadCustomAttributeElementValue(typeDefinition.GetEnumUnderlyingType());
		}

		// Token: 0x06000A96 RID: 2710 RVA: 0x00024350 File Offset: 0x00022550
		public SecurityAttribute ReadSecurityAttribute()
		{
			SecurityAttribute securityAttribute = new SecurityAttribute(this.ReadTypeReference());
			base.ReadCompressedUInt32();
			this.ReadCustomAttributeNamedArguments((ushort)base.ReadCompressedUInt32(), ref securityAttribute.fields, ref securityAttribute.properties);
			return securityAttribute;
		}

		// Token: 0x06000A97 RID: 2711 RVA: 0x0002438C File Offset: 0x0002258C
		public MarshalInfo ReadMarshalInfo()
		{
			NativeType nativeType = this.ReadNativeType();
			if (nativeType <= NativeType.SafeArray)
			{
				if (nativeType == NativeType.FixedSysString)
				{
					FixedSysStringMarshalInfo fixedSysStringMarshalInfo = new FixedSysStringMarshalInfo();
					if (this.CanReadMore())
					{
						fixedSysStringMarshalInfo.size = (int)base.ReadCompressedUInt32();
					}
					return fixedSysStringMarshalInfo;
				}
				if (nativeType == NativeType.SafeArray)
				{
					SafeArrayMarshalInfo safeArrayMarshalInfo = new SafeArrayMarshalInfo();
					if (this.CanReadMore())
					{
						safeArrayMarshalInfo.element_type = this.ReadVariantType();
					}
					return safeArrayMarshalInfo;
				}
			}
			else
			{
				if (nativeType == NativeType.FixedArray)
				{
					FixedArrayMarshalInfo fixedArrayMarshalInfo = new FixedArrayMarshalInfo();
					if (this.CanReadMore())
					{
						fixedArrayMarshalInfo.size = (int)base.ReadCompressedUInt32();
					}
					if (this.CanReadMore())
					{
						fixedArrayMarshalInfo.element_type = this.ReadNativeType();
					}
					return fixedArrayMarshalInfo;
				}
				if (nativeType == NativeType.Array)
				{
					ArrayMarshalInfo arrayMarshalInfo = new ArrayMarshalInfo();
					if (this.CanReadMore())
					{
						arrayMarshalInfo.element_type = this.ReadNativeType();
					}
					if (this.CanReadMore())
					{
						arrayMarshalInfo.size_parameter_index = (int)base.ReadCompressedUInt32();
					}
					if (this.CanReadMore())
					{
						arrayMarshalInfo.size = (int)base.ReadCompressedUInt32();
					}
					if (this.CanReadMore())
					{
						arrayMarshalInfo.size_parameter_multiplier = (int)base.ReadCompressedUInt32();
					}
					return arrayMarshalInfo;
				}
				if (nativeType == NativeType.CustomMarshaler)
				{
					CustomMarshalInfo customMarshalInfo = new CustomMarshalInfo();
					string text = this.ReadUTF8String();
					customMarshalInfo.guid = ((!string.IsNullOrEmpty(text)) ? new Guid(text) : Guid.Empty);
					customMarshalInfo.unmanaged_type = this.ReadUTF8String();
					customMarshalInfo.managed_type = this.ReadTypeReference();
					customMarshalInfo.cookie = this.ReadUTF8String();
					return customMarshalInfo;
				}
			}
			return new MarshalInfo(nativeType);
		}

		// Token: 0x06000A98 RID: 2712 RVA: 0x000244E9 File Offset: 0x000226E9
		private NativeType ReadNativeType()
		{
			return (NativeType)base.ReadByte();
		}

		// Token: 0x06000A99 RID: 2713 RVA: 0x000244E9 File Offset: 0x000226E9
		private VariantType ReadVariantType()
		{
			return (VariantType)base.ReadByte();
		}

		// Token: 0x06000A9A RID: 2714 RVA: 0x000244F4 File Offset: 0x000226F4
		private string ReadUTF8String()
		{
			if (this.buffer[this.position] == 255)
			{
				this.position++;
				return null;
			}
			int num = (int)base.ReadCompressedUInt32();
			if (num == 0)
			{
				return string.Empty;
			}
			if (this.position + num > this.buffer.Length)
			{
				return string.Empty;
			}
			string @string = Encoding.UTF8.GetString(this.buffer, this.position, num);
			this.position += num;
			return @string;
		}

		// Token: 0x06000A9B RID: 2715 RVA: 0x00024574 File Offset: 0x00022774
		public string ReadDocumentName()
		{
			char c = (char)this.buffer[this.position];
			this.position++;
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			while (this.CanReadMore())
			{
				if (num > 0 && c != '\0')
				{
					stringBuilder.Append(c);
				}
				uint num2 = base.ReadCompressedUInt32();
				if (num2 != 0U)
				{
					stringBuilder.Append(this.reader.ReadUTF8StringBlob(num2));
				}
				num++;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000A9C RID: 2716 RVA: 0x000245E8 File Offset: 0x000227E8
		public Collection<SequencePoint> ReadSequencePoints(Document document)
		{
			base.ReadCompressedUInt32();
			if (document == null)
			{
				document = this.reader.GetDocument(base.ReadCompressedUInt32());
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			bool flag = true;
			Collection<SequencePoint> collection = new Collection<SequencePoint>((int)((ulong)this.sig_length - (ulong)((long)this.position - (long)((ulong)this.start))) / 5);
			int num4 = 0;
			while (this.CanReadMore())
			{
				int num5 = (int)base.ReadCompressedUInt32();
				if (num4 > 0 && num5 == 0)
				{
					document = this.reader.GetDocument(base.ReadCompressedUInt32());
				}
				else
				{
					num += num5;
					int num6 = (int)base.ReadCompressedUInt32();
					int num7 = (int)((num6 == 0) ? base.ReadCompressedUInt32() : ((uint)base.ReadCompressedInt32()));
					if (num6 == 0 && num7 == 0)
					{
						collection.Add(new SequencePoint(num, document)
						{
							StartLine = 16707566,
							EndLine = 16707566,
							StartColumn = 0,
							EndColumn = 0
						});
					}
					else
					{
						if (flag)
						{
							num2 = (int)base.ReadCompressedUInt32();
							num3 = (int)base.ReadCompressedUInt32();
						}
						else
						{
							num2 += base.ReadCompressedInt32();
							num3 += base.ReadCompressedInt32();
						}
						collection.Add(new SequencePoint(num, document)
						{
							StartLine = num2,
							StartColumn = num3,
							EndLine = num2 + num6,
							EndColumn = num3 + num7
						});
						flag = false;
					}
				}
				num4++;
			}
			return collection;
		}

		// Token: 0x06000A9D RID: 2717 RVA: 0x00024733 File Offset: 0x00022933
		public bool CanReadMore()
		{
			return (long)this.position - (long)((ulong)this.start) < (long)((ulong)this.sig_length);
		}

		// Token: 0x04000346 RID: 838
		private readonly MetadataReader reader;

		// Token: 0x04000347 RID: 839
		internal readonly uint start;

		// Token: 0x04000348 RID: 840
		internal readonly uint sig_length;
	}
}
