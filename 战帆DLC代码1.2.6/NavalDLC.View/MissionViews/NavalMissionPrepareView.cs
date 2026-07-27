using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.ShipActuators;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.Tableaus.Thumbnails;

namespace NavalDLC.View.MissionViews
{
	// Token: 0x02000020 RID: 32
	public class NavalMissionPrepareView : MissionView
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x060000CE RID: 206 RVA: 0x00006FA0 File Offset: 0x000051A0
		private string BannerTag
		{
			get
			{
				return "banner_with_faction_color";
			}
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00006FA8 File Offset: 0x000051A8
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._navalShipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
			this._navalShipsLogic.ShipSpawnedEvent += this.OnShipSpawned;
			this._navalShipsLogic.ShipCapturedEvent += this.StartBannerChangeAnimationForShip;
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00006FFC File Offset: 0x000051FC
		public void OnShipSpawned(MissionShip missionShip)
		{
			foreach (GameEntity gameEntity in missionShip.BannerEntities)
			{
				this.SetOwnerBanner(gameEntity, missionShip.Banner);
			}
			foreach (GameEntity gameEntity2 in missionShip.SailMeshEntities)
			{
				ValueTuple<uint, uint> sailColors = missionShip.SailColors;
				uint item = sailColors.Item1;
				uint item2 = sailColors.Item2;
				this.SetSailColors(gameEntity2, item, item2);
			}
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x000070B0 File Offset: 0x000052B0
		private void SetSailColors(GameEntity sailEntity, uint sailColor1, uint sailColor2)
		{
			if (sailEntity.Skeleton != null)
			{
				foreach (Mesh mesh in sailEntity.Skeleton.GetAllMeshes())
				{
					if (mesh.HasTag("faction_color"))
					{
						mesh.Color = sailColor1;
						mesh.Color2 = sailColor2;
					}
				}
			}
			foreach (Mesh mesh2 in sailEntity.WeakEntity.GetAllMeshesWithTag("faction_color"))
			{
				mesh2.Color = sailColor1;
				mesh2.Color2 = sailColor2;
			}
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00007174 File Offset: 0x00005374
		private void SetOwnerBanner(GameEntity bannerEntity, Banner ownerBanner)
		{
			BannerDebugInfo bannerDebugInfo = BannerDebugInfo.CreateManual(base.GetType().Name);
			BannerVisualExtensions.GetTableauTextureLarge(ownerBanner, ref bannerDebugInfo, delegate(Texture tex)
			{
				this.OnTextureRendered(tex, bannerEntity);
			});
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x000071BC File Offset: 0x000053BC
		private void OnTextureRendered(Texture tex, GameEntity bannerEntity)
		{
			List<Mesh> list = bannerEntity.GetAllMeshesWithTag(this.BannerTag).ToList<Mesh>();
			if (Extensions.IsEmpty<Mesh>(list))
			{
				list.Add(bannerEntity.GetFirstMesh());
			}
			foreach (Mesh mesh in list)
			{
				if (mesh != null)
				{
					Material material = mesh.GetMaterial().CreateCopy();
					material.SetTexture(1, tex);
					uint num = (uint)material.GetShader().GetMaterialShaderFlagMask("use_tableau_blending", true);
					ulong shaderFlags = material.GetShaderFlags();
					material.SetShaderFlags(shaderFlags | (ulong)num);
					mesh.SetMaterial(material);
				}
			}
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x00007278 File Offset: 0x00005478
		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
			this._navalShipsLogic.ShipSpawnedEvent -= this.OnShipSpawned;
			this._navalShipsLogic.ShipCapturedEvent -= this.StartBannerChangeAnimationForShip;
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x000072B0 File Offset: 0x000054B0
		public void StartBannerChangeAnimationForShip(MissionShip ship, MissionShip ship2, Formation formation, Formation formation2)
		{
			Banner banner = ship.Banner;
			BannerDebugInfo bannerDebugInfo = BannerDebugInfo.CreateManual(base.GetType().Name);
			BannerVisualExtensions.GetTableauTextureLarge(banner, ref bannerDebugInfo, delegate(Texture tex)
			{
				this.OnCaptureBannerTextureRendered(tex, ship);
			});
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00007304 File Offset: 0x00005504
		private void OnCaptureBannerTextureRendered(Texture newTexture, MissionShip ship)
		{
			foreach (MissionSail missionSail in ship.Sails)
			{
				missionSail.StartShipCaptureAnimation(newTexture);
			}
		}

		// Token: 0x0400004C RID: 76
		private NavalShipsLogic _navalShipsLogic;
	}
}
