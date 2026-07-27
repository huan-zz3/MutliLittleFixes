using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

// Token: 0x02000008 RID: 8
[ScriptComponentParams("ship_visual_only", "")]
internal class ShipBurningSystem : ScriptComponentBehavior
{
	// Token: 0x06000045 RID: 69 RVA: 0x00002FEC File Offset: 0x000011EC
	public void DummyFunc()
	{
		Debug.Print(this._stopFire.ToString(), 0, 12, 17592186044416UL);
		Debug.Print(this._stopFire.ToString(), 0, 12, 17592186044416UL);
		Debug.Print(this._startFire.ToString(), 0, 12, 17592186044416UL);
		Debug.Print(this._allFireMode.ToString(), 0, 12, 17592186044416UL);
		Debug.Print(this._hitDebug.ToString(), 0, 12, 17592186044416UL);
	}

	// Token: 0x06000047 RID: 71 RVA: 0x00003134 File Offset: 0x00001334
	protected override void OnInit()
	{
		this.FetchEntities();
		this._randomGenerator = new MBFastRandom((uint)((ulong)base.GameEntity.Pointer & (ulong)(-1)));
	}

	// Token: 0x06000048 RID: 72 RVA: 0x00003169 File Offset: 0x00001369
	protected override void OnTickParallel(float dt)
	{
		if (this._fireStarted)
		{
			this.TickFire(dt);
		}
		this.HandleTemporaryBurningNodes(dt);
	}

	// Token: 0x06000049 RID: 73 RVA: 0x00003181 File Offset: 0x00001381
	public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
	{
		return 4;
	}

	// Token: 0x0600004A RID: 74 RVA: 0x00003184 File Offset: 0x00001384
	private void TickFire(float dt)
	{
		float num = 0f;
		int num2 = 0;
		if (this._railingFire != null)
		{
			this._railingFire.Tick(dt);
			num += this._railingFire.AverageFireProgress;
			num2++;
		}
		if (this._shipDeckFire != null)
		{
			this._shipDeckFire.Tick(dt);
			num += this._shipDeckFire.AverageFireProgress;
			num2++;
		}
		if (this._deckUpgradeFire != null)
		{
			this._deckUpgradeFire.Tick(dt);
			num += this._deckUpgradeFire.AverageFireProgress;
			num2++;
		}
		if (this._mastFire != null)
		{
			this._mastFire.Tick(dt);
			num += this._mastFire.AverageFireProgress;
			num2++;
		}
		if (num2 > 0)
		{
			num /= (float)num2;
			if (num < this._minFireProgressLight)
			{
				using (List<Light>.Enumerator enumerator = this._burningLights.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Light light = enumerator.Current;
						light.GetEntity().SetVisibilityExcludeParents(false);
					}
					goto IL_016A;
				}
			}
			float num3 = (num - this._minFireProgressLight) / (this._maxFireProgressLight - this._minFireProgressLight);
			num3 = MathF.Clamp(num3, 0f, 1f) * this._maxLightIntensity;
			foreach (Light light2 in this._burningLights)
			{
				light2.GetEntity().SetVisibilityExcludeParents(true);
				light2.Intensity = num3;
			}
		}
		IL_016A:
		foreach (BurningNode burningNode in this._railingNodes)
		{
			MatrixFrame globalFrame = burningNode.GameEntity.GetGlobalFrame();
			float waterLevelAtPosition = base.GameEntity.GetWaterLevelAtPosition(globalFrame.origin.AsVec2, true, false);
			if (globalFrame.origin.z < waterLevelAtPosition)
			{
				this._railingFire.SetFlameProgressOfAdvancedNode(burningNode, 0f);
				burningNode.CurrentFireProgress = 0f;
				burningNode.BurningTimer = 3f;
			}
		}
		foreach (BurningNode burningNode2 in this._shipDeckNodes)
		{
			MatrixFrame globalFrame2 = burningNode2.GameEntity.GetGlobalFrame();
			float waterLevelAtPosition2 = base.GameEntity.GetWaterLevelAtPosition(globalFrame2.origin.AsVec2, true, false);
			if (globalFrame2.origin.z < waterLevelAtPosition2)
			{
				this._shipDeckFire.SetFlameProgressOfAdvancedNode(burningNode2, 0f);
				burningNode2.CurrentFireProgress = 0f;
				burningNode2.BurningTimer = 3f;
			}
		}
		foreach (BurningNode burningNode3 in this._deckUpgradeNodes)
		{
			MatrixFrame globalFrame3 = burningNode3.GameEntity.GetGlobalFrame();
			float waterLevelAtPosition3 = base.GameEntity.GetWaterLevelAtPosition(globalFrame3.origin.AsVec2, true, false);
			if (globalFrame3.origin.z < waterLevelAtPosition3)
			{
				this._deckUpgradeFire.SetFlameProgressOfAdvancedNode(burningNode3, 0f);
				burningNode3.CurrentFireProgress = 0f;
				burningNode3.BurningTimer = 3f;
			}
		}
		foreach (BurningNode burningNode4 in this._mastNodes)
		{
			MatrixFrame globalFrame4 = burningNode4.GameEntity.GetGlobalFrame();
			float waterLevelAtPosition4 = base.GameEntity.GetWaterLevelAtPosition(globalFrame4.origin.AsVec2, true, false);
			if (globalFrame4.origin.z < waterLevelAtPosition4)
			{
				this._mastFire.SetFlameProgressOfAdvancedNode(burningNode4, 0f);
				burningNode4.CurrentFireProgress = 0f;
				burningNode4.BurningTimer = 3f;
			}
		}
	}

	// Token: 0x0600004B RID: 75 RVA: 0x000035B0 File Offset: 0x000017B0
	private void FillFireSystemWithNodes(ref List<BurningNode> nodes, ref BurningSystem fire)
	{
		nodes.Sort((BurningNode x, BurningNode y) => x.NodeIndex.CompareTo(x.NodeIndex));
		fire = new BurningSystem(null, 1f / this._spreadRate);
		fire.AddAdvancedNode(nodes[0], nodes[nodes.Count - 1], nodes[1]);
		for (int i = 1; i < nodes.Count - 1; i++)
		{
			fire.AddAdvancedNode(nodes[i], nodes[i - 1], nodes[i + 1]);
			foreach (BurningSoundNode burningSoundNode in this._soundNodes)
			{
				burningSoundNode.AddBurningNode(nodes[i]);
			}
		}
		fire.AddAdvancedNode(nodes[nodes.Count - 1], nodes[nodes.Count - 2], nodes[0]);
		for (int j = 0; j < this._fireStartRandomCount; j++)
		{
			int num = MBRandom.RandomInt(nodes.Count);
			fire.SetFlameProgressOfAdvancedNode(nodes[num], 0.05f + MBRandom.RandomFloat * 0.1f);
		}
	}

	// Token: 0x0600004C RID: 76 RVA: 0x00003710 File Offset: 0x00001910
	private void FetchEntities()
	{
		this._railingNodes.Clear();
		WeakGameEntity firstChildEntityWithTag = base.GameEntity.GetFirstChildEntityWithTag("railing_parent");
		if (firstChildEntityWithTag != null)
		{
			foreach (WeakGameEntity weakGameEntity in firstChildEntityWithTag.GetChildren())
			{
				BurningNode firstScriptOfType = weakGameEntity.GetFirstScriptOfType<BurningNode>();
				if (firstScriptOfType != null)
				{
					this._railingNodes.Add(firstScriptOfType);
				}
			}
		}
		this._shipDeckNodes.Clear();
		WeakGameEntity firstChildEntityWithTag2 = base.GameEntity.GetFirstChildEntityWithTag("ship_deck_parent");
		if (firstChildEntityWithTag2 != null)
		{
			foreach (WeakGameEntity weakGameEntity2 in firstChildEntityWithTag2.GetChildren())
			{
				BurningNode firstScriptOfType2 = weakGameEntity2.GetFirstScriptOfType<BurningNode>();
				if (firstScriptOfType2 != null)
				{
					this._shipDeckNodes.Add(firstScriptOfType2);
				}
			}
		}
		this._deckUpgradeNodes.Clear();
		WeakGameEntity firstChildEntityWithTag3 = base.GameEntity.GetFirstChildEntityWithTag("deck_upgrade_parent");
		if (firstChildEntityWithTag3 != null)
		{
			foreach (WeakGameEntity weakGameEntity3 in firstChildEntityWithTag3.GetChildren())
			{
				BurningNode firstScriptOfType3 = weakGameEntity3.GetFirstScriptOfType<BurningNode>();
				if (firstScriptOfType3 != null)
				{
					this._deckUpgradeNodes.Add(firstScriptOfType3);
				}
			}
		}
		this._mastNodes.Clear();
		WeakGameEntity firstChildEntityWithTag4 = base.GameEntity.GetFirstChildEntityWithTag("mast_parent");
		if (firstChildEntityWithTag4 != null)
		{
			foreach (WeakGameEntity weakGameEntity4 in firstChildEntityWithTag4.GetChildren())
			{
				BurningNode firstScriptOfType4 = weakGameEntity4.GetFirstScriptOfType<BurningNode>();
				if (firstScriptOfType4 != null)
				{
					this._mastNodes.Add(firstScriptOfType4);
				}
			}
		}
		this._burningLights.Clear();
		WeakGameEntity firstChildEntityWithTag5 = base.GameEntity.GetFirstChildEntityWithTag("light_parent");
		if (firstChildEntityWithTag5 != null)
		{
			foreach (WeakGameEntity weakGameEntity5 in firstChildEntityWithTag5.GetChildren())
			{
				Light light = weakGameEntity5.GetComponentAtIndex(0, 1) as Light;
				if (light != null)
				{
					this._burningLights.Add(light);
					if (!this._allFireMode)
					{
						weakGameEntity5.SetVisibilityExcludeParents(false);
					}
				}
			}
		}
		this._soundNodes.Clear();
		WeakGameEntity firstChildEntityWithTag6 = base.GameEntity.GetFirstChildEntityWithTag("sound_parent");
		if (firstChildEntityWithTag6 != null)
		{
			foreach (WeakGameEntity weakGameEntity6 in firstChildEntityWithTag6.GetChildren())
			{
				BurningSoundNode firstScriptOfType5 = weakGameEntity6.GetFirstScriptOfType<BurningSoundNode>();
				if (firstScriptOfType5 != null)
				{
					this._soundNodes.Add(firstScriptOfType5);
				}
			}
		}
	}

	// Token: 0x0600004D RID: 77 RVA: 0x00003A3C File Offset: 0x00001C3C
	private void HandleTemporaryBurningNodes(float dt)
	{
		float num = 0.05f;
		for (int i = 0; i < this._temporaryBurningNodes.Count; i++)
		{
			BurningNode burningNode = this._temporaryBurningNodes[i];
			burningNode.CurrentFireProgress -= dt * num;
			if (burningNode.CurrentFireProgress == 0f)
			{
				this._temporaryBurningNodes[i] = this._temporaryBurningNodes[this._temporaryBurningNodes.Count - 1];
				this._temporaryBurningNodes.Remove(this._temporaryBurningNodes[this._temporaryBurningNodes.Count - 1]);
				i--;
			}
		}
	}

	// Token: 0x0600004E RID: 78 RVA: 0x00003ADC File Offset: 0x00001CDC
	private void RegisterBlowAux(Vec3 collisionPosition, List<BurningNode> nodes, BurningSystem fire)
	{
		float num = 6f;
		float num2 = num * num;
		float num3 = 2f;
		float num4 = 0.75f;
		float num5 = 0.35f;
		foreach (BurningNode burningNode in nodes)
		{
			if (burningNode.CurrentFireProgress < 1f)
			{
				float num6 = burningNode.GameEntity.GetGlobalFrame().origin.DistanceSquared(collisionPosition);
				if (num6 < num2)
				{
					float num7 = MathF.Sqrt(num6);
					float num8 = 1f - MathF.Clamp((num7 - num3) / num, 0f, 1f);
					float num9 = this._randomGenerator.NextFloatRanged(num5, num4) * num8;
					if (fire != null)
					{
						fire.SetFlameProgressOfAdvancedNode(burningNode, burningNode.CurrentFireProgress);
					}
					else if (burningNode.CurrentFireProgress == 0f)
					{
						this._temporaryBurningNodes.Add(burningNode);
					}
					burningNode.CurrentFireProgress += num9;
				}
			}
		}
	}

	// Token: 0x0600004F RID: 79 RVA: 0x00003BF8 File Offset: 0x00001DF8
	public void RegisterBlow(Vec3 collisionPosition)
	{
		this.RegisterBlowAux(collisionPosition, this._railingNodes, this._railingFire);
		this.RegisterBlowAux(collisionPosition, this._shipDeckNodes, this._shipDeckFire);
		this.RegisterBlowAux(collisionPosition, this._deckUpgradeNodes, this._deckUpgradeFire);
		this.RegisterBlowAux(collisionPosition, this._mastNodes, this._mastFire);
	}

	// Token: 0x06000050 RID: 80 RVA: 0x00003C54 File Offset: 0x00001E54
	public void StartFire()
	{
		this._fireStarted = true;
		if (this._railingNodes.Count > 2)
		{
			this.FillFireSystemWithNodes(ref this._railingNodes, ref this._railingFire);
		}
		if (this._shipDeckNodes.Count > 2)
		{
			this.FillFireSystemWithNodes(ref this._shipDeckNodes, ref this._shipDeckFire);
		}
		if (this._deckUpgradeNodes.Count > 2)
		{
			this.FillFireSystemWithNodes(ref this._deckUpgradeNodes, ref this._deckUpgradeFire);
		}
		if (this._mastNodes.Count > 2)
		{
			this.FillFireSystemWithNodes(ref this._mastNodes, ref this._mastFire);
		}
		foreach (BurningSoundNode burningSoundNode in this._soundNodes)
		{
			burningSoundNode.StartFire();
		}
	}

	// Token: 0x04000020 RID: 32
	private const string RailingParentTag = "railing_parent";

	// Token: 0x04000021 RID: 33
	private bool _fireStarted;

	// Token: 0x04000022 RID: 34
	private BurningSystem _railingFire;

	// Token: 0x04000023 RID: 35
	private BurningSystem _shipDeckFire;

	// Token: 0x04000024 RID: 36
	private BurningSystem _deckUpgradeFire;

	// Token: 0x04000025 RID: 37
	private BurningSystem _mastFire;

	// Token: 0x04000026 RID: 38
	private List<BurningNode> _railingNodes = new List<BurningNode>();

	// Token: 0x04000027 RID: 39
	private List<BurningNode> _shipDeckNodes = new List<BurningNode>();

	// Token: 0x04000028 RID: 40
	private List<BurningNode> _deckUpgradeNodes = new List<BurningNode>();

	// Token: 0x04000029 RID: 41
	private List<BurningNode> _mastNodes = new List<BurningNode>();

	// Token: 0x0400002A RID: 42
	private List<BurningSoundNode> _soundNodes = new List<BurningSoundNode>();

	// Token: 0x0400002B RID: 43
	private List<Light> _burningLights = new List<Light>();

	// Token: 0x0400002C RID: 44
	private MBFastRandom _randomGenerator;

	// Token: 0x0400002D RID: 45
	private List<BurningNode> _temporaryBurningNodes = new List<BurningNode>();

	// Token: 0x0400002E RID: 46
	[EditableScriptComponentVariable(true, "Start Fire")]
	private SimpleButton _startFire = new SimpleButton();

	// Token: 0x0400002F RID: 47
	[EditableScriptComponentVariable(true, "Stop Fire")]
	private SimpleButton _stopFire = new SimpleButton();

	// Token: 0x04000030 RID: 48
	[EditableScriptComponentVariable(true, "Spread Rate")]
	private float _spreadRate = 1f;

	// Token: 0x04000031 RID: 49
	[EditableScriptComponentVariable(true, "Fire Start Random Count")]
	private int _fireStartRandomCount = 2;

	// Token: 0x04000032 RID: 50
	[EditableScriptComponentVariable(true, "All Fire Mode")]
	private bool _allFireMode;

	// Token: 0x04000033 RID: 51
	[EditableScriptComponentVariable(true, "Small Hit Debug")]
	private bool _hitDebug;

	// Token: 0x04000034 RID: 52
	[EditableScriptComponentVariable(true, "Min Fire Progress For Light")]
	private float _minFireProgressLight = 0.5f;

	// Token: 0x04000035 RID: 53
	[EditableScriptComponentVariable(true, "Max Fire Progress For Light")]
	private float _maxFireProgressLight = 1f;

	// Token: 0x04000036 RID: 54
	[EditableScriptComponentVariable(true, "Max Light Intensity")]
	private float _maxLightIntensity = 5000f;
}
