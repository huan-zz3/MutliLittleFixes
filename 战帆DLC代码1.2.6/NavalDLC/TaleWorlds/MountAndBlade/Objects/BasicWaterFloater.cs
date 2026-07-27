using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Objects
{
	// Token: 0x02000014 RID: 20
	internal class BasicWaterFloater : ScriptComponentBehavior
	{
		// Token: 0x060000D6 RID: 214 RVA: 0x0000761E File Offset: 0x0000581E
		protected override void OnInit()
		{
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00007620 File Offset: 0x00005820
		protected override void OnTick(float dt)
		{
			this.Float();
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x00007628 File Offset: 0x00005828
		protected override void OnEditorInit()
		{
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x0000762A File Offset: 0x0000582A
		protected override void OnEditorTick(float dt)
		{
			this.Float();
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00007634 File Offset: 0x00005834
		private void Float()
		{
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			globalFrame.origin.z = base.Scene.GetWaterLevelAtPosition(globalFrame.origin.AsVec2, true, false);
			base.GameEntity.SetGlobalFrame(ref globalFrame, true);
		}
	}
}
