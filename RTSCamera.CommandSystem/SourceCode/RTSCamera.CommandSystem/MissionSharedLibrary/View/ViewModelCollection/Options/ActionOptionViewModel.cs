using System;
using MissionLibrary.View;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace MissionSharedLibrary.View.ViewModelCollection.Options
{
	// Token: 0x02000023 RID: 35
	public class ActionOptionViewModel : OptionViewModel, IOption, IViewModelProvider<ViewModel>
	{
		// Token: 0x06000139 RID: 313 RVA: 0x00005BAC File Offset: 0x00003DAC
		public ActionOptionViewModel(TextObject name, TextObject description, Action onAction)
			: base(name, description, 5, true)
		{
			this._onAction = onAction;
		}

		// Token: 0x0600013A RID: 314 RVA: 0x00005BBF File Offset: 0x00003DBF
		private void ExecuteAction()
		{
			Action onAction = this._onAction;
			if (onAction == null)
			{
				return;
			}
			Common.DynamicInvokeWithLog(onAction, Array.Empty<object>());
		}

		// Token: 0x0600013B RID: 315 RVA: 0x00005BD7 File Offset: 0x00003DD7
		public ViewModel GetViewModel()
		{
			return this;
		}

		// Token: 0x0600013C RID: 316 RVA: 0x00005BDA File Offset: 0x00003DDA
		public void Commit()
		{
		}

		// Token: 0x0600013D RID: 317 RVA: 0x00005BDC File Offset: 0x00003DDC
		public void Cancel()
		{
		}

		// Token: 0x0400007A RID: 122
		private readonly Action _onAction;
	}
}
