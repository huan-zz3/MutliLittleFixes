using System;
using System.Runtime.CompilerServices;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace NavalDLC.Missions.ShipActuators
{
	// Token: 0x02000095 RID: 149
	public class SailWindProfile
	{
		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x06000A9E RID: 2718 RVA: 0x00049ED8 File Offset: 0x000480D8
		public static SailWindProfile Instance
		{
			get
			{
				return SailWindProfile._instance;
			}
		}

		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x06000A9F RID: 2719 RVA: 0x00049EDF File Offset: 0x000480DF
		public static bool IsSailWindProfileInitialized
		{
			get
			{
				return SailWindProfile._instance != null;
			}
		}

		// Token: 0x06000AA0 RID: 2720 RVA: 0x00049EE9 File Offset: 0x000480E9
		public static void InitializeProfile()
		{
			SailWindProfile._instance = new SailWindProfile();
		}

		// Token: 0x06000AA1 RID: 2721 RVA: 0x00049EF5 File Offset: 0x000480F5
		public static void InitializeProfileForEditor()
		{
			if (SailWindProfile._instance == null)
			{
				SailWindProfile._instance = new SailWindProfile();
			}
		}

		// Token: 0x06000AA2 RID: 2722 RVA: 0x00049F08 File Offset: 0x00048108
		public static void FinalizeProfile()
		{
			SailWindProfile._instance.Destroy();
			SailWindProfile._instance = null;
		}

		// Token: 0x06000AA3 RID: 2723 RVA: 0x00049F1C File Offset: 0x0004811C
		private void FillSailProfiles()
		{
			ValueTuple<float, float>[] array = this.GenerateSquareSailWindProfile();
			this._sailWindProfiles[0] = array;
			ValueTuple<float, float>[] array2 = this.GenerateLateenSailWindProfile();
			this._sailWindProfiles[1] = array2;
		}

		// Token: 0x06000AA4 RID: 2724 RVA: 0x00049F49 File Offset: 0x00048149
		private SailWindProfile()
		{
			this._sailWindProfiles = new ValueTuple<float, float>[2][];
			this.FillSailProfiles();
		}

		// Token: 0x06000AA5 RID: 2725 RVA: 0x00049F64 File Offset: 0x00048164
		private void Destroy()
		{
			for (int i = 0; i < 2; i++)
			{
				this._sailWindProfiles[i] = null;
			}
			this._sailWindProfiles = null;
		}

		// Token: 0x06000AA6 RID: 2726 RVA: 0x00049F8D File Offset: 0x0004818D
		public float ComputeSailThrustValue(SailType sailType, Vec2 sailDir, Vec2 desiredThrustDir, Vec2 windDir)
		{
			return Vec2.DotProduct(this.GetSailForceCoefficients(sailType, sailDir, windDir), desiredThrustDir);
		}

		// Token: 0x06000AA7 RID: 2727 RVA: 0x00049FA0 File Offset: 0x000481A0
		public Vec2 GetMaximumSailForceCoefficients(SailType sailType)
		{
			float num = -3.1415927f;
			float num2 = -3.1415927f;
			Vec2 vec;
			vec..ctor(0f, 0f);
			float num3 = 0.17453292f;
			for (int i = 0; i < 36; i++)
			{
				Vec2 vec2;
				vec2..ctor(MathF.Cos(num), MathF.Sin(num));
				for (int j = 0; j < 36; j++)
				{
					Vec2 vec3;
					vec3..ctor(MathF.Cos(num2), MathF.Sin(num2));
					float angleOfAttack = SailWindProfile.GetAngleOfAttack(in vec2, in vec3);
					ValueTuple<float, float> sailCoefs = this.GetSailCoefs(angleOfAttack, sailType);
					Vec2 vec4 = vec3.LeftVec();
					Vec2 vec5 = vec3 * sailCoefs.Item1 + vec4 * sailCoefs.Item2;
					if (vec5.LengthSquared >= vec.LengthSquared)
					{
						vec = vec5;
					}
					num2 += num3;
				}
				num += num3;
			}
			return vec;
		}

		// Token: 0x06000AA8 RID: 2728 RVA: 0x0004A07C File Offset: 0x0004827C
		public Vec2 GetSailForceCoefficients(SailType sailType, Vec2 sailDir, Vec2 windDir)
		{
			float angleOfAttack = SailWindProfile.GetAngleOfAttack(in sailDir, in windDir);
			ValueTuple<float, float> sailCoefs = this.GetSailCoefs(angleOfAttack, sailType);
			Vec2 vec = windDir.LeftVec();
			return windDir * sailCoefs.Item1 + vec * sailCoefs.Item2;
		}

		// Token: 0x06000AA9 RID: 2729 RVA: 0x0004A0C4 File Offset: 0x000482C4
		[return: TupleElementNames(new string[] { "dragCoef", "liftCoef" })]
		public ValueTuple<float, float> GetSailCoefs(float angleOfAttackInRadians, SailType sailType)
		{
			float num = ((angleOfAttackInRadians < 0f) ? (angleOfAttackInRadians + 6.2831855f) : angleOfAttackInRadians) * 57.29578f;
			int num2 = (int)(num / 10f) % 36;
			int num3 = (num2 + 1) % 36;
			ValueTuple<float, float>[] array = this._sailWindProfiles[sailType];
			float num4 = num % 10f / 10f;
			float num5 = (1f - num4) * array[num2].Item1 + num4 * array[num3].Item1;
			float num6 = (1f - num4) * array[num2].Item2 + num4 * array[num3].Item2;
			return new ValueTuple<float, float>(num5, num6);
		}

		// Token: 0x06000AAA RID: 2730 RVA: 0x0004A164 File Offset: 0x00048364
		[return: TupleElementNames(new string[] { "dragCoef", "liftCoef" })]
		private ValueTuple<float, float>[] GenerateLateenSailWindProfile()
		{
			return new ValueTuple<float, float>[]
			{
				new ValueTuple<float, float>(0.02f, 0f),
				new ValueTuple<float, float>(0.06f, 0.08f),
				new ValueTuple<float, float>(0.08f, 0.12f),
				new ValueTuple<float, float>(0.12f, 0.1f),
				new ValueTuple<float, float>(0.13f, 0.08f),
				new ValueTuple<float, float>(0.17f, 0.06f),
				new ValueTuple<float, float>(0.28f, 0.04f),
				new ValueTuple<float, float>(0.41f, 0.03f),
				new ValueTuple<float, float>(0.46f, 0.02f),
				new ValueTuple<float, float>(0.6f, 0f),
				new ValueTuple<float, float>(0.46f, -0.02f),
				new ValueTuple<float, float>(0.41f, -0.03f),
				new ValueTuple<float, float>(0.28f, -0.04f),
				new ValueTuple<float, float>(0.17f, -0.06f),
				new ValueTuple<float, float>(0.13f, -0.08f),
				new ValueTuple<float, float>(0.12f, -0.1f),
				new ValueTuple<float, float>(0.08f, -0.12f),
				new ValueTuple<float, float>(0.06f, -0.08f),
				new ValueTuple<float, float>(0.02f, 0f),
				new ValueTuple<float, float>(0.06f, 0.12f),
				new ValueTuple<float, float>(0.08f, 0.38f),
				new ValueTuple<float, float>(0.14f, 0.36f),
				new ValueTuple<float, float>(0.26f, 0.24f),
				new ValueTuple<float, float>(0.34f, 0.16f),
				new ValueTuple<float, float>(0.56f, 0.12f),
				new ValueTuple<float, float>(0.82f, 0.09f),
				new ValueTuple<float, float>(0.92f, 0.03f),
				new ValueTuple<float, float>(1f, 0f),
				new ValueTuple<float, float>(0.92f, -0.03f),
				new ValueTuple<float, float>(0.82f, -0.09f),
				new ValueTuple<float, float>(0.56f, -0.12f),
				new ValueTuple<float, float>(0.34f, -0.16f),
				new ValueTuple<float, float>(0.26f, -0.24f),
				new ValueTuple<float, float>(0.14f, -0.36f),
				new ValueTuple<float, float>(0.08f, -0.38f),
				new ValueTuple<float, float>(0.06f, -0.12f)
			};
		}

		// Token: 0x06000AAB RID: 2731 RVA: 0x0004A4AC File Offset: 0x000486AC
		[return: TupleElementNames(new string[] { "dragCoef", "liftCoef" })]
		private ValueTuple<float, float>[] GenerateSquareSailWindProfile()
		{
			return new ValueTuple<float, float>[]
			{
				new ValueTuple<float, float>(1f, 0f),
				new ValueTuple<float, float>(0.94f, -0.03f),
				new ValueTuple<float, float>(0.86f, -0.09f),
				new ValueTuple<float, float>(0.72f, -0.12f),
				new ValueTuple<float, float>(0.52f, -0.16f),
				new ValueTuple<float, float>(0.36f, -0.24f),
				new ValueTuple<float, float>(0.32f, -0.36f),
				new ValueTuple<float, float>(0.18f, -0.38f),
				new ValueTuple<float, float>(0.06f, -0.12f),
				new ValueTuple<float, float>(0.04f, -0f),
				new ValueTuple<float, float>(0.06f, 0.03f),
				new ValueTuple<float, float>(0.18f, 0.07f),
				new ValueTuple<float, float>(0.32f, 0.1f),
				new ValueTuple<float, float>(0.36f, 0.13f),
				new ValueTuple<float, float>(0.52f, 0.13f),
				new ValueTuple<float, float>(0.72f, 0.1f),
				new ValueTuple<float, float>(0.86f, 0.07f),
				new ValueTuple<float, float>(0.94f, 0.03f),
				new ValueTuple<float, float>(1f, 0f),
				new ValueTuple<float, float>(0.94f, -0.03f),
				new ValueTuple<float, float>(0.86f, -0.07f),
				new ValueTuple<float, float>(0.72f, -0.1f),
				new ValueTuple<float, float>(0.52f, -0.13f),
				new ValueTuple<float, float>(0.36f, -0.13f),
				new ValueTuple<float, float>(0.32f, -0.1f),
				new ValueTuple<float, float>(0.18f, -0.07f),
				new ValueTuple<float, float>(0.06f, -0.03f),
				new ValueTuple<float, float>(0.04f, 0f),
				new ValueTuple<float, float>(0.06f, 0.12f),
				new ValueTuple<float, float>(0.18f, 0.38f),
				new ValueTuple<float, float>(0.32f, 0.36f),
				new ValueTuple<float, float>(0.36f, 0.24f),
				new ValueTuple<float, float>(0.52f, 0.16f),
				new ValueTuple<float, float>(0.72f, 0.12f),
				new ValueTuple<float, float>(0.86f, 0.09f),
				new ValueTuple<float, float>(0.94f, 0.03f)
			};
		}

		// Token: 0x06000AAC RID: 2732 RVA: 0x0004A7F4 File Offset: 0x000489F4
		public static float GetAngleOfAttack(in Vec2 sailDir, in Vec2 windDir)
		{
			Vec2 vec = sailDir;
			Vec3 vec2 = vec.ToVec3(0f);
			vec = windDir;
			Vec3 vec3 = Vec3.CrossProduct(vec2, vec.ToVec3(0f));
			float num = Vec2.DotProduct(sailDir, windDir);
			return MathF.Atan2(vec3.z, num);
		}

		// Token: 0x06000AAD RID: 2733 RVA: 0x0004A849 File Offset: 0x00048A49
		public static float NormalizeThrustValue(float thrustValue, float minThrustValue, float maxThrustValue)
		{
			if (maxThrustValue == minThrustValue)
			{
				return 0f;
			}
			return (thrustValue - minThrustValue) / (maxThrustValue - minThrustValue);
		}

		// Token: 0x0400062E RID: 1582
		private const int BinCount = 36;

		// Token: 0x0400062F RID: 1583
		private const float BinAngleInDegrees = 10f;

		// Token: 0x04000630 RID: 1584
		private static SailWindProfile _instance;

		// Token: 0x04000631 RID: 1585
		[TupleElementNames(new string[] { "dragCoef", "liftCoef" })]
		private ValueTuple<float, float>[][] _sailWindProfiles;
	}
}
