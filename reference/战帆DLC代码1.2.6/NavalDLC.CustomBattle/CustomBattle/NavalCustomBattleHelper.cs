using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions.MissionLogics;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.CustomBattle.CustomBattle
{
	// Token: 0x02000012 RID: 18
	public static class NavalCustomBattleHelper
	{
		// Token: 0x060000B8 RID: 184 RVA: 0x00004E8C File Offset: 0x0000308C
		public static void StartGame(NavalCustomBattleData data)
		{
			Game.Current.PlayerTroop = data.PlayerCharacter;
			if (data.GameTypeStringId == "NavalBattle")
			{
				CustomNavalMissions.OpenNavalBattleForCustomMission(data.SceneId, data.PlayerCharacter, data.PlayerParty, Extensions.ToMBList<IShipOrigin>(data.PlayerShips), data.EnemyParty, Extensions.ToMBList<IShipOrigin>(data.EnemyShips), true, data.SeasonId, data.TimeOfDay, data.WindStrength, data.WindDirection, data.Terrain, data.ForcedSceneLevel);
				return;
			}
			if (data.GameTypeStringId == "NavalRaid")
			{
				MBList<IShipOrigin> mblist = ((data.PlayerParty.Side == 1) ? Extensions.ToMBList<IShipOrigin>(data.PlayerShips) : Extensions.ToMBList<IShipOrigin>(data.EnemyShips));
				CustomNavalMissions.OpenNavalRaidBattleForCustomMission(data.SceneId, data.PlayerCharacter, data.PlayerParty, data.EnemyParty, mblist, true, data.SeasonId, data.TimeOfDay, 0.5f, NavalCustomBattleWindConfig.Direction.TowardsAttacker, data.Terrain, data.ForcedSceneLevel);
				return;
			}
			Debug.FailedAssert("NavalCustomBattleData.GameTypeStringId: \"" + data.GameTypeStringId + "\" is invalid!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.CustomBattle\\CustomBattle\\NavalCustomBattleHelper.cs", "StartGame", 76);
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00004FB4 File Offset: 0x000031B4
		public static NavalCustomBattleData PrepareBattleData(BasicCharacterObject playerCharacter, CustomBattleCombatant playerParty, List<IShipOrigin> playerShips, CustomBattleCombatant enemyParty, List<IShipOrigin> enemyShips, string gameTypeStringId, string scene, string season, float timeOfDay, float windStrength, NavalCustomBattleWindConfig.Direction windDirection, TerrainType terrain, string forcedSceneLevel)
		{
			return new NavalCustomBattleData
			{
				GameTypeStringId = gameTypeStringId,
				SceneId = scene,
				PlayerCharacter = playerCharacter,
				PlayerParty = playerParty,
				PlayerShips = playerShips,
				EnemyParty = enemyParty,
				EnemyShips = enemyShips,
				SeasonId = season,
				TimeOfDay = timeOfDay,
				WindStrength = windStrength,
				WindDirection = windDirection,
				Terrain = terrain,
				ForcedSceneLevel = forcedSceneLevel
			};
		}

		// Token: 0x060000BA RID: 186 RVA: 0x0000503C File Offset: 0x0000323C
		public static CustomBattleCombatant[] GetCustomBattleParties(BasicCharacterObject playerCharacter, BasicCharacterObject enemyCharacter, List<BasicCharacterObject> remainingHeroes, BasicCultureObject playerFaction, int[] playerNumbers, List<BasicCharacterObject>[] playerTroopSelections, int playerHeroCount, BasicCultureObject enemyFaction, int[] enemyNumbers, List<BasicCharacterObject>[] enemyTroopSelections, int enemyHeroCount, bool isPlayerAttacker)
		{
			string text;
			if (playerFaction == null)
			{
				text = null;
			}
			else
			{
				Banner banner = playerFaction.Banner;
				text = ((banner != null) ? banner.BannerCode : null);
			}
			Banner banner2;
			if (Banner.IsValidBannerCode(text ?? string.Empty))
			{
				banner2 = new Banner(playerFaction.Banner, playerFaction.Color, playerFaction.Color2);
			}
			else
			{
				string text2 = "Banner code for player faction is not valid: ";
				string text3;
				if (playerFaction == null)
				{
					text3 = null;
				}
				else
				{
					Banner banner3 = playerFaction.Banner;
					text3 = ((banner3 != null) ? banner3.BannerCode : null);
				}
				Debug.FailedAssert(text2 + text3, "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.CustomBattle\\CustomBattle\\NavalCustomBattleHelper.cs", "GetCustomBattleParties", 126);
				banner2 = Banner.CreateOneColoredEmptyBanner(92);
			}
			string text4;
			if (enemyFaction == null)
			{
				text4 = null;
			}
			else
			{
				Banner banner4 = enemyFaction.Banner;
				text4 = ((banner4 != null) ? banner4.BannerCode : null);
			}
			Banner banner5;
			if (Banner.IsValidBannerCode(text4 ?? string.Empty))
			{
				banner5 = new Banner(enemyFaction.Banner, enemyFaction.Color, enemyFaction.Color2);
			}
			else
			{
				string text5 = "Banner code for enemy faction is not valid: ";
				string text6;
				if (playerFaction == null)
				{
					text6 = null;
				}
				else
				{
					Banner banner6 = playerFaction.Banner;
					text6 = ((banner6 != null) ? banner6.BannerCode : null);
				}
				Debug.FailedAssert(text5 + text6, "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.CustomBattle\\CustomBattle\\NavalCustomBattleHelper.cs", "GetCustomBattleParties", 136);
				banner5 = Banner.CreateOneColoredEmptyBanner(92);
			}
			if (playerFaction.StringId == enemyFaction.StringId)
			{
				uint primaryColor = banner5.GetPrimaryColor();
				banner5.ChangePrimaryColor(banner5.GetFirstIconColor());
				banner5.ChangeIconColors(primaryColor);
			}
			CustomBattleCombatant[] array = new CustomBattleCombatant[]
			{
				new CustomBattleCombatant(new TextObject("{=sSJSTe5p}Player Party", null), playerFaction, banner2),
				new CustomBattleCombatant(new TextObject("{=0xC75dN6}Enemy Party", null), enemyFaction, banner5)
			};
			int num = playerHeroCount - 1;
			int num2 = enemyHeroCount - 1;
			array[0].Side = (isPlayerAttacker ? 1 : 0);
			array[0].AddCharacter(playerCharacter, 1);
			array[0].SetGeneral(playerCharacter);
			for (int i = 0; i < num; i++)
			{
				int num3 = MBRandom.RandomInt(0, remainingHeroes.Count);
				array[0].AddCharacter(remainingHeroes[num3], 1);
				remainingHeroes.RemoveAt(num3);
			}
			array[1].Side = Extensions.GetOppositeSide(array[0].Side);
			array[1].AddCharacter(enemyCharacter, 1);
			for (int j = 0; j < num2; j++)
			{
				int num4 = MBRandom.RandomInt(0, remainingHeroes.Count);
				array[1].AddCharacter(remainingHeroes[num4], 1);
				remainingHeroes.RemoveAt(num4);
			}
			for (int k = 0; k < array.Length; k++)
			{
				NavalCustomBattleHelper.PopulateListsWithDefaults(ref array[k], (k == 0) ? playerNumbers : enemyNumbers, (k == 0) ? playerTroopSelections : enemyTroopSelections);
			}
			return array;
		}

		// Token: 0x060000BB RID: 187 RVA: 0x000052A0 File Offset: 0x000034A0
		public static List<IShipOrigin>[] GetCustomBattleShipLists(List<IShipOrigin> playerShips, List<IShipOrigin> enemyShips)
		{
			List<IShipOrigin>[] array = new List<IShipOrigin>[]
			{
				new List<IShipOrigin>(),
				new List<IShipOrigin>()
			};
			using (List<IShipOrigin>.Enumerator enumerator = playerShips.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CustomBattleShip customBattleShip;
					if ((customBattleShip = enumerator.Current as CustomBattleShip) != null)
					{
						array[0].Add(customBattleShip.GetCopy());
					}
				}
			}
			using (List<IShipOrigin>.Enumerator enumerator = enemyShips.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CustomBattleShip customBattleShip2;
					if ((customBattleShip2 = enumerator.Current as CustomBattleShip) != null)
					{
						array[1].Add(customBattleShip2.GetCopy());
					}
				}
			}
			return array;
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00005364 File Offset: 0x00003564
		private static void PopulateListsWithDefaults(ref CustomBattleCombatant customBattleParties, int[] numbers, List<BasicCharacterObject>[] troopList)
		{
			BasicCultureObject basicCulture = customBattleParties.BasicCulture;
			if (troopList == null)
			{
				troopList = new List<BasicCharacterObject>[]
				{
					new List<BasicCharacterObject>(),
					new List<BasicCharacterObject>(),
					new List<BasicCharacterObject>(),
					new List<BasicCharacterObject>()
				};
			}
			if (troopList[0].Count == 0)
			{
				troopList[0] = new List<BasicCharacterObject> { NavalCustomBattleHelper.GetDefaultTroopOfFormationForFaction(basicCulture, 0) };
			}
			if (troopList[1].Count == 0)
			{
				troopList[1] = new List<BasicCharacterObject> { NavalCustomBattleHelper.GetDefaultTroopOfFormationForFaction(basicCulture, 1) };
			}
			if (troopList[2].Count == 0)
			{
				troopList[2] = new List<BasicCharacterObject> { NavalCustomBattleHelper.GetDefaultTroopOfFormationForFaction(basicCulture, 2) };
			}
			if (troopList[3].Count == 0)
			{
				troopList[3] = new List<BasicCharacterObject> { NavalCustomBattleHelper.GetDefaultTroopOfFormationForFaction(basicCulture, 3) };
			}
			if (troopList[3].Count != 0)
			{
				if (!troopList[3].All<BasicCharacterObject>((BasicCharacterObject troop) => troop == null))
				{
					goto IL_012C;
				}
			}
			numbers[2] += numbers[3] / 3;
			numbers[1] += numbers[3] / 3;
			numbers[0] += numbers[3] / 3;
			numbers[0] += numbers[3] - numbers[3] / 3 * 3;
			numbers[3] = 0;
			IL_012C:
			for (int i = 0; i < 4; i++)
			{
				int count = troopList[i].Count;
				int num = numbers[i];
				if (num > 0)
				{
					float num2 = (float)num / (float)count;
					float num3 = 0f;
					for (int j = 0; j < count; j++)
					{
						float num4 = num2 + num3;
						int num5 = MathF.Floor(num4);
						num3 = num4 - (float)num5;
						customBattleParties.AddCharacter(troopList[i][j], num5);
						numbers[i] -= num5;
						if (j == count - 1 && numbers[i] > 0)
						{
							customBattleParties.AddCharacter(troopList[i][j], numbers[i]);
							numbers[i] = 0;
						}
					}
				}
			}
		}

		// Token: 0x060000BD RID: 189 RVA: 0x00005538 File Offset: 0x00003738
		public static int[] GetTroopCounts(int armySize, int heroCount, NavalCustomBattleCompositionData compositionData)
		{
			int[] array = new int[4];
			armySize -= heroCount;
			array[1] = MathF.Round(compositionData.RangedPercentage * (float)armySize);
			array[2] = MathF.Round(compositionData.CavalryPercentage * (float)armySize);
			array[3] = MathF.Round(compositionData.RangedCavalryPercentage * (float)armySize);
			array[0] = armySize - array.Sum();
			return array;
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00005590 File Offset: 0x00003790
		private static BasicCharacterObject GetTroopFromId(string troopId)
		{
			return MBObjectManager.Instance.GetObject<BasicCharacterObject>(troopId);
		}

		// Token: 0x060000BF RID: 191 RVA: 0x000055A0 File Offset: 0x000037A0
		public static BasicCharacterObject GetDefaultTroopOfFormationForFaction(BasicCultureObject culture, FormationClass formation)
		{
			if (culture.StringId.ToLower() == "empire")
			{
				switch (formation)
				{
				case 0:
					return NavalCustomBattleHelper.GetTroopFromId("imperial_veteran_infantryman");
				case 1:
					return NavalCustomBattleHelper.GetTroopFromId("imperial_archer");
				case 2:
					return NavalCustomBattleHelper.GetTroopFromId("imperial_heavy_horseman");
				case 3:
					return NavalCustomBattleHelper.GetTroopFromId("bucellarii");
				}
			}
			else if (culture.StringId.ToLower() == "sturgia")
			{
				switch (formation)
				{
				case 0:
					return NavalCustomBattleHelper.GetTroopFromId("sturgian_spearman");
				case 1:
					return NavalCustomBattleHelper.GetTroopFromId("sturgian_archer");
				case 2:
					return NavalCustomBattleHelper.GetTroopFromId("sturgian_hardened_brigand");
				}
			}
			else if (culture.StringId.ToLower() == "aserai")
			{
				switch (formation)
				{
				case 0:
					return NavalCustomBattleHelper.GetTroopFromId("aserai_infantry");
				case 1:
					return NavalCustomBattleHelper.GetTroopFromId("aserai_archer");
				case 2:
					return NavalCustomBattleHelper.GetTroopFromId("aserai_mameluke_cavalry");
				case 3:
					return NavalCustomBattleHelper.GetTroopFromId("aserai_faris");
				}
			}
			else if (culture.StringId.ToLower() == "vlandia")
			{
				switch (formation)
				{
				case 0:
					return NavalCustomBattleHelper.GetTroopFromId("vlandian_swordsman");
				case 1:
					return NavalCustomBattleHelper.GetTroopFromId("vlandian_hardened_crossbowman");
				case 2:
					return NavalCustomBattleHelper.GetTroopFromId("vlandian_knight");
				}
			}
			else if (culture.StringId.ToLower() == "battania")
			{
				switch (formation)
				{
				case 0:
					return NavalCustomBattleHelper.GetTroopFromId("battanian_picked_warrior");
				case 1:
					return NavalCustomBattleHelper.GetTroopFromId("battanian_hero");
				case 2:
					return NavalCustomBattleHelper.GetTroopFromId("battanian_scout");
				}
			}
			else if (culture.StringId.ToLower() == "khuzait")
			{
				switch (formation)
				{
				case 0:
					return NavalCustomBattleHelper.GetTroopFromId("khuzait_spear_infantry");
				case 1:
					return NavalCustomBattleHelper.GetTroopFromId("khuzait_archer");
				case 2:
					return NavalCustomBattleHelper.GetTroopFromId("khuzait_lancer");
				case 3:
					return NavalCustomBattleHelper.GetTroopFromId("khuzait_horse_archer");
				}
			}
			else if (culture.StringId.ToLower() == "nord")
			{
				if (formation == null)
				{
					return NavalCustomBattleHelper.GetTroopFromId("nord_spear_warrior");
				}
				if (formation == 1)
				{
					return NavalCustomBattleHelper.GetTroopFromId("nord_marksman");
				}
			}
			return null;
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x000057E8 File Offset: 0x000039E8
		public static bool CanShipHullBeUsedInRaid(ShipHull shipHull)
		{
			return shipHull.CanNavigateShallowWater;
		}

		// Token: 0x04000058 RID: 88
		public const string DefaultNavalBattleGameTypeStringId = "NavalBattle";

		// Token: 0x04000059 RID: 89
		public const string DefaultNavalRaidGameTypeStringId = "NavalRaid";

		// Token: 0x0400005A RID: 90
		private const string EmpireInfantryTroop = "imperial_veteran_infantryman";

		// Token: 0x0400005B RID: 91
		private const string EmpireRangedTroop = "imperial_archer";

		// Token: 0x0400005C RID: 92
		private const string EmpireCavalryTroop = "imperial_heavy_horseman";

		// Token: 0x0400005D RID: 93
		private const string EmpireHorseArcherTroop = "bucellarii";

		// Token: 0x0400005E RID: 94
		private const string SturgiaInfantryTroop = "sturgian_spearman";

		// Token: 0x0400005F RID: 95
		private const string SturgiaRangedTroop = "sturgian_archer";

		// Token: 0x04000060 RID: 96
		private const string SturgiaCavalryTroop = "sturgian_hardened_brigand";

		// Token: 0x04000061 RID: 97
		private const string AseraiInfantryTroop = "aserai_infantry";

		// Token: 0x04000062 RID: 98
		private const string AseraiRangedTroop = "aserai_archer";

		// Token: 0x04000063 RID: 99
		private const string AseraiCavalryTroop = "aserai_mameluke_cavalry";

		// Token: 0x04000064 RID: 100
		private const string AseraiHorseArcherTroop = "aserai_faris";

		// Token: 0x04000065 RID: 101
		private const string VlandiaInfantryTroop = "vlandian_swordsman";

		// Token: 0x04000066 RID: 102
		private const string VlandiaRangedTroop = "vlandian_hardened_crossbowman";

		// Token: 0x04000067 RID: 103
		private const string VlandiaCavalryTroop = "vlandian_knight";

		// Token: 0x04000068 RID: 104
		private const string BattaniaInfantryTroop = "battanian_picked_warrior";

		// Token: 0x04000069 RID: 105
		private const string BattaniaRangedTroop = "battanian_hero";

		// Token: 0x0400006A RID: 106
		private const string BattaniaCavalryTroop = "battanian_scout";

		// Token: 0x0400006B RID: 107
		private const string KhuzaitInfantryTroop = "khuzait_spear_infantry";

		// Token: 0x0400006C RID: 108
		private const string KhuzaitRangedTroop = "khuzait_archer";

		// Token: 0x0400006D RID: 109
		private const string KhuzaitCavalryTroop = "khuzait_lancer";

		// Token: 0x0400006E RID: 110
		private const string KhuzaitHorseArcherTroop = "khuzait_horse_archer";

		// Token: 0x0400006F RID: 111
		private const string NordInfantryTroop = "nord_spear_warrior";

		// Token: 0x04000070 RID: 112
		private const string NordRangedTroop = "nord_marksman";
	}
}
