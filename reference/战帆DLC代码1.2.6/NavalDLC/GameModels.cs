using System;
using System.Collections.Generic;
using NavalDLC.ComponentInterfaces;
using TaleWorlds.Core;

namespace NavalDLC
{
	// Token: 0x02000018 RID: 24
	public sealed class GameModels : GameModelsManager
	{
		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000108 RID: 264 RVA: 0x000081CC File Offset: 0x000063CC
		public static GameModels Instance
		{
			get
			{
				return NavalDLCManager.Instance.GameModels;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000109 RID: 265 RVA: 0x000081D8 File Offset: 0x000063D8
		// (set) Token: 0x0600010A RID: 266 RVA: 0x000081E0 File Offset: 0x000063E0
		public ShipPhysicsParametersModel ShipPhysicsParametersModel { get; private set; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600010B RID: 267 RVA: 0x000081E9 File Offset: 0x000063E9
		// (set) Token: 0x0600010C RID: 268 RVA: 0x000081F1 File Offset: 0x000063F1
		public ClanShipOwnershipModel ClanShipOwnershipModel { get; private set; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600010D RID: 269 RVA: 0x000081FA File Offset: 0x000063FA
		// (set) Token: 0x0600010E RID: 270 RVA: 0x00008202 File Offset: 0x00006402
		public ShipDistributionModel ShipDistributionModel { get; set; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600010F RID: 271 RVA: 0x0000820B File Offset: 0x0000640B
		// (set) Token: 0x06000110 RID: 272 RVA: 0x00008213 File Offset: 0x00006413
		public ShipDeploymentModel ShipDeploymentModel { get; private set; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000111 RID: 273 RVA: 0x0000821C File Offset: 0x0000641C
		// (set) Token: 0x06000112 RID: 274 RVA: 0x00008224 File Offset: 0x00006424
		public MapStormModel MapStormModel { get; private set; }

		// Token: 0x06000113 RID: 275 RVA: 0x0000822D File Offset: 0x0000642D
		public GameModels(IEnumerable<GameModel> inputComponents)
			: base(inputComponents)
		{
			this.GetDefaultGameModels();
		}

		// Token: 0x06000114 RID: 276 RVA: 0x0000823C File Offset: 0x0000643C
		private void GetDefaultGameModels()
		{
			this.ShipPhysicsParametersModel = base.GetGameModel<ShipPhysicsParametersModel>();
			this.ClanShipOwnershipModel = base.GetGameModel<ClanShipOwnershipModel>();
			this.ShipDistributionModel = base.GetGameModel<ShipDistributionModel>();
			this.ShipDeploymentModel = base.GetGameModel<ShipDeploymentModel>();
			this.MapStormModel = base.GetGameModel<MapStormModel>();
		}
	}
}
