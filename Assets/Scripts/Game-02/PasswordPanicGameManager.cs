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

    private void Awake()
    {
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

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneIndex);
        if (asyncLoad == null)
        {
            Debug.LogError($"❌ Scene index {nextSceneIndex} not in Build Settings!");
            isCompleting = false;
            yield break;
        }

        asyncLoad.allowSceneActivation = false;

        // Safe to clean up UI now that async load is in-flight
        if (passwordChecker != null)
            passwordChecker.gameObject.SetActive(false);

        if (passwordUiCanvas != null)
            Destroy(passwordUiCanvas);

        while (asyncLoad.progress < 0.9f)
        {
            Debug.Log($"⏳ Loading: {asyncLoad.progress:P0}");
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;
        while (!asyncLoad.isDone) yield return null;

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