using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.Objects.UsableMachines
{
	// Token: 0x020000AD RID: 173
	public class AgentBindsMachine : UsableMachine
	{
		// Token: 0x1700025B RID: 603
		// (get) Token: 0x06000D36 RID: 3382 RVA: 0x00068997 File Offset: 0x00066B97
		// (set) Token: 0x06000D37 RID: 3383 RVA: 0x0006899F File Offset: 0x00066B9F
		public ShipOarMachine ShipOarMachine { get; private set; }

		// Token: 0x06000D38 RID: 3384 RVA: 0x000689A8 File Offset: 0x00066BA8
		public void SetOarMachine(ShipOarMachine shipOarMachine)
		{
			this.ShipOarMachine = shipOarMachine;
		}

		// Token: 0x1700025C RID: 604
		// (get) Token: 0x06000D39 RID: 3385 RVA: 0x000689B1 File Offset: 0x00066BB1
		public bool HasCaptive
		{
			get
			{
				return this.ShipOarMachine.PilotStandingPoint.HasUser;
			}
		}

		// Token: 0x06000D3A RID: 3386 RVA: 0x000689C3 File Offset: 0x00066BC3
		protected override void OnInit()
		{
			base.OnInit();
			base.SetScriptComponentToTick(this.GetTickRequirement());
			base.PilotStandingPoint.AddComponent(new ResetAnimationOnStopUsageComponent(ActionIndexCache.act_none, false));
		}

		// Token: 0x06000D3B RID: 3387 RVA: 0x000689ED File Offset: 0x00066BED
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 2;
		}

		// Token: 0x06000D3C RID: 3388 RVA: 0x000689F0 File Offset: 0x00066BF0
		protected override void OnTick(float dt)
		{
			ShipOarMachine shipOarMachine = this.ShipOarMachine;
			Agent agent = ((shipOarMachine != null) ? shipOarMachine.PilotAgent : null);
			base.PilotStandingPoint.SetIsDeactivatedSynched(agent == null);
			if (base.PilotAgent != null)
			{
				if (base.PilotAgent.SetActionChannel(0, ref this._breakChainsShortAction, false, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true))
				{
					if (base.PilotAgent.GetCurrentActionProgress(0) > 0.99f)
					{
						base.PilotAgent.SetActionChannel(0, ref ActionIndexCache.act_none, true, 0L, 0f, 1f, -0.2f, 0.4f, 0f, false, -0.2f, 0, true);
						base.PilotAgent.StopUsingGameObject(true, 1);
						if (agent != null)
						{
							agent.StopUsingGameObject(true, 1);
							agent.ClearHandInverseKinematics();
							return;
						}
					}
				}
				else
				{
					base.PilotAgent.StopUsingGameObject(true, 1);
					agent.MakeVoice(SkinVoiceManager.VoiceType.MpThanks, 2);
				}
			}
		}

		// Token: 0x06000D3D RID: 3389 RVA: 0x00068AE3 File Offset: 0x00066CE3
		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			TextObject textObject = new TextObject("{=fEQAPJ2e}{KEY} Use", null);
			textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13), 1f));
			return textObject;
		}

		// Token: 0x06000D3E RID: 3390 RVA: 0x00068B12 File Offset: 0x00066D12
		public override TextObject GetDescriptionText(WeakGameEntity gameEntity)
		{
			return new TextObject("{=ut9C8hA9}Chains", null);
		}

		// Token: 0x04000833 RID: 2099
		private readonly ActionIndexCache _breakChainsShortAction = ActionIndexCache.Create("act_cutscene_break_chains_short");
	}
}
