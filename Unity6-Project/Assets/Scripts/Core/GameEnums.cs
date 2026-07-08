namespace TiredDad
{
    /// <summary>Which bucket a collectible item belongs to.</summary>
    public enum ItemCategory
    {
        Positive,    // good (was: cylinders)
        Negative,    // bad  (was: capsules)
        HalfAndHalf  // mixed (was: spheres)
    }

    /// <summary>Reason a level ended, used to pick the ending text/flow.</summary>
    public enum LevelOutcome
    {
        None,
        BabyAsleep,      // WIN  — baby sleepiness reached goal
        FatherAsleep,    // LOSE — father tiredness maxed
        Depressed        // LOSE — morale maxed
    }

    /// <summary>The baby's active needs (can be combined).</summary>
    public enum BabyNeed
    {
        Food,
        Diaper,
        Attention
    }
}
