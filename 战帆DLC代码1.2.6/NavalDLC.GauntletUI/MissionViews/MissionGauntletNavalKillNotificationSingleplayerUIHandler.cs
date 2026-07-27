using System;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.GauntletUI.Mission.Singleplayer;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews.Singleplayer;

namespace NavalDLC.GauntletUI.MissionViews
{
	// Token: 0x02000018 RID: 24
	[OverrideView(typeof(MissionSingleplayerKillNotificationUIHandler))]
	internal class MissionGauntletNavalKillNotificationSingleplayerUIHandler : MissionGauntletKillNotificationSingleplayerUIHandler
	{
		// Token: 0x0600008D RID: 141 RVA: 0x00006665 File Offset: 0x00004865
		public override void OnMissionScreenInitialize()
		{
			base.OnMissionScreenInitialize();
			this._navalShipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
			if (this._navalShipsLogic != null)
			{
				this._navalShipsLogic.ShipRammingEvent += this.OnShipRamming;
			}
		}

		// Token: 0x0600008E RID: 142 RVA: 0x0000669D File Offset: 0x0000489D
		public override void OnMissionScreenFinalize()
		{
			base.OnMissionScreenFinalize();
			if (this._navalShipsLogic != null)
			{
				this._navalShipsLogic.ShipRammingEvent -= this.OnShipRamming;
			}
		}

		// Token: 0x0600008F RID: 143 RVA: 0x000066C4 File Offset: 0x000048C4
		private void OnShipRamming(MissionShip rammingShip, MissionShip rammedShip, float damagePercent, bool isFirstImpact, CapsuleData capsuleData, int ramQuality)
		{
			if (this._isPersonalFeedEnabled && this._dataSource != null && isFirstImpact && damagePercent > 0f && rammingShip != null && rammedShip != null && rammingShip.IsPlayerShip && rammingShip.CanDealDamage(rammedShip))
			{
				string text;
				switch (ramQuality)
				{
				case 1:
					text = new TextObject("{=P49bHPbv}Ineffective Ram!", null).ToString();
					break;
				case 2:
					text = new TextObject("{=SdAhadD3}Weak Ram!", null).ToString();
					break;
				case 3:
					text = new TextObject("{=CbaYmAuR}Average Ram!", null).ToString();
					break;
				case 4:
					text = new TextObject("{=GaCMFRjH}Good Ram!", null).ToString();
					break;
				case 5:
					text = new TextObject("{=DKukCkai}Excellent Ram!", null).ToString();
					break;
				default:
					Debug.FailedAssert("Ram quality is out of bounds!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.GauntletUI\\MissionViews\\MissionGauntletNavalKillNotificationSingleplayerUIHandler.cs", "OnShipRamming", 70);
					text = new TextObject("{=CbaYmAuR}Average Ram!", null).ToString();
					break;
				}
				this._dataSource.OnPersonalMessage(text);
			}
		}

		// Token: 0x04000050 RID: 80
		private NavalShipsLogic _navalShipsLogic;
	}
}
