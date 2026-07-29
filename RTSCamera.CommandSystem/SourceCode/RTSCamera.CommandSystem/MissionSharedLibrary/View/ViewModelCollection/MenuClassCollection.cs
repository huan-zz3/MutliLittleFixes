using System;
using System.Collections.Generic;
using System.Linq;
using MissionLibrary.Provider;
using MissionLibrary.View;
using MissionSharedLibrary.Category;
using MissionSharedLibrary.Config;
using TaleWorlds.Library;

namespace MissionSharedLibrary.View.ViewModelCollection
{
	// Token: 0x0200001D RID: 29
	public class MenuClassCollection : AMenuClassCollection
	{
		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000106 RID: 262 RVA: 0x0000541B File Offset: 0x0000361B
		public override Dictionary<string, IProvider<AOptionClass>> Items
		{
			get
			{
				return this._repositoryImplementation.Items;
			}
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00005428 File Offset: 0x00003628
		public override void RegisterItem(IProvider<AOptionClass> category, bool addOnlyWhenMissing = true)
		{
			this._repositoryImplementation.RegisterItem(category, addOnlyWhenMissing);
		}

		// Token: 0x06000108 RID: 264 RVA: 0x00005437 File Offset: 0x00003637
		public override AOptionClass GetItem(string categoryId)
		{
			return this._repositoryImplementation.GetItem(categoryId);
		}

		// Token: 0x06000109 RID: 265 RVA: 0x00005445 File Offset: 0x00003645
		public override T GetItem<T>(string categoryId)
		{
			return this._repositoryImplementation.GetItem<T>(categoryId);
		}

		// Token: 0x0600010A RID: 266 RVA: 0x00005453 File Offset: 0x00003653
		public override void OnOptionClassSelected(AOptionClass optionClass)
		{
			this._config.PreviouslySelectedOptionClassId = optionClass.ItemId;
			MenuClassCollectionViewModel viewModel = this._viewModel;
			if (viewModel != null)
			{
				viewModel.OnOptionClassSelected(optionClass);
			}
			this._config.Serialize();
		}

		// Token: 0x0600010B RID: 267 RVA: 0x00005484 File Offset: 0x00003684
		public override void Clear()
		{
			this._viewModel = null;
			foreach (KeyValuePair<string, IProvider<AOptionClass>> keyValuePair in this.Items)
			{
				keyValuePair.Value.Clear();
			}
		}

		// Token: 0x0600010C RID: 268 RVA: 0x000054E4 File Offset: 0x000036E4
		public override ViewModel GetViewModel()
		{
			MenuClassCollectionViewModel menuClassCollectionViewModel;
			if ((menuClassCollectionViewModel = this._viewModel) == null)
			{
				menuClassCollectionViewModel = (this._viewModel = new MenuClassCollectionViewModel(this.Items.Select<KeyValuePair<string, IProvider<AOptionClass>>, IProvider<AOptionClass>>((KeyValuePair<string, IProvider<AOptionClass>> p) => p.Value).ToList<IProvider<AOptionClass>>(), this._config.PreviouslySelectedOptionClassId));
			}
			return menuClassCollectionViewModel;
		}

		// Token: 0x04000062 RID: 98
		private RepositoryImplementation<AOptionClass> _repositoryImplementation = new RepositoryImplementation<AOptionClass>();

		// Token: 0x04000063 RID: 99
		private MenuClassCollectionViewModel _viewModel;

		// Token: 0x04000064 RID: 100
		private GeneralConfig _config = MissionConfigBase<GeneralConfig>.Get();
	}
}
