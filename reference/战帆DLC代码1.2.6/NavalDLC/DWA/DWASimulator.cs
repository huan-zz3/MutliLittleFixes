using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace NavalDLC.DWA
{
	// Token: 0x0200014E RID: 334
	public class DWASimulator
	{
		// Token: 0x170003B5 RID: 949
		// (get) Token: 0x060015CD RID: 5581 RVA: 0x000987EC File Offset: 0x000969EC
		public bool IsInitialized
		{
			get
			{
				return this._isInitialized;
			}
		}

		// Token: 0x170003B6 RID: 950
		// (get) Token: 0x060015CE RID: 5582 RVA: 0x000987F4 File Offset: 0x000969F4
		internal readonly ref DWASimulatorParameters Parameters
		{
			get
			{
				return ref this._parameters;
			}
		}

		// Token: 0x170003B7 RID: 951
		// (get) Token: 0x060015CF RID: 5583 RVA: 0x000987FC File Offset: 0x000969FC
		public int NumAgents
		{
			get
			{
				return this._agentsData.Count - this._removedAgentIndices.Count;
			}
		}

		// Token: 0x170003B8 RID: 952
		// (get) Token: 0x060015D0 RID: 5584 RVA: 0x00098815 File Offset: 0x00096A15
		public int NumObstacles
		{
			get
			{
				return this._obstaclesData.Count;
			}
		}

		// Token: 0x170003B9 RID: 953
		// (get) Token: 0x060015D1 RID: 5585 RVA: 0x00098822 File Offset: 0x00096A22
		internal MBReadOnlyList<DWAAgent> AgentsIncludingRemoved
		{
			get
			{
				return this._agentsData;
			}
		}

		// Token: 0x170003BA RID: 954
		// (get) Token: 0x060015D2 RID: 5586 RVA: 0x0009882A File Offset: 0x00096A2A
		internal MBReadOnlyList<DWAObstacleVertex> Obstacles
		{
			get
			{
				return this._obstaclesData;
			}
		}

		// Token: 0x060015D3 RID: 5587 RVA: 0x00098834 File Offset: 0x00096A34
		public DWASimulator()
		{
			this._agentsData = new MBList<DWAAgent>();
			this._obstaclesData = new MBList<DWAObstacleVertex>();
			this._obstacleIndices = new MBList<int>();
			this._removedAgentIndices = new MBList<int>();
			DWASimulatorParameters dwasimulatorParameters = DWASimulatorParameters.Create();
			this.SetParameters(in dwasimulatorParameters);
			this._kdTree = new DWAKdTree(this);
			this.RunSampleThreadsAuxParallelPredicate = new TWParallel.ParallelForAuxPredicate(this.RunSampleThreadsAuxParallel);
			this._parity = 0;
		}

		// Token: 0x060015D4 RID: 5588 RVA: 0x000988A8 File Offset: 0x00096AA8
		public void SetParameters(in DWASimulatorParameters newParameters)
		{
			this._parameters.CopyFrom(in newParameters);
			if (this._parameters.CheckRequiresUpdate(true))
			{
				this._agentsToProcessCount = 0;
				this._agentsToProcess = new DWAAgent[this._parameters.AgentsToProcessPerTick];
				this._currentAgentIndexToProcess = 0;
				this._processThreads = new DWAThread[this._parameters.TotalNumAccelerationSamples];
				for (int i = 0; i < this._processThreads.Length; i++)
				{
					this._processThreads[i] = new DWAThread(i);
				}
				foreach (DWAAgent dwaagent in this._agentsData)
				{
					if (dwaagent != null)
					{
						dwaagent.SetForecastStates(this._parameters.NumTimeSamples);
					}
				}
			}
		}

		// Token: 0x060015D5 RID: 5589 RVA: 0x00098980 File Offset: 0x00096B80
		public unsafe DWAAgentState GetAgentAgentNeighbor(int agentId, int neighborIndex)
		{
			return *this._agentsData[agentId].AgentNeighbors[neighborIndex].Value.State;
		}

		// Token: 0x060015D6 RID: 5590 RVA: 0x000989B8 File Offset: 0x00096BB8
		public IDWAObstacleVertex GetAgentObstacleNeighbor(int agentId, int neighborIndex)
		{
			return this._agentsData[agentId].ObstacleNeighbors[neighborIndex].Value;
		}

		// Token: 0x060015D7 RID: 5591 RVA: 0x000989E4 File Offset: 0x00096BE4
		public unsafe DWAAgentState GetAgentState(int agentId)
		{
			return *this._agentsData[agentId].State;
		}

		// Token: 0x060015D8 RID: 5592 RVA: 0x000989FC File Offset: 0x00096BFC
		public int GetAgentNumAgentNeighbors(int agentId)
		{
			return this._agentsData[agentId].AgentNeighbors.Count;
		}

		// Token: 0x060015D9 RID: 5593 RVA: 0x00098A14 File Offset: 0x00096C14
		public int GetAgentNumObstacleNeighbors(int agentId)
		{
			return this._agentsData[agentId].ObstacleNeighbors.Count;
		}

		// Token: 0x060015DA RID: 5594 RVA: 0x00098A2C File Offset: 0x00096C2C
		public IDWAObstacleVertex GetObstacle(int obstacleId)
		{
			return this._obstaclesData[obstacleId];
		}

		// Token: 0x060015DB RID: 5595 RVA: 0x00098A3A File Offset: 0x00096C3A
		public IDWAObstacleVertex GetNextObstacleOfObstacle(int obstacleId)
		{
			return this._obstaclesData[obstacleId].Next;
		}

		// Token: 0x060015DC RID: 5596 RVA: 0x00098A4D File Offset: 0x00096C4D
		public IDWAObstacleVertex GetPrevObstacleOfObstacle(int obstacleId)
		{
			return this._obstaclesData[obstacleId].Previous;
		}

		// Token: 0x060015DD RID: 5597 RVA: 0x00098A60 File Offset: 0x00096C60
		public int AddAgent(IDWAAgentDelegate agentDelegate)
		{
			int num;
			DWAAgent dwaagent;
			if (this._removedAgentIndices.Count > 0)
			{
				num = this._removedAgentIndices.Last<int>();
				this._removedAgentIndices.RemoveAt(this._removedAgentIndices.Count - 1);
				dwaagent = new DWAAgent(this, num, agentDelegate);
				this._agentsData[num] = dwaagent;
			}
			else
			{
				num = this._agentsData.Count;
				dwaagent = new DWAAgent(this, num, agentDelegate);
				this._agentsData.Add(dwaagent);
			}
			dwaagent.SetForecastStates(this._parameters.NumTimeSamples);
			dwaagent.Delegate.Initialize(num);
			return num;
		}

		// Token: 0x060015DE RID: 5598 RVA: 0x00098AF8 File Offset: 0x00096CF8
		public bool RemoveAgent(IDWAAgentDelegate agentDelegate)
		{
			for (int i = 0; i < this._agentsData.Count; i++)
			{
				if (this._agentsData[i] != null && this.AgentsIncludingRemoved[i].Delegate == agentDelegate)
				{
					this.RemoveAgent(i);
					return true;
				}
			}
			return false;
		}

		// Token: 0x060015DF RID: 5599 RVA: 0x00098B47 File Offset: 0x00096D47
		public void RemoveAgent(int agentIndex)
		{
			this._agentsData[agentIndex] = null;
			this.InsertRemovedIndex(agentIndex);
		}

		// Token: 0x060015E0 RID: 5600 RVA: 0x00098B60 File Offset: 0x00096D60
		public int AddObstacle(MBList<Vec3> vertices)
		{
			if (vertices.Count < 2)
			{
				Debug.FailedAssert("Obstacle vertex count must be greater than one", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\DWACollision\\DWASimulator.cs", "AddObstacle", 329);
				return -1;
			}
			int count = this._obstaclesData.Count;
			for (int i = 0; i < vertices.Count; i++)
			{
				DWAObstacleVertex dwaobstacleVertex = new DWAObstacleVertex(this._obstaclesData.Count);
				dwaobstacleVertex.Point = vertices[i].AsVec2;
				dwaobstacleVertex.PointZ = vertices[i].z;
				if (i != 0)
				{
					dwaobstacleVertex.Previous = this._obstaclesData[this._obstaclesData.Count - 1];
					dwaobstacleVertex.Previous.Next = dwaobstacleVertex;
				}
				if (i == vertices.Count - 1)
				{
					dwaobstacleVertex.Next = this._obstaclesData[count];
					dwaobstacleVertex.Next.Previous = dwaobstacleVertex;
				}
				DWAObstacleVertex dwaobstacleVertex2 = dwaobstacleVertex;
				Vec2 vec = vertices[(i == vertices.Count - 1) ? 0 : (i + 1)].AsVec2 - vertices[i].AsVec2;
				dwaobstacleVertex2.Direction = vec.Normalized();
				if (vertices.Count == 2)
				{
					dwaobstacleVertex.IsConvex = true;
				}
				else
				{
					DWAObstacleVertex dwaobstacleVertex3 = dwaobstacleVertex;
					vec = vertices[(i == 0) ? (vertices.Count - 1) : (i - 1)].AsVec2;
					Vec2 asVec = vertices[i].AsVec2;
					Vec2 asVec2 = vertices[(i == vertices.Count - 1) ? 0 : (i + 1)].AsVec2;
					dwaobstacleVertex3.IsConvex = MBMath.GetSignedDistanceOfPointToLineSegment(ref vec, ref asVec, ref asVec2) >= 0f;
				}
				this._obstaclesData.Add(dwaobstacleVertex);
			}
			this._obstacleIndices.Add(count);
			return count;
		}

		// Token: 0x060015E1 RID: 5601 RVA: 0x00098D1C File Offset: 0x00096F1C
		public void Clear()
		{
			this._agentsData.Clear();
			this._obstaclesData.Clear();
			this._obstacleIndices.Clear();
			this._kdTree = new DWAKdTree(this);
			this._removedAgentIndices.Clear();
			this._currentAgentIndexToProcess = 0;
			this._agentsToProcessCount = 0;
			for (int i = 0; i < this._agentsToProcess.Length; i++)
			{
				this._agentsToProcess[i] = null;
			}
			for (int j = 0; j < this._processThreads.Length; j++)
			{
				this._processThreads[j].Clear();
			}
			this._isInitialized = false;
		}

		// Token: 0x060015E2 RID: 5602 RVA: 0x00098DB4 File Offset: 0x00096FB4
		public void Tick(float dt)
		{
			if (this._isInitialized)
			{
				this._kdTree.BuildAgentTree();
				this.ComputeAndUpdateAgentsToProcess(this._parity, ref this._currentAgentIndexToProcess, out this._agentsToProcessCount);
				if (this._agentsToProcessCount > 0)
				{
					this.ComputeAndForecastNeighbors(this._parity);
					foreach (DWAAgent dwaagent in this._agentsToProcess)
					{
						if (dwaagent != null)
						{
							dwaagent.InitializeThreads(in this._parameters, this._processThreads);
							dwaagent.ComputeTargetOcclusion();
							TWParallel.For(0, this._processThreads.Length, this.RunSampleThreadsAuxParallelPredicate, 16);
							int num;
							DWAThread dwathread;
							ValueTuple<float, float> valueTuple = dwaagent.SelectAction(this._processThreads, out num, out dwathread);
							float item = valueTuple.Item1;
							float item2 = valueTuple.Item2;
							dwaagent.Delegate.UpdateSelectedAction(item, item2);
						}
					}
				}
				this.ClearProcessThreads();
				this._parity += 1;
			}
		}

		// Token: 0x060015E3 RID: 5603 RVA: 0x00098E92 File Offset: 0x00097092
		public bool QueryVisibility(Vec2 point1, Vec2 point2, float radius)
		{
			return this._kdTree.QueryVisibility(in point1, in point2, radius);
		}

		// Token: 0x060015E4 RID: 5604 RVA: 0x00098EA4 File Offset: 0x000970A4
		private void RunSampleThreadsAuxParallel(int startInclusive, int endExclusive)
		{
			for (int i = startInclusive; i < endExclusive; i++)
			{
				this._processThreads[i].Run();
			}
		}

		// Token: 0x060015E5 RID: 5605 RVA: 0x00098ECA File Offset: 0x000970CA
		internal void AddObstacleVertex(DWAObstacleVertex newObstacle)
		{
			this._obstaclesData.Add(newObstacle);
		}

		// Token: 0x060015E6 RID: 5606 RVA: 0x00098ED8 File Offset: 0x000970D8
		internal void ComputeAgentNeighbors(DWAAgent agent, float rangeSq, ushort parity)
		{
			this._kdTree.ComputeAgentNeighbors(agent, rangeSq, parity);
		}

		// Token: 0x060015E7 RID: 5607 RVA: 0x00098EE8 File Offset: 0x000970E8
		internal void ComputeObstacleNeighbors(DWAAgent agent, float rangeSq)
		{
			this._kdTree.ComputeObstacleNeighbors(agent, rangeSq);
		}

		// Token: 0x060015E8 RID: 5608 RVA: 0x00098EF7 File Offset: 0x000970F7
		internal void Initialize()
		{
			this._kdTree.BuildObstacleTree();
			this._isInitialized = true;
		}

		// Token: 0x060015E9 RID: 5609 RVA: 0x00098F0C File Offset: 0x0009710C
		private void ComputeAndUpdateAgentsToProcess(ushort parity, ref int currentAgentIndexToProcess, out int agentsToProcessCount)
		{
			agentsToProcessCount = 0;
			if (this._agentsData.Count > 0)
			{
				int num = currentAgentIndexToProcess;
				do
				{
					DWAAgent dwaagent = this._agentsData[currentAgentIndexToProcess];
					if (dwaagent != null && dwaagent.Delegate.CanPlanTrajectory())
					{
						dwaagent.TryUpdateState(parity);
						if (!dwaagent.Delegate.HasArrivedAtTarget())
						{
							this._agentsToProcess[agentsToProcessCount] = dwaagent;
							agentsToProcessCount++;
						}
						else
						{
							dwaagent.Delegate.UpdateSelectedAction(0f, 0f);
						}
					}
					currentAgentIndexToProcess = (currentAgentIndexToProcess + 1) % this._agentsData.Count;
				}
				while (agentsToProcessCount < this._agentsToProcess.Length && currentAgentIndexToProcess != num);
			}
		}

		// Token: 0x060015EA RID: 5610 RVA: 0x00098FB0 File Offset: 0x000971B0
		private void ComputeAndForecastNeighbors(ushort parity)
		{
			for (int i = 0; i < this._agentsToProcessCount; i++)
			{
				this._agentsToProcess[i].ComputeNeighbors(parity);
				foreach (KeyValuePair<float, DWAAgent> keyValuePair in this._agentsToProcess[i].AgentNeighbors)
				{
					DWAAgent value = keyValuePair.Value;
					if (!value.IsForecast)
					{
						value.ForecastTrajectory(this._parameters.DeltaTime, this._parameters.NumTimeSamples);
					}
				}
			}
		}

		// Token: 0x060015EB RID: 5611 RVA: 0x00099050 File Offset: 0x00097250
		private void ClearProcessThreads()
		{
			for (int i = 0; i < this._processThreads.Length; i++)
			{
				this._processThreads[i].Clear();
			}
		}

		// Token: 0x060015EC RID: 5612 RVA: 0x00099080 File Offset: 0x00097280
		private void InsertRemovedIndex(int removedIndex)
		{
			int num = this._removedAgentIndices.BinarySearch(removedIndex, Comparer<int>.Create((int a, int b) => b.CompareTo(a)));
			if (num < 0)
			{
				num = ~num;
			}
			this._removedAgentIndices.Insert(num, removedIndex);
		}

		// Token: 0x04000B40 RID: 2880
		internal const int MaxObstacleVertexCount = 32;

		// Token: 0x04000B41 RID: 2881
		private MBList<DWAAgent> _agentsData;

		// Token: 0x04000B42 RID: 2882
		private MBList<DWAObstacleVertex> _obstaclesData;

		// Token: 0x04000B43 RID: 2883
		private DWAKdTree _kdTree;

		// Token: 0x04000B44 RID: 2884
		private MBList<int> _obstacleIndices;

		// Token: 0x04000B45 RID: 2885
		private MBList<int> _removedAgentIndices;

		// Token: 0x04000B46 RID: 2886
		private bool _isInitialized;

		// Token: 0x04000B47 RID: 2887
		private int _currentAgentIndexToProcess;

		// Token: 0x04000B48 RID: 2888
		private DWAAgent[] _agentsToProcess;

		// Token: 0x04000B49 RID: 2889
		private int _agentsToProcessCount;

		// Token: 0x04000B4A RID: 2890
		private DWAThread[] _processThreads;

		// Token: 0x04000B4B RID: 2891
		private DWASimulatorParameters _parameters;

		// Token: 0x04000B4C RID: 2892
		private ushort _parity;

		// Token: 0x04000B4D RID: 2893
		private readonly TWParallel.ParallelForAuxPredicate RunSampleThreadsAuxParallelPredicate;
	}
}
