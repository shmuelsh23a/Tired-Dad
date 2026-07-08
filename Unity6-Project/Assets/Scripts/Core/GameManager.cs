using UnityEngine;
using UnityEngine.SceneManagement;

namespace TiredDad
{
    /// <summary>
    /// Orchestrates a single gameplay level: builds the level via LevelGenerator,
    /// ticks the StatSystem, drives spawning and needs, checks win/lose, and hands
    /// off to the score/level-end flow. Replaces the old 1,360-line Game.cs.
    ///
    /// Scene wiring (Game scene): put this on a "GameManager" object and assign the
    /// player, generator, spawn/needs systems, HUD and the LevelDefinition in the Inspector.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("References")]
        public PlayerMover player;
        public LevelGenerator levelGenerator;
        public SpawnSystem spawnSystem;
        public NeedsSystem needsSystem;
        public GameHud hud;

        [Header("Config")]
        public LevelDefinition levelDefinition;
        [Tooltip("0 = use a fresh random seed each level.")]
        public int fixedSeed = 0;
        public string levelEndSceneName = "LevelEnd";

        [Header("Runtime (read-only)")]
        public StatSystem stats = new StatSystem();
        public LevelOutcome outcome = LevelOutcome.None;
        public bool running;

        private System.Random _rng;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            // Ensure a ScoreManager exists even if we boot straight into the Game scene.
            if (ScoreManager.Instance == null)
            {
                var go = new GameObject("ScoreManager");
                go.AddComponent<ScoreManager>();
            }
        }

        void Start()
        {
            BeginLevel();
        }

        public void BeginLevel()
        {
            int seed = fixedSeed != 0 ? fixedSeed : System.Environment.TickCount;
            _rng = new System.Random(seed);

            ScoreManager.Instance.LevelIndex++;
            int level = ScoreManager.Instance.LevelIndex;

            // Build the room, furniture, spawn table, starting stats and goal.
            var result = levelGenerator.Generate(level, levelDefinition, _rng, ScoreManager.Instance);

            stats = result.stats;
            spawnSystem.Begin(result.spawnTable, result.difficulty, _rng);
            needsSystem.Begin(stats, _rng);

            hud.SetLevel(level);
            hud.SetScore(ScoreManager.Instance.TotalScore);

            outcome = LevelOutcome.None;
            running = true;
            if (player != null) player.enabled = true;
        }

        void Update()
        {
            if (!running) return;

            float dt = Time.deltaTime;
            bool walking = player != null && player.IsWalking;
            float tempo01 = player != null ? player.Tempo01 : 0f;

            needsSystem.Tick(dt);
            stats.Tick(dt, walking, tempo01, needsSystem.HasUnmetNeed);
            spawnSystem.Tick(dt);

            hud.UpdateStats(stats, needsSystem, Time.timeSinceLevelLoad);

            outcome = stats.CheckOutcome();
            if (outcome != LevelOutcome.None)
                EndLevel();
        }

        private void EndLevel()
        {
            running = false;
            if (player != null) player.enabled = false;

            hud.ShowEnding(outcome);
            ScoreManager.Instance.ScoreLevel(stats);

            // The HUD/ending prompt calls Continue() on tap (see HUDController).
        }

        /// <summary>Advance to the next generated level (same scene, regenerated).</summary>
        public void ContinueToNextLevel()
        {
            BeginLevel();
        }

        /// <summary>Go to the dedicated level-end scene (optional flow).</summary>
        public void GoToLevelEndScene()
        {
            SceneManager.LoadScene(levelEndSceneName);
        }
    }
}
