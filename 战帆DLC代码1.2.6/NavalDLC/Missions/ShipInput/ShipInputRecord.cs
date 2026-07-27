using System;

namespace NavalDLC.Missions.ShipInput
{
	// Token: 0x0200008A RID: 138
	public struct ShipInputRecord
	{
		// Token: 0x17000193 RID: 403
		// (get) Token: 0x060009B8 RID: 2488 RVA: 0x00045292 File Offset: 0x00043492
		// (set) Token: 0x060009B9 RID: 2489 RVA: 0x0004529A File Offset: 0x0004349A
		public RowerLateralInput RowerLateral { get; private set; }

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x060009BA RID: 2490 RVA: 0x000452A3 File Offset: 0x000434A3
		// (set) Token: 0x060009BB RID: 2491 RVA: 0x000452AB File Offset: 0x000434AB
		public RowerLongitudinalInput RowerLongitudinal { get; private set; }

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x060009BC RID: 2492 RVA: 0x000452B4 File Offset: 0x000434B4
		// (set) Token: 0x060009BD RID: 2493 RVA: 0x000452BC File Offset: 0x000434BC
		public RowerLongitudinalInput RowerLongitudinalDoubleTap { get; private set; }

		// Token: 0x17000196 RID: 406
		// (get) Token: 0x060009BE RID: 2494 RVA: 0x000452C5 File Offset: 0x000434C5
		// (set) Token: 0x060009BF RID: 2495 RVA: 0x000452CD File Offset: 0x000434CD
		public float RudderLateral { get; private set; }

		// Token: 0x17000197 RID: 407
		// (get) Token: 0x060009C0 RID: 2496 RVA: 0x000452D6 File Offset: 0x000434D6
		// (set) Token: 0x060009C1 RID: 2497 RVA: 0x000452DE File Offset: 0x000434DE
		public SailInput Sail { get; private set; }

		// Token: 0x060009C2 RID: 2498 RVA: 0x000452E7 File Offset: 0x000434E7
		public ShipInputRecord(RowerLateralInput rowerLateral, RowerLongitudinalInput rowerLongitudinal, RowerLongitudinalInput rowerLongitudinalDoubleTap, float rudderLateral, SailInput sail)
		{
			this.RowerLateral = rowerLateral;
			this.RowerLongitudinal = rowerLongitudinal;
			this.RowerLongitudinalDoubleTap = rowerLongitudinalDoubleTap;
			this.RudderLateral = rudderLateral;
			this.Sail = sail;
		}

		// Token: 0x060009C3 RID: 2499 RVA: 0x0004530E File Offset: 0x0004350E
		public void SetRowerLateral(RowerLateralInput value)
		{
			this.RowerLateral = value;
		}

		// Token: 0x060009C4 RID: 2500 RVA: 0x00045317 File Offset: 0x00043517
		public void SetRowerLongitudinal(RowerLongitudinalInput value)
		{
			this.RowerLongitudinal = value;
		}

		// Token: 0x060009C5 RID: 2501 RVA: 0x00045320 File Offset: 0x00043520
		public void SetRowerLongitudinalDoupleTap(RowerLongitudinalInput value)
		{
			this.RowerLongitudinalDoubleTap = value;
		}

		// Token: 0x060009C6 RID: 2502 RVA: 0x00045329 File Offset: 0x00043529
		public void SetRudderLateral(float value)
		{
			this.RudderLateral = value;
		}

		// Token: 0x060009C7 RID: 2503 RVA: 0x00045332 File Offset: 0x00043532
		public void SetSail(SailInput value)
		{
			this.Sail = value;
		}

		// Token: 0x060009C8 RID: 2504 RVA: 0x0004533B File Offset: 0x0004353B
		public static ShipInputRecord None()
		{
			return new ShipInputRecord(RowerLateralInput.None, RowerLongitudinalInput.None, RowerLongitudinalInput.None, 0f, SailInput.Raised);
		}

		// Token: 0x060009C9 RID: 2505 RVA: 0x0004534B File Offset: 0x0004354B
		public static ShipInputRecord Stop()
		{
			return new ShipInputRecord(RowerLateralInput.Stop, RowerLongitudinalInput.Stop, RowerLongitudinalInput.Stop, 0f, SailInput.Raised);
		}
	}
}
