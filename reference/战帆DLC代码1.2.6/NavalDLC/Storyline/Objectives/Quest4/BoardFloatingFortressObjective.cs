using System;
using NavalDLC.Missions.Objects;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Quest4
{
	// Token: 0x02000054 RID: 84
	public class BoardFloatingFortressObjective : MissionObjective
	{
		// Token: 0x1700010A RID: 266
		// (get) Token: 0x06000586 RID: 1414 RVA: 0x000223EF File Offset: 0x000205EF
		public override string UniqueId
		{
			get
			{
				return "naval_storyline_quest_4_board_floating_fortress_objective";
			}
		}

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x06000587 RID: 1415 RVA: 0x000223F6 File Offset: 0x000205F6
		public override TextObject Name
		{
			get
			{
				return new TextObject("{=UcZmBaYV}Storm the Floating Fortress", null);
			}
		}

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x06000588 RID: 1416 RVA: 0x00022403 File Offset: 0x00020603
		public override TextObject Description
		{
			get
			{
				return new TextObject("{=wCiAvXU6}Lead your fleet in to board Crusas’ lashed-together ships", null);
			}
		}

		// Token: 0x06000589 RID: 1417 RVA: 0x00022410 File Offset: 0x00020610
		public BoardFloatingFortressObjective(Mission mission, MissionShip playerShip, MBList<MissionShip> enemyShips)
			: base(mission)
		{
			this._playerShip = playerShip;
			this._enemyShips = enemyShips;
		}

		// Token: 0x0600058A RID: 1418 RVA: 0x00022427 File Offset: 0x00020627
		protected override bool IsActivationRequirementsMet()
		{
			return true;
		}

		// Token: 0x0600058B RID: 1419 RVA: 0x0002242C File Offset: 0x0002062C
		protected override bool IsCompletionRequirementsMet()
		{
			foreach (MissionShip missionShip in this._playerShip.GetConnectedShips())
			{
				if (this._enemyShips.Contains(missionShip))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x040002B7 RID: 695
		private readonly MissionShip _playerShip;

		// Token: 0x040002B8 RID: 696
		private readonly MBList<MissionShip> _enemyShips;
	}
}
