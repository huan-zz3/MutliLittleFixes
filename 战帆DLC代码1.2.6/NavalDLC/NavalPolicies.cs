using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace NavalDLC
{
	// Token: 0x02000024 RID: 36
	public class NavalPolicies
	{
		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000184 RID: 388 RVA: 0x00009E6F File Offset: 0x0000806F
		private static NavalPolicies Instance
		{
			get
			{
				return NavalDLCManager.Instance.NavalPolicies;
			}
		}

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000185 RID: 389 RVA: 0x00009E7B File Offset: 0x0000807B
		public static PolicyObject FraternalFleetDoctrine
		{
			get
			{
				return NavalPolicies.Instance._policyFraternalFleetDoctrine;
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000186 RID: 390 RVA: 0x00009E87 File Offset: 0x00008087
		public static PolicyObject KingsTitheOnKeels
		{
			get
			{
				return NavalPolicies.Instance._policyKingsTitheOnKeels;
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000187 RID: 391 RVA: 0x00009E93 File Offset: 0x00008093
		public static PolicyObject RoyalRansomClaim
		{
			get
			{
				return NavalPolicies.Instance._policyRoyalRansomClaim;
			}
		}

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000188 RID: 392 RVA: 0x00009E9F File Offset: 0x0000809F
		public static PolicyObject RoyalNavyPrerogative
		{
			get
			{
				return NavalPolicies.Instance._policyRoyalNavyPrerogative;
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000189 RID: 393 RVA: 0x00009EAB File Offset: 0x000080AB
		public static PolicyObject MaritimeWealEdict
		{
			get
			{
				return NavalPolicies.Instance._policyMaritimeWealEdict;
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x0600018A RID: 394 RVA: 0x00009EB7 File Offset: 0x000080B7
		public static PolicyObject KingsPardonForPirates
		{
			get
			{
				return NavalPolicies.Instance._policyKingsPardonForPirates;
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x0600018B RID: 395 RVA: 0x00009EC3 File Offset: 0x000080C3
		public static PolicyObject RaidersSpoils
		{
			get
			{
				return NavalPolicies.Instance._policyRaidersSpoils;
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x0600018C RID: 396 RVA: 0x00009ECF File Offset: 0x000080CF
		public static PolicyObject CoastalGuardEdict
		{
			get
			{
				return NavalPolicies.Instance._policyCoastalGuardEdict;
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x0600018D RID: 397 RVA: 0x00009EDB File Offset: 0x000080DB
		public static PolicyObject BolsterTheFyrd
		{
			get
			{
				return NavalPolicies.Instance._policyBolsterTheFyrd;
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x0600018E RID: 398 RVA: 0x00009EE7 File Offset: 0x000080E7
		public static PolicyObject NavalConjoiningStatute
		{
			get
			{
				return NavalPolicies.Instance._policyNavalConjoiningStatute;
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x0600018F RID: 399 RVA: 0x00009EF3 File Offset: 0x000080F3
		public static PolicyObject ArsenalDepositoryAct
		{
			get
			{
				return NavalPolicies.Instance._policyArsenalDepositoryAct;
			}
		}

		// Token: 0x06000190 RID: 400 RVA: 0x00009EFF File Offset: 0x000080FF
		public NavalPolicies()
		{
			this.RegisterAll();
			this.InitializeAll();
		}

		// Token: 0x06000191 RID: 401 RVA: 0x00009F14 File Offset: 0x00008114
		private void RegisterAll()
		{
			this._policyFraternalFleetDoctrine = NavalPolicies.Create("policy_fraternal_fleet_doctrine");
			this._policyKingsTitheOnKeels = NavalPolicies.Create("policy_kings_tithe_on_keels");
			this._policyRoyalRansomClaim = NavalPolicies.Create("policy_royal_ransom_claim");
			this._policyRoyalNavyPrerogative = NavalPolicies.Create("policy_royal_navy_prerogative");
			this._policyMaritimeWealEdict = NavalPolicies.Create("policy_maritime_weal_edict");
			this._policyKingsPardonForPirates = NavalPolicies.Create("policy_Kings_pardon_for_pirates");
			this._policyRaidersSpoils = NavalPolicies.Create("policy_raiders_spoils");
			this._policyCoastalGuardEdict = NavalPolicies.Create("policy_coastal_guard_edict");
			this._policyBolsterTheFyrd = NavalPolicies.Create("policy_bolster_the_fyrd");
			this._policyNavalConjoiningStatute = NavalPolicies.Create("policy_naval_conjoining_statute");
			this._policyArsenalDepositoryAct = NavalPolicies.Create("policy_arsenal_depository_act");
		}

		// Token: 0x06000192 RID: 402 RVA: 0x00009FD1 File Offset: 0x000081D1
		private static PolicyObject Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<PolicyObject>(new PolicyObject(stringId));
		}

		// Token: 0x06000193 RID: 403 RVA: 0x00009FE8 File Offset: 0x000081E8
		private void InitializeAll()
		{
			this._policyFraternalFleetDoctrine.Initialize(new TextObject("{=wNt5Bfkb}Auxiliaries to the Fleet", null), new TextObject("{=BhGsrGwR}Troops are required to spend part of their time training at sea, practicing shooting and fighting on an unstable deck.", null), new TextObject("{=vJcdw5Ht}requiring troops to train to fight at sea.", null), new TextObject("{=BaTU5lic}Naval combat morale of lord parties is increased by 20%{newline}Troop XP gain is reduced by 15%", null), -0.7f, 0.1f, -0.8f);
			this._policyKingsTitheOnKeels.Initialize(new TextObject("{=lydo4bTx}Tithe on Keels", null), new TextObject("{=5r88g5cS}The ruler is given a share of the revenue whenever one of the great houses of the realm sells a ship.", null), new TextObject("{=4wBzaO3j}allowing the ruler to collect a share of the revenue from the sales of ships.", null), new TextObject("{=UMacLoNj}The ruler's clan receives 15% of the revenue from ship sales of other clans.", null), 0.9f, -0.5f, -0.5f);
			this._policyRoyalRansomClaim.Initialize(new TextObject("{=nSgFcRRj}Royal Ransom Claim", null), new TextObject("{=oPHK8IQh}The ruler is granted a share of all ransoms collected by the lords of the realm.", null), new TextObject("{=akNKrmKX}granting the ruler a share of all ransoms.", null), new TextObject("{=JBN6C6Nb}The ruling clan collects a 15% commission on ransom payments to other clans.", null), 0.8f, -0.6f, -0.4f);
			this._policyRoyalNavyPrerogative.Initialize(new TextObject("{=njYCvNGT}Royal Navy Prerogative", null), new TextObject("{=RH9vo6xc}The ruler has the right to purchase the finest ship lumber and fittings from smiths and carpenters at a fixed price.", null), new TextObject("{=lgAVWSwJ}granting the ruler the right to purchase lumber and fittings at a discount.", null), new TextObject("{=2F0GnVTR}Purchase price and upgrade costs of the ruling clan's ships is decreased by 10%{newline}Wood and smithy workshop output is decreased by 5% kingdom-wide.", null), 0.5f, -0.6f, -0.6f);
			this._policyMaritimeWealEdict.Initialize(new TextObject("{=4aDWXjRX}Maritime Weal Edict", null), new TextObject("{=DZ3R3ROK}The edict requires inland towns to support coastal settlements with materials and trained workmen.", null), new TextObject("{=xcJON23P}issuing an edict granting special privileges to enterprises in coastal towns.", null), new TextObject("{=HzrCHG0N}Production in coastal settlement workshops and their bounded villages is increased by 25%{newline}Settlement project building speed in non-coastal towns is decreased by 20%", null), 0.1f, 0.5f, -0.4f);
			this._policyKingsPardonForPirates.Initialize(new TextObject("{=ITgQ4Le0}Amnesties for Pirates", null), new TextObject("{=ShbbtHrT}Local magistrates are authorized to issue pardons to pirates in the name of the ruler, bringing their members into the garrison. This will reduce the immediate threat to maritime trade, but could also undercut deterrence.", null), new TextObject("{=qWBxsOti}allowing officials to issue pardons to pirates.", null), new TextObject("{=amZ3pGbZ}Each day, a pirate party operating within the kingdom's coastline has a 5% chance to surrender to the nearest coastal town. Upon surrendering, its ships are donated to the town's shipyard, and some of its members join the town's garrison.{newline}For each surrendered pirate party that settlement's security is immediately decreased by 5.", null), 0.2f, 0.1f, -0.3f);
			this._policyRaidersSpoils.Initialize(new TextObject("{=1cLKUsq6}Writs of Reprisal", null), new TextObject("{=tprteUZD}Lords are issued commendations for successful raids on enemy territory, building support for long-ranging pillaging expeditions but also attracting a more lawless element to the armies.", null), new TextObject("{=32yg996m}encouraging lords to pillage enemy territory.", null), new TextObject("{=5JhZWebu}Successfully raiding a village grants the raiding clan +5 influence.{newline}For each lord party currently staying in a town owned by the kingdom, town security is reduced by 1 daily", null), -0.8f, -0.1f, 0.6f);
			this._policyCoastalGuardEdict.Initialize(new TextObject("{=YXzJMHPx}Coastal Guard Edict", null), new TextObject("{=kC2dxF1F}Towns are given extra funds to keep a small squadron of ships standing by, ready to sail forth into coastal waters to assist against enemy fleets or pirates.", null), new TextObject("{=cyzUKVhI}providing towns with finances to maintain small coastal squadrons", null), new TextObject("{=AvPX6JUF}A coastal guard force stands ready to assist allies in battles within the town's territorial waters{newline}Coastal town garrisons wages are increased by 15%", null), -0.1f, -0.1f, 0.4f);
			this._policyBolsterTheFyrd.Initialize(new TextObject("{=mumHAaVd}Bolster the Militia", null), new TextObject("{=ZQUwHH7T}This act requires villagers to provide stores to the local militia, including weapons, food, clothing and pack animals.", null), new TextObject("{=swkQVma8}requiring villages to set aside some goods for the militia.", null), new TextObject("{=6fuARR0S}Kingdom-wide militia generation boost: +25%{newline}Kingdom-wide village production penalty: -5%", null), -0.2f, 0.4f, -0.1f);
			this._policyNavalConjoiningStatute.Initialize(new TextObject("{=WKaTn7zA}Naval Wardenships", null), new TextObject("{=XBzBDoga}This statute grants special titles and ceremonial rights to lords who own powerful warships, to aid in the defense of the seas.", null), new TextObject("{=cWYn4vt4}encouraging lords to maintain powerful warships.", null), new TextObject("{=FpPhYbAk}Clans with a heavy ship gain +1 influence daily.{newline}Clans possessing no heavy or medium ships lose -1 influence daily", null), 0f, 0.2f, -0.8f);
			this._policyArsenalDepositoryAct.Initialize(new TextObject("{=eVr68fw7}Arsenals of State", null), new TextObject("{=tlbrO7bJ}The act creates arsenals to stockpile naval supplies at all ports, providing timber, ropes, tar and sailcloth to any nobles building ships.", null), new TextObject("{=GvXCPsq8}establishing arsenals to stockpile naval supplies.", null), new TextObject("{=a9qqNqSS}All clans within the kingdom benefit from a -15% reduction in ship purchase costs on own kingdom's ports.{newline}-10% tariff income", null), -0.2f, 0.1f, 0.6f);
		}

		// Token: 0x040000A7 RID: 167
		private PolicyObject _policyFraternalFleetDoctrine;

		// Token: 0x040000A8 RID: 168
		private PolicyObject _policyKingsTitheOnKeels;

		// Token: 0x040000A9 RID: 169
		private PolicyObject _policyRoyalRansomClaim;

		// Token: 0x040000AA RID: 170
		private PolicyObject _policyRoyalNavyPrerogative;

		// Token: 0x040000AB RID: 171
		private PolicyObject _policyMaritimeWealEdict;

		// Token: 0x040000AC RID: 172
		private PolicyObject _policyKingsPardonForPirates;

		// Token: 0x040000AD RID: 173
		private PolicyObject _policyRaidersSpoils;

		// Token: 0x040000AE RID: 174
		private PolicyObject _policyCoastalGuardEdict;

		// Token: 0x040000AF RID: 175
		private PolicyObject _policyBolsterTheFyrd;

		// Token: 0x040000B0 RID: 176
		private PolicyObject _policyNavalConjoiningStatute;

		// Token: 0x040000B1 RID: 177
		private PolicyObject _policyArsenalDepositoryAct;
	}
}
