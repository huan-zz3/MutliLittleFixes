using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.ViewModelCollection.HUD.ShipMarker
{
	// Token: 0x02000035 RID: 53
	public class NavalShipMarkersVM : ViewModel
	{
		// Token: 0x06000422 RID: 1058 RVA: 0x00013803 File Offset: 0x00011A03
		public NavalShipMarkersVM(Mission mission)
		{
			this._mission = mission;
			this._comparer = new NavalShipMarkersVM.ShipMarkerDistanceComparer();
			this.ShipMarkers = new MBBindingList<NavalShipMarkerItemVM>();
		}

		// Token: 0x06000423 RID: 1059 RVA: 0x00013828 File Offset: 0x00011A28
		public void RefreshShipMarkers()
		{
			if (this._navalShipsLogic == null)
			{
				this._navalShipsLogic = this._mission.GetMissionBehavior<NavalShipsLogic>();
			}
			if (this._navalShipsLogic == null)
			{
				this.ShipMarkers.Clear();
				return;
			}
			List<Formation> list = this._mission.Teams.SelectMany<Team, Formation>((Team x) => x.FormationsIncludingSpecialAndEmpty).ToList<Formation>();
			MBList<NavalShipMarkerItemVM> mblist;
			MBList<NavalShipMarkerItemVM> mblist2;
			this.GetShipChanges(list, this.ShipMarkers, out mblist, out mblist2);
			for (int i = 0; i < mblist.Count; i++)
			{
				NavalShipMarkerItemVM navalShipMarkerItemVM = mblist[i];
				this.ShipMarkers.Remove(navalShipMarkerItemVM);
			}
			for (int j = 0; j < mblist2.Count; j++)
			{
				NavalShipMarkerItemVM navalShipMarkerItemVM2 = mblist2[j];
				this.ShipMarkers.Add(navalShipMarkerItemVM2);
				navalShipMarkerItemVM2.IsEnabled = this.IsEnabled;
				navalShipMarkerItemVM2.IsShipTargetRelevant = this.IsShipTargetingRelevant;
				navalShipMarkerItemVM2.ShowDistanceTexts = this.ShowDistanceTexts;
			}
			this.ShipMarkers.Sort(this._comparer);
			for (int k = 0; k < this.ShipMarkers.Count; k++)
			{
				NavalShipMarkerItemVM navalShipMarkerItemVM3 = this.ShipMarkers[k];
				navalShipMarkerItemVM3.Refresh();
				navalShipMarkerItemVM3.IsEnabled = this.IsEnabled && (navalShipMarkerItemVM3.Ship == null || navalShipMarkerItemVM3.Ship != this._navalShipsLogic.PlayerControlledShip);
			}
		}

		// Token: 0x06000424 RID: 1060 RVA: 0x0001399C File Offset: 0x00011B9C
		private void GetShipChanges(List<Formation> allFormations, MBBindingList<NavalShipMarkerItemVM> activeMarkers, out MBList<NavalShipMarkerItemVM> markersToRemove, out MBList<NavalShipMarkerItemVM> markersToAdd)
		{
			markersToAdd = new MBList<NavalShipMarkerItemVM>();
			markersToRemove = new MBList<NavalShipMarkerItemVM>();
			List<ValueTuple<Formation, MissionShip>> list = new List<ValueTuple<Formation, MissionShip>>();
			for (int i = 0; i < allFormations.Count; i++)
			{
				Formation formation = allFormations[i];
				MissionShip missionShip;
				this._navalShipsLogic.GetShip(formation, out missionShip);
				if ((missionShip != null || formation.CountOfUnits > 0) && (missionShip == null || (!missionShip.IsDisabled && !missionShip.IsRemoved)))
				{
					list.Add(new ValueTuple<Formation, MissionShip>(formation, missionShip));
				}
			}
			for (int j = 0; j < activeMarkers.Count; j++)
			{
				NavalShipMarkerItemVM navalShipMarkerItemVM = activeMarkers[j];
				bool flag = false;
				for (int k = 0; k < list.Count; k++)
				{
					Formation item = list[k].Item1;
					MissionShip item2 = list[k].Item2;
					if (item == navalShipMarkerItemVM.Formation && item2 == navalShipMarkerItemVM.Ship)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					markersToRemove.Add(navalShipMarkerItemVM);
				}
			}
			for (int l = 0; l < list.Count; l++)
			{
				Formation item3 = list[l].Item1;
				MissionShip item4 = list[l].Item2;
				bool flag2 = false;
				for (int m = 0; m < activeMarkers.Count; m++)
				{
					NavalShipMarkerItemVM navalShipMarkerItemVM2 = activeMarkers[m];
					if (navalShipMarkerItemVM2.Formation == item3 && navalShipMarkerItemVM2.Ship == item4)
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					markersToAdd.Add(new NavalShipMarkerItemVM(item3, item4));
				}
			}
		}

		// Token: 0x06000425 RID: 1061 RVA: 0x00013B10 File Offset: 0x00011D10
		public void UpdateCrewCounts()
		{
			for (int i = 0; i < this.ShipMarkers.Count; i++)
			{
				NavalShipMarkerItemVM navalShipMarkerItemVM = this.ShipMarkers[i];
				navalShipMarkerItemVM.CrewCount = navalShipMarkerItemVM.Formation.CountOfUnits;
			}
		}

		// Token: 0x1700011A RID: 282
		// (get) Token: 0x06000426 RID: 1062 RVA: 0x00013B4F File Offset: 0x00011D4F
		// (set) Token: 0x06000427 RID: 1063 RVA: 0x00013B58 File Offset: 0x00011D58
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
					for (int i = 0; i < this.ShipMarkers.Count; i++)
					{
						this.ShipMarkers[i].IsEnabled = value;
					}
				}
			}
		}

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x06000428 RID: 1064 RVA: 0x00013BA9 File Offset: 0x00011DA9
		// (set) Token: 0x06000429 RID: 1065 RVA: 0x00013BB4 File Offset: 0x00011DB4
		[DataSourceProperty]
		public bool IsShipTargetingRelevant
		{
			get
			{
				return this._isShipTargetingRelevant;
			}
			set
			{
				if (value != this._isShipTargetingRelevant)
				{
					this._isShipTargetingRelevant = value;
					base.OnPropertyChangedWithValue(value, "IsShipTargetingRelevant");
					for (int i = 0; i < this.ShipMarkers.Count; i++)
					{
						this.ShipMarkers[i].IsShipTargetRelevant = value;
					}
				}
			}
		}

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x0600042A RID: 1066 RVA: 0x00013C05 File Offset: 0x00011E05
		// (set) Token: 0x0600042B RID: 1067 RVA: 0x00013C10 File Offset: 0x00011E10
		[DataSourceProperty]
		public bool ShowDistanceTexts
		{
			get
			{
				return this._showDistanceTexts;
			}
			set
			{
				if (value != this._showDistanceTexts)
				{
					this._showDistanceTexts = value;
					base.OnPropertyChangedWithValue(value, "ShowDistanceTexts");
					for (int i = 0; i < this.ShipMarkers.Count; i++)
					{
						this.ShipMarkers[i].ShowDistanceTexts = value;
					}
				}
			}
		}

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x0600042C RID: 1068 RVA: 0x00013C61 File Offset: 0x00011E61
		// (set) Token: 0x0600042D RID: 1069 RVA: 0x00013C69 File Offset: 0x00011E69
		[DataSourceProperty]
		public MBBindingList<NavalShipMarkerItemVM> ShipMarkers
		{
			get
			{
				return this._shipMarkers;
			}
			set
			{
				if (value != this._shipMarkers)
				{
					this._shipMarkers = value;
					base.OnPropertyChangedWithValue<MBBindingList<NavalShipMarkerItemVM>>(value, "ShipMarkers");
				}
			}
		}

		// Token: 0x0400019C RID: 412
		private readonly Mission _mission;

		// Token: 0x0400019D RID: 413
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x0400019E RID: 414
		private readonly NavalShipMarkersVM.ShipMarkerDistanceComparer _comparer;

		// Token: 0x0400019F RID: 415
		private bool _isEnabled;

		// Token: 0x040001A0 RID: 416
		private bool _isShipTargetingRelevant;

		// Token: 0x040001A1 RID: 417
		private bool _showDistanceTexts;

		// Token: 0x040001A2 RID: 418
		private MBBindingList<NavalShipMarkerItemVM> _shipMarkers;

		// Token: 0x02000076 RID: 118
		public class ShipMarkerDistanceComparer : IComparer<NavalShipMarkerItemVM>
		{
			// Token: 0x06000519 RID: 1305 RVA: 0x0001520C File Offset: 0x0001340C
			public int Compare(NavalShipMarkerItemVM x, NavalShipMarkerItemVM y)
			{
				return y.Distance.CompareTo(x.Distance);
			}
		}
	}
}
