using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.Storyline;
using StoryMode;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace NavalDLC
{
	// Token: 0x0200001F RID: 31
	public static class NavalDLCHelpers
	{
		// Token: 0x06000141 RID: 321 RVA: 0x00008EAC File Offset: 0x000070AC
		public static ExplainedNumber GetAveragePartySizeLimitFromTemplate(PartyTemplateObject templateObject)
		{
			int num = 0;
			foreach (PartyTemplateStack partyTemplateStack in templateObject.Stacks)
			{
				num += (partyTemplateStack.MaxValue + partyTemplateStack.MinValue) / 2;
			}
			return new ExplainedNumber((float)num, false, null);
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00008F18 File Offset: 0x00007118
		public static ExplainedNumber GetMaxPartySizeLimitFromTemplate(PartyTemplateObject templateObject)
		{
			int num = 0;
			foreach (PartyTemplateStack partyTemplateStack in templateObject.Stacks)
			{
				num += partyTemplateStack.MaxValue;
			}
			return new ExplainedNumber((float)num, false, null);
		}

		// Token: 0x06000143 RID: 323 RVA: 0x00008F78 File Offset: 0x00007178
		public static List<Ship> GetSetPieceBattleShips(PartyTemplateObject template, PartyBase party)
		{
			List<Ship> list = party.Ships.Where<Ship>((Ship s) => s.IsUsedByQuest).ToList<Ship>();
			int num = 0;
			foreach (ShipTemplateStack shipTemplateStack in template.ShipHulls)
			{
				num += shipTemplateStack.MaxValue;
			}
			int num2 = num - list.Count<Ship>();
			if (num2 > 0)
			{
				foreach (Ship ship in (from s in party.Ships
					where !s.IsUsedByQuest
					orderby s.FlagshipScore descending
					select s).ToList<Ship>())
				{
					if (num2 <= 0)
					{
						break;
					}
					list.Add(ship);
					num2--;
				}
			}
			return list;
		}

		// Token: 0x06000144 RID: 324 RVA: 0x000090AC File Offset: 0x000072AC
		public static bool IsNavalRaidMissionOpen()
		{
			return Mission.Current != null && Mission.Current.IsNavalRaidBattle;
		}

		// Token: 0x06000145 RID: 325 RVA: 0x000090C4 File Offset: 0x000072C4
		public static bool IsShipOrdersAvailable()
		{
			if (Mission.Current == null || !Mission.Current.IsNavalBattle)
			{
				return false;
			}
			Team playerTeam = Mission.Current.PlayerTeam;
			if (((playerTeam != null) ? playerTeam.PlayerOrderController : null) == null)
			{
				return false;
			}
			if (Mission.Current.GetMissionBehavior<NavalShipsLogic>() == null)
			{
				return false;
			}
			MBReadOnlyList<Formation> selectedFormations = Mission.Current.PlayerTeam.PlayerOrderController.SelectedFormations;
			if (selectedFormations == null)
			{
				return false;
			}
			for (int i = 0; i < selectedFormations.Count; i++)
			{
				if (NavalDLCHelpers.IsPlayerCaptainOfFormationShip(selectedFormations[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000146 RID: 326 RVA: 0x0000914A File Offset: 0x0000734A
		public static bool IsPlayerCaptainOfFormationShip(Formation formation)
		{
			return NavalDLCHelpers.IsAgentCaptainOfFormationShip(Agent.Main, formation);
		}

		// Token: 0x06000147 RID: 327 RVA: 0x00009158 File Offset: 0x00007358
		public static bool IsAgentCaptainOfFormationShip(Agent agent, Formation formation)
		{
			Mission mission = Mission.Current;
			NavalShipsLogic navalShipsLogic = ((mission != null) ? mission.GetMissionBehavior<NavalShipsLogic>() : null);
			MissionShip missionShip;
			return navalShipsLogic != null && navalShipsLogic.GetShip(formation.Team.TeamSide, formation.FormationIndex, out missionShip) && ((agent != null && missionShip.Captain == agent) || (agent != null && agent.IsPlayerControlled && missionShip.Formation.Team.IsPlayerTeam && missionShip.Formation.Index == 0));
		}

		// Token: 0x06000148 RID: 328 RVA: 0x000091D4 File Offset: 0x000073D4
		public static void SetCustomSailPatternOfPartyShips(MobileParty party, string sailId)
		{
			foreach (Ship ship in party.Ships)
			{
				ship.CustomSailPatternId = sailId;
			}
			MobileParty.MainParty.SetNavalVisualAsDirty();
		}

		// Token: 0x06000149 RID: 329 RVA: 0x00009230 File Offset: 0x00007430
		public static void AddUpgradePiecesToPartyShips(MobileParty party, Dictionary<string, string> upgradePiecesBySlot, Figurehead figurehead = null)
		{
			foreach (Ship ship in party.Ships)
			{
				foreach (KeyValuePair<string, string> keyValuePair in upgradePiecesBySlot)
				{
					if (ship.HasSlot(keyValuePair.Key))
					{
						ship.EquipUpgradePiece(keyValuePair.Key, MBObjectManager.Instance.GetObject<ShipUpgradePiece>(keyValuePair.Value));
					}
				}
				if (figurehead != null)
				{
					ship.ChangeFigurehead(figurehead);
				}
			}
		}

		// Token: 0x0600014A RID: 330 RVA: 0x000092EC File Offset: 0x000074EC
		public static void AddSisterToClan()
		{
			StoryModeHeroes.LittleSister.Clan = Clan.PlayerClan;
			if (StoryModeHeroes.LittleSister.Age >= (float)Campaign.Current.Models.AgeModel.HeroComesOfAge)
			{
				Town town = SettlementHelper.FindNearestTownToMobileParty(MobileParty.MainParty, 3, (Settlement s) => s.OwnerClan.MapFaction == Clan.PlayerClan.MapFaction);
				Settlement settlement = ((town != null) ? town.Settlement : null);
				if (settlement == null)
				{
					Town town2 = SettlementHelper.FindNearestTownToMobileParty(MobileParty.MainParty, 3, (Settlement s) => !Clan.PlayerClan.MapFaction.IsAtWarWith(s.OwnerClan.MapFaction));
					settlement = ((town2 != null) ? town2.Settlement : null);
				}
				if (settlement == null)
				{
					settlement = SettlementHelper.FindRandomSettlement((Settlement s) => s.IsTown);
				}
				if (Settlement.CurrentSettlement == settlement)
				{
					TeleportHeroAction.ApplyImmediateTeleportToSettlement(StoryModeHeroes.LittleSister, settlement);
				}
				else
				{
					TeleportHeroAction.ApplyDelayedTeleportToSettlement(StoryModeHeroes.LittleSister, settlement);
				}
				StoryModeHelpers.SetPlayerSiblingsSkillsIfNeeded(StoryModeHeroes.LittleSister);
			}
			else
			{
				StoryModeHeroes.LittleSister.ChangeState(0);
			}
			StoryModeHeroes.LittleSister.UpdateLastKnownClosestSettlement(NavalStorylineData.HomeSettlement);
			TextObject textObject = new TextObject("{=7XTkTi9B}{PLAYER_LITTLE_SISTER.NAME} is the little sister of {PLAYER.LINK}.", null);
			StringHelpers.SetCharacterProperties("PLAYER_LITTLE_SISTER", StoryModeHeroes.LittleSister.CharacterObject, textObject, false);
			StringHelpers.SetCharacterProperties("PLAYER", CharacterObject.PlayerCharacter, textObject, false);
			StoryModeHeroes.LittleSister.EncyclopediaText = textObject;
		}
	}
}
