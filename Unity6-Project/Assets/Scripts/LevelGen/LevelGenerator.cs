using UnityEngine;

namespace TiredDad
{
    /// <summary>
    /// Procedurally builds a level: resolves difficulty from the level index, builds
    /// the room + furniture (RoomLayout), assembles a weighted SpawnTable, and rolls
    /// randomized starting stats + goal (all seeded for reproducibility).
    ///
    /// This is the NEW system requested for the modernized game. Put it on the same
    /// object as (or reference it from) the GameManager, and assign a parent transform
    /// under which generated geometry lives so it can be cleared between levels.
    /// </summary>
    public class LevelGenerator : MonoBehaviour
    {
        [Tooltip("Empty transform that generated room/furniture is parented to (cleared each level).")]
        public Transform levelRoot;

        [Tooltip("Where the player is repositioned at the start of each level.")]
        public Transform playerStart;
        public PlayerMover player;

        private readonly RoomLayout _room = new RoomLayout();

        public struct Result
        {
            public StatSystem stats;
            public SpawnTable spawnTable;
            public DifficultySettings difficulty;
        }

        public Result Generate(int level, LevelDefinition def, System.Random rng, ScoreManager score)
        {
            if (levelRoot == null)
            {
                var root = new GameObject("LevelRoot");
                levelRoot = root.transform;
            }

            var diff = ResolveDifficulty(level, def, rng);

            // 1. Room + furniture.
            _room.Build(levelRoot, def, diff.roomSize, rng);

            // 2. Reposition player to the room center / designated start.
            if (player != null)
            {
                Vector3 start = playerStart != null ? playerStart.position : new Vector3(0, 1f, 0);
                var cc = player.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;
                player.transform.position = start;
                player.transform.rotation = Quaternion.identity;
                if (cc != null) cc.enabled = true;
            }

            // 3. Weighted spawn table over the item pools, biased by difficulty.
            var table = BuildSpawnTable(def, diff);

            // 4. Randomized starting stats + goal (with carry-over clamping).
            var stats = BuildStats(def, diff, score, rng);

            return new Result { stats = stats, spawnTable = table, difficulty = diff };
        }

        private DifficultySettings ResolveDifficulty(int level, LevelDefinition def, System.Random rng)
        {
            float t = def.maxLevelForCurve > 1
                ? Mathf.Clamp01((level - 1f) / (def.maxLevelForCurve - 1f))
                : 1f;

            // Optional per-level jitter so equal indices still feel varied.
            float j = def.perLevelJitter;
            float tj = Mathf.Clamp01(t + (float)(rng.NextDouble() * 2 - 1) * j);

            float L(float easy, float hard) => Mathf.Lerp(easy, hard, tj);

            return new DifficultySettings
            {
                moraleRisePerSecond      = L(def.easyMoraleRise, def.hardMoraleRise),
                fatherTirednessPerSecond = L(def.easyFatherTired, def.hardFatherTired),
                babySleepWalkMultiplier  = L(def.easyBabyWalkMul, def.hardBabyWalkMul),
                needsActivePerSecond     = L(def.easyNeedsActive, def.hardNeedsActive),
                spawnIntervalMin         = L(def.easySpawnMin, def.hardSpawnMin),
                spawnIntervalMax         = L(def.easySpawnMax, def.hardSpawnMax),
                maxConcurrent            = Mathf.RoundToInt(L(def.easyMaxConcurrent, def.hardMaxConcurrent)),
                positiveWeight           = L(def.easyPositiveWeight, def.hardPositiveWeight),
                negativeWeight           = L(def.easyNegativeWeight, def.hardNegativeWeight),
                halfAndHalfWeight        = L(def.easyHalfWeight, def.hardHalfWeight),
                roomSize                 = L(def.easyRoomSize, def.hardRoomSize),
            };
        }

        private SpawnTable BuildSpawnTable(LevelDefinition def, DifficultySettings diff)
        {
            var table = new SpawnTable { area = _room.interior };

            AddPool(table, def.positives,   diff.positiveWeight);
            AddPool(table, def.negatives,   diff.negativeWeight);
            AddPool(table, def.halfAndHalf, diff.halfAndHalfWeight);

            // Caps grow modestly with concurrency budget.
            table.capPositive = table.capNegative = table.capHalfAndHalf =
                Mathf.Clamp(diff.maxConcurrent / 2, 3, 8);
            return table;
        }

        private static void AddPool(SpawnTable table, ItemDefinition[] pool, float categoryWeight)
        {
            if (pool == null || pool.Length == 0) return;
            float per = categoryWeight / pool.Length;
            foreach (var item in pool)
                table.Add(item, per);
        }

        private StatSystem BuildStats(LevelDefinition def, DifficultySettings diff, ScoreManager score, System.Random rng)
        {
            var s = new StatSystem
            {
                moraleRisePerSecond      = diff.moraleRisePerSecond,
                fatherTirednessPerSecond = diff.fatherTirednessPerSecond,
                babySleepWalkMultiplier  = diff.babySleepWalkMultiplier,
                needsActivePerSecond     = diff.needsActivePerSecond,
            };

            float baby   = RandRange(def.startBabySleepRange, rng);
            float needs  = RandRange(def.startNeedsRange, rng);
            float father = RandRange(def.startFatherTiredRange, rng);
            float morale = RandRange(def.startMoraleRange, rng);
            float goal   = RandRange(def.sleepGoalRange, rng);

            // Carry-over from a previous level (clamped into this level's ranges).
            if (score != null && score.LevelIndex > 1)
            {
                father = Mathf.Clamp(score.PrevFatherTiredness * 0.5f, def.startFatherTiredRange.x, 60f);
                morale = Mathf.Clamp(score.PrevMorale * 0.5f, def.startMoraleRange.x, 60f);
            }

            s.Init(father, baby, morale, needs, 0f, goal);
            return s;
        }

        private float RandRange(Vector2 range, System.Random rng) =>
            Mathf.Lerp(range.x, range.y, (float)rng.NextDouble());
    }
}
