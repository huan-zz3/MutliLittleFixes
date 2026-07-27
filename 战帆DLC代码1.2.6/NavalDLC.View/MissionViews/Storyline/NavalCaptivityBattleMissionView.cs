using System;
using NavalDLC.Storyline;
using NavalDLC.View.Missions;
using SandBox.ViewModelCollection.Missions.NameMarker;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Hints;
using TaleWorlds.MountAndBlade.Missions.MissionLogics;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace NavalDLC.View.MissionViews.Storyline
{
	// Token: 0x02000029 RID: 41
	public class NavalCaptivityBattleMissionView : MissionView
	{
		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000102 RID: 258 RVA: 0x00007E52 File Offset: 0x00006052
		private TextObject FreeHintText
		{
			get
			{
				return new TextObject("{=EThbCDao}Free yourself", null);
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000103 RID: 259 RVA: 0x00007E5F File Offset: 0x0000605F
		// (set) Token: 0x06000104 RID: 260 RVA: 0x00007E67 File Offset: 0x00006067
		public bool AreMarkersDirty { get; private set; }

		// Token: 0x06000105 RID: 261 RVA: 0x00007E70 File Offset: 0x00006070
		public override void OnBehaviorInitialize()
		{
			MissionNameMarkerFactory.PushContext("NavalCaptivityBattleContext", false).AddProvider<NavalStorylineCaptivityMissionNameMarkerProvider>();
			this._captivityMissionController = base.Mission.GetMissionBehavior<NavalStorylineCaptivityMissionController>();
			this._missionHintLogic = base.Mission.GetMissionBehavior<MissionHintLogic>();
			if (this._captivityMissionController != null)
			{
				NavalStorylineCaptivityMissionController captivityMissionController = this._captivityMissionController;
				captivityMissionController.OnMarkedObjectStatusChangedEvent = (Action)Delegate.Combine(captivityMissionController.OnMarkedObjectStatusChangedEvent, new Action(this.OnMarkersAreDirty));
				NavalStorylineCaptivityMissionController captivityMissionController2 = this._captivityMissionController;
				captivityMissionController2.OnPlayerStartedEscapeEvent = (Action)Delegate.Combine(captivityMissionController2.OnPlayerStartedEscapeEvent, new Action(this.OnPlayerStartedEscape));
				NavalStorylineCaptivityMissionController captivityMissionController3 = this._captivityMissionController;
				captivityMissionController3.OnConversationSetupEvent = (Action<Vec3>)Delegate.Combine(captivityMissionController3.OnConversationSetupEvent, new Action<Vec3>(this.OnConversationSetup));
				NavalStorylineCaptivityMissionController captivityMissionController4 = this._captivityMissionController;
				captivityMissionController4.OnStartFadeOutEvent = (Action<float, float, float>)Delegate.Combine(captivityMissionController4.OnStartFadeOutEvent, new Action<float, float, float>(this.OnStartFadeOut));
				NavalStorylineCaptivityMissionController captivityMissionController5 = this._captivityMissionController;
				captivityMissionController5.OnFirstHighlightClearedEvent = (Action)Delegate.Combine(captivityMissionController5.OnFirstHighlightClearedEvent, new Action(this.OnFirstHighlightCleared));
				NavalStorylineCaptivityMissionController captivityMissionController6 = this._captivityMissionController;
				captivityMissionController6.OnOarsmenLevelChanged = (Action<int>)Delegate.Combine(captivityMissionController6.OnOarsmenLevelChanged, new Action<int>(this.OnOarsmenLevelChanged));
			}
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00007FA4 File Offset: 0x000061A4
		public override void OnMissionScreenFinalize()
		{
			if (this._captivityMissionController != null)
			{
				NavalStorylineCaptivityMissionController captivityMissionController = this._captivityMissionController;
				captivityMissionController.OnMarkedObjectStatusChangedEvent = (Action)Delegate.Remove(captivityMissionController.OnMarkedObjectStatusChangedEvent, new Action(this.OnMarkersAreDirty));
			}
			MissionNameMarkerFactory.PopContext("NavalCaptivityBattleContext");
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00007FDF File Offset: 0x000061DF
		public override void AfterStart()
		{
			this.ShowFreePlayerHintText();
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00007FE7 File Offset: 0x000061E7
		private void OnPlayerStartedEscape()
		{
			this._missionHintLogic.Clear();
			this.OnPlayerStartedEscapeInternal();
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00007FFA File Offset: 0x000061FA
		private void OnOarsmenLevelChanged(int level)
		{
			this.OnOarsmenLevelChangedInternal(level);
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00008003 File Offset: 0x00006203
		private void OnStartFadeOut(float fadeInDuration, float blackDuration, float fadeOutDuration)
		{
			ScreenFadeController.BeginFadeOutAndIn(fadeOutDuration, blackDuration, fadeInDuration);
		}

		// Token: 0x0600010B RID: 267 RVA: 0x0000800D File Offset: 0x0000620D
		private void OnConversationSetup(Vec3 direction)
		{
			base.MissionScreen.CameraBearing = direction.RotationZ;
		}

		// Token: 0x0600010C RID: 268 RVA: 0x00008024 File Offset: 0x00006224
		private void ShowFreePlayerHintText()
		{
			if (this._missionHintLogic.ActiveHint != null)
			{
				this._missionHintLogic.Clear();
			}
			MissionHint missionHint = MissionHint.CreateWithKeyAndAction(this.FreeHintText, HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13));
			this._missionHintLogic.SetActiveHint(missionHint);
		}

		// Token: 0x0600010D RID: 269 RVA: 0x0000806D File Offset: 0x0000626D
		private void OnFirstHighlightCleared()
		{
			this.OnFirstHighlightClearedInternal();
		}

		// Token: 0x0600010E RID: 270 RVA: 0x00008075 File Offset: 0x00006275
		protected virtual void OnFirstHighlightClearedInternal()
		{
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00008077 File Offset: 0x00006277
		protected virtual void OnPlayerStartedEscapeInternal()
		{
		}

		// Token: 0x06000110 RID: 272 RVA: 0x00008079 File Offset: 0x00006279
		protected virtual void OnOarsmenLevelChangedInternal(int level)
		{
		}

		// Token: 0x06000111 RID: 273 RVA: 0x0000807B File Offset: 0x0000627B
		private void OnMarkersAreDirty()
		{
			this.AreMarkersDirty = true;
		}

		// Token: 0x06000112 RID: 274 RVA: 0x00008084 File Offset: 0x00006284
		public void OnDirtyMarkersHandled()
		{
			this.AreMarkersDirty = false;
		}

		// Token: 0x04000062 RID: 98
		private const int FreeHintHotKey = 13;

		// Token: 0x04000063 RID: 99
		private NavalStorylineCaptivityMissionController _captivityMissionController;

		// Token: 0x04000064 RID: 100
		private MissionHintLogic _missionHintLogic;
	}
}
