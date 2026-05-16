using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PasswordPanicGameManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Leave null — auto-found via GetComponent at runtime.")]
    [SerializeField] private PasswordStrengthChecker passwordChecker;

    [Tooltip("Drag the Canvas GameObject here if you want it destroyed on scene exit.")]
    [SerializeField] private GameObject passwordUiCanvas;

    [Header("Next Scene")]
    [SerializeField] private int nextSceneIndex = 17;

    [Header("Debug")]
    public bool resetProgress = false;

    private bool isCompleting  = false;
    private const int TOTAL_AVATARS = 5;

    public static PasswordPanicGameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (passwordChecker == null)
            passwordChecker = GetComponent<PasswordStrengthChecker>();

        if (passwordChecker == null)
            passwordChecker = FindFirstObjectByType<PasswordStrengthChecker>(FindObjectsInactive.Include);

        if (passwordChecker == null)
            Debug.LogError("❌ PasswordStrengthChecker not found on GameManager!");
        else
            Debug.Log("✅ PasswordStrengthChecker found.");
    }

    private void Start() => ResetProgress();

    private void Update()
    {
        if (resetProgress)
        {
            ResetProgress();
            resetProgress = false;
        }

        if (!isCompleting && CheckAllPasswordsSet())
            StartCoroutine(HandleGameCompletion());
    }

    private bool CheckAllPasswordsSet()
    {
        if (passwordChecker == null) return false;
        int count = 0;
        for (int i = 0; i < TOTAL_AVATARS; i++)
            if (passwordChecker.IsPasswordSet(i)) count++;
        return count >= TOTAL_AVATARS;
    }

    private IEnumerator HandleGameCompletion()
    {
        isCompleting = true;
        Debug.Log("✅ All 5 passwords set — starting scene transition.");

        // ANDROID FIX: stop ReturnToAvatarSelection coroutine before UI cleanup
        if (passwordChecker != null)
        {
            passwordChecker.StopAllCoroutines();
            Debug.Log("🛑 Stopped checker coroutines.");
        }

        yield return null; // flush Unity callbacks
        yield return null;

        UpdatePlayerProgress();

        // Synchronous load for debugging - replace with async if needed
        Debug.Log($"🔄 Loading scene {nextSceneIndex} synchronously...");
        SceneManager.LoadScene(nextSceneIndex);
        Debug.Log("🎉 Scene transition complete.");
    }

    private void UpdatePlayerProgress()
    {
        if (ProgressManager.Instance != null)
        {
            for (int i = 0; i <= 3; i++)
                ProgressManager.Instance.SetChapterCompletedOnly(i, true);
            for (int i = 0; i <= 4; i++)
                ProgressManager.Instance.SetChapterUnlocked(i, true);
            Debug.Log("🏆 Progress saved.");
        }
        else
        {
            Debug.LogWarning("⚠️ ProgressManager.Instance is null — progress not saved.");
        }
    }

    public void ResetProgress()
    {
        if (passwordChecker != null) passwordChecker.ClearPasswords();
        isCompleting = false;
        Debug.Log("🔄 Progress reset.");
    }
}