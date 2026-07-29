using System;
using System.Collections.Generic;
using MissionLibrary.HotKey;
using MissionLibrary.Repository;
using MissionSharedLibrary.Config;
using MissionSharedLibrary.Config.HotKey;
using MissionSharedLibrary.HotKey;
using MissionSharedLibrary.Usage;
using TaleWorlds.InputSystem;

namespace RTSCamera.CommandSystem.Config.HotKey
{
	// Token: 0x02000094 RID: 148
	public class CommandSystemGameKeyCategory
	{
		// Token: 0x170000CE RID: 206
		// (get) Token: 0x0600055A RID: 1370 RVA: 0x0001FD13 File Offset: 0x0001DF13
		public static AGameKeyCategory Category
		{
			get
			{
				return ARepository<AGameKeyCategoryManager, AGameKeyCategory>.Get().GetItem("RTSCameraCommandSystemHotKey");
			}
		}

		// Token: 0x0600055B RID: 1371 RVA: 0x0001FD24 File Offset: 0x0001DF24
		public static void RegisterGameKeyCategory()
		{
			AGameKeyCategoryManager agameKeyCategoryManager = ARepository<AGameKeyCategoryManager, AGameKeyCategory>.Get();
			if (agameKeyCategoryManager == null)
			{
				return;
			}
			agameKeyCategoryManager.RegisterGameKeyCategory(new Func<AGameKeyCategory>(CommandSystemGameKeyCategory.CreateCategory), "RTSCameraCommandSystemHotKey", new Version(1, 0), true);
		}

		// Token: 0x0600055C RID: 1372 RVA: 0x0001FD50 File Offset: 0x0001DF50
		public static GameKeyCategory CreateCategory()
		{
			GameKeyCategory gameKeyCategory = new GameKeyCategory("RTSCameraCommandSystemHotKey", 9, MissionConfigBase<CommandSystemGameKeyConfig>.Get());
			gameKeyCategory.AddGameKeySequence(new GameKeySequence(0, "SelectFormation", "RTSCameraCommandSystemHotKey", new List<GameKeySequenceAlternative>
			{
				new GameKeySequenceAlternative(new List<InputKey> { 226 })
			}, false, null));
			gameKeyCategory.AddGameKeySequence(new GameKeySequence(1, "KeepMovementOrder", "RTSCameraCommandSystemHotKey", new List<GameKeySequenceAlternative>
			{
				new GameKeySequenceAlternative(new List<InputKey> { 56 }),
				new GameKeySequenceAlternative(new List<InputKey> { 184 })
			}, false, null));
			gameKeyCategory.AddGameKeySequence(new GameKeySequence(2, "FormationLockMovement", "RTSCameraCommandSystemHotKey", new List<GameKeySequenceAlternative>
			{
				new GameKeySequenceAlternative(new List<InputKey> { 56 }),
				new GameKeySequenceAlternative(new List<InputKey> { 184 })
			}, false, null));
			gameKeyCategory.AddGameKeySequence(new GameKeySequence(3, "SelectTargetForCommand", "RTSCameraCommandSystemHotKey", new List<GameKeySequenceAlternative>
			{
				new GameKeySequenceAlternative(new List<InputKey> { 56 }),
				new GameKeySequenceAlternative(new List<InputKey> { 184 })
			}, false, null));
			gameKeyCategory.AddGameKeySequence(new GameKeySequence(4, "CommandQueue", "RTSCameraCommandSystemHotKey", new List<GameKeySequenceAlternative>
			{
				new GameKeySequenceAlternative(new List<InputKey> { 42 }),
				new GameKeySequenceAlternative(new List<InputKey> { 54 })
			}, false, null));
			gameKeyCategory.AddGameKeySequence(new GameKeySequence(5, "KeepFormationWidth", "RTSCameraCommandSystemHotKey", new List<GameKeySequenceAlternative>
			{
				new GameKeySequenceAlternative(new List<InputKey> { 29 }),
				new GameKeySequenceAlternative(new List<InputKey> { 157 })
			}, false, null));
			gameKeyCategory.AddGameKeySequence(new GameKeySequence(6, "AutoVolley", "RTSCameraCommandSystemHotKey", new List<GameKeySequenceAlternative>
			{
				new GameKeySequenceAlternative(new List<InputKey> { 35 })
			}, false, null));
			gameKeyCategory.AddGameKeySequence(new GameKeySequence(7, "ManualVolley", "RTSCameraCommandSystemHotKey", new List<GameKeySequenceAlternative>
			{
				new GameKeySequenceAlternative(new List<InputKey> { 36 })
			}, false, null));
			gameKeyCategory.AddGameKeySequence(new GameKeySequence(8, "VolleyFire", "RTSCameraCommandSystemHotKey", new List<GameKeySequenceAlternative>
			{
				new GameKeySequenceAlternative(new List<InputKey> { 37 })
			}, false, null));
			return gameKeyCategory;
		}

		// Token: 0x0600055D RID: 1373 RVA: 0x0001FFD2 File Offset: 0x0001E1D2
		public static IGameKeySequence GetKey(GameKeyEnum key)
		{
			AGameKeyCategory category = CommandSystemGameKeyCategory.Category;
			if (category == null)
			{
				return null;
			}
			return category.GetGameKeySequence((int)key);
		}

		// Token: 0x0400029A RID: 666
		public const string CategoryId = "RTSCameraCommandSystemHotKey";
	}
}
