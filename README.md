# Knights Project (RPG Prototype)

> **This is a prototype / proof-of-concept for a turn-based RPG built in Unity.** Systems described below are working but not production-ready.

---

## Character Creation

![Character Creation](preview/char_creation.png)

Players enter a name and customize their avatar by choosing from 12 face options (eye + mouth combos), multiple hair styles, and a set of hair colors applied via swappable materials. A `BodyPartRenderer` toggles the correct GameObjects on a modular character rig based on these selections. On submit, the `CharAvatar` data is saved to Firebase and the player receives a default set of beginner weapons, armor, and skills.

---

## Skin & Armor Rendering

![Equipment](preview/equipments.png)

Characters use a modular mesh approach where each equipment piece (weapon, armor) is a child GameObject on the player rig. `EquipmentRenderer` deactivates all currently visible equipment parts and activates the new ones whenever gear changes. A PBRMaskTint shader allows multiple color regions on a single mesh using grayscale masks, enabling visual variety across armor sets. Weapon switches also update the animation set automatically — each `WeaponClass` (Sword, Wand, Bow) maps to a naming suffix (e.g. `_THS`, `_MagicWand`, `_BowAndArrow`) so the correct idle, attack, and movement clips play for the equipped weapon.

---

## Equipment Switching

The inventory UI displays owned items in a paginated grid; selecting an item shows its stats and an Equip button. `ItemActionsUI` handles equip actions by updating `CharEquipment.Weapon` or `CharEquipment.Armor`, saving to Firebase, and triggering `EquipmentRenderer` to swap the visible GameObjects. Equipped items display a marker in the inventory grid. Combat stats (Damage, Defense, Health) are recomputed from the newly equipped `ItemSO` scriptable objects each time gear changes.

---

## Skillbar & Skill Assignment

![Skills and Potions](preview/skill_system.png)

The skillbar has 4 active-skill slots plus 2 dedicated potion slots (HP and MP), managed by `CharSkillBar` and rendered by `SkillbarUI` and `PotionsUI`. Players assign skills from their skill book to specific slots via the dashboard; each slot stores a `SkillBookSOId` reference. Skill buttons validate availability at render time — active skills check that the equipped weapon matches the skill's `WeaponClass`, and potions check remaining quantity. During combat, clicking a skill button sets the `CombatState.SelectedSkillBook` and transitions the phase to target selection or action confirmation depending on the skill's `TargetType`.

---

## Potions & Consumables

Potions are defined as `SkillBookSO` assets with a `SkillType` of HPPotion or MPPotion and a `HealAmount`. When used during a combat turn, `CombatController.UseConsumableSkillBook()` decrements the quantity in `CharSkillBooks` and persists it to Firebase. A `TriggerHealState` fires an `OnHealEvent` that restores HP (capped at max) or MP on the target, and a floating damage number in green or blue confirms the heal visually. Potion buttons grey out and become unusable once quantity reaches zero.

---

## Turn-Based Combat

![Combat - Skill Selection](preview/combat_part1.png)
![Combat - Target Selection](preview/combat_part2.png)

Combat begins when the player enters a `CombatZone` trigger; all player and enemy `CombatParticipant`s are collected and shuffled into a random turn order. On a player's turn the skillbar activates for skill/potion selection, then phases through target selection and action confirmation before executing the move. Skill execution is composed from chainable reactive states (`DashToTargetState`, `MeleeAttack01`, `RangeAttack01`, `TriggerHitState`, `DashBackHomeState`, etc.) orchestrated by `CombineState` for sequential or parallel playback. Enemies use simple AI — they pick a random player target and use their highest-mana-cost affordable attack. The battle ends when all enemies are defeated, triggering a victory animation and a loot reward screen.

---

## List Rendering (Angular-Style ngFor)

`ListContainer` is a generic utility that replicates Angular's `*ngFor` pattern in Unity. `PopulateListItems<T>()` clones a template UI element until the list has enough children to fill a page, and `RenderItems<T, D>()` takes a data list, slices it by page number, and calls a render callback on each child — passing `default` for empty slots. This gives any list-based UI (inventory grid, skill book, etc.) automatic pagination and data-driven rendering with just a few lines of setup. The pattern keeps list logic reusable across different UI screens without duplicating instantiation or paging code.

---

## Reactive State Machine

`RxStateMachine` manages complex animation and gameplay sequences using R3 (Rx.NET for Unity) with minimal boilerplate. It holds a `BehaviorSubject<IRxState>` — when a new state is pushed via `SetState()`, the observable chain calls `state.Play()` (which returns an `Observable<int>`) and uses `.Switch()` to automatically cancel any in-progress state. Each skill, movement, or combat action implements `IRxState`, and `CombineState` chains them sequentially or in parallel via Concat/Merge. This means orchestrating a full combat turn — dash, attack, hit, dash back — is just composing small state objects, and the reactive pipeline handles timing, cancellation, and sequencing automatically.
