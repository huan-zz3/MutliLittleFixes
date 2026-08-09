using System;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Quest5
{
	// Token: 0x0200004D RID: 77
	public class Quest5EscapeObjective : MissionObjective
	{
		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x0600055C RID: 1372 RVA: 0x00022172 File Offset: 0x00020372
		public override string UniqueId
		{
			get
			{
				return "quest_5_escape_objective";
			}
		}

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x0600055D RID: 1373 RVA: 0x00022179 File Offset: 0x00020379
		public override TextObject Name
		{
			get
			{
				return new TextObject("{=BkIpqqTD}Hold Off Sea Hounds", null);
			}
		}

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x0600055E RID: 1374 RVA: 0x00022186 File Offset: 0x00020386
		public override TextObject Description
		{
			get
			{
				return new TextObject("{=ZTiHqbi9}Fight off any Sea Hound pursuers until Gunnar can steer the ship to safety.", null);
			}
		}

		// Token: 0x0600055F RID: 1375 RVA: 0x00022193 File Offset: 0x00020393
		public Quest5EscapeObjective(Mission mission, TextObject description)
			: base(mission)
		{
			this._description = description;
		}

		// Token: 0x06000560 RID: 1376 RVA: 0x000221A3 File Offset: 0x000203A3
		protected override bool IsActivationRequirementsMet()
		{
			return true;
		}

		// Token: 0x06000561 RID: 1377 RVA: 0x000221A6 File Offset: 0x000203A6
		protected override bool IsCompletionRequirementsMet()
		{
			return false;
		}

		// Token: 0x040002AF RID: 687
		private readonly TextObject _description;
	}
}
