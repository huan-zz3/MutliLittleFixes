using System;
using MissionLibrary;
using MissionLibrary.Controller;
using MissionSharedLibrary.HotKey;

namespace MissionSharedLibrary.Controller
{
	// Token: 0x02000038 RID: 56
	public class GeneralResourceCreator : AResourceCreator
	{
		// Token: 0x06000201 RID: 513 RVA: 0x000079B8 File Offset: 0x00005BB8
		public GeneralResourceCreator()
		{
			Global.GetInstance<AMissionStartingManager>("").AddHandler(new DefaultMissionStartingHandler());
			GeneralGameKeyCategory.RegisterGameKeyCategory();
		}
	}
}
