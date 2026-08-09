using System;
using NavalDLC.Storyline;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace NavalDLC.View.MissionViews.Storyline
{
	// Token: 0x02000028 RID: 40
	public class HelpingAnAllyMissionView : MissionView
	{
		// Token: 0x060000FD RID: 253 RVA: 0x00007DC4 File Offset: 0x00005FC4
		public override void OnBehaviorInitialize()
		{
			this._controller = base.Mission.GetMissionBehavior<HelpingAnAllySetPieceBattleMissionController>();
			if (this._controller != null)
			{
				HelpingAnAllySetPieceBattleMissionController controller = this._controller;
				controller.OnShipsInitializedEvent = (Action)Delegate.Combine(controller.OnShipsInitializedEvent, new Action(this.OnShipsInitialized));
				HelpingAnAllySetPieceBattleMissionController controller2 = this._controller;
				controller2.OnDefeatedEvent = (Action<float>)Delegate.Combine(controller2.OnDefeatedEvent, new Action<float>(this.OnDefeated));
			}
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00007E38 File Offset: 0x00006038
		private void OnDefeated(float duration)
		{
			ScreenFadeController.BeginFadeOut(duration);
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00007E40 File Offset: 0x00006040
		private void OnShipsInitialized()
		{
			this.OnShipsInitializedInternal();
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00007E48 File Offset: 0x00006048
		protected virtual void OnShipsInitializedInternal()
		{
		}

		// Token: 0x04000061 RID: 97
		private HelpingAnAllySetPieceBattleMissionController _controller;
	}
}
