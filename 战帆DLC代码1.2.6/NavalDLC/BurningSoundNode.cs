using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;

// Token: 0x02000006 RID: 6
internal class BurningSoundNode : ScriptComponentBehavior
{
	// Token: 0x06000023 RID: 35 RVA: 0x00002771 File Offset: 0x00000971
	public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
	{
		return 10;
	}

	// Token: 0x06000024 RID: 36 RVA: 0x00002778 File Offset: 0x00000978
	protected override void OnTick(float dt)
	{
		if (this._enabled)
		{
			this._burningSoundEvent.SetPosition(base.GameEntity.GlobalPosition);
			this._burningSoundEvent.SetParameter("FireIntensity", this._burningSoundEventIntensityParam);
		}
	}

	// Token: 0x06000025 RID: 37 RVA: 0x000027BC File Offset: 0x000009BC
	protected override void OnEditorTick(float dt)
	{
		if (this._enabled)
		{
			this._burningSoundEvent.SetPosition(base.GameEntity.GlobalPosition);
			float num = 0f;
			foreach (BurningNode burningNode in this._burningNodesAttached)
			{
				num += burningNode.CurrentFireProgress;
			}
			this._burningSoundEventIntensityParam = num;
			this._burningSoundEvent.SetPosition(base.GameEntity.GlobalPosition);
			this._burningSoundEvent.SetParameter("FireIntensity", this._burningSoundEventIntensityParam);
		}
		base.GameEntity.IsSelectedOnEditor();
	}

	// Token: 0x06000026 RID: 38 RVA: 0x00002880 File Offset: 0x00000A80
	protected override void OnTickParallel2(float dt)
	{
		if (this._enabled)
		{
			float num = 0f;
			foreach (BurningNode burningNode in this._burningNodesAttached)
			{
				num += burningNode.CurrentFireProgress;
			}
			this._burningSoundEventIntensityParam = num;
		}
	}

	// Token: 0x06000027 RID: 39 RVA: 0x000028EC File Offset: 0x00000AEC
	public void AddBurningNode(BurningNode node)
	{
		if (node.GameEntity.GlobalPosition.DistanceSquared(base.GameEntity.GlobalPosition) < 25f)
		{
			this._burningNodesAttached.Add(node);
		}
	}

	// Token: 0x06000028 RID: 40 RVA: 0x00002930 File Offset: 0x00000B30
	public void StartFire()
	{
		this._enabled = true;
		string text = "event:/mission/ambient/detail/fire/fire_dynamic";
		Mission mission = Mission.Current;
		this._burningSoundEvent = SoundEvent.CreateEventFromString(text, (mission != null) ? mission.Scene : null);
		this._burningSoundEvent.SetPosition(base.GameEntity.GlobalPosition);
		this._burningSoundEvent.Play();
		this._burningSoundEvent.SetParameter("FireIntensity", this._burningSoundEventIntensityParam);
	}

	// Token: 0x06000029 RID: 41 RVA: 0x000029A0 File Offset: 0x00000BA0
	public void StopFire()
	{
		this._enabled = false;
		this._burningSoundEvent.Stop();
		this._burningSoundEvent = null;
		this._burningNodesAttached.Clear();
	}

	// Token: 0x0400000E RID: 14
	private const int MaxNumberOfCachedBurningNodes = 5;

	// Token: 0x0400000F RID: 15
	private const string _soundPath = "event:/mission/ambient/detail/fire/fire_dynamic";

	// Token: 0x04000010 RID: 16
	private const float FireRadius = 5f;

	// Token: 0x04000011 RID: 17
	private const float FireRadiusSq = 25f;

	// Token: 0x04000012 RID: 18
	private List<BurningNode> _burningNodesAttached = new List<BurningNode>();

	// Token: 0x04000013 RID: 19
	private bool _enabled;

	// Token: 0x04000014 RID: 20
	private float _burningSoundEventIntensityParam;

	// Token: 0x04000015 RID: 21
	private SoundEvent _burningSoundEvent;
}
