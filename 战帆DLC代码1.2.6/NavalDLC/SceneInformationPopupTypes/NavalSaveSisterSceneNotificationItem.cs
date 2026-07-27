using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace NavalDLC.SceneInformationPopupTypes
{
	// Token: 0x0200007E RID: 126
	public class NavalSaveSisterSceneNotificationItem : SceneNotificationData
	{
		// Token: 0x17000177 RID: 375
		// (get) Token: 0x060008F5 RID: 2293 RVA: 0x0003EDC9 File Offset: 0x0003CFC9
		// (set) Token: 0x060008F6 RID: 2294 RVA: 0x0003EDD1 File Offset: 0x0003CFD1
		public Hero MainHero { get; private set; }

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x060008F7 RID: 2295 RVA: 0x0003EDDA File Offset: 0x0003CFDA
		// (set) Token: 0x060008F8 RID: 2296 RVA: 0x0003EDE2 File Offset: 0x0003CFE2
		public Hero Sister { get; private set; }

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x060008F9 RID: 2297 RVA: 0x0003EDEB File Offset: 0x0003CFEB
		public override string SceneID
		{
			get
			{
				return "cutscene_saving_sister";
			}
		}

		// Token: 0x1700017A RID: 378
		// (get) Token: 0x060008FA RID: 2298 RVA: 0x0003EDF2 File Offset: 0x0003CFF2
		public override SceneNotificationData.RelevantContextType RelevantContext
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x060008FB RID: 2299 RVA: 0x0003EDF5 File Offset: 0x0003CFF5
		public override TextObject TitleText
		{
			get
			{
				return new TextObject("{=kpBuCL0h}The danger has passed. Your sister is now out of harm's way.", null);
			}
		}

		// Token: 0x060008FC RID: 2300 RVA: 0x0003EE02 File Offset: 0x0003D002
		public NavalSaveSisterSceneNotificationItem(Hero mainHero, Hero sister, Action onCloseAction)
		{
			this.MainHero = mainHero;
			this.Sister = sister;
			this._onCloseAction = onCloseAction;
		}

		// Token: 0x060008FD RID: 2301 RVA: 0x0003EE20 File Offset: 0x0003D020
		public override SceneNotificationData.SceneNotificationCharacter[] GetSceneNotificationCharacters()
		{
			new List<SceneNotificationData.SceneNotificationCharacter>();
			Equipment equipment = this.MainHero.BattleEquipment.Clone(false);
			CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment, true, false);
			Equipment equipment2 = this.Sister.BattleEquipment.Clone(false);
			CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment2, true, false);
			Equipment equipment3 = this.Sister.BattleEquipment.Clone(false);
			CampaignSceneNotificationHelper.RemoveWeaponsFromEquipment(ref equipment3, true, false);
			return new SceneNotificationData.SceneNotificationCharacter[]
			{
				CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(this.MainHero, equipment, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, false),
				CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(this.Sister, equipment2, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, false),
				CampaignSceneNotificationHelper.CreateNotificationCharacterFromHero(this.Sister, equipment3, false, default(BodyProperties), uint.MaxValue, uint.MaxValue, false)
			};
		}

		// Token: 0x060008FE RID: 2302 RVA: 0x0003EEEA File Offset: 0x0003D0EA
		public override void OnCloseAction()
		{
			base.OnCloseAction();
			Action onCloseAction = this._onCloseAction;
			if (onCloseAction == null)
			{
				return;
			}
			onCloseAction();
		}

		// Token: 0x0400053C RID: 1340
		private readonly Action _onCloseAction;
	}
}
