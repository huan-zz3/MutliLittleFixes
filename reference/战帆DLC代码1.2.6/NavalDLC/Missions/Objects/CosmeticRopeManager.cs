using System;
using System.Collections.Generic;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace NavalDLC.Missions.Objects
{
	// Token: 0x02000098 RID: 152
	[ScriptComponentParams("ship_visual_only", "")]
	public class CosmeticRopeManager : ScriptComponentBehavior
	{
		// Token: 0x06000AD6 RID: 2774 RVA: 0x0004D2F8 File Offset: 0x0004B4F8
		protected override void OnEditorInit()
		{
			this.FetchEntities();
		}

		// Token: 0x06000AD7 RID: 2775 RVA: 0x0004D300 File Offset: 0x0004B500
		protected override void OnInit()
		{
			this.FetchEntities();
		}

		// Token: 0x06000AD8 RID: 2776 RVA: 0x0004D308 File Offset: 0x0004B508
		protected override void OnEditorTick(float dt)
		{
			this.FetchEntities();
			this.HandleLOD();
		}

		// Token: 0x06000AD9 RID: 2777 RVA: 0x0004D316 File Offset: 0x0004B516
		protected override void OnTickParallel(float dt)
		{
			this.HandleLOD();
		}

		// Token: 0x06000ADA RID: 2778 RVA: 0x0004D31E File Offset: 0x0004B51E
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 4;
		}

		// Token: 0x06000ADB RID: 2779 RVA: 0x0004D324 File Offset: 0x0004B524
		private void FetchEntities()
		{
			if (!base.GameEntity.IsInEditorScene())
			{
				base.GameEntity.SetEntityFlags(base.GameEntity.EntityFlags | 536870912);
			}
			this._cosmeticsRopeSegments.Clear();
			foreach (WeakGameEntity weakGameEntity in base.GameEntity.GetChildren())
			{
				WeakGameEntity firstChildEntityWithTag = weakGameEntity.GetFirstChildEntityWithTag("simple_rope_start");
				if (firstChildEntityWithTag.IsValid)
				{
					RopeSegment firstScriptOfType = firstChildEntityWithTag.GetFirstScriptOfType<RopeSegment>();
					if (firstScriptOfType != null)
					{
						this._cosmeticsRopeSegments.Add(firstScriptOfType);
					}
				}
			}
		}

		// Token: 0x06000ADC RID: 2780 RVA: 0x0004D3E0 File Offset: 0x0004B5E0
		private void HandleLOD()
		{
			Vec3 lastFinalRenderCameraPositionOfScene = base.GameEntity.GetLastFinalRenderCameraPositionOfScene();
			Vec3 origin = base.GameEntity.GetGlobalFrame().origin;
			float num = lastFinalRenderCameraPositionOfScene.DistanceSquared(origin);
			bool flag = num > 10000f;
			bool flag2 = num > 2025f;
			if (this._ropesWereInvisibleLastFrame != flag || this._lodCheckFirstFrame)
			{
				base.GameEntity.SetVisibilityExcludeParents(!flag);
			}
			if (this._ropesWereLinearLastFrame != flag2 || this._lodCheckFirstFrame)
			{
				foreach (RopeSegment ropeSegment in this._cosmeticsRopeSegments)
				{
					ropeSegment.SetLinearMode(flag2);
				}
			}
			this._ropesWereInvisibleLastFrame = flag;
			this._ropesWereLinearLastFrame = flag2;
			this._lodCheckFirstFrame = false;
		}

		// Token: 0x04000665 RID: 1637
		private const string RopeScriptEntityTag = "simple_rope_start";

		// Token: 0x04000666 RID: 1638
		private const float InvisibleDistanceSquared = 10000f;

		// Token: 0x04000667 RID: 1639
		private const float LinearDistanceSquared = 2025f;

		// Token: 0x04000668 RID: 1640
		private List<RopeSegment> _cosmeticsRopeSegments = new List<RopeSegment>();

		// Token: 0x04000669 RID: 1641
		private bool _ropesWereInvisibleLastFrame;

		// Token: 0x0400066A RID: 1642
		private bool _ropesWereLinearLastFrame;

		// Token: 0x0400066B RID: 1643
		private bool _lodCheckFirstFrame = true;
	}
}
