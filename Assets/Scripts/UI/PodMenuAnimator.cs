using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CyberSafe.UI
{
    /// <summary>
    /// PodMenuAnimator - Creates a futuristic pod opening animation for the main menu.
    /// 
    /// This script animates two side pods (left and right) that start closed and then
    /// stretch/slide open to reveal the main menu UI. Inspired by futuristic sci-fi interfaces.
    /// 
    /// Setup Requirements:
    /// - Assign left and right pod Image components (LeftPod Menu.png, RightPod Menu.png)
    /// - Assign the main menu Image component (Main Menu.png or Empty Menu.png as background)
    /// - Attach this script to a GameObject in the menu scene
    /// - Optionally integrate with MainMenuAnimator for coordinated animations
    /// 
    /// Animation Flow:
    /// 1. Pods start closed (overlapping center, covering menu)
    /// 2. Pods animate outward (sliding and stretching) to reveal menu
    /// 3. Menu content fades in as pods open
    /// 4. Once complete, menu is fully interactive
    /// </summary>
    public class PodMenuAnimator : MonoBehaviour
    {
        [Header("Pod References")]
        [Tooltip("Left pod image (LeftPod Menu.png)")]
        [SerializeField] private RectTransform leftPod;
        
        [Tooltip("Right pod image (RightPod Menu.png)")]
        [SerializeField] private RectTransform rightPod;
        
        [Tooltip("Main menu content that appears as pods open")]
        [SerializeField] private CanvasGroup menuContent;
        
        [Tooltip("Optional: Background image (Empty Menu.png or Main Menu.png)")]
        [SerializeField] private Image backgroundImage;

        [Header("Animation Settings")]
        [Tooltip("Duration of the pod opening animation in seconds")]
        [SerializeField] private float openDuration = 1.5f;
        
        [Tooltip("Duration of the pod closing animation in seconds")]
        [SerializeField] private float closeDuration = 1.0f;
        
        [Tooltip("Ease curve for the opening animation")]
        [SerializeField] private AnimationCurve openCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        [Tooltip("How far the pods move horizontally when opening (in pixels)")]
        [SerializeField] private float podSlideDistance = 500f;
        
        [Tooltip("Scale multiplier for pods during opening (stretching effect)")]
        [SerializeField] private float podStretchScale = 1.2f;

        [Header("Intro Settings")]
        [Tooltip("If true, plays intro animation on Start")]
        [SerializeField] private bool playIntroOnStart = true;
        
        [Tooltip("Delay before starting intro animation")]
        [SerializeField] private float introDelay = 0.3f;
        
        [Tooltip("PlayerPrefs key to track if intro has played")]
        [SerializeField] private string introPlayedKey = "PodMenuIntroPlayed";

        [Header("Debug")]
        [Tooltip("Enable debug logs")]
        [SerializeField] private bool debugLogs = false;

        // Cached initial positions and scales
        private Vector2 leftPodInitialPos;
        private Vector2 rightPodInitialPos;
        private Vector3 leftPodInitialScale;
        private Vector3 rightPodInitialScale;
        
        private bool isAnimating = false;
        private bool isOpen = false;

        private void Awake()
        {
            // Cache initial transforms
            if (leftPod != null)
            {
                leftPodInitialPos = leftPod.anchoredPosition;
                leftPodInitialScale = leftPod.localScale;
            }
            
            if (rightPod != null)
            {
                rightPodInitialPos = rightPod.anchoredPosition;
                rightPodInitialScale = rightPod.localScale;
            }

            // Initially hide menu content
            if (menuContent != null)
            {
                menuContent.alpha = 0f;
                menuContent.interactable = false;
                menuContent.blocksRaycasts = false;
            }
        }

        private void Start()
        {
            if (playIntroOnStart)
            {
                bool hasPlayedIntro = PlayerPrefs.GetInt(introPlayedKey, 0) == 1;
                
                if (!hasPlayedIntro)
                {
                    // First time - play full intro
                    if (debugLogs) Debug.Log("[PodMenuAnimator] Playing intro animation (first time)");
                    StartCoroutine(PlayIntroAnimation());
                    PlayerPrefs.SetInt(introPlayedKey, 1);
                    PlayerPrefs.Save();
                }
                else
                {
                    // Subsequent times - quick open
                    if (debugLogs) Debug.Log("[PodMenuAnimator] Playing quick open animation");
                    StartCoroutine(PlayIntroAnimation());
                }
            }
        }

        /// <summary>
        /// Plays the intro animation sequence: pods closed -> pods open -> menu revealed
        /// </summary>
        public IEnumerator PlayIntroAnimation()
        {
            yield return new WaitForSeconds(introDelay);
            
            // Ensure pods are in closed position
            SetPodsToClosedPosition();
            
            // Open the pods
            yield return StartCoroutine(OpenPods());
        }

        /// <summary>
        /// Opens the pods with sliding and stretching animation
        /// </summary>
        public IEnumerator OpenPods()
        {
            if (isAnimating)
            {
                if (debugLogs) Debug.LogWarning("[PodMenuAnimator] Already animating, skipping");
                yield break;
            }

            isAnimating = true;
            if (debugLogs) Debug.Log("[PodMenuAnimator] Opening pods...");

            float elapsed = 0f;
            
            // Calculate target positions (pods slide outward)
            Vector2 leftPodTargetPos = leftPodInitialPos - new Vector2(podSlideDistance, 0);
            Vector2 rightPodTargetPos = rightPodInitialPos + new Vector2(podSlideDistance, 0);
            
            // Calculate target scales (pods stretch)
            Vector3 leftPodTargetScale = leftPodInitialScale * podStretchScale;
            Vector3 rightPodTargetScale = rightPodInitialScale * podStretchScale;

            while (elapsed < openDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / openDuration;
                float curveValue = openCurve.Evaluate(t);

                // Animate left pod
                if (leftPod != null)
                {
                    leftPod.anchoredPosition = Vector2.Lerp(leftPodInitialPos, leftPodTargetPos, curveValue);
                    leftPod.localScale = Vector3.Lerp(leftPodInitialScale, leftPodTargetScale, curveValue);
                }

                // Animate right pod
                if (rightPod != null)
                {
                    rightPod.anchoredPosition = Vector2.Lerp(rightPodInitialPos, rightPodTargetPos, curveValue);
                    rightPod.localScale = Vector3.Lerp(rightPodInitialScale, rightPodTargetScale, curveValue);
                }

                // Fade in menu content as pods open
                if (menuContent != null)
                {
                    menuContent.alpha = Mathf.Lerp(0f, 1f, curveValue);
                }

                yield return null;
            }

            // Ensure final state
            if (leftPod != null)
            {
                leftPod.anchoredPosition = leftPodTargetPos;
                leftPod.localScale = leftPodTargetScale;
            }
            
            if (rightPod != null)
            {
                rightPod.anchoredPosition = rightPodTargetPos;
                rightPod.localScale = rightPodTargetScale;
            }

            if (menuContent != null)
            {
                menuContent.alpha = 1f;
                menuContent.interactable = true;
                menuContent.blocksRaycasts = true;
            }

            isOpen = true;
            isAnimating = false;
            if (debugLogs) Debug.Log("[PodMenuAnimator] Pods opened successfully");
        }

        /// <summary>
        /// Closes the pods with reverse animation
        /// </summary>
        public IEnumerator ClosePods()
        {
            if (isAnimating)
            {
                if (debugLogs) Debug.LogWarning("[PodMenuAnimator] Already animating, skipping");
                yield break;
            }

            isAnimating = true;
            if (debugLogs) Debug.Log("[PodMenuAnimator] Closing pods...");

            float elapsed = 0f;
            
            // Get current positions
            Vector2 leftPodCurrentPos = leftPod != null ? leftPod.anchoredPosition : Vector2.zero;
            Vector2 rightPodCurrentPos = rightPod != null ? rightPod.anchoredPosition : Vector2.zero;
            Vector3 leftPodCurrentScale = leftPod != null ? leftPod.localScale : Vector3.one;
            Vector3 rightPodCurrentScale = rightPod != null ? rightPod.localScale : Vector3.one;

            while (elapsed < closeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / closeDuration;
                float curveValue = openCurve.Evaluate(t);

                // Animate left pod back
                if (leftPod != null)
                {
                    leftPod.anchoredPosition = Vector2.Lerp(leftPodCurrentPos, leftPodInitialPos, curveValue);
                    leftPod.localScale = Vector3.Lerp(leftPodCurrentScale, leftPodInitialScale, curveValue);
                }

                // Animate right pod back
                if (rightPod != null)
                {
                    rightPod.anchoredPosition = Vector2.Lerp(rightPodCurrentPos, rightPodInitialPos, curveValue);
                    rightPod.localScale = Vector3.Lerp(rightPodCurrentScale, rightPodInitialScale, curveValue);
                }

                // Fade out menu content as pods close
                if (menuContent != null)
                {
                    menuContent.alpha = Mathf.Lerp(1f, 0f, curveValue);
                }

                yield return null;
            }

            // Ensure final state
            SetPodsToClosedPosition();
            
            if (menuContent != null)
            {
                menuContent.alpha = 0f;
                menuContent.interactable = false;
                menuContent.blocksRaycasts = false;
            }

            isOpen = false;
            isAnimating = false;
            if (debugLogs) Debug.Log("[PodMenuAnimator] Pods closed successfully");
        }

        /// <summary>
        /// Sets pods to their closed (initial) position
        /// </summary>
        private void SetPodsToClosedPosition()
        {
            if (leftPod != null)
            {
                leftPod.anchoredPosition = leftPodInitialPos;
                leftPod.localScale = leftPodInitialScale;
            }
            
            if (rightPod != null)
            {
                rightPod.anchoredPosition = rightPodInitialPos;
                rightPod.localScale = rightPodInitialScale;
            }
        }

        /// <summary>
        /// Public method to manually trigger pod opening
        /// </summary>
        public void Open()
        {
            if (!isOpen && !isAnimating)
            {
                StartCoroutine(OpenPods());
            }
        }

        /// <summary>
        /// Public method to manually trigger pod closing
        /// </summary>
        public void Close()
        {
            if (isOpen && !isAnimating)
            {
                StartCoroutine(ClosePods());
            }
        }

        /// <summary>
        /// Resets the intro flag (useful for testing)
        /// </summary>
        public void ResetIntroFlag()
        {
            PlayerPrefs.DeleteKey(introPlayedKey);
            PlayerPrefs.Save();
            if (debugLogs) Debug.Log("[PodMenuAnimator] Intro flag reset");
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Ensure we have valid references in editor
            if (leftPod == null)
                Debug.LogWarning("[PodMenuAnimator] Left pod not assigned!");
            
            if (rightPod == null)
                Debug.LogWarning("[PodMenuAnimator] Right pod not assigned!");
            
            if (menuContent == null)
                Debug.LogWarning("[PodMenuAnimator] Menu content not assigned!");
        }
#endif
    }
}
