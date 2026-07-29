using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FormationFilter.Utilities;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace FormationFilter.Models
{
	// Token: 0x0200001C RID: 28
	[NullableContext(1)]
	[Nullable(0)]
	public class TroopFilter
	{
		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000F9 RID: 249 RVA: 0x00008860 File Offset: 0x00006A60
		// (set) Token: 0x060000FA RID: 250 RVA: 0x00008868 File Offset: 0x00006A68
		public ulong Bitmask { get; private set; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000FB RID: 251 RVA: 0x00008871 File Offset: 0x00006A71
		// (set) Token: 0x060000FC RID: 252 RVA: 0x00008879 File Offset: 0x00006A79
		public float Weight { get; private set; } = 1f;

		// Token: 0x060000FD RID: 253 RVA: 0x00008882 File Offset: 0x00006A82
		public TroopFilter()
		{
			this.Bitmask = this.GetFilterBitmask();
			this.Weight = 1f;
		}

		// Token: 0x060000FE RID: 254 RVA: 0x000088B7 File Offset: 0x00006AB7
		public TroopFilter(Dictionary<FilterTypeEnum, FilterValueEnum> filterDict, float weight)
		{
			this._filterDict = filterDict;
			this.Bitmask = this.GetFilterBitmask();
			this.Weight = weight;
		}

		// Token: 0x060000FF RID: 255 RVA: 0x000088F0 File Offset: 0x00006AF0
		public TroopFilter(ulong bitmask, float weight)
		{
			for (FilterTypeEnum filterTypeEnum = FilterTypeEnum.HasMount; filterTypeEnum < FilterTypeEnum.Count; filterTypeEnum++)
			{
				int num = (int)(filterTypeEnum * FilterTypeEnum.HasTwoHanded);
				ulong num2 = (bitmask >> num) & 3UL;
				if (num2 == 0UL)
				{
					Utility.DisplayInvalidFilterBitmask(bitmask);
				}
				else if (num2 != 3UL)
				{
					this._filterDict[filterTypeEnum] = (FilterValueEnum)num2;
				}
			}
			this.Bitmask = this.GetFilterBitmask();
			this.Weight = weight;
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00008965 File Offset: 0x00006B65
		public void ClearAllFilters()
		{
			this._filterDict.Clear();
			this.Bitmask = this.GetFilterBitmask();
		}

		// Token: 0x06000101 RID: 257 RVA: 0x0000897E File Offset: 0x00006B7E
		public void SetFilter(FilterTypeEnum filterType, FilterValueEnum filterValue)
		{
			this._filterDict[filterType] = filterValue;
			this.Bitmask = this.GetFilterBitmask();
		}

		// Token: 0x06000102 RID: 258 RVA: 0x0000899C File Offset: 0x00006B9C
		public void SetFilters([Nullable(new byte[] { 1, 0 })] List<ValueTuple<FilterTypeEnum, FilterValueEnum>> filters)
		{
			foreach (ValueTuple<FilterTypeEnum, FilterValueEnum> valueTuple in filters)
			{
				FilterTypeEnum item = valueTuple.Item1;
				FilterValueEnum item2 = valueTuple.Item2;
				this._filterDict[item] = item2;
			}
			this.Bitmask = this.GetFilterBitmask();
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00008A08 File Offset: 0x00006C08
		public FilterValueEnum GetFilter(FilterTypeEnum filterType)
		{
			if (this._filterDict.ContainsKey(filterType))
			{
				return this._filterDict[filterType];
			}
			return FilterValueEnum.Any;
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00008A28 File Offset: 0x00006C28
		public Dictionary<FilterTypeEnum, FilterValueEnum> GetAllFilters(bool excludeBasicFilter = false)
		{
			Dictionary<FilterTypeEnum, FilterValueEnum> dictionary = new Dictionary<FilterTypeEnum, FilterValueEnum>();
			foreach (KeyValuePair<FilterTypeEnum, FilterValueEnum> keyValuePair in this._filterDict)
			{
				if (keyValuePair.Value != FilterValueEnum.Any && keyValuePair.Value != FilterValueEnum.Invalid && (!excludeBasicFilter || !keyValuePair.Key.IsBasicFilter()))
				{
					dictionary.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			return dictionary;
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00008AB4 File Offset: 0x00006CB4
		private ulong GetFilterBitmask()
		{
			ulong num = 0UL;
			for (FilterTypeEnum filterTypeEnum = FilterTypeEnum.HasMount; filterTypeEnum < FilterTypeEnum.Count; filterTypeEnum++)
			{
				FilterValueEnum filter = this.GetFilter(filterTypeEnum);
				if (filter == FilterValueEnum.Invalid)
				{
					return 0UL;
				}
				int num2 = (int)(filterTypeEnum * FilterTypeEnum.HasTwoHanded);
				num |= (ulong)((ulong)((long)filter) << num2);
			}
			return num;
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00008AEE File Offset: 0x00006CEE
		public bool Matches(ulong bitmask)
		{
			return TroopFilter.HasIntersection(this.Bitmask, bitmask);
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00008AFC File Offset: 0x00006CFC
		public ulong IntersectWith(ulong bitmask)
		{
			return TroopFilter.Intersects(this.Bitmask, bitmask);
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00008B0A File Offset: 0x00006D0A
		public void SetWeight(float weight)
		{
			this.Weight = weight;
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00008B13 File Offset: 0x00006D13
		public static FilterValueEnum GetFilterEnum(Agent agent, FilterTypeEnum filterTypeEnum)
		{
			if (!Utility.HasFilterType(agent, filterTypeEnum))
			{
				return FilterValueEnum.No;
			}
			return FilterValueEnum.Yes;
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00008B21 File Offset: 0x00006D21
		public static FilterValueEnum GetFilterEnum(IAgentOriginBase agentOriginBase, FilterTypeEnum filterTypeEnum)
		{
			if (!Utility.HasFilterType(agentOriginBase, filterTypeEnum))
			{
				return FilterValueEnum.No;
			}
			return FilterValueEnum.Yes;
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00008B30 File Offset: 0x00006D30
		public static ulong GetAgentBitMask(Agent agent)
		{
			ulong num = 0UL;
			for (FilterTypeEnum filterTypeEnum = FilterTypeEnum.HasMount; filterTypeEnum < FilterTypeEnum.Count; filterTypeEnum++)
			{
				int num2 = (int)(filterTypeEnum * FilterTypeEnum.HasTwoHanded);
				FilterValueEnum filterEnum = TroopFilter.GetFilterEnum(agent, filterTypeEnum);
				num |= (ulong)((ulong)((long)filterEnum) << num2);
			}
			return num;
		}

		// Token: 0x0600010C RID: 268 RVA: 0x00008B64 File Offset: 0x00006D64
		public static ulong GetIAgentOriginBaseBitMask(IAgentOriginBase agentOriginBase)
		{
			ulong num = 0UL;
			for (FilterTypeEnum filterTypeEnum = FilterTypeEnum.HasMount; filterTypeEnum < FilterTypeEnum.Count; filterTypeEnum++)
			{
				int num2 = (int)(filterTypeEnum * FilterTypeEnum.HasTwoHanded);
				FilterValueEnum filterEnum = TroopFilter.GetFilterEnum(agentOriginBase, filterTypeEnum);
				num |= (ulong)((ulong)((long)filterEnum) << num2);
			}
			return num;
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00008B98 File Offset: 0x00006D98
		public static bool HasIntersection(ulong bitmask1, ulong bitmask2)
		{
			return !TroopFilter.IsEmpty(bitmask1 & bitmask2);
		}

		// Token: 0x0600010E RID: 270 RVA: 0x00008BA8 File Offset: 0x00006DA8
		public static ulong Intersects(ulong bitmask1, ulong bitmask2)
		{
			ulong num = bitmask1 & bitmask2;
			if (TroopFilter.IsEmpty(num))
			{
				return 0UL;
			}
			return num;
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00008BC8 File Offset: 0x00006DC8
		public static bool IsEmpty(ulong bitmask)
		{
			for (FilterTypeEnum filterTypeEnum = FilterTypeEnum.HasMount; filterTypeEnum < FilterTypeEnum.Count; filterTypeEnum++)
			{
				int num = (int)(filterTypeEnum * FilterTypeEnum.HasTwoHanded);
				if ((bitmask & (3UL << num)) == 0UL)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x04000082 RID: 130
		private Dictionary<FilterTypeEnum, FilterValueEnum> _filterDict = new Dictionary<FilterTypeEnum, FilterValueEnum>();
	}
}
