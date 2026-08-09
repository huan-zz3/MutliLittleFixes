using System;
using System.Collections.Generic;
using System.Linq;
using SandBox;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace NavalDLC.View
{
	// Token: 0x02000008 RID: 8
	public class NavalMapSceneWrapper : INavalMapSceneWrapper
	{
		// Token: 0x06000040 RID: 64 RVA: 0x000033E2 File Offset: 0x000015E2
		public NavalMapSceneWrapper()
		{
			this._mapScene = (MapScene)Campaign.Current.MapSceneWrapper;
			this.InitializePirateSpawnPoints();
			this.InitializeDropOffLocations();
			this.InitializeMapWaterWake();
		}

		// Token: 0x06000041 RID: 65 RVA: 0x0000341C File Offset: 0x0000161C
		public void Tick(float dt)
		{
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00003420 File Offset: 0x00001620
		private void InitializePirateSpawnPoints()
		{
			List<GameEntity> list = new List<GameEntity>();
			this._mapScene.Scene.GetAllEntitiesWithScriptComponent<PirateSpawnPoint>(ref list);
			for (int i = 0; i < list.Count; i++)
			{
				PirateSpawnPoint firstScriptOfType = list[i].GetFirstScriptOfType<PirateSpawnPoint>();
				string clanStringId = firstScriptOfType.ClanStringId;
				List<ValueTuple<CampaignVec2, float>> list2;
				if (!this._pirateSpawnPoints.TryGetValue(clanStringId, out list2))
				{
					this._pirateSpawnPoints[clanStringId] = new List<ValueTuple<CampaignVec2, float>>();
				}
				CampaignVec2 campaignVec;
				campaignVec..ctor(firstScriptOfType.GetPosition(), false);
				this._pirateSpawnPoints[clanStringId].Add(new ValueTuple<CampaignVec2, float>(campaignVec, firstScriptOfType.Radius));
			}
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000034B8 File Offset: 0x000016B8
		public List<ValueTuple<CampaignVec2, float>> GetSpawnPoints(string stringId)
		{
			List<ValueTuple<CampaignVec2, float>> list;
			if (this._pirateSpawnPoints.TryGetValue(stringId, out list))
			{
				return list;
			}
			return new List<ValueTuple<CampaignVec2, float>>();
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000034DC File Offset: 0x000016DC
		private List<ValueTuple<CampaignVec2, float>> GetSpawnPoints()
		{
			List<ValueTuple<CampaignVec2, float>> list = new List<ValueTuple<CampaignVec2, float>>();
			foreach (KeyValuePair<string, List<ValueTuple<CampaignVec2, float>>> keyValuePair in this._pirateSpawnPoints)
			{
				list.AddRange(keyValuePair.Value);
			}
			return list;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x0000353C File Offset: 0x0000173C
		private void InitializeDropOffLocations()
		{
			using (IEnumerator<GameEntity> enumerator = this._mapScene.Scene.FindEntitiesWithTag("main_map_village_dropoff").GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					GameEntity entity = enumerator.Current;
					SettlementComponent settlementComponent = Village.All.FirstOrDefault<Village>((Village x) => x.Settlement.StringId == entity.Parent.Name);
					CampaignVec2 campaignVec;
					campaignVec..ctor(entity.GlobalPosition.AsVec2, false);
					settlementComponent.Settlement.SetPortPosition(campaignVec);
				}
			}
		}

		// Token: 0x06000046 RID: 70 RVA: 0x000035D8 File Offset: 0x000017D8
		public Vec2 GetWindAtPosition(Vec2 position)
		{
			return this._mapScene.GetWindAtPosition(position);
		}

		// Token: 0x06000047 RID: 71 RVA: 0x000035E6 File Offset: 0x000017E6
		private void InitializeMapWaterWake()
		{
			this._mapScene.SetupWaterWake(128f, 8f);
		}

		// Token: 0x04000018 RID: 24
		private const string VillageDropOffPointTag = "main_map_village_dropoff";

		// Token: 0x04000019 RID: 25
		private MapScene _mapScene;

		// Token: 0x0400001A RID: 26
		private Dictionary<string, List<ValueTuple<CampaignVec2, float>>> _pirateSpawnPoints = new Dictionary<string, List<ValueTuple<CampaignVec2, float>>>();
	}
}
