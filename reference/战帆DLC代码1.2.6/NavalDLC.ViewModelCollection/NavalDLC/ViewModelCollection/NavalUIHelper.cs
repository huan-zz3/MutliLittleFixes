using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.CampaignBehaviors;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.ViewModelCollection
{
	// Token: 0x02000009 RID: 9
	public static class NavalUIHelper
	{
		// Token: 0x06000026 RID: 38 RVA: 0x00005261 File Offset: 0x00003461
		public static float GetHealthPercent(this Ship ship)
		{
			if (ship.MaxHitPoints == 0f)
			{
				return 0f;
			}
			return ship.HitPoints / ship.MaxHitPoints * 100f;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x0000528C File Offset: 0x0000348C
		private static Tuple<bool, TextObject> GetIsStringApplicableForShipName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return new Tuple<bool, TextObject>(false, new TextObject("{=aw2fR5fK}Ship name cannot be empty", null));
			}
			bool flag;
			if (name.Length < 3)
			{
				if (!name.Any<char>((char c) => Common.IsCharAsian(c)))
				{
					flag = false;
					goto IL_005A;
				}
			}
			flag = name.Length <= 50;
			IL_005A:
			if (!flag)
			{
				TextObject textObject = new TextObject("{=cSLiAJUw}Ship name should be between {MIN} and {MAX} characters", null);
				textObject.SetTextVariable("MIN", 3);
				textObject.SetTextVariable("MAX", 50);
				return new Tuple<bool, TextObject>(false, textObject);
			}
			if (!name.All<char>((char x) => (char.IsLetterOrDigit(x) || char.IsWhiteSpace(x) || char.IsPunctuation(x)) && x != '{' && x != '}'))
			{
				return new Tuple<bool, TextObject>(false, new TextObject("{=t9bmsau2}Ship name cannot contain special characters", null));
			}
			if (name.StartsWith(" ") || name.EndsWith(" "))
			{
				return new Tuple<bool, TextObject>(false, new TextObject("{=ol9uYvPl}Ship name cannot start or end with a white space", null));
			}
			if (name.Contains("  "))
			{
				return new Tuple<bool, TextObject>(false, new TextObject("{=bX4OPIPP}Ship name cannot contain consecutive white spaces", null));
			}
			return new Tuple<bool, TextObject>(true, TextObject.GetEmpty());
		}

		// Token: 0x06000028 RID: 40 RVA: 0x000053B4 File Offset: 0x000035B4
		public static Tuple<bool, string> IsStringApplicableForShipName(string name)
		{
			Tuple<bool, TextObject> isStringApplicableForShipName = NavalUIHelper.GetIsStringApplicableForShipName(name);
			return new Tuple<bool, string>(isStringApplicableForShipName.Item1, isStringApplicableForShipName.Item2.ToString());
		}

		// Token: 0x06000029 RID: 41 RVA: 0x000053DE File Offset: 0x000035DE
		public static Ship GetFlagship(PartyBase party)
		{
			return party.FlagShip;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000053E8 File Offset: 0x000035E8
		public static List<TooltipProperty> GetShipyardTooltip(Town town)
		{
			if (town == null)
			{
				return new List<TooltipProperty>();
			}
			List<TooltipProperty> list = new List<TooltipProperty>();
			Building shipyard = town.GetShipyard();
			list.Add(new TooltipProperty(string.Empty, new TextObject("{=4vkUyYui}Shipyard{newline}Level {LEVEL}", null).SetTextVariable("LEVEL", shipyard.CurrentLevel).ToString(), 0, false, 0));
			return list;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00005440 File Offset: 0x00003640
		public static string GetTownCoastalPatrolTooltip(Town town)
		{
			TextObject textObject = GameTexts.FindText("str_string_newline_string", null);
			textObject.SetTextVariable("newline", "\n");
			textObject.SetTextVariable("STR1", GameTexts.FindText("str_coastal_patrol", null));
			INavalPatrolPartiesCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<INavalPatrolPartiesCampaignBehavior>();
			TextObject textObject2;
			if (CampaignUIHelper.IsSettlementInformationHidden(town.Settlement, ref textObject2))
			{
				textObject.SetTextVariable("STR2", GameTexts.FindText("str_missing_info_indicator", null).ToString());
			}
			else if (campaignBehavior.GetNavalPatrolParty(town.Settlement) != null)
			{
				textObject.SetTextVariable("STR2", campaignBehavior.GetNavalPatrolParty(town.Settlement).GetBehaviorText().ToString());
			}
			else
			{
				textObject.SetTextVariable("STR2", campaignBehavior.GetSettlementPatrolStatus(town.Settlement).ToString());
			}
			return textObject.ToString();
		}

		// Token: 0x0600002C RID: 44 RVA: 0x0000550E File Offset: 0x0000370E
		public static string GetPrefabIdOfShipHull(ShipHull shipHull)
		{
			MissionShipObject @object = MBObjectManager.Instance.GetObject<MissionShipObject>(shipHull.MissionShipObjectId);
			return ((@object != null) ? @object.Prefab : null) ?? string.Empty;
		}
	}
}
