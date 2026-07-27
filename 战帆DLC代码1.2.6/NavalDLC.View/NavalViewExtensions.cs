using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace NavalDLC.View
{
	// Token: 0x0200000B RID: 11
	public static class NavalViewExtensions
	{
		// Token: 0x06000051 RID: 81 RVA: 0x00003970 File Offset: 0x00001B70
		public static BoundingBox GetBoundingBoxIncludingChildren(this GameEntity entity)
		{
			BoundingBox boundingBox = default(BoundingBox);
			NavalViewExtensions.GetBoundingBoxIncludingChildrenAux(entity, ref boundingBox);
			boundingBox.RecomputeRadius();
			return boundingBox;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003998 File Offset: 0x00001B98
		private static void GetBoundingBoxIncludingChildrenAux(GameEntity entity, ref BoundingBox boundingBox)
		{
			int componentCount = entity.GetComponentCount(0);
			for (int i = 0; i < componentCount; i++)
			{
				MetaMesh metaMesh = entity.GetMetaMesh(i);
				if (metaMesh != null)
				{
					BoundingBox boundingBox2 = metaMesh.GetBoundingBox();
					boundingBox.RelaxMinMaxWithPoint(ref boundingBox2.min);
					boundingBox.RelaxMinMaxWithPoint(ref boundingBox2.max);
				}
			}
			Mesh firstMesh = entity.GetFirstMesh();
			if (firstMesh != null)
			{
				Vec3 vec = firstMesh.GetBoundingBoxMin();
				boundingBox.RelaxMinMaxWithPoint(ref vec);
				vec = firstMesh.GetBoundingBoxMax();
				boundingBox.RelaxMinMaxWithPoint(ref vec);
			}
			for (int j = 0; j < entity.ChildCount; j++)
			{
				NavalViewExtensions.GetBoundingBoxIncludingChildrenAux(entity.GetChild(j), ref boundingBox);
			}
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00003A40 File Offset: 0x00001C40
		public static void FitEntityInsideView(this Camera camera, Vec3 normalizedCameraOffset, GameEntity entity)
		{
			entity.RecomputeBoundingBox();
			float boundingBoxRadius = entity.GetBoundingBoxRadius();
			Vec3 vec = entity.GetFrame().origin + (entity.GetBoundingBoxMin() + entity.GetBoundingBoxMax()) * 0.5f;
			float num = boundingBoxRadius / MathF.Abs(MathF.Sin(camera.HorizontalFov * 0.5f));
			Vec3 vec2 = vec + normalizedCameraOffset * num;
			camera.LookAt(vec2, vec, Vec3.Up);
		}
	}
}
