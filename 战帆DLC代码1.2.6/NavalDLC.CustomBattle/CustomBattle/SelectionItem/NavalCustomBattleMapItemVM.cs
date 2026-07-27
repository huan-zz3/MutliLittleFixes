using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;

namespace NavalDLC.CustomBattle.CustomBattle.SelectionItem
{
	// Token: 0x0200001F RID: 31
	public class NavalCustomBattleMapItemVM : SelectorItemVM
	{
		// Token: 0x170000AB RID: 171
		// (get) Token: 0x060001E0 RID: 480 RVA: 0x00008FF8 File Offset: 0x000071F8
		// (set) Token: 0x060001E1 RID: 481 RVA: 0x00009000 File Offset: 0x00007200
		public string MapName { get; private set; }

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x060001E2 RID: 482 RVA: 0x00009009 File Offset: 0x00007209
		// (set) Token: 0x060001E3 RID: 483 RVA: 0x00009011 File Offset: 0x00007211
		public string MapId { get; private set; }

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x060001E4 RID: 484 RVA: 0x0000901A File Offset: 0x0000721A
		// (set) Token: 0x060001E5 RID: 485 RVA: 0x00009022 File Offset: 0x00007222
		public TerrainType Terrain { get; private set; }

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x060001E6 RID: 486 RVA: 0x0000902B File Offset: 0x0000722B
		// (set) Token: 0x060001E7 RID: 487 RVA: 0x00009033 File Offset: 0x00007233
		public string ForcedSceneLevel { get; private set; }

		// Token: 0x060001E8 RID: 488 RVA: 0x0000903C File Offset: 0x0000723C
		public NavalCustomBattleMapItemVM(string mapName, string mapId, TerrainType terrain, string forcedSceneLevel)
			: base(mapName)
		{
			this.MapName = mapName;
			this.MapId = mapId;
			this.NameText = mapName;
			this.Terrain = terrain;
			this.ForcedSceneLevel = forcedSceneLevel;
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x0000906C File Offset: 0x0000726C
		public void UpdateSearchedText(string searchedText)
		{
			this._searchedText = searchedText;
			string text = null;
			if (this.MapName.IndexOf(this._searchedText, StringComparison.OrdinalIgnoreCase) != -1)
			{
				text = this.MapName.Substring(this.MapName.IndexOf(this._searchedText, StringComparison.OrdinalIgnoreCase), this._searchedText.Length);
			}
			if (!string.IsNullOrEmpty(text))
			{
				this.NameText = this.MapName.Replace(text, "<a>" + text + "</a>");
				return;
			}
			this.NameText = this.MapName;
		}

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x060001EA RID: 490 RVA: 0x000090F7 File Offset: 0x000072F7
		// (set) Token: 0x060001EB RID: 491 RVA: 0x000090FF File Offset: 0x000072FF
		[DataSourceProperty]
		public string NameText
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (this._nameText != value)
				{
					this._nameText = value;
					base.OnPropertyChangedWithValue<string>(value, "NameText");
				}
			}
		}

		// Token: 0x040000E7 RID: 231
		private string _searchedText;

		// Token: 0x040000EC RID: 236
		public string _nameText;
	}
}
