using UnityEngine;

namespace TiredDad
{
    /// <summary>
    /// Base class the GameManager talks to, so either the polished TMP-based
    /// HUDController or the zero-setup SimpleHUD can be plugged in.
    /// </summary>
    public abstract class GameHud : MonoBehaviour
    {
        public abstract void SetLevel(int level);
        public abstract void SetScore(int score);
        public abstract void UpdateStats(StatSystem stats, NeedsSystem needs, float time);
        public abstract void ShowEnding(LevelOutcome outcome);
    }
}
