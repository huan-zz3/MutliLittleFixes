using System;
using System.Collections.Generic;
using NavalDLC.Missions.NavalPhysics;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;

namespace NavalDLC.View.MissionViews
{
	// Token: 0x0200001E RID: 30
	public class NavalMissionDeploymentBoundaryMarker : MissionDeploymentBoundaryMarker
	{
		// Token: 0x060000CA RID: 202 RVA: 0x00006E01 File Offset: 0x00005001
		public NavalMissionDeploymentBoundaryMarker(string smallPrefabName, string largePrefabName, float markerInterval = 20f)
			: base(smallPrefabName, markerInterval)
		{
			this._largePrefabName = largePrefabName;
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00006E14 File Offset: 0x00005014
		protected override void MarkLine(Vec3 startPoint, Vec3 endPoint, List<GameEntity> boundary, Banner banner = null)
		{
			Vec3 vec = endPoint - startPoint;
			float length = vec.Length;
			Vec3 vec2 = vec;
			vec2.Normalize();
			vec2 *= this.MarkerInterval;
			for (float num = 0f; num < length; num += this.MarkerInterval)
			{
				GameEntity gameEntity = this.CreateBoundaryEntity((int)(num / this.MarkerInterval) % 4 == 0);
				NavalPhysics firstScriptOfType = gameEntity.GetFirstScriptOfType<NavalPhysics>();
				MatrixFrame identity = MatrixFrame.Identity;
				identity.rotation.RotateAboutUp(vec.RotationZ + 3.1415927f);
				identity.origin = startPoint;
				identity.origin.z = gameEntity.GetWaterLevelAtPosition(identity.origin.AsVec2, true, false) - ((firstScriptOfType != null) ? firstScriptOfType.StabilitySubmergedHeightOfShip : 0f);
				gameEntity.SetFrame(ref identity, true);
				if (firstScriptOfType != null)
				{
					firstScriptOfType.SetAnchor(true, true, 1f);
				}
				boundary.Add(gameEntity);
				startPoint += vec2;
			}
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00006F0C File Offset: 0x0000510C
		private GameEntity CreateBoundaryEntity(bool isLarge)
		{
			Scene scene = Mission.Current.Scene;
			if (isLarge && this._cachedLargeEntity == null)
			{
				this._cachedLargeEntity = GameEntity.Instantiate(null, this._largePrefabName, false, true, "");
			}
			else if (!isLarge && this._cachedEntity == null)
			{
				this._cachedEntity = GameEntity.Instantiate(null, this._prefabName, false, true, "");
			}
			GameEntity gameEntity = GameEntity.CopyFrom(scene, isLarge ? this._cachedLargeEntity : this._cachedEntity, true, true);
			gameEntity.SetMobility(1);
			return gameEntity;
		}

		// Token: 0x0400004A RID: 74
		private readonly string _largePrefabName;

		// Token: 0x0400004B RID: 75
		private GameEntity _cachedLargeEntity;
	}
}
