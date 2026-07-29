using System;
using MissionLibrary.Provider;

namespace MissionSharedLibrary.Provider
{
	// Token: 0x02000049 RID: 73
	public class ConcreteVersionProvider<T> : IVersionProvider<T>, IVersionProvider where T : ATag<T>
	{
		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000268 RID: 616 RVA: 0x00008AE8 File Offset: 0x00006CE8
		public Version ProviderVersion { get; }

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x06000269 RID: 617 RVA: 0x00008AF0 File Offset: 0x00006CF0
		public T Value
		{
			get
			{
				T t;
				if ((t = this._value) == null)
				{
					t = (this._value = this.Create());
				}
				return t;
			}
		}

		// Token: 0x0600026A RID: 618 RVA: 0x00008B1B File Offset: 0x00006D1B
		public ConcreteVersionProvider(Func<ATag<T>> creator, Version providerVersion)
		{
			this.ProviderVersion = providerVersion;
			this._creator = creator;
		}

		// Token: 0x0600026B RID: 619 RVA: 0x00008B31 File Offset: 0x00006D31
		public void ForceCreate()
		{
			if (this._value == null)
			{
				this._value = this.Create();
			}
		}

		// Token: 0x0600026C RID: 620 RVA: 0x00008B4C File Offset: 0x00006D4C
		public void Clear()
		{
			this._value = default(T);
		}

		// Token: 0x0600026D RID: 621 RVA: 0x00008B5C File Offset: 0x00006D5C
		private T Create()
		{
			Func<ATag<T>> creator = this._creator;
			if (creator == null)
			{
				return default(T);
			}
			return creator().Self;
		}

		// Token: 0x040000F1 RID: 241
		private readonly Func<ATag<T>> _creator;

		// Token: 0x040000F2 RID: 242
		private T _value;
	}
}
