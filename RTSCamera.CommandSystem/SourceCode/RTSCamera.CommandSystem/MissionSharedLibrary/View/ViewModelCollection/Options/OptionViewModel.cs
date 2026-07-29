using System;
using MissionSharedLibrary.View.ViewModelCollection.Basic;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.ViewModelCollection.GameOptions;

namespace MissionSharedLibrary.View.ViewModelCollection.Options
{
	// Token: 0x02000026 RID: 38
	public abstract class OptionViewModel : ViewModel
	{
		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000155 RID: 341 RVA: 0x00005D9D File Offset: 0x00003F9D
		public TextViewModel Name { get; }

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000156 RID: 342 RVA: 0x00005DA5 File Offset: 0x00003FA5
		// (set) Token: 0x06000157 RID: 343 RVA: 0x00005DAD File Offset: 0x00003FAD
		[DataSourceProperty]
		public HintViewModel Description
		{
			get
			{
				return this._description;
			}
			set
			{
				if (this._description == value)
				{
					return;
				}
				this._description = value;
				base.OnPropertyChanged("Description");
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000158 RID: 344 RVA: 0x00005DCB File Offset: 0x00003FCB
		// (set) Token: 0x06000159 RID: 345 RVA: 0x00005DD3 File Offset: 0x00003FD3
		[DataSourceProperty]
		public string[] ImageIDs
		{
			get
			{
				return this._imageIDs;
			}
			set
			{
				if (value == this._imageIDs)
				{
					return;
				}
				this._imageIDs = value;
				base.OnPropertyChangedWithValue<string[]>(value, "ImageIDs");
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x0600015A RID: 346 RVA: 0x00005DF2 File Offset: 0x00003FF2
		// (set) Token: 0x0600015B RID: 347 RVA: 0x00005DFA File Offset: 0x00003FFA
		[DataSourceProperty]
		public int OptionTypeID
		{
			get
			{
				return this._optionTypeId;
			}
			set
			{
				if (value == this._optionTypeId)
				{
					return;
				}
				this._optionTypeId = value;
				base.OnPropertyChangedWithValue(value, "OptionTypeID");
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x0600015C RID: 348 RVA: 0x00005E19 File Offset: 0x00004019
		// (set) Token: 0x0600015D RID: 349 RVA: 0x00005E21 File Offset: 0x00004021
		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value == this._isEnabled)
				{
					return;
				}
				this._isEnabled = value;
				base.OnPropertyChangedWithValue(value, "IsEnabled");
			}
		}

		// Token: 0x0600015E RID: 350 RVA: 0x00005E40 File Offset: 0x00004040
		protected OptionViewModel(TextObject name, TextObject description, OptionsVM.OptionsDataType typeID, bool isEnabled = true)
		{
			this._descriptionText = description;
			this.Name = new TextViewModel(name, true);
			if (this._descriptionText != null)
			{
				this.Description = new HintViewModel(this._descriptionText, null);
			}
			this.OptionTypeID = typeID;
			this.IsEnabled = isEnabled;
			this.Refresh();
		}

		// Token: 0x0600015F RID: 351 RVA: 0x00005EA3 File Offset: 0x000040A3
		public virtual void UpdateData(bool initUpdate)
		{
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00005EA5 File Offset: 0x000040A5
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.Refresh();
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00005EB3 File Offset: 0x000040B3
		private void Refresh()
		{
			this.Name.RefreshValues();
			if (this._descriptionText != null)
			{
				this.Description = new HintViewModel(this._descriptionText, null);
			}
		}

		// Token: 0x04000083 RID: 131
		private readonly TextObject _descriptionText;

		// Token: 0x04000084 RID: 132
		private int _optionTypeId = -1;

		// Token: 0x04000085 RID: 133
		private string[] _imageIDs;

		// Token: 0x04000086 RID: 134
		private HintViewModel _description;

		// Token: 0x04000087 RID: 135
		private bool _isEnabled;
	}
}
