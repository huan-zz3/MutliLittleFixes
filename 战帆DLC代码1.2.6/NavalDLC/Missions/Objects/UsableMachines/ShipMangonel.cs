using System;
using NavalDLC.Missions.MissionLogics;
using TaleWorlds.DotNet;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.Objects.UsableMachines
{
	// Token: 0x020000B7 RID: 183
	public class ShipMangonel : Mangonel
	{
		// Token: 0x1700028E RID: 654
		// (get) Token: 0x06000E05 RID: 3589 RVA: 0x0006DCC0 File Offset: 0x0006BEC0
		public override string MultipleProjectileId
		{
			get
			{
				return "mangonel_c_grapeshot_stack";
			}
		}

		// Token: 0x1700028F RID: 655
		// (get) Token: 0x06000E06 RID: 3590 RVA: 0x0006DCC7 File Offset: 0x0006BEC7
		public override float DirectionRestriction
		{
			get
			{
				return this._directionRestriction;
			}
		}

		// Token: 0x17000290 RID: 656
		// (get) Token: 0x06000E07 RID: 3591 RVA: 0x0006DCCF File Offset: 0x0006BECF
		public override string MultipleProjectileFlyingId
		{
			get
			{
				return "mangonel_c_grapeshot_projectile";
			}
		}

		// Token: 0x17000291 RID: 657
		// (get) Token: 0x06000E08 RID: 3592 RVA: 0x0006DCD6 File Offset: 0x0006BED6
		public override string MultipleFireProjectileId
		{
			get
			{
				return "mangonel_c_grapeshot_fire_stack";
			}
		}

		// Token: 0x17000292 RID: 658
		// (get) Token: 0x06000E09 RID: 3593 RVA: 0x0006DCDD File Offset: 0x0006BEDD
		public override string MultipleFireProjectileFlyingId
		{
			get
			{
				return "mangonel_c_grapeshot_fire_projectile";
			}
		}

		// Token: 0x17000293 RID: 659
		// (get) Token: 0x06000E0A RID: 3594 RVA: 0x0006DCE4 File Offset: 0x0006BEE4
		protected override float ReloadSpeedMultiplier
		{
			get
			{
				return 6.2f;
			}
		}

		// Token: 0x17000294 RID: 660
		// (get) Token: 0x06000E0B RID: 3595 RVA: 0x0006DCEB File Offset: 0x0006BEEB
		protected override float HorizontalAimSensitivity
		{
			get
			{
				return 0.5f;
			}
		}

		// Token: 0x06000E0C RID: 3596 RVA: 0x0006DCF4 File Offset: 0x0006BEF4
		protected override void OnInit()
		{
			this._ship = base.GameEntity.Root.GetFirstScriptOfType<MissionShip>();
			base.OnInit();
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this._navalShipsLogic.ShipSpawnedEvent += this.OnShipSpawned;
			foreach (StandingPoint standingPoint in base.StandingPoints)
			{
				standingPoint.IsDisabledForPlayers = true;
			}
		}

		// Token: 0x06000E0D RID: 3597 RVA: 0x0006DD90 File Offset: 0x0006BF90
		private void OnShipSpawned(MissionShip ship)
		{
			if (ship == this._ship)
			{
				this.DefaultSide = ship.BattleSide;
			}
			this._navalShipsLogic.ShipSpawnedEvent -= this.OnShipSpawned;
		}

		// Token: 0x040008BF RID: 2239
		private MissionShip _ship;

		// Token: 0x040008C0 RID: 2240
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x040008C1 RID: 2241
		[EditableScriptComponentVariable(true, "")]
		private float _directionRestriction = 2.0943952f;
	}
}
