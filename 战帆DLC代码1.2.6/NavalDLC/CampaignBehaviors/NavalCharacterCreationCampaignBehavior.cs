using System;
using System.Collections.Generic;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace NavalDLC.CampaignBehaviors
{
	// Token: 0x02000163 RID: 355
	public class NavalCharacterCreationCampaignBehavior : CampaignBehaviorBase, ICharacterCreationContentHandler
	{
		// Token: 0x060016FE RID: 5886 RVA: 0x0009CA0C File Offset: 0x0009AC0C
		private string GetMotherEquipmentId(CharacterCreationManager characterCreationManager, string occupationType, string cultureId)
		{
			string text;
			characterCreationManager.CharacterCreationContent.TryGetEquipmentToUse(occupationType, ref text);
			return "mother_char_creation_" + text + "_" + cultureId;
		}

		// Token: 0x060016FF RID: 5887 RVA: 0x0009CA3C File Offset: 0x0009AC3C
		private string GetFatherEquipmentId(CharacterCreationManager characterCreationManager, string occupationType, string cultureId)
		{
			string text;
			characterCreationManager.CharacterCreationContent.TryGetEquipmentToUse(occupationType, ref text);
			return "father_char_creation_" + text + "_" + cultureId;
		}

		// Token: 0x06001700 RID: 5888 RVA: 0x0009CA6C File Offset: 0x0009AC6C
		private string GetPlayerEquipmentId(CharacterCreationManager characterCreationManager, string occupationType, string cultureId, bool isFemale)
		{
			string text;
			characterCreationManager.CharacterCreationContent.TryGetEquipmentToUse(occupationType, ref text);
			return string.Concat(new string[]
			{
				"player_char_creation_",
				cultureId,
				"_",
				text,
				"_",
				isFemale ? "f" : "m"
			});
		}

		// Token: 0x06001701 RID: 5889 RVA: 0x0009CAC6 File Offset: 0x0009ACC6
		public override void RegisterEvents()
		{
			CampaignEvents.OnCharacterCreationInitializedEvent.AddNonSerializedListener(this, new Action<CharacterCreationManager>(this.OnCharacterCreationInitialized));
		}

		// Token: 0x06001702 RID: 5890 RVA: 0x0009CADF File Offset: 0x0009ACDF
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x06001703 RID: 5891 RVA: 0x0009CAE4 File Offset: 0x0009ACE4
		private void OnCharacterCreationInitialized(CharacterCreationManager characterCreationManager)
		{
			this._focusToAdd = characterCreationManager.CharacterCreationContent.FocusToAdd;
			this._skillLevelToAdd = characterCreationManager.CharacterCreationContent.SkillLevelToAdd;
			this._attributeLevelToAdd = characterCreationManager.CharacterCreationContent.AttributeLevelToAdd;
			characterCreationManager.CharacterCreationContent.DefaultSelectedTitleType = "guard";
			characterCreationManager.RegisterCharacterCreationContentHandler(this, 1000);
		}

		// Token: 0x06001704 RID: 5892 RVA: 0x0009CB40 File Offset: 0x0009AD40
		void ICharacterCreationContentHandler.InitializeContent(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.AddEquipmentToUseGetter(delegate(string occupationId, out string equipmentId)
			{
				return this._occupationToEquipmentMapping.TryGetValue(occupationId, out equipmentId);
			});
			this.InitializeCharacterCreationCultures(characterCreationManager);
			this.InitializeData(characterCreationManager);
		}

		// Token: 0x06001705 RID: 5893 RVA: 0x0009CB67 File Offset: 0x0009AD67
		void ICharacterCreationContentHandler.AfterInitializeContent(CharacterCreationManager characterCreationManager)
		{
		}

		// Token: 0x06001706 RID: 5894 RVA: 0x0009CB69 File Offset: 0x0009AD69
		void ICharacterCreationContentHandler.OnStageCompleted(CharacterCreationStageBase stage)
		{
		}

		// Token: 0x06001707 RID: 5895 RVA: 0x0009CB6B File Offset: 0x0009AD6B
		void ICharacterCreationContentHandler.OnCharacterCreationFinalize(CharacterCreationManager characterCreationManager)
		{
		}

		// Token: 0x06001708 RID: 5896 RVA: 0x0009CB6D File Offset: 0x0009AD6D
		public void InitializeCharacterCreationCultures(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.AddCharacterCreationCulture(Game.Current.ObjectManager.GetObject<CultureObject>("nord"), 1, 10);
		}

		// Token: 0x06001709 RID: 5897 RVA: 0x0009CB94 File Offset: 0x0009AD94
		public void InitializeData(CharacterCreationManager characterCreationManager)
		{
			this.AddVlandiaParentMenuOptions(characterCreationManager);
			this.AddSturgiaParentMenuOptions(characterCreationManager);
			this.AddAseraiParentMenuOptions(characterCreationManager);
			this.AddBattaniaParentMenuOptions(characterCreationManager);
			this.AddKhuzaitParentMenuOptions(characterCreationManager);
			this.AddEmpireParentMenuOptions(characterCreationManager);
			this.AddNordParentMenuOptions(characterCreationManager);
			this.AddEarlyChildhoodMenuOptions(characterCreationManager);
			this.AddEducationMenuOptions(characterCreationManager);
			this.AddYouthMenuOptions(characterCreationManager);
		}

		// Token: 0x0600170A RID: 5898 RVA: 0x0009CBE8 File Offset: 0x0009ADE8
		private void AddVlandiaParentMenuOptions(CharacterCreationManager characterCreationManager)
		{
			NarrativeMenu narrativeMenuWithId = characterCreationManager.GetNarrativeMenuWithId("narrative_parent_menu");
			NarrativeMenuOption narrativeMenuOption = new NarrativeMenuOption("vlandia_coastal_fisherman_option", new TextObject("{=MPaZbhRc}Coastal fisherman", null), new TextObject("{=VBy8WxVw}Your family has been fishing these waters for generations, struggling to make a living off the unpredictable sea. You grew up mending nets, hauling in catches, and dreaming of a life beyond the constant struggle for survival.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetVlandiaCoastalFishermanNarrativeOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.VlandiaCoastalFishermanNarrativeOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.VlandiaCoastalFishermanNarrativeOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption);
			NarrativeMenuOption narrativeMenuOption2 = new NarrativeMenuOption("vlandia_dockers_option", new TextObject("{=rsUCF3H8}Dockers", null), new TextObject("{=OyIKF2r6}Your family toiled on the docks, their hands calloused from hauling the endless flow of goods from the sea. A vital but often thankless task that kept Vlandia's ports alive. You learned the rhythm of the tides and the languages of foreign sailors before you learned to read.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetVlandiaDockersNarrativeOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.VlandiaDockersNarrativeOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.GetVlandiaDockersNarrativeOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption2);
		}

		// Token: 0x0600170B RID: 5899 RVA: 0x0009CC9C File Offset: 0x0009AE9C
		private void GetVlandiaCoastalFishermanNarrativeOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				NavalSkills.Boatswain,
				DefaultSkills.Scouting
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Endurance, this._attributeLevelToAdd);
		}

		// Token: 0x0600170C RID: 5900 RVA: 0x0009CCF0 File Offset: 0x0009AEF0
		private bool VlandiaCoastalFishermanNarrativeOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "vlandia";
		}

		// Token: 0x0600170D RID: 5901 RVA: 0x0009CD0C File Offset: 0x0009AF0C
		private void VlandiaCoastalFishermanNarrativeOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SetParentOccupation("seafarer");
			string motherEquipmentId = this.GetMotherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			string fatherEquipmentId = this.GetFatherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			MBEquipmentRoster @object = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(motherEquipmentId);
			MBEquipmentRoster object2 = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(fatherEquipmentId);
			string text = "act_character_creation_vlandia_fisherman_mother";
			string text2 = "act_character_creation_vlandia_fisherman_father";
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "mother_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_vlandia_fisherman_mother");
					narrativeMenuCharacter.SetLeftHandItem("fishnet_char_creation");
				}
				if (narrativeMenuCharacter.StringId == "father_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_vlandia_fisherman_father");
					narrativeMenuCharacter.SetRightHandItem("fishing_rod_s");
				}
			}
			this.UpdateParentEquipment(characterCreationManager, @object, object2, text, text2);
		}

		// Token: 0x0600170E RID: 5902 RVA: 0x0009CE48 File Offset: 0x0009B048
		private void GetVlandiaDockersNarrativeOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				NavalSkills.Shipmaster,
				DefaultSkills.Athletics
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Vigor, this._attributeLevelToAdd);
		}

		// Token: 0x0600170F RID: 5903 RVA: 0x0009CE9C File Offset: 0x0009B09C
		private bool VlandiaDockersNarrativeOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "vlandia";
		}

		// Token: 0x06001710 RID: 5904 RVA: 0x0009CEB8 File Offset: 0x0009B0B8
		private void GetVlandiaDockersNarrativeOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SetParentOccupation("shipmaster_urban");
			string motherEquipmentId = this.GetMotherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			string fatherEquipmentId = this.GetFatherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			MBEquipmentRoster @object = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(motherEquipmentId);
			MBEquipmentRoster object2 = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(fatherEquipmentId);
			string text = "act_character_creation_vlandia_dockers_mother";
			string text2 = "act_character_creation_vlandia_dockers_father";
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "mother_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_vlandia_dockers_mother");
					narrativeMenuCharacter.SetRightHandItem("sack_s");
				}
				if (narrativeMenuCharacter.StringId == "father_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_vlandia_dockers_father");
					narrativeMenuCharacter.SetRightHandItem("sack");
				}
			}
			this.UpdateParentEquipment(characterCreationManager, @object, object2, text, text2);
		}

		// Token: 0x06001711 RID: 5905 RVA: 0x0009CFF4 File Offset: 0x0009B1F4
		private void AddSturgiaParentMenuOptions(CharacterCreationManager characterCreationManager)
		{
			NarrativeMenu narrativeMenuWithId = characterCreationManager.GetNarrativeMenuWithId("narrative_parent_menu");
			NarrativeMenuOption narrativeMenuOption = new NarrativeMenuOption("sturgia_river_fisherman_option", new TextObject("{=iuAi8rZ4}River Fisherman", null), new TextObject("{=gpNBMzW8}Your family lived by the water, skilled in casting nets, setting lines, and mending the wear and tear of daily fishing. You understood the currents, the seasons of the fish, and the importance of a good catch for your community. Life was dictated by the river's flow and its bounty.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetSturgiaRiverFishermanNarrativeOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.SturgiaRiverFishermanNarrativeOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.GetSturgiaRiverFishermanNarrativeOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption);
			NarrativeMenuOption narrativeMenuOption2 = new NarrativeMenuOption("sturgia_shipbuilders_option", new TextObject("{=V0GSUvaU}Shipbuilders", null), new TextObject("{=9XmQrI23}Your family builded longships for the Sturgian river lords. You grew up amidst the sounds of hammering and the smell of tar, learning the craft of shipbuilding from your father and uncles.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetSturgiaShipbuildersNarrativeOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.SturgiaShipbuildersNarrativeOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.GetSturgiaShipbuildersNarrativeOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption2);
		}

		// Token: 0x06001712 RID: 5906 RVA: 0x0009D0A8 File Offset: 0x0009B2A8
		private void GetSturgiaRiverFishermanNarrativeOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				NavalSkills.Boatswain,
				DefaultSkills.Throwing
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Control, this._attributeLevelToAdd);
		}

		// Token: 0x06001713 RID: 5907 RVA: 0x0009D0FC File Offset: 0x0009B2FC
		private bool SturgiaRiverFishermanNarrativeOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "sturgia";
		}

		// Token: 0x06001714 RID: 5908 RVA: 0x0009D118 File Offset: 0x0009B318
		private void GetSturgiaRiverFishermanNarrativeOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SetParentOccupation("seafarer");
			string motherEquipmentId = this.GetMotherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			string fatherEquipmentId = this.GetFatherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			MBEquipmentRoster @object = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(motherEquipmentId);
			MBEquipmentRoster object2 = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(fatherEquipmentId);
			string text = "act_character_creation_sturgia_riverfisherman_mother";
			string text2 = "act_character_creation_sturgia_riverfisherman_father";
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "mother_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_sturgia_riverfisherman_mother");
				}
				if (narrativeMenuCharacter.StringId == "father_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_sturgia_riverfisherman_father");
					narrativeMenuCharacter.SetLeftHandItem("fishnet");
				}
			}
			this.UpdateParentEquipment(characterCreationManager, @object, object2, text, text2);
		}

		// Token: 0x06001715 RID: 5909 RVA: 0x0009D248 File Offset: 0x0009B448
		private void GetSturgiaShipbuildersNarrativeOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				NavalSkills.Shipmaster,
				DefaultSkills.Engineering
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Intelligence, this._attributeLevelToAdd);
		}

		// Token: 0x06001716 RID: 5910 RVA: 0x0009D29C File Offset: 0x0009B49C
		private bool SturgiaShipbuildersNarrativeOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "sturgia";
		}

		// Token: 0x06001717 RID: 5911 RVA: 0x0009D2B8 File Offset: 0x0009B4B8
		private void GetSturgiaShipbuildersNarrativeOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SetParentOccupation("shipmaster_urban");
			string motherEquipmentId = this.GetMotherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			string fatherEquipmentId = this.GetFatherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			MBEquipmentRoster @object = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(motherEquipmentId);
			MBEquipmentRoster object2 = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(fatherEquipmentId);
			string text = "act_character_creation_sturgia_shipbuilder_mother";
			string text2 = "act_character_creation_sturgia_shipbuilder_father";
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "mother_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_sturgia_shipbuilder_mother");
				}
				if (narrativeMenuCharacter.StringId == "father_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_sturgia_shipbuilder_father");
					narrativeMenuCharacter.SetLeftHandItem("blacksmith_hammer");
				}
			}
			this.UpdateParentEquipment(characterCreationManager, @object, object2, text, text2);
		}

		// Token: 0x06001718 RID: 5912 RVA: 0x0009D3E8 File Offset: 0x0009B5E8
		private void AddAseraiParentMenuOptions(CharacterCreationManager characterCreationManager)
		{
			NarrativeMenu narrativeMenuWithId = characterCreationManager.GetNarrativeMenuWithId("narrative_parent_menu");
			NarrativeMenuOption narrativeMenuOption = new NarrativeMenuOption("aserai_ferryman_option", new TextObject("{=PaXaNLrb}Ferryman", null), new TextObject("{=LtOCnEC8}Your family are from a small rural community along a river bank where they operated a small ferry to transport goods and people across the river, connecting rural communities. You learned about boats and ebbs and flows of the river navigation.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetAseraiFerrymanNarrativeOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.AseraiFerrymanNarrativeOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.GetAseraiFerrymanNarrativeOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption);
			NarrativeMenuOption narrativeMenuOption2 = new NarrativeMenuOption("aserai_corsair_traders_option", new TextObject("{=V0IGaFFn}Corsair Traders", null), new TextObject("{=Gl5CFpEM}Raised on Aserai dhows, your father thought you about the trade winds and routes. The ship you were raised in made long and tedious voyages, smuggling silks and spices or ambushing Vlandian ships when profits dwindled.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetAseraiCorsairTradersNarrativeOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.AseraiCorsairTradersNarrativeOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.GetAseraiCorsairTradersNarrativeOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption2);
		}

		// Token: 0x06001719 RID: 5913 RVA: 0x0009D49C File Offset: 0x0009B69C
		private void GetAseraiFerrymanNarrativeOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				NavalSkills.Boatswain,
				DefaultSkills.Trade
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Cunning, this._attributeLevelToAdd);
		}

		// Token: 0x0600171A RID: 5914 RVA: 0x0009D4F0 File Offset: 0x0009B6F0
		private bool AseraiFerrymanNarrativeOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "aserai";
		}

		// Token: 0x0600171B RID: 5915 RVA: 0x0009D50C File Offset: 0x0009B70C
		private void GetAseraiFerrymanNarrativeOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SetParentOccupation("seafarer");
			string motherEquipmentId = this.GetMotherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			string fatherEquipmentId = this.GetFatherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			MBEquipmentRoster @object = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(motherEquipmentId);
			MBEquipmentRoster object2 = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(fatherEquipmentId);
			string text = "act_character_creation_aserai_ferryman_mother";
			string text2 = "act_character_creation_aserai_ferryman_father";
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "mother_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_aserai_ferryman_mother");
				}
				if (narrativeMenuCharacter.StringId == "father_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_aserai_ferryman_father");
					narrativeMenuCharacter.SetRightHandItem("shovel_right_hand");
				}
			}
			foreach (NarrativeMenuCharacter narrativeMenuCharacter2 in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter2.StringId == "mother_character")
				{
					narrativeMenuCharacter2.SetAnimationId("act_character_creation_aserai_ferryman_mother");
				}
				if (narrativeMenuCharacter2.StringId == "father_character")
				{
					narrativeMenuCharacter2.SetAnimationId("act_character_creation_aserai_ferryman_father");
					narrativeMenuCharacter2.SetRightHandItem("shovel_right_hand");
				}
			}
			this.UpdateParentEquipment(characterCreationManager, @object, object2, text, text2);
		}

		// Token: 0x0600171C RID: 5916 RVA: 0x0009D6C8 File Offset: 0x0009B8C8
		private void GetAseraiCorsairTradersNarrativeOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				NavalSkills.Boatswain,
				NavalSkills.Mariner
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Vigor, this._attributeLevelToAdd);
		}

		// Token: 0x0600171D RID: 5917 RVA: 0x0009D71C File Offset: 0x0009B91C
		private bool AseraiCorsairTradersNarrativeOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "aserai";
		}

		// Token: 0x0600171E RID: 5918 RVA: 0x0009D738 File Offset: 0x0009B938
		private void GetAseraiCorsairTradersNarrativeOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SetParentOccupation("shipmaster_urban");
			string motherEquipmentId = this.GetMotherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			string fatherEquipmentId = this.GetFatherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			MBEquipmentRoster @object = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(motherEquipmentId);
			MBEquipmentRoster object2 = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(fatherEquipmentId);
			string text = "act_character_creation_aserai_corsair_trader_mother";
			string text2 = "act_character_creation_aserai_corsair_trader_father";
			this.UpdateParentEquipment(characterCreationManager, @object, object2, text, text2);
		}

		// Token: 0x0600171F RID: 5919 RVA: 0x0009D7D8 File Offset: 0x0009B9D8
		private void AddBattaniaParentMenuOptions(CharacterCreationManager characterCreationManager)
		{
			NarrativeMenu narrativeMenuWithId = characterCreationManager.GetNarrativeMenuWithId("narrative_parent_menu");
			NarrativeMenuOption narrativeMenuOption = new NarrativeMenuOption("battania_currach_sailors_option", new TextObject("{=4zNU0J1S}Currach Sailors", null), new TextObject("{=bnrmJHc6}Your kin braved the lakes and rivers in hide-covered currachs, fishing icy waters and facing the dangers of strong currents and occasional banditry. You grew up learning to navigate the treacherous waters and to defend yourself from those who would prey on the river traffic.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetBattaniaCurrachSailorsNarrativeOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.BattaniaCurrachSailorsNarrativeOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.GetBattaniaCurrachSailorsNarrativeOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption);
			NarrativeMenuOption narrativeMenuOption2 = new NarrativeMenuOption("battania_guardian_of_the_lake_option", new TextObject("{=o7BFw2WW}Guardian of the Lake", null), new TextObject("{=ydyaMa6E}Your kin were part of a group of warriors tasked with maintaining small boats for defense or patrol of vital waterways, protecting it from raiders or invaders. While they weren't around much while you were growing up, you still earned some riverine navigation and combat skills.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetBattaniaGuardianOfTheLakeNarrativeOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.BattaniaGuardianOfTheLakeNarrativeOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.GetBattaniaGuardianOfTheLakeNarrativeOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption2);
		}

		// Token: 0x06001720 RID: 5920 RVA: 0x0009D88C File Offset: 0x0009BA8C
		private void GetBattaniaCurrachSailorsNarrativeOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				NavalSkills.Boatswain,
				DefaultSkills.Bow
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Control, this._attributeLevelToAdd);
		}

		// Token: 0x06001721 RID: 5921 RVA: 0x0009D8E0 File Offset: 0x0009BAE0
		private bool BattaniaCurrachSailorsNarrativeOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "battania";
		}

		// Token: 0x06001722 RID: 5922 RVA: 0x0009D8FC File Offset: 0x0009BAFC
		private void GetBattaniaCurrachSailorsNarrativeOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SetParentOccupation("seafarer");
			string motherEquipmentId = this.GetMotherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			string fatherEquipmentId = this.GetFatherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			MBEquipmentRoster @object = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(motherEquipmentId);
			MBEquipmentRoster object2 = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(fatherEquipmentId);
			string text = "act_character_creation_battania_currach_sailors_mother";
			string text2 = "act_character_creation_battania_currach_sailors_father";
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "mother_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_battania_currach_sailors_mother");
					narrativeMenuCharacter.SetLeftHandItem("bow");
				}
				if (narrativeMenuCharacter.StringId == "father_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_battania_currach_sailors_father");
					narrativeMenuCharacter.SetRightHandItem("battle_axe");
				}
			}
			this.UpdateParentEquipment(characterCreationManager, @object, object2, text, text2);
		}

		// Token: 0x06001723 RID: 5923 RVA: 0x0009DA38 File Offset: 0x0009BC38
		private void GetBattaniaGuardianOfTheLakeNarrativeOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				NavalSkills.Mariner,
				DefaultSkills.Polearm
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Vigor, this._attributeLevelToAdd);
		}

		// Token: 0x06001724 RID: 5924 RVA: 0x0009DA8C File Offset: 0x0009BC8C
		private bool BattaniaGuardianOfTheLakeNarrativeOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "battania";
		}

		// Token: 0x06001725 RID: 5925 RVA: 0x0009DAA8 File Offset: 0x0009BCA8
		private void GetBattaniaGuardianOfTheLakeNarrativeOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SetParentOccupation("shipmaster_urban");
			string motherEquipmentId = this.GetMotherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			string fatherEquipmentId = this.GetFatherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			MBEquipmentRoster @object = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(motherEquipmentId);
			MBEquipmentRoster object2 = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(fatherEquipmentId);
			string text = "act_character_creation_battania_guardian_of_the_lake_mother";
			string text2 = "act_character_creation_battania_guardian_of_the_lake_father";
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "mother_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_battania_guardian_of_the_lake_mother");
					narrativeMenuCharacter.SetRightHandItem("javelin_a");
				}
				if (narrativeMenuCharacter.StringId == "father_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_battania_guardian_of_the_lake_father");
					narrativeMenuCharacter.SetLeftHandItem("heater_shield");
					narrativeMenuCharacter.SetRightHandItem("blacksmith_sword");
				}
			}
			this.UpdateParentEquipment(characterCreationManager, @object, object2, text, text2);
		}

		// Token: 0x06001726 RID: 5926 RVA: 0x0009DBF0 File Offset: 0x0009BDF0
		private void AddKhuzaitParentMenuOptions(CharacterCreationManager characterCreationManager)
		{
			NarrativeMenu narrativeMenuWithId = characterCreationManager.GetNarrativeMenuWithId("narrative_parent_menu");
			NarrativeMenuOption narrativeMenuOption = new NarrativeMenuOption("khuzait_river_foragers_option", new TextObject("{=fEIzJtSF}River Foragers", null), new TextObject("{=2rNqqZnm}Along the winding veins of a major river that cuts through the steppe, your family carved a life from the water's edge. Using small, makeshift rafts and boats, they developed a keen eye for the river's bounty, gathering specific plants from its banks and fishing in its shallows. From your humble parents, you inherited a deep well of knowledge.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetKhuzaitRiverForagersNarrativeOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.KhuzaitRiverForagersNarrativeOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.GetKhuzaitRiverForagersNarrativeOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption);
			NarrativeMenuOption narrativeMenuOption2 = new NarrativeMenuOption("khuzait_river_traders_option", new TextObject("{=DQQogYtq}River Traders", null), new TextObject("{=enS8isiB}Your family transports goods and people along the river, facing the dangers of strong currents and occasional banditry. You grew up learning to navigate the treacherous waters and to defend yourself from those who would prey on the river traffic.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetKhuzaitRiverTradersNarrativeOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.KhuzaitRiverTradersNarrativeOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.GetKhuzaitRiverTradersNarrativeOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption2);
		}

		// Token: 0x06001727 RID: 5927 RVA: 0x0009DCA4 File Offset: 0x0009BEA4
		private void GetKhuzaitRiverForagersNarrativeOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				NavalSkills.Boatswain,
				NavalSkills.Shipmaster
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Endurance, this._attributeLevelToAdd);
		}

		// Token: 0x06001728 RID: 5928 RVA: 0x0009DCF8 File Offset: 0x0009BEF8
		private bool KhuzaitRiverForagersNarrativeOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "khuzait";
		}

		// Token: 0x06001729 RID: 5929 RVA: 0x0009DD14 File Offset: 0x0009BF14
		private void GetKhuzaitRiverForagersNarrativeOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SetParentOccupation("seafarer");
			string motherEquipmentId = this.GetMotherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			string fatherEquipmentId = this.GetFatherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			MBEquipmentRoster @object = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(motherEquipmentId);
			MBEquipmentRoster object2 = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(fatherEquipmentId);
			string text = "act_character_creation_khuzait_river_foragers_mother";
			string text2 = "act_character_creation_khuzait_river_foragers_father";
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "mother_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_khuzait_river_foragers_mother");
				}
				if (narrativeMenuCharacter.StringId == "father_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_khuzait_river_foragers_father");
					narrativeMenuCharacter.SetLeftHandItem("fish_stick");
				}
			}
			this.UpdateParentEquipment(characterCreationManager, @object, object2, text, text2);
		}

		// Token: 0x0600172A RID: 5930 RVA: 0x0009DE44 File Offset: 0x0009C044
		private void GetKhuzaitRiverTradersNarrativeOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				DefaultSkills.Bow,
				NavalSkills.Mariner
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Cunning, this._attributeLevelToAdd);
		}

		// Token: 0x0600172B RID: 5931 RVA: 0x0009DE98 File Offset: 0x0009C098
		private bool KhuzaitRiverTradersNarrativeOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "khuzait";
		}

		// Token: 0x0600172C RID: 5932 RVA: 0x0009DEB4 File Offset: 0x0009C0B4
		private void GetKhuzaitRiverTradersNarrativeOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SetParentOccupation("shipmaster_urban");
			string motherEquipmentId = this.GetMotherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			string fatherEquipmentId = this.GetFatherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			MBEquipmentRoster @object = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(motherEquipmentId);
			MBEquipmentRoster object2 = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(fatherEquipmentId);
			string text = "act_character_creation_khuzait_river_traders_mother";
			string text2 = "act_character_creation_khuzait_river_traders_father";
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "mother_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_khuzait_river_foragers_mother");
				}
				if (narrativeMenuCharacter.StringId == "father_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_khuzait_river_foragers_father");
					narrativeMenuCharacter.SetLeftHandItem("stick");
				}
			}
			this.UpdateParentEquipment(characterCreationManager, @object, object2, text, text2);
		}

		// Token: 0x0600172D RID: 5933 RVA: 0x0009DFE4 File Offset: 0x0009C1E4
		private void AddEmpireParentMenuOptions(CharacterCreationManager characterCreationManager)
		{
			NarrativeMenu narrativeMenuWithId = characterCreationManager.GetNarrativeMenuWithId("narrative_parent_menu");
			NarrativeMenuOption narrativeMenuOption = new NarrativeMenuOption("empire_small_boat_fisherman_option", new TextObject("{=e1aebAAL}Small Boat Fisherman", null), new TextObject("{=nBr0jL3X}Your family inhabited a small, relatively isolated coastal village within the Empire. They foraged along the shoreline for fish using small boats. You grew up with the smell of salt and the rhythm of the tides, learning to navigate close to shore and brave the smaller waves in your sturdy little vessel.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetEmpireSmallBoatFishermanNarrativeOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.EmpireSmallBoatFishermanNarrativeOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.GetEmpireSmallBoatFishermanNarrativeOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption);
			NarrativeMenuOption narrativeMenuOption2 = new NarrativeMenuOption("empire_imperial_fleet_option", new TextObject("{=LdCQfaUi}Imperial Fleet", null), new TextObject("{=N6o7Gnpz}Your father served in one the Imperial Navy's liburna as a quartermaster. He bought supplies for the crew and basically kept the ship running. He wanted the same path for you so you were schooled in trading and ship maintenance.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetEmpireImperialFleetNarrativeOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.EmpireImperialFleetNarrativeOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.GetEmpireImperialFleetNarrativeOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption2);
		}

		// Token: 0x0600172E RID: 5934 RVA: 0x0009E098 File Offset: 0x0009C298
		private void GetEmpireSmallBoatFishermanNarrativeOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				NavalSkills.Boatswain,
				DefaultSkills.Throwing
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Control, this._attributeLevelToAdd);
		}

		// Token: 0x0600172F RID: 5935 RVA: 0x0009E0EC File Offset: 0x0009C2EC
		private bool EmpireSmallBoatFishermanNarrativeOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "empire";
		}

		// Token: 0x06001730 RID: 5936 RVA: 0x0009E108 File Offset: 0x0009C308
		private void GetEmpireSmallBoatFishermanNarrativeOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SetParentOccupation("seafarer");
			string motherEquipmentId = this.GetMotherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			string fatherEquipmentId = this.GetFatherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			MBEquipmentRoster @object = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(motherEquipmentId);
			MBEquipmentRoster object2 = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(fatherEquipmentId);
			string text = "act_character_creation_empire_smallboatfisherman_mother";
			string text2 = "act_character_creation_empire_smallboatfisherman_father";
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "mother_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_empire_smallboatfisherman_mother");
				}
				if (narrativeMenuCharacter.StringId == "father_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_empire_smallboatfisherman_father");
					narrativeMenuCharacter.SetLeftHandItem("hanging_fishes");
					narrativeMenuCharacter.SetRightHandItem("hanging_fishes");
				}
			}
			this.UpdateParentEquipment(characterCreationManager, @object, object2, text, text2);
		}

		// Token: 0x06001731 RID: 5937 RVA: 0x0009E244 File Offset: 0x0009C444
		private void GetEmpireImperialFleetNarrativeOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				DefaultSkills.Trade,
				NavalSkills.Shipmaster
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Social, this._attributeLevelToAdd);
		}

		// Token: 0x06001732 RID: 5938 RVA: 0x0009E298 File Offset: 0x0009C498
		private bool EmpireImperialFleetNarrativeOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "empire";
		}

		// Token: 0x06001733 RID: 5939 RVA: 0x0009E2B4 File Offset: 0x0009C4B4
		private void GetEmpireImperialFleetNarrativeOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SetParentOccupation("shipmaster_urban");
			string motherEquipmentId = this.GetMotherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			string fatherEquipmentId = this.GetFatherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			MBEquipmentRoster @object = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(motherEquipmentId);
			MBEquipmentRoster object2 = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(fatherEquipmentId);
			string text = "act_character_creation_empire_imperial_fleet_mother";
			string text2 = "act_character_creation_empire_imperial_fleet_father";
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "mother_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_empire_imperial_fleet_mother");
				}
				if (narrativeMenuCharacter.StringId == "father_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_empire_imperial_fleet_father");
					narrativeMenuCharacter.SetRightHandItem("book_right_hand");
				}
			}
			this.UpdateParentEquipment(characterCreationManager, @object, object2, text, text2);
		}

		// Token: 0x06001734 RID: 5940 RVA: 0x0009E3E4 File Offset: 0x0009C5E4
		private void AddNordParentMenuOptions(CharacterCreationManager characterCreationManager)
		{
			NarrativeMenu narrativeMenuWithId = characterCreationManager.GetNarrativeMenuWithId("narrative_parent_menu");
			NarrativeMenuOption narrativeMenuOption = new NarrativeMenuOption("nord_hersir_option", new TextObject("{=DRC5bTE5}Hersir", null), new TextObject("{=w3AI4lwM}Your family's loyalty ran deep, not in sprawling lands or grand titles, but in service. For generations, they'd served as hersirs, the trusted retainers, for a minor Jarl who kept watch over a windswept corner of the Nord territory.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetNordHersirNarrativeOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.NordHersirNarrativeOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.NordHersirNarrativeOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption);
			NarrativeMenuOption narrativeMenuOption2 = new NarrativeMenuOption("nord_market_trader_option", new TextObject("{=uqpHfuZV}Peddler", null), new TextObject("{=DvgmjoCE}You grew up amidst the bustling chaos of a Norse market town, a hub of trade where goods from across the known world exchanged hands. Your family were established traders, perhaps dealing in furs, amber, crafted goods, or even imported luxuries. You learned the art of negotiation, the value of different commodities, and the diverse languages and customs of the merchants who passed through. The market was your school, and shrewd dealing your lesson.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetNordMarketTraderNarrativeOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.NordMarketTraderNarrativeOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.NordMarketTraderNarrativeOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption2);
			NarrativeMenuOption narrativeMenuOption3 = new NarrativeMenuOption("nord_skald_option", new TextObject("{=1lX8eks5}Travelling skalds", null), new TextObject("{=KtucaHqd}Your family's voices carried the tales of the North. Not grand courtly Skalds, but traveling storytellers with weathered cloaks and worn lutes. They wandered from village to village, weaving tales of heroes and hearth into songs and sagas. You grew up surrounded by the rhythmic strum of their instruments and the flickering firelight reflecting off their eyes as they spun fantastical yarns. These weren't just stories - they were the beating heart of Nord culture, passed down from generation to generation by your family's calloused hands and booming voices.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetNordSkaldNarrativeOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.NordSkaldNarrativeOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.NordSkaldNarrativeOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption3);
			NarrativeMenuOption narrativeMenuOption4 = new NarrativeMenuOption("nord_blacksmith_option", new TextObject("{=v48N6h1t}Urban artisans", null), new TextObject("{=AAHhp1ly}The clang of hammer on hot iron was the defining sound of your upbringing. Your family were more than mere smiths; they were artisans who coaxed wonders from limited resources, shaping valuable iron into formidable weapons and treasured tools. From the forge, you learned to work with what little you had, understanding the unique properties of each piece and the almost magical skill required to transform it.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetNordBlacksmithNarrativeOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.NordBlacksmithNarrativeOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.NordBlacksmithNarrativeOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption4);
			NarrativeMenuOption narrativeMenuOption5 = new NarrativeMenuOption("nord_hunter_option", new TextObject("{=izTHRXo5}Hunters", null), new TextObject("{=rdRamFhv}You were born into a family of foresters living off the land. You learned to track prey, hunt for sustenance and gathering herbs and mushrooms from a young age. The forest provided, but it also demanded respect. You learned the medicinal properties of plants and mushrooms for the inevitable scrapes and ailments that came with life in the wild. The harsh environment became your teacher, and survival your greatest lesson.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetNordHunterNarrativeOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.NordHunterNarrativeOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.NordHunterNarrativeOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption5);
			NarrativeMenuOption narrativeMenuOption6 = new NarrativeMenuOption("nord_vagabonds_option", new TextObject("{=TPoK3GSj}Vagabonds", null), new TextObject("{=nrtrMbLx}You were part of a tight-knit family scraping by on the fringes of a bustling Nord port. Hard work wasn't always an option, and your kin did what they had to - unloading ships one day, \"borrowing\" a stray coin the next. Life was rough, lessons learned on cobblestone streets, but the fierce loyalty that bound your family together was stronger than any harbor wall.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetNordVagabondNarrativeOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.NordVagabondNarrativeOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.NordVagabondNarrativeOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption6);
			NarrativeMenuOption narrativeMenuOption7 = new NarrativeMenuOption("nord_sailors_option", new TextObject("{=6aKaV4ua}Sailors", null), new TextObject("{=BbOM3F8H}Your family was a tight-knit crew on a sturdy fishing vessel. They weren't charting uncharted seas, but venturing just beyond the familiar fjords, bartering with coastal settlements for smoked fish and bragging rights about the biggest catch. Tales of faraway lands might have been spun under flickering lanterns, but the reality was weathered sails, calloused hands, and a knack for reading the temperamental sea.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetNordSailorsNarrativeOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.NordSailorsNarrativeOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.GetNordSailorsNarrativeOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption7);
			NarrativeMenuOption narrativeMenuOption8 = new NarrativeMenuOption("nord_shipwrights_option", new TextObject("{=WYS68dRq}Shipwrights", null), new TextObject("{=qUwVnncn}Your kin weren't grand shipwrights building mighty drakkars, but a family of skilled boatbuilders crafting sturdy vessels. Their longships weren't feared in battle, but prized for braving the treacherous coasts. Each plank and sail held the legacy of generations, passed down through calloused hands and the rhythmic tap of the hammer.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetNordShipwrightsNarrativeOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.NordShipwrightsNarrativeOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.GetNordShipwrightsNarrativeOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption8);
		}

		// Token: 0x06001735 RID: 5941 RVA: 0x0009E66C File Offset: 0x0009C86C
		private void GetNordHersirNarrativeOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				DefaultSkills.Steward,
				DefaultSkills.OneHanded
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Vigor, this._attributeLevelToAdd);
		}

		// Token: 0x06001736 RID: 5942 RVA: 0x0009E6C0 File Offset: 0x0009C8C0
		private bool NordHersirNarrativeOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "nord";
		}

		// Token: 0x06001737 RID: 5943 RVA: 0x0009E6DC File Offset: 0x0009C8DC
		private void NordHersirNarrativeOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SetParentOccupation("retainer");
			string motherEquipmentId = this.GetMotherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			string fatherEquipmentId = this.GetFatherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			MBEquipmentRoster @object = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(motherEquipmentId);
			MBEquipmentRoster object2 = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(fatherEquipmentId);
			string text = "act_character_creation_female_default_side_to_side_1";
			string text2 = "act_character_creation_male_default_side_to_side_1";
			this.UpdateParentEquipment(characterCreationManager, @object, object2, text, text2);
		}

		// Token: 0x06001738 RID: 5944 RVA: 0x0009E77C File Offset: 0x0009C97C
		private void GetNordMarketTraderNarrativeOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				DefaultSkills.Trade,
				DefaultSkills.Charm
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Social, this._attributeLevelToAdd);
		}

		// Token: 0x06001739 RID: 5945 RVA: 0x0009E7D0 File Offset: 0x0009C9D0
		private bool NordMarketTraderNarrativeOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "nord";
		}

		// Token: 0x0600173A RID: 5946 RVA: 0x0009E7EC File Offset: 0x0009C9EC
		private void NordMarketTraderNarrativeOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SetParentOccupation("merchant_urban");
			string motherEquipmentId = this.GetMotherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			string fatherEquipmentId = this.GetFatherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			MBEquipmentRoster @object = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(motherEquipmentId);
			MBEquipmentRoster object2 = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(fatherEquipmentId);
			string text = "act_character_creation_female_default_side_to_side_2";
			string text2 = "act_character_creation_male_default_side_to_side_2";
			this.UpdateParentEquipment(characterCreationManager, @object, object2, text, text2);
		}

		// Token: 0x0600173B RID: 5947 RVA: 0x0009E88C File Offset: 0x0009CA8C
		private void GetNordSkaldNarrativeOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				DefaultSkills.Scouting,
				DefaultSkills.Charm
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Intelligence, this._attributeLevelToAdd);
		}

		// Token: 0x0600173C RID: 5948 RVA: 0x0009E8E0 File Offset: 0x0009CAE0
		private bool NordSkaldNarrativeOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "nord";
		}

		// Token: 0x0600173D RID: 5949 RVA: 0x0009E8FC File Offset: 0x0009CAFC
		private void NordSkaldNarrativeOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SetParentOccupation("bard");
			string motherEquipmentId = this.GetMotherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			string fatherEquipmentId = this.GetFatherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			MBEquipmentRoster @object = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(motherEquipmentId);
			MBEquipmentRoster object2 = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(fatherEquipmentId);
			string text = "act_character_creation_female_default_father_sitting";
			string text2 = "act_character_creation_male_default_father_sitting";
			this.UpdateParentEquipment(characterCreationManager, @object, object2, text, text2);
		}

		// Token: 0x0600173E RID: 5950 RVA: 0x0009E99C File Offset: 0x0009CB9C
		private void GetNordBlacksmithNarrativeOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				DefaultSkills.Crafting,
				DefaultSkills.Engineering
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Intelligence, this._attributeLevelToAdd);
		}

		// Token: 0x0600173F RID: 5951 RVA: 0x0009E9F0 File Offset: 0x0009CBF0
		private bool NordBlacksmithNarrativeOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "nord";
		}

		// Token: 0x06001740 RID: 5952 RVA: 0x0009EA0C File Offset: 0x0009CC0C
		private void NordBlacksmithNarrativeOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SetParentOccupation("artisan_urban");
			string motherEquipmentId = this.GetMotherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			string fatherEquipmentId = this.GetFatherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			MBEquipmentRoster @object = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(motherEquipmentId);
			MBEquipmentRoster object2 = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(fatherEquipmentId);
			string text = "act_character_creation_female_default_mother_front";
			string text2 = "act_character_creation_male_default_mother_front";
			this.UpdateParentEquipment(characterCreationManager, @object, object2, text, text2);
		}

		// Token: 0x06001741 RID: 5953 RVA: 0x0009EAAC File Offset: 0x0009CCAC
		private void GetNordHunterNarrativeOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				DefaultSkills.Bow,
				DefaultSkills.Medicine
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Control, this._attributeLevelToAdd);
		}

		// Token: 0x06001742 RID: 5954 RVA: 0x0009EB00 File Offset: 0x0009CD00
		private bool NordHunterNarrativeOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "nord";
		}

		// Token: 0x06001743 RID: 5955 RVA: 0x0009EB1C File Offset: 0x0009CD1C
		private void NordHunterNarrativeOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SetParentOccupation("hunter");
			string motherEquipmentId = this.GetMotherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			string fatherEquipmentId = this.GetFatherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			MBEquipmentRoster @object = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(motherEquipmentId);
			MBEquipmentRoster object2 = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(fatherEquipmentId);
			string text = "act_character_creation_female_default_side_to_side_3";
			string text2 = "act_character_creation_male_default_side_to_side_3";
			this.UpdateParentEquipment(characterCreationManager, @object, object2, text, text2);
		}

		// Token: 0x06001744 RID: 5956 RVA: 0x0009EBBC File Offset: 0x0009CDBC
		private void GetNordVagabondNarrativeOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				DefaultSkills.Throwing,
				DefaultSkills.Roguery
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Cunning, this._attributeLevelToAdd);
		}

		// Token: 0x06001745 RID: 5957 RVA: 0x0009EC10 File Offset: 0x0009CE10
		private bool NordVagabondNarrativeOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "nord";
		}

		// Token: 0x06001746 RID: 5958 RVA: 0x0009EC2C File Offset: 0x0009CE2C
		private void NordVagabondNarrativeOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SetParentOccupation("vagabond_urban");
			string motherEquipmentId = this.GetMotherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			string fatherEquipmentId = this.GetFatherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			MBEquipmentRoster @object = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(motherEquipmentId);
			MBEquipmentRoster object2 = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(fatherEquipmentId);
			string text = "act_character_creation_female_default_hugging";
			string text2 = "act_character_creation_male_default_hugging";
			this.UpdateParentEquipment(characterCreationManager, @object, object2, text, text2);
		}

		// Token: 0x06001747 RID: 5959 RVA: 0x0009ECCC File Offset: 0x0009CECC
		private void GetNordSailorsNarrativeOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				DefaultSkills.Trade,
				NavalSkills.Boatswain
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Social, this._attributeLevelToAdd);
		}

		// Token: 0x06001748 RID: 5960 RVA: 0x0009ED20 File Offset: 0x0009CF20
		private bool NordSailorsNarrativeOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "nord";
		}

		// Token: 0x06001749 RID: 5961 RVA: 0x0009ED3C File Offset: 0x0009CF3C
		private void GetNordSailorsNarrativeOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SetParentOccupation("seafarer");
			string motherEquipmentId = this.GetMotherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			string fatherEquipmentId = this.GetFatherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			MBEquipmentRoster @object = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(motherEquipmentId);
			MBEquipmentRoster object2 = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(fatherEquipmentId);
			string text = "act_character_creation_nord_sailors_mother";
			string text2 = "act_character_creation_nord_sailors_father";
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "mother_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_nord_sailors_mother");
					narrativeMenuCharacter.SetRightHandItem("fish_basket");
				}
				if (narrativeMenuCharacter.StringId == "father_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_nord_sailors_father");
					narrativeMenuCharacter.SetLeftHandItem("fish_left_hand");
				}
			}
			this.UpdateParentEquipment(characterCreationManager, @object, object2, text, text2);
		}

		// Token: 0x0600174A RID: 5962 RVA: 0x0009EE78 File Offset: 0x0009D078
		private void GetNordShipwrightsNarrativeOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				DefaultSkills.Engineering,
				NavalSkills.Shipmaster
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Endurance, this._attributeLevelToAdd);
		}

		// Token: 0x0600174B RID: 5963 RVA: 0x0009EECC File Offset: 0x0009D0CC
		private bool NordShipwrightsNarrativeOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "nord";
		}

		// Token: 0x0600174C RID: 5964 RVA: 0x0009EEE8 File Offset: 0x0009D0E8
		private void GetNordShipwrightsNarrativeOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SetParentOccupation("shipmaster_urban");
			string motherEquipmentId = this.GetMotherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			string fatherEquipmentId = this.GetFatherEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedParentOccupation, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId);
			MBEquipmentRoster @object = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(motherEquipmentId);
			MBEquipmentRoster object2 = Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(fatherEquipmentId);
			string text = "act_character_creation_nord_shipwrights_mother";
			string text2 = "act_character_creation_nord_shipwrights_father";
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "mother_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_nord_shipwrights_mother");
				}
				if (narrativeMenuCharacter.StringId == "father_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_nord_shipwrights_father");
					narrativeMenuCharacter.SetRightHandItem("blacksmith_hammer");
				}
			}
			this.UpdateParentEquipment(characterCreationManager, @object, object2, text, text2);
		}

		// Token: 0x0600174D RID: 5965 RVA: 0x0009F018 File Offset: 0x0009D218
		private void UpdateParentEquipment(CharacterCreationManager characterCreationManager, MBEquipmentRoster motherEquipment, MBEquipmentRoster fatherEquipment, string motherAnimation, string fatherAnimation)
		{
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId.Equals("mother_character"))
				{
					narrativeMenuCharacter.SetEquipment(motherEquipment);
					narrativeMenuCharacter.SetAnimationId(motherAnimation);
				}
				if (narrativeMenuCharacter.StringId.Equals("father_character"))
				{
					narrativeMenuCharacter.SetEquipment(fatherEquipment);
					narrativeMenuCharacter.SetAnimationId(fatherAnimation);
				}
			}
		}

		// Token: 0x0600174E RID: 5966 RVA: 0x0009F0AC File Offset: 0x0009D2AC
		private void AddEarlyChildhoodMenuOptions(CharacterCreationManager characterCreationManager)
		{
			NarrativeMenu narrativeMenuWithId = characterCreationManager.GetNarrativeMenuWithId("narrative_childhood_menu");
			NarrativeMenuOption narrativeMenuOption = new NarrativeMenuOption("childhood_predict_weather_option", new TextObject("{=cYIB0838}your uncanny ability to predict the weather.", null), new TextObject("{=w77I1ijB}You were fascinated with clouds and patterns and always observed weather, often warning your family of impending storms with uncanny accuracy.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetChildhoodPredictWeatherOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.ChildhoodPredictWeatherOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.ChildhoodPredictWeatherOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption);
		}

		// Token: 0x0600174F RID: 5967 RVA: 0x0009F110 File Offset: 0x0009D310
		private void GetChildhoodPredictWeatherOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				NavalSkills.Boatswain,
				DefaultSkills.Scouting
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Intelligence, this._attributeLevelToAdd);
		}

		// Token: 0x06001750 RID: 5968 RVA: 0x0009F164 File Offset: 0x0009D364
		private bool ChildhoodPredictWeatherOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return true;
		}

		// Token: 0x06001751 RID: 5969 RVA: 0x0009F168 File Offset: 0x0009D368
		private void ChildhoodPredictWeatherOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "player_childhood_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_childhood_memory");
				}
			}
		}

		// Token: 0x06001752 RID: 5970 RVA: 0x0009F1D8 File Offset: 0x0009D3D8
		private void AddEducationMenuOptions(CharacterCreationManager characterCreationManager)
		{
			NarrativeMenu narrativeMenuWithId = characterCreationManager.GetNarrativeMenuWithId("narrative_education_menu");
			NarrativeMenuOption narrativeMenuOption = new NarrativeMenuOption("education_fishing_boat", new TextObject("{=MHXeREoc}worked as a deckhand on a fishing boat.", null), new TextObject("{=3H4sk6zN}You spent your adolescence helping your uncle with his fishing business, learning the ropes (literally!) of seamanship, from mending nets to hauling in the catch.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetEducationFishingBoatOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.EducationFishingBoatOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.EducationFishingBoatOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption);
			NarrativeMenuOption narrativeMenuOption2 = new NarrativeMenuOption("education_docks", new TextObject("{=eTXb0QYP}worked at the docks.", null), new TextObject("{=EDwrct2r}You spent your adolescence helping out at the bustling docks, assisting with the loading and unloading of ships, and learning the ins and outs of maritime trade. You witnessed the arrival and departure of exotic goods and people from far-off lands, fueling your dreams of adventure on the high seas.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetEducationDocksOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.EducationDocksOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.EducationDocksOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption2);
		}

		// Token: 0x06001753 RID: 5971 RVA: 0x0009F28C File Offset: 0x0009D48C
		private void GetEducationFishingBoatOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				NavalSkills.Boatswain,
				DefaultSkills.Athletics
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Endurance, this._attributeLevelToAdd);
		}

		// Token: 0x06001754 RID: 5972 RVA: 0x0009F2E0 File Offset: 0x0009D4E0
		private bool EducationFishingBoatOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return !NavalCharacterCreationCampaignBehavior.NavalCharacterOccupationTypes.IsUrbanOccupation(characterCreationManager.CharacterCreationContent.SelectedParentOccupation);
		}

		// Token: 0x06001755 RID: 5973 RVA: 0x0009F2F8 File Offset: 0x0009D4F8
		private void EducationFishingBoatOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "player_education_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_childhood_athlete");
					break;
				}
			}
		}

		// Token: 0x06001756 RID: 5974 RVA: 0x0009F368 File Offset: 0x0009D568
		private void GetEducationDocksOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				NavalSkills.Shipmaster,
				DefaultSkills.Trade
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Social, this._attributeLevelToAdd);
		}

		// Token: 0x06001757 RID: 5975 RVA: 0x0009F3BC File Offset: 0x0009D5BC
		private bool EducationDocksOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return NavalCharacterCreationCampaignBehavior.NavalCharacterOccupationTypes.IsUrbanOccupation(characterCreationManager.CharacterCreationContent.SelectedParentOccupation);
		}

		// Token: 0x06001758 RID: 5976 RVA: 0x0009F3D0 File Offset: 0x0009D5D0
		private void EducationDocksOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "player_education_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_childhood_tough");
					break;
				}
			}
		}

		// Token: 0x06001759 RID: 5977 RVA: 0x0009F440 File Offset: 0x0009D640
		private void AddYouthMenuOptions(CharacterCreationManager characterCreationManager)
		{
			NarrativeMenu narrativeMenuWithId = characterCreationManager.GetNarrativeMenuWithId("narrative_youth_menu");
			NarrativeMenuOption narrativeMenuOption = new NarrativeMenuOption("youth_nord_guard_option", new TextObject("{=I23UbK4E}served as a shieldbearer to a huscarl.", null), new TextObject("{=Rffyscuk}War was a constant presence in your village. You served as a shieldbearer to a renowned Huscarl, a veteran Nord warrior. Witnessing countless battles and learning the art of defense from a master, you yearn to prove yourself worthy of wielding a weapon in the front lines.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetYouthNordGuardOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.YouthNordGuardOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.YouthNordGuardOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption);
			NarrativeMenuOption narrativeMenuOption2 = new NarrativeMenuOption("youth_nord_skirmisher_option", new TextObject("{=8c7mwLQQ}joined the raiders as a lookout.", null), new TextObject("{=6X2hZY6z}Growing up on the harsh Nordic coast, you were trained from a young age to spot enemy sails and signal incoming raids. Agile and quick-witted, you honed your skills with a throwing axe and learned to fight in skirmishes. You dream of joining a raiding party and tasting the glory of conquest.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetYouthNordSkirmisherOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.YouthNordSkirmisherOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.YouthNordSkirmisherOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption2);
			NarrativeMenuOption narrativeMenuOption3 = new NarrativeMenuOption("youth_nord_vagabond_option", new TextObject("{=T7B4KmHz}drafted to war as a thrall.", null), new TextObject("{=lilGmaCg}Thrown into servitude to a Jarl, war ripped through your village. Drafted alongside other thralls, you were thrown into battle with minimal training and a simple spear. Though fear grips you, a deep loyalty to your Jarl and a desperate will to survive drive you forward.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetYouthNordVagabondOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.YouthNordVagabondOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.YouthNordVagabondOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption3);
			NarrativeMenuOption narrativeMenuOption4 = new NarrativeMenuOption("youth_nord_artisan_option", new TextObject("{=qJweXkmJ}stood sentry at the Walls of the Hold.", null), new TextObject("{=XpiyI865}With enemy forces constantly threatening your village, you spent your youth helping fortify the local hold. You became skilled in basic construction, learned to use a pickaxe and shovel, and assisted in defending the walls during sieges. Now, you yearn to be part of the offensive and take the fight to the enemy.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetYouthNordArtisanOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.YouthNordArtisanOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.YouthNordArtisanOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption4);
			NarrativeMenuOption narrativeMenuOption5 = new NarrativeMenuOption("youth_nord_infantry_option", new TextObject("{=ZfaBIuFL}scavenged the battlefields for scraps.", null), new TextObject("{=unIV7bqB}Born into a Calradia perpetually at war, you didn't know playgrounds, you knew battlefields. Survival as a youngster meant picking through the battlefields. You learned to be self-sufficient, sometimes tending to wounds amidst the carnage. The law of the battlefield was simple: take what you can, and don't get caught.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetYouthNordInfantryOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.YouthNordInfantryOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.YouthNordInfantryOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption5);
			NarrativeMenuOption narrativeMenuOption6 = new NarrativeMenuOption("youth_nord_mercenary_option", new TextObject("{=On8SIR0J}became a warchild of the North.", null), new TextObject("{=La4V8zQn}War ravaged your village, leaving you orphaned and hardened by hardship. You scavenged for scraps, learning to fight for survival in the harsh wilderness. Now, driven by a thirst for vengeance and a desire to carve your own path, you seek to join a warband and prove your worth.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetYouthNordMercenaryOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.YouthNordMercenaryOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.YouthNordMercenaryOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption6);
			NarrativeMenuOption narrativeMenuOption7 = new NarrativeMenuOption("youth_crewed_a_galley_option", new TextObject("{=Hhkt6gtQ}crewed a galley in the coastal raids.", null), new TextObject("{=KWI2QOAO}You spent your youth participating in coastal raids, learning the skills of a rower, a lookout, and a boarding party. You witnessed the thrill of naval combat firsthand, experiencing the fear and the glory of maritime warfare.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetYouthCrewedAGalleyNarrativeOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.YouthCrewedAGalleyNarrativeOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.YouthCrewedAGalleyNarrativeOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption7);
			NarrativeMenuOption narrativeMenuOption8 = new NarrativeMenuOption("youth_rowed_river_trader_option", new TextObject("{=BRcMIDYK}rowed on a river trader.", null), new TextObject("{=urpdbYXl}You spent your youth helping your family transport goods along the river, learning to navigate the treacherous currents and to defend yourselves from raiders. You witnessed the bustling trade centers and encountered a diverse array of cultures.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetYouthRowedRiverTraderNarrativeOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.YouthRowedRiverTraderNarrativeOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.YouthRowedRiverTraderNarrativeOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption8);
			NarrativeMenuOption narrativeMenuOption9 = new NarrativeMenuOption("youth_deckhand_corsair_option", new TextObject("{=h0h4abww}served as a deckhand on a corsair.", null), new TextObject("{=LVxRFT5b}Growing up in a coastal town, you were drawn to the allure of the sea and the thrill of adventure. You joined a corsair crew as a deckhand, learning the ropes seamanship and witnessing the brutality of pirate raids firsthand.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetYouthDeckhandCorsairNarrativeOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.YouthDeckhandCorsairNarrativeOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.YouthDeckhandCorsairNarrativeOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption9);
			NarrativeMenuOption narrativeMenuOption10 = new NarrativeMenuOption("youth_raided_river_traffic_option", new TextObject("{=C04DgO2S}raided river traffic.", null), new TextObject("{=lHd0H3jg}You grew up along the great rivers, learning to navigate the treacherous currents and to fight from swift riverboats. You learned to raid rival clans and extort tribute from wealthy merchants, honing your skills as a river pirate.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetYouthRaidedRiverTrafficNarrativeOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.YouthRaidedRiverTrafficNarrativeOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.YouthRaidedRiverTrafficNarrativeOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption10);
			NarrativeMenuOption narrativeMenuOption11 = new NarrativeMenuOption("youth_coastal_defender_option", new TextObject("{=OMnJGBCR}served as a coastal defender.", null), new TextObject("{=gvp4AsMQ}You grew up amidst tales of legendary sea battles and legendary heroes, destined to carry on the proud traditions of your seafaring ancestors and defend your coastal towns just like them. You learned the skills of a mariner, a rower, and a warrior.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetYouthCoastalDefenderNarrativeOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.YouthCoastalDefenderNarrativeOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.YouthCoastalDefenderNarrativeOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption11);
			NarrativeMenuOption narrativeMenuOption12 = new NarrativeMenuOption("youth_serve_raider_ship_option", new TextObject("{=8GnOKv5r}went serving on a raider ship.", null), new TextObject("{=Xclr9fU3}You grew up in a coastal village, surrounded by tales of legendary Viking warriors and their daring voyages. As a youth, you learned the skills of a sailor, a warrior, and a raider, preparing for the day when you would join your kin on a voyage of exploration and conquest.", null), new GetNarrativeMenuOptionArgsDelegate(this.GetYouthServeRaiderShipNarrativeOptionArgs), new NarrativeMenuOptionOnConditionDelegate(this.YouthServeRaiderShipNarrativeOptionOnCondition), new NarrativeMenuOptionOnSelectDelegate(this.YouthServeRaiderShipNarrativeOptionOnSelect), null);
			narrativeMenuWithId.AddNarrativeMenuOption(narrativeMenuOption12);
		}

		// Token: 0x0600175A RID: 5978 RVA: 0x0009F804 File Offset: 0x0009DA04
		private void GetYouthNordGuardOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				DefaultSkills.Charm,
				DefaultSkills.Scouting
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Social, this._attributeLevelToAdd);
		}

		// Token: 0x0600175B RID: 5979 RVA: 0x0009F858 File Offset: 0x0009DA58
		private bool YouthNordGuardOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "nord";
		}

		// Token: 0x0600175C RID: 5980 RVA: 0x0009F874 File Offset: 0x0009DA74
		private void YouthNordGuardOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SelectedTitleType = "guard";
			string playerEquipmentId = this.GetPlayerEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedTitleType, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId, Hero.MainHero.IsFemale);
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "player_youth_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_nord_served_as_a_shieldbearer");
					narrativeMenuCharacter.SetEquipment(Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(playerEquipmentId));
				}
			}
		}

		// Token: 0x0600175D RID: 5981 RVA: 0x0009F938 File Offset: 0x0009DB38
		private void GetYouthNordSkirmisherOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				DefaultSkills.Throwing,
				DefaultSkills.Tactics
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Cunning, this._attributeLevelToAdd);
		}

		// Token: 0x0600175E RID: 5982 RVA: 0x0009F98C File Offset: 0x0009DB8C
		private bool YouthNordSkirmisherOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "nord";
		}

		// Token: 0x0600175F RID: 5983 RVA: 0x0009F9A8 File Offset: 0x0009DBA8
		private void YouthNordSkirmisherOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SelectedTitleType = "skirmisher";
			string playerEquipmentId = this.GetPlayerEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedTitleType, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId, Hero.MainHero.IsFemale);
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "player_youth_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_childhood_fox");
					narrativeMenuCharacter.SetEquipment(Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(playerEquipmentId));
				}
			}
		}

		// Token: 0x06001760 RID: 5984 RVA: 0x0009FA6C File Offset: 0x0009DC6C
		private void GetYouthNordVagabondOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				DefaultSkills.OneHanded,
				DefaultSkills.Polearm
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Endurance, this._attributeLevelToAdd);
		}

		// Token: 0x06001761 RID: 5985 RVA: 0x0009FAC0 File Offset: 0x0009DCC0
		private bool YouthNordVagabondOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "nord";
		}

		// Token: 0x06001762 RID: 5986 RVA: 0x0009FADC File Offset: 0x0009DCDC
		private void YouthNordVagabondOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SelectedTitleType = "vagabond";
			string playerEquipmentId = this.GetPlayerEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedTitleType, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId, Hero.MainHero.IsFemale);
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "player_youth_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_drafted_to_war_pose");
					narrativeMenuCharacter.SetEquipment(Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(playerEquipmentId));
				}
			}
		}

		// Token: 0x06001763 RID: 5987 RVA: 0x0009FBA0 File Offset: 0x0009DDA0
		private void GetYouthNordArtisanOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				DefaultSkills.Engineering,
				DefaultSkills.Bow
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Intelligence, this._attributeLevelToAdd);
		}

		// Token: 0x06001764 RID: 5988 RVA: 0x0009FBF4 File Offset: 0x0009DDF4
		private bool YouthNordArtisanOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "nord";
		}

		// Token: 0x06001765 RID: 5989 RVA: 0x0009FC10 File Offset: 0x0009DE10
		private void YouthNordArtisanOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SelectedTitleType = "artisan";
			string playerEquipmentId = this.GetPlayerEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedTitleType, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId, Hero.MainHero.IsFemale);
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "player_youth_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_childhood_decisive");
					narrativeMenuCharacter.SetEquipment(Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(playerEquipmentId));
				}
			}
		}

		// Token: 0x06001766 RID: 5990 RVA: 0x0009FCD4 File Offset: 0x0009DED4
		private void GetYouthNordInfantryOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				DefaultSkills.Roguery,
				DefaultSkills.Medicine
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Cunning, this._attributeLevelToAdd);
		}

		// Token: 0x06001767 RID: 5991 RVA: 0x0009FD28 File Offset: 0x0009DF28
		private bool YouthNordInfantryOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "nord";
		}

		// Token: 0x06001768 RID: 5992 RVA: 0x0009FD44 File Offset: 0x0009DF44
		private void YouthNordInfantryOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SelectedTitleType = "infantry";
			string playerEquipmentId = this.GetPlayerEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedTitleType, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId, Hero.MainHero.IsFemale);
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "player_youth_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_character_creation_nord_served_as_a_shieldbearer");
					narrativeMenuCharacter.SetEquipment(Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(playerEquipmentId));
				}
			}
		}

		// Token: 0x06001769 RID: 5993 RVA: 0x0009FE08 File Offset: 0x0009E008
		private void GetYouthNordMercenaryOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				DefaultSkills.Throwing,
				DefaultSkills.OneHanded
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Control, this._attributeLevelToAdd);
		}

		// Token: 0x0600176A RID: 5994 RVA: 0x0009FE5C File Offset: 0x0009E05C
		private bool YouthNordMercenaryOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "nord";
		}

		// Token: 0x0600176B RID: 5995 RVA: 0x0009FE78 File Offset: 0x0009E078
		private void YouthNordMercenaryOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SelectedTitleType = "mercenary";
			string playerEquipmentId = this.GetPlayerEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedTitleType, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId, Hero.MainHero.IsFemale);
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "player_youth_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_childhood_decisive");
					narrativeMenuCharacter.SetEquipment(Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(playerEquipmentId));
				}
			}
		}

		// Token: 0x0600176C RID: 5996 RVA: 0x0009FF3C File Offset: 0x0009E13C
		private void GetYouthCrewedAGalleyNarrativeOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				NavalSkills.Mariner,
				DefaultSkills.OneHanded
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Vigor, this._attributeLevelToAdd);
		}

		// Token: 0x0600176D RID: 5997 RVA: 0x0009FF90 File Offset: 0x0009E190
		private bool YouthCrewedAGalleyNarrativeOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "vlandia" || characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "empire";
		}

		// Token: 0x0600176E RID: 5998 RVA: 0x0009FFCC File Offset: 0x0009E1CC
		private void YouthCrewedAGalleyNarrativeOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SelectedTitleType = "seafarer";
			string playerEquipmentId = this.GetPlayerEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedTitleType, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId, Hero.MainHero.IsFemale);
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "player_youth_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_childhood_athlete");
					narrativeMenuCharacter.SetEquipment(Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(playerEquipmentId));
				}
			}
		}

		// Token: 0x0600176F RID: 5999 RVA: 0x000A0090 File Offset: 0x0009E290
		private void GetYouthRowedRiverTraderNarrativeOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				NavalSkills.Boatswain,
				DefaultSkills.TwoHanded
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Endurance, this._attributeLevelToAdd);
		}

		// Token: 0x06001770 RID: 6000 RVA: 0x000A00E4 File Offset: 0x0009E2E4
		private bool YouthRowedRiverTraderNarrativeOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "sturgia";
		}

		// Token: 0x06001771 RID: 6001 RVA: 0x000A0100 File Offset: 0x0009E300
		private void YouthRowedRiverTraderNarrativeOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SelectedTitleType = "seafarer";
			string playerEquipmentId = this.GetPlayerEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedTitleType, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId, Hero.MainHero.IsFemale);
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "player_youth_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_childhood_athlete");
					narrativeMenuCharacter.SetEquipment(Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(playerEquipmentId));
				}
			}
		}

		// Token: 0x06001772 RID: 6002 RVA: 0x000A01C4 File Offset: 0x0009E3C4
		private void GetYouthDeckhandCorsairNarrativeOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				NavalSkills.Mariner,
				DefaultSkills.OneHanded
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Cunning, this._attributeLevelToAdd);
		}

		// Token: 0x06001773 RID: 6003 RVA: 0x000A0218 File Offset: 0x0009E418
		private bool YouthDeckhandCorsairNarrativeOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "aserai";
		}

		// Token: 0x06001774 RID: 6004 RVA: 0x000A0234 File Offset: 0x0009E434
		private void YouthDeckhandCorsairNarrativeOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SelectedTitleType = "seafarer";
			string playerEquipmentId = this.GetPlayerEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedTitleType, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId, Hero.MainHero.IsFemale);
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "player_youth_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_childhood_athlete");
					narrativeMenuCharacter.SetEquipment(Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(playerEquipmentId));
				}
			}
		}

		// Token: 0x06001775 RID: 6005 RVA: 0x000A02F8 File Offset: 0x0009E4F8
		private void GetYouthRaidedRiverTrafficNarrativeOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				NavalSkills.Mariner,
				DefaultSkills.Charm
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Social, this._attributeLevelToAdd);
		}

		// Token: 0x06001776 RID: 6006 RVA: 0x000A034C File Offset: 0x0009E54C
		private bool YouthRaidedRiverTrafficNarrativeOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "khuzait";
		}

		// Token: 0x06001777 RID: 6007 RVA: 0x000A0368 File Offset: 0x0009E568
		private void YouthRaidedRiverTrafficNarrativeOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SelectedTitleType = "seafarer";
			string playerEquipmentId = this.GetPlayerEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedTitleType, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId, Hero.MainHero.IsFemale);
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "player_youth_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_childhood_athlete");
					narrativeMenuCharacter.SetEquipment(Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(playerEquipmentId));
				}
			}
		}

		// Token: 0x06001778 RID: 6008 RVA: 0x000A042C File Offset: 0x0009E62C
		private void GetYouthCoastalDefenderNarrativeOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				DefaultSkills.Bow,
				NavalSkills.Boatswain
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Vigor, this._attributeLevelToAdd);
		}

		// Token: 0x06001779 RID: 6009 RVA: 0x000A0480 File Offset: 0x0009E680
		private bool YouthCoastalDefenderNarrativeOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "battania";
		}

		// Token: 0x0600177A RID: 6010 RVA: 0x000A049C File Offset: 0x0009E69C
		private void YouthCoastalDefenderNarrativeOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SelectedTitleType = "seafarer";
			string playerEquipmentId = this.GetPlayerEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedTitleType, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId, Hero.MainHero.IsFemale);
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "player_youth_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_childhood_athlete");
					narrativeMenuCharacter.SetEquipment(Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(playerEquipmentId));
				}
			}
		}

		// Token: 0x0600177B RID: 6011 RVA: 0x000A0560 File Offset: 0x0009E760
		private void GetYouthServeRaiderShipNarrativeOptionArgs(NarrativeMenuOptionArgs args)
		{
			SkillObject[] array = new SkillObject[]
			{
				NavalSkills.Mariner,
				DefaultSkills.OneHanded
			};
			args.SetAffectedSkills(array);
			args.SetFocusToSkills(this._focusToAdd);
			args.SetLevelToSkills(this._skillLevelToAdd);
			args.SetLevelToAttribute(DefaultCharacterAttributes.Vigor, this._attributeLevelToAdd);
		}

		// Token: 0x0600177C RID: 6012 RVA: 0x000A05B4 File Offset: 0x0009E7B4
		private bool YouthServeRaiderShipNarrativeOptionOnCondition(CharacterCreationManager characterCreationManager)
		{
			return characterCreationManager.CharacterCreationContent.SelectedCulture.StringId == "nord";
		}

		// Token: 0x0600177D RID: 6013 RVA: 0x000A05D0 File Offset: 0x0009E7D0
		private void YouthServeRaiderShipNarrativeOptionOnSelect(CharacterCreationManager characterCreationManager)
		{
			characterCreationManager.CharacterCreationContent.SelectedTitleType = "seafarer";
			string playerEquipmentId = this.GetPlayerEquipmentId(characterCreationManager, characterCreationManager.CharacterCreationContent.SelectedTitleType, characterCreationManager.CharacterCreationContent.SelectedCulture.StringId, Hero.MainHero.IsFemale);
			foreach (NarrativeMenuCharacter narrativeMenuCharacter in characterCreationManager.CurrentMenu.Characters)
			{
				if (narrativeMenuCharacter.StringId == "player_youth_character")
				{
					narrativeMenuCharacter.SetAnimationId("act_childhood_athlete");
					narrativeMenuCharacter.SetEquipment(Game.Current.ObjectManager.GetObject<MBEquipmentRoster>(playerEquipmentId));
				}
			}
		}

		// Token: 0x04000BCC RID: 3020
		private readonly IReadOnlyDictionary<string, string> _occupationToEquipmentMapping = new Dictionary<string, string>
		{
			{ "retainer", "retainer" },
			{ "bard", "bard" },
			{ "hunter", "hunter" },
			{ "mercenary", "mercenary" },
			{ "infantry", "infantry" },
			{ "skirmisher", "skirmisher" },
			{ "artisan", "artisan" },
			{ "vagabond", "vagabond" },
			{ "guard", "guard" },
			{ "artisan_urban", "artisan" },
			{ "mercenary_urban", "artisan" },
			{ "merchant_urban", "merchant" },
			{ "vagabond_urban", "vagabond" },
			{ "seafarer", "seafarer" },
			{ "shipmaster_urban", "shipmaster" }
		};

		// Token: 0x04000BCD RID: 3021
		public const string MotherNarrativeCharacterStringId = "mother_character";

		// Token: 0x04000BCE RID: 3022
		public const string FatherNarrativeCharacterStringId = "father_character";

		// Token: 0x04000BCF RID: 3023
		public const string PlayerChildhoodCharacterStringId = "player_childhood_character";

		// Token: 0x04000BD0 RID: 3024
		public const string PlayerEducationCharacterStringId = "player_education_character";

		// Token: 0x04000BD1 RID: 3025
		public const string PlayerYouthCharacterStringId = "player_youth_character";

		// Token: 0x04000BD2 RID: 3026
		private int _focusToAdd;

		// Token: 0x04000BD3 RID: 3027
		private int _skillLevelToAdd;

		// Token: 0x04000BD4 RID: 3028
		private int _attributeLevelToAdd;

		// Token: 0x02000293 RID: 659
		private static class NavalCharacterOccupationTypes
		{
			// Token: 0x06001CC3 RID: 7363 RVA: 0x000B9D64 File Offset: 0x000B7F64
			public static bool IsUrbanOccupation(string occupation)
			{
				return occupation == "mercenary_urban" || occupation == "merchant_urban" || occupation == "vagabond_urban" || occupation == "artisan_urban" || occupation == "shipmaster_urban" || occupation == "retainer_urban" || occupation == "physician_urban" || occupation == "healer_urban" || occupation == "bard_urban";
			}

			// Token: 0x0400110F RID: 4367
			public const string Retainer = "retainer";

			// Token: 0x04001110 RID: 4368
			public const string Bard = "bard";

			// Token: 0x04001111 RID: 4369
			public const string Hunter = "hunter";

			// Token: 0x04001112 RID: 4370
			public const string Mercenary = "mercenary";

			// Token: 0x04001113 RID: 4371
			public const string Infantry = "infantry";

			// Token: 0x04001114 RID: 4372
			public const string Skirmisher = "skirmisher";

			// Token: 0x04001115 RID: 4373
			public const string Artisan = "artisan";

			// Token: 0x04001116 RID: 4374
			public const string Vagabond = "vagabond";

			// Token: 0x04001117 RID: 4375
			public const string Guard = "guard";

			// Token: 0x04001118 RID: 4376
			public const string ArtisanUrban = "artisan_urban";

			// Token: 0x04001119 RID: 4377
			public const string MercenaryUrban = "mercenary_urban";

			// Token: 0x0400111A RID: 4378
			public const string MerchantUrban = "merchant_urban";

			// Token: 0x0400111B RID: 4379
			public const string VagabondUrban = "vagabond_urban";

			// Token: 0x0400111C RID: 4380
			public const string RetainerUrban = "retainer_urban";

			// Token: 0x0400111D RID: 4381
			public const string PhysicianUrban = "physician_urban";

			// Token: 0x0400111E RID: 4382
			public const string HealerUrban = "healer_urban";

			// Token: 0x0400111F RID: 4383
			public const string BardUrban = "bard_urban";

			// Token: 0x04001120 RID: 4384
			public const string Seafarer = "seafarer";

			// Token: 0x04001121 RID: 4385
			public const string ShipmasterUrban = "shipmaster_urban";
		}
	}
}
