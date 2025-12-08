using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MazeGameManager : MonoBehaviour
{
   [Header("Shields")]
    public GameObject shieldPrefab;
    public List<Transform> shieldSpawnPoints;
    public int shieldsPerRun = 3;

    [Header("UI")]
    public GameObject popupCanvas;
    public TMPro.TextMeshProUGUI popupText;
    public string[] popupMessages = {
        "Antivirus activated!",
        "Firewall boost enabled!",
        "System cleaning in progress!"
    };

    [Header("Return")]
    public string labSceneName = "Chapter01";
    public float returnDelay = 0.5f;

    [Header("Next Chapter")]
    [Tooltip("Scene to load after all 5 hacked screens are cleared. Leave empty to always return to lab.")]
    public string nextChapterSceneName = "RedLinks";

    private int collected = 0;
    private int spawned = 0;

    private void Start()
    {
        if (!shieldPrefab) Debug.LogError("MazeGameManager: shieldPrefab not assigned.");
        if (shieldSpawnPoints == null || shieldSpawnPoints.Count == 0) Debug.LogError("MazeGameManager: no shieldSpawnPoints assigned.");
        if (!popupCanvas) Debug.LogWarning("MazeGameManager: popupCanvas not assigned (popups will not show).");
        if (!popupText) Debug.LogWarning("MazeGameManager: popupText not assigned (popups will not show text).");

        if (popupCanvas) popupCanvas.SetActive(false);

        if (shieldSpawnPoints.Count < 3)
        {
            Debug.LogError($"MazeGameManager: Need at least 3 spawn points; currently {shieldSpawnPoints.Count}.");
        }

        shieldsPerRun = Mathf.Clamp(shieldsPerRun, 1, shieldSpawnPoints.Count);

        SpawnShields();
    }

    private void SpawnShields()
    {
        collected = 0;
        spawned = 0;

        var points = new List<Transform>(shieldSpawnPoints);

        // Shuffle points
        for (int i = 0; i < points.Count; i++)
        {
            int k = Random.Range(i, points.Count);
            (points[i], points[k]) = (points[k], points[i]);
        }

        int toSpawn = Mathf.Min(shieldsPerRun, points.Count);
        for (int i = 0; i < toSpawn; i++)
        {
            Quaternion rot = points[i].rotation * Quaternion.Euler(-90f, 0f, 0f);
            var go = Instantiate(shieldPrefab, points[i].position, rot);
            var pickup = go.GetComponent<ShieldPickup>();
            if (!pickup)
            {
                Debug.LogError("Spawned shield missing ShieldPickup component.");
                continue;
            }
            pickup.manager = this;
            spawned++;
        }

        Debug.Log($"MazeGameManager: Spawned {spawned} shields, need to collect all to return.");
    }

    public void OnShieldCollected(ShieldPickup pickup)
    {
        collected++;
        Debug.Log($"MazeGameManager: Shield collected {collected}/{spawned}");

        ShowPopup(collected - 1);

        // Only when ALL spawned shields are collected, return to lab / next chapter
        if (spawned > 0 && collected >= spawned)
        {
            StartCoroutine(ReturnToLab());
        }
    }

    private void ShowPopup(int index)
    {
        if (popupCanvas == null || popupText == null) return;
        popupCanvas.SetActive(true);
        var msg = popupMessages.Length > 0 ? popupMessages[index % popupMessages.Length] : "Shield collected!";
        popupText.text = msg;
        StartCoroutine(HidePopupAfter(1.0f));
    }

    private IEnumerator HidePopupAfter(float t)
    {
        yield return new WaitForSeconds(t);
        if (popupCanvas) popupCanvas.SetActive(false);
    }

    private IEnumerator ReturnToLab()
    {
        yield return new WaitForSeconds(returnDelay);

        // Which hacked screen did we come from?
        int hackedIndex = HackedRunState.GetLastHackedIndex();
        Debug.Log($"ReturnToLab: last hacked index = {hackedIndex}");

        if (hackedIndex >= 0)
        {
            // Simple if/else – only that screen is marked cleared
            if (hackedIndex == 0)
                SessionProgress.MarkScreenCleared(0);
            else if (hackedIndex == 1)
                SessionProgress.MarkScreenCleared(1);
            else if (hackedIndex == 2)
                SessionProgress.MarkScreenCleared(2);
            else if (hackedIndex == 3)
                SessionProgress.MarkScreenCleared(3);
            else if (hackedIndex == 4)
                SessionProgress.MarkScreenCleared(4);
            else
                SessionProgress.MarkScreenCleared(hackedIndex);
        }

        // Are all 5 hacked screens done?
        bool allFiveDone = SessionProgress.HackedClearedCount >= 5;

        if (allFiveDone && ProgressManager.Instance != null)
        {
            ProgressManager.Instance.SetChapterCompleted(1, true);
        }

        // Decide which scene to load
        if (allFiveDone && !string.IsNullOrEmpty(nextChapterSceneName))
        {
            Debug.Log($"All 5 hacked screens cleared. Loading next chapter scene: {nextChapterSceneName}");
            SceneManager.LoadScene(nextChapterSceneName);
        }
        else
        {
            Debug.Log($"Returning to lab scene: {labSceneName}");
            SceneManager.LoadScene(labSceneName);
        }
    }
}
