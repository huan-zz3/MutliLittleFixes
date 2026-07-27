using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000137 RID: 311
	public class NavalDLCShipStatModel : ShipStatModel
	{
		// Token: 0x06001510 RID: 5392 RVA: 0x00094A57 File Offset: 0x00092C57
		public override float GetShipFlagshipScore(Ship ship)
		{
			return this.GetShipTierf(ship) * MathF.Max(0.1f, ship.HitPoints / ship.MaxHitPoints);
		}

		// Token: 0x06001511 RID: 5393 RVA: 0x00094A78 File Offset: 0x00092C78
		private float GetShipTierf(Ship ship)
		{
			int num = ship.ShipHull.Value;
			foreach (KeyValuePair<string, ShipSlot> keyValuePair in ship.ShipHull.AvailableSlots)
			{
				ShipUpgradePiece pieceAtSlot = ship.GetPieceAtSlot(keyValuePair.Key);
				if (pieceAtSlot != null)
				{
					if (ship.ShipHull.Type == null)
					{
						num += pieceAtSlot.LightValue;
					}
					else if (ship.ShipHull.Type == 1)
					{
						num += pieceAtSlot.MediumValue;
					}
					else
					{
						num += pieceAtSlot.HeavyValue;
					}
				}
			}
			if (ship.Figurehead != null)
			{
				num += 15000;
			}
			return (float)num;
		}
	}
}
