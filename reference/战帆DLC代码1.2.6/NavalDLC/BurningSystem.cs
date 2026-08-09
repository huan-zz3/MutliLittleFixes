using System;
using System.Collections.Generic;
using NavalDLC.Missions.Objects;
using TaleWorlds.Engine;
using TaleWorlds.Library;

// Token: 0x02000007 RID: 7
internal class BurningSystem
{
	// Token: 0x17000006 RID: 6
	// (get) Token: 0x0600002A RID: 42 RVA: 0x000029C6 File Offset: 0x00000BC6
	// (set) Token: 0x0600002B RID: 43 RVA: 0x000029CE File Offset: 0x00000BCE
	public bool AdvancedSpread { get; private set; }

	// Token: 0x17000007 RID: 7
	// (get) Token: 0x0600002C RID: 44 RVA: 0x000029D7 File Offset: 0x00000BD7
	public float AverageFireProgress
	{
		get
		{
			return this._averageFireProgress;
		}
	}

	// Token: 0x17000008 RID: 8
	// (get) Token: 0x0600002D RID: 45 RVA: 0x000029DF File Offset: 0x00000BDF
	// (set) Token: 0x0600002E RID: 46 RVA: 0x000029E7 File Offset: 0x00000BE7
	public float SpreadRate { get; set; }

	// Token: 0x17000009 RID: 9
	// (get) Token: 0x0600002F RID: 47 RVA: 0x000029F0 File Offset: 0x00000BF0
	// (set) Token: 0x06000030 RID: 48 RVA: 0x000029F8 File Offset: 0x00000BF8
	public RopeSegment BurnedRope { get; private set; }

	// Token: 0x1700000A RID: 10
	// (get) Token: 0x06000031 RID: 49 RVA: 0x00002A01 File Offset: 0x00000C01
	// (set) Token: 0x06000032 RID: 50 RVA: 0x00002A09 File Offset: 0x00000C09
	public PulleySystem BurnedPulley { get; private set; }

	// Token: 0x1700000B RID: 11
	// (get) Token: 0x06000033 RID: 51 RVA: 0x00002A12 File Offset: 0x00000C12
	public MBReadOnlyList<BurningNode> BurningNodes
	{
		get
		{
			return this._burningNodes;
		}
	}

	// Token: 0x06000034 RID: 52 RVA: 0x00002A1C File Offset: 0x00000C1C
	public BurningSystem(GameEntity fireRoot, float spreadRate)
	{
		this._fireRoot = fireRoot;
		this.SpreadRate = spreadRate;
		this.BurnedRope = null;
		this.BurnedPulley = null;
		this.AdvancedSpread = false;
		this._lastBurningIndex = 0;
	}

	// Token: 0x06000035 RID: 53 RVA: 0x00002A78 File Offset: 0x00000C78
	public BurningSystem(GameEntity fireRoot, float spreadRate, PulleySystem pulley)
	{
		this._fireRoot = fireRoot;
		this.SpreadRate = spreadRate;
		this.BurnedRope = null;
		this.BurnedPulley = pulley;
		this._lastBurningIndex = 0;
	}

	// Token: 0x06000036 RID: 54 RVA: 0x00002ACC File Offset: 0x00000CCC
	public BurningSystem(GameEntity fireRoot, float spreadRate, RopeSegment rope)
	{
		this._fireRoot = fireRoot;
		this.SpreadRate = spreadRate;
		this.BurnedRope = rope;
		this.BurnedPulley = null;
		this._lastBurningIndex = 0;
	}

	// Token: 0x06000037 RID: 55 RVA: 0x00002B1F File Offset: 0x00000D1F
	public void Tick(float dt)
	{
		if (this.AdvancedSpread)
		{
			this.DoAdvancedSpread(dt);
			return;
		}
		this.DoSimpleSpread(dt);
	}

	// Token: 0x06000038 RID: 56 RVA: 0x00002B38 File Offset: 0x00000D38
	private void DoAdvancedSpread(float dt)
	{
		int num = (this._currentAdvancedSpreadFlameIndex + 1) % 2;
		this._averageFireProgress = 0f;
		foreach (BurningSystem.AdvancedSpreadNode advancedSpreadNode in this._advancedNodes.Values)
		{
			float num2 = advancedSpreadNode.CurrentFlame[this._currentAdvancedSpreadFlameIndex];
			this._averageFireProgress += num2;
			if (num2 < 1f)
			{
				if (advancedSpreadNode.Node.BurningTimer > 0f)
				{
					advancedSpreadNode.Node.BurningTimer -= dt;
				}
				else
				{
					if (num2 > 0f)
					{
						num2 += this.SpreadRate * dt;
					}
					if (num2 < 0.01f && advancedSpreadNode.NextNode != null && this._advancedNodes[advancedSpreadNode.NextNode].CurrentFlame[this._currentAdvancedSpreadFlameIndex] > 0.5f)
					{
						num2 = 0.01f;
					}
					if (num2 < 0.01f && advancedSpreadNode.PrevNode != null && this._advancedNodes[advancedSpreadNode.PrevNode].CurrentFlame[this._currentAdvancedSpreadFlameIndex] > 0.5f)
					{
						num2 = 0.01f;
					}
					num2 = MathF.Min(num2, 1f);
					advancedSpreadNode.CurrentFlame[num] = num2;
					advancedSpreadNode.Node.CurrentFireProgress = num2;
				}
			}
		}
		if (this._advancedNodes.Count > 0)
		{
			this._averageFireProgress /= (float)this._advancedNodes.Count;
		}
		this._currentAdvancedSpreadFlameIndex = num;
	}

	// Token: 0x06000039 RID: 57 RVA: 0x00002CD8 File Offset: 0x00000ED8
	private void DoSimpleSpread(float dt)
	{
		if (this._lastBurningIndex != -1 && this._lastBurningIndex != this._burningNodes.Count)
		{
			BurningNode burningNode = this._burningNodes[this._lastBurningIndex];
			burningNode.CurrentFireProgress += this.SpreadRate * dt;
			if (burningNode.CurrentFireProgress >= 1f)
			{
				this._lastBurningIndex++;
			}
		}
	}

	// Token: 0x0600003A RID: 58 RVA: 0x00002D41 File Offset: 0x00000F41
	public void SetSpreadRate(float value)
	{
		this.SpreadRate = value;
	}

	// Token: 0x0600003B RID: 59 RVA: 0x00002D4A File Offset: 0x00000F4A
	public void AddNewNode(BurningNode node)
	{
		this._burningNodes.Add(node);
	}

	// Token: 0x0600003C RID: 60 RVA: 0x00002D58 File Offset: 0x00000F58
	public void AddAdvancedNode(BurningNode node, BurningNode prevNode, BurningNode nextNode)
	{
		this.AdvancedSpread = true;
		BurningSystem.AdvancedSpreadNode advancedSpreadNode = new BurningSystem.AdvancedSpreadNode();
		advancedSpreadNode.Node = node;
		advancedSpreadNode.NextNode = nextNode;
		advancedSpreadNode.PrevNode = prevNode;
		advancedSpreadNode.CurrentFlame = new float[2];
		node.CurrentFireProgress = 0f;
		advancedSpreadNode.CurrentFlame[0] = node.CurrentFireProgress;
		advancedSpreadNode.CurrentFlame[1] = advancedSpreadNode.CurrentFlame[0];
		this._advancedNodes.Add(node, advancedSpreadNode);
	}

	// Token: 0x0600003D RID: 61 RVA: 0x00002DCC File Offset: 0x00000FCC
	public void SetFlameProgressOfAdvancedNode(BurningNode node, float progress)
	{
		BurningSystem.AdvancedSpreadNode advancedSpreadNode;
		if (this._advancedNodes.TryGetValue(node, out advancedSpreadNode))
		{
			advancedSpreadNode.CurrentFlame[this._currentAdvancedSpreadFlameIndex] = progress;
		}
	}

	// Token: 0x0600003E RID: 62 RVA: 0x00002DF8 File Offset: 0x00000FF8
	public float GetFlameProgress()
	{
		if (this._lastBurningIndex >= this._burningNodes.Count)
		{
			return 1f;
		}
		if (this._lastBurningIndex >= 0)
		{
			return ((float)this._lastBurningIndex + this._burningNodes[this._lastBurningIndex].CurrentFireProgress) / (float)this._burningNodes.Count;
		}
		return 0f;
	}

	// Token: 0x0600003F RID: 63 RVA: 0x00002E58 File Offset: 0x00001058
	public bool FireStarted()
	{
		return this._lastBurningIndex != -1;
	}

	// Token: 0x06000040 RID: 64 RVA: 0x00002E66 File Offset: 0x00001066
	public bool FlamesReachedEnd()
	{
		return this._lastBurningIndex == this._burningNodes.Count;
	}

	// Token: 0x06000041 RID: 65 RVA: 0x00002E7C File Offset: 0x0000107C
	public void Remove()
	{
		foreach (BurningNode burningNode in this._burningNodes)
		{
			RopeSegmentCosmetics firstScriptOfType = burningNode.GameEntity.GetFirstScriptOfType<RopeSegmentCosmetics>();
			if (firstScriptOfType != null)
			{
				if (this.BurnedRope != null)
				{
					this.BurnedRope.DeregisterRopeSegmentCosmetics(firstScriptOfType);
				}
				if (this.BurnedPulley != null)
				{
					this.BurnedPulley.DeregisterRopeSegmentCosmetics(firstScriptOfType);
				}
			}
			burningNode.GameEntity.Remove(33);
		}
		if (this._fireRoot != null)
		{
			this._fireRoot.Remove(33);
		}
	}

	// Token: 0x06000042 RID: 66 RVA: 0x00002F30 File Offset: 0x00001130
	public float GetBurningAnimationDuration()
	{
		return (float)this._burningNodes.Count / this.SpreadRate;
	}

	// Token: 0x06000043 RID: 67 RVA: 0x00002F48 File Offset: 0x00001148
	public void SetExternalFlameMultiplier(float value)
	{
		foreach (BurningNode burningNode in this._burningNodes)
		{
			burningNode.SetExternalFlameMultiplier(value);
		}
	}

	// Token: 0x06000044 RID: 68 RVA: 0x00002F9C File Offset: 0x0000119C
	public void CheckWater()
	{
		foreach (BurningNode burningNode in this._burningNodes)
		{
			burningNode.CheckWater();
		}
	}

	// Token: 0x04000016 RID: 22
	private GameEntity _fireRoot;

	// Token: 0x04000017 RID: 23
	private MBList<BurningNode> _burningNodes = new MBList<BurningNode>();

	// Token: 0x04000018 RID: 24
	private int _lastBurningIndex = -1;

	// Token: 0x04000019 RID: 25
	private int _currentAdvancedSpreadFlameIndex;

	// Token: 0x0400001A RID: 26
	private Dictionary<BurningNode, BurningSystem.AdvancedSpreadNode> _advancedNodes = new Dictionary<BurningNode, BurningSystem.AdvancedSpreadNode>();

	// Token: 0x0400001B RID: 27
	private float _averageFireProgress;

	// Token: 0x0200017C RID: 380
	private class AdvancedSpreadNode
	{
		// Token: 0x04000C15 RID: 3093
		internal BurningNode Node;

		// Token: 0x04000C16 RID: 3094
		internal BurningNode NextNode;

		// Token: 0x04000C17 RID: 3095
		internal BurningNode PrevNode;

		// Token: 0x04000C18 RID: 3096
		internal float[] CurrentFlame;
	}
}
