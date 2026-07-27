using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace NavalDLC.CustomBattle
{
	// Token: 0x02000005 RID: 5
	public struct NavalCustomBattleSceneData
	{
		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600002F RID: 47 RVA: 0x00002F8E File Offset: 0x0000118E
		// (set) Token: 0x06000030 RID: 48 RVA: 0x00002F96 File Offset: 0x00001196
		public string SceneID { get; private set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000031 RID: 49 RVA: 0x00002F9F File Offset: 0x0000119F
		// (set) Token: 0x06000032 RID: 50 RVA: 0x00002FA7 File Offset: 0x000011A7
		public TextObject Name { get; private set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000033 RID: 51 RVA: 0x00002FB0 File Offset: 0x000011B0
		// (set) Token: 0x06000034 RID: 52 RVA: 0x00002FB8 File Offset: 0x000011B8
		public TerrainType Terrain { get; private set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000035 RID: 53 RVA: 0x00002FC1 File Offset: 0x000011C1
		// (set) Token: 0x06000036 RID: 54 RVA: 0x00002FC9 File Offset: 0x000011C9
		public string ForcedSceneLevel { get; private set; }

		// Token: 0x06000037 RID: 55 RVA: 0x00002FD2 File Offset: 0x000011D2
		public NavalCustomBattleSceneData(string sceneID, TextObject name, TerrainType terrain, string forcedSceneLevel)
		{
			this.SceneID = sceneID;
			this.Name = name;
			this.Terrain = terrain;
			this.ForcedSceneLevel = forcedSceneLevel;
		}
	}
}
