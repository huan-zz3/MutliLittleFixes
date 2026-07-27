using System;

namespace NavalDLC.DWA
{
	// Token: 0x0200014F RID: 335
	public struct DWASimulatorParameters
	{
		// Token: 0x170003BB RID: 955
		// (get) Token: 0x060015ED RID: 5613 RVA: 0x000990D2 File Offset: 0x000972D2
		// (set) Token: 0x060015EE RID: 5614 RVA: 0x000990DA File Offset: 0x000972DA
		public int NumTimeSamples { get; private set; }

		// Token: 0x170003BC RID: 956
		// (get) Token: 0x060015EF RID: 5615 RVA: 0x000990E3 File Offset: 0x000972E3
		// (set) Token: 0x060015F0 RID: 5616 RVA: 0x000990EB File Offset: 0x000972EB
		public int SamplesPerSecond { get; private set; }

		// Token: 0x170003BD RID: 957
		// (get) Token: 0x060015F1 RID: 5617 RVA: 0x000990F4 File Offset: 0x000972F4
		// (set) Token: 0x060015F2 RID: 5618 RVA: 0x000990FC File Offset: 0x000972FC
		public int AgentsToProcessPerTick { get; private set; }

		// Token: 0x170003BE RID: 958
		// (get) Token: 0x060015F3 RID: 5619 RVA: 0x00099105 File Offset: 0x00097305
		// (set) Token: 0x060015F4 RID: 5620 RVA: 0x0009910D File Offset: 0x0009730D
		public int LinearAccelerationResolution { get; private set; }

		// Token: 0x170003BF RID: 959
		// (get) Token: 0x060015F5 RID: 5621 RVA: 0x00099116 File Offset: 0x00097316
		// (set) Token: 0x060015F6 RID: 5622 RVA: 0x0009911E File Offset: 0x0009731E
		public int AngularAccelerationResolution { get; private set; }

		// Token: 0x170003C0 RID: 960
		// (get) Token: 0x060015F7 RID: 5623 RVA: 0x00099127 File Offset: 0x00097327
		// (set) Token: 0x060015F8 RID: 5624 RVA: 0x0009912F File Offset: 0x0009732F
		public int MaxAgentNeighbors { get; private set; }

		// Token: 0x170003C1 RID: 961
		// (get) Token: 0x060015F9 RID: 5625 RVA: 0x00099138 File Offset: 0x00097338
		// (set) Token: 0x060015FA RID: 5626 RVA: 0x00099140 File Offset: 0x00097340
		public int MaxObstacleNeighbors { get; private set; }

		// Token: 0x170003C2 RID: 962
		// (get) Token: 0x060015FB RID: 5627 RVA: 0x00099149 File Offset: 0x00097349
		// (set) Token: 0x060015FC RID: 5628 RVA: 0x00099151 File Offset: 0x00097351
		public bool IgnoreZeroAction { get; private set; }

		// Token: 0x170003C3 RID: 963
		// (get) Token: 0x060015FD RID: 5629 RVA: 0x0009915A File Offset: 0x0009735A
		public int NumLinearAccelerationSamples
		{
			get
			{
				return this._numLinearAccelerationSamples;
			}
		}

		// Token: 0x170003C4 RID: 964
		// (get) Token: 0x060015FE RID: 5630 RVA: 0x00099162 File Offset: 0x00097362
		public int NumAngularAccelerationSamples
		{
			get
			{
				return this._numAngularAccelerationSamples;
			}
		}

		// Token: 0x170003C5 RID: 965
		// (get) Token: 0x060015FF RID: 5631 RVA: 0x0009916A File Offset: 0x0009736A
		public int TotalNumAccelerationSamples
		{
			get
			{
				return this._totalNumAccelerationSamples;
			}
		}

		// Token: 0x170003C6 RID: 966
		// (get) Token: 0x06001600 RID: 5632 RVA: 0x00099172 File Offset: 0x00097372
		public float TimeHorizon
		{
			get
			{
				return (float)this.NumTimeSamples * this.DeltaTime;
			}
		}

		// Token: 0x170003C7 RID: 967
		// (get) Token: 0x06001601 RID: 5633 RVA: 0x00099182 File Offset: 0x00097382
		public float DeltaTime
		{
			get
			{
				return 1f / (float)this.SamplesPerSecond;
			}
		}

		// Token: 0x06001602 RID: 5634 RVA: 0x00099194 File Offset: 0x00097394
		private DWASimulatorParameters(int numTimeSamples, int samplesPerSecond, int agentsToProcessPerTick, int linearAccelerationResolution, int angularAccelerationResolution, bool ignoreZeroAction, int maxAgentNeighbors, int maxObstacleNeighbors, int numLinearAccelerationSamples, int numAngularAccelerationSamples, int totalNumAccelerationSamples, bool requiresUpdate)
		{
			this.NumTimeSamples = numTimeSamples;
			this.SamplesPerSecond = samplesPerSecond;
			this.AgentsToProcessPerTick = agentsToProcessPerTick;
			this.LinearAccelerationResolution = linearAccelerationResolution;
			this.AngularAccelerationResolution = angularAccelerationResolution;
			this.IgnoreZeroAction = ignoreZeroAction;
			this.MaxAgentNeighbors = maxAgentNeighbors;
			this.MaxObstacleNeighbors = maxObstacleNeighbors;
			this._numLinearAccelerationSamples = numLinearAccelerationSamples;
			this._numAngularAccelerationSamples = numAngularAccelerationSamples;
			this._totalNumAccelerationSamples = totalNumAccelerationSamples;
			this._requiresUpdate = requiresUpdate;
		}

		// Token: 0x06001603 RID: 5635 RVA: 0x000991FE File Offset: 0x000973FE
		public bool CheckRequiresUpdate(bool reset)
		{
			bool requiresUpdate = this._requiresUpdate;
			if (reset)
			{
				this._requiresUpdate = false;
			}
			return requiresUpdate;
		}

		// Token: 0x06001604 RID: 5636 RVA: 0x00099210 File Offset: 0x00097410
		public void SetNumTimeSamples(int numTimeSamples)
		{
			if (this.NumTimeSamples != numTimeSamples)
			{
				this.NumTimeSamples = numTimeSamples;
				this.RecomputeDerivedParameters();
				this._requiresUpdate = true;
			}
		}

		// Token: 0x06001605 RID: 5637 RVA: 0x0009922F File Offset: 0x0009742F
		public void SetSamplesPerSecond(int samplesPerSecond)
		{
			if (this.SamplesPerSecond != samplesPerSecond)
			{
				this.SamplesPerSecond = samplesPerSecond;
				this._requiresUpdate = true;
			}
		}

		// Token: 0x06001606 RID: 5638 RVA: 0x00099248 File Offset: 0x00097448
		public void SetAgentsToProcessPerTick(int agentsToProcessPerTick)
		{
			if (this.AgentsToProcessPerTick != agentsToProcessPerTick)
			{
				this.AgentsToProcessPerTick = agentsToProcessPerTick;
				this._requiresUpdate = true;
			}
		}

		// Token: 0x06001607 RID: 5639 RVA: 0x00099261 File Offset: 0x00097461
		public void SetLinearAccelerationResolution(int linearAccelerationResolution)
		{
			if (this.LinearAccelerationResolution != linearAccelerationResolution)
			{
				this.LinearAccelerationResolution = linearAccelerationResolution;
				this.RecomputeDerivedParameters();
				this._requiresUpdate = true;
			}
		}

		// Token: 0x06001608 RID: 5640 RVA: 0x00099280 File Offset: 0x00097480
		public void SetAngularAccelerationResolution(int angularAccelerationResolution)
		{
			if (this.AngularAccelerationResolution != angularAccelerationResolution)
			{
				this.AngularAccelerationResolution = angularAccelerationResolution;
				this.RecomputeDerivedParameters();
				this._requiresUpdate = true;
			}
		}

		// Token: 0x06001609 RID: 5641 RVA: 0x0009929F File Offset: 0x0009749F
		public void SetIgnoreZeroAction(bool ignoreZeroAction)
		{
			if (this.IgnoreZeroAction != ignoreZeroAction)
			{
				this.IgnoreZeroAction = ignoreZeroAction;
				this.RecomputeDerivedParameters();
				this._requiresUpdate = true;
			}
		}

		// Token: 0x0600160A RID: 5642 RVA: 0x000992BE File Offset: 0x000974BE
		public void SetMaxAgentNeighbors(int maxAgentNeighbors)
		{
			if (this.MaxAgentNeighbors != maxAgentNeighbors)
			{
				this.MaxAgentNeighbors = maxAgentNeighbors;
				this._requiresUpdate = true;
			}
		}

		// Token: 0x0600160B RID: 5643 RVA: 0x000992D7 File Offset: 0x000974D7
		public void SetMaxObstacleNeighbors(int maxObstacleNeighbors)
		{
			if (this.MaxObstacleNeighbors != maxObstacleNeighbors)
			{
				this.MaxObstacleNeighbors = maxObstacleNeighbors;
				this._requiresUpdate = true;
			}
		}

		// Token: 0x0600160C RID: 5644 RVA: 0x000992F0 File Offset: 0x000974F0
		public void CopyFrom(in DWASimulatorParameters otherParameters)
		{
			DWASimulatorParameters dwasimulatorParameters = otherParameters;
			this.SetNumTimeSamples(dwasimulatorParameters.NumTimeSamples);
			dwasimulatorParameters = otherParameters;
			this.SetSamplesPerSecond(dwasimulatorParameters.SamplesPerSecond);
			dwasimulatorParameters = otherParameters;
			this.SetAgentsToProcessPerTick(dwasimulatorParameters.AgentsToProcessPerTick);
			dwasimulatorParameters = otherParameters;
			this.SetLinearAccelerationResolution(dwasimulatorParameters.LinearAccelerationResolution);
			dwasimulatorParameters = otherParameters;
			this.SetAngularAccelerationResolution(dwasimulatorParameters.AngularAccelerationResolution);
			dwasimulatorParameters = otherParameters;
			this.SetIgnoreZeroAction(dwasimulatorParameters.IgnoreZeroAction);
			dwasimulatorParameters = otherParameters;
			this.SetMaxAgentNeighbors(dwasimulatorParameters.MaxAgentNeighbors);
			dwasimulatorParameters = otherParameters;
			this.SetMaxObstacleNeighbors(dwasimulatorParameters.MaxObstacleNeighbors);
		}

		// Token: 0x0600160D RID: 5645 RVA: 0x0009939D File Offset: 0x0009759D
		private void RecomputeDerivedParameters()
		{
			DWASimulatorParameters.ComputeDerivedParameters(this.LinearAccelerationResolution, this.AngularAccelerationResolution, this.IgnoreZeroAction, out this._numLinearAccelerationSamples, out this._numAngularAccelerationSamples, out this._totalNumAccelerationSamples);
		}

		// Token: 0x0600160E RID: 5646 RVA: 0x000993C8 File Offset: 0x000975C8
		public static DWASimulatorParameters Create()
		{
			int num;
			int num2;
			int num3;
			DWASimulatorParameters.ComputeDerivedParameters(3, 3, true, out num, out num2, out num3);
			return new DWASimulatorParameters(12, 4, 1, 3, 3, true, 3, 3, num, num2, num3, false);
		}

		// Token: 0x0600160F RID: 5647 RVA: 0x000993F5 File Offset: 0x000975F5
		public static void ComputeDerivedParameters(int linearAccelerationResolution, int angularAccelerationResolution, bool ignoreZeroAction, out int numLinearAccelerationSamples, out int numAngularAccelerationSamples, out int numTotalAccelerationSamples)
		{
			numLinearAccelerationSamples = 2 * linearAccelerationResolution + 1;
			numAngularAccelerationSamples = 2 * angularAccelerationResolution + 1;
			numTotalAccelerationSamples = numLinearAccelerationSamples * numAngularAccelerationSamples;
			if (ignoreZeroAction)
			{
				numTotalAccelerationSamples--;
			}
		}

		// Token: 0x04000B4E RID: 2894
		public const int DefaultMaxNumTimeSamples = 12;

		// Token: 0x04000B4F RID: 2895
		public const int DefaultSamplesPerSecond = 4;

		// Token: 0x04000B50 RID: 2896
		public const int DefaultAgentsToProcessPerTick = 1;

		// Token: 0x04000B51 RID: 2897
		public const int DefaultLinearAccelerationResolution = 3;

		// Token: 0x04000B52 RID: 2898
		public const int DefaultAngularAccelerationResolution = 3;

		// Token: 0x04000B53 RID: 2899
		public const bool DefaultIgnoreZeroAction = true;

		// Token: 0x04000B54 RID: 2900
		public const int DefaultMaxAgentNeighbors = 3;

		// Token: 0x04000B55 RID: 2901
		public const int DefaultMaxObstacleNeighbors = 3;

		// Token: 0x04000B5E RID: 2910
		private int _numLinearAccelerationSamples;

		// Token: 0x04000B5F RID: 2911
		private int _numAngularAccelerationSamples;

		// Token: 0x04000B60 RID: 2912
		private int _totalNumAccelerationSamples;

		// Token: 0x04000B61 RID: 2913
		private bool _requiresUpdate;
	}
}
