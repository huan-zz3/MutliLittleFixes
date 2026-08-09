using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NavalDLC.Missions.AI.Behaviors;
using NavalDLC.Missions.AI.TeamAI;
using NavalDLC.Missions.NavalPhysics;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.Objects.UsableMachines;
using NavalDLC.Missions.ShipActuators;
using NavalDLC.Missions.ShipControl;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.MissionLogics
{
	// Token: 0x020000D4 RID: 212
	public class NavalShipsLogic : MissionLogic, IVehicleHandler, IMissionBehavior
	{
		// Token: 0x1400000D RID: 13
		// (add) Token: 0x06001001 RID: 4097 RVA: 0x000799E8 File Offset: 0x00077BE8
		// (remove) Token: 0x06001002 RID: 4098 RVA: 0x00079A20 File Offset: 0x00077C20
		public event Action<MissionShip> ShipSpawnedEvent;

		// Token: 0x1400000E RID: 14
		// (add) Token: 0x06001003 RID: 4099 RVA: 0x00079A58 File Offset: 0x00077C58
		// (remove) Token: 0x06001004 RID: 4100 RVA: 0x00079A90 File Offset: 0x00077C90
		public event Action<MissionShip, Formation> BeforeShipTransferredToFormationEvent;

		// Token: 0x1400000F RID: 15
		// (add) Token: 0x06001005 RID: 4101 RVA: 0x00079AC8 File Offset: 0x00077CC8
		// (remove) Token: 0x06001006 RID: 4102 RVA: 0x00079B00 File Offset: 0x00077D00
		public event Action<MissionShip, Formation> ShipTransferredToFormationEvent;

		// Token: 0x14000010 RID: 16
		// (add) Token: 0x06001007 RID: 4103 RVA: 0x00079B38 File Offset: 0x00077D38
		// (remove) Token: 0x06001008 RID: 4104 RVA: 0x00079B70 File Offset: 0x00077D70
		public event Action<MissionShip, Team, Formation> BeforeShipTransferredToTeamEvent;

		// Token: 0x14000011 RID: 17
		// (add) Token: 0x06001009 RID: 4105 RVA: 0x00079BA8 File Offset: 0x00077DA8
		// (remove) Token: 0x0600100A RID: 4106 RVA: 0x00079BE0 File Offset: 0x00077DE0
		public event Action<MissionShip, Team, Formation> ShipTransferredToTeamEvent;

		// Token: 0x14000012 RID: 18
		// (add) Token: 0x0600100B RID: 4107 RVA: 0x00079C18 File Offset: 0x00077E18
		// (remove) Token: 0x0600100C RID: 4108 RVA: 0x00079C50 File Offset: 0x00077E50
		public event Action<MissionShip, MissionShip, Formation, Formation> ShipCapturedEvent;

		// Token: 0x14000013 RID: 19
		// (add) Token: 0x0600100D RID: 4109 RVA: 0x00079C88 File Offset: 0x00077E88
		// (remove) Token: 0x0600100E RID: 4110 RVA: 0x00079CC0 File Offset: 0x00077EC0
		public event Action<MissionShip> ShipSunkEvent;

		// Token: 0x14000014 RID: 20
		// (add) Token: 0x0600100F RID: 4111 RVA: 0x00079CF8 File Offset: 0x00077EF8
		// (remove) Token: 0x06001010 RID: 4112 RVA: 0x00079D30 File Offset: 0x00077F30
		public event Action<ShipAttachmentMachine, ShipAttachmentPointMachine> ShipAttachmentBrokenEvent;

		// Token: 0x14000015 RID: 21
		// (add) Token: 0x06001011 RID: 4113 RVA: 0x00079D68 File Offset: 0x00077F68
		// (remove) Token: 0x06001012 RID: 4114 RVA: 0x00079DA0 File Offset: 0x00077FA0
		public event Action<MissionShip, MatrixFrame, MatrixFrame> ShipTeleportedEvent;

		// Token: 0x14000016 RID: 22
		// (add) Token: 0x06001013 RID: 4115 RVA: 0x00079DD8 File Offset: 0x00077FD8
		// (remove) Token: 0x06001014 RID: 4116 RVA: 0x00079E10 File Offset: 0x00078010
		public event Action<MissionShip> BeforeShipRemovedEvent;

		// Token: 0x14000017 RID: 23
		// (add) Token: 0x06001015 RID: 4117 RVA: 0x00079E48 File Offset: 0x00078048
		// (remove) Token: 0x06001016 RID: 4118 RVA: 0x00079E80 File Offset: 0x00078080
		public event Action<MissionShip> ShipRemovedEvent;

		// Token: 0x14000018 RID: 24
		// (add) Token: 0x06001017 RID: 4119 RVA: 0x00079EB8 File Offset: 0x000780B8
		// (remove) Token: 0x06001018 RID: 4120 RVA: 0x00079EF0 File Offset: 0x000780F0
		public event Action<MissionShip> ShipControllerChanged;

		// Token: 0x14000019 RID: 25
		// (add) Token: 0x06001019 RID: 4121 RVA: 0x00079F28 File Offset: 0x00078128
		// (remove) Token: 0x0600101A RID: 4122 RVA: 0x00079F60 File Offset: 0x00078160
		public event Action<MissionShip, MissionShip, float, bool, CapsuleData, int> ShipRammingEvent;

		// Token: 0x1400001A RID: 26
		// (add) Token: 0x0600101B RID: 4123 RVA: 0x00079F98 File Offset: 0x00078198
		// (remove) Token: 0x0600101C RID: 4124 RVA: 0x00079FD0 File Offset: 0x000781D0
		public event Action<MissionShip, MissionShip> ShipsConnectedEvent;

		// Token: 0x1400001B RID: 27
		// (add) Token: 0x0600101D RID: 4125 RVA: 0x0007A008 File Offset: 0x00078208
		// (remove) Token: 0x0600101E RID: 4126 RVA: 0x0007A040 File Offset: 0x00078240
		public event Action<MissionShip, WeakGameEntity, BodyFlags, Vec3, Vec3, bool> ShipCollisionEvent;

		// Token: 0x1400001C RID: 28
		// (add) Token: 0x0600101F RID: 4127 RVA: 0x0007A078 File Offset: 0x00078278
		// (remove) Token: 0x06001020 RID: 4128 RVA: 0x0007A0B0 File Offset: 0x000782B0
		public event Action<MissionShip, MissionShip> ShipHookThrowEvent;

		// Token: 0x1400001D RID: 29
		// (add) Token: 0x06001021 RID: 4129 RVA: 0x0007A0E8 File Offset: 0x000782E8
		// (remove) Token: 0x06001022 RID: 4130 RVA: 0x0007A120 File Offset: 0x00078320
		public event Action MissionEndEvent;

		// Token: 0x1400001E RID: 30
		// (add) Token: 0x06001023 RID: 4131 RVA: 0x0007A158 File Offset: 0x00078358
		// (remove) Token: 0x06001024 RID: 4132 RVA: 0x0007A190 File Offset: 0x00078390
		public event Action<MissionShip, Agent, int, Vec3, Vec3, MissionWeapon, int> ShipHitEvent;

		// Token: 0x1400001F RID: 31
		// (add) Token: 0x06001025 RID: 4133 RVA: 0x0007A1C8 File Offset: 0x000783C8
		// (remove) Token: 0x06001026 RID: 4134 RVA: 0x0007A200 File Offset: 0x00078400
		public event Action<MissionShip> ShipBurnedEvent;

		// Token: 0x14000020 RID: 32
		// (add) Token: 0x06001027 RID: 4135 RVA: 0x0007A238 File Offset: 0x00078438
		// (remove) Token: 0x06001028 RID: 4136 RVA: 0x0007A270 File Offset: 0x00078470
		public event Action<MissionShip> ShipPreparedForAbandonmentEvent;

		// Token: 0x14000021 RID: 33
		// (add) Token: 0x06001029 RID: 4137 RVA: 0x0007A2A8 File Offset: 0x000784A8
		// (remove) Token: 0x0600102A RID: 4138 RVA: 0x0007A2E0 File Offset: 0x000784E0
		public event Action<MissionShip> SailsDeadEvent;

		// Token: 0x14000022 RID: 34
		// (add) Token: 0x0600102B RID: 4139 RVA: 0x0007A318 File Offset: 0x00078518
		// (remove) Token: 0x0600102C RID: 4140 RVA: 0x0007A350 File Offset: 0x00078550
		public event Action<MissionShip> ShipLowHealthEvent;

		// Token: 0x14000023 RID: 35
		// (add) Token: 0x0600102D RID: 4141 RVA: 0x0007A388 File Offset: 0x00078588
		// (remove) Token: 0x0600102E RID: 4142 RVA: 0x0007A3C0 File Offset: 0x000785C0
		public event Action<MissionShip, MissionShip, float, float> ShipAboutToBeRammedEvent;

		// Token: 0x14000024 RID: 36
		// (add) Token: 0x0600102F RID: 4143 RVA: 0x0007A3F8 File Offset: 0x000785F8
		// (remove) Token: 0x06001030 RID: 4144 RVA: 0x0007A430 File Offset: 0x00078630
		public event Action<MissionShip, MissionShip> ShipAttachmentLostEvent;

		// Token: 0x14000025 RID: 37
		// (add) Token: 0x06001031 RID: 4145 RVA: 0x0007A468 File Offset: 0x00078668
		// (remove) Token: 0x06001032 RID: 4146 RVA: 0x0007A4A0 File Offset: 0x000786A0
		public event Action<MissionShip, MissionShip> BridgeConnectedEvent;

		// Token: 0x14000026 RID: 38
		// (add) Token: 0x06001033 RID: 4147 RVA: 0x0007A4D8 File Offset: 0x000786D8
		// (remove) Token: 0x06001034 RID: 4148 RVA: 0x0007A510 File Offset: 0x00078710
		public event Action<MissionShip> CutLooseOrderEvent;

		// Token: 0x14000027 RID: 39
		// (add) Token: 0x06001035 RID: 4149 RVA: 0x0007A548 File Offset: 0x00078748
		// (remove) Token: 0x06001036 RID: 4150 RVA: 0x0007A580 File Offset: 0x00078780
		public event Action<MissionShip, MissionShip> BoardingOrderEvent;

		// Token: 0x14000028 RID: 40
		// (add) Token: 0x06001037 RID: 4151 RVA: 0x0007A5B8 File Offset: 0x000787B8
		// (remove) Token: 0x06001038 RID: 4152 RVA: 0x0007A5F0 File Offset: 0x000787F0
		public event Action<Mission.Missile> AddShipSiegeEngineMissileEvent;

		// Token: 0x170002E8 RID: 744
		// (get) Token: 0x06001039 RID: 4153 RVA: 0x0007A625 File Offset: 0x00078825
		// (set) Token: 0x0600103A RID: 4154 RVA: 0x0007A62D File Offset: 0x0007882D
		public MissionShip PlayerControlledShip { get; private set; }

		// Token: 0x170002E9 RID: 745
		// (get) Token: 0x0600103B RID: 4155 RVA: 0x0007A636 File Offset: 0x00078836
		// (set) Token: 0x0600103C RID: 4156 RVA: 0x0007A63E File Offset: 0x0007883E
		public bool SeaPathfindingEnabled { get; private set; }

		// Token: 0x170002EA RID: 746
		// (get) Token: 0x0600103D RID: 4157 RVA: 0x0007A647 File Offset: 0x00078847
		// (set) Token: 0x0600103E RID: 4158 RVA: 0x0007A64F File Offset: 0x0007884F
		public bool IsTeleportingShips { get; private set; }

		// Token: 0x170002EB RID: 747
		// (get) Token: 0x0600103F RID: 4159 RVA: 0x0007A658 File Offset: 0x00078858
		// (set) Token: 0x06001040 RID: 4160 RVA: 0x0007A660 File Offset: 0x00078860
		public bool IsMissionEnding { get; private set; }

		// Token: 0x170002EC RID: 748
		// (get) Token: 0x06001041 RID: 4161 RVA: 0x0007A669 File Offset: 0x00078869
		// (set) Token: 0x06001042 RID: 4162 RVA: 0x0007A671 File Offset: 0x00078871
		public bool IsDeploymentMode { get; private set; }

		// Token: 0x170002ED RID: 749
		// (get) Token: 0x06001043 RID: 4163 RVA: 0x0007A67A File Offset: 0x0007887A
		public MBReadOnlyList<MissionShip> AllShips
		{
			get
			{
				return this._allShips;
			}
		}

		// Token: 0x170002EE RID: 750
		// (get) Token: 0x06001044 RID: 4164 RVA: 0x0007A682 File Offset: 0x00078882
		// (set) Token: 0x06001045 RID: 4165 RVA: 0x0007A68A File Offset: 0x0007888A
		public bool CanHaveConnectionCooldown { get; private set; } = true;

		// Token: 0x06001046 RID: 4166 RVA: 0x0007A694 File Offset: 0x00078894
		public NavalShipsLogic()
		{
			this._shipIndexGenerator = 0;
			this._allShips = new MBList<MissionShip>();
			this._tmpCollisionFreeFrameSearchQueue = new PriorityQueue<int, ValueTuple<int, Vec2i>>();
			this._removedShipsPool = new MBList<MissionShip>();
			this._shipAssignments = new ShipAssignment[3, 11];
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 11; j++)
				{
					this._shipAssignments[i, j] = ShipAssignment.Create(i, j, null);
				}
			}
			int num = 3;
			this._teamDeploymentLimits = new NavalShipDeploymentLimit[num];
			for (int k = 0; k < num; k++)
			{
				this._teamDeploymentLimits[k] = NavalShipDeploymentLimit.Invalid();
			}
			this._shipSiegeEngineMissileDictionary = new Dictionary<int, Mission.Missile>();
			this._missileHittingSailDictionary = new Dictionary<int, float>();
		}

		// Token: 0x06001047 RID: 4167 RVA: 0x0007A753 File Offset: 0x00078953
		public bool IsMissileFromShipSiegeEngine(int missileIndex)
		{
			return this._shipSiegeEngineMissileDictionary.ContainsKey(missileIndex);
		}

		// Token: 0x06001048 RID: 4168 RVA: 0x0007A761 File Offset: 0x00078961
		public void AddShipSiegeEngineMissile(Mission.Missile missile)
		{
			if (!this._shipSiegeEngineMissileDictionary.ContainsKey(missile.Index))
			{
				this._shipSiegeEngineMissileDictionary.Add(missile.Index, missile);
				Action<Mission.Missile> addShipSiegeEngineMissileEvent = this.AddShipSiegeEngineMissileEvent;
				if (addShipSiegeEngineMissileEvent == null)
				{
					return;
				}
				addShipSiegeEngineMissileEvent(missile);
			}
		}

		// Token: 0x06001049 RID: 4169 RVA: 0x0007A79C File Offset: 0x0007899C
		private void AddMissileHittingSail(Mission.Missile missile)
		{
			if (!this._missileHittingSailDictionary.ContainsKey(missile.Index))
			{
				this._missileHittingSailDictionary.Add(missile.Index, missile.GetVelocity().Length);
			}
		}

		// Token: 0x0600104A RID: 4170 RVA: 0x0007A7DC File Offset: 0x000789DC
		public float GetMissileVelocityLengthOnFirstSailHit(int missileIndex)
		{
			float num;
			if (this._missileHittingSailDictionary.TryGetValue(missileIndex, out num))
			{
				return num;
			}
			return -1f;
		}

		// Token: 0x0600104B RID: 4171 RVA: 0x0007A800 File Offset: 0x00078A00
		public override void OnMissileRemoved(int MissileIndex)
		{
			base.OnMissileRemoved(MissileIndex);
			if (this._shipSiegeEngineMissileDictionary.ContainsKey(MissileIndex))
			{
				this._shipSiegeEngineMissileDictionary.Remove(MissileIndex);
				return;
			}
			if (this._missileHittingSailDictionary.ContainsKey(MissileIndex))
			{
				this._missileHittingSailDictionary.Remove(MissileIndex);
			}
		}

		// Token: 0x0600104C RID: 4172 RVA: 0x0007A840 File Offset: 0x00078A40
		public override void OnDeploymentFinished()
		{
			foreach (MissionShip missionShip in this._allShips)
			{
				missionShip.OnDeploymentFinished();
			}
		}

		// Token: 0x0600104D RID: 4173 RVA: 0x0007A890 File Offset: 0x00078A90
		public override void OnBehaviorInitialize()
		{
			Mission.Current.OnMissileRemovedEvent += this.OnMissileRemoved;
		}

		// Token: 0x0600104E RID: 4174 RVA: 0x0007A8AC File Offset: 0x00078AAC
		public override void AfterStart()
		{
			base.AfterStart();
			MissionShipFactory.ResetShipUniqueBitwiseIDNext();
			this.SeaPathfindingEnabled = (base.Mission.IsNavalBattle || base.Mission.IsNavalRaidBattle) && base.Mission.Scene.SetAbilityOfFacesWithId(1, false) > 0;
		}

		// Token: 0x0600104F RID: 4175 RVA: 0x0007A8FC File Offset: 0x00078AFC
		public override void OnMissionTick(float dt)
		{
			foreach (Mission.Missile missile in base.Mission.MissilesList)
			{
				MissionWeapon missionWeapon = missile.Weapon;
				WeaponComponentData currentUsageItem = missionWeapon.CurrentUsageItem;
				if (currentUsageItem != null && Extensions.HasAnyFlag<WeaponFlags>(currentUsageItem.WeaponFlags, 32768L))
				{
					Vec3 oldPosition = missile.GetOldPosition();
					Vec3 position = missile.GetPosition();
					foreach (MissionShip missionShip in this.AllShips)
					{
						MissionShip missionShip2 = missionShip;
						Agent shooterAgent = missile.ShooterAgent;
						Mission.Missile missile2 = missile;
						missionWeapon = missile.Weapon;
						MissionSail missionSail = missionShip2.CheckHitSails(shooterAgent, missile2, in oldPosition, in position, in missionWeapon);
						if (missionSail != null)
						{
							this.HandleSailsHit(missionShip, missionSail, missile.ShooterAgent, missile, missile.Weapon);
							missile.PassThroughEntity(missionSail.SailEntity);
						}
					}
				}
			}
		}

		// Token: 0x06001050 RID: 4176 RVA: 0x0007AA14 File Offset: 0x00078C14
		protected override void OnEndMission()
		{
			base.OnEndMission();
			this.IsMissionEnding = true;
			Action missionEndEvent = this.MissionEndEvent;
			if (missionEndEvent != null)
			{
				missionEndEvent();
			}
			foreach (MissionShip missionShip in this._allShips.ToList<MissionShip>())
			{
				this.RemoveShip(missionShip);
			}
			this.SetDeploymentMode(false);
			Mission.Current.OnMissileRemovedEvent -= this.OnMissileRemoved;
		}

		// Token: 0x06001051 RID: 4177 RVA: 0x0007AAA8 File Offset: 0x00078CA8
		public void OnShipControllerChanged(MissionShip ship)
		{
			if (ship.IsPlayerControlled && this.PlayerControlledShip != ship)
			{
				this.PlayerControlledShip = ship;
			}
			else if (!ship.IsPlayerControlled && this.PlayerControlledShip == ship)
			{
				this.PlayerControlledShip = null;
			}
			Action<MissionShip> shipControllerChanged = this.ShipControllerChanged;
			if (shipControllerChanged == null)
			{
				return;
			}
			shipControllerChanged(ship);
		}

		// Token: 0x06001052 RID: 4178 RVA: 0x0007AAF8 File Offset: 0x00078CF8
		public void OnShipSunk(MissionShip ship)
		{
			Action<MissionShip> shipSunkEvent = this.ShipSunkEvent;
			if (shipSunkEvent == null)
			{
				return;
			}
			shipSunkEvent(ship);
		}

		// Token: 0x06001053 RID: 4179 RVA: 0x0007AB0B File Offset: 0x00078D0B
		public void OnAttachmentBroken(ShipAttachmentMachine attachmentMachine, ShipAttachmentPointMachine attachmentPointMachine)
		{
			Action<ShipAttachmentMachine, ShipAttachmentPointMachine> shipAttachmentBrokenEvent = this.ShipAttachmentBrokenEvent;
			if (shipAttachmentBrokenEvent == null)
			{
				return;
			}
			shipAttachmentBrokenEvent(attachmentMachine, attachmentPointMachine);
		}

		// Token: 0x06001054 RID: 4180 RVA: 0x0007AB1F File Offset: 0x00078D1F
		public void OnShipCollision(MissionShip ship, WeakGameEntity targetEntity, BodyFlags bodyFlags, Vec3 averageContactPoint, Vec3 totalImpulseOnShip, bool isFirstImpact)
		{
			Action<MissionShip, WeakGameEntity, BodyFlags, Vec3, Vec3, bool> shipCollisionEvent = this.ShipCollisionEvent;
			if (shipCollisionEvent == null)
			{
				return;
			}
			shipCollisionEvent(ship, targetEntity, bodyFlags, averageContactPoint, totalImpulseOnShip, isFirstImpact);
		}

		// Token: 0x06001055 RID: 4181 RVA: 0x0007AB3A File Offset: 0x00078D3A
		public void OnShipRamming(MissionShip rammingShip, MissionShip rammedShip, float damagePercent, bool isFirstImpact, CapsuleData capsuleData, int ramQuality)
		{
			Action<MissionShip, MissionShip, float, bool, CapsuleData, int> shipRammingEvent = this.ShipRammingEvent;
			if (shipRammingEvent == null)
			{
				return;
			}
			shipRammingEvent(rammingShip, rammedShip, damagePercent, isFirstImpact, capsuleData, ramQuality);
		}

		// Token: 0x06001056 RID: 4182 RVA: 0x0007AB55 File Offset: 0x00078D55
		public void OnShipsConnected(MissionShip ownerShip, MissionShip targetShip)
		{
			Action<MissionShip, MissionShip> shipsConnectedEvent = this.ShipsConnectedEvent;
			if (shipsConnectedEvent == null)
			{
				return;
			}
			shipsConnectedEvent(ownerShip, targetShip);
		}

		// Token: 0x06001057 RID: 4183 RVA: 0x0007AB69 File Offset: 0x00078D69
		public void OnSuccessfulHookThrow(MissionShip hookingShip, MissionShip hookedShip)
		{
			Action<MissionShip, MissionShip> shipHookThrowEvent = this.ShipHookThrowEvent;
			if (shipHookThrowEvent == null)
			{
				return;
			}
			shipHookThrowEvent(hookingShip, hookedShip);
		}

		// Token: 0x06001058 RID: 4184 RVA: 0x0007AB7D File Offset: 0x00078D7D
		public void OnShipHit(MissionShip ship, Agent attackerAgent, int damage, Vec3 impactPosition, Vec3 impactDirection, in MissionWeapon weapon, int affectorWeaponSlotOrMissileIndex)
		{
			Action<MissionShip, Agent, int, Vec3, Vec3, MissionWeapon, int> shipHitEvent = this.ShipHitEvent;
			if (shipHitEvent == null)
			{
				return;
			}
			shipHitEvent(ship, attackerAgent, damage, impactPosition, impactDirection, weapon, affectorWeaponSlotOrMissileIndex);
		}

		// Token: 0x06001059 RID: 4185 RVA: 0x0007AB9F File Offset: 0x00078D9F
		public void OnShipPreparedForAbandonment(MissionShip ship)
		{
			Action<MissionShip> shipPreparedForAbandonmentEvent = this.ShipPreparedForAbandonmentEvent;
			if (shipPreparedForAbandonmentEvent == null)
			{
				return;
			}
			shipPreparedForAbandonmentEvent(ship);
		}

		// Token: 0x0600105A RID: 4186 RVA: 0x0007ABB2 File Offset: 0x00078DB2
		public void OnShipBurned(MissionShip ship)
		{
			Action<MissionShip> shipBurnedEvent = this.ShipBurnedEvent;
			if (shipBurnedEvent == null)
			{
				return;
			}
			shipBurnedEvent(ship);
		}

		// Token: 0x0600105B RID: 4187 RVA: 0x0007ABC5 File Offset: 0x00078DC5
		public void OnShipAttachmentLost(MissionShip hookingShip, MissionShip hookedShip)
		{
			Action<MissionShip, MissionShip> shipAttachmentLostEvent = this.ShipAttachmentLostEvent;
			if (shipAttachmentLostEvent == null)
			{
				return;
			}
			shipAttachmentLostEvent(hookingShip, hookedShip);
		}

		// Token: 0x0600105C RID: 4188 RVA: 0x0007ABD9 File Offset: 0x00078DD9
		public void OnSailsDead(MissionShip ship)
		{
			Action<MissionShip> sailsDeadEvent = this.SailsDeadEvent;
			if (sailsDeadEvent == null)
			{
				return;
			}
			sailsDeadEvent(ship);
		}

		// Token: 0x0600105D RID: 4189 RVA: 0x0007ABEC File Offset: 0x00078DEC
		public void OnShipLowHealth(MissionShip ship)
		{
			Action<MissionShip> shipLowHealthEvent = this.ShipLowHealthEvent;
			if (shipLowHealthEvent == null)
			{
				return;
			}
			shipLowHealthEvent(ship);
		}

		// Token: 0x0600105E RID: 4190 RVA: 0x0007ABFF File Offset: 0x00078DFF
		public void OnShipAboutToBeRammed(MissionShip rammingShip, MissionShip rammedShip, float distance, float speedInRamDirection)
		{
			Action<MissionShip, MissionShip, float, float> shipAboutToBeRammedEvent = this.ShipAboutToBeRammedEvent;
			if (shipAboutToBeRammedEvent == null)
			{
				return;
			}
			shipAboutToBeRammedEvent(rammingShip, rammedShip, distance, speedInRamDirection);
		}

		// Token: 0x0600105F RID: 4191 RVA: 0x0007AC16 File Offset: 0x00078E16
		public void OnCutLooseOrder(MissionShip ship)
		{
			Action<MissionShip> cutLooseOrderEvent = this.CutLooseOrderEvent;
			if (cutLooseOrderEvent == null)
			{
				return;
			}
			cutLooseOrderEvent(ship);
		}

		// Token: 0x06001060 RID: 4192 RVA: 0x0007AC29 File Offset: 0x00078E29
		public void OnBoardingOrder(MissionShip boardingShip, MissionShip boardedShip)
		{
			Action<MissionShip, MissionShip> boardingOrderEvent = this.BoardingOrderEvent;
			if (boardingOrderEvent == null)
			{
				return;
			}
			boardingOrderEvent(boardingShip, boardedShip);
		}

		// Token: 0x06001061 RID: 4193 RVA: 0x0007AC3D File Offset: 0x00078E3D
		public void OnBridgeConnected(MissionShip sourceShip, MissionShip targetShip)
		{
			Action<MissionShip, MissionShip> bridgeConnectedEvent = this.BridgeConnectedEvent;
			if (bridgeConnectedEvent == null)
			{
				return;
			}
			bridgeConnectedEvent(sourceShip, targetShip);
		}

		// Token: 0x06001062 RID: 4194 RVA: 0x0007AC54 File Offset: 0x00078E54
		public void SetDeploymentMode(bool value)
		{
			if (value != this.IsDeploymentMode)
			{
				this.IsDeploymentMode = value;
				if (!value)
				{
					foreach (MissionShip missionShip in this._removedShipsPool)
					{
						base.Mission.Scene.RemoveEntity(missionShip.GameEntity, 121);
					}
					this._removedShipsPool.Clear();
				}
			}
		}

		// Token: 0x06001063 RID: 4195 RVA: 0x0007ACD8 File Offset: 0x00078ED8
		public int GetNumTeamShips(TeamSideEnum teamSide)
		{
			int num = 0;
			for (int i = 0; i < 11; i++)
			{
				if (this._shipAssignments[teamSide, i].HasMissionShip)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06001064 RID: 4196 RVA: 0x0007AD0D File Offset: 0x00078F0D
		public bool GetShip(Formation formation, out MissionShip ship)
		{
			return this.GetShip(formation.Team.TeamSide, formation.FormationIndex, out ship);
		}

		// Token: 0x06001065 RID: 4197 RVA: 0x0007AD28 File Offset: 0x00078F28
		public bool GetShip(TeamSideEnum teamSide, FormationClass formationIndex, out MissionShip ship)
		{
			ShipAssignment shipAssignment = this.GetShipAssignment(teamSide, formationIndex);
			if (shipAssignment.HasMissionShip)
			{
				ship = shipAssignment.MissionShip;
				return true;
			}
			ship = null;
			return false;
		}

		// Token: 0x06001066 RID: 4198 RVA: 0x0007AD54 File Offset: 0x00078F54
		public void FillTeamShips(TeamSideEnum teamSide, MBList<MissionShip> teamShips)
		{
			for (int i = 0; i < 11; i++)
			{
				ShipAssignment shipAssignment = this._shipAssignments[teamSide, i];
				if (shipAssignment.HasMissionShip)
				{
					teamShips.Add(shipAssignment.MissionShip);
				}
			}
		}

		// Token: 0x06001067 RID: 4199 RVA: 0x0007AD90 File Offset: 0x00078F90
		public ShipAssignment GetShipAssignment(TeamSideEnum teamSide, FormationClass formationIndex)
		{
			return this._shipAssignments[teamSide, formationIndex];
		}

		// Token: 0x06001068 RID: 4200 RVA: 0x0007ADA0 File Offset: 0x00078FA0
		public bool GetShipAssignmentWithShipIndex(int shipIndex, out ShipAssignment shipAssignment)
		{
			ShipAssignment[,] shipAssignments = this._shipAssignments;
			int upperBound = shipAssignments.GetUpperBound(0);
			int upperBound2 = shipAssignments.GetUpperBound(1);
			for (int i = shipAssignments.GetLowerBound(0); i <= upperBound; i++)
			{
				for (int j = shipAssignments.GetLowerBound(1); j <= upperBound2; j++)
				{
					ShipAssignment shipAssignment2 = shipAssignments[i, j];
					if (shipAssignment2.HasMissionShip && shipAssignment2.MissionShip.Index == shipIndex)
					{
						shipAssignment = shipAssignment2;
						return true;
					}
				}
			}
			shipAssignment = null;
			return false;
		}

		// Token: 0x06001069 RID: 4201 RVA: 0x0007AE1C File Offset: 0x0007901C
		public int GetCountOfSetShipAssignments(TeamSideEnum teamSide)
		{
			int num = 0;
			ShipAssignment[,] shipAssignments = this._shipAssignments;
			int upperBound = shipAssignments.GetUpperBound(0);
			int upperBound2 = shipAssignments.GetUpperBound(1);
			for (int i = shipAssignments.GetLowerBound(0); i <= upperBound; i++)
			{
				for (int j = shipAssignments.GetLowerBound(1); j <= upperBound2; j++)
				{
					ShipAssignment shipAssignment = shipAssignments[i, j];
					if (shipAssignment.TeamSide == teamSide && shipAssignment.IsSet)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x0600106A RID: 4202 RVA: 0x0007AE94 File Offset: 0x00079094
		public bool GetShipWithShipIndex(int shipIndex, out MissionShip missionShip)
		{
			ShipAssignment shipAssignment;
			if (this.GetShipAssignmentWithShipIndex(shipIndex, out shipAssignment))
			{
				missionShip = shipAssignment.MissionShip;
				return true;
			}
			missionShip = null;
			return false;
		}

		// Token: 0x0600106B RID: 4203 RVA: 0x0007AEBC File Offset: 0x000790BC
		internal MissionShip GetConnectedTeamShip(TeamSideEnum teamSide, ulong shipUniqueBitwiseID)
		{
			MissionShip missionShip = null;
			for (int i = 0; i < 11; i++)
			{
				ShipAssignment shipAssignment = this._shipAssignments[teamSide, i];
				if (shipAssignment.HasMissionShip)
				{
					MissionShip missionShip2 = shipAssignment.MissionShip;
					if ((missionShip2.ShipIslandCombinedID & shipUniqueBitwiseID) != 0UL)
					{
						if (missionShip2.ShipUniqueBitwiseID == shipUniqueBitwiseID)
						{
							return missionShip2;
						}
						missionShip = missionShip2;
					}
				}
			}
			return missionShip;
		}

		// Token: 0x0600106C RID: 4204 RVA: 0x0007AF10 File Offset: 0x00079110
		internal MissionShip GetNearestTeamShip(TeamSideEnum teamSide, in Vec3 position, float maxDistance = 3.4028235E+38f, Func<MissionShip, bool> shipFilter = null)
		{
			MissionShip missionShip = null;
			float num = maxDistance;
			for (int i = 0; i < 11; i++)
			{
				ShipAssignment shipAssignment = this._shipAssignments[teamSide, i];
				if (shipAssignment.HasMissionShip)
				{
					MissionShip missionShip2 = shipAssignment.MissionShip;
					if (shipFilter == null || shipFilter(missionShip2))
					{
						float num2 = missionShip2.GlobalFrame.origin.DistanceSquared(position);
						if (num2 <= num)
						{
							num = num2;
							missionShip = missionShip2;
						}
					}
				}
			}
			return missionShip;
		}

		// Token: 0x0600106D RID: 4205 RVA: 0x0007AF84 File Offset: 0x00079184
		public void FillClosestShips(in MatrixFrame shipEntityGlobalFrame, float distanceLimit, [TupleElementNames(new string[] { "ship", "shipSide" })] MBList<ValueTuple<MissionShip, OarSidePhaseController.OarSide>> closestShips, MissionShip ignoreShip = null)
		{
			foreach (MissionShip missionShip in this._allShips)
			{
				if (missionShip != null && missionShip != ignoreShip)
				{
					BoundingBox boundingBox = missionShip.Physics.PhysicsBoundingBoxWithChildren;
					Vec2 vec = Vec2.Abs(boundingBox.max.AsVec2);
					boundingBox = missionShip.Physics.PhysicsBoundingBoxWithChildren;
					float num = distanceLimit + Vec2.Max(vec, Vec2.Abs(boundingBox.min.AsVec2)).Length;
					Vec3 origin = missionShip.GameEntity.GetBodyWorldTransform().origin;
					Vec3 origin2 = shipEntityGlobalFrame.origin;
					if (origin2.DistanceSquared(origin) <= num * num)
					{
						Vec3 vec2 = origin - shipEntityGlobalFrame.origin;
						OarSidePhaseController.OarSide oarSide = ((Vec3.CrossProduct(shipEntityGlobalFrame.rotation.f, vec2).z >= 0f) ? OarSidePhaseController.OarSide.Left : OarSidePhaseController.OarSide.Right);
						closestShips.Add(new ValueTuple<MissionShip, OarSidePhaseController.OarSide>(missionShip, oarSide));
					}
				}
			}
		}

		// Token: 0x0600106E RID: 4206 RVA: 0x0007B0A0 File Offset: 0x000792A0
		public MatrixFrame GetMeanFrameOfTeamShips(TeamSideEnum teamSide)
		{
			Vec3 vec = Vec3.Zero;
			Vec3 vec2 = Vec3.Forward;
			int num = 0;
			ShipAssignment[,] shipAssignments = this._shipAssignments;
			int upperBound = shipAssignments.GetUpperBound(0);
			int upperBound2 = shipAssignments.GetUpperBound(1);
			for (int i = shipAssignments.GetLowerBound(0); i <= upperBound; i++)
			{
				for (int j = shipAssignments.GetLowerBound(1); j <= upperBound2; j++)
				{
					ShipAssignment shipAssignment = shipAssignments[i, j];
					if (shipAssignment.HasMissionShip && shipAssignment.TeamSide == teamSide)
					{
						MatrixFrame globalFrame = shipAssignment.MissionShip.GlobalFrame;
						vec += globalFrame.origin;
						float num2 = 1f / ((float)num + 1f);
						vec2 = Vec3.Slerp(vec2, globalFrame.rotation.f, num2);
						num++;
					}
				}
			}
			Mat3 identity = Mat3.Identity;
			identity.f = vec2;
			identity.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			return new MatrixFrame(ref identity, ref vec);
		}

		// Token: 0x0600106F RID: 4207 RVA: 0x0007B190 File Offset: 0x00079390
		public bool GetCollisionFreeShipFrame(in MatrixFrame shipFrame, in Vec2 shipDimensions, out MatrixFrame collisionFreeFrame, bool checkBoundaries = true, NavalShipsLogic.NavalBoundaryCheckType boundaryToCheck = NavalShipsLogic.NavalBoundaryCheckType.HardBoundary, Team team = null, float searchCellWidth = 10f, int searchCellDistance = 10, float clearanceMargin = 1f, int ignoreShipIndex = -1)
		{
			Vec3 vec = shipFrame.rotation.f;
			Vec2 vec2 = vec.AsVec2.Normalized();
			vec = shipFrame.origin;
			Vec2 asVec = vec.AsVec2;
			searchCellWidth = MathF.Max(1f, searchCellWidth);
			searchCellDistance = MathF.Max(0, searchCellDistance);
			clearanceMargin = MathF.Max(0f, clearanceMargin);
			this._tmpCollisionFreeFrameSearchQueue.Clear();
			this._tmpCollisionFreeFrameSearchQueue.Enqueue(0, new ValueTuple<int, Vec2i>(0, Vec2i.Zero));
			Oriented2DArea oriented2DArea = new Oriented2DArea(ref Vec2.Zero, ref vec2, ref shipDimensions);
			bool flag2;
			do
			{
				ValueTuple<int, Vec2i> value = this._tmpCollisionFreeFrameSearchQueue.Dequeue().Value;
				Vec2i item = value.Item2;
				Vec2 vec3 = asVec + oriented2DArea.GlobalForward.RightVec() * searchCellWidth * (float)item.X + oriented2DArea.GlobalForward * searchCellWidth * (float)item.Y;
				oriented2DArea.SetGlobalCenter(ref vec3);
				bool flag = !checkBoundaries || NavalShipsLogic.IsPositionInsideBoundaries(in vec3, boundaryToCheck, team);
				flag2 = flag && this.IsAreaFreeOfShipCollision(in oriented2DArea, clearanceMargin, ignoreShipIndex);
				if (!flag2 && value.Item1 < searchCellDistance)
				{
					int num = value.Item1 + 1;
					int num2 = num + (flag ? 0 : searchCellDistance);
					if (item.X <= 0)
					{
						this._tmpCollisionFreeFrameSearchQueue.Enqueue(num2, new ValueTuple<int, Vec2i>(num, new Vec2i(item.X - 1, item.Y)));
					}
					if (item.X >= 0)
					{
						this._tmpCollisionFreeFrameSearchQueue.Enqueue(num2, new ValueTuple<int, Vec2i>(num, new Vec2i(item.X + 1, item.Y)));
					}
					if (item.X == 0)
					{
						if (item.Y >= 0)
						{
							this._tmpCollisionFreeFrameSearchQueue.Enqueue(num2, new ValueTuple<int, Vec2i>(num, new Vec2i(item.X, item.Y + 1)));
						}
						if (item.Y <= 0)
						{
							this._tmpCollisionFreeFrameSearchQueue.Enqueue(num2, new ValueTuple<int, Vec2i>(num, new Vec2i(item.X, item.Y - 1)));
						}
					}
				}
			}
			while (!Extensions.IsEmpty<KeyValuePair<int, ValueTuple<int, Vec2i>>>(this._tmpCollisionFreeFrameSearchQueue) && !flag2);
			collisionFreeFrame = MatrixFrame.Identity;
			if (flag2)
			{
				Vec3 vec4 = oriented2DArea.GlobalCenter.ToVec3(0f);
				vec4.z = base.Mission.Scene.GetWaterLevelAtPosition(oriented2DArea.GlobalCenter, true, false);
				collisionFreeFrame.origin = vec4;
				collisionFreeFrame.rotation.f = vec2.ToVec3(0f);
				collisionFreeFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
			}
			return flag2;
		}

		// Token: 0x06001070 RID: 4208 RVA: 0x0007B444 File Offset: 0x00079644
		public bool IsShipFrameCollisionFree(in MatrixFrame frame, in Vec2 localDimensions, bool checkBoundaries = true, NavalShipsLogic.NavalBoundaryCheckType boundaryToCheck = NavalShipsLogic.NavalBoundaryCheckType.HardBoundary, Team team = null, float clearanceMargin = 1f, int ignoreShipIndex = -1)
		{
			Vec3 vec = frame.origin;
			Vec2 asVec = vec.AsVec2;
			vec = frame.rotation.f;
			Vec2 vec2 = vec.AsVec2.Normalized();
			Oriented2DArea oriented2DArea = new Oriented2DArea(ref asVec, ref vec2, ref localDimensions);
			return (!checkBoundaries || NavalShipsLogic.IsPositionInsideBoundaries(in asVec, boundaryToCheck, team)) && this.IsAreaFreeOfShipCollision(in oriented2DArea, clearanceMargin, ignoreShipIndex);
		}

		// Token: 0x06001071 RID: 4209 RVA: 0x0007B4AC File Offset: 0x000796AC
		public float ComputeSpawnPathDeploymentOffset(Path path)
		{
			int num = 0;
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 11; j++)
				{
					if (this._shipAssignments[i, j].IsSet)
					{
						num++;
					}
				}
			}
			float num2 = 400f;
			if (num > 2)
			{
				int num3 = 14;
				float num4 = (float)Math.Min(num - 2, num3) / (float)num3;
				num2 += 460f * MathF.Pow(num4, 0.6f);
			}
			num2 = MathF.Min(path.GetTotalLength(), num2);
			return -num2 / 2f + 1f;
		}

		// Token: 0x06001072 RID: 4210 RVA: 0x0007B53C File Offset: 0x0007973C
		public bool FindAssignmentOfShipOrigin(IShipOrigin shipOrigin, out ShipAssignment shipAssignment)
		{
			shipAssignment = null;
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 11; j++)
				{
					ShipAssignment shipAssignment2 = this._shipAssignments[i, j];
					if (shipAssignment2.HasMissionShip && shipAssignment2.MissionShip.ShipOrigin == shipOrigin)
					{
						shipAssignment = shipAssignment2;
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06001073 RID: 4211 RVA: 0x0007B58F File Offset: 0x0007978F
		public void SetTeleportShips(bool value)
		{
			this.IsTeleportingShips = value;
		}

		// Token: 0x06001074 RID: 4212 RVA: 0x0007B598 File Offset: 0x00079798
		public void TeleportShip(MissionShip ship, MatrixFrame targetFrame, bool checkFreeArea, bool anchorShip = false, bool snapToWater = true)
		{
			MatrixFrame globalFrame = ship.GameEntity.GetGlobalFrame();
			this.TeleportShipAux(ship, ref targetFrame, checkFreeArea, anchorShip, snapToWater);
			Action<MissionShip, MatrixFrame, MatrixFrame> shipTeleportedEvent = this.ShipTeleportedEvent;
			if (shipTeleportedEvent == null)
			{
				return;
			}
			shipTeleportedEvent(ship, globalFrame, targetFrame);
		}

		// Token: 0x06001075 RID: 4213 RVA: 0x0007B5D8 File Offset: 0x000797D8
		public bool IsAreaFreeOfShipCollision(in Oriented2DArea area, float clearanceMargin = 1f, int ignoreShipIndex = -1)
		{
			foreach (MissionShip missionShip in this._allShips)
			{
				if (missionShip.Index != ignoreShipIndex && missionShip.Physics.NavalSinkingState != NavalPhysics.SinkingState.Sunk)
				{
					Oriented2DArea globalMaximal2DArea = missionShip.Physics.GetGlobalMaximal2DArea();
					Oriented2DArea oriented2DArea = area;
					if (oriented2DArea.Overlaps(ref globalMaximal2DArea, clearanceMargin))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06001076 RID: 4214 RVA: 0x0007B664 File Offset: 0x00079864
		public bool IsAShipAssignedToFormation(Formation formation)
		{
			return this.GetShipAssignment(formation.Team.TeamSide, formation.FormationIndex).HasMissionShip;
		}

		// Token: 0x06001077 RID: 4215 RVA: 0x0007B684 File Offset: 0x00079884
		public bool IsShipAssignedToFormation(MissionShip ship, Formation formation)
		{
			ShipAssignment shipAssignment = this.GetShipAssignment(formation.Team.TeamSide, formation.FormationIndex);
			return shipAssignment.HasMissionShip && shipAssignment.MissionShip == ship;
		}

		// Token: 0x06001078 RID: 4216 RVA: 0x0007B6BC File Offset: 0x000798BC
		public void ClearShipAssignments()
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 11; j++)
				{
					TeamSideEnum teamSideEnum = i;
					this.ClearShipAssignment(teamSideEnum, j);
				}
			}
		}

		// Token: 0x06001079 RID: 4217 RVA: 0x0007B6EC File Offset: 0x000798EC
		public ShipAssignment SetShipAssignment(TeamSideEnum teamSide, FormationClass formationIndex, IShipOrigin shipOrigin)
		{
			ShipAssignment shipAssignment = this._shipAssignments[teamSide, formationIndex];
			shipAssignment.Set(shipOrigin);
			return shipAssignment;
		}

		// Token: 0x0600107A RID: 4218 RVA: 0x0007B704 File Offset: 0x00079904
		public MissionShip SpawnShip(IShipOrigin shipOrigin, in MatrixFrame shipFrame, Team team, Formation formation = null, bool spawnAnchored = false, FormationClass formationSearchRange = 8, bool checkForFreeArea = true)
		{
			if (formation == null)
			{
				formation = this.FindFirstFormationWithoutShip(team, formationSearchRange);
			}
			TeamSideEnum teamSide = team.TeamSide;
			FormationClass formationIndex = formation.FormationIndex;
			this.GetShipAssignment(teamSide, formationIndex).Set(shipOrigin);
			return this.SpawnShip(formation, in shipFrame, spawnAnchored, checkForFreeArea);
		}

		// Token: 0x0600107B RID: 4219 RVA: 0x0007B74C File Offset: 0x0007994C
		public MissionShip SpawnShip(Formation formation, in MatrixFrame spawnFrame, bool spawnAnchored = true, bool checkForFreeArea = true)
		{
			TeamSideEnum teamSide = formation.Team.TeamSide;
			FormationClass formationIndex = formation.FormationIndex;
			int num = teamSide;
			ShipAssignment shipAssignment = this._shipAssignments[num, formationIndex];
			MatrixFrame matrixFrame = spawnFrame;
			IMissionDeploymentPlan deploymentPlan = base.Mission.DeploymentPlan;
			bool flag = deploymentPlan.IsPlanMade(formation.Team);
			if (matrixFrame.IsZero && flag)
			{
				matrixFrame = deploymentPlan.GetDeploymentFrame(formation.Team);
			}
			if (matrixFrame.IsZero)
			{
				matrixFrame = this.GetMeanFrameOfTeamShips(teamSide);
			}
			if (checkForFreeArea)
			{
				Vec2 deploymentArea = shipAssignment.MissionShipObject.DeploymentArea;
				MatrixFrame matrixFrame2;
				if (this.GetCollisionFreeShipFrame(in matrixFrame, in deploymentArea, out matrixFrame2, true, flag ? NavalShipsLogic.NavalBoundaryCheckType.DeploymentBoundary : NavalShipsLogic.NavalBoundaryCheckType.HardBoundary, formation.Team, 1f, 400, 1f, -1))
				{
					matrixFrame = matrixFrame2;
				}
			}
			float waterLevelAtPosition = base.Mission.Scene.GetWaterLevelAtPosition(matrixFrame.origin.AsVec2, true, true);
			matrixFrame.origin.z = waterLevelAtPosition;
			MissionShip missionShip = LinQuick.FirstOrDefaultQ<MissionShip>(this._removedShipsPool, (MissionShip ship) => ship.ShipOrigin == shipAssignment.ShipOrigin);
			MissionShip missionShip2;
			if (missionShip != null)
			{
				this._removedShipsPool.Remove(missionShip);
				missionShip.SetRemoved(false);
				missionShip.SetEnabledAndMakeVisible(true, true);
				missionShip.SetFormation(formation);
				shipAssignment.SetMissionShip(missionShip);
				missionShip2 = missionShip;
				missionShip2.ResetFormationPositioning();
			}
			else
			{
				MissionShipFactory.CreateMissionShip(this._shipIndexGenerator, shipAssignment, this, in matrixFrame);
				this._shipIndexGenerator++;
				missionShip2 = shipAssignment.MissionShip;
			}
			missionShip2.SetOriginalTeamSide(formation.Team.TeamSide);
			this._allShips.Add(missionShip2);
			if (missionShip != null)
			{
				this.TeleportShipAux(missionShip2, ref matrixFrame, false, false, true);
			}
			if (spawnAnchored)
			{
				missionShip2.SetAnchor(true, true, 1f);
			}
			Action<MissionShip> shipSpawnedEvent = this.ShipSpawnedEvent;
			if (shipSpawnedEvent != null)
			{
				shipSpawnedEvent(missionShip2);
			}
			return missionShip2;
		}

		// Token: 0x0600107C RID: 4220 RVA: 0x0007B934 File Offset: 0x00079B34
		public void RemoveShip(MissionShip ship)
		{
			ShipAssignment shipAssignment;
			this.FindAssignmentOfShipOrigin(ship.ShipOrigin, out shipAssignment);
			if (shipAssignment != null)
			{
				this.RemoveShipAux(shipAssignment);
			}
		}

		// Token: 0x0600107D RID: 4221 RVA: 0x0007B95C File Offset: 0x00079B5C
		public void RemoveShip(Formation formation)
		{
			TeamSideEnum teamSide = formation.Team.TeamSide;
			ShipAssignment shipAssignment = this.GetShipAssignment(teamSide, formation.FormationIndex);
			this.RemoveShipAux(shipAssignment);
		}

		// Token: 0x0600107E RID: 4222 RVA: 0x0007B98C File Offset: 0x00079B8C
		public void TransferShipToFormation(MissionShip ship, Formation toFormation)
		{
			ShipAssignment shipAssignment;
			this.FindAssignmentOfShipOrigin(ship.ShipOrigin, out shipAssignment);
			this.TransferShipToFormation(ship.ShipOrigin, shipAssignment.Formation, toFormation);
		}

		// Token: 0x0600107F RID: 4223 RVA: 0x0007B9BC File Offset: 0x00079BBC
		public void TransferShipToTeam(MissionShip ship, Team targetTeam, Formation targetFormation = null, FormationClass searchRange = 8)
		{
			ShipAssignment shipAssignment;
			bool flag = this.FindAssignmentOfShipOrigin(ship.ShipOrigin, out shipAssignment);
			if (targetFormation == null)
			{
				targetFormation = this.FindFirstFormationWithoutShip(targetTeam, searchRange);
			}
			Action<MissionShip, Team, Formation> beforeShipTransferredToTeamEvent = this.BeforeShipTransferredToTeamEvent;
			if (beforeShipTransferredToTeamEvent != null)
			{
				beforeShipTransferredToTeamEvent(ship, targetTeam, targetFormation);
			}
			Team team = ship.Team;
			Formation formation = ship.Formation;
			ShipAssignment shipAssignment2 = this.GetShipAssignment(targetFormation.Team.TeamSide, targetFormation.FormationIndex);
			if (flag)
			{
				shipAssignment.RemoveShip();
				shipAssignment.Clear();
			}
			shipAssignment2.Set(ship.ShipOrigin);
			shipAssignment2.SetMissionShip(ship);
			ship.SetFormation(targetFormation);
			Action<MissionShip, Team, Formation> shipTransferredToTeamEvent = this.ShipTransferredToTeamEvent;
			if (shipTransferredToTeamEvent == null)
			{
				return;
			}
			shipTransferredToTeamEvent(ship, team, formation);
		}

		// Token: 0x06001080 RID: 4224 RVA: 0x0007BA5C File Offset: 0x00079C5C
		private void RefreshTeamAIBehaviorShipReferences(Team team)
		{
			foreach (Formation formation in team.FormationsIncludingSpecialAndEmpty)
			{
				if (formation.AI != null)
				{
					for (int i = 0; i < formation.AI.BehaviorCount; i++)
					{
						NavalBehaviorComponent navalBehaviorComponent;
						if ((navalBehaviorComponent = formation.AI.GetBehaviorAtIndex(i) as NavalBehaviorComponent) != null)
						{
							navalBehaviorComponent.RefreshShipReferences();
						}
					}
				}
			}
		}

		// Token: 0x06001081 RID: 4225 RVA: 0x0007BAE4 File Offset: 0x00079CE4
		public void OnShipCaptured(MissionShip ship, Formation targetFormation)
		{
			Formation formation = ship.Formation;
			ShipAssignment shipAssignment;
			bool flag = this.FindAssignmentOfShipOrigin(ship.ShipOrigin, out shipAssignment);
			ShipAssignment shipAssignment2 = this.GetShipAssignment(targetFormation.Team.TeamSide, targetFormation.FormationIndex);
			MissionShip missionShip = shipAssignment2.MissionShip;
			shipAssignment2.SetMissionShip(ship);
			ship.SetFormation(null);
			missionShip.SetFormation(null);
			if (flag)
			{
				shipAssignment.RemoveShip();
				shipAssignment.Clear();
			}
			ship.SetFormation(targetFormation);
			ShipController controller = missionShip.Controller;
			ship.SetController((controller != null) ? controller.ControllerType : ShipControllerType.None, true);
			missionShip.SetController(ShipControllerType.None, true);
			foreach (MissionShip missionShip2 in this.AllShips)
			{
				missionShip2.ShipOrder.OnShipCaptured(ship, missionShip);
			}
			TeamAINavalComponent teamAINavalComponent = (TeamAINavalComponent)((formation != null) ? formation.Team.TeamAI : null);
			if (teamAINavalComponent != null)
			{
				teamAINavalComponent.TeamNavalQuerySystem.ForceExpireAll();
			}
			((TeamAINavalComponent)targetFormation.Team.TeamAI).TeamNavalQuerySystem.ForceExpireAll();
			Action<MissionShip, MissionShip, Formation, Formation> shipCapturedEvent = this.ShipCapturedEvent;
			if (shipCapturedEvent != null)
			{
				shipCapturedEvent(ship, missionShip, formation, targetFormation);
			}
			if (formation != null)
			{
				this.RefreshTeamAIBehaviorShipReferences(formation.Team);
			}
			this.RefreshTeamAIBehaviorShipReferences(targetFormation.Team);
		}

		// Token: 0x06001082 RID: 4226 RVA: 0x0007BC28 File Offset: 0x00079E28
		public Formation FindFirstFormationWithoutShip(Team team, FormationClass searchRange = 8)
		{
			int teamSide = team.TeamSide;
			FormationClass formationClass = 11;
			for (int i = 0; i < searchRange; i++)
			{
				ShipAssignment shipAssignment = this._shipAssignments[teamSide, i];
				if (!shipAssignment.IsSet && !shipAssignment.HasMissionShip)
				{
					formationClass = shipAssignment.FormationIndex;
					break;
				}
			}
			Formation formation;
			if (formationClass != 11)
			{
				formation = team.GetFormation(formationClass);
			}
			else
			{
				formation = null;
			}
			return formation;
		}

		// Token: 0x06001083 RID: 4227 RVA: 0x0007BC88 File Offset: 0x00079E88
		public void SetTeamShipDeploymentLimit(TeamSideEnum teamSide, NavalShipDeploymentLimit deploymentLimit)
		{
			this._teamDeploymentLimits[teamSide] = deploymentLimit;
		}

		// Token: 0x06001084 RID: 4228 RVA: 0x0007BC98 File Offset: 0x00079E98
		public void TransferShipToFormation(IShipOrigin shipOrigin, Formation fromFormation, Formation toFormation)
		{
			int teamSide = fromFormation.Team.TeamSide;
			int formationIndex = fromFormation.FormationIndex;
			int formationIndex2 = toFormation.FormationIndex;
			ShipAssignment shipAssignment = this._shipAssignments[teamSide, formationIndex];
			ShipAssignment shipAssignment2 = this._shipAssignments[teamSide, formationIndex2];
			MissionShip missionShip = shipAssignment.MissionShip;
			Action<MissionShip, Formation> beforeShipTransferredToFormationEvent = this.BeforeShipTransferredToFormationEvent;
			if (beforeShipTransferredToFormationEvent != null)
			{
				beforeShipTransferredToFormationEvent(missionShip, toFormation);
			}
			if (!shipAssignment2.IsSet)
			{
				shipAssignment2.Set(missionShip.ShipOrigin);
			}
			shipAssignment2.SetMissionShip(missionShip);
			shipAssignment.RemoveShip();
			shipAssignment.Clear();
			missionShip.SetFormation(toFormation);
			missionShip.ResetFormationPositioning();
			Action<MissionShip, Formation> shipTransferredToFormationEvent = this.ShipTransferredToFormationEvent;
			if (shipTransferredToFormationEvent == null)
			{
				return;
			}
			shipTransferredToFormationEvent(missionShip, fromFormation);
		}

		// Token: 0x06001085 RID: 4229 RVA: 0x0007BD40 File Offset: 0x00079F40
		public int GetShipDeploymentLimit(TeamSideEnum teamSide, out NavalShipDeploymentLimit deploymentLimit)
		{
			deploymentLimit = this._teamDeploymentLimits[teamSide];
			return deploymentLimit.NetDeploymentLimit;
		}

		// Token: 0x06001086 RID: 4230 RVA: 0x0007BD5A File Offset: 0x00079F5A
		public void SetCanHaveConnectionCooldown(bool value)
		{
			this.CanHaveConnectionCooldown = value;
		}

		// Token: 0x06001087 RID: 4231 RVA: 0x0007BD64 File Offset: 0x00079F64
		private void ClearShipAssignment(TeamSideEnum teamSide, int formationIndex)
		{
			this._shipAssignments[teamSide, formationIndex];
			this._shipAssignments[teamSide, formationIndex].Clear();
		}

		// Token: 0x06001088 RID: 4232 RVA: 0x0007BD94 File Offset: 0x00079F94
		bool IVehicleHandler.IsAgentInVehicle(Agent agent, out WeakGameEntity vehicleEntity)
		{
			foreach (MissionShip missionShip in this.AllShips)
			{
				if (missionShip.GetIsAgentOnShip(agent, false))
				{
					vehicleEntity = missionShip.GameEntity;
					return true;
				}
			}
			vehicleEntity = WeakGameEntity.Invalid;
			return false;
		}

		// Token: 0x06001089 RID: 4233 RVA: 0x0007BE08 File Offset: 0x0007A008
		private void RemoveShipAux(ShipAssignment shipAssignment)
		{
			MissionShip shipToRemove = shipAssignment.MissionShip;
			Action<MissionShip> beforeShipRemovedEvent = this.BeforeShipRemovedEvent;
			if (beforeShipRemovedEvent != null)
			{
				beforeShipRemovedEvent(shipToRemove);
			}
			shipAssignment.RemoveShip();
			shipAssignment.Clear();
			this._allShips.RemoveAll((MissionShip s) => s == shipToRemove);
			Action<MissionShip> shipRemovedEvent = this.ShipRemovedEvent;
			if (shipRemovedEvent != null)
			{
				shipRemovedEvent(shipToRemove);
			}
			shipToRemove.SetFormation(null);
			shipToRemove.SetRemoved(true);
			WeakGameEntity gameEntity = shipToRemove.GameEntity;
			if (this.IsDeploymentMode && !this.IsMissionEnding)
			{
				MatrixFrame globalFrame = shipToRemove.GlobalFrame;
				globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
				gameEntity.SetGlobalFrame(ref globalFrame, true);
				GameEntityPhysicsExtensions.SetAngularVelocity(gameEntity, Vec3.Zero);
				GameEntityPhysicsExtensions.SetLinearVelocity(gameEntity, Vec3.Zero);
				shipToRemove.SetDisabledAndMakeInvisible(true, true);
				this._removedShipsPool.Add(shipToRemove);
			}
			else
			{
				base.Mission.Scene.RemoveEntity(gameEntity, 121);
			}
			foreach (MissionShip missionShip in this.AllShips)
			{
				if (missionShip != shipToRemove && missionShip.HasController && missionShip.Controller.IsAIControlled && missionShip.AIController.TargetShip == shipToRemove)
				{
					missionShip.AIController.ClearTarget();
				}
			}
		}

		// Token: 0x0600108A RID: 4234 RVA: 0x0007BF9C File Offset: 0x0007A19C
		private void TeleportShipAux(MissionShip ship, ref MatrixFrame targetFrame, bool checkFreeArea, bool anchorShip = false, bool snapToWater = true)
		{
			if (checkFreeArea)
			{
				Vec2 deploymentArea = ship.MissionShipObject.DeploymentArea;
				MatrixFrame matrixFrame;
				if (this.GetCollisionFreeShipFrame(in targetFrame, in deploymentArea, out matrixFrame, true, (base.Mission.Mode == 6) ? NavalShipsLogic.NavalBoundaryCheckType.DeploymentBoundary : NavalShipsLogic.NavalBoundaryCheckType.HardBoundary, ship.Team, 1f, 400, 1f, ship.Index))
				{
					targetFrame = matrixFrame;
				}
			}
			if (snapToWater)
			{
				targetFrame.origin.z = base.Mission.Scene.GetWaterLevelAtPosition(new Vec2(targetFrame.origin.x, targetFrame.origin.y), true, false);
				targetFrame.origin.z = targetFrame.origin.z - ship.Physics.StabilitySubmergedHeightOfShip;
			}
			ship.GameEntity.SetGlobalFrame(ref targetFrame, true);
			ship.GameEntity.UpdateAttachedNavigationMeshFaces();
			ship.ResetFormationPositioning();
			if (anchorShip)
			{
				ship.SetAnchor(true, true, 1f);
			}
			if (ship.ShipOrder != null)
			{
				ship.ShipOrder.RefreshOrders();
			}
		}

		// Token: 0x0600108B RID: 4235 RVA: 0x0007C098 File Offset: 0x0007A298
		internal void HandleSailsHit(MissionShip ship, MissionSail sail, Agent attackerAgent, Mission.Missile missile, MissionWeapon missileWeapon)
		{
			float num = (float)missileWeapon.CurrentUsageItem.FireDamage;
			float missileVelocityLengthOnFirstSailHit = this.GetMissileVelocityLengthOnFirstSailHit(missile.Index);
			if (missileVelocityLengthOnFirstSailHit < 0f)
			{
				this.AddMissileHittingSail(missile);
			}
			else
			{
				float length = missile.GetVelocity().Length;
				num *= length / missileVelocityLengthOnFirstSailHit;
			}
			float num2 = MissionGameModels.Current.AgentApplyDamageModel.CalculateSailFireDamage(attackerAgent, ship.ShipOrigin, num, this.IsMissileFromShipSiegeEngine(missile.Index));
			ship.DealDamageToSails(attackerAgent, num, num2, sail);
		}

		// Token: 0x0600108C RID: 4236 RVA: 0x0007C11A File Offset: 0x0007A31A
		public static bool IsPositionInsideBoundaries(in Vec2 position, NavalShipsLogic.NavalBoundaryCheckType boundaryType = NavalShipsLogic.NavalBoundaryCheckType.HardBoundary, Team team = null)
		{
			if (boundaryType == NavalShipsLogic.NavalBoundaryCheckType.HardBoundary)
			{
				return Mission.Current.IsPositionInsideHardBoundaries(position);
			}
			return boundaryType == NavalShipsLogic.NavalBoundaryCheckType.DeploymentBoundary && Mission.Current.DeploymentPlan.IsPositionInsideDeploymentBoundaries(team, ref position);
		}

		// Token: 0x0600108D RID: 4237 RVA: 0x0007C148 File Offset: 0x0007A348
		private static bool FindAndRemoveClosestDeckFrameToPosition(MBList<MatrixFrame> deckFrames, in Vec3 position, out MatrixFrame foundFrame)
		{
			int num = -1;
			float num2 = float.MaxValue;
			for (int i = 0; i < deckFrames.Count; i++)
			{
				float lengthSquared = (deckFrames[i].origin - position).LengthSquared;
				if (lengthSquared <= num2)
				{
					num = i;
					num2 = lengthSquared;
				}
			}
			if (num >= 0)
			{
				foundFrame = deckFrames[num];
				Vec3 vec = 1f * Vec3.Up;
				foundFrame.origin += vec;
				deckFrames.RemoveAt(num);
				return true;
			}
			foundFrame = MatrixFrame.Identity;
			return false;
		}

		// Token: 0x040009B1 RID: 2481
		public const int MaxTeamShipDeploymentLimit = 8;

		// Token: 0x040009B7 RID: 2487
		private readonly Dictionary<int, Mission.Missile> _shipSiegeEngineMissileDictionary;

		// Token: 0x040009B8 RID: 2488
		private readonly Dictionary<int, float> _missileHittingSailDictionary;

		// Token: 0x040009B9 RID: 2489
		private readonly ShipAssignment[,] _shipAssignments;

		// Token: 0x040009BA RID: 2490
		private readonly MBList<MissionShip> _allShips;

		// Token: 0x040009BB RID: 2491
		private readonly NavalShipDeploymentLimit[] _teamDeploymentLimits;

		// Token: 0x040009BC RID: 2492
		private readonly MBList<MissionShip> _removedShipsPool;

		// Token: 0x040009BD RID: 2493
		private int _shipIndexGenerator;

		// Token: 0x040009BE RID: 2494
		[TupleElementNames(new string[] { "distance", "index" })]
		private readonly PriorityQueue<int, ValueTuple<int, Vec2i>> _tmpCollisionFreeFrameSearchQueue;

		// Token: 0x02000254 RID: 596
		public enum NavalBoundaryCheckType
		{
			// Token: 0x04001061 RID: 4193
			HardBoundary,
			// Token: 0x04001062 RID: 4194
			DeploymentBoundary
		}

		// Token: 0x02000255 RID: 597
		// (Invoke) Token: 0x06001BB6 RID: 7094
		public delegate bool ShipFilter(ShipAssignment assignment);
	}
}
