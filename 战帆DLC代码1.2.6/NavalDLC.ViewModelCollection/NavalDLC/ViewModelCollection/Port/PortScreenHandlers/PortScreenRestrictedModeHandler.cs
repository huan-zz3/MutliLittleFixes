using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.ViewModelCollection.Port.PortScreenHandlers
{
	// Token: 0x0200001D RID: 29
	public class PortScreenRestrictedModeHandler : PortScreenHandler
	{
		// Token: 0x06000239 RID: 569 RVA: 0x0000C54D File Offset: 0x0000A74D
		public PortScreenRestrictedModeHandler(PartyBase leftOwner, PartyBase rightOwner)
			: base(leftOwner.Ships, new MBReadOnlyList<Ship>())
		{
			this._leftOwner = leftOwner;
			this._rightOwner = rightOwner;
		}

		// Token: 0x0600023A RID: 570 RVA: 0x0000C56E File Offset: 0x0000A76E
		protected override PortActionInfo CanBuyShip(Ship ship)
		{
			return PortActionInfo.CreateValid(false, this.GetTradeCostOfShip(ship, false), GameTexts.FindText("str_port_buy_ship", null), new TextObject("{=a2oyqIOU}You cannot buy ships when your fleet is away", null));
		}

		// Token: 0x0600023B RID: 571 RVA: 0x0000C594 File Offset: 0x0000A794
		protected override PortActionInfo CanSellShip(Ship ship)
		{
			return PortActionInfo.CreateValid(false, this.GetTradeCostOfShip(ship, true), GameTexts.FindText("str_port_sell_ship", null), new TextObject("{=YCwajsdL}You cannot sell ships when your fleet is away", null));
		}

		// Token: 0x0600023C RID: 572 RVA: 0x0000C5BA File Offset: 0x0000A7BA
		protected override PortActionInfo CanRenameShip(Ship ship)
		{
			return PortActionInfo.CreateValid(false, 0, GameTexts.FindText("str_port_rename_ship", null), new TextObject("{=xmmYDcyd}You cannot rename ships when your fleet is away", null));
		}

		// Token: 0x0600023D RID: 573 RVA: 0x0000C5D9 File Offset: 0x0000A7D9
		protected override PortActionInfo CanRepairShip(Ship ship)
		{
			return PortActionInfo.CreateValid(false, 0, GameTexts.FindText("str_port_repair_ship", null), new TextObject("{=7ccDIA8H}You cannot repair ships when your fleet is away", null));
		}

		// Token: 0x0600023E RID: 574 RVA: 0x0000C5F8 File Offset: 0x0000A7F8
		protected override PortActionInfo CanRepairAll()
		{
			return PortActionInfo.CreateValid(false, 0, GameTexts.FindText("str_port_repair_all_ships", null), new TextObject("{=7ccDIA8H}You cannot repair ships when your fleet is away", null));
		}

		// Token: 0x0600023F RID: 575 RVA: 0x0000C617 File Offset: 0x0000A817
		protected override PortActionInfo CanUpgradeShip(Ship ship)
		{
			return PortActionInfo.CreateValid(false, 0, GameTexts.FindText("str_port_upgrade_ship", null), new TextObject("{=5CXQsbqV}You cannot upgrade ships when your fleet is away", null));
		}

		// Token: 0x06000240 RID: 576 RVA: 0x0000C636 File Offset: 0x0000A836
		protected override PortActionInfo CanSendToClan(Ship ship)
		{
			return PortActionInfo.CreateInvalid(null);
		}

		// Token: 0x06000241 RID: 577 RVA: 0x0000C63E File Offset: 0x0000A83E
		public override bool GetCanConfirm(out TextObject disabledHint)
		{
			disabledHint = TextObject.GetEmpty();
			return true;
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0000C648 File Offset: 0x0000A848
		public override TextObject GetLeftRosterName()
		{
			PartyBase leftOwner = this._leftOwner;
			if (leftOwner != null && leftOwner.IsSettlement)
			{
				return new TextObject("{=UeUkbDVz}Port of {SETTLEMENT}", null).SetTextVariable("SETTLEMENT", this._leftOwner.Name);
			}
			PartyBase leftOwner2 = this._leftOwner;
			if (leftOwner2 == null)
			{
				return null;
			}
			return leftOwner2.Name;
		}

		// Token: 0x06000243 RID: 579 RVA: 0x0000C69C File Offset: 0x0000A89C
		public override int GetTradeCostOfShip(Ship ship, bool isRightSideSelling)
		{
			PartyBase partyBase = (isRightSideSelling ? this._rightOwner : this._leftOwner);
			PartyBase partyBase2 = (isRightSideSelling ? this._leftOwner : this._rightOwner);
			return (int)Campaign.Current.Models.ShipCostModel.GetShipTradeValue(ship, partyBase, partyBase2);
		}

		// Token: 0x06000244 RID: 580 RVA: 0x0000C6E5 File Offset: 0x0000A8E5
		public override int GetRepairCostOfShip(Ship ship, bool isRightSideRepairing)
		{
			return 0;
		}

		// Token: 0x06000245 RID: 581 RVA: 0x0000C6E8 File Offset: 0x0000A8E8
		public override int GetUpgradeCostOfShip(Ship ship, ShipUpgradePiece piece, bool isRightSideUpgrading)
		{
			return 0;
		}

		// Token: 0x06000246 RID: 582 RVA: 0x0000C6EB File Offset: 0x0000A8EB
		public override TextObject GetRightRosterName()
		{
			PartyBase rightOwner = this._rightOwner;
			if (rightOwner == null)
			{
				return null;
			}
			return rightOwner.Name;
		}

		// Token: 0x06000247 RID: 583 RVA: 0x0000C6FE File Offset: 0x0000A8FE
		public override PartyBase GetLeftSideOwnerParty()
		{
			return this._leftOwner;
		}

		// Token: 0x06000248 RID: 584 RVA: 0x0000C706 File Offset: 0x0000A906
		public override PartyBase GetRightSideOwnerParty()
		{
			return this._rightOwner;
		}

		// Token: 0x06000249 RID: 585 RVA: 0x0000C70E File Offset: 0x0000A90E
		public override int GetTotalGoldCost()
		{
			return 0;
		}

		// Token: 0x0600024A RID: 586 RVA: 0x0000C711 File Offset: 0x0000A911
		public override void OnConfirmChanges()
		{
		}

		// Token: 0x0600024B RID: 587 RVA: 0x0000C713 File Offset: 0x0000A913
		public override List<PortChangeInfo> GetChanges()
		{
			return new List<PortChangeInfo>();
		}

		// Token: 0x040000C6 RID: 198
		private readonly PartyBase _leftOwner;

		// Token: 0x040000C7 RID: 199
		private readonly PartyBase _rightOwner;
	}
}
