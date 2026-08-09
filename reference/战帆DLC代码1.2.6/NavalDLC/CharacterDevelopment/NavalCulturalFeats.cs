using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;

namespace NavalDLC.CharacterDevelopment
{
	// Token: 0x0200015A RID: 346
	public class NavalCulturalFeats
	{
		// Token: 0x170003E1 RID: 993
		// (get) Token: 0x06001683 RID: 5763 RVA: 0x00099D66 File Offset: 0x00097F66
		public static NavalCulturalFeats Instance
		{
			get
			{
				return NavalDLCManager.Instance.NavalCulturalFeats;
			}
		}

		// Token: 0x170003E2 RID: 994
		// (get) Token: 0x06001684 RID: 5764 RVA: 0x00099D72 File Offset: 0x00097F72
		public static FeatObject NordHostileActionBonusFeat
		{
			get
			{
				return NavalCulturalFeats.Instance._nordHostileActionBonusLootFeat;
			}
		}

		// Token: 0x170003E3 RID: 995
		// (get) Token: 0x06001685 RID: 5765 RVA: 0x00099D7E File Offset: 0x00097F7E
		public static FeatObject NordHostileActionSpeedFeat
		{
			get
			{
				return NavalCulturalFeats.Instance._nordHostileActionSpeedFeat;
			}
		}

		// Token: 0x170003E4 RID: 996
		// (get) Token: 0x06001686 RID: 5766 RVA: 0x00099D8A File Offset: 0x00097F8A
		public static FeatObject NordShipMovementFeat
		{
			get
			{
				return NavalCulturalFeats.Instance._nordShipMovementFeat;
			}
		}

		// Token: 0x170003E5 RID: 997
		// (get) Token: 0x06001687 RID: 5767 RVA: 0x00099D96 File Offset: 0x00097F96
		public static FeatObject NordArmyCohesionFeat
		{
			get
			{
				return NavalCulturalFeats.Instance._nordArmyCohesionFeat;
			}
		}

		// Token: 0x06001688 RID: 5768 RVA: 0x00099DA2 File Offset: 0x00097FA2
		public NavalCulturalFeats()
		{
			this.RegisterAll();
			this.InitializeAll();
		}

		// Token: 0x06001689 RID: 5769 RVA: 0x00099DB8 File Offset: 0x00097FB8
		private void RegisterAll()
		{
			this._nordHostileActionBonusLootFeat = this.Create("nord_hostile_action_bonus");
			this._nordHostileActionSpeedFeat = this.Create("nord_hostile_action_speed");
			this._nordShipMovementFeat = this.Create("nord_ship_movemenet_increase");
			this._nordArmyCohesionFeat = this.Create("nord_decreased_cohesion_rate");
		}

		// Token: 0x0600168A RID: 5770 RVA: 0x00099E09 File Offset: 0x00098009
		private FeatObject Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<FeatObject>(new FeatObject(stringId));
		}

		// Token: 0x0600168B RID: 5771 RVA: 0x00099E20 File Offset: 0x00098020
		private void InitializeAll()
		{
			this._nordHostileActionSpeedFeat.Initialize("{=!}nord_hostile_action_speed", "{=eI8zKXld}20% raid speed bonus while raiding.", 0.2f, true, 1);
			this._nordHostileActionBonusLootFeat.Initialize("{=!}nord_hostile_action_bonus", "{=hUSnaX6O}+30% more loot from villages, villagers and caravans.", 0.3f, true, 1);
			this._nordShipMovementFeat.Initialize("{=!}nord_ship_movemenet_increase", "{=bEw6FNpM}20% ship movement speed in rivers and coastal seas.", 0.1f, true, 1);
			this._nordArmyCohesionFeat.Initialize("{=!}nord_decreased_cohesion_rate", "{=AnanB4d6}Armies that are commanded by a Nord commander lose 30% more cohesion on land.", -0.3f, false, 1);
		}

		// Token: 0x04000B71 RID: 2929
		private FeatObject _nordHostileActionBonusLootFeat;

		// Token: 0x04000B72 RID: 2930
		private FeatObject _nordHostileActionSpeedFeat;

		// Token: 0x04000B73 RID: 2931
		private FeatObject _nordShipMovementFeat;

		// Token: 0x04000B74 RID: 2932
		private FeatObject _nordArmyCohesionFeat;
	}
}
