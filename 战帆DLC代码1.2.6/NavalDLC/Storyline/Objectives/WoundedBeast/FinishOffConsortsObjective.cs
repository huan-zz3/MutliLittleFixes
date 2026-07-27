using System;
using System.Collections.Generic;
using NavalDLC.Missions.Objects;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.WoundedBeast
{
	// Token: 0x02000044 RID: 68
	internal class FinishOffConsortsObjective : MissionObjective
	{
		// Token: 0x170000DA RID: 218
		// (get) Token: 0x06000520 RID: 1312 RVA: 0x0002199E File Offset: 0x0001FB9E
		public override string UniqueId
		{
			get
			{
				return "naval_storyline_quest_2_sink_ship_objective";
			}
		}

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x06000521 RID: 1313 RVA: 0x000219A5 File Offset: 0x0001FBA5
		public override TextObject Name
		{
			get
			{
				return new TextObject("{=seOnzgCs}Defeat Fahda's consorts", null);
			}
		}

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06000522 RID: 1314 RVA: 0x000219B2 File Offset: 0x0001FBB2
		public override TextObject Description
		{
			get
			{
				return new TextObject("{=3lZywscl}Fahda's flagship is going down. Defeat the rest of her fleet.", null);
			}
		}

		// Token: 0x06000523 RID: 1315 RVA: 0x000219C0 File Offset: 0x0001FBC0
		public FinishOffConsortsObjective(Mission mission, List<MissionShip> targetShips)
			: base(mission)
		{
			this._targets = targetShips;
			foreach (MissionShip missionShip in this._targets)
			{
				base.AddTarget(new FinishOffConsortsObjective.ShipObjectiveTarget(missionShip, new TextObject("{=UaWgrVnN}Fahda's Consort", null)));
			}
			this._cachedProgress.RequiredProgressAmount = targetShips.Count;
		}

		// Token: 0x06000524 RID: 1316 RVA: 0x00021A44 File Offset: 0x0001FC44
		public override MissionObjectiveProgressInfo GetCurrentProgress()
		{
			return this._cachedProgress;
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x00021A4C File Offset: 0x0001FC4C
		protected override void OnTick(float dt)
		{
			base.OnTick(dt);
			int num = 0;
			using (List<MissionShip>.Enumerator enumerator = this._targets.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Formation.CountOfUnits == 0)
					{
						num++;
					}
				}
			}
			this._cachedProgress.CurrentProgressAmount = num;
		}

		// Token: 0x06000526 RID: 1318 RVA: 0x00021ABC File Offset: 0x0001FCBC
		protected override bool IsActivationRequirementsMet()
		{
			return true;
		}

		// Token: 0x06000527 RID: 1319 RVA: 0x00021ABF File Offset: 0x0001FCBF
		protected override bool IsCompletionRequirementsMet()
		{
			return false;
		}

		// Token: 0x0400029D RID: 669
		private MissionObjectiveProgressInfo _cachedProgress;

		// Token: 0x0400029E RID: 670
		private List<MissionShip> _targets;

		// Token: 0x020001C1 RID: 449
		private class ShipObjectiveTarget : MissionObjectiveTarget
		{
			// Token: 0x060019EA RID: 6634 RVA: 0x000AE436 File Offset: 0x000AC636
			public ShipObjectiveTarget(MissionShip targetShip, TextObject name)
			{
				this.TargetShip = targetShip;
				this._name = name;
			}

			// Token: 0x060019EB RID: 6635 RVA: 0x000AE44C File Offset: 0x000AC64C
			public override Vec3 GetGlobalPosition()
			{
				return this.TargetShip.GameEntity.GlobalPosition;
			}

			// Token: 0x060019EC RID: 6636 RVA: 0x000AE46C File Offset: 0x000AC66C
			public override TextObject GetName()
			{
				return this._name;
			}

			// Token: 0x060019ED RID: 6637 RVA: 0x000AE474 File Offset: 0x000AC674
			public override bool IsActive()
			{
				return this.TargetShip != null && !this.TargetShip.IsDisabled && !this.TargetShip.IsSinking && this.TargetShip.Formation.CountOfUnits > 0;
			}

			// Token: 0x04000D34 RID: 3380
			public readonly MissionShip TargetShip;

			// Token: 0x04000D35 RID: 3381
			private readonly TextObject _name;
		}
	}
}
