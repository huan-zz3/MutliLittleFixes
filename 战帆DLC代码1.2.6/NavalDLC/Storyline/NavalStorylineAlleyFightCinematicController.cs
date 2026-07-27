using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Hints;
using TaleWorlds.MountAndBlade.Missions.MissionLogics;

namespace NavalDLC.Storyline
{
	// Token: 0x0200002D RID: 45
	public class NavalStorylineAlleyFightCinematicController : MissionLogic
	{
		// Token: 0x1700005C RID: 92
		// (get) Token: 0x0600022E RID: 558 RVA: 0x0000FA07 File Offset: 0x0000DC07
		private TextObject SkipHintText
		{
			get
			{
				return new TextObject("{=FiSENWMB}Skip Cinematic", null);
			}
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x0600022F RID: 559 RVA: 0x0000FA14 File Offset: 0x0000DC14
		// (remove) Token: 0x06000230 RID: 560 RVA: 0x0000FA4C File Offset: 0x0000DC4C
		public event Action<NavalStorylineAlleyFightCinematicController.NavalAlleyFightCinematicState> OnCinematicStateChanged;

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000231 RID: 561 RVA: 0x0000FA84 File Offset: 0x0000DC84
		// (remove) Token: 0x06000232 RID: 562 RVA: 0x0000FABC File Offset: 0x0000DCBC
		public event Action<float, float, float> OnFightEndedEvent;

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000233 RID: 563 RVA: 0x0000FAF4 File Offset: 0x0000DCF4
		// (remove) Token: 0x06000234 RID: 564 RVA: 0x0000FB2C File Offset: 0x0000DD2C
		public event Action<Vec3> OnConversationSetupEvent;

		// Token: 0x06000235 RID: 565 RVA: 0x0000FB64 File Offset: 0x0000DD64
		public override void OnMissionTick(float dt)
		{
			if (!this._isMissionInitialized)
			{
				this.Initialize();
			}
			this.TickCinematic(dt);
			if (this._isPostFightConversationQueued)
			{
				this._postFightDialogueFadeTimer += dt;
				if (!this._isConversationSetup && this._postFightDialogueFadeTimer >= 0.75f)
				{
					this._isConversationSetup = true;
					this._missionController.SetupConversation();
				}
				if (this._postFightDialogueFadeTimer >= 1.75f)
				{
					this._isPostFightConversationQueued = false;
					this._missionController.StartPostFightConversation();
				}
			}
		}

		// Token: 0x06000236 RID: 566 RVA: 0x0000FBE4 File Offset: 0x0000DDE4
		private void Initialize()
		{
			this._isMissionInitialized = true;
			this.UpdateEntityReferences();
			this._missionController = base.Mission.GetMissionBehavior<NavalStorylineAlleyFightMissionController>();
			this._missionHintLogic = base.Mission.GetMissionBehavior<MissionHintLogic>();
			this._cinematicTriggerZone = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("trigger_cutscene"));
			this._cameraEntity = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("sp_camera"));
			this._cameraEntity2 = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("sp_camera_2"));
			this._currentCameraEntity = this._cameraEntity;
			SoundManager.SetListenerFrame(this._currentCameraEntity.GetGlobalFrame(), this._currentCameraEntity.GlobalPosition);
			this._enemyCharacterObject = this._missionController.GetEnemyCharacterObject();
			this._allLines = new List<NavalStorylineAlleyFightCinematicController.ConversationLine>
			{
				new NavalStorylineAlleyFightCinematicController.ConversationLine(new TextObject("{=4nAQl8Vx}Listen, you lot, I'm in a bit of a hurry. If I give you a penny each will you stop pestering me?", null), NavalStorylineData.Gunnar.CharacterObject),
				new NavalStorylineAlleyFightCinematicController.ConversationLine(new TextObject("{=p7Gxhb6O}You're Gunnar of Lagshofn, aren't you? We've got a message from the Sea Hounds for you.", null), this._enemyCharacterObject),
				new NavalStorylineAlleyFightCinematicController.ConversationLine(new TextObject("{=G6NrtQuF}You’ve got a message from those curs? Out with it, then. What’s your message?", null), NavalStorylineData.Gunnar.CharacterObject),
				new NavalStorylineAlleyFightCinematicController.ConversationLine(new TextObject("{=OMpfszRu}The message... the message is that you will die, you damn fool.", null), this._enemyCharacterObject),
				new NavalStorylineAlleyFightCinematicController.ConversationLine(new TextObject("{=qtz4B25N}And how should I die, then? Of old age, while you three work up the courage to attack a wizened graybeard? Go on, you've delivered your message, now scamper off.", null), NavalStorylineData.Gunnar.CharacterObject),
				new NavalStorylineAlleyFightCinematicController.ConversationLine(new TextObject("{=Nmv85ZfP}We’ll send you down to the Pale One right now. Kill him, boys!", null), this._enemyCharacterObject)
			};
		}

		// Token: 0x06000237 RID: 567 RVA: 0x0000FDA7 File Offset: 0x0000DFA7
		private void UpdateEntityReferences()
		{
			base.Mission.Scene.GetEntities(ref this._entities);
		}

		// Token: 0x06000238 RID: 568 RVA: 0x0000FDBF File Offset: 0x0000DFBF
		public void GetCameraFrame(out Vec3 position, out Vec3 forward)
		{
			if (!this._isMissionInitialized)
			{
				this.Initialize();
			}
			position = this._currentCameraEntity.GlobalPosition;
			forward = this._currentCameraEntity.GetGlobalFrame().rotation.f;
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0000FDFB File Offset: 0x0000DFFB
		public float GetFadeDuration()
		{
			return 0.75f;
		}

		// Token: 0x0600023A RID: 570 RVA: 0x0000FE02 File Offset: 0x0000E002
		public float GetBlackScreenDuration()
		{
			return 0.25f;
		}

		// Token: 0x0600023B RID: 571 RVA: 0x0000FE09 File Offset: 0x0000E009
		private void SetCinematicState(NavalStorylineAlleyFightCinematicController.NavalAlleyFightCinematicState newState)
		{
			this._cinematicTimer = 0f;
			this._currentCinematicState = newState;
			this.OnCinematicStateChanged(this._currentCinematicState);
			if (newState == NavalStorylineAlleyFightCinematicController.NavalAlleyFightCinematicState.FirstCamera)
			{
				this.ShowSkipCinematicHintText();
			}
		}

		// Token: 0x0600023C RID: 572 RVA: 0x0000FE38 File Offset: 0x0000E038
		private void TickCinematic(float dt)
		{
			if (this._currentCinematicState == NavalStorylineAlleyFightCinematicController.NavalAlleyFightCinematicState.Completed)
			{
				return;
			}
			if (this._currentCinematicState == NavalStorylineAlleyFightCinematicController.NavalAlleyFightCinematicState.Ready && Agent.Main != null && this._cinematicTriggerZone.GlobalPosition.DistanceSquared(Agent.Main.Position) <= 9f)
			{
				if (Mission.Current.CameraIsFirstPerson)
				{
					Mission.Current.CameraIsFirstPerson = false;
				}
				this._missionController.OnCinematicStarted();
				this.SetCinematicState(NavalStorylineAlleyFightCinematicController.NavalAlleyFightCinematicState.InitialFadeOut);
			}
			this._cinematicTimer += dt;
			if (this._currentCinematicState == NavalStorylineAlleyFightCinematicController.NavalAlleyFightCinematicState.InitialFadeOut && this._cinematicTimer >= 0.75f)
			{
				this.SetCinematicState(NavalStorylineAlleyFightCinematicController.NavalAlleyFightCinematicState.BlackScreen);
			}
			if (this._currentCinematicState == NavalStorylineAlleyFightCinematicController.NavalAlleyFightCinematicState.BlackScreen)
			{
				if (this._cinematicTimer >= 0.25f)
				{
					this.ActivatePlayerEavesdropAnimation();
					this.SetCinematicState(NavalStorylineAlleyFightCinematicController.NavalAlleyFightCinematicState.InitialFadeIn);
				}
			}
			else if (this._currentCinematicState == NavalStorylineAlleyFightCinematicController.NavalAlleyFightCinematicState.InitialFadeIn)
			{
				if (this._cinematicTimer >= 0.75f)
				{
					foreach (NavalStorylineAlleyFightCinematicController.ConversationLine conversationLine in this._allLines)
					{
						MBInformationManager.DialogNotificationHandle dialogNotificationHandle = CampaignInformationManager.AddDialogLine(conversationLine.Line, conversationLine.Speaker, conversationLine.Speaker.FirstCivilianEquipment, 0, 4);
						conversationLine.Handle = dialogNotificationHandle;
					}
					this.SetCinematicState(NavalStorylineAlleyFightCinematicController.NavalAlleyFightCinematicState.FirstCamera);
				}
			}
			else if (this._currentCinematicState == NavalStorylineAlleyFightCinematicController.NavalAlleyFightCinematicState.FirstCamera)
			{
				if (this._cinematicTimer >= 10f)
				{
					this._currentCameraEntity = this._cameraEntity2;
					this.SetCinematicState(NavalStorylineAlleyFightCinematicController.NavalAlleyFightCinematicState.FinalCamera);
					SoundManager.SetListenerFrame(this._currentCameraEntity.GetGlobalFrame(), this._currentCameraEntity.GlobalPosition);
				}
			}
			else if (this._currentCinematicState == NavalStorylineAlleyFightCinematicController.NavalAlleyFightCinematicState.FinalCamera)
			{
				if (this._allLines.TrueForAll((NavalStorylineAlleyFightCinematicController.ConversationLine x) => CampaignInformationManager.GetStatusOfDialogNotification(x.Handle) == 0))
				{
					this.FinishCinematic();
				}
			}
			this.HandleSkipCinematic();
		}

		// Token: 0x0600023D RID: 573 RVA: 0x00010014 File Offset: 0x0000E214
		private void ActivatePlayerEavesdropAnimation()
		{
			if (Agent.Main.GetCurrentAction(0) != ActionIndexCache.act_cutscene_npc_argue_player_1)
			{
				Agent.Main.TryToSheathWeaponInHand(1, 1);
				Agent.Main.TryToSheathWeaponInHand(0, 1);
				Agent.Main.SetActionChannel(0, ref ActionIndexCache.act_none, true, 0L, 0f, 1f, 0f, 0.4f, 0f, false, -0.2f, 0, true);
				Agent.Main.SetActionChannel(1, ref ActionIndexCache.act_none, true, 0L, 0f, 1f, 0f, 0.4f, 0f, false, -0.2f, 0, true);
				GameEntity gameEntity = this._entities.FirstOrDefault<GameEntity>((GameEntity t) => t.HasTag("sp_player_wait"));
				Agent.Main.TeleportToPosition(gameEntity.GlobalPosition);
				Vec3 f = gameEntity.GetGlobalFrame().rotation.f;
				Agent.Main.LookDirection = f;
				this.OnConversationSetupEvent(f);
				Agent.Main.SetActionChannel(0, ref ActionIndexCache.act_cutscene_npc_argue_player_1, true, 0L, 0f, 1f, 0f, 0.4f, 0f, false, -0.2f, 0, true);
			}
		}

		// Token: 0x0600023E RID: 574 RVA: 0x00010158 File Offset: 0x0000E358
		private void FinishCinematic()
		{
			this.SetCinematicState(NavalStorylineAlleyFightCinematicController.NavalAlleyFightCinematicState.Completed);
			this._missionController.StartFight();
			Agent.Main.SetActionChannel(0, ref ActionIndexCache.act_none, true, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
			this._missionHintLogic.Clear();
		}

		// Token: 0x0600023F RID: 575 RVA: 0x000101B8 File Offset: 0x0000E3B8
		private void HandleSkipCinematic()
		{
			if ((this._currentCinematicState == NavalStorylineAlleyFightCinematicController.NavalAlleyFightCinematicState.FirstCamera || this._currentCinematicState == NavalStorylineAlleyFightCinematicController.NavalAlleyFightCinematicState.FinalCamera) && Mission.Current.InputManager.IsGameKeyDown(14))
			{
				if (this._allLines.Any<NavalStorylineAlleyFightCinematicController.ConversationLine>((NavalStorylineAlleyFightCinematicController.ConversationLine x) => CampaignInformationManager.GetStatusOfDialogNotification(x.Handle) > 0))
				{
					foreach (NavalStorylineAlleyFightCinematicController.ConversationLine conversationLine in this._allLines)
					{
						CampaignInformationManager.ClearDialogNotification(conversationLine.Handle, false);
					}
					this.FinishCinematic();
				}
			}
		}

		// Token: 0x06000240 RID: 576 RVA: 0x00010268 File Offset: 0x0000E468
		public void OnFightEnded()
		{
			this._isPostFightConversationQueued = true;
			this.OnFightEndedEvent(0.75f, 1f, 0.75f);
		}

		// Token: 0x06000241 RID: 577 RVA: 0x0001028C File Offset: 0x0000E48C
		private void ShowSkipCinematicHintText()
		{
			if (this._missionHintLogic.ActiveHint != null)
			{
				this._missionHintLogic.Clear();
			}
			MissionHint missionHint = MissionHint.CreateWithKeyAndAction(this.SkipHintText, HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 14));
			this._missionHintLogic.SetActiveHint(missionHint);
		}

		// Token: 0x06000242 RID: 578 RVA: 0x000102D5 File Offset: 0x0000E4D5
		public void OnConversationSetup(Vec3 direction)
		{
			this.OnConversationSetupEvent(direction);
		}

		// Token: 0x04000136 RID: 310
		private const float CinematicTriggerRadius = 3f;

		// Token: 0x04000137 RID: 311
		private const float FadeDuration = 0.75f;

		// Token: 0x04000138 RID: 312
		private const float BlackScreenDuration = 0.25f;

		// Token: 0x04000139 RID: 313
		private const float FirstCameraDuration = 10f;

		// Token: 0x0400013A RID: 314
		private const int SkipHotKey = 14;

		// Token: 0x0400013B RID: 315
		private bool _isMissionInitialized;

		// Token: 0x0400013C RID: 316
		private List<GameEntity> _entities = new List<GameEntity>();

		// Token: 0x0400013D RID: 317
		private GameEntity _currentCameraEntity;

		// Token: 0x0400013E RID: 318
		private GameEntity _cameraEntity;

		// Token: 0x0400013F RID: 319
		private GameEntity _cameraEntity2;

		// Token: 0x04000140 RID: 320
		private GameEntity _cinematicTriggerZone;

		// Token: 0x04000141 RID: 321
		private NavalStorylineAlleyFightCinematicController.NavalAlleyFightCinematicState _currentCinematicState;

		// Token: 0x04000142 RID: 322
		private float _cinematicTimer;

		// Token: 0x04000143 RID: 323
		private NavalStorylineAlleyFightMissionController _missionController;

		// Token: 0x04000144 RID: 324
		private MissionHintLogic _missionHintLogic;

		// Token: 0x04000145 RID: 325
		private List<NavalStorylineAlleyFightCinematicController.ConversationLine> _allLines;

		// Token: 0x04000146 RID: 326
		private CharacterObject _enemyCharacterObject;

		// Token: 0x0400014A RID: 330
		private bool _isPostFightConversationQueued;

		// Token: 0x0400014B RID: 331
		private float _postFightDialogueFadeTimer;

		// Token: 0x0400014C RID: 332
		private bool _isConversationSetup;

		// Token: 0x0400014D RID: 333
		private const float PostFightDialogueFadeOutDuration = 0.75f;

		// Token: 0x0400014E RID: 334
		private const float PostFightDialogueBlackDuration = 1f;

		// Token: 0x0400014F RID: 335
		private const float PostFightDialogueFadeInDuration = 0.75f;

		// Token: 0x0200019D RID: 413
		public enum NavalAlleyFightCinematicState
		{
			// Token: 0x04000C78 RID: 3192
			Ready,
			// Token: 0x04000C79 RID: 3193
			InitialFadeOut,
			// Token: 0x04000C7A RID: 3194
			BlackScreen,
			// Token: 0x04000C7B RID: 3195
			InitialFadeIn,
			// Token: 0x04000C7C RID: 3196
			FirstCamera,
			// Token: 0x04000C7D RID: 3197
			FinalCamera,
			// Token: 0x04000C7E RID: 3198
			Completed
		}

		// Token: 0x0200019E RID: 414
		private class ConversationLine
		{
			// Token: 0x0600194F RID: 6479 RVA: 0x000AD8C1 File Offset: 0x000ABAC1
			public ConversationLine(TextObject line, CharacterObject speaker)
			{
				this.Line = line;
				this.Speaker = speaker;
			}

			// Token: 0x04000C7F RID: 3199
			public TextObject Line;

			// Token: 0x04000C80 RID: 3200
			public CharacterObject Speaker;

			// Token: 0x04000C81 RID: 3201
			public MBInformationManager.DialogNotificationHandle Handle;
		}
	}
}
