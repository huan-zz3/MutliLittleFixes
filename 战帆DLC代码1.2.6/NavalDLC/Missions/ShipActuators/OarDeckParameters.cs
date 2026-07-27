using System;

namespace NavalDLC.Missions.ShipActuators
{
	// Token: 0x02000093 RID: 147
	public class OarDeckParameters
	{
		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x06000A76 RID: 2678 RVA: 0x00049930 File Offset: 0x00047B30
		// (set) Token: 0x06000A77 RID: 2679 RVA: 0x00049938 File Offset: 0x00047B38
		public float VerticalBaseAngle { get; private set; }

		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x06000A78 RID: 2680 RVA: 0x00049941 File Offset: 0x00047B41
		// (set) Token: 0x06000A79 RID: 2681 RVA: 0x00049949 File Offset: 0x00047B49
		public float LateralBaseAngle { get; private set; }

		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x06000A7A RID: 2682 RVA: 0x00049952 File Offset: 0x00047B52
		// (set) Token: 0x06000A7B RID: 2683 RVA: 0x0004995A File Offset: 0x00047B5A
		public float VerticalRotationAngle { get; private set; }

		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x06000A7C RID: 2684 RVA: 0x00049963 File Offset: 0x00047B63
		// (set) Token: 0x06000A7D RID: 2685 RVA: 0x0004996B File Offset: 0x00047B6B
		public float LateralRotationAngle { get; private set; }

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x06000A7E RID: 2686 RVA: 0x00049974 File Offset: 0x00047B74
		// (set) Token: 0x06000A7F RID: 2687 RVA: 0x0004997C File Offset: 0x00047B7C
		public float OarLength { get; private set; }

		// Token: 0x170001CB RID: 459
		// (get) Token: 0x06000A80 RID: 2688 RVA: 0x00049985 File Offset: 0x00047B85
		// (set) Token: 0x06000A81 RID: 2689 RVA: 0x0004998D File Offset: 0x00047B8D
		public float RetractionRate { get; private set; }

		// Token: 0x170001CC RID: 460
		// (get) Token: 0x06000A82 RID: 2690 RVA: 0x00049996 File Offset: 0x00047B96
		// (set) Token: 0x06000A83 RID: 2691 RVA: 0x0004999E File Offset: 0x00047B9E
		public float RetractionOffset { get; private set; }

		// Token: 0x06000A84 RID: 2692 RVA: 0x000499A7 File Offset: 0x00047BA7
		public OarDeckParameters(float verticalBaseAngle = 0.2617994f, float lateralBaseAngle = 0f, float verticalRotationAngle = 0.17453292f, float lateralRotationAngle = 0.30019665f, float oarLength = 4f, float retractionRate = 0.4f, float retractionOffset = 1f)
		{
			this.SetParameters(verticalBaseAngle, lateralBaseAngle, verticalRotationAngle, lateralRotationAngle, oarLength, retractionRate, retractionOffset);
		}

		// Token: 0x06000A85 RID: 2693 RVA: 0x000499C0 File Offset: 0x00047BC0
		public OarDeckParameters()
		{
			this.SetParameters(0.2617994f, 0f, 0.17453292f, 0.30019665f, 4f, 0.4f, 1f);
		}

		// Token: 0x06000A86 RID: 2694 RVA: 0x000499F1 File Offset: 0x00047BF1
		public void SetParameters(float verticalBaseAngle = 0.2617994f, float lateralBaseAngle = 0f, float verticalRotationAngle = 0.17453292f, float lateralRotationAngle = 0.30019665f, float oarLength = 4f, float retractionRate = 0.4f, float retractionOffset = 1f)
		{
			this.VerticalBaseAngle = verticalBaseAngle;
			this.LateralBaseAngle = lateralBaseAngle;
			this.VerticalRotationAngle = verticalRotationAngle;
			this.LateralRotationAngle = lateralRotationAngle;
			this.OarLength = oarLength;
			this.RetractionRate = retractionRate;
			this.RetractionOffset = retractionOffset;
		}

		// Token: 0x04000613 RID: 1555
		public const float DefaultVerticalBaseAngle = 0.2617994f;

		// Token: 0x04000614 RID: 1556
		public const float DefaultLateralBaseAngle = 0f;

		// Token: 0x04000615 RID: 1557
		public const float DefaultVerticalRotationAngle = 0.17453292f;

		// Token: 0x04000616 RID: 1558
		public const float DefaultLateralRotationAngle = 0.30019665f;

		// Token: 0x04000617 RID: 1559
		public const float DefaultOarLength = 4f;

		// Token: 0x04000618 RID: 1560
		public const float DefaultRetractionRate = 0.4f;

		// Token: 0x04000619 RID: 1561
		public const float DefaultRetractionOffset = 1f;
	}
}
