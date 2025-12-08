using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ToLab : MonoBehaviour
{
   [Header("Scene Settings")]
    [SerializeField] private string sceneName;

    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;

    [Header("UI Elements to Hide")]
    [SerializeField] private GameObject[] uiElementsToHide;

    private bool isLoading = false;

    void Start()
    {
        // Ensure fade canvas starts fully transparent
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Only load scene if the player enters the trigger and not already loading
        if (other.CompareTag("Player") && !isLoading)
        {
            isLoading = true;
            
            // Hide UI elements immediately
            HideUIElements();
            
            // Start fade and load scene
            StartCoroutine(FadeAndLoadScene());
        }
    }

    private void HideUIElements()
    {
        foreach (GameObject uiElement in uiElementsToHide)
        {
            if (uiElement != null)
            {
                uiElement.SetActive(false);
            }
        }
    }

    private IEnumerator FadeAndLoadScene()
    {
        // Start loading the scene asynchronously
        AsyncOperation asyncLoad = SceneManager. LoadSceneAsync(sceneName);
        
        // Prevent the scene from activating immediately when loaded
        asyncLoad.allowSceneActivation = false;

        // Fade alpha based on loading progress (0 to 0.9 range)
        // Note: progress stops at 0.9 when allowSceneActivation is false
        while (asyncLoad.progress < 0.9f)
        {
            // Map progress (0 to 0.9) to alpha (0 to 1)
            fadeCanvasGroup.alpha = Mathf. Clamp01(asyncLoad.progress / 0.9f);
            yield return null;
        }

        // Ensure alpha is exactly 1 before switching scenes
        fadeCanvasGroup.alpha = 1f;

        // Allow the scene to activate
        asyncLoad.allowSceneActivation = true;
    }
}
//Start-Attack