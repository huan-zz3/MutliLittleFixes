using System;
using System.Collections.Generic;
using MissionLibrary.HotKey;
using MissionLibrary.Provider;
using MissionSharedLibrary.Category;
using MissionSharedLibrary.Utilities;

namespace MissionSharedLibrary.HotKey
{
	// Token: 0x0200000D RID: 13
	public class GameKeyCategoryManager : AGameKeyCategoryManager
	{
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600008E RID: 142 RVA: 0x00004467 File Offset: 0x00002667
		public override Dictionary<string, IProvider<AGameKeyCategory>> Items
		{
			get
			{
				return this._repositoryImplementation.Items;
			}
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00004474 File Offset: 0x00002674
		public override AGameKeyCategory GetItem(string categoryId)
		{
			return this._repositoryImplementation.GetItem(categoryId);
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00004482 File Offset: 0x00002682
		public override T GetItem<T>(string categoryId)
		{
			return this._repositoryImplementation.GetItem<T>(categoryId);
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00004490 File Offset: 0x00002690
		public override void RegisterItem(IProvider<AGameKeyCategory> provider, bool addOnlyWhenMissing = true)
		{
			try
			{
				this._repositoryImplementation.RegisterItem(provider, addOnlyWhenMissing);
			}
			catch (Exception ex)
			{
				Utility.DisplayMessage(ex.ToString());
				Console.WriteLine(ex);
			}
		}

		// Token: 0x06000092 RID: 146 RVA: 0x000044D0 File Offset: 0x000026D0
		public override void Save()
		{
			foreach (KeyValuePair<string, IProvider<AGameKeyCategory>> keyValuePair in this.Items)
			{
				keyValuePair.Value.Value.Save();
			}
		}

		// Token: 0x04000033 RID: 51
		private readonly RepositoryImplementation<AGameKeyCategory> _repositoryImplementation = new RepositoryImplementation<AGameKeyCategory>();
	}
}
