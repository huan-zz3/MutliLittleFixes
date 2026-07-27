using System;
using System.Collections.Generic;
using Helpers;
using NavalDLC.CharacterDevelopment;
using NavalDLC.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Map;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Library;

namespace NavalDLC.CampaignBehaviors
{
	// Token: 0x0200016E RID: 366
	public class NavalStormriderCampaignBehaviour : CampaignBehaviorBase
	{
		// Token: 0x060017F3 RID: 6131 RVA: 0x000A3824 File Offset: 0x000A1A24
		public override void RegisterEvents()
		{
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.OnHourlyTick));
			CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(this.TickEvent));
			CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.OnMobilePartyDestroyed));
		}

		// Token: 0x060017F4 RID: 6132 RVA: 0x000A3878 File Offset: 0x000A1A78
		private void TickEvent(float deltaTime)
		{
			if (this._playerLastStormEnterTime.ElapsedDaysUntilNow > 1f || this._playerLastStormEnterTime == CampaignTime.Never)
			{
				foreach (Storm storm in NavalDLCManager.Instance.StormManager.SpawnedStorms)
				{
					if (MobileParty.MainParty.Position.DistanceSquared(storm.CurrentPosition) <= storm.EffectRadius * storm.EffectRadius)
					{
						this._playerLastStormEnterTime = CampaignTime.Now;
						NavalStormriderCampaignBehaviour.AddXpToTroops(MobileParty.MainParty, MathF.Round(NavalPerks.Shipmaster.Stormrider.PrimaryBonus));
					}
				}
			}
		}

		// Token: 0x060017F5 RID: 6133 RVA: 0x000A3940 File Offset: 0x000A1B40
		private void OnHourlyTick()
		{
			foreach (Storm storm in NavalDLCManager.Instance.StormManager.SpawnedStorms)
			{
				LocatableSearchData<MobileParty> locatableSearchData = MobileParty.StartFindingLocatablesAroundPosition(storm.CurrentPosition, storm.EffectRadius);
				MobileParty mobileParty = MobileParty.FindNextLocatable(ref locatableSearchData);
				while (mobileParty != null)
				{
					if (mobileParty == MobileParty.MainParty)
					{
						mobileParty = MobileParty.FindNextLocatable(ref locatableSearchData);
					}
					else
					{
						if (mobileParty.IsCurrentlyAtSea && mobileParty.MapEvent == null && (!this._partiesEnteredStorm.ContainsKey(mobileParty) || this._partiesEnteredStorm[mobileParty].ElapsedDaysUntilNow > 1f))
						{
							this.OnPartyEnteredStorm(mobileParty);
						}
						mobileParty = MobileParty.FindNextLocatable(ref locatableSearchData);
					}
				}
			}
		}

		// Token: 0x060017F6 RID: 6134 RVA: 0x000A3A18 File Offset: 0x000A1C18
		private void OnPartyEnteredStorm(MobileParty party)
		{
			if (party.HasPerk(NavalPerks.Shipmaster.Stormrider, false))
			{
				this._partiesEnteredStorm[party] = CampaignTime.Now;
				NavalStormriderCampaignBehaviour.AddXpToTroops(party, MathF.Round(NavalPerks.Shipmaster.Stormrider.PrimaryBonus));
			}
		}

		// Token: 0x060017F7 RID: 6135 RVA: 0x000A3A50 File Offset: 0x000A1C50
		private static void AddXpToTroops(MobileParty party, int amount)
		{
			TroopRoster memberRoster = party.MemberRoster;
			for (int i = 0; i < memberRoster.Count; i++)
			{
				TroopRosterElement elementCopyAtIndex = memberRoster.GetElementCopyAtIndex(i);
				int num;
				if (!elementCopyAtIndex.Character.IsHero && MobilePartyHelper.CanTroopGainXp(party.Party, elementCopyAtIndex.Character, ref num))
				{
					int num2 = Math.Min(num, amount);
					memberRoster.AddXpToTroopAtIndex(i, num2);
				}
			}
		}

		// Token: 0x060017F8 RID: 6136 RVA: 0x000A3AB1 File Offset: 0x000A1CB1
		private void OnMobilePartyDestroyed(MobileParty mobileParty, PartyBase party)
		{
			this._partiesEnteredStorm.Remove(mobileParty);
		}

		// Token: 0x060017F9 RID: 6137 RVA: 0x000A3AC0 File Offset: 0x000A1CC0
		private void DoCleanUp()
		{
			List<MobileParty> list = new List<MobileParty>();
			foreach (KeyValuePair<MobileParty, CampaignTime> keyValuePair in this._partiesEnteredStorm)
			{
				if (keyValuePair.Value.ElapsedDaysUntilNow > 1f)
				{
					list.Add(keyValuePair.Key);
				}
			}
			foreach (MobileParty mobileParty in list)
			{
				this._partiesEnteredStorm.Remove(mobileParty);
			}
		}

		// Token: 0x060017FA RID: 6138 RVA: 0x000A3B7C File Offset: 0x000A1D7C
		public override void SyncData(IDataStore dataStore)
		{
			this.DoCleanUp();
			dataStore.SyncData<Dictionary<MobileParty, CampaignTime>>("_partiesEnteredStorm", ref this._partiesEnteredStorm);
			dataStore.SyncData<CampaignTime>("_playerLastStormEnterTime", ref this._playerLastStormEnterTime);
		}

		// Token: 0x04000BF0 RID: 3056
		private Dictionary<MobileParty, CampaignTime> _partiesEnteredStorm = new Dictionary<MobileParty, CampaignTime>();

		// Token: 0x04000BF1 RID: 3057
		private CampaignTime _playerLastStormEnterTime = CampaignTime.Never;
	}
}
