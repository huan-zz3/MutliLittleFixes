using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.CustomBattle.CustomBattle
{
	// Token: 0x0200000A RID: 10
	public class NavalCustomBattleArmyCompositionGroupVM : ViewModel
	{
		// Token: 0x06000053 RID: 83 RVA: 0x00003800 File Offset: 0x00001A00
		public NavalCustomBattleArmyCompositionGroupVM(NavalCustomBattleTroopTypeSelectionPopUpVM troopTypeSelectionPopUp)
		{
			foreach (BasicCharacterObject basicCharacterObject in from c in Game.Current.ObjectManager.GetObjectTypeList<BasicCharacterObject>()
				where c.IsSoldier && !c.IsObsolete
				select c)
			{
				this._allCharacterObjects.Add(basicCharacterObject);
			}
			this.CompositionValues = new int[4];
			this.CompositionValues[0] = 50;
			this.CompositionValues[1] = 50;
			this.CompositionValues[2] = 0;
			this.CompositionValues[3] = 0;
			this.MeleeInfantryComposition = new NavalCustomBattleArmyCompositionItemVM(NavalCustomBattleArmyCompositionItemVM.CompositionType.MeleeInfantry, this._allCharacterObjects, this._allSkills, new Action<int, int>(this.UpdateSliders), troopTypeSelectionPopUp, this.CompositionValues);
			this.RangedInfantryComposition = new NavalCustomBattleArmyCompositionItemVM(NavalCustomBattleArmyCompositionItemVM.CompositionType.RangedInfantry, this._allCharacterObjects, this._allSkills, new Action<int, int>(this.UpdateSliders), troopTypeSelectionPopUp, this.CompositionValues);
			this.MeleeCavalryComposition = new NavalCustomBattleArmyCompositionItemVM(NavalCustomBattleArmyCompositionItemVM.CompositionType.MeleeCavalry, this._allCharacterObjects, this._allSkills, new Action<int, int>(this.UpdateSliders), troopTypeSelectionPopUp, this.CompositionValues);
			this.RangedCavalryComposition = new NavalCustomBattleArmyCompositionItemVM(NavalCustomBattleArmyCompositionItemVM.CompositionType.RangedCavalry, this._allCharacterObjects, this._allSkills, new Action<int, int>(this.UpdateSliders), troopTypeSelectionPopUp, this.CompositionValues);
			this._cachedArmySizeRatio = 0.725f;
			this._cachedLandArmyCount = BannerlordConfig.GetRealBattleSizeForNaval() / 5;
			this.UpdateTroopCountLimits(1, BannerlordConfig.MaxBattleSize, 1, 1);
			this.RefreshValues();
		}

		// Token: 0x06000054 RID: 84 RVA: 0x000039B0 File Offset: 0x00001BB0
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ArmySizeTitle = (this.IsLand ? GameTexts.FindText("str_army_size", null).ToString() : new TextObject("{=EQLbYxec}Crew Count", null).ToString());
			this.MeleeInfantryComposition.RefreshValues();
			this.RangedInfantryComposition.RefreshValues();
			this.MeleeCavalryComposition.RefreshValues();
			this.RangedCavalryComposition.RefreshValues();
			this.UpdateIsWarned();
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00003A28 File Offset: 0x00001C28
		private static int SumOfValues(int[] array, bool[] enabledArray, int excludedIndex = -1)
		{
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (enabledArray[i] && excludedIndex != i)
				{
					num += array[i];
				}
			}
			return num;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00003A58 File Offset: 0x00001C58
		public void SetCurrentSelectedCulture(BasicCultureObject selectedCulture)
		{
			if (this._selectedCulture != selectedCulture)
			{
				this.MeleeInfantryComposition.SetCurrentSelectedCulture(selectedCulture);
				this.RangedInfantryComposition.SetCurrentSelectedCulture(selectedCulture);
				this.MeleeCavalryComposition.SetCurrentSelectedCulture(selectedCulture);
				this.RangedCavalryComposition.SetCurrentSelectedCulture(selectedCulture);
				this._selectedCulture = selectedCulture;
			}
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00003AA8 File Offset: 0x00001CA8
		private void UpdateSliders(int value, int changedSliderIndex)
		{
			if (this._updatingSliders)
			{
				return;
			}
			this._updatingSliders = true;
			bool[] array = new bool[]
			{
				!this.MeleeInfantryComposition.IsLocked,
				!this.RangedInfantryComposition.IsLocked,
				!this.MeleeCavalryComposition.IsLocked,
				!this.RangedCavalryComposition.IsLocked
			};
			int[] array2 = new int[]
			{
				this.CompositionValues[0],
				this.CompositionValues[1],
				this.CompositionValues[2],
				this.CompositionValues[3]
			};
			int[] array3 = new int[]
			{
				this.CompositionValues[0],
				this.CompositionValues[1],
				this.CompositionValues[2],
				this.CompositionValues[3]
			};
			int num = array.Count<bool>((bool s) => s);
			if (array[changedSliderIndex])
			{
				num--;
			}
			if (num > 0)
			{
				int num2 = NavalCustomBattleArmyCompositionGroupVM.SumOfValues(array2, array, -1);
				array[changedSliderIndex] = false;
				if (value >= num2)
				{
					value = num2;
				}
				int num3 = value - array2[changedSliderIndex];
				if (num3 != 0)
				{
					array3[changedSliderIndex] = value;
					int num4 = -num3;
					int num5 = num4 / num;
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i])
						{
							array3[i] += num5;
							num4 -= num5;
						}
					}
					for (int j = 0; j < array.Length; j++)
					{
						if (array[j] && array3[j] < 0)
						{
							num4 += array3[j];
							array3[j] = 0;
						}
					}
					if (num4 > 0)
					{
						while (num4 != 0)
						{
							int num6 = int.MaxValue;
							int num7 = -1;
							for (int k = 0; k < array.Length; k++)
							{
								if (array[k] && array3[k] < num6)
								{
									num6 = array3[k];
									num7 = k;
								}
							}
							array3[num7]++;
							num4--;
						}
					}
					else if (num4 < 0)
					{
						while (num4 != 0)
						{
							int num8 = int.MinValue;
							int num9 = -1;
							for (int l = 0; l < array.Length; l++)
							{
								if (array[l] && array3[l] > num8)
								{
									num8 = array3[l];
									num9 = l;
								}
							}
							array3[num9]--;
							num4++;
						}
					}
				}
			}
			this.SetArmyCompositionValue(0, array3[0], this.MeleeInfantryComposition);
			this.SetArmyCompositionValue(1, array3[1], this.RangedInfantryComposition);
			this.SetArmyCompositionValue(2, array3[2], this.MeleeCavalryComposition);
			this.SetArmyCompositionValue(3, array3[3], this.RangedCavalryComposition);
			this._updatingSliders = false;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00003D27 File Offset: 0x00001F27
		private void SetArmyCompositionValue(int index, int value, NavalCustomBattleArmyCompositionItemVM composition)
		{
			this.CompositionValues[index] = value;
			composition.RefreshCompositionValue();
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00003D38 File Offset: 0x00001F38
		public void ExecuteRandomize(int targetDeckSize)
		{
			if (this.IsLand)
			{
				int num = MBRandom.RandomInt(100);
				this.MeleeInfantryComposition.ExecuteRandomize(num);
				this.RangedInfantryComposition.ExecuteRandomize(100 - num);
				this.ArmySize = targetDeckSize;
				return;
			}
			int num2 = MBRandom.RandomInt(100);
			int num3 = MBRandom.RandomInt(100);
			int num4 = MBRandom.RandomInt(100);
			int num5 = MBRandom.RandomInt(100);
			int num6 = num2 + num3 + num4 + num5;
			int num7 = MathF.Round(100f * ((float)num2 / (float)num6));
			int num8 = MathF.Round(100f * ((float)num3 / (float)num6));
			int num9 = MathF.Round(100f * ((float)num4 / (float)num6));
			int num10 = 100 - (num7 + num8 + num9);
			this.MeleeInfantryComposition.ExecuteRandomize(num7);
			this.RangedInfantryComposition.ExecuteRandomize(num8);
			this.MeleeCavalryComposition.ExecuteRandomize(num9);
			this.RangedCavalryComposition.ExecuteRandomize(num10);
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00003E20 File Offset: 0x00002020
		public void UpdateTroopCountLimits(int minTroopCount, int maxTroopCount, int skeletalSize, int deckSize)
		{
			this.MinArmySize = MathF.Max(1, minTroopCount);
			this.MaxArmySize = MathF.Min(BannerlordConfig.MaxBattleSize, maxTroopCount);
			if (this.MaxArmySize < this.MinArmySize)
			{
				Debug.FailedAssert("Max army size is less than min army size!", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.CustomBattle\\CustomBattle\\NavalCustomBattleArmyCompositionGroupVM.cs", "UpdateTroopCountLimits", 261);
				this.MaxArmySize = this.MinArmySize;
			}
			this.SkeletalSize = skeletalSize;
			this.DeckSize = deckSize;
			if (this.IsLand)
			{
				this.ArmySize = this._cachedLandArmyCount;
			}
			else
			{
				float cachedArmySizeRatio = this._cachedArmySizeRatio;
				this.ArmySize = MathF.Round(MathF.Lerp((float)this.MinArmySize, (float)this.MaxArmySize, cachedArmySizeRatio, 1E-05f));
				this._cachedArmySizeRatio = cachedArmySizeRatio;
			}
			this.UpdateIsWarned();
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00003EDC File Offset: 0x000020DC
		private void UpdateIsWarned()
		{
			if (this.IsLand)
			{
				this.IsWarned = false;
				this.WarningText = null;
				return;
			}
			this.IsWarned = this.ArmySize < this.SkeletalSize;
			if (this.IsWarned)
			{
				this.WarningText = new TextObject("{=nkIeNadI}Ships may be undercrewed!", null).ToString();
				return;
			}
			if (this.ArmySize > this.DeckSize)
			{
				this.WarningText = new TextObject("{=JaFgzRhS}{AMOUNT} troops in reserve", null).SetTextVariable("AMOUNT", this.ArmySize - this.DeckSize).ToString();
				return;
			}
			this.WarningText = null;
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600005C RID: 92 RVA: 0x00003F76 File Offset: 0x00002176
		// (set) Token: 0x0600005D RID: 93 RVA: 0x00003F7E File Offset: 0x0000217E
		[DataSourceProperty]
		public NavalCustomBattleArmyCompositionItemVM MeleeInfantryComposition
		{
			get
			{
				return this._meleeInfantryComposition;
			}
			set
			{
				if (value != this._meleeInfantryComposition)
				{
					this._meleeInfantryComposition = value;
					base.OnPropertyChangedWithValue<NavalCustomBattleArmyCompositionItemVM>(value, "MeleeInfantryComposition");
				}
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600005E RID: 94 RVA: 0x00003F9C File Offset: 0x0000219C
		// (set) Token: 0x0600005F RID: 95 RVA: 0x00003FA4 File Offset: 0x000021A4
		[DataSourceProperty]
		public NavalCustomBattleArmyCompositionItemVM RangedInfantryComposition
		{
			get
			{
				return this._rangedInfantryComposition;
			}
			set
			{
				if (value != this._rangedInfantryComposition)
				{
					this._rangedInfantryComposition = value;
					base.OnPropertyChangedWithValue<NavalCustomBattleArmyCompositionItemVM>(value, "RangedInfantryComposition");
				}
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000060 RID: 96 RVA: 0x00003FC2 File Offset: 0x000021C2
		// (set) Token: 0x06000061 RID: 97 RVA: 0x00003FCA File Offset: 0x000021CA
		[DataSourceProperty]
		public NavalCustomBattleArmyCompositionItemVM MeleeCavalryComposition
		{
			get
			{
				return this._meleeCavalryComposition;
			}
			set
			{
				if (value != this._meleeCavalryComposition)
				{
					this._meleeCavalryComposition = value;
					base.OnPropertyChangedWithValue<NavalCustomBattleArmyCompositionItemVM>(value, "MeleeCavalryComposition");
				}
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000062 RID: 98 RVA: 0x00003FE8 File Offset: 0x000021E8
		// (set) Token: 0x06000063 RID: 99 RVA: 0x00003FF0 File Offset: 0x000021F0
		[DataSourceProperty]
		public NavalCustomBattleArmyCompositionItemVM RangedCavalryComposition
		{
			get
			{
				return this._rangedCavalryComposition;
			}
			set
			{
				if (value != this._rangedCavalryComposition)
				{
					this._rangedCavalryComposition = value;
					base.OnPropertyChangedWithValue<NavalCustomBattleArmyCompositionItemVM>(value, "RangedCavalryComposition");
				}
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000064 RID: 100 RVA: 0x0000400E File Offset: 0x0000220E
		// (set) Token: 0x06000065 RID: 101 RVA: 0x00004016 File Offset: 0x00002216
		[DataSourceProperty]
		public string ArmySizeTitle
		{
			get
			{
				return this._armySizeTitle;
			}
			set
			{
				if (value != this._armySizeTitle)
				{
					this._armySizeTitle = value;
					base.OnPropertyChangedWithValue<string>(value, "ArmySizeTitle");
				}
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000066 RID: 102 RVA: 0x00004039 File Offset: 0x00002239
		// (set) Token: 0x06000067 RID: 103 RVA: 0x00004041 File Offset: 0x00002241
		[DataSourceProperty]
		public string WarningText
		{
			get
			{
				return this._warningText;
			}
			set
			{
				if (value != this._warningText)
				{
					this._warningText = value;
					base.OnPropertyChangedWithValue<string>(value, "WarningText");
				}
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000068 RID: 104 RVA: 0x00004064 File Offset: 0x00002264
		// (set) Token: 0x06000069 RID: 105 RVA: 0x0000406C File Offset: 0x0000226C
		[DataSourceProperty]
		public bool IsWarned
		{
			get
			{
				return this._isWarned;
			}
			set
			{
				if (value != this._isWarned)
				{
					this._isWarned = value;
					base.OnPropertyChangedWithValue(value, "IsWarned");
				}
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x0600006A RID: 106 RVA: 0x0000408A File Offset: 0x0000228A
		// (set) Token: 0x0600006B RID: 107 RVA: 0x00004094 File Offset: 0x00002294
		[DataSourceProperty]
		public int ArmySize
		{
			get
			{
				return this._armySize;
			}
			set
			{
				value = (int)MathF.Clamp((float)value, (float)this.MinArmySize, (float)this.MaxArmySize);
				if (this._armySize != value)
				{
					this._armySize = value;
					base.OnPropertyChangedWithValue(value, "ArmySize");
					if (!this.IsLand)
					{
						this._cachedArmySizeRatio = (float)(value - this.MinArmySize) / (float)(this.MaxArmySize - this.MinArmySize);
					}
					else
					{
						this._cachedLandArmyCount = value;
					}
					this.UpdateIsWarned();
				}
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x0600006C RID: 108 RVA: 0x0000410A File Offset: 0x0000230A
		// (set) Token: 0x0600006D RID: 109 RVA: 0x00004112 File Offset: 0x00002312
		[DataSourceProperty]
		public int MaxArmySize
		{
			get
			{
				return this._maxArmySize;
			}
			set
			{
				if (this._maxArmySize != value)
				{
					this._maxArmySize = value;
					base.OnPropertyChangedWithValue(value, "MaxArmySize");
				}
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x0600006E RID: 110 RVA: 0x00004130 File Offset: 0x00002330
		// (set) Token: 0x0600006F RID: 111 RVA: 0x00004138 File Offset: 0x00002338
		[DataSourceProperty]
		public int MinArmySize
		{
			get
			{
				return this._minArmySize;
			}
			set
			{
				if (this._minArmySize != value)
				{
					this._minArmySize = value;
					base.OnPropertyChangedWithValue(value, "MinArmySize");
				}
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000070 RID: 112 RVA: 0x00004156 File Offset: 0x00002356
		// (set) Token: 0x06000071 RID: 113 RVA: 0x0000415E File Offset: 0x0000235E
		public int SkeletalSize
		{
			get
			{
				return this._skeletalSize;
			}
			set
			{
				if (this._skeletalSize != value)
				{
					this._skeletalSize = value;
					base.OnPropertyChangedWithValue(value, "SkeletalSize");
				}
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000072 RID: 114 RVA: 0x0000417C File Offset: 0x0000237C
		// (set) Token: 0x06000073 RID: 115 RVA: 0x00004184 File Offset: 0x00002384
		public int DeckSize
		{
			get
			{
				return this._deckSize;
			}
			set
			{
				if (this._deckSize != value)
				{
					this._deckSize = value;
					base.OnPropertyChangedWithValue(value, "DeckSize");
				}
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000074 RID: 116 RVA: 0x000041A2 File Offset: 0x000023A2
		// (set) Token: 0x06000075 RID: 117 RVA: 0x000041AC File Offset: 0x000023AC
		public bool IsLand
		{
			get
			{
				return this._isLand;
			}
			set
			{
				if (this._isLand != value)
				{
					this._isLand = value;
					base.OnPropertyChangedWithValue(value, "IsLand");
					this.RefreshValues();
					this.MeleeInfantryComposition.IsLand = value;
					this.RangedInfantryComposition.IsLand = value;
					this.MeleeCavalryComposition.IsLand = value;
					this.RangedCavalryComposition.IsLand = value;
				}
			}
		}

		// Token: 0x04000010 RID: 16
		public int[] CompositionValues;

		// Token: 0x04000011 RID: 17
		private bool _updatingSliders;

		// Token: 0x04000012 RID: 18
		private BasicCultureObject _selectedCulture;

		// Token: 0x04000013 RID: 19
		private float _cachedArmySizeRatio;

		// Token: 0x04000014 RID: 20
		private int _cachedLandArmyCount;

		// Token: 0x04000015 RID: 21
		private readonly MBReadOnlyList<SkillObject> _allSkills = Game.Current.ObjectManager.GetObjectTypeList<SkillObject>();

		// Token: 0x04000016 RID: 22
		private readonly List<BasicCharacterObject> _allCharacterObjects = new List<BasicCharacterObject>();

		// Token: 0x04000017 RID: 23
		private NavalCustomBattleArmyCompositionItemVM _meleeInfantryComposition;

		// Token: 0x04000018 RID: 24
		private NavalCustomBattleArmyCompositionItemVM _rangedInfantryComposition;

		// Token: 0x04000019 RID: 25
		private NavalCustomBattleArmyCompositionItemVM _meleeCavalryComposition;

		// Token: 0x0400001A RID: 26
		private NavalCustomBattleArmyCompositionItemVM _rangedCavalryComposition;

		// Token: 0x0400001B RID: 27
		private int _armySize;

		// Token: 0x0400001C RID: 28
		private int _maxArmySize;

		// Token: 0x0400001D RID: 29
		private int _minArmySize;

		// Token: 0x0400001E RID: 30
		private int _skeletalSize;

		// Token: 0x0400001F RID: 31
		private int _deckSize;

		// Token: 0x04000020 RID: 32
		private string _armySizeTitle;

		// Token: 0x04000021 RID: 33
		private string _warningText;

		// Token: 0x04000022 RID: 34
		private bool _isWarned;

		// Token: 0x04000023 RID: 35
		private bool _isLand;
	}
}
