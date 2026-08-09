using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace NavalDLC.DWA
{
	// Token: 0x0200014B RID: 331
	internal class DWAKdTree
	{
		// Token: 0x060015AF RID: 5551 RVA: 0x0009787A File Offset: 0x00095A7A
		internal DWAKdTree(DWASimulator simulator)
		{
			this._simulator = simulator;
		}

		// Token: 0x060015B0 RID: 5552 RVA: 0x0009788C File Offset: 0x00095A8C
		internal void BuildAgentTree()
		{
			if (this._agents == null || this._agents.Length != this._simulator.NumAgents)
			{
				this._agents = new DWAAgent[this._simulator.NumAgents];
				int num = 0;
				for (int i = 0; i < this._simulator.AgentsIncludingRemoved.Count; i++)
				{
					DWAAgent dwaagent = this._simulator.AgentsIncludingRemoved[i];
					if (dwaagent != null)
					{
						this._agents[num] = dwaagent;
						num++;
					}
				}
				this._agentTree = new DWAAgentTreeNode[2 * this._agents.Length];
				for (int j = 0; j < this._agentTree.Length; j++)
				{
					this._agentTree[j] = default(DWAAgentTreeNode);
				}
			}
			if (this._agents.Length != 0)
			{
				this.BuildAgentTreeRecursive(0, this._agents.Length, 0);
			}
		}

		// Token: 0x060015B1 RID: 5553 RVA: 0x00097964 File Offset: 0x00095B64
		internal void BuildObstacleTree()
		{
			this._obstacleTree = new DWAObstacleTreeNode();
			IList<DWAObstacleVertex> list = new List<DWAObstacleVertex>(this._simulator.NumObstacles);
			for (int i = 0; i < this._simulator.NumObstacles; i++)
			{
				list.Add(this._simulator.Obstacles[i]);
			}
			this._obstacleTree = this.BuildObstacleTreeRecursive(list);
		}

		// Token: 0x060015B2 RID: 5554 RVA: 0x000979C7 File Offset: 0x00095BC7
		internal void ComputeAgentNeighbors(DWAAgent agent, float rangeSq, ushort parity)
		{
			this.QueryAgentTreeRecursive(agent, ref rangeSq, 0, parity);
		}

		// Token: 0x060015B3 RID: 5555 RVA: 0x000979D4 File Offset: 0x00095BD4
		internal void ComputeObstacleNeighbors(DWAAgent agent, float rangeSq)
		{
			this.QueryObstacleTreeRecursive(agent, ref rangeSq, this._obstacleTree);
		}

		// Token: 0x060015B4 RID: 5556 RVA: 0x000979E5 File Offset: 0x00095BE5
		internal bool QueryVisibility(in Vec2 point1, in Vec2 point2, float radius)
		{
			return this.QueryVisibilityRecursive(in point1, in point2, radius, this._obstacleTree);
		}

		// Token: 0x060015B5 RID: 5557 RVA: 0x000979F8 File Offset: 0x00095BF8
		private void BuildAgentTreeRecursive(int begin, int end, int node)
		{
			this._agentTree[node].Begin = begin;
			this._agentTree[node].End = end;
			this._agentTree[node].MinX = (this._agentTree[node].MaxX = this._agents[begin].State.Position.x);
			this._agentTree[node].MinY = (this._agentTree[node].MaxY = this._agents[begin].State.Position.y);
			for (int i = begin + 1; i < end; i++)
			{
				this._agentTree[node].MaxX = Math.Max(this._agentTree[node].MaxX, this._agents[i].State.Position.x);
				this._agentTree[node].MinX = Math.Min(this._agentTree[node].MinX, this._agents[i].State.Position.x);
				this._agentTree[node].MaxY = Math.Max(this._agentTree[node].MaxY, this._agents[i].State.Position.y);
				this._agentTree[node].MinY = Math.Min(this._agentTree[node].MinY, this._agents[i].State.Position.y);
			}
			if (end - begin > 10)
			{
				bool flag = this._agentTree[node].MaxX - this._agentTree[node].MinX > this._agentTree[node].MaxY - this._agentTree[node].MinY;
				float num = 0.5f * (flag ? (this._agentTree[node].MaxX + this._agentTree[node].MinX) : (this._agentTree[node].MaxY + this._agentTree[node].MinY));
				int j = begin;
				int num2 = end;
				while (j < num2)
				{
					while (j < num2)
					{
						if ((flag ? this._agents[j].State.Position.x : this._agents[j].State.Position.y) >= num)
						{
							break;
						}
						j++;
					}
					while (num2 > j && (flag ? this._agents[num2 - 1].State.Position.x : this._agents[num2 - 1].State.Position.y) >= num)
					{
						num2--;
					}
					if (j < num2)
					{
						DWAAgent dwaagent = this._agents[j];
						this._agents[j] = this._agents[num2 - 1];
						this._agents[num2 - 1] = dwaagent;
						j++;
						num2--;
					}
				}
				int num3 = j - begin;
				if (num3 == 0)
				{
					num3++;
					j++;
					num2++;
				}
				this._agentTree[node].Left = node + 1;
				this._agentTree[node].Right = node + 2 * num3;
				this.BuildAgentTreeRecursive(begin, j, this._agentTree[node].Left);
				this.BuildAgentTreeRecursive(j, end, this._agentTree[node].Right);
			}
		}

		// Token: 0x060015B6 RID: 5558 RVA: 0x00097DB0 File Offset: 0x00095FB0
		private DWAObstacleTreeNode BuildObstacleTreeRecursive(IList<DWAObstacleVertex> obstacles)
		{
			if (obstacles.Count == 0)
			{
				return null;
			}
			DWAObstacleTreeNode dwaobstacleTreeNode = new DWAObstacleTreeNode();
			int num = 0;
			int num2 = obstacles.Count;
			int num3 = obstacles.Count;
			for (int i = 0; i < obstacles.Count; i++)
			{
				int num4 = 0;
				int num5 = 0;
				DWAObstacleVertex dwaobstacleVertex = obstacles[i];
				DWAObstacleVertex next = dwaobstacleVertex.Next;
				for (int j = 0; j < obstacles.Count; j++)
				{
					if (i != j)
					{
						DWAObstacleVertex dwaobstacleVertex2 = obstacles[j];
						DWAObstacleVertex next2 = dwaobstacleVertex2.Next;
						Vec2 vec = dwaobstacleVertex.Point;
						Vec2 vec2 = next.Point;
						Vec2 vec3 = dwaobstacleVertex2.Point;
						float signedDistanceOfPointToLineSegment = MBMath.GetSignedDistanceOfPointToLineSegment(ref vec, ref vec2, ref vec3);
						vec = dwaobstacleVertex.Point;
						vec2 = next.Point;
						vec3 = next2.Point;
						float signedDistanceOfPointToLineSegment2 = MBMath.GetSignedDistanceOfPointToLineSegment(ref vec, ref vec2, ref vec3);
						if (signedDistanceOfPointToLineSegment >= -1E-05f && signedDistanceOfPointToLineSegment2 >= -1E-05f)
						{
							num4++;
						}
						else if (signedDistanceOfPointToLineSegment <= 1E-05f && signedDistanceOfPointToLineSegment2 <= 1E-05f)
						{
							num5++;
						}
						else
						{
							num4++;
							num5++;
						}
						if (new DWAFloatPair((float)Math.Max(num4, num5), (float)Math.Min(num4, num5)) >= new DWAFloatPair((float)Math.Max(num2, num3), (float)Math.Min(num2, num3)))
						{
							break;
						}
					}
				}
				if (new DWAFloatPair((float)Math.Max(num4, num5), (float)Math.Min(num4, num5)) < new DWAFloatPair((float)Math.Max(num2, num3), (float)Math.Min(num2, num3)))
				{
					num2 = num4;
					num3 = num5;
					num = i;
				}
			}
			IList<DWAObstacleVertex> list = new List<DWAObstacleVertex>(num2);
			for (int k = 0; k < num2; k++)
			{
				list.Add(null);
			}
			IList<DWAObstacleVertex> list2 = new List<DWAObstacleVertex>(num3);
			for (int l = 0; l < num3; l++)
			{
				list2.Add(null);
			}
			int num6 = 0;
			int num7 = 0;
			int num8 = num;
			DWAObstacleVertex dwaobstacleVertex3 = obstacles[num8];
			DWAObstacleVertex next3 = dwaobstacleVertex3.Next;
			for (int m = 0; m < obstacles.Count; m++)
			{
				if (num8 != m)
				{
					DWAObstacleVertex dwaobstacleVertex4 = obstacles[m];
					DWAObstacleVertex next4 = dwaobstacleVertex4.Next;
					Vec2 vec = dwaobstacleVertex3.Point;
					Vec2 vec2 = next3.Point;
					Vec2 vec3 = dwaobstacleVertex4.Point;
					float signedDistanceOfPointToLineSegment3 = MBMath.GetSignedDistanceOfPointToLineSegment(ref vec, ref vec2, ref vec3);
					vec = dwaobstacleVertex3.Point;
					vec2 = next3.Point;
					vec3 = next4.Point;
					float signedDistanceOfPointToLineSegment4 = MBMath.GetSignedDistanceOfPointToLineSegment(ref vec, ref vec2, ref vec3);
					if (signedDistanceOfPointToLineSegment3 >= -1E-05f && signedDistanceOfPointToLineSegment4 >= -1E-05f)
					{
						list[num6++] = obstacles[m];
					}
					else if (signedDistanceOfPointToLineSegment3 <= 1E-05f && signedDistanceOfPointToLineSegment4 <= 1E-05f)
					{
						list2[num7++] = obstacles[m];
					}
					else
					{
						vec = next3.Point - dwaobstacleVertex3.Point;
						vec2 = dwaobstacleVertex4.Point - dwaobstacleVertex3.Point;
						float num9 = Vec2.Determinant(ref vec, ref vec2);
						vec3 = next3.Point - dwaobstacleVertex3.Point;
						Vec2 vec4 = dwaobstacleVertex4.Point - next4.Point;
						float num10 = num9 / Vec2.Determinant(ref vec3, ref vec4);
						Vec2 vec5 = dwaobstacleVertex4.Point + num10 * (next4.Point - dwaobstacleVertex4.Point);
						float num11 = dwaobstacleVertex4.PointZ + num10 * (next4.PointZ - dwaobstacleVertex4.PointZ);
						DWAObstacleVertex dwaobstacleVertex5 = new DWAObstacleVertex(this._simulator.NumObstacles);
						dwaobstacleVertex5.Point = vec5;
						dwaobstacleVertex5.PointZ = num11;
						dwaobstacleVertex5.Previous = dwaobstacleVertex4;
						dwaobstacleVertex5.Next = next4;
						dwaobstacleVertex5.IsConvex = true;
						dwaobstacleVertex5.Direction = dwaobstacleVertex4.Direction;
						this._simulator.AddObstacleVertex(dwaobstacleVertex5);
						dwaobstacleVertex4.Next = dwaobstacleVertex5;
						next4.Previous = dwaobstacleVertex5;
						if (signedDistanceOfPointToLineSegment3 > 0f)
						{
							list[num6++] = dwaobstacleVertex4;
							list2[num7++] = dwaobstacleVertex5;
						}
						else
						{
							list2[num7++] = dwaobstacleVertex4;
							list[num6++] = dwaobstacleVertex5;
						}
					}
				}
			}
			dwaobstacleTreeNode.Obstacle = dwaobstacleVertex3;
			dwaobstacleTreeNode.Left = this.BuildObstacleTreeRecursive(list);
			dwaobstacleTreeNode.Right = this.BuildObstacleTreeRecursive(list2);
			return dwaobstacleTreeNode;
		}

		// Token: 0x060015B7 RID: 5559 RVA: 0x00098210 File Offset: 0x00096410
		private void QueryAgentTreeRecursive(DWAAgent agent, ref float rangeSq, int node, ushort parity)
		{
			if (this._agentTree[node].End - this._agentTree[node].Begin <= 10)
			{
				for (int i = this._agentTree[node].Begin; i < this._agentTree[node].End; i++)
				{
					DWAAgent dwaagent = this._agents[i];
					if (agent.Id != dwaagent.Id && agent.Delegate.IsAgentEligibleNeighbor(dwaagent.Id, dwaagent.Delegate))
					{
						agent.InsertAgentNeighbor(dwaagent, ref rangeSq, parity);
					}
				}
				return;
			}
			Vec2 position = agent.State.Position;
			int left = this._agentTree[node].Left;
			DWAAgentTreeNode dwaagentTreeNode = this._agentTree[left];
			float num = Math.Max(0f, dwaagentTreeNode.MinX - position.x);
			float num2 = Math.Max(0f, position.x - dwaagentTreeNode.MaxX);
			float num3 = Math.Max(0f, dwaagentTreeNode.MinY - position.y);
			float num4 = Math.Max(0f, position.y - dwaagentTreeNode.MaxY);
			float num5 = num * num + num2 * num2 + num3 * num3 + num4 * num4;
			int right = this._agentTree[node].Right;
			DWAAgentTreeNode dwaagentTreeNode2 = this._agentTree[right];
			float num6 = Math.Max(0f, dwaagentTreeNode2.MinX - position.x);
			float num7 = Math.Max(0f, position.x - dwaagentTreeNode2.MaxX);
			float num8 = Math.Max(0f, dwaagentTreeNode2.MinY - position.y);
			float num9 = Math.Max(0f, position.y - dwaagentTreeNode2.MaxY);
			float num10 = num6 * num6 + num7 * num7 + num8 * num8 + num9 * num9;
			if (num5 < num10)
			{
				if (num5 < rangeSq)
				{
					this.QueryAgentTreeRecursive(agent, ref rangeSq, left, parity);
					if (num10 < rangeSq)
					{
						this.QueryAgentTreeRecursive(agent, ref rangeSq, right, parity);
						return;
					}
				}
			}
			else if (num10 < rangeSq)
			{
				this.QueryAgentTreeRecursive(agent, ref rangeSq, right, parity);
				if (num5 < rangeSq)
				{
					this.QueryAgentTreeRecursive(agent, ref rangeSq, left, parity);
				}
			}
		}

		// Token: 0x060015B8 RID: 5560 RVA: 0x00098440 File Offset: 0x00096640
		private void QueryObstacleTreeRecursive(DWAAgent agent, ref float rangeSq, DWAObstacleTreeNode node)
		{
			if (node != null)
			{
				DWAObstacleVertex obstacle = node.Obstacle;
				DWAObstacleVertex next = obstacle.Next;
				Vec2 vec = obstacle.Point;
				Vec2 point = next.Point;
				float signedDistanceOfPointToLineSegment = MBMath.GetSignedDistanceOfPointToLineSegment(ref vec, ref point, ref agent.State.Position);
				this.QueryObstacleTreeRecursive(agent, ref rangeSq, (signedDistanceOfPointToLineSegment >= 0f) ? node.Left : node.Right);
				float num = signedDistanceOfPointToLineSegment * signedDistanceOfPointToLineSegment;
				vec = next.Point - obstacle.Point;
				if (num / vec.LengthSquared < rangeSq)
				{
					if (signedDistanceOfPointToLineSegment < 0f && agent.Delegate.IsObstacleSegmentEligibleNeighbor(obstacle, next))
					{
						agent.InsertObstacleNeighbor(node.Obstacle, ref rangeSq);
					}
					this.QueryObstacleTreeRecursive(agent, ref rangeSq, (signedDistanceOfPointToLineSegment >= 0f) ? node.Right : node.Left);
				}
			}
		}

		// Token: 0x060015B9 RID: 5561 RVA: 0x00098508 File Offset: 0x00096708
		private bool QueryVisibilityRecursive(in Vec2 q1, in Vec2 q2, float radius, DWAObstacleTreeNode node)
		{
			if (node == null)
			{
				return true;
			}
			DWAObstacleVertex obstacle = node.Obstacle;
			DWAObstacleVertex next = obstacle.Next;
			Vec2 vec = obstacle.Point;
			Vec2 vec2 = next.Point;
			float signedDistanceOfPointToLineSegment = MBMath.GetSignedDistanceOfPointToLineSegment(ref vec, ref vec2, ref q1);
			vec = obstacle.Point;
			vec2 = next.Point;
			float signedDistanceOfPointToLineSegment2 = MBMath.GetSignedDistanceOfPointToLineSegment(ref vec, ref vec2, ref q2);
			float num = 1f;
			vec = next.Point - obstacle.Point;
			float num2 = num / vec.LengthSquared;
			float num3 = signedDistanceOfPointToLineSegment * signedDistanceOfPointToLineSegment;
			float num4 = signedDistanceOfPointToLineSegment2 * signedDistanceOfPointToLineSegment2;
			float num5 = radius * radius;
			if (signedDistanceOfPointToLineSegment >= 0f && signedDistanceOfPointToLineSegment2 >= 0f)
			{
				return this.QueryVisibilityRecursive(in q1, in q2, radius, node.Left) && ((num3 * num2 >= num5 && num4 * num2 >= num5) || this.QueryVisibilityRecursive(in q1, in q2, radius, node.Right));
			}
			if (signedDistanceOfPointToLineSegment <= 0f && signedDistanceOfPointToLineSegment2 <= 0f)
			{
				return this.QueryVisibilityRecursive(in q1, in q2, radius, node.Right) && ((num3 * num2 >= num5 && num4 * num2 >= num5) || this.QueryVisibilityRecursive(in q1, in q2, radius, node.Left));
			}
			if (signedDistanceOfPointToLineSegment >= 0f && signedDistanceOfPointToLineSegment2 <= 0f)
			{
				return this.QueryVisibilityRecursive(in q1, in q2, radius, node.Left) && this.QueryVisibilityRecursive(in q1, in q2, radius, node.Right);
			}
			vec = obstacle.Point;
			float signedDistanceOfPointToLineSegment3 = MBMath.GetSignedDistanceOfPointToLineSegment(ref q1, ref q2, ref vec);
			vec = next.Point;
			float signedDistanceOfPointToLineSegment4 = MBMath.GetSignedDistanceOfPointToLineSegment(ref q1, ref q2, ref vec);
			float num6 = 1f;
			vec = q2 - q1;
			float num7 = num6 / vec.LengthSquared;
			float num8 = signedDistanceOfPointToLineSegment3 * signedDistanceOfPointToLineSegment3;
			float num9 = signedDistanceOfPointToLineSegment4 * signedDistanceOfPointToLineSegment4;
			return signedDistanceOfPointToLineSegment3 * signedDistanceOfPointToLineSegment4 >= 0f && num8 * num7 > num5 && num9 * num7 > num5 && this.QueryVisibilityRecursive(in q1, in q2, radius, node.Left) && this.QueryVisibilityRecursive(in q1, in q2, radius, node.Right);
		}

		// Token: 0x04000B31 RID: 2865
		private const int MaxLeafSize = 10;

		// Token: 0x04000B32 RID: 2866
		private DWAAgent[] _agents;

		// Token: 0x04000B33 RID: 2867
		private DWAAgentTreeNode[] _agentTree;

		// Token: 0x04000B34 RID: 2868
		private DWAObstacleTreeNode _obstacleTree;

		// Token: 0x04000B35 RID: 2869
		private DWASimulator _simulator;
	}
}
