using System;
using NavalDLC.View.Map.Visuals;
using SandBox.View;
using SandBox.View.Map;
using SandBox.View.Map.Managers;
using SandBox.View.Map.Visuals;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace NavalDLC.View.Map.Managers
{
	// Token: 0x02000038 RID: 56
	public class AnchorVisualManager : EntityVisualManagerBase<AnchorPoint>
	{
		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060001AA RID: 426 RVA: 0x0000CFA6 File Offset: 0x0000B1A6
		public static AnchorVisualManager Current
		{
			get
			{
				return SandBoxViewSubModule.SandBoxViewVisualManager.GetEntityComponent<AnchorVisualManager>();
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060001AB RID: 427 RVA: 0x0000CFB2 File Offset: 0x0000B1B2
		public override int Priority
		{
			get
			{
				return 30;
			}
		}

		// Token: 0x060001AD RID: 429 RVA: 0x0000CFBE File Offset: 0x0000B1BE
		public override MapEntityVisual<AnchorPoint> GetVisualOfEntity(AnchorPoint entity)
		{
			return this._anchorVisual;
		}

		// Token: 0x060001AE RID: 430 RVA: 0x0000CFC8 File Offset: 0x0000B1C8
		protected override void OnInitialize()
		{
			base.OnInitialize();
			if (this.CanPartyHaveAnchor())
			{
				if (this._anchorVisual == null)
				{
					this.CreateNewVisual();
				}
				else
				{
					this._anchorVisual.OnVisualUpdate();
				}
			}
			this._anchorCircleDecal = MapScreen.DecalEntity.Create(base.MapScene, "decal_city_circle_a", "TownCircle");
		}

		// Token: 0x060001AF RID: 431 RVA: 0x0000D01C File Offset: 0x0000B21C
		public override bool OnVisualIntersected(Ray mouseRay, UIntPtr[] intersectedEntityIDs, Intersection[] intersectionInfos, int entityCount, Vec3 worldMouseNear, Vec3 worldMouseFar, Vec3 terrainIntersectionPoint, ref MapEntityVisual hoveredVisual, ref MapEntityVisual selectedVisual)
		{
			for (int i = entityCount - 1; i >= 0; i--)
			{
				UIntPtr uintPtr = intersectedEntityIDs[i];
				MapEntityVisual mapEntityVisual;
				if (uintPtr != UIntPtr.Zero && MapScreen.VisualsOfEntities.TryGetValue(uintPtr, out mapEntityVisual) && mapEntityVisual is AnchorVisual && mapEntityVisual.IsVisibleOrFadingOut())
				{
					hoveredVisual = mapEntityVisual;
					selectedVisual = mapEntityVisual;
				}
			}
			return selectedVisual != null;
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x0000D078 File Offset: 0x0000B278
		public override void OnVisualTick(MapScreen screen, float realDt, float dt)
		{
			bool flag = false;
			MatrixFrame identity = MatrixFrame.Identity;
			if (this._anchorVisual != null && ((MobileParty.MainParty.Ai.AiBehaviorInteractable != null && MobileParty.MainParty.Ai.AiBehaviorInteractable is AnchorPoint) || (MapScreen.Instance.CurrentVisualOfTooltip != null && MapScreen.Instance.CurrentVisualOfTooltip is AnchorVisual)))
			{
				flag = true;
				identity.origin = this._anchorVisual.GetVisualPosition();
			}
			this._anchorCircleDecal.GameEntity.SetVisibilityExcludeParents(flag);
			if (flag)
			{
				this._anchorCircleDecal.Decal.SetVectorArgument(1f, 1f, 0f, 0f);
				this._anchorCircleDecal.Decal.SetFactor1Linear(4291596077U);
				this._anchorCircleDecal.GameEntity.SetGlobalFrame(ref identity, true);
			}
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0000D150 File Offset: 0x0000B350
		public override void OnTick(float realDt, float dt)
		{
			base.OnTick(realDt, dt);
			AnchorVisual anchorVisual = this._anchorVisual;
			bool flag = ((anchorVisual != null) ? anchorVisual.Entity : null) != null && this._anchorVisual.Entity.IsVisibleIncludeParents() && (this._anchorVisual.MapEntity != MobileParty.MainParty.Anchor || !MobileParty.MainParty.IsActive || MobileParty.MainParty.Anchor.IsDisabled);
			if (this._anchorVisual != null && (flag || PartyBase.MainParty.Ships.Count == 0))
			{
				this.RemoveAnchorVisual();
			}
			if (this.CanPartyHaveAnchor())
			{
				if (this._anchorVisual != null)
				{
					this.UpdateAnchorVisual();
				}
				else
				{
					this.CreateNewVisual();
				}
			}
			if (this._cachedPosition != MobileParty.MainParty.Anchor.Position && (this._cachedPosition.IsValid() || MobileParty.MainParty.Anchor.IsValid) && !MobileParty.MainParty.Anchor.IsDisabled)
			{
				this.OnAnchorPositionUpdated();
				this._cachedPosition = MobileParty.MainParty.Anchor.Position;
			}
			if (this._cachedDisabledValue.Item1 != MobileParty.MainParty.Anchor.IsDisabled || this._cachedDisabledValue.Item2 != MobileParty.MainParty.IsActive)
			{
				this.OnAnchorPositionUpdated();
				this._cachedDisabledValue = new ValueTuple<bool, bool>(MobileParty.MainParty.Anchor.IsDisabled, MobileParty.MainParty.IsActive);
			}
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x0000D2CD File Offset: 0x0000B4CD
		internal void OnAnchorPositionUpdated()
		{
			if (this._anchorVisual == null)
			{
				if (this.CanPartyHaveAnchor())
				{
					this.CreateNewVisual();
				}
				return;
			}
			if (this.CanPartyHaveAnchor())
			{
				this._anchorVisual.UpdateAnchorVisualPosition();
				return;
			}
			this.RemoveAnchorVisual();
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x0000D300 File Offset: 0x0000B500
		private bool CanPartyHaveAnchor()
		{
			return !MobileParty.MainParty.IsCurrentlyAtSea && MobileParty.MainParty.IsActive && MobileParty.MainParty.Anchor.IsValid && MobileParty.MainParty.HasNavalNavigationCapability && !MobileParty.MainParty.Anchor.IsDisabled;
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x0000D358 File Offset: 0x0000B558
		private void CreateNewVisual()
		{
			this._anchorVisual = new AnchorVisual(MobileParty.MainParty.Anchor);
			this._anchorVisual.OnStartup();
			this._cachedPosition = this._anchorVisual.MapEntity.Position;
			MapScreen.VisualsOfEntities.Add(this._anchorVisual.Entity.Pointer, this._anchorVisual);
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x0000D3BB File Offset: 0x0000B5BB
		private void RemoveAnchorVisual()
		{
			MapScreen.VisualsOfEntities.Remove(this._anchorVisual.Entity.Pointer);
			this._anchorVisual.OnRemoved();
			this._cachedPosition = CampaignVec2.Invalid;
			this._anchorVisual = null;
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x0000D3F8 File Offset: 0x0000B5F8
		private void UpdateAnchorVisual()
		{
			this._anchorVisual.OnVisualUpdate();
			AnchorVisual anchorVisual = this._anchorVisual;
			if (((anchorVisual != null) ? anchorVisual.Entity : null) != null && !MapScreen.VisualsOfEntities.ContainsKey(this._anchorVisual.Entity.Pointer))
			{
				MapScreen.VisualsOfEntities.Add(this._anchorVisual.Entity.Pointer, this._anchorVisual);
			}
		}

		// Token: 0x040000B7 RID: 183
		private const float DecalEntityHeight = 1f;

		// Token: 0x040000B8 RID: 184
		private const uint DecalColor = 4291596077U;

		// Token: 0x040000B9 RID: 185
		private AnchorVisual _anchorVisual;

		// Token: 0x040000BA RID: 186
		private MapScreen.DecalEntity _anchorCircleDecal;

		// Token: 0x040000BB RID: 187
		private CampaignVec2 _cachedPosition;

		// Token: 0x040000BC RID: 188
		private ValueTuple<bool, bool> _cachedDisabledValue;
	}
}
