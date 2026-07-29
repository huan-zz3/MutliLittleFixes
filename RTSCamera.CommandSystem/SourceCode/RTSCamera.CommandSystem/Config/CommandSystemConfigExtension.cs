using System;

namespace RTSCamera.CommandSystem.Config
{
	// Token: 0x02000091 RID: 145
	public static class CommandSystemConfigExtension
	{
		// Token: 0x06000557 RID: 1367 RVA: 0x0001FCCE File Offset: 0x0001DECE
		public static bool IsMouseOverEnabled(this CommandSystemConfig config)
		{
			return config.ClickToSelectFormation || config.AttackSpecificFormation;
		}
	}
}
