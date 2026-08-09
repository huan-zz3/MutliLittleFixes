using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Quest5
{
	// Token: 0x02000047 RID: 71
	public class Quest5CheckInteriorObjective : MissionObjective
	{
		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x06000534 RID: 1332 RVA: 0x00021C02 File Offset: 0x0001FE02
		public override string UniqueId
		{
			get
			{
				return "quest_5_check_interior_objective";
			}
		}

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x06000535 RID: 1333 RVA: 0x00021C09 File Offset: 0x0001FE09
		public override TextObject Name
		{
			get
			{
				return new TextObject("{=eVJ4HNv1}Enter the hold", null);
			}
		}

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x06000536 RID: 1334 RVA: 0x00021C16 File Offset: 0x0001FE16
		public override TextObject Description
		{
			get
			{
				return new TextObject("{=aKzRozvo}Enter the hold of the ship.", null);
			}
		}

		// Token: 0x06000537 RID: 1335 RVA: 0x00021C23 File Offset: 0x0001FE23
		public Quest5CheckInteriorObjective(Mission mission, GameEntity targetDoor, GameEntity interiorSpawnPointEntity)
			: base(mission)
		{
			this._interiorSpawnPointEntity = interiorSpawnPointEntity;
			this._targetDoor = new Quest5CheckInteriorObjective.CheckInteriorObjectiveTarget(targetDoor);
			base.AddTarget(this._targetDoor);
		}

		// Token: 0x06000538 RID: 1336 RVA: 0x00021C4B File Offset: 0x0001FE4B
		protected override bool IsActivationRequirementsMet()
		{
			return this._targetDoor != null;
		}

		// Token: 0x06000539 RID: 1337 RVA: 0x00021C58 File Offset: 0x0001FE58
		protected override bool IsCompletionRequirementsMet()
		{
			return this._targetDoor != null && Agent.Main.Position.Distance(this._interiorSpawnPointEntity.GlobalPosition) <= 3f;
		}

		// Token: 0x040002A4 RID: 676
		private readonly GameEntity _interiorSpawnPointEntity;

		// Token: 0x040002A5 RID: 677
		private Quest5CheckInteriorObjective.CheckInteriorObjectiveTarget _targetDoor;

		// Token: 0x020001C4 RID: 452
		private class CheckInteriorObjectiveTarget : MissionObjectiveTarget<GameEntity>
		{
			// Token: 0x060019F6 RID: 6646 RVA: 0x000AE532 File Offset: 0x000AC732
			public CheckInteriorObjectiveTarget(GameEntity target)
				: base(target)
			{
			}

			// Token: 0x060019F7 RID: 6647 RVA: 0x000AE53B File Offset: 0x000AC73B
			public override TextObject GetName()
			{
				return new TextObject("{=shipHold}Hold", null);
			}

			// Token: 0x060019F8 RID: 6648 RVA: 0x000AE548 File Offset: 0x000AC748
			public override Vec3 GetGlobalPosition()
			{
				return base.Target.GetGlobalFrame().origin + Vec3.Up;
			}

			// Token: 0x060019F9 RID: 6649 RVA: 0x000AE564 File Offset: 0x000AC764
			public override bool IsActive()
			{
				return true;
			}
		}
	}
}
