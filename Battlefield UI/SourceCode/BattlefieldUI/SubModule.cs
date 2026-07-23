using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace BattlefieldUI
{
	// Token: 0x02000004 RID: 4
	public sealed class SubModule : MBSubModuleBase
	{
		// Token: 0x06000003 RID: 3 RVA: 0x00002060 File Offset: 0x00000260
		protected override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();
			try
			{
				this._harmony = new Harmony("ori.bannerlord.battlefield-ui");
				this._harmony.PatchAll(Assembly.GetExecutingAssembly());
				Debug.Print("[BattlefieldUI] Loaded and mission view injection registered.", 0, 12, 17592186044416UL);
			}
			catch (Exception ex)
			{
				string text = "[BattlefieldUI] Failed to initialize: ";
				Exception ex2 = ex;
				Debug.Print(text + ((ex2 != null) ? ex2.ToString() : null), 0, 12, 17592186044416UL);
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020E8 File Offset: 0x000002E8
		protected override void OnSubModuleUnloaded()
		{
			if (this._harmony != null)
			{
				this._harmony.UnpatchAll(this._harmony.Id);
			}
			base.OnSubModuleUnloaded();
		}

		// Token: 0x04000001 RID: 1
		private Harmony _harmony;
	}
}
