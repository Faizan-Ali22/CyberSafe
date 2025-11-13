# Pod Menu Animation - Usage Examples

This document provides practical examples of using the PodMenuAnimator system in various scenarios.

## Example 1: Basic Setup (Minimal Code)

The simplest way to use the pod animation - just attach the component and configure in Inspector:

```csharp
using UnityEngine;
using CyberSafe.UI;

public class SimpleMenuController : MonoBehaviour
{
    // PodMenuAnimator handles everything automatically
    // Just configure references in Inspector
    
    // No code needed! The PodMenuAnimator will:
    // - Play intro animation on first launch
    // - Show quick open on subsequent launches
    // - Manage menu fade in/out
}
```

## Example 2: Manual Control

When you need programmatic control over the animation:

```csharp
using UnityEngine;
using CyberSafe.UI;
using System.Collections;

public class ManualMenuController : MonoBehaviour
{
    public PodMenuAnimator podAnimator;
    
    private void Start()
    {
        // Disable auto-play in Inspector
        // Then manually trigger:
        StartCoroutine(InitMenu());
    }
    
    private IEnumerator InitMenu()
    {
        // Wait for some condition
        yield return new WaitForSeconds(1f);
        
        // Then open
        yield return podAnimator.OpenPods();
        
        Debug.Log("Menu is now open!");
    }
    
    public void OnStartGamePressed()
    {
        StartCoroutine(TransitionToGame());
    }
    
    private IEnumerator TransitionToGame()
    {
        // Close pods before loading scene
        yield return podAnimator.ClosePods();
        
        // Load game
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}
```

## Example 3: Integration with Existing MainMenuAnimator

Coordinating both animation systems:

```csharp
using UnityEngine;
using CyberSafe.UI;
using System.Collections;

public class CoordinatedMenuController : MonoBehaviour
{
    public PodMenuAnimator podAnimator;
    public MainMenuAnimator menuAnimator;
    public MainMenu mainMenu;
    
    private IEnumerator Start()
    {
        // Disable menu initially
        if (mainMenu) mainMenu.enabled = false;
        
        // Play pod intro
        yield return podAnimator.PlayIntroAnimation();
        
        // Small polish delay
        yield return new WaitForSeconds(0.2f);
        
        // Activate MainMenuAnimator
        if (menuAnimator) menuAnimator.OpenMenu();
        
        // Enable menu interactions
        if (mainMenu) mainMenu.enabled = true;
    }
    
    public void OnQuitPressed()
    {
        StartCoroutine(QuitSequence());
    }
    
    private IEnumerator QuitSequence()
    {
        // Disable input
        if (mainMenu) mainMenu.enabled = false;
        
        // Close main menu animator
        if (menuAnimator) menuAnimator.CloseMenu();
        
        yield return new WaitForSeconds(0.3f);
        
        // Close pods
        yield return podAnimator.ClosePods();
        
        // Quit
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
```

## Example 4: Scene Transition with Fade

Adding a fade effect during transitions:

```csharp
using UnityEngine;
using CyberSafe.UI;
using System.Collections;
using UnityEngine.UI;

public class FadeMenuController : MonoBehaviour
{
    public PodMenuAnimator podAnimator;
    public Image fadePanel; // Black panel for fading
    
    private IEnumerator Start()
    {
        // Start with black screen
        fadePanel.color = Color.black;
        
        // Fade in
        yield return FadeOut(1f);
        
        // Open pods
        yield return podAnimator.OpenPods();
    }
    
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneWithTransition(sceneName));
    }
    
    private IEnumerator LoadSceneWithTransition(string sceneName)
    {
        // Close pods
        yield return podAnimator.ClosePods();
        
        // Fade to black
        yield return FadeIn(0.5f);
        
        // Load scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
    
    private IEnumerator FadeIn(float duration)
    {
        float elapsed = 0f;
        Color c = fadePanel.color;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, elapsed / duration);
            fadePanel.color = c;
            yield return null;
        }
        
        c.a = 1f;
        fadePanel.color = c;
    }
    
    private IEnumerator FadeOut(float duration)
    {
        float elapsed = 0f;
        Color c = fadePanel.color;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsed / duration);
            fadePanel.color = c;
            yield return null;
        }
        
        c.a = 0f;
        fadePanel.color = c;
    }
}
```

## Example 5: With Sound Effects

Adding audio feedback to the animations:

```csharp
using UnityEngine;
using CyberSafe.UI;
using System.Collections;

public class AudioMenuController : MonoBehaviour
{
    public PodMenuAnimator podAnimator;
    public AudioSource audioSource;
    public AudioClip podsOpeningSound;
    public AudioClip podsClosingSound;
    
    private IEnumerator Start()
    {
        yield return PlayIntroWithSound();
    }
    
    private IEnumerator PlayIntroWithSound()
    {
        // Play opening sound
        if (audioSource && podsOpeningSound)
        {
            audioSource.PlayOneShot(podsOpeningSound);
        }
        
        // Play animation
        yield return podAnimator.PlayIntroAnimation();
    }
    
    public void CloseMenuWithSound()
    {
        StartCoroutine(CloseWithSoundCoroutine());
    }
    
    private IEnumerator CloseWithSoundCoroutine()
    {
        // Play closing sound
        if (audioSource && podsClosingSound)
        {
            audioSource.PlayOneShot(podsClosingSound);
        }
        
        // Close animation
        yield return podAnimator.ClosePods();
    }
}
```

## Example 6: Dynamic Pod Customization

Changing pod behavior at runtime:

```csharp
using UnityEngine;
using CyberSafe.UI;
using System.Collections;

public class DynamicPodController : MonoBehaviour
{
    public PodMenuAnimator podAnimator;
    
    // Reference to access private fields via reflection or
    // modify public properties if exposed
    
    public void SetFastAnimation()
    {
        // Adjust speed for quick transitions
        // Note: You'd need to expose these fields as public in PodMenuAnimator
        // or use SerializedObject for editor-time changes
        
        Debug.Log("Setting fast animation mode");
        // podAnimator.openDuration = 0.5f;
        // podAnimator.closeDuration = 0.3f;
    }
    
    public void SetSlowAnimation()
    {
        Debug.Log("Setting slow animation mode");
        // podAnimator.openDuration = 2.5f;
        // podAnimator.closeDuration = 2.0f;
    }
    
    public void TogglePods()
    {
        StartCoroutine(TogglePodsCoroutine());
    }
    
    private IEnumerator TogglePodsCoroutine()
    {
        // Check if pods are open (you'd need to expose isOpen)
        // For now, just demonstrate the API
        
        yield return podAnimator.ClosePods();
        yield return new WaitForSeconds(0.5f);
        yield return podAnimator.OpenPods();
    }
}
```

## Example 7: Intro Scene to Main Menu Transition

Transitioning from an intro scene to the main menu with pod animation:

```csharp
using UnityEngine;
using CyberSafe.UI;
using System.Collections;
using UnityEngine.SceneManagement;

// Attach to IntroSceneManager or similar
public class IntroToMenuTransition : MonoBehaviour
{
    public float introDuration = 5f;
    public string mainMenuSceneName = "MainMenu";
    
    private IEnumerator Start()
    {
        // Play your intro sequence
        yield return new WaitForSeconds(introDuration);
        
        // Load main menu scene
        // The PodMenuAnimator in that scene will auto-play
        SceneManager.LoadScene(mainMenuSceneName);
    }
}

// In MainMenu scene:
public class MainMenuWithPods : MonoBehaviour
{
    public PodMenuAnimator podAnimator;
    
    private void Start()
    {
        // PodMenuAnimator auto-plays if configured
        // Or manually trigger:
        // StartCoroutine(podAnimator.PlayIntroAnimation());
    }
}
```

## Example 8: Sub-Menu with Pods

Using pods for sub-menus (settings, credits, etc):

```csharp
using UnityEngine;
using CyberSafe.UI;
using System.Collections;

public class SubMenuController : MonoBehaviour
{
    public PodMenuAnimator mainMenuPods;
    public PodMenuAnimator settingsMenuPods;
    
    public void ShowSettings()
    {
        StartCoroutine(TransitionToSettings());
    }
    
    private IEnumerator TransitionToSettings()
    {
        // Close main menu
        yield return mainMenuPods.ClosePods();
        
        // Small delay
        yield return new WaitForSeconds(0.2f);
        
        // Open settings menu
        yield return settingsMenuPods.OpenPods();
    }
    
    public void BackToMainMenu()
    {
        StartCoroutine(TransitionBackToMain());
    }
    
    private IEnumerator TransitionBackToMain()
    {
        // Close settings
        yield return settingsMenuPods.ClosePods();
        
        yield return new WaitForSeconds(0.2f);
        
        // Open main menu
        yield return mainMenuPods.OpenPods();
    }
}
```

## Example 9: Testing and Debugging

Utility for testing the animation:

```csharp
using UnityEngine;
using CyberSafe.UI;
using System.Collections;

public class PodAnimationTester : MonoBehaviour
{
    public PodMenuAnimator podAnimator;
    public bool debugMode = true;
    
    private void Update()
    {
        if (!debugMode) return;
        
        // Keyboard shortcuts for testing
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("Testing: Open Pods");
            podAnimator.Open();
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Testing: Close Pods");
            podAnimator.Close();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Testing: Reset Intro Flag");
            podAnimator.ResetIntroFlag();
        }
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Testing: Play Intro");
            StartCoroutine(podAnimator.PlayIntroAnimation());
        }
    }
}
```

## Example 10: Button Integration

Hooking up Unity UI buttons:

```csharp
using UnityEngine;
using CyberSafe.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuButtons : MonoBehaviour
{
    public PodMenuAnimator podAnimator;
    
    // Call from Button.onClick in Inspector
    public void OnNewGame()
    {
        StartCoroutine(LoadGameScene());
    }
    
    // Call from Button.onClick in Inspector
    public void OnContinue()
    {
        StartCoroutine(LoadGameScene());
    }
    
    // Call from Button.onClick in Inspector
    public void OnSettings()
    {
        Debug.Log("Opening Settings...");
        // Open settings sub-menu
    }
    
    // Call from Button.onClick in Inspector
    public void OnQuit()
    {
        StartCoroutine(QuitGame());
    }
    
    private IEnumerator LoadGameScene()
    {
        yield return podAnimator.ClosePods();
        SceneManager.LoadScene("Game");
    }
    
    private IEnumerator QuitGame()
    {
        yield return podAnimator.ClosePods();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
```

## Tips and Best Practices

1. **Always yield on coroutines**: Wait for animations to complete
2. **Disable input during transitions**: Prevents double-clicks
3. **Use PlayerPrefs wisely**: Clear during development with ResetIntroFlag()
4. **Test both first and subsequent launches**: Different animation paths
5. **Add audio feedback**: Makes the UI feel more responsive
6. **Consider performance**: Avoid heavy operations during animation
7. **Use CanvasGroup for groups**: Easier to fade entire UI sections
8. **Coordinate with other systems**: MainMenuAnimator, scene loaders, etc.
9. **Handle edge cases**: Scene reloads, application pause, etc.
10. **Profile in builds**: Editor performance differs from builds

## Common Pitfalls

❌ **DON'T**: Call Open() and Close() in rapid succession
✅ **DO**: Wait for animations to complete with yield

❌ **DON'T**: Forget to assign references in Inspector
✅ **DO**: Use OnValidate warnings to catch missing refs

❌ **DON'T**: Modify RectTransforms manually during animation
✅ **DO**: Let PodMenuAnimator handle all transforms

❌ **DON'T**: Have multiple PodMenuAnimators fighting for control
✅ **DO**: Coordinate through a single controller script

❌ **DON'T**: Skip the CanvasGroup on menuContent
✅ **DO**: Always add CanvasGroup for proper fading

---

**Need more examples?** Check the README and PodMenuIntegration.cs for additional patterns!
