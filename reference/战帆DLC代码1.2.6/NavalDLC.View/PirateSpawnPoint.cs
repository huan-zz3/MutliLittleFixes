using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.View
{
	// Token: 0x0200000D RID: 13
	public class PirateSpawnPoint : ScriptComponentBehavior
	{
		// Token: 0x06000062 RID: 98 RVA: 0x00004E24 File Offset: 0x00003024
		public Vec2 GetPosition()
		{
			return base.GameEntity.GlobalPosition.AsVec2;
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00004E47 File Offset: 0x00003047
		protected override void OnInit()
		{
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00004E49 File Offset: 0x00003049
		protected override void OnEditorInit()
		{
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00004E4B File Offset: 0x0000304B
		protected override void OnSceneSave(string saveFolder)
		{
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00004E50 File Offset: 0x00003050
		protected override void OnEditorTick(float dt)
		{
			if (this.ToggleDebugRadius || MBEditor.IsEntitySelected(base.GameEntity))
			{
				Scene scene = base.Scene;
				MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
				float radius = this.Radius;
				Color red = Colors.Red;
				DebugExtensions.RenderDebugCircleOnTerrain(scene, globalFrame, radius, red.ToUnsignedInteger(), true, false);
			}
		}

		// Token: 0x0400001D RID: 29
		public string ClanStringId;

		// Token: 0x0400001E RID: 30
		public bool ToggleDebugRadius;

		// Token: 0x0400001F RID: 31
		public float Radius = 10f;
	}
}
