using System;
using System.Collections.Generic;
using System.Linq;
using NavalDLC.Missions.AI.UsableMachineAIs;
using NavalDLC.Missions.NavalPhysics;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace NavalDLC.Missions.Objects.UsableMachines
{
	// Token: 0x020000B9 RID: 185
	public class ShipPullingMachine : UsableMachine
	{
		// Token: 0x06000E27 RID: 3623 RVA: 0x0006EC5C File Offset: 0x0006CE5C
		protected override void OnInit()
		{
			base.OnInit();
			base.SetScriptComponentToTick(this.GetTickRequirement());
		}

		// Token: 0x06000E28 RID: 3624 RVA: 0x0006EC70 File Offset: 0x0006CE70
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 2;
		}

		// Token: 0x06000E29 RID: 3625 RVA: 0x0006EC74 File Offset: 0x0006CE74
		private void RotateMachine(float dt)
		{
			float num = 0f;
			Vec2 vec;
			vec..ctor(-Input.GetMouseMoveX(), -Input.GetMouseMoveY());
			if (vec.IsNonZero())
			{
				float num2 = Math.Min(vec.Normalize(), 5f) * 0.2f;
				num = vec.x * num2;
			}
			if (num != 0f)
			{
				this.currentDirection += 1f * dt * num;
				this.currentDirection = MBMath.WrapAngle(this.currentDirection);
			}
			MatrixFrame frame = base.GameEntity.GetFrame();
			frame.rotation = Mat3.Identity;
			frame.rotation.RotateAboutUp(this.currentDirection);
			base.GameEntity.SetFrame(ref frame, true);
		}

		// Token: 0x06000E2A RID: 3626 RVA: 0x0006ED33 File Offset: 0x0006CF33
		protected override void OnFixedTick(float fixedDt)
		{
		}

		// Token: 0x06000E2B RID: 3627 RVA: 0x0006ED38 File Offset: 0x0006CF38
		protected override void OnTick(float dt)
		{
			if (base.UserCountNotInStruckAction > 0 && base.PilotAgent != null)
			{
				this.RotateMachine(dt);
				if (!base.PilotAgent.IsInBeingStruckAction && base.PilotAgent.Mission.InputManager.IsGameKeyDown(9))
				{
					if (this.pointToPull != null)
					{
						this.PullOtherShip(this.pointToPull);
						return;
					}
				}
				else
				{
					this.FindPointToPull();
				}
			}
		}

		// Token: 0x06000E2C RID: 3628 RVA: 0x0006EDA4 File Offset: 0x0006CFA4
		private void FindPointToPull()
		{
			WeakGameEntity pullPointHolderEntity = WeakGameEntity.Invalid;
			foreach (WeakGameEntity weakGameEntity in base.GameEntity.Root.GetChildren())
			{
				if (weakGameEntity.Name == "pull_point_holder")
				{
					pullPointHolderEntity = weakGameEntity;
					break;
				}
			}
			MatrixFrame globalFrame = base.GameEntity.GetGlobalFrame();
			Vec3 vec = globalFrame.rotation.f.NormalizedCopy();
			IEnumerable<GameEntity> enumerable = from x in base.Scene.FindEntitiesWithTag("ShipPullPoint")
				where x.Parent != pullPointHolderEntity
				select x;
			GameEntity gameEntity = null;
			float num = -1.1f;
			Vec3 lookDirection = base.StandingPoints[0].UserAgent.LookDirection;
			Vec3 position = base.StandingPoints[0].UserAgent.Position;
			lookDirection.Normalize();
			foreach (GameEntity gameEntity2 in enumerable)
			{
				MatrixFrame globalFrame2 = gameEntity2.GetGlobalFrame();
				if (Vec3.DotProduct(globalFrame2.origin - globalFrame.origin, vec) > 0f && Vec3.DotProduct(globalFrame2.rotation.f.NormalizedCopy(), vec) < 0f)
				{
					float num2 = Vec3.DotProduct((globalFrame2.origin - position).NormalizedCopy(), lookDirection);
					if (num2 > num)
					{
						num = num2;
						gameEntity = gameEntity2;
					}
				}
			}
			if (gameEntity != null)
			{
				this.pointToPull = gameEntity;
			}
		}

		// Token: 0x06000E2D RID: 3629 RVA: 0x0006EF70 File Offset: 0x0006D170
		private void PullOtherShip(GameEntity otherAttachmentPoint)
		{
			MissionShip firstScriptOfType = base.GameEntity.Root.GetFirstScriptOfType<MissionShip>();
			MissionShip firstScriptOfType2 = otherAttachmentPoint.Root.GetFirstScriptOfType<MissionShip>();
			Vec3 vec = otherAttachmentPoint.GlobalPosition - base.GameEntity.GlobalPosition;
			vec.Normalize();
			float num = 25f;
			NavalPhysics physics = firstScriptOfType.Physics;
			MatrixFrame matrixFrame = base.GameEntity.GetFrame();
			Vec3 vec2 = vec * num;
			physics.ApplyGlobalForceAtLocalPos(in matrixFrame.origin, in vec2, 0);
			NavalPhysics physics2 = firstScriptOfType2.Physics;
			matrixFrame = otherAttachmentPoint.GetFrame();
			vec2 = -vec * num;
			physics2.ApplyGlobalForceAtLocalPos(in matrixFrame.origin, in vec2, 0);
		}

		// Token: 0x06000E2E RID: 3630 RVA: 0x0006F01F File Offset: 0x0006D21F
		protected override void OnMissionReset()
		{
		}

		// Token: 0x06000E2F RID: 3631 RVA: 0x0006F021 File Offset: 0x0006D221
		public override TextObject GetActionTextForStandingPoint(UsableMissionObject usableGameObject)
		{
			TextObject textObject = new TextObject("{=fEQAPJ2e}{KEY} Use", null);
			textObject.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13), 1f));
			return textObject;
		}

		// Token: 0x06000E30 RID: 3632 RVA: 0x0006F050 File Offset: 0x0006D250
		public override TextObject GetDescriptionText(WeakGameEntity gameEntity)
		{
			return new TextObject("{=5Pf5coO6}Ship Pulling machine", null);
		}

		// Token: 0x06000E31 RID: 3633 RVA: 0x0006F05D File Offset: 0x0006D25D
		public override UsableMachineAIBase CreateAIBehaviorObject()
		{
			return new ShipPullingMachineAI(this);
		}

		// Token: 0x040008DA RID: 2266
		private const string ShipPullPointTag = "ShipPullPoint";

		// Token: 0x040008DB RID: 2267
		private const float pullForceMult = 25f;

		// Token: 0x040008DC RID: 2268
		private float currentDirection;

		// Token: 0x040008DD RID: 2269
		private GameEntity pointToPull;
	}
}
