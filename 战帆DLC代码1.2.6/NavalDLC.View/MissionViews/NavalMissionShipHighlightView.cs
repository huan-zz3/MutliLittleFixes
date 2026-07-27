using System;
using System.Collections.Generic;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace NavalDLC.View.MissionViews
{
	// Token: 0x02000021 RID: 33
	public class NavalMissionShipHighlightView : MissionView
	{
		// Token: 0x060000D8 RID: 216 RVA: 0x00007360 File Offset: 0x00005560
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._navalShipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00007379 File Offset: 0x00005579
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			this.UpdateSelectedShipContours();
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00007388 File Offset: 0x00005588
		public override void OnMissionScreenDeactivate()
		{
			base.OnMissionScreenDeactivate();
			this._contourCache.Clear();
		}

		// Token: 0x060000DB RID: 219 RVA: 0x0000739B File Offset: 0x0000559B
		public void OnShipFocused(MissionShip focusedShip)
		{
			this._focusedShip = focusedShip;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x000073A4 File Offset: 0x000055A4
		private void UpdateSelectedShipContours()
		{
			NavalShipsLogic navalShipsLogic = this._navalShipsLogic;
			if (((navalShipsLogic != null) ? navalShipsLogic.AllShips : null) == null)
			{
				foreach (KeyValuePair<MissionShip, ValueTuple<bool, uint>> keyValuePair in this._contourCache)
				{
					MissionShip key = keyValuePair.Key;
					if (key != null && key.GameEntity.IsValid)
					{
						MissionShip key2 = keyValuePair.Key;
						if (key2 != null)
						{
							key2.GameEntity.SetContourColor(null, false);
						}
					}
				}
				return;
			}
			for (int i = 0; i < this._navalShipsLogic.AllShips.Count; i++)
			{
				MissionShip missionShip = this._navalShipsLogic.AllShips[i];
				if (missionShip != null && missionShip.GameEntity.IsValid)
				{
					uint num = 0U;
					bool flag;
					if (base.Mission.Mode == 6 || base.Mission.IsOrderMenuOpen)
					{
						flag = missionShip.Formation != null && (missionShip.Captain == null || missionShip.Captain != Agent.Main) && base.Mission.PlayerTeam.PlayerOrderController.SelectedFormations.Contains(missionShip.Formation);
						num = 4294105105U;
					}
					else
					{
						flag = this._focusedShip == missionShip && base.Input.IsGameKeyDown(5);
						MissionShip focusedShip = this._focusedShip;
						if (((focusedShip != null) ? focusedShip.Team : null) != null)
						{
							switch (this._focusedShip.Team.TeamSide)
							{
							case 0:
								num = 4282512610U;
								break;
							case 1:
								num = 4282578006U;
								break;
							case 2:
								num = 4294197569U;
								break;
							}
						}
					}
					bool flag2 = false;
					ValueTuple<bool, uint> valueTuple;
					if (this._contourCache.TryGetValue(missionShip, out valueTuple))
					{
						if (valueTuple.Item1 != flag || valueTuple.Item2 != num)
						{
							flag2 = true;
							this._contourCache[missionShip] = new ValueTuple<bool, uint>(flag, num);
						}
					}
					else
					{
						flag2 = true;
						this._contourCache[missionShip] = new ValueTuple<bool, uint>(flag, num);
					}
					if (flag2)
					{
						if (flag)
						{
							missionShip.GameEntity.SetContourColor(new uint?(num), true);
						}
						else
						{
							missionShip.GameEntity.SetContourColor(null, false);
						}
					}
				}
			}
		}

		// Token: 0x0400004D RID: 77
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x0400004E RID: 78
		private Dictionary<MissionShip, ValueTuple<bool, uint>> _contourCache = new Dictionary<MissionShip, ValueTuple<bool, uint>>();

		// Token: 0x0400004F RID: 79
		private MissionShip _focusedShip;
	}
}
