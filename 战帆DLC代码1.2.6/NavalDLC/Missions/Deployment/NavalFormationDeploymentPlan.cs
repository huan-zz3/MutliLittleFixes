using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace NavalDLC.Missions.Deployment
{
	// Token: 0x020000DD RID: 221
	public class NavalFormationDeploymentPlan : IFormationDeploymentPlan
	{
		// Token: 0x17000307 RID: 775
		// (get) Token: 0x0600114F RID: 4431 RVA: 0x000808AB File Offset: 0x0007EAAB
		public FormationClass Class
		{
			get
			{
				return this._class;
			}
		}

		// Token: 0x17000308 RID: 776
		// (get) Token: 0x06001150 RID: 4432 RVA: 0x000808B3 File Offset: 0x0007EAB3
		public FormationClass SpawnClass
		{
			get
			{
				return this._class;
			}
		}

		// Token: 0x17000309 RID: 777
		// (get) Token: 0x06001151 RID: 4433 RVA: 0x000808BC File Offset: 0x0007EABC
		public float PlannedWidth
		{
			get
			{
				if (this.ShipObject == null)
				{
					return 0f;
				}
				return this.ShipObject.DeploymentArea.X;
			}
		}

		// Token: 0x1700030A RID: 778
		// (get) Token: 0x06001152 RID: 4434 RVA: 0x000808EC File Offset: 0x0007EAEC
		public float PlannedDepth
		{
			get
			{
				if (this.ShipObject == null)
				{
					return 0f;
				}
				return this.ShipObject.DeploymentArea.Y;
			}
		}

		// Token: 0x1700030B RID: 779
		// (get) Token: 0x06001153 RID: 4435 RVA: 0x0008091A File Offset: 0x0007EB1A
		public int PlannedTroopCount
		{
			get
			{
				if (this.ShipObject == null)
				{
					return 0;
				}
				return this.ShipOrigin.TotalCrewCapacity;
			}
		}

		// Token: 0x1700030C RID: 780
		// (get) Token: 0x06001154 RID: 4436 RVA: 0x00080931 File Offset: 0x0007EB31
		public bool HasDimensions
		{
			get
			{
				return this.PlannedWidth >= 1E-05f && this.PlannedDepth >= 1E-05f;
			}
		}

		// Token: 0x1700030D RID: 781
		// (get) Token: 0x06001155 RID: 4437 RVA: 0x00080952 File Offset: 0x0007EB52
		public bool HasShipObject
		{
			get
			{
				return this.ShipObject != null;
			}
		}

		// Token: 0x1700030E RID: 782
		// (get) Token: 0x06001156 RID: 4438 RVA: 0x0008095D File Offset: 0x0007EB5D
		// (set) Token: 0x06001157 RID: 4439 RVA: 0x00080965 File Offset: 0x0007EB65
		public IShipOrigin ShipOrigin { get; private set; }

		// Token: 0x1700030F RID: 783
		// (get) Token: 0x06001158 RID: 4440 RVA: 0x0008096E File Offset: 0x0007EB6E
		// (set) Token: 0x06001159 RID: 4441 RVA: 0x00080976 File Offset: 0x0007EB76
		public MissionShipObject ShipObject { get; private set; }

		// Token: 0x0600115A RID: 4442 RVA: 0x0008097F File Offset: 0x0007EB7F
		public NavalFormationDeploymentPlan(FormationClass fClass, Mission mission)
		{
			this._class = fClass;
			this.Clear();
			this._hasFrame = false;
			this.ShipOrigin = null;
			this.ShipObject = null;
			this._mission = mission;
		}

		// Token: 0x0600115B RID: 4443 RVA: 0x000809B0 File Offset: 0x0007EBB0
		public bool HasFrame()
		{
			return this._hasFrame;
		}

		// Token: 0x0600115C RID: 4444 RVA: 0x000809B8 File Offset: 0x0007EBB8
		public FormationDeploymentFlank GetDefaultFlank()
		{
			if (!this.HasShipObject)
			{
			}
			switch (this._class)
			{
			case 1:
			case 8:
			case 9:
			case 10:
				return 3;
			case 2:
			case 7:
				return 1;
			case 3:
			case 6:
				return 2;
			}
			return 0;
		}

		// Token: 0x0600115D RID: 4445 RVA: 0x00080A19 File Offset: 0x0007EC19
		public MatrixFrame GetFrame()
		{
			this.UpdateFrameZ();
			return this._spawnFrame;
		}

		// Token: 0x0600115E RID: 4446 RVA: 0x00080A27 File Offset: 0x0007EC27
		public Vec3 GetPosition()
		{
			this.UpdateFrameZ();
			return this._spawnFrame.origin;
		}

		// Token: 0x0600115F RID: 4447 RVA: 0x00080A3A File Offset: 0x0007EC3A
		public Vec2 GetDirection()
		{
			return this._spawnFrame.rotation.f.AsVec2;
		}

		// Token: 0x06001160 RID: 4448 RVA: 0x00080A51 File Offset: 0x0007EC51
		public WorldPosition CreateNewDeploymentWorldPosition(WorldPosition.WorldPositionEnforcedCache worldPositionEnforcedCache)
		{
			return WorldPosition.Invalid;
		}

		// Token: 0x06001161 RID: 4449 RVA: 0x00080A58 File Offset: 0x0007EC58
		public void Clear()
		{
			this._spawnFrame = MatrixFrame.Identity;
			this._hasFrame = false;
			this.ShipOrigin = null;
			this.ShipObject = null;
		}

		// Token: 0x06001162 RID: 4450 RVA: 0x00080A7A File Offset: 0x0007EC7A
		public void SetShipOrigin(IShipOrigin shipOrigin)
		{
			if (shipOrigin != null)
			{
				this.ShipOrigin = shipOrigin;
			}
			else
			{
				this.ShipOrigin = null;
			}
			if (this.ShipOrigin != null)
			{
				this.ShipObject = MBObjectManager.Instance.GetObject<MissionShipObject>(this.ShipOrigin.OriginShipId);
				return;
			}
			this.ShipObject = null;
		}

		// Token: 0x06001163 RID: 4451 RVA: 0x00080ABC File Offset: 0x0007ECBC
		public void SetFrame(in Vec2 deployPosition, in Vec2 deployDirection)
		{
			Vec2 vec = deployDirection;
			Vec3 vec2 = vec.ToVec3(0f);
			Mat3 mat = Mat3.CreateMat3WithForward(ref vec2);
			vec = deployPosition;
			vec2 = vec.ToVec3(0f);
			this._spawnFrame = new MatrixFrame(ref mat, ref vec2);
			this.UpdateFrameZ();
			this._hasFrame = true;
		}

		// Token: 0x06001164 RID: 4452 RVA: 0x00080B15 File Offset: 0x0007ED15
		private void UpdateFrameZ()
		{
			this._spawnFrame.origin.z = this._mission.Scene.GetWaterLevelAtPosition(this._spawnFrame.origin.AsVec2, true, false);
		}

		// Token: 0x04000A04 RID: 2564
		private MatrixFrame _spawnFrame;

		// Token: 0x04000A05 RID: 2565
		private readonly FormationClass _class;

		// Token: 0x04000A06 RID: 2566
		private bool _hasFrame;

		// Token: 0x04000A07 RID: 2567
		private Mission _mission;
	}
}
