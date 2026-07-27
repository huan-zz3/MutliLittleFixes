using System;
using System.Collections.Generic;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace NavalDLC.View.MissionViews
{
	// Token: 0x02000024 RID: 36
	public class NavalShipTargetSelectionHandler : MissionView
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x060000E0 RID: 224 RVA: 0x00007634 File Offset: 0x00005834
		// (remove) Token: 0x060000E1 RID: 225 RVA: 0x0000766C File Offset: 0x0000586C
		public event Action<MBReadOnlyList<MissionShip>> OnShipsFocused;

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x060000E2 RID: 226 RVA: 0x000076A1 File Offset: 0x000058A1
		private Camera ActiveCamera
		{
			get
			{
				return base.MissionScreen.CustomCamera ?? base.MissionScreen.CombatCamera;
			}
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x000076C0 File Offset: 0x000058C0
		public override void OnPreDisplayMissionTick(float dt)
		{
			base.OnPreDisplayMissionTick(dt);
			this._distanceCache.Clear();
			this._focusedShipsCache.Clear();
			this._enemyShipsCache.Clear();
			NavalShipsLogic missionBehavior = base.Mission.GetMissionBehavior<NavalShipsLogic>();
			if (missionBehavior == null)
			{
				return;
			}
			if (!this._isTargetingDisabled)
			{
				missionBehavior.FillTeamShips(2, this._enemyShipsCache);
				Vec3 position = this.ActiveCamera.Position;
				this._centerOfScreen.x = Screen.RealScreenResolutionWidth / 2f;
				this._centerOfScreen.y = Screen.RealScreenResolutionHeight / 2f;
				for (int i = 0; i < this._enemyShipsCache.Count; i++)
				{
					MissionShip missionShip = this._enemyShipsCache[i];
					float shipDistanceToCenter = this.GetShipDistanceToCenter(missionShip, position);
					this._distanceCache.Add(new ValueTuple<MissionShip, float>(missionShip, shipDistanceToCenter));
				}
			}
			if (this._distanceCache.Count == 0)
			{
				Action<MBReadOnlyList<MissionShip>> onShipsFocused = this.OnShipsFocused;
				if (onShipsFocused == null)
				{
					return;
				}
				onShipsFocused(null);
				return;
			}
			else
			{
				MissionShip missionShip2 = null;
				float num = this.MaxDistanceToCenterForFocus;
				for (int j = 0; j < this._distanceCache.Count; j++)
				{
					ValueTuple<MissionShip, float> valueTuple = this._distanceCache[j];
					if (valueTuple.Item2 == 0f)
					{
						this._focusedShipsCache.Add(valueTuple.Item1);
					}
					else if (valueTuple.Item2 < num)
					{
						num = valueTuple.Item2;
						missionShip2 = valueTuple.Item1;
					}
				}
				if (missionShip2 != null)
				{
					this._focusedShipsCache.Add(missionShip2);
				}
				Action<MBReadOnlyList<MissionShip>> onShipsFocused2 = this.OnShipsFocused;
				if (onShipsFocused2 == null)
				{
					return;
				}
				onShipsFocused2(this._focusedShipsCache);
				return;
			}
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x00007850 File Offset: 0x00005A50
		private float GetShipDistanceToCenter(MissionShip ship, Vec3 cameraPosition)
		{
			Vec3 origin = ship.GlobalFrame.origin;
			float num = origin.AsVec2.Distance(cameraPosition.AsVec2);
			if (num >= 1000f)
			{
				return 2.1474836E+09f;
			}
			if (num <= 10f)
			{
				return 0f;
			}
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			MBWindowManager.WorldToScreenInsideUsableArea(this.ActiveCamera, origin + Vec3.Up * 3f, ref num2, ref num3, ref num4);
			if (num4 <= 0f)
			{
				return 2.1474836E+09f;
			}
			return new Vec2(num2, num3).Distance(this._centerOfScreen);
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x000078FC File Offset: 0x00005AFC
		public void SetIsFormationTargetingDisabled(bool isDisabled)
		{
			if (this._isTargetingDisabled != isDisabled)
			{
				this._isTargetingDisabled = isDisabled;
				if (isDisabled)
				{
					this._distanceCache.Clear();
					this._enemyShipsCache.Clear();
					this._focusedShipsCache.Clear();
					Action<MBReadOnlyList<MissionShip>> onShipsFocused = this.OnShipsFocused;
					if (onShipsFocused == null)
					{
						return;
					}
					onShipsFocused(null);
				}
			}
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x0000794E File Offset: 0x00005B4E
		public override void OnRemoveBehavior()
		{
			this._distanceCache.Clear();
			this._focusedShipsCache.Clear();
			this.OnShipsFocused = null;
			base.OnRemoveBehavior();
		}

		// Token: 0x04000051 RID: 81
		public const float MaxDistanceForFocusCheck = 1000f;

		// Token: 0x04000052 RID: 82
		public const float MinDistanceForFocusCheck = 10f;

		// Token: 0x04000053 RID: 83
		public readonly float MaxDistanceToCenterForFocus = 70f * (Screen.RealScreenResolutionHeight / 1080f);

		// Token: 0x04000054 RID: 84
		private readonly List<ValueTuple<MissionShip, float>> _distanceCache = new List<ValueTuple<MissionShip, float>>();

		// Token: 0x04000055 RID: 85
		private readonly MBList<MissionShip> _focusedShipsCache = new MBList<MissionShip>();

		// Token: 0x04000056 RID: 86
		private readonly MBList<MissionShip> _enemyShipsCache = new MBList<MissionShip>();

		// Token: 0x04000057 RID: 87
		private Vec2 _centerOfScreen = new Vec2(Screen.RealScreenResolutionWidth / 2f, Screen.RealScreenResolutionHeight / 2f);

		// Token: 0x04000058 RID: 88
		private bool _isTargetingDisabled;
	}
}
