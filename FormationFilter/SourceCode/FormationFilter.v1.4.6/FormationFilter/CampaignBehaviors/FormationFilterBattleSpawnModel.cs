using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FormationFilter.Config;
using FormationFilter.Models;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace FormationFilter.CampaignBehaviors
{
	// Token: 0x02000021 RID: 33
	[NullableContext(1)]
	[Nullable(0)]
	public class FormationFilterBattleSpawnModel : BattleSpawnModel
	{
		// Token: 0x06000139 RID: 313 RVA: 0x00009121 File Offset: 0x00007321
		public override void OnMissionStart()
		{
			base.OnMissionStart();
			BattleSpawnModel baseModel = base.BaseModel;
			if (baseModel == null)
			{
				return;
			}
			baseModel.OnMissionStart();
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00009139 File Offset: 0x00007339
		public override void OnMissionEnd()
		{
			base.OnMissionEnd();
			BattleSpawnModel baseModel = base.BaseModel;
			if (baseModel == null)
			{
				return;
			}
			baseModel.OnMissionEnd();
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00009151 File Offset: 0x00007351
		[return: TupleElementNames(new string[] { "origin", "formationIndex" })]
		[return: Nullable(new byte[] { 1, 0, 1 })]
		public override List<ValueTuple<IAgentOriginBase, int>> GetInitialSpawnAssignments(BattleSideEnum battleSide, List<IAgentOriginBase> troopOrigins)
		{
			BattleSpawnModel baseModel = base.BaseModel;
			return ((baseModel != null) ? baseModel.GetInitialSpawnAssignments(battleSide, troopOrigins) : null) ?? new List<ValueTuple<IAgentOriginBase, int>>();
		}

		// Token: 0x0600013C RID: 316 RVA: 0x00009170 File Offset: 0x00007370
		[return: TupleElementNames(new string[] { "origin", "formationIndex" })]
		[return: Nullable(new byte[] { 1, 0, 1 })]
		public override List<ValueTuple<IAgentOriginBase, int>> GetReinforcementAssignments(BattleSideEnum battleSide, List<IAgentOriginBase> troopOrigins)
		{
			if (!Mission.Current.IsNavalBattle && !Mission.Current.IsNavalRaidBattle)
			{
				FormationFilterSettings instance = GlobalSettings<FormationFilterSettings>.Instance;
				if (instance != null && instance.AssignReinforcementAccordingToFormationFilter)
				{
					List<IAgentOriginBase> list;
					List<ValueTuple<IAgentOriginBase, int>> reinforcementAssignments = TeamFilter.GetReinforcementAssignments(battleSide, troopOrigins, out list);
					List<ValueTuple<IAgentOriginBase, int>> list2 = base.BaseModel.GetReinforcementAssignments(battleSide, list) ?? new List<ValueTuple<IAgentOriginBase, int>>();
					reinforcementAssignments.AddRange(list2);
					return reinforcementAssignments;
				}
			}
			BattleSpawnModel baseModel = base.BaseModel;
			return ((baseModel != null) ? baseModel.GetReinforcementAssignments(battleSide, troopOrigins) : null) ?? new List<ValueTuple<IAgentOriginBase, int>>();
		}
	}
}
