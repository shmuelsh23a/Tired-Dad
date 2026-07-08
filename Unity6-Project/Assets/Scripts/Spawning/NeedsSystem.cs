using UnityEngine;

namespace TiredDad
{
    /// <summary>
    /// Manages the baby's discrete needs (Food / Diaper / Attention). Over time,
    /// and more often as Needs rises, a new need may appear. Unmet needs make the
    /// Needs stat climb faster (handled in StatSystem). Items/furniture clear needs
    /// via ClearNeed(). Replaces the Needs* methods scattered through Game.cs, with
    /// the original's missing/overlapping probability bands fixed.
    /// </summary>
    public class NeedsSystem : MonoBehaviour
    {
        public bool NeedFood, NeedDiaper, NeedAttention;
        public bool HasUnmetNeed => NeedFood || NeedDiaper || NeedAttention;

        [Header("Optional prompt spawns")]
        [Tooltip("Spawned when Attention need appears (e.g. a pacifier). Optional.")]
        public ItemDefinition pacifierItem;
        [Tooltip("Spawned when Food need appears (e.g. a bottle). Optional.")]
        public ItemDefinition bottleItem;
        public SpawnSystem spawnSystem;   // used only if the above are assigned

        private StatSystem _stats;
        private System.Random _rng;
        private float _rollTimer;
        private const float RollEvery = 1.0f;   // consider a new need at most once/sec

        public void Begin(StatSystem stats, System.Random rng)
        {
            _stats = stats;
            _rng = rng;
            NeedFood = NeedDiaper = NeedAttention = false;
            _rollTimer = 0f;
        }

        public void Tick(float dt)
        {
            if (_stats == null) return;
            _rollTimer += dt;
            if (_rollTimer < RollEvery) return;
            _rollTimer = 0f;

            // Contiguous probability bands over Needs (percent chance per roll).
            float needs = _stats.Needs;
            int chance;
            if (needs < 20f)       chance = 0;    // calm baby, no new needs yet
            else if (needs < 60f)  chance = 5;
            else if (needs < 80f)  chance = 20;
            else                   chance = 50;

            if (chance == 0) return;
            if (_rng.Next(0, 100) < chance)
                RaiseRandomNeed();
        }

        private void RaiseRandomNeed()
        {
            // Pick among needs not already active.
            int which = _rng.Next(0, 3);
            for (int i = 0; i < 3; i++)
            {
                int idx = (which + i) % 3;
                if (idx == 0 && !NeedFood)      { NeedFood = true;      TrySpawn(bottleItem);   return; }
                if (idx == 1 && !NeedDiaper)    { NeedDiaper = true;    /* changing table clears */ return; }
                if (idx == 2 && !NeedAttention) { NeedAttention = true; TrySpawn(pacifierItem); return; }
            }
        }

        private void TrySpawn(ItemDefinition item)
        {
            // Prompt items are optional; the SpawnSystem places them if provided.
            // (Kept lightweight here; wire in if you want guaranteed need-prompts.)
        }

        /// <summary>Raise a need — called by items (Poop/Food/Attention).</summary>
        public void RaiseNeed(BabyNeed need)
        {
            switch (need)
            {
                case BabyNeed.Food:      NeedFood = true; break;
                case BabyNeed.Diaper:    NeedDiaper = true; break;
                case BabyNeed.Attention: NeedAttention = true; break;
            }
        }

        /// <summary>Clear a need — called by items (Bottle/Pacifier) or the changing table.</summary>
        public void ClearNeed(BabyNeed need)
        {
            switch (need)
            {
                case BabyNeed.Food:      NeedFood = false; break;
                case BabyNeed.Diaper:    NeedDiaper = false; break;
                case BabyNeed.Attention: NeedAttention = false; break;
            }
        }

        /// <summary>Human-readable list of active needs for the HUD.</summary>
        public string NeedsLabel()
        {
            string s = "";
            if (NeedFood) s += " Food";
            if (NeedDiaper) s += " Diaper";
            if (NeedAttention) s += " Attention";
            return s;
        }
    }
}
