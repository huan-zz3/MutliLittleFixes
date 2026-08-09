using System;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.MissionLogics
{
	// Token: 0x020000CD RID: 205
	public class NavalCustomBattleWindAndWaveLogic : MissionLogic
	{
		// Token: 0x06000F6A RID: 3946 RVA: 0x000763F4 File Offset: 0x000745F4
		public NavalCustomBattleWindAndWaveLogic(NavalCustomBattleWindConfig.Direction windDirection, TerrainType terrainType)
		{
			this._windDirection = windDirection;
			this._terrainType = terrainType;
		}

		// Token: 0x06000F6B RID: 3947 RVA: 0x0007640A File Offset: 0x0007460A
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._deploymentMissionController = base.Mission.GetMissionBehavior<DeploymentMissionController>();
			this._deploymentMissionController.OnAfterSetupTeams += this.OnAfterSetupTeams;
		}

		// Token: 0x06000F6C RID: 3948 RVA: 0x0007643A File Offset: 0x0007463A
		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
			this._deploymentMissionController.OnAfterSetupTeams -= this.OnAfterSetupTeams;
		}

		// Token: 0x06000F6D RID: 3949 RVA: 0x00076459 File Offset: 0x00074659
		public override void AfterStart()
		{
		}

		// Token: 0x06000F6E RID: 3950 RVA: 0x0007645B File Offset: 0x0007465B
		public void OnAfterSetupTeams()
		{
			this.UpdateSceneWindDirection();
			this.UpdateSceneWaterStrength();
		}

		// Token: 0x06000F6F RID: 3951 RVA: 0x0007646C File Offset: 0x0007466C
		private void UpdateSceneWindDirection()
		{
			Vec2 vec = Vec2.Zero;
			Vec2 vec2 = Vec2.Zero;
			int num = 0;
			int num2 = 0;
			foreach (Team team in Mission.Current.Teams)
			{
				if (team.Side == 1)
				{
					Vec2 vec3 = vec;
					MatrixFrame matrixFrame = base.Mission.DeploymentPlan.GetDeploymentFrame(team);
					vec = vec3 + matrixFrame.origin.AsVec2;
					num++;
				}
				else if (team.Side == null)
				{
					Vec2 vec4 = vec2;
					MatrixFrame matrixFrame = base.Mission.DeploymentPlan.GetDeploymentFrame(team);
					vec2 = vec4 + matrixFrame.origin.AsVec2;
					num2++;
				}
			}
			vec /= (float)num;
			vec2 /= (float)num2;
			Vec2 vec5 = (vec2 - vec).Normalized();
			float length = Mission.Current.Scene.GetGlobalWindVelocity().Length;
			Vec2 vec6 = length * vec5;
			switch (this._windDirection)
			{
			case NavalCustomBattleWindConfig.Direction.TowardsDefender:
				vec6.RotateCCW(-0.5235988f);
				break;
			case NavalCustomBattleWindConfig.Direction.TowardsAttacker:
				vec6 *= -1f;
				vec6.RotateCCW(-0.5235988f);
				break;
			case NavalCustomBattleWindConfig.Direction.Side:
				vec6 = Vec3.CrossProduct(Vec3.Up, vec5.ToVec3(0f)).AsVec2 * length;
				break;
			case NavalCustomBattleWindConfig.Direction.Random:
				vec6 = length * new Vec2(MBRandom.RandomFloatNormal, MBRandom.RandomFloatNormal).Normalized();
				break;
			}
			Mission.Current.Scene.SetGlobalWindVelocity(ref vec6);
		}

		// Token: 0x06000F70 RID: 3952 RVA: 0x0007662C File Offset: 0x0007482C
		private void UpdateSceneWaterStrength()
		{
			if (this._terrainType == 11)
			{
				Mission.Current.Scene.SetWaterStrength(0.5f);
			}
		}

		// Token: 0x04000952 RID: 2386
		private NavalCustomBattleWindConfig.Direction _windDirection;

		// Token: 0x04000953 RID: 2387
		private TerrainType _terrainType;

		// Token: 0x04000954 RID: 2388
		private DeploymentMissionController _deploymentMissionController;
	}
}
