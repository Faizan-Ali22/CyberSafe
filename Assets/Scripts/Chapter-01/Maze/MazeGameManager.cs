using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class MazeGameManager : MonoBehaviour
{
   public static MazeGameManager Instance { get; private set; }

    [Header("Shields")]
    public GameObject shieldPrefab;
    public List<Transform> shieldSpawnPoints;
    public int shieldsPerRun = 3;

    [Header("Timer")]
    public float levelTimeSeconds = 60f;
    private float timeLeft;

    [Header("PC Health")]
    public float maxHealth = 100f;
    public float healthDrainPerSecond = 2f;
    public float virusHitDamage = 20f;
    private float currentHealth;

    [Header("UI")]
    public GameObject popupCanvas;
    public TextMeshProUGUI popupText;
    public string[] popupMessages = {
        "Antivirus activated!\nThis program finds and removes malware hiding on the PC.",
        "Firewall boost enabled!\nIt blocks suspicious traffic trying to reach this computer.",
        "Security update installed!\nFixes bugs that hackers use to break into systems."
    };
    public TextMeshProUGUI shieldCountText;
    public TextMeshProUGUI timerText;
    public PCHealthUI pcHealthUI;

    [Header("Retry Popup")]
    public GameObject retryPanel;
    public TextMeshProUGUI retryReasonText;

    [Header("Return")]
    public string labSceneName = "Chapter01";
    public float returnDelay = 0.5f;

    [Header("Next Chapter")]
    [Tooltip("Scene to load after all 5 hacked screens are cleared. Leave empty to always return to lab.")]
    public string nextChapterSceneName = "RedLinks";

    [Header("Intro Popup (in Maze scene)")]
    public GameObject mazeIntroPopup;          // optional panel with a brief message
    public float mazeIntroPopupDuration = 1.0f;  // shown before gameplay starts

    private int collected = 0;
    private int spawned = 0;
    private bool isReturning = false;
    private bool isFailed = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private IEnumerator Start()
    {
        if (!shieldPrefab) Debug.LogError("MazeGameManager: shieldPrefab not assigned.");
        if (shieldSpawnPoints == null || shieldSpawnPoints.Count == 0) Debug.LogError("MazeGameManager: no shieldSpawnPoints assigned.");
        if (popupCanvas) popupCanvas.SetActive(false);
        if (retryPanel) retryPanel.SetActive(false);

        shieldsPerRun = Mathf.Clamp(shieldsPerRun, 1, shieldSpawnPoints.Count);

        SpawnShields();

        timeLeft = levelTimeSeconds;
        currentHealth = maxHealth;
        UpdateShieldUI();
        UpdateTimerUI();
        UpdateHealthUI();

        // Optional: brief intro popup in the Maze scene itself
        if (mazeIntroPopup != null)
        {
            mazeIntroPopup.SetActive(true);
            yield return new WaitForSeconds(mazeIntroPopupDuration);
            mazeIntroPopup.SetActive(false);
        }
    }

    private void Update()
    {
        if (isReturning || isFailed) return;

        // Timer
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            UpdateTimerUI();
            FailRun("Time is up!");
            return;
        }
        UpdateTimerUI();

        // Passive health drain
        currentHealth -= healthDrainPerSecond * Time.deltaTime;
        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            UpdateHealthUI();
            FailRun("PC health depleted!");
            return;
        }
        UpdateHealthUI();
    }

    private void SpawnShields()
    {
        collected = 0;
        spawned = 0;

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
        UpdateShieldUI();
    }

    public void OnShieldCollected(ShieldPickup pickup)
    {
        if (isReturning || isFailed) return;

        collected++;
        Debug.Log($"MazeGameManager: Shield collected {collected}/{spawned}");

        // Heal based on remaining shields so all will bring you to 100%
        ApplyShieldHeal();

        ShowPopup(collected - 1);

        if (spawned > 0 && collected >= spawned)
        {
            StartCoroutine(ReturnToLab());
        }

        UpdateShieldUI();
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
        isReturning = true;
        yield return new WaitForSeconds(returnDelay);

        int hackedIndex = HackedRunState.GetLastHackedIndex();
        Debug.Log($"ReturnToLab: last hacked index = {hackedIndex}");

        if (hackedIndex >= 0)
        {
            SessionProgress.MarkScreenCleared(hackedIndex);
        }

        bool allFiveDone = SessionProgress.HackedClearedCount >= 5;

        if (allFiveDone && ProgressManager.Instance != null)
        {
            ProgressManager.Instance.SetChapterCompleted(1, true);
        }

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

    public void DamagePlayer(float amount)
    {
        if (isReturning || isFailed) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(0f, currentHealth);
        UpdateHealthUI();

        if (currentHealth <= 0f)
        {
            FailRun("PC health depleted!");
        }
    }

    private void FailRun(string reason)
    {
        if (isFailed || isReturning) return;
        isFailed = true;

        Time.timeScale = 0f;
        if (retryPanel) retryPanel.SetActive(true);
        if (retryReasonText) retryReasonText.text = reason;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToLab()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(labSceneName);
    }

    private void UpdateShieldUI()
    {
        if (shieldCountText == null) return;
        int remaining = Mathf.Max(0, spawned - collected);
        shieldCountText.text = remaining > 0 ? $"{remaining} left" : "Completed!";
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = $"{Mathf.CeilToInt(timeLeft)}s";
    }

    private void UpdateHealthUI()
    {
        pcHealthUI?.SetValue(currentHealth / maxHealth, $"PC Health {Mathf.CeilToInt(currentHealth)}");
    }

    // Heal logic when collecting a shield
    private void ApplyShieldHeal()
    {
        if (spawned <= 0) return;

        // Remaining shields INCLUDING this one
        float remainingIncludingThis = Mathf.Max(1, spawned - collected + 1);

        float missing = maxHealth - currentHealth;
        float heal = missing / remainingIncludingThis;

        currentHealth = Mathf.Min(maxHealth, currentHealth + heal);
        UpdateHealthUI();
    }
}
