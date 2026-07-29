using System;
using Mono.Collections.Generic;

namespace Mono.Cecil.Cil
{
	// Token: 0x020002F2 RID: 754
	internal sealed class VariableDefinitionCollection : Collection<VariableDefinition>
	{
		// Token: 0x060013B8 RID: 5048 RVA: 0x0003E15E File Offset: 0x0003C35E
		internal VariableDefinitionCollection(MethodDefinition method)
		{
			this.method = method;
		}

		// Token: 0x060013B9 RID: 5049 RVA: 0x0003E16D File Offset: 0x0003C36D
		internal VariableDefinitionCollection(MethodDefinition method, int capacity)
			: base(capacity)
		{
			this.method = method;
		}

		// Token: 0x060013BA RID: 5050 RVA: 0x0003E17D File Offset: 0x0003C37D
		protected override void OnAdd(VariableDefinition item, int index)
		{
			item.index = index;
		}

		// Token: 0x060013BB RID: 5051 RVA: 0x0003E186 File Offset: 0x0003C386
		protected override void OnInsert(VariableDefinition item, int index)
		{
			item.index = index;
			this.UpdateVariableIndices(index, 1, null);
		}

		// Token: 0x060013BC RID: 5052 RVA: 0x0003E17D File Offset: 0x0003C37D
		protected override void OnSet(VariableDefinition item, int index)
		{
			item.index = index;
		}

		// Token: 0x060013BD RID: 5053 RVA: 0x0003E198 File Offset: 0x0003C398
		protected override void OnRemove(VariableDefinition item, int index)
		{
			this.UpdateVariableIndices(index + 1, -1, item);
			item.index = -1;
		}

		// Token: 0x060013BE RID: 5054 RVA: 0x0003E1AC File Offset: 0x0003C3AC
		private void UpdateVariableIndices(int startIndex, int offset, VariableDefinition variableToRemove = null)
		{
			for (int i = startIndex; i < this.size; i++)
			{
				this.items[i].index = i + offset;
			}
			MethodDebugInformation methodDebugInformation = ((this.method == null) ? null : this.method.debug_info);
			if (methodDebugInformation == null || methodDebugInformation.Scope == null)
			{
				return;
			}
			foreach (ScopeDebugInformation scopeDebugInformation in methodDebugInformation.GetScopes())
			{
				if (scopeDebugInformation.HasVariables)
				{
					Collection<VariableDebugInformation> variables = scopeDebugInformation.Variables;
					int num = -1;
					for (int j = 0; j < variables.Count; j++)
					{
						VariableDebugInformation variableDebugInformation = variables[j];
						if (variableToRemove != null && ((variableDebugInformation.index.IsResolved && variableDebugInformation.index.ResolvedVariable == variableToRemove) || (!variableDebugInformation.index.IsResolved && variableDebugInformation.Index == variableToRemove.Index)))
						{
							num = j;
						}
						else if (!variableDebugInformation.index.IsResolved && variableDebugInformation.Index >= startIndex)
						{
							variableDebugInformation.index = new VariableIndex(variableDebugInformation.Index + offset);
						}
					}
					if (num >= 0)
					{
						variables.RemoveAt(num);
					}
				}
			}
		}

		// Token: 0x040008B9 RID: 2233
		private readonly MethodDefinition method;
	}
}
