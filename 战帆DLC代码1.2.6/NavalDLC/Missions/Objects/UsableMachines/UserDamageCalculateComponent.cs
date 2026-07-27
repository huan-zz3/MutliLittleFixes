using System;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.Objects.UsableMachines
{
	// Token: 0x020000BD RID: 189
	public class UserDamageCalculateComponent : UsableMissionObjectComponent
	{
		// Token: 0x17000298 RID: 664
		// (get) Token: 0x06000E43 RID: 3651 RVA: 0x0006F3E2 File Offset: 0x0006D5E2
		// (set) Token: 0x06000E44 RID: 3652 RVA: 0x0006F3EA File Offset: 0x0006D5EA
		public float DamageReductionFactor { get; private set; }

		// Token: 0x06000E45 RID: 3653 RVA: 0x0006F3F3 File Offset: 0x0006D5F3
		public UserDamageCalculateComponent(PerkObject perkObject, bool isPrimaryBonus, float damageReductionFactor)
		{
			this._perkObject = perkObject;
			this._isPrimaryBonus = isPrimaryBonus;
			this.DamageReductionFactor = damageReductionFactor;
		}

		// Token: 0x06000E46 RID: 3654 RVA: 0x0006F410 File Offset: 0x0006D610
		public void ApplyPerkBonusForCharacter(PerkObject perkObject, bool isPrimaryBonus, CharacterObject agentCharacterObject, ref ExplainedNumber damageResult)
		{
			if (perkObject == this._perkObject && isPrimaryBonus == this._isPrimaryBonus)
			{
				PerkHelper.AddPerkBonusForCharacter(this._perkObject, agentCharacterObject, this._isPrimaryBonus, ref damageResult, false);
			}
		}

		// Token: 0x040008E7 RID: 2279
		private PerkObject _perkObject;

		// Token: 0x040008E8 RID: 2280
		private bool _isPrimaryBonus;
	}
}
