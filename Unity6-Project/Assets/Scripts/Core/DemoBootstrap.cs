using UnityEngine;

namespace TiredDad
{
    /// <summary>
    /// ONE-CLICK DEMO. Put this single component on an empty GameObject in an empty
    /// scene and press Play. It builds everything at runtime — camera, lighting, the
    /// father, the HUD, all game systems, the 30 items, and a LevelDefinition — so no
    /// manual scene wiring or asset authoring is needed. Uses coloured primitives
    /// (cylinders = good, capsules = bad, spheres = mixed), matching the original.
    ///
    /// Controls: TAP in rhythm (faster than once/2s, slower than once/0.5s) to walk;
    /// SWIPE left/right to turn 90°. In the Editor, mouse-click acts as a tap and a
    /// click-drag acts as a swipe. Reach the changing table when the baby needs a diaper.
    /// </summary>
    public class DemoBootstrap : MonoBehaviour
    {
        [Header("Optional overrides")]
        [Tooltip("0 = fresh random level each time; non-zero = reproducible seed.")]
        public int fixedSeed = 0;

        private GameManager _gm;

        void Awake()
        {
            BuildItems();
            var levelDef = BuildLevelDefinition();

            var player   = BuildPlayer();
            BuildCamera(player.transform);
            BuildLight();

            var hud = BuildHud();
            BuildSystems(player, hud, levelDef);
        }

        // ---------------------------------------------------------------- items
        private void BuildItems()
        {
            ItemDatabase.Build();
        }

        private LevelDefinition BuildLevelDefinition()
        {
            var def = ScriptableObject.CreateInstance<LevelDefinition>();
            def.name = "GeneratedLevelDefinition";
            def.positives   = ItemDatabase.Positives.ToArray();
            def.negatives   = ItemDatabase.Negatives.ToArray();
            def.halfAndHalf = ItemDatabase.HalfAndHalf.ToArray();
            // Auto-use furniture models if placed under Resources/Models; else primitives.
            def.bedPrefab            = Resources.Load<GameObject>("Models/Bed");
            def.chairPrefab          = Resources.Load<GameObject>("Models/Chair");
            def.sofaPrefab           = Resources.Load<GameObject>("Models/Sofa");
            def.changingTablePrefab  = Resources.Load<GameObject>("Models/ChangingTable");
            return def;
        }

        // --------------------------------------------------------------- player
        private PlayerMover BuildPlayer()
        {
            var go = new GameObject("Father");
            go.transform.position = new Vector3(0f, 1f, 0f);

            var cc = go.AddComponent<CharacterController>();
            cc.radius = 0.35f;
            cc.height = 1.6f;
            cc.center = new Vector3(0f, 0.8f, 0f);

            var input = go.AddComponent<TouchInputController>();
            var mover = go.AddComponent<PlayerMover>();
            mover.input = input;

            // Visual body: use the Father .obj model if it's under Resources/Models,
            // otherwise a coloured capsule + facing indicator.
            var fatherModel = Resources.Load<GameObject>("Models/Father");
            if (fatherModel != null)
            {
                var vis = Instantiate(fatherModel, go.transform, false);
                vis.name = "FatherModel";
                vis.transform.localPosition = Vector3.zero;   // model base at feet
                StripColliders(vis);
            }
            else
            {
                var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                body.name = "Body";
                body.transform.SetParent(go.transform, false);
                body.transform.localPosition = new Vector3(0f, 0.8f, 0f);
                Destroy(body.GetComponent<Collider>());
                Colorize(body, new Color(0.25f, 0.45f, 0.9f));

                var nose = GameObject.CreatePrimitive(PrimitiveType.Cube);
                nose.name = "Facing";
                nose.transform.SetParent(go.transform, false);
                nose.transform.localScale = new Vector3(0.25f, 0.25f, 0.4f);
                nose.transform.localPosition = new Vector3(0f, 0.8f, 0.45f);
                Destroy(nose.GetComponent<Collider>());
                Colorize(nose, Color.white);
            }

            return mover;
        }

        // --------------------------------------------------------------- camera
        private void BuildCamera(Transform target)
        {
            var go = new GameObject("Main Camera");
            go.tag = "MainCamera";
            var cam = go.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.09f, 0.10f, 0.15f);
            go.AddComponent<AudioListener>();
            var follow = go.AddComponent<CameraFollow>();
            follow.target = target;
            go.transform.position = target.position + follow.offset;
        }

        private void BuildLight()
        {
            var go = new GameObject("Sun");
            var light = go.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.1f;
            light.color = new Color(1f, 0.96f, 0.9f);
            go.transform.rotation = Quaternion.Euler(55f, -30f, 0f);
        }

        // ------------------------------------------------------------------ hud
        private SimpleHUD BuildHud()
        {
            var go = new GameObject("HUD");
            var hud = go.AddComponent<SimpleHUD>();
            hud.Build();
            return hud;
        }

        // -------------------------------------------------------------- systems
        private void BuildSystems(PlayerMover player, GameHud hud, LevelDefinition def)
        {
            var go = new GameObject("GameSystems");

            var spawn  = go.AddComponent<SpawnSystem>();
            var needs  = go.AddComponent<NeedsSystem>();
            var levelGen = go.AddComponent<LevelGenerator>();

            // Level root + player start.
            var root = new GameObject("LevelRoot");
            levelGen.levelRoot = root.transform;
            var start = new GameObject("PlayerStart");
            start.transform.position = new Vector3(0f, 1f, 0f);
            levelGen.playerStart = start.transform;
            levelGen.player = player;

            // GameManager last: its Awake sets up the singleton + ScoreManager.
            _gm = go.AddComponent<GameManager>();
            _gm.player = player;
            _gm.levelGenerator = levelGen;
            _gm.spawnSystem = spawn;
            _gm.needsSystem = needs;
            _gm.hud = hud;
            _gm.levelDefinition = def;
            _gm.fixedSeed = fixedSeed;

            // Fresh run (LevelIndex 0 → first BeginLevel makes it Level 1).
            if (ScoreManager.Instance != null) ScoreManager.Instance.NewRun();

            // End-of-level tap handling.
            var flow = go.AddComponent<ScreenFlow>();
            flow.gameManager = _gm;
            flow.requireLevelEnded = true;
        }

        // ------------------------------------------------------------- helpers
        private static void Colorize(GameObject go, Color c)
        {
            var r = go.GetComponent<Renderer>();
            if (r != null) r.material.color = c;
        }

        private static void StripColliders(GameObject root)
        {
            foreach (var col in root.GetComponentsInChildren<Collider>())
                Destroy(col);
        }
    }
}
