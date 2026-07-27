using System;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.ShipInput;
using TaleWorlds.CampaignSystem.ViewModelCollection.Input;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Tutorial;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.ViewModelCollection.Missions.ShipControl
{
	// Token: 0x02000028 RID: 40
	public class MissionShipControlVM : ViewModel
	{
		// Token: 0x06000377 RID: 887 RVA: 0x0001214C File Offset: 0x0001034C
		public MissionShipControlVM()
		{
			this._activeSailState = MissionShipControlVM.SailStateVisual.Invalid;
			this._activeOarsmenState = MissionShipControlVM.OarsmenStateVisual.Invalid;
			this.ShipHitPoints = new MissionHitPointPropertiesVM();
			this.SailHitPoints = new MissionHitPointPropertiesVM();
			this.FireHitPoints = new MissionHitPointPropertiesVM();
			Game.Current.EventManager.RegisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChanged));
			this.RefreshValues();
		}

		// Token: 0x06000378 RID: 888 RVA: 0x000121B0 File Offset: 0x000103B0
		public override void RefreshValues()
		{
			base.RefreshValues();
			this.CancelText = new TextObject("{=3CpNUnVl}Cancel", null).ToString();
			MissionShipControlVM.ShipControlInputKeyItemVM changeCameraKey = this.ChangeCameraKey;
			if (changeCameraKey != null)
			{
				changeCameraKey.RefreshValues();
			}
			MissionShipControlVM.ShipControlInputKeyItemVM cutLooseKey = this.CutLooseKey;
			if (cutLooseKey != null)
			{
				cutLooseKey.RefreshValues();
			}
			MissionShipControlVM.ShipControlInputKeyItemVM toggleOarsmenKey = this.ToggleOarsmenKey;
			if (toggleOarsmenKey != null)
			{
				toggleOarsmenKey.RefreshValues();
			}
			MissionShipControlVM.ShipControlInputKeyItemVM toggleSailKey = this.ToggleSailKey;
			if (toggleSailKey != null)
			{
				toggleSailKey.RefreshValues();
			}
			MissionShipControlVM.ShipControlInputKeyItemVM toggleBallistaKey = this.ToggleBallistaKey;
			if (toggleBallistaKey != null)
			{
				toggleBallistaKey.RefreshValues();
			}
			MissionShipControlVM.ShipControlInputKeyItemVM attemptBoardingKey = this.AttemptBoardingKey;
			if (attemptBoardingKey != null)
			{
				attemptBoardingKey.RefreshValues();
			}
			MissionShipControlVM.ShipControlInputKeyItemVM stopUsingShipKey = this.StopUsingShipKey;
			if (stopUsingShipKey == null)
			{
				return;
			}
			stopUsingShipKey.RefreshValues();
		}

		// Token: 0x06000379 RID: 889 RVA: 0x00012250 File Offset: 0x00010450
		public override void OnFinalize()
		{
			base.OnFinalize();
			MissionShipControlVM.ShipControlInputKeyItemVM changeCameraKey = this.ChangeCameraKey;
			if (changeCameraKey != null)
			{
				changeCameraKey.OnFinalize();
			}
			MissionShipControlVM.ShipControlInputKeyItemVM cutLooseKey = this.CutLooseKey;
			if (cutLooseKey != null)
			{
				cutLooseKey.OnFinalize();
			}
			MissionShipControlVM.ShipControlInputKeyItemVM toggleOarsmenKey = this.ToggleOarsmenKey;
			if (toggleOarsmenKey != null)
			{
				toggleOarsmenKey.OnFinalize();
			}
			MissionShipControlVM.ShipControlInputKeyItemVM toggleSailKey = this.ToggleSailKey;
			if (toggleSailKey != null)
			{
				toggleSailKey.OnFinalize();
			}
			MissionShipControlVM.ShipControlInputKeyItemVM toggleBallistaKey = this.ToggleBallistaKey;
			if (toggleBallistaKey != null)
			{
				toggleBallistaKey.OnFinalize();
			}
			MissionShipControlVM.ShipControlInputKeyItemVM attemptBoardingKey = this.AttemptBoardingKey;
			if (attemptBoardingKey != null)
			{
				attemptBoardingKey.OnFinalize();
			}
			MissionShipControlVM.ShipControlInputKeyItemVM stopUsingShipKey = this.StopUsingShipKey;
			if (stopUsingShipKey != null)
			{
				stopUsingShipKey.OnFinalize();
			}
			Game.Current.EventManager.UnregisterEvent<TutorialNotificationElementChangeEvent>(new Action<TutorialNotificationElementChangeEvent>(this.OnTutorialNotificationElementIDChanged));
		}

		// Token: 0x0600037A RID: 890 RVA: 0x000122F8 File Offset: 0x000104F8
		public void SetTargetedShip(MissionShip ship, float screenX = -5000f, float screenY = -5000f, float screenW = -1f)
		{
			if (ship == null)
			{
				this.HasTargetedShip = false;
				this.IsTargetedShipPlayerTeam = false;
				this.IsTargetedShipPlayerAllyTeam = false;
				this.IsTargetedShipEnemyTeam = false;
				this.TargetedShipPosition = new Vec2(-5000f, -5000f);
				this.TargetedShipWSign = -1;
				return;
			}
			this.HasTargetedShip = true;
			Team team = ship.Team;
			this.IsTargetedShipPlayerTeam = team != null && team.TeamSide == 0;
			Team team2 = ship.Team;
			this.IsTargetedShipPlayerAllyTeam = team2 != null && team2.TeamSide == 1;
			Team team3 = ship.Team;
			this.IsTargetedShipEnemyTeam = team3 != null && team3.TeamSide == 2;
			this.TargetedShipPosition = new Vec2(screenX, screenY);
			this.TargetedShipWSign = ((screenW > 0f) ? 1 : (-1));
		}

		// Token: 0x0600037B RID: 891 RVA: 0x000123BC File Offset: 0x000105BC
		public void SetBoardingTargetShip(MissionShip ship, float screenX = -5000f, float screenY = -5000f, float screenW = -1f)
		{
			if (ship == null)
			{
				this.BoardingTargetShipPosition = new Vec2(-5000f, -5000f);
				this.BoardingTargetShipWSign = -1;
				return;
			}
			this.BoardingTargetShipPosition = new Vec2(screenX, screenY);
			this.BoardingTargetShipWSign = ((screenW > 0f) ? 1 : (-1));
		}

		// Token: 0x0600037C RID: 892 RVA: 0x0001240C File Offset: 0x0001060C
		public void SetSailState(SailInput input)
		{
			MissionShipControlVM.SailStateVisual sailVisual = MissionShipControlVM.GetSailVisual(input);
			if (this._activeSailState == sailVisual)
			{
				return;
			}
			this.SailState = sailVisual.ToString();
			this._activeSailState = sailVisual;
		}

		// Token: 0x0600037D RID: 893 RVA: 0x00012444 File Offset: 0x00010644
		public void SetOarsmanLevel(int level)
		{
			if (level == (int)this._activeOarsmenState)
			{
				return;
			}
			if (level == 0)
			{
				this._activeOarsmenState = MissionShipControlVM.OarsmenStateVisual.Idle;
			}
			else if (level == 1)
			{
				this._activeOarsmenState = MissionShipControlVM.OarsmenStateVisual.Normal;
			}
			else if (level == 2)
			{
				this._activeOarsmenState = MissionShipControlVM.OarsmenStateVisual.Fast;
			}
			else
			{
				Debug.FailedAssert(string.Format("Invalid oarsman state: {0}", level), "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC.ViewModelCollection\\Missions\\ShipControl\\MissionShipControlVM.cs", "SetOarsmanLevel", 157);
			}
			this.OarsmenState = this._activeOarsmenState.ToString();
		}

		// Token: 0x0600037E RID: 894 RVA: 0x000124BC File Offset: 0x000106BC
		public void SetSailType(bool hasLateenSail, bool hasSquareSail)
		{
			if (hasLateenSail && hasSquareSail)
			{
				this._activeSailType = MissionShipControlVM.SailTypeVisual.Hybrid;
			}
			else if (hasLateenSail)
			{
				this._activeSailType = MissionShipControlVM.SailTypeVisual.Lateen;
			}
			else
			{
				this._activeSailType = MissionShipControlVM.SailTypeVisual.Square;
			}
			this.SailType = (int)this._activeSailType;
		}

		// Token: 0x0600037F RID: 895 RVA: 0x000124EB File Offset: 0x000106EB
		private static MissionShipControlVM.SailStateVisual GetSailVisual(SailInput input)
		{
			switch (input)
			{
			case SailInput.Raised:
				return MissionShipControlVM.SailStateVisual.Raised;
			case SailInput.SquareSailsRaised:
				return MissionShipControlVM.SailStateVisual.SquareSailsRaised;
			case SailInput.Full:
				return MissionShipControlVM.SailStateVisual.Full;
			default:
				return MissionShipControlVM.SailStateVisual.Invalid;
			}
		}

		// Token: 0x06000380 RID: 896 RVA: 0x00012508 File Offset: 0x00010708
		private void OnTutorialNotificationElementIDChanged(TutorialNotificationElementChangeEvent obj)
		{
			this.IsSailHighlightActive = obj.NewNotificationElementID == "SailToggle";
			this.IsOarsmenHighlightActive = obj.NewNotificationElementID == "OarsmenToggle";
		}

		// Token: 0x06000381 RID: 897 RVA: 0x00012536 File Offset: 0x00010736
		public void SetChangeCameraKey(GameKey gameKey)
		{
			this.ChangeCameraKey = new MissionShipControlVM.ShipControlInputKeyItemVM(InputKeyItemVM.CreateFromGameKey(gameKey, false));
		}

		// Token: 0x06000382 RID: 898 RVA: 0x0001254A File Offset: 0x0001074A
		public void SetToggleSailKey(GameKey gameKey)
		{
			this.ToggleSailKey = new MissionShipControlVM.ShipControlInputKeyItemVM(InputKeyItemVM.CreateFromGameKey(gameKey, false));
		}

		// Token: 0x06000383 RID: 899 RVA: 0x0001255E File Offset: 0x0001075E
		public void SetCutLooseKey(GameKey gameKey)
		{
			this.CutLooseKey = new MissionShipControlVM.ShipControlInputKeyItemVM(InputKeyItemVM.CreateFromGameKey(gameKey, false));
		}

		// Token: 0x06000384 RID: 900 RVA: 0x00012572 File Offset: 0x00010772
		public void SetToggleOarsmenKey(GameKey gameKey)
		{
			this.ToggleOarsmenKey = new MissionShipControlVM.ShipControlInputKeyItemVM(InputKeyItemVM.CreateFromGameKey(gameKey, false));
		}

		// Token: 0x06000385 RID: 901 RVA: 0x00012586 File Offset: 0x00010786
		public void SetToggleBallistaKey(GameKey gameKey)
		{
			this.ToggleBallistaKey = new MissionShipControlVM.ShipControlInputKeyItemVM(InputKeyItemVM.CreateFromGameKey(gameKey, false));
		}

		// Token: 0x06000386 RID: 902 RVA: 0x0001259A File Offset: 0x0001079A
		public void SetAttemptBoardingKey(GameKey gameKey)
		{
			this.AttemptBoardingKey = new MissionShipControlVM.ShipControlInputKeyItemVM(InputKeyItemVM.CreateFromGameKey(gameKey, false));
		}

		// Token: 0x06000387 RID: 903 RVA: 0x000125AE File Offset: 0x000107AE
		public void SetStopUsingShipKey(GameKey gameKey)
		{
			this.StopUsingShipKey = new MissionShipControlVM.ShipControlInputKeyItemVM(InputKeyItemVM.CreateFromGameKey(gameKey, false));
		}

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x06000388 RID: 904 RVA: 0x000125C2 File Offset: 0x000107C2
		// (set) Token: 0x06000389 RID: 905 RVA: 0x000125CA File Offset: 0x000107CA
		[DataSourceProperty]
		public MissionShipControlVM.ShipControlInputKeyItemVM ChangeCameraKey
		{
			get
			{
				return this._changeCameraKey;
			}
			set
			{
				if (value != this._changeCameraKey)
				{
					this._changeCameraKey = value;
					base.OnPropertyChangedWithValue<MissionShipControlVM.ShipControlInputKeyItemVM>(value, "ChangeCameraKey");
				}
			}
		}

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x0600038A RID: 906 RVA: 0x000125E8 File Offset: 0x000107E8
		// (set) Token: 0x0600038B RID: 907 RVA: 0x000125F0 File Offset: 0x000107F0
		[DataSourceProperty]
		public MissionShipControlVM.ShipControlInputKeyItemVM ToggleSailKey
		{
			get
			{
				return this._toggleSailKey;
			}
			set
			{
				if (value != this._toggleSailKey)
				{
					this._toggleSailKey = value;
					base.OnPropertyChangedWithValue<MissionShipControlVM.ShipControlInputKeyItemVM>(value, "ToggleSailKey");
				}
			}
		}

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x0600038C RID: 908 RVA: 0x0001260E File Offset: 0x0001080E
		// (set) Token: 0x0600038D RID: 909 RVA: 0x00012616 File Offset: 0x00010816
		[DataSourceProperty]
		public MissionShipControlVM.ShipControlInputKeyItemVM CutLooseKey
		{
			get
			{
				return this._cutLooseKey;
			}
			set
			{
				if (value != this._cutLooseKey)
				{
					this._cutLooseKey = value;
					base.OnPropertyChangedWithValue<MissionShipControlVM.ShipControlInputKeyItemVM>(value, "CutLooseKey");
				}
			}
		}

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x0600038E RID: 910 RVA: 0x00012634 File Offset: 0x00010834
		// (set) Token: 0x0600038F RID: 911 RVA: 0x0001263C File Offset: 0x0001083C
		[DataSourceProperty]
		public MissionShipControlVM.ShipControlInputKeyItemVM ToggleOarsmenKey
		{
			get
			{
				return this._toggleOarsmenKey;
			}
			set
			{
				if (value != this._toggleOarsmenKey)
				{
					this._toggleOarsmenKey = value;
					base.OnPropertyChangedWithValue<MissionShipControlVM.ShipControlInputKeyItemVM>(value, "ToggleOarsmenKey");
				}
			}
		}

		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x06000390 RID: 912 RVA: 0x0001265A File Offset: 0x0001085A
		// (set) Token: 0x06000391 RID: 913 RVA: 0x00012662 File Offset: 0x00010862
		[DataSourceProperty]
		public MissionShipControlVM.ShipControlInputKeyItemVM ToggleBallistaKey
		{
			get
			{
				return this._toggleBallistaKey;
			}
			set
			{
				if (value != this._toggleBallistaKey)
				{
					this._toggleBallistaKey = value;
					base.OnPropertyChangedWithValue<MissionShipControlVM.ShipControlInputKeyItemVM>(value, "ToggleBallistaKey");
				}
			}
		}

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x06000392 RID: 914 RVA: 0x00012680 File Offset: 0x00010880
		// (set) Token: 0x06000393 RID: 915 RVA: 0x00012688 File Offset: 0x00010888
		[DataSourceProperty]
		public MissionShipControlVM.ShipControlInputKeyItemVM AttemptBoardingKey
		{
			get
			{
				return this._attemptBoardingKey;
			}
			set
			{
				if (value != this._attemptBoardingKey)
				{
					this._attemptBoardingKey = value;
					base.OnPropertyChangedWithValue<MissionShipControlVM.ShipControlInputKeyItemVM>(value, "AttemptBoardingKey");
				}
			}
		}

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x06000394 RID: 916 RVA: 0x000126A6 File Offset: 0x000108A6
		// (set) Token: 0x06000395 RID: 917 RVA: 0x000126AE File Offset: 0x000108AE
		[DataSourceProperty]
		public MissionShipControlVM.ShipControlInputKeyItemVM StopUsingShipKey
		{
			get
			{
				return this._stopUsingShipKey;
			}
			set
			{
				if (value != this._stopUsingShipKey)
				{
					this._stopUsingShipKey = value;
					base.OnPropertyChangedWithValue<MissionShipControlVM.ShipControlInputKeyItemVM>(value, "StopUsingShipKey");
				}
			}
		}

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x06000396 RID: 918 RVA: 0x000126CC File Offset: 0x000108CC
		// (set) Token: 0x06000397 RID: 919 RVA: 0x000126D4 File Offset: 0x000108D4
		[DataSourceProperty]
		public bool IsControllingShip
		{
			get
			{
				return this._isControllingShip;
			}
			set
			{
				if (value != this._isControllingShip)
				{
					this._isControllingShip = value;
					base.OnPropertyChangedWithValue(value, "IsControllingShip");
				}
			}
		}

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x06000398 RID: 920 RVA: 0x000126F2 File Offset: 0x000108F2
		// (set) Token: 0x06000399 RID: 921 RVA: 0x000126FA File Offset: 0x000108FA
		[DataSourceProperty]
		public bool IsUsingBallistaRemotely
		{
			get
			{
				return this._isUsingBallistaRemotely;
			}
			set
			{
				if (value != this._isUsingBallistaRemotely)
				{
					this._isUsingBallistaRemotely = value;
					base.OnPropertyChangedWithValue(value, "IsUsingBallistaRemotely");
				}
			}
		}

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x0600039A RID: 922 RVA: 0x00012718 File Offset: 0x00010918
		// (set) Token: 0x0600039B RID: 923 RVA: 0x00012720 File Offset: 0x00010920
		[DataSourceProperty]
		public bool IsUsingBallistaDirectly
		{
			get
			{
				return this._isUsingBallistaDirectly;
			}
			set
			{
				if (value != this._isUsingBallistaDirectly)
				{
					this._isUsingBallistaDirectly = value;
					base.OnPropertyChangedWithValue(value, "IsUsingBallistaDirectly");
				}
			}
		}

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x0600039C RID: 924 RVA: 0x0001273E File Offset: 0x0001093E
		// (set) Token: 0x0600039D RID: 925 RVA: 0x00012746 File Offset: 0x00010946
		[DataSourceProperty]
		public bool HasTargetedShip
		{
			get
			{
				return this._hasTargetedShip;
			}
			set
			{
				if (value != this._hasTargetedShip)
				{
					this._hasTargetedShip = value;
					base.OnPropertyChangedWithValue(value, "HasTargetedShip");
				}
			}
		}

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x0600039E RID: 926 RVA: 0x00012764 File Offset: 0x00010964
		// (set) Token: 0x0600039F RID: 927 RVA: 0x0001276C File Offset: 0x0001096C
		[DataSourceProperty]
		public bool IsTargetedShipPlayerTeam
		{
			get
			{
				return this._isTargetedShipPlayerTeam;
			}
			set
			{
				if (value != this._isTargetedShipPlayerTeam)
				{
					this._isTargetedShipPlayerTeam = value;
					base.OnPropertyChangedWithValue(value, "IsTargetedShipPlayerTeam");
				}
			}
		}

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x060003A0 RID: 928 RVA: 0x0001278A File Offset: 0x0001098A
		// (set) Token: 0x060003A1 RID: 929 RVA: 0x00012792 File Offset: 0x00010992
		[DataSourceProperty]
		public bool IsTargetedShipPlayerAllyTeam
		{
			get
			{
				return this._isTargetedShipPlayerAllyTeam;
			}
			set
			{
				if (value != this._isTargetedShipPlayerAllyTeam)
				{
					this._isTargetedShipPlayerAllyTeam = value;
					base.OnPropertyChangedWithValue(value, "IsTargetedShipPlayerAllyTeam");
				}
			}
		}

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x060003A2 RID: 930 RVA: 0x000127B0 File Offset: 0x000109B0
		// (set) Token: 0x060003A3 RID: 931 RVA: 0x000127B8 File Offset: 0x000109B8
		[DataSourceProperty]
		public bool IsTargetedShipEnemyTeam
		{
			get
			{
				return this._isTargetedShipEnemyTeam;
			}
			set
			{
				if (value != this._isTargetedShipEnemyTeam)
				{
					this._isTargetedShipEnemyTeam = value;
					base.OnPropertyChangedWithValue(value, "IsTargetedShipEnemyTeam");
				}
			}
		}

		// Token: 0x170000EE RID: 238
		// (get) Token: 0x060003A4 RID: 932 RVA: 0x000127D6 File Offset: 0x000109D6
		// (set) Token: 0x060003A5 RID: 933 RVA: 0x000127DE File Offset: 0x000109DE
		[DataSourceProperty]
		public bool TargetedShipHasAction
		{
			get
			{
				return this._targetedShipHasAction;
			}
			set
			{
				if (value != this._targetedShipHasAction)
				{
					this._targetedShipHasAction = value;
					base.OnPropertyChangedWithValue(value, "TargetedShipHasAction");
				}
			}
		}

		// Token: 0x170000EF RID: 239
		// (get) Token: 0x060003A6 RID: 934 RVA: 0x000127FC File Offset: 0x000109FC
		// (set) Token: 0x060003A7 RID: 935 RVA: 0x00012804 File Offset: 0x00010A04
		[DataSourceProperty]
		public bool IsSailHighlightActive
		{
			get
			{
				return this._isSailHighlightActive;
			}
			set
			{
				if (value != this._isSailHighlightActive)
				{
					this._isSailHighlightActive = value;
					base.OnPropertyChangedWithValue(value, "IsSailHighlightActive");
				}
			}
		}

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x060003A8 RID: 936 RVA: 0x00012822 File Offset: 0x00010A22
		// (set) Token: 0x060003A9 RID: 937 RVA: 0x0001282A File Offset: 0x00010A2A
		[DataSourceProperty]
		public bool IsOarsmenHighlightActive
		{
			get
			{
				return this._isOarsmenHighlightActive;
			}
			set
			{
				if (value != this._isOarsmenHighlightActive)
				{
					this._isOarsmenHighlightActive = value;
					base.OnPropertyChangedWithValue(value, "IsOarsmenHighlightActive");
				}
			}
		}

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x060003AA RID: 938 RVA: 0x00012848 File Offset: 0x00010A48
		// (set) Token: 0x060003AB RID: 939 RVA: 0x00012850 File Offset: 0x00010A50
		[DataSourceProperty]
		public MissionHitPointPropertiesVM ShipHitPoints
		{
			get
			{
				return this._shipHitPoints;
			}
			set
			{
				if (value != this._shipHitPoints)
				{
					this._shipHitPoints = value;
					base.OnPropertyChangedWithValue<MissionHitPointPropertiesVM>(value, "ShipHitPoints");
				}
			}
		}

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x060003AC RID: 940 RVA: 0x0001286E File Offset: 0x00010A6E
		// (set) Token: 0x060003AD RID: 941 RVA: 0x00012876 File Offset: 0x00010A76
		[DataSourceProperty]
		public MissionHitPointPropertiesVM SailHitPoints
		{
			get
			{
				return this._sailHitPoints;
			}
			set
			{
				if (value != this._sailHitPoints)
				{
					this._sailHitPoints = value;
					base.OnPropertyChangedWithValue<MissionHitPointPropertiesVM>(value, "SailHitPoints");
				}
			}
		}

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x060003AE RID: 942 RVA: 0x00012894 File Offset: 0x00010A94
		// (set) Token: 0x060003AF RID: 943 RVA: 0x0001289C File Offset: 0x00010A9C
		[DataSourceProperty]
		public Vec2 TargetedShipPosition
		{
			get
			{
				return this._targetedShipPosition;
			}
			set
			{
				if (value != this._targetedShipPosition)
				{
					this._targetedShipPosition = value;
					base.OnPropertyChangedWithValue(value, "TargetedShipPosition");
				}
			}
		}

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x060003B0 RID: 944 RVA: 0x000128BF File Offset: 0x00010ABF
		// (set) Token: 0x060003B1 RID: 945 RVA: 0x000128C7 File Offset: 0x00010AC7
		[DataSourceProperty]
		public Vec2 BoardingTargetShipPosition
		{
			get
			{
				return this._boardingTargetShipPosition;
			}
			set
			{
				if (value != this._boardingTargetShipPosition)
				{
					this._boardingTargetShipPosition = value;
					base.OnPropertyChangedWithValue(value, "BoardingTargetShipPosition");
				}
			}
		}

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x060003B2 RID: 946 RVA: 0x000128EA File Offset: 0x00010AEA
		// (set) Token: 0x060003B3 RID: 947 RVA: 0x000128F2 File Offset: 0x00010AF2
		[DataSourceProperty]
		public MissionHitPointPropertiesVM FireHitPoints
		{
			get
			{
				return this._fireHitPoints;
			}
			set
			{
				if (value != this._fireHitPoints)
				{
					this._fireHitPoints = value;
					base.OnPropertyChangedWithValue<MissionHitPointPropertiesVM>(value, "FireHitPoints");
				}
			}
		}

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x060003B4 RID: 948 RVA: 0x00012910 File Offset: 0x00010B10
		// (set) Token: 0x060003B5 RID: 949 RVA: 0x00012918 File Offset: 0x00010B18
		[DataSourceProperty]
		public int TargetedShipWSign
		{
			get
			{
				return this._targetedShipWSign;
			}
			set
			{
				if (value != this._targetedShipWSign)
				{
					this._targetedShipWSign = value;
					base.OnPropertyChangedWithValue(value, "TargetedShipWSign");
				}
			}
		}

		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x060003B6 RID: 950 RVA: 0x00012936 File Offset: 0x00010B36
		// (set) Token: 0x060003B7 RID: 951 RVA: 0x0001293E File Offset: 0x00010B3E
		[DataSourceProperty]
		public int BoardingTargetShipWSign
		{
			get
			{
				return this._boardingTargetShipWSign;
			}
			set
			{
				if (value != this._boardingTargetShipWSign)
				{
					this._boardingTargetShipWSign = value;
					base.OnPropertyChangedWithValue(value, "BoardingTargetShipWSign");
				}
			}
		}

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x060003B8 RID: 952 RVA: 0x0001295C File Offset: 0x00010B5C
		// (set) Token: 0x060003B9 RID: 953 RVA: 0x00012964 File Offset: 0x00010B64
		[DataSourceProperty]
		public string SailState
		{
			get
			{
				return this._sailState;
			}
			set
			{
				if (value != this._sailState)
				{
					this._sailState = value;
					base.OnPropertyChangedWithValue<string>(value, "SailState");
				}
			}
		}

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x060003BA RID: 954 RVA: 0x00012987 File Offset: 0x00010B87
		// (set) Token: 0x060003BB RID: 955 RVA: 0x0001298F File Offset: 0x00010B8F
		[DataSourceProperty]
		public string OarsmenState
		{
			get
			{
				return this._oarsmenState;
			}
			set
			{
				if (value != this._oarsmenState)
				{
					this._oarsmenState = value;
					base.OnPropertyChangedWithValue<string>(value, "OarsmenState");
				}
			}
		}

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x060003BC RID: 956 RVA: 0x000129B2 File Offset: 0x00010BB2
		// (set) Token: 0x060003BD RID: 957 RVA: 0x000129BA File Offset: 0x00010BBA
		[DataSourceProperty]
		public string CancelText
		{
			get
			{
				return this._cancelText;
			}
			set
			{
				if (value != this._cancelText)
				{
					this._cancelText = value;
					base.OnPropertyChangedWithValue<string>(value, "CancelText");
				}
			}
		}

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x060003BE RID: 958 RVA: 0x000129DD File Offset: 0x00010BDD
		// (set) Token: 0x060003BF RID: 959 RVA: 0x000129E5 File Offset: 0x00010BE5
		[DataSourceProperty]
		public Vec2 ProjectedWindDirection
		{
			get
			{
				return this._projectedWindDirection;
			}
			set
			{
				if (value != this._projectedWindDirection)
				{
					this._projectedWindDirection = value;
					base.OnPropertyChangedWithValue(value, "ProjectedWindDirection");
				}
			}
		}

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x060003C0 RID: 960 RVA: 0x00012A08 File Offset: 0x00010C08
		// (set) Token: 0x060003C1 RID: 961 RVA: 0x00012A10 File Offset: 0x00010C10
		[DataSourceProperty]
		public int SailType
		{
			get
			{
				return this._sailType;
			}
			set
			{
				if (value != this._sailType)
				{
					this._sailType = value;
					base.OnPropertyChangedWithValue(value, "SailType");
				}
			}
		}

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x060003C2 RID: 962 RVA: 0x00012A2E File Offset: 0x00010C2E
		// (set) Token: 0x060003C3 RID: 963 RVA: 0x00012A36 File Offset: 0x00010C36
		[DataSourceProperty]
		public int BallistaAmmoCount
		{
			get
			{
				return this._ballistaAmmoCount;
			}
			set
			{
				if (value != this._ballistaAmmoCount)
				{
					this._ballistaAmmoCount = value;
					base.OnPropertyChangedWithValue(value, "BallistaAmmoCount");
				}
			}
		}

		// Token: 0x170000FE RID: 254
		// (get) Token: 0x060003C4 RID: 964 RVA: 0x00012A54 File Offset: 0x00010C54
		// (set) Token: 0x060003C5 RID: 965 RVA: 0x00012A5C File Offset: 0x00010C5C
		[DataSourceProperty]
		public bool IsAmmoCountWarned
		{
			get
			{
				return this._isAmmoCountWarned;
			}
			set
			{
				if (value != this._isAmmoCountWarned)
				{
					this._isAmmoCountWarned = value;
					base.OnPropertyChangedWithValue(value, "IsAmmoCountWarned");
				}
			}
		}

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x060003C6 RID: 966 RVA: 0x00012A7A File Offset: 0x00010C7A
		// (set) Token: 0x060003C7 RID: 967 RVA: 0x00012A82 File Offset: 0x00010C82
		[DataSourceProperty]
		public bool IsCutLooseOrderActive
		{
			get
			{
				return this._isCutLooseOrderActive;
			}
			set
			{
				if (value != this._isCutLooseOrderActive)
				{
					this._isCutLooseOrderActive = value;
					base.OnPropertyChangedWithValue(value, "IsCutLooseOrderActive");
				}
			}
		}

		// Token: 0x17000100 RID: 256
		// (get) Token: 0x060003C8 RID: 968 RVA: 0x00012AA0 File Offset: 0x00010CA0
		// (set) Token: 0x060003C9 RID: 969 RVA: 0x00012AA8 File Offset: 0x00010CA8
		[DataSourceProperty]
		public bool IsAttemptBoardingOrderActive
		{
			get
			{
				return this._isAttemptBoardingOrderActive;
			}
			set
			{
				if (value != this._isAttemptBoardingOrderActive)
				{
					this._isAttemptBoardingOrderActive = value;
					base.OnPropertyChangedWithValue(value, "IsAttemptBoardingOrderActive");
				}
			}
		}

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x060003CA RID: 970 RVA: 0x00012AC6 File Offset: 0x00010CC6
		// (set) Token: 0x060003CB RID: 971 RVA: 0x00012ACE File Offset: 0x00010CCE
		[DataSourceProperty]
		public bool IsCancelBoardingOrderAvailable
		{
			get
			{
				return this._isCancelBoardingOrderAvailable;
			}
			set
			{
				if (value != this._isCancelBoardingOrderAvailable)
				{
					this._isCancelBoardingOrderAvailable = value;
					base.OnPropertyChangedWithValue(value, "IsCancelBoardingOrderAvailable");
				}
			}
		}

		// Token: 0x04000156 RID: 342
		public const int WindSailSegmentCount = 24;

		// Token: 0x04000157 RID: 343
		private MissionShipControlVM.OarsmenStateVisual _activeOarsmenState;

		// Token: 0x04000158 RID: 344
		private MissionShipControlVM.SailStateVisual _activeSailState;

		// Token: 0x04000159 RID: 345
		private MissionShipControlVM.SailTypeVisual _activeSailType;

		// Token: 0x0400015A RID: 346
		private MissionShipControlVM.ShipControlInputKeyItemVM _changeCameraKey;

		// Token: 0x0400015B RID: 347
		private MissionShipControlVM.ShipControlInputKeyItemVM _toggleSailKey;

		// Token: 0x0400015C RID: 348
		private MissionShipControlVM.ShipControlInputKeyItemVM _cutLooseKey;

		// Token: 0x0400015D RID: 349
		private MissionShipControlVM.ShipControlInputKeyItemVM _toggleOarsmenKey;

		// Token: 0x0400015E RID: 350
		private MissionShipControlVM.ShipControlInputKeyItemVM _toggleBallistaKey;

		// Token: 0x0400015F RID: 351
		private MissionShipControlVM.ShipControlInputKeyItemVM _attemptBoardingKey;

		// Token: 0x04000160 RID: 352
		private MissionShipControlVM.ShipControlInputKeyItemVM _stopUsingShipKey;

		// Token: 0x04000161 RID: 353
		private bool _isControllingShip;

		// Token: 0x04000162 RID: 354
		private bool _isUsingBallistaRemotely;

		// Token: 0x04000163 RID: 355
		private bool _isUsingBallistaDirectly;

		// Token: 0x04000164 RID: 356
		private bool _hasTargetedShip;

		// Token: 0x04000165 RID: 357
		private bool _isTargetedShipPlayerTeam;

		// Token: 0x04000166 RID: 358
		private bool _isTargetedShipPlayerAllyTeam;

		// Token: 0x04000167 RID: 359
		private bool _isTargetedShipEnemyTeam;

		// Token: 0x04000168 RID: 360
		private bool _targetedShipHasAction;

		// Token: 0x04000169 RID: 361
		private Vec2 _targetedShipPosition;

		// Token: 0x0400016A RID: 362
		private Vec2 _boardingTargetShipPosition;

		// Token: 0x0400016B RID: 363
		private bool _isSailHighlightActive;

		// Token: 0x0400016C RID: 364
		private bool _isOarsmenHighlightActive;

		// Token: 0x0400016D RID: 365
		private int _targetedShipWSign;

		// Token: 0x0400016E RID: 366
		private int _boardingTargetShipWSign;

		// Token: 0x0400016F RID: 367
		private string _sailState;

		// Token: 0x04000170 RID: 368
		private string _oarsmenState;

		// Token: 0x04000171 RID: 369
		private string _cancelText;

		// Token: 0x04000172 RID: 370
		private int _sailType;

		// Token: 0x04000173 RID: 371
		private Vec2 _projectedWindDirection;

		// Token: 0x04000174 RID: 372
		private int _ballistaAmmoCount;

		// Token: 0x04000175 RID: 373
		private bool _isAmmoCountWarned;

		// Token: 0x04000176 RID: 374
		private bool _isCutLooseOrderActive;

		// Token: 0x04000177 RID: 375
		private bool _isAttemptBoardingOrderActive;

		// Token: 0x04000178 RID: 376
		private bool _isCancelBoardingOrderAvailable;

		// Token: 0x04000179 RID: 377
		private MissionHitPointPropertiesVM _shipHitPoints;

		// Token: 0x0400017A RID: 378
		private MissionHitPointPropertiesVM _sailHitPoints;

		// Token: 0x0400017B RID: 379
		private MissionHitPointPropertiesVM _fireHitPoints;

		// Token: 0x02000070 RID: 112
		private enum SailStateVisual
		{
			// Token: 0x04000221 RID: 545
			Invalid = -1,
			// Token: 0x04000222 RID: 546
			Raised,
			// Token: 0x04000223 RID: 547
			SquareSailsRaised,
			// Token: 0x04000224 RID: 548
			Full
		}

		// Token: 0x02000071 RID: 113
		private enum OarsmenStateVisual
		{
			// Token: 0x04000226 RID: 550
			Invalid = -1,
			// Token: 0x04000227 RID: 551
			Idle,
			// Token: 0x04000228 RID: 552
			Normal,
			// Token: 0x04000229 RID: 553
			Fast
		}

		// Token: 0x02000072 RID: 114
		private enum SailTypeVisual
		{
			// Token: 0x0400022B RID: 555
			Square,
			// Token: 0x0400022C RID: 556
			Lateen,
			// Token: 0x0400022D RID: 557
			Hybrid
		}

		// Token: 0x02000073 RID: 115
		public class ShipControlInputKeyItemVM : ViewModel
		{
			// Token: 0x0600050C RID: 1292 RVA: 0x00015139 File Offset: 0x00013339
			public ShipControlInputKeyItemVM(InputKeyItemVM key)
			{
				this.Key = key;
				this.RefreshValues();
			}

			// Token: 0x0600050D RID: 1293 RVA: 0x0001514E File Offset: 0x0001334E
			public override void RefreshValues()
			{
				base.RefreshValues();
				this.Key.RefreshValues();
			}

			// Token: 0x0600050E RID: 1294 RVA: 0x00015161 File Offset: 0x00013361
			public override void OnFinalize()
			{
				base.OnFinalize();
				this.Key.OnFinalize();
			}

			// Token: 0x17000128 RID: 296
			// (get) Token: 0x0600050F RID: 1295 RVA: 0x00015174 File Offset: 0x00013374
			// (set) Token: 0x06000510 RID: 1296 RVA: 0x0001517C File Offset: 0x0001337C
			[DataSourceProperty]
			public bool IsVisible
			{
				get
				{
					return this._isVisible;
				}
				set
				{
					if (value != this._isVisible)
					{
						this._isVisible = value;
						base.OnPropertyChangedWithValue(value, "IsVisible");
					}
				}
			}

			// Token: 0x17000129 RID: 297
			// (get) Token: 0x06000511 RID: 1297 RVA: 0x0001519A File Offset: 0x0001339A
			// (set) Token: 0x06000512 RID: 1298 RVA: 0x000151A2 File Offset: 0x000133A2
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

			// Token: 0x1700012A RID: 298
			// (get) Token: 0x06000513 RID: 1299 RVA: 0x000151C0 File Offset: 0x000133C0
			// (set) Token: 0x06000514 RID: 1300 RVA: 0x000151C8 File Offset: 0x000133C8
			[DataSourceProperty]
			public InputKeyItemVM Key
			{
				get
				{
					return this._key;
				}
				set
				{
					if (value != this._key)
					{
						this._key = value;
						base.OnPropertyChangedWithValue<InputKeyItemVM>(value, "Key");
					}
				}
			}

			// Token: 0x0400022E RID: 558
			private bool _isVisible;

			// Token: 0x0400022F RID: 559
			private bool _isDisabled;

			// Token: 0x04000230 RID: 560
			private InputKeyItemVM _key;
		}
	}
}
