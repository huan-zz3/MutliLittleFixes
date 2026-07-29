using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FormationFilter.Models;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;

namespace FormationFilter.Logics
{
	// Token: 0x0200001F RID: 31
	[NullableContext(1)]
	[Nullable(0)]
	public class FormationFilterLogic : MissionLogic
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x0600011D RID: 285 RVA: 0x00008E08 File Offset: 0x00007008
		// (remove) Token: 0x0600011E RID: 286 RVA: 0x00008E3C File Offset: 0x0000703C
		[Nullable(new byte[] { 2, 1 })]
		[field: Nullable(new byte[] { 2, 1 })]
		public static event Action<TeamFilter> OnTeamFilterConfigurationLoaded;

		// Token: 0x06000120 RID: 288 RVA: 0x00008E82 File Offset: 0x00007082
		public static void TeamFilterConfigurationLoaded(TeamFilter teamFilter)
		{
			Action<TeamFilter> onTeamFilterConfigurationLoaded = FormationFilterLogic.OnTeamFilterConfigurationLoaded;
			if (onTeamFilterConfigurationLoaded == null)
			{
				return;
			}
			onTeamFilterConfigurationLoaded(teamFilter);
		}

		// Token: 0x06000121 RID: 289 RVA: 0x00008E94 File Offset: 0x00007094
		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
			FormationFilterLogic.OnTeamFilterConfigurationLoaded = null;
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00008EA2 File Offset: 0x000070A2
		public override void AfterAddTeam(Team team)
		{
			base.AfterAddTeam(team);
			if (team == base.Mission.PlayerTeam)
			{
				this.TryAddTeam(team);
			}
		}

		// Token: 0x06000123 RID: 291 RVA: 0x00008EC0 File Offset: 0x000070C0
		public override void OnTeamDeployed(Team team)
		{
			base.OnTeamDeployed(team);
			if (team == base.Mission.PlayerTeam)
			{
				this.TryAddTeam(team);
			}
		}

		// Token: 0x06000124 RID: 292 RVA: 0x00008EDE File Offset: 0x000070DE
		[return: Nullable(2)]
		public TeamFilter GetTeamFilter(Team team)
		{
			if (!this._teamFilters.ContainsKey(team))
			{
				return null;
			}
			return this._teamFilters[team];
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00008EFC File Offset: 0x000070FC
		private void TryAddTeam(Team team)
		{
			if (!this._teamFilters.ContainsKey(team))
			{
				TeamFilter teamFilter = new TeamFilter();
				this._teamFilters[team] = teamFilter;
			}
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00008F2C File Offset: 0x0000712C
		public override void OnAfterDeploymentFinished()
		{
			base.OnAfterDeploymentFinished();
			foreach (KeyValuePair<Team, TeamFilter> keyValuePair in this._teamFilters)
			{
				Team key = keyValuePair.Key;
				TeamFilter value = keyValuePair.Value;
			}
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00008F90 File Offset: 0x00007190
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (Input.IsKeyPressed(48))
			{
				foreach (KeyValuePair<Team, TeamFilter> keyValuePair in this._teamFilters)
				{
					Team key = keyValuePair.Key;
					TeamFilter value = keyValuePair.Value;
				}
			}
		}

		// Token: 0x04000087 RID: 135
		private Dictionary<Team, TeamFilter> _teamFilters = new Dictionary<Team, TeamFilter>();
	}
}
