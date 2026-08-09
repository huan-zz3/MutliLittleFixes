using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.ViewModelCollection.Port
{
	// Token: 0x02000010 RID: 16
	public class ShipStatsVM : ViewModel
	{
		// Token: 0x0600013C RID: 316 RVA: 0x00008DB5 File Offset: 0x00006FB5
		public ShipStatsVM(Ship ship)
		{
			this._ship = ship;
			this.StatList = new MBBindingList<ShipStatVM>();
			this.RefreshStats(this._ship.HitPoints, null);
			this.RefreshValues();
		}

		// Token: 0x0600013D RID: 317 RVA: 0x00008DE7 File Offset: 0x00006FE7
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.StatList.ApplyActionOnAllItems(delegate(ShipStatVM s)
			{
				s.RefreshValues();
			});
		}

		// Token: 0x0600013E RID: 318 RVA: 0x00008E1C File Offset: 0x0000701C
		public void RefreshStats(float currentHp, MBReadOnlyList<ValueTuple<string, ShipUpgradePiece>> newlySelectedPieces)
		{
			this.StatList.Clear();
			MissionShipObject @object = MBObjectManager.Instance.GetObject<MissionShipObject>(this._ship.ShipHull.MissionShipObjectId);
			if (@object == null)
			{
				Debug.FailedAssert("Failed to find mission ship object with id: " + this._ship.ShipHull.MissionShipObjectId, "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.ViewModelCollection\\Port\\ShipStatsVM.cs", "RefreshStats", 40);
				return;
			}
			MBList<ShipUpgradePiece> mblist = new MBList<ShipUpgradePiece>();
			foreach (KeyValuePair<string, ShipSlot> keyValuePair in this._ship.ShipHull.AvailableSlots)
			{
				mblist.Add(this._ship.GetPieceAtSlot(keyValuePair.Key));
			}
			float num = 1f;
			float num2 = 1f;
			float num3 = 1f;
			float num4 = 1f;
			float num5 = 1f;
			for (int i = 0; i < mblist.Count; i++)
			{
				ShipUpgradePiece shipUpgradePiece = mblist[i];
				if (shipUpgradePiece != null)
				{
					num += shipUpgradePiece.CampaignSpeedBonusMultiplier;
					num2 += shipUpgradePiece.MaxHitPointsBonusMultiplier;
					num3 += shipUpgradePiece.InventoryCapacityBonusMultiplier;
					num4 += shipUpgradePiece.ShipWeightBonusMultiplier;
					num5 += shipUpgradePiece.CrewCapacityBonusMultiplier;
				}
			}
			float num6 = 0f;
			float num7 = 0f;
			float num8 = 0f;
			float num9 = 0f;
			float num10 = 0f;
			int num11 = 0;
			if (newlySelectedPieces != null && newlySelectedPieces.Count > 0)
			{
				for (int j = 0; j < newlySelectedPieces.Count; j++)
				{
					string item = newlySelectedPieces[j].Item1;
					ShipUpgradePiece item2 = newlySelectedPieces[j].Item2;
					if (item2 != null)
					{
						num6 += item2.CampaignSpeedBonusMultiplier;
						num7 += item2.MaxHitPointsBonusMultiplier;
						num8 += item2.InventoryCapacityBonusMultiplier;
						num9 += item2.ShipWeightBonusMultiplier;
						num10 += item2.CrewCapacityBonusMultiplier;
						num11 += item2.SeaWorthinessBonus;
					}
					ShipUpgradePiece pieceAtSlot = this._ship.GetPieceAtSlot(item);
					if (pieceAtSlot != null)
					{
						num6 -= pieceAtSlot.CampaignSpeedBonusMultiplier;
						num7 -= pieceAtSlot.MaxHitPointsBonusMultiplier;
						num8 -= pieceAtSlot.InventoryCapacityBonusMultiplier;
						num9 -= pieceAtSlot.ShipWeightBonusMultiplier;
						num10 -= pieceAtSlot.CrewCapacityBonusMultiplier;
						num11 -= pieceAtSlot.SeaWorthinessBonus;
					}
				}
			}
			num6 /= num;
			num7 /= num2;
			num8 /= num3;
			num9 /= num4;
			num10 /= num5;
			this.StatList.Add(new ShipStatVM("hull", new TextObject("{=wEmx6fZi}Hull", null), this._ship.ShipHull.Name.ToString(), "", true, null));
			this.StatList.Add(new ShipStatVM("class", new TextObject("{=sqdzHOPe}Class", null), this.GetClassStr(this._ship), "", true, null));
			this.StatList.Add(new ShipStatVM("crew", new TextObject("{=wXCM8BnW}Crew", null), this.GetCrewCapacityStr(this._ship), this.GetBonusStr(num10, true), num10 > 0f, () => this.GetCrewCapacityTooltip(this._ship)));
			this.StatList.Add(new ShipStatVM("cargo_capacity", new TextObject("{=IE1KbkaH}Cargo Capacity", null), this._ship.InventoryCapacity.ToString(), this.GetBonusStr(num8, true), num8 > 0f, null));
			this.StatList.Add(new ShipStatVM("weight", new TextObject("{=4Dd2xgPm}Weight", null), (@object.Mass * (1f + this._ship.ShipWeightFactor)).ToString("0"), this.GetBonusStr(num9, true), num9 < 0f, null));
			this.StatList.Add(new ShipStatVM("travel_speed", new TextObject("{=DbERaPfF}Travel Speed", null), this._ship.GetCampaignSpeed().ToString("0.##"), this.GetBonusStr(num6, true), num6 > 0f, null));
			this.StatList.Add(new ShipStatVM("sail_type", new TextObject("{=PJyFY05L}Sail", null), this.GetSailTypeStr(@object), "", true, null));
			this.StatList.Add(new ShipStatVM("draft_type", new TextObject("{=I4bu7cLr}Draft", null), this.GetDraftTypeStr(this._ship), "", true, null));
			this.StatList.Add(new ShipStatVM("sea_worthiness", new TextObject("{=yCzuXN3O}Seaworthiness", null), this._ship.SeaWorthiness.ToString(), this.GetBonusStr((float)num11, false), num11 > 0, null));
			this.StatList.Add(new ShipStatVM("hit_points", new TextObject("{=oBbiVeKE}Hit Points", null), this.GetHitPointsStr(this._ship, currentHp), this.GetBonusStr(num7, true), num7 > 0f, null));
		}

		// Token: 0x0600013F RID: 319 RVA: 0x00009328 File Offset: 0x00007528
		private string GetBonusStr(float bonus, bool isPercentage)
		{
			if (MathF.Abs(bonus) < 0.001f)
			{
				return string.Empty;
			}
			if (isPercentage)
			{
				string text = GameTexts.FindText("str_NUMBER_percent", null).SetTextVariable("NUMBER", (bonus * 100f).ToString("+#;-#")).ToString();
				return GameTexts.FindText("str_STR_in_parentheses", null).SetTextVariable("STR", text).ToString();
			}
			return GameTexts.FindText("str_STR_in_parentheses", null).SetTextVariable("STR", bonus.ToString("+#;-#")).ToString();
		}

		// Token: 0x06000140 RID: 320 RVA: 0x000093BC File Offset: 0x000075BC
		private string GetClassStr(Ship ship)
		{
			return GameTexts.FindText("str_ship_type", ship.ShipHull.Type.ToString().ToLowerInvariant()).ToString();
		}

		// Token: 0x06000141 RID: 321 RVA: 0x000093F8 File Offset: 0x000075F8
		private string GetCrewCapacityStr(Ship ship)
		{
			int skeletalCrewCapacity = ship.SkeletalCrewCapacity;
			int mainDeckCrewCapacity = ship.MainDeckCrewCapacity;
			int num = ship.TotalCrewCapacity - ship.MainDeckCrewCapacity;
			TextObject textObject;
			if (num > 0)
			{
				textObject = new TextObject("{=!}{SKELETAL} • {DECK} + {RESERVE}", null);
			}
			else
			{
				textObject = new TextObject("{=!}{SKELETAL} • {DECK}", null);
			}
			return textObject.SetTextVariable("SKELETAL", skeletalCrewCapacity).SetTextVariable("DECK", mainDeckCrewCapacity).SetTextVariable("RESERVE", num)
				.ToString();
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00009468 File Offset: 0x00007668
		private List<TooltipProperty> GetCrewCapacityTooltip(Ship ship)
		{
			List<TooltipProperty> list = new List<TooltipProperty>();
			int skeletalCrewCapacity = ship.SkeletalCrewCapacity;
			int mainDeckCrewCapacity = ship.MainDeckCrewCapacity;
			int totalCrewCapacity = ship.TotalCrewCapacity;
			int num = totalCrewCapacity - mainDeckCrewCapacity;
			list.Add(new TooltipProperty(new TextObject("{=kalMphFt}Skeletal Capacity", null).ToString(), skeletalCrewCapacity.ToString(), 0, false, 0));
			list.Add(new TooltipProperty(string.Empty, GameTexts.FindText("str_ship_stat_explanation", "crewskeletal").ToString(), -1, false, 1));
			list.Add(new TooltipProperty(string.Empty, string.Empty, 0, false, 1024));
			list.Add(new TooltipProperty(new TextObject("{=Bt82dbKu}Deck Capacity", null).ToString(), mainDeckCrewCapacity.ToString(), 0, false, 0));
			list.Add(new TooltipProperty(string.Empty, GameTexts.FindText("str_ship_stat_explanation", "crewdeck").ToString(), -1, false, 1));
			list.Add(new TooltipProperty(string.Empty, string.Empty, 0, false, 0));
			list.Add(new TooltipProperty(new TextObject("{=HThruy9f}Reserve Capacity", null).ToString(), num.ToString(), 0, false, 0));
			list.Add(new TooltipProperty(string.Empty, GameTexts.FindText("str_ship_stat_explanation", "crewreserve").ToString(), -1, false, 1));
			list.Add(new TooltipProperty(string.Empty, string.Empty, 0, false, 512));
			list.Add(new TooltipProperty(new TextObject("{=kLvWPxIK}Total Capacity", null).ToString(), totalCrewCapacity.ToString(), 0, false, 0));
			list.Add(new TooltipProperty(string.Empty, GameTexts.FindText("str_ship_stat_explanation", "crewtotal").ToString(), -1, false, 1));
			return list;
		}

		// Token: 0x06000143 RID: 323 RVA: 0x00009614 File Offset: 0x00007814
		private string GetSailTypeStr(MissionShipObject missionShipObject)
		{
			if (missionShipObject.HasSails)
			{
				bool flag = missionShipObject.Sails.Any<ShipSail>((ShipSail x) => x.Type == 1);
				bool flag2 = missionShipObject.Sails.Any<ShipSail>((ShipSail x) => x.Type == 0);
				if (flag && flag2)
				{
					return new TextObject("{=bXJLb0BE}Hybrid", null).ToString();
				}
				if (flag)
				{
					return new TextObject("{=kNxD2oer}Lateen", null).ToString();
				}
				if (flag2)
				{
					return new TextObject("{=squareSail}Square", null).ToString();
				}
			}
			return new TextObject("{=koX9okuG}None", null).ToString();
		}

		// Token: 0x06000144 RID: 324 RVA: 0x000096D0 File Offset: 0x000078D0
		private string GetDraftTypeStr(Ship ship)
		{
			if (ship.ShipHull.CanNavigateShallowWater)
			{
				return new TextObject("{=ShipDraftTypeShallow}Shallow", null).ToString();
			}
			return new TextObject("{=ShipDraftTypeDeep}Deep", null).ToString();
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00009700 File Offset: 0x00007900
		private string GetHitPointsStr(Ship ship, float currentHp)
		{
			return GameTexts.FindText("str_LEFT_over_RIGHT_no_space", null).SetTextVariable("LEFT", currentHp.ToString("0")).SetTextVariable("RIGHT", ship.MaxHitPoints.ToString("0"))
				.ToString();
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000146 RID: 326 RVA: 0x00009750 File Offset: 0x00007950
		// (set) Token: 0x06000147 RID: 327 RVA: 0x00009758 File Offset: 0x00007958
		[DataSourceProperty]
		public MBBindingList<ShipStatVM> StatList
		{
			get
			{
				return this._statList;
			}
			set
			{
				if (value != this._statList)
				{
					this._statList = value;
					base.OnPropertyChangedWithValue<MBBindingList<ShipStatVM>>(value, "StatList");
				}
			}
		}

		// Token: 0x04000077 RID: 119
		private readonly Ship _ship;

		// Token: 0x04000078 RID: 120
		private MBBindingList<ShipStatVM> _statList;
	}
}
