using System;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace NavalDLC.Settlements.Building
{
	// Token: 0x0200007D RID: 125
	public class NavalBuildingTypes
	{
		// Token: 0x17000175 RID: 373
		// (get) Token: 0x060008EF RID: 2287 RVA: 0x0003ECEB File Offset: 0x0003CEEB
		public static BuildingType SettlementShipyard
		{
			get
			{
				return NavalBuildingTypes.Instance._buildingShipyard;
			}
		}

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x060008F0 RID: 2288 RVA: 0x0003ECF7 File Offset: 0x0003CEF7
		private static NavalBuildingTypes Instance
		{
			get
			{
				return NavalDLCManager.Instance.NavalBuildingTypes;
			}
		}

		// Token: 0x060008F1 RID: 2289 RVA: 0x0003ED03 File Offset: 0x0003CF03
		public NavalBuildingTypes()
		{
			this.RegisterAll();
			this.InitializeAll();
		}

		// Token: 0x060008F2 RID: 2290 RVA: 0x0003ED17 File Offset: 0x0003CF17
		private void RegisterAll()
		{
			this._buildingShipyard = this.Create("building_shipyard");
		}

		// Token: 0x060008F3 RID: 2291 RVA: 0x0003ED2A File Offset: 0x0003CF2A
		private BuildingType Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<BuildingType>(new BuildingType(stringId));
		}

		// Token: 0x060008F4 RID: 2292 RVA: 0x0003ED44 File Offset: 0x0003CF44
		private void InitializeAll()
		{
			this._buildingShipyard.Initialize(GameTexts.FindText("str_shipyard", null), new TextObject("{=bDDtGsyv}Allows ship production. Enables repair, trading, and upgrades of ships.", null), new int[] { 0, 4800, 6000 }, new Tuple<BuildingEffectEnum, BuildingEffectIncrementType, float, float, float>[]
			{
				new Tuple<BuildingEffectEnum, BuildingEffectIncrementType, float, float, float>(28, 0, 1f, 2f, 3f),
				new Tuple<BuildingEffectEnum, BuildingEffectIncrementType, float, float, float>(29, 0, 9f, 12f, 15f)
			}, false, 0f, 1);
		}

		// Token: 0x0400053B RID: 1339
		private BuildingType _buildingShipyard;
	}
}
