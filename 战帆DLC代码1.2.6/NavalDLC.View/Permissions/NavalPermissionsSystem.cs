using System;
using NavalDLC.Storyline;
using NavalDLC.Storyline.Quests;
using SandBox.View.Map.Navigation.NavigationElements;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Events;
using TaleWorlds.CampaignSystem.ViewModelCollection.KingdomManagement;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace NavalDLC.View.Permissions
{
	// Token: 0x02000016 RID: 22
	public class NavalPermissionsSystem
	{
		// Token: 0x0600008A RID: 138 RVA: 0x000059A0 File Offset: 0x00003BA0
		private NavalPermissionsSystem()
		{
			this.RegisterEvents();
		}

		// Token: 0x0600008B RID: 139 RVA: 0x000059AE File Offset: 0x00003BAE
		public static void OnInitialize()
		{
			if (NavalPermissionsSystem.Current == null)
			{
				NavalPermissionsSystem.Current = new NavalPermissionsSystem();
			}
		}

		// Token: 0x0600008C RID: 140 RVA: 0x000059C1 File Offset: 0x00003BC1
		internal static void OnUnload()
		{
			if (NavalPermissionsSystem.Current != null)
			{
				NavalPermissionsSystem.Current.UnregisterEvents();
				NavalPermissionsSystem.Current = null;
			}
		}

		// Token: 0x0600008D RID: 141 RVA: 0x000059DA File Offset: 0x00003BDA
		private void OnClanScreenPermission(ClanScreenPermissionEvent obj)
		{
		}

		// Token: 0x0600008E RID: 142 RVA: 0x000059DC File Offset: 0x00003BDC
		private void OnSettlementOverlayTalkPermission(SettlementOverlayTalkPermissionEvent obj)
		{
			if (Settlement.CurrentSettlement == NavalStorylineData.HomeSettlement && obj.HeroToTalkTo == NavalStorylineData.Gunnar && Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(SpeakToGunnarAndSisterQuest)))
			{
				obj.IsTalkAvailable(false, new TextObject("{=bkppYuaB}Take a walk around the port and find Gunnar to talk to him.", null));
			}
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00005A34 File Offset: 0x00003C34
		private void OnSettlementOverlayQuickTalkPermission(SettlementOverylayQuickTalkPermissionEvent obj)
		{
			if (NavalStorylineData.IsNavalStorylineHero(obj.HeroToTalkTo) && (!NavalStorylineData.HasCompletedLast(NavalStorylineData.NavalStorylineStage.Act3Quest5) || Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(SpeakToGunnarAndSisterQuest))))
			{
				if (Campaign.Current.QuestManager.IsThereActiveQuestWithType(typeof(SpeakToGunnarAndSisterQuest)))
				{
					obj.IsTalkAvailable(false, new TextObject("{=bkppYuaB}Take a walk around the port and find Gunnar to talk to him.", null));
					return;
				}
				obj.IsTalkAvailable(false, new TextObject("{=UjERCi2F}This feature is disabled.", null));
			}
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00005ABB File Offset: 0x00003CBB
		private void OnSettlementOverlayLeaveMemberPermission(SettlementOverlayLeaveCharacterPermissionEvent obj)
		{
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00005ABD File Offset: 0x00003CBD
		private void OnLeaveKingdomPermissionEvent(LeaveKingdomPermissionEvent obj)
		{
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00005AC0 File Offset: 0x00003CC0
		private void RegisterEvents()
		{
			Game.Current.EventManager.RegisterEvent<ClanScreenPermissionEvent>(new Action<ClanScreenPermissionEvent>(this.OnClanScreenPermission));
			Game.Current.EventManager.RegisterEvent<SettlementOverlayTalkPermissionEvent>(new Action<SettlementOverlayTalkPermissionEvent>(this.OnSettlementOverlayTalkPermission));
			Game.Current.EventManager.RegisterEvent<SettlementOverylayQuickTalkPermissionEvent>(new Action<SettlementOverylayQuickTalkPermissionEvent>(this.OnSettlementOverlayQuickTalkPermission));
			Game.Current.EventManager.RegisterEvent<SettlementOverlayLeaveCharacterPermissionEvent>(new Action<SettlementOverlayLeaveCharacterPermissionEvent>(this.OnSettlementOverlayLeaveMemberPermission));
			Game.Current.EventManager.RegisterEvent<LeaveKingdomPermissionEvent>(new Action<LeaveKingdomPermissionEvent>(this.OnLeaveKingdomPermissionEvent));
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00005B54 File Offset: 0x00003D54
		internal void UnregisterEvents()
		{
			Game.Current.EventManager.UnregisterEvent<ClanScreenPermissionEvent>(new Action<ClanScreenPermissionEvent>(this.OnClanScreenPermission));
			Game.Current.EventManager.UnregisterEvent<SettlementOverlayTalkPermissionEvent>(new Action<SettlementOverlayTalkPermissionEvent>(this.OnSettlementOverlayTalkPermission));
			Game.Current.EventManager.UnregisterEvent<SettlementOverylayQuickTalkPermissionEvent>(new Action<SettlementOverylayQuickTalkPermissionEvent>(this.OnSettlementOverlayQuickTalkPermission));
			Game.Current.EventManager.UnregisterEvent<SettlementOverlayLeaveCharacterPermissionEvent>(new Action<SettlementOverlayLeaveCharacterPermissionEvent>(this.OnSettlementOverlayLeaveMemberPermission));
			Game.Current.EventManager.UnregisterEvent<LeaveKingdomPermissionEvent>(new Action<LeaveKingdomPermissionEvent>(this.OnLeaveKingdomPermissionEvent));
		}

		// Token: 0x04000029 RID: 41
		private static NavalPermissionsSystem Current;
	}
}
