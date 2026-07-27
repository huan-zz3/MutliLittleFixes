using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000110 RID: 272
	public class NavalDLCClanFinanceModel : ClanFinanceModel
	{
		// Token: 0x17000357 RID: 855
		// (get) Token: 0x060013A3 RID: 5027 RVA: 0x0008DF8A File Offset: 0x0008C18A
		public override int PartyGoldLowerThreshold
		{
			get
			{
				return base.BaseModel.PartyGoldLowerThreshold;
			}
		}

		// Token: 0x060013A4 RID: 5028 RVA: 0x0008DF98 File Offset: 0x0008C198
		public override ExplainedNumber CalculateClanGoldChange(Clan clan, bool includeDescriptions = false, bool applyWithdrawals = false, bool includeDetails = false)
		{
			ExplainedNumber explainedNumber = base.BaseModel.CalculateClanGoldChange(clan, includeDescriptions, applyWithdrawals, includeDetails);
			if (clan.Kingdom != null && clan.Kingdom.HasPolicy(NavalPolicies.CoastalGuardEdict))
			{
				ExplainedNumber explainedNumber2;
				explainedNumber2..ctor(0f, false, null);
				foreach (Town town in clan.Fiefs)
				{
					if (town.Settlement.HasPort && town.GarrisonParty != null && town.GarrisonParty.IsActive)
					{
						int num = this.AddPartyExpense(town.GarrisonParty, clan, explainedNumber, applyWithdrawals);
						explainedNumber2.Add((float)num, null, null);
					}
				}
				explainedNumber.Add(explainedNumber2.ResultNumber * -0.15f, NavalPolicies.CoastalGuardEdict.Name, null);
			}
			return explainedNumber;
		}

		// Token: 0x060013A5 RID: 5029 RVA: 0x0008E084 File Offset: 0x0008C284
		public override ExplainedNumber CalculateClanIncome(Clan clan, bool includeDescriptions = false, bool applyWithdrawals = false, bool includeDetails = false)
		{
			return base.BaseModel.CalculateClanIncome(clan, includeDescriptions, applyWithdrawals, includeDetails);
		}

		// Token: 0x060013A6 RID: 5030 RVA: 0x0008E098 File Offset: 0x0008C298
		public override ExplainedNumber CalculateClanExpenses(Clan clan, bool includeDescriptions = false, bool applyWithdrawals = false, bool includeDetails = false)
		{
			ExplainedNumber explainedNumber = base.BaseModel.CalculateClanExpenses(clan, includeDescriptions, applyWithdrawals, includeDetails);
			if (clan.Kingdom != null && clan.Kingdom.HasPolicy(NavalPolicies.CoastalGuardEdict))
			{
				ExplainedNumber explainedNumber2;
				explainedNumber2..ctor(0f, false, null);
				foreach (Town town in clan.Fiefs)
				{
					if (town.Settlement.HasPort && town.GarrisonParty != null && town.GarrisonParty.IsActive)
					{
						int num = this.AddPartyExpense(town.GarrisonParty, clan, explainedNumber, applyWithdrawals);
						explainedNumber2.Add((float)num, null, null);
					}
				}
				explainedNumber.Add(explainedNumber2.ResultNumber * 0.15f, NavalPolicies.CoastalGuardEdict.Name, null);
			}
			return explainedNumber;
		}

		// Token: 0x060013A7 RID: 5031 RVA: 0x0008E184 File Offset: 0x0008C384
		public override ExplainedNumber CalculateTownIncomeFromTariffs(Clan clan, Town town, bool applyWithdrawals = false)
		{
			ExplainedNumber explainedNumber = base.BaseModel.CalculateTownIncomeFromTariffs(clan, town, applyWithdrawals);
			if (clan.Kingdom != null && clan.Kingdom.HasPolicy(NavalPolicies.ArsenalDepositoryAct))
			{
				explainedNumber.AddFactor(-0.1f, NavalPolicies.ArsenalDepositoryAct.Name);
			}
			return explainedNumber;
		}

		// Token: 0x060013A8 RID: 5032 RVA: 0x0008E1D1 File Offset: 0x0008C3D1
		public override int CalculateTownIncomeFromProjects(Town town)
		{
			return base.BaseModel.CalculateTownIncomeFromProjects(town);
		}

		// Token: 0x060013A9 RID: 5033 RVA: 0x0008E1DF File Offset: 0x0008C3DF
		public override int CalculateNotableDailyGoldChange(Hero hero, bool applyWithdrawals)
		{
			return base.BaseModel.CalculateNotableDailyGoldChange(hero, applyWithdrawals);
		}

		// Token: 0x060013AA RID: 5034 RVA: 0x0008E1EE File Offset: 0x0008C3EE
		public override int CalculateVillageIncome(Clan clan, Village village, bool applyWithdrawals = false)
		{
			return base.BaseModel.CalculateVillageIncome(clan, village, applyWithdrawals);
		}

		// Token: 0x060013AB RID: 5035 RVA: 0x0008E1FE File Offset: 0x0008C3FE
		public override int CalculateOwnerIncomeFromCaravan(MobileParty caravan)
		{
			return base.BaseModel.CalculateOwnerIncomeFromCaravan(caravan);
		}

		// Token: 0x060013AC RID: 5036 RVA: 0x0008E20C File Offset: 0x0008C40C
		public override int CalculateOwnerIncomeFromWorkshop(Workshop workshop)
		{
			return base.BaseModel.CalculateOwnerIncomeFromWorkshop(workshop);
		}

		// Token: 0x060013AD RID: 5037 RVA: 0x0008E21A File Offset: 0x0008C41A
		public override float RevenueSmoothenFraction()
		{
			return base.BaseModel.RevenueSmoothenFraction();
		}

		// Token: 0x060013AE RID: 5038 RVA: 0x0008E228 File Offset: 0x0008C428
		private int AddPartyExpense(MobileParty party, Clan clan, ExplainedNumber goldChange, bool applyWithdrawals)
		{
			int num = clan.Gold + (int)goldChange.ResultNumber;
			int num2 = num;
			if (num < (party.IsGarrison ? 8000 : 4000) && applyWithdrawals && clan != Clan.PlayerClan)
			{
				num2 = ((party.LeaderHero != null && party.PartyTradeGold < 500) ? MathF.Min(num, 250) : 0);
			}
			int num3 = NavalDLCClanFinanceModel.CalculatePartyWage(party, num2, applyWithdrawals);
			int num4 = party.PartyTradeGold;
			if (applyWithdrawals)
			{
				if (party.IsLordParty && party.LeaderHero == null)
				{
					party.ActualClan.Leader.Gold -= num3;
				}
				else
				{
					party.PartyTradeGold -= num3;
				}
			}
			num4 -= num3;
			if (num4 < this.PartyGoldLowerThreshold)
			{
				int num5 = this.PartyGoldLowerThreshold - num4;
				if (party.IsLordParty && party.LeaderHero == null)
				{
					num5 = num3;
				}
				if (applyWithdrawals)
				{
					num5 = MathF.Min(num5, num2);
					party.PartyTradeGold += num5;
				}
				return -num5;
			}
			return 0;
		}

		// Token: 0x060013AF RID: 5039 RVA: 0x0008E328 File Offset: 0x0008C528
		private static int CalculatePartyWage(MobileParty mobileParty, int budget, bool applyWithdrawals)
		{
			int totalWage = mobileParty.TotalWage;
			int num = totalWage;
			if (applyWithdrawals)
			{
				num = MathF.Min(totalWage, budget);
				NavalDLCClanFinanceModel.ApplyMoraleEffect(mobileParty, totalWage, num);
			}
			return num;
		}

		// Token: 0x060013B0 RID: 5040 RVA: 0x0008E354 File Offset: 0x0008C554
		private static void ApplyMoraleEffect(MobileParty mobileParty, int wage, int paymentAmount)
		{
			if (paymentAmount < wage && wage > 0)
			{
				float num = 1f - (float)paymentAmount / (float)wage;
				float num2 = (float)Campaign.Current.Models.PartyMoraleModel.GetDailyNoWageMoralePenalty(mobileParty) * num;
				if (mobileParty.HasUnpaidWages < num)
				{
					num2 += (float)Campaign.Current.Models.PartyMoraleModel.GetDailyNoWageMoralePenalty(mobileParty) * (num - mobileParty.HasUnpaidWages);
				}
				mobileParty.RecentEventsMorale += num2;
				mobileParty.HasUnpaidWages = num;
				MBTextManager.SetTextVariable("reg1", MathF.Round(MathF.Abs(num2), 1), 2);
				if (mobileParty == MobileParty.MainParty)
				{
					MBInformationManager.AddQuickInformation(GameTexts.FindText("str_party_loses_moral_due_to_insufficent_funds", null), 0, null, null, "");
					return;
				}
			}
			else
			{
				mobileParty.HasUnpaidWages = 0f;
			}
		}

		// Token: 0x04000AC4 RID: 2756
		private const int payGarrisonWagesTreshold = 8000;

		// Token: 0x04000AC5 RID: 2757
		private const int payClanPartiesTreshold = 4000;
	}
}
