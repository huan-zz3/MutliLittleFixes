using System;
using NavalDLC.Missions.NavalPhysics;
using NavalDLC.Missions.ShipActuators;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.Objects.UsableMachines
{
	// Token: 0x020000BB RID: 187
	public class ShipUnmannedOar : ScriptComponentBehavior, IShipOarScriptComponent
	{
		// Token: 0x06000E38 RID: 3640 RVA: 0x0006F108 File Offset: 0x0006D308
		protected override void OnInit()
		{
			base.OnInit();
			WeakGameEntity weakGameEntity;
			WeakGameEntity weakGameEntity2;
			ShipOarDeck.LoadOarScriptEntity(base.GameEntity, out weakGameEntity, ref this._oarExtractedEntitialFrame, ref this._oarRetractedEntitialFrame, out weakGameEntity2);
			this._oarEntity = (weakGameEntity.IsValid ? GameEntity.CreateFromWeakEntity(weakGameEntity) : null);
			base.SetScriptComponentToTick(this.GetTickRequirement());
			weakGameEntity2 = base.GameEntity;
			this._destructableComponent = weakGameEntity2.GetFirstScriptOfType<DestructableComponent>();
			weakGameEntity2 = base.GameEntity;
			weakGameEntity2.SetHasCustomBoundingBoxValidationSystem(true);
			weakGameEntity2 = base.GameEntity;
			this._unmannedOarBaseBoundingBox = weakGameEntity2.ComputeBoundingBoxFromLongestHalfDimension(2f);
		}

		// Token: 0x06000E39 RID: 3641 RVA: 0x0006F196 File Offset: 0x0006D396
		public void InitializeOar(MissionOar oar)
		{
			this._oar = oar;
		}

		// Token: 0x06000E3A RID: 3642 RVA: 0x0006F19F File Offset: 0x0006D39F
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return base.GetTickRequirement() | 4;
		}

		// Token: 0x06000E3B RID: 3643 RVA: 0x0006F1AC File Offset: 0x0006D3AC
		public void ArrangeOarBoundingBox()
		{
			base.GameEntity.SetManualLocalBoundingBox(ref this._unmannedOarBaseBoundingBox);
			base.GameEntity.Parent.SetBoundingboxDirty();
		}

		// Token: 0x06000E3C RID: 3644 RVA: 0x0006F1E4 File Offset: 0x0006D3E4
		protected override void OnBoundingBoxValidate()
		{
			BoundingBox boundingBox = base.GameEntity.ComputeBoundingBoxIncludeChildren();
			boundingBox.RelaxWithBoundingBox(this._unmannedOarBaseBoundingBox);
			boundingBox.RecomputeRadius();
			base.GameEntity.RelaxLocalBoundingBox(ref boundingBox);
		}

		// Token: 0x06000E3D RID: 3645 RVA: 0x0006F224 File Offset: 0x0006D424
		public bool CheckOarMachineFlags(bool editMode)
		{
			foreach (WeakGameEntity weakGameEntity in base.GameEntity.GetChildren())
			{
				if (!Extensions.HasAnyFlag<EntityFlags>(weakGameEntity.EntityFlags, 131072) && !Extensions.HasAnyFlag<EntityFlags>(weakGameEntity.EntityFlags, 4096))
				{
					string text = string.Format("Root Entity: {0} {1}'s child {2} must have Does not Affect Parent's Local Bounding Box flag.", base.GameEntity.Root.Name, base.GameEntity.Name, weakGameEntity.Name);
					if (editMode)
					{
						MBEditor.AddEntityWarning(weakGameEntity, text);
					}
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000E3E RID: 3646 RVA: 0x0006F2E4 File Offset: 0x0006D4E4
		public void SetSlowDownPhaseForDuration(float slowDownMultiplier, float slowDownDuration)
		{
			this._oar.SetSlowDownPhaseForDuration(slowDownMultiplier, slowDownDuration);
		}

		// Token: 0x06000E3F RID: 3647 RVA: 0x0006F2F4 File Offset: 0x0006D4F4
		protected override void OnTickParallel(float dt)
		{
			bool flag = !this._oar.OwnerShip.BeingAbandoned && this._oar.OwnerShip.Physics.NavalSinkingState == NavalPhysics.SinkingState.Floating && (this._destructableComponent == null || !this._destructableComponent.IsDestroyed);
			this._oar.SetUsed(flag, -1);
			MissionOar oar = this._oar;
			MatrixFrame localFrame = base.GameEntity.GetLocalFrame();
			MatrixFrame localFrame2 = this._oarEntity.GetLocalFrame();
			MatrixFrame matrixFrame = oar.ComputeOarEntityFrame(dt, in localFrame, in localFrame2, in this._oarExtractedEntitialFrame, in this._oarRetractedEntitialFrame, this._lastIdleTime, true);
			this._oarEntity.SetLocalFrame(ref matrixFrame, false);
			if (!this._oar.IsExtracted)
			{
				this._lastIdleTime = Mission.Current.CurrentTime;
			}
		}

		// Token: 0x040008DF RID: 2271
		private GameEntity _oarEntity;

		// Token: 0x040008E0 RID: 2272
		private MatrixFrame _oarExtractedEntitialFrame;

		// Token: 0x040008E1 RID: 2273
		private MatrixFrame _oarRetractedEntitialFrame;

		// Token: 0x040008E2 RID: 2274
		private MissionOar _oar;

		// Token: 0x040008E3 RID: 2275
		private float _lastIdleTime;

		// Token: 0x040008E4 RID: 2276
		private DestructableComponent _destructableComponent;

		// Token: 0x040008E5 RID: 2277
		private BoundingBox _unmannedOarBaseBoundingBox;
	}
}
