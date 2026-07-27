using System;
using System.Runtime.CompilerServices;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000139 RID: 313
	public class NavalDLCTournamentModel : TournamentModel
	{
		// Token: 0x06001524 RID: 5412 RVA: 0x00094F6C File Offset: 0x0009316C
		public override MBList<ItemObject> GetEliteRewardItems(Town town, int regularRewardMinValue, int regularRewardMaxValue)
		{
			MBList<ItemObject> eliteRewardItems = base.BaseModel.GetEliteRewardItems(town, regularRewardMinValue, regularRewardMaxValue);
			foreach (string text in new string[] { "head_breaker_2haxe", "world_chopper__1haxe" })
			{
				ItemObject @object = Game.Current.ObjectManager.GetObject<ItemObject>(text);
				if (@object != null)
				{
					eliteRewardItems.Add(@object);
				}
			}
			return eliteRewardItems;
		}

		// Token: 0x06001525 RID: 5413 RVA: 0x00094FD0 File Offset: 0x000931D0
		public override MBList<ItemObject> GetRegularRewardItems(Town town, int regularRewardMinValue, int regularRewardMaxValue)
		{
			return base.BaseModel.GetRegularRewardItems(town, regularRewardMinValue, regularRewardMaxValue);
		}

		// Token: 0x06001526 RID: 5414 RVA: 0x00094FE0 File Offset: 0x000931E0
		public override TournamentGame CreateTournament(Town town)
		{
			return base.BaseModel.CreateTournament(town);
		}

		// Token: 0x06001527 RID: 5415 RVA: 0x00094FEE File Offset: 0x000931EE
		public override int GetInfluenceReward(Hero winner, Town town)
		{
			return base.BaseModel.GetInfluenceReward(winner, town);
		}

		// Token: 0x06001528 RID: 5416 RVA: 0x00094FFD File Offset: 0x000931FD
		public override int GetNumLeaderboardVictoriesAtGameStart()
		{
			return base.BaseModel.GetNumLeaderboardVictoriesAtGameStart();
		}

		// Token: 0x06001529 RID: 5417 RVA: 0x0009500A File Offset: 0x0009320A
		public override Equipment GetParticipantArmor(CharacterObject participant)
		{
			return base.BaseModel.GetParticipantArmor(participant);
		}

		// Token: 0x0600152A RID: 5418 RVA: 0x00095018 File Offset: 0x00093218
		public override int GetRenownReward(Hero winner, Town town)
		{
			return base.BaseModel.GetRenownReward(winner, town);
		}

		// Token: 0x0600152B RID: 5419 RVA: 0x00095027 File Offset: 0x00093227
		[return: TupleElementNames(new string[] { "skill", "xp" })]
		public override ValueTuple<SkillObject, int> GetSkillXpGainFromTournament(Town town)
		{
			return base.BaseModel.GetSkillXpGainFromTournament(town);
		}

		// Token: 0x0600152C RID: 5420 RVA: 0x00095035 File Offset: 0x00093235
		public override float GetTournamentEndChance(TournamentGame tournament)
		{
			return base.BaseModel.GetTournamentEndChance(tournament);
		}

		// Token: 0x0600152D RID: 5421 RVA: 0x00095043 File Offset: 0x00093243
		public override float GetTournamentSimulationScore(CharacterObject character)
		{
			return base.BaseModel.GetTournamentSimulationScore(character);
		}

		// Token: 0x0600152E RID: 5422 RVA: 0x00095051 File Offset: 0x00093251
		public override float GetTournamentStartChance(Town town)
		{
			return base.BaseModel.GetTournamentStartChance(town);
		}
	}
}
