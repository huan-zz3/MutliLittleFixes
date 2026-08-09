using System;
using System.Collections.Generic;
using NavalDLC.Missions.Objects.UsableMachines;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
	// Token: 0x02000012 RID: 18
	public class PlankBridgeSteppedAgentManager : ScriptComponentBehavior
	{
		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600009A RID: 154 RVA: 0x00006869 File Offset: 0x00004A69
		// (set) Token: 0x0600009B RID: 155 RVA: 0x00006871 File Offset: 0x00004A71
		public Vec3 WeightedPosition { get; private set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600009C RID: 156 RVA: 0x0000687A File Offset: 0x00004A7A
		// (set) Token: 0x0600009D RID: 157 RVA: 0x00006882 File Offset: 0x00004A82
		public float TotalMass { get; private set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x0600009E RID: 158 RVA: 0x0000688B File Offset: 0x00004A8B
		// (set) Token: 0x0600009F RID: 159 RVA: 0x00006893 File Offset: 0x00004A93
		public int AgentCount { get; private set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x060000A0 RID: 160 RVA: 0x0000689C File Offset: 0x00004A9C
		// (set) Token: 0x060000A1 RID: 161 RVA: 0x000068A4 File Offset: 0x00004AA4
		public ShipAttachmentMachine.ShipBridgeNavmeshHolder NavmeshHolder { get; private set; }

		// Token: 0x060000A2 RID: 162 RVA: 0x000068AD File Offset: 0x00004AAD
		public void SetNavmeshHolder(ShipAttachmentMachine.ShipBridgeNavmeshHolder navmeshHolder)
		{
			this.NavmeshHolder = navmeshHolder;
			this._accumulatedCostDict = new Dictionary<int, float>();
			this._accumulatedCostDict.Add(this.NavmeshHolder.GetFace1GroupIndex(), 0f);
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x000068DC File Offset: 0x00004ADC
		protected override void OnInit()
		{
			base.OnInit();
			this.WeightedPosition = Vec3.Zero;
			this.TotalMass = 0f;
			this.AgentCount = 0;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00006904 File Offset: 0x00004B04
		public void ClearAgentWeightAndPositionInformation()
		{
			this.WeightedPosition = Vec3.Zero;
			this.TotalMass = 0f;
			this.AgentCount = 0;
			ShipAttachmentMachine.ShipBridgeNavmeshHolder navmeshHolder = this.NavmeshHolder;
			if (navmeshHolder == null)
			{
				return;
			}
			navmeshHolder.GameEntity.SetCostAdderForAttachedFaces(0f);
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x0000694C File Offset: 0x00004B4C
		public void AddAgentWeightAndPositionInformation(Agent agent)
		{
			float totalMass = agent.GetTotalMass();
			int currentNavigationFaceId = agent.GetCurrentNavigationFaceId();
			if (this.NavmeshHolder != null && this._accumulatedCostDict.ContainsKey(currentNavigationFaceId))
			{
				Dictionary<int, float> accumulatedCostDict = this._accumulatedCostDict;
				int num = currentNavigationFaceId;
				accumulatedCostDict[num] += 7.5f;
				Mission.Current.SetNavigationFaceCostWithIdAroundPosition(currentNavigationFaceId, agent.Position, this._accumulatedCostDict[currentNavigationFaceId]);
			}
			Vec3 position = agent.Position;
			if (base.GameEntity.GetGlobalFrame().origin.DistanceSquared(position) < 25f)
			{
				this.WeightedPosition += totalMass * agent.Position;
				this.TotalMass += totalMass;
				int num = this.AgentCount;
				this.AgentCount = num + 1;
			}
		}

		// Token: 0x04000070 RID: 112
		private Dictionary<int, float> _accumulatedCostDict;
	}
}
