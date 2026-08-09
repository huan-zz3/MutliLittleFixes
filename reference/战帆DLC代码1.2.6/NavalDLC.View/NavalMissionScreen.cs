using System;
using NavalDLC.HotKeyCategories;
using NavalDLC.Missions.MissionLogics;
using NavalDLC.Missions.Objects;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.Screens;

namespace NavalDLC.View
{
	// Token: 0x02000009 RID: 9
	[GameStateScreen(typeof(NavalMissionState))]
	internal class NavalMissionScreen : MissionScreen
	{
		// Token: 0x06000048 RID: 72 RVA: 0x000035FD File Offset: 0x000017FD
		public NavalMissionScreen(MissionState missionState)
			: base(missionState)
		{
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00003606 File Offset: 0x00001806
		protected override void InitializeMissionView()
		{
			base.InitializeMissionView();
			SceneLayer sceneLayer = base.FindLayer<SceneLayer>();
			if (sceneLayer != null)
			{
				sceneLayer.Input.RegisterHotKeyCategory(new NavalCheatsHotKeyCategory());
			}
			this._navalShipsLogic = base.Mission.GetMissionBehavior<NavalShipsLogic>();
		}

		// Token: 0x0600004A RID: 74 RVA: 0x0000363A File Offset: 0x0000183A
		protected override bool CanViewCharacter()
		{
			return this._navalShipsLogic == null || this._navalShipsLogic.PlayerControlledShip == null;
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00003654 File Offset: 0x00001854
		protected override bool CanToggleCamera()
		{
			NavalShipsLogic navalShipsLogic = this._navalShipsLogic;
			return ((navalShipsLogic != null) ? navalShipsLogic.PlayerControlledShip : null) == null && base.CanToggleCamera();
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00003674 File Offset: 0x00001874
		public override void TeleportMainAgentToCameraFocusForCheat()
		{
			NavalShipsLogic missionBehavior = Mission.Current.GetMissionBehavior<NavalShipsLogic>();
			MissionShip missionShip = ((missionBehavior != null) ? missionBehavior.PlayerControlledShip : null);
			if (missionShip != null)
			{
				MatrixFrame globalFrame = missionShip.GlobalFrame;
				MatrixFrame lastFinalRenderCameraFrame = base.Mission.Scene.LastFinalRenderCameraFrame;
				float num = globalFrame.origin.Z - lastFinalRenderCameraFrame.origin.Z;
				Vec3 vec = -lastFinalRenderCameraFrame.rotation.u;
				float num2 = num / vec.Z;
				Vec3 f = lastFinalRenderCameraFrame.rotation.f;
				f.z = 0f;
				f.Normalize();
				if (num2 <= 400f)
				{
					vec *= num2;
					globalFrame.origin = lastFinalRenderCameraFrame.origin + vec;
					globalFrame.origin = new Vec3(globalFrame.origin.AsVec2, Mission.Current.Scene.GetWaterLevelAtPosition(globalFrame.origin.AsVec2, true, false), -1f);
					globalFrame.rotation = Mat3.CreateMat3WithForward(ref f);
					missionBehavior.TeleportShip(missionShip, globalFrame, false, false, true);
					return;
				}
			}
			else
			{
				base.TeleportMainAgentToCameraFocusForCheat();
			}
		}

		// Token: 0x0400001B RID: 27
		private NavalShipsLogic _navalShipsLogic;
	}
}
