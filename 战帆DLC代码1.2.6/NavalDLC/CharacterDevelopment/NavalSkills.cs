using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace NavalDLC.CharacterDevelopment
{
	// Token: 0x0200015E RID: 350
	public class NavalSkills
	{
		// Token: 0x170003EE RID: 1006
		// (get) Token: 0x060016D2 RID: 5842 RVA: 0x0009BA45 File Offset: 0x00099C45
		private static NavalSkills Instance
		{
			get
			{
				return NavalDLCManager.Instance.NavalSkills;
			}
		}

		// Token: 0x170003EF RID: 1007
		// (get) Token: 0x060016D3 RID: 5843 RVA: 0x0009BA51 File Offset: 0x00099C51
		public static SkillObject Mariner
		{
			get
			{
				return NavalSkills.Instance._skillMariner;
			}
		}

		// Token: 0x170003F0 RID: 1008
		// (get) Token: 0x060016D4 RID: 5844 RVA: 0x0009BA5D File Offset: 0x00099C5D
		public static SkillObject Boatswain
		{
			get
			{
				return NavalSkills.Instance._skillBoatswain;
			}
		}

		// Token: 0x170003F1 RID: 1009
		// (get) Token: 0x060016D5 RID: 5845 RVA: 0x0009BA69 File Offset: 0x00099C69
		public static SkillObject Shipmaster
		{
			get
			{
				return NavalSkills.Instance._skillShipmaster;
			}
		}

		// Token: 0x060016D6 RID: 5846 RVA: 0x0009BA75 File Offset: 0x00099C75
		private SkillObject Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<SkillObject>(new SkillObject(stringId));
		}

		// Token: 0x060016D7 RID: 5847 RVA: 0x0009BA8C File Offset: 0x00099C8C
		private void InitializeAll()
		{
			this._skillMariner.Initialize(new TextObject("{=bOhiqquf}Mariner", null), new TextObject("{=JSvE81Iw}Enhances your personal combat prowess during naval engagements and bolsters your effectiveness in leading troops and employing tactics in sea battles.", null), new CharacterAttribute[]
			{
				DefaultCharacterAttributes.Endurance,
				DefaultCharacterAttributes.Cunning
			});
			this._skillBoatswain.Initialize(new TextObject("{=olTmdP9j}Boatswain", null), new TextObject("{=SZ0BH8b1}Governs the well-being and discipline of your ship's crew, as well as the vessel's overall combat readiness, including rigging and supplies.", null), new CharacterAttribute[]
			{
				DefaultCharacterAttributes.Control,
				DefaultCharacterAttributes.Social
			});
			this._skillShipmaster.Initialize(new TextObject("{=SSLTboWZ}Shipmaster", null), new TextObject("{=CmXMqtcU}Improves your navigational abilities, the effectiveness of naval siege engines under your command, and the speed and quality of ship repairs and upgrades.", null), new CharacterAttribute[]
			{
				DefaultCharacterAttributes.Vigor,
				DefaultCharacterAttributes.Intelligence
			});
		}

		// Token: 0x060016D8 RID: 5848 RVA: 0x0009BB41 File Offset: 0x00099D41
		public NavalSkills()
		{
			this.RegisterAll();
		}

		// Token: 0x060016D9 RID: 5849 RVA: 0x0009BB4F File Offset: 0x00099D4F
		private void RegisterAll()
		{
			this._skillMariner = this.Create("Mariner");
			this._skillBoatswain = this.Create("Boatswain");
			this._skillShipmaster = this.Create("Shipmaster");
			this.InitializeAll();
		}

		// Token: 0x04000BBC RID: 3004
		private SkillObject _skillMariner;

		// Token: 0x04000BBD RID: 3005
		private SkillObject _skillBoatswain;

		// Token: 0x04000BBE RID: 3006
		private SkillObject _skillShipmaster;
	}
}
