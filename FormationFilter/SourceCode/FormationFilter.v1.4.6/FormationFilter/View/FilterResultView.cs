using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FormationFilter.View.ViewModels;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace FormationFilter.View
{
	// Token: 0x02000007 RID: 7
	[NullableContext(2)]
	[Nullable(0)]
	[DefaultView]
	public class FilterResultView : MissionView
	{
		// Token: 0x0600000E RID: 14 RVA: 0x0000229E File Offset: 0x0000049E
		public FilterResultView()
		{
			this._dataSource = new FormationFilterResultVM();
			this.ViewOrderPriority = 14;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000022BC File Offset: 0x000004BC
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			string text = "FormationFilterResult";
			this._gauntletLayer = new GauntletLayer(text, this.ViewOrderPriority, false);
			this._movie = this._gauntletLayer.LoadMovie(text, this._dataSource);
			base.MissionScreen.AddLayer(this._gauntletLayer);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002311 File Offset: 0x00000511
		public override void OnAfterDeploymentFinished()
		{
			base.OnAfterDeploymentFinished();
			FormationFilterResultVM dataSource = this._dataSource;
			if (dataSource != null)
			{
				dataSource.OnFinalize();
			}
			this._dataSource = null;
			base.MissionScreen.RemoveLayer(this._gauntletLayer);
			this._gauntletLayer = null;
			this._movie = null;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002350 File Offset: 0x00000550
		[NullableContext(1)]
		public void SetResult(List<Agent> agents)
		{
			if (this._dataSource != null)
			{
				this._dataSource.SetResult(agents);
			}
		}

		// Token: 0x04000008 RID: 8
		private FormationFilterResultVM _dataSource;

		// Token: 0x04000009 RID: 9
		private GauntletLayer _gauntletLayer;

		// Token: 0x0400000A RID: 10
		private GauntletMovieIdentifier _movie;
	}
}
