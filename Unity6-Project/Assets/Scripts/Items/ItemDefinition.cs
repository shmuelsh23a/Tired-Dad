using UnityEngine;

namespace TiredDad
{
    /// <summary>
    /// Data for one collectible. Create assets via
    /// Assets ▸ Create ▸ TiredDad ▸ Item. One asset per object (~30 total) replaces
    /// the ~50 hardcoded "xxxOffset" fields in the old Game.cs, so BALANCING becomes
    /// data editing rather than code changes.
    ///
    /// Convention: a positive number RAISES that stat, negative LOWERS it.
    /// (e.g. Coffee sets fatherTiredness to a negative value; Cry sets morale positive.)
    /// </summary>
    [CreateAssetMenu(menuName = "TiredDad/Item", fileName = "Item_")]
    public class ItemDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string displayName;      // "Coffee"
        public string displayNameHe;    // "קפה"
        public ItemCategory category;

        [Header("Visual (fallback primitive used if null)")]
        public GameObject modelPrefab;
        public Color fallbackColor = Color.white;

        [Header("Stat deltas applied on pickup (+raises / -lowers)")]
        public float fatherTiredness;
        public float babySleepiness;
        public float morale;
        public float needs;
        public float guilt;

        [Header("Special needs handling")]
        [Tooltip("If set, picking this up clears the given baby need (e.g. Bottle→Food, Pacifier→Attention).")]
        public bool clearsNeed;
        public BabyNeed needCleared;

        [Tooltip("If set, picking this up raises the given baby need (e.g. Poop→Diaper, Food→Food).")]
        public bool raisesNeed;
        public BabyNeed needRaised;

        [Header("Audio")]
        public AudioClip pickupSound;

        /// <summary>Apply this item's effect to the running game.</summary>
        public void Apply(GameManager gm)
        {
            gm.stats.Apply(fatherTiredness, babySleepiness, morale, needs, guilt);
            if (clearsNeed) gm.needsSystem.ClearNeed(needCleared);
            if (raisesNeed) gm.needsSystem.RaiseNeed(needRaised);
            if (pickupSound != null && Camera.main != null)
                AudioSource.PlayClipAtPoint(pickupSound, Camera.main.transform.position);
        }
    }
}
