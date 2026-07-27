using System;
using NavalDLC.Missions.NavalPhysics;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace NavalDLC.View.Cinematic
{
	// Token: 0x0200003D RID: 61
	public class PopupSceneShipController : ScriptComponentBehavior
	{
		// Token: 0x060001D6 RID: 470 RVA: 0x0000DEC6 File Offset: 0x0000C0C6
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 16;
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0000DECA File Offset: 0x0000C0CA
		public PopupSceneShipController()
		{
			this.StartApplyingForce = new SimpleButton();
			this.StopApplyingForce = new SimpleButton();
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x0000DEFE File Offset: 0x0000C0FE
		protected override void OnInit()
		{
			base.OnInit();
			this._isApplyingForce = true;
			this._targetShipEntity = base.Scene.FindEntityWithTag(this._targetShipEntityTag);
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x0000DF24 File Offset: 0x0000C124
		protected override void OnFixedTick(float fixedDt)
		{
			this.ApplyForce(fixedDt);
		}

		// Token: 0x060001DA RID: 474 RVA: 0x0000DF2D File Offset: 0x0000C12D
		protected override void OnEditorTick(float dt)
		{
			base.OnEditorTick(dt);
			this.ApplyForce(0.016f);
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000DF41 File Offset: 0x0000C141
		protected override void OnParallelFixedTick(float fixedDt)
		{
			base.OnParallelFixedTick(fixedDt);
			this.ApplyForce(fixedDt);
		}

		// Token: 0x060001DC RID: 476 RVA: 0x0000DF54 File Offset: 0x0000C154
		private void ApplyForce(float dt)
		{
			GameEntity targetShipEntity = this._targetShipEntity;
			if (((targetShipEntity != null) ? targetShipEntity.Scene : null) != base.Scene)
			{
				this._targetShipEntity = base.Scene.FindEntityWithTag(this._targetShipEntityTag);
			}
			GameEntity targetShipEntity2 = this._targetShipEntity;
			if (((targetShipEntity2 != null) ? targetShipEntity2.Scene : null) != base.Scene)
			{
				return;
			}
			NavalPhysics firstScriptOfType = this._targetShipEntity.GetFirstScriptOfType<NavalPhysics>();
			if (this._isAnchored)
			{
				if (firstScriptOfType != null)
				{
					firstScriptOfType.SetAnchorFrame(in Vec2.Zero, in Vec2.Forward, 1f);
					firstScriptOfType.SetAnchor(true, false, 1f);
					return;
				}
			}
			else if (this._isApplyingForce)
			{
				Vec3 vec = this._continousForce * this._targetShipEntity.Mass * dt;
				GameEntityPhysicsExtensions.ApplyLocalForceAtLocalPosToDynamicBody(this._targetShipEntity, base.GameEntity.CenterOfMass, vec, 0);
			}
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0000E034 File Offset: 0x0000C234
		protected override void OnEditorVariableChanged(string variableName)
		{
			base.OnEditorVariableChanged(variableName);
			if (variableName == "StartApplyingForce")
			{
				if (this._isApplyingForce)
				{
					return;
				}
				this._targetShipEntity = base.Scene.FindEntityWithTag(this._targetShipEntityTag);
				if (this._targetShipEntity != null)
				{
					this._isApplyingForce = true;
					this._initialShipFrame = this._targetShipEntity.GetGlobalFrame();
					return;
				}
			}
			else if (variableName == "StopApplyingForce")
			{
				if (!this._isApplyingForce)
				{
					return;
				}
				this._targetShipEntity = base.Scene.FindEntityWithTag(this._targetShipEntityTag);
				if (this._targetShipEntity != null)
				{
					this._targetShipEntity.SetGlobalFrame(ref this._initialShipFrame, true);
					GameEntityPhysicsExtensions.SetAngularVelocity(this._targetShipEntity, Vec3.Zero);
					GameEntityPhysicsExtensions.SetLinearVelocity(this._targetShipEntity, Vec3.Zero);
					this._isApplyingForce = false;
				}
			}
		}

		// Token: 0x040000C9 RID: 201
		[EditableScriptComponentVariable(true, "")]
		private Vec3 _continousForce = Vec3.Zero;

		// Token: 0x040000CA RID: 202
		[EditableScriptComponentVariable(true, "")]
		private bool _isAnchored;

		// Token: 0x040000CB RID: 203
		[EditableScriptComponentVariable(true, "")]
		private string _targetShipEntityTag = string.Empty;

		// Token: 0x040000CC RID: 204
		private GameEntity _targetShipEntity;

		// Token: 0x040000CD RID: 205
		private MatrixFrame _initialShipFrame;

		// Token: 0x040000CE RID: 206
		private bool _isApplyingForce;

		// Token: 0x040000CF RID: 207
		public SimpleButton StartApplyingForce;

		// Token: 0x040000D0 RID: 208
		public SimpleButton StopApplyingForce;
	}
}
