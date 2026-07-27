using System;
using System.Linq;
using NavalDLC.Storyline;
using SandBox.Conversation.MissionLogics;
using StoryMode.StoryModeObjects;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.AgentOrigins;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace NavalDLC.View.MissionViews
{
	// Token: 0x0200001C RID: 28
	public class NavalFinalConversationMissionView : MissionView
	{
		// Token: 0x060000C5 RID: 197 RVA: 0x00006C2C File Offset: 0x00004E2C
		public override void OnMissionTick(float dt)
		{
			this._currentConversationCharacter = Campaign.Current.ConversationManager.OneToOneConversationCharacter;
			if (this._shouldStartSisterConversation && !ScreenFadeController.IsFadeActive)
			{
				Agent agent = Mission.Current.Agents.FirstOrDefault<Agent>((Agent x) => x.Character == StoryModeHeroes.LittleSister.CharacterObject);
				MissionConversationLogic missionBehavior = Mission.Current.GetMissionBehavior<MissionConversationLogic>();
				if (missionBehavior != null)
				{
					missionBehavior.StartConversation(agent, false, false);
				}
				this._shouldStartSisterConversation = false;
			}
			if (this._shouldSpawnSister && this._remainingSisterSpawnTime > 0f)
			{
				this._remainingSisterSpawnTime -= dt;
				if (this._remainingSisterSpawnTime <= 0f)
				{
					this.TransitionToSister();
					this._shouldSpawnSister = false;
				}
			}
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00006CE8 File Offset: 0x00004EE8
		public override void OnConversationEnd()
		{
			if (this._currentConversationCharacter == NavalStorylineData.Gunnar.CharacterObject)
			{
				ScreenFadeController.BeginFadeOutAndIn(0.5f, 0.5f, 0.5f);
				this._shouldSpawnSister = true;
			}
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00006D18 File Offset: 0x00004F18
		private void TransitionToSister()
		{
			AgentBuildData agentBuildData = new AgentBuildData(StoryModeHeroes.LittleSister.CharacterObject);
			agentBuildData.TroopOrigin(new SimpleAgentOrigin(agentBuildData.AgentCharacter, -1, null, default(UniqueTroopDescriptor)));
			Agent agent = Mission.Current.Agents.FirstOrDefault<Agent>((Agent x) => x.Character == NavalStorylineData.Gunnar.CharacterObject);
			Vec3 position = agent.Position;
			agentBuildData.InitialPosition(ref position);
			AgentBuildData agentBuildData2 = agentBuildData;
			Vec2 vec = Agent.Main.LookDirection.AsVec2;
			vec = -vec.Normalized();
			agentBuildData2.InitialDirection(ref vec);
			agentBuildData.NoHorses(true);
			agentBuildData.CivilianEquipment(true);
			Mission.Current.SpawnAgent(agentBuildData, false);
			agent.FadeOut(true, true);
			this._shouldStartSisterConversation = true;
		}

		// Token: 0x04000045 RID: 69
		private const float FadeDuration = 0.5f;

		// Token: 0x04000046 RID: 70
		private CharacterObject _currentConversationCharacter;

		// Token: 0x04000047 RID: 71
		private float _remainingSisterSpawnTime = 0.6f;

		// Token: 0x04000048 RID: 72
		private bool _shouldSpawnSister;

		// Token: 0x04000049 RID: 73
		private bool _shouldStartSisterConversation;
	}
}
