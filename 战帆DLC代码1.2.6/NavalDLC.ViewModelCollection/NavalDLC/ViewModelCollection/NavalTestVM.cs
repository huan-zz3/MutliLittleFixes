using System;
using TaleWorlds.Library;

namespace NavalDLC.ViewModelCollection
{
	// Token: 0x02000008 RID: 8
	public class NavalTestVM : ViewModel
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000024 RID: 36 RVA: 0x00005252 File Offset: 0x00003452
		[DataSourceProperty]
		public string NavalText
		{
			get
			{
				return "Text from NavalTestVM";
			}
		}
	}
}
