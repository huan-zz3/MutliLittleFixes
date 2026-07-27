using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.View
{
	// Token: 0x02000005 RID: 5
	public class NavalDLCViewCheats
	{
		// Token: 0x0600002E RID: 46 RVA: 0x00002F70 File Offset: 0x00001170
		[CommandLineFunctionality.CommandLineArgumentFunction("focus_player_anchor", "naval")]
		public static string FocusPlayerAnchor(List<string> strings)
		{
			string empty = string.Empty;
			if (!NavalDLCCheats.CheckCheatUsage(ref empty))
			{
				return empty;
			}
			if (CampaignCheats.CheckHelp(strings))
			{
				return "Format is \"naval.focus_player_anchor\".";
			}
			if (!MobileParty.MainParty.Anchor.IsValid)
			{
				return "Anchor is not valid";
			}
			MapScreen.Instance.FastMoveCameraToPosition(MobileParty.MainParty.Anchor.Position);
			return "Success";
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002FD4 File Offset: 0x000011D4
		[CommandLineFunctionality.CommandLineArgumentFunction("focus_ship", "naval")]
		public static string FocusShip(List<string> strings)
		{
			if (!CampaignCheats.CheckCheatUsage(ref CampaignCheats.ErrorType))
			{
				return CampaignCheats.ErrorType;
			}
			string text = "Format is \"naval.focus_ship [ShipHullStringId/ShipHullName]\".";
			if (CampaignCheats.CheckParameters(strings, 0) || CampaignCheats.CheckHelp(strings))
			{
				return text;
			}
			string text2 = CampaignCheats.ConcatenateString(strings);
			ShipHull shipHull = MBObjectManager.Instance.GetObject<ShipHull>(text2);
			if (shipHull == null)
			{
				foreach (ShipHull shipHull2 in MBObjectManager.Instance.GetObjectTypeList<ShipHull>())
				{
					if (string.Equals(shipHull2.Name.ToString().ToLower(), text2.ToLower(), StringComparison.OrdinalIgnoreCase))
					{
						shipHull = shipHull2;
						break;
					}
				}
			}
			if (shipHull != null)
			{
				string shipHullStringId = shipHull.StringId;
				Predicate<Ship> <>9__1;
				Town town = Town.AllTowns.FirstOrDefault<Town>(delegate(Town x)
				{
					List<Ship> availableShips = x.AvailableShips;
					Predicate<Ship> predicate;
					if ((predicate = <>9__1) == null)
					{
						predicate = (<>9__1 = (Ship y) => y.ShipHull.StringId == shipHullStringId);
					}
					return availableShips.Exists(predicate);
				});
				if (town != null)
				{
					town.AvailableShips.First<Ship>((Ship x) => x.ShipHull.StringId == shipHullStringId);
					MapScreen.Instance.MapCameraView.SetCameraMode(1);
					town.Settlement.Party.SetAsCameraFollowParty();
					return "Success! Found in " + town.Name.ToString();
				}
			}
			return "Ship is not found : " + text2 + "\n" + text;
		}
	}
}
