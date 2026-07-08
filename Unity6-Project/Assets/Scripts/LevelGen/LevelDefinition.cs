using UnityEngine;

namespace TiredDad
{
    /// <summary>
    /// The tuning + content pool for procedural levels. Create one via
    /// Assets ▸ Create ▸ TiredDad ▸ Level Definition. Endpoints describe an "easy"
    /// level 1 and a "hard" level at <see cref="maxLevelForCurve"/>; LevelGenerator
    /// interpolates between them (with optional per-level jitter) for any level index.
    /// </summary>
    [CreateAssetMenu(menuName = "TiredDad/Level Definition", fileName = "LevelDefinition")]
    public class LevelDefinition : ScriptableObject
    {
        [Header("Curve")]
        [Tooltip("Level index at which the 'hard' endpoints are reached.")]
        public int maxLevelForCurve = 10;
        [Range(0f, 0.5f)] public float perLevelJitter = 0.1f;

        [Header("Item pools")]
        public ItemDefinition[] positives;
        public ItemDefinition[] negatives;
        public ItemDefinition[] halfAndHalf;

        [Header("Room prefabs (parity-safe: leave null to auto-build primitives)")]
        public GameObject floorPrefab;
        public GameObject wallPrefab;
        public GameObject bedPrefab;
        public GameObject chairPrefab;
        public GameObject sofaPrefab;
        public GameObject changingTablePrefab;

        // ---- Easy endpoints (level 1) ----
        [Header("Easy (level 1)")]
        public float easyMoraleRise = 0.6f;
        public float easyFatherTired = 0.6f;
        public float easyBabyWalkMul = 1.2f;
        public float easyNeedsActive = 0.8f;
        public float easySpawnMin = 2.0f;
        public float easySpawnMax = 4.0f;
        public int   easyMaxConcurrent = 6;
        public float easyPositiveWeight = 3f;
        public float easyNegativeWeight = 1f;
        public float easyHalfWeight = 1f;
        public float easyRoomSize = 5.0f;

        // ---- Hard endpoints (maxLevelForCurve) ----
        [Header("Hard (max level)")]
        public float hardMoraleRise = 1.6f;
        public float hardFatherTired = 1.6f;
        public float hardBabyWalkMul = 0.8f;
        public float hardNeedsActive = 1.8f;
        public float hardSpawnMin = 0.8f;
        public float hardSpawnMax = 2.0f;
        public int   hardMaxConcurrent = 12;
        public float hardPositiveWeight = 1f;
        public float hardNegativeWeight = 3f;
        public float hardHalfWeight = 2f;
        public float hardRoomSize = 7.0f;

        // ---- Starting stats & goal ranges ----
        [Header("Starting stats & goal (randomized per level)")]
        public Vector2 startBabySleepRange = new Vector2(0f, 20f);
        public Vector2 startNeedsRange = new Vector2(0f, 40f);
        public Vector2 startFatherTiredRange = new Vector2(0f, 15f);
        public Vector2 startMoraleRange = new Vector2(0f, 10f);
        public Vector2 sleepGoalRange = new Vector2(90f, 100f);
    }
}
