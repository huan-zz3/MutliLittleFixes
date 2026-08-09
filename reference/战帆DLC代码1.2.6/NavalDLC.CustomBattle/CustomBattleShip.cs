using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.CustomBattle
{
	// Token: 0x02000002 RID: 2
	public class CustomBattleShip : IShipOrigin
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		public ShipHull Hull
		{
			get
			{
				return this._shipHull;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000002 RID: 2 RVA: 0x00002050 File Offset: 0x00000250
		public TextObject Name
		{
			get
			{
				return this._shipHull.Name;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000003 RID: 3 RVA: 0x0000205D File Offset: 0x0000025D
		public string OriginShipId
		{
			get
			{
				return this._shipHull.MissionShipObjectId;
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000004 RID: 4 RVA: 0x0000206A File Offset: 0x0000026A
		public bool IsPlayerShip
		{
			get
			{
				return this._isPlayerShip;
			}
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000005 RID: 5 RVA: 0x00002072 File Offset: 0x00000272
		// (set) Token: 0x06000006 RID: 6 RVA: 0x0000207C File Offset: 0x0000027C
		public float HitPoints
		{
			get
			{
				return this._remainingHitPoints;
			}
			set
			{
				float num = MathF.Clamp(value, 0f, this.MaxHitPoints);
				this._remainingHitPoints = num;
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000007 RID: 7 RVA: 0x000020A4 File Offset: 0x000002A4
		public float MaxHitPoints
		{
			get
			{
				float num = 1f;
				foreach (ShipUpgradePiece shipUpgradePiece in this._shipPieces.Values)
				{
					if (shipUpgradePiece != null)
					{
						num += shipUpgradePiece.MaxHitPointsBonusMultiplier;
					}
				}
				return (float)this._shipHull.MaxHitPoints * num;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000008 RID: 8 RVA: 0x00002118 File Offset: 0x00000318
		// (set) Token: 0x06000009 RID: 9 RVA: 0x00002120 File Offset: 0x00000320
		public float FireHitPoints
		{
			get
			{
				return this._remainingFireHitPoints;
			}
			set
			{
				float num = MathF.Clamp(value, 0f, this.MaxFireHitPoints);
				this._remainingFireHitPoints = num;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600000A RID: 10 RVA: 0x00002148 File Offset: 0x00000348
		public float MaxFireHitPoints
		{
			get
			{
				float num = 1f;
				return (float)this._shipHull.MaxFireHitPoints * num;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600000B RID: 11 RVA: 0x00002169 File Offset: 0x00000369
		// (set) Token: 0x0600000C RID: 12 RVA: 0x00002174 File Offset: 0x00000374
		public float SailHitPoints
		{
			get
			{
				return this._remainingSailHitPoints;
			}
			set
			{
				float num = MathF.Clamp(value, 0f, this.MaxSailHitPoints);
				this._remainingSailHitPoints = num;
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600000D RID: 13 RVA: 0x0000219C File Offset: 0x0000039C
		public float MaxSailHitPoints
		{
			get
			{
				float num = 1f;
				foreach (ShipUpgradePiece shipUpgradePiece in this._shipPieces.Values)
				{
					if (shipUpgradePiece != null)
					{
						num += shipUpgradePiece.MaxSailHitPointsBonusMultiplier;
					}
				}
				return (float)this._shipHull.MaxSailHitPoints * num;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600000E RID: 14 RVA: 0x00002210 File Offset: 0x00000410
		public int TotalCrewCapacity
		{
			get
			{
				int totalCrewCapacity = this._shipHull.TotalCrewCapacity;
				float num = 1f;
				foreach (ShipUpgradePiece shipUpgradePiece in this._shipPieces.Values)
				{
					if (shipUpgradePiece != null)
					{
						num += shipUpgradePiece.CrewCapacityBonusMultiplier;
					}
				}
				return (int)((float)totalCrewCapacity * num);
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600000F RID: 15 RVA: 0x00002284 File Offset: 0x00000484
		public int MainDeckCrewCapacity
		{
			get
			{
				int mainDeckCrewCapacity = this._shipHull.MainDeckCrewCapacity;
				float num = 1f;
				foreach (ShipUpgradePiece shipUpgradePiece in this._shipPieces.Values)
				{
					if (shipUpgradePiece != null)
					{
						num += shipUpgradePiece.CrewCapacityBonusMultiplier;
					}
				}
				return (int)((float)mainDeckCrewCapacity * num);
			}
		}

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000010 RID: 16 RVA: 0x000022F8 File Offset: 0x000004F8
		public int SkeletalCrewCapacity
		{
			get
			{
				return this._shipHull.SkeletalCrewCapacity;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000011 RID: 17 RVA: 0x00002305 File Offset: 0x00000505
		public int DefaultFormationGroupIndex
		{
			get
			{
				return this._shipHull.DefaultFormationGroup;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000012 RID: 18 RVA: 0x00002314 File Offset: 0x00000514
		public float ForwardDragFactor
		{
			get
			{
				float num = 0f;
				foreach (ShipUpgradePiece shipUpgradePiece in this._shipPieces.Values)
				{
					if (shipUpgradePiece != null)
					{
						num += shipUpgradePiece.DecreaseForwardDragMultiplier;
					}
				}
				return -num;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000013 RID: 19 RVA: 0x0000237C File Offset: 0x0000057C
		public float ShipWeightFactor
		{
			get
			{
				float num = 0f;
				foreach (ShipUpgradePiece shipUpgradePiece in this._shipPieces.Values)
				{
					if (shipUpgradePiece != null)
					{
						num += shipUpgradePiece.ShipWeightBonusMultiplier;
					}
				}
				return num;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000014 RID: 20 RVA: 0x000023E0 File Offset: 0x000005E0
		public float RudderSurfaceAreaFactor
		{
			get
			{
				float num = 0f;
				foreach (ShipUpgradePiece shipUpgradePiece in this._shipPieces.Values)
				{
					if (shipUpgradePiece != null)
					{
						num += shipUpgradePiece.RudderSurfaceAreaBonusMultiplier;
					}
				}
				return num;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000015 RID: 21 RVA: 0x00002444 File Offset: 0x00000644
		public int RandomValue
		{
			get
			{
				return 123457;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000016 RID: 22 RVA: 0x0000244B File Offset: 0x0000064B
		// (set) Token: 0x06000017 RID: 23 RVA: 0x00002453 File Offset: 0x00000653
		public string CustomSailPatternId { get; set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000018 RID: 24 RVA: 0x0000245C File Offset: 0x0000065C
		public float MaxRudderForceFactor
		{
			get
			{
				float num = 0f;
				foreach (ShipUpgradePiece shipUpgradePiece in this._shipPieces.Values)
				{
					if (shipUpgradePiece != null)
					{
						num += shipUpgradePiece.MaxRudderForceBonusMultiplier;
					}
				}
				return num;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000019 RID: 25 RVA: 0x000024C0 File Offset: 0x000006C0
		public float MaxOarForceFactor
		{
			get
			{
				float num = 0f;
				foreach (ShipUpgradePiece shipUpgradePiece in this._shipPieces.Values)
				{
					if (shipUpgradePiece != null)
					{
						num += shipUpgradePiece.MaxOarForceBonusMultiplier;
					}
				}
				return num;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600001A RID: 26 RVA: 0x00002524 File Offset: 0x00000724
		public float SailForceFactor
		{
			get
			{
				float num = 0f;
				foreach (ShipUpgradePiece shipUpgradePiece in this._shipPieces.Values)
				{
					if (shipUpgradePiece != null)
					{
						num += shipUpgradePiece.SailForceBonusMultiplier;
					}
				}
				return num;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600001B RID: 27 RVA: 0x00002588 File Offset: 0x00000788
		public float MaxOarPowerFactor
		{
			get
			{
				float num = 0f;
				foreach (ShipUpgradePiece shipUpgradePiece in this._shipPieces.Values)
				{
					if (shipUpgradePiece != null)
					{
						num += shipUpgradePiece.MaxOarPowerBonusMultiplier;
					}
				}
				return num;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600001C RID: 28 RVA: 0x000025EC File Offset: 0x000007EC
		public float SailRotationSpeedFactor
		{
			get
			{
				float num = 0f;
				foreach (ShipUpgradePiece shipUpgradePiece in this._shipPieces.Values)
				{
					if (shipUpgradePiece != null)
					{
						num += shipUpgradePiece.SailRotationSpeedBonusMultiplier;
					}
				}
				return num;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600001D RID: 29 RVA: 0x00002650 File Offset: 0x00000850
		public float FurlUnfurlSpeedFactor
		{
			get
			{
				float num = 0f;
				foreach (ShipUpgradePiece shipUpgradePiece in this._shipPieces.Values)
				{
					if (shipUpgradePiece != null)
					{
						num += shipUpgradePiece.FurlUnfurlSpeedBonusMultiplier;
					}
				}
				return num;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600001E RID: 30 RVA: 0x000026B4 File Offset: 0x000008B4
		public float CrewShieldHitPointsFactor
		{
			get
			{
				float num = 0f;
				foreach (ShipUpgradePiece shipUpgradePiece in this._shipPieces.Values)
				{
					if (shipUpgradePiece != null)
					{
						num += shipUpgradePiece.CrewShieldHitPointsBonusMultiplier;
					}
				}
				return num;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600001F RID: 31 RVA: 0x00002718 File Offset: 0x00000918
		public float CrewMeleeDamageFactor
		{
			get
			{
				float num = 0f;
				foreach (ShipUpgradePiece shipUpgradePiece in this._shipPieces.Values)
				{
					if (shipUpgradePiece != null)
					{
						num += shipUpgradePiece.CrewMeleeDamageBonusMultiplier;
					}
				}
				return num;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x06000020 RID: 32 RVA: 0x0000277C File Offset: 0x0000097C
		public int AdditionalArcherQuivers
		{
			get
			{
				int num = 0;
				foreach (ShipUpgradePiece shipUpgradePiece in this._shipPieces.Values)
				{
					if (shipUpgradePiece != null)
					{
						num += shipUpgradePiece.ArcherQuiverBonus;
					}
				}
				return num;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000021 RID: 33 RVA: 0x000027DC File Offset: 0x000009DC
		public int AdditionalThrowingWeaponStack
		{
			get
			{
				int num = 0;
				foreach (ShipUpgradePiece shipUpgradePiece in this._shipPieces.Values)
				{
					if (shipUpgradePiece != null)
					{
						num += shipUpgradePiece.ThrowingWeaponStackBonus;
					}
				}
				return num;
			}
		}

		// Token: 0x06000022 RID: 34 RVA: 0x0000283C File Offset: 0x00000A3C
		public CustomBattleShip(ShipHull shipHull, bool isPlayerShip)
		{
			this._shipHull = shipHull;
			this._isPlayerShip = isPlayerShip;
			this._remainingHitPoints = this.MaxHitPoints;
			this._remainingSailHitPoints = this.MaxSailHitPoints;
			foreach (KeyValuePair<string, ShipSlot> keyValuePair in shipHull.AvailableSlots)
			{
				this._shipPieces.Add(keyValuePair.Key, null);
			}
		}

		// Token: 0x06000023 RID: 35 RVA: 0x000028D4 File Offset: 0x00000AD4
		public List<ShipVisualSlotInfo> GetShipVisualSlotInfos()
		{
			List<ShipVisualSlotInfo> list = new List<ShipVisualSlotInfo>();
			foreach (KeyValuePair<string, ShipUpgradePiece> keyValuePair in this._shipPieces)
			{
				if (keyValuePair.Value != null)
				{
					list.Add(new ShipVisualSlotInfo(keyValuePair.Key, keyValuePair.Value.SlotPrefabChildTagId));
				}
			}
			return list;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002950 File Offset: 0x00000B50
		public List<ShipSlotAndPieceName> GetShipSlotAndPieceNames()
		{
			List<ShipSlotAndPieceName> list = new List<ShipSlotAndPieceName>();
			foreach (KeyValuePair<string, ShipUpgradePiece> keyValuePair in this._shipPieces)
			{
				if (keyValuePair.Value != null)
				{
					list.Add(new ShipSlotAndPieceName(this.Hull.AvailableSlots[keyValuePair.Key].GetSlotTypeName().ToString(), keyValuePair.Value.GetName().ToString()));
				}
			}
			return list;
		}

		// Token: 0x06000025 RID: 37 RVA: 0x000029EC File Offset: 0x00000BEC
		public void OnSailDamaged(float rawDamage, float inflictedDamage)
		{
			this._remainingSailHitPoints -= inflictedDamage;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000029FC File Offset: 0x00000BFC
		public void OnShipDamaged(float rawDamage, IShipOrigin rammingShip, out float modifiedDamage)
		{
			this._remainingHitPoints -= rawDamage;
			modifiedDamage = 0f;
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002A14 File Offset: 0x00000C14
		public void SetPieceAtSlot(string slotTag, ShipUpgradePiece upgradePiece)
		{
			float num = this.HitPoints / this.MaxHitPoints;
			ShipSlot shipSlot = this._shipHull.AvailableSlots[slotTag];
			this._shipPieces[slotTag] = upgradePiece;
			this.HitPoints = Math.Max(1f, num * this.MaxHitPoints);
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002A68 File Offset: 0x00000C68
		public CustomBattleShip GetCopy()
		{
			CustomBattleShip customBattleShip = new CustomBattleShip(this._shipHull, this.IsPlayerShip);
			foreach (KeyValuePair<string, ShipUpgradePiece> keyValuePair in this._shipPieces)
			{
				customBattleShip.SetPieceAtSlot(keyValuePair.Key, keyValuePair.Value);
			}
			return customBattleShip;
		}

		// Token: 0x04000001 RID: 1
		private readonly ShipHull _shipHull;

		// Token: 0x04000002 RID: 2
		private readonly Dictionary<string, ShipUpgradePiece> _shipPieces = new Dictionary<string, ShipUpgradePiece>();

		// Token: 0x04000003 RID: 3
		private readonly bool _isPlayerShip;

		// Token: 0x04000004 RID: 4
		private float _remainingHitPoints;

		// Token: 0x04000005 RID: 5
		private float _remainingFireHitPoints;

		// Token: 0x04000006 RID: 6
		private float _remainingSailHitPoints;
	}
}
