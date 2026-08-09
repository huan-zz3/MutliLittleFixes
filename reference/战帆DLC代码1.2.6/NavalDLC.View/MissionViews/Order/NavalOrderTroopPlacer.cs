using System;
using NavalDLC.Missions.MissionLogics;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews.Order;

namespace NavalDLC.View.MissionViews.Order
{
	// Token: 0x0200002E RID: 46
	public class NavalOrderTroopPlacer : OrderTroopPlacer
	{
		// Token: 0x06000129 RID: 297 RVA: 0x00008DFA File Offset: 0x00006FFA
		public NavalOrderTroopPlacer(OrderController orderController)
			: base(orderController)
		{
		}

		// Token: 0x0600012A RID: 298 RVA: 0x00008E03 File Offset: 0x00007003
		public override void AfterStart()
		{
			base.AfterStart();
			base.OrderFlag.IsVisible = false;
			this._navalShipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
		}

		// Token: 0x0600012B RID: 299 RVA: 0x00008E28 File Offset: 0x00007028
		protected override bool CanUpdate()
		{
			bool flag = base.OrderController == Mission.Current.PlayerEnemyTeam.MasterOrderController;
			bool flag2 = base.Mission.IsNavalRaidBattle && base.OrderController.Team.Side == 0;
			if (flag || flag2)
			{
				return base.CanUpdate();
			}
			if (base.CanUpdate())
			{
				NavalShipsLogic navalShipsLogic = this._navalShipsLogic;
				return navalShipsLogic != null && navalShipsLogic.GetNumTeamShips(0) > 0;
			}
			return false;
		}

		// Token: 0x0600012C RID: 300 RVA: 0x00008E9A File Offset: 0x0000709A
		protected override OrderFlag CreateOrderFlag()
		{
			return new NavalOrderFlag(base.Mission, base.MissionScreen, 20f);
		}

		// Token: 0x0600012D RID: 301 RVA: 0x00008EB2 File Offset: 0x000070B2
		protected override OrderTroopPlacer.CursorState GetCursorState()
		{
			if (base.Mission.IsNavalBattle)
			{
				return base.GetGroundOrNormalCursor();
			}
			return base.GetCursorState();
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00008ED0 File Offset: 0x000070D0
		protected override bool TryGetScreenMiddleToWorldPosition(out WorldPosition worldPosition, out float collisionDistance, out WeakGameEntity collidedEntity)
		{
			if (!base.Mission.IsNavalBattle)
			{
				return base.TryGetScreenMiddleToWorldPosition(ref worldPosition, ref collisionDistance, ref collidedEntity);
			}
			Vec3 vec;
			if (base.MissionScreen.GetProjectedMousePositionOnWater(ref vec))
			{
				worldPosition = new WorldPosition(base.Mission.Scene, vec);
				collisionDistance = (vec - base.Mission.GetCameraFrame().origin).Length;
				collidedEntity = WeakGameEntity.Invalid;
				return true;
			}
			worldPosition = WorldPosition.Invalid;
			collisionDistance = 0f;
			collidedEntity = WeakGameEntity.Invalid;
			return false;
		}

		// Token: 0x0600012F RID: 303 RVA: 0x00008F68 File Offset: 0x00007168
		protected override Vec3 GetGroundedVec3(WorldPosition worldPosition)
		{
			if (base.Mission.IsNavalBattle)
			{
				Vec2 asVec = worldPosition.AsVec2;
				return new Vec3(asVec.X, asVec.Y, base.Mission.Scene.GetWaterLevelAtPosition(asVec, true, true), -1f);
			}
			return base.GetGroundedVec3(worldPosition);
		}

		// Token: 0x0400007B RID: 123
		private NavalShipsLogic _navalShipsLogic;
	}
}
