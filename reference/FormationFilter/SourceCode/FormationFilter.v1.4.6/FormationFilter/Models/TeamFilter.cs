using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using FormationFilter.CampaignBehaviors;
using FormationFilter.Logics;
using FormationFilter.Utilities;
using FormationFilter.View;
using HarmonyLib;
using SandBox.ViewModelCollection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;

namespace FormationFilter.Models
{
	// Token: 0x02000018 RID: 24
	[NullableContext(1)]
	[Nullable(0)]
	public class TeamFilter
	{
		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000D1 RID: 209 RVA: 0x00005F7A File Offset: 0x0000417A
		// (set) Token: 0x060000D2 RID: 210 RVA: 0x00005F82 File Offset: 0x00004182
		public bool IsLoaded { get; private set; }

		// Token: 0x060000D3 RID: 211 RVA: 0x00005F8C File Offset: 0x0000418C
		static TeamFilter()
		{
			Harmony harmony = new Harmony("FormationFilter.TeamFilter");
			harmony.Patch(typeof(OrderOfBattleVM).GetMethod("LoadConfiguration", BindingFlags.Instance | BindingFlags.NonPublic), null, new HarmonyMethod(typeof(TeamFilter).GetMethod("Postfix_LoadBasicConfiguration", BindingFlags.Static | BindingFlags.Public)), null, null);
			harmony.Patch(typeof(OrderOfBattleVM).GetMethod("SaveConfiguration", BindingFlags.Instance | BindingFlags.NonPublic), null, new HarmonyMethod(typeof(TeamFilter).GetMethod("Postfix_SaveBasicConfiguration", BindingFlags.Static | BindingFlags.Public)), null, null);
			harmony.Patch(typeof(SPOrderOfBattleVM).GetMethod("LoadConfiguration", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(TeamFilter).GetMethod("Prefix_LoadConfiguration", BindingFlags.Static | BindingFlags.Public)), null, null, null);
			harmony.Patch(typeof(SPOrderOfBattleVM).GetMethod("SaveConfiguration", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(TeamFilter).GetMethod("Prefix_SaveConfiguration", BindingFlags.Static | BindingFlags.Public)), null, null, null);
			harmony.Patch(typeof(OrderOfBattleVM).GetMethod("DistributeTroops", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(TeamFilter).GetMethod("Prefix_DistributeTroops", BindingFlags.Static | BindingFlags.Public)), null, null, null);
			harmony.Patch(typeof(OrderOfBattleVM).GetMethod("DistributeAllTroops", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(TeamFilter).GetMethod("Prefix_DistributeTroops", BindingFlags.Static | BindingFlags.Public)), null, null, null);
			harmony.Patch(typeof(OrderOfBattleVM).GetMethod("TransferAllAvailableTroopsToFormation", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(TeamFilter).GetMethod("Prefix_DistributeTroops", BindingFlags.Static | BindingFlags.Public)), null, null, null);
			harmony.Patch(typeof(OrderOfBattleVM).GetMethod("OnFilterUseToggled", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(TeamFilter).GetMethod("Prefix_DistributeTroops", BindingFlags.Static | BindingFlags.Public)), null, null, null);
			harmony.Patch(typeof(OrderOfBattleVM).GetMethod("DistributeWeights", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(TeamFilter).GetMethod("Prefix_DistributeWeights", BindingFlags.Static | BindingFlags.Public)), null, null, null);
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x000061B8 File Offset: 0x000043B8
		public static void Postfix_LoadBasicConfiguration(OrderOfBattleVM __instance)
		{
			TeamFilter teamFilter = Mission.Current.GetMissionBehavior<FormationFilterLogic>().GetTeamFilter(Mission.Current.PlayerTeam);
			if (teamFilter == null)
			{
				return;
			}
			if (Campaign.Current == null)
			{
				if (TeamFilter._customBattleSavedFilters == null)
				{
					TeamFilter.LoadBasicConfiguration(teamFilter);
				}
				else
				{
					for (int i = 0; i < 8; i++)
					{
						FormationFilters formationFilters = TeamFilter._customBattleSavedFilters[i];
						Formation formation = Mission.Current.PlayerTeam.GetFormation(i);
						teamFilter._allFilters[formation] = formationFilters;
					}
				}
				teamFilter.IsLoaded = true;
				teamFilter.OnFormationFilterUpdated();
				FormationFilterLogic.TeamFilterConfigurationLoaded(teamFilter);
			}
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00006244 File Offset: 0x00004444
		public static void Postfix_SaveBasicConfiguration(OrderOfBattleVM __instance)
		{
			TeamFilter teamFilter = Mission.Current.GetMissionBehavior<FormationFilterLogic>().GetTeamFilter(Mission.Current.PlayerTeam);
			if (teamFilter == null)
			{
				return;
			}
			if (Campaign.Current == null)
			{
				TeamFilter._customBattleSavedFilters = new List<FormationFilters>();
				for (int i = 0; i < 8; i++)
				{
					FormationFilters formationFilters = teamFilter.GetFormationFilters(Mission.Current.PlayerTeam.GetFormation(i)) ?? new FormationFilters();
					TeamFilter._customBattleSavedFilters.Add(formationFilters);
				}
			}
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x000062B8 File Offset: 0x000044B8
		public static bool Prefix_LoadConfiguration(SPOrderOfBattleVM __instance, List<OrderOfBattleFormationItemVM> ____allFormations, List<OrderOfBattleHeroItemVM> ____allHeroes)
		{
			TeamFilter teamFilter = Mission.Current.GetMissionBehavior<FormationFilterLogic>().GetTeamFilter(Mission.Current.PlayerTeam);
			if (teamFilter == null)
			{
				return true;
			}
			if (Campaign.Current != null)
			{
				TeamFilter.LoadCampaignConfiguration(__instance, ____allFormations, ____allHeroes, teamFilter);
				teamFilter.IsLoaded = true;
				teamFilter.OnFormationFilterUpdated();
				FormationFilterLogic.TeamFilterConfigurationLoaded(teamFilter);
				return false;
			}
			return true;
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x0000630C File Offset: 0x0000450C
		public static bool Prefix_SaveConfiguration(SPOrderOfBattleVM __instance, List<OrderOfBattleFormationItemVM> ____allFormations)
		{
			TeamFilter teamFilter = Mission.Current.GetMissionBehavior<FormationFilterLogic>().GetTeamFilter(Mission.Current.PlayerTeam);
			if (teamFilter == null)
			{
				return true;
			}
			if (Campaign.Current != null)
			{
				TeamFilter.SaveCampaignConfiguration(__instance, ____allFormations, teamFilter);
				return false;
			}
			return true;
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x0000634C File Offset: 0x0000454C
		private static void LoadBasicConfiguration(TeamFilter playerTeamFilter)
		{
			for (int i = 0; i < 4; i++)
			{
				Formation formation = Mission.Current.PlayerTeam.FormationsIncludingSpecialAndEmpty[i];
				FormationFilters formationFilters = playerTeamFilter.InitializeFormationFilters(formation);
				TroopFilter troopFilter = formationFilters.ForceGetTroopFilterAtIndex(0, i);
			}
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x0000638C File Offset: 0x0000458C
		private static void LoadCampaignConfiguration(OrderOfBattleVM __instance, List<OrderOfBattleFormationItemVM> ____allFormations, List<OrderOfBattleHeroItemVM> ____allHeroes, TeamFilter playerTeamFilter)
		{
			FormationFilterCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<FormationFilterCampaignBehavior>();
			if (campaignBehavior == null || !__instance.IsPlayerGeneral)
			{
				return;
			}
			for (int i = 0; i < Mission.Current.PlayerTeam.FormationsIncludingEmpty.Count; i++)
			{
				FormationFilterFormationSaveData formationSaveData = campaignBehavior.GetFormationDataAtIndex(i, Mission.Current.IsSiegeBattle, MobileParty.MainParty.Army != null);
				if (____allFormations[i].Formation != null && formationSaveData != null)
				{
					FormationFilters formationFilters = playerTeamFilter.InitializeFormationFilters(____allFormations[i].Formation);
					if (formationSaveData.FormationClassFilters != null)
					{
						for (int j = 0; j < formationSaveData.FormationClassFilters.Count; j++)
						{
							FormationFilterFormationClassSaveData formationFilterFormationClassSaveData = formationSaveData.FormationClassFilters[j];
							if (formationFilterFormationClassSaveData != null)
							{
								TroopFilter troopFilter = formationFilters.ForceGetTroopFilterAtIndex(j, formationFilterFormationClassSaveData.BasicFormationClass);
								TeamFilter.AddSavedFilter(troopFilter, formationFilterFormationClassSaveData);
							}
						}
					}
					if (!formationFilters.Filters.All<TroopFilter>((TroopFilter filter) => filter.DetectBasicFomrationClass() == 10))
					{
						if (formationSaveData.Captain != null)
						{
							OrderOfBattleHeroItemVM orderOfBattleHeroItemVM = ____allHeroes.FirstOrDefault<OrderOfBattleHeroItemVM>((OrderOfBattleHeroItemVM heroItemVM) => heroItemVM.Agent.Character.StringId == formationSaveData.Captain);
							if (orderOfBattleHeroItemVM != null)
							{
								TeamFilter.AssignCaptain(orderOfBattleHeroItemVM, ____allFormations[i]);
							}
						}
						if (formationSaveData.HeroTroops == null)
						{
							goto IL_01BD;
						}
						using (List<string>.Enumerator enumerator = formationSaveData.HeroTroops.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								string heroTroop = enumerator.Current;
								OrderOfBattleHeroItemVM orderOfBattleHeroItemVM2 = ____allHeroes.FirstOrDefault<OrderOfBattleHeroItemVM>((OrderOfBattleHeroItemVM heroItemVM) => heroItemVM.Agent.Character.StringId == heroTroop);
								if (orderOfBattleHeroItemVM2 != null)
								{
									____allFormations[i].AddHeroTroop(orderOfBattleHeroItemVM2);
								}
							}
							goto IL_01BD;
						}
					}
					____allFormations[i].UnassignCaptain();
				}
				IL_01BD:;
			}
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00006584 File Offset: 0x00004784
		private static void AssignCaptain(OrderOfBattleHeroItemVM heroItemVM, OrderOfBattleFormationItemVM formationItem)
		{
			if (formationItem == null || heroItemVM == null || formationItem.Captain == heroItemVM)
			{
				return;
			}
			if (formationItem.HasCaptain)
			{
				formationItem.Captain.IsSelected = false;
				formationItem.UnassignCaptain();
			}
			formationItem.Captain = heroItemVM;
		}

		// Token: 0x060000DB RID: 219 RVA: 0x000065B8 File Offset: 0x000047B8
		private static void SaveCampaignConfiguration(SPOrderOfBattleVM __instance, List<OrderOfBattleFormationItemVM> ____allFormations, TeamFilter playerTeamFilter)
		{
			bool flag = MissionGameModels.Current.BattleInitializationModel.CanPlayerSideDeployWithOrderOfBattle();
			FormationFilterCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<FormationFilterCampaignBehavior>();
			if (campaignBehavior == null || !__instance.IsPlayerGeneral || !flag)
			{
				return;
			}
			List<FormationFilterFormationSaveData> list = new List<FormationFilterFormationSaveData>();
			for (int i = 0; i < Mission.Current.PlayerTeam.FormationsIncludingEmpty.Count; i++)
			{
				OrderOfBattleFormationItemVM formationItemVM = ____allFormations[i];
				Formation formation = formationItemVM.Formation;
				FormationFilters formationFilters = playerTeamFilter.GetFormationFilters(formation);
				Hero hero = null;
				if (formationItemVM.Captain.Agent != null)
				{
					hero = Hero.FindFirst((Hero h) => h.CharacterObject == formationItemVM.Captain.Agent.Character);
				}
				List<Hero> list2 = (from heroTroop in formationItemVM.HeroTroops
					select Hero.FindFirst((Hero hero) => hero.CharacterObject == heroTroop.Agent.Character) into h
					where h != null
					select h).ToList<Hero>();
				if (formationFilters != null)
				{
					formationFilters.ClearInvalidTroopFilter();
				}
				List<TroopFilter> list3 = ((formationFilters != null) ? formationFilters.GetTroopFilters() : null) ?? new List<TroopFilter>();
				FormationFilterFormationSaveData formationFilterFormationSaveData = new FormationFilterFormationSaveData();
				formationFilterFormationSaveData.Captain = ((hero != null) ? hero.StringId : null) ?? "";
				formationFilterFormationSaveData.HeroTroops = list2.Select<Hero, string>((Hero hero) => hero.StringId).ToList<string>() ?? new List<string>();
				FormationFilterFormationSaveData formationFilterFormationSaveData2 = formationFilterFormationSaveData;
				for (int j = 0; j < list3.Count; j++)
				{
					TroopFilter troopFilter = list3[j];
					formationFilterFormationSaveData2.FormationClassFilters.Add(new FormationFilterFormationClassSaveData
					{
						BasicFormationClass = troopFilter.DetectBasicFomrationClass(),
						FilterValueDictionary = troopFilter.GetAllFilters(true),
						Weight = troopFilter.Weight
					});
				}
				list.Add(formationFilterFormationSaveData2);
			}
			campaignBehavior.SetFormationData(list, Mission.Current.IsSiegeBattle, MobileParty.MainParty.Army != null);
		}

		// Token: 0x060000DC RID: 220 RVA: 0x000067D0 File Offset: 0x000049D0
		private static void AddSavedFilter(TroopFilter filter, FormationFilterFormationClassSaveData saveData)
		{
			Dictionary<FilterTypeEnum, FilterValueEnum> filterValueDictionary = saveData.FilterValueDictionary;
			IEnumerable<ValueTuple<FilterTypeEnum, FilterValueEnum>> enumerable;
			if (filterValueDictionary == null)
			{
				enumerable = null;
			}
			else
			{
				enumerable = filterValueDictionary.Select<KeyValuePair<FilterTypeEnum, FilterValueEnum>, ValueTuple<FilterTypeEnum, FilterValueEnum>>((KeyValuePair<FilterTypeEnum, FilterValueEnum> pair) => new ValueTuple<FilterTypeEnum, FilterValueEnum>(pair.Key, pair.Value));
			}
			IEnumerable<ValueTuple<FilterTypeEnum, FilterValueEnum>> enumerable2;
			if ((enumerable2 = enumerable) == null)
			{
				IEnumerable<ValueTuple<FilterTypeEnum, FilterValueEnum>> enumerable3 = Enumerable.Empty<ValueTuple<FilterTypeEnum, FilterValueEnum>>();
				enumerable2 = enumerable3;
			}
			filter.SetFilters(enumerable2.ToList<ValueTuple<FilterTypeEnum, FilterValueEnum>>());
			filter.SetWeight(saveData.Weight);
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00006830 File Offset: 0x00004A30
		public static bool Prefix_DistributeTroops(OrderOfBattleVM __instance, List<OrderOfBattleFormationItemVM> ____allFormations)
		{
			TeamFilter teamFilter = Mission.Current.GetMissionBehavior<FormationFilterLogic>().GetTeamFilter(Mission.Current.PlayerTeam);
			if (teamFilter == null)
			{
				return true;
			}
			List<Agent> list = ____allFormations.SelectMany<OrderOfBattleFormationItemVM, Agent>((OrderOfBattleFormationItemVM vm) => Utility.GetExcludedAgents(vm)).ToList<Agent>();
			List<Formation> list2 = teamFilter.ApplyFilters(Mission.Current.PlayerTeam, list, TeamFilter.IsAdustingWeights);
			foreach (OrderOfBattleFormationItemVM orderOfBattleFormationItemVM in ____allFormations)
			{
				if (list2.Contains(orderOfBattleFormationItemVM.Formation))
				{
					orderOfBattleFormationItemVM.OnSizeChanged();
				}
			}
			return false;
		}

		// Token: 0x060000DE RID: 222 RVA: 0x000068F0 File Offset: 0x00004AF0
		public static bool Prefix_DistributeWeights(OrderOfBattleVM __instance, List<OrderOfBattleFormationItemVM> ____allFormations)
		{
			return Mission.Current.GetMissionBehavior<FormationFilterLogic>().GetTeamFilter(Mission.Current.PlayerTeam) == null;
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00006920 File Offset: 0x00004B20
		public void FillFiltersForTeam(Team team)
		{
			this._allFilters.Clear();
			foreach (Formation formation in team.FormationsIncludingEmpty)
			{
				if (!this._allFilters.ContainsKey(formation))
				{
					this._allFilters[formation] = new FormationFilters();
				}
				FormationFilters formationFilters = this._allFilters[formation];
				switch (formation.FormationIndex)
				{
				case 0:
					formationFilters.Filters.Add(new TroopFilter(new Dictionary<FilterTypeEnum, FilterValueEnum>
					{
						{
							FilterTypeEnum.HasMount,
							FilterValueEnum.No
						},
						{
							FilterTypeEnum.HasRanged,
							FilterValueEnum.No
						}
					}, 0f));
					break;
				case 1:
					formationFilters.Filters.Add(new TroopFilter(new Dictionary<FilterTypeEnum, FilterValueEnum>
					{
						{
							FilterTypeEnum.HasMount,
							FilterValueEnum.No
						},
						{
							FilterTypeEnum.HasRanged,
							FilterValueEnum.Yes
						}
					}, 0f));
					break;
				case 2:
					formationFilters.Filters.Add(new TroopFilter(new Dictionary<FilterTypeEnum, FilterValueEnum>
					{
						{
							FilterTypeEnum.HasMount,
							FilterValueEnum.Yes
						},
						{
							FilterTypeEnum.HasRanged,
							FilterValueEnum.No
						}
					}, 1f));
					break;
				case 3:
					formationFilters.Filters.Add(new TroopFilter(new Dictionary<FilterTypeEnum, FilterValueEnum>
					{
						{
							FilterTypeEnum.HasMount,
							FilterValueEnum.Yes
						},
						{
							FilterTypeEnum.HasRanged,
							FilterValueEnum.Yes
						}
					}, 0f));
					break;
				case 4:
					formationFilters.Filters.Add(new TroopFilter(new Dictionary<FilterTypeEnum, FilterValueEnum>
					{
						{
							FilterTypeEnum.HasMount,
							FilterValueEnum.No
						},
						{
							FilterTypeEnum.HasRanged,
							FilterValueEnum.No
						},
						{
							FilterTypeEnum.HasThrowing,
							FilterValueEnum.Yes
						},
						{
							FilterTypeEnum.HasTwoHanded,
							FilterValueEnum.No
						}
					}, 1f));
					break;
				case 5:
					formationFilters.Filters.Add(new TroopFilter(new Dictionary<FilterTypeEnum, FilterValueEnum>
					{
						{
							FilterTypeEnum.HasMount,
							FilterValueEnum.No
						},
						{
							FilterTypeEnum.HasRanged,
							FilterValueEnum.No
						},
						{
							FilterTypeEnum.HasTwoHanded,
							FilterValueEnum.Yes
						}
					}, 1f));
					break;
				case 6:
					formationFilters.Filters.Add(new TroopFilter(new Dictionary<FilterTypeEnum, FilterValueEnum>
					{
						{
							FilterTypeEnum.HasMount,
							FilterValueEnum.Yes
						},
						{
							FilterTypeEnum.HasRanged,
							FilterValueEnum.No
						},
						{
							FilterTypeEnum.HasThrowing,
							FilterValueEnum.Yes
						},
						{
							FilterTypeEnum.HasTwoHanded,
							FilterValueEnum.No
						}
					}, 1f));
					break;
				}
			}
			this.OnFormationFilterUpdated();
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00006B54 File Offset: 0x00004D54
		public void UpdateFormationFilter(Formation formation, int indexInFormation, FilterTypeEnum filterType, FilterValueEnum filterEnum)
		{
			FormationFilters formationFilters;
			if (!this._allFilters.TryGetValue(formation, out formationFilters))
			{
				InformationManager.DisplayMessage(new InformationMessage("FormationFilter: Unexpected formation encountered", new Color(1f, 0f, 0f, 1f)));
				this._allFilters.Add(formation, new FormationFilters());
			}
			else
			{
				formationFilters.SetFilter(indexInFormation, filterType, filterEnum);
			}
			this.OnFormationFilterUpdated();
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00006BBC File Offset: 0x00004DBC
		[return: Nullable(2)]
		public FormationFilters GetFormationFilters(Formation formation)
		{
			FormationFilters formationFilters;
			if (!this._allFilters.TryGetValue(formation, out formationFilters))
			{
				return null;
			}
			return formationFilters;
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00006BDC File Offset: 0x00004DDC
		[return: Nullable(2)]
		public TroopFilter GetTroopFilter(Formation formation, int index)
		{
			FormationFilters formationFilters;
			if (!this._allFilters.TryGetValue(formation, out formationFilters))
			{
				return null;
			}
			return formationFilters.TryGetTroopFilterAtIndex(index);
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00006C02 File Offset: 0x00004E02
		[return: Nullable(2)]
		public TroopFilter GetTroopFilter(TroopFilterIdentifier troopFilterIdentifier)
		{
			return this.GetTroopFilter(troopFilterIdentifier.Formation, troopFilterIdentifier.Index);
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x00006C16 File Offset: 0x00004E16
		public List<TroopFilterIdentifier> GetAllTroopFilterIdentifiers()
		{
			return this._allFilters.SelectMany<KeyValuePair<Formation, FormationFilters>, TroopFilterIdentifier>((KeyValuePair<Formation, FormationFilters> pair) => pair.Value.Filters.Select<TroopFilter, TroopFilterIdentifier>((TroopFilter filter, int index) => new TroopFilterIdentifier(pair.Key, index))).ToList<TroopFilterIdentifier>();
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00006C48 File Offset: 0x00004E48
		public FormationFilters InitializeFormationFilters(Formation formation)
		{
			FormationFilters formationFilters = new FormationFilters();
			this._allFilters[formation] = formationFilters;
			return formationFilters;
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00006C69 File Offset: 0x00004E69
		public void OnFormationFilterUpdated()
		{
			this.UpdateIntersectedFilters();
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00006C74 File Offset: 0x00004E74
		private void UpdateIntersectedFilters()
		{
			Dictionary<ulong, Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>> dictionary = new Dictionary<ulong, Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>>();
			List<TroopFilterIdentifier> allTroopFilterIdentifiers = this.GetAllTroopFilterIdentifiers();
			for (int i = 0; i < allTroopFilterIdentifiers.Count; i++)
			{
				TroopFilterIdentifier troopFilterIdentifier = allTroopFilterIdentifiers[i];
				Formation formation = troopFilterIdentifier.Formation;
				TroopFilter troopFilter = this.GetTroopFilter(troopFilterIdentifier);
				if (troopFilter != null)
				{
					ulong bitmask = troopFilter.Bitmask;
					if (!TroopFilter.IsEmpty(bitmask))
					{
						if (!dictionary.ContainsKey(bitmask))
						{
							dictionary[bitmask] = new Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>();
						}
						Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>> dictionary2 = dictionary[bitmask];
						if (!dictionary2.ContainsKey(bitmask))
						{
							dictionary2[bitmask] = new Dictionary<Formation, List<TroopFilterIdentifier>>();
						}
						if (!dictionary2[bitmask].ContainsKey(formation))
						{
							dictionary2[bitmask][formation] = new List<TroopFilterIdentifier>();
						}
						dictionary2[bitmask][formation].Add(troopFilterIdentifier);
					}
				}
			}
			int num = 27;
			int j;
			for (j = 0; j < num; j++)
			{
				bool flag = false;
				List<KeyValuePair<ulong, Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>>> list = dictionary.ToList<KeyValuePair<ulong, Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>>>();
				for (int k = 0; k < list.Count; k++)
				{
					ulong key = list[k].Key;
					Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>> value = list[k].Value;
					for (int l = k + 1; l < list.Count; l++)
					{
						ulong key2 = list[l].Key;
						Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>> value2 = list[l].Value;
						ulong num2 = TroopFilter.Intersects(key, key2);
						if (num2 != 0UL)
						{
							if (!dictionary.ContainsKey(num2))
							{
								dictionary[num2] = new Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>();
								flag = true;
							}
							flag |= TeamFilter.AddInto(dictionary[num2], value);
							flag |= TeamFilter.AddInto(dictionary[num2], value2);
						}
					}
				}
				if (!flag)
				{
					break;
				}
			}
			if (j == num)
			{
				InformationManager.DisplayMessage(new InformationMessage("FormationFilter: max loop count encoutered", new Color(1f, 0f, 0f, 1f)));
			}
			this._intersectedFiltersMap = dictionary;
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00006E88 File Offset: 0x00005088
		private static bool AddInto(Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>> dicA, Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>> dicB)
		{
			bool flag = false;
			foreach (KeyValuePair<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>> keyValuePair in dicB)
			{
				if (!dicA.ContainsKey(keyValuePair.Key))
				{
					dicA[keyValuePair.Key] = keyValuePair.Value;
					flag = true;
				}
			}
			return flag;
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00006EF8 File Offset: 0x000050F8
		public List<Formation> ApplyFilters(Team team, List<Agent> excludedAgents, bool isAdjustingWeights = false)
		{
			this._totalUnitCountOfFilter.Clear();
			List<IFilteredAgent> filteredAgentList = TeamFilter.GetFilteredAgentList(team, excludedAgents);
			Dictionary<ulong, ValueTuple<Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>, List<IFilteredAgent>>> dictionary;
			List<IFilteredAgent> list;
			Dictionary<ulong, int> dictionary2;
			Dictionary<ulong, int> dictionary3;
			this.ComputeMinMaskToFilterAndAgents(team, filteredAgentList, out dictionary, out list, out dictionary2, out dictionary3);
			List<Agent> list2 = (from filteredAgent in list
				select filteredAgent as FilteredAgent into filteredAgent
				where filteredAgent != null
				select filteredAgent.Agent).ToList<Agent>();
			this._totalUnitCountOfFilter = dictionary2;
			this._minimumUnitCountOfFilter = dictionary3;
			bool flag = list2.Count == 0;
			if (!isAdjustingWeights)
			{
				bool flag2 = flag;
				bool? isPreviousSuccessful = this._isPreviousSuccessful;
				if (!((flag2 == isPreviousSuccessful.GetValueOrDefault()) & (isPreviousSuccessful != null)))
				{
					if (!flag)
					{
						Utility.DisplayRemainingAgents(list2);
					}
					else
					{
						Utility.DisplayNoRemainingAgents();
					}
				}
			}
			this._isPreviousSuccessful = new bool?(flag);
			FilterResultView missionBehavior = Mission.Current.GetMissionBehavior<FilterResultView>();
			if (missionBehavior != null)
			{
				missionBehavior.SetResult(list2);
			}
			return this.TransferAgents(team, this.ToAgentList(dictionary));
		}

		// Token: 0x060000EA RID: 234 RVA: 0x0000701C File Offset: 0x0000521C
		private static List<IFilteredAgent> GetFilteredAgentList(Team team, List<Agent> excludedAgents)
		{
			return (from formationUnit in team.FormationsIncludingEmpty.SelectMany<Formation, IFormationUnit>((Formation f) => f.DetachedUnits.Concat<IFormationUnit>(f.Arrangement.GetAllUnits()).Except<IFormationUnit>(excludedAgents))
				where formationUnit != null
				select formationUnit as Agent into agent
				where agent != null
				select new FilteredAgent(agent)).ToList<IFilteredAgent>();
		}

		// Token: 0x060000EB RID: 235 RVA: 0x000070E4 File Offset: 0x000052E4
		[return: TupleElementNames(new string[] { "matchedFilters", "agents" })]
		[return: Nullable(new byte[] { 1, 0, 1, 1, 1, 1, 1, 1, 1 })]
		private Dictionary<ulong, ValueTuple<Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>, List<Agent>>> ToAgentList([TupleElementNames(new string[] { "matchedFilters", "agents" })] [Nullable(new byte[] { 1, 0, 1, 1, 1, 1, 1, 1, 1 })] Dictionary<ulong, ValueTuple<Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>, List<IFilteredAgent>>> minBitmaskToFiltersAndAgents)
		{
			Dictionary<ulong, ValueTuple<Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>, List<Agent>>> dictionary = new Dictionary<ulong, ValueTuple<Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>, List<Agent>>>();
			foreach (KeyValuePair<ulong, ValueTuple<Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>, List<IFilteredAgent>>> keyValuePair in minBitmaskToFiltersAndAgents)
			{
				ulong key = keyValuePair.Key;
				Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>> item = keyValuePair.Value.Item1;
				List<IFilteredAgent> item2 = keyValuePair.Value.Item2;
				List<Agent> list = (from filteredAgent in item2
					select filteredAgent as FilteredAgent into filteredAgent
					where filteredAgent != null
					select filteredAgent.Agent).ToList<Agent>();
				dictionary[key] = new ValueTuple<Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>, List<Agent>>(item, list);
			}
			return dictionary;
		}

		// Token: 0x060000EC RID: 236 RVA: 0x000071E4 File Offset: 0x000053E4
		public void UpdateTotalAndActualUnitCountOfFilters(Team team, List<Agent> excludedAgents)
		{
			List<IFilteredAgent> filteredAgentList = TeamFilter.GetFilteredAgentList(team, excludedAgents);
			Dictionary<ulong, ValueTuple<Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>, List<IFilteredAgent>>> dictionary;
			List<IFilteredAgent> list;
			Dictionary<ulong, int> dictionary2;
			Dictionary<ulong, int> dictionary3;
			this.ComputeMinMaskToFilterAndAgents(team, filteredAgentList, out dictionary, out list, out dictionary2, out dictionary3);
			this._totalUnitCountOfFilter = dictionary2;
			this._minimumUnitCountOfFilter = dictionary3;
			HashSet<Formation> hashSet;
			Dictionary<TroopFilterIdentifier, int> dictionary4;
			Dictionary<ulong, Dictionary<TroopFilterIdentifier, int>> dictionary5;
			this.ComputeTroopFilterTransferList(this.ToAgentList(dictionary), out hashSet, out dictionary4, out dictionary5);
			this._troopFiltersActualUnitCount = dictionary4;
			this._intersectedFilterAssignedUnitCount = dictionary5;
		}

		// Token: 0x060000ED RID: 237 RVA: 0x0000723C File Offset: 0x0000543C
		private void ComputeMinMaskToFilterAndAgents(Team team, List<IFilteredAgent> agentsToClassify, [TupleElementNames(new string[] { "matchedFilters", "agents" })] [Nullable(new byte[] { 1, 0, 1, 1, 1, 1, 1, 1, 1 })] out Dictionary<ulong, ValueTuple<Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>, List<IFilteredAgent>>> minBitmaskToFiltersAndAgents, out List<IFilteredAgent> remainingAgents, out Dictionary<ulong, int> totalUnitCountOfFilters, out Dictionary<ulong, int> minimumUnitCountOfFilters)
		{
			minBitmaskToFiltersAndAgents = new Dictionary<ulong, ValueTuple<Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>, List<IFilteredAgent>>>();
			remainingAgents = new List<IFilteredAgent>();
			totalUnitCountOfFilters = new Dictionary<ulong, int>();
			minimumUnitCountOfFilters = new Dictionary<ulong, int>();
			foreach (IFilteredAgent filteredAgent in agentsToClassify)
			{
				ulong agentBitMask = filteredAgent.GetAgentBitMask();
				ulong num = ulong.MaxValue;
				foreach (KeyValuePair<ulong, Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>> keyValuePair in this._intersectedFiltersMap)
				{
					ulong key = keyValuePair.Key;
					Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>> value = keyValuePair.Value;
					if (TroopFilter.HasIntersection(agentBitMask, key) && num > key)
					{
						num = key;
					}
				}
				if (num == 18446744073709551615UL)
				{
					remainingAgents.Add(filteredAgent);
				}
				else
				{
					Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>> dictionary = this._intersectedFiltersMap[num];
					foreach (KeyValuePair<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>> keyValuePair2 in dictionary)
					{
						if (!totalUnitCountOfFilters.ContainsKey(keyValuePair2.Key))
						{
							totalUnitCountOfFilters[keyValuePair2.Key] = 0;
						}
						Dictionary<ulong, int> dictionary2 = totalUnitCountOfFilters;
						ulong num2 = keyValuePair2.Key;
						int num3 = dictionary2[num2];
						dictionary2[num2] = num3 + 1;
					}
					if (dictionary.Count == 1 && dictionary.First<KeyValuePair<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>>().Value.Count == 1 && dictionary.First<KeyValuePair<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>>().Value.First<KeyValuePair<Formation, List<TroopFilterIdentifier>>>().Value.Count == 1)
					{
						ulong num4 = num;
						if (!minimumUnitCountOfFilters.ContainsKey(num4))
						{
							minimumUnitCountOfFilters[num4] = 0;
						}
						Dictionary<ulong, int> dictionary3 = minimumUnitCountOfFilters;
						ulong num2 = num4;
						int num3 = dictionary3[num2];
						dictionary3[num2] = num3 + 1;
					}
					ValueTuple<Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>, List<IFilteredAgent>> valueTuple;
					if (minBitmaskToFiltersAndAgents.TryGetValue(num, out valueTuple))
					{
						valueTuple.Item2.Add(filteredAgent);
					}
					else
					{
						Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>> dictionary4 = this._intersectedFiltersMap[num];
						minBitmaskToFiltersAndAgents[num] = new ValueTuple<Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>, List<IFilteredAgent>>(dictionary4, new List<IFilteredAgent> { filteredAgent });
					}
				}
			}
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00007498 File Offset: 0x00005698
		private List<Formation> TransferAgents(Team team, [TupleElementNames(new string[] { "matchedFilters", "agents" })] [Nullable(new byte[] { 1, 0, 1, 1, 1, 1, 1, 1, 1 })] Dictionary<ulong, ValueTuple<Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>, List<Agent>>> bitmaskToFiltersAndAgents)
		{
			List<Formation> list = new List<Formation>();
			HashSet<Formation> hashSet;
			Dictionary<TroopFilterIdentifier, int> dictionary2;
			Dictionary<ulong, Dictionary<TroopFilterIdentifier, int>> dictionary3;
			Dictionary<Formation, List<Agent>> dictionary = this.ComputeTroopFilterTransferList(bitmaskToFiltersAndAgents, out hashSet, out dictionary2, out dictionary3);
			this._troopFiltersActualUnitCount = dictionary2;
			this._intersectedFilterAssignedUnitCount = dictionary3;
			foreach (Formation formation in hashSet)
			{
				formation.OnMassUnitTransferStart();
				if (formation.GetReadonlyMovementOrderReference().OrderEnum == 9 && formation.CountOfUnits > 0)
				{
					list.Add(formation);
					formation.SetMovementOrder(MovementOrder.MovementOrderMove(formation.CreateNewOrderWorldPosition(0)));
				}
			}
			foreach (KeyValuePair<Formation, List<Agent>> keyValuePair in dictionary)
			{
				foreach (Agent agent in keyValuePair.Value)
				{
					Formation key = keyValuePair.Key;
					if (key.CountOfUnits == 0)
					{
						Formation formation2 = agent.Formation;
						Utility.CopyOrdersFrom(key, formation2);
						key.SetPositioning(new WorldPosition?(formation2.CreateNewOrderWorldPosition(0)), new Vec2?(formation2.Direction), new int?(formation2.UnitSpacing));
					}
					agent.Formation = key;
				}
			}
			foreach (Formation formation3 in hashSet)
			{
				team.TriggerOnFormationsChanged(formation3);
				formation3.OnMassUnitTransferEnd();
				if (formation3.CountOfUnits > 0 && !formation3.OrderPositionIsValid)
				{
					Vec2 averagePositionOfUnits = formation3.GetAveragePositionOfUnits(false, false);
					float terrainHeight = Mission.Current.Scene.GetTerrainHeight(averagePositionOfUnits, true);
					Mission.Current.Scene.GetHeightAtPoint(averagePositionOfUnits, 0, ref terrainHeight);
					Vec3 vec;
					vec..ctor(averagePositionOfUnits, terrainHeight, -1f);
					WorldPosition worldPosition;
					worldPosition..ctor(Mission.Current.Scene, UIntPtr.Zero, vec, false);
					formation3.SetPositioning(new WorldPosition?(worldPosition), null, null);
				}
			}
			foreach (Formation formation4 in list)
			{
				formation4.SetMovementOrder(MovementOrder.MovementOrderStop);
			}
			return hashSet.ToList<Formation>();
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00007744 File Offset: 0x00005944
		private Dictionary<TroopFilterIdentifier, float> CollectTroopFilterIdentifierWeights(Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>> matchedFilters)
		{
			Dictionary<TroopFilterIdentifier, float> dictionary = new Dictionary<TroopFilterIdentifier, float>();
			foreach (KeyValuePair<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>> keyValuePair in matchedFilters)
			{
				Dictionary<Formation, List<TroopFilterIdentifier>> value = keyValuePair.Value;
				foreach (KeyValuePair<Formation, List<TroopFilterIdentifier>> keyValuePair2 in value)
				{
					Formation key = keyValuePair2.Key;
					List<TroopFilterIdentifier> value2 = keyValuePair2.Value;
					foreach (TroopFilterIdentifier troopFilterIdentifier in value2)
					{
						if (!dictionary.ContainsKey(troopFilterIdentifier))
						{
							dictionary[troopFilterIdentifier] = 0f;
						}
						Dictionary<TroopFilterIdentifier, float> dictionary2 = dictionary;
						TroopFilterIdentifier troopFilterIdentifier2 = troopFilterIdentifier;
						Dictionary<TroopFilterIdentifier, float> dictionary3 = dictionary2;
						TroopFilterIdentifier troopFilterIdentifier3 = troopFilterIdentifier2;
						float num = dictionary2[troopFilterIdentifier2];
						TroopFilter troopFilter = this.GetTroopFilter(troopFilterIdentifier);
						dictionary3[troopFilterIdentifier3] = num + ((troopFilter != null) ? troopFilter.Weight : 0f);
					}
				}
			}
			return dictionary;
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00007874 File Offset: 0x00005A74
		private Dictionary<Formation, List<Agent>> ComputeTroopFilterTransferList([TupleElementNames(new string[] { "matchedFilters", "agents" })] [Nullable(new byte[] { 1, 0, 1, 1, 1, 1, 1, 1, 1 })] Dictionary<ulong, ValueTuple<Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>, List<Agent>>> bitmaskToFiltersAndAgents, out HashSet<Formation> involvedFormations, out Dictionary<TroopFilterIdentifier, int> troopFiltersActualUnitCount, out Dictionary<ulong, Dictionary<TroopFilterIdentifier, int>> intersectedFilterAssignedUnitCount)
		{
			involvedFormations = new HashSet<Formation>();
			Dictionary<Formation, List<Agent>> dictionary = new Dictionary<Formation, List<Agent>>();
			Dictionary<TroopFilterIdentifier, List<Agent>> dictionary2 = new Dictionary<TroopFilterIdentifier, List<Agent>>();
			intersectedFilterAssignedUnitCount = new Dictionary<ulong, Dictionary<TroopFilterIdentifier, int>>();
			foreach (KeyValuePair<ulong, ValueTuple<Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>, List<Agent>>> keyValuePair in bitmaskToFiltersAndAgents)
			{
				ulong key5 = keyValuePair.Key;
				Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>> item = keyValuePair.Value.Item1;
				List<Agent> item2 = keyValuePair.Value.Item2;
				Dictionary<TroopFilterIdentifier, float> troopFilterIdentifierWeightMap = this.CollectTroopFilterIdentifierWeights(item);
				if (troopFilterIdentifierWeightMap.Count != 0)
				{
					float num = troopFilterIdentifierWeightMap.Sum<KeyValuePair<TroopFilterIdentifier, float>>((KeyValuePair<TroopFilterIdentifier, float> pair) => pair.Value);
					if (num == 0f)
					{
						foreach (TroopFilterIdentifier troopFilterIdentifier in troopFilterIdentifierWeightMap.Keys.ToList<TroopFilterIdentifier>())
						{
							troopFilterIdentifierWeightMap[troopFilterIdentifier] = 1f;
						}
						num = troopFilterIdentifierWeightMap.Sum<KeyValuePair<TroopFilterIdentifier, float>>((KeyValuePair<TroopFilterIdentifier, float> kvp) => kvp.Value);
					}
					Dictionary<Formation, List<Agent>> dictionary3 = troopFilterIdentifierWeightMap.Keys.Select<TroopFilterIdentifier, Formation>((TroopFilterIdentifier key) => key.Formation).Distinct<Formation>().ToDictionary<Formation, Formation, List<Agent>>((Formation key) => key, (Formation key) => new List<Agent>());
					Dictionary<TroopFilterIdentifier, List<Agent>> dictionary4 = troopFilterIdentifierWeightMap.Keys.ToDictionary<TroopFilterIdentifier, TroopFilterIdentifier, List<Agent>>((TroopFilterIdentifier key) => key, (TroopFilterIdentifier key) => new List<Agent>());
					Func<KeyValuePair<TroopFilterIdentifier, List<Agent>>, float> <>9__9;
					foreach (Agent agent in item2)
					{
						TroopFilterIdentifier troopFilterIdentifier2 = null;
						Formation previousFormation = agent.Formation;
						if (previousFormation != null)
						{
							List<TroopFilterIdentifier> list = troopFilterIdentifierWeightMap.Keys.Where<TroopFilterIdentifier>((TroopFilterIdentifier tfi) => tfi.Formation == previousFormation).ToList<TroopFilterIdentifier>();
							if (list.Any<TroopFilterIdentifier>())
							{
								foreach (TroopFilterIdentifier troopFilterIdentifier3 in list)
								{
									List<Agent> list2 = dictionary4[troopFilterIdentifier3];
									float num2 = troopFilterIdentifierWeightMap[troopFilterIdentifier3] / num * (float)item2.Count;
									if (num2 > (float)list2.Count)
									{
										troopFilterIdentifier2 = troopFilterIdentifier3;
										break;
									}
								}
							}
							if (previousFormation != ((troopFilterIdentifier2 != null) ? troopFilterIdentifier2.Formation : null))
							{
								involvedFormations.Add(previousFormation);
							}
						}
						if (troopFilterIdentifier2 == null)
						{
							IEnumerable<KeyValuePair<TroopFilterIdentifier, List<Agent>>> enumerable = dictionary4;
							Func<KeyValuePair<TroopFilterIdentifier, List<Agent>>, float> func;
							if ((func = <>9__9) == null)
							{
								func = (<>9__9 = delegate(KeyValuePair<TroopFilterIdentifier, List<Agent>> pair)
								{
									if (troopFilterIdentifierWeightMap[pair.Key] != 0f)
									{
										return (float)pair.Value.Count / troopFilterIdentifierWeightMap[pair.Key];
									}
									return float.MaxValue;
								});
							}
							troopFilterIdentifier2 = Extensions.MinBy<KeyValuePair<TroopFilterIdentifier, List<Agent>>, float>(enumerable, func).Key;
						}
						involvedFormations.Add(troopFilterIdentifier2.Formation);
						dictionary4[troopFilterIdentifier2].Add(agent);
						dictionary3[troopFilterIdentifier2.Formation].Add(agent);
					}
					foreach (KeyValuePair<TroopFilterIdentifier, List<Agent>> keyValuePair2 in dictionary4)
					{
						TroopFilterIdentifier key2 = keyValuePair2.Key;
						if (!dictionary2.ContainsKey(key2))
						{
							dictionary2[key2] = new List<Agent>();
						}
						dictionary2[key2].AddRange(keyValuePair2.Value);
					}
					foreach (KeyValuePair<TroopFilterIdentifier, List<Agent>> keyValuePair3 in dictionary4)
					{
						TroopFilterIdentifier key3 = keyValuePair3.Key;
						if (!intersectedFilterAssignedUnitCount.ContainsKey(key5))
						{
							intersectedFilterAssignedUnitCount[key5] = new Dictionary<TroopFilterIdentifier, int>();
						}
						if (!intersectedFilterAssignedUnitCount[key5].ContainsKey(key3))
						{
							intersectedFilterAssignedUnitCount[key5][key3] = 0;
						}
						Dictionary<TroopFilterIdentifier, int> dictionary5 = intersectedFilterAssignedUnitCount[key5];
						TroopFilterIdentifier troopFilterIdentifier4 = key3;
						dictionary5[troopFilterIdentifier4] += keyValuePair3.Value.Count;
					}
					foreach (KeyValuePair<Formation, List<Agent>> keyValuePair4 in dictionary3)
					{
						Formation key4 = keyValuePair4.Key;
						if (!dictionary.ContainsKey(key4))
						{
							dictionary[key4] = new List<Agent>();
						}
						dictionary[key4].AddRange(keyValuePair4.Value);
					}
				}
			}
			troopFiltersActualUnitCount = dictionary2.ToDictionary<KeyValuePair<TroopFilterIdentifier, List<Agent>>, TroopFilterIdentifier, int>((KeyValuePair<TroopFilterIdentifier, List<Agent>> pair) => pair.Key, (KeyValuePair<TroopFilterIdentifier, List<Agent>> pair) => pair.Value.Count);
			return dictionary;
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x00007E74 File Offset: 0x00006074
		public int GetTotalTroopCountOfFilter(ulong filterBitmask)
		{
			int num;
			if (!this._totalUnitCountOfFilter.TryGetValue(filterBitmask, out num))
			{
				return 0;
			}
			return num;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00007E94 File Offset: 0x00006094
		public int GetMinimumTroopCountOfFilter(ulong filterBitmask)
		{
			int num;
			if (!this._minimumUnitCountOfFilter.TryGetValue(filterBitmask, out num))
			{
				return 0;
			}
			return num;
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00007EB4 File Offset: 0x000060B4
		public int GetUnitCountOfTroopFilter(TroopFilterIdentifier troopFilterIdentifier)
		{
			int num;
			if (!this._troopFiltersActualUnitCount.TryGetValue(troopFilterIdentifier, out num))
			{
				return 0;
			}
			return num;
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00007ED4 File Offset: 0x000060D4
		public Dictionary<TroopFilterIdentifier, int> GetUnitCountOfRelatedTroopFilters(TroopFilterIdentifier troopFilterIdentifier)
		{
			Dictionary<TroopFilterIdentifier, int> dictionary = new Dictionary<TroopFilterIdentifier, int>();
			TroopFilter troopFilter = this.GetTroopFilter(troopFilterIdentifier);
			if (troopFilter == null)
			{
				return dictionary;
			}
			foreach (KeyValuePair<ulong, Dictionary<TroopFilterIdentifier, int>> keyValuePair in this._intersectedFilterAssignedUnitCount)
			{
				ulong key = keyValuePair.Key;
				Dictionary<TroopFilterIdentifier, int> value = keyValuePair.Value;
				if (TroopFilter.HasIntersection(troopFilter.Bitmask, key))
				{
					foreach (KeyValuePair<TroopFilterIdentifier, int> keyValuePair2 in value)
					{
						TroopFilterIdentifier key2 = keyValuePair2.Key;
						if (!key2.Equals(troopFilterIdentifier))
						{
							int value2 = keyValuePair2.Value;
							if (!dictionary.ContainsKey(key2))
							{
								dictionary[key2] = 0;
							}
							Dictionary<TroopFilterIdentifier, int> dictionary2 = dictionary;
							TroopFilterIdentifier troopFilterIdentifier2 = key2;
							dictionary2[troopFilterIdentifier2] += value2;
						}
					}
				}
			}
			return dictionary;
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00007FE0 File Offset: 0x000061E0
		[return: TupleElementNames(new string[] { "origin", "formationIndex" })]
		[return: Nullable(new byte[] { 1, 0, 1 })]
		public static List<ValueTuple<IAgentOriginBase, int>> GetReinforcementAssignments(BattleSideEnum battleSide, List<IAgentOriginBase> troopOrigins, out List<IAgentOriginBase> remainingTroopOrigins)
		{
			Dictionary<Team, List<IAgentOriginBase>> dictionary = new Dictionary<Team, List<IAgentOriginBase>>();
			remainingTroopOrigins = new List<IAgentOriginBase>();
			bool flag = battleSide == Mission.Current.PlayerTeam.Side;
			foreach (IAgentOriginBase agentOriginBase in troopOrigins)
			{
				Team agentTeam = Mission.GetAgentTeam(agentOriginBase, flag);
				List<IAgentOriginBase> list;
				if (!dictionary.TryGetValue(agentTeam, out list))
				{
					list = new List<IAgentOriginBase>();
					dictionary.Add(agentTeam, list);
				}
				list.Add(agentOriginBase);
			}
			List<ValueTuple<IAgentOriginBase, int>> list2 = new List<ValueTuple<IAgentOriginBase, int>>();
			foreach (KeyValuePair<Team, List<IAgentOriginBase>> keyValuePair in dictionary)
			{
				Team key = keyValuePair.Key;
				List<IAgentOriginBase> value = keyValuePair.Value;
				FormationFilterLogic missionBehavior = Mission.Current.GetMissionBehavior<FormationFilterLogic>();
				TeamFilter teamFilter = ((missionBehavior != null) ? missionBehavior.GetTeamFilter(key) : null);
				if (teamFilter == null)
				{
					remainingTroopOrigins.AddRange(value);
				}
				else
				{
					List<IAgentOriginBase> list3;
					list2.AddRange(teamFilter.GetReinforcementAssignments(key, value, out list3));
					remainingTroopOrigins.AddRange(list3);
				}
			}
			if (list2.Count + remainingTroopOrigins.Count != troopOrigins.Count)
			{
				Utility.DisplayAgentOriginCountMismatch();
			}
			return list2;
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x0000812C File Offset: 0x0000632C
		[return: TupleElementNames(new string[] { null, "formationIndex" })]
		[return: Nullable(new byte[] { 1, 0, 1 })]
		private List<ValueTuple<IAgentOriginBase, int>> GetReinforcementAssignments(Team team, List<IAgentOriginBase> troopOrigins, out List<IAgentOriginBase> remainingOrigins)
		{
			List<Agent> excludedAgents = Utility.GetExcludedAgents(team);
			List<IFilteredAgent> list = troopOrigins.Select<IAgentOriginBase, IFilteredAgent>((IAgentOriginBase troopOrigin) => new FilteredAgentOriginBase(troopOrigin)).ToList<IFilteredAgent>();
			Dictionary<ulong, ValueTuple<Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>, List<IFilteredAgent>>> dictionary;
			List<IFilteredAgent> list2;
			Dictionary<ulong, int> dictionary2;
			Dictionary<ulong, int> dictionary3;
			this.ComputeMinMaskToFilterAndAgents(team, list, out dictionary, out list2, out dictionary2, out dictionary3);
			remainingOrigins = (from filteredAgentOriginBase in list2
				select filteredAgentOriginBase as FilteredAgentOriginBase into filteredAgentOrign
				where filteredAgentOrign != null
				select filteredAgentOrign into filteredAgentOrigin
				select filteredAgentOrigin.AgentOriginBase).ToList<IAgentOriginBase>();
			List<IFilteredAgent> filteredAgentList = TeamFilter.GetFilteredAgentList(team, excludedAgents);
			Dictionary<ulong, ValueTuple<Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>, List<IFilteredAgent>>> dictionary4;
			List<IFilteredAgent> list3;
			this.ComputeMinMaskToFilterAndAgents(team, filteredAgentList, out dictionary4, out list3, out dictionary3, out dictionary2);
			List<ValueTuple<IAgentOriginBase, int>> list4 = new List<ValueTuple<IAgentOriginBase, int>>();
			foreach (KeyValuePair<ulong, ValueTuple<Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>, List<IFilteredAgent>>> keyValuePair in dictionary)
			{
				ulong key5 = keyValuePair.Key;
				Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>> item = keyValuePair.Value.Item1;
				List<IAgentOriginBase> list5 = (from filteredAgentOrigin in keyValuePair.Value.Item2
					select filteredAgentOrigin as FilteredAgentOriginBase into filteredAgentOrigin
					where filteredAgentOrigin != null
					select filteredAgentOrigin.AgentOriginBase).ToList<IAgentOriginBase>();
				Dictionary<TroopFilterIdentifier, float> troopFilterIdentifierWeightMap = (from pair in this.CollectTroopFilterIdentifierWeights(item)
					where pair.Key.Formation.GetReadonlyMovementOrderReference().OrderEnum != 8
					select pair).ToDictionary<KeyValuePair<TroopFilterIdentifier, float>, TroopFilterIdentifier, float>((KeyValuePair<TroopFilterIdentifier, float> pair) => pair.Key, (KeyValuePair<TroopFilterIdentifier, float> pair) => pair.Value);
				if (troopFilterIdentifierWeightMap.Count != 0)
				{
					float num = troopFilterIdentifierWeightMap.Sum<KeyValuePair<TroopFilterIdentifier, float>>((KeyValuePair<TroopFilterIdentifier, float> pair) => pair.Value);
					if (num == 0f)
					{
						foreach (TroopFilterIdentifier troopFilterIdentifier in troopFilterIdentifierWeightMap.Keys.ToList<TroopFilterIdentifier>())
						{
							troopFilterIdentifierWeightMap[troopFilterIdentifier] = 1f;
						}
						num = troopFilterIdentifierWeightMap.Sum<KeyValuePair<TroopFilterIdentifier, float>>((KeyValuePair<TroopFilterIdentifier, float> kvp) => kvp.Value);
					}
					List<IFilteredAgent> list6 = new List<IFilteredAgent>();
					ValueTuple<Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>, List<IFilteredAgent>> valueTuple;
					if (dictionary4.TryGetValue(key5, out valueTuple))
					{
						list6 = valueTuple.Item2;
					}
					Dictionary<TroopFilterIdentifier, List<Agent>> troopFiltersToExistingAgentsMap = troopFilterIdentifierWeightMap.Keys.ToDictionary<TroopFilterIdentifier, TroopFilterIdentifier, List<Agent>>((TroopFilterIdentifier key) => key, (TroopFilterIdentifier key) => new List<Agent>());
					Func<KeyValuePair<TroopFilterIdentifier, List<Agent>>, float> <>9__19;
					foreach (IFilteredAgent filteredAgent in list6)
					{
						FilteredAgent filteredAgent2 = filteredAgent as FilteredAgent;
						if (filteredAgent2 != null)
						{
							IEnumerable<KeyValuePair<TroopFilterIdentifier, List<Agent>>> troopFiltersToExistingAgentsMap2 = troopFiltersToExistingAgentsMap;
							Func<KeyValuePair<TroopFilterIdentifier, List<Agent>>, float> func;
							if ((func = <>9__19) == null)
							{
								func = (<>9__19 = delegate(KeyValuePair<TroopFilterIdentifier, List<Agent>> pair)
								{
									if (troopFilterIdentifierWeightMap[pair.Key] != 0f)
									{
										return (float)pair.Value.Count / troopFilterIdentifierWeightMap[pair.Key];
									}
									return float.MaxValue;
								});
							}
							TroopFilterIdentifier key2 = Extensions.MinBy<KeyValuePair<TroopFilterIdentifier, List<Agent>>, float>(troopFiltersToExistingAgentsMap2, func).Key;
							troopFiltersToExistingAgentsMap[key2].Add(filteredAgent2.Agent);
						}
					}
					Dictionary<Formation, List<IAgentOriginBase>> dictionary5 = troopFilterIdentifierWeightMap.Keys.Select<TroopFilterIdentifier, Formation>((TroopFilterIdentifier key) => key.Formation).Distinct<Formation>().ToDictionary<Formation, Formation, List<IAgentOriginBase>>((Formation key) => key, (Formation key) => new List<IAgentOriginBase>());
					Dictionary<TroopFilterIdentifier, List<IAgentOriginBase>> dictionary6 = troopFilterIdentifierWeightMap.Keys.ToDictionary<TroopFilterIdentifier, TroopFilterIdentifier, List<IAgentOriginBase>>((TroopFilterIdentifier key) => key, (TroopFilterIdentifier key) => new List<IAgentOriginBase>());
					Func<KeyValuePair<TroopFilterIdentifier, List<IAgentOriginBase>>, float> <>9__20;
					foreach (IAgentOriginBase agentOriginBase in list5)
					{
						IEnumerable<KeyValuePair<TroopFilterIdentifier, List<IAgentOriginBase>>> enumerable = dictionary6;
						Func<KeyValuePair<TroopFilterIdentifier, List<IAgentOriginBase>>, float> func2;
						if ((func2 = <>9__20) == null)
						{
							func2 = (<>9__20 = delegate(KeyValuePair<TroopFilterIdentifier, List<IAgentOriginBase>> pair)
							{
								if (troopFilterIdentifierWeightMap[pair.Key] != 0f)
								{
									return (float)(pair.Value.Count + troopFiltersToExistingAgentsMap[pair.Key].Count) / troopFilterIdentifierWeightMap[pair.Key];
								}
								return float.MaxValue;
							});
						}
						TroopFilterIdentifier key3 = Extensions.MinBy<KeyValuePair<TroopFilterIdentifier, List<IAgentOriginBase>>, float>(enumerable, func2).Key;
						dictionary6[key3].Add(agentOriginBase);
						dictionary5[key3.Formation].Add(agentOriginBase);
					}
					foreach (KeyValuePair<Formation, List<IAgentOriginBase>> keyValuePair2 in dictionary5)
					{
						Formation key4 = keyValuePair2.Key;
						List<IAgentOriginBase> value = keyValuePair2.Value;
						int formationIndex = key4.Index;
						list4.AddRange(value.Select<IAgentOriginBase, ValueTuple<IAgentOriginBase, int>>((IAgentOriginBase agentOrigin) => new ValueTuple<IAgentOriginBase, int>(agentOrigin, formationIndex)));
					}
				}
			}
			return list4;
		}

		// Token: 0x04000057 RID: 87
		[Nullable(new byte[] { 2, 1 })]
		private static List<FormationFilters> _customBattleSavedFilters;

		// Token: 0x04000059 RID: 89
		public static bool IsAdustingWeights;

		// Token: 0x0400005A RID: 90
		private Dictionary<Formation, FormationFilters> _allFilters = new Dictionary<Formation, FormationFilters>();

		// Token: 0x0400005B RID: 91
		private Dictionary<ulong, Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>> _intersectedFiltersMap = new Dictionary<ulong, Dictionary<ulong, Dictionary<Formation, List<TroopFilterIdentifier>>>>();

		// Token: 0x0400005C RID: 92
		private bool? _isPreviousSuccessful;

		// Token: 0x0400005D RID: 93
		private Dictionary<ulong, int> _totalUnitCountOfFilter = new Dictionary<ulong, int>();

		// Token: 0x0400005E RID: 94
		private Dictionary<ulong, int> _minimumUnitCountOfFilter = new Dictionary<ulong, int>();

		// Token: 0x0400005F RID: 95
		private Dictionary<TroopFilterIdentifier, int> _troopFiltersActualUnitCount = new Dictionary<TroopFilterIdentifier, int>();

		// Token: 0x04000060 RID: 96
		private Dictionary<ulong, Dictionary<TroopFilterIdentifier, int>> _intersectedFilterAssignedUnitCount = new Dictionary<ulong, Dictionary<TroopFilterIdentifier, int>>();
	}
}
