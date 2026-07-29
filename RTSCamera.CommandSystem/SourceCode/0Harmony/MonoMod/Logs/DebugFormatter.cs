using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using MonoMod.Utils;

namespace MonoMod.Logs
{
	// Token: 0x02000815 RID: 2069
	internal static class DebugFormatter
	{
		// Token: 0x06002790 RID: 10128 RVA: 0x0008813C File Offset: 0x0008633C
		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool CanDebugFormat<T>([Nullable(1)] in T value, out object extraData)
		{
			extraData = null;
			if (typeof(T) == typeof(Type))
			{
				return true;
			}
			if (typeof(T) == typeof(MethodBase))
			{
				return true;
			}
			if (typeof(T) == typeof(MethodInfo))
			{
				return true;
			}
			if (typeof(T) == typeof(ConstructorInfo))
			{
				return true;
			}
			if (typeof(T) == typeof(FieldInfo))
			{
				return true;
			}
			if (typeof(T) == typeof(PropertyInfo))
			{
				return true;
			}
			if (typeof(T) == typeof(Exception))
			{
				return true;
			}
			if (typeof(T) == typeof(IDebugFormattable))
			{
				return true;
			}
			T t = value;
			bool flag = t is Type || t is MethodBase || t is FieldInfo || t is PropertyInfo;
			if (flag)
			{
				return true;
			}
			Exception ex = value as Exception;
			if (ex != null)
			{
				extraData = ex.ToString();
				return true;
			}
			return value is IDebugFormattable;
		}

		// Token: 0x06002791 RID: 10129 RVA: 0x000882B0 File Offset: 0x000864B0
		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static bool TryFormatInto<T>([Nullable(1)] in T value, object extraData, [Nullable(0)] Span<char> into, out int wrote)
		{
			if (default(T) == null && value == null)
			{
				wrote = 0;
				return true;
			}
			if (typeof(T) == typeof(Type))
			{
				return DebugFormatter.TryFormatType(*DebugFormatter.<TryFormatInto>g__Transmute|1_0<T, Type>(in value), into, out wrote);
			}
			if (typeof(T) == typeof(MethodInfo))
			{
				return DebugFormatter.TryFormatMethodInfo(*DebugFormatter.<TryFormatInto>g__Transmute|1_0<T, MethodInfo>(in value), into, out wrote);
			}
			if (typeof(T) == typeof(ConstructorInfo))
			{
				return DebugFormatter.TryFormatMethodBase(*DebugFormatter.<TryFormatInto>g__Transmute|1_0<T, ConstructorInfo>(in value), into, out wrote);
			}
			if (typeof(T) == typeof(FieldInfo))
			{
				return DebugFormatter.TryFormatFieldInfo(*DebugFormatter.<TryFormatInto>g__Transmute|1_0<T, FieldInfo>(in value), into, out wrote);
			}
			if (typeof(T) == typeof(PropertyInfo))
			{
				return DebugFormatter.TryFormatPropertyInfo(*DebugFormatter.<TryFormatInto>g__Transmute|1_0<T, PropertyInfo>(in value), into, out wrote);
			}
			if (typeof(T) == typeof(Exception))
			{
				return DebugFormatter.TryFormatException(*DebugFormatter.<TryFormatInto>g__Transmute|1_0<T, Exception>(in value), Unsafe.As<string>(extraData), into, out wrote);
			}
			if (typeof(T) == typeof(IDebugFormattable))
			{
				return DebugFormatter.<TryFormatInto>g__Transmute|1_0<T, IDebugFormattable>(in value)->TryFormatInto(into, out wrote);
			}
			Type type = value as Type;
			if (type != null)
			{
				return DebugFormatter.TryFormatType(type, into, out wrote);
			}
			MethodInfo methodInfo = value as MethodInfo;
			if (methodInfo != null)
			{
				return DebugFormatter.TryFormatMethodInfo(methodInfo, into, out wrote);
			}
			ConstructorInfo constructorInfo = value as ConstructorInfo;
			if (constructorInfo != null)
			{
				return DebugFormatter.TryFormatMethodBase(constructorInfo, into, out wrote);
			}
			MethodBase methodBase = value as MethodBase;
			if (methodBase != null)
			{
				return DebugFormatter.TryFormatMethodBase(methodBase, into, out wrote);
			}
			FieldInfo fieldInfo = value as FieldInfo;
			if (fieldInfo != null)
			{
				return DebugFormatter.TryFormatFieldInfo(fieldInfo, into, out wrote);
			}
			PropertyInfo propertyInfo = value as PropertyInfo;
			if (propertyInfo != null)
			{
				return DebugFormatter.TryFormatPropertyInfo(propertyInfo, into, out wrote);
			}
			Exception ex = value as Exception;
			if (ex != null)
			{
				return DebugFormatter.TryFormatException(ex, Unsafe.As<string>(extraData), into, out wrote);
			}
			if (value is IDebugFormattable)
			{
				return ((IDebugFormattable)((object)value)).TryFormatInto(into, out wrote);
			}
			bool flag = false;
			bool flag2 = flag;
			bool flag3;
			AssertionInterpolatedStringHandler assertionInterpolatedStringHandler = new AssertionInterpolatedStringHandler(48, 1, flag, out flag3);
			if (flag3)
			{
				assertionInterpolatedStringHandler.AppendLiteral("Called TryFormatInto with value of unknown type ");
				T t = value;
				assertionInterpolatedStringHandler.AppendFormatted<Type>(t.GetType());
			}
			Helpers.Assert(flag2, ref assertionInterpolatedStringHandler, "false");
			wrote = 0;
			return false;
		}

		// Token: 0x06002792 RID: 10130 RVA: 0x00088560 File Offset: 0x00086760
		private unsafe static bool TryFormatException([Nullable(1)] Exception e, [Nullable(2)] string eStr, Span<char> into, out int wrote)
		{
			wrote = 0;
			if (eStr == null)
			{
				eStr = e.ToString();
			}
			string newLine = Environment.NewLine;
			if (into.Slice(wrote).Length < eStr.Length)
			{
				return false;
			}
			eStr.AsSpan().CopyTo(into.Slice(wrote));
			wrote += eStr.Length;
			ReflectionTypeLoadException ex = e as ReflectionTypeLoadException;
			if (ex != null)
			{
				int num = 0;
				while (num < 4 && num < ex.Types.Length)
				{
					Span<char> span = into.Slice(wrote);
					Span<char> span2 = span;
					bool flag;
					FormatIntoInterpolatedStringHandler formatIntoInterpolatedStringHandler = new FormatIntoInterpolatedStringHandler(56, 3, span, out flag);
					if (flag && formatIntoInterpolatedStringHandler.AppendFormatted(newLine) && formatIntoInterpolatedStringHandler.AppendLiteral("System.Reflection.ReflectionTypeLoadException.Types[") && formatIntoInterpolatedStringHandler.AppendFormatted<int>(num) && formatIntoInterpolatedStringHandler.AppendLiteral("] = "))
					{
						formatIntoInterpolatedStringHandler.AppendFormatted<Type>(ex.Types[num]);
					}
					int num2;
					if (!DebugFormatter.Into(span2, out num2, ref formatIntoInterpolatedStringHandler))
					{
						return false;
					}
					wrote += num2;
					num++;
				}
				if (ex.Types.Length >= 4)
				{
					Span<char> span = into.Slice(wrote);
					Span<char> span3 = span;
					bool flag2;
					FormatIntoInterpolatedStringHandler formatIntoInterpolatedStringHandler2 = new FormatIntoInterpolatedStringHandler(62, 1, span, out flag2);
					if (flag2 && formatIntoInterpolatedStringHandler2.AppendFormatted(newLine))
					{
						formatIntoInterpolatedStringHandler2.AppendLiteral("System.Reflection.ReflectionTypeLoadException.Types[...] = ...");
					}
					int num2;
					if (!DebugFormatter.Into(span3, out num2, ref formatIntoInterpolatedStringHandler2))
					{
						return false;
					}
					wrote += num2;
				}
				if (ex.LoaderExceptions.Length != 0)
				{
					if (into.Slice(wrote).Length < newLine.Length + "System.Reflection.ReflectionTypeLoadException.LoaderExceptions = [".Length)
					{
						return false;
					}
					newLine.AsSpan().CopyTo(into.Slice(wrote));
					wrote += newLine.Length;
					"System.Reflection.ReflectionTypeLoadException.LoaderExceptions = [".AsSpan().CopyTo(into.Slice(wrote));
					wrote += "System.Reflection.ReflectionTypeLoadException.LoaderExceptions = [".Length;
					for (int i = 0; i < ex.LoaderExceptions.Length; i++)
					{
						Exception ex2 = ex.LoaderExceptions[i];
						if (ex2 != null)
						{
							if (into.Slice(wrote).Length < newLine.Length)
							{
								return false;
							}
							newLine.AsSpan().CopyTo(into.Slice(wrote));
							wrote += newLine.Length;
							int num2;
							if (!DebugFormatter.TryFormatException(ex2, null, into.Slice(wrote), out num2))
							{
								return false;
							}
							wrote += num2;
						}
					}
					if (into.Slice(wrote).Length < newLine.Length + 1)
					{
						return false;
					}
					newLine.AsSpan().CopyTo(into.Slice(wrote));
					wrote += newLine.Length;
					int num3 = wrote;
					wrote = num3 + 1;
					*into[num3] = ']';
				}
			}
			TypeLoadException ex3 = e as TypeLoadException;
			if (ex3 != null)
			{
				Span<char> span = into.Slice(wrote);
				Span<char> span4 = span;
				bool flag3;
				FormatIntoInterpolatedStringHandler formatIntoInterpolatedStringHandler3 = new FormatIntoInterpolatedStringHandler(36, 2, span, out flag3);
				if (flag3 && formatIntoInterpolatedStringHandler3.AppendFormatted(newLine) && formatIntoInterpolatedStringHandler3.AppendLiteral("System.TypeLoadException.TypeName = "))
				{
					formatIntoInterpolatedStringHandler3.AppendFormatted(ex3.TypeName);
				}
				int num2;
				if (!DebugFormatter.Into(span4, out num2, ref formatIntoInterpolatedStringHandler3))
				{
					return false;
				}
				wrote += num2;
			}
			BadImageFormatException ex4 = e as BadImageFormatException;
			if (ex4 != null)
			{
				Span<char> span = into.Slice(wrote);
				Span<char> span5 = span;
				bool flag4;
				FormatIntoInterpolatedStringHandler formatIntoInterpolatedStringHandler4 = new FormatIntoInterpolatedStringHandler(42, 2, span, out flag4);
				if (flag4 && formatIntoInterpolatedStringHandler4.AppendFormatted(newLine) && formatIntoInterpolatedStringHandler4.AppendLiteral("System.BadImageFormatException.FileName = "))
				{
					formatIntoInterpolatedStringHandler4.AppendFormatted(ex4.FileName);
				}
				int num2;
				if (!DebugFormatter.Into(span5, out num2, ref formatIntoInterpolatedStringHandler4))
				{
					return false;
				}
				wrote += num2;
			}
			return true;
		}

		// Token: 0x06002793 RID: 10131 RVA: 0x000888E4 File Offset: 0x00086AE4
		private static bool TryFormatType([Nullable(1)] Type type, Span<char> into, out int wrote)
		{
			wrote = 0;
			string text;
			if (type.HasElementType && type.GetElementType() == null)
			{
				text = type.Name;
			}
			else
			{
				string fullName = type.FullName;
				if (fullName == null)
				{
					return true;
				}
				text = fullName;
			}
			if (into.Length < text.Length)
			{
				return false;
			}
			text.AsSpan().CopyTo(into);
			wrote = text.Length;
			return true;
		}

		// Token: 0x06002794 RID: 10132 RVA: 0x0008894C File Offset: 0x00086B4C
		private unsafe static bool TryFormatMethodInfo([Nullable(1)] MethodInfo method, Span<char> into, out int wrote)
		{
			Type returnType = method.ReturnType;
			wrote = 0;
			int num;
			if (!DebugFormatter.TryFormatType(returnType, into.Slice(wrote), out num))
			{
				return false;
			}
			wrote += num;
			if (into.Slice(wrote).Length < 1)
			{
				return false;
			}
			int num2 = wrote;
			wrote = num2 + 1;
			*into[num2] = ' ';
			if (!DebugFormatter.TryFormatMethodBase(method, into.Slice(wrote), out num))
			{
				return false;
			}
			wrote += num;
			return true;
		}

		// Token: 0x06002795 RID: 10133 RVA: 0x000889C4 File Offset: 0x00086BC4
		private unsafe static bool TryFormatMemberInfoName([Nullable(1)] MemberInfo member, Span<char> into, out int wrote)
		{
			wrote = 0;
			Type declaringType = member.DeclaringType;
			if (declaringType != null)
			{
				int num;
				if (!DebugFormatter.TryFormatType(declaringType, into.Slice(wrote), out num))
				{
					return false;
				}
				wrote += num;
				if (into.Slice(wrote).Length < 1)
				{
					return false;
				}
				int num2 = wrote;
				wrote = num2 + 1;
				*into[num2] = ':';
			}
			string name = member.Name;
			if (into.Slice(wrote).Length < name.Length)
			{
				return false;
			}
			name.AsSpan().CopyTo(into.Slice(wrote));
			wrote += name.Length;
			return true;
		}

		// Token: 0x06002796 RID: 10134 RVA: 0x00088A6C File Offset: 0x00086C6C
		private unsafe static bool TryFormatMethodBase([Nullable(1)] MethodBase method, Span<char> into, out int wrote)
		{
			wrote = 0;
			int num;
			if (!DebugFormatter.TryFormatMemberInfoName(method, into.Slice(wrote), out num))
			{
				return false;
			}
			wrote += num;
			int num2;
			if (method.IsGenericMethod)
			{
				if (into.Slice(wrote).Length < 1)
				{
					return false;
				}
				num2 = wrote;
				wrote = num2 + 1;
				*into[num2] = '<';
				Type[] genericArguments = method.GetGenericArguments();
				for (int i = 0; i < genericArguments.Length; i++)
				{
					if (i != 0)
					{
						if (into.Slice(wrote).Length < 2)
						{
							return false;
						}
						num2 = wrote;
						wrote = num2 + 1;
						*into[num2] = ',';
						num2 = wrote;
						wrote = num2 + 1;
						*into[num2] = ' ';
					}
					if (!DebugFormatter.TryFormatType(genericArguments[i], into.Slice(wrote), out num))
					{
						return false;
					}
					wrote += num;
				}
				if (into.Slice(wrote).Length < 1)
				{
					return false;
				}
				num2 = wrote;
				wrote = num2 + 1;
				*into[num2] = '>';
			}
			ParameterInfo[] parameters = method.GetParameters();
			if (into.Slice(wrote).Length < 1)
			{
				return false;
			}
			num2 = wrote;
			wrote = num2 + 1;
			*into[num2] = '(';
			for (int j = 0; j < parameters.Length; j++)
			{
				if (j != 0)
				{
					if (into.Slice(wrote).Length < 2)
					{
						return false;
					}
					num2 = wrote;
					wrote = num2 + 1;
					*into[num2] = ',';
					num2 = wrote;
					wrote = num2 + 1;
					*into[num2] = ' ';
				}
				if (!DebugFormatter.TryFormatType(parameters[j].ParameterType, into.Slice(wrote), out num))
				{
					return false;
				}
				wrote += num;
			}
			if (into.Slice(wrote).Length < 1)
			{
				return false;
			}
			num2 = wrote;
			wrote = num2 + 1;
			*into[num2] = ')';
			return true;
		}

		// Token: 0x06002797 RID: 10135 RVA: 0x00088C54 File Offset: 0x00086E54
		private unsafe static bool TryFormatFieldInfo([Nullable(1)] FieldInfo field, Span<char> into, out int wrote)
		{
			wrote = 0;
			int num;
			if (!DebugFormatter.TryFormatType(field.FieldType, into.Slice(wrote), out num))
			{
				return false;
			}
			wrote += num;
			if (into.Slice(wrote).Length < 1)
			{
				return false;
			}
			int num2 = wrote;
			wrote = num2 + 1;
			*into[num2] = ' ';
			if (!DebugFormatter.TryFormatMemberInfoName(field, into.Slice(wrote), out num))
			{
				return false;
			}
			wrote += num;
			return true;
		}

		// Token: 0x06002798 RID: 10136 RVA: 0x00088CCC File Offset: 0x00086ECC
		private unsafe static bool TryFormatPropertyInfo([Nullable(1)] PropertyInfo prop, Span<char> into, out int wrote)
		{
			wrote = 0;
			int num;
			if (!DebugFormatter.TryFormatType(prop.PropertyType, into.Slice(wrote), out num))
			{
				return false;
			}
			wrote += num;
			if (into.Slice(wrote).Length < 1)
			{
				return false;
			}
			int num2 = wrote;
			wrote = num2 + 1;
			*into[num2] = ' ';
			if (!DebugFormatter.TryFormatMemberInfoName(prop, into.Slice(wrote), out num))
			{
				return false;
			}
			wrote += num;
			bool canRead = prop.CanRead;
			bool canWrite = prop.CanWrite;
			int num3 = 5 + (canRead ? 4 : 0) + (canWrite ? 4 : 0) + ((canRead && canWrite) ? 1 : 0);
			if (into.Slice(wrote).Length < num3)
			{
				return false;
			}
			" { ".AsSpan().CopyTo(into.Slice(wrote));
			wrote += 3;
			if (canRead)
			{
				"get;".AsSpan().CopyTo(into.Slice(wrote));
				wrote += 4;
			}
			if (canRead && canWrite)
			{
				num2 = wrote;
				wrote = num2 + 1;
				*into[num2] = ' ';
			}
			if (canWrite)
			{
				"set;".AsSpan().CopyTo(into.Slice(wrote));
				wrote += 4;
			}
			" }".AsSpan().CopyTo(into.Slice(wrote));
			wrote += 2;
			return true;
		}

		// Token: 0x06002799 RID: 10137 RVA: 0x00088E2F File Offset: 0x0008702F
		[NullableContext(1)]
		public static string Format(ref FormatInterpolatedStringHandler handler)
		{
			return handler.ToStringAndClear();
		}

		// Token: 0x0600279A RID: 10138 RVA: 0x00088E37 File Offset: 0x00087037
		public static bool Into(Span<char> into, out int wrote, [InterpolatedStringHandlerArgument("into")] ref FormatIntoInterpolatedStringHandler handler)
		{
			wrote = handler.pos;
			return !handler.incomplete;
		}

		// Token: 0x0600279B RID: 10139 RVA: 0x00088E4A File Offset: 0x0008704A
		[CompilerGenerated]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static ref TOut <TryFormatInto>g__Transmute|1_0<T, TOut>(in T val)
		{
			return Unsafe.As<T, TOut>(Unsafe.AsRef<T>(in val));
		}
	}
}
