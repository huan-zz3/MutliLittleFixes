using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FormationFilter.Config;
using FormationFilter.Models;
using FormationFilter.View.ViewModels;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;

namespace FormationFilter.Utilities
{
	// Token: 0x02000012 RID: 18
	[NullableContext(1)]
	[Nullable(0)]
	public class Utility
	{
		// Token: 0x060000A6 RID: 166 RVA: 0x0000510C File Offset: 0x0000330C
		private static FilterTypeEnum GetInvalidFilterType(ulong bitmask)
		{
			for (FilterTypeEnum filterTypeEnum = FilterTypeEnum.HasMount; filterTypeEnum < FilterTypeEnum.Count; filterTypeEnum++)
			{
				int num = (int)(filterTypeEnum * FilterTypeEnum.HasTwoHanded);
				if (((bitmask >> num) & 3UL) == 0UL)
				{
					return filterTypeEnum;
				}
			}
			return FilterTypeEnum.Count;
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x0000513B File Offset: 0x0000333B
		public static DeploymentFormationClass GetDeploymentFormationClass(bool hasInfantry, bool hasRanged, bool hasCavalry, bool hasHorseArcher)
		{
			if (hasInfantry)
			{
				if (hasRanged)
				{
					return 5;
				}
				return 1;
			}
			else
			{
				if (hasRanged)
				{
					return 2;
				}
				if (hasCavalry)
				{
					if (hasHorseArcher)
					{
						return 6;
					}
					return 3;
				}
				else
				{
					if (hasHorseArcher)
					{
						return 4;
					}
					return 0;
				}
			}
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x0000515C File Offset: 0x0000335C
		public static void DisplayException(Exception e)
		{
			InformationManager.DisplayMessage(new InformationMessage(e.ToString(), new Color(1f, 0f, 0f, 1f)));
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00005188 File Offset: 0x00003388
		public static void DisplayInvalidFilterBitmask(ulong bitmask)
		{
			FilterTypeEnum invalidFilterType = Utility.GetInvalidFilterType(bitmask);
			InformationManager.DisplayMessage(new InformationMessage(string.Format("FormationFilter: Invalid filter bitmask {0} with filterType {1} encountered.", bitmask, invalidFilterType), new Color(1f, 0f, 0f, 1f)));
		}

		// Token: 0x060000AA RID: 170 RVA: 0x000051D5 File Offset: 0x000033D5
		public static void DisplayInvalidCustomFormationFilterType(CustomFormationFilterType customFormationFilterType)
		{
			InformationManager.DisplayMessage(new InformationMessage(string.Format("FormationFilter: Invalid custom formation filter {0}", customFormationFilterType), new Color(1f, 0f, 0f, 1f)));
		}

		// Token: 0x060000AB RID: 171 RVA: 0x0000520C File Offset: 0x0000340C
		public static void DisplayRemainingAgents(List<Agent> agents)
		{
			string text = "FormationFilter: ";
			TextObject textObject = GameTexts.FindText("str_formation_filter_result_fail", null).SetTextVariable("number", agents.Count);
			string text2 = "name";
			Agent agent = agents.FirstOrDefault<Agent>();
			InformationManager.DisplayMessage(new InformationMessage(text + textObject.SetTextVariable(text2, ((agent != null) ? agent.Name : null) ?? "").ToString(), new Color(0.8f, 0.3f, 0.3f, 1f)));
		}

		// Token: 0x060000AC RID: 172 RVA: 0x0000528B File Offset: 0x0000348B
		public static void DisplayNoRemainingAgents()
		{
			InformationManager.DisplayMessage(new InformationMessage("FormationFilter: " + GameTexts.FindText("str_formation_filter_result_success", null).ToString(), new Color(0.3f, 0.8f, 0.3f, 1f)));
		}

		// Token: 0x060000AD RID: 173 RVA: 0x000052CA File Offset: 0x000034CA
		public static void DisplayAgentOriginCountMismatch()
		{
			InformationManager.DisplayMessage(new InformationMessage("FormationFilter: Agent origin count mismatch encountered.", new Color(0.3f, 0.8f, 0.3f, 1f)));
		}

		// Token: 0x060000AE RID: 174 RVA: 0x000052F4 File Offset: 0x000034F4
		public static bool HasFilterType(IAgentOriginBase agentOriginBase, FilterTypeEnum filterTypeEnum)
		{
			BasicCharacterObject troop = agentOriginBase.Troop;
			if (troop == null)
			{
				return false;
			}
			switch (filterTypeEnum)
			{
			case FilterTypeEnum.HasMount:
				return troop.HasMount() && !Mission.Current.IsSiegeBattle;
			case FilterTypeEnum.HasOneHanded:
			case FilterTypeEnum.HasTwoHanded:
			case FilterTypeEnum.HasPolearm:
			case FilterTypeEnum.HasRanged:
			case FilterTypeEnum.HasThrowing:
			case FilterTypeEnum.HasShield:
			case FilterTypeEnum.HasBow:
			case FilterTypeEnum.HasCrossBow:
			case FilterTypeEnum.HasSling:
			{
				for (int i = 0; i < 5; i++)
				{
					EquipmentElement equipmentElement = troop.FirstBattleEquipment[i];
					if (!equipmentElement.IsEmpty && equipmentElement.Item.HasWeaponComponent && Utility.DoesItemSatisfyFilter(equipmentElement.Item, filterTypeEnum))
					{
						return true;
					}
				}
				return false;
			}
			case FilterTypeEnum.HeavyArmor:
				return agentOriginBase.HasHeavyArmor;
			case FilterTypeEnum.HighTier:
			{
				int battleTier = troop.GetBattleTier();
				FormationFilterSettings instance = GlobalSettings<FormationFilterSettings>.Instance;
				int? num = ((instance != null) ? new int?(instance.HighTierThreshold) : null);
				return (battleTier >= num.GetValueOrDefault()) & (num != null);
			}
			case FilterTypeEnum.LowTier:
			{
				int battleTier2 = troop.GetBattleTier();
				FormationFilterSettings instance2 = GlobalSettings<FormationFilterSettings>.Instance;
				int? num = ((instance2 != null) ? new int?(instance2.LowTierThreshold) : null);
				return (battleTier2 <= num.GetValueOrDefault()) & (num != null);
			}
			case FilterTypeEnum.HasOneHandedSword:
			case FilterTypeEnum.HasOneHandedAxe:
			case FilterTypeEnum.HasOneHandedMace:
			case FilterTypeEnum.HasTwoHandedSword:
			case FilterTypeEnum.HasTwoHandedAxe:
			case FilterTypeEnum.HasTwoHandedMace:
			case FilterTypeEnum.HasOneHandedPolearm:
			case FilterTypeEnum.HasTwoHandedPolearm:
			case FilterTypeEnum.HasThrowingAxe:
			case FilterTypeEnum.HasThrowingKnife:
			case FilterTypeEnum.HasJavelin:
			case FilterTypeEnum.HasSmallShield:
			case FilterTypeEnum.HasLargeShield:
			{
				WeaponClass weaponClass = filterTypeEnum.ToWeaponClass();
				return weaponClass != null && troop.FirstBattleEquipment.HasWeaponOfClass(weaponClass);
			}
			default:
				InformationManager.DisplayMessage(new InformationMessage("FormationFilter: Unexpected filter type encountered.", new Color(1f, 0f, 0f, 1f)));
				return false;
			}
		}

		// Token: 0x060000AF RID: 175 RVA: 0x0000549C File Offset: 0x0000369C
		public static bool HasFilterType(Agent agent, FilterTypeEnum filterTypeEnum)
		{
			if (agent.Equipment == null)
			{
				return false;
			}
			switch (filterTypeEnum)
			{
			case FilterTypeEnum.HasMount:
			{
				BasicCharacterObject character = agent.Character;
				return character != null && character.HasMount() && !Mission.Current.IsSiegeBattle;
			}
			case FilterTypeEnum.HasOneHanded:
			case FilterTypeEnum.HasTwoHanded:
			case FilterTypeEnum.HasPolearm:
			case FilterTypeEnum.HasRanged:
			case FilterTypeEnum.HasThrowing:
			case FilterTypeEnum.HasShield:
			case FilterTypeEnum.HasBow:
			case FilterTypeEnum.HasCrossBow:
			case FilterTypeEnum.HasSling:
			{
				for (int i = 0; i < 5; i++)
				{
					MissionWeapon missionWeapon = agent.Equipment[i];
					if (!missionWeapon.IsEmpty && missionWeapon.Item.HasWeaponComponent && Utility.DoesItemSatisfyFilter(missionWeapon.Item, filterTypeEnum))
					{
						return true;
					}
				}
				return false;
			}
			case FilterTypeEnum.HeavyArmor:
				return agent.Origin.HasHeavyArmor;
			case FilterTypeEnum.HighTier:
			{
				int battleTier = agent.Character.GetBattleTier();
				FormationFilterSettings instance = GlobalSettings<FormationFilterSettings>.Instance;
				int? num = ((instance != null) ? new int?(instance.HighTierThreshold) : null);
				return (battleTier >= num.GetValueOrDefault()) & (num != null);
			}
			case FilterTypeEnum.LowTier:
			{
				int battleTier2 = agent.Character.GetBattleTier();
				FormationFilterSettings instance2 = GlobalSettings<FormationFilterSettings>.Instance;
				int? num = ((instance2 != null) ? new int?(instance2.LowTierThreshold) : null);
				return (battleTier2 <= num.GetValueOrDefault()) & (num != null);
			}
			case FilterTypeEnum.HasOneHandedSword:
			case FilterTypeEnum.HasOneHandedAxe:
			case FilterTypeEnum.HasOneHandedMace:
			case FilterTypeEnum.HasTwoHandedSword:
			case FilterTypeEnum.HasTwoHandedAxe:
			case FilterTypeEnum.HasTwoHandedMace:
			case FilterTypeEnum.HasOneHandedPolearm:
			case FilterTypeEnum.HasTwoHandedPolearm:
			case FilterTypeEnum.HasThrowingAxe:
			case FilterTypeEnum.HasThrowingKnife:
			case FilterTypeEnum.HasJavelin:
			case FilterTypeEnum.HasSmallShield:
			case FilterTypeEnum.HasLargeShield:
			{
				WeaponClass weaponClass = filterTypeEnum.ToWeaponClass();
				if (weaponClass == null)
				{
					return false;
				}
				for (int j = 0; j < 5; j++)
				{
					MissionWeapon missionWeapon2 = agent.Equipment[j];
					if (!missionWeapon2.IsEmpty && missionWeapon2.Item.HasWeaponComponent && Utility.HasWeaponOfClass(missionWeapon2.Item.WeaponComponent, weaponClass, false))
					{
						return true;
					}
				}
				return false;
			}
			default:
				InformationManager.DisplayMessage(new InformationMessage("FormationFilter: Unexpected filter type encountered.", new Color(1f, 0f, 0f, 1f)));
				return false;
			}
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x0000569C File Offset: 0x0000389C
		private static bool DoesItemSatisfyFilter(ItemObject item, FilterTypeEnum filterTypeEnum)
		{
			switch (filterTypeEnum)
			{
			case FilterTypeEnum.HasOneHanded:
				return Utility.IsOneHanded(item);
			case FilterTypeEnum.HasTwoHanded:
				return Utility.IsTwoHanded(item);
			case FilterTypeEnum.HasPolearm:
				return Utility.IsPolearm(item);
			case FilterTypeEnum.HasRanged:
				return Utility.IsRanged(item);
			case FilterTypeEnum.HasThrowing:
				return Utility.IsThrowing(item);
			case FilterTypeEnum.HasShield:
				return Utility.IsShield(item);
			case FilterTypeEnum.HasBow:
				return Utility.IsBow(item);
			case FilterTypeEnum.HasCrossBow:
				return Utility.IsCrossbow(item);
			case FilterTypeEnum.HasSling:
				return Utility.IsSling(item);
			}
			InformationManager.DisplayMessage(new InformationMessage("FormationFilter: Unexpected filter type encountered in item check.", new Color(1f, 0f, 0f, 1f)));
			return false;
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x0000574C File Offset: 0x0000394C
		private static bool IsOneHanded(ItemObject item)
		{
			return Utility.HasWeaponOfClass(item.WeaponComponent, 1, false) || Utility.HasWeaponOfClass(item.WeaponComponent, 2, false) || Utility.HasWeaponOfClass(item.WeaponComponent, 4, false) || Utility.HasWeaponOfClass(item.WeaponComponent, 6, false) || Utility.HasWeaponOfClass(item.WeaponComponent, 7, false);
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x000057A8 File Offset: 0x000039A8
		private static bool IsTwoHanded(ItemObject item)
		{
			FormationFilterSettings instance = GlobalSettings<FormationFilterSettings>.Instance;
			return (instance != null && instance.TreatSwingPolearmAsTwoHandedWeapon && (Utility.HasWeaponOfClass(item.WeaponComponent, 9, true) || Utility.HasWeaponOfClass(item.WeaponComponent, 10, true) || Utility.HasWeaponOfClass(item.WeaponComponent, 11, true))) || (Utility.HasWeaponOfClass(item.WeaponComponent, 3, false) || Utility.HasWeaponOfClass(item.WeaponComponent, 5, false) || Utility.HasWeaponOfClass(item.WeaponComponent, 8, false));
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x0000582C File Offset: 0x00003A2C
		private static bool IsPolearm(ItemObject item)
		{
			FormationFilterSettings instance = GlobalSettings<FormationFilterSettings>.Instance;
			if (instance != null && instance.TreatSwingPolearmAsTwoHandedWeapon && (Utility.HasWeaponOfClass(item.WeaponComponent, 9, true) || Utility.HasWeaponOfClass(item.WeaponComponent, 10, true) || Utility.HasWeaponOfClass(item.WeaponComponent, 11, true)))
			{
				return false;
			}
			if (item.ItemType == 4)
			{
				FormationFilterSettings instance2 = GlobalSettings<FormationFilterSettings>.Instance;
				return (instance2 != null && instance2.TreatThrowingSpearAsPolearm) || !Utility.HasWeaponOfClass(item.WeaponComponent, 23, false);
			}
			return false;
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x000058B0 File Offset: 0x00003AB0
		private static bool IsRanged(ItemObject item)
		{
			return Utility.HasWeaponOfClass(item.WeaponComponent, 16, false) || Utility.HasWeaponOfClass(item.WeaponComponent, 17, false) || Utility.HasWeaponOfClass(item.WeaponComponent, 24, false) || Utility.HasWeaponOfClass(item.WeaponComponent, 25, false);
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00005900 File Offset: 0x00003B00
		private static bool IsThrowing(ItemObject item)
		{
			FormationFilterSettings instance = GlobalSettings<FormationFilterSettings>.Instance;
			return ((instance != null && instance.TreatThrowingSpearAsThrowingWeapon) || item.ItemType != 4) && (Utility.HasWeaponOfClass(item.WeaponComponent, 19, false) || Utility.HasWeaponOfClass(item.WeaponComponent, 20, false) || Utility.HasWeaponOfClass(item.WeaponComponent, 21, false) || Utility.HasWeaponOfClass(item.WeaponComponent, 22, false) || Utility.HasWeaponOfClass(item.WeaponComponent, 23, false));
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x0000597C File Offset: 0x00003B7C
		private static bool IsShield(ItemObject item)
		{
			WeaponClass weaponClass = item.PrimaryWeapon.WeaponClass;
			return weaponClass == 28 || weaponClass == 29;
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x000059A2 File Offset: 0x00003BA2
		private static bool IsBow(ItemObject item)
		{
			return Utility.HasWeaponOfClass(item.WeaponComponent, 16, false);
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x000059B7 File Offset: 0x00003BB7
		private static bool IsCrossbow(ItemObject item)
		{
			return Utility.HasWeaponOfClass(item.WeaponComponent, 17, false);
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x000059CC File Offset: 0x00003BCC
		private static bool IsSling(ItemObject item)
		{
			return Utility.HasWeaponOfClass(item.WeaponComponent, 18, false);
		}

		// Token: 0x060000BA RID: 186 RVA: 0x000059E4 File Offset: 0x00003BE4
		private static bool HasWeaponOfClass(WeaponComponent weaponComponent, WeaponClass weaponClass, bool requireSwingDamage = false)
		{
			foreach (WeaponComponentData weaponComponentData in weaponComponent.Weapons)
			{
				if (weaponComponentData.WeaponClass == weaponClass && (!requireSwingDamage || weaponComponentData.SwingDamageType != -1))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00005A4C File Offset: 0x00003C4C
		public unsafe static void CopyOrdersFrom(Formation self, Formation target)
		{
			self.SetMovementOrder(*target.GetReadonlyMovementOrderReference());
			self.SetFormOrder(target.FormOrder, true);
			int? num = new int?(target.UnitSpacing);
			self.SetPositioning(null, null, num);
			self.SetRidingOrder(target.RidingOrder);
			self.SetFiringOrder(target.FiringOrder);
			self.SetControlledByAI(target.IsAIControlled || !target.Team.IsPlayerGeneral, false);
			if (target.AI.Side != 3)
			{
				self.AI.Side = target.AI.Side;
			}
			self.SetMovementOrder(*target.GetReadonlyMovementOrderReference());
			self.SetTargetFormation(target.TargetFormation);
			self.SetFacingOrder(target.FacingOrder);
			self.SetArrangementOrder(target.ArrangementOrder);
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00005B30 File Offset: 0x00003D30
		public static List<Agent> GetExcludedAgents(OrderOfBattleFormationItemVM formationVM)
		{
			List<Agent> list = new List<Agent>();
			if (formationVM.HasCaptain)
			{
				list.Add(formationVM.Captain.Agent);
			}
			if (formationVM.HeroTroops.Count > 0)
			{
				list.AddRange(formationVM.HeroTroops.Select<OrderOfBattleHeroItemVM, Agent>((OrderOfBattleHeroItemVM t) => t.Agent));
			}
			foreach (IFormationUnit formationUnit in formationVM.Formation.Arrangement.GetAllUnits())
			{
				Agent agent = (Agent)formationUnit;
				if (agent.Banner != null)
				{
					list.Add(agent);
				}
			}
			return list.Distinct<Agent>().ToList<Agent>();
		}

		// Token: 0x060000BD RID: 189 RVA: 0x00005C04 File Offset: 0x00003E04
		public static List<Agent> GetExcludedAgents(Team team)
		{
			List<Agent> result = new List<Agent>();
			Action<Agent> <>9__0;
			foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty)
			{
				Formation formation2 = formation;
				Action<Agent> action;
				if ((action = <>9__0) == null)
				{
					action = (<>9__0 = delegate(Agent agent)
					{
						if (agent.IsHero || agent.Banner != null)
						{
							result.Add(agent);
						}
					});
				}
				formation2.ApplyActionOnEachUnit(action, null);
			}
			return result;
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00005C90 File Offset: 0x00003E90
		public static FormationClass ToFormationClass(DeploymentFormationClass deploymentFormationClass)
		{
			if (deploymentFormationClass != null)
			{
				return deploymentFormationClass - 1;
			}
			return 10;
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00005C9B File Offset: 0x00003E9B
		public static DeploymentFormationClass ToDeploymentFormationClass(FormationClass formationClass)
		{
			if (formationClass != 10)
			{
				return formationClass + 1;
			}
			return 0;
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00005CA7 File Offset: 0x00003EA7
		public static DeploymentFormationClass ToFootmanDeploymentFormationClass(DeploymentFormationClass deploymentFormationClass)
		{
			switch (deploymentFormationClass)
			{
			case 0:
			case 1:
			case 2:
			case 5:
				return deploymentFormationClass;
			case 3:
				return 1;
			case 4:
				return 2;
			case 6:
				return 5;
			default:
				return 0;
			}
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00005CD6 File Offset: 0x00003ED6
		public static FormationClass GetActualTroopType(Agent agent)
		{
			if (QueryLibrary.IsInfantry(agent))
			{
				return 0;
			}
			if (QueryLibrary.IsRanged(agent))
			{
				return 1;
			}
			if (QueryLibrary.IsCavalry(agent))
			{
				return 2;
			}
			if (!QueryLibrary.IsRangedCavalry(agent))
			{
				return 10;
			}
			return 3;
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00005D04 File Offset: 0x00003F04
		public static int GetTotalTroopCountWithFilter(Team team, FilterTypeEnum filterType, FilterValueEnum filterValue)
		{
			int num = 0;
			Func<Agent, bool> <>9__0;
			foreach (Formation formation in team.FormationsIncludingEmpty)
			{
				int num2 = num;
				Formation formation2 = formation;
				Func<Agent, bool> func;
				if ((func = <>9__0) == null)
				{
					func = (<>9__0 = (Agent agent) => TroopFilter.GetFilterEnum(agent, filterType) == filterValue);
				}
				num = num2 + formation2.GetCountOfUnitsWithCondition(func);
			}
			return num;
		}
	}
}
