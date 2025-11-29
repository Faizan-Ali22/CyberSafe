using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Button))]
public class ChapterButtonLoader : MonoBehaviour
{
    [Tooltip("Scene name to load when this chapter is clicked.")]
    public string sceneName;

    [Tooltip("Show debug logs for loading progress.")]
    public bool showDebugLogs = false;

    private Michsky.UI.Shift.ChapterButton chapterButton;

    private void Awake()
    {
        chapterButton = GetComponent<Michsky.UI.Shift.ChapterButton>();

        var btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        // Only allow if unlocked
        if (ProgressManager.Instance != null && chapterButton != null)
        {
            if (!ProgressManager.Instance.IsChapterUnlocked(chapterButton.chapterIndex))
            {
                if (showDebugLogs)
                    Debug.Log("Chapter is locked, cannot load scene.");
                return;
            }
        }

        if (!string.IsNullOrEmpty(sceneName))
        {
            StartCoroutine(LoadSceneAsync());
        }
        else
        {
            Debug.LogWarning("ChapterButtonLoader: sceneName is empty.");
        }
    }

    private IEnumerator LoadSceneAsync()
    {
        if (showDebugLogs)
            Debug.Log($"Starting async load of scene: {sceneName}");

        // Start loading but don't activate immediately
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        // TODO: Show loading UI here (enable a loading panel / animation) if you want

        // Wait until loading is almost done (progress goes 0 -> 0.9)
        while (op.progress < 0.9f)
        {
            if (showDebugLogs)
                Debug.Log($"Loading progress: {op.progress}");

            // TODO: Update loading bar here using op.progress if you have one

            yield return null;
        }

        if (showDebugLogs)
            Debug.Log("Scene is ready. Waiting a short moment before activation.");

        // Small delay so you can show a final animation / fade if you like
        yield return new WaitForSeconds(0.5f);

        // TODO: Optionally play a fade-out here before activating

        // Now show the new scene
        op.allowSceneActivation = true;

        // When allowSceneActivation = true and op is ready, it will finish
        while (!op.isDone)
        {
            yield return null;
        }

        if (showDebugLogs)
            Debug.Log("Scene load completed.");
    }
}
