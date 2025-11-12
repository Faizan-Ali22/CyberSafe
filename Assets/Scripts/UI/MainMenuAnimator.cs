using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// MainMenuAnimator
// Drives first-time intro animation and regular open/close transitions for a main menu panel.
// Attach to the root GameObject of your Main Menu (the panel that holds buttons).
// Requirements:
//  - Animator with states: Closed (default), Open, IntroOpen, Close (optional if not using reverse of Open)
//  - Animator triggers: trIntro, trOpen, trClose
//  - CanvasGroup on same GameObject for fading & input gating
// Usage:
//  - On first launch (PlayerPrefs flag) IntroOpen plays, then normal Open is used afterwards.
//  - Assign button OnClick events to methods like CloseAndLoadScene or OpenMenu.
//  - For sub-panels (Settings, Credits) you can temporarily CloseMenu() then Open that panel.
//
// Edge cases handled:
//  - Animator reference auto-fetched if missing.
//  - Prevents interaction while animating.
//  - Safe waiting for state start & finish to avoid race conditions if Animator transitions blend.
//  - Scene loading waits until menu fully closed.
//
// Optional: You can add subtle side-element animations by creating additional scripts that trigger
// during IntroOpen via Animation Events or separate animators.
namespace CyberSafe.UI
{
    public class MainMenuAnimator : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Animator with states: Closed (default), Open, IntroOpen, Close (or use Closed for closing).")]
        public Animator animator;
        [Tooltip("CanvasGroup for fading and input gating.")] public CanvasGroup canvasGroup;

        [Header("Config")]
        [Tooltip("PlayerPrefs key tracking whether intro has already played.")] public string introPlayedKey = "MainMenuIntroPlayed";
        [Tooltip("If true, will always play the intro regardless of PlayerPrefs key.")] public bool forceIntroThisSession = false;

        // State names (must match Animator clip state names on layer 0)
        private const string StateOpen = "Open";
        private const string StateClosed = "Closed"; // default
        private const string StateIntro = "IntroOpen";
        private const string StateClose = "Close"; // optional; if omitted animator should transition to Closed

        // Trigger hashes
        private static readonly int TrIntro = Animator.StringToHash("trIntro");
        private static readonly int TrOpen = Animator.StringToHash("trOpen");
        private static readonly int TrClose = Animator.StringToHash("trClose");

        private bool _introScheduled;

        private void Awake()
        {
            if (!animator) animator = GetComponent<Animator>();
            if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
            SetInteractable(false);
        }

        private void Start()
        {
            bool played = PlayerPrefs.GetInt(introPlayedKey, 0) == 1;
            if (!played || forceIntroThisSession)
            {
                _introScheduled = true;
                animator.ResetTrigger(TrOpen);
                animator.SetTrigger(TrIntro);
                if (!forceIntroThisSession)
                {
                    PlayerPrefs.SetInt(introPlayedKey, 1);
                    PlayerPrefs.Save();
                }
                StartCoroutine(CoEnableWhenStateFinished(StateIntro));
            }
            else
            {
                animator.SetTrigger(TrOpen);
                StartCoroutine(CoEnableWhenStateReached(StateOpen));
            }
        }

        /// <summary>
        /// Opens the menu (normal open).
        /// </summary>
        public void OpenMenu()
        {
            SetInteractable(false);
            animator.SetTrigger(TrOpen);
            StartCoroutine(CoEnableWhenStateReached(StateOpen));
        }

        /// <summary>
        /// Closes the menu (normal close). Interaction disabled during animation.
        /// </summary>
        public void CloseMenu()
        {
            SetInteractable(false);
            animator.SetTrigger(TrClose);
        }

        /// <summary>
        /// Plays the intro animation again (if you want to manually replay, e.g., from an options toggle).
        /// </summary>
        public void ReplayIntro()
        {
            SetInteractable(false);
            animator.SetTrigger(TrIntro);
            StartCoroutine(CoEnableWhenStateFinished(StateIntro));
        }

        /// <summary>
        /// Close menu and then load the target scene after animation finishes.
        /// </summary>
        public void CloseAndLoadScene(string sceneName)
        {
            StartCoroutine(CoCloseAndLoad(sceneName));
        }

        private IEnumerator CoCloseAndLoad(string sceneName)
        {
            CloseMenu();
            // Wait until we get into either Close or Closed state, then until finished.
            yield return CoWaitForStateStart(StateClose, StateClosed);
            yield return CoWaitForStateFinish(StateClose, StateClosed);
            yield return SceneManager.LoadSceneAsync(sceneName);
        }

        private void SetInteractable(bool value)
        {
            if (!canvasGroup) return;
            canvasGroup.interactable = value;
            canvasGroup.blocksRaycasts = value;
        }

        #region Coroutines
        private IEnumerator CoEnableWhenStateReached(string stateName)
        {
            yield return CoWaitForStateStart(stateName);
            SetInteractable(true);
        }

        private IEnumerator CoEnableWhenStateFinished(string stateName)
        {
            yield return CoWaitForStateStart(stateName);
            yield return CoWaitForStateFinish(stateName);
            SetInteractable(true);
        }

        private IEnumerator CoWaitForStateStart(params string[] possibleNames)
        {
            bool started = false;
            while (!started)
            {
                var info = animator.GetCurrentAnimatorStateInfo(0);
                foreach (var p in possibleNames)
                {
                    if (info.IsName(p)) { started = true; break; }
                }
                yield return null;
            }
        }

        private IEnumerator CoWaitForStateFinish(params string[] possibleNames)
        {
            bool finished = false;
            while (!finished)
            {
                var info = animator.GetCurrentAnimatorStateInfo(0);
                bool inState = false;
                foreach (var p in possibleNames)
                {
                    if (info.IsName(p)) { inState = true; break; }
                }
                if (inState && info.normalizedTime >= 1f)
                {
                    finished = true;
                    break;
                }
                yield return null;
            }
        }
        #endregion

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!animator) animator = GetComponent<Animator>();
            if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
        }
#endif
    }
}
