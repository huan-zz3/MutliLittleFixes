using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace MissionSharedLibrary.View.Widgets
{
	// Token: 0x02000019 RID: 25
	public class MissionLibraryGameKeyGroupWidget : ListPanel
	{
		// Token: 0x060000D6 RID: 214 RVA: 0x00004D67 File Offset: 0x00002F67
		public MissionLibraryGameKeyGroupWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00004D70 File Offset: 0x00002F70
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x00004D79 File Offset: 0x00002F79
		// (set) Token: 0x060000D9 RID: 217 RVA: 0x00004D81 File Offset: 0x00002F81
		public string OptionTitle { get; set; }

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000DA RID: 218 RVA: 0x00004D8A File Offset: 0x00002F8A
		// (set) Token: 0x060000DB RID: 219 RVA: 0x00004D92 File Offset: 0x00002F92
		public string OptionDescription { get; set; }
	}
}
