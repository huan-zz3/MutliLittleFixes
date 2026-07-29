using System;
using HarmonyLib;
using MissionLibrary.Controller;
using MissionLibrary.View;
using MissionSharedLibrary;
using MissionSharedLibrary.Config;
using MissionSharedLibrary.Utilities;
using RTSCamera.CommandSystem.CampaignGame;
using RTSCamera.CommandSystem.Config;
using RTSCamera.CommandSystem.Config.HotKey;
using RTSCamera.CommandSystem.Orders;
using RTSCamera.CommandSystem.Patch;
using RTSCamera.CommandSystem.Usage;
using RTSCamera.CommandSystem.Utilities;
using RTSCameraAgentComponent;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order.Visual;

namespace RTSCamera.CommandSystem
{
	// Token: 0x0200004D RID: 77
	public class CommandSystemSubModule : MBSubModuleBase
	{
		// Token: 0x06000278 RID: 632 RVA: 0x00008D10 File Offset: 0x00006F10
		protected override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();
			CommandSystemSubModule.IsRealisticBattleModuleInstalled = global::MissionSharedLibrary.Utilities.Utility.IsModuleInstalled("RBM") && global::MissionSharedLibrary.Utilities.Utility.IsModuleInstalled("RealisticBattleAiModule");
			global::MissionSharedLibrary.Utilities.Utility.ShouldDisplayMessage = true;
			this.Initialize();
			if (!UIConfig.DoNotUseGeneratedPrefabs && MissionConfigBase<CommandSystemConfig>.Get().OrderUIClickable)
			{
				UIConfig.DoNotUseGeneratedPrefabs = true;
			}
			VisualOrderFactory.RegisterProvider(new RTSCommandVisualOrderProvider());
		}

		// Token: 0x06000279 RID: 633 RVA: 0x00008D70 File Offset: 0x00006F70
		private void Initialize()
		{
			Initializer.Initialize(CommandSystemSubModule.ShortModuleId);
		}

		// Token: 0x0600027A RID: 634 RVA: 0x00008D80 File Offset: 0x00006F80
		protected override void OnBeforeInitialModuleScreenSetAsRoot()
		{
			base.OnBeforeInitialModuleScreenSetAsRoot();
			if (!this.ThirdInitialize())
			{
				return;
			}
			try
			{
				Module.CurrentModule.GlobalTextManager.LoadGameTexts();
			}
			catch (Exception ex)
			{
				MBDebug.Print(ex.ToString(), 0, 12, 17592186044416UL);
				InformationManager.DisplayMessage(new InformationMessage(string.Format("RTS Command: failed to load game texts: {0}", ex)));
			}
			RTSCamera.CommandSystem.Utilities.Utility.PrintOrderHint();
		}

		// Token: 0x0600027B RID: 635 RVA: 0x00008DF4 File Offset: 0x00006FF4
		protected override void OnApplicationTick(float dt)
		{
			base.OnApplicationTick(dt);
			Initializer.OnApplicationTick(dt);
		}

		// Token: 0x0600027C RID: 636 RVA: 0x00008E04 File Offset: 0x00007004
		private bool ThirdInitialize()
		{
			if (!Initializer.ThirdInitialize())
			{
				return false;
			}
			CommandSystemGameKeyCategory.RegisterGameKeyCategory();
			CommandSystemUsageCategory.RegisterUsageCategory();
			AMenuManager.Get().OnMenuClosedEvent += CommandSystemConfig.OnMenuClosed;
			AMenuClassCollection menuClassCollection = AMenuManager.Get().MenuClassCollection;
			menuClassCollection.RegisterItem(CommandSystemOptionClassFactory.CreateOptionClassProvider(menuClassCollection), true);
			AMissionStartingManager amissionStartingManager = AMissionStartingManager.Get();
			amissionStartingManager.AddHandler(new CommandSystemMissionStartingHandler());
			amissionStartingManager.AddSingletonHandler("RTSCameraAgentComponent.MissionStartingHandler", new MissionStartingHandler(), new Version(1, 0, 0));
			this._successPatch = true;
			this._successPatch &= Patch_OrderTroopPlacer.Patch(this._harmony);
			this._successPatch &= Patch_OrderTroopItemVM.Patch(this._harmony);
			this._successPatch &= Patch_MissionOrderTroopControllerVM.Patch(this._harmony);
			this._successPatch &= Patch_OrderController.Patch(this._harmony);
			this._successPatch &= Patch_Formation.Patch(this._harmony);
			this._successPatch &= Patch_MissionOrderVM.Patch(this._harmony);
			this._successPatch &= Patch_GauntletOrderUIHandler.Patch(this._harmony);
			this._successPatch &= Patch_OrderSetVM.Patch(this._harmony);
			this._successPatch &= Patch_ArrangementOrder.Patch(this._harmony);
			this._successPatch &= Patch_SquareFormation.Patch(this._harmony);
			this._successPatch &= Patch_FacingOrder.Patch(this._harmony);
			this._successPatch &= Patch_CircularFormation.Patch(this._harmony);
			this._successPatch &= Patch_HumanAIComponent.Patch(this._harmony);
			this._successPatch &= Patch_MissionGauntletFormationMarker.Patch(this._harmony);
			if (!this._successPatch)
			{
				InformationManager.DisplayMessage(new InformationMessage("RTS Command: patch failed"));
			}
			return true;
		}

		// Token: 0x0600027D RID: 637 RVA: 0x00008FE8 File Offset: 0x000071E8
		protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
		{
			base.OnGameStart(game, gameStarterObject);
			CommandSystemSkillBehavior.CanIssueChargeToFormationOrder = true;
			game.GameTextManager.LoadGameTexts();
			CampaignGameStarter campaignGameStarter = gameStarterObject as CampaignGameStarter;
			if (campaignGameStarter != null)
			{
				campaignGameStarter.AddBehavior(new CommandSystemSkillBehavior());
			}
		}

		// Token: 0x040000F5 RID: 245
		public static readonly string ShortModuleId = "RTSCommand";

		// Token: 0x040000F6 RID: 246
		public static readonly string ModuleId = "RTSCamera.CommandSystem";

		// Token: 0x040000F7 RID: 247
		public static bool IsRealisticBattleModuleInstalled = true;

		// Token: 0x040000F8 RID: 248
		private readonly Harmony _harmony = new Harmony("RTSCommandPatch");

		// Token: 0x040000F9 RID: 249
		private bool _successPatch;
	}
}
