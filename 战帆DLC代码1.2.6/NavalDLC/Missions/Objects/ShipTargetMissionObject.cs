using System;
using System.Collections.Generic;
using NavalDLC.Missions.MissionLogics;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.Objects
{
	// Token: 0x020000A9 RID: 169
	public class ShipTargetMissionObject : MissionObject, ITargetable
	{
		// Token: 0x06000CF8 RID: 3320 RVA: 0x00064700 File Offset: 0x00062900
		protected override void OnInit()
		{
			this._ship = base.GameEntity.Root.GetFirstScriptOfType<MissionShip>();
			this._navalAgentsLogic = Mission.Current.GetMissionBehavior<NavalAgentsLogic>();
		}

		// Token: 0x06000CF9 RID: 3321 RVA: 0x0006473C File Offset: 0x0006293C
		public TargetFlags GetTargetFlags()
		{
			TargetFlags targetFlags = 513;
			if (this._ship.IsSinking)
			{
				targetFlags |= 64;
			}
			return targetFlags;
		}

		// Token: 0x06000CFA RID: 3322 RVA: 0x00064762 File Offset: 0x00062962
		public float GetTargetValue(List<Vec3> weaponPositions)
		{
			return 500f * this.GetMultiplierOfShip();
		}

		// Token: 0x06000CFB RID: 3323 RVA: 0x00064770 File Offset: 0x00062970
		public WeakGameEntity GetTargetEntity()
		{
			return base.GameEntity;
		}

		// Token: 0x06000CFC RID: 3324 RVA: 0x00064778 File Offset: 0x00062978
		public Vec3 GetTargetingOffset()
		{
			return Vec3.Zero;
		}

		// Token: 0x06000CFD RID: 3325 RVA: 0x0006477F File Offset: 0x0006297F
		public BattleSideEnum GetSide()
		{
			return this._ship.BattleSide;
		}

		// Token: 0x06000CFE RID: 3326 RVA: 0x0006478C File Offset: 0x0006298C
		public WeakGameEntity Entity()
		{
			return base.GameEntity;
		}

		// Token: 0x06000CFF RID: 3327 RVA: 0x00064794 File Offset: 0x00062994
		public ValueTuple<Vec3, Vec3> ComputeGlobalPhysicsBoundingBoxMinMax()
		{
			Vec3 globalPosition = base.GameEntity.GlobalPosition;
			return new ValueTuple<Vec3, Vec3>(globalPosition - this.BoundingBoxOffset, globalPosition + this.BoundingBoxOffset);
		}

		// Token: 0x06000D00 RID: 3328 RVA: 0x000647D0 File Offset: 0x000629D0
		public Vec3 GetTargetGlobalVelocity()
		{
			return GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(this._ship.GameEntity, base.GameEntity.GlobalPosition);
		}

		// Token: 0x06000D01 RID: 3329 RVA: 0x000647FB File Offset: 0x000629FB
		public bool IsDestructable()
		{
			return true;
		}

		// Token: 0x06000D02 RID: 3330 RVA: 0x00064800 File Offset: 0x00062A00
		private float GetMultiplierOfShip()
		{
			float num = (float)this._navalAgentsLogic.GetActiveAgentCountOfShip(this._ship) / ((float)this._ship.CrewSizeOnMainDeck * 1f);
			num *= num;
			if (num < 0.0025000002f)
			{
				num = 0f;
			}
			float num2 = MathF.Max(1f, 2f - MathF.Log10(this._ship.HitPoints / this._ship.MaxHealth * 10f + 1f));
			return num * num2;
		}

		// Token: 0x040007E3 RID: 2019
		private readonly Vec3 BoundingBoxOffset = Vec3.One;

		// Token: 0x040007E4 RID: 2020
		private MissionShip _ship;

		// Token: 0x040007E5 RID: 2021
		private NavalAgentsLogic _navalAgentsLogic;
	}
}
