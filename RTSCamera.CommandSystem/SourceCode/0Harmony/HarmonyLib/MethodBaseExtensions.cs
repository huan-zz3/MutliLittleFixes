using System;
using System.Reflection;

namespace HarmonyLib
{
	// Token: 0x020001C4 RID: 452
	public static class MethodBaseExtensions
	{
		// Token: 0x060007E3 RID: 2019 RVA: 0x00019EEC File Offset: 0x000180EC
		public static bool HasMethodBody(this MethodBase member)
		{
			MethodBody methodBody = member.GetMethodBody();
			int? num;
			if (methodBody == null)
			{
				num = null;
			}
			else
			{
				byte[] ilasByteArray = methodBody.GetILAsByteArray();
				num = ((ilasByteArray != null) ? new int?(ilasByteArray.Length) : null);
			}
			int? num2 = num;
			return num2.GetValueOrDefault() > 0;
		}
	}
}
