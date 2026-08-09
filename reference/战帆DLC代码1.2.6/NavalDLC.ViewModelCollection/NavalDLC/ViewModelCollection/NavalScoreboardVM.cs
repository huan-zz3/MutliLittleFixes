using System;
using System.Collections.Generic;
using Helpers;
using NavalDLC.Missions.BattleScore;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using SandBox.ViewModelCollection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.BattleScore;
using TaleWorlds.MountAndBlade.ViewModelCollection;
using TaleWorlds.MountAndBlade.ViewModelCollection.Scoreboard;

namespace NavalDLC.ViewModelCollection
{
	// Token: 0x02000006 RID: 6
	public class NavalScoreboardVM : SPScoreboardVM
	{
		// Token: 0x06000013 RID: 19 RVA: 0x000045D0 File Offset: 0x000027D0
		public static NavalScoreboardVM CreateSimulation(BattleSimulation simulation)
		{
			return new NavalScoreboardVM(new NavalSimulationBattleScoreContext(simulation), simulation);
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000045DE File Offset: 0x000027DE
		public static NavalScoreboardVM CreateMission(Mission mission)
		{
			return new NavalScoreboardVM(new NavalBattleScoreContext(mission), null);
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000045EC File Offset: 0x000027EC
		public static NavalScoreboardVM CreateCustom(BattleScoreContext battleScoreContext, BattleSimulation simulation = null)
		{
			return new NavalScoreboardVM(battleScoreContext, simulation);
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000045F5 File Offset: 0x000027F5
		private NavalScoreboardVM(BattleScoreContext scoreboardContext, BattleSimulation simulation)
			: base(scoreboardContext, simulation)
		{
			SPScoreboardShipVM.GetTooltip = new Func<SPScoreboardShipVM, List<TooltipProperty>>(this.GetShipTooltip);
			base.IsNavalBattle = true;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00004624 File Offset: 0x00002824
		public override void Initialize(IMissionScreen missionScreen, Mission mission, Action releaseSimulationSources, Action<bool> onToggle)
		{
			base.Initialize(missionScreen, mission, releaseSimulationSources, onToggle);
			if (base.IsSimulation)
			{
				MobileParty mainParty = MobileParty.MainParty;
				MapEvent mapEvent = ((mainParty != null) ? mainParty.MapEvent : null);
				if (mapEvent == null || (!mapEvent.IsNavalMapEvent && !MapEventHelper.IsNavalRaid(mapEvent)))
				{
					Debug.FailedAssert("Naval scoreboard initialized in simulation mode, but the current map event isn't naval!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.ViewModelCollection\\NavalScoreboardVM.cs", "Initialize", 59);
					return;
				}
			}
			else
			{
				Mission mission2 = Mission.Current;
				if (mission2 == null || (!mission2.IsNavalBattle && !mission2.IsNavalRaidBattle))
				{
					Debug.FailedAssert("Naval scoreboard initialized in mission mode, but the current mission isn't naval!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.ViewModelCollection\\NavalScoreboardVM.cs", "Initialize", 69);
					return;
				}
			}
			if (base.IsSimulation)
			{
				bool flag = MobileParty.MainParty.MapEvent.PlayerSide == 1;
				foreach (Ship ship in MobileParty.MainParty.MapEvent.AttackerSide.SimulationShipList)
				{
					TeamSideEnum teamSideEnum;
					if (flag)
					{
						if (ship.Owner != PartyBase.MainParty)
						{
							Army army = MobileParty.MainParty.Army;
							if (army == null || !army.DoesLeaderPartyAndAttachedPartiesContain(ship.Owner.MobileParty))
							{
								teamSideEnum = 1;
								goto IL_0104;
							}
						}
						teamSideEnum = 0;
					}
					else
					{
						teamSideEnum = 2;
					}
					IL_0104:
					base.Attackers.GetShipAddIfNotExists(ship, ship.ShipHull.Type.ToString(), ship.Owner, teamSideEnum);
				}
				MobileParty mainParty2 = MobileParty.MainParty;
				if (!MapEventHelper.IsNavalRaid((mainParty2 != null) ? mainParty2.MapEvent : null))
				{
					foreach (Ship ship2 in MobileParty.MainParty.MapEvent.DefenderSide.SimulationShipList)
					{
						TeamSideEnum teamSideEnum2;
						if (flag)
						{
							teamSideEnum2 = 2;
						}
						else
						{
							if (ship2.Owner != PartyBase.MainParty)
							{
								Army army2 = MobileParty.MainParty.Army;
								if (army2 == null || !army2.DoesLeaderPartyAndAttachedPartiesContain(ship2.Owner.MobileParty))
								{
									teamSideEnum2 = 1;
									goto IL_01D4;
								}
							}
							teamSideEnum2 = 0;
						}
						IL_01D4:
						base.Defenders.GetShipAddIfNotExists(ship2, ship2.ShipHull.Type.ToString(), ship2.Owner, teamSideEnum2);
					}
				}
				base.Attackers.Ships.Sort(this._scoreboardShipComparer);
				base.Defenders.Ships.Sort(this._scoreboardShipComparer);
				return;
			}
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000048AC File Offset: 0x00002AAC
		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (base.IsSimulation)
			{
				for (int i = 0; i < base.Attackers.Ships.Count; i++)
				{
					base.Attackers.Ships[i].CurrentHealth = base.Attackers.Ships[i].Ship.HitPoints;
				}
				for (int j = 0; j < base.Defenders.Ships.Count; j++)
				{
					base.Defenders.Ships[j].CurrentHealth = base.Defenders.Ships[j].Ship.HitPoints;
				}
				return;
			}
			if (this._navalShipsLogic != null)
			{
				this.UpdateTeamShips(false, true, false);
				for (int k = 0; k < base.Attackers.Ships.Count; k++)
				{
					SPScoreboardShipVM spscoreboardShipVM = base.Attackers.Ships[k];
					ShipAssignment shipAssignment;
					bool flag = this._navalShipsLogic.FindAssignmentOfShipOrigin(spscoreboardShipVM.Ship, out shipAssignment);
					spscoreboardShipVM.CurrentHealth = (flag ? shipAssignment.MissionShip.HitPoints : 0f);
					SPScoreboardShipVM spscoreboardShipVM2 = spscoreboardShipVM;
					bool flag2;
					if (flag)
					{
						Formation formation = shipAssignment.Formation;
						flag2 = formation != null && formation.CountOfUnits == 0;
					}
					else
					{
						flag2 = true;
					}
					spscoreboardShipVM2.IsInactive = flag2;
				}
				for (int l = 0; l < base.Defenders.Ships.Count; l++)
				{
					SPScoreboardShipVM spscoreboardShipVM3 = base.Defenders.Ships[l];
					ShipAssignment shipAssignment2;
					bool flag3 = this._navalShipsLogic.FindAssignmentOfShipOrigin(spscoreboardShipVM3.Ship, out shipAssignment2);
					spscoreboardShipVM3.CurrentHealth = (flag3 ? shipAssignment2.MissionShip.HitPoints : 0f);
					SPScoreboardShipVM spscoreboardShipVM4 = spscoreboardShipVM3;
					bool flag4;
					if (flag3)
					{
						Formation formation2 = shipAssignment2.Formation;
						flag4 = formation2 != null && formation2.CountOfUnits == 0;
					}
					else
					{
						flag4 = true;
					}
					spscoreboardShipVM4.IsInactive = flag4;
				}
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00004A88 File Offset: 0x00002C88
		public override void OnFinalize()
		{
			base.OnFinalize();
			SPScoreboardShipVM.GetTooltip = null;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00004A96 File Offset: 0x00002C96
		public override void OnDeploymentFinished()
		{
			base.OnDeploymentFinished();
			this.UpdateTeamShips(true, true, true);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00004AA8 File Offset: 0x00002CA8
		private void UpdateTeamShips(bool removeOld, bool addNew, bool sort)
		{
			if (removeOld)
			{
				for (int i = base.Attackers.Ships.Count - 1; i >= 0; i--)
				{
					ShipAssignment shipAssignment;
					if (!this._navalShipsLogic.FindAssignmentOfShipOrigin(base.Attackers.Ships[i].Ship, out shipAssignment))
					{
						base.Attackers.Ships.RemoveAt(i);
					}
				}
				for (int j = base.Defenders.Ships.Count - 1; j >= 0; j--)
				{
					ShipAssignment shipAssignment;
					if (!this._navalShipsLogic.FindAssignmentOfShipOrigin(base.Defenders.Ships[j].Ship, out shipAssignment))
					{
						base.Defenders.Ships.RemoveAt(j);
					}
				}
			}
			if (addNew)
			{
				MBList<MissionShip> mblist = new MBList<MissionShip>();
				this._navalShipsLogic.FillTeamShips(Mission.Current.AttackerTeam.TeamSide, mblist);
				MBList<MissionShip> mblist2 = new MBList<MissionShip>();
				if (Mission.Current.AttackerAllyTeam != null)
				{
					this._navalShipsLogic.FillTeamShips(Mission.Current.AttackerAllyTeam.TeamSide, mblist2);
				}
				MBList<MissionShip> mblist3 = new MBList<MissionShip>();
				this._navalShipsLogic.FillTeamShips(Mission.Current.DefenderTeam.TeamSide, mblist3);
				MBList<MissionShip> mblist4 = new MBList<MissionShip>();
				if (Mission.Current.DefenderAllyTeam != null)
				{
					this._navalShipsLogic.FillTeamShips(Mission.Current.DefenderAllyTeam.TeamSide, mblist4);
				}
				for (int k = 0; k < mblist.Count; k++)
				{
					MissionShip missionShip = mblist[k];
					base.Attackers.GetShipAddIfNotExists(missionShip.ShipOrigin, missionShip.ShipOrigin.Hull.Type.ToString(), (missionShip.ShipOrigin as Ship).Owner, Mission.Current.AttackerTeam.TeamSide);
				}
				for (int l = 0; l < mblist2.Count; l++)
				{
					MissionShip missionShip2 = mblist2[l];
					base.Attackers.GetShipAddIfNotExists(missionShip2.ShipOrigin, missionShip2.ShipOrigin.Hull.Type.ToString(), (missionShip2.ShipOrigin as Ship).Owner, Mission.Current.AttackerAllyTeam.TeamSide);
				}
				for (int m = 0; m < mblist3.Count; m++)
				{
					MissionShip missionShip3 = mblist3[m];
					base.Defenders.GetShipAddIfNotExists(missionShip3.ShipOrigin, missionShip3.ShipOrigin.Hull.Type.ToString(), (missionShip3.ShipOrigin as Ship).Owner, Mission.Current.DefenderTeam.TeamSide);
				}
				for (int n = 0; n < mblist4.Count; n++)
				{
					MissionShip missionShip4 = mblist4[n];
					base.Defenders.GetShipAddIfNotExists(missionShip4.ShipOrigin, missionShip4.ShipOrigin.Hull.Type.ToString(), (missionShip4.ShipOrigin as Ship).Owner, Mission.Current.DefenderAllyTeam.TeamSide);
				}
			}
			if (sort)
			{
				base.Attackers.Ships.Sort(this._scoreboardShipComparer);
				base.Defenders.Ships.Sort(this._scoreboardShipComparer);
			}
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00004E08 File Offset: 0x00003008
		private List<TooltipProperty> GetShipTooltip(SPScoreboardShipVM shipVM)
		{
			IShipOrigin ship = shipVM.Ship;
			List<TooltipProperty> list = new List<TooltipProperty>
			{
				new TooltipProperty(ship.Name.ToString(), string.Empty, 0, false, 4096)
			};
			if (shipVM.IsDestroyed)
			{
				list.Add(new TooltipProperty(string.Empty, new TextObject("{=w8Yzf0F0}Destroyed", null).ToString(), -1, false, 0));
				list.Add(new TooltipProperty(string.Empty, string.Empty, 0, false, 0));
			}
			if (shipVM.Owner != null)
			{
				list.Add(new TooltipProperty(GameTexts.FindText("str_owner", null).ToString(), shipVM.Owner.Name.ToString(), 0, false, 0));
			}
			list.Add(new TooltipProperty(new TextObject("{=wEmx6fZi}Hull", null).ToString(), ship.Hull.Name.ToString(), 0, false, 0));
			list.Add(new TooltipProperty(new TextObject("{=sqdzHOPe}Class", null).ToString(), GameTexts.FindText("str_ship_type", ship.Hull.Type.ToString().ToLowerInvariant()).ToString(), 0, false, 0));
			MissionShip missionShip = null;
			ShipAssignment shipAssignment;
			if (this._navalShipsLogic != null && this._navalShipsLogic.FindAssignmentOfShipOrigin(ship, out shipAssignment))
			{
				missionShip = shipAssignment.MissionShip;
			}
			if (missionShip == null)
			{
				string text = GameTexts.FindText("str_LEFT_over_RIGHT_no_space", null).SetTextVariable("LEFT", (int)ship.HitPoints).SetTextVariable("RIGHT", (int)ship.MaxHitPoints)
					.ToString();
				list.Add(new TooltipProperty(new TextObject("{=oBbiVeKE}Hit Points", null).ToString(), text, 0, false, 0));
			}
			else
			{
				string text2 = GameTexts.FindText("str_LEFT_over_RIGHT_no_space", null).SetTextVariable("LEFT", (int)missionShip.HitPoints).SetTextVariable("RIGHT", (int)ship.MaxHitPoints)
					.ToString();
				list.Add(new TooltipProperty(new TextObject("{=oBbiVeKE}Hit Points", null).ToString(), text2, 0, false, 0));
				TextObject textObject = GameTexts.FindText("str_LEFT_over_RIGHT_no_space", null);
				string text3 = "LEFT";
				Formation formation = missionShip.Formation;
				string text4 = textObject.SetTextVariable(text3, (formation != null) ? formation.CountOfUnits : 0).SetTextVariable("RIGHT", missionShip.CrewSizeOnMainDeck).ToString();
				list.Add(new TooltipProperty(new TextObject("{=aClquusd}Troop Count", null).ToString(), text4, 0, false, 0));
			}
			List<ShipSlotAndPieceName> shipSlotAndPieceNames = ship.GetShipSlotAndPieceNames();
			if (shipSlotAndPieceNames.Count > 0)
			{
				list.Add(new TooltipProperty(string.Empty, string.Empty, 0, false, 1024)
				{
					OnlyShowWhenExtended = true
				});
				list.Add(new TooltipProperty(string.Empty, new TextObject("{=zMvUzdKR}Ship Upgrades", null).ToString(), -1, false, 0)
				{
					OnlyShowWhenExtended = true
				});
				foreach (ShipSlotAndPieceName shipSlotAndPieceName in shipSlotAndPieceNames)
				{
					list.Add(new TooltipProperty(shipSlotAndPieceName.SlotName, shipSlotAndPieceName.PieceName, 0, false, 0)
					{
						OnlyShowWhenExtended = true
					});
				}
			}
			if (shipSlotAndPieceNames.Count > 0)
			{
				if (Input.IsGamepadActive)
				{
					GameTexts.SetVariable("EXTEND_KEY", GameKeyTextExtensions.GetHotKeyGameText(Game.Current.GameTextManager, "MapHotKeyCategory", "MapFollowModifier").ToString());
				}
				else
				{
					GameTexts.SetVariable("EXTEND_KEY", Game.Current.GameTextManager.FindText("str_game_key_text", "anyalt").ToString());
				}
				list.Add(new TooltipProperty(string.Empty, string.Empty, 0, false, 0)
				{
					OnlyShowWhenNotExtended = true
				});
				list.Add(new TooltipProperty(string.Empty, GameTexts.FindText("str_map_tooltip_info", null).ToString(), -1, false, 0)
				{
					OnlyShowWhenNotExtended = true
				});
			}
			return list;
		}

		// Token: 0x04000006 RID: 6
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x04000007 RID: 7
		private NavalScoreboardVM.ScoreboardShipComparer _scoreboardShipComparer = new NavalScoreboardVM.ScoreboardShipComparer();

		// Token: 0x02000042 RID: 66
		private class ScoreboardShipComparer : IComparer<SPScoreboardShipVM>
		{
			// Token: 0x0600046C RID: 1132 RVA: 0x00014924 File Offset: 0x00012B24
			public int Compare(SPScoreboardShipVM x, SPScoreboardShipVM y)
			{
				bool flag = x.Owner == PartyBase.MainParty;
				int num = (y.Owner == PartyBase.MainParty).CompareTo(flag);
				if (num != 0)
				{
					return num;
				}
				num = y.IsPlayerTeam.CompareTo(x.IsPlayerTeam);
				if (num != 0)
				{
					return num;
				}
				IBattleCombatant owner = x.Owner;
				string text = ((owner != null) ? owner.Name.ToString() : null) ?? string.Empty;
				IBattleCombatant owner2 = y.Owner;
				string text2 = ((owner2 != null) ? owner2.Name.ToString() : null) ?? string.Empty;
				num = text.CompareTo(text2);
				if (num != 0)
				{
					return num;
				}
				return this.ResolveEquality(x, y);
			}

			// Token: 0x0600046D RID: 1133 RVA: 0x000149CC File Offset: 0x00012BCC
			private int ResolveEquality(SPScoreboardShipVM x, SPScoreboardShipVM y)
			{
				return (y.Ship as Ship).ShipHull.Value.CompareTo((x.Ship as Ship).ShipHull.Value);
			}
		}
	}
}
