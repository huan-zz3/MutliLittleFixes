using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x0200000D RID: 13
	public class TacticNavalRaidDefense : TacticComponent
	{
		// Token: 0x06000065 RID: 101 RVA: 0x000044FF File Offset: 0x000026FF
		public TacticNavalRaidDefense(Team team)
			: base(team)
		{
			this._teamAI = team.TeamAI as TeamAINavalRaidDefenderComponent;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00004519 File Offset: 0x00002719
		protected override void ManageFormationCounts()
		{
			base.AssignTacticFormations1121();
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00004524 File Offset: 0x00002724
		private void FightOffAttackers()
		{
			if (base.Team.IsPlayerTeam && !base.Team.IsPlayerGeneral && base.Team.IsPlayerSergeant)
			{
				base.SoundTacticalHorn(TacticComponent.MoveHornSoundIndex);
			}
			if (this._mainInfantry != null)
			{
				this._mainInfantry.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._mainInfantry);
				this._mainInfantry.AI.SetBehaviorWeight<BehaviorNavalRaidHoldChokePoint>(1f).SetTacticalDefendPosition(this._chokePointTacticalPosition);
			}
			if (this._archers != null)
			{
				this._archers.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._archers);
				this._archers.AI.SetBehaviorWeight<BehaviorSkirmishLine>(1f);
				this._archers.AI.SetBehaviorWeight<BehaviorScreenedSkirmish>(1f);
				if (this._linkedRangedDefensivePosition != null)
				{
					this._archers.AI.SetBehaviorWeight<BehaviorNavalRaidCliffShooting>(1f).SetTacticalDefendPosition(this._linkedRangedDefensivePosition);
				}
			}
			if (this._leftCavalry != null)
			{
				this._leftCavalry.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._leftCavalry);
				this._leftCavalry.AI.SetBehaviorWeight<BehaviorProtectFlank>(1f).FlankSide = 0;
				this._leftCavalry.AI.SetBehaviorWeight<BehaviorCavalryScreen>(1f);
			}
			if (this._rightCavalry != null)
			{
				this._rightCavalry.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._rightCavalry);
				this._rightCavalry.AI.SetBehaviorWeight<BehaviorProtectFlank>(1f).FlankSide = 2;
				this._rightCavalry.AI.SetBehaviorWeight<BehaviorCavalryScreen>(1f);
			}
			if (this._rangedCavalry != null)
			{
				this._rangedCavalry.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._rangedCavalry);
				this._rangedCavalry.AI.SetBehaviorWeight<BehaviorMountedSkirmish>(1f);
				this._rangedCavalry.AI.SetBehaviorWeight<BehaviorHorseArcherSkirmish>(1f);
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00004714 File Offset: 0x00002914
		private void Defend()
		{
			if (base.Team.IsPlayerTeam && !base.Team.IsPlayerGeneral && base.Team.IsPlayerSergeant)
			{
				base.SoundTacticalHorn(TacticComponent.MoveHornSoundIndex);
			}
			if (this._mainInfantry != null)
			{
				this._mainInfantry.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._mainInfantry);
				this._mainInfantry.AI.SetBehaviorWeight<BehaviorNavalRaidHoldChokePoint>(1000f).SetTacticalDefendPosition(this._chokePointTacticalPosition);
			}
			if (this._archers != null)
			{
				this._archers.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._archers);
				this._archers.AI.SetBehaviorWeight<BehaviorSkirmishLine>(1f);
				this._archers.AI.SetBehaviorWeight<BehaviorScreenedSkirmish>(1f);
				if (this._linkedRangedDefensivePosition != null)
				{
					this._archers.AI.SetBehaviorWeight<BehaviorNavalRaidCliffShooting>(1000f).SetTacticalDefendPosition(this._linkedRangedDefensivePosition);
				}
			}
			if (this._leftCavalry != null)
			{
				this._leftCavalry.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._leftCavalry);
				this._leftCavalry.AI.SetBehaviorWeight<BehaviorProtectFlank>(1f).FlankSide = 0;
				this._leftCavalry.AI.SetBehaviorWeight<BehaviorCavalryScreen>(1f);
			}
			if (this._rightCavalry != null)
			{
				this._rightCavalry.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._rightCavalry);
				this._rightCavalry.AI.SetBehaviorWeight<BehaviorProtectFlank>(1f).FlankSide = 2;
				this._rightCavalry.AI.SetBehaviorWeight<BehaviorCavalryScreen>(1f);
			}
			if (this._rangedCavalry != null)
			{
				this._rangedCavalry.AI.ResetBehaviorWeights();
				TacticComponent.SetDefaultBehaviorWeights(this._rangedCavalry);
				this._rangedCavalry.AI.SetBehaviorWeight<BehaviorMountedSkirmish>(1f);
				this._rangedCavalry.AI.SetBehaviorWeight<BehaviorHorseArcherSkirmish>(1f);
			}
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00004904 File Offset: 0x00002B04
		protected override bool CheckAndSetAvailableFormationsChanged()
		{
			int aicontrolledFormationCount = base.Team.GetAIControlledFormationCount();
			bool flag = aicontrolledFormationCount != this._AIControlledFormationCount;
			if (flag)
			{
				this._AIControlledFormationCount = aicontrolledFormationCount;
				this.IsTacticReapplyNeeded = true;
			}
			return flag || (this._mainInfantry != null && (this._mainInfantry.CountOfUnits == 0 || !this._mainInfantry.QuerySystem.IsInfantryFormation)) || (this._archers != null && (this._archers.CountOfUnits == 0 || !this._archers.QuerySystem.IsRangedFormation)) || (this._leftCavalry != null && (this._leftCavalry.CountOfUnits == 0 || !this._leftCavalry.QuerySystem.IsCavalryFormation)) || (this._rightCavalry != null && (this._rightCavalry.CountOfUnits == 0 || !this._rightCavalry.QuerySystem.IsCavalryFormation)) || (this._rangedCavalry != null && (this._rangedCavalry.CountOfUnits == 0 || !this._rangedCavalry.QuerySystem.IsRangedCavalryFormation));
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00004A14 File Offset: 0x00002C14
		public override void TickOccasionally()
		{
			if (!base.AreFormationsCreated)
			{
				return;
			}
			if (this._hasLandingCompleted != this._teamAI.LandingCompleted)
			{
				this.IsTacticReapplyNeeded = true;
				this._hasLandingCompleted = this._teamAI.LandingCompleted;
			}
			if (this.CheckAndSetAvailableFormationsChanged() || this.IsTacticReapplyNeeded)
			{
				this.ManageFormationCounts();
				if (!this._hasLandingCompleted)
				{
					this.Defend();
				}
				else
				{
					this.FightOffAttackers();
				}
				this.IsTacticReapplyNeeded = false;
			}
			base.TickOccasionally();
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00004A90 File Offset: 0x00002C90
		protected override float GetTacticWeight()
		{
			if (!base.Team.TeamAI.IsCurrentTactic(this) || this._chokePointTacticalPosition == null || !this.IsTacticalPositionEligible(this._chokePointTacticalPosition))
			{
				this.DetermineChokePoints();
			}
			if (this._chokePointTacticalPosition == null)
			{
				return 0f;
			}
			if (!this._teamAI.LandingCompleted)
			{
				return 1000f;
			}
			if (!this._teamAI.IsDefenseApplicable)
			{
				return 0.1f;
			}
			return 1f;
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00004B08 File Offset: 0x00002D08
		private bool IsTacticalPositionEligible(TacticalPosition tacticalPosition)
		{
			if (tacticalPosition.TacticalPositionType == 2)
			{
				return true;
			}
			if (!base.CheckAndDetermineFormation(ref this._mainInfantry, (Formation f) => f.CountOfUnits > 0 && f.QuerySystem.IsInfantryFormation))
			{
				return false;
			}
			float num = base.Team.QuerySystem.AveragePosition.Distance(tacticalPosition.Position.AsVec2);
			float num2 = base.Team.QuerySystem.AverageEnemyPosition.Distance(this._mainInfantry.CachedAveragePosition);
			if (num > 20f && num > num2 * 0.5f)
			{
				return false;
			}
			if (this._mainInfantry.MaximumWidth < tacticalPosition.Width)
			{
				return false;
			}
			float num3 = (base.Team.QuerySystem.AverageEnemyPosition - tacticalPosition.Position.AsVec2).Normalized().DotProduct(tacticalPosition.Direction);
			if (tacticalPosition.IsInsurmountable)
			{
				return MathF.Abs(num3) >= 0.5f;
			}
			return num3 >= 0.5f;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00004C28 File Offset: 0x00002E28
		private float GetTacticalPositionScore(TacticalPosition tacticalPosition)
		{
			if (base.CheckAndDetermineFormation(ref this._mainInfantry, (Formation f) => f.CountOfUnits > 0 && f.QuerySystem.IsInfantryFormation))
			{
				float num = MBMath.Lerp(1f, 1.5f, MBMath.ClampFloat(tacticalPosition.Slope, 0f, 60f) / 60f, 1E-05f);
				int countOfUnits = this._mainInfantry.CountOfUnits;
				float num2 = this._mainInfantry.Interval * (float)(countOfUnits - 1) + this._mainInfantry.UnitDiameter * (float)countOfUnits;
				float num3 = MBMath.Lerp(0.67f, 1.5f, (MBMath.ClampFloat(num2 / tacticalPosition.Width, 0.5f, 3f) - 0.5f) / 2.5f, 1E-05f);
				float num4 = 1f;
				if (base.CheckAndDetermineFormation(ref this._archers, (Formation f) => f.CountOfUnits > 0 && f.QuerySystem.IsRangedFormation))
				{
					if (tacticalPosition.LinkedTacticalPositions.Where<TacticalPosition>((TacticalPosition lcp) => lcp.TacticalPositionType == 3).ToList<TacticalPosition>().Count > 0)
					{
						num4 = MBMath.Lerp(1f, 1.5f, (MBMath.ClampFloat(base.Team.QuerySystem.RangedRatio, 0.05f, 0.25f) - 0.05f) * 5f, 1E-05f);
					}
				}
				float num5 = this._mainInfantry.CachedAveragePosition.Distance(tacticalPosition.Position.AsVec2);
				float num6 = MBMath.Lerp(0.7f, 1f, (150f - MBMath.ClampFloat(num5, 50f, 150f)) / 100f, 1E-05f);
				return num * num3 * num4 * num6;
			}
			return 0f;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00004E07 File Offset: 0x00003007
		protected override bool ResetTacticalPositions()
		{
			this.DetermineChokePoints();
			return true;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00004E10 File Offset: 0x00003010
		private void DetermineChokePoints()
		{
			IEnumerable<ValueTuple<TacticalPosition, float>> enumerable = from tp in base.Team.TeamAI.TacticalPositions
				where tp.TacticalPositionType == 2 && this.IsTacticalPositionEligible(tp)
				select new ValueTuple<TacticalPosition, float>(tp, this.GetTacticalPositionScore(tp));
			IEnumerable<ValueTuple<TacticalPosition, float>> enumerable2 = from tp in base.Team.TeamAI.TacticalRegions.SelectMany<TacticalRegion, TacticalPosition>((TacticalRegion r) => r.LinkedTacticalPositions.Where<TacticalPosition>((TacticalPosition tpftr) => tpftr.TacticalPositionType == 2 && this.IsTacticalPositionEligible(tpftr)))
				select new ValueTuple<TacticalPosition, float>(tp, this.GetTacticalPositionScore(tp));
			IEnumerable<ValueTuple<TacticalPosition, float>> enumerable3 = enumerable.Concat<ValueTuple<TacticalPosition, float>>(enumerable2);
			if (enumerable3.Any<ValueTuple<TacticalPosition, float>>())
			{
				TacticalPosition item = Extensions.MaxBy<ValueTuple<TacticalPosition, float>, float>(enumerable3, ([TupleElementNames(new string[] { "tp", null })] ValueTuple<TacticalPosition, float> pst) => pst.Item2).Item1;
				if (item != this._chokePointTacticalPosition)
				{
					this._chokePointTacticalPosition = item;
					this.IsTacticReapplyNeeded = true;
				}
				if (this._chokePointTacticalPosition.LinkedTacticalPositions.Count <= 0)
				{
					this._linkedRangedDefensivePosition = null;
					return;
				}
				TacticalPosition tacticalPosition = this._chokePointTacticalPosition.LinkedTacticalPositions.FirstOrDefault<TacticalPosition>();
				if (tacticalPosition != this._linkedRangedDefensivePosition)
				{
					this._linkedRangedDefensivePosition = tacticalPosition;
					this.IsTacticReapplyNeeded = true;
					return;
				}
			}
			else
			{
				this._chokePointTacticalPosition = null;
			}
		}

		// Token: 0x0400003C RID: 60
		private TacticalPosition _chokePointTacticalPosition;

		// Token: 0x0400003D RID: 61
		private TacticalPosition _linkedRangedDefensivePosition;

		// Token: 0x0400003E RID: 62
		private bool _hasLandingCompleted;

		// Token: 0x0400003F RID: 63
		private TeamAINavalRaidDefenderComponent _teamAI;
	}
}
