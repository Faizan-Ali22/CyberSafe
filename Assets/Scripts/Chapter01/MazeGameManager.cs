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
    public float returnDelay = 1.0f;

    private int collected = 0;

    private void Start()
    {
        if (!shieldPrefab) Debug.LogError("MazeGameManager: shieldPrefab not assigned.");
        if (shieldSpawnPoints == null || shieldSpawnPoints.Count == 0) Debug.LogError("MazeGameManager: no shieldSpawnPoints assigned.");
        if (!popupCanvas) Debug.LogWarning("MazeGameManager: popupCanvas not assigned (popups will not show).");
        if (!popupText) Debug.LogWarning("MazeGameManager: popupText not assigned (popups will not show text).");

        // Force popup UI off at start
        if (popupCanvas) popupCanvas.SetActive(false);

        SpawnShields();
    }

    private void SpawnShields()
    {
        collected = 0;
        var points = new List<Transform>(shieldSpawnPoints);

        // Shuffle
        for (int i = 0; i < points.Count; i++)
        {
            int k = Random.Range(i, points.Count);
            (points[i], points[k]) = (points[k], points[i]);
        }

        int toSpawn = Mathf.Min(shieldsPerRun, points.Count);
        for (int i = 0; i < toSpawn; i++)
        {
            var go = Instantiate(shieldPrefab, points[i].position, points[i].rotation);
            var pickup = go.GetComponent<ShieldPickup>();
            if (!pickup)
            {
                Debug.LogError("Spawned shield missing ShieldPickup component.");
                continue;
            }
            pickup.manager = this;
        }
    }

    public void OnShieldCollected(ShieldPickup pickup)
    {
        collected++;
        ShowPopup(collected - 1);

        if (collected >= shieldsPerRun)
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
        StartCoroutine(HidePopupAfter(1.5f));
    }

    private IEnumerator HidePopupAfter(float t)
    {
        yield return new WaitForSeconds(t);
        if (popupCanvas) popupCanvas.SetActive(false);
    }

    private IEnumerator ReturnToLab()
    {
        yield return new WaitForSeconds(returnDelay);

        SessionProgress.AddCleared(1);

        if (SessionProgress.HackedClearedCount >= 5 && ProgressManager.Instance != null)
        {
            ProgressManager.Instance.SetChapterCompleted(1, true);
        }

        SceneManager.LoadScene(labSceneName);
    }
}
