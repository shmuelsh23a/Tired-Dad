using UnityEngine;
using UnityEngine.InputSystem;

namespace TiredDad
{
    /// <summary>
    /// Reads touch on the New Input System and produces two signals:
    ///   * Tap tempo   — taps in the valid rhythm window build "tempo" (drives walking).
    ///   * Swipe       — a horizontal swipe past a threshold requests a 90° turn.
    ///
    /// Rhythm rule (from Readme.docx): tap faster than once / MaxInterval but slower
    /// than once / MinInterval. Taps inside the window raise tempo; being outside it
    /// (too fast, too slow, or idle) lets tempo decay. This replaces the old Walker.cs
    /// tempo logic, which contained a branch that could never evaluate true.
    /// </summary>
    public class TouchInputController : MonoBehaviour
    {
        [Header("Rhythm window (seconds between taps)")]
        public float minInterval = 0.5f;   // taps closer than this = too fast
        public float maxInterval = 2.0f;   // taps farther than this = too slow

        [Header("Tempo")]
        [Range(1, 10)] public int maxTempo = 5;
        public float tempoDecayPerSecond = 1.0f;   // tempo lost while not tapping in rhythm

        [Header("Swipe")]
        public float swipeThresholdPixels = 75f;

        // Outputs consumed by PlayerMover.
        public int Tempo { get; private set; }
        public float Tempo01 => maxTempo > 0 ? (float)Tempo / maxTempo : 0f;
        /// <summary>+1 = turn right (CW), -1 = turn left (CCW), 0 = no turn this frame.</summary>
        public int TurnRequest { get; private set; }

        private float _lastTapTime = -999f;
        private Vector2 _touchStartPos;
        private bool _touchActive;
        private float _decayAccumulator;

        void Update()
        {
            TurnRequest = 0;
            var ts = Touchscreen.current;
            if (ts == null)
            {
                // Editor fallback: mouse click acts as a tap.
                HandleMouseFallback();
                DecayTempo(Time.deltaTime);
                return;
            }

            var touch = ts.primaryTouch;
            bool pressedThisFrame  = touch.press.wasPressedThisFrame;
            bool releasedThisFrame = touch.press.wasReleasedThisFrame;

            if (pressedThisFrame)
            {
                _touchStartPos = touch.position.ReadValue();
                _touchActive = true;
                RegisterTap();
            }

            if (releasedThisFrame && _touchActive)
            {
                Vector2 endPos = touch.position.ReadValue();
                EvaluateSwipe(_touchStartPos, endPos);
                _touchActive = false;
            }

            DecayTempo(Time.deltaTime);
        }

        private void RegisterTap()
        {
            float now = Time.time;
            float interval = now - _lastTapTime;

            // Only taps within the valid rhythm window build tempo.
            if (interval >= minInterval && interval <= maxInterval)
            {
                if (Tempo < maxTempo) Tempo++;
                _decayAccumulator = 0f;
            }
            else if (interval < minInterval)
            {
                // Too fast — mild penalty so mashing doesn't help.
                if (Tempo > 0) Tempo--;
            }
            // interval > maxInterval: treated as a fresh start; decay already handled it.

            _lastTapTime = now;
        }

        private void DecayTempo(float dt)
        {
            if (Time.time - _lastTapTime <= maxInterval) return; // still "in rhythm"
            _decayAccumulator += tempoDecayPerSecond * dt;
            while (_decayAccumulator >= 1f && Tempo > 0)
            {
                Tempo--;
                _decayAccumulator -= 1f;
            }
        }

        private void EvaluateSwipe(Vector2 start, Vector2 end)
        {
            float dx = end.x - start.x;
            if (dx > swipeThresholdPixels) TurnRequest = +1;
            else if (dx < -swipeThresholdPixels) TurnRequest = -1;
        }

        private void HandleMouseFallback()
        {
            var mouse = Mouse.current;
            if (mouse == null) return;
            if (mouse.leftButton.wasPressedThisFrame)
            {
                _touchStartPos = mouse.position.ReadValue();
                _touchActive = true;
                RegisterTap();
            }
            if (mouse.leftButton.wasReleasedThisFrame && _touchActive)
            {
                EvaluateSwipe(_touchStartPos, mouse.position.ReadValue());
                _touchActive = false;
            }
        }
    }
}
