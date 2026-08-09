using System;
using TaleWorlds.SaveSystem;

namespace NavalDLC
{
	// Token: 0x0200001A RID: 26
	public static class MetaDataExtensions
	{
		// Token: 0x06000118 RID: 280 RVA: 0x0000827C File Offset: 0x0000647C
		public static bool HasNavalDLC(this MetaData metaData)
		{
			bool flag = false;
			string text;
			if (metaData != null && metaData.TryGetValue("Modules", ref text))
			{
				string[] array = text.Split(new char[] { ';' });
				for (int i = 0; i < array.Length; i++)
				{
					if (string.Equals(array[i], "NavalDLC", StringComparison.OrdinalIgnoreCase))
					{
						flag = true;
						break;
					}
				}
			}
			return flag;
		}
	}
}
