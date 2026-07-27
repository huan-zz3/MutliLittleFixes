using System;
using System.Collections.Generic;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
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
	// Token: 0x02000005 RID: 5
	public class NavalCustomBattleScoreboardVM : CustomBattleScoreboardVM
	{
		// Token: 0x0600000B RID: 11 RVA: 0x00003D94 File Offset: 0x00001F94
		public static NavalCustomBattleScoreboardVM Create(Mission mission, BattleScoreContext scoreboardContext = null)
		{
			return new NavalCustomBattleScoreboardVM(scoreboardContext ?? new CustomBattleScoreContext(mission));
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00003DA6 File Offset: 0x00001FA6
		private NavalCustomBattleScoreboardVM(BattleScoreContext scoreboardContext)
			: base(scoreboardContext)
		{
			SPScoreboardShipVM.GetTooltip = new Func<SPScoreboardShipVM, List<TooltipProperty>>(this.GetShipTooltip);
			base.IsNavalBattle = true;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00003DD2 File Offset: 0x00001FD2
		public override void Initialize(IMissionScreen missionScreen, Mission mission, Action releaseSimulationSources, Action<bool> onToggle)
		{
			base.Initialize(missionScreen, mission, releaseSimulationSources, onToggle);
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00003DF0 File Offset: 0x00001FF0
		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (this._navalShipsLogic != null)
			{
				this.UpdateTeamShips(false, true, false);
				for (int i = 0; i < base.Attackers.Ships.Count; i++)
				{
					SPScoreboardShipVM spscoreboardShipVM = base.Attackers.Ships[i];
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
				for (int j = 0; j < base.Defenders.Ships.Count; j++)
				{
					SPScoreboardShipVM spscoreboardShipVM3 = base.Defenders.Ships[j];
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

		// Token: 0x0600000F RID: 15 RVA: 0x00003F23 File Offset: 0x00002123
		public override void OnFinalize()
		{
			base.OnFinalize();
			SPScoreboardShipVM.GetTooltip = null;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00003F31 File Offset: 0x00002131
		public override void OnDeploymentFinished()
		{
			base.OnDeploymentFinished();
			this.UpdateTeamShips(true, true, true);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00003F44 File Offset: 0x00002144
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
					base.Attackers.GetShipAddIfNotExists(missionShip.ShipOrigin, missionShip.ShipOrigin.Hull.Type.ToString(), null, Mission.Current.AttackerTeam.TeamSide);
				}
				for (int l = 0; l < mblist2.Count; l++)
				{
					MissionShip missionShip2 = mblist2[l];
					base.Attackers.GetShipAddIfNotExists(missionShip2.ShipOrigin, missionShip2.ShipOrigin.Hull.Type.ToString(), null, Mission.Current.AttackerAllyTeam.TeamSide);
				}
				for (int m = 0; m < mblist3.Count; m++)
				{
					MissionShip missionShip3 = mblist3[m];
					base.Defenders.GetShipAddIfNotExists(missionShip3.ShipOrigin, missionShip3.ShipOrigin.Hull.Type.ToString(), null, Mission.Current.DefenderTeam.TeamSide);
				}
				for (int n = 0; n < mblist4.Count; n++)
				{
					MissionShip missionShip4 = mblist4[n];
					base.Defenders.GetShipAddIfNotExists(missionShip4.ShipOrigin, missionShip4.ShipOrigin.Hull.Type.ToString(), null, Mission.Current.DefenderAllyTeam.TeamSide);
				}
			}
			if (sort)
			{
				base.Attackers.Ships.Sort(this._scoreboardShipComparer);
				base.Defenders.Ships.Sort(this._scoreboardShipComparer);
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00004264 File Offset: 0x00002464
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

		// Token: 0x04000004 RID: 4
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x04000005 RID: 5
		private readonly NavalCustomBattleScoreboardVM.ScoreboardShipComparer _scoreboardShipComparer = new NavalCustomBattleScoreboardVM.ScoreboardShipComparer();

		// Token: 0x02000041 RID: 65
		private class ScoreboardShipComparer : IComparer<SPScoreboardShipVM>
		{
			// Token: 0x06000469 RID: 1129 RVA: 0x00014898 File Offset: 0x00012A98
			public int Compare(SPScoreboardShipVM x, SPScoreboardShipVM y)
			{
				bool isPlayerShip = x.Ship.IsPlayerShip;
				int num = y.Ship.IsPlayerShip.CompareTo(isPlayerShip);
				if (num != 0)
				{
					return num;
				}
				num = y.IsPlayerTeam.CompareTo(x.IsPlayerTeam);
				if (num != 0)
				{
					return num;
				}
				return this.ResolveEquality(x, y);
			}

			// Token: 0x0600046A RID: 1130 RVA: 0x000148F0 File Offset: 0x00012AF0
			private int ResolveEquality(SPScoreboardShipVM x, SPScoreboardShipVM y)
			{
				return y.Ship.MaxHitPoints.CompareTo(x.Ship.MaxHitPoints);
			}
		}
	}
}
