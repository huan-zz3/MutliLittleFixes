using System;
using System.Collections.Generic;
using NavalDLC.Missions.NavalPhysics;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.MissionLogics
{
	// Token: 0x020000CE RID: 206
	public class NavalFloatsamLogic : MissionLogic
	{
		// Token: 0x06000F72 RID: 3954 RVA: 0x0007666A File Offset: 0x0007486A
		public override void OnBehaviorInitialize()
		{
			Mission.Current.OnMissileRemovedEvent += this.OnMissileRemoved;
		}

		// Token: 0x06000F73 RID: 3955 RVA: 0x00076683 File Offset: 0x00074883
		public override void AfterStart()
		{
		}

		// Token: 0x06000F74 RID: 3956 RVA: 0x00076685 File Offset: 0x00074885
		public override void OnMissionTick(float dt)
		{
			this.CheckFloatsamTimers();
			this.TickFadingOutEntities();
		}

		// Token: 0x06000F75 RID: 3957 RVA: 0x00076693 File Offset: 0x00074893
		protected override void OnEndMission()
		{
			this._orderedEntities.Clear();
			this._fadingOutEntities.Clear();
			this._orderedEntities = null;
			this._fadingOutEntities = null;
		}

		// Token: 0x06000F76 RID: 3958 RVA: 0x000766BC File Offset: 0x000748BC
		public void RegisterFloatsamInstance(GameEntity entity)
		{
			if (this._orderedEntities.Count >= 40)
			{
				NavalFloatsamLogic.FloatSamRecord floatSamRecord = this._orderedEntities.Dequeue();
				NavalFloatsamLogic.FadingOutRecord fadingOutRecord = new NavalFloatsamLogic.FadingOutRecord
				{
					FloatsamEntity = floatSamRecord.FloatsamEntity
				};
				fadingOutRecord.FloatsamEntity.GetFirstScriptOfType<NavalPhysics>().ForceSink();
				fadingOutRecord.FadeOutTimer = new Timer(Mission.Current.CurrentTime, 5f, true);
				this._fadingOutEntities.Enqueue(fadingOutRecord);
			}
			NavalFloatsamLogic.FloatSamRecord floatSamRecord2 = default(NavalFloatsamLogic.FloatSamRecord);
			floatSamRecord2.FloatsamEntity = entity;
			floatSamRecord2.DeSpawnTimer = new Timer(Mission.Current.CurrentTime, MBRandom.RandomFloatRanged(10f, 15f), true);
			this._orderedEntities.Enqueue(floatSamRecord2);
		}

		// Token: 0x06000F77 RID: 3959 RVA: 0x00076774 File Offset: 0x00074974
		private void CheckFloatsamTimers()
		{
			while (this._orderedEntities.Count > 0)
			{
				NavalFloatsamLogic.FloatSamRecord floatSamRecord = this._orderedEntities.Peek();
				if (!floatSamRecord.DeSpawnTimer.Check(Mission.Current.CurrentTime))
				{
					break;
				}
				NavalFloatsamLogic.FadingOutRecord fadingOutRecord = new NavalFloatsamLogic.FadingOutRecord
				{
					FloatsamEntity = floatSamRecord.FloatsamEntity
				};
				fadingOutRecord.FloatsamEntity.GetFirstScriptOfType<NavalPhysics>().ForceSink();
				fadingOutRecord.FadeOutTimer = new Timer(Mission.Current.CurrentTime, 5f, true);
				this._fadingOutEntities.Enqueue(fadingOutRecord);
				this._orderedEntities.Dequeue();
			}
		}

		// Token: 0x06000F78 RID: 3960 RVA: 0x00076810 File Offset: 0x00074A10
		private void TickFadingOutEntities()
		{
			float currentTime = Mission.Current.CurrentTime;
			while (this._fadingOutEntities.Count > 0)
			{
				NavalFloatsamLogic.FadingOutRecord fadingOutRecord = this._fadingOutEntities.Peek();
				if (!fadingOutRecord.FadeOutTimer.Check(currentTime))
				{
					break;
				}
				if (fadingOutRecord.FloatsamEntity.HasScene())
				{
					Mission.Current.Scene.RemoveEntity(fadingOutRecord.FloatsamEntity, 35);
				}
				this._fadingOutEntities.Dequeue();
			}
			foreach (NavalFloatsamLogic.FadingOutRecord fadingOutRecord2 in this._fadingOutEntities)
			{
				float num = 1f - MBMath.ClampFloat((fadingOutRecord2.FadeOutTimer.StartTime - currentTime) / 5f, 0f, 1f);
				fadingOutRecord2.FloatsamEntity.SetAlpha(num);
			}
		}

		// Token: 0x04000955 RID: 2389
		private const int MaxNumberOfFloatsam = 40;

		// Token: 0x04000956 RID: 2390
		private const float MinFloatsamAliveDuration = 10f;

		// Token: 0x04000957 RID: 2391
		private const float MaxFloatsamAliveDuration = 15f;

		// Token: 0x04000958 RID: 2392
		private const float FadeOutDuration = 5f;

		// Token: 0x04000959 RID: 2393
		private Queue<NavalFloatsamLogic.FloatSamRecord> _orderedEntities = new Queue<NavalFloatsamLogic.FloatSamRecord>();

		// Token: 0x0400095A RID: 2394
		private Queue<NavalFloatsamLogic.FadingOutRecord> _fadingOutEntities = new Queue<NavalFloatsamLogic.FadingOutRecord>();

		// Token: 0x0200024F RID: 591
		private struct FloatSamRecord
		{
			// Token: 0x04001056 RID: 4182
			internal GameEntity FloatsamEntity;

			// Token: 0x04001057 RID: 4183
			internal Timer DeSpawnTimer;
		}

		// Token: 0x02000250 RID: 592
		private struct FadingOutRecord
		{
			// Token: 0x04001058 RID: 4184
			internal GameEntity FloatsamEntity;

			// Token: 0x04001059 RID: 4185
			internal Timer FadeOutTimer;
		}
	}
}
