using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace NavalDLC
{
	// Token: 0x02000017 RID: 23
	public class FishingPartyComponent : VillagerPartyComponent
	{
		// Token: 0x060000FA RID: 250 RVA: 0x00007FBE File Offset: 0x000061BE
		public static MobileParty CreateFishingParty(string stringId, Village village)
		{
			return MobileParty.CreateParty(stringId, new FishingPartyComponent(village));
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000FB RID: 251 RVA: 0x00007FCC File Offset: 0x000061CC
		// (set) Token: 0x060000FC RID: 252 RVA: 0x00007FD4 File Offset: 0x000061D4
		public bool IsFishing
		{
			get
			{
				return this._isFishing;
			}
			set
			{
				if (this._isFishing != value)
				{
					this._isFishing = value;
					if (this._isFishing)
					{
						this.FishingWaitStartTime = CampaignTime.Now;
						return;
					}
					this.FishingWaitStartTime = CampaignTime.Never;
				}
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000FD RID: 253 RVA: 0x00008005 File Offset: 0x00006205
		// (set) Token: 0x060000FE RID: 254 RVA: 0x0000800D File Offset: 0x0000620D
		public bool IsRoaming
		{
			get
			{
				return this._isRoaming;
			}
			set
			{
				if (this._isRoaming != value)
				{
					this._isRoaming = value;
					if (this._isRoaming)
					{
						this.RoamingStartTime = CampaignTime.Now;
						return;
					}
					this.RoamingStartTime = CampaignTime.Never;
				}
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000FF RID: 255 RVA: 0x0000803E File Offset: 0x0000623E
		// (set) Token: 0x06000100 RID: 256 RVA: 0x00008046 File Offset: 0x00006246
		[SaveableProperty(3)]
		public CampaignTime FishingWaitStartTime { get; private set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000101 RID: 257 RVA: 0x0000804F File Offset: 0x0000624F
		// (set) Token: 0x06000102 RID: 258 RVA: 0x00008057 File Offset: 0x00006257
		[SaveableProperty(4)]
		public CampaignTime RoamingStartTime { get; private set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000103 RID: 259 RVA: 0x00008060 File Offset: 0x00006260
		public override TextObject Name
		{
			get
			{
				if (this._cachedName == null)
				{
					this._cachedName = new TextObject("{=a9TivyGv}Fishers of {VILLAGE_NAME}", null);
					this._cachedName.SetTextVariable("VILLAGE_NAME", base.Village.Name);
				}
				return this._cachedName;
			}
		}

		// Token: 0x06000104 RID: 260 RVA: 0x000080AE File Offset: 0x000062AE
		protected FishingPartyComponent(Village village)
			: base(village, null)
		{
		}

		// Token: 0x06000105 RID: 261 RVA: 0x000080B8 File Offset: 0x000062B8
		protected override void OnMobilePartySetOnCreation()
		{
			base.MobileParty.Aggressiveness = 0f;
			base.MobileParty.InitializePartyTrade(0);
			PartyTemplateObject fishingPartyTemplate = base.Village.Settlement.Culture.FishingPartyTemplate;
			CampaignVec2 portPosition = base.Village.Settlement.PortPosition;
			base.MobileParty.InitializeMobilePartyAroundPosition(fishingPartyTemplate, portPosition, 1f, 0f);
			base.Party.SetVisualAsDirty();
			base.MobileParty.SetLandNavigationAccess(false);
		}

		// Token: 0x06000106 RID: 262 RVA: 0x00008138 File Offset: 0x00006338
		protected override void OnInitialize()
		{
			List<FishingPartyComponent> list;
			if (!NavalDLCManager.Instance.FishingParties.TryGetValue(base.Village, out list))
			{
				list = new List<FishingPartyComponent>();
				NavalDLCManager.Instance.FishingParties.Add(base.Village, list);
			}
			list.Add(this);
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00008184 File Offset: 0x00006384
		protected override void OnFinalize()
		{
			List<FishingPartyComponent> list;
			if (NavalDLCManager.Instance.FishingParties.TryGetValue(base.Village, out list))
			{
				list.Remove(this);
				return;
			}
			Debug.FailedAssert("parties.Contains(fishingParty)", "C:\\BuildAgent\\work\\mb3\\Source\\Bannerlord\\NavalDLC\\FishingPartyComponent.cs", "OnFinalize", 136);
		}

		// Token: 0x0400007F RID: 127
		[SaveableField(1)]
		private bool _isFishing;

		// Token: 0x04000080 RID: 128
		[SaveableField(2)]
		private bool _isRoaming;
	}
}
