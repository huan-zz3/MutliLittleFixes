using System;
using MissionLibrary.Provider;

namespace MissionLibrary.Controller.Camera
{
	// Token: 0x02000027 RID: 39
	public abstract class ACameraControllerManager : ATag<ACameraControllerManager>
	{
		// Token: 0x0600009E RID: 158 RVA: 0x00002A7D File Offset: 0x00000C7D
		public static ACameraControllerManager Get()
		{
			return Global.GetInstance<ACameraControllerManager>("");
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600009F RID: 159
		// (set) Token: 0x060000A0 RID: 160
		public abstract ICameraController Instance { get; set; }

		// Token: 0x060000A1 RID: 161
		public abstract void Clear();
	}
}
