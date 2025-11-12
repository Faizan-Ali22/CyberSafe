using UnityEngine;
using CyberSafe.UI;
using System.Collections;

/// <summary>
/// Example integration script showing how to coordinate PodMenuAnimator with MainMenuAnimator
/// Attach this to the same GameObject as PodMenuAnimator for seamless menu transitions
/// </summary>
[RequireComponent(typeof(PodMenuAnimator))]
public class PodMenuIntegration : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the PodMenuAnimator component")]
    [SerializeField] private PodMenuAnimator podAnimator;
    
    [Tooltip("Optional: Reference to MainMenuAnimator for coordinated animations")]
    [SerializeField] private MainMenuAnimator mainMenuAnimator;
    
    [Tooltip("Optional: Reference to MainMenu for button functionality")]
    [SerializeField] private MainMenu mainMenu;

    [Header("Timing")]
    [Tooltip("Delay before enabling menu interactions after pods open")]
    [SerializeField] private float enableDelay = 0.2f;

    private void Awake()
    {
        if (podAnimator == null)
            podAnimator = GetComponent<PodMenuAnimator>();
    }

    private void Start()
    {
        StartCoroutine(InitializeMenu());
    }

    /// <summary>
    /// Initialize menu with pod animation
    /// </summary>
    private IEnumerator InitializeMenu()
    {
        // Disable main menu interactions initially
        if (mainMenu != null)
            mainMenu.enabled = false;

        // Play pod opening animation
        yield return podAnimator.PlayIntroAnimation();

        // Small delay for polish
        yield return new WaitForSeconds(enableDelay);

        // Enable menu functionality
        if (mainMenu != null)
            mainMenu.enabled = true;

        // Trigger MainMenuAnimator if present
        if (mainMenuAnimator != null)
            mainMenuAnimator.OpenMenu();

        Debug.Log("✅ Pod menu ready and interactive!");
    }

    /// <summary>
    /// Call this when transitioning out of the menu (e.g., starting a game)
    /// </summary>
    public IEnumerator TransitionOut()
    {
        // Disable interactions
        if (mainMenu != null)
            mainMenu.enabled = false;

        // Close main menu animator first if present
        if (mainMenuAnimator != null)
            mainMenuAnimator.CloseMenu();

        yield return new WaitForSeconds(0.3f);

        // Close pods
        yield return podAnimator.ClosePods();
    }

    /// <summary>
    /// Public method to close menu and load scene
    /// Call this from UI buttons
    /// </summary>
    public void CloseAndLoadScene(string sceneName)
    {
        StartCoroutine(CloseAndLoadSceneCoroutine(sceneName));
    }

    private IEnumerator CloseAndLoadSceneCoroutine(string sceneName)
    {
        yield return TransitionOut();
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (podAnimator == null)
            podAnimator = GetComponent<PodMenuAnimator>();
    }
#endif
}
