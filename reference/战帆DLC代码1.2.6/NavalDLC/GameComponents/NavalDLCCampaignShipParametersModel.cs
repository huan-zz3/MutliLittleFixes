using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace NavalDLC.GameComponents
{
	// Token: 0x0200010E RID: 270
	public class NavalDLCCampaignShipParametersModel : CampaignShipParametersModel
	{
		// Token: 0x0600138A RID: 5002 RVA: 0x0008D708 File Offset: 0x0008B908
		public override float GetDefaultCombatFactor(ShipHull shipHull)
		{
			switch (shipHull.Type)
			{
			case 0:
				return 1f;
			case 1:
				return 1.2f;
			case 2:
				return 1.4f;
			default:
				Debug.FailedAssert("Unhandled ship type", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\GameComponents\\NavalDLCCampaignShipParametersModel.cs", "GetDefaultCombatFactor", 25);
				return 1f;
			}
		}

		// Token: 0x0600138B RID: 5003 RVA: 0x0008D760 File Offset: 0x0008B960
		public override float GetShipSizeWeatherFactor(ShipHull shipHull)
		{
			switch (shipHull.Type)
			{
			case 0:
				return 35f;
			case 1:
				return 70f;
			case 2:
				return 105f;
			default:
				Debug.FailedAssert("Unhandled ship type", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\GameComponents\\NavalDLCCampaignShipParametersModel.cs", "GetShipSizeWeatherFactor", 41);
				return 0.1f;
			}
		}

		// Token: 0x0600138C RID: 5004 RVA: 0x0008D7B8 File Offset: 0x0008B9B8
		public override float GetCampaignSpeedBonusFactor(Ship ship)
		{
			float num = 0f;
			foreach (KeyValuePair<string, ShipSlot> keyValuePair in ship.ShipHull.AvailableSlots)
			{
				ShipUpgradePiece pieceAtSlot = ship.GetPieceAtSlot(keyValuePair.Key);
				if (pieceAtSlot != null && pieceAtSlot.CampaignSpeedBonusMultiplier > 0f)
				{
					num += pieceAtSlot.CampaignSpeedBonusMultiplier;
				}
			}
			if (ship.Figurehead != null && ship.Figurehead == DefaultFigureheads.Horse)
			{
				num += ship.Figurehead.EffectAmount;
			}
			return num;
		}

		// Token: 0x0600138D RID: 5005 RVA: 0x0008D85C File Offset: 0x0008BA5C
		public override float GetCrewCapacityBonusFactor(Ship ship)
		{
			float num = 0f;
			foreach (KeyValuePair<string, ShipSlot> keyValuePair in ship.ShipHull.AvailableSlots)
			{
				ShipUpgradePiece pieceAtSlot = ship.GetPieceAtSlot(keyValuePair.Key);
				if (pieceAtSlot != null)
				{
					num += pieceAtSlot.CrewCapacityBonusMultiplier;
				}
			}
			return num;
		}

		// Token: 0x0600138E RID: 5006 RVA: 0x0008D8D0 File Offset: 0x0008BAD0
		public override float GetShipWeightFactor(Ship ship)
		{
			float num = 0f;
			foreach (KeyValuePair<string, ShipSlot> keyValuePair in ship.ShipHull.AvailableSlots)
			{
				ShipUpgradePiece pieceAtSlot = ship.GetPieceAtSlot(keyValuePair.Key);
				if (pieceAtSlot != null)
				{
					num += pieceAtSlot.ShipWeightBonusMultiplier;
				}
			}
			return num;
		}

		// Token: 0x0600138F RID: 5007 RVA: 0x0008D944 File Offset: 0x0008BB44
		public override float GetForwardDragFactor(Ship ship)
		{
			float num = 0f;
			foreach (KeyValuePair<string, ShipSlot> keyValuePair in ship.ShipHull.AvailableSlots)
			{
				ShipUpgradePiece pieceAtSlot = ship.GetPieceAtSlot(keyValuePair.Key);
				if (pieceAtSlot != null)
				{
					num += pieceAtSlot.DecreaseForwardDragMultiplier;
				}
			}
			return -num;
		}

		// Token: 0x06001390 RID: 5008 RVA: 0x0008D9B8 File Offset: 0x0008BBB8
		public override float GetCrewShieldHitPointsFactor(Ship ship)
		{
			float num = 0f;
			foreach (KeyValuePair<string, ShipSlot> keyValuePair in ship.ShipHull.AvailableSlots)
			{
				ShipUpgradePiece pieceAtSlot = ship.GetPieceAtSlot(keyValuePair.Key);
				if (pieceAtSlot != null)
				{
					num += pieceAtSlot.CrewShieldHitPointsBonusMultiplier;
				}
			}
			if (ship.Figurehead == DefaultFigureheads.Turtle)
			{
				num += ship.Figurehead.EffectAmount;
			}
			return num;
		}

		// Token: 0x06001391 RID: 5009 RVA: 0x0008DA48 File Offset: 0x0008BC48
		public override int GetAdditionalAmmoBonus(Ship ship)
		{
			int num = 0;
			foreach (KeyValuePair<string, ShipSlot> keyValuePair in ship.ShipHull.AvailableSlots)
			{
				ShipUpgradePiece pieceAtSlot = ship.GetPieceAtSlot(keyValuePair.Key);
				if (pieceAtSlot != null)
				{
					num += pieceAtSlot.AdditionalAmmoBonus;
				}
			}
			return num;
		}

		// Token: 0x06001392 RID: 5010 RVA: 0x0008DAB8 File Offset: 0x0008BCB8
		public override float GetMaxOarPowerFactor(Ship ship)
		{
			float num = 0f;
			foreach (KeyValuePair<string, ShipSlot> keyValuePair in ship.ShipHull.AvailableSlots)
			{
				ShipUpgradePiece pieceAtSlot = ship.GetPieceAtSlot(keyValuePair.Key);
				if (pieceAtSlot != null)
				{
					num += pieceAtSlot.MaxOarPowerBonusMultiplier;
				}
			}
			return num;
		}

		// Token: 0x06001393 RID: 5011 RVA: 0x0008DB2C File Offset: 0x0008BD2C
		public override float GetMaxOarForceFactor(Ship ship)
		{
			float num = 0f;
			foreach (KeyValuePair<string, ShipSlot> keyValuePair in ship.ShipHull.AvailableSlots)
			{
				ShipUpgradePiece pieceAtSlot = ship.GetPieceAtSlot(keyValuePair.Key);
				if (pieceAtSlot != null)
				{
					num += pieceAtSlot.MaxOarForceBonusMultiplier;
				}
			}
			if (ship.Figurehead == DefaultFigureheads.Deer)
			{
				num += ship.Figurehead.EffectAmount;
			}
			return num;
		}

		// Token: 0x06001394 RID: 5012 RVA: 0x0008DBBC File Offset: 0x0008BDBC
		public override float GetSailForceFactor(Ship ship)
		{
			float num = 0f;
			foreach (KeyValuePair<string, ShipSlot> keyValuePair in ship.ShipHull.AvailableSlots)
			{
				ShipUpgradePiece pieceAtSlot = ship.GetPieceAtSlot(keyValuePair.Key);
				if (pieceAtSlot != null)
				{
					num += pieceAtSlot.SailForceBonusMultiplier;
				}
			}
			if (ship.Figurehead == DefaultFigureheads.Swan)
			{
				num += ship.Figurehead.EffectAmount;
			}
			return num;
		}

		// Token: 0x06001395 RID: 5013 RVA: 0x0008DC4C File Offset: 0x0008BE4C
		public override float GetCrewMeleeDamageFactor(Ship ship)
		{
			float num = 0f;
			foreach (KeyValuePair<string, ShipSlot> keyValuePair in ship.ShipHull.AvailableSlots)
			{
				ShipUpgradePiece pieceAtSlot = ship.GetPieceAtSlot(keyValuePair.Key);
				if (pieceAtSlot != null)
				{
					num += pieceAtSlot.CrewMeleeDamageBonusMultiplier;
				}
			}
			return num;
		}

		// Token: 0x06001396 RID: 5014 RVA: 0x0008DCC0 File Offset: 0x0008BEC0
		public override int GetAdditionalArcherQuivers(Ship ship)
		{
			int num = 0;
			foreach (KeyValuePair<string, ShipSlot> keyValuePair in ship.ShipHull.AvailableSlots)
			{
				ShipUpgradePiece pieceAtSlot = ship.GetPieceAtSlot(keyValuePair.Key);
				if (pieceAtSlot != null)
				{
					num += pieceAtSlot.ArcherQuiverBonus;
				}
			}
			return num;
		}

		// Token: 0x06001397 RID: 5015 RVA: 0x0008DD30 File Offset: 0x0008BF30
		public override int GetAdditionalThrowingWeaponStack(Ship ship)
		{
			int num = 0;
			foreach (KeyValuePair<string, ShipSlot> keyValuePair in ship.ShipHull.AvailableSlots)
			{
				ShipUpgradePiece pieceAtSlot = ship.GetPieceAtSlot(keyValuePair.Key);
				if (pieceAtSlot != null)
				{
					num += pieceAtSlot.ThrowingWeaponStackBonus;
				}
			}
			return num;
		}

		// Token: 0x06001398 RID: 5016 RVA: 0x0008DDA0 File Offset: 0x0008BFA0
		public override float GetSailRotationSpeedFactor(Ship ship)
		{
			float num = 0f;
			foreach (KeyValuePair<string, ShipSlot> keyValuePair in ship.ShipHull.AvailableSlots)
			{
				ShipUpgradePiece pieceAtSlot = ship.GetPieceAtSlot(keyValuePair.Key);
				if (pieceAtSlot != null)
				{
					num += pieceAtSlot.SailRotationSpeedBonusMultiplier;
				}
			}
			return num;
		}

		// Token: 0x06001399 RID: 5017 RVA: 0x0008DE14 File Offset: 0x0008C014
		public override float GetFurlUnfurlSpeedFactor(Ship ship)
		{
			float num = 0f;
			foreach (KeyValuePair<string, ShipSlot> keyValuePair in ship.ShipHull.AvailableSlots)
			{
				ShipUpgradePiece pieceAtSlot = ship.GetPieceAtSlot(keyValuePair.Key);
				if (pieceAtSlot != null)
				{
					num += pieceAtSlot.FurlUnfurlSpeedBonusMultiplier;
				}
			}
			return num;
		}
	}
}
