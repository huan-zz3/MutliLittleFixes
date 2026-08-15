using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;

namespace AutoResolveRebalanced
{
	// Token: 0x0200000E RID: 14
	public class SimulateData
	{
		// Token: 0x06000082 RID: 130 RVA: 0x00002F6E File Offset: 0x0000116E
		public SimulateData(MapEventSide side, List<UniqueTroopDescriptor> troopList)
		{
			this.UpdateDict(side, troopList);
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00002F9C File Offset: 0x0000119C
		public void UpdateDict(MapEventSide side, List<UniqueTroopDescriptor> troopList)
		{
			this._side = side;
			this._troopList = troopList;
			float num = 1f;
			if (this._hitPointAverage > 0f)
			{
				float num2 = 0f;
				int num3 = 0;
				foreach (UniqueTroopDescriptor uniqueTroopDescriptor in troopList)
				{
					if (!side.GetAllocatedTroop(uniqueTroopDescriptor).IsHero)
					{
						num2 += (float)side.GetAllocatedTroop(uniqueTroopDescriptor).HitPoints;
						num3++;
					}
				}
				if (num3 > 0)
				{
					num2 = (float)Math.Ceiling((double)(num2 / (float)num3));
				}
				if (this._troopNumber > 0 && troopList.Count > this._troopNumber)
				{
					this._hitPointAverage = (num2 * (float)(troopList.Count - this._troopNumber) + this._hitPointAverage * (float)this._troopNumber) / (float)troopList.Count;
					Debugger.Message("Troop Count Changed newAvg: " + this._hitPointAverage.ToString("0"), Debugger.Type.Log, this._side.MapEvent, false);
				}
				if (num2 > this._hitPointAverage)
				{
					num = this._hitPointAverage / num2;
				}
				Debugger.Message(string.Concat(new string[]
				{
					"Party: ",
					side.LeaderParty.ToString(),
					" remainHP: ",
					num.ToString("0.0"),
					" currHP: ",
					num2.ToString("0"),
					" storedHP: ",
					this._hitPointAverage.ToString("0"),
					" numTrp: ",
					side.NumRemainingSimulationTroops.ToString()
				}), Debugger.Type.Log, this._side.MapEvent, false);
				this._hitPointAverage = -1f;
				this._troopNumber = -1;
			}
			foreach (UniqueTroopDescriptor uniqueTroopDescriptor2 in troopList)
			{
				if (!side.GetAllocatedTroop(uniqueTroopDescriptor2).IsHero)
				{
					int num4 = (int)((float)side.GetAllocatedTroop(uniqueTroopDescriptor2).HitPoints * num);
					if (!this._hitPointDict.TryAdd(uniqueTroopDescriptor2, num4))
					{
						Debugger.Message("Duplicated Key Ignored in Dict at UpdateDict", Debugger.Type.Error, this._side.MapEvent, false);
					}
				}
			}
		}

		// Token: 0x06000084 RID: 132 RVA: 0x000031F4 File Offset: 0x000013F4
		public bool GetHitPoint(UniqueTroopDescriptor desc, out int hp)
		{
			bool flag = this._hitPointDict.TryGetValue(desc, out hp);
			if (!flag)
			{
				hp = MBRandom.RandomInt(SimulateData._defaultHitPoints);
			}
			return flag;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00003214 File Offset: 0x00001414
		public void StoreHitPointAverage()
		{
			int num = 0;
			this._hitPointAverage = 0f;
			foreach (int num2 in this._hitPointDict.Values)
			{
				if (num2 > 0)
				{
					num++;
					this._hitPointAverage += (float)num2;
				}
			}
			if (num > 0)
			{
				this._hitPointAverage = (float)Math.Ceiling((double)(this._hitPointAverage / (float)num));
			}
			if (this._hitPointAverage <= 0f && num > 0)
			{
				Debugger.Message("hitPointAverage below zero at StoreHitPointAverage", Debugger.Type.Error, this._side.MapEvent, false);
			}
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000032C8 File Offset: 0x000014C8
		public void StoreTroopNumber(int n)
		{
			this._troopNumber = n;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x000032D1 File Offset: 0x000014D1
		public void SetHitPoint(UniqueTroopDescriptor desc, int hp)
		{
			this._hitPointDict[desc] = hp;
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000032E0 File Offset: 0x000014E0
		public void Clear(bool clearAvg = false)
		{
			this._hitPointDict.Clear();
			if (clearAvg)
			{
				this._hitPointAverage = -1f;
				this._troopNumber = -1;
			}
		}

		// Token: 0x0400002E RID: 46
		private List<UniqueTroopDescriptor> _troopList;

		// Token: 0x0400002F RID: 47
		private MapEventSide _side;

		// Token: 0x04000030 RID: 48
		private ConcurrentDictionary<UniqueTroopDescriptor, int> _hitPointDict = new ConcurrentDictionary<UniqueTroopDescriptor, int>();

		// Token: 0x04000031 RID: 49
		private float _hitPointAverage = -1f;

		// Token: 0x04000032 RID: 50
		private int _troopNumber = -1;

		// Token: 0x04000033 RID: 51
		private static int _defaultHitPoints = 100;
	}
}
