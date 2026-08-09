using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace NavalDLC.CampaignBehaviors
{
	// Token: 0x0200016C RID: 364
	public class NavalOrderOfBattleCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x060017E3 RID: 6115 RVA: 0x000A3263 File Offset: 0x000A1463
		public NavalOrderOfBattleCampaignBehavior()
		{
			this._navalBattleFormationInfos = new List<NavalOrderOfBattleCampaignBehavior.NavalOrderOfBattleFormationData>();
			this._navalBattleArmyFormationInfos = new List<NavalOrderOfBattleCampaignBehavior.NavalOrderOfBattleFormationData>();
		}

		// Token: 0x060017E4 RID: 6116 RVA: 0x000A3281 File Offset: 0x000A1481
		public override void RegisterEvents()
		{
			CampaignEvents.OnShipDestroyedEvent.AddNonSerializedListener(this, new Action<PartyBase, Ship, DestroyShipAction.ShipDestroyDetail>(this.OnShipDestroyed));
			CampaignEvents.OnHeroUnregisteredEvent.AddNonSerializedListener(this, new Action<Hero>(this.OnHeroUnregistered));
		}

		// Token: 0x060017E5 RID: 6117 RVA: 0x000A32B1 File Offset: 0x000A14B1
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<List<NavalOrderOfBattleCampaignBehavior.NavalOrderOfBattleFormationData>>("_navalBattleFormationInfos", ref this._navalBattleFormationInfos);
			dataStore.SyncData<List<NavalOrderOfBattleCampaignBehavior.NavalOrderOfBattleFormationData>>("_navalArmyBattleFormationInfos", ref this._navalBattleArmyFormationInfos);
		}

		// Token: 0x060017E6 RID: 6118 RVA: 0x000A32D7 File Offset: 0x000A14D7
		public NavalOrderOfBattleCampaignBehavior.NavalOrderOfBattleFormationData GetFormationDataAtIndex(int formationIndex, bool isInArmy)
		{
			if (isInArmy)
			{
				if (this._navalBattleArmyFormationInfos.Count > formationIndex)
				{
					return this._navalBattleArmyFormationInfos[formationIndex];
				}
				return null;
			}
			else
			{
				if (this._navalBattleFormationInfos.Count > formationIndex)
				{
					return this._navalBattleFormationInfos[formationIndex];
				}
				return null;
			}
		}

		// Token: 0x060017E7 RID: 6119 RVA: 0x000A3315 File Offset: 0x000A1515
		public void SetFormationInfos(List<NavalOrderOfBattleCampaignBehavior.NavalOrderOfBattleFormationData> formationInfos, bool isInArmy)
		{
			if (isInArmy)
			{
				this._navalBattleArmyFormationInfos = new List<NavalOrderOfBattleCampaignBehavior.NavalOrderOfBattleFormationData>(formationInfos);
				return;
			}
			this._navalBattleFormationInfos = new List<NavalOrderOfBattleCampaignBehavior.NavalOrderOfBattleFormationData>(formationInfos);
		}

		// Token: 0x060017E8 RID: 6120 RVA: 0x000A3334 File Offset: 0x000A1534
		private void OnShipDestroyed(PartyBase owner, Ship ship, DestroyShipAction.ShipDestroyDetail detail)
		{
			for (int i = this._navalBattleFormationInfos.Count - 1; i >= 0; i--)
			{
				if (this._navalBattleFormationInfos[i].Ship == ship)
				{
					this._navalBattleFormationInfos.RemoveAt(i);
				}
			}
			for (int j = this._navalBattleArmyFormationInfos.Count - 1; j >= 0; j--)
			{
				if (this._navalBattleArmyFormationInfos[j].Ship == ship)
				{
					this._navalBattleArmyFormationInfos.RemoveAt(j);
				}
			}
		}

		// Token: 0x060017E9 RID: 6121 RVA: 0x000A33B4 File Offset: 0x000A15B4
		private void OnHeroUnregistered(Hero hero)
		{
			for (int i = this._navalBattleFormationInfos.Count - 1; i >= 0; i--)
			{
				NavalOrderOfBattleCampaignBehavior.NavalOrderOfBattleFormationData navalOrderOfBattleFormationData = this._navalBattleFormationInfos[i];
				if (navalOrderOfBattleFormationData.Captain == hero)
				{
					this._navalBattleFormationInfos[i] = new NavalOrderOfBattleCampaignBehavior.NavalOrderOfBattleFormationData(null, navalOrderOfBattleFormationData.Ship, navalOrderOfBattleFormationData.FormationClass, navalOrderOfBattleFormationData.Filters);
				}
			}
			for (int j = this._navalBattleArmyFormationInfos.Count - 1; j >= 0; j--)
			{
				NavalOrderOfBattleCampaignBehavior.NavalOrderOfBattleFormationData navalOrderOfBattleFormationData2 = this._navalBattleArmyFormationInfos[j];
				if (navalOrderOfBattleFormationData2.Captain == hero)
				{
					this._navalBattleArmyFormationInfos[j] = new NavalOrderOfBattleCampaignBehavior.NavalOrderOfBattleFormationData(null, navalOrderOfBattleFormationData2.Ship, navalOrderOfBattleFormationData2.FormationClass, navalOrderOfBattleFormationData2.Filters);
				}
			}
		}

		// Token: 0x04000BEE RID: 3054
		private List<NavalOrderOfBattleCampaignBehavior.NavalOrderOfBattleFormationData> _navalBattleFormationInfos;

		// Token: 0x04000BEF RID: 3055
		private List<NavalOrderOfBattleCampaignBehavior.NavalOrderOfBattleFormationData> _navalBattleArmyFormationInfos;

		// Token: 0x02000299 RID: 665
		public class NavalOrderOfBattleFormationData
		{
			// Token: 0x06001CD7 RID: 7383 RVA: 0x000B9F14 File Offset: 0x000B8114
			public NavalOrderOfBattleFormationData(Hero captain, Ship ship, DeploymentFormationClass formationClass, Dictionary<FormationFilterType, bool> filters)
			{
				this.Captain = captain;
				this.Ship = ship;
				this.FormationClass = formationClass;
				this.Filters = new Dictionary<FormationFilterType, bool>();
				foreach (FormationFilterType formationFilterType in filters.Keys)
				{
					this.Filters.Add(formationFilterType, filters[formationFilterType]);
				}
			}

			// Token: 0x04001130 RID: 4400
			[SaveableField(1)]
			public readonly Hero Captain;

			// Token: 0x04001131 RID: 4401
			[SaveableField(2)]
			public readonly Ship Ship;

			// Token: 0x04001132 RID: 4402
			[SaveableField(3)]
			public readonly DeploymentFormationClass FormationClass;

			// Token: 0x04001133 RID: 4403
			[SaveableField(4)]
			public readonly Dictionary<FormationFilterType, bool> Filters;
		}
	}
}
