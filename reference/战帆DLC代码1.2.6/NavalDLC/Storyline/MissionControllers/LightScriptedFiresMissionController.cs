using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Storyline.MissionControllers
{
	// Token: 0x0200006B RID: 107
	public class LightScriptedFiresMissionController : MissionLogic
	{
		// Token: 0x0600068C RID: 1676 RVA: 0x00027744 File Offset: 0x00025944
		public override void AfterStart()
		{
			base.AfterStart();
			List<GameEntity> list = Mission.Current.Scene.FindEntitiesWithTagExpression("light_scripted_fire(_\\d+)*").ToList<GameEntity>();
			GameEntity[] array = new GameEntity[list.Count];
			foreach (GameEntity gameEntity in list)
			{
				gameEntity.SetVisibilityExcludeParents(false);
				string[] array2 = gameEntity.Tags.FirstOrDefault<string>().Split(new char[] { '_' });
				int num = int.Parse(array2[array2.Length - 1]);
				array[num - 1] = gameEntity;
			}
			foreach (GameEntity gameEntity2 in array)
			{
				this._fireEntities.Enqueue(gameEntity2);
			}
		}

		// Token: 0x0600068D RID: 1677 RVA: 0x00027814 File Offset: 0x00025A14
		public override void OnMissionTick(float dt)
		{
			base.OnMissionTick(dt);
			if (this._isFiringTriggered)
			{
				if (this._fireTimer == null)
				{
					this._fireTimer = new MissionTimer(3f);
					return;
				}
				if (this._fireTimer.Check(false))
				{
					this._fireTimer = null;
					this._fireEntities.Dequeue().SetVisibilityExcludeParents(true);
					if (Extensions.IsEmpty<GameEntity>(this._fireEntities))
					{
						this._isFiringTriggered = false;
					}
				}
			}
		}

		// Token: 0x0600068E RID: 1678 RVA: 0x00027883 File Offset: 0x00025A83
		public void TriggerFiring()
		{
			if (!Extensions.IsEmpty<GameEntity>(this._fireEntities))
			{
				this._isFiringTriggered = true;
			}
		}

		// Token: 0x0600068F RID: 1679 RVA: 0x0002789C File Offset: 0x00025A9C
		public void PutOutFires()
		{
			foreach (GameEntity gameEntity in Mission.Current.Scene.FindEntitiesWithTagExpression("light_scripted_fire(_\\d+)*").ToList<GameEntity>())
			{
				gameEntity.SetVisibilityExcludeParents(false);
			}
		}

		// Token: 0x04000353 RID: 851
		private const string FireTagExpression = "light_scripted_fire(_\\d+)*";

		// Token: 0x04000354 RID: 852
		private const float FireTimerAsSeconds = 3f;

		// Token: 0x04000355 RID: 853
		private Queue<GameEntity> _fireEntities = new Queue<GameEntity>();

		// Token: 0x04000356 RID: 854
		private MissionTimer _fireTimer;

		// Token: 0x04000357 RID: 855
		private bool _isFiringTriggered;
	}
}
