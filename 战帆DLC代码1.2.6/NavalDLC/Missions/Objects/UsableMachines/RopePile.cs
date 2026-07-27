using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace NavalDLC.Missions.Objects.UsableMachines
{
	// Token: 0x020000AF RID: 175
	public class RopePile : ScriptComponentBehavior
	{
		// Token: 0x06000D41 RID: 3393 RVA: 0x00068B37 File Offset: 0x00066D37
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return base.GetTickRequirement() | 2;
		}

		// Token: 0x06000D42 RID: 3394 RVA: 0x00068B44 File Offset: 0x00066D44
		protected override void OnInit()
		{
			base.SetScriptComponentToTick(this.GetTickRequirement());
			base.GameEntity.GetFirstMesh().SetupAdditionalBoneBuffer(1);
		}

		// Token: 0x06000D43 RID: 3395 RVA: 0x00068B74 File Offset: 0x00066D74
		protected override void OnTick(float dt)
		{
			Mesh firstMesh = base.GameEntity.GetFirstMesh();
			Mat3 mat = new Mat3(ref this.point0, ref this.point1, ref this.point2);
			MatrixFrame matrixFrame = new MatrixFrame(ref mat, ref this.point3);
			firstMesh.SetAdditionalBoneFrame(0, ref matrixFrame);
			Vec3 vectorArgument = firstMesh.GetVectorArgument();
			vectorArgument.z = this.factor;
			firstMesh.SetVectorArgument(vectorArgument.x, vectorArgument.y, vectorArgument.z, vectorArgument.w);
		}

		// Token: 0x04000834 RID: 2100
		public Vec3 point0 = new Vec3(0f, 0f, 0f, 0f);

		// Token: 0x04000835 RID: 2101
		public Vec3 point1 = new Vec3(0f, 0f, 0f, 0f);

		// Token: 0x04000836 RID: 2102
		public Vec3 point2 = new Vec3(0f, 0f, 0f, 0f);

		// Token: 0x04000837 RID: 2103
		public Vec3 point3 = new Vec3(0f, 0f, 0f, 0f);

		// Token: 0x04000838 RID: 2104
		public float factor;
	}
}
