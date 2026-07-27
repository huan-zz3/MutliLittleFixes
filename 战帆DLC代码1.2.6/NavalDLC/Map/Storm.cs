using System;
using System.Diagnostics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem;

namespace NavalDLC.Map
{
	// Token: 0x020000FC RID: 252
	public class Storm
	{
		// Token: 0x1700032E RID: 814
		// (get) Token: 0x060012AB RID: 4779 RVA: 0x000894C8 File Offset: 0x000876C8
		public bool IsActive
		{
			get
			{
				return !this.IsInDevelopingState && !this.IsInFinalizingState;
			}
		}

		// Token: 0x1700032F RID: 815
		// (get) Token: 0x060012AC RID: 4780 RVA: 0x000894DD File Offset: 0x000876DD
		// (set) Token: 0x060012AD RID: 4781 RVA: 0x000894E5 File Offset: 0x000876E5
		public Vec2 CurrentPosition
		{
			get
			{
				return this._currentPosition;
			}
			private set
			{
				this._currentPosition = value;
				if (this.IsPositionOutOfMapBoundary(this._currentPosition))
				{
					this.ForceDeactivate();
				}
				this.SetVisualDirty();
			}
		}

		// Token: 0x17000330 RID: 816
		// (get) Token: 0x060012AE RID: 4782 RVA: 0x00089508 File Offset: 0x00087708
		// (set) Token: 0x060012AF RID: 4783 RVA: 0x00089510 File Offset: 0x00087710
		public float Intensity
		{
			get
			{
				return this._intensity;
			}
			set
			{
				this._intensity = MBMath.ClampFloat(value, 0f, 1f);
				if (this._intensity <= 0f)
				{
					this.ForceDeactivate();
				}
			}
		}

		// Token: 0x17000331 RID: 817
		// (get) Token: 0x060012B0 RID: 4784 RVA: 0x0008953B File Offset: 0x0008773B
		public bool IsInDevelopingState
		{
			get
			{
				return this._developingStateFinishCampaignTime.IsFuture;
			}
		}

		// Token: 0x17000332 RID: 818
		// (get) Token: 0x060012B1 RID: 4785 RVA: 0x00089548 File Offset: 0x00087748
		public bool IsInFinalizingState
		{
			get
			{
				return this._finalizingStateStartCampaignTime.IsPast;
			}
		}

		// Token: 0x17000333 RID: 819
		// (get) Token: 0x060012B2 RID: 4786 RVA: 0x00089558 File Offset: 0x00087758
		public bool IsReadyToBeFinalized
		{
			get
			{
				return (this._finalizingStateStartCampaignTime + NavalDLCManager.Instance.GameModels.MapStormModel.GetFinalizingStateDurationOfStorm(this)).IsPast;
			}
		}

		// Token: 0x17000334 RID: 820
		// (get) Token: 0x060012B3 RID: 4787 RVA: 0x0008958D File Offset: 0x0008778D
		// (set) Token: 0x060012B4 RID: 4788 RVA: 0x00089595 File Offset: 0x00087795
		public bool IsVisuallyDirty { get; private set; }

		// Token: 0x17000335 RID: 821
		// (get) Token: 0x060012B5 RID: 4789 RVA: 0x0008959E File Offset: 0x0008779E
		public float EffectRadius
		{
			get
			{
				return NavalDLCManager.Instance.GameModels.MapStormModel.GetEffectRadiusOfStorm(this);
			}
		}

		// Token: 0x17000336 RID: 822
		// (get) Token: 0x060012B6 RID: 4790 RVA: 0x000895B5 File Offset: 0x000877B5
		public float EyeRadius
		{
			get
			{
				return NavalDLCManager.Instance.GameModels.MapStormModel.GetEyeRadiusOfStorm(this);
			}
		}

		// Token: 0x060012B7 RID: 4791 RVA: 0x000895CC File Offset: 0x000877CC
		public Storm(Vec2 initialPosition, Storm.StormTypes stormType)
		{
			this.StormType = stormType;
			this.CurrentPosition = initialPosition;
			this.Intensity = 0.5f;
			this._speed = NavalDLCManager.Instance.GameModels.MapStormModel.GetSpeedOfStorm(this);
			this._developingStateFinishCampaignTime = CampaignTime.Now + NavalDLCManager.Instance.GameModels.MapStormModel.GetDevelopingStateDurationOfStorm(this);
			CampaignTime campaignTime;
			CampaignTime campaignTime2;
			NavalDLCManager.Instance.GameModels.MapStormModel.GetStormLifeSpan(out campaignTime, out campaignTime2);
			CampaignTime campaignTime3 = campaignTime + CampaignTime.Days((float)MBRandom.RandomInt(0, (int)campaignTime2.ToDays));
			this._finalizingStateStartCampaignTime = this._developingStateFinishCampaignTime + campaignTime3;
			this.ChangeMoveDirection();
			this.SetVisualDirty();
		}

		// Token: 0x060012B8 RID: 4792 RVA: 0x00089695 File Offset: 0x00087895
		public void ForceDeactivate()
		{
			this._finalizingStateStartCampaignTime = CampaignTime.Now;
			this.SetVisualDirty();
		}

		// Token: 0x060012B9 RID: 4793 RVA: 0x000896A8 File Offset: 0x000878A8
		public void SetVisualDirty()
		{
			this.IsVisuallyDirty = true;
		}

		// Token: 0x060012BA RID: 4794 RVA: 0x000896B1 File Offset: 0x000878B1
		public void OnVisualUpdated()
		{
			this.IsVisuallyDirty = false;
		}

		// Token: 0x060012BB RID: 4795 RVA: 0x000896BC File Offset: 0x000878BC
		public bool HasWetWeatherEffectAtPosition(Vec2 pos)
		{
			if (this.CurrentPosition.DistanceSquared(pos) < this.EffectRadius * this.EffectRadius * 1.2f)
			{
				return true;
			}
			int num = Math.Min(this._nextUpdatePreviousDataArrayIndex, 6);
			for (int i = 0; i < num; i++)
			{
				Storm.PreviousData previousData = this._previousPositionsAndRadius[i];
				if (previousData.Position.DistanceSquared(pos) < previousData.EffectRadius * previousData.EffectRadius * 1.2f)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060012BC RID: 4796 RVA: 0x0008973C File Offset: 0x0008793C
		public void HourlyTick()
		{
			if (this.IsActive && this._nextUpdateTime.IsPast)
			{
				this._previousPositionsAndRadius[this._nextUpdatePreviousDataArrayIndex] = new Storm.PreviousData(this.CurrentPosition, this.EffectRadius);
				this._nextUpdatePreviousDataArrayIndex = (this._nextUpdatePreviousDataArrayIndex + 1) % this._previousPositionsAndRadius.Length;
				this._nextUpdateTime = CampaignTime.HoursFromNow(4f);
			}
		}

		// Token: 0x060012BD RID: 4797 RVA: 0x000897A8 File Offset: 0x000879A8
		public void Tick(float dt)
		{
			if (this.IsActive && !NavalDLCManager.Instance.StormManager.DebugVisualsStopped)
			{
				this._currentMoveDirection = Vec2.Lerp(this._currentMoveDirection, this._desiredMoveDirection, dt);
				this.CurrentPosition += this._currentMoveDirection * dt * this._speed;
			}
		}

		// Token: 0x060012BE RID: 4798 RVA: 0x0008980E File Offset: 0x00087A0E
		public void OnAfterLoad()
		{
			this._speed = NavalDLCManager.Instance.GameModels.MapStormModel.GetSpeedOfStorm(this);
			this.SetVisualDirty();
		}

		// Token: 0x060012BF RID: 4799 RVA: 0x00089834 File Offset: 0x00087A34
		public void ChangeMoveDirection()
		{
			this._desiredMoveDirection = Campaign.Current.Models.MapWeatherModel.GetWindForPosition(new CampaignVec2(this.CurrentPosition, false));
			float num = MBRandom.RandomFloatNormal * 30f;
			this._desiredMoveDirection.RotateCCW(num * 0.017453292f);
			this._desiredMoveDirection.Normalize();
		}

		// Token: 0x060012C0 RID: 4800 RVA: 0x00089894 File Offset: 0x00087A94
		private bool IsPositionOutOfMapBoundary(Vec2 position)
		{
			Vec2 vec;
			Vec2 vec2;
			float num;
			Campaign.Current.MapSceneWrapper.GetMapBorders(ref vec, ref vec2, ref num);
			return position.X < vec.X || position.X > vec2.X || position.Y < vec.Y || position.Y > vec2.Y;
		}

		// Token: 0x060012C1 RID: 4801 RVA: 0x000898FC File Offset: 0x00087AFC
		[Conditional("DEBUG")]
		private void DebugVisualTick()
		{
			if (NavalDLCManager.Instance.StormManager.DebugVisualsEnabled)
			{
				bool isActive = this.IsActive;
				new Vec3(this.CurrentPosition, 5f, -1f);
				Storm.PreviousData[] previousPositionsAndRadius = this._previousPositionsAndRadius;
				for (int i = 0; i < previousPositionsAndRadius.Length; i++)
				{
				}
			}
		}

		// Token: 0x04000A89 RID: 2697
		private const int PreviousPositionsCount = 6;

		// Token: 0x04000A8A RID: 2698
		private const int LastPositionUpdatePeriodInHours = 4;

		// Token: 0x04000A8B RID: 2699
		[SaveableField(10)]
		private Vec2 _currentPosition;

		// Token: 0x04000A8C RID: 2700
		[SaveableField(100)]
		private Storm.PreviousData[] _previousPositionsAndRadius = new Storm.PreviousData[6];

		// Token: 0x04000A8D RID: 2701
		[SaveableField(120)]
		private int _nextUpdatePreviousDataArrayIndex;

		// Token: 0x04000A8E RID: 2702
		[SaveableField(130)]
		private CampaignTime _nextUpdateTime;

		// Token: 0x04000A8F RID: 2703
		[SaveableField(20)]
		public readonly Storm.StormTypes StormType;

		// Token: 0x04000A90 RID: 2704
		[SaveableField(30)]
		private float _intensity;

		// Token: 0x04000A91 RID: 2705
		private float _speed;

		// Token: 0x04000A92 RID: 2706
		[SaveableField(50)]
		private CampaignTime _developingStateFinishCampaignTime;

		// Token: 0x04000A93 RID: 2707
		[SaveableField(60)]
		private CampaignTime _finalizingStateStartCampaignTime;

		// Token: 0x04000A94 RID: 2708
		[SaveableField(80)]
		private Vec2 _desiredMoveDirection;

		// Token: 0x04000A95 RID: 2709
		[SaveableField(90)]
		private Vec2 _currentMoveDirection;

		// Token: 0x02000272 RID: 626
		public enum StormTypes
		{
			// Token: 0x040010B6 RID: 4278
			Storm,
			// Token: 0x040010B7 RID: 4279
			ThunderStorm,
			// Token: 0x040010B8 RID: 4280
			Hurricane
		}

		// Token: 0x02000273 RID: 627
		public struct PreviousData : ISavedStruct
		{
			// Token: 0x17000422 RID: 1058
			// (get) Token: 0x06001C06 RID: 7174 RVA: 0x000B9247 File Offset: 0x000B7447
			public static Storm.PreviousData Invalid
			{
				get
				{
					return new Storm.PreviousData(Vec2.Invalid, -1f);
				}
			}

			// Token: 0x06001C07 RID: 7175 RVA: 0x000B9258 File Offset: 0x000B7458
			public PreviousData(Vec2 position, float effectRadius)
			{
				this.Position = position;
				this.EffectRadius = effectRadius;
			}

			// Token: 0x06001C08 RID: 7176 RVA: 0x000B9268 File Offset: 0x000B7468
			public bool IsDefault()
			{
				return this.Position == Vec2.Zero && this.EffectRadius == 0f;
			}

			// Token: 0x06001C09 RID: 7177 RVA: 0x000B928B File Offset: 0x000B748B
			public override string ToString()
			{
				return this.Position.ToString() + ": " + this.EffectRadius;
			}

			// Token: 0x040010B9 RID: 4281
			[SaveableField(10)]
			public Vec2 Position;

			// Token: 0x040010BA RID: 4282
			[SaveableField(20)]
			public float EffectRadius;
		}
	}
}
