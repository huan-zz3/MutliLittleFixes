using System;
using System.Collections.Generic;
using System.Linq;

namespace MissionLibrary.Provider
{
	// Token: 0x02000019 RID: 25
	public class ProviderManager : IProviderManager
	{
		// Token: 0x06000059 RID: 89 RVA: 0x0000242C File Offset: 0x0000062C
		public void RegisterInstance<T>(IVersionProvider<T> newProvider, string key = "") where T : ATag<T>
		{
			Dictionary<string, IVersionProvider> dictionary;
			if (!this._providersWithKey.TryGetValue(typeof(T), out dictionary))
			{
				Dictionary<Type, Dictionary<string, IVersionProvider>> providersWithKey = this._providersWithKey;
				Type typeFromHandle = typeof(T);
				Dictionary<string, IVersionProvider> dictionary2 = new Dictionary<string, IVersionProvider>();
				dictionary2[key] = newProvider;
				providersWithKey.Add(typeFromHandle, dictionary2);
				return;
			}
			IVersionProvider versionProvider;
			if (!dictionary.TryGetValue(key, out versionProvider))
			{
				dictionary.Add(key, newProvider);
				return;
			}
			if (versionProvider.ProviderVersion.CompareTo(newProvider.ProviderVersion) <= 0)
			{
				dictionary[key] = newProvider;
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000024A8 File Offset: 0x000006A8
		public T GetInstance<T>(string key = "") where T : ATag<T>
		{
			Dictionary<string, IVersionProvider> dictionary;
			IVersionProvider versionProvider;
			if (this._providersWithKey.TryGetValue(typeof(T), out dictionary) && dictionary.TryGetValue(key, out versionProvider))
			{
				IVersionProvider<T> versionProvider2 = versionProvider as IVersionProvider<T>;
				if (versionProvider2 != null)
				{
					return versionProvider2.Value;
				}
			}
			return default(T);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x000024F4 File Offset: 0x000006F4
		public IEnumerable<T> GetInstances<T>() where T : ATag<T>
		{
			Dictionary<string, IVersionProvider> dictionary;
			if (!this._providersWithKey.TryGetValue(typeof(T), out dictionary))
			{
				return Enumerable.Empty<T>();
			}
			return dictionary.Values.Where<IVersionProvider>((IVersionProvider v) => v is IVersionProvider<T>).Select<IVersionProvider, T>(delegate(IVersionProvider v)
			{
				IVersionProvider<T> versionProvider = v as IVersionProvider<T>;
				if (versionProvider == null)
				{
					return default(T);
				}
				return versionProvider.Value;
			});
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00002570 File Offset: 0x00000770
		public void InstantiateAll()
		{
			foreach (KeyValuePair<Type, Dictionary<string, IVersionProvider>> keyValuePair in this._providersWithKey)
			{
				foreach (KeyValuePair<string, IVersionProvider> keyValuePair2 in keyValuePair.Value)
				{
					keyValuePair2.Value.ForceCreate();
				}
			}
		}

		// Token: 0x0400000D RID: 13
		private readonly Dictionary<Type, Dictionary<string, IVersionProvider>> _providersWithKey = new Dictionary<Type, Dictionary<string, IVersionProvider>>();
	}
}
