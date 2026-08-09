using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.AI.TeamAI
{
	// Token: 0x020000ED RID: 237
	public class NavalQuerySystem
	{
		// Token: 0x17000322 RID: 802
		// (get) Token: 0x06001214 RID: 4628 RVA: 0x000834EC File Offset: 0x000816EC
		public Vec2 AverageShipPosition
		{
			get
			{
				return this._averageShipPosition.Value;
			}
		}

		// Token: 0x17000323 RID: 803
		// (get) Token: 0x06001215 RID: 4629 RVA: 0x000834F9 File Offset: 0x000816F9
		public Vec2 AverageEnemyShipPosition
		{
			get
			{
				return this._averageEnemyShipPosition.Value;
			}
		}

		// Token: 0x17000324 RID: 804
		// (get) Token: 0x06001216 RID: 4630 RVA: 0x00083506 File Offset: 0x00081706
		public MBReadOnlyList<Formation> FormationsInShipsInLeftToRightOrder
		{
			get
			{
				return Extensions.ToMBList<Formation>(this._formationsInShipsInLeftToRightOrder.Value);
			}
		}

		// Token: 0x17000325 RID: 805
		// (get) Token: 0x06001217 RID: 4631 RVA: 0x00083518 File Offset: 0x00081718
		public MBReadOnlyList<MissionShip> EnemyShipsInLeftToRightOrder
		{
			get
			{
				return this._enemyShipsInLeftToRightOrder.Value;
			}
		}

		// Token: 0x17000326 RID: 806
		// (get) Token: 0x06001218 RID: 4632 RVA: 0x00083525 File Offset: 0x00081725
		public MBReadOnlyList<MissionShip> EnemyShipsWithFormationsInLeftToRightOrder
		{
			get
			{
				return this._enemyShipsWithFormationsInLeftToRightOrder.Value;
			}
		}

		// Token: 0x17000327 RID: 807
		// (get) Token: 0x06001219 RID: 4633 RVA: 0x00083532 File Offset: 0x00081732
		public MBReadOnlyList<MissionShip> TeamShipsWithFormationsInLeftToRightOrder
		{
			get
			{
				return this._teamShipsWithFormationsInLeftToRightOrder.Value;
			}
		}

		// Token: 0x17000328 RID: 808
		// (get) Token: 0x0600121A RID: 4634 RVA: 0x0008353F File Offset: 0x0008173F
		public float ClosestDistanceSquaredToEnemyShip
		{
			get
			{
				return this._closestDistanceSquaredToEnemyShip.Value;
			}
		}

		// Token: 0x0600121B RID: 4635 RVA: 0x0008354C File Offset: 0x0008174C
		public NavalQuerySystem(Team team)
		{
			Mission mission = Mission.Current;
			this._team = team;
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this._averageShipPosition = new QueryData<Vec2>(delegate
			{
				Vec2 vec;
				vec..ctor(0f, 0f);
				int num = 0;
				foreach (Formation formation in this._team.FormationsIncludingEmpty)
				{
					if (formation.CountOfUnits > 0)
					{
						MissionShip missionShip;
						this._navalShipsLogic.GetShip(this._team.TeamSide, formation.FormationIndex, out missionShip);
						vec += missionShip.GameEntity.GlobalPosition.AsVec2;
						num++;
					}
				}
				if (num <= 0)
				{
					return vec;
				}
				return vec / (float)num;
			}, 1f);
			this._averageEnemyShipPosition = new QueryData<Vec2>(delegate
			{
				Vec2 vec2;
				vec2..ctor(0f, 0f);
				int num2 = 0;
				foreach (Team team2 in Mission.Current.Teams)
				{
					if (this._team.IsEnemyOf(team2))
					{
						foreach (Formation formation2 in team2.FormationsIncludingEmpty)
						{
							if (formation2.CountOfUnits > 0)
							{
								MissionShip missionShip2;
								this._navalShipsLogic.GetShip(team2.TeamSide, formation2.FormationIndex, out missionShip2);
								vec2 += missionShip2.GameEntity.GlobalPosition.AsVec2;
								num2++;
							}
						}
					}
				}
				if (num2 <= 0)
				{
					return vec2;
				}
				return vec2 / (float)num2;
			}, 1f);
			this._formationsInShipsInLeftToRightOrder = new QueryData<MBReadOnlyList<Formation>>(delegate
			{
				this._temporaryFormationPositionTupleContainer.Clear();
				foreach (Formation formation3 in this._team.FormationsIncludingEmpty)
				{
					MissionShip missionShip3;
					if (formation3.CountOfUnits > 0 && this._navalShipsLogic.GetShip(this._team.TeamSide, formation3.FormationIndex, out missionShip3))
					{
						this._temporaryFormationPositionTupleContainer.Add(new Tuple<Formation, Vec2>(formation3, missionShip3.GameEntity.GlobalPosition.AsVec2));
					}
				}
				return Extensions.ToMBList<Formation>(from fst in this._temporaryFormationPositionTupleContainer
					orderby (fst.Item2 - this.AverageShipPosition).DotProduct((this.AverageEnemyShipPosition - this.AverageShipPosition).LeftVec()) descending
					select fst.Item1);
			}, 5f);
			this._enemyShipsInLeftToRightOrder = new QueryData<MBReadOnlyList<MissionShip>>(delegate
			{
				this._temporaryMissionShipContainer.Clear();
				foreach (Team team3 in Mission.Current.Teams)
				{
					if (MBExtensions.IsOpponentOf(this._team.Side, team3.Side))
					{
						this._navalShipsLogic.FillTeamShips(team3.TeamSide, this._temporaryMissionShipContainer);
					}
				}
				return Extensions.ToMBList<MissionShip>(this._temporaryMissionShipContainer.OrderByDescending<MissionShip, float>((MissionShip sl) => (sl.GameEntity.GlobalPosition.AsVec2 - this.AverageEnemyShipPosition).DotProduct((this.AverageShipPosition - this.AverageEnemyShipPosition).LeftVec())));
			}, 5f);
			this._enemyShipsWithFormationsInLeftToRightOrder = new QueryData<MBReadOnlyList<MissionShip>>(delegate
			{
				this._temporaryMissionShipContainer.Clear();
				foreach (Team team4 in Mission.Current.Teams)
				{
					if (MBExtensions.IsOpponentOf(this._team.Side, team4.Side))
					{
						foreach (Formation formation4 in team4.FormationsIncludingEmpty)
						{
							MissionShip missionShip4;
							if (formation4.CountOfUnits > 0 && this._navalShipsLogic.GetShip(team4.TeamSide, formation4.FormationIndex, out missionShip4))
							{
								this._temporaryMissionShipContainer.Add(missionShip4);
							}
						}
					}
				}
				return Extensions.ToMBList<MissionShip>(this._temporaryMissionShipContainer.OrderByDescending<MissionShip, float>((MissionShip sl) => (sl.GameEntity.GlobalPosition.AsVec2 - this.AverageEnemyShipPosition).DotProduct((this.AverageShipPosition - this.AverageEnemyShipPosition).LeftVec())));
			}, 5f);
			this._teamShipsWithFormationsInLeftToRightOrder = new QueryData<MBReadOnlyList<MissionShip>>(delegate
			{
				this._temporaryMissionShipContainer.Clear();
				foreach (Formation formation5 in this._team.FormationsIncludingEmpty)
				{
					MissionShip missionShip5;
					if (formation5.CountOfUnits > 0 && this._navalShipsLogic.GetShip(this._team.TeamSide, formation5.FormationIndex, out missionShip5))
					{
						this._temporaryMissionShipContainer.Add(missionShip5);
					}
				}
				return Extensions.ToMBList<MissionShip>(this._temporaryMissionShipContainer.OrderByDescending<MissionShip, float>((MissionShip sl) => (sl.GameEntity.GlobalPosition.AsVec2 - this.AverageShipPosition).DotProduct((this.AverageShipPosition - this.AverageShipPosition).LeftVec())));
			}, 5f);
			this._shipInCriticalZoneDictionary = new QueryData<Dictionary<ValueTuple<MissionShip, MissionShip>, bool>>(delegate
			{
				MBReadOnlyList<MissionShip> allShips = this._navalShipsLogic.AllShips;
				foreach (MissionShip missionShip6 in allShips)
				{
					foreach (MissionShip missionShip7 in missionShip6.GetConnectedShips())
					{
						ValueTuple<MissionShip, MissionShip> valueTuple = ((missionShip6.GetHashCode() < missionShip7.GetHashCode()) ? new ValueTuple<MissionShip, MissionShip>(missionShip6, missionShip7) : new ValueTuple<MissionShip, MissionShip>(missionShip7, missionShip6));
						if (missionShip6.IsShipInCriticalZoneBetween(missionShip7, allShips))
						{
							this._shipsInCriticalZoneContainer[valueTuple] = true;
						}
						else
						{
							this._shipsInCriticalZoneContainer[valueTuple] = false;
						}
					}
				}
				return this._shipsInCriticalZoneContainer;
			}, 5f);
			this._closestDistanceSquaredToEnemyShip = new QueryData<float>(delegate
			{
				float num3 = float.MaxValue;
				foreach (Formation formation6 in this.FormationsInShipsInLeftToRightOrder)
				{
					if (formation6.CountOfUnits > 0 && formation6.CachedClosestEnemyFormationDistanceSquared < num3)
					{
						num3 = formation6.CachedClosestEnemyFormationDistanceSquared;
					}
				}
				return num3;
			}, 1f);
			this.InitializeTelemetryScopeNames();
		}

		// Token: 0x0600121C RID: 4636 RVA: 0x00083683 File Offset: 0x00081883
		public void ForceExpireSameSideShipLists()
		{
			this._teamShipsWithFormationsInLeftToRightOrder.Expire();
			this._formationsInShipsInLeftToRightOrder.Expire();
		}

		// Token: 0x0600121D RID: 4637 RVA: 0x0008369C File Offset: 0x0008189C
		public void ForceExpireAll()
		{
			this._averageShipPosition.Expire();
			this._averageEnemyShipPosition.Expire();
			this._formationsInShipsInLeftToRightOrder.Expire();
			this._enemyShipsInLeftToRightOrder.Expire();
			this._enemyShipsWithFormationsInLeftToRightOrder.Expire();
			this._teamShipsWithFormationsInLeftToRightOrder.Expire();
			this._shipInCriticalZoneDictionary.Expire();
			this._closestDistanceSquaredToEnemyShip.Expire();
		}

		// Token: 0x0600121E RID: 4638 RVA: 0x00083704 File Offset: 0x00081904
		public bool IsAnyShipInCriticalZoneBetween(MissionShip ship1, MissionShip ship2)
		{
			if (this._shipInCriticalZoneDictionary == null || this._shipInCriticalZoneDictionary.Value == null)
			{
				return false;
			}
			Dictionary<ValueTuple<MissionShip, MissionShip>, bool> value = this._shipInCriticalZoneDictionary.Value;
			ValueTuple<MissionShip, MissionShip> valueTuple = ((ship1.GetHashCode() < ship2.GetHashCode()) ? new ValueTuple<MissionShip, MissionShip>(ship1, ship2) : new ValueTuple<MissionShip, MissionShip>(ship2, ship1));
			bool flag;
			return value.TryGetValue(valueTuple, out flag) && flag;
		}

		// Token: 0x0600121F RID: 4639 RVA: 0x0008375C File Offset: 0x0008195C
		private void InitializeTelemetryScopeNames()
		{
		}

		// Token: 0x04000A23 RID: 2595
		private readonly MBList<Tuple<Formation, Vec2>> _temporaryFormationPositionTupleContainer = new MBList<Tuple<Formation, Vec2>>();

		// Token: 0x04000A24 RID: 2596
		private readonly MBList<MissionShip> _temporaryMissionShipContainer = new MBList<MissionShip>();

		// Token: 0x04000A25 RID: 2597
		private readonly Dictionary<ValueTuple<MissionShip, MissionShip>, bool> _shipsInCriticalZoneContainer = new Dictionary<ValueTuple<MissionShip, MissionShip>, bool>();

		// Token: 0x04000A26 RID: 2598
		private readonly QueryData<Vec2> _averageShipPosition;

		// Token: 0x04000A27 RID: 2599
		private readonly QueryData<Vec2> _averageEnemyShipPosition;

		// Token: 0x04000A28 RID: 2600
		private readonly QueryData<MBReadOnlyList<Formation>> _formationsInShipsInLeftToRightOrder;

		// Token: 0x04000A29 RID: 2601
		private readonly QueryData<MBReadOnlyList<MissionShip>> _enemyShipsInLeftToRightOrder;

		// Token: 0x04000A2A RID: 2602
		private readonly QueryData<MBReadOnlyList<MissionShip>> _enemyShipsWithFormationsInLeftToRightOrder;

		// Token: 0x04000A2B RID: 2603
		private readonly QueryData<MBReadOnlyList<MissionShip>> _teamShipsWithFormationsInLeftToRightOrder;

		// Token: 0x04000A2C RID: 2604
		private readonly QueryData<Dictionary<ValueTuple<MissionShip, MissionShip>, bool>> _shipInCriticalZoneDictionary;

		// Token: 0x04000A2D RID: 2605
		private readonly QueryData<float> _closestDistanceSquaredToEnemyShip;

		// Token: 0x04000A2E RID: 2606
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x04000A2F RID: 2607
		private Team _team;
	}
}
