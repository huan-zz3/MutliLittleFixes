using System;
using System.ComponentModel;

namespace HarmonyLib
{
	// Token: 0x02000006 RID: 6
	// (Invoke) Token: 0x0600000B RID: 11
	[Obsolete("Use AccessTools.FieldRefAccess<T, S> for fields and AccessTools.MethodDelegate<Action<T, S>> for property setters")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public delegate void SetterHandler<in T, in S>(T source, S value);
}
