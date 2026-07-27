using System;
using System.Collections.Generic;
using NavalDLC;
using NavalDLC.Map;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Objects
{
	// Token: 0x02000015 RID: 21
	internal class CampaignMapAmbientOccluder : ScriptComponentBehavior
	{
		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000DC RID: 220 RVA: 0x0000768E File Offset: 0x0000588E
		private int MaximumNumberOfStorms
		{
			get
			{
				return NavalDLCManager.Instance.GameModels.MapStormModel.MaximumNumberOfStorms + 2;
			}
		}

		// Token: 0x060000DD RID: 221 RVA: 0x000076A8 File Offset: 0x000058A8
		protected override void OnInit()
		{
			Mesh firstMesh = base.GameEntity.GetFirstMesh();
			int num = MathF.Max(this.MaximumNumberOfStorms, 16);
			firstMesh.SetupAdditionalBoneBuffer(num);
			for (int i = 0; i < num; i++)
			{
				MatrixFrame zero = MatrixFrame.Zero;
				firstMesh.SetAdditionalBoneFrame(i, ref zero);
			}
		}

		// Token: 0x060000DE RID: 222 RVA: 0x000076F8 File Offset: 0x000058F8
		protected override void OnTick(float dt)
		{
			int i = 0;
			foreach (Storm storm in NavalDLCManager.Instance.StormManager.SpawnedStorms)
			{
				this.SetBoneFrame(storm.CurrentPosition.ToVec3(0f), base.GameEntity, i++);
			}
			foreach (GameEntity gameEntity in this._questStorms)
			{
				this.SetBoneFrame(gameEntity.GlobalPosition, base.GameEntity, i++);
			}
			int num = MathF.Max(this.MaximumNumberOfStorms, 16);
			while (i < num)
			{
				MatrixFrame zero = MatrixFrame.Zero;
				base.GameEntity.SetBoneFrameToAllMeshes(i, ref zero);
				i++;
			}
		}

		// Token: 0x060000DF RID: 223 RVA: 0x000077FC File Offset: 0x000059FC
		protected override void OnEditorInit()
		{
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x000077FE File Offset: 0x000059FE
		protected override void OnEditorTick(float dt)
		{
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00007800 File Offset: 0x00005A00
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 2;
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00007803 File Offset: 0x00005A03
		public void RegisterQuestStorm(GameEntity stormEntity)
		{
			this._questStorms.Add(stormEntity);
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00007811 File Offset: 0x00005A11
		public void UnregisterQuestStorm(GameEntity stormEntity)
		{
			this._questStorms.Remove(stormEntity);
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x00007820 File Offset: 0x00005A20
		private void SetBoneFrame(Vec3 origin, WeakGameEntity gameEntity, int boneIndex)
		{
			MatrixFrame identity = MatrixFrame.Identity;
			Vec3 vec = new Vec3(60f, 0f, 0f, -1f);
			identity.Scale(ref vec);
			identity.origin = origin;
			base.GameEntity.SetBoneFrameToAllMeshes(boneIndex, ref identity);
		}

		// Token: 0x0400007B RID: 123
		private const int MaximumSpecialStormNumber = 2;

		// Token: 0x0400007C RID: 124
		private readonly List<GameEntity> _questStorms = new List<GameEntity>();
	}
}
