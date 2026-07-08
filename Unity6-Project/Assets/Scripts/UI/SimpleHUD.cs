using UnityEngine;
using UnityEngine.UI;

namespace TiredDad
{
    /// <summary>
    /// Zero-setup HUD used by DemoBootstrap. Renders with legacy uGUI Text and Unity's
    /// built-in font, so it needs no TextMeshPro import and no assigned assets. Good
    /// enough to play/tune the demo; swap for HUDController + TMP for the polished v0.5 UI.
    /// </summary>
    public class SimpleHUD : GameHud
    {
        private Text _stats;
        private Text _ending;

        /// <summary>Builds a Canvas + two Text elements under this object.</summary>
        public void Build()
        {
            var canvasGO = new GameObject("HUDCanvas");
            canvasGO.transform.SetParent(transform, false);
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGO.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1080, 1920);
            canvasGO.AddComponent<GraphicRaycaster>();

            _stats  = MakeText(canvasGO.transform, "Stats", new Vector2(0, 1), new Vector2(20, -20),
                              TextAnchor.UpperLeft, 34, new Vector2(700, 400));
            _ending = MakeText(canvasGO.transform, "Ending", new Vector2(0.5f, 0.5f), Vector2.zero,
                              TextAnchor.MiddleCenter, 40, new Vector2(900, 300));
            _ending.color = Color.yellow;
        }

        private Text MakeText(Transform parent, string name, Vector2 anchor, Vector2 offset,
                              TextAnchor align, int size, Vector2 sizeDelta)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var t = go.AddComponent<Text>();
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.fontSize = size;
            t.alignment = align;
            t.color = Color.white;
            t.horizontalOverflow = HorizontalWrapMode.Overflow;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            var rt = t.rectTransform;
            rt.anchorMin = rt.anchorMax = anchor;
            rt.pivot = anchor;
            rt.anchoredPosition = offset;
            rt.sizeDelta = sizeDelta;
            return t;
        }

        public override void SetLevel(int level) { }   // shown in the stats block
        public override void SetScore(int score) { }

        public override void UpdateStats(StatSystem s, NeedsSystem needs, float time)
        {
            if (_stats == null) return;
            int lvl = ScoreManager.Instance != null ? ScoreManager.Instance.LevelIndex : 1;
            int score = ScoreManager.Instance != null ? ScoreManager.Instance.TotalScore : 0;
            string needLabel = needs != null ? needs.NeedsLabel() : "";
            _stats.text =
                $"Level {lvl}    Score {score}    Time {(int)time}\n" +
                $"Sleepiness  {(int)s.BabySleepiness} / {(int)s.sleepGoal}   (GOAL)\n" +
                $"Tiredness   {(int)s.FatherTiredness}\n" +
                $"Morale      {(int)s.Morale}\n" +
                $"Guilt       {(int)s.Guilt}\n" +
                $"Needs       {(int)s.Needs}{needLabel}";
        }

        public override void ShowEnding(LevelOutcome outcome)
        {
            if (_ending == null) return;
            _ending.text = outcome switch
            {
                LevelOutcome.BabyAsleep   => "Baby is asleep!  \nTap once to play the next level.",
                LevelOutcome.FatherAsleep => "You fell asleep.  \nTap once to try again.",
                LevelOutcome.Depressed    => "You are exhausted.  \nTap once to try again.",
                _ => ""
            };
        }
    }
}
