using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.View.Map.Navigation;
using TaleWorlds.CampaignSystem;

namespace NavalDLC.View.Map.Navigation
{
	// Token: 0x02000037 RID: 55
	public class NavalMapNavigationHandler : MapNavigationHandler
	{
		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060001A6 RID: 422 RVA: 0x0000CF62 File Offset: 0x0000B162
		// (set) Token: 0x060001A7 RID: 423 RVA: 0x0000CF6A File Offset: 0x0000B16A
		public ManageFleetNavigationElement ManageFleetNavigationElement { get; private set; }

		// Token: 0x060001A8 RID: 424 RVA: 0x0000CF73 File Offset: 0x0000B173
		protected override INavigationElement[] OnCreateElements()
		{
			this.ManageFleetNavigationElement = new ManageFleetNavigationElement(this);
			List<INavigationElement> list = base.OnCreateElements().ToList<INavigationElement>();
			list.Insert(3, this.ManageFleetNavigationElement);
			return list.ToArray();
		}
	}
}
