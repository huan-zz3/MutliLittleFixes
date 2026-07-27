using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace NavalDLC.Settlements
{
	// Token: 0x0200007C RID: 124
	public class NavalVillageTypes
	{
		// Token: 0x17000170 RID: 368
		// (get) Token: 0x060008E2 RID: 2274 RVA: 0x0003EB2F File Offset: 0x0003CD2F
		private static NavalVillageTypes Instance
		{
			get
			{
				return NavalDLCManager.Instance.NavalVillageTypes;
			}
		}

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x060008E3 RID: 2275 RVA: 0x0003EB3B File Offset: 0x0003CD3B
		public static VillageType WalrusHunter
		{
			get
			{
				return NavalVillageTypes.Instance.VillageTypeWalrusHunter;
			}
		}

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x060008E4 RID: 2276 RVA: 0x0003EB47 File Offset: 0x0003CD47
		public static VillageType Whaler
		{
			get
			{
				return NavalVillageTypes.Instance.VillageTypeWhaler;
			}
		}

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x060008E5 RID: 2277 RVA: 0x0003EB53 File Offset: 0x0003CD53
		// (set) Token: 0x060008E6 RID: 2278 RVA: 0x0003EB5B File Offset: 0x0003CD5B
		internal VillageType VillageTypeWalrusHunter { get; private set; }

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x060008E7 RID: 2279 RVA: 0x0003EB64 File Offset: 0x0003CD64
		// (set) Token: 0x060008E8 RID: 2280 RVA: 0x0003EB6C File Offset: 0x0003CD6C
		internal VillageType VillageTypeWhaler { get; private set; }

		// Token: 0x060008E9 RID: 2281 RVA: 0x0003EB75 File Offset: 0x0003CD75
		public NavalVillageTypes()
		{
			this.RegisterAll();
			this.InitializeAll();
			this.AddProductions();
		}

		// Token: 0x060008EA RID: 2282 RVA: 0x0003EB8F File Offset: 0x0003CD8F
		private VillageType Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<VillageType>(new VillageType(stringId));
		}

		// Token: 0x060008EB RID: 2283 RVA: 0x0003EBA6 File Offset: 0x0003CDA6
		private void RegisterAll()
		{
			this.VillageTypeWalrusHunter = this.Create("walrus_hunter");
			this.VillageTypeWhaler = this.Create("whaler");
		}

		// Token: 0x060008EC RID: 2284 RVA: 0x0003EBCA File Offset: 0x0003CDCA
		private ItemObject GetItemObject(string objectId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<ItemObject>(new ItemObject(objectId));
		}

		// Token: 0x060008ED RID: 2285 RVA: 0x0003EBE4 File Offset: 0x0003CDE4
		private void InitializeAll()
		{
			this.VillageTypeWalrusHunter.Initialize(new TextObject("{=Eg7KEtGg}Walrus Tusk Hunters", null), "kitchen_horn", "fisherman_ucon", "fisherman_burned", new ValueTuple<ItemObject, float>[]
			{
				new ValueTuple<ItemObject, float>(this.GetItemObject("fish"), 5f)
			});
			this.VillageTypeWhaler.Initialize(new TextObject("{=QdCFs5tT}Whalers", null), "bd_barrel_a", "fisherman_ucon", "fisherman_burned", new ValueTuple<ItemObject, float>[]
			{
				new ValueTuple<ItemObject, float>(this.GetItemObject("fish"), 5f)
			});
		}

		// Token: 0x060008EE RID: 2286 RVA: 0x0003EC84 File Offset: 0x0003CE84
		private void AddProductions()
		{
			this.VillageTypeWalrusHunter.AddProductions(new ValueTuple<ItemObject, float>[]
			{
				new ValueTuple<ItemObject, float>(this.GetItemObject("walrus_tusk"), 1.4f)
			});
			this.VillageTypeWhaler.AddProductions(new ValueTuple<ItemObject, float>[]
			{
				new ValueTuple<ItemObject, float>(this.GetItemObject("whale_oil"), 1.8f)
			});
		}
	}
}
