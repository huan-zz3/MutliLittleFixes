using System;
using MissionLibrary.Repository;
using MissionLibrary.Usage;
using MissionLibrary.View;
using MissionSharedLibrary.Config;
using TaleWorlds.MountAndBlade;

namespace MissionSharedLibrary.Controller.MissionBehaviors
{
	// Token: 0x0200003A RID: 58
	public class MissionLibraryLogic : MissionLogic
	{
		// Token: 0x06000206 RID: 518 RVA: 0x000079FC File Offset: 0x00005BFC
		public override void OnTeamDeployed(Team team)
		{
			base.OnTeamDeployed(team);
			if (team == base.Mission.PlayerTeam && !this._config.HasUsageShown && ARepository<AUsageCategoryManager, AUsageCategory>.Get().Items.Count > 0)
			{
				this._config.HasUsageShown = true;
				this._config.Serialize();
				AMenuManager amenuManager = AMenuManager.Get();
				if (amenuManager == null)
				{
					return;
				}
				amenuManager.RequestToOpenUsageView();
			}
		}

		// Token: 0x06000207 RID: 519 RVA: 0x00007A64 File Offset: 0x00005C64
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (this._showUsageHintAfterTimeUp)
			{
				this._timerForShowingUsageHint -= dt;
				if (this._timerForShowingUsageHint < 0f)
				{
					AMenuManager amenuManager = AMenuManager.Get();
					if (amenuManager != null)
					{
						amenuManager.RequestToOpenUsageView();
					}
					this._showUsageHintAfterTimeUp = false;
					this._timerForShowingUsageHint = 0f;
				}
			}
		}

		// Token: 0x040000D1 RID: 209
		private GeneralConfig _config = MissionConfigBase<GeneralConfig>.Get();

		// Token: 0x040000D2 RID: 210
		private bool _showUsageHintAfterTimeUp;

		// Token: 0x040000D3 RID: 211
		private float _timerForShowingUsageHint;
	}
}
