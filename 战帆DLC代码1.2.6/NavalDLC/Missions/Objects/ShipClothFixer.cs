using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace NavalDLC.Missions.Objects
{
	// Token: 0x020000A3 RID: 163
	internal class ShipClothFixer : ScriptComponentBehavior
	{
		// Token: 0x06000CAE RID: 3246 RVA: 0x00061716 File Offset: 0x0005F916
		private ShipClothFixer()
		{
		}

		// Token: 0x06000CAF RID: 3247 RVA: 0x0006173F File Offset: 0x0005F93F
		protected override void OnEditorInit()
		{
			this.FetchClothComponents();
		}

		// Token: 0x06000CB0 RID: 3248 RVA: 0x00061747 File Offset: 0x0005F947
		protected override void OnInit()
		{
			this.FetchClothComponents();
		}

		// Token: 0x06000CB1 RID: 3249 RVA: 0x00061750 File Offset: 0x0005F950
		protected override void OnEditorTick(float dt)
		{
			foreach (ShipClothFixer.ClothData clothData in this._shipCloths)
			{
				this.SetPrevFrameToCloth(clothData);
			}
		}

		// Token: 0x06000CB2 RID: 3250 RVA: 0x000617A4 File Offset: 0x0005F9A4
		public override ScriptComponentBehavior.TickRequirement GetTickRequirement()
		{
			return 36;
		}

		// Token: 0x06000CB3 RID: 3251 RVA: 0x000617A8 File Offset: 0x0005F9A8
		protected override void OnParallelFixedTick(float fixedDt)
		{
			this._prevPrevShipFrame = this._prevShipFrame;
			this._prevShipFrame = base.GameEntity.Root.GetBodyWorldTransform();
			this._fixedDt = fixedDt;
			this._frameCounter++;
		}

		// Token: 0x06000CB4 RID: 3252 RVA: 0x000617F4 File Offset: 0x0005F9F4
		protected override void OnTickParallel(float dt)
		{
			foreach (ShipClothFixer.ClothData clothData in this._shipCloths)
			{
				this.SetPrevFrameToCloth(clothData);
			}
		}

		// Token: 0x06000CB5 RID: 3253 RVA: 0x00061848 File Offset: 0x0005FA48
		private void FetchClothComponents()
		{
			this._shipCloths.Clear();
			MatrixFrame globalFrame = base.GameEntity.Root.GetGlobalFrame();
			List<WeakGameEntity> list = new List<WeakGameEntity>();
			base.GameEntity.Root.GetChildrenRecursive(ref list);
			foreach (WeakGameEntity weakGameEntity in list)
			{
				int componentCount = weakGameEntity.GetComponentCount(3);
				for (int i = 0; i < componentCount; i++)
				{
					ShipClothFixer.ClothData clothData = default(ShipClothFixer.ClothData);
					clothData.ClothComponent = weakGameEntity.GetComponentAtIndex(i, 3) as ClothSimulatorComponent;
					MatrixFrame matrixFrame = weakGameEntity.GetGlobalFrame();
					clothData.ShipLocalFrame = globalFrame.TransformToLocal(ref matrixFrame);
					this._shipCloths.Add(clothData);
				}
				if (weakGameEntity.Skeleton != null)
				{
					int componentCount2 = weakGameEntity.Skeleton.GetComponentCount(3);
					for (int j = 0; j < componentCount2; j++)
					{
						ShipClothFixer.ClothData clothData2 = default(ShipClothFixer.ClothData);
						clothData2.ClothComponent = weakGameEntity.Skeleton.GetComponentAtIndex(3, j) as ClothSimulatorComponent;
						MatrixFrame matrixFrame = weakGameEntity.GetGlobalFrame();
						clothData2.ShipLocalFrame = globalFrame.TransformToLocal(ref matrixFrame);
						this._shipCloths.Add(clothData2);
					}
				}
			}
		}

		// Token: 0x06000CB6 RID: 3254 RVA: 0x000619B0 File Offset: 0x0005FBB0
		private void SetPrevFrameToCloth(ShipClothFixer.ClothData clothData)
		{
			Vec3 vec = Vec3.Zero;
			if (this._frameCounter > 2)
			{
				vec = (this._prevShipFrame.TransformToParent(ref clothData.ShipLocalFrame.origin) - this._prevPrevShipFrame.TransformToParent(ref clothData.ShipLocalFrame.origin)) / this._fixedDt;
			}
			clothData.ClothComponent.SetForcedVelocity(ref vec);
		}

		// Token: 0x04000798 RID: 1944
		private List<ShipClothFixer.ClothData> _shipCloths = new List<ShipClothFixer.ClothData>();

		// Token: 0x04000799 RID: 1945
		private MatrixFrame _prevPrevShipFrame = MatrixFrame.Identity;

		// Token: 0x0400079A RID: 1946
		private MatrixFrame _prevShipFrame = MatrixFrame.Identity;

		// Token: 0x0400079B RID: 1947
		private float _fixedDt;

		// Token: 0x0400079C RID: 1948
		private int _frameCounter;

		// Token: 0x0200022B RID: 555
		private struct ClothData
		{
			// Token: 0x04000F4A RID: 3914
			internal ClothSimulatorComponent ClothComponent;

			// Token: 0x04000F4B RID: 3915
			internal MatrixFrame ShipLocalFrame;
		}
	}
}
