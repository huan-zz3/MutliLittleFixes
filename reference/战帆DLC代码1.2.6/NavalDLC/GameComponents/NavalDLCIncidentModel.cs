using System;
using NavalDLC.Storyline;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;

namespace NavalDLC.GameComponents
{
	// Token: 0x0200011A RID: 282
	public class NavalDLCIncidentModel : IncidentModel
	{
		// Token: 0x0600141C RID: 5148 RVA: 0x0008FE01 File Offset: 0x0008E001
		public override float GetIncidentTriggerGlobalProbability()
		{
			if (NavalStorylineData.IsNavalStoryLineActive())
			{
				return 0f;
			}
			return base.BaseModel.GetIncidentTriggerGlobalProbability();
		}

		// Token: 0x0600141D RID: 5149 RVA: 0x0008FE1B File Offset: 0x0008E01B
		public override float GetIncidentTriggerProbabilityDuringSiege()
		{
			if (NavalStorylineData.IsNavalStoryLineActive())
			{
				return 0f;
			}
			return base.BaseModel.GetIncidentTriggerProbabilityDuringSiege();
		}

		// Token: 0x0600141E RID: 5150 RVA: 0x0008FE35 File Offset: 0x0008E035
		public override float GetIncidentTriggerProbabilityDuringWait()
		{
			if (NavalStorylineData.IsNavalStoryLineActive())
			{
				return 0f;
			}
			return base.BaseModel.GetIncidentTriggerProbabilityDuringWait();
		}

		// Token: 0x0600141F RID: 5151 RVA: 0x0008FE4F File Offset: 0x0008E04F
		public override CampaignTime GetMaxGlobalCooldownTime()
		{
			return base.BaseModel.GetMaxGlobalCooldownTime();
		}

		// Token: 0x06001420 RID: 5152 RVA: 0x0008FE5C File Offset: 0x0008E05C
		public override CampaignTime GetMinGlobalCooldownTime()
		{
			return base.BaseModel.GetMinGlobalCooldownTime();
		}
	}
}
