using System;
using System.Runtime.CompilerServices;
using TaleWorlds.Library;

namespace NavalDLC.DWA
{
	// Token: 0x02000151 RID: 337
	public interface IDWAAgentDelegate
	{
		// Token: 0x170003D4 RID: 980
		// (get) Token: 0x0600162D RID: 5677
		readonly ref DWAAgentState State { get; }

		// Token: 0x170003D5 RID: 981
		// (get) Token: 0x0600162E RID: 5678
		float NeighborDistance { get; }

		// Token: 0x170003D6 RID: 982
		// (get) Token: 0x0600162F RID: 5679
		float MaxLinearSpeed { get; }

		// Token: 0x170003D7 RID: 983
		// (get) Token: 0x06001630 RID: 5680
		float MaxLinearAcceleration { get; }

		// Token: 0x170003D8 RID: 984
		// (get) Token: 0x06001631 RID: 5681
		float MaxAngularSpeed { get; }

		// Token: 0x170003D9 RID: 985
		// (get) Token: 0x06001632 RID: 5682
		float MaxAngularAcceleration { get; }

		// Token: 0x170003DA RID: 986
		// (get) Token: 0x06001633 RID: 5683
		bool AvoidAgentCollisions { get; }

		// Token: 0x170003DB RID: 987
		// (get) Token: 0x06001634 RID: 5684
		bool AvoidObstacleCollisions { get; }

		// Token: 0x06001635 RID: 5685
		void Initialize(int id);

		// Token: 0x06001636 RID: 5686
		void SetParameters(in DWASimulatorParameters parameters);

		// Token: 0x06001637 RID: 5687
		float GetSafetyFactor();

		// Token: 0x06001638 RID: 5688
		bool CanPlanTrajectory();

		// Token: 0x06001639 RID: 5689
		bool HasArrivedAtTarget();

		// Token: 0x0600163A RID: 5690
		bool IsAgentEligibleNeighbor(int targetAgentId, IDWAAgentDelegate targetAgentDelegate);

		// Token: 0x0600163B RID: 5691
		bool IsObstacleSegmentEligibleNeighbor(IDWAObstacleVertex obstacle1, IDWAObstacleVertex obstacle2);

		// Token: 0x0600163C RID: 5692
		void OnStateUpdate();

		// Token: 0x0600163D RID: 5693
		void UpdateSelectedAction(float dV, float dOmega);

		// Token: 0x0600163E RID: 5694
		float GetGoalDirection(out Vec2 goalDir);

		// Token: 0x0600163F RID: 5695
		[return: TupleElementNames(new string[] { "dV", "dOmega" })]
		ValueTuple<float, float> GetSelectedAction();

		// Token: 0x06001640 RID: 5696
		void ComputeExternalAccelerationsOnState(float dt, in DWAAgentState state, out Vec2 extLinearAcc, out float extAngularAcc);

		// Token: 0x06001641 RID: 5697
		float ComputeGoalCost(int sampleIndex, in DWAAgentState atState, [TupleElementNames(new string[] { "distance", "amount" })] ValueTuple<float, float> targetOcclusion);
	}
}
