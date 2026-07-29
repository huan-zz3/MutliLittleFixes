using System;
using MissionLibrary.Provider;

namespace MissionSharedLibrary.Provider
{
	// Token: 0x02000045 RID: 69
	public class ConcreteProvider<T> : IProvider<T>, IProvider where T : ATag<T>
	{
		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000257 RID: 599 RVA: 0x00008972 File Offset: 0x00006B72
		public string Id { get; }

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x06000258 RID: 600 RVA: 0x0000897A File Offset: 0x00006B7A
		public Version ProviderVersion { get; }

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000259 RID: 601 RVA: 0x00008984 File Offset: 0x00006B84
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

		// Token: 0x0600025A RID: 602 RVA: 0x000089AF File Offset: 0x00006BAF
		public ConcreteProvider(Func<ATag<T>> creator, string id, Version providerVersion)
		{
			this.Id = id;
			this.ProviderVersion = providerVersion;
			this._creator = creator;
		}

		// Token: 0x0600025B RID: 603 RVA: 0x000089CC File Offset: 0x00006BCC
		public void ForceCreate()
		{
			if (this._value == null)
			{
				this._value = this.Create();
			}
		}

		// Token: 0x0600025C RID: 604 RVA: 0x000089E7 File Offset: 0x00006BE7
		public void Clear()
		{
			this._value = default(T);
		}

		// Token: 0x0600025D RID: 605 RVA: 0x000089F8 File Offset: 0x00006BF8
		private T Create()
		{
			Func<ATag<T>> creator = this._creator;
			if (creator == null)
			{
				return default(T);
			}
			return creator().Self;
		}

		// Token: 0x040000EA RID: 234
		private readonly Func<ATag<T>> _creator;

		// Token: 0x040000EC RID: 236
		private T _value;
	}
}
