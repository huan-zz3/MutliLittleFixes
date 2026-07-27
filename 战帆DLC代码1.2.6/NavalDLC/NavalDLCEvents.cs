using System;
using NavalDLC.Map;
using NavalDLC.Storyline;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;

namespace NavalDLC
{
	// Token: 0x0200001D RID: 29
	public class NavalDLCEvents : CampaignEventReceiver
	{
		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000125 RID: 293 RVA: 0x00008B08 File Offset: 0x00006D08
		public static NavalDLCEvents Instance
		{
			get
			{
				return NavalDLCManager.Instance.NavalDLCEvents;
			}
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00008B14 File Offset: 0x00006D14
		public override void RemoveListeners(object obj)
		{
			this._onNavalStorylineActivityChangedEvent.ClearListeners(obj);
			this._isPartyQuestPartyEvent.ClearListeners(obj);
			this._onGunnarSavedEvent.ClearListeners(obj);
			this._onNavalStorylineCanceledEvent.ClearListeners(obj);
			this._onStormCreatedEvent.ClearListeners(obj);
			this._onSisterRansomedEvent.ClearListeners(obj);
			this._onSisterRansomRequestedEvent.ClearListeners(obj);
			this._onNavalStorylineTutorialSkippedEvent.ClearListeners(obj);
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000127 RID: 295 RVA: 0x00008B81 File Offset: 0x00006D81
		public static IMbEvent<PartyBase, NavalStorylinePartyData> IsNavalQuestPartyEvent
		{
			get
			{
				return NavalDLCEvents.Instance._isPartyQuestPartyEvent;
			}
		}

		// Token: 0x06000128 RID: 296 RVA: 0x00008B8D File Offset: 0x00006D8D
		public void IsNavalQuestParty(PartyBase party, NavalStorylinePartyData result)
		{
			NavalDLCEvents.Instance._isPartyQuestPartyEvent.Invoke(party, result);
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000129 RID: 297 RVA: 0x00008BA0 File Offset: 0x00006DA0
		public static IMbEvent<bool> OnNavalStorylineActivityChangedEvent
		{
			get
			{
				return NavalDLCEvents.Instance._onNavalStorylineActivityChangedEvent;
			}
		}

		// Token: 0x0600012A RID: 298 RVA: 0x00008BAC File Offset: 0x00006DAC
		public void OnNavalStorylineActivityChanged(bool activity)
		{
			NavalDLCEvents.Instance._onNavalStorylineActivityChangedEvent.Invoke(activity);
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x0600012B RID: 299 RVA: 0x00008BBE File Offset: 0x00006DBE
		public static IMbEvent OnSisterRansomedEvent
		{
			get
			{
				return NavalDLCEvents.Instance._onSisterRansomedEvent;
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x0600012C RID: 300 RVA: 0x00008BCA File Offset: 0x00006DCA
		public static IMbEvent OnGunnarSavedEvent
		{
			get
			{
				return NavalDLCEvents.Instance._onGunnarSavedEvent;
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600012D RID: 301 RVA: 0x00008BD6 File Offset: 0x00006DD6
		public static IMbEvent OnSisterRansomRequestedEvent
		{
			get
			{
				return NavalDLCEvents.Instance._onSisterRansomRequestedEvent;
			}
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00008BE2 File Offset: 0x00006DE2
		public void OnSisterRansomRequested()
		{
			NavalDLCEvents.Instance._onSisterRansomRequestedEvent.Invoke();
		}

		// Token: 0x0600012F RID: 303 RVA: 0x00008BF3 File Offset: 0x00006DF3
		public void OnSisterRansomed()
		{
			NavalDLCEvents.Instance._onSisterRansomedEvent.Invoke();
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00008C04 File Offset: 0x00006E04
		public void OnGunnarSaved()
		{
			NavalDLCEvents.Instance._onGunnarSavedEvent.Invoke();
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000131 RID: 305 RVA: 0x00008C15 File Offset: 0x00006E15
		public static IMbEvent<NavalStorylineData.StorylineCancelDetail> OnNavalStorylineCanceledEvent
		{
			get
			{
				return NavalDLCEvents.Instance._onNavalStorylineCanceledEvent;
			}
		}

		// Token: 0x06000132 RID: 306 RVA: 0x00008C21 File Offset: 0x00006E21
		public void OnNavalStorylineCanceled(NavalStorylineData.StorylineCancelDetail detail)
		{
			NavalDLCEvents.Instance._onNavalStorylineCanceledEvent.Invoke(detail);
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000133 RID: 307 RVA: 0x00008C33 File Offset: 0x00006E33
		public static IMbEvent OnNavalStorylineTutorialSkippedEvent
		{
			get
			{
				return NavalDLCEvents.Instance._onNavalStorylineTutorialSkippedEvent;
			}
		}

		// Token: 0x06000134 RID: 308 RVA: 0x00008C3F File Offset: 0x00006E3F
		public void OnNavalStorylineTutorialSkipped()
		{
			NavalDLCEvents.Instance._onNavalStorylineTutorialSkippedEvent.Invoke();
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000135 RID: 309 RVA: 0x00008C50 File Offset: 0x00006E50
		public static IMbEvent<Storm> OnStormCreatedEvent
		{
			get
			{
				return NavalDLCEvents.Instance._onStormCreatedEvent;
			}
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00008C5C File Offset: 0x00006E5C
		public void OnStormCreated(Storm storm)
		{
			NavalDLCEvents.Instance._onStormCreatedEvent.Invoke(storm);
		}

		// Token: 0x0400008A RID: 138
		private readonly MbEvent<PartyBase, NavalStorylinePartyData> _isPartyQuestPartyEvent = new MbEvent<PartyBase, NavalStorylinePartyData>();

		// Token: 0x0400008B RID: 139
		private readonly MbEvent<bool> _onNavalStorylineActivityChangedEvent = new MbEvent<bool>();

		// Token: 0x0400008C RID: 140
		private readonly MbEvent _onGunnarSavedEvent = new MbEvent();

		// Token: 0x0400008D RID: 141
		private readonly MbEvent _onSisterRansomRequestedEvent = new MbEvent();

		// Token: 0x0400008E RID: 142
		private readonly MbEvent _onSisterRansomedEvent = new MbEvent();

		// Token: 0x0400008F RID: 143
		private readonly MbEvent<NavalStorylineData.StorylineCancelDetail> _onNavalStorylineCanceledEvent = new MbEvent<NavalStorylineData.StorylineCancelDetail>();

		// Token: 0x04000090 RID: 144
		private readonly MbEvent _onNavalStorylineTutorialSkippedEvent = new MbEvent();

		// Token: 0x04000091 RID: 145
		private readonly MbEvent<Storm> _onStormCreatedEvent = new MbEvent<Storm>();
	}
}
