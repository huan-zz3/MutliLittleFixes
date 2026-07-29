using System;
using System.Collections.Generic;
using System.Linq;
using MissionLibrary;
using MissionLibrary.Controller;
using MissionSharedLibrary.Provider;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace MissionSharedLibrary.Controller
{
	// Token: 0x02000037 RID: 55
	public class MissionStartingManager : AMissionStartingManager
	{
		// Token: 0x060001FA RID: 506 RVA: 0x000078B4 File Offset: 0x00005AB4
		public static void AddMissionBehavior(MissionView entranceView, MissionBehavior behaviour)
		{
			behaviour.OnAfterMissionCreated();
			entranceView.Mission.AddMissionBehavior(behaviour);
		}

		// Token: 0x060001FB RID: 507 RVA: 0x000078C8 File Offset: 0x00005AC8
		public override void OnCreated(MissionView entranceView)
		{
			foreach (AMissionStartingHandler amissionStartingHandler in this.GetHandlers())
			{
				amissionStartingHandler.OnCreated(entranceView);
			}
		}

		// Token: 0x060001FC RID: 508 RVA: 0x00007914 File Offset: 0x00005B14
		public override void OnPreMissionTick(MissionView entranceView, float dt)
		{
			foreach (AMissionStartingHandler amissionStartingHandler in this.GetHandlers())
			{
				amissionStartingHandler.OnPreMissionTick(entranceView, dt);
			}
		}

		// Token: 0x060001FD RID: 509 RVA: 0x00007960 File Offset: 0x00005B60
		public override void AddHandler(AMissionStartingHandler handler)
		{
			this._handlers.Add(handler);
		}

		// Token: 0x060001FE RID: 510 RVA: 0x0000796E File Offset: 0x00005B6E
		public override void AddSingletonHandler(string key, AMissionStartingHandler handler, Version version)
		{
			Global.RegisterInstance<AMissionStartingHandler>(VersionProviderCreator.Create<AMissionStartingHandler>(() => handler, version), key);
		}

		// Token: 0x060001FF RID: 511 RVA: 0x00007993 File Offset: 0x00005B93
		private IEnumerable<AMissionStartingHandler> GetHandlers()
		{
			return this._handlers.Concat<AMissionStartingHandler>(Global2.GetInstances<AMissionStartingHandler>());
		}

		// Token: 0x040000CF RID: 207
		private readonly List<AMissionStartingHandler> _handlers = new List<AMissionStartingHandler>();
	}
}
