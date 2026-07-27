using System;
using NavalDLC.Map;
using SandBox;
using SandBox.View.Map.Visuals;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace NavalDLC.View.Map.Visuals
{
	// Token: 0x02000035 RID: 53
	public class StormVisual : MapEntityVisual<Storm>
	{
		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600018B RID: 395 RVA: 0x0000C9DC File Offset: 0x0000ABDC
		public override CampaignVec2 InteractionPositionForPlayer
		{
			get
			{
				return new CampaignVec2(base.MapEntity.CurrentPosition, true);
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600018C RID: 396 RVA: 0x0000C9EF File Offset: 0x0000ABEF
		public override MapEntityVisual AttachedTo
		{
			get
			{
				return null;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600018D RID: 397 RVA: 0x0000C9F2 File Offset: 0x0000ABF2
		public bool IsReadyToBeReleased
		{
			get
			{
				return base.MapEntity.IsReadyToBeFinalized;
			}
		}

		// Token: 0x0600018E RID: 398 RVA: 0x0000CA00 File Offset: 0x0000AC00
		public StormVisual(Storm storm)
			: base(storm)
		{
			this._mapScene = ((MapScene)Campaign.Current.MapSceneWrapper).Scene;
			this._visualState = StormVisual.StormVisualState.VisualNotInitialized;
			this._stormSoundEvent = SoundManager.CreateEvent("event:/map/ambient/node/hurricane", this._mapScene);
			this._stormSoundEvent.SetPosition(storm.CurrentPosition.ToVec3(0f));
			this._stormSoundEvent.SetParameter("StormIntensity", (float)storm.StormType);
		}

		// Token: 0x0600018F RID: 399 RVA: 0x0000CA80 File Offset: 0x0000AC80
		public override bool OnMapClick(bool followModifierUsed)
		{
			return false;
		}

		// Token: 0x06000190 RID: 400 RVA: 0x0000CA83 File Offset: 0x0000AC83
		public override void OnHover()
		{
		}

		// Token: 0x06000191 RID: 401 RVA: 0x0000CA85 File Offset: 0x0000AC85
		public override void OnOpenEncyclopedia()
		{
		}

		// Token: 0x06000192 RID: 402 RVA: 0x0000CA87 File Offset: 0x0000AC87
		public override bool IsVisibleOrFadingOut()
		{
			return base.MapEntity.IsActive;
		}

		// Token: 0x06000193 RID: 403 RVA: 0x0000CA94 File Offset: 0x0000AC94
		public override Vec3 GetVisualPosition()
		{
			return this.InteractionPositionForPlayer.AsVec3();
		}

		// Token: 0x06000194 RID: 404 RVA: 0x0000CAB0 File Offset: 0x0000ACB0
		public void Tick()
		{
			StormVisual.StormVisualState stormVisualState = this.GetStormVisualState(base.MapEntity);
			if (this._visualState != stormVisualState)
			{
				this.UpdateVisualState(stormVisualState);
			}
			if (this.VisualEntity != null)
			{
				this.VisualTick();
			}
			base.MapEntity.OnVisualUpdated();
		}

		// Token: 0x06000195 RID: 405 RVA: 0x0000CAFC File Offset: 0x0000ACFC
		private void VisualTick()
		{
			Vec3 vec;
			vec..ctor(base.MapEntity.CurrentPosition, 0f, -1f);
			this.VisualEntity.SetLocalPosition(vec);
			this._stormSoundEvent.SetPosition(this.VisualEntity.GlobalPosition);
		}

		// Token: 0x06000196 RID: 406 RVA: 0x0000CB48 File Offset: 0x0000AD48
		private void UpdateVisualState(StormVisual.StormVisualState newState)
		{
			if (this.VisualEntity != null)
			{
				this._mapScene.RemoveEntity(this.VisualEntity, 0);
				this.VisualEntity = null;
			}
			this._visualState = newState;
			switch (newState)
			{
			case StormVisual.StormVisualState.Developing:
				if (NavalDLCManager.Instance.StormManager.DebugVisualsEnabled)
				{
					this.VisualEntity = GameEntity.Instantiate(this._mapScene, "editor_cube", MatrixFrame.Identity, true);
					return;
				}
				break;
			case StormVisual.StormVisualState.Active:
				this._stormSoundEvent.Play();
				switch (base.MapEntity.StormType)
				{
				case Storm.StormTypes.Storm:
					this.VisualEntity = GameEntity.Instantiate(this._mapScene, "psys_mapicon_lightclouds", MatrixFrame.Identity, true);
					break;
				case Storm.StormTypes.ThunderStorm:
					this.VisualEntity = GameEntity.Instantiate(this._mapScene, "psys_mapicon_darkclouds", MatrixFrame.Identity, true);
					break;
				case Storm.StormTypes.Hurricane:
					this.VisualEntity = GameEntity.Instantiate(this._mapScene, "psys_mapicon_typhoon", MatrixFrame.Identity, true);
					break;
				}
				this._visualState = StormVisual.StormVisualState.Active;
				return;
			case StormVisual.StormVisualState.Finalizing:
				this._stormSoundEvent.Stop();
				if (NavalDLCManager.Instance.StormManager.DebugVisualsEnabled)
				{
					this.VisualEntity = GameEntity.Instantiate(this._mapScene, "editor_cube", MatrixFrame.Identity, true);
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06000197 RID: 407 RVA: 0x0000CC8D File Offset: 0x0000AE8D
		private StormVisual.StormVisualState GetStormVisualState(Storm storm)
		{
			if (storm.IsReadyToBeFinalized)
			{
				return StormVisual.StormVisualState.ReadyToBeReleased;
			}
			if (storm.IsActive)
			{
				return StormVisual.StormVisualState.Active;
			}
			if (storm.IsInDevelopingState)
			{
				return StormVisual.StormVisualState.Developing;
			}
			if (storm.IsInFinalizingState)
			{
				return StormVisual.StormVisualState.Finalizing;
			}
			return StormVisual.StormVisualState.VisualNotInitialized;
		}

		// Token: 0x040000B1 RID: 177
		public const int DefaultStormVisualHeight = 0;

		// Token: 0x040000B2 RID: 178
		private StormVisual.StormVisualState _visualState;

		// Token: 0x040000B3 RID: 179
		private SoundEvent _stormSoundEvent;

		// Token: 0x040000B4 RID: 180
		public GameEntity VisualEntity;

		// Token: 0x040000B5 RID: 181
		private Scene _mapScene;

		// Token: 0x02000053 RID: 83
		private enum StormVisualState
		{
			// Token: 0x0400012C RID: 300
			VisualNotInitialized,
			// Token: 0x0400012D RID: 301
			Developing,
			// Token: 0x0400012E RID: 302
			Active,
			// Token: 0x0400012F RID: 303
			Finalizing,
			// Token: 0x04000130 RID: 304
			ReadyToBeReleased
		}
	}
}
