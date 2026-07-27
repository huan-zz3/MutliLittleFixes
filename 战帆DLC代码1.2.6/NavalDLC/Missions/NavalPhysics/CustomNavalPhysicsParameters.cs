using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace NavalDLC.Missions.NavalPhysics
{
	// Token: 0x020000BE RID: 190
	public class CustomNavalPhysicsParameters : ScriptComponentBehavior
	{
		// Token: 0x06000E48 RID: 3656 RVA: 0x0006F4A8 File Offset: 0x0006D6A8
		protected override void OnInit()
		{
			base.OnInit();
			base.GameEntity.GetFirstScriptOfType<NavalPhysics>().SetContinuousDriftSpeed(this.ContinuousDriftSpeed);
		}

		// Token: 0x06000E49 RID: 3657 RVA: 0x0006F4D4 File Offset: 0x0006D6D4
		protected override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			NavalPhysics firstScriptOfType = base.GameEntity.GetFirstScriptOfType<NavalPhysics>();
			if (firstScriptOfType == null)
			{
				return;
			}
			firstScriptOfType.SetContinuousDriftSpeed(this.ContinuousDriftSpeed);
		}

		// Token: 0x040008E9 RID: 2281
		public bool BehaveLikeShip;

		// Token: 0x040008EA RID: 2282
		public float FloatingForceMultiplier = 1f;

		// Token: 0x040008EB RID: 2283
		public float LinearFrictionMultiplierRight = 1f;

		// Token: 0x040008EC RID: 2284
		public float LinearFrictionMultiplierLeft = 1f;

		// Token: 0x040008ED RID: 2285
		public float LinearFrictionMultiplierForward = 1f;

		// Token: 0x040008EE RID: 2286
		public float LinearFrictionMultiplierBackward = 1f;

		// Token: 0x040008EF RID: 2287
		public float LinearFrictionMultiplierUp = 1f;

		// Token: 0x040008F0 RID: 2288
		public float LinearFrictionMultiplierDown = 1f;

		// Token: 0x040008F1 RID: 2289
		public Vec3 AngularFrictionMultiplier = Vec3.One;

		// Token: 0x040008F2 RID: 2290
		public float ContinuousDriftSpeed;
	}
}
