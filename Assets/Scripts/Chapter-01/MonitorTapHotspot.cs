using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MonitorTapHotspot : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string mazeSceneName = "Game01";

    [Header("Intro")]
    [SerializeField] private Canvas logoIntroCanvas;
    [SerializeField] private CanvasGroup logoCanvasGroup;
    [SerializeField] private float logoDuration = 1f;
    [SerializeField] private float fadeDuration = 0.15f;

    [Header("Player")]
    [SerializeField] private Transform playerTransform;

    [Header("UI to hide on tap")]
    [SerializeField] private GameObject[] uiElementsToHide;

    private bool busy;

    private void Awake()
    {
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

    // This is what each Button will call, with a different index value.
    public void OnTappedWithIndex(int hackedIndex)
    {
        if (busy) return;

        HackedRunState.SetLastHackedIndex(hackedIndex);
        Debug.Log($"MonitorTapHotspot: tapped hacked index {hackedIndex}", this);

        if (uiElementsToHide != null)
        {
            foreach (var go in uiElementsToHide)
                if (go) go.SetActive(false);
        }

        StartCoroutine(LoadMazeRoutine());
    }

    private IEnumerator LoadMazeRoutine()
    {
        busy = true;

        // Begin async load immediately
        AsyncOperation op = SceneManager.LoadSceneAsync(mazeSceneName);
        op.allowSceneActivation = false;

        // Fade in logo (counts toward total intro time)
        float fadeInTime = 0f;
        if (logoIntroCanvas) logoIntroCanvas.enabled = true;
        if (logoCanvasGroup)
        {
            logoCanvasGroup.blocksRaycasts = true;
            logoCanvasGroup.interactable = true;
            var fade = StartCoroutine(FadeCanvasGroup(logoCanvasGroup, 0f, 1f, fadeDuration));
            float start = Time.realtimeSinceStartup;
            yield return fade;
            fadeInTime = Time.realtimeSinceStartup - start;
        }

        // Wait remaining intro time, if any
        float remaining = logoDuration - fadeInTime;
        if (remaining > 0f)
            yield return new WaitForSecondsRealtime(remaining);

        // Save player pose
        if (playerTransform) LabReturnState.SavePlayerPose(playerTransform);

        // Wait until scene is ready
        while (op.progress < 0.9f)
            yield return null;

        // Activate immediately (skip fade-out to avoid extra delay)
        op.allowSceneActivation = true;
        while (!op.isDone)
            yield return null;

        // Optional: hide logo after switch
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