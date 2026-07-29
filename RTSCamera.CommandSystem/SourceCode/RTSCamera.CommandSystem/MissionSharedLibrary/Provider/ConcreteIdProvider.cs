using System;
using MissionLibrary.Provider;

namespace MissionSharedLibrary.Provider
{
	// Token: 0x02000047 RID: 71
	public class ConcreteIdProvider<T> : IIdProvider<T>, IIdProvider where T : ATag<T>
	{
		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000260 RID: 608 RVA: 0x00008A35 File Offset: 0x00006C35
		public string Id { get; }

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x06000261 RID: 609 RVA: 0x00008A40 File Offset: 0x00006C40
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

		// Token: 0x06000262 RID: 610 RVA: 0x00008A6B File Offset: 0x00006C6B
		public ConcreteIdProvider(Func<ATag<T>> creator, string id)
		{
			this.Id = id;
			this._creator = creator;
		}

		// Token: 0x06000263 RID: 611 RVA: 0x00008A81 File Offset: 0x00006C81
		public void ForceCreate()
		{
			if (this._value == null)
			{
				this._value = this.Create();
			}
		}

		// Token: 0x06000264 RID: 612 RVA: 0x00008A9C File Offset: 0x00006C9C
		public void Clear()
		{
			this._value = default(T);
		}

		// Token: 0x06000265 RID: 613 RVA: 0x00008AAC File Offset: 0x00006CAC
		private T Create()
		{
			Func<ATag<T>> creator = this._creator;
			if (creator == null)
			{
				return default(T);
			}
			return creator().Self;
		}

		// Token: 0x040000EE RID: 238
		private readonly Func<ATag<T>> _creator;

		// Token: 0x040000EF RID: 239
		private T _value;
	}
}
