using System;
using NavalDLC.View.Map;
using NavalDLC.ViewModelCollection.Map;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.ScreenSystem;

namespace NavalDLC.GauntletUI.Map
{
	// Token: 0x02000020 RID: 32
	[OverrideView(typeof(NavalMapAnchorTrackerView))]
	public class GauntletNavalMapAnchorTrackerView : MapView
	{
		// Token: 0x060000F8 RID: 248 RVA: 0x00009EBC File Offset: 0x000080BC
		protected override void OnMapConversationStart()
		{
			base.OnMapConversationStart();
			if (this._gauntletLayer != null)
			{
				ScreenManager.SetSuspendLayer(this._gauntletLayer, true);
			}
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00009ED8 File Offset: 0x000080D8
		protected override void OnMapConversationOver()
		{
			base.OnMapConversationOver();
			if (this._gauntletLayer != null)
			{
				ScreenManager.SetSuspendLayer(this._gauntletLayer, false);
			}
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00009EF4 File Offset: 0x000080F4
		protected override void CreateLayout()
		{
			base.CreateLayout();
			this._dataSource = new MapAnchorTrackerVM(new Action(this.OnMoveCameraToAnchor));
			this._gauntletLayer = new GauntletLayer("NavalAnchorTracker", 15, false);
			base.Layer = this._gauntletLayer;
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(false, 3);
			this._gauntletLayer.LoadMovie("AnchorTracker", this._dataSource);
			base.MapScreen.AddLayer(base.Layer);
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00009F78 File Offset: 0x00008178
		private void OnMoveCameraToAnchor()
		{
			MobileParty mainParty = MobileParty.MainParty;
			bool flag;
			if (mainParty == null)
			{
				flag = false;
			}
			else
			{
				AnchorPoint anchor = mainParty.Anchor;
				bool? flag2 = ((anchor != null) ? new bool?(anchor.IsValid) : null);
				bool flag3 = true;
				flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
			}
			if (flag)
			{
				base.MapScreen.FastMoveCameraToPosition(MobileParty.MainParty.Anchor.Position);
			}
		}

		// Token: 0x060000FC RID: 252 RVA: 0x00009FE0 File Offset: 0x000081E0
		protected override void OnFinalize()
		{
			base.OnFinalize();
			this._dataSource.OnFinalize();
			base.MapScreen.RemoveLayer(base.Layer);
		}

		// Token: 0x060000FD RID: 253 RVA: 0x0000A004 File Offset: 0x00008204
		protected override void OnMapScreenUpdate(float dt)
		{
			base.OnMapScreenUpdate(dt);
			AnchorPoint anchor = MobileParty.MainParty.Anchor;
			float seeingRange = MobileParty.MainParty.SeeingRange;
			float num = anchor.Position.Distance(MobileParty.MainParty.Position);
			float num2 = base.MapScreen.MapCameraView.Camera.Position.Distance(MobileParty.MainParty.GetPositionAsVec3());
			float num3 = -5000f;
			float num4 = -5000f;
			float num5 = -5000f;
			if (anchor != null && anchor.IsValid && !anchor.IsDisabled && (num > seeingRange || num2 >= 110f))
			{
				this.GetAnchorScreenPosition(anchor, out num3, out num4, out num5);
			}
			this._dataSource.IsVisible = num5 >= 0f;
			this._dataSource.PositionX = num3;
			this._dataSource.PositionY = num4;
			this._dataSource.PositionW = num5;
		}

		// Token: 0x060000FE RID: 254 RVA: 0x0000A0F0 File Offset: 0x000082F0
		private void GetAnchorScreenPosition(AnchorPoint anchor, out float screenX, out float screenY, out float screenW)
		{
			Vec3 position = anchor.GetPosition();
			screenX = -5000f;
			screenY = -5000f;
			screenW = -1f;
			MBWindowManager.WorldToScreenInsideUsableArea(base.MapScreen.MapCameraView.Camera, position, ref screenX, ref screenY, ref screenW);
		}

		// Token: 0x0400008E RID: 142
		private GauntletLayer _gauntletLayer;

		// Token: 0x0400008F RID: 143
		private MapAnchorTrackerVM _dataSource;
	}
}
