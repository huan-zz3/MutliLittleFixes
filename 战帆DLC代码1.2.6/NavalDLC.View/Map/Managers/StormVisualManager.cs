using System;
using System.Collections.Generic;
using NavalDLC.Map;
using NavalDLC.View.Map.Visuals;
using SandBox.View;
using SandBox.View.Map;
using SandBox.View.Map.Managers;
using SandBox.View.Map.Visuals;

namespace NavalDLC.View.Map.Managers
{
	// Token: 0x0200003A RID: 58
	public class StormVisualManager : EntityVisualManagerBase<Storm>
	{
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060001CD RID: 461 RVA: 0x0000DCDE File Offset: 0x0000BEDE
		public static StormVisualManager Current
		{
			get
			{
				return SandBoxViewSubModule.SandBoxViewVisualManager.GetEntityComponent<StormVisualManager>();
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060001CE RID: 462 RVA: 0x0000DCEA File Offset: 0x0000BEEA
		public override int Priority
		{
			get
			{
				return 80;
			}
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0000DCF0 File Offset: 0x0000BEF0
		public StormVisualManager()
		{
			this._allStormVisuals = new List<StormVisual>();
			NavalDLCEvents.OnStormCreatedEvent.AddNonSerializedListener(this, new Action<Storm>(this.StormCreated));
			foreach (Storm storm in NavalDLCManager.Instance.StormManager.SpawnedStorms)
			{
				StormVisual stormVisual = new StormVisual(storm);
				this._allStormVisuals.Add(stormVisual);
			}
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x0000DD80 File Offset: 0x0000BF80
		private void StormCreated(Storm storm)
		{
			this._allStormVisuals.Add(new StormVisual(storm));
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x0000DD94 File Offset: 0x0000BF94
		public override MapEntityVisual<Storm> GetVisualOfEntity(Storm entity)
		{
			foreach (StormVisual stormVisual in this._allStormVisuals)
			{
				if (stormVisual.MapEntity == entity)
				{
					return stormVisual;
				}
			}
			return null;
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x0000DDF0 File Offset: 0x0000BFF0
		public override void OnVisualTick(MapScreen screen, float realDt, float dt)
		{
			for (int i = this._allStormVisuals.Count - 1; i >= 0; i--)
			{
				StormVisual stormVisual = this._allStormVisuals[i];
				stormVisual.Tick();
				if (stormVisual.IsReadyToBeReleased)
				{
					this._allStormVisuals.RemoveAt(i);
				}
			}
		}

		// Token: 0x040000C8 RID: 200
		private readonly List<StormVisual> _allStormVisuals;
	}
}
