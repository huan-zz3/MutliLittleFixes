using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace NavalDLC.CharacterDevelopment
{
	// Token: 0x0200015C RID: 348
	public class NavalSkillEffects
	{
		// Token: 0x170003E7 RID: 999
		// (get) Token: 0x06001693 RID: 5779 RVA: 0x0009B348 File Offset: 0x00099548
		private static NavalSkillEffects Instance
		{
			get
			{
				return NavalDLCManager.Instance.NavalSkillEffects;
			}
		}

		// Token: 0x170003E8 RID: 1000
		// (get) Token: 0x06001694 RID: 5780 RVA: 0x0009B354 File Offset: 0x00099554
		public static SkillEffect WindBonus
		{
			get
			{
				return NavalSkillEffects.Instance._effectWindBonus;
			}
		}

		// Token: 0x170003E9 RID: 1001
		// (get) Token: 0x06001695 RID: 5781 RVA: 0x0009B360 File Offset: 0x00099560
		public static SkillEffect NavalAutoBattleSimulationAdvantage
		{
			get
			{
				return NavalSkillEffects.Instance._effectNavalAutoBattleSimulationAdvantage;
			}
		}

		// Token: 0x170003EA RID: 1002
		// (get) Token: 0x06001696 RID: 5782 RVA: 0x0009B36C File Offset: 0x0009956C
		public static SkillEffect NavalAutoBattleCombatPenaltyNegation
		{
			get
			{
				return NavalSkillEffects.Instance._effectNavalAutoBattleCombatPenaltyNegation;
			}
		}

		// Token: 0x170003EB RID: 1003
		// (get) Token: 0x06001697 RID: 5783 RVA: 0x0009B378 File Offset: 0x00099578
		public static SkillEffect NavalBattleCombatPenaltyNegation
		{
			get
			{
				return NavalSkillEffects.Instance._effectNavalBattleCombatPenaltyNegation;
			}
		}

		// Token: 0x170003EC RID: 1004
		// (get) Token: 0x06001698 RID: 5784 RVA: 0x0009B384 File Offset: 0x00099584
		public static SkillEffect NavalBattleUnderwaterBreathingDurationBonus
		{
			get
			{
				return NavalSkillEffects.Instance._effectNavalBattleUnderwaterBreathingDurationBonus;
			}
		}

		// Token: 0x170003ED RID: 1005
		// (get) Token: 0x06001699 RID: 5785 RVA: 0x0009B390 File Offset: 0x00099590
		public static SkillEffect ShipDamageReduction
		{
			get
			{
				return NavalSkillEffects.Instance._effectShipDamageReduction;
			}
		}

		// Token: 0x0600169A RID: 5786 RVA: 0x0009B39C File Offset: 0x0009959C
		public NavalSkillEffects()
		{
			this.RegisterAll();
		}

		// Token: 0x0600169B RID: 5787 RVA: 0x0009B3AC File Offset: 0x000995AC
		private void RegisterAll()
		{
			this._effectWindBonus = this.Create("WindBonus");
			this._effectNavalAutoBattleSimulationAdvantage = this.Create("NavalAutoBattleSimulationAdvantage");
			this._effectNavalAutoBattleCombatPenaltyNegation = this.Create("NavalAutoBattleCombatPenaltyNegation");
			this._effectNavalBattleCombatPenaltyNegation = this.Create("NavalBattleCombatPenaltyNegation");
			this._effectNavalBattleUnderwaterBreathingDurationBonus = this.Create("NavalBattleUnderwaterBreathingDurationBonus");
			this._effectShipDamageReduction = this.Create("ShipDamageReduction");
			this.InitializeAll();
		}

		// Token: 0x0600169C RID: 5788 RVA: 0x0009B425 File Offset: 0x00099625
		private SkillEffect Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<SkillEffect>(new SkillEffect(stringId));
		}

		// Token: 0x0600169D RID: 5789 RVA: 0x0009B43C File Offset: 0x0009963C
		private void InitializeAll()
		{
			this._effectWindBonus.Initialize(new TextObject("{=LxA3WTjm}Sailing speed increased by {a0}%", null), NavalSkills.Shipmaster, 15, 0.0005f, 1, 0f, float.MinValue, float.MaxValue);
			this._effectNavalAutoBattleSimulationAdvantage.Initialize(new TextObject("{=Z2uaBxah}Naval simulation advantage: +{a0}%", null), NavalSkills.Mariner, 5, 0.001f, 1, 0f, float.MinValue, float.MaxValue);
			this._effectNavalAutoBattleCombatPenaltyNegation.Initialize(new TextObject("{=7XMyYI9e}Naval Auto Battle Combat Penalty Negation Effect", null), NavalSkills.Mariner, 5, 0.5f, 1, 0f, float.MinValue, float.MaxValue);
			this._effectNavalBattleCombatPenaltyNegation.Initialize(new TextObject("{=k6EubLby}Naval Battle Combat Penalty Negation Effect", null), NavalSkills.Mariner, 12, -0.005f, 1, 0f, -1f, float.MaxValue);
			this._effectNavalBattleUnderwaterBreathingDurationBonus.Initialize(new TextObject("{=95kCGbUp}Naval battle underwater breathing duration: +{a0} Seconds", null), NavalSkills.Mariner, 12, 0.005f, 1, 0f, 0f, 20f);
			this._effectShipDamageReduction.Initialize(new TextObject("{=CyZvyfRa}Reduce ships' received damage by {a0}%", null), NavalSkills.Boatswain, 14, -0.0025f, 1, 0f, float.MinValue, float.MaxValue);
		}

		// Token: 0x04000BB4 RID: 2996
		private SkillEffect _effectWindBonus;

		// Token: 0x04000BB5 RID: 2997
		private SkillEffect _effectNavalAutoBattleSimulationAdvantage;

		// Token: 0x04000BB6 RID: 2998
		private SkillEffect _effectNavalAutoBattleCombatPenaltyNegation;

		// Token: 0x04000BB7 RID: 2999
		private SkillEffect _effectNavalBattleCombatPenaltyNegation;

		// Token: 0x04000BB8 RID: 3000
		private SkillEffect _effectNavalBattleUnderwaterBreathingDurationBonus;

		// Token: 0x04000BB9 RID: 3001
		private SkillEffect _effectShipDamageReduction;
	}
}
