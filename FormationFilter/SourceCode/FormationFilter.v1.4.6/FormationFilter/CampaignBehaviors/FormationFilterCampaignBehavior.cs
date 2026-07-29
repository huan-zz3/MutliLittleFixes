using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FormationFilter.Utilities;
using Newtonsoft.Json;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;

namespace FormationFilter.CampaignBehaviors
{
	// Token: 0x02000022 RID: 34
	[NullableContext(1)]
	[Nullable(0)]
	public class FormationFilterCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600013E RID: 318 RVA: 0x000091F7 File Offset: 0x000073F7
		public override void RegisterEvents()
		{
		}

		// Token: 0x0600013F RID: 319 RVA: 0x000091FC File Offset: 0x000073FC
		public override void SyncData(IDataStore dataStore)
		{
			if (dataStore.IsSaving)
			{
				this.SaveFormationData(dataStore, "_siegeFormationSaveData", this._siegeFormationSaveData);
				this.SaveFormationData(dataStore, "_siegeArmyFormationSaveData", this._siegeArmyFormationSaveData);
				this.SaveFormationData(dataStore, "_fieldBattleFormationSaveData", this._fieldBattleFormationSaveData);
				this.SaveFormationData(dataStore, "_fieldBattleArmyFormationSaveData", this._fieldBattleArmyFormationSaveData);
			}
			if (dataStore.IsLoading)
			{
				this.LoadFormationData(dataStore, "_siegeFormationSaveData", ref this._siegeFormationSaveData);
				this.LoadFormationData(dataStore, "_siegeArmyFormationSaveData", ref this._siegeArmyFormationSaveData);
				this.LoadFormationData(dataStore, "_fieldBattleFormationSaveData", ref this._fieldBattleFormationSaveData);
				this.LoadFormationData(dataStore, "_fieldBattleArmyFormationSaveData", ref this._fieldBattleArmyFormationSaveData);
			}
		}

		// Token: 0x06000140 RID: 320 RVA: 0x000092AC File Offset: 0x000074AC
		private void SaveFormationData(IDataStore dataStore, string name, List<FormationFilterFormationSaveData> data)
		{
			try
			{
				string text = JsonConvert.SerializeObject(data);
				dataStore.SyncData<string>(name, ref text);
			}
			catch (Exception ex)
			{
				Utility.DisplayException(ex);
			}
		}

		// Token: 0x06000141 RID: 321 RVA: 0x000092E8 File Offset: 0x000074E8
		private void LoadFormationData(IDataStore dataStore, string name, ref List<FormationFilterFormationSaveData> data)
		{
			List<FormationFilterFormationSaveData> list = null;
			string text = "";
			try
			{
				if (dataStore.SyncData<string>(name, ref text) && !string.IsNullOrEmpty(text))
				{
					list = JsonConvert.DeserializeObject<List<FormationFilterFormationSaveData>>(text);
				}
			}
			catch (Exception ex)
			{
				Utility.DisplayException(ex);
			}
			if (list == null)
			{
				return;
			}
			data = list;
		}

		// Token: 0x06000142 RID: 322 RVA: 0x0000933C File Offset: 0x0000753C
		[NullableContext(2)]
		public FormationFilterFormationSaveData GetFormationDataAtIndex(int formationIndex, bool isSiegeBattle, bool isInArmy)
		{
			List<FormationFilterFormationSaveData> list = (isSiegeBattle ? (isInArmy ? this._siegeArmyFormationSaveData : this._siegeFormationSaveData) : (isInArmy ? this._fieldBattleArmyFormationSaveData : this._fieldBattleFormationSaveData));
			if (list == null || list.Count <= formationIndex)
			{
				return this.GetFormationDataAtIndexFromNativeConfig(formationIndex, isSiegeBattle, isInArmy);
			}
			return list[formationIndex];
		}

		// Token: 0x06000143 RID: 323 RVA: 0x0000938E File Offset: 0x0000758E
		public void SetFormationData(List<FormationFilterFormationSaveData> formationSaveData, bool isSiegeBattle, bool isInArmy)
		{
			if (isSiegeBattle)
			{
				if (isInArmy)
				{
					this._siegeArmyFormationSaveData = formationSaveData;
					return;
				}
				this._siegeFormationSaveData = formationSaveData;
				return;
			}
			else
			{
				if (isInArmy)
				{
					this._fieldBattleArmyFormationSaveData = formationSaveData;
					return;
				}
				this._fieldBattleFormationSaveData = formationSaveData;
				return;
			}
		}

		// Token: 0x06000144 RID: 324 RVA: 0x000093B8 File Offset: 0x000075B8
		[NullableContext(2)]
		private FormationFilterFormationSaveData GetFormationDataAtIndexFromNativeConfig(int formationIndex, bool isSiegeBattle, bool isInArmy)
		{
			OrderOfBattleCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<OrderOfBattleCampaignBehavior>();
			OrderOfBattleCampaignBehavior.OrderOfBattleFormationData orderOfBattleFormationData = ((campaignBehavior != null) ? campaignBehavior.GetFormationDataAtIndex(formationIndex, isSiegeBattle, isInArmy) : null);
			if (orderOfBattleFormationData == null)
			{
				return null;
			}
			FormationFilterFormationSaveData formationFilterFormationSaveData = new FormationFilterFormationSaveData();
			Hero captain = orderOfBattleFormationData.Captain;
			formationFilterFormationSaveData.Captain = ((captain != null) ? captain.StringId : null) ?? "";
			formationFilterFormationSaveData.HeroTroops = orderOfBattleFormationData.HeroTroops.Select<Hero, string>((Hero hero) => hero.StringId).ToList<string>();
			formationFilterFormationSaveData.FormationClassFilters = FormationFilterCampaignBehavior.GetFormationClassSaveDataFromNativeFormationInfo(orderOfBattleFormationData);
			return formationFilterFormationSaveData;
		}

		// Token: 0x06000145 RID: 325 RVA: 0x0000944C File Offset: 0x0000764C
		private static List<FormationFilterFormationClassSaveData> GetFormationClassSaveDataFromNativeFormationInfo(OrderOfBattleCampaignBehavior.OrderOfBattleFormationData formationInfo)
		{
			switch (formationInfo.FormationClass)
			{
			case 0:
				return new List<FormationFilterFormationClassSaveData>();
			case 1:
				return new List<FormationFilterFormationClassSaveData>
				{
					new FormationFilterFormationClassSaveData
					{
						BasicFormationClass = 0,
						Weight = (float)formationInfo.PrimaryClassWeight / 100f
					}
				};
			case 2:
				return new List<FormationFilterFormationClassSaveData>
				{
					new FormationFilterFormationClassSaveData
					{
						BasicFormationClass = 1,
						Weight = (float)formationInfo.PrimaryClassWeight / 100f
					}
				};
			case 3:
				return new List<FormationFilterFormationClassSaveData>
				{
					new FormationFilterFormationClassSaveData
					{
						BasicFormationClass = 2,
						Weight = (float)formationInfo.PrimaryClassWeight / 100f
					}
				};
			case 4:
				return new List<FormationFilterFormationClassSaveData>
				{
					new FormationFilterFormationClassSaveData
					{
						BasicFormationClass = 3,
						Weight = (float)formationInfo.PrimaryClassWeight / 100f
					}
				};
			case 5:
				return new List<FormationFilterFormationClassSaveData>
				{
					new FormationFilterFormationClassSaveData
					{
						BasicFormationClass = 0,
						Weight = (float)formationInfo.PrimaryClassWeight / 100f
					},
					new FormationFilterFormationClassSaveData
					{
						BasicFormationClass = 1,
						Weight = (float)formationInfo.SecondaryClassWeight / 100f
					}
				};
			case 6:
				return new List<FormationFilterFormationClassSaveData>
				{
					new FormationFilterFormationClassSaveData
					{
						BasicFormationClass = 2,
						Weight = (float)formationInfo.PrimaryClassWeight / 100f
					},
					new FormationFilterFormationClassSaveData
					{
						BasicFormationClass = 3,
						Weight = (float)formationInfo.SecondaryClassWeight / 100f
					}
				};
			default:
				return new List<FormationFilterFormationClassSaveData>();
			}
		}

		// Token: 0x04000090 RID: 144
		private List<FormationFilterFormationSaveData> _siegeFormationSaveData = new List<FormationFilterFormationSaveData>();

		// Token: 0x04000091 RID: 145
		private List<FormationFilterFormationSaveData> _siegeArmyFormationSaveData = new List<FormationFilterFormationSaveData>();

		// Token: 0x04000092 RID: 146
		private List<FormationFilterFormationSaveData> _fieldBattleFormationSaveData = new List<FormationFilterFormationSaveData>();

		// Token: 0x04000093 RID: 147
		private List<FormationFilterFormationSaveData> _fieldBattleArmyFormationSaveData = new List<FormationFilterFormationSaveData>();
	}
}
