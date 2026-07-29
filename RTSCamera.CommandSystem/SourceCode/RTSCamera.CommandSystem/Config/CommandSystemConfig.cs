using System;
using System.IO;
using System.Xml.Serialization;
using MissionSharedLibrary.Config;
using MissionSharedLibrary.Utilities;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace RTSCamera.CommandSystem.Config
{
	// Token: 0x02000090 RID: 144
	public class CommandSystemConfig : MissionConfigBase<CommandSystemConfig>
	{
		// Token: 0x170000CA RID: 202
		// (get) Token: 0x0600054E RID: 1358 RVA: 0x0001F83C File Offset: 0x0001DA3C
		protected override XmlSerializer Serializer
		{
			get
			{
				return new XmlSerializer(typeof(CommandSystemConfig));
			}
		}

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x0600054F RID: 1359 RVA: 0x0001F84D File Offset: 0x0001DA4D
		protected static Version BinaryVersion
		{
			get
			{
				return new Version(1, 3);
			}
		}

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x06000550 RID: 1360 RVA: 0x0001F856 File Offset: 0x0001DA56
		// (set) Token: 0x06000551 RID: 1361 RVA: 0x0001F85E File Offset: 0x0001DA5E
		public string ConfigVersion { get; set; } = CommandSystemConfig.BinaryVersion.ToString();

		// Token: 0x06000552 RID: 1362 RVA: 0x0001F868 File Offset: 0x0001DA68
		protected override void CopyFrom(CommandSystemConfig other)
		{
			this.ConfigVersion = other.ConfigVersion;
			this.ClickToSelectFormation = other.ClickToSelectFormation;
			this.AttackSpecificFormation = other.AttackSpecificFormation;
			this.DisableNativeAttack = other.DisableNativeAttack;
			this.BehaviorAfterCharge = other.BehaviorAfterCharge;
			this.TroopHighlightStyleInCharacterMode = other.TroopHighlightStyleInCharacterMode;
			this.TroopHighlightStyleInRTSMode = other.TroopHighlightStyleInRTSMode;
			this.HighlightTroopsWhenShowingIndicators = other.HighlightTroopsWhenShowingIndicators;
			this.HighlightTroopsWithoutFormation = other.HighlightTroopsWithoutFormation;
			this.MovementTargetHighlightStyleInCharacterMode = other.MovementTargetHighlightStyleInCharacterMode;
			this.MovementTargetHighlightStyleInRTSMode = other.MovementTargetHighlightStyleInRTSMode;
			this.MovementTargetHighlightMode = other.MovementTargetHighlightMode;
			this.MoreVisibleMovementTarget = other.MoreVisibleMovementTarget;
			this.MovementTargetFadeOutDuration = other.MovementTargetFadeOutDuration;
			this.MovementTargetMoreVisibleOnRtsViewOnly = other.MovementTargetMoreVisibleOnRtsViewOnly;
			this.CommandQueueFlagShowMode = other.CommandQueueFlagShowMode;
			this.CommandQueueArrowShowMode = other.CommandQueueArrowShowMode;
			this.CommandQueueFormationShapeShowMode = other.CommandQueueFormationShapeShowMode;
			this.FormationLockCondition = other.FormationLockCondition;
			this.FormationSpeedSyncMode = other.FormationSpeedSyncMode;
			this.HasHintDisplayed = other.HasHintDisplayed;
			this.HollowSquare = other.HollowSquare;
			this.SquareFormationCornerFix = other.SquareFormationCornerFix;
			this.OrderUIClickable = other.OrderUIClickable;
			this.OrderUIClickableExtension = other.OrderUIClickableExtension;
			this.FacingEnemyByDefault = other.FacingEnemyByDefault;
			this.CircleFormationUnitSpacingPreference = other.CircleFormationUnitSpacingPreference;
			this.MountedUnitsIntervalThreshold = other.MountedUnitsIntervalThreshold;
			this.FixAdvaneOrderForThrowing = other.FixAdvaneOrderForThrowing;
			this.ApplyAdvanceOrderFixForAI = other.ApplyAdvanceOrderFixForAI;
			this.ThrowerRatioThreshold = other.ThrowerRatioThreshold;
			this.RemainingAmmoRatioThreshold = other.RemainingAmmoRatioThreshold;
			this.ShortenRangeBasedOnRemainingAmmo = other.ShortenRangeBasedOnRemainingAmmo;
			this.VolleyPreAimingMode = other.VolleyPreAimingMode;
			this.ReadyRatioInAutoVolley = other.ReadyRatioInAutoVolley;
			this.MaxAimingTime = other.MaxAimingTime;
			this.AutoVolleyByWeaponTypeForNonThrown = other.AutoVolleyByWeaponTypeForNonThrown;
			this.AutoVolleyByWeaponTypeForThrown = other.AutoVolleyByWeaponTypeForThrown;
			this.IsCommandOptionVisible = other.IsCommandOptionVisible;
			this.IsAdvanceOrderOptionVisible = other.IsAdvanceOrderOptionVisible;
			this.IsVolleyOrderOptionVisible = other.IsVolleyOrderOptionVisible;
		}

		// Token: 0x06000553 RID: 1363 RVA: 0x0001FA61 File Offset: 0x0001DC61
		public static void OnMenuClosed()
		{
			MissionConfigBase<CommandSystemConfig>.Get().Serialize();
		}

		// Token: 0x06000554 RID: 1364 RVA: 0x0001FA70 File Offset: 0x0001DC70
		protected override void UpgradeToCurrentVersion()
		{
			string configVersion = this.ConfigVersion;
			if (!(configVersion == "1.0"))
			{
				if (!(configVersion == "1.1"))
				{
					if (configVersion == "1.2")
					{
						goto IL_00C4;
					}
					if (configVersion == "1.3")
					{
						this.ConfigVersion = "1.3";
						return;
					}
					Utility.DisplayMessage(Module.CurrentModule.GlobalTextManager.FindText("str_mission_library_config_incompatible", null).ToString(), new Color(1f, 0f, 0f, 1f));
					base.ResetToDefault();
					this.Serialize();
				}
			}
			else if (this.MoreVisibleMovementTarget)
			{
				if (this.MovementTargetMoreVisibleOnRtsViewOnly)
				{
					this.MovementTargetHighlightMode = MovementTargetHighlightMode.FreeCameraOnly;
				}
				else
				{
					this.MovementTargetHighlightMode = MovementTargetHighlightMode.Always;
				}
			}
			else
			{
				this.MovementTargetHighlightMode = MovementTargetHighlightMode.Never;
			}
			if (this.MovementTargetHighlightMode == MovementTargetHighlightMode.FreeCameraOnly)
			{
				this.MovementTargetHighlightMode = MovementTargetHighlightMode.NightOrFreeCamera;
			}
			IL_00C4:
			switch (this.MovementTargetHighlightMode)
			{
			case MovementTargetHighlightMode.Never:
				this.MovementTargetHighlightStyleInCharacterMode = MovementTargetHighlightStyle.Original;
				this.MovementTargetHighlightStyleInRTSMode = MovementTargetHighlightStyle.Original;
				return;
			case MovementTargetHighlightMode.FreeCameraOnly:
				this.MovementTargetHighlightStyleInCharacterMode = MovementTargetHighlightStyle.Original;
				this.MovementTargetHighlightStyleInRTSMode = MovementTargetHighlightStyle.AlwaysVisible;
				return;
			case MovementTargetHighlightMode.NightOrFreeCamera:
				return;
			case MovementTargetHighlightMode.Always:
				this.MovementTargetHighlightStyleInCharacterMode = MovementTargetHighlightStyle.NewModelOnly;
				this.MovementTargetHighlightStyleInRTSMode = MovementTargetHighlightStyle.AlwaysVisible;
				return;
			default:
				return;
			}
		}

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x06000555 RID: 1365 RVA: 0x0001FB97 File Offset: 0x0001DD97
		[XmlIgnore]
		protected override string SaveName
		{
			get
			{
				return Path.Combine(ConfigPath.ConfigDir, "RTSCamera", "CommandSystemConfig.xml");
			}
		}

		// Token: 0x04000267 RID: 615
		public bool ClickToSelectFormation = true;

		// Token: 0x04000268 RID: 616
		public bool AttackSpecificFormation = true;

		// Token: 0x04000269 RID: 617
		public bool DisableNativeAttack;

		// Token: 0x0400026A RID: 618
		public BehaviorAfterCharge BehaviorAfterCharge = ((!CommandSystemSubModule.IsRealisticBattleModuleInstalled) ? BehaviorAfterCharge.Hold : BehaviorAfterCharge.Charge);

		// Token: 0x0400026B RID: 619
		public TroopHighlightStyle TroopHighlightStyleInCharacterMode = TroopHighlightStyle.GroundMarker;

		// Token: 0x0400026C RID: 620
		public TroopHighlightStyle TroopHighlightStyleInRTSMode = TroopHighlightStyle.GroundMarker;

		// Token: 0x0400026D RID: 621
		public ShowMode HighlightTroopsWhenShowingIndicators = ShowMode.Always;

		// Token: 0x0400026E RID: 622
		public bool HighlightTroopsWithoutFormation;

		// Token: 0x0400026F RID: 623
		public MovementTargetHighlightStyle MovementTargetHighlightStyleInCharacterMode = MovementTargetHighlightStyle.NewModelOnly;

		// Token: 0x04000270 RID: 624
		public MovementTargetHighlightStyle MovementTargetHighlightStyleInRTSMode = MovementTargetHighlightStyle.AlwaysVisible;

		// Token: 0x04000271 RID: 625
		public MovementTargetHighlightMode MovementTargetHighlightMode = MovementTargetHighlightMode.Always;

		// Token: 0x04000272 RID: 626
		public bool MoreVisibleMovementTarget = true;

		// Token: 0x04000273 RID: 627
		public float MovementTargetFadeOutDuration = 0.5f;

		// Token: 0x04000274 RID: 628
		public bool MovementTargetMoreVisibleOnRtsViewOnly = true;

		// Token: 0x04000275 RID: 629
		public ShowMode CommandQueueFlagShowMode = ShowMode.FreeCameraOnly;

		// Token: 0x04000276 RID: 630
		public ShowMode CommandQueueArrowShowMode = ShowMode.FreeCameraOnly;

		// Token: 0x04000277 RID: 631
		public ShowMode CommandQueueFormationShapeShowMode = ShowMode.Always;

		// Token: 0x04000278 RID: 632
		public FormationLockCondition FormationLockCondition = FormationLockCondition.WhenNotPressed;

		// Token: 0x04000279 RID: 633
		public FormationSpeedSyncMode FormationSpeedSyncMode = FormationSpeedSyncMode.WaitForLastFormation;

		// Token: 0x0400027A RID: 634
		public bool HasHintDisplayed;

		// Token: 0x0400027B RID: 635
		public bool HollowSquare = true;

		// Token: 0x0400027C RID: 636
		public bool SquareFormationCornerFix = true;

		// Token: 0x0400027D RID: 637
		public bool OrderUIClickable = true;

		// Token: 0x0400027E RID: 638
		public bool OrderUIClickableExtension;

		// Token: 0x0400027F RID: 639
		public bool FacingEnemyByDefault;

		// Token: 0x04000280 RID: 640
		public CircleFormationUnitSpacingPreference CircleFormationUnitSpacingPreference;

		// Token: 0x04000281 RID: 641
		public float MountedUnitsIntervalThreshold = 0.1f;

		// Token: 0x04000282 RID: 642
		public bool FixAdvaneOrderForThrowing = true;

		// Token: 0x04000283 RID: 643
		public bool ApplyAdvanceOrderFixForAI;

		// Token: 0x04000284 RID: 644
		public float ThrowerRatioThreshold = 0.5f;

		// Token: 0x04000285 RID: 645
		public float RemainingAmmoRatioThreshold = 0.1f;

		// Token: 0x04000286 RID: 646
		public bool ShortenRangeBasedOnRemainingAmmo;

		// Token: 0x04000287 RID: 647
		public VolleyPreAimingMode VolleyPreAimingMode = VolleyPreAimingMode.BothAutoAndManualVolley;

		// Token: 0x04000288 RID: 648
		public float ReadyRatioInAutoVolley = 0.8f;

		// Token: 0x04000289 RID: 649
		public float MaxAimingTime = 1.5f;

		// Token: 0x0400028A RID: 650
		public bool AutoVolleyByWeaponTypeForNonThrown = true;

		// Token: 0x0400028B RID: 651
		public bool AutoVolleyByWeaponTypeForThrown;

		// Token: 0x0400028C RID: 652
		public bool IsCommandOptionVisible = true;

		// Token: 0x0400028D RID: 653
		public bool IsAdvanceOrderOptionVisible = true;

		// Token: 0x0400028E RID: 654
		public bool IsVolleyOrderOptionVisible = true;
	}
}
