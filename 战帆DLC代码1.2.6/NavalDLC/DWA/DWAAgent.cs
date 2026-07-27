using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TaleWorlds.Library;

namespace NavalDLC.DWA
{
	// Token: 0x02000147 RID: 327
	public class DWAAgent
	{
		// Token: 0x1700039E RID: 926
		// (get) Token: 0x0600158C RID: 5516 RVA: 0x00096ACC File Offset: 0x00094CCC
		// (set) Token: 0x0600158D RID: 5517 RVA: 0x00096AD4 File Offset: 0x00094CD4
		public int Id { get; private set; }

		// Token: 0x1700039F RID: 927
		// (get) Token: 0x0600158E RID: 5518 RVA: 0x00096ADD File Offset: 0x00094CDD
		public readonly ref DWAAgentState State
		{
			get
			{
				return this.Delegate.State;
			}
		}

		// Token: 0x170003A0 RID: 928
		// (get) Token: 0x0600158F RID: 5519 RVA: 0x00096AEA File Offset: 0x00094CEA
		public MBReadOnlyList<KeyValuePair<float, DWAAgent>> AgentNeighbors
		{
			get
			{
				return this._agentNeighbors;
			}
		}

		// Token: 0x170003A1 RID: 929
		// (get) Token: 0x06001590 RID: 5520 RVA: 0x00096AF2 File Offset: 0x00094CF2
		public MBReadOnlyList<KeyValuePair<float, DWAObstacleVertex>> ObstacleNeighbors
		{
			get
			{
				return this._obstacleNeighbors;
			}
		}

		// Token: 0x170003A2 RID: 930
		// (get) Token: 0x06001591 RID: 5521 RVA: 0x00096AFA File Offset: 0x00094CFA
		// (set) Token: 0x06001592 RID: 5522 RVA: 0x00096B02 File Offset: 0x00094D02
		public IDWAAgentDelegate Delegate { get; private set; }

		// Token: 0x170003A3 RID: 931
		// (get) Token: 0x06001593 RID: 5523 RVA: 0x00096B0B File Offset: 0x00094D0B
		// (set) Token: 0x06001594 RID: 5524 RVA: 0x00096B13 File Offset: 0x00094D13
		public bool IsForecast { get; private set; }

		// Token: 0x170003A4 RID: 932
		// (get) Token: 0x06001595 RID: 5525 RVA: 0x00096B1C File Offset: 0x00094D1C
		// (set) Token: 0x06001596 RID: 5526 RVA: 0x00096B24 File Offset: 0x00094D24
		public int LastForecastNumTimeSamples { get; private set; }

		// Token: 0x170003A5 RID: 933
		// (get) Token: 0x06001597 RID: 5527 RVA: 0x00096B2D File Offset: 0x00094D2D
		[TupleElementNames(new string[] { "distance", "amount" })]
		public ValueTuple<float, float> TargetOcclusion
		{
			[return: TupleElementNames(new string[] { "distance", "amount" })]
			get
			{
				return this._targetOcclusion;
			}
		}

		// Token: 0x06001598 RID: 5528 RVA: 0x00096B35 File Offset: 0x00094D35
		public DWAAgent(DWASimulator simulator, int id, IDWAAgentDelegate agentDelegate)
		{
			this.Id = id;
			this._simulator = simulator;
			this.Delegate = agentDelegate;
			this._lastStateUpdateParity = ushort.MaxValue;
		}

		// Token: 0x06001599 RID: 5529 RVA: 0x00096B73 File Offset: 0x00094D73
		public bool TryUpdateState(ushort parity)
		{
			if (parity != this._lastStateUpdateParity)
			{
				this.IsForecast = false;
				this.Delegate.OnStateUpdate();
				this._lastStateUpdateParity = parity;
				return true;
			}
			return false;
		}

		// Token: 0x0600159A RID: 5530 RVA: 0x00096B9A File Offset: 0x00094D9A
		public bool IsStateUpToDate(ushort parity)
		{
			return this._lastStateUpdateParity == parity;
		}

		// Token: 0x0600159B RID: 5531 RVA: 0x00096BA8 File Offset: 0x00094DA8
		public unsafe void ComputeNeighbors(ushort parity)
		{
			this._obstacleNeighbors.Clear();
			float neighborDistance = this.Delegate.NeighborDistance;
			float num = neighborDistance * neighborDistance;
			if (this.Delegate.AvoidObstacleCollisions)
			{
				DWASimulatorParameters dwasimulatorParameters = *this._simulator.Parameters;
				if (dwasimulatorParameters.MaxObstacleNeighbors > 0)
				{
					this._simulator.ComputeObstacleNeighbors(this, num);
				}
			}
			this._agentNeighbors.Clear();
			if (this.Delegate.AvoidAgentCollisions)
			{
				DWASimulatorParameters dwasimulatorParameters = *this._simulator.Parameters;
				if (dwasimulatorParameters.MaxAgentNeighbors > 0)
				{
					this._simulator.ComputeAgentNeighbors(this, num, parity);
				}
			}
		}

		// Token: 0x0600159C RID: 5532 RVA: 0x00096C44 File Offset: 0x00094E44
		public void SetForecastStates(int maxTimeSamples)
		{
			if (this._forecastStates == null || this._forecastStates.Length != maxTimeSamples)
			{
				this._forecastStates = new DWAAgentState[maxTimeSamples];
			}
		}

		// Token: 0x0600159D RID: 5533 RVA: 0x00096C68 File Offset: 0x00094E68
		public unsafe void ForecastTrajectory(float dt, int numTimeSamples)
		{
			this.LastForecastNumTimeSamples = numTimeSamples;
			DWAAgentState dwaagentState = *this.State;
			DWAAgentState dwaagentState2 = default(DWAAgentState);
			for (int i = 0; i < numTimeSamples; i++)
			{
				this.IntegrateState(in dwaagentState, dt, ref dwaagentState2);
				this._forecastStates[i] = dwaagentState2;
				dwaagentState = dwaagentState2;
			}
			this.IsForecast = true;
		}

		// Token: 0x0600159E RID: 5534 RVA: 0x00096CC0 File Offset: 0x00094EC0
		public unsafe void InsertAgentNeighbor(DWAAgent agent, ref float rangeSq, ushort parity)
		{
			if (this != agent)
			{
				agent.TryUpdateState(parity);
				float lengthSquared = (this.State.Position - agent.State.Position).LengthSquared;
				DWASimulatorParameters dwasimulatorParameters = *this._simulator.Parameters;
				int maxAgentNeighbors = dwasimulatorParameters.MaxAgentNeighbors;
				int num = this._agentNeighbors.Count;
				if (num == maxAgentNeighbors && lengthSquared >= rangeSq)
				{
					return;
				}
				if (num < maxAgentNeighbors)
				{
					this._agentNeighbors.Add(new KeyValuePair<float, DWAAgent>(lengthSquared, agent));
					num++;
				}
				int num2 = num - 1;
				while (num2 != 0 && lengthSquared < this._agentNeighbors[num2 - 1].Key)
				{
					this._agentNeighbors[num2] = this._agentNeighbors[num2 - 1];
					num2--;
				}
				this._agentNeighbors[num2] = new KeyValuePair<float, DWAAgent>(lengthSquared, agent);
				if (this._agentNeighbors.Count == maxAgentNeighbors)
				{
					rangeSq = this._agentNeighbors[this._agentNeighbors.Count - 1].Key;
				}
			}
		}

		// Token: 0x0600159F RID: 5535 RVA: 0x00096DD0 File Offset: 0x00094FD0
		public unsafe void InsertObstacleNeighbor(DWAObstacleVertex obstacle, ref float rangeSq)
		{
			DWAObstacleVertex next = obstacle.Next;
			DWASimulatorParameters dwasimulatorParameters = *this._simulator.Parameters;
			int maxObstacleNeighbors = dwasimulatorParameters.MaxObstacleNeighbors;
			Vec2 point = obstacle.Point;
			Vec2 point2 = next.Point;
			float distanceSquareOfPointToLineSegment = MBMath.GetDistanceSquareOfPointToLineSegment(ref point, ref point2, this.State.Position);
			int num = this._obstacleNeighbors.Count;
			if (num == maxObstacleNeighbors && distanceSquareOfPointToLineSegment >= rangeSq)
			{
				return;
			}
			if (num < maxObstacleNeighbors)
			{
				this._obstacleNeighbors.Add(default(KeyValuePair<float, DWAObstacleVertex>));
				num++;
			}
			int num2 = num - 1;
			while (num2 != 0 && distanceSquareOfPointToLineSegment < this._obstacleNeighbors[num2 - 1].Key)
			{
				this._obstacleNeighbors[num2] = this._obstacleNeighbors[num2 - 1];
				num2--;
			}
			this._obstacleNeighbors[num2] = new KeyValuePair<float, DWAObstacleVertex>(distanceSquareOfPointToLineSegment, obstacle);
			if (this._obstacleNeighbors.Count == maxObstacleNeighbors)
			{
				rangeSq = this._obstacleNeighbors[this._obstacleNeighbors.Count - 1].Key;
			}
		}

		// Token: 0x060015A0 RID: 5536 RVA: 0x00096EE4 File Offset: 0x000950E4
		public void InitializeThreads(in DWASimulatorParameters parameters, DWAThread[] processThreads)
		{
			DWASimulatorParameters dwasimulatorParameters = parameters;
			int numLinearAccelerationSamples = dwasimulatorParameters.NumLinearAccelerationSamples;
			dwasimulatorParameters = parameters;
			int numAngularAccelerationSamples = dwasimulatorParameters.NumAngularAccelerationSamples;
			dwasimulatorParameters = parameters;
			bool ignoreZeroAction = dwasimulatorParameters.IgnoreZeroAction;
			float maxLinearAcceleration = this.Delegate.MaxLinearAcceleration;
			float maxAngularAcceleration = this.Delegate.MaxAngularAcceleration;
			int num = numLinearAccelerationSamples / 2;
			int num2 = numAngularAccelerationSamples / 2;
			float num3 = ((numLinearAccelerationSamples > 1) ? (2f * maxLinearAcceleration / (float)(numLinearAccelerationSamples - 1)) : 0f);
			float num4 = ((numAngularAccelerationSamples > 1) ? (2f * maxAngularAcceleration / (float)(numAngularAccelerationSamples - 1)) : 0f);
			int num5 = 0;
			for (int i = 0; i < numLinearAccelerationSamples; i++)
			{
				float num6 = -maxLinearAcceleration + (float)i * num3;
				if (i == num)
				{
					num6 = 0f;
				}
				for (int j = 0; j < numAngularAccelerationSamples; j++)
				{
					float num7 = -maxAngularAcceleration + (float)j * num4;
					if (j == num2)
					{
						num7 = 0f;
					}
					if (!ignoreZeroAction || j != num2 || i != num)
					{
						DWAThread dwathread = processThreads[num5++];
						float num8 = num6;
						float num9 = num7;
						dwasimulatorParameters = parameters;
						float deltaTime = dwasimulatorParameters.DeltaTime;
						dwasimulatorParameters = parameters;
						dwathread.Initialize(this, num8, num9, deltaTime, dwasimulatorParameters.NumTimeSamples);
					}
				}
			}
		}

		// Token: 0x060015A1 RID: 5537 RVA: 0x00097014 File Offset: 0x00095214
		public unsafe void ComputeTargetOcclusion()
		{
			Vec2 vec;
			float goalDirection = this.Delegate.GetGoalDirection(out vec);
			DWAAgentState dwaagentState = *this.Delegate.State;
			float minExtent = dwaagentState.MinExtent;
			dwaagentState = *this.Delegate.State;
			float maxExtent = dwaagentState.MaxExtent;
			float num = 2.5f * minExtent;
			float num2 = MathF.Min(goalDirection, 8f * maxExtent);
			float num3 = 0f;
			float num4 = float.PositiveInfinity;
			foreach (KeyValuePair<float, DWAAgent> keyValuePair in this._agentNeighbors)
			{
				dwaagentState = *keyValuePair.Value.State;
				Vec2 vec2 = dwaagentState.ShapeCenter - this.State.Position;
				float num5 = Vec2.DotProduct(vec2, vec);
				if (num5 > 0f && num5 < num2)
				{
					float num6 = MathF.Abs(Vec2.DotProduct(vec2, vec.LeftVec()));
					float num7 = 2f * maxExtent;
					float num8 = DWAHelpers.GateNear(num6, num, 0f) * DWAHelpers.GateNear(num5, MathF.Max(num2 - num7, 1E-05f), num7);
					if (num8 > num3)
					{
						num3 = num8;
					}
					if (num6 < num && num5 < num4)
					{
						num4 = num5;
					}
				}
			}
			int num9 = 100;
			foreach (KeyValuePair<float, DWAObstacleVertex> keyValuePair2 in this._obstacleNeighbors)
			{
				DWAObstacleVertex value = keyValuePair2.Value;
				int num10 = 0;
				DWAObstacleVertex dwaobstacleVertex = value;
				do
				{
					Vec2 vec3 = dwaobstacleVertex.Point - this.State.Position;
					float num11 = Vec2.DotProduct(vec3, vec);
					if (num11 > 0f && num11 < num2)
					{
						float num12 = MathF.Abs(Vec2.DotProduct(vec3, vec.LeftVec()));
						float num13 = DWAHelpers.GateNear(num12, num, 0f) * DWAHelpers.GateNear(num11, num2, 0f);
						if (num13 > num3)
						{
							num3 = num13;
						}
						if (num12 < num && num11 < num4)
						{
							num4 = num11;
						}
					}
					dwaobstacleVertex = dwaobstacleVertex.Next;
				}
				while (dwaobstacleVertex != value && num10 < num9);
			}
			if (float.IsPositiveInfinity(num4))
			{
				num4 = num2;
			}
			this._targetOcclusion = new ValueTuple<float, float>(num4, num3);
		}

		// Token: 0x060015A2 RID: 5538 RVA: 0x00097270 File Offset: 0x00095470
		public void EvaluateState(in DWAAgentState state, int sampleIndex, out bool hasCollision, out DWAAgent collidedAgent, out DWAObstacleVertex collidedObstacle, out float goalCost, out float proxCost, out float maxPenetration, Vec2[] obstaclePolyBuffer)
		{
			goalCost = this.Delegate.ComputeGoalCost(sampleIndex, in state, this._targetOcclusion);
			hasCollision = false;
			collidedAgent = null;
			collidedObstacle = null;
			Vec2 shapeHalfSize = state.ShapeHalfSize;
			MathF.Max(shapeHalfSize.x, shapeHalfSize.y);
			float safetyFactor = this.Delegate.GetSafetyFactor();
			float num = 0f;
			float num2 = 0f;
			maxPenetration = 0f;
			foreach (KeyValuePair<float, DWAAgent> keyValuePair in this._agentNeighbors)
			{
				DWAAgent value = keyValuePair.Value;
				ref DWAAgentState ptr = ref value._forecastStates[sampleIndex];
				DWAAgentState dwaagentState = state;
				Vec2 vec = dwaagentState.ShapeCenter;
				dwaagentState = ptr;
				Vec2 shapeCenter = dwaagentState.ShapeCenter;
				float num3 = DWAHelpers.AgentToAgentSignedClearance(in vec, in state.Direction, in state.ShapeHalfSize, in shapeCenter, in ptr.Direction, in ptr.ShapeHalfSize);
				bool flag = num3 < 0f;
				float num4 = -MathF.Min(0f, num3);
				maxPenetration = MathF.Max(maxPenetration, num4);
				float num5 = DWAAgent.ProximityCost(num3, safetyFactor);
				num += num5;
				if (flag && collidedAgent == null)
				{
					hasCollision = true;
					collidedAgent = value;
				}
			}
			foreach (KeyValuePair<float, DWAObstacleVertex> keyValuePair2 in this._obstacleNeighbors)
			{
				DWAObstacleVertex value2 = keyValuePair2.Value;
				int num6;
				DWAHelpers.ReadStaticObstacle(value2, obstaclePolyBuffer, out num6);
				DWAAgentState dwaagentState = state;
				Vec2 vec = dwaagentState.ShapeCenter;
				bool flag2;
				float num7 = DWAHelpers.AgentToConvexPolySignedClearance(in vec, in state.Direction, in state.ShapeHalfSize, obstaclePolyBuffer, num6, out flag2);
				float num8 = -MathF.Min(0f, num7);
				maxPenetration = MathF.Max(maxPenetration, num8);
				float num9 = MathF.Max(0f, num7);
				float num10;
				if (flag2)
				{
					hasCollision = true;
					if (collidedObstacle == null)
					{
						collidedObstacle = value2;
					}
					num10 = DWAAgent.ProximityCost(0f, safetyFactor);
				}
				else
				{
					num10 = DWAAgent.ProximityCost(num9, safetyFactor);
				}
				num2 += num10;
			}
			proxCost = num + num2;
		}

		// Token: 0x060015A3 RID: 5539 RVA: 0x000974A4 File Offset: 0x000956A4
		[return: TupleElementNames(new string[] { "dV", "dOmega" })]
		public ValueTuple<float, float> SelectAction(DWAThread[] threads, out int selectedActionThreadIndex, out DWAThread selectedActionThread)
		{
			float num = 0.02f;
			float num2 = 1f;
			Vec2 shapeHalfSize = this.State.ShapeHalfSize;
			float y = shapeHalfSize.Y;
			selectedActionThread = null;
			selectedActionThreadIndex = -1;
			int num3 = 0;
			float num4 = float.PositiveInfinity;
			for (int i = 0; i < threads.Length; i++)
			{
				float cost = threads[i].Cost;
				if (cost < num4)
				{
					num4 = cost;
					num3 = i;
				}
			}
			DWAThread dwathread = threads[num3];
			ValueTuple<float, float> selectedAction = this.Delegate.GetSelectedAction();
			float item = selectedAction.Item1;
			float item2 = selectedAction.Item2;
			int num5 = 0;
			float num6 = float.PositiveInfinity;
			for (int j = 0; j < threads.Length; j++)
			{
				DWAThread dwathread2 = threads[j];
				float num7 = num2 * MathF.Abs(dwathread2.DV - item) + y * MathF.Abs(dwathread2.DOmega - item2);
				if (num7 < num6)
				{
					num6 = num7;
					num5 = j;
				}
			}
			DWAThread dwathread3 = threads[num5];
			if (num5 == num3)
			{
				selectedActionThreadIndex = num3;
				selectedActionThread = dwathread;
				return new ValueTuple<float, float>(dwathread.DV, dwathread.DOmega);
			}
			float cost2 = dwathread3.Cost;
			float num8 = cost2 - num4;
			float num9 = MathF.Max(1f, cost2);
			if (num8 / num9 >= num)
			{
				selectedActionThreadIndex = num3;
				selectedActionThread = dwathread;
				return new ValueTuple<float, float>(dwathread.DV, dwathread.DOmega);
			}
			selectedActionThreadIndex = num5;
			selectedActionThread = dwathread3;
			return new ValueTuple<float, float>(dwathread3.DV, dwathread3.DOmega);
		}

		// Token: 0x060015A4 RID: 5540 RVA: 0x00097600 File Offset: 0x00095800
		internal void IntegrateState(in DWAAgentState curState, float dt, ref DWAAgentState newState)
		{
			float num = dt * dt;
			Vec2 position = curState.Position;
			Vec2 direction = curState.Direction;
			Vec2 linearVelocity = curState.LinearVelocity;
			float angularVelocity = curState.AngularVelocity;
			float linearAcceleration = curState.LinearAcceleration;
			float angularAcceleration = curState.AngularAcceleration;
			Vec2 vec;
			float num2;
			this.Delegate.ComputeExternalAccelerationsOnState(dt, in curState, out vec, out num2);
			float num3 = angularVelocity * dt + 0.5f * angularAcceleration * num;
			Vec2 vec2 = direction;
			vec2.RotateCCW(num3 * 0.5f);
			Vec2 vec3 = linearVelocity + (linearAcceleration * vec2 + vec) * dt;
			float num4 = angularVelocity + (angularAcceleration + num2) * dt;
			Vec2 vec4 = position + 0.5f * (linearVelocity + vec3) * dt;
			Vec2 vec5 = direction;
			vec5.RotateCCW(num3);
			newState.Position = vec4;
			newState.Direction = vec5;
			newState.LinearVelocity = vec3;
			newState.AngularVelocity = num4;
			newState.LinearAcceleration = curState.LinearAcceleration;
			newState.AngularAcceleration = curState.AngularAcceleration;
			newState.PositionZ = curState.PositionZ;
			newState.ShapeHalfSize = curState.ShapeHalfSize;
			newState.ShapeOffset = curState.ShapeOffset;
		}

		// Token: 0x060015A5 RID: 5541 RVA: 0x0009771C File Offset: 0x0009591C
		public static float ProximityCost(float signedClearDist, float safetyFactor = 1f)
		{
			float num = 1f;
			if (signedClearDist <= 0f)
			{
				return 1f;
			}
			float num2 = 1f / (1f + signedClearDist / safetyFactor);
			return num * num2;
		}

		// Token: 0x04000B18 RID: 2840
		private ushort _lastStateUpdateParity;

		// Token: 0x04000B19 RID: 2841
		private MBList<KeyValuePair<float, DWAAgent>> _agentNeighbors = new MBList<KeyValuePair<float, DWAAgent>>();

		// Token: 0x04000B1A RID: 2842
		private MBList<KeyValuePair<float, DWAObstacleVertex>> _obstacleNeighbors = new MBList<KeyValuePair<float, DWAObstacleVertex>>();

		// Token: 0x04000B1B RID: 2843
		private readonly DWASimulator _simulator;

		// Token: 0x04000B1C RID: 2844
		private DWAAgentState[] _forecastStates;

		// Token: 0x04000B1D RID: 2845
		[TupleElementNames(new string[] { "distance", "amount" })]
		private ValueTuple<float, float> _targetOcclusion;
	}
}
