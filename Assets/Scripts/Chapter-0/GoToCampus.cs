using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GoToCampus : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject ToCampusUI;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private GameObject[] uiToHideOnClick; // Array of UI elements to hide

    [Header("Scene")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private int nextSceneIndex = 6; // Use -1 to use scene name instead

    [Header("Settings")]
    [SerializeField] private string playerTag = "Player";

    private bool isTransitioning = false;

    private void Start()
    {
        // Ensure UI is hidden at start
        if (ToCampusUI != null)
        {
            ToCampusUI.SetActive(false);
        }

        // Ensure fade image is transparent at start
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
            fadeImage.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !isTransitioning)
        {
            if (ToCampusUI != null)
            {
                ToCampusUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag) && !isTransitioning)
        {
            if (ToCampusUI != null)
            {
                ToCampusUI.SetActive(false);
            }
        }
    }

    // Call this method from the Button's OnClick event
    public void OnCampusClicked()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        // Hide the change clothes UI
        if (ToCampusUI != null)
        {
            ToCampusUI.SetActive(false);
        }

        // Hide all UI elements in the array
        foreach (GameObject uiElement in uiToHideOnClick)
        {
            if (uiElement != null)
            {
                uiElement.SetActive(false);
            }
        }

        StartCoroutine(FadeAndLoadScene());
    }

    private IEnumerator FadeAndLoadScene()
    {
        // Enable fade image
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
        }

        // Fade to black (alpha 0 to 1)
        yield return StartCoroutine(FadeImage(0f, 1f));

        // Load next scene asynchronously
        AsyncOperation asyncLoad;

        if (nextSceneIndex >= 0)
        {
            asyncLoad = SceneManager.LoadSceneAsync(nextSceneIndex);
        }
        else
        {
            asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);
        }

        // Wait until scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator FadeImage(float startAlpha, float endAlpha)
    {
        if (fadeImage == null) yield break;

        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            color.a = alpha;
            fadeImage.color = color;
            yield return null;
        }

        // Ensure final alpha is set
        color.a = endAlpha;
        fadeImage.color = color;
    }
}
