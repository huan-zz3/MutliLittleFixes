using System;
using TaleWorlds.Library;

namespace NavalDLC.Missions.NavalPhysics
{
	// Token: 0x020000C4 RID: 196
	public static class ShipPhysicsInitializer
	{
		// Token: 0x06000EBD RID: 3773 RVA: 0x00072FD8 File Offset: 0x000711D8
		public static Vec3 GetDefaultInertia(float mass, in Vec3 draftVolume)
		{
			float num = 0.08333f * mass * (draftVolume.y * draftVolume.y + draftVolume.z * draftVolume.z);
			float num2 = 0.08333f * mass * (draftVolume.x * draftVolume.x + draftVolume.z * draftVolume.z);
			float num3 = 0.08333f * mass * (draftVolume.x * draftVolume.x + draftVolume.y * draftVolume.y);
			return new Vec3(num, num2, num3, -1f);
		}
	}
}
