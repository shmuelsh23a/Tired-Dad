using UnityEngine;

namespace TiredDad
{
    /// <summary>
    /// Spawns collectibles over time from a weighted SpawnTable, respecting per-category
    /// caps, a max-concurrent limit, and a collision check so items don't overlap
    /// furniture/walls. Replaces the giant SpawnObjects() coroutine + switch ladders
    /// in the old Game.cs. Items are built from ItemDefinition.modelPrefab, falling
    /// back to a coloured primitive when no model is assigned (parity build).
    /// </summary>
    public class SpawnSystem : MonoBehaviour
    {
        [Header("Placement")]
        public float spawnY = 0.7f;             // kept above the floor so clearance check is clean
        public float clearanceRadius = 0.5f;    // no other collider within this radius
        public LayerMask blockingLayers = ~0;   // walls/furniture/other items
        public int placementAttempts = 12;

        private SpawnTable _table;
        private DifficultySettings _difficulty;
        private System.Random _rng;

        private int _livePositive, _liveNegative, _liveHalf;
        private float _timer;
        private float _nextInterval;
        private bool _active;

        public void Begin(SpawnTable table, DifficultySettings difficulty, System.Random rng)
        {
            _table = table;
            _difficulty = difficulty;
            _rng = rng;
            _livePositive = _liveNegative = _liveHalf = 0;
            _timer = 0f;
            _nextInterval = RollInterval();
            _active = true;
        }

        public void Tick(float dt)
        {
            if (!_active || _table == null) return;

            _timer += dt;
            if (_timer < _nextInterval) return;
            _timer = 0f;
            _nextInterval = RollInterval();

            if (LiveTotal() >= _difficulty.maxConcurrent) return;

            var item = _table.Pick(_rng);
            if (item == null) return;
            if (LiveFor(item.category) >= _table.CapFor(item.category)) return;

            if (TryFindPosition(out Vector3 pos))
                SpawnItem(item, pos);
        }

        private float RollInterval()
        {
            float t = (float)_rng.NextDouble();
            return Mathf.Lerp(_difficulty.spawnIntervalMin, _difficulty.spawnIntervalMax, t);
        }

        private bool TryFindPosition(out Vector3 pos)
        {
            for (int i = 0; i < placementAttempts; i++)
            {
                float x = Mathf.Lerp(_table.area.min.x, _table.area.max.x, (float)_rng.NextDouble());
                float z = Mathf.Lerp(_table.area.min.z, _table.area.max.z, (float)_rng.NextDouble());
                Vector3 candidate = new Vector3(x, spawnY, z);
                if (!Physics.CheckSphere(candidate, clearanceRadius, blockingLayers))
                {
                    pos = candidate;
                    return true;
                }
            }
            pos = Vector3.zero;
            return false;
        }

        private void SpawnItem(ItemDefinition def, Vector3 pos)
        {
            GameObject go;
            if (def.modelPrefab != null)
            {
                go = Instantiate(def.modelPrefab, pos, Quaternion.identity);
            }
            else
            {
                // Parity fallback: primitive shaped by category (cyl/capsule/sphere).
                var shape = def.category switch
                {
                    ItemCategory.Positive    => PrimitiveType.Cylinder,
                    ItemCategory.Negative    => PrimitiveType.Capsule,
                    _                        => PrimitiveType.Sphere
                };
                go = GameObject.CreatePrimitive(shape);
                go.transform.position = pos;
                go.transform.localScale = Vector3.one * 0.6f;
                var rend = go.GetComponent<Renderer>();
                if (rend != null) rend.material.color = def.fallbackColor;
            }

            // Ensure trigger collider + kinematic rigidbody so trigger events fire
            // against the player's CharacterController.
            var col = go.GetComponent<Collider>();
            if (col == null) col = go.AddComponent<SphereCollider>();
            col.isTrigger = true;

            var rb = go.GetComponent<Rigidbody>();
            if (rb == null) rb = go.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

            var effect = go.GetComponent<ItemEffect>();
            if (effect == null) effect = go.AddComponent<ItemEffect>();
            effect.definition = def;
            effect.owner = this;

            RegisterLive(def.category, +1);
        }

        /// <summary>Called by ItemEffect when an item is picked up.</summary>
        public void NotifyConsumed(ItemCategory category) => RegisterLive(category, -1);

        private void RegisterLive(ItemCategory c, int delta)
        {
            switch (c)
            {
                case ItemCategory.Positive:    _livePositive = Mathf.Max(0, _livePositive + delta); break;
                case ItemCategory.Negative:    _liveNegative = Mathf.Max(0, _liveNegative + delta); break;
                case ItemCategory.HalfAndHalf: _liveHalf     = Mathf.Max(0, _liveHalf + delta);     break;
            }
        }

        private int LiveFor(ItemCategory c) => c switch
        {
            ItemCategory.Positive    => _livePositive,
            ItemCategory.Negative    => _liveNegative,
            ItemCategory.HalfAndHalf => _liveHalf,
            _ => 0
        };

        private int LiveTotal() => _livePositive + _liveNegative + _liveHalf;
    }
}
