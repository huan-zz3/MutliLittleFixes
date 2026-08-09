using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.ViewModelCollection.Encyclopedia
{
	// Token: 0x02000038 RID: 56
	[EncyclopediaViewModel(typeof(Settlement))]
	public class NavalEncyclopediaSettlementPageVM : EncyclopediaSettlementPageVM
	{
		// Token: 0x06000444 RID: 1092 RVA: 0x00013F0D File Offset: 0x0001210D
		public NavalEncyclopediaSettlementPageVM(EncyclopediaPageArgs args)
			: base(args)
		{
		}

		// Token: 0x06000445 RID: 1093 RVA: 0x00013F18 File Offset: 0x00012118
		public override void Refresh()
		{
			base.Refresh();
			Town town = this._settlement.Town;
			if (((town != null) ? town.GetShipyard() : null) != null)
			{
				TextObject textObject;
				bool flag = CampaignUIHelper.IsSettlementInformationHidden(this._settlement, ref textObject);
				string text = GameTexts.FindText("str_missing_info_indicator", null).ToString();
				string text2;
				if (!flag)
				{
					Town town2 = this._settlement.Town;
					if (town2 == null)
					{
						text2 = null;
					}
					else
					{
						Building shipyard = town2.GetShipyard();
						text2 = ((shipyard != null) ? shipyard.CurrentLevel.ToString() : null);
					}
				}
				else
				{
					text2 = text;
				}
				this.ShipyardText = text2;
				this.ShipyardHint = new BasicTooltipViewModel(() => NavalUIHelper.GetShipyardTooltip(this._settlement.Town));
				for (int i = 0; i < base.LeftSideProperties.Count; i++)
				{
					if (base.LeftSideProperties[i].TypeString == "Wall")
					{
						EncyclopediaSettlementPageStatItemVM encyclopediaSettlementPageStatItemVM = base.LeftSideProperties[base.LeftSideProperties.Count - 1];
						base.LeftSideProperties.Remove(encyclopediaSettlementPageStatItemVM);
						base.RightSideProperties.Insert(0, encyclopediaSettlementPageStatItemVM);
						base.LeftSideProperties.Insert(i + 1, new EncyclopediaSettlementPageStatItemVM(this.ShipyardHint, 1, this.ShipyardText));
						return;
					}
				}
			}
		}

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x06000446 RID: 1094 RVA: 0x00014047 File Offset: 0x00012247
		// (set) Token: 0x06000447 RID: 1095 RVA: 0x0001404F File Offset: 0x0001224F
		[DataSourceProperty]
		public string ShipyardText
		{
			get
			{
				return this._shipyardText;
			}
			set
			{
				if (value != this._shipyardText)
				{
					this._shipyardText = value;
					base.OnPropertyChangedWithValue<string>(value, "ShipyardText");
				}
			}
		}

		// Token: 0x17000127 RID: 295
		// (get) Token: 0x06000448 RID: 1096 RVA: 0x00014072 File Offset: 0x00012272
		// (set) Token: 0x06000449 RID: 1097 RVA: 0x0001407A File Offset: 0x0001227A
		[DataSourceProperty]
		public BasicTooltipViewModel ShipyardHint
		{
			get
			{
				return this._shipyardHint;
			}
			set
			{
				if (value != this._shipyardHint)
				{
					this._shipyardHint = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "ShipyardHint");
				}
			}
		}

		// Token: 0x040001AC RID: 428
		private string _shipyardText;

		// Token: 0x040001AD RID: 429
		private BasicTooltipViewModel _shipyardHint;
	}
}
