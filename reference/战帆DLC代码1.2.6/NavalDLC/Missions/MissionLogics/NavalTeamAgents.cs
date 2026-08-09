using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions.Deployment;
using NavalDLC.Missions.Objects;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.MissionLogics
{
	// Token: 0x020000D5 RID: 213
	internal class NavalTeamAgents
	{
		// Token: 0x170002EF RID: 751
		// (get) Token: 0x0600108E RID: 4238 RVA: 0x0007C1E6 File Offset: 0x0007A3E6
		internal IReadOnlyCollection<IAgentOriginBase> AllTroopOrigins
		{
			get
			{
				return this._allTroopOrigins;
			}
		}

		// Token: 0x170002F0 RID: 752
		// (get) Token: 0x0600108F RID: 4239 RVA: 0x0007C1EE File Offset: 0x0007A3EE
		internal IReadOnlyCollection<IAgentOriginBase> AllHeroOrigins
		{
			get
			{
				return this._allHeroOrigins;
			}
		}

		// Token: 0x170002F1 RID: 753
		// (get) Token: 0x06001090 RID: 4240 RVA: 0x0007C1F6 File Offset: 0x0007A3F6
		// (set) Token: 0x06001091 RID: 4241 RVA: 0x0007C1FE File Offset: 0x0007A3FE
		internal int NumberOfSpawnedAgents { get; private set; }

		// Token: 0x170002F2 RID: 754
		// (get) Token: 0x06001092 RID: 4242 RVA: 0x0007C207 File Offset: 0x0007A407
		internal int NumberOfActiveTroops
		{
			get
			{
				return this._agentToShipAgents.Count;
			}
		}

		// Token: 0x170002F3 RID: 755
		// (get) Token: 0x06001093 RID: 4243 RVA: 0x0007C214 File Offset: 0x0007A414
		internal int NumberOfUnassignedTroops
		{
			get
			{
				return this._unassignedTroops.Count;
			}
		}

		// Token: 0x170002F4 RID: 756
		// (get) Token: 0x06001094 RID: 4244 RVA: 0x0007C221 File Offset: 0x0007A421
		// (set) Token: 0x06001095 RID: 4245 RVA: 0x0007C229 File Offset: 0x0007A429
		internal bool SpawnReinforcementsOnTick { get; private set; }

		// Token: 0x170002F5 RID: 757
		// (get) Token: 0x06001096 RID: 4246 RVA: 0x0007C232 File Offset: 0x0007A432
		// (set) Token: 0x06001097 RID: 4247 RVA: 0x0007C23A File Offset: 0x0007A43A
		public bool RestrictRecentlySwappedAgentTransfers { get; private set; }

		// Token: 0x06001098 RID: 4248 RVA: 0x0007C244 File Offset: 0x0007A444
		internal NavalTeamAgents(NavalAgentsLogic agentsLogic, BattleSideEnum battleSide, TeamSideEnum teamSide)
		{
			this.AgentsLogic = agentsLogic;
			this.BattleSide = battleSide;
			this.TeamSide = teamSide;
			this._allTroopOrigins = new HashSet<IAgentOriginBase>();
			this._allHeroOrigins = new HashSet<IAgentOriginBase>();
			this._unassignedTroops = new Dictionary<IAgentOriginBase, NavalTroopAssignment>();
			this._unassignedOrderedTroops = new MBSortedMultiList<int, NavalTroopAssignment>(true);
			this._unassignedTroopCountData = default(NavalTeamAgents.TroopCountData);
			this._unassignedReservedAgents = new Dictionary<Agent, MissionShip>();
			this._allShipAgents = new MBList<NavalShipAgents>();
			this._agentToShipAgents = new Dictionary<Agent, NavalShipAgents>();
			this._tempSpawnedAgentsList = new MBList<Agent>();
			this._tempShipsWithMissingTroops = new MBList<NavalShipAgents>();
			this._tempUnassignedTroops = new MBList<NavalTroopAssignment>();
			this._tempAgentsNotUsingMachines = new MBList<Agent>();
			this._tempIncompatibleAgentsList = new MBList<Agent>();
			this._tempIncompatibleReservesList = new MBList<IAgentOriginBase>();
		}

		// Token: 0x06001099 RID: 4249 RVA: 0x0007C314 File Offset: 0x0007A514
		internal void AddAgentToShip(Agent agent, MissionShip targetShip)
		{
			MissionShip missionShip;
			bool flag = this.IsAgentOnAnyShip(agent, out missionShip);
			bool flag2 = this._unassignedTroops.ContainsKey(agent.Origin);
			if (!flag && !flag2)
			{
				NavalShipAgents navalShipAgents;
				this.TryGetShipAgents(targetShip, out navalShipAgents);
				this.AddTroopOriginAux(agent.Origin);
				if (this.AgentsLogic.IsDeploymentMode)
				{
					this.MakeSpaceForOneAgent(navalShipAgents, true);
				}
				this.AddAgentAux(agent, navalShipAgents);
			}
		}

		// Token: 0x0600109A RID: 4250 RVA: 0x0007C374 File Offset: 0x0007A574
		internal void RemoveAgentFromShip(Agent agent, MissionShip ship)
		{
			MissionShip missionShip;
			this.IsAgentOnAnyShip(agent, out missionShip);
			NavalShipAgents navalShipAgents;
			this.TryGetShipAgents(ship, out navalShipAgents);
			this.RemoveAgentAux(agent, navalShipAgents);
			this.RemoveTroopOriginAux(agent.Origin);
		}

		// Token: 0x0600109B RID: 4251 RVA: 0x0007C3AC File Offset: 0x0007A5AC
		internal int GetNumberOfReservedTroops(bool spawnableOnly)
		{
			int num = 0;
			foreach (NavalShipAgents navalShipAgents in this._allShipAgents)
			{
				if (navalShipAgents.SpawnReinforcements)
				{
					num += navalShipAgents.ReservedTroopsCount;
				}
			}
			return num;
		}

		// Token: 0x0600109C RID: 4252 RVA: 0x0007C40C File Offset: 0x0007A60C
		internal bool AddReservedTroopToShip(IAgentOriginBase troopOrigin, MissionShip ship)
		{
			NavalShipAgents navalShipAgents;
			this.TryGetShipAgents(ship, out navalShipAgents);
			return this.AddReservedTroopToShipAux(troopOrigin, navalShipAgents);
		}

		// Token: 0x0600109D RID: 4253 RVA: 0x0007C42C File Offset: 0x0007A62C
		internal int AddReservedTroopsToShip(MBList<IAgentOriginBase> troopOrigins, MissionShip ship)
		{
			int num = 0;
			NavalShipAgents navalShipAgents;
			this.TryGetShipAgents(ship, out navalShipAgents);
			foreach (IAgentOriginBase agentOriginBase in troopOrigins)
			{
				if (this.AddReservedTroopToShipAux(agentOriginBase, navalShipAgents))
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x0600109E RID: 4254 RVA: 0x0007C490 File Offset: 0x0007A690
		internal void RemoveReservedTroopFromShip(IAgentOriginBase troopOrigin, MissionShip ship)
		{
			NavalShipAgents navalShipAgents;
			this.TryGetShipAgents(ship, out navalShipAgents);
			this.RemoveReservedTroopFromShipAux(troopOrigin, navalShipAgents);
		}

		// Token: 0x0600109F RID: 4255 RVA: 0x0007C4B0 File Offset: 0x0007A6B0
		internal void RemoveReservedTroopsFromShip(MBList<IAgentOriginBase> troopOrigins, MissionShip ship)
		{
			NavalShipAgents navalShipAgents;
			this.TryGetShipAgents(ship, out navalShipAgents);
			foreach (IAgentOriginBase agentOriginBase in troopOrigins)
			{
				this.RemoveReservedTroopFromShipAux(agentOriginBase, navalShipAgents);
			}
		}

		// Token: 0x060010A0 RID: 4256 RVA: 0x0007C50C File Offset: 0x0007A70C
		internal int RemoveReservedTroopsFromShip(MissionShip ship, int count)
		{
			int num = 0;
			NavalShipAgents navalShipAgents;
			this.TryGetShipAgents(ship, out navalShipAgents);
			if (count <= 0)
			{
				count = navalShipAgents.ReservedTroopsCount;
			}
			else
			{
				count = MathF.Min(navalShipAgents.ReservedTroopsCount, count);
			}
			while (num < count && this.RemoveReservedTroopFromShipAux(navalShipAgents))
			{
				num++;
			}
			return num;
		}

		// Token: 0x060010A1 RID: 4257 RVA: 0x0007C558 File Offset: 0x0007A758
		internal void RemoveAllReservedTroopsFromShip(MissionShip ship)
		{
			NavalShipAgents navalShipAgents;
			this.TryGetShipAgents(ship, out navalShipAgents);
			int reservedTroopsCount = navalShipAgents.ReservedTroopsCount;
			this.RemoveReservedTroopsFromShip(ship, reservedTroopsCount);
		}

		// Token: 0x060010A2 RID: 4258 RVA: 0x0007C580 File Offset: 0x0007A780
		internal bool TransferAgentToShip(Agent agent, MissionShip targetShip, bool swapAgents)
		{
			NavalShipAgents navalShipAgents;
			this._agentToShipAgents.TryGetValue(agent, out navalShipAgents);
			NavalShipAgents navalShipAgents2;
			this.TryGetShipAgents(targetShip, out navalShipAgents2);
			bool flag = false;
			if (navalShipAgents == navalShipAgents2)
			{
				flag = true;
			}
			else
			{
				if (swapAgents && this.AgentsLogic.IsDeploymentMode && navalShipAgents2.ActiveAgents.Count > 0)
				{
					Agent minimumPriorityActiveAgent = navalShipAgents2.GetMinimumPriorityActiveAgent(this._recentlySwappedAgents);
					this.RemoveAgentAux(minimumPriorityActiveAgent, navalShipAgents2);
					this.MakeSpaceForOneAgent(navalShipAgents2, true);
					this.TransferAgentAux(agent, navalShipAgents, navalShipAgents2);
					this.AddAgentAux(minimumPriorityActiveAgent, navalShipAgents);
					if (this.RestrictRecentlySwappedAgentTransfers && !this._recentlySwappedAgents.Contains(minimumPriorityActiveAgent))
					{
						this._recentlySwappedAgents.Add(minimumPriorityActiveAgent);
					}
					flag = true;
				}
				else if (navalShipAgents2.CanAddMoreAgents || this.AgentsLogic.IsDeploymentMode)
				{
					if (this.AgentsLogic.IsDeploymentMode)
					{
						this.MakeSpaceForOneAgent(navalShipAgents2, true);
					}
					this.TransferAgentAux(agent, navalShipAgents, navalShipAgents2);
					flag = true;
				}
				if (flag)
				{
					Formation formation = navalShipAgents.Ship.Formation;
					if (((formation != null) ? formation.Captain : null) == agent)
					{
						this.SetManagedCaptainOfFormation(null, navalShipAgents.Ship.Formation);
					}
				}
			}
			return flag;
		}

		// Token: 0x060010A3 RID: 4259 RVA: 0x0007C690 File Offset: 0x0007A890
		internal void AssignCaptainToShip(Agent captainAgent, MissionShip targetShip, bool swapOnTransfer, MissionShip captainsCurrentShip)
		{
			NavalShipAgents navalShipAgents;
			this.TryGetShipAgents(targetShip, out navalShipAgents);
			Formation formation = navalShipAgents.Ship.Formation;
			if (targetShip.Captain != captainAgent)
			{
				if (targetShip.Captain != null)
				{
					this.UnassignCaptainOfShip(targetShip);
				}
				if (captainAgent != null)
				{
					if (captainsCurrentShip == null)
					{
						this.IsAgentOnAnyShip(captainAgent, out captainsCurrentShip);
					}
					if (captainsCurrentShip != targetShip)
					{
						this.TransferAgentToShip(captainAgent, targetShip, swapOnTransfer);
					}
					this.SetManagedCaptainOfFormation(captainAgent, formation);
				}
			}
		}

		// Token: 0x060010A4 RID: 4260 RVA: 0x0007C6F3 File Offset: 0x0007A8F3
		internal void UnassignCaptainOfShip(MissionShip targetShip)
		{
			this.SetManagedCaptainOfFormation(null, targetShip.Formation);
		}

		// Token: 0x060010A5 RID: 4261 RVA: 0x0007C704 File Offset: 0x0007A904
		internal IAgentOriginBase FindTroopOrigin(Predicate<IAgentOriginBase> predicate)
		{
			foreach (IAgentOriginBase agentOriginBase in this._allTroopOrigins)
			{
				if (predicate(agentOriginBase))
				{
					return agentOriginBase;
				}
			}
			return null;
		}

		// Token: 0x060010A6 RID: 4262 RVA: 0x0007C760 File Offset: 0x0007A960
		internal int FindTroopOrigins(Predicate<IAgentOriginBase> predicate, ref MBList<IAgentOriginBase> foundOrigins)
		{
			if (foundOrigins == null)
			{
				foundOrigins = new MBList<IAgentOriginBase>();
			}
			foundOrigins.Clear();
			foreach (IAgentOriginBase agentOriginBase in this._allTroopOrigins)
			{
				if (predicate(agentOriginBase))
				{
					foundOrigins.Add(agentOriginBase);
				}
			}
			return foundOrigins.Count;
		}

		// Token: 0x060010A7 RID: 4263 RVA: 0x0007C7D8 File Offset: 0x0007A9D8
		internal bool IsTroopUnassigned(IAgentOriginBase troopOrigin)
		{
			return this._unassignedTroops.ContainsKey(troopOrigin);
		}

		// Token: 0x060010A8 RID: 4264 RVA: 0x0007C7E8 File Offset: 0x0007A9E8
		internal bool IsTroopInShipReserves(IAgentOriginBase origin, out MissionShip ship)
		{
			ship = null;
			foreach (NavalShipAgents navalShipAgents in this._allShipAgents)
			{
				if (navalShipAgents.IsOriginInReserves(origin))
				{
					ship = navalShipAgents.Ship;
					return true;
				}
			}
			return false;
		}

		// Token: 0x060010A9 RID: 4265 RVA: 0x0007C850 File Offset: 0x0007AA50
		internal bool IsAgentOnAnyShip(IAgentOriginBase origin, out Agent agent, out MissionShip ship)
		{
			agent = null;
			ship = null;
			Func<Agent, bool> <>9__0;
			foreach (NavalShipAgents navalShipAgents in this._allShipAgents)
			{
				MBReadOnlyList<Agent> mbreadOnlyList = (origin.Troop.IsHero ? navalShipAgents.ActiveHeroAgents : navalShipAgents.ActiveNonHeroAgents);
				IEnumerable<Agent> enumerable = mbreadOnlyList;
				Func<Agent, bool> func;
				if ((func = <>9__0) == null)
				{
					func = (<>9__0 = (Agent agnt) => agnt.Origin == origin);
				}
				agent = enumerable.FirstOrDefault<Agent>(func);
				if (agent != null)
				{
					ship = navalShipAgents.Ship;
					break;
				}
			}
			return agent != null;
		}

		// Token: 0x060010AA RID: 4266 RVA: 0x0007C90C File Offset: 0x0007AB0C
		internal bool IsAgentOnAnyShip(Agent agent, out MissionShip ship)
		{
			NavalShipAgents navalShipAgents;
			if (this._agentToShipAgents.TryGetValue(agent, out navalShipAgents))
			{
				ship = navalShipAgents.Ship;
				return true;
			}
			ship = null;
			return false;
		}

		// Token: 0x060010AB RID: 4267 RVA: 0x0007C938 File Offset: 0x0007AB38
		internal bool IsAgentOnShip(Agent agent, MissionShip ship)
		{
			NavalShipAgents navalShipAgents;
			return this._agentToShipAgents.TryGetValue(agent, out navalShipAgents) && navalShipAgents.Ship == ship;
		}

		// Token: 0x060010AC RID: 4268 RVA: 0x0007C960 File Offset: 0x0007AB60
		internal MBReadOnlyList<Agent> GetActiveAgents()
		{
			MBList<Agent> mblist = new MBList<Agent>();
			foreach (NavalShipAgents navalShipAgents in this._allShipAgents)
			{
				mblist.AddRange(navalShipAgents.ActiveAgents);
			}
			return mblist;
		}

		// Token: 0x060010AD RID: 4269 RVA: 0x0007C9C0 File Offset: 0x0007ABC0
		internal int GetActiveTroopsCountOfShip(MissionShip ship)
		{
			return this.GetActiveAgentsOfShip(ship).Count;
		}

		// Token: 0x060010AE RID: 4270 RVA: 0x0007C9D0 File Offset: 0x0007ABD0
		internal MBReadOnlyList<Agent> GetActiveAgentsOfShip(MissionShip ship)
		{
			NavalShipAgents navalShipAgents;
			this.TryGetShipAgents(ship, out navalShipAgents);
			if (navalShipAgents == null)
			{
				return null;
			}
			return navalShipAgents.ActiveAgents;
		}

		// Token: 0x060010AF RID: 4271 RVA: 0x0007C9F4 File Offset: 0x0007ABF4
		internal int GetTotalTroopsCountOfShip(MissionShip ship, bool spawnableReservesOnly)
		{
			NavalShipAgents navalShipAgents;
			this.TryGetShipAgents(ship, out navalShipAgents);
			int num = navalShipAgents.ActiveAgents.Count;
			if (!spawnableReservesOnly || navalShipAgents.SpawnReinforcements)
			{
				num += navalShipAgents.ReservedTroopsCount;
			}
			return num;
		}

		// Token: 0x060010B0 RID: 4272 RVA: 0x0007CA2C File Offset: 0x0007AC2C
		internal int GetReservedTroopsCountOfShip(MissionShip ship)
		{
			NavalShipAgents navalShipAgents;
			this.TryGetShipAgents(ship, out navalShipAgents);
			return navalShipAgents.ReservedTroopsCount;
		}

		// Token: 0x060010B1 RID: 4273 RVA: 0x0007CA4C File Offset: 0x0007AC4C
		internal void FillReservedTroopsOfShip(MissionShip ship, MBList<IAgentOriginBase> reservedTroops)
		{
			NavalShipAgents navalShipAgents;
			this.TryGetShipAgents(ship, out navalShipAgents);
			navalShipAgents.FillReservedTroops(reservedTroops);
		}

		// Token: 0x060010B2 RID: 4274 RVA: 0x0007CA6C File Offset: 0x0007AC6C
		internal MBReadOnlyList<Agent> GetActiveHeroesOfShip(MissionShip ship)
		{
			NavalShipAgents navalShipAgents;
			this.TryGetShipAgents(ship, out navalShipAgents);
			return navalShipAgents.ActiveHeroAgents;
		}

		// Token: 0x060010B3 RID: 4275 RVA: 0x0007CA8C File Offset: 0x0007AC8C
		internal void AutoComputeDesiredTroopCountsPerShip(bool loadBalanceShips, int troopLimitFromBattleSize)
		{
			if (loadBalanceShips)
			{
				int num = 0;
				foreach (NavalShipAgents navalShipAgents in this._allShipAgents)
				{
					num += navalShipAgents.Ship.TotalCrewCapacity;
				}
				int num2 = Math.Min(troopLimitFromBattleSize, this._allTroopOrigins.Count);
				float num3 = (float)num2 / (float)num;
				float num4 = (float)troopLimitFromBattleSize / (float)this._allShipAgents.Count;
				int num5 = 0;
				foreach (NavalShipAgents navalShipAgents2 in this._allShipAgents)
				{
					float num6 = MathF.Min((float)navalShipAgents2.Ship.TotalCrewCapacity * num3, (float)navalShipAgents2.Ship.TotalCrewCapacity);
					if (num6 < (float)navalShipAgents2.Ship.ShipOrigin.SkeletalCrewCapacity)
					{
						num6 = (float)navalShipAgents2.Ship.ShipOrigin.SkeletalCrewCapacity;
					}
					if (num6 > num4)
					{
						num6 = num4;
					}
					int num7 = (int)num6;
					navalShipAgents2.SetDesiredTroopCount(num7);
					num5 += num7;
				}
				int num8 = Math.Min(num2, num) - num5;
				bool flag = true;
				while (flag)
				{
					if (num8 <= 0)
					{
						return;
					}
					flag = false;
					float num9 = float.MaxValue;
					int num10 = -1;
					for (int i = 0; i < this._allShipAgents.Count; i++)
					{
						NavalShipAgents navalShipAgents3 = this._allShipAgents[i];
						if (navalShipAgents3.DesiredTroopCount < navalShipAgents3.Ship.TotalCrewCapacity)
						{
							float num11 = (float)navalShipAgents3.DesiredTroopCount / (float)navalShipAgents3.Ship.ShipOrigin.SkeletalCrewCapacity;
							if (num9 > num11)
							{
								num9 = num11;
								num10 = i;
							}
						}
					}
					if (num10 != -1)
					{
						NavalShipAgents navalShipAgents4 = this._allShipAgents[num10];
						navalShipAgents4.SetDesiredTroopCount(navalShipAgents4.DesiredTroopCount + 1);
						num5++;
						num8--;
						flag = true;
					}
				}
			}
			else
			{
				foreach (NavalShipAgents navalShipAgents5 in this._allShipAgents)
				{
					navalShipAgents5.SetDesiredTroopCount(navalShipAgents5.Ship.TotalCrewCapacity);
				}
			}
		}

		// Token: 0x060010B4 RID: 4276 RVA: 0x0007CCD4 File Offset: 0x0007AED4
		internal void SetDesiredTroopCountOfShip(MissionShip ship, int desiredTroopCount)
		{
			NavalShipAgents navalShipAgents;
			this.TryGetShipAgents(ship, out navalShipAgents);
			navalShipAgents.SetDesiredTroopCount(desiredTroopCount);
		}

		// Token: 0x060010B5 RID: 4277 RVA: 0x0007CCF4 File Offset: 0x0007AEF4
		internal int GetDesiredTroopCountOfShip(MissionShip ship)
		{
			NavalShipAgents navalShipAgents;
			this.TryGetShipAgents(ship, out navalShipAgents);
			return navalShipAgents.DesiredTroopCount;
		}

		// Token: 0x060010B6 RID: 4278 RVA: 0x0007CD14 File Offset: 0x0007AF14
		internal void SetIgnoreTroopCapacities(bool value)
		{
			foreach (NavalShipAgents navalShipAgents in this._allShipAgents)
			{
				navalShipAgents.SetIgnoreCapacityChecks(value);
			}
		}

		// Token: 0x060010B7 RID: 4279 RVA: 0x0007CD68 File Offset: 0x0007AF68
		internal void SetIgnoreTroopCapacities(MissionShip ship, bool value)
		{
			NavalShipAgents navalShipAgents;
			this.TryGetShipAgents(ship, out navalShipAgents);
			navalShipAgents.SetIgnoreCapacityChecks(value);
		}

		// Token: 0x060010B8 RID: 4280 RVA: 0x0007CD88 File Offset: 0x0007AF88
		internal int SpawnNextBatch(bool isReinforcement, MBList<Agent> spawnedAgents = null)
		{
			int num = 0;
			foreach (NavalShipAgents navalShipAgents in this._allShipAgents)
			{
				ValueTuple<int, int> valueTuple = navalShipAgents.SpawnNextBatch(isReinforcement, this._tempSpawnedAgentsList);
				int item = valueTuple.Item1;
				int item2 = valueTuple.Item2;
				num += item + item2;
				this.NumberOfSpawnedAgents += item;
				foreach (Agent agent in this._tempSpawnedAgentsList)
				{
					this._agentToShipAgents[agent] = navalShipAgents;
				}
				if (spawnedAgents != null)
				{
					spawnedAgents.AddRange(this._tempSpawnedAgentsList);
				}
				this._tempSpawnedAgentsList.Clear();
			}
			return num;
		}

		// Token: 0x060010B9 RID: 4281 RVA: 0x0007CE74 File Offset: 0x0007B074
		internal void SetSpawnReinforcementsOnTick(bool value, bool resetShips)
		{
			this.SpawnReinforcementsOnTick = value;
			if (resetShips)
			{
				foreach (NavalShipAgents navalShipAgents in this._allShipAgents)
				{
					navalShipAgents.SetSpawnReinforcements(value);
				}
			}
		}

		// Token: 0x060010BA RID: 4282 RVA: 0x0007CED0 File Offset: 0x0007B0D0
		internal void SetSpawnReinforcementsForShip(MissionShip ship, bool value)
		{
			NavalShipAgents navalShipAgents;
			this.TryGetShipAgents(ship, out navalShipAgents);
			navalShipAgents.SetSpawnReinforcements(value);
		}

		// Token: 0x060010BB RID: 4283 RVA: 0x0007CEF0 File Offset: 0x0007B0F0
		internal bool GetSpawnReinforcementsForShip(MissionShip ship)
		{
			NavalShipAgents navalShipAgents;
			this.TryGetShipAgents(ship, out navalShipAgents);
			return navalShipAgents.SpawnReinforcements;
		}

		// Token: 0x060010BC RID: 4284 RVA: 0x0007CF10 File Offset: 0x0007B110
		internal int CheckSpawnReinforcements(MBList<Agent> spawnedAgents = null)
		{
			int num = 0;
			foreach (NavalShipAgents navalShipAgents in this._allShipAgents)
			{
				if (navalShipAgents.SpawnReinforcements)
				{
					int num2 = navalShipAgents.CheckSpawnReinforcements(this._tempSpawnedAgentsList);
					num += num2;
					this.NumberOfSpawnedAgents += num2;
					foreach (Agent agent in this._tempSpawnedAgentsList)
					{
						this._agentToShipAgents[agent] = navalShipAgents;
					}
					if (spawnedAgents != null)
					{
						spawnedAgents.AddRange(this._tempSpawnedAgentsList);
					}
					this._tempSpawnedAgentsList.Clear();
				}
			}
			return num;
		}

		// Token: 0x060010BD RID: 4285 RVA: 0x0007CFF4 File Offset: 0x0007B1F4
		internal void InitializeReinforcementTimers(bool randomizeTimers, bool autoComputeDurations)
		{
			if (autoComputeDurations)
			{
				foreach (NavalShipAgents navalShipAgents in this._allShipAgents)
				{
					navalShipAgents.SetReinforcementSpawnDuration(0f);
				}
			}
			foreach (NavalShipAgents navalShipAgents2 in this._allShipAgents)
			{
				navalShipAgents2.InitializeReinforcementTimer(randomizeTimers);
			}
		}

		// Token: 0x060010BE RID: 4286 RVA: 0x0007D08C File Offset: 0x0007B28C
		internal void SetReinforcementSpawnDurationOfShip(MissionShip ship, float duration)
		{
			NavalShipAgents navalShipAgents;
			this.TryGetShipAgents(ship, out navalShipAgents);
			navalShipAgents.SetReinforcementSpawnDuration(duration);
		}

		// Token: 0x060010BF RID: 4287 RVA: 0x0007D0AC File Offset: 0x0007B2AC
		internal void AutoComputeReinforcementSpawnDurations()
		{
			foreach (NavalShipAgents navalShipAgents in this._allShipAgents)
			{
				navalShipAgents.SetReinforcementSpawnDuration(0f);
			}
		}

		// Token: 0x060010C0 RID: 4288 RVA: 0x0007D104 File Offset: 0x0007B304
		internal void ClearRecentlySwappedAgents()
		{
			this._recentlySwappedAgents.Clear();
		}

		// Token: 0x060010C1 RID: 4289 RVA: 0x0007D114 File Offset: 0x0007B314
		internal void OnAgentRemoved(Agent agent)
		{
			NavalShipAgents navalShipAgents;
			if (this._agentToShipAgents.TryGetValue(agent, out navalShipAgents))
			{
				this.RemoveAgentAux(agent, navalShipAgents);
				this.RemoveTroopOriginAux(agent.Origin);
			}
		}

		// Token: 0x060010C2 RID: 4290 RVA: 0x0007D148 File Offset: 0x0007B348
		internal void OnShipSpawned(MissionShip ship, bool ignoreTroopCapacities)
		{
			NavalShipAgents navalShipAgents;
			this.TryGetShipAgents(ship, out navalShipAgents);
			navalShipAgents = new NavalShipAgents(ship, this);
			navalShipAgents.SetIgnoreCapacityChecks(ignoreTroopCapacities);
			this._allShipAgents.Add(navalShipAgents);
		}

		// Token: 0x060010C3 RID: 4291 RVA: 0x0007D17C File Offset: 0x0007B37C
		internal void OnShipRemoved(MissionShip ship)
		{
			NavalShipAgents navalShipAgents;
			if (this.TryGetShipAgents(ship, out navalShipAgents))
			{
				if (this.AgentsLogic.IsDeploymentMode && !this.AgentsLogic.IsMissionEnding)
				{
					while (navalShipAgents.ActiveAgents.Count > 0)
					{
						Agent agent = navalShipAgents.ActiveAgents.Last<Agent>();
						this.UnassignAgentAux(navalShipAgents, agent);
					}
					while (navalShipAgents.ReservedTroopsCount > 0)
					{
						NavalTroopAssignment navalTroopAssignment = this.DequeueReservedTroop(navalShipAgents);
						this.EnqueueUnassignedTroop(in navalTroopAssignment);
					}
				}
				else
				{
					while (navalShipAgents.ActiveAgents.Count > 0)
					{
						Agent agent2 = navalShipAgents.ActiveAgents.Last<Agent>();
						this.RemoveAgentAux(agent2, navalShipAgents);
						this.RemoveTroopOriginAux(agent2.Origin);
						if (agent2 != Agent.Main)
						{
							agent2.FadeOut(true, true);
						}
					}
					while (navalShipAgents.ReservedTroopsCount > 0)
					{
						NavalTroopAssignment navalTroopAssignment2 = this.DequeueReservedTroop(navalShipAgents);
						this.RemoveTroopOriginAux(navalTroopAssignment2.Origin);
					}
				}
				this._allShipAgents.RemoveAll((NavalShipAgents sAgentsData) => sAgentsData.Ship == ship);
			}
		}

		// Token: 0x060010C4 RID: 4292 RVA: 0x0007D284 File Offset: 0x0007B484
		internal void OnShipCaptured(MissionShip ship, MissionShip ship2)
		{
			NavalShipAgents navalShipAgents;
			this.TryGetShipAgents(ship, out navalShipAgents);
			navalShipAgents.OnShipCaptured(ship2);
		}

		// Token: 0x060010C5 RID: 4293 RVA: 0x0007D2A4 File Offset: 0x0007B4A4
		internal void OnShipTransferredToFormation(MissionShip ship, Formation oldFormation)
		{
			NavalShipAgents navalShipAgents;
			this.TryGetShipAgents(ship, out navalShipAgents);
			foreach (Agent agent in navalShipAgents.ActiveAgents)
			{
				bool flag = agent == oldFormation.Captain;
				this.SetManagedAgentFormation(agent, ship.Formation);
				if (flag)
				{
					this.SetManagedCaptainOfFormation(agent, ship.Formation);
				}
			}
		}

		// Token: 0x060010C6 RID: 4294 RVA: 0x0007D320 File Offset: 0x0007B520
		internal void OnEndDeploymentMode()
		{
			int num = 0;
			while (this.NumberOfUnassignedTroops > 0)
			{
				NavalTroopAssignment navalTroopAssignment;
				this.DequeueUnassignedTroop(out navalTroopAssignment);
				IAgentOriginBase origin = navalTroopAssignment.Origin;
				if (navalTroopAssignment.HasAgent)
				{
					navalTroopAssignment.Agent.FadeOut(true, true);
					num++;
				}
				this.RemoveTroopOriginAux(origin);
			}
			foreach (KeyValuePair<Agent, MissionShip> keyValuePair in this._unassignedReservedAgents)
			{
				keyValuePair.Key.FadeOut(true, true);
				num++;
			}
			this._unassignedReservedAgents.Clear();
			this.NumberOfSpawnedAgents -= num;
			foreach (NavalShipAgents navalShipAgents in this._allShipAgents)
			{
				navalShipAgents.OnEndDeploymentMode();
			}
		}

		// Token: 0x060010C7 RID: 4295 RVA: 0x0007D418 File Offset: 0x0007B618
		internal void SetManagedAgentFormation(Agent agent, Formation formation)
		{
			Formation formation2 = agent.Formation;
			if (formation2 != formation)
			{
				if (formation2 != null && formation2.Captain == agent)
				{
					this.SetManagedCaptainOfFormation(null, formation2);
				}
				agent.Formation = formation;
			}
		}

		// Token: 0x060010C8 RID: 4296 RVA: 0x0007D44B File Offset: 0x0007B64B
		internal void SetManagedCaptainOfFormation(Agent captain, Formation formation)
		{
			if (formation.Captain != captain)
			{
				formation.Captain = captain;
			}
		}

		// Token: 0x060010C9 RID: 4297 RVA: 0x0007D460 File Offset: 0x0007B660
		internal void AddTroopOrigin(IAgentOriginBase origin)
		{
			this.AddTroopOriginAux(origin);
			NavalTroopAssignment navalTroopAssignment = NavalTroopAssignment.Create(origin, null);
			this.EnqueueUnassignedTroop(in navalTroopAssignment);
		}

		// Token: 0x060010CA RID: 4298 RVA: 0x0007D484 File Offset: 0x0007B684
		internal bool SpawnExistingHero(IAgentOriginBase heroOrigin, MissionShip ship, out Agent spawnedHero)
		{
			spawnedHero = null;
			Agent agent;
			MissionShip missionShip;
			if (this.IsAgentOnAnyShip(heroOrigin, out agent, out missionShip))
			{
				return false;
			}
			NavalShipAgents navalShipAgents;
			this.TryGetShipAgents(ship, out navalShipAgents);
			if (this.AgentsLogic.IsDeploymentMode)
			{
				this.MakeSpaceForOneAgent(navalShipAgents, true);
			}
			NavalTroopAssignment navalTroopAssignment;
			bool flag = this._unassignedTroops.TryGetValue(heroOrigin, out navalTroopAssignment);
			if (flag && navalTroopAssignment.HasAgent)
			{
				spawnedHero = this.ReassignAgentAux(navalShipAgents, navalTroopAssignment.Agent);
			}
			else
			{
				NavalTroopAssignment navalTroopAssignment2 = NavalTroopAssignment.Invalid();
				bool flag2;
				if (flag)
				{
					this.DequeueUnassignedTroop(navalTroopAssignment.Origin, out navalTroopAssignment2);
					flag2 = true;
				}
				else
				{
					NavalShipAgents navalShipAgents2 = null;
					foreach (NavalShipAgents navalShipAgents3 in this._allShipAgents)
					{
						if (navalShipAgents3.IsOriginInReserves(heroOrigin))
						{
							navalShipAgents2 = navalShipAgents3;
							break;
						}
					}
					this.DequeueReservedTroop(heroOrigin, navalShipAgents2, out navalTroopAssignment2);
					if (navalShipAgents2 != navalShipAgents)
					{
						NavalTroopAssignment navalTroopAssignment3;
						if (navalShipAgents.ReservedTroopsCount > 0)
						{
							this.TransferReservedTroop(navalShipAgents, navalShipAgents2, null);
						}
						else if (this.AgentsLogic.IsDeploymentMode && this.DequeueUnassignedTroop(out navalTroopAssignment3))
						{
							this.EnqueueReservedTroop(in navalTroopAssignment3, navalShipAgents2);
						}
					}
					flag2 = true;
				}
				if (flag2)
				{
					this.EnqueueReservedTroop(in navalTroopAssignment2, navalShipAgents);
					bool flag3;
					spawnedHero = navalShipAgents.SpawnHeroFromReserve(heroOrigin, out flag3);
					this._agentToShipAgents[spawnedHero] = navalShipAgents;
					if (!flag3)
					{
						int numberOfSpawnedAgents = this.NumberOfSpawnedAgents;
						this.NumberOfSpawnedAgents = numberOfSpawnedAgents + 1;
					}
				}
			}
			return spawnedHero != null;
		}

		// Token: 0x060010CB RID: 4299 RVA: 0x0007D5F4 File Offset: 0x0007B7F4
		internal void AssignAndTeleportCrewToShipMachines(MissionShip targetShip)
		{
			NavalShipAgents navalShipAgents;
			this.TryGetShipAgents(targetShip, out navalShipAgents);
			navalShipAgents.AssignAndTeleportCrewToShipMachines();
		}

		// Token: 0x060010CC RID: 4300 RVA: 0x0007D614 File Offset: 0x0007B814
		internal void AssignAndTeleportCrewToShipMachines()
		{
			foreach (NavalShipAgents navalShipAgents in this._allShipAgents)
			{
				navalShipAgents.AssignAndTeleportCrewToShipMachines();
			}
		}

		// Token: 0x060010CD RID: 4301 RVA: 0x0007D664 File Offset: 0x0007B864
		internal void UnassignTroops()
		{
			this.UnassignIncompatibleTroops();
			this.UnassignExcessTroopsFromShips();
		}

		// Token: 0x060010CE RID: 4302 RVA: 0x0007D674 File Offset: 0x0007B874
		internal void SetTroopTraitsFilter(MissionShip ship, TroopTraitsMask troopTraitsFilter)
		{
			NavalShipAgents navalShipAgents;
			this.TryGetShipAgents(ship, out navalShipAgents);
			navalShipAgents.SetTroopTraitsFilter(troopTraitsFilter);
		}

		// Token: 0x060010CF RID: 4303 RVA: 0x0007D694 File Offset: 0x0007B894
		private void UnassignIncompatibleTroops()
		{
			foreach (NavalShipAgents navalShipAgents in this._allShipAgents)
			{
				foreach (Agent agent in navalShipAgents.ActiveAgents)
				{
					if (!navalShipAgents.IsAgentCompatibleWithShip(agent, false))
					{
						this._tempIncompatibleAgentsList.Add(agent);
						agent.Formation.OnBatchUnitRemovalStart();
					}
				}
				foreach (Agent agent2 in this._tempIncompatibleAgentsList)
				{
					this.UnassignAgentAux(navalShipAgents, agent2);
				}
				foreach (Team team in Mission.Current.Teams)
				{
					foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty)
					{
						formation.OnBatchUnitRemovalEnd();
					}
				}
				this._tempIncompatibleAgentsList.Clear();
				foreach (NavalTroopAssignment navalTroopAssignment in navalShipAgents.ReservedTroops)
				{
					IAgentOriginBase origin = navalTroopAssignment.Origin;
					if (!navalShipAgents.IsTroopCompatibleWithShip(origin))
					{
						this._tempIncompatibleReservesList.Add(origin);
					}
				}
				foreach (IAgentOriginBase agentOriginBase in this._tempIncompatibleReservesList)
				{
					NavalTroopAssignment navalTroopAssignment2;
					this.DequeueReservedTroop(agentOriginBase, navalShipAgents, out navalTroopAssignment2);
					this.EnqueueUnassignedTroop(in navalTroopAssignment2);
				}
				this._tempIncompatibleReservesList.Clear();
			}
		}

		// Token: 0x060010D0 RID: 4304 RVA: 0x0007D918 File Offset: 0x0007BB18
		private void UnassignExcessTroopsFromShips()
		{
			int num = 0;
			foreach (NavalShipAgents navalShipAgents in this._allShipAgents)
			{
				num += navalShipAgents.MissingTroopCount;
			}
			int num2 = 0;
			bool flag = true;
			while (num2 < num && flag)
			{
				flag = false;
				float num3 = 0f;
				NavalShipAgents navalShipAgents2 = null;
				foreach (NavalShipAgents navalShipAgents3 in this._allShipAgents)
				{
					if (navalShipAgents3.TroopFillRatio >= num3)
					{
						num3 = navalShipAgents3.TroopFillRatio;
						navalShipAgents2 = navalShipAgents3;
					}
				}
				if (navalShipAgents2 != null)
				{
					if (navalShipAgents2.ActiveAgents.Count > 0)
					{
						Agent agent = null;
						if (!Extensions.IsEmpty<Agent>(navalShipAgents2.ActiveNonHeroAgents))
						{
							agent = Extensions.MinBy<Agent, float>(navalShipAgents2.ActiveNonHeroAgents, (Agent a2) => NavalAgentsLogic.GetAgentPriority(a2));
						}
						if (agent == null)
						{
							agent = Extensions.MinBy<Agent, float>(navalShipAgents2.ActiveHeroAgents, (Agent a) => NavalAgentsLogic.GetAgentPriority(a));
						}
						if (!agent.IsMainAgent && !agent.IsPlayerTroop && agent != navalShipAgents2.Ship.Formation.Captain)
						{
							this.UnassignAgentAux(navalShipAgents2, agent);
							num2++;
							flag = true;
						}
					}
					if (!flag && navalShipAgents2.ReservedTroopsCount > 0)
					{
						NavalTroopAssignment navalTroopAssignment = this.DequeueReservedTroop(navalShipAgents2);
						this.EnqueueUnassignedTroop(in navalTroopAssignment);
						num2++;
						flag = true;
					}
				}
			}
		}

		// Token: 0x060010D1 RID: 4305 RVA: 0x0007DACC File Offset: 0x0007BCCC
		internal void SetTroopClassFilter(MissionShip ship, TroopTraitsMask troopClassFilter)
		{
			NavalShipAgents navalShipAgents;
			this.TryGetShipAgents(ship, out navalShipAgents);
			navalShipAgents.SetTroopClassFilter(troopClassFilter);
		}

		// Token: 0x060010D2 RID: 4306 RVA: 0x0007DAEA File Offset: 0x0007BCEA
		private void AddTroopOriginAux(IAgentOriginBase troopOrigin)
		{
			this._allTroopOrigins.Add(troopOrigin);
			if (troopOrigin.Troop.IsHero)
			{
				this._allHeroOrigins.Add(troopOrigin);
			}
		}

		// Token: 0x060010D3 RID: 4307 RVA: 0x0007DB13 File Offset: 0x0007BD13
		public void RemoveTroopOriginAux(IAgentOriginBase troopOrigin)
		{
			this._allTroopOrigins.Remove(troopOrigin);
			if (troopOrigin.Troop.IsHero)
			{
				this._allHeroOrigins.Remove(troopOrigin);
			}
		}

		// Token: 0x060010D4 RID: 4308 RVA: 0x0007DB3C File Offset: 0x0007BD3C
		private bool AddReservedTroopToShipAux(IAgentOriginBase agentOrigin, NavalShipAgents shipAgentsData)
		{
			if (shipAgentsData.IsOriginInReserves(agentOrigin))
			{
				return true;
			}
			if (this.AgentsLogic.IsDeploymentMode || shipAgentsData.CanAddMoreReserves)
			{
				NavalTroopAssignment navalTroopAssignment = NavalTroopAssignment.Invalid();
				if (shipAgentsData.CanAddMoreReserves && !this._allTroopOrigins.Contains(agentOrigin))
				{
					this.AddTroopOriginAux(agentOrigin);
					navalTroopAssignment = NavalTroopAssignment.Create(agentOrigin, null);
				}
				else if (this.AgentsLogic.IsDeploymentMode)
				{
					this.DequeueUnassignedTroop(agentOrigin, out navalTroopAssignment);
				}
				if (navalTroopAssignment.IsValid)
				{
					this.EnqueueReservedTroop(in navalTroopAssignment, shipAgentsData);
					return true;
				}
			}
			return false;
		}

		// Token: 0x060010D5 RID: 4309 RVA: 0x0007DBC4 File Offset: 0x0007BDC4
		private bool RemoveReservedTroopFromShipAux(NavalShipAgents shipAgentsData)
		{
			if (shipAgentsData.ReservedTroopsCount > 0)
			{
				NavalTroopAssignment navalTroopAssignment = this.DequeueReservedTroop(shipAgentsData);
				if (this.AgentsLogic.IsDeploymentMode)
				{
					this.EnqueueUnassignedTroop(in navalTroopAssignment);
				}
				else
				{
					this.RemoveTroopOriginAux(navalTroopAssignment.Origin);
				}
				return true;
			}
			return false;
		}

		// Token: 0x060010D6 RID: 4310 RVA: 0x0007DC08 File Offset: 0x0007BE08
		private void UpdateTemporaryShipsWithMissingTroopsAux(int shipIndex, NavalShipAgents shipAgentsData)
		{
			if (shipAgentsData.HasMissingTroops)
			{
				int num = shipIndex;
				while (num > 0 && this._tempShipsWithMissingTroops[num - 1].TroopFillRatio < shipAgentsData.TroopFillRatio)
				{
					this._tempShipsWithMissingTroops[num] = this._tempShipsWithMissingTroops[num - 1];
					num--;
				}
				if (num != shipIndex)
				{
					this._tempShipsWithMissingTroops[num] = shipAgentsData;
					return;
				}
			}
			else
			{
				this._tempShipsWithMissingTroops.RemoveAt(shipIndex);
			}
		}

		// Token: 0x060010D7 RID: 4311 RVA: 0x0007DC7C File Offset: 0x0007BE7C
		private bool TryGetShipAgents(MissionShip ship, out NavalShipAgents shipAgents)
		{
			shipAgents = null;
			foreach (NavalShipAgents navalShipAgents in this._allShipAgents)
			{
				if (navalShipAgents.Ship == ship)
				{
					shipAgents = navalShipAgents;
					return true;
				}
			}
			return false;
		}

		// Token: 0x060010D8 RID: 4312 RVA: 0x0007DCE0 File Offset: 0x0007BEE0
		private void EnqueueUnassignedTroop(in NavalTroopAssignment troop)
		{
			this._unassignedTroops.Add(troop.Origin, troop);
			MBSortedMultiList<int, NavalTroopAssignment> unassignedOrderedTroops = this._unassignedOrderedTroops;
			NavalTroopAssignment navalTroopAssignment = troop;
			unassignedOrderedTroops.Add(navalTroopAssignment.Priority, troop);
			this._unassignedTroopCountData.Add(in troop);
		}

		// Token: 0x060010D9 RID: 4313 RVA: 0x0007DD30 File Offset: 0x0007BF30
		private bool DequeueUnassignedTroop(IAgentOriginBase troopOrigin, out NavalTroopAssignment dequeuedTroop)
		{
			dequeuedTroop = NavalTroopAssignment.Invalid();
			if (this._unassignedOrderedTroops.Count > 0)
			{
				int num = this._unassignedOrderedTroops.FindIndex((KeyValuePair<int, NavalTroopAssignment> tuple) => tuple.Value.Origin == troopOrigin, !troopOrigin.Troop.IsHero);
				if (num >= 0)
				{
					dequeuedTroop = this._unassignedOrderedTroops[num];
					this._unassignedOrderedTroops.RemoveAt(num);
					this._unassignedTroops.Remove(dequeuedTroop.Origin);
					this._unassignedTroopCountData.Remove(in dequeuedTroop);
					return true;
				}
			}
			return false;
		}

		// Token: 0x060010DA RID: 4314 RVA: 0x0007DDD4 File Offset: 0x0007BFD4
		private bool DequeueUnassignedTroop(out NavalTroopAssignment dequeuedTroop)
		{
			dequeuedTroop = NavalTroopAssignment.Invalid();
			if (this._unassignedOrderedTroops.Count > 0)
			{
				dequeuedTroop = this._unassignedOrderedTroops.LastValue;
				this._unassignedOrderedTroops.RemoveLast();
				this._unassignedTroops.Remove(dequeuedTroop.Origin);
				this._unassignedTroopCountData.Remove(in dequeuedTroop);
				return true;
			}
			return false;
		}

		// Token: 0x060010DB RID: 4315 RVA: 0x0007DE38 File Offset: 0x0007C038
		internal void AssignTroops(bool useDynamicTroopTraits = false)
		{
			this._tempShipsWithMissingTroops.Clear();
			using (List<NavalShipAgents>.Enumerator enumerator = this._allShipAgents.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					NavalShipAgents navalShipAgents = enumerator.Current;
					if (navalShipAgents.HasMissingTroops)
					{
						int num = 0;
						while (num < this._tempShipsWithMissingTroops.Count && navalShipAgents.TroopFillRatio < this._tempShipsWithMissingTroops[num].TroopFillRatio)
						{
							num++;
						}
						this._tempShipsWithMissingTroops.Insert(num, navalShipAgents);
					}
				}
				goto IL_0141;
			}
			IL_0081:
			int num2 = -1;
			int num3 = -1;
			NavalTroopAssignment navalTroopAssignment;
			this.DequeueUnassignedTroop(out navalTroopAssignment);
			TroopTraitsMask troopTraitsMask;
			if (useDynamicTroopTraits && navalTroopAssignment.Agent != null)
			{
				troopTraitsMask = navalTroopAssignment.Agent.GetTraitsMask();
			}
			else
			{
				troopTraitsMask = navalTroopAssignment.Origin.GetTraitsMask();
			}
			for (int i = this._tempShipsWithMissingTroops.Count - 1; i >= 0; i--)
			{
				NavalShipAgents navalShipAgents2 = this._tempShipsWithMissingTroops[i];
				if (navalShipAgents2.IsTroopCompatibleWithClassFilter(troopTraitsMask))
				{
					int traitsFilterPriority = navalShipAgents2.GetTraitsFilterPriority(navalTroopAssignment, false);
					if (traitsFilterPriority > num2)
					{
						num2 = traitsFilterPriority;
						num3 = i;
					}
				}
			}
			if (num3 >= 0)
			{
				NavalShipAgents navalShipAgents3 = this._tempShipsWithMissingTroops[num3];
				this.EnqueueReservedTroop(in navalTroopAssignment, navalShipAgents3);
				this.UpdateTemporaryShipsWithMissingTroopsAux(num3, navalShipAgents3);
			}
			else
			{
				this._tempUnassignedTroops.Add(navalTroopAssignment);
			}
			IL_0141:
			if (this.NumberOfUnassignedTroops <= 0)
			{
				int num4 = 0;
				while (num4 < this._tempUnassignedTroops.Count && this._tempShipsWithMissingTroops.Count > 0)
				{
					NavalTroopAssignment navalTroopAssignment2 = this._tempUnassignedTroops[num4];
					int num5 = this._tempShipsWithMissingTroops.Count - 1;
					NavalShipAgents navalShipAgents4 = this._tempShipsWithMissingTroops[num5];
					this.EnqueueReservedTroop(in navalTroopAssignment2, navalShipAgents4);
					this.UpdateTemporaryShipsWithMissingTroopsAux(num5, navalShipAgents4);
					this._tempUnassignedTroops[num4] = NavalTroopAssignment.Invalid();
					num4++;
				}
				if (this._tempUnassignedTroops.Count > 0)
				{
					foreach (NavalTroopAssignment navalTroopAssignment3 in this._tempUnassignedTroops)
					{
						if (navalTroopAssignment3.IsValid)
						{
							this.EnqueueUnassignedTroop(in navalTroopAssignment3);
						}
					}
					this._tempUnassignedTroops.Clear();
				}
				return;
			}
			goto IL_0081;
		}

		// Token: 0x060010DC RID: 4316 RVA: 0x0007E084 File Offset: 0x0007C284
		private bool DequeueUnassignedAgent(out NavalTroopAssignment dequeuedTroop)
		{
			dequeuedTroop = NavalTroopAssignment.Invalid();
			if (this._unassignedOrderedTroops.Count > 0)
			{
				int num = this._unassignedOrderedTroops.FindIndex((KeyValuePair<int, NavalTroopAssignment> tuple) => tuple.Value.HasAgent, false);
				if (num >= 0)
				{
					dequeuedTroop = this._unassignedOrderedTroops[num];
					this._unassignedOrderedTroops.RemoveAt(num);
					this._unassignedTroops.Remove(dequeuedTroop.Origin);
					this._unassignedTroopCountData.Remove(in dequeuedTroop);
					return true;
				}
			}
			return false;
		}

		// Token: 0x060010DD RID: 4317 RVA: 0x0007E11C File Offset: 0x0007C31C
		private void EnqueueReservedTroop(in NavalTroopAssignment troop, NavalShipAgents shipAgentsData)
		{
			shipAgentsData.EnqueueReservedTroop(in troop);
			NavalTroopAssignment navalTroopAssignment = troop;
			if (navalTroopAssignment.HasAgent)
			{
				this._unassignedReservedAgents.Add(troop.Agent, shipAgentsData.Ship);
			}
		}

		// Token: 0x060010DE RID: 4318 RVA: 0x0007E158 File Offset: 0x0007C358
		private bool RemoveReservedTroopFromShipAux(IAgentOriginBase troopOrigin, NavalShipAgents shipAgentsData)
		{
			NavalTroopAssignment navalTroopAssignment;
			if (shipAgentsData.ReservedTroopsCount > 0 && this.DequeueReservedTroop(troopOrigin, shipAgentsData, out navalTroopAssignment))
			{
				if (this.AgentsLogic.IsDeploymentMode)
				{
					this.EnqueueUnassignedTroop(in navalTroopAssignment);
				}
				else
				{
					this.RemoveTroopOriginAux(navalTroopAssignment.Origin);
				}
				return true;
			}
			return false;
		}

		// Token: 0x060010DF RID: 4319 RVA: 0x0007E1A0 File Offset: 0x0007C3A0
		private NavalTroopAssignment DequeueReservedTroop(NavalShipAgents shipAgentsData)
		{
			NavalTroopAssignment navalTroopAssignment;
			shipAgentsData.DequeueReservedTroop(out navalTroopAssignment);
			if (navalTroopAssignment.HasAgent)
			{
				this._unassignedReservedAgents.Remove(navalTroopAssignment.Agent);
			}
			return navalTroopAssignment;
		}

		// Token: 0x060010E0 RID: 4320 RVA: 0x0007E1D2 File Offset: 0x0007C3D2
		private bool DequeueReservedTroop(IAgentOriginBase troopOrigin, NavalShipAgents shipAgentsData, out NavalTroopAssignment dequeuedTroop)
		{
			dequeuedTroop = NavalTroopAssignment.Invalid();
			if (shipAgentsData.DequeueReservedTroop(troopOrigin, out dequeuedTroop))
			{
				if (dequeuedTroop.HasAgent)
				{
					this._unassignedReservedAgents.Remove(dequeuedTroop.Agent);
				}
				return true;
			}
			return false;
		}

		// Token: 0x060010E1 RID: 4321 RVA: 0x0007E208 File Offset: 0x0007C408
		private void TransferReservedTroop(NavalShipAgents fromShipAgentsData, NavalShipAgents toShipAgentsData, IAgentOriginBase troopOrigin = null)
		{
			NavalTroopAssignment navalTroopAssignment = NavalTroopAssignment.Invalid();
			if (troopOrigin != null)
			{
				fromShipAgentsData.DequeueReservedTroop(troopOrigin, out navalTroopAssignment);
			}
			else
			{
				fromShipAgentsData.DequeueReservedTroop(out navalTroopAssignment);
			}
			toShipAgentsData.EnqueueReservedTroop(in navalTroopAssignment);
			if (navalTroopAssignment.HasAgent)
			{
				this._unassignedReservedAgents[navalTroopAssignment.Agent] = toShipAgentsData.Ship;
			}
		}

		// Token: 0x060010E2 RID: 4322 RVA: 0x0007E25C File Offset: 0x0007C45C
		private void UnassignAgentAux(NavalShipAgents shipAgentsData, Agent agent)
		{
			this.RemoveAgentAux(agent, shipAgentsData);
			MissionShip ship = shipAgentsData.Ship;
			agent.SetDetachableFromFormation(false);
			agent.SetRenderCheckEnabled(false);
			agent.AgentVisuals.SetVisible(false);
			agent.SetIsPhysicsForceClosed(true);
			AgentNavalComponent component = agent.GetComponent<AgentNavalComponent>();
			agent.RemoveComponent(component);
			AgentNavalAIComponent component2 = agent.GetComponent<AgentNavalAIComponent>();
			agent.RemoveComponent(component2);
			NavalMissionDeploymentPlanningLogic navalMissionDeploymentPlanningLogic;
			Mission.Current.GetDeploymentPlan<NavalMissionDeploymentPlanningLogic>(ref navalMissionDeploymentPlanningLogic);
			Vec2 vec;
			navalMissionDeploymentPlanningLogic.GetMeanBoundaryPosition(agent.Team, out vec, 0);
			agent.TeleportToPosition(vec.ToVec3(500f));
			NavalTroopAssignment navalTroopAssignment = NavalTroopAssignment.Create(agent.Origin, agent);
			this.EnqueueUnassignedTroop(in navalTroopAssignment);
		}

		// Token: 0x060010E3 RID: 4323 RVA: 0x0007E2FC File Offset: 0x0007C4FC
		internal Agent ReassignAgentAux(NavalShipAgents shipAgentsData, Agent agent = null)
		{
			NavalTroopAssignment navalTroopAssignment = NavalTroopAssignment.Invalid();
			if (agent == null)
			{
				this.DequeueUnassignedAgent(out navalTroopAssignment);
			}
			else if (this._unassignedReservedAgents.ContainsKey(agent))
			{
				this._unassignedReservedAgents.Remove(agent);
				navalTroopAssignment = NavalTroopAssignment.Create(agent.Origin, agent);
			}
			else
			{
				this.DequeueUnassignedTroop(agent.Origin, out navalTroopAssignment);
			}
			agent = navalTroopAssignment.Agent;
			AgentNavalComponent agentNavalComponent = agent.GetComponent<AgentNavalComponent>();
			AgentNavalAIComponent agentNavalAIComponent = agent.GetComponent<AgentNavalAIComponent>();
			agentNavalComponent = new AgentNavalComponent(agent);
			agent.AddComponent(agentNavalComponent);
			agentNavalAIComponent = new AgentNavalAIComponent(agent);
			agent.AddComponent(agentNavalAIComponent);
			agentNavalComponent.Initialize();
			agent.AgentVisuals.SetVisible(true);
			agent.SetRenderCheckEnabled(true);
			agent.SetIsPhysicsForceClosed(false);
			if (!agent.IsPlayerTroop)
			{
				agent.SetDetachableFromFormation(true);
			}
			this.AddAgentAux(agent, shipAgentsData);
			return agent;
		}

		// Token: 0x060010E4 RID: 4324 RVA: 0x0007E3C0 File Offset: 0x0007C5C0
		internal void SetRestrictRecentlySwappedAgentTransfers(bool value)
		{
			if (this.RestrictRecentlySwappedAgentTransfers && !value)
			{
				this.ClearRecentlySwappedAgents();
			}
			this.RestrictRecentlySwappedAgentTransfers = value;
		}

		// Token: 0x060010E5 RID: 4325 RVA: 0x0007E3DA File Offset: 0x0007C5DA
		private void AddAgentAux(Agent agent, NavalShipAgents shipAgentsData)
		{
			shipAgentsData.AddAgent(agent);
			this._agentToShipAgents[agent] = shipAgentsData;
		}

		// Token: 0x060010E6 RID: 4326 RVA: 0x0007E3F0 File Offset: 0x0007C5F0
		private void RemoveAgentAux(Agent agent, NavalShipAgents targetShipAgentsData)
		{
			targetShipAgentsData.RemoveAgent(agent);
			this._agentToShipAgents.Remove(agent);
			if (this._recentlySwappedAgents.Count > 0)
			{
				this._recentlySwappedAgents.Remove(agent);
			}
		}

		// Token: 0x060010E7 RID: 4327 RVA: 0x0007E421 File Offset: 0x0007C621
		private void TransferAgentAux(Agent agent, NavalShipAgents originShipAgentsData, NavalShipAgents targetShipAgentsData)
		{
			if (originShipAgentsData != null)
			{
				originShipAgentsData.RemoveAgent(agent);
			}
			targetShipAgentsData.AddAgent(agent);
			this._agentToShipAgents[agent] = targetShipAgentsData;
		}

		// Token: 0x060010E8 RID: 4328 RVA: 0x0007E444 File Offset: 0x0007C644
		private void MakeSpaceForOneAgent(NavalShipAgents shipAgentsData, bool ignorePlayerTroop = true)
		{
			while (shipAgentsData.MissingAgentCountOnMainDeck == 0 && shipAgentsData.ActiveAgents.Count > 0)
			{
				Agent minimumPriorityActiveAgent = shipAgentsData.GetMinimumPriorityActiveAgent(null);
				if (ignorePlayerTroop && minimumPriorityActiveAgent.IsPlayerTroop)
				{
					break;
				}
				this.UnassignAgentAux(shipAgentsData, minimumPriorityActiveAgent);
			}
			this.MakeSpaceInReserves(shipAgentsData);
		}

		// Token: 0x060010E9 RID: 4329 RVA: 0x0007E48B File Offset: 0x0007C68B
		private void MakeSpaceInReserves(NavalShipAgents shipAgentsData)
		{
			while (shipAgentsData.MissingTroopCount == 0 && shipAgentsData.ReservedTroopsCount > 0)
			{
				this.RemoveReservedTroopFromShipAux(shipAgentsData);
			}
		}

		// Token: 0x040009C0 RID: 2496
		internal readonly BattleSideEnum BattleSide;

		// Token: 0x040009C1 RID: 2497
		internal readonly TeamSideEnum TeamSide;

		// Token: 0x040009C2 RID: 2498
		internal readonly NavalAgentsLogic AgentsLogic;

		// Token: 0x040009C6 RID: 2502
		private readonly HashSet<IAgentOriginBase> _allTroopOrigins;

		// Token: 0x040009C7 RID: 2503
		private readonly HashSet<IAgentOriginBase> _allHeroOrigins;

		// Token: 0x040009C8 RID: 2504
		private readonly MBList<NavalShipAgents> _allShipAgents;

		// Token: 0x040009C9 RID: 2505
		private readonly Dictionary<IAgentOriginBase, NavalTroopAssignment> _unassignedTroops;

		// Token: 0x040009CA RID: 2506
		private readonly Dictionary<Agent, NavalShipAgents> _agentToShipAgents;

		// Token: 0x040009CB RID: 2507
		private readonly MBSortedMultiList<int, NavalTroopAssignment> _unassignedOrderedTroops;

		// Token: 0x040009CC RID: 2508
		private NavalTeamAgents.TroopCountData _unassignedTroopCountData;

		// Token: 0x040009CD RID: 2509
		private readonly Dictionary<Agent, MissionShip> _unassignedReservedAgents;

		// Token: 0x040009CE RID: 2510
		private MBList<Agent> _tempSpawnedAgentsList;

		// Token: 0x040009CF RID: 2511
		private MBList<NavalTroopAssignment> _tempUnassignedTroops;

		// Token: 0x040009D0 RID: 2512
		private MBList<NavalShipAgents> _tempShipsWithMissingTroops;

		// Token: 0x040009D1 RID: 2513
		private MBList<Agent> _tempIncompatibleAgentsList;

		// Token: 0x040009D2 RID: 2514
		private MBList<IAgentOriginBase> _tempIncompatibleReservesList;

		// Token: 0x040009D3 RID: 2515
		private MBList<Agent> _tempAgentsNotUsingMachines;

		// Token: 0x040009D4 RID: 2516
		private MBList<Agent> _recentlySwappedAgents = new MBList<Agent>();

		// Token: 0x02000258 RID: 600
		private struct TroopCountData
		{
			// Token: 0x1700041C RID: 1052
			// (get) Token: 0x06001BBD RID: 7101 RVA: 0x000B8DC0 File Offset: 0x000B6FC0
			public int NonHeroOriginsCount
			{
				get
				{
					return this._nonHeroOriginsCount;
				}
			}

			// Token: 0x1700041D RID: 1053
			// (get) Token: 0x06001BBE RID: 7102 RVA: 0x000B8DC8 File Offset: 0x000B6FC8
			public int HeroOriginsCount
			{
				get
				{
					return this._heroOriginsCount;
				}
			}

			// Token: 0x1700041E RID: 1054
			// (get) Token: 0x06001BBF RID: 7103 RVA: 0x000B8DD0 File Offset: 0x000B6FD0
			public int NonHeroAgentsCount
			{
				get
				{
					return this._nonHeroAgentsCount;
				}
			}

			// Token: 0x1700041F RID: 1055
			// (get) Token: 0x06001BC0 RID: 7104 RVA: 0x000B8DD8 File Offset: 0x000B6FD8
			public int HeroAgentsCount
			{
				get
				{
					return this._heroAgentsCount;
				}
			}

			// Token: 0x17000420 RID: 1056
			// (get) Token: 0x06001BC1 RID: 7105 RVA: 0x000B8DE0 File Offset: 0x000B6FE0
			public int OriginsCount
			{
				get
				{
					return this._nonHeroOriginsCount + this._heroOriginsCount;
				}
			}

			// Token: 0x17000421 RID: 1057
			// (get) Token: 0x06001BC2 RID: 7106 RVA: 0x000B8DEF File Offset: 0x000B6FEF
			public int AgentsCount
			{
				get
				{
					return this._nonHeroAgentsCount + this._heroAgentsCount;
				}
			}

			// Token: 0x06001BC3 RID: 7107 RVA: 0x000B8E00 File Offset: 0x000B7000
			public void Add(in NavalTroopAssignment troop)
			{
				NavalTroopAssignment navalTroopAssignment = troop;
				if (navalTroopAssignment.HasAgent)
				{
					if (troop.Agent.IsHero)
					{
						this._heroAgentsCount++;
						return;
					}
					this._nonHeroAgentsCount++;
					return;
				}
				else
				{
					if (troop.Origin.Troop.IsHero)
					{
						this._heroOriginsCount++;
						return;
					}
					this._nonHeroOriginsCount++;
					return;
				}
			}

			// Token: 0x06001BC4 RID: 7108 RVA: 0x000B8E78 File Offset: 0x000B7078
			public void Remove(in NavalTroopAssignment troop)
			{
				NavalTroopAssignment navalTroopAssignment = troop;
				if (navalTroopAssignment.HasAgent)
				{
					if (troop.Agent.IsHero)
					{
						this._heroAgentsCount--;
						return;
					}
					this._nonHeroAgentsCount--;
					return;
				}
				else
				{
					if (troop.Origin.Troop.IsHero)
					{
						this._heroOriginsCount--;
						return;
					}
					this._nonHeroOriginsCount--;
					return;
				}
			}

			// Token: 0x06001BC5 RID: 7109 RVA: 0x000B8EF0 File Offset: 0x000B70F0
			public bool Equals(in NavalTeamAgents.TroopCountData other)
			{
				int heroOriginsCount = this._heroOriginsCount;
				NavalTeamAgents.TroopCountData troopCountData = other;
				if (heroOriginsCount == troopCountData.HeroOriginsCount)
				{
					int nonHeroOriginsCount = this._nonHeroOriginsCount;
					troopCountData = other;
					if (nonHeroOriginsCount == troopCountData.NonHeroOriginsCount)
					{
						int heroAgentsCount = this._heroAgentsCount;
						troopCountData = other;
						if (heroAgentsCount == troopCountData.HeroAgentsCount)
						{
							int nonHeroAgentsCount = this._nonHeroAgentsCount;
							troopCountData = other;
							return nonHeroAgentsCount == troopCountData.NonHeroAgentsCount;
						}
					}
				}
				return false;
			}

			// Token: 0x04001065 RID: 4197
			private int _nonHeroOriginsCount;

			// Token: 0x04001066 RID: 4198
			private int _heroOriginsCount;

			// Token: 0x04001067 RID: 4199
			private int _nonHeroAgentsCount;

			// Token: 0x04001068 RID: 4200
			private int _heroAgentsCount;
		}
	}
}
