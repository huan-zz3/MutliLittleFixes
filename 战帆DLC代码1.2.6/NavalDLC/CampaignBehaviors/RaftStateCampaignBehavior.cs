using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace NavalDLC.CampaignBehaviors
{
	// Token: 0x02000173 RID: 371
	public class RaftStateCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x06001863 RID: 6243 RVA: 0x000A64DC File Offset: 0x000A46DC
		public override void RegisterEvents()
		{
			CampaignEvents.OnMobilePartyRaftStateChangedEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.OnMobilePartyRaftStateChanged));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.HeroPrisonerReleased.AddNonSerializedListener(this, new Action<Hero, PartyBase, IFaction, EndCaptivityDetail, bool>(this.OnHeroPrisonerReleased));
			CampaignEvents.HeroPrisonerTaken.AddNonSerializedListener(this, new Action<PartyBase, Hero>(this.OnHeroPrisonerTaken));
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnMapEventEnded));
			CampaignEvents.OnShipDestroyedEvent.AddNonSerializedListener(this, new Action<PartyBase, Ship, DestroyShipAction.ShipDestroyDetail>(this.OnShipDestroyed));
			CampaignEvents.OnPartyLeftArmyEvent.AddNonSerializedListener(this, new Action<MobileParty, Army>(this.OnPartyLeftArmy));
			CampaignEvents.SettlementEntered.AddNonSerializedListener(this, new Action<MobileParty, Settlement, Hero>(this.OnSettlementEntered));
			CampaignEvents.CanHaveCampaignIssuesEvent.AddNonSerializedListener(this, new ReferenceAction<Hero, bool>(this.CanHaveCampaignIssues));
			CampaignEvents.OnPlayerCharacterChangedEvent.AddNonSerializedListener(this, new Action<Hero, Hero, MobileParty, bool>(this.OnPlayerCharacterChanged));
		}

		// Token: 0x06001864 RID: 6244 RVA: 0x000A65CF File Offset: 0x000A47CF
		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Dictionary<Ship, List<ShipUpgradePiece>>>("_playerCachedShips", ref this._playerCachedShips);
			dataStore.SyncData<AnchorPoint>("_cachedAnchorPoint", ref this._cachedAnchorPoint);
		}

		// Token: 0x06001865 RID: 6245 RVA: 0x000A65F8 File Offset: 0x000A47F8
		private void OnSessionLaunched(CampaignGameStarter gameStarter)
		{
			gameStarter.AddGameMenu("player_raft_state", "{=5ROdLNNo}You no longer have a seaworthy ship. Your party will land on the nearest shore.", new OnInitDelegate(this.player_raft_state_on_init), 0, 0, null);
			gameStarter.AddGameMenuOption("player_raft_state", "continue", "{=DM6luo3c}Continue", new GameMenuOption.OnConditionDelegate(this.continue_condition), new GameMenuOption.OnConsequenceDelegate(this.player_raft_state_continue_on_consequence), false, -1, false, null);
			gameStarter.AddGameMenu("player_raft_state_after_prisoner", "{=BF4ybrgP}You are no longer a prisoner. Since you don't have a seaworthy ship, your party will land on the nearest shore.", new OnInitDelegate(this.player_raft_state_on_init), 0, 0, null);
			gameStarter.AddGameMenuOption("player_raft_state_after_prisoner", "continue", "{=DM6luo3c}Continue", new GameMenuOption.OnConditionDelegate(this.continue_condition), new GameMenuOption.OnConsequenceDelegate(this.player_raft_state_after_prisoner_on_consequence), false, -1, false, null);
			gameStarter.AddWaitGameMenu("player_raft_state_wait", "{=nxA52tGB}Your party is stranded at sea.", null, null, null, new OnTickDelegate(this.player_raft_state_menu_on_tick), 3, 0, 0f, 0, null);
			gameStarter.AddGameMenu("player_raft_state_end", "{=iQkp5KSA}Your party has washed ashore.", null, 0, 0, null);
			gameStarter.AddGameMenuOption("player_raft_state_end", "continue", "{=DM6luo3c}Continue", new GameMenuOption.OnConditionDelegate(this.continue_condition), new GameMenuOption.OnConsequenceDelegate(this.player_raft_state_end_continue_on_consequence), false, -1, false, null);
		}

		// Token: 0x06001866 RID: 6246 RVA: 0x000A6712 File Offset: 0x000A4912
		[GameMenuInitializationHandler("player_raft_state")]
		[GameMenuInitializationHandler("player_raft_state_after_prisoner")]
		[GameMenuInitializationHandler("player_raft_state_wait")]
		public static void game_menu_player_raft_state_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName("raft_state");
		}

		// Token: 0x06001867 RID: 6247 RVA: 0x000A6724 File Offset: 0x000A4924
		[GameMenuInitializationHandler("player_raft_state_end")]
		public static void game_menu_player_raft_state_end_on_init(MenuCallbackArgs args)
		{
			args.MenuContext.SetBackgroundMeshName("captive_at_sea_escape");
		}

		// Token: 0x06001868 RID: 6248 RVA: 0x000A6736 File Offset: 0x000A4936
		private bool continue_condition(MenuCallbackArgs args)
		{
			args.optionLeaveType = 17;
			return true;
		}

		// Token: 0x06001869 RID: 6249 RVA: 0x000A6744 File Offset: 0x000A4944
		private void player_raft_state_end_continue_on_consequence(MenuCallbackArgs args)
		{
			GameMenu.ExitToLast();
			if (this._playerCachedShips.Count > 0)
			{
				this.GiveCachedShipsToMainParty();
				MobileParty.MainParty.Anchor.SetPosition(this._cachedAnchorPoint.Position);
				MobileParty.MainParty.Anchor.SetLastUsedDisembarkPosition(this._cachedAnchorPoint.GetLastUsedDisembarkPosition());
				this._cachedAnchorPoint = null;
			}
		}

		// Token: 0x0600186A RID: 6250 RVA: 0x000A67A5 File Offset: 0x000A49A5
		private void player_raft_state_menu_on_tick(MenuCallbackArgs args, CampaignTime dt)
		{
			if (!MobileParty.MainParty.IsCurrentlyAtSea)
			{
				GameMenu.SwitchToMenu("player_raft_state_end");
				MobileParty.MainParty.SetMoveGoToPoint(MobileParty.MainParty.Position, MobileParty.MainParty.NavigationCapability);
			}
		}

		// Token: 0x0600186B RID: 6251 RVA: 0x000A67DC File Offset: 0x000A49DC
		private void player_raft_state_after_prisoner_on_consequence(MenuCallbackArgs args)
		{
			Campaign.Current.TimeControlMode = 4;
			if (!MobileParty.MainParty.IsInRaftState)
			{
				if (MobileParty.MainParty.Ships.Count > 0)
				{
					this._cachedAnchorPoint = new AnchorPoint(MobileParty.MainParty.Anchor);
					for (int i = MobileParty.MainParty.Ships.Count - 1; i >= 0; i--)
					{
						Ship ship = MobileParty.MainParty.Ships[i];
						this._playerCachedShips.Add(ship, ship.UnlockedUpgradePieces.ToList<ShipUpgradePiece>());
						ship.Owner = null;
					}
				}
				RaftStateCampaignBehavior.HandleRaftStateActivate(MobileParty.MainParty, null);
			}
			GameMenu.SwitchToMenu("player_raft_state_wait");
		}

		// Token: 0x0600186C RID: 6252 RVA: 0x000A6887 File Offset: 0x000A4A87
		private void player_raft_state_continue_on_consequence(MenuCallbackArgs args)
		{
			Campaign.Current.TimeControlMode = 4;
			if (!MobileParty.MainParty.IsInRaftState)
			{
				RaftStateCampaignBehavior.HandleRaftStateActivate(MobileParty.MainParty, null);
			}
			GameMenu.SwitchToMenu("player_raft_state_wait");
		}

		// Token: 0x0600186D RID: 6253 RVA: 0x000A68B5 File Offset: 0x000A4AB5
		private void player_raft_state_on_init(MenuCallbackArgs args)
		{
			Campaign.Current.TimeControlMode = 0;
		}

		// Token: 0x0600186E RID: 6254 RVA: 0x000A68C4 File Offset: 0x000A4AC4
		private static void HandleRaftStateActivate(MobileParty mobileParty, MapEvent mapEvent)
		{
			if (mobileParty.HasLandNavigationCapability)
			{
				RaftStateChangeAction.ActivateRaftStateForParty(mobileParty);
				return;
			}
			if (mobileParty.IsCaravan && mobileParty.LeaderHero != null)
			{
				mobileParty.LeaderHero.ChangeState(2);
			}
			DestroyPartyAction.Apply((mapEvent != null) ? mapEvent.Winner.LeaderParty : null, mobileParty);
		}

		// Token: 0x0600186F RID: 6255 RVA: 0x000A6913 File Offset: 0x000A4B13
		private bool ShouldActivateRaftStateForMobileParty(MobileParty mobileParty)
		{
			return mobileParty.IsCurrentlyAtSea && !mobileParty.IsInRaftState && !mobileParty.HasNavalNavigationCapability && mobileParty.IsActive;
		}

		// Token: 0x06001870 RID: 6256 RVA: 0x000A6935 File Offset: 0x000A4B35
		private void ConsiderMemberAndArmyRaftStateStatus(MobileParty party, Army army)
		{
			if (this.ShouldActivateRaftStateForMobileParty(party))
			{
				RaftStateCampaignBehavior.HandleRaftStateActivate(party, party.MapEvent);
			}
			if (army != null && army.LeaderParty.IsCurrentlyAtSea && !army.LeaderParty.HasNavalNavigationCapability)
			{
				DisbandArmyAction.ApplyByNoShip(army);
			}
		}

		// Token: 0x06001871 RID: 6257 RVA: 0x000A696F File Offset: 0x000A4B6F
		private void ConsiderArmyRaftState(MobileParty mobileParty)
		{
			if (!mobileParty.Army.LeaderParty.HasNavalNavigationCapability)
			{
				DisbandArmyAction.ApplyByNoShip(mobileParty.Army);
				return;
			}
			mobileParty.Army = null;
		}

		// Token: 0x06001872 RID: 6258 RVA: 0x000A6998 File Offset: 0x000A4B98
		private void OnMapEventEnded(MapEvent mapEvent)
		{
			foreach (PartyBase partyBase in mapEvent.InvolvedParties.ToList<PartyBase>())
			{
				if (partyBase.IsMobile && this.ShouldActivateRaftStateForMobileParty(partyBase.MobileParty))
				{
					if (partyBase.MobileParty.Army != null)
					{
						this.ConsiderMemberAndArmyRaftStateStatus(partyBase.MobileParty, partyBase.MobileParty.Army);
					}
					else
					{
						RaftStateCampaignBehavior.HandleRaftStateActivate(partyBase.MobileParty, mapEvent);
					}
				}
			}
		}

		// Token: 0x06001873 RID: 6259 RVA: 0x000A6A34 File Offset: 0x000A4C34
		private void OnShipDestroyed(PartyBase owner, Ship ship, DestroyShipAction.ShipDestroyDetail detail)
		{
			if (owner != null && owner.MapEvent == null && owner.IsMobile && this.ShouldActivateRaftStateForMobileParty(owner.MobileParty))
			{
				if (owner.MobileParty.Army != null)
				{
					this.ConsiderMemberAndArmyRaftStateStatus(owner.MobileParty, owner.MobileParty.Army);
					return;
				}
				RaftStateCampaignBehavior.HandleRaftStateActivate(owner.MobileParty, null);
			}
		}

		// Token: 0x06001874 RID: 6260 RVA: 0x000A6A93 File Offset: 0x000A4C93
		private void OnPartyLeftArmy(MobileParty party, Army army)
		{
			if (party.IsCurrentlyAtSea || army.LeaderParty.IsCurrentlyAtSea)
			{
				this.ConsiderMemberAndArmyRaftStateStatus(party, army);
			}
		}

		// Token: 0x06001875 RID: 6261 RVA: 0x000A6AB2 File Offset: 0x000A4CB2
		private void OnHeroPrisonerTaken(PartyBase party, Hero hero)
		{
			if (hero == Hero.MainHero && MobileParty.MainParty.IsInRaftState)
			{
				RaftStateChangeAction.DeactivateRaftStateForParty(MobileParty.MainParty);
			}
		}

		// Token: 0x06001876 RID: 6262 RVA: 0x000A6AD2 File Offset: 0x000A4CD2
		private void OnSettlementEntered(MobileParty mobileParty, Settlement settlement, Hero hero)
		{
			if (mobileParty != null && mobileParty.IsInRaftState)
			{
				Debug.FailedAssert("this should not be possible natively.", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\CampaignBehaviors\\RaftStateCampaignBehavior.cs", "OnSettlementEntered", 274);
				RaftStateChangeAction.DeactivateRaftStateForParty(MobileParty.MainParty);
			}
		}

		// Token: 0x06001877 RID: 6263 RVA: 0x000A6B02 File Offset: 0x000A4D02
		private void OnHeroPrisonerReleased(Hero prisoner, PartyBase party, IFaction capturerFaction, EndCaptivityDetail detail, bool showNotification = true)
		{
			if (prisoner != Hero.MainHero)
			{
				MakeHeroFugitiveAction.Apply(prisoner, false);
				return;
			}
			if (MobileParty.MainParty.IsCurrentlyAtSea)
			{
				GameMenu.ActivateGameMenu("player_raft_state_after_prisoner");
			}
		}

		// Token: 0x06001878 RID: 6264 RVA: 0x000A6B2C File Offset: 0x000A4D2C
		public void OnPlayerCharacterChanged(Hero oldPlayer, Hero newPlayer, MobileParty newMainParty, bool isMainPartyChanged)
		{
			if (this.ShouldActivateRaftStateForMobileParty(newMainParty))
			{
				RaftStateChangeAction.ActivateRaftStateForParty(newMainParty);
			}
			else if (this._playerCachedShips.Count > 0)
			{
				this._cachedAnchorPoint = null;
				this.GiveCachedShipsToMainParty();
			}
			Army army = newMainParty.Army;
			if (army != null && army.LeaderParty.IsCurrentlyAtSea && !army.LeaderParty.HasNavalNavigationCapability)
			{
				DisbandArmyAction.ApplyByNoShip(army);
			}
		}

		// Token: 0x06001879 RID: 6265 RVA: 0x000A6B8F File Offset: 0x000A4D8F
		private void OnMobilePartyRaftStateChanged(MobileParty mobileParty)
		{
			if (mobileParty.IsMainParty && mobileParty.IsActive && mobileParty.IsInRaftState)
			{
				GameMenu.ActivateGameMenu("player_raft_state");
			}
		}

		// Token: 0x0600187A RID: 6266 RVA: 0x000A6BB4 File Offset: 0x000A4DB4
		private void GiveCachedShipsToMainParty()
		{
			foreach (KeyValuePair<Ship, List<ShipUpgradePiece>> keyValuePair in this._playerCachedShips)
			{
				Ship key = keyValuePair.Key;
				List<ShipUpgradePiece> value = keyValuePair.Value;
				ChangeShipOwnerAction.ApplyByTransferring(PartyBase.MainParty, key);
				if (value.Count > 0)
				{
					foreach (KeyValuePair<string, ShipSlot> keyValuePair2 in key.ShipHull.AvailableSlots)
					{
						string key2 = keyValuePair2.Key;
						ShipUpgradePiece pieceAtSlot = key.GetPieceAtSlot(key2);
						foreach (ShipUpgradePiece shipUpgradePiece in value)
						{
							if (shipUpgradePiece.DoesPieceMatchSlot(keyValuePair2.Value))
							{
								key.EquipUpgradePiece(key2, shipUpgradePiece);
							}
						}
						if (pieceAtSlot != null)
						{
							key.EquipUpgradePiece(key2, pieceAtSlot);
						}
					}
				}
			}
			this._playerCachedShips.Clear();
		}

		// Token: 0x0600187B RID: 6267 RVA: 0x000A6CF0 File Offset: 0x000A4EF0
		private void CanHaveCampaignIssues(Hero hero, ref bool canHaveCampaignIssues)
		{
			if ((hero.PartyBelongedTo != null) & canHaveCampaignIssues)
			{
				canHaveCampaignIssues = !hero.PartyBelongedTo.IsCurrentlyAtSea;
			}
		}

		// Token: 0x04000C00 RID: 3072
		private Dictionary<Ship, List<ShipUpgradePiece>> _playerCachedShips = new Dictionary<Ship, List<ShipUpgradePiece>>();

		// Token: 0x04000C01 RID: 3073
		private AnchorPoint _cachedAnchorPoint;
	}
}
