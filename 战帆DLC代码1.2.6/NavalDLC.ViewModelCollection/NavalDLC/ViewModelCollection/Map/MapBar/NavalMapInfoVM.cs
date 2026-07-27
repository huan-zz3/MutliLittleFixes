using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.ViewModelCollection.Map.MapBar;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.ViewModelCollection.Map.MapBar
{
	// Token: 0x0200002F RID: 47
	public class NavalMapInfoVM : MapInfoVM
	{
		// Token: 0x060003EE RID: 1006 RVA: 0x00012FA9 File Offset: 0x000111A9
		public NavalMapInfoVM()
		{
			this.RefreshValues();
		}

		// Token: 0x060003EF RID: 1007 RVA: 0x00012FC2 File Offset: 0x000111C2
		public override void RefreshValues()
		{
			base.RefreshValues();
			this._invalidShipHealthText = new TextObject("{=4NaOKslb}-", null).ToString();
		}

		// Token: 0x060003F0 RID: 1008 RVA: 0x00012FE0 File Offset: 0x000111E0
		protected override void CreateItems()
		{
			base.CreateItems();
			this._shipHealthInfo = new MapInfoItemVM("ship_health", new Func<List<TooltipProperty>>(this.GetShipTooltip));
			base.PrimaryInfoItems.Insert(2, this._shipHealthInfo);
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x00013018 File Offset: 0x00011218
		protected override void UpdatePlayerInfo(bool updateForced)
		{
			base.UpdatePlayerInfo(updateForced);
			MobileParty mainParty = MobileParty.MainParty;
			if (((mainParty != null) ? mainParty.Ships : null) == null || MobileParty.MainParty.Ships.Count == 0)
			{
				this._shipHealthInfo.Value = this._invalidShipHealthText;
				return;
			}
			float num = MobileParty.MainParty.Ships.Average<Ship>((Ship s) => s.GetHealthPercent());
			this._shipHealthInfo.HasWarning = num < 20f;
			if (this._shipHealthInfo.FloatValue != num)
			{
				this._shipHealthInfo.Value = GameTexts.FindText("str_NUMBER_percent", null).SetTextVariable("NUMBER", MathF.Ceiling(num).ToString()).ToString();
			}
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x000130E8 File Offset: 0x000112E8
		private List<TooltipProperty> GetShipTooltip()
		{
			MobileParty mainParty = MobileParty.MainParty;
			if (((mainParty != null) ? mainParty.Ships : null) == null || MobileParty.MainParty.Ships.Count == 0)
			{
				return new List<TooltipProperty>
				{
					new TooltipProperty("", new TextObject("{=lb2hbQyx}You don't have any ships", null).ToString(), 0, false, 0)
				};
			}
			List<TooltipProperty> list = new List<TooltipProperty>();
			float num = MobileParty.MainParty.Ships.Average<Ship>((Ship s) => s.GetHealthPercent());
			list.Add(new TooltipProperty(new TextObject("{=oTM78wf6}Fleet Condition", null).ToString(), GameTexts.FindText("str_NUMBER_percent", null).SetTextVariable("NUMBER", MathF.Ceiling(num).ToString()).ToString(), 0, false, 4096));
			List<Ship> list2 = MobileParty.MainParty.Ships.ToList<Ship>();
			list2.Sort(this._shipHealthPercentageComparer);
			foreach (Ship ship in list2)
			{
				string text = GameTexts.FindText("str_NUMBER_percent", null).SetTextVariable("NUMBER", MathF.Ceiling(ship.GetHealthPercent()).ToString()).ToString();
				list.Add(new TooltipProperty(ship.Name.ToString(), text, 0, false, 0));
			}
			return list;
		}

		// Token: 0x04000185 RID: 389
		private MapInfoItemVM _shipHealthInfo;

		// Token: 0x04000186 RID: 390
		private string _invalidShipHealthText;

		// Token: 0x04000187 RID: 391
		private readonly ShipHealthPercentageComparer _shipHealthPercentageComparer = new ShipHealthPercentageComparer();
	}
}
