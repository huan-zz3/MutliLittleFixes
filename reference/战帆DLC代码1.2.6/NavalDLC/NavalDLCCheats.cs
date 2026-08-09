using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.ObjectSystem;

namespace NavalDLC
{
	// Token: 0x0200001C RID: 28
	public static class NavalDLCCheats
	{
		// Token: 0x0600011E RID: 286 RVA: 0x00008578 File Offset: 0x00006778
		public static bool CheckCheatUsage(ref string message)
		{
			if (!CampaignCheats.CheckCheatUsage(ref message))
			{
				return false;
			}
			ModuleInfo moduleInfo = ModuleHelper.GetModuleInfo("NavalDLC");
			if (moduleInfo == null || !moduleInfo.IsActive)
			{
				message = "DLC is not active.";
				return false;
			}
			return true;
		}

		// Token: 0x0600011F RID: 287 RVA: 0x000085B0 File Offset: 0x000067B0
		[CommandLineFunctionality.CommandLineArgumentFunction("damage_player_ships", "naval")]
		public static string DamagePlayerShips(List<string> strings)
		{
			string empty = string.Empty;
			if (!NavalDLCCheats.CheckCheatUsage(ref empty))
			{
				return empty;
			}
			MobileParty mainParty = MobileParty.MainParty;
			MBReadOnlyList<Ship> mbreadOnlyList = ((mainParty != null) ? mainParty.Ships : null);
			if (mbreadOnlyList == null || mbreadOnlyList.Count == 0)
			{
				return "Player does not have any ships";
			}
			float num = 0.5f;
			if (strings.Count == 1 && float.TryParse(strings[0], out num) && (MBMath.ApproximatelyEqualsTo(num, 0f, 1E-05f) || num < 0f))
			{
				num = 0.5f;
			}
			for (int i = mbreadOnlyList.Count - 1; i >= 0; i--)
			{
				Ship ship = mbreadOnlyList[i];
				float num2;
				ship.OnShipDamaged(ship.MaxHitPoints * num, null, ref num2);
			}
			return "All ship hit points are reduced";
		}

		// Token: 0x06000120 RID: 288 RVA: 0x00008660 File Offset: 0x00006860
		[CommandLineFunctionality.CommandLineArgumentFunction("add_ship_to_player", "naval")]
		public static string AddShipToPlayer(List<string> strings)
		{
			string empty = string.Empty;
			if (!NavalDLCCheats.CheckCheatUsage(ref empty))
			{
				return empty;
			}
			string text = "Format is \"naval.add_ship_to_player [ShipName] | [Count] - (Empty = Random 1 Ship)\".";
			if (CampaignCheats.CheckHelp(strings))
			{
				return text;
			}
			if (!CampaignCheats.IsPartySuitableToUseCheat(PartyBase.MainParty, false))
			{
				return "Main party not suitable to take ship right now";
			}
			List<string> separatedNames = CampaignCheats.GetSeparatedNames(strings, true);
			MBList<ShipHull> shipHulls = Extensions.ToMBList<ShipHull>(Kingdom.All.SelectMany<Kingdom, ShipHull>((Kingdom x) => x.Culture.AvailableShipHulls));
			string text2 = string.Empty;
			int num = 1;
			ShipHull shipHull = null;
			if (separatedNames.Count == 0 || string.IsNullOrEmpty(separatedNames[0]))
			{
				shipHull = Extensions.GetRandomElement<ShipHull>(shipHulls);
			}
			else if (separatedNames.Count == 1)
			{
				text2 = separatedNames[0];
			}
			else
			{
				if (separatedNames.Count != 2)
				{
					return text;
				}
				text2 = separatedNames[0];
				int.TryParse(separatedNames[1], out num);
			}
			if (num <= 0 || num > 33)
			{
				return string.Format("Ship count must between 0-{0}", 33);
			}
			string text3;
			if (shipHull != null || CampaignCheats.TryGetObject<ShipHull>(text2, ref shipHull, ref text3, (ShipHull x) => shipHulls.Contains(x)))
			{
				for (int i = 0; i < num; i++)
				{
					Ship ship = new Ship(shipHull);
					ChangeShipOwnerAction.ApplyByLooting(PartyBase.MainParty, ship);
				}
				if (!MobileParty.MainParty.IsCurrentlyAtSea && !MobileParty.MainParty.Anchor.IsValid)
				{
					MobileParty.MainParty.Anchor.SetSettlement(NavalDLCCheats.FindAnchorSettlementForParty(MobileParty.MainParty));
				}
				return string.Format("{0} {1} were added to main party.", num, shipHull.Name);
			}
			return text3 + "    " + text;
		}

		// Token: 0x06000121 RID: 289 RVA: 0x00008810 File Offset: 0x00006A10
		public static Settlement FindAnchorSettlementForParty(MobileParty party)
		{
			IEnumerable<Town> enumerable = Town.AllTowns.Where<Town>((Town x) => x.Settlement.HasPort && !party.MapFaction.IsAtWarWith(x.MapFaction));
			if (Extensions.IsEmpty<Town>(enumerable))
			{
				enumerable = Town.AllTowns.Where<Town>((Town x) => x.Settlement.HasPort);
			}
			return Extensions.MinBy<Town, float>(enumerable, (Town x) => x.Settlement.PortPosition.Distance(party.Position)).Settlement;
		}

		// Token: 0x06000122 RID: 290 RVA: 0x0000888C File Offset: 0x00006A8C
		[CommandLineFunctionality.CommandLineArgumentFunction("unlock_figurehead", "naval")]
		public static string UnlockFigurehead(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			string text = "Format is \"naval.unlock_figurehead [figurehead_id or all]\".";
			if (!CampaignCheats.CheckParameters(strings, 1) || CampaignCheats.CheckHelp(strings))
			{
				return text;
			}
			if (!CampaignCheats.IsPartySuitableToUseCheat(PartyBase.MainParty, false))
			{
				return "Main party not suitable to take figurehead right now";
			}
			string text2 = strings[0];
			if (string.Equals(text2, "all", StringComparison.OrdinalIgnoreCase))
			{
				foreach (Figurehead figurehead in MBObjectManager.Instance.GetObjectTypeList<Figurehead>())
				{
					if (!Campaign.Current.UnlockedFigureheadsByMainHero.Contains(figurehead))
					{
						Campaign.Current.UnlockFigurehead(figurehead);
					}
				}
				return "All figureheads unlocked for the player";
			}
			Figurehead figurehead2;
			string text3;
			if (!CampaignCheats.TryGetObject<Figurehead>(text2, ref figurehead2, ref text3, null))
			{
				return "Figurehead with id " + text2 + " does not exist.";
			}
			if (!Campaign.Current.UnlockedFigureheadsByMainHero.Contains(figurehead2))
			{
				Campaign.Current.UnlockFigurehead(figurehead2);
				return string.Format("Figurehead {0} is unlocked", figurehead2.Name);
			}
			return "This figurehead already unlocked by the player";
		}

		// Token: 0x06000123 RID: 291 RVA: 0x000089A8 File Offset: 0x00006BA8
		[CommandLineFunctionality.CommandLineArgumentFunction("list_all_ship_names", "naval")]
		public static string ListAllShipNames(List<string> strings)
		{
			string empty = string.Empty;
			if (!NavalDLCCheats.CheckCheatUsage(ref empty))
			{
				return empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ShipHull shipHull in MBObjectManager.Instance.GetObjects<ShipHull>((ShipHull x) => x != null))
			{
				stringBuilder.AppendLine(shipHull.Name.ToString() + "   -   " + shipHull.StringId);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000124 RID: 292 RVA: 0x00008A58 File Offset: 0x00006C58
		[CommandLineFunctionality.CommandLineArgumentFunction("list_all_figurehead_names", "naval")]
		public static string ListAllFigureheads(List<string> strings)
		{
			string empty = string.Empty;
			if (!NavalDLCCheats.CheckCheatUsage(ref empty))
			{
				return empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Figurehead figurehead in MBObjectManager.Instance.GetObjects<Figurehead>((Figurehead x) => x != null))
			{
				stringBuilder.AppendLine(figurehead.Name.ToString() + "   -   " + figurehead.StringId);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04000089 RID: 137
		public const string DLCNotActive = "DLC is not active.";
	}
}
