using System;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

// Token: 0x02000005 RID: 5
[ScriptComponentParams("ship_visual_only", "")]
internal class BurningNode : ScriptComponentBehavior
{
	// Token: 0x17000001 RID: 1
	// (get) Token: 0x0600000D RID: 13 RVA: 0x0000249D File Offset: 0x0000069D
	// (set) Token: 0x0600000E RID: 14 RVA: 0x000024A5 File Offset: 0x000006A5
	public Vec2 SailStripLocation { get; private set; }

	// Token: 0x17000002 RID: 2
	// (get) Token: 0x0600000F RID: 15 RVA: 0x000024AE File Offset: 0x000006AE
	// (set) Token: 0x06000010 RID: 16 RVA: 0x000024B6 File Offset: 0x000006B6
	public float ExternalFlameMultiplier { get; private set; }

	// Token: 0x17000003 RID: 3
	// (get) Token: 0x06000011 RID: 17 RVA: 0x000024BF File Offset: 0x000006BF
	// (set) Token: 0x06000012 RID: 18 RVA: 0x000024C7 File Offset: 0x000006C7
	public float BurningTimer { get; set; }

	// Token: 0x17000004 RID: 4
	// (get) Token: 0x06000013 RID: 19 RVA: 0x000024D0 File Offset: 0x000006D0
	public int NodeIndex
	{
		get
		{
			return this._nodeIndex;
		}
	}

	// Token: 0x17000005 RID: 5
	// (get) Token: 0x06000015 RID: 21 RVA: 0x000024F0 File Offset: 0x000006F0
	// (set) Token: 0x06000014 RID: 20 RVA: 0x000024D8 File Offset: 0x000006D8
	public float CurrentFireProgress
	{
		get
		{
			return this._currentFireProgress;
		}
		set
		{
			this._currentFireProgress = MathF.Clamp(value, 0f, 1f);
		}
	}

	// Token: 0x06000016 RID: 22 RVA: 0x000024F8 File Offset: 0x000006F8
	public BurningNode()
	{
		this.SailStripLocation = Vec2.Zero;
		this.ExternalFlameMultiplier = 1f;
		this.BurningTimer = 0f;
	}

	// Token: 0x06000017 RID: 23 RVA: 0x00002528 File Offset: 0x00000728
	public void SetSailStripLocation(Vec2 sailStripLocation)
	{
		this.SailStripLocation = sailStripLocation;
	}

	// Token: 0x06000018 RID: 24 RVA: 0x00002531 File Offset: 0x00000731
	public void SetExternalFlameMultiplier(float externalFlameMultiplier)
	{
		this.ExternalFlameMultiplier = externalFlameMultiplier;
	}

	// Token: 0x06000019 RID: 25 RVA: 0x0000253A File Offset: 0x0000073A
	protected override void OnEditorInit()
	{
		base.OnEditorInit();
		this.FetchEntities();
	}

	// Token: 0x0600001A RID: 26 RVA: 0x00002548 File Offset: 0x00000748
	protected override void OnEditorTick(float dt)
	{
		base.OnEditorTick(dt);
		this.FetchEntities();
		this.TickAux();
	}

	// Token: 0x0600001B RID: 27 RVA: 0x0000255D File Offset: 0x0000075D
	protected override void OnInit()
	{
		base.OnInit();
		this.FetchEntities();
	}

	// Token: 0x0600001C RID: 28 RVA: 0x0000256B File Offset: 0x0000076B
	public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
	{
		return 4;
	}

	// Token: 0x0600001D RID: 29 RVA: 0x0000256E File Offset: 0x0000076E
	protected override void OnTickParallel(float dt)
	{
		this.TickAux();
	}

	// Token: 0x0600001E RID: 30 RVA: 0x00002578 File Offset: 0x00000778
	private void FetchEntities()
	{
		this._light = null;
		this._particle = null;
		WeakGameEntity firstChildEntityWithTag = base.GameEntity.GetFirstChildEntityWithTag("light_entity");
		if (firstChildEntityWithTag != null)
		{
			firstChildEntityWithTag.SetVisibilityExcludeParents(true);
			this._light = (Light)firstChildEntityWithTag.GetComponentAtIndex(0, 1);
		}
		WeakGameEntity firstChildEntityWithTag2 = base.GameEntity.GetFirstChildEntityWithTag("particle_entity");
		if (firstChildEntityWithTag2 != null)
		{
			firstChildEntityWithTag2.SetVisibilityExcludeParents(true);
			this._particle = (ParticleSystem)firstChildEntityWithTag2.GetComponentAtIndex(0, 4);
		}
	}

	// Token: 0x0600001F RID: 31 RVA: 0x00002608 File Offset: 0x00000808
	private void TickAux()
	{
		bool flag = this._currentFireProgress > 0f;
		if (this._particle != null)
		{
			this._particle.SetEnable(flag);
		}
		if (this._light != null)
		{
			this._light.SetVisibility(flag && this._lightEnabled);
		}
		if (this._sparkParticle != null)
		{
			this._sparkParticle.SetEnable(flag && this._sparksEnabled);
		}
		if (flag)
		{
			if (this._particle != null)
			{
				this._particle.SetRuntimeEmissionRateMultiplier(this._currentFireProgress * this.ExternalFlameMultiplier);
			}
			if (this._sparkParticle != null)
			{
				this._sparkParticle.SetRuntimeEmissionRateMultiplier(this._currentFireProgress * this.ExternalFlameMultiplier);
			}
		}
	}

	// Token: 0x06000020 RID: 32 RVA: 0x000026D8 File Offset: 0x000008D8
	public void EnableSparks()
	{
		this._sparksEnabled = true;
		MatrixFrame identity = MatrixFrame.Identity;
		this._sparkParticle = ParticleSystem.CreateParticleSystemAttachedToEntity("psys_dripping_flame", base.GameEntity, ref identity);
	}

	// Token: 0x06000021 RID: 33 RVA: 0x0000270C File Offset: 0x0000090C
	public void CheckWater()
	{
		MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
		float waterLevelAtPosition = base.GameEntity.GetWaterLevelAtPosition(globalFrame.origin.AsVec2, true, false);
		if (globalFrame.origin.z < waterLevelAtPosition)
		{
			this.CurrentFireProgress = 0f;
		}
	}

	// Token: 0x04000002 RID: 2
	private const string LightEntityTag = "light_entity";

	// Token: 0x04000003 RID: 3
	private const string ParticleEntityTag = "particle_entity";

	// Token: 0x04000004 RID: 4
	[EditableScriptComponentVariable(true, "Node Index")]
	private int _nodeIndex = -1;

	// Token: 0x04000005 RID: 5
	private Light _light;

	// Token: 0x04000006 RID: 6
	private ParticleSystem _particle;

	// Token: 0x04000007 RID: 7
	private ParticleSystem _sparkParticle;

	// Token: 0x04000008 RID: 8
	private bool _lightEnabled;

	// Token: 0x04000009 RID: 9
	private bool _sparksEnabled;

	// Token: 0x0400000A RID: 10
	private float _currentFireProgress;
}
