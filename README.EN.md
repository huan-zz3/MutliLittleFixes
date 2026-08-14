![](https://images.mountblade.top/forum/202608/11/094232jvbnvx9s8ax8tt9h.png)

> English name: MutliLittleFixes
> Chinese name: 许多小修小补
> Author: Huanzze (幻酌大大)

**Version Notes & Prerequisites**

- Current version: v1.2.0 (see changelog at the end)
- Requires the "Four Prerequisites" (framework mods)
- Tested compatible with 1.4.5 and 1.4.6; 1.4.7 and 1.4.8 are unconfirmed — feedback in the comments is appreciated. 1.3.x is **not** supported.

**Community & Bug Feedback QQ Group:** 
**658975856** — New group, free to join.

**MutliLittleFixes Core Features**

- **No-Ammo Ranged to Formation 9:** Ranged soldiers who have exhausted their ammo are automatically moved into Formation 9 to stand by; press 9 to select this group for independent command; they automatically rejoin their formation once ammo is restored — no more wasted shots.
- **NPC Lord Party Enhancement:** Phantom reinforcements and lost-settlement bonuses, greatly boosting weak kingdoms' ability to fight back.
- **Combat & Formations:** Improved battlefield formation layout — front-line infantry crouch to form a full shield wall; archers crouch to give teammates room to shoot.
- **Vanilla Kingdom AI Playability Fixes:** Disable AI declarations of war, player's sieges always a candidate, forbid clan party conscription, forbid clan parties donating troops.
- **Grain Support Between Player Fiefs:** Every 3 in-game hours, surplus towns automatically send garrison-converted grain convoys to player-clan towns/castles that are short on grain.
- **Village Funded Reconstruction:** Razed villages can be rebuilt by the player for 10,000 denars, completing after three days and raising relation with the village's notables.

**MutliLittleFixes Full Feature Guide**
**I. Progression & Experience**

- **Experience Multiplier:** The protagonist can gain up to 1000× experience, affecting all skill XP gain and character leveling speed — great for fast leveling.
- **Attribute Bonus Learning Multiplier:** The six attributes (Vigor, Control, Endurance, Cunning, Social, Intelligence) each have an independently configurable learning multiplier; the bonus growth of their corresponding skills doesn't interfere with one another.
- **Skill Level Cap:** Set separate level caps (10\~1024, with 1024 being the vanilla hard cap) for the skills under each of the six attributes, plus a global default cap — breaking the vanilla 275-level growth ceiling.

**II. Kingdom & Diplomacy**

- **Disable AI Auto Declaration of War:** When the player is king, vassal lords no longer automatically initiate declaration-of-war decisions, preventing the AI from dragging the kingdom into war on its own.
- **Player's Sieges Always a Candidate:** Towns conquered by the player personally leading the army (mere participation doesn't count) are always included in the fief-granting vote, never silently assigned to another lord.
- **Forbid Clan Party Conscription:** Prevents AI lords from conscripting non-protagonist parties of the player's clan into armies, protecting parties deployed around the map.
- **Forbid Clan Parties Donating Troops:** Prevents non-protagonist parties of the player's clan from automatically donating troops to garrisons, so AI-managed parties don't waste their manpower.

**III. Lords & Parties**

- **NPC Clan Party Count Bonus:** Adds extra deployed parties on top of the vanilla party-count cap for all AI lord clans (doesn't affect the player's clan), making NPC kingdoms feel more alive throughout the game.
- **Lord Release Replenishment:** After releasing a captured lord, their party is automatically replenished after a configurable number of days; troop composition ratios can be set per tier group (Tier 1-2, Tier 3-4, Tier 5-6), and gold and grain are granted to prevent starvation losses; if the party hasn't been formed by the abandon-days limit, replenishment is dropped.
- **Territory Loss Compensation:** Each time a kingdom loses a town or castle, its party cap increases by a configurable amount as compensation; consecutive losses decay the bonus; settlements held beyond a set number of days count as native territory and no longer cancel compensation, and settlements lost too long ago are excluded from the calculation; optionally applies to vassals only. The companion "Nation Bonuses" tab in the kingdom management screen shows each kingdom's current compensation values.
- **Nation Bonuses Tab:** A new "Nation Bonuses" tab in the kingdom management screen, showing each kingdom's current territory-compensation values in real time, linked to the territory-loss compensation feature.
- **Exiled Clans Never Die Out:** Removes the 28-day survival countdown extinction mechanic for landless exiled clans (clans wandering after their kingdom is destroyed), letting them persist forever until they join another kingdom or gain territory; when disabled, the vanilla countdown extinction is restored.
- **Removing a Captive Grants Relation:** When a hero captive is directly removed from the party roster (dragged out to the left) in the party screen, relation increases just like releasing them through vanilla dialogue — relation no longer only rises via dialogue release.

**IV. Recruitment & Supply**

- **Recruit Replenishment Multiplier:** The daily chance for town/village notables to replenish recruitable soldiers can be amplified up to 5×, so recruiting no longer means sitting around waiting.
- **Volunteer Upgrade Chance Multiplier:** The daily upgrade chance of notables' soldiers can be amplified up to 100×; the vanilla's extremely slow upgrade rate (a level-2 notable with 30 influence upgrades about 5% per day) can be pushed to near-certain upgrades.
- **Custom Deployment Ratio:** With the game setting "Unit Spawn Priority = High-Tier First" enabled, the spawn cadence is scheduled by four quota weights — infantry, archers, cavalry, and horse archers; within each troop type, units still spawn from highest to lowest tier, preventing high-tier units from hogging all spawn slots.

**V. Fiefs & Grain**

- **Grain Support Between Player Fiefs:** Every 3 in-game hours, surplus towns of the player's clan are checked and automatically send grain convoys — formed by converting garrison troops (randomly drawn, half high-tier and half low-tier) — to player-clan towns/castles that are short on grain. Grain is deducted directly from the source town on departure and added directly to the target on arrival, bypassing market consumption/conversion. The convoys have no commander and keep the vanilla caravan-style AI (they flee from strong enemies like caravans do); they wait in place if the target is under siege, return with a full refund if the target changes hands, and disband if the source town is lost. The shortage/supportable thresholds, convoy size, grain per soldier, per-town support cap, and per-town dispatch cap are all independently configurable.
- **Grain Convoys Globally Visible:** Grain support convoys are visible on the campaign map from any distance, ignoring line of sight and war fog — displayed like clan armies with a flagged nameplate at all times, so you can track in-transit deliveries at a glance; also fixes the bug where convoys got stuck mid-route.
- **Village Funded Reconstruction:** For completely razed (ruined) villages, the "Fund Reconstruction" option is available in the "Razed Village" menu: spend 10,000 denars and the village is automatically rebuilt after 3 days — its health refills and it returns to normal operation (triggering the vanilla recovery flow +20 militia), and all of the village's notables gain 25~35 relation. After paying, the button immediately turns into a disabled "Reconstruction in Progress" state, preventing the same village from being funded repeatedly. When disabled, the menu option is hidden, while reconstructions already in progress are unaffected and still complete on time.

**VI. Combat & Formations**

- **Auto Crouch:** Pure-infantry/pure-ranged squads automatically crouch while holding in the Hold (stationary) state — the front line of line formations, the front half of ranged formations, and all ranged units in loose formation crouch, reducing the chance of being hit by ranged fire.
- **Raise Shields When Crouching:** Front-line soldiers raise their shields upward (instead of guarding low) when crouching, forming a more sensible defensive posture together with auto crouch.
- **Banner Bearer Positioning:** Moves the banner bearer from the far-left front to the middle of the back row, keeping the bearer alive longer and the party bonus active longer.
- **No-Ammo Ranged to Formation 9:** Ranged soldiers who have run out of ammo are automatically moved into Formation 9 to stand by; press 9 to select this group for independent command; they automatically rejoin once ammo is restored — no more wasted shots.
- **Mounted Polearm Guaranteed Knockdown:** Couched lance (passive attack) and ordinary mounted polearm thrusts that hit dismounted infantry/ranged units always knock them down — symmetric for both sides and blockable; the thrust also has two tunable parameters: a minimum relative-speed threshold (so stabbing in place doesn't knock down) and a damage bonus on knockdown.
- **Battle Results Sort Order Reversal:** The sort cycle when clicking column headers on the results screen is reversed to "Default → Descending → Ascending", matching most players' intuition.
- **Free Retreat After Joining a Battle:** After the player joins an existing friendly battle on the campaign map, the encounter menu always offers a "Leave" option regardless of whether the friend is the attacker or defender, so you can pull out with your party at any time; self-initiated siege/defense battles keep the vanilla rules.
- **No Party Takeover on Player Death:** When the player character dies, the system is prevented from forcing the player's party under full AI command — the party keeps fighting with the last order given at the moment of death; note that the vanilla command UI is still closed after death, so no further manual orders are possible. When disabled, the vanilla "AI takes full control on death" behavior is restored. (Off by default)

**VII. Sieges & Naval Combat**

- **Siege Engines Prioritize Enemy Engines:** When the player is attacking, siege engines prioritize enemy engines, eliminating the most threatening firepower first.
- **Projectile Trajectory Preview:** Individually toggleable trajectory previews for ballistae/scorpions and catapults/trebuchets, so you can see where shots will land before firing. Middle mouse button switches to the global view.
- **Coordinate-Targeted AI Artillery:** After marking a target coordinate with the period key (.), AI-controlled siege engines will volley at the marked point, concentrating AI fire on the designated area; the mark point is automatically raised 1.5 meters, fixing the problem of projectiles hitting the ground early and landing short.
- **Player Artillery Precision:** Corrects the trajectory error when the player manually operates siege engines, making manual shots hit exactly where you aim.
- **Naval Battle Ship Cap:** The maximum number of ships the player can field simultaneously in naval battles/coastal raids is adjustable (3\~8 ships); requires the Naval DLC (战帆) — this feature is safely skipped when the DLC isn't installed.

**VIII. Saves & Notifications**

- **Saves Named by Date & Time:** Quick Saves and Auto Saves are named with the date/time of saving; each campaign rotates independently, and once full, saving a new file automatically retires that campaign's oldest save; Save As and Ironman mode keep the vanilla logic, and previously generated date-named saves are not auto-deleted when the feature is disabled.
- **Clan Member Available Alert:** When a clan member becomes available after being released from captivity or escaping, a toast notification pops up on screen — no more manually combing through the roster.

**IX. UI**

- **Special NPC Labels for Prisoners:** In the Prisoners tab of the party screen, rulers, lords, and mercenary leaders are labeled with their identity, so you can tell at a glance who's worth keeping.
- **Encyclopedia Clan Exile Filter:** The "Status" filter group on the encyclopedia's clan list gains "In Exile / Not in Exile" filters; "exile" means a clan with no kingdom, no settlement, and not a rebel/bandit/minor faction (excluding the player's clan), making it easy to track clans wandering after their kingdom's fall.
- **Bilingual EN/CN Support:** All player-visible text (MCM settings and hints, in-game menu options, notification messages, etc.) is integrated with the game's native localization system, automatically switching between English and Chinese with the game language; untranslated entries fall back to English.

**Acknowledgments to Predecessor Mods:** 
Throughout the journey of developing this mod, I've learned from countless mods along the way — some for the creativity I admired and reimplemented in my own way, others for the technical ingenuity that solved puzzles that had troubled me for a long time. Every generation brings new talents, and here I list all the predecessors who have had a direct influence on this mod, as a token of gratitude.

- Catapult Guide: A mod I love dearly — it provides an intuitive aiming method for players like me who aren't great at operating siege engines. Its shortcoming is that the aiming trajectory is affected by DLSS offset. The mindset of optimizing this mod sparked my journey of making Mount & Blade mods.
- Battlefield UI: The real-time entity creation technique it uses for in-battle health bars and name displays greatly inspired me, which led to fixing the DLSS offset issue in the projectile trajectory.
- RTSCamera.CommandSystem: Its fame needs no introduction. It inspired me to group no-ammo archers into Formation 9 and provided the technical reference implementation.
- FormationFilter: Of the same origin as RTSCamera — no need to elaborate.
- AnimusForge: A uniquely distinctive AI mod and a favorite among veteran and new M&B players alike — a true gem standing alongside AI Effect. It inspired me to add the Kingdom → Nation Bonuses tab.
- Quick Cover Retreat (快速掩护撤退): Inspired the "Free Retreat After Joining a Battle" feature.
- Enemy Party Enhancement (敌人部队增强): Inspired the "Lord Release Replenishment and Lost-Settlement Bonuses" features.
- Village Rebuild (村庄重建): Inspired me to recreate the "Fund Reconstruction" feature.

This mod is also open source on GitHub — any fellow player is welcome to reference and create: huan-zz3/MutliLittleFixes

![](https://images.mountblade.top/forum/202608/11/093816geghyett2sq0khhe.png)

**Changelog**

**v1.2.0** (current version)
- New feature: First-row shield-bearer formation fix — fixes the vanilla formation-layout convergence flaw, guaranteeing shield-bearers/pikemen land in the front row whenever a swappable unit exists, eliminating the "non-shield front row + shielded second row" mess; sorting anomalies automatically fall back to vanilla logic, avoiding crashes and stutters.
- New feature: Village funded reconstruction — the "Fund Reconstruction" option appears in the menu of completely razed villages: pay 10,000 denars and the village is automatically rebuilt after 3 days (village returns to normal + 20 militia + 25~35 relation with all village notables).
- Improvement: Village reconstruction can't be funded twice — after paying, the button immediately turns into a disabled "Reconstruction in Progress" state, preventing the same village from being funded repeatedly within the same menu session.
- New feature: Removing a captive from the party screen grants relation — when a hero captive is dragged out of the roster and removed, relation increases by 4 just like releasing them through dialogue.
- Improvement: Grain convoys globally visible, stuck-convoy bug fixed — grain convoys are now visible on the campaign map at any distance (ignoring line of sight and war fog, shown with a flagged nameplate in army style), and the bug where convoys got stuck mid-route is fixed.
- Bilingual support: EN/CN localization — all player-visible text is integrated with the game's native localization system, automatically switching between English and Chinese.

**v1.1.0**
- New feature: Grain support between player fiefs — surplus towns of the player's clan automatically send garrison-converted grain convoys to grain-short towns/castles; grain is added/deducted directly without going through the market, bundled with the "No Over-Capacity Slowdown for Grain Convoys" optimization.
- Improvement: Removed the hard speed cap of grain convoys — the convoy party cap is topped up, eliminating the vanilla over-capacity slowdown penalty.
- Improvement: Trebuchet targeting position raised 1.5 m on the Z axis — the marked point is automatically raised, fixing AI-targeted shots landing short.
- New feature: Encyclopedia clan exile filter — the "Status" filter group on the clan list gains "In Exile / Not in Exile" filter options.
- New feature: Exiled clans never die out — removes the 28-day survival countdown extinction for exiled clans, letting them persist forever.
- New feature: No party takeover on player death — the party keeps fighting with its last order after the player dies, not taken over by AI.

**v1.0** (initial release)
- Initial release. Core features: no-ammo ranged to Formation 9, auto crouch & shield wall, mounted polearm guaranteed knockdown, battle results sort order reversal, free retreat after joining a battle; siege engines prioritize enemy engines, projectile trajectory preview, coordinate-targeted AI artillery, player artillery precision, naval battle ship cap; experience multiplier, attribute bonus learning multiplier, skill level caps; NPC clan party count bonus, lord release replenishment (incl. grain food), territory loss compensation & Nation Bonuses tab, custom deployment ratio, recruit replenishment multiplier, volunteer upgrade chance multiplier; disable AI auto declaration of war, player's sieges always a candidate, forbid clan party conscription & troop donation; saves named by date & time, clan member available alert, special NPC labels for prisoners.
