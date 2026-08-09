using System;
using NavalDLC.Missions.AI.UsableMachineAIs;
using NavalDLC.Missions.MissionLogics;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.Objects.UsableMachines
{
	// Token: 0x020000B4 RID: 180
	public class ShipBallista : Ballista
	{
		// Token: 0x17000278 RID: 632
		// (get) Token: 0x06000DC9 RID: 3529 RVA: 0x0006C5CA File Offset: 0x0006A7CA
		protected override float HorizontalAimSensitivity
		{
			get
			{
				return this._horizontalAimSensitivity;
			}
		}

		// Token: 0x17000279 RID: 633
		// (get) Token: 0x06000DCA RID: 3530 RVA: 0x0006C5D2 File Offset: 0x0006A7D2
		protected override float VerticalAimSensitivity
		{
			get
			{
				return this._verticalAimSensitivity;
			}
		}

		// Token: 0x1700027A RID: 634
		// (get) Token: 0x06000DCB RID: 3531 RVA: 0x0006C5DA File Offset: 0x0006A7DA
		protected override bool WeaponMovesDownToReload
		{
			get
			{
				return !(base.Ai as ShipBallistaAI).IsUnderDirectControl && base.PilotAgent.IsAIControlled;
			}
		}

		// Token: 0x1700027B RID: 635
		// (get) Token: 0x06000DCC RID: 3532 RVA: 0x0006C5FB File Offset: 0x0006A7FB
		public override string MultipleProjectileId
		{
			get
			{
				return "ballista_c_projectile_grape";
			}
		}

		// Token: 0x1700027C RID: 636
		// (get) Token: 0x06000DCD RID: 3533 RVA: 0x0006C602 File Offset: 0x0006A802
		public override string MultipleProjectileFlyingId
		{
			get
			{
				return "ballista_c_projectile_grape_projectile";
			}
		}

		// Token: 0x1700027D RID: 637
		// (get) Token: 0x06000DCE RID: 3534 RVA: 0x0006C609 File Offset: 0x0006A809
		public override string MultipleFireProjectileId
		{
			get
			{
				return "ballista_c_projectile_grape_fire";
			}
		}

		// Token: 0x1700027E RID: 638
		// (get) Token: 0x06000DCF RID: 3535 RVA: 0x0006C610 File Offset: 0x0006A810
		public override string MultipleFireProjectileFlyingId
		{
			get
			{
				return "ballista_c_projectile_grape_fire_projectile";
			}
		}

		// Token: 0x06000DD0 RID: 3536 RVA: 0x0006C618 File Offset: 0x0006A818
		protected override void OnInit()
		{
			this._ship = base.GameEntity.Root.GetFirstScriptOfType<MissionShip>();
			base.OnInit();
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this._navalShipsLogic.ShipSpawnedEvent += this.OnShipSpawned;
		}

		// Token: 0x06000DD1 RID: 3537 RVA: 0x0006C66E File Offset: 0x0006A86E
		private void OnShipSpawned(MissionShip ship)
		{
			if (ship == this._ship)
			{
				this.DefaultSide = ship.BattleSide;
			}
			this._navalShipsLogic.ShipSpawnedEvent -= this.OnShipSpawned;
		}

		// Token: 0x06000DD2 RID: 3538 RVA: 0x0006C69C File Offset: 0x0006A89C
		public override float GetTargetReleaseAngle(Vec3 target)
		{
			Vec3 globalVelocity = this.GetGlobalVelocity();
			float num = (this.ShootingSpeed * this.ShootingDirection + globalVelocity).Normalize();
			return Mission.GetMissileVerticalAimCorrection(target - base.MissileStartingGlobalPositionForSimulation, num, ref this.OriginalMissileWeaponStatsDataForTargeting, ItemObject.GetAirFrictionConstant(this.OriginalMissileItem.PrimaryWeapon.WeaponClass, this.OriginalMissileItem.PrimaryWeapon.WeaponFlags)) + MBMath.ToRadians(base.GameEntity.GetGlobalFrame().rotation.GetEulerAngles().x);
		}

		// Token: 0x06000DD3 RID: 3539 RVA: 0x0006C734 File Offset: 0x0006A934
		public override Vec3 GetEstimatedTargetMovementVector(Vec3 targetPosition, Vec3 targetVelocity)
		{
			Vec3 vec = this.ShootingSpeed * this.ShootingDirection + this.GetGlobalVelocity();
			float num = vec.Normalize();
			float num2 = 0f;
			float num3 = this.GetMissileTravelTimeApproximation(base.MissileStartingGlobalPositionForSimulation, targetPosition, vec * num, ItemObject.GetAirFrictionConstant(this.OriginalMissileItem.PrimaryWeapon.WeaponClass, this.OriginalMissileItem.PrimaryWeapon.WeaponFlags));
			Vec3 vec2 = targetPosition + targetVelocity * num3;
			int num4 = 0;
			while (MathF.Abs(num3 - num2) > 1E-05f && num4++ < 10)
			{
				num2 = num3;
				num3 = this.GetMissileTravelTimeApproximation(base.MissileStartingGlobalPositionForSimulation, vec2, vec * num, ItemObject.GetAirFrictionConstant(this.OriginalMissileItem.PrimaryWeapon.WeaponClass, this.OriginalMissileItem.PrimaryWeapon.WeaponFlags));
				vec2 = targetPosition + targetVelocity * num3;
			}
			return vec2 - targetPosition;
		}

		// Token: 0x06000DD4 RID: 3540 RVA: 0x0006C828 File Offset: 0x0006AA28
		private float GetMissileTravelTimeApproximation(Vec3 startingPos, Vec3 targetPos, Vec3 velocity, float airFriction)
		{
			Vec3 vec = startingPos;
			float num = 0f;
			do
			{
				vec += velocity * 0.02f;
				velocity += MBGlobals.GravitationalAcceleration * 0.02f;
				float num2 = velocity.Normalize();
				num2 -= airFriction * num2 * num2 * 0.02f;
				velocity *= num2;
				num += 0.02f;
			}
			while (vec.DistanceSquared(targetPos) >= 0.1f && (vec.DistanceSquared(startingPos) <= 100f || vec.z >= targetPos.z));
			return num;
		}

		// Token: 0x06000DD5 RID: 3541 RVA: 0x0006C8BC File Offset: 0x0006AABC
		protected override Mission.Missile ShootProjectileAux(ItemObject missileItem, bool randomizeMissileSpeed)
		{
			Vec3 vec;
			Mat3 mat;
			float num;
			float num2;
			base.SetupProjectileToShoot(randomizeMissileSpeed, ref vec, ref mat, ref num, ref num2);
			if (base.PlayerForceUse)
			{
				this.LastShooterAgent = Agent.Main;
			}
			MissionObject missionObject = base.GameEntity.Root.GetFirstScriptOfType<MissionObject>() ?? this;
			Mission mission = Mission.Current;
			Agent lastShooterAgent = this.LastShooterAgent;
			ItemModifier itemModifier = null;
			IAgentOriginBase origin = this.LastShooterAgent.Origin;
			Mission.Missile missile = mission.AddCustomMissile(lastShooterAgent, new MissionWeapon(missileItem, itemModifier, (origin != null) ? origin.Banner : null, 1), this.ProjectileEntityCurrentGlobalPosition, vec, mat, num2, num, false, missionObject, -1);
			this._navalShipsLogic.AddShipSiegeEngineMissile(missile);
			return missile;
		}

		// Token: 0x06000DD6 RID: 3542 RVA: 0x0006C958 File Offset: 0x0006AB58
		public override Vec3 GetGlobalVelocity()
		{
			return GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(this._ship.GameEntity, base.MissileStartingGlobalPositionForSimulation);
		}

		// Token: 0x06000DD7 RID: 3543 RVA: 0x0006C970 File Offset: 0x0006AB70
		protected override bool CheckFriendlyFireForObjects(Vec3 globalTargetPosition)
		{
			if (base.CheckFriendlyFireForObjects(globalTargetPosition))
			{
				return true;
			}
			foreach (MissionShip missionShip in this._ship.ShipsLogic.AllShips)
			{
				if (missionShip != this._ship && missionShip.Team != null && this._ship.Team != null && missionShip.Team.TeamSide == this._ship.Team.TeamSide)
				{
					MatrixFrame globalFrame = missionShip.GameEntity.GetGlobalFrame();
					Vec3 max = missionShip.Physics.PhysicsBoundingBoxWithChildren.max;
					Vec3 min = missionShip.Physics.PhysicsBoundingBoxWithChildren.min;
					Vec3 center = missionShip.Physics.PhysicsBoundingBoxWithChildren.center;
					Vec2 asVec = globalFrame.TransformToParent(ref center).AsVec2;
					Vec2 vec = globalFrame.rotation.f.AsVec2.Normalized();
					Vec2 asVec2 = (max - min).AsVec2;
					Oriented2DArea oriented2DArea = new Oriented2DArea(ref asVec, ref vec, ref asVec2);
					LineSegment2D lineSegment2D;
					lineSegment2D..ctor(globalTargetPosition.AsVec2, base.MissileStartingGlobalPositionForSimulation.AsVec2);
					if (oriented2DArea.Intersects(ref lineSegment2D, 1f))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000DD8 RID: 3544 RVA: 0x0006CAF4 File Offset: 0x0006ACF4
		public override float ProcessTargetValue(float baseValue, TargetFlags flags)
		{
			if (Extensions.HasAnyFlag<TargetFlags>(flags, 64))
			{
				return -1000f;
			}
			if (Extensions.HasAnyFlag<TargetFlags>(flags, 512))
			{
				baseValue *= 2f;
			}
			if (Extensions.HasAnyFlag<TargetFlags>(flags, 128))
			{
				baseValue *= 10000f;
			}
			return baseValue;
		}

		// Token: 0x06000DD9 RID: 3545 RVA: 0x0006CB33 File Offset: 0x0006AD33
		protected override void DetermineDefaultBattleSide()
		{
			this.DefaultSide = this._ship.BattleSide;
		}

		// Token: 0x06000DDA RID: 3546 RVA: 0x0006CB46 File Offset: 0x0006AD46
		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new ShipBallistaAI(this);
		}

		// Token: 0x06000DDB RID: 3547 RVA: 0x0006CB4E File Offset: 0x0006AD4E
		protected override void GetSoundEventIndices()
		{
			this.MoveSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/ballista_naval/move");
			this.ReloadSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/ballista_naval/reload");
			this.FireSoundIndex = SoundEvent.GetEventIdFromString("event:/mission/ballista_naval/fire");
		}

		// Token: 0x04000896 RID: 2198
		private MissionShip _ship;

		// Token: 0x04000897 RID: 2199
		[EditableScriptComponentVariable(true, "")]
		private float _horizontalAimSensitivity = 0.5f;

		// Token: 0x04000898 RID: 2200
		[EditableScriptComponentVariable(true, "")]
		private float _verticalAimSensitivity = 0.5f;

		// Token: 0x04000899 RID: 2201
		private NavalShipsLogic _navalShipsLogic;
	}
}
