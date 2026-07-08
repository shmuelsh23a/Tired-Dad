# Tired Dad — 3D Models

36 procedurally-generated low-poly meshes in Wavefront **`.obj`** format, each with a
shared **`tireddad.mtl`** colour material. Style: geometric "blockout" — recognizable,
lightweight (18–1000 tris each), ready to import and iterate on.

See `_preview_contact_sheet.png` in this folder for a render of all 36.

## What's included

```
Assets/Models/
├── Characters/   Father, Baby                     (+ tireddad.mtl)
├── Furniture/    Bed, Chair, Sofa, ChangingTable  (+ tireddad.mtl)
└── Items/        30 item models (Item_*.obj)       (+ tireddad.mtl)
```

The 30 items map 1:1 to the design table (`../../טבלת משימות.xlsx`) and to the
`ItemDatabase` names, e.g. `Item_Coffee`, `Item_Bottle`, `Item_Poop`, `Item_Cars`,
`Item_SleepingTiger`, `Item_LightsOn`.

## Conventions

- **Units:** metres, Y-up (Unity's convention). No import rotation needed.
- **Pivot / origin:**
  - Characters & furniture — base sits at **y = 0** (feet/legs on the floor).
  - Items — centred on the origin, roughly **0.6 m** tall (drop-in for the spawner).
- **Facing:** characters face **+Z** (matches `PlayerMover.transform.forward`).
- **Materials:** flat colours via `tireddad.mtl`. Unity auto-creates materials on import;
  you can replace them with URP/Lit materials or textures later.

## Import into Unity

1. The `Assets/Models` folder is already in the project — Unity imports the `.obj`
   files automatically and generates a model prefab + materials for each.
2. Select an imported model → set **Scale Factor = 1**, **Convert Units** off (they're
   already in metres). Enable **Generate Colliders** only if you want per-model collision
   (the game adds its own trigger colliders, so usually leave off).

## Use them in the one-click demo (automatic)

`DemoBootstrap` and `ItemDatabase` already try `Resources.Load<GameObject>("Models/<Name>")`
and fall back to primitives when nothing is found. To switch the demo from primitives to
these models, just make the models loadable from a **Resources** folder:

1. Create `Assets/Resources/Models/`.
2. Copy (or move) the `.obj` models so they resolve by name, e.g.
   `Assets/Resources/Models/Item_Coffee.obj`, `.../Bed.obj`, `.../Father.obj`, etc.
   (A flat folder is simplest — the loader looks up `Models/<Name>`.)
3. Press Play. Items, furniture, and the father now use the meshes; anything missing
   still falls back to a coloured primitive, so it can't break the build.

> Prefer manual control? Instead of Resources, assign each mesh to the matching
> `ItemDefinition.modelPrefab` (and the `LevelDefinition` furniture fields) in the
> Inspector once you author those as real assets.

## Regenerating / tuning the models

The generator and renderer live at the project root:

- `_gen_models.py <outdir>` — rebuilds every `.obj` + `.mtl`.
- `_render_preview.py` — rebuilds `_preview_contact_sheet.png`.

Edit a builder function (e.g. `coffee()`, `father()`) to change a shape, colour, or
proportion, then re-run. Colours live in the `PALETTE` dict. These are Python helpers,
not part of the Unity build (Unity ignores `.py`).

## Notes / next steps

- These are **blockout** meshes — great for playtesting and to hand to an artist as a
  scale/silhouette reference. For a polished look, replace or retopo later and add
  textures/URP materials.
- A few items are stylized symbols by design (Almost Asleep = "Zzz", Attention = "!",
  Must Be Easier = a heavy dumbbell) since they represent feelings/events, not objects.
