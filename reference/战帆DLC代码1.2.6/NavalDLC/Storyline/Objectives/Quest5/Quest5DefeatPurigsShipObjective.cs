using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions.Objects;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Quest5
{
	// Token: 0x0200004C RID: 76
	public class Quest5DefeatPurigsShipObjective : MissionObjective
	{
		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x06000555 RID: 1365 RVA: 0x000220A8 File Offset: 0x000202A8
		public override string UniqueId
		{
			get
			{
				return "quest_5_defeat_purigs_ship_objective";
			}
		}

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x06000556 RID: 1366 RVA: 0x000220AF File Offset: 0x000202AF
		public override TextObject Name
		{
			get
			{
				return new TextObject("{=CedcuMUS}Defeat Purig's crew", null);
			}
		}

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x06000557 RID: 1367 RVA: 0x000220BC File Offset: 0x000202BC
		public override TextObject Description
		{
			get
			{
				return new TextObject("{=YDPv1Nsm}Board Purig's ship and defeat his crew.", null);
			}
		}

		// Token: 0x06000558 RID: 1368 RVA: 0x000220C9 File Offset: 0x000202C9
		public Quest5DefeatPurigsShipObjective(Mission mission, List<Agent> purigShipAgents, MissionShip purigsShip)
			: base(mission)
		{
			base.AddTarget(new Quest5DefeatPurigsShipObjective.DefeatPurigsShipTarget(purigsShip));
			this._purigShipAgents = purigShipAgents;
			this._cachedProgress = default(MissionObjectiveProgressInfo);
			this._cachedProgress.RequiredProgressAmount = this._purigShipAgents.Count;
		}

		// Token: 0x06000559 RID: 1369 RVA: 0x00022107 File Offset: 0x00020307
		public override MissionObjectiveProgressInfo GetCurrentProgress()
		{
			this._cachedProgress.CurrentProgressAmount = this._cachedProgress.RequiredProgressAmount - this._purigShipAgents.Count<Agent>();
			return this._cachedProgress;
		}

		// Token: 0x0600055A RID: 1370 RVA: 0x00022131 File Offset: 0x00020331
		protected override bool IsActivationRequirementsMet()
		{
			return true;
		}

		// Token: 0x0600055B RID: 1371 RVA: 0x00022134 File Offset: 0x00020334
		protected override bool IsCompletionRequirementsMet()
		{
			if (!Extensions.IsEmpty<Agent>(this._purigShipAgents))
			{
				return !this._purigShipAgents.Any<Agent>((Agent a) => a.IsActive());
			}
			return true;
		}

		// Token: 0x040002AD RID: 685
		private readonly List<Agent> _purigShipAgents;

		// Token: 0x040002AE RID: 686
		private MissionObjectiveProgressInfo _cachedProgress;

		// Token: 0x020001C9 RID: 457
		private class DefeatPurigsShipTarget : MissionObjectiveTarget
		{
			// Token: 0x06001A0A RID: 6666 RVA: 0x000AE6A7 File Offset: 0x000AC8A7
			public DefeatPurigsShipTarget(MissionShip target)
			{
				this._target = target;
			}

			// Token: 0x06001A0B RID: 6667 RVA: 0x000AE6B6 File Offset: 0x000AC8B6
			public override TextObject GetName()
			{
				return new TextObject("{=ny9Rllh3}Purig's Ship", null);
			}

			// Token: 0x06001A0C RID: 6668 RVA: 0x000AE6C3 File Offset: 0x000AC8C3
			public override Vec3 GetGlobalPosition()
			{
				return this._target.GlobalFrame.origin + Vec3.Up;
			}

			// Token: 0x06001A0D RID: 6669 RVA: 0x000AE6DF File Offset: 0x000AC8DF
			public override bool IsActive()
			{
				Agent main = Agent.Main;
				return main != null && main.IsActive() && !this._target.GetIsAgentOnShip(Agent.Main, false);
			}

			// Token: 0x04000D40 RID: 3392
			private readonly MissionShip _target;
		}
	}
}
