using System;
using TaleWorlds.Core;

namespace NavalDLC.CustomBattle
{
	// Token: 0x02000006 RID: 6
	public class NavalCustomBattleState : GameState
	{
		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000038 RID: 56 RVA: 0x00002FF1 File Offset: 0x000011F1
		public override bool IsMusicMenuState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00002FFC File Offset: 0x000011FC
		protected override void OnInitialize()
		{
			base.OnInitialize();
		}
	}
}
