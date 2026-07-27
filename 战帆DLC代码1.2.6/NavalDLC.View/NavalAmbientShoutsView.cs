using System;
using System.Collections.Generic;
using NavalDLC.Missions;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace NavalDLC.View
{
	// Token: 0x02000004 RID: 4
	internal class NavalAmbientShoutsView : MissionView
	{
		// Token: 0x06000020 RID: 32 RVA: 0x00002A20 File Offset: 0x00000C20
		public override void OnBehaviorInitialize()
		{
			base.OnBehaviorInitialize();
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this._navalAgentsLogic = base.Mission.GetMissionBehavior<NavalAgentsLogic>();
			this._navalShipsLogic.ShipSunkEvent += this.OnShipSunk;
			this._navalShipsLogic.ShipHookThrowEvent += this.OnShipHookThrow;
			this._navalShipsLogic.SailsDeadEvent += this.OnSailsDead;
			this._navalShipsLogic.ShipLowHealthEvent += this.OnShipLowHealth;
			this._navalShipsLogic.ShipAboutToBeRammedEvent += this.OnShipAboutToBeRammed;
			this._navalShipsLogic.ShipAttachmentLostEvent += this.OnShipAttachmentLost;
			this._navalShipsLogic.BoardingOrderEvent += this.OnBoardingOrder;
			this._navalShipsLogic.CutLooseOrderEvent += this.OnCutLooseOrder;
			this._navalShipsLogic.BridgeConnectedEvent += this.OnBridgeConnected;
			this._hooksLaunchedTimer = new MissionTimer(15f);
			this._shipGotHookedTimer = new MissionTimer(15f);
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002B44 File Offset: 0x00000D44
		public override void OnRemoveBehavior()
		{
			base.OnRemoveBehavior();
			this._navalShipsLogic.ShipSunkEvent -= this.OnShipSunk;
			this._navalShipsLogic.ShipHookThrowEvent -= this.OnShipHookThrow;
			this._navalShipsLogic.SailsDeadEvent -= this.OnSailsDead;
			this._navalShipsLogic.ShipLowHealthEvent -= this.OnShipLowHealth;
			this._navalShipsLogic.ShipAboutToBeRammedEvent -= this.OnShipAboutToBeRammed;
			this._navalShipsLogic.ShipAttachmentLostEvent -= this.OnShipAttachmentLost;
			this._navalShipsLogic.BoardingOrderEvent -= this.OnBoardingOrder;
			this._navalShipsLogic.CutLooseOrderEvent -= this.OnCutLooseOrder;
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002C10 File Offset: 0x00000E10
		public void OnShipSunk(MissionShip ship)
		{
			if (ship.Team != null)
			{
				if (ship.Team.IsPlayerAlly)
				{
					if (this.IsMainAgentOnTheShip(ship))
					{
						this.PlayShoutFromShip(NavalAmbientShoutsView.Shouts.PlayerShipSinking, ship, 5);
						return;
					}
					NavalAmbientShoutsView.Shouts shouts = NavalAmbientShoutsView.Shouts.AllySinking;
					Agent main = Agent.Main;
					this.PlayShoutFromShip(shouts, (main != null) ? main.GetComponent<AgentNavalComponent>().SteppedShip : null, 5);
					return;
				}
				else
				{
					NavalAmbientShoutsView.Shouts shouts2 = NavalAmbientShoutsView.Shouts.EnemySinking;
					Agent main2 = Agent.Main;
					this.PlayShoutFromShip(shouts2, (main2 != null) ? main2.GetComponent<AgentNavalComponent>().SteppedShip : null, 5);
				}
			}
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002C84 File Offset: 0x00000E84
		public void OnShipHookThrow(MissionShip hookingShip, MissionShip hookedShip)
		{
			if (hookingShip.Team != null && hookedShip.Team != null)
			{
				bool flag = hookingShip.Team.IsPlayerAlly && !hookedShip.Team.IsPlayerAlly;
				if (flag && this._hooksLaunchedTimer.Check(true))
				{
					this.PlayShoutFromShip(NavalAmbientShoutsView.Shouts.HooksLaunched, hookingShip, 3);
					return;
				}
				if (!flag && this._shipGotHookedTimer.Check(true))
				{
					this.PlayShoutFromShip(NavalAmbientShoutsView.Shouts.AllyShipGotHooked, hookedShip, 3);
				}
			}
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002CF5 File Offset: 0x00000EF5
		public void OnSailsDead(MissionShip ship)
		{
			if (this.IsMainAgentOnTheShip(ship))
			{
				this.PlayShoutFromShip(NavalAmbientShoutsView.Shouts.SailsDead, ship, 5);
			}
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00002D09 File Offset: 0x00000F09
		public void OnShipLowHealth(MissionShip ship)
		{
			if (this.IsMainAgentOnTheShip(ship))
			{
				this.PlayShoutFromShip(NavalAmbientShoutsView.Shouts.ShipLowHealth, ship, 5);
			}
		}

		// Token: 0x06000026 RID: 38 RVA: 0x00002D1D File Offset: 0x00000F1D
		public void OnCutLooseOrder(MissionShip ship)
		{
			if (this.IsMainAgentOnTheShip(ship))
			{
				this.PlayShoutFromShip(NavalAmbientShoutsView.Shouts.CutLooseOrder, ship, 3);
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00002D32 File Offset: 0x00000F32
		public void OnBoardingOrder(MissionShip boardingShip, MissionShip boardedShip)
		{
			if (this.IsMainAgentOnTheShip(boardingShip))
			{
				this.PlayShoutFromShip(NavalAmbientShoutsView.Shouts.BoardingOrder, boardingShip, 5);
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002D47 File Offset: 0x00000F47
		public void OnBridgeConnected(MissionShip sourceShip, MissionShip targetShip)
		{
			if (this.IsMainAgentOnTheShip(sourceShip))
			{
				this.PlayShoutFromShip(NavalAmbientShoutsView.Shouts.BoardingOrder, sourceShip, 5);
				return;
			}
			if (this.IsMainAgentOnTheShip(targetShip))
			{
				this.PlayShoutFromShip(NavalAmbientShoutsView.Shouts.BoardingOrder, targetShip, 5);
			}
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002D70 File Offset: 0x00000F70
		public void OnShipAboutToBeRammed(MissionShip rammingShip, MissionShip rammedShip, float distance, float speedInRamDirection)
		{
			if (speedInRamDirection > 3f && rammedShip.Team != null && rammingShip.Team != null)
			{
				float currentTime = Mission.Current.CurrentTime;
				float num;
				if (rammedShip.Team.IsPlayerAlly && (!this._shipRammingShoutCooldown.TryGetValue(rammedShip, out num) || currentTime - num > 15f))
				{
					this.PlayShoutFromShip(NavalAmbientShoutsView.Shouts.GettingRammed, rammedShip, 9);
					this._shipRammingShoutCooldown[rammedShip] = currentTime;
				}
				if (rammingShip.Team.IsPlayerAlly && (!this._shipRammingShoutCooldown.TryGetValue(rammingShip, out num) || currentTime - num > 15f))
				{
					this.PlayShoutFromShip(NavalAmbientShoutsView.Shouts.GettingRammed, rammingShip, 3);
					this._shipRammingShoutCooldown[rammingShip] = currentTime;
				}
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002E25 File Offset: 0x00001025
		public void OnShipAttachmentLost(MissionShip hookingShip, MissionShip hookedShip)
		{
			if (this.IsMainAgentOnTheShip(hookingShip) && hookingShip.ComputeActiveShipAttachmentCount() == 1)
			{
				this.PlayShoutFromShip(NavalAmbientShoutsView.Shouts.HooksLost, hookingShip, 3);
				return;
			}
			if (this.IsMainAgentOnTheShip(hookingShip) && hookedShip.ComputeActiveShipAttachmentCount() == 1)
			{
				this.PlayShoutFromShip(NavalAmbientShoutsView.Shouts.HooksLost, hookedShip, 3);
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002E5E File Offset: 0x0000105E
		private bool IsMainAgentOnTheShip(MissionShip ship)
		{
			return Agent.Main != null && Agent.Main.IsActive() && ship.GetIsAgentOnShip(Agent.Main, false);
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002E84 File Offset: 0x00001084
		private void PlayShoutFromShip(NavalAmbientShoutsView.Shouts shoutType, MissionShip ship, int numberOfAgentsToShout)
		{
			if (ship != null)
			{
				string eventName = this.GetEventName(shoutType);
				MBReadOnlyList<Agent> activeAgentsOfShip = this._navalAgentsLogic.GetActiveAgentsOfShip(ship);
				if (activeAgentsOfShip != null)
				{
					int count = activeAgentsOfShip.Count;
					int num = 0;
					while (num < numberOfAgentsToShout && num < count)
					{
						Vec3 position = Extensions.GetRandomElement<Agent>(activeAgentsOfShip).Position;
						SoundManager.StartOneShotEvent(eventName, ref position);
						num++;
					}
				}
			}
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002EDC File Offset: 0x000010DC
		private string GetEventName(NavalAmbientShoutsView.Shouts shoutType)
		{
			switch (shoutType)
			{
			case NavalAmbientShoutsView.Shouts.AllySinking:
				return "event:/alerts/naval/ally_sunk";
			case NavalAmbientShoutsView.Shouts.EnemySinking:
				return "event:/alerts/naval/enemy_sunk";
			case NavalAmbientShoutsView.Shouts.GettingRammed:
				return "event:/alerts/naval/getting_rammed";
			case NavalAmbientShoutsView.Shouts.HooksLaunched:
				return "event:/alerts/naval/hooks_launch";
			case NavalAmbientShoutsView.Shouts.HooksLost:
				return "event:/alerts/naval/hooks_lost";
			case NavalAmbientShoutsView.Shouts.SailsDead:
				return "event:/alerts/naval/sails_dead";
			case NavalAmbientShoutsView.Shouts.AllyShipGotHooked:
				return "event:/alerts/naval/ship_got_hooked";
			case NavalAmbientShoutsView.Shouts.ShipLowHealth:
				return "event:/alerts/naval/ship_low_health";
			case NavalAmbientShoutsView.Shouts.PlayerShipSinking:
				return "event:/alerts/naval/ship_sinking";
			case NavalAmbientShoutsView.Shouts.BoardingOrder:
				return "event:/alerts/nods/attack";
			case NavalAmbientShoutsView.Shouts.CutLooseOrder:
				return "event:/alerts/naval/ship_separate";
			case NavalAmbientShoutsView.Shouts.Engaging:
				return "event:/alerts/naval/engaging";
			default:
				return "";
			}
		}

		// Token: 0x0400000D RID: 13
		private const float RammingShoutCooldown = 15f;

		// Token: 0x0400000E RID: 14
		private const float HooksTimer = 15f;

		// Token: 0x0400000F RID: 15
		private NavalShipsLogic _navalShipsLogic;

		// Token: 0x04000010 RID: 16
		private NavalAgentsLogic _navalAgentsLogic;

		// Token: 0x04000011 RID: 17
		private readonly Dictionary<MissionShip, float> _shipRammingShoutCooldown = new Dictionary<MissionShip, float>();

		// Token: 0x04000012 RID: 18
		private MissionTimer _hooksLaunchedTimer;

		// Token: 0x04000013 RID: 19
		private MissionTimer _shipGotHookedTimer;

		// Token: 0x02000040 RID: 64
		private enum Shouts
		{
			// Token: 0x040000DD RID: 221
			AllySinking,
			// Token: 0x040000DE RID: 222
			EnemySinking,
			// Token: 0x040000DF RID: 223
			GettingRammed,
			// Token: 0x040000E0 RID: 224
			HooksLaunched,
			// Token: 0x040000E1 RID: 225
			HooksLost,
			// Token: 0x040000E2 RID: 226
			SailsDead,
			// Token: 0x040000E3 RID: 227
			AllyShipGotHooked,
			// Token: 0x040000E4 RID: 228
			ShipLowHealth,
			// Token: 0x040000E5 RID: 229
			PlayerShipSinking,
			// Token: 0x040000E6 RID: 230
			BoardingOrder,
			// Token: 0x040000E7 RID: 231
			CutLooseOrder,
			// Token: 0x040000E8 RID: 232
			Engaging
		}
	}
}
