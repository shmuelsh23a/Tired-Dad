using UnityEngine;

namespace TiredDad
{
    /// <summary>
    /// Owns the five gameplay stats and their update rules. Replaces the tangle of
    /// stat logic that lived inside the old Game.cs. All values are clamped 0..100.
    ///
    /// Bugs fixed vs. the original (see FRAMEWORK.md §4.3):
    ///  - Guilt boundary gaps: inclusive, contiguous bands (no value falls through).
    ///  - Needs start band 76..100 now covered (see LevelGenerator).
    /// </summary>
    [System.Serializable]
    public class StatSystem
    {
        public const float Min = 0f;
        public const float Max = 100f;

        // The five stats.
        public float FatherTiredness;
        public float BabySleepiness;
        public float Morale;      // really "depression" — rising is bad
        public float Needs;
        public float Guilt;

        // Per-level tuning, supplied by the LevelDefinition / difficulty curve.
        public float moraleRisePerSecond      = 1.0f;
        public float fatherTirednessPerSecond = 1.0f;   // when standing still
        public float fatherWalkRecoverPerSec  = 0.5f;   // walking reduces tiredness
        public float babySleepBasePerSecond   = 0.01f;  // baseline drowsiness
        public float babySleepWalkMultiplier  = 1.0f;   // extra while father walks
        public float needsIdlePerSecond       = 0.1f;
        public float needsActivePerSecond     = 1.0f;   // when an unmet need exists

        // Win threshold for baby sleepiness (usually 100, can vary per level).
        public float sleepGoal = 100f;

        /// <summary>Reset to a fresh level's starting values.</summary>
        public void Init(float father, float baby, float morale, float needs, float guilt, float goal)
        {
            FatherTiredness = Clamp(father);
            BabySleepiness  = Clamp(baby);
            Morale          = Clamp(morale);
            Needs           = Clamp(needs);
            Guilt           = Clamp(guilt);
            sleepGoal       = Mathf.Clamp(goal, 1f, Max);
        }

        /// <summary>Advance all continuous stats by dt. Discrete item pickups go through Apply().</summary>
        public void Tick(float dt, bool isWalking, float walkTempo01, bool hasUnmetNeed)
        {
            // Father tiredness: walking is (oddly) restful in this game's fiction.
            if (isWalking)
                FatherTiredness -= fatherWalkRecoverPerSec * dt;
            else
                FatherTiredness += fatherTirednessPerSecond * dt;

            // Baby sleepiness: baseline plus a bonus that scales with walk tempo.
            BabySleepiness += babySleepBasePerSecond * dt;
            if (isWalking)
                BabySleepiness += babySleepWalkMultiplier * walkTempo01 * dt;

            // Morale creeps upward over time.
            Morale += moraleRisePerSecond * dt;

            // Needs drift up; faster when a need is currently unmet.
            Needs += (hasUnmetNeed ? needsActivePerSecond : needsIdlePerSecond) * dt;

            // Guilt scales off Needs in contiguous, inclusive bands (bug fixed).
            Guilt += GuiltRate(Needs) * dt;

            ClampAll();
        }

        /// <summary>Guilt growth rate as a function of Needs. Contiguous bands, no gaps.</summary>
        private static float GuiltRate(float needs)
        {
            if (needs < 10f)  return 0.0f;
            if (needs < 20f)  return 0.2f;
            if (needs < 40f)  return 1.5f;
            if (needs < 60f)  return 2.0f;
            if (needs < 80f)  return 2.5f;
            if (needs < 90f)  return 3.0f;
            return 5.0f;
        }

        /// <summary>Apply a discrete delta set from an item pickup.</summary>
        public void Apply(float father, float baby, float morale, float needs, float guilt)
        {
            FatherTiredness += father;
            BabySleepiness  += baby;
            Morale          += morale;
            Needs           += needs;
            Guilt           += guilt;
            ClampAll();
        }

        /// <summary>Check win/lose. Returns None while the level is still in play.</summary>
        public LevelOutcome CheckOutcome()
        {
            if (BabySleepiness >= sleepGoal)   return LevelOutcome.BabyAsleep;
            if (FatherTiredness >= Max)        return LevelOutcome.FatherAsleep;
            if (Morale >= Max)                 return LevelOutcome.Depressed;
            return LevelOutcome.None;
        }

        private void ClampAll()
        {
            FatherTiredness = Clamp(FatherTiredness);
            BabySleepiness  = Clamp(BabySleepiness);
            Morale          = Clamp(Morale);
            Needs           = Clamp(Needs);
            Guilt           = Clamp(Guilt);
        }

        private static float Clamp(float v) => Mathf.Clamp(v, Min, Max);
    }
}
