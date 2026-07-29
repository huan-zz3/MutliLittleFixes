using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MissionLibrary.Event;
using MissionSharedLibrary.Config;
using MissionSharedLibrary.QuerySystem;
using MissionSharedLibrary.Utilities;
using RTSCamera.CommandSystem.CampaignGame;
using RTSCamera.CommandSystem.Config;
using RTSCamera.CommandSystem.Config.HotKey;
using RTSCamera.CommandSystem.Logic;
using RTSCamera.CommandSystem.Logic.SubLogic;
using RTSCamera.CommandSystem.Orders;
using RTSCamera.CommandSystem.Utilities;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.MissionViews.Order;

namespace RTSCamera.CommandSystem.Patch
{
	// Token: 0x02000066 RID: 102
	public class Patch_OrderTroopPlacer
	{
		// Token: 0x060003C9 RID: 969 RVA: 0x00015F84 File Offset: 0x00014184
		public static bool Patch(Harmony harmony)
		{
			bool flag;
			try
			{
				if (Patch_OrderTroopPlacer._patched)
				{
					flag = false;
				}
				else
				{
					Patch_OrderTroopPlacer._patched = true;
					harmony.Patch(typeof(OrderTroopPlacer).GetMethod("OnMissionTick", BindingFlags.Instance | BindingFlags.Public), null, new HarmonyMethod(typeof(Patch_OrderTroopPlacer).GetMethod("Postfix_OnMissionTick", BindingFlags.Static | BindingFlags.Public)), null, null);
					harmony.Patch(typeof(OrderTroopPlacer).GetMethod("HandleMouseDown", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(Patch_OrderTroopPlacer).GetMethod("Prefix_HandleMouseDown", BindingFlags.Static | BindingFlags.Public)), null, null, null);
					harmony.Patch(typeof(OrderTroopPlacer).GetMethod("GetCursorState", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(Patch_OrderTroopPlacer).GetMethod("Prefix_GetCursorState", BindingFlags.Static | BindingFlags.Public)), null, null, null);
					harmony.Patch(typeof(OrderTroopPlacer).GetMethod("AddOrderPositionEntity", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(Patch_OrderTroopPlacer).GetMethod("Prefix_AddOrderPositionEntity", BindingFlags.Static | BindingFlags.Public)), null, null, null);
					harmony.Patch(typeof(OrderTroopPlacer).GetMethod("OnMissionScreenTick", BindingFlags.Instance | BindingFlags.Public), new HarmonyMethod(typeof(Patch_OrderTroopPlacer).GetMethod("Prefix_OnMissionScreenTick", BindingFlags.Static | BindingFlags.Public)), null, null, null);
					harmony.Patch(typeof(OrderTroopPlacer).GetMethod("UpdateFormationDrawingForMovementOrder", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(Patch_OrderTroopPlacer).GetMethod("Prefix_UpdateFormationDrawingForMovementOrder", BindingFlags.Static | BindingFlags.Public)), null, null, null);
					harmony.Patch(typeof(OrderTroopPlacer).GetMethod("UpdateFormationDrawingForFacingOrder", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(Patch_OrderTroopPlacer).GetMethod("Prefix_UpdateFormationDrawingForFacingOrder", BindingFlags.Static | BindingFlags.Public)), null, null, null);
					harmony.Patch(typeof(OrderTroopPlacer).GetMethod("HideOrderPositionEntities", BindingFlags.Instance | BindingFlags.NonPublic), new HarmonyMethod(typeof(Patch_OrderTroopPlacer).GetMethod("Prefix_HideOrderPositionEntities", BindingFlags.Static | BindingFlags.Public)), null, null, null);
					flag = true;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				MissionSharedLibrary.Utilities.Utility.DisplayMessage(ex.ToString());
				MBDebug.Print(ex.ToString(), 0, 12, 17592186044416UL);
				flag = false;
			}
			return flag;
		}

		// Token: 0x060003CA RID: 970 RVA: 0x000161CC File Offset: 0x000143CC
		public static void Postfix_OnMissionTick(OrderTroopPlacer __instance, bool ____initialized)
		{
			if (!____initialized)
			{
				return;
			}
			if (Patch_OrderTroopPlacer._isInitialized)
			{
				return;
			}
			Patch_OrderTroopPlacer._isInitialized = true;
			Patch_OrderTroopPlacer._orderTroopPlacer = __instance;
			Patch_OrderTroopPlacer._cachedCursorState = new UiQueryData<CurrentCursorState>(new Func<CurrentCursorState>(Patch_OrderTroopPlacer.GetCursorState), 0.05f);
			Patch_OrderTroopPlacer._clickedFormation = null;
			Patch_OrderTroopPlacer._outlineView = Mission.Current.GetMissionBehavior<CommandSystemLogic>().OutlineColorSubLogic;
			Patch_OrderTroopPlacer._groundMarkerView = Mission.Current.GetMissionBehavior<CommandSystemLogic>().GroundMarkerColorSubLogic;
			typeof(Input).GetProperty("DebugInput", BindingFlags.Static | BindingFlags.Public).SetValue(null, __instance.Input);
			Patch_OrderTroopPlacer._cachedTimeOfDay = __instance.Mission.Scene.TimeOfDay;
		}

		// Token: 0x060003CB RID: 971 RVA: 0x00016274 File Offset: 0x00014474
		public static void OnBehaviorInitialize()
		{
			Patch_OrderTroopPlacer._targetSelectionHandler = Mission.Current.GetMissionBehavior<MissionFormationTargetSelectionHandler>();
			if (Patch_OrderTroopPlacer._targetSelectionHandler != null)
			{
				Patch_OrderTroopPlacer._targetSelectionHandler.OnFormationFocused += Patch_OrderTroopPlacer.OnFormationFocused;
			}
			MissionEvent.ToggleFreeCamera += Patch_OrderTroopPlacer.OnToggleFreeCamera;
			Patch_OrderTroopPlacer.IsFreeCamera = false;
			Patch_OrderTroopPlacer._previousMovementTargetHightlightStyle = MovementTargetHighlightStyle.Count;
			Patch_OrderTroopPlacer._clearMouseOverFormationTimer = new Timer(-1f, -1f, false);
			Patch_OrderTroopPlacer._changeMouseOverFormationTimer = new Timer(-1f, -1f, false);
		}

		// Token: 0x060003CC RID: 972 RVA: 0x000162F4 File Offset: 0x000144F4
		public static void OnRemoveBehavior()
		{
			Patch_OrderTroopPlacer._cachedTimeOfDay = 0f;
			Patch_OrderTroopPlacer._orderTroopPlacer = null;
			Patch_OrderTroopPlacer._isInitialized = false;
			Patch_OrderTroopPlacer._outlineView = null;
			Patch_OrderTroopPlacer._groundMarkerView = null;
			Patch_OrderTroopPlacer._cachedCursorState = null;
			Patch_OrderTroopPlacer._clickedFormation = null;
			Patch_OrderTroopPlacer._lastNewMouseOverFormation = null;
			Patch_OrderTroopPlacer._focusedFormationsCache = null;
			if (Patch_OrderTroopPlacer._targetSelectionHandler != null)
			{
				Patch_OrderTroopPlacer._targetSelectionHandler.OnFormationFocused -= Patch_OrderTroopPlacer.OnFormationFocused;
			}
			Patch_OrderTroopPlacer._targetSelectionHandler = null;
			MissionEvent.ToggleFreeCamera -= Patch_OrderTroopPlacer.OnToggleFreeCamera;
			Patch_OrderTroopPlacer.IsFreeCamera = false;
			Patch_OrderTroopPlacer._originalOrderPositionEntities = (Patch_OrderTroopPlacer._newModelOrderPositionEntities = (Patch_OrderTroopPlacer._alwaysVisibleOrderPositionEntities = null));
			Patch_OrderTroopPlacer._currentMaterial = (Patch_OrderTroopPlacer._originalMaterial = (Patch_OrderTroopPlacer._newModelMaterial = (Patch_OrderTroopPlacer._alwaysVisibleMaterial = null)));
			Patch_OrderTroopPlacer._previousMovementTargetHightlightStyle = MovementTargetHighlightStyle.Count;
			Patch_OrderTroopPlacer._clearMouseOverFormationTimer = null;
			Patch_OrderTroopPlacer._changeMouseOverFormationTimer = null;
		}

		// Token: 0x060003CD RID: 973 RVA: 0x000163B1 File Offset: 0x000145B1
		private static void OnFormationFocused(MBReadOnlyList<Formation> focusedFormations)
		{
			Patch_OrderTroopPlacer._focusedFormationsCache = focusedFormations;
		}

		// Token: 0x060003CE RID: 974 RVA: 0x000163B9 File Offset: 0x000145B9
		private static void OnToggleFreeCamera(bool isFreeCamera)
		{
			Patch_OrderTroopPlacer.IsFreeCamera = isFreeCamera;
			RTSCommandVisualOrder.OrderToSelectTarget = SelectTargetMode.None;
		}

		// Token: 0x060003CF RID: 975 RVA: 0x000163C8 File Offset: 0x000145C8
		public static bool IsDraggingFormation(OrderTroopPlacer __instance, Vec2? ____formationDrawingStartingPointOfMouse, float? ____formationDrawingStartingTime)
		{
			if (____formationDrawingStartingPointOfMouse != null)
			{
				Vec2 vec = ____formationDrawingStartingPointOfMouse.Value - __instance.Input.GetMousePositionPixel();
				if ((double)Math.Abs(vec.x) >= 10.0 || (double)Math.Abs(vec.y) >= 10.0)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x00016427 File Offset: 0x00014627
		public static CurrentCursorState GetCursorState()
		{
			Patch_OrderTroopPlacer._cursorState.Invoke(Patch_OrderTroopPlacer._orderTroopPlacer, new object[0]);
			return Patch_OrderTroopPlacer._currentCursorState;
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x00016444 File Offset: 0x00014644
		private static Vec2 GetScreenPoint(OrderTroopPlacer __instance)
		{
			return (Vec2)Patch_OrderTroopPlacer._getScreenPoint.Invoke(__instance, new object[0]);
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x0001645C File Offset: 0x0001465C
		private static void BeginFormationDraggingOrClicking(OrderTroopPlacer __instance, ref Vec2 ____deltaMousePosition, ref WorldPosition? ____formationDrawingStartingPosition, ref Vec2? ____formationDrawingStartingPointOfMouse, ref float? ____formationDrawingStartingTime)
		{
			WorldPosition worldPosition;
			float num;
			WeakGameEntity weakGameEntity;
			if (Patch_OrderTroopPlacer.TryGetScreenMiddleToWorldPosition(__instance, ref ____deltaMousePosition, out worldPosition, out num, out weakGameEntity))
			{
				____formationDrawingStartingPosition = new WorldPosition?(worldPosition);
				____formationDrawingStartingPointOfMouse = new Vec2?(__instance.Input.GetMousePositionPixel());
				____formationDrawingStartingTime = new float?(0f);
				return;
			}
			____formationDrawingStartingPosition = null;
			____formationDrawingStartingPointOfMouse = null;
			____formationDrawingStartingTime = null;
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x000164C4 File Offset: 0x000146C4
		private static bool TryGetScreenMiddleToWorldPosition(OrderTroopPlacer __instance, ref Vec2 ____deltaMousePosition, out WorldPosition worldPosition, out float collisionDistance, out WeakGameEntity collidedEntity)
		{
			if (!__instance.Mission.IsNavalBattle)
			{
				Vec3 vec;
				Vec3 vec2;
				__instance.MissionScreen.ScreenPointToWorldRay(Patch_OrderTroopPlacer.GetScreenPoint(__instance), ref vec, ref vec2);
				float num;
				WeakGameEntity weakGameEntity;
				if (__instance.Mission.Scene.RayCastForClosestEntityOrTerrain(vec, vec2, ref num, ref weakGameEntity, 0.3f, 67188481))
				{
					Vec3 vec3 = vec2 - vec;
					vec3.Normalize();
					collisionDistance = num;
					collidedEntity = weakGameEntity;
					worldPosition = new WorldPosition(__instance.Mission.Scene, UIntPtr.Zero, vec + vec3 * collisionDistance, false);
					return true;
				}
				worldPosition = WorldPosition.Invalid;
				collisionDistance = 0f;
				collidedEntity = WeakGameEntity.Invalid;
				return false;
			}
			else
			{
				Vec3 vec4;
				if (__instance.MissionScreen.GetProjectedMousePositionOnWater(ref vec4))
				{
					worldPosition = new WorldPosition(__instance.Mission.Scene, vec4);
					collisionDistance = (vec4 - __instance.Mission.GetCameraFrame().origin).Length;
					collidedEntity = WeakGameEntity.Invalid;
					return true;
				}
				worldPosition = WorldPosition.Invalid;
				collisionDistance = 0f;
				collidedEntity = WeakGameEntity.Invalid;
				return false;
			}
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x000165F8 File Offset: 0x000147F8
		public static bool Prefix_HandleMouseDown(OrderTroopPlacer __instance, ref Formation ____mouseOverFormation, ref bool ____formationDrawingMode, ref Vec2 ____deltaMousePosition, ref WorldPosition? ____formationDrawingStartingPosition, ref Vec2? ____formationDrawingStartingPointOfMouse, ref float? ____formationDrawingStartingTime, bool ____isMouseDown)
		{
			if (Extensions.IsEmpty<Formation>(__instance.Mission.PlayerTeam.PlayerOrderController.SelectedFormations) || Patch_OrderTroopPlacer._clickedFormation != null)
			{
				return false;
			}
			switch (__instance.Mission.IsNavalBattle ? ((CurrentCursorState)Patch_OrderTroopPlacer._activeCursorState.GetValue(__instance)) : Patch_OrderTroopPlacer._currentCursorState)
			{
			case CurrentCursorState.Normal:
			case CurrentCursorState.Friend:
			case CurrentCursorState.Enemy:
				____formationDrawingMode = true;
				Patch_OrderTroopPlacer.BeginFormationDraggingOrClicking(__instance, ref ____deltaMousePosition, ref ____formationDrawingStartingPosition, ref ____formationDrawingStartingPointOfMouse, ref ____formationDrawingStartingTime);
				break;
			case CurrentCursorState.Rotation:
				return true;
			}
			return false;
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x0001668C File Offset: 0x0001488C
		private static void HideNonSelectedOrderRotationEntities(OrderController ___PlayerOrderController, List<GameEntity> ____orderRotationEntities, Formation formation)
		{
			for (int i = 0; i < ____orderRotationEntities.Count; i++)
			{
				GameEntity gameEntity = ____orderRotationEntities[i];
				if (gameEntity == null && gameEntity.IsVisibleIncludeParents() && ___PlayerOrderController.SelectedFormations.ElementAt<Formation>(i / 2) != formation)
				{
					gameEntity.SetVisibilityExcludeParents(false);
					gameEntity.BodyFlag |= 1;
				}
			}
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x000166EC File Offset: 0x000148EC
		private static void TryTransformFromClickingToDragging(OrderTroopPlacer __instance, Vec2? ____formationDrawingStartingPointOfMouse, float? ____formationDrawingStartingTime, OrderController ___PlayerOrderController, ref Formation ____clickedFormation, ref bool ____formationDrawingMode, bool ____isMouseDown)
		{
			if (Extensions.IsEmpty<Formation>(___PlayerOrderController.SelectedFormations))
			{
				return;
			}
			CurrentCursorState currentCursorState = Patch_OrderTroopPlacer._currentCursorState;
			if (currentCursorState - CurrentCursorState.Friend <= 1 && Patch_OrderTroopPlacer.IsDraggingFormation(__instance, ____formationDrawingStartingPointOfMouse, ____formationDrawingStartingTime) && ((__instance.Input.IsKeyDown(224) || __instance.Input.IsKeyDown(255)) && ____isMouseDown))
			{
				____formationDrawingMode = true;
				____clickedFormation = null;
			}
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x00016750 File Offset: 0x00014950
		public static bool Prefix_GetCursorState(OrderTroopPlacer __instance, ref OrderTroopPlacer.CursorState __result, ref Formation ____mouseOverFormation, List<GameEntity> ____orderRotationEntities, ref Vec2 ____deltaMousePosition, ref bool ____formationDrawingMode, ref int ____mouseOverDirection, bool ____isMouseDown)
		{
			OrderTroopPlacer.CursorState cursorState = (OrderTroopPlacer.CursorState)Patch_OrderTroopPlacer._activeCursorState.GetValue(__instance);
			OrderTroopPlacer.CursorState cursorState2 = 0;
			CurrentCursorState currentCursorState = CurrentCursorState.Invisible;
			if (!Extensions.IsEmpty<Formation>(__instance.Mission.PlayerTeam.PlayerOrderController.SelectedFormations) && Patch_OrderTroopPlacer._clickedFormation == null)
			{
				WorldPosition worldPosition;
				float num;
				WeakGameEntity weakGameEntity;
				if (!Patch_OrderTroopPlacer.TryGetScreenMiddleToWorldPosition(__instance, ref ____deltaMousePosition, out worldPosition, out num, out weakGameEntity))
				{
					num = 1000f;
				}
				if (cursorState2 == null && (double)num < 1000.0)
				{
					if (!____formationDrawingMode && !weakGameEntity.IsValid)
					{
						for (int i = 0; i < ____orderRotationEntities.Count; i++)
						{
							GameEntity gameEntity = ____orderRotationEntities[i];
							if (gameEntity.IsVisibleIncludeParents() && weakGameEntity == gameEntity)
							{
								____mouseOverFormation = __instance.Mission.PlayerTeam.PlayerOrderController.SelectedFormations.ElementAt<Formation>(i / 2);
								____mouseOverDirection = 1 - (i & 1);
								cursorState2 = 3;
								currentCursorState = CurrentCursorState.Rotation;
								break;
							}
						}
					}
					if (cursorState2 == null)
					{
						if (__instance.MissionScreen.OrderFlag.FocusedOrderableObject != null)
						{
							cursorState2 = 5;
							currentCursorState = CurrentCursorState.OrderableEntity;
						}
						else if (MissionConfigBase<CommandSystemConfig>.Get().IsMouseOverEnabled())
						{
							Formation mouseOverFormation = Patch_OrderTroopPlacer.GetMouseOverFormation(__instance, num, __instance.Mission.PlayerTeam.PlayerOrderController, ref ____deltaMousePosition, ____formationDrawingMode);
							if (mouseOverFormation != null)
							{
								Patch_OrderTroopPlacer._lastNewMouseOverFormation = mouseOverFormation;
							}
							if (____mouseOverFormation != null && mouseOverFormation == null)
							{
								if (Patch_OrderTroopPlacer._clearMouseOverFormationTimer.Check(MBCommon.GetApplicationTime()))
								{
									____mouseOverFormation = mouseOverFormation;
									Patch_OrderTroopPlacer._clearMouseOverFormationTimer.Reset(MBCommon.GetApplicationTime(), 0.2f);
									Patch_OrderTroopPlacer._changeMouseOverFormationTimer.Reset(MBCommon.GetApplicationTime(), 0.15f);
								}
								else
								{
									____mouseOverFormation = Patch_OrderTroopPlacer._lastNewMouseOverFormation;
								}
							}
							else if ((____mouseOverFormation != null && mouseOverFormation != null && mouseOverFormation != ____mouseOverFormation && Patch_OrderTroopPlacer._changeMouseOverFormationTimer.Check(MBCommon.GetApplicationTime())) || (mouseOverFormation != null && ____mouseOverFormation == null))
							{
								____mouseOverFormation = mouseOverFormation;
								Patch_OrderTroopPlacer._clearMouseOverFormationTimer.Reset(MBCommon.GetApplicationTime(), 0.2f);
								Patch_OrderTroopPlacer._changeMouseOverFormationTimer.Reset(MBCommon.GetApplicationTime(), 0.15f);
							}
							if (____mouseOverFormation != null)
							{
								if (____mouseOverFormation.Team.IsEnemyOf(__instance.Mission.PlayerTeam))
								{
									if (MissionConfigBase<CommandSystemConfig>.Get().AttackSpecificFormation)
									{
										currentCursorState = CurrentCursorState.Enemy;
									}
								}
								else if (MissionConfigBase<CommandSystemConfig>.Get().ClickToSelectFormation)
								{
									currentCursorState = CurrentCursorState.Friend;
								}
							}
						}
					}
					if (cursorState2 == null)
					{
						cursorState2 = Patch_OrderTroopPlacer.IsCursorStateGroundOrNormal(____formationDrawingMode);
					}
					if ((currentCursorState == CurrentCursorState.Invisible) | ____formationDrawingMode)
					{
						currentCursorState = cursorState2;
					}
				}
			}
			else if (Patch_OrderTroopPlacer._clickedFormation != null)
			{
				cursorState2 = cursorState;
				currentCursorState = Patch_OrderTroopPlacer._currentCursorState;
			}
			if (cursorState2 != 2 && cursorState2 != 3)
			{
				____mouseOverDirection = 0;
			}
			Patch_OrderTroopPlacer._currentCursorState = currentCursorState;
			__result = cursorState2;
			return false;
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x000169AD File Offset: 0x00014BAD
		private static OrderTroopPlacer.CursorState IsCursorStateGroundOrNormal(bool ____formationDrawingMode)
		{
			if (____formationDrawingMode)
			{
				return 2;
			}
			return 1;
		}

		// Token: 0x060003D9 RID: 985 RVA: 0x000169B8 File Offset: 0x00014BB8
		private static Agent RayCastForAgent(OrderTroopPlacer __instance, float distance, ref Vec2 ____deltaMousePosition)
		{
			Vec3 vec;
			Vec3 vec2;
			__instance.MissionScreen.ScreenPointToWorldRay(Patch_OrderTroopPlacer.GetScreenPoint(__instance), ref vec, ref vec2);
			Mission mission = __instance.Mission;
			Vec3 vec3 = vec;
			Vec3 vec4 = vec2;
			Agent lastFollowedAgent = __instance.MissionScreen.LastFollowedAgent;
			float num;
			Agent agent = mission.RayCastForClosestAgent(vec3, vec4, (lastFollowedAgent != null) ? lastFollowedAgent.Index : (-1), 0.3f, ref num);
			if (num > distance || agent == null)
			{
				Mission mission2 = __instance.Mission;
				Vec3 vec5 = vec;
				Vec3 vec6 = vec2;
				Agent lastFollowedAgent2 = __instance.MissionScreen.LastFollowedAgent;
				agent = mission2.RayCastForClosestAgent(vec5, vec6, (lastFollowedAgent2 != null) ? lastFollowedAgent2.Index : (-1), 0.8f, ref num);
			}
			if (num <= distance)
			{
				return agent;
			}
			return null;
		}

		// Token: 0x060003DA RID: 986 RVA: 0x00016A40 File Offset: 0x00014C40
		private static Formation GetMouseOverFormation(OrderTroopPlacer __instance, float collisionDistance, OrderController ___PlayerOrderController, ref Vec2 ____deltaMousePosition, bool ____formationDrawingMode)
		{
			if (Patch_OrderTroopPlacer._focusedFormationsCache != null && Patch_OrderTroopPlacer._focusedFormationsCache.Count > 0)
			{
				return Patch_OrderTroopPlacer._focusedFormationsCache[0];
			}
			Agent agent = Patch_OrderTroopPlacer.RayCastForAgent(__instance, collisionDistance, ref ____deltaMousePosition);
			if (agent != null && agent.IsMount)
			{
				agent = agent.RiderAgent;
			}
			if (agent == null)
			{
				return null;
			}
			if (MissionConfigBase<CommandSystemConfig>.Get().IsMouseOverEnabled() && !__instance.IsDrawingForced && !____formationDrawingMode && ((agent != null) ? agent.Formation : null) != null)
			{
				return agent.Formation;
			}
			return null;
		}

		// Token: 0x060003DB RID: 987 RVA: 0x00016ABC File Offset: 0x00014CBC
		private static void AddOrderPositionEntity(OrderTroopPlacer __instance, int entityIndex, Vec3 groundPosition, bool fadeOut, float alpha, ref List<GameEntity> ____orderPositionEntities)
		{
			CommandSystemConfig commandSystemConfig = MissionConfigBase<CommandSystemConfig>.Get();
			MovementTargetHighlightStyle movementTargetHighlightStyle = (Patch_OrderTroopPlacer.IsFreeCamera ? commandSystemConfig.MovementTargetHighlightStyleInRTSMode : commandSystemConfig.MovementTargetHighlightStyleInCharacterMode);
			if (__instance.Mission.IsNavalRaidBattle)
			{
				movementTargetHighlightStyle = MovementTargetHighlightStyle.Original;
			}
			switch (movementTargetHighlightStyle)
			{
			case MovementTargetHighlightStyle.Original:
				if (Patch_OrderTroopPlacer._originalOrderPositionEntities == null)
				{
					Patch_OrderTroopPlacer._originalOrderPositionEntities = new List<GameEntity>();
					Patch_OrderTroopPlacer._originalMaterial = null;
				}
				break;
			case MovementTargetHighlightStyle.NewModelOnly:
				if (Patch_OrderTroopPlacer._newModelOrderPositionEntities == null)
				{
					Patch_OrderTroopPlacer._newModelOrderPositionEntities = new List<GameEntity>();
					Patch_OrderTroopPlacer._newModelMaterial = Material.GetFromResource("vertex_color_blend_mat").CreateCopy();
				}
				break;
			case MovementTargetHighlightStyle.AlwaysVisible:
				if (Patch_OrderTroopPlacer._alwaysVisibleOrderPositionEntities == null)
				{
					Patch_OrderTroopPlacer._alwaysVisibleOrderPositionEntities = new List<GameEntity>();
					Patch_OrderTroopPlacer._alwaysVisibleMaterial = Material.GetFromResource("vertex_color_blend_no_depth_mat").CreateCopy();
					Patch_OrderTroopPlacer._alwaysVisibleMaterial.Flags |= 2;
					Patch_OrderTroopPlacer._alwaysVisibleMaterial.Flags &= -536870913;
				}
				break;
			}
			if (Patch_OrderTroopPlacer._previousMovementTargetHightlightStyle != movementTargetHighlightStyle)
			{
				IEnumerable<GameEntity> enumerable;
				switch (Patch_OrderTroopPlacer._previousMovementTargetHightlightStyle)
				{
				case MovementTargetHighlightStyle.Original:
				{
					Patch_OrderTroopPlacer._originalMaterial = Patch_OrderTroopPlacer._currentMaterial;
					enumerable = Patch_OrderTroopPlacer._originalOrderPositionEntities;
					using (IEnumerator<GameEntity> enumerator = (enumerable ?? Enumerable.Empty<GameEntity>()).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							GameEntity gameEntity = enumerator.Current;
							gameEntity.SetVisibilityExcludeParents(false);
						}
						goto IL_01B2;
					}
					break;
				}
				case MovementTargetHighlightStyle.NewModelOnly:
					break;
				case MovementTargetHighlightStyle.AlwaysVisible:
					goto IL_0178;
				default:
					goto IL_01B2;
				}
				enumerable = Patch_OrderTroopPlacer._newModelOrderPositionEntities;
				using (IEnumerator<GameEntity> enumerator = (enumerable ?? Enumerable.Empty<GameEntity>()).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						GameEntity gameEntity2 = enumerator.Current;
						gameEntity2.SetVisibilityExcludeParents(false);
					}
					goto IL_01B2;
				}
				IL_0178:
				enumerable = Patch_OrderTroopPlacer._alwaysVisibleOrderPositionEntities;
				foreach (GameEntity gameEntity3 in (enumerable ?? Enumerable.Empty<GameEntity>()))
				{
					gameEntity3.SetVisibilityExcludeParents(false);
				}
				IL_01B2:
				Patch_OrderTroopPlacer._previousMovementTargetHightlightStyle = movementTargetHighlightStyle;
				switch (movementTargetHighlightStyle)
				{
				case MovementTargetHighlightStyle.Original:
					____orderPositionEntities = Patch_OrderTroopPlacer._originalOrderPositionEntities;
					Patch_OrderTroopPlacer._currentMaterial = Patch_OrderTroopPlacer._originalMaterial;
					break;
				case MovementTargetHighlightStyle.NewModelOnly:
					____orderPositionEntities = Patch_OrderTroopPlacer._newModelOrderPositionEntities;
					Patch_OrderTroopPlacer._currentMaterial = Patch_OrderTroopPlacer._newModelMaterial;
					break;
				case MovementTargetHighlightStyle.AlwaysVisible:
					____orderPositionEntities = Patch_OrderTroopPlacer._alwaysVisibleOrderPositionEntities;
					Patch_OrderTroopPlacer._currentMaterial = Patch_OrderTroopPlacer._alwaysVisibleMaterial;
					break;
				}
			}
			if (movementTargetHighlightStyle != MovementTargetHighlightStyle.Original)
			{
				while (____orderPositionEntities.Count <= entityIndex)
				{
					GameEntity gameEntity4 = GameEntity.CreateEmpty(__instance.Mission.Scene, true, true, true);
					gameEntity4.EntityFlags |= 4194304;
					MetaMesh copy = MetaMesh.GetCopy("barrier_sphere", true, false);
					if (Patch_OrderTroopPlacer._currentMaterial == null)
					{
						Patch_OrderTroopPlacer._currentMaterial = Material.GetFromResource("vertex_color_blend_no_depth_mat").CreateCopy();
					}
					copy.SetMaterial(Patch_OrderTroopPlacer._currentMaterial);
					copy.SetFactor1(Patch_OrderTroopPlacer.OrderPositionEntityColor);
					gameEntity4.AddComponent(copy);
					gameEntity4.SetVisibilityExcludeParents(false);
					____orderPositionEntities.Add(gameEntity4);
				}
				GameEntity gameEntity5 = ____orderPositionEntities[entityIndex];
				Mat3 mat = Mat3.Identity;
				Vec3 vec = groundPosition + Vec3.Up * 1f;
				MatrixFrame matrixFrame = new MatrixFrame(ref mat, ref vec);
				gameEntity5.SetFrame(ref matrixFrame, true);
				if (fadeOut)
				{
					GameEntityExtensions.FadeOut(gameEntity5, MissionConfigBase<CommandSystemConfig>.Get().MovementTargetFadeOutDuration, false);
					return;
				}
				if ((double)alpha != -1.0)
				{
					alpha = Patch_OrderTroopPlacer.OrderPositionEntityDestinationAlpha;
					gameEntity5.SetVisibilityExcludeParents(true);
					gameEntity5.SetAlpha(alpha);
					return;
				}
				GameEntityExtensions.FadeIn(gameEntity5, true);
				return;
			}
			else
			{
				while (____orderPositionEntities.Count <= entityIndex)
				{
					GameEntity gameEntity6 = GameEntity.CreateEmpty(__instance.Mission.Scene, true, true, true);
					gameEntity6.EntityFlags |= 4194304;
					MetaMesh copy2 = MetaMesh.GetCopy("order_flag_small", true, false);
					gameEntity6.AddComponent(copy2);
					gameEntity6.SetVisibilityExcludeParents(false);
					____orderPositionEntities.Add(gameEntity6);
				}
				GameEntity gameEntity7 = ____orderPositionEntities[entityIndex];
				Mat3 mat = Mat3.Identity;
				MatrixFrame matrixFrame2 = new MatrixFrame(ref mat, ref groundPosition);
				gameEntity7.SetFrame(ref matrixFrame2, true);
				if ((double)alpha != -1.0)
				{
					gameEntity7.SetVisibilityExcludeParents(true);
					gameEntity7.SetAlpha(alpha);
					return;
				}
				if (fadeOut)
				{
					GameEntityExtensions.FadeOut(gameEntity7, 0.3f, false);
					return;
				}
				GameEntityExtensions.FadeIn(gameEntity7, true);
				return;
			}
		}

		// Token: 0x060003DC RID: 988 RVA: 0x00016EDC File Offset: 0x000150DC
		public static bool Prefix_AddOrderPositionEntity(OrderTroopPlacer __instance, int entityIndex, ref Vec3 groundPosition, bool fadeOut, float alpha, ref List<GameEntity> ____orderPositionEntities)
		{
			Patch_OrderTroopPlacer.AddOrderPositionEntity(__instance, entityIndex, groundPosition, fadeOut, alpha, ref ____orderPositionEntities);
			return false;
		}

		// Token: 0x060003DD RID: 989 RVA: 0x00016EF4 File Offset: 0x000150F4
		private static void HandleSelectFormationKeyDown(OrderTroopPlacer __instance, ref Formation ____clickedFormation, ref Formation ____mouseOverFormation, ref bool ____formationDrawingMode, ref Vec2 ____deltaMousePosition, ref WorldPosition? ____formationDrawingStartingPosition, ref Vec2? ____formationDrawingStartingPointOfMouse, ref float? ____formationDrawingStartingTime)
		{
			if (Extensions.IsEmpty<Formation>(__instance.Mission.PlayerTeam.PlayerOrderController.SelectedFormations) || ____clickedFormation != null)
			{
				return;
			}
			CurrentCursorState currentCursorState = Patch_OrderTroopPlacer._currentCursorState;
			if (currentCursorState != CurrentCursorState.Friend)
			{
				if (currentCursorState == CurrentCursorState.Enemy)
				{
					____formationDrawingMode = false;
					____clickedFormation = ____mouseOverFormation;
					Patch_OrderTroopPlacer.BeginFormationDraggingOrClicking(__instance, ref ____deltaMousePosition, ref ____formationDrawingStartingPosition, ref ____formationDrawingStartingPointOfMouse, ref ____formationDrawingStartingTime);
					return;
				}
			}
			else
			{
				____formationDrawingMode = false;
				if (____mouseOverFormation != null && __instance.Mission.PlayerTeam.PlayerOrderController.IsFormationSelectable(____mouseOverFormation))
				{
					____clickedFormation = ____mouseOverFormation;
				}
				Patch_OrderTroopPlacer.BeginFormationDraggingOrClicking(__instance, ref ____deltaMousePosition, ref ____formationDrawingStartingPosition, ref ____formationDrawingStartingPointOfMouse, ref ____formationDrawingStartingTime);
			}
		}

		// Token: 0x060003DE RID: 990 RVA: 0x00016F78 File Offset: 0x00015178
		private static void HandleSelectFormationKeyUp(OrderTroopPlacer __instance, ref Formation ____clickedFormation, OrderController ___PlayerOrderController, List<GameEntity> ____orderRotationEntities, ref bool ____formationDrawingMode, ref Vec2 ____deltaMousePosition, ref WorldPosition? ____formationDrawingStartingPosition, ref Vec2? ____formationDrawingStartingPointOfMouse, ref float? ____formationDrawingStartingTime)
		{
			if (!____formationDrawingMode)
			{
				if (Patch_OrderTroopPlacer._focusedFormationsCache != null && Patch_OrderTroopPlacer._focusedFormationsCache.Count > 0)
				{
					____clickedFormation = Patch_OrderTroopPlacer._focusedFormationsCache.FirstOrDefault<Formation>();
				}
				if (____clickedFormation != null)
				{
					if (____clickedFormation.CountOfUnits > 0)
					{
						if (!MissionSharedLibrary.Utilities.Utility.IsEnemy(____clickedFormation))
						{
							Patch_OrderTroopPlacer.HideNonSelectedOrderRotationEntities(___PlayerOrderController, ____orderRotationEntities, ____clickedFormation);
							if (___PlayerOrderController.IsFormationSelectable(____clickedFormation))
							{
								Patch_OrderTroopPlacer.SelectFormationFromController(__instance, ___PlayerOrderController, ____clickedFormation);
							}
						}
						else if (MissionConfigBase<CommandSystemConfig>.Get().AttackSpecificFormation)
						{
							if (CommandSystemGameKeyCategory.GetKey(GameKeyEnum.KeepMovementOrder).IsKeyDownInOrder(__instance.Input))
							{
								if (Campaign.Current == null || CommandSystemSkillBehavior.CanIssueChargeToFormationOrder)
								{
									RTSCamera.CommandSystem.Utilities.Utility.FocusOnFormation(___PlayerOrderController, ____clickedFormation);
								}
								else
								{
									MissionSharedLibrary.Utilities.Utility.DisplayMessage(GameTexts.FindText("str_rts_camera_command_system_tactic_level_required", null).SetTextVariable("level", CommandSystemSkillBehavior.RequiredTacticsLevelToIssueChargeToFormationOrder).ToString());
								}
							}
							else
							{
								RTSCamera.CommandSystem.Utilities.Utility.ChargeToFormation(___PlayerOrderController, ____clickedFormation);
							}
						}
					}
					____clickedFormation = null;
					____formationDrawingMode = false;
					____formationDrawingStartingPosition = null;
					____formationDrawingStartingPointOfMouse = null;
					____formationDrawingStartingTime = null;
					____deltaMousePosition = Vec2.Zero;
				}
			}
		}

		// Token: 0x060003DF RID: 991 RVA: 0x00017078 File Offset: 0x00015278
		public static bool Prefix_OnMissionScreenTick(OrderTroopPlacer __instance, ref bool ____initialized, ref bool ____isDrawnThisFrame, ref bool ____isMouseDown, ref Timer ___formationDrawTimer, ref Vec2? ____formationDrawingStartingPointOfMouse, ref float? ____formationDrawingStartingTime, ref bool ____formationDrawingMode, Formation ____mouseOverFormation, ref List<GameEntity> ____orderPositionEntities, ref List<GameEntity> ____orderRotationEntities, ref bool ____wasDrawingForced, ref bool ____wasDrawingFacing, ref bool ____wasDrawingForming, ref bool ____wasDrawnPreviousFrame, ref WorldPosition? ____formationDrawingStartingPosition, ref Vec2 ____deltaMousePosition, ref OrderController ____orderController)
		{
			if (!____initialized)
			{
				return false;
			}
			Patch_OrderTroopPlacer._activeCursorState.SetValue(__instance, (OrderTroopPlacer.CursorState)Patch_OrderTroopPlacer._cursorState.Invoke(Patch_OrderTroopPlacer._orderTroopPlacer, new object[0]));
			if (!Patch_OrderTroopPlacer.CanUpdate(__instance, ____orderController))
			{
				return false;
			}
			____isDrawnThisFrame = false;
			if (__instance.SuspendTroopPlacer)
			{
				return false;
			}
			bool flag = MissionConfigBase<CommandSystemConfig>.Get().IsMouseOverEnabled() && CommandSystemGameKeyCategory.GetKey(GameKeyEnum.SelectFormation).IsKeyPressed(__instance.Input);
			bool flag2 = MissionConfigBase<CommandSystemConfig>.Get().IsMouseOverEnabled() && CommandSystemGameKeyCategory.GetKey(GameKeyEnum.SelectFormation).IsKeyReleased(__instance.Input);
			bool flag3 = MissionConfigBase<CommandSystemConfig>.Get().IsMouseOverEnabled() && CommandSystemGameKeyCategory.GetKey(GameKeyEnum.SelectFormation).IsKeyDown(__instance.Input);
			if (__instance.Input.IsKeyPressed(224) || __instance.Input.IsKeyPressed(255))
			{
				____isMouseDown = true;
				MethodInfo handleMouseDown = Patch_OrderTroopPlacer._handleMouseDown;
				if (handleMouseDown != null)
				{
					handleMouseDown.Invoke(__instance, new object[0]);
				}
			}
			if (flag)
			{
				Patch_OrderTroopPlacer.HandleSelectFormationKeyDown(__instance, ref Patch_OrderTroopPlacer._clickedFormation, ref ____mouseOverFormation, ref ____formationDrawingMode, ref ____deltaMousePosition, ref ____formationDrawingStartingPosition, ref ____formationDrawingStartingPointOfMouse, ref ____formationDrawingStartingTime);
			}
			if (flag2)
			{
				Patch_OrderTroopPlacer.HandleSelectFormationKeyUp(__instance, ref Patch_OrderTroopPlacer._clickedFormation, __instance.Mission.PlayerTeam.PlayerOrderController, ____orderRotationEntities, ref ____formationDrawingMode, ref ____deltaMousePosition, ref ____formationDrawingStartingPosition, ref ____formationDrawingStartingPointOfMouse, ref ____formationDrawingStartingTime);
			}
			if (((__instance.Input.IsKeyReleased(224) || __instance.Input.IsKeyReleased(255)) & ____isMouseDown) || flag2)
			{
				____isMouseDown = false;
				Patch_OrderTroopPlacer._skipDrawingForDestinationForOneTick = true;
				MethodInfo handleMouseUp = Patch_OrderTroopPlacer._handleMouseUp;
				if (handleMouseUp != null)
				{
					handleMouseUp.Invoke(__instance, new object[0]);
				}
			}
			else if (____isMouseDown && flag3 && !__instance.IsDrawingFacing && !__instance.IsDrawingForming)
			{
				Patch_OrderTroopPlacer.TryTransformFromClickingToDragging(__instance, ____formationDrawingStartingPointOfMouse, ____formationDrawingStartingTime, __instance.Mission.PlayerTeam.PlayerOrderController, ref Patch_OrderTroopPlacer._clickedFormation, ref ____formationDrawingMode, ____isMouseDown);
			}
			if ((__instance.Input.IsKeyDown(224) || __instance.Input.IsKeyDown(255)) & ____isMouseDown)
			{
				if (___formationDrawTimer.Check(MBCommon.GetApplicationTime()) && !__instance.IsDrawingFacing && !__instance.IsDrawingForming && Patch_OrderTroopPlacer._currentCursorState == CurrentCursorState.Ground)
				{
					__instance.UpdateFormationDrawing(false);
				}
			}
			else if (__instance.IsDrawingForced)
			{
				if (___formationDrawTimer.Check(MBCommon.GetApplicationTime()))
				{
					Patch_OrderTroopPlacer.Reset(ref ____isMouseDown, ref ____formationDrawingMode, ref ____formationDrawingStartingPosition, ref ____formationDrawingStartingPointOfMouse, ref ____formationDrawingStartingTime, ref ____mouseOverFormation, ref Patch_OrderTroopPlacer._clickedFormation);
					____formationDrawingMode = true;
					Patch_OrderTroopPlacer.BeginFormationDraggingOrClicking(__instance, ref ____deltaMousePosition, ref ____formationDrawingStartingPosition, ref ____formationDrawingStartingPointOfMouse, ref ____formationDrawingStartingTime);
					__instance.UpdateFormationDrawing(false);
				}
			}
			else if (__instance.IsDrawingFacing | ____wasDrawingFacing)
			{
				if (__instance.IsDrawingFacing)
				{
					Patch_OrderTroopPlacer.Reset(ref ____isMouseDown, ref ____formationDrawingMode, ref ____formationDrawingStartingPosition, ref ____formationDrawingStartingPointOfMouse, ref ____formationDrawingStartingTime, ref ____mouseOverFormation, ref Patch_OrderTroopPlacer._clickedFormation);
					Patch_OrderTroopPlacer._updateFormationDrawingForFacingOrder.Invoke(__instance, new object[] { false });
				}
			}
			else if (__instance.IsDrawingForming | ____wasDrawingForming)
			{
				if (__instance.IsDrawingForming)
				{
					Patch_OrderTroopPlacer.Reset(ref ____isMouseDown, ref ____formationDrawingMode, ref ____formationDrawingStartingPosition, ref ____formationDrawingStartingPointOfMouse, ref ____formationDrawingStartingTime, ref ____mouseOverFormation, ref Patch_OrderTroopPlacer._clickedFormation);
					Patch_OrderTroopPlacer._updateFormationDrawingForFormingOrder.Invoke(__instance, new object[] { false });
				}
			}
			else if (____wasDrawingForced)
			{
				Patch_OrderTroopPlacer.Reset(ref ____isMouseDown, ref ____formationDrawingMode, ref ____formationDrawingStartingPosition, ref ____formationDrawingStartingPointOfMouse, ref ____formationDrawingStartingTime, ref ____mouseOverFormation, ref Patch_OrderTroopPlacer._clickedFormation);
			}
			else if (Patch_OrderTroopPlacer._skipDrawingForDestinationForOneTick)
			{
				Patch_OrderTroopPlacer._skipDrawingForDestinationForOneTick = false;
			}
			else
			{
				Patch_OrderTroopPlacer._updateFormationDrawingForDestination.Invoke(__instance, new object[] { false });
			}
			Patch_OrderTroopPlacer.UpdateMouseOverFormation(____mouseOverFormation);
			foreach (GameEntity gameEntity in ____orderPositionEntities)
			{
				gameEntity.SetPreviousFrameInvalid();
			}
			foreach (GameEntity gameEntity2 in ____orderRotationEntities)
			{
				gameEntity2.SetPreviousFrameInvalid();
			}
			____wasDrawingForced = __instance.IsDrawingForced;
			____wasDrawingFacing = __instance.IsDrawingFacing;
			____wasDrawingForming = __instance.IsDrawingForming;
			____wasDrawnPreviousFrame = ____isDrawnThisFrame;
			return false;
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x00017480 File Offset: 0x00015680
		private static void UpdateMouseOverFormation(Formation ____mouseOverFormation)
		{
			FormationColorSubLogicV2 outlineView = Patch_OrderTroopPlacer._outlineView;
			if (outlineView != null)
			{
				outlineView.MouseOver(____mouseOverFormation);
			}
			FormationColorSubLogicV2 groundMarkerView = Patch_OrderTroopPlacer._groundMarkerView;
			if (groundMarkerView == null)
			{
				return;
			}
			groundMarkerView.MouseOver(____mouseOverFormation);
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x000174A3 File Offset: 0x000156A3
		private static void Reset(ref bool ____isMouseDown, ref bool ____formationDrawingMode, ref WorldPosition? ____formationDrawingStartingPosition, ref Vec2? ____formationDrawingStartingPointOfMouse, ref float? ____formationDrawingStartingTime, ref Formation ____mouseOverFormation, ref Formation ____clickedFormation)
		{
			____isMouseDown = false;
			____formationDrawingMode = false;
			____formationDrawingStartingPosition = null;
			____formationDrawingStartingPointOfMouse = null;
			____formationDrawingStartingTime = null;
			____mouseOverFormation = null;
			____clickedFormation = null;
		}

		// Token: 0x060003E2 RID: 994 RVA: 0x000174C9 File Offset: 0x000156C9
		public static void SelectFormationFromController(OrderTroopPlacer __instance, OrderController ___PlayerOrderController, Formation ____clickedFormation)
		{
			if (!CommandSystemGameKeyCategory.GetKey(GameKeyEnum.KeepFormationWidth).IsKeyDownInOrder(__instance.Input))
			{
				___PlayerOrderController.ClearSelectedFormations();
				___PlayerOrderController.SelectFormation(____clickedFormation);
				return;
			}
			if (___PlayerOrderController.IsFormationListening(____clickedFormation))
			{
				___PlayerOrderController.DeselectFormation(____clickedFormation);
				return;
			}
			___PlayerOrderController.SelectFormation(____clickedFormation);
		}

		// Token: 0x060003E3 RID: 995 RVA: 0x00017504 File Offset: 0x00015704
		public static bool Prefix_UpdateFormationDrawingForMovementOrder(OrderTroopPlacer __instance, bool giveOrder, WorldPosition formationRealStartingPosition, WorldPosition formationRealEndingPosition, bool isFormationLayoutVertical, ref bool ____isDrawnThisFrame)
		{
			bool flag = RTSCamera.CommandSystem.Utilities.Utility.ShouldQueueCommand();
			if (!flag)
			{
				Patch_OrderController.LivePreviewFormationChanges.SetChanges(CommandQueueLogic.CurrentFormationChanges.CollectChanges(__instance.Mission.PlayerTeam.PlayerOrderController.SelectedFormations));
			}
			else
			{
				Patch_OrderController.LivePreviewFormationChanges.SetChanges(CommandQueueLogic.LatestOrderInQueueChanges.CollectChanges(__instance.Mission.PlayerTeam.PlayerOrderController.SelectedFormations));
			}
			Patch_OrderController.LivePreviewFormationChanges.SetMovementOrder(isFormationLayoutVertical ? 2 : 3, __instance.Mission.PlayerTeam.PlayerOrderController.SelectedFormations, null, null, null);
			____isDrawnThisFrame = true;
			List<WorldPosition> list = null;
			bool flag2 = false;
			IEnumerable<Formation> enumerable = __instance.Mission.PlayerTeam.PlayerOrderController.SelectedFormations.Where<Formation>((Formation f) => f.CountOfUnitsWithoutDetachedOnes > 0);
			if (!enumerable.Any<Formation>())
			{
				return true;
			}
			bool flag3 = RTSCamera.CommandSystem.Utilities.Utility.ShouldFadeOut() && giveOrder && !flag;
			bool flag4 = !giveOrder || flag3;
			List<ValueTuple<Formation, int, float, WorldPosition, Vec2>> list2;
			Patch_OrderController.SimulateNewOrderWithPositionAndDirection(enumerable, __instance.Mission.PlayerTeam.PlayerOrderController.simulationFormations, formationRealStartingPosition, formationRealEndingPosition, flag4, out list, giveOrder, out list2, out flag2, isFormationLayoutVertical, false);
			bool flag5 = RTSCamera.CommandSystem.Utilities.Utility.ShouldLockFormation();
			if (giveOrder)
			{
				if (!flag)
				{
					if (!isFormationLayoutVertical)
					{
						__instance.Mission.PlayerTeam.PlayerOrderController.SetOrderWithTwoPositions(3, formationRealStartingPosition, formationRealEndingPosition);
					}
					else
					{
						__instance.Mission.PlayerTeam.PlayerOrderController.SetOrderWithTwoPositions(2, formationRealStartingPosition, formationRealEndingPosition);
					}
					CommandQueueLogic.TryPendingOrder(enumerable, new OrderInQueue
					{
						SelectedFormations = enumerable.ToList<Formation>(),
						OrderType = (isFormationLayoutVertical ? 2 : 3),
						IsLineShort = flag2,
						PositionBegin = formationRealStartingPosition,
						PositionEnd = formationRealEndingPosition,
						VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(__instance.Mission.PlayerTeam.PlayerOrderController.SelectedFormations),
						ShouldAdjustFormationSpeed = flag5
					});
				}
				else
				{
					CommandQueueLogic.AddOrderToQueue(new OrderInQueue
					{
						SelectedFormations = enumerable.ToList<Formation>(),
						OrderType = (isFormationLayoutVertical ? 2 : 3),
						IsLineShort = flag2,
						ActualFormationChanges = list2,
						PositionBegin = formationRealStartingPosition,
						PositionEnd = formationRealEndingPosition,
						VirtualFormationChanges = Patch_OrderController.LivePreviewFormationChanges.CollectChanges(__instance.Mission.PlayerTeam.PlayerOrderController.SelectedFormations),
						ShouldAdjustFormationSpeed = flag5
					});
					RTSCamera.CommandSystem.Utilities.Utility.MissionOrderVM_OnOrderExecutedWithId("order_movement_move");
				}
			}
			if (flag4)
			{
				Patch_OrderTroopPlacer.AddOrderPositionEntities(list, flag3, 0);
			}
			return false;
		}

		// Token: 0x060003E4 RID: 996 RVA: 0x00017764 File Offset: 0x00015964
		private static Vec3 GetGroundedVec3(Mission mission, WorldPosition worldPosition)
		{
			if (mission.IsNavalBattle)
			{
				Vec2 asVec = worldPosition.AsVec2;
				return new Vec3(asVec.X, asVec.Y, mission.Scene.GetWaterLevelAtPosition(asVec, true, true), -1f);
			}
			return worldPosition.GetGroundVec3();
		}

		// Token: 0x060003E5 RID: 997 RVA: 0x000177B0 File Offset: 0x000159B0
		public static bool Prefix_UpdateFormationDrawingForFacingOrder(OrderTroopPlacer __instance, bool giveOrder)
		{
			if (!RTSCamera.CommandSystem.Utilities.Utility.ShouldQueueCommand())
			{
				Patch_OrderController.LivePreviewFormationChanges.SetChanges(CommandQueueLogic.CurrentFormationChanges.CollectChanges(__instance.Mission.PlayerTeam.PlayerOrderController.SelectedFormations));
			}
			else
			{
				Patch_OrderController.LivePreviewFormationChanges.SetChanges(CommandQueueLogic.LatestOrderInQueueChanges.CollectChanges(__instance.Mission.PlayerTeam.PlayerOrderController.SelectedFormations));
			}
			return true;
		}

		// Token: 0x060003E6 RID: 998 RVA: 0x00017819 File Offset: 0x00015A19
		public static void SetIsDrawingFacing(bool isDrawingFacing)
		{
			if (Patch_OrderTroopPlacer._orderTroopPlacer == null)
			{
				return;
			}
			Patch_OrderTroopPlacer._orderTroopPlacer.IsDrawingFacing = isDrawingFacing;
		}

		// Token: 0x060003E7 RID: 999 RVA: 0x0001782E File Offset: 0x00015A2E
		public static void Reset()
		{
			if (Patch_OrderTroopPlacer._orderTroopPlacer == null)
			{
				return;
			}
			Patch_OrderTroopPlacer._reset.Invoke(Patch_OrderTroopPlacer._orderTroopPlacer, new object[0]);
		}

		// Token: 0x060003E8 RID: 1000 RVA: 0x00017850 File Offset: 0x00015A50
		public static void AddOrderPositionEntities(List<WorldPosition> agentFrames, bool fadeOut, int startIndex = 0)
		{
			if (agentFrames == null)
			{
				MissionSharedLibrary.Utilities.Utility.DisplayMessage("RTS Command: agentFrames is null in AddOrderPositionEntities", new Color(1f, 0f, 0f, 1f));
				return;
			}
			ref List<GameEntity> ptr = ref AccessTools.FieldRefAccess<OrderTroopPlacer, List<GameEntity>>(Patch_OrderTroopPlacer._orderTroopPlacer, Patch_OrderTroopPlacer._orderPositionEntities);
			foreach (WorldPosition worldPosition in agentFrames)
			{
				Patch_OrderTroopPlacer.AddOrderPositionEntity(Patch_OrderTroopPlacer._orderTroopPlacer, startIndex, Patch_OrderTroopPlacer.GetGroundedVec3(Patch_OrderTroopPlacer._orderTroopPlacer.Mission, worldPosition), fadeOut, -1f, ref ptr);
				startIndex++;
			}
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x000178F8 File Offset: 0x00015AF8
		public static bool Prefix_HideOrderPositionEntities(OrderTroopPlacer __instance, ref List<GameEntity> ____orderPositionEntities, List<GameEntity> ____orderRotationEntities)
		{
			if (__instance.SuspendTroopPlacer)
			{
				using (List<GameEntity>.Enumerator enumerator = ____orderPositionEntities.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						GameEntity gameEntity = enumerator.Current;
						if (gameEntity.IsVisibleIncludeParents())
						{
							GameEntityExtensions.FadeOut(gameEntity, MissionConfigBase<CommandSystemConfig>.Get().MovementTargetFadeOutDuration, false);
						}
						else
						{
							GameEntityExtensions.HideIfNotFadingOut(gameEntity);
						}
					}
					goto IL_0084;
				}
			}
			foreach (GameEntity gameEntity2 in ____orderPositionEntities)
			{
				gameEntity2.SetVisibilityExcludeParents(false);
			}
			IL_0084:
			for (int i = 0; i < ____orderRotationEntities.Count; i++)
			{
				GameEntity gameEntity3 = ____orderRotationEntities[i];
				gameEntity3.SetVisibilityExcludeParents(false);
				gameEntity3.BodyFlag |= 1;
			}
			return false;
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x000179D4 File Offset: 0x00015BD4
		private static bool CanUpdate(OrderTroopPlacer __instance, OrderController ____orderController)
		{
			OrderController orderController = Patch_OrderTroopPlacer.GetOrderController(__instance, ____orderController);
			if (!__instance.Mission.IsNavalRaidBattle || orderController == Mission.Current.PlayerEnemyTeam.MasterOrderController || (__instance.Mission.IsNavalRaidBattle && orderController.Team.Side == null))
			{
				return Patch_OrderTroopPlacer.base_CanUpdate(__instance, ____orderController);
			}
			if (!Patch_OrderTroopPlacer.base_CanUpdate(__instance, ____orderController))
			{
				return false;
			}
			MissionBehavior navalShipsLogic = MissionSharedLibrary.Utilities.Utility.GetNavalShipsLogic(__instance.Mission);
			return navalShipsLogic != null && MissionSharedLibrary.Utilities.Utility.GetNumTeamShips(navalShipsLogic, 0) > 0;
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x00017A50 File Offset: 0x00015C50
		private static bool base_CanUpdate(OrderTroopPlacer __instance, OrderController ____orderController)
		{
			return Patch_OrderTroopPlacer.GetOrderController(__instance, ____orderController).SelectedFormations.Count > 0;
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x00017A66 File Offset: 0x00015C66
		private static OrderController GetOrderController(OrderTroopPlacer __instance, OrderController ____orderController)
		{
			if (____orderController == null)
			{
				return Mission.Current.PlayerTeam.PlayerOrderController;
			}
			return ____orderController;
		}

		// Token: 0x04000174 RID: 372
		public static uint OrderPositionEntityColor = new Color(0.15f, 0.65f, 0.15f, 1f).ToUnsignedInteger();

		// Token: 0x04000175 RID: 373
		public static float OrderPositionEntityPreviewAlpha = 1f;

		// Token: 0x04000176 RID: 374
		public static float OrderPositionEntityDestinationAlpha = 0.5f;

		// Token: 0x04000177 RID: 375
		private static float _cachedTimeOfDay = 0f;

		// Token: 0x04000178 RID: 376
		private static bool _patched;

		// Token: 0x04000179 RID: 377
		private static readonly FieldInfo _dataSource = typeof(MissionGauntletSingleplayerOrderUIHandler).GetField("_dataSource", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x0400017A RID: 378
		private static readonly FieldInfo _orderPositionEntities = typeof(OrderTroopPlacer).GetField("_orderPositionEntities", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x0400017B RID: 379
		private static readonly PropertyInfo _activeCursorState = typeof(OrderTroopPlacer).GetProperty("ActiveCursorState", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x0400017C RID: 380
		private static readonly MethodInfo _cursorState = typeof(OrderTroopPlacer).GetMethod("GetCursorState", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x0400017D RID: 381
		private static readonly MethodInfo _handleMouseDown = typeof(OrderTroopPlacer).GetMethod("HandleMouseDown", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x0400017E RID: 382
		private static readonly MethodInfo _handleMouseUp = typeof(OrderTroopPlacer).GetMethod("HandleMouseUp", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x0400017F RID: 383
		private static readonly MethodInfo _updateFormationDrawingForFacingOrder = typeof(OrderTroopPlacer).GetMethod("UpdateFormationDrawingForFacingOrder", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000180 RID: 384
		private static readonly MethodInfo _updateFormationDrawingForFormingOrder = typeof(OrderTroopPlacer).GetMethod("UpdateFormationDrawingForFormingOrder", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000181 RID: 385
		private static readonly MethodInfo _updateFormationDrawingForDestination = typeof(OrderTroopPlacer).GetMethod("UpdateFormationDrawingForDestination", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000182 RID: 386
		private static readonly MethodInfo _getGroundVec3 = typeof(OrderTroopPlacer).GetMethod("GetGroundedVec3", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000183 RID: 387
		private static readonly MethodInfo _addOrderPositionEntity = typeof(OrderTroopPlacer).GetMethod("AddOrderPositionEntity", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000184 RID: 388
		private static readonly MethodInfo _reset = typeof(OrderTroopPlacer).GetMethod("Reset", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000185 RID: 389
		private static readonly MethodInfo _getScreenPoint = typeof(OrderTroopPlacer).GetMethod("GetScreenPoint", BindingFlags.Instance | BindingFlags.NonPublic);

		// Token: 0x04000186 RID: 390
		private static bool _isInitialized = false;

		// Token: 0x04000187 RID: 391
		private static CurrentCursorState _currentCursorState = CurrentCursorState.Invisible;

		// Token: 0x04000188 RID: 392
		private static Formation _clickedFormation = null;

		// Token: 0x04000189 RID: 393
		private static Formation _lastNewMouseOverFormation = null;

		// Token: 0x0400018A RID: 394
		private static UiQueryData<CurrentCursorState> _cachedCursorState;

		// Token: 0x0400018B RID: 395
		private static FormationColorSubLogicV2 _outlineView;

		// Token: 0x0400018C RID: 396
		private static FormationColorSubLogicV2 _groundMarkerView;

		// Token: 0x0400018D RID: 397
		private static OrderTroopPlacer _orderTroopPlacer;

		// Token: 0x0400018E RID: 398
		private static MissionFormationTargetSelectionHandler _targetSelectionHandler;

		// Token: 0x0400018F RID: 399
		private static MBReadOnlyList<Formation> _focusedFormationsCache;

		// Token: 0x04000190 RID: 400
		public static bool IsFreeCamera;

		// Token: 0x04000191 RID: 401
		private static MovementTargetHighlightStyle _previousMovementTargetHightlightStyle = MovementTargetHighlightStyle.Count;

		// Token: 0x04000192 RID: 402
		private static List<GameEntity> _originalOrderPositionEntities;

		// Token: 0x04000193 RID: 403
		private static List<GameEntity> _newModelOrderPositionEntities;

		// Token: 0x04000194 RID: 404
		private static List<GameEntity> _alwaysVisibleOrderPositionEntities;

		// Token: 0x04000195 RID: 405
		private static Material _currentMaterial;

		// Token: 0x04000196 RID: 406
		private static Material _originalMaterial;

		// Token: 0x04000197 RID: 407
		private static Material _newModelMaterial;

		// Token: 0x04000198 RID: 408
		private static Material _alwaysVisibleMaterial;

		// Token: 0x04000199 RID: 409
		private static bool _skipDrawingForDestinationForOneTick;

		// Token: 0x0400019A RID: 410
		private static Timer _clearMouseOverFormationTimer;

		// Token: 0x0400019B RID: 411
		private static Timer _changeMouseOverFormationTimer;
	}
}
