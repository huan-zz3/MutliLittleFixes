using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets;

namespace MissionSharedLibrary.View.Widgets
{
	// Token: 0x0200001A RID: 26
	public class MissionLibraryToggleWidget : ToggleButtonWidget
	{
		// Token: 0x060000DC RID: 220 RVA: 0x00004D9B File Offset: 0x00002F9B
		public MissionLibraryToggleWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000DD RID: 221 RVA: 0x00004DAB File Offset: 0x00002FAB
		// (set) Token: 0x060000DE RID: 222 RVA: 0x00004DB3 File Offset: 0x00002FB3
		public bool AutoToggleTransferButtonState { get; set; } = true;

		// Token: 0x060000DF RID: 223 RVA: 0x00004DBC File Offset: 0x00002FBC
		protected override void OnClick(Widget widget)
		{
			if (this._listPanel != null && this._listPanel.ChildCount <= 0)
			{
				return;
			}
			base.OnClick(widget);
			this.UpdateCollapseIndicator();
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00004DE2 File Offset: 0x00002FE2
		private void OnListSizeChange(Widget widget)
		{
			this.UpdateSize();
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00004DEA File Offset: 0x00002FEA
		private void OnListSizeChange(Widget parentWidget, Widget addedWidget)
		{
			this.UpdateSize();
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00004DF2 File Offset: 0x00002FF2
		protected override void OnConnectedToRoot()
		{
			base.OnConnectedToRoot();
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00004DFA File Offset: 0x00002FFA
		public override void SetState(string stateName)
		{
			if (this._listPanel != null && this._listPanel.ChildCount <= 0)
			{
				return;
			}
			base.SetState(stateName);
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x00004E1C File Offset: 0x0000301C
		private void UpdateSize()
		{
			if (this.TransferButtonWidget != null && this.AutoToggleTransferButtonState)
			{
				this.TransferButtonWidget.IsEnabled = this._listPanel.ChildCount > 0;
			}
			base.IsVisible = true;
			this._latestChildCount = this._listPanel.ChildCount;
			this.UpdateCollapseIndicator();
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00004E70 File Offset: 0x00003070
		private void ListPanelUpdated()
		{
			if (this.TransferButtonWidget != null)
			{
				this.TransferButtonWidget.IsEnabled = false;
			}
			this._listPanel.ItemAfterRemoveEventHandlers.Add(new Action<Widget>(this.OnListSizeChange));
			this._listPanel.ItemAddEventHandlers.Add(new Action<Widget, Widget>(this.OnListSizeChange));
			this.UpdateSize();
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00004ECF File Offset: 0x000030CF
		private void TransferButtonUpdated()
		{
			this.TransferButtonWidget.IsEnabled = false;
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x00004EDD File Offset: 0x000030DD
		private void CollapseIndicatorUpdated()
		{
			this.CollapseIndicator.AddState("Collapsed");
			this.CollapseIndicator.AddState("Expanded");
			this.UpdateCollapseIndicator();
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00004F08 File Offset: 0x00003108
		private void UpdateCollapseIndicator()
		{
			if (base.WidgetToClose == null || this.CollapseIndicator == null)
			{
				return;
			}
			if (base.WidgetToClose.IsVisible)
			{
				this.CollapseIndicator.SetState("Expanded");
				return;
			}
			this.CollapseIndicator.SetState("Collapsed");
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000E9 RID: 233 RVA: 0x00004F54 File Offset: 0x00003154
		// (set) Token: 0x060000EA RID: 234 RVA: 0x00004F5C File Offset: 0x0000315C
		[Editor(false)]
		public ListPanel ListPanel
		{
			get
			{
				return this._listPanel;
			}
			set
			{
				if (this._listPanel == value)
				{
					return;
				}
				this._listPanel = value;
				base.OnPropertyChanged<ListPanel>(value, "ListPanel");
				this.ListPanelUpdated();
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000EB RID: 235 RVA: 0x00004F81 File Offset: 0x00003181
		// (set) Token: 0x060000EC RID: 236 RVA: 0x00004F89 File Offset: 0x00003189
		[Editor(false)]
		public ButtonWidget TransferButtonWidget
		{
			get
			{
				return this._transferButtonWidget;
			}
			set
			{
				if (this._transferButtonWidget == value)
				{
					return;
				}
				this._transferButtonWidget = value;
				base.OnPropertyChanged<ButtonWidget>(value, "TransferButtonWidget");
				this.TransferButtonUpdated();
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000ED RID: 237 RVA: 0x00004FAE File Offset: 0x000031AE
		// (set) Token: 0x060000EE RID: 238 RVA: 0x00004FB6 File Offset: 0x000031B6
		[Editor(false)]
		public BrushWidget CollapseIndicator
		{
			get
			{
				return this._collapseIndicator;
			}
			set
			{
				if (this._collapseIndicator == value)
				{
					return;
				}
				this._collapseIndicator = value;
				base.OnPropertyChanged<BrushWidget>(value, "CollapseIndicator");
				this.CollapseIndicatorUpdated();
			}
		}

		// Token: 0x0400004F RID: 79
		private int _latestChildCount;

		// Token: 0x04000050 RID: 80
		private ListPanel _listPanel;

		// Token: 0x04000051 RID: 81
		private ButtonWidget _transferButtonWidget;

		// Token: 0x04000052 RID: 82
		private BrushWidget _collapseIndicator;
	}
}
