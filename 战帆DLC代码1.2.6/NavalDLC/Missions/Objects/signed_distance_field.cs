using System;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace NavalDLC.Missions.Objects
{
	// Token: 0x020000AA RID: 170
	[ScriptComponentParams("ship_visual_only", "")]
	public class signed_distance_field : ScriptComponentBehavior
	{
		// Token: 0x06000D04 RID: 3332 RVA: 0x00064894 File Offset: 0x00062A94
		public void DummyFunc()
		{
			Debug.Print(this._visualizeSDF.ToString(), 0, 12, 17592186044416UL);
		}

		// Token: 0x06000D05 RID: 3333 RVA: 0x000648B2 File Offset: 0x00062AB2
		private signed_distance_field()
		{
		}

		// Token: 0x06000D06 RID: 3334 RVA: 0x000648C1 File Offset: 0x00062AC1
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 4;
		}

		// Token: 0x06000D07 RID: 3335 RVA: 0x000648C4 File Offset: 0x00062AC4
		protected override void OnInit()
		{
			if (base.GameEntity.IsGhostObject())
			{
				return;
			}
			this._sdfIndex = base.GameEntity.RegisterWaterSDFClip(this._sdfTexture);
			this.SetSDFParams();
		}

		// Token: 0x06000D08 RID: 3336 RVA: 0x00064904 File Offset: 0x00062B04
		protected override void OnEditorInit()
		{
			if (base.GameEntity.IsGhostObject())
			{
				return;
			}
			this._sdfIndex = base.GameEntity.RegisterWaterSDFClip(this._sdfTexture);
			this.SetSDFParams();
		}

		// Token: 0x06000D09 RID: 3337 RVA: 0x00064942 File Offset: 0x00062B42
		protected override void OnTickParallel(float dt)
		{
			this.SetSDFParams();
		}

		// Token: 0x06000D0A RID: 3338 RVA: 0x0006494A File Offset: 0x00062B4A
		protected override void OnEditorVariableChanged(string variableName)
		{
		}

		// Token: 0x06000D0B RID: 3339 RVA: 0x0006494C File Offset: 0x00062B4C
		protected override void OnRemoved(int removeReason)
		{
			if (this._sdfIndex != -1)
			{
				base.GameEntity.DeRegisterWaterSDFClip(this._sdfIndex);
			}
		}

		// Token: 0x06000D0C RID: 3340 RVA: 0x00064978 File Offset: 0x00062B78
		private MatrixFrame ComputeBBOXFrame(ref Vec3 sdfBBExtend)
		{
			Vec3 vec = default(Vec3);
			Vec3 vec2 = default(Vec3);
			this._sdfTexture.GetSDFBoundingBoxData(ref vec, ref vec2);
			BoundingBox boundingBox = default(BoundingBox);
			boundingBox.BeginRelaxation();
			boundingBox.RelaxMinMaxWithPoint(ref vec);
			boundingBox.RelaxMinMaxWithPoint(ref vec2);
			boundingBox.RecomputeRadius();
			MatrixFrame identity = MatrixFrame.Identity;
			identity.origin = boundingBox.center;
			sdfBBExtend = boundingBox.max - boundingBox.min;
			identity.rotation.s = identity.rotation.s * (sdfBBExtend.x * 0.5f);
			identity.rotation.f = identity.rotation.f * (sdfBBExtend.y * 0.5f);
			identity.rotation.u = identity.rotation.u * (sdfBBExtend.z * 0.5f);
			return identity;
		}

		// Token: 0x06000D0D RID: 3341 RVA: 0x00064A70 File Offset: 0x00062C70
		private void SetSDFParams()
		{
			if (this._sdfTexture != null && this._sdfIndex != -1)
			{
				Vec3 vec = default(Vec3);
				MatrixFrame matrixFrame = this.ComputeBBOXFrame(ref vec);
				matrixFrame = base.GameEntity.GetGlobalFrame().TransformToParent(ref matrixFrame);
				matrixFrame.Fill();
				MatrixFrame matrixFrame2 = matrixFrame.Inverse();
				matrixFrame2.Fill();
				base.GameEntity.SetWaterSDFClipData(this._sdfIndex, ref matrixFrame2, base.GameEntity.IsVisibleIncludeParents());
			}
		}

		// Token: 0x040007E6 RID: 2022
		[EditableScriptComponentVariable(true, "SDF Texture")]
		private Texture _sdfTexture;

		// Token: 0x040007E7 RID: 2023
		[EditableScriptComponentVariable(true, "Visualize SDF")]
		private bool _visualizeSDF;

		// Token: 0x040007E8 RID: 2024
		private int _sdfIndex = -1;
	}
}
