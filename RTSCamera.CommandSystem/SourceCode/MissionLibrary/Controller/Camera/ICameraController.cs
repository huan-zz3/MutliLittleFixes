using System;
using TaleWorlds.Library;

namespace MissionLibrary.Controller.Camera
{
	// Token: 0x02000028 RID: 40
	public interface ICameraController
	{
		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060000A3 RID: 163
		// (set) Token: 0x060000A4 RID: 164
		float ViewAngle { get; set; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000A5 RID: 165
		// (set) Token: 0x060000A6 RID: 166
		float RollAngle { get; set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000A7 RID: 167
		// (set) Token: 0x060000A8 RID: 168
		bool SmoothRotationMode { get; set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000A9 RID: 169
		// (set) Token: 0x060000AA RID: 170
		float MovementSpeedFactor { get; set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000AB RID: 171
		// (set) Token: 0x060000AC RID: 172
		float VerticalMovementSpeedFactor { get; set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000AD RID: 173
		// (set) Token: 0x060000AE RID: 174
		float DepthOfFieldDistance { get; set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000AF RID: 175
		// (set) Token: 0x060000B0 RID: 176
		float DepthOfFieldStart { get; set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000B1 RID: 177
		// (set) Token: 0x060000B2 RID: 178
		float DepthOfFieldEnd { get; set; }

		// Token: 0x060000B3 RID: 179
		bool RequestCameraGoTo(Vec3 position, Vec3 direction = default(Vec3));

		// Token: 0x060000B4 RID: 180
		bool RequestCameraGoTo(Vec2 position, Vec2 direction = default(Vec2));
	}
}
