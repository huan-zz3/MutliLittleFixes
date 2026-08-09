using System;
using System.Collections.Generic;
using NavalDLC.Missions.Objects;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000013 RID: 19
	public class ShipPlacementDetachment : IDetachment
	{
		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060000A7 RID: 167 RVA: 0x00006A2B File Offset: 0x00004C2B
		public MBReadOnlyList<Formation> UserFormations
		{
			get
			{
				return this._userFormations;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x00006A33 File Offset: 0x00004C33
		public bool IsLoose
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060000A9 RID: 169 RVA: 0x00006A36 File Offset: 0x00004C36
		public bool IsActive
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000AA RID: 170 RVA: 0x00006A39 File Offset: 0x00004C39
		public bool HasAgent
		{
			get
			{
				return this.CountOfAgents > 0;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060000AB RID: 171 RVA: 0x00006A44 File Offset: 0x00004C44
		// (set) Token: 0x060000AC RID: 172 RVA: 0x00006A4C File Offset: 0x00004C4C
		public int CountOfAgents { get; private set; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000AD RID: 173 RVA: 0x00006A55 File Offset: 0x00004C55
		public bool HasAvailableSlots
		{
			get
			{
				return this._shipPlacementPositions.Count > this.CountOfAgents;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000AE RID: 174 RVA: 0x00006A6A File Offset: 0x00004C6A
		public bool IsTickRequired
		{
			get
			{
				return this._isTickRequired || this._placementDetachmentTimer.Check(false);
			}
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00006A84 File Offset: 0x00004C84
		public ShipPlacementDetachment(in MissionShip ownerShip)
		{
			this._ownerShip = ownerShip;
			this._userFormations = new MBList<Formation>();
			this._shipPlacementPositions = new MBList<ShipPlacementDetachment.ShipPlacementPosition>();
			float num = 0f;
			foreach (MatrixFrame matrixFrame in ownerShip.OuterDeckLocalFrames)
			{
				num += matrixFrame.origin.z;
			}
			foreach (MatrixFrame matrixFrame2 in ownerShip.InnerDeckLocalFrames)
			{
				num += matrixFrame2.origin.z;
			}
			foreach (MatrixFrame matrixFrame3 in ownerShip.CrewSpawnLocalFrames)
			{
				num += matrixFrame3.origin.z;
			}
			int num2 = ownerShip.OuterDeckLocalFrames.Count + ownerShip.InnerDeckLocalFrames.Count + ownerShip.CrewSpawnLocalFrames.Count;
			float num3 = num / (float)((num2 > 0) ? num2 : 1);
			foreach (MatrixFrame matrixFrame4 in ownerShip.OuterDeckLocalFrames)
			{
				this._shipPlacementPositions.Add(new ShipPlacementDetachment.ShipPlacementPosition(matrixFrame4, true, matrixFrame4.origin.z - num3 >= 1f));
			}
			foreach (MatrixFrame matrixFrame5 in ownerShip.InnerDeckLocalFrames)
			{
				this._shipPlacementPositions.Add(new ShipPlacementDetachment.ShipPlacementPosition(matrixFrame5, false, matrixFrame5.origin.z - num3 >= 1f));
			}
			foreach (MatrixFrame matrixFrame6 in ownerShip.CrewSpawnLocalFrames)
			{
				this._shipPlacementPositions.Add(new ShipPlacementDetachment.ShipPlacementPosition(matrixFrame6, false, matrixFrame6.origin.z - num3 >= 1f));
			}
			this._agents = new Agent[this._shipPlacementPositions.Count];
			this._boardingDirection = Vec2.Zero;
			this._placementDetachmentTimer = new MissionTimer(5f);
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00006D48 File Offset: 0x00004F48
		public void AddAgent(Agent agent, int slotIndex, Agent.AIScriptedFrameFlags customFlags = 0)
		{
			this._agents[slotIndex] = agent;
			int countOfAgents = this.CountOfAgents;
			this.CountOfAgents = countOfAgents + 1;
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00006D70 File Offset: 0x00004F70
		public void AddAgentAtSlotIndex(Agent agent, int slotIndex)
		{
			this._agents[slotIndex] = agent;
			int countOfAgents = this.CountOfAgents;
			this.CountOfAgents = countOfAgents + 1;
			this._shipPlacementPositions[slotIndex].SetAgent(agent);
			Formation formation = agent.Formation;
			if (formation != null)
			{
				formation.DetachUnit(agent, true);
			}
			agent.Detachment = this;
			agent.SetDetachmentWeight(1f);
			agent.SetDetachmentIndex(slotIndex);
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00006DD4 File Offset: 0x00004FD4
		public void AddAgent(Agent agent)
		{
			for (int i = 0; i < this._agents.Length; i++)
			{
				if (this._agents[i] == null)
				{
					this.AddAgentAtSlotIndex(agent, i);
					return;
				}
			}
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00006E07 File Offset: 0x00005007
		void IDetachment.FormationStartUsing(Formation formation)
		{
			this._userFormations.Add(formation);
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00006E15 File Offset: 0x00005015
		void IDetachment.FormationStopUsing(Formation formation)
		{
			this._userFormations.Remove(formation);
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00006E24 File Offset: 0x00005024
		public bool IsUsedByFormation(Formation formation)
		{
			return this._userFormations.Contains(formation);
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00006E32 File Offset: 0x00005032
		Agent IDetachment.GetMovingAgentAtSlotIndex(int slotIndex)
		{
			if (slotIndex >= this._agents.Length)
			{
				return null;
			}
			return this._agents[slotIndex];
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00006E49 File Offset: 0x00005049
		void IDetachment.GetSlotIndexWeightTuples(List<ValueTuple<int, float>> slotIndexWeightTuples)
		{
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00006E4B File Offset: 0x0000504B
		bool IDetachment.IsSlotAtIndexAvailableForAgent(int slotIndex, Agent agent)
		{
			return false;
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00006E4E File Offset: 0x0000504E
		bool IDetachment.IsAgentEligible(Agent agent)
		{
			return agent.Detachment == this;
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00006E59 File Offset: 0x00005059
		void IDetachment.UnmarkDetachment()
		{
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00006E5B File Offset: 0x0000505B
		bool IDetachment.IsDetachmentRecentlyEvaluated()
		{
			return true;
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00006E5E File Offset: 0x0000505E
		void IDetachment.MarkSlotAtIndex(int slotIndex)
		{
		}

		// Token: 0x060000BD RID: 189 RVA: 0x00006E60 File Offset: 0x00005060
		bool IDetachment.IsAgentUsingOrInterested(Agent agent)
		{
			return agent.DetachmentIndex >= 0 && agent.DetachmentIndex < this._agents.Length && this._agents[agent.DetachmentIndex] == agent;
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00006E90 File Offset: 0x00005090
		void IDetachment.OnFormationLeave(Formation formation)
		{
			for (int i = this._agents.Length - 1; i >= 0; i--)
			{
				Agent agent = this._agents[i];
				if (agent != null && agent.Formation == formation && !agent.IsPlayerControlled)
				{
					this._agents[i] = null;
					int countOfAgents = this.CountOfAgents;
					this.CountOfAgents = countOfAgents - 1;
					agent.SetCrouchMode(false);
					agent.EnforceShieldUsage(-1);
					agent.DisableScriptedMovement();
					agent.DisableScriptedCombatMovement();
					formation.AttachUnit(agent);
				}
			}
			for (int j = 0; j < this._shipPlacementPositions.Count; j++)
			{
				this._shipPlacementPositions[j].ResetPlacementPosition();
			}
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00006F30 File Offset: 0x00005130
		public bool IsStandingPointAvailableForAgent(Agent agent)
		{
			return false;
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00006F33 File Offset: 0x00005133
		public List<float> GetTemplateCostsOfAgent(Agent candidate, List<float> oldValue)
		{
			return oldValue;
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00006F36 File Offset: 0x00005136
		float IDetachment.GetExactCostOfAgentAtSlot(Agent candidate, int slotIndex)
		{
			return float.MaxValue;
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00006F3D File Offset: 0x0000513D
		public float GetTemplateWeightOfAgent(Agent candidate)
		{
			return float.MaxValue;
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00006F44 File Offset: 0x00005144
		public float? GetWeightOfAgentAtNextSlot(List<Agent> newAgents, out Agent match)
		{
			match = null;
			return null;
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00006F60 File Offset: 0x00005160
		public float? GetWeightOfAgentAtNextSlot(List<ValueTuple<Agent, float>> agentTemplateScores, out Agent match)
		{
			match = null;
			return null;
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00006F79 File Offset: 0x00005179
		public float? GetWeightOfAgentAtOccupiedSlot(Agent detachedAgent, List<Agent> newAgents, out Agent match)
		{
			match = null;
			return new float?(float.MaxValue);
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00006F88 File Offset: 0x00005188
		public void RemoveAgent(Agent agent)
		{
			this._agents[agent.DetachmentIndex] = null;
			int countOfAgents = this.CountOfAgents;
			this.CountOfAgents = countOfAgents - 1;
			this._shipPlacementPositions[agent.DetachmentIndex].RemoveAgent();
			agent.SetCrouchMode(false);
			agent.EnforceShieldUsage(-1);
			agent.DisableScriptedMovement();
			agent.DisableScriptedCombatMovement();
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00006FE3 File Offset: 0x000051E3
		public int GetNumberOfUsableSlots()
		{
			return this._shipPlacementPositions.Count - this.CountOfAgents;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00006FF7 File Offset: 0x000051F7
		public void SetUnderMissileFire(bool isUnderMissileFire)
		{
			if (this._isUnderMissileFire != isUnderMissileFire)
			{
				this._isUnderMissileFire = isUnderMissileFire;
				this._isTickRequired = true;
			}
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00007010 File Offset: 0x00005210
		public void SetBoarding(bool isBoarding, Vec2 localDir)
		{
			if (this._isBoarding != isBoarding || (this._boardingDirection.IsNonZero() && !localDir.IsNonZero()) || (!this._boardingDirection.IsNonZero() && localDir.IsNonZero()))
			{
				if (!isBoarding || !localDir.IsNonZero())
				{
					for (int i = 0; i < this._shipPlacementPositions.Count; i++)
					{
						this._shipPlacementPositions[i].ResetExtraPosition();
					}
				}
				this._isBoarding = isBoarding;
				this._boardingDirection = localDir;
				this._isTickRequired = true;
			}
		}

		// Token: 0x060000CA RID: 202 RVA: 0x0000709C File Offset: 0x0000529C
		public void Tick()
		{
			float num = 0f;
			int num2 = -1;
			float num3 = float.MaxValue;
			int num4 = -1;
			ShipPlacementDetachment.PositionCondition positionCondition = ShipPlacementDetachment.PositionCondition.Any;
			bool flag = false;
			float num5 = 0f;
			float num6 = 0f;
			ShipPlacementDetachment.PositionCondition positionCondition2 = ShipPlacementDetachment.PositionCondition.Any;
			bool flag2 = false;
			for (int i = 0; i < this._shipPlacementPositions.Count; i++)
			{
				if (this._isBoarding)
				{
					this._shipPlacementPositions[i].CalculateBoardingScore(this._boardingDirection, out num6, out num5, out positionCondition2, out flag2);
				}
				else if (this._isUnderMissileFire)
				{
					this._shipPlacementPositions[i].CalculateUnderMissileFireScore(out num6, out num5, out positionCondition2);
				}
				else
				{
					this._shipPlacementPositions[i].CalculateDefaultScore(out num6, out num5, out positionCondition2);
				}
				if (num5 > num)
				{
					num = num5;
					num2 = i;
					positionCondition = positionCondition2;
					flag = flag2;
				}
			}
			for (int j = 0; j < this._shipPlacementPositions.Count; j++)
			{
				if (this._shipPlacementPositions[j].AssignedAgent != null && !this._shipPlacementPositions[j].LentToOtherFrame && ShipPlacementDetachment.CheckCondition(positionCondition, this._shipPlacementPositions[j].AssignedAgent))
				{
					if (this._isBoarding)
					{
						this._shipPlacementPositions[j].CalculateBoardingScore(this._boardingDirection, out num6, out num5, out positionCondition2, out flag2);
					}
					else if (this._isUnderMissileFire)
					{
						this._shipPlacementPositions[j].CalculateUnderMissileFireScore(out num6, out num5, out positionCondition2);
					}
					else
					{
						this._shipPlacementPositions[j].CalculateDefaultScore(out num6, out num5, out positionCondition2);
					}
					if (num6 < num3)
					{
						num3 = num6;
						num4 = j;
					}
				}
			}
			if (num2 != num4 && num2 > -1 && num4 > -1 && num > num3)
			{
				Agent assignedAgent = this._shipPlacementPositions[num2].AssignedAgent;
				Agent assignedAgent2 = this._shipPlacementPositions[num4].AssignedAgent;
				if (flag)
				{
					this._shipPlacementPositions[num4].LendToExtraPosition(num2);
					this._shipPlacementPositions[num2].SetExtraAgent(assignedAgent2);
				}
				else
				{
					this._shipPlacementPositions[num2].SetAgent(assignedAgent2);
					this._agents[num2] = assignedAgent2;
					assignedAgent2.SetDetachmentIndex(num2);
					if (assignedAgent != null)
					{
						this._shipPlacementPositions[num4].SetAgent(assignedAgent);
						this._agents[num4] = assignedAgent;
						assignedAgent.SetDetachmentIndex(num4);
					}
					else
					{
						this._shipPlacementPositions[num4].RemoveAgent();
						this._agents[num4] = null;
					}
				}
				this._isTickRequired = true;
				return;
			}
			this._isTickRequired = false;
			this._placementDetachmentTimer.Reset();
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00007330 File Offset: 0x00005530
		public WorldFrame? GetAgentFrame(Agent agent)
		{
			ShipPlacementDetachment.ShipPlacementPosition shipPlacementPosition = this._shipPlacementPositions[agent.DetachmentIndex];
			if (shipPlacementPosition.LentToOtherFrame)
			{
				shipPlacementPosition = this._shipPlacementPositions[shipPlacementPosition.ExtraFrameIndex];
			}
			agent.EnforceShieldUsage((this._isUnderMissileFire && !agent.HasAnyRangedWeaponCached) ? ((shipPlacementPosition.IsOuterPos && agent.HasShieldCached) ? 5 : 4) : (-1));
			MatrixFrame matrixFrame = shipPlacementPosition.LocalFrame;
			if (this._isBoarding && shipPlacementPosition.HasExtraAgent)
			{
				Vec3 vec = new Vec3(matrixFrame.origin.x, matrixFrame.origin.y + ((agent == shipPlacementPosition.AssignedAgent) ? (-0.5f) : 0.5f), matrixFrame.origin.z, -1f);
				matrixFrame = new MatrixFrame(ref matrixFrame.rotation, ref vec);
			}
			MatrixFrame matrixFrame2 = this._ownerShip.GlobalFrame;
			MatrixFrame matrixFrame3 = matrixFrame2.TransformToParent(ref matrixFrame);
			Mat3 mat;
			if ((shipPlacementPosition.IsOuterPos && (agent.HasAnyRangedWeaponCached || this._isBoarding)) || (this._isUnderMissileFire && agent.HasShieldCached))
			{
				if (matrixFrame.origin.x > 0f)
				{
					Vec3 vec = -this._ownerShip.GlobalFrame.rotation.f;
					matrixFrame2 = this._ownerShip.GlobalFrame;
					MatrixFrame matrixFrame4 = this._ownerShip.GlobalFrame;
					mat = new Mat3(ref vec, ref matrixFrame2.rotation.s, ref matrixFrame4.rotation.u);
				}
				else
				{
					matrixFrame2 = this._ownerShip.GlobalFrame;
					Vec3 vec = -this._ownerShip.GlobalFrame.rotation.s;
					MatrixFrame matrixFrame4 = this._ownerShip.GlobalFrame;
					mat = new Mat3(ref matrixFrame2.rotation.f, ref vec, ref matrixFrame4.rotation.u);
				}
			}
			else
			{
				mat = matrixFrame3.rotation;
			}
			bool flag;
			if (this._isUnderMissileFire && !agent.HasAnyRangedWeaponCached && !agent.HasShieldCached)
			{
				Vec3 vec = agent.Position;
				flag = vec.DistanceSquared(matrixFrame3.origin) <= 1f;
			}
			else
			{
				flag = false;
			}
			agent.SetCrouchMode(flag);
			return new WorldFrame?(new WorldFrame(mat, ModuleExtensions.ToWorldPosition(matrixFrame3.origin)));
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00007564 File Offset: 0x00005764
		public float? GetWeightOfNextSlot(BattleSideEnum side)
		{
			return null;
		}

		// Token: 0x060000CD RID: 205 RVA: 0x0000757A File Offset: 0x0000577A
		public float GetWeightOfOccupiedSlot(Agent agent)
		{
			return float.MinValue;
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00007581 File Offset: 0x00005781
		float IDetachment.GetDetachmentWeight(BattleSideEnum side)
		{
			return float.MinValue;
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00007588 File Offset: 0x00005788
		void IDetachment.ResetEvaluation()
		{
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x0000758A File Offset: 0x0000578A
		bool IDetachment.IsEvaluated()
		{
			return true;
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x0000758D File Offset: 0x0000578D
		void IDetachment.SetAsEvaluated()
		{
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x0000758F File Offset: 0x0000578F
		float IDetachment.GetDetachmentWeightFromCache()
		{
			return float.MinValue;
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00007596 File Offset: 0x00005796
		float IDetachment.ComputeAndCacheDetachmentWeight(BattleSideEnum side)
		{
			return float.MinValue;
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x000075A0 File Offset: 0x000057A0
		public Agent PickLastAgent()
		{
			Agent agent = null;
			for (int i = this._agents.Length - 1; i >= 0; i--)
			{
				if (this._agents[i] != null)
				{
					agent = this._agents[i];
					this.RemoveAgent(agent);
					agent.Formation.AttachUnit(agent);
					return agent;
				}
			}
			return agent;
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x000075ED File Offset: 0x000057ED
		private static bool CheckCondition(ShipPlacementDetachment.PositionCondition positionCondition, Agent checkedAgent)
		{
			switch (positionCondition)
			{
			case ShipPlacementDetachment.PositionCondition.Any:
				return true;
			case ShipPlacementDetachment.PositionCondition.RangedOrShield:
				return checkedAgent.HasShieldCached || checkedAgent.HasAnyRangedWeaponCached;
			case ShipPlacementDetachment.PositionCondition.Ranged:
				return checkedAgent.HasAnyRangedWeaponCached;
			default:
				return false;
			}
		}

		// Token: 0x04000071 RID: 113
		private readonly Agent[] _agents;

		// Token: 0x04000072 RID: 114
		private readonly MBList<Formation> _userFormations;

		// Token: 0x04000073 RID: 115
		private readonly MBList<ShipPlacementDetachment.ShipPlacementPosition> _shipPlacementPositions;

		// Token: 0x04000074 RID: 116
		private readonly MissionShip _ownerShip;

		// Token: 0x04000075 RID: 117
		private bool _isUnderMissileFire;

		// Token: 0x04000076 RID: 118
		private bool _isBoarding;

		// Token: 0x04000077 RID: 119
		private Vec2 _boardingDirection;

		// Token: 0x04000078 RID: 120
		private MissionTimer _placementDetachmentTimer;

		// Token: 0x04000079 RID: 121
		private bool _isTickRequired = true;

		// Token: 0x02000181 RID: 385
		private class ShipPlacementPosition
		{
			// Token: 0x170003F4 RID: 1012
			// (get) Token: 0x060018E4 RID: 6372 RVA: 0x000ACAC1 File Offset: 0x000AACC1
			// (set) Token: 0x060018E5 RID: 6373 RVA: 0x000ACAC9 File Offset: 0x000AACC9
			public Agent AssignedAgent { get; private set; }

			// Token: 0x170003F5 RID: 1013
			// (get) Token: 0x060018E6 RID: 6374 RVA: 0x000ACAD2 File Offset: 0x000AACD2
			public MatrixFrame LocalFrame { get; }

			// Token: 0x170003F6 RID: 1014
			// (get) Token: 0x060018E7 RID: 6375 RVA: 0x000ACADA File Offset: 0x000AACDA
			public bool IsOuterPos { get; }

			// Token: 0x170003F7 RID: 1015
			// (get) Token: 0x060018E8 RID: 6376 RVA: 0x000ACAE2 File Offset: 0x000AACE2
			// (set) Token: 0x060018E9 RID: 6377 RVA: 0x000ACAEA File Offset: 0x000AACEA
			public bool HasExtraAgent { get; private set; }

			// Token: 0x170003F8 RID: 1016
			// (get) Token: 0x060018EA RID: 6378 RVA: 0x000ACAF3 File Offset: 0x000AACF3
			public bool LentToOtherFrame
			{
				get
				{
					return this.ExtraFrameIndex >= 0;
				}
			}

			// Token: 0x170003F9 RID: 1017
			// (get) Token: 0x060018EB RID: 6379 RVA: 0x000ACB01 File Offset: 0x000AAD01
			// (set) Token: 0x060018EC RID: 6380 RVA: 0x000ACB09 File Offset: 0x000AAD09
			public int ExtraFrameIndex { get; private set; } = -1;

			// Token: 0x060018ED RID: 6381 RVA: 0x000ACB12 File Offset: 0x000AAD12
			public ShipPlacementPosition(MatrixFrame frame, bool isOuterPos, bool isHighPos)
			{
				this.LocalFrame = frame;
				this.IsOuterPos = isOuterPos;
				this._isHighPos = isHighPos;
				this.HasExtraAgent = false;
				this.AssignedAgent = null;
				this._extraAgent = null;
			}

			// Token: 0x060018EE RID: 6382 RVA: 0x000ACB4B File Offset: 0x000AAD4B
			public void RemoveAgent()
			{
				this.AssignedAgent = null;
				this._extraAgent = null;
			}

			// Token: 0x060018EF RID: 6383 RVA: 0x000ACB5B File Offset: 0x000AAD5B
			public void LendToExtraPosition(int extraFrameIndex)
			{
				this.ExtraFrameIndex = extraFrameIndex;
			}

			// Token: 0x060018F0 RID: 6384 RVA: 0x000ACB64 File Offset: 0x000AAD64
			public void ResetPlacementPosition()
			{
				this.AssignedAgent = null;
				this.ResetExtraPosition();
			}

			// Token: 0x060018F1 RID: 6385 RVA: 0x000ACB73 File Offset: 0x000AAD73
			public void ResetExtraPosition()
			{
				this.ExtraFrameIndex = -1;
				this.HasExtraAgent = false;
				this._extraAgent = null;
			}

			// Token: 0x060018F2 RID: 6386 RVA: 0x000ACB8A File Offset: 0x000AAD8A
			public void SetAgent(Agent agent)
			{
				this.AssignedAgent = agent;
			}

			// Token: 0x060018F3 RID: 6387 RVA: 0x000ACB93 File Offset: 0x000AAD93
			public void SetExtraAgent(Agent agent)
			{
				this.HasExtraAgent = agent != null;
				this._extraAgent = agent;
			}

			// Token: 0x060018F4 RID: 6388 RVA: 0x000ACBA8 File Offset: 0x000AADA8
			public void CalculateDefaultScore(out float resultScore, out float resultPossibleGain, out ShipPlacementDetachment.PositionCondition outGainCondition)
			{
				float num = 1f * (this.IsOuterPos ? 10f : 1f) * (this._isHighPos ? 50f : 1f);
				resultScore = ((this.AssignedAgent == null) ? 0f : (this.AssignedAgent.HasAnyRangedWeaponCached ? num : 1f));
				resultPossibleGain = num - resultScore;
				outGainCondition = ShipPlacementDetachment.PositionCondition.Ranged;
			}

			// Token: 0x060018F5 RID: 6389 RVA: 0x000ACC14 File Offset: 0x000AAE14
			public void CalculateUnderMissileFireScore(out float resultScore, out float resultPossibleGain, out ShipPlacementDetachment.PositionCondition outGainCondition)
			{
				float num = 1f * (this.IsOuterPos ? 50f : 1f) * (this._isHighPos ? 50f : 1f);
				if (!this.IsOuterPos && !this._isHighPos)
				{
					num = 1f;
					resultScore = ((this.AssignedAgent != null) ? num : 0f);
					resultPossibleGain = num - resultScore;
					outGainCondition = ShipPlacementDetachment.PositionCondition.Any;
					return;
				}
				if (!this._isHighPos)
				{
					num = 50f;
					outGainCondition = ShipPlacementDetachment.PositionCondition.RangedOrShield;
					resultScore = ((this.AssignedAgent == null) ? 0f : (ShipPlacementDetachment.CheckCondition(outGainCondition, this.AssignedAgent) ? 50f : 1f));
					resultPossibleGain = num - resultScore;
					return;
				}
				if (!this.IsOuterPos)
				{
					num = 50f;
					outGainCondition = ShipPlacementDetachment.PositionCondition.Ranged;
					resultScore = ((this.AssignedAgent == null) ? 0f : (ShipPlacementDetachment.CheckCondition(outGainCondition, this.AssignedAgent) ? 50f : 1f));
					resultPossibleGain = num - resultScore;
					return;
				}
				num = 250f;
				outGainCondition = ShipPlacementDetachment.PositionCondition.Ranged;
				resultScore = ((this.AssignedAgent == null) ? 0f : (ShipPlacementDetachment.CheckCondition(outGainCondition, this.AssignedAgent) ? 250f : 1f));
				resultPossibleGain = num - resultScore;
			}

			// Token: 0x060018F6 RID: 6390 RVA: 0x000ACD48 File Offset: 0x000AAF48
			public void CalculateBoardingScore(Vec2 boardingLocalPosition, out float resultScore, out float resultPossibleGain, out ShipPlacementDetachment.PositionCondition outGainCondition, out bool requestExtraAgent)
			{
				requestExtraAgent = false;
				if (this._isHighPos)
				{
					float num = 1f;
					if (boardingLocalPosition.IsNonZero())
					{
						if (boardingLocalPosition.x * this.LocalFrame.origin.x >= 0f)
						{
							Vec2 vec = boardingLocalPosition.Normalized();
							MatrixFrame matrixFrame = this.LocalFrame;
							float num2 = vec.DotProduct(matrixFrame.origin.AsVec2.Normalized());
							if (num2 >= 0f)
							{
								num = MathF.Clamp(num2 * 10f, 1f, 10f);
							}
						}
					}
					else
					{
						num = 10f;
					}
					float num3 = 50f * (this.IsOuterPos ? 10f : 1f) * num;
					outGainCondition = ShipPlacementDetachment.PositionCondition.Ranged;
					resultScore = ((this.AssignedAgent == null) ? 0f : (ShipPlacementDetachment.CheckCondition(outGainCondition, this.AssignedAgent) ? num3 : 1f));
					resultPossibleGain = num3 - resultScore;
					return;
				}
				float num4;
				if (boardingLocalPosition.IsNonZero())
				{
					num4 = 0.1f;
					if (boardingLocalPosition.x * this.LocalFrame.origin.x >= 0f)
					{
						Vec2 vec = boardingLocalPosition.Normalized();
						MatrixFrame matrixFrame = this.LocalFrame;
						num4 = MathF.Clamp((vec.DotProduct(matrixFrame.origin.AsVec2.Normalized()) + 1f) * 10f, 1f, 15f);
						requestExtraAgent = this.AssignedAgent != null && this._extraAgent == null;
					}
				}
				else
				{
					num4 = 10f;
				}
				float num5 = 100f * (this.IsOuterPos ? 10f : 1f) * num4;
				outGainCondition = ShipPlacementDetachment.PositionCondition.Any;
				resultScore = ((this.AssignedAgent == null) ? 0f : ((ShipPlacementDetachment.CheckCondition(outGainCondition, this.AssignedAgent) ? num5 : (num5 * 0.1f)) + ((this._extraAgent == null) ? 0f : (ShipPlacementDetachment.CheckCondition(outGainCondition, this._extraAgent) ? num5 : (num5 * 0.1f)))));
				resultPossibleGain = num5 * (requestExtraAgent ? 2f : 1f) - resultScore;
			}

			// Token: 0x04000C26 RID: 3110
			private bool _isHighPos;

			// Token: 0x04000C27 RID: 3111
			private Agent _extraAgent;
		}

		// Token: 0x02000182 RID: 386
		private enum PositionCondition
		{
			// Token: 0x04000C2E RID: 3118
			Any,
			// Token: 0x04000C2F RID: 3119
			RangedOrShield,
			// Token: 0x04000C30 RID: 3120
			Ranged
		}
	}
}
