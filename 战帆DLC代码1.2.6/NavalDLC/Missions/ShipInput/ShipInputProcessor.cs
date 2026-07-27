using System;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.ShipActuators;
using TaleWorlds.Library;

namespace NavalDLC.Missions.ShipInput
{
	// Token: 0x02000089 RID: 137
	public class ShipInputProcessor
	{
		// Token: 0x060009B5 RID: 2485 RVA: 0x00045060 File Offset: 0x00043260
		public ShipInputProcessor(MissionShip ownerShip)
		{
			this._ownerShip = ownerShip;
			this._rowerThrust = 0f;
			this._rowerRotation = 0f;
			this._rudderRotation = 0f;
			this._squareSailSetting = 0f;
			this._lateenSailSetting = 0f;
		}

		// Token: 0x060009B6 RID: 2486 RVA: 0x000450B4 File Offset: 0x000432B4
		public ShipActuatorRecord OnParallelFixedTick(float fixedDt, in ShipInputRecord inputRecord)
		{
			ShipInputRecord shipInputRecord = inputRecord;
			if (shipInputRecord.RowerLongitudinal == RowerLongitudinalInput.Forward)
			{
				this._rowerThrust = 1f;
			}
			else
			{
				shipInputRecord = inputRecord;
				if (shipInputRecord.RowerLongitudinal == RowerLongitudinalInput.Backward)
				{
					this._rowerThrust = -1f;
				}
				else
				{
					this._rowerThrust = 0f;
				}
			}
			float num = 0f;
			shipInputRecord = inputRecord;
			if (shipInputRecord.RowerLongitudinalDoubleTap == RowerLongitudinalInput.Forward)
			{
				num = 1f;
			}
			else
			{
				shipInputRecord = inputRecord;
				if (shipInputRecord.RowerLongitudinalDoubleTap == RowerLongitudinalInput.Backward)
				{
					num = -1f;
				}
			}
			shipInputRecord = inputRecord;
			if (shipInputRecord.RowerLateral == RowerLateralInput.Left)
			{
				this._rowerRotation = 1f;
			}
			else
			{
				shipInputRecord = inputRecord;
				if (shipInputRecord.RowerLateral == RowerLateralInput.Right)
				{
					this._rowerRotation = -1f;
				}
				else
				{
					shipInputRecord = inputRecord;
					if (shipInputRecord.RowerLateral == RowerLateralInput.Stop)
					{
						this._rowerRotation = 0f;
					}
					else
					{
						this._rowerRotation = 0f;
					}
				}
			}
			shipInputRecord = inputRecord;
			this._rudderRotation = shipInputRecord.RudderLateral;
			shipInputRecord = inputRecord;
			if (shipInputRecord.Sail == SailInput.Raised)
			{
				this._squareSailSetting = 0f;
				this._lateenSailSetting = 0f;
			}
			else
			{
				shipInputRecord = inputRecord;
				if (shipInputRecord.Sail == SailInput.SquareSailsRaised)
				{
					this._squareSailSetting = 0f;
					this._lateenSailSetting = 1f;
				}
				else
				{
					shipInputRecord = inputRecord;
					if (shipInputRecord.Sail == SailInput.Full)
					{
						this._squareSailSetting = 1f;
						this._lateenSailSetting = 1f;
					}
				}
			}
			this._squareSailSetting = MathF.Clamp(this._squareSailSetting, 0f, 1f);
			this._lateenSailSetting = MathF.Clamp(this._lateenSailSetting, 0f, 1f);
			return new ShipActuatorRecord(this._rowerThrust, num, this._rowerRotation, this._rudderRotation, this._squareSailSetting, this._lateenSailSetting);
		}

		// Token: 0x060009B7 RID: 2487 RVA: 0x00045289 File Offset: 0x00043489
		public void Deallocate()
		{
			this._ownerShip = null;
		}

		// Token: 0x040005A1 RID: 1441
		private MissionShip _ownerShip;

		// Token: 0x040005A2 RID: 1442
		private float _rowerThrust;

		// Token: 0x040005A3 RID: 1443
		private float _rowerRotation;

		// Token: 0x040005A4 RID: 1444
		private float _rudderRotation;

		// Token: 0x040005A5 RID: 1445
		private float _squareSailSetting;

		// Token: 0x040005A6 RID: 1446
		private float _lateenSailSetting;
	}
}
