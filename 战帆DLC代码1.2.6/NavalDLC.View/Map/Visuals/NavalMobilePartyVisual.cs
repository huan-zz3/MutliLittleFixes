using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Helpers;
using NavalDLC.Missions.Objects;
using NavalDLC.View.Map.Managers;
using SandBox;
using SandBox.View.Map;
using SandBox.View.Map.Visuals;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;

namespace NavalDLC.View.Map.Visuals
{
	// Token: 0x02000034 RID: 52
	public class NavalMobilePartyVisual : MapEntityVisual<PartyBase>
	{
		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000155 RID: 341 RVA: 0x000096CC File Offset: 0x000078CC
		private Scene MapScene
		{
			get
			{
				if (this._mapScene == null && Campaign.Current != null && Campaign.Current.MapSceneWrapper != null)
				{
					this._mapScene = ((MapScene)Campaign.Current.MapSceneWrapper).Scene;
				}
				return this._mapScene;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000156 RID: 342 RVA: 0x0000971A File Offset: 0x0000791A
		public override float BearingRotation
		{
			get
			{
				return this._bearingRotation;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000157 RID: 343 RVA: 0x00009722 File Offset: 0x00007922
		public override MapEntityVisual AttachedTo
		{
			get
			{
				MobileParty mobileParty = base.MapEntity.MobileParty;
				if (((mobileParty != null) ? mobileParty.AttachedTo : null) != null)
				{
					return NavalMobilePartyVisualManager.Current.GetVisualOfEntity(base.MapEntity.MobileParty.AttachedTo.Party);
				}
				return null;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000158 RID: 344 RVA: 0x0000975E File Offset: 0x0000795E
		public override CampaignVec2 InteractionPositionForPlayer
		{
			get
			{
				return base.MapEntity.GetInteractionPosition(MobileParty.MainParty);
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000159 RID: 345 RVA: 0x00009770 File Offset: 0x00007970
		public override bool IsMobileEntity
		{
			get
			{
				return base.MapEntity.IsMobile;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600015A RID: 346 RVA: 0x0000977D File Offset: 0x0000797D
		public override bool IsMainEntity
		{
			get
			{
				return base.MapEntity == PartyBase.MainParty;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600015B RID: 347 RVA: 0x0000978C File Offset: 0x0000798C
		// (set) Token: 0x0600015C RID: 348 RVA: 0x00009794 File Offset: 0x00007994
		public GameEntity StrategicEntity { get; private set; }

		// Token: 0x0600015D RID: 349 RVA: 0x000097A0 File Offset: 0x000079A0
		public NavalMobilePartyVisual(PartyBase partyBase)
			: base(partyBase)
		{
			this.CircleLocalFrame = MatrixFrame.Identity;
		}

		// Token: 0x0600015E RID: 350 RVA: 0x00009819 File Offset: 0x00007A19
		public override bool IsEnemyOf(IFaction faction)
		{
			return FactionManager.IsAtWarAgainstFaction(base.MapEntity.MapFaction, faction.MapFaction);
		}

		// Token: 0x0600015F RID: 351 RVA: 0x00009831 File Offset: 0x00007A31
		public override bool IsInSameFaction(IFaction faction)
		{
			return DiplomacyHelper.IsSameFactionAndNotEliminated(base.MapEntity.MapFaction, faction.MapFaction);
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00009849 File Offset: 0x00007A49
		public override bool IsAllyOf(IFaction faction)
		{
			return DiplomacyHelper.HasAllianceWithFaction(base.MapEntity.MapFaction, faction.MapFaction);
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00009861 File Offset: 0x00007A61
		internal void OnPartyRemoved()
		{
			if (this.StrategicEntity != null)
			{
				this.RemoveVisualFromVisualsOfEntities();
				this.ReleaseResources();
				this.StrategicEntity.Remove(111);
				this._isVisualInRaftState = false;
			}
		}

		// Token: 0x06000162 RID: 354 RVA: 0x00009894 File Offset: 0x00007A94
		public override void OnTrackAction()
		{
			MobileParty mobileParty = base.MapEntity.MobileParty;
			if (mobileParty != null)
			{
				if (Campaign.Current.VisualTrackerManager.CheckTracked(mobileParty))
				{
					Campaign.Current.VisualTrackerManager.RemoveTrackedObject(mobileParty, false);
					return;
				}
				Campaign.Current.VisualTrackerManager.RegisterObject(mobileParty);
			}
		}

		// Token: 0x06000163 RID: 355 RVA: 0x000098E4 File Offset: 0x00007AE4
		public override bool OnMapClick(bool followModifierUsed)
		{
			MobileParty.NavigationType navigationType;
			if (this.IsMainEntity)
			{
				MobileParty.MainParty.SetMoveModeHold();
			}
			else if (base.MapEntity.MobileParty.IsCurrentlyAtSea == MobileParty.MainParty.IsCurrentlyAtSea && NavigationHelper.CanPlayerNavigateToPosition(base.MapEntity.MobileParty.Position, ref navigationType))
			{
				if (followModifierUsed)
				{
					MobileParty.MainParty.SetMoveEscortParty(base.MapEntity.MobileParty, navigationType, false);
				}
				else
				{
					MobileParty.MainParty.SetMoveEngageParty(base.MapEntity.MobileParty, navigationType);
				}
			}
			return true;
		}

		// Token: 0x06000164 RID: 356 RVA: 0x00009970 File Offset: 0x00007B70
		public override void OnHover()
		{
			if (base.MapEntity.MapEvent != null)
			{
				InformationManager.ShowTooltip(typeof(MapEvent), new object[] { base.MapEntity.MapEvent });
				return;
			}
			if (base.MapEntity.IsMobile && base.MapEntity.IsVisible)
			{
				if (base.MapEntity.MobileParty.Army != null && base.MapEntity.MobileParty.Army.DoesLeaderPartyAndAttachedPartiesContain(base.MapEntity.MobileParty))
				{
					if (base.MapEntity.MobileParty.Army.LeaderParty.SiegeEvent != null)
					{
						InformationManager.ShowTooltip(typeof(SiegeEvent), new object[] { base.MapEntity.MobileParty.Army.LeaderParty.SiegeEvent });
						return;
					}
					InformationManager.ShowTooltip(typeof(Army), new object[]
					{
						base.MapEntity.MobileParty.Army,
						false,
						true
					});
					return;
				}
				else
				{
					if (base.MapEntity.MobileParty.SiegeEvent != null)
					{
						InformationManager.ShowTooltip(typeof(SiegeEvent), new object[] { base.MapEntity.MobileParty.SiegeEvent });
						return;
					}
					InformationManager.ShowTooltip(typeof(MobileParty), new object[]
					{
						base.MapEntity.MobileParty,
						false,
						true
					});
				}
			}
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00009B04 File Offset: 0x00007D04
		public override Vec3 GetVisualPosition()
		{
			return base.MapEntity.MobileParty.VisualPosition2DWithoutError.ToVec3(base.MapEntity.Position.AsVec3().Z);
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00009B44 File Offset: 0x00007D44
		public override void ReleaseResources()
		{
			this.ResetPartyIcon();
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00009B4C File Offset: 0x00007D4C
		public override bool IsVisibleOrFadingOut()
		{
			return this._entityAlpha > 0f;
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00009B5C File Offset: 0x00007D5C
		public override void OnOpenEncyclopedia()
		{
			if (base.MapEntity.MobileParty.IsLordParty && base.MapEntity.MobileParty.LeaderHero != null)
			{
				Campaign.Current.EncyclopediaManager.GoToLink(base.MapEntity.MobileParty.LeaderHero.EncyclopediaLink);
			}
		}

		// Token: 0x06000169 RID: 361 RVA: 0x00009BB4 File Offset: 0x00007DB4
		internal void Tick(float dt, float realDt, ref int dirtyPartiesCount, ref NavalMobilePartyVisual[] dirtyPartiesList)
		{
			if (this.StrategicEntity == null)
			{
				return;
			}
			if (base.MapEntity.MobileParty.IsNavalVisualDirty && (this._entityAlpha > 0f || base.MapEntity.IsVisible))
			{
				int num = Interlocked.Increment(ref dirtyPartiesCount);
				dirtyPartiesList[num] = this;
			}
			if (this.HasNavalVisual())
			{
				if (!base.MapEntity.MobileParty.IsTransitionInProgress)
				{
					if (this.IsVisibleOrFadingOut() && this.StrategicEntity != null)
					{
						this.UpdateEntityPosition(dt, realDt, true);
					}
				}
				else if (this.GetTransitionProgress() <= 1f)
				{
					this.TickTransitionFadeState(dt);
				}
				if (this._raidAgentVisuals != null)
				{
					float num2 = MathF.Min(0.25f, 20f);
					this._raidAgentVisuals.Tick(null, dt, false, num2);
				}
			}
		}

		// Token: 0x0600016A RID: 362 RVA: 0x00009C80 File Offset: 0x00007E80
		internal void UpdateEntityPosition(float dt, float realDt, bool isVisible = false)
		{
			MobileParty mobileParty = base.MapEntity.MobileParty;
			this.UpdateBearingRotation(realDt);
			MatrixFrame identity = MatrixFrame.Identity;
			identity.origin = this.GetVisualPosition();
			MatrixFrame localFrame = this.StrategicEntity.GetLocalFrame();
			Vec2 vec = identity.origin.AsVec2 - localFrame.origin.AsVec2;
			float length = vec.Length;
			float num = ((dt > 0f) ? (length / dt) : 0f);
			if (mobileParty.Army != null && mobileParty.AttachedTo == mobileParty.Army.LeaderParty && (base.MapEntity.MapEvent == null || !base.MapEntity.MapEvent.IsFieldBattle))
			{
				if (num > 20f)
				{
					identity.rotation.RotateAboutUp(this._bearingRotation);
				}
				else if (mobileParty.CurrentSettlement == null)
				{
					float num2 = MBMath.LerpRadians(localFrame.rotation.f.AsVec2.RotationInRadians, (vec + Vec2.FromRotation(this._bearingRotation) * 0.01f).RotationInRadians, Math.Min(6f * dt, 1f), 0.03f * dt, 10f * dt);
					identity.rotation.RotateAboutUp(num2);
				}
				else
				{
					float rotationInRadians = localFrame.rotation.f.AsVec2.RotationInRadians;
					identity.rotation.RotateAboutUp(rotationInRadians);
				}
			}
			else if (mobileParty.CurrentSettlement == null)
			{
				identity.rotation.RotateAboutUp(this.GetVisualRotation());
				Vec3 vec2 = Vec3.Zero;
				for (int i = -2; i <= 2; i++)
				{
					for (int j = -2; j <= 2; j++)
					{
						Vec2 vec3 = identity.origin.AsVec2 + new Vec2((float)i * 0.5f, (float)j * 0.5f);
						float num3;
						Vec3 up;
						Campaign.Current.MapSceneWrapper.GetTerrainHeightAndNormal(vec3, ref num3, ref up);
						if (num3 < 2.58f)
						{
							up = Vec3.Up;
						}
						vec2 += up;
					}
				}
				vec2 /= MathF.Pow(5f, 2f);
				float num4 = Vec3.DotProduct(identity.rotation.u, vec2);
				float num5 = Vec3.DotProduct(identity.rotation.f, vec2);
				Vec3 vec4 = identity.rotation.u * num4;
				Vec3 vec5 = identity.rotation.f * num5;
				Vec3 vec6 = vec4 + vec5;
				float num6 = Vec3.AngleBetweenTwoVectors(identity.rotation.u, vec6) * 0.5f;
				float num7 = ((num5 < 0f) ? 1f : (-1f));
				this._lastFrameLerpedAngle = MathF.Lerp(this._lastFrameLerpedAngle, num7 * num6, 0.1f, 1E-05f);
				identity.rotation.RotateAboutSide(this._lastFrameLerpedAngle);
			}
			if (base.MapEntity.MobileParty.IsMainParty && MobileParty.MainParty.IsCurrentlyAtSea)
			{
				this.CheckBridgeFadeState();
			}
			if (this._shipEntity != null && !base.MapEntity.MobileParty.IsInRaftState && isVisible)
			{
				Vec2 windForPosition = Campaign.Current.Models.MapWeatherModel.GetWindForPosition(base.MapEntity.Position);
				this.ApplyWindEffect(windForPosition, identity.rotation.f.AsVec2, realDt, dt);
				this.TickSailingSound(num);
				this.TickOars(dt, realDt);
				this.TickIdleShipAnimation(base.MapEntity.FlagShip, ref this._rockingPhase, ref identity, false);
				this.TickSwayingAnimation(ref identity);
				float speedUpMultiplier = Campaign.Current.SpeedUpMultiplier;
				float num8 = realDt;
				if (Campaign.Current.TimeControlMode == 4 && !Campaign.Current.IsMainPartyWaiting)
				{
					num8 *= speedUpMultiplier;
				}
				else if (Campaign.Current.TimeControlMode == 5 || Campaign.Current.TimeControlMode == 2)
				{
					num8 *= speedUpMultiplier;
				}
				this.TickFoamDecals(num8);
			}
			if (!Extensions.IsEmpty<KeyValuePair<Ship, NavalMobilePartyVisual.BlockadeShipVisual>>(this._shipToBlockadeShipVisualCache))
			{
				foreach (KeyValuePair<Ship, NavalMobilePartyVisual.BlockadeShipVisual> keyValuePair in this._shipToBlockadeShipVisualCache.ToList<KeyValuePair<Ship, NavalMobilePartyVisual.BlockadeShipVisual>>())
				{
					NavalMobilePartyVisual.BlockadeShipVisual value = keyValuePair.Value;
					MatrixFrame localFrame2 = value.ShipEntity.GetLocalFrame();
					this.TickIdleShipAnimation(keyValuePair.Key, ref value.RockingPhase, ref localFrame2, true);
					value.ShipEntity.SetLocalFrame(ref localFrame2, true);
					this._shipToBlockadeShipVisualCache[keyValuePair.Key] = value;
				}
			}
			if (!this.StrategicEntity.GetFrame().NearlyEquals(identity, 1E-05f))
			{
				this.StrategicEntity.SetFrame(ref identity, true);
			}
		}

		// Token: 0x0600016B RID: 363 RVA: 0x0000A168 File Offset: 0x00008368
		internal void OnStartup()
		{
			bool flag = false;
			if (base.MapEntity.IsMobile)
			{
				this.StrategicEntity = GameEntity.CreateEmpty(NavalMobilePartyVisualManager.Current.MapScene, true, true, true);
				if (!base.MapEntity.IsVisible)
				{
					this.StrategicEntity.EntityFlags |= 536870912;
				}
			}
			CharacterObject visualPartyLeader = PartyBaseHelper.GetVisualPartyLeader(base.MapEntity);
			if (!flag)
			{
				this.CircleLocalFrame = MatrixFrame.Identity;
				if ((visualPartyLeader != null && visualPartyLeader.HasMount()) || base.MapEntity.MobileParty.IsCaravan)
				{
					MatrixFrame circleLocalFrame = this.CircleLocalFrame;
					Mat3 rotation = circleLocalFrame.rotation;
					rotation.ApplyScaleLocal(0.4625f);
					circleLocalFrame.rotation = rotation;
					this.CircleLocalFrame = circleLocalFrame;
				}
				else
				{
					MatrixFrame circleLocalFrame2 = this.CircleLocalFrame;
					Mat3 rotation2 = circleLocalFrame2.rotation;
					rotation2.ApplyScaleLocal(0.3725f);
					circleLocalFrame2.rotation = rotation2;
					this.CircleLocalFrame = circleLocalFrame2;
				}
			}
			this._bearingRotation = base.MapEntity.MobileParty.Bearing.RotationInRadians;
			this.StrategicEntity.SetVisibilityExcludeParents(base.MapEntity.IsVisible);
			this.StrategicEntity.SetReadyToRender(true);
			this.StrategicEntity.SetEntityEnvMapVisibility(false);
			this._entityAlpha = (base.MapEntity.IsVisible ? 1f : 0f);
			this._sailAlpha = 1f;
			this.AddVisualToVisualsOfEntities();
		}

		// Token: 0x0600016C RID: 364 RVA: 0x0000A2C8 File Offset: 0x000084C8
		internal void TickFadingState(float realDt)
		{
			if ((this._entityAlpha < 1f && base.MapEntity.IsVisible) || (this._entityAlpha > 0f && !base.MapEntity.IsVisible))
			{
				if (base.MapEntity.IsVisible)
				{
					if (this._entityAlpha <= 0f)
					{
						foreach (NavalMobilePartyVisual.BlockadeShipVisual blockadeShipVisual in this._shipToBlockadeShipVisualCache.Values)
						{
							blockadeShipVisual.ShipEntity.SetVisibilityExcludeParents(true);
						}
						this.StrategicEntity.SetVisibilityExcludeParents(true);
					}
					this._entityAlpha = MathF.Min(this._entityAlpha + MathF.Max(realDt, 1E-05f), 1f);
					this.StrategicEntity.SetAlpha(this._entityAlpha);
					this.StrategicEntity.EntityFlags &= -536870913;
					using (Dictionary<Ship, NavalMobilePartyVisual.BlockadeShipVisual>.ValueCollection.Enumerator enumerator = this._shipToBlockadeShipVisualCache.Values.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							NavalMobilePartyVisual.BlockadeShipVisual blockadeShipVisual2 = enumerator.Current;
							blockadeShipVisual2.ShipEntity.SetAlpha(this._entityAlpha);
						}
						return;
					}
				}
				this._entityAlpha = MathF.Max(this._entityAlpha - MathF.Max(realDt, 1E-05f), 0f);
				this.StrategicEntity.SetAlpha(this._entityAlpha);
				foreach (NavalMobilePartyVisual.BlockadeShipVisual blockadeShipVisual3 in this._shipToBlockadeShipVisualCache.Values)
				{
					blockadeShipVisual3.ShipEntity.SetAlpha(this._entityAlpha);
				}
				if (this._entityAlpha <= 0f)
				{
					this.StrategicEntity.SetVisibilityExcludeParents(false);
					foreach (NavalMobilePartyVisual.BlockadeShipVisual blockadeShipVisual4 in this._shipToBlockadeShipVisualCache.Values)
					{
						blockadeShipVisual4.ShipEntity.SetVisibilityExcludeParents(false);
					}
					this.StrategicEntity.EntityFlags |= 536870912;
					foreach (NavalMobilePartyVisual.ShipFoamDecal shipFoamDecal in this._splashFoamDecals)
					{
						if (shipFoamDecal != null && shipFoamDecal._splashFoamDecal != null)
						{
							shipFoamDecal._splashFoamDecal.SetIsVisible(false);
						}
					}
					return;
				}
			}
			else
			{
				NavalMobilePartyVisualManager.Current.UnRegisterFadingVisual(this);
			}
		}

		// Token: 0x0600016D RID: 365 RVA: 0x0000A564 File Offset: 0x00008764
		private void TickTransitionFadeState(float dt)
		{
			if (this.GetTransitionProgress() > 0f && base.MapEntity.MobileParty.IsCurrentlyAtSea && this._shipEntity != null && base.MapEntity.IsVisible)
			{
				CampaignVec2 campaignVec = base.MapEntity.MobileParty.EndPositionForNavigationTransition - base.MapEntity.MobileParty.Position.ToVec2();
				MatrixFrame globalFrame = this.StrategicEntity.GetGlobalFrame();
				float smallestDifferenceBetweenTwoAngles = MBMath.GetSmallestDifferenceBetweenTwoAngles(campaignVec.LeftVec().RotationInRadians, globalFrame.rotation.f.AsVec2.RotationInRadians);
				float smallestDifferenceBetweenTwoAngles2 = MBMath.GetSmallestDifferenceBetweenTwoAngles(campaignVec.RightVec().RotationInRadians, globalFrame.rotation.f.AsVec2.RotationInRadians);
				float num = ((MathF.Abs(smallestDifferenceBetweenTwoAngles2) > MathF.Abs(smallestDifferenceBetweenTwoAngles)) ? smallestDifferenceBetweenTwoAngles : smallestDifferenceBetweenTwoAngles2);
				float num2 = MathF.Lerp(0f, num, dt * 5f, 1E-05f);
				MatrixFrame localFrame = this.StrategicEntity.GetLocalFrame();
				localFrame.Rotate(MathF.Abs(num2), ref Vec3.Up);
				this.StrategicEntity.SetLocalFrame(ref localFrame, false);
				MatrixFrame globalFrame2 = this.StrategicEntity.GetGlobalFrame();
				CampaignVec2 campaignVec2 = base.MapEntity.MobileParty.Position + base.MapEntity.MobileParty.ArmyPositionAdder * 0.7f;
				float num3 = MathF.Lerp(globalFrame2.origin.X, campaignVec2.X, dt * 5f, 1E-05f);
				float num4 = MathF.Lerp(globalFrame2.origin.Y, campaignVec2.Y, dt * 5f, 1E-05f);
				globalFrame2.origin = new Vec3(num3, num4, globalFrame2.origin.z, -1f);
				this.StrategicEntity.SetGlobalFrame(ref globalFrame2, true);
			}
		}

		// Token: 0x0600016E RID: 366 RVA: 0x0000A767 File Offset: 0x00008967
		internal void ClearVisualMemory()
		{
			this.ResetPartyIcon();
			base.MapEntity.SetVisualAsDirty();
		}

		// Token: 0x0600016F RID: 367 RVA: 0x0000A77C File Offset: 0x0000897C
		internal void ValidateIsDirty()
		{
			if (base.MapEntity.MemberRoster.TotalManCount != 0)
			{
				this.RefreshPartyIcon();
				if ((this._entityAlpha < 1f && base.MapEntity.IsVisible) || (this._entityAlpha > 0f && !base.MapEntity.IsVisible))
				{
					if (base.MapEntity.MobileParty.IsTransitionInProgress && !this.StrategicEntity.GlobalPosition.IsNonZero)
					{
						this.UpdateEntityPosition(0.1f, 0.1f, false);
					}
					NavalMobilePartyVisualManager.Current.RegisterFadingVisual(this);
					return;
				}
			}
			else
			{
				this.ResetPartyIcon();
			}
		}

		// Token: 0x06000170 RID: 368 RVA: 0x0000A820 File Offset: 0x00008A20
		private void RefreshPartyIcon()
		{
			if (base.MapEntity.MobileParty.IsNavalVisualDirty)
			{
				base.MapEntity.MobileParty.OnNavalVisualsUpdated();
				if (this._raidAgentVisuals != null)
				{
					this._raidAgentVisuals.Reset();
					this._raidAgentVisuals = null;
				}
				MatrixFrame circleLocalFrame = this.CircleLocalFrame;
				circleLocalFrame.origin = Vec3.Zero;
				this.CircleLocalFrame = circleLocalFrame;
				if (!this.HasNavalVisual())
				{
					if (base.MapEntity.MobileParty.Ships.Count == 0 || base.MapEntity.MobileParty.IsInRaftState)
					{
						this.ResetPartyIcon();
					}
					else
					{
						this.RemoveBlockadeVisuals();
						this.HideNavalVisual();
					}
					this.RemoveVisualFromVisualsOfEntities();
				}
				else
				{
					this.AddVisualToVisualsOfEntities();
					Settlement besiegedSettlement = base.MapEntity.MobileParty.BesiegedSettlement;
					if (((besiegedSettlement != null) ? besiegedSettlement.SiegeEvent : null) != null && base.MapEntity.MobileParty.BesiegedSettlement.SiegeEvent.BesiegerCamp.HasInvolvedPartyForEventType(base.MapEntity, 5))
					{
						this.HideNavalVisual();
						if (base.MapEntity.MobileParty.BesiegedSettlement.SiegeEvent.IsBlockadeActive)
						{
							NavalDLCViewHelpers.BlockadeVisualHelper.AddBlockadeVisuals(this._shipToBlockadeShipVisualCache, base.MapEntity, this.StrategicEntity);
						}
						else
						{
							this.RemoveBlockadeVisuals();
						}
					}
					else if (base.MapEntity.MobileParty != null && (base.MapEntity.MobileParty.IsCurrentlyAtSea || base.MapEntity.MobileParty.IsTransitionInProgress) && (base.MapEntity.MobileParty.CurrentSettlement == null || base.MapEntity.MobileParty.IsTargetingPort))
					{
						if (base.MapEntity.MobileParty.IsInRaftState)
						{
							this.ResetPartyIcon();
							this.AddRaftVisual();
						}
						else if (base.MapEntity.Ships.Count > 0)
						{
							this.AddShipVisual();
						}
						this.InitializePartyCollider(base.MapEntity);
						if (base.MapEntity.MobileParty.MapEvent != null)
						{
							Settlement mapEventSettlement = base.MapEntity.MobileParty.MapEvent.MapEventSettlement;
							if (mapEventSettlement != null && mapEventSettlement.IsVillage)
							{
								if (this._raidAgentVisuals == null)
								{
									this.AddRaidPartyVisual(base.MapEntity.MobileParty.Party);
								}
								MatrixFrame identity = MatrixFrame.Identity;
								identity.origin = base.MapEntity.MobileParty.MapEvent.MapEventSettlement.Position.AsVec3();
								identity.rotation.ApplyScaleLocal(this._raidAgentVisuals.GetScale());
								this._raidAgentVisuals.GetWeakEntity().SetFrame(ref identity, true);
							}
						}
					}
				}
				this.StrategicEntity.CheckResources(true, false);
			}
		}

		// Token: 0x06000171 RID: 369 RVA: 0x0000AAD0 File Offset: 0x00008CD0
		private void AddRaidPartyVisual(PartyBase party)
		{
			CharacterObject visualPartyLeader = PartyBaseHelper.GetVisualPartyLeader(party);
			Equipment equipment = visualPartyLeader.Equipment.Clone(false);
			int num;
			this.GetMeleeWeaponToWield(party, out num);
			Monster baseMonsterFromRace = FaceGen.GetBaseMonsterFromRace(visualPartyLeader.Race);
			MBActionSet actionSetWithSuffix = MBGlobals.GetActionSetWithSuffix(baseMonsterFromRace, visualPartyLeader.IsFemale, "_map_with_banner");
			AgentVisualsData agentVisualsData = new AgentVisualsData().UseMorphAnims(true).Equipment(equipment).BodyProperties(visualPartyLeader.GetBodyProperties(visualPartyLeader.Equipment, -1))
				.SkeletonType(visualPartyLeader.IsFemale ? 1 : 0)
				.Scale(0.3f)
				.Frame(this.StrategicEntity.GetFrame())
				.ActionSet(actionSetWithSuffix)
				.Scene(this.MapScene)
				.Monster(baseMonsterFromRace)
				.PrepareImmediately(false)
				.RightWieldedItemIndex(num)
				.HasClippingPlane(true)
				.UseScaledWeapons(true);
			IFaction mapFaction = party.MapFaction;
			AgentVisualsData agentVisualsData2 = agentVisualsData.ClothColor1((mapFaction != null) ? mapFaction.Color : 4291609515U);
			IFaction mapFaction2 = party.MapFaction;
			AgentVisualsData agentVisualsData3 = agentVisualsData2.ClothColor2((mapFaction2 != null) ? mapFaction2.Color2 : 4291609515U).CharacterObjectStringId(visualPartyLeader.StringId).Race(visualPartyLeader.Race);
			this._raidAgentVisuals = AgentVisuals.Create(agentVisualsData3, "PartyIcon " + visualPartyLeader.Name, false, false, false);
			if (this._raidAgentVisuals != null)
			{
				MBSkeletonExtensions.SetAgentActionChannel(this._raidAgentVisuals.GetVisuals().GetSkeleton(), 0, ref ActionIndexCache.act_map_raid, MBRandom.NondeterministicRandomFloat * 0.7f, -0.2f, true, 0f);
				WeakGameEntity weakEntity = this._raidAgentVisuals.GetWeakEntity();
				uint num2 = (FactionManager.IsAtWarAgainstFaction(party.MapFaction, Hero.MainHero.MapFaction) ? 4294905856U : 4278206719U);
				weakEntity.SetContourColor(new uint?(num2), false);
				float num3 = MathF.Min(0.25f, 20f);
				this._raidAgentVisuals.Tick(null, 0.0001f, false, num3);
				weakEntity.Skeleton.ForceUpdateBoneFrames();
			}
		}

		// Token: 0x06000172 RID: 370 RVA: 0x0000ACB4 File Offset: 0x00008EB4
		private void GetMeleeWeaponToWield(PartyBase party, out int wieldedItemIndex)
		{
			wieldedItemIndex = -1;
			CharacterObject visualPartyLeader = PartyBaseHelper.GetVisualPartyLeader(party);
			if (visualPartyLeader != null)
			{
				for (int i = 0; i < 5; i++)
				{
					if (visualPartyLeader.Equipment[i].Item != null && visualPartyLeader.Equipment[i].Item.PrimaryWeapon.IsMeleeWeapon)
					{
						wieldedItemIndex = i;
						return;
					}
				}
			}
		}

		// Token: 0x06000173 RID: 371 RVA: 0x0000AD14 File Offset: 0x00008F14
		private void InitializePartyCollider(PartyBase party)
		{
			if (this.StrategicEntity != null && party.IsMobile)
			{
				if (this._shipEntity != null && this._bodyMeshEntity.IsValid)
				{
					this.UpdateEntityPosition(0.1f, 0.1f, false);
					MatrixFrame matrixFrame = this.StrategicEntity.GetGlobalFrame();
					Vec3 eulerAngles = matrixFrame.rotation.GetEulerAngles();
					matrixFrame = this._bodyMeshEntity.GetGlobalFrame();
					Vec3 eulerAngles2 = matrixFrame.rotation.GetEulerAngles();
					BoundingBox localPhysicsBoundingBox = GameEntityPhysicsExtensions.GetLocalPhysicsBoundingBox(this._bodyMeshEntity, false);
					localPhysicsBoundingBox.max.RotateAboutZ(eulerAngles.RotationZ - eulerAngles2.RotationZ);
					localPhysicsBoundingBox.min.RotateAboutZ(eulerAngles.RotationZ - eulerAngles2.RotationZ);
					float num = MathF.Abs(localPhysicsBoundingBox.max.x - localPhysicsBoundingBox.min.x) / 40f;
					float num2 = num / 2f;
					float num3 = MathF.Max(localPhysicsBoundingBox.max.y, localPhysicsBoundingBox.min.y);
					float num4 = MathF.Min(localPhysicsBoundingBox.max.y, localPhysicsBoundingBox.min.y);
					Vec3 vec;
					vec..ctor(0f, num3 / 20f - num2, num2 + 0.01f, -1f);
					Vec3 vec2;
					vec2..ctor(0f, num4 / 20f + num2, num2 + 0.01f, -1f);
					GameEntityPhysicsExtensions.AddCapsuleAsBody(this.StrategicEntity, vec, vec2, num, 144, "");
					return;
				}
				GameEntityPhysicsExtensions.AddCapsuleAsBody(this.StrategicEntity, new Vec3(0f, 0.5f, 0f, -1f), new Vec3(0f, -0.5f, 0f, -1f), 0.5f, 144, "");
			}
		}

		// Token: 0x06000174 RID: 372 RVA: 0x0000AEFC File Offset: 0x000090FC
		private void ResetPartyIcon()
		{
			if (this.StrategicEntity != null)
			{
				if ((this.StrategicEntity.EntityFlags & 268435456) != null)
				{
					this.StrategicEntity.RemoveFromPredisplayEntity();
				}
				this.StrategicEntity.ClearComponents();
			}
			if (this._shipEntity != null)
			{
				this._shipEntity.ClearComponents();
				this._sailVisualCache.Clear();
				this._oars.Clear();
				this._shipEntity = null;
				SoundEvent sailingSoundEvent = this._sailingSoundEvent;
				if (sailingSoundEvent != null)
				{
					sailingSoundEvent.Stop();
				}
				this._sailingSoundEvent = null;
				this._oarPhase = 0f;
			}
			if (this._raidAgentVisuals != null)
			{
				this._raidAgentVisuals.Reset();
				this._raidAgentVisuals = null;
			}
			this.RemoveBlockadeVisuals();
			if (this._currentCollidedBridgeEntity != null)
			{
				this._currentCollidedBridgeEntity.SetAlpha(1f);
				this._currentCollidedBridgeEntity = null;
			}
			this._bearingRotation = base.MapEntity.MobileParty.Bearing.RotationInRadians;
			this._isVisualInRaftState = false;
			NavalMobilePartyVisualManager.Current.UnRegisterFadingVisual(this);
			foreach (NavalMobilePartyVisual.ShipFoamDecal shipFoamDecal in this._splashFoamDecals)
			{
				if (shipFoamDecal != null && shipFoamDecal._splashFoamDecal != null)
				{
					this._ownerSceneCached.RemoveDecalInstance(shipFoamDecal._splashFoamDecal, "editor_set");
					shipFoamDecal._splashFoamDecal = null;
				}
			}
		}

		// Token: 0x06000175 RID: 373 RVA: 0x0000B058 File Offset: 0x00009258
		private void HideNavalVisual()
		{
			this.StrategicEntity.SetVisibilityExcludeParents(false);
			this._bearingRotation = base.MapEntity.MobileParty.Bearing.RotationInRadians;
			if (this._currentCollidedBridgeEntity != null)
			{
				this._currentCollidedBridgeEntity.SetAlpha(1f);
				this._currentCollidedBridgeEntity = null;
			}
			foreach (NavalMobilePartyVisual.ShipFoamDecal shipFoamDecal in this._splashFoamDecals)
			{
				if (shipFoamDecal != null && shipFoamDecal._splashFoamDecal != null)
				{
					shipFoamDecal._splashFoamDecal.SetIsVisible(false);
				}
			}
			NavalMobilePartyVisualManager.Current.UnRegisterFadingVisual(this);
		}

		// Token: 0x06000176 RID: 374 RVA: 0x0000B0F8 File Offset: 0x000092F8
		private float GetTransitionProgress()
		{
			if (this.IsMobileEntity && base.MapEntity.MobileParty.IsTransitionInProgress && base.MapEntity.MobileParty.NavigationTransitionDuration != CampaignTime.Zero)
			{
				return MBMath.ClampFloat(base.MapEntity.MobileParty.NavigationTransitionStartTime.ElapsedHoursUntilNow / (float)base.MapEntity.MobileParty.NavigationTransitionDuration.ToHours, 0f, 1f);
			}
			return 1f;
		}

		// Token: 0x06000177 RID: 375 RVA: 0x0000B182 File Offset: 0x00009382
		private float GetVisualRotation()
		{
			if (base.MapEntity.IsMobile && base.MapEntity.MapEvent != null && base.MapEntity.MapEvent.IsFieldBattle)
			{
				return this.GetMapEventVisualRotation();
			}
			return this._bearingRotation;
		}

		// Token: 0x06000178 RID: 376 RVA: 0x0000B1C0 File Offset: 0x000093C0
		private float GetMapEventVisualRotation()
		{
			if (base.MapEntity.MapEventSide.OtherSide.LeaderParty != null && base.MapEntity.MapEventSide.OtherSide.LeaderParty.IsMobile && base.MapEntity.MapEventSide.OtherSide.LeaderParty.IsMobile)
			{
				Vec2 vec = (base.MapEntity.MapEventSide.OtherSide.LeaderParty.MobileParty.VisualPosition2DWithoutError - base.MapEntity.MobileParty.VisualPosition2DWithoutError).Normalized();
				if (base.MapEntity.MapEvent.IsNavalMapEvent)
				{
					vec.RotateCCW(0.6f);
				}
				return vec.RotationInRadians;
			}
			return this._bearingRotation;
		}

		// Token: 0x06000179 RID: 377 RVA: 0x0000B288 File Offset: 0x00009488
		private void CollectOars()
		{
			this._oars.Clear();
			foreach (WeakGameEntity weakGameEntity in this._shipEntity.WeakEntity.CollectChildrenEntitiesWithTagAsEnumarable("oar_gate_left"))
			{
				WeakGameEntity firstChildEntityWithTag = weakGameEntity.GetFirstChildEntityWithTag("upgrade_slot");
				if (firstChildEntityWithTag != null)
				{
					NavalMobilePartyVisual.ShipOar shipOar = new NavalMobilePartyVisual.ShipOar
					{
						_oarEntity = firstChildEntityWithTag,
						_sideSign = 1f
					};
					this._oars.Add(shipOar);
				}
			}
			foreach (WeakGameEntity weakGameEntity2 in this._shipEntity.WeakEntity.CollectChildrenEntitiesWithTagAsEnumarable("oar_gate_right"))
			{
				WeakGameEntity firstChildEntityWithTag2 = weakGameEntity2.GetFirstChildEntityWithTag("upgrade_slot");
				if (firstChildEntityWithTag2 != null)
				{
					NavalMobilePartyVisual.ShipOar shipOar2 = new NavalMobilePartyVisual.ShipOar
					{
						_oarEntity = firstChildEntityWithTag2,
						_sideSign = -1f
					};
					this._oars.Add(shipOar2);
				}
			}
			this._firstOarRotationFrameCached = MatrixFrame.Identity;
			this._secondOarRotationFrameCached = MatrixFrame.Identity;
			this._firstOarRotationFrameCached.rotation.RotateAboutSide(-0.17453292f);
			this._secondOarRotationFrameCached.rotation.RotateAboutSide(-0.14835298f);
		}

		// Token: 0x0600017A RID: 378 RVA: 0x0000B3FC File Offset: 0x000095FC
		private void UpdateBearingRotation(float realDt)
		{
			float num = MBMath.WrapAngle(base.MapEntity.MobileParty.Bearing.RotationInRadians - this._bearingRotation);
			float num2 = realDt / 2f;
			float num3 = (((base.MapEntity.MobileParty.NextTargetPosition.ToVec2() - base.MapEntity.MobileParty.VisualPosition2DWithoutError).Length < 2f) ? 7.5f : 3f);
			this._bearingRotation += num * MathF.Min(num2 * num3, 1f);
			this._bearingRotation = MBMath.WrapAngle(this._bearingRotation);
		}

		// Token: 0x0600017B RID: 379 RVA: 0x0000B4A9 File Offset: 0x000096A9
		private float GetOarVerticalAngle(float phase, float verticalBaseAngle, float verticalRotationAngle)
		{
			return verticalBaseAngle + MathF.Cos(-phase) * verticalRotationAngle;
		}

		// Token: 0x0600017C RID: 380 RVA: 0x0000B4B6 File Offset: 0x000096B6
		private void TickSailingSound(float speed)
		{
			this._sailingSoundEvent.SetPosition(this.GetVisualPosition());
			if (!this._sailingSoundEvent.IsPlaying())
			{
				this._sailingSoundEvent.Play();
			}
			this._sailingSoundEvent.SetParameter("ShipSpeed", speed);
		}

		// Token: 0x0600017D RID: 381 RVA: 0x0000B4F4 File Offset: 0x000096F4
		private MatrixFrame ComputeOarFrame(NavalMobilePartyVisual.ShipOar oar)
		{
			MatrixFrame identity = MatrixFrame.Identity;
			identity.rotation.RotateAboutForward(oar._sideSign * this._oarPhase);
			MatrixFrame matrixFrame = identity.TransformToParent(ref this._firstOarRotationFrameCached);
			return this._secondOarRotationFrameCached.TransformToParent(ref matrixFrame);
		}

		// Token: 0x0600017E RID: 382 RVA: 0x0000B53C File Offset: 0x0000973C
		private void TickOars(float dt, float realDt)
		{
			if (this.IsMoving())
			{
				float num = ((dt > 0f) ? dt : (realDt * 0.25f));
				float num2 = (base.MapEntity.MobileParty.IsActive ? base.MapEntity.MobileParty.LastCalculatedBaseSpeed : 0f);
				this._oarPhase += num * num2 * 1.87f;
			}
			foreach (NavalMobilePartyVisual.ShipOar shipOar in this._oars)
			{
				MatrixFrame matrixFrame = this.ComputeOarFrame(shipOar);
				WeakGameEntity oarEntity = shipOar._oarEntity;
				oarEntity.SetFrame(ref matrixFrame, false);
			}
		}

		// Token: 0x0600017F RID: 383 RVA: 0x0000B600 File Offset: 0x00009800
		private void AddShipVisual()
		{
			if (base.MapEntity.IsActive)
			{
				this._isSailFolded = true;
				Ship flagShip = base.MapEntity.FlagShip;
				if (this._flagShipId == flagShip.ShipHull.StringId && this._shipEntity != null && this._isVisualInRaftState == base.MapEntity.MobileParty.IsInRaftState)
				{
					NavalDLCViewHelpers.ShipVisualHelper.RefreshShipVisuals(this._shipEntity.WeakEntity, flagShip, this._sailVisualCache);
				}
				else
				{
					if (this.StrategicEntity != null)
					{
						if ((this.StrategicEntity.EntityFlags & 268435456) != null)
						{
							this.StrategicEntity.RemoveFromPredisplayEntity();
						}
						this.StrategicEntity.ClearComponents();
					}
					if (this._shipEntity != null)
					{
						this._shipEntity.ClearComponents();
						this._sailVisualCache.Clear();
						this._shipEntity = null;
					}
					else
					{
						this._sailingSoundEvent = SoundEvent.CreateEventFromString("event:/map/army/sail", NavalMobilePartyVisualManager.Current.MapScene);
						this._sailingSoundEvent.SetPosition(this.GetVisualPosition());
					}
					this._shipEntity = NavalDLCViewHelpers.ShipVisualHelper.GetShipEntityForCampaign(flagShip, this.StrategicEntity.Scene, flagShip.GetShipVisualSlotInfos());
					NavalDLCViewHelpers.ShipVisualHelper.CollectSailVisuals(this._shipEntity.WeakEntity, this._sailVisualCache);
					this.CollectOars();
					float num = 50f;
					foreach (SailVisual sailVisual in this._sailVisualCache)
					{
						if (sailVisual.Type == SailVisual.SailType.LateenSail)
						{
							MatrixFrame localFrame = sailVisual.SailYawRotationEntity.GetLocalFrame();
							localFrame.rotation = Mat3.Identity;
							localFrame.rotation.RotateAboutUp(num * 0.017453292f);
							sailVisual.SailYawRotationEntity.SetFrame(ref localFrame, false);
						}
					}
					this._bodyMeshEntity = this._shipEntity.WeakEntity.GetFirstChildEntityWithTagRecursive("body_mesh");
					this.StrategicEntity.AddChild(this._shipEntity, false);
					this._shipEntity.SetVisibilityExcludeParents(true);
					this._flagShipId = flagShip.ShipHull.StringId;
					this._ownerSceneCached = this._shipEntity.Scene;
					this._shipMovementParticleEntity = GameEntity.CreateEmpty(this._ownerSceneCached, false, false, false);
					this._shipMovementParticleEntity.Name = "movement_particle";
					this._shipEntity.AddChild(this._shipMovementParticleEntity, false);
					MatrixFrame identity = MatrixFrame.Identity;
					if (this._bodyMeshEntity.IsValid)
					{
						MetaMesh metaMesh = this._bodyMeshEntity.GetMetaMesh(0);
						if (metaMesh != null)
						{
							this._wakeBB = metaMesh.GetBoundingBox();
							identity.origin.y = identity.origin.y + this._wakeBB.max.y * 0.8f;
							identity.rotation.ApplyScaleLocal(20f);
							this._shipMovementParticleEntity.SetFrame(ref identity, true);
						}
					}
					this._shipMovementParticleEntity.SetLocalFrame(ref identity, true);
					this._lastDecalSpawnPosition = this._shipEntity.GetGlobalFrame().origin;
					for (int i = 0; i < 20; i++)
					{
						this._splashFoamDecals[i] = new NavalMobilePartyVisual.ShipFoamDecal();
					}
					MatrixFrame identity2 = MatrixFrame.Identity;
					this._shipMovementParticle = ParticleSystem.CreateParticleSystemAttachedToEntity("psys_campaign_ship_trail", this._shipMovementParticleEntity, ref identity2);
					this._shipStillMovementParticleEntity = GameEntity.CreateEmpty(this._ownerSceneCached, false, false, false);
					this._shipStillMovementParticleEntity.Name = "movement_particle_still";
					this._shipEntity.AddChild(this._shipStillMovementParticleEntity, false);
					this._shipStillMovementParticleEntity.SetFrame(ref identity, true);
					this._shipStillMovementParticle = ParticleSystem.CreateParticleSystemAttachedToEntity("psys_campaign_ship_trail_still", this._shipStillMovementParticleEntity, ref identity2);
					this._shipStillMovementParticleEntity.SetVisibilityExcludeParents(false);
				}
				this._shipEntity.SetAlpha(this.GetTransitionProgress());
				this.StrategicEntity.SetAlpha(this.GetTransitionProgress());
				this.StrategicEntity.SetVisibilityExcludeParents(true);
				this._isVisualInRaftState = false;
			}
		}

		// Token: 0x06000180 RID: 384 RVA: 0x0000B9EC File Offset: 0x00009BEC
		private bool IsMoving()
		{
			bool flag = false;
			if (base.MapEntity.MobileParty != null && base.MapEntity.MobileParty.IsMainParty)
			{
				flag = !Campaign.Current.IsMainPartyWaiting;
			}
			else
			{
				MobileParty mobileParty = base.MapEntity.MobileParty;
				if (mobileParty != null && !mobileParty.Position.NearlyEquals(base.MapEntity.MobileParty.NextTargetPosition.ToVec2(), 1E-05f))
				{
					flag = true;
				}
			}
			return flag;
		}

		// Token: 0x06000181 RID: 385 RVA: 0x0000BA6C File Offset: 0x00009C6C
		private void TickIdleShipAnimation(Ship ship, ref float rockingPhase, ref MatrixFrame entityFrame, bool isBlockadeShip = false)
		{
			if (MBMath.ApproximatelyEqualsTo(MBMath.WrapAngle(base.MapEntity.MobileParty.Bearing.RotationInRadians - this._bearingRotation), 0f, 0.003f))
			{
				float num = 1f;
				float num2 = 0.07853982f;
				if (ship.ShipHull.Type == null)
				{
					num = 2f;
				}
				else if (ship.ShipHull.Type == 1)
				{
					num = 1.5f;
				}
				rockingPhase += num * 0.01f;
				if (this._swayingAngle != 0f)
				{
					this._swayingAngle = 0f;
					rockingPhase = 1.5707964f;
				}
				if (MathF.Abs(this._rollingAngle) > num2)
				{
					num2 = MathF.Abs(this._rollingAngle);
				}
				rockingPhase = MBMath.WrapAngle(rockingPhase);
				float num3 = MBMath.Map(MathF.Cos(rockingPhase), -1f, 1f, -num2, num2);
				if (isBlockadeShip)
				{
					Vec3 eulerAngles = entityFrame.rotation.GetEulerAngles();
					eulerAngles.y = num3 - eulerAngles.y;
					entityFrame.rotation.RotateAboutForward(eulerAngles.Y);
					return;
				}
				this._rollingAngle = MBMath.LerpRadians(this._rollingAngle, num3, 0.01f, 0f, num2);
				entityFrame.rotation.RotateAboutForward(this._rollingAngle);
			}
		}

		// Token: 0x06000182 RID: 386 RVA: 0x0000BBB0 File Offset: 0x00009DB0
		private void TickFoamDecals(float dt)
		{
			MatrixFrame globalFrame = this._shipEntity.GetGlobalFrame();
			Vec3 vec = new Vec3(0.013f, 0.025f, 1f, -1f) * 1.176f * 2f;
			Vec3 vec2 = vec * 17.5f;
			foreach (NavalMobilePartyVisual.ShipFoamDecal shipFoamDecal in this._splashFoamDecals)
			{
				if (shipFoamDecal._splashFoamDecal != null && shipFoamDecal._cumulativeDtTillStart < 3.15f)
				{
					shipFoamDecal._cumulativeDtTillStart += dt;
					float num = 4f;
					float num3;
					if (shipFoamDecal._cumulativeDtTillStart > 0.45f)
					{
						float num2 = shipFoamDecal._cumulativeDtTillStart - 0.45f;
						num3 = MathF.Clamp(1f - num2 / 2.7f, 0f, 1f);
					}
					else
					{
						num3 = MathF.Clamp(shipFoamDecal._cumulativeDtTillStart / 0.45f, 0f, 1f);
					}
					float num4 = 0.475f;
					float num5 = MathF.Pow(num3, num) * this._entityAlpha * (0.95f - num4) + num4;
					shipFoamDecal._splashFoamDecal.SetAlpha(num5);
					NavalMobilePartyVisual.ShipFoamDecal shipFoamDecal2 = shipFoamDecal;
					shipFoamDecal2._currentFrame.origin = shipFoamDecal2._currentFrame.origin + shipFoamDecal._currentSpeed * dt;
					Vec3 currentSpeed = shipFoamDecal._currentSpeed;
					float num6 = currentSpeed.Normalize();
					num6 = MathF.Max(num6 - dt * 2.5f, 0f);
					shipFoamDecal._currentSpeed = num6 * currentSpeed;
					float num7 = MathF.Clamp(shipFoamDecal._cumulativeDtTillStart / 3.15f, 0f, 1f);
					num7 = MathF.Pow(num7, 0.4f);
					Vec3 vec3 = Vec3.Lerp(vec, vec2, num7);
					vec3.x *= shipFoamDecal._randomScale.x;
					vec3.y *= shipFoamDecal._randomScale.y;
					vec3.z *= shipFoamDecal._randomScale.z;
					float num8 = 3.15f;
					float num9 = MathF.Clamp(shipFoamDecal._cumulativeDtTillStart / num8, 0f, 1f);
					Vec3 vec4 = Vec3.Slerp(shipFoamDecal._sideVectorStart, shipFoamDecal._sideVectorEnd, num9);
					vec4.Normalize();
					shipFoamDecal._currentFrame.rotation.s = vec4;
					shipFoamDecal._currentFrame.rotation.u = Vec3.Up;
					shipFoamDecal._currentFrame.rotation.f = -shipFoamDecal._currentFrame.rotation.s.CrossProductWithUp();
					shipFoamDecal._currentFrame.rotation.ApplyScaleLocal(ref vec3);
					shipFoamDecal._splashFoamDecal.Frame = shipFoamDecal._currentFrame;
				}
				else if (shipFoamDecal._splashFoamDecal != null)
				{
					shipFoamDecal._splashFoamDecal.SetIsVisible(false);
				}
			}
			Vec3 origin = globalFrame.origin;
			float num10 = this._lastDecalSpawnPosition.DistanceSquared(origin);
			if (this._nextDecalSpawnMetersSq < num10)
			{
				Vec3 vec5 = globalFrame.rotation.f.NormalizedCopy() * 0.5f;
				Vec3 s = globalFrame.rotation.s;
				s.z = 0f;
				s.Normalize();
				NavalMobilePartyVisual.ShipFoamDecal shipFoamDecal3 = this._splashFoamDecals[this._nextDecalToUse];
				if (shipFoamDecal3._splashFoamDecal == null)
				{
					Decal decal = Decal.CreateDecal(null);
					decal.SetMaterial(Material.GetFromResource("decal_water_foam"));
					this._ownerSceneCached.AddDecalInstance(decal, "editor_set", true);
					shipFoamDecal3._splashFoamDecal = decal;
				}
				shipFoamDecal3._splashFoamDecal.SetIsVisible(true);
				Vec3 vec6 = origin;
				vec6 -= globalFrame.rotation.f * this._wakeBB.max.z * 1.85f;
				float num11 = (0.5f + (MBRandom.RandomFloat - 0.5f) * 0.5f) * 0.33f;
				this._nextDecalSpawnMetersSq = num11 * num11;
				Vec3 vec7 = s;
				MatrixFrame identity = MatrixFrame.Identity;
				identity.origin = vec6;
				identity.rotation.u = Vec3.Up;
				Vec3 vec8 = globalFrame.rotation.TransformToParent(ref vec7);
				vec8.z = 0f;
				vec8.Normalize();
				identity.rotation.s = vec8;
				identity.rotation.f = -identity.rotation.s.CrossProductWithUp();
				identity.rotation.f.Normalize();
				shipFoamDecal3._cumulativeDtTillStart = 0f;
				float num12 = 0.6f;
				shipFoamDecal3._randomScale = Vec3.One * (0.9f + MBRandom.RandomFloat * 0.2f) * num12;
				NavalMobilePartyVisual.ShipFoamDecal shipFoamDecal4 = shipFoamDecal3;
				shipFoamDecal4._randomScale.x = shipFoamDecal4._randomScale.x * (1f * MBRandom.RandomFloat + 0.4f);
				identity.rotation.ApplyScaleLocal(ref vec);
				shipFoamDecal3._splashFoamDecal.Frame = identity;
				shipFoamDecal3._splashFoamDecal.SetAlpha(0f);
				shipFoamDecal3._currentFrame = identity;
				int num13 = MBRandom.RandomInt(3);
				float num14 = (float)(num13 % 2) * 0.5f;
				float num15 = (float)(num13 / 2) * 0.5f;
				shipFoamDecal3._splashFoamDecal.SetVectorArgument(num14, num15, -0.5f, -0.5f);
				float num16 = 0.16f * (0.8f + MBRandom.RandomFloat * 0.4f);
				float num17 = 0.45f * (0.8f + MBRandom.RandomFloat * 0.4f);
				shipFoamDecal3._currentSpeed = vec5 * num17 + identity.rotation.s * vec5.Length * num16;
				float num18 = -0.34906584f * (0.8f + MBRandom.RandomFloat * 0.4f);
				shipFoamDecal3._sideVectorStart = vec8;
				shipFoamDecal3._sideVectorStart.RotateAboutZ(1.5707964f);
				shipFoamDecal3._sideVectorEnd = shipFoamDecal3._sideVectorStart;
				shipFoamDecal3._sideVectorEnd.RotateAboutZ(num18);
				Vec2 vec9;
				vec9..ctor(2.5f, 2.5f);
				shipFoamDecal3._splashFoamDecal.OverrideRoadBoundaryP0(vec9);
				Vec2 vec10;
				vec10..ctor(MBRandom.RandomFloat, MBRandom.RandomFloat);
				shipFoamDecal3._splashFoamDecal.OverrideRoadBoundaryP1(vec10);
				this._nextDecalToUse = (this._nextDecalToUse + 1) % 20;
				this._lastDecalSpawnPosition = origin;
			}
		}

		// Token: 0x06000183 RID: 387 RVA: 0x0000C234 File Offset: 0x0000A434
		private void TickSwayingAnimation(ref MatrixFrame entityFrame)
		{
			float num = MBMath.WrapAngle(base.MapEntity.MobileParty.Bearing.RotationInRadians - this._bearingRotation);
			if (!MBMath.ApproximatelyEqualsTo(num, 0f, 0.003f))
			{
				float num2 = 0.5f;
				float num3 = 0.1f;
				if (base.MapEntity.MobileParty.TargetParty != null)
				{
					num2 = 1.5f;
					num3 = 0.01f * MBMath.Map(num, 0f, 3.1415927f, 1f, 10f);
				}
				if (this._swayingAngle == 0f || !MBMath.ApproximatelyEqualsTo(this._targetPositionForSwaying.Distance(base.MapEntity.MobileParty.NextTargetPosition), 0f, num2))
				{
					this._swayingAngle = num;
					this._targetPositionForSwaying = base.MapEntity.MobileParty.NextTargetPosition;
				}
				float num4;
				if (this._swayingAngle >= 0f)
				{
					num4 = MBMath.Map(num, 0f, this._swayingAngle, 0f, 3.1415927f);
				}
				else
				{
					num4 = MBMath.Map(num, this._swayingAngle, 0f, -3.1415927f, 0f);
				}
				float num5 = MBMath.Map(MathF.Abs(this._swayingAngle), 0f, 3.1415927f, 0f, 0.62831855f);
				float num6 = MBMath.Map(MathF.Sin(num4), -1f, 1f, -num5, num5);
				this._rollingAngle = MBMath.LerpRadians(this._rollingAngle, num6, num3, 0f, num5);
				entityFrame.rotation.RotateAboutForward(this._rollingAngle);
			}
		}

		// Token: 0x06000184 RID: 388 RVA: 0x0000C3CC File Offset: 0x0000A5CC
		private void CheckBridgeFadeState()
		{
			if (Campaign.Current.MapSceneWrapper.GetFaceTerrainType(base.MapEntity.MobileParty.CurrentNavigationFace) == 25)
			{
				GameEntity nearbyBridgeToParty = NavalMobilePartyVisualManager.Current.GetNearbyBridgeToParty(base.MapEntity);
				if (nearbyBridgeToParty != null)
				{
					nearbyBridgeToParty.SetAlpha(0.3f);
				}
				if (this._currentCollidedBridgeEntity != nearbyBridgeToParty)
				{
					GameEntity currentCollidedBridgeEntity = this._currentCollidedBridgeEntity;
					if (currentCollidedBridgeEntity != null)
					{
						currentCollidedBridgeEntity.SetAlpha(1f);
					}
					this._currentCollidedBridgeEntity = nearbyBridgeToParty;
					return;
				}
			}
			else
			{
				GameEntity currentCollidedBridgeEntity2 = this._currentCollidedBridgeEntity;
				if (currentCollidedBridgeEntity2 != null)
				{
					currentCollidedBridgeEntity2.SetAlpha(1f);
				}
				this._currentCollidedBridgeEntity = null;
			}
		}

		// Token: 0x06000185 RID: 389 RVA: 0x0000C464 File Offset: 0x0000A664
		private void ApplyWindEffect(Vec2 windVector, Vec2 shipDirection, float realDt, float dt)
		{
			if (MathF.Abs(MBMath.ToDegrees(windVector.AngleBetween(shipDirection))) > 80f)
			{
				if (!this._isSailFolded && this._sailVisualCache.Count > 0)
				{
					this._isSailFolded = true;
					NavalDLCViewHelpers.ShipVisualHelper.FoldSails(this._sailVisualCache);
				}
			}
			else if (this._isSailFolded && this._sailVisualCache.Count > 0)
			{
				this._isSailFolded = false;
				NavalDLCViewHelpers.ShipVisualHelper.UnfoldSails(this._sailVisualCache);
			}
			if (!base.MapEntity.MobileParty.IsMainParty)
			{
				if (Campaign.Current.MapSceneWrapper.GetFaceTerrainType(base.MapEntity.MobileParty.CurrentNavigationFace) == 25)
				{
					this._sailAlpha = MathF.Max(this._sailAlpha - MathF.Max(realDt, 1E-05f), 0.01f);
					if (this._sailAlpha <= 0.00999f)
					{
						goto IL_0176;
					}
					using (List<SailVisual>.Enumerator enumerator = this._sailVisualCache.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							SailVisual sailVisual = enumerator.Current;
							sailVisual.SetSailEntityAlpha(this._sailAlpha);
						}
						goto IL_0176;
					}
				}
				this._sailAlpha = MathF.Min(this._sailAlpha + MathF.Max(realDt, 1E-05f), 1f);
				if (this._sailAlpha < 1.00001f)
				{
					foreach (SailVisual sailVisual2 in this._sailVisualCache)
					{
						sailVisual2.SetSailEntityAlpha(this._sailAlpha);
					}
				}
			}
			IL_0176:
			float length = windVector.Length;
			Vec3 vec = windVector.Normalized().ToVec3(0f);
			if (this._sailVisualCache.Any<SailVisual>() && !this._isSailFolded)
			{
				float num = MathF.Clamp(length * 5f, 0.5f, 2.5f);
				foreach (SailVisual sailVisual3 in this._sailVisualCache)
				{
					if (sailVisual3 != null)
					{
						ClothSimulatorComponent sailClothComponent = sailVisual3.SailClothComponent;
						if (sailClothComponent != null)
						{
							sailClothComponent.SetForcedWind(vec * num, false);
						}
					}
				}
			}
			if (this._sailVisualCache.Any<SailVisual>())
			{
				float num2 = MathF.Clamp(length * 3f, 0.3f, 2.5f);
				foreach (SailVisual sailVisual4 in this._sailVisualCache)
				{
					if (sailVisual4 != null)
					{
						ClothSimulatorComponent sailTopBannerClothComponent = sailVisual4.SailTopBannerClothComponent;
						if (sailTopBannerClothComponent != null)
						{
							sailTopBannerClothComponent.SetForcedWind(vec * num2, false);
						}
					}
				}
			}
		}

		// Token: 0x06000186 RID: 390 RVA: 0x0000C724 File Offset: 0x0000A924
		private void AddRaftVisual()
		{
			this._shipEntity = GameEntity.Instantiate(this.StrategicEntity.Scene, "raft", MatrixFrame.Identity, true);
			this.StrategicEntity.AddChild(this._shipEntity, false);
			bool isMainParty = base.MapEntity.MobileParty.IsMainParty;
			this._shipEntity.SetVisibilityExcludeParents(isMainParty);
			this._shipEntity.SetAlpha(isMainParty ? 1f : 0f);
			this._sailingSoundEvent = SoundEvent.CreateEventFromString("event:/map/army/sail", NavalMobilePartyVisualManager.Current.MapScene);
			this._sailingSoundEvent.SetPosition(this.GetVisualPosition());
			this._isVisualInRaftState = true;
			this._bodyMeshEntity = this._shipEntity.WeakEntity.GetFirstChildEntityWithTagRecursive("body_mesh");
		}

		// Token: 0x06000187 RID: 391 RVA: 0x0000C7EC File Offset: 0x0000A9EC
		private void RemoveBlockadeVisuals()
		{
			if (!Extensions.IsEmpty<KeyValuePair<Ship, NavalMobilePartyVisual.BlockadeShipVisual>>(this._shipToBlockadeShipVisualCache))
			{
				foreach (KeyValuePair<Ship, NavalMobilePartyVisual.BlockadeShipVisual> keyValuePair in this._shipToBlockadeShipVisualCache)
				{
					keyValuePair.Value.ShipEntity.SetVisibilityExcludeParents(false);
					keyValuePair.Value.ShipEntity.ClearComponents();
				}
				this._shipToBlockadeShipVisualCache.Clear();
			}
		}

		// Token: 0x06000188 RID: 392 RVA: 0x0000C874 File Offset: 0x0000AA74
		private bool HasNavalVisual()
		{
			if ((base.MapEntity.MobileParty.Ships.Count <= 0 && !base.MapEntity.MobileParty.IsInRaftState) || !base.MapEntity.MobileParty.IsCurrentlyAtSea || (base.MapEntity.MobileParty.CurrentSettlement != null && !base.MapEntity.MobileParty.IsTargetingPort))
			{
				if (base.MapEntity.MobileParty.Ships.Count > 0)
				{
					SiegeEvent siegeEvent = base.MapEntity.MobileParty.SiegeEvent;
					if (((siegeEvent != null) ? siegeEvent.BesiegedSettlement : null) != null)
					{
						SiegeEvent siegeEvent2 = base.MapEntity.MobileParty.SiegeEvent;
						return siegeEvent2 != null && siegeEvent2.IsBlockadeActive;
					}
				}
				return false;
			}
			return true;
		}

		// Token: 0x06000189 RID: 393 RVA: 0x0000C936 File Offset: 0x0000AB36
		private void AddVisualToVisualsOfEntities()
		{
			if (!MapScreen.VisualsOfEntities.ContainsKey(this.StrategicEntity.Pointer))
			{
				MapScreen.VisualsOfEntities.Add(this.StrategicEntity.Pointer, this);
			}
		}

		// Token: 0x0600018A RID: 394 RVA: 0x0000C968 File Offset: 0x0000AB68
		private void RemoveVisualFromVisualsOfEntities()
		{
			MapScreen.VisualsOfEntities.Remove(this.StrategicEntity.Pointer);
			foreach (GameEntity gameEntity in this.StrategicEntity.GetChildren())
			{
				MapScreen.VisualsOfEntities.Remove(gameEntity.Pointer);
			}
		}

		// Token: 0x04000088 RID: 136
		private const float DefaultWaterLevelZ = 2.58f;

		// Token: 0x04000089 RID: 137
		private const float SailWindVisualAmplifier = 5f;

		// Token: 0x0400008A RID: 138
		private const float BannerWindVisualAmplifier = 3f;

		// Token: 0x0400008B RID: 139
		private const string LeftOarTag = "oar_gate_left";

		// Token: 0x0400008C RID: 140
		private const string RightOarTag = "oar_gate_right";

		// Token: 0x0400008D RID: 141
		private const string BodyMeshTag = "body_mesh";

		// Token: 0x0400008E RID: 142
		private const int NumberOfSplashDecal = 20;

		// Token: 0x0400008F RID: 143
		private float _entityAlpha;

		// Token: 0x04000090 RID: 144
		private bool _isSailFolded;

		// Token: 0x04000091 RID: 145
		private float _sailAlpha;

		// Token: 0x04000092 RID: 146
		private Scene _mapScene;

		// Token: 0x04000093 RID: 147
		private AgentVisuals _raidAgentVisuals;

		// Token: 0x04000094 RID: 148
		private string _flagShipId;

		// Token: 0x04000095 RID: 149
		private bool _isVisualInRaftState;

		// Token: 0x04000096 RID: 150
		private MatrixFrame _firstOarRotationFrameCached = MatrixFrame.Identity;

		// Token: 0x04000097 RID: 151
		private MatrixFrame _secondOarRotationFrameCached = MatrixFrame.Identity;

		// Token: 0x04000098 RID: 152
		private readonly Dictionary<Ship, NavalMobilePartyVisual.BlockadeShipVisual> _shipToBlockadeShipVisualCache = new Dictionary<Ship, NavalMobilePartyVisual.BlockadeShipVisual>();

		// Token: 0x04000099 RID: 153
		private readonly List<NavalMobilePartyVisual.ShipOar> _oars = new List<NavalMobilePartyVisual.ShipOar>();

		// Token: 0x0400009A RID: 154
		private readonly List<SailVisual> _sailVisualCache = new List<SailVisual>();

		// Token: 0x0400009B RID: 155
		private SoundEvent _sailingSoundEvent;

		// Token: 0x0400009C RID: 156
		private float _oarPhase;

		// Token: 0x0400009D RID: 157
		private float _rockingPhase;

		// Token: 0x0400009E RID: 158
		private float _swayingAngle;

		// Token: 0x0400009F RID: 159
		private float _rollingAngle;

		// Token: 0x040000A0 RID: 160
		private CampaignVec2 _targetPositionForSwaying;

		// Token: 0x040000A1 RID: 161
		private float _lastFrameLerpedAngle;

		// Token: 0x040000A2 RID: 162
		private GameEntity _shipEntity;

		// Token: 0x040000A3 RID: 163
		private WeakGameEntity _bodyMeshEntity;

		// Token: 0x040000A4 RID: 164
		private GameEntity _currentCollidedBridgeEntity;

		// Token: 0x040000A5 RID: 165
		private float _bearingRotation;

		// Token: 0x040000A6 RID: 166
		private GameEntity _shipMovementParticleEntity;

		// Token: 0x040000A7 RID: 167
		private ParticleSystem _shipMovementParticle;

		// Token: 0x040000A8 RID: 168
		private GameEntity _shipStillMovementParticleEntity;

		// Token: 0x040000A9 RID: 169
		private ParticleSystem _shipStillMovementParticle;

		// Token: 0x040000AA RID: 170
		private BoundingBox _wakeBB;

		// Token: 0x040000AB RID: 171
		private Scene _ownerSceneCached;

		// Token: 0x040000AC RID: 172
		private NavalMobilePartyVisual.ShipFoamDecal[] _splashFoamDecals = new NavalMobilePartyVisual.ShipFoamDecal[20];

		// Token: 0x040000AD RID: 173
		private Vec3 _lastDecalSpawnPosition = Vec3.Zero;

		// Token: 0x040000AE RID: 174
		private float _nextDecalSpawnMetersSq = 0.09f;

		// Token: 0x040000AF RID: 175
		private int _nextDecalToUse;

		// Token: 0x02000050 RID: 80
		private struct ShipOar
		{
			// Token: 0x04000120 RID: 288
			internal WeakGameEntity _oarEntity;

			// Token: 0x04000121 RID: 289
			internal float _sideSign;
		}

		// Token: 0x02000051 RID: 81
		public struct BlockadeShipVisual
		{
			// Token: 0x04000122 RID: 290
			public GameEntity ShipEntity;

			// Token: 0x04000123 RID: 291
			public float RockingPhase;
		}

		// Token: 0x02000052 RID: 82
		private class ShipFoamDecal
		{
			// Token: 0x060001FB RID: 507 RVA: 0x0000EFE4 File Offset: 0x0000D1E4
			internal ShipFoamDecal()
			{
				this._splashFoamDecal = null;
				this._currentFrame = MatrixFrame.Identity;
				this._sideVectorStart = Vec3.Zero;
				this._sideVectorEnd = Vec3.Zero;
				this._cumulativeDtTillStart = 0f;
				this._randomScale = new Vec3(1f, 1f, 1f, -1f);
				this._currentSpeed = Vec3.Zero;
			}

			// Token: 0x04000124 RID: 292
			internal Decal _splashFoamDecal;

			// Token: 0x04000125 RID: 293
			internal MatrixFrame _currentFrame;

			// Token: 0x04000126 RID: 294
			internal float _cumulativeDtTillStart;

			// Token: 0x04000127 RID: 295
			internal Vec3 _randomScale;

			// Token: 0x04000128 RID: 296
			internal Vec3 _currentSpeed;

			// Token: 0x04000129 RID: 297
			internal Vec3 _sideVectorStart;

			// Token: 0x0400012A RID: 298
			internal Vec3 _sideVectorEnd;
		}
	}
}
