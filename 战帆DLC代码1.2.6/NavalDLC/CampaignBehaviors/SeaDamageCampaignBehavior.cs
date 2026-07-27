using System;
using System.Linq;
using Helpers;
using NavalDLC.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace NavalDLC.CampaignBehaviors
{
	// Token: 0x02000174 RID: 372
	public class SeaDamageCampaignBehavior : CampaignBehaviorBase
	{
		// Token: 0x0600187D RID: 6269 RVA: 0x000A6D23 File Offset: 0x000A4F23
		public override void RegisterEvents()
		{
			CampaignEvents.HourlyTickPartyEvent.AddNonSerializedListener(this, new Action<MobileParty>(this.HourlyTickParty));
			CampaignEvents.TickEvent.AddNonSerializedListener(this, new Action<float>(this.Tick));
		}

		// Token: 0x0600187E RID: 6270 RVA: 0x000A6D54 File Offset: 0x000A4F54
		private void Tick(float dt)
		{
			if (SeaDamageCampaignBehavior.DebugSeaDamage)
			{
				foreach (MobileParty mobileParty in MobileParty.All)
				{
					if (mobileParty.IsVisible && mobileParty.CurrentSettlement == null && mobileParty.IsCurrentlyAtSea && mobileParty.Ships.Any<Ship>())
					{
						Ship ship = mobileParty.Ships[0];
						float maxHitPoints = ship.MaxHitPoints;
						float hitPoints = ship.HitPoints;
						(mobileParty.Position.AsVec3() + Vec3.Up * 3.75f).x -= 1f;
						int num = 0;
						float num2 = (float)Campaign.Current.Models.CampaignShipDamageModel.GetHourlyShipDamage(mobileParty, ship);
						if (num2 > 0f)
						{
							num = (int)(ship.HitPoints / ship.MaxHitPoints / num2);
						}
						TerrainType faceGroupIndex = mobileParty.CurrentNavigationFace.FaceGroupIndex;
						string text = faceGroupIndex.ToString();
						string text2 = string.Format("Max Hitpoints: {0}\nHitpoints: {1}\nSeaworthiness: {2}\nTerrain: {3}\nEffected by: {4}", new object[]
						{
							maxHitPoints,
							hitPoints,
							ship.SeaWorthiness,
							text,
							Campaign.Current.Models.MapWeatherModel.GetWeatherEventInPosition(mobileParty.Position.ToVec2()).ToString()
						});
						if (num > 0)
						{
							text2 += string.Format("\nEstimated Hours: {0}", num);
						}
						else
						{
							text2 += "\nEstimated Hours: N/A";
						}
					}
				}
			}
		}

		// Token: 0x0600187F RID: 6271 RVA: 0x000A6F30 File Offset: 0x000A5130
		private void HourlyTickParty(MobileParty party)
		{
			if (party.IsActive && party.IsCurrentlyAtSea && !party.IsInRaftState && party.MapEvent == null)
			{
				for (int i = party.Ships.Count - 1; i >= 0; i--)
				{
					float num = (float)Campaign.Current.Models.CampaignShipDamageModel.GetHourlyShipDamage(party, party.Ships[i]);
					if (num > 0f)
					{
						float num2;
						party.Ships[i].OnShipDamaged(num, null, ref num2);
					}
				}
				if (party.HasPerk(NavalPerks.Shipmaster.MasterAndCommander, false))
				{
					SeaDamageCampaignBehavior.AddXpToTroops(party, MathF.Round(NavalPerks.Shipmaster.MasterAndCommander.PrimaryBonus));
				}
			}
		}

		// Token: 0x06001880 RID: 6272 RVA: 0x000A6FE4 File Offset: 0x000A51E4
		private static void AddXpToTroops(MobileParty party, int amount)
		{
			TroopRoster memberRoster = party.MemberRoster;
			for (int i = 0; i < memberRoster.Count; i++)
			{
				TroopRosterElement elementCopyAtIndex = memberRoster.GetElementCopyAtIndex(i);
				int num;
				if (!elementCopyAtIndex.Character.IsHero && MobilePartyHelper.CanTroopGainXp(party.Party, elementCopyAtIndex.Character, ref num))
				{
					int num2 = Math.Min(num, amount);
					memberRoster.AddXpToTroopAtIndex(i, num2);
				}
			}
		}

		// Token: 0x06001881 RID: 6273 RVA: 0x000A7045 File Offset: 0x000A5245
		public override void SyncData(IDataStore dataStore)
		{
		}

		// Token: 0x04000C02 RID: 3074
		public static bool DebugSeaDamage;
	}
}
