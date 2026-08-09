using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;

namespace NavalDLC.Storyline.Quests
{
	// Token: 0x0200003A RID: 58
	public abstract class NavalStorylineQuestBase : QuestBase
	{
		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x0600040C RID: 1036 RVA: 0x0001D032 File Offset: 0x0001B232
		public sealed override bool IsRemainingTimeHidden
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x0600040D RID: 1037 RVA: 0x0001D035 File Offset: 0x0001B235
		public override string SpecialQuestType
		{
			get
			{
				return "NavalStoryline";
			}
		}

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x0600040E RID: 1038
		public abstract NavalStorylineData.NavalStorylineStage Stage { get; }

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x0600040F RID: 1039
		public abstract bool WillProgressStoryline { get; }

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000410 RID: 1040
		protected abstract string MainPartyTemplateStringId { get; }

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000411 RID: 1041 RVA: 0x0001D03C File Offset: 0x0001B23C
		public PartyTemplateObject Template
		{
			get
			{
				if (string.IsNullOrEmpty(this.MainPartyTemplateStringId))
				{
					return null;
				}
				return Campaign.Current.ObjectManager.GetObject<PartyTemplateObject>(this.MainPartyTemplateStringId);
			}
		}

		// Token: 0x06000412 RID: 1042 RVA: 0x0001D062 File Offset: 0x0001B262
		protected NavalStorylineQuestBase(string questId, Hero questGiver, CampaignTime duration, int rewardGold)
			: base(questId, questGiver, duration, rewardGold)
		{
		}

		// Token: 0x06000413 RID: 1043 RVA: 0x0001D06F File Offset: 0x0001B26F
		protected sealed override void RegisterEvents()
		{
			NavalDLCEvents.OnNavalStorylineActivityChangedEvent.AddNonSerializedListener(this, new Action<bool>(this.OnNavalStorylineActivityChanged));
			NavalDLCEvents.IsNavalQuestPartyEvent.AddNonSerializedListener(this, new Action<PartyBase, NavalStorylinePartyData>(this.IsNavalQuestParty));
			this.RegisterEventsInternal();
		}

		// Token: 0x06000414 RID: 1044 RVA: 0x0001D0A8 File Offset: 0x0001B2A8
		private void IsNavalQuestParty(PartyBase partyBase, NavalStorylinePartyData data)
		{
			if (partyBase == PartyBase.MainParty)
			{
				data.IsQuestParty = true;
				data.Template = this.Template;
				if (data.Template != null)
				{
					data.PartySize = (int)NavalDLCHelpers.GetMaxPartySizeLimitFromTemplate(data.Template).ResultNumber + 2;
				}
			}
			this.IsNavalQuestPartyInternal(partyBase, data);
		}

		// Token: 0x06000415 RID: 1045 RVA: 0x0001D0FC File Offset: 0x0001B2FC
		protected virtual void IsNavalQuestPartyInternal(PartyBase partyBase, NavalStorylinePartyData data)
		{
		}

		// Token: 0x06000416 RID: 1046 RVA: 0x0001D0FE File Offset: 0x0001B2FE
		private void OnNavalStorylineActivityChanged(bool activity)
		{
			if (base.IsOngoing && !activity)
			{
				this.ResetQuest();
			}
			this.OnNavalStorylineActivityChangedInternal(activity);
		}

		// Token: 0x06000417 RID: 1047 RVA: 0x0001D118 File Offset: 0x0001B318
		protected virtual void OnNavalStorylineActivityChangedInternal(bool activity)
		{
		}

		// Token: 0x06000418 RID: 1048
		protected abstract void RegisterEventsInternal();

		// Token: 0x06000419 RID: 1049 RVA: 0x0001D11A File Offset: 0x0001B31A
		public void ResetQuest()
		{
			base.CompleteQuestWithCancel(null);
		}

		// Token: 0x0600041A RID: 1050 RVA: 0x0001D123 File Offset: 0x0001B323
		protected sealed override void OnStartQuest()
		{
			if (this.WillProgressStoryline)
			{
				NavalStorylineData.OnStorylineProgress(this);
			}
			this.OnStartQuestInternal();
		}

		// Token: 0x0600041B RID: 1051 RVA: 0x0001D139 File Offset: 0x0001B339
		protected sealed override void OnFinalize()
		{
			this.OnFinalizeInternal();
		}

		// Token: 0x0600041C RID: 1052 RVA: 0x0001D141 File Offset: 0x0001B341
		protected sealed override void InitializeQuestOnGameLoad()
		{
			this.InitializeQuestOnGameLoadInternal();
		}

		// Token: 0x0600041D RID: 1053 RVA: 0x0001D149 File Offset: 0x0001B349
		protected virtual void InitializeQuestOnGameLoadInternal()
		{
		}

		// Token: 0x0600041E RID: 1054 RVA: 0x0001D14B File Offset: 0x0001B34B
		protected virtual void OnStartQuestInternal()
		{
		}

		// Token: 0x0600041F RID: 1055 RVA: 0x0001D14D File Offset: 0x0001B34D
		protected virtual void OnFinalizeInternal()
		{
		}

		// Token: 0x06000420 RID: 1056 RVA: 0x0001D14F File Offset: 0x0001B34F
		public sealed override void OnCanceled()
		{
			this.OnCanceledInternal();
		}

		// Token: 0x06000421 RID: 1057 RVA: 0x0001D157 File Offset: 0x0001B357
		protected virtual void OnCanceledInternal()
		{
		}

		// Token: 0x06000422 RID: 1058 RVA: 0x0001D159 File Offset: 0x0001B359
		public sealed override void OnFailed()
		{
			this.OnFailedInternal();
		}

		// Token: 0x06000423 RID: 1059 RVA: 0x0001D161 File Offset: 0x0001B361
		protected virtual void OnFailedInternal()
		{
		}

		// Token: 0x06000424 RID: 1060 RVA: 0x0001D163 File Offset: 0x0001B363
		protected sealed override void OnCompleteWithSuccess()
		{
			this.OnCompleteWithSuccessInternal();
		}

		// Token: 0x06000425 RID: 1061 RVA: 0x0001D16B File Offset: 0x0001B36B
		protected virtual void OnCompleteWithSuccessInternal()
		{
		}
	}
}
