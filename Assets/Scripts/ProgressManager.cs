using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    // Total chapters
    public const int ChapterCount = 8;

    // Progress data
    public bool[] chapterCompleted = new bool[ChapterCount];
    public bool[] chapterUnlocked  = new bool[ChapterCount];

    private const string SaveKeyCompleted = "ChapterCompleted";
    private const string SaveKeyUnlocked  = "ChapterUnlocked";

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeDefaultUnlocks();
        LoadProgress();
    }

    private void InitializeDefaultUnlocks()
    {
        // Only chapter 0 unlocked at the beginning
        for (int i = 0; i < ChapterCount; i++)
        {
            chapterUnlocked[i]  = (i == 0);
            chapterCompleted[i] = false;
        }
    }

    #region Public API

    public bool IsChapterUnlocked(int chapterIndex)
    {
        if (!IsValidChapter(chapterIndex))
            return false;

        return chapterUnlocked[chapterIndex];
    }

    public bool IsChapterCompleted(int chapterIndex)
    {
        if (!IsValidChapter(chapterIndex))
            return false;

        return chapterCompleted[chapterIndex];
    }

    /// <summary>
    /// Mark a chapter completed and unlock the next one.
    /// </summary>
    public void SetChapterCompleted(int chapterIndex, bool completed = true)
    {
        if (!IsValidChapter(chapterIndex))
        {
            Debug.LogWarning($"ProgressManager: Invalid chapter index {chapterIndex}");
            return;
        }

        chapterCompleted[chapterIndex] = completed;

        if (completed)
        {
            int nextIndex = chapterIndex + 1;
            if (IsValidChapter(nextIndex))
                chapterUnlocked[nextIndex] = true;
        }

        SaveProgress();
    }

    /// <summary>
    /// Dev / debug: reset everything to default.
    /// </summary>
    public void ResetAllProgress()
    {
        InitializeDefaultUnlocks();
        PlayerPrefs.DeleteKey(SaveKeyCompleted);
        PlayerPrefs.DeleteKey(SaveKeyUnlocked);
        PlayerPrefs.Save();
        Debug.Log("ProgressManager: All progress reset.");
    }

    #endregion

    #region Save / Load

    private void SaveProgress()
    {
        char[] completedBits = new char[ChapterCount];
        char[] unlockedBits  = new char[ChapterCount];

        for (int i = 0; i < ChapterCount; i++)
        {
            completedBits[i] = chapterCompleted[i] ? '1' : '0';
            unlockedBits[i]  = chapterUnlocked[i]  ? '1' : '0';
        }

        PlayerPrefs.SetString(SaveKeyCompleted, new string(completedBits));
        PlayerPrefs.SetString(SaveKeyUnlocked,  new string(unlockedBits));
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        if (!PlayerPrefs.HasKey(SaveKeyCompleted) || !PlayerPrefs.HasKey(SaveKeyUnlocked))
            return;

        string completedData = PlayerPrefs.GetString(SaveKeyCompleted);
        string unlockedData  = PlayerPrefs.GetString(SaveKeyUnlocked);

        int limitCompleted = Mathf.Min(completedData.Length, ChapterCount);
        int limitUnlocked  = Mathf.Min(unlockedData.Length,  ChapterCount);

        for (int i = 0; i < limitCompleted; i++)
            chapterCompleted[i] = (completedData[i] == '1');

        for (int i = 0; i < limitUnlocked; i++)
            chapterUnlocked[i] = (unlockedData[i] == '1');
    }

    #endregion

    private bool IsValidChapter(int index)
    {
        return index >= 0 && index < ChapterCount;
    }
}
