using System;
using System.Text;
using Mono.Cecil.Cil;
using Mono.Cecil.Metadata;
using Mono.Cecil.PE;
using Mono.Collections.Generic;

namespace Mono.Cecil
{
	// Token: 0x02000225 RID: 549
	internal sealed class SignatureWriter : ByteBuffer
	{
		// Token: 0x06000B88 RID: 2952 RVA: 0x00028AE0 File Offset: 0x00026CE0
		public SignatureWriter(MetadataBuilder metadata)
			: base(6)
		{
			this.metadata = metadata;
		}

		// Token: 0x06000B89 RID: 2953 RVA: 0x00028AF0 File Offset: 0x00026CF0
		public void WriteElementType(ElementType element_type)
		{
			base.WriteByte((byte)element_type);
		}

		// Token: 0x06000B8A RID: 2954 RVA: 0x00028AFC File Offset: 0x00026CFC
		public void WriteUTF8String(string @string)
		{
			if (@string == null)
			{
				base.WriteByte(byte.MaxValue);
				return;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(@string);
			base.WriteCompressedUInt32((uint)bytes.Length);
			base.WriteBytes(bytes);
		}

		// Token: 0x06000B8B RID: 2955 RVA: 0x00028B34 File Offset: 0x00026D34
		public void WriteMethodSignature(IMethodSignature method)
		{
			byte b = (byte)method.CallingConvention;
			if (method.HasThis)
			{
				b |= 32;
			}
			if (method.ExplicitThis)
			{
				b |= 64;
			}
			IGenericParameterProvider genericParameterProvider = method as IGenericParameterProvider;
			int num = ((genericParameterProvider != null && genericParameterProvider.HasGenericParameters) ? genericParameterProvider.GenericParameters.Count : 0);
			if (num > 0)
			{
				b |= 16;
			}
			int num2 = (method.HasParameters ? method.Parameters.Count : 0);
			base.WriteByte(b);
			if (num > 0)
			{
				base.WriteCompressedUInt32((uint)num);
			}
			base.WriteCompressedUInt32((uint)num2);
			this.WriteTypeSignature(method.ReturnType);
			if (num2 == 0)
			{
				return;
			}
			Collection<ParameterDefinition> parameters = method.Parameters;
			for (int i = 0; i < num2; i++)
			{
				this.WriteTypeSignature(parameters[i].ParameterType);
			}
		}

		// Token: 0x06000B8C RID: 2956 RVA: 0x00028BFB File Offset: 0x00026DFB
		private uint MakeTypeDefOrRefCodedRID(TypeReference type)
		{
			return CodedIndex.TypeDefOrRef.CompressMetadataToken(this.metadata.LookupToken(type));
		}

		// Token: 0x06000B8D RID: 2957 RVA: 0x00028C0F File Offset: 0x00026E0F
		public void WriteTypeToken(TypeReference type)
		{
			base.WriteCompressedUInt32(this.MakeTypeDefOrRefCodedRID(type));
		}

		// Token: 0x06000B8E RID: 2958 RVA: 0x00028C20 File Offset: 0x00026E20
		public void WriteTypeSignature(TypeReference type)
		{
			if (type == null)
			{
				throw new ArgumentNullException();
			}
			ElementType etype = type.etype;
			if (etype <= ElementType.GenericInst)
			{
				if (etype == ElementType.None)
				{
					this.WriteElementType(type.IsValueType ? ElementType.ValueType : ElementType.Class);
					base.WriteCompressedUInt32(this.MakeTypeDefOrRefCodedRID(type));
					return;
				}
				switch (etype)
				{
				case ElementType.Ptr:
				case ElementType.ByRef:
					goto IL_00D7;
				case ElementType.ValueType:
				case ElementType.Class:
					goto IL_016F;
				case ElementType.Var:
					break;
				case ElementType.Array:
				{
					ArrayType arrayType = (ArrayType)type;
					if (!arrayType.IsVector)
					{
						this.WriteArrayTypeSignature(arrayType);
						return;
					}
					this.WriteElementType(ElementType.SzArray);
					this.WriteTypeSignature(arrayType.ElementType);
					return;
				}
				case ElementType.GenericInst:
				{
					GenericInstanceType genericInstanceType = (GenericInstanceType)type;
					this.WriteElementType(ElementType.GenericInst);
					this.WriteElementType(genericInstanceType.IsValueType ? ElementType.ValueType : ElementType.Class);
					base.WriteCompressedUInt32(this.MakeTypeDefOrRefCodedRID(genericInstanceType.ElementType));
					this.WriteGenericInstanceSignature(genericInstanceType);
					return;
				}
				default:
					goto IL_016F;
				}
			}
			else
			{
				switch (etype)
				{
				case ElementType.FnPtr:
				{
					FunctionPointerType functionPointerType = (FunctionPointerType)type;
					this.WriteElementType(ElementType.FnPtr);
					this.WriteMethodSignature(functionPointerType);
					return;
				}
				case ElementType.Object:
				case ElementType.SzArray:
					goto IL_016F;
				case ElementType.MVar:
					break;
				case ElementType.CModReqD:
				case ElementType.CModOpt:
				{
					IModifierType modifierType = (IModifierType)type;
					this.WriteModifierSignature(etype, modifierType);
					return;
				}
				default:
					if (etype != ElementType.Sentinel && etype != ElementType.Pinned)
					{
						goto IL_016F;
					}
					goto IL_00D7;
				}
			}
			GenericParameter genericParameter = (GenericParameter)type;
			this.WriteElementType(etype);
			int position = genericParameter.Position;
			if (position == -1)
			{
				throw new NotSupportedException();
			}
			base.WriteCompressedUInt32((uint)position);
			return;
			IL_00D7:
			TypeSpecification typeSpecification = (TypeSpecification)type;
			this.WriteElementType(etype);
			this.WriteTypeSignature(typeSpecification.ElementType);
			return;
			IL_016F:
			if (!this.TryWriteElementType(type))
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x06000B8F RID: 2959 RVA: 0x00028DAC File Offset: 0x00026FAC
		private void WriteArrayTypeSignature(ArrayType array)
		{
			this.WriteElementType(ElementType.Array);
			this.WriteTypeSignature(array.ElementType);
			Collection<ArrayDimension> dimensions = array.Dimensions;
			int count = dimensions.Count;
			base.WriteCompressedUInt32((uint)count);
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < count; i++)
			{
				ArrayDimension arrayDimension = dimensions[i];
				if (arrayDimension.UpperBound != null)
				{
					num++;
					num2++;
				}
				else if (arrayDimension.LowerBound != null)
				{
					num2++;
				}
			}
			int[] array2 = new int[num];
			int[] array3 = new int[num2];
			for (int j = 0; j < num2; j++)
			{
				ArrayDimension arrayDimension2 = dimensions[j];
				array3[j] = arrayDimension2.LowerBound.GetValueOrDefault();
				if (arrayDimension2.UpperBound != null)
				{
					array2[j] = arrayDimension2.UpperBound.Value - array3[j] + 1;
				}
			}
			base.WriteCompressedUInt32((uint)num);
			for (int k = 0; k < num; k++)
			{
				base.WriteCompressedUInt32((uint)array2[k]);
			}
			base.WriteCompressedUInt32((uint)num2);
			for (int l = 0; l < num2; l++)
			{
				base.WriteCompressedInt32(array3[l]);
			}
		}

		// Token: 0x06000B90 RID: 2960 RVA: 0x00028EE4 File Offset: 0x000270E4
		public void WriteGenericInstanceSignature(IGenericInstance instance)
		{
			Collection<TypeReference> genericArguments = instance.GenericArguments;
			int count = genericArguments.Count;
			base.WriteCompressedUInt32((uint)count);
			for (int i = 0; i < count; i++)
			{
				this.WriteTypeSignature(genericArguments[i]);
			}
		}

		// Token: 0x06000B91 RID: 2961 RVA: 0x00028F1F File Offset: 0x0002711F
		private void WriteModifierSignature(ElementType element_type, IModifierType type)
		{
			this.WriteElementType(element_type);
			base.WriteCompressedUInt32(this.MakeTypeDefOrRefCodedRID(type.ModifierType));
			this.WriteTypeSignature(type.ElementType);
		}

		// Token: 0x06000B92 RID: 2962 RVA: 0x00028F48 File Offset: 0x00027148
		private bool TryWriteElementType(TypeReference type)
		{
			ElementType etype = type.etype;
			if (etype == ElementType.None)
			{
				return false;
			}
			this.WriteElementType(etype);
			return true;
		}

		// Token: 0x06000B93 RID: 2963 RVA: 0x00028F69 File Offset: 0x00027169
		public void WriteConstantString(string value)
		{
			if (value != null)
			{
				base.WriteBytes(Encoding.Unicode.GetBytes(value));
				return;
			}
			base.WriteByte(byte.MaxValue);
		}

		// Token: 0x06000B94 RID: 2964 RVA: 0x00028F8B File Offset: 0x0002718B
		public void WriteConstantPrimitive(object value)
		{
			this.WritePrimitiveValue(value);
		}

		// Token: 0x06000B95 RID: 2965 RVA: 0x00028F94 File Offset: 0x00027194
		public void WriteCustomAttributeConstructorArguments(CustomAttribute attribute)
		{
			if (!attribute.HasConstructorArguments)
			{
				return;
			}
			Collection<CustomAttributeArgument> constructorArguments = attribute.ConstructorArguments;
			Collection<ParameterDefinition> parameters = attribute.Constructor.Parameters;
			if (parameters.Count != constructorArguments.Count)
			{
				throw new InvalidOperationException();
			}
			for (int i = 0; i < constructorArguments.Count; i++)
			{
				TypeReference typeReference = GenericParameterResolver.ResolveParameterTypeIfNeeded(attribute.Constructor, parameters[i]);
				this.WriteCustomAttributeFixedArgument(typeReference, constructorArguments[i]);
			}
		}

		// Token: 0x06000B96 RID: 2966 RVA: 0x00029003 File Offset: 0x00027203
		private void WriteCustomAttributeFixedArgument(TypeReference type, CustomAttributeArgument argument)
		{
			if (type.IsArray)
			{
				this.WriteCustomAttributeFixedArrayArgument((ArrayType)type, argument);
				return;
			}
			this.WriteCustomAttributeElement(type, argument);
		}

		// Token: 0x06000B97 RID: 2967 RVA: 0x00029024 File Offset: 0x00027224
		private void WriteCustomAttributeFixedArrayArgument(ArrayType type, CustomAttributeArgument argument)
		{
			CustomAttributeArgument[] array = argument.Value as CustomAttributeArgument[];
			if (array == null)
			{
				base.WriteUInt32(uint.MaxValue);
				return;
			}
			base.WriteInt32(array.Length);
			if (array.Length == 0)
			{
				return;
			}
			TypeReference elementType = type.ElementType;
			for (int i = 0; i < array.Length; i++)
			{
				this.WriteCustomAttributeElement(elementType, array[i]);
			}
		}

		// Token: 0x06000B98 RID: 2968 RVA: 0x0002907C File Offset: 0x0002727C
		private void WriteCustomAttributeElement(TypeReference type, CustomAttributeArgument argument)
		{
			if (type.IsArray)
			{
				this.WriteCustomAttributeFixedArrayArgument((ArrayType)type, argument);
				return;
			}
			if (type.etype == ElementType.Object)
			{
				argument = (CustomAttributeArgument)argument.Value;
				type = argument.Type;
				this.WriteCustomAttributeFieldOrPropType(type);
				this.WriteCustomAttributeElement(type, argument);
				return;
			}
			this.WriteCustomAttributeValue(type, argument.Value);
		}

		// Token: 0x06000B99 RID: 2969 RVA: 0x000290E0 File Offset: 0x000272E0
		private void WriteCustomAttributeValue(TypeReference type, object value)
		{
			ElementType etype = type.etype;
			if (etype != ElementType.None)
			{
				if (etype != ElementType.String)
				{
					if (etype != ElementType.GenericInst)
					{
						this.WritePrimitiveValue(value);
						return;
					}
					this.WriteCustomAttributeEnumValue(type, value);
					return;
				}
				else
				{
					string text = (string)value;
					if (text == null)
					{
						base.WriteByte(byte.MaxValue);
						return;
					}
					this.WriteUTF8String(text);
					return;
				}
			}
			else
			{
				if (type.IsTypeOf("System", "Type"))
				{
					this.WriteCustomAttributeTypeValue((TypeReference)value);
					return;
				}
				this.WriteCustomAttributeEnumValue(type, value);
				return;
			}
		}

		// Token: 0x06000B9A RID: 2970 RVA: 0x0002915C File Offset: 0x0002735C
		private void WriteCustomAttributeTypeValue(TypeReference value)
		{
			TypeDefinition typeDefinition = value as TypeDefinition;
			if (typeDefinition != null)
			{
				TypeDefinition typeDefinition2 = typeDefinition;
				while (typeDefinition2.DeclaringType != null)
				{
					typeDefinition2 = typeDefinition2.DeclaringType;
				}
				if (WindowsRuntimeProjections.IsClrImplementationType(typeDefinition2))
				{
					WindowsRuntimeProjections.Project(typeDefinition2);
					this.WriteTypeReference(value);
					WindowsRuntimeProjections.RemoveProjection(typeDefinition2);
					return;
				}
			}
			this.WriteTypeReference(value);
		}

		// Token: 0x06000B9B RID: 2971 RVA: 0x000291AC File Offset: 0x000273AC
		private void WritePrimitiveValue(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException();
			}
			switch (Type.GetTypeCode(value.GetType()))
			{
			case TypeCode.Boolean:
				base.WriteByte(((bool)value > false) ? 1 : 0);
				return;
			case TypeCode.Char:
				base.WriteInt16((short)((char)value));
				return;
			case TypeCode.SByte:
				base.WriteSByte((sbyte)value);
				return;
			case TypeCode.Byte:
				base.WriteByte((byte)value);
				return;
			case TypeCode.Int16:
				base.WriteInt16((short)value);
				return;
			case TypeCode.UInt16:
				base.WriteUInt16((ushort)value);
				return;
			case TypeCode.Int32:
				base.WriteInt32((int)value);
				return;
			case TypeCode.UInt32:
				base.WriteUInt32((uint)value);
				return;
			case TypeCode.Int64:
				base.WriteInt64((long)value);
				return;
			case TypeCode.UInt64:
				base.WriteUInt64((ulong)value);
				return;
			case TypeCode.Single:
				base.WriteSingle((float)value);
				return;
			case TypeCode.Double:
				base.WriteDouble((double)value);
				return;
			default:
				throw new NotSupportedException(value.GetType().FullName);
			}
		}

		// Token: 0x06000B9C RID: 2972 RVA: 0x000292BC File Offset: 0x000274BC
		private void WriteCustomAttributeEnumValue(TypeReference enum_type, object value)
		{
			TypeDefinition typeDefinition = enum_type.CheckedResolve();
			if (!typeDefinition.IsEnum)
			{
				throw new ArgumentException();
			}
			this.WriteCustomAttributeValue(typeDefinition.GetEnumUnderlyingType(), value);
		}

		// Token: 0x06000B9D RID: 2973 RVA: 0x000292EC File Offset: 0x000274EC
		private void WriteCustomAttributeFieldOrPropType(TypeReference type)
		{
			if (type.IsArray)
			{
				ArrayType arrayType = (ArrayType)type;
				this.WriteElementType(ElementType.SzArray);
				this.WriteCustomAttributeFieldOrPropType(arrayType.ElementType);
				return;
			}
			ElementType etype = type.etype;
			if (etype != ElementType.None)
			{
				if (etype == ElementType.GenericInst)
				{
					this.WriteElementType(ElementType.Enum);
					this.WriteTypeReference(type);
					return;
				}
				if (etype == ElementType.Object)
				{
					this.WriteElementType(ElementType.Boxed);
					return;
				}
				this.WriteElementType(etype);
				return;
			}
			else
			{
				if (type.IsTypeOf("System", "Type"))
				{
					this.WriteElementType(ElementType.Type);
					return;
				}
				this.WriteElementType(ElementType.Enum);
				this.WriteTypeReference(type);
				return;
			}
		}

		// Token: 0x06000B9E RID: 2974 RVA: 0x0002937C File Offset: 0x0002757C
		public void WriteCustomAttributeNamedArguments(CustomAttribute attribute)
		{
			int namedArgumentCount = SignatureWriter.GetNamedArgumentCount(attribute);
			base.WriteUInt16((ushort)namedArgumentCount);
			if (namedArgumentCount == 0)
			{
				return;
			}
			this.WriteICustomAttributeNamedArguments(attribute);
		}

		// Token: 0x06000B9F RID: 2975 RVA: 0x000293A4 File Offset: 0x000275A4
		private static int GetNamedArgumentCount(ICustomAttribute attribute)
		{
			int num = 0;
			if (attribute.HasFields)
			{
				num += attribute.Fields.Count;
			}
			if (attribute.HasProperties)
			{
				num += attribute.Properties.Count;
			}
			return num;
		}

		// Token: 0x06000BA0 RID: 2976 RVA: 0x000293E0 File Offset: 0x000275E0
		private void WriteICustomAttributeNamedArguments(ICustomAttribute attribute)
		{
			if (attribute.HasFields)
			{
				this.WriteCustomAttributeNamedArguments(83, attribute.Fields);
			}
			if (attribute.HasProperties)
			{
				this.WriteCustomAttributeNamedArguments(84, attribute.Properties);
			}
		}

		// Token: 0x06000BA1 RID: 2977 RVA: 0x00029410 File Offset: 0x00027610
		private void WriteCustomAttributeNamedArguments(byte kind, Collection<CustomAttributeNamedArgument> named_arguments)
		{
			for (int i = 0; i < named_arguments.Count; i++)
			{
				this.WriteCustomAttributeNamedArgument(kind, named_arguments[i]);
			}
		}

		// Token: 0x06000BA2 RID: 2978 RVA: 0x0002943C File Offset: 0x0002763C
		private void WriteCustomAttributeNamedArgument(byte kind, CustomAttributeNamedArgument named_argument)
		{
			CustomAttributeArgument argument = named_argument.Argument;
			base.WriteByte(kind);
			this.WriteCustomAttributeFieldOrPropType(argument.Type);
			this.WriteUTF8String(named_argument.Name);
			this.WriteCustomAttributeFixedArgument(argument.Type, argument);
		}

		// Token: 0x06000BA3 RID: 2979 RVA: 0x00029480 File Offset: 0x00027680
		private void WriteSecurityAttribute(SecurityAttribute attribute)
		{
			this.WriteTypeReference(attribute.AttributeType);
			int namedArgumentCount = SignatureWriter.GetNamedArgumentCount(attribute);
			if (namedArgumentCount == 0)
			{
				base.WriteCompressedUInt32(1U);
				base.WriteCompressedUInt32(0U);
				return;
			}
			SignatureWriter signatureWriter = new SignatureWriter(this.metadata);
			signatureWriter.WriteCompressedUInt32((uint)namedArgumentCount);
			signatureWriter.WriteICustomAttributeNamedArguments(attribute);
			base.WriteCompressedUInt32((uint)signatureWriter.length);
			base.WriteBytes(signatureWriter);
		}

		// Token: 0x06000BA4 RID: 2980 RVA: 0x000294E0 File Offset: 0x000276E0
		public void WriteSecurityDeclaration(SecurityDeclaration declaration)
		{
			base.WriteByte(46);
			Collection<SecurityAttribute> security_attributes = declaration.security_attributes;
			if (security_attributes == null)
			{
				throw new NotSupportedException();
			}
			base.WriteCompressedUInt32((uint)security_attributes.Count);
			for (int i = 0; i < security_attributes.Count; i++)
			{
				this.WriteSecurityAttribute(security_attributes[i]);
			}
		}

		// Token: 0x06000BA5 RID: 2981 RVA: 0x00029530 File Offset: 0x00027730
		public void WriteXmlSecurityDeclaration(SecurityDeclaration declaration)
		{
			string xmlSecurityDeclaration = SignatureWriter.GetXmlSecurityDeclaration(declaration);
			if (xmlSecurityDeclaration == null)
			{
				throw new NotSupportedException();
			}
			base.WriteBytes(Encoding.Unicode.GetBytes(xmlSecurityDeclaration));
		}

		// Token: 0x06000BA6 RID: 2982 RVA: 0x00029560 File Offset: 0x00027760
		private static string GetXmlSecurityDeclaration(SecurityDeclaration declaration)
		{
			if (declaration.security_attributes == null || declaration.security_attributes.Count != 1)
			{
				return null;
			}
			SecurityAttribute securityAttribute = declaration.security_attributes[0];
			if (!securityAttribute.AttributeType.IsTypeOf("System.Security.Permissions", "PermissionSetAttribute"))
			{
				return null;
			}
			if (securityAttribute.properties == null || securityAttribute.properties.Count != 1)
			{
				return null;
			}
			CustomAttributeNamedArgument customAttributeNamedArgument = securityAttribute.properties[0];
			if (customAttributeNamedArgument.Name != "XML")
			{
				return null;
			}
			return (string)customAttributeNamedArgument.Argument.Value;
		}

		// Token: 0x06000BA7 RID: 2983 RVA: 0x000295F9 File Offset: 0x000277F9
		private void WriteTypeReference(TypeReference type)
		{
			this.WriteUTF8String(TypeParser.ToParseable(type, false));
		}

		// Token: 0x06000BA8 RID: 2984 RVA: 0x00029608 File Offset: 0x00027808
		public void WriteMarshalInfo(MarshalInfo marshal_info)
		{
			this.WriteNativeType(marshal_info.native);
			NativeType native = marshal_info.native;
			if (native <= NativeType.SafeArray)
			{
				if (native == NativeType.FixedSysString)
				{
					FixedSysStringMarshalInfo fixedSysStringMarshalInfo = (FixedSysStringMarshalInfo)marshal_info;
					if (fixedSysStringMarshalInfo.size > -1)
					{
						base.WriteCompressedUInt32((uint)fixedSysStringMarshalInfo.size);
					}
					return;
				}
				if (native != NativeType.SafeArray)
				{
					return;
				}
				SafeArrayMarshalInfo safeArrayMarshalInfo = (SafeArrayMarshalInfo)marshal_info;
				if (safeArrayMarshalInfo.element_type != VariantType.None)
				{
					this.WriteVariantType(safeArrayMarshalInfo.element_type);
				}
				return;
			}
			else
			{
				if (native == NativeType.FixedArray)
				{
					FixedArrayMarshalInfo fixedArrayMarshalInfo = (FixedArrayMarshalInfo)marshal_info;
					if (fixedArrayMarshalInfo.size > -1)
					{
						base.WriteCompressedUInt32((uint)fixedArrayMarshalInfo.size);
					}
					if (fixedArrayMarshalInfo.element_type != NativeType.None)
					{
						this.WriteNativeType(fixedArrayMarshalInfo.element_type);
					}
					return;
				}
				if (native == NativeType.Array)
				{
					ArrayMarshalInfo arrayMarshalInfo = (ArrayMarshalInfo)marshal_info;
					if (arrayMarshalInfo.element_type != NativeType.None)
					{
						this.WriteNativeType(arrayMarshalInfo.element_type);
					}
					if (arrayMarshalInfo.size_parameter_index > -1)
					{
						base.WriteCompressedUInt32((uint)arrayMarshalInfo.size_parameter_index);
					}
					if (arrayMarshalInfo.size > -1)
					{
						base.WriteCompressedUInt32((uint)arrayMarshalInfo.size);
					}
					if (arrayMarshalInfo.size_parameter_multiplier > -1)
					{
						base.WriteCompressedUInt32((uint)arrayMarshalInfo.size_parameter_multiplier);
					}
					return;
				}
				if (native != NativeType.CustomMarshaler)
				{
					return;
				}
				CustomMarshalInfo customMarshalInfo = (CustomMarshalInfo)marshal_info;
				this.WriteUTF8String((customMarshalInfo.guid != Guid.Empty) ? customMarshalInfo.guid.ToString() : string.Empty);
				this.WriteUTF8String(customMarshalInfo.unmanaged_type);
				this.WriteTypeReference(customMarshalInfo.managed_type);
				this.WriteUTF8String(customMarshalInfo.cookie);
				return;
			}
		}

		// Token: 0x06000BA9 RID: 2985 RVA: 0x0002977D File Offset: 0x0002797D
		private void WriteNativeType(NativeType native)
		{
			base.WriteByte((byte)native);
		}

		// Token: 0x06000BAA RID: 2986 RVA: 0x0002977D File Offset: 0x0002797D
		private void WriteVariantType(VariantType variant)
		{
			base.WriteByte((byte)variant);
		}

		// Token: 0x06000BAB RID: 2987 RVA: 0x00029788 File Offset: 0x00027988
		public void WriteSequencePoints(MethodDebugInformation info)
		{
			int num = -1;
			int num2 = -1;
			base.WriteCompressedUInt32(info.local_var_token.RID);
			Document document;
			if (!info.TryGetUniqueDocument(out document))
			{
				document = null;
			}
			for (int i = 0; i < info.SequencePoints.Count; i++)
			{
				SequencePoint sequencePoint = info.SequencePoints[i];
				Document document2 = sequencePoint.Document;
				if (document != document2)
				{
					MetadataToken documentToken = this.metadata.GetDocumentToken(document2);
					if (document != null)
					{
						base.WriteCompressedUInt32(0U);
					}
					base.WriteCompressedUInt32(documentToken.RID);
					document = document2;
				}
				if (i > 0)
				{
					base.WriteCompressedUInt32((uint)(sequencePoint.Offset - info.SequencePoints[i - 1].Offset));
				}
				else
				{
					base.WriteCompressedUInt32((uint)sequencePoint.Offset);
				}
				if (sequencePoint.IsHidden)
				{
					base.WriteInt16(0);
				}
				else
				{
					int num3 = sequencePoint.EndLine - sequencePoint.StartLine;
					int num4 = sequencePoint.EndColumn - sequencePoint.StartColumn;
					base.WriteCompressedUInt32((uint)num3);
					if (num3 == 0)
					{
						base.WriteCompressedUInt32((uint)num4);
					}
					else
					{
						base.WriteCompressedInt32(num4);
					}
					if (num < 0)
					{
						base.WriteCompressedUInt32((uint)sequencePoint.StartLine);
						base.WriteCompressedUInt32((uint)sequencePoint.StartColumn);
					}
					else
					{
						base.WriteCompressedInt32(sequencePoint.StartLine - num);
						base.WriteCompressedInt32(sequencePoint.StartColumn - num2);
					}
					num = sequencePoint.StartLine;
					num2 = sequencePoint.StartColumn;
				}
			}
		}

		// Token: 0x04000388 RID: 904
		private readonly MetadataBuilder metadata;
	}
}
