using UnityEngine;

namespace TiredDad
{
    /// <summary>
    /// Cumulative scoring and stat carry-over between levels. Persists across scene
    /// loads. Replaces the old ScoreManager + duplicated logic in Game.cs.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        [Header("Cumulative")]
        public int TotalScore;
        public int LevelIndex;          // 0 before the first level, 1 during level 1, ...

        [Header("End-of-level snapshot (for carry-over)")]
        public float PrevFatherTiredness;
        public float PrevBabySleepiness;
        public float PrevMorale;
        public float PrevNeeds;
        public float PrevGuilt;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>Score the finished level and snapshot its ending stats.</summary>
        public int ScoreLevel(StatSystem s)
        {
            PrevFatherTiredness = s.FatherTiredness;
            PrevBabySleepiness  = s.BabySleepiness;
            PrevMorale          = s.Morale;
            PrevNeeds           = s.Needs;
            PrevGuilt           = s.Guilt;

            // Reward: low needs, low guilt, low morale, low father-tiredness at level end.
            int gained = Mathf.RoundToInt(
                (100f - PrevNeeds - PrevGuilt) +
                (100f - PrevMorale) +
                (100f - PrevFatherTiredness));

            TotalScore += Mathf.Max(0, gained);
            return TotalScore;
        }

        /// <summary>Reset everything for a brand-new run.</summary>
        public void NewRun()
        {
            TotalScore = 0;
            LevelIndex = 0;
            PrevFatherTiredness = PrevBabySleepiness = PrevMorale = PrevNeeds = PrevGuilt = 0f;
        }
    }
}
