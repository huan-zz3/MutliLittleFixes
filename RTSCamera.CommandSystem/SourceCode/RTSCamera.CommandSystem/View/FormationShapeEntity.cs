using System;
using MissionSharedLibrary.Config;
using RTSCamera.CommandSystem.Config;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace RTSCamera.CommandSystem.View
{
	// Token: 0x02000051 RID: 81
	public class FormationShapeEntity
	{
		// Token: 0x06000286 RID: 646 RVA: 0x00009255 File Offset: 0x00007455
		public static void Initialize()
		{
			if (FormationShapeEntity._cachedEntity == null)
			{
				FormationShapeEntity._cachedEntity = GameEntity.Instantiate(Mission.Current.Scene, "rts_decal_white_prefab_2", false, true, "");
			}
		}

		// Token: 0x06000287 RID: 647 RVA: 0x00009284 File Offset: 0x00007484
		public static void Clear()
		{
			FormationShapeEntity._cachedEntity = null;
		}

		// Token: 0x06000288 RID: 648 RVA: 0x0000928C File Offset: 0x0000748C
		public void CreateEntities()
		{
			this.FrontLine = this.CreateLineEntity();
			this.LeftLine = this.CreateLineEntity();
			this.RightLine = this.CreateLineEntity();
			this.LeftBackLine = this.CreateLineEntity();
			this.RightBackLine = this.CreateLineEntity();
			this._isShown = true;
		}

		// Token: 0x06000289 RID: 649 RVA: 0x000092DC File Offset: 0x000074DC
		private GameEntity CreateLineEntity()
		{
			GameEntity gameEntity = GameEntity.CopyFrom(Mission.Current.Scene, FormationShapeEntity._cachedEntity, true, true);
			gameEntity.SetMobility(1);
			Decal decal = gameEntity.GetComponentAtIndex(0, 7) as Decal;
			if (decal != null)
			{
				decal.SetIsVisible(true);
				decal.CheckAndRegisterToDecalSet();
				Mission.Current.Scene.AddDecalInstance(decal, "editor_set", true);
			}
			return gameEntity;
		}

		// Token: 0x0600028A RID: 650 RVA: 0x00009340 File Offset: 0x00007540
		public void Update(Vec3 orderPosition, Vec2 direciton, float width, float depth, float rightSideOffset, bool isSelected)
		{
			this._isShown = true;
			float num = 0.5f;
			float num2 = 0.1f;
			float num3 = 0.1f + rightSideOffset;
			float num4 = 0f;
			Vec2 vec = direciton.RightVec();
			float num5 = 1f;
			uint num6 = (isSelected ? FormationShapeEntity.SelectedColor : FormationShapeEntity.UnselectedColor);
			MatrixFrame matrixFrame = this.GetMatrixFrame(orderPosition + Vec3.Up * num5 + (direciton * num + vec * (num3 - num2) / 2f).ToVec3(0f), vec, width + num2 + num3);
			this.FrontLine.SetGlobalFrame(ref matrixFrame, true);
			this.FrontLine.SetVisibilityExcludeParents(true);
			this.FrontLine.SetFactorColor(num6);
			this.FrontLine.SetAlpha(isSelected ? (-1f) : 0.2f);
			MatrixFrame matrixFrame2 = this.GetMatrixFrame(orderPosition + Vec3.Up * num5 + (vec * (-width / 2f - num2) + direciton * (-depth + num - num4) / 2f).ToVec3(0f), direciton, depth + num + num4);
			this.LeftLine.SetGlobalFrame(ref matrixFrame2, true);
			this.LeftLine.SetVisibilityExcludeParents(true);
			this.LeftLine.SetAlpha(isSelected ? (-1f) : 0.2f);
			this.LeftLine.SetFactorColor(num6);
			MatrixFrame matrixFrame3 = this.GetMatrixFrame(orderPosition + Vec3.Up * num5 + (vec * (width / 2f + num3) + direciton * (-depth + num - num4) / 2f).ToVec3(0f), direciton, depth + num + num4);
			this.RightLine.SetGlobalFrame(ref matrixFrame3, true);
			this.RightLine.SetVisibilityExcludeParents(true);
			this.RightLine.SetAlpha(isSelected ? (-1f) : 0.2f);
			this.RightLine.SetFactorColor(num6);
			float num7 = MathF.Min(MathF.Clamp(width * 0.1f, 1f, 10f), depth * 0.3f);
			MatrixFrame matrixFrame4 = this.GetMatrixFrame(orderPosition + Vec3.Up * num5 + (direciton * (-depth - num4) + vec * ((num7 - width) / 2f - num2)).ToVec3(0f), vec, num7);
			this.LeftBackLine.SetGlobalFrame(ref matrixFrame4, true);
			this.LeftBackLine.SetVisibilityExcludeParents(true);
			this.LeftBackLine.SetAlpha(isSelected ? (-1f) : 0.2f);
			this.LeftBackLine.SetFactorColor(num6);
			MatrixFrame matrixFrame5 = this.GetMatrixFrame(orderPosition + Vec3.Up * num5 + (direciton * (-depth - num4) + vec * ((width - num7) / 2f + num3)).ToVec3(0f), vec, num7);
			this.RightBackLine.SetGlobalFrame(ref matrixFrame5, true);
			this.RightBackLine.SetVisibilityExcludeParents(true);
			this.RightBackLine.SetAlpha(isSelected ? (-1f) : 0.2f);
			this.RightBackLine.SetFactorColor(num6);
		}

		// Token: 0x0600028B RID: 651 RVA: 0x000096C0 File Offset: 0x000078C0
		private MatrixFrame GetMatrixFrame(Vec3 middlePosition, Vec2 lineDirection, float length)
		{
			MatrixFrame identity = MatrixFrame.Identity;
			identity.origin = middlePosition;
			Vec3 vec = lineDirection.ToVec3(0f);
			identity.rotation = Mat3.CreateMat3WithForward(ref vec);
			vec = new Vec3(0.1f, length / 2f, 100f, -1f);
			identity.Scale(ref vec);
			return identity;
		}

		// Token: 0x0600028C RID: 652 RVA: 0x0000971C File Offset: 0x0000791C
		public void Hide(bool isPreviewShown)
		{
			if (isPreviewShown)
			{
				this.FrontLine.SetVisibilityExcludeParents(false);
				this.LeftLine.SetVisibilityExcludeParents(false);
				this.RightLine.SetVisibilityExcludeParents(false);
				this.LeftBackLine.SetVisibilityExcludeParents(false);
				this.RightBackLine.SetVisibilityExcludeParents(false);
			}
			else if (this._isShown)
			{
				GameEntityExtensions.FadeOut(this.FrontLine, MissionConfigBase<CommandSystemConfig>.Get().MovementTargetFadeOutDuration, false);
				GameEntityExtensions.FadeOut(this.LeftLine, MissionConfigBase<CommandSystemConfig>.Get().MovementTargetFadeOutDuration, false);
				GameEntityExtensions.FadeOut(this.RightLine, MissionConfigBase<CommandSystemConfig>.Get().MovementTargetFadeOutDuration, false);
				GameEntityExtensions.FadeOut(this.LeftBackLine, MissionConfigBase<CommandSystemConfig>.Get().MovementTargetFadeOutDuration, false);
				GameEntityExtensions.FadeOut(this.RightBackLine, MissionConfigBase<CommandSystemConfig>.Get().MovementTargetFadeOutDuration, false);
			}
			this._isShown = false;
		}

		// Token: 0x04000112 RID: 274
		public GameEntity FrontLine;

		// Token: 0x04000113 RID: 275
		public GameEntity LeftLine;

		// Token: 0x04000114 RID: 276
		public GameEntity RightLine;

		// Token: 0x04000115 RID: 277
		public GameEntity LeftBackLine;

		// Token: 0x04000116 RID: 278
		public GameEntity RightBackLine;

		// Token: 0x04000117 RID: 279
		private bool _isShown;

		// Token: 0x04000118 RID: 280
		public static uint SelectedColor = new Color(0.5f, 1f, 0.5f, 1f).ToUnsignedInteger();

		// Token: 0x04000119 RID: 281
		public static uint UnselectedColor = new Color(1f, 1f, 1f, 1f).ToUnsignedInteger();

		// Token: 0x0400011A RID: 282
		private static GameEntity _cachedEntity;

		// Token: 0x0400011B RID: 283
		public static uint FormationShapeColor = new Color(0.7f, 1f, 0.7f, 1f).ToUnsignedInteger();
	}
}
