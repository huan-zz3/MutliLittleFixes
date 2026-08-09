using System;
using NavalDLC.Storyline.MissionControllers;
using SandBox.AI;
using SandBox.Objects.AnimationPoints;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Storyline.Objects
{
	// Token: 0x02000043 RID: 67
	public class BurnShipObject : UsableMachine
	{
		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x06000516 RID: 1302 RVA: 0x0002179E File Offset: 0x0001F99E
		public bool HasUser
		{
			get
			{
				return this._machineUsePoint.HasUser;
			}
		}

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x06000517 RID: 1303 RVA: 0x000217AB File Offset: 0x0001F9AB
		public override bool IsDeactivated
		{
			get
			{
				return this._used;
			}
		}

		// Token: 0x06000518 RID: 1304 RVA: 0x000217B4 File Offset: 0x0001F9B4
		protected override void OnInit()
		{
			base.OnInit();
			this._controller = Mission.Current.GetMissionBehavior<BlockedEstuaryMissionController>();
			this._machineUsePoint = (DynamicObjectAnimationPoint)base.PilotStandingPoint;
			this._machineUsePoint.IsDeactivated = false;
			this._machineUsePoint.IsDisabledForPlayers = true;
			this._machineUsePoint.LockUserFrames = false;
			this._machineUsePoint.LockUserPositions = false;
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x06000519 RID: 1305 RVA: 0x00021824 File Offset: 0x0001FA24
		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (this._hasUserCached != this.HasUser)
			{
				this._timer = 0f;
				this._hasUserCached = this.HasUser;
			}
			if (this._used)
			{
				return;
			}
			if (this._machineUsePoint.HasUser && !this._stateSet)
			{
				ActionIndexCache actionIndexCache = ActionIndexCache.Create(this._machineUsePoint.LoopStartAction);
				this._machineUsePoint.UserAgent.SetActionChannel(0, ref actionIndexCache, false, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
				this._stateSet = true;
			}
			if (this._machineUsePoint.HasUser)
			{
				this._timer += dt;
			}
			if (this._stateSet && this._machineUsePoint.HasUser && this._timer > this.UseTime)
			{
				this.OnUse();
			}
		}

		// Token: 0x0600051A RID: 1306 RVA: 0x0002190F File Offset: 0x0001FB0F
		public override TextObject GetDescriptionText(WeakGameEntity gameEntity)
		{
			return new TextObject("{=eAnAZNib}Barrel of oil", null);
		}

		// Token: 0x0600051B RID: 1307 RVA: 0x0002191C File Offset: 0x0001FB1C
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 2 | base.GetTickRequirement();
		}

		// Token: 0x0600051C RID: 1308 RVA: 0x00021926 File Offset: 0x0001FB26
		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			TextObject textObject = GameTexts.FindText("str_key_action", null);
			textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13), 1f));
			return textObject;
		}

		// Token: 0x0600051D RID: 1309 RVA: 0x00021955 File Offset: 0x0001FB55
		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new UsablePlaceAI(this);
		}

		// Token: 0x0600051E RID: 1310 RVA: 0x0002195D File Offset: 0x0001FB5D
		private void OnUse()
		{
			this._machineUsePoint.UserAgent.StopUsingGameObject(true, 1);
			base.SetDisabled(true);
			this._used = true;
			this._controller.OnBurningMachineUsed(this);
		}

		// Token: 0x04000296 RID: 662
		public float UseTime = 5f;

		// Token: 0x04000297 RID: 663
		private DynamicObjectAnimationPoint _machineUsePoint;

		// Token: 0x04000298 RID: 664
		private BlockedEstuaryMissionController _controller;

		// Token: 0x04000299 RID: 665
		private bool _hasUserCached;

		// Token: 0x0400029A RID: 666
		private bool _stateSet;

		// Token: 0x0400029B RID: 667
		private bool _used;

		// Token: 0x0400029C RID: 668
		private float _timer;
	}
}
