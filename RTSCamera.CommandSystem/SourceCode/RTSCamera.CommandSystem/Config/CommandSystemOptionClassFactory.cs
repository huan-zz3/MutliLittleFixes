using System;
using System.Collections.Generic;
using MissionLibrary.Provider;
using MissionLibrary.View;
using MissionSharedLibrary.Config;
using MissionSharedLibrary.Provider;
using MissionSharedLibrary.View.ViewModelCollection;
using MissionSharedLibrary.View.ViewModelCollection.Options;
using MissionSharedLibrary.View.ViewModelCollection.Options.Selection;
using RTSCamera.CommandSystem.Config.HotKey;
using RTSCamera.CommandSystem.Logic;
using RTSCamera.CommandSystem.Logic.SubLogic;
using RTSCamera.CommandSystem.Orders;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.MountAndBlade;

namespace RTSCamera.CommandSystem.Config
{
	// Token: 0x02000092 RID: 146
	public class CommandSystemOptionClassFactory
	{
		// Token: 0x06000558 RID: 1368 RVA: 0x0001FCE0 File Offset: 0x0001DEE0
		public static IProvider<AOptionClass> CreateOptionClassProvider(AMenuClassCollection menuClassCollection)
		{
			return ProviderCreator.Create<AOptionClass>(delegate
			{
				FormationColorSubLogicV2 outlineView = Mission.Current.GetMissionBehavior<CommandSystemLogic>().OutlineColorSubLogic;
				FormationColorSubLogicV2 groundMarkerView = Mission.Current.GetMissionBehavior<CommandSystemLogic>().GroundMarkerColorSubLogic;
				OptionClass optionClass = new OptionClass(CommandSystemSubModule.ModuleId, GameTexts.FindText("str_rts_camera_command_system_option_class", null), menuClassCollection);
				OptionCategory optionCategory = new OptionCategory("Command", GameTexts.FindText("str_rts_camera_command_system_command_system_options", null), () => MissionConfigBase<CommandSystemConfig>.Get().IsCommandOptionVisible, delegate(bool b)
				{
					MissionConfigBase<CommandSystemConfig>.Get().IsCommandOptionVisible = b;
				});
				optionCategory.AddOption(new BoolOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_click_to_select_formation", null), GameTexts.FindText("str_rts_camera_command_system_click_to_select_formation_hint", null).SetTextVariable("KeyName", CommandSystemGameKeyCategory.GetKey(GameKeyEnum.SelectFormation).ToSequenceString()), () => MissionConfigBase<CommandSystemConfig>.Get().ClickToSelectFormation, delegate(bool b)
				{
					MissionConfigBase<CommandSystemConfig>.Get().ClickToSelectFormation = b;
					FormationColorSubLogicV2 outlineView3 = outlineView;
					if (outlineView3 != null)
					{
						outlineView3.OnMouseOverEnabledChanged(b);
					}
					FormationColorSubLogicV2 groundMarkerView3 = groundMarkerView;
					if (groundMarkerView3 == null)
					{
						return;
					}
					groundMarkerView3.OnMouseOverEnabledChanged(b);
				}));
				optionCategory.AddOption(new BoolOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_attack_specific_formation", null), GameTexts.FindText("str_rts_camera_command_system_attack_specific_formation_hint", null).SetTextVariable("KeyName", CommandSystemGameKeyCategory.GetKey(GameKeyEnum.SelectFormation).ToSequenceString()), () => MissionConfigBase<CommandSystemConfig>.Get().AttackSpecificFormation, delegate(bool b)
				{
					MissionConfigBase<CommandSystemConfig>.Get().AttackSpecificFormation = b;
					FormationColorSubLogicV2 outlineView2 = outlineView;
					if (outlineView2 != null)
					{
						outlineView2.OnMouseOverEnabledChanged(b);
					}
					FormationColorSubLogicV2 groundMarkerView2 = groundMarkerView;
					if (groundMarkerView2 == null)
					{
						return;
					}
					groundMarkerView2.OnMouseOverEnabledChanged(b);
				}));
				optionCategory.AddOption(new BoolOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_disable_native_attack", null), GameTexts.FindText("str_rts_camera_command_system_disable_native_attack_hint", null).SetTextVariable("KeyName", CommandSystemGameKeyCategory.GetKey(GameKeyEnum.SelectFormation).ToSequenceString()), () => MissionConfigBase<CommandSystemConfig>.Get().DisableNativeAttack, delegate(bool b)
				{
					MissionConfigBase<CommandSystemConfig>.Get().DisableNativeAttack = b;
				}));
				optionCategory.AddOption(new SelectionOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_after_enemy_formation_eliminated", null), GameTexts.FindText("str_rts_camera_command_system_after_enemy_formation_eliminated_hint", null), new SelectionOptionData(delegate(int i)
				{
					MissionConfigBase<CommandSystemConfig>.Get().BehaviorAfterCharge = (BehaviorAfterCharge)i;
				}, () => (int)MissionConfigBase<CommandSystemConfig>.Get().BehaviorAfterCharge, () => 2, () => new List<SelectionItem>
				{
					new SelectionItem(true, "str_rts_camera_command_system_after_charge_behavior", "charge"),
					new SelectionItem(true, "str_rts_camera_command_system_after_charge_behavior", "hold")
				}), false, false));
				optionCategory.AddOption(new SelectionOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_troop_highlight_character_mode", null), GameTexts.FindText("str_rts_camera_command_system_troop_highlight_character_mode_hint", null), new SelectionOptionData(delegate(int i)
				{
					MissionConfigBase<CommandSystemConfig>.Get().TroopHighlightStyleInCharacterMode = (TroopHighlightStyle)i;
				}, () => (int)MissionConfigBase<CommandSystemConfig>.Get().TroopHighlightStyleInCharacterMode, () => 3, () => new List<SelectionItem>
				{
					new SelectionItem(true, "str_rts_camera_command_system_troop_highlight_option", "No"),
					new SelectionItem(true, "str_rts_camera_command_system_troop_highlight_option", "Outline"),
					new SelectionItem(true, "str_rts_camera_command_system_troop_highlight_option", "GroundMarker")
				}), false, false));
				optionCategory.AddOption(new SelectionOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_troop_highlight_rts_mode", null), GameTexts.FindText("str_rts_camera_command_system_troop_highlight_rts_mode_hint", null), new SelectionOptionData(delegate(int i)
				{
					MissionConfigBase<CommandSystemConfig>.Get().TroopHighlightStyleInRTSMode = (TroopHighlightStyle)i;
				}, () => (int)MissionConfigBase<CommandSystemConfig>.Get().TroopHighlightStyleInRTSMode, () => 3, () => new List<SelectionItem>
				{
					new SelectionItem(true, "str_rts_camera_command_system_troop_highlight_option", "No"),
					new SelectionItem(true, "str_rts_camera_command_system_troop_highlight_option", "Outline"),
					new SelectionItem(true, "str_rts_camera_command_system_troop_highlight_option", "GroundMarker")
				}), false, false));
				optionCategory.AddOption(new SelectionOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_highlight_troops_when_showing_indicators", null), GameTexts.FindText("str_rts_camera_command_system_highlight_troops_when_showing_indicators_hint", null), new SelectionOptionData(delegate(int i)
				{
					MissionConfigBase<CommandSystemConfig>.Get().HighlightTroopsWhenShowingIndicators = (ShowMode)i;
				}, () => (int)MissionConfigBase<CommandSystemConfig>.Get().HighlightTroopsWhenShowingIndicators, () => 3, () => new List<SelectionItem>
				{
					new SelectionItem(true, "str_rts_camera_command_system_highlight_troops_when_showing_indicators_option", "Never"),
					new SelectionItem(true, "str_rts_camera_command_system_highlight_troops_when_showing_indicators_option", "FreeCameraOnly"),
					new SelectionItem(true, "str_rts_camera_command_system_highlight_troops_when_showing_indicators_option", "Always")
				}), false, false));
				optionCategory.AddOption(new BoolOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_highlight_troops_without_formation", null), GameTexts.FindText("str_rts_camera_command_system_highlight_troops_without_formation_hint", null), () => MissionConfigBase<CommandSystemConfig>.Get().HighlightTroopsWithoutFormation, delegate(bool b)
				{
					MissionConfigBase<CommandSystemConfig>.Get().HighlightTroopsWithoutFormation = b;
				}));
				optionCategory.AddOption(new SelectionOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_movement_target_highlight_character_mode", null), GameTexts.FindText("str_rts_camera_command_system_movement_target_highlight_character_mode_hint", null), new SelectionOptionData(delegate(int i)
				{
					MissionConfigBase<CommandSystemConfig>.Get().MovementTargetHighlightStyleInCharacterMode = (MovementTargetHighlightStyle)i;
				}, () => (int)MissionConfigBase<CommandSystemConfig>.Get().MovementTargetHighlightStyleInCharacterMode, () => 3, () => new List<SelectionItem>
				{
					new SelectionItem(true, "str_rts_camera_command_system_movement_target_highlight_style_option", "Original"),
					new SelectionItem(true, "str_rts_camera_command_system_movement_target_highlight_style_option", "NewModelOnly"),
					new SelectionItem(true, "str_rts_camera_command_system_movement_target_highlight_style_option", "AlwaysVisible")
				}), false, false));
				optionCategory.AddOption(new SelectionOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_movement_target_highlight_rts_mode", null), GameTexts.FindText("str_rts_camera_command_system_movement_target_highlight_rts_mode_hint", null), new SelectionOptionData(delegate(int i)
				{
					MissionConfigBase<CommandSystemConfig>.Get().MovementTargetHighlightStyleInRTSMode = (MovementTargetHighlightStyle)i;
				}, () => (int)MissionConfigBase<CommandSystemConfig>.Get().MovementTargetHighlightStyleInRTSMode, () => 3, () => new List<SelectionItem>
				{
					new SelectionItem(true, "str_rts_camera_command_system_movement_target_highlight_style_option", "Original"),
					new SelectionItem(true, "str_rts_camera_command_system_movement_target_highlight_style_option", "NewModelOnly"),
					new SelectionItem(true, "str_rts_camera_command_system_movement_target_highlight_style_option", "AlwaysVisible")
				}), false, false));
				optionCategory.AddOption(new NumericOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_movement_target_fade_out_duration", null), GameTexts.FindText("str_rts_camera_command_system_movement_target_fade_out_duration_hint", null), () => MissionConfigBase<CommandSystemConfig>.Get().MovementTargetFadeOutDuration, delegate(float f)
				{
					MissionConfigBase<CommandSystemConfig>.Get().MovementTargetFadeOutDuration = f;
				}, 0f, 2f, false, true));
				optionCategory.AddOption(new SelectionOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_command_queue_flag_show_mode", null), GameTexts.FindText("str_rts_camera_command_system_command_queue_flag_show_mode_hint", null), new SelectionOptionData(delegate(int i)
				{
					MissionConfigBase<CommandSystemConfig>.Get().CommandQueueFlagShowMode = (ShowMode)i;
				}, () => (int)MissionConfigBase<CommandSystemConfig>.Get().CommandQueueFlagShowMode, () => 3, () => new List<SelectionItem>
				{
					new SelectionItem(true, "str_rts_camera_command_system_command_queue_flag_show_mode_option", "Never"),
					new SelectionItem(true, "str_rts_camera_command_system_command_queue_flag_show_mode_option", "FreeCameraOnly"),
					new SelectionItem(true, "str_rts_camera_command_system_command_queue_flag_show_mode_option", "Always")
				}), false, false));
				optionCategory.AddOption(new SelectionOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_command_queue_arrow_show_mode", null), GameTexts.FindText("str_rts_camera_command_system_command_queue_arrow_show_mode_hint", null), new SelectionOptionData(delegate(int i)
				{
					MissionConfigBase<CommandSystemConfig>.Get().CommandQueueArrowShowMode = (ShowMode)i;
				}, () => (int)MissionConfigBase<CommandSystemConfig>.Get().CommandQueueArrowShowMode, () => 3, () => new List<SelectionItem>
				{
					new SelectionItem(true, "str_rts_camera_command_system_command_queue_arrow_show_mode_option", "Never"),
					new SelectionItem(true, "str_rts_camera_command_system_command_queue_arrow_show_mode_option", "FreeCameraOnly"),
					new SelectionItem(true, "str_rts_camera_command_system_command_queue_arrow_show_mode_option", "Always")
				}), false, false));
				optionCategory.AddOption(new SelectionOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_command_queue_formation_shape_show_mode", null), GameTexts.FindText("str_rts_camera_command_system_command_queue_formation_shape_show_mode_hint", null), new SelectionOptionData(delegate(int i)
				{
					MissionConfigBase<CommandSystemConfig>.Get().CommandQueueFormationShapeShowMode = (ShowMode)i;
				}, () => (int)MissionConfigBase<CommandSystemConfig>.Get().CommandQueueFormationShapeShowMode, () => 3, () => new List<SelectionItem>
				{
					new SelectionItem(true, "str_rts_camera_command_system_command_queue_formation_shape_show_mode_option", "Never"),
					new SelectionItem(true, "str_rts_camera_command_system_command_queue_formation_shape_show_mode_option", "FreeCameraOnly"),
					new SelectionItem(true, "str_rts_camera_command_system_command_queue_formation_shape_show_mode_option", "Always")
				}), false, false));
				optionCategory.AddOption(new SelectionOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_formation_lock_condition", null), GameTexts.FindText("str_rts_camera_command_system_formation_lock_condition_hint", null), new SelectionOptionData(delegate(int i)
				{
					MissionConfigBase<CommandSystemConfig>.Get().FormationLockCondition = (FormationLockCondition)i;
				}, () => (int)MissionConfigBase<CommandSystemConfig>.Get().FormationLockCondition, () => 3, () => new List<SelectionItem>
				{
					new SelectionItem(true, "str_rts_camera_command_system_formation_lock_condition_option", "Never"),
					new SelectionItem(true, "str_rts_camera_command_system_formation_lock_condition_option", "WhenPressed"),
					new SelectionItem(true, "str_rts_camera_command_system_formation_lock_condition_option", "WhenNotPressed")
				}), false, false));
				optionCategory.AddOption(new SelectionOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_formation_speed_sync_mode", null), GameTexts.FindText("str_rts_camera_command_system_formation_speed_sync_mode_hint", null), new SelectionOptionData(delegate(int i)
				{
					MissionConfigBase<CommandSystemConfig>.Get().FormationSpeedSyncMode = (FormationSpeedSyncMode)i;
				}, () => (int)MissionConfigBase<CommandSystemConfig>.Get().FormationSpeedSyncMode, () => 4, () => new List<SelectionItem>
				{
					new SelectionItem(true, "str_rts_camera_command_system_formation_speed_sync_mode_option", "Disabled"),
					new SelectionItem(true, "str_rts_camera_command_system_formation_speed_sync_mode_option", "Linear"),
					new SelectionItem(true, "str_rts_camera_command_system_formation_speed_sync_mode_option", "CatchUp"),
					new SelectionItem(true, "str_rts_camera_command_system_formation_speed_sync_mode_option", "WaitForLastFormation")
				}), false, false));
				optionCategory.AddOption(new BoolOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_hollow_square_formation", null), GameTexts.FindText("str_rts_camera_command_system_hollow_square_formation_hint", null), () => MissionConfigBase<CommandSystemConfig>.Get().HollowSquare, delegate(bool b)
				{
					MissionConfigBase<CommandSystemConfig>.Get().HollowSquare = b;
				}));
				optionCategory.AddOption(new BoolOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_square_formation_corner_fix", null), GameTexts.FindText("str_rts_camera_command_system_square_formation_corner_fix_hint", null), () => MissionConfigBase<CommandSystemConfig>.Get().SquareFormationCornerFix, delegate(bool b)
				{
					MissionConfigBase<CommandSystemConfig>.Get().SquareFormationCornerFix = b;
				}));
				optionCategory.AddOption(new SelectionOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_circle_formation_preference", null), GameTexts.FindText("str_rts_camera_command_system_circle_formation_preference_hint", null), new SelectionOptionData(delegate(int i)
				{
					MissionConfigBase<CommandSystemConfig>.Get().CircleFormationUnitSpacingPreference = (CircleFormationUnitSpacingPreference)i;
				}, () => (int)MissionConfigBase<CommandSystemConfig>.Get().CircleFormationUnitSpacingPreference, () => 2, () => new List<SelectionItem>
				{
					new SelectionItem(true, "str_rts_camera_command_system_circle_formation_preference_option", "Tight"),
					new SelectionItem(true, "str_rts_camera_command_system_circle_formation_preference_option", "Loose")
				}), false, false));
				optionCategory.AddOption(new BoolOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_order_ui_clickable", null), GameTexts.FindText("str_rts_camera_command_system_order_ui_clickable_hint", null), () => MissionConfigBase<CommandSystemConfig>.Get().OrderUIClickable, delegate(bool b)
				{
					CommandSystemConfig commandSystemConfig = MissionConfigBase<CommandSystemConfig>.Get();
					UIConfig.DoNotUseGeneratedPrefabs = b;
					commandSystemConfig.OrderUIClickable = b;
				}));
				optionCategory.AddOption(new BoolOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_order_ui_clickable_extension", null), GameTexts.FindText("str_rts_camera_command_system_order_ui_clickable_extension_hint", null).SetTextVariable("KeyName", CommandSystemGameKeyCategory.GetKey(GameKeyEnum.SelectTargetForCommand).ToSequenceString()), () => MissionConfigBase<CommandSystemConfig>.Get().OrderUIClickableExtension, delegate(bool b)
				{
					MissionConfigBase<CommandSystemConfig>.Get().OrderUIClickableExtension = b;
					if (!b)
					{
						RTSCommandVisualOrder.OrderToSelectTarget = SelectTargetMode.None;
					}
				}));
				optionCategory.AddOption(new BoolOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_face_enemy_by_default", null), GameTexts.FindText("str_rts_camera_command_system_face_enemy_by_default_hint", null), () => MissionConfigBase<CommandSystemConfig>.Get().FacingEnemyByDefault, delegate(bool b)
				{
					MissionConfigBase<CommandSystemConfig>.Get().FacingEnemyByDefault = b;
				}));
				optionCategory.AddOption(new NumericOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_mounted_units_interval_threshold", null), GameTexts.FindText("str_rts_camera_command_system_mounted_units_interval_threshold_hint", null), () => MissionConfigBase<CommandSystemConfig>.Get().MountedUnitsIntervalThreshold, delegate(float f)
				{
					MissionConfigBase<CommandSystemConfig>.Get().MountedUnitsIntervalThreshold = f;
				}, 0.01f, 0.5f, false, true));
				optionClass.AddOptionCategory(0, optionCategory);
				OptionCategory optionCategory2 = new OptionCategory("AdvanceOrder", GameTexts.FindText("str_rts_camera_command_system_advance_order_options", null), () => MissionConfigBase<CommandSystemConfig>.Get().IsAdvanceOrderOptionVisible, delegate(bool b)
				{
					MissionConfigBase<CommandSystemConfig>.Get().IsAdvanceOrderOptionVisible = b;
				});
				optionCategory2.AddOption(new BoolOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_fix_advance_order_for_throwing_weapons", null), GameTexts.FindText("str_rts_camera_command_system_fix_advance_order_for_throwing_weapons_hint", null), () => MissionConfigBase<CommandSystemConfig>.Get().FixAdvaneOrderForThrowing, delegate(bool b)
				{
					MissionConfigBase<CommandSystemConfig>.Get().FixAdvaneOrderForThrowing = b;
				}));
				optionCategory2.AddOption(new BoolOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_apply_advance_order_fix_for_ai", null), GameTexts.FindText("str_rts_camera_command_system_apply_advance_order_fix_for_ai_hint", null), () => MissionConfigBase<CommandSystemConfig>.Get().ApplyAdvanceOrderFixForAI, delegate(bool b)
				{
					MissionConfigBase<CommandSystemConfig>.Get().ApplyAdvanceOrderFixForAI = b;
				}));
				optionCategory2.AddOption(new NumericOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_thrower_ratio_threshold", null), GameTexts.FindText("str_rts_camera_command_system_thrower_ratio_threshold_hint", null), () => MissionConfigBase<CommandSystemConfig>.Get().ThrowerRatioThreshold, delegate(float f)
				{
					MissionConfigBase<CommandSystemConfig>.Get().ThrowerRatioThreshold = f;
				}, 0f, 1f, false, true));
				optionCategory2.AddOption(new NumericOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_remaining_ammo_ratio_threshold", null), GameTexts.FindText("str_rts_camera_command_system_remaining_ammo_ratio_threshold_hint", null), () => MissionConfigBase<CommandSystemConfig>.Get().RemainingAmmoRatioThreshold, delegate(float f)
				{
					MissionConfigBase<CommandSystemConfig>.Get().RemainingAmmoRatioThreshold = f;
				}, 0f, 1f, false, true));
				optionCategory2.AddOption(new BoolOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_shorten_range_based_on_remaining_ammo", null), GameTexts.FindText("str_rts_camera_command_system_shorten_range_based_on_remaining_ammo_hint", null), () => MissionConfigBase<CommandSystemConfig>.Get().ShortenRangeBasedOnRemainingAmmo, delegate(bool b)
				{
					MissionConfigBase<CommandSystemConfig>.Get().ShortenRangeBasedOnRemainingAmmo = b;
				}));
				optionClass.AddOptionCategory(1, optionCategory2);
				OptionCategory optionCategory3 = new OptionCategory("VolleyOrder", GameTexts.FindText("str_rts_camera_command_system_volley_order_options", null), () => MissionConfigBase<CommandSystemConfig>.Get().IsVolleyOrderOptionVisible, delegate(bool b)
				{
					MissionConfigBase<CommandSystemConfig>.Get().IsVolleyOrderOptionVisible = b;
				});
				optionCategory3.AddOption(new SelectionOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_volley_pre_aiming_mode", null), GameTexts.FindText("str_rts_camera_command_system_volley_pre_aiming_mode_hint", null), new SelectionOptionData(delegate(int i)
				{
					MissionConfigBase<CommandSystemConfig>.Get().VolleyPreAimingMode = (VolleyPreAimingMode)i;
				}, () => (int)MissionConfigBase<CommandSystemConfig>.Get().VolleyPreAimingMode, () => 2, () => new List<SelectionItem>
				{
					new SelectionItem(true, "str_rts_camera_command_system_volley_pre_aiming_mode_option", "InAutoVolley"),
					new SelectionItem(true, "str_rts_camera_command_system_volley_pre_aiming_mode_option", "BothAutoAndManualVolley")
				}), false, false));
				optionCategory3.AddOption(new NumericOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_ready_ratio_in_auto_volley", null), GameTexts.FindText("str_rts_camera_command_system_ready_ratio_in_auto_volley_hint", null), () => MissionConfigBase<CommandSystemConfig>.Get().ReadyRatioInAutoVolley, delegate(float f)
				{
					MissionConfigBase<CommandSystemConfig>.Get().ReadyRatioInAutoVolley = f;
				}, 0.1f, 1f, false, true));
				optionCategory3.AddOption(new NumericOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_max_aiming_time", null), GameTexts.FindText("str_rts_camera_command_system_max_aiming_time_hint", null), () => MissionConfigBase<CommandSystemConfig>.Get().MaxAimingTime, delegate(float f)
				{
					MissionConfigBase<CommandSystemConfig>.Get().MaxAimingTime = f;
				}, 0.1f, 10f, false, true));
				optionCategory3.AddOption(new BoolOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_auto_volley_by_nonthrown_weapon_type", null), GameTexts.FindText("str_rts_camera_command_system_auto_volley_by_nonthrown_weapon_type_hint", null), () => MissionConfigBase<CommandSystemConfig>.Get().AutoVolleyByWeaponTypeForNonThrown, delegate(bool b)
				{
					MissionConfigBase<CommandSystemConfig>.Get().AutoVolleyByWeaponTypeForNonThrown = b;
				}));
				optionCategory3.AddOption(new BoolOptionViewModel(GameTexts.FindText("str_rts_camera_command_system_auto_volley_by_thrown_weapon_type", null), GameTexts.FindText("str_rts_camera_command_system_auto_volley_by_thrown_weapon_type_hint", null), () => MissionConfigBase<CommandSystemConfig>.Get().AutoVolleyByWeaponTypeForThrown, delegate(bool b)
				{
					MissionConfigBase<CommandSystemConfig>.Get().AutoVolleyByWeaponTypeForThrown = b;
				}));
				optionClass.AddOptionCategory(1, optionCategory3);
				return optionClass;
			}, CommandSystemSubModule.ModuleId, new Version(1, 0, 0));
		}
	}
}
