using System;
using NavalDLC.Missions.Objects;
using NavalDLC.Storyline.MissionControllers;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Quest3
{
	// Token: 0x02000059 RID: 89
	internal class BurnShipObjective : MissionObjective
	{
		// Token: 0x17000113 RID: 275
		// (get) Token: 0x060005A2 RID: 1442 RVA: 0x000226C8 File Offset: 0x000208C8
		public override string UniqueId
		{
			get
			{
				return "naval_storyline_quest_3_burn_ship_objective";
			}
		}

		// Token: 0x17000114 RID: 276
		// (get) Token: 0x060005A3 RID: 1443 RVA: 0x000226CF File Offset: 0x000208CF
		public override TextObject Name
		{
			get
			{
				return new TextObject("{=Ry0xZCO2}Ram Enemy Ship", null);
			}
		}

		// Token: 0x17000115 RID: 277
		// (get) Token: 0x060005A4 RID: 1444 RVA: 0x000226DC File Offset: 0x000208DC
		public override TextObject Description
		{
			get
			{
				return new TextObject("{=BHR7DWsG}Destroy the enemy ship by ramming it with your fireship.", null);
			}
		}

		// Token: 0x060005A5 RID: 1445 RVA: 0x000226E9 File Offset: 0x000208E9
		internal BurnShipObjective(Mission mission, MissionShip targetShip)
			: base(mission)
		{
			this._controller = base.Mission.GetMissionBehavior<BlockedEstuaryMissionController>();
			this._targetShip = targetShip;
			base.AddTarget(new ShipObjectiveTarget(this._targetShip, new TextObject("{=EBLRhSsY}Target Ship", null), false));
		}

		// Token: 0x060005A6 RID: 1446 RVA: 0x00022727 File Offset: 0x00020927
		protected override bool IsActivationRequirementsMet()
		{
			return this._targetShip != null;
		}

		// Token: 0x060005A7 RID: 1447 RVA: 0x00022732 File Offset: 0x00020932
		protected override bool IsCompletionRequirementsMet()
		{
			return this._controller.ShipsCollided;
		}

		// Token: 0x040002BD RID: 701
		private BlockedEstuaryMissionController _controller;

		// Token: 0x040002BE RID: 702
		private MissionShip _targetShip;
	}
}
