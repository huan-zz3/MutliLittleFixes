using System;
using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace AutoResolveRebalanced
{
	// Token: 0x02000006 RID: 6
	[HarmonyPatch(typeof(MapEventSide), "ApplySimulationDamageToSelectedTroop")]
	internal class Patch_ApplySimulationDamageToSelectedTroop
	{
		// Token: 0x0600000A RID: 10 RVA: 0x000024BC File Offset: 0x000006BC
		public unsafe static bool Prefix(ref int damage, DamageTypes damageType, PartyBase strikerParty, MapEventSide __instance, ref bool __result, ref CharacterObject ____selectedSimulationTroop, ref UniqueTroopDescriptor ____selectedSimulationTroopDescriptor, ref int ____selectedSimulationTroopIndex, ref List<UniqueTroopDescriptor> ____simulationTroopList, ref Dictionary<UniqueTroopDescriptor, MapEventParty> ____allocatedTroops)
		{
			Settings settings = new Settings();
			bool flag2;
			try
			{
				if (settings.aiEnabled || __instance.MapEvent.IsPlayerSimulation)
				{
					bool flag = false;
					CharacterObject characterObject = *MapEventSideAccessTools._selectedSimulationTroop.Invoke(__instance);
					CharacterObject characterObject2 = *MapEventSideAccessTools._selectedSimulationTroop.Invoke(__instance.OtherSide);
					damage = (int)((float)damage * settings.damageModifier);
					if (characterObject.IsHero)
					{
						if (characterObject != null && characterObject2 != null)
						{
							int hitPoints = characterObject.HitPoints;
							int num = hitPoints - damage;
							string text = characterObject.ToString() + "(L" + characterObject.Level.ToString() + ")";
							string text2;
							if (characterObject2.IsHero)
							{
								text2 = characterObject2.ToString() + "(L" + characterObject2.Level.ToString() + ")";
							}
							else
							{
								text2 = characterObject2.ToString() + "(T" + characterObject2.Tier.ToString() + ")";
							}
							Debugger.Message(string.Concat(new string[]
							{
								text2,
								" deals ",
								damage.ToString(),
								"DMG to ",
								text,
								". HP decreased ",
								hitPoints.ToString(),
								" to ",
								num.ToString(),
								"."
							}), Debugger.Type.Log, __instance.MapEvent, false);
						}
						flag2 = true;
					}
					else
					{
						SimulateData simulateData;
						if (!SimulateDataDict.GetData(__instance, out simulateData))
						{
							simulateData = new SimulateData(__instance, ____simulationTroopList);
							SimulateDataDict.AddData(__instance, simulateData);
							Debugger.Message("Failed GetData at ApplyDamage, SD generated.", Debugger.Type.Warn, __instance.MapEvent, false);
						}
						int hitPoints;
						if (!simulateData.GetHitPoint(____selectedSimulationTroopDescriptor, out hitPoints))
						{
							simulateData.Clear(true);
							SimulateDataDict.RemoveData(__instance);
							simulateData = new SimulateData(__instance, ____simulationTroopList);
							SimulateDataDict.AddData(__instance, simulateData);
							Debugger.Message("Failed GetHitPoint on ApplyDamage, SD generated.", Debugger.Type.Warn, __instance.MapEvent, false);
							if (!simulateData.GetHitPoint(____selectedSimulationTroopDescriptor, out hitPoints))
							{
								Debugger.Message("Error on GetHitPoint at ApplyDamage.", Debugger.Type.Error, __instance.MapEvent, false);
							}
						}
						int num = hitPoints - damage;
						simulateData.SetHitPoint(____selectedSimulationTroopDescriptor, num);
						if (characterObject != null && characterObject2 != null)
						{
							string text3 = characterObject.ToString() + "(T" + characterObject.Tier.ToString() + ")";
							string text4;
							if (characterObject2.IsHero)
							{
								text4 = characterObject2.ToString() + "(L" + characterObject2.Level.ToString() + ")";
							}
							else
							{
								text4 = characterObject2.ToString() + "(T" + characterObject2.Tier.ToString() + ")";
							}
							Debugger.Message(string.Concat(new string[]
							{
								text4,
								" deals ",
								damage.ToString(),
								"DMG to ",
								text3,
								". HP decreased ",
								hitPoints.ToString(),
								" to ",
								num.ToString(),
								"."
							}), Debugger.Type.Log, __instance.MapEvent, false);
						}
						if (num <= 0)
						{
							PartyBase party = ____allocatedTroops[____selectedSimulationTroopDescriptor].Party;
							float survivalChance = Campaign.Current.Models.PartyHealingModel.GetSurvivalChance(party, ____selectedSimulationTroop, damageType, false, strikerParty);
							if (MBRandom.RandomFloat < survivalChance)
							{
								__instance.OnTroopWounded(____selectedSimulationTroopDescriptor);
								IBattleObserver battleObserver = MapEventSideAccessTools.GetBattleObserver(__instance);
								if (battleObserver != null)
								{
									battleObserver.TroopNumberChanged(__instance.MissionSide, __instance.GetAllocatedTroopParty(____selectedSimulationTroopDescriptor), ____selectedSimulationTroop, -1, 0, 1, 0, 0, 0);
								}
								SkillLevelingManager.OnSurgeryApplied(party.MobileParty, true, ____selectedSimulationTroop.Tier);
								if (((strikerParty != null) ? strikerParty.MobileParty : null) != null && strikerParty.MobileParty.HasPerk(DefaultPerks.Medicine.DoctorsOath, false))
								{
									SkillLevelingManager.OnSurgeryApplied(strikerParty.MobileParty, true, ____selectedSimulationTroop.Tier);
								}
							}
							else
							{
								__instance.OnTroopKilled(____selectedSimulationTroopDescriptor);
								IBattleObserver battleObserver2 = MapEventSideAccessTools.GetBattleObserver(__instance);
								if (battleObserver2 != null)
								{
									battleObserver2.TroopNumberChanged(__instance.MissionSide, __instance.GetAllocatedTroopParty(____selectedSimulationTroopDescriptor), ____selectedSimulationTroop, -1, 1, 0, 0, 0, 0);
								}
								SkillLevelingManager.OnSurgeryApplied(party.MobileParty, false, ____selectedSimulationTroop.Tier);
								if (((strikerParty != null) ? strikerParty.MobileParty : null) != null && strikerParty.MobileParty.HasPerk(DefaultPerks.Medicine.DoctorsOath, false))
								{
									SkillLevelingManager.OnSurgeryApplied(strikerParty.MobileParty, false, ____selectedSimulationTroop.Tier);
								}
							}
							flag = true;
						}
						if (flag)
						{
							MapEventSideAccessTools.InvokeRemoveSelectedTroopFromSimulationList(__instance);
						}
						__result = flag;
						flag2 = false;
					}
				}
				else
				{
					flag2 = true;
				}
			}
			catch (Exception ex)
			{
				Debugger.Message(ex.ToString(), Debugger.Type.Exception, null, false);
				flag2 = true;
			}
			return flag2;
		}
	}
}
