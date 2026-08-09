using System;
using System.Collections.Generic;
using System.Linq;
using SandBox.ViewModelCollection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.ViewModelCollection.OrderOfBattle
{
	// Token: 0x02000024 RID: 36
	public class NavalOrderOfBattleHeroItemVM : ViewModel
	{
		// Token: 0x060002E2 RID: 738 RVA: 0x0000F054 File Offset: 0x0000D254
		public NavalOrderOfBattleHeroItemVM(IAgentOriginBase agentOrigin, Action<NavalOrderOfBattleHeroItemVM, bool> onSelected)
		{
			this._onSelected = onSelected;
			this.AgentOrigin = agentOrigin;
			this.ImageIdentifier = new CharacterImageIdentifierVM(CharacterCode.CreateFrom(agentOrigin.Troop));
			this.IsMainHero = agentOrigin.Troop.IsPlayerCharacter;
			this.Tooltip = new BasicTooltipViewModel(() => this._cachedTooltipProperties);
			this.RefreshValues();
		}

		// Token: 0x060002E3 RID: 739 RVA: 0x0000F119 File Offset: 0x0000D319
		public override void RefreshValues()
		{
			base.RefreshValues();
			this._cachedTooltipProperties = this.GetTooltip();
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x0000F12D File Offset: 0x0000D32D
		public void ExecuteSelect()
		{
			if (!this.IsDisabled)
			{
				Action<NavalOrderOfBattleHeroItemVM, bool> onSelected = this._onSelected;
				if (onSelected == null)
				{
					return;
				}
				onSelected(this, true);
			}
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x0000F149 File Offset: 0x0000D349
		public void ExecuteToggleSelect()
		{
			if (!this.IsDisabled)
			{
				Action<NavalOrderOfBattleHeroItemVM, bool> onSelected = this._onSelected;
				if (onSelected == null)
				{
					return;
				}
				onSelected(this, !this.IsSelected);
			}
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x0000F16D File Offset: 0x0000D36D
		public void ExecuteDeselect()
		{
			if (!this.IsDisabled)
			{
				Action<NavalOrderOfBattleHeroItemVM, bool> onSelected = this._onSelected;
				if (onSelected == null)
				{
					return;
				}
				onSelected(this, false);
			}
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x0000F18C File Offset: 0x0000D38C
		private List<TooltipProperty> GetTooltip()
		{
			CharacterObject characterObject = this.AgentOrigin.Troop as CharacterObject;
			Hero hero = ((characterObject != null) ? characterObject.HeroObject : null);
			List<TooltipProperty> list = new List<TooltipProperty>
			{
				new TooltipProperty(((hero != null) ? hero.Name.ToString() : null) ?? this.AgentOrigin.Troop.Name.ToString(), string.Empty, 0, false, 4096)
			};
			if (this.IsMainHero)
			{
				list.Add(new TooltipProperty(string.Empty, new TextObject("{=9y7LtTLf}Main hero is always assigned to the first formation.", null).ToString(), 0, false, 0));
				list.Add(new TooltipProperty(string.Empty, string.Empty, 0, false, 0));
			}
			else if (this.IsDisabled)
			{
				list.Add(new TooltipProperty(string.Empty, new TextObject("{=3XlyBbSE}You cannot move heroes when you are not the general.", null).ToString(), 0, false, 0));
				list.Add(new TooltipProperty(string.Empty, string.Empty, 0, false, 0));
			}
			if (((hero != null) ? hero.PartyBelongedTo : null) != null)
			{
				list.Add(new TooltipProperty(GameTexts.FindText("str_party", null).ToString(), hero.PartyBelongedTo.Name.ToString(), 0, false, 0));
			}
			if (hero != null)
			{
				foreach (SkillObject skillObject in Skills.All)
				{
					if (skillObject.StringId == "Mariner" || skillObject.StringId == "Boatswain" || skillObject.StringId == "Shipmaster")
					{
						list.Add(new TooltipProperty(skillObject.Name.ToString(), hero.GetSkillValue(skillObject).ToString(), 0, false, 0)
						{
							OnlyShowWhenNotExtended = true
						});
					}
				}
				list.Add(new TooltipProperty(string.Empty, string.Empty, 0, false, 1024)
				{
					OnlyShowWhenNotExtended = true
				});
				List<PerkObject> list2;
				float captainRatingForTroopUsages = Campaign.Current.Models.BattleCaptainModel.GetCaptainRatingForTroopUsages(hero, FormationClassExtensions.GetTroopUsageFlags(0), ref list2);
				List<PerkObject> list3;
				float captainRatingForTroopUsages2 = Campaign.Current.Models.BattleCaptainModel.GetCaptainRatingForTroopUsages(hero, FormationClassExtensions.GetTroopUsageFlags(1), ref list3);
				list.Add(new TooltipProperty(this._infantryInfluenceText.ToString(), ((int)(captainRatingForTroopUsages * 100f)).ToString(), 0, false, 0)
				{
					OnlyShowWhenNotExtended = true
				});
				list.Add(new TooltipProperty(this._rangedInfluenceText.ToString(), ((int)(captainRatingForTroopUsages2 * 100f)).ToString(), 0, false, 0)
				{
					OnlyShowWhenNotExtended = true
				});
				list.Add(new TooltipProperty(string.Empty, string.Empty, 0, false, 0)
				{
					OnlyShowWhenNotExtended = true
				});
				List<PerkObject> list4 = list2.Union<PerkObject>(list3).ToList<PerkObject>();
				list4.Sort(this._perkComparer);
				if (list4.Count != 0)
				{
					list.Add(new TooltipProperty(this._captainPerksText.ToString(), string.Empty, 0, true, 4096));
					using (List<PerkObject>.Enumerator enumerator2 = list4.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							PerkObject perkObject = enumerator2.Current;
							if (perkObject.PrimaryRole == 13 || perkObject.SecondaryRole == 13)
							{
								TextObject textObject = ((perkObject.PrimaryRole == 13) ? perkObject.PrimaryDescription : perkObject.SecondaryDescription);
								string genericImageText = HyperlinkTexts.GetGenericImageText(CampaignUIHelper.GetSkillMeshId(perkObject.Skill, true), 2);
								this._perkDefinitionText.SetTextVariable("PERK_NAME", perkObject.Name).SetTextVariable("SKILL", genericImageText).SetTextVariable("SKILL_LEVEL", perkObject.RequiredSkillValue, 2);
								list.Add(new TooltipProperty(this._perkDefinitionText.ToString(), textObject.ToString(), 0, true, 0));
							}
						}
						goto IL_03EA;
					}
				}
				list.Add(new TooltipProperty(this._noPerksText.ToString(), string.Empty, 0, true, 0));
				IL_03EA:
				if (Input.IsGamepadActive)
				{
					GameTexts.SetVariable("EXTEND_KEY", GameKeyTextExtensions.GetHotKeyGameText(Game.Current.GameTextManager, "MapHotKeyCategory", "MapFollowModifier").ToString());
				}
				else
				{
					GameTexts.SetVariable("EXTEND_KEY", Game.Current.GameTextManager.FindText("str_game_key_text", "anyalt").ToString());
				}
				list.Add(new TooltipProperty(string.Empty, GameTexts.FindText("str_map_tooltip_info", null).ToString(), -1, false, 0)
				{
					OnlyShowWhenNotExtended = true
				});
			}
			return list;
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x0000F624 File Offset: 0x0000D824
		public bool GetCanBeUnassignedOrMoved()
		{
			return !this.IsDisabled && !this.IsMainHero;
		}

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x060002E9 RID: 745 RVA: 0x0000F639 File Offset: 0x0000D839
		// (set) Token: 0x060002EA RID: 746 RVA: 0x0000F641 File Offset: 0x0000D841
		[DataSourceProperty]
		public bool IsDisabled
		{
			get
			{
				return this._isDisabled;
			}
			set
			{
				if (value != this._isDisabled)
				{
					this._isDisabled = value;
					base.OnPropertyChangedWithValue(value, "IsDisabled");
				}
			}
		}

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x060002EB RID: 747 RVA: 0x0000F65F File Offset: 0x0000D85F
		// (set) Token: 0x060002EC RID: 748 RVA: 0x0000F667 File Offset: 0x0000D867
		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x060002ED RID: 749 RVA: 0x0000F685 File Offset: 0x0000D885
		// (set) Token: 0x060002EE RID: 750 RVA: 0x0000F68D File Offset: 0x0000D88D
		[DataSourceProperty]
		public bool IsMainHero
		{
			get
			{
				return this._isMainHero;
			}
			set
			{
				if (value != this._isMainHero)
				{
					this._isMainHero = value;
					base.OnPropertyChangedWithValue(value, "IsMainHero");
				}
			}
		}

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x060002EF RID: 751 RVA: 0x0000F6AB File Offset: 0x0000D8AB
		// (set) Token: 0x060002F0 RID: 752 RVA: 0x0000F6B3 File Offset: 0x0000D8B3
		[DataSourceProperty]
		public CharacterImageIdentifierVM ImageIdentifier
		{
			get
			{
				return this._imageIdentifier;
			}
			set
			{
				if (value != this._imageIdentifier)
				{
					this._imageIdentifier = value;
					base.OnPropertyChangedWithValue<CharacterImageIdentifierVM>(value, "ImageIdentifier");
				}
			}
		}

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x060002F1 RID: 753 RVA: 0x0000F6D1 File Offset: 0x0000D8D1
		// (set) Token: 0x060002F2 RID: 754 RVA: 0x0000F6D9 File Offset: 0x0000D8D9
		[DataSourceProperty]
		public BasicTooltipViewModel Tooltip
		{
			get
			{
				return this._tooltip;
			}
			set
			{
				if (value != this._tooltip)
				{
					this._tooltip = value;
					base.OnPropertyChangedWithValue<BasicTooltipViewModel>(value, "Tooltip");
				}
			}
		}

		// Token: 0x04000106 RID: 262
		public readonly IAgentOriginBase AgentOrigin;

		// Token: 0x04000107 RID: 263
		private readonly Action<NavalOrderOfBattleHeroItemVM, bool> _onSelected;

		// Token: 0x04000108 RID: 264
		private List<TooltipProperty> _cachedTooltipProperties;

		// Token: 0x04000109 RID: 265
		private readonly TextObject _perkDefinitionText = new TextObject("{=jCdZY3i4}{PERK_NAME} ({SKILL_LEVEL} - {SKILL})", null);

		// Token: 0x0400010A RID: 266
		private readonly TextObject _captainPerksText = new TextObject("{=pgXuyHxH}Captain Perks", null);

		// Token: 0x0400010B RID: 267
		private readonly TextObject _infantryInfluenceText = new TextObject("{=SSLUHH6j}Infantry Influence", null);

		// Token: 0x0400010C RID: 268
		private readonly TextObject _rangedInfluenceText = new TextObject("{=0DMM0agr}Ranged Influence", null);

		// Token: 0x0400010D RID: 269
		private readonly TextObject _noPerksText = new TextObject("{=7yaDnyKb}There is no additional perk influence.", null);

		// Token: 0x0400010E RID: 270
		private readonly PerkObjectComparer _perkComparer = new PerkObjectComparer();

		// Token: 0x0400010F RID: 271
		private bool _isDisabled;

		// Token: 0x04000110 RID: 272
		private bool _isSelected;

		// Token: 0x04000111 RID: 273
		private bool _isMainHero;

		// Token: 0x04000112 RID: 274
		private CharacterImageIdentifierVM _imageIdentifier;

		// Token: 0x04000113 RID: 275
		private BasicTooltipViewModel _tooltip;
	}
}
