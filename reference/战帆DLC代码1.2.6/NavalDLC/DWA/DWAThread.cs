using System;
using TaleWorlds.Library;

namespace NavalDLC.DWA
{
	// Token: 0x02000150 RID: 336
	public class DWAThread
	{
		// Token: 0x170003C8 RID: 968
		// (get) Token: 0x06001610 RID: 5648 RVA: 0x0009941A File Offset: 0x0009761A
		// (set) Token: 0x06001611 RID: 5649 RVA: 0x00099422 File Offset: 0x00097622
		public int Index { get; private set; }

		// Token: 0x170003C9 RID: 969
		// (get) Token: 0x06001612 RID: 5650 RVA: 0x0009942B File Offset: 0x0009762B
		// (set) Token: 0x06001613 RID: 5651 RVA: 0x00099433 File Offset: 0x00097633
		public float DV { get; private set; }

		// Token: 0x170003CA RID: 970
		// (get) Token: 0x06001614 RID: 5652 RVA: 0x0009943C File Offset: 0x0009763C
		// (set) Token: 0x06001615 RID: 5653 RVA: 0x00099444 File Offset: 0x00097644
		public float DOmega { get; private set; }

		// Token: 0x170003CB RID: 971
		// (get) Token: 0x06001616 RID: 5654 RVA: 0x0009944D File Offset: 0x0009764D
		// (set) Token: 0x06001617 RID: 5655 RVA: 0x00099455 File Offset: 0x00097655
		public DWAAgent Owner { get; private set; }

		// Token: 0x170003CC RID: 972
		// (get) Token: 0x06001618 RID: 5656 RVA: 0x0009945E File Offset: 0x0009765E
		// (set) Token: 0x06001619 RID: 5657 RVA: 0x00099466 File Offset: 0x00097666
		public float DT { get; private set; }

		// Token: 0x170003CD RID: 973
		// (get) Token: 0x0600161A RID: 5658 RVA: 0x0009946F File Offset: 0x0009766F
		// (set) Token: 0x0600161B RID: 5659 RVA: 0x00099477 File Offset: 0x00097677
		public int TimeSamples { get; private set; }

		// Token: 0x170003CE RID: 974
		// (get) Token: 0x0600161C RID: 5660 RVA: 0x00099480 File Offset: 0x00097680
		// (set) Token: 0x0600161D RID: 5661 RVA: 0x00099488 File Offset: 0x00097688
		public float Cost { get; private set; }

		// Token: 0x170003CF RID: 975
		// (get) Token: 0x0600161E RID: 5662 RVA: 0x00099491 File Offset: 0x00097691
		// (set) Token: 0x0600161F RID: 5663 RVA: 0x00099499 File Offset: 0x00097699
		public bool HasCollision { get; private set; }

		// Token: 0x170003D0 RID: 976
		// (get) Token: 0x06001620 RID: 5664 RVA: 0x000994A2 File Offset: 0x000976A2
		// (set) Token: 0x06001621 RID: 5665 RVA: 0x000994AA File Offset: 0x000976AA
		public int CollisionSampleIndex { get; private set; }

		// Token: 0x170003D1 RID: 977
		// (get) Token: 0x06001622 RID: 5666 RVA: 0x000994B3 File Offset: 0x000976B3
		// (set) Token: 0x06001623 RID: 5667 RVA: 0x000994BB File Offset: 0x000976BB
		public DWAAgent CollidedAgent { get; private set; }

		// Token: 0x170003D2 RID: 978
		// (get) Token: 0x06001624 RID: 5668 RVA: 0x000994C4 File Offset: 0x000976C4
		// (set) Token: 0x06001625 RID: 5669 RVA: 0x000994CC File Offset: 0x000976CC
		public DWAObstacleVertex CollidedObstacle { get; private set; }

		// Token: 0x170003D3 RID: 979
		// (get) Token: 0x06001626 RID: 5670 RVA: 0x000994D5 File Offset: 0x000976D5
		// (set) Token: 0x06001627 RID: 5671 RVA: 0x000994DD File Offset: 0x000976DD
		public bool IsFinished { get; private set; }

		// Token: 0x06001628 RID: 5672 RVA: 0x000994E8 File Offset: 0x000976E8
		public DWAThread(int index)
		{
			this.Index = index;
			this.Owner = null;
			this.DV = 0f;
			this.DOmega = 0f;
			this.DT = 0f;
			this.TimeSamples = 0;
			this.ClearAux();
		}

		// Token: 0x06001629 RID: 5673 RVA: 0x00099544 File Offset: 0x00097744
		public void Initialize(DWAAgent owner, float dV, float dOmega, float dt, int timeSamples)
		{
			this.Owner = owner;
			this.DV = dV;
			this.DOmega = dOmega;
			this.DT = dt;
			this.TimeSamples = timeSamples;
			this.ClearAux();
		}

		// Token: 0x0600162A RID: 5674 RVA: 0x00099571 File Offset: 0x00097771
		internal void Clear()
		{
			this.Owner = null;
			this.DV = 0f;
			this.DOmega = 0f;
			this.DT = 0f;
			this.TimeSamples = 0;
			this.ClearAux();
		}

		// Token: 0x0600162B RID: 5675 RVA: 0x000995A8 File Offset: 0x000977A8
		public unsafe void Run()
		{
			DWAAgentState dwaagentState = *this.Owner.State;
			dwaagentState.LinearAcceleration = this.DV;
			dwaagentState.AngularAcceleration = this.DOmega;
			DWAAgentState dwaagentState2 = default(DWAAgentState);
			bool flag = false;
			DWAAgent dwaagent = null;
			DWAObstacleVertex dwaobstacleVertex = null;
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			for (int i = 0; i < this.TimeSamples; i++)
			{
				this.Owner.IntegrateState(in dwaagentState, this.DT, ref dwaagentState2);
				bool flag2;
				DWAAgent dwaagent2;
				DWAObstacleVertex dwaobstacleVertex2;
				float num5;
				float num6;
				float num7;
				this.Owner.EvaluateState(in dwaagentState2, i, out flag2, out dwaagent2, out dwaobstacleVertex2, out num5, out num6, out num7, this._tempObstaclePoly);
				if (flag2)
				{
					num4 += this.DT;
					num3 = MathF.Max(num3, num7);
					if (!flag)
					{
						flag = true;
						dwaagent = dwaagent2;
						dwaobstacleVertex = dwaobstacleVertex2;
					}
				}
				num2 += num6;
				num += num5;
				dwaagentState = dwaagentState2;
			}
			float num8 = 0.5f;
			float num9 = 1.5f;
			float num10 = (float)this.TimeSamples * this.DT;
			float num11 = num3;
			DWAAgentState dwaagentState3 = *this.Owner.State;
			float num12 = MathF.Clamp(num11 / dwaagentState3.MaxExtent, 0f, 1f);
			float num13 = MathF.Clamp(num4 / num10, 0f, 1f);
			float num14 = num12 * num9;
			float num15 = num13 * num8;
			float num16 = (1f + num15 + num14 * num14) * num2;
			this.Cost = (num + num16) / (float)this.TimeSamples;
			this.HasCollision = flag;
			if (flag && dwaagent != null)
			{
				this.CollidedAgent = dwaagent;
			}
			if (flag && dwaobstacleVertex != null)
			{
				this.CollidedObstacle = dwaobstacleVertex;
			}
			this.IsFinished = true;
		}

		// Token: 0x0600162C RID: 5676 RVA: 0x00099747 File Offset: 0x00097947
		private void ClearAux()
		{
			this.IsFinished = false;
			this.Cost = 0f;
			this.HasCollision = false;
			this.CollisionSampleIndex = -1;
			this.CollidedAgent = null;
			this.CollidedObstacle = null;
		}

		// Token: 0x04000B6E RID: 2926
		private Vec2[] _tempObstaclePoly = new Vec2[32];
	}
}
