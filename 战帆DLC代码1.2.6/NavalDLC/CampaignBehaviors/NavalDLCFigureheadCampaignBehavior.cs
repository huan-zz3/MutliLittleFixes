using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.CampaignBehaviors
{
	// Token: 0x02000165 RID: 357
	public class NavalDLCFigureheadCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x170003F2 RID: 1010
		// (get) Token: 0x06001794 RID: 6036 RVA: 0x000A0C41 File Offset: 0x0009EE41
		public CampaignTime LastFigureheadLootTime
		{
			get
			{
				return this._lastFigureheadLootTime;
			}
		}

		// Token: 0x06001795 RID: 6037 RVA: 0x000A0C49 File Offset: 0x0009EE49
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<CampaignTime>("_lastFigureheadLootTime", ref this._lastFigureheadLootTime);
			dataStore.SyncData<Dictionary<Hero, Figurehead>>("_aiLordSelectedFigureheads", ref this._aiLordSelectedFigureheads);
		}

		// Token: 0x06001796 RID: 6038 RVA: 0x000A0C6F File Offset: 0x0009EE6F
		public override void RegisterEvents()
		{
			CampaignEvents.OnShipOwnerChangedEvent.AddNonSerializedListener(this, new Action<Ship, PartyBase, ChangeShipOwnerAction.ShipOwnerChangeDetail>(this.OnShipOwnerChanged));
			CampaignEvents.OnFigureheadUnlockedEvent.AddNonSerializedListener(this, new Action<Figurehead>(this.OnFigureheadUnlocked));
		}

		// Token: 0x06001797 RID: 6039 RVA: 0x000A0CA0 File Offset: 0x0009EEA0
		private void OnFigureheadUnlocked(Figurehead figurehead)
		{
			TextObject textObject = new TextObject("{=jBGi3saG}New figurehead \"{FIGUREHEAD_NAME}\" unlocked!", null);
			textObject.SetTextVariable("FIGUREHEAD_NAME", figurehead.Name);
			MBInformationManager.AddQuickInformation(textObject, 0, null, null, "event:/ui/notification/quest_update");
			InformationManager.DisplayMessage(new InformationMessage(textObject.ToString(), new Color(0f, 1f, 0f, 1f)));
			this._lastFigureheadLootTime = CampaignTime.Now;
		}

		// Token: 0x06001798 RID: 6040 RVA: 0x000A0D0B File Offset: 0x0009EF0B
		private MBReadOnlyList<Figurehead> GetAllFigureheads()
		{
			if (this._allFigureheadsCache == null || Extensions.IsEmpty<Figurehead>(this._allFigureheadsCache))
			{
				this._allFigureheadsCache = MBObjectManager.Instance.GetObjectTypeList<Figurehead>();
			}
			return this._allFigureheadsCache;
		}

		// Token: 0x06001799 RID: 6041 RVA: 0x000A0D38 File Offset: 0x0009EF38
		private void OnShipOwnerChanged(Ship ship, PartyBase oldOwner, ChangeShipOwnerAction.ShipOwnerChangeDetail changeDetail)
		{
			if (ship.CanEquipFigurehead)
			{
				if (ship.Figurehead == null)
				{
					PartyBase owner = ship.Owner;
					if (owner != null && owner.IsMobile && ship.Owner.MobileParty.LeaderHero != null && ship.Owner.MobileParty.ActualClan != Clan.PlayerClan)
					{
						Figurehead figurehead;
						if (this._aiLordSelectedFigureheads.TryGetValue(ship.Owner.MobileParty.LeaderHero, out figurehead))
						{
							ship.ChangeFigurehead(figurehead);
							return;
						}
						List<ValueTuple<Figurehead, float>> list = new List<ValueTuple<Figurehead, float>>();
						foreach (Figurehead figurehead2 in this.GetAllFigureheads())
						{
							if (figurehead2.Culture == ship.Owner.MobileParty.LeaderHero.Culture)
							{
								list.Add(new ValueTuple<Figurehead, float>(figurehead2, 0.2f));
							}
							else
							{
								list.Add(new ValueTuple<Figurehead, float>(figurehead2, 0.1f));
							}
						}
						Figurehead figurehead3 = MBRandom.ChooseWeighted<Figurehead>(list);
						ship.ChangeFigurehead(figurehead3);
						this._aiLordSelectedFigureheads.Add(ship.Owner.MobileParty.LeaderHero, figurehead3);
						return;
					}
				}
				else
				{
					PartyBase owner2 = ship.Owner;
					if (owner2 == null || !owner2.IsSettlement)
					{
						PartyBase owner3 = ship.Owner;
						if (owner3 == null || !owner3.IsMobile || ship.Owner.MobileParty.ActualClan != Clan.PlayerClan)
						{
							return;
						}
						Clan clan;
						if (oldOwner == null)
						{
							clan = null;
						}
						else
						{
							MobileParty mobileParty = oldOwner.MobileParty;
							clan = ((mobileParty != null) ? mobileParty.ActualClan : null);
						}
						if (clan == Clan.PlayerClan)
						{
							return;
						}
					}
					ship.ChangeFigurehead(null);
				}
			}
		}

		// Token: 0x04000BD5 RID: 3029
		private MBReadOnlyList<Figurehead> _allFigureheadsCache;

		// Token: 0x04000BD6 RID: 3030
		private CampaignTime _lastFigureheadLootTime = CampaignTime.Zero;

		// Token: 0x04000BD7 RID: 3031
		private Dictionary<Hero, Figurehead> _aiLordSelectedFigureheads = new Dictionary<Hero, Figurehead>();
	}
}
