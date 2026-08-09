using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace NavalDLC.CampaignBehaviors
{
	// Token: 0x02000166 RID: 358
	public class NavalDLCTutorialBoxCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x170003F3 RID: 1011
		// (get) Token: 0x0600179B RID: 6043 RVA: 0x000A0F02 File Offset: 0x0009F102
		public MBReadOnlyList<CampaignTutorial> AvailableTutorials
		{
			get
			{
				return this._availableTutorials;
			}
		}

		// Token: 0x0600179C RID: 6044 RVA: 0x000A0F0C File Offset: 0x0009F10C
		public override void RegisterEvents()
		{
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.OnTutorialCompletedEvent.AddNonSerializedListener(this, new Action<string>(this.OnTutorialCompleted));
			CampaignEvents.CollectAvailableTutorialsEvent.AddNonSerializedListener(this, new Action<List<CampaignTutorial>>(this.OnTutorialListRequested));
			CampaignEvents.OnQuestStartedEvent.AddNonSerializedListener(this, new Action<QuestBase>(this.OnQuestStarted));
			CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, new Action<QuestBase, QuestBase.QuestCompleteDetails>(this.OnQuestCompleted));
			CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, new Action<IMission>(this.OnMissionEnded));
			Game.Current.EventManager.RegisterEvent<ResetAllTutorialsEvent>(new Action<ResetAllTutorialsEvent>(this.OnResetAllTutorials));
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Combine(Input.OnGamepadActiveStateChanged, new Action(NavalDLCTutorialBoxCampaignBehavior.UpdateKeyTexts));
			HotKeyManager.OnKeybindsChanged += new HotKeyManager.OnKeybindsChangedEvent(NavalDLCTutorialBoxCampaignBehavior.UpdateKeyTexts);
			NavalDLCTutorialBoxCampaignBehavior.UpdateKeyTexts();
		}

		// Token: 0x0600179D RID: 6045 RVA: 0x000A0FF4 File Offset: 0x0009F1F4
		private void OnMissionEnded(IMission obj)
		{
			if (this._tutorialsToResetAfterMission.Count > 0)
			{
				foreach (CampaignTutorial campaignTutorial in this._tutorialsToResetAfterMission)
				{
					this._availableTutorials.Add(campaignTutorial);
					this._shownTutorials.Remove(campaignTutorial.TutorialTypeId);
					if (!this._tutorialBackup.ContainsKey(campaignTutorial.TutorialTypeId))
					{
						this._tutorialBackup.Add(campaignTutorial.TutorialTypeId, campaignTutorial.Priority);
					}
				}
				this._availableTutorials.Sort(delegate(CampaignTutorial x, CampaignTutorial y)
				{
					int priority = x.Priority;
					return priority.CompareTo(y.Priority);
				});
				this._tutorialsToResetAfterMission.Clear();
			}
		}

		// Token: 0x0600179E RID: 6046 RVA: 0x000A10D0 File Offset: 0x0009F2D0
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<List<string>>("_shownTutorials", ref this._shownTutorials);
			dataStore.SyncData<Dictionary<string, int>>("_tutorialBackup", ref this._tutorialBackup);
		}

		// Token: 0x0600179F RID: 6047 RVA: 0x000A10F8 File Offset: 0x0009F2F8
		private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
		{
			this.AddTutorial("ShipControlTutorial", 1);
			this.AddTutorial("ShipOarsmanTutorial", 2);
			this.AddTutorial("ShipCameraTutorial", 3);
			this.AddTutorial("ShipSailTutorial", 4);
			this.AddTutorial("ShipCloseSailTutorial", 5);
			this.AddTutorial("ShipBoardingApproachTutorial", 6);
			this.AddTutorial("ShipBoardingAttemptBoardingTutorial", 7);
			this.AddTutorial("ShipBoardingTroopChargeTutorial", 8);
			this.AddTutorial("ShipCutLooseTutorial", 9);
			this.AddTutorial("ShipCommandingShipsTutorial", 10);
			this._availableTutorials.Sort(delegate(CampaignTutorial x, CampaignTutorial y)
			{
				int priority = x.Priority;
				return priority.CompareTo(y.Priority);
			});
		}

		// Token: 0x060017A0 RID: 6048 RVA: 0x000A11A9 File Offset: 0x0009F3A9
		private void OnQuestStarted(QuestBase quest)
		{
			this._availableTutorials.Sort(delegate(CampaignTutorial x, CampaignTutorial y)
			{
				int priority = x.Priority;
				return priority.CompareTo(y.Priority);
			});
		}

		// Token: 0x060017A1 RID: 6049 RVA: 0x000A11D5 File Offset: 0x0009F3D5
		private void OnQuestCompleted(QuestBase quest, QuestBase.QuestCompleteDetails detail)
		{
			this._availableTutorials.Sort(delegate(CampaignTutorial x, CampaignTutorial y)
			{
				int priority = x.Priority;
				return priority.CompareTo(y.Priority);
			});
		}

		// Token: 0x060017A2 RID: 6050 RVA: 0x000A1204 File Offset: 0x0009F404
		private void OnTutorialCompleted(string completedTutorialType)
		{
			CampaignTutorial campaignTutorial = this._availableTutorials.Find((CampaignTutorial t) => t.TutorialTypeId == completedTutorialType);
			if (campaignTutorial != null)
			{
				if (campaignTutorial.TutorialTypeId == "ShipControlTutorial" || campaignTutorial.TutorialTypeId == "ShipSailTutorial" || campaignTutorial.TutorialTypeId == "ShipOarsmanTutorial" || campaignTutorial.TutorialTypeId == "ShipBoardingApproachTutorial" || campaignTutorial.TutorialTypeId == "ShipBoardingAttemptBoardingTutorial" || campaignTutorial.TutorialTypeId == "ShipBoardingTroopChargeTutorial" || campaignTutorial.TutorialTypeId == "ShipCutLooseTutorial" || campaignTutorial.TutorialTypeId == "ShipCommandingShipsTutorial" || campaignTutorial.TutorialTypeId == "ShipCameraTutorial" || campaignTutorial.TutorialTypeId == "ShipCloseSailTutorial")
				{
					this._tutorialsToResetAfterMission.Add(campaignTutorial);
				}
				this._availableTutorials.Remove(campaignTutorial);
				this._shownTutorials.Add(completedTutorialType);
				this._tutorialBackup.Remove(completedTutorialType);
			}
		}

		// Token: 0x060017A3 RID: 6051 RVA: 0x000A1334 File Offset: 0x0009F534
		private void OnTutorialListRequested(List<CampaignTutorial> campaignTutorials)
		{
			foreach (CampaignTutorial campaignTutorial in this.AvailableTutorials)
			{
				campaignTutorials.Add(campaignTutorial);
			}
		}

		// Token: 0x060017A4 RID: 6052 RVA: 0x000A1388 File Offset: 0x0009F588
		private void BackupTutorial(string tutorialTypeId, int priority)
		{
			if (!this._shownTutorials.Contains(tutorialTypeId) && !this._tutorialBackup.ContainsKey(tutorialTypeId))
			{
				this._tutorialBackup.Add(tutorialTypeId, priority);
			}
		}

		// Token: 0x060017A5 RID: 6053 RVA: 0x000A13B4 File Offset: 0x0009F5B4
		private void AddTutorial(string tutorialTypeId, int priority)
		{
			if (!this._shownTutorials.Contains(tutorialTypeId))
			{
				CampaignTutorial campaignTutorial = new CampaignTutorial(tutorialTypeId, priority);
				this._availableTutorials.Add(campaignTutorial);
				if (!this._tutorialBackup.ContainsKey(tutorialTypeId))
				{
					this._tutorialBackup.Add(tutorialTypeId, priority);
				}
			}
		}

		// Token: 0x060017A6 RID: 6054 RVA: 0x000A13FE File Offset: 0x0009F5FE
		public void OnResetAllTutorials(ResetAllTutorialsEvent obj)
		{
			this._shownTutorials.Clear();
		}

		// Token: 0x060017A7 RID: 6055 RVA: 0x000A140C File Offset: 0x0009F60C
		private static void UpdateKeyTexts()
		{
			string keyHyperlinkText = HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("NavalShipControlsHotKeyCategory", 110), 1f);
			GameTexts.SetVariable("TOGGLE_SAIL_KEY", keyHyperlinkText);
			string keyHyperlinkText2 = HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("NavalShipControlsHotKeyCategory", 111), 1f);
			GameTexts.SetVariable("TOGGLE_OARSMEN_KEY", keyHyperlinkText2);
			string keyHyperlinkText3 = HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("NavalShipControlsHotKeyCategory", 112), 1f);
			GameTexts.SetVariable("TOGGLE_CAMERA_KEY", keyHyperlinkText3);
			string keyHyperlinkText4 = HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("NavalShipControlsHotKeyCategory", 111), 1f);
			GameTexts.SetVariable("CUT_LOOSE_KEY", keyHyperlinkText4);
			string keyHyperlinkText5 = HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("NavalShipControlsHotKeyCategory", 114), 1f);
			GameTexts.SetVariable("ATTEMPT_BOARDING_KEY", keyHyperlinkText5);
		}

		// Token: 0x04000BD8 RID: 3032
		private List<string> _shownTutorials = new List<string>();

		// Token: 0x04000BD9 RID: 3033
		private readonly MBList<CampaignTutorial> _availableTutorials = new MBList<CampaignTutorial>();

		// Token: 0x04000BDA RID: 3034
		private Dictionary<string, int> _tutorialBackup = new Dictionary<string, int>();

		// Token: 0x04000BDB RID: 3035
		private List<CampaignTutorial> _tutorialsToResetAfterMission = new List<CampaignTutorial>();
	}
}
