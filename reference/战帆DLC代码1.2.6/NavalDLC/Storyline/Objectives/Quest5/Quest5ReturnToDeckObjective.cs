using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Quest5
{
	// Token: 0x02000051 RID: 81
	public class Quest5ReturnToDeckObjective : MissionObjective
	{
		// Token: 0x17000101 RID: 257
		// (get) Token: 0x06000574 RID: 1396 RVA: 0x0002229C File Offset: 0x0002049C
		public override string UniqueId
		{
			get
			{
				return "quest_5_return_to_deck_objective";
			}
		}

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x06000575 RID: 1397 RVA: 0x000222A3 File Offset: 0x000204A3
		public override TextObject Name
		{
			get
			{
				return new TextObject("{=Cvwf3F6h}Return to Gunnar", null);
			}
		}

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x06000576 RID: 1398 RVA: 0x000222B0 File Offset: 0x000204B0
		public override TextObject Description
		{
			get
			{
				return new TextObject("{=ZRLg1dYM}Leave the hold to talk to Gunnar.", null);
			}
		}

		// Token: 0x06000577 RID: 1399 RVA: 0x000222BD File Offset: 0x000204BD
		public Quest5ReturnToDeckObjective(Mission mission, GameEntity targetDoorEntity, GameEntity deckSpawnPointEntity)
			: base(mission)
		{
			this._deckSpawnPointEntity = deckSpawnPointEntity;
			this._targetDoor = new Quest5ReturnToDeckObjective.ReturnToDeckObjectiveTarget(targetDoorEntity);
			base.AddTarget(this._targetDoor);
		}

		// Token: 0x06000578 RID: 1400 RVA: 0x000222E5 File Offset: 0x000204E5
		protected override bool IsActivationRequirementsMet()
		{
			return this._targetDoor != null;
		}

		// Token: 0x06000579 RID: 1401 RVA: 0x000222F0 File Offset: 0x000204F0
		protected override bool IsCompletionRequirementsMet()
		{
			return this._targetDoor != null && Agent.Main.Position.Distance(this._deckSpawnPointEntity.GlobalPosition) <= 3f;
		}

		// Token: 0x040002B2 RID: 690
		private GameEntity _deckSpawnPointEntity;

		// Token: 0x040002B3 RID: 691
		private Quest5ReturnToDeckObjective.ReturnToDeckObjectiveTarget _targetDoor;

		// Token: 0x020001CC RID: 460
		private class ReturnToDeckObjectiveTarget : MissionObjectiveTarget<GameEntity>
		{
			// Token: 0x06001A15 RID: 6677 RVA: 0x000AE76B File Offset: 0x000AC96B
			public ReturnToDeckObjectiveTarget(GameEntity target)
				: base(target)
			{
			}

			// Token: 0x06001A16 RID: 6678 RVA: 0x000AE774 File Offset: 0x000AC974
			public override TextObject GetName()
			{
				return new TextObject("{=5MH4xtlD}Gunnar", null);
			}

			// Token: 0x06001A17 RID: 6679 RVA: 0x000AE781 File Offset: 0x000AC981
			public override Vec3 GetGlobalPosition()
			{
				return base.Target.GetGlobalFrame().origin + Vec3.Up;
			}

			// Token: 0x06001A18 RID: 6680 RVA: 0x000AE79D File Offset: 0x000AC99D
			public override bool IsActive()
			{
				return true;
			}
		}
	}
}
