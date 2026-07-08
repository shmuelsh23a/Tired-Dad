# Tired Dad — Modernization & Completion Framework

**Status:** Pre-Alpha (v0.3) → target v1.0
**Engine:** Unity 5.0.0f4 (2015) → **Unity 6 LTS**
**Platform:** Android (primary), iOS (secondary)
**Genre:** Casual, single-hand mobile game
**Perspective:** 3D, top-down (unchanged)

---

## 1. Concept (unchanged)

You play a sleep-deprived father in a baby's room, trying to get the baby to fall asleep before you crack. You **tap the screen at a steady rhythm** to keep walking (faster than once/2s, slower than once/0.5s) and **swipe to turn 90°**. As you move you collect floating objects that push five stat bars up or down. Get the baby's Sleepiness to 100 and you win the level. Let your own Tiredness, Morale (depression), or the baby's unmet Needs run away from you and you lose.

The design intent, tone, and Hebrew content design (from `טבלת משימות.xlsx`) are preserved. This document covers **how to rebuild it cleanly in modern Unity and finish it**, plus a new **random level generation** system.

---

## 2. The Five Stats

| Stat | Hebrew | Behavior | Fail / Win condition |
|------|--------|----------|----------------------|
| **Baby Sleepiness** | הירדמות | Rises over time; rises *faster* while the father walks | **Win** at 100 ("Baby is asleep") |
| **Father Tiredness** | עייפות | Rises over time; *reduced* by walking | **Lose** at 100 ("You fell asleep") |
| **Morale** (depression) | מוראל | Creeps upward over time | **Lose** at 100 ("You are depressed") |
| **Needs** | צרכים | Baby randomly needs Food / Diaper / Attention | Drives Guilt; extreme neglect ends level |
| **Guilt** | אשמה | Scales off unmet Needs (higher Needs → faster Guilt) | Contributes to loss / low score |

**Scoring (per level):** `score += (100 − Needs − Guilt) + (100 − Morale) + (100 − FatherTiredness)`. Remaining Sleepiness rewards a clean finish. Levels chain: end-of-level stats seed the next level's starting stats.

---

## 3. Object Catalog

Three collectible categories plus furniture. In 3D top-down we **keep the shapes as fallback silhouettes** but replace them with real 3D models (per your decision).

### Positives (good — currently cylinders)
Coffee · Awake Anyway · Almost Asleep · Blanky · Calm Movement · Calm Music · Cute When Asleep · Happy · Hug · Lullaby · Pacifier · Smile

### Negatives (bad — currently capsules)
Attention · Barking Dogs · Cars · Cry · Food · It Is So Late · Loud Music · Must Be Easier · No Sleep · Poop · Toys · Tummy Hurts

### Half-and-Half (mixed — currently spheres)
Burp · Kuchy Kuch Ku · Lights On · Mom Is Asleep · Sleeping Tiger · Work Tomorrow

### Furniture
Bed · Chair · Sofa · Changing Table (used to change diapers when Needs = Poop)

Each object maps to a data-driven effect on one or more stats (see §6 ScriptableObjects).

---

## 4. Modernization Plan (Unity 5 → Unity 6 LTS)

The existing C# will **not compile in Unity 6** without changes. The plan below replaces deprecated APIs and restructures the code so it's maintainable and testable.

### 4.1 Hard blockers (must fix to even run)

| Old (Unity 5) | New (Unity 6) |
|---------------|---------------|
| `Application.LoadLevel("Score")` | `SceneManager.LoadScene("Score")` (`using UnityEngine.SceneManagement;`) |
| Legacy `Input.touchCount` / `Input.GetTouch` | **New Input System** package (`UnityEngine.InputSystem`, `Touchscreen.current`) |
| `Application.Quit()` on level end | keep, but gate behind a proper menu |
| Legacy UI `Text` | **TextMeshPro** (`TMP_Text`) |
| `Object.DontDestroyOnLoad` singleton pattern | keep, but move to a single clean `GameManager` (see §5) |

### 4.2 Code smells to clean up during the port

- **`Game.cs` is a 1,360-line god class.** Split into `GameManager`, `StatSystem`, `SpawnSystem`, `NeedsSystem`, `InputController`, `LevelGenerator` (see §5).
- **Duplicated scoring logic** exists both (commented) in `Game.cs` and live in `ScoreManager.cs`. Keep one source of truth in `StatSystem`/`ScoreManager`.
- **Dead/commented code** throughout `Game.cs`, `Walker.cs`, `Score.cs` — delete on migration.
- **Magic numbers everywhere** (offsets, thresholds) → move to ScriptableObjects (§6).

### 4.3 Known logic bugs to fix (found in current code)

1. **Guilt boundary gaps** — `GuiltMenager()` uses strict `>`/`<` on ranges (`needs > 10 && needs < 20`, etc.), so exact values 10, 20, 40, 60, 80, 90 fall through and update nothing. Use inclusive lower bounds / `else if` chains.
2. **Walker tempo dead branch** — condition `((touchBeganTime - prevTouchBeganTime) < 0.5f) && (... > 2)` can never be true (a number can't be both < 0.5 and > 2). Tempo-decrease-on-slow-tap never fires. Rewrite the tempo model against the new Input System.
3. **`RandomNeedsDecider()` missing range** — start-of-level needs randomizer has no case for the 76–100 band; `case 1` sets 0, `case 2` 10–50, `case 3` 51–75. Add full coverage.
4. **Bottle & Pacifier commented out of the positive spawner** — they only appear via the Needs system. Decide intended behavior and restore or document.
5. **`CalculateNewStats()` can call `Random.Range(10, prevValue)`** where `prevValue < 10`, throwing/《inverting the range. Clamp inputs.

### 4.4 Project settings

- Set **Portrait** orientation (mobile, one-handed).
- **Android**: min API level per current Play Store requirement; IL2CPP + ARM64 (Play Store mandates 64-bit).
- Switch rendering to **URP** (Universal Render Pipeline) for mobile performance and modern lighting.
- Enable the **Input System** package and set Active Input Handling to "Input System (New)".

---

## 5. Target Architecture

Single responsibility, data-driven, testable. All systems coordinated by one persistent `GameManager`.

```
/Assets
  /Scripts
    /Core
      GameManager.cs        // persistent singleton; owns game state, scene flow, level index
      StatSystem.cs         // the 5 stats: update rules, clamping, win/lose checks
      ScoreManager.cs       // per-level + cumulative score, carry-over stats
    /Input
      TouchInputController.cs   // New Input System: tap-tempo + swipe-to-turn
      PlayerMover.cs            // moves/rotates the father via CharacterController
    /Items
      ItemEffect.cs         // MonoBehaviour on each object; applies its ItemDefinition on pickup
      ItemDefinition.cs     // ScriptableObject: name, category, stat deltas
    /Spawning
      SpawnSystem.cs        // spawns items over time, respects caps & collision checks
      NeedsSystem.cs        // baby needs (food/diaper/attention) + pacifier/bottle spawns
    /LevelGen
      LevelGenerator.cs     // NEW: builds a random room + furniture + spawn table + goals
      LevelDefinition.cs    // ScriptableObject: difficulty curve parameters
      RoomLayout.cs         // room shape/size + furniture placement with validation
    /UI
      HUDController.cs      // stat bars, clock, score (TextMeshPro)
      ScreenFlow.cs         // start / help / level-end / credits screens
/ScriptableObjects
  /Items      (one asset per object, ~30 total)
  /Levels     (difficulty presets / generation ranges)
```

**Scene list:** `Boot` → `MainMenu` → `Game` → `LevelEnd` (→ loops to `Game`).

---

## 6. Data-Driven Item Effects (ScriptableObjects)

Replace the ~50 `float xxxOffset` fields in `Game.cs` with one asset per item:

```csharp
[CreateAssetMenu(menuName = "TiredDad/Item")]
public class ItemDefinition : ScriptableObject
{
    public string displayName;              // e.g. "Coffee"
    public string displayNameHe;            // e.g. "קפה"
    public ItemCategory category;           // Positive / Negative / HalfAndHalf
    public GameObject modelPrefab;          // 3D model (fallback primitive if null)

    // Stat deltas applied on pickup (positive = increases that stat)
    public float fatherTiredness;
    public float babySleepiness;
    public float morale;
    public float guilt;
    public float needs;

    public AudioClip pickupSound;           // §8 sound
}
```

`ItemEffect.OnTriggerEnter` reads its `ItemDefinition` and applies the deltas through `StatSystem` — no more per-item hardcoded methods. This makes **balancing (v0.4) a matter of editing assets**, not code.

---

## 7. Random Level Generation (NEW)

Per your spec, four things are randomized each level. Driven by `LevelGenerator` + a `LevelDefinition` difficulty curve keyed to the level index.

### 7.1 Room layout & furniture
- **Room**: pick a size within a range that grows/shrinks with difficulty (e.g. 8×10 → 12×14). Optionally vary shape (rectangular now; L-shape later).
- **Furniture**: always place a **Changing Table** on a random wall (as today), plus a random subset of {Bed, Chair, Sofa} at validated, non-overlapping positions using the existing raycast collision check (reused from `FrnitureSpawner`, but extracted and cleaned).
- Guarantee **navigability**: the father must be able to reach the changing table (simple reachability check on a grid).

### 7.2 Item spawn mix
- Each level draws from a **weighted spawn table** derived from `LevelDefinition`. Early levels weight Positives higher; later levels raise Negative/Half-and-Half weights.
- Caps per category (currently 5 each) become difficulty-scaled.
- Spawn interval and simultaneous-object count scale with difficulty.

### 7.3 Difficulty scaling
`LevelDefinition` exposes curves (or a formula) over the level index:

| Parameter | Easy (Lvl 1) | Hard (Lvl 10+) |
|-----------|--------------|----------------|
| Morale rise rate | slow | fast |
| Father tiredness rise | slow | fast |
| Negative spawn weight | low | high |
| Needs frequency | rare | frequent |
| Tempo tolerance window | wide | tight |
| Sleepiness gain per walk | high | lower |

Formula sketch: `value = Mathf.Lerp(easy, hard, Mathf.Clamp01(level / maxLevel))` with optional per-level jitter for variety.

### 7.4 Starting stats & goals
- Baby's **starting Sleepiness/Needs** randomized within difficulty-bounded ranges (replaces `RandomNeedsDecider`, with the missing-band bug fixed).
- **Target threshold** can vary: most levels win at Sleepiness 100, but "quick" levels might target 80 with tighter time, or a level might start the father partway tired.
- Carry-over: `ScoreManager` still seeds next level from end-of-level stats, but generation clamps them to the level's valid ranges.

### 7.5 Generation flow
```
GameManager.LoadNextLevel()
  → LevelGenerator.Generate(levelIndex, LevelDefinition)
      1. Build room (size/shape)
      2. Place furniture (validated, reachable)
      3. Build weighted spawn table
      4. Roll starting stats + goal
  → StatSystem.Init(startingStats, goal)
  → SpawnSystem.Begin(spawnTable, difficulty)
```

**Seeded runs:** pass an optional RNG seed so a level can be reproduced (useful for debugging balance and for "daily challenge" style features later).

---

## 8. Completion Roadmap (from your task table)

Ordered to unblock playtesting as early as possible. `[x]` = already built in the Unity 5 version.

### Phase A — Modernization (foundation)
- [ ] A1. Create Unity 6 LTS project, URP, Input System, TextMeshPro
- [ ] A2. Port & split `Game.cs` into Core systems (§5); fix hard blockers (§4.1)
- [ ] A3. Convert items to `ItemDefinition` ScriptableObjects (§6)
- [ ] A4. Rewrite input (tap-tempo + swipe) on the New Input System
- [ ] A5. Fix the known logic bugs (§4.3)
- [ ] A6. Get the existing gameplay running with primitives (parity build)

### Phase B — Random level generation (v0.4-new)
- [ ] B1. `LevelDefinition` ScriptableObjects + difficulty curves
- [ ] B2. `LevelGenerator`: room + furniture placement with validation
- [ ] B3. Weighted spawn tables + difficulty-scaled spawning
- [ ] B4. Randomized starting stats & goals + carry-over clamping
- [ ] B5. Seeded generation for reproducible testing

### Phase C — Balancing (v0.4)
- [ ] C1. Tune item deltas via ScriptableObjects
- [ ] C2. Tune walk speed, stat rise rates, scoring
- [ ] C3. Playtest the difficulty curve across ~10 levels

### Phase D — GUI (v0.5)
- [ ] D1. Stat **bars** (replace text) + clock + score HUD
- [ ] D2. "Lights On" screen-reveal effect / lighting states

### Phase E — Screens (v0.6)
- [ ] E1. Start screen · E2. Help screen · E3. Level-end screen · E4. Credits

### Phase F — Graphics (v0.7)
- [ ] F1. Father model + animations (idle/walk)
- [ ] F2. Baby + furniture models
- [ ] F3. Item models (30 objects) · F4. Room/environment art

### Phase G — Sound (v0.8)
- [ ] G1. Item pickup SFX · G2. Background music · G3. Walk/footstep SFX

### Phase H — Extras (v0.9)
- [ ] H1. Social/login · H2. User profile · H3. Achievements

### Phase I — Release (v1.0)
- [ ] I1. Android 64-bit build, store listing, icons/splash · I2. QA pass · I3. Publish

---

## 9. Immediate Next Steps

1. Confirm this framework.
2. I generate the **Phase A skeleton scripts** (GameManager, StatSystem, SpawnSystem, NeedsSystem, TouchInputController, PlayerMover, ItemDefinition, ItemEffect) plus the **LevelGenerator** (Phase B) as drop-in C# files for a fresh Unity 6 project.
3. You create the empty Unity 6 project; we drop scripts in, wire the scene, and get a primitives parity build running.
4. Iterate on balancing via ScriptableObjects.

---

*Source design references: `Readme.docx` (instructions), `טבלת משימות.xlsx` (version roadmap & Hebrew object/value design), original `Assets/Scripts/` (Game.cs, Walker.cs, ScoreManager.cs, Score.cs, and the Positives/Negatives/Half-and-Half item scripts).*
