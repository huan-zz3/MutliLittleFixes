using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.Library;

namespace NavalDLC.ViewModelCollection.Map.MapBar
{
	// Token: 0x02000030 RID: 48
	public class ShipHealthPercentageComparer : IComparer<Ship>
	{
		// Token: 0x060003F3 RID: 1011 RVA: 0x00013264 File Offset: 0x00011464
		public int Compare(Ship x, Ship y)
		{
			int num = MathF.Ceiling(y.GetHealthPercent()).CompareTo(MathF.Ceiling(x.GetHealthPercent()));
			if (num != 0)
			{
				return num;
			}
			return this.ResolveEquality(x, y);
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x0001329D File Offset: 0x0001149D
		private int ResolveEquality(Ship x, Ship y)
		{
			return x.Name.ToString().CompareTo(y.Name.ToString());
		}
	}
}
