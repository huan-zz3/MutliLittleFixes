using System;
using TaleWorlds.Library;

namespace NavalDLC.Missions.ShipControl
{
	// Token: 0x0200008C RID: 140
	public struct NavalState
	{
		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x060009F8 RID: 2552 RVA: 0x00046490 File Offset: 0x00044690
		public Vec2 Position
		{
			get
			{
				return this._position;
			}
		}

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x060009F9 RID: 2553 RVA: 0x00046498 File Offset: 0x00044698
		public float Orientation
		{
			get
			{
				return this._orientation;
			}
		}

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x060009FA RID: 2554 RVA: 0x000464A0 File Offset: 0x000446A0
		public Vec2 Direction
		{
			get
			{
				return Vec2.FromRotation(this._orientation);
			}
		}

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x060009FB RID: 2555 RVA: 0x000464AD File Offset: 0x000446AD
		public float Speed
		{
			get
			{
				return this._speed;
			}
		}

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x060009FC RID: 2556 RVA: 0x000464B5 File Offset: 0x000446B5
		public static NavalState Zero
		{
			get
			{
				return new NavalState(in Vec2.Zero, 0f, 0f);
			}
		}

		// Token: 0x060009FD RID: 2557 RVA: 0x000464CB File Offset: 0x000446CB
		public NavalState(in Vec2 position, float orientation, float speed = 0f)
		{
			this._position = position;
			this._orientation = MBMath.WrapAngle(orientation);
			this._speed = speed;
		}

		// Token: 0x060009FE RID: 2558 RVA: 0x000464EC File Offset: 0x000446EC
		public NavalState(in Vec2 position, in Vec2 direction, float speed = 0f)
		{
			this._position = position;
			Vec2 vec = direction;
			this._orientation = vec.RotationInRadians;
			this._speed = speed;
		}

		// Token: 0x060009FF RID: 2559 RVA: 0x00046520 File Offset: 0x00044720
		public NavalState(in Vec2 position)
		{
			this._position = position;
			this._orientation = 0f;
			this._speed = 0f;
		}

		// Token: 0x06000A00 RID: 2560 RVA: 0x00046544 File Offset: 0x00044744
		public static NavalState operator +(in NavalState state, in NavalVec vector)
		{
			Vec2 position = state._position;
			NavalVec navalVec = vector;
			Vec2 vec = position + navalVec.DeltaPosition;
			float orientation = state._orientation;
			navalVec = vector;
			float num = orientation + navalVec.DeltaOrientation;
			NavalState navalState = state;
			float speed = navalState.Speed;
			navalVec = vector;
			float num2 = speed + navalVec.DeltaSpeed;
			return new NavalState(in vec, num, num2);
		}

		// Token: 0x06000A01 RID: 2561 RVA: 0x000465AC File Offset: 0x000447AC
		public static NavalState operator -(in NavalState state, in NavalVec vector)
		{
			Vec2 position = state._position;
			NavalVec navalVec = vector;
			Vec2 vec = position - navalVec.DeltaPosition;
			float orientation = state._orientation;
			navalVec = vector;
			float num = orientation - navalVec.DeltaOrientation;
			NavalState navalState = state;
			float speed = navalState.Speed;
			navalVec = vector;
			float num2 = speed - navalVec.DeltaSpeed;
			return new NavalState(in vec, num, num2);
		}

		// Token: 0x06000A02 RID: 2562 RVA: 0x00046614 File Offset: 0x00044814
		public static NavalVec operator -(in NavalState toState, in NavalState fromState)
		{
			Vec2 vec = toState._position - fromState._position;
			float smallestDifferenceBetweenTwoAngles = MBMath.GetSmallestDifferenceBetweenTwoAngles(MBMath.WrapAngle(fromState._orientation), MBMath.WrapAngle(toState._orientation));
			NavalState navalState = toState;
			float speed = navalState.Speed;
			navalState = fromState;
			float num = speed - navalState.Speed;
			return new NavalVec(in vec, smallestDifferenceBetweenTwoAngles, num);
		}

		// Token: 0x06000A03 RID: 2563 RVA: 0x00046678 File Offset: 0x00044878
		public void SetTargetDirection(in Vec2 targetDirection)
		{
			Vec2 vec = targetDirection;
			this._orientation = vec.RotationInRadians;
		}

		// Token: 0x040005CA RID: 1482
		private Vec2 _position;

		// Token: 0x040005CB RID: 1483
		private float _orientation;

		// Token: 0x040005CC RID: 1484
		private float _speed;
	}
}
