# Tired Dad — Unity 6 Project Setup

Drop-in C# for the modernized **Tired Dad**. These scripts implement Phase A
(modernized core) + Phase B (random level generation) from `../FRAMEWORK.md`.
They're written for **Unity 6 LTS** and are designed to run as a **primitives
parity build** even before any art exists.

---

## ⚡ Fastest path — play the demo in ~5 minutes (recommended)

You do **not** need to wire a scene or create any assets. One component builds
everything at runtime (camera, father, HUD, all systems, all 30 items, a level).

1. **Unity Hub → New Project → 3D (URP) → Unity 6 LTS.**
2. **Package Manager → install *Input System*.** When prompted about the backend,
   click **Yes** (sets Active Input Handling; the editor restarts).
3. Copy this **`Assets/Scripts`** folder into the project's `Assets/`.
4. Create an empty scene. Add an empty GameObject, name it `Bootstrap`, and add the
   **`DemoBootstrap`** component to it (Add Component → search "DemoBootstrap").
5. Press **Play.**

**Controls (Editor):** click the mouse *in rhythm* — faster than once every 2s,
slower than once every 0.5s — to walk. **Click-drag left/right** to turn 90°.
On a phone build these are taps and swipes. Reach the changing table when the baby
needs a diaper. Get **Sleepiness** to the goal to win; then tap once for a new
randomly generated level. `DemoBootstrap ▸ fixedSeed` (non-zero) reproduces a level.

> That's the whole demo. Everything below is the manual/production setup for when
> you start adding real art, TextMeshPro UI, and authored assets.

---

## Manual / production setup (optional — for authored assets & polished UI)

## 1. Create the project
1. Unity Hub → New Project → **3D (URP)** → Unity 6 LTS.
2. Copy this `Assets/Scripts` folder into the new project's `Assets/`.

## 2. Required packages (Window ▸ Package Manager)
- **Input System** (`com.unity.inputsystem`) — then Edit ▸ Project Settings ▸ Player ▸
  *Active Input Handling* → **Input System Package (New)** (or Both). Restart when prompted.
- **TextMeshPro** — import Essentials when asked.
- URP is included with the 3D (URP) template.

## 3. Tags & layers (Project Settings ▸ Tags and Layers)
- Add tag **`Player`** and assign it to the father object.
- Add tag **`ChangingTable`** (RoomLayout assigns it to the diaper station).

## 4. Scene wiring (a single `Game` scene is enough to start)
Create these objects:

**Player** (the father)
- Add `CharacterController`, `TouchInputController`, `PlayerMover`.
- Tag = `Player`. In `PlayerMover`, assign `input` = its `TouchInputController`.

**GameManager** (empty object)
- Add `GameManager`, `SpawnSystem`, `NeedsSystem`, `LevelGenerator`.
- Assign in `GameManager`: `player`, `levelGenerator`, `spawnSystem`, `needsSystem`,
  `hud`, and a `LevelDefinition` asset (step 5).
- In `LevelGenerator`: assign `player`, and optionally `levelRoot` (empty object) and
  `playerStart` (empty at room center). If left empty they're auto-created / defaulted.

**HUD** (Canvas)
- Add a `Canvas` + `HUDController`. Create TMP labels for tiredness, sleepiness,
  morale, guilt, needs, time, score, level, ending — assign the ones you want.
  Fill-bar `Image`s are optional (v0.5).

**ScreenFlow** (on the ending overlay or GameManager)
- Add `ScreenFlow`, assign `gameManager`. One-finger tap = next level, two = menu.

## 5. Create data assets
- **Items:** Assets ▸ Create ▸ **TiredDad ▸ Item**. One asset per object. Set
  category and the stat deltas (positive raises a stat, negative lowers it). Examples:
  - *Coffee* → Positive, `fatherTiredness = -20`.
  - *Cry* → Negative, `morale = +15`.
  - *Bottle* → Positive, `clearsNeed = true`, `needCleared = Food`, `needs = -20`.
- **Level:** Assets ▸ Create ▸ **TiredDad ▸ Level Definition**. Drag your item
  assets into `positives` / `negatives` / `halfAndHalf`. Tune easy/hard endpoints.
  Leave room/furniture prefabs empty to auto-build primitives for now.

## 6. Player Settings (for Android)
- Orientation → **Portrait**.
- Other Settings → **IL2CPP**, **ARM64** (Play Store requires 64-bit).
- Set a company/product name and bundle id.

## 7. Press Play
With no models assigned you'll get the parity build: coloured cylinders (good),
capsules (bad), spheres (mixed) spawning in a randomly generated room. Editor
mouse-click acts as a tap for quick testing.

---

## Script map
| Folder | Script | Role |
|--------|--------|------|
| Core | `DemoBootstrap` | **One-click demo** — builds the whole playable scene at runtime |
| Core | `CameraFollow` | Angled top-down camera that follows the father |
| Core | `GameManager` | Orchestrates a level; ticks systems; win/lose; scene flow |
| Core | `StatSystem` | The 5 stats, update rules, clamping, outcome check *(bugs fixed)* |
| Core | `ScoreManager` | Cumulative score + stat carry-over (persists across scenes) |
| Input | `TouchInputController` | Tap-tempo + swipe on the New Input System |
| Input | `PlayerMover` | CharacterController movement / 90° turns |
| Items | `ItemDefinition` (SO) | Data-driven item effects (replaces hardcoded offsets) |
| Items | `ItemDatabase` | Code-generated 30 items mapped from the Hebrew design table |
| Items | `ItemEffect` | Pickup component (replaces ~30 per-item scripts) |
| Items | `ChangingTableTrigger` | Clears the diaper need when the father reaches the table |
| Spawning | `SpawnTable` | Weighted pool + caps + area |
| Spawning | `SpawnSystem` | Timed, capped, collision-checked spawning |
| Spawning | `NeedsSystem` | Baby needs (food/diaper/attention) *(bands fixed)* |
| LevelGen | `DifficultySettings` | Resolved per-level tuning |
| LevelGen | `LevelDefinition` (SO) | Difficulty curve + content pools |
| LevelGen | `RoomLayout` | Builds room + validated furniture |
| LevelGen | `LevelGenerator` | **Random level generation** (room, spawn table, stats, seeded) |
| UI | `GameHud` | Base class so either HUD implementation can plug in |
| UI | `HUDController` | Polished TMP stat text/bars, ending prompt (production) |
| UI | `SimpleHUD` | Zero-setup legacy-Text HUD used by the demo (no TMP import) |
| UI | `ScreenFlow` | End-screen tap handling |

## Notes
- Call `ScoreManager.Instance.NewRun()` from your Main Menu "Play" button to reset a run.
- Set `GameManager.fixedSeed` to a non-zero value to reproduce an exact level (debugging/balance).
- The changing table is tagged `ChangingTable`; wire a trigger on it to call
  `NeedsSystem.ClearNeed(BabyNeed.Diaper)` when the father reaches it (diaper-change flow).
