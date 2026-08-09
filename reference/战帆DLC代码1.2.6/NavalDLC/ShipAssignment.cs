using System;
using NavalDLC.Missions.Objects;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace NavalDLC
{
	// Token: 0x02000027 RID: 39
	public class ShipAssignment
	{
		// Token: 0x1700004F RID: 79
		// (get) Token: 0x0600019B RID: 411 RVA: 0x0000A5EB File Offset: 0x000087EB
		// (set) Token: 0x0600019C RID: 412 RVA: 0x0000A5F3 File Offset: 0x000087F3
		public TeamSideEnum TeamSide { get; private set; }

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x0600019D RID: 413 RVA: 0x0000A5FC File Offset: 0x000087FC
		// (set) Token: 0x0600019E RID: 414 RVA: 0x0000A604 File Offset: 0x00008804
		public FormationClass FormationIndex { get; private set; }

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x0600019F RID: 415 RVA: 0x0000A60D File Offset: 0x0000880D
		// (set) Token: 0x060001A0 RID: 416 RVA: 0x0000A615 File Offset: 0x00008815
		public MissionShipObject MissionShipObject { get; private set; }

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x060001A1 RID: 417 RVA: 0x0000A61E File Offset: 0x0000881E
		// (set) Token: 0x060001A2 RID: 418 RVA: 0x0000A626 File Offset: 0x00008826
		public IShipOrigin ShipOrigin { get; private set; }

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060001A3 RID: 419 RVA: 0x0000A62F File Offset: 0x0000882F
		// (set) Token: 0x060001A4 RID: 420 RVA: 0x0000A637 File Offset: 0x00008837
		public MissionShip MissionShip { get; private set; }

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060001A5 RID: 421 RVA: 0x0000A640 File Offset: 0x00008840
		public Formation Formation
		{
			get
			{
				MissionShip missionShip = this.MissionShip;
				if (missionShip == null)
				{
					return null;
				}
				return missionShip.Formation;
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060001A6 RID: 422 RVA: 0x0000A653 File Offset: 0x00008853
		public bool IsSet
		{
			get
			{
				return this.ShipOrigin != null && this.MissionShipObject != null;
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060001A7 RID: 423 RVA: 0x0000A668 File Offset: 0x00008868
		public bool HasMissionShip
		{
			get
			{
				return this.IsSet && this.MissionShip != null;
			}
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x0000A680 File Offset: 0x00008880
		internal void Set(IShipOrigin shipOrigin)
		{
			this.ShipOrigin = shipOrigin;
			IShipOrigin shipOrigin2 = this.ShipOrigin;
			if (!string.IsNullOrEmpty((shipOrigin2 != null) ? shipOrigin2.OriginShipId : null))
			{
				this.MissionShipObject = MBObjectManager.Instance.GetObject<MissionShipObject>(this.ShipOrigin.OriginShipId);
			}
			this.MissionShip = null;
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x0000A6CF File Offset: 0x000088CF
		internal void RemoveShip()
		{
			this.MissionShip = null;
		}

		// Token: 0x060001AA RID: 426 RVA: 0x0000A6D8 File Offset: 0x000088D8
		internal void Clear()
		{
			this.ShipOrigin = null;
			this.MissionShipObject = null;
			this.MissionShip = null;
		}

		// Token: 0x060001AB RID: 427 RVA: 0x0000A6EF File Offset: 0x000088EF
		internal void SetMissionShip(MissionShip missionShip)
		{
			this.MissionShip = missionShip;
			this.ShipOrigin = missionShip.ShipOrigin;
			this.MissionShipObject = missionShip.MissionShipObject;
		}

		// Token: 0x060001AC RID: 428 RVA: 0x0000A710 File Offset: 0x00008910
		internal static ShipAssignment Create(TeamSideEnum teamSide, FormationClass formationIndex, IShipOrigin shipOrigin = null)
		{
			return new ShipAssignment(teamSide, formationIndex, shipOrigin);
		}

		// Token: 0x060001AD RID: 429 RVA: 0x0000A71A File Offset: 0x0000891A
		private ShipAssignment(TeamSideEnum teamSide, FormationClass formationIndex, IShipOrigin shipOrigin = null)
		{
			this.ShipOrigin = shipOrigin;
			this.TeamSide = teamSide;
			this.FormationIndex = formationIndex;
			if (shipOrigin != null)
			{
				this.Set(shipOrigin);
				return;
			}
			this.ShipOrigin = null;
			this.MissionShipObject = null;
			this.MissionShip = null;
		}
	}
}
