using System;
using NavalDLC.Missions.MissionLogics;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.GauntletUI.Mission;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.ViewModelCollection;

namespace NavalDLC.GauntletUI.MissionViews
{
	// Token: 0x02000016 RID: 22
	[OverrideView(typeof(MissionAgentStatusUIHandler))]
	internal class MissionGauntletNavalAgentStatus : MissionGauntletAgentStatus
	{
		// Token: 0x0600007E RID: 126 RVA: 0x00006213 File Offset: 0x00004413
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._navalShipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Combine(Input.OnGamepadActiveStateChanged, new Action(this.RefreshTexts));
			this.RefreshTexts();
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00006252 File Offset: 0x00004452
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			Input.OnGamepadActiveStateChanged = (Action)Delegate.Remove(Input.OnGamepadActiveStateChanged, new Action(this.RefreshTexts));
		}

		// Token: 0x06000080 RID: 128 RVA: 0x0000627A File Offset: 0x0000447A
		public override void OnMissionScreenTick(float dt)
		{
			base.OnMissionScreenTick(dt);
			MissionAgentStatusVM dataSource = this._dataSource;
			NavalShipsLogic navalShipsLogic = this._navalShipsLogic;
			dataSource.IsAgentStatusPrioritized = ((navalShipsLogic != null) ? navalShipsLogic.PlayerControlledShip : null) == null;
		}

		// Token: 0x06000081 RID: 129 RVA: 0x000062A4 File Offset: 0x000044A4
		private void RefreshTexts()
		{
			this._selectShipText = GameTexts.FindText("str_key_action", null).SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("NavalShipControlsHotKeyCategory", 113), 1f)).SetTextVariable("ACTION", new TextObject("{=QVlyuUu6}Select Ship", null));
			this._attemptBoardingText = GameTexts.FindText("str_key_action", null).SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("NavalShipControlsHotKeyCategory", 114), 1f)).SetTextVariable("ACTION", new TextObject("{=DJA4aQ8n}Attempt Boarding", null));
			this._cancelBoardingText = GameTexts.FindText("str_key_action", null).SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("NavalShipControlsHotKeyCategory", 114), 1f)).SetTextVariable("ACTION", new TextObject("{=0bSBXtCi}Cancel Boarding", null));
			this.SetShipInteractionTexts();
		}

		// Token: 0x06000082 RID: 130 RVA: 0x0000638C File Offset: 0x0000458C
		public void UpdateShipInteractionTexts(IShipOrigin origin, bool isEnemy = false, bool canSelectShip = false, bool canAttemptBoarding = false, bool isBoardingBlocked = false, bool canCancelBoarding = false)
		{
			if (origin == this._focusedShipOrigin && isEnemy == this._focusedShipIsEnemy && canSelectShip == this._canSelectShip && canAttemptBoarding == this._canAttemptBoarding && isBoardingBlocked == this._isBoardingBlocked && canCancelBoarding == this._canCancelBoarding)
			{
				return;
			}
			this._focusedShipOrigin = origin;
			this._focusedShipIsEnemy = isEnemy;
			this._canSelectShip = canSelectShip;
			this._canAttemptBoarding = canAttemptBoarding;
			this._isBoardingBlocked = isBoardingBlocked;
			this._canCancelBoarding = canCancelBoarding;
			this.SetShipInteractionTexts();
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00006408 File Offset: 0x00004608
		private void SetShipInteractionTexts()
		{
			this._dataSource.InteractionInterface.ClearForcedInteractionTexts();
			if (this._focusedShipOrigin != null)
			{
				TextObject textObject = (this._focusedShipIsEnemy ? new TextObject("{=PFqAEWSt}Enemy {SHIP_NAME}", null).SetTextVariable("SHIP_NAME", this._focusedShipOrigin.Hull.Name) : this._focusedShipOrigin.Name);
				TextObject textObject2 = null;
				bool flag = false;
				if (this._canSelectShip)
				{
					textObject2 = this._selectShipText;
				}
				else if (this._canAttemptBoarding)
				{
					if (this._canCancelBoarding)
					{
						textObject2 = this._cancelBoardingText;
					}
					else
					{
						textObject2 = this._attemptBoardingText;
						flag = this._isBoardingBlocked;
					}
				}
				this._dataSource.InteractionInterface.SetForcedInteractionTexts(textObject, false, textObject2, flag);
				return;
			}
			NavalShipsLogic navalShipsLogic = this._navalShipsLogic;
			if (((navalShipsLogic != null) ? navalShipsLogic.PlayerControlledShip : null) != null)
			{
				this._dataSource.InteractionInterface.SetForcedInteractionTexts(TextObject.GetEmpty(), false, TextObject.GetEmpty(), false);
			}
		}

		// Token: 0x04000043 RID: 67
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x04000044 RID: 68
		private TextObject _selectShipText;

		// Token: 0x04000045 RID: 69
		private TextObject _attemptBoardingText;

		// Token: 0x04000046 RID: 70
		private TextObject _cancelBoardingText;

		// Token: 0x04000047 RID: 71
		private IShipOrigin _focusedShipOrigin;

		// Token: 0x04000048 RID: 72
		private bool _focusedShipIsEnemy;

		// Token: 0x04000049 RID: 73
		private bool _canSelectShip;

		// Token: 0x0400004A RID: 74
		private bool _canAttemptBoarding;

		// Token: 0x0400004B RID: 75
		private bool _isBoardingBlocked;

		// Token: 0x0400004C RID: 76
		private bool _canCancelBoarding;
	}
}
