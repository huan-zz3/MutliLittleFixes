using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Bannerlord.UIExtenderEx;
using FormationFilter.CampaignBehaviors;
using FormationFilter.Logics;
using FormationFilter.Patch;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace FormationFilter
{
	// Token: 0x02000006 RID: 6
	[NullableContext(1)]
	[Nullable(0)]
	public class SubModule : MBSubModuleBase
	{
		// Token: 0x06000006 RID: 6 RVA: 0x00002098 File Offset: 0x00000298
		protected override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();
			try
			{
				this._uiExtender = UIExtender.Create("FormationFilter");
				this._uiExtender.Register(typeof(SubModule).Assembly);
				this._uiExtender.Enable();
				this._patchSuccess &= Patch_OrderOfBattleVM.Patch(SubModule.Harmony);
			}
			catch (Exception)
			{
				this._enableUIExtenderFailed = true;
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002114 File Offset: 0x00000314
		protected override void OnSubModuleUnloaded()
		{
			base.OnSubModuleUnloaded();
		}

		// Token: 0x06000008 RID: 8 RVA: 0x0000211C File Offset: 0x0000031C
		protected override void OnBeforeInitialModuleScreenSetAsRoot()
		{
			base.OnBeforeInitialModuleScreenSetAsRoot();
			try
			{
				Module.CurrentModule.GlobalTextManager.LoadGameTexts();
			}
			catch (Exception ex)
			{
				MBDebug.Print(ex.ToString(), 0, 12, 17592186044416UL);
				InformationManager.DisplayMessage(new InformationMessage(string.Format("FormationFilter: failed to load game texts: {0}", ex)));
			}
			if (!this._patchSuccess)
			{
				InformationManager.DisplayMessage(new InformationMessage("FormationFilter: patch failed", new Color(1f, 0f, 0f, 1f)));
			}
			if (this._enableUIExtenderFailed)
			{
				InformationManager.DisplayMessage(new InformationMessage("FormationFilter: enable UIExtender Failed", new Color(1f, 0f, 0f, 1f)));
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000021E0 File Offset: 0x000003E0
		protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
		{
			base.OnGameStart(game, gameStarterObject);
			game.GameTextManager.LoadGameTexts();
			CampaignGameStarter campaignGameStarter = gameStarterObject as CampaignGameStarter;
			if (campaignGameStarter != null)
			{
				campaignGameStarter.AddBehavior(new FormationFilterCampaignBehavior());
			}
			gameStarterObject.AddModel<BattleSpawnModel>(new FormationFilterBattleSpawnModel());
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002220 File Offset: 0x00000420
		public override void OnBeforeMissionBehaviorInitialize(Mission mission)
		{
			base.OnBeforeMissionBehaviorInitialize(mission);
			mission.AddMissionBehavior(new FormationFilterLogic());
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002234 File Offset: 0x00000434
		private T GetGameModel<[Nullable(0)] T>(IGameStarter gameStarter) where T : GameModel
		{
			GameModel[] array = gameStarter.Models.ToArray<GameModel>();
			for (int i = array.Length - 1; i >= 0; i--)
			{
				T t = array[i] as T;
				if (t != null)
				{
					return t;
				}
			}
			return default(T);
		}

		// Token: 0x04000004 RID: 4
		[Nullable(2)]
		private UIExtender _uiExtender;

		// Token: 0x04000005 RID: 5
		private bool _enableUIExtenderFailed;

		// Token: 0x04000006 RID: 6
		private bool _patchSuccess = true;

		// Token: 0x04000007 RID: 7
		public static Harmony Harmony = new Harmony("FormationFilter");
	}
}
