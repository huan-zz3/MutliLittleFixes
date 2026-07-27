using System;
using System.Runtime.CompilerServices;
using NavalDLC.DWA;
using TaleWorlds.Library;

// Token: 0x02000004 RID: 4
public static class DWAHelpers
{
	// Token: 0x06000003 RID: 3 RVA: 0x00002058 File Offset: 0x00000258
	public static float AgentToAgentSignedClearance(in Vec2 center1, in Vec2 dir1, in Vec2 halfSize1, in Vec2 center2, in Vec2 dir2, in Vec2 halfSize2)
	{
		Vec2 vec;
		Vec2 vec2;
		DWAHelpers.OBBAxes(in dir1, out vec, out vec2);
		Vec2 vec3;
		Vec2 vec4;
		DWAHelpers.OBBAxes(in dir2, out vec3, out vec4);
		Vec2 vec5 = center2 - center1;
		bool flag = false;
		float num = 0f;
		float maxValue = float.MaxValue;
		DWAHelpers.CheckAxisSeparationBetweenOBBs(in vec, in vec5, in vec, in vec2, in halfSize1, in vec3, in vec4, in halfSize2, ref flag, ref num, ref maxValue);
		DWAHelpers.CheckAxisSeparationBetweenOBBs(in vec2, in vec5, in vec, in vec2, in halfSize1, in vec3, in vec4, in halfSize2, ref flag, ref num, ref maxValue);
		DWAHelpers.CheckAxisSeparationBetweenOBBs(in vec3, in vec5, in vec, in vec2, in halfSize1, in vec3, in vec4, in halfSize2, ref flag, ref num, ref maxValue);
		DWAHelpers.CheckAxisSeparationBetweenOBBs(in vec4, in vec5, in vec, in vec2, in halfSize1, in vec3, in vec4, in halfSize2, ref flag, ref num, ref maxValue);
		if (!flag)
		{
			return -maxValue;
		}
		return num;
	}

	// Token: 0x06000004 RID: 4 RVA: 0x00002110 File Offset: 0x00000310
	public static float AgentToConvexPolySignedClearance(in Vec2 center, in Vec2 dir, in Vec2 half, Vec2[] verts, int count, out bool overlap)
	{
		Vec2 vec;
		Vec2 vec2;
		DWAHelpers.OBBAxes(in dir, out vec, out vec2);
		bool flag = false;
		float num = 0f;
		float num2 = float.MaxValue;
		for (int i = 0; i < count; i++)
		{
			Vec2 vec3 = verts[i];
			Vec2 vec4 = verts[(i + 1) % count] - vec3;
			if (vec4.Normalize() > 1E-06f)
			{
				Vec2 vec5 = vec4.RightVec();
				float num3;
				float num4;
				DWAHelpers.ProjectPolyOnAxis(in vec5, verts, count, out num3, out num4);
				float num5 = Vec2.DotProduct(center, vec5);
				float num6 = DWAHelpers.ProjectOBBOnAxis(in vec5, in vec, in vec2, in half);
				float num7 = num5 - num6;
				float num8 = num5 + num6;
				if (num8 < num3)
				{
					flag = true;
					float num9 = num3 - num8;
					if (num9 > num)
					{
						num = num9;
					}
				}
				else if (num4 < num7)
				{
					flag = true;
					float num10 = num7 - num4;
					if (num10 > num)
					{
						num = num10;
					}
				}
				else
				{
					float num11 = MathF.Min(num8, num4) - MathF.Max(num7, num3);
					if (num11 < num2)
					{
						num2 = num11;
					}
				}
			}
		}
		DWAHelpers.CheckAxisSeparationBetweenOBBAndPoly(in vec, in center, in vec, in vec2, in half, verts, count, ref flag, ref num, ref num2);
		DWAHelpers.CheckAxisSeparationBetweenOBBAndPoly(in vec2, in center, in vec, in vec2, in half, verts, count, ref flag, ref num, ref num2);
		overlap = !flag;
		if (!flag)
		{
			return -num2;
		}
		return num;
	}

	// Token: 0x06000005 RID: 5 RVA: 0x00002248 File Offset: 0x00000448
	private static void CheckAxisSeparationBetweenOBBs(in Vec2 axis, in Vec2 centerDiff, in Vec2 side1, in Vec2 fwd1, in Vec2 half1, in Vec2 side2, in Vec2 fwd2, in Vec2 half2, ref bool separated, ref float maxGap, ref float minOverlap)
	{
		float num = DWAHelpers.ProjectOBBOnAxis(in axis, in side1, in fwd1, in half1);
		float num2 = DWAHelpers.ProjectOBBOnAxis(in axis, in side2, in fwd2, in half2);
		float num3 = MathF.Abs(Vec2.DotProduct(centerDiff, axis)) - (num + num2);
		if (num3 > 0f)
		{
			separated = true;
			if (num3 > maxGap)
			{
				maxGap = num3;
				return;
			}
		}
		else
		{
			float num4 = -num3;
			if (num4 < minOverlap)
			{
				minOverlap = num4;
			}
		}
	}

	// Token: 0x06000006 RID: 6 RVA: 0x000022AC File Offset: 0x000004AC
	private static void CheckAxisSeparationBetweenOBBAndPoly(in Vec2 axis, in Vec2 center, in Vec2 side, in Vec2 fwd, in Vec2 half, Vec2[] verts, int count, ref bool separated, ref float maxGap, ref float minOverlap)
	{
		float num;
		float num2;
		DWAHelpers.ProjectPolyOnAxis(in axis, verts, count, out num, out num2);
		float num3 = DWAHelpers.ProjectOBBOnAxis(in axis, in side, in fwd, in half);
		float num4 = Vec2.DotProduct(center, axis);
		float num5 = num4 - num3;
		float num6 = num4 + num3;
		if (num6 < num)
		{
			separated = true;
			float num7 = num - num6;
			if (num7 > maxGap)
			{
				maxGap = num7;
				return;
			}
		}
		else if (num2 < num5)
		{
			separated = true;
			float num8 = num5 - num2;
			if (num8 > maxGap)
			{
				maxGap = num8;
				return;
			}
		}
		else
		{
			float num9 = MathF.Min(num6, num2) - MathF.Max(num5, num);
			if (num9 < minOverlap)
			{
				minOverlap = num9;
			}
		}
	}

	// Token: 0x06000007 RID: 7 RVA: 0x0000233F File Offset: 0x0000053F
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float ProjectOBBOnAxis(in Vec2 axis, in Vec2 side, in Vec2 fwd, in Vec2 half)
	{
		return MathF.Abs(Vec2.DotProduct(side, axis)) * half.x + MathF.Abs(Vec2.DotProduct(fwd, axis)) * half.y;
	}

	// Token: 0x06000008 RID: 8 RVA: 0x0000237C File Offset: 0x0000057C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void ProjectPolyOnAxis(in Vec2 axis, Vec2[] verts, int vertexCount, out float dMin, out float dMax)
	{
		float num = Vec2.DotProduct(verts[0], axis);
		dMin = num;
		dMax = num;
		for (int i = 1; i < vertexCount; i++)
		{
			float num2 = Vec2.DotProduct(verts[i], axis);
			if (num2 < dMin)
			{
				dMin = num2;
			}
			if (num2 > dMax)
			{
				dMax = num2;
			}
		}
	}

	// Token: 0x06000009 RID: 9 RVA: 0x000023D4 File Offset: 0x000005D4
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void OBBAxes(in Vec2 forward, out Vec2 xSide, out Vec2 yFwd)
	{
		yFwd = forward;
		Vec2 vec = forward;
		xSide = -vec.LeftVec();
	}

	// Token: 0x0600000A RID: 10 RVA: 0x00002408 File Offset: 0x00000608
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void ReadStaticObstacle(DWAObstacleVertex obstacleVertex, Vec2[] obsVertices, out int obsVertexCount)
	{
		DWAObstacleVertex dwaobstacleVertex = obstacleVertex;
		int num = 0;
		do
		{
			obsVertices[num] = dwaobstacleVertex.Point;
			num++;
			dwaobstacleVertex = dwaobstacleVertex.Next;
		}
		while (dwaobstacleVertex != obstacleVertex && num < obsVertices.Length);
		obsVertexCount = num;
	}

	// Token: 0x0600000B RID: 11 RVA: 0x00002440 File Offset: 0x00000640
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float GateNear(float distance, float gateLength, float gateStart = 0f)
	{
		float num = gateLength;
		if (num < 1E-06f)
		{
			num = 1E-06f;
		}
		float num2 = gateStart + num;
		float num3 = MBMath.SmoothStep(gateStart, num2, distance);
		return 1f - num3;
	}

	// Token: 0x0600000C RID: 12 RVA: 0x00002474 File Offset: 0x00000674
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float GateFar(float distance, float gateLength, float gateStart = 0f)
	{
		float num = gateLength;
		if (num < 1E-06f)
		{
			num = 1E-06f;
		}
		float num2 = gateStart + num;
		return MBMath.SmoothStep(gateStart, num2, distance);
	}

	// Token: 0x04000001 RID: 1
	private const float Epsilon = 1E-06f;
}
