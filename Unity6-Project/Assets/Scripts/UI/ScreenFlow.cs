using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace TiredDad
{
    /// <summary>
    /// Handles taps on the end-of-level / menu screens using the New Input System.
    /// One finger = continue (regenerate a new level), two fingers = back to menu.
    /// Attach to the GameManager's ending overlay, or use standalone in a menu scene.
    /// Replaces the touchCount branching in the old Score.cs / EndGame().
    /// </summary>
    public class ScreenFlow : MonoBehaviour
    {
        public GameManager gameManager;         // for in-place continue
        public string menuSceneName = "MainMenu";
        public string gameSceneName = "Game";

        [Tooltip("Only react to taps once the level has ended.")]
        public bool requireLevelEnded = true;

        void Update()
        {
            if (requireLevelEnded && gameManager != null && gameManager.running) return;

            int fingers = ActiveTouchCount();
            if (fingers == 1 && WasTapThisFrame())
                Continue();
            else if (fingers == 2 && WasTapThisFrame())
                ToMenu();
        }

        private void Continue()
        {
            if (gameManager != null) gameManager.ContinueToNextLevel();
            else SceneManager.LoadScene(gameSceneName);
        }

        private void ToMenu() => SceneManager.LoadScene(menuSceneName);

        private static int ActiveTouchCount()
        {
            var ts = Touchscreen.current;
            if (ts == null) return Mouse.current != null && Mouse.current.leftButton.isPressed ? 1 : 0;
            int n = 0;
            foreach (var t in ts.touches)
                if (t.press.isPressed) n++;
            return n;
        }

        private static bool WasTapThisFrame()
        {
            var ts = Touchscreen.current;
            if (ts != null) return ts.primaryTouch.press.wasPressedThisFrame;
            return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        }
    }
}
