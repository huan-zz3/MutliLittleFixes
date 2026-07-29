using System;
using System.Collections.Generic;

namespace Mono.Cecil
{
	// Token: 0x02000270 RID: 624
	internal sealed class MethodReferenceComparer : EqualityComparer<MethodReference>
	{
		// Token: 0x06000EBC RID: 3772 RVA: 0x0002F69D File Offset: 0x0002D89D
		public override bool Equals(MethodReference x, MethodReference y)
		{
			return MethodReferenceComparer.AreEqual(x, y);
		}

		// Token: 0x06000EBD RID: 3773 RVA: 0x0002F6A6 File Offset: 0x0002D8A6
		public override int GetHashCode(MethodReference obj)
		{
			return MethodReferenceComparer.GetHashCodeFor(obj);
		}

		// Token: 0x06000EBE RID: 3774 RVA: 0x0002F6B0 File Offset: 0x0002D8B0
		public static bool AreEqual(MethodReference x, MethodReference y)
		{
			if (x == y)
			{
				return true;
			}
			if (x.HasThis != y.HasThis)
			{
				return false;
			}
			if (x.HasParameters != y.HasParameters)
			{
				return false;
			}
			if (x.HasGenericParameters != y.HasGenericParameters)
			{
				return false;
			}
			if (x.Parameters.Count != y.Parameters.Count)
			{
				return false;
			}
			if (x.Name != y.Name)
			{
				return false;
			}
			if (!TypeReferenceEqualityComparer.AreEqual(x.DeclaringType, y.DeclaringType, TypeComparisonMode.Exact))
			{
				return false;
			}
			GenericInstanceMethod genericInstanceMethod = x as GenericInstanceMethod;
			GenericInstanceMethod genericInstanceMethod2 = y as GenericInstanceMethod;
			if (genericInstanceMethod != null || genericInstanceMethod2 != null)
			{
				if (genericInstanceMethod == null || genericInstanceMethod2 == null)
				{
					return false;
				}
				if (genericInstanceMethod.GenericArguments.Count != genericInstanceMethod2.GenericArguments.Count)
				{
					return false;
				}
				for (int i = 0; i < genericInstanceMethod.GenericArguments.Count; i++)
				{
					if (!TypeReferenceEqualityComparer.AreEqual(genericInstanceMethod.GenericArguments[i], genericInstanceMethod2.GenericArguments[i], TypeComparisonMode.Exact))
					{
						return false;
					}
				}
			}
			MethodDefinition methodDefinition = x.Resolve();
			MethodDefinition methodDefinition2 = y.Resolve();
			if (methodDefinition != methodDefinition2)
			{
				return false;
			}
			if (methodDefinition == null)
			{
				if (MethodReferenceComparer.xComparisonStack == null)
				{
					MethodReferenceComparer.xComparisonStack = new List<MethodReference>();
				}
				if (MethodReferenceComparer.yComparisonStack == null)
				{
					MethodReferenceComparer.yComparisonStack = new List<MethodReference>();
				}
				for (int j = 0; j < MethodReferenceComparer.xComparisonStack.Count; j++)
				{
					if (MethodReferenceComparer.xComparisonStack[j] == x && MethodReferenceComparer.yComparisonStack[j] == y)
					{
						return true;
					}
				}
				MethodReferenceComparer.xComparisonStack.Add(x);
				try
				{
					MethodReferenceComparer.yComparisonStack.Add(y);
					try
					{
						for (int k = 0; k < x.Parameters.Count; k++)
						{
							if (!TypeReferenceEqualityComparer.AreEqual(x.Parameters[k].ParameterType, y.Parameters[k].ParameterType, TypeComparisonMode.Exact))
							{
								return false;
							}
						}
					}
					finally
					{
						MethodReferenceComparer.yComparisonStack.RemoveAt(MethodReferenceComparer.yComparisonStack.Count - 1);
					}
				}
				finally
				{
					MethodReferenceComparer.xComparisonStack.RemoveAt(MethodReferenceComparer.xComparisonStack.Count - 1);
				}
				return true;
			}
			return true;
		}

		// Token: 0x06000EBF RID: 3775 RVA: 0x0002F8D4 File Offset: 0x0002DAD4
		public static bool AreSignaturesEqual(MethodReference x, MethodReference y, TypeComparisonMode comparisonMode = TypeComparisonMode.Exact)
		{
			if (x.HasThis != y.HasThis)
			{
				return false;
			}
			if (x.Parameters.Count != y.Parameters.Count)
			{
				return false;
			}
			if (x.GenericParameters.Count != y.GenericParameters.Count)
			{
				return false;
			}
			for (int i = 0; i < x.Parameters.Count; i++)
			{
				if (!TypeReferenceEqualityComparer.AreEqual(x.Parameters[i].ParameterType, y.Parameters[i].ParameterType, comparisonMode))
				{
					return false;
				}
			}
			return TypeReferenceEqualityComparer.AreEqual(x.ReturnType, y.ReturnType, comparisonMode);
		}

		// Token: 0x06000EC0 RID: 3776 RVA: 0x0002F980 File Offset: 0x0002DB80
		public static int GetHashCodeFor(MethodReference obj)
		{
			GenericInstanceMethod genericInstanceMethod = obj as GenericInstanceMethod;
			if (genericInstanceMethod != null)
			{
				int num = MethodReferenceComparer.GetHashCodeFor(genericInstanceMethod.ElementMethod);
				for (int i = 0; i < genericInstanceMethod.GenericArguments.Count; i++)
				{
					num = num * 486187739 + TypeReferenceEqualityComparer.GetHashCodeFor(genericInstanceMethod.GenericArguments[i]);
				}
				return num;
			}
			return TypeReferenceEqualityComparer.GetHashCodeFor(obj.DeclaringType) * 486187739 + obj.Name.GetHashCode();
		}

		// Token: 0x04000487 RID: 1159
		[ThreadStatic]
		private static List<MethodReference> xComparisonStack;

		// Token: 0x04000488 RID: 1160
		[ThreadStatic]
		private static List<MethodReference> yComparisonStack;
	}
}
