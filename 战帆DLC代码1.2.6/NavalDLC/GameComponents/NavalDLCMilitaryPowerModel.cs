using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;

namespace NavalDLC.GameComponents
{
	// Token: 0x0200011F RID: 287
	public class NavalDLCMilitaryPowerModel : MilitaryPowerModel
	{
		// Token: 0x0600144E RID: 5198 RVA: 0x000911A0 File Offset: 0x0008F3A0
		public override float GetPowerOfParty(PartyBase party, BattleSideEnum side, MapEvent.PowerCalculationContext context)
		{
			float num = base.BaseModel.GetPowerOfParty(party, side, context);
			if (context == 9 || context == 10 || context == 11)
			{
				if (party.Ships.Count == 0)
				{
					return 0f;
				}
				float num2 = LinQuick.AverageQ<Ship>(party.Ships, (Ship x) => x.GetCombatFactor());
				num *= num2;
				num *= this.GetTroopAccommodationRatio(party);
			}
			else if (context == 13 && party.IsMobile && party.MobileParty.IsCurrentlyAtSea)
			{
				num *= this.GetTroopAccommodationRatio(party);
			}
			return num;
		}

		// Token: 0x0600144F RID: 5199 RVA: 0x00091240 File Offset: 0x0008F440
		public override float GetContextModifier(CharacterObject troop, BattleSideEnum battleSideEnum, MapEvent.PowerCalculationContext context)
		{
			if (context == 9 || context == 10 || context == 11)
			{
				return 0f;
			}
			if (context == 12)
			{
				if (battleSideEnum == null)
				{
					if (troop.IsRanged)
					{
						return 0.1f;
					}
				}
				else if (battleSideEnum == 1 && troop.IsRanged && troop.HasMount())
				{
					return -0.5f;
				}
			}
			return base.BaseModel.GetContextModifier(troop, battleSideEnum, context);
		}

		// Token: 0x06001450 RID: 5200 RVA: 0x000912A0 File Offset: 0x0008F4A0
		public override float GetContextModifier(Ship ship, BattleSideEnum battleSide, MapEvent.PowerCalculationContext context)
		{
			if (context == 9 || context == 10 || context == 11)
			{
				switch (ship.ShipHull.Type)
				{
				case 0:
					return this.GetLightShipContextModifier(ship, battleSide, context);
				case 1:
					return this.GetMediumShipContextModifier(ship, battleSide, context);
				case 2:
					return this.GetHeavyShipContextModifier(ship, battleSide, context);
				default:
					Debug.FailedAssert("unhandled ship type", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\GameComponents\\NavalDLCMilitaryPowerModel.cs", "GetContextModifier", 136);
					break;
				}
			}
			return base.BaseModel.GetContextModifier(ship, battleSide, context);
		}

		// Token: 0x06001451 RID: 5201 RVA: 0x00091321 File Offset: 0x0008F521
		public override MapEvent.PowerCalculationContext GetContextForPosition(CampaignVec2 position)
		{
			return base.BaseModel.GetContextForPosition(position);
		}

		// Token: 0x06001452 RID: 5202 RVA: 0x0009132F File Offset: 0x0008F52F
		public override float GetDefaultTroopPower(CharacterObject troop)
		{
			return base.BaseModel.GetDefaultTroopPower(troop);
		}

		// Token: 0x06001453 RID: 5203 RVA: 0x0009133D File Offset: 0x0008F53D
		public override float GetPowerModifierOfHero(Hero leaderHero)
		{
			return base.BaseModel.GetPowerModifierOfHero(leaderHero);
		}

		// Token: 0x06001454 RID: 5204 RVA: 0x0009134C File Offset: 0x0008F54C
		public override float GetTroopPower(CharacterObject troop, BattleSideEnum side, MapEvent.PowerCalculationContext context, float leaderModifier)
		{
			float num = base.BaseModel.GetTroopPower(troop, side, context, leaderModifier);
			if ((context == 9 || context == 12) && !troop.IsHero && troop.IsMariner)
			{
				num *= 1.2f;
			}
			return num;
		}

		// Token: 0x06001455 RID: 5205 RVA: 0x0009138D File Offset: 0x0008F58D
		private float GetLightShipContextModifier(Ship ship, BattleSideEnum battleSide, MapEvent.PowerCalculationContext context)
		{
			if (battleSide != 1)
			{
				return NavalDLCMilitaryPowerModel._lightShipDefenderModifiers[context];
			}
			return NavalDLCMilitaryPowerModel._lightShipAttackerModifiers[context];
		}

		// Token: 0x06001456 RID: 5206 RVA: 0x000913AA File Offset: 0x0008F5AA
		private float GetMediumShipContextModifier(Ship ship, BattleSideEnum battleSide, MapEvent.PowerCalculationContext context)
		{
			if (battleSide != 1)
			{
				return NavalDLCMilitaryPowerModel._mediumShipDefenderModifiers[context];
			}
			return NavalDLCMilitaryPowerModel._mediumShipAttackerModifiers[context];
		}

		// Token: 0x06001457 RID: 5207 RVA: 0x000913C7 File Offset: 0x0008F5C7
		private float GetHeavyShipContextModifier(Ship ship, BattleSideEnum battleSide, MapEvent.PowerCalculationContext context)
		{
			if (battleSide != 1)
			{
				return NavalDLCMilitaryPowerModel._heavyShipDefenderModifiers[context];
			}
			return NavalDLCMilitaryPowerModel._heavyShipAttackerModifiers[context];
		}

		// Token: 0x06001458 RID: 5208 RVA: 0x000913E4 File Offset: 0x0008F5E4
		private float GetTroopAccommodationRatio(PartyBase party)
		{
			float num = 1f;
			float num2 = (float)LinQuick.SumQ<Ship>(party.Ships, (Ship x) => x.TotalCrewCapacity);
			if ((float)party.NumberOfAllMembers > num2)
			{
				num = num2 / (float)party.NumberOfAllMembers;
			}
			return num;
		}

		// Token: 0x04000ADC RID: 2780
		private const float MarinerTroopSeaBattlePowerBonus = 1.2f;

		// Token: 0x04000ADD RID: 2781
		private static readonly Dictionary<MapEvent.PowerCalculationContext, float> _lightShipAttackerModifiers = new Dictionary<MapEvent.PowerCalculationContext, float>
		{
			{ 9, 0.2f },
			{ 10, -0.2f },
			{ 11, 0.2f }
		};

		// Token: 0x04000ADE RID: 2782
		private static readonly Dictionary<MapEvent.PowerCalculationContext, float> _lightShipDefenderModifiers = new Dictionary<MapEvent.PowerCalculationContext, float>
		{
			{ 9, 0.2f },
			{ 10, -0.2f },
			{ 11, 0.2f }
		};

		// Token: 0x04000ADF RID: 2783
		private static readonly Dictionary<MapEvent.PowerCalculationContext, float> _mediumShipAttackerModifiers = new Dictionary<MapEvent.PowerCalculationContext, float>
		{
			{ 9, 0f },
			{ 10, 0f },
			{ 11, 0f }
		};

		// Token: 0x04000AE0 RID: 2784
		private static readonly Dictionary<MapEvent.PowerCalculationContext, float> _mediumShipDefenderModifiers = new Dictionary<MapEvent.PowerCalculationContext, float>
		{
			{ 9, 0f },
			{ 10, 0f },
			{ 11, 0f }
		};

		// Token: 0x04000AE1 RID: 2785
		private static readonly Dictionary<MapEvent.PowerCalculationContext, float> _heavyShipAttackerModifiers = new Dictionary<MapEvent.PowerCalculationContext, float>
		{
			{ 9, -0.2f },
			{ 10, 0.2f },
			{ 11, -0.2f }
		};

		// Token: 0x04000AE2 RID: 2786
		private static readonly Dictionary<MapEvent.PowerCalculationContext, float> _heavyShipDefenderModifiers = new Dictionary<MapEvent.PowerCalculationContext, float>
		{
			{ 9, -0.2f },
			{ 10, 0.2f },
			{ 11, -0.2f }
		};
	}
}
