using System;
using System.Collections.Generic;
using System.Linq;
using MissionLibrary.HotKey;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;

namespace MissionSharedLibrary.Config.HotKey
{
	// Token: 0x0200003F RID: 63
	public class GameKeySequence : IGameKeySequence
	{
		// Token: 0x06000233 RID: 563 RVA: 0x000083F4 File Offset: 0x000065F4
		public GameKeySequence(int id, string stringId, string categoryId, List<GameKeySequenceAlternative> sequenceAlternatives, bool mandatory = false, List<GameKeySequenceAlternative> forbiddenAlternatives = null)
		{
			this.Id = id;
			this.StringId = stringId;
			this.CategoryId = categoryId;
			this.Mandatory = mandatory;
			this._defaultGameKeys = sequenceAlternatives;
			this._forbiddenGameKeys = forbiddenAlternatives ?? new List<GameKeySequenceAlternative>();
			sequenceAlternatives = this.NormalizeGameKeySequenceAlternatives(sequenceAlternatives);
			this.SetGameKeys(sequenceAlternatives);
		}

		// Token: 0x06000234 RID: 564 RVA: 0x00008450 File Offset: 0x00006650
		public SerializedGameKeySequence ToSerializedGameKeySequence()
		{
			SerializedGameKeySequence serializedGameKeySequence = new SerializedGameKeySequence();
			serializedGameKeySequence.StringId = this.StringId;
			serializedGameKeySequence.GameKeyAlternatives = this.NormalizeGameKeySequenceAlternatives(this.KeyAlternatives).Select<GameKeySequenceAlternative, SerializedGameKeySequenceAlternative>(delegate(GameKeySequenceAlternative sa)
			{
				SerializedGameKeySequenceAlternative serializedGameKeySequenceAlternative = new SerializedGameKeySequenceAlternative();
				serializedGameKeySequenceAlternative.KeyboardKeys = sa.Keys.Select<Key, InputKey>((Key key) => key.InputKey).ToList<InputKey>();
				return serializedGameKeySequenceAlternative;
			}).ToList<SerializedGameKeySequenceAlternative>();
			return serializedGameKeySequence;
		}

		// Token: 0x06000235 RID: 565 RVA: 0x000084AC File Offset: 0x000066AC
		public void SetGameKeys(List<GameKeySequenceAlternative> inputKeys)
		{
			List<GameKeySequenceAlternative> list = this.NormalizeGameKeySequenceAlternatives(inputKeys);
			if (this.Mandatory && list.Count == 0)
			{
				this.ResetToDefault();
				return;
			}
			this.KeyAlternatives = list;
		}

		// Token: 0x06000236 RID: 566 RVA: 0x000084DF File Offset: 0x000066DF
		public void ClearInvalidKeys()
		{
			this.KeyAlternatives = this.NormalizeGameKeySequenceAlternatives(this.KeyAlternatives);
		}

		// Token: 0x06000237 RID: 567 RVA: 0x000084F3 File Offset: 0x000066F3
		public void ResetToDefault()
		{
			this.SetGameKeys(this._defaultGameKeys);
		}

		// Token: 0x06000238 RID: 568 RVA: 0x00008504 File Offset: 0x00006704
		public string ToSequenceString()
		{
			List<string> list = (from sa in this.KeyAlternatives
				where sa.Keys.Any<Key>()
				select sa.ToHintString()).ToList<string>();
			if (list.Count == 0)
			{
				return "[No key]";
			}
			return string.Join(Module.CurrentModule.GlobalTextManager.FindText("str_mission_library_game_key_or", null).ToString(), list);
		}

		// Token: 0x06000239 RID: 569 RVA: 0x00008594 File Offset: 0x00006794
		private List<GameKeySequenceAlternative> NormalizeGameKeySequenceAlternatives(List<GameKeySequenceAlternative> alternatives)
		{
			List<GameKeySequenceAlternative> list = new List<GameKeySequenceAlternative>();
			foreach (GameKeySequenceAlternative gameKeySequenceAlternative in alternatives)
			{
				GameKeySequenceAlternative gameKeySequenceAlternative2 = new GameKeySequenceAlternative((from key in gameKeySequenceAlternative.Keys
					where key.InputKey != -1
					select key.InputKey).ToList<InputKey>());
				if (gameKeySequenceAlternative2.Keys.Any<Key>() && !this.IsSequenceAlternativeForbidden(gameKeySequenceAlternative2))
				{
					list.Add(gameKeySequenceAlternative2);
				}
			}
			return list;
		}

		// Token: 0x0600023A RID: 570 RVA: 0x0000865C File Offset: 0x0000685C
		private bool IsSequenceAlternativeForbidden(GameKeySequenceAlternative sequenceAlternative)
		{
			foreach (GameKeySequenceAlternative gameKeySequenceAlternative in this._forbiddenGameKeys)
			{
				if (sequenceAlternative.Keys.Select<Key, InputKey>((Key key) => key.InputKey).SequenceEqual<InputKey>(gameKeySequenceAlternative.Keys.Select<Key, InputKey>((Key key) => key.InputKey)))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600023B RID: 571 RVA: 0x0000870C File Offset: 0x0000690C
		public bool IsKeyDownInOrder(IInputContext input = null)
		{
			bool flag = false;
			for (int i = 0; i < this.KeyAlternatives.Count; i++)
			{
				flag |= this.KeyAlternatives[i].IsKeyDownInOrder(input);
			}
			return flag;
		}

		// Token: 0x0600023C RID: 572 RVA: 0x00008748 File Offset: 0x00006948
		public bool IsKeyPressedInOrder(IInputContext input = null)
		{
			bool flag = false;
			for (int i = 0; i < this.KeyAlternatives.Count; i++)
			{
				flag |= this.KeyAlternatives[i].IsKeyPressedInOrder(input);
			}
			return flag;
		}

		// Token: 0x0600023D RID: 573 RVA: 0x00008784 File Offset: 0x00006984
		public bool IsKeyReleasedInOrder(IInputContext input = null)
		{
			bool flag = false;
			for (int i = 0; i < this.KeyAlternatives.Count; i++)
			{
				flag |= this.KeyAlternatives[i].IsKeyReleasedInOrder(input);
			}
			return flag;
		}

		// Token: 0x0600023E RID: 574 RVA: 0x000087C0 File Offset: 0x000069C0
		public bool IsKeyDown(IInputContext input = null)
		{
			bool flag = false;
			for (int i = 0; i < this.KeyAlternatives.Count; i++)
			{
				flag |= this.KeyAlternatives[i].IsKeyDown(input);
			}
			return flag;
		}

		// Token: 0x0600023F RID: 575 RVA: 0x000087FC File Offset: 0x000069FC
		public bool IsKeyPressed(IInputContext input = null)
		{
			bool flag = false;
			for (int i = 0; i < this.KeyAlternatives.Count; i++)
			{
				flag |= this.KeyAlternatives[i].IsKeyPressed(input);
			}
			return flag;
		}

		// Token: 0x06000240 RID: 576 RVA: 0x00008838 File Offset: 0x00006A38
		public bool IsKeyReleased(IInputContext input = null)
		{
			bool flag = false;
			for (int i = 0; i < this.KeyAlternatives.Count; i++)
			{
				flag |= this.KeyAlternatives[i].IsKeyReleased(input);
			}
			return flag;
		}

		// Token: 0x040000DD RID: 221
		public int Id;

		// Token: 0x040000DE RID: 222
		public string StringId;

		// Token: 0x040000DF RID: 223
		public string CategoryId;

		// Token: 0x040000E0 RID: 224
		public List<GameKeySequenceAlternative> KeyAlternatives;

		// Token: 0x040000E1 RID: 225
		public bool Mandatory;

		// Token: 0x040000E2 RID: 226
		private readonly List<GameKeySequenceAlternative> _defaultGameKeys;

		// Token: 0x040000E3 RID: 227
		private readonly List<GameKeySequenceAlternative> _forbiddenGameKeys;
	}
}
