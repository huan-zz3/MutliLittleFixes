using System;
using System.Collections.Generic;
using NavalDLC.Missions.MissionLogics;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.CustomBattle.CustomBattle
{
	// Token: 0x0200000E RID: 14
	public struct NavalCustomBattleData
	{
		// Token: 0x1700003E RID: 62
		// (get) Token: 0x06000092 RID: 146 RVA: 0x00004966 File Offset: 0x00002B66
		public static IEnumerable<Tuple<string, string>> GameTypes
		{
			get
			{
				yield return new Tuple<string, string>(new TextObject("{=lr2UaD9m}Naval Battle", null).ToString(), "NavalBattle");
				yield return new Tuple<string, string>(new TextObject("{=3oDQHZrf}Naval Raid", null).ToString(), "NavalRaid");
				yield break;
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000093 RID: 147 RVA: 0x0000496F File Offset: 0x00002B6F
		public static IEnumerable<Tuple<string, NavalCustomBattlePlayerSide>> PlayerSides
		{
			get
			{
				yield return new Tuple<string, NavalCustomBattlePlayerSide>(new TextObject("{=KASD0tnO}Attacker", null).ToString(), NavalCustomBattlePlayerSide.Attacker);
				yield return new Tuple<string, NavalCustomBattlePlayerSide>(new TextObject("{=XEVFUaFj}Defender", null).ToString(), NavalCustomBattlePlayerSide.Defender);
				yield break;
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000094 RID: 148 RVA: 0x00004978 File Offset: 0x00002B78
		public static IEnumerable<BasicCharacterObject> Characters
		{
			get
			{
				yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_1");
				yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_2");
				yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_3");
				yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_4");
				yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_5");
				yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_6");
				yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_7");
				yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_8");
				yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_9");
				yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_10");
				yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_11");
				yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_12");
				yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_13");
				yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_14");
				yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_15");
				yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_16");
				yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_17");
				yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_18");
				yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_19");
				yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_20");
				yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_21");
				yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_22");
				yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_23");
				yield return Game.Current.ObjectManager.GetObject<BasicCharacterObject>("commander_24");
				yield break;
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000095 RID: 149 RVA: 0x00004981 File Offset: 0x00002B81
		public static IEnumerable<BasicCultureObject> Factions
		{
			get
			{
				yield return Game.Current.ObjectManager.GetObject<BasicCultureObject>("empire");
				yield return Game.Current.ObjectManager.GetObject<BasicCultureObject>("sturgia");
				yield return Game.Current.ObjectManager.GetObject<BasicCultureObject>("aserai");
				yield return Game.Current.ObjectManager.GetObject<BasicCultureObject>("vlandia");
				yield return Game.Current.ObjectManager.GetObject<BasicCultureObject>("battania");
				yield return Game.Current.ObjectManager.GetObject<BasicCultureObject>("khuzait");
				yield return Game.Current.ObjectManager.GetObject<BasicCultureObject>("nord");
				yield break;
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000096 RID: 150 RVA: 0x0000498A File Offset: 0x00002B8A
		public static IEnumerable<ShipHull> ShipHulls
		{
			get
			{
				yield return Game.Current.ObjectManager.GetObject<ShipHull>("northern_light_ship");
				yield return Game.Current.ObjectManager.GetObject<ShipHull>("northern_medium_ship");
				yield return Game.Current.ObjectManager.GetObject<ShipHull>("nord_medium_ship");
				yield return Game.Current.ObjectManager.GetObject<ShipHull>("sturgia_heavy_ship");
				yield return Game.Current.ObjectManager.GetObject<ShipHull>("western_light_ship");
				yield return Game.Current.ObjectManager.GetObject<ShipHull>("central_light_ship");
				yield return Game.Current.ObjectManager.GetObject<ShipHull>("empire_medium_ship");
				yield return Game.Current.ObjectManager.GetObject<ShipHull>("eastern_medium_ship");
				yield return Game.Current.ObjectManager.GetObject<ShipHull>("empire_heavy_ship");
				yield return Game.Current.ObjectManager.GetObject<ShipHull>("aserai_heavy_ship");
				yield return Game.Current.ObjectManager.GetObject<ShipHull>("khuzait_heavy_ship");
				yield return Game.Current.ObjectManager.GetObject<ShipHull>("eastern_heavy_ship");
				yield return Game.Current.ObjectManager.GetObject<ShipHull>("battanian_light_ship");
				yield return Game.Current.ObjectManager.GetObject<ShipHull>("western_medium_ship");
				yield return Game.Current.ObjectManager.GetObject<ShipHull>("vlandia_heavy_ship");
				yield return Game.Current.ObjectManager.GetObject<ShipHull>("northern_trade_ship");
				yield return Game.Current.ObjectManager.GetObject<ShipHull>("eastern_trade_ship");
				yield return Game.Current.ObjectManager.GetObject<ShipHull>("empire_trade_ship");
				yield return Game.Current.ObjectManager.GetObject<ShipHull>("nord_mediumballista_ship");
				yield return Game.Current.ObjectManager.GetObject<ShipHull>("battanian_medium_ship");
				yield return Game.Current.ObjectManager.GetObject<ShipHull>("western_trade_ship");
				yield break;
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000097 RID: 151 RVA: 0x00004993 File Offset: 0x00002B93
		public static IEnumerable<Tuple<string, NavalCustomBattleTimeOfDay>> TimesOfDay
		{
			get
			{
				yield return new Tuple<string, NavalCustomBattleTimeOfDay>(new TextObject("{=X3gcUz7C}Morning", null).ToString(), NavalCustomBattleTimeOfDay.Morning);
				yield return new Tuple<string, NavalCustomBattleTimeOfDay>(new TextObject("{=CTtjSwRb}Noon", null).ToString(), NavalCustomBattleTimeOfDay.Noon);
				yield return new Tuple<string, NavalCustomBattleTimeOfDay>(new TextObject("{=J2gvnexb}Afternoon", null).ToString(), NavalCustomBattleTimeOfDay.Afternoon);
				yield return new Tuple<string, NavalCustomBattleTimeOfDay>(new TextObject("{=gENb9SSW}Evening", null).ToString(), NavalCustomBattleTimeOfDay.Evening);
				yield return new Tuple<string, NavalCustomBattleTimeOfDay>(new TextObject("{=fAxjyMt5}Night", null).ToString(), NavalCustomBattleTimeOfDay.Night);
				yield break;
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000098 RID: 152 RVA: 0x0000499C File Offset: 0x00002B9C
		public static IEnumerable<Tuple<string, string>> Seasons
		{
			get
			{
				yield return new Tuple<string, string>(new TextObject("{=f7vOVQb7}Summer", null).ToString(), "summer");
				yield return new Tuple<string, string>(new TextObject("{=cZzfNlxd}Fall", null).ToString(), "fall");
				yield return new Tuple<string, string>(new TextObject("{=nwqUFaU8}Winter", null).ToString(), "winter");
				yield return new Tuple<string, string>(new TextObject("{=nWbp3o3H}Spring", null).ToString(), "spring");
				yield break;
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000099 RID: 153 RVA: 0x000049A5 File Offset: 0x00002BA5
		public static IEnumerable<Tuple<string, float>> WindStrengths
		{
			get
			{
				yield return new Tuple<string, float>(new TextObject("{=windstrengthweak}Weak", null).ToString(), 0.4f);
				yield return new Tuple<string, float>(new TextObject("{=windstrengthmild}Mild", null).ToString(), 0.5f);
				yield return new Tuple<string, float>(new TextObject("{=windstrengthstrong}Strong", null).ToString(), 0.7f);
				yield return new Tuple<string, float>(new TextObject("{=windstrengthstormy}Stormy", null).ToString(), 1f);
				yield break;
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x0600009A RID: 154 RVA: 0x000049AE File Offset: 0x00002BAE
		public static IEnumerable<Tuple<string, NavalCustomBattleWindConfig.Direction>> WindDirections
		{
			get
			{
				yield return new Tuple<string, NavalCustomBattleWindConfig.Direction>(new TextObject("{=vz4kmcdI}Towards the Defender", null).ToString(), NavalCustomBattleWindConfig.Direction.TowardsDefender);
				yield return new Tuple<string, NavalCustomBattleWindConfig.Direction>(new TextObject("{=OjOsvTkT}Towards the Side", null).ToString(), NavalCustomBattleWindConfig.Direction.Side);
				yield return new Tuple<string, NavalCustomBattleWindConfig.Direction>(new TextObject("{=M0Fiya6u}Towards the Attacker", null).ToString(), NavalCustomBattleWindConfig.Direction.TowardsAttacker);
				yield return new Tuple<string, NavalCustomBattleWindConfig.Direction>(new TextObject("{=vBkrw5VV}Random", null).ToString(), NavalCustomBattleWindConfig.Direction.Random);
				yield break;
			}
		}

		// Token: 0x0400003B RID: 59
		public string GameTypeStringId;

		// Token: 0x0400003C RID: 60
		public string SceneId;

		// Token: 0x0400003D RID: 61
		public string SeasonId;

		// Token: 0x0400003E RID: 62
		public BasicCharacterObject PlayerCharacter;

		// Token: 0x0400003F RID: 63
		public CustomBattleCombatant PlayerParty;

		// Token: 0x04000040 RID: 64
		public CustomBattleCombatant EnemyParty;

		// Token: 0x04000041 RID: 65
		public List<IShipOrigin> PlayerShips;

		// Token: 0x04000042 RID: 66
		public List<IShipOrigin> EnemyShips;

		// Token: 0x04000043 RID: 67
		public float TimeOfDay;

		// Token: 0x04000044 RID: 68
		public float WindStrength;

		// Token: 0x04000045 RID: 69
		public NavalCustomBattleWindConfig.Direction WindDirection;

		// Token: 0x04000046 RID: 70
		public TerrainType Terrain;

		// Token: 0x04000047 RID: 71
		public string ForcedSceneLevel;
	}
}
