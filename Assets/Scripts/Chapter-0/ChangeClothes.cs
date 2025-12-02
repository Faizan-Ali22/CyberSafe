using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ChangeClothes : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject changeClothesUI;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Scene")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private int nextSceneIndex = -1; // Use -1 to use scene name instead

    [Header("Settings")]
    [SerializeField] private string playerTag = "Player";

    private bool isTransitioning = false;

    private void Start()
    {
        // Ensure UI is hidden at start
        if (changeClothesUI != null)
        {
            changeClothesUI.SetActive(false);
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
            if (changeClothesUI != null)
            {
                changeClothesUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag) && !isTransitioning)
        {
            if (changeClothesUI != null)
            {
                changeClothesUI.SetActive(false);
            }
        }
    }

    // Call this method from the Button's OnClick event
    public void OnChangeClothesButtonClicked()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        // Hide the change clothes UI
        if (changeClothesUI != null)
        {
            changeClothesUI.SetActive(false);
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
