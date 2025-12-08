using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MonitorTapHotspot : MonoBehaviour
{
     [Header("Scene")]
    [SerializeField] private string mazeSceneName = "Game01";

    [Header("Intro")]
    [SerializeField] private Canvas logoIntroCanvas;
    [SerializeField] private CanvasGroup logoCanvasGroup; // assign the CanvasGroup on the logo canvas
    [SerializeField] private float logoDuration = 2f;
    [SerializeField] private float fadeDuration = 0.25f;

    [Header("Player")]
    [SerializeField] private Transform playerTransform; // assign your player root

    [Header("UI to hide on tap")]
    [SerializeField] private GameObject[] uiElementsToHide; // assign any UI elements you want hidden on tap

    [Header("Screen Id")]
    [SerializeField] private int screenId = 0; // set per monitor: Hacked-1 -> 0, Hacked-2 -> 1, ...

    private bool busy;

    private void Awake()
    {
        // Force logo hidden initially
        if (logoIntroCanvas) logoIntroCanvas.enabled = true;
        if (logoCanvasGroup)
        {
            logoCanvasGroup.alpha = 0f;
            logoCanvasGroup.blocksRaycasts = false;
            logoCanvasGroup.interactable = false;
        }
        else if (logoIntroCanvas)
        {
            logoIntroCanvas.enabled = false;
        }
    }

    // Hook this to the UI Button's OnClick()
    public void OnTapped()
    {
        if (busy) return;

        // Remember which screen was tapped
        LabReturnState.SetSelectedScreenId(screenId);

        // Hide extra UI
        if (uiElementsToHide != null)
        {
            foreach (var go in uiElementsToHide)
            {
                if (go) go.SetActive(false);
            }
        }

        StartCoroutine(LoadMazeRoutine());
    }

    private IEnumerator LoadMazeRoutine()
    {
        busy = true;

        // Fade in logo
        if (logoIntroCanvas) logoIntroCanvas.enabled = true;
        if (logoCanvasGroup)
        {
            logoCanvasGroup.blocksRaycasts = true;
            logoCanvasGroup.interactable = true;
            yield return StartCoroutine(FadeCanvasGroup(logoCanvasGroup, 0f, 1f, fadeDuration));
        }

        // Begin async load immediately, but hold activation
        AsyncOperation op = SceneManager.LoadSceneAsync(mazeSceneName);
        op.allowSceneActivation = false;

        // Keep logo up for the intro duration
        yield return new WaitForSecondsRealtime(logoDuration);

        // Save player pose
        if (playerTransform) LabReturnState.SavePlayerPose(playerTransform);

        // Wait until scene is ready
        while (op.progress < 0.9f)
            yield return null;

        // Fade out logo (optional)
        if (logoCanvasGroup)
        {
            yield return StartCoroutine(FadeCanvasGroup(logoCanvasGroup, 1f, 0f, fadeDuration));
            logoCanvasGroup.blocksRaycasts = false;
            logoCanvasGroup.interactable = false;
        }
        else if (logoIntroCanvas)
        {
            logoIntroCanvas.enabled = false;
        }

        // Activate the scene
        op.allowSceneActivation = true;
        while (!op.isDone)
            yield return null;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        float t = 0f;
        cg.alpha = from;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        cg.alpha = to;
    }
}