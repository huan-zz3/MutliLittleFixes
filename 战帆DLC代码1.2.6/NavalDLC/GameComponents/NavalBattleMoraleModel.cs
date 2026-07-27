using System;
using System.Runtime.CompilerServices;
using Helpers;
using NavalDLC.CharacterDevelopment;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace NavalDLC.GameComponents
{
	// Token: 0x02000105 RID: 261
	public class NavalBattleMoraleModel : BattleMoraleModel
	{
		// Token: 0x0600130F RID: 4879 RVA: 0x0008B995 File Offset: 0x00089B95
		private NavalShipsLogic GetNavalShipsLogic()
		{
			return Mission.Current.GetMissionBehavior<NavalShipsLogic>();
		}

		// Token: 0x06001310 RID: 4880 RVA: 0x0008B9A4 File Offset: 0x00089BA4
		[return: TupleElementNames(new string[] { "affectedSideMaxMoraleLoss", "affectorSideMaxMoraleGain" })]
		public override ValueTuple<float, float> CalculateMaxMoraleChangeDueToAgentIncapacitated(Agent affectedAgent, AgentState affectedAgentState, Agent affectorAgent, in KillingBlow killingBlow)
		{
			ValueTuple<float, float> valueTuple = base.BaseModel.CalculateMaxMoraleChangeDueToAgentIncapacitated(affectedAgent, affectedAgentState, affectorAgent, ref killingBlow);
			float item = valueTuple.Item1;
			float item2 = valueTuple.Item2;
			if (Mission.Current.IsNavalBattle)
			{
				ExplainedNumber explainedNumber;
				explainedNumber..ctor(item2, false, null);
				ExplainedNumber explainedNumber2;
				explainedNumber2..ctor(item, false, null);
				if (((affectorAgent != null) ? affectorAgent.Character : null) is CharacterObject)
				{
					object obj;
					if (affectorAgent == null)
					{
						obj = null;
					}
					else
					{
						Formation formation = affectorAgent.Formation;
						if (formation == null)
						{
							obj = null;
						}
						else
						{
							Agent captain = formation.Captain;
							obj = ((captain != null) ? captain.Character : null);
						}
					}
					CharacterObject characterObject = obj as CharacterObject;
					if (characterObject != null && characterObject.GetPerkValue(NavalPerks.Mariner.TerrorOfTheSeas))
					{
						explainedNumber2.AddFactor(NavalPerks.Mariner.TerrorOfTheSeas.PrimaryBonus, null);
					}
				}
				return new ValueTuple<float, float>(explainedNumber2.ResultNumber, explainedNumber.ResultNumber);
			}
			return new ValueTuple<float, float>(item, item2);
		}

		// Token: 0x06001311 RID: 4881 RVA: 0x0008BA6E File Offset: 0x00089C6E
		[return: TupleElementNames(new string[] { "affectedSideMaxMoraleLoss", "affectorSideMaxMoraleGain" })]
		public override ValueTuple<float, float> CalculateMaxMoraleChangeDueToAgentPanicked(Agent agent)
		{
			return base.BaseModel.CalculateMaxMoraleChangeDueToAgentPanicked(agent);
		}

		// Token: 0x06001312 RID: 4882 RVA: 0x0008BA7C File Offset: 0x00089C7C
		public override float CalculateMoraleChangeToCharacter(Agent agent, float maxMoraleChange)
		{
			return base.BaseModel.CalculateMoraleChangeToCharacter(agent, maxMoraleChange);
		}

		// Token: 0x06001313 RID: 4883 RVA: 0x0008BA8C File Offset: 0x00089C8C
		public override float GetEffectiveInitialMorale(Agent agent, float baseMorale)
		{
			float effectiveInitialMorale = base.BaseModel.GetEffectiveInitialMorale(agent, baseMorale);
			ExplainedNumber explainedNumber;
			explainedNumber..ctor(effectiveInitialMorale, false, null);
			object obj;
			if (agent == null)
			{
				obj = null;
			}
			else
			{
				IAgentOriginBase origin = agent.Origin;
				obj = ((origin != null) ? origin.BattleCombatant : null);
			}
			PartyBase partyBase = (PartyBase)obj;
			MobileParty mobileParty = ((partyBase != null && partyBase.IsMobile) ? partyBase.MobileParty : null);
			CharacterObject characterObject = ((agent != null) ? agent.Character : null) as CharacterObject;
			bool flag = false;
			Ship ship = null;
			if (mobileParty != null && characterObject != null)
			{
				Army army = mobileParty.Army;
				CharacterObject characterObject2;
				if (army == null)
				{
					characterObject2 = null;
				}
				else
				{
					MobileParty leaderParty = army.LeaderParty;
					if (leaderParty == null)
					{
						characterObject2 = null;
					}
					else
					{
						Hero leaderHero = leaderParty.LeaderHero;
						characterObject2 = ((leaderHero != null) ? leaderHero.CharacterObject : null);
					}
				}
				CharacterObject characterObject3 = characterObject2;
				Hero leaderHero2 = mobileParty.LeaderHero;
				CharacterObject characterObject4 = ((leaderHero2 != null) ? leaderHero2.CharacterObject : null);
				Formation formation = agent.Formation;
				object obj2;
				if (formation == null)
				{
					obj2 = null;
				}
				else
				{
					Agent captain = formation.Captain;
					obj2 = ((captain != null) ? captain.Character : null);
				}
				CharacterObject characterObject5 = obj2 as CharacterObject;
				if (characterObject == characterObject5)
				{
				}
				if (partyBase != null)
				{
					MBReadOnlyList<Ship> ships = partyBase.Ships;
					int? num = ((ships != null) ? new int?(ships.Count) : null);
					int num2 = 0;
					if ((num.GetValueOrDefault() > num2) & (num != null))
					{
						ship = partyBase.FlagShip;
						Figurehead figurehead = ((ship != null) ? ship.Figurehead : null);
						flag = characterObject3 != null && characterObject3.GetPerkValue(NavalPerks.Shipmaster.Commodore) && ship != null && figurehead != null;
						if (flag && figurehead == DefaultFigureheads.Lion)
						{
							explainedNumber.Add(figurehead.EffectAmount, null, null);
						}
					}
				}
				CharacterObject characterObject6 = ((characterObject3 != characterObject) ? characterObject3 : null);
				characterObject4 = ((characterObject4 != characterObject) ? characterObject4 : null);
				if (characterObject4 != null)
				{
					if (Mission.Current.IsNavalBattle)
					{
						PerkHelper.AddPerkBonusForParty(NavalPerks.Mariner.RallyingCry, mobileParty, true, ref explainedNumber, false);
						if (characterObject.IsMariner)
						{
							PerkHelper.AddPerkBonusForParty(NavalPerks.Mariner.AxeOfTheNorthwind, mobileParty, false, ref explainedNumber, false);
						}
						else
						{
							PerkHelper.AddPerkBonusForParty(NavalPerks.Mariner.SunnyDisposition, mobileParty, false, ref explainedNumber, false);
						}
					}
					if (characterObject4.IsHero)
					{
						Clan clan = characterObject4.HeroObject.Clan;
						if (((clan != null) ? clan.Kingdom : null) != null && characterObject4.HeroObject.Clan.Kingdom.HasPolicy(NavalPolicies.FraternalFleetDoctrine))
						{
							explainedNumber.AddFactor(0.2f, NavalPolicies.FraternalFleetDoctrine.Name);
						}
					}
				}
			}
			NavalShipsLogic missionBehavior = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			if (missionBehavior != null)
			{
				foreach (MissionShip missionShip in missionBehavior.AllShips)
				{
					Ship ship2 = missionShip.ShipOrigin as Ship;
					if (!flag || ship2 != ship)
					{
						Ship ship3 = missionShip.ShipOrigin as Ship;
						Figurehead figurehead2 = ((ship3 != null) ? ship3.Figurehead : null);
						if (figurehead2 != null && figurehead2 == DefaultFigureheads.Lion && missionShip.GetIsAgentOnShip(agent, false))
						{
							explainedNumber.Add(figurehead2.EffectAmount, null, null);
						}
					}
				}
			}
			return explainedNumber.ResultNumber;
		}

		// Token: 0x06001314 RID: 4884 RVA: 0x0008BD80 File Offset: 0x00089F80
		public override bool CanPanicDueToMorale(Agent agent)
		{
			return base.BaseModel.CanPanicDueToMorale(agent);
		}

		// Token: 0x06001315 RID: 4885 RVA: 0x0008BD8E File Offset: 0x00089F8E
		public override float CalculateCasualtiesFactor(BattleSideEnum battleSide)
		{
			return base.BaseModel.CalculateCasualtiesFactor(battleSide);
		}

		// Token: 0x06001316 RID: 4886 RVA: 0x0008BD9C File Offset: 0x00089F9C
		public override float GetAverageMorale(Formation formation)
		{
			return base.BaseModel.GetAverageMorale(formation);
		}

		// Token: 0x06001317 RID: 4887 RVA: 0x0008BDAC File Offset: 0x00089FAC
		public CharacterObject GetEnemyArmyLeaderCharacter(IShipOrigin shipOrigin)
		{
			ShipAssignment shipAssignment;
			this.GetNavalShipsLogic().FindAssignmentOfShipOrigin(shipOrigin, out shipAssignment);
			Agent agent;
			if (shipAssignment == null)
			{
				agent = null;
			}
			else
			{
				Formation formation = shipAssignment.Formation;
				agent = ((formation != null) ? formation.GetFirstUnit() : null);
			}
			Agent agent2 = agent;
			if (agent2 != null)
			{
				foreach (Team team in Mission.Current.Teams)
				{
					if (team.IsEnemyOf(agent2.Team) && team.ActiveAgents.Count > 0)
					{
						Agent agent3 = team.ActiveAgents[0];
						object obj;
						if (agent3 == null)
						{
							obj = null;
						}
						else
						{
							IAgentOriginBase origin = agent3.Origin;
							obj = ((origin != null) ? origin.BattleCombatant : null);
						}
						PartyBase partyBase = (PartyBase)obj;
						MobileParty mobileParty = ((partyBase != null && partyBase.IsMobile) ? partyBase.MobileParty : null);
						CharacterObject characterObject;
						if (mobileParty == null)
						{
							characterObject = null;
						}
						else
						{
							Army army = mobileParty.Army;
							if (army == null)
							{
								characterObject = null;
							}
							else
							{
								MobileParty leaderParty = army.LeaderParty;
								if (leaderParty == null)
								{
									characterObject = null;
								}
								else
								{
									Hero leaderHero = leaderParty.LeaderHero;
									characterObject = ((leaderHero != null) ? leaderHero.CharacterObject : null);
								}
							}
						}
						return characterObject;
					}
				}
			}
			return null;
		}

		// Token: 0x06001318 RID: 4888 RVA: 0x0008BECC File Offset: 0x0008A0CC
		public override float CalculateMoraleChangeOnShipSunk(IShipOrigin shipOrigin)
		{
			float num = base.BaseModel.CalculateMoraleChangeOnShipSunk(shipOrigin);
			CharacterObject enemyArmyLeaderCharacter = this.GetEnemyArmyLeaderCharacter(shipOrigin);
			if (enemyArmyLeaderCharacter != null && enemyArmyLeaderCharacter.GetPerkValue(NavalPerks.Mariner.EnemyOfTheWood))
			{
				num += NavalPerks.Mariner.EnemyOfTheWood.PrimaryBonus;
			}
			return num;
		}

		// Token: 0x06001319 RID: 4889 RVA: 0x0008BF0C File Offset: 0x0008A10C
		public override float CalculateMoraleOnRamming(Agent agent, IShipOrigin rammingShip, IShipOrigin rammedShip)
		{
			float num = base.BaseModel.CalculateMoraleOnRamming(agent, rammingShip, rammedShip);
			ExplainedNumber explainedNumber;
			explainedNumber..ctor(num, false, null);
			Formation formation = agent.Formation;
			object obj;
			if (formation == null)
			{
				obj = null;
			}
			else
			{
				Agent captain = formation.Captain;
				obj = ((captain != null) ? captain.Character : null);
			}
			CharacterObject characterObject = obj as CharacterObject;
			if (((agent != null) ? agent.Character : null) == characterObject)
			{
				characterObject = null;
			}
			PerkHelper.AddPerkBonusFromCaptain(NavalPerks.Shipmaster.ShockAndAwe, characterObject, ref explainedNumber);
			Figurehead figurehead = (rammingShip as Ship).Figurehead;
			if (figurehead != null && figurehead == DefaultFigureheads.Ram)
			{
				explainedNumber.AddFactor(figurehead.EffectAmount, null);
			}
			return explainedNumber.ResultNumber;
		}

		// Token: 0x0600131A RID: 4890 RVA: 0x0008BFA0 File Offset: 0x0008A1A0
		public override float CalculateMoraleOnShipsConnected(Agent agent, IShipOrigin ownerShip, IShipOrigin targetShip)
		{
			float num = base.BaseModel.CalculateMoraleOnShipsConnected(agent, ownerShip, targetShip);
			ExplainedNumber explainedNumber;
			explainedNumber..ctor(num, false, null);
			Figurehead figurehead = (ownerShip as Ship).Figurehead;
			if (figurehead != null && figurehead == DefaultFigureheads.Dragon)
			{
				explainedNumber.Add(figurehead.EffectAmount, null, null);
			}
			return explainedNumber.ResultNumber;
		}
	}
}
