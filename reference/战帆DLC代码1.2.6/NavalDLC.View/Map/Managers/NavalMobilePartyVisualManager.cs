using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.View.Map.Visuals;
using SandBox.View;
using SandBox.View.Map;
using SandBox.View.Map.Managers;
using SandBox.View.Map.Visuals;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.View.Map.Managers
{
	// Token: 0x02000039 RID: 57
	public class NavalMobilePartyVisualManager : EntityVisualManagerBase<PartyBase>
	{
		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060001B7 RID: 439 RVA: 0x0000D466 File Offset: 0x0000B666
		public static NavalMobilePartyVisualManager Current
		{
			get
			{
				return SandBoxViewSubModule.SandBoxViewVisualManager.GetEntityComponent<NavalMobilePartyVisualManager>();
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060001B8 RID: 440 RVA: 0x0000D472 File Offset: 0x0000B672
		public override int Priority
		{
			get
			{
				return 20;
			}
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0000D478 File Offset: 0x0000B678
		public override void OnTick(float realDt, float dt)
		{
			if (!base.MapScene.HasTerrainHeightmap || !base.MapScene.ContainsTerrain)
			{
				return;
			}
			this._dirtyPartyVisualCount = -1;
			TWParallel.For(0, this._visualsFlattened.Count, delegate(int startInclusive, int endExclusive)
			{
				for (int k = startInclusive; k < endExclusive; k++)
				{
					this._visualsFlattened[k].Tick(dt, realDt, ref this._dirtyPartyVisualCount, ref this._dirtyPartiesList);
				}
			}, 16);
			for (int i = 0; i < this._dirtyPartyVisualCount + 1; i++)
			{
				this._dirtyPartiesList[i].ValidateIsDirty();
			}
			for (int j = this._fadingPartiesFlatten.Count - 1; j >= 0; j--)
			{
				this._fadingPartiesFlatten[j].TickFadingState(realDt);
			}
			if (dt > 0f && this._timeElapsedSinceLastShipDamageSoundPlayed < 0f)
			{
				this._timeElapsedSinceLastShipDamageSoundPlayed += realDt;
			}
			if (this._timeElapsedSinceLastShipDamageSoundPlayed >= 0f && MobileParty.MainParty.IsCurrentlyAtSea && MobileParty.MainParty.Ships.Any<Ship>())
			{
				this.TriggerShipDamageSound();
			}
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000D590 File Offset: 0x0000B790
		public override void ClearVisualMemory()
		{
			foreach (NavalMobilePartyVisual navalMobilePartyVisual in this._visualsFlattened)
			{
				navalMobilePartyVisual.ClearVisualMemory();
			}
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000D5E0 File Offset: 0x0000B7E0
		public override MapEntityVisual<PartyBase> GetVisualOfEntity(PartyBase partyBase)
		{
			MobileParty mobileParty = partyBase.MobileParty;
			if (mobileParty != null && mobileParty.IsCurrentlyAtSea)
			{
				NavalMobilePartyVisual navalMobilePartyVisual;
				this._partiesAndVisuals.TryGetValue(partyBase, out navalMobilePartyVisual);
				return navalMobilePartyVisual;
			}
			return null;
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0000D614 File Offset: 0x0000B814
		public override bool OnVisualIntersected(Ray mouseRay, UIntPtr[] intersectedEntityIDs, Intersection[] intersectionInfos, int entityCount, Vec3 worldMouseNear, Vec3 worldMouseFar, Vec3 terrainIntersectionPoint, ref MapEntityVisual hoveredVisual, ref MapEntityVisual selectedVisual)
		{
			for (int i = entityCount - 1; i >= 0; i--)
			{
				UIntPtr uintPtr = intersectedEntityIDs[i];
				MapEntityVisual mapEntityVisual;
				NavalMobilePartyVisual navalMobilePartyVisual;
				if (uintPtr != UIntPtr.Zero && MapScreen.VisualsOfEntities.TryGetValue(uintPtr, out mapEntityVisual) && (navalMobilePartyVisual = mapEntityVisual as NavalMobilePartyVisual) != null && mapEntityVisual.IsVisibleOrFadingOut() && (!navalMobilePartyVisual.MapEntity.IsMobile || navalMobilePartyVisual.MapEntity.MobileParty.IsMainParty || !navalMobilePartyVisual.MapEntity.MobileParty.IsInRaftState))
				{
					Intersection intersection = intersectionInfos[i];
					float length = (worldMouseNear - intersection.IntersectionPoint).Length;
					if (mapEntityVisual.AttachedTo == null)
					{
						hoveredVisual = mapEntityVisual;
					}
					else
					{
						hoveredVisual = mapEntityVisual.AttachedTo;
					}
					if (!mapEntityVisual.IsMainEntity && (mapEntityVisual.AttachedTo == null || !mapEntityVisual.AttachedTo.IsMainEntity))
					{
						if (mapEntityVisual.AttachedTo != null)
						{
							selectedVisual = mapEntityVisual.AttachedTo;
						}
						else
						{
							selectedVisual = mapEntityVisual;
						}
					}
				}
			}
			return selectedVisual != null;
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000D718 File Offset: 0x0000B918
		protected override void OnInitialize()
		{
			base.OnInitialize();
			foreach (MobileParty mobileParty in MobileParty.All)
			{
				this.AddNewPartyVisualForParty(mobileParty, true);
			}
			CampaignEvents.MobilePartyDestroyed.AddNonSerializedListener(this, new Action<MobileParty, PartyBase>(this.OnMobilePartyDestroyed));
			CampaignEvents.MobilePartyCreated.AddNonSerializedListener(this, new Action<MobileParty>(this.OnMobilePartyCreated));
			CampaignEvents.OnMobilePartyNavigationStateChangedEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.OnMobilePartyNavigationStateChanged));
			CampaignEvents.OnMobilePartyJoinedToSiegeEventEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.OnMobilePartyJoinedToSiegeEvent));
			CampaignEvents.OnMobilePartyLeftSiegeEventEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.OnMobilePartyLeftSiegeEvent));
			if (MobileParty.MainParty.Ships.Any<Ship>())
			{
				this._mainPartyPreviousShipDamageTriggerHealthPercent = MobileParty.MainParty.Ships.Average<Ship>((Ship s) => s.HitPoints / s.MaxHitPoints);
			}
			this._bridgeEntityCache.AddRange(base.MapScene.FindEntitiesWithTag("bridge"));
		}

		// Token: 0x060001BE RID: 446 RVA: 0x0000D844 File Offset: 0x0000BA44
		protected override void OnFinalize()
		{
			foreach (NavalMobilePartyVisual navalMobilePartyVisual in this._partiesAndVisuals.Values)
			{
				navalMobilePartyVisual.ReleaseResources();
			}
			CampaignEventDispatcher.Instance.RemoveListeners(this);
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0000D8A4 File Offset: 0x0000BAA4
		public NavalMobilePartyVisual GetPartyVisual(PartyBase partyBase)
		{
			return this._partiesAndVisuals[partyBase];
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x0000D8B2 File Offset: 0x0000BAB2
		internal void RegisterFadingVisual(NavalMobilePartyVisual visual)
		{
			if (!this._fadingPartiesSet.Contains(visual))
			{
				this._fadingPartiesFlatten.Add(visual);
				this._fadingPartiesSet.Add(visual);
			}
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x0000D8DC File Offset: 0x0000BADC
		internal GameEntity GetNearbyBridgeToParty(PartyBase partyBase)
		{
			NavalMobilePartyVisual visual;
			if (this._partiesAndVisuals.TryGetValue(partyBase, out visual))
			{
				return this._bridgeEntityCache.FirstOrDefault<GameEntity>((GameEntity x) => x.GlobalPosition.Distance(visual.StrategicEntity.GlobalPosition) < 3f);
			}
			return null;
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000D91C File Offset: 0x0000BB1C
		private void OnMobilePartyNavigationStateChanged(MobileParty mobileParty)
		{
			if (mobileParty.IsCurrentlyAtSea && mobileParty.Ships.Count > 0)
			{
				if (mobileParty.IsMainParty)
				{
					SoundEvent.PlaySound2D("event:/ui/ship_disembark");
					return;
				}
			}
			else if (mobileParty.IsMainParty)
			{
				SoundEvent.PlaySound2D("event:/ui/ship_embark");
			}
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x0000D95C File Offset: 0x0000BB5C
		private void TriggerShipDamageSound()
		{
			float num = MobileParty.MainParty.Ships.Average<Ship>((Ship s) => s.HitPoints / s.MaxHitPoints);
			float num2 = this._mainPartyPreviousShipDamageTriggerHealthPercent - num;
			if (num2 > 0.01f)
			{
				this._mainPartyPreviousShipDamageTriggerHealthPercent = num;
				this._timeElapsedSinceLastShipDamageSoundPlayed = -2f;
				SoundEventParameter soundEventParameter;
				soundEventParameter..ctor("Campaign Ship Damage", num2 * 10f);
				MBSoundEvent.PlaySound(NavalMobilePartyVisualManager._shipDamageSoundEventId, ref soundEventParameter, Vec3.Zero);
			}
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x0000D9E0 File Offset: 0x0000BBE0
		private void OnMobilePartyLeftSiegeEvent(MobileParty mobileParty)
		{
			if (mobileParty.SiegeEvent != null && mobileParty.SiegeEvent.BesiegedSettlement.HasPort && !mobileParty.SiegeEvent.BlockadeShouldBeActivated && mobileParty.Ships.Any<Ship>())
			{
				mobileParty.SetNavalVisualAsDirty();
				foreach (PartyBase partyBase in mobileParty.BesiegerCamp.GetInvolvedPartiesForEventType(9))
				{
					partyBase.MobileParty.SetNavalVisualAsDirty();
				}
			}
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x0000DA70 File Offset: 0x0000BC70
		private void OnMobilePartyJoinedToSiegeEvent(MobileParty mobileParty)
		{
			SiegeEvent siegeEvent = mobileParty.SiegeEvent;
			if (siegeEvent != null && siegeEvent.IsBlockadeActive && mobileParty.Ships.Any<Ship>())
			{
				foreach (PartyBase partyBase in mobileParty.BesiegerCamp.GetInvolvedPartiesForEventType(9))
				{
					partyBase.MobileParty.SetNavalVisualAsDirty();
				}
			}
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x0000DAE8 File Offset: 0x0000BCE8
		private void OnMobilePartyDestroyed(MobileParty mobileParty, PartyBase _)
		{
			this.RemovePartyVisualForParty(mobileParty);
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0000DAF1 File Offset: 0x0000BCF1
		private void OnMobilePartyCreated(MobileParty mobileParty)
		{
			this.AddNewPartyVisualForParty(mobileParty, false);
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x0000DAFC File Offset: 0x0000BCFC
		internal void UnRegisterFadingVisual(NavalMobilePartyVisual visual)
		{
			if (this._fadingPartiesSet.Contains(visual))
			{
				int num = this._fadingPartiesFlatten.IndexOf(visual);
				this._fadingPartiesFlatten[num] = this._fadingPartiesFlatten[this._fadingPartiesFlatten.Count - 1];
				this._fadingPartiesFlatten.Remove(this._fadingPartiesFlatten[this._fadingPartiesFlatten.Count - 1]);
				this._fadingPartiesSet.Remove(visual);
			}
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x0000DB7C File Offset: 0x0000BD7C
		private void AddNewPartyVisualForParty(MobileParty mobileParty, bool shouldTick = false)
		{
			if (!mobileParty.IsGarrison && !mobileParty.IsMilitia && !this._partiesAndVisuals.ContainsKey(mobileParty.Party))
			{
				NavalMobilePartyVisual navalMobilePartyVisual = new NavalMobilePartyVisual(mobileParty.Party);
				navalMobilePartyVisual.OnStartup();
				this._partiesAndVisuals.Add(mobileParty.Party, navalMobilePartyVisual);
				this._visualsFlattened.Add(navalMobilePartyVisual);
				if (shouldTick)
				{
					navalMobilePartyVisual.Tick(0.1f, 0.1f, ref this._dirtyPartyVisualCount, ref this._dirtyPartiesList);
					if (mobileParty.IsTransitionInProgress)
					{
						mobileParty.SetNavalVisualAsDirty();
						navalMobilePartyVisual.UpdateEntityPosition(0.1f, 0.1f, false);
					}
				}
			}
		}

		// Token: 0x060001CA RID: 458 RVA: 0x0000DC20 File Offset: 0x0000BE20
		private void RemovePartyVisualForParty(MobileParty mobileParty)
		{
			NavalMobilePartyVisual navalMobilePartyVisual;
			if (this._partiesAndVisuals.TryGetValue(mobileParty.Party, out navalMobilePartyVisual))
			{
				navalMobilePartyVisual.OnPartyRemoved();
				this._visualsFlattened.Remove(navalMobilePartyVisual);
				this._partiesAndVisuals.Remove(mobileParty.Party);
			}
		}

		// Token: 0x040000BD RID: 189
		private const float DamageSoundCooldown = 2f;

		// Token: 0x040000BE RID: 190
		private static int _shipDamageSoundEventId = SoundManager.GetEventGlobalIndex("event:/ui/campaign/ship_damage");

		// Token: 0x040000BF RID: 191
		private readonly Dictionary<PartyBase, NavalMobilePartyVisual> _partiesAndVisuals = new Dictionary<PartyBase, NavalMobilePartyVisual>();

		// Token: 0x040000C0 RID: 192
		private readonly List<NavalMobilePartyVisual> _visualsFlattened = new List<NavalMobilePartyVisual>();

		// Token: 0x040000C1 RID: 193
		private int _dirtyPartyVisualCount;

		// Token: 0x040000C2 RID: 194
		private NavalMobilePartyVisual[] _dirtyPartiesList = new NavalMobilePartyVisual[2500];

		// Token: 0x040000C3 RID: 195
		private float _timeElapsedSinceLastShipDamageSoundPlayed;

		// Token: 0x040000C4 RID: 196
		private float _mainPartyPreviousShipDamageTriggerHealthPercent = 1f;

		// Token: 0x040000C5 RID: 197
		private readonly List<NavalMobilePartyVisual> _fadingPartiesFlatten = new List<NavalMobilePartyVisual>();

		// Token: 0x040000C6 RID: 198
		private readonly HashSet<NavalMobilePartyVisual> _fadingPartiesSet = new HashSet<NavalMobilePartyVisual>();

		// Token: 0x040000C7 RID: 199
		private readonly List<GameEntity> _bridgeEntityCache = new List<GameEntity>();
	}
}
