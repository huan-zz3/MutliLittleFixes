using System;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;

namespace NavalDLC.CharacterDevelopment
{
	// Token: 0x0200015B RID: 347
	public class NavalPerks
	{
		// Token: 0x170003E6 RID: 998
		// (get) Token: 0x0600168C RID: 5772 RVA: 0x00099E9D File Offset: 0x0009809D
		private static NavalPerks Instance
		{
			get
			{
				return NavalDLCManager.Instance.NavalPerks;
			}
		}

		// Token: 0x0600168D RID: 5773 RVA: 0x00099EA9 File Offset: 0x000980A9
		public NavalPerks()
		{
			this.RegisterAll();
			this.InitializeAll();
		}

		// Token: 0x0600168E RID: 5774 RVA: 0x00099EC0 File Offset: 0x000980C0
		private void RegisterAll()
		{
			this._rollingThunder = this.Create("RollingThunder");
			this._piratesProwess = this.Create("PiratesProwess");
			this._forceful = this.Create("Forceful");
			this._bruteForce = this.Create("BruteForce");
			this._axeOfTheNorthwind = this.Create("AxeOfTheNorthwind");
			this._sunnyDisposition = this.Create("SunnyDisposition");
			this._enemyOfTheWood = this.Create("EnemyOfTheWood");
			this._navalFightingTraining = this.Create("NavalFightingTraining");
			this._terrorOfTheSeas = this.Create("TerrorOfTheSeas");
			this._rallyingCry = this.Create("RallyingCry");
			this._shatteringBlow = this.Create("ShatteringBlow");
			this._shatteringVolley = this.Create("ShatteringVolley");
			this._arr = this.Create("Arr");
			this._pirateHunter = this.Create("PirateHunter");
			this._boardingMaster = this.Create("BoardingMaster");
			this._homeTurfAdvantage = this.Create("HomeTurfAdvantage");
			this._mightyBlows = this.Create("MightyBlows");
			this._crewOfSpears = this.Create("CrewOfSpears");
			this._theSkysFury = this.Create("TheSkysFury");
			this._warriorsMight = this.Create("WarriorsMight");
			this._merchantPrince = this.Create("MerchantPrince");
			this._masterShipwright = this.Create("MasterShipwright");
			this._streamlinedOperations = this.Create("StreamlinedOperations");
			this._wellStocked = this.Create("WellStocked");
			this._navalHorde = this.Create("NavalHorde");
			this._optimization = this.Create("Optimization");
			this._gildedPurse = this.Create("GildedPurse");
			this._veteransWisdom = this.Create("VeteransWisdom");
			this._shipwrightsInsight = this.Create("ShipwrightsInsight");
			this._specialArrows = this.Create("SpecialArrows");
			this._smoothOperator = this.Create("SmoothOperator");
			this._accuracyTraining = this.Create("Accuracytraining");
			this._efficientCaptain = this.Create("EfficientCaptain");
			this._popularCaptain = this.Create("PopularCaptain");
			this._portAuthority = this.Create("PortAuthority");
			this._blessingsOfTheSea = this.Create("BlessingsOfTheSea");
			this._shipwrightsHand = this.Create("ShipwrightsHand");
			this._salvage = this.Create("Salvage");
			this._merchantFleet = this.Create("MerchantFleet");
			this._resilience = this.Create("Resilience");
			this._navalBombardment = this.Create("NavalBombardment");
			this._masterAngler = this.Create("MasterAngler");
			this._oldSaltsTouch = this.Create("OldSaltsTouch");
			this._ghostShip = this.Create("GhostShip");
			this._windRider = this.Create("WindRider");
			this._riverRaider = this.Create("RiverRaider");
			this._nightRaider = this.Create("NightRaider");
			this._windborne = this.Create("Windborne");
			this._shockAndAwe = this.Create("ShockAndAwe");
			this._theHelmsmansShield = this.Create("TheHelmsmansShield");
			this._ravenEye = this.Create("RavenEye");
			this._fairWinds = this.Create("FairWinds");
			this._favorableTide = this.Create("FavorableTide");
			this._unflinching = this.Create("Unflinching");
			this._shoreMaster = this.Create("ShoreMaster");
			this._fleetCommander = this.Create("FleetCommander");
			this._chainToOars = this.Create("ChainToOars");
			this._stormrider = this.Create("Stormrider");
			this._masterAndCommander = this.Create("MasterAndCommander");
			this._theCorsairsEdge = this.Create("TheCorsairsEdge");
			this._seaborneFortress = this.Create("SeaborneFortress");
			this._commodore = this.Create("Commodore");
		}

		// Token: 0x0600168F RID: 5775 RVA: 0x0009A2EC File Offset: 0x000984EC
		private void InitializeAll()
		{
			this._rollingThunder.Initialize("{=AtNKfDDP}Rolling Thunder", NavalSkills.Mariner, NavalPerks.GetTierCost(1), this._piratesProwess, "{=aYaJGhh9}{VALUE}% accuracy penalty from ship roll.", 12, -0.3f, 1, "{=mbcN7l2f}{VALUE}% conformity gain for pirate prisoners in your party while at sea.", 5, 0.5f, 1, 65535, 65535);
			this._piratesProwess.Initialize("{=csEbtEj5}Pirate's Prowess", NavalSkills.Mariner, NavalPerks.GetTierCost(1), this._rollingThunder, "{=7MtEpwKM}{VALUE}% melee weapon handling.", 12, 0.25f, 1, "{=bNZPW3qe}{VALUE}% loot from defeated merchant convoys.", 5, 0.3f, 1, 65535, 65535);
			this._forceful.Initialize("{=DeWp2GjP}Shield Breaker", NavalSkills.Mariner, NavalPerks.GetTierCost(2), this._bruteForce, "{=yfpyCOuu}{VALUE}% damage to shields dealt by crew.", 13, 0.3f, 1, "{=S4GbRzTr}{VALUE}% naval raid speed.", 5, 0.25f, 1, 0, 65535);
			this._bruteForce.Initialize("{=DLcRb2jH}Brute Force", NavalSkills.Mariner, NavalPerks.GetTierCost(2), this._forceful, "{=Jbc2m29I}{VALUE}% kicking and bashing damage.", 12, 0.5f, 1, "{=YzZr0gtE}{VALUE}% more loot from naval raids.", 5, 0.2f, 1, 65535, 65535);
			this._axeOfTheNorthwind.Initialize("{=mhzGQYl9}Axe of the North Wind", NavalSkills.Mariner, NavalPerks.GetTierCost(3), this._sunnyDisposition, "{=SlnKlVOl}{VALUE}% damage dealt by axes.", 12, 0.2f, 1, "{=8STt46ci}{VALUE} morale for mariner troops at start of battle.", 5, 20f, 0, 65535, 65535);
			this._sunnyDisposition.Initialize("{=MKOmGiqt}Sunny Disposition", NavalSkills.Mariner, NavalPerks.GetTierCost(3), this._axeOfTheNorthwind, "{=VB9rTE73}{VALUE}% damage dealt by swords.", 12, 0.2f, 1, "{=BYY8MiFe}{VALUE} morale for regular troops at start of battle.", 5, 20f, 0, 65535, 65535);
			this._enemyOfTheWood.Initialize("{=Tf7zOvfL}Enemy of the Wood", NavalSkills.Mariner, NavalPerks.GetTierCost(4), this._navalFightingTraining, "{=McqgoySh}{VALUE} morale to enemy for each ship destroyed in battle.", 4, -10f, 0, "{=6YsSNwTW}{VALUE}% fire damage dealt by your ship and crew to enemy sails.", 13, 0.25f, 1, 65535, 0);
			this._navalFightingTraining.Initialize("{=cvOhFtKn}Naval Fighting Training", NavalSkills.Mariner, NavalPerks.GetTierCost(4), this._enemyOfTheWood, "{=pRTSU12h}{VALUE}% to xp gained by party companions after each naval battle.", 5, 0.1f, 1, "{=F8cJJlZn}{VALUE}% Increase to militia veterancy in coastal settlements.", 3, 0.1f, 1, 65535, 65535);
			this._terrorOfTheSeas.Initialize("{=nUUAag3J}Terror of the Seas", NavalSkills.Mariner, NavalPerks.GetTierCost(5), this._rallyingCry, "{=VAFEpyau}{VALUE}% to morale loss suffered by enemy ships in battle.", 5, 0.2f, 1, "{=hMFAmWkE}{VALUE}% melee damage taken by crew while on enemy ships.", 13, -0.1f, 1, 65535, 0);
			this._rallyingCry.Initialize("{=5S1QiUvh}Rallying Cry", NavalSkills.Mariner, NavalPerks.GetTierCost(5), this._terrorOfTheSeas, "{=GTY1d7RX}{VALUE}% morale boost for crew while on own ship.", 13, 0.2f, 1, "{=GX5M0gbo}{VALUE}% melee damage taken by crew while on own ships.", 13, -0.1f, 1, 0, 0);
			this._shatteringBlow.Initialize("{=mbaYZ0QB}Shattering Blow", NavalSkills.Mariner, NavalPerks.GetTierCost(6), this._shatteringVolley, "{=MUsv10MO}{VALUE}% armor penetration for melee weapons.", 12, 0.5f, 1, "{=vmBVfzVL}{VALUE}% armor penetration for melee weapons wielded by crew.", 13, 0.5f, 1, 65535, 4);
			this._shatteringVolley.Initialize("{=InUgc3PT}Shattering Volley", NavalSkills.Mariner, NavalPerks.GetTierCost(6), this._shatteringBlow, "{=pKk0fKba}{VALUE}% armor penetration for ranged weapons.", 12, 0.5f, 1, "{=MzQBOs13}{VALUE}% armor penetration for ranged weapons wielded by crew.", 13, 0.5f, 1, 65535, 8);
			this._arr.Initialize("{=OlvwVG3b}Arr!", NavalSkills.Mariner, NavalPerks.GetTierCost(7), this._pirateHunter, "{=Sa7FPVnT}Surrendering pirate parties can be recruited.", 12, 0f, 0, "{=eHg3h3j7}{VALUE}% xp gain after each battle for mariner troops under character's command.", 13, 0.15f, 1, 65535, 0);
			this._pirateHunter.Initialize("{=qlMgDT7y}Pirate Hunter", NavalSkills.Mariner, NavalPerks.GetTierCost(7), this._arr, "{=HnyLGFbu}{VALUE}% bonus when crew is sent to confront pirates.", 5, 0.2f, 1, "{=sIOdlPOA}{VALUE}% xp gain after each battle for regular troops under character's command.", 13, 0.1f, 1, 65535, 0);
			this._boardingMaster.Initialize("{=gkJ1fRSM}Boarding Master", NavalSkills.Mariner, NavalPerks.GetTierCost(8), this._homeTurfAdvantage, "{=HAbSFYFz}{VALUE}% melee damage dealt by character when fighting on other ships.", 12, 0.15f, 1, "{=pR8afW3c}{VALUE}% melee damage dealt by crew when fighting on other ships.", 13, 0.15f, 1, 65535, 4);
			this._homeTurfAdvantage.Initialize("{=n5g7EvDQ}Home Turf Advantage", NavalSkills.Mariner, NavalPerks.GetTierCost(8), this._boardingMaster, "{=RLrenzWj}{VALUE}% melee damage dealt by character when fighting on own ship.", 12, 0.2f, 1, "{=8Qp9IKuG}{VALUE}% melee damage dealt by crew when fighting on own ship.", 13, 0.2f, 1, 65535, 4);
			this._mightyBlows.Initialize("{=RSMwW4mr}Mighty Blows", NavalSkills.Mariner, NavalPerks.GetTierCost(9), this._crewOfSpears, "{=YbAddajR}Better cleave with two handed weapons swings. (Two handed weapons lose {VALUE}% less damage when they cut through the first opponent.)", 12, -0.5f, 1, "{=fjettvEU}{VALUE}% melee damage dealt by crew armed with two-handed weapons.", 13, 0.15f, 1, 65535, 64);
			this._crewOfSpears.Initialize("{=BDp8MzPJ}Crew of Spears", NavalSkills.Mariner, NavalPerks.GetTierCost(9), this._mightyBlows, "{=IQhRdMoc}Impale shields with thrown javelins, and throwing axes deals damage after if they break a shield.", 12, 0f, 0, "{=wMwRA172}{VALUE}% ranged damage dealt by crew armed with throwing weapons.", 13, 0.15f, 1, 65535, 512);
			this._theSkysFury.Initialize("{=fS6ZrhCH}The Sky's Fury", NavalSkills.Mariner, NavalPerks.GetTierCost(10), this._warriorsMight, "{=aP1gxbRF}{VALUE}% ranged damage dealt by character.", 12, 0.15f, 1, "{=MHC4pkJE}{VALUE}% to bow and crossbow damage dealt by crew.", 13, 0.15f, 1, 65535, 1280);
			this._warriorsMight.Initialize("{=CuNzwLc3}Warrior's Might", NavalSkills.Mariner, NavalPerks.GetTierCost(10), this._theSkysFury, "{=H7Hs2E3D}{VALUE}% melee damage dealt by character.", 12, 0.2f, 1, "{=KvGLih9i}{VALUE}% to throwing damage dealt by crew.", 13, 0.2f, 1, 65535, 512);
			this._merchantPrince.Initialize("{=P79raYEW}Merchant Prince", NavalSkills.Boatswain, NavalPerks.GetTierCost(1), this._masterShipwright, "{=UL4LyWhF}{VALUE}% to ship repair cost", 14, -0.3f, 1, "{=bQ5iyRiM}{VALUE} denars for each ship bought or sold in governed settlement.", 3, 500f, 0, 65535, 65535);
			this._masterShipwright.Initialize("{=DQ7KWQJq}Master Shipwright", NavalSkills.Boatswain, NavalPerks.GetTierCost(1), this._merchantPrince, "{=R0akt6jI}{VALUE}% to cost of ship upgrades", 14, -0.3f, 1, "{=WvBaUVAr}{VALUE} denars from each ship repaired at governed settlement", 3, 30f, 0, 65535, 65535);
			this._streamlinedOperations.Initialize("{=oUtZ27Id}Streamlined Operations", NavalSkills.Boatswain, NavalPerks.GetTierCost(2), this._wellStocked, "{=Trsw67ag}{VALUE}% ballista reload time.", 14, 0.1f, 1, "{=ZWOY78k2}{VALUE}% to shipyard production rate in governed settlement.", 3, 0.2f, 1, 0, 65535);
			this._wellStocked.Initialize("{=ReTUWtie}Well Stocked", NavalSkills.Boatswain, NavalPerks.GetTierCost(2), this._streamlinedOperations, "{=Myea2YPh}{VALUE}% ammunition per stack for crew under command.", 14, 0.3f, 1, "{=4XCAUAee}{VALUE} extra ammunition per stack for thrown weapons of the crew.", 14, 2f, 0, 0, 512);
			this._navalHorde.Initialize("{=1uWha4cw}Naval Horde", NavalSkills.Boatswain, NavalPerks.GetTierCost(3), this._optimization, "{=1aCQf9Xf}{VALUE}% to wages for cavalry troops while at sea.", 14, -0.3f, 1, "{=9Hsd1fuX}{VALUE}% to weight of mounts when in ship's cargo.", 14, -0.3f, 1, 65535, 65535);
			this._optimization.Initialize("{=ON5j1Gwp}Optimization", NavalSkills.Boatswain, NavalPerks.GetTierCost(3), this._navalHorde, "{=KVrphJkB}{VALUE}% to wages of non-mariner troops while at sea.", 14, -0.1f, 1, "{=wdnSjdLE}{VALUE}% to weight of pack animals and livestock when in ship's cargo", 14, -0.3f, 1, 65535, 65535);
			this._gildedPurse.Initialize("{=tXOmhbFz}Gilded Purse", NavalSkills.Boatswain, NavalPerks.GetTierCost(4), this._veteransWisdom, "{=xI8UK8Wp}{VALUE}% to chance of capturing ships after battle.", 5, 0.25f, 1, "{=bUXtraYH}{VALUE}% to weight of trade goods in ship's cargo.", 14, -0.15f, 1, 65535, 65535);
			this._veteransWisdom.Initialize("{=Nlz7g0GX}Veteran's Wisdom", NavalSkills.Boatswain, NavalPerks.GetTierCost(4), this._gildedPurse, "{=ziKgHMqy}Daily bonus of {VALUE}x character level xp to companions.", 5, 10f, 0, "{=jzwCbxni}{VALUE}% overburden penalty for ships with too much cargo.", 14, -0.2f, 1, 65535, 65535);
			this._shipwrightsInsight.Initialize("{=6gQTNK1Q}Shipwright's Insight", NavalSkills.Boatswain, NavalPerks.GetTierCost(5), this._specialArrows, "{=fEiCOPxK}{VALUE}% damage to hulls of enemy ships dealt by ballista.", 14, 0.3f, 1, "{=9WinSM8I}{VALUE}% extra ammo for crew.", 14, 0.25f, 1, 0, 0);
			this._specialArrows.Initialize("{=sSwzYLVp}Special Arrows", NavalSkills.Boatswain, NavalPerks.GetTierCost(5), this._shipwrightsInsight, "{=KqEZ5bht}{VALUE} armor for low-tier troops.", 13, 5f, 0, "{=8kHlbgJZ}{VALUE}% damage dealt by crew to enemy sails.", 14, 0.4f, 1, 0, 0);
			this._smoothOperator.Initialize("{=k5xfZsXE}Smooth Operator", NavalSkills.Boatswain, NavalPerks.GetTierCost(6), this._accuracyTraining, "{=elLNH0Ys}{VALUE} ammo for ballista.", 14, 5f, 0, "{=JLP7pNIv}{VALUE}% food consumption at sea.", 14, -0.3f, 1, 0, 65535);
			this._accuracyTraining.Initialize("{=09T5s6fh}Accuracy Training", NavalSkills.Boatswain, NavalPerks.GetTierCost(6), this._smoothOperator, "{=TAcbw7Ac}{VALUE}% damage dealt to shields with ranged weapons wielded by crew.", 14, 0.3f, 1, "{=MDDAbNbX}{VALUE} militia in coastal towns and villages.", 3, 2f, 0, 8, 65535);
			this._efficientCaptain.Initialize("{=bb4nRJwq}Efficient Captain", NavalSkills.Boatswain, NavalPerks.GetTierCost(7), this._popularCaptain, "{=OFa86K25}{VALUE}% upgrade cost for mariner troops.", 5, -0.3f, 1, "{=VbWCb1JQ}{VALUE} morale for the party while waiting in a port.", 5, 5f, 0, 65535, 65535);
			this._popularCaptain.Initialize("{=di0OsDy8}Popular Captain", NavalSkills.Boatswain, NavalPerks.GetTierCost(7), this._efficientCaptain, "{=V4ornHeA}{VALUE}% recruitment cost for mariner troops.", 5, -0.3f, 1, "{=oIWHDTkm}{VALUE}% combat deck size for the ship under the character's command.", 5, 0.05f, 1, 65535, 0);
			this._portAuthority.Initialize("{=MATYHBox}Port Authority", NavalSkills.Boatswain, NavalPerks.GetTierCost(8), this._blessingsOfTheSea, "{=2mzGeKT1}{VALUE} ship to command limit.", 5, 1f, 0, "{=dW9eIKTx}{VALUE}% to production of walrus ivory and whale oil at villages of the governed settlement.", 3, 0.15f, 1, 65535, 65535);
			this._blessingsOfTheSea.Initialize("{=eeY1vDcp}Blessings Of The Sea", NavalSkills.Boatswain, NavalPerks.GetTierCost(8), this._portAuthority, "{=2mzGeKT1}{VALUE} ship to command limit.", 5, 1f, 0, "{=astRi69P}{VALUE}% to production of fish at villages governed of the governed settlement.", 3, 0.25f, 1, 65535, 65535);
			this._shipwrightsHand.Initialize("{=2DY8xbnU}Shipwright's Hand", NavalSkills.Boatswain, NavalPerks.GetTierCost(9), this._salvage, "{=MsFlwAwg}Discarded ships repair party ships.", 5, 0f, 0, "{=XSAXjPX3}{VALUE} recruit to garrison for each merchant convoy entering port.", 3, 1f, 0, 65535, 65535);
			this._salvage.Initialize("{=LkaTaAyq}Salvage", NavalSkills.Boatswain, NavalPerks.GetTierCost(9), this._shipwrightsHand, "{=0zuAboaA}Discarded ships provide influence.", 5, 0f, 0, "{=L7uvT9VN}{VALUE} denars gained for each merchant convoy entering port", 3, 40f, 0, 65535, 65535);
			this._merchantFleet.Initialize("{=0xz4b4wl}Merchant Fleet", NavalSkills.Boatswain, NavalPerks.GetTierCost(10), this._resilience, "{=Iu7QpMVa}{VALUE} to command limit of ships in battle.", 5, 1f, 0, "{=JfZESBwx}{VALUE} influence gained for each ship built in the governed settlement.", 3, 5f, 0, 65535, 65535);
			this._resilience.Initialize("{=9mNUfKMo}Resilience", NavalSkills.Boatswain, NavalPerks.GetTierCost(10), this._merchantFleet, "{=qVyf6an2}{VALUE}% of the health points lost in a battle are recovered after a victory.", 12, 0.3f, 1, "{=KkCqaWLc}{VALUE}% to troops' healing rate while at sea.", 14, 0.3f, 1, 65535, 65535);
			this._navalBombardment.Initialize("{=21meODRf}Naval Bombardment", NavalSkills.Boatswain, NavalPerks.GetTierCost(11), null, "{=61FfgHSc}Shipboard ballistas fire during sieges", 5, 0f, 0, "", 0, 0f, -1, 65535, 65535);
			this._masterAngler.Initialize("{=DWBiOEdQ}Master Angler", NavalSkills.Shipmaster, NavalPerks.GetTierCost(1), this._oldSaltsTouch, "{=DrNqwl3D}{VALUE}% chance per hour of campaign time to catch fish while sailing on campaign map.", 15, 0.25f, 1, "{=CHCbc79h}{VALUE}% to catch brought in by fishing boats from settlements.", 3, 0.15f, 1, 65535, 65535);
			this._oldSaltsTouch.Initialize("{=vcyysBJb}Old Salt's Touch", NavalSkills.Shipmaster, NavalPerks.GetTierCost(1), this._masterAngler, "{=l6P2ivTu}{VALUE}% to travel speed at sea.", 15, 0.02f, 1, "{=1UIq6BRz}{VALUE}% to swimming endurance of crew", 13, 0.3f, 1, 65535, 0);
			this._ghostShip.Initialize("{=Sya9pgBv}Ghost Ship", NavalSkills.Shipmaster, NavalPerks.GetTierCost(2), this._windRider, "{=nTPPc5CT}{VALUE}% fewer troops lost when running a blockade and {VALUE}% chance to avoid leaving ships behind when escaping engagements.", 15, 0.2f, 1, "{=B9AfUcsl}{VALUE}% to campaign map speed of town's fishing boats.", 3, 0.2f, 1, 65535, 65535);
			this._windRider.Initialize("{=bIFiogWa}Wind Rider", NavalSkills.Shipmaster, NavalPerks.GetTierCost(2), this._ghostShip, "{=kIb7qvy8}{VALUE}% to deck movement penalty.", 12, -0.5f, 1, "{=m2ZMZ27i}{VALUE}% to deck movement penalty for crew.", 13, -0.5f, 1, 65535, 0);
			this._riverRaider.Initialize("{=46X8OsCn}River Raider", NavalSkills.Shipmaster, NavalPerks.GetTierCost(3), this._nightRaider, "{=fMe3Yas2}{VALUE}% to coastal movement speed penalty.", 15, -0.03f, 1, "{=XbbSGawE}{VALUE}% to chance of capturing enemy troops as prisoners.", 5, 0.1f, 1, 65535, 0);
			this._nightRaider.Initialize("{=MQlcI2bf}Night Raider", NavalSkills.Shipmaster, NavalPerks.GetTierCost(3), this._riverRaider, "{=CRSaWc9S}{VALUE}% to night spotting range penalty on campaign map.", 15, -0.5f, 1, "{=tYqawD5G}{VALUE} fish per day produced by coastal villages.", 3, 5f, 0, 65535, 65535);
			this._windborne.Initialize("{=49NFcwEM}Windborne", NavalSkills.Shipmaster, NavalPerks.GetTierCost(4), this._shockAndAwe, "{=6JZ8ZAqD}{VALUE}% to sail forces in missions.", 13, 0.2f, 1, "{=dnCVmLuI}{VALUE}% to duration of disorganized state while at sea.", 15, -0.5f, 1, 0, 65535);
			this._shockAndAwe.Initialize("{=M1RMMBzF}Shock and Awe", NavalSkills.Shipmaster, NavalPerks.GetTierCost(4), this._windborne, "{=nxYOlJuf}{VALUE}% morale boost when ship rams enemy ship.", 13, 0.3f, 1, "{=YEpGBEOb}{VALUE}% to speed on campaign map when against the wind.", 15, 0.1f, 1, 0, 65535);
			this._theHelmsmansShield.Initialize("{=nO4nrVeF}The Helmsman's Shield", NavalSkills.Shipmaster, NavalPerks.GetTierCost(5), this._ravenEye, "{=IEQgidZB}{VALUE}% to ranged damage suffered by character while steering the ship.", 12, -0.5f, 1, "{=ZV0bkXpX}{VALUE} prosperity for each fishing boat returning to port.", 3, 1f, 0, 65535, 65535);
			this._ravenEye.Initialize("{=NMgbVLbx}Raven Eye", NavalSkills.Shipmaster, NavalPerks.GetTierCost(5), this._theHelmsmansShield, "{=zEJLYSYa}{VALUE}% to spotting range on campaign map.", 15, 0.2f, 1, "{=5bFACRPa}{VALUE} loyalty for each fishing boat returning to port.", 3, 1f, 0, 65535, 65535);
			this._fairWinds.Initialize("{=LOZkPSV1}Fair Winds", NavalSkills.Shipmaster, NavalPerks.GetTierCost(6), this._favorableTide, "{=qC0VbVsB}{VALUE}% to campaign map travel speed when running before the wind.", 15, 0.1f, 1, "{=alTcdq2M}{VALUE}% to hearth growth rate in coastal villages", 3, 0.1f, 1, 65535, 65535);
			this._favorableTide.Initialize("{=uZo1RgiX}Favorable Tide", NavalSkills.Shipmaster, NavalPerks.GetTierCost(6), this._fairWinds, "{=MgNvqdV5}{VALUE}% to campaign map travel speed ", 15, 0.05f, 1, "{=5fwthk1U}{VALUE} building material in settlement for each merchant convoy visiting port", 3, 1f, 0, 65535, 65535);
			this._unflinching.Initialize("{=WdFTSzc1}Unflinching", NavalSkills.Shipmaster, NavalPerks.GetTierCost(7), this._shoreMaster, "{=K7hL8TgJ}{VALUE}% to disembarkation speed.", 5, 1f, 1, "", 0, 0f, -1, 65535, 65535);
			this._shoreMaster.Initialize("{=YluQBFDG}Shore Master", NavalSkills.Shipmaster, NavalPerks.GetTierCost(7), this._unflinching, "{=qhsfjbv6}{VALUE}% to ship recall time.", 5, -0.5f, 1, "{=mbT5BIx3}{VALUE}% to fleet size penalty for campaign map movement", 15, -0.3f, 1, 65535, 65535);
			this._fleetCommander.Initialize("{=ZJyEHWfa}Fleet Commander", NavalSkills.Shipmaster, NavalPerks.GetTierCost(8), this._chainToOars, "{=mbT5BIx3}{VALUE}% to fleet size penalty for campaign map movement", 5, -0.3f, 1, "{=F4zoUlg1}{VALUE}% to skeleton crew requirement for any ship in party", 5, -0.2f, 1, 65535, 65535);
			this._chainToOars.Initialize("{=R1izgjgK}Chain to Oars", NavalSkills.Shipmaster, NavalPerks.GetTierCost(8), this._fleetCommander, "{=vSJEt7l8}{VALUE}% to oar force in mission", 13, 0.2f, 1, "{=vRjKbjOa}Prisoners help meet skeleton crew requirement", 5, 0f, 0, 0, 65535);
			this._stormrider.Initialize("{=uLPXg0qS}Stormrider", NavalSkills.Shipmaster, NavalPerks.GetTierCost(9), this._masterAndCommander, "{=EAlm5gSh}{VALUE} xp gained by each troop once per day when entering storms", 5, 30f, 0, "{=2mzGeKT1}{VALUE} ship to command limit.", 5, 1f, 0, 65535, 65535);
			this._masterAndCommander.Initialize("{=tivTy0RA}Master and commander", NavalSkills.Shipmaster, NavalPerks.GetTierCost(9), this._stormrider, "{=dGZAPFdQ}{VALUE} xp gained by each troop per hour at sea", 5, 1f, 0, "{=2mzGeKT1}{VALUE} ship to command limit.", 5, 1f, 0, 65535, 65535);
			this._theCorsairsEdge.Initialize("{=JvwosrT8}The Corsair's Edge", NavalSkills.Shipmaster, NavalPerks.GetTierCost(10), this._seaborneFortress, "{=QUI46tx7}{VALUE}% damage when wielding one-handed weapons at sea", 12, 0.1f, 1, "{=DRb8Us5b}{VALUE} fishing boat produced by each coastal settlement.", 3, 1f, 0, 65535, 65535);
			this._seaborneFortress.Initialize("{=Dyy3HcUI}Seaborne Fortress", NavalSkills.Shipmaster, NavalPerks.GetTierCost(10), this._theCorsairsEdge, "{=kAajH6yd}{VALUE}% to damage sustained by ships when crew is sent to confront the enemies.", 5, -0.1f, 1, "{=OszgWi0t}{VALUE}% to ranged damage taken by crew if not boarded.", 13, -0.2f, 1, 65535, 0);
			this._commodore.Initialize("{=NbZeB1RT}Commodore", NavalSkills.Shipmaster, NavalPerks.GetTierCost(11), null, "{=1g9TufnU}Flagship figurehead provides bonus to all allied ships.", 4, 0f, 0, "", 0, 0f, -1, 65535, 65535);
		}

		// Token: 0x06001690 RID: 5776 RVA: 0x0009B30D File Offset: 0x0009950D
		private PerkObject Create(string stringId)
		{
			return Game.Current.ObjectManager.RegisterPresumedObject<PerkObject>(new PerkObject(stringId));
		}

		// Token: 0x06001691 RID: 5777 RVA: 0x0009B324 File Offset: 0x00099524
		private static int GetTierCost(int tierIndex)
		{
			return NavalPerks.TierSkillRequirements[tierIndex - 1];
		}

		// Token: 0x04000B75 RID: 2933
		private static readonly int[] TierSkillRequirements = new int[]
		{
			25, 50, 75, 100, 125, 150, 175, 200, 225, 250,
			275, 300
		};

		// Token: 0x04000B76 RID: 2934
		private PerkObject _rollingThunder;

		// Token: 0x04000B77 RID: 2935
		private PerkObject _piratesProwess;

		// Token: 0x04000B78 RID: 2936
		private PerkObject _forceful;

		// Token: 0x04000B79 RID: 2937
		private PerkObject _bruteForce;

		// Token: 0x04000B7A RID: 2938
		private PerkObject _axeOfTheNorthwind;

		// Token: 0x04000B7B RID: 2939
		private PerkObject _sunnyDisposition;

		// Token: 0x04000B7C RID: 2940
		private PerkObject _enemyOfTheWood;

		// Token: 0x04000B7D RID: 2941
		private PerkObject _navalFightingTraining;

		// Token: 0x04000B7E RID: 2942
		private PerkObject _terrorOfTheSeas;

		// Token: 0x04000B7F RID: 2943
		private PerkObject _rallyingCry;

		// Token: 0x04000B80 RID: 2944
		private PerkObject _shatteringBlow;

		// Token: 0x04000B81 RID: 2945
		private PerkObject _shatteringVolley;

		// Token: 0x04000B82 RID: 2946
		private PerkObject _arr;

		// Token: 0x04000B83 RID: 2947
		private PerkObject _pirateHunter;

		// Token: 0x04000B84 RID: 2948
		private PerkObject _boardingMaster;

		// Token: 0x04000B85 RID: 2949
		private PerkObject _homeTurfAdvantage;

		// Token: 0x04000B86 RID: 2950
		private PerkObject _mightyBlows;

		// Token: 0x04000B87 RID: 2951
		private PerkObject _crewOfSpears;

		// Token: 0x04000B88 RID: 2952
		private PerkObject _theSkysFury;

		// Token: 0x04000B89 RID: 2953
		private PerkObject _warriorsMight;

		// Token: 0x04000B8A RID: 2954
		private PerkObject _merchantPrince;

		// Token: 0x04000B8B RID: 2955
		private PerkObject _masterShipwright;

		// Token: 0x04000B8C RID: 2956
		private PerkObject _streamlinedOperations;

		// Token: 0x04000B8D RID: 2957
		private PerkObject _wellStocked;

		// Token: 0x04000B8E RID: 2958
		private PerkObject _navalHorde;

		// Token: 0x04000B8F RID: 2959
		private PerkObject _optimization;

		// Token: 0x04000B90 RID: 2960
		private PerkObject _gildedPurse;

		// Token: 0x04000B91 RID: 2961
		private PerkObject _veteransWisdom;

		// Token: 0x04000B92 RID: 2962
		private PerkObject _shipwrightsInsight;

		// Token: 0x04000B93 RID: 2963
		private PerkObject _specialArrows;

		// Token: 0x04000B94 RID: 2964
		private PerkObject _smoothOperator;

		// Token: 0x04000B95 RID: 2965
		private PerkObject _accuracyTraining;

		// Token: 0x04000B96 RID: 2966
		private PerkObject _efficientCaptain;

		// Token: 0x04000B97 RID: 2967
		private PerkObject _popularCaptain;

		// Token: 0x04000B98 RID: 2968
		private PerkObject _portAuthority;

		// Token: 0x04000B99 RID: 2969
		private PerkObject _blessingsOfTheSea;

		// Token: 0x04000B9A RID: 2970
		private PerkObject _shipwrightsHand;

		// Token: 0x04000B9B RID: 2971
		private PerkObject _salvage;

		// Token: 0x04000B9C RID: 2972
		private PerkObject _merchantFleet;

		// Token: 0x04000B9D RID: 2973
		private PerkObject _resilience;

		// Token: 0x04000B9E RID: 2974
		private PerkObject _navalBombardment;

		// Token: 0x04000B9F RID: 2975
		private PerkObject _masterAngler;

		// Token: 0x04000BA0 RID: 2976
		private PerkObject _oldSaltsTouch;

		// Token: 0x04000BA1 RID: 2977
		private PerkObject _ghostShip;

		// Token: 0x04000BA2 RID: 2978
		private PerkObject _windRider;

		// Token: 0x04000BA3 RID: 2979
		private PerkObject _riverRaider;

		// Token: 0x04000BA4 RID: 2980
		private PerkObject _nightRaider;

		// Token: 0x04000BA5 RID: 2981
		private PerkObject _windborne;

		// Token: 0x04000BA6 RID: 2982
		private PerkObject _shockAndAwe;

		// Token: 0x04000BA7 RID: 2983
		private PerkObject _theHelmsmansShield;

		// Token: 0x04000BA8 RID: 2984
		private PerkObject _ravenEye;

		// Token: 0x04000BA9 RID: 2985
		private PerkObject _fairWinds;

		// Token: 0x04000BAA RID: 2986
		private PerkObject _favorableTide;

		// Token: 0x04000BAB RID: 2987
		private PerkObject _unflinching;

		// Token: 0x04000BAC RID: 2988
		private PerkObject _shoreMaster;

		// Token: 0x04000BAD RID: 2989
		private PerkObject _fleetCommander;

		// Token: 0x04000BAE RID: 2990
		private PerkObject _chainToOars;

		// Token: 0x04000BAF RID: 2991
		private PerkObject _stormrider;

		// Token: 0x04000BB0 RID: 2992
		private PerkObject _masterAndCommander;

		// Token: 0x04000BB1 RID: 2993
		private PerkObject _theCorsairsEdge;

		// Token: 0x04000BB2 RID: 2994
		private PerkObject _seaborneFortress;

		// Token: 0x04000BB3 RID: 2995
		private PerkObject _commodore;

		// Token: 0x0200028D RID: 653
		public class Mariner
		{
			// Token: 0x17000425 RID: 1061
			// (get) Token: 0x06001C72 RID: 7282 RVA: 0x000B9971 File Offset: 0x000B7B71
			public static PerkObject RollingThunder
			{
				get
				{
					return NavalPerks.Instance._rollingThunder;
				}
			}

			// Token: 0x17000426 RID: 1062
			// (get) Token: 0x06001C73 RID: 7283 RVA: 0x000B997D File Offset: 0x000B7B7D
			public static PerkObject PiratesProwess
			{
				get
				{
					return NavalPerks.Instance._piratesProwess;
				}
			}

			// Token: 0x17000427 RID: 1063
			// (get) Token: 0x06001C74 RID: 7284 RVA: 0x000B9989 File Offset: 0x000B7B89
			public static PerkObject Forceful
			{
				get
				{
					return NavalPerks.Instance._forceful;
				}
			}

			// Token: 0x17000428 RID: 1064
			// (get) Token: 0x06001C75 RID: 7285 RVA: 0x000B9995 File Offset: 0x000B7B95
			public static PerkObject BruteForce
			{
				get
				{
					return NavalPerks.Instance._bruteForce;
				}
			}

			// Token: 0x17000429 RID: 1065
			// (get) Token: 0x06001C76 RID: 7286 RVA: 0x000B99A1 File Offset: 0x000B7BA1
			public static PerkObject AxeOfTheNorthwind
			{
				get
				{
					return NavalPerks.Instance._axeOfTheNorthwind;
				}
			}

			// Token: 0x1700042A RID: 1066
			// (get) Token: 0x06001C77 RID: 7287 RVA: 0x000B99AD File Offset: 0x000B7BAD
			public static PerkObject SunnyDisposition
			{
				get
				{
					return NavalPerks.Instance._sunnyDisposition;
				}
			}

			// Token: 0x1700042B RID: 1067
			// (get) Token: 0x06001C78 RID: 7288 RVA: 0x000B99B9 File Offset: 0x000B7BB9
			public static PerkObject EnemyOfTheWood
			{
				get
				{
					return NavalPerks.Instance._enemyOfTheWood;
				}
			}

			// Token: 0x1700042C RID: 1068
			// (get) Token: 0x06001C79 RID: 7289 RVA: 0x000B99C5 File Offset: 0x000B7BC5
			public static PerkObject NavalFightingTraining
			{
				get
				{
					return NavalPerks.Instance._navalFightingTraining;
				}
			}

			// Token: 0x1700042D RID: 1069
			// (get) Token: 0x06001C7A RID: 7290 RVA: 0x000B99D1 File Offset: 0x000B7BD1
			public static PerkObject TerrorOfTheSeas
			{
				get
				{
					return NavalPerks.Instance._terrorOfTheSeas;
				}
			}

			// Token: 0x1700042E RID: 1070
			// (get) Token: 0x06001C7B RID: 7291 RVA: 0x000B99DD File Offset: 0x000B7BDD
			public static PerkObject RallyingCry
			{
				get
				{
					return NavalPerks.Instance._rallyingCry;
				}
			}

			// Token: 0x1700042F RID: 1071
			// (get) Token: 0x06001C7C RID: 7292 RVA: 0x000B99E9 File Offset: 0x000B7BE9
			public static PerkObject ShatteringBlow
			{
				get
				{
					return NavalPerks.Instance._shatteringBlow;
				}
			}

			// Token: 0x17000430 RID: 1072
			// (get) Token: 0x06001C7D RID: 7293 RVA: 0x000B99F5 File Offset: 0x000B7BF5
			public static PerkObject ShatteringVolley
			{
				get
				{
					return NavalPerks.Instance._shatteringVolley;
				}
			}

			// Token: 0x17000431 RID: 1073
			// (get) Token: 0x06001C7E RID: 7294 RVA: 0x000B9A01 File Offset: 0x000B7C01
			public static PerkObject Arr
			{
				get
				{
					return NavalPerks.Instance._arr;
				}
			}

			// Token: 0x17000432 RID: 1074
			// (get) Token: 0x06001C7F RID: 7295 RVA: 0x000B9A0D File Offset: 0x000B7C0D
			public static PerkObject PirateHunter
			{
				get
				{
					return NavalPerks.Instance._pirateHunter;
				}
			}

			// Token: 0x17000433 RID: 1075
			// (get) Token: 0x06001C80 RID: 7296 RVA: 0x000B9A19 File Offset: 0x000B7C19
			public static PerkObject BoardingMaster
			{
				get
				{
					return NavalPerks.Instance._boardingMaster;
				}
			}

			// Token: 0x17000434 RID: 1076
			// (get) Token: 0x06001C81 RID: 7297 RVA: 0x000B9A25 File Offset: 0x000B7C25
			public static PerkObject HomeTurfAdvantage
			{
				get
				{
					return NavalPerks.Instance._homeTurfAdvantage;
				}
			}

			// Token: 0x17000435 RID: 1077
			// (get) Token: 0x06001C82 RID: 7298 RVA: 0x000B9A31 File Offset: 0x000B7C31
			public static PerkObject MightyBlows
			{
				get
				{
					return NavalPerks.Instance._mightyBlows;
				}
			}

			// Token: 0x17000436 RID: 1078
			// (get) Token: 0x06001C83 RID: 7299 RVA: 0x000B9A3D File Offset: 0x000B7C3D
			public static PerkObject CrewOfSpears
			{
				get
				{
					return NavalPerks.Instance._crewOfSpears;
				}
			}

			// Token: 0x17000437 RID: 1079
			// (get) Token: 0x06001C84 RID: 7300 RVA: 0x000B9A49 File Offset: 0x000B7C49
			public static PerkObject TheSkysFury
			{
				get
				{
					return NavalPerks.Instance._theSkysFury;
				}
			}

			// Token: 0x17000438 RID: 1080
			// (get) Token: 0x06001C85 RID: 7301 RVA: 0x000B9A55 File Offset: 0x000B7C55
			public static PerkObject WarriorsMight
			{
				get
				{
					return NavalPerks.Instance._warriorsMight;
				}
			}
		}

		// Token: 0x0200028E RID: 654
		public class Boatswain
		{
			// Token: 0x17000439 RID: 1081
			// (get) Token: 0x06001C87 RID: 7303 RVA: 0x000B9A69 File Offset: 0x000B7C69
			public static PerkObject MerchantPrince
			{
				get
				{
					return NavalPerks.Instance._merchantPrince;
				}
			}

			// Token: 0x1700043A RID: 1082
			// (get) Token: 0x06001C88 RID: 7304 RVA: 0x000B9A75 File Offset: 0x000B7C75
			public static PerkObject MasterShipwright
			{
				get
				{
					return NavalPerks.Instance._masterShipwright;
				}
			}

			// Token: 0x1700043B RID: 1083
			// (get) Token: 0x06001C89 RID: 7305 RVA: 0x000B9A81 File Offset: 0x000B7C81
			public static PerkObject StreamlinedOperations
			{
				get
				{
					return NavalPerks.Instance._streamlinedOperations;
				}
			}

			// Token: 0x1700043C RID: 1084
			// (get) Token: 0x06001C8A RID: 7306 RVA: 0x000B9A8D File Offset: 0x000B7C8D
			public static PerkObject WellStocked
			{
				get
				{
					return NavalPerks.Instance._wellStocked;
				}
			}

			// Token: 0x1700043D RID: 1085
			// (get) Token: 0x06001C8B RID: 7307 RVA: 0x000B9A99 File Offset: 0x000B7C99
			public static PerkObject NavalHorde
			{
				get
				{
					return NavalPerks.Instance._navalHorde;
				}
			}

			// Token: 0x1700043E RID: 1086
			// (get) Token: 0x06001C8C RID: 7308 RVA: 0x000B9AA5 File Offset: 0x000B7CA5
			public static PerkObject Optimization
			{
				get
				{
					return NavalPerks.Instance._optimization;
				}
			}

			// Token: 0x1700043F RID: 1087
			// (get) Token: 0x06001C8D RID: 7309 RVA: 0x000B9AB1 File Offset: 0x000B7CB1
			public static PerkObject GildedPurse
			{
				get
				{
					return NavalPerks.Instance._gildedPurse;
				}
			}

			// Token: 0x17000440 RID: 1088
			// (get) Token: 0x06001C8E RID: 7310 RVA: 0x000B9ABD File Offset: 0x000B7CBD
			public static PerkObject VeteransWisdom
			{
				get
				{
					return NavalPerks.Instance._veteransWisdom;
				}
			}

			// Token: 0x17000441 RID: 1089
			// (get) Token: 0x06001C8F RID: 7311 RVA: 0x000B9AC9 File Offset: 0x000B7CC9
			public static PerkObject ShipwrightsInsight
			{
				get
				{
					return NavalPerks.Instance._shipwrightsInsight;
				}
			}

			// Token: 0x17000442 RID: 1090
			// (get) Token: 0x06001C90 RID: 7312 RVA: 0x000B9AD5 File Offset: 0x000B7CD5
			public static PerkObject SpecialArrows
			{
				get
				{
					return NavalPerks.Instance._specialArrows;
				}
			}

			// Token: 0x17000443 RID: 1091
			// (get) Token: 0x06001C91 RID: 7313 RVA: 0x000B9AE1 File Offset: 0x000B7CE1
			public static PerkObject SmoothOperator
			{
				get
				{
					return NavalPerks.Instance._smoothOperator;
				}
			}

			// Token: 0x17000444 RID: 1092
			// (get) Token: 0x06001C92 RID: 7314 RVA: 0x000B9AED File Offset: 0x000B7CED
			public static PerkObject AccuracyTraining
			{
				get
				{
					return NavalPerks.Instance._accuracyTraining;
				}
			}

			// Token: 0x17000445 RID: 1093
			// (get) Token: 0x06001C93 RID: 7315 RVA: 0x000B9AF9 File Offset: 0x000B7CF9
			public static PerkObject EfficientCaptain
			{
				get
				{
					return NavalPerks.Instance._efficientCaptain;
				}
			}

			// Token: 0x17000446 RID: 1094
			// (get) Token: 0x06001C94 RID: 7316 RVA: 0x000B9B05 File Offset: 0x000B7D05
			public static PerkObject PopularCaptain
			{
				get
				{
					return NavalPerks.Instance._popularCaptain;
				}
			}

			// Token: 0x17000447 RID: 1095
			// (get) Token: 0x06001C95 RID: 7317 RVA: 0x000B9B11 File Offset: 0x000B7D11
			public static PerkObject PortAuthority
			{
				get
				{
					return NavalPerks.Instance._portAuthority;
				}
			}

			// Token: 0x17000448 RID: 1096
			// (get) Token: 0x06001C96 RID: 7318 RVA: 0x000B9B1D File Offset: 0x000B7D1D
			public static PerkObject BlessingsOfTheSea
			{
				get
				{
					return NavalPerks.Instance._blessingsOfTheSea;
				}
			}

			// Token: 0x17000449 RID: 1097
			// (get) Token: 0x06001C97 RID: 7319 RVA: 0x000B9B29 File Offset: 0x000B7D29
			public static PerkObject ShipwrightsHand
			{
				get
				{
					return NavalPerks.Instance._shipwrightsHand;
				}
			}

			// Token: 0x1700044A RID: 1098
			// (get) Token: 0x06001C98 RID: 7320 RVA: 0x000B9B35 File Offset: 0x000B7D35
			public static PerkObject Salvage
			{
				get
				{
					return NavalPerks.Instance._salvage;
				}
			}

			// Token: 0x1700044B RID: 1099
			// (get) Token: 0x06001C99 RID: 7321 RVA: 0x000B9B41 File Offset: 0x000B7D41
			public static PerkObject MerchantFleet
			{
				get
				{
					return NavalPerks.Instance._merchantFleet;
				}
			}

			// Token: 0x1700044C RID: 1100
			// (get) Token: 0x06001C9A RID: 7322 RVA: 0x000B9B4D File Offset: 0x000B7D4D
			public static PerkObject Resilience
			{
				get
				{
					return NavalPerks.Instance._resilience;
				}
			}

			// Token: 0x1700044D RID: 1101
			// (get) Token: 0x06001C9B RID: 7323 RVA: 0x000B9B59 File Offset: 0x000B7D59
			public static PerkObject NavalBombardment
			{
				get
				{
					return NavalPerks.Instance._navalBombardment;
				}
			}
		}

		// Token: 0x0200028F RID: 655
		public class Shipmaster
		{
			// Token: 0x1700044E RID: 1102
			// (get) Token: 0x06001C9D RID: 7325 RVA: 0x000B9B6D File Offset: 0x000B7D6D
			public static PerkObject MasterAngler
			{
				get
				{
					return NavalPerks.Instance._masterAngler;
				}
			}

			// Token: 0x1700044F RID: 1103
			// (get) Token: 0x06001C9E RID: 7326 RVA: 0x000B9B79 File Offset: 0x000B7D79
			public static PerkObject OldSaltsTouch
			{
				get
				{
					return NavalPerks.Instance._oldSaltsTouch;
				}
			}

			// Token: 0x17000450 RID: 1104
			// (get) Token: 0x06001C9F RID: 7327 RVA: 0x000B9B85 File Offset: 0x000B7D85
			public static PerkObject GhostShip
			{
				get
				{
					return NavalPerks.Instance._ghostShip;
				}
			}

			// Token: 0x17000451 RID: 1105
			// (get) Token: 0x06001CA0 RID: 7328 RVA: 0x000B9B91 File Offset: 0x000B7D91
			public static PerkObject WindRider
			{
				get
				{
					return NavalPerks.Instance._windRider;
				}
			}

			// Token: 0x17000452 RID: 1106
			// (get) Token: 0x06001CA1 RID: 7329 RVA: 0x000B9B9D File Offset: 0x000B7D9D
			public static PerkObject RiverRaider
			{
				get
				{
					return NavalPerks.Instance._riverRaider;
				}
			}

			// Token: 0x17000453 RID: 1107
			// (get) Token: 0x06001CA2 RID: 7330 RVA: 0x000B9BA9 File Offset: 0x000B7DA9
			public static PerkObject NightRaider
			{
				get
				{
					return NavalPerks.Instance._nightRaider;
				}
			}

			// Token: 0x17000454 RID: 1108
			// (get) Token: 0x06001CA3 RID: 7331 RVA: 0x000B9BB5 File Offset: 0x000B7DB5
			public static PerkObject Windborne
			{
				get
				{
					return NavalPerks.Instance._windborne;
				}
			}

			// Token: 0x17000455 RID: 1109
			// (get) Token: 0x06001CA4 RID: 7332 RVA: 0x000B9BC1 File Offset: 0x000B7DC1
			public static PerkObject ShockAndAwe
			{
				get
				{
					return NavalPerks.Instance._shockAndAwe;
				}
			}

			// Token: 0x17000456 RID: 1110
			// (get) Token: 0x06001CA5 RID: 7333 RVA: 0x000B9BCD File Offset: 0x000B7DCD
			public static PerkObject TheHelmsmansShield
			{
				get
				{
					return NavalPerks.Instance._theHelmsmansShield;
				}
			}

			// Token: 0x17000457 RID: 1111
			// (get) Token: 0x06001CA6 RID: 7334 RVA: 0x000B9BD9 File Offset: 0x000B7DD9
			public static PerkObject RavenEye
			{
				get
				{
					return NavalPerks.Instance._ravenEye;
				}
			}

			// Token: 0x17000458 RID: 1112
			// (get) Token: 0x06001CA7 RID: 7335 RVA: 0x000B9BE5 File Offset: 0x000B7DE5
			public static PerkObject FairWinds
			{
				get
				{
					return NavalPerks.Instance._fairWinds;
				}
			}

			// Token: 0x17000459 RID: 1113
			// (get) Token: 0x06001CA8 RID: 7336 RVA: 0x000B9BF1 File Offset: 0x000B7DF1
			public static PerkObject FavorableTide
			{
				get
				{
					return NavalPerks.Instance._favorableTide;
				}
			}

			// Token: 0x1700045A RID: 1114
			// (get) Token: 0x06001CA9 RID: 7337 RVA: 0x000B9BFD File Offset: 0x000B7DFD
			public static PerkObject Unflinching
			{
				get
				{
					return NavalPerks.Instance._unflinching;
				}
			}

			// Token: 0x1700045B RID: 1115
			// (get) Token: 0x06001CAA RID: 7338 RVA: 0x000B9C09 File Offset: 0x000B7E09
			public static PerkObject ShoreMaster
			{
				get
				{
					return NavalPerks.Instance._shoreMaster;
				}
			}

			// Token: 0x1700045C RID: 1116
			// (get) Token: 0x06001CAB RID: 7339 RVA: 0x000B9C15 File Offset: 0x000B7E15
			public static PerkObject FleetCommander
			{
				get
				{
					return NavalPerks.Instance._fleetCommander;
				}
			}

			// Token: 0x1700045D RID: 1117
			// (get) Token: 0x06001CAC RID: 7340 RVA: 0x000B9C21 File Offset: 0x000B7E21
			public static PerkObject ChainToOars
			{
				get
				{
					return NavalPerks.Instance._chainToOars;
				}
			}

			// Token: 0x1700045E RID: 1118
			// (get) Token: 0x06001CAD RID: 7341 RVA: 0x000B9C2D File Offset: 0x000B7E2D
			public static PerkObject Stormrider
			{
				get
				{
					return NavalPerks.Instance._stormrider;
				}
			}

			// Token: 0x1700045F RID: 1119
			// (get) Token: 0x06001CAE RID: 7342 RVA: 0x000B9C39 File Offset: 0x000B7E39
			public static PerkObject MasterAndCommander
			{
				get
				{
					return NavalPerks.Instance._masterAndCommander;
				}
			}

			// Token: 0x17000460 RID: 1120
			// (get) Token: 0x06001CAF RID: 7343 RVA: 0x000B9C45 File Offset: 0x000B7E45
			public static PerkObject TheCorsairsEdge
			{
				get
				{
					return NavalPerks.Instance._theCorsairsEdge;
				}
			}

			// Token: 0x17000461 RID: 1121
			// (get) Token: 0x06001CB0 RID: 7344 RVA: 0x000B9C51 File Offset: 0x000B7E51
			public static PerkObject SeaborneFortress
			{
				get
				{
					return NavalPerks.Instance._seaborneFortress;
				}
			}

			// Token: 0x17000462 RID: 1122
			// (get) Token: 0x06001CB1 RID: 7345 RVA: 0x000B9C5D File Offset: 0x000B7E5D
			public static PerkObject Commodore
			{
				get
				{
					return NavalPerks.Instance._commodore;
				}
			}
		}
	}
}
