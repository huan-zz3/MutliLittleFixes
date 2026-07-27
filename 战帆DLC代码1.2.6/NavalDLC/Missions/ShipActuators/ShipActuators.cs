using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.NavalPhysics;
using NavalDLC.Missions.Objects;
using NavalDLC.Missions.Objects.UsableMachines;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.ShipActuators
{
	// Token: 0x02000097 RID: 151
	public class ShipActuators
	{
		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x06000AB0 RID: 2736 RVA: 0x0004A88D File Offset: 0x00048A8D
		// (set) Token: 0x06000AB1 RID: 2737 RVA: 0x0004A895 File Offset: 0x00048A95
		public int VisualRudderPullDirection { get; private set; }

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x06000AB2 RID: 2738 RVA: 0x0004A89E File Offset: 0x00048A9E
		// (set) Token: 0x06000AB3 RID: 2739 RVA: 0x0004A8A6 File Offset: 0x00048AA6
		public float VisualRudderLocalRotation { get; private set; }

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x06000AB4 RID: 2740 RVA: 0x0004A8AF File Offset: 0x00048AAF
		public MBReadOnlyList<MissionSail> Sails
		{
			get
			{
				return this._sails;
			}
		}

		// Token: 0x06000AB5 RID: 2741 RVA: 0x0004A8B8 File Offset: 0x00048AB8
		public ShipActuators(MissionShip ownerShip)
		{
			this._ownerMissionShip = ownerShip;
			this._cachedOwnerScene = ownerShip.GameEntity.Scene;
			this._navalShipsLogic = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			this.OnShipObjectUpdated();
			this._rowersPhase = 3.1415927f;
			this._evenCycle = true;
			this._nearbyShips = new MBList<ValueTuple<MissionShip, OarSidePhaseController.OarSide>>();
			this._timeLeftToUpdateNearbyShips = 0f;
		}

		// Token: 0x06000AB6 RID: 2742 RVA: 0x0004A972 File Offset: 0x00048B72
		public void OnShipObjectUpdated()
		{
			this.LoadRudder();
			this.LoadOars();
			this.LoadSails();
		}

		// Token: 0x06000AB7 RID: 2743 RVA: 0x0004A988 File Offset: 0x00048B88
		public ShipForceRecord OnParallelFixedTick(float fixedDt, in ShipActuatorRecord actuatorInput)
		{
			MatrixFrame bodyWorldTransform = this._ownerMissionShip.GameEntity.GetBodyWorldTransform();
			TWSharedMutexReadLock twsharedMutexReadLock;
			twsharedMutexReadLock..ctor(Scene.PhysicsAndRayCastLock);
			Vec3 linearVelocityMT;
			Vec3 angularVelocityMT;
			try
			{
				linearVelocityMT = GameEntityPhysicsExtensions.GetLinearVelocityMT(this._ownerMissionShip.GameEntity);
				angularVelocityMT = GameEntityPhysicsExtensions.GetAngularVelocityMT(this._ownerMissionShip.GameEntity);
			}
			finally
			{
				twsharedMutexReadLock.Dispose();
			}
			float num = Vec3.DotProduct(linearVelocityMT, bodyWorldTransform.rotation.f);
			this.FixedUpdateRowers(fixedDt, in actuatorInput, in bodyWorldTransform, num);
			if (this._sails.Count > 0)
			{
				this.FixedUpdateSails(fixedDt, in actuatorInput, in linearVelocityMT, in angularVelocityMT);
			}
			this.FixedUpdateRudder(fixedDt, in actuatorInput, in bodyWorldTransform, num);
			MBReadOnlyList<ShipForce> leftOarForces = this._leftOarForces;
			MBReadOnlyList<ShipForce> rightOarForces = this._rightOarForces;
			MBReadOnlyList<ShipForce> sailForces = this._sailForces;
			return new ShipForceRecord(leftOarForces, rightOarForces, in sailForces, in this._rudderShipForce);
		}

		// Token: 0x06000AB8 RID: 2744 RVA: 0x0004AA5C File Offset: 0x00048C5C
		public void OnTickParallel(float dt)
		{
			this.OnParallelTickRowers(dt);
			this.OnParallelTickRudder(dt);
		}

		// Token: 0x06000AB9 RID: 2745 RVA: 0x0004AA6C File Offset: 0x00048C6C
		private void CalculateOarSoundPositionsAndParams()
		{
			if (this._ownerMissionShip.ShouldUpdateSoundPos)
			{
				if (this._ownerMissionShip.Physics.LastSubmergedHeightFactorForActuators > 0.01f)
				{
					MatrixFrame bodyWorldTransform = this._ownerMissionShip.GameEntity.GetBodyWorldTransform();
					for (int i = 0; i < 2; i++)
					{
						if (this._rowingSoundEventData[i].NumberOfActiveOars > 0)
						{
							MBList<ShipForce> mblist = ((i == 0) ? this._leftOarForces : this._rightOarForces);
							float num = ((i == 0) ? this._leftOarsPhaseController.VisualPhase : this._rightOarsPhaseController.VisualPhase);
							ShipForce shipForce = mblist[this._rowingSoundEventData[i].ClosestOarIndex];
							Vec3 vec = bodyWorldTransform.TransformToParent(ref shipForce.LocalPosition);
							shipForce = mblist[this._rowingSoundEventData[i].FurthestOarIndex];
							Vec3 vec2 = bodyWorldTransform.TransformToParent(ref shipForce.LocalPosition);
							this._rowingSoundEventData[i].RowingSoundEventPositions = this.CalculateRowingSoundPosition(in vec, in vec2);
							if (MBMath.IsBetweenInclusive(num, -1.3962634f, 1.3962634f))
							{
								if (!this._rowingSoundEventData[i].IsOarsInWater)
								{
									this._rowingSoundEventData[i].SoundEventRowingPowerParam = this.CalculateOarRowingPowerSoundParameter((OarSidePhaseController.OarSide)i, in this._rowingSoundEventData[i].RowingSoundEventPositions);
									if (this._rowingSoundEventData[i].SoundEventRowingPowerParam > 0f)
									{
										this._rowingSoundEventData[i].ShouldTriggerOarSound = true;
										this._rowingSoundEventData[i].IsOarsInWater = true;
									}
								}
							}
							else
							{
								this._rowingSoundEventData[i].IsOarsInWater = false;
							}
						}
					}
					return;
				}
				this._rowingSoundEventData[0].IsOarsInWater = false;
				this._rowingSoundEventData[0].ShouldTriggerOarSound = false;
				this._rowingSoundEventData[1].IsOarsInWater = false;
				this._rowingSoundEventData[1].ShouldTriggerOarSound = false;
			}
		}

		// Token: 0x06000ABA RID: 2746 RVA: 0x0004AC60 File Offset: 0x00048E60
		internal void Update(float dt)
		{
			for (int i = 0; i < this._sails.Count; i++)
			{
				this._sails[i].Update(dt);
			}
			this.UpdateSoundEventPositions();
		}

		// Token: 0x06000ABB RID: 2747 RVA: 0x0004AC9C File Offset: 0x00048E9C
		private void FixedUpdateSails(float fixedDt, in ShipActuatorRecord actuatorInput, in Vec3 shipLinearVelocityGlobal, in Vec3 shipAngularVelocityGlobal)
		{
			for (int i = 0; i < this._sails.Count; i++)
			{
				MissionSail missionSail = this._sails[i];
				missionSail.FixedUpdate(fixedDt, in actuatorInput, in shipLinearVelocityGlobal, in shipAngularVelocityGlobal);
				this._sailForces[i] = missionSail.Force;
			}
		}

		// Token: 0x06000ABC RID: 2748 RVA: 0x0004ACEC File Offset: 0x00048EEC
		private void UpdateSoundEventPositions()
		{
			if (this._ownerMissionShip.ShouldUpdateSoundPos)
			{
				if (this._rudderSoundEvent == null)
				{
					this._rudderSoundEvent = SoundEvent.CreateEvent(ShipActuators._rudderSoundEventId, this._cachedOwnerScene);
					this._shipPresenceSoundEvent = SoundEvent.CreateEvent(ShipActuators._shipPresenceSoundEventId, this._cachedOwnerScene);
					this._rudderSoundEvent.Play();
					this._shipPresenceSoundEvent.Play();
				}
				for (int i = 0; i < 2; i++)
				{
					if (this._rowingSoundEventData[i].ShouldTriggerOarSound)
					{
						SoundEvent oarsSoundEvents = this._rowingSoundEventData[i].OarsSoundEvents;
						if (oarsSoundEvents != null)
						{
							oarsSoundEvents.Stop();
						}
						this._rowingSoundEventData[i].OarsSoundEvents = SoundEvent.CreateEvent(ShipActuators._rowingSoundEventIds[i], this._cachedOwnerScene);
						this._rowingSoundEventData[i].OarsSoundEvents.SetParameter("RowingPower", this._rowingSoundEventData[i].SoundEventRowingPowerParam);
						this._rowingSoundEventData[i].OarsSoundEvents.SetParameter("OarsmanLevel", (float)this._rowingSoundEventData[i].NumberOfActiveOars);
						this._rowingSoundEventData[i].OarsSoundEvents.SetPosition(this._rowingSoundEventData[i].RowingSoundEventPositions);
						this._rowingSoundEventData[i].OarsSoundEvents.Play();
						this._rowingSoundEventData[i].ShouldTriggerOarSound = false;
					}
					else
					{
						SoundEvent oarsSoundEvents2 = this._rowingSoundEventData[i].OarsSoundEvents;
						if (oarsSoundEvents2 != null)
						{
							oarsSoundEvents2.SetPosition(this._rowingSoundEventData[i].RowingSoundEventPositions);
						}
					}
				}
				MatrixFrame globalFrame = this._ownerMissionShip.GlobalFrame;
				Vec3 centerOfMass = this._ownerMissionShip.GameEntity.CenterOfMass;
				Vec3 vec = globalFrame.TransformToParent(ref centerOfMass);
				vec.z += 3f;
				this._shipPresenceSoundEvent.SetPosition(vec);
				this._shipPresenceSoundEvent.SetParameter("ForceContinuous", this._shipPresenceSoundParam);
				this._rudderSoundEvent.SetPosition(globalFrame.TransformToParent(ref this._rudderShipForce.LocalPosition));
				this._rudderSoundEvent.SetParameter("RudderStress", this._rudderStressSoundParam);
				return;
			}
			SoundEvent oarsSoundEvents3 = this._rowingSoundEventData[0].OarsSoundEvents;
			if (oarsSoundEvents3 != null)
			{
				oarsSoundEvents3.Stop();
			}
			SoundEvent oarsSoundEvents4 = this._rowingSoundEventData[1].OarsSoundEvents;
			if (oarsSoundEvents4 != null)
			{
				oarsSoundEvents4.Stop();
			}
			SoundEvent rudderSoundEvent = this._rudderSoundEvent;
			if (rudderSoundEvent != null)
			{
				rudderSoundEvent.Stop();
			}
			SoundEvent shipPresenceSoundEvent = this._shipPresenceSoundEvent;
			if (shipPresenceSoundEvent != null)
			{
				shipPresenceSoundEvent.Stop();
			}
			this._rowingSoundEventData[0].OarsSoundEvents = null;
			this._rowingSoundEventData[1].OarsSoundEvents = null;
			this._rudderSoundEvent = null;
			this._shipPresenceSoundEvent = null;
		}

		// Token: 0x06000ABD RID: 2749 RVA: 0x0004AFB4 File Offset: 0x000491B4
		private Vec3 CalculateRowingSoundPosition(in Vec3 closestOarGlobalPos, in Vec3 furthestOarGlobalPos)
		{
			Vec3 origin = SoundManager.GetListenerFrame().origin;
			Vec3 vec = furthestOarGlobalPos - closestOarGlobalPos;
			float num = Vec3.DotProduct(origin - closestOarGlobalPos, vec) / vec.LengthSquared;
			return closestOarGlobalPos + MathF.Clamp(num, 0f, 1f) * vec;
		}

		// Token: 0x06000ABE RID: 2750 RVA: 0x0004B018 File Offset: 0x00049218
		private float CalculateOarRowingPowerSoundParameter(OarSidePhaseController.OarSide oarSide, in Vec3 soundPos)
		{
			MBList<ShipForce> mblist = null;
			MBList<ValueTuple<GameEntity, MissionOar>> mblist2 = null;
			if (oarSide != OarSidePhaseController.OarSide.Left)
			{
				if (oarSide == OarSidePhaseController.OarSide.Right)
				{
					mblist = this._rightOarForces;
					mblist2 = this._rightSideOars;
				}
			}
			else
			{
				mblist = this._leftOarForces;
				mblist2 = this._leftSideOars;
			}
			MatrixFrame bodyWorldTransform = this._ownerMissionShip.GameEntity.GetBodyWorldTransform();
			float num = 0f;
			float num2 = 0f;
			float num3 = -1f;
			for (int i = 0; i < mblist.Count; i++)
			{
				float num4 = (mblist2[i].Item2.IsExtracted ? 5000f : 0f);
				Vec3 vec = soundPos;
				float num5 = vec.Distance(bodyWorldTransform.TransformToParent(ref mblist[i].LocalPosition));
				if (num5 < 0.010000001f && num4 > 0f)
				{
					num3 = num4;
					break;
				}
				if (num5 > 0.010000001f)
				{
					float num6 = 1f / num5;
					num += num6 * num4;
					num2 += num6;
				}
			}
			if (num3 == -1f && num2 != 0f)
			{
				num3 = num / num2;
			}
			return MathF.Min(num3 * 0.1f, 500f);
		}

		// Token: 0x06000ABF RID: 2751 RVA: 0x0004B144 File Offset: 0x00049344
		private void LoadOars()
		{
			MBList<ShipOarDeck> mblist = MBExtensions.CollectScriptComponentsIncludingChildrenRecursive<ShipOarDeck>(this._ownerMissionShip.GameEntity);
			this._leftOarsPhaseController = new OarSidePhaseController(this._ownerMissionShip, OarSidePhaseController.OarSide.Left);
			this._leftSideOars.Clear();
			this._leftOarForces.Clear();
			this._rightOarsPhaseController = new OarSidePhaseController(this._ownerMissionShip, OarSidePhaseController.OarSide.Right);
			this._rightSideOars.Clear();
			this._rightOarForces.Clear();
			this._maxOarLength = 0f;
			for (int i = 0; i < mblist.Count; i++)
			{
				ShipOarDeck shipOarDeck = mblist[i];
				OarDeckParameters parameters = shipOarDeck.GetParameters();
				this._maxOarLength = MathF.Max(this._maxOarLength, parameters.OarLength);
				List<WeakGameEntity> list = shipOarDeck.GameEntity.CollectChildrenEntitiesWithTag("oar_gate_left");
				List<WeakGameEntity> list2 = shipOarDeck.GameEntity.CollectChildrenEntitiesWithTag("oar_gate_right");
				foreach (WeakGameEntity weakGameEntity in list)
				{
					GameEntity gameEntity = GameEntity.CreateFromWeakEntity(weakGameEntity);
					MissionOar missionOar = MissionOar.CreateShipOar(this._ownerMissionShip, gameEntity, parameters, this._leftOarsPhaseController);
					this.GetOarScriptFromEntity(weakGameEntity).InitializeOar(missionOar);
					this._leftSideOars.Add(new ValueTuple<GameEntity, MissionOar>(gameEntity, missionOar));
					this._leftOarForces.Add(ShipForce.None(ShipForce.SourceType.Oar));
				}
				foreach (WeakGameEntity weakGameEntity2 in list2)
				{
					GameEntity gameEntity2 = GameEntity.CreateFromWeakEntity(weakGameEntity2);
					MissionOar missionOar2 = MissionOar.CreateShipOar(this._ownerMissionShip, gameEntity2, parameters, this._rightOarsPhaseController);
					this.GetOarScriptFromEntity(weakGameEntity2).InitializeOar(missionOar2);
					this._rightSideOars.Add(new ValueTuple<GameEntity, MissionOar>(gameEntity2, missionOar2));
					this._rightOarForces.Add(ShipForce.None(ShipForce.SourceType.Oar));
				}
			}
			OarDeckParameters oarDeckParameters;
			OarDeckParameters oarDeckParameters2;
			ShipActuators.GenerateAverageSideDeckParameters(out oarDeckParameters, out oarDeckParameters2, this._leftSideOars, this._rightSideOars);
			this._leftOarsPhaseController.SetAverageOarDeckParameters(oarDeckParameters);
			this._rightOarsPhaseController.SetAverageOarDeckParameters(oarDeckParameters2);
			this._rowingSoundEventData[0].ClosestOarIndex = 0;
			this._rowingSoundEventData[1].ClosestOarIndex = 0;
			this._rowingSoundEventData[0].FurthestOarIndex = 0;
			this._rowingSoundEventData[1].FurthestOarIndex = 0;
			this._leftSideAverageOarLocalPos = Vec3.Zero;
			this._rightSideAverageOarLocalPos = Vec3.Zero;
			for (int j = 0; j < this._leftSideOars.Count; j++)
			{
				Vec3 bladeContact = this._leftSideOars[j].Item2.BladeContact;
				if (bladeContact.DistanceSquared(this._rudderStockLocalPosition) > this._leftSideOars[this._rowingSoundEventData[0].FurthestOarIndex].Item2.BladeContact.DistanceSquared(this._rudderStockLocalPosition))
				{
					this._rowingSoundEventData[0].FurthestOarIndex = j;
				}
				this._leftSideAverageOarLocalPos += bladeContact;
			}
			this._leftSideAverageOarLocalPos /= (float)this._leftSideOars.Count;
			for (int k = 0; k < this._leftSideOars.Count; k++)
			{
				Vec3 bladeContact2 = this._leftSideOars[k].Item2.BladeContact;
				Vec3 bladeContact3 = this._leftSideOars[this._rowingSoundEventData[0].FurthestOarIndex].Item2.BladeContact;
				if (bladeContact2.DistanceSquared(bladeContact3) > this._leftSideOars[this._rowingSoundEventData[0].ClosestOarIndex].Item2.BladeContact.DistanceSquared(bladeContact3))
				{
					this._rowingSoundEventData[0].ClosestOarIndex = k;
				}
			}
			for (int l = 0; l < this._rightSideOars.Count; l++)
			{
				Vec3 bladeContact4 = this._rightSideOars[l].Item2.BladeContact;
				if (bladeContact4.DistanceSquared(this._rudderStockLocalPosition) > this._rightSideOars[this._rowingSoundEventData[1].FurthestOarIndex].Item2.BladeContact.DistanceSquared(this._rudderStockLocalPosition))
				{
					this._rowingSoundEventData[1].FurthestOarIndex = l;
				}
				this._rightSideAverageOarLocalPos += bladeContact4;
			}
			this._rightSideAverageOarLocalPos /= (float)this._rightSideOars.Count;
			for (int m = 0; m < this._rightSideOars.Count; m++)
			{
				Vec3 bladeContact5 = this._rightSideOars[m].Item2.BladeContact;
				Vec3 bladeContact6 = this._rightSideOars[this._rowingSoundEventData[1].FurthestOarIndex].Item2.BladeContact;
				if (bladeContact5.DistanceSquared(bladeContact6) > this._rightSideOars[this._rowingSoundEventData[1].ClosestOarIndex].Item2.BladeContact.DistanceSquared(bladeContact6))
				{
					this._rowingSoundEventData[1].ClosestOarIndex = m;
				}
			}
			float num = 1f;
			float num2 = 1f;
			if (this._ownerMissionShip.ShipOrigin != null)
			{
				num = 1f + this._ownerMissionShip.ShipOrigin.MaxOarForceFactor;
				num2 = 1f + this._ownerMissionShip.ShipOrigin.MaxOarPowerFactor;
			}
			this._oarsmenForceMultiplier = this._ownerMissionShip.MissionShipObject.OarsmenForceMultiplier * num;
			this._oarsmenSpeedMultiplier = num2;
			this._oarFrictionMultiplier = this._ownerMissionShip.MissionShipObject.OarFrictionMultiplier;
			Vec3 vec = MissionOar.ComputeBladeContactVelocityAux(oarDeckParameters, 0f, 6.2831855f, 1f);
			this._oarsTipSpeedReferenceMultiplier = MathF.Abs(this._ownerMissionShip.MissionShipObject.OarsTipSpeed / vec.y);
			this._oarAppliedForceMultiplierForStoryMission = 1f;
		}

		// Token: 0x06000AC0 RID: 2752 RVA: 0x0004B778 File Offset: 0x00049978
		public void OnShipRemoved(MissionShip ship)
		{
			this._nearbyShips.Clear();
			this._timeLeftToUpdateNearbyShips = 0f;
		}

		// Token: 0x06000AC1 RID: 2753 RVA: 0x0004B790 File Offset: 0x00049990
		private static OarDeckParameters GenerateAverageSideDeckParametersAux([TupleElementNames(new string[] { "entity", "oar" })] MBList<ValueTuple<GameEntity, MissionOar>> sideOars)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			float num7 = 0f;
			foreach (ValueTuple<GameEntity, MissionOar> valueTuple in sideOars)
			{
				OarDeckParameters deckParameters = valueTuple.Item2.DeckParameters;
				num += deckParameters.VerticalBaseAngle;
				num2 += deckParameters.LateralBaseAngle;
				num3 += deckParameters.VerticalRotationAngle;
				num4 += deckParameters.LateralRotationAngle;
				num5 += deckParameters.OarLength;
				num6 += deckParameters.RetractionRate;
				num7 += deckParameters.RetractionOffset;
			}
			float num8 = 1f / (float)sideOars.Count;
			num *= num8;
			num2 *= num8;
			num3 *= num8;
			num4 *= num8;
			num5 *= num8;
			num6 *= num8;
			num7 *= num8;
			return new OarDeckParameters(num, num2, num3, num4, num5, num6, num7);
		}

		// Token: 0x06000AC2 RID: 2754 RVA: 0x0004B8A4 File Offset: 0x00049AA4
		private static void GenerateAverageSideDeckParameters(out OarDeckParameters leftSideAverageDeckParameters, out OarDeckParameters rightSideAverageDeckParameters, [TupleElementNames(new string[] { "entity", "oar" })] MBList<ValueTuple<GameEntity, MissionOar>> leftSideOars, [TupleElementNames(new string[] { "entity", "oar" })] MBList<ValueTuple<GameEntity, MissionOar>> rightSideOars)
		{
			leftSideAverageDeckParameters = ShipActuators.GenerateAverageSideDeckParametersAux(leftSideOars);
			rightSideAverageDeckParameters = ShipActuators.GenerateAverageSideDeckParametersAux(rightSideOars);
		}

		// Token: 0x06000AC3 RID: 2755 RVA: 0x0004B8B8 File Offset: 0x00049AB8
		private void LoadSails()
		{
			this._sails.Clear();
			this._sailForces.Clear();
			WeakGameEntity gameEntity = this._ownerMissionShip.GameEntity;
			for (int i = 0; i < this._ownerMissionShip.MissionShipObject.Sails.Count; i++)
			{
				ShipSail shipSail = this._ownerMissionShip.MissionShipObject.Sails[i];
				string text = "sail_center_" + i;
				List<WeakGameEntity> list = gameEntity.CollectChildrenEntitiesWithTag(text);
				if (list.Count > 0)
				{
					SailVisual firstScriptOfType = list[0].GetFirstScriptOfType<SailVisual>();
					firstScriptOfType.SoundsEnabled = true;
					list[0].CreateAndAddScriptComponent("MissionSail", true);
					MissionSail firstScriptOfType2 = list[0].GetFirstScriptOfType<MissionSail>();
					firstScriptOfType2.InitWithVariables(shipSail, this._ownerMissionShip, firstScriptOfType);
					this._sails.Add(firstScriptOfType2);
					this._sailForces.Add(ShipForce.None());
				}
				else
				{
					Debug.FailedAssert("Unable to find a sail entity on ship prefab (" + gameEntity.GetPrefabName() + ") with tag: " + text, "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Missions\\ShipActuators\\ShipActuators.cs", "LoadSails", 643);
				}
			}
		}

		// Token: 0x06000AC4 RID: 2756 RVA: 0x0004B9E8 File Offset: 0x00049BE8
		private void LoadRudder()
		{
			WeakGameEntity gameEntity = this._ownerMissionShip.GameEntity;
			if (this._ownerMissionShip.MissionShipObject.HasValidRudderStockPosition)
			{
				this._rudderStockLocalPosition = this._ownerMissionShip.MissionShipObject.RudderStockPosition;
				return;
			}
			List<WeakGameEntity> list = gameEntity.CollectChildrenEntitiesWithTag("rudder_stock");
			if (list.Count > 0)
			{
				this._rudderStockLocalPosition = list[0].GetFrame().origin;
				return;
			}
			Debug.FailedAssert("Stock position is not defined for ship: " + gameEntity.Name, "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\Missions\\ShipActuators\\ShipActuators.cs", "LoadRudder", 665);
			this._rudderStockLocalPosition = Vec3.Zero;
		}

		// Token: 0x06000AC5 RID: 2757 RVA: 0x0004BA8C File Offset: 0x00049C8C
		private void OnParallelTickRowers(float dt)
		{
			this._leftOarsPhaseController.OnParallelTick(dt);
			this._rightOarsPhaseController.OnParallelTick(dt);
			for (int i = 0; i < this._leftSideOars.Count; i++)
			{
				this._leftSideOars[i].Item2.OnParallelTick(dt);
			}
			for (int j = 0; j < this._rightSideOars.Count; j++)
			{
				this._rightSideOars[j].Item2.OnParallelTick(dt);
			}
		}

		// Token: 0x06000AC6 RID: 2758 RVA: 0x0004BB0C File Offset: 0x00049D0C
		public static void BlendPhaseTo(ref ShipActuators.OarPhaseData phaseData, float targetPhase, float alphaInRadOverSeconds, float maxAlphaInRadOverSeconds, float fixedDt, bool toFullStop, bool isPartialStop)
		{
			targetPhase = MBMath.WrapAngleSafe(targetPhase);
			float num = MathF.Abs(MBMath.GetSmallestDifferenceBetweenTwoAngles(phaseData.CurPhase, targetPhase));
			if (phaseData.LockedToTargetPhase && num > alphaInRadOverSeconds * fixedDt * 2f)
			{
				phaseData.LockedToTargetPhase = false;
			}
			bool flag = false;
			if (!phaseData.LockedToTargetPhase)
			{
				if (toFullStop)
				{
					alphaInRadOverSeconds = maxAlphaInRadOverSeconds * 1.4f;
				}
				else if (isPartialStop)
				{
					alphaInRadOverSeconds = maxAlphaInRadOverSeconds * 1.4f;
				}
				else
				{
					alphaInRadOverSeconds = maxAlphaInRadOverSeconds * 1.3f;
					flag = true;
				}
			}
			if (!phaseData.LockedToTargetPhase)
			{
				float smallestDifferenceBetweenTwoAngles = MBMath.GetSmallestDifferenceBetweenTwoAngles(MBMath.WrapAngleSafe(phaseData.CurPhase + alphaInRadOverSeconds * fixedDt), targetPhase);
				float smallestDifferenceBetweenTwoAngles2 = MBMath.GetSmallestDifferenceBetweenTwoAngles(MBMath.WrapAngleSafe(phaseData.CurPhase - alphaInRadOverSeconds * fixedDt), targetPhase);
				float smallestDifferenceBetweenTwoAngles3 = MBMath.GetSmallestDifferenceBetweenTwoAngles(phaseData.CurPhase, targetPhase);
				float num2 = (flag ? 0.005f : 0.3f);
				float num3 = MathF.Abs(smallestDifferenceBetweenTwoAngles3) / alphaInRadOverSeconds;
				float num4 = ((toFullStop || isPartialStop) ? 0.03f : 0.1f);
				float num5;
				if ((MathF.Abs(smallestDifferenceBetweenTwoAngles3) > 1.5707964f) ? (MathF.Sign(phaseData.LastNonZeroRevolutionRate) >= 0) : (MathF.Abs(smallestDifferenceBetweenTwoAngles) < MathF.Abs(smallestDifferenceBetweenTwoAngles2)))
				{
					if (num3 > num2)
					{
						num5 = alphaInRadOverSeconds;
					}
					else
					{
						num5 = alphaInRadOverSeconds * MathF.Max(num3 / num2, num4);
					}
				}
				else if (num3 > num2)
				{
					num5 = -alphaInRadOverSeconds;
				}
				else
				{
					num5 = -alphaInRadOverSeconds * MathF.Max(num3 / num2, num4);
				}
				if (MathF.Abs(smallestDifferenceBetweenTwoAngles3 / num5) <= 0f)
				{
					phaseData.LockedToTargetPhase = true;
				}
				float smallestDifferenceBetweenTwoAngles4 = MBMath.GetSmallestDifferenceBetweenTwoAngles(MBMath.WrapAngleSafe(phaseData.CurPhase + num5 * fixedDt), targetPhase);
				float smallestDifferenceBetweenTwoAngles5 = MBMath.GetSmallestDifferenceBetweenTwoAngles(MBMath.WrapAngleSafe(phaseData.CurPhase - num5 * fixedDt), targetPhase);
				if (smallestDifferenceBetweenTwoAngles4 * smallestDifferenceBetweenTwoAngles5 <= 0f && MathF.Abs(smallestDifferenceBetweenTwoAngles3) <= MathF.Abs(num5 * fixedDt))
				{
					phaseData.LockedToTargetPhase = true;
				}
				phaseData.CurPhase += num5 * fixedDt;
				phaseData.CurPhase = MBMath.WrapAngleSafe(phaseData.CurPhase);
			}
			if (phaseData.LockedToTargetPhase)
			{
				phaseData.CurPhase = targetPhase;
			}
			phaseData.CurPhase = MBMath.WrapAngleSafe(phaseData.CurPhase);
			float num6 = 1f;
			float num7 = 0.9599311f;
			if (!phaseData.LockedToTargetPhase && toFullStop && phaseData.CurPhase < num7 && phaseData.CurPhase > -num7)
			{
				num6 = 0f;
			}
			phaseData.CycleArcSizeMult = MathF.Lerp(phaseData.CycleArcSizeMult, num6, fixedDt * 1.2f, 1E-05f);
		}

		// Token: 0x06000AC7 RID: 2759 RVA: 0x0004BD68 File Offset: 0x00049F68
		private static float GetRowSpeedAccordingToPhase(float phase, bool forwards, bool partialTurn, bool onPointTurn)
		{
			ShipActuators.OarAnimKeyFrame[] array;
			if (onPointTurn)
			{
				array = ShipActuators.OarRowSpeedAnimationManager.OnPointTurnPhaseSpeedAnim;
				forwards = true;
			}
			else
			{
				array = (partialTurn ? ShipActuators.OarRowSpeedAnimationManager.PartialPhaseSpeedAnim : ShipActuators.OarRowSpeedAnimationManager.ForwardPhaseSpeedAnim);
			}
			float num = ((forwards ? phase : MBMath.WrapAngleSafe(6.2831855f - phase)) + 3.1415927f) / 6.2831855f;
			if (num >= 1f)
			{
				num -= 1f;
			}
			float num2 = 1f;
			if (forwards)
			{
				for (int i = 0; i < array.Length - 1; i++)
				{
					ShipActuators.OarAnimKeyFrame oarAnimKeyFrame = array[i];
					ShipActuators.OarAnimKeyFrame oarAnimKeyFrame2 = array[i + 1];
					if (oarAnimKeyFrame.KeyProgress <= num && num < oarAnimKeyFrame2.KeyProgress)
					{
						float num3 = oarAnimKeyFrame2.KeyProgress - oarAnimKeyFrame.KeyProgress;
						float num4 = (num - oarAnimKeyFrame.KeyProgress) / num3;
						num2 = MathF.Lerp(oarAnimKeyFrame.Speed, oarAnimKeyFrame2.Speed, num4, 1E-05f);
						break;
					}
				}
			}
			else
			{
				for (int j = array.Length - 1; j >= 1; j--)
				{
					ShipActuators.OarAnimKeyFrame oarAnimKeyFrame3 = array[j];
					ShipActuators.OarAnimKeyFrame oarAnimKeyFrame4 = array[j - 1];
					if (oarAnimKeyFrame4.KeyProgress <= num && num < oarAnimKeyFrame3.KeyProgress)
					{
						float num5 = oarAnimKeyFrame3.KeyProgress - oarAnimKeyFrame4.KeyProgress;
						float num6 = (num - oarAnimKeyFrame4.KeyProgress) / num5;
						num2 = MathF.Lerp(oarAnimKeyFrame4.Speed, oarAnimKeyFrame3.Speed, num6, 1E-05f);
						break;
					}
				}
			}
			return num2;
		}

		// Token: 0x06000AC8 RID: 2760 RVA: 0x0004BEC0 File Offset: 0x0004A0C0
		private void FixedUpdateRowers(float fixedDt, in ShipActuatorRecord actuatorInput, in MatrixFrame shipEntityGlobalFrame, float shipForwardSpeed)
		{
			if (this._ownerMissionShip.Physics.NavalSinkingState == NavalPhysics.SinkingState.Floating && !this._ownerMissionShip.BeingAbandoned)
			{
				this._timeLeftToUpdateNearbyShips -= fixedDt;
				if (this._timeLeftToUpdateNearbyShips < 0f)
				{
					this._timeLeftToUpdateNearbyShips = MBRandom.RandomFloatRanged(0.15f, 0.2f);
					BoundingBox boundingBox = this._ownerMissionShip.Physics.PhysicsBoundingBoxWithChildren;
					Vec2 vec = Vec2.Abs(boundingBox.max.AsVec2);
					boundingBox = this._ownerMissionShip.Physics.PhysicsBoundingBoxWithChildren;
					float num = Vec2.Max(vec, Vec2.Abs(boundingBox.min.AsVec2)).Length + this._maxOarLength;
					this._nearbyShips.Clear();
					NavalShipsLogic navalShipsLogic = this._navalShipsLogic;
					if (navalShipsLogic != null)
					{
						navalShipsLogic.FillClosestShips(in shipEntityGlobalFrame, num, this._nearbyShips, this._ownerMissionShip);
					}
				}
				int num2 = this._leftSideOars.Count + this._rightSideOars.Count;
				float num3 = (float)this.ComputeUsedOarCount() / (float)num2;
				num3 = num3 * 0.9f + 0.1f;
				float num4 = 1f;
				this.FixedUpdateSideOars(fixedDt, in shipEntityGlobalFrame, this._nearbyShips, this._leftSideOars, ref num4);
				this.FixedUpdateSideOars(fixedDt, in shipEntityGlobalFrame, this._nearbyShips, this._rightSideOars, ref num4);
				float num5;
				float num6;
				this.UpdateRowerParameters(actuatorInput.RowerThrust, actuatorInput.RowerRotation, shipForwardSpeed, out num5, out num6);
				float num7 = ((num5 >= 0f) ? this._rowersPhase : MBMath.WrapAngleSafe(6.2831855f - this._rowersPhase));
				float num8 = ((num6 >= 0f) ? this._rowersPhase : MBMath.WrapAngleSafe(6.2831855f - this._rowersPhase));
				if (num5 == 0f && num6 == 0f)
				{
					num7 = 3.1415927f;
					num8 = 3.1415927f;
				}
				else if (num5 == 0f)
				{
					num7 = 3.1415927f;
				}
				else if (num6 == 0f)
				{
					num8 = 3.1415927f;
				}
				if (num5 != 0f)
				{
					this._leftPhaseData.LastNonZeroRevolutionRate = num5;
				}
				if (num6 != 0f)
				{
					this._rightPhaseData.LastNonZeroRevolutionRate = num6;
				}
				float num9 = MathF.Abs(num6);
				float num10 = MathF.Abs(num5);
				if (num9 == 1f && num10 == 1f)
				{
					this._evenCycle = true;
				}
				bool flag = false;
				if (!this._evenCycle)
				{
					if (num9 < 1f && num9 > 0f)
					{
						num8 = 3.1415927f;
						flag = true;
					}
					else if (num10 < 1f && num10 > 0f)
					{
						num7 = 3.1415927f;
						flag = true;
					}
				}
				else if (num9 < 1f && num9 > 0f)
				{
					flag = true;
				}
				else if (num10 < 1f && num10 > 0f)
				{
					flag = true;
				}
				float num11 = MathF.Clamp(this._ownerMissionShip.Physics.LastSubmergedHeightFactorForActuators, 0f, 1.2f);
				bool flag2 = num5 * num6 < 0f;
				float num12 = ShipActuators.GetRowSpeedAccordingToPhase(num7, num5 >= 0f, flag, flag2);
				float num13 = ShipActuators.GetRowSpeedAccordingToPhase(num8, num6 >= 0f, flag, flag2);
				if (num9 < 1f && num9 > 0f)
				{
					num13 = float.MaxValue;
				}
				else if (num10 < 1f && num10 > 0f)
				{
					num12 = float.MaxValue;
				}
				float num14 = MathF.Min(num12, num13);
				float num15 = 6.2831855f * this._oarsTipSpeedReferenceMultiplier * this._oarsmenSpeedMultiplier;
				num15 *= num14;
				float num16;
				if (num5 == 0f && num6 == 0f)
				{
					num16 = 0f;
				}
				else
				{
					num16 = MathF.Lerp(this._lastFramePhaseRate, num15, 5f * fixedDt, 1E-05f);
				}
				ValueTuple<float, float> valueTuple = this.ComputeAverageOarTipPointForwardVelocities();
				float item = valueTuple.Item1;
				float item2 = valueTuple.Item2;
				float num17 = this._ownerMissionShip.MissionShipObject.OarsTipSpeed / MathF.Max(num11, 0.5f);
				ValueTuple<float, float> valueTuple2 = this._leftOarsPhaseController.ComputeForceAndSlowDownFactor(num5, item, num7, num16, this._oarsmenForceMultiplier * num4, this._oarFrictionMultiplier * num11, num17);
				float item3 = valueTuple2.Item1;
				float item4 = valueTuple2.Item2;
				ValueTuple<float, float> valueTuple3 = this._rightOarsPhaseController.ComputeForceAndSlowDownFactor(num6, item2, num8, num16, this._oarsmenForceMultiplier * num4, this._oarFrictionMultiplier * num11, num17);
				float item5 = valueTuple3.Item1;
				float item6 = valueTuple3.Item2;
				float num18 = MathF.Min(item4, item6);
				num16 *= num18;
				this._lastFramePhaseRate = num16;
				this._rowersPhase += num16 * fixedDt;
				if (this._rowersPhase >= 3.1415927f)
				{
					this._evenCycle = !this._evenCycle;
				}
				this._rowersPhase = MBMath.WrapAngleSafe(this._rowersPhase);
				float num19 = num16;
				float num20 = num16;
				if (num5 == 0f)
				{
					num19 = 0f;
				}
				else if (num6 == 0f)
				{
					num20 = 0f;
				}
				bool flag3 = false;
				bool flag4 = false;
				if (!this._evenCycle)
				{
					if (num9 < 1f && num9 > 0f)
					{
						num20 = 0f;
						num8 = 3.1415927f;
						flag4 = true;
					}
					else if (num10 < 1f && num10 > 0f)
					{
						num19 = 0f;
						num7 = 3.1415927f;
						flag3 = true;
					}
				}
				else
				{
					if (num6 < 1f && num6 > 0f && this._rowersPhase > 1.5707964f)
					{
						num8 = 3.1415927f;
						flag4 = true;
					}
					else if (num6 > -1f && num6 < 0f && this._rowersPhase > 1.5707964f)
					{
						num8 = 3.1415927f;
						flag4 = true;
					}
					if (num5 < 1f && num5 > 0f && this._rowersPhase > 1.5707964f)
					{
						num7 = 3.1415927f;
						flag3 = true;
					}
					else if (num5 > -1f && num5 < 0f && this._rowersPhase > 1.5707964f)
					{
						num7 = 3.1415927f;
						flag3 = true;
					}
				}
				bool flag5 = false;
				if (num5 == 0f && num6 == 0f)
				{
					flag5 = true;
					this._rowersPhase = 3.1415927f;
				}
				ShipActuators.BlendPhaseTo(ref this._leftPhaseData, num7, num19, num15, fixedDt, flag5, flag3);
				ShipActuators.BlendPhaseTo(ref this._rightPhaseData, num8, num20, num15, fixedDt, flag5, flag4);
				this._leftOarsPhaseController.SetPhaseData(this._leftPhaseData.CurPhase, num19, this._leftPhaseData.CycleArcSizeMult, num5);
				this._rightOarsPhaseController.SetPhaseData(this._rightPhaseData.CurPhase, num20, this._rightPhaseData.CycleArcSizeMult, num6);
				Vec3 f = shipEntityGlobalFrame.rotation.f;
				f.z = 0f;
				f.Normalize();
				this._rowingSoundEventData[0].NumberOfActiveOars = 0;
				this._rowingSoundEventData[1].NumberOfActiveOars = 0;
				for (int i = 0; i < this._leftSideOars.Count; i++)
				{
					Vec3 bladeContact = this._leftSideOars[i].Item2.BladeContact;
					Vec3 vec2 = num3 * item3 * this._oarAppliedForceMultiplierForStoryMission * num11 * f;
					this._leftOarForces[i] = new ShipForce(in bladeContact, in vec2, ShipForce.SourceType.Oar, 1f);
					ShipActuators.RowingSoundEventData[] rowingSoundEventData = this._rowingSoundEventData;
					int num21 = 0;
					rowingSoundEventData[num21].NumberOfActiveOars = rowingSoundEventData[num21].NumberOfActiveOars + (this._leftSideOars[i].Item2.IsExtracted ? 1 : 0);
				}
				for (int j = 0; j < this._rightSideOars.Count; j++)
				{
					Vec3 bladeContact2 = this._rightSideOars[j].Item2.BladeContact;
					Vec3 vec3 = num3 * item5 * this._oarAppliedForceMultiplierForStoryMission * num11 * f;
					this._rightOarForces[j] = new ShipForce(in bladeContact2, in vec3, ShipForce.SourceType.Oar, 1f);
					ShipActuators.RowingSoundEventData[] rowingSoundEventData2 = this._rowingSoundEventData;
					int num22 = 1;
					rowingSoundEventData2[num22].NumberOfActiveOars = rowingSoundEventData2[num22].NumberOfActiveOars + (this._rightSideOars[j].Item2.IsExtracted ? 1 : 0);
				}
				this.CalculateOarSoundPositionsAndParams();
				return;
			}
			this.StopRovers();
		}

		// Token: 0x06000AC9 RID: 2761 RVA: 0x0004C6B0 File Offset: 0x0004A8B0
		private void StopRovers()
		{
			this._leftOarsPhaseController.Stop();
			for (int i = 0; i < this._leftSideOars.Count; i++)
			{
				this._leftOarForces[i] = ShipForce.None();
			}
			this._rightOarsPhaseController.Stop();
			for (int j = 0; j < this._rightSideOars.Count; j++)
			{
				this._rightOarForces[j] = ShipForce.None();
			}
			for (int k = 0; k < 2; k++)
			{
				SoundEvent oarsSoundEvents = this._rowingSoundEventData[k].OarsSoundEvents;
				if (oarsSoundEvents != null)
				{
					oarsSoundEvents.Stop();
				}
				this._rowingSoundEventData[k].OarsSoundEvents = null;
			}
		}

		// Token: 0x06000ACA RID: 2762 RVA: 0x0004C75C File Offset: 0x0004A95C
		private void FixedUpdateRudder(float fixedDt, in ShipActuatorRecord actuatorInput, in MatrixFrame shipEntityGlobalFrame, float shipForwardSpeed)
		{
			Vec3 u = shipEntityGlobalFrame.rotation.u;
			MatrixFrame matrixFrame = shipEntityGlobalFrame;
			Vec3 vec = matrixFrame.TransformToParent(ref this._rudderStockLocalPosition);
			Vec3 linearVelocityAtGlobalPointForEntityWithDynamicBody = GameEntityPhysicsExtensions.GetLinearVelocityAtGlobalPointForEntityWithDynamicBody(this._ownerMissionShip.GameEntity, vec);
			Mat3 mat = shipEntityGlobalFrame.rotation;
			Vec3 vec2 = mat.TransformToLocal(ref linearVelocityAtGlobalPointForEntityWithDynamicBody);
			float lengthSquared = vec2.LengthSquared;
			if (lengthSquared < 16f)
			{
				if (lengthSquared <= 1f)
				{
					vec2 = Vec3.Zero;
				}
				else
				{
					float length = vec2.Length;
					float num = 1f - (length - 1f) / 3f;
					vec2 = Vec3.Lerp(vec2, new Vec3(0f, (float)MathF.Sign(vec2.y) * length, 0f, -1f), num);
				}
			}
			Vec3 vec3 = vec2;
			vec3.z = 0f;
			vec3 = ((vec3.LengthSquared > 0.0001f) ? vec3 : new Vec3(0f, -1f, 0f, -1f));
			vec3.Normalize();
			Vec3 vec4 = vec3;
			if (vec4.y >= 0f)
			{
				vec4 = -vec4;
			}
			float rudderRotationMax = this._ownerMissionShip.MissionShipObject.RudderRotationMax;
			float num2 = -vec4.AsVec2.AngleBetween(new Vec2(0f, -1f));
			float num3 = 0.8f;
			num2 = MathF.Clamp(num2, -rudderRotationMax * num3, rudderRotationMax * num3);
			float num4 = fixedDt * this._ownerMissionShip.MissionShipObject.RudderRotationRate * 2f;
			this._lastTargetRudderStabilityLocalRotation = num2;
			if (this._lastTargetRudderStabilityLocalRotation > num2)
			{
				this._lastTargetRudderStabilityLocalRotation -= num4;
				if (this._lastTargetRudderStabilityLocalRotation < num2)
				{
					this._lastTargetRudderStabilityLocalRotation = num2;
				}
			}
			else if (this._lastTargetRudderStabilityLocalRotation < num2)
			{
				this._lastTargetRudderStabilityLocalRotation += num4;
				if (this._lastTargetRudderStabilityLocalRotation > num2)
				{
					this._lastTargetRudderStabilityLocalRotation = num2;
				}
			}
			num2 = this._lastTargetRudderStabilityLocalRotation;
			float num5 = actuatorInput.RudderRotation;
			num5 = (float)MathF.Sign(num5) * MathF.Pow(num5, 2f);
			int num6 = -MathF.Sign(num5);
			float num7 = num5 * (float)MathF.Sign((shipForwardSpeed > -1f) ? 1f : shipForwardSpeed) * this._ownerMissionShip.MissionShipObject.RudderRotationMax;
			this.VisualRudderPullDirection = MathF.Sign(num7);
			float num8 = fixedDt * this._ownerMissionShip.MissionShipObject.RudderRotationRate * ((this._lastAddedFromInputRudderLocalRotation * num7 <= 0f) ? 1f : 1f);
			if (this._lastAddedFromInputRudderLocalRotation > num7)
			{
				this._lastAddedFromInputRudderLocalRotation -= num8;
				if (this._lastAddedFromInputRudderLocalRotation < num7)
				{
					this._lastAddedFromInputRudderLocalRotation = num7;
				}
			}
			else if (this._lastAddedFromInputRudderLocalRotation < num7)
			{
				this._lastAddedFromInputRudderLocalRotation += num8;
				if (this._lastAddedFromInputRudderLocalRotation > num7)
				{
					this._lastAddedFromInputRudderLocalRotation = num7;
				}
			}
			this._lastAddedFromInputRudderLocalRotation = MathF.Clamp(num2 + this._lastAddedFromInputRudderLocalRotation, -rudderRotationMax, rudderRotationMax) - num2;
			float num9 = MathF.Clamp(this._ownerMissionShip.Physics.LastSubmergedHeightFactorForActuators, 0f, 1f);
			float num10 = this._ownerMissionShip.MissionShipObject.RudderBladeLength * this._ownerMissionShip.MissionShipObject.RudderBladeHeight;
			float rudderDeflectionCoef = this._ownerMissionShip.MissionShipObject.RudderDeflectionCoef;
			float rudderForceMax = this._ownerMissionShip.MissionShipObject.RudderForceMax;
			Vec3 vec5 = Vec3.Zero;
			float num11 = this._lastAddedFromInputRudderLocalRotation;
			int num12 = ((this._lastAddedFromInputRudderLocalRotation == 0f) ? 1 : (MathF.Ceiling(MathF.Abs(this._lastAddedFromInputRudderLocalRotation) / 0.0017453292f) + 1));
			num12 = MBMath.ClampInt(num12, 1, 250);
			for (int i = 0; i <= num12; i++)
			{
				float num13 = (float)i / (float)num12 * this._lastAddedFromInputRudderLocalRotation;
				float num14 = num2 + num13;
				num14 = MathF.Clamp(num14, -rudderRotationMax, rudderRotationMax);
				ValueTuple<Vec3, Vec3> valueTuple = ShipActuators.ComputeRudderDeflectionForce(num14, in vec4, in vec2, in vec3, num10);
				Vec3 item = valueTuple.Item1;
				Vec3 item2 = valueTuple.Item2;
				if (MathF.Sign(item2.x) == num6)
				{
					Vec3 vec6 = item + item2;
					vec6 *= MathF.Abs(u.z);
					vec6 *= rudderDeflectionCoef;
					vec6 *= num9;
					mat = shipEntityGlobalFrame.rotation;
					vec5 = mat.TransformToParent(ref vec6);
					num11 = num14 - num2;
					if (MathF.Abs(vec6.x) >= rudderForceMax)
					{
						vec5 *= rudderForceMax / MathF.Abs(vec6.x);
						break;
					}
				}
			}
			this._lastAddedFromInputRudderLocalRotation = num11;
			float num15 = fixedDt * this._ownerMissionShip.MissionShipObject.RudderRotationRate * 0.5f;
			if (this._lastAddedFromInputRudderLocalRotation > num11)
			{
				this._lastAddedFromInputRudderLocalRotation -= num15;
				if (this._lastAddedFromInputRudderLocalRotation < num11)
				{
					this._lastAddedFromInputRudderLocalRotation = num11;
				}
			}
			else if (this._lastAddedFromInputRudderLocalRotation < num11)
			{
				this._lastAddedFromInputRudderLocalRotation += num15;
				if (this._lastAddedFromInputRudderLocalRotation > num11)
				{
					this._lastAddedFromInputRudderLocalRotation = num11;
				}
			}
			num11 = this._lastAddedFromInputRudderLocalRotation;
			this._lastRudderLocalRotation = this._rudderLocalRotation;
			float num16 = MathF.Clamp(num2 + num11, -rudderRotationMax, rudderRotationMax);
			this._rudderLocalRotation = MathF.Lerp(this._rudderLocalRotation, num16, fixedDt * 5f, 1E-05f);
			vec5 *= 1f + this._ownerMissionShip.ShipOrigin.RudderSurfaceAreaFactor;
			Vec3 vec7;
			vec7..ctor(0f, -1f, 0f, -1f);
			vec7.RotateAboutZ(this._rudderLocalRotation);
			Vec3 vec8 = this._rudderStockLocalPosition + vec7 * (this._ownerMissionShip.MissionShipObject.RudderBladeLength * 0.5f);
			this._rudderShipForce = new ShipForce(in vec8, in vec5, ShipForce.SourceType.Rudder, rudderDeflectionCoef);
			this._shipPresenceSoundParam = MathF.Min(MathF.Abs(this._rudderShipForce.Force.Length / 10000f), 1f);
			this._rudderStressSoundParam = this._rudderShipForce.Force.LengthSquared / (rudderForceMax * rudderForceMax);
		}

		// Token: 0x06000ACB RID: 2763 RVA: 0x0004CD70 File Offset: 0x0004AF70
		private void OnParallelTickRudder(float dt)
		{
			float num;
			float num2;
			this._cachedOwnerScene.GetInterpolationFactorForBodyWorldTransformSmoothing(ref num, ref num2);
			this.VisualRudderLocalRotation = MathF.Lerp(this._lastRudderLocalRotation, this._rudderLocalRotation, num, 1E-05f);
			Vec3 vec = this._ownerMissionShip.GameEntity.GetGlobalFrame().TransformToParent(ref this._rudderStockLocalPosition);
			float num3 = MathF.Clamp(this._ownerMissionShip.Physics.LastSubmergedHeightFactorForActuators, 0f, 1f);
			float num4 = 0.15f * dt * num3;
			float num5 = this._shipPresenceSoundParam * 0.25f + 0.1f;
			this._cachedOwnerScene.AddWaterWakeWithCapsule(vec, num5 * 1.5f, vec, num5, num4, 0f);
		}

		// Token: 0x06000ACC RID: 2764 RVA: 0x0004CE2C File Offset: 0x0004B02C
		private int ComputeExtractedOarCount()
		{
			int num = 0;
			for (int i = 0; i < this._leftSideOars.Count; i++)
			{
				if (this._leftSideOars[i].Item2.IsExtracted)
				{
					num++;
				}
			}
			for (int j = 0; j < this._rightSideOars.Count; j++)
			{
				if (this._rightSideOars[j].Item2.IsExtracted)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06000ACD RID: 2765 RVA: 0x0004CEA0 File Offset: 0x0004B0A0
		private int ComputeUsedOarCount()
		{
			int num = 0;
			for (int i = 0; i < this._leftSideOars.Count; i++)
			{
				if (this._leftSideOars[i].Item2.IsUsed)
				{
					num++;
				}
			}
			for (int j = 0; j < this._rightSideOars.Count; j++)
			{
				if (this._rightSideOars[j].Item2.IsUsed)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06000ACE RID: 2766 RVA: 0x0004CF14 File Offset: 0x0004B114
		private ValueTuple<float, float> ComputeAverageOarTipPointForwardVelocities()
		{
			MatrixFrame bodyWorldTransform = this._ownerMissionShip.GameEntity.GetBodyWorldTransform();
			Vec3 centerOfMass = this._ownerMissionShip.GameEntity.CenterOfMass;
			Vec3 vec = bodyWorldTransform.TransformToParent(ref centerOfMass);
			Vec3 linearVelocity = this._ownerMissionShip.Physics.LinearVelocity;
			Vec3 angularVelocity = this._ownerMissionShip.Physics.AngularVelocity;
			Vec3 vec2 = bodyWorldTransform.TransformToParent(ref this._leftSideAverageOarLocalPos) - vec;
			Vec3 vec3 = Vec3.CrossProduct(angularVelocity, vec2);
			float num = Vec3.DotProduct(linearVelocity + vec3, bodyWorldTransform.rotation.f);
			Vec3 vec4 = bodyWorldTransform.TransformToParent(ref this._rightSideAverageOarLocalPos) - vec;
			Vec3 vec5 = Vec3.CrossProduct(angularVelocity, vec4);
			float num2 = Vec3.DotProduct(linearVelocity + vec5, bodyWorldTransform.rotation.f);
			return new ValueTuple<float, float>(num, num2);
		}

		// Token: 0x06000ACF RID: 2767 RVA: 0x0004CFF0 File Offset: 0x0004B1F0
		private void FixedUpdateSideOars(float fixedDt, in MatrixFrame shipGlobalFrame, [TupleElementNames(new string[] { "ship", "shipSide" })] MBList<ValueTuple<MissionShip, OarSidePhaseController.OarSide>> nearbyShips, [TupleElementNames(new string[] { "entity", "oar" })] MBList<ValueTuple<GameEntity, MissionOar>> shipOars, ref float maxForceMultiplierFromUser)
		{
			for (int i = 0; i < shipOars.Count; i++)
			{
				MissionOar item = shipOars[i].Item2;
				item.FixedUpdate(fixedDt, in shipGlobalFrame, nearbyShips);
				maxForceMultiplierFromUser = MathF.Max(maxForceMultiplierFromUser, item.ForceMultiplierFromUserAgent);
			}
		}

		// Token: 0x06000AD0 RID: 2768 RVA: 0x0004D038 File Offset: 0x0004B238
		private void UpdateRowerParameters(float rowersThrustRate, float rowersRotationRate, float shipForwardSpeed, out float leftRowersNeededRevolutionRate, out float rightRowersNeededRevolutionRate)
		{
			if (rowersThrustRate != 0f || rowersRotationRate == 0f)
			{
				leftRowersNeededRevolutionRate = rowersThrustRate;
				rightRowersNeededRevolutionRate = rowersThrustRate;
				if (rowersRotationRate != 0f)
				{
					float num = 0.5f;
					if (shipForwardSpeed * rowersThrustRate < 0f && MathF.Abs(shipForwardSpeed) > 6f)
					{
						num = 0f;
					}
					if (rowersThrustRate * rowersRotationRate > 0f)
					{
						leftRowersNeededRevolutionRate = num;
						return;
					}
					rightRowersNeededRevolutionRate = num;
				}
				return;
			}
			if (MathF.Abs(shipForwardSpeed) <= 6f)
			{
				leftRowersNeededRevolutionRate = -rowersRotationRate;
				rightRowersNeededRevolutionRate = rowersRotationRate;
				return;
			}
			if (rowersRotationRate > 0f)
			{
				rightRowersNeededRevolutionRate = rowersRotationRate;
				leftRowersNeededRevolutionRate = 0f;
				return;
			}
			leftRowersNeededRevolutionRate = -rowersRotationRate;
			rightRowersNeededRevolutionRate = 0f;
		}

		// Token: 0x06000AD1 RID: 2769 RVA: 0x0004D0D8 File Offset: 0x0004B2D8
		private IShipOarScriptComponent GetOarScriptFromEntity(WeakGameEntity oarEntity)
		{
			IShipOarScriptComponent shipOarScriptComponent = null;
			WeakGameEntity weakGameEntity = oarEntity;
			while (weakGameEntity.IsValid && shipOarScriptComponent == null)
			{
				shipOarScriptComponent = weakGameEntity.GetFirstScriptOfType<ShipOarMachine>();
				if (shipOarScriptComponent == null)
				{
					shipOarScriptComponent = weakGameEntity.GetFirstScriptOfType<ShipUnmannedOar>();
				}
				weakGameEntity = weakGameEntity.Parent;
			}
			return shipOarScriptComponent;
		}

		// Token: 0x06000AD2 RID: 2770 RVA: 0x0004D114 File Offset: 0x0004B314
		internal static float ComputeActuatorParameter(float value, float target, float dt, float incrementRate)
		{
			float num = target - value;
			float num2 = Math.Min(Math.Abs(num), dt * incrementRate);
			return value + (float)MathF.Sign(num) * num2;
		}

		// Token: 0x06000AD3 RID: 2771 RVA: 0x0004D140 File Offset: 0x0004B340
		private static ValueTuple<Vec3, Vec3> ComputeRudderDeflectionForce(float totalTargetRot, in Vec3 unClampedRudderStabilityDirectionLocal, in Vec3 rudderStockLocalVelocity, in Vec3 rudderStockLocalVelocityDirection, float rudderSurfaceArea)
		{
			Vec3 vec;
			vec..ctor(0f, -1f, 0f, -1f);
			vec.RotateAboutZ(totalTargetRot);
			Vec2 asVec = vec.AsVec2;
			Vec3 vec2 = unClampedRudderStabilityDirectionLocal;
			float num = asVec.AngleBetween(vec2.AsVec2);
			if (num < -1.5707964f)
			{
				num += 3.1415927f;
			}
			else if (num > 1.5707964f)
			{
				num -= 3.1415927f;
			}
			float num2 = 0.5f * NavalPhysics.GetWaterDensity();
			vec2 = rudderStockLocalVelocity;
			float num3 = num2 * vec2.LengthSquared;
			float num4 = MathF.Abs(num);
			float num5 = (float)MathF.Sign((num == 0f) ? 1f : num);
			float num6 = 0.72f * (6.2831855f * num);
			float num7 = 1.1f * MathF.Sin(2f * num4) * num5;
			float num8 = MathF.Sin(num4);
			float num9 = MBMath.SmoothStep(0.20943952f, 0.61086524f, num4);
			float num10 = MBMath.Lerp(num6, num7, num9, 1E-05f);
			float num11 = (0.06f + 0.1f * num10 * num10 + 1.1f * num8) * num8;
			float num12 = num10 * num3 * rudderSurfaceArea;
			float num13 = num11 * num3 * rudderSurfaceArea;
			Vec3 vec3 = -rudderStockLocalVelocityDirection;
			Vec3 vec4 = vec3;
			vec4.RotateAboutZ(1.5707964f);
			Vec3 vec5 = num13 * vec3;
			Vec3 vec6 = num12 * vec4;
			return new ValueTuple<Vec3, Vec3>(vec5, vec6);
		}

		// Token: 0x06000AD4 RID: 2772 RVA: 0x0004D29C File Offset: 0x0004B49C
		public void SetOarAppliedForceMultiplierForStoryMission(float newOarAppliedForceMultiplierForStoryMission)
		{
			this._oarAppliedForceMultiplierForStoryMission = newOarAppliedForceMultiplierForStoryMission;
		}

		// Token: 0x04000638 RID: 1592
		private static readonly int[] _rowingSoundEventIds = new int[]
		{
			SoundManager.GetEventGlobalIndex("event:/mission/movement/vessel/rowing/rowing_left_side"),
			SoundManager.GetEventGlobalIndex("event:/mission/movement/vessel/rowing/rowing_right_side")
		};

		// Token: 0x04000639 RID: 1593
		public const string SailTagPrefix = "sail_center_";

		// Token: 0x0400063A RID: 1594
		public const string RudderStockPositionTag = "rudder_stock";

		// Token: 0x0400063B RID: 1595
		private const float MinSpeedToUseBothOarsToTurn = 6f;

		// Token: 0x0400063C RID: 1596
		private static readonly int _rudderSoundEventId = SoundEvent.GetEventIdFromString("event:/mission/movement/vessel/ship_steering");

		// Token: 0x0400063D RID: 1597
		private static readonly int _shipPresenceSoundEventId = SoundEvent.GetEventIdFromString("event:/mission/movement/vessel/basic_ship_presence");

		// Token: 0x04000640 RID: 1600
		private float _rudderLocalRotation;

		// Token: 0x04000641 RID: 1601
		private float _lastRudderLocalRotation;

		// Token: 0x04000642 RID: 1602
		private float _lastAddedFromInputRudderLocalRotation;

		// Token: 0x04000643 RID: 1603
		private float _lastTargetRudderStabilityLocalRotation;

		// Token: 0x04000644 RID: 1604
		private Vec3 _rudderStockLocalPosition;

		// Token: 0x04000645 RID: 1605
		private readonly MissionShip _ownerMissionShip;

		// Token: 0x04000646 RID: 1606
		private readonly Scene _cachedOwnerScene;

		// Token: 0x04000647 RID: 1607
		private float _rowersPhase;

		// Token: 0x04000648 RID: 1608
		private float _lastFramePhaseRate;

		// Token: 0x04000649 RID: 1609
		private bool _evenCycle;

		// Token: 0x0400064A RID: 1610
		private ShipActuators.OarPhaseData _leftPhaseData;

		// Token: 0x0400064B RID: 1611
		private ShipActuators.OarPhaseData _rightPhaseData;

		// Token: 0x0400064C RID: 1612
		private readonly MBList<MissionSail> _sails = new MBList<MissionSail>();

		// Token: 0x0400064D RID: 1613
		[TupleElementNames(new string[] { "entity", "oar" })]
		private readonly MBList<ValueTuple<GameEntity, MissionOar>> _leftSideOars = new MBList<ValueTuple<GameEntity, MissionOar>>();

		// Token: 0x0400064E RID: 1614
		[TupleElementNames(new string[] { "entity", "oar" })]
		private readonly MBList<ValueTuple<GameEntity, MissionOar>> _rightSideOars = new MBList<ValueTuple<GameEntity, MissionOar>>();

		// Token: 0x0400064F RID: 1615
		private MBList<ShipForce> _leftOarForces = new MBList<ShipForce>();

		// Token: 0x04000650 RID: 1616
		private MBList<ShipForce> _rightOarForces = new MBList<ShipForce>();

		// Token: 0x04000651 RID: 1617
		private MBList<ShipForce> _sailForces = new MBList<ShipForce>();

		// Token: 0x04000652 RID: 1618
		private ShipForce _rudderShipForce;

		// Token: 0x04000653 RID: 1619
		private OarSidePhaseController _leftOarsPhaseController;

		// Token: 0x04000654 RID: 1620
		private OarSidePhaseController _rightOarsPhaseController;

		// Token: 0x04000655 RID: 1621
		private float _oarsmenForceMultiplier;

		// Token: 0x04000656 RID: 1622
		private float _oarsmenSpeedMultiplier;

		// Token: 0x04000657 RID: 1623
		private float _oarsTipSpeedReferenceMultiplier;

		// Token: 0x04000658 RID: 1624
		private float _oarFrictionMultiplier;

		// Token: 0x04000659 RID: 1625
		private float _oarAppliedForceMultiplierForStoryMission;

		// Token: 0x0400065A RID: 1626
		private float _maxOarLength;

		// Token: 0x0400065B RID: 1627
		[TupleElementNames(new string[] { "ship", "shipSide" })]
		private readonly MBList<ValueTuple<MissionShip, OarSidePhaseController.OarSide>> _nearbyShips;

		// Token: 0x0400065C RID: 1628
		private float _timeLeftToUpdateNearbyShips;

		// Token: 0x0400065D RID: 1629
		private readonly NavalShipsLogic _navalShipsLogic;

		// Token: 0x0400065E RID: 1630
		private Vec3 _leftSideAverageOarLocalPos;

		// Token: 0x0400065F RID: 1631
		private Vec3 _rightSideAverageOarLocalPos;

		// Token: 0x04000660 RID: 1632
		private SoundEvent _rudderSoundEvent;

		// Token: 0x04000661 RID: 1633
		private SoundEvent _shipPresenceSoundEvent;

		// Token: 0x04000662 RID: 1634
		private ShipActuators.RowingSoundEventData[] _rowingSoundEventData = new ShipActuators.RowingSoundEventData[2];

		// Token: 0x04000663 RID: 1635
		private float _rudderStressSoundParam;

		// Token: 0x04000664 RID: 1636
		private float _shipPresenceSoundParam;

		// Token: 0x0200020F RID: 527
		private struct RowingSoundEventData
		{
			// Token: 0x04000EB9 RID: 3769
			internal float SoundEventRowingPowerParam;

			// Token: 0x04000EBA RID: 3770
			internal int NumberOfActiveOars;

			// Token: 0x04000EBB RID: 3771
			internal bool ShouldTriggerOarSound;

			// Token: 0x04000EBC RID: 3772
			internal bool IsOarsInWater;

			// Token: 0x04000EBD RID: 3773
			internal Vec3 RowingSoundEventPositions;

			// Token: 0x04000EBE RID: 3774
			internal int FurthestOarIndex;

			// Token: 0x04000EBF RID: 3775
			internal int ClosestOarIndex;

			// Token: 0x04000EC0 RID: 3776
			internal SoundEvent OarsSoundEvents;
		}

		// Token: 0x02000210 RID: 528
		public struct OarPhaseData
		{
			// Token: 0x04000EC1 RID: 3777
			public float CurPhase;

			// Token: 0x04000EC2 RID: 3778
			public float LastNonZeroRevolutionRate;

			// Token: 0x04000EC3 RID: 3779
			public bool LockedToTargetPhase;

			// Token: 0x04000EC4 RID: 3780
			public float CycleArcSizeMult;
		}

		// Token: 0x02000211 RID: 529
		public struct OarAnimKeyFrame
		{
			// Token: 0x06001AFD RID: 6909 RVA: 0x000B1C69 File Offset: 0x000AFE69
			public OarAnimKeyFrame(float keyProgress, float speed)
			{
				this.KeyProgress = keyProgress;
				this.Speed = speed;
			}

			// Token: 0x04000EC5 RID: 3781
			public float KeyProgress;

			// Token: 0x04000EC6 RID: 3782
			public float Speed;
		}

		// Token: 0x02000212 RID: 530
		private static class OarRowSpeedAnimationManager
		{
			// Token: 0x04000EC7 RID: 3783
			public static ShipActuators.OarAnimKeyFrame[] ForwardPhaseSpeedAnim = new ShipActuators.OarAnimKeyFrame[]
			{
				new ShipActuators.OarAnimKeyFrame(0f, 1.5f),
				new ShipActuators.OarAnimKeyFrame(0.15f, 1.6f),
				new ShipActuators.OarAnimKeyFrame(0.25f, 1.2f),
				new ShipActuators.OarAnimKeyFrame(0.3f, 1f),
				new ShipActuators.OarAnimKeyFrame(0.65f, 1f),
				new ShipActuators.OarAnimKeyFrame(0.7f, 1.4f),
				new ShipActuators.OarAnimKeyFrame(0.75f, 1.5f),
				new ShipActuators.OarAnimKeyFrame(0.9f, 1.5f),
				new ShipActuators.OarAnimKeyFrame(1f, 1.5f)
			};

			// Token: 0x04000EC8 RID: 3784
			public static ShipActuators.OarAnimKeyFrame[] PartialPhaseSpeedAnim = new ShipActuators.OarAnimKeyFrame[]
			{
				new ShipActuators.OarAnimKeyFrame(0f, 1.5f),
				new ShipActuators.OarAnimKeyFrame(0.15f, 1.6f),
				new ShipActuators.OarAnimKeyFrame(0.25f, 1.2f),
				new ShipActuators.OarAnimKeyFrame(0.3f, 1f),
				new ShipActuators.OarAnimKeyFrame(0.65f, 1f),
				new ShipActuators.OarAnimKeyFrame(0.7f, 1.4f),
				new ShipActuators.OarAnimKeyFrame(0.75f, 1.5f),
				new ShipActuators.OarAnimKeyFrame(0.9f, 1.5f),
				new ShipActuators.OarAnimKeyFrame(1f, 1.5f)
			};

			// Token: 0x04000EC9 RID: 3785
			public static ShipActuators.OarAnimKeyFrame[] OnPointTurnPhaseSpeedAnim = new ShipActuators.OarAnimKeyFrame[]
			{
				new ShipActuators.OarAnimKeyFrame(0f, 1.5f),
				new ShipActuators.OarAnimKeyFrame(0.15f, 1.6f),
				new ShipActuators.OarAnimKeyFrame(0.25f, 1.2f),
				new ShipActuators.OarAnimKeyFrame(0.3f, 1f),
				new ShipActuators.OarAnimKeyFrame(0.65f, 1f),
				new ShipActuators.OarAnimKeyFrame(0.7f, 1.4f),
				new ShipActuators.OarAnimKeyFrame(0.75f, 1.5f),
				new ShipActuators.OarAnimKeyFrame(0.9f, 1.5f),
				new ShipActuators.OarAnimKeyFrame(1f, 1.5f)
			};
		}
	}
}
