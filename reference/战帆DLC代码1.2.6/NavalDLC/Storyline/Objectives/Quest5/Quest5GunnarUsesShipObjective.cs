using System;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Missions.Objectives;

namespace NavalDLC.Storyline.Objectives.Quest5
{
	// Token: 0x0200004E RID: 78
	public class Quest5GunnarUsesShipObjective : MissionObjective
	{
		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x06000562 RID: 1378 RVA: 0x000221A9 File Offset: 0x000203A9
		public override string UniqueId
		{
			get
			{
				return "quest_5_gunnar_uses_ship_objective";
			}
		}

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x06000563 RID: 1379 RVA: 0x000221B0 File Offset: 0x000203B0
		public override TextObject Name
		{
			get
			{
				return new TextObject("{=LBNwZ3HS}Keep Watch", null);
			}
		}

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x06000564 RID: 1380 RVA: 0x000221BD File Offset: 0x000203BD
		public override TextObject Description
		{
			get
			{
				return new TextObject("{=araGPQbp}Keep watch for approaching enemy ships.", null);
			}
		}

		// Token: 0x06000565 RID: 1381 RVA: 0x000221CA File Offset: 0x000203CA
		public Quest5GunnarUsesShipObjective(Mission mission)
			: base(mission)
		{
		}

		// Token: 0x06000566 RID: 1382 RVA: 0x000221D3 File Offset: 0x000203D3
		protected override bool IsActivationRequirementsMet()
		{
			return true;
		}

		// Token: 0x06000567 RID: 1383 RVA: 0x000221D6 File Offset: 0x000203D6
		protected override bool IsCompletionRequirementsMet()
		{
			return false;
		}
	}
}
