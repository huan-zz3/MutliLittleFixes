using System;
using System.Collections.Generic;
using MissionLibrary.Config.HotKey;
using MissionLibrary.HotKey;
using MissionLibrary.Repository;
using MissionSharedLibrary.Config;
using MissionSharedLibrary.Config.HotKey;
using MissionSharedLibrary.Usage;
using TaleWorlds.InputSystem;

namespace MissionSharedLibrary.HotKey
{
	// Token: 0x0200000C RID: 12
	public class GeneralGameKeyCategory
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000089 RID: 137 RVA: 0x00004382 File Offset: 0x00002582
		public static AGameKeyCategory Category
		{
			get
			{
				AGameKeyCategoryManager agameKeyCategoryManager = ARepository<AGameKeyCategoryManager, AGameKeyCategory>.Get();
				if (agameKeyCategoryManager == null)
				{
					return null;
				}
				return agameKeyCategoryManager.GetItem("MissionLibraryGeneralGameKey");
			}
		}

		// Token: 0x0600008A RID: 138 RVA: 0x0000439C File Offset: 0x0000259C
		public static AGameKeyCategory CreateGeneralGameKeyCategory()
		{
			GameKeyCategory gameKeyCategory = new GameKeyCategory("MissionLibraryGeneralGameKey", 1, MissionConfigBase<GeneralGameKeyConfig>.Get());
			gameKeyCategory.AddGameKeySequence(new GameKeySequence(0, "OpenMenu", "MissionLibraryGeneralGameKey", new List<GameKeySequenceAlternative>
			{
				new GameKeySequenceAlternative(new List<InputKey> { 38 })
			}, true, new List<GameKeySequenceAlternative>
			{
				new GameKeySequenceAlternative(new List<InputKey> { 224 }),
				new GameKeySequenceAlternative(new List<InputKey> { 225 })
			}));
			return gameKeyCategory;
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00004428 File Offset: 0x00002628
		public static void RegisterGameKeyCategory()
		{
			AGameKeyCategoryManager agameKeyCategoryManager = ARepository<AGameKeyCategoryManager, AGameKeyCategory>.Get();
			if (agameKeyCategoryManager == null)
			{
				return;
			}
			agameKeyCategoryManager.RegisterGameKeyCategory(new Func<AGameKeyCategory>(GeneralGameKeyCategory.CreateGeneralGameKeyCategory), "MissionLibraryGeneralGameKey", new Version(2, 0), true);
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00004452 File Offset: 0x00002652
		public static IGameKeySequence GetKey(GeneralGameKey key)
		{
			return GeneralGameKeyCategory.Category.GetGameKeySequence((int)key);
		}

		// Token: 0x04000032 RID: 50
		public const string CategoryId = "MissionLibraryGeneralGameKey";
	}
}
