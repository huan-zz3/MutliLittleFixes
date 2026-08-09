using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using NavalDLC.Missions.Objects;
using NavalDLC.View.Map.Visuals;
using SandBox.View.Map;
using SandBox.View.Map.Managers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Objects;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.Tableaus.Thumbnails;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.View
{
	// Token: 0x02000006 RID: 6
	public class NavalDLCViewHelpers
	{
		// Token: 0x02000042 RID: 66
		public static class ShipVisualHelper
		{
			// Token: 0x060001E2 RID: 482 RVA: 0x0000E184 File Offset: 0x0000C384
			public static GameEntity GetFlagshipEntity(PartyBase party, Scene scene)
			{
				if (party.Ships.Count > 0)
				{
					Ship flagShip = party.FlagShip;
					return NavalDLCViewHelpers.ShipVisualHelper.GetShipEntityForCampaign(flagShip, scene, flagShip.GetShipVisualSlotInfos());
				}
				float num = 0.4f;
				MatrixFrame identity = MatrixFrame.Identity;
				GameEntity gameEntity = GameEntity.CreateEmpty(scene, true, true, true);
				gameEntity.AddMultiMesh(MetaMesh.GetCopy("boat_sail_on", true, false), true);
				identity.rotation.ApplyScaleLocal(num);
				gameEntity.SetFrame(ref identity, true);
				return gameEntity;
			}

			// Token: 0x060001E3 RID: 483 RVA: 0x0000E1F4 File Offset: 0x0000C3F4
			public static GameEntity GetShipEntity(Ship ship, Scene scene, List<ShipVisualSlotInfo> selectedPieces, bool createPhysics = false)
			{
				MissionShipObject @object = MBObjectManager.Instance.GetObject<MissionShipObject>(ship.ShipHull.MissionShipObjectId);
				int randomValue = ship.RandomValue;
				float mapVisualScale = ship.ShipHull.MapVisualScale;
				string text = ((@object != null) ? @object.Prefab : null);
				ValueTuple<uint, uint> sailColors = ShipHelper.GetSailColors(ship, null);
				uint item = sailColors.Item1;
				uint item2 = sailColors.Item2;
				GameEntity gameEntity = VisualShipFactory.CreateVisualShip(text, scene, selectedPieces, randomValue, ship.HitPoints / ship.MaxHitPoints, item, item2, createPhysics);
				ShipVisual firstScriptOfType = gameEntity.GetFirstScriptOfType<ShipVisual>();
				if (firstScriptOfType != null)
				{
					using (List<ScriptComponentBehavior>.Enumerator enumerator = firstScriptOfType.SailVisuals.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							SailVisual sailVisual;
							if ((sailVisual = enumerator.Current as SailVisual) != null && sailVisual.SailTopBannerEntity != null && sailVisual.SailTopBannerEntity.HasTag("banner_with_faction_color"))
							{
								NavalDLCViewHelpers.ShipVisualHelper.SetBanner(sailVisual.SailTopBannerEntity, ShipHelper.GetShipBanner(ship, null), false);
							}
						}
					}
				}
				if (gameEntity != null)
				{
					GameEntityPhysicsExtensions.SetPhysicsState(gameEntity, false, true);
				}
				gameEntity.SetBodyFlags(144);
				MatrixFrame identity = MatrixFrame.Identity;
				identity.rotation.ApplyScaleLocal(mapVisualScale);
				gameEntity.SetFrame(ref identity, true);
				return gameEntity;
			}

			// Token: 0x060001E4 RID: 484 RVA: 0x0000E32C File Offset: 0x0000C52C
			public static GameEntity GetShipEntityForCampaign(Ship ship, Scene scene, List<ShipVisualSlotInfo> selectedPieces)
			{
				MissionShipObject @object = MBObjectManager.Instance.GetObject<MissionShipObject>(ship.ShipHull.MissionShipObjectId);
				int randomValue = ship.RandomValue;
				string customSailPatternId = ship.CustomSailPatternId;
				float mapVisualScale = ship.ShipHull.MapVisualScale;
				string text = ((@object != null) ? @object.Prefab : null);
				ValueTuple<uint, uint> sailColors = ShipHelper.GetSailColors(ship, null);
				uint item = sailColors.Item1;
				uint item2 = sailColors.Item2;
				GameEntity gameEntity = VisualShipFactory.CreateVisualShipForCampaign(text, scene, selectedPieces, randomValue, customSailPatternId, item, item2);
				ShipVisual firstScriptOfType = gameEntity.GetFirstScriptOfType<ShipVisual>();
				if (firstScriptOfType != null)
				{
					using (List<ScriptComponentBehavior>.Enumerator enumerator = firstScriptOfType.SailVisuals.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							SailVisual sailVisual;
							if ((sailVisual = enumerator.Current as SailVisual) != null && sailVisual.SailTopBannerEntity != null && sailVisual.SailTopBannerEntity.HasTag("banner_with_faction_color"))
							{
								NavalDLCViewHelpers.ShipVisualHelper.SetBanner(sailVisual.SailTopBannerEntity, ShipHelper.GetShipBanner(ship, null), false);
							}
						}
					}
				}
				if (gameEntity != null)
				{
					GameEntityPhysicsExtensions.SetPhysicsState(gameEntity, false, true);
				}
				gameEntity.SetBodyFlags(144);
				MatrixFrame identity = MatrixFrame.Identity;
				identity.rotation.ApplyScaleLocal(mapVisualScale);
				gameEntity.SetFrame(ref identity, true);
				return gameEntity;
			}

			// Token: 0x060001E5 RID: 485 RVA: 0x0000E460 File Offset: 0x0000C660
			public static void CollectSailVisuals(WeakGameEntity shipEntity, List<SailVisual> sailVisuals)
			{
				sailVisuals.Clear();
				ShipVisual firstScriptOfType = shipEntity.GetFirstScriptOfType<ShipVisual>();
				if (firstScriptOfType != null)
				{
					using (List<ScriptComponentBehavior>.Enumerator enumerator = firstScriptOfType.SailVisuals.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							SailVisual sailVisual;
							if ((sailVisual = enumerator.Current as SailVisual) != null)
							{
								sailVisual.SailEnabled = false;
								sailVisual.SetFoldSailStepMultiplier(0.3f);
								sailVisual.SetFoldSailDuration(0.4f);
								sailVisual.SetUnfoldSailDuration(0.2f);
								sailVisual.FoldAnimationEnabled = false;
								sailVisuals.Add(sailVisual);
							}
						}
					}
				}
			}

			// Token: 0x060001E6 RID: 486 RVA: 0x0000E4FC File Offset: 0x0000C6FC
			public static void FoldSails(List<SailVisual> sailVisuals)
			{
				foreach (SailVisual sailVisual in sailVisuals)
				{
					sailVisual.SailEnabled = false;
				}
			}

			// Token: 0x060001E7 RID: 487 RVA: 0x0000E548 File Offset: 0x0000C748
			public static void UnfoldSails(List<SailVisual> sailVisuals)
			{
				foreach (SailVisual sailVisual in sailVisuals)
				{
					sailVisual.SailEnabled = true;
				}
			}

			// Token: 0x060001E8 RID: 488 RVA: 0x0000E594 File Offset: 0x0000C794
			public static void RefreshShipVisuals(WeakGameEntity shipEntity, Ship ship, List<SailVisual> sailVisuals)
			{
				VisualShipFactory.RefreshUpgrades(shipEntity, ship.GetShipVisualSlotInfos());
				ValueTuple<uint, uint> sailColors = ShipHelper.GetSailColors(ship, null);
				foreach (SailVisual sailVisual in sailVisuals)
				{
					sailVisual.ShipVisual.SailColors = sailColors;
					sailVisual.ShipVisual.Health = ship.HitPoints / ship.MaxHitPoints;
					sailVisual.RefreshSailVisual();
				}
				NavalDLCViewHelpers.ShipVisualHelper.UpdateBanner(ShipHelper.GetShipBanner(ship, null), sailVisuals);
				foreach (Mesh mesh in shipEntity.GetAllMeshesWithTag("faction_color"))
				{
					mesh.Color = sailColors.Item1;
					mesh.Color2 = sailColors.Item2;
				}
			}

			// Token: 0x060001E9 RID: 489 RVA: 0x0000E678 File Offset: 0x0000C878
			public static void RefreshShipVisuals(GameEntity shipEntity, List<ShipVisualSlotInfo> selectedPieces, uint sailColor1, uint sailColor2, Banner banner, float healthPercent)
			{
				VisualShipFactory.RefreshUpgrades(shipEntity.WeakEntity, selectedPieces);
				ShipVisual firstScriptOfType = shipEntity.GetFirstScriptOfType<ShipVisual>();
				if (firstScriptOfType != null)
				{
					firstScriptOfType.SailColors = new ValueTuple<uint, uint>(sailColor1, sailColor2);
					firstScriptOfType.Health = healthPercent;
					using (List<ScriptComponentBehavior>.Enumerator enumerator = firstScriptOfType.SailVisuals.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							SailVisual sailVisual;
							if ((sailVisual = enumerator.Current as SailVisual) != null)
							{
								if (sailVisual.SailTopBannerEntity != null && sailVisual.SailTopBannerEntity.HasTag("banner_with_faction_color"))
								{
									NavalDLCViewHelpers.ShipVisualHelper.SetBanner(sailVisual.SailTopBannerEntity, banner, false);
								}
								sailVisual.RefreshSailVisual();
							}
						}
					}
				}
				foreach (Mesh mesh in shipEntity.WeakEntity.GetAllMeshesWithTag("faction_color"))
				{
					mesh.Color = sailColor1;
					mesh.Color2 = sailColor2;
				}
			}

			// Token: 0x060001EA RID: 490 RVA: 0x0000E784 File Offset: 0x0000C984
			private static void UpdateBanner(Banner banner, List<SailVisual> sailVisuals)
			{
				foreach (SailVisual sailVisual in sailVisuals)
				{
					if (sailVisual.SailTopBannerEntity != null && sailVisual.SailTopBannerEntity.HasTag("banner_with_faction_color"))
					{
						NavalDLCViewHelpers.ShipVisualHelper.SetBanner(sailVisual.SailTopBannerEntity, banner, true);
					}
				}
			}

			// Token: 0x060001EB RID: 491 RVA: 0x0000E7F8 File Offset: 0x0000C9F8
			private static void SetBanner(GameEntity bannerEntity, Banner banner, bool isUpdated = false)
			{
				NavalDLCViewHelpers.ShipVisualHelper.<>c__DisplayClass11_0 CS$<>8__locals1 = new NavalDLCViewHelpers.ShipVisualHelper.<>c__DisplayClass11_0();
				CS$<>8__locals1.bannerEntity = bannerEntity;
				CS$<>8__locals1.isUpdated = isUpdated;
				BannerDebugInfo bannerDebugInfo = BannerDebugInfo.CreateManual("SetBanner");
				BannerVisualExtensions.GetTableauTextureLarge(banner, ref bannerDebugInfo, new Action<Texture>(CS$<>8__locals1.<SetBanner>g__onTextureRendered|0));
			}

			// Token: 0x040000EB RID: 235
			private const string BannerTag = "banner_with_faction_color";

			// Token: 0x040000EC RID: 236
			private const float AnimationSpeedMultiplier = 0.1f;
		}

		// Token: 0x02000043 RID: 67
		public static class BannerVisualHelper
		{
			// Token: 0x060001EC RID: 492 RVA: 0x0000E83C File Offset: 0x0000CA3C
			public static MetaMesh GetBannerOfCharacter(Banner banner, string bannerMeshName)
			{
				MetaMesh copy = MetaMesh.GetCopy(bannerMeshName, true, false);
				for (int i = 0; i < copy.MeshCount; i++)
				{
					Mesh meshAtIndex = copy.GetMeshAtIndex(i);
					if (!meshAtIndex.HasTag("dont_use_tableau"))
					{
						Material material = meshAtIndex.GetMaterial();
						Material tableauMaterial = null;
						Tuple<Material, Banner> tuple = new Tuple<Material, Banner>(material, banner);
						if (MapScreen.Instance.CharacterBannerMaterialCache.ContainsKey(tuple))
						{
							tableauMaterial = MapScreen.Instance.CharacterBannerMaterialCache[tuple];
						}
						else
						{
							tableauMaterial = material.CreateCopy();
							Action<Texture> action = delegate(Texture tex)
							{
								tableauMaterial.SetTexture(1, tex);
								uint num = (uint)tableauMaterial.GetShader().GetMaterialShaderFlagMask("use_tableau_blending", true);
								ulong shaderFlags = tableauMaterial.GetShaderFlags();
								tableauMaterial.SetShaderFlags(shaderFlags | (ulong)num);
							};
							BannerDebugInfo bannerDebugInfo = BannerDebugInfo.CreateManual("GetBannerOfCharacter");
							BannerVisualExtensions.GetTableauTextureLarge(banner, ref bannerDebugInfo, action);
							MapScreen.Instance.CharacterBannerMaterialCache[tuple] = tableauMaterial;
						}
						meshAtIndex.SetMaterial(tableauMaterial);
					}
				}
				return copy;
			}
		}

		// Token: 0x02000044 RID: 68
		public static class BlockadeVisualHelper
		{
			// Token: 0x060001ED RID: 493 RVA: 0x0000E924 File Offset: 0x0000CB24
			public static List<Vec3> GetPositionsOnBlockadeArc(Settlement settlement, int numberOfArcs, int numberOfPositions, float angle, float distanceBetweenArcs)
			{
				CampaignVec2 portPosition = settlement.PortPosition;
				Vec2 vec = settlement.PortPosition.ToVec2() - settlement.Position.ToVec2();
				List<Vec3> list = new List<Vec3>();
				Vec2 vec2 = vec.Normalized();
				vec2.RotateCCW(-angle / 2f);
				Vec2 vec3 = vec2;
				int num = 1;
				while (numberOfArcs >= num && numberOfPositions > 0)
				{
					int num2 = MathF.Min(num, numberOfPositions);
					for (int i = 0; i < num2; i++)
					{
						Vec3 vec4 = ((num == 1) ? portPosition : (portPosition + vec3 * (float)(num - 1) * distanceBetweenArcs)).AsVec3();
						vec3.RotateCCW(angle / (float)MathF.Max(1, num2 - 1));
						list.Add(vec4);
					}
					vec3 = vec2;
					numberOfPositions -= num;
					num++;
				}
				return list;
			}

			// Token: 0x060001EE RID: 494 RVA: 0x0000EA00 File Offset: 0x0000CC00
			public static void AddBlockadeVisuals(Dictionary<Ship, NavalMobilePartyVisual.BlockadeShipVisual> shipToBlockadeShipVisualCache, PartyBase party, GameEntity strategicEntity)
			{
				int num = 0;
				int num2 = 0;
				SiegeEvent siegeEvent = party.MobileParty.SiegeEvent;
				Settlement besiegedSettlement = siegeEvent.BesiegedSettlement;
				BlockadePositionScript firstScriptOfType = SettlementVisualManager.Current.GetSettlementVisual(besiegedSettlement).StrategicEntity.GetFirstScriptOfType<BlockadePositionScript>();
				IEnumerable<PartyBase> involvedPartiesForEventType = siegeEvent.BesiegerCamp.GetInvolvedPartiesForEventType(5);
				MobileParty leaderParty = siegeEvent.BesiegerCamp.LeaderParty;
				if (firstScriptOfType != null)
				{
					if (!Extensions.IsEmpty<KeyValuePair<Ship, NavalMobilePartyVisual.BlockadeShipVisual>>(shipToBlockadeShipVisualCache))
					{
						foreach (KeyValuePair<Ship, NavalMobilePartyVisual.BlockadeShipVisual> keyValuePair in shipToBlockadeShipVisualCache)
						{
							keyValuePair.Value.ShipEntity.SetVisibilityExcludeParents(false);
						}
					}
					Vec3 vec;
					List<List<Vec3>> blockadeArc = firstScriptOfType.GetBlockadeArc(involvedPartiesForEventType.Sum<PartyBase>((PartyBase p) => p.Ships.Count), ref vec);
					int num3 = ((leaderParty.Ships.Count > 0) ? (blockadeArc[0].Count / 2) : (-1));
					foreach (PartyBase partyBase in involvedPartiesForEventType)
					{
						if (num == blockadeArc.Count)
						{
							break;
						}
						if (!Extensions.IsEmpty<Ship>(partyBase.Ships))
						{
							Ship flagShip = partyBase.FlagShip;
							if (leaderParty.Party == partyBase)
							{
								if (partyBase == party)
								{
									NavalMobilePartyVisual.BlockadeShipVisual blockadeShipVisual;
									if (!shipToBlockadeShipVisualCache.TryGetValue(flagShip, out blockadeShipVisual))
									{
										blockadeShipVisual = NavalDLCViewHelpers.BlockadeVisualHelper.CreateBlockadeShipVisual(NavalDLCViewHelpers.ShipVisualHelper.GetFlagshipEntity(partyBase, strategicEntity.Scene));
										shipToBlockadeShipVisualCache[flagShip] = blockadeShipVisual;
									}
									NavalDLCViewHelpers.BlockadeVisualHelper.InitializeBlockadeVisual(blockadeArc[0][num3], blockadeShipVisual.ShipEntity, vec);
								}
							}
							else
							{
								if (num2 == num3 && num == 0)
								{
									num2++;
								}
								if (num2 < blockadeArc[num].Count && partyBase == party)
								{
									NavalMobilePartyVisual.BlockadeShipVisual blockadeShipVisual2;
									if (!shipToBlockadeShipVisualCache.TryGetValue(flagShip, out blockadeShipVisual2))
									{
										blockadeShipVisual2 = NavalDLCViewHelpers.BlockadeVisualHelper.CreateBlockadeShipVisual(NavalDLCViewHelpers.ShipVisualHelper.GetFlagshipEntity(partyBase, strategicEntity.Scene));
										shipToBlockadeShipVisualCache[flagShip] = blockadeShipVisual2;
									}
									NavalDLCViewHelpers.BlockadeVisualHelper.InitializeBlockadeVisual(blockadeArc[num][num2], blockadeShipVisual2.ShipEntity, vec);
								}
								num2++;
							}
							if (num2 >= blockadeArc[num].Count)
							{
								num++;
								num2 = 0;
							}
						}
					}
					if (num < blockadeArc.Count)
					{
						foreach (PartyBase partyBase2 in involvedPartiesForEventType)
						{
							if (num == blockadeArc.Count)
							{
								break;
							}
							if (partyBase2.Ships.Count<Ship>() > 1)
							{
								List<Ship> list;
								if (partyBase2 != party)
								{
									list = partyBase2.Ships;
								}
								else
								{
									list = Extensions.ToMBList<Ship>(partyBase2.Ships.OrderByDescending<Ship, float>((Ship x) => x.FlagshipScore));
								}
								foreach (Ship ship in list)
								{
									if (num == blockadeArc.Count)
									{
										break;
									}
									if (ship != partyBase2.FlagShip)
									{
										if (num2 == num3 && num == 0)
										{
											num2++;
										}
										if (partyBase2 == party)
										{
											NavalMobilePartyVisual.BlockadeShipVisual blockadeShipVisual3;
											if (!shipToBlockadeShipVisualCache.TryGetValue(ship, out blockadeShipVisual3))
											{
												blockadeShipVisual3 = NavalDLCViewHelpers.BlockadeVisualHelper.CreateBlockadeShipVisual(NavalDLCViewHelpers.ShipVisualHelper.GetShipEntityForCampaign(ship, strategicEntity.Scene, ship.GetShipVisualSlotInfos()));
												shipToBlockadeShipVisualCache[ship] = blockadeShipVisual3;
											}
											NavalDLCViewHelpers.BlockadeVisualHelper.InitializeBlockadeVisual(blockadeArc[num][num2], blockadeShipVisual3.ShipEntity, vec);
										}
										num2++;
										if (num2 >= blockadeArc[num].Count)
										{
											num++;
											num2 = 0;
										}
									}
								}
							}
						}
					}
				}
			}

			// Token: 0x060001EF RID: 495 RVA: 0x0000EDF4 File Offset: 0x0000CFF4
			private static NavalMobilePartyVisual.BlockadeShipVisual CreateBlockadeShipVisual(GameEntity shipEntity)
			{
				return new NavalMobilePartyVisual.BlockadeShipVisual
				{
					ShipEntity = shipEntity,
					RockingPhase = MBRandom.RandomFloatRanged(-3.1415927f, 3.1415927f)
				};
			}

			// Token: 0x060001F0 RID: 496 RVA: 0x0000EE28 File Offset: 0x0000D028
			private static void InitializeBlockadeVisual(Vec3 position, GameEntity shipEntity, Vec3 centerOfArc)
			{
				Vec2 asVec = position.AsVec2;
				Vec2 vec = asVec - centerOfArc.AsVec2;
				MatrixFrame frame = shipEntity.GetFrame();
				CampaignVec2 campaignVec;
				campaignVec..ctor(asVec, false);
				position.z = campaignVec.AsVec3().Z;
				frame.origin = position;
				float num = vec.AngleBetween(frame.rotation.f.AsVec2);
				frame.Rotate(1.5707964f - num, ref Vec3.Up);
				shipEntity.SetFrame(ref frame, true);
				shipEntity.SetVisibilityExcludeParents(true);
				ShipVisual firstScriptOfType = shipEntity.GetFirstScriptOfType<ShipVisual>();
				if (firstScriptOfType != null)
				{
					using (List<ScriptComponentBehavior>.Enumerator enumerator = firstScriptOfType.SailVisuals.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							SailVisual sailVisual;
							if ((sailVisual = enumerator.Current as SailVisual) != null)
							{
								sailVisual.SailEnabled = false;
								sailVisual.SetFoldSailStepMultiplier(0.3f);
								sailVisual.SetFoldSailDuration(0.4f);
								sailVisual.SetUnfoldSailDuration(0.2f);
								sailVisual.FoldAnimationEnabled = false;
							}
						}
					}
				}
			}

			// Token: 0x040000ED RID: 237
			private const float AnimationSpeedMultiplier = 0.1f;
		}
	}
}
