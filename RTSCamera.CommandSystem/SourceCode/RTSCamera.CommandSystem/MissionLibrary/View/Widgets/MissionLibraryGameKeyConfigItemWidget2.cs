using System;
using System.Linq;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace MissionLibrary.View.Widgets
{
	// Token: 0x02000002 RID: 2
	public class MissionLibraryGameKeyConfigItemWidget2 : ListPanel
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public MissionLibraryGameKeyConfigItemWidget2(UIContext context)
			: base(context)
		{
		}

		// Token: 0x06000002 RID: 2 RVA: 0x0000205C File Offset: 0x0000025C
		protected override void OnLateUpdate(float dt)
		{
			base.OnLateUpdate(dt);
			if (!this._initialized)
			{
				this._initialized = true;
				if (this.V1ui != null)
				{
					this.V1ui.IsVisible = this.VMVersion == null || this.VMVersion == "v1";
				}
				if (this.V2ui != null)
				{
					this.V2ui.IsVisible = this.VMVersion == "v2";
				}
			}
			if (this._screenWidget == null)
			{
				this._screenWidget = base.EventManager.Root.GetChild(0).FindChild("Options", true) as MissionLibraryGameKeyConfigWidget2;
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020FF File Offset: 0x000002FF
		protected override void OnChildAdded(Widget child)
		{
			base.OnChildAdded(child);
			child.boolPropertyChanged += this.Child_BoolPropertyChanged;
			child.EventFire += this.OnEventFired;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x0000212C File Offset: 0x0000032C
		public override void OnBeforeRemovedChild(Widget widget)
		{
			base.OnBeforeRemovedChild(widget);
			widget.boolPropertyChanged -= this.Child_BoolPropertyChanged;
			widget.EventFire -= this.OnEventFired;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002159 File Offset: 0x00000359
		protected override void OnHoverBegin()
		{
			base.OnHoverBegin();
			this.SetCurrentOption(false, false, -1);
		}

		// Token: 0x06000006 RID: 6 RVA: 0x0000216A File Offset: 0x0000036A
		protected override void OnHoverEnd()
		{
			base.OnHoverEnd();
			this.ResetCurrentOption();
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002178 File Offset: 0x00000378
		private void SetCurrentOption(bool fromHoverOverDropdown, bool fromBooleanSelection, int hoverDropdownItemIndex = -1)
		{
			MissionLibraryGameKeyConfigWidget2 screenWidget = this._screenWidget;
			if (screenWidget == null)
			{
				return;
			}
			screenWidget.SetCurrentOption(this, null);
		}

		// Token: 0x06000008 RID: 8 RVA: 0x0000218C File Offset: 0x0000038C
		private void ResetCurrentOption()
		{
			MissionLibraryGameKeyConfigWidget2 screenWidget = this._screenWidget;
			if (screenWidget == null)
			{
				return;
			}
			screenWidget.SetCurrentOption(null, null);
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000021A0 File Offset: 0x000003A0
		private void RegisterHoverEvents()
		{
			foreach (Widget widget in base.GetAllChildrenRecursive(null))
			{
				widget.boolPropertyChanged += this.Child_BoolPropertyChanged;
			}
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002200 File Offset: 0x00000400
		private void Child_BoolPropertyChanged(PropertyOwnerObject childWidget, string propertyName, bool propertyValue)
		{
			if (propertyName != "IsHovered")
			{
				return;
			}
			if (propertyValue)
			{
				this.SetCurrentOption(false, false, -1);
				return;
			}
			this.ResetCurrentOption();
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002224 File Offset: 0x00000424
		private void OnEventFired(Widget w, string name, object[] obj)
		{
			if (name == "ItemAdd")
			{
				Widget widget = obj.FirstOrDefault<object>() as Widget;
				if (widget != null)
				{
					widget.boolPropertyChanged += this.Child_BoolPropertyChanged;
					widget.EventFire += this.OnEventFired;
					return;
				}
			}
			else if (name == "ItemRemove")
			{
				Widget widget2 = obj.FirstOrDefault<object>() as Widget;
				if (widget2 != null)
				{
					widget2.boolPropertyChanged -= this.Child_BoolPropertyChanged;
					widget2.EventFire -= this.OnEventFired;
				}
			}
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600000C RID: 12 RVA: 0x000022B2 File Offset: 0x000004B2
		// (set) Token: 0x0600000D RID: 13 RVA: 0x000022BA File Offset: 0x000004BA
		public string OptionTitle { get; set; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000E RID: 14 RVA: 0x000022C3 File Offset: 0x000004C3
		// (set) Token: 0x0600000F RID: 15 RVA: 0x000022CB File Offset: 0x000004CB
		public string OptionDescription { get; set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000010 RID: 16 RVA: 0x000022D4 File Offset: 0x000004D4
		// (set) Token: 0x06000011 RID: 17 RVA: 0x000022DC File Offset: 0x000004DC
		public string VMVersion { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000012 RID: 18 RVA: 0x000022E5 File Offset: 0x000004E5
		// (set) Token: 0x06000013 RID: 19 RVA: 0x000022ED File Offset: 0x000004ED
		public Widget V2ui { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000014 RID: 20 RVA: 0x000022F6 File Offset: 0x000004F6
		// (set) Token: 0x06000015 RID: 21 RVA: 0x000022FE File Offset: 0x000004FE
		public Widget V1ui { get; set; }

		// Token: 0x04000001 RID: 1
		private MissionLibraryGameKeyConfigWidget2 _screenWidget;

		// Token: 0x04000002 RID: 2
		private bool _initialized;
	}
}
