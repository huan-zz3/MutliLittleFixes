using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace HarmonyLib
{
	// Token: 0x020001B7 RID: 439
	public class CodeMatcher
	{
		// Token: 0x170001ED RID: 493
		// (get) Token: 0x06000739 RID: 1849 RVA: 0x000178D4 File Offset: 0x00015AD4
		// (set) Token: 0x0600073A RID: 1850 RVA: 0x000178DC File Offset: 0x00015ADC
		public int Pos { get; private set; } = -1;

		// Token: 0x0600073B RID: 1851 RVA: 0x000178E5 File Offset: 0x00015AE5
		private void FixStart()
		{
			this.Pos = Math.Max(0, this.Pos);
		}

		// Token: 0x0600073C RID: 1852 RVA: 0x000178F9 File Offset: 0x00015AF9
		private T HandleException<T>(string error, T defaultValue)
		{
			if (this.errorHandler != null && this.errorHandler(this, error))
			{
				return defaultValue;
			}
			this.lastError = error;
			throw new InvalidOperationException(error);
		}

		// Token: 0x0600073D RID: 1853 RVA: 0x00017921 File Offset: 0x00015B21
		private void HandleException(string error)
		{
			this.lastError = error;
			if (this.errorHandler != null)
			{
				this.errorHandler(this, error);
				return;
			}
			throw new InvalidOperationException(error);
		}

		// Token: 0x0600073E RID: 1854 RVA: 0x00017947 File Offset: 0x00015B47
		private void SetOutOfBounds(int direction)
		{
			this.Pos = ((direction > 0) ? this.Length : (-1));
		}

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x0600073F RID: 1855 RVA: 0x0001795C File Offset: 0x00015B5C
		public int Length
		{
			get
			{
				return this.codes.Count;
			}
		}

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x06000740 RID: 1856 RVA: 0x00017969 File Offset: 0x00015B69
		public bool IsValid
		{
			get
			{
				return this.Pos >= 0 && this.Pos < this.Length;
			}
		}

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x06000741 RID: 1857 RVA: 0x00017984 File Offset: 0x00015B84
		public bool IsInvalid
		{
			get
			{
				return this.Pos < 0 || this.Pos >= this.Length;
			}
		}

		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x06000742 RID: 1858 RVA: 0x000179A2 File Offset: 0x00015BA2
		public int Remaining
		{
			get
			{
				return this.Length - Math.Max(0, this.Pos);
			}
		}

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x06000743 RID: 1859 RVA: 0x000179B7 File Offset: 0x00015BB7
		public ref OpCode Opcode
		{
			get
			{
				return ref this.codes[this.Pos].opcode;
			}
		}

		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x06000744 RID: 1860 RVA: 0x000179CF File Offset: 0x00015BCF
		public ref object Operand
		{
			get
			{
				return ref this.codes[this.Pos].operand;
			}
		}

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x06000745 RID: 1861 RVA: 0x000179E7 File Offset: 0x00015BE7
		public ref List<Label> Labels
		{
			get
			{
				return ref this.codes[this.Pos].labels;
			}
		}

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x06000746 RID: 1862 RVA: 0x000179FF File Offset: 0x00015BFF
		public ref List<ExceptionBlock> Blocks
		{
			get
			{
				return ref this.codes[this.Pos].blocks;
			}
		}

		// Token: 0x06000747 RID: 1863 RVA: 0x00017A17 File Offset: 0x00015C17
		public CodeMatcher()
		{
		}

		// Token: 0x06000748 RID: 1864 RVA: 0x00017A3C File Offset: 0x00015C3C
		public CodeMatcher(IEnumerable<CodeInstruction> instructions, ILGenerator generator = null)
		{
			this.generator = generator;
			this.codes = instructions.Select<CodeInstruction, CodeInstruction>((CodeInstruction c) => new CodeInstruction(c)).ToList<CodeInstruction>();
		}

		// Token: 0x06000749 RID: 1865 RVA: 0x00017AA4 File Offset: 0x00015CA4
		public CodeMatcher Clone()
		{
			return new CodeMatcher(this.codes, this.generator)
			{
				Pos = this.Pos,
				lastMatches = new Dictionary<string, CodeInstruction>(this.lastMatches),
				lastError = this.lastError,
				lastMatchCall = this.lastMatchCall,
				errorHandler = this.errorHandler
			};
		}

		// Token: 0x0600074A RID: 1866 RVA: 0x00017B03 File Offset: 0x00015D03
		public CodeMatcher Reset(bool atFirstInstruction = true)
		{
			this.Pos = (atFirstInstruction ? 0 : (-1));
			this.lastMatches.Clear();
			this.lastError = null;
			this.lastMatchCall = null;
			return this;
		}

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x0600074B RID: 1867 RVA: 0x00017B2C File Offset: 0x00015D2C
		public CodeInstruction Instruction
		{
			get
			{
				return this.codes[this.Pos];
			}
		}

		// Token: 0x0600074C RID: 1868 RVA: 0x00017B3F File Offset: 0x00015D3F
		public CodeInstruction InstructionAt(int offset)
		{
			return this.codes[this.Pos + offset];
		}

		// Token: 0x0600074D RID: 1869 RVA: 0x00017B54 File Offset: 0x00015D54
		public List<CodeInstruction> Instructions()
		{
			return this.codes;
		}

		// Token: 0x0600074E RID: 1870 RVA: 0x00017B5C File Offset: 0x00015D5C
		public IEnumerable<CodeInstruction> InstructionEnumeration()
		{
			return this.codes.AsEnumerable<CodeInstruction>();
		}

		// Token: 0x0600074F RID: 1871 RVA: 0x00017B6C File Offset: 0x00015D6C
		public List<CodeInstruction> Instructions(int count)
		{
			if (this.Pos < 0 || this.Pos + count > this.Length)
			{
				return this.HandleException<List<CodeInstruction>>("Cannot retrieve instructions: range is out-of-bounds.", new List<CodeInstruction>());
			}
			return (from c in this.codes.GetRange(this.Pos, count)
				select new CodeInstruction(c)).ToList<CodeInstruction>();
		}

		// Token: 0x06000750 RID: 1872 RVA: 0x00017BE0 File Offset: 0x00015DE0
		public List<CodeInstruction> InstructionsInRange(int start, int end)
		{
			List<CodeInstruction> range = this.codes;
			if (start > end)
			{
				int num = start;
				start = end;
				end = num;
			}
			if (start < 0 || end >= this.Length)
			{
				return this.HandleException<List<CodeInstruction>>("Cannot retrieve instructions: range is out-of-bounds.", new List<CodeInstruction>());
			}
			range = range.GetRange(start, end - start + 1);
			return range.Select<CodeInstruction, CodeInstruction>((CodeInstruction c) => new CodeInstruction(c)).ToList<CodeInstruction>();
		}

		// Token: 0x06000751 RID: 1873 RVA: 0x00017C53 File Offset: 0x00015E53
		public List<CodeInstruction> InstructionsWithOffsets(int startOffset, int endOffset)
		{
			return this.InstructionsInRange(this.Pos + startOffset, this.Pos + endOffset);
		}

		// Token: 0x06000752 RID: 1874 RVA: 0x00017C6B File Offset: 0x00015E6B
		public List<Label> DistinctLabels(IEnumerable<CodeInstruction> instructions)
		{
			return instructions.SelectMany<CodeInstruction, Label>((CodeInstruction instruction) => instruction.labels).Distinct<Label>().ToList<Label>();
		}

		// Token: 0x06000753 RID: 1875 RVA: 0x00017C9C File Offset: 0x00015E9C
		public bool ReportFailure(MethodBase method, Action<string> logger)
		{
			if (this.IsValid)
			{
				return false;
			}
			string text = this.lastError ?? "Unexpected code";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 2);
			defaultInterpolatedStringHandler.AppendFormatted(text);
			defaultInterpolatedStringHandler.AppendLiteral(" in ");
			defaultInterpolatedStringHandler.AppendFormatted<MethodBase>(method);
			logger(defaultInterpolatedStringHandler.ToStringAndClear());
			return true;
		}

		// Token: 0x06000754 RID: 1876 RVA: 0x00017CF6 File Offset: 0x00015EF6
		public CodeMatcher ThrowIfInvalid(string explanation)
		{
			if (explanation == null)
			{
				throw new ArgumentNullException("explanation");
			}
			if (this.IsInvalid)
			{
				return this.HandleException<CodeMatcher>(explanation + " - Current state is invalid", this);
			}
			return this;
		}

		// Token: 0x06000755 RID: 1877 RVA: 0x00017D22 File Offset: 0x00015F22
		public CodeMatcher ThrowIfNotMatch(string explanation, params CodeMatch[] matches)
		{
			this.ThrowIfInvalid(explanation);
			if (!this.MatchSequence(this.Pos, matches))
			{
				return this.HandleException<CodeMatcher>(explanation + " - Match failed", this);
			}
			return this;
		}

		// Token: 0x06000756 RID: 1878 RVA: 0x00017D50 File Offset: 0x00015F50
		private void ThrowIfNotMatch(string explanation, int direction, CodeMatch[] matches)
		{
			this.ThrowIfInvalid(explanation);
			int pos = this.Pos;
			try
			{
				if (this.Match(matches, direction, CodeMatcher.MatchPosition.Start, false).IsInvalid)
				{
					this.HandleException(explanation + " - Match failed");
				}
			}
			finally
			{
				this.Pos = pos;
			}
		}

		// Token: 0x06000757 RID: 1879 RVA: 0x00017DA8 File Offset: 0x00015FA8
		public CodeMatcher ThrowIfNotMatchForward(string explanation, params CodeMatch[] matches)
		{
			this.ThrowIfNotMatch(explanation, 1, matches);
			return this;
		}

		// Token: 0x06000758 RID: 1880 RVA: 0x00017DB4 File Offset: 0x00015FB4
		public CodeMatcher ThrowIfNotMatchBack(string explanation, params CodeMatch[] matches)
		{
			this.ThrowIfNotMatch(explanation, -1, matches);
			return this;
		}

		// Token: 0x06000759 RID: 1881 RVA: 0x00017DC0 File Offset: 0x00015FC0
		public CodeMatcher ThrowIfFalse(string explanation, Func<CodeMatcher, bool> stateCheckFunc)
		{
			if (stateCheckFunc == null)
			{
				throw new ArgumentNullException("stateCheckFunc");
			}
			this.ThrowIfInvalid(explanation);
			if (!stateCheckFunc(this))
			{
				return this.HandleException<CodeMatcher>(explanation + " - Check function returned false", this);
			}
			return this;
		}

		// Token: 0x0600075A RID: 1882 RVA: 0x00017DF5 File Offset: 0x00015FF5
		public CodeMatcher Do(Action<CodeMatcher> action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			action(this);
			return this;
		}

		// Token: 0x0600075B RID: 1883 RVA: 0x00017E0D File Offset: 0x0001600D
		public CodeMatcher OnError(CodeMatcher.ErrorHandler errorHandler)
		{
			this.errorHandler = errorHandler;
			return this;
		}

		// Token: 0x0600075C RID: 1884 RVA: 0x00017E17 File Offset: 0x00016017
		public CodeMatcher SetInstruction(CodeInstruction instruction)
		{
			if (this.IsInvalid)
			{
				return this.HandleException<CodeMatcher>("Cannot set instruction/opcode at invalid position.", this);
			}
			this.codes[this.Pos] = instruction;
			return this;
		}

		// Token: 0x0600075D RID: 1885 RVA: 0x00017E44 File Offset: 0x00016044
		public CodeMatcher SetInstructionAndAdvance(CodeInstruction instruction)
		{
			this.SetInstruction(instruction);
			int pos = this.Pos;
			this.Pos = pos + 1;
			return this;
		}

		// Token: 0x0600075E RID: 1886 RVA: 0x00017E6A File Offset: 0x0001606A
		public unsafe CodeMatcher Set(OpCode opcode, object operand)
		{
			if (this.IsInvalid)
			{
				return this.HandleException<CodeMatcher>("Cannot set values at invalid position.", this);
			}
			*this.Opcode = opcode;
			*this.Operand = operand;
			return this;
		}

		// Token: 0x0600075F RID: 1887 RVA: 0x00017E98 File Offset: 0x00016098
		public CodeMatcher SetAndAdvance(OpCode opcode, object operand)
		{
			this.Set(opcode, operand);
			int pos = this.Pos;
			this.Pos = pos + 1;
			return this;
		}

		// Token: 0x06000760 RID: 1888 RVA: 0x00017EC0 File Offset: 0x000160C0
		public unsafe CodeMatcher SetOpcodeAndAdvance(OpCode opcode)
		{
			if (this.IsInvalid)
			{
				return this.HandleException<CodeMatcher>("Cannot set opcode at invalid position.", this);
			}
			*this.Opcode = opcode;
			int pos = this.Pos;
			this.Pos = pos + 1;
			return this;
		}

		// Token: 0x06000761 RID: 1889 RVA: 0x00017F00 File Offset: 0x00016100
		public unsafe CodeMatcher SetOperandAndAdvance(object operand)
		{
			if (this.IsInvalid)
			{
				return this.HandleException<CodeMatcher>("Cannot set operand at invalid position.", this);
			}
			*this.Operand = operand;
			int pos = this.Pos;
			this.Pos = pos + 1;
			return this;
		}

		// Token: 0x06000762 RID: 1890 RVA: 0x00017F3B File Offset: 0x0001613B
		public CodeMatcher DeclareLocal(Type variableType, out LocalBuilder localVariable)
		{
			if (this.generator == null)
			{
				localVariable = null;
				return this.HandleException<CodeMatcher>("Generator must be provided to use this method", this);
			}
			localVariable = this.generator.DeclareLocal(variableType);
			return this;
		}

		// Token: 0x06000763 RID: 1891 RVA: 0x00017F64 File Offset: 0x00016164
		public CodeMatcher DefineLabel(out Label label)
		{
			if (this.generator == null)
			{
				label = default(Label);
				return this.HandleException<CodeMatcher>("Generator must be provided to use this method", this);
			}
			label = this.generator.DefineLabel();
			return this;
		}

		// Token: 0x06000764 RID: 1892 RVA: 0x00017F94 File Offset: 0x00016194
		public unsafe CodeMatcher CreateLabel(out Label label)
		{
			if (this.generator == null)
			{
				label = default(Label);
				return this.HandleException<CodeMatcher>("Generator must be provided to use this method", this);
			}
			label = this.generator.DefineLabel();
			this.Labels->Add(label);
			return this;
		}

		// Token: 0x06000765 RID: 1893 RVA: 0x00017FE4 File Offset: 0x000161E4
		public CodeMatcher CreateLabelAt(int position, out Label label)
		{
			if (this.generator == null)
			{
				label = default(Label);
				return this.HandleException<CodeMatcher>("Generator must be provided to use this method", this);
			}
			label = this.generator.DefineLabel();
			this.AddLabelsAt(position, new <>z__ReadOnlySingleElementList<Label>(label));
			return this;
		}

		// Token: 0x06000766 RID: 1894 RVA: 0x00018034 File Offset: 0x00016234
		public CodeMatcher CreateLabelWithOffsets(int offset, out Label label)
		{
			if (this.generator == null)
			{
				label = default(Label);
				return this.HandleException<CodeMatcher>("Generator must be provided to use this method", this);
			}
			label = this.generator.DefineLabel();
			return this.AddLabelsAt(this.Pos + offset, new <>z__ReadOnlySingleElementList<Label>(label));
		}

		// Token: 0x06000767 RID: 1895 RVA: 0x00018087 File Offset: 0x00016287
		public unsafe CodeMatcher AddLabels(IEnumerable<Label> labels)
		{
			this.Labels->AddRange(labels);
			return this;
		}

		// Token: 0x06000768 RID: 1896 RVA: 0x00018097 File Offset: 0x00016297
		public CodeMatcher AddLabelsAt(int position, IEnumerable<Label> labels)
		{
			if (position < 0 || position >= this.Length)
			{
				return this.HandleException<CodeMatcher>("Cannot add labels at invalid position.", this);
			}
			this.codes[position].labels.AddRange(labels);
			return this;
		}

		// Token: 0x06000769 RID: 1897 RVA: 0x000180CB File Offset: 0x000162CB
		public CodeMatcher SetJumpTo(OpCode opcode, int destination, out Label label)
		{
			this.CreateLabelAt(destination, out label);
			return this.Set(opcode, label);
		}

		// Token: 0x0600076A RID: 1898 RVA: 0x000180E8 File Offset: 0x000162E8
		public CodeMatcher Insert(params CodeInstruction[] instructions)
		{
			if (instructions != null)
			{
				if (!instructions.Any<CodeInstruction>((CodeInstruction i) => i == null))
				{
					if (this.IsInvalid)
					{
						return this.HandleException<CodeMatcher>("Cannot insert instructions at invalid position.", this);
					}
					this.codes.InsertRange(this.Pos, instructions);
					return this;
				}
			}
			throw new ArgumentNullException("instructions");
		}

		// Token: 0x0600076B RID: 1899 RVA: 0x00018154 File Offset: 0x00016354
		public CodeMatcher Insert(IEnumerable<CodeInstruction> instructions)
		{
			if (instructions != null)
			{
				if (!instructions.Any<CodeInstruction>((CodeInstruction i) => i == null))
				{
					if (this.IsInvalid)
					{
						return this.HandleException<CodeMatcher>("Cannot insert instructions at invalid position.", this);
					}
					this.codes.InsertRange(this.Pos, instructions);
					return this;
				}
			}
			throw new ArgumentNullException("instructions");
		}

		// Token: 0x0600076C RID: 1900 RVA: 0x000181C0 File Offset: 0x000163C0
		public CodeMatcher InsertBranch(OpCode opcode, int destination)
		{
			if (this.IsInvalid)
			{
				return this.HandleException<CodeMatcher>("Cannot insert instructions at invalid position.", this);
			}
			Label label;
			this.CreateLabelAt(destination, out label);
			this.codes.Insert(this.Pos, new CodeInstruction(opcode, label));
			return this;
		}

		// Token: 0x0600076D RID: 1901 RVA: 0x0001820C File Offset: 0x0001640C
		public CodeMatcher InsertAndAdvance(params CodeInstruction[] instructions)
		{
			if (instructions != null)
			{
				if (!instructions.Any<CodeInstruction>((CodeInstruction i) => i == null))
				{
					foreach (CodeInstruction codeInstruction in instructions)
					{
						this.Insert(new CodeInstruction[] { codeInstruction });
						int pos = this.Pos;
						this.Pos = pos + 1;
					}
					return this;
				}
			}
			throw new ArgumentNullException("instructions");
		}

		// Token: 0x0600076E RID: 1902 RVA: 0x00018284 File Offset: 0x00016484
		public CodeMatcher InsertAndAdvance(IEnumerable<CodeInstruction> instructions)
		{
			if (instructions != null)
			{
				if (!instructions.Any<CodeInstruction>((CodeInstruction i) => i == null))
				{
					foreach (CodeInstruction codeInstruction in instructions)
					{
						this.InsertAndAdvance(new CodeInstruction[] { codeInstruction });
					}
					return this;
				}
			}
			throw new ArgumentNullException("instructions");
		}

		// Token: 0x0600076F RID: 1903 RVA: 0x0001830C File Offset: 0x0001650C
		public CodeMatcher InsertBranchAndAdvance(OpCode opcode, int destination)
		{
			this.InsertBranch(opcode, destination);
			int pos = this.Pos;
			this.Pos = pos + 1;
			return this;
		}

		// Token: 0x06000770 RID: 1904 RVA: 0x00018334 File Offset: 0x00016534
		public CodeMatcher InsertAfter(params CodeInstruction[] instructions)
		{
			if (instructions != null)
			{
				if (!instructions.Any<CodeInstruction>((CodeInstruction i) => i == null))
				{
					if (this.IsInvalid)
					{
						return this.HandleException<CodeMatcher>("Cannot insert instructions at invalid position.", this);
					}
					this.codes.InsertRange(this.Pos + 1, instructions);
					return this;
				}
			}
			throw new ArgumentNullException("instructions");
		}

		// Token: 0x06000771 RID: 1905 RVA: 0x000183A0 File Offset: 0x000165A0
		public CodeMatcher InsertAfter(IEnumerable<CodeInstruction> instructions)
		{
			if (instructions != null)
			{
				if (!instructions.Any<CodeInstruction>((CodeInstruction i) => i == null))
				{
					if (this.IsInvalid)
					{
						return this.HandleException<CodeMatcher>("Cannot insert instructions at invalid position.", this);
					}
					this.codes.InsertRange(this.Pos + 1, instructions);
					return this;
				}
			}
			return this.HandleException<CodeMatcher>("Cannot insert null instructions.", this);
		}

		// Token: 0x06000772 RID: 1906 RVA: 0x00018410 File Offset: 0x00016610
		public CodeMatcher InsertBranchAfter(OpCode opcode, int destination)
		{
			if (this.IsInvalid)
			{
				return this.HandleException<CodeMatcher>("Cannot insert instructions at invalid position.", this);
			}
			Label label;
			this.CreateLabelAt(destination, out label);
			this.codes.Insert(this.Pos + 1, new CodeInstruction(opcode, label));
			return this;
		}

		// Token: 0x06000773 RID: 1907 RVA: 0x0001845C File Offset: 0x0001665C
		public CodeMatcher InsertAfterAndAdvance(params CodeInstruction[] instructions)
		{
			this.InsertAfter(instructions);
			this.Pos += instructions.Length;
			return this;
		}

		// Token: 0x06000774 RID: 1908 RVA: 0x00018478 File Offset: 0x00016678
		public CodeMatcher InsertAfterAndAdvance(IEnumerable<CodeInstruction> instructions)
		{
			if (instructions != null)
			{
				if (!instructions.Any<CodeInstruction>((CodeInstruction i) => i == null))
				{
					List<CodeInstruction> list = instructions.ToList<CodeInstruction>();
					this.InsertAfter(list);
					this.Pos += list.Count;
					return this;
				}
			}
			return this.HandleException<CodeMatcher>("Cannot insert null instructions.", this);
		}

		// Token: 0x06000775 RID: 1909 RVA: 0x000184E0 File Offset: 0x000166E0
		public CodeMatcher InsertBranchAfterAndAdvance(OpCode opcode, int destination)
		{
			this.InsertBranchAfter(opcode, destination);
			int pos = this.Pos;
			this.Pos = pos + 1;
			return this;
		}

		// Token: 0x06000776 RID: 1910 RVA: 0x00018507 File Offset: 0x00016707
		public CodeMatcher RemoveInstruction()
		{
			if (this.IsInvalid)
			{
				return this.HandleException<CodeMatcher>("Cannot remove instructions from an invalid position.", this);
			}
			this.codes.RemoveAt(this.Pos);
			return this;
		}

		// Token: 0x06000777 RID: 1911 RVA: 0x00018530 File Offset: 0x00016730
		public CodeMatcher RemoveInstructions(int count)
		{
			if (this.IsInvalid || this.Pos + count > this.Length)
			{
				return this.HandleException<CodeMatcher>("Cannot remove instructions from an invalid or out-of-range position.", this);
			}
			this.codes.RemoveRange(this.Pos, count);
			return this;
		}

		// Token: 0x06000778 RID: 1912 RVA: 0x0001856A File Offset: 0x0001676A
		public CodeMatcher RemoveInstructionsInRange(int start, int end)
		{
			if (start > end)
			{
				int num = start;
				start = end;
				end = num;
			}
			if (start < 0 || end >= this.Length)
			{
				return this.HandleException<CodeMatcher>("Cannot remove instructions: range is out-of-bounds.", this);
			}
			this.codes.RemoveRange(start, end - start + 1);
			return this;
		}

		// Token: 0x06000779 RID: 1913 RVA: 0x000185A2 File Offset: 0x000167A2
		public CodeMatcher RemoveInstructionsWithOffsets(int startOffset, int endOffset)
		{
			return this.RemoveInstructionsInRange(this.Pos + startOffset, this.Pos + endOffset);
		}

		// Token: 0x0600077A RID: 1914 RVA: 0x000185BA File Offset: 0x000167BA
		public CodeMatcher Advance(int offset = 1)
		{
			this.Pos += offset;
			if (!this.IsValid)
			{
				this.SetOutOfBounds(offset);
			}
			return this;
		}

		// Token: 0x0600077B RID: 1915 RVA: 0x000185DA File Offset: 0x000167DA
		public CodeMatcher Start()
		{
			this.Pos = 0;
			return this;
		}

		// Token: 0x0600077C RID: 1916 RVA: 0x000185E4 File Offset: 0x000167E4
		public CodeMatcher End()
		{
			this.Pos = this.Length - 1;
			return this;
		}

		// Token: 0x0600077D RID: 1917 RVA: 0x000185F5 File Offset: 0x000167F5
		public CodeMatcher SearchForward(Func<CodeInstruction, bool> predicate)
		{
			return this.Search(predicate, 1);
		}

		// Token: 0x0600077E RID: 1918 RVA: 0x000185FF File Offset: 0x000167FF
		public CodeMatcher SearchBackwards(Func<CodeInstruction, bool> predicate)
		{
			return this.Search(predicate, -1);
		}

		// Token: 0x0600077F RID: 1919 RVA: 0x0001860C File Offset: 0x0001680C
		private CodeMatcher Search(Func<CodeInstruction, bool> predicate, int direction)
		{
			this.FixStart();
			while (this.IsValid && !predicate(this.Instruction))
			{
				this.Pos += direction;
			}
			string text;
			if (!this.IsInvalid)
			{
				text = null;
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Cannot find ");
				defaultInterpolatedStringHandler.AppendFormatted<Func<CodeInstruction, bool>>(predicate);
				text = defaultInterpolatedStringHandler.ToStringAndClear();
			}
			this.lastError = text;
			return this;
		}

		// Token: 0x06000780 RID: 1920 RVA: 0x0001867C File Offset: 0x0001687C
		public CodeMatcher MatchStartForward(params CodeMatch[] matches)
		{
			return this.Match(matches, 1, CodeMatcher.MatchPosition.Start, false);
		}

		// Token: 0x06000781 RID: 1921 RVA: 0x00018688 File Offset: 0x00016888
		public CodeMatcher PrepareMatchStartForward(params CodeMatch[] matches)
		{
			return this.Match(matches, 1, CodeMatcher.MatchPosition.Start, true);
		}

		// Token: 0x06000782 RID: 1922 RVA: 0x00018694 File Offset: 0x00016894
		public CodeMatcher MatchEndForward(params CodeMatch[] matches)
		{
			return this.Match(matches, 1, CodeMatcher.MatchPosition.End, false);
		}

		// Token: 0x06000783 RID: 1923 RVA: 0x000186A0 File Offset: 0x000168A0
		public CodeMatcher PrepareMatchEndForward(params CodeMatch[] matches)
		{
			return this.Match(matches, 1, CodeMatcher.MatchPosition.End, true);
		}

		// Token: 0x06000784 RID: 1924 RVA: 0x000186AC File Offset: 0x000168AC
		public CodeMatcher MatchStartBackwards(params CodeMatch[] matches)
		{
			return this.Match(matches, -1, CodeMatcher.MatchPosition.Start, false);
		}

		// Token: 0x06000785 RID: 1925 RVA: 0x000186B8 File Offset: 0x000168B8
		public CodeMatcher PrepareMatchStartBackwards(params CodeMatch[] matches)
		{
			return this.Match(matches, -1, CodeMatcher.MatchPosition.Start, true);
		}

		// Token: 0x06000786 RID: 1926 RVA: 0x000186C4 File Offset: 0x000168C4
		public CodeMatcher MatchEndBackwards(params CodeMatch[] matches)
		{
			return this.Match(matches, -1, CodeMatcher.MatchPosition.End, false);
		}

		// Token: 0x06000787 RID: 1927 RVA: 0x000186D0 File Offset: 0x000168D0
		public CodeMatcher PrepareMatchEndBackwards(params CodeMatch[] matches)
		{
			return this.Match(matches, -1, CodeMatcher.MatchPosition.End, true);
		}

		// Token: 0x06000788 RID: 1928 RVA: 0x000186DC File Offset: 0x000168DC
		public CodeMatcher RemoveSearchForward(Func<CodeInstruction, bool> predicate)
		{
			if (this.IsInvalid)
			{
				return this.HandleException<CodeMatcher>("Cannot remove instructions from an invalid position.", this);
			}
			int pos = this.Pos;
			CodeMatcher codeMatcher = this.Clone().SearchForward(predicate);
			if (codeMatcher.IsInvalid)
			{
				this.lastError = codeMatcher.lastError;
				this.SetOutOfBounds(1);
				return this;
			}
			int num = codeMatcher.Pos - 1;
			if (num >= pos)
			{
				this.RemoveInstructionsInRange(pos, num);
			}
			return this;
		}

		// Token: 0x06000789 RID: 1929 RVA: 0x00018748 File Offset: 0x00016948
		public CodeMatcher RemoveSearchBackward(Func<CodeInstruction, bool> predicate)
		{
			if (this.IsInvalid)
			{
				return this.HandleException<CodeMatcher>("Cannot remove instructions from an invalid position.", this);
			}
			int pos = this.Pos;
			CodeMatcher codeMatcher = this.Clone().SearchBackwards(predicate);
			if (codeMatcher.IsInvalid)
			{
				this.lastError = codeMatcher.lastError;
				this.SetOutOfBounds(-1);
				return this;
			}
			int pos2 = codeMatcher.Pos;
			int num = pos2 + 1;
			if (pos >= num)
			{
				this.RemoveInstructionsInRange(num, pos);
			}
			this.Pos = pos2;
			return this;
		}

		// Token: 0x0600078A RID: 1930 RVA: 0x000187BC File Offset: 0x000169BC
		public CodeMatcher RemoveUntilForward(params CodeMatch[] matches)
		{
			if (this.IsInvalid)
			{
				return this.HandleException<CodeMatcher>("Cannot remove instructions from an invalid position.", this);
			}
			int pos = this.Pos;
			CodeMatcher codeMatcher = this.Clone().MatchStartForward(matches);
			if (codeMatcher.IsInvalid)
			{
				this.lastError = codeMatcher.lastError;
				this.SetOutOfBounds(1);
				return this;
			}
			int num = codeMatcher.Pos - 1;
			if (num >= pos)
			{
				this.RemoveInstructionsInRange(pos, num);
			}
			return this;
		}

		// Token: 0x0600078B RID: 1931 RVA: 0x00018828 File Offset: 0x00016A28
		public CodeMatcher RemoveUntilBackward(params CodeMatch[] matches)
		{
			if (this.IsInvalid)
			{
				return this.HandleException<CodeMatcher>("Cannot remove instructions from an invalid position.", this);
			}
			int pos = this.Pos;
			CodeMatcher codeMatcher = this.Clone().MatchEndBackwards(matches);
			if (codeMatcher.IsInvalid)
			{
				this.lastError = codeMatcher.lastError;
				this.SetOutOfBounds(-1);
				return this;
			}
			int pos2 = codeMatcher.Pos;
			if (pos > pos2)
			{
				this.RemoveInstructionsInRange(pos2 + 1, pos);
			}
			this.Pos = pos2;
			return this;
		}

		// Token: 0x0600078C RID: 1932 RVA: 0x0001889C File Offset: 0x00016A9C
		private CodeMatcher Match(CodeMatch[] matches, int direction, CodeMatcher.MatchPosition mode, bool prepareOnly)
		{
			this.lastMatchCall = delegate
			{
				while (this.IsValid)
				{
					if (this.MatchSequence(this.Pos, matches))
					{
						if (mode == CodeMatcher.MatchPosition.End)
						{
							this.Pos += matches.Length - 1;
							break;
						}
						break;
					}
					else
					{
						this.Pos += direction;
					}
				}
				this.lastError = (this.IsInvalid ? ("Cannot find " + matches.Join<CodeMatch>(null, ", ")) : null);
				return this;
			};
			if (prepareOnly)
			{
				return this;
			}
			this.FixStart();
			return this.lastMatchCall();
		}

		// Token: 0x0600078D RID: 1933 RVA: 0x000188F4 File Offset: 0x00016AF4
		public CodeMatcher Repeat(Action<CodeMatcher> matchAction, Action<string> notFoundAction = null)
		{
			int num = 0;
			if (this.lastMatchCall == null)
			{
				return this.HandleException<CodeMatcher>("No previous Match operation - cannot repeat", this);
			}
			while (this.IsValid)
			{
				matchAction(this);
				this.lastMatchCall();
				num++;
			}
			this.lastMatchCall = null;
			if (num == 0 && notFoundAction != null)
			{
				notFoundAction(this.lastError);
			}
			return this;
		}

		// Token: 0x0600078E RID: 1934 RVA: 0x00018951 File Offset: 0x00016B51
		public CodeInstruction NamedMatch(string name)
		{
			return this.lastMatches[name];
		}

		// Token: 0x0600078F RID: 1935 RVA: 0x00018960 File Offset: 0x00016B60
		private bool MatchSequence(int start, CodeMatch[] matches)
		{
			if (start < 0)
			{
				return false;
			}
			this.lastMatches = new Dictionary<string, CodeInstruction>();
			foreach (CodeMatch codeMatch in matches)
			{
				if (start >= this.Length || !codeMatch.Matches(this.codes, this.codes[start]))
				{
					return false;
				}
				if (codeMatch.name != null)
				{
					this.lastMatches.Add(codeMatch.name, this.codes[start]);
				}
				start++;
			}
			return true;
		}

		// Token: 0x0400028A RID: 650
		private readonly ILGenerator generator;

		// Token: 0x0400028B RID: 651
		private readonly List<CodeInstruction> codes = new List<CodeInstruction>();

		// Token: 0x0400028D RID: 653
		private Dictionary<string, CodeInstruction> lastMatches = new Dictionary<string, CodeInstruction>();

		// Token: 0x0400028E RID: 654
		private string lastError;

		// Token: 0x0400028F RID: 655
		private CodeMatcher.MatchDelegate lastMatchCall;

		// Token: 0x04000290 RID: 656
		private CodeMatcher.ErrorHandler errorHandler;

		// Token: 0x020001B8 RID: 440
		// (Invoke) Token: 0x06000791 RID: 1937
		public delegate bool ErrorHandler(CodeMatcher matcher, string error);

		// Token: 0x020001B9 RID: 441
		private enum MatchPosition
		{
			// Token: 0x04000292 RID: 658
			Start,
			// Token: 0x04000293 RID: 659
			End
		}

		// Token: 0x020001BA RID: 442
		// (Invoke) Token: 0x06000795 RID: 1941
		private delegate CodeMatcher MatchDelegate();
	}
}
