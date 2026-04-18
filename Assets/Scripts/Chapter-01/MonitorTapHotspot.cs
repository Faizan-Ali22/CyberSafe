using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MonitorTapHotspot : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string mazeSceneName = "Game01";

    [Header("This monitor's index (0 or 1)")]
    [SerializeField] private int hackedIndex = 0;

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

    public void OnTapped()
    {
        if (busy) return;

        if (hackedIndex < 0 || hackedIndex > 1)
        {
            Debug.LogWarning($"[MonitorTapHotspot] Invalid hackedIndex={hackedIndex}. Use 0 or 1.");
            return;
        }

        HackedRunState.SetLastHackedIndex(hackedIndex);
        Debug.Log($"MonitorTapHotspot: tapped hacked index {hackedIndex}", this);

        if (uiElementsToHide != null)
        {
            foreach (var go in uiElementsToHide)
                if (go) go.SetActive(false);
        }

        StartCoroutine(LoadMazeRoutine());
    }

    // optional if using button events with arg
    public void OnTappedWithIndex(int index)
    {
        hackedIndex = index;
        OnTapped();
    }

    private IEnumerator LoadMazeRoutine()
    {
        busy = true;

        AsyncOperation op = SceneManager.LoadSceneAsync(mazeSceneName);
        op.allowSceneActivation = false;

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

        float remaining = logoDuration - fadeInTime;
        if (remaining > 0f)
            yield return new WaitForSecondsRealtime(remaining);

        if (playerTransform) LabReturnState.SavePlayerPose(playerTransform);

        while (op.progress < 0.9f)
            yield return null;

        op.allowSceneActivation = true;
        while (!op.isDone)
            yield return null;

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