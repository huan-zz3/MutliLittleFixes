using System;
using NavalDLC.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace NavalDLC.ComponentInterfaces
{
	// Token: 0x02000157 RID: 343
	public abstract class MapStormModel : MBGameModel<MapStormModel>
	{
		// Token: 0x170003DF RID: 991
		// (get) Token: 0x06001658 RID: 5720
		public abstract float MinimumWeatherStrengthInsideStorm { get; }

		// Token: 0x170003E0 RID: 992
		// (get) Token: 0x06001659 RID: 5721
		public abstract int MaximumNumberOfStorms { get; }

		// Token: 0x0600165A RID: 5722
		public abstract float GetPositionDamageForStorm(Storm storm, Vec2 shipPosition, Ship ship);

		// Token: 0x0600165B RID: 5723
		public abstract float GetHourlyIntensityChangeForStorm(Storm storm);

		// Token: 0x0600165C RID: 5724
		public abstract void GetStormLifeSpan(out CampaignTime minimumDuration, out CampaignTime maximumDuration);

		// Token: 0x0600165D RID: 5725
		public abstract CampaignTime GetDevelopingStateDurationOfStorm(Storm storm);

		// Token: 0x0600165E RID: 5726
		public abstract CampaignTime GetFinalizingStateDurationOfStorm(Storm storm);

		// Token: 0x0600165F RID: 5727
		public abstract float GetHourlyStormSpawnChanceForPosition(Vec2 position);

		// Token: 0x06001660 RID: 5728
		public abstract Storm.StormTypes GetSpawnedStormTypeForPosition(Vec2 position);

		// Token: 0x06001661 RID: 5729
		public abstract bool CanPartyGetDamagedByStorm(MobileParty mobileParty);

		// Token: 0x06001662 RID: 5730
		public abstract float GetEffectRadiusOfStorm(Storm storm);

		// Token: 0x06001663 RID: 5731
		public abstract float GetEyeRadiusOfStorm(Storm storm);

		// Token: 0x06001664 RID: 5732
		public abstract float GetSpeedOfStorm(Storm storm);

		// Token: 0x06001665 RID: 5733
		public abstract float GetMaximumWeatherStrengthAtEye(Storm storm);

		// Token: 0x06001666 RID: 5734
		public abstract float GetStormSpawnDistanceSquaredThresholdWithOtherStorms();

		// Token: 0x06001667 RID: 5735
		public abstract float GetNormalizedWindStrengthOfStormForPosition(Vec2 position);
	}
}
