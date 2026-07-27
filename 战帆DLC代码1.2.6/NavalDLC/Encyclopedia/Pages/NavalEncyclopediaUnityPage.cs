using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.CampaignSystem.Encyclopedia.Pages;
using TaleWorlds.Localization;

namespace NavalDLC.Encyclopedia.Pages
{
	// Token: 0x02000146 RID: 326
	[OverrideEncyclopediaModel(new Type[] { typeof(CharacterObject) })]
	public class NavalEncyclopediaUnityPage : DefaultEncyclopediaUnitPage
	{
		// Token: 0x0600158A RID: 5514 RVA: 0x00096A87 File Offset: 0x00094C87
		protected override List<EncyclopediaFilterItem> GetTypeFilterItems()
		{
			List<EncyclopediaFilterItem> typeFilterItems = base.GetTypeFilterItems();
			typeFilterItems.Add(new EncyclopediaFilterItem(new TextObject("{=bOhiqquf}Mariner", null), (object s) => ((CharacterObject)s).IsMariner));
			return typeFilterItems;
		}
	}
}
