using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Handlers;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace NavalDLC.Map
{
	// Token: 0x020000FD RID: 253
	public class StormManager : ICustomSystemManager
	{
		// Token: 0x17000337 RID: 823
		// (get) Token: 0x060012C2 RID: 4802 RVA: 0x00089954 File Offset: 0x00087B54
		public MBReadOnlyList<Storm> SpawnedStorms
		{
			get
			{
				return this._spawnedStorms;
			}
		}

		// Token: 0x060012C3 RID: 4803 RVA: 0x0008995C File Offset: 0x00087B5C
		public StormManager()
		{
			CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(this.CampaignTick));
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
		}

		// Token: 0x060012C4 RID: 4804 RVA: 0x000899A8 File Offset: 0x00087BA8
		private void HourlyTick()
		{
			for (int i = 0; i < this._spawnedStorms.Count; i++)
			{
				this._spawnedStorms[i].HourlyTick();
			}
		}

		// Token: 0x060012C5 RID: 4805 RVA: 0x000899DC File Offset: 0x00087BDC
		private void CampaignTick(float campaignDt)
		{
			if (campaignDt > 0f)
			{
				for (int i = this._spawnedStorms.Count - 1; i >= 0; i--)
				{
					Storm storm = this._spawnedStorms[i];
					if (storm.IsReadyToBeFinalized)
					{
						storm.SetVisualDirty();
						this._spawnedStorms.RemoveAt(i);
					}
					else
					{
						storm.Tick(campaignDt);
					}
				}
				this.StormCollisionTick();
			}
		}

		// Token: 0x060012C6 RID: 4806 RVA: 0x00089A40 File Offset: 0x00087C40
		private void StormCollisionTick()
		{
			for (int i = 0; i < this._spawnedStorms.Count; i++)
			{
				for (int j = i + 1; j < this._spawnedStorms.Count; j++)
				{
					Storm storm = this._spawnedStorms[i];
					Storm storm2 = this._spawnedStorms[j];
					if (storm.CurrentPosition.Distance(storm2.CurrentPosition) < storm.EffectRadius + storm2.EffectRadius)
					{
						((storm.EffectRadius > storm2.EffectRadius) ? storm2 : storm).ForceDeactivate();
					}
				}
			}
		}

		// Token: 0x060012C7 RID: 4807 RVA: 0x00089AD0 File Offset: 0x00087CD0
		public void CreateStormAtPosition(Vec2 position)
		{
			Storm storm = new Storm(position, NavalDLCManager.Instance.GameModels.MapStormModel.GetSpawnedStormTypeForPosition(position));
			this._spawnedStorms.Add(storm);
			NavalDLCEvents.Instance.OnStormCreated(storm);
		}

		// Token: 0x060012C8 RID: 4808 RVA: 0x00089B10 File Offset: 0x00087D10
		public void CreateStormAtPosition(Vec2 position, Storm.StormTypes stormType)
		{
			Storm storm = new Storm(position, stormType);
			this._spawnedStorms.Add(storm);
			NavalDLCEvents.Instance.OnStormCreated(storm);
		}

		// Token: 0x060012C9 RID: 4809 RVA: 0x00089B3C File Offset: 0x00087D3C
		public void OnAfterLoad()
		{
			CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(this.CampaignTick));
			CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, new Action(this.HourlyTick));
			for (int i = 0; i < this._spawnedStorms.Count; i++)
			{
				this._spawnedStorms[i].OnAfterLoad();
			}
		}

		// Token: 0x04000A97 RID: 2711
		[SaveableField(10)]
		private MBList<Storm> _spawnedStorms = new MBList<Storm>();

		// Token: 0x04000A98 RID: 2712
		public bool DebugVisualsEnabled;

		// Token: 0x04000A99 RID: 2713
		public bool DebugVisualsStopped;
	}
}
