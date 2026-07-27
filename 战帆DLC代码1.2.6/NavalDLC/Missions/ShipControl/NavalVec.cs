using System;
using TaleWorlds.Library;

namespace NavalDLC.Missions.ShipControl
{
	// Token: 0x0200008D RID: 141
	public struct NavalVec
	{
		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x06000A04 RID: 2564 RVA: 0x00046699 File Offset: 0x00044899
		public Vec2 DeltaPosition
		{
			get
			{
				return this._deltaPosition;
			}
		}

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x06000A05 RID: 2565 RVA: 0x000466A1 File Offset: 0x000448A1
		public float DeltaOrientation
		{
			get
			{
				return this._deltaOrientation;
			}
		}

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x06000A06 RID: 2566 RVA: 0x000466A9 File Offset: 0x000448A9
		public float DeltaSpeed
		{
			get
			{
				return this._deltaSpeed;
			}
		}

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x06000A07 RID: 2567 RVA: 0x000466B1 File Offset: 0x000448B1
		public static NavalVec Zero
		{
			get
			{
				return new NavalVec(in Vec2.Zero, 0f, 0f);
			}
		}

		// Token: 0x06000A08 RID: 2568 RVA: 0x000466C7 File Offset: 0x000448C7
		public NavalVec(in Vec2 deltaPosition, float deltaRotation, float deltaSpeed = 0f)
		{
			this._deltaPosition = deltaPosition;
			this._deltaOrientation = deltaRotation;
			this._deltaSpeed = deltaSpeed;
		}

		// Token: 0x06000A09 RID: 2569 RVA: 0x000466E3 File Offset: 0x000448E3
		public NavalVec(in Vec2 deltaPosition)
		{
			this._deltaPosition = deltaPosition;
			this._deltaOrientation = 0f;
			this._deltaSpeed = 0f;
		}

		// Token: 0x06000A0A RID: 2570 RVA: 0x00046707 File Offset: 0x00044907
		public void ClampAngle()
		{
			this._deltaOrientation = MathF.Clamp(this._deltaOrientation, -3.1415927f, 3.1415927f);
		}

		// Token: 0x06000A0B RID: 2571 RVA: 0x00046724 File Offset: 0x00044924
		public static NavalVec operator +(in NavalVec vec1, in NavalVec vec2)
		{
			NavalVec navalVec = vec1;
			Vec2 deltaPosition = navalVec.DeltaPosition;
			navalVec = vec2;
			Vec2 vec3 = deltaPosition + navalVec.DeltaPosition;
			navalVec = vec1;
			float deltaOrientation = navalVec.DeltaOrientation;
			navalVec = vec2;
			float num = deltaOrientation + navalVec.DeltaOrientation;
			navalVec = vec1;
			float deltaSpeed = navalVec.DeltaSpeed;
			navalVec = vec2;
			return new NavalVec(in vec3, num, deltaSpeed + navalVec.DeltaSpeed);
		}

		// Token: 0x06000A0C RID: 2572 RVA: 0x00046794 File Offset: 0x00044994
		public static NavalVec operator -(in NavalVec vec1, in NavalVec vec2)
		{
			NavalVec navalVec = vec1;
			Vec2 deltaPosition = navalVec.DeltaPosition;
			navalVec = vec2;
			Vec2 vec3 = deltaPosition - navalVec.DeltaPosition;
			navalVec = vec1;
			float deltaOrientation = navalVec.DeltaOrientation;
			navalVec = vec2;
			float num = deltaOrientation - navalVec.DeltaOrientation;
			navalVec = vec1;
			float deltaSpeed = navalVec.DeltaSpeed;
			navalVec = vec2;
			return new NavalVec(in vec3, num, deltaSpeed - navalVec.DeltaSpeed);
		}

		// Token: 0x06000A0D RID: 2573 RVA: 0x00046804 File Offset: 0x00044A04
		public static NavalVec operator *(in NavalVec vector, float scalar)
		{
			NavalVec navalVec = vector;
			Vec2 vec = navalVec.DeltaPosition * scalar;
			navalVec = vector;
			float num = navalVec.DeltaOrientation * scalar;
			navalVec = vector;
			return new NavalVec(in vec, num, navalVec.DeltaSpeed * scalar);
		}

		// Token: 0x06000A0E RID: 2574 RVA: 0x00046850 File Offset: 0x00044A50
		public static NavalVec operator *(float scalar, in NavalVec vector)
		{
			NavalVec navalVec = vector;
			Vec2 vec = scalar * navalVec.DeltaPosition;
			navalVec = vector;
			float num = scalar * navalVec.DeltaOrientation;
			navalVec = vector;
			return new NavalVec(in vec, num, scalar * navalVec.DeltaSpeed);
		}

		// Token: 0x06000A0F RID: 2575 RVA: 0x0004689C File Offset: 0x00044A9C
		public static NavalVec operator *(in Vec3 vector, in NavalVec nVector)
		{
			float x = vector.x;
			NavalVec navalVec = nVector;
			Vec2 vec = x * navalVec.DeltaPosition;
			float y = vector.y;
			navalVec = nVector;
			float num = y * navalVec.DeltaOrientation;
			float z = vector.z;
			navalVec = nVector;
			return new NavalVec(in vec, num, z * navalVec.DeltaSpeed);
		}

		// Token: 0x06000A10 RID: 2576 RVA: 0x000468F4 File Offset: 0x00044AF4
		public static NavalVec operator *(in NavalVec nVector, in Vec3 vector)
		{
			NavalVec navalVec = nVector;
			Vec2 vec = navalVec.DeltaPosition * vector.x;
			navalVec = nVector;
			float num = navalVec.DeltaOrientation * vector.y;
			navalVec = nVector;
			return new NavalVec(in vec, num, navalVec.DeltaSpeed * vector.z);
		}

		// Token: 0x06000A11 RID: 2577 RVA: 0x0004694C File Offset: 0x00044B4C
		public static NavalVec operator /(in NavalVec vector, float scalar)
		{
			NavalVec navalVec = vector;
			Vec2 vec = navalVec.DeltaPosition / scalar;
			navalVec = vector;
			float num = navalVec.DeltaOrientation / scalar;
			navalVec = vector;
			return new NavalVec(in vec, num, navalVec.DeltaSpeed / scalar);
		}

		// Token: 0x040005CD RID: 1485
		private Vec2 _deltaPosition;

		// Token: 0x040005CE RID: 1486
		private float _deltaOrientation;

		// Token: 0x040005CF RID: 1487
		private float _deltaSpeed;
	}
}
