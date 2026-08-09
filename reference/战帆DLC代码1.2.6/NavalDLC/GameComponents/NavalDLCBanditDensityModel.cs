using System;
using StoryMode;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000109 RID: 265
	public class NavalDLCBanditDensityModel : BanditDensityModel
	{
		// Token: 0x17000348 RID: 840
		// (get) Token: 0x06001349 RID: 4937 RVA: 0x0008C3DD File Offset: 0x0008A5DD
		private Clan DeserterClan
		{
			get
			{
				if (this._deserterClan == null)
				{
					this._deserterClan = Clan.FindFirst((Clan x) => x.StringId == "deserters");
				}
				return this._deserterClan;
			}
		}

		// Token: 0x17000349 RID: 841
		// (get) Token: 0x0600134A RID: 4938 RVA: 0x0008C417 File Offset: 0x0008A617
		public override int NumberOfMinimumBanditPartiesInAHideoutToInfestIt
		{
			get
			{
				return base.BaseModel.NumberOfMinimumBanditPartiesInAHideoutToInfestIt;
			}
		}

		// Token: 0x1700034A RID: 842
		// (get) Token: 0x0600134B RID: 4939 RVA: 0x0008C424 File Offset: 0x0008A624
		public override int NumberOfMaximumBanditPartiesInEachHideout
		{
			get
			{
				return base.BaseModel.NumberOfMaximumBanditPartiesInEachHideout;
			}
		}

		// Token: 0x1700034B RID: 843
		// (get) Token: 0x0600134C RID: 4940 RVA: 0x0008C431 File Offset: 0x0008A631
		public override int NumberOfMaximumBanditPartiesAroundEachHideout
		{
			get
			{
				return base.BaseModel.NumberOfMaximumBanditPartiesAroundEachHideout;
			}
		}

		// Token: 0x1700034C RID: 844
		// (get) Token: 0x0600134D RID: 4941 RVA: 0x0008C43E File Offset: 0x0008A63E
		public override int NumberOfMinimumBanditTroopsInHideoutMission
		{
			get
			{
				return base.BaseModel.NumberOfMinimumBanditTroopsInHideoutMission;
			}
		}

		// Token: 0x1700034D RID: 845
		// (get) Token: 0x0600134E RID: 4942 RVA: 0x0008C44C File Offset: 0x0008A64C
		public override int NumberOfInitialHideoutsAtEachBanditFaction
		{
			get
			{
				StoryModeManager storyModeManager = StoryModeManager.Current;
				bool flag;
				if (storyModeManager == null)
				{
					flag = false;
				}
				else
				{
					MainStoryLine mainStoryLine = storyModeManager.MainStoryLine;
					bool? flag2 = ((mainStoryLine != null) ? new bool?(mainStoryLine.IsPlayerInteractionRestricted) : null);
					bool flag3 = true;
					flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
				if (flag)
				{
					return 0;
				}
				return 8;
			}
		}

		// Token: 0x1700034E RID: 846
		// (get) Token: 0x0600134F RID: 4943 RVA: 0x0008C4A0 File Offset: 0x0008A6A0
		public override int NumberOfMaximumHideoutsAtEachBanditFaction
		{
			get
			{
				StoryModeManager storyModeManager = StoryModeManager.Current;
				bool flag;
				if (storyModeManager == null)
				{
					flag = false;
				}
				else
				{
					MainStoryLine mainStoryLine = storyModeManager.MainStoryLine;
					bool? flag2 = ((mainStoryLine != null) ? new bool?(mainStoryLine.IsPlayerInteractionRestricted) : null);
					bool flag3 = true;
					flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
				if (flag)
				{
					return 0;
				}
				return 9;
			}
		}

		// Token: 0x1700034F RID: 847
		// (get) Token: 0x06001350 RID: 4944 RVA: 0x0008C4F2 File Offset: 0x0008A6F2
		public override int NumberOfMaximumTroopCountForFirstFightInHideout
		{
			get
			{
				return base.BaseModel.NumberOfMaximumTroopCountForFirstFightInHideout;
			}
		}

		// Token: 0x17000350 RID: 848
		// (get) Token: 0x06001351 RID: 4945 RVA: 0x0008C4FF File Offset: 0x0008A6FF
		public override int NumberOfMaximumTroopCountForBossFightInHideout
		{
			get
			{
				return base.BaseModel.NumberOfMaximumTroopCountForBossFightInHideout;
			}
		}

		// Token: 0x17000351 RID: 849
		// (get) Token: 0x06001352 RID: 4946 RVA: 0x0008C50C File Offset: 0x0008A70C
		public override float SpawnPercentageForFirstFightInHideoutMission
		{
			get
			{
				return base.BaseModel.SpawnPercentageForFirstFightInHideoutMission;
			}
		}

		// Token: 0x06001353 RID: 4947 RVA: 0x0008C519 File Offset: 0x0008A719
		public override int GetMaximumTroopCountForHideoutMission(MobileParty party, bool isAssault)
		{
			return base.BaseModel.GetMaximumTroopCountForHideoutMission(party, isAssault);
		}

		// Token: 0x06001354 RID: 4948 RVA: 0x0008C528 File Offset: 0x0008A728
		public override bool IsPositionInsideNavalSafeZone(CampaignVec2 position)
		{
			if (position.IsValid() && !position.IsOnLand)
			{
				Settlement item = Campaign.Current.Models.MapDistanceModel.GetClosestEntranceToFace(position.Face, 2).Item1;
				float distance = Campaign.Current.Models.MapDistanceModel.GetDistance(item, ref position, true, 2);
				float num = (item.IsVillage ? 7f : 15f);
				if (distance < num)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001355 RID: 4949 RVA: 0x0008C59C File Offset: 0x0008A79C
		public override int GetMaxSupportedNumberOfLootersForClan(Clan clan)
		{
			StoryModeManager storyModeManager = StoryModeManager.Current;
			bool flag;
			if (storyModeManager == null)
			{
				flag = false;
			}
			else
			{
				MainStoryLine mainStoryLine = storyModeManager.MainStoryLine;
				bool? flag2 = ((mainStoryLine != null) ? new bool?(mainStoryLine.IsPlayerInteractionRestricted) : null);
				bool flag3 = true;
				flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
			}
			if (flag)
			{
				return 0;
			}
			if (clan.HasNavalNavigationCapability)
			{
				return NavalDLCManager.Instance.NavalMapSceneWrapper.GetSpawnPoints(clan.StringId).Count;
			}
			if (clan.StringId == "looters")
			{
				return 300 - ((this.DeserterClan != null) ? this.DeserterClan.WarPartyComponents.Count : 0);
			}
			return base.BaseModel.GetMaxSupportedNumberOfLootersForClan(clan);
		}

		// Token: 0x06001356 RID: 4950 RVA: 0x0008C64F File Offset: 0x0008A84F
		public override int GetMinimumTroopCountForHideoutMission(MobileParty party, bool isAssault)
		{
			return base.BaseModel.GetMinimumTroopCountForHideoutMission(party, isAssault);
		}

		// Token: 0x04000ABE RID: 2750
		private const float GetNavalSafeZoneRadiusForFortificationPort = 15f;

		// Token: 0x04000ABF RID: 2751
		private const float GetNavalSafeZoneRadiusForVillagePort = 7f;

		// Token: 0x04000AC0 RID: 2752
		private Clan _deserterClan;
	}
}
