using System;
using System.Collections.Generic;
using NavalDLC.Missions.Objects;
using NavalDLC.Storyline.MissionControllers;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Quest3
{
	// Token: 0x0200005B RID: 91
	internal class ReachEscapeZoneObjective : MissionObjective
	{
		// Token: 0x1700011B RID: 283
		// (get) Token: 0x060005BA RID: 1466 RVA: 0x0002285F File Offset: 0x00020A5F
		public override string UniqueId
		{
			get
			{
				return "naval_storyline_quest_3_reach_position_objective";
			}
		}

		// Token: 0x1700011C RID: 284
		// (get) Token: 0x060005BB RID: 1467 RVA: 0x00022866 File Offset: 0x00020A66
		public override TextObject Name
		{
			get
			{
				return new TextObject("{=nGpnbplB}Escape Zone", null);
			}
		}

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x060005BC RID: 1468 RVA: 0x00022873 File Offset: 0x00020A73
		public override TextObject Description
		{
			get
			{
				return new TextObject("{=4YtHaWFC}Reach the open seas by avoiding enemy ships.", null);
			}
		}

		// Token: 0x060005BD RID: 1469 RVA: 0x00022880 File Offset: 0x00020A80
		internal ReachEscapeZoneObjective(Mission mission, MissionShip ship, Vec3 position)
			: base(mission)
		{
			this._controller = base.Mission.GetMissionBehavior<BlockedEstuaryMissionController>();
			base.AddTarget(new ShipObjectiveTarget(ship, ship.ShipOrigin.Name, true));
			List<GameEntity> list = this.CollectCheckpoints();
			if (list != null && list.Count > 0)
			{
				foreach (GameEntity gameEntity in list)
				{
					CheckpointObjectiveTarget checkpointObjectiveTarget = new CheckpointObjectiveTarget(gameEntity);
					base.AddTarget(checkpointObjectiveTarget);
					this._targets.Add(checkpointObjectiveTarget);
				}
				this._targets[0].SetActive(true);
				this._targets[this._targets.Count - 1].SetName(new TextObject("{=nGpnbplB}Escape Zone", null));
			}
		}

		// Token: 0x060005BE RID: 1470 RVA: 0x0002296C File Offset: 0x00020B6C
		private List<GameEntity> CollectCheckpoints()
		{
			List<GameEntity> list = new List<GameEntity>();
			int num = 1;
			for (;;)
			{
				GameEntity gameEntity = base.Mission.Scene.FindEntityWithTag("sp_escape_objective_" + num.ToString());
				if (gameEntity == null)
				{
					break;
				}
				list.Add(gameEntity);
				num++;
			}
			return list;
		}

		// Token: 0x060005BF RID: 1471 RVA: 0x000229BC File Offset: 0x00020BBC
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

		// Token: 0x060005C0 RID: 1472 RVA: 0x00022A64 File Offset: 0x00020C64
		protected override bool IsActivationRequirementsMet()
		{
			return this._controller != null && this._controller.CurrentPhase == BlockedEstuaryMissionController.BattlePhase.Phase3;
		}

		// Token: 0x060005C1 RID: 1473 RVA: 0x00022A7E File Offset: 0x00020C7E
		protected override bool IsCompletionRequirementsMet()
		{
			return false;
		}

		// Token: 0x040002C4 RID: 708
		private BlockedEstuaryMissionController _controller;

		// Token: 0x040002C5 RID: 709
		private List<CheckpointObjectiveTarget> _targets = new List<CheckpointObjectiveTarget>();
	}
}
