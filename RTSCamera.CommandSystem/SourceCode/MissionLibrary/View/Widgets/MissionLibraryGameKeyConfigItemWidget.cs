using System;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace MissionLibrary.View.Widgets
{
	// Token: 0x0200000B RID: 11
	public class MissionLibraryGameKeyConfigItemWidget : ListPanel
	{
		// Token: 0x06000026 RID: 38 RVA: 0x0000219F File Offset: 0x0000039F
		public MissionLibraryGameKeyConfigItemWidget(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000027 RID: 39 RVA: 0x000021A8 File Offset: 0x000003A8
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (this._screenWidget == null)
			{
				this._screenWidget = base.EventManager.Root.GetChild(0).FindChild("Options") as MissionLibraryGameKeyConfigWidget;
			}
			if (this._eventsRegistered)
			{
				return;
			}
			this.RegisterHoverEvents();
			this._eventsRegistered = true;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002200 File Offset: 0x00000400
		protected override void OnHoverBegin()
		{
			base.OnHoverBegin();
			this.SetCurrentOption(false, false, -1);
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00002211 File Offset: 0x00000411
		protected override void OnHoverEnd()
		{
			base.OnHoverEnd();
			this.ResetCurrentOption();
		}

		// Token: 0x0600002A RID: 42 RVA: 0x0000221F File Offset: 0x0000041F
		private void SetCurrentOption(bool fromHoverOverDropdown, bool fromBooleanSelection, int hoverDropdownItemIndex = -1)
		{
			MissionLibraryGameKeyConfigWidget screenWidget = this._screenWidget;
			if (screenWidget == null)
			{
				return;
			}
			screenWidget.SetCurrentOption(this, null);
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002233 File Offset: 0x00000433
		private void ResetCurrentOption()
		{
			MissionLibraryGameKeyConfigWidget screenWidget = this._screenWidget;
			if (screenWidget == null)
			{
				return;
			}
			screenWidget.SetCurrentOption(null, null);
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002248 File Offset: 0x00000448
		private void RegisterHoverEvents()
		{
			foreach (Widget widget in base.GetAllChildrenRecursive(null))
			{
				widget.PropertyChanged += this.Child_PropertyChanged;
			}
		}

		// Token: 0x0600002D RID: 45 RVA: 0x000022A8 File Offset: 0x000004A8
		private void Child_PropertyChanged(PropertyOwnerObject childWidget, string propertyName, object propertyValue)
		{
			if (propertyName != "IsHovered")
			{
				return;
			}
			if ((bool)propertyValue)
			{
				this.SetCurrentOption(false, false, -1);
				return;
			}
			this.ResetCurrentOption();
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600002E RID: 46 RVA: 0x000022D0 File Offset: 0x000004D0
		// (set) Token: 0x0600002F RID: 47 RVA: 0x000022D8 File Offset: 0x000004D8
		public string OptionTitle { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000030 RID: 48 RVA: 0x000022E1 File Offset: 0x000004E1
		// (set) Token: 0x06000031 RID: 49 RVA: 0x000022E9 File Offset: 0x000004E9
		public string OptionDescription { get; set; }

		// Token: 0x04000005 RID: 5
		private MissionLibraryGameKeyConfigWidget _screenWidget;

		// Token: 0x04000006 RID: 6
		private bool _eventsRegistered;
	}
}
