using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.Objects.UsableMachines
{
	// Token: 0x020000B6 RID: 182
	public class ShipFireBallista : ShipBallista
	{
		// Token: 0x06000E02 RID: 3586 RVA: 0x0006DC52 File Offset: 0x0006BE52
		public override SiegeEngineType GetSiegeEngineType()
		{
			return DefaultSiegeEngineTypes.FireBallista;
		}

		// Token: 0x06000E03 RID: 3587 RVA: 0x0006DC5C File Offset: 0x0006BE5C
		public override float ProcessTargetValue(float baseValue, TargetFlags flags)
		{
			if (Extensions.HasAnyFlag<TargetFlags>(flags, 64))
			{
				return -1000f;
			}
			if (Extensions.HasAnyFlag<TargetFlags>(flags, 512))
			{
				baseValue *= 2f;
			}
			if (Extensions.HasAnyFlag<TargetFlags>(flags, 2))
			{
				baseValue *= 2f;
			}
			if (Extensions.HasAnyFlag<TargetFlags>(flags, 128))
			{
				baseValue *= 1000f;
			}
			return baseValue;
		}
	}
}
