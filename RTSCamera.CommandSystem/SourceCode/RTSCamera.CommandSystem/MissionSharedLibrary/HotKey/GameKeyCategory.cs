using System;
using System.Collections.Generic;
using System.Linq;
using MissionLibrary.HotKey;
using MissionSharedLibrary.Config.HotKey;
using MissionSharedLibrary.Utilities;
using MissionSharedLibrary.View.ViewModelCollection.HotKey;

namespace MissionSharedLibrary.HotKey
{
	// Token: 0x0200000A RID: 10
	public class GameKeyCategory : AGameKeyCategory
	{
		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600007E RID: 126 RVA: 0x000040BF File Offset: 0x000022BF
		public List<GameKeySequence> GameKeySequences { get; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600007F RID: 127 RVA: 0x000040C7 File Offset: 0x000022C7
		public override string ItemId { get; }

		// Token: 0x06000080 RID: 128 RVA: 0x000040CF File Offset: 0x000022CF
		public GameKeySequence GetKeySequence(int i)
		{
			if (this.GameKeySequences == null || i < 0 || i >= this.GameKeySequences.Count)
			{
				return new GameKeySequence(0, "", "", new List<GameKeySequenceAlternative>(), false, null);
			}
			return this.GameKeySequences[i];
		}

		// Token: 0x06000081 RID: 129 RVA: 0x0000410F File Offset: 0x0000230F
		public override IGameKeySequence GetGameKeySequence(int i)
		{
			return this.GetKeySequence(i);
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00004118 File Offset: 0x00002318
		public SerializedGameKeyCategory ToSerializedGameKeyCategory()
		{
			SerializedGameKeyCategory serializedGameKeyCategory = new SerializedGameKeyCategory();
			serializedGameKeyCategory.CategoryId = this.ItemId;
			serializedGameKeyCategory.GameKeySequences = this.GameKeySequences.Select<GameKeySequence, SerializedGameKeySequence>((GameKeySequence sequence) => sequence.ToSerializedGameKeySequence()).ToList<SerializedGameKeySequence>();
			return serializedGameKeyCategory;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x0000416C File Offset: 0x0000236C
		public void FromSerializedGameKeyCategory(SerializedGameKeyCategory category)
		{
			Dictionary<string, SerializedGameKeySequence> dictionary = (from stringId in category.GameKeySequences.Select<SerializedGameKeySequence, string>((SerializedGameKeySequence gameKeySequence) => gameKeySequence.StringId).Distinct<string>()
				select category.GameKeySequences.First<SerializedGameKeySequence>((SerializedGameKeySequence gameKeySequence) => gameKeySequence.StringId == stringId)).ToDictionary<SerializedGameKeySequence, string>((SerializedGameKeySequence serializedGameKey) => serializedGameKey.StringId);
			for (int i = 0; i < this.GameKeySequences.Count; i++)
			{
				GameKeySequence gameKeySequence2 = this.GameKeySequences[i];
				SerializedGameKeySequence serializedGameKeySequence;
				if (dictionary.TryGetValue(gameKeySequence2.StringId, out serializedGameKeySequence))
				{
					this.GameKeySequences[i].SetGameKeys(serializedGameKeySequence.GameKeyAlternatives.Select<SerializedGameKeySequenceAlternative, GameKeySequenceAlternative>((SerializedGameKeySequenceAlternative sa) => new GameKeySequenceAlternative(sa.KeyboardKeys)).ToList<GameKeySequenceAlternative>());
				}
			}
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00004264 File Offset: 0x00002464
		public override void Save()
		{
			try
			{
				this._config.Category = this.ToSerializedGameKeyCategory();
				this._config.Serialize();
			}
			catch (Exception ex)
			{
				Utility.DisplayMessage(ex.ToString());
				Console.WriteLine(ex);
			}
		}

		// Token: 0x06000085 RID: 133 RVA: 0x000042B4 File Offset: 0x000024B4
		public override void Load()
		{
			try
			{
				this.FromSerializedGameKeyCategory(this._config.Category);
			}
			catch (Exception ex)
			{
				Utility.DisplayMessage(ex.ToString());
				Console.WriteLine(ex);
			}
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000042F8 File Offset: 0x000024F8
		public GameKeyCategory(string categoryId, int gameKeysCount, IGameKeyConfig config)
		{
			this.ItemId = categoryId;
			this._config = config;
			this.GameKeySequences = new List<GameKeySequence>(gameKeysCount);
			for (int i = 0; i < gameKeysCount; i++)
			{
				this.GameKeySequences.Add(null);
			}
		}

		// Token: 0x06000087 RID: 135 RVA: 0x0000433D File Offset: 0x0000253D
		public void AddGameKeySequence(GameKeySequence gameKeySequence)
		{
			if (gameKeySequence.Id < 0 || gameKeySequence.Id >= this.GameKeySequences.Count)
			{
				return;
			}
			this.GameKeySequences[gameKeySequence.Id] = gameKeySequence;
		}

		// Token: 0x06000088 RID: 136 RVA: 0x0000436E File Offset: 0x0000256E
		public override AHotKeyConfigVM CreateViewModel(Action<IHotKeySetter> onKeyBindRequest)
		{
			return new MissionLibraryGameKeySequenceGroupVM(this.ItemId, this.GameKeySequences, onKeyBindRequest);
		}

		// Token: 0x0400002E RID: 46
		private readonly IGameKeyConfig _config;
	}
}
