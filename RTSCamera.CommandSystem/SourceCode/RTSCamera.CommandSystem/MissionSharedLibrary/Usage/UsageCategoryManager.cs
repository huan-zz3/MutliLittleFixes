using System;
using System.Collections.Generic;
using MissionLibrary.Provider;
using MissionLibrary.Usage;
using MissionSharedLibrary.Category;
using MissionSharedLibrary.View.ViewModelCollection.Usage;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace MissionSharedLibrary.Usage
{
	// Token: 0x02000011 RID: 17
	public class UsageCategoryManager : AUsageCategoryManager
	{
		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600009D RID: 157 RVA: 0x000045EC File Offset: 0x000027EC
		public override Dictionary<string, IProvider<AUsageCategory>> Items
		{
			get
			{
				return this._repositoryImplementation.Items;
			}
		}

		// Token: 0x0600009E RID: 158 RVA: 0x000045F9 File Offset: 0x000027F9
		public override AUsageCategory GetItem(string categoryId)
		{
			return this._repositoryImplementation.GetItem(categoryId);
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00004607 File Offset: 0x00002807
		public override T GetItem<T>(string categoryId)
		{
			return this._repositoryImplementation.GetItem<T>(categoryId);
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00004615 File Offset: 0x00002815
		public override void RegisterItem(IProvider<AUsageCategory> category, bool addOnlyWhenMissing = true)
		{
			this._repositoryImplementation.RegisterItem(category, addOnlyWhenMissing);
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00004624 File Offset: 0x00002824
		public override ViewModel GetViewModel()
		{
			UsageCollectionViewModel usageCollectionViewModel;
			if ((usageCollectionViewModel = this._viewModel) == null)
			{
				usageCollectionViewModel = (this._viewModel = new UsageCollectionViewModel(GameTexts.FindText("str_mission_library_usages", null), this, null));
			}
			return usageCollectionViewModel;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00004656 File Offset: 0x00002856
		public override void OnUsageCategorySelected(AUsageCategory usageCategory)
		{
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00004658 File Offset: 0x00002858
		public override void Clear()
		{
			this._viewModel = null;
			foreach (KeyValuePair<string, IProvider<AUsageCategory>> keyValuePair in this.Items)
			{
				keyValuePair.Value.Clear();
			}
		}

		// Token: 0x04000038 RID: 56
		private UsageCollectionViewModel _viewModel;

		// Token: 0x04000039 RID: 57
		private RepositoryImplementation<AUsageCategory> _repositoryImplementation = new RepositoryImplementation<AUsageCategory>();
	}
}
