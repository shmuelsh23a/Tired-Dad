using System.Collections.Generic;
using UnityEngine;

namespace TiredDad
{
    /// <summary>
    /// Builds the physical room each level: a floor, four walls sized to the
    /// difficulty's room footprint, and a validated, non-overlapping set of furniture
    /// (always a changing table on a random wall, plus a random subset of bed/chair/sofa).
    /// If prefabs are missing it falls back to primitives so a parity build still runs.
    /// Reuses the original's raycast-clearance idea, cleaned up into a sphere check.
    /// </summary>
    public class RoomLayout
    {
        public Bounds interior;                 // XZ area usable for spawning
        public readonly List<GameObject> spawned = new List<GameObject>();

        private const float WallHeight = 2.0f;
        private const float FurnitureClearance = 1.0f;

        public void Build(Transform parent, LevelDefinition def, float halfSize, System.Random rng)
        {
            Clear();

            // Floor.
            if (def.floorPrefab != null)
                Track(Object.Instantiate(def.floorPrefab, Vector3.zero, Quaternion.identity, parent));
            else
            {
                var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
                floor.name = "Floor";
                floor.transform.SetParent(parent);
                floor.transform.localScale = new Vector3(halfSize * 2f / 5f, 1f, halfSize * 2f / 5f);
                Track(floor);
            }

            // Four walls.
            BuildWall(parent, def, new Vector3(0, WallHeight / 2f,  halfSize), new Vector3(halfSize * 2f, WallHeight, 0.2f));
            BuildWall(parent, def, new Vector3(0, WallHeight / 2f, -halfSize), new Vector3(halfSize * 2f, WallHeight, 0.2f));
            BuildWall(parent, def, new Vector3( halfSize, WallHeight / 2f, 0), new Vector3(0.2f, WallHeight, halfSize * 2f));
            BuildWall(parent, def, new Vector3(-halfSize, WallHeight / 2f, 0), new Vector3(0.2f, WallHeight, halfSize * 2f));

            // Usable interior, inset from the walls.
            float inset = halfSize - 0.8f;
            interior = new Bounds(new Vector3(0, 0.5f, 0), new Vector3(inset * 2f, 1f, inset * 2f));

            // Changing table on a random wall (as in the original).
            PlaceChangingTable(parent, def, halfSize, rng);

            // Random subset of the remaining furniture.
            var pool = new List<GameObject> { def.bedPrefab, def.chairPrefab, def.sofaPrefab };
            Shuffle(pool, rng);
            int count = rng.Next(1, pool.Count + 1);   // 1..3 pieces
            for (int i = 0; i < count; i++)
                PlaceFreeFurniture(parent, pool[i], "Furniture", halfSize, rng);
        }

        private void BuildWall(Transform parent, LevelDefinition def, Vector3 pos, Vector3 size)
        {
            GameObject wall;
            if (def.wallPrefab != null)
            {
                wall = Object.Instantiate(def.wallPrefab, pos, Quaternion.identity, parent);
            }
            else
            {
                wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wall.name = "Wall";
                wall.transform.SetParent(parent);
                wall.transform.position = pos;
                wall.transform.localScale = size;
            }
            Track(wall);
        }

        private void PlaceChangingTable(Transform parent, LevelDefinition def, float halfSize, System.Random rng)
        {
            int wall = rng.Next(0, 4);
            float along = (float)(rng.NextDouble() * (halfSize * 1.6f) - halfSize * 0.8f);
            Vector3 pos; Quaternion rot = Quaternion.identity;
            switch (wall)
            {
                case 0: pos = new Vector3( halfSize - 0.5f, 0.5f, along); rot = Quaternion.Euler(0, 90, 0); break;
                case 1: pos = new Vector3(-halfSize + 0.5f, 0.5f, along); rot = Quaternion.Euler(0, 90, 0); break;
                case 2: pos = new Vector3(along, 0.5f,  halfSize - 0.5f); break;
                default:pos = new Vector3(along, 0.5f, -halfSize + 0.5f); break;
            }
            var go = Spawn(def.changingTablePrefab, PrimitiveType.Cube, pos, parent);
            go.transform.rotation = rot;
            go.name = "ChangingTable";

            // Add a slightly larger trigger volume so the father can "reach" it, plus
            // the diaper-change behaviour. Detected by component, no tag needed.
            var trigger = new GameObject("ChangingTableTrigger");
            trigger.transform.SetParent(go.transform, false);
            var box = trigger.AddComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = new Vector3(2f, 2f, 2f);
            trigger.AddComponent<ChangingTableTrigger>();
        }

        private void PlaceFreeFurniture(Transform parent, GameObject prefab, string name, float halfSize, System.Random rng)
        {
            for (int attempt = 0; attempt < 20; attempt++)
            {
                float x = (float)(rng.NextDouble() * (interior.size.x) - interior.extents.x);
                float z = (float)(rng.NextDouble() * (interior.size.z) - interior.extents.z);
                Vector3 pos = new Vector3(x, 0.5f, z);
                // Check above the floor so the floor plane itself doesn't count as blocking.
                if (!Physics.CheckSphere(pos + Vector3.up * 0.7f, FurnitureClearance))
                {
                    var go = Spawn(prefab, PrimitiveType.Cube, pos, parent);
                    go.name = name;
                    return;
                }
            }
        }

        private GameObject Spawn(GameObject prefab, PrimitiveType fallback, Vector3 pos, Transform parent)
        {
            GameObject go = prefab != null
                ? Object.Instantiate(prefab, pos, Quaternion.identity, parent)
                : GameObject.CreatePrimitive(fallback);
            if (prefab == null)
            {
                go.transform.SetParent(parent);
                go.transform.position = pos;
            }
            Track(go);
            return go;
        }

        public void Clear()
        {
            foreach (var go in spawned) if (go != null) Object.Destroy(go);
            spawned.Clear();
        }

        private void Track(GameObject go) => spawned.Add(go);

        private static void Shuffle<T>(IList<T> list, System.Random rng)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rng.Next(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
