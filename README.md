# Knights Project (RPG Prototype)

> **This is a prototype / proof-of-concept for a turn-based RPG built in Unity.** Systems described below are working but not production-ready.

---

## Character Creation

![Character Creation](preview/char_creation.png)

The character rig is built as a modular prefab where every face, hair, and mouth variant exists as a child GameObject. `CharCreationForm` stores selections into a `CharAvatar` data object, and on every change calls `BodyPartRenderer.SetBodyPart()` which deactivates all body part GameObjects, then looks up the selected hair/eye/mouth by name in a dictionary and activates only those. Hair color is solved by swapping the `MeshRenderer.material` from a `HairColorList` ScriptableObject that maps color names to materials. This approach avoids runtime mesh generation entirely — all visual customization is just toggling pre-existing GameObjects on and off.

---

## Skin & Armor Rendering

![Equipment](preview/equipments.png)

I used the same activate/deactivate pattern for equipment rendering. Every weapon and armor piece lives as a pre-placed child on the rig, and `EquipmentRenderer` tracks which GameObjects are currently active, deactivates them all on any equipment change, then looks up the new item's `GameObjectName` via `EquipmentList.GetSOGO()` and activates the matching GameObjects. To handle weapon-specific animations without a complex lookup, I used a naming convention — each `WeaponClass` maps to a suffix (`Sword` → `_THS`, `Wand` → `_MagicWand`, `Bow` → `_BowAndArrow`) and an extension method `WithWeapon()` appends it to any animation name. This way one call like `GetClipWithWeapon(PlayerAnims.Idle_Battle)` resolves to the correct clip for whatever weapon is equipped.

---

## Equipment Switching

Equipment switching is driven reactively. `ItemActionsUI` checks the selected item's `WeaponClass` and `ArmorPart` to decide which slot it goes into, writes it to `CharEquipment`, and saves to Firebase — the `CharEquipmentObs` observable then propagates the change automatically to `EquipmentRenderer` (visuals), `PlayerCombatHandler` (stats recomputation), and `PlayerAnimation` (weapon-appropriate idle). `PlayerCombatHandler.ComputeStatsFromEquipment()` reads the `ItemSO` off the equipment and directly sets `MaxHealth`, `Defense`, and `Damage` on the `CombatParticipant`. The entire chain from equip click to updated visuals + stats is one reactive subscription with no manual refresh calls.

---

## Skillbar & Skill Assignment

![Skills and Potions](preview/skill_system.png)

`SkillButtonUI` uses `Observable.CombineLatest` to merge the current equipment and the render trigger into a single stream, so any weapon change automatically re-evaluates whether each skill button should be enabled. Each button validates itself: active skills check if the equipped weapon's `WeaponClass` matches the skill's required class, and potion buttons check remaining quantity. On click, the button clones the current `CombatState`, sets `SelectedSkillBook`, determines the next phase based on `TargetType` (single-target goes to `TurnSelectTarget`, multi-target auto-populates targets and goes straight to `TurnConfirmAction`), and pushes the new state through `CombatController.SetCombatState()`. This keeps all skill selection logic self-contained in the button rather than in a central controller.

---

## Potions & Consumables

Potions reuse the `SkillBookSO` system with a `SkillType` of HPPotion or MPPotion instead of creating a separate consumable system. `PotionsUI` uses `Observable.CombineLatest` on the skillbar and skill books observables to always render the correct quantity count. When used in combat, `PlayerCombatHandler.PerformPotion()` calls `CombatController.UseConsumableSkillBook()` which decrements the quantity in the skill books list and saves to Firebase in one step. The heal itself is an `IRxState` (`TriggerHealState`) composed into the turn sequence — for multi-target heals, multiple `TriggerHealState` instances are merged in parallel via `CombineState(true, heals)`.

---

## Turn-Based Combat

![Combat - Skill Selection](preview/combat_part1.png)
![Combat - Target Selection](preview/combat_part2.png)

Combat state is managed through a single `BehaviorSubject<CombatState>` on `CombatController`, and phase transitions are processed as side effects via a debounced subscription in `ProcessSideEffects()`. Turn order is determined by concatenating all player and enemy `CombatParticipant`s and shuffling with `OrderBy(_ => random.Next())`, then cycling through with a `TurnIndex` that skips dead participants. Each skill's execution is built by composing small `IRxState` objects — for example, `BeginnerSlash` chains `DashToTargetState` → `IdleState` → `MeleeAttack01` → `DashBackHomeState` → `TurnEndState` via `CombineState`, which internally uses `Observable.Concat` for sequential or `Observable.Merge` for parallel playback. Every participant — player or enemy — subscribes to the same `CombatStateObs` and only acts when it's their turn and the phase matches, keeping the logic decentralized.

---

## List Rendering (Angular-Style ngFor)

`ListContainer` replicates Angular's `*ngFor` in Unity by separating population from rendering. `PopulateListItems<T>()` clones a single template UI element until the container has enough children, and `RenderItems<T, D>()` slices the data list by page number, calls a render callback for each slot, and passes `default` for empty slots. This lets any list-based UI (inventory, skill books, etc.) get pagination and data binding with just two calls — no per-screen duplication of instantiation or paging logic.

---

## Reactive State Machine

`RxStateMachine` uses a `BehaviorSubject<IRxState>` and `.Switch()` — pushing a new state automatically cancels whatever was playing and starts the new one. Each game action (dash, attack, heal, projectile, etc.) implements `IRxState.Play()` returning an `Observable<int>`, and `CombineState` composes them via `Observable.Concat` (sequential) or `Observable.Merge` (parallel). This means building a full combat turn is just `new CombineState(dash, idle, attack, backHome, idle, turnEnd)` — the reactive pipeline handles timing, ordering, and cancellation with no coroutines or update loops.
