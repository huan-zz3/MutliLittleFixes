using System;
using System.Collections.Generic;
using NavalDLC.Missions.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace SandBox.GameComponents
{
	// Token: 0x0200000A RID: 10
	public class NavalBattleInitializationModel : BattleInitializationModel
	{
		// Token: 0x06000054 RID: 84 RVA: 0x00003D75 File Offset: 0x00001F75
		public override List<FormationClass> GetAllAvailableTroopTypes()
		{
			return base.BaseModel.GetAllAvailableTroopTypes();
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00003D84 File Offset: 0x00001F84
		protected override bool CanPlayerSideDeployWithOrderOfBattleAux()
		{
			if (Mission.Current.IsSallyOutBattle)
			{
				return false;
			}
			MapEvent playerMapEvent = MapEvent.PlayerMapEvent;
			if (MapEvent.PlayerMapEvent == null)
			{
				return false;
			}
			PartyBase leaderParty = playerMapEvent.GetLeaderParty(playerMapEvent.PlayerSide);
			if (leaderParty != PartyBase.MainParty && (!leaderParty.IsSettlement || leaderParty.Settlement.OwnerClan.Leader != Hero.MainHero) && !playerMapEvent.IsPlayerSergeant())
			{
				return false;
			}
			IMissionAgentSpawnLogic missionBehavior = Mission.Current.GetMissionBehavior<IMissionAgentSpawnLogic>();
			INavalMissionAgentSpawnLogic navalMissionAgentSpawnLogic;
			if ((navalMissionAgentSpawnLogic = missionBehavior as INavalMissionAgentSpawnLogic) != null)
			{
				return navalMissionAgentSpawnLogic.DeployablePlayerShipCount > 1;
			}
			return missionBehavior.GetNumberOfPlayerControllableTroops() >= 20;
		}
	}
}
