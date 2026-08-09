using System;
using System.Linq;
using Helpers;
using NavalDLC.CharacterDevelopment;
using NavalDLC.Storyline;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000126 RID: 294
	public class NavalDLCPartySpeedCalculationModel : PartySpeedModel
	{
		// Token: 0x1700037A RID: 890
		// (get) Token: 0x06001495 RID: 5269 RVA: 0x00091D49 File Offset: 0x0008FF49
		public override float BaseSpeed
		{
			get
			{
				return base.BaseModel.BaseSpeed;
			}
		}

		// Token: 0x1700037B RID: 891
		// (get) Token: 0x06001496 RID: 5270 RVA: 0x00091D56 File Offset: 0x0008FF56
		public override float MinimumSpeed
		{
			get
			{
				return base.BaseModel.MinimumSpeed;
			}
		}

		// Token: 0x06001497 RID: 5271 RVA: 0x00091D63 File Offset: 0x0008FF63
		public override ExplainedNumber CalculateBaseSpeed(MobileParty party, bool includeDescriptions = false, int additionalTroopOnFootCount = 0, int additionalTroopOnHorseCount = 0)
		{
			if (party.IsCurrentlyAtSea)
			{
				return this.CalculateNavalBaseSpeed(party, includeDescriptions);
			}
			return base.BaseModel.CalculateBaseSpeed(party, includeDescriptions, additionalTroopOnFootCount, additionalTroopOnHorseCount);
		}

		// Token: 0x06001498 RID: 5272 RVA: 0x00091D88 File Offset: 0x0008FF88
		private ExplainedNumber CalculateNavalBaseSpeed(MobileParty mobileParty, bool includeDescriptions = false)
		{
			if (!mobileParty.Ships.Any<Ship>())
			{
				return new ExplainedNumber(4f, includeDescriptions, null);
			}
			float num = 0f;
			float maxValue = float.MaxValue;
			int num2 = 0;
			int num3 = mobileParty.MemberRoster.TotalManCount;
			float num4 = mobileParty.TotalWeightCarried;
			int num5 = mobileParty.Ships.Count;
			int num6 = 0;
			this.GetMobilePartyShipSpeedData(mobileParty, ref num2, ref num6, ref num, ref maxValue);
			if (mobileParty.AttachedParties.Count != 0)
			{
				foreach (MobileParty mobileParty2 in mobileParty.AttachedParties)
				{
					num5 += mobileParty2.Ships.Count;
					num3 += mobileParty2.MemberRoster.TotalManCount;
					num4 += mobileParty2.TotalWeightCarried;
					this.GetMobilePartyShipSpeedData(mobileParty2, ref num2, ref num6, ref num, ref maxValue);
				}
			}
			float num7 = (num / (float)num5 + maxValue) * 0.5f;
			ExplainedNumber explainedNumber;
			explainedNumber..ctor(num7, includeDescriptions, null);
			if (mobileParty.IsFishingParty())
			{
				Settlement bound = mobileParty.VillagerPartyComponent.Village.Bound;
				PerkHelper.AddPerkBonusForTown(NavalPerks.Shipmaster.GhostShip, bound.Town, ref explainedNumber);
			}
			ExplainedNumber explainedNumber2;
			explainedNumber2..ctor((float)num2, false, null);
			PerkHelper.AddPerkBonusForParty(NavalPerks.Shipmaster.FleetCommander, mobileParty, false, ref explainedNumber2, false);
			num2 = explainedNumber2.RoundedResultNumber;
			if (mobileParty.HasPerk(NavalPerks.Shipmaster.ChainToOars, true))
			{
				num3 += mobileParty.PrisonRoster.TotalManCount;
			}
			foreach (MobileParty mobileParty3 in mobileParty.AttachedParties)
			{
				if (mobileParty3.HasPerk(NavalPerks.Shipmaster.ChainToOars, true))
				{
					num3 += mobileParty3.PrisonRoster.TotalManCount;
				}
			}
			if (num3 < num2)
			{
				float underSkeletalCrewEffect = this.GetUnderSkeletalCrewEffect((float)num3, (float)num2);
				TextObject textObject = null;
				if (includeDescriptions)
				{
					textObject = new TextObject("{=4LlzFaUa}Undermanned ({AVAILABLE_CREW}/{NEEDED_CREW})", null);
					textObject.SetTextVariable("AVAILABLE_CREW", num3);
					textObject.SetTextVariable("NEEDED_CREW", num2);
				}
				explainedNumber.AddFactor(underSkeletalCrewEffect, textObject);
			}
			if (num3 > num6)
			{
				float overCrewSizeEffect = this.GetOverCrewSizeEffect(num3, num6);
				TextObject textObject2 = null;
				if (includeDescriptions)
				{
					textObject2 = new TextObject("{=X8V6b6mC}Overmanned ({AVAILABLE_CREW}/{NEEDED_CREW})", null);
					textObject2.SetTextVariable("AVAILABLE_CREW", num3);
					textObject2.SetTextVariable("NEEDED_CREW", num6);
				}
				explainedNumber.AddFactor(overCrewSizeEffect, textObject2);
			}
			int num8 = (int)Campaign.Current.Models.InventoryCapacityModel.CalculateInventoryCapacity(mobileParty, mobileParty.IsCurrentlyAtSea, false, 0, 0, 0, false).ResultNumber;
			if (num4 > (float)num8)
			{
				ExplainedNumber overburdenedEffect = this.GetOverburdenedEffect(mobileParty, num4 - (float)num8, num8, includeDescriptions);
				explainedNumber.AddFromExplainedNumber(overburdenedEffect, NavalDLCPartySpeedCalculationModel._textOverburdened);
			}
			if (num5 > 3)
			{
				int num9 = num5 - 3;
				float num10 = 0.2f;
				float num11 = 0.5f;
				float num12 = num10 / (1f + (float)Math.Exp((double)(-(double)num11 * ((float)num9 - 3f))));
				if (mobileParty.HasPerk(NavalPerks.Shipmaster.ShoreMaster, true))
				{
					num12 *= 1f + NavalPerks.Shipmaster.ShoreMaster.SecondaryBonus;
				}
				if (mobileParty.HasPerk(NavalPerks.Shipmaster.FleetCommander, false))
				{
					num12 *= 1f + NavalPerks.Shipmaster.FleetCommander.PrimaryBonus;
				}
				explainedNumber.AddFactor(-num12, NavalDLCPartySpeedCalculationModel._textOverFleetSize);
			}
			if (mobileParty.IsDisorganized)
			{
				explainedNumber.AddFactor(-0.4f, NavalDLCPartySpeedCalculationModel._textDisorganized);
			}
			explainedNumber.LimitMin(this.MinimumSpeed);
			return explainedNumber;
		}

		// Token: 0x06001499 RID: 5273 RVA: 0x000920F8 File Offset: 0x000902F8
		public override ExplainedNumber CalculateFinalSpeed(MobileParty mobileParty, ExplainedNumber finalSpeed)
		{
			ExplainedNumber explainedNumber = base.BaseModel.CalculateFinalSpeed(mobileParty, finalSpeed);
			TerrainType faceTerrainType = Campaign.Current.MapSceneWrapper.GetFaceTerrainType(mobileParty.CurrentNavigationFace);
			if (mobileParty.IsCurrentlyAtSea)
			{
				if (faceTerrainType == 19)
				{
					explainedNumber.AddFactor(0.448f, NavalDLCPartySpeedCalculationModel._openSeaEffect);
				}
				else if (faceTerrainType == 11)
				{
					explainedNumber.AddFactor(0.5f, NavalDLCPartySpeedCalculationModel._riverEffect);
				}
				if (mobileParty.Ships.Count > 0)
				{
					float num = 0f;
					foreach (Ship ship in mobileParty.Ships)
					{
						if (ship.ShipHull.CanNavigateShallowWater)
						{
							if (faceTerrainType == 18 || faceTerrainType == 11 || faceTerrainType == 25)
							{
								num += ship.GetCampaignSpeed() * 0.066f;
							}
							else
							{
								num -= ship.GetCampaignSpeed() * 0.066f;
							}
						}
					}
					explainedNumber.Add(num / (float)mobileParty.Ships.Count, NavalDLCPartySpeedCalculationModel._textShallowDraftPenalty, null);
				}
				if ((faceTerrainType == 11 || faceTerrainType == 18 || faceTerrainType == 25) && mobileParty.HasPerk(NavalPerks.Shipmaster.RiverRaider, false))
				{
					explainedNumber.AddFactor(-0.448f * NavalPerks.Shipmaster.RiverRaider.PrimaryBonus, NavalPerks.Shipmaster.RiverRaider.Name);
				}
				if ((faceTerrainType == 11 || faceTerrainType == 18 || faceTerrainType == 25) && PartyBaseHelper.HasFeat(mobileParty.Party, NavalCulturalFeats.NordShipMovementFeat))
				{
					explainedNumber.AddFactor(NavalCulturalFeats.NordShipMovementFeat.EffectBonus, this._cultureEffect);
				}
				SkillHelper.AddSkillBonusForParty(NavalSkillEffects.WindBonus, mobileParty, ref explainedNumber);
				PerkHelper.AddPerkBonusForParty(NavalPerks.Shipmaster.OldSaltsTouch, mobileParty, true, ref explainedNumber, false);
				PerkHelper.AddPerkBonusForParty(NavalPerks.Shipmaster.FavorableTide, mobileParty, true, ref explainedNumber, false);
				float num2 = this.CalculateWindBoostForParty(mobileParty);
				explainedNumber.AddFactor(num2 * (1f + explainedNumber.SumOfFactors), NavalDLCPartySpeedCalculationModel._windEffect);
				if (mobileParty.IsMainParty && NavalStorylineData.IsNavalStoryLineActive())
				{
					explainedNumber.Add(1f, NavalDLCPartySpeedCalculationModel._gunnarEffect, null);
				}
				explainedNumber.LimitMax(10f, null);
			}
			return explainedNumber;
		}

		// Token: 0x0600149A RID: 5274 RVA: 0x00092308 File Offset: 0x00090508
		private float CalculateWindBoostForParty(MobileParty mobileParty)
		{
			Vec2 windForPosition = Campaign.Current.Models.MapWeatherModel.GetWindForPosition(mobileParty.Position);
			float num = MathF.Abs(mobileParty.Bearing.RotationInRadians - windForPosition.RotationInRadians) * 57.29578f;
			if (windForPosition.Length <= 0f)
			{
				return 0f;
			}
			if (num < 120f)
			{
				float num2 = MBMath.ClampFloat(MBMath.Map(num, 0f, 120f, windForPosition.Length, 0f) * 1.5f, 0f, 1.5f);
				if (mobileParty.HasPerk(NavalPerks.Shipmaster.FairWinds, false))
				{
					num2 += NavalPerks.Shipmaster.FairWinds.PrimaryBonus;
				}
				return num2;
			}
			float num3 = 0f;
			if (mobileParty.HasPerk(NavalPerks.Shipmaster.ShockAndAwe, true))
			{
				num3 = NavalPerks.Shipmaster.ShockAndAwe.SecondaryBonus;
			}
			return num3;
		}

		// Token: 0x0600149B RID: 5275 RVA: 0x000923E0 File Offset: 0x000905E0
		private ExplainedNumber GetOverburdenedEffect(MobileParty party, float extraWeightCarried, int partyCapacity, bool includeDescriptions)
		{
			ExplainedNumber explainedNumber;
			explainedNumber..ctor(-1f * (extraWeightCarried / (float)partyCapacity), includeDescriptions, null);
			PerkHelper.AddPerkBonusForParty(NavalPerks.Boatswain.VeteransWisdom, party, false, ref explainedNumber, false);
			return explainedNumber;
		}

		// Token: 0x0600149C RID: 5276 RVA: 0x00092414 File Offset: 0x00090614
		private void GetMobilePartyShipSpeedData(MobileParty mobileParty, ref int neededSkeletalCrew, ref int maximumCrewLimit, ref float totalShipSpeed, ref float minimumShipSpeed)
		{
			foreach (Ship ship in mobileParty.Ships)
			{
				neededSkeletalCrew += ship.SkeletalCrewCapacity;
				maximumCrewLimit += ship.TotalCrewCapacity;
				float campaignSpeed = ship.GetCampaignSpeed();
				totalShipSpeed += campaignSpeed;
				if (campaignSpeed < minimumShipSpeed)
				{
					minimumShipSpeed = campaignSpeed;
				}
			}
		}

		// Token: 0x0600149D RID: 5277 RVA: 0x00092490 File Offset: 0x00090690
		private float GetOverCrewSizeEffect(int totalMenCount, int maxCrewSize)
		{
			return 1f / ((float)totalMenCount / (float)maxCrewSize) - 1f;
		}

		// Token: 0x0600149E RID: 5278 RVA: 0x000924A4 File Offset: 0x000906A4
		private float GetUnderSkeletalCrewEffect(float totalManCount, float neededSkeletalCrew)
		{
			float num = totalManCount / neededSkeletalCrew;
			return -(1f - num) * 0.4f;
		}

		// Token: 0x04000AE6 RID: 2790
		private const float RiverBonus = 0.5f;

		// Token: 0x04000AE7 RID: 2791
		private const float OpenSeaBonus = 0.448f;

		// Token: 0x04000AE8 RID: 2792
		private const int PartyFleetSizeThreshold = 3;

		// Token: 0x04000AE9 RID: 2793
		private const int RaftStateSpeed = 4;

		// Token: 0x04000AEA RID: 2794
		private const float DisorganizedEffect = -0.4f;

		// Token: 0x04000AEB RID: 2795
		private const float WindDeadZoneThresholdInDegrees = 60f;

		// Token: 0x04000AEC RID: 2796
		private const float OverburdenedEffect = -1f;

		// Token: 0x04000AED RID: 2797
		private const float MaximumNavalSpeed = 10f;

		// Token: 0x04000AEE RID: 2798
		private static readonly TextObject _textOverburdened = new TextObject("{=xgO3cCgR}Overburdened", null);

		// Token: 0x04000AEF RID: 2799
		private static readonly TextObject _textOverFleetSize = new TextObject("{=D3OvWCpp}Over fleet size", null);

		// Token: 0x04000AF0 RID: 2800
		private static readonly TextObject _textDisorganized = new TextObject("{=JuwBb2Yg}Disorganized", null);

		// Token: 0x04000AF1 RID: 2801
		private static readonly TextObject _textShallowDraftPenalty = new TextObject("{=RU7pNBts}Shallow Draft", null);

		// Token: 0x04000AF2 RID: 2802
		private static readonly TextObject _openSeaEffect = new TextObject("{=KzEFMlfZ}Open Sea", null);

		// Token: 0x04000AF3 RID: 2803
		private static readonly TextObject _riverEffect = new TextObject("{=UvIsHvrt}River", null);

		// Token: 0x04000AF4 RID: 2804
		private static readonly TextObject _windEffect = new TextObject("{=lJDeXyt1}Wind", null);

		// Token: 0x04000AF5 RID: 2805
		private static readonly TextObject _gunnarEffect = new TextObject("{=LSVGrpMr}Gunnar's Skill", null);

		// Token: 0x04000AF6 RID: 2806
		private readonly TextObject _cultureEffect = GameTexts.FindText("str_culture", null);
	}
}
