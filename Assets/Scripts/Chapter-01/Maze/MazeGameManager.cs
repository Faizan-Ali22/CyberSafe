using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Manages the Malware Maze minigame including shield collection, health, and timer.
/// Uses Singleton pattern for scene-scoped single instance.
/// Uses Object Pool pattern to pre-warm shields and prevent runtime lag.
/// </summary>
public class MazeGameManager : Singleton<MazeGameManager>
{
    [Header("Shields")]
    public GameObject shieldPrefab;
    public List<Transform> shieldSpawnPoints;
    public int shieldsPerRun = 3;

    [Header("Object Pool Settings")]
    [Tooltip("Enable object pooling for shields (recommended for performance)")]
    public bool useObjectPool = true;
    [Tooltip("Number of shields to pre-instantiate in the pool")]
    public int poolInitialSize = 10;

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

    // Object pool for shields
    private ShieldPool _shieldPool;
    private List<PoolableShield> _activeShields = new List<PoolableShield>();

    private IEnumerator Start()
    {
        if (!shieldPrefab) Debug.LogError("MazeGameManager: shieldPrefab not assigned.");
        if (shieldSpawnPoints == null || shieldSpawnPoints.Count == 0) Debug.LogError("MazeGameManager: no shieldSpawnPoints assigned.");
        if (popupCanvas) popupCanvas.SetActive(false);
        if (retryPanel) retryPanel.SetActive(false);

        shieldsPerRun = Mathf.Clamp(shieldsPerRun, 1, shieldSpawnPoints.Count);

        // Initialize object pool if enabled
        if (useObjectPool && shieldPrefab != null)
        {
            yield return StartCoroutine(InitializeShieldPool());
        }

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

    /// <summary>
    /// Initializes and pre-warms the shield object pool.
    /// </summary>
    private IEnumerator InitializeShieldPool()
    {
        // Create the pool container
        var poolContainer = new GameObject("ShieldPool");
        poolContainer.transform.SetParent(transform);
        _shieldPool = poolContainer.AddComponent<ShieldPool>();
        _shieldPool.Initialize(shieldPrefab);

        // Pre-warm the pool asynchronously
        yield return StartCoroutine(_shieldPool.PreWarmAsync());
        Debug.Log("MazeGameManager: Shield pool pre-warmed and ready.");
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
            
            if (useObjectPool && _shieldPool != null)
            {
                // Use object pool for better performance
                var poolableShield = _shieldPool.GetShield(points[i].position, rot);
                if (poolableShield != null)
                {
                    poolableShield.manager = this;
                    _activeShields.Add(poolableShield);
                    spawned++;
                }
                else
                {
                    Debug.LogError("MazeGameManager: Failed to get shield from pool.");
                }
            }
            else
            {
                // Fallback to direct instantiation (backward compatibility)
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
        }

        Debug.Log($"MazeGameManager: Spawned {spawned} shields, need to collect all to return.");
        UpdateShieldUI();
    }

    /// <summary>
    /// Called when a poolable shield is collected.
    /// Returns the shield to the pool instead of destroying it.
    /// </summary>
    /// <param name="shield">The collected shield.</param>
    public void OnPoolableShieldCollected(PoolableShield shield)
    {
        if (isReturning || isFailed) return;

        collected++;
        Debug.Log($"MazeGameManager: Shield collected {collected}/{spawned}");

        // Return shield to pool instead of destroying
        _activeShields.Remove(shield);
        _shieldPool?.ReturnShield(shield);

        // Heal based on remaining shields so all will bring you to 100%
        ApplyShieldHeal();

        ShowPopup(collected - 1);

        if (spawned > 0 && collected >= spawned)
        {
            StartCoroutine(ReturnToLab());
        }

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
