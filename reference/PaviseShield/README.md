# Pavise Shield
**Version:** 1.0.0 | **For:** Mount & Blade II: Bannerlord 1.2+

Allows any troop carrying the Vlandian pavise shield to plant it in the ground during battle, locking them in place with their shield raised.

---

## How to Use

1. Select a formation containing Vlandian crossbowmen (or any troop with the pavise shield)
2. Press **F9** to deploy — troops will plant their shields and hold position
3. Press **F9** again to stand them back up and resume normal orders

If no formation is selected, F9 toggles all pavise bearers on your team at once.

---

## Behavior When Deployed

- Troops stop moving and hold their current position
- Shield remains active for normal blocking
- Troops will not attack or chase enemies while planted
- Dead or routed troops are automatically removed from deployed state

---

## Installation

1. Extract the `PaviseShield` folder into:
   ```
   ...\Mount & Blade II Bannerlord\Modules\
   ```
2. Enable **Pavise Shield** in the Bannerlord launcher under the Mods tab
3. Load order: after Native, SandBoxCore, Sandbox, StoryMode

---

## Requirements

- Mount & Blade II: Bannerlord 1.2 or later
- [Harmony](https://www.nexusmods.com/mountandblade2bannerlord/mods/2006) (Steam Workshop ID: 3596693285)

---

## Tuning

In `PaviseShieldMissionBehavior.cs`:
- `PAVISE_ITEM_ID` — change this to support other shield item IDs
- `DEPLOY_KEY` — change `InputKey.F9` to any other key
