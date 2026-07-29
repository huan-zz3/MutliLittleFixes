using System;
using System.Collections.Generic;
using System.Linq;
using MissionSharedLibrary.Utilities;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;

namespace MissionSharedLibrary.Config.HotKey
{
	// Token: 0x0200003E RID: 62
	public class GameKeySequenceAlternative
	{
		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000223 RID: 547 RVA: 0x00007F28 File Offset: 0x00006128
		// (set) Token: 0x06000224 RID: 548 RVA: 0x00007F30 File Offset: 0x00006130
		public List<Key> Keys { get; set; } = new List<Key>();

		// Token: 0x06000225 RID: 549 RVA: 0x00007F3C File Offset: 0x0000613C
		public GameKeySequenceAlternative(List<InputKey> keys)
		{
			this.Keys = keys.Select<InputKey, Key>((InputKey key) => new Key(key)).ToList<Key>();
		}

		// Token: 0x06000226 RID: 550 RVA: 0x00007F8C File Offset: 0x0000618C
		public SerializedGameKeySequenceAlternative ToSerializedGameKeySequenceAlternative()
		{
			SerializedGameKeySequenceAlternative serializedGameKeySequenceAlternative = new SerializedGameKeySequenceAlternative();
			serializedGameKeySequenceAlternative.KeyboardKeys = (from key in this.Keys
				select key.InputKey into inputKey
				where inputKey != -1
				select inputKey).ToList<InputKey>();
			return serializedGameKeySequenceAlternative;
		}

		// Token: 0x06000227 RID: 551 RVA: 0x00007FF8 File Offset: 0x000061F8
		public void SetGameKeys(List<InputKey> inputKeys)
		{
			List<Key> list = (from key in inputKeys
				where key != -1
				select key into inputKey
				select new Key(inputKey)).ToList<Key>();
			if (list.Count == 0)
			{
				return;
			}
			this.Keys = list;
		}

		// Token: 0x06000228 RID: 552 RVA: 0x00008064 File Offset: 0x00006264
		public bool IsKeyDownInOrder(IInputContext input = null)
		{
			if (!this.CheckCurrentProgress(input))
			{
				return false;
			}
			for (int i = this._progress; i < this.Keys.Count; i++)
			{
				if (!this.IsKeyDown(input, i))
				{
					return false;
				}
				this._progress++;
			}
			return true;
		}

		// Token: 0x06000229 RID: 553 RVA: 0x000080B4 File Offset: 0x000062B4
		public bool IsKeyPressedInOrder(IInputContext input = null)
		{
			if (!this.CheckCurrentProgress(input))
			{
				return false;
			}
			for (int i = this._progress; i < this.Keys.Count - 1; i++)
			{
				if (!this.IsKeyDown(input, i))
				{
					return false;
				}
				this._progress++;
			}
			return this.IsKeyPressed(input, this.Keys.Count - 1);
		}

		// Token: 0x0600022A RID: 554 RVA: 0x0000811C File Offset: 0x0000631C
		public bool IsKeyReleasedInOrder(IInputContext input = null)
		{
			if (!this.CheckCurrentProgress(input))
			{
				return false;
			}
			for (int i = this._progress; i < this.Keys.Count - 1; i++)
			{
				if (!this.IsKeyDown(input, i))
				{
					return false;
				}
				this._progress++;
			}
			return this.IsKeyReleased(input, this.Keys.Count - 1);
		}

		// Token: 0x0600022B RID: 555 RVA: 0x00008184 File Offset: 0x00006384
		public bool IsKeyDown(IInputContext input = null)
		{
			if (this.Keys.Count == 0)
			{
				return false;
			}
			for (int i = 0; i < this.Keys.Count; i++)
			{
				if (!this.IsKeyDown(input, i))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600022C RID: 556 RVA: 0x000081C4 File Offset: 0x000063C4
		public bool IsKeyPressed(IInputContext input = null)
		{
			if (this.Keys.Count == 0)
			{
				return false;
			}
			for (int i = 0; i < this.Keys.Count - 1; i++)
			{
				if (!this.IsKeyDown(input, i))
				{
					return false;
				}
			}
			return this.IsKeyPressed(input, this.Keys.Count - 1);
		}

		// Token: 0x0600022D RID: 557 RVA: 0x00008218 File Offset: 0x00006418
		public bool IsKeyReleased(IInputContext input = null)
		{
			if (this.Keys.Count == 0)
			{
				return false;
			}
			for (int i = 0; i < this.Keys.Count - 1; i++)
			{
				if (!this.IsKeyDown(input, i))
				{
					return false;
				}
			}
			return this.IsKeyReleased(input, this.Keys.Count - 1);
		}

		// Token: 0x0600022E RID: 558 RVA: 0x0000826C File Offset: 0x0000646C
		private bool CheckCurrentProgress(IInputContext input)
		{
			if (this.Keys == null || this.Keys.Count == 0)
			{
				return false;
			}
			for (int i = 0; i < this._progress; i++)
			{
				if (!this.IsKeyDown(input, i))
				{
					this._progress = i;
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600022F RID: 559 RVA: 0x000082B5 File Offset: 0x000064B5
		private bool IsKeyDown(IInputContext input, int i)
		{
			if (input == null)
			{
				return Input.IsKeyDown(this.Keys[i].InputKey);
			}
			return input.IsKeyDown(this.Keys[i].InputKey);
		}

		// Token: 0x06000230 RID: 560 RVA: 0x000082E8 File Offset: 0x000064E8
		private bool IsKeyPressed(IInputContext input, int i)
		{
			if (input == null)
			{
				return Input.IsKeyPressed(this.Keys[i].InputKey);
			}
			return input.IsKeyPressed(this.Keys[i].InputKey);
		}

		// Token: 0x06000231 RID: 561 RVA: 0x0000831B File Offset: 0x0000651B
		private bool IsKeyReleased(IInputContext input, int i)
		{
			if (input == null)
			{
				return Input.IsKeyReleased(this.Keys[i].InputKey);
			}
			return input.IsKeyReleased(this.Keys[i].InputKey);
		}

		// Token: 0x06000232 RID: 562 RVA: 0x00008350 File Offset: 0x00006550
		public string ToHintString()
		{
			if (this.Keys.Count == 0)
			{
				return "[No key]";
			}
			string text = "";
			for (int i = 0; i < this.Keys.Count - 1; i++)
			{
				string text2 = text;
				TextObject textObject = Utility.TextForKey(this.Keys[i].InputKey);
				text = text2 + ((textObject != null) ? textObject.ToString() : null) + "+";
			}
			string text3 = text;
			TextObject textObject2 = Utility.TextForKey(this.Keys[this.Keys.Count - 1].InputKey);
			return text3 + ((textObject2 != null) ? textObject2.ToString() : null);
		}

		// Token: 0x040000DC RID: 220
		private int _progress;
	}
}
