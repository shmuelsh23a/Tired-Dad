using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TiredDad
{
    /// <summary>
    /// Draws the in-game HUD. Supports both TextMeshPro labels (like the original's
    /// text stats) and optional Image fill bars (v0.5 goal). Assign whichever you use;
    /// unassigned fields are simply skipped. Also shows the end-of-level prompt and
    /// forwards the continue/quit taps back to the GameManager.
    /// </summary>
    public class HUDController : GameHud
    {
        [Header("Text labels (optional)")]
        public TMP_Text fatherTirednessText;
        public TMP_Text babySleepinessText;
        public TMP_Text moraleText;
        public TMP_Text guiltText;
        public TMP_Text needsText;
        public TMP_Text timeText;
        public TMP_Text scoreText;
        public TMP_Text levelText;
        public TMP_Text endingText;

        [Header("Fill bars (optional, 0..1)")]
        public Image fatherTirednessBar;
        public Image babySleepinessBar;
        public Image moraleBar;
        public Image guiltBar;
        public Image needsBar;

        public override void SetLevel(int level)
        {
            if (levelText != null) levelText.text = $"Level {level}";
        }

        public override void SetScore(int score)
        {
            if (scoreText != null) scoreText.text = $"Score {score}";
        }

        public override void UpdateStats(StatSystem s, NeedsSystem needs, float time)
        {
            SetLabel(fatherTirednessText, "Tiredness", s.FatherTiredness);
            SetLabel(babySleepinessText, "Sleepiness", s.BabySleepiness);
            SetLabel(moraleText, "Morale", s.Morale);
            SetLabel(guiltText, "Guilt", s.Guilt);

            if (needsText != null)
                needsText.text = $"Needs: {(int)s.Needs}{(needs != null ? needs.NeedsLabel() : "")}";

            if (timeText != null) timeText.text = $"Time: {(int)time}";

            SetBar(fatherTirednessBar, s.FatherTiredness);
            SetBar(babySleepinessBar, s.BabySleepiness);
            SetBar(moraleBar, s.Morale);
            SetBar(guiltBar, s.Guilt);
            SetBar(needsBar, s.Needs);
        }

        public override void ShowEnding(LevelOutcome outcome)
        {
            if (endingText == null) return;
            string msg = outcome switch
            {
                LevelOutcome.BabyAsleep   => "Baby is asleep. Tap to continue.",
                LevelOutcome.FatherAsleep => "You fell asleep. Tap to continue.",
                LevelOutcome.Depressed    => "You are depressed, you lost. Tap to continue.",
                _ => ""
            };
            endingText.text = msg;
        }

        private static void SetLabel(TMP_Text t, string label, float value)
        {
            if (t != null) t.text = $"{label} {(int)value}";
        }

        private static void SetBar(Image bar, float value0to100)
        {
            if (bar != null) bar.fillAmount = Mathf.Clamp01(value0to100 / 100f);
        }
    }
}
