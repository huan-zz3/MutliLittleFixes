using System;
using System.Collections.Generic;
using NavalDLC.Settlements.Building;
using NavalDLC.Storyline;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace NavalDLC
{
	// Token: 0x0200001E RID: 30
	public static class NavalDLCExtensions
	{
		// Token: 0x06000138 RID: 312 RVA: 0x00008CDB File Offset: 0x00006EDB
		public static bool IsFishingParty(this MobileParty party)
		{
			return party.PartyComponent is FishingPartyComponent;
		}

		// Token: 0x06000139 RID: 313 RVA: 0x00008CEC File Offset: 0x00006EEC
		public static MBReadOnlyList<FishingPartyComponent> FishingParties(this Village village)
		{
			List<FishingPartyComponent> list;
			if (!NavalDLCManager.Instance.FishingParties.TryGetValue(village, out list))
			{
				list = new List<FishingPartyComponent>();
			}
			return new MBReadOnlyList<FishingPartyComponent>(list);
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00008D19 File Offset: 0x00006F19
		public static bool IsPirate(this CharacterObject characterObject)
		{
			return characterObject.IsMariner && !characterObject.IsHero && characterObject.Occupation == 15;
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00008D38 File Offset: 0x00006F38
		public static Building GetShipyard(this Town town)
		{
			foreach (Building building in town.Buildings)
			{
				if (building.BuildingType == NavalBuildingTypes.SettlementShipyard)
				{
					return building;
				}
			}
			return null;
		}

		// Token: 0x0600013C RID: 316 RVA: 0x00008D98 File Offset: 0x00006F98
		public static List<ShipUpgradePiece> GetAvailableShipUpgradePieces(this Town town)
		{
			List<ShipUpgradePiece> list = new List<ShipUpgradePiece>();
			List<ShipUpgradePiece> objectTypeList = MBObjectManager.Instance.GetObjectTypeList<ShipUpgradePiece>();
			CultureObject culture = town.Culture;
			Building shipyard = town.GetShipyard();
			int num = ((shipyard != null) ? shipyard.CurrentLevel : 0);
			foreach (ShipUpgradePiece shipUpgradePiece in objectTypeList)
			{
				if (!shipUpgradePiece.NotMerchandise && shipUpgradePiece.RequiredPortLevel <= num && ((shipUpgradePiece.RequiredCulture1 == null && shipUpgradePiece.RequiredCulture2 == null) || culture == shipUpgradePiece.RequiredCulture1 || culture == shipUpgradePiece.RequiredCulture2))
				{
					list.Add(shipUpgradePiece);
				}
			}
			return list;
		}

		// Token: 0x0600013D RID: 317 RVA: 0x00008E50 File Offset: 0x00007050
		public static bool IsNavalStorylineQuestParty(this PartyBase party, out NavalStorylinePartyData partyData)
		{
			partyData = new NavalStorylinePartyData();
			NavalDLCEvents.Instance.IsNavalQuestParty(party, partyData);
			return partyData.IsQuestParty;
		}

		// Token: 0x0600013E RID: 318 RVA: 0x00008E70 File Offset: 0x00007070
		public static bool IsNavalStorylineQuestParty(this PartyBase party)
		{
			NavalStorylinePartyData navalStorylinePartyData;
			return party.IsNavalStorylineQuestParty(out navalStorylinePartyData);
		}

		// Token: 0x0600013F RID: 319 RVA: 0x00008E85 File Offset: 0x00007085
		public static bool IsNavalStorylineQuestParty(this MobileParty mobileParty, out NavalStorylinePartyData partyData)
		{
			return mobileParty.Party.IsNavalStorylineQuestParty(out partyData);
		}

		// Token: 0x06000140 RID: 320 RVA: 0x00008E94 File Offset: 0x00007094
		public static bool IsNavalStorylineQuestParty(this MobileParty mobileParty)
		{
			NavalStorylinePartyData navalStorylinePartyData;
			return mobileParty.IsNavalStorylineQuestParty(out navalStorylinePartyData);
		}
	}
}
