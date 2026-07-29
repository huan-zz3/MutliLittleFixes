using System;
using System.Collections.Generic;
using MissionLibrary.Repository;
using MissionLibrary.Usage;
using MissionSharedLibrary.Usage;
using RTSCamera.CommandSystem.Config.HotKey;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace RTSCamera.CommandSystem.Usage
{
	// Token: 0x02000055 RID: 85
	public class CommandSystemUsageCategory
	{
		// Token: 0x1700008B RID: 139
		// (get) Token: 0x060002FF RID: 767 RVA: 0x0000D2B8 File Offset: 0x0000B4B8
		public static AUsageCategory Category
		{
			get
			{
				return ARepository<AUsageCategoryManager, AUsageCategory>.Get().GetItem("CommandSystemUsage");
			}
		}

		// Token: 0x06000300 RID: 768 RVA: 0x0000D2C9 File Offset: 0x0000B4C9
		public static void RegisterUsageCategory()
		{
			AUsageCategoryManager ausageCategoryManager = ARepository<AUsageCategoryManager, AUsageCategory>.Get();
			if (ausageCategoryManager == null)
			{
				return;
			}
			ausageCategoryManager.RegisterItem<AUsageCategoryManager, AUsageCategory>(new Func<AUsageCategory>(CommandSystemUsageCategory.CreateCategory), "CommandSystemUsage", new Version(1, 0), true);
		}

		// Token: 0x06000301 RID: 769 RVA: 0x0000D2F4 File Offset: 0x0000B4F4
		public static UsageCategory CreateCategory()
		{
			UsageCategoryData usageCategoryData = new UsageCategoryData(GameTexts.FindText("str_rts_camera_command_system_option_class", null), new List<TextObject>
			{
				GameTexts.FindText("str_rts_camera_command_system_order_queue_usage", null).SetTextVariable("KeyName", CommandSystemGameKeyCategory.GetKey(GameKeyEnum.CommandQueue).ToSequenceString()),
				GameTexts.FindText("str_rts_camera_command_system_lock_formation_usage", null),
				GameTexts.FindText("str_rts_camera_command_system_lock_formation_width_usage", null).SetTextVariable("KeyName", CommandSystemGameKeyCategory.GetKey(GameKeyEnum.KeepFormationWidth).ToSequenceString()),
				GameTexts.FindText("str_rts_camera_command_system_toggle_locking_usage", null).SetTextVariable("KeyName", CommandSystemGameKeyCategory.GetKey(GameKeyEnum.FormationLockMovement).ToSequenceString()),
				GameTexts.FindText("str_rts_camera_command_system_attack_specific_formation_hint", null).SetTextVariable("KeyName", CommandSystemGameKeyCategory.GetKey(GameKeyEnum.SelectFormation).ToSequenceString()),
				GameTexts.FindText("str_rts_camera_command_system_attack_specific_formation_alt_hint", null),
				GameTexts.FindText("str_rts_camera_command_system_target_only_usage", null),
				GameTexts.FindText("str_rts_camera_command_system_click_troop_card_usage", null)
			});
			return new UsageCategory("CommandSystemUsage", usageCategoryData);
		}

		// Token: 0x04000130 RID: 304
		public const string CategoryId = "CommandSystemUsage";
	}
}
