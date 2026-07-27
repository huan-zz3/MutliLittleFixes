using System;
using TaleWorlds.Localization;

namespace NavalDLC.ViewModelCollection.Port.PortScreenHandlers
{
	// Token: 0x02000017 RID: 23
	public readonly struct PortActionInfo
	{
		// Token: 0x060001C8 RID: 456 RVA: 0x0000ACA2 File Offset: 0x00008EA2
		private PortActionInfo(bool isRelevant, bool isEnabled, int goldCost, TextObject actionName, TextObject tooltip = null)
		{
			this.IsRelevant = isRelevant;
			this.IsEnabled = isEnabled;
			this.GoldCost = goldCost;
			this.ActionName = actionName;
			this.Tooltip = tooltip ?? TextObject.GetEmpty();
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x0000ACD2 File Offset: 0x00008ED2
		public static PortActionInfo CreateValid(bool isEnabled, int goldCost, TextObject name, TextObject tooltip)
		{
			return new PortActionInfo(true, isEnabled, goldCost, name, tooltip);
		}

		// Token: 0x060001CA RID: 458 RVA: 0x0000ACDE File Offset: 0x00008EDE
		public static PortActionInfo CreateInvalid(TextObject reason = null)
		{
			return new PortActionInfo(false, false, 0, TextObject.GetEmpty(), reason);
		}

		// Token: 0x040000AF RID: 175
		public readonly bool IsRelevant;

		// Token: 0x040000B0 RID: 176
		public readonly bool IsEnabled;

		// Token: 0x040000B1 RID: 177
		public readonly int GoldCost;

		// Token: 0x040000B2 RID: 178
		public readonly TextObject ActionName;

		// Token: 0x040000B3 RID: 179
		public readonly TextObject Tooltip;
	}
}
