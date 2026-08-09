using System;
using NavalDLC.Storyline;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.Localization;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000118 RID: 280
	public class NavalDLCFleetManagementModel : FleetManagementModel
	{
		// Token: 0x17000365 RID: 869
		// (get) Token: 0x06001414 RID: 5140 RVA: 0x0008FAD0 File Offset: 0x0008DCD0
		public override int MinimumTroopCountRequiredToSendShips
		{
			get
			{
				return 8;
			}
		}

		// Token: 0x06001415 RID: 5141 RVA: 0x0008FAD3 File Offset: 0x0008DCD3
		public override bool CanTroopsReturn()
		{
			return !Hero.MainHero.IsPrisoner && (!MobileParty.MainParty.IsCurrentlyAtSea || Settlement.CurrentSettlement != null) && MobileParty.MainParty.MapEvent == null;
		}

		// Token: 0x06001416 RID: 5142 RVA: 0x0008FB04 File Offset: 0x0008DD04
		public override CampaignTime GetReturnTimeForTroops(Ship ship)
		{
			return CampaignTime.DaysFromNow(RandomOwnerExtensions.RandomFloatWithSeed(Hero.MainHero, (uint)CampaignTime.Now.ToMinutes, 3f, 6f));
		}

		// Token: 0x06001417 RID: 5143 RVA: 0x0008FB38 File Offset: 0x0008DD38
		public override bool CanSendShipToPlayerClan(Ship ship, int playerShipsCount, int troopsCountToSend, out TextObject hint)
		{
			hint = TextObject.GetEmpty();
			bool flag = true;
			if (NavalStorylineData.IsNavalStoryLineActive())
			{
				hint = new TextObject("{=lwbwTg5b}You can't perform this action during this time.", null);
				flag = false;
			}
			else if (!ship.IsTradeable || ship.IsUsedByQuest)
			{
				hint = GameTexts.FindText("str_port_cant_take_action_quest_ship", null);
				flag = false;
			}
			else if (playerShipsCount == 1 && MobileParty.MainParty.IsCurrentlyAtSea)
			{
				hint = GameTexts.FindText("str_cannot_give_all_ships", null);
				flag = false;
			}
			else if (LinQuick.AllQ<WarPartyComponent>(Clan.PlayerClan.WarPartyComponents, (WarPartyComponent x) => !NavalDLCManager.Instance.GameModels.ShipDistributionModel.CanSendShipToParty(ship, x.MobileParty)))
			{
				hint = new TextObject("{=SwV5iZbN}There are no suitable parties in your clan to send ships to.", null);
				flag = false;
			}
			else if (MobileParty.MainParty.MemberRoster.TotalRegulars - troopsCountToSend < Campaign.Current.Models.FleetManagementModel.MinimumTroopCountRequiredToSendShips)
			{
				hint = new TextObject("{=U4avdcnH}You need at least {NUMBER} troops to send with the ship.", null);
				hint.SetTextVariable("NUMBER", Campaign.Current.Models.FleetManagementModel.MinimumTroopCountRequiredToSendShips);
				flag = false;
			}
			else if (MobileParty.MainParty.MapEvent != null && MobileParty.MainParty.MapEvent.PlayerSide != MobileParty.MainParty.MapEvent.WinningSide)
			{
				hint = GameTexts.FindText("str_action_disabled_reason_encounter", null);
				flag = false;
			}
			else
			{
				hint = new TextObject("{=iRfrlsB8}{NUMBER} troops will spend {DAYS} {?DAYS > 1}days{?}day{\\?} to deliver the ship and return to your party.", null);
				hint.SetTextVariable("NUMBER", Campaign.Current.Models.FleetManagementModel.MinimumTroopCountRequiredToSendShips);
				int num = MathF.Round(Campaign.Current.Models.FleetManagementModel.GetReturnTimeForTroops(ship).RemainingDaysFromNow);
				hint.SetTextVariable("DAYS", num);
			}
			return flag;
		}
	}
}
