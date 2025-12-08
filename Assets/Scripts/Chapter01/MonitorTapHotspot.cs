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

    [Header("Player")]
    [SerializeField] private Transform playerTransform; // assign your player root

    [Header("UI to hide on tap")]
    [SerializeField] private GameObject[] uiElementsToHide; // assign any UI elements you want hidden on tap

    private bool busy;

    private void Awake()
    {
        // Force logo off at scene load
        if (logoIntroCanvas) logoIntroCanvas.enabled = true; // keep canvas enabled so alpha works
        if (logoCanvasGroup)
        {
            logoCanvasGroup.alpha = 0f;
            logoCanvasGroup.blocksRaycasts = false;
            logoCanvasGroup.interactable = false;
        }
        else if (logoIntroCanvas)
        {
            // fallback: disable canvas if no CanvasGroup provided
            logoIntroCanvas.enabled = false;
        }
    }

    // Hook this to the UI Button's OnClick()
    public void OnTapped()
    {
        if (busy) return;

        // Hide any extra UI elements when tapped
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

        // Show logo and keep it visible
        if (logoIntroCanvas) logoIntroCanvas.enabled = true;
        if (logoCanvasGroup)
        {
            logoCanvasGroup.alpha = 1f;
            logoCanvasGroup.blocksRaycasts = true;
            logoCanvasGroup.interactable = true;
        }

        // Wait in realtime for the intro duration
        yield return new WaitForSecondsRealtime(logoDuration);

        // Save player pose
        if (playerTransform) LabReturnState.SavePlayerPose(playerTransform);

        // Load next scene asynchronously; keep logo visible until load completes
        AsyncOperation op = SceneManager.LoadSceneAsync(mazeSceneName);
        op.allowSceneActivation = true; // default; still we wait for completion
        while (!op.isDone)
        {
            yield return null;
        }
    }
}