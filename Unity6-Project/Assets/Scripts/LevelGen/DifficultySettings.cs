namespace TiredDad
{
    /// <summary>
    /// Resolved, per-level tuning values produced by LevelGenerator by interpolating
    /// a LevelDefinition's easy→hard endpoints against the current level index.
    /// </summary>
    public struct DifficultySettings
    {
        // Stat rates (fed into StatSystem).
        public float moraleRisePerSecond;
        public float fatherTirednessPerSecond;
        public float babySleepWalkMultiplier;
        public float needsActivePerSecond;

        // Spawning.
        public float spawnIntervalMin;
        public float spawnIntervalMax;
        public int   maxConcurrent;

        // Category weighting bias (higher = more of that category appears).
        public float positiveWeight;
        public float negativeWeight;
        public float halfAndHalfWeight;

        // Room.
        public float roomSize;   // half-extent used for the room footprint
    }
}
