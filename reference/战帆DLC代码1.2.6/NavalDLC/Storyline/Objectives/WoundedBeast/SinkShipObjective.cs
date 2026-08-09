using System;
using NavalDLC.Missions.Objects;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.WoundedBeast
{
	// Token: 0x02000045 RID: 69
	internal class SinkShipObjective : MissionObjective
	{
		// Token: 0x170000DD RID: 221
		// (get) Token: 0x06000528 RID: 1320 RVA: 0x00021AC2 File Offset: 0x0001FCC2
		public override string UniqueId
		{
			get
			{
				return "naval_storyline_quest_2_sink_ship_objective";
			}
		}

		// Token: 0x170000DE RID: 222
		// (get) Token: 0x06000529 RID: 1321 RVA: 0x00021AC9 File Offset: 0x0001FCC9
		public override TextObject Name
		{
			get
			{
				return new TextObject("{=VMVbnNau}Sink Fahda's Flagship", null);
			}
		}

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x0600052A RID: 1322 RVA: 0x00021AD6 File Offset: 0x0001FCD6
		public override TextObject Description
		{
			get
			{
				return new TextObject("{=rlEJ3pC8}Fahda's flagship was crippled by the storm. Ram it until it sinks!", null);
			}
		}

		// Token: 0x0600052B RID: 1323 RVA: 0x00021AE3 File Offset: 0x0001FCE3
		public SinkShipObjective(Mission mission, MissionShip targetShip)
			: base(mission)
		{
			this._targetShip = targetShip;
			this._sinkShipObjectiveTarget = new SinkShipObjective.SinkShipObjectiveTarget(this._targetShip, new TextObject("{=gCWSOyLJ}Fahda's Ship", null));
			base.AddTarget(this._sinkShipObjectiveTarget);
		}

		// Token: 0x0600052C RID: 1324 RVA: 0x00021B1B File Offset: 0x0001FD1B
		protected override bool IsActivationRequirementsMet()
		{
			return this._targetShip != null;
		}

		// Token: 0x0600052D RID: 1325 RVA: 0x00021B26 File Offset: 0x0001FD26
		protected override bool IsCompletionRequirementsMet()
		{
			return this._targetShip != null && (this._targetShip.HitPoints <= 0f || this._targetShip.IsSinking);
		}

		// Token: 0x0400029F RID: 671
		private readonly MissionShip _targetShip;

		// Token: 0x040002A0 RID: 672
		private SinkShipObjective.SinkShipObjectiveTarget _sinkShipObjectiveTarget;

		// Token: 0x020001C2 RID: 450
		private class SinkShipObjectiveTarget : MissionObjectiveTarget
		{
			// Token: 0x060019EE RID: 6638 RVA: 0x000AE4AD File Offset: 0x000AC6AD
			public SinkShipObjectiveTarget(MissionShip targetShip, TextObject name)
			{
				this.TargetShip = targetShip;
				this._name = name;
			}

			// Token: 0x060019EF RID: 6639 RVA: 0x000AE4C4 File Offset: 0x000AC6C4
			public override Vec3 GetGlobalPosition()
			{
				return this.TargetShip.GameEntity.GlobalPosition;
			}

			// Token: 0x060019F0 RID: 6640 RVA: 0x000AE4E4 File Offset: 0x000AC6E4
			public override TextObject GetName()
			{
				return this._name;
			}

			// Token: 0x060019F1 RID: 6641 RVA: 0x000AE4EC File Offset: 0x000AC6EC
			public override bool IsActive()
			{
				return this.TargetShip != null && !this.TargetShip.IsSinking;
			}

			// Token: 0x04000D36 RID: 3382
			public readonly MissionShip TargetShip;

			// Token: 0x04000D37 RID: 3383
			private readonly TextObject _name;
		}
	}
}
