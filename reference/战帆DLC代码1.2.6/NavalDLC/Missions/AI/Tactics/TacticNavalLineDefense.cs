using System;
using NavalDLC.Missions.AI.Behaviors;
using NavalDLC.Missions.AI.TeamAI;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.AI.Tactics
{
	// Token: 0x020000F2 RID: 242
	public class TacticNavalLineDefense : NavalTacticComponent
	{
		// Token: 0x06001246 RID: 4678 RVA: 0x00084DE4 File Offset: 0x00082FE4
		public TacticNavalLineDefense(Team team)
			: base(team)
		{
			this._teamAINavalComponent = team.TeamAI as TeamAINavalComponent;
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
		}

		// Token: 0x06001247 RID: 4679 RVA: 0x00084E10 File Offset: 0x00083010
		protected override bool CheckAndSetAvailableFormationsChanged()
		{
			int aicontrolledFormationCount = base.Team.GetAIControlledFormationCount();
			bool flag = aicontrolledFormationCount != this._AIControlledFormationCount;
			if (flag)
			{
				this._AIControlledFormationCount = aicontrolledFormationCount;
				this.IsTacticReapplyNeeded = true;
			}
			return flag;
		}

		// Token: 0x06001248 RID: 4680 RVA: 0x00084E48 File Offset: 0x00083048
		private void NavalDefensiveEngage()
		{
			int num = this._teamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.Count / 2;
			int num2 = num - 1;
			bool flag = this._teamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.Count > this._teamAINavalComponent.TeamNavalQuerySystem.EnemyShipsWithFormationsInLeftToRightOrder.Count;
			Formation formation = this._teamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder[num];
			formation.AI.ResetBehaviorWeights();
			NavalTacticComponent.SetDefaultNavalBehaviorWeights(formation);
			formation.AI.SetBehaviorWeight<BehaviorNavalEngageCorrespondingEnemy>(1f).SetTargetShipSideAndOrder(!flag, num);
			formation.AI.SetBehaviorWeight<BehaviorNavalDefendInLine>(1f).SetTargetShipSideAndOrder(true, num, true);
			formation.AI.SetBehaviorWeight<BehaviorNavalSkirmish>(1f);
			formation.AI.SetBehaviorWeight<BehaviorNavalRamming>(1f);
			for (int i = num + 1; i < this._teamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.Count; i++)
			{
				Formation formation2 = this._teamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder[i];
				formation2.AI.ResetBehaviorWeights();
				NavalTacticComponent.SetDefaultNavalBehaviorWeights(formation2);
				formation2.AI.SetBehaviorWeight<BehaviorNavalEngageCorrespondingEnemy>(1f).SetTargetShipSideAndOrder(!flag, i);
				BehaviorNavalDefendInLine behaviorNavalDefendInLine = formation2.AI.SetBehaviorWeight<BehaviorNavalDefendInLine>(1f);
				MissionShip missionShip;
				this._navalShipsLogic.GetShip(formation.Team.TeamSide, formation.FormationIndex, out missionShip);
				behaviorNavalDefendInLine.SetTargetShipSideAndOrder(true, i, false);
				formation2.AI.SetBehaviorWeight<BehaviorNavalSkirmish>(1f);
				formation2.AI.SetBehaviorWeight<BehaviorNavalRamming>(1f);
				formation = formation2;
			}
			if (num2 >= 0 && num2 < this._teamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.Count)
			{
				formation = this._teamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder[num2];
				formation.AI.ResetBehaviorWeights();
				NavalTacticComponent.SetDefaultNavalBehaviorWeights(formation);
				formation.AI.SetBehaviorWeight<BehaviorNavalEngageCorrespondingEnemy>(1f).SetTargetShipSideAndOrder(flag, num2);
				formation.AI.SetBehaviorWeight<BehaviorNavalDefendInLine>(1f).SetTargetShipSideAndOrder(false, num2, false);
				formation.AI.SetBehaviorWeight<BehaviorNavalSkirmish>(1f);
				formation.AI.SetBehaviorWeight<BehaviorNavalRamming>(1f);
				for (int j = num2 - 1; j >= 0; j--)
				{
					Formation formation3 = this._teamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder[j];
					formation3.AI.ResetBehaviorWeights();
					NavalTacticComponent.SetDefaultNavalBehaviorWeights(formation3);
					formation3.AI.SetBehaviorWeight<BehaviorNavalEngageCorrespondingEnemy>(1f).SetTargetShipSideAndOrder(flag, j);
					BehaviorNavalDefendInLine behaviorNavalDefendInLine2 = formation3.AI.SetBehaviorWeight<BehaviorNavalDefendInLine>(1f);
					MissionShip missionShip2;
					this._navalShipsLogic.GetShip(formation.Team.TeamSide, formation.FormationIndex, out missionShip2);
					behaviorNavalDefendInLine2.SetTargetShipSideAndOrder(false, j, false);
					formation3.AI.SetBehaviorWeight<BehaviorNavalSkirmish>(1f);
					formation3.AI.SetBehaviorWeight<BehaviorNavalRamming>(1f);
					formation = formation3;
				}
			}
		}

		// Token: 0x06001249 RID: 4681 RVA: 0x00085134 File Offset: 0x00083334
		private void NavalDefensivePositioning()
		{
			int num = this.TeamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.Count / 2;
			int num2 = num - 1;
			int count = this.TeamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.Count;
			int count2 = this.TeamAINavalComponent.TeamNavalQuerySystem.EnemyShipsWithFormationsInLeftToRightOrder.Count;
			Formation formation = this.TeamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder[num];
			formation.AI.ResetBehaviorWeights();
			NavalTacticComponent.SetDefaultNavalBehaviorWeights(formation);
			formation.AI.SetBehaviorWeight<BehaviorNavalDefendInLine>(1f).SetTargetShipSideAndOrder(true, num, true);
			NavalShipsLogic missionBehavior = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			for (int i = num + 1; i < this.TeamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.Count; i++)
			{
				Formation formation2 = this.TeamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder[i];
				formation2.AI.ResetBehaviorWeights();
				NavalTacticComponent.SetDefaultNavalBehaviorWeights(formation2);
				BehaviorNavalDefendInLine behaviorNavalDefendInLine = formation2.AI.SetBehaviorWeight<BehaviorNavalDefendInLine>(1f);
				MissionShip missionShip;
				missionBehavior.GetShip(formation.Team.TeamSide, formation.FormationIndex, out missionShip);
				behaviorNavalDefendInLine.SetTargetShipSideAndOrder(true, i, false);
				formation = formation2;
			}
			if (num2 >= 0 && num2 < this.TeamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.Count)
			{
				formation = this.TeamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder[num2];
				formation.AI.ResetBehaviorWeights();
				NavalTacticComponent.SetDefaultNavalBehaviorWeights(formation);
				formation.AI.SetBehaviorWeight<BehaviorNavalDefendInLine>(1f).SetTargetShipSideAndOrder(false, num2, false);
				for (int j = num2 - 1; j >= 0; j--)
				{
					Formation formation3 = this.TeamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder[j];
					formation3.AI.ResetBehaviorWeights();
					NavalTacticComponent.SetDefaultNavalBehaviorWeights(formation3);
					BehaviorNavalDefendInLine behaviorNavalDefendInLine2 = formation3.AI.SetBehaviorWeight<BehaviorNavalDefendInLine>(1f);
					MissionShip missionShip2;
					missionBehavior.GetShip(formation.Team.TeamSide, formation.FormationIndex, out missionShip2);
					behaviorNavalDefendInLine2.SetTargetShipSideAndOrder(false, j, false);
					formation = formation3;
				}
			}
		}

		// Token: 0x0600124A RID: 4682 RVA: 0x00085328 File Offset: 0x00083528
		public override void TickOccasionally()
		{
			if (base.AreFormationsCreated && this._teamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.Count > 0 && this._teamAINavalComponent.TeamNavalQuerySystem.EnemyShipsWithFormationsInLeftToRightOrder.Count > 0)
			{
				bool flag = this.CheckAndSetAvailableFormationsChanged();
				bool flag2 = flag || base.HasShipOrderChanged();
				if (!this.HasBattleBeenJoined)
				{
					base.CheckAndSetHasBattleBeenJoined();
					this.IsTacticReapplyNeeded |= this.HasBattleBeenJoined;
				}
				if (flag || flag2 || this.IsTacticReapplyNeeded)
				{
					if (flag)
					{
						this.ManageFormationCounts();
					}
					if (flag2)
					{
						this._shipOrderCached = Extensions.ToMBList<Formation>(this._teamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder);
					}
					if (this._teamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.Count > 0)
					{
						if (this.HasBattleBeenJoined)
						{
							this.NavalDefensiveEngage();
						}
						else if (!this._teamAINavalComponent.UseSpawnPathApproachPosition || flag || this.IsTacticReapplyNeeded)
						{
							this.NavalDefensivePositioning();
						}
					}
					this.IsTacticReapplyNeeded = false;
				}
			}
			base.TickOccasionally();
		}

		// Token: 0x0600124B RID: 4683 RVA: 0x00085434 File Offset: 0x00083634
		protected override float GetTacticWeight()
		{
			if (base.Team.TeamAI.IsDefenseApplicable)
			{
				return 1.5f;
			}
			return 0f;
		}

		// Token: 0x04000A3E RID: 2622
		private readonly TeamAINavalComponent _teamAINavalComponent;

		// Token: 0x04000A3F RID: 2623
		private readonly NavalShipsLogic _navalShipsLogic;
	}
}
