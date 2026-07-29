using System;

namespace MissionLibrary.Provider
{
	// Token: 0x02000011 RID: 17
	public abstract class ATag<T> where T : ATag<T>
	{
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000049 RID: 73 RVA: 0x00002419 File Offset: 0x00000619
		public virtual T Self
		{
			get
			{
				return (T)((object)this);
			}
		}
	}
}
