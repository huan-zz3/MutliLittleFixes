using System;
using NavalDLC.Missions.AI.Behaviors;
using NavalDLC.Missions.AI.TeamAI;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.AI.Tactics
{
	// Token: 0x020000F1 RID: 241
	public class TacticNavalBalancedOffense : NavalTacticComponent
	{
		// Token: 0x06001241 RID: 4673 RVA: 0x000849D2 File Offset: 0x00082BD2
		public TacticNavalBalancedOffense(Team team)
			: base(team)
		{
			this._teamAINavalComponent = team.TeamAI as TeamAINavalComponent;
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
		}

		// Token: 0x06001242 RID: 4674 RVA: 0x000849FC File Offset: 0x00082BFC
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

		// Token: 0x06001243 RID: 4675 RVA: 0x00084A34 File Offset: 0x00082C34
		private void NavalEngage()
		{
			int num = this._teamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.Count / 2;
			int num2 = num - 1;
			bool flag = this._teamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.Count > this._teamAINavalComponent.TeamNavalQuerySystem.EnemyShipsWithFormationsInLeftToRightOrder.Count;
			Formation formation = this._teamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder[num];
			formation.AI.ResetBehaviorWeights();
			NavalTacticComponent.SetDefaultNavalBehaviorWeights(formation);
			formation.AI.SetBehaviorWeight<BehaviorNavalEngageCorrespondingEnemy>(1f).SetTargetShipSideAndOrder(!flag, num);
			formation.AI.SetBehaviorWeight<BehaviorNavalSkirmish>(1f);
			formation.AI.SetBehaviorWeight<BehaviorNavalRamming>(1f);
			for (int i = num + 1; i < this._teamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.Count; i++)
			{
				Formation formation2 = this._teamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder[i];
				formation2.AI.ResetBehaviorWeights();
				NavalTacticComponent.SetDefaultNavalBehaviorWeights(formation2);
				formation2.AI.SetBehaviorWeight<BehaviorNavalEngageCorrespondingEnemy>(1f).SetTargetShipSideAndOrder(!flag, i);
				formation2.AI.SetBehaviorWeight<BehaviorNavalSkirmish>(1f);
				formation2.AI.SetBehaviorWeight<BehaviorNavalRamming>(1f);
			}
			if (num2 >= 0 && num2 < this._teamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder.Count)
			{
				Formation formation3 = this._teamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder[num2];
				formation3.AI.ResetBehaviorWeights();
				NavalTacticComponent.SetDefaultNavalBehaviorWeights(formation3);
				formation3.AI.SetBehaviorWeight<BehaviorNavalEngageCorrespondingEnemy>(1f).SetTargetShipSideAndOrder(flag, num2);
				formation3.AI.SetBehaviorWeight<BehaviorNavalSkirmish>(1f);
				formation3.AI.SetBehaviorWeight<BehaviorNavalRamming>(1f);
				for (int j = num2 - 1; j >= 0; j--)
				{
					Formation formation4 = this._teamAINavalComponent.TeamNavalQuerySystem.FormationsInShipsInLeftToRightOrder[j];
					formation4.AI.ResetBehaviorWeights();
					NavalTacticComponent.SetDefaultNavalBehaviorWeights(formation4);
					formation4.AI.SetBehaviorWeight<BehaviorNavalEngageCorrespondingEnemy>(1f).SetTargetShipSideAndOrder(flag, j);
					formation4.AI.SetBehaviorWeight<BehaviorNavalSkirmish>(1f);
					formation4.AI.SetBehaviorWeight<BehaviorNavalRamming>(1f);
				}
			}
			foreach (MissionShip missionShip in this.TeamAINavalComponent.TeamNavalQuerySystem.TeamShipsWithFormationsInLeftToRightOrder)
			{
				missionShip.ShipOrder.SetEnforcedSailUsage(0);
			}
		}

		// Token: 0x06001244 RID: 4676 RVA: 0x00084CBC File Offset: 0x00082EBC
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
							this.NavalEngage();
						}
						else if (!this._teamAINavalComponent.UseSpawnPathApproachPosition || flag || this.IsTacticReapplyNeeded)
						{
							base.NavalApproach();
						}
					}
					this.IsTacticReapplyNeeded = false;
				}
			}
			base.TickOccasionally();
		}

		// Token: 0x06001245 RID: 4677 RVA: 0x00084DC8 File Offset: 0x00082FC8
		protected override float GetTacticWeight()
		{
			return MathF.Max(base.Team.QuerySystem.TotalPowerRatio, 0.1f);
		}

		// Token: 0x04000A3C RID: 2620
		private readonly TeamAINavalComponent _teamAINavalComponent;

		// Token: 0x04000A3D RID: 2621
		private readonly NavalShipsLogic _navalShipsLogic;
	}
}
