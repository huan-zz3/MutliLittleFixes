using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.Objects.UsableMachines;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.MissionLogics
{
	// Token: 0x020000D2 RID: 210
	internal class NavalShipAgents
	{
		// Token: 0x170002D0 RID: 720
		// (get) Token: 0x06000FC1 RID: 4033 RVA: 0x00078987 File Offset: 0x00076B87
		// (set) Token: 0x06000FC2 RID: 4034 RVA: 0x0007898F File Offset: 0x00076B8F
		public TroopTraitsMask TroopClassFilter { get; private set; }

		// Token: 0x170002D1 RID: 721
		// (get) Token: 0x06000FC3 RID: 4035 RVA: 0x00078998 File Offset: 0x00076B98
		// (set) Token: 0x06000FC4 RID: 4036 RVA: 0x000789A0 File Offset: 0x00076BA0
		public TroopTraitsMask TroopTraitsFilter { get; private set; }

		// Token: 0x170002D2 RID: 722
		// (get) Token: 0x06000FC5 RID: 4037 RVA: 0x000789A9 File Offset: 0x00076BA9
		internal MBReadOnlyList<Agent> ActiveAgents
		{
			get
			{
				return this._activeAgents;
			}
		}

		// Token: 0x170002D3 RID: 723
		// (get) Token: 0x06000FC6 RID: 4038 RVA: 0x000789B1 File Offset: 0x00076BB1
		internal MBReadOnlyList<Agent> ActiveHeroAgents
		{
			get
			{
				return this._activeHeroAgents;
			}
		}

		// Token: 0x170002D4 RID: 724
		// (get) Token: 0x06000FC7 RID: 4039 RVA: 0x000789B9 File Offset: 0x00076BB9
		internal MBReadOnlyList<Agent> ActiveNonHeroAgents
		{
			get
			{
				return this._activeNonHeroAgents;
			}
		}

		// Token: 0x170002D5 RID: 725
		// (get) Token: 0x06000FC8 RID: 4040 RVA: 0x000789C1 File Offset: 0x00076BC1
		internal MBSortedMultiList<int, NavalTroopAssignment> ReservedTroops
		{
			get
			{
				return this._reservedOrderedTroops;
			}
		}

		// Token: 0x170002D6 RID: 726
		// (get) Token: 0x06000FC9 RID: 4041 RVA: 0x000789C9 File Offset: 0x00076BC9
		// (set) Token: 0x06000FCA RID: 4042 RVA: 0x000789D1 File Offset: 0x00076BD1
		internal int ReservedHeroesCount { get; private set; }

		// Token: 0x170002D7 RID: 727
		// (get) Token: 0x06000FCB RID: 4043 RVA: 0x000789DA File Offset: 0x00076BDA
		internal int ReservedNonHeroesCount
		{
			get
			{
				return this._reservedTroops.Count - this.ReservedHeroesCount;
			}
		}

		// Token: 0x170002D8 RID: 728
		// (get) Token: 0x06000FCC RID: 4044 RVA: 0x000789EE File Offset: 0x00076BEE
		internal int ReservedTroopsCount
		{
			get
			{
				return this._reservedTroops.Count;
			}
		}

		// Token: 0x170002D9 RID: 729
		// (get) Token: 0x06000FCD RID: 4045 RVA: 0x000789FB File Offset: 0x00076BFB
		internal int AllTroopsCount
		{
			get
			{
				return this._reservedTroops.Count + this._activeAgents.Count;
			}
		}

		// Token: 0x170002DA RID: 730
		// (get) Token: 0x06000FCE RID: 4046 RVA: 0x00078A14 File Offset: 0x00076C14
		// (set) Token: 0x06000FCF RID: 4047 RVA: 0x00078A1C File Offset: 0x00076C1C
		internal MissionShip Ship { get; private set; }

		// Token: 0x170002DB RID: 731
		// (get) Token: 0x06000FD0 RID: 4048 RVA: 0x00078A25 File Offset: 0x00076C25
		internal bool CanAddMoreReserves
		{
			get
			{
				return this.IgnoreCapacityChecks || this.HasMissingTroops;
			}
		}

		// Token: 0x170002DC RID: 732
		// (get) Token: 0x06000FD1 RID: 4049 RVA: 0x00078A37 File Offset: 0x00076C37
		internal bool CanAddMoreAgents
		{
			get
			{
				return this.IgnoreCapacityChecks || (this.HasMissingAgentsOnMainDeck && this.HasMissingTroops);
			}
		}

		// Token: 0x170002DD RID: 733
		// (get) Token: 0x06000FD2 RID: 4050 RVA: 0x00078A53 File Offset: 0x00076C53
		// (set) Token: 0x06000FD3 RID: 4051 RVA: 0x00078A5B File Offset: 0x00076C5B
		internal bool SpawnReinforcements { get; private set; } = true;

		// Token: 0x170002DE RID: 734
		// (get) Token: 0x06000FD4 RID: 4052 RVA: 0x00078A64 File Offset: 0x00076C64
		// (set) Token: 0x06000FD5 RID: 4053 RVA: 0x00078A6C File Offset: 0x00076C6C
		internal bool IgnoreCapacityChecks { get; private set; }

		// Token: 0x170002DF RID: 735
		// (get) Token: 0x06000FD6 RID: 4054 RVA: 0x00078A75 File Offset: 0x00076C75
		internal bool HasPlayerAgent
		{
			get
			{
				return Agent.Main != null && this._activeHeroAgents.Contains(Agent.Main);
			}
		}

		// Token: 0x170002E0 RID: 736
		// (get) Token: 0x06000FD7 RID: 4055 RVA: 0x00078A90 File Offset: 0x00076C90
		// (set) Token: 0x06000FD8 RID: 4056 RVA: 0x00078A98 File Offset: 0x00076C98
		internal int DesiredTroopCount { get; private set; }

		// Token: 0x170002E1 RID: 737
		// (get) Token: 0x06000FD9 RID: 4057 RVA: 0x00078AA1 File Offset: 0x00076CA1
		internal bool HasMissingTroops
		{
			get
			{
				return this.MissingTroopCount > 0;
			}
		}

		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x06000FDA RID: 4058 RVA: 0x00078AAC File Offset: 0x00076CAC
		internal int MissingTroopCount
		{
			get
			{
				return MathF.Max(0, MathF.Min(this.DesiredTroopCount, this.Ship.TotalCrewCapacity) - this.AllTroopsCount);
			}
		}

		// Token: 0x170002E3 RID: 739
		// (get) Token: 0x06000FDB RID: 4059 RVA: 0x00078AD1 File Offset: 0x00076CD1
		internal int MissingAgentCountOnMainDeck
		{
			get
			{
				return MathF.Max(0, MathF.Min(this.DesiredTroopCount, this.Ship.CrewSizeOnMainDeck) - this.ActiveAgents.Count);
			}
		}

		// Token: 0x170002E4 RID: 740
		// (get) Token: 0x06000FDC RID: 4060 RVA: 0x00078AFB File Offset: 0x00076CFB
		internal bool HasMissingAgentsOnMainDeck
		{
			get
			{
				return this.MissingAgentCountOnMainDeck > 0;
			}
		}

		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x06000FDD RID: 4061 RVA: 0x00078B06 File Offset: 0x00076D06
		internal float TroopFillRatio
		{
			get
			{
				if (this.DesiredTroopCount > 0)
				{
					return (float)this.AllTroopsCount / (float)this.DesiredTroopCount;
				}
				if (this.AllTroopsCount != 0)
				{
					return float.MaxValue;
				}
				return 0f;
			}
		}

		// Token: 0x06000FDE RID: 4062 RVA: 0x00078B34 File Offset: 0x00076D34
		internal NavalShipAgents(MissionShip ship, NavalTeamAgents teamAgents)
		{
			this.Ship = ship;
			this._teamAgents = teamAgents;
			this.DesiredTroopCount = this.Ship.TotalCrewCapacity;
			this._reinforcementTimer = new MissionTimer(0f);
			this.SetTroopClassFilter(3);
			this.SetTroopTraitsFilter(0);
		}

		// Token: 0x06000FDF RID: 4063 RVA: 0x00078BC3 File Offset: 0x00076DC3
		internal void InitializeReinforcementTimer(bool randomizeTimers)
		{
			if (randomizeTimers)
			{
				this._reinforcementTimer.Set(MBRandom.RandomFloat * this._reinforcementTimer.GetTimerDuration());
				return;
			}
			this._reinforcementTimer.Reset();
		}

		// Token: 0x06000FE0 RID: 4064 RVA: 0x00078BF0 File Offset: 0x00076DF0
		internal void SetReinforcementSpawnDuration(float duration = 0f)
		{
			if (duration <= 0f)
			{
				duration = NavalAgentsLogic.ComputeReinforcementSpawnDuration(this.ReservedTroopsCount);
			}
			this._reinforcementTimer.SetDuration(duration);
		}

		// Token: 0x06000FE1 RID: 4065 RVA: 0x00078C13 File Offset: 0x00076E13
		internal void SetIgnoreCapacityChecks(bool value)
		{
			this.IgnoreCapacityChecks = value;
		}

		// Token: 0x06000FE2 RID: 4066 RVA: 0x00078C1C File Offset: 0x00076E1C
		internal void SetSpawnReinforcements(bool value)
		{
			if (this.SpawnReinforcements != value)
			{
				this.SpawnReinforcements = value;
			}
		}

		// Token: 0x06000FE3 RID: 4067 RVA: 0x00078C2E File Offset: 0x00076E2E
		internal void SetTroopClassFilter(TroopTraitsMask troopClassFilter)
		{
			this.TroopClassFilter = troopClassFilter;
		}

		// Token: 0x06000FE4 RID: 4068 RVA: 0x00078C37 File Offset: 0x00076E37
		internal void SetTroopTraitsFilter(TroopTraitsMask troopTraitsFilter)
		{
			this.TroopTraitsFilter = troopTraitsFilter;
			this._compatibilityTraitsFilter = troopTraitsFilter & 65151;
		}

		// Token: 0x06000FE5 RID: 4069 RVA: 0x00078C50 File Offset: 0x00076E50
		internal bool IsAgentCompatibleWithShip(Agent agent, bool checkDynamicCompatibility = false)
		{
			if (agent == this.Ship.Captain || agent.IsPlayerTroop)
			{
				return true;
			}
			TroopTraitsMask troopTraitsMask = (checkDynamicCompatibility ? agent.GetTraitsMask() : agent.Origin.GetTraitsMask());
			return this.IsTroopCompatibleWithClassFilter(troopTraitsMask) && this.IsTroopCompatibleWithTraitsFilter(troopTraitsMask, agent.Character.GetBattleTier());
		}

		// Token: 0x06000FE6 RID: 4070 RVA: 0x00078CAC File Offset: 0x00076EAC
		internal bool IsTroopCompatibleWithShip(IAgentOriginBase troopOrigin)
		{
			if (troopOrigin.Troop.IsPlayerCharacter)
			{
				return true;
			}
			TroopTraitsMask traitsMask = troopOrigin.GetTraitsMask();
			bool flag = this.IsTroopCompatibleWithClassFilter(traitsMask);
			bool flag2 = this.IsTroopCompatibleWithTraitsFilter(traitsMask, troopOrigin.Troop.GetBattleTier());
			return flag && flag2;
		}

		// Token: 0x06000FE7 RID: 4071 RVA: 0x00078CEC File Offset: 0x00076EEC
		internal int GetTraitsFilterPriority(NavalTroopAssignment troopAssignment, bool checkDynamicCompatibility = false)
		{
			int num;
			if (troopAssignment.Agent != null && checkDynamicCompatibility)
			{
				Agent agent = troopAssignment.Agent;
				num = TroopFilteringUtilities.GetTroopPriority(agent.GetTraitsMask(), agent.Origin.Troop.GetBattleTier(), this.TroopTraitsFilter);
			}
			else
			{
				IAgentOriginBase origin = troopAssignment.Origin;
				num = TroopFilteringUtilities.GetTroopPriority(origin.GetTraitsMask(), origin.Troop.GetBattleTier(), this.TroopTraitsFilter);
			}
			return num;
		}

		// Token: 0x06000FE8 RID: 4072 RVA: 0x00078D56 File Offset: 0x00076F56
		internal bool IsTroopCompatibleWithClassFilter(TroopTraitsMask troopClassMask)
		{
			return (troopClassMask & this.TroopClassFilter) > 0;
		}

		// Token: 0x06000FE9 RID: 4073 RVA: 0x00078D64 File Offset: 0x00076F64
		internal bool IsTroopCompatibleWithTraitsFilter(TroopTraitsMask troopTraitsMask, int troopBattleTier)
		{
			if (this.TroopTraitsFilter == null)
			{
				return true;
			}
			if ((troopTraitsMask & this._compatibilityTraitsFilter) == this._compatibilityTraitsFilter)
			{
				float num = (float)troopBattleTier;
				float num2 = 3.5f;
				if ((this.TroopTraitsFilter & 256) != null && num >= num2)
				{
					return true;
				}
				if ((this.TroopTraitsFilter & 128) != null && num <= num2)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000FEA RID: 4074 RVA: 0x00078DC0 File Offset: 0x00076FC0
		public void OnShipCaptured(MissionShip newShip)
		{
			int num = ((newShip != null) ? newShip.TotalCrewCapacity : 0);
			int num2 = ((newShip != null) ? newShip.CrewSizeOnMainDeck : 0);
			this.DesiredTroopCount = ((this.DesiredTroopCount == this.Ship.TotalCrewCapacity) ? num : Math.Min(this.DesiredTroopCount, num));
			this.Ship = newShip;
			int num3 = num - num2;
			while (this.ReservedTroopsCount > num3)
			{
				NavalTroopAssignment navalTroopAssignment;
				this.DequeueReservedTroop(out navalTroopAssignment);
				this._teamAgents.RemoveTroopOriginAux(navalTroopAssignment.Origin);
			}
		}

		// Token: 0x06000FEB RID: 4075 RVA: 0x00078E3F File Offset: 0x0007703F
		internal void SetDesiredTroopCount(int value)
		{
			this.DesiredTroopCount = value;
		}

		// Token: 0x06000FEC RID: 4076 RVA: 0x00078E48 File Offset: 0x00077048
		internal bool IsOriginInReserves(IAgentOriginBase origin)
		{
			return this._reservedTroops.ContainsKey(origin);
		}

		// Token: 0x06000FED RID: 4077 RVA: 0x00078E58 File Offset: 0x00077058
		internal void EnqueueReservedTroop(in NavalTroopAssignment troop)
		{
			MBSortedMultiList<int, NavalTroopAssignment> reservedOrderedTroops = this._reservedOrderedTroops;
			NavalTroopAssignment navalTroopAssignment = troop;
			reservedOrderedTroops.Add(navalTroopAssignment.Priority, troop);
			this._reservedTroops.Add(troop.Origin, troop);
			if (troop.Origin.Troop.IsHero)
			{
				int reservedHeroesCount = this.ReservedHeroesCount;
				this.ReservedHeroesCount = reservedHeroesCount + 1;
			}
			this._teamAgents.AgentsLogic.InvokeTroopAddedToReserves(troop.Origin, this.Ship);
		}

		// Token: 0x06000FEE RID: 4078 RVA: 0x00078EDC File Offset: 0x000770DC
		internal bool DequeueReservedTroop(out NavalTroopAssignment dequeuedTroop)
		{
			dequeuedTroop = NavalTroopAssignment.Invalid();
			if (this._reservedOrderedTroops.Count > 0)
			{
				dequeuedTroop = this._reservedOrderedTroops.LastValue;
				this._reservedOrderedTroops.RemoveLast();
				this._reservedTroops.Remove(dequeuedTroop.Origin);
				if (dequeuedTroop.Origin.Troop.IsHero)
				{
					int reservedHeroesCount = this.ReservedHeroesCount;
					this.ReservedHeroesCount = reservedHeroesCount - 1;
				}
			}
			if (dequeuedTroop.IsValid)
			{
				this._teamAgents.AgentsLogic.InvokeTroopRemovedFromReserves(dequeuedTroop.Origin, this.Ship);
			}
			return dequeuedTroop.IsValid;
		}

		// Token: 0x06000FEF RID: 4079 RVA: 0x00078F7C File Offset: 0x0007717C
		internal bool DequeueReservedTroop(IAgentOriginBase origin, out NavalTroopAssignment dequeuedTroop)
		{
			dequeuedTroop = NavalTroopAssignment.Invalid();
			if (this._reservedTroops.TryGetValue(origin, out dequeuedTroop))
			{
				int priority = NavalTroopAssignment.GetPriority(dequeuedTroop.Origin, dequeuedTroop.Agent);
				int i = this._reservedOrderedTroops.FirstIndexOf(priority);
				while (i <= this._reservedOrderedTroops.Count)
				{
					if (this._reservedOrderedTroops[i].Origin == origin)
					{
						this._reservedOrderedTroops.RemoveAt(i);
						this._reservedTroops.Remove(origin);
						if (origin.Troop.IsHero)
						{
							int reservedHeroesCount = this.ReservedHeroesCount;
							this.ReservedHeroesCount = reservedHeroesCount - 1;
							break;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
			if (dequeuedTroop.IsValid)
			{
				this._teamAgents.AgentsLogic.InvokeTroopRemovedFromReserves(origin, this.Ship);
			}
			return dequeuedTroop.IsValid;
		}

		// Token: 0x06000FF0 RID: 4080 RVA: 0x00079048 File Offset: 0x00077248
		internal void FillReservedTroops(MBList<IAgentOriginBase> reservedTroops)
		{
			foreach (KeyValuePair<IAgentOriginBase, NavalTroopAssignment> keyValuePair in this._reservedTroops)
			{
				reservedTroops.Add(keyValuePair.Key);
			}
		}

		// Token: 0x06000FF1 RID: 4081 RVA: 0x000790A4 File Offset: 0x000772A4
		internal void AddAgent(Agent agent)
		{
			this._teamAgents.SetManagedAgentFormation(agent, this.Ship.Formation);
			this._activeAgents.Add(agent);
			if (agent.IsHero)
			{
				this._activeHeroAgents.Add(agent);
			}
			else
			{
				this._activeNonHeroAgents.Add(agent);
			}
			this._teamAgents.AgentsLogic.InvokeAgentAddedToShip(agent, this.Ship);
		}

		// Token: 0x06000FF2 RID: 4082 RVA: 0x00079110 File Offset: 0x00077310
		internal void RemoveAgent(Agent agent)
		{
			NavalAgentsLogic.TryStopMachineUseAndReattachAgent(agent);
			agent.TryRemoveAllDetachmentScores();
			if (agent.IsHero)
			{
				this._activeHeroAgents.Remove(agent);
			}
			else
			{
				this._activeNonHeroAgents.Remove(agent);
			}
			this._activeAgents.Remove(agent);
			this._teamAgents.SetManagedAgentFormation(agent, null);
			this._teamAgents.AgentsLogic.InvokeAgentRemovedFromShip(agent, this.Ship);
		}

		// Token: 0x06000FF3 RID: 4083 RVA: 0x00079180 File Offset: 0x00077380
		[return: TupleElementNames(new string[] { "spawnedCount", "reassignedCount" })]
		internal ValueTuple<int, int> SpawnNextBatch(bool isReinforcement, MBList<Agent> spawnedAgents)
		{
			int i = 0;
			int num = 0;
			int num2 = MathF.Min(this.ReservedTroopsCount, this.MissingAgentCountOnMainDeck);
			if (isReinforcement && num2 > 0)
			{
				num2 = Math.Min(this.Ship.CrewSpawnLocalFrames.Count, num2);
			}
			if (num2 > 0)
			{
				while (i < num2)
				{
					NavalTroopAssignment navalTroopAssignment;
					this.DequeueReservedTroop(out navalTroopAssignment);
					Agent agent;
					if (navalTroopAssignment.Agent != null)
					{
						agent = this.ReassignFromUnassignedReserves(navalTroopAssignment);
					}
					else
					{
						agent = this.SpawnAgentAux(navalTroopAssignment.Origin, isReinforcement);
						num++;
					}
					i++;
					spawnedAgents.Add(agent);
				}
			}
			int num3 = i - num;
			return new ValueTuple<int, int>(num, num3);
		}

		// Token: 0x06000FF4 RID: 4084 RVA: 0x00079214 File Offset: 0x00077414
		internal int CheckSpawnReinforcements(MBList<Agent> spawnedAgents)
		{
			int num = 0;
			if (this._reinforcementTimer.Check(true))
			{
				MissionShip ship = this.Ship;
				if (((ship != null) ? ship.Team : null) != null && !this.Ship.IsShipNavmeshDisabled)
				{
					num = this.SpawnNextBatch(true, spawnedAgents).Item1;
					if (num > 0)
					{
						float num2 = NavalAgentsLogic.ComputeReinforcementSpawnDuration(this.ReservedTroopsCount);
						this._reinforcementTimer.SetDuration(num2);
					}
				}
			}
			return num;
		}

		// Token: 0x06000FF5 RID: 4085 RVA: 0x00079280 File Offset: 0x00077480
		internal Agent SpawnHeroFromReserve(IAgentOriginBase heroOrigin, out bool isReassigned)
		{
			NavalTroopAssignment navalTroopAssignment;
			this.DequeueReservedTroop(heroOrigin, out navalTroopAssignment);
			isReassigned = false;
			Agent agent;
			if (navalTroopAssignment.Agent != null)
			{
				agent = this.ReassignFromUnassignedReserves(navalTroopAssignment);
				isReassigned = true;
			}
			else
			{
				agent = this.SpawnAgentAux(heroOrigin, false);
			}
			return agent;
		}

		// Token: 0x06000FF6 RID: 4086 RVA: 0x000792BC File Offset: 0x000774BC
		internal void AssignAndTeleportCrewToShipMachines()
		{
			Mission mission = this._teamAgents.AgentsLogic.Mission;
			ShipControllerMachine shipControllerMachine = this.Ship.ShipControllerMachine;
			this.Ship.ShipOrder.ManageShipDetachments();
			if (this.Ship.ShipPlacementDetachment.IsUsedByFormation(this.Ship.Formation))
			{
				do
				{
					this.Ship.ShipPlacementDetachment.Tick();
				}
				while (this.Ship.ShipPlacementDetachment.IsTickRequired);
			}
			bool isTeleportingAgents = mission.IsTeleportingAgents;
			mission.IsTeleportingAgents = true;
			if (shipControllerMachine.PilotStandingPoint.HasAIMovingTo)
			{
				shipControllerMachine.PilotStandingPoint.MovingAgent.UseGameObject(shipControllerMachine.PilotStandingPoint, -1);
				shipControllerMachine.OnPilotAssignedDuringSpawn();
			}
			if (this.Ship.Captain != null && shipControllerMachine.PilotAgent != this.Ship.Captain && (this.Ship.Captain != Agent.Main || !this.Ship.HasPlayerStandingPointEntity) && (this.Ship.Captain.IsAIControlled ? (!shipControllerMachine.IsDisabledForAI) : (!shipControllerMachine.PilotStandingPoint.IsDisabledForPlayers)))
			{
				this.Ship.Captain.UseGameObject(shipControllerMachine.PilotStandingPoint, -1);
				shipControllerMachine.OnPilotAssignedDuringSpawn();
			}
			foreach (ShipOarMachine shipOarMachine in this.Ship.ShipOarMachines)
			{
				if (shipOarMachine.PilotStandingPoint.HasAIMovingTo)
				{
					shipOarMachine.PilotStandingPoint.MovingAgent.UseGameObject(shipOarMachine.PilotStandingPoint, -1);
					shipOarMachine.OnPilotAssignedDuringSpawn();
				}
			}
			if (this.Ship.ShipSiegeWeapon != null)
			{
				RangedSiegeWeapon shipSiegeWeapon = this.Ship.ShipSiegeWeapon;
				if (shipSiegeWeapon.PilotStandingPoint.HasAIMovingTo)
				{
					shipSiegeWeapon.PilotStandingPoint.MovingAgent.UseGameObject(shipSiegeWeapon.PilotStandingPoint, -1);
					shipSiegeWeapon.OnPilotAssignedDuringSpawn();
				}
			}
			Agent main;
			if (this.Ship.HasPlayerStandingPointEntity && this.Ship.IsPlayerShip && (main = Agent.Main) != null)
			{
				MatrixFrame globalFrame = this.Ship.PlayerStandingPointEntity.GetGlobalFrame();
				Vec3 origin = globalFrame.origin;
				Vec2 vec = globalFrame.rotation.f.AsVec2.Normalized();
				main.TeleportToPosition(origin);
				main.SetMovementDirection(ref vec);
			}
			foreach (Agent agent in this._activeAgents)
			{
				if (agent.IsAIControlled && !agent.InteractingWithAnyGameObject())
				{
					agent.ForceUpdateCachedAndFormationValues(true, false);
				}
			}
			mission.IsTeleportingAgents = isTeleportingAgents;
		}

		// Token: 0x06000FF7 RID: 4087 RVA: 0x0007957C File Offset: 0x0007777C
		internal void OnEndDeploymentMode()
		{
			List<KeyValuePair<int, NavalTroopAssignment>> list = new List<KeyValuePair<int, NavalTroopAssignment>>();
			foreach (NavalTroopAssignment navalTroopAssignment in this._reservedOrderedTroops)
			{
				if (navalTroopAssignment.HasAgent)
				{
					KeyValuePair<int, NavalTroopAssignment> keyValuePair = new KeyValuePair<int, NavalTroopAssignment>(navalTroopAssignment.Priority, NavalTroopAssignment.Create(navalTroopAssignment.Origin, null));
					list.Add(keyValuePair);
				}
			}
			this._reservedOrderedTroops.RemoveAll((KeyValuePair<int, NavalTroopAssignment> tuple) => tuple.Value.HasAgent);
			this._reservedOrderedTroops.AddRange(list);
			foreach (KeyValuePair<int, NavalTroopAssignment> keyValuePair2 in list)
			{
				IAgentOriginBase origin = keyValuePair2.Value.Origin;
				this._reservedTroops[origin] = NavalTroopAssignment.Create(origin, null);
			}
		}

		// Token: 0x06000FF8 RID: 4088 RVA: 0x00079684 File Offset: 0x00077884
		private Agent SpawnAgentAux(IAgentOriginBase agentOrigin, bool isReinforcement)
		{
			MatrixFrame nextOuterInnerSpawnGlobalFrame;
			if (isReinforcement)
			{
				this.Ship.GetNextCrewSpawnGlobalFrame(out nextOuterInnerSpawnGlobalFrame);
			}
			else
			{
				nextOuterInnerSpawnGlobalFrame = this.Ship.GetNextOuterInnerSpawnGlobalFrame();
			}
			Vec2 vec = nextOuterInnerSpawnGlobalFrame.rotation.f.AsVec2.Normalized();
			bool flag = this.Ship.BattleSide == Mission.Current.PlayerTeam.Side;
			Agent agent = Mission.Current.SpawnTroop(agentOrigin, flag, true, false, false, 0, 0, true, true, new Vec3?(nextOuterInnerSpawnGlobalFrame.origin), new Vec2?(vec), null, null, this.Ship.FormationIndex, false);
			bool flag2 = false;
			if (!Mission.Current.IsNavalRaidBattle)
			{
				foreach (ShipVisualSlotInfo shipVisualSlotInfo in this.Ship.ShipOrigin.GetShipVisualSlotInfos())
				{
					if (shipVisualSlotInfo.VisualSlotTag == "side" && shipVisualSlotInfo.VisualPieceId == "brazier")
					{
						flag2 = true;
					}
				}
			}
			if (agent != null && flag2)
			{
				for (EquipmentIndex equipmentIndex = 0; equipmentIndex < 4; equipmentIndex++)
				{
					EquipmentElement equipmentElement = agent.SpawnEquipment[equipmentIndex];
					ItemObject item = equipmentElement.Item;
					if (item != null && item.ItemType == 5)
					{
						ItemObject @object = Game.Current.ObjectManager.GetObject<ItemObject>("burning_arrows");
						EquipmentElement equipmentElement2;
						equipmentElement2..ctor(@object, null, null, false);
						agent.SpawnEquipment[equipmentIndex] = equipmentElement2;
						MissionWeapon missionWeapon;
						missionWeapon..ctor(equipmentElement2.Item, equipmentElement2.ItemModifier, agentOrigin.Banner);
						agent.RemoveEquippedWeapon(equipmentIndex);
						agent.EquipWeaponWithNewEntity(equipmentIndex, ref missionWeapon);
					}
					else
					{
						ItemObject item2 = equipmentElement.Item;
						if (item2 != null && item2.ItemType == 6)
						{
							ItemObject object2 = Game.Current.ObjectManager.GetObject<ItemObject>("burning_bolts");
							EquipmentElement equipmentElement3;
							equipmentElement3..ctor(object2, null, null, false);
							agent.SpawnEquipment[equipmentIndex] = equipmentElement3;
							MissionWeapon missionWeapon2;
							missionWeapon2..ctor(equipmentElement3.Item, equipmentElement3.ItemModifier, agentOrigin.Banner);
							agent.RemoveEquippedWeapon(equipmentIndex);
							agent.EquipWeaponWithNewEntity(equipmentIndex, ref missionWeapon2);
						}
					}
				}
			}
			HumanAIComponent component = agent.GetComponent<HumanAIComponent>();
			if (component != null)
			{
				component.ForceDisablePickUpForAgent();
			}
			this.AddAgent(agent);
			return agent;
		}

		// Token: 0x06000FF9 RID: 4089 RVA: 0x000798D8 File Offset: 0x00077AD8
		private Agent ReassignFromUnassignedReserves(NavalTroopAssignment suppliedTroop)
		{
			this._teamAgents.ReassignAgentAux(this, suppliedTroop.Agent);
			return suppliedTroop.Agent;
		}

		// Token: 0x06000FFA RID: 4090 RVA: 0x000798F4 File Offset: 0x00077AF4
		internal Agent GetMinimumPriorityActiveAgent(MBList<Agent> agentsToIgnore = null)
		{
			Agent agent = null;
			float num = float.MaxValue;
			bool flag = agentsToIgnore != null && agentsToIgnore.Count > 0;
			foreach (Agent agent2 in this.ActiveAgents)
			{
				if (!flag || !agentsToIgnore.Contains(agent2))
				{
					float agentPriority = NavalAgentsLogic.GetAgentPriority(agent2);
					if (agentPriority <= num)
					{
						agent = agent2;
						num = agentPriority;
					}
				}
			}
			return agent;
		}

		// Token: 0x04000983 RID: 2435
		private readonly MBList<Agent> _activeAgents = new MBList<Agent>();

		// Token: 0x04000984 RID: 2436
		private readonly MBList<Agent> _activeHeroAgents = new MBList<Agent>();

		// Token: 0x04000985 RID: 2437
		private readonly MBList<Agent> _activeNonHeroAgents = new MBList<Agent>();

		// Token: 0x04000986 RID: 2438
		private readonly MBSortedMultiList<int, NavalTroopAssignment> _reservedOrderedTroops = new MBSortedMultiList<int, NavalTroopAssignment>(true);

		// Token: 0x04000987 RID: 2439
		private readonly Dictionary<IAgentOriginBase, NavalTroopAssignment> _reservedTroops = new Dictionary<IAgentOriginBase, NavalTroopAssignment>();

		// Token: 0x04000988 RID: 2440
		private readonly MissionTimer _reinforcementTimer;

		// Token: 0x04000989 RID: 2441
		private readonly NavalTeamAgents _teamAgents;

		// Token: 0x0400098A RID: 2442
		private TroopTraitsMask _compatibilityTraitsFilter;
	}
}
