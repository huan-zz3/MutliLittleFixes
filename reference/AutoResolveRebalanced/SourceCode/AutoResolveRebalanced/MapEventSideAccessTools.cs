using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;

namespace AutoResolveRebalanced
{
	// Token: 0x02000007 RID: 7
	internal static class MapEventSideAccessTools
	{
		// Token: 0x0600000C RID: 12 RVA: 0x0000298C File Offset: 0x00000B8C
		internal static IBattleObserver GetBattleObserver(MapEventSide mapEventSide)
		{
			return (IBattleObserver)MapEventSideAccessTools._battleObserverPropertyGetter.GetValue(mapEventSide);
		}

		// Token: 0x0600000D RID: 13 RVA: 0x0000299E File Offset: 0x00000B9E
		internal static void InvokeRemoveSelectedTroopFromSimulationList(MapEventSide mapEventSide)
		{
			MapEventSideAccessTools._RemoveSelectedTroopFromSimulationListGetter.Invoke(mapEventSide, null);
		}

		// Token: 0x04000001 RID: 1
		private static readonly PropertyInfo _battleObserverPropertyGetter = AccessTools.Property(typeof(MapEventSide), "BattleObserver");

		// Token: 0x04000002 RID: 2
		private static readonly MethodInfo _RemoveSelectedTroopFromSimulationListGetter = AccessTools.Method(typeof(MapEventSide), "RemoveSelectedTroopFromSimulationList", null, null);

		// Token: 0x04000003 RID: 3
		public static readonly AccessTools.FieldRef<MapEventSide, Dictionary<UniqueTroopDescriptor, MapEventParty>> _allocatedTroops = AccessTools.FieldRefAccess<MapEventSide, Dictionary<UniqueTroopDescriptor, MapEventParty>>("_allocatedTroops");

		// Token: 0x04000004 RID: 4
		public static readonly AccessTools.FieldRef<MapEventSide, CharacterObject> _selectedSimulationTroop = AccessTools.FieldRefAccess<MapEventSide, CharacterObject>("_selectedSimulationTroop");
	}
}
