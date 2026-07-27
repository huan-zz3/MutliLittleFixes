using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NavalDLC.Missions;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.Objects.UsableMachines;
using NavalDLC.Storyline;
using NavalDLC.View.MissionViews;
using SandBox.Objects.AreaMarkers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Hints;
using TaleWorlds.MountAndBlade.Missions.MissionLogics;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace NavalDLC.GauntletUI.MissionViews
{
	// Token: 0x02000014 RID: 20
	[OverrideView(typeof(FloatingFortressView))]
	public class MissionGauntletFloatingFortressView : FloatingFortressView
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000072 RID: 114 RVA: 0x00005664 File Offset: 0x00003864
		// (set) Token: 0x06000073 RID: 115 RVA: 0x0000566C File Offset: 0x0000386C
		public bool AreMarkersDirty { get; private set; }

		// Token: 0x06000074 RID: 116 RVA: 0x00005678 File Offset: 0x00003878
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			if (!this._isInitialized)
			{
				this.InitializeView();
				this._fadeOutReason = MissionGauntletFloatingFortressView.FadeOutReason.Initialize;
				this._canInvokeFadeOutEvent = true;
				ScreenFadeController.BeginFadeOut(0f);
				this._isInitialized = true;
			}
			if (!Mission.Current.Scene.IsLoadingFinished())
			{
				return;
			}
			Agent main = Agent.Main;
			MissionShip missionShip = ((main != null) ? main.GetComponent<AgentNavalComponent>().FormationShip : null);
			bool flag;
			if (!this._hasUsedBallista)
			{
				if (missionShip == null)
				{
					flag = false;
				}
				else
				{
					RangedSiegeWeapon shipSiegeWeapon = missionShip.ShipSiegeWeapon;
					bool? flag2 = ((shipSiegeWeapon != null) ? new bool?(shipSiegeWeapon.PlayerForceUse) : null);
					bool flag3 = true;
					flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
			}
			else
			{
				flag = true;
			}
			this._hasUsedBallista = flag;
			if (this._isShowingBallistaHint && this._hintLogic.ActiveHint != null && (this._hasUsedBallista || this._navalShipsLogic.PlayerControlledShip == null))
			{
				this._isShowingBallistaHint = false;
				this._hintLogic.Clear();
			}
			if (this._initialFadeOutWaitTime > 0f)
			{
				this._initialFadeOutWaitTime -= dt;
				return;
			}
			if (this._controller.IsPhaseOneCompleted && !this._isPhaseOneCompleted)
			{
				this._isPhaseOneCompleted = true;
				this.OnPhaseOneCompleted();
			}
			if (this._willFadeOutForPhaseOneCompletion)
			{
				this._remainingTimeForPhaseOneFadeOut -= dt;
				if (this._remainingTimeForPhaseOneFadeOut <= 0f)
				{
					this._fadeOutReason = MissionGauntletFloatingFortressView.FadeOutReason.PhaseOneCompleted;
					ScreenFadeController.BeginFadeOutAndIn(0.1f, 0.75f, 0.75f);
					this._canInvokeFadeOutEvent = true;
					this._willFadeOutForPhaseOneCompletion = false;
				}
			}
			foreach (MissionShip missionShip2 in this._controller.EnemyShipsOrdered)
			{
				if (missionShip2.ShipSiegeWeapon != null)
				{
					RangedSiegeWeapon shipSiegeWeapon2 = missionShip2.ShipSiegeWeapon;
					if (!shipSiegeWeapon2.IsDestroyed && !this._markerByBallista.ContainsKey(shipSiegeWeapon2.DestructionComponent))
					{
						shipSiegeWeapon2.DestructionComponent.OnDestroyed += new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.OnBallistaDestroyed);
						GameEntity gameEntity = GameEntity.CreateEmpty(base.Mission.Scene, true, true, true);
						gameEntity.WeakEntity.SetGlobalPosition(shipSiegeWeapon2.GameEntity.GlobalPosition);
						AnimatedBasicAreaIndicator animatedBasicAreaIndicator = MissionGauntletFloatingFortressView.AddMarker(gameEntity.WeakEntity, new TextObject("{=cn28TEkM}Target", null), "quest", 1.5f);
						this._markerByBallista.Add(shipSiegeWeapon2.DestructionComponent, animatedBasicAreaIndicator);
						this.AreMarkersDirty = true;
					}
				}
			}
			foreach (KeyValuePair<DestructableComponent, AnimatedBasicAreaIndicator> keyValuePair in this._markerByBallista)
			{
				keyValuePair.Value.GameEntity.SetGlobalPosition(keyValuePair.Key.GameEntity.GlobalPosition);
			}
			if (ScreenFadeController.IsFadedOut && this._canInvokeFadeOutEvent)
			{
				if (this._controller.IsStartedFromCheckpoint)
				{
					this._fadeOutReason = MissionGauntletFloatingFortressView.FadeOutReason.PhaseOneCompleted;
					ScreenFadeController.BeginFadeIn(1f);
				}
				if (this._fadeOutReason == MissionGauntletFloatingFortressView.FadeOutReason.Initialize)
				{
					this._cinematicCamera = Camera.CreateCamera();
					this._cinematicCamera.SetFovHorizontal(base.MissionScreen.CombatCamera.HorizontalFov, base.MissionScreen.CombatCamera.GetAspectRatio(), base.MissionScreen.CombatCamera.Near, base.MissionScreen.CombatCamera.Far);
					this._cinematicCamera.Frame = base.MissionScreen.CombatCamera.Frame;
					base.MissionScreen.CustomCamera = this._cinematicCamera;
					ScreenFadeController.BeginFadeIn(1f);
					this._shouldTickCinematic = true;
					MissionHint missionHint = MissionHint.CreateWithKeyAndAction(new TextObject("{=FiSENWMB}Skip Cinematic", null), HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 14));
					this._hintLogic.SetActiveHint(missionHint);
					this._missionMainAgentController.Disable();
					this._suspendedFeatures = this._shipControlView.SuspendedFeatures;
					this._shipControlView.SuspendFeature(~this._suspendedFeatures);
				}
				else if (this._fadeOutReason == MissionGauntletFloatingFortressView.FadeOutReason.BallistaCinematicEnded)
				{
					base.MissionScreen.CustomCamera = null;
					Camera cinematicCamera = this._cinematicCamera;
					if (cinematicCamera != null)
					{
						cinematicCamera.ReleaseCamera();
					}
					this._cinematicCamera = null;
					this._shouldTickCinematic = false;
					if (!this._controller.IsPhaseOneCompleted && !this._controller.IsStartedFromCheckpoint && !this._isShowingBallistaHint && !this._hasUsedBallista && missionShip != null)
					{
						if (Agent.Main != null)
						{
							ShipControllerMachine shipControllerMachine = missionShip.ShipControllerMachine;
							Agent.Main.UseGameObject(shipControllerMachine.PilotStandingPoint, -1);
						}
						this._missionMainAgentController.Enable();
						this._shipControlView.ResumeFeature(~this._suspendedFeatures);
						this._hintLogic.Clear();
						this._isShowingBallistaHint = true;
						MissionHint missionHint2 = MissionHint.CreateWithKeyAndAction(new TextObject("{=aTEkCItM}Control Ballista", null), HotKeyManager.GetHotKeyId("NavalShipControlsHotKeyCategory", 115));
						this._hintLogic.SetActiveHint(missionHint2);
					}
				}
				this._controller.OnViewFadeOut((int)this._fadeOutReason);
				this._canInvokeFadeOutEvent = false;
			}
			if (!ScreenFadeController.IsFadeActive && !this._canInvokeFadeOutEvent)
			{
				this._canInvokeFadeOutEvent = true;
			}
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00005BAC File Offset: 0x00003DAC
		private void OnBallistaDestroyed(DestructableComponent target, Agent attackerAgent, in MissionWeapon weapon, ScriptComponentBehavior attackerScriptComponentBehavior, int inflictedDamage)
		{
			AnimatedBasicAreaIndicator animatedBasicAreaIndicator;
			if (this._markerByBallista.TryGetValue(target, out animatedBasicAreaIndicator))
			{
				animatedBasicAreaIndicator.SetIsActive(false);
				this.AreMarkersDirty = true;
			}
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00005BD8 File Offset: 0x00003DD8
		public override void OnFixedMissionTick(float fixedDt)
		{
			if (this._shouldTickCinematic && !Game.Current.GameStateManager.ActiveStateDisabledByUser)
			{
				this._cinematicElapsedTime += fixedDt;
				this._cinematicCamera.Frame = this._cinematicCameraTrack.Evaluate(this._cinematicElapsedTime);
				this._cinematicEventTrack.Evaluate(this._cinematicElapsedTime);
				if ((Mission.Current.InputManager.IsGameKeyDown(14) && this._cinematicElapsedTime >= 2.5f) || (this._cinematicCameraTrack.IsCompleted(this._cinematicElapsedTime) && this._cinematicEventTrack.IsCompleted(this._cinematicElapsedTime)))
				{
					this._shouldTickCinematic = false;
					this._hintLogic.Clear();
					CampaignInformationManager.ClearAllDialogNotifications(true);
					this._fadeOutReason = MissionGauntletFloatingFortressView.FadeOutReason.BallistaCinematicEnded;
					ScreenFadeController.BeginFadeOutAndIn(0.5f, 0.5f, 0.5f);
					this._canInvokeFadeOutEvent = true;
				}
			}
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00005CC8 File Offset: 0x00003EC8
		private void InitializeView()
		{
			this._controller = base.Mission.GetMissionBehavior<FloatingFortressSetPieceBattleMissionController>();
			this._hintLogic = base.Mission.GetMissionBehavior<MissionHintLogic>();
			this._navalShipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
			this._missionMainAgentController = base.Mission.GetMissionBehavior<MissionMainAgentController>();
			this._shipControlView = base.Mission.GetMissionBehavior<MissionGauntletShipControlView>();
			this.InitializeCinematicKeyframes();
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00005D30 File Offset: 0x00003F30
		private void InitializeCinematicKeyframes()
		{
			MatrixFrame globalFrame = base.Mission.Scene.FindEntityWithTag("sp_camera_initial").GetGlobalFrame();
			MatrixFrame globalFrame2 = base.Mission.Scene.FindEntityWithTag("sp_camera_1").GetGlobalFrame();
			MatrixFrame globalFrame3 = base.Mission.Scene.FindEntityWithTag("sp_camera_1a").GetGlobalFrame();
			MatrixFrame globalFrame4 = base.Mission.Scene.FindEntityWithTag("sp_camera_2").GetGlobalFrame();
			MatrixFrame globalFrame5 = base.Mission.Scene.FindEntityWithTag("sp_camera_2a").GetGlobalFrame();
			MatrixFrame globalFrame6 = base.Mission.Scene.FindEntityWithTag("sp_camera_3").GetGlobalFrame();
			MatrixFrame globalFrame7 = base.Mission.Scene.FindEntityWithTag("sp_camera_3a").GetGlobalFrame();
			MatrixFrame globalFrame8 = base.Mission.Scene.FindEntityWithTag("sp_camera_4").GetGlobalFrame();
			MatrixFrame globalFrame9 = base.Mission.Scene.FindEntityWithTag("sp_camera_4a").GetGlobalFrame();
			MatrixFrame globalFrame10 = base.Mission.Scene.FindEntityWithTag("sp_camera_5").GetGlobalFrame();
			MatrixFrame globalFrame11 = base.Mission.Scene.FindEntityWithTag("sp_camera_5a").GetGlobalFrame();
			TextObject dialogueText1 = new TextObject("{=VUWTon9z}Have a good look at Crusas's floating fortress before we attack. It's formidable, but it's not going anywhere.", null);
			TextObject dialogueText2 = new TextObject("{=0JjVa9p9}He has no less than eight ships lashed together. They mount four heavy mangonels - big ones. Most ships would tip over from the recoil if they weren't chained to each other.", null);
			TextObject dialogueText3 = new TextObject("{=4Bhb39KH}One is on the roundship, which is the fortress's keep, as it were.", null);
			TextObject dialogueText4 = new TextObject("{=MTJMs4A7}Another three are on cogs - one is to the northwest.", null);
			TextObject dialogueText5 = new TextObject("{=ObjIiR2M}The others are to the northeast and southeast.", null);
			TextObject dialogueText6 = new TextObject("{=mVa3D9xf}You must steer the Wasp to take out those mangonels. You need direct hits - but don’t get too close, as their decks are packed with archers. ", null);
			TextObject dialogueText7 = new TextObject("{=afb9bd35}Also, keep moving. One or two hits could shatter our timbers or set us alight and make an end of us.", null);
			TextObject dialogueText8 = new TextObject("{=NIlRAHPb}We're right behind you, brother. Let's take this vile toad of a merchant down!", null);
			this._cinematicCameraTrack = new MissionGauntletFloatingFortressView.MatrixFrameTrack();
			this._cinematicEventTrack = new MissionGauntletFloatingFortressView.EventTrack();
			float num = 0f;
			this._cinematicCameraTrack.AddKeyframe(new MissionGauntletFloatingFortressView.MatrixFrameKeyFrame(num, globalFrame));
			this._cinematicEventTrack.AddKeyframe(new MissionGauntletFloatingFortressView.EventKeyframe(num, delegate
			{
				CampaignInformationManager.ClearAllDialogNotifications(true);
				CampaignInformationManager.AddDialogLine(dialogueText1, NavalStorylineData.Bjolgur.CharacterObject, null, 0, 2);
			}));
			num += 10f;
			this._cinematicEventTrack.AddKeyframe(new MissionGauntletFloatingFortressView.EventKeyframe(num, delegate
			{
				CampaignInformationManager.ClearAllDialogNotifications(true);
				CampaignInformationManager.AddDialogLine(dialogueText2, NavalStorylineData.Bjolgur.CharacterObject, null, 0, 2);
			}));
			num += 15f;
			this._cinematicCameraTrack.AddKeyframe(new MissionGauntletFloatingFortressView.MatrixFrameKeyFrame(num, globalFrame2));
			this._cinematicEventTrack.AddKeyframe(new MissionGauntletFloatingFortressView.EventKeyframe(num, delegate
			{
				CampaignInformationManager.ClearAllDialogNotifications(true);
				CampaignInformationManager.AddDialogLine(dialogueText3, NavalStorylineData.Bjolgur.CharacterObject, null, 0, 2);
			}));
			num += 6f;
			this._cinematicCameraTrack.AddKeyframe(new MissionGauntletFloatingFortressView.MatrixFrameKeyFrame(num, globalFrame3));
			this._cinematicEventTrack.AddKeyframe(new MissionGauntletFloatingFortressView.EventKeyframe(num, delegate
			{
				CampaignInformationManager.ClearAllDialogNotifications(true);
				CampaignInformationManager.AddDialogLine(dialogueText4, NavalStorylineData.Bjolgur.CharacterObject, null, 0, 2);
			}));
			num += 0.5f;
			this._cinematicCameraTrack.AddKeyframe(new MissionGauntletFloatingFortressView.MatrixFrameKeyFrame(num, globalFrame4));
			num += 5.5f;
			this._cinematicCameraTrack.AddKeyframe(new MissionGauntletFloatingFortressView.MatrixFrameKeyFrame(num, globalFrame5));
			this._cinematicEventTrack.AddKeyframe(new MissionGauntletFloatingFortressView.EventKeyframe(num, delegate
			{
				CampaignInformationManager.ClearAllDialogNotifications(true);
				CampaignInformationManager.AddDialogLine(dialogueText5, NavalStorylineData.Bjolgur.CharacterObject, null, 0, 2);
			}));
			num += 0.5f;
			this._cinematicCameraTrack.AddKeyframe(new MissionGauntletFloatingFortressView.MatrixFrameKeyFrame(num, globalFrame6));
			num += 1.5f;
			this._cinematicCameraTrack.AddKeyframe(new MissionGauntletFloatingFortressView.MatrixFrameKeyFrame(num, globalFrame7));
			num += 0.5f;
			this._cinematicCameraTrack.AddKeyframe(new MissionGauntletFloatingFortressView.MatrixFrameKeyFrame(num, globalFrame8));
			num += 1.5f;
			this._cinematicCameraTrack.AddKeyframe(new MissionGauntletFloatingFortressView.MatrixFrameKeyFrame(num, globalFrame9));
			this._cinematicEventTrack.AddKeyframe(new MissionGauntletFloatingFortressView.EventKeyframe(num, delegate
			{
				CampaignInformationManager.ClearAllDialogNotifications(true);
				CampaignInformationManager.AddDialogLine(dialogueText6, NavalStorylineData.Bjolgur.CharacterObject, null, 0, 2);
			}));
			num += 6f;
			this._cinematicCameraTrack.AddKeyframe(new MissionGauntletFloatingFortressView.MatrixFrameKeyFrame(num, globalFrame10));
			num += 6f;
			this._cinematicEventTrack.AddKeyframe(new MissionGauntletFloatingFortressView.EventKeyframe(num, delegate
			{
				CampaignInformationManager.ClearAllDialogNotifications(true);
				CampaignInformationManager.AddDialogLine(dialogueText7, NavalStorylineData.Bjolgur.CharacterObject, null, 0, 2);
			}));
			num += 8f;
			this._cinematicEventTrack.AddKeyframe(new MissionGauntletFloatingFortressView.EventKeyframe(num, delegate
			{
				CampaignInformationManager.ClearAllDialogNotifications(true);
				CampaignInformationManager.AddDialogLine(dialogueText8, NavalStorylineData.Bjolgur.CharacterObject, null, 0, 2);
			}));
			num += 7f;
			this._cinematicCameraTrack.AddKeyframe(new MissionGauntletFloatingFortressView.MatrixFrameKeyFrame(num, globalFrame11));
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00006164 File Offset: 0x00004364
		private void OnPhaseOneCompleted()
		{
			if (this._controller.IsStartedFromCheckpoint)
			{
				ScreenFadeController.BeginFadeIn(0.75f);
				return;
			}
			this._willFadeOutForPhaseOneCompletion = true;
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00006185 File Offset: 0x00004385
		private static AnimatedBasicAreaIndicator AddMarker(WeakGameEntity gameEntity, TextObject name, string type, float radius = 5f)
		{
			gameEntity.CreateAndAddScriptComponent("AnimatedBasicAreaIndicator", true);
			AnimatedBasicAreaIndicator firstScriptOfType = gameEntity.GetFirstScriptOfType<AnimatedBasicAreaIndicator>();
			firstScriptOfType.AreaRadius = radius;
			firstScriptOfType.Type = type;
			firstScriptOfType.SetOverriddenName(name);
			return firstScriptOfType;
		}

		// Token: 0x0400002B RID: 43
		private const float EarliestSkipTime = 2.5f;

		// Token: 0x0400002C RID: 44
		private const float FadeOutTransitionTime = 1.5f;

		// Token: 0x0400002E RID: 46
		private readonly Dictionary<DestructableComponent, AnimatedBasicAreaIndicator> _markerByBallista = new Dictionary<DestructableComponent, AnimatedBasicAreaIndicator>();

		// Token: 0x0400002F RID: 47
		private bool _canInvokeFadeOutEvent = true;

		// Token: 0x04000030 RID: 48
		private MissionGauntletFloatingFortressView.FadeOutReason _fadeOutReason;

		// Token: 0x04000031 RID: 49
		private float _initialFadeOutWaitTime = 2f;

		// Token: 0x04000032 RID: 50
		private bool _isInitialized;

		// Token: 0x04000033 RID: 51
		private bool _isPhaseOneCompleted;

		// Token: 0x04000034 RID: 52
		private bool _isShowingBallistaHint;

		// Token: 0x04000035 RID: 53
		private bool _hasUsedBallista;

		// Token: 0x04000036 RID: 54
		private bool _willFadeOutForPhaseOneCompletion;

		// Token: 0x04000037 RID: 55
		private float _remainingTimeForPhaseOneFadeOut = 1.5f;

		// Token: 0x04000038 RID: 56
		private Camera _cinematicCamera;

		// Token: 0x04000039 RID: 57
		private bool _shouldTickCinematic;

		// Token: 0x0400003A RID: 58
		private float _cinematicElapsedTime;

		// Token: 0x0400003B RID: 59
		private MissionGauntletFloatingFortressView.MatrixFrameTrack _cinematicCameraTrack;

		// Token: 0x0400003C RID: 60
		private MissionGauntletFloatingFortressView.EventTrack _cinematicEventTrack;

		// Token: 0x0400003D RID: 61
		private FloatingFortressSetPieceBattleMissionController _controller;

		// Token: 0x0400003E RID: 62
		private MissionHintLogic _hintLogic;

		// Token: 0x0400003F RID: 63
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x04000040 RID: 64
		private MissionMainAgentController _missionMainAgentController;

		// Token: 0x04000041 RID: 65
		private MissionGauntletShipControlView _shipControlView;

		// Token: 0x04000042 RID: 66
		private MissionGauntletShipControlView.ShipControlFeatureFlags _suspendedFeatures;

		// Token: 0x0200002C RID: 44
		private abstract class Keyframe<T>
		{
			// Token: 0x17000004 RID: 4
			// (get) Token: 0x0600011C RID: 284 RVA: 0x0000A419 File Offset: 0x00008619
			// (set) Token: 0x0600011D RID: 285 RVA: 0x0000A421 File Offset: 0x00008621
			public float Time { get; set; }

			// Token: 0x17000005 RID: 5
			// (get) Token: 0x0600011E RID: 286 RVA: 0x0000A42A File Offset: 0x0000862A
			// (set) Token: 0x0600011F RID: 287 RVA: 0x0000A432 File Offset: 0x00008632
			public T Value { get; set; }

			// Token: 0x06000120 RID: 288 RVA: 0x0000A43B File Offset: 0x0000863B
			public Keyframe(float time, T value)
			{
				this.Time = time;
				this.Value = value;
			}
		}

		// Token: 0x0200002D RID: 45
		private abstract class Track<TKeyframe, TValue> where TKeyframe : MissionGauntletFloatingFortressView.Keyframe<TValue>
		{
			// Token: 0x06000121 RID: 289 RVA: 0x0000A451 File Offset: 0x00008651
			public void AddKeyframe(TKeyframe keyframe)
			{
				this.Keyframes.Add(keyframe);
				this.Keyframes.Sort((TKeyframe a, TKeyframe b) => a.Time.CompareTo(b.Time));
			}

			// Token: 0x06000122 RID: 290 RVA: 0x0000A489 File Offset: 0x00008689
			public void RemoveKeyframe(TKeyframe keyframe)
			{
				this.Keyframes.Remove(keyframe);
			}

			// Token: 0x06000123 RID: 291 RVA: 0x0000A498 File Offset: 0x00008698
			public void ClearKeyframes()
			{
				this.Keyframes.Clear();
				this._lastKeyframeIndex = 0;
			}

			// Token: 0x06000124 RID: 292 RVA: 0x0000A4AC File Offset: 0x000086AC
			public bool IsCompleted(float time)
			{
				return this.Keyframes.Count == 0 || this.Keyframes.Last<TKeyframe>().Time <= time;
			}

			// Token: 0x06000125 RID: 293
			public abstract TValue Evaluate(float time);

			// Token: 0x06000126 RID: 294 RVA: 0x0000A4D8 File Offset: 0x000086D8
			[return: TupleElementNames(new string[] { "prev", "next", "t" })]
			protected ValueTuple<TKeyframe, TKeyframe, float> GetKeyframesAtTime(float time)
			{
				if (this.Keyframes.Count == 0)
				{
					return new ValueTuple<TKeyframe, TKeyframe, float>(default(TKeyframe), default(TKeyframe), 0f);
				}
				if (time <= this.Keyframes[0].Time)
				{
					return new ValueTuple<TKeyframe, TKeyframe, float>(this.Keyframes[0], this.Keyframes[0], 0f);
				}
				if (time >= this.Keyframes[this.Keyframes.Count - 1].Time)
				{
					return new ValueTuple<TKeyframe, TKeyframe, float>(this.Keyframes[this.Keyframes.Count - 1], this.Keyframes[this.Keyframes.Count - 1], 1f);
				}
				int num = Math.Max(0, Math.Min(this._lastKeyframeIndex, this.Keyframes.Count - 2));
				if (this.Keyframes[num].Time > time)
				{
					for (int i = num; i >= 0; i--)
					{
						if (this.Keyframes[i].Time <= time && this.Keyframes[i + 1].Time > time)
						{
							this._lastKeyframeIndex = i;
							float num2 = (time - this.Keyframes[i].Time) / (this.Keyframes[i + 1].Time - this.Keyframes[i].Time);
							return new ValueTuple<TKeyframe, TKeyframe, float>(this.Keyframes[i], this.Keyframes[i + 1], num2);
						}
					}
				}
				else
				{
					for (int j = num; j < this.Keyframes.Count - 1; j++)
					{
						if (this.Keyframes[j].Time <= time && this.Keyframes[j + 1].Time > time)
						{
							this._lastKeyframeIndex = j;
							float num3 = (time - this.Keyframes[j].Time) / (this.Keyframes[j + 1].Time - this.Keyframes[j].Time);
							return new ValueTuple<TKeyframe, TKeyframe, float>(this.Keyframes[j], this.Keyframes[j + 1], num3);
						}
					}
				}
				return new ValueTuple<TKeyframe, TKeyframe, float>(this.Keyframes[0], this.Keyframes[0], 0f);
			}

			// Token: 0x040000B7 RID: 183
			protected readonly List<TKeyframe> Keyframes = new List<TKeyframe>();

			// Token: 0x040000B8 RID: 184
			private int _lastKeyframeIndex;
		}

		// Token: 0x0200002E RID: 46
		private class MatrixFrameKeyFrame : MissionGauntletFloatingFortressView.Keyframe<MatrixFrame>
		{
			// Token: 0x06000128 RID: 296 RVA: 0x0000A7AE File Offset: 0x000089AE
			public MatrixFrameKeyFrame(float time, MatrixFrame value)
				: base(time, value)
			{
			}
		}

		// Token: 0x0200002F RID: 47
		private class MatrixFrameTrack : MissionGauntletFloatingFortressView.Track<MissionGauntletFloatingFortressView.MatrixFrameKeyFrame, MatrixFrame>
		{
			// Token: 0x06000129 RID: 297 RVA: 0x0000A7B8 File Offset: 0x000089B8
			public override MatrixFrame Evaluate(float time)
			{
				ValueTuple<MissionGauntletFloatingFortressView.MatrixFrameKeyFrame, MissionGauntletFloatingFortressView.MatrixFrameKeyFrame, float> keyframesAtTime = base.GetKeyframesAtTime(time);
				MissionGauntletFloatingFortressView.MatrixFrameKeyFrame item = keyframesAtTime.Item1;
				MissionGauntletFloatingFortressView.MatrixFrameKeyFrame item2 = keyframesAtTime.Item2;
				float item3 = keyframesAtTime.Item3;
				if (item == null || item2 == null)
				{
					return MatrixFrame.Zero;
				}
				if (item == item2)
				{
					return item.Value;
				}
				MatrixFrame value = item.Value;
				MatrixFrame value2 = item2.Value;
				return MatrixFrame.Lerp(ref value, ref value2, item3 * item3 * (3f - 2f * item3));
			}
		}

		// Token: 0x02000030 RID: 48
		private class EventKeyframe : MissionGauntletFloatingFortressView.Keyframe<Action>
		{
			// Token: 0x0600012B RID: 299 RVA: 0x0000A828 File Offset: 0x00008A28
			public EventKeyframe(float time, Action value)
				: base(time, value)
			{
			}
		}

		// Token: 0x02000031 RID: 49
		private class EventTrack : MissionGauntletFloatingFortressView.Track<MissionGauntletFloatingFortressView.EventKeyframe, Action>
		{
			// Token: 0x0600012C RID: 300 RVA: 0x0000A834 File Offset: 0x00008A34
			public override Action Evaluate(float time)
			{
				if (time < this._lastEvaluatedTime)
				{
					this._triggeredEvents.RemoveWhere((MissionGauntletFloatingFortressView.EventKeyframe e) => e.Time > time);
				}
				this._lastEvaluatedTime = time;
				foreach (MissionGauntletFloatingFortressView.EventKeyframe eventKeyframe in this.Keyframes)
				{
					if (eventKeyframe.Time <= time && this._triggeredEvents.Add(eventKeyframe))
					{
						Action value = eventKeyframe.Value;
						if (value != null)
						{
							value();
						}
					}
				}
				return null;
			}

			// Token: 0x040000B9 RID: 185
			private readonly HashSet<MissionGauntletFloatingFortressView.EventKeyframe> _triggeredEvents = new HashSet<MissionGauntletFloatingFortressView.EventKeyframe>();

			// Token: 0x040000BA RID: 186
			private float _lastEvaluatedTime = -0f;
		}

		// Token: 0x02000032 RID: 50
		private enum FadeOutReason
		{
			// Token: 0x040000BC RID: 188
			Initialize,
			// Token: 0x040000BD RID: 189
			BallistaCinematicEnded,
			// Token: 0x040000BE RID: 190
			PhaseOneCompleted
		}
	}
}
