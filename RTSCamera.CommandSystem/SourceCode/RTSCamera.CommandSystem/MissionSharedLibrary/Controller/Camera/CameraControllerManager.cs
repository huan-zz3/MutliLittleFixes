using System;
using MissionLibrary.Controller.Camera;

namespace MissionSharedLibrary.Controller.Camera
{
	// Token: 0x02000039 RID: 57
	public class CameraControllerManager : ACameraControllerManager
	{
		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000202 RID: 514 RVA: 0x000079D9 File Offset: 0x00005BD9
		// (set) Token: 0x06000203 RID: 515 RVA: 0x000079E1 File Offset: 0x00005BE1
		public override ICameraController Instance { get; set; }

		// Token: 0x06000204 RID: 516 RVA: 0x000079EA File Offset: 0x00005BEA
		public override void Clear()
		{
			this.Instance = null;
		}
	}
}
