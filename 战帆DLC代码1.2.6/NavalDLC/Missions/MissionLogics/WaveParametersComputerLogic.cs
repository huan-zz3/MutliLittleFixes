using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.MissionLogics
{
	// Token: 0x020000DA RID: 218
	public class WaveParametersComputerLogic : MissionLogic
	{
		// Token: 0x0600111F RID: 4383 RVA: 0x0007FC88 File Offset: 0x0007DE88
		public static WaveParametersComputerLogic.WaterParameters AnalyzeHeightMap(Vec2 waveDirection, Scene scene)
		{
			waveDirection = waveDirection.Normalized();
			float num = float.MaxValue;
			float num2 = float.MinValue;
			float num3 = 0f;
			float num4 = 0f;
			List<float> list = new List<float>();
			float num5 = 0f;
			bool flag = false;
			float num6 = 0.15f;
			float num7 = 0f;
			Vec2 vec;
			vec..ctor(num3, num4);
			float num8 = scene.GetWaterLevelAtPosition(vec, true, false);
			for (int i = 0; i < 1000; i++)
			{
				vec += waveDirection * num6;
				Vec2 vec2 = vec + waveDirection * num6;
				float waterLevelAtPosition = scene.GetWaterLevelAtPosition(vec, true, false);
				float waterLevelAtPosition2 = scene.GetWaterLevelAtPosition(vec2, true, false);
				if (waterLevelAtPosition > num8 && waterLevelAtPosition > waterLevelAtPosition2)
				{
					if (flag)
					{
						float num9 = num7 - num5;
						list.Add(num9);
						num5 = num7;
					}
					else
					{
						flag = true;
						num5 = num7;
					}
				}
				num8 = waterLevelAtPosition;
				num7 += num6;
				if (waterLevelAtPosition < num)
				{
					num = waterLevelAtPosition;
				}
				if (waterLevelAtPosition > num2)
				{
					num2 = waterLevelAtPosition;
				}
			}
			float num12;
			if (list.Count >= 1)
			{
				float num10 = 0f;
				foreach (float num11 in list)
				{
					num10 += num11;
				}
				num12 = num10 / (float)list.Count;
			}
			else
			{
				num12 = 80f;
			}
			float num13 = (num2 - num) * 0.5f;
			float num14 = 6.2831855f / num12;
			float num15 = MathF.Sqrt(9.806f * num14);
			return new WaveParametersComputerLogic.WaterParameters
			{
				Amplitude = num13,
				Wavelength = num12,
				WaveNumber = num14,
				Omega = num15,
				WaveMax = num2,
				WaveMin = num
			};
		}

		// Token: 0x02000260 RID: 608
		public struct WaterParameters
		{
			// Token: 0x04001077 RID: 4215
			public float Amplitude;

			// Token: 0x04001078 RID: 4216
			public float Wavelength;

			// Token: 0x04001079 RID: 4217
			public float WaveNumber;

			// Token: 0x0400107A RID: 4218
			public float Omega;

			// Token: 0x0400107B RID: 4219
			public float WaveMax;

			// Token: 0x0400107C RID: 4220
			public float WaveMin;
		}
	}
}
