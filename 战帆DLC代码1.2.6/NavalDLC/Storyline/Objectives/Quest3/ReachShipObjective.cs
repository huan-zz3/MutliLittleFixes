using System;
using System.Collections.Generic;
using NavalDLC.Missions.Objects;
using NavalDLC.Storyline.MissionControllers;
using TaleWorlds.Engine;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Quest3
{
	// Token: 0x0200005D RID: 93
	internal class ReachShipObjective : MissionObjective
	{
		// Token: 0x1700011E RID: 286
		// (get) Token: 0x060005C6 RID: 1478 RVA: 0x00022ABE File Offset: 0x00020CBE
		public override string UniqueId
		{
			get
			{
				return "naval_storyline_quest_3_reach_ship_objective";
			}
		}

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x060005C7 RID: 1479 RVA: 0x00022AC5 File Offset: 0x00020CC5
		public override TextObject Name
		{
			get
			{
				return new TextObject("{=4mQj5K5L}Reach the ship", null);
			}
		}

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x060005C8 RID: 1480 RVA: 0x00022AD2 File Offset: 0x00020CD2
		public override TextObject Description
		{
			get
			{
				return new TextObject("{=fE1Atxa5}Get to the Sturgian ship. There may be enemies nearby.", null);
			}
		}

		// Token: 0x060005C9 RID: 1481 RVA: 0x00022AE0 File Offset: 0x00020CE0
		internal ReachShipObjective(Mission mission, Agent gunnarAgent, MissionShip ship)
			: base(mission)
		{
			this._controller = base.Mission.GetMissionBehavior<BlockedEstuaryMissionController>();
			this._playerShip = ship;
			if (gunnarAgent != null && gunnarAgent.IsActive())
			{
				base.AddTarget(new AgentObjectiveTarget(gunnarAgent));
			}
			List<GameEntity> list = this.CollectCheckpoints();
			if (list != null && list.Count > 0)
			{
				foreach (GameEntity gameEntity in list)
				{
					CheckpointObjectiveTarget checkpointObjectiveTarget = new CheckpointObjectiveTarget(gameEntity);
					base.AddTarget(checkpointObjectiveTarget);
					this._targets.Add(checkpointObjectiveTarget);
				}
				if (this._playerShip != null)
				{
					CheckpointObjectiveTarget checkpointObjectiveTarget2 = new CheckpointObjectiveTarget(GameEntity.CreateFromWeakEntity(this._playerShip.GameEntity));
					checkpointObjectiveTarget2.SetName(this._playerShip.ShipOrigin.Name);
					this._targets.Add(checkpointObjectiveTarget2);
					base.AddTarget(checkpointObjectiveTarget2);
				}
				this._targets[0].SetActive(true);
			}
		}

		// Token: 0x060005CA RID: 1482 RVA: 0x00022BF4 File Offset: 0x00020DF4
		private List<GameEntity> CollectCheckpoints()
		{
			List<GameEntity> list = new List<GameEntity>();
			int num = 1;
			for (;;)
			{
				GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("sp_horse_objective_" + num.ToString());
				if (gameEntity == null)
				{
					break;
				}
				list.Add(gameEntity);
				num++;
			}
			return list;
		}

		// Token: 0x060005CB RID: 1483 RVA: 0x00022C44 File Offset: 0x00020E44
		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			if (Agent.Main == null || !Agent.Main.IsActive())
			{
				return;
			}
			for (int i = 0; i < this._targets.Count; i++)
			{
				CheckpointObjectiveTarget checkpointObjectiveTarget = this._targets[i];
				if (checkpointObjectiveTarget.IsInside(Agent.Main.Position))
				{
					checkpointObjectiveTarget.SetActive(false);
					for (int j = i - 1; j >= 0; j--)
					{
						this._targets[j].SetActive(false);
					}
					if (i < this._targets.Count - 1)
					{
						this._targets[i + 1].SetActive(true);
					}
				}
			}
		}

		// Token: 0x060005CC RID: 1484 RVA: 0x00022CEC File Offset: 0x00020EEC
		protected override bool IsActivationRequirementsMet()
		{
			return true;
		}

		// Token: 0x060005CD RID: 1485 RVA: 0x00022CEF File Offset: 0x00020EEF
		protected override bool IsCompletionRequirementsMet()
		{
			return this._controller != null && this._controller.CurrentPhase == BlockedEstuaryMissionController.BattlePhase.Phase3;
		}

		// Token: 0x040002C8 RID: 712
		private BlockedEstuaryMissionController _controller;

		// Token: 0x040002C9 RID: 713
		private List<CheckpointObjectiveTarget> _targets = new List<CheckpointObjectiveTarget>();

		// Token: 0x040002CA RID: 714
		private MissionShip _playerShip;
	}
}
