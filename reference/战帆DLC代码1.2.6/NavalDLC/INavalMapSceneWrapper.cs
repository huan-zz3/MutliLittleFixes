using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace NavalDLC
{
	// Token: 0x02000019 RID: 25
	public interface INavalMapSceneWrapper
	{
		// Token: 0x06000115 RID: 277
		List<ValueTuple<CampaignVec2, float>> GetSpawnPoints(string tag);

		// Token: 0x06000116 RID: 278
		Vec2 GetWindAtPosition(Vec2 position);

		// Token: 0x06000117 RID: 279
		void Tick(float dt);
	}
}
