using System;
using System.Collections.Generic;
using NavalDLC.Missions.AI.Behaviors;
using NavalDLC.Missions.AI.TeamAI;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.AI.Tactics
{
	// Token: 0x020000F0 RID: 240
	public abstract class NavalTacticComponent : TacticComponent
	{
		// Token: 0x0600123B RID: 4667 RVA: 0x0008459C File Offset: 0x0008279C
		public NavalTacticComponent(Team team)
			: base(team)
		{
			this.TeamAINavalComponent = team.TeamAI as TeamAINavalComponent;
			this._shipOrderCached = new MBReadOnlyList<Formation>();
		}

		// Token: 0x0600123C RID: 4668 RVA: 0x000845C1 File Offset: 0x000827C1
		public static void SetDefaultNavalBehaviorWeights(Formation f)
		{
			f.AI.SetBehaviorWeight<BehaviorNavalRemoveConnection>(1f);
		}

		// Token: 0x0600123D RID: 4669 RVA: 0x000845D4 File Offset: 0x000827D4
		protected void NavalApproach()
		{
			int num = this.TeamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.Count / 2;
			int num2 = num - 1;
			int count = this.TeamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.Count;
			int count2 = this.TeamAINavalComponent.TeamNavalQuerySystem.EnemyShipsWithFormationsInLeftToRightOrder.Count;
			Formation formation = this.TeamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder[num];
			formation.AI.ResetBehaviorWeights();
			NavalTacticComponent.SetDefaultNavalBehaviorWeights(formation);
			formation.AI.SetBehaviorWeight<BehaviorNavalApproachInLine>(1f).SetTargetShipSideAndOrder(true, num, true);
			NavalShipsLogic missionBehavior = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			for (int i = num + 1; i < this.TeamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.Count; i++)
			{
				Formation formation2 = this.TeamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder[i];
				formation2.AI.ResetBehaviorWeights();
				NavalTacticComponent.SetDefaultNavalBehaviorWeights(formation2);
				BehaviorNavalApproachInLine behaviorNavalApproachInLine = formation2.AI.SetBehaviorWeight<BehaviorNavalApproachInLine>(1f);
				MissionShip missionShip;
				missionBehavior.GetShip(formation.Team.TeamSide, formation.FormationIndex, out missionShip);
				behaviorNavalApproachInLine.SetTargetShipSideAndOrder(true, i, false);
				formation = formation2;
			}
			if (num2 >= 0 && num2 < this.TeamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.Count)
			{
				formation = this.TeamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder[num2];
				formation.AI.ResetBehaviorWeights();
				NavalTacticComponent.SetDefaultNavalBehaviorWeights(formation);
				formation.AI.SetBehaviorWeight<BehaviorNavalApproachInLine>(1f).SetTargetShipSideAndOrder(false, num2, false);
				for (int j = num2 - 1; j >= 0; j--)
				{
					Formation formation3 = this.TeamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder[j];
					formation3.AI.ResetBehaviorWeights();
					NavalTacticComponent.SetDefaultNavalBehaviorWeights(formation3);
					BehaviorNavalApproachInLine behaviorNavalApproachInLine2 = formation3.AI.SetBehaviorWeight<BehaviorNavalApproachInLine>(1f);
					MissionShip missionShip2;
					missionBehavior.GetShip(formation.Team.TeamSide, formation.FormationIndex, out missionShip2);
					behaviorNavalApproachInLine2.SetTargetShipSideAndOrder(false, j, false);
					formation = formation3;
				}
			}
			if (!this.TeamAINavalComponent.UseSpawnPathApproachPosition && base.Team.IsAttacker)
			{
				Vec2 globalWindVelocity = Mission.Current.Scene.GetGlobalWindVelocity();
				Vec2 vec = (this.TeamAINavalComponent.TeamNavalQuerySystem.AverageEnemyShipPosition - this.TeamAINavalComponent.TeamNavalQuerySystem.AverageShipPosition).Normalized();
				if (globalWindVelocity.Normalized().DotProduct(vec) > 0.5f)
				{
					foreach (MissionShip missionShip3 in this.TeamAINavalComponent.TeamNavalQuerySystem.TeamShipsWithFormationsInLeftToRightOrder)
					{
						missionShip3.ShipOrder.SetEnforcedSailUsage(1);
					}
				}
			}
		}

		// Token: 0x0600123E RID: 4670 RVA: 0x00084894 File Offset: 0x00082A94
		protected void CheckAndSetHasBattleBeenJoined()
		{
			if (this.TeamAINavalComponent.TeamNavalQuerySystem.ClosestDistanceSquaredToEnemyShip <= 40000f || base.Team.QuerySystem.DeathByRangedCount > 10 || (float)base.Team.QuerySystem.DeathByRangedCount > (float)base.Team.QuerySystem.AllyUnitCount * 0.1f)
			{
				this.HasBattleBeenJoined = true;
				return;
			}
			using (List<MissionShip>.Enumerator enumerator = this.TeamAINavalComponent.TeamNavalQuerySystem.TeamShipsWithFormationsInLeftToRightOrder.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.GetIsConnectedToEnemy())
					{
						this.HasBattleBeenJoined = true;
						break;
					}
				}
			}
		}

		// Token: 0x0600123F RID: 4671 RVA: 0x00084958 File Offset: 0x00082B58
		protected bool HasShipOrderChanged()
		{
			int num = 0;
			while (num < this._shipOrderCached.Count && num < this.TeamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.Count)
			{
				if (this._shipOrderCached[num] != this.TeamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder[num])
				{
					return true;
				}
				num++;
			}
			return false;
		}

		// Token: 0x06001240 RID: 4672 RVA: 0x000849BA File Offset: 0x00082BBA
		protected override void ManageFormationCounts()
		{
			base.ManageFormationCounts();
			this.TeamAINavalComponent.TeamNavalQuerySystem.ForceExpireSameSideShipLists();
		}

		// Token: 0x04000A38 RID: 2616
		private const float EngagementDistanceSquared = 40000f;

		// Token: 0x04000A39 RID: 2617
		protected readonly TeamAINavalComponent TeamAINavalComponent;

		// Token: 0x04000A3A RID: 2618
		protected bool HasBattleBeenJoined;

		// Token: 0x04000A3B RID: 2619
		protected MBReadOnlyList<Formation> _shipOrderCached;
	}
}
