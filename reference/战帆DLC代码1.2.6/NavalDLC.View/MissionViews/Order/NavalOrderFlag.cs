using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews.Order;
using TaleWorlds.MountAndBlade.View.Screens;

namespace NavalDLC.View.MissionViews.Order
{
	// Token: 0x0200002D RID: 45
	public class NavalOrderFlag : OrderFlag
	{
		// Token: 0x06000126 RID: 294 RVA: 0x00008CD6 File Offset: 0x00006ED6
		public NavalOrderFlag(Mission mission, MissionScreen missionScreen, float flagScale = 20f)
			: base(mission, missionScreen, flagScale)
		{
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00008CE4 File Offset: 0x00006EE4
		protected override Vec3 GetFlagPosition(out bool isOnValidGround, bool checkForTargetEntity, Vec3 targetCollisionPoint)
		{
			if (!this._mission.IsNavalBattle)
			{
				return base.GetFlagPosition(ref isOnValidGround, checkForTargetEntity, targetCollisionPoint);
			}
			Vec3 vec;
			if (this._missionScreen.GetProjectedMousePositionOnWater(ref vec))
			{
				vec..ctor(vec.x, vec.y, this._mission.Scene.GetWaterLevelAtPosition(vec.AsVec2, true, true), -1f);
				WorldPosition worldPosition;
				worldPosition..ctor(Mission.Current.Scene, UIntPtr.Zero, vec, false);
				isOnValidGround = this.IsPositionOnValidGround(worldPosition);
				return vec;
			}
			isOnValidGround = false;
			return new Vec3(0f, 0f, -10000f, -1f);
		}

		// Token: 0x06000128 RID: 296 RVA: 0x00008D88 File Offset: 0x00006F88
		public override bool IsPositionOnValidGround(WorldPosition worldPosition)
		{
			if (!this._mission.IsNavalBattle)
			{
				return base.IsPositionOnValidGround(worldPosition);
			}
			if (Mission.Current.Mode == 6 && Mission.Current.DeploymentPlan.HasDeploymentBoundaries(Mission.Current.PlayerTeam))
			{
				IMissionDeploymentPlan deploymentPlan = Mission.Current.DeploymentPlan;
				Team playerTeam = Mission.Current.PlayerTeam;
				Vec2 asVec = worldPosition.AsVec2;
				if (!deploymentPlan.IsPositionInsideDeploymentBoundaries(playerTeam, ref asVec))
				{
					return false;
				}
			}
			return true;
		}
	}
}
