using System;
using System.Collections.Generic;
using NavalDLC.Missions.Objects;
using SandBox;
using SandBox.View.Map;
using SandBox.View.Map.Visuals;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace NavalDLC.View.Map.Visuals
{
	// Token: 0x02000033 RID: 51
	public class AnchorVisual : MapEntityVisual<AnchorPoint>
	{
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000140 RID: 320 RVA: 0x000092F1 File Offset: 0x000074F1
		public override CampaignVec2 InteractionPositionForPlayer
		{
			get
			{
				return base.MapEntity.GetInteractionPosition(MobileParty.MainParty);
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000141 RID: 321 RVA: 0x00009303 File Offset: 0x00007503
		public override MapEntityVisual AttachedTo
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000142 RID: 322 RVA: 0x00009306 File Offset: 0x00007506
		// (set) Token: 0x06000143 RID: 323 RVA: 0x0000930E File Offset: 0x0000750E
		public GameEntity Entity { get; private set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000144 RID: 324 RVA: 0x00009318 File Offset: 0x00007518
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

		// Token: 0x06000145 RID: 325 RVA: 0x00009366 File Offset: 0x00007566
		public AnchorVisual(AnchorPoint mapEntity)
			: base(mapEntity)
		{
		}

		// Token: 0x06000146 RID: 326 RVA: 0x0000937C File Offset: 0x0000757C
		public override Vec3 GetVisualPosition()
		{
			return base.MapEntity.Position.AsVec3();
		}

		// Token: 0x06000147 RID: 327 RVA: 0x0000939C File Offset: 0x0000759C
		public override bool IsVisibleOrFadingOut()
		{
			return !base.MapEntity.Owner.IsTransitionInProgress;
		}

		// Token: 0x06000148 RID: 328 RVA: 0x000093B1 File Offset: 0x000075B1
		public override void OnHover()
		{
			InformationManager.ShowTooltip(typeof(AnchorPoint), new object[] { base.MapEntity });
		}

		// Token: 0x06000149 RID: 329 RVA: 0x000093D1 File Offset: 0x000075D1
		public override bool OnMapClick(bool followModifierUsed)
		{
			MobileParty.MainParty.SetMoveGoToInteractablePoint(base.MapEntity, 3);
			return true;
		}

		// Token: 0x0600014A RID: 330 RVA: 0x000093E5 File Offset: 0x000075E5
		public override void ReleaseResources()
		{
		}

		// Token: 0x0600014B RID: 331 RVA: 0x000093E7 File Offset: 0x000075E7
		public override void OnOpenEncyclopedia()
		{
		}

		// Token: 0x0600014C RID: 332 RVA: 0x000093E9 File Offset: 0x000075E9
		public void OnStartup()
		{
			if (this.Entity != null)
			{
				this.OnVisualUpdate();
				return;
			}
			this.RefreshGameEntity();
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00009408 File Offset: 0x00007608
		public void OnRemoved()
		{
			if (PartyBase.MainParty.Ships.Count > 0)
			{
				this.Entity.SetVisibilityExcludeParents(false);
				return;
			}
			base.MapEntity.ResetPosition();
			GameEntity entity = this.Entity;
			if (entity != null)
			{
				entity.Remove(111);
			}
			this.Entity = null;
			this.ResetVersionCache();
			this._sailVisuals.Clear();
		}

		// Token: 0x0600014E RID: 334 RVA: 0x0000946C File Offset: 0x0000766C
		public void OnVisualUpdate()
		{
			Ship flagShip = PartyBase.MainParty.FlagShip;
			if (this._flagshipHull == null || this._flagshipHull != flagShip.ShipHull)
			{
				if (this.Entity != null)
				{
					MapScreen.VisualsOfEntities.Remove(this.Entity.Pointer);
					GameEntity entity = this.Entity;
					if (entity != null)
					{
						entity.Remove(111);
					}
				}
				this.RefreshGameEntity();
				return;
			}
			if (flagShip.VersionNo != this._cachedVersion)
			{
				this.UpdateVersionCache();
				NavalDLCViewHelpers.ShipVisualHelper.RefreshShipVisuals(this.Entity.WeakEntity, flagShip, this._sailVisuals);
			}
		}

		// Token: 0x0600014F RID: 335 RVA: 0x00009504 File Offset: 0x00007704
		private void UpdateVersionCache()
		{
			Ship flagShip = PartyBase.MainParty.FlagShip;
			this._cachedVersion = flagShip.VersionNo;
			this._flagshipHull = flagShip.ShipHull;
		}

		// Token: 0x06000150 RID: 336 RVA: 0x00009534 File Offset: 0x00007734
		private void ResetVersionCache()
		{
			this._flagshipHull = null;
			this._cachedVersion = 0U;
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00009544 File Offset: 0x00007744
		private void RefreshGameEntity()
		{
			this.UpdateVersionCache();
			this.Entity = NavalDLCViewHelpers.ShipVisualHelper.GetFlagshipEntity(PartyBase.MainParty, this.MapScene);
			NavalDLCViewHelpers.ShipVisualHelper.CollectSailVisuals(this.Entity.WeakEntity, this._sailVisuals);
			this.Entity.SetVisibilityExcludeParents(false);
			GameEntityPhysicsExtensions.AddSphereAsBody(this.Entity, new Vec3(0f, 0f, 0f, -1f), 3f, 144);
			this.UpdateAnchorVisualPosition();
		}

		// Token: 0x06000152 RID: 338 RVA: 0x000095C4 File Offset: 0x000077C4
		internal void UpdateAnchorVisualPosition()
		{
			MatrixFrame matrixFrame = this.CalculateAnchorFrame(base.MapEntity);
			this.Entity.SetFrame(ref matrixFrame, true);
			this.Entity.SetVisibilityExcludeParents(true);
		}

		// Token: 0x06000153 RID: 339 RVA: 0x000095F8 File Offset: 0x000077F8
		private MatrixFrame CalculateAnchorFrame(AnchorPoint anchor)
		{
			Vec2 vec = (anchor.GetInteractionPosition(anchor.Owner).ToVec2() - anchor.Position.ToVec2()).Normalized();
			Vec3 localScale = this.Entity.GetLocalScale();
			MatrixFrame identity = MatrixFrame.Identity;
			identity.origin = this.GetVisualPosition();
			identity.rotation.f.AsVec2 = vec.RightVec();
			identity.rotation.f.NormalizeWithoutChangingZ();
			identity.rotation.Orthonormalize();
			identity.rotation.ApplyScaleLocal(ref localScale);
			return identity;
		}

		// Token: 0x06000154 RID: 340 RVA: 0x0000969A File Offset: 0x0000789A
		private bool CanHaveAnchor()
		{
			return base.MapEntity.Owner.HasNavalNavigationCapability && base.MapEntity.IsValid && !base.MapEntity.IsDisabled;
		}

		// Token: 0x04000084 RID: 132
		private ShipHull _flagshipHull;

		// Token: 0x04000085 RID: 133
		private uint _cachedVersion;

		// Token: 0x04000086 RID: 134
		private List<SailVisual> _sailVisuals = new List<SailVisual>();

		// Token: 0x04000087 RID: 135
		private Scene _mapScene;
	}
}
