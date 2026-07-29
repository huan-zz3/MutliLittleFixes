using System;
using RTSCamera.CommandSystem.AgentComponents;
using RTSCamera.CommandSystem.Patch;
using RTSCamera.CommandSystem.Utilities;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace RTSCamera.CommandSystem.QuerySystem
{
	// Token: 0x02000056 RID: 86
	public class CommandFormationQuerySystem
	{
		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000303 RID: 771 RVA: 0x0000D406 File Offset: 0x0000B606
		public Formation ClosestEnemyFormation
		{
			get
			{
				if (this._closestEnemyFormation.Value == null || this._closestEnemyFormation.Value.CountOfUnits == 0)
				{
					this._closestEnemyFormation.Expire();
				}
				return this._closestEnemyFormation.Value;
			}
		}

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000304 RID: 772 RVA: 0x0000D43D File Offset: 0x0000B63D
		public Agent ClosestEnemyAgent
		{
			get
			{
				return this._closestEnemyAgent.Value;
			}
		}

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000305 RID: 773 RVA: 0x0000D44A File Offset: 0x0000B64A
		public Vec2 VirtualWeightedAverageEnemyPosition
		{
			get
			{
				return this._virtualWeightedAverageEnemyPosition.Value;
			}
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x06000306 RID: 774 RVA: 0x0000D457 File Offset: 0x0000B657
		public Vec2 WeightedAverageFacingTargetEnemyPosition
		{
			get
			{
				return this._weightedAverageFacingTargetEnemyPosition.Value;
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x06000307 RID: 775 RVA: 0x0000D464 File Offset: 0x0000B664
		public Vec2 VirtualWeightedAverageFacingTargetEnemyPosition
		{
			get
			{
				return this._virtualWeightedAverageFacingTargetEnemyPosition.Value;
			}
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000308 RID: 776 RVA: 0x0000D471 File Offset: 0x0000B671
		public bool AreAgentsNearTargetPositions
		{
			get
			{
				return this._areAgentsNearTargetPositions.Value;
			}
		}

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x06000309 RID: 777 RVA: 0x0000D47E File Offset: 0x0000B67E
		public bool CoolDownToEvaluateAgentsDistanceToTarget
		{
			get
			{
				return this._coolDownToEvaluateAgentsDistanceToTarget.Value;
			}
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x0600030A RID: 778 RVA: 0x0000D48B File Offset: 0x0000B68B
		public float AverageMissileRangeAdjusted
		{
			get
			{
				return this._averageMissileRangeAdjusted.Value;
			}
		}

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x0600030B RID: 779 RVA: 0x0000D498 File Offset: 0x0000B698
		public float RatioOfAgentsHavingAmmo
		{
			get
			{
				return this._ratioOfAgentsHavingAmmo.Value;
			}
		}

		// Token: 0x17000095 RID: 149
		// (get) Token: 0x0600030C RID: 780 RVA: 0x0000D4A5 File Offset: 0x0000B6A5
		public float RatioOfRemainingAmmo
		{
			get
			{
				return this._ratioOfRemainingAmmoQuery.Value;
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x0600030D RID: 781 RVA: 0x0000D4B2 File Offset: 0x0000B6B2
		public bool HasCurrentMovementOrderCompleted
		{
			get
			{
				if (!this.NeedToUpdateTargetPositionDistance)
				{
					return true;
				}
				if (this.CoolDownToEvaluateAgentsDistanceToTarget)
				{
					return false;
				}
				if (this.AreAgentsNearTargetPositions)
				{
					this.NeedToUpdateTargetPositionDistance = false;
					return true;
				}
				return false;
			}
		}

		// Token: 0x0600030E RID: 782 RVA: 0x0000D4DC File Offset: 0x0000B6DC
		public CommandFormationQuerySystem(Formation formation)
		{
			CommandFormationQuerySystem.<>c__DisplayClass35_0 CS$<>8__locals1 = new CommandFormationQuerySystem.<>c__DisplayClass35_0();
			CS$<>8__locals1.formation = formation;
			base..ctor();
			CS$<>8__locals1.<>4__this = this;
			this.Formation = CS$<>8__locals1.formation;
			Mission mission = Mission.Current;
			this._closestEnemyFormation = new QueryData<Formation>(delegate
			{
				float num = float.MaxValue;
				Formation formation2 = null;
				foreach (Team team in mission.Teams)
				{
					if (team.IsEnemyOf(CS$<>8__locals1.formation.Team))
					{
						foreach (Formation formation3 in team.FormationsIncludingSpecialAndEmpty)
						{
							if (formation3.CountOfUnits > 0)
							{
								WorldPosition? worldPosition;
								Patch_OrderController.GetFormationMovingTargetForPreview(CS$<>8__locals1.formation, out worldPosition, null);
								float num2 = formation3.CachedMedianPosition.GetNavMeshVec3().DistanceSquared((worldPosition ?? CS$<>8__locals1.formation.CachedMedianPosition).GetNavMeshVec3());
								if (num2 < num)
								{
									num = num2;
									formation2 = formation3;
								}
							}
						}
					}
				}
				return formation2;
			}, 1.5f);
			this._closestEnemyAgent = new QueryData<Agent>(delegate
			{
				float num3 = float.MaxValue;
				Agent agent = null;
				foreach (Team team2 in mission.Teams)
				{
					if (team2.IsEnemyOf(CS$<>8__locals1.formation.Team))
					{
						foreach (Agent agent2 in team2.ActiveAgents)
						{
							WorldPosition? worldPosition2;
							Patch_OrderController.GetFormationMovingTargetForPreview(CS$<>8__locals1.formation, out worldPosition2, null);
							float num4 = agent2.Position.DistanceSquared((worldPosition2 ?? CS$<>8__locals1.formation.CachedMedianPosition).GetNavMeshVec3());
							if ((double)num4 < (double)num3)
							{
								num3 = num4;
								agent = agent2;
							}
						}
					}
				}
				return agent;
			}, 1.5f);
			this._virtualWeightedAverageEnemyPosition = new QueryData<Vec2>(() => CS$<>8__locals1.<>4__this.Formation.Team.GetWeightedAverageOfEnemies(Patch_OrderController.GetFormationVirtualPositionVec2(CS$<>8__locals1.formation)), 0.5f);
			this._weightedAverageFacingTargetEnemyPosition = new QueryData<Vec2>(delegate
			{
				Formation facingEnemyTargetFormation = Patch_OrderController.GetFacingEnemyTargetFormation(CS$<>8__locals1.formation);
				if (facingEnemyTargetFormation == null)
				{
					return CS$<>8__locals1.formation.QuerySystem.WeightedAverageEnemyPosition;
				}
				Vec2 currentPosition = CS$<>8__locals1.formation.CurrentPosition;
				return CommandFormationQuerySystem.WeightedAverageFormationPosition(facingEnemyTargetFormation, currentPosition);
			}, 0.5f);
			this._virtualWeightedAverageFacingTargetEnemyPosition = new QueryData<Vec2>(delegate
			{
				Formation virtualFacingEnemyTargetFormation = Patch_OrderController.GetVirtualFacingEnemyTargetFormation(CS$<>8__locals1.formation);
				if (virtualFacingEnemyTargetFormation == null)
				{
					return CS$<>8__locals1.formation.QuerySystem.WeightedAverageEnemyPosition;
				}
				Vec2 formationVirtualPositionVec = Patch_OrderController.GetFormationVirtualPositionVec2(CS$<>8__locals1.formation);
				return CommandFormationQuerySystem.WeightedAverageFormationPosition(virtualFacingEnemyTargetFormation, formationVirtualPositionVec);
			}, 0.5f);
			this._areAgentsNearTargetPositions = new QueryData<bool>(delegate
			{
				if (Utility.FormationArrangementContainsPlayerOnly(CS$<>8__locals1.formation) && Agent.Main != null && !Agent.Main.IsAIControlled)
				{
					return true;
				}
				if (CS$<>8__locals1.formation.CountOfUnitsWithoutLooseDetachedOnes > 0)
				{
					float scoreSum = 0f;
					int unitCount = CS$<>8__locals1.formation.CountOfUnitsWithoutLooseDetachedOnes;
					CS$<>8__locals1.formation.ApplyActionOnEachAttachedUnit(delegate(Agent agent)
					{
						if (!agent.IsAIControlled)
						{
							int num5 = unitCount - 1;
							unitCount = num5;
						}
						float maximumForwardUnlimitedSpeed = agent.GetMaximumForwardUnlimitedSpeed();
						float num6 = MathF.Max(maximumForwardUnlimitedSpeed, 4f);
						CommandSystemAgentComponent component = agent.GetComponent<CommandSystemAgentComponent>();
						float num7 = ((component != null) ? component.DistanceSquaredToTargetPosition : 0f);
						float num8 = MathF.Pow(2.7182817f, MathF.Min(-(num7 - maximumForwardUnlimitedSpeed * maximumForwardUnlimitedSpeed) / num6, 0f));
						scoreSum += num8;
					});
					return scoreSum > (float)unitCount * 0.5f;
				}
				return true;
			}, 0.5f);
			this._coolDownToEvaluateAgentsDistanceToTarget = new QueryData<bool>(() => false, 0.31f + MBRandom.RandomFloat * 0.2f);
			this._averageMissileRangeAdjusted = new QueryData<float>(delegate
			{
				if (CS$<>8__locals1.formation.CountOfUnits == 0)
				{
					return 0f;
				}
				float sum = 0f;
				int count = 0;
				CS$<>8__locals1.formation.ApplyActionOnEachUnit(delegate(Agent agent)
				{
					if (agent.MissileRangeAdjusted > 0f)
					{
						sum += agent.MissileRangeAdjusted;
						int count2 = count;
						count = count2 + 1;
					}
				}, null);
				if (count == 0)
				{
					return 0f;
				}
				return sum / (float)count;
			}, 5f);
			this._ratioOfAgentsHavingAmmo = new QueryData<float>(delegate
			{
				if (CS$<>8__locals1.formation.CountOfUnits == 0)
				{
					return 0f;
				}
				int countHavingAmmo = 0;
				int totalCurrentAmmo = 0;
				int totalMaxAmmo = 0;
				CS$<>8__locals1.formation.ApplyActionOnEachUnit(delegate(Agent agent)
				{
					int num9;
					int num10;
					Utility.GetMaxAndCurrentAmmoOfAgent(agent, out num9, out num10);
					totalCurrentAmmo += num9;
					totalMaxAmmo += num10;
					if (num10 > 0 && num9 > 0)
					{
						int countHavingAmmo2 = countHavingAmmo;
						countHavingAmmo = countHavingAmmo2 + 1;
					}
				}, null);
				CS$<>8__locals1.<>4__this._ratioOfRemainingAmmo = (float)totalCurrentAmmo / (float)totalMaxAmmo;
				return (float)countHavingAmmo / (float)CS$<>8__locals1.formation.CountOfUnits;
			}, 5f);
			this._ratioOfRemainingAmmoQuery = new QueryData<float>(() => CS$<>8__locals1.<>4__this._ratioOfRemainingAmmo, 5f);
			this._ratioOfRemainingAmmoQuery.SetSyncGroup(new IQueryData[] { this._ratioOfAgentsHavingAmmo });
		}

		// Token: 0x0600030F RID: 783 RVA: 0x0000D6A0 File Offset: 0x0000B8A0
		public void OnOrderPended()
		{
			this._areAgentsNearTargetPositions.Expire();
			this._coolDownToEvaluateAgentsDistanceToTarget.SetValue(true, Mission.Current.CurrentTime);
			this.NeedToUpdateTargetPositionDistance = true;
			this._ratioOfRemainingAmmo = 0f;
		}

		// Token: 0x06000310 RID: 784 RVA: 0x0000D6D8 File Offset: 0x0000B8D8
		private static Vec2 WeightedAverageFormationPosition(Formation targetFormation, Vec2 basePoint)
		{
			Vec2 zero = Vec2.Zero;
			float num1 = 0f;
			targetFormation.ApplyActionOnEachUnit(delegate(Agent agent)
			{
				Vec2 asVec = agent.Position.AsVec2;
				float num = 1f / (basePoint - asVec).LengthSquared;
				zero += asVec * num;
				num1 += num;
			}, null);
			if ((double)num1 <= 0.0)
			{
				return Vec2.Invalid;
			}
			return zero * (1f / num1);
		}

		// Token: 0x04000131 RID: 305
		public readonly Formation Formation;

		// Token: 0x04000132 RID: 306
		public readonly QueryData<Formation> _closestEnemyFormation;

		// Token: 0x04000133 RID: 307
		private readonly QueryData<Agent> _closestEnemyAgent;

		// Token: 0x04000134 RID: 308
		private readonly QueryData<Vec2> _virtualWeightedAverageEnemyPosition;

		// Token: 0x04000135 RID: 309
		private readonly QueryData<Vec2> _weightedAverageFacingTargetEnemyPosition;

		// Token: 0x04000136 RID: 310
		private readonly QueryData<Vec2> _virtualWeightedAverageFacingTargetEnemyPosition;

		// Token: 0x04000137 RID: 311
		private readonly QueryData<bool> _areAgentsNearTargetPositions;

		// Token: 0x04000138 RID: 312
		private readonly QueryData<bool> _coolDownToEvaluateAgentsDistanceToTarget;

		// Token: 0x04000139 RID: 313
		private readonly QueryData<float> _averageMissileRangeAdjusted;

		// Token: 0x0400013A RID: 314
		private readonly QueryData<float> _ratioOfAgentsHavingAmmo;

		// Token: 0x0400013B RID: 315
		private readonly QueryData<float> _ratioOfRemainingAmmoQuery;

		// Token: 0x0400013C RID: 316
		private float _ratioOfRemainingAmmo;

		// Token: 0x0400013D RID: 317
		public bool NeedToUpdateTargetPositionDistance;
	}
}
