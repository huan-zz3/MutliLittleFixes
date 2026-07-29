using System;
using TaleWorlds.InputSystem;

namespace MissionLibrary.HotKey
{
	// Token: 0x0200001D RID: 29
	public interface IGameKeySequence
	{
		// Token: 0x06000069 RID: 105
		bool IsKeyDownInOrder(IInputContext input = null);

		// Token: 0x0600006A RID: 106
		bool IsKeyPressedInOrder(IInputContext input = null);

		// Token: 0x0600006B RID: 107
		bool IsKeyReleasedInOrder(IInputContext input = null);

		// Token: 0x0600006C RID: 108
		bool IsKeyDown(IInputContext input = null);

		// Token: 0x0600006D RID: 109
		bool IsKeyPressed(IInputContext input = null);

		// Token: 0x0600006E RID: 110
		bool IsKeyReleased(IInputContext input = null);

		// Token: 0x0600006F RID: 111
		string ToSequenceString();
	}
}
