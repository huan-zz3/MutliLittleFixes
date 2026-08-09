using System;
using NavalDLC.Map;
using NavalDLC.Storyline;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000120 RID: 288
	public class NavalDLCMobilePartyAIModel : MobilePartyAIModel
	{
		// Token: 0x1700036B RID: 875
		// (get) Token: 0x0600145B RID: 5211 RVA: 0x00091573 File Offset: 0x0008F773
		private IPiratePatrolBehavior PiratePatrolBehavior
		{
			get
			{
				if (this._piratePatrolBehavior == null)
				{
					this._piratePatrolBehavior = Campaign.Current.GetCampaignBehavior<IPiratePatrolBehavior>();
				}
				return this._piratePatrolBehavior;
			}
		}

		// Token: 0x1700036C RID: 876
		// (get) Token: 0x0600145C RID: 5212 RVA: 0x00091593 File Offset: 0x0008F793
		public override float AiCheckInterval
		{
			get
			{
				return base.BaseModel.AiCheckInterval;
			}
		}

		// Token: 0x1700036D RID: 877
		// (get) Token: 0x0600145D RID: 5213 RVA: 0x000915A0 File Offset: 0x0008F7A0
		public override float FleeToNearbyPartyRadius
		{
			get
			{
				return base.BaseModel.FleeToNearbyPartyRadius;
			}
		}

		// Token: 0x1700036E RID: 878
		// (get) Token: 0x0600145E RID: 5214 RVA: 0x000915AD File Offset: 0x0008F7AD
		public override float FleeToNearbySettlementRadius
		{
			get
			{
				return base.BaseModel.FleeToNearbySettlementRadius;
			}
		}

		// Token: 0x1700036F RID: 879
		// (get) Token: 0x0600145F RID: 5215 RVA: 0x000915BA File Offset: 0x0008F7BA
		public override float HideoutPatrolDistanceAsDays
		{
			get
			{
				return base.BaseModel.HideoutPatrolDistanceAsDays;
			}
		}

		// Token: 0x17000370 RID: 880
		// (get) Token: 0x06001460 RID: 5216 RVA: 0x000915C7 File Offset: 0x0008F7C7
		public override float FortificationPatrolDistanceAsDays
		{
			get
			{
				return base.BaseModel.FortificationPatrolDistanceAsDays;
			}
		}

		// Token: 0x17000371 RID: 881
		// (get) Token: 0x06001461 RID: 5217 RVA: 0x000915D4 File Offset: 0x0008F7D4
		public override float FortificationPortPatrolDistanceAsDays
		{
			get
			{
				return 0.5f;
			}
		}

		// Token: 0x17000372 RID: 882
		// (get) Token: 0x06001462 RID: 5218 RVA: 0x000915DB File Offset: 0x0008F7DB
		public override float VillagePatrolDistanceAsDays
		{
			get
			{
				return base.BaseModel.VillagePatrolDistanceAsDays;
			}
		}

		// Token: 0x17000373 RID: 883
		// (get) Token: 0x06001463 RID: 5219 RVA: 0x000915E8 File Offset: 0x0008F7E8
		public override float SettlementDefendingNearbyPartyCheckRadius
		{
			get
			{
				return 20f;
			}
		}

		// Token: 0x17000374 RID: 884
		// (get) Token: 0x06001464 RID: 5220 RVA: 0x000915EF File Offset: 0x0008F7EF
		public override float SettlementDefendingWaitingPositionRadius
		{
			get
			{
				return 3f;
			}
		}

		// Token: 0x17000375 RID: 885
		// (get) Token: 0x06001465 RID: 5221 RVA: 0x000915F6 File Offset: 0x0008F7F6
		public override float NeededFoodsInDaysThresholdForSiege
		{
			get
			{
				return base.BaseModel.NeededFoodsInDaysThresholdForSiege;
			}
		}

		// Token: 0x17000376 RID: 886
		// (get) Token: 0x06001466 RID: 5222 RVA: 0x00091603 File Offset: 0x0008F803
		public override float NeededFoodsInDaysThresholdForRaid
		{
			get
			{
				return base.BaseModel.NeededFoodsInDaysThresholdForRaid;
			}
		}

		// Token: 0x06001467 RID: 5223 RVA: 0x00091610 File Offset: 0x0008F810
		public override float GetPatrolRadius(MobileParty mobileParty, CampaignVec2 patrolPoint)
		{
			if (!patrolPoint.IsOnLand && patrolPoint.IsValid())
			{
				if (mobileParty.IsBandit && this.PiratePatrolBehavior != null)
				{
					return this.PiratePatrolBehavior.GetPatrolRadius(mobileParty);
				}
				if (mobileParty.IsLordParty)
				{
					if (!mobileParty.IsCurrentlyAtSea)
					{
						return 0f;
					}
					float num = 1f;
					if (mobileParty.TargetSettlement.MapFaction == mobileParty.MapFaction)
					{
						num = MBMath.Map(mobileParty.TargetSettlement.NearbyNavalThreatIntensity, 0f, 2f, 1f, 0.5f);
					}
					return base.BaseModel.GetPatrolRadius(mobileParty, patrolPoint) * num;
				}
				else if (mobileParty.IsPatrolParty)
				{
					return Campaign.Current.EstimatedAverageBanditPartyNavalSpeed * (float)CampaignTime.HoursInDay * 0.5f;
				}
			}
			return base.BaseModel.GetPatrolRadius(mobileParty, patrolPoint);
		}

		// Token: 0x06001468 RID: 5224 RVA: 0x000916E2 File Offset: 0x0008F8E2
		public override float GetSettlementNearbyThreatAndAllyCheckRadius(Settlement settlement, bool isPort)
		{
			return base.BaseModel.GetSettlementNearbyThreatAndAllyCheckRadius(settlement, isPort);
		}

		// Token: 0x06001469 RID: 5225 RVA: 0x000916F1 File Offset: 0x0008F8F1
		public override bool ShouldPartyCheckInitiativeBehavior(MobileParty mobileParty)
		{
			return base.BaseModel.ShouldPartyCheckInitiativeBehavior(mobileParty);
		}

		// Token: 0x0600146A RID: 5226 RVA: 0x00091700 File Offset: 0x0008F900
		public override void GetBestInitiativeBehavior(MobileParty mobileParty, out AiBehavior bestInitiativeBehavior, out MobileParty bestInitiativeTargetParty, out float bestInitiativeBehaviorScore, out Vec2 averageEnemyVec)
		{
			base.BaseModel.GetBestInitiativeBehavior(mobileParty, ref bestInitiativeBehavior, ref bestInitiativeTargetParty, ref bestInitiativeBehaviorScore, ref averageEnemyVec);
			float num = ((mobileParty.ShortTermBehavior == 10 && mobileParty.ShortTermTargetParty == null) ? 0.7f : 0.5f);
			Storm storm = null;
			float num2 = float.MaxValue;
			foreach (Storm storm2 in NavalDLCManager.Instance.StormManager.SpawnedStorms)
			{
				if (storm2.IsActive)
				{
					num2 = storm2.CurrentPosition.Distance(mobileParty.Position.ToVec2());
					if (num2 < storm2.EffectRadius)
					{
						storm = storm2;
					}
				}
			}
			if (storm != null && mobileParty.IsCurrentlyAtSea)
			{
				float num3 = 1f - num2 / storm.EffectRadius;
				float num4 = LinQuick.SumQ<Ship>(mobileParty.Ships, (Ship x) => x.HitPoints / x.MaxHitPoints) / (float)mobileParty.Ships.Count - num;
				if (num3 - num4 > 0f)
				{
					bestInitiativeBehaviorScore = 5f;
					bestInitiativeTargetParty = null;
					if (NavalDLCManager.Instance.GameModels.MapStormModel.CanPartyGetDamagedByStorm(mobileParty))
					{
						bool debugVisualsEnabled = NavalDLCManager.Instance.StormManager.DebugVisualsEnabled;
						averageEnemyVec = storm.CurrentPosition - mobileParty.Position.ToVec2();
						bestInitiativeBehavior = 10;
						return;
					}
					if (mobileParty.CurrentSettlement != null)
					{
						bestInitiativeBehavior = 0;
					}
				}
			}
		}

		// Token: 0x0600146B RID: 5227 RVA: 0x00091894 File Offset: 0x0008FA94
		public override bool ShouldConsiderAttacking(MobileParty party, MobileParty targetParty)
		{
			return (!NavalStorylineData.IsNavalStoryLineActive() || (!targetParty.IsNavalStorylineQuestParty() && !targetParty.IsMainParty) || party.IsBandit) && (!party.IsBandit || !party.IsCurrentlyAtSea || !Campaign.Current.Models.BanditDensityModel.IsPositionInsideNavalSafeZone(targetParty.Position)) && base.BaseModel.ShouldConsiderAttacking(party, targetParty);
		}

		// Token: 0x0600146C RID: 5228 RVA: 0x000918FD File Offset: 0x0008FAFD
		public override bool ShouldConsiderAvoiding(MobileParty party, MobileParty targetParty)
		{
			return (party.IsCurrentlyAtSea == targetParty.IsCurrentlyAtSea || party.CurrentSettlement == null) && base.BaseModel.ShouldConsiderAvoiding(party, targetParty);
		}

		// Token: 0x04000AE3 RID: 2787
		private IPiratePatrolBehavior _piratePatrolBehavior;
	}
}
