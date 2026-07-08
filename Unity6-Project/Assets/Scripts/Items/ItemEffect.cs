using UnityEngine;

namespace TiredDad
{
    /// <summary>
    /// Sits on a spawned collectible. On contact with the player it applies its
    /// ItemDefinition, notifies the SpawnSystem (to free a slot), and self-destructs.
    /// Replaces the ~30 near-identical per-item scripts (Coffee.cs, Cry.cs, ...).
    ///
    /// The collider should be a trigger; the player must be tagged "Player".
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ItemEffect : MonoBehaviour
    {
        public ItemDefinition definition;

        [HideInInspector] public SpawnSystem owner;   // set by the spawner

        private bool _consumed;

        void Reset()
        {
            var col = GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
        }

        void OnTriggerEnter(Collider other)
        {
            if (_consumed) return;
            // Detect the player by component (no tag setup required).
            if (other.GetComponentInParent<PlayerMover>() == null) return;
            if (definition == null || GameManager.Instance == null) return;

            _consumed = true;
            definition.Apply(GameManager.Instance);
            if (owner != null) owner.NotifyConsumed(definition.category);
            Destroy(gameObject);
        }
    }
}
